using System;
using Microsoft.Xna.Framework;

namespace Game.World.Views.Hallway1;

internal class View1 : View
{
	private Animation2D m_right_animation1;

	private Animation2D m_right_animation2;

	private float m_light_timer;

	private int m_light_timeout_min = 100;

	private int m_light_timeout_max = 1500;

	public View1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "front_view")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "front_view_nolamp")));
		m_left_animation = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to3/");
		m_right_animation1 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2_closed/");
		m_right_animation2 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2/");
		m_right_animation = m_right_animation1;
		m_light_timer = m_light_timeout_min;
	}

	public override void Clear()
	{
		if (m_right_animation1 != null)
		{
			m_right_animation1.Clear();
			m_right_animation1 = null;
		}
		if (m_right_animation2 != null)
		{
			m_right_animation2.Clear();
			m_right_animation2 = null;
		}
		base.Clear();
	}

	public override void Setup()
	{
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		m_left_trigger = new ViewTrigger(m_game, this, m_room.GetView("View3"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.LEFT);
		m_right_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.RIGHT_REVERSE);
		m_down_trigger = new ViewTrigger(m_game, this, m_room.GetView("View4"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_skrubb_door", trigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
		trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		item = new CursorTrigger(m_game, new Rectangle(252, 97, 401, 198), trigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
		if (m_game.m_inventory.FindItem("Note02"))
		{
			HandleNote02();
		}
	}

	private void HandleNote02()
	{
		m_right_animation = m_right_animation2;
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "PickupNote02")
		{
			HandleNote02();
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
		m_light_timer -= (float)elapsed.TotalMilliseconds;
		if (m_light_timer <= 0f)
		{
			m_light_timer = m_game.GetRandom(m_light_timeout_min, m_light_timeout_max);
			if (m_current_scene == 0)
			{
				ChangeScene(1);
			}
			else
			{
				ChangeScene(0);
			}
		}
	}
}
