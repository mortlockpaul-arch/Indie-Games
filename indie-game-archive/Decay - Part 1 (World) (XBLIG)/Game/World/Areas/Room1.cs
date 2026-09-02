using System;
using Game.World.Views.Room1;
using Microsoft.Xna.Framework.Audio;

namespace Game.World.Areas;

public class Room1 : Area
{
	private SoundEffect m_ambient;

	private SoundEffectInstance m_ambient_inst;

	private bool m_fadeout_ambient;

	public Room1()
	{
		m_content_path = "World/Room1/";
		m_name = "Room1";
	}

	public override void Load(Game game)
	{
		base.Load(game);
		TextureAnimation textureAnimation = null;
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 50);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2Gate1/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 21);
		textureAnimation.m_frame_smoothing = true;
		textureAnimation.AddAnimation((TextureAnimation)m_CL.GetContent(m_content_path + "Animations/View1to2/"), 21, 49);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2Gate3/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 50);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2Gate3_water/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 14);
		textureAnimation.AddAnimation((TextureAnimation)m_CL.GetContent(m_content_path + "Animations/View1to2Gate3/"), 14, 49);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/Dive/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 50);
		textureAnimation.SetFPS(15.0);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(m_game, m_CL, m_content_path + "Animations/Polygrip/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 30);
		m_CL.AddContent(textureAnimation);
		textureAnimation = null;
		m_ambient = m_CL.LoadSound(m_content_path + "/Sound/bathroom_ambient");
		m_ambient_inst = m_ambient.CreateInstance();
		m_ambient_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * 0.7f;
		m_ambient_inst.IsLooped = true;
		m_ambient_inst.Play();
		new View1(m_game, this);
		new View1_1(m_game, this);
		new View1_1_1(m_game, this);
		new View1_2(m_game, this);
		new View1_2_1(m_game, this);
		new View2(m_game, this);
		new View2_1(m_game, this);
		new View2_2(m_game, this);
		new View2_2_1(m_game, this);
		SetupViews();
		m_game.FadeOutMusic();
	}

	public override void Clear()
	{
		if (m_ambient_inst != null)
		{
			m_ambient_inst.Stop();
			m_ambient_inst.Dispose();
			m_ambient_inst = null;
		}
		m_ambient = null;
		base.Clear();
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "Key01":
		case "Pincett01":
		case "Polygrip01":
			m_game.m_inventory.AskPickup(s_event);
			break;
		case "PickupKey01":
			m_game.m_inventory.AddItem("Key01");
			break;
		case "PickupPincett01":
			m_game.m_inventory.AddItem("Pincett01");
			break;
		case "KeyholeStuck":
			m_game.m_hud.ShowText("Something seems to be stuck inside the keyhole.", fade: false);
			break;
		case "DoorHandle":
			if (m_game.m_game_data.GetState("Room1.Door") != "Unlocked")
			{
				m_game.m_hud.ShowText("The door is locked.", fade: false);
				break;
			}
			m_game.m_game_data.SetState("Music", "1");
			m_game.ChangeArea("Room2", "View1", door_sound: true);
			break;
		case "Room1.StopAmbient":
			m_fadeout_ambient = true;
			break;
		case "VolumeChanged":
			if (m_ambient_inst != null)
			{
				m_ambient_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * 0.7f;
			}
			break;
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
		if (m_fadeout_ambient && m_ambient_inst != null)
		{
			float volume = m_ambient_inst.Volume;
			volume -= (float)elapsed.TotalMilliseconds * 0.001f * 0.2f;
			if (volume <= 0f)
			{
				volume = 0f;
			}
			m_ambient_inst.Volume = volume;
		}
	}
}
