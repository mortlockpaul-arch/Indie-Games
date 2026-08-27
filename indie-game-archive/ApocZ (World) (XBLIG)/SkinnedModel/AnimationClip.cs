using System;
using System.Collections.Generic;

namespace SkinnedModel;

public class AnimationClip
{
	private static TimeSpan tmpTS = new TimeSpan(0, 0, 0, 0, 100);

	public TimeSpan TimeStep = new TimeSpan(167000L);

	public bool BlendOverRide;

	public float BlendInTime;

	public TimeSpan BlendOutTime;

	public float fBlendOutTime;

	public float Speed;

	public AnimationType AnimType;

	public AnimFlag AnimFlag;

	public List<Keyframe[]> Keyframes;

	private TimeSpan durationValue;

	public TimeSpan Duration
	{
		get
		{
			return durationValue;
		}
		set
		{
			durationValue = value;
		}
	}

	public AnimationClip(TimeSpan duration, List<Keyframe[]> keyframes)
	{
		AnimType = AnimationType.Undefined;
		Duration = duration;
		Keyframes = keyframes;
		BlendInTime = 1f;
		BlendOutTime = Duration.Subtract(tmpTS);
		BlendOverRide = false;
		Speed = 0f;
	}

	private AnimationClip()
	{
	}
}
