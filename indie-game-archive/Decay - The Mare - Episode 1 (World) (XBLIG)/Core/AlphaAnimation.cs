using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

public class AlphaAnimation : Animation2D
{
	private float m_alpha;

	public AlphaAnimation(Game game, uint frames, bool reverse, Texture2D texture)
		: base(game, "", frames, reverse)
	{
		m_alpha = 1f;
		m_texture = texture;
	}

	public override void Play()
	{
		base.Play();
		if (m_reverse)
		{
			m_alpha = 0f;
		}
		else
		{
			m_alpha = 1f;
		}
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
		float num = (float)m_current_frame / (float)m_num_frames;
		m_alpha = 1f - num * 1f;
	}

	public override void Draw(SpriteBatch SB)
	{
		if (m_texture != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_texture, Game.VIEW_RECT, Color.White * m_alpha);
			SB.End();
		}
	}

	public override void Draw(SpriteBatch SB, Vector2 pos)
	{
		if (m_texture != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_texture, new Rectangle((int)pos.X, (int)pos.Y, m_texture.Width, m_texture.Height), Color.White * m_alpha);
			SB.End();
		}
	}

	public override void Draw(SpriteBatch SB, Rectangle dest_rect, Rectangle source_rect, Color color)
	{
		if (m_texture != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_texture, dest_rect, source_rect, color * m_alpha);
			SB.End();
		}
	}
}
