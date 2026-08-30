using System.Collections.Generic;
using System.IO;
using SheetEdit.TextureSheet;

namespace MapEdit.map;

public class Layer
{
	public const int MID = 0;

	public const int MIDFORE = 1;

	public const int FORE = 2;

	public const int GLOWYBACK = 3;

	public const int COL = 4;

	public const int SCRIPT = 5;

	public const int SEG_COUNT = 256;

	public Seg[] seg;

	public float depth;

	public Layer()
	{
		seg = new Seg[256];
	}

	public void Write(BinaryWriter writer, Dictionary<string, XTexture> texture)
	{
		for (int i = 0; i < 256; i++)
		{
			Seg seg = this.seg[i];
			if (seg == null)
			{
				writer.Write(value: false);
				continue;
			}
			writer.Write(value: true);
			this.seg[i].Write(writer, texture);
		}
	}

	public void Read(BinaryReader reader, Dictionary<string, XTexture> texture)
	{
		for (int i = 0; i < 256; i++)
		{
			if (reader.ReadBoolean())
			{
				seg[i] = new Seg();
				_ = seg[i];
				seg[i].Read(reader, texture);
			}
			else
			{
				seg[i] = null;
			}
		}
	}

	public void RefreshDepths(Dictionary<string, XTexture> textures)
	{
		float num = depth;
		for (int i = 0; i < seg.Length; i++)
		{
			if (seg[i] != null)
			{
				seg[i].depth = num;
				if (textures[seg[i].texture].cell[seg[i].idx].flags == 4)
				{
					num += 0.05f;
				}
			}
		}
	}
}
