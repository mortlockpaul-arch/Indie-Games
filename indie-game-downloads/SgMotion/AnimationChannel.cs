using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SgMotion;

public class AnimationChannel : ReadOnlyCollection<AnimationChannelKeyframe>
{
	public AnimationChannel(IList<AnimationChannelKeyframe> list)
		: base(list)
	{
	}

	public int GetKeyframeIndexByTime(TimeSpan time)
	{
		if (base.Count == 0)
		{
			throw new InvalidOperationException("empty channel");
		}
		int num = 0;
		int num2 = 0;
		int num3 = base.Items.Count - 1;
		while (num3 >= num2)
		{
			num = (num2 + num3) / 2;
			if (base.Items[num].Time < time)
			{
				num2 = num + 1;
				continue;
			}
			if (!(base.Items[num].Time > time))
			{
				break;
			}
			num3 = num - 1;
		}
		if (base.Items[num].Time > time)
		{
			num--;
		}
		return num;
	}

	public AnimationChannelKeyframe GetKeyframeByTime(TimeSpan time)
	{
		int keyframeIndexByTime = GetKeyframeIndexByTime(time);
		return base.Items[keyframeIndexByTime];
	}
}
