using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class LayerFadeFx : ScenaricEntitie
{
	private enum FADE_MODE
	{
		NONE,
		FADE_IN,
		FADE_OUT
	}

	private const float FREQUENCY = 10000f;

	private const float FADE_SPEED = 1f;

	public Texture2D m_Texture;

	public Vector2 Location;

	public int Width;

	public int Height;

	public Rectangle m_TextureSource;

	private SpriteBatch m_Batch;

	public Color m_TextureColor;

	private FADE_MODE m_FadeMode;

	private float m_FadeTimer;

	private float m_TriggerTime;

	private Random m_Random;

	public LayerFadeFx(SpriteBatch LocalBatch, Texture2D Sprite, int x, int y, string name, int seed)
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
		m_Random = new Random(seed);
		m_TriggerTime = (float)m_Random.NextDouble() * 10000f;
		InitEntity();
		m_TextureColor.A = 0;
		SetVisible(bVisible: false);
	}

	public void SetTextureColor(Color color)
	{
		m_TextureColor = color;
	}

	public override void Update(GameTime gameTime)
	{
		if (m_FadeMode != FADE_MODE.NONE)
		{
			m_FadeTimer += (float)gameTime.ElapsedGameTime.Milliseconds * 1f;
			if (m_FadeTimer < 1000f)
			{
				if (m_FadeMode == FADE_MODE.FADE_IN)
				{
					m_TextureColor.A = (byte)MathHelper.Lerp(0f, 255f, m_FadeTimer / 1000f);
				}
				else
				{
					m_TextureColor.A = (byte)MathHelper.Lerp(255f, 0f, m_FadeTimer / 1000f);
				}
				return;
			}
			if (m_FadeMode == FADE_MODE.FADE_IN)
			{
				m_TextureColor.A = byte.MaxValue;
			}
			else
			{
				m_TextureColor.A = 0;
				SetVisible(bVisible: false);
			}
			m_TriggerTime = (float)m_Random.NextDouble() * 10000f;
			m_FadeMode = FADE_MODE.NONE;
		}
		else
		{
			m_TriggerTime -= gameTime.ElapsedGameTime.Milliseconds;
			if (m_TriggerTime <= 0f)
			{
				m_TextureColor.A = byte.MaxValue;
				FadeOut();
			}
		}
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

	public void FadeIn()
	{
		m_FadeTimer = 0f;
		SetVisible(bVisible: true);
		m_FadeMode = FADE_MODE.FADE_IN;
	}

	public void FadeOut()
	{
		m_FadeTimer = 0f;
		SetVisible(bVisible: true);
		m_FadeMode = FADE_MODE.FADE_OUT;
	}

	public override void Draw()
	{
		if (m_bVisible)
		{
			m_Batch.Draw(m_Texture, Location, m_TextureSource, m_TextureColor, 0f, Vector2.Zero, 1f, m_SpriteEffect, m_zOrder);
		}
	}
}
