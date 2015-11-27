using System;
using Ct3dRenderer.Data;
using Ct3dRenderer.Rendering;
using Ct3dRenderer.Utils;
using UnityEngine;

namespace Ct3dRenderer.Core
{
	public class Visualisation : UnityObject<Visualisation>
	{
		private IChunkProvider _chunkProvider;
		private IChunkRenderingAlgorithm[] _renderingAlgorithms = 
		{
			new SelectionChunkRenderingAlgorithm(),
			new XrayChunkRenderingAlgorithm()
		};
		private int _renderedChunks;
		private int _chunksToRender;

		public override void Start()
		{
			base.Start();
			var meshGenStopwatch = new Stopwatch("Mesh generation");
			for (int x = 0; x < _chunkProvider.NumberOfChunks.x; x++)
				for (int y = 0; y < _chunkProvider.NumberOfChunks.y; y++)
					for (int z = 0; z < _chunkProvider.NumberOfChunks.z; z++)
					{
						ThreadedChunkRenderer chunkRenderer = 
							ThreadedChunkRenderer.Create(_chunkProvider.GetChunk(new IntVector3(x, y, z)), _renderingAlgorithms);
						chunkRenderer.transform.parent = this.transform;
						Action onRendered = () =>
						{
							_renderedChunks ++;
							if (_renderedChunks == _chunksToRender)
								meshGenStopwatch.Stop();
						};
						chunkRenderer.Render(onRendered);
					}
			transform.localScale = new Vector3(1, 1, 15);
			transform.rotation = Quaternion.Euler(270, 0, 0);
		}
		
		public static Visualisation Create(IChunkProvider chunkProvider)
		{
			var self = Create();
			self._chunkProvider = chunkProvider;
			self._chunksToRender = chunkProvider.NumberOfChunks.x * 
								   chunkProvider.NumberOfChunks.y *
								   chunkProvider.NumberOfChunks.z;
            return self;
		}
	}
}