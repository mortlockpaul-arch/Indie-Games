using System;
using Microsoft.Xna.Framework;

namespace Common;

public class Keyframe
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

	public static int CompareTimes(Keyframe a, Keyframe b)
	{
		return a.Time.CompareTo(b.Time);
	}
}
