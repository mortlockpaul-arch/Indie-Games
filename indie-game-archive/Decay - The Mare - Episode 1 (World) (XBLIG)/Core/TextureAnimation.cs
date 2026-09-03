using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace Core;

public class TextureAnimation : Animation2D
{
	protected struct ChainedAnimation
	{
		public int m_start_frame;

		public int m_end_frame;

		public TextureAnimation m_anim;
	}

	private List<Texture2D> m_frames = new List<Texture2D>();

	public int m_combined_frames;

	protected bool m_update_chain_animation;

	protected int m_chain_anim_index;

	public int m_start_frame;

	public int m_end_frame;

	protected float m_current_alpha = 255f;

	private int m_frame_width;

	private int m_frame_height;

	public Rectangle m_dest_rect;

	public bool m_positioned;

	protected List<ChainedAnimation> m_animations;

	private int m_local_frame;

	private int m_local_frame_x;

	private int m_local_frame_y;

	private int m_prev_local_frame_x;

	private int m_prev_local_frame_y;

	private int m_frames_x;

	private int m_frames_y;

	public TextureAnimation(Game game, ContentManager CM, string content_path, uint frames, bool reverse)
		: base(game, content_path, frames, reverse)
	{
		m_start_frame = 0;
		m_end_frame = m_num_frames - 1;
		for (int i = 1; i <= frames; i++)
		{
			string text = "0000";
			text = text.Insert(text.Length - i.ToString().Length, i.ToString());
			text = text.Substring(0, 4);
			m_frames.Add(CM.Load<Texture2D>(content_path + text));
			if (m_width < m_frames[i - 1].Width)
			{
				m_width = m_frames[i - 1].Width;
			}
			if (m_height < m_frames[i - 1].Height)
			{
				m_height = m_frames[i - 1].Height;
			}
			m_frame_width = m_width;
			m_frame_height = m_height;
			text = null;
		}
	}

	public TextureAnimation(Game game, SGSContentLoader CL, string content_path, uint frames, bool reverse)
		: base(game, content_path, frames, reverse)
	{
		m_start_frame = 0;
		m_end_frame = m_num_frames - 1;
		for (int i = 1; i <= frames; i++)
		{
			string text = "0000";
			text = text.Insert(text.Length - i.ToString().Length, i.ToString());
			text = text.Substring(0, 4);
			m_frames.Add(CL.LoadTexture(content_path + text));
			if (m_width < m_frames[i - 1].Width)
			{
				m_width = m_frames[i - 1].Width;
			}
			if (m_height < m_frames[i - 1].Height)
			{
				m_height = m_frames[i - 1].Height;
			}
			m_frame_width = m_width;
			m_frame_height = m_height;
			text = null;
		}
	}

	public void AddFrames(Game game, ContentManager CM, string content_path, int start_frame, uint frames)
	{
		m_num_frames += (int)frames;
		for (int i = start_frame; i <= m_num_frames; i++)
		{
			string text = "0000";
			text = text.Insert(text.Length - i.ToString().Length, i.ToString());
			text = text.Substring(0, 4);
			m_frames.Add(CM.Load<Texture2D>(content_path + text));
			if (m_width < m_frames[i - 1].Width)
			{
				m_width = m_frames[i - 1].Width;
			}
			if (m_height < m_frames[i - 1].Height)
			{
				m_height = m_frames[i - 1].Height;
			}
			text = null;
		}
	}

	public void AddAnimation(TextureAnimation anim, int start_frame, int end_frame)
	{
		if (m_animations == null)
		{
			m_animations = new List<ChainedAnimation>();
		}
		ChainedAnimation item = new ChainedAnimation
		{
			m_start_frame = start_frame,
			m_end_frame = end_frame,
			m_anim = anim
		};
		m_animations.Add(item);
	}

	public void AddFrames(Game game, SGSContentLoader CL, string content_path, int start_frame, uint frames)
	{
		m_num_frames += (int)frames;
		for (int i = start_frame; i <= m_num_frames; i++)
		{
			string text = "0000";
			text = text.Insert(text.Length - i.ToString().Length, i.ToString());
			text = text.Substring(0, 4);
			m_frames.Add(CL.LoadTexture(content_path + text));
			if (m_width < m_frames[i - 1].Width)
			{
				m_width = m_frames[i - 1].Width;
			}
			if (m_height < m_frames[i - 1].Height)
			{
				m_height = m_frames[i - 1].Height;
			}
			text = null;
		}
	}

	public override void Clear()
	{
		if (m_animations != null)
		{
			m_animations.Clear();
			m_animations = null;
		}
		if (m_frames != null)
		{
			for (int i = 0; i < m_frames.Count; i++)
			{
				m_frames[i] = null;
			}
			m_frames.Clear();
			m_frames = null;
		}
		base.Clear();
	}

	public void UseCombinedFrames(int frame_width, int frame_height, int total_frames, int size)
	{
		m_frame_width = frame_width;
		m_frame_height = frame_height;
		m_frames_x = (int)Math.Floor((float)size / (float)frame_width);
		m_frames_y = (int)Math.Floor((float)size / (float)frame_height);
		m_combined_frames = m_frames_x * m_frames_y;
		m_num_frames = total_frames;
		m_start_frame = 0;
		m_end_frame = m_num_frames - 1;
	}

	public override void Play()
	{
		m_state = ANIM_STATE.ANIM_STATE_PLAYING;
		m_timer = TimeSpan.Zero;
		if (m_reverse)
		{
			m_current_frame = m_end_frame;
			m_smoothing_frame = m_current_frame;
		}
		else
		{
			m_current_frame = m_start_frame;
			m_smoothing_frame = m_current_frame;
		}
		m_smoothing_alpha = m_current_alpha;
		if (m_num_frames == 0)
		{
			if (m_animations != null)
			{
				m_update_chain_animation = true;
				m_animations[m_chain_anim_index].m_anim.m_timer = m_timer;
				m_animations[m_chain_anim_index].m_anim.m_reverse = m_reverse;
				m_animations[m_chain_anim_index].m_anim.Play();
				if (m_reverse)
				{
					m_chain_anim_index = m_animations.Count - 1;
					m_animations[m_chain_anim_index].m_anim.SetFrame(m_animations[m_chain_anim_index].m_end_frame);
				}
				else
				{
					m_animations[m_chain_anim_index].m_anim.SetFrame(m_animations[m_chain_anim_index].m_start_frame);
				}
				m_animations[m_chain_anim_index].m_anim.m_start_frame = m_animations[m_chain_anim_index].m_start_frame;
				m_animations[m_chain_anim_index].m_anim.m_end_frame = m_animations[m_chain_anim_index].m_end_frame;
			}
			return;
		}
		if (m_combined_frames != 0)
		{
			m_local_frame = 0;
			m_local_frame_x = 0;
			m_local_frame_y = 0;
			m_prev_local_frame_x = 0;
			m_prev_local_frame_y = 0;
		}
		if (m_reverse && m_animations != null)
		{
			m_chain_anim_index = m_animations.Count - 1;
			m_update_chain_animation = true;
			m_animations[m_chain_anim_index].m_anim.m_timer = m_timer;
			m_animations[m_chain_anim_index].m_anim.m_reverse = m_reverse;
			m_animations[m_chain_anim_index].m_anim.Play();
			m_animations[m_chain_anim_index].m_anim.SetFrame(m_animations[m_chain_anim_index].m_end_frame);
			m_animations[m_chain_anim_index].m_anim.m_start_frame = m_animations[m_chain_anim_index].m_start_frame;
			m_animations[m_chain_anim_index].m_anim.m_end_frame = m_animations[m_chain_anim_index].m_end_frame;
		}
		SetFrame(m_current_frame);
	}

	public void SetFrame(int frame)
	{
		m_current_frame = frame;
		m_smoothing_frame = m_current_frame;
		m_smoothing_alpha = m_current_alpha;
		if (m_combined_frames != 0)
		{
			float num = (float)m_current_frame / (float)m_combined_frames - (float)(m_current_frame / m_combined_frames);
			m_local_frame = (int)Math.Round((float)m_combined_frames * num);
			m_local_frame_y = (int)Math.Floor((float)m_local_frame / (float)m_frames_x);
			m_prev_local_frame_y = m_local_frame_y;
			m_local_frame_x = m_local_frame - m_local_frame_y * m_frames_x;
			m_prev_local_frame_x = m_local_frame_x;
		}
	}

	protected override void onNextFrame()
	{
		base.onNextFrame();
		if (m_combined_frames == 0)
		{
			return;
		}
		m_prev_local_frame_x = m_local_frame_x;
		m_prev_local_frame_y = m_local_frame_y;
		if (!m_reverse)
		{
			m_local_frame++;
			if (m_local_frame >= m_combined_frames)
			{
				m_local_frame = 0;
				m_local_frame_x = 0;
				m_local_frame_y = 0;
				return;
			}
			m_local_frame_x++;
			if (m_local_frame_x >= m_frames_x)
			{
				m_local_frame_x = 0;
				m_local_frame_y++;
			}
			return;
		}
		m_local_frame--;
		if (m_local_frame < 0)
		{
			m_local_frame = m_combined_frames - 1;
			m_local_frame_x = m_frames_x - 1;
			m_local_frame_y = m_frames_y - 1;
			return;
		}
		m_local_frame_x--;
		if (m_local_frame_x < 0)
		{
			m_local_frame_x = m_frames_x - 1;
			m_local_frame_y--;
		}
	}

	public override void Update(TimeSpan elapsed)
	{
		if (m_update_chain_animation)
		{
			m_animations[m_chain_anim_index].m_anim.Update(elapsed);
			if (!m_reverse)
			{
				if (m_animations[m_chain_anim_index].m_anim.m_state == ANIM_STATE.ANIM_STATE_STOPPED)
				{
					m_chain_anim_index++;
					if (m_chain_anim_index >= m_animations.Count)
					{
						m_update_chain_animation = false;
						m_state = ANIM_STATE.ANIM_STATE_STOPPED;
						return;
					}
					m_animations[m_chain_anim_index].m_anim.m_timer = m_animations[m_chain_anim_index - 1].m_anim.m_timer;
					m_animations[m_chain_anim_index].m_anim.m_reverse = m_reverse;
					m_animations[m_chain_anim_index].m_anim.Play();
					m_animations[m_chain_anim_index].m_anim.SetFrame(m_animations[m_chain_anim_index].m_start_frame);
					m_animations[m_chain_anim_index].m_anim.m_start_frame = m_animations[m_chain_anim_index].m_start_frame;
					m_animations[m_chain_anim_index].m_anim.m_end_frame = m_animations[m_chain_anim_index].m_end_frame;
					Console.WriteLine("Chained again!");
				}
			}
			else if (m_animations[m_chain_anim_index].m_anim.m_state == ANIM_STATE.ANIM_STATE_STOPPED)
			{
				m_chain_anim_index--;
				if (m_chain_anim_index < 0)
				{
					m_update_chain_animation = false;
					m_timer = m_animations[0].m_anim.m_timer;
					return;
				}
				m_animations[m_chain_anim_index].m_anim.m_timer = m_animations[m_chain_anim_index + 1].m_anim.m_timer;
				m_animations[m_chain_anim_index].m_anim.m_reverse = m_reverse;
				m_animations[m_chain_anim_index].m_anim.Play();
				m_animations[m_chain_anim_index].m_anim.SetFrame(m_animations[m_chain_anim_index].m_end_frame);
				m_animations[m_chain_anim_index].m_anim.m_start_frame = m_animations[m_chain_anim_index].m_start_frame;
				m_animations[m_chain_anim_index].m_anim.m_end_frame = m_animations[m_chain_anim_index].m_end_frame;
			}
			return;
		}
		m_timer += elapsed;
		switch (m_state)
		{
		case ANIM_STATE.ANIM_STATE_PLAYING:
			if (!(m_timer > m_time_per_frame))
			{
				break;
			}
			onNextFrame();
			m_smoothing_frame = m_current_frame;
			m_smoothing_alpha = m_current_alpha;
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
				SetFrame(m_current_frame);
				return;
			}
			if (!m_reverse)
			{
				m_current_frame++;
				if (m_current_frame <= m_end_frame)
				{
					break;
				}
				if (m_loop == LOOP_TYPE.NO_LOOP)
				{
					if (m_animations != null)
					{
						m_update_chain_animation = true;
						m_chain_anim_index = 0;
						m_animations[m_chain_anim_index].m_anim.m_timer = m_timer;
						m_animations[m_chain_anim_index].m_anim.m_reverse = m_reverse;
						m_animations[m_chain_anim_index].m_anim.Play();
						m_animations[m_chain_anim_index].m_anim.SetFrame(m_animations[m_chain_anim_index].m_start_frame);
						m_animations[m_chain_anim_index].m_anim.m_start_frame = m_animations[m_chain_anim_index].m_start_frame;
						m_animations[m_chain_anim_index].m_anim.m_end_frame = m_animations[m_chain_anim_index].m_end_frame;
					}
					else
					{
						m_state = ANIM_STATE.ANIM_STATE_STOPPED;
						m_current_frame = m_end_frame;
						SetFrame(m_current_frame);
					}
				}
				else if (m_loop == LOOP_TYPE.PING_PONG)
				{
					m_current_frame = m_end_frame;
					m_reverse = true;
					SetFrame(m_current_frame);
				}
				else if (m_loop == LOOP_TYPE.CYCLE)
				{
					m_current_frame = m_start_frame;
					SetFrame(m_current_frame);
				}
				break;
			}
			m_current_frame--;
			if (m_current_frame < m_start_frame)
			{
				if (m_loop == LOOP_TYPE.NO_LOOP)
				{
					m_state = ANIM_STATE.ANIM_STATE_STOPPED;
					m_current_frame = m_start_frame;
				}
				else if (m_loop == LOOP_TYPE.PING_PONG)
				{
					m_current_frame = m_start_frame;
					m_reverse = false;
					SetFrame(m_current_frame);
				}
			}
			break;
		}
		if (m_frame_smoothing)
		{
			m_smoothing_alpha = m_current_alpha - Math.Abs((float)m_timer.TotalMilliseconds / (float)m_time_per_frame.TotalMilliseconds * 255f);
			if (m_smoothing_alpha < 0f)
			{
				m_smoothing_alpha = 0f;
			}
		}
	}

	public override void Draw(SpriteBatch SB)
	{
		if (!m_positioned)
		{
			Draw(SB, Color.White);
		}
		else
		{
			Draw(SB, new Rectangle(m_dest_rect.X, m_dest_rect.Y, m_dest_rect.Width, m_dest_rect.Height), new Rectangle(0, 0, m_frame_width, m_frame_height), Color.White);
		}
	}

	public override void Draw(SpriteBatch SB, Color c)
	{
		if (m_positioned)
		{
			Draw(SB, new Rectangle(m_dest_rect.X, m_dest_rect.Y, m_dest_rect.Width, m_dest_rect.Height), new Rectangle(0, 0, m_frame_width, m_frame_height), c);
			return;
		}
		m_current_alpha = (float)(int)c.A / 255f;
		if (m_update_chain_animation)
		{
			m_animations[m_chain_anim_index].m_anim.Draw(SB, c);
			return;
		}
		if (m_combined_frames == 0)
		{
			m_texture = m_frames[m_current_frame];
			if (m_frame_smoothing)
			{
				m_smoothing_texture = m_frames[m_smoothing_frame];
			}
			base.Draw(SB);
			return;
		}
		int index = (int)Math.Floor((float)m_current_frame / (float)m_combined_frames);
		m_texture = m_frames[index];
		Rectangle value = new Rectangle(0, 0, m_frame_width, m_frame_height);
		value.X = m_local_frame_x * m_frame_width;
		value.Y = m_local_frame_y * m_frame_height;
		if (m_texture != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_texture, Game.VIEW_RECT, value, c);
			SB.End();
		}
		if (m_frame_smoothing && c.A == byte.MaxValue)
		{
			index = (m_reverse ? ((int)Math.Floor((float)(m_current_frame + 1) / (float)m_combined_frames)) : ((int)Math.Floor((float)(m_current_frame - 1) / (float)m_combined_frames)));
			if (index >= 0 && index < m_frames.Count)
			{
				m_smoothing_texture = m_frames[index];
			}
			else
			{
				m_smoothing_texture = null;
			}
			if (m_smoothing_texture != null)
			{
				value.X = m_prev_local_frame_x * m_frame_width;
				value.Y = m_prev_local_frame_y * m_frame_height;
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_smoothing_texture, Game.VIEW_RECT, value, Color.White * m_smoothing_alpha);
				SB.End();
			}
		}
	}

	public override void Draw(SpriteBatch SB, Vector2 pos, Rectangle source_rect, Color color)
	{
		m_texture = m_frames[m_current_frame];
		base.Draw(SB, pos, source_rect, color);
	}

	public override void Draw(SpriteBatch SB, Vector2 pos, Color color)
	{
		Draw(SB, new Rectangle((int)pos.X, (int)pos.Y, m_width, m_height), new Rectangle(0, 0, m_width, m_height), color);
	}

	public override void Draw(SpriteBatch SB, Rectangle dest_rect, Rectangle source_rect, Color color)
	{
		try
		{
			m_current_alpha = (int)color.A;
			if (m_update_chain_animation)
			{
				m_animations[m_chain_anim_index].m_anim.Draw(SB, dest_rect, source_rect, color);
				return;
			}
			if (m_combined_frames == 0)
			{
				m_texture = m_frames[m_current_frame];
				base.Draw(SB, dest_rect, source_rect, color);
				if (m_frame_smoothing)
				{
					m_smoothing_texture = m_frames[m_smoothing_frame];
					SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
					SB.Draw(m_smoothing_texture, dest_rect, source_rect, Color.White * m_smoothing_alpha);
					SB.End();
				}
				return;
			}
			int index = (int)Math.Floor((float)m_current_frame / (float)m_combined_frames);
			m_texture = m_frames[index];
			Rectangle value = new Rectangle(0, 0, m_frame_width, m_frame_height);
			value.X = m_local_frame_x * m_frame_width;
			value.Y = m_local_frame_y * m_frame_height;
			value.X += source_rect.X;
			value.Y += source_rect.Y;
			value.Width = source_rect.Width;
			value.Height = source_rect.Height;
			if (m_texture != null)
			{
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_texture, dest_rect, value, color);
				SB.End();
			}
			if (m_frame_smoothing && color.A == byte.MaxValue)
			{
				index = (m_reverse ? ((int)Math.Floor((float)(m_current_frame + 1) / (float)m_combined_frames)) : ((int)Math.Floor((float)(m_current_frame - 1) / (float)m_combined_frames)));
				if (index >= 0 && index < m_frames.Count)
				{
					m_smoothing_texture = m_frames[index];
				}
				else
				{
					m_smoothing_texture = null;
				}
				if (m_smoothing_texture != null)
				{
					value.X = m_prev_local_frame_x * m_frame_width;
					value.Y = m_prev_local_frame_y * m_frame_height;
					SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
					SB.Draw(m_smoothing_texture, dest_rect, value, Color.White * m_smoothing_alpha);
					SB.End();
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("TextureAnimation.Draw: " + ex.Message);
		}
	}

	public override void DrawMultiply(SpriteBatch SB, Color color)
	{
		m_texture = m_frames[m_current_frame];
		base.DrawMultiply(SB, color);
	}
}
