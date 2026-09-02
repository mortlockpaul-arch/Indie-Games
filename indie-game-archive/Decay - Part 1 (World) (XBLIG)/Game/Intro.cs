using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SGSCore;

namespace Game;

public class Intro
{
	private enum INTRO_STATE
	{
		WAIT,
		FADE_IN_LOGO,
		SHOW_LOGO,
		FADE_OUT_LOGO,
		FADE_IN_PRESENTS,
		SHOW_PRESENTS,
		FADE_OUT_PRESENTS,
		FADE_IN_PROGRAMMING,
		SHOW_PROGRAMMING,
		FADE_OUT_PROGRAMMING,
		FADE_IN_ART,
		SHOW_ART,
		FADE_OUT_ART,
		WAIT_FADE_OUT,
		FADE_OUT
	}

	protected Game m_game;

	protected Texture2D m_logo;

	protected Movie m_movie;

	protected float m_alpha;

	private SpriteFont m_font;

	private INTRO_STATE m_state;

	protected float m_timer;

	protected string m_text = "";

	protected bool m_fade_in_part;

	public Intro(Game game, SGSContentLoader CL)
	{
		m_game = game;
		m_logo = CL.LoadTexture("Intro/sg_logo");
		m_font = CL.LoadFont("Fonts/SpriteFont2");
		m_movie = new Movie();
		m_movie.Load(CL, "Intro/intro");
	}

	public void Start()
	{
		m_state = INTRO_STATE.FADE_IN_LOGO;
		m_alpha = 0f;
	}

	public virtual void Clear()
	{
		m_game = null;
		m_logo = null;
		m_movie = null;
		m_font = null;
	}

	public virtual void Update(TimeSpan elapsed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		KeyboardState state = Keyboard.GetState();
		GamePadState state2 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadButtons buttons = ((GamePadState)(ref state2)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).A != 1)
		{
			GamePadState state3 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons2 = ((GamePadState)(ref state3)).Buttons;
			if ((int)((GamePadButtons)(ref buttons2)).Start != 1 && !((KeyboardState)(ref state)).IsKeyDown((Keys)65))
			{
				m_game.m_a_pressed = false;
				goto IL_0100;
			}
		}
		if (!m_game.m_a_pressed)
		{
			m_game.m_a_pressed = true;
			if (m_state == INTRO_STATE.FADE_IN_LOGO || m_state == INTRO_STATE.SHOW_LOGO || m_state == INTRO_STATE.FADE_OUT_LOGO)
			{
				m_alpha = 0f;
				m_state = INTRO_STATE.FADE_IN_PRESENTS;
				m_movie.Play();
				m_movie.m_video_player.Volume = m_game.m_game_settings.m_sound_volume * 0.1f;
				m_text = "Shining Gate Software\n\rPresents";
			}
			else
			{
				m_movie.m_video_player.Stop();
				m_game.onIntroFinished();
			}
			return;
		}
		goto IL_0100;
		IL_0100:
		switch (m_state)
		{
		case INTRO_STATE.WAIT:
			return;
		case INTRO_STATE.FADE_IN_LOGO:
			m_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 256f;
			if (m_alpha >= 255f)
			{
				m_alpha = 255f;
				m_state = INTRO_STATE.SHOW_LOGO;
				m_timer = 3f;
			}
			return;
		case INTRO_STATE.SHOW_LOGO:
			m_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_timer <= 0f)
			{
				m_timer = 0f;
				m_state = INTRO_STATE.FADE_OUT_LOGO;
			}
			return;
		case INTRO_STATE.FADE_OUT_LOGO:
			m_alpha -= (float)elapsed.TotalMilliseconds * 0.001f * 128f;
			if (m_alpha <= 0f)
			{
				m_alpha = 0f;
				m_state = INTRO_STATE.FADE_IN_PRESENTS;
				m_movie.Play();
				m_text = "Shining Gate Software\n\rPresents";
			}
			return;
		case INTRO_STATE.FADE_IN_PRESENTS:
			m_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 256f;
			if (m_alpha >= 255f)
			{
				m_alpha = 255f;
				m_state = INTRO_STATE.SHOW_PRESENTS;
				m_timer = 6f;
			}
			break;
		case INTRO_STATE.SHOW_PRESENTS:
			m_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_timer <= 0f)
			{
				m_timer = 0f;
				m_state = INTRO_STATE.FADE_OUT_PRESENTS;
			}
			break;
		case INTRO_STATE.FADE_OUT_PRESENTS:
			m_alpha -= (float)elapsed.TotalMilliseconds * 0.001f * 512f;
			if (m_alpha <= 0f)
			{
				m_alpha = 0f;
				m_state = INTRO_STATE.FADE_IN_PROGRAMMING;
				m_text = "Programming & Design by\n\rFredrik Westlund";
				m_timer = 20f;
			}
			break;
		case INTRO_STATE.FADE_IN_PROGRAMMING:
			m_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_timer <= 0f)
			{
				m_timer = 0f;
				m_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 256f;
				if (m_alpha >= 255f)
				{
					m_alpha = 255f;
					m_state = INTRO_STATE.SHOW_PROGRAMMING;
					m_timer = 3f;
				}
			}
			break;
		case INTRO_STATE.SHOW_PROGRAMMING:
			m_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_timer <= 0f)
			{
				m_timer = 0f;
				m_state = INTRO_STATE.FADE_OUT_PROGRAMMING;
			}
			break;
		case INTRO_STATE.FADE_OUT_PROGRAMMING:
			m_alpha -= (float)elapsed.TotalMilliseconds * 0.001f * 512f;
			if (m_alpha <= 0f)
			{
				m_alpha = 0f;
				m_state = INTRO_STATE.FADE_IN_ART;
				m_text = "Art, Music & Design by\n\rJohannes Rae";
				m_timer = 0.5f;
			}
			break;
		case INTRO_STATE.FADE_IN_ART:
			m_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_timer <= 0f)
			{
				m_timer = 0f;
				m_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 256f;
				if (m_alpha >= 255f)
				{
					m_alpha = 255f;
					m_state = INTRO_STATE.SHOW_ART;
					m_timer = 3f;
				}
			}
			break;
		case INTRO_STATE.SHOW_ART:
			m_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_timer <= 0f)
			{
				m_timer = 0f;
				m_state = INTRO_STATE.FADE_OUT_ART;
			}
			break;
		case INTRO_STATE.FADE_OUT_ART:
			m_alpha -= (float)elapsed.TotalMilliseconds * 0.001f * 512f;
			if (m_alpha <= 0f)
			{
				m_alpha = 0f;
				m_state = INTRO_STATE.WAIT_FADE_OUT;
				m_text = "";
				m_timer = 0f;
			}
			break;
		case INTRO_STATE.WAIT_FADE_OUT:
			if ((m_movie.m_video.Duration - m_movie.m_video_player.PlayPosition).TotalMilliseconds <= 3000.0)
			{
				m_state = INTRO_STATE.FADE_OUT;
			}
			break;
		case INTRO_STATE.FADE_OUT:
		{
			float volume = m_movie.m_video_player.Volume;
			volume -= (float)elapsed.TotalMilliseconds * 0.001f * 0.5f;
			if (volume < 0f)
			{
				volume = 0f;
			}
			m_movie.m_video_player.Volume = volume;
			break;
		}
		}
		if ((m_movie.m_video.Duration - m_movie.m_video_player.PlayPosition).TotalMilliseconds <= 4500.0)
		{
			m_fade_in_part = true;
			m_text = "PART I";
		}
		if (m_fade_in_part)
		{
			m_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 256f;
			if (m_alpha >= 255f)
			{
				m_alpha = 255f;
			}
		}
		if (m_movie != null && (int)m_movie.m_video_player.State == 0)
		{
			m_movie.m_video_player.Stop();
			m_game.onIntroFinished();
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		switch (m_state)
		{
		case INTRO_STATE.WAIT:
			return;
		case INTRO_STATE.FADE_IN_LOGO:
		case INTRO_STATE.SHOW_LOGO:
		case INTRO_STATE.FADE_OUT_LOGO:
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_logo, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Math.Round(m_alpha)));
			SB.End();
			return;
		}
		if (m_movie != null)
		{
			m_movie.Draw(SB, Color.White);
		}
		if (m_text != "")
		{
			Vector2 val = m_font.MeasureString(m_text);
			Vector2 zero = Vector2.Zero;
			if (!m_fade_in_part)
			{
				zero.X = (float)((Rectangle)(ref Game.TS_AREA)).Right - val.X;
				zero.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val.Y;
			}
			else
			{
				zero.X = ((float)Game.VIEW_RECT.Width - val.X) / 2f;
				zero.Y = Game.VIEW_RECT.Height - 170;
			}
			SB.Begin((SpriteBlendMode)1);
			SB.DrawString(m_font, m_text, new Vector2(zero.X + 1f, zero.Y + 2f), new Color((byte)0, (byte)0, (byte)0, (byte)Math.Round(m_alpha)));
			SB.DrawString(m_font, m_text, zero, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Math.Round(m_alpha)));
			SB.End();
		}
	}
}
