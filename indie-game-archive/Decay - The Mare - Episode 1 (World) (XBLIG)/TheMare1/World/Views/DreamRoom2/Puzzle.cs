using System;
using Core;
using Core.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheMare1.World.Views.DreamRoom2;

internal class Puzzle : View
{
	private bool m_draw_hud;

	private bool m_button_mode;

	public Puzzle(Core.Game game, Area room, string xml_path)
		: base(game, room, xml_path)
	{
		m_images.Add("LS", new Image(getContentLoader().LoadTexture("HUD/LS"), new Rectangle(Core.Game.TS_AREA.X, Core.Game.TS_AREA.Bottom - 44 + 7, 44, 44)));
		m_images.Add("X", new Image(getContentLoader().LoadTexture("HUD/x_button"), new Rectangle(Core.Game.TS_AREA.X, Core.Game.TS_AREA.Bottom - 30, 27, 30)));
	}

	public override void Clear()
	{
		base.Clear();
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "Puzzle.onReset":
			m_draw_hud = true;
			break;
		case "Puzzle.EnterButtonMode":
			m_game.m_left_pressed = true;
			m_game.m_right_pressed = true;
			m_game.m_a_pressed = true;
			m_game.m_b_pressed = true;
			m_game.m_hud.FadeIn();
			m_button_mode = true;
			break;
		case "Puzzle.onBack":
			if (!m_button_mode)
			{
				m_draw_hud = false;
			}
			break;
		}
		base.HandleEvent(s_event);
	}

	private void onLeaveButtonMode()
	{
		try
		{
			HandleEvent(m_name + ".LeaveButtonMode");
			m_game.m_x_pressed = true;
			m_button_mode = false;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void Update(TimeSpan elapsed)
	{
		try
		{
			base.Update(elapsed);
			if (m_game.m_state != Core.Game.GAME_STATE.SCENE)
			{
				return;
			}
			KeyboardState state = Keyboard.GetState();
			if (m_local_states["ButtonMode"] != "1")
			{
				if (m_game.m_input_blocked)
				{
					return;
				}
				if (state.IsKeyDown(Keys.X) || GamePad.GetState(Core.Game.PLAYER_INDEX).IsButtonDown(Buttons.X))
				{
					if (!m_game.m_x_pressed)
					{
						m_game.m_x_pressed = true;
						m_game.m_a_pressed = true;
						m_images["X"].FadeOut();
						HandleEvent(m_name + ".onUseHandle");
					}
				}
				else
				{
					m_game.m_x_pressed = false;
				}
				return;
			}
			if (state.IsKeyDown(Keys.A) || GamePad.GetState(Core.Game.PLAYER_INDEX).IsButtonDown(Buttons.A))
			{
				if (!m_game.m_a_pressed)
				{
					m_game.m_a_pressed = true;
					onLeaveButtonMode();
				}
			}
			else
			{
				m_game.m_a_pressed = false;
			}
			if (state.IsKeyDown(Keys.B) || GamePad.GetState(Core.Game.PLAYER_INDEX).IsButtonDown(Buttons.B))
			{
				if (!m_game.m_b_pressed)
				{
					m_game.m_b_pressed = true;
					onLeaveButtonMode();
				}
			}
			else
			{
				m_game.m_b_pressed = false;
			}
			if (state.IsKeyDown(Keys.Left) || GamePad.GetState(Core.Game.PLAYER_INDEX).ThumbSticks.Left.X < -0.2f || GamePad.GetState(Core.Game.PLAYER_INDEX).DPad.Left == ButtonState.Pressed)
			{
				if (!m_game.m_left_pressed)
				{
					m_game.m_left_pressed = true;
					HandleEvent(m_name + ".Button" + m_local_states["CurrentButton"] + ".onLeft");
				}
				return;
			}
			m_game.m_left_pressed = false;
			if (state.IsKeyDown(Keys.Right) || GamePad.GetState(Core.Game.PLAYER_INDEX).ThumbSticks.Left.X > 0.2f || GamePad.GetState(Core.Game.PLAYER_INDEX).DPad.Right == ButtonState.Pressed)
			{
				if (!m_game.m_right_pressed)
				{
					m_game.m_right_pressed = true;
					HandleEvent(m_name + ".Button" + m_local_states["CurrentButton"] + ".onRight");
				}
			}
			else
			{
				m_game.m_right_pressed = false;
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
		if (m_game.m_state == Core.Game.GAME_STATE.SCENE && m_draw_hud)
		{
			string text = "";
			Vector2 vector2 = default(Vector2);
			if (!m_button_mode)
			{
				text = m_game.m_language.GetString("USE HANDLE");
				Vector2 vector = m_game.m_hud.m_font.MeasureString(text);
				vector2.X = Core.Game.TS_AREA.X + 27 + 10;
				vector2.Y = (float)Core.Game.TS_AREA.Bottom - vector.Y;
				m_images["X"].Draw(SB, m_game.m_hud.m_color * m_game.m_hud.m_alpha);
			}
			else
			{
				text = m_game.m_language.GetString("ROTATE");
				Vector2 vector = m_game.m_hud.m_font.MeasureString(text);
				vector2.X = Core.Game.TS_AREA.X + 44 + 5;
				vector2.Y = (float)Core.Game.TS_AREA.Bottom - vector.Y;
				m_images["LS"].Draw(SB, m_game.m_hud.m_color * m_game.m_hud.m_alpha);
			}
			m_game.m_hud.DrawText(SB, text, new Vector2(vector2.X, vector2.Y), m_game.m_hud.m_color * m_game.m_hud.m_alpha);
		}
	}
}
