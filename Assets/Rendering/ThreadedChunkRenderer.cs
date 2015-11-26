using System.Threading;
using Ct3dRenderer.Data;
using Ct3dRenderer.Utils;
using UnityEngine;

namespace Ct3dRenderer.Rendering
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class ThreadedChunkRenderer : UnityObject<ThreadedChunkRenderer>, IChunkRenderer
	{
		private IChunk _chunk;
		private IChunkRenderingAlgorithm _renderingAlgorithm; // TODO: support for multiple at once
		private MeshBuilder _updatedMesh;

		public static ThreadedChunkRenderer Create (IChunk chunk, IChunkRenderingAlgorithm renderingAlgorithm)
		{
			var self = Create();
			self._chunk = chunk;
			self._renderingAlgorithm = renderingAlgorithm;
			self.name = chunk.Position.ToString() + " | " + self.name;
			self.transform.position = chunk.Position * chunk.Size;
			//self.gameObject.hideFlags = HideFlags.HideInHierarchy;
			return self;
		}

		public void Render()
		{
			DebugUtils.Log("Creation of mesh for chunk " + _chunk.Position + ": queued");
			ThreadPool.QueueUserWorkItem(state =>
			{
				var s = new Stopwatch("Creation of mesh for chunk " + _chunk.Position);
				_updatedMesh = _renderingAlgorithm.CreateMesh(_chunk);
				_updatedMesh.materialsCount = _updatedMesh.Materials.Count;
                if (_updatedMesh.Materials.Count == 0)
				DebugUtils.Log("Created mesh has no materials in queue!!! " + _chunk.Position);
				DebugUtils.Log("Creation of mesh for chunk " + _chunk.Position + ": " + _updatedMesh.Materials.Count + " materials");
				s.Stop();
			});
		}

		public void Update()
		{
			if (_updatedMesh != null)
			{
				if (_updatedMesh.Materials.Count == 0)
				DebugUtils.Log("Created mesh has no materials in update!!! " + _chunk.Position);
				_updatedMesh.Apply(this.gameObject);
				_updatedMesh = null;
				DebugUtils.Log("Mesh for chunk " + _chunk.Position + " updated");
			}
		}
	}
}