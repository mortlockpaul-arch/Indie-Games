using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

public class Animation2D : Object2D
{
	public enum ANIM_STATE
	{
		ANIM_STATE_PLAYING,
		ANIM_STATE_PAUSED,
		ANIM_STATE_STOPPED
	}

	public enum LOOP_TYPE
	{
		NO_LOOP,
		CYCLE,
		PING_PONG
	}

	protected TimeSpan m_time_per_frame = TimeSpan.FromSeconds(1.0 / 30.0);

	protected TimeSpan m_timer = TimeSpan.Zero;

	protected int m_num_frames;

	public int m_current_frame;

	public bool m_reverse;

	protected Game m_game;

	protected Texture2D m_texture;

	protected Texture2D m_smoothing_texture;

	protected double m_fps = 30.0;

	protected BlendState m_multiply_BS;

	public LOOP_TYPE m_loop;

	protected bool m_FPS_changed;

	public float m_smoothing_alpha = 1f;

	protected int m_smoothing_frame = 1;

	public ANIM_STATE m_state = ANIM_STATE.ANIM_STATE_STOPPED;

	public int m_width;

	public int m_height;

	public bool m_random_mode;

	public bool m_frame_smoothing;

	public Animation2D(Game game, string content_path, uint frames, bool reverse)
		: base(content_path)
	{
		m_game = game;
		m_reverse = reverse;
		m_num_frames = (int)frames;
		m_multiply_BS = new BlendState();
		m_multiply_BS.ColorSourceBlend = Blend.DestinationColor;
		m_multiply_BS.ColorDestinationBlend = Blend.SourceColor;
	}

	public override void Clear()
	{
		m_game = null;
		if (m_multiply_BS != null)
		{
			m_multiply_BS.Dispose();
			m_multiply_BS = null;
		}
		m_texture = null;
		m_smoothing_texture = null;
	}

	public virtual void SetFPS(double fps)
	{
		m_fps = Math.Round(fps);
		m_time_per_frame = TimeSpan.FromSeconds(1.0 / fps);
		m_FPS_changed = true;
	}

	public virtual double GetFPS()
	{
		return m_fps;
	}

	protected virtual void onNextFrame()
	{
	}

	public override void Update(TimeSpan elapsed)
	{
		switch (m_state)
		{
		case ANIM_STATE.ANIM_STATE_PLAYING:
			m_timer += elapsed;
			if (m_timer > m_time_per_frame)
			{
				onNextFrame();
				m_smoothing_frame = m_current_frame;
				m_smoothing_alpha = 1f;
				m_timer -= m_time_per_frame;
				if (m_FPS_changed)
				{
					m_timer = TimeSpan.Zero;
					m_FPS_changed = false;
				}
				if (m_random_mode)
				{
					int current_frame = m_current_frame;
					m_current_frame = m_game.GetRandom(0, m_num_frames - 1);
					if (m_current_frame == current_frame)
					{
						m_current_frame++;
						if (m_current_frame >= m_num_frames)
						{
							m_current_frame = 0;
						}
					}
					break;
				}
				if (!m_reverse)
				{
					m_current_frame++;
					if (m_current_frame >= m_num_frames)
					{
						if (m_loop == LOOP_TYPE.NO_LOOP)
						{
							m_state = ANIM_STATE.ANIM_STATE_STOPPED;
							m_current_frame = m_num_frames - 1;
						}
						else if (m_loop == LOOP_TYPE.PING_PONG)
						{
							m_current_frame = m_num_frames - 1;
							m_reverse = true;
						}
						else if (m_loop == LOOP_TYPE.CYCLE)
						{
							m_current_frame = 0;
						}
					}
					break;
				}
				m_current_frame--;
				if (m_current_frame < 0)
				{
					if (m_loop == LOOP_TYPE.NO_LOOP)
					{
						m_state = ANIM_STATE.ANIM_STATE_STOPPED;
						m_current_frame = 0;
					}
					else if (m_loop == LOOP_TYPE.PING_PONG)
					{
						m_current_frame = 0;
						m_reverse = false;
					}
				}
			}
			else
			{
				m_smoothing_alpha = 1f - Math.Abs((float)m_timer.TotalMilliseconds / (float)m_time_per_frame.TotalMilliseconds * 1f);
				if (m_smoothing_alpha < 0f)
				{
					m_smoothing_alpha = 0f;
				}
			}
			break;
		case ANIM_STATE.ANIM_STATE_PAUSED:
		case ANIM_STATE.ANIM_STATE_STOPPED:
			break;
		}
	}

	public virtual void Play()
	{
		m_state = ANIM_STATE.ANIM_STATE_PLAYING;
		m_timer = TimeSpan.Zero;
		if (m_reverse)
		{
			m_current_frame = m_num_frames - 1;
			m_smoothing_frame = m_current_frame;
			if (m_frame_smoothing)
			{
				m_current_frame--;
			}
		}
		else
		{
			m_current_frame = 0;
			m_smoothing_frame = m_current_frame;
			if (m_frame_smoothing)
			{
				m_current_frame++;
			}
		}
		m_smoothing_alpha = 1f;
	}

	public void Play(LOOP_TYPE loop)
	{
		m_loop = loop;
		Play();
	}

	public override void Draw(SpriteBatch SB)
	{
		if (m_texture != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_texture, Game.VIEW_RECT, Color.White);
			if (m_smoothing_texture != null)
			{
				SB.Draw(m_smoothing_texture, Game.VIEW_RECT, Color.White * m_smoothing_alpha);
			}
			SB.End();
		}
	}

	public override void Draw(SpriteBatch SB, Color color)
	{
		if (m_texture != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_texture, Game.VIEW_RECT, color);
			if (m_smoothing_texture != null)
			{
				SB.Draw(m_smoothing_texture, Game.VIEW_RECT, color * m_smoothing_alpha);
			}
			SB.End();
		}
	}

	public override void Draw(SpriteBatch SB, Vector2 pos)
	{
		if (m_texture != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_texture, pos, Color.White);
			SB.End();
		}
	}

	public override void Draw(SpriteBatch SB, Vector2 pos, Rectangle source_rect, Color color)
	{
		if (m_texture != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_texture, pos, source_rect, color);
			SB.End();
		}
	}

	public override void Draw(SpriteBatch SB, Vector2 pos, Color color)
	{
		if (m_texture != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_texture, pos, color);
			SB.End();
		}
	}

	public override void Draw(SpriteBatch SB, Rectangle dest_rect, Rectangle source_rect, Color color)
	{
		if (m_texture != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_texture, dest_rect, source_rect, color);
			SB.End();
		}
	}

	public override void DrawMultiply(SpriteBatch SB, Color color)
	{
		try
		{
			if (m_game.m_GDM.GraphicsProfile != GraphicsProfile.Reach && m_texture != null)
			{
				m_game.GraphicsDevice.BlendState = m_multiply_BS;
				SB.Begin(SpriteSortMode.Texture, BlendState.Additive);
				SB.Draw(m_texture, Game.VIEW_RECT, color);
				SB.End();
				m_game.GraphicsDevice.BlendState = BlendState.Opaque;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Animation2D.DrawMultiply: " + ex.Message);
		}
	}
}
