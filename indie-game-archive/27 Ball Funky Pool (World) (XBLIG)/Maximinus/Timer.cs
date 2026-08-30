using System;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class Timer
{
	private bool started;

	private double startTime;

	private float waitTime;

	private double pausedTime;

	private double pausedAtTime;

	public bool Started => started;

	public double StartTime => startTime;

	public double WaitTime => waitTime;

	public bool Paused => pausedAtTime != -1.0;

	public Timer(GameTime gameTime, float nbSeconds)
		: this(gameTime.TotalGameTime.TotalSeconds, nbSeconds)
	{
	}

	public Timer(double totalSeconds, float nbSeconds)
	{
		Reset(totalSeconds, nbSeconds);
	}

	public void Reset(double totalSeconds, float nbSeconds)
	{
		startTime = totalSeconds;
		waitTime = nbSeconds;
		pausedTime = 0.0;
		pausedAtTime = -1.0;
		started = true;
	}

	public void Reset(GameTime gameTime, float nbSeconds)
	{
		startTime = gameTime.TotalGameTime.TotalSeconds;
		waitTime = nbSeconds;
		pausedTime = 0.0;
		pausedAtTime = -1.0;
		started = true;
	}

	public void Pause(GameTime gameTime)
	{
		if (Paused)
		{
			throw new Exception("set pause for timer already paused");
		}
		pausedAtTime = gameTime.TotalGameTime.TotalSeconds;
	}

	public void UnPause(GameTime gameTime)
	{
		if (!Paused)
		{
			throw new Exception("unset pause for timer not paused");
		}
		pausedTime += gameTime.TotalGameTime.TotalSeconds - pausedAtTime;
		pausedAtTime = -1.0;
	}

	public void Stop()
	{
		started = false;
	}

	public Timer()
	{
		started = false;
	}

	public bool IsFinished(GameTime gameTime)
	{
		if (!started)
		{
			return true;
		}
		return ValueSeconds(gameTime) > (double)waitTime;
	}

	public double ValueSeconds(GameTime gameTime)
	{
		if (!started)
		{
			throw new Exception("Timer not started");
		}
		if (Paused)
		{
			return pausedAtTime - startTime - pausedTime;
		}
		return gameTime.TotalGameTime.TotalSeconds - startTime - pausedTime;
	}

	public double RemainingSeconds(GameTime gameTime)
	{
		return (double)waitTime - ValueSeconds(gameTime);
	}

	public float RemainingRatio(GameTime gameTime)
	{
		if (IsFinished(gameTime))
		{
			return 0f;
		}
		return (float)(RemainingSeconds(gameTime) / (double)waitTime);
	}

	public double ElapsedRatio(GameTime gameTime)
	{
		return 1f - RemainingRatio(gameTime);
	}

	public static float Ratio(GameTime gameTime, double startTime, double maxTime)
	{
		return (float)Utils.clampRatio((gameTime.TotalGameTime.TotalSeconds - startTime) / maxTime);
	}

	public static float RatioLoop(GameTime gameTime, double startTime, double maxTime)
	{
		return (float)Utils.clampRatio((gameTime.TotalGameTime.TotalSeconds - startTime) % maxTime / maxTime);
	}
}
