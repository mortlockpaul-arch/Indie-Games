using System;
using Game.World.Views.Room3;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Game.World.Areas;

public class Room3 : Area
{
	private class FilterLayer
	{
		public float m_filter_alpha;

		public int m_next_filter_alpha;

		private int m_filter_alpha_min = 16;

		private int m_filter_alpha_max = 24;

		private float m_filter_alpha_speed = 50f;

		private int m_filter_alpha_speed_min = 50;

		private int m_filter_alpha_speed_max = 100;

		private Texture2D m_texture;

		private Texture2D m_texture1;

		private Texture2D m_texture2;

		private Game m_game;

		public FilterLayer(Game game, Texture2D texture, Texture2D texture2)
		{
			m_game = game;
			m_texture1 = texture;
			m_texture2 = texture2;
			m_texture = m_texture1;
		}

		public void Clear()
		{
			m_game = null;
			m_texture = null;
			m_texture1 = null;
			m_texture2 = null;
		}

		public void Update(TimeSpan elapsed)
		{
			bool flag = false;
			if (m_filter_alpha <= (float)m_next_filter_alpha)
			{
				m_filter_alpha += (float)elapsed.TotalMilliseconds * 0.001f * m_filter_alpha_speed;
				if (m_filter_alpha >= (float)m_next_filter_alpha)
				{
					flag = true;
				}
			}
			else
			{
				m_filter_alpha -= (float)elapsed.TotalMilliseconds * 0.001f * m_filter_alpha_speed;
				if (m_filter_alpha <= (float)m_next_filter_alpha)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			m_filter_alpha = m_next_filter_alpha;
			m_filter_alpha_speed = m_game.GetRandom(m_filter_alpha_speed_min, m_filter_alpha_speed_max);
			m_game.GetRandom(0, 1);
			if (m_next_filter_alpha > 0)
			{
				m_next_filter_alpha = 0;
				return;
			}
			m_next_filter_alpha = m_game.GetRandom(m_filter_alpha_min, m_filter_alpha_max);
			if (m_texture == m_texture1)
			{
				m_texture = m_texture2;
			}
			else
			{
				m_texture = m_texture1;
			}
		}

		public void Draw(SpriteBatch SB)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			SB.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
			SB.GraphicsDevice.RenderState.SourceBlend = (Blend)5;
			SB.GraphicsDevice.RenderState.DestinationBlend = (Blend)6;
			SB.Draw(m_texture, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)m_filter_alpha));
			SB.End();
		}
	}

	private Animation2D m_effect;

	private Texture2D m_o_white_1;

	private Texture2D m_o_white_2;

	private Texture2D m_o_white_3;

	private Texture2D m_o_white;

	private Texture2D m_filter1;

	private Texture2D m_filter2;

	private Texture2D m_filter3;

	private float m_ow_timer;

	private int m_ow_timout_min = 4000;

	private int m_ow_timout_max = 8000;

	private float m_ow_show_timer;

	private int m_ow_show_timer_min = 500;

	private int m_ow_show_timer_max = 2000;

	private bool m_show_ow;

	private int m_ow_alpha;

	private int m_ow_alpha_min = 32;

	private int m_ow_alpha_max = 92;

	private bool m_draw_effect = true;

	private SoundEffect m_breathe1;

	private SoundEffect m_breathe2;

	private SoundEffect m_breathe3;

	private FilterLayer m_fl1;

	private FilterLayer m_fl2;

	private FilterLayer m_fl3;

	public Room3()
	{
		m_content_path = "World/Room3/";
		m_name = "Room3";
	}

	public override void Load(Game game)
	{
		base.Load(game);
		TextureAnimation textureAnimation = null;
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/RotatePainting/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 31);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/Shadows/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 13);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2_wrong/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 50);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2_completed/", 1u, reverse: false);
		textureAnimation.UseCombinedFrames(320, 180, 50);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2_correct/", 1u, reverse: false);
		textureAnimation.m_path = m_content_path + "Animations/View1to2_correct_/";
		textureAnimation.UseCombinedFrames(320, 180, 29);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2_completed/", 1u, reverse: false);
		textureAnimation.m_path = m_content_path + "Animations/View1to2_correct/";
		textureAnimation.UseCombinedFrames(320, 180, 21);
		textureAnimation.m_frame_smoothing = true;
		textureAnimation.AddAnimation((TextureAnimation)m_CL.GetContent(m_content_path + "Animations/View1to2_correct_/"), 0, 28);
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2_correct2/", 1u, reverse: false);
		textureAnimation.m_path = m_content_path + "Animations/View1to2_correct2_/";
		textureAnimation.UseCombinedFrames(320, 180, 7);
		textureAnimation.m_frame_smoothing = true;
		m_CL.AddContent(textureAnimation);
		textureAnimation = new TextureAnimation(game, m_CL, m_content_path + "Animations/View1to2_completed/", 1u, reverse: false);
		textureAnimation.m_path = m_content_path + "Animations/View1to2_correct2/";
		textureAnimation.UseCombinedFrames(320, 180, 22);
		textureAnimation.m_frame_smoothing = true;
		textureAnimation.AddAnimation((TextureAnimation)m_CL.GetContent(m_content_path + "Animations/View1to2_correct2_/"), 0, 6);
		textureAnimation.AddAnimation((TextureAnimation)m_CL.GetContent(m_content_path + "Animations/View1to2_wrong/"), 29, 49);
		m_CL.AddContent(textureAnimation);
		textureAnimation = null;
		m_breathe1 = m_CL.LoadSound(m_content_path + "Sound/andning1");
		m_breathe2 = m_CL.LoadSound(m_content_path + "Sound/andning2");
		m_breathe3 = m_CL.LoadSound(m_content_path + "Sound/andning3");
		new View1(m_game, this);
		new View1_1(m_game, this);
		new View2(m_game, this);
		new View2_1(m_game, this);
		new View2_2(m_game, this);
		new View2_2_1(m_game, this);
		new View3(m_game, this);
		SetupViews();
		m_o_white_1 = m_CL.LoadTexture("World/Room3/Effect/o_white_effect1");
		m_o_white_2 = m_CL.LoadTexture("World/Room3/Effect/o_white_effect2");
		m_o_white_3 = m_CL.LoadTexture("World/Room3/Effect/o_white_effect3");
		m_o_white = m_o_white_1;
		m_filter1 = m_CL.LoadTexture("World/Room3/Filter/filter1");
		m_filter2 = m_CL.LoadTexture("World/Room3/Filter/filter2");
		m_filter3 = m_CL.LoadTexture("World/Room3/Filter/filter3");
		m_fl1 = new FilterLayer(m_game, m_filter1, m_filter3);
		m_fl1.m_filter_alpha = 0f;
		m_fl1.m_next_filter_alpha = 32;
		m_fl2 = new FilterLayer(m_game, m_filter2, m_filter3);
		m_fl2.m_filter_alpha = 32f;
		m_fl2.m_next_filter_alpha = 0;
		m_fl3 = new FilterLayer(m_game, m_filter3, m_filter3);
		m_fl3.m_filter_alpha = 24f;
		m_fl3.m_next_filter_alpha = 0;
		if (m_game.m_game_data.GetState("Room3.TicTacCompleted") == "1")
		{
			HandleTicTacCompleted();
			m_game.m_game_data.SetState("Music", "");
		}
		else
		{
			m_game.m_game_data.SetState("Music", "2");
		}
		m_game.m_play_door_sound = false;
	}

	public override void Clear()
	{
		m_breathe1 = null;
		m_breathe2 = null;
		m_breathe3 = null;
		if (m_effect != null)
		{
			m_effect.Clear();
			m_effect = null;
		}
		m_o_white = null;
		if (m_o_white_1 != null)
		{
			((GraphicsResource)m_o_white_1).Dispose();
			m_o_white_1 = null;
		}
		if (m_o_white_2 != null)
		{
			((GraphicsResource)m_o_white_2).Dispose();
			m_o_white_2 = null;
		}
		if (m_o_white_3 != null)
		{
			((GraphicsResource)m_o_white_3).Dispose();
			m_o_white_3 = null;
		}
		if (m_fl1 != null)
		{
			m_fl1.Clear();
			m_fl1 = null;
		}
		if (m_fl2 != null)
		{
			m_fl2.Clear();
			m_fl2 = null;
		}
		if (m_fl3 != null)
		{
			m_fl3.Clear();
			m_fl3 = null;
		}
		if (m_filter1 != null)
		{
			((GraphicsResource)m_filter1).Dispose();
			m_filter1 = null;
		}
		if (m_filter2 != null)
		{
			((GraphicsResource)m_filter2).Dispose();
			m_filter2 = null;
		}
		if (m_filter3 != null)
		{
			((GraphicsResource)m_filter3).Dispose();
			m_filter3 = null;
		}
		base.Clear();
	}

	public override void Init()
	{
		base.Init();
		if (m_game.m_game_data.GetState("Room3.Entered") != "1")
		{
			m_game.m_show_cursor = false;
		}
	}

	private void HandleTicTacCompleted()
	{
		m_draw_effect = false;
		m_game.FadeOutMusic();
	}

	public override void HandleEvent(string s_event)
	{
		switch (s_event)
		{
		case "View2_1.onWin2":
			HandleTicTacCompleted();
			break;
		case "View2_1.TicTacLost":
			m_draw_effect = true;
			break;
		}
		base.HandleEvent(s_event);
	}

	public override void UpdateEffect(TimeSpan elapsed)
	{
		base.UpdateEffect(elapsed);
		if (!m_draw_effect)
		{
			return;
		}
		if (m_effect != null)
		{
			m_effect.Update(elapsed);
		}
		if (m_fl1 != null)
		{
			m_fl1.Update(elapsed);
		}
		if (m_fl2 != null)
		{
			m_fl2.Update(elapsed);
		}
		if (m_fl3 != null)
		{
			m_fl3.Update(elapsed);
		}
		if (!m_show_ow)
		{
			m_ow_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_ow_timer <= 0f)
			{
				m_show_ow = true;
				m_ow_show_timer = (float)m_game.GetRandom(m_ow_show_timer_min, m_ow_show_timer_max) * 0.001f;
				switch (m_game.GetRandom(0, 2))
				{
				case 0:
					m_o_white = m_o_white_1;
					break;
				case 1:
					m_o_white = m_o_white_2;
					break;
				case 2:
					m_o_white = m_o_white_3;
					break;
				}
				SoundEffect val = null;
				switch (m_game.GetRandom(0, 2))
				{
				case 0:
					val = m_breathe1;
					break;
				case 1:
					val = m_breathe2;
					break;
				case 2:
					val = m_breathe3;
					break;
				}
				if (val != null)
				{
					float num = m_game.GetRandom(0, 20);
					float num2 = m_game.GetRandom(0, 8);
					num *= 0.1f;
					num2 *= 0.1f;
					num--;
					num2--;
					m_game.PlaySound(val, 0.1f, num, num2);
				}
				val = null;
			}
		}
		else
		{
			m_ow_show_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_ow_show_timer <= 0f)
			{
				m_show_ow = false;
				m_ow_timer = (float)m_game.GetRandom(m_ow_timout_min, m_ow_timout_max) * 0.001f;
			}
			m_ow_alpha = m_game.GetRandom(m_ow_alpha_min, m_ow_alpha_max);
		}
	}

	public override void DrawEffect(SpriteBatch SB)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.DrawEffect(SB);
		if (m_draw_effect)
		{
			if (m_show_ow)
			{
				SB.Begin((SpriteBlendMode)1);
				SB.Draw(m_o_white, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)m_ow_alpha));
				SB.End();
			}
			if (m_fl1 != null)
			{
				m_fl1.Draw(SB);
			}
			if (m_fl2 != null)
			{
				m_fl2.Draw(SB);
			}
			if (m_effect != null)
			{
				m_effect.Draw(SB);
			}
		}
	}
}
