using System;
using System.Collections.Generic;
using Quasar.GameUtils.Scores;

namespace Quasar.GameUtils.XBLIG.Scores;

public class XBLIGScoreManager<T> : ScoreManager<T> where T : Highscore, IComparable<T>, IEquatable<T>, new()
{
	private bool enabled = true;

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
		}
	}

	public XBLIGScoreManager(KeyValuePair<int, int> identifier, ScoreOrganizer<T> organizer)
		: base(organizer)
	{
		storeLocalData = false;
		storeGlobalData = false;
		dataLoaded = true;
		supportsPeriods = true;
	}

	public override void Update()
	{
		if (enabled)
		{
			base.Update();
		}
	}
}
