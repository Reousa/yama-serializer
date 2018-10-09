using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Blue.GenericSerializer;

namespace FeatureTesting
{
	public class Vector2
	{
		public float x, y;

		public Vector2(float x, float y)
		{
			this.x = x;
			this.y = y;
		}
	}
	public class Vector3
	{
		public float magnitude, normalized, sqrMagnitude;
		public float x, y, z;

		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}

	[GenericSerializable(true, false, false)]
	public class TestSerializable : IGenericSerializable<BinaryReader, BinaryWriter>
	{
		public static Action<BinaryWriter, TestSerializable> serialize;
		public static Action<BinaryReader, TestSerializable> deserialize;

		public int[] array1 = new int[6] { 1, 2, 3, 4, 5, 6 };

		[GenericSerializable(new[] { "x", "y", "z" }, true, false, false)]
		public Vector3[] array2 = new Vector3[3] { new Vector3(1, 2, 3), new Vector3(3, 4, 3), new Vector3(4, 5, 3) };

		public List<int> list1 = new List<int>() { 5, 6, 33, 4 };

		[GenericSerializable(new[] { "x", "y", "z" }, true, false, false)]
		public List<Vector3> list2 = new List<Vector3>() { new Vector3(1, 2, 3), new Vector3(3, 4, 3), new Vector3(4, 5, 3) };

		public bool var0 = false;
		public byte var1 = 1;
		public sbyte var2 { get; set; } = 2;
		public short var3 { get; set; } = 3;
		public ushort var4 { get; set; } = 4;
		public int var5 { get; set; } = 5;
		public uint var6 = 6;
		public long var7 = 7;
		public ulong var8 = 8;
		public float var9 = 9.5f;
		public double var10 = 10.55;
		public decimal var11 = 11;
		public char var12 = 'a';
		public string var13 = "5568";

		Vector3 vector3 = new Vector3(0, 0, 0);

		public void Serialize(BinaryWriter writer)
		{
			serialize(writer, this);
		}

		public void Deserialize(BinaryReader reader)
		{
			deserialize(reader, this);
		}

		public void GenerateRandomValues()
		{
			Random rnd = new Random();

			for (int i = 0; i < array1.Length; i++)
			{
				array1[i] = rnd.Next(0, 200);
			}

			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = new Vector3(rnd.Next(), rnd.Next(), rnd.Next());
			}

			for (int i = 0; i < list1.Count; i++)
			{
				list1[i] = rnd.Next(0, 200);
			}

			for (int i = 0; i < list2.Count; i++)
			{
				list2[i] = new Vector3(rnd.Next(0, 200), rnd.Next(0, 1000), rnd.Next(0, 1000));
			}

			var1 = (byte)rnd.Next(0, 200);
			var2 = (sbyte)rnd.Next(0, 200);
			var3 = (byte)rnd.Next(0, 200);
			var4 = (byte)rnd.Next(0, 200);
			var5 = (byte)rnd.Next(0, 200);
			var6 = (byte)rnd.Next(0, 200);
			var7 = (byte)rnd.Next(0, 200);
			var8 = (byte)rnd.Next(0, 200);
			var9 = (byte)rnd.Next(0, 200);
			var10 = (byte)rnd.Next(0, 200);
			var11 = (byte)rnd.Next(0, 200);
			var12 = (char)rnd.Next(0, 200);
			var13 = rnd.Next().ToString();

			vector3 = new Vector3(rnd.Next(0, 200), rnd.Next(0, 1000), rnd.Next(0, 1000));
		}

		public void DirectDeserialize(BinaryReader br)
		{
			for (int i = 0; i < array1.Length; i++)
			{
				array1[i] = br.ReadInt32();
			}

			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].x = br.ReadSingle();
				array2[i].y = br.ReadSingle();
				array2[i].z = br.ReadSingle();
			}

			for (int i = 0; i < list1.Count; i++)
			{
				list1[i] = br.ReadInt32();
			}

			for (int i = 0; i < list2.Count; i++)
			{
				list2[i].x = br.ReadSingle();
				list2[i].y = br.ReadSingle();
				list2[i].z = br.ReadSingle();
			}

			var0 = br.ReadBoolean();
			var1 = br.ReadByte();
			var2 = br.ReadSByte();
			var3 = br.ReadInt16();
			var4 = br.ReadUInt16();
			var5 = br.ReadInt32();
			var6 = br.ReadUInt32();
			var7 = br.ReadInt64();
			var8 = br.ReadUInt64();
			var9 = br.ReadSingle();
			var10 = br.ReadDouble();
			var11 = br.ReadDecimal();
			var12 = br.ReadChar();
			var13 = br.ReadString();

			vector3.x = br.ReadSingle();
			vector3.y = br.ReadSingle();
			vector3.z = br.ReadSingle();
		}

		public void DirectSerialize(BinaryWriter bw)
		{
			for (int i = 0; i < array1.Length; i++)
			{
				bw.Write(array1[i]);
			}

			for (int i = 0; i < array2.Length; i++)
			{
				bw.Write(array2[i].x);
				bw.Write(array2[i].y);
				bw.Write(array2[i].z);
			}

			for (int i = 0; i < list1.Count; i++)
			{
				bw.Write(list1[i]);
			}

			for (int i = 0; i < list2.Count; i++)
			{
				bw.Write(list2[i].x);
				bw.Write(list2[i].y);
				bw.Write(list2[i].z);
			}

			bw.Write(var0);
			bw.Write(var1);
			bw.Write(var2);
			bw.Write(var3);
			bw.Write(var4);
			bw.Write(var5);
			bw.Write(var6);
			bw.Write(var7);
			bw.Write(var8);
			bw.Write(var9);
			bw.Write(var10);
			bw.Write(var11);
			bw.Write(var12);
			bw.Write(var13);

			bw.Write(vector3.x);
			bw.Write(vector3.y);
			bw.Write(vector3.z);
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			Stopwatch sw = new Stopwatch();

			sw.Start();
			if (!GenericSerializer.IsInitialized)
				GenericSerializer.Initialize();
			sw.Stop();

			Console.WriteLine(sw.Elapsed);

			MemoryStream stream = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(stream);
			BinaryReader br = new BinaryReader(stream);

			TestSerializable fieldTest = new TestSerializable();
			TestSerializable randomValues = new TestSerializable();

			randomValues.GenerateRandomValues();
			randomValues.Serialize(bw);
			stream.Position = 0;

			fieldTest.Deserialize(br);
			stream.Position = 0;

			Console.ReadKey();
		}
	}
}
