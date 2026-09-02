using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game.World.Views.Room2;

internal class View1_3 : View
{
	private SoundEffect m_tv_sound;

	private SoundEffectInstance m_tv_sound_inst;

	private Texture2D m_remote;

	private Texture2D m_fade;

	private Animation2D m_tv_animation1;

	private Animation2D m_tv_animation2;

	private Animation2D m_tv_animation;

	private SpriteFont m_font;

	private bool m_tv_puzzle;

	private Vector2 m_remote_pos;

	private string m_channel_text;

	private float m_channel_timeout;

	private float m_channel_timer;

	private bool m_update_channel;

	private float m_hide_channel_timeout;

	private float m_hide_channel_timer;

	private bool m_hide_channel;

	private int m_channel;

	private float m_change_channel_timer;

	public View1_3(Game game, Area room)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		m_remote_pos = Vector2.Zero;
		m_channel_text = "";
		m_channel_timeout = 3000f;
		m_hide_channel_timeout = 2000f;
		base._002Ector(game, room);
		m_name = "View1_3";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "tv")));
		m_tv_animation1 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/TV1/");
		m_tv_animation1.SetFPS(15.0);
		m_tv_animation1.m_random_mode = true;
		m_tv_animation2 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/TV2/");
		m_tv_animation2.SetFPS(15.0);
		m_tv_animation2.m_random_mode = true;
		m_tv_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "/Sound/bedroom_tvsound");
		m_tv_sound_inst = m_tv_sound.CreateInstance();
		m_tv_sound_inst.IsLooped = true;
		m_tv_sound_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * 0.1f;
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
		m_remote = m_room.m_CL.LoadTexture(m_room.m_content_path + "remote_pushbuttons");
		m_remote_pos = new Vector2(100f, (float)((Game.VIEW_RECT.Height - m_remote.Height) / 2));
		m_fade = m_room.m_CL.LoadTexture("HUD/black");
		m_font = m_room.m_CL.LoadFont("Fonts/SpriteFont3");
	}

	public override void Clear()
	{
		if (m_tv_sound_inst != null)
		{
			m_tv_sound_inst.Stop();
			m_tv_sound_inst.Dispose();
			m_tv_sound_inst = null;
		}
		m_tv_sound = null;
		m_font = null;
		if (m_remote != null)
		{
			((GraphicsResource)m_remote).Dispose();
			m_remote = null;
		}
		if (m_fade != null)
		{
			((GraphicsResource)m_fade).Dispose();
			m_fade = null;
		}
		m_tv_animation = null;
		if (m_tv_animation1 != null)
		{
			m_tv_animation1.Clear();
			m_tv_animation1 = null;
		}
		if (m_tv_animation2 != null)
		{
			m_tv_animation2.Clear();
			m_tv_animation2 = null;
		}
		base.Clear();
	}

	public override void Setup()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK);
		CursorTrigger cursorTrigger = null;
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(59, 75, 82, 109), "UseRemote011", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(88, 75, 111, 109), "UseRemote012", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(117, 75, 141, 109), "UseRemote013", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(59, 113, 82, 146), "UseRemote014", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(88, 113, 111, 146), "UseRemote015", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(117, 113, 141, 146), "UseRemote016", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(59, 150, 82, 183), "UseRemote017", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(88, 150, 111, 183), "UseRemote018", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(117, 150, 141, 183), "UseRemote019", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(88, 186, 111, 220), "UseRemote010", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(198, 60, 396, 202), "View1_3.onTV", Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(416, 152, 434, 203), "View1_3.onBottle", Trigger.TRIGGER_TYPE.ZOOM_SMALL);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
	}

	public override bool HandleUseEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "Remote01")
		{
			m_tv_puzzle = true;
			m_tv_animation = m_tv_animation1;
			m_tv_animation.Play(Animation2D.LOOP_TYPE.CYCLE);
			m_game.m_cursor.m_pos.X = 144f;
			m_game.m_cursor.m_pos.Y = 186f;
			m_game.m_cursor.m_state = Cursor.CURSOR_STATE.IDLE;
			m_game.m_update_cursor = false;
			m_game.m_input_enabled = false;
			m_game.m_inventory_enabled = false;
			m_game.m_hud.m_state = HUD.HUD_STATE.CANCEL;
			for (int i = 0; i < m_triggers.Count; i++)
			{
				if (m_triggers[i] != null)
				{
					m_triggers[i].m_enabled = true;
				}
			}
			m_tv_sound_inst.Play();
			return true;
		}
		return base.HandleUseEvent(s_event);
	}

	private void onRemoteButton(int button)
	{
		m_update_channel = true;
		m_hide_channel = false;
		if (m_channel_text.IndexOf("-") == 0)
		{
			m_channel_text = m_channel.ToString() + button;
			m_channel = int.Parse(m_channel_text);
			m_channel_timer = 0f;
		}
		else
		{
			m_channel_text = "-" + button;
			m_channel = button;
			m_channel_timer = m_channel_timeout;
		}
	}

	private void onChannel44()
	{
		m_tv_animation = m_tv_animation2;
		m_tv_animation.Play(Animation2D.LOOP_TYPE.CYCLE);
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "UseRemote011":
			onRemoteButton(1);
			break;
		case "UseRemote012":
			onRemoteButton(2);
			break;
		case "UseRemote013":
			onRemoteButton(3);
			break;
		case "UseRemote014":
			onRemoteButton(4);
			break;
		case "UseRemote015":
			onRemoteButton(5);
			break;
		case "UseRemote016":
			onRemoteButton(6);
			break;
		case "UseRemote017":
			onRemoteButton(7);
			break;
		case "UseRemote018":
			onRemoteButton(8);
			break;
		case "UseRemote019":
			onRemoteButton(9);
			break;
		case "UseRemote010":
			onRemoteButton(0);
			break;
		case "View1_3.onTV":
			m_game.m_hud.ShowText("It's an old TV.", m_use_text_fade);
			break;
		case "View1_3.onBottle":
			m_game.m_hud.ShowText("The bottle is empty.", m_use_text_fade);
			break;
		case "VolumeChanged":
			if (m_tv_sound_inst != null)
			{
				m_tv_sound_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * 0.1f;
			}
			break;
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Invalid comparison between Unknown and I4
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Invalid comparison between Unknown and I4
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Invalid comparison between Unknown and I4
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Invalid comparison between Unknown and I4
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Invalid comparison between Unknown and I4
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Invalid comparison between Unknown and I4
		base.Update(elapsed);
		if (!m_tv_puzzle)
		{
			return;
		}
		if (m_update_channel)
		{
			m_channel_timer -= (float)elapsed.TotalMilliseconds;
			if (m_channel_timer <= 0f)
			{
				m_update_channel = false;
				m_hide_channel = true;
				m_hide_channel_timer = m_hide_channel_timeout;
				if (m_channel < 10)
				{
					m_channel_text = " " + m_channel;
				}
				m_change_channel_timer = 200f;
				m_tv_sound_inst.Pause();
				if (m_channel == 44)
				{
					onChannel44();
				}
				else
				{
					m_tv_animation = m_tv_animation1;
					m_tv_animation.Play(Animation2D.LOOP_TYPE.CYCLE);
				}
			}
		}
		if (m_hide_channel)
		{
			m_hide_channel_timer -= (float)elapsed.TotalMilliseconds;
			if (m_hide_channel_timer <= 0f)
			{
				m_hide_channel = false;
				m_channel_text = "";
			}
		}
		if (m_change_channel_timer > 0f)
		{
			m_change_channel_timer -= (float)elapsed.TotalMilliseconds;
			if (m_change_channel_timer < 0f)
			{
				m_change_channel_timer = 0f;
				if (m_channel != 44)
				{
					m_tv_sound_inst.Play();
				}
			}
		}
		KeyboardState state = Keyboard.GetState();
		m_tv_animation.Update(elapsed);
		GamePadState state2 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadButtons buttons = ((GamePadState)(ref state2)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
		{
			m_tv_puzzle = false;
			m_game.m_update_cursor = true;
			m_game.m_input_enabled = true;
			m_game.m_inventory_enabled = true;
			m_game.m_hud.m_state = HUD.HUD_STATE.NONE;
			for (int i = 0; i < 10; i++)
			{
				if (m_triggers[i] != null)
				{
					m_triggers[i].m_enabled = false;
				}
			}
			m_tv_sound_inst.Stop();
		}
		GamePadState state3 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad = ((GamePadState)(ref state3)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Right != 1)
		{
			GamePadState state4 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref state4)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks)).Left.X >= 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)102))
			{
				m_game.m_d_right_pressed = false;
				goto IL_030b;
			}
		}
		if (!m_game.m_d_right_pressed)
		{
			m_game.m_d_left_pressed = true;
			m_game.m_d_right_pressed = true;
			m_game.m_d_up_pressed = true;
			m_game.m_d_down_pressed = true;
			if (m_game.m_over_trigger != m_triggers[9])
			{
				ref Vector2 pos = ref m_game.m_cursor.m_pos;
				pos.X += 59f;
				if (m_game.m_cursor.m_pos.X > 262f)
				{
					m_game.m_cursor.m_pos.X = 144f;
				}
			}
			return;
		}
		goto IL_030b;
		IL_0531:
		GamePadState state5 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad2 = ((GamePadState)(ref state5)).DPad;
		if ((int)((GamePadDPad)(ref dPad2)).Up != 1)
		{
			GamePadState state6 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref state6)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks2)).Left.Y >= 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)104))
			{
				m_game.m_d_up_pressed = false;
				goto IL_065e;
			}
		}
		if (!m_game.m_d_up_pressed)
		{
			m_game.m_d_left_pressed = true;
			m_game.m_d_right_pressed = true;
			m_game.m_d_up_pressed = true;
			m_game.m_d_down_pressed = true;
			if (m_game.m_over_trigger == m_triggers[1])
			{
				m_game.m_cursor.m_pos.Y = 405f;
				return;
			}
			ref Vector2 pos2 = ref m_game.m_cursor.m_pos;
			pos2.Y -= 73f;
			if (m_game.m_cursor.m_pos.Y < 186f)
			{
				m_game.m_cursor.m_pos.Y = 332f;
			}
			return;
		}
		goto IL_065e;
		IL_065e:
		GamePadState state7 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadButtons buttons2 = ((GamePadState)(ref state7)).Buttons;
		if ((int)((GamePadButtons)(ref buttons2)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)65))
		{
			if (m_game.m_a_pressed)
			{
				return;
			}
			m_game.m_a_pressed = true;
			if (m_game.m_over_trigger != null)
			{
				if (!m_game.m_over_trigger.m_activate_own)
				{
					m_game.ActivateTrigger(m_game.m_over_trigger);
					return;
				}
				m_game.m_over_trigger.Activate();
				m_game.m_over_trigger = null;
			}
		}
		else
		{
			m_game.m_a_pressed = false;
		}
		return;
		IL_041f:
		GamePadState state8 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad3 = ((GamePadState)(ref state8)).DPad;
		if ((int)((GamePadDPad)(ref dPad3)).Down != 1)
		{
			GamePadState state9 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref state9)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks3)).Left.Y <= -0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)98))
			{
				m_game.m_d_down_pressed = false;
				goto IL_0531;
			}
		}
		if (!m_game.m_d_down_pressed)
		{
			m_game.m_d_left_pressed = true;
			m_game.m_d_right_pressed = true;
			m_game.m_d_up_pressed = true;
			m_game.m_d_down_pressed = true;
			ref Vector2 pos3 = ref m_game.m_cursor.m_pos;
			pos3.Y += 73f;
			if (m_game.m_over_trigger != m_triggers[7] && m_game.m_cursor.m_pos.Y > 332f)
			{
				m_game.m_cursor.m_pos.Y = 186f;
			}
			return;
		}
		goto IL_0531;
		IL_030b:
		GamePadState state10 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad4 = ((GamePadState)(ref state10)).DPad;
		if ((int)((GamePadDPad)(ref dPad4)).Left != 1)
		{
			GamePadState state11 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref state11)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks4)).Left.X <= -0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)100))
			{
				m_game.m_d_left_pressed = false;
				goto IL_041f;
			}
		}
		if (!m_game.m_d_left_pressed)
		{
			m_game.m_d_left_pressed = true;
			m_game.m_d_right_pressed = true;
			m_game.m_d_up_pressed = true;
			m_game.m_d_down_pressed = true;
			if (m_game.m_over_trigger != m_triggers[9])
			{
				ref Vector2 pos4 = ref m_game.m_cursor.m_pos;
				pos4.X -= 59f;
				if (m_game.m_cursor.m_pos.X < 144f)
				{
					m_game.m_cursor.m_pos.X = 262f;
				}
			}
			return;
		}
		goto IL_041f;
	}

	public override void Draw(SpriteBatch SB)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		if (m_tv_puzzle)
		{
			if (m_change_channel_timer <= 0f)
			{
				m_tv_animation.Draw(SB);
			}
			else
			{
				SB.Begin((SpriteBlendMode)1);
				SB.Draw(m_scenes[m_current_scene].m_texture, Game.VIEW_RECT, Color.White);
				SB.End();
			}
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_remote, m_remote_pos, Color.White);
			if (m_change_channel_timer <= 0f && m_channel_text.Length > 0)
			{
				Vector2 val = m_font.MeasureString(m_channel_text);
				Vector2 zero = Vector2.Zero;
				zero.X = 675f - val.X;
				zero.Y = 188f;
				Rectangle val2 = default(Rectangle);
				((Rectangle)(ref val2))._002Ector((int)zero.X - 10, (int)zero.Y - 5, (int)val.X + 20, (int)val.Y + 6);
				SB.Draw(m_fade, val2, new Color((byte)0, (byte)0, (byte)0, (byte)128));
				SB.DrawString(m_font, m_channel_text, zero, Color.LightGreen);
			}
			SB.End();
		}
		else
		{
			base.Draw(SB);
		}
	}
}
