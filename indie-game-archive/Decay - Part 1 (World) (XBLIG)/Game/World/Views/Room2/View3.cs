using Microsoft.Xna.Framework;

namespace Game.World.Views.Room2;

internal class View3 : View
{
	public View3(Game game, Area room)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(game, room);
		m_name = "View3";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "vinkel 3")));
		m_left_animation = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to3Empty/");
		ViewItem viewItem = null;
		viewItem = new ViewItem("Frame01", game, m_room.m_CL, m_room.m_content_path + "vinkel 3", new Rectangle(0, 180, 640, 80));
		viewItem.LoadLeftAnimation((TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to3Items_item/"));
		m_items.Add(viewItem);
		viewItem = new ViewItem("Remote01", game, m_room.m_CL, m_room.m_content_path + "vinkel 3", new Rectangle(0, 280, 640, 80));
		viewItem.LoadLeftAnimation((TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to3Items_item/"));
		viewItem.m_update_animation = false;
		m_items.Add(viewItem);
	}

	public override void Clear()
	{
		base.Clear();
	}

	public override void Setup()
	{
		m_left_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.LEFT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_door_vinkel-3", "Door_View3", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View3_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_article_zoom", trigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(cursorTrigger);
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
		case "Door_View3":
			m_game.ChangeArea("Room1", "View2", door_sound: true);
			break;
		}
		base.HandleEvent(s_event);
	}
}
