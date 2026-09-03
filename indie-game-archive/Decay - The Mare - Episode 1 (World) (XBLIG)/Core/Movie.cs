using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SGSCore;

namespace Core;

public class Movie
{
	public Video m_video;

	public VideoPlayer m_video_player;

	public Movie()
	{
		m_video_player = new VideoPlayer();
	}

	public void Load(SGSContentLoader CL, string path)
	{
		m_video = CL.LoadVideo(path);
	}

	public void Play()
	{
		if (m_video_player != null)
		{
			m_video_player.IsLooped = false;
			m_video_player.Play(m_video);
		}
	}

	public bool Finished()
	{
		if (m_video_player != null && m_video_player.State == MediaState.Stopped)
		{
			return true;
		}
		return false;
	}

	public virtual void Clear()
	{
		m_video = null;
		if (m_video_player != null)
		{
			m_video_player.Dispose();
			m_video_player = null;
		}
	}

	public virtual void Update(TimeSpan elapsed)
	{
	}

	public virtual void Draw(SpriteBatch SB, Color color)
	{
		if (m_video_player != null)
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_video_player.GetTexture(), Game.VIEW_RECT, color);
			SB.End();
		}
	}
}
