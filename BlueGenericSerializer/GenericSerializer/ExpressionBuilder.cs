using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Blue.GenericSerializer
{
	public static class ExpressionBuilder
	{
		public static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

		public static FieldInfo[] GetFields(Type objectType, GenericSerializable gsAttribute = null)
		{
			Log.Info("Getting fields for type: " + objectType.Name);

			if (gsAttribute == null)
				gsAttribute = (GenericSerializable)objectType.GetCustomAttribute(typeof(GenericSerializable));
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

		public static FieldInfo[] GetFields(FieldInfo field, GenericSerializable gsAttribute = null)
		{
			Log.Info("Getting fields for field: " + field.Name);
			if (gsAttribute == null)
				gsAttribute = (GenericSerializable)field.GetCustomAttribute(typeof(GenericSerializable));
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

		public static FieldInfo[] GetFields(PropertyInfo prop, GenericSerializable gsAttribute = null)
		{
			Log.Info("Getting fields for property: " + prop.Name);

			if (gsAttribute == null)
				gsAttribute = (GenericSerializable)prop.GetCustomAttribute(typeof(GenericSerializable));
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

		public static PropertyInfo[] GetProperties(Type objectType, GenericSerializable gsAttribute = null)
		{
			Log.Info("Getting Properties for type: " + objectType.Name);

			if (gsAttribute == null)
				gsAttribute = (GenericSerializable)objectType.GetCustomAttribute(typeof(GenericSerializable));
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

		public static PropertyInfo[] GetProperties(FieldInfo field, GenericSerializable gsAttribute = null)
		{
			Log.Info("Getting properties for field: " + field.Name);

			if (gsAttribute == null)
				gsAttribute = (GenericSerializable)field.GetCustomAttribute(typeof(GenericSerializable));
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

		public static PropertyInfo[] GetProperties(PropertyInfo prop, GenericSerializable gsAttribute = null)
		{
			Log.Info("Getting properties for property: " + prop.Name);

			if (gsAttribute == null)
				gsAttribute = (GenericSerializable)prop.GetCustomAttribute(typeof(GenericSerializable));
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
		public static Expression GenerateWriteCalls(FieldInfo field, Expression writer, Expression instance)
		{
			Log.Info("Generating write calls for field: {0}.{1}", instance.ToString(), field.Name);

			var type = field.FieldType;
			if (type.IsPrimitive || type == typeof(decimal) || type == typeof(string))
			{
				return Expression.Call(writer, "Write", null, Expression.Field(instance, field));
			}
			else if (type.IsArray && type.GetElementType() != null)
			{
				return WriteArray(field, writer, instance);
			}
			else if (type.GetInterfaces().Contains(typeof(System.Collections.IList)) && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
			{
				return WriteList(field, writer, instance);
			}
			else if (type.IsClass && !type.IsArray && !type.IsEnum)
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

		public static Expression GenerateWriteCalls(PropertyInfo prop, Expression writer, Expression instance)
		{
			Log.Info("Generating write calls for property: {0}.{1}", instance, prop.Name);

			var type = prop.PropertyType;
			if (type.IsPrimitive || type == typeof(decimal) || type == typeof(string))
			{
				return Expression.Call(writer, "Write", null, Expression.Property(instance, prop));
			}
			else if (type.IsArray && type.GetElementType() != null)
			{
				return WriteArray(prop, writer, instance);
			}
			else if (type.GetInterfaces().Contains(typeof(System.Collections.IList)) && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
			{
				return WriteList(prop, writer, instance);
			}
			else if (type.IsClass && !type.IsArray && !type.IsEnum)
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
		public static Expression GenerateReadCalls(FieldInfo field, Expression reader, Expression instance)
		{
			Log.Info("Generating read calls for field: {0}.{1}", instance, field.Name);

			var type = field.FieldType;
			if (type.IsPrimitive || type == typeof(decimal) || type == typeof(string))
			{
				return
					Expression.Assign(
						Expression.Field(instance, field),
						Expression.Call(reader, "Read" + type.Name, null, null));
			}
			else if (type.IsArray && type.GetElementType() != null)
			{
				return ReadArray(field, reader, instance);
			}
			else if (type.GetInterfaces().Contains(typeof(System.Collections.IList)) && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
			{
				return ReadList(field, reader, instance);
			}
			else if (type.IsClass && !type.IsArray && !type.IsEnum)
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

		public static Expression GenerateReadCalls(PropertyInfo prop, Expression reader, Expression instance)
		{
			Log.Info("Generating read calls for property: {0}.{1}", instance, prop.Name);

			var type = prop.PropertyType;
			if (type.IsPrimitive || type == typeof(decimal) || type == typeof(string))
			{
				return
					Expression.Assign(
						Expression.Property(instance, prop),
						Expression.Call(reader, "Read" + type.Name, null, null));
			}
			else if (type.IsArray && type.GetElementType() != null)
			{
				return ReadArray(prop, reader, instance);
			}
			else if (type.GetInterfaces().Contains(typeof(System.Collections.IList)) && type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
			{
				return ReadList(prop, reader, instance);
			}
			else if (type.IsClass && !type.IsArray && !type.IsEnum)
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

		#region ARRAYS

		/// <summary>
		/// Generates Array writing code.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="writer"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static Expression WriteArray(FieldInfo array, Expression writer, Expression instance)
		{
			var itemType = array.FieldType.GetElementType();
			if (itemType.IsPrimitive || itemType == typeof(decimal) || itemType == typeof(string))
				return WriteArrayPrimitive(array, writer, instance);

			else if (itemType.IsClass && !itemType.IsArray && !itemType.IsEnum)
				return WriteArrayClass(array, writer, instance);

			else
				return Expression.Empty();
		}

		public static Expression WriteArray(PropertyInfo array, Expression writer, Expression instance)
		{
			var itemType = array.PropertyType.GetElementType();
			if (itemType.IsPrimitive || itemType == typeof(decimal) || itemType == typeof(string))
				return WriteArrayPrimitive(array, writer, instance);

			else if (itemType.IsClass && !itemType.IsArray && !itemType.IsEnum)
				return WriteArrayClass(array, writer, instance);

			else
				return Expression.Empty();
		}

		public static Expression ReadArray(FieldInfo array, Expression reader, Expression instance)
		{
			var itemType = array.FieldType.GetElementType();
			if (itemType.IsPrimitive || itemType == typeof(decimal) || itemType == typeof(string))
				return ReadArrayPrimitive(array, reader, instance);

			else if (itemType.IsClass && !itemType.IsArray && !itemType.IsEnum)
				return ReadArrayClass(array, reader, instance);

			else
				return Expression.Empty();
		}

		public static Expression ReadArray(PropertyInfo array, Expression reader, Expression instance)
		{
			var itemType = array.PropertyType.GetElementType();
			if (itemType.IsPrimitive || itemType == typeof(decimal) || itemType == typeof(string))
				return ReadArrayPrimitive(array, reader, instance);

			else if (itemType.IsClass && !itemType.IsArray && !itemType.IsEnum)
				return ReadArrayClass(array, reader, instance);

			else
				return Expression.Empty();
		}

		private static Expression WriteArrayPrimitive(FieldInfo array, Expression writer, Expression instance)
		{
			var arr = Expression.Field(instance, array);
			var counter1 = Expression.Variable(typeof(int), "counter1");
			var counter2 = Expression.Variable(typeof(int), "counter2");

			var breakLabel = Expression.Label("breakLabel");
			var breakLabelmd = Expression.Label("breakLabelmd");

			Expression.Assign(counter1, Expression.Constant(0));
			Expression.Assign(counter2, Expression.Constant(0));

			Expression block = Expression.Empty();

			var ranks = array.FieldType.GetArrayRank();
			if (ranks > 2)
				throw new NotImplementedException("2D Arrays or less are currently implemented.");

			else if (ranks == 2)
			{
				var length1 = Expression.Call(Expression.Field(instance, array), "GetLength", null, Expression.Constant(0));
				var length2 = Expression.Call(Expression.Field(instance, array), "GetLength", null, Expression.Constant(1));
				block = Expression.Block(
					new[] { counter1, counter2, },
					Expression.Loop(
						Expression.IfThenElse(
							Expression.LessThan(counter1, length1),
								Expression.Loop(
									Expression.IfThenElse(
										Expression.LessThan(counter2, length2),
										Expression.Block(
											Expression.Call(writer, "Write", null, Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 })),
											Expression.PostIncrementAssign(counter2)
											),
										Expression.Block(
											Expression.PostIncrementAssign(counter1),
											Expression.Assign(counter2, Expression.Constant(0)),
											Expression.Break(breakLabel)
											)
										),
									breakLabel
									),
							Expression.Break(breakLabelmd)
							),
						breakLabelmd
						)
					);
			}
			else if (ranks == 1)
			{
				var length1 = Expression.Call(Expression.Field(instance, array), "GetLength", null, Expression.Constant(0));

				block = Expression.Block(
					new[] { counter1 },
					Expression.Call(writer, "Write", null, length1),
					Expression.Loop(
						Expression.IfThenElse(
							Expression.LessThan(counter1, Expression.ArrayLength(arr)),
							Expression.Block(
								Expression.Call(writer, "Write", null, Expression.ArrayAccess(arr, counter1)),
								Expression.PostIncrementAssign(counter1)
							),
							Expression.Break(breakLabel)
						),
						breakLabel
					)
				);
			}
			return block;
		}

		private static Expression WriteArrayPrimitive(PropertyInfo array, Expression writer, Expression instance)
		{
			var arr = Expression.Property(instance, array);
			var counter1 = Expression.Variable(typeof(int), "counter1");
			var counter2 = Expression.Variable(typeof(int), "counter2");

			var breakLabel = Expression.Label("breakLabel");
			var breakLabelmd = Expression.Label("breakLabelmd");

			Expression.Assign(counter1, Expression.Constant(0));
			Expression.Assign(counter2, Expression.Constant(0));

			Expression block = Expression.Empty();

			var ranks = array.PropertyType.GetArrayRank();

			if (ranks > 2)
				throw new NotImplementedException("2D Arrays or less are currently implemented.");

			else if (ranks == 2)
			{
				var length1 = Expression.Call(Expression.Property(instance, array), "GetLength", null, Expression.Constant(0));
				var length2 = Expression.Call(Expression.Property(instance, array), "GetLength", null, Expression.Constant(1));
				block = Expression.Block(
					new[] { counter1, counter2, },
					Expression.Loop(
						Expression.IfThenElse(
							Expression.LessThan(counter1, length1),
								Expression.Loop(
									Expression.IfThenElse(
										Expression.LessThan(counter2, length2),
										Expression.Block(
											Expression.Call(writer, "Write", null, Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 })),
											Expression.PostIncrementAssign(counter2)
											),
										Expression.Block(
											Expression.PostIncrementAssign(counter1),
											Expression.Assign(counter2, Expression.Constant(0)),
											Expression.Break(breakLabel)
											)
										),
									breakLabel
									),
							Expression.Break(breakLabelmd)
							),
						breakLabelmd
						)
					);
			}
			else if (ranks == 1)
			{
				var length1 = Expression.Call(Expression.Property(instance, array), "GetLength", null, Expression.Constant(0));

				block = Expression.Block(
					new[] { counter1 },
					Expression.Call(writer, "Write", null, length1),
					Expression.Loop(
						Expression.IfThenElse(
							Expression.LessThan(counter1, Expression.ArrayLength(arr)),
							Expression.Block(
								Expression.Call(writer, "Write", null, Expression.ArrayAccess(arr, counter1)),
								Expression.PostIncrementAssign(counter1)
							),
							Expression.Break(breakLabel)
						),
						breakLabel
					)
				);
			}
			return block;
		}

		private static Expression ReadArrayPrimitive(FieldInfo array, Expression reader, Expression instance)
		{
			var arr = Expression.Field(instance, array);
			var counter1 = Expression.Variable(typeof(int), "counter1");
			var counter2 = Expression.Variable(typeof(int), "counter2");

			var breakLabel = Expression.Label("breakLabel");
			var breakLabelmd = Expression.Label("breakLabelmd");

			Expression.Assign(counter1, Expression.Constant(0));
			Expression.Assign(counter2, Expression.Constant(0));

			Expression block = Expression.Empty();

			var ranks = array.FieldType.GetArrayRank();
			if (ranks > 2)
				throw new NotImplementedException("2D Arrays or less are currently implemented.");

			else if (ranks == 2)
			{
				var length1 = Expression.Call(Expression.Field(instance, array), "GetLength", null, Expression.Constant(0));
				var length2 = Expression.Call(Expression.Field(instance, array), "GetLength", null, Expression.Constant(1));
				block = Expression.Block(
					new[] { counter1, counter2, },
					Expression.Loop(
						Expression.IfThenElse(
							Expression.LessThan(counter1, length1),
								Expression.Loop(
									Expression.IfThenElse(
										Expression.LessThan(counter2, length2),
										Expression.Block(
											Expression.Assign(
												Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }),
												Expression.Call(reader, "Read" + array.FieldType.GetElementType().Name, null, null)
											),
											Expression.PostIncrementAssign(counter2)
											),
										Expression.Block(
											Expression.PostIncrementAssign(counter1),
											Expression.Assign(counter2, Expression.Constant(0)),
											Expression.Break(breakLabel)
											)
										),
									breakLabel
									),
							Expression.Break(breakLabelmd)
							),
						breakLabelmd
						)
					);
			}
			else if(ranks == 1)
			{
				var readSerializedLength = Expression.Call(reader, "ReadInt32", null, null);
				block = Expression.Block(
				new[] { counter1 },
				Expression.Assign(arr, Expression.NewArrayBounds(array.FieldType.GetElementType(), new[] { readSerializedLength })),
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter1, Expression.ArrayLength(arr)),
						Expression.Block(
							Expression.Assign(
								Expression.ArrayAccess(arr, counter1),
								Expression.Call(reader, "Read" + array.FieldType.GetElementType().Name, null, null)
								),
							Expression.PostIncrementAssign(counter1)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				));
			}
			return block;
		}

		private static Expression ReadArrayPrimitive(PropertyInfo array, Expression reader, Expression instance)
		{
			var arr = Expression.Property(instance, array);
			var counter1 = Expression.Variable(typeof(int), "counter1");
			var counter2 = Expression.Variable(typeof(int), "counter2");

			var breakLabel = Expression.Label("breakLabel");
			var breakLabelmd = Expression.Label("breakLabelmd");

			Expression.Assign(counter1, Expression.Constant(0));
			Expression.Assign(counter2, Expression.Constant(0));

			Expression block = Expression.Empty();

			var ranks = array.PropertyType.GetArrayRank();
			if (ranks > 2)
				throw new NotImplementedException("2D Arrays or less are currently implemented.");

			else if (ranks == 2)
			{
				var length1 = Expression.Call(Expression.Property(instance, array), "GetLength", null, Expression.Constant(0));
				var length2 = Expression.Call(Expression.Property(instance, array), "GetLength", null, Expression.Constant(1));
				block = Expression.Block(
					new[] { counter1, counter2, },
					Expression.Loop(
						Expression.IfThenElse(
							Expression.LessThan(counter1, length1),
								Expression.Loop(
									Expression.IfThenElse(
										Expression.LessThan(counter2, length2),
										Expression.Block(
											Expression.Assign(
												Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }),
												Expression.Call(reader, "Read" + array.PropertyType.GetElementType().Name, null, null)
											),
											Expression.PostIncrementAssign(counter2)
											),
										Expression.Block(
											Expression.PostIncrementAssign(counter1),
											Expression.Assign(counter2, Expression.Constant(0)),
											Expression.Break(breakLabel)
											)
										),
									breakLabel
									),
							Expression.Break(breakLabelmd)
							),
						breakLabelmd
						)
					);
			}
			else if (ranks == 1)
			{
				var readSerializedLength = Expression.Call(reader, "ReadInt32", null, null);
				block = Expression.Block(
				new[] { counter1 },
				Expression.Assign(arr, Expression.NewArrayBounds(array.PropertyType.GetElementType(), new[] { readSerializedLength })),
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter1, Expression.ArrayLength(arr)),
						Expression.Block(
							Expression.Assign(
								Expression.ArrayAccess(arr, counter1),
								Expression.Call(reader, "Read" + array.PropertyType.GetElementType().Name, null, null)
								),
							Expression.PostIncrementAssign(counter1)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				));
			}
			return block;
		}

		//Writes 1 or 2 extra ints for array length.
		private static Expression WriteArrayClass(FieldInfo array, Expression writer, Expression instance)
		{
			var arr = Expression.Field(instance, array);
			var counter1 = Expression.Variable(typeof(int), "counter1");
			var counter2 = Expression.Variable(typeof(int), "counter2");
			var breakLabel = Expression.Label("breakLabel");
			var breakLabelmd = Expression.Label("breakLabelmd");
			Expression.Assign(counter1, Expression.Constant(0));
			Expression.Assign(counter2, Expression.Constant(0));

			var attribute = (GenericSerializable)array.GetCustomAttribute(typeof(GenericSerializable));
			var elementType = array.FieldType.GetElementType();

			var fields = GetFields(elementType, attribute);
			var props = GetProperties(elementType, attribute);

			//Generating calls for the array's data type
			List<Expression> calls = new List<Expression>();

			Expression block = Expression.Empty();

			var ranks = array.FieldType.GetArrayRank();
			if (ranks > 2)
				throw new NotImplementedException("2D Arrays or less are currently implemented.");

			else if (ranks == 2)
			{
				//If the array value is null, assign to it using default constructor.
				calls.Add(Expression.IfThen(
					Expression.Equal(
						Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }), Expression.Constant(null)),
						Expression.Assign(
							Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }),
							Expression.New(array.FieldType.GetElementType())
							)
					));

				//Simulate serializing the type of the array. i.e: Int, Vector3 etc
				calls.AddRange(fields.Select((f, index) => GenerateWriteCalls(f, writer, Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }))));
				calls.AddRange(props.Select((p, index) => GenerateWriteCalls(p, writer, Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }))));

				var length1 = Expression.Call(Expression.Field(instance, array), "GetLength", null, Expression.Constant(0));
				var length2 = Expression.Call(Expression.Field(instance, array), "GetLength", null, Expression.Constant(1));

				block = Expression.Block(
					new[] { counter1, counter2, },
				Expression.Call(writer, "Write", null, length1),
				Expression.Call(writer, "Write", null, length2),
				Expression.Loop(
						Expression.IfThenElse(
							Expression.LessThan(counter1, length1),
								Expression.Loop(
									Expression.IfThenElse(
										Expression.LessThan(counter2, length2),
										Expression.Block(
											Expression.Block(
												calls
											),
											Expression.PostIncrementAssign(counter2)
											),
										Expression.Block(
											Expression.PostIncrementAssign(counter1),
											Expression.Assign(counter2, Expression.Constant(0)),
											Expression.Break(breakLabel)
											)
										),
									breakLabel
									),
							Expression.Break(breakLabelmd)
							),
						breakLabelmd
						)
					);
			}
			else if (ranks == 1)
			{
				var length1 = Expression.Call(Expression.Field(instance, array), "GetLength", null, Expression.Constant(0));

				calls.AddRange(fields.Select((f, index) => GenerateWriteCalls(f, writer, Expression.ArrayAccess(arr, counter1))));
				calls.AddRange(props.Select((p, index) => GenerateWriteCalls(p, writer, Expression.ArrayAccess(arr, counter1))));

				block = Expression.Block(
				new[] { counter1 },
				Expression.Call(writer, "Write", null, length1),
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter1, Expression.ArrayLength(arr)),
						Expression.Block(
							Expression.Block(
								calls
							),
							Expression.PostIncrementAssign(counter1)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				));
			}
			return block;
		}

		private static Expression WriteArrayClass(PropertyInfo array, Expression writer, Expression instance)
		{
			var arr = Expression.Property(instance, array);
			var counter1 = Expression.Variable(typeof(int), "counter1");
			var counter2 = Expression.Variable(typeof(int), "counter2");
			var breakLabel = Expression.Label("breakLabel");
			var breakLabelmd = Expression.Label("breakLabelmd");
			Expression.Assign(counter1, Expression.Constant(0));
			Expression.Assign(counter2, Expression.Constant(0));

			var attribute = (GenericSerializable)array.GetCustomAttribute(typeof(GenericSerializable));
			var elementType = array.PropertyType.GetElementType();

			var fields = GetFields(elementType, attribute);
			var props = GetProperties(elementType, attribute);

			//Generating calls for the array's data type
			List<Expression> calls = new List<Expression>();

			Expression block = Expression.Empty();

			var ranks = array.PropertyType.GetArrayRank();
			if (ranks > 2)
				throw new NotImplementedException("2D Arrays or less are currently implemented.");

			else if (ranks == 2)
			{
				//If the array value is null, assign to it using default constructor.
				calls.Add(Expression.IfThen(
					Expression.Equal(
						Expression.ArrayAccess(arr, new [] { counter1, counter2 }), Expression.Constant(null)),
					//Then
						Expression.Assign(
							Expression.ArrayAccess(arr, new [] { counter1, counter2 }),
							Expression.New(array.PropertyType.GetElementType())
							)
					));

				//Simulate serializing the type of the array. i.e: Int, Vector3 etc
				calls.AddRange(fields.Select((f, index) => GenerateWriteCalls(f, writer, Expression.ArrayAccess(arr, new [] { counter1, counter2 }))));
				calls.AddRange(props.Select((p, index) => GenerateWriteCalls(p, writer, Expression.ArrayAccess(arr, new [] { counter1, counter2 }))));

				var length1 = Expression.Call(Expression.Property(instance, array), "GetLength", null, Expression.Constant(0));
				var length2 = Expression.Call(Expression.Property(instance, array), "GetLength", null, Expression.Constant(1));

				block = Expression.Block(
					new[] { counter1, counter2, },
					Expression.Call(writer, "Write", null, length1),
					Expression.Call(writer, "Write", null, length2),
					Expression.Loop(
						Expression.IfThenElse(
							Expression.LessThan(counter1, length1),
								Expression.Loop(
									Expression.IfThenElse(
										Expression.LessThan(counter2, length2),
										Expression.Block(
											Expression.Block(
												calls
											),
											Expression.PostIncrementAssign(counter2)
											),
										Expression.Block(
											Expression.PostIncrementAssign(counter1),
											Expression.Assign(counter2, Expression.Constant(0)),
											Expression.Break(breakLabel)
											)
										),
									breakLabel
									),
							Expression.Break(breakLabelmd)
							),
						breakLabelmd
						)
					);
			}
			else if (ranks == 1)
			{
				var length1 = Expression.Call(Expression.Property(instance, array), "GetLength", null, Expression.Constant(0));

				calls.AddRange(fields.Select((f, index) => GenerateWriteCalls(f, writer, Expression.ArrayAccess(arr, counter1))));
				calls.AddRange(props.Select((p, index) => GenerateWriteCalls(p, writer, Expression.ArrayAccess(arr, counter1))));

				block = Expression.Block(
				new[] { counter1 },
				Expression.Call(writer, "Write", null, length1),
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter1, Expression.ArrayLength(arr)),
						Expression.Block(
							Expression.Block(
								calls
							),
							Expression.PostIncrementAssign(counter1)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				));
			}
			return block;
		}
		//Writes 1 or 2 extra ints for array length.
		private static Expression ReadArrayClass(FieldInfo array, Expression reader, Expression instance)
		{
			var arr = Expression.Field(instance, array);
			var counter1 = Expression.Variable(typeof(int), "counter1");
			var counter2 = Expression.Variable(typeof(int), "counter2");
			var breakLabel = Expression.Label("breakLabel");
			var breakLabelmd = Expression.Label("breakLabelmd");
			Expression.Assign(counter1, Expression.Constant(0));
			Expression.Assign(counter2, Expression.Constant(0));

			var attribute = (GenericSerializable)array.GetCustomAttribute(typeof(GenericSerializable));
			var elementType = array.FieldType.GetElementType();

			var fields = GetFields(elementType, attribute);
			var props = GetProperties(elementType, attribute);

			//Generating calls for the array's data type
			List<Expression> calls = new List<Expression>();

			Expression block = Expression.Empty();

			var ranks = array.FieldType.GetArrayRank();
			if (ranks > 2)
				throw new NotImplementedException("2D Arrays or less are currently implemented.");

			else if (ranks == 2)
			{
				//If the array[x] value is null, assign to it using default constructor.
				calls.Add(Expression.IfThen(
					Expression.Equal(
						Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }), Expression.Constant(null)),
						Expression.Assign(
							Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }),
							Expression.New(array.FieldType.GetElementType())
							)
					));

				//Simulate serializing the type of the array. i.e: Int, Vector3 etc
				calls.AddRange(fields.Select((f, index) => GenerateReadCalls(f, reader, Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }))));
				calls.AddRange(props.Select((p, index) => GenerateReadCalls(p, reader, Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }))));

				var length1 = Expression.Call(Expression.Field(instance, array), "GetLength", null, Expression.Constant(0));
				var length2 = Expression.Call(Expression.Field(instance, array), "GetLength", null, Expression.Constant(1));
				var readSerializedLength = Expression.Call(reader, "ReadInt32", null, null);

				block = Expression.Block(
				new[] { counter1, counter2 },
				//Re-assign the array, needs to check whether the array actually needs to be reassigned (compare old length to new length).
				Expression.Assign(arr ,Expression.NewArrayBounds(array.FieldType.GetElementType(), new[] { readSerializedLength, readSerializedLength })),
				Expression.Loop(
						Expression.IfThenElse(
							Expression.LessThan(counter1, length1),
								Expression.Loop(
									Expression.IfThenElse(
										Expression.LessThan(counter2, length2),
										Expression.Block(
											Expression.Block(
												calls
											),
											Expression.PostIncrementAssign(counter2)
											),
										Expression.Block(
											Expression.PostIncrementAssign(counter1),
											Expression.Assign(counter2, Expression.Constant(0)),
											Expression.Break(breakLabel)
											)
										),
									breakLabel
									),
							Expression.Break(breakLabelmd)
							),
						breakLabelmd
						)
					);
			}
			else if (ranks == 1)
			{
				//If the array[x] value is null, assign to it using default constructor.
				calls.Add(Expression.IfThen(
					Expression.Equal(
						Expression.ArrayAccess(arr, new List<Expression> { counter1 }), Expression.Constant(null)),
						Expression.Assign(
							Expression.ArrayAccess(arr, new List<Expression> { counter1 }),
							Expression.New(array.FieldType.GetElementType())
							)
					));

				calls.AddRange(fields.Select((f, index) => GenerateReadCalls(f, reader, Expression.ArrayAccess(arr, counter1))));
				calls.AddRange(props.Select((p, index) => GenerateReadCalls(p, reader, Expression.ArrayAccess(arr, counter1))));
				var readSerializedLength = Expression.Call(reader, "ReadInt32", null, null);

				block = Expression.Block(
				new[] { counter1 },
				Expression.Assign(arr, Expression.NewArrayBounds(array.FieldType.GetElementType(), new[] { readSerializedLength })),
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter1, Expression.ArrayLength(arr)),
						Expression.Block(
							Expression.Block(
								calls
							),
							Expression.PostIncrementAssign(counter1)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				));
			}
			return block;
		}

		private static Expression ReadArrayClass(PropertyInfo array, Expression reader, Expression instance)
			{
			var arr = Expression.Property(instance, array);
			var counter1 = Expression.Variable(typeof(int), "counter1");
			var counter2 = Expression.Variable(typeof(int), "counter2");
			var breakLabel = Expression.Label("breakLabel");
			var breakLabelmd = Expression.Label("breakLabelmd");
			Expression.Assign(counter1, Expression.Constant(0));
			Expression.Assign(counter2, Expression.Constant(0));

			var attribute = (GenericSerializable)array.GetCustomAttribute(typeof(GenericSerializable));
			var elementType = array.PropertyType.GetElementType();

			var fields = GetFields(elementType, attribute);
			var props = GetProperties(elementType, attribute);

			//Generating calls for the array's data type
			List<Expression> calls = new List<Expression>();

			Expression block = Expression.Empty();

			var ranks = array.PropertyType.GetArrayRank();
			if (ranks > 2)
				throw new NotImplementedException("2D Arrays or less are currently implemented.");

			else if (ranks == 2)
			{
				//If the array value is null, assign to it using default constructor.
				calls.Add(Expression.IfThen(
					Expression.Equal(
						Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }), Expression.Constant(null)),
						Expression.Assign(
							Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }),
							Expression.New(array.PropertyType.GetElementType())
							)
					));

				//Simulate serializing the type of the array. i.e: Int, Vector3 etc
				calls.AddRange(fields.Select((f, index) => GenerateReadCalls(f, reader, Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }))));
				calls.AddRange(props.Select((p, index) => GenerateReadCalls(p, reader, Expression.ArrayAccess(arr, new List<Expression> { counter1, counter2 }))));


				var length1 = Expression.Call(Expression.Property(instance, array), "GetLength", null, Expression.Constant(0));
				var length2 = Expression.Call(Expression.Property(instance, array), "GetLength", null, Expression.Constant(1));
				var readSerializedLength = Expression.Call(reader, "ReadInt32", null, null);

				block = Expression.Block(
				new[] { counter1, counter2 },
				Expression.Assign(arr, Expression.NewArrayBounds(array.PropertyType.GetElementType(), new[] { readSerializedLength, readSerializedLength })),
				Expression.Loop(
						Expression.IfThenElse(
							Expression.LessThan(counter1, length1),
								Expression.Loop(
									Expression.IfThenElse(
										Expression.LessThan(counter2, length2),
										Expression.Block(
											Expression.Block(
												calls
											),
											Expression.PostIncrementAssign(counter2)
											),
										Expression.Block(
											Expression.PostIncrementAssign(counter1),
											Expression.Assign(counter2, Expression.Constant(0)),
											Expression.Break(breakLabel)
											)
										),
									breakLabel
									),
							Expression.Break(breakLabelmd)
							),
						breakLabelmd
						)
					);
			}
			else if (ranks == 1)
			{
				calls.AddRange(fields.Select((f, index) => GenerateReadCalls(f, reader, Expression.ArrayAccess(arr, counter1))));
				calls.AddRange(props.Select((p, index) => GenerateReadCalls(p, reader, Expression.ArrayAccess(arr, counter1))));
				var readSerializedLength = Expression.Call(reader, "ReadInt32", null, null);

				block = Expression.Block(
				new[] { counter1 },
				Expression.Assign(arr, Expression.NewArrayBounds(array.PropertyType.GetElementType(), new[] { readSerializedLength })),
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter1, Expression.ArrayLength(arr)),
						Expression.Block(
							Expression.Block(
								calls
							),
							Expression.PostIncrementAssign(counter1)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				));
			}
			return block;
		}

		#endregion

		#region LISTS

		public static Expression WriteList(FieldInfo list, Expression writer, Expression instance)
		{
			var itemType = list.FieldType.GetGenericArguments()[0];
			if (itemType.IsPrimitive || itemType == typeof(decimal) || itemType == typeof(string))
				return WriteListPrimitive(list, writer, instance);

			else if (itemType.IsClass && !itemType.IsArray && !itemType.IsEnum)
				return WriteListClass(list, writer, instance);

			else
				return Expression.Empty();
		}

		public static Expression WriteList(PropertyInfo list, Expression writer, Expression instance)
		{
			var itemType = list.PropertyType.GetGenericArguments()[0];
			if (itemType.IsPrimitive || itemType == typeof(decimal) || itemType == typeof(string))
				return WriteListPrimitive(list, writer, instance);

			else if (itemType.IsClass && !itemType.IsArray && !itemType.IsEnum)
				return WriteListClass(list, writer, instance);

			else
				return Expression.Empty();
		}

		public static Expression ReadList(FieldInfo list, Expression reader, Expression instance)
		{
			var itemType = list.FieldType.GetGenericArguments()[0];
			if (itemType.IsPrimitive || itemType == typeof(decimal) || itemType == typeof(string))
				return ReadListPrimitive(list, reader, instance);

			else if (itemType.IsClass && !itemType.IsArray && !itemType.IsEnum)
				return ReadListClass(list, reader, instance);

			else
				return Expression.Empty();
		}

		public static Expression ReadList(PropertyInfo list, Expression reader, Expression instance)
		{
			var itemType = list.PropertyType.GetGenericArguments()[0];
			if (itemType.IsPrimitive || itemType == typeof(decimal) || itemType == typeof(string))
				return ReadListPrimitive(list, reader, instance);

			else if (itemType.IsClass && !itemType.IsArray && !itemType.IsEnum)
				return ReadListClass(list, reader, instance);

			else
				return Expression.Empty();
		}

		private static Expression WriteListPrimitive(FieldInfo list, Expression writer, Expression instance)
		{
			var lst = Expression.Field(instance, list);
			var counter = Expression.Variable(typeof(int), "counter");
			var breakLabel = Expression.Label("breakLabel");

			Expression.Assign(counter, Expression.Constant(0));

			MemberInfo count = list.FieldType.GetMember("Count")[0];

			var block = Expression.Block(
				new[] { counter },
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter, Expression.MakeMemberAccess(lst, count)),
						Expression.Block(
							Expression.Call(writer, "Write", null, Expression.Property(lst, "Item", counter)),
							Expression.PostIncrementAssign(counter)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				)
			);

			return block;
		}

		private static Expression WriteListPrimitive(PropertyInfo list, Expression writer, Expression instance)
		{
			var ls = Expression.Property(instance, list);
			var counter = Expression.Variable(typeof(int), "counter");
			var breakLabel = Expression.Label("breakLabel");

			Expression.Assign(counter, Expression.Constant(0));

			MemberInfo count = list.PropertyType.GetMember("Count")[0];

			var block = Expression.Block(
				new[] { counter },
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter, Expression.MakeMemberAccess(ls, count)),
						Expression.Block(
							Expression.Call(writer, "Write", null, Expression.Property(ls, "Item", counter)),
							Expression.PostIncrementAssign(counter)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				)
			);

			return block;
		}

		private static Expression ReadListPrimitive(FieldInfo list, Expression reader, Expression instance)
		{
			var ls = Expression.Field(instance, list);
			var counter = Expression.Variable(typeof(int), "counter");
			var breakLabel = Expression.Label("breakLabel");

			Expression.Assign(counter, Expression.Constant(0));

			MemberInfo count = list.FieldType.GetMember("Count")[0];
			var itemType = list.FieldType.GetGenericArguments()[0];

			var block = Expression.Block(
				new[] { counter },
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter, Expression.MakeMemberAccess(ls, count)),
						Expression.Block(
							Expression.Assign(Expression.Property(ls, "Item", counter), Expression.Call(reader, "Read" + itemType.Name, null, null)),
							Expression.PostIncrementAssign(counter)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				)
			);

			return block;
		}

		private static Expression ReadListPrimitive(PropertyInfo list, Expression reader, Expression instance)
		{
			var ls = Expression.Property(instance, list);
			var counter = Expression.Variable(typeof(int), "counter");
			var breakLabel = Expression.Label("breakLabel");

			Expression.Assign(counter, Expression.Constant(0));

			MemberInfo count = list.PropertyType.GetMember("Count")[0];
			var itemType = list.PropertyType.GetGenericArguments()[0];

			var block = Expression.Block(
				new[] { counter },
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter, Expression.MakeMemberAccess(ls, count)),
						Expression.Block(
							Expression.Assign(Expression.Property(ls, "Item", counter), Expression.Call(reader, "Read" + itemType.Name, null, null)),
							Expression.PostIncrementAssign(counter)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				)
			);

			return block;
		}

		private static Expression WriteListClass(FieldInfo list, Expression writer, Expression instance)
		{
			var lst = Expression.Field(instance, list);
			var counter = Expression.Variable(typeof(int), "counter");
			var breakLabel = Expression.Label("breakLabel");

			Expression.Assign(counter, Expression.Constant(0));
			MemberInfo count = list.FieldType.GetMember("Count")[0];

			var attribute = (GenericSerializable)list.GetCustomAttribute(typeof(GenericSerializable));
			var elementType = list.FieldType.GetGenericArguments()[0];

			var fields = GetFields(elementType, attribute);
			var props = GetProperties(elementType, attribute);

			List<Expression> calls = new List<Expression>();
			calls.AddRange(fields.Select((f, index) => GenerateWriteCalls(f, writer, Expression.Property(lst, "Item", counter))));
			calls.AddRange(props.Select((p, index) => GenerateWriteCalls(p, writer, Expression.Property(lst, "Item", counter))));

			var block = Expression.Block(
				new[] { counter },
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter, Expression.MakeMemberAccess(lst, count)),
						Expression.Block(
							Expression.Block(
								calls
							),
							Expression.PostIncrementAssign(counter)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				)
			);

			return block;
		}

		private static Expression WriteListClass(PropertyInfo list, Expression writer, Expression instance)
		{
			var lst = Expression.Property(instance, list);
			var counter = Expression.Variable(typeof(int), "counter");
			var breakLabel = Expression.Label("breakLabel");

			Expression.Assign(counter, Expression.Constant(0));
			MemberInfo count = list.PropertyType.GetMember("Count")[0];

			var attribute = (GenericSerializable)list.GetCustomAttribute(typeof(GenericSerializable));
			var elementType = list.PropertyType.GetGenericArguments()[0];

			var fields = GetFields(elementType, attribute);
			var props = GetProperties(elementType, attribute);

			List<Expression> calls = new List<Expression>();
			calls.AddRange(fields.Select((f, index) => GenerateWriteCalls(f, writer, Expression.Property(lst, "Item", counter))));
			calls.AddRange(props.Select((p, index) => GenerateWriteCalls(p, writer, Expression.Property(lst, "Item", counter))));

			var block = Expression.Block(
				new[] { counter },
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter, Expression.MakeMemberAccess(lst, count)),
						Expression.Block(
							Expression.Block(
								calls
							),
							Expression.PostIncrementAssign(counter)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				)
			);

			return block;
		}

		private static Expression ReadListClass(FieldInfo list, Expression reader, Expression instance)
		{
			var lst = Expression.Field(instance, list);
			var counter = Expression.Variable(typeof(int), "counter");
			var breakLabel = Expression.Label("breakLabel");

			Expression.Assign(counter, Expression.Constant(0));
			MemberInfo count = list.FieldType.GetMember("Count")[0];

			var attribute = (GenericSerializable)list.GetCustomAttribute(typeof(GenericSerializable));
			var elementType = list.FieldType.GetGenericArguments()[0];

			var fields = GetFields(elementType, attribute);
			var props = GetProperties(elementType, attribute);

			List<Expression> calls = new List<Expression>();
			calls.AddRange(fields.Select((f, index) => GenerateReadCalls(f, reader, Expression.Property(lst, "Item", counter))));
			calls.AddRange(props.Select((p, index) => GenerateReadCalls(p, reader, Expression.Property(lst, "Item", counter))));

			var block = Expression.Block(
				new[] { counter },
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter, Expression.MakeMemberAccess(lst, count)),
						Expression.Block(
							Expression.Block(
								calls
							),
							Expression.PostIncrementAssign(counter)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				)
			);

			return block;
		}

		private static Expression ReadListClass(PropertyInfo list, Expression reader, Expression instance)
		{
			var lst = Expression.Property(instance, list);
			var counter = Expression.Variable(typeof(int), "counter");
			var breakLabel = Expression.Label("breakLabel");

			Expression.Assign(counter, Expression.Constant(0));
			MemberInfo count = list.PropertyType.GetMember("Count")[0];

			var attribute = (GenericSerializable)list.GetCustomAttribute(typeof(GenericSerializable));
			var elementType = list.PropertyType.GetGenericArguments()[0];

			var fields = GetFields(elementType, attribute);
			var props = GetProperties(elementType, attribute);

			List<Expression> calls = new List<Expression>();
			calls.AddRange(fields.Select((f, index) => GenerateReadCalls(f, reader, Expression.Property(lst, "Item", counter))));
			calls.AddRange(props.Select((p, index) => GenerateReadCalls(p, reader, Expression.Property(lst, "Item", counter))));

			var block = Expression.Block(
				new[] { counter },
				Expression.Loop(
					Expression.IfThenElse(
						Expression.LessThan(counter, Expression.MakeMemberAccess(lst, count)),
						Expression.Block(
							Expression.Block(
								calls
							),
							Expression.PostIncrementAssign(counter)
						),
						Expression.Break(breakLabel)
					),
					breakLabel
				)
			);

			return block;
		}

		#endregion
	}
}
