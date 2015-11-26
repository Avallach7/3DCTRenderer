using Ct3dRenderer.Core;
using Ct3dRenderer.Data;
using UnityEngine;

namespace Ct3dRenderer.Rendering
{
	public interface IChunkRenderingAlgorithm
	{
		MeshBuilder CreateMesh(IChunk chunk);
	}
}