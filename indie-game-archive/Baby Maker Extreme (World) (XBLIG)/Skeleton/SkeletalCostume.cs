using System.Collections.Generic;
using System.Xml;
using ImportData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Renderer;

namespace Skeleton;

public class SkeletalCostume
{
	private List<CharacterSprite> sprites;

	private int m_iSpriteCount;

	public float Depth
	{
		get
		{
			return sprites[0].Depth;
		}
		set
		{
			for (int i = 0; i < m_iSpriteCount; i++)
			{
				sprites[i].Depth = value;
			}
		}
	}

	public int NumSprites => m_iSpriteCount;

	public SkeletalCostume()
	{
		sprites = new List<CharacterSprite>();
		m_iSpriteCount = 0;
	}

	public void SetColor(Color c)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		foreach (CharacterSprite sprite in sprites)
		{
			sprite.SetColor(c);
		}
	}

	public CharacterSprite AddSprite(Joint j, RenderSprite spr, int type)
	{
		CharacterSprite characterSprite = new CharacterSprite(spr, j, type);
		sprites.Add(characterSprite);
		m_iSpriteCount = sprites.Count;
		return characterSprite;
	}

	public void SetMouth(int i)
	{
		for (int j = 0; j < m_iSpriteCount; j++)
		{
			if (sprites[j].IsMouth())
			{
				sprites[j].SetTypedPose(i);
			}
		}
	}

	public int GetMouth()
	{
		for (int i = 0; i < m_iSpriteCount; i++)
		{
			if (sprites[i].IsMouth())
			{
				return sprites[i].GetTypedPose();
			}
		}
		return 0;
	}

	public void SetEyeAimAt(Vector2 v)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_iSpriteCount; i++)
		{
			if (sprites[i].IsEye())
			{
				sprites[i].SetEyePosition(v);
			}
		}
	}

	public Vector2 GetEyeAimAt()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_iSpriteCount; i++)
		{
			if (sprites[i].IsEye())
			{
				return sprites[i].GetEyePosition();
			}
		}
		return default(Vector2);
	}

	public void Update(TimeTracker gameTime, float mirrorPos)
	{
		for (int i = 0; i < m_iSpriteCount; i++)
		{
			sprites[i].Update(gameTime, mirrorPos);
		}
	}

	public void Draw(TimeTracker gameTime)
	{
		for (int i = 0; i < m_iSpriteCount; i++)
		{
			sprites[i].Draw(gameTime);
		}
	}

	public CharacterSprite GetFirstCharacterSprite(int id)
	{
		foreach (CharacterSprite sprite in sprites)
		{
			if (sprite.Child.GetJointId() == id)
			{
				return sprite;
			}
		}
		return sprites[0];
	}

	public void MirrorSprite(int i)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (i < m_iSpriteCount && i >= 0)
		{
			Vector2 surfaceScale = sprites[i].GetSprite().SurfaceScale;
			sprites[i].SurfaceScale = new Vector2(0f - surfaceScale.X, surfaceScale.Y);
		}
	}

	public void Rotate(int i, float angle)
	{
		if (i < m_iSpriteCount && i >= 0)
		{
			sprites[i].Rotation += angle;
		}
	}

	public void Move(int i, Vector2 dist)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (i < m_iSpriteCount && i >= 0)
		{
			CharacterSprite characterSprite = sprites[i];
			characterSprite.Offset += dist;
		}
	}

	public void Scale(int i, Vector2 amount)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (i < m_iSpriteCount && i >= 0)
		{
			CharacterSprite characterSprite = sprites[i];
			characterSprite.SurfaceScale += sprites[i].SurfaceScale * amount;
		}
	}

	public void DeleteSprite(int i)
	{
		if (i < m_iSpriteCount && i >= 0)
		{
			sprites.RemoveAt(i);
		}
		m_iSpriteCount = sprites.Count;
	}

	public float GetDepth(int i)
	{
		if (i < m_iSpriteCount && i >= 0)
		{
			return sprites[i].GetSprite().Depth;
		}
		return 0f;
	}

	public void ModDepth(int i, float f)
	{
		if (i < m_iSpriteCount && i >= 0)
		{
			sprites[i].GetSprite().Depth += f;
		}
	}

	public void Write(XmlWriter writer)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		foreach (CharacterSprite sprite in sprites)
		{
			writer.WriteStartElement("joint", "1");
			writer.WriteElementString("jId", sprite.Child.GetJointId().ToString());
			writer.WriteElementString("pageName", sprite.GetSprite().GetSpriteImage().GetSpritePage()
				.Name);
				int x = sprite.GetSprite().GetSpriteImage().GetPageRect()
					.X;
				writer.WriteElementString("boundingX", x.ToString());
				int y = sprite.GetSprite().GetSpriteImage().GetPageRect()
					.Y;
				writer.WriteElementString("boundingY", y.ToString());
				int width = sprite.GetSprite().GetSpriteImage().GetPageRect()
					.Width;
				writer.WriteElementString("boundingW", width.ToString());
				int height = sprite.GetSprite().GetSpriteImage().GetPageRect()
					.Height;
				writer.WriteElementString("boundingH", height.ToString());
				writer.WriteElementString("depth", sprite.GetSprite().Depth.ToString());
				float x2 = sprite.GetSprite().SurfaceScale.X;
				writer.WriteElementString("scaleX", x2.ToString());
				float y2 = sprite.GetSprite().SurfaceScale.Y;
				writer.WriteElementString("scaleY", y2.ToString());
				writer.WriteElementString("rotation", sprite.Rotation.ToString());
				float x3 = sprite.Offset.X;
				writer.WriteElementString("offsetX", x3.ToString());
				float y3 = sprite.Offset.Y;
				writer.WriteElementString("offsetY", y3.ToString());
				writer.WriteElementString("type", sprite.Type.ToString());
				writer.WriteEndElement();
			}
		}

		public void Read(CostumeImportData c, SkeletonManager skel)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			foreach (SkeletalCostumeRawData rawDatum in c.GetRawData())
			{
				RenderSprite sprite = SpriteManager.GetSprite(rawDatum.PageName, rawDatum.Bounding, default(Vector2), rawDatum.Depth);
				sprite.SurfaceScale = rawDatum.Scale;
				CharacterSprite characterSprite = AddSprite(skel.FindJointById(rawDatum.Id), sprite, rawDatum.Type);
				characterSprite.Offset = rawDatum.Offset;
				characterSprite.Rotation = rawDatum.Rotation;
			}
		}

		public void Read(XmlReader reader, SkeletonManager skel)
		{
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			reader.ReadStartElement("root");
			Rectangle pageCoords = default(Rectangle);
			Vector2 surfaceScale = default(Vector2);
			Vector2 offset = default(Vector2);
			while (reader.Name != "root")
			{
				reader.ReadStartElement("joint");
				int id = int.Parse(reader.ReadElementString("jId"));
				string pageName = reader.ReadElementString("pageName");
				((Rectangle)(ref pageCoords))._002Ector(int.Parse(reader.ReadElementString("boundingX")), int.Parse(reader.ReadElementString("boundingY")), int.Parse(reader.ReadElementString("boundingW")), int.Parse(reader.ReadElementString("boundingH")));
				float depth = float.Parse(reader.ReadElementString("depth"));
				((Vector2)(ref surfaceScale))._002Ector(float.Parse(reader.ReadElementString("scaleX")), float.Parse(reader.ReadElementString("scaleY")));
				float rotation = float.Parse(reader.ReadElementString("rotation"));
				((Vector2)(ref offset))._002Ector(float.Parse(reader.ReadElementString("offsetX")), float.Parse(reader.ReadElementString("offsetY")));
				int type = int.Parse(reader.ReadElementString("type"));
				reader.ReadEndElement();
				RenderSprite sprite = SpriteManager.GetSprite(pageName, pageCoords, default(Vector2), depth);
				sprite.SurfaceScale = surfaceScale;
				CharacterSprite characterSprite = AddSprite(skel.FindJointById(id), sprite, type);
				characterSprite.Offset = offset;
				characterSprite.Rotation = rotation;
			}
		}

		public void Clone(SkeletalCostume costume, SkeletonManager skel)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			foreach (CharacterSprite sprite2 in costume.sprites)
			{
				RenderSprite sprite = SpriteManager.GetSprite(sprite2.GetSprite().GetSpriteImage().GetSpritePage()
					.Name, sprite2.GetSprite().GetSpriteImage().GetPageRect(), default(Vector2), sprite2.GetSprite().Depth);
					sprite.SurfaceScale = sprite2.GetSprite().SurfaceScale;
					CharacterSprite characterSprite = AddSprite(skel.FindJointById(sprite2.Child.GetJointId()), sprite, sprite2.Type);
					characterSprite.Offset = sprite2.Offset;
					characterSprite.Rotation = sprite2.Rotation;
				}
			}
		}
