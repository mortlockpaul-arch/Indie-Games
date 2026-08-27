using System;
using Microsoft.Xna.Framework;

namespace SkinnedModel;

public struct Keyframe(int bone, TimeSpan time, Matrix transform)
{
	private int boneValue = bone;

	private TimeSpan timeValue = time;

	private Matrix transformValue = transform;

	public int Bone => boneValue;

	public TimeSpan Time => timeValue;

	public Matrix Transform => transformValue;

	public void Set(int bone, TimeSpan time, Matrix transform)
	{
		boneValue = bone;
		timeValue = time;
		transformValue = transform;
	}
}
