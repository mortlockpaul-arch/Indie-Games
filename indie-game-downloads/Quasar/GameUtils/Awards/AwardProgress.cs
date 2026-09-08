using System;
using System.Xml.Linq;
using Quasar.Global;

namespace Quasar.GameUtils.Awards;

public class AwardProgress
{
	private Award award;

	private uint progress;

	private bool savePending;

	private DateTime unlockDate;

	public Award Award => award;

	public uint Progress => progress;

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

	public DateTime UnlockDate => unlockDate;

	public bool IsUnlocked => progress >= award.ProgressNeeded;

	protected uint NotifyLevel => progress / award.ProgressIncrement;

	public float Percentage => (float)progress / (float)award.ProgressNeeded;

	public AwardResult IncProgress(uint progress)
	{
		if (IsUnlocked)
		{
			return AwardResult.None;
		}
		if (progress == 0)
		{
			return AwardResult.None;
		}
		uint notifyLevel = NotifyLevel;
		this.progress += progress;
		savePending = true;
		if (IsUnlocked)
		{
			unlockDate = DateTime.Today;
			return AwardResult.Unlocked;
		}
		if (!award.IsSecret && NotifyLevel > notifyLevel)
		{
			return AwardResult.Notify;
		}
		return AwardResult.None;
	}

	public AwardResult Unlock()
	{
		if (IsUnlocked)
		{
			return AwardResult.None;
		}
		unlockDate = DateTime.Today;
		progress = award.ProgressNeeded;
		savePending = true;
		return AwardResult.Unlocked;
	}

	public void Relock()
	{
		if (IsUnlocked)
		{
			progress = 0u;
			savePending = true;
		}
	}

	public AwardProgress(Award award)
	{
		this.award = award;
	}

	public void FromXml(XElement xe)
	{
		progress = xe.ParseUIntAttribute("progress", 0u);
		unlockDate = xe.ParseDateAttribute("unlock_date");
	}

	public void ToXml(XElement xe)
	{
		xe.SetUIntAttribute("progress", progress);
		xe.SetDateAttribute("unlock_date", unlockDate);
	}
}
