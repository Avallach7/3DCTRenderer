using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ct3dRenderer.Data;
using Ct3dRenderer.Utils;
using UnityEngine;

namespace Ct3dRenderer.Rendering
{
	public class SelectionChunkRenderingAlgorithm : IChunkRenderingAlgorithm
	{
		private static List<Material> _materials = null;
		private static List<Material> Materials
		{
			get
			{
				return _materials ?? (_materials = CreateMaterials(6));
			}
		}

		public MeshBuilder CreateMesh(IChunk chunk)
		{
			var vertsSize = new IntVector3(chunk.Size.x + 1, chunk.Size.y + 1, chunk.Size.z + 1);
			var mesh = new MeshBuilder();
			var verticeIndices = new int[vertsSize.x, vertsSize.y, vertsSize.z];
			MiscUtils.ArrayFill(verticeIndices, -1);
			mesh.Vertices.Capacity = verticeIndices.Length; // TODO: test
			mesh.TrianglesBySubmesh.AddRange(Enumerable.Range(0, Materials.Count).Select(i => new List<int>()));
			for (int x = 0; x < chunk.Size.x; x++)
				for (int y = 0; y < chunk.Size.y; y++)
					for (int z = 0; z < chunk.Size.z; z++)
						CreateBlockMesh(chunk, new IntVector3(x, y, z), verticeIndices, mesh);
			mesh.Materials.AddRange(Materials);
			mesh.AssertIsValid();
			return mesh;
		}

		private void CreateBlockMesh(IChunk chunk, IntVector3 blockPos, int[,,] verticeIndices, MeshBuilder mesh)
		{
			int materialIndex = GetMaterialIndex(chunk[blockPos.x, blockPos.y, blockPos.z]);
			if (materialIndex == 0)
				return;
			//create triangles by declaring connections between vertices
			List<int> triangles = mesh.TrianglesBySubmesh[materialIndex - 1];
			for (int faceId = 0; faceId < 6; faceId++)
				if (materialIndex != GetMaterialIndex(chunk[blockPos.x + NeighbourBlockOffsets[faceId, 0],
					blockPos.y + NeighbourBlockOffsets[faceId, 1],
					blockPos.z + NeighbourBlockOffsets[faceId, 2]]))
					for (int vertId = 0; vertId < 6; vertId++)
						triangles.Add(GetVerticeIndex(blockPos, TriangleVerticePositionOffsets[faceId, vertId], verticeIndices, mesh.Vertices));
		}

		private int GetVerticeIndex(IntVector3 blockPosition, IntVector3 verticePositionOffset, int[,,] verticeIndices, List<Vector3> vertices)
		{
			IntVector3 position = blockPosition + verticePositionOffset;
			int index = verticeIndices[position.x, position.y, position.z];
			if (index == -1)
			{
				vertices.Add(new Vector3(position.x, position.y, position.z));
				index = verticeIndices[position.x, position.y, position.z] = vertices.Count - 1;
			}
			return index;
		}

		private static List<Material> CreateMaterials(int number)
		{
			var shader = Shader.Find("Custom/CtScanSelection");
			return Enumerable.Range(1, number).Select(i => new Material(shader)
			{
				color = i < number ? new HslColor(360f * i / (number - 1), 1f, 0.5f, 0.2f).ToRGBA() : new Color(1, 0, 0, 0.5f)
			}).ToList();
		}

		private static int GetMaterialIndex(Block block)
		{
			switch ((byte)(block.Data & ~Block.OpacityMask))
			{
				case 0: return 0;
				case 1 << (1 - 1): return 1;
				case 1 << (2 - 1): return 2;
				case 1 << (3 - 1): return 3;
				case 1 << (4 - 1): return 4;
				case 1 << (5 - 1): return 5;
				default: return 6;
			}
		}

		private static readonly int[,] NeighbourBlockOffsets =
		{
			{ -1,  0,  0  }, // [0] Front
			{  0, -1,  0  }, // [1] Bottom
			{  0,  0, -1  }, // [2] Left
			{  0,  0,  1  }, // [3] Right
			{  0,  1,  0  }, // [4] Top
			{  1,  0,  0  }, // [5] Back
		};

		private static readonly IntVector3[,] TriangleVerticePositionOffsets =
		{
			{	// [0] Front
				new IntVector3(0, 1, 0), new IntVector3(0, 0, 0), new IntVector3(0, 1, 1),
				new IntVector3(0, 1, 1), new IntVector3(0, 0, 0), new IntVector3(0, 0, 1)
			},
			{	// [1] Bottom
				new IntVector3(0, 0, 1), new IntVector3(0, 0, 0), new IntVector3(1, 0, 1),
				new IntVector3(1, 0, 1), new IntVector3(0, 0, 0), new IntVector3(1, 0, 0)
			},
			{	// [2] Left
				new IntVector3(1, 0, 0), new IntVector3(0, 0, 0), new IntVector3(1, 1, 0),
				new IntVector3(1, 1, 0), new IntVector3(0, 0, 0), new IntVector3(0, 1, 0)
			},
			{	// [3] Right
				new IntVector3(1, 0, 1), new IntVector3(1, 1, 1), new IntVector3(0, 0, 1),
				new IntVector3(0, 0, 1), new IntVector3(1, 1, 1), new IntVector3(0, 1, 1)
			},
			{	// [4] Top
				new IntVector3(0, 1, 1), new IntVector3(1, 1, 1), new IntVector3(0, 1, 0),
				new IntVector3(0, 1, 0), new IntVector3(1, 1, 1), new IntVector3(1, 1, 0)
			},
			{	// [5] Back
				new IntVector3(1, 1, 0), new IntVector3(1, 1, 1), new IntVector3(1, 0, 0),
				new IntVector3(1, 0, 0), new IntVector3(1, 1, 1), new IntVector3(1, 0, 1)
			}
		};
	}
}