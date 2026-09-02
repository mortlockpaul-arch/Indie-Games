using System;
using System.Collections.Generic;
using Game.Inventory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game.World.Views.Hallway1;

internal class View1_1_1 : View
{
	private struct LockNumber
	{
		public Texture2D m_texture;

		public int m_number;

		public Vector2 m_pos;
	}

	private SoundEffect m_tick_sound;

	private SoundEffect m_unlock_sound;

	private List<Texture2D> m_lock_textures = new List<Texture2D>(10);

	private Texture2D m_arrow;

	private Texture2D m_arrow_green;

	private Texture2D m_left_arrow;

	private Texture2D m_right_arrow;

	private LockNumber[] m_lock_numbers;

	private int m_current_lock_index;

	private bool m_update_puzzle;

	public View1_1_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_1_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "lock_zoom")));
		m_tick_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/korri_combinationlock_sound");
		m_unlock_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/korri_combinationlock_opens");
		m_arrow = m_room.m_CL.LoadTexture("Inventory/arrow");
		m_arrow_green = m_room.m_CL.LoadTexture("Inventory/arrow_green");
		m_left_arrow = m_arrow;
		m_right_arrow = m_arrow;
		for (int i = 0; i < 10; i++)
		{
			m_lock_textures.Add(m_room.m_CL.LoadTexture(m_room.m_content_path + "Lock puzzle/000" + i));
		}
		m_lock_numbers = new LockNumber[3];
		m_lock_numbers[0].m_number = 7;
		m_lock_numbers[0].m_pos.X = 659f;
		m_lock_numbers[0].m_pos.Y = 325f;
		m_lock_numbers[1].m_number = 4;
		m_lock_numbers[1].m_pos.X = 659f;
		m_lock_numbers[1].m_pos.Y = 413f;
		m_lock_numbers[2].m_number = 8;
		m_lock_numbers[2].m_pos.X = 659f;
		m_lock_numbers[2].m_pos.Y = 503f;
		UpdateLockNumbers();
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.CANCEL;
	}

	public override void Clear()
	{
		m_tick_sound = null;
		m_unlock_sound = null;
		for (int i = 0; i < m_lock_textures.Count; i++)
		{
			if (m_lock_textures[i] != null)
			{
				((GraphicsResource)m_lock_textures[i]).Dispose();
				m_lock_textures[i] = null;
			}
		}
		m_lock_textures.Clear();
		m_lock_textures = null;
		m_left_arrow = null;
		m_right_arrow = null;
		if (m_arrow != null)
		{
			((GraphicsResource)m_arrow).Dispose();
			m_arrow = null;
		}
		if (m_arrow_green != null)
		{
			((GraphicsResource)m_arrow_green).Dispose();
			m_arrow_green = null;
		}
		if (m_RT != null)
		{
			((RenderTarget)m_RT).Dispose();
			m_RT = null;
		}
		base.Clear();
	}

	public override void Reset()
	{
		base.Reset();
		m_game.m_input_enabled = false;
		m_game.m_inventory_enabled = false;
		m_game.m_show_cursor = false;
		m_game.m_game_data.m_view = "View1_1";
		if (m_game.m_inventory.m_state != global::Game.Inventory.Inventory.INVENTORY_STATE.DISABLED)
		{
			m_game.m_inventory.m_state = global::Game.Inventory.Inventory.INVENTORY_STATE.DISABLED;
		}
		m_update_puzzle = true;
	}

	public override void Setup()
	{
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK);
	}

	private void UpdateLockNumbers()
	{
		for (int i = 0; i < m_lock_numbers.Length; i++)
		{
			m_lock_numbers[i].m_texture = m_lock_textures[m_lock_numbers[i].m_number];
		}
		if (m_lock_numbers[0].m_number == 5 && m_lock_numbers[1].m_number == 2 && m_lock_numbers[2].m_number == 2)
		{
			m_game.m_game_data.SetState(m_room.m_name + ".Door01", "Unlocked");
			m_game.HandleEvent("Door01_Unlocked");
			m_game.PlaySound(m_unlock_sound, 0.3f);
			m_game.ActivateTrigger(m_back_trigger);
		}
	}

	public override void HandleEvent(string s_event)
	{
		base.HandleEvent(s_event);
	}

	private void Leave()
	{
		m_game.m_input_enabled = true;
		m_game.m_inventory_enabled = true;
		m_update_puzzle = false;
		m_game.ActivateTrigger(m_back_trigger);
	}

	public override void Update(TimeSpan elapsed)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Invalid comparison between Unknown and I4
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Invalid comparison between Unknown and I4
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Invalid comparison between Unknown and I4
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Invalid comparison between Unknown and I4
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Invalid comparison between Unknown and I4
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		base.Update(elapsed);
		if (!m_update_puzzle)
		{
			return;
		}
		m_game.m_input_enabled = false;
		m_game.m_show_cursor = false;
		m_game.m_update_cursor = false;
		KeyboardState state = Keyboard.GetState();
		GamePadState state2 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadButtons buttons = ((GamePadState)(ref state2)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
		{
			if (!m_game.m_b_pressed)
			{
				m_game.m_b_pressed = true;
				m_game.ActivateTrigger(m_back_trigger);
			}
		}
		else
		{
			m_game.m_b_pressed = false;
		}
		GamePadState state3 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad = ((GamePadState)(ref state3)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Right != 1)
		{
			GamePadState state4 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref state4)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks)).Left.X >= 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)102))
			{
				m_right_arrow = m_arrow;
				m_game.m_d_right_pressed = false;
				goto IL_019b;
			}
		}
		m_right_arrow = m_arrow_green;
		if (!m_game.m_d_right_pressed)
		{
			m_game.PlaySound(m_tick_sound, 0.2f);
			m_game.m_d_right_pressed = true;
			m_lock_numbers[m_current_lock_index].m_number++;
			if (m_lock_numbers[m_current_lock_index].m_number > 9)
			{
				m_lock_numbers[m_current_lock_index].m_number = 0;
			}
			UpdateLockNumbers();
		}
		goto IL_019b;
		IL_033a:
		GamePadState state5 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad2 = ((GamePadState)(ref state5)).DPad;
		if ((int)((GamePadDPad)(ref dPad2)).Down != 1)
		{
			GamePadState state6 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref state6)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks2)).Left.Y <= -0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)98))
			{
				m_game.m_d_down_pressed = false;
				return;
			}
		}
		if (!m_game.m_d_down_pressed)
		{
			m_game.m_d_down_pressed = true;
			m_current_lock_index++;
			if (m_current_lock_index > 2)
			{
				m_current_lock_index = 0;
			}
			UpdateLockNumbers();
		}
		return;
		IL_019b:
		GamePadState state7 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad3 = ((GamePadState)(ref state7)).DPad;
		if ((int)((GamePadDPad)(ref dPad3)).Left != 1)
		{
			GamePadState state8 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref state8)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks3)).Left.X <= -0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)100))
			{
				m_left_arrow = m_arrow;
				m_game.m_d_left_pressed = false;
				goto IL_029d;
			}
		}
		m_left_arrow = m_arrow_green;
		if (!m_game.m_d_left_pressed)
		{
			m_game.PlaySound(m_tick_sound, 0.2f);
			m_game.m_d_left_pressed = true;
			m_lock_numbers[m_current_lock_index].m_number--;
			if (m_lock_numbers[m_current_lock_index].m_number < 0)
			{
				m_lock_numbers[m_current_lock_index].m_number = 9;
			}
			UpdateLockNumbers();
		}
		goto IL_029d;
		IL_029d:
		GamePadState state9 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad4 = ((GamePadState)(ref state9)).DPad;
		if ((int)((GamePadDPad)(ref dPad4)).Up != 1)
		{
			GamePadState state10 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref state10)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks4)).Left.Y >= 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)104))
			{
				m_game.m_d_up_pressed = false;
				goto IL_033a;
			}
		}
		if (!m_game.m_d_up_pressed)
		{
			m_game.m_d_up_pressed = true;
			m_current_lock_index--;
			if (m_current_lock_index < 0)
			{
				m_current_lock_index = 2;
			}
			UpdateLockNumbers();
		}
		goto IL_033a;
	}

	public override void Draw(SpriteBatch SB)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		base.Draw(SB);
		if (m_update_puzzle)
		{
			SB.Begin((SpriteBlendMode)1);
			for (int i = 0; i < m_lock_numbers.Length; i++)
			{
				SB.Draw(m_lock_numbers[i].m_texture, m_lock_numbers[i].m_pos, Color.White);
			}
			switch (m_current_lock_index)
			{
			case 0:
				SB.Draw(m_left_arrow, new Rectangle(659, 331, m_arrow.Width, m_arrow.Height), (Rectangle?)null, Color.White, 0f, Vector2.Zero, (SpriteEffects)1, 0f);
				SB.Draw(m_right_arrow, new Vector2(790f, 331f), Color.White);
				break;
			case 1:
				SB.Draw(m_left_arrow, new Rectangle(659, 419, m_arrow.Width, m_arrow.Height), (Rectangle?)null, Color.White, 0f, Vector2.Zero, (SpriteEffects)1, 0f);
				SB.Draw(m_right_arrow, new Vector2(790f, 419f), Color.White);
				break;
			case 2:
				SB.Draw(m_left_arrow, new Rectangle(659, 509, m_arrow.Width, m_arrow.Height), (Rectangle?)null, Color.White, 0f, Vector2.Zero, (SpriteEffects)1, 0f);
				SB.Draw(m_right_arrow, new Vector2(790f, 509f), Color.White);
				break;
			}
			SB.End();
		}
	}
}
