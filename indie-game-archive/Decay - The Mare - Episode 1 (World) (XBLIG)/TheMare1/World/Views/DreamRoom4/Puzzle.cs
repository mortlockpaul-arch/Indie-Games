using System;
using Core;
using Core.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheMare1.World.Views.DreamRoom4;

internal class Puzzle : View
{
	public Puzzle(Core.Game game, Area room, string xml_path)
		: base(game, room, xml_path)
	{
		m_images.Add("LS", new Image(getContentLoader().LoadTexture("HUD/LS"), new Rectangle(Core.Game.TS_AREA.X, Core.Game.TS_AREA.Bottom - 44 + 7, 44, 44)));
		m_images.Add("A", new Image(getContentLoader().LoadTexture("HUD/a_button"), new Rectangle(Core.Game.TS_AREA.X + 8, Core.Game.TS_AREA.Bottom - 44 + 7 - 40, 27, 30)));
	}

	public override void Clear()
	{
		base.Clear();
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "Puzzle.onReset")
		{
			m_game.m_left_pressed = true;
			m_game.m_right_pressed = true;
			m_game.m_a_pressed = true;
			m_game.m_b_pressed = true;
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		try
		{
			base.Update(elapsed);
			if (!m_game.m_input_enabled)
			{
				return;
			}
			KeyboardState state = Keyboard.GetState();
			if (state.IsKeyDown(Keys.A) || GamePad.GetState(Core.Game.PLAYER_INDEX).IsButtonDown(Buttons.A))
			{
				if (!m_game.m_a_pressed)
				{
					m_game.m_a_pressed = true;
					string text = "";
					switch (m_local_states["CurrentArrow"])
					{
					case "Hour":
						text = "Minute";
						break;
					case "Minute":
						text = "Second";
						break;
					case "Second":
						text = "Hour";
						break;
					}
					m_game.HandleEvent(m_name + ".SelectArrow" + text);
				}
			}
			else
			{
				m_game.m_a_pressed = false;
			}
			if (state.IsKeyDown(Keys.Left) || GamePad.GetState(Core.Game.PLAYER_INDEX).ThumbSticks.Left.X < -0.2f || GamePad.GetState(Core.Game.PLAYER_INDEX).DPad.Left == ButtonState.Pressed)
			{
				if (!m_game.m_left_pressed)
				{
					m_game.m_left_pressed = true;
					HandleEvent(m_name + ".Arrow" + m_local_states["CurrentArrow"] + ".onLeft");
				}
				return;
			}
			m_game.m_left_pressed = false;
			if (state.IsKeyDown(Keys.Right) || GamePad.GetState(Core.Game.PLAYER_INDEX).ThumbSticks.Left.X > 0.2f || GamePad.GetState(Core.Game.PLAYER_INDEX).DPad.Right == ButtonState.Pressed)
			{
				if (!m_game.m_right_pressed)
				{
					m_game.m_right_pressed = true;
					HandleEvent(m_name + ".Arrow" + m_local_states["CurrentArrow"] + ".onRight");
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
		string text = "";
		text = m_game.m_language.GetString("SELECT CLOCK HAND");
		Vector2 vector = m_game.m_hud.m_font.MeasureString(text);
		Vector2 vector2 = default(Vector2);
		vector2.X = Core.Game.TS_AREA.X + 44 + 5;
		vector2.Y = (float)Core.Game.TS_AREA.Bottom - vector.Y - (float)m_images["LS"].m_texture.Height - 5f;
		m_images["A"].Draw(SB, m_game.m_hud.m_color * m_game.m_hud.m_alpha);
		m_game.m_hud.DrawText(SB, text, new Vector2(vector2.X, vector2.Y), m_game.m_hud.m_color * m_game.m_hud.m_alpha);
		text = m_game.m_language.GetString("ROTATE CLOCK HAND");
		vector = m_game.m_hud.m_font.MeasureString(text);
		vector2.X = Core.Game.TS_AREA.X + 44 + 5;
		vector2.Y = (float)Core.Game.TS_AREA.Bottom - vector.Y;
		m_images["LS"].Draw(SB, m_game.m_hud.m_color * m_game.m_hud.m_alpha);
		m_game.m_hud.DrawText(SB, text, new Vector2(vector2.X, vector2.Y), m_game.m_hud.m_color * m_game.m_hud.m_alpha);
	}
}
