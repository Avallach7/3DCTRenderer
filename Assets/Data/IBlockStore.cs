namespace Ct3dRenderer.Data
{
	public interface IBlockStore
	{
		Block[,,] Blocks { get; }
		IntVector3 ChunkSize { get; }
		IntVector3 DataSize { get; }
	}
}