using System;
using System.Diagnostics;

namespace Quasar.Global;

public class Timer
{
	private const float MAX_INTERVAL = 0.1f;

	public static readonly Timer DefaultTimer = new Timer();

	private Stopwatch stopWatch = new Stopwatch();

	private float timeScale = 1f;

	private int time;

	private float timeSeconds;

	private long totalTime;

	private float totalTimeSeconds;

	private long lastTime;

	private float lastTimeSeconds;

	private float timeRest;

	private long lastStopwatchTime;

	public float TimeScale
	{
		get
		{
			return timeScale;
		}
		set
		{
			timeScale = value;
		}
	}

	public int LastInterval => time;

	public float LastIntervalSeconds => timeSeconds;

	public long LastTime => lastTime;

	public float LastTimeSeconds => lastTimeSeconds;

	public long TotalTime => totalTime;

	public float TotalTimeSeconds => totalTimeSeconds;

	public long CurrentTicks => stopWatch.ElapsedTicks;

	public long CurrentTotalTime => (stopWatch.ElapsedTicks - lastStopwatchTime) * 1000 / Stopwatch.Frequency + totalTime;

	public float CurrentTotalTimeSeconds => (float)(stopWatch.ElapsedTicks - lastStopwatchTime) / (float)Stopwatch.Frequency + totalTimeSeconds;

	public bool Enabled => stopWatch.IsRunning;

	public Timer()
	{
		stopWatch.Start();
	}

	public void Clock()
	{
		Clock(0.1f);
	}

	public void Clock(float maxInterval)
	{
		long elapsedTicks = stopWatch.ElapsedTicks;
		timeSeconds = Math.Min((float)(elapsedTicks - lastStopwatchTime) / (float)Stopwatch.Frequency, maxInterval);
		timeSeconds *= timeScale;
		float num = timeSeconds * 1000f + timeRest;
		time = (int)num;
		timeSeconds = (float)time * 0.001f;
		timeRest = num - (float)time;
		lastTime = totalTime;
		lastTimeSeconds = totalTimeSeconds;
		totalTime += time;
		totalTimeSeconds = (float)totalTime * 0.001f;
		lastStopwatchTime = elapsedTicks;
	}

	public void Reset()
	{
		stopWatch.Reset();
		stopWatch.Stop();
		time = 0;
		timeSeconds = 0f;
		totalTime = 0L;
		totalTimeSeconds = 0f;
		lastTime = 0L;
		lastTimeSeconds = 0f;
		lastStopwatchTime = 0L;
	}

	public void Pause()
	{
		stopWatch.Stop();
	}

	public void Resume()
	{
		stopWatch.Start();
	}

	public long TimeSince(long time)
	{
		return TotalTime - time;
	}

	public float TimeSince(float time)
	{
		return TotalTimeSeconds - time;
	}
}
