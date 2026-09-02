using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game;

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
		RESUME,
		OPTIONS,
		CONTROLS,
		SAVE,
		EXIT
	}

	public GAMEMENU_STATE m_state;

	protected Texture2D m_fade;

	protected Texture2D m_bkg;

	protected Texture2D m_resume;

	protected Texture2D m_options;

	protected Texture2D m_controls;

	protected Texture2D m_controls_bkg;

	protected Texture2D m_save;

	protected Texture2D m_exit;

	protected Texture2D m_a_button;

	protected Texture2D m_b_button;

	protected SpriteFont m_font;

	protected SpriteFont m_font2;

	protected Game m_game;

	protected GAMEMENU_SELECTION m_selection;

	protected OptionsMenu m_options_menu;

	protected bool m_trial_message;

	protected IAsyncResult m_mb_res;

	private bool m_goto_store;

	public GameMenu(Game game)
	{
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_game = game;
		m_fade = m_game.m_fade_texture;
		m_bkg = m_game.m_CL.LoadTexture("Inventory/rost");
		m_resume = m_game.m_CL.LoadTexture("GameMenu/resume");
		m_options = m_game.m_CL.LoadTexture("StartMenu/options");
		m_controls = m_game.m_CL.LoadTexture("StartMenu/controls");
		m_controls_bkg = m_game.m_CL.LoadTexture("StartMenu/controllers");
		m_save = m_game.m_CL.LoadTexture("GameMenu/save");
		m_exit = m_game.m_CL.LoadTexture("StartMenu/exit");
		m_a_button = m_game.m_CL.LoadTexture("HUD/a_button");
		m_b_button = m_game.m_CL.LoadTexture("HUD/b_button");
		m_font = m_game.m_CL.LoadFont("Fonts/SpriteFont1");
		m_font2 = m_game.m_CL.LoadFont("Fonts/SpriteFont2");
		m_selection = GAMEMENU_SELECTION.RESUME;
		m_options_menu = new OptionsMenu(m_game, new Vector2(0f, -80f));
	}

	public virtual void Clear()
	{
		m_game = null;
		m_fade = null;
		m_options_menu = null;
		m_bkg = null;
		m_resume = null;
		m_options = null;
		m_controls = null;
		m_controls_bkg = null;
		m_save = null;
		m_exit = null;
		m_a_button = null;
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
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Invalid comparison between Unknown and I4
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Invalid comparison between Unknown and I4
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
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
				goto IL_009c;
			}
		}
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
		goto IL_009c;
		IL_0134:
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
			case GAMEMENU_SELECTION.RESUME:
				m_game.m_show_game_menu = false;
				if (m_game.m_inventory != null)
				{
					m_game.m_inventory.onGameMenuClosed();
				}
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
					Guide.BeginShowMessageBox(Game.PLAYER_INDEX, "Message", "This feature is only available in the full version.", (IEnumerable<string>)new string[2] { "Unlock full game", "Ok" }, 1, (MessageBoxIcon)0, (AsyncCallback)onMessageFinished, (object)object.Equals(0, 0));
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
		return;
		IL_009c:
		GamePadState state5 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad2 = ((GamePadState)(ref state5)).DPad;
		if ((int)((GamePadDPad)(ref dPad2)).Up != 1)
		{
			GamePadState state6 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref state6)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks2)).Left.Y > 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)38))
			{
				m_game.m_up_pressed = false;
				goto IL_0134;
			}
		}
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
		goto IL_0134;
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
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Invalid comparison between Unknown and I4
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Invalid comparison between Unknown and I4
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Invalid comparison between Unknown and I4
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Invalid comparison between Unknown and I4
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Invalid comparison between Unknown and I4
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
					Guide.BeginShowMessageBox(Game.PLAYER_INDEX, "Message", ex.Message, (IEnumerable<string>)new string[1] { "Ok" }, 0, (MessageBoxIcon)0, (AsyncCallback)onMessage2Finished, (object)object.Equals(0, 0));
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
		{
			GamePadState state3 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons2 = ((GamePadState)(ref state3)).Buttons;
			if ((int)((GamePadButtons)(ref buttons2)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)65))
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
			GamePadState state4 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons3 = ((GamePadState)(ref state4)).Buttons;
			if ((int)((GamePadButtons)(ref buttons3)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
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
		}
		case GAMEMENU_STATE.ASK_OVERWRITE:
		{
			GamePadState state5 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons4 = ((GamePadState)(ref state5)).Buttons;
			if ((int)((GamePadButtons)(ref buttons4)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)65))
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
			GamePadState state6 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons5 = ((GamePadState)(ref state6)).Buttons;
			if ((int)((GamePadButtons)(ref buttons5)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
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
		}
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
		{
			GamePadState state2 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons = ((GamePadState)(ref state2)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
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
		}
		case GAMEMENU_STATE.SAVE:
			break;
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0adb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0778: Unknown result type (might be due to invalid IL or missing references)
		//IL_077a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		//IL_0842: Unknown result type (might be due to invalid IL or missing references)
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0910: Unknown result type (might be due to invalid IL or missing references)
		//IL_0915: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Unknown result type (might be due to invalid IL or missing references)
		//IL_0979: Unknown result type (might be due to invalid IL or missing references)
		//IL_098c: Unknown result type (might be due to invalid IL or missing references)
		//IL_098e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
		if (m_trial_message || SB.GraphicsDevice.IsDisposed || (int)SB.GraphicsDevice.GraphicsDeviceStatus != 0 || SB == null)
		{
			return;
		}
		if (m_state == GAMEMENU_STATE.ASK_EXIT)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_bkg, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)64));
			SB.Draw(m_fade, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)192));
			string text = "All unsaved progress will be lost.";
			string text2 = "Do you really want to exit?";
			Vector2 val = m_font2.MeasureString(text);
			Vector2 val2 = m_font2.MeasureString(text2);
			float x = val.X;
			Vector2 zero = Vector2.Zero;
			zero.X = ((float)Game.VIEW_RECT.Width - val.X) / 2f;
			zero.Y = ((float)Game.VIEW_RECT.Height - val.Y) / 2f - 50f;
			SB.DrawString(m_font2, text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font2, text, zero, Color.White);
			zero.X = ((float)Game.VIEW_RECT.Width - val2.X) / 2f;
			zero.Y += val.Y;
			SB.DrawString(m_font2, text2, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font2, text2, zero, Color.White);
			text = "EXIT";
			text2 = "CANCEL";
			val = m_font.MeasureString(text);
			val2 = m_font.MeasureString(text2);
			x = (float)(m_a_button.Width + 10) + val.X + 40f + (float)m_b_button.Width + 10f + val2.X;
			zero.X = ((float)Game.VIEW_RECT.Width - x) / 2f;
			zero.Y += val.Y * 3f;
			SB.Draw(m_a_button, zero, Color.White);
			zero.X += (float)(m_a_button.Width + 10);
			SB.DrawString(m_font, text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, text, zero, Color.White);
			zero.X += val.X + 40f;
			SB.Draw(m_b_button, zero, Color.White);
			zero.X += (float)(m_b_button.Width + 10);
			SB.DrawString(m_font, text2, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, text2, zero, Color.White);
			SB.End();
			return;
		}
		if (m_state == GAMEMENU_STATE.ASK_OVERWRITE)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_bkg, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)64));
			SB.Draw(m_fade, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)192));
			string text3 = "Overwrite last save?";
			string text4 = "";
			Vector2 val3 = m_font2.MeasureString(text3);
			Vector2 zero2 = Vector2.Zero;
			float x2 = val3.X;
			Vector2 zero3 = Vector2.Zero;
			zero3.X = ((float)Game.VIEW_RECT.Width - val3.X) / 2f;
			zero3.Y = ((float)Game.VIEW_RECT.Height - val3.Y) / 2f - 50f;
			SB.DrawString(m_font2, text3, new Vector2(zero3.X + 1f, zero3.Y + 2f), Color.Black);
			SB.DrawString(m_font2, text3, zero3, Color.White);
			text3 = "YES";
			text4 = "CANCEL";
			val3 = m_font.MeasureString(text3);
			zero2 = m_font.MeasureString(text4);
			x2 = (float)(m_a_button.Width + 10) + val3.X + 40f + (float)m_b_button.Width + 10f + zero2.X;
			zero3.X = ((float)Game.VIEW_RECT.Width - x2) / 2f;
			zero3.Y += val3.Y * 3f;
			SB.Draw(m_a_button, zero3, Color.White);
			zero3.X += (float)(m_a_button.Width + 10);
			SB.DrawString(m_font, text3, new Vector2(zero3.X + 1f, zero3.Y + 2f), Color.Black);
			SB.DrawString(m_font, text3, zero3, Color.White);
			zero3.X += val3.X + 40f;
			SB.Draw(m_b_button, zero3, Color.White);
			zero3.X += (float)(m_b_button.Width + 10);
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
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_bkg, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)64));
			SB.Draw(m_fade, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)192));
			Vector2 zero5 = Vector2.Zero;
			zero5.X = (Game.VIEW_RECT.Width - m_resume.Width) / 2;
			zero5.Y = 300f;
			Color val5 = Color.White;
			Color val6 = default(Color);
			((Color)(ref val6))._002Ector(byte.MaxValue, (byte)30, (byte)30, byte.MaxValue);
			if (m_selection == GAMEMENU_SELECTION.RESUME)
			{
				SB.Draw(m_resume, zero5, val6);
			}
			else
			{
				SB.Draw(m_resume, zero5, val5);
			}
			zero5.X = (Game.VIEW_RECT.Width - m_options.Width) / 2;
			zero5.Y += (float)(m_resume.Height + 10);
			if (m_selection == GAMEMENU_SELECTION.OPTIONS)
			{
				SB.Draw(m_options, zero5, val6);
			}
			else
			{
				SB.Draw(m_options, zero5, val5);
			}
			zero5.X = (Game.VIEW_RECT.Width - m_controls.Width) / 2;
			zero5.Y += (float)m_options.Height;
			if (m_selection == GAMEMENU_SELECTION.CONTROLS)
			{
				SB.Draw(m_controls, zero5, val6);
			}
			else
			{
				SB.Draw(m_controls, zero5, val5);
			}
			zero5.X = (Game.VIEW_RECT.Width - m_save.Width) / 2;
			zero5.Y += (float)m_controls.Height;
			if (m_selection == GAMEMENU_SELECTION.SAVE)
			{
				SB.Draw(m_save, zero5, val6);
			}
			else
			{
				SB.Draw(m_save, zero5, val5);
			}
			zero5.X = (Game.VIEW_RECT.Width - m_exit.Width) / 2;
			zero5.Y += (float)m_save.Height;
			if (m_selection == GAMEMENU_SELECTION.EXIT)
			{
				SB.Draw(m_exit, zero5, val6);
			}
			else
			{
				SB.Draw(m_exit, zero5, val5);
			}
			SB.End();
			if (m_state == GAMEMENU_STATE.SAVE)
			{
				SB.Begin((SpriteBlendMode)1);
				val5 = Color.Black;
				((Color)(ref val5)).A = 128;
				SB.Draw(m_fade, Game.VIEW_RECT, val5);
				string text6 = "Saving, do not turn off your console.";
				Vector2 val7 = m_font2.MeasureString(text6);
				zero5.X = ((float)Game.VIEW_RECT.Width - val7.X) / 2f;
				zero5.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val7.Y;
				SB.DrawString(m_font2, text6, new Vector2(zero5.X + 1f, zero5.Y + 2f), Color.Black);
				SB.DrawString(m_font2, text6, zero5, Color.White);
				SB.End();
				((Game)m_game).GraphicsDevice.Present();
				m_game.SaveGameData();
				((Game)m_game).GraphicsDevice.Clear(Color.Black);
				m_state = GAMEMENU_STATE.MENU;
			}
			break;
		}
		case GAMEMENU_STATE.OPTIONS:
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_bkg, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)64));
			SB.Draw(m_fade, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)192));
			SB.End();
			if (m_options_menu != null)
			{
				m_options_menu.Draw(SB);
			}
			break;
		case GAMEMENU_STATE.CONTROLS:
			SB.GraphicsDevice.Clear(Color.Black);
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_controls_bkg, new Rectangle(Game.TS_AREA.X - 20, Game.TS_AREA.Y - 20, Game.TS_AREA.Width + 40, Game.TS_AREA.Height + 40), Color.White);
			SB.End();
			if (m_b_button != null)
			{
				SB.Begin((SpriteBlendMode)1);
				string text5 = "BACK";
				Vector2 val4 = m_font.MeasureString(text5);
				Vector2 zero4 = Vector2.Zero;
				zero4.X = (float)((Rectangle)(ref Game.TS_AREA)).Right - val4.X;
				zero4.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val4.Y;
				SB.DrawString(m_font, text5, new Vector2(zero4.X + 1f, zero4.Y + 2f), Color.Black);
				SB.DrawString(m_font, text5, zero4, Color.White);
				zero4.X -= (float)(m_b_button.Width + 10);
				SB.Draw(m_b_button, zero4, Color.White);
				SB.End();
			}
			break;
		}
	}
}
