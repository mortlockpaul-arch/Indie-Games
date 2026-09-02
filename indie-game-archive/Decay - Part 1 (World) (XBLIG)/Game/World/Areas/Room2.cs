using System;
using Game.World.Views.Room2;

namespace Game.World.Areas;

public class Room2 : Area
{
	public Room2()
	{
		m_content_path = "World/Room2/";
		m_name = "Room2";
	}

	public override void Load(Game game)
	{
		base.Load(game);
		TextureAnimation textureAnimation = null;
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2Empty/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 50);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2Empty/", 1u, reverse: false);
		textureAnimation.m_path = m_content_path + "Animations/View1to2Empty_item/";
		textureAnimation.UseCombinedFrames(320, 180, 50);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2Items/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 21);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2Items/", 1u, reverse: false);
		textureAnimation.m_path = m_content_path + "Animations/View1to2Items_item/";
		textureAnimation.UseCombinedFrames(320, 180, 21);
		textureAnimation.AddAnimation((TextureAnimation)m_CL.GetContent(m_content_path + "Animations/View1to2Empty_item/"), 21, 49);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to3Empty/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 50);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to3Empty/", 1u, reverse: false);
		textureAnimation.m_path = m_content_path + "Animations/View1to3Empty_item/";
		textureAnimation.UseCombinedFrames(320, 180, 50);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to3Items/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 22);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to3Empty/", 1u, reverse: false);
		textureAnimation.m_path = m_content_path + "Animations/View1to3Items_item/";
		textureAnimation.UseCombinedFrames(320, 180, 28);
		textureAnimation.AddAnimation((TextureAnimation)m_CL.GetContent(m_content_path + "Animations/View1to3Items/"), 0, 21);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/TV1/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(640, 360, 3);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/TV2/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(640, 360, 4);
		m_CL.AddContent(textureAnimation);
		textureAnimation = null;
		new View1(m_game, this);
		new View1_1(m_game, this);
		new View1_2(m_game, this);
		new View1_2_1(m_game, this);
		new View1_3(m_game, this);
		new View2(m_game, this);
		new View2_1(m_game, this);
		new View3(m_game, this);
		new View3_1(m_game, this);
		SetupViews();
		m_game.m_game_data.SetState("Music", "1");
	}

	public override void Clear()
	{
		base.Clear();
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "Frame01":
		case "Remote01":
			m_game.m_inventory.AskPickup(s_event);
			break;
		case "PickupFrame01":
			m_game.m_inventory.AddItem("Frame01");
			break;
		case "PickupRemote01":
			m_game.m_inventory.AddItem("Remote01");
			break;
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
	}
}
