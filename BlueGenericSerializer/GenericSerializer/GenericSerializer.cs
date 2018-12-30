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
		static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

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
				Log.Info("Creating serialization methods for: " + sType.Name);
				//Can this use better naming?
				FieldInfo serializeDelegate = sType.GetField("serialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				FieldInfo deserializeDelegate = sType.GetField("deserialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

				if (serializeDelegate == null || deserializeDelegate == null)
					throw new NotImplementedException(String.Format("Type `{0}` does not implement one of the Serialize or Deserialize delegates.", sType));

				var writerType = serializeDelegate.FieldType.GenericTypeArguments.First();
				var readerType = deserializeDelegate.FieldType.GenericTypeArguments.First();

				MethodInfo createSerialize = typeof(Blue.GenericSerializer.GenericSerializer).GetMethod("CreateSerialize", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(writerType, sType);
				MethodInfo createDeserialize = typeof(Blue.GenericSerializer.GenericSerializer).GetMethod("CreateDeserialize", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(readerType, sType);

				serializeDelegate.SetValue(null, createSerialize.Invoke(null, null));
				deserializeDelegate.SetValue(null, createDeserialize.Invoke(null, null));

				Log.Info("Finished: " + sType.Name);
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

			var fields = ExpressionBuilder.GetFields(objectType);
			var props = ExpressionBuilder.GetProperties(objectType);

			foreach (var field in fields)
			{
				calls.Add(
					ExpressionBuilder.GenerateWriteCalls(field, writer, instance));
			}

			foreach (var prop in props)
			{
				calls.Add(
					ExpressionBuilder.GenerateWriteCalls(prop, writer, instance));
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

			var fields = ExpressionBuilder.GetFields(objectType);
			var props = ExpressionBuilder.GetProperties(objectType);

			foreach (var field in fields)
				calls.Add(
						ExpressionBuilder.GenerateReadCalls(field, reader, instance));

			foreach (var prop in props)
				calls.Add(
					ExpressionBuilder.GenerateReadCalls(prop, reader, instance));

			Expression block = Expression.Block(
				calls
			);

			Expression<Action<TReader, TObject>> lambda = Expression.Lambda<Action<TReader, TObject>>(block, reader, instance);
			return lambda.Compile();
		}


	}
}
