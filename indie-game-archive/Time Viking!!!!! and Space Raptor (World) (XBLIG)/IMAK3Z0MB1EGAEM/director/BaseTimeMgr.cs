using System;
using System.Collections.Generic;
using IMAK3Z0MB1EGAEM.audio;
using IMAK3Z0MB1EGAEM.hud;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.director;

public class BaseTimeMgr
{
	public enum PlayMode
	{
		Stopped,
		Playing,
		Paused
	}

	public List<TimeSlice> timeSlice;

	public double time;

	private long startTime;

	public PlayMode playMode;

	public long pauseStartTime;

	public int phase;

	public double pulse;

	public int beat;

	public int quadbeat;

	public int octobeat;

	public int hexadecobeat;

	public double trackTime;

	public double trackLeft;

	public int playNum;

	public void Pause(int idx)
	{
		switch (GameState.state)
		{
		case GameState.State.VikingMenu:
		case GameState.State.VikingPlaying:
			Game1.vgame.paused = true;
			break;
		}
		HUD.pauseOwner = idx;
		HUD.pauseMenu.grace = 5;
		pauseStartTime = DateTime.UtcNow.Ticks;
		playMode = PlayMode.Paused;
		Music.Pause();
	}

	public void UnPause()
	{
		switch (GameState.state)
		{
		case GameState.State.VikingMenu:
		case GameState.State.VikingPlaying:
			Game1.vgame.paused = false;
			break;
		}
		long num = DateTime.UtcNow.Ticks - pauseStartTime;
		startTime += num;
		playMode = PlayMode.Playing;
		Music.Resume();
	}

	public void Start()
	{
		beat = -1;
		phase = 0;
		startTime = DateTime.UtcNow.Ticks;
		playMode = PlayMode.Playing;
		playNum++;
	}

	public void Update()
	{
		int num = beat;
		double num2 = time;
		if (playMode != PlayMode.Playing)
		{
			return;
		}
		long ticks = DateTime.UtcNow.Ticks;
		for (int i = 0; i < timeSlice.Count; i++)
		{
			if (timeSlice[i].start < time)
			{
				phase = i;
			}
		}
		double num3 = time - timeSlice[phase].start;
		double num4 = 60.0 / timeSlice[phase].bpm;
		pulse = num3;
		beat = 0;
		while (pulse > num4)
		{
			pulse -= num4;
			beat++;
		}
		pulse /= num4;
		_ = pulse / num4;
		quadbeat = beat * 4 + (int)(pulse * 4.0);
		octobeat = beat * 8 + (int)(pulse * 8.0);
		hexadecobeat = beat * 16 + (int)(pulse * 16.0);
		trackTime = time - timeSlice[phase].start;
		if (phase < timeSlice.Count - 1)
		{
			trackLeft = timeSlice[phase + 1].start - time;
		}
		else
		{
			trackLeft = 0.0;
		}
		if (trackTime >= 0.0)
		{
			if (beat != num)
			{
				SpawnMgr.DoClick(phase, beat);
			}
			else if (num2 < timeSlice[0].start && time >= timeSlice[0].start)
			{
				SpawnMgr.DoClick(0, 0);
			}
		}
	}
}
