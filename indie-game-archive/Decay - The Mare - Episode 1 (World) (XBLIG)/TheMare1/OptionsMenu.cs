using System;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMare1;

public class OptionsMenu : Core.OptionsMenu
{
	protected Texture2D m_fade;

	protected Texture2D m_bkg;

	protected Texture2D m_arrow;

	private bool m_start_menu;

	public OptionsMenu(Core.Game game, bool start_menu)
		: base(game)
	{
		m_start_menu = start_menu;
		m_fade = m_game.m_CL.LoadTexture("HUD/black");
		m_arrow = m_game.m_CL.LoadTexture("OptionsMenu/arrow_white");
	}

	public override void Clear()
	{
		m_fade = null;
		m_arrow = null;
		base.Clear();
	}

	public override void Draw(SpriteBatch SB)
	{
		try
		{
			if (SB != null)
			{
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				Vector2 zero = Vector2.Zero;
				Color white = Color.White;
				Color color = new Color(255, 30, 30, 255);
				Color color2 = new Color(30, 255, 30, 255);
				string text = m_game.m_language.GetString("Brightness");
				Vector2 vector = m_font.MeasureString(text);
				float num = vector.X + 80f + (float)m_arrow.Width + 80f + (float)m_arrow.Width;
				if (m_start_menu)
				{
					zero.X = Core.Game.TS_AREA.Left + 40;
				}
				else
				{
					zero.X = ((float)Core.Game.VIEW_RECT.Width - num) / 2f;
				}
				if (m_start_menu)
				{
					zero.Y = Core.Game.TS_AREA.Top + 180;
				}
				else
				{
					zero.Y = 360f;
				}
				Color color3 = white;
				if (m_selection == OPTIONS_SELECTION.BRIGHTNESS)
				{
					color3 = color;
				}
				SB.DrawString(m_font, text, zero, color3);
				zero.X += vector.X + 80f + (float)m_arrow.Width;
				zero.Y += 5f;
				if (m_arrow_state == OPTIONS_ARROW_STATE.BRIGHTNESS_DECREASE)
				{
					SB.Draw(m_arrow, zero, null, color2, 0f, Vector2.Zero, 0.5f, SpriteEffects.FlipHorizontally, 0f);
				}
				else
				{
					SB.Draw(m_arrow, zero, null, color3, 0f, Vector2.Zero, 0.5f, SpriteEffects.FlipHorizontally, 0f);
				}
				zero.Y -= 5f;
				zero.X += m_arrow.Width;
				float x = zero.X;
				string text2 = ScriptObject.ParseStringFromFloat(m_game.m_game_settings.m_brightness);
				zero.X += 40f - m_font.MeasureString(text2).X / 2f;
				SB.DrawString(m_font, text2, zero, Color.White);
				zero.X = x + 80f;
				zero.Y += 5f;
				if (m_arrow_state == OPTIONS_ARROW_STATE.BRIGHTNESS_INCREASE)
				{
					SB.Draw(m_arrow, zero, null, color2, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
				}
				else
				{
					SB.Draw(m_arrow, zero, null, color3, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
				}
				zero.Y -= 5f;
				if (m_start_menu)
				{
					zero.X = Core.Game.TS_AREA.Left + 40;
				}
				else
				{
					zero.X = ((float)Core.Game.VIEW_RECT.Width - num) / 2f;
				}
				zero.Y += vector.Y;
				text = m_game.m_language.GetString("Sound");
				color3 = white;
				if (m_selection == OPTIONS_SELECTION.SOUND)
				{
					color3 = color;
				}
				SB.DrawString(m_font, text, zero, color3);
				zero.X += vector.X + 80f + (float)m_arrow.Width;
				zero.Y += 5f;
				if (m_arrow_state == OPTIONS_ARROW_STATE.SOUND_DECREASE)
				{
					SB.Draw(m_arrow, zero, null, color2, 0f, Vector2.Zero, 0.5f, SpriteEffects.FlipHorizontally, 0f);
				}
				else
				{
					SB.Draw(m_arrow, zero, null, color3, 0f, Vector2.Zero, 0.5f, SpriteEffects.FlipHorizontally, 0f);
				}
				zero.Y -= 5f;
				zero.X += m_arrow.Width;
				x = zero.X;
				text2 = ScriptObject.ParseStringFromFloat(m_game.m_game_settings.m_sound_volume);
				zero.X += 40f - m_font.MeasureString(text2).X / 2f;
				SB.DrawString(m_font, text2, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
				SB.DrawString(m_font, text2, zero, Color.White);
				zero.X = x + 80f;
				zero.Y += 5f;
				if (m_arrow_state == OPTIONS_ARROW_STATE.SOUND_INCREASE)
				{
					SB.Draw(m_arrow, zero, null, color2, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
				}
				else
				{
					SB.Draw(m_arrow, zero, null, color3, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
				}
				zero.Y -= 5f;
				zero.Y += vector.Y * 2f;
				text = m_game.m_language.GetString("Default");
				vector = m_font.MeasureString(text);
				if (m_start_menu)
				{
					zero.X = Core.Game.TS_AREA.Left + 40;
				}
				else
				{
					zero.X = ((float)Core.Game.VIEW_RECT.Width - vector.X) / 2f;
				}
				color3 = white;
				if (m_selection == OPTIONS_SELECTION.DEFAULT)
				{
					color3 = color;
				}
				SB.DrawString(m_font, text, zero, color3);
				zero.Y += vector.Y * 1.5f;
				text = m_game.m_language.GetString("Back");
				vector = m_font.MeasureString(text);
				if (m_start_menu)
				{
					zero.X = Core.Game.TS_AREA.Left + 40;
				}
				else
				{
					zero.X = ((float)Core.Game.VIEW_RECT.Width - vector.X) / 2f;
				}
				color3 = white;
				if (m_selection == OPTIONS_SELECTION.BACK)
				{
					color3 = color;
				}
				SB.DrawString(m_font, text, zero, color3);
				SB.End();
				if (m_state == OPTIONS_STATE.SAVE_SETTINGS)
				{
					SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
					white = Color.Black * 0.5f;
					SB.Draw(m_fade, Core.Game.VIEW_RECT, white);
					string text3 = m_game.m_language.GetString("Saving, do not turn off your console.");
					Vector2 vector2 = m_font.MeasureString(text3);
					num = vector2.X;
					zero.X = ((float)Core.Game.VIEW_RECT.Width - vector2.X) / 2f;
					zero.Y = (float)Core.Game.TS_AREA.Bottom - vector2.Y;
					SB.DrawString(m_font, text3, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
					SB.DrawString(m_font, text3, zero, Color.White);
					SB.End();
					m_game.m_overlay.Draw(SB);
					m_game.GraphicsDevice.Present();
					m_game.SaveSettings();
					m_game.GraphicsDevice.Clear(Color.Black);
					SB.GraphicsDevice.SetRenderTarget(m_game.m_RT);
					SB.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0f, 0);
					m_state = OPTIONS_STATE.DEFAULT;
					m_game.onOptionsClosed();
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
