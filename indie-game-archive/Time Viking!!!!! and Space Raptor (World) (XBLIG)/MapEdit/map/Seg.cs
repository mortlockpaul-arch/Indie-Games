using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using SheetEdit.TextureSheet;

namespace MapEdit.map;

public class Seg
{
	public Vector2 loc;

	public int idx;

	public string texture;

	public float rotation;

	public Vector2 scaling;

	public Vector3 cVec;

	public int flag;

	public string clothes;

	public string food;

	public float depth;

	internal void CopyFrom(Seg seg)
	{
		if (seg != null)
		{
			try
			{
				loc = seg.loc;
				idx = seg.idx;
				texture = seg.texture;
				rotation = seg.rotation;
				scaling = seg.scaling;
				cVec = seg.cVec;
				clothes = seg.clothes;
				food = seg.food;
				flag = seg.flag;
			}
			catch
			{
			}
		}
	}

	internal void Read(BinaryReader reader, Dictionary<string, XTexture> t)
	{
		idx = reader.ReadInt32();
		loc = new Vector2(reader.ReadSingle(), reader.ReadSingle());
		rotation = reader.ReadSingle();
		scaling = new Vector2(reader.ReadSingle(), reader.ReadSingle());
		texture = reader.ReadString();
		cVec = new Vector3(-1f, -1f, -1f);
	}

	internal void Write(BinaryWriter writer, Dictionary<string, XTexture> t)
	{
		writer.Write(idx);
		writer.Write(loc.X);
		writer.Write(loc.Y);
		writer.Write(rotation);
		writer.Write(scaling.X);
		writer.Write(scaling.Y);
		writer.Write(texture);
	}
}
