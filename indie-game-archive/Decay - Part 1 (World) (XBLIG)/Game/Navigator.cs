using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game;

public class Navigator
{
	private enum NAVIGATOR_STATE
	{
		IDLE,
		FADE_OUT,
		FADE_IN
	}

	private NAVIGATOR_STATE m_state;

	private Color m_color;

	private float m_alpha;

	private Texture2D m_background;

	private Texture2D m_left;

	private Texture2D m_right;

	private Texture2D m_up;

	private Texture2D m_down;

	private bool m_left_enabled;

	private bool m_right_enabled;

	private bool m_up_enabled;

	private bool m_down_enabled;

	private Vector2 m_pos;

	private Game m_game;

	public Navigator(Game game)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		m_color = Color.White;
		base._002Ector();
		m_game = game;
		m_background = ((Game)m_game).Content.Load<Texture2D>("HUD/Navigator/arrow_bg");
		m_left = ((Game)m_game).Content.Load<Texture2D>("HUD/Navigator/arrow_west");
		m_right = ((Game)m_game).Content.Load<Texture2D>("HUD/Navigator/arrow_east");
		m_up = ((Game)m_game).Content.Load<Texture2D>("HUD/Navigator/arrow_north");
		m_down = ((Game)m_game).Content.Load<Texture2D>("HUD/Navigator/arrow_south");
		m_pos = new Vector2((float)(((Rectangle)(ref Game.TS_AREA)).Right - m_background.Width), (float)(((Rectangle)(ref Game.TS_AREA)).Bottom - m_background.Height));
		((Color)(ref m_color)).A = (byte)Math.Floor(m_alpha);
	}

	public virtual void Clear()
	{
		m_game = null;
		m_background = null;
		m_left = null;
		m_right = null;
		m_up = null;
		m_down = null;
	}

	public virtual void Setup(bool left, bool right, bool up, bool down)
	{
		m_left_enabled = left;
		m_right_enabled = right;
		m_up_enabled = up;
		m_down_enabled = down;
	}

	public void FadeOut()
	{
		if (m_state != NAVIGATOR_STATE.FADE_OUT)
		{
			m_alpha = 255f;
			((Color)(ref m_color)).A = (byte)Math.Round(m_alpha);
			m_state = NAVIGATOR_STATE.FADE_OUT;
		}
	}

	public void FadeIn()
	{
		m_alpha = 0f;
		((Color)(ref m_color)).A = (byte)Math.Round(m_alpha);
		m_state = NAVIGATOR_STATE.FADE_IN;
	}

	public virtual void Update(TimeSpan elapsed)
	{
		switch (m_state)
		{
		case NAVIGATOR_STATE.FADE_OUT:
			m_alpha -= (float)elapsed.TotalMilliseconds * 0.001f * 400f;
			if (m_alpha <= 0f)
			{
				m_alpha = 0f;
				m_state = NAVIGATOR_STATE.IDLE;
			}
			((Color)(ref m_color)).A = (byte)Math.Floor(m_alpha);
			break;
		case NAVIGATOR_STATE.FADE_IN:
			m_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 200f;
			if (m_alpha >= 255f)
			{
				m_alpha = 255f;
				m_state = NAVIGATOR_STATE.IDLE;
			}
			((Color)(ref m_color)).A = (byte)Math.Floor(m_alpha);
			break;
		case NAVIGATOR_STATE.IDLE:
			break;
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		SB.Begin((SpriteBlendMode)1);
		SB.Draw(m_background, m_pos, m_color);
		if (m_left_enabled)
		{
			SB.Draw(m_left, m_pos, m_color);
		}
		if (m_right_enabled)
		{
			SB.Draw(m_right, m_pos, m_color);
		}
		if (m_up_enabled)
		{
			SB.Draw(m_up, m_pos, m_color);
		}
		if (m_down_enabled)
		{
			SB.Draw(m_down, m_pos, m_color);
		}
		SB.End();
	}
}
