using Game.World.Views.Hallway1;

namespace Game.World.Areas;

public class Hallway1 : Area
{
	public Hallway1()
	{
		m_content_path = "World/Hallway1/";
		m_name = "Hallway1";
	}

	public override void Load(Game game)
	{
		base.Load(game);
		TextureAnimation textureAnimation = null;
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 50);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2_closed/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 15);
		textureAnimation.m_frame_smoothing = true;
		textureAnimation.AddAnimation((TextureAnimation)m_CL.GetContent(m_content_path + "Animations/View1to2/"), 16, 49);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to3/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 50);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View2to4/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 50);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View2to4_closed/", 1u, reverse: false);
		textureAnimation.m_path = m_content_path + "Animations/View2to4_closed_/";
		textureAnimation.UseCombinedFrames(320, 180, 25);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View2to4/", 1u, reverse: false);
		textureAnimation.m_path = m_content_path + "Animations/View2to4_closed/";
		textureAnimation.UseCombinedFrames(320, 180, 25);
		textureAnimation.m_frame_smoothing = true;
		textureAnimation.AddAnimation((TextureAnimation)m_CL.GetContent(m_content_path + "Animations/View2to4_closed_/"), 0, 24);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View3to4/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 50);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = null;
		new View1(m_game, this);
		new View1_1(m_game, this);
		new View1_1_1(m_game, this);
		new View1_1_2(m_game, this);
		new View1_1_2_1(m_game, this);
		new View1_1_2_2(m_game, this);
		new View1_2(m_game, this);
		new View2(m_game, this);
		new View2_1(m_game, this);
		new View3(m_game, this);
		new View3_1(m_game, this);
		new View3_1_1(m_game, this);
		new View3_1_2(m_game, this);
		new View3_1_3(m_game, this);
		new View3_2(m_game, this);
		new View4(m_game, this);
		SetupViews();
		if (m_game.m_game_data.GetState("Room3.Completed") == "1")
		{
			m_game.HandleEvent("Room3.Completed");
		}
		m_game.m_game_data.SetState("Music", "1");
	}

	public override void Clear()
	{
		base.Clear();
	}

	public override void HandleEvent(string s_event)
	{
		base.HandleEvent(s_event);
	}
}
