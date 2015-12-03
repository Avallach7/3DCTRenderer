using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ct3dRenderer.Data;
using Ct3dRenderer.Utils;
using UnityEngine;

namespace Ct3dRenderer.Rendering
{
	class SmoothSelectionRenderingAlgorithm : IChunkRenderingAlgorithm
	{
		private static readonly Material Material = 
			new Material(Shader.Find("Custom/CtScanSelection")) { color = new Color(1f, 0f, 0f, 0.5f) };

		private const byte _noSelection = 0;
		private const byte _selection1 = 1 << (1 - 1);
		private const byte _selection2 = 1 << (2 - 1);
		private const byte _selection3 = 1 << (3 - 1);
		private const byte _selection4 = 1 << (4 - 1);
		private const byte _selection5 = 1 << (5 - 1);
		private static int GetMaterialIndex(Block block)
		{
			switch ((byte)(block.Data & ~Block.OpacityMask))
			{
				case _noSelection: return 0;
				case _selection1: return 1;
				case _selection2: return 2;
				case _selection3: return 3;
				case _selection4: return 4;
				case _selection5: return 5;
				default: return 6;
			}
		}

		public MeshBuilder CreateMesh(IChunk chunk)
		{
			throw new NotImplementedException();
			float[,,] voxels = new float[chunk.Size.x, chunk.Size.y, chunk.Size.z * 2];
			for (int x = 0; x < voxels.GetLength(0); x++)
				for (int y = 0; y < voxels.GetLength(1); y++)
					for (int z = 0; z < voxels.GetLength(2); z++)
					{
						int visibilityPoints = isVisible(x, y, z, chunk) ? 1 : 0;
						int neighbouringPoints = (isVisible(x, y + 1, z, chunk) ? 1 : 0) + (isVisible(x, y - 1, z, chunk) ? 1 : 0);

						if (visibilityPoints == 0)
						{
							if (neighbouringPoints == 0)
								voxels[x, y, z] = 0;
							else if (neighbouringPoints == 1)
								voxels[x, y, z] = 0.2f;
							else if (neighbouringPoints == 2)
								voxels[x, y, z] = 0.4f;
						}
						else if (visibilityPoints == 1)
						{
							if (neighbouringPoints == 0)
								voxels[x, y, z] = 0.6f;
							else if (neighbouringPoints == 1)
								voxels[x, y, z] = 2f;
							else if (neighbouringPoints == 2)
								voxels[x, y, z] = 20f;
						}
					}

			//Mesh mesh = MarchingCubes.CreateMesh(voxels);
			var meshBuilder = new MeshBuilder();
			meshBuilder.Materials.Add(Material);
			return meshBuilder;
		}

		private bool isVisible(int x, int y, int z, IChunk chunk)
		{
			return GetMaterialIndex(chunk[x, y, z]) > 0;
		}
	}
}
