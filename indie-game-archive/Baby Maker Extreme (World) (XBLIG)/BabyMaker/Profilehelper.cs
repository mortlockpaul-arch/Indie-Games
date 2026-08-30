using System.Collections.Generic;
using System.Diagnostics;

namespace BabyMaker;

public class Profilehelper
{
	private bool m_bEnabled;

	private Dictionary<int, Stopwatch> profiles;

	private string m_results;

	public bool Enabled
	{
		get
		{
			return m_bEnabled;
		}
		set
		{
			if (!value)
			{
				foreach (int key in profiles.Keys)
				{
					EndTimer((ProfileTypes)key);
				}
			}
			m_bEnabled = value;
			GenerateOutput();
		}
	}

	public Profilehelper()
	{
		m_bEnabled = false;
		profiles = new Dictionary<int, Stopwatch>();
		m_results = "";
		profiles[5] = new Stopwatch();
	}

	public void StartTimer(ProfileTypes t)
	{
		if (m_bEnabled)
		{
			if (profiles.ContainsKey((int)t))
			{
				profiles[(int)t].Start();
				return;
			}
			profiles[(int)t] = new Stopwatch();
			profiles[(int)t].Start();
		}
	}

	public void EndTimer(ProfileTypes t)
	{
		if (m_bEnabled && profiles.ContainsKey((int)t))
		{
			profiles[(int)t].Stop();
		}
	}

	public void GenerateOutput()
	{
		m_results = "";
		foreach (int key in profiles.Keys)
		{
			object results = m_results;
			m_results = string.Concat(results, key, ":", profiles[key].ElapsedMilliseconds, "\n");
		}
	}

	public string GetOutput()
	{
		return m_results;
	}
}
