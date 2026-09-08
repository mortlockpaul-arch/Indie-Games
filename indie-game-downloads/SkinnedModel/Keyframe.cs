using System;
using Microsoft.Xna.Framework;

namespace SkinnedModel;

public struct Keyframe
{
	private int boneValue;

	private TimeSpan timeValue;

	private Matrix transformValue;

	public int Bone => boneValue;

	public TimeSpan Time => timeValue;

	public Matrix Transform => transformValue;

	public Keyframe(int bone, TimeSpan time, Matrix transform)
	{
		boneValue = bone;
		timeValue = time;
		transformValue = transform;
	}

	public void Set(int bone, TimeSpan time, Matrix transform)
	{
		boneValue = bone;
		timeValue = time;
		transformValue = transform;
	}
}
