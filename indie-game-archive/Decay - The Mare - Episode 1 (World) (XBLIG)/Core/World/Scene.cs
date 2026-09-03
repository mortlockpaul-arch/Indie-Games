using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core.World;

public class Scene
{
	public enum FADE_STATE
	{
		IDLE,
		FADE_IN,
		FADE_OUT
	}

	public FADE_STATE m_fade_state;

	protected float m_fade_speed = 1.25f;

	public Color m_color = Color.White;

	protected float m_alpha = 1f;

	public Texture2D m_texture;

	public Scene(Texture2D texture)
	{
		m_texture = texture;
	}

	public virtual void Clear()
	{
		m_texture = null;
	}

	public virtual float GetAlpha()
	{
		return m_alpha;
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

	public virtual void Update(TimeSpan elapsed)
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
				m_alpha = 1f;
				m_fade_state = FADE_STATE.IDLE;
			}
			break;
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
		if (SB != null && m_texture != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_texture, Game.VIEW_RECT, m_color * m_alpha);
			SB.End();
		}
	}
}
