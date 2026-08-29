using System;
using System.Collections.Generic;

namespace Common;

public class AnimationClip
{
	private TimeSpan durationValue;

	private IList<Keyframe> keyframesValue;

	public TimeSpan Duration => durationValue;

	public IList<Keyframe> Keyframes => keyframesValue;

	public AnimationClip(TimeSpan duration, IList<Keyframe> keyframes)
	{
		durationValue = duration;
		keyframesValue = keyframes;
	}
}
