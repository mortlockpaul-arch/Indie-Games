using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Core;

public class OptionsMenu
{
	protected enum OPTIONS_STATE
	{
		DEFAULT,
		SAVE_SETTINGS
	}

	protected enum OPTIONS_SELECTION
	{
		NONE,
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

	protected SpriteFont m_font;

	protected Game m_game;

	protected OPTIONS_STATE m_state;

	protected OPTIONS_SELECTION m_selection = OPTIONS_SELECTION.BRIGHTNESS;

	protected OPTIONS_ARROW_STATE m_arrow_state;

	protected bool m_save_settings;

	public OptionsMenu(Game game)
	{
		m_game = game;
		m_font = m_game.Content.Load<SpriteFont>("Fonts/SpriteFont2");
		m_selection = OPTIONS_SELECTION.BRIGHTNESS;
		SetGamma(m_game.m_game_settings.m_brightness);
		m_save_settings = false;
	}

	public virtual void Clear()
	{
		m_game = null;
		m_font = null;
	}

	public virtual void Update(TimeSpan elapsed)
	{
		KeyboardState state = Keyboard.GetState();
		switch (m_selection)
		{
		case OPTIONS_SELECTION.BRIGHTNESS:
			if (GamePad.GetState(Game.PLAYER_INDEX).DPad.Left == ButtonState.Pressed || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X < -0.2f || state.IsKeyDown(Keys.Left))
			{
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
			if (m_game.m_left_pressed)
			{
				m_arrow_state = OPTIONS_ARROW_STATE.NONE;
			}
			m_game.m_left_pressed = false;
			if (GamePad.GetState(Game.PLAYER_INDEX).DPad.Right == ButtonState.Pressed || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X > 0.2f || state.IsKeyDown(Keys.Right))
			{
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
			if (m_game.m_right_pressed)
			{
				m_arrow_state = OPTIONS_ARROW_STATE.NONE;
			}
			m_game.m_right_pressed = false;
			break;
		case OPTIONS_SELECTION.SOUND:
			if (GamePad.GetState(Game.PLAYER_INDEX).DPad.Left == ButtonState.Pressed || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X < -0.2f || state.IsKeyDown(Keys.Left))
			{
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
			if (m_game.m_left_pressed)
			{
				m_arrow_state = OPTIONS_ARROW_STATE.NONE;
			}
			m_game.m_left_pressed = false;
			if (GamePad.GetState(Game.PLAYER_INDEX).DPad.Right == ButtonState.Pressed || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X > 0.2f || state.IsKeyDown(Keys.Right))
			{
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
			if (m_game.m_right_pressed)
			{
				m_arrow_state = OPTIONS_ARROW_STATE.NONE;
			}
			m_game.m_right_pressed = false;
			break;
		case OPTIONS_SELECTION.DEFAULT:
			m_arrow_state = OPTIONS_ARROW_STATE.NONE;
			if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter))
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
		case OPTIONS_SELECTION.BACK:
			m_arrow_state = OPTIONS_ARROW_STATE.NONE;
			if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter))
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
		if (GamePad.GetState(Game.PLAYER_INDEX).DPad.Down == ButtonState.Pressed || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.Y < -0.2f || state.IsKeyDown(Keys.Down))
		{
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
		else
		{
			m_game.m_up_pressed = false;
		}
	}

	public void SetGamma(float gamma)
	{
		m_save_settings = true;
	}

	protected void SetSound(float sound)
	{
		try
		{
			m_save_settings = true;
			MediaPlayer.Volume = sound * 0.1f * Sound.MUSIC_VOL_DEC_MULTI;
			SoundEffect.MasterVolume = sound * 0.1f;
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
