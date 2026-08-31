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
		private Stopwatch HCB = new Stopwatch();

		private SystemStatistic HC_0002;

		internal float TotalMilliseconds => (float)HCB.Elapsed.TotalMilliseconds;

		internal bool IsRunning => HCB.IsRunning;

		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		public TimeTracker(string name)
		{
			HC_0002 = SystemConsole.GetStatistic(name, SystemStatisticCategory.Performance);
		}

		/// <summary>
		///
		/// </summary>
		[Conditional("ENABLE_TIMETRACKER")]
		public void Begin()
		{
			HCB.Start();
		}

		/// <summary>
		///
		/// </summary>
		[Conditional("ENABLE_TIMETRACKER")]
		public void End()
		{
			HCB.Stop();
			HC_0002.AccumulationValue = (int)(HCB.Elapsed.TotalMilliseconds * 1000.0);
		}

		/// <summary>
		///
		/// </summary>
		[Conditional("ENABLE_TIMETRACKER")]
		public void Reset()
		{
			HCB.Reset();
		}
	}

	private static Dictionary<string, TimeTracker> HCB = new Dictionary<string, TimeTracker>(64);

	internal static Dictionary<string, TimeTracker> TimeTrackers => HCB;

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
		foreach (KeyValuePair<string, TimeTracker> item in HCB)
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
