using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Quasar.Meshes.SkinnedBodies;

public class AnimationClipReader : ContentTypeReader<AnimationClip>
{
	protected override AnimationClip Read(ContentReader input, AnimationClip existingInstance)
	{
		TimeSpan duration = input.ReadObject<TimeSpan>();
		List<Keyframe> keyframes = input.ReadObject<List<Keyframe>>();
		bool loop = input.ReadBoolean();
		return new AnimationClip(duration, keyframes, loop);
	}
}
