namespace Ct3dRenderer.Data
{
	public class CommonStoreBasedChunk : IChunk
	{
		private readonly IBlockStore _commonStore;
		public IntVector3 Position { get; private set; }
		public IntVector3 Size { get { return _commonStore.ChunkSize; } }
		private readonly bool _isOnEdge; //prevents unneeded bounds checks

		public CommonStoreBasedChunk(IBlockStore commonStore, IntVector3 position)
		{
			_commonStore = commonStore;
			Position = position;
			_isOnEdge = position.x == 0 || (position.x*_commonStore.ChunkSize.x >= _commonStore.DataSize.x) ||
				        position.y == 0 || (position.y*_commonStore.ChunkSize.y >= _commonStore.DataSize.y) ||
				        position.z == 0 || (position.z*_commonStore.ChunkSize.z >= _commonStore.DataSize.z);
		}

		public Block this[int p_x, int p_y, int p_z]
		{
			get
			{
				int x = p_x + Position.x * _commonStore.ChunkSize.x;
				int y = p_y + Position.y * _commonStore.ChunkSize.y;
				int z = p_z + Position.z * _commonStore.ChunkSize.z;
				if (_isOnEdge && 
				       (0 > x || x >= _commonStore.DataSize.x ||
					    0 > y || y >= _commonStore.DataSize.y ||
					    0 > z || z >= _commonStore.DataSize.z))
						return default(Block);
				return _commonStore.Blocks[x, y, z];
			}
		}

		public override string ToString()
		{
			return "Chunk{Pos=" + Position + ",...}";
		}
	}
}