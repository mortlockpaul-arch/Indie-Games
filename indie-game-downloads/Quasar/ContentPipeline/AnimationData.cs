using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.GamerServices;

namespace Quasar.ContentPipeline;

public class AnimationData
{
	public TimeSpan Length;

	public AvatarAnimationPreset correspondingPreset;

	public List<AnimationFrameData> animationData = new List<AnimationFrameData>();

	public string Title;
}
