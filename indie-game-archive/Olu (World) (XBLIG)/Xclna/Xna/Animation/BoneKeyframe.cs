using Microsoft.Xna.Framework;

namespace Xclna.Xna.Animation;

public struct BoneKeyframe
{
	public readonly Matrix Transform;

	public readonly long Time;

	public BoneKeyframe(Matrix transform, long time)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Transform = transform;
		Time = time;
	}
}
