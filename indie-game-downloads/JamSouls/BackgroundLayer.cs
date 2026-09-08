using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class BackgroundLayer : ScenaricEntitie
{
	public Texture2D m_Texture;

	public Vector2 Location;

	public int Width;

	public int Height;

	public Rectangle m_TextureSource;

	private SpriteBatch m_Batch;

	public Color m_TextureColor;

	public BackgroundLayer(SpriteBatch LocalBatch, Texture2D Sprite, int x, int y, string name)
	{
		m_Texture = Sprite;
		Location.X = x;
		Location.Y = y;
		Width = m_Texture.Width;
		Height = m_Texture.Height;
		m_Batch = LocalBatch;
		Name = name;
		m_TextureColor = Color.White;
		TypeId = SCENARIC.TYPE_LAYER;
		m_TextureSource = new Rectangle(0, 0, Width, Height);
		InitEntity();
	}

	public override void Update(GameTime gameTime)
	{
	}

	public void SetTextureColor(Color color)
	{
		m_TextureColor = color;
	}

	public override void SetPosition(Vector2 pos)
	{
		Location = pos;
	}

	public Color GetColor()
	{
		return m_TextureColor;
	}

	public override Vector2 GetPosition()
	{
		return Location;
	}

	public override void Draw()
	{
		if (m_bVisible)
		{
			m_Batch.Draw(m_Texture, Location, m_TextureSource, m_TextureColor, 0f, Vector2.Zero, 1f, m_SpriteEffect, m_zOrder);
		}
	}
}
