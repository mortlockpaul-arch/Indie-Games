using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game.World.Views.Room3;

internal class View1_1 : View
{
	private enum MOVE_STATE
	{
		MOVE_LEFT,
		ABORT,
		MOVE_RIGHT
	}

	private SoundEffect m_sound;

	private bool m_ask_move;

	private Texture2D m_fade;

	private SpriteFont m_font;

	private Animation2D m_shadow_anim;

	private bool m_show_shadow_anim;

	private float m_shadow_anim_timer = 1000f;

	private bool m_shadow_countdown;

	private MOVE_STATE m_move_state = MOVE_STATE.ABORT;

	private int m_selected_painting = 1;

	private int[] m_paintings;

	private Texture2D[] m_painting_textures;

	private Animation2D[] m_painting_fades;

	public View1_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "frames_zoom_wrong_positions")));
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "frames_zoom")));
		m_shadow_anim = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/Shadows/");
		m_shadow_anim.SetFPS(60.0);
		m_sound = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/rightroom_skuggor_forsvinner");
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.CANCEL;
		m_fade = m_room.m_CL.LoadTexture("HUD/black");
		m_font = m_room.m_CL.LoadFont("Fonts/SpriteFont2");
		m_painting_textures = (Texture2D[])(object)new Texture2D[4];
		m_painting_textures[0] = m_room.m_CL.LoadTexture(m_room.m_content_path + "frame_girl");
		m_painting_textures[1] = m_room.m_CL.LoadTexture(m_room.m_content_path + "frame_man");
		m_painting_textures[2] = m_room.m_CL.LoadTexture(m_room.m_content_path + "frame_kid");
		m_painting_textures[3] = m_room.m_CL.LoadTexture(m_room.m_content_path + "frame_boy");
		m_paintings = new int[4];
		m_paintings[0] = 3;
		m_paintings[1] = 2;
		m_paintings[2] = 1;
		m_paintings[3] = 0;
		m_painting_fades = new Animation2D[4];
		m_painting_fades[0] = null;
		m_painting_fades[1] = null;
		m_painting_fades[2] = null;
		m_painting_fades[3] = null;
	}

	public override void Clear()
	{
		m_sound = null;
		if (m_fade != null)
		{
			((GraphicsResource)m_fade).Dispose();
			m_fade = null;
		}
		if (m_shadow_anim != null)
		{
			m_shadow_anim.Clear();
			m_shadow_anim = null;
		}
		for (int i = 0; i < m_painting_textures.Length; i++)
		{
			if (m_painting_textures[i] != null)
			{
				((GraphicsResource)m_painting_textures[i]).Dispose();
				m_painting_textures[i] = null;
			}
		}
		m_painting_textures = null;
		m_paintings = null;
		for (int j = 0; j < m_painting_fades.Length; j++)
		{
			if (m_painting_fades[j] != null)
			{
				m_painting_fades[j].Clear();
				m_painting_fades[j] = null;
			}
		}
		m_painting_fades = null;
		m_font = null;
		base.Clear();
	}

	public override void Reset()
	{
		base.Reset();
		m_game.m_game_data.m_view = "View1";
	}

	public override void Setup()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, new Rectangle(89, 53, 194, 182), "View1_1.Painting1", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(216, 37, 317, 165), "View1_1.Painting2", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(328, 185, 433, 313), "View1_1.Painting3", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
		cursorTrigger = new CursorTrigger(m_game, new Rectangle(444, 78, 550, 207), "View1_1.Painting4", Trigger.TRIGGER_TYPE.USE);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "View1_1.onReset":
		{
			m_painting_textures[0] = m_room.m_CL.LoadTexture(m_room.m_content_path + "frame_girl");
			m_painting_textures[1] = m_room.m_CL.LoadTexture(m_room.m_content_path + "frame_man");
			m_painting_textures[2] = m_room.m_CL.LoadTexture(m_room.m_content_path + "frame_kid");
			m_painting_textures[3] = m_room.m_CL.LoadTexture(m_room.m_content_path + "frame_boy");
			m_paintings[0] = 3;
			m_paintings[1] = 2;
			m_paintings[2] = 1;
			m_paintings[3] = 0;
			for (int i = 0; i < m_painting_fades.Length; i++)
			{
				if (m_painting_fades[i] != null)
				{
					m_painting_fades[i].Clear();
					m_painting_fades[i] = null;
				}
			}
			break;
		}
		case "View1_1.Painting1":
			Console.WriteLine("Painting1");
			onPainting(0);
			break;
		case "View1_1.Painting2":
			Console.WriteLine("Painting2");
			onPainting(1);
			break;
		case "View1_1.Painting3":
			Console.WriteLine("Painting3");
			onPainting(2);
			break;
		case "View1_1.Painting4":
			Console.WriteLine("Painting4");
			onPainting(3);
			break;
		}
		base.HandleEvent(s_event);
	}

	private void onPainting(int painting)
	{
		m_ask_move = true;
		m_game.m_show_cursor = false;
		m_game.m_update_cursor = false;
		m_game.m_inventory_enabled = false;
		m_game.m_hud.m_state = HUD.HUD_STATE.NONE;
		m_move_state = MOVE_STATE.ABORT;
		m_selected_painting = painting;
	}

	public override void Update(TimeSpan elapsed)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Invalid comparison between Unknown and I4
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Invalid comparison between Unknown and I4
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Invalid comparison between Unknown and I4
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Invalid comparison between Unknown and I4
		base.Update(elapsed);
		if (m_shadow_countdown)
		{
			m_shadow_anim_timer -= (float)elapsed.TotalMilliseconds;
			if (m_shadow_anim_timer <= 0f)
			{
				m_shadow_countdown = false;
				m_show_shadow_anim = true;
				ChangeScene(1);
				m_shadow_anim.Play();
				m_game.PlaySound(m_sound, 0.5f);
				m_game.m_input_enabled = false;
				m_game.m_show_cursor = false;
				m_game.m_inventory_enabled = false;
			}
		}
		if (m_show_shadow_anim && m_shadow_anim != null)
		{
			m_shadow_anim.Update(elapsed);
			if (m_shadow_anim.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
			{
				m_show_shadow_anim = false;
				m_game.ActivateTrigger(m_back_trigger);
			}
		}
		KeyboardState state = Keyboard.GetState();
		for (int i = 0; i < m_painting_fades.Length; i++)
		{
			if (m_painting_fades[i] != null)
			{
				m_painting_fades[i].Update(elapsed);
			}
		}
		if (m_ask_move)
		{
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
					goto IL_01c5;
				}
			}
			if (!m_game.m_d_left_pressed)
			{
				m_game.m_d_left_pressed = true;
				if (m_selected_painting != 0)
				{
					if (m_move_state != MOVE_STATE.MOVE_LEFT)
					{
						m_move_state--;
					}
				}
				else if (m_move_state != MOVE_STATE.ABORT)
				{
					m_move_state--;
				}
			}
			goto IL_01c5;
		}
		GamePadState state4 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadButtons buttons = ((GamePadState)(ref state4)).Buttons;
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
		return;
		IL_0277:
		GamePadState state5 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadButtons buttons2 = ((GamePadState)(ref state5)).Buttons;
		if ((int)((GamePadButtons)(ref buttons2)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)65))
		{
			if (m_game.m_a_pressed)
			{
				return;
			}
			m_game.m_a_pressed = true;
			m_ask_move = false;
			m_game.m_input_enabled = true;
			m_game.m_show_cursor = true;
			m_game.m_update_cursor = true;
			m_game.m_inventory_enabled = true;
			m_game.m_hud.m_state = HUD.HUD_STATE.CANCEL;
			switch (m_move_state)
			{
			case MOVE_STATE.MOVE_RIGHT:
			{
				if (m_painting_fades[m_selected_painting] != null)
				{
					m_painting_fades[m_selected_painting].Clear();
					m_painting_fades[m_selected_painting] = null;
				}
				if (m_painting_fades[m_selected_painting + 1] != null)
				{
					m_painting_fades[m_selected_painting + 1].Clear();
					m_painting_fades[m_selected_painting + 1] = null;
				}
				int num3 = m_paintings[m_selected_painting];
				int num4 = m_paintings[m_selected_painting + 1];
				m_paintings[m_selected_painting] = num4;
				m_paintings[m_selected_painting + 1] = num3;
				m_painting_fades[m_selected_painting] = new AlphaAnimation(m_game, 25u, reverse: false, m_painting_textures[m_paintings[m_selected_painting + 1]]);
				m_painting_fades[m_selected_painting].Play();
				m_painting_fades[m_selected_painting + 1] = new AlphaAnimation(m_game, 25u, reverse: false, m_painting_textures[m_paintings[m_selected_painting]]);
				m_painting_fades[m_selected_painting + 1].Play();
				CheckPositions();
				break;
			}
			case MOVE_STATE.MOVE_LEFT:
			{
				if (m_painting_fades[m_selected_painting] != null)
				{
					m_painting_fades[m_selected_painting].Clear();
					m_painting_fades[m_selected_painting] = null;
				}
				if (m_painting_fades[m_selected_painting - 1] != null)
				{
					m_painting_fades[m_selected_painting - 1].Clear();
					m_painting_fades[m_selected_painting - 1] = null;
				}
				int num = m_paintings[m_selected_painting];
				int num2 = m_paintings[m_selected_painting - 1];
				m_paintings[m_selected_painting] = num2;
				m_paintings[m_selected_painting - 1] = num;
				m_painting_fades[m_selected_painting] = new AlphaAnimation(m_game, 25u, reverse: false, m_painting_textures[m_paintings[m_selected_painting - 1]]);
				m_painting_fades[m_selected_painting].Play();
				m_painting_fades[m_selected_painting - 1] = new AlphaAnimation(m_game, 25u, reverse: false, m_painting_textures[m_paintings[m_selected_painting]]);
				m_painting_fades[m_selected_painting - 1].Play();
				CheckPositions();
				break;
			}
			case MOVE_STATE.ABORT:
				break;
			}
		}
		else
		{
			m_game.m_a_pressed = false;
		}
		return;
		IL_01c5:
		GamePadState state6 = GamePad.GetState(Game.PLAYER_INDEX);
		GamePadDPad dPad2 = ((GamePadState)(ref state6)).DPad;
		if ((int)((GamePadDPad)(ref dPad2)).Right != 1)
		{
			GamePadState state7 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref state7)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks2)).Left.X >= 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)39))
			{
				m_game.m_d_right_pressed = false;
				goto IL_0277;
			}
		}
		if (!m_game.m_d_right_pressed)
		{
			m_game.m_d_right_pressed = true;
			if (m_selected_painting != 3)
			{
				if (m_move_state != MOVE_STATE.MOVE_RIGHT)
				{
					m_move_state++;
				}
			}
			else if (m_move_state != MOVE_STATE.ABORT)
			{
				m_move_state++;
			}
		}
		goto IL_0277;
	}

	private void CheckPositions()
	{
		if (m_paintings[0] != 0 || m_paintings[1] != 1 || m_paintings[2] != 2 || m_paintings[3] != 3)
		{
			return;
		}
		m_shadow_countdown = true;
		m_shadow_anim_timer = 1000f;
		for (int i = 0; i < m_triggers.Count; i++)
		{
			if (m_triggers[i] != null)
			{
				m_triggers[i].m_enabled = false;
			}
		}
		m_game.m_cursor.onOut();
		m_game.m_game_data.SetState("Room3.PuzzleCompleted", "1");
		m_game.HandleEvent("Room3.PuzzleCompleted");
		m_game.m_hud.m_state = HUD.HUD_STATE.NONE;
	}

	public override void Draw(SpriteBatch SB)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		base.Draw(SB);
		if (m_show_shadow_anim && m_shadow_anim != null)
		{
			m_shadow_anim.Draw(SB);
		}
		SB.Begin((SpriteBlendMode)1);
		SB.Draw(m_painting_textures[m_paintings[0]], new Vector2(178f, 107f), Color.White);
		SB.Draw(m_painting_textures[m_paintings[1]], new Vector2(432f, 74f), Color.White);
		SB.Draw(m_painting_textures[m_paintings[2]], new Vector2(656f, 370f), Color.White);
		SB.Draw(m_painting_textures[m_paintings[3]], new Vector2(889f, 157f), Color.White);
		SB.End();
		if (m_painting_fades[0] != null && m_painting_fades[0].m_state == Animation2D.ANIM_STATE.ANIM_STATE_PLAYING)
		{
			m_painting_fades[0].Draw(SB, new Vector2(178f, 107f));
		}
		if (m_painting_fades[1] != null && m_painting_fades[1].m_state == Animation2D.ANIM_STATE.ANIM_STATE_PLAYING)
		{
			m_painting_fades[1].Draw(SB, new Vector2(432f, 74f));
		}
		if (m_painting_fades[2] != null && m_painting_fades[2].m_state == Animation2D.ANIM_STATE.ANIM_STATE_PLAYING)
		{
			m_painting_fades[2].Draw(SB, new Vector2(656f, 370f));
		}
		if (m_painting_fades[3] != null && m_painting_fades[3].m_state == Animation2D.ANIM_STATE.ANIM_STATE_PLAYING)
		{
			m_painting_fades[3].Draw(SB, new Vector2(889f, 157f));
		}
		if (m_ask_move)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_fade, Game.VIEW_RECT, new Color((byte)0, (byte)0, (byte)0, (byte)128));
			string text = "Move Left";
			string text2 = "Abort";
			string text3 = "Move Right";
			Vector2 val = m_font.MeasureString(text);
			Vector2 val2 = m_font.MeasureString(text2);
			Vector2 val3 = m_font.MeasureString(text3);
			float num = 20f;
			Vector2 val4 = default(Vector2);
			((Vector2)(ref val4))._002Ector(((float)Game.VIEW_RECT.Width - (val.X + num + val2.X + num + val3.X)) / 2f, (float)(Game.VIEW_RECT.Height - 100));
			if (m_move_state == MOVE_STATE.MOVE_LEFT)
			{
				SB.DrawString(m_font, text, val4, Color.Red);
			}
			else if (m_selected_painting != 0)
			{
				SB.DrawString(m_font, text, val4, Color.White);
			}
			else
			{
				SB.DrawString(m_font, text, val4, Color.DimGray);
			}
			val4.X += val.X + num;
			if (m_move_state == MOVE_STATE.ABORT)
			{
				SB.DrawString(m_font, text2, val4, Color.Red);
			}
			else
			{
				SB.DrawString(m_font, text2, val4, Color.White);
			}
			val4.X += val2.X + num;
			if (m_move_state == MOVE_STATE.MOVE_RIGHT)
			{
				SB.DrawString(m_font, text3, val4, Color.Red);
			}
			else if (m_selected_painting != 3)
			{
				SB.DrawString(m_font, text3, val4, Color.White);
			}
			else
			{
				SB.DrawString(m_font, text3, val4, Color.DimGray);
			}
			SB.End();
		}
	}
}
