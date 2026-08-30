using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SheetEdit.TextureSheet;

public class XTexture
{
	public XSprite[] cell;

	public Texture2D texture;

	public string name = "";

	private int idx;

	public XTexture()
	{
		cell = new XSprite[128];
	}

	public XTexture(ContentManager Content, string path)
	{
		cell = new XSprite[128];
		texture = Content.Load<Texture2D>(path);
		Read(path + ".zsx");
	}

	public XTexture(ContentManager Content, BinaryReader reader, Texture2D tex)
	{
		cell = new XSprite[128];
		texture = tex;
		ReadData(reader);
	}

	public void Write(string path)
	{
		BinaryWriter binaryWriter = new BinaryWriter(File.Open(path, FileMode.Create, FileAccess.Write));
		for (int i = 0; i < cell.Length; i++)
		{
			if (cell[i] != null)
			{
				binaryWriter.Write(value: true);
				binaryWriter.Write(cell[i].name);
				binaryWriter.Write(cell[i].srcRect.X);
				binaryWriter.Write(cell[i].srcRect.Y);
				binaryWriter.Write(cell[i].srcRect.Width);
				binaryWriter.Write(cell[i].srcRect.Height);
				binaryWriter.Write(cell[i].origin.X);
				binaryWriter.Write(cell[i].origin.Y);
				binaryWriter.Write(cell[i].flags);
			}
			else
			{
				binaryWriter.Write(value: false);
			}
		}
		binaryWriter.Close();
	}

	public void Read(string path)
	{
		BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read));
		ReadData(binaryReader);
		binaryReader.Close();
	}

	public XTexture(string path, Texture2D tex)
	{
		BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read));
		ReadData(binaryReader);
		binaryReader.Close();
		texture = tex;
	}

	private void ReadData(BinaryReader reader)
	{
		for (int i = 0; i < cell.Length; i++)
		{
			if (reader.ReadBoolean())
			{
				cell[i] = new XSprite();
				cell[i].name = reader.ReadString();
				cell[i].srcRect = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
				cell[i].origin = new Vector2(reader.ReadSingle(), reader.ReadSingle());
				cell[i].flags = reader.ReadInt32();
			}
			else
			{
				cell[i] = null;
			}
		}
	}

	public int GetSpriteFlags(int idx)
	{
		if (idx < cell.Length && cell[idx] != null)
		{
			return cell[idx].flags;
		}
		return 0;
	}

	public Rectangle GetSpriteRect(int idx)
	{
		if (idx < cell.Length && cell[idx] != null)
		{
			return cell[idx].srcRect;
		}
		return default(Rectangle);
	}

	public string GetSpriteName(int idx)
	{
		if (idx < cell.Length && cell[idx] != null)
		{
			return cell[idx].name;
		}
		return "";
	}

	public Vector2 GetSpriteOrigin(int idx)
	{
		if (idx < cell.Length && cell[idx] != null)
		{
			return cell[idx].origin;
		}
		return default(Vector2);
	}

	public Vector2 GetRelativeSpriteOrigin(int idx)
	{
		if (idx < cell.Length && cell[idx] != null)
		{
			return cell[idx].origin - new Vector2(cell[idx].srcRect.X, cell[idx].srcRect.Y);
		}
		return default(Vector2);
	}

	public void SetSpriteRect(int idx, Rectangle rect)
	{
		if (idx < cell.Length)
		{
			if (cell[idx] == null)
			{
				cell[idx] = new XSprite();
			}
			cell[idx].srcRect = rect;
		}
	}

	public void SetSpriteOrigin(int idx, Vector2 origin)
	{
		if (idx < cell.Length)
		{
			if (cell[idx] == null)
			{
				cell[idx] = new XSprite();
			}
			cell[idx].origin = origin;
		}
	}
}
