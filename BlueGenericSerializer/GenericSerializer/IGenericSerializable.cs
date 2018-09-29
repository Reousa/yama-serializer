using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.GenericSerializer
{
	public interface IGenericSerializable<Reader, Writer>
	{
		void Serialize(Writer writer);
		void Deserialize(Reader reader);
	}
}
