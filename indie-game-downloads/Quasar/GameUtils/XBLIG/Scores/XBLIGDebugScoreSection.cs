using System;
using Quasar.GameUtils.Scores;
using Quasar.GameUtils.Sections;

namespace Quasar.GameUtils.XBLIG.Scores;

public class XBLIGDebugScoreSection<T> : ExtraSection where T : Highscore, IComparable<T>, IEquatable<T>, new()
{
	public static bool ReadyToUse => false;

	public XBLIGDebugScoreSection(XBLIGScoreManager<T> scoreManager)
		: base(Priority.Background)
	{
	}

	protected override void initScenes()
	{
	}
}
