using System;
using System.Diagnostics;
using System.IO;
using Blue.GenericSerializer;
using FeatureTesting;

namespace Benchmark
{
	class Program
	{
		//This benchmarking should be re-written -- WIP?
		static void Main(string[] args)
		{
			GenericSerializer.Initialize();
			//Console.ForegroundColor = ConsoleColor.Gray;

			Stopwatch sw = new Stopwatch();
			var TestClass = new TestSerializable();

			long iterationsPerClass = 10000;
			long iterationsToAverage = 1000;
			long averageTime = 0;

			while (true)
			{
				Console.ReadKey();

				using (MemoryStream stream = new MemoryStream(Int32.MaxValue / 5))
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
							TestClass.DirectSerialize(bw);
						}
						sw.Stop();
						averageTime += sw.ElapsedTicks;
					}

					averageTime = averageTime / iterationsToAverage;
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine(String.Format("Method: Manual serialization. Iterations: {0} averaged over {1} iterations. Average time: {2}", iterationsPerClass, iterationsToAverage, new TimeSpan(averageTime)));

					averageTime = 0;
					for (int ii = 0; ii < iterationsToAverage; ii++)
					{
						stream.Position = 0;
						sw.Reset();
						sw.Start();
						for (int i = 0; i < iterationsPerClass-1; i++)
						{
							TestClass.DirectDeserialize(br);
						}
						sw.Stop();
						averageTime += sw.ElapsedTicks;
					}

					averageTime = averageTime / iterationsToAverage;
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine(String.Format("Method: Manual Deserialization. Iterations: {0} averaged over {1} iterations. Average time: {2}", iterationsPerClass, iterationsToAverage, new TimeSpan(averageTime)));



					averageTime = 0;
					for (int ii = 0; ii < iterationsToAverage; ii++)
					{
						stream.Position = 0;
						sw.Reset();
						sw.Start();
						for (int i = 0; i < iterationsPerClass; i++)
						{
							//TestClass.Serialize(bw);
							TestSerializable.serialize(bw, TestClass);
						}
						sw.Stop();
						averageTime += sw.ElapsedTicks;
					}

					averageTime = averageTime / iterationsToAverage;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine(String.Format("Method: Generic serialization. Iterations: {0} averaged over {1} iterations. Average time: {2}", iterationsPerClass, iterationsToAverage, new TimeSpan(averageTime)));

					averageTime = 0;
					for (int ii = 0; ii < iterationsToAverage; ii++)
					{
						stream.Position = 0;
						sw.Reset();
						sw.Start();
						for (int i = 0; i < iterationsPerClass; i++)
						{
							//NetworkSerializableClass.deserialize(br, TestClass);
							//TestClass.Deserialize(br);
							TestSerializable.deserialize(br, TestClass);
						}
						sw.Stop();
						averageTime += sw.ElapsedTicks;
					}

					averageTime = averageTime / iterationsToAverage;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine(String.Format("Method: Generic Deserialization. Iterations: {0} averaged over {1} iterations. Average time: {2}", iterationsPerClass, iterationsToAverage, new TimeSpan(averageTime)));

					averageTime = 0;
				}
			}
		}
	}
}
