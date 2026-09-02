using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Game.World.Views.Room2;

internal class View2 : View
{
	private bool m_goto_store;

	public View2(Game game, Area room)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(game, room);
		m_name = "View2";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "vinkel 2")));
		m_right_animation = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2Empty/");
		ViewItem viewItem = null;
		viewItem = new ViewItem("Frame01", game, m_room.m_CL, m_room.m_content_path + "vinkel 2", new Rectangle(0, 180, 640, 80));
		viewItem.LoadRightAnimation((TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2Items_item/"));
		m_items.Add(viewItem);
		viewItem = new ViewItem("Remote01", game, m_room.m_CL, m_room.m_content_path + "vinkel 2", new Rectangle(0, 280, 640, 80));
		viewItem.LoadRightAnimation((TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2Items_item/"));
		viewItem.m_update_animation = false;
		m_items.Add(viewItem);
	}

	public override void Clear()
	{
		base.Clear();
	}

	public override void Setup()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		m_right_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.RIGHT_REVERSE);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_door_vinkel-2", "Door_View2", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		ViewTrigger trigger = new ViewTrigger(m_game, this, m_room.GetView("View2_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(123, 118, 178, 190), trigger, Trigger.TRIGGER_TYPE.ZOOM);
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
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		switch (s_event)
		{
		case "PickupFrame01":
			RemoveFrame01();
			break;
		case "PickupRemote01":
			RemoveRemote01();
			break;
		case "Door_View2":
			if (!Guide.IsTrialMode)
			{
				m_game.ChangeArea("Hallway1", "View1", door_sound: true);
				break;
			}
			Guide.BeginShowMessageBox(Game.PLAYER_INDEX, "Message", "This area is only available in the full version.", (IEnumerable<string>)new string[2] { "Unlock full game", "Continue" }, 1, (MessageBoxIcon)0, (AsyncCallback)onMessageFinished, (object)object.Equals(0, 0));
			break;
		}
		base.HandleEvent(s_event);
	}

	protected void onMessageFinished(IAsyncResult res)
	{
		try
		{
			int? num = Guide.EndShowMessageBox(res);
			int? num2 = num;
			if (num2.GetValueOrDefault() == 0 && num2.HasValue)
			{
				m_goto_store = true;
			}
		}
		catch
		{
		}
	}

	protected void onMessage2Finished(IAsyncResult res)
	{
		try
		{
			Guide.EndShowMessageBox(res);
		}
		catch
		{
		}
	}

	public override void Update(TimeSpan elapsed)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		base.Update(elapsed);
		if (m_goto_store && !Guide.IsVisible)
		{
			m_goto_store = false;
			try
			{
				Guide.ShowMarketplace(Game.PLAYER_INDEX);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Guide.BeginShowMessageBox(Game.PLAYER_INDEX, "Message", ex.Message, (IEnumerable<string>)new string[1] { "Ok" }, 0, (MessageBoxIcon)0, (AsyncCallback)onMessage2Finished, (object)object.Equals(0, 0));
			}
		}
	}
}
