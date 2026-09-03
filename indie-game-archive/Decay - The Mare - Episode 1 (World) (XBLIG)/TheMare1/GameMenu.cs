using System;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMare1;

public class GameMenu : Core.GameMenu
{
	protected Texture2D m_fade;

	protected Texture2D m_controls_bkg;

	public GameMenu(TheMare1 game)
		: base(game)
	{
		m_fade = m_game.m_fade_texture;
		m_controls_bkg = m_game.m_CL.LoadTexture("GameMenu/controllers");
		m_font = m_game.m_CL.LoadFont("Fonts/SpriteFont1");
		m_font2 = m_game.m_CL.LoadFont("Fonts/SpriteFont2");
		m_selection = GAMEMENU_SELECTION.RESUME;
	}

	public override void Clear()
	{
		m_fade = null;
		m_controls_bkg = null;
		base.Clear();
	}

	protected override Core.OptionsMenu CreateOptionsMenu()
	{
		return new OptionsMenu(m_game, start_menu: false);
	}

	public override void Draw(SpriteBatch SB)
	{
		try
		{
			if (m_trial_message || SB.GraphicsDevice.IsDisposed || SB.GraphicsDevice.GraphicsDeviceStatus != GraphicsDeviceStatus.Normal || SB == null)
			{
				return;
			}
			if (m_state == GAMEMENU_STATE.ASK_EXIT)
			{
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_fade, Core.Game.VIEW_RECT, Color.White * 0.75f);
				string text = m_game.m_language.GetString("All unsaved progress will be lost.");
				string text2 = m_game.m_language.GetString("Do you really want to exit?");
				Vector2 vector = m_font2.MeasureString(text);
				Vector2 vector2 = m_font2.MeasureString(text2);
				float x = vector.X;
				Vector2 zero = Vector2.Zero;
				zero.X = ((float)Core.Game.VIEW_RECT.Width - vector.X) / 2f;
				zero.Y = ((float)Core.Game.VIEW_RECT.Height - vector.Y) / 2f - 50f;
				SB.DrawString(m_font2, text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
				SB.DrawString(m_font2, text, zero, Color.White);
				zero.X = ((float)Core.Game.VIEW_RECT.Width - vector2.X) / 2f;
				zero.Y += vector.Y;
				SB.DrawString(m_font2, text2, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
				SB.DrawString(m_font2, text2, zero, Color.White);
				text = m_game.m_language.GetString("EXIT");
				text2 = m_game.m_language.GetString("CANCEL");
				vector = m_font.MeasureString(text);
				vector2 = m_font.MeasureString(text2);
				x = (float)(m_game.m_hud.m_a_button.Width + 10) + vector.X + 40f + (float)m_game.m_hud.m_b_button.Width + 10f + vector2.X;
				zero.X = ((float)Core.Game.VIEW_RECT.Width - x) / 2f;
				zero.Y += vector.Y * 3f;
				SB.Draw(m_game.m_hud.m_a_button, zero, Color.White);
				zero.X += m_game.m_hud.m_a_button.Width + 10;
				SB.DrawString(m_font, text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
				SB.DrawString(m_font, text, zero, Color.White);
				zero.X += vector.X + 40f;
				SB.Draw(m_game.m_hud.m_b_button, zero, Color.White);
				zero.X += m_game.m_hud.m_b_button.Width + 10;
				SB.DrawString(m_font, text2, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
				SB.DrawString(m_font, text2, zero, Color.White);
				SB.End();
				return;
			}
			if (m_state == GAMEMENU_STATE.ASK_OVERWRITE)
			{
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_fade, Core.Game.VIEW_RECT, Color.White * 0.75f);
				string text3 = m_game.m_language.GetString("Overwrite last save?");
				Vector2 vector3 = m_font2.MeasureString(text3);
				Vector2 zero2 = Vector2.Zero;
				float x2 = vector3.X;
				Vector2 zero3 = Vector2.Zero;
				zero3.X = ((float)Core.Game.VIEW_RECT.Width - vector3.X) / 2f;
				zero3.Y = ((float)Core.Game.VIEW_RECT.Height - vector3.Y) / 2f - 50f;
				SB.DrawString(m_font2, text3, new Vector2(zero3.X + 1f, zero3.Y + 2f), Color.Black);
				SB.DrawString(m_font2, text3, zero3, Color.White);
				string text4 = "";
				text3 = m_game.m_language.GetString("YES");
				text4 = m_game.m_language.GetString("CANCEL");
				vector3 = m_font.MeasureString(text3);
				zero2 = m_font.MeasureString(text4);
				x2 = (float)(m_game.m_hud.m_a_button.Width + 10) + vector3.X + 40f + (float)m_game.m_hud.m_b_button.Width + 10f + zero2.X;
				zero3.X = ((float)Core.Game.VIEW_RECT.Width - x2) / 2f;
				zero3.Y += vector3.Y * 3f;
				SB.Draw(m_game.m_hud.m_a_button, zero3, Color.White);
				zero3.X += m_game.m_hud.m_a_button.Width + 10;
				SB.DrawString(m_font, text3, new Vector2(zero3.X + 1f, zero3.Y + 2f), Color.Black);
				SB.DrawString(m_font, text3, zero3, Color.White);
				zero3.X += vector3.X + 40f;
				SB.Draw(m_game.m_hud.m_b_button, zero3, Color.White);
				zero3.X += m_game.m_hud.m_b_button.Width + 10;
				SB.DrawString(m_font, text4, new Vector2(zero3.X + 1f, zero3.Y + 2f), Color.Black);
				SB.DrawString(m_font, text4, zero3, Color.White);
				SB.End();
				return;
			}
			switch (m_state)
			{
			case GAMEMENU_STATE.MENU:
			case GAMEMENU_STATE.SAVE:
			{
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_fade, Core.Game.VIEW_RECT, Color.White * 0.75f);
				Vector2 zero5 = Vector2.Zero;
				zero5.Y = 300f;
				Color white = Color.White;
				Color color = new Color(255, 30, 30, 255);
				string text6 = m_game.m_language.GetString("Resume");
				Vector2 vector5 = m_font2.MeasureString(text6);
				zero5.X = ((float)Core.Game.VIEW_RECT.Width - vector5.X) / 2f;
				if (m_selection == GAMEMENU_SELECTION.RESUME)
				{
					SB.DrawString(m_font2, text6, zero5, color);
				}
				else
				{
					SB.DrawString(m_font2, text6, zero5, white);
				}
				zero5.Y += vector5.Y;
				text6 = m_game.m_language.GetString("Options");
				vector5 = m_font2.MeasureString(text6);
				zero5.X = ((float)Core.Game.VIEW_RECT.Width - vector5.X) / 2f;
				if (m_selection == GAMEMENU_SELECTION.OPTIONS)
				{
					SB.DrawString(m_font2, text6, zero5, color);
				}
				else
				{
					SB.DrawString(m_font2, text6, zero5, white);
				}
				zero5.Y += vector5.Y;
				text6 = m_game.m_language.GetString("Controls");
				vector5 = m_font2.MeasureString(text6);
				zero5.X = ((float)Core.Game.VIEW_RECT.Width - vector5.X) / 2f;
				if (m_selection == GAMEMENU_SELECTION.CONTROLS)
				{
					SB.DrawString(m_font2, text6, zero5, color);
				}
				else
				{
					SB.DrawString(m_font2, text6, zero5, white);
				}
				zero5.Y += vector5.Y;
				text6 = m_game.m_language.GetString("Save");
				vector5 = m_font2.MeasureString(text6);
				zero5.X = ((float)Core.Game.VIEW_RECT.Width - vector5.X) / 2f;
				if (m_selection == GAMEMENU_SELECTION.SAVE)
				{
					SB.DrawString(m_font2, text6, zero5, color);
				}
				else
				{
					SB.DrawString(m_font2, text6, zero5, white);
				}
				zero5.Y += vector5.Y;
				text6 = m_game.m_language.GetString("Exit");
				vector5 = m_font2.MeasureString(text6);
				zero5.X = ((float)Core.Game.VIEW_RECT.Width - vector5.X) / 2f;
				if (m_selection == GAMEMENU_SELECTION.EXIT)
				{
					SB.DrawString(m_font2, text6, zero5, color);
				}
				else
				{
					SB.DrawString(m_font2, text6, zero5, white);
				}
				SB.End();
				if (m_state == GAMEMENU_STATE.SAVE)
				{
					SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
					SB.Draw(m_fade, Core.Game.VIEW_RECT, Color.Black * 0.5f);
					string text7 = m_game.m_language.GetString("Saving, do not turn off your console.");
					Vector2 vector6 = m_font2.MeasureString(text7);
					zero5.X = ((float)Core.Game.VIEW_RECT.Width - vector6.X) / 2f;
					zero5.Y = (float)Core.Game.TS_AREA.Bottom - vector6.Y;
					SB.DrawString(m_font2, text7, new Vector2(zero5.X + 1f, zero5.Y + 2f), Color.Black);
					SB.DrawString(m_font2, text7, zero5, Color.White);
					SB.End();
					m_game.m_overlay.Draw(SB);
					m_game.GraphicsDevice.Present();
					m_game.SaveGameData();
					m_game.SaveSettings();
					m_game.GraphicsDevice.Clear(Color.Black);
					SB.GraphicsDevice.SetRenderTarget(m_game.m_RT);
					SB.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0f, 0);
					m_state = GAMEMENU_STATE.MENU;
				}
				break;
			}
			case GAMEMENU_STATE.OPTIONS:
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_fade, Core.Game.VIEW_RECT, Color.White * 0.75f);
				SB.End();
				if (m_options_menu != null)
				{
					m_options_menu.Draw(SB);
				}
				break;
			case GAMEMENU_STATE.CONTROLS:
				SB.GraphicsDevice.Clear(Color.Black);
				SB.GraphicsDevice.SetRenderTarget(m_game.m_RT);
				SB.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0f, 0);
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_controls_bkg, new Rectangle(Core.Game.TS_AREA.X - 20, Core.Game.TS_AREA.Y - 20, Core.Game.TS_AREA.Width + 40, Core.Game.TS_AREA.Height + 40), Color.White);
				SB.End();
				if (m_game.m_hud.m_b_button != null)
				{
					SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
					string text5 = m_game.m_language.GetString("BACK");
					Vector2 vector4 = m_font.MeasureString(text5);
					Vector2 zero4 = Vector2.Zero;
					zero4.X = (float)Core.Game.TS_AREA.Right - vector4.X;
					zero4.Y = (float)Core.Game.TS_AREA.Bottom - vector4.Y;
					SB.DrawString(m_font, text5, new Vector2(zero4.X + 1f, zero4.Y + 2f), Color.Black);
					SB.DrawString(m_font, text5, zero4, Color.White);
					zero4.X -= m_game.m_hud.m_b_button.Width + 10;
					SB.Draw(m_game.m_hud.m_b_button, zero4, Color.White);
					SB.End();
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
