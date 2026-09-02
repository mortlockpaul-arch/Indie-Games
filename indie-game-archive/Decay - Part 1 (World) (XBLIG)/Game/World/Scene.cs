using System;
using Microsoft.Xna.Framework.Graphics;

namespace Game.World;

public class Scene
{
	public enum FADE_STATE
	{
		IDLE,
		FADE_IN,
		FADE_OUT
	}

	public FADE_STATE m_fade_state;

	protected float m_fade_speed;

	public Color m_color;

	protected float m_alpha;

	public Texture2D m_texture;

	public Scene(Texture2D texture)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		m_fade_speed = 255f;
		m_color = Color.White;
		m_alpha = 255f;
		base._002Ector();
		m_texture = texture;
		((Color)(ref m_color)).A = (byte)Math.Round(m_alpha);
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
		((Color)(ref m_color)).A = (byte)m_alpha;
	}

	public virtual void FadeOut(float speed)
	{
		m_fade_speed = speed;
		FadeOut();
	}

	public virtual void FadeOut()
	{
		m_fade_state = FADE_STATE.FADE_OUT;
		m_alpha = 255f;
		((Color)(ref m_color)).A = (byte)m_alpha;
	}

	public virtual void Update(TimeSpan elapsed)
	{
		switch (m_fade_state)
		{
		case FADE_STATE.FADE_IN:
			m_alpha += (float)elapsed.TotalSeconds * m_fade_speed;
			if (m_alpha >= 255f)
			{
				m_alpha = 255f;
				m_fade_state = FADE_STATE.IDLE;
			}
			((Color)(ref m_color)).A = (byte)Math.Round(m_alpha);
			break;
		case FADE_STATE.FADE_OUT:
			m_alpha -= (float)elapsed.TotalSeconds * m_fade_speed;
			if (m_alpha <= 0f)
			{
				m_alpha = 255f;
				m_fade_state = FADE_STATE.IDLE;
			}
			((Color)(ref m_color)).A = (byte)Math.Round(m_alpha);
			break;
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (SB != null && m_texture != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_texture, Game.VIEW_RECT, m_color);
			SB.End();
		}
	}
}
