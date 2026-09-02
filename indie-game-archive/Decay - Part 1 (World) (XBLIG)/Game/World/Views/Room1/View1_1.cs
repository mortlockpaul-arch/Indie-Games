using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game.World.Views.Room1;

internal class View1_1 : View
{
	private enum TEXT_STATE
	{
		WAIT,
		FADE_IN,
		SHOW,
		FADE_OUT
	}

	private SoundEffect m_sound_dive;

	private float m_dive_sound_timeout = 2f;

	private Animation2D m_water_anim;

	private bool m_play_water_anim;

	private bool m_fade_to_black;

	private float m_fade_alpha;

	private Texture2D m_fade;

	private float m_fade_timer;

	private float m_fade_timeout = 1f;

	private float m_text_alpha;

	private int m_text_index;

	private float m_timer;

	private TEXT_STATE m_text_state;

	private SpriteFont m_font;

	private SpriteFont m_font2;

	private bool m_save;

	private bool m_goto_menu;

	public View1_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "badkar")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "badkar_water")));
		m_fade = m_room.m_CL.LoadTexture("HUD/black");
		m_scenes.Add(new Scene(m_fade));
		m_water_anim = (Animation2D)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/Dive/");
		m_sound_dive = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/bathroom_dive");
		m_font = m_room.m_CL.LoadFont("Fonts/SpriteFont4");
		m_font2 = m_room.m_CL.LoadFont("Fonts/SpriteFont2");
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
		m_use_text_fade = true;
	}

	public override void Clear()
	{
		m_font = null;
		m_font2 = null;
		m_sound_dive = null;
		if (m_water_anim != null)
		{
			m_water_anim.Clear();
			m_water_anim = null;
		}
		if (m_fade != null)
		{
			((GraphicsResource)m_fade).Dispose();
			m_fade = null;
		}
		base.Clear();
	}

	public override void Setup()
	{
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_blandare", trigger, Trigger.TRIGGER_TYPE.ZOOM_SMALL);
		m_triggers.Add(item);
		item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_water", "View1_1.onWater", Trigger.TRIGGER_TYPE.USE);
		item.m_activate_own = true;
		item.m_enabled = false;
		m_triggers.Add(item);
		if (m_game.m_game_data.GetState("Room1.GateState") == "4")
		{
			HandleBlackWater();
		}
	}

	public override void Reset()
	{
		base.Reset();
		m_timer = 3f;
	}

	private void HandleBlackWater()
	{
		ChangeScene(1);
		m_triggers[0].m_enabled = false;
		m_triggers[1].m_enabled = true;
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "View1_1_1.UsePolygrip":
			HandleBlackWater();
			break;
		case "View1_1.onWater":
			m_game.m_hud.ShowAsk("The water is all black. I can't feel the bottom ...", "DIVE", "CANCEL", "View1_1.onDive", "", m_use_text_fade);
			break;
		case "View1_1.onDive":
			m_play_water_anim = true;
			m_water_anim.Play();
			m_game.m_game_menu_enabled = false;
			m_game.m_hud.m_state = HUD.HUD_STATE.NONE;
			m_game.m_input_enabled = false;
			m_game.m_show_cursor = false;
			m_game.m_inventory_enabled = false;
			m_fade_to_black = true;
			m_game.m_game_data.SetState("Music", "1");
			m_game.PlayMusic(m_game.m_music1);
			m_game.FadeInMusic();
			m_room.HandleEvent("Room1.StopAmbient");
			m_game.m_game_settings.m_extras_unlocked = true;
			break;
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Invalid comparison between Unknown and I4
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Invalid comparison between Unknown and I4
		base.Update(elapsed);
		KeyboardState state = Keyboard.GetState();
		if (m_play_water_anim && m_water_anim != null)
		{
			m_water_anim.Update(elapsed);
			if (m_water_anim.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
			{
				ChangeScene(2);
				m_play_water_anim = false;
			}
		}
		if (m_fade_to_black)
		{
			if (m_fade_alpha < 255f)
			{
				m_fade_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 80f;
				if (m_fade_alpha >= 255f)
				{
					m_fade_alpha = 255f;
					m_fade_timer = m_fade_timeout;
				}
			}
			if (m_dive_sound_timeout > 0f)
			{
				m_dive_sound_timeout -= (float)elapsed.TotalMilliseconds * 0.001f;
				if (m_dive_sound_timeout <= 0f)
				{
					m_dive_sound_timeout = 0f;
					m_game.PlaySound(m_sound_dive, 0.2f);
				}
			}
		}
		if (m_fade_timer > 0f)
		{
			m_fade_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_fade_timer <= 0f)
			{
				m_fade_timer = 0f;
				m_text_state = TEXT_STATE.WAIT;
				m_text_index = 0;
				m_timer = 1f;
			}
		}
		if (m_current_scene != 2)
		{
			return;
		}
		switch (m_text_state)
		{
		case TEXT_STATE.WAIT:
			m_timer -= (float)elapsed.TotalSeconds;
			if (m_timer <= 0f)
			{
				m_text_state = TEXT_STATE.FADE_IN;
			}
			break;
		case TEXT_STATE.FADE_IN:
			m_text_alpha += (float)elapsed.TotalSeconds * 255f * 0.5f;
			if (m_text_alpha >= 255f)
			{
				m_text_alpha = 255f;
				m_text_state = TEXT_STATE.SHOW;
				m_timer = 5f;
			}
			break;
		case TEXT_STATE.SHOW:
			if (m_text_index < 7)
			{
				m_timer -= (float)elapsed.TotalSeconds;
				if (m_timer <= 0f)
				{
					m_text_state = TEXT_STATE.FADE_OUT;
				}
			}
			break;
		case TEXT_STATE.FADE_OUT:
			m_text_alpha -= (float)elapsed.TotalSeconds * 255f * 0.5f;
			if (m_text_alpha <= 0f)
			{
				m_text_state = TEXT_STATE.FADE_IN;
				m_text_index++;
			}
			break;
		}
		GamePadState state2 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadButtons buttons = ((GamePadState)(ref state2)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).A != 1)
		{
			GamePadState state3 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons2 = ((GamePadState)(ref state3)).Buttons;
			if ((int)((GamePadButtons)(ref buttons2)).Start != 1 && !((KeyboardState)(ref state)).IsKeyDown((Keys)65))
			{
				m_game.m_a_pressed = false;
				goto IL_02e9;
			}
		}
		if (!m_game.m_a_pressed)
		{
			m_game.m_a_pressed = true;
			SaveAndExit();
		}
		goto IL_02e9;
		IL_02e9:
		if (m_goto_menu)
		{
			m_goto_menu = false;
			m_game.onExitGame();
		}
	}

	private void SaveAndExit()
	{
		m_save = true;
	}

	public override void Draw(SpriteBatch SB)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		base.Draw(SB);
		if (m_play_water_anim)
		{
			m_water_anim.Draw(SB);
		}
		if (m_fade_to_black)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_fade, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Math.Round(m_fade_alpha)));
			SB.End();
		}
		if (m_current_scene == 2)
		{
			string text = "";
			string text2 = "";
			switch (m_text_index)
			{
			case 0:
				text = "Developed By";
				text2 = "Shining Gate Software";
				break;
			case 1:
				text = "Programming & Design";
				text2 = "Fredrik Westlund";
				break;
			case 2:
				text = "Art, Music & Design";
				text2 = "Johannes Rae";
				break;
			case 3:
				text = "Lead QA";
				text2 = "Fredrik Westlund";
				break;
			case 4:
				text = "Additional QA";
				text2 = "Robert Ottone";
				break;
			case 5:
				text = "Additional Testing";
				text2 = "Anna-Maria Taawo";
				break;
			case 6:
				text = "Special Thanks";
				text2 = "Jenny Taawo";
				break;
			case 7:
				text = "To be continued ...";
				break;
			}
			if (m_text_index == 7)
			{
				Vector2 val = m_game.m_hud.m_font2.MeasureString(text);
				Vector2 pos = default(Vector2);
				((Vector2)(ref pos))._002Ector(((float)Game.VIEW_RECT.Width - val.X) / 2f, ((float)Game.VIEW_RECT.Height - val.Y) / 2f);
				m_game.m_hud.DrawText2(SB, text, pos, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Math.Round(m_text_alpha)));
			}
			else
			{
				Vector2 val2 = m_game.m_hud.m_font2.MeasureString(text);
				Vector2 pos2 = default(Vector2);
				((Vector2)(ref pos2))._002Ector(((float)Game.VIEW_RECT.Width - val2.X) / 2f, (float)(Game.VIEW_RECT.Height / 2) - val2.Y);
				m_game.m_hud.DrawText2(SB, text, pos2, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Math.Round(m_text_alpha)));
				val2 = m_game.m_hud.m_font2.MeasureString(text2);
				((Vector2)(ref pos2))._002Ector(((float)Game.VIEW_RECT.Width - val2.X) / 2f, (float)(Game.VIEW_RECT.Height / 2));
				m_game.m_hud.DrawText2(SB, text2, pos2, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Math.Round(m_text_alpha)));
			}
			if (m_save)
			{
				text = "Saving, do not turn off your console.";
				Vector2 val3 = m_font2.MeasureString(text);
				Vector2 zero = Vector2.Zero;
				zero.X = ((float)Game.VIEW_RECT.Width - val3.X) / 2f;
				zero.Y = (float)Game.VIEW_RECT.Height - val3.Y * 2f;
				SB.Begin((SpriteBlendMode)1);
				SB.DrawString(m_font2, text, zero, Color.White);
				SB.End();
				((Game)m_game).GraphicsDevice.Present();
				m_game.SaveSettings();
				((Game)m_game).GraphicsDevice.Clear(Color.Black);
				m_save = false;
				m_goto_menu = true;
			}
		}
	}
}
