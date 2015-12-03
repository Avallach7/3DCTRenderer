using System;
using System.Linq;

namespace Ct3dRenderer.Utils
{
	static class TempBenchmark
	{
		public static void Run()
		{
            bool result = false;
			const int repetitions = 1000;
			Stopwatch.Start("Method1");
			for (int i = 0; i < repetitions; i++)
				result ^= Method1();
			Stopwatch.StopCurrent();

			Stopwatch.Start("Method2");
			for (int i = 0; i < repetitions; i++)
				result ^= Method2();
			Stopwatch.StopCurrent();
			
		}

		private static bool Method1()
		{
			var array = new int[30, 30, 30];
			ArrayFill1(array, -1);
			return true;
		}
		
		private static bool Method2()
		{
			var array = new int[30, 30, 30];
			ArrayFill2(array, -1);
			return true;
		}
		
		public static void ArrayFill1<T>(Array destination, T element) // 4x slower than with known rank
		{
			destination.SetValue(element, Enumerable.Repeat(0, destination.Rank).ToArray());
			int arrayToFillHalfLength = destination.Length / 2;
			for (int i = 1; i < destination.Length; i *= 2)
				Array.Copy(destination, 0, destination, i, (i <= arrayToFillHalfLength) ? i : (destination.Length - i));
		}

		public static void ArrayFill2<T>(T[,,] destination, T element)
		{
			for (int x = destination.GetLength(0) - 1; x >= 0; x--)
				for (int y = destination.GetLength(1) - 1; y >= 0; y--)
					for (int z = destination.GetLength(2) - 1; z >= 0; z--)
						destination[x, y, z] = element;
		}
	}
}
