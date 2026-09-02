using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game.World.Views.Room3;

internal class View2_2 : View
{
	private SoundEffect m_onframe_sound;

	private Texture2D m_o_white;

	private Animation2D m_effect_anim;

	private bool m_show_effect;

	private ViewTrigger m_VT_move_frame;

	public View2_2(Game game, Area room)
		: base(game, room)
	{
		m_name = "View2_2";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "frame_zoom1")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "frame_zoom2")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "frame_zoom3")));
		m_o_white = m_room.m_CL.LoadTexture(m_room.m_content_path + "Effect/o_white_effect3");
		m_effect_anim = new AlphaAnimation(m_game, 25u, reverse: false, m_o_white);
		m_onframe_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/rightroom_trycka_pa_tavla");
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
	}

	public override void Clear()
	{
		m_onframe_sound = null;
		if (m_o_white != null)
		{
			((GraphicsResource)m_o_white).Dispose();
			m_o_white = null;
		}
		if (m_effect_anim != null)
		{
			m_effect_anim.Clear();
			m_effect_anim = null;
		}
		if (m_VT_move_frame != null)
		{
			m_VT_move_frame.Clear();
			m_VT_move_frame = null;
		}
		base.Clear();
	}

	public override void Setup()
	{
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		m_VT_move_frame = new ViewTrigger(m_game, this, m_room.GetView("View2_2_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_frame_ud_zoom", "View2_2.onFrame", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, m_room.m_content_path + "collision_frame_zoom", "View2_2.onFrame", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		if (m_game.m_game_data.GetState("Room3.PuzzleCompleted") == "1")
		{
			HandlePuzzleCompleted();
		}
		if (m_game.m_game_data.GetState("Room3.TicTacCompleted") == "1")
		{
			HandleTicTacCompleted();
		}
	}

	private void HandlePuzzleCompleted()
	{
		ChangeScene(1);
	}

	private void HandleTicTacCompleted()
	{
		ChangeScene(2);
		m_triggers[0].m_enabled = true;
		m_triggers[1].m_enabled = false;
	}

	public override void HandleEvent(string s_event)
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		switch (s_event)
		{
		case "Room3.PuzzleCompleted":
			HandlePuzzleCompleted();
			break;
		case "View2_1.onWin1":
			ChangeScene(0);
			break;
		case "View2_1.onWin3":
			HandleTicTacCompleted();
			break;
		case "View2_2.onFrame":
			m_game.m_hud.ShowAsk("", "MOVE FRAME", "CANCEL", "View2_2.onMoveFrame", "", m_use_text_fade);
			break;
		case "View2_2.onMoveFrame":
			if (m_current_scene != 2)
			{
				m_show_effect = true;
				m_effect_anim.Play();
				m_game.m_input_enabled = false;
				m_game.m_inventory_enabled = false;
				GamePad.SetVibration(Game.PLAYER_INDEX, 1f, 1f);
				m_game.PlaySound(m_onframe_sound, 0.5f);
				m_game.m_hud.m_state = HUD.HUD_STATE.NONE;
				m_game.m_show_cursor = false;
			}
			else
			{
				m_game.ActivateTrigger(m_VT_move_frame);
				m_game.m_hud.m_state = HUD.HUD_STATE.NONE;
			}
			break;
		}
		base.HandleEvent(s_event);
	}

	public override void UpdateEffect(TimeSpan elapsed)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateEffect(elapsed);
		if (m_show_effect && m_effect_anim != null)
		{
			m_effect_anim.Update(elapsed);
			if (m_effect_anim.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
			{
				m_show_effect = false;
				m_game.m_input_enabled = true;
				m_game.m_inventory_enabled = true;
				m_game.ActivateTrigger(m_back_trigger);
				GamePad.SetVibration(Game.PLAYER_INDEX, 0f, 0f);
			}
		}
	}

	public override void DrawEffect(SpriteBatch SB)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		base.DrawEffect(SB);
		if (m_show_effect && m_effect_anim != null)
		{
			m_effect_anim.Draw(SB, Game.VIEW_RECT, new Rectangle(0, 0, m_o_white.Width, m_o_white.Height), new Color(byte.MaxValue, (byte)0, (byte)0, byte.MaxValue));
			m_effect_anim.Draw(SB, Game.VIEW_RECT, new Rectangle(0, 0, m_o_white.Width, m_o_white.Height), new Color(byte.MaxValue, (byte)0, (byte)0, byte.MaxValue));
		}
	}
}
