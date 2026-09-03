using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace SGSCore;

public class AnimationClip
{
	[ContentSerializer]
	public TimeSpan Duration { get; private set; }

	[ContentSerializer]
	public List<Keyframe> Keyframes { get; private set; }

	public AnimationClip(TimeSpan duration, List<Keyframe> keyframes)
	{
		Duration = duration;
		Keyframes = keyframes;
	}

	private AnimationClip()
	{
	}
}
