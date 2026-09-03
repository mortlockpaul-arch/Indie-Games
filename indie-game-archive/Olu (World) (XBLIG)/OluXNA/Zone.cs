using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class Zone
{
	public int id;

	public EnemyQueue eq;

	public List<IDrawable> background;

	public List<MusicPart> music;

	public List<MusicPart> endMusic;

	public List<ChannelPart> channel;

	public List<ChannelPart> endChannel;

	public Dictionary<int, PathList> paths;

	public PAbstractSet playerPath;

	public int zoneEndTime;

	public bool muteSound;

	public Zone()
	{
		background = new List<IDrawable>();
		music = new List<MusicPart>();
		endMusic = new List<MusicPart>();
		channel = new List<ChannelPart>();
		endChannel = new List<ChannelPart>();
		paths = new Dictionary<int, PathList>();
		eq = new EnemyQueue();
		playerPath = new PAbstractSet();
	}

	public Zone(int _id)
		: this()
	{
		id = _id;
	}

	public void Draw(GameTime gametime)
	{
		foreach (IDrawable item in background)
		{
			item.Draw(gametime);
		}
	}

	public void Update(GameTime gametime)
	{
		eq.Update(gametime);
		while (eq.enemyReady())
		{
			BaseGame.Get().enems.Add(eq.Peek());
			BaseGame.Get().enems[BaseGame.Get().enems.Count - 1].start();
			eq.Popoff();
		}
		if (BaseGame.Get().playBGMusic)
		{
			for (int num = music.Count - 1; num >= 0; num--)
			{
				music[num].Update(gametime);
				if (music[num].done)
				{
					music.RemoveAt(num);
				}
			}
			for (int num2 = channel.Count - 1; num2 >= 0; num2--)
			{
				channel[num2].Update(gametime);
				if (channel[num2].done)
				{
					channel.RemoveAt(num2);
				}
			}
		}
		if (BaseGame.Get().movingToNextZone)
		{
			for (int num3 = endMusic.Count - 1; num3 >= 0; num3--)
			{
				endMusic[num3].Update(gametime);
				if (endMusic[num3].done)
				{
					endMusic.RemoveAt(num3);
				}
			}
			for (int num4 = endChannel.Count - 1; num4 >= 0; num4--)
			{
				endChannel[num4].Update(gametime);
				if (endChannel[num4].done)
				{
					endChannel.RemoveAt(num4);
				}
			}
		}
		foreach (IDrawable item in background)
		{
			item.Update(gametime);
		}
		if (BaseGame.Get().movingToNextZone)
		{
			playerPath.Update(gametime);
		}
	}

	public void LoadGraphics()
	{
		foreach (BackgroundElement item in background)
		{
			item.LoadGraphics();
		}
	}

	public void Start()
	{
		foreach (BackgroundElement item in background)
		{
			item.Start();
		}
		eq.Start();
	}

	public void Finish()
	{
		eq.Clear();
		background.Clear();
		music.Clear();
		endMusic.Clear();
		channel.Clear();
		endChannel.Clear();
		paths.Clear();
		playerPath.pSet.Clear();
	}
}
