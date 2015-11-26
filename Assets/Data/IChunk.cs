namespace Ct3dRenderer.Data
{
	public interface IChunk
	{
		Block this[int x, int y, int z] { get; } // TODO: replace with method using IntVector3 
		IntVector3 Position { get; }
		IntVector3 Size { get; } // shared between all the chunks composing on scene, included here to ease iteration over blocks
	}
}