using System;
using System.Diagnostics;
using System.IO;
using Blue.GenericSerializer;

namespace NetCoreBenchmark
{
	[GenericSerializable]
	public class NetworkSerializableClass
	{
		[GenericSerializable]
		public long f1 = 1;
		[GenericSerializable]
		public int f2 = 2;
		[GenericSerializable]
		public int p1 { get; set; } = 3;
		[GenericSerializable]
		public int p2 { get; set; } = 4;

		public static Action<BinaryWriter, NetworkSerializableClass> serialize;
		public static Action<BinaryReader, NetworkSerializableClass> deserialize;

		public void Serialize(BinaryWriter bw)
		{
			serialize(bw, this);
		}
		public void Deserialize(BinaryReader br)
		{
			deserialize(br, this);
		}

		public void ManualSerialize(BinaryWriter bw)
		{
			bw.Write(f1);
			bw.Write(f2);
			bw.Write(p1);
			bw.Write(p2);
		}

		public void ManualDeserialize(BinaryReader br)
		{
			f1 = br.ReadInt64();
			f2 = br.ReadInt32();
			p1 = br.ReadInt32();
			p2 = br.ReadInt32();
		}
	}

	class Program
	{
		//This benchmarking should be re-written -- WIP?
		static void Main(string[] args)
		{
			GenericSerializer.Initialize();
			//Console.ForegroundColor = ConsoleColor.Gray;

			Stopwatch sw = new Stopwatch();
			var TestClass = new NetworkSerializableClass();

			long iterationsPerClass = 1000000;
			long iterationsToAverage = 50;
			long averageTime = 0;

			while (true)
			{

				Console.ReadKey();

				using (MemoryStream stream = new MemoryStream(20000000))
				using (BinaryWriter bw = new BinaryWriter(stream))
				using (BinaryReader br = new BinaryReader(stream))
				{
					averageTime = 0;
					for (int ii = 0; ii < iterationsToAverage; ii++)
					{
						stream.Position = 0;
						sw.Reset();
						sw.Start();
						for (int i = 0; i < iterationsPerClass; i++)
						{
							TestClass.ManualSerialize(bw);
						}
						sw.Stop();
						averageTime += sw.ElapsedTicks;
						bw.Flush();						
					}

					averageTime = averageTime / iterationsToAverage;
					Console.BackgroundColor = ConsoleColor.DarkCyan;
					Console.WriteLine(String.Format("Method: Manual serialization. Iterations: {0} averaged on {1} times. Average time: {2}", iterationsPerClass, iterationsToAverage, new TimeSpan(averageTime)));

					averageTime = 0;
					for (int ii = 0; ii < iterationsToAverage; ii++)
					{
						stream.Position = 0;
						sw.Reset();
						sw.Start();
						for (int i = 0; i < iterationsPerClass; i++)
						{
							TestClass.ManualDeserialize(br);
						}
						sw.Stop();
						averageTime += sw.ElapsedTicks;
					}

					averageTime = averageTime / iterationsToAverage;
					Console.BackgroundColor = ConsoleColor.DarkCyan;
					Console.WriteLine(String.Format("Method: Manual Deserialization. Iterations: {0} averaged on {1} times. Average time: {2}", iterationsPerClass, iterationsToAverage, new TimeSpan(averageTime)));



					averageTime = 0;
					for (int ii = 0; ii < iterationsToAverage; ii++)
					{
						stream.Position = 0;
						sw.Reset();
						sw.Start();
						for (int i = 0; i < iterationsPerClass; i++)
						{
							NetworkSerializableClass.serialize(bw, TestClass);
						}
						sw.Stop();
						averageTime += sw.ElapsedTicks;
						bw.Flush();
					}

					averageTime = averageTime / iterationsToAverage;
					Console.BackgroundColor = ConsoleColor.DarkGreen;
					Console.WriteLine(String.Format("Method: Generic serialization. Iterations: {0} averaged on {1} times. Average time: {2}", iterationsPerClass, iterationsToAverage, new TimeSpan(averageTime)));

					averageTime = 0;
					for (int ii = 0; ii < iterationsToAverage; ii++)
					{
						stream.Position = 0;
						sw.Reset();
						sw.Start();
						for (int i = 0; i < iterationsPerClass; i++)
						{
							NetworkSerializableClass.deserialize(br, TestClass);
						}
						sw.Stop();
						averageTime += sw.ElapsedTicks;
					}

					averageTime = averageTime / iterationsToAverage;
					Console.BackgroundColor = ConsoleColor.DarkGreen;
					Console.WriteLine(String.Format("Method: Generic Deserialization. Iterations: {0} averaged on {1} times. Average time: {2}", iterationsPerClass, iterationsToAverage, new TimeSpan(averageTime)));

					averageTime = 0;
				}
			}
		}
	}
}
