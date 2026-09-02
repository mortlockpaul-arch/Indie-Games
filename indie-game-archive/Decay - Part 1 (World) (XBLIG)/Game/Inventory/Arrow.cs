using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Inventory;

public class Arrow
{
	public enum ARROW_STATE
	{
		IDLE,
		ACTIVE
	}

	private ARROW_STATE m_state;

	private Texture2D m_idle_texture;

	private Texture2D m_active_texture;

	private bool m_flip;

	public float m_width;

	public float m_height;

	public Vector2 m_pos;

	public Arrow(Texture2D idle, Texture2D active, bool flip)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_pos = Vector2.Zero;
		base._002Ector();
		m_flip = flip;
		m_idle_texture = idle;
		m_active_texture = active;
		m_width = m_idle_texture.Width;
		m_height = m_idle_texture.Height;
	}

	public virtual void Clear()
	{
		m_idle_texture = null;
		m_active_texture = null;
	}

	public void SetState(ARROW_STATE state)
	{
		m_state = state;
	}

	public virtual void Update(TimeSpan elapsed)
	{
	}

	public virtual void Draw(SpriteBatch SB, Color color)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		SB.Begin((SpriteBlendMode)1);
		if (m_flip)
		{
			if (m_state == ARROW_STATE.IDLE)
			{
				SB.Draw(m_idle_texture, new Rectangle((int)m_pos.X, (int)m_pos.Y, m_idle_texture.Width, m_idle_texture.Height), (Rectangle?)null, color, 0f, Vector2.Zero, (SpriteEffects)1, 1f);
			}
			else
			{
				SB.Draw(m_active_texture, new Rectangle((int)m_pos.X, (int)m_pos.Y, m_idle_texture.Width, m_idle_texture.Height), (Rectangle?)null, color, 0f, Vector2.Zero, (SpriteEffects)1, 1f);
			}
		}
		else if (m_state == ARROW_STATE.IDLE)
		{
			SB.Draw(m_idle_texture, new Rectangle((int)m_pos.X, (int)m_pos.Y, m_idle_texture.Width, m_idle_texture.Height), (Rectangle?)null, color, 0f, Vector2.Zero, (SpriteEffects)0, 1f);
		}
		else
		{
			SB.Draw(m_active_texture, new Rectangle((int)m_pos.X, (int)m_pos.Y, m_idle_texture.Width, m_idle_texture.Height), (Rectangle?)null, color, 0f, Vector2.Zero, (SpriteEffects)0, 1f);
		}
		SB.End();
	}
}
