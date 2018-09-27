using System;
using System.Collections.Generic;
using System.Reflection;

namespace Blue.GenericSerializer
{
	/// <summary>
	/// Same rules & options apply for both marking classes and properties/fields
	/// WARNING: if there are inconsistencies in a server - client relation, things will break.
	/// </summary>
	public class GenericSerializable : Attribute
	{
		/// <summary>
		/// Specifies that the attribute was constructed using the default constructor with no options provided.
		/// </summary>
		public readonly bool DefaultOptions = false;

		/// <summary>
		/// Specifies whether or not serializable members are to be filtered by name.
		/// </summary>
		public readonly bool FilterByNames = false;

		/// <summary>
		/// Binding flags
		/// </summary>
		public readonly BindingFlags Flags = BindingFlags.Default;

		/// <summary>
		/// Member names to serialize, other members are ignored.
		/// </summary>
		public readonly string[] MemberNames;

		/// <summary>
		/// The lazy constructor,
		/// constructing using this means you MUST add [GenericSerializable] to the members you want to serialize.
		/// DO NOT USE THIS for class/struct type members!
		/// </summary>
		public GenericSerializable()
		{
			this.DefaultOptions = true;
		}

		/// <summary>
		/// Allows you to specify your own binding flags for serialization. 
		/// </summary>
		/// <param name="Flags">Binding Flags</param>
		public GenericSerializable(BindingFlags Flags)
		{
			this.Flags = Flags;
		}


		/// <summary>
		/// Simple serialization options. WARNING: Setting SerializeAllPublic to false means you do NOT serialize anything.
		/// </summary>
		/// <param name="SerializePublic">Serialize all public members.</param>
		/// <param name="SerializeInstances">Serialize inherited members.</param>
		public GenericSerializable(bool SerializePublic)
		{
			if (SerializePublic)
				Flags = Flags | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
		}

		/// <summary>
		/// Simple serialization options.
		/// </summary>
		/// <param name="SerializePublic">Serialize all public members.</param>
		/// <param name="SerializePrivate">Serialize all private members.</param>
		public GenericSerializable(bool SerializePublic, bool SerializePrivate)
		{
			Flags = Flags | BindingFlags.Instance | BindingFlags.DeclaredOnly;
			if (SerializePublic)
				Flags = Flags | BindingFlags.Public;
			if (SerializePrivate)
				Flags = Flags | BindingFlags.NonPublic;
		}

		/// <summary>
		/// Simple serialization options.
		/// </summary>
		/// <param name="SerializePublic">Serialize all public members.</param>
		/// <param name="SerializePrivate">Serialize all private members.</param>
		/// <param name="SerializeInstances">Serialize inherited members.</param>
		public GenericSerializable(bool SerializePublic, bool SerializePrivate, bool SerializeInherited)
		{
			Flags = Flags | BindingFlags.Instance;
			if (SerializePublic)
				Flags = Flags | BindingFlags.Public;
			if (SerializePrivate)
				Flags = Flags | BindingFlags.NonPublic;
			if (SerializeInherited)
				Flags = Flags | BindingFlags.FlattenHierarchy;
			else if (!SerializeInherited)
				Flags = Flags | BindingFlags.DeclaredOnly;
		}

		/// <summary>
		/// Filters the serializable members by name.
		/// </summary>
		/// <param name="MemberNames"></param>
		/// <param name="SerializeInherited"></param>
		public GenericSerializable(string[] MemberNames, bool SerializeInherited)
		{
			this.MemberNames = MemberNames;
			this.FilterByNames = true;

			this.Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			if (SerializeInherited)
				Flags = Flags | BindingFlags.FlattenHierarchy;
			else if (!SerializeInherited)
				Flags = Flags | BindingFlags.DeclaredOnly;
		}
	}
}