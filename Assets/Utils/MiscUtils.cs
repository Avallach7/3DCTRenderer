using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Ct3dRenderer.Utils
{
	static class MiscUtils
	{
		[Pure]
		public static string GetTimestamp()
		{
			return DateTime.Now.ToString("MMddHHmmssffff");
		}

		public static void Repeat(this Action action, int times)
		{
			for (int i = 0; i < times; i++)
				action();
		}

		[Pure]
		public static T GetRandom<T>(this List<T> list)
		{
			return list[(int)(UnityEngine.Random.value * (list.Count - float.Epsilon))];
		}

		public static void ArrayFill<T>(Array destination, T element)
		{
			destination.SetValue(element, Enumerable.Repeat(0, destination.Rank).ToArray());
			int arrayToFillHalfLength = Enumerable.Range(0, destination.Rank)
												  .Select(dimension => destination.GetLength(dimension))
												  .Aggregate(1, (x, y) => x * y) / 2;
			for (int i = 1; i < destination.Length; i *= 2)
				Array.Copy(destination, 0, destination, i, (i <= arrayToFillHalfLength) ? i : (destination.Length - i));
		}

		public static void ArrayFill<T>(T[,,] destination, T element)
		{
			for (int x = destination.GetLength(0)-1; x >= 0; x--)
				for (int y = destination.GetLength(1)-1; y >= 0; y--)
					for (int z = destination.GetLength(2) - 1; z >= 0; z--)
						destination[x, y, z] = element;
        }

		public static void ArrayFillGeneric<T>(Array destination, T element) // 4x slower than with known rank
		{
			destination.SetValue(element, Enumerable.Repeat(0, destination.Rank).ToArray());
			int arrayToFillHalfLength = destination.Length / 2;
			for (int i = 1; i < destination.Length; i *= 2)
				Array.Copy(destination, 0, destination, i, (i <= arrayToFillHalfLength) ? i : (destination.Length - i));
		}
	}
}