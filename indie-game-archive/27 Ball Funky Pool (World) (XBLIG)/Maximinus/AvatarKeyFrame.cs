using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Maximinus;

public class AvatarKeyFrame
{
	[ContentSerializer]
	public int Bone { get; private set; }

	[ContentSerializer]
	public TimeSpan Time { get; private set; }

	[ContentSerializer]
	public Matrix Transform { get; private set; }

	private AvatarKeyFrame()
	{
	}

	public AvatarKeyFrame(int bone, TimeSpan time, Matrix transform)
	{
		Bone = bone;
		Time = time;
		Transform = transform;
	}
}
