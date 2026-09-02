using System;
using Game.Inventory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game;

public class HUD
{
	public enum HUD_STATE
	{
		NONE,
		NAVIGATOR,
		BACK,
		CANCEL,
		PROCEED,
		MOVE_FORWARD
	}

	public enum HUD_FADE_STATE
	{
		NONE,
		FADE_OUT,
		FADE_IN_BACK,
		FADE_IN_CANCEL,
		FADE_IN_PROCEED,
		FADE_IN_MOVE_FORWARD
	}

	private enum HUD_TEXT_STATE
	{
		VISIBLE,
		INVISIBLE,
		FADE_IN,
		FADE_OUT
	}

	public HUD_STATE m_state;

	public HUD_FADE_STATE m_fade_state;

	public Navigator m_navigator;

	private Texture2D m_a_button;

	private Texture2D m_b_button;

	private Texture2D m_y_button;

	private Texture2D m_dpad;

	private Animation2D m_LS_animation;

	private Animation2D m_LT_animation;

	private Animation2D m_RT_animation;

	private TextureAnimation m_move_forward_animation;

	private Game m_game;

	public SpriteFont m_font;

	public SpriteFont m_font2;

	public Texture2D m_fade;

	public Color m_color;

	private float m_alpha;

	private string m_text;

	private string m_ask_title;

	private string m_ask_text1;

	private string m_ask_text2;

	private HUD_TEXT_STATE m_text_state;

	private float m_text_alpha;

	private bool m_text_fade;

	private string m_text_event;

	private string m_show_ask_a_event;

	private string m_show_ask_b_event;

	public HUD(Game game)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_state = HUD_STATE.NAVIGATOR;
		m_color = Color.White;
		m_alpha = 255f;
		m_text = "";
		m_ask_title = "";
		m_ask_text1 = "";
		m_ask_text2 = "";
		m_text_state = HUD_TEXT_STATE.INVISIBLE;
		m_text_event = "";
		m_show_ask_a_event = "";
		m_show_ask_b_event = "";
		base._002Ector();
		m_game = game;
		m_font = m_game.m_CL.LoadFont("Fonts/SpriteFont1");
		m_font2 = m_game.m_CL.LoadFont("Fonts/SpriteFont2");
		m_a_button = m_game.m_CL.LoadTexture("HUD/a_button");
		m_b_button = m_game.m_CL.LoadTexture("HUD/b_button");
		m_y_button = m_game.m_CL.LoadTexture("HUD/y_button");
		m_dpad = m_game.m_CL.LoadTexture("HUD/dpad");
		m_LS_animation = new TextureAnimation(m_game, m_game.m_CL, "HUD/LeftStick/", 36u, reverse: false);
		m_LS_animation.SetFPS(15.0);
		m_LS_animation.Play(Animation2D.LOOP_TYPE.CYCLE);
		m_LT_animation = new TextureAnimation(m_game, m_game.m_CL, "HUD/LeftTrigger/", 11u, reverse: false);
		m_LT_animation.SetFPS(15.0);
		m_LT_animation.Play(Animation2D.LOOP_TYPE.CYCLE);
		m_RT_animation = new TextureAnimation(m_game, m_game.m_CL, "HUD/RightTrigger/", 11u, reverse: false);
		m_RT_animation.SetFPS(15.0);
		m_RT_animation.Play(Animation2D.LOOP_TYPE.CYCLE);
		m_move_forward_animation = new TextureAnimation(m_game, m_game.m_CL, "HUD/LeftStick/", 28u, reverse: false);
		m_move_forward_animation.SetFPS(5.0);
		m_navigator = new Navigator(m_game);
		m_fade = m_game.m_CL.LoadTexture("HUD/black");
	}

	public virtual void Clear()
	{
		m_game = null;
		m_font = null;
		m_font2 = null;
		if (m_navigator != null)
		{
			m_navigator.Clear();
			m_navigator = null;
		}
		if (m_a_button != null)
		{
			((GraphicsResource)m_a_button).Dispose();
			m_a_button = null;
		}
		if (m_b_button != null)
		{
			((GraphicsResource)m_b_button).Dispose();
			m_b_button = null;
		}
		if (m_y_button != null)
		{
			((GraphicsResource)m_y_button).Dispose();
			m_y_button = null;
		}
		if (m_LS_animation != null)
		{
			m_LS_animation.Clear();
			m_LS_animation = null;
		}
		if (m_LT_animation != null)
		{
			m_LT_animation.Clear();
			m_LT_animation = null;
		}
		if (m_RT_animation != null)
		{
			m_RT_animation.Clear();
			m_RT_animation = null;
		}
		m_fade = null;
	}

	public virtual void FadeOut()
	{
		switch (m_state)
		{
		case HUD_STATE.BACK:
		case HUD_STATE.CANCEL:
		case HUD_STATE.PROCEED:
		case HUD_STATE.MOVE_FORWARD:
			m_alpha = 255f;
			((Color)(ref m_color)).A = (byte)Math.Round(m_alpha);
			m_fade_state = HUD_FADE_STATE.FADE_OUT;
			break;
		case HUD_STATE.NAVIGATOR:
			m_navigator.FadeOut();
			break;
		}
	}

	public virtual void ClearAlpha()
	{
		m_alpha = 255f;
		((Color)(ref m_color)).A = (byte)Math.Round(m_alpha);
	}

	public virtual void FadeIn()
	{
		switch (m_state)
		{
		case HUD_STATE.PROCEED:
			m_alpha = 0f;
			((Color)(ref m_color)).A = (byte)Math.Round(m_alpha);
			m_fade_state = HUD_FADE_STATE.FADE_IN_PROCEED;
			break;
		case HUD_STATE.MOVE_FORWARD:
			m_alpha = 0f;
			((Color)(ref m_color)).A = (byte)Math.Round(m_alpha);
			m_fade_state = HUD_FADE_STATE.FADE_IN_MOVE_FORWARD;
			if (m_move_forward_animation != null)
			{
				m_move_forward_animation.Play(Animation2D.LOOP_TYPE.CYCLE);
				m_move_forward_animation.SetFrame(17);
				m_move_forward_animation.m_start_frame = 17;
				m_move_forward_animation.m_end_frame = 27;
			}
			break;
		case HUD_STATE.BACK:
			m_alpha = 0f;
			((Color)(ref m_color)).A = (byte)Math.Round(m_alpha);
			m_fade_state = HUD_FADE_STATE.FADE_IN_BACK;
			break;
		case HUD_STATE.CANCEL:
			m_alpha = 0f;
			((Color)(ref m_color)).A = (byte)Math.Round(m_alpha);
			m_fade_state = HUD_FADE_STATE.FADE_IN_CANCEL;
			break;
		case HUD_STATE.NAVIGATOR:
			m_navigator.FadeIn();
			break;
		}
	}

	public virtual void ShowText(string text, string text_event, bool fade)
	{
		m_text_fade = fade;
		m_text_event = text_event;
		ShowText(text);
	}

	public virtual void ShowText(string text, bool fade)
	{
		m_text_fade = fade;
		ShowText(text);
	}

	protected virtual void ShowText(string text)
	{
		m_text = text;
		m_text_alpha = 255f;
		m_text_state = HUD_TEXT_STATE.VISIBLE;
		m_game.m_show_cursor = false;
		m_game.m_state = Game.GAME_STATE.SHOW_TEXT;
	}

	public virtual void ShowAsk(string title, string a_text, string b_text, bool fade)
	{
		ShowAsk(title, a_text, b_text, "", "", fade);
	}

	public virtual void ShowAsk(string title, string a_text, string b_text, string a_event, string b_event, bool fade)
	{
		m_text_fade = fade;
		m_ask_title = title;
		m_ask_text1 = a_text;
		m_ask_text2 = b_text;
		m_text_alpha = 255f;
		m_text_state = HUD_TEXT_STATE.VISIBLE;
		m_game.m_show_cursor = false;
		m_game.m_state = Game.GAME_STATE.SHOW_ASK;
		m_game.m_input_enabled = false;
		m_game.m_inventory_enabled = false;
		if (m_game.m_inventory != null)
		{
			m_game.m_inventory.m_state = global::Game.Inventory.Inventory.INVENTORY_STATE.DISABLED;
		}
		m_show_ask_a_event = a_event;
		m_show_ask_b_event = b_event;
	}

	public virtual void HideText()
	{
		if (m_text_event != "")
		{
			string text_event = m_text_event;
			m_text_event = "";
			m_game.HandleEvent(text_event);
			return;
		}
		m_text_alpha = 255f;
		m_text_state = HUD_TEXT_STATE.INVISIBLE;
		m_game.m_show_cursor = true;
		m_game.m_state = Game.GAME_STATE.SCENE;
		m_text_fade = false;
		FadeIn();
	}

	public virtual void HideAsk()
	{
		m_text_alpha = 255f;
		m_text_state = HUD_TEXT_STATE.INVISIBLE;
		m_game.m_show_cursor = true;
		m_game.m_state = Game.GAME_STATE.SCENE;
		m_game.m_input_enabled = true;
		m_game.m_inventory_enabled = true;
		m_text_fade = false;
		FadeIn();
	}

	public virtual void Update(TimeSpan elapsed)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Invalid comparison between Unknown and I4
		if (m_move_forward_animation != null)
		{
			m_move_forward_animation.Update(elapsed);
		}
		if (m_game.m_state == Game.GAME_STATE.SHOW_ASK)
		{
			KeyboardState state = Keyboard.GetState();
			GamePadState state2 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons = ((GamePadState)(ref state2)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)65))
			{
				if (!m_game.m_a_pressed)
				{
					m_game.m_a_pressed = true;
					HideAsk();
					if (m_show_ask_a_event != "")
					{
						m_game.HandleEvent(m_show_ask_a_event);
					}
				}
			}
			else
			{
				m_game.m_a_pressed = false;
			}
			GamePadState state3 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons2 = ((GamePadState)(ref state3)).Buttons;
			if ((int)((GamePadButtons)(ref buttons2)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
			{
				if (!m_game.m_b_pressed)
				{
					m_game.m_b_pressed = true;
					HideAsk();
					if (m_show_ask_b_event != "")
					{
						m_game.HandleEvent(m_show_ask_b_event);
					}
				}
			}
			else
			{
				m_game.m_b_pressed = false;
			}
		}
		if (m_fade_state != HUD_FADE_STATE.NONE)
		{
			switch (m_fade_state)
			{
			case HUD_FADE_STATE.FADE_OUT:
				m_alpha -= (float)elapsed.TotalMilliseconds * 0.001f * 400f;
				if (m_alpha <= 0f)
				{
					m_alpha = 0f;
					m_state = HUD_STATE.NONE;
					m_fade_state = HUD_FADE_STATE.NONE;
				}
				((Color)(ref m_color)).A = (byte)Math.Round(m_alpha);
				break;
			case HUD_FADE_STATE.FADE_IN_BACK:
			case HUD_FADE_STATE.FADE_IN_CANCEL:
			case HUD_FADE_STATE.FADE_IN_PROCEED:
			case HUD_FADE_STATE.FADE_IN_MOVE_FORWARD:
				m_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 400f;
				if (m_alpha >= 255f)
				{
					m_alpha = 255f;
					m_fade_state = HUD_FADE_STATE.NONE;
				}
				((Color)(ref m_color)).A = (byte)Math.Round(m_alpha);
				break;
			}
			return;
		}
		if (m_navigator != null)
		{
			m_navigator.Update(elapsed);
		}
		switch (m_text_state)
		{
		case HUD_TEXT_STATE.FADE_IN:
			m_text_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 400f;
			if (m_text_alpha >= 255f)
			{
				m_text_alpha = 255f;
				m_text_state = HUD_TEXT_STATE.VISIBLE;
			}
			break;
		case HUD_TEXT_STATE.FADE_OUT:
			m_text_alpha -= (float)elapsed.TotalMilliseconds * 0.001f * 400f;
			if (m_text_alpha <= 0f)
			{
				m_text_alpha = 0f;
				m_text_state = HUD_TEXT_STATE.INVISIBLE;
			}
			break;
		}
		if (m_game.m_state == Game.GAME_STATE.TUTORIAL)
		{
			switch (m_game.m_tutorial_state)
			{
			case Game.TUTORIAL_STATE.MOVE_CURSOR:
				m_LS_animation.Update(elapsed);
				break;
			case Game.TUTORIAL_STATE.DECREASE_SPEED:
				m_LT_animation.Update(elapsed);
				m_LS_animation.Update(elapsed);
				break;
			case Game.TUTORIAL_STATE.INCREASE_SPEED:
				m_RT_animation.Update(elapsed);
				m_LS_animation.Update(elapsed);
				break;
			case Game.TUTORIAL_STATE.CHANGE_VIEW:
				m_navigator.Update(elapsed);
				break;
			}
		}
	}

	public virtual void DrawTutorial(SpriteBatch SB)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Unknown result type (might be due to invalid IL or missing references)
		switch (m_game.m_tutorial_state)
		{
		case Game.TUTORIAL_STATE.MOVE_CURSOR:
		{
			string text7 = "Use the Left Stick to move the cursor.";
			Vector2 zero4 = Vector2.Zero;
			Vector2 val7 = m_font2.MeasureString(text7);
			float num3 = (float)(m_LS_animation.m_width + 10) + val7.X;
			zero4.X = ((float)Game.VIEW_RECT.Width - num3) / 2f;
			zero4.Y = ((Rectangle)(ref Game.TS_AREA)).Top;
			m_LS_animation.Draw(SB, zero4, m_color);
			zero4.X += (float)(m_LS_animation.m_width + 10);
			zero4.Y += 25f;
			SB.Begin((SpriteBlendMode)1);
			Color black4 = Color.Black;
			((Color)(ref black4)).A = (byte)Math.Round(m_alpha);
			SB.DrawString(m_font2, text7, new Vector2(zero4.X + 1f, zero4.Y + 2f), black4);
			SB.DrawString(m_font2, text7, zero4, m_color);
			SB.End();
			break;
		}
		case Game.TUTORIAL_STATE.DECREASE_SPEED:
		{
			string text5 = "Use the Left Trigger to slow down the cursor.";
			string text6 = "+";
			Vector2 zero3 = Vector2.Zero;
			Vector2 val5 = m_font2.MeasureString(text5);
			Vector2 val6 = m_font2.MeasureString(text6);
			float num2 = (float)m_LT_animation.m_width + val6.X + (float)m_LS_animation.m_width + 10f + val5.X;
			zero3.X = ((float)Game.VIEW_RECT.Width - num2) / 2f;
			zero3.Y = ((Rectangle)(ref Game.TS_AREA)).Top;
			m_LT_animation.Draw(SB, zero3, m_color);
			zero3.X += (float)(m_LT_animation.m_width + 10);
			zero3.Y += 25f;
			SB.Begin((SpriteBlendMode)1);
			Color black3 = Color.Black;
			((Color)(ref black3)).A = (byte)Math.Round(m_alpha);
			SB.DrawString(m_font2, text6, new Vector2(zero3.X + 1f, zero3.Y + 2f), black3);
			SB.DrawString(m_font2, text6, zero3, m_color);
			SB.End();
			zero3.X += val6.X;
			zero3.Y -= 25f;
			m_LS_animation.Draw(SB, zero3, m_color);
			zero3.X += (float)(m_LS_animation.m_width + 10);
			zero3.Y += 25f;
			SB.Begin((SpriteBlendMode)1);
			black3 = Color.Black;
			((Color)(ref black3)).A = (byte)Math.Round(m_alpha);
			SB.DrawString(m_font2, text5, new Vector2(zero3.X + 1f, zero3.Y + 2f), black3);
			SB.DrawString(m_font2, text5, zero3, m_color);
			SB.End();
			break;
		}
		case Game.TUTORIAL_STATE.INCREASE_SPEED:
		{
			string text3 = "Use the Right Trigger to speed up the cursor.";
			string text4 = "+";
			Vector2 zero2 = Vector2.Zero;
			Vector2 val3 = m_font2.MeasureString(text3);
			Vector2 val4 = m_font2.MeasureString(text4);
			float num = (float)m_RT_animation.m_width + val4.X + (float)m_LS_animation.m_width + 10f + val3.X;
			zero2.X = ((float)Game.VIEW_RECT.Width - num) / 2f;
			zero2.Y = ((Rectangle)(ref Game.TS_AREA)).Top;
			m_RT_animation.Draw(SB, zero2, m_color);
			zero2.X += (float)(m_RT_animation.m_width + 10);
			zero2.Y += 25f;
			SB.Begin((SpriteBlendMode)1);
			Color black2 = Color.Black;
			((Color)(ref black2)).A = (byte)Math.Round(m_alpha);
			SB.DrawString(m_font2, text4, new Vector2(zero2.X + 1f, zero2.Y + 2f), black2);
			SB.DrawString(m_font2, text4, zero2, m_color);
			SB.End();
			zero2.X += val4.X;
			zero2.Y -= 25f;
			m_LS_animation.Draw(SB, zero2, m_color);
			zero2.X += (float)(m_LS_animation.m_width + 10);
			zero2.Y += 25f;
			SB.Begin((SpriteBlendMode)1);
			black2 = Color.Black;
			((Color)(ref black2)).A = (byte)Math.Round(m_alpha);
			SB.DrawString(m_font2, text3, new Vector2(zero2.X + 1f, zero2.Y + 2f), black2);
			SB.DrawString(m_font2, text3, zero2, m_color);
			SB.End();
			break;
		}
		case Game.TUTORIAL_STATE.CHANGE_VIEW:
		{
			string text = "Down to the right you can see the navigation icon.";
			string text2 = "Use the Directional Pad to turn around.";
			Vector2 zero = Vector2.Zero;
			Vector2 val = m_font2.MeasureString(text);
			Vector2 val2 = m_font2.MeasureString(text2);
			float x = val.X;
			zero.X = ((float)Game.VIEW_RECT.Width - x) / 2f;
			zero.Y = ((Rectangle)(ref Game.TS_AREA)).Top;
			SB.Begin((SpriteBlendMode)1);
			Color black = Color.Black;
			((Color)(ref black)).A = (byte)Math.Round(m_alpha);
			SB.DrawString(m_font2, text, new Vector2(zero.X + 1f, zero.Y + 2f), black);
			SB.DrawString(m_font2, text, zero, m_color);
			x = (float)(m_dpad.Width + 10) + val2.X;
			zero.X = ((float)Game.VIEW_RECT.Width - x) / 2f - 5f;
			zero.Y += 25f;
			SB.Draw(m_dpad, zero, m_color);
			zero.X += (float)(m_dpad.Width + 10);
			zero.Y += 20f;
			black = Color.Black;
			((Color)(ref black)).A = (byte)Math.Round(m_alpha);
			SB.DrawString(m_font2, text2, new Vector2(zero.X + 1f, zero.Y + 2f), black);
			SB.DrawString(m_font2, text2, zero, m_color);
			SB.End();
			m_navigator.Draw(SB);
			break;
		}
		}
	}

	protected virtual void DrawText(SpriteBatch SB)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		SB.Begin((SpriteBlendMode)1);
		if (m_text_fade)
		{
			SB.Draw(m_fade, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)192));
		}
		Vector2 val = m_font2.MeasureString(m_text);
		Vector2 zero = Vector2.Zero;
		zero.X = ((float)Game.VIEW_RECT.Width - val.X) / 2f;
		zero.Y = (float)(((Rectangle)(ref Game.TS_AREA)).Bottom - m_a_button.Height - 20) - val.Y;
		Color val2 = Color.Black;
		((Color)(ref val2)).A = (byte)Math.Round(m_text_alpha);
		SB.DrawString(m_font2, m_text, new Vector2(zero.X + 1f, zero.Y + 2f), val2);
		val2 = Color.White;
		((Color)(ref val2)).A = (byte)Math.Round(m_text_alpha);
		SB.DrawString(m_font2, m_text, zero, val2);
		if (m_text_alpha >= 255f)
		{
			float num = m_a_button.Width;
			zero.X = ((float)Game.VIEW_RECT.Width - num) / 2f;
			zero.Y = ((Rectangle)(ref Game.TS_AREA)).Bottom - m_a_button.Height;
			SB.Draw(m_a_button, zero, Color.White);
		}
		SB.End();
	}

	protected virtual void DrawAsk(SpriteBatch SB)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (m_text_fade)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_fade, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)192));
			SB.End();
		}
		Vector2 val = m_font2.MeasureString(m_ask_title);
		Vector2 zero = Vector2.Zero;
		zero.X = ((float)Game.VIEW_RECT.Width - val.X) / 2f;
		zero.Y = (float)(((Rectangle)(ref Game.TS_AREA)).Bottom - m_a_button.Height - 20) - val.Y;
		SB.Begin((SpriteBlendMode)1);
		SB.DrawString(m_font2, m_ask_title, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
		SB.DrawString(m_font2, m_ask_title, zero, Color.White);
		SB.End();
		Vector2 val2 = m_font.MeasureString(m_ask_text1);
		Vector2 val3 = m_font.MeasureString(m_ask_text2);
		float num = 10f;
		float num2 = 40f;
		float num3 = (float)m_a_button.Width + num + (float)(int)val2.X + num2 + (float)m_b_button.Width + num + (float)(int)val3.X;
		zero.X = (int)((float)Game.VIEW_RECT.Width - num3) / 2;
		zero.Y = ((Rectangle)(ref Game.TS_AREA)).Bottom - m_a_button.Height;
		SB.Begin((SpriteBlendMode)1);
		SB.Draw(m_a_button, zero, Color.White);
		zero.X += (float)m_a_button.Width + num;
		SB.DrawString(m_font, m_ask_text1, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
		SB.DrawString(m_font, m_ask_text1, zero, Color.White);
		zero.X += (float)(int)val2.X + num2;
		SB.Draw(m_b_button, zero, Color.White);
		zero.X += (float)m_b_button.Width + num;
		SB.DrawString(m_font, m_ask_text2, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
		SB.DrawString(m_font, m_ask_text2, zero, Color.White);
		SB.End();
	}

	public virtual void Draw(SpriteBatch SB)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		if (m_game.m_state == Game.GAME_STATE.TUTORIAL)
		{
			DrawTutorial(SB);
		}
		else if (m_game.m_state == Game.GAME_STATE.ASK_TUTORIAL)
		{
			string text = "PLAY TUTORIAL";
			string text2 = "SKIP TUTORIAL";
			Vector2 zero = Vector2.Zero;
			Vector2 val = m_font.MeasureString(text);
			Vector2 val2 = m_font.MeasureString(text2);
			float num = (float)(m_a_button.Width + 10) + val.X + 40f + (float)m_b_button.Width + 10f + val2.X;
			zero.X = ((float)Game.VIEW_RECT.Width - num) / 2f;
			zero.Y = ((Rectangle)(ref Game.TS_AREA)).Top;
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_a_button, zero, m_color);
			zero.X += (float)(m_a_button.Width + 10);
			Color black = Color.Black;
			((Color)(ref black)).A = (byte)Math.Round(m_alpha);
			SB.DrawString(m_font, text, new Vector2(zero.X + 1f, zero.Y + 2f), black);
			SB.DrawString(m_font, text, zero, m_color);
			zero.X += val.X + 40f;
			SB.Draw(m_b_button, zero, m_color);
			zero.X += (float)(m_b_button.Width + 10);
			black = Color.Black;
			((Color)(ref black)).A = (byte)Math.Round(m_alpha);
			SB.DrawString(m_font, text2, new Vector2(zero.X + 1f, zero.Y + 2f), black);
			SB.DrawString(m_font, text2, zero, m_color);
			SB.End();
		}
		else if (m_text_state != HUD_TEXT_STATE.INVISIBLE)
		{
			switch (m_game.m_state)
			{
			case Game.GAME_STATE.SHOW_TEXT:
				DrawText(SB);
				break;
			case Game.GAME_STATE.SHOW_ASK:
				DrawAsk(SB);
				break;
			}
		}
		else
		{
			if (m_game.m_state == Game.GAME_STATE.SHOW_TEXT || m_game.m_state == Game.GAME_STATE.SHOW_ASK)
			{
				return;
			}
			switch (m_state)
			{
			case HUD_STATE.NAVIGATOR:
				if (m_navigator != null)
				{
					m_navigator.Draw(SB);
				}
				break;
			case HUD_STATE.BACK:
			case HUD_STATE.CANCEL:
			case HUD_STATE.PROCEED:
				if (m_b_button != null)
				{
					SB.Begin((SpriteBlendMode)1);
					string text3 = "";
					if (m_state == HUD_STATE.BACK)
					{
						text3 = "BACK";
					}
					if (m_state == HUD_STATE.CANCEL)
					{
						text3 = "CANCEL";
					}
					Vector2 val3 = m_font.MeasureString(text3);
					Vector2 zero3 = Vector2.Zero;
					zero3.X = (float)((Rectangle)(ref Game.TS_AREA)).Right - val3.X;
					zero3.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val3.Y;
					Color black2 = Color.Black;
					((Color)(ref black2)).A = ((Color)(ref m_color)).A;
					SB.DrawString(m_font, text3, new Vector2(zero3.X + 1f, zero3.Y + 2f), black2);
					SB.DrawString(m_font, text3, zero3, m_color);
					if (m_state == HUD_STATE.PROCEED)
					{
						zero3.X = ((Rectangle)(ref Game.TS_AREA)).Right - m_a_button.Width;
						zero3.Y = ((Rectangle)(ref Game.TS_AREA)).Bottom - m_a_button.Height;
						SB.Draw(m_a_button, zero3, m_color);
					}
					else
					{
						zero3.X -= (float)(m_b_button.Width + 10);
						SB.Draw(m_b_button, zero3, m_color);
					}
					SB.End();
				}
				break;
			case HUD_STATE.MOVE_FORWARD:
				if (m_move_forward_animation != null)
				{
					Vector2 zero2 = Vector2.Zero;
					zero2.X = ((Rectangle)(ref Game.TS_AREA)).Right - m_move_forward_animation.m_width;
					zero2.Y = ((Rectangle)(ref Game.TS_AREA)).Bottom - m_move_forward_animation.m_height;
					Color white = Color.White;
					((Color)(ref white)).A = ((Color)(ref m_color)).A;
					m_move_forward_animation.Draw(SB, zero2, white);
				}
				break;
			}
			if (m_game.m_tutorial_state == Game.TUTORIAL_STATE.USE)
			{
				string text4 = "Move the cursor and use";
				string text5 = "to interact with an object.";
				Vector2 zero4 = Vector2.Zero;
				Vector2 val4 = m_font2.MeasureString(text4);
				Vector2 val5 = m_font2.MeasureString(text5);
				float num2 = val4.X + 10f + (float)m_a_button.Width + 10f + val5.X;
				zero4.X = ((float)Game.VIEW_RECT.Width - num2) / 2f;
				zero4.Y = ((Rectangle)(ref Game.TS_AREA)).Top;
				SB.Begin((SpriteBlendMode)1);
				SB.DrawString(m_font2, text4, new Vector2(zero4.X + 1f, zero4.Y + 2f), Color.Black);
				SB.DrawString(m_font2, text4, zero4, Color.White);
				zero4.X += val4.X + 10f;
				zero4.Y += 5f;
				SB.Draw(m_a_button, zero4, Color.White);
				zero4.X += (float)(m_a_button.Width + 10);
				zero4.Y -= 5f;
				SB.DrawString(m_font2, text5, new Vector2(zero4.X + 1f, zero4.Y + 2f), Color.Black);
				SB.DrawString(m_font2, text5, zero4, Color.White);
				SB.End();
			}
			if (m_game.m_tutorial_state == Game.TUTORIAL_STATE.INVENTORY)
			{
				string text6 = "Press";
				string text7 = "to open the inventory.";
				Vector2 zero5 = Vector2.Zero;
				Vector2 val6 = m_font2.MeasureString(text6);
				Vector2 val7 = m_font2.MeasureString(text7);
				float num3 = val6.X + 10f + (float)m_y_button.Width + 10f + val7.X;
				zero5.X = ((float)Game.VIEW_RECT.Width - num3) / 2f;
				zero5.Y = ((Rectangle)(ref Game.TS_AREA)).Top;
				SB.Begin((SpriteBlendMode)1);
				SB.DrawString(m_font2, text6, new Vector2(zero5.X + 1f, zero5.Y + 2f), Color.Black);
				SB.DrawString(m_font2, text6, zero5, Color.White);
				zero5.X += (float)((int)val6.X + 10);
				zero5.Y += 5f;
				SB.Draw(m_y_button, zero5, Color.White);
				zero5.X += (float)(m_y_button.Width + 10);
				zero5.Y -= 5f;
				SB.DrawString(m_font2, text7, new Vector2(zero5.X + 1f, zero5.Y + 2f), Color.Black);
				SB.DrawString(m_font2, text7, zero5, Color.White);
				SB.End();
			}
		}
	}

	public virtual void DrawText(SpriteBatch SB, string text, Vector2 pos)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		DrawText(SB, text, pos, Color.White);
	}

	public virtual void DrawText(SpriteBatch SB, string text, Vector2 pos, Color c)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (SB != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.DrawString(m_font, text, new Vector2(pos.X + 1f, pos.Y + 2f), new Color((byte)0, (byte)0, (byte)0, ((Color)(ref c)).A));
			SB.DrawString(m_font, text, pos, c);
			SB.End();
		}
	}

	public virtual void DrawText2(SpriteBatch SB, string text, Vector2 pos)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		DrawText2(SB, text, pos, Color.White);
	}

	public virtual void DrawText2(SpriteBatch SB, string text, Vector2 pos, Color c)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (SB != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.DrawString(m_font2, text, new Vector2(pos.X + 1f, pos.Y + 2f), new Color((byte)0, (byte)0, (byte)0, ((Color)(ref c)).A));
			SB.DrawString(m_font2, text, pos, c);
			SB.End();
		}
	}

	public virtual void DrawText3(SpriteBatch SB, string text, Vector2 pos, Color c)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		if (SB != null)
		{
			SB.Begin((SpriteBlendMode)1);
			Vector2 val = m_font2.MeasureString(text);
			Rectangle val2 = default(Rectangle);
			((Rectangle)(ref val2))._002Ector((int)Math.Round(pos.X) - 5, (int)Math.Round(pos.Y - 2.5f), (int)Math.Round(val.X) + 10, (int)Math.Round(val.Y) + 5);
			float num = (int)((Color)(ref c)).A;
			if (num > 128f)
			{
				num = 128f;
			}
			SB.Draw(m_game.m_fade_texture, val2, new Color((byte)0, (byte)0, (byte)0, (byte)num));
			SB.DrawString(m_font2, text, new Vector2(pos.X + 1f, pos.Y + 2f), new Color((byte)0, (byte)0, (byte)0, ((Color)(ref c)).A));
			SB.DrawString(m_font2, text, pos, c);
			SB.End();
		}
	}
}
