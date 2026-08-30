using Microsoft.Xna.Framework;

namespace Skeleton;

public class JointState
{
	private float rotation;

	private Vector2 boneScale;

	private Vector2 offset;

	public float Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
		}
	}

	public Vector2 Scale
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return boneScale;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			boneScale = value;
		}
	}

	public Vector2 Offset
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return offset;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			offset = value;
		}
	}

	public JointState(float rot, Vector2 scale, Vector2 off)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		rotation = rot;
		boneScale = scale;
		offset = off;
	}

	public void Merge(JointState state2, float proportionalUsage, ref JointState resultState)
	{
		float num = 1f - proportionalUsage;
		resultState.rotation = rotation * proportionalUsage + state2.rotation * num;
		resultState.boneScale.X = boneScale.X * proportionalUsage + state2.boneScale.X * num;
		resultState.boneScale.Y = boneScale.Y * proportionalUsage + state2.boneScale.Y * num;
		resultState.offset.X = offset.X * proportionalUsage + state2.offset.X * num;
		resultState.offset.Y = offset.Y * proportionalUsage + state2.offset.Y * num;
	}
}
