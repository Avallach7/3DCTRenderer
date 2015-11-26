using System;

namespace Ct3dRenderer.Data
{
	public interface IChunkProvider
	{
		IChunk GetChunk(IntVector3 position);
		IntVector3 NumberOfChunks { get; } //use -1 for unlimited amount of data in given dimension
	}
}