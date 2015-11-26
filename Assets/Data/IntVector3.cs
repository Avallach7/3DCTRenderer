using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ct3dRenderer.Data
{
	public struct IntVector3
	{
		public int x,y,z;

		public int this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return x;
					case 1:
						return y;
					case 2:
						return z;
					default:
						throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						x = value;
						break;
					case 1:
						y = value;
						break;
					case 2:
						z = value;
						break;
					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		public IntVector3(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static IntVector3 operator *(IntVector3 a, IntVector3 b)
		{
			return new IntVector3(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static implicit operator Vector3(IntVector3 that)
		{
			return new Vector3(that.x, that.y, that.z);
		}

		public override string ToString()
		{
			return "{x=" + x + ",y=" + y + ",z=" + z + "}";
		}

		public IEnumerable<IntVector3> Iterate()
		{
			for (int x = 0; x < this.x; x++)
				for (int y = 0; y < this.y; y++)
					for (int z = 0; z < this.z; z++)
						yield return new IntVector3(x,y,z);
		}
	}
}