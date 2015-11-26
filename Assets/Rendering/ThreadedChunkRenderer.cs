using System;
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
		private IChunkRenderingAlgorithm[] _renderingAlgorithms;
        private MeshBuilder _updatedMesh;

		public static ThreadedChunkRenderer Create (IChunk chunk, IChunkRenderingAlgorithm[] renderingAlgorithms)
		{
			var self = Create();
			self._chunk = chunk;
			self._renderingAlgorithms = renderingAlgorithms;
			self.name = chunk.Position.ToString() + " | " + self.name;
			self.transform.position = chunk.Position * chunk.Size;
			//self.gameObject.hideFlags = HideFlags.HideInHierarchy;
			return self;
		}

		public void Render(Action onRendered)
		{
			//DebugUtils.Log("Creation of mesh for chunk " + _chunk.Position + ": queued");
			ThreadPool.QueueUserWorkItem(state =>
			{
				var updatedMesh = _renderingAlgorithms[1].CreateMesh(_chunk);
				updatedMesh.AssertIsValid();
				_updatedMesh = updatedMesh;
				_updatedMesh.Join(_renderingAlgorithms[0].CreateMesh(_chunk));
				onRendered();
			});
		}

		public void Update()
		{
			if (_updatedMesh != null)
			{
				_updatedMesh.AssertIsValid();
				_updatedMesh.Apply(this.gameObject);
				_updatedMesh = null;
				//DebugUtils.Log("Mesh for chunk " + _chunk.Position + " updated");
			}
		}
	}
}