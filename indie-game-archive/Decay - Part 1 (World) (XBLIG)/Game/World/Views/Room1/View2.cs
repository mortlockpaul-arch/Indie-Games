using System.Collections.Generic;

namespace Game.World.Views.Room1;

internal class View2 : View
{
	protected List<TextureAnimation> m_gate_animations = new List<TextureAnimation>(3);

	public View2(Game game, Area room)
		: base(game, room)
	{
		m_name = "View2";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view2")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view2_doll")));
		m_down_animation = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2/");
		m_gate_animations.Add((TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2Gate1/"));
		m_gate_animations.Add((TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2Gate3/"));
		m_gate_animations.Add((TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2Gate3_water/"));
	}

	public override void Clear()
	{
		for (int i = 0; i < m_gate_animations.Count; i++)
		{
			if (m_gate_animations[i] != null)
			{
				m_gate_animations[i].Clear();
				m_gate_animations[i] = null;
			}
		}
		m_gate_animations.Clear();
		m_gate_animations = null;
		base.Clear();
	}

	public override void Setup()
	{
		m_down_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.DOWN_REVERSE);
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View2_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_handfat", trigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
		trigger = new ViewTrigger(m_game, this, m_room.GetView("View2_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_skap", trigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
		item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_snara", "Snara", Trigger.TRIGGER_TYPE.ZOOM_SMALL);
		item.m_activate_own = true;
		m_triggers.Add(item);
		if (m_game.m_game_data.GetState("Room1.GateState") != "")
		{
			HandleGateState(m_game.m_game_data.GetState("Room1.GateState"));
		}
		if (m_game.m_inventory.FindItem("Key01") || m_game.m_game_data.GetState("Room1.Door") == "Unlocked")
		{
			HandleKey01();
		}
	}

	private void HandleKey01()
	{
		RemoveTrigger(m_triggers[0]);
	}

	private void HandleGateState(string state)
	{
		switch (state)
		{
		case "1":
			RemoveTrigger(m_triggers[1]);
			m_down_animation = m_gate_animations[0];
			break;
		case "3":
			ChangeScene(1);
			RemoveTrigger(m_triggers[1]);
			m_down_animation = m_gate_animations[1];
			break;
		case "4":
			ChangeScene(1);
			RemoveTrigger(m_triggers[1]);
			m_down_animation = m_gate_animations[2];
			break;
		}
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "PickupKey01":
			HandleKey01();
			break;
		case "PickupPincett01":
			HandleGateState("1");
			break;
		case "Snara":
			m_game.m_hud.ShowText("What's this!? Did I really try to kill myself?", m_use_text_fade);
			break;
		case "View1_1_1.UsePolygrip":
			HandleGateState(m_game.m_game_data.GetState("Room1.GateState"));
			break;
		case "View2.onDoll":
			m_game.m_hud.ShowText("How did that doll end up in here!?", m_use_text_fade);
			break;
		}
		base.HandleEvent(s_event);
	}
}
