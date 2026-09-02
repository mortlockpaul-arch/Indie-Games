using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game;

public class AlphaAnimation : Animation2D
{
	private Color m_color;

	public AlphaAnimation(Game game, uint frames, bool reverse, Texture2D texture)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(game, "", frames, reverse);
		m_color = Color.White;
		m_texture = texture;
	}

	public override void Play()
	{
		base.Play();
		if (m_reverse)
		{
			((Color)(ref m_color)).A = 0;
		}
		else
		{
			((Color)(ref m_color)).A = byte.MaxValue;
		}
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
		float num = (float)m_current_frame / (float)m_num_frames;
		((Color)(ref m_color)).A = (byte)(255 - (int)(num * 255f));
	}

	public override void Draw(SpriteBatch SB)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_texture, Game.VIEW_RECT, m_color);
			SB.End();
		}
	}

	public override void Draw(SpriteBatch SB, Vector2 pos)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_texture, new Rectangle((int)pos.X, (int)pos.Y, m_texture.Width, m_texture.Height), m_color);
			SB.End();
		}
	}

	public override void Draw(SpriteBatch SB, Rectangle dest_rect, Rectangle source_rect, Color color)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_texture, dest_rect, (Rectangle?)source_rect, new Color(((Color)(ref color)).R, ((Color)(ref color)).G, ((Color)(ref color)).B, ((Color)(ref m_color)).A));
			SB.End();
		}
	}
}
