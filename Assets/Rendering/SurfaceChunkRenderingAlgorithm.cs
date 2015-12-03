using System;
using System.Collections.Generic;
using System.Linq;
using Ct3dRenderer.Data;
using Ct3dRenderer.Utils;
using UnityEngine;

namespace Ct3dRenderer.Rendering
{
	public class SurfaceChunkRenderingAlgorithm : IChunkRenderingAlgorithm
	{
		public static readonly Material Material =
				new Material(Shader.Find("Custom/CtScanSelection"))
				{
					color = new Color(0, 0, 0, 220f/255f),
				};

		public MeshBuilder CreateMesh(IChunk chunk)
		{
			var vertsSize = new IntVector3(chunk.Size.x + 1, chunk.Size.y + 1, chunk.Size.z + 1);
			var mesh = new MeshBuilder();
			var verticeIndices = new int[vertsSize.x, vertsSize.y, vertsSize.z];
			MiscUtils.ArrayFill(verticeIndices, -1);
			mesh.TrianglesBySubmesh.Add(new List<int>());
			for (int x = 0; x < chunk.Size.x; x++)
				for (int y = 0; y < chunk.Size.y; y++)
					for (int z = 0; z < chunk.Size.z; z++)
						CreateBlockMesh(chunk, x, y, z, verticeIndices, mesh);
			mesh.Materials.Add(Material);
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

		private bool IsVisible(Block block)
		{
			return block.XrayOpacity == 224;
		}

		private void CreateBlockMesh(IChunk chunk, int x, int y, int z, int[,,] verticeIndices, MeshBuilder mesh)
		{
			if (!IsVisible(chunk[x, y, z]))
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
			List<int> triangles = mesh.TrianglesBySubmesh[0];
			//for (int i = 0; i < 36; i++)
			//	triangles.Add(triangleVertIndices[i]);
			for (int faceId = 0; faceId < 6; faceId++)
				if (!IsVisible(chunk[x + neighbourBlockOffsets[faceId * 3 + 0],
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