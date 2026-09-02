using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Game.World.Views.Room1;

internal class View1 : View
{
	protected List<TextureAnimation> m_gate_animations = new List<TextureAnimation>(3);

	public View1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view1")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view1gate1")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view1gate3")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view1_gate3_water")));
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
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		m_down_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.DOWN);
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_badkar", trigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
		trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_door_vinkel1", trigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
		item = new CursorTrigger(m_game, new Rectangle(70, 25, 275, 187), "View1.onGate1", Trigger.TRIGGER_TYPE.ZOOM);
		item.m_activate_own = true;
		item.m_enabled = false;
		m_triggers.Add(item);
		if (m_game.m_game_data.GetState("Room1.GateState") != "")
		{
			HandleGateState(m_game.m_game_data.GetState("Room1.GateState"));
		}
		if (m_game.m_game_data.GetState("Room1.Door") == "Unlocked")
		{
			HandleDoorUnlocked();
		}
	}

	private void HandleGateState(string state)
	{
		switch (state)
		{
		case "1":
			ChangeScene(1);
			m_down_animation = m_gate_animations[0];
			m_triggers[2].m_enabled = true;
			break;
		case "3":
			ChangeScene(2);
			m_down_animation = m_gate_animations[1];
			m_triggers[2].m_enabled = false;
			break;
		case "4":
			ChangeScene(3);
			m_down_animation = m_gate_animations[2];
			m_triggers[2].m_enabled = false;
			break;
		}
	}

	public void HandleDoorUnlocked()
	{
		RemoveTrigger(m_triggers[1]);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_door_vinkel1", "DoorHandle", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
	}

	public override bool HandleUseEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "Key01" && m_game.m_game_data.GetState("Room1.Door") == "Unlocked")
		{
			HandleDoorUnlocked();
			return true;
		}
		return base.HandleUseEvent(s_event);
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "PickupPincett01":
			HandleGateState("1");
			break;
		case "View1_1_1.UsePolygrip":
			HandleGateState(m_game.m_game_data.GetState("Room1.GateState"));
			break;
		case "View1.onGate1":
			m_game.m_hud.ShowText("Some sort of writing appeared on the wall ...", m_use_text_fade);
			break;
		}
		base.HandleEvent(s_event);
	}
}
