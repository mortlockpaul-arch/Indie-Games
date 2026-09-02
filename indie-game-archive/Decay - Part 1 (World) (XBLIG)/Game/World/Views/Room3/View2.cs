using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Game.World.Views.Room3;

internal class View2 : View
{
	private SoundEffect m_sound;

	private SoundEffect m_sound_blood;

	private Animation2D m_right_anim_1;

	private Animation2D m_right_anim_2;

	private Animation2D m_right_anim_3;

	private Animation2D m_right_anim_4;

	private Animation2D m_rotate_painting;

	private Animation2D m_win_anim;

	private int m_play_win_anim;

	private Texture2D m_blood;

	private Animation2D m_blood_anim;

	private float m_blood_timer;

	private int m_blood_timer_min = 2;

	private int m_blood_timer_max = 10;

	public View2(Game game, Area room)
		: base(game, room)
	{
		m_name = "View2";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view2_oldman_shadow")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view2_all_shadows")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view2_noshadow")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "view2_upsidedown")));
		m_blood = m_room.m_CL.LoadTexture(m_room.m_content_path + "view2_oldman_shadow_blood");
		m_right_anim_1 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2_wrong/");
		m_right_anim_2 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2_correct/");
		m_right_anim_3 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2_correct2/");
		m_right_anim_4 = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/View1to2_completed/");
		m_right_animation = m_right_anim_1;
		m_rotate_painting = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/RotatePainting/");
		m_rotate_painting.SetFPS(25.0);
		m_blood_anim = new AlphaAnimation(m_game, 25u, reverse: false, m_blood);
		m_blood_timer = m_game.GetRandom(m_blood_timer_min, m_blood_timer_max);
		m_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/right_room_tavla_roterar");
		m_sound_blood = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/rightroom_trycka_pa_tavla");
	}

	public override void Clear()
	{
		m_sound = null;
		m_sound_blood = null;
		if (m_blood != null)
		{
			((GraphicsResource)m_blood).Dispose();
			m_blood = null;
		}
		if (m_blood_anim != null)
		{
			m_blood_anim.Clear();
			m_blood_anim = null;
		}
		if (m_right_anim_1 != null)
		{
			m_right_anim_1.Clear();
			m_right_anim_1 = null;
		}
		if (m_right_anim_2 != null)
		{
			m_right_anim_2.Clear();
			m_right_anim_2 = null;
		}
		if (m_right_anim_3 != null)
		{
			m_right_anim_3.Clear();
			m_right_anim_3 = null;
		}
		if (m_right_anim_4 != null)
		{
			m_right_anim_4.Clear();
			m_right_anim_4 = null;
		}
		if (m_rotate_painting != null)
		{
			m_rotate_painting.Clear();
			m_rotate_painting = null;
		}
		if (m_win_anim != null)
		{
			m_win_anim.Clear();
			m_win_anim = null;
		}
		base.Clear();
	}

	public override void Reset()
	{
		base.Reset();
		m_game.m_show_cursor = true;
		m_game.m_inventory_enabled = true;
		((View2_1)m_room.GetView("View2_1"))?.ResetBoard();
		switch (m_play_win_anim)
		{
		case 1:
			if (m_win_anim != null)
			{
				m_win_anim.Clear();
				m_win_anim = null;
			}
			m_win_anim = new AlphaAnimation(m_game, 25u, reverse: false, m_scenes[m_current_scene].m_texture);
			m_win_anim.Play();
			ChangeScene(0);
			m_right_animation = m_right_anim_3;
			m_game.m_show_cursor = false;
			m_hud_state = HUD.HUD_STATE.NONE;
			break;
		case 2:
			Console.WriteLine("Stop effect!");
			m_play_win_anim = 0;
			m_game.m_show_cursor = true;
			m_game.m_update_cursor = true;
			m_game.m_input_enabled = true;
			m_hud_state = HUD.HUD_STATE.NAVIGATOR;
			m_game.m_hud.m_state = HUD.HUD_STATE.NAVIGATOR;
			m_game.m_hud.FadeIn();
			m_game.m_cursor.onOut();
			break;
		case 3:
			if (m_win_anim != null)
			{
				m_win_anim.Clear();
				m_win_anim = null;
			}
			m_win_anim = new AlphaAnimation(m_game, 25u, reverse: false, m_scenes[m_current_scene].m_texture);
			m_win_anim.Play();
			ChangeScene(2);
			m_game.m_show_cursor = false;
			m_hud_state = HUD.HUD_STATE.NONE;
			break;
		}
	}

	public override void Setup()
	{
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		m_right_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.RIGHT_REVERSE);
		m_left_trigger = new ViewTrigger(m_game, this, m_room.GetView("View3"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		if (m_game.m_game_data.GetState("Room3.PuzzleCompleted") == "1")
		{
			HandlePuzzleCompleted();
		}
		ViewTrigger viewTrigger = new ViewTrigger(m_game, this, m_room.GetView("View2_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK);
		viewTrigger.m_event = "View2_1.onZoom";
		CursorTrigger item = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_table", viewTrigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
		viewTrigger = new ViewTrigger(m_game, this, m_room.GetView("View2_2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		item = new CursorTrigger(m_game, new Rectangle(324, 109, 382, 178), viewTrigger, Trigger.TRIGGER_TYPE.ZOOM);
		m_triggers.Add(item);
		if (m_game.m_game_data.GetState("Room3.TicTacCompleted") == "1")
		{
			HandleTicTacCompleted();
		}
		if (m_game.m_game_data.GetState("Room3.Completed") == "1")
		{
			HandleRoom3Completed();
		}
	}

	private void HandleRoom3Completed()
	{
		m_triggers[1].m_enabled = false;
	}

	private void HandlePuzzleCompleted()
	{
		ChangeScene(1);
		m_right_animation = m_right_anim_2;
	}

	private void HandleTicTacCompleted()
	{
		m_triggers[0].m_enabled = false;
		ChangeScene(3);
		m_right_animation = m_right_anim_4;
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "Room3.PuzzleCompleted":
			HandlePuzzleCompleted();
			break;
		case "View2_1.onWin1":
			m_play_win_anim = 1;
			break;
		case "View2_1.onWin2":
			m_play_win_anim = 2;
			break;
		case "View2_1.onWin3":
			m_play_win_anim = 3;
			break;
		case "Room3.Completed":
			HandleRoom3Completed();
			break;
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
		if (m_blood_timer > 0f && m_current_scene == 0)
		{
			m_blood_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_blood_timer <= 0f)
			{
				m_blood_timer = 0f;
				if (m_blood_anim != null)
				{
					m_blood_anim.Play();
					m_game.PlaySound(m_sound_blood, 0.5f, -1f);
				}
			}
		}
		if (m_blood_anim != null)
		{
			m_blood_anim.Update(elapsed);
		}
		if (m_play_win_anim == 0 || m_game.m_state == Game.GAME_STATE.ACTIVE_TRIGGER || m_win_anim == null)
		{
			return;
		}
		m_win_anim.Update(elapsed);
		if (m_win_anim.m_state != Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
		{
			return;
		}
		if (m_play_win_anim != 3)
		{
			m_play_win_anim = 0;
			m_game.m_show_cursor = true;
			m_game.m_update_cursor = true;
			m_game.m_input_enabled = true;
			m_hud_state = HUD.HUD_STATE.NAVIGATOR;
			m_game.m_hud.m_state = HUD.HUD_STATE.NAVIGATOR;
			m_game.m_hud.FadeIn();
			base.Reset();
			m_game.m_cursor.onOut();
		}
		else
		{
			m_play_win_anim = 4;
			if (m_win_anim != null)
			{
				m_win_anim.Clear();
				m_win_anim = null;
			}
			m_win_anim = m_rotate_painting;
			m_win_anim.Play();
			m_game.PlaySound(m_sound, 0.5f);
			HandleTicTacCompleted();
		}
	}

	public override void Draw(SpriteBatch SB)
	{
		base.Draw(SB);
		if (m_blood_anim != null && m_current_scene == 0 && m_blood_anim.m_state == Animation2D.ANIM_STATE.ANIM_STATE_PLAYING)
		{
			m_blood_anim.Draw(SB);
		}
		if (m_play_win_anim != 0 && m_win_anim != null)
		{
			m_win_anim.Draw(SB);
		}
	}
}
