using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SGSCore;

namespace Game;

public class Movie
{
	public Video m_video;

	public VideoPlayer m_video_player;

	public Movie()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		base._002Ector();
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
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (m_video_player != null && (int)m_video_player.State == 0)
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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (m_video_player != null)
		{
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_video_player.GetTexture(), Game.VIEW_RECT, color);
			SB.End();
		}
	}
}
