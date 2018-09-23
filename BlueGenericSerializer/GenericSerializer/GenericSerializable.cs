using System;
using System.Reflection;

namespace Blue.GenericSerializer
{
	public class GenericSerializable : Attribute
	{
		/// <summary>
		/// Specifies that the attribute was constructed using the default constructor with no options provided.
		/// </summary>
		public readonly bool Default = false;

		public readonly BindingFlags Flags = BindingFlags.Default;

		/// <summary>
		/// Constructing using this means you MUST add [GenericSerializable] to the fields/props you want to serialize.
		/// DANGEROUS! if there are inconsistencies between server client
		/// </summary>
		public GenericSerializable()
		{
			this.Default = true;
		}

		/// <summary>
		/// Allows you to specify your own binding flags for serialization.
		/// DANGEROUS! if there are inconsistencies between server client
		/// </summary>
		/// <param name="Flags"></param>
		public GenericSerializable(BindingFlags Flags)
		{
			this.Flags = Flags;
		}

		/// <summary>
		/// Quickie to serialize all Public Instance fields/properties.
		/// </summary>
		/// <param name="SerializeAllPublic"></param>
		public GenericSerializable(bool SerializeAllPublic)
		{
			if (SerializeAllPublic)
				this.Flags = BindingFlags.Public | BindingFlags.Instance;
			else
				Default = true;
		}
	}
}