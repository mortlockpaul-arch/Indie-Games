using Microsoft.Xna.Framework;

namespace Game.World.Views.Room2;

internal class View1 : View
{
	public View1(Game game, Area room)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(game, room);
		m_name = "View1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "vinkel 1 no objects")));
		m_left_animation = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2Empty/");
		m_right_animation = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to3Empty/");
		ViewItem viewItem = null;
		viewItem = new ViewItem("Frame01", game, m_room.m_CL, m_room.m_content_path + "vinkel 1", new Rectangle(0, 180, 640, 80));
		viewItem.LoadLeftAnimation((TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2Items_item/"));
		viewItem.LoadRightAnimation((TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to3Items_item/"));
		m_items.Add(viewItem);
		viewItem = new ViewItem("Remote01", game, m_room.m_CL, m_room.m_content_path + "vinkel 1", new Rectangle(0, 280, 640, 80));
		viewItem.LoadLeftAnimation((TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2Items_item/"));
		viewItem.LoadRightAnimation((TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to3Items_item/"));
		viewItem.m_update_animation = false;
		m_items.Add(viewItem);
	}

	public override void Setup()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		m_left_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.LEFT);
		m_right_trigger = new ViewTrigger(m_game, this, m_room.GetView("View3"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.RIGHT_REVERSE);
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK);
		CursorTrigger item = new CursorTrigger(m_game, new Rectangle(270, 135, 475, 255), trigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
		trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK);
		item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_table", trigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
		trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_3"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK);
		item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_tv", trigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
		if (m_game.m_inventory.FindItem("Frame01"))
		{
			RemoveFrame01();
		}
		if (m_game.m_inventory.FindItem("Remote01"))
		{
			RemoveRemote01();
		}
	}

	protected void RemoveFrame01()
	{
		RemoveItem("Frame01");
	}

	protected void RemoveRemote01()
	{
		RemoveItem("Remote01");
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "PickupFrame01":
			RemoveFrame01();
			break;
		case "PickupRemote01":
			RemoveRemote01();
			break;
		}
		base.HandleEvent(s_event);
	}
}
