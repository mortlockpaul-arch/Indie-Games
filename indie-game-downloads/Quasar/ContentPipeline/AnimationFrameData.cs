using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar.ContentPipeline;

public class AnimationFrameData
{
	public List<Matrix> BoneTransforms = new List<Matrix>();

	public TimeSpan FramePosition;
}
