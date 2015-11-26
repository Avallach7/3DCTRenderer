using Ct3dRenderer.Data;
using Ct3dRenderer.Rendering;
using Ct3dRenderer.Utils;

namespace Ct3dRenderer.Core
{
	public class Scene : UnityObject<Scene>
	{
		private IChunkProvider _chunkProvider;
		private IChunkRenderingAlgorithm _renderingAlgorithm = new XrayChunkRenderingAlgorithm();

		public override void Start()
		{
			base.Start();
			Stopwatch.Start("Queueing chunk mesh generation");
			for (int x = 0; x < _chunkProvider.NumberOfChunks.x; x++)
				for (int y = 0; y < _chunkProvider.NumberOfChunks.y; y++)
					for (int z = 0; z < _chunkProvider.NumberOfChunks.z; z++)
					{
						IChunkRenderer chunkRenderer = ThreadedChunkRenderer.Create(_chunkProvider.GetChunk(new IntVector3(x, y, z)), _renderingAlgorithm); 
						chunkRenderer.Render();
					}
			Stopwatch.StopCurrent();
		}
		
		public static Scene Create(IChunkProvider chunkProvider)
		{
			var self = Create();
			self._chunkProvider = chunkProvider;
			return self;
		}
	}
}