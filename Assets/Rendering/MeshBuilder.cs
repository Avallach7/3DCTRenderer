using System;
using System.Collections.Generic;
using Ct3dRenderer.Utils;
using UnityEngine;

namespace Ct3dRenderer.Rendering
{
	public class MeshBuilder
	{
		public List<Vector3> Vertices;
		public List<List<int>> TrianglesBySubmesh;
		public List<Material> Materials;
		public int materialsCount;
		private readonly Mesh _meshToReuse;

		public MeshBuilder(Mesh meshToReuse = null)
		{
			_meshToReuse = meshToReuse;
			Vertices = new List<Vector3>();
			TrianglesBySubmesh = new List<List<int>> {new List<int>()};
			Materials = new List<Material>();
		}

		// can be only called from main thread!
		public void Apply(GameObject go)
		{
			if(0 == Materials.Count) DebugUtils.Log("Cannot apply mesh without materials!!! " + go.name);
			var mesh = _meshToReuse ?? new Mesh();
			mesh.vertices = Vertices.ToArray();
			mesh.subMeshCount = TrianglesBySubmesh.Count;
			for (int i = 0; i < TrianglesBySubmesh.Count; i++)
				mesh.SetTriangles(TrianglesBySubmesh[i].ToArray(), i);

			//mesh.Optimize();
			mesh.RecalculateNormals();
			go.GetComponent<MeshFilter>().mesh = mesh;
			go.GetComponent<MeshRenderer>().materials = Materials.ToArray();

			DebugUtils.Log("Clearing meshBuilder for obj " + go.name);
			//Vertices.Clear();
			//TrianglesBySubmesh.Clear();
			//Materials.Clear();
		}
	}
}