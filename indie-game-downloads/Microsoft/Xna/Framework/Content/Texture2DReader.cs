using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content;

internal class Texture2DReader : ContentTypeReader<Texture2D>
{
	internal Texture2DReader()
	{
	}

	protected internal override Texture2D Read(ContentReader reader, Texture2D existingInstance)
	{
		Texture2D texture2D = null;
		SurfaceFormat surfaceFormat = ((reader.version >= 5) ? ((SurfaceFormat)reader.ReadInt32()) : (reader.ReadInt32() switch
		{
			1 => SurfaceFormat.ColorBgraEXT, 
			28 => SurfaceFormat.Dxt1, 
			30 => SurfaceFormat.Dxt3, 
			32 => SurfaceFormat.Dxt5, 
			_ => throw new NotSupportedException("Unsupported legacy surface format."), 
		}));
		int num = reader.ReadInt32();
		int num2 = reader.ReadInt32();
		int num3 = reader.ReadInt32();
		int num4 = num3;
		GraphicsDevice graphicsDevice = reader.ContentManager.GetGraphicsDevice();
		SurfaceFormat surfaceFormat2 = surfaceFormat;
		if (surfaceFormat == SurfaceFormat.Dxt1 && FNA3D.FNA3D_SupportsDXT1(graphicsDevice.GLDevice) == 0)
		{
			surfaceFormat2 = SurfaceFormat.Color;
		}
		else if ((surfaceFormat == SurfaceFormat.Dxt3 || surfaceFormat == SurfaceFormat.Dxt5) && FNA3D.FNA3D_SupportsS3TC(graphicsDevice.GLDevice) == 0)
		{
			surfaceFormat2 = SurfaceFormat.Color;
		}
		texture2D = ((existingInstance != null) ? existingInstance : new Texture2D(graphicsDevice, num, num2, num4 > 1, surfaceFormat2));
		for (int i = 0; i < num3; i++)
		{
			int num5 = reader.ReadInt32();
			byte[] buffer = null;
			int width = num >> i;
			int height = num2 >> i;
			if (i >= num4)
			{
				continue;
			}
			if (reader.platform == 'x')
			{
				if (surfaceFormat == SurfaceFormat.Color || surfaceFormat == SurfaceFormat.ColorBgraEXT)
				{
					buffer = X360TexUtil.SwapColor(reader.ReadBytes(num5));
					num5 = buffer.Length;
				}
				else
				{
					switch (surfaceFormat)
					{
					case SurfaceFormat.Dxt1:
						buffer = X360TexUtil.SwapDxt1(reader.ReadBytes(num5), width, height);
						num5 = buffer.Length;
						break;
					case SurfaceFormat.Dxt3:
						buffer = X360TexUtil.SwapDxt3(reader.ReadBytes(num5), width, height);
						num5 = buffer.Length;
						break;
					case SurfaceFormat.Dxt5:
						buffer = X360TexUtil.SwapDxt5(reader.ReadBytes(num5), width, height);
						num5 = buffer.Length;
						break;
					}
				}
			}
			if (surfaceFormat2 != surfaceFormat)
			{
				if (buffer == null)
				{
					buffer = reader.ReadBytes(num5);
				}
				switch (surfaceFormat)
				{
				case SurfaceFormat.Dxt1:
					buffer = DxtUtil.DecompressDxt1(buffer, width, height);
					break;
				case SurfaceFormat.Dxt3:
					buffer = DxtUtil.DecompressDxt3(buffer, width, height);
					break;
				case SurfaceFormat.Dxt5:
					buffer = DxtUtil.DecompressDxt5(buffer, width, height);
					break;
				}
				num5 = buffer.Length;
			}
			int startIndex = 0;
			if (buffer == null)
			{
				if (reader.BaseStream is MemoryStream && ((MemoryStream)reader.BaseStream).TryGetBuffer(out buffer))
				{
					startIndex = (int)reader.BaseStream.Seek(0L, SeekOrigin.Current);
					reader.BaseStream.Seek(num5, SeekOrigin.Current);
				}
				else
				{
					buffer = reader.ReadBytes(num5);
				}
			}
			texture2D.SetData(i, null, buffer, startIndex, num5);
		}
		return texture2D;
	}
}
