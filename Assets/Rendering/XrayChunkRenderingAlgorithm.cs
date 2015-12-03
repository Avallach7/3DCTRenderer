using System.Collections.Generic;
using System.Linq;
using Ct3dRenderer.Data;
using Ct3dRenderer.Utils;
using UnityEngine;

namespace Ct3dRenderer.Rendering
{
	public class XrayChunkRenderingAlgorithm : IChunkRenderingAlgorithm
	{
		private static List<Material> Materials = CreateMaterials(1);

		private static List<Material> CreateMaterials(int number)
		{
			Shader shader = Shader.Find("Custom/CtScanShader");
			return Enumerable.Range(1, number).Select(i =>
			{
				var material = new Material(shader);
				float alpha = Mathf.Pow(1f*i/(number), 3) / 60;
				float brightness = 1;
				material.color = new Color(brightness, brightness, brightness, alpha);
				//DebugUtils.Log("CreateMaterial: i=" + i + " => alpha=" + alpha);
				return material;
			}).ToList();
		}

		private static int GetMaterialIndex(Block block)
		{
			var result = block.XrayOpacity * (Materials.Count) / 224;
			//DebugUtils.Log("GetMaterialIndex(" + block.XrayOpacity + ") => " + result);
			return result;
		}

		public MeshBuilder CreateMesh(IChunk chunk)
		{
			var vertsSize = new IntVector3(chunk.Size.x + 1, chunk.Size.y + 1, chunk.Size.z + 1);
			var mesh = new MeshBuilder();
            var verticeIndices = new int[vertsSize.x, vertsSize.y, vertsSize.z];
			MiscUtils.ArrayFill(verticeIndices, -1);
			//mesh.TrianglesBySubmesh.AddRange(Enumerable.Repeat(new List<int>(), Materials.Count));
			mesh.TrianglesBySubmesh.AddRange(Enumerable.Range(0, Materials.Count).Select(i => new List<int>()));
			for (int x = 0; x < chunk.Size.x; x++)
				for (int y = 0; y < chunk.Size.y; y++)
					for (int z = 0; z < chunk.Size.z; z++)
						CreateBlockMesh(x, y, z, GetMaterialIndex(chunk[x, y, z]), verticeIndices, mesh);
			mesh.Materials.AddRange(Materials);
			mesh.AssertIsValid();
			return mesh;
		}

		private void CreateBlockMesh(int x, int y, int z, int materialIndex, int[,,] verticeIndices, MeshBuilder mesh)
		{
			if (materialIndex == 0)
				return;

			//vertice indices - using separate variables is significantly faster than array :|
			int v0i = GetVerticeIndex(x,     y,     z    , verticeIndices, mesh.Vertices);
			int v1i = GetVerticeIndex(x + 1, y,     z    , verticeIndices, mesh.Vertices);
			int v2i = GetVerticeIndex(x,     y + 1, z    , verticeIndices, mesh.Vertices);
			int v3i = GetVerticeIndex(x,     y,     z + 1, verticeIndices, mesh.Vertices);
			int v4i = GetVerticeIndex(x + 1, y + 1, z    , verticeIndices, mesh.Vertices);
			int v5i = GetVerticeIndex(x + 1, y,     z + 1, verticeIndices, mesh.Vertices);
			int v6i = GetVerticeIndex(x,     y + 1, z + 1, verticeIndices, mesh.Vertices);
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
			for (int i = 0; i < 36; i++)
				triangles.Add(triangleVertIndices[i]);
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