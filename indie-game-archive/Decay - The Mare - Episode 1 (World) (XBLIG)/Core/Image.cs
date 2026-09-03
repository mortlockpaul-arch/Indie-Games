using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

public class Image : Object2D
{
	public enum FADE_STATE
	{
		IDLE,
		FADE_IN,
		FADE_OUT
	}

	public FADE_STATE m_fade_state;

	protected float m_fade_speed = 1.25f;

	public float m_alpha = 1f;

	public Texture2D m_texture;

	protected Rectangle m_dest_rect = Rectangle.Empty;

	public Vector2 m_center = Vector2.Zero;

	public float m_rotation;

	public Image(Texture2D texture, Rectangle dest_rect)
		: base("")
	{
		m_texture = texture;
		m_dest_rect = dest_rect;
	}

	public override void Clear()
	{
		m_texture = null;
		base.Clear();
	}

	public virtual void FadeIn(float speed)
	{
		m_fade_speed = speed;
		FadeIn();
	}

	public virtual void FadeIn()
	{
		m_fade_state = FADE_STATE.FADE_IN;
		m_alpha = 0f;
	}

	public virtual void FadeOut(float speed)
	{
		m_fade_speed = speed;
		FadeOut();
	}

	public virtual void FadeOut()
	{
		m_fade_state = FADE_STATE.FADE_OUT;
		m_alpha = 1f;
	}

	public override void Update(TimeSpan elapsed)
	{
		switch (m_fade_state)
		{
		case FADE_STATE.FADE_IN:
			m_alpha += (float)elapsed.TotalSeconds * m_fade_speed;
			if (m_alpha >= 1f)
			{
				m_alpha = 1f;
				m_fade_state = FADE_STATE.IDLE;
			}
			break;
		case FADE_STATE.FADE_OUT:
			m_alpha -= (float)elapsed.TotalSeconds * m_fade_speed;
			if (m_alpha <= 0f)
			{
				m_alpha = 0f;
				m_fade_state = FADE_STATE.IDLE;
			}
			break;
		}
	}

	public override void Draw(SpriteBatch SB, Color color)
	{
		if (SB != null && m_texture != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_texture, m_dest_rect, null, color * m_alpha, m_rotation, m_center, SpriteEffects.None, 0f);
			SB.End();
		}
	}
}
