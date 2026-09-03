using System;
using System.Collections.Generic;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SGSCore;

namespace TheMare1;

public class Extras : Core.Extras
{
	private int m_music = 2;

	private List<Texture2D> m_images = new List<Texture2D>();

	private int m_current_image;

	private Texture2D m_X;

	private Texture2D m_LS;

	private Texture2D m_B;

	public Extras(Core.Game game, SGSContentLoader CL)
		: base(game, CL)
	{
		m_images.Add(m_game.m_CL.LoadTexture("Extras/level1"));
		m_images.Add(m_game.m_CL.LoadTexture("Extras/level2"));
		m_images.Add(m_game.m_CL.LoadTexture("Extras/level3"));
		m_images.Add(m_game.m_CL.LoadTexture("Extras/cat"));
		m_images.Add(m_game.m_CL.LoadTexture("Extras/dog"));
		m_images.Add(m_game.m_CL.LoadTexture("Extras/worms"));
		m_images.Add(m_game.m_CL.LoadTexture("Extras/fan_puzzle"));
		m_images.Add(m_game.m_CL.LoadTexture("Extras/room_puzzle"));
		m_images.Add(m_game.m_CL.LoadTexture("Extras/sam_neutral"));
		m_LS = m_game.m_CL.LoadTexture("HUD/LS");
		m_X = m_game.m_CL.LoadTexture("HUD/x_button");
		m_B = m_game.m_CL.LoadTexture("HUD/b_button");
	}

	public override void Clear()
	{
		if (m_images != null)
		{
			m_images.Clear();
			m_images = null;
		}
		m_LS = null;
		m_X = null;
		m_B = null;
		base.Clear();
	}

	public override void Reset()
	{
		m_current_image = 0;
		m_music = 2;
		base.Reset();
	}

	public override void Update(TimeSpan elapsed)
	{
		KeyboardState state = Keyboard.GetState();
		if (GamePad.GetState(Core.Game.PLAYER_INDEX).Buttons.B == ButtonState.Pressed || state.IsKeyDown(Keys.B))
		{
			if (!m_game.m_b_pressed)
			{
				m_game.m_b_pressed = true;
				m_music = 2;
				if (m_game.m_game_data != null)
				{
					m_game.m_game_data.SetState("Music", m_music.ToString());
				}
				m_game.PlayMusic(m_music);
				m_game.onExtrasClosed();
			}
		}
		else
		{
			m_game.m_b_pressed = false;
		}
		if (!m_game.m_game_settings.m_extras_unlocked)
		{
			return;
		}
		if (GamePad.GetState(Core.Game.PLAYER_INDEX).Buttons.X == ButtonState.Pressed || state.IsKeyDown(Keys.X))
		{
			if (!m_game.m_x_pressed)
			{
				m_game.m_x_pressed = true;
				switch (m_music)
				{
				case 1:
					m_music = 2;
					break;
				case 2:
					m_music = 3;
					break;
				case 3:
					m_music = 1;
					break;
				}
				if (m_game.m_game_data != null)
				{
					m_game.m_game_data.SetState("Music", m_music.ToString());
				}
				m_game.PlayMusic(m_music);
			}
		}
		else
		{
			m_game.m_x_pressed = false;
		}
		if (state.IsKeyDown(Keys.Left) || GamePad.GetState(Core.Game.PLAYER_INDEX).ThumbSticks.Left.X < -0.2f || GamePad.GetState(Core.Game.PLAYER_INDEX).DPad.Left == ButtonState.Pressed)
		{
			if (!m_game.m_left_pressed)
			{
				m_game.m_left_pressed = true;
				m_current_image--;
				if (m_current_image < 0)
				{
					m_current_image = m_images.Count - 1;
				}
			}
			return;
		}
		m_game.m_left_pressed = false;
		if (state.IsKeyDown(Keys.Right) || GamePad.GetState(Core.Game.PLAYER_INDEX).ThumbSticks.Left.X > 0.2f || GamePad.GetState(Core.Game.PLAYER_INDEX).DPad.Right == ButtonState.Pressed)
		{
			if (!m_game.m_right_pressed)
			{
				m_game.m_right_pressed = true;
				m_current_image++;
				if (m_current_image >= m_images.Count)
				{
					m_current_image = 0;
				}
			}
		}
		else
		{
			m_game.m_right_pressed = false;
		}
	}

	public override void Draw(SpriteBatch SB)
	{
		try
		{
			if (SB == null)
			{
				return;
			}
			SB.Begin();
			string text = "";
			Vector2 zero = Vector2.Zero;
			Vector2 zero2 = Vector2.Zero;
			text = m_game.m_language.GetString("BACK");
			zero = m_font.MeasureString(text);
			zero2 = Vector2.Zero;
			zero2.X = (float)Core.Game.TS_AREA.Right - zero.X;
			zero2.Y = (float)Core.Game.TS_AREA.Bottom - zero.Y;
			SB.DrawString(m_font2, text, new Vector2(zero2.X + 1f, zero2.Y + 2f), Color.Black);
			SB.DrawString(m_font2, text, zero2, Color.White);
			zero2.X -= m_B.Width + 10;
			SB.Draw(m_B, zero2, Color.White);
			if (!m_game.m_game_settings.m_extras_unlocked)
			{
				text = m_game.m_language.GetString("Collect all coins and complete the game");
				zero = m_font.MeasureString(text);
				zero2 = new Vector2(((float)Core.Game.VIEW_RECT.Width - zero.X) / 2f + 1f, ((float)Core.Game.VIEW_RECT.Height - zero.Y) / 2f + 2f - zero.Y / 2f);
				SB.DrawString(m_font, text, zero2, Color.Black);
				zero2.X--;
				zero2.Y -= 2f;
				SB.DrawString(m_font, text, zero2, Color.White);
				text = m_game.m_language.GetString("to unlock this feature.");
				zero = m_font.MeasureString(text);
				zero2 = new Vector2(((float)Core.Game.VIEW_RECT.Width - zero.X) / 2f + 1f, ((float)Core.Game.VIEW_RECT.Height - zero.Y) / 2f + 2f + zero.Y / 2f);
				SB.DrawString(m_font, text, zero2, Color.Black);
				zero2.X--;
				zero2.Y -= 2f;
				SB.DrawString(m_font, text, zero2, Color.White);
				SB.End();
				return;
			}
			if (m_images != null)
			{
				SB.Draw(m_images[m_current_image], new Vector2((Core.Game.VIEW_RECT.Width - m_images[m_current_image].Width) / 2, (Core.Game.VIEW_RECT.Height - m_images[m_current_image].Height) / 2), Color.White);
			}
			text = m_game.m_language.GetString("CHANGE MUSIC");
			zero = m_font.MeasureString(text);
			zero2 = Vector2.Zero;
			zero2.X = Core.Game.TS_AREA.Left + m_LS.Width + 10;
			zero2.Y = (float)Core.Game.TS_AREA.Bottom - zero.Y;
			SB.DrawString(m_font2, text, new Vector2(zero2.X + 1f, zero2.Y + 2f), Color.Black);
			SB.DrawString(m_font2, text, zero2, Color.White);
			zero2.X -= m_X.Width + 10 + 9;
			SB.Draw(m_X, zero2, Color.White);
			text = m_game.m_language.GetString("CHANGE IMAGE");
			zero = m_font.MeasureString(text);
			zero2 = Vector2.Zero;
			zero2.X = Core.Game.TS_AREA.Left + m_LS.Width + 10;
			zero2.Y = (float)Core.Game.TS_AREA.Bottom - zero.Y * 2f - 10f;
			SB.DrawString(m_font2, text, new Vector2(zero2.X + 1f, zero2.Y + 2f), Color.Black);
			SB.DrawString(m_font2, text, zero2, Color.White);
			zero2.X -= m_LS.Width + 10;
			zero2.Y -= 8f;
			SB.Draw(m_LS, zero2, Color.White);
			zero2.X = Core.Game.TS_AREA.Left;
			switch (m_current_image)
			{
			case 0:
				zero2.X += 60f;
				text = m_game.m_language.GetString("Concept art for\nthe first floor.");
				break;
			case 1:
				zero2.X += 40f;
				text = m_game.m_language.GetString("Concept art for\nthe second floor.");
				break;
			case 2:
				zero2.X += 60f;
				text = m_game.m_language.GetString("Concept art for\nthe third floor.");
				break;
			case 3:
				zero2.X += 40f;
				text = m_game.m_language.GetString("Concept art for one\nof the paintings\nin the game.");
				break;
			case 4:
				zero2.X += 0f;
				text = m_game.m_language.GetString("Concept art for one\nof the paintings\nin the game.");
				break;
			case 5:
				zero2.X += 60f;
				text = m_game.m_language.GetString("Concept art for one\nof the paintings\nin the game.");
				break;
			case 6:
				zero2.X += 60f;
				text = m_game.m_language.GetString("Concept art for\nthe 'fan puzzle'.");
				break;
			case 7:
				zero2.X += 60f;
				text = m_game.m_language.GetString("Concept art for\nthe 'maze puzzle'.");
				break;
			case 8:
				zero2.X += 60f;
				text = m_game.m_language.GetString("Concept art for\n'Sam'.");
				break;
			}
			zero = m_font2.MeasureString(text);
			zero2.Y = Core.Game.TS_AREA.Top + 60;
			SB.DrawString(m_font2, text, new Vector2(zero2.X + 1f, zero2.Y + 2f), Color.Black);
			SB.DrawString(m_font2, text, zero2, Color.White);
			SB.End();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
