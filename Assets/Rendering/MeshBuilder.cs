using System;
using System.Collections.Generic;
using System.Linq;
using Ct3dRenderer.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ct3dRenderer.Rendering
{
	public class MeshBuilder
	{
		public List<Vector3> Vertices { get; private set; }
		public List<List<int>> TrianglesBySubmesh { get; private set; }
		public List<Material> Materials { get; private set; }

		public MeshBuilder()
		{
			Vertices = new List<Vector3>();
			TrianglesBySubmesh = new List<List<int>>();
			Materials = new List<Material>();
		}

		public void AssertIsValid()
		{
			//if (Materials.Count != TrianglesBySubmesh.Count)
			//	throw new Ct3dRendererException("Materials.Count(" + Materials.Count + ") is not equal to TrianglesBySubmesh.Count(" +
			//									TrianglesBySubmesh.Count + ")"); //fires randomly: 3 != 1
		}

		// can be only called from main thread!
		public void Apply(GameObject go)
		{
			AssertIsValid();
            if (TrianglesBySubmesh.Sum(s => s.Count) < 30)
			{
				//go.SetActive(false);
				Object.Destroy(go);
				return;
			}
			var mesh = new Mesh
			{
				vertices = Vertices.ToArray(),
				subMeshCount = TrianglesBySubmesh.Count
			};
			//UnityEngine.Assertions.Assert.AreEqual(mesh.subMeshCount, TrianglesBySubmesh.Count, "Assignment failed: mesh.subMeshCount = TrianglesBySubmesh.Count");
			for (int i = 0; i < TrianglesBySubmesh.Count; i++)
			{
				//if (mesh.subMeshCount != TrianglesBySubmesh.Count)
				//	throw new Ct3dRendererException("Assignment failed: mesh.subMeshCount("+ mesh.subMeshCount+ ") != TrianglesBySubmesh.Count(" + TrianglesBySubmesh.Count + ")"); //fires randomly: 7 != 1
				//UnityEngine.Assertions.Assert.AreEqual(mesh.subMeshCount, TrianglesBySubmesh.Count, "mesh.subMeshCount != TrianglesBySubmesh.Count");
				//UnityEngine.Assertions.Assert.IsTrue(i < mesh.subMeshCount, "i="+i+ ", mesh.subMeshCount="+ mesh.subMeshCount + ", TrianglesBySubmesh.Count=" + TrianglesBySubmesh.Count);
				mesh.SetTriangles(TrianglesBySubmesh[i].ToArray(), i); //Failed setting triangles. Submesh index is out of bounds. UnityEngine.Mesh:SetTriangles(Int32[], Int32)
			}

			//mesh.Optimize();
			mesh.RecalculateNormals();
			go.GetComponent<MeshFilter>().mesh = mesh;
			go.GetComponent<MeshRenderer>().sharedMaterials = Materials.ToArray();
		}

		public void Clear()
		{
			Vertices = new List<Vector3>();
			TrianglesBySubmesh = new List<List<int>>();
			Materials = new List<Material>();
		}

		public void Join(MeshBuilder other)
		{
			AssertIsValid();
			TrianglesBySubmesh.AddRange(other.TrianglesBySubmesh.Select(s => s.Select(v => v+Vertices.Count).ToList()));
			Vertices.AddRange(other.Vertices);
			Materials.AddRange(other.Materials);
			AssertIsValid();
		}
	}
}