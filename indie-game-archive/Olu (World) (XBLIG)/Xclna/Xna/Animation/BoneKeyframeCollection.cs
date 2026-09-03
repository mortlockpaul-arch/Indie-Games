using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xclna.Xna.Animation;

public class BoneKeyframeCollection : ReadOnlyCollection<BoneKeyframe>
{
	private string boneName;

	private long duration;

	public long Duration => duration;

	public string BoneName => boneName;

	internal BoneKeyframeCollection(string boneName, IList<BoneKeyframe> list)
		: base(list)
	{
		this.boneName = boneName;
		duration = list[list.Count - 1].Time;
	}

	public int GetIndexByTime(long ticks)
	{
		int i = (int)(ticks / 166666);
		if (i >= base.Count)
		{
			i = base.Count - 1;
		}
		for (; i < base.Count - 1 && base[i + 1].Time < ticks; i++)
		{
		}
		while (i >= 0 && base[i].Time > ticks)
		{
			i--;
		}
		return i;
	}
}
