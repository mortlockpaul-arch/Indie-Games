using System.Collections.Generic;
using System.Diagnostics;

namespace EGEngine;

public class Profiler
{
	public static List<Profiler> AllProfilers = new List<Profiler>();

	private string name;

	private double elapsedTime;

	private Stopwatch stopwatch;

	public Profiler(string name)
	{
		this.name = name;
		AllProfilers.Add(this);
	}

	public void Start()
	{
		stopwatch = Stopwatch.StartNew();
	}

	public void Stop()
	{
		elapsedTime += stopwatch.Elapsed.TotalSeconds;
	}

	public void Print(double totalTime)
	{
		elapsedTime = 0.0;
	}
}
