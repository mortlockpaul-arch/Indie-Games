using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
///
/// </summary>
public class LightingSystemPerformance
{
	/// <summary>
	///
	/// </summary>
	public class TimeTracker
	{
		private Stopwatch _3A_0018 = new Stopwatch();

		private SystemStatistic _3AL;

		internal float TotalMilliseconds => (float)_3A_0018.Elapsed.TotalMilliseconds;

		internal bool IsRunning => _3A_0018.IsRunning;

		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		public TimeTracker(string name)
		{
			_3AL = SystemConsole.GetStatistic(name, SystemStatisticCategory.Performance);
		}

		/// <summary>
		///
		/// </summary>
		[Conditional("ENABLE_TIMETRACKER")]
		public void Begin()
		{
			_3A_0018.Start();
		}

		/// <summary>
		///
		/// </summary>
		[Conditional("ENABLE_TIMETRACKER")]
		public void End()
		{
			_3A_0018.Stop();
			_3AL.AccumulationValue = (int)(_3A_0018.Elapsed.TotalMilliseconds * 1000.0);
		}

		/// <summary>
		///
		/// </summary>
		[Conditional("ENABLE_TIMETRACKER")]
		public void Reset()
		{
			_3A_0018.Reset();
		}
	}

	private static Dictionary<string, TimeTracker> _3A_0018 = new Dictionary<string, TimeTracker>(64);

	internal static Dictionary<string, TimeTracker> TimeTrackers => _3A_0018;

	/// <summary>
	///
	/// </summary>
	/// <param name="codearea"></param>
	/// <returns></returns>
	public static TimeTracker Begin(string codearea)
	{
		return null;
	}

	/// <summary>
	///
	/// </summary>
	[Conditional("ENABLE_TIMETRACKER")]
	public static void Reset()
	{
		foreach (KeyValuePair<string, TimeTracker> item in _3A_0018)
		{
			if (item.Value.IsRunning)
			{
				throw new Exception("TimeTracker not properly ended.");
			}
		}
	}

	/// <summary>
	///
	/// </summary>
	/// <returns></returns>
	public static string Dump()
	{
		return "";
	}
}
