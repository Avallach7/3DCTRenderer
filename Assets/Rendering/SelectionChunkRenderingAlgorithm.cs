using System;
using System.Collections.Generic;
using System.Linq;
using Ct3dRenderer.Data;
using Ct3dRenderer.Utils;
using UnityEngine;

namespace Ct3dRenderer.Rendering
{
	public class SelectionChunkRenderingAlgorithm : IChunkRenderingAlgorithm
	{
		private static readonly List<Material> Materials = CreateMaterials(6);

		private static List<Material> CreateMaterials(int number)
		{
			var shader = Shader.Find("Custom/CtScanSelection");
			return Enumerable.Range(1, number).Select(i => new Material(shader)
			{
				color = i < number ? new HslColor(360f*i/(number - 1), 1f, 0.5f, 0.2f).ToRGBA() : new Color(255, 0, 0, 0.5f)
			}).ToList();
		}

		private const byte _noSelection = 0;
		private const byte _selection1 = 1 << (1 - 1);
		private const byte _selection2 = 1 << (2 - 1);
		private const byte _selection3 = 1 << (3 - 1);
		private const byte _selection4 = 1 << (4 - 1);
		private const byte _selection5 = 1 << (5 - 1);
		private static int GetMaterialIndex(Block block)
		{
			switch ((byte) (block.Data & ~Block.OpacityMask))
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
			var vertsSize = new IntVector3(chunk.Size.x + 1, chunk.Size.y + 1, chunk.Size.z + 1);
			var mesh = new MeshBuilder();
			var verticeIndices = new int[vertsSize.x, vertsSize.y, vertsSize.z];
			MiscUtils.ArrayFill(verticeIndices, -1);
			mesh.TrianglesBySubmesh.AddRange(Enumerable.Range(0, Materials.Count).Select(i => new List<int>()));
			for (int x = 0; x < chunk.Size.x; x++)
				for (int y = 0; y < chunk.Size.y; y++)
					for (int z = 0; z < chunk.Size.z; z++)
						CreateBlockMesh(chunk, x, y, z, verticeIndices, mesh);
			mesh.Materials.AddRange(Materials);
			mesh.AssertIsValid();
			return mesh;
		}

		private static readonly int[] neighbourBlockOffsets =
		{
			 0,  0, -1,	// [0] Front
			 0, -1,  0,	// [1] Bottom
			-1,  0,  0,	// [2] Left
			 1,  0,  0,	// [3] Right
			 0,  1,  0,	// [4] Top
			 0,  0,  1	// [5] Back
		};

		private void CreateBlockMesh(IChunk chunk, int x, int y, int z, int[,,] verticeIndices, MeshBuilder mesh)
		{
			int materialIndex = GetMaterialIndex(chunk[x, y, z]);
			if (materialIndex == 0)
				return;

			//vertice indices - using separate variables is significantly faster than array :|
			int v0i = GetVerticeIndex(x, y, z, verticeIndices, mesh.Vertices);
			int v1i = GetVerticeIndex(x + 1, y, z, verticeIndices, mesh.Vertices);
			int v2i = GetVerticeIndex(x, y + 1, z, verticeIndices, mesh.Vertices);
			int v3i = GetVerticeIndex(x, y, z + 1, verticeIndices, mesh.Vertices);
			int v4i = GetVerticeIndex(x + 1, y + 1, z, verticeIndices, mesh.Vertices);
			int v5i = GetVerticeIndex(x + 1, y, z + 1, verticeIndices, mesh.Vertices);
			int v6i = GetVerticeIndex(x, y + 1, z + 1, verticeIndices, mesh.Vertices);
			int v7i = GetVerticeIndex(x + 1, y + 1, z + 1, verticeIndices, mesh.Vertices);


			//create triangles by declaring connections between vertices
			int[] triangleVertIndices =
			{
				v1i, v0i, v4i,    v4i, v0i, v2i,	// [0] Front
				v5i, v0i, v1i,    v3i, v0i, v5i,	// [1] Bottom
				v2i, v0i, v6i,    v6i, v0i, v3i,	// [2] Left
				v4i, v7i, v1i,    v1i, v7i, v5i,	// [3] Right
				v2i, v7i, v4i,    v6i, v7i, v2i,	// [4] Top
				v5i, v7i, v3i,    v3i, v7i, v6i		// [5] Back
			};
			List<int> triangles = mesh.TrianglesBySubmesh[materialIndex - 1];
			//for (int i = 0; i < 36; i++)
			//	triangles.Add(triangleVertIndices[i]);
			for (int faceId = 0; faceId < 6; faceId++)
				if (materialIndex != GetMaterialIndex(chunk[x + neighbourBlockOffsets[faceId * 3 + 0],
								y + neighbourBlockOffsets[faceId * 3 + 1],
								z + neighbourBlockOffsets[faceId * 3 + 2]]))
					for (int vertId = 0; vertId < 6; vertId++)
						triangles.Add(triangleVertIndices[faceId * 6 + vertId]);
		}

		private int GetVerticeIndex(int x, int y, int z, int[,,] verticeIndices, List<Vector3> vertices)
		{
			int index = verticeIndices[x, y, z];
			if (index == -1)
			{
				vertices.Add(new Vector3(x, y, z));
				index = verticeIndices[x, y, z] = vertices.Count - 1;
			}
			return index;
		}
	}
}