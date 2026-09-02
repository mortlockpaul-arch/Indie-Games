using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Game.World.Views.Hallway1;

internal class View1_1 : View
{
	private SoundEffect m_skrap_sound;

	private SoundEffectInstance m_skrap_sound_inst;

	private SoundEffect m_footsteps;

	private SoundEffectInstance m_footsteps_inst;

	private bool m_pan_footsteps;

	private float m_footsteps_pan = -1f;

	public View1_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "door_zoom")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "door_zoom_unlocked")));
		m_skrap_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/skrubb_oljud");
		m_skrap_sound_inst = m_skrap_sound.CreateInstance();
		m_skrap_sound_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * 0.2f;
		m_skrap_sound_inst.Pitch = -0.5f;
		m_skrap_sound_inst.IsLooped = true;
		m_footsteps = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/skrubb_doll_fotsteg");
		m_footsteps_inst = m_footsteps.CreateInstance();
		m_footsteps_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * 0.5f;
		m_footsteps_inst.Pan = m_footsteps_pan;
		m_footsteps_inst.Pitch = 0.2f;
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Clear()
	{
		if (m_skrap_sound_inst != null)
		{
			m_skrap_sound_inst.Stop();
			m_skrap_sound_inst.Dispose();
			m_skrap_sound_inst = null;
		}
		m_skrap_sound = null;
		if (m_footsteps_inst != null)
		{
			m_footsteps_inst.Stop();
			m_footsteps_inst.Dispose();
			m_footsteps_inst = null;
		}
		m_footsteps = null;
		base.Clear();
	}

	public override void Setup()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		m_back_trigger.m_stop_sound = m_skrap_sound_inst;
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger item = new CursorTrigger(m_game, new Rectangle(339, 212, 359, 238), trigger, Trigger.TRIGGER_TYPE.USE_SMALL);
		m_triggers.Add(item);
		if (m_game.m_game_data.GetState(m_room.m_name + ".Door01") == "Unlocked")
		{
			HandleDoor01Unlocked();
		}
	}

	public override void Reset()
	{
		base.Reset();
		m_game.m_update_cursor = true;
		m_game.m_show_cursor = true;
		m_game.m_inventory_enabled = true;
		m_room.HandleEvent("View1_1_2.LightOff");
		m_game.m_game_data.SetState("Music", "1");
		m_game.PlayMusic(m_game.m_music1);
		if (m_game.m_game_data.GetState("Hallway1.View1_1_2.SeenDoll") != "1")
		{
			m_skrap_sound_inst.Play();
		}
		if (m_game.m_game_data.GetState("Hallway1.View1_1.PlayFootsteps") == "1")
		{
			m_game.m_game_data.SetState("Hallway1.View1_1.PlayFootsteps", "0");
			m_pan_footsteps = true;
			m_footsteps_inst.Play();
		}
	}

	private void HandleDoor01Unlocked()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		ChangeScene(1);
		RemoveTrigger(m_triggers[0]);
		ViewTrigger viewTrigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		viewTrigger.m_sound = m_game.m_door_open;
		viewTrigger.m_stop_sound = m_skrap_sound_inst;
		CursorTrigger item = new CursorTrigger(m_game, new Rectangle(147, 0, 383, Game.VIEW_RECT.Height / 2), viewTrigger, Trigger.TRIGGER_TYPE.USE);
		m_triggers.Add(item);
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "Door01_Unlocked":
			HandleDoor01Unlocked();
			break;
		case "VolumeChanged":
			if (m_skrap_sound_inst != null)
			{
				m_skrap_sound_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * 0.2f;
			}
			break;
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
		if (m_pan_footsteps && m_footsteps_pan < 1f)
		{
			m_footsteps_pan += (float)elapsed.TotalMilliseconds * 0.001f * 1f;
			if (m_footsteps_pan >= 1f)
			{
				m_footsteps_pan = 1f;
				m_pan_footsteps = false;
			}
			m_footsteps_inst.Pan = m_footsteps_pan;
		}
	}
}
