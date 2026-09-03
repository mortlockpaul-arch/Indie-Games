using System;
using Core;
using Core.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheMare1.World.Views.DreamRoom11;

internal class Left_Door_Zoom : View
{
	private enum STATE
	{
		IDLE,
		ASK1,
		ASK2
	}

	private STATE m_state;

	public Left_Door_Zoom(Core.Game game, Area room, string xml_path)
		: base(game, room, xml_path)
	{
		m_images.Add("A", new Image(getContentLoader().LoadTexture("HUD/a_button"), new Rectangle(Core.Game.TS_AREA.X, Core.Game.TS_AREA.Bottom - 90 - 20, 27, 30)));
		m_images.Add("X", new Image(getContentLoader().LoadTexture("HUD/x_button"), new Rectangle(Core.Game.TS_AREA.X, Core.Game.TS_AREA.Bottom - 60 - 10, 27, 30)));
		m_images.Add("Y", new Image(getContentLoader().LoadTexture("HUD/y_button"), new Rectangle(Core.Game.TS_AREA.X, Core.Game.TS_AREA.Bottom - 30, 27, 30)));
	}

	public override void Clear()
	{
		base.Clear();
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "Left_Door_Zoom.onAsk1":
			m_state = STATE.ASK1;
			m_game.m_a_pressed = true;
			m_game.m_x_pressed = true;
			m_game.m_y_pressed = true;
			break;
		case "Left_Door_Zoom.onAsk2":
			m_state = STATE.ASK2;
			m_game.m_a_pressed = true;
			m_game.m_x_pressed = true;
			m_game.m_y_pressed = true;
			break;
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		try
		{
			base.Update(elapsed);
			if (m_state == STATE.IDLE)
			{
				return;
			}
			KeyboardState state = Keyboard.GetState();
			if (state.IsKeyDown(Keys.A) || GamePad.GetState(Core.Game.PLAYER_INDEX).IsButtonDown(Buttons.A))
			{
				if (!m_game.m_a_pressed)
				{
					m_game.m_a_pressed = true;
					switch (m_state)
					{
					case STATE.ASK1:
						m_game.HandleEvent(m_name + ".onAsk1Answer1");
						break;
					case STATE.ASK2:
						m_game.HandleEvent(m_name + ".onAsk2Answer1");
						break;
					}
					m_state = STATE.IDLE;
				}
			}
			else
			{
				m_game.m_a_pressed = false;
			}
			if (state.IsKeyDown(Keys.X) || GamePad.GetState(Core.Game.PLAYER_INDEX).IsButtonDown(Buttons.X))
			{
				if (!m_game.m_x_pressed)
				{
					switch (m_state)
					{
					case STATE.ASK1:
						m_game.HandleEvent(m_name + ".onAsk1Answer2");
						break;
					case STATE.ASK2:
						m_game.HandleEvent(m_name + ".onAsk2Answer2");
						break;
					}
					m_state = STATE.IDLE;
				}
			}
			else
			{
				m_game.m_x_pressed = false;
			}
			if (state.IsKeyDown(Keys.Y) || GamePad.GetState(Core.Game.PLAYER_INDEX).IsButtonDown(Buttons.Y))
			{
				if (!m_game.m_y_pressed)
				{
					switch (m_state)
					{
					case STATE.ASK1:
						m_game.HandleEvent(m_name + ".onAsk1Answer3");
						break;
					case STATE.ASK2:
						m_game.HandleEvent(m_name + ".onAsk2Answer3");
						break;
					}
					m_state = STATE.IDLE;
				}
			}
			else
			{
				m_game.m_y_pressed = false;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void Draw(SpriteBatch SB)
	{
		base.Draw(SB);
		if (m_state != STATE.IDLE)
		{
			string text = "";
			Vector2 vector = default(Vector2);
			vector.X = Core.Game.TS_AREA.X + 44 + 5;
			switch (m_state)
			{
			case STATE.ASK1:
				vector.Y = Core.Game.TS_AREA.Bottom - 90 - 20;
				text = m_game.m_language.GetString("No, I'm not your mother.");
				m_game.m_hud.m_font.MeasureString(text);
				m_images["A"].Draw(SB, m_game.m_hud.m_color);
				m_game.m_hud.DrawText(SB, text, new Vector2(vector.X, vector.Y), m_game.m_hud.m_color);
				vector.Y = Core.Game.TS_AREA.Bottom - 60 - 10;
				text = m_game.m_language.GetString("... yes.");
				m_game.m_hud.m_font.MeasureString(text);
				m_images["X"].Draw(SB, m_game.m_hud.m_color);
				m_game.m_hud.DrawText(SB, text, new Vector2(vector.X, vector.Y), m_game.m_hud.m_color);
				vector.Y = Core.Game.TS_AREA.Bottom - 30;
				text = m_game.m_language.GetString("What the hell are you?");
				m_game.m_hud.m_font.MeasureString(text);
				m_images["Y"].Draw(SB, m_game.m_hud.m_color);
				m_game.m_hud.DrawText(SB, text, new Vector2(vector.X, vector.Y), m_game.m_hud.m_color);
				break;
			case STATE.ASK2:
				vector.Y = Core.Game.TS_AREA.Bottom - 90 - 20;
				text = m_game.m_language.GetString("...");
				m_game.m_hud.m_font.MeasureString(text);
				m_images["A"].Draw(SB, m_game.m_hud.m_color);
				m_game.m_hud.DrawText(SB, text, new Vector2(vector.X, vector.Y), m_game.m_hud.m_color);
				vector.Y = Core.Game.TS_AREA.Bottom - 60 - 10;
				text = m_game.m_language.GetString("Who are you?");
				m_game.m_hud.m_font.MeasureString(text);
				m_images["X"].Draw(SB, m_game.m_hud.m_color);
				m_game.m_hud.DrawText(SB, text, new Vector2(vector.X, vector.Y), m_game.m_hud.m_color);
				vector.Y = Core.Game.TS_AREA.Bottom - 30;
				text = m_game.m_language.GetString("What are you?");
				m_game.m_hud.m_font.MeasureString(text);
				m_images["Y"].Draw(SB, m_game.m_hud.m_color);
				m_game.m_hud.DrawText(SB, text, new Vector2(vector.X, vector.Y), m_game.m_hud.m_color);
				break;
			}
		}
	}
}
