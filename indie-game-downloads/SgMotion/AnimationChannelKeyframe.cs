using System;

namespace SgMotion;

public struct AnimationChannelKeyframe
{
	private readonly TimeSpan time;

	private readonly Pose pose;

	public TimeSpan Time => time;

	public Pose Pose => pose;

	public AnimationChannelKeyframe(TimeSpan time, Pose pose)
	{
		this.time = time;
		this.pose = pose;
	}
}
