using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Ct3dRenderer.Utils;
using UnityEngine;

namespace Ct3dRenderer.Data
{
	// TODO: extract blockStore to separate object, so that loader can have more members and die after it's work is done
	public class CtScanChunkLoader : IChunkProvider, IBlockStore
	{
		private static readonly Regex _ctScanPhotoNameRegex = new Regex("\\d+\\.(png|jpg)");
		private delegate void BlockLoader(Color32 color32, ref Block block);
		public Block[,,] Blocks { get; private set; }
		public IntVector3 ChunkSize { get; private set; }
		public IntVector3 DataSize { get; private set; }
		public IntVector3 NumberOfChunks { get; private set; }
		private int _foundSelections;
		private int _zIndexOffset;
		private readonly string _scanPath;

		public CtScanChunkLoader(string scanPath)
		{
			_scanPath = scanPath;
            SetDataSizeAndZIndexOffset();
			SetChunkSize();
			SetNumberOfChunks();
			LoadLayers();
		}

		private void LoadLayers()
		{
			Stopwatch.Start("Loading scan data");
			Blocks = new Block[DataSize.x, DataSize.y, DataSize.z];
			var layerDirectories = Directory.GetDirectories(_scanPath);
			if (layerDirectories.Length == 0)
				LoadLayer(_scanPath);
			else
				foreach (var layerDirectory in layerDirectories)
					LoadLayer(layerDirectory);
			Stopwatch.StopCurrent();
		}

		private void SetChunkSize()
		{
			IntVector3 chunkSize = new IntVector3(30, 30, 30);
			for(int i=0; i<3; i++)
				chunkSize[i] = Math.Min(chunkSize[i], DataSize[i]);
			ChunkSize = chunkSize;
		}

		private void SetNumberOfChunks()
		{
			NumberOfChunks = new IntVector3
			{
				x = (int)Math.Ceiling(1d * DataSize.x / ChunkSize.x),
				y = (int)Math.Ceiling(1d * DataSize.y / ChunkSize.y),
				z = (int)Math.Ceiling(1d * DataSize.z / ChunkSize.z)
			};
		}

		private void SetDataSizeAndZIndexOffset()
		{
			var files = Directory.GetFiles(_scanPath, "*", SearchOption.AllDirectories);
			var fileIndices = files.Select(file => Path.GetFileName(file))
				.Where(name => IsCtScanPhoto(name))
				.Select(name => int.Parse(Path.GetFileNameWithoutExtension(name)))
				.ToArray();
			_zIndexOffset = fileIndices.Min();
			Texture2D sampleSlice = LoadTexture(files.First(name => IsCtScanPhoto(name)));
			DataSize = new IntVector3
			{ 
				z = fileIndices.Max() - fileIndices.Min() + 1,
				x = sampleSlice.width,
				y = sampleSlice.height,
			};
		}
		
		private void LoadLayer(string scanPath)
		{
			BlockLoader loadBlock = ChooseBlockLoader(scanPath);
			var files = Directory.GetFiles(scanPath, "*", SearchOption.TopDirectoryOnly)
				.Where(file => IsCtScanPhoto(Path.GetFileName(file)));
			foreach (var file in files)
			{
				int z = int.Parse(Path.GetFileNameWithoutExtension(file)) - _zIndexOffset;
				Color32[] pixels = LoadTexture(file).GetPixels32();

				for (int x = 0; x < DataSize.x; x++)
					for (int y = 0; y < DataSize.y; y++)
						loadBlock(pixels[x + y * DataSize.x], ref Blocks[x, y, z]);
			}
		}

		private BlockLoader ChooseBlockLoader(string scanPath)
		{
			var files = Directory.GetFiles(scanPath).Where(file => IsCtScanPhoto(Path.GetFileName(file))).ToList();
			files.Sort(StringComparer.OrdinalIgnoreCase);
			Texture2D sampleImage = LoadTexture(files[files.Count / 2]);
			Color bgColor = sampleImage.GetPixel(0, 0);
			if (bgColor.grayscale < 0.1)
				return (Color32 color, ref Block block) => block.XrayOpacity |= (byte) ((color.r + color.g + color.b)/3);
			else
			{
				int selectionIndex = _foundSelections;
				_foundSelections++;
                return (Color32 color, ref Block block) => block[selectionIndex] = (color.r + color.g + color.b) < 750;
			}
		}
		
		private static bool IsCtScanPhoto(String fileName)
		{
			return _ctScanPhotoNameRegex.IsMatch(fileName);
		}

		public IChunk GetChunk(IntVector3 position)
		{
			//todo: return null if chunk is mostly transparent
			return new CommonStoreBasedChunk(this, position);
		}

		private static Texture2D LoadTexture(string path)
		{
			byte[] bytes = File.ReadAllBytes(path);
			Texture2D texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
			texture.LoadImage(bytes);
			return texture;
		}
	}
}