using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SGSCore;

namespace Game;

public class Credits
{
	private enum TEXT_STATE
	{
		WAIT,
		FADE_IN,
		SHOW,
		FADE_OUT
	}

	private Game m_game;

	private SpriteFont m_font;

	private SpriteFont m_font2;

	private Texture2D m_b_button;

	private float m_text_alpha;

	private int m_text_index;

	private TEXT_STATE m_text_state;

	private float m_timer;

	public Credits(Game game, SGSContentLoader CL)
	{
		m_game = game;
		m_font = CL.LoadFont("Fonts/SpriteFont2");
		m_font2 = CL.LoadFont("Fonts/SpriteFont1");
		m_b_button = CL.LoadTexture("HUD/b_button");
	}

	public virtual void Clear()
	{
		m_game = null;
		m_font = null;
		m_font2 = null;
	}

	public virtual void Reset()
	{
		m_text_index = 0;
		m_timer = 0f;
		m_text_state = TEXT_STATE.WAIT;
		m_text_alpha = 0f;
	}

	public virtual void Update(TimeSpan elapsed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		KeyboardState state = Keyboard.GetState();
		GamePadState state2 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadButtons buttons = ((GamePadState)(ref state2)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
		{
			if (!m_game.m_b_pressed)
			{
				m_game.m_b_pressed = true;
				m_game.onCreditsClosed();
			}
		}
		else
		{
			m_game.m_b_pressed = false;
		}
		switch (m_text_state)
		{
		case TEXT_STATE.WAIT:
			m_timer -= (float)elapsed.TotalSeconds;
			if (m_timer <= 0f)
			{
				m_text_state = TEXT_STATE.FADE_IN;
			}
			break;
		case TEXT_STATE.FADE_IN:
			m_text_alpha += (float)elapsed.TotalSeconds * 255f * 0.5f;
			if (m_text_alpha >= 255f)
			{
				m_text_alpha = 255f;
				m_text_state = TEXT_STATE.SHOW;
				m_timer = 5f;
			}
			break;
		case TEXT_STATE.SHOW:
			m_timer -= (float)elapsed.TotalSeconds;
			if (m_timer <= 0f)
			{
				m_text_state = TEXT_STATE.FADE_OUT;
			}
			break;
		case TEXT_STATE.FADE_OUT:
			m_text_alpha -= (float)elapsed.TotalSeconds * 255f * 0.5f;
			if (m_text_alpha <= 0f)
			{
				m_text_state = TEXT_STATE.FADE_IN;
				m_text_index++;
				if (m_text_index > 6)
				{
					m_text_index = 0;
				}
			}
			break;
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		if (SB != null)
		{
			string text = "";
			string text2 = "";
			switch (m_text_index)
			{
			case 0:
				text = "Developed By";
				text2 = "Shining Gate Software";
				break;
			case 1:
				text = "Programming & Design";
				text2 = "Fredrik Westlund";
				break;
			case 2:
				text = "Art, Music & Design";
				text2 = "Johannes Rae";
				break;
			case 3:
				text = "Lead QA";
				text2 = "Fredrik Westlund";
				break;
			case 4:
				text = "Additional QA";
				text2 = "Robert Ottone";
				break;
			case 5:
				text = "Additional Testing";
				text2 = "Anna-Maria Taawo";
				break;
			case 6:
				text = "Special Thanks";
				text2 = "Jenny Taawo";
				break;
			}
			Vector2 zero = Vector2.Zero;
			Vector2 zero2 = Vector2.Zero;
			SB.Begin((SpriteBlendMode)1);
			if (m_text_index == -1)
			{
				zero = m_font.MeasureString(text);
				((Vector2)(ref zero2))._002Ector(((float)Game.VIEW_RECT.Width - zero.X) / 2f, ((float)Game.VIEW_RECT.Height - zero.Y) / 2f);
				SB.DrawString(m_font, text, zero2, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Math.Round(m_text_alpha)));
			}
			else
			{
				zero = m_font.MeasureString(text);
				((Vector2)(ref zero2))._002Ector(((float)Game.VIEW_RECT.Width - zero.X) / 2f, (float)(Game.VIEW_RECT.Height / 2) - zero.Y);
				SB.DrawString(m_font, text, zero2, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Math.Round(m_text_alpha)));
				zero = m_font.MeasureString(text2);
				((Vector2)(ref zero2))._002Ector(((float)Game.VIEW_RECT.Width - zero.X) / 2f, (float)(Game.VIEW_RECT.Height / 2));
				SB.DrawString(m_font, text2, zero2, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Math.Round(m_text_alpha)));
			}
			zero = m_font2.MeasureString("BACK");
			zero2.X = (float)((Rectangle)(ref Game.TS_AREA)).Right - zero.X;
			zero2.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - zero.Y;
			SB.DrawString(m_font2, "BACK", zero2, Color.White);
			zero2.X -= (float)(m_b_button.Width + 10);
			SB.Draw(m_b_button, zero2, Color.White);
			SB.End();
		}
	}
}
