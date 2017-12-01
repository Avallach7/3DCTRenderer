using System;
using System.Threading;
using Ct3dRenderer.Data;
using Ct3dRenderer.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ct3dRenderer.Rendering
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class ThreadedChunkRenderer : UnityObject<ThreadedChunkRenderer>, IChunkRenderer
	{
		private IChunk _chunk;
		private IChunkRenderingAlgorithm[] _renderingAlgorithms;
        private MeshBuilder _updatedMesh;
		const bool Multithreaded = false;

		public static ThreadedChunkRenderer Create (IChunk chunk, IChunkRenderingAlgorithm[] renderingAlgorithms)
		{
			var self = Create();
			self._chunk = chunk;
			self._renderingAlgorithms = renderingAlgorithms;
			self.name = chunk.Position.ToString() + " | " + self.name;
			self.transform.position = chunk.Position * chunk.Size;
			self.SimplifyRendering();
			//self.gameObject.hideFlags = HideFlags.HideInHierarchy;
			return self;
		}

		private void SimplifyRendering()
		{
			var meshRenderer = GetComponent<MeshRenderer>();
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;
			meshRenderer.useLightProbes = false;
			meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
		}

		public void Render(Action onRendered)
		{
			//DebugUtils.Log("Creation of mesh for chunk " + _chunk.Position + ": queued");
			WaitCallback renderAction = state =>
			{
				var updatedMesh = new MeshBuilder();
				foreach (var renderingAlgorithm in _renderingAlgorithms)
				{
					updatedMesh.Join(renderingAlgorithm.CreateMesh(_chunk));
					updatedMesh.AssertIsValid();
				}
				_updatedMesh = updatedMesh;
				onRendered();
			};
			if (Multithreaded)
				ThreadPool.QueueUserWorkItem(renderAction);
			else
				renderAction(null);
		}

		public void Update()
		{
			if (_updatedMesh != null)
			{
				_updatedMesh.AssertIsValid();
				_updatedMesh.Apply(this.gameObject);
				_updatedMesh = null;
			}
		}
	}
}