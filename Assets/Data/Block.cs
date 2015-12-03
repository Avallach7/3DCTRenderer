namespace Ct3dRenderer.Data
{
	public struct Block
	{
		private const byte MaxSelections = 5;
		// 0    32   64   96   128  160  192  224
		// 0.00 0.14 0.29 0.43 0.57 0.71 0.86 1.00
		public const byte OpacityMask = 224; //Convert.ToByte("11100000", 2);
		private byte _data;

		public byte XrayOpacity
		{
			get { return (byte) (_data & OpacityMask); }
			set { _data = (byte) (value & OpacityMask); }
		}

		public bool this[int selectionId]
		{
			get { return (_data & (1 << (selectionId%MaxSelections))) != 0; }
			set
			{
				_data = value
					? (byte) (_data | (1 << (selectionId%MaxSelections)))
					: (byte) (_data & ~(1 << (selectionId%MaxSelections)));
			}
		}

		public byte Data //TODO: remove that dirty performance hack?
		{
			get { return _data; }
		}
	}
}