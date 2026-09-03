using System;
using Core.Inventory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Core;

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

	public HUD_STATE m_state = HUD_STATE.NAVIGATOR;

	public HUD_FADE_STATE m_fade_state;

	public Navigator m_navigator;

	public Texture2D m_a_button;

	public Texture2D m_b_button;

	public Texture2D m_y_button;

	private Texture2D m_dpad;

	private Animation2D m_LS_animation;

	private Animation2D m_LT_animation;

	private Animation2D m_RT_animation;

	private TextureAnimation m_move_forward_animation;

	private Game m_game;

	public SpriteFont m_font;

	public SpriteFont m_font2;

	public Texture2D m_fade;

	public Color m_color = Color.White;

	public float m_alpha;

	private string m_text = "";

	private string m_ask_title = "";

	private string m_ask_text1 = "";

	private string m_ask_text2 = "";

	private HUD_TEXT_STATE m_text_state = HUD_TEXT_STATE.INVISIBLE;

	private float m_text_alpha;

	private bool m_text_fade;

	private string m_text_event = "";

	private string m_show_ask_a_event = "";

	private string m_show_ask_b_event = "";

	public HUD(Game game)
	{
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
		m_dpad = null;
		if (m_a_button != null)
		{
			m_a_button.Dispose();
			m_a_button = null;
		}
		if (m_b_button != null)
		{
			m_b_button.Dispose();
			m_b_button = null;
		}
		if (m_y_button != null)
		{
			m_y_button.Dispose();
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
		case HUD_STATE.NONE:
		case HUD_STATE.BACK:
		case HUD_STATE.CANCEL:
		case HUD_STATE.PROCEED:
		case HUD_STATE.MOVE_FORWARD:
			m_alpha = 1f;
			m_fade_state = HUD_FADE_STATE.FADE_OUT;
			break;
		case HUD_STATE.NAVIGATOR:
			m_navigator.FadeOut();
			break;
		}
	}

	public virtual void FadeIn()
	{
		switch (m_state)
		{
		case HUD_STATE.PROCEED:
			m_alpha = 0f;
			m_fade_state = HUD_FADE_STATE.FADE_IN_PROCEED;
			break;
		case HUD_STATE.MOVE_FORWARD:
			m_alpha = 0f;
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
			m_fade_state = HUD_FADE_STATE.FADE_IN_BACK;
			break;
		case HUD_STATE.CANCEL:
			m_alpha = 0f;
			m_fade_state = HUD_FADE_STATE.FADE_IN_CANCEL;
			break;
		case HUD_STATE.NAVIGATOR:
			m_navigator.FadeIn();
			break;
		}
	}

	public virtual void ShowText(string text, string text_event, bool fade, bool no_fade)
	{
		fade = true;
		m_text_fade = fade;
		if (no_fade)
		{
			m_text_fade = false;
		}
		m_text_event = text_event;
		ShowText(text);
	}

	public virtual void ShowText(string text, bool fade, bool no_fade)
	{
		fade = true;
		m_text_fade = fade;
		if (no_fade)
		{
			m_text_fade = false;
		}
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

	public virtual void ShowAsk(string title, string a_text, string b_text, bool fade, bool no_fade)
	{
		fade = true;
		ShowAsk(title, a_text, b_text, "", "", fade, no_fade);
	}

	public virtual void ShowAsk(string title, string a_text, string b_text, string a_event, string b_event, bool fade, bool no_fade)
	{
		fade = true;
		m_text_fade = fade;
		if (no_fade)
		{
			m_text_fade = false;
		}
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
			m_game.m_inventory.m_state = Core.Inventory.Inventory.INVENTORY_STATE.DISABLED;
		}
		m_show_ask_a_event = a_event;
		m_show_ask_b_event = b_event;
	}

	public virtual void HideText()
	{
		m_text_alpha = 255f;
		m_text_state = HUD_TEXT_STATE.INVISIBLE;
		m_game.m_state = Game.GAME_STATE.SCENE;
		m_text_fade = false;
		if (!m_game.m_input_blocked)
		{
			m_game.m_show_cursor = true;
			FadeIn();
		}
		if (m_text_event != "")
		{
			string text_event = m_text_event;
			m_text_event = "";
			m_game.HandleEvent(text_event);
		}
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
		if (m_move_forward_animation != null)
		{
			m_move_forward_animation.Update(elapsed);
		}
		if (m_game.m_state == Game.GAME_STATE.SHOW_ASK)
		{
			KeyboardState state = Keyboard.GetState();
			if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.A))
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
			if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.B == ButtonState.Pressed || state.IsKeyDown(Keys.B))
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
				m_alpha -= (float)elapsed.TotalSeconds * 2f;
				if (m_alpha <= 0f)
				{
					m_alpha = 0f;
					m_fade_state = HUD_FADE_STATE.NONE;
				}
				break;
			case HUD_FADE_STATE.FADE_IN_BACK:
			case HUD_FADE_STATE.FADE_IN_CANCEL:
			case HUD_FADE_STATE.FADE_IN_PROCEED:
			case HUD_FADE_STATE.FADE_IN_MOVE_FORWARD:
				m_alpha += (float)elapsed.TotalSeconds * 2f;
				if (m_alpha >= 1f)
				{
					m_alpha = 1f;
					m_fade_state = HUD_FADE_STATE.NONE;
				}
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
			case Tutorial.STATE.MOVE_CURSOR:
				m_LS_animation.Update(elapsed);
				break;
			case Tutorial.STATE.DECREASE_SPEED:
				m_LT_animation.Update(elapsed);
				m_LS_animation.Update(elapsed);
				break;
			case Tutorial.STATE.INCREASE_SPEED:
				m_RT_animation.Update(elapsed);
				m_LS_animation.Update(elapsed);
				break;
			case Tutorial.STATE.CHANGE_VIEW:
				m_navigator.Update(elapsed);
				break;
			}
		}
	}

	public virtual void DrawTutorial(SpriteBatch SB)
	{
		switch (m_game.m_tutorial_state)
		{
		case Tutorial.STATE.MOVE_CURSOR:
		{
			string text7 = "Use the Left Stick to move the cursor.";
			Vector2 zero4 = Vector2.Zero;
			Vector2 vector7 = m_font2.MeasureString(text7);
			float num3 = (float)(m_LS_animation.m_width + 10) + vector7.X;
			zero4.X = ((float)Game.VIEW_RECT.Width - num3) / 2f;
			zero4.Y = Game.TS_AREA.Top;
			m_LS_animation.Draw(SB, zero4, m_color * m_alpha);
			zero4.X += m_LS_animation.m_width + 10;
			zero4.Y += 25f;
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			Color black = Color.Black;
			black.A = (byte)Math.Round(m_alpha);
			SB.DrawString(m_font2, text7, new Vector2(zero4.X + 1f, zero4.Y + 2f), black);
			SB.DrawString(m_font2, text7, zero4, m_color * m_alpha);
			SB.End();
			break;
		}
		case Tutorial.STATE.DECREASE_SPEED:
		{
			string text5 = "Use the Left Trigger to slow down the cursor.";
			string text6 = "+";
			Vector2 zero3 = Vector2.Zero;
			Vector2 vector5 = m_font2.MeasureString(text5);
			Vector2 vector6 = m_font2.MeasureString(text6);
			float num2 = (float)m_LT_animation.m_width + vector6.X + (float)m_LS_animation.m_width + 10f + vector5.X;
			zero3.X = ((float)Game.VIEW_RECT.Width - num2) / 2f;
			zero3.Y = Game.TS_AREA.Top;
			m_LT_animation.Draw(SB, zero3, m_color * m_alpha);
			zero3.X += m_LT_animation.m_width + 10;
			zero3.Y += 25f;
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.DrawString(m_font2, text6, new Vector2(zero3.X + 1f, zero3.Y + 2f), Color.Black * m_alpha);
			SB.DrawString(m_font2, text6, zero3, m_color * m_alpha);
			SB.End();
			zero3.X += vector6.X;
			zero3.Y -= 25f;
			m_LS_animation.Draw(SB, zero3, m_color * m_alpha);
			zero3.X += m_LS_animation.m_width + 10;
			zero3.Y += 25f;
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.DrawString(m_font2, text5, new Vector2(zero3.X + 1f, zero3.Y + 2f), Color.Black * m_alpha);
			SB.DrawString(m_font2, text5, zero3, m_color * m_alpha);
			SB.End();
			break;
		}
		case Tutorial.STATE.INCREASE_SPEED:
		{
			string text3 = "Use the Right Trigger to speed up the cursor.";
			string text4 = "+";
			Vector2 zero2 = Vector2.Zero;
			Vector2 vector3 = m_font2.MeasureString(text3);
			Vector2 vector4 = m_font2.MeasureString(text4);
			float num = (float)m_RT_animation.m_width + vector4.X + (float)m_LS_animation.m_width + 10f + vector3.X;
			zero2.X = ((float)Game.VIEW_RECT.Width - num) / 2f;
			zero2.Y = Game.TS_AREA.Top;
			m_RT_animation.Draw(SB, zero2, m_color * m_alpha);
			zero2.X += m_RT_animation.m_width + 10;
			zero2.Y += 25f;
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.DrawString(m_font2, text4, new Vector2(zero2.X + 1f, zero2.Y + 2f), Color.Black * m_alpha);
			SB.DrawString(m_font2, text4, zero2, m_color * m_alpha);
			SB.End();
			zero2.X += vector4.X;
			zero2.Y -= 25f;
			m_LS_animation.Draw(SB, zero2, m_color * m_alpha);
			zero2.X += m_LS_animation.m_width + 10;
			zero2.Y += 25f;
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.DrawString(m_font2, text3, new Vector2(zero2.X + 1f, zero2.Y + 2f), Color.Black * m_alpha);
			SB.DrawString(m_font2, text3, zero2, m_color * m_alpha);
			SB.End();
			break;
		}
		case Tutorial.STATE.CHANGE_VIEW:
		{
			string text = "Down to the right you can see the navigation icon.";
			string text2 = "Use the Directional Pad to turn around.";
			Vector2 zero = Vector2.Zero;
			Vector2 vector = m_font2.MeasureString(text);
			Vector2 vector2 = m_font2.MeasureString(text2);
			float x = vector.X;
			zero.X = ((float)Game.VIEW_RECT.Width - x) / 2f;
			zero.Y = Game.TS_AREA.Top;
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.DrawString(m_font2, text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black * m_alpha);
			SB.DrawString(m_font2, text, zero, m_color * m_alpha);
			x = (float)(m_dpad.Width + 10) + vector2.X;
			zero.X = ((float)Game.VIEW_RECT.Width - x) / 2f - 5f;
			zero.Y += 25f;
			SB.Draw(m_dpad, zero, m_color * m_alpha);
			zero.X += m_dpad.Width + 10;
			zero.Y += 20f;
			SB.DrawString(m_font2, text2, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black * m_alpha);
			SB.DrawString(m_font2, text2, zero, m_color * m_alpha);
			SB.End();
			m_navigator.Draw(SB);
			break;
		}
		}
	}

	protected virtual void DrawText(SpriteBatch SB)
	{
		SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		if (m_text_fade)
		{
			SB.Draw(m_fade, Game.VIEW_RECT, Color.White * 0.5f);
		}
		Vector2 vector = m_font2.MeasureString(m_text);
		Vector2 zero = Vector2.Zero;
		zero.X = ((float)Game.VIEW_RECT.Width - vector.X) / 2f;
		zero.Y = (float)(Game.TS_AREA.Bottom - m_a_button.Height - 20) - vector.Y;
		SB.DrawString(m_font2, m_text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black * m_text_alpha);
		SB.DrawString(m_font2, m_text, zero, Color.White * m_text_alpha);
		if (m_text_alpha >= 1f)
		{
			float num = m_a_button.Width;
			zero.X = ((float)Game.VIEW_RECT.Width - num) / 2f;
			zero.Y = Game.TS_AREA.Bottom - m_a_button.Height;
			SB.Draw(m_a_button, zero, Color.White);
		}
		SB.End();
	}

	protected virtual void DrawAsk(SpriteBatch SB)
	{
		if (m_text_fade)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_fade, Game.VIEW_RECT, Color.White * 0.5f);
			SB.End();
		}
		Vector2 vector = m_font2.MeasureString(m_ask_title);
		Vector2 zero = Vector2.Zero;
		zero.X = ((float)Game.VIEW_RECT.Width - vector.X) / 2f;
		zero.Y = (float)(Game.TS_AREA.Bottom - m_a_button.Height - 20) - vector.Y;
		SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		SB.DrawString(m_font2, m_ask_title, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
		SB.DrawString(m_font2, m_ask_title, zero, Color.White);
		SB.End();
		Vector2 vector2 = m_font.MeasureString(m_ask_text1);
		Vector2 vector3 = m_font.MeasureString(m_ask_text2);
		float num = 10f;
		float num2 = 40f;
		float num3 = (float)m_a_button.Width + num + (float)(int)vector2.X + num2 + (float)m_b_button.Width + num + (float)(int)vector3.X;
		zero.X = (int)((float)Game.VIEW_RECT.Width - num3) / 2;
		zero.Y = Game.TS_AREA.Bottom - m_a_button.Height;
		SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		SB.Draw(m_a_button, zero, Color.White);
		zero.X += (float)m_a_button.Width + num;
		SB.DrawString(m_font, m_ask_text1, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
		SB.DrawString(m_font, m_ask_text1, zero, Color.White);
		zero.X += (float)(int)vector2.X + num2;
		SB.Draw(m_b_button, zero, Color.White);
		zero.X += (float)m_b_button.Width + num;
		SB.DrawString(m_font, m_ask_text2, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
		SB.DrawString(m_font, m_ask_text2, zero, Color.White);
		SB.End();
	}

	public virtual void Draw(SpriteBatch SB)
	{
		if (m_game.m_state == Game.GAME_STATE.TUTORIAL)
		{
			DrawTutorial(SB);
		}
		else if (m_game.m_state == Game.GAME_STATE.ASK_TUTORIAL)
		{
			string text = "PLAY TUTORIAL";
			string text2 = "SKIP TUTORIAL";
			Vector2 zero = Vector2.Zero;
			Vector2 vector = m_font.MeasureString(text);
			Vector2 vector2 = m_font.MeasureString(text2);
			float num = (float)(m_a_button.Width + 10) + vector.X + 40f + (float)m_b_button.Width + 10f + vector2.X;
			zero.X = ((float)Game.VIEW_RECT.Width - num) / 2f;
			zero.Y = Game.TS_AREA.Top;
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_a_button, zero, m_color * m_alpha);
			zero.X += m_a_button.Width + 10;
			SB.DrawString(m_font, text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black * m_alpha);
			SB.DrawString(m_font, text, zero, m_color * m_alpha);
			zero.X += vector.X + 40f;
			SB.Draw(m_b_button, zero, m_color * m_alpha);
			zero.X += m_b_button.Width + 10;
			SB.DrawString(m_font, text2, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black * m_alpha);
			SB.DrawString(m_font, text2, zero, m_color * m_alpha);
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
					SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
					string text3 = "";
					if (m_state == HUD_STATE.BACK)
					{
						text3 = "BACK";
					}
					if (m_state == HUD_STATE.CANCEL)
					{
						text3 = "CANCEL";
					}
					Vector2 vector3 = m_font.MeasureString(text3);
					Vector2 zero3 = Vector2.Zero;
					zero3.X = (float)Game.TS_AREA.Right - vector3.X;
					zero3.Y = (float)Game.TS_AREA.Bottom - vector3.Y;
					SB.DrawString(m_font, text3, new Vector2(zero3.X + 1f, zero3.Y + 2f), Color.Black * m_alpha);
					SB.DrawString(m_font, text3, zero3, m_color * m_alpha);
					if (m_state == HUD_STATE.PROCEED)
					{
						zero3.X = Game.TS_AREA.Right - m_a_button.Width;
						zero3.Y = Game.TS_AREA.Bottom - m_a_button.Height;
						SB.Draw(m_a_button, zero3, m_color * m_alpha);
					}
					else
					{
						zero3.X -= m_b_button.Width + 10;
						SB.Draw(m_b_button, zero3, m_color * m_alpha);
					}
					SB.End();
				}
				break;
			case HUD_STATE.MOVE_FORWARD:
				if (m_move_forward_animation != null)
				{
					Vector2 zero2 = Vector2.Zero;
					zero2.X = Game.TS_AREA.Right - m_move_forward_animation.m_width;
					zero2.Y = Game.TS_AREA.Bottom - m_move_forward_animation.m_height;
					m_move_forward_animation.Draw(SB, zero2, Color.White * m_alpha);
				}
				break;
			}
			if (m_game.m_tutorial_state == Tutorial.STATE.USE)
			{
				string text4 = "Move the cursor and use";
				string text5 = "to interact with an object.";
				Vector2 zero4 = Vector2.Zero;
				Vector2 vector4 = m_font2.MeasureString(text4);
				Vector2 vector5 = m_font2.MeasureString(text5);
				float num2 = vector4.X + 10f + (float)m_a_button.Width + 10f + vector5.X;
				zero4.X = ((float)Game.VIEW_RECT.Width - num2) / 2f;
				zero4.Y = Game.TS_AREA.Top;
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.DrawString(m_font2, text4, new Vector2(zero4.X + 1f, zero4.Y + 2f), Color.Black);
				SB.DrawString(m_font2, text4, zero4, Color.White);
				zero4.X += vector4.X + 10f;
				zero4.Y += 5f;
				SB.Draw(m_a_button, zero4, Color.White);
				zero4.X += m_a_button.Width + 10;
				zero4.Y -= 5f;
				SB.DrawString(m_font2, text5, new Vector2(zero4.X + 1f, zero4.Y + 2f), Color.Black);
				SB.DrawString(m_font2, text5, zero4, Color.White);
				SB.End();
			}
			if (m_game.m_tutorial_state == Tutorial.STATE.INVENTORY)
			{
				string text6 = "Press";
				string text7 = "to open the inventory.";
				Vector2 zero5 = Vector2.Zero;
				Vector2 vector6 = m_font2.MeasureString(text6);
				Vector2 vector7 = m_font2.MeasureString(text7);
				float num3 = vector6.X + 10f + (float)m_y_button.Width + 10f + vector7.X;
				zero5.X = ((float)Game.VIEW_RECT.Width - num3) / 2f;
				zero5.Y = Game.TS_AREA.Top;
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.DrawString(m_font2, text6, new Vector2(zero5.X + 1f, zero5.Y + 2f), Color.Black);
				SB.DrawString(m_font2, text6, zero5, Color.White);
				zero5.X += (int)vector6.X + 10;
				zero5.Y += 5f;
				SB.Draw(m_y_button, zero5, Color.White);
				zero5.X += m_y_button.Width + 10;
				zero5.Y -= 5f;
				SB.DrawString(m_font2, text7, new Vector2(zero5.X + 1f, zero5.Y + 2f), Color.Black);
				SB.DrawString(m_font2, text7, zero5, Color.White);
				SB.End();
			}
		}
	}

	public virtual void DrawText(SpriteBatch SB, string text, Vector2 pos)
	{
		DrawText(SB, text, pos, Color.White);
	}

	public virtual void DrawText(SpriteBatch SB, string text, Vector2 pos, Color c)
	{
		if (SB != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.DrawString(m_font, text, new Vector2(pos.X + 1f, pos.Y + 2f), Color.Black * ((float)(int)c.A / 255f));
			SB.DrawString(m_font, text, pos, c);
			SB.End();
		}
	}

	public virtual void DrawText2(SpriteBatch SB, string text, Vector2 pos)
	{
		DrawText2(SB, text, pos, Color.White);
	}

	public virtual void DrawText2(SpriteBatch SB, string text, Vector2 pos, Color c)
	{
		if (SB != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.DrawString(m_font2, text, new Vector2(pos.X + 1f, pos.Y + 2f), Color.Black * ((float)(int)c.A / 255f));
			SB.DrawString(m_font2, text, pos, c);
			SB.End();
		}
	}

	public virtual void DrawText3(SpriteBatch SB, string text, Vector2 pos, Color c)
	{
		if (SB != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			Vector2 vector = m_font2.MeasureString(text);
			Rectangle destinationRectangle = new Rectangle((int)Math.Round(pos.X) - 5, (int)Math.Round(pos.Y - 2.5f), (int)Math.Round(vector.X) + 10, (int)Math.Round(vector.Y) + 5);
			float num = (float)(int)c.A / 255f;
			if (num > 0.5f)
			{
				num = 0.5f;
			}
			SB.Draw(m_game.m_fade_texture, destinationRectangle, Color.Black * num);
			SB.DrawString(m_font2, text, new Vector2(pos.X + 1f, pos.Y + 2f), Color.Black * num);
			SB.DrawString(m_font2, text, pos, c);
			SB.End();
		}
	}
}
