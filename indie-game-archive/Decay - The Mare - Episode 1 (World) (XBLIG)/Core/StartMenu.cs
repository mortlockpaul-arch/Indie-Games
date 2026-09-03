using System;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Core;

public class StartMenu
{
	public enum STARTMENU_STATE
	{
		MAIN,
		OPTIONS,
		EXTRAS,
		SHARE,
		CREDITS
	}

	public enum STARTMENU_SELECTION
	{
		NONE,
		CONTINUE_UNLOCK,
		NEW_GAME,
		OPTIONS,
		EXTRAS,
		SHARE,
		CREDITS,
		EXIT
	}

	public STARTMENU_STATE m_state;

	protected SpriteFont m_font;

	protected SpriteFont m_font2;

	protected Game m_game;

	public STARTMENU_SELECTION m_selection = STARTMENU_SELECTION.NEW_GAME;

	public OptionsMenu m_options_menu;

	protected Extras m_extras_menu;

	protected Credits m_credits_menu;

	protected bool m_exit_store;

	public StartMenu(Game game)
	{
		m_game = game;
		m_font = m_game.Content.Load<SpriteFont>("Fonts/SpriteFont1");
		m_font2 = m_game.Content.Load<SpriteFont>("Fonts/SpriteFont2");
		m_selection = STARTMENU_SELECTION.NEW_GAME;
		if (m_game.m_game_data_found)
		{
			m_selection = STARTMENU_SELECTION.CONTINUE_UNLOCK;
		}
		m_options_menu = CreateOptionsMenu();
		m_extras_menu = CreateExtras();
		m_credits_menu = CreateCredits();
		if (m_game.m_game_data != null)
		{
			m_game.m_game_data.SetState("Music", "0");
		}
	}

	public virtual void Clear()
	{
		m_game = null;
		if (m_options_menu != null)
		{
			m_options_menu.Clear();
			m_options_menu = null;
		}
		if (m_extras_menu != null)
		{
			m_extras_menu.Clear();
			m_extras_menu = null;
		}
		if (m_credits_menu != null)
		{
			m_credits_menu.Clear();
			m_credits_menu = null;
		}
		m_font = null;
		m_font2 = null;
	}

	protected virtual OptionsMenu CreateOptionsMenu()
	{
		return null;
	}

	protected virtual Extras CreateExtras()
	{
		return null;
	}

	protected virtual Credits CreateCredits()
	{
		return null;
	}

	protected virtual void TriggerCredits()
	{
		try
		{
			m_credits_menu.Reset();
			m_state = STARTMENU_STATE.CREDITS;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected virtual void UpdateMain(TimeSpan elapsed)
	{
		KeyboardState state = Keyboard.GetState();
		if (GamePad.GetState(Game.PLAYER_INDEX).DPad.Down == ButtonState.Pressed || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.Y < -0.2f || state.IsKeyDown(Keys.Down))
		{
			if (!m_game.m_down_pressed)
			{
				m_game.m_down_pressed = true;
				if (m_selection == STARTMENU_SELECTION.EXIT)
				{
					if (m_game.m_game_data_found || Guide.IsTrialMode)
					{
						m_selection = STARTMENU_SELECTION.CONTINUE_UNLOCK;
					}
					else
					{
						m_selection = STARTMENU_SELECTION.NEW_GAME;
					}
				}
				else
				{
					m_selection++;
				}
			}
		}
		else
		{
			m_game.m_down_pressed = false;
		}
		if (GamePad.GetState(Game.PLAYER_INDEX).DPad.Up == ButtonState.Pressed || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.Y > 0.2f || state.IsKeyDown(Keys.Up))
		{
			if (!m_game.m_up_pressed)
			{
				m_game.m_up_pressed = true;
				if (m_game.m_game_data_found || Guide.IsTrialMode)
				{
					if (m_selection == STARTMENU_SELECTION.CONTINUE_UNLOCK)
					{
						m_selection = STARTMENU_SELECTION.EXIT;
					}
					else
					{
						m_selection--;
					}
				}
				else if (m_selection == STARTMENU_SELECTION.NEW_GAME)
				{
					m_selection = STARTMENU_SELECTION.EXIT;
				}
				else
				{
					m_selection--;
				}
			}
		}
		else
		{
			m_game.m_up_pressed = false;
		}
		if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter))
		{
			if (m_game.m_a_pressed)
			{
				return;
			}
			m_game.m_a_pressed = true;
			switch (m_selection)
			{
			case STARTMENU_SELECTION.CONTINUE_UNLOCK:
				if (!Guide.IsTrialMode)
				{
					m_game.onContinueGame();
					break;
				}
				try
				{
					Guide.ShowMarketplace(Game.PLAYER_INDEX);
					m_selection = STARTMENU_SELECTION.NEW_GAME;
					break;
				}
				catch (Exception ex2)
				{
					Console.WriteLine("Failed to show marketplace: " + ex2.Message);
					Guide.BeginShowMessageBox(Game.PLAYER_INDEX, "Message", ex2.Message, new string[1] { "Ok" }, 0, MessageBoxIcon.None, onMessageFinished, object.Equals(0, 0));
					break;
				}
			case STARTMENU_SELECTION.NEW_GAME:
				m_game.onNewGame();
				break;
			case STARTMENU_SELECTION.OPTIONS:
				m_state = STARTMENU_STATE.OPTIONS;
				break;
			case STARTMENU_SELECTION.EXTRAS:
				m_state = STARTMENU_STATE.EXTRAS;
				m_extras_menu.Reset();
				break;
			case STARTMENU_SELECTION.SHARE:
				try
				{
					Guide.ShowComposeMessage(Game.PLAYER_INDEX, m_game.m_language.GetString("Check out 'Decay: The Mare' on Xbox Live Indie Games!"), null);
					break;
				}
				catch (Exception ex)
				{
					Console.WriteLine("Guide.ShowComposeMessage failed: " + ex.Message);
					Guide.BeginShowMessageBox(Game.PLAYER_INDEX, "Message", ex.Message, new string[1] { "Ok" }, 0, MessageBoxIcon.None, onMessageFinished, object.Equals(0, 0));
					break;
				}
			case STARTMENU_SELECTION.CREDITS:
				TriggerCredits();
				break;
			case STARTMENU_SELECTION.EXIT:
				try
				{
					if (Guide.IsTrialMode)
					{
						Guide.ShowMarketplace(Game.PLAYER_INDEX);
						m_exit_store = true;
					}
					else
					{
						m_game.Exit();
					}
					break;
				}
				catch
				{
					m_game.Exit();
					break;
				}
			}
		}
		else
		{
			m_game.m_a_pressed = false;
		}
	}

	protected void onMessageFinished(IAsyncResult res)
	{
		try
		{
			Guide.EndShowMessageBox(res);
		}
		catch
		{
		}
	}

	public virtual void Update(TimeSpan elapsed)
	{
		try
		{
			if (m_exit_store)
			{
				if (!Guide.IsVisible)
				{
					m_exit_store = false;
					m_game.Exit();
				}
				return;
			}
			switch (m_state)
			{
			case STARTMENU_STATE.MAIN:
			case STARTMENU_STATE.SHARE:
				UpdateMain(elapsed);
				break;
			case STARTMENU_STATE.OPTIONS:
				if (m_options_menu != null)
				{
					m_options_menu.Update(elapsed);
				}
				break;
			case STARTMENU_STATE.EXTRAS:
				if (m_extras_menu != null)
				{
					m_extras_menu.Update(elapsed);
				}
				break;
			case STARTMENU_STATE.CREDITS:
				if (m_credits_menu != null)
				{
					m_credits_menu.Update(elapsed);
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
	}
}
