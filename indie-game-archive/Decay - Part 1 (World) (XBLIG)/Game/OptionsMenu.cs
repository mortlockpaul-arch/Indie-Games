using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Game;

public class OptionsMenu
{
	protected enum OPTIONS_STATE
	{
		DEFAULT,
		SAVE_SETTINGS
	}

	protected enum OPTIONS_SELECTION
	{
		BRIGHTNESS,
		SOUND,
		DEFAULT,
		BACK
	}

	protected enum OPTIONS_ARROW_STATE
	{
		NONE,
		BRIGHTNESS_INCREASE,
		BRIGHTNESS_DECREASE,
		SOUND_INCREASE,
		SOUND_DECREASE
	}

	protected Texture2D m_fade;

	protected Texture2D m_bkg;

	protected Texture2D m_continue;

	protected Texture2D m_new_game;

	protected Texture2D m_options;

	protected Texture2D m_exit;

	protected Texture2D m_a_button;

	protected Texture2D m_b_button;

	protected Texture2D m_options_bkg;

	protected Texture2D m_brightness;

	protected Texture2D m_sound;

	protected Texture2D m_default;

	protected Texture2D m_back;

	protected Texture2D m_arrow;

	protected SpriteFont m_font;

	protected Game m_game;

	protected OPTIONS_STATE m_state;

	protected OPTIONS_SELECTION m_selection;

	protected OPTIONS_ARROW_STATE m_arrow_state;

	protected bool m_save_settings;

	protected Vector2 m_pos;

	public OptionsMenu(Game game, Vector2 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		m_pos = Vector2.Zero;
		base._002Ector();
		m_game = game;
		m_pos = pos;
		m_fade = ((Game)m_game).Content.Load<Texture2D>("HUD/black");
		m_brightness = ((Game)m_game).Content.Load<Texture2D>("OptionsMenu/brightness");
		m_sound = ((Game)m_game).Content.Load<Texture2D>("OptionsMenu/sound");
		m_default = ((Game)m_game).Content.Load<Texture2D>("OptionsMenu/default");
		m_back = ((Game)m_game).Content.Load<Texture2D>("OptionsMenu/back");
		m_arrow = ((Game)m_game).Content.Load<Texture2D>("OptionsMenu/arrow_white");
		m_font = ((Game)m_game).Content.Load<SpriteFont>("Fonts/SpriteFont2");
		m_selection = OPTIONS_SELECTION.BRIGHTNESS;
		SetGamma(m_game.m_game_settings.m_brightness);
		m_save_settings = false;
	}

	public virtual void Clear()
	{
		m_game = null;
		if (m_fade != null)
		{
			((GraphicsResource)m_fade).Dispose();
			m_fade = null;
		}
		((GraphicsResource)m_brightness).Dispose();
		m_brightness = null;
		((GraphicsResource)m_sound).Dispose();
		m_sound = null;
		((GraphicsResource)m_default).Dispose();
		m_default = null;
		((GraphicsResource)m_back).Dispose();
		m_back = null;
		((GraphicsResource)m_arrow).Dispose();
		m_arrow = null;
		m_font = null;
	}

	public virtual void Update(TimeSpan elapsed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Invalid comparison between Unknown and I4
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Invalid comparison between Unknown and I4
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Invalid comparison between Unknown and I4
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Invalid comparison between Unknown and I4
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Invalid comparison between Unknown and I4
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Invalid comparison between Unknown and I4
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Invalid comparison between Unknown and I4
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		KeyboardState state = Keyboard.GetState();
		switch (m_selection)
		{
		case OPTIONS_SELECTION.BRIGHTNESS:
		{
			GamePadState state4 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadDPad dPad = ((GamePadState)(ref state4)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Left != 1)
			{
				GamePadState state5 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadThumbSticks thumbSticks = ((GamePadState)(ref state5)).ThumbSticks;
				if (!(((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)37))
				{
					if (m_game.m_left_pressed)
					{
						m_arrow_state = OPTIONS_ARROW_STATE.NONE;
					}
					m_game.m_left_pressed = false;
					GamePadState state6 = GamePad.GetState(Game.PLAYER_INDEX);
					GamePadDPad dPad2 = ((GamePadState)(ref state6)).DPad;
					if ((int)((GamePadDPad)(ref dPad2)).Right != 1)
					{
						GamePadState state7 = GamePad.GetState(Game.PLAYER_INDEX);
						GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref state7)).ThumbSticks;
						if (!(((GamePadThumbSticks)(ref thumbSticks2)).Left.X > 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)39))
						{
							if (m_game.m_right_pressed)
							{
								m_arrow_state = OPTIONS_ARROW_STATE.NONE;
							}
							m_game.m_right_pressed = false;
							break;
						}
					}
					if (!m_game.m_right_pressed)
					{
						m_game.m_right_pressed = true;
						m_arrow_state = OPTIONS_ARROW_STATE.BRIGHTNESS_INCREASE;
						m_game.m_game_settings.m_brightness++;
						if (m_game.m_game_settings.m_brightness > 10f)
						{
							m_game.m_game_settings.m_brightness = 10f;
						}
						SetGamma(m_game.m_game_settings.m_brightness);
					}
					return;
				}
			}
			if (!m_game.m_left_pressed)
			{
				m_game.m_left_pressed = true;
				m_arrow_state = OPTIONS_ARROW_STATE.BRIGHTNESS_DECREASE;
				m_game.m_game_settings.m_brightness--;
				if (m_game.m_game_settings.m_brightness < 0f)
				{
					m_game.m_game_settings.m_brightness = 0f;
				}
				SetGamma(m_game.m_game_settings.m_brightness);
			}
			return;
		}
		case OPTIONS_SELECTION.SOUND:
		{
			GamePadState state8 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadDPad dPad3 = ((GamePadState)(ref state8)).DPad;
			if ((int)((GamePadDPad)(ref dPad3)).Left != 1)
			{
				GamePadState state9 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref state9)).ThumbSticks;
				if (!(((GamePadThumbSticks)(ref thumbSticks3)).Left.X < -0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)37))
				{
					if (m_game.m_left_pressed)
					{
						m_arrow_state = OPTIONS_ARROW_STATE.NONE;
					}
					m_game.m_left_pressed = false;
					GamePadState state10 = GamePad.GetState(Game.PLAYER_INDEX);
					GamePadDPad dPad4 = ((GamePadState)(ref state10)).DPad;
					if ((int)((GamePadDPad)(ref dPad4)).Right != 1)
					{
						GamePadState state11 = GamePad.GetState(Game.PLAYER_INDEX);
						GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref state11)).ThumbSticks;
						if (!(((GamePadThumbSticks)(ref thumbSticks4)).Left.X > 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)39))
						{
							if (m_game.m_right_pressed)
							{
								m_arrow_state = OPTIONS_ARROW_STATE.NONE;
							}
							m_game.m_right_pressed = false;
							break;
						}
					}
					if (!m_game.m_right_pressed)
					{
						m_game.m_right_pressed = true;
						m_arrow_state = OPTIONS_ARROW_STATE.SOUND_INCREASE;
						m_game.m_game_settings.m_sound_volume++;
						if (m_game.m_game_settings.m_sound_volume > 10f)
						{
							m_game.m_game_settings.m_sound_volume = 10f;
						}
						SetSound(m_game.m_game_settings.m_sound_volume);
					}
					return;
				}
			}
			if (!m_game.m_left_pressed)
			{
				m_game.m_left_pressed = true;
				m_arrow_state = OPTIONS_ARROW_STATE.SOUND_DECREASE;
				m_game.m_game_settings.m_sound_volume--;
				if (m_game.m_game_settings.m_sound_volume < 0f)
				{
					m_game.m_game_settings.m_sound_volume = 0f;
				}
				SetSound(m_game.m_game_settings.m_sound_volume);
			}
			return;
		}
		case OPTIONS_SELECTION.DEFAULT:
		{
			m_arrow_state = OPTIONS_ARROW_STATE.NONE;
			GamePadState state3 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons2 = ((GamePadState)(ref state3)).Buttons;
			if ((int)((GamePadButtons)(ref buttons2)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)13))
			{
				if (!m_game.m_a_pressed)
				{
					m_game.m_a_pressed = true;
					bool extras_unlocked = false;
					if (m_game.m_game_settings != null)
					{
						extras_unlocked = m_game.m_game_settings.m_extras_unlocked;
						m_game.m_game_settings.Clear();
						m_game.m_game_settings = null;
					}
					m_game.m_game_settings = new GameSettings();
					m_game.m_game_settings.m_extras_unlocked = extras_unlocked;
					SetGamma(m_game.m_game_settings.m_brightness);
					SetSound(m_game.m_game_settings.m_sound_volume);
				}
			}
			else
			{
				m_game.m_a_pressed = false;
			}
			break;
		}
		case OPTIONS_SELECTION.BACK:
		{
			m_arrow_state = OPTIONS_ARROW_STATE.NONE;
			GamePadState state2 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons = ((GamePadState)(ref state2)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)13))
			{
				if (!m_game.m_a_pressed)
				{
					m_game.m_a_pressed = true;
					if (Guide.IsTrialMode)
					{
						m_save_settings = false;
					}
					if (m_save_settings)
					{
						m_save_settings = false;
						m_state = OPTIONS_STATE.SAVE_SETTINGS;
					}
					else
					{
						m_game.onOptionsClosed();
					}
				}
			}
			else
			{
				m_game.m_a_pressed = false;
			}
			break;
		}
		}
		GamePadState state12 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad5 = ((GamePadState)(ref state12)).DPad;
		if ((int)((GamePadDPad)(ref dPad5)).Down != 1)
		{
			GamePadState state13 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks5 = ((GamePadState)(ref state13)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks5)).Left.Y < -0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)40))
			{
				m_game.m_down_pressed = false;
				goto IL_0606;
			}
		}
		if (!m_game.m_down_pressed)
		{
			m_game.m_down_pressed = true;
			if (m_selection == OPTIONS_SELECTION.BACK)
			{
				m_selection = OPTIONS_SELECTION.BRIGHTNESS;
			}
			else
			{
				m_selection++;
			}
		}
		goto IL_0606;
		IL_0606:
		GamePadState state14 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad6 = ((GamePadState)(ref state14)).DPad;
		if ((int)((GamePadDPad)(ref dPad6)).Up != 1)
		{
			GamePadState state15 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks6 = ((GamePadState)(ref state15)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks6)).Left.Y > 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)38))
			{
				m_game.m_up_pressed = false;
				return;
			}
		}
		if (!m_game.m_up_pressed)
		{
			m_game.m_up_pressed = true;
			if (m_selection == OPTIONS_SELECTION.BRIGHTNESS)
			{
				m_selection = OPTIONS_SELECTION.BACK;
			}
			else
			{
				m_selection--;
			}
		}
	}

	public void SetGamma(float gamma)
	{
		m_save_settings = true;
		GammaRamp gammaRamp = ((Game)m_game).GraphicsDevice.GetGammaRamp();
		gamma -= 5f;
		gamma /= 7.5f;
		gamma = MathHelper.Clamp(gamma, -1f, 1f);
		short[] array = new short[256];
		short[] array2 = new short[256];
		short[] array3 = new short[256];
		for (int num = 255; num >= 0; num--)
		{
			array[num] = (array2[num] = (array3[num] = (short)((int)Math.Min(255f, (float)num * (gamma + 1f)) << 8)));
		}
		gammaRamp.SetRed(array);
		gammaRamp.SetGreen(array2);
		gammaRamp.SetBlue(array3);
		((Game)m_game).GraphicsDevice.SetGammaRamp(true, gammaRamp);
	}

	protected void SetSound(float sound)
	{
		m_save_settings = true;
		MediaPlayer.Volume = sound * 0.1f * Game.MUSIC_VOL_DEC_MULTI;
		m_game.m_volume_changed = true;
	}

	public virtual void Draw(SpriteBatch SB)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		if (SB != null)
		{
			SB.Begin((SpriteBlendMode)1);
			Vector2 zero = Vector2.Zero;
			Color val = Color.White;
			Color val2 = default(Color);
			((Color)(ref val2))._002Ector(byte.MaxValue, (byte)30, (byte)30, byte.MaxValue);
			Color val3 = default(Color);
			((Color)(ref val3))._002Ector((byte)30, byte.MaxValue, (byte)30, byte.MaxValue);
			float num = m_brightness.Width + 80 + m_arrow.Width + 80 + m_arrow.Width;
			zero.X = ((float)Game.VIEW_RECT.Width - num) / 2f;
			zero.Y = 360f;
			zero.X += m_pos.X;
			zero.Y += m_pos.Y;
			Color val4 = val;
			if (m_selection == OPTIONS_SELECTION.BRIGHTNESS)
			{
				val4 = val2;
			}
			SB.Draw(m_brightness, zero, val4);
			zero.X += (float)(m_brightness.Width + 80 + m_arrow.Width);
			if (m_arrow_state == OPTIONS_ARROW_STATE.BRIGHTNESS_DECREASE)
			{
				SB.Draw(m_arrow, zero, (Rectangle?)null, val3, 0f, Vector2.Zero, 1f, (SpriteEffects)1, 0f);
			}
			else
			{
				SB.Draw(m_arrow, zero, (Rectangle?)null, val4, 0f, Vector2.Zero, 1f, (SpriteEffects)1, 0f);
			}
			zero.X += (float)m_arrow.Width;
			float x = zero.X;
			string text = m_game.m_game_settings.m_brightness.ToString();
			zero.X += 40f - m_font.MeasureString(text).X / 2f;
			SB.DrawString(m_font, text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, text, zero, Color.White);
			zero.X = x + 80f;
			if (m_arrow_state == OPTIONS_ARROW_STATE.BRIGHTNESS_INCREASE)
			{
				SB.Draw(m_arrow, zero, val3);
			}
			else
			{
				SB.Draw(m_arrow, zero, val4);
			}
			zero.X = ((float)Game.VIEW_RECT.Width - num) / 2f;
			zero.X += m_pos.X;
			zero.Y += (float)m_brightness.Height;
			val4 = val;
			if (m_selection == OPTIONS_SELECTION.SOUND)
			{
				val4 = val2;
			}
			SB.Draw(m_sound, zero, val4);
			zero.X += (float)(m_brightness.Width + 80 + m_arrow.Width);
			if (m_arrow_state == OPTIONS_ARROW_STATE.SOUND_DECREASE)
			{
				SB.Draw(m_arrow, zero, (Rectangle?)null, val3, 0f, Vector2.Zero, 1f, (SpriteEffects)1, 0f);
			}
			else
			{
				SB.Draw(m_arrow, zero, (Rectangle?)null, val4, 0f, Vector2.Zero, 1f, (SpriteEffects)1, 0f);
			}
			zero.X += (float)m_arrow.Width;
			x = zero.X;
			text = m_game.m_game_settings.m_sound_volume.ToString();
			zero.X += 40f - m_font.MeasureString(text).X / 2f;
			SB.DrawString(m_font, text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, text, zero, Color.White);
			zero.X = x + 80f;
			if (m_arrow_state == OPTIONS_ARROW_STATE.SOUND_INCREASE)
			{
				SB.Draw(m_arrow, zero, val3);
			}
			else
			{
				SB.Draw(m_arrow, zero, val4);
			}
			zero.X = (Game.VIEW_RECT.Width - m_default.Width) / 2;
			zero.X += m_pos.X;
			zero.Y += (float)m_sound.Height * 2f;
			val4 = val;
			if (m_selection == OPTIONS_SELECTION.DEFAULT)
			{
				val4 = val2;
			}
			SB.Draw(m_default, zero, (Rectangle?)null, val4);
			zero.X = (Game.VIEW_RECT.Width - m_back.Width) / 2;
			zero.X += m_pos.X;
			zero.Y += (float)m_default.Height * 1.5f;
			val4 = val;
			if (m_selection == OPTIONS_SELECTION.BACK)
			{
				val4 = val2;
			}
			SB.Draw(m_back, zero, val4);
			SB.End();
			if (m_state == OPTIONS_STATE.SAVE_SETTINGS)
			{
				SB.Begin((SpriteBlendMode)1);
				val = Color.Black;
				((Color)(ref val)).A = 128;
				SB.Draw(m_fade, Game.VIEW_RECT, val);
				string text2 = "Saving, do not turn off your console.";
				Vector2 val5 = m_font.MeasureString(text2);
				num = val5.X;
				zero.X = ((float)Game.VIEW_RECT.Width - val5.X) / 2f;
				zero.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val5.Y;
				SB.DrawString(m_font, text2, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
				SB.DrawString(m_font, text2, zero, Color.White);
				SB.End();
				((Game)m_game).GraphicsDevice.Present();
				m_game.SaveSettings();
				((Game)m_game).GraphicsDevice.Clear(Color.Black);
				m_state = OPTIONS_STATE.DEFAULT;
				m_game.onOptionsClosed();
			}
		}
	}
}
