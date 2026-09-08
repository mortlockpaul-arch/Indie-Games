using System;
using System.Xml.Linq;
using Quasar.Global;

namespace Quasar.GameUtils.Stats;

public class StatProgress
{
	private Stat stat;

	private long progress;

	private int count;

	private bool savePending;

	public Stat Stat => stat;

	public long Progress => progress;

	public int Count => count;

	public bool SavePending
	{
		get
		{
			return savePending;
		}
		set
		{
			savePending = value;
		}
	}

	public void IncProgress(long progress)
	{
		if (!stat.IsMax)
		{
			this.progress += progress;
			if (stat.IsAverage)
			{
				count++;
			}
			savePending = true;
		}
	}

	public void IncProgress(long progress, int count)
	{
		if (stat.IsAverage)
		{
			this.progress += progress;
			this.count += count;
			savePending = true;
		}
	}

	public void SetProgress(long progress)
	{
		if (stat.IsMax)
		{
			this.progress = Math.Max(this.progress, progress);
			savePending = true;
		}
	}

	public StatProgress(Stat stat)
	{
		this.stat = stat;
	}

	public void FromXml(XElement xe)
	{
		progress = xe.ParseLongAttribute("progress", 0L);
		count = xe.ParseIntAttribute("count", 0);
	}

	public void ToXml(XElement xe)
	{
		xe.SetLongAttribute("progress", progress);
		xe.SetIntAttribute("count", count);
	}
}
