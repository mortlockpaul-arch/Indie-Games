using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Game;

public class StartMenu
{
	public enum STARTMENU_STATE
	{
		MAIN,
		OPTIONS,
		EXTRAS,
		CREDITS
	}

	public enum STARTMENU_SELECTION
	{
		CONTINUE_UNLOCK,
		NEW_GAME,
		OPTIONS,
		EXTRAS,
		CREDITS,
		EXIT
	}

	public STARTMENU_STATE m_state;

	protected Texture2D m_bkg;

	protected Texture2D m_continue;

	protected Texture2D m_unlock;

	protected Texture2D m_new_game;

	protected Texture2D m_options;

	protected Texture2D m_extras;

	protected Texture2D m_credits;

	protected Texture2D m_exit;

	protected Texture2D m_a_button;

	protected Texture2D m_b_button;

	protected Texture2D m_options_bkg;

	protected SpriteFont m_font;

	protected SpriteFont m_font2;

	protected Game m_game;

	public STARTMENU_SELECTION m_selection;

	public OptionsMenu m_options_menu;

	protected Extras m_extras_menu;

	protected Credits m_credits_menu;

	protected bool m_exit_store;

	public StartMenu(Game game)
	{
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		m_selection = STARTMENU_SELECTION.NEW_GAME;
		base._002Ector();
		m_game = game;
		m_bkg = ((Game)m_game).Content.Load<Texture2D>("StartMenu/bkg");
		m_continue = ((Game)m_game).Content.Load<Texture2D>("StartMenu/continue");
		m_unlock = ((Game)m_game).Content.Load<Texture2D>("StartMenu/unlock");
		m_new_game = ((Game)m_game).Content.Load<Texture2D>("StartMenu/newgame");
		m_options = ((Game)m_game).Content.Load<Texture2D>("StartMenu/options");
		m_extras = ((Game)m_game).Content.Load<Texture2D>("StartMenu/extras");
		m_credits = ((Game)m_game).Content.Load<Texture2D>("StartMenu/credits");
		m_exit = ((Game)m_game).Content.Load<Texture2D>("StartMenu/exit");
		m_options_bkg = ((Game)m_game).Content.Load<Texture2D>("StartMenu/options_bkg");
		m_a_button = ((Game)m_game).Content.Load<Texture2D>("HUD/a_button");
		m_b_button = ((Game)m_game).Content.Load<Texture2D>("HUD/b_button");
		m_font = ((Game)m_game).Content.Load<SpriteFont>("Fonts/SpriteFont1");
		m_font2 = ((Game)m_game).Content.Load<SpriteFont>("Fonts/SpriteFont2");
		m_selection = STARTMENU_SELECTION.NEW_GAME;
		if (m_game.m_game_data_found)
		{
			m_selection = STARTMENU_SELECTION.CONTINUE_UNLOCK;
		}
		m_options_menu = new OptionsMenu(m_game, Vector2.Zero);
		m_extras_menu = new Extras(m_game, m_game.m_CL);
		m_credits_menu = new Credits(m_game, m_game.m_CL);
		if (m_game.m_music3 == (Song)null)
		{
			m_game.m_music3 = ((Game)m_game).Content.Load<Song>("Music/menu");
		}
		if (m_game.m_game_data != null)
		{
			m_game.m_game_data.SetState("Music", "3");
		}
		m_game.PlayMusic(m_game.m_music3);
		m_game.FadeInMusic();
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
		if (m_bkg != null)
		{
			((GraphicsResource)m_bkg).Dispose();
			m_bkg = null;
		}
		if (m_options_bkg != null)
		{
			((GraphicsResource)m_options_bkg).Dispose();
			m_options_bkg = null;
		}
		((GraphicsResource)m_continue).Dispose();
		m_continue = null;
		((GraphicsResource)m_unlock).Dispose();
		m_unlock = null;
		((GraphicsResource)m_new_game).Dispose();
		m_new_game = null;
		((GraphicsResource)m_options).Dispose();
		m_options = null;
		((GraphicsResource)m_extras).Dispose();
		m_extras = null;
		((GraphicsResource)m_exit).Dispose();
		m_exit = null;
		((GraphicsResource)m_a_button).Dispose();
		m_a_button = null;
		((GraphicsResource)m_b_button).Dispose();
		m_b_button = null;
		m_font = null;
		m_font2 = null;
	}

	protected virtual void UpdateMain(TimeSpan elapsed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Invalid comparison between Unknown and I4
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Invalid comparison between Unknown and I4
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		KeyboardState state = Keyboard.GetState();
		GamePadState state2 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad = ((GamePadState)(ref state2)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Down != 1)
		{
			GamePadState state3 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref state3)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks)).Left.Y < -0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)40))
			{
				m_game.m_down_pressed = false;
				goto IL_00dc;
			}
		}
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
				if (m_selection == STARTMENU_SELECTION.EXTRAS && !m_game.m_game_settings.m_extras_unlocked)
				{
					m_selection = STARTMENU_SELECTION.CREDITS;
				}
			}
		}
		goto IL_00dc;
		IL_01f7:
		GamePadState state4 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadButtons buttons = ((GamePadState)(ref state4)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)13))
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
				catch (Exception ex)
				{
					Console.WriteLine("Failed to show marketplace: " + ex.Message);
					Guide.BeginShowMessageBox(Game.PLAYER_INDEX, "Message", ex.Message, (IEnumerable<string>)new string[1] { "Ok" }, 0, (MessageBoxIcon)0, (AsyncCallback)onMessageFinished, (object)object.Equals(0, 0));
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
			case STARTMENU_SELECTION.CREDITS:
				m_credits_menu.Reset();
				m_state = STARTMENU_STATE.CREDITS;
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
						((Game)m_game).Exit();
					}
					break;
				}
				catch
				{
					((Game)m_game).Exit();
					break;
				}
			}
		}
		else
		{
			m_game.m_a_pressed = false;
		}
		return;
		IL_00dc:
		GamePadState state5 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad2 = ((GamePadState)(ref state5)).DPad;
		if ((int)((GamePadDPad)(ref dPad2)).Up != 1)
		{
			GamePadState state6 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref state6)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks2)).Left.Y > 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)38))
			{
				m_game.m_up_pressed = false;
				goto IL_01f7;
			}
		}
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
					if (m_selection == STARTMENU_SELECTION.EXTRAS && !m_game.m_game_settings.m_extras_unlocked)
					{
						m_selection = STARTMENU_SELECTION.OPTIONS;
					}
				}
			}
			else if (m_selection == STARTMENU_SELECTION.NEW_GAME)
			{
				m_selection = STARTMENU_SELECTION.EXIT;
			}
			else
			{
				m_selection--;
				if (m_selection == STARTMENU_SELECTION.EXTRAS && !m_game.m_game_settings.m_extras_unlocked)
				{
					m_selection = STARTMENU_SELECTION.OPTIONS;
				}
			}
		}
		goto IL_01f7;
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
		if (m_exit_store)
		{
			if (!Guide.IsVisible)
			{
				m_exit_store = false;
				((Game)m_game).Exit();
			}
			return;
		}
		switch (m_state)
		{
		case STARTMENU_STATE.MAIN:
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

	public virtual void Draw(SpriteBatch SB)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		if (SB == null)
		{
			return;
		}
		switch (m_state)
		{
		case STARTMENU_STATE.MAIN:
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_bkg, Game.VIEW_RECT, Color.White);
			Vector2 zero = Vector2.Zero;
			if (!Guide.IsTrialMode)
			{
				zero.X = (Game.VIEW_RECT.Width - m_continue.Width) / 2;
			}
			else
			{
				zero.X = (Game.VIEW_RECT.Width - m_unlock.Width) / 2;
			}
			zero.Y = 340f;
			Color white = Color.White;
			Color val = default(Color);
			((Color)(ref val))._002Ector(byte.MaxValue, (byte)30, (byte)30, byte.MaxValue);
			if (m_game.m_game_data_found || Guide.IsTrialMode)
			{
				if (m_selection == STARTMENU_SELECTION.CONTINUE_UNLOCK)
				{
					if (!Guide.IsTrialMode)
					{
						SB.Draw(m_continue, zero, val);
					}
					else
					{
						SB.Draw(m_unlock, zero, val);
					}
				}
				else if (!Guide.IsTrialMode)
				{
					SB.Draw(m_continue, zero, white);
				}
				else
				{
					SB.Draw(m_unlock, zero, white);
				}
			}
			else
			{
				((Color)(ref white)).A = 64;
				if (!Guide.IsTrialMode)
				{
					SB.Draw(m_continue, zero, white);
				}
				else
				{
					SB.Draw(m_unlock, zero, white);
				}
				((Color)(ref white)).A = byte.MaxValue;
			}
			zero.X = (Game.VIEW_RECT.Width - m_new_game.Width) / 2;
			if (!Guide.IsTrialMode)
			{
				zero.Y += (float)m_continue.Height;
			}
			else
			{
				zero.Y += (float)m_unlock.Height;
			}
			if (m_selection == STARTMENU_SELECTION.NEW_GAME)
			{
				SB.Draw(m_new_game, zero, val);
			}
			else
			{
				SB.Draw(m_new_game, zero, white);
			}
			zero.X = (Game.VIEW_RECT.Width - m_options.Width) / 2;
			zero.Y += (float)(m_new_game.Height + 2);
			if (m_selection == STARTMENU_SELECTION.OPTIONS)
			{
				SB.Draw(m_options, zero, val);
			}
			else
			{
				SB.Draw(m_options, zero, white);
			}
			zero.X = (Game.VIEW_RECT.Width - m_extras.Width) / 2;
			zero.Y += (float)m_options.Height;
			if (m_game.m_game_settings.m_extras_unlocked)
			{
				if (m_selection == STARTMENU_SELECTION.EXTRAS)
				{
					SB.Draw(m_extras, zero, val);
				}
				else
				{
					SB.Draw(m_extras, zero, white);
				}
			}
			else
			{
				((Color)(ref white)).A = 64;
				SB.Draw(m_extras, zero, white);
				((Color)(ref white)).A = byte.MaxValue;
			}
			zero.X = (Game.VIEW_RECT.Width - m_credits.Width) / 2;
			zero.Y += (float)(m_extras.Height + 2);
			if (m_selection == STARTMENU_SELECTION.CREDITS)
			{
				SB.Draw(m_credits, zero, val);
			}
			else
			{
				SB.Draw(m_credits, zero, white);
			}
			zero.X = (Game.VIEW_RECT.Width - m_exit.Width) / 2;
			zero.Y += (float)(m_credits.Height + 5);
			if (m_selection == STARTMENU_SELECTION.EXIT)
			{
				SB.Draw(m_exit, zero, val);
			}
			else
			{
				SB.Draw(m_exit, zero, white);
			}
			SB.End();
			break;
		}
		case STARTMENU_STATE.OPTIONS:
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_options_bkg, Game.VIEW_RECT, Color.White);
			SB.End();
			if (m_options_menu != null)
			{
				m_options_menu.Draw(SB);
			}
			break;
		case STARTMENU_STATE.EXTRAS:
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_bkg, new Rectangle(0, -Game.VIEW_RECT.Height, Game.VIEW_RECT.Width, (int)((float)Game.VIEW_RECT.Height * 2f)), Color.White);
			SB.End();
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
		}
	}
}
