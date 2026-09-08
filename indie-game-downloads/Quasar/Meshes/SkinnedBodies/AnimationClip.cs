using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Quasar.Meshes.SkinnedBodies;

public class AnimationClip
{
	public bool Loop { get; set; }

	[ContentSerializer]
	public TimeSpan Duration { get; private set; }

	[ContentSerializer]
	public List<Keyframe> Keyframes { get; private set; }

	public AnimationClip(TimeSpan duration, List<Keyframe> keyframes, bool loop)
	{
		Duration = duration;
		Keyframes = keyframes;
		Loop = loop;
	}

	private AnimationClip()
	{
	}
}
