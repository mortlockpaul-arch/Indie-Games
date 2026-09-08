using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Quasar.Meshes.SkinnedBodies;

public class KeyframeReader : ContentTypeReader<Keyframe>
{
	protected override Keyframe Read(ContentReader input, Keyframe existingInstance)
	{
		int bone = input.ReadObject<int>();
		TimeSpan time = input.ReadObject<TimeSpan>();
		Matrix transform = input.ReadObject<Matrix>();
		return new Keyframe(bone, time, transform);
	}
}
