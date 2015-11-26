using System;

namespace Ct3dRenderer.Rendering
{
	public interface IChunkRenderer
	{
		void Render(Action onRendered);
	}
}