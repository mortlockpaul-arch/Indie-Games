using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Game.World.Views.Room3;

internal class View1 : View
{
	private Animation2D m_intro_anim;

	private bool m_show_intro = true;

	private Animation2D m_left_anim_1;

	private Animation2D m_left_anim_2;

	private Animation2D m_left_anim_3;

	private Animation2D m_left_anim_4;

	private SoundEffect m_door_lock;

	public View1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view1_wrongpositions")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view1_rightpositions")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view1_wrongpositions_noshadows")));
		m_left_anim_1 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2_wrong/");
		m_left_anim_2 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2_correct/");
		m_left_anim_3 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2_correct2/");
		m_left_anim_4 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2_completed/");
		m_left_animation = m_left_anim_1;
		m_intro_anim = new AlphaAnimation(game, 100u, reverse: false, m_scenes[2].m_texture);
		m_intro_anim.Play();
		m_door_lock = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/door_lock");
	}

	public override void Clear()
	{
		m_door_lock = null;
		if (m_intro_anim != null)
		{
			m_intro_anim.Clear();
			m_intro_anim = null;
		}
		if (m_left_anim_1 != null)
		{
			m_left_anim_1.Clear();
			m_left_anim_1 = null;
		}
		if (m_left_anim_2 != null)
		{
			m_left_anim_2.Clear();
			m_left_anim_2 = null;
		}
		if (m_left_anim_3 != null)
		{
			m_left_anim_3.Clear();
			m_left_anim_3 = null;
		}
		if (m_left_anim_4 != null)
		{
			m_left_anim_4.Clear();
			m_left_anim_4 = null;
		}
		base.Clear();
	}

	public override void Reset()
	{
		base.Reset();
		m_game.m_input_enabled = true;
		m_game.m_show_cursor = true;
		m_game.m_inventory_enabled = true;
		if (m_show_intro)
		{
			m_game.m_input_enabled = false;
			m_game.m_show_cursor = false;
			m_game.m_update_cursor = false;
			m_game.m_inventory_enabled = false;
			m_game.m_hud.m_state = HUD.HUD_STATE.NONE;
			m_game.PlaySound(m_door_lock, 0.5f);
		}
	}

	public override void Setup()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		m_left_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.LEFT);
		m_down_trigger = new ViewTrigger(m_game, this, m_room.GetView("View3"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		ViewTrigger viewTrigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		viewTrigger.m_event = "View1_1.onReset";
		CursorTrigger item = new CursorTrigger(m_game, new Rectangle(140, 21, 472, 205), viewTrigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
		if (m_game.m_game_data.GetState("Room3.PuzzleCompleted") == "1")
		{
			HandlePuzzleCompleted();
		}
		if (m_game.m_game_data.GetState("Room3.Entered") == "1")
		{
			m_show_intro = false;
		}
		if (m_game.m_game_data.GetState("Room3.TicTacCompleted") == "1")
		{
			HandleTicTacCompleted();
		}
	}

	private void HandlePuzzleCompleted()
	{
		ChangeScene(1);
		m_triggers[0].m_enabled = false;
		m_left_animation = m_left_anim_2;
	}

	private void HandleTicTacCompleted()
	{
		m_left_animation = m_left_anim_4;
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "Room3.PuzzleCompleted":
			HandlePuzzleCompleted();
			break;
		case "View2_1.onWin1":
			m_left_animation = m_left_anim_3;
			break;
		case "View2_1.onWin3":
			HandleTicTacCompleted();
			break;
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
		if (!m_show_intro)
		{
			return;
		}
		m_game.m_input_enabled = false;
		m_game.m_show_cursor = false;
		m_game.m_update_cursor = false;
		m_game.m_inventory_enabled = false;
		m_game.m_hud.m_state = HUD.HUD_STATE.NONE;
		if (m_intro_anim != null)
		{
			m_intro_anim.Update(elapsed);
			if (m_intro_anim.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
			{
				m_show_intro = false;
				m_game.m_input_enabled = true;
				m_game.m_show_cursor = true;
				m_game.m_update_cursor = true;
				m_game.m_inventory_enabled = true;
				m_game.m_hud.m_state = HUD.HUD_STATE.NAVIGATOR;
				m_game.m_hud.FadeIn();
				m_triggers[0].m_state = Trigger.TRIGGER_STATE.IDLE;
				m_game.m_game_data.SetState("Room3.Entered", "1");
			}
		}
	}

	public override void Draw(SpriteBatch SB)
	{
		base.Draw(SB);
		if (m_show_intro && m_intro_anim != null)
		{
			m_intro_anim.Draw(SB);
		}
	}
}
