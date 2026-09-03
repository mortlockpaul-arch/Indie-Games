using System;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Core;

public class GameMenu
{
	public enum GAMEMENU_STATE
	{
		MENU,
		OPTIONS,
		CONTROLS,
		SAVE,
		ASK_EXIT,
		ASK_OVERWRITE
	}

	protected enum GAMEMENU_SELECTION
	{
		NONE,
		RESUME,
		OPTIONS,
		CONTROLS,
		SAVE,
		EXIT
	}

	public GAMEMENU_STATE m_state;

	protected SpriteFont m_font;

	protected SpriteFont m_font2;

	protected Game m_game;

	protected GAMEMENU_SELECTION m_selection = GAMEMENU_SELECTION.RESUME;

	protected OptionsMenu m_options_menu;

	protected bool m_trial_message;

	private bool m_goto_store;

	public GameMenu(Game game)
	{
		m_game = game;
		m_font = m_game.m_CL.LoadFont("Fonts/SpriteFont1");
		m_font2 = m_game.m_CL.LoadFont("Fonts/SpriteFont2");
		m_selection = GAMEMENU_SELECTION.RESUME;
		m_options_menu = CreateOptionsMenu();
	}

	public virtual void Clear()
	{
		m_game = null;
		if (m_options_menu != null)
		{
			m_options_menu.Clear();
			m_options_menu = null;
		}
		m_font = null;
		m_font2 = null;
	}

	protected virtual OptionsMenu CreateOptionsMenu()
	{
		return null;
	}

	protected virtual void UpdateMain(TimeSpan elapsed)
	{
		KeyboardState state = Keyboard.GetState();
		if (GamePad.GetState(Game.PLAYER_INDEX).DPad.Down == ButtonState.Pressed || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.Y < -0.2f || state.IsKeyDown(Keys.Down))
		{
			if (!m_game.m_down_pressed)
			{
				m_game.m_down_pressed = true;
				if (m_selection == GAMEMENU_SELECTION.EXIT)
				{
					m_selection = GAMEMENU_SELECTION.RESUME;
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
				if (m_selection == GAMEMENU_SELECTION.RESUME)
				{
					m_selection = GAMEMENU_SELECTION.EXIT;
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
			case GAMEMENU_SELECTION.RESUME:
				m_game.m_show_game_menu = false;
				if (m_game.m_inventory != null)
				{
					m_game.m_inventory.onGameMenuClosed();
				}
				m_game.HandleEvent("Game.Resume");
				break;
			case GAMEMENU_SELECTION.OPTIONS:
				m_state = GAMEMENU_STATE.OPTIONS;
				break;
			case GAMEMENU_SELECTION.CONTROLS:
				m_state = GAMEMENU_STATE.CONTROLS;
				break;
			case GAMEMENU_SELECTION.SAVE:
				if (!Guide.IsTrialMode)
				{
					if (m_game.m_game_data_found)
					{
						m_state = GAMEMENU_STATE.ASK_OVERWRITE;
					}
					else
					{
						m_state = GAMEMENU_STATE.SAVE;
					}
				}
				else
				{
					m_trial_message = true;
					Guide.BeginShowMessageBox(Game.PLAYER_INDEX, "Message", m_game.m_language.GetString("This feature is only available in the full version."), new string[2]
					{
						m_game.m_language.GetString("Unlock full game"),
						"Ok"
					}, 1, MessageBoxIcon.None, onMessageFinished, object.Equals(0, 0));
				}
				break;
			case GAMEMENU_SELECTION.EXIT:
				m_state = GAMEMENU_STATE.ASK_EXIT;
				break;
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
			int? num = Guide.EndShowMessageBox(res);
			int? num2 = num;
			if (num2.GetValueOrDefault() == 0 && num2.HasValue)
			{
				m_goto_store = true;
			}
			m_trial_message = false;
		}
		catch
		{
		}
	}

	protected void onMessage2Finished(IAsyncResult res)
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
			if (m_goto_store)
			{
				if (!Guide.IsVisible)
				{
					m_goto_store = false;
					try
					{
						Guide.ShowMarketplace(Game.PLAYER_INDEX);
						return;
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						Guide.BeginShowMessageBox(Game.PLAYER_INDEX, "Message", ex.Message, new string[1] { "Ok" }, 0, MessageBoxIcon.None, onMessage2Finished, object.Equals(0, 0));
						return;
					}
				}
				return;
			}
			KeyboardState state = Keyboard.GetState();
			if (m_trial_message)
			{
				return;
			}
			switch (m_state)
			{
			case GAMEMENU_STATE.ASK_EXIT:
				if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.A))
				{
					if (!m_game.m_a_pressed)
					{
						m_game.m_a_pressed = true;
						m_game.onExitGame();
						break;
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
						m_state = GAMEMENU_STATE.MENU;
					}
				}
				else
				{
					m_game.m_b_pressed = false;
				}
				break;
			case GAMEMENU_STATE.ASK_OVERWRITE:
				if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.A))
				{
					if (!m_game.m_a_pressed)
					{
						m_game.m_a_pressed = true;
						m_state = GAMEMENU_STATE.SAVE;
						break;
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
						m_state = GAMEMENU_STATE.MENU;
					}
				}
				else
				{
					m_game.m_b_pressed = false;
				}
				break;
			case GAMEMENU_STATE.MENU:
				UpdateMain(elapsed);
				break;
			case GAMEMENU_STATE.OPTIONS:
				if (m_options_menu != null)
				{
					m_options_menu.Update(elapsed);
				}
				break;
			case GAMEMENU_STATE.CONTROLS:
				if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.B == ButtonState.Pressed || state.IsKeyDown(Keys.B))
				{
					if (!m_game.m_b_pressed)
					{
						m_game.m_b_pressed = true;
						m_state = GAMEMENU_STATE.MENU;
					}
				}
				else
				{
					m_game.m_b_pressed = false;
				}
				break;
			case GAMEMENU_STATE.SAVE:
				break;
			}
		}
		catch (Exception ex2)
		{
			Console.WriteLine(ex2.Message);
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
	}
}
