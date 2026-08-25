using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class Atlas
{
	private List<Sprite> m_Sprites = new List<Sprite>();

	private RenderTarget2D m_AtlasData;

	private SpriteBatch m_Batcher;

	private ContentManager m_Content;

	private string m_Path;

	public Atlas(string AtlasPath, ContentManager content, SpriteBatch batcher)
	{
		m_Path = AtlasPath;
		batcher.GraphicsDevice.DeviceReset += GraphicsDevice_DeviceReset;
		m_Batcher = batcher;
		m_Content = content;
		InitTexture();
		XmlReaderSettings settings = new XmlReaderSettings
		{
			ConformanceLevel = ConformanceLevel.Fragment,
			IgnoreWhitespace = true,
			IgnoreComments = true
		};
		XmlReader xmlReader = XmlReader.Create(content.RootDirectory + "/" + AtlasPath + ".xml", settings);
		while (xmlReader.Read())
		{
			if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.AttributeCount > 3)
			{
				m_Sprites.Add(new Sprite(m_Sprites.Count, xmlReader.GetAttribute(1).ToString(), new Rectangle(int.Parse(xmlReader.GetAttribute(3).ToString()), int.Parse(xmlReader.GetAttribute(4).ToString()), int.Parse(xmlReader.GetAttribute(5).ToString()), int.Parse(xmlReader.GetAttribute(6).ToString())), this));
			}
		}
	}

	private void GraphicsDevice_DeviceReset(object sender, EventArgs e)
	{
		InitTexture();
	}

	private void InitTexture()
	{
		Texture2D texture2D = m_Content.Load<Texture2D>(m_Path);
		if (m_AtlasData == null)
		{
			m_AtlasData = new RenderTarget2D(m_Batcher.GraphicsDevice, texture2D.Width, texture2D.Height);
		}
		RenderTarget2D renderTarget = null;
		if (m_Batcher.GraphicsDevice.GetRenderTargets().Count() > 0)
		{
			renderTarget = (RenderTarget2D)m_Batcher.GraphicsDevice.GetRenderTargets()[0].RenderTarget;
		}
		m_Batcher.GraphicsDevice.SetRenderTarget(m_AtlasData);
		m_Batcher.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
		m_Batcher.Draw(texture2D, Vector2.Zero, Color.White);
		m_Batcher.End();
		m_Batcher.GraphicsDevice.SetRenderTarget(renderTarget);
	}

	public Sprite FindInAtlas(string name)
	{
		for (int i = 0; i < m_Sprites.Count; i++)
		{
			if (name + ".png" == m_Sprites[i].name)
			{
				return m_Sprites[i];
			}
		}
		return null;
	}

	public Sprite GetSprite(int id)
	{
		return m_Sprites[id];
	}

	public Texture2D GetTexture()
	{
		return m_AtlasData;
	}

	public void Draw(int SpriteId, Vector2 Position, Color color)
	{
		m_Batcher.Draw(m_AtlasData, Position, m_Sprites[SpriteId].rect, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
	}

	public void Draw(int SpriteId, Vector2 Position, SpriteEffects effect, float depth)
	{
		m_Batcher.Draw(m_AtlasData, Position, m_Sprites[SpriteId].rect, Color.White, 0f, Vector2.Zero, 1f, effect, depth);
	}

	public void Draw(int SpriteId, Vector2 Position, SpriteEffects effect, float depth, Color color)
	{
		m_Batcher.Draw(m_AtlasData, Position, m_Sprites[SpriteId].rect, color, 0f, Vector2.Zero, 1f, effect, depth);
	}

	public void Draw(int SpriteId, Vector2 Position, SpriteEffects effect, float depth, Color color, float rotation, Vector2 origin)
	{
		m_Batcher.Draw(m_AtlasData, Position, m_Sprites[SpriteId].rect, color, rotation, origin, 1f, effect, depth);
	}

	public void Draw(int SpriteId, Vector2 Position, SpriteEffects effect, float depth, Color color, float rotation, float scale)
	{
		m_Batcher.Draw(m_AtlasData, Position, m_Sprites[SpriteId].rect, color, rotation, Vector2.Zero, scale, effect, depth);
	}
}
