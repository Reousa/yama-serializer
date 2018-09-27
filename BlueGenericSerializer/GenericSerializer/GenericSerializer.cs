using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Blue.GenericSerializer
{
	public class GenericSerializer
	{
		public static bool IsInitialized = false;

		/// <summary>
		/// Generates all the Serialize/Deserialize methods for classes marked with [Blue.NetworkSerializable]
		/// </summary>
		/// <returns></returns>
		public static bool Initialize()
		{
			var serializableTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
			.Where(x => x.IsDefined(typeof(GenericSerializable))).ToArray();

			foreach (var sType in serializableTypes)
			{
				//Can this use better naming?
				FieldInfo serializeDelegate = sType.GetField("serialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				FieldInfo deserializeDelegate = sType.GetField("deserialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

				if (serializeDelegate == null || deserializeDelegate == null)
					throw new NotImplementedException(String.Format("Class `{0}` does not implement one of the Serialize or Deserialize delegates.", sType));

				var writerType = serializeDelegate.FieldType.GenericTypeArguments.First();
				var readerType = deserializeDelegate.FieldType.GenericTypeArguments.First();

				MethodInfo createSerialize = typeof(Blue.GenericSerializer.GenericSerializer).GetMethod("CreateSerialize", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(writerType, sType);
				MethodInfo createDeserialize = typeof(Blue.GenericSerializer.GenericSerializer).GetMethod("CreateDeserialize", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(readerType, sType);

				serializeDelegate.SetValue(null, createSerialize.Invoke(null, null));
				deserializeDelegate.SetValue(null, createDeserialize.Invoke(null, null));
			}

			IsInitialized = true;
			return IsInitialized;
		}

		/// <summary>
		/// Generates the serialization IL for a specific class type.
		/// </summary>
		/// <typeparam name="TWriter">Writer class (ex: BinaryWriter)</typeparam>
		/// <typeparam name="TObject">Type of class to create the `Serialize` delegate for.</typeparam>
		/// <returns>Returns a delegate with the IL generated for the Serialize method</returns>
		static Action<TWriter, TObject> CreateSerialize<TWriter, TObject>()
		{
			ParameterExpression instance = Expression.Parameter(typeof(TObject), "instance");
			ParameterExpression writer = Expression.Parameter(typeof(TWriter), "writer");
			List<Expression> calls = new List<Expression>();

			var objectType = typeof(TObject);

			var fields = GetFields(objectType);
			var props = GetProperties(objectType);

			foreach (var field in fields)
			{
				calls.Add(
					GenerateWriteCalls(field, writer, instance));
			}

			foreach (var prop in props)
			{
				calls.Add(
					GenerateWriteCalls(prop, writer, instance));
			}

			Expression block = Expression.Block(
				calls
			);

			Expression<Action<TWriter, TObject>> lambda = Expression.Lambda<Action<TWriter, TObject>>(block, writer, instance);
			return lambda.Compile();
		}

		/// <summary>
		/// Generates the deserialization IL for a specific class type.
		/// </summary>
		/// <typeparam name="TWriter">Reader class (ex: BinaryReader)</typeparam>
		/// <typeparam name="TObject">Type of class to create the `Deserialize` delegate for.</typeparam>
		/// <returns>Returns a delegate with the IL generated for Deserialization</returns>
		static Action<TReader, TObject> CreateDeserialize<TReader, TObject>()
		{
			ParameterExpression instance = Expression.Parameter(typeof(TObject), "instance");
			ParameterExpression reader = Expression.Parameter(typeof(TReader), "writer");
			List<Expression> calls = new List<Expression>();

			var objectType = typeof(TObject);

			var fields = GetFields(objectType);
			var props = GetProperties(objectType);

			foreach (var field in fields)
				calls.Add(
						GenerateReadCalls(field, reader, instance));

			foreach (var prop in props)
				calls.Add(
					GenerateReadCalls(prop, reader, instance));

			Expression block = Expression.Block(
				calls
			);

			Expression<Action<TReader, TObject>> lambda = Expression.Lambda<Action<TReader, TObject>>(block, reader, instance);
			return lambda.Compile();
		}

		static FieldInfo[] GetFields(Type objectType)
		{
			GenericSerializable gsAttribute = (GenericSerializable)objectType.GetCustomAttribute(typeof(GenericSerializable));
			//If this is null, it means the class holding a variable of this type is marked as "SerializeAllPublic"
			//Thus; serialize all publics in the type's member aswell.
			if (gsAttribute == null)
				return objectType.GetFields()
					.OrderBy(f => f.Name).ToArray();

			else if (gsAttribute.FilterByNames)
				return objectType.GetFields(gsAttribute.Flags)
					.Where(f => gsAttribute.MemberNames.Contains(f.Name))
					.OrderBy(f => f.Name).ToArray();

			else if (!gsAttribute.DefaultOptions)
				return objectType.GetFields(gsAttribute.Flags)
					.OrderBy(f => f.Name).ToArray();

			else
				return objectType.GetFields()
					.Where(f => f.IsDefined(typeof(GenericSerializable)))
					.OrderBy(f => f.Name).ToArray();
		}

		static FieldInfo[] GetFields(FieldInfo field)
		{
			GenericSerializable gsAttribute = (GenericSerializable)field.GetCustomAttribute(typeof(GenericSerializable));
			//If this is null, it means the class holding a variable of this type is marked as "SerializeAllPublic"
			//Thus; serialize all publics in the type's member aswell.
			if (gsAttribute == null)
				return field.FieldType.GetFields()
					.OrderBy(f => f.Name).ToArray();

			else if (gsAttribute.FilterByNames)
				return field.FieldType.GetFields(gsAttribute.Flags)
					.Where(f => gsAttribute.MemberNames.Contains(f.Name))
					.OrderBy(f => f.Name).ToArray();

			else if (!gsAttribute.DefaultOptions)
				return field.FieldType.GetFields(gsAttribute.Flags)
					.OrderBy(f => f.Name).ToArray();

			else
				return field.FieldType.GetFields()
					.Where(f => f.IsDefined(typeof(GenericSerializable)))
					.OrderBy(f => f.Name).ToArray();
		}

		static FieldInfo[] GetFields(PropertyInfo prop)
		{
			GenericSerializable gsAttribute = (GenericSerializable)prop.GetCustomAttribute(typeof(GenericSerializable));
			//If this is null, it means the class holding a variable of this type is marked as "SerializeAllPublic"
			//Thus; serialize all publics in the type's member aswell.
			if (gsAttribute == null)
				return prop.PropertyType.GetFields()
					.OrderBy(f => f.Name).ToArray();

			else if (gsAttribute.FilterByNames)
				return prop.PropertyType.GetFields(gsAttribute.Flags)
					.Where(f => gsAttribute.MemberNames.Contains(f.Name))
					.OrderBy(f => f.Name).ToArray();

			else if (!gsAttribute.DefaultOptions)
				return prop.PropertyType.GetFields(gsAttribute.Flags)
					.OrderBy(f => f.Name).ToArray();

			else
				return prop.PropertyType.GetFields()
					.Where(f => f.IsDefined(typeof(GenericSerializable)))
					.OrderBy(f => f.Name).ToArray();
		}

		static PropertyInfo[] GetProperties(Type objectType)
		{
			GenericSerializable gsAttribute = (GenericSerializable)objectType.GetCustomAttribute(typeof(GenericSerializable));
			//If this is null, it means the class holding a variable of this type is marked as "SerializeAllPublic"
			//Thus; serialize all publics in the type's member aswell.
			if (gsAttribute == null)
				return objectType.GetProperties()
					.OrderBy(p => p.Name).ToArray();

			else if (gsAttribute.FilterByNames)
				return objectType.GetProperties(gsAttribute.Flags)
					.Where(p => gsAttribute.MemberNames.Contains(p.Name))
					.OrderBy(p => p.Name).ToArray();

			else if (!gsAttribute.DefaultOptions)
				return objectType.GetProperties(gsAttribute.Flags)
					.OrderBy(p => p.Name).ToArray();

			else
				return objectType.GetProperties()
					.Where(p => p.IsDefined(typeof(GenericSerializable)))
					.OrderBy(p => p.Name).ToArray();
		}

		static PropertyInfo[] GetProperties(FieldInfo field)
		{
			GenericSerializable gsAttribute = (GenericSerializable)field.GetCustomAttribute(typeof(GenericSerializable));
			//If this is null, it means the class holding a variable of this type is marked as "SerializeAllPublic"
			//Thus; serialize all publics in the type's member aswell.
			if (gsAttribute == null)
				return field.FieldType.GetProperties()
					.OrderBy(p => p.Name).ToArray();

			else if (gsAttribute.FilterByNames)
				return field.FieldType.GetProperties(gsAttribute.Flags)
					.Where(p => gsAttribute.MemberNames.Contains(p.Name))
					.OrderBy(p => p.Name).ToArray();

			else if (!gsAttribute.DefaultOptions)
				return field.FieldType.GetProperties(gsAttribute.Flags)
					.OrderBy(p => p.Name).ToArray();

			else
				return field.FieldType.GetProperties().
					Where(p => p.IsDefined(typeof(GenericSerializable))).
					OrderBy(p => p.Name).ToArray();
		}

		static PropertyInfo[] GetProperties(PropertyInfo prop)
		{
			GenericSerializable gsAttribute = (GenericSerializable)prop.GetCustomAttribute(typeof(GenericSerializable));
			//If this is null, it means the class holding a variable of this type is marked as "SerializeAllPublic"
			//Thus; serialize all publics in the type's member aswell.
			if (gsAttribute == null)
				return prop.PropertyType.GetProperties()
					.OrderBy(p => p.Name).ToArray();

			else if (gsAttribute.FilterByNames)
				return prop.PropertyType.GetProperties(gsAttribute.Flags)
					.Where(p => gsAttribute.MemberNames.Contains(p.Name))
					.OrderBy(p => p.Name).ToArray();

			else if (!gsAttribute.DefaultOptions)
				return prop.PropertyType.GetProperties(gsAttribute.Flags)
					.OrderBy(p => p.Name).ToArray();

			else
				return prop.PropertyType.GetProperties().
					Where(p => p.IsDefined(typeof(GenericSerializable))).
					OrderBy(p => p.Name).ToArray();
		}

		/// <summary>
		/// Generates write calls for different data types. It is recursive & is dependent "GetProperties" & "GetFields"
		/// </summary>
		/// <param name="field">The field we're writing to the stream</param>
		/// <param name="writer">Writer instance</param>
		/// <param name="instance">Type instance - This can be an instance of a type inside another type being serialized</param>
		/// <returns></returns>
		static Expression GenerateWriteCalls(FieldInfo field, Expression writer, Expression instance)
		{
			if (field.FieldType.IsPrimitive || field.FieldType == typeof(decimal) || field.FieldType == typeof(string))
			{
				return Expression.Call(writer, "Write", null, Expression.Field(instance, field));
			}
			else if (field.FieldType.IsClass)
			{
				List<Expression> calls = new List<Expression>();
				foreach (var innerField in GetFields(field))
				{
					Expression fieldInstance = Expression.Field(instance, field);
					calls.Add(GenerateWriteCalls(innerField, writer, fieldInstance));
				}
				foreach (var innerProperty in GetProperties(field))
				{
					Expression fieldInstance = Expression.Field(instance, field);
					calls.Add(GenerateWriteCalls(innerProperty, writer, fieldInstance));
				}
				return Expression.Block(calls);
			}
			else
				return Expression.Empty();
		}

		static Expression GenerateWriteCalls(PropertyInfo prop, Expression writer, Expression instance)
		{
			if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(string))
			{
				return Expression.Call(writer, "Write", null, Expression.Property(instance, prop));
			}
			else if (prop.PropertyType.IsClass)
			{
				List<Expression> calls = new List<Expression>();
				foreach (var innerField in GetFields(prop))
				{
					Expression fieldInstance = Expression.Property(instance, prop);
					calls.Add(GenerateWriteCalls(innerField, writer, fieldInstance));
				}
				foreach (var innerProperty in GetProperties(prop))
				{
					Expression fieldInstance = Expression.Property(instance, prop);
					calls.Add(GenerateWriteCalls(innerProperty, writer, fieldInstance));
				}
				return Expression.Block(calls);
			}
			else
				return Expression.Empty();
		}

		/// <summary>
		/// Generates read calls for different data types. It is recursive & is dependent "GetProperties" & "GetFields"
		/// </summary>
		/// <param name="field">The field we're writing to the stream</param>
		/// <param name="reader">Reader instance</param>
		/// <param name="instance">Type instance - This can be an instance of a type member inside another type being serialized</param>
		/// <returns></returns>
		static Expression GenerateReadCalls(FieldInfo field, Expression reader, Expression instance)
		{
			if (field.FieldType.IsPrimitive || field.FieldType == typeof(decimal) || field.FieldType == typeof(string))
			{
				return
					Expression.Assign(
						Expression.Field(instance, field),
						Expression.Call(reader, "Read" + field.FieldType.Name, null, null));
			}
			else if (field.FieldType.IsClass)
			{
				List<Expression> calls = new List<Expression>();
				foreach (var innerField in GetFields(field))
				{
					Expression fieldInstance = Expression.Field(instance, field);
					calls.Add(GenerateReadCalls(innerField, reader, fieldInstance));
				}
				foreach (var innerProperty in GetProperties(field))
				{
					Expression fieldInstance = Expression.Field(instance, field);
					calls.Add(GenerateReadCalls(innerProperty, reader, fieldInstance));
				}
				return Expression.Block(calls);
			}

			else
				return Expression.Empty();
		}

		/// <summary>
		/// Generates read calls for different data types. It is recursive & is dependent "GetProperties" & "GetFields"
		/// </summary>
		/// <param name="field">The field we're writing to the stream</param>
		/// <param name="reader">Reader instance</param>
		/// <param name="instance">Type instance - This can be an instance of a type member inside another type being serialized</param>
		/// <returns></returns>
		static Expression GenerateReadCalls(PropertyInfo prop, Expression reader, Expression instance)
		{
			if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(string))
			{
				return
					Expression.Assign(
						Expression.Property(instance, prop),
						Expression.Call(reader, "Read" + prop.PropertyType.Name, null, null));
			}
			else if (prop.PropertyType.IsClass)
			{
				List<Expression> calls = new List<Expression>();
				foreach (var innerField in GetFields(prop))
				{
					Expression fieldInstance = Expression.Property(instance, prop);
					calls.Add(GenerateReadCalls(innerField, reader, fieldInstance));
				}
				foreach (var innerProperty in GetProperties(prop))
				{
					Expression fieldInstance = Expression.Property(instance, prop);
					calls.Add(GenerateReadCalls(innerProperty, reader, fieldInstance));
				}
				return Expression.Block(calls);
			}

			else
				return Expression.Empty();
		}
	}
}
