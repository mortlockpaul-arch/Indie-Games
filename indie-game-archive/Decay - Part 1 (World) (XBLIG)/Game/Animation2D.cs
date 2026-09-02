using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace Game;

public class Animation2D : SGSContent
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

	public LOOP_TYPE m_loop;

	protected bool m_FPS_changed;

	public float m_smoothing_alpha = 255f;

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
	}

	public override void Clear()
	{
		m_game = null;
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

	public virtual void Update(TimeSpan elapsed)
	{
		switch (m_state)
		{
		case ANIM_STATE.ANIM_STATE_PLAYING:
			m_timer += elapsed;
			if (m_timer > m_time_per_frame)
			{
				onNextFrame();
				m_smoothing_frame = m_current_frame;
				m_smoothing_alpha = 255f;
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
				m_smoothing_alpha = 255f - Math.Abs((float)m_timer.TotalMilliseconds / (float)m_time_per_frame.TotalMilliseconds * 255f);
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
		m_smoothing_alpha = 255f;
	}

	public void Play(LOOP_TYPE loop)
	{
		m_loop = loop;
		Play();
	}

	public virtual void Draw(SpriteBatch SB)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_texture, Game.VIEW_RECT, Color.White);
			if (m_smoothing_texture != null)
			{
				SB.Draw(m_smoothing_texture, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)m_smoothing_alpha));
			}
			SB.End();
		}
	}

	public virtual void Draw(SpriteBatch SB, Color color)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_texture, Game.VIEW_RECT, color);
			if (m_smoothing_texture != null)
			{
				SB.Draw(m_smoothing_texture, Game.VIEW_RECT, new Color(((Color)(ref color)).R, ((Color)(ref color)).G, ((Color)(ref color)).B, (byte)m_smoothing_alpha));
			}
			SB.End();
		}
	}

	public virtual void Draw(SpriteBatch SB, Vector2 pos)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_texture, pos, Color.White);
			SB.End();
		}
	}

	public virtual void Draw(SpriteBatch SB, Vector2 pos, Rectangle source_rect, Color color)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_texture, pos, (Rectangle?)source_rect, color);
			SB.End();
		}
	}

	public virtual void Draw(SpriteBatch SB, Vector2 pos, Color color)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_texture, pos, color);
			SB.End();
		}
	}

	public virtual void Draw(SpriteBatch SB, Rectangle dest_rect, Rectangle source_rect, Color color)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_texture, dest_rect, (Rectangle?)source_rect, color);
			SB.End();
		}
	}

	public virtual void DrawMultiply(SpriteBatch SB, Color color)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (m_texture != null)
		{
			SB.Begin((SpriteBlendMode)2, (SpriteSortMode)2, (SaveStateMode)0);
			((Game)m_game).GraphicsDevice.RenderState.SourceBlend = (Blend)9;
			((Game)m_game).GraphicsDevice.RenderState.DestinationBlend = (Blend)3;
			SB.Draw(m_texture, Game.VIEW_RECT, color);
			SB.End();
		}
	}
}
