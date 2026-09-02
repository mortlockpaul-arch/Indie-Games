using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game.World.Views.Room3;

internal class View2_1 : View
{
	private enum GAME_STATE
	{
		WAIT,
		AI_TURN,
		FADE_IN_AI_MARKER,
		PLAYER_TURN,
		FADE_IN_PLAYER_MARKER
	}

	private class Marker
	{
		public Vector2 m_pos;

		public Texture2D m_texture;

		public Color m_color;

		public float m_alpha;

		public Marker(float x, float y)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			m_pos = Vector2.Zero;
			m_color = Color.TransparentWhite;
			base._002Ector();
			m_pos.X = x;
			m_pos.Y = y;
		}

		public void Clear()
		{
			if (m_texture != null)
			{
				((GraphicsResource)m_texture).Dispose();
				m_texture = null;
			}
		}

		public void Draw(SpriteBatch SB)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			if (m_texture != null)
			{
				SB.Draw(m_texture, new Vector2(m_pos.X - (float)(m_texture.Width / 2), m_pos.Y - (float)(m_texture.Height / 2)), m_color);
			}
		}
	}

	private SoundEffect m_loose_sound;

	private Texture2D m_o_white;

	private Animation2D m_effect_anim;

	private bool m_show_effect;

	private GAME_STATE m_game_state = GAME_STATE.AI_TURN;

	private bool m_game_enabled;

	private byte[][] m_board;

	private byte m_AI_marker;

	private byte m_player_marker;

	private Texture2D m_o;

	private Texture2D m_x;

	private int m_wins;

	private List<Marker> m_markers = new List<Marker>(9);

	private Texture2D m_game_info;

	public View2_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View2_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "table_zoom")));
		m_o_white = m_room.m_CL.LoadTexture(m_room.m_content_path + "Effect/o_white_effect3");
		m_effect_anim = new AlphaAnimation(m_game, 25u, reverse: false, m_o_white);
		m_loose_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/rightroom_trycka_pa_tavla");
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
		m_o = m_room.m_CL.LoadTexture(m_room.m_content_path + "table_o");
		m_x = m_room.m_CL.LoadTexture(m_room.m_content_path + "table_x");
		m_board = new byte[3][];
		for (int i = 0; i < m_board.Length; i++)
		{
			m_board[i] = new byte[3];
			for (int j = 0; j < m_board[i].Length; j++)
			{
				m_board[i][j] = 0;
			}
		}
		m_game_info = m_room.m_CL.LoadTexture(m_room.m_content_path + "table_zoom_text");
		m_markers.Add(new Marker(502f, 266f));
		m_markers.Add(new Marker(647f, 263f));
		m_markers.Add(new Marker(801f, 266f));
		m_markers.Add(new Marker(502f, 364f));
		m_markers.Add(new Marker(653f, 367f));
		m_markers.Add(new Marker(798f, 364f));
		m_markers.Add(new Marker(506f, 471f));
		m_markers.Add(new Marker(657f, 473f));
		m_markers.Add(new Marker(798f, 473f));
	}

	public override void Clear()
	{
		m_loose_sound = null;
		m_game_info = null;
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
		if (m_board != null)
		{
			for (int i = 0; i < m_board.Length; i++)
			{
				m_board[i] = null;
			}
		}
		m_board = null;
		if (m_markers != null)
		{
			for (int j = 0; j < m_markers.Count; j++)
			{
				if (m_markers[j] != null)
				{
					m_markers[j].Clear();
					m_markers[j] = null;
				}
			}
		}
		m_markers.Clear();
		m_markers = null;
		if (m_o != null)
		{
			((GraphicsResource)m_o).Dispose();
			m_o = null;
		}
		if (m_x != null)
		{
			((GraphicsResource)m_x).Dispose();
			m_x = null;
		}
		base.Clear();
	}

	public override void Setup()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View2"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK);
		if (m_game.m_game_data.GetState("Room3.PuzzleCompleted") == "1")
		{
			HandlePuzzleCompleted();
		}
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, new Rectangle(225, 114, 276, 152), "View2_1.Square1", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(294, 112, 355, 150), "View2_1.Square2", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(372, 110, 430, 148), "View2_1.Square3", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(225, 162, 280, 199), "View2_1.Square4", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(294, 160, 355, 200), "View2_1.Square5", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(371, 161, 425, 201), "View2_1.Square6", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(224, 214, 280, 254), "View2_1.Square7", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(294, 213, 360, 252), "View2_1.Square8", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(373, 213, 423, 255), "View2_1.Square9", Trigger.TRIGGER_TYPE.UNKNOWN);
		cursorTrigger.m_activate_own = true;
		cursorTrigger.m_enabled = false;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(215, 100, 460, 270), "View2_1.Board", Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
	}

	public override void Reset()
	{
		base.Reset();
		m_game.m_show_cursor = true;
		if (!m_game_enabled)
		{
			return;
		}
		m_game.m_game_data.m_view = "View2";
		m_game.m_cursor.m_state = Cursor.CURSOR_STATE.IDLE;
		m_game.m_update_cursor = false;
		m_game.m_inventory_enabled = false;
		m_back_trigger.m_enabled = false;
		for (int i = 0; i < 9; i++)
		{
			if (m_triggers[i] != null)
			{
				m_triggers[i].m_enabled = true;
			}
		}
		if (m_triggers[9] != null)
		{
			m_triggers[9].m_enabled = false;
		}
	}

	private void HandlePuzzleCompleted()
	{
		m_game_enabled = true;
		m_back_trigger.m_enabled = false;
		m_hud_state = HUD.HUD_STATE.NONE;
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "View2_1.onZoom":
			if (m_game_enabled)
			{
				m_game.m_show_cursor = false;
				m_game.m_inventory_enabled = false;
			}
			break;
		case "Room3.PuzzleCompleted":
			HandlePuzzleCompleted();
			break;
		case "View2_1.Square1":
			Console.WriteLine("Square 1");
			break;
		case "View2_1.Square2":
			Console.WriteLine("Square 2");
			break;
		case "View2_1.Square3":
			Console.WriteLine("Square 3");
			break;
		case "View2_1.Square4":
			Console.WriteLine("Square 4");
			break;
		case "View2_1.Square5":
			Console.WriteLine("Square 5");
			break;
		case "View2_1.Square6":
			Console.WriteLine("Square 6");
			break;
		case "View2_1.Square7":
			Console.WriteLine("Square 7");
			break;
		case "View2_1.Square8":
			Console.WriteLine("Square 8");
			break;
		case "View2_1.Square9":
			Console.WriteLine("Square 9");
			break;
		case "View2_1.onWin1":
		case "View2_1.onWin2":
		case "View2_1.onWin3":
			m_back_trigger.m_enabled = true;
			m_game.ActivateTrigger(m_back_trigger);
			m_game.m_show_cursor = false;
			break;
		case "View2_1.Board":
			m_game.m_hud.ShowText("These squares are carved into the table and then painted with ... blood?", m_use_text_fade);
			break;
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Invalid comparison between Unknown and I4
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Invalid comparison between Unknown and I4
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Invalid comparison between Unknown and I4
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Invalid comparison between Unknown and I4
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Invalid comparison between Unknown and I4
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		base.Update(elapsed);
		if (!m_game_enabled)
		{
			return;
		}
		KeyboardState state;
		GamePadState state4;
		GamePadDPad dPad2;
		GamePadState state6;
		GamePadDPad dPad3;
		GamePadState state8;
		GamePadButtons buttons;
		GamePadState state9;
		GamePadDPad dPad4;
		switch (m_game_state)
		{
		case GAME_STATE.AI_TURN:
			CalcAIMarker();
			break;
		case GAME_STATE.FADE_IN_AI_MARKER:
			if (m_markers[m_AI_marker] != null)
			{
				m_markers[m_AI_marker].m_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 200f;
				if (m_markers[m_AI_marker].m_alpha >= 255f)
				{
					m_markers[m_AI_marker].m_alpha = 255f;
					m_game.m_cursor.m_pos = m_markers[m_player_marker].m_pos;
					m_game.m_show_cursor = true;
					m_game_state = GAME_STATE.PLAYER_TURN;
					CheckBoard();
				}
				((Color)(ref m_markers[m_AI_marker].m_color)).A = (byte)Math.Round(m_markers[m_AI_marker].m_alpha);
			}
			break;
		case GAME_STATE.FADE_IN_PLAYER_MARKER:
			if (m_markers[m_player_marker] != null)
			{
				m_markers[m_player_marker].m_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 200f;
				if (m_markers[m_player_marker].m_alpha >= 255f)
				{
					m_markers[m_player_marker].m_alpha = 255f;
					m_game_state = GAME_STATE.AI_TURN;
					CheckBoard();
				}
				((Color)(ref m_markers[m_player_marker].m_color)).A = (byte)Math.Round(m_markers[m_player_marker].m_alpha);
			}
			break;
		case GAME_STATE.PLAYER_TURN:
			{
				state = Keyboard.GetState();
				m_game.m_input_enabled = false;
				GamePadState state2 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadDPad dPad = ((GamePadState)(ref state2)).DPad;
				if ((int)((GamePadDPad)(ref dPad)).Left != 1)
				{
					GamePadState state3 = GamePad.GetState(Game.PLAYER_INDEX);
					GamePadThumbSticks thumbSticks = ((GamePadState)(ref state3)).ThumbSticks;
					if (!(((GamePadThumbSticks)(ref thumbSticks)).Left.X <= -0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)37))
					{
						m_game.m_d_left_pressed = false;
						goto IL_02cb;
					}
				}
				if (!m_game.m_d_left_pressed)
				{
					m_game.m_d_left_pressed = true;
					if (m_player_marker != 0 && m_player_marker != 3 && m_player_marker != 6)
					{
						m_player_marker--;
						m_game.m_cursor.m_pos = m_markers[m_player_marker].m_pos;
					}
				}
				goto IL_02cb;
			}
			IL_02cb:
			state4 = GamePad.GetState(Game.PLAYER_INDEX);
			dPad2 = ((GamePadState)(ref state4)).DPad;
			if ((int)((GamePadDPad)(ref dPad2)).Right != 1)
			{
				GamePadState state5 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref state5)).ThumbSticks;
				if (!(((GamePadThumbSticks)(ref thumbSticks2)).Left.X >= 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)39))
				{
					m_game.m_d_right_pressed = false;
					goto IL_0394;
				}
			}
			if (!m_game.m_d_right_pressed)
			{
				m_game.m_d_right_pressed = true;
				if (m_player_marker != 2 && m_player_marker != 5 && m_player_marker != 8)
				{
					m_player_marker++;
					m_game.m_cursor.m_pos = m_markers[m_player_marker].m_pos;
				}
			}
			goto IL_0394;
			IL_0394:
			state6 = GamePad.GetState(Game.PLAYER_INDEX);
			dPad3 = ((GamePadState)(ref state6)).DPad;
			if ((int)((GamePadDPad)(ref dPad3)).Up != 1)
			{
				GamePadState state7 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref state7)).ThumbSticks;
				if (!(((GamePadThumbSticks)(ref thumbSticks3)).Left.Y >= 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)38))
				{
					m_game.m_d_up_pressed = false;
					goto IL_044b;
				}
			}
			if (!m_game.m_d_up_pressed)
			{
				m_game.m_d_up_pressed = true;
				if (m_player_marker >= 3)
				{
					m_player_marker -= 3;
					m_game.m_cursor.m_pos = m_markers[m_player_marker].m_pos;
				}
			}
			goto IL_044b;
			IL_0502:
			state8 = GamePad.GetState(Game.PLAYER_INDEX);
			buttons = ((GamePadState)(ref state8)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)65))
			{
				if (!m_game.m_a_pressed)
				{
					m_game.m_a_pressed = true;
					if (m_markers[m_player_marker].m_texture == null)
					{
						m_markers[m_player_marker].m_alpha = 0f;
						m_markers[m_player_marker].m_texture = m_x;
						m_game.m_show_cursor = false;
						UpdateBoard(m_player_marker, 2);
						m_game_state = GAME_STATE.FADE_IN_PLAYER_MARKER;
					}
				}
			}
			else
			{
				m_game.m_a_pressed = false;
			}
			break;
			IL_044b:
			state9 = GamePad.GetState(Game.PLAYER_INDEX);
			dPad4 = ((GamePadState)(ref state9)).DPad;
			if ((int)((GamePadDPad)(ref dPad4)).Down != 1)
			{
				GamePadState state10 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref state10)).ThumbSticks;
				if (!(((GamePadThumbSticks)(ref thumbSticks4)).Left.Y <= -0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)40))
				{
					m_game.m_d_down_pressed = false;
					goto IL_0502;
				}
			}
			if (!m_game.m_d_down_pressed)
			{
				m_game.m_d_down_pressed = true;
				if (m_player_marker <= 5)
				{
					m_player_marker += 3;
					m_game.m_cursor.m_pos = m_markers[m_player_marker].m_pos;
				}
			}
			goto IL_0502;
		}
	}

	private void CheckBoard()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (CheckRows(1))
		{
			m_wins = 0;
			m_game.m_update_cursor = true;
			ResetBoard();
			m_game.m_game_data.SetState("Room3.PuzzleCompleted", "0");
			m_game.m_play_door_sound = false;
			m_room.HandleEvent("View2_1.TicTacLost");
			m_show_effect = true;
			m_effect_anim.Play();
			m_game.m_input_enabled = false;
			GamePad.SetVibration(Game.PLAYER_INDEX, 1f, 1f);
			m_game.PlaySound(m_loose_sound, 0.5f);
			m_game_state = GAME_STATE.WAIT;
			m_game.m_show_cursor = false;
			return;
		}
		bool flag = true;
		for (int i = 0; i < m_board.Length; i++)
		{
			for (int j = 0; j < m_board[i].Length; j++)
			{
				if (m_board[i][j] == 0)
				{
					flag = false;
				}
			}
		}
		if (CheckRows(2) || flag)
		{
			Console.WriteLine("Player won!");
			m_wins++;
			m_game_state = GAME_STATE.WAIT;
			if (m_wins == 1)
			{
				m_game.HandleEvent("View2_1.onWin1");
			}
			else if (m_wins == 2)
			{
				m_game.HandleEvent("View2_1.onWin2");
			}
			else if (m_wins == 3)
			{
				m_game.m_game_data.SetState("Room3.TicTacCompleted", "1");
				m_game.HandleEvent("View2_1.onWin3");
			}
		}
	}

	private void CalcAIMarker()
	{
		bool flag = true;
		for (int i = 0; i < m_board.Length; i++)
		{
			for (int j = 0; j < m_board[i].Length; j++)
			{
				if (m_board[i][j] != 0)
				{
					flag = false;
				}
			}
		}
		if (flag)
		{
			SetAIMarker((byte)m_game.GetRandom(0, 8));
			return;
		}
		int num = CheckPossibleRows(1);
		if (num != -1)
		{
			SetAIMarker((byte)num);
			return;
		}
		num = CheckPossibleRows(2);
		if (num != -1)
		{
			SetAIMarker((byte)num);
			return;
		}
		for (int k = 0; k < m_markers.Count; k++)
		{
			if (m_markers[k] != null && m_markers[k].m_texture == null)
			{
				m_AI_marker = (byte)k;
				SetAIMarker(m_AI_marker);
				break;
			}
		}
	}

	private int CheckPossibleRows(byte player)
	{
		int num = 0;
		int num2 = -1;
		if (m_board[0][0] == player)
		{
			num++;
		}
		else if (m_board[0][0] == 0)
		{
			num2 = 0;
		}
		if (m_board[0][1] == player)
		{
			num++;
		}
		else if (m_board[0][1] == 0)
		{
			num2 = 1;
		}
		if (m_board[0][2] == player)
		{
			num++;
		}
		else if (m_board[0][2] == 0)
		{
			num2 = 2;
		}
		if (num >= 2 && num2 != -1)
		{
			return num2;
		}
		num = 0;
		num2 = -1;
		if (m_board[1][0] == player)
		{
			num++;
		}
		else if (m_board[1][0] == 0)
		{
			num2 = 3;
		}
		if (m_board[1][1] == player)
		{
			num++;
		}
		else if (m_board[1][1] == 0)
		{
			num2 = 4;
		}
		if (m_board[1][2] == player)
		{
			num++;
		}
		else if (m_board[1][2] == 0)
		{
			num2 = 5;
		}
		if (num >= 2 && num2 != -1)
		{
			return num2;
		}
		num = 0;
		num2 = -1;
		if (m_board[2][0] == player)
		{
			num++;
		}
		else if (m_board[2][0] == 0)
		{
			num2 = 6;
		}
		if (m_board[2][1] == player)
		{
			num++;
		}
		else if (m_board[2][1] == 0)
		{
			num2 = 7;
		}
		if (m_board[2][2] == player)
		{
			num++;
		}
		else if (m_board[2][2] == 0)
		{
			num2 = 8;
		}
		if (num >= 2 && num2 != -1)
		{
			return num2;
		}
		num = 0;
		num2 = -1;
		if (m_board[0][0] == player)
		{
			num++;
		}
		else if (m_board[0][0] == 0)
		{
			num2 = 0;
		}
		if (m_board[1][0] == player)
		{
			num++;
		}
		else if (m_board[1][0] == 0)
		{
			num2 = 3;
		}
		if (m_board[2][0] == player)
		{
			num++;
		}
		else if (m_board[2][0] == 0)
		{
			num2 = 6;
		}
		if (num >= 2 && num2 != -1)
		{
			return num2;
		}
		num = 0;
		num2 = -1;
		if (m_board[0][1] == player)
		{
			num++;
		}
		else if (m_board[0][1] == 0)
		{
			num2 = 1;
		}
		if (m_board[1][1] == player)
		{
			num++;
		}
		else if (m_board[1][1] == 0)
		{
			num2 = 4;
		}
		if (m_board[2][1] == player)
		{
			num++;
		}
		else if (m_board[2][1] == 0)
		{
			num2 = 7;
		}
		if (num >= 2 && num2 != -1)
		{
			return num2;
		}
		num = 0;
		num2 = -1;
		if (m_board[0][2] == player)
		{
			num++;
		}
		else if (m_board[0][2] == 0)
		{
			num2 = 2;
		}
		if (m_board[1][2] == player)
		{
			num++;
		}
		else if (m_board[1][2] == 0)
		{
			num2 = 5;
		}
		if (m_board[2][2] == player)
		{
			num++;
		}
		else if (m_board[2][2] == 0)
		{
			num2 = 8;
		}
		if (num >= 2 && num2 != -1)
		{
			return num2;
		}
		num = 0;
		num2 = -1;
		if (m_board[0][0] == player)
		{
			num++;
		}
		else if (m_board[0][0] == 0)
		{
			num2 = 0;
		}
		if (m_board[1][1] == player)
		{
			num++;
		}
		else if (m_board[1][1] == 0)
		{
			num2 = 4;
		}
		if (m_board[2][2] == player)
		{
			num++;
		}
		else if (m_board[2][2] == 0)
		{
			num2 = 8;
		}
		if (num >= 2 && num2 != -1)
		{
			return num2;
		}
		num = 0;
		num2 = -1;
		if (m_board[0][2] == player)
		{
			num++;
		}
		else if (m_board[0][2] == 0)
		{
			num2 = 2;
		}
		if (m_board[1][1] == player)
		{
			num++;
		}
		else if (m_board[1][1] == 0)
		{
			num2 = 4;
		}
		if (m_board[2][0] == player)
		{
			num++;
		}
		else if (m_board[2][0] == 0)
		{
			num2 = 6;
		}
		if (num >= 2 && num2 != -1)
		{
			return num2;
		}
		return -1;
	}

	private bool CheckRows(byte player)
	{
		if (m_board[0][0] == player && m_board[0][1] == player && m_board[0][2] == player)
		{
			return true;
		}
		if (m_board[1][0] == player && m_board[1][1] == player && m_board[1][2] == player)
		{
			return true;
		}
		if (m_board[2][0] == player && m_board[2][1] == player && m_board[2][2] == player)
		{
			return true;
		}
		if (m_board[0][0] == player && m_board[1][0] == player && m_board[2][0] == player)
		{
			return true;
		}
		if (m_board[0][1] == player && m_board[1][1] == player && m_board[2][1] == player)
		{
			return true;
		}
		if (m_board[0][2] == player && m_board[1][2] == player && m_board[2][2] == player)
		{
			return true;
		}
		if (m_board[0][0] == player && m_board[1][1] == player && m_board[2][2] == player)
		{
			return true;
		}
		if (m_board[0][2] == player && m_board[1][1] == player && m_board[2][0] == player)
		{
			return true;
		}
		return false;
	}

	private void SetAIMarker(byte marker)
	{
		m_AI_marker = marker;
		m_markers[m_AI_marker].m_alpha = 0f;
		m_markers[m_AI_marker].m_texture = m_o;
		UpdateBoard(m_AI_marker, 1);
		m_game_state = GAME_STATE.FADE_IN_AI_MARKER;
	}

	private void UpdateBoard(int square, byte type)
	{
		switch (square)
		{
		case 0:
			m_board[0][0] = type;
			break;
		case 1:
			m_board[0][1] = type;
			break;
		case 2:
			m_board[0][2] = type;
			break;
		case 3:
			m_board[1][0] = type;
			break;
		case 4:
			m_board[1][1] = type;
			break;
		case 5:
			m_board[1][2] = type;
			break;
		case 6:
			m_board[2][0] = type;
			break;
		case 7:
			m_board[2][1] = type;
			break;
		case 8:
			m_board[2][2] = type;
			break;
		}
	}

	public override void Draw(SpriteBatch SB)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		base.Draw(SB);
		if (!m_game_enabled)
		{
			return;
		}
		SB.Begin((SpriteBlendMode)1);
		if (m_game_info != null)
		{
			SB.Draw(m_game_info, Game.VIEW_RECT, Color.White);
		}
		if (!m_show_effect)
		{
			for (int i = 0; i < m_markers.Count; i++)
			{
				if (m_markers[i] != null)
				{
					m_markers[i].Draw(SB);
				}
			}
		}
		SB.End();
	}

	public void ResetBoard()
	{
		for (int i = 0; i < m_board.Length; i++)
		{
			for (int j = 0; j < m_board[i].Length; j++)
			{
				m_board[i][j] = 0;
			}
		}
		if (m_markers != null)
		{
			for (int k = 0; k < m_markers.Count; k++)
			{
				if (m_markers[k] != null)
				{
					m_markers[k].m_texture = null;
					m_markers[k].m_alpha = 0f;
					((Color)(ref m_markers[k].m_color)).A = 0;
				}
			}
		}
		m_game_state = GAME_STATE.AI_TURN;
	}

	public override void UpdateEffect(TimeSpan elapsed)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateEffect(elapsed);
		if (m_show_effect && m_effect_anim != null)
		{
			m_effect_anim.Update(elapsed);
			if (m_effect_anim.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
			{
				m_show_effect = false;
				m_game.m_input_enabled = true;
				m_game.ActivateTrigger(m_back_trigger);
				GamePad.SetVibration(Game.PLAYER_INDEX, 0f, 0f);
				m_game.ChangeArea("Room3", "View1", door_sound: false);
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
