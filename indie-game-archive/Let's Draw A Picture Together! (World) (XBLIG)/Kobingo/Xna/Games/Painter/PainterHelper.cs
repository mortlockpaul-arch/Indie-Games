using System.IO;
using System.IO.Compression;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Games.Painter;

internal class PainterHelper
{
	public static Point Convert(Vector2 location)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return new Point((int)location.X, (int)location.Y);
	}

	public static void SavePictureToFile(Texture2D picture, string filepath)
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 5 });
		uint[] array = new uint[picture.Width * picture.Height];
		picture.GetData<uint>(array);
		MemoryStream memoryStream = new MemoryStream();
		using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
		{
			for (int i = 0; i < array.Length; i++)
			{
				binaryWriter.Write(array[i]);
			}
		}
		byte[] array2 = memoryStream.ToArray();
		using FileStream stream = new FileStream(filepath, FileMode.Create);
		using GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress);
		gZipStream.Write(array2, 0, array2.Length);
	}

	public static Texture2D LoadPictureFromFile(GraphicsDevice graphicsDevice, string filepath, int width, int height)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 5 });
		uint[] array = new uint[width * height];
		using (FileStream stream = new FileStream(filepath, FileMode.Open))
		{
			using GZipStream input = new GZipStream(stream, CompressionMode.Decompress);
			using BinaryReader binaryReader = new BinaryReader(input);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = binaryReader.ReadUInt32();
			}
		}
		Texture2D val = new Texture2D(graphicsDevice, width, height);
		val.SetData<uint>(array);
		return val;
	}
}
