using System;
using Microsoft.XboxLive.Avatars.Internal.Assets;
using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Animations;

public class AvatarAnimation : Animation
{
	public int m_JointCount;

	public Pose[][] m_AvatarKeyframes;

	public Pose[][] m_CarraybleKeyframes;

	public AvatarExpression[] m_AvatarFacialAnimation;

	public AvatarGender m_bodyTypeMask;

	public AvatarGender Gender => m_bodyTypeMask;

	public bool HasCarryableKeyframes => m_CarraybleKeyframes != null;

	public int AvatarJointsCount
	{
		get
		{
			if (m_AvatarKeyframes == null)
			{
				return 0;
			}
			return m_AvatarKeyframes[0].Length;
		}
	}

	public int CarryableJointsCount
	{
		get
		{
			if (m_CarraybleKeyframes == null)
			{
				return 0;
			}
			return m_CarraybleKeyframes[0].Length;
		}
	}

	public AvatarAnimation(Pose[][] avatarPoses, Pose[][] carryablePoses, AvatarExpression[] facialExp, float fps, AvatarGender bodyTypeMask)
		: base(avatarPoses.Length, fps)
	{
		m_bodyTypeMask = bodyTypeMask;
		m_AvatarKeyframes = avatarPoses;
		m_CarraybleKeyframes = carryablePoses;
		m_JointCount = avatarPoses[0].Length;
		m_AvatarFacialAnimation = facialExp;
	}

	public static AnimationCursor InitializeCursor(float speed, AnimationPlayMode animationPlayMode)
	{
		return new AnimationCursor(speed, animationPlayMode);
	}

	public float GetLoopPhase(AnimationCursor cursor)
	{
		return cursor.Time % base.Length;
	}

	public void GetSkeletonPose(Pose[][] keyframes, AnimationCursor cursor, float blendWeight, Pose[] jointBuffer)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 1f - blendWeight;
		int num4 = 0;
		int num5 = 0;
		if (cursor.PlayMode == AnimationPlayMode.Once)
		{
			float num6 = cursor.Time * base.Fps;
			num4 = (int)Math.Floor(num6);
			num2 = num6 - (float)num4;
			num = 1f - num2;
			num4 = Math.Min(num4, base.FrameCount - 1);
			num5 = Math.Min(num4 + 1, base.FrameCount - 1);
		}
		else if (cursor.PlayMode == AnimationPlayMode.Loop)
		{
			float num7 = cursor.Time * base.Fps;
			num4 = (int)Math.Floor(num7);
			num2 = num7 - (float)num4;
			num = 1f - num2;
			num4 %= base.FrameCount;
			num5 = (num4 + 1) % base.FrameCount;
		}
		else if (cursor.PlayMode == AnimationPlayMode.Bounce)
		{
			float num8 = cursor.Time * base.Fps;
			num4 = (int)Math.Floor(num8);
			num2 = num8 - (float)num4;
			num = 1f - num2;
			int num9 = num4 / base.FrameCount;
			if ((num9 & 1) == 0)
			{
				num4 %= base.FrameCount;
				num5 = Math.Min(num4 + 1, base.FrameCount - 1);
			}
			else
			{
				num4 = base.FrameCount - 1 - num4 % base.FrameCount;
				num5 = Math.Max(num4 - 1, 0);
			}
		}
		for (int i = 0; i < jointBuffer.Length && i < keyframes[num4].Length; i++)
		{
			Vector3 position = keyframes[num4][i].position;
			Vector3 position2 = keyframes[num5][i].position;
			Vector3 scale = keyframes[num4][i].scale;
			Vector3 scale2 = keyframes[num5][i].scale;
			Quaternion rotation = keyframes[num4][i].rotation;
			Quaternion rotation2 = keyframes[num5][i].rotation;
			position.X = num * position.X + num2 * position2.X;
			position.Y = num * position.Y + num2 * position2.Y;
			position.Z = num * position.Z + num2 * position2.Z;
			scale.X = num * scale.X + num2 * scale2.X;
			scale.Y = num * scale.Y + num2 * scale2.Y;
			scale.Z = num * scale.Z + num2 * scale2.Z;
			float num10 = rotation.X * rotation2.X + rotation.Y * rotation2.Y + rotation.Z * rotation2.Z + rotation.W * rotation2.W;
			if (num10 >= 0f)
			{
				rotation.X = num * rotation.X + num2 * rotation2.X;
				rotation.Y = num * rotation.Y + num2 * rotation2.Y;
				rotation.Z = num * rotation.Z + num2 * rotation2.Z;
				rotation.W = num * rotation.W + num2 * rotation2.W;
			}
			else
			{
				rotation.X = num * rotation.X - num2 * rotation2.X;
				rotation.Y = num * rotation.Y - num2 * rotation2.Y;
				rotation.Z = num * rotation.Z - num2 * rotation2.Z;
				rotation.W = num * rotation.W - num2 * rotation2.W;
			}
			float num11 = rotation.X * rotation.X + rotation.Y * rotation.Y + rotation.Z * rotation.Z + rotation.W * rotation.W;
			if (num11 > 0f)
			{
				num11 = (float)(1.0 / Math.Sqrt(num11));
				rotation.X *= num11;
				rotation.Y *= num11;
				rotation.Z *= num11;
				rotation.W *= num11;
			}
			if (blendWeight == 1f)
			{
				jointBuffer[i].position = position;
				jointBuffer[i].scale = scale;
				jointBuffer[i].rotation = rotation;
				continue;
			}
			position2 = jointBuffer[i].position;
			scale2 = jointBuffer[i].scale;
			rotation2 = jointBuffer[i].rotation;
			position2.X = blendWeight * position.X + num3 * position2.X;
			position2.Y = blendWeight * position.Y + num3 * position2.Y;
			position2.Z = blendWeight * position.Z + num3 * position2.Z;
			scale2.X = blendWeight * scale.X + num3 * scale2.X;
			scale2.Y = blendWeight * scale.Y + num3 * scale2.Y;
			scale2.Z = blendWeight * scale.Z + num3 * scale2.Z;
			num10 = rotation.X * rotation2.X + rotation.Y * rotation2.Y + rotation.Z * rotation2.Z + rotation.W * rotation2.W;
			if (num10 >= 0f)
			{
				rotation2.X = blendWeight * rotation.X + num3 * rotation2.X;
				rotation2.Y = blendWeight * rotation.Y + num3 * rotation2.Y;
				rotation2.Z = blendWeight * rotation.Z + num3 * rotation2.Z;
				rotation2.W = blendWeight * rotation.W + num3 * rotation2.W;
			}
			else
			{
				rotation2.X = blendWeight * rotation.X - num3 * rotation2.X;
				rotation2.Y = blendWeight * rotation.Y - num3 * rotation2.Y;
				rotation2.Z = blendWeight * rotation.Z - num3 * rotation2.Z;
				rotation2.W = blendWeight * rotation.W - num3 * rotation2.W;
			}
			num11 = rotation2.X * rotation2.X + rotation2.Y * rotation2.Y + rotation2.Z * rotation2.Z + rotation2.W * rotation2.W;
			if (num11 > 0f)
			{
				num11 = (float)(1.0 / Math.Sqrt(num11));
				rotation2.X *= num11;
				rotation2.Y *= num11;
				rotation2.Z *= num11;
				rotation2.W *= num11;
			}
			jointBuffer[i].position = position2;
			jointBuffer[i].scale = scale2;
			jointBuffer[i].rotation = rotation2;
		}
	}

	public bool GetCarryablePose(AnimationCursor cursor, float blendWeight, Pose[] jointBuffer)
	{
		if (m_CarraybleKeyframes == null)
		{
			return false;
		}
		GetSkeletonPose(m_CarraybleKeyframes, cursor, blendWeight, jointBuffer);
		return true;
	}

	public bool GetAvatarPose(AnimationCursor cursor, float blendWeight, Pose[] jointBuffer)
	{
		GetSkeletonPose(m_AvatarKeyframes, cursor, blendWeight, jointBuffer);
		return true;
	}

	public float[] GetCarryableMaxSkeletonScaling(Skeleton carryableSkeleton)
	{
		if (m_CarraybleKeyframes == null)
		{
			return null;
		}
		int num = m_CarraybleKeyframes[0].Length;
		float[] array = new float[num];
		float[] array2 = new float[num];
		for (int i = 0; i < m_CarraybleKeyframes.Length; i++)
		{
			for (int j = 0; j < num; j++)
			{
				Vector3 scale = m_CarraybleKeyframes[i][j].scale;
				array2[j] = scale.X;
				if (array2[j] < scale.Y)
				{
					array2[j] = scale.Y;
				}
				if (array2[j] < scale.Z)
				{
					array2[j] = scale.Z;
				}
				float num2 = carryableSkeleton.Joints[j].Local.scale.X;
				if (num2 < carryableSkeleton.Joints[j].Local.scale.Y)
				{
					num2 = carryableSkeleton.Joints[j].Local.scale.Y;
				}
				if (num2 < carryableSkeleton.Joints[j].Local.scale.Z)
				{
					num2 = carryableSkeleton.Joints[j].Local.scale.Z;
				}
				array2[j] *= num2;
				float num3 = ((j == 0) ? 1f : array2[carryableSkeleton.Joints[j].Parent]);
				array2[j] *= num3;
				if (array2[j] > array[j])
				{
					array[j] = array2[j];
				}
			}
		}
		return array;
	}

	public AvatarExpression GetAnimableTextureLayers(AnimationCursor cursor)
	{
		float num;
		if (cursor.PlayMode == AnimationPlayMode.Once)
		{
			num = cursor.Time;
		}
		else
		{
			num = cursor.Time % base.Length;
			if (cursor.PlayMode == AnimationPlayMode.Bounce)
			{
				int num2 = (int)(cursor.Time / base.Length);
				if ((num2 & 1) == 1)
				{
					num = base.Length - num;
				}
			}
		}
		int num3 = (int)(num * base.Fps);
		if (num3 >= base.FrameCount)
		{
			num3 = base.FrameCount - 1;
		}
		return m_AvatarFacialAnimation[num3];
	}

	public AvatarAnimation(int jointCount, int frameCount, int fps)
		: base(frameCount, fps)
	{
		m_JointCount = jointCount;
		m_AvatarKeyframes = new Pose[frameCount][];
		for (int i = 0; i < frameCount; i++)
		{
			m_AvatarKeyframes[i] = new Pose[m_JointCount];
		}
		m_AvatarFacialAnimation = new AvatarExpression[frameCount];
	}

	public void SetSkeletonPose(int joint, int frame, ref Pose pose)
	{
		ref Pose reference = ref m_AvatarKeyframes[frame][joint];
		reference = pose;
	}

	public void SetAvatarExpression(int frame, ref AvatarExpression expression)
	{
		ref AvatarExpression reference = ref m_AvatarFacialAnimation[frame];
		reference = expression;
	}

	public Pose[] GetKeyframe(int frame)
	{
		return m_AvatarKeyframes[frame];
	}

	public int GetMemoryUsage()
	{
		int num = 64;
		if (m_AvatarKeyframes != null)
		{
			num += m_AvatarKeyframes[0].Length * m_AvatarKeyframes.Length * 48;
		}
		if (m_CarraybleKeyframes != null)
		{
			num += m_CarraybleKeyframes[0].Length * m_CarraybleKeyframes.Length * 48;
		}
		if (m_AvatarFacialAnimation != null)
		{
			num += m_AvatarFacialAnimation.Length * 32;
		}
		return num;
	}
}
