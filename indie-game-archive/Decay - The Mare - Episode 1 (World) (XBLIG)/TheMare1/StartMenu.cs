using System;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace TheMare1;

public class StartMenu : Core.StartMenu
{
	private TextureAnimation m_bkg;

	public StartMenu(TheMare1 game)
		: base(game)
	{
		m_bkg = new TextureAnimation(m_game, m_game.m_CL, "StartMenu/Animation/", 1u, reverse: false);
		m_bkg.UseCombinedFrames(640, 360, 3, 1280);
		m_bkg.m_random_mode = true;
		m_bkg.SetFPS(7.0);
		m_bkg.Play();
		if (game != null && game.m_exit_game)
		{
			TriggerCredits();
			return;
		}
		m_game.PlayMusic(2);
		m_game.FadeInMusic();
	}

	public override void Clear()
	{
		if (m_bkg != null)
		{
			m_bkg.Clear();
			m_bkg = null;
		}
		base.Clear();
	}

	protected override Core.OptionsMenu CreateOptionsMenu()
	{
		return new OptionsMenu(m_game, start_menu: true);
	}

	protected override Core.Extras CreateExtras()
	{
		return new Extras(m_game, m_game.m_CL);
	}

	protected override Core.Credits CreateCredits()
	{
		return new Credits(m_game, m_game.m_CL);
	}

	public override void Update(TimeSpan elapsed)
	{
		if (m_bkg != null)
		{
			m_bkg.Update(elapsed);
		}
		base.Update(elapsed);
	}

	public override void Draw(SpriteBatch SB)
	{
		try
		{
			if (SB == null)
			{
				return;
			}
			switch (m_state)
			{
			case STARTMENU_STATE.MAIN:
			{
				m_bkg.Draw(SB);
				SB.Begin();
				Vector2 zero = Vector2.Zero;
				string text = m_game.m_language.GetString("Continue");
				Vector2 vector = m_font2.MeasureString(text);
				string text2 = m_game.m_language.GetString("Unlock Full Game");
				Vector2 vector2 = m_font2.MeasureString(text2);
				zero.X = Core.Game.TS_AREA.Left + 40;
				zero.Y = Core.Game.TS_AREA.Top + 120;
				Color white = Color.White;
				Color color = new Color(255, 30, 30, 255);
				if (m_game.m_game_data_found || Guide.IsTrialMode)
				{
					if (m_selection == STARTMENU_SELECTION.CONTINUE_UNLOCK)
					{
						if (!Guide.IsTrialMode)
						{
							SB.DrawString(m_font2, text, zero, color);
						}
						else
						{
							SB.DrawString(m_font2, text2, zero, color);
						}
					}
					else if (!Guide.IsTrialMode)
					{
						SB.DrawString(m_font2, text, zero, white);
					}
					else
					{
						SB.DrawString(m_font2, text2, zero, white);
					}
				}
				else if (!Guide.IsTrialMode)
				{
					SB.DrawString(m_font2, text, zero, white * 0.25f);
				}
				else
				{
					SB.DrawString(m_font2, text2, zero, white * 0.25f);
				}
				if (!Guide.IsTrialMode)
				{
					zero.Y += vector.Y;
				}
				else
				{
					zero.Y += vector2.Y;
				}
				string text3 = m_game.m_language.GetString("New Game");
				Vector2 vector3 = m_font2.MeasureString(text3);
				if (m_selection == STARTMENU_SELECTION.NEW_GAME)
				{
					SB.DrawString(m_font2, text3, zero, color);
				}
				else
				{
					SB.DrawString(m_font2, text3, zero, white);
				}
				zero.Y += vector3.Y;
				text3 = m_game.m_language.GetString("Options");
				vector3 = m_font2.MeasureString(text3);
				if (m_selection == STARTMENU_SELECTION.OPTIONS)
				{
					SB.DrawString(m_font2, text3, zero, color);
				}
				else
				{
					SB.DrawString(m_font2, text3, zero, white);
				}
				zero.Y += vector3.Y;
				text3 = m_game.m_language.GetString("Extras");
				vector3 = m_font2.MeasureString(text3);
				if (m_selection == STARTMENU_SELECTION.EXTRAS)
				{
					SB.DrawString(m_font2, text3, zero, color);
				}
				else
				{
					SB.DrawString(m_font2, text3, zero, white);
				}
				zero.Y += vector3.Y;
				text3 = m_game.m_language.GetString("Share");
				vector3 = m_font2.MeasureString(text3);
				if (m_selection == STARTMENU_SELECTION.SHARE)
				{
					SB.DrawString(m_font2, text3, zero, color);
				}
				else
				{
					SB.DrawString(m_font2, text3, zero, white);
				}
				zero.Y += vector3.Y;
				text3 = m_game.m_language.GetString("Credits");
				vector3 = m_font2.MeasureString(text3);
				if (m_selection == STARTMENU_SELECTION.CREDITS)
				{
					SB.DrawString(m_font2, text3, zero, color);
				}
				else
				{
					SB.DrawString(m_font2, text3, zero, white);
				}
				zero.Y += vector3.Y * 2f;
				text3 = m_game.m_language.GetString("Exit Game");
				vector3 = m_font2.MeasureString(text3);
				if (m_selection == STARTMENU_SELECTION.EXIT)
				{
					SB.DrawString(m_font2, text3, zero, color);
				}
				else
				{
					SB.DrawString(m_font2, text3, zero, white);
				}
				SB.End();
				break;
			}
			case STARTMENU_STATE.OPTIONS:
				m_bkg.Draw(SB);
				if (m_options_menu != null)
				{
					m_options_menu.Draw(SB);
				}
				break;
			case STARTMENU_STATE.EXTRAS:
				m_bkg.Draw(SB);
				if (m_extras_menu != null)
				{
					m_extras_menu.Draw(SB);
				}
				break;
			case STARTMENU_STATE.CREDITS:
				if (m_credits_menu != null)
				{
					m_credits_menu.Draw(SB);
				}
				break;
			case STARTMENU_STATE.SHARE:
				break;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
