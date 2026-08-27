using System;
using Microsoft.Xna.Framework;

namespace SkinnedModel;

public class AnimationPlayer
{
	private TimeSpan elapsedTime = new TimeSpan(167000L);

	private TimeSpan currentTimeValue;

	private AnimationClip currentClipValue;

	private int currentKeyframe;

	private int nextKeyFrame;

	private int currentAnimation;

	private float currentInterpolator;

	public AnimationStateFlags AnimStateFlags;

	public bool BlendOutTimeReached;

	public bool EndReached;

	public Matrix[] boneTransforms;

	public Matrix[] worldTransforms;

	public Matrix[] skinTransforms;

	public UserTransformStruct[] ApplyUserTransform;

	public SkinningData skinningDataValue;

	public Matrix RootTransform;

	private TimeSpan eTime = TimeSpan.Zero;

	private static Matrix matUser;

	private static Matrix tmpMatNext = Matrix.Identity;

	private static Vector3 vecZero = Vector3.Zero;

	public AnimationClip CurrentClip => currentClipValue;

	public TimeSpan CurrentTime
	{
		get
		{
			return currentTimeValue;
		}
		set
		{
			currentTimeValue = value;
		}
	}

	public TimeSpan ElapsedTimeStep
	{
		get
		{
			return elapsedTime;
		}
		set
		{
			elapsedTime = value;
		}
	}

	public int CurrentAnimation
	{
		get
		{
			return currentAnimation;
		}
		set
		{
			currentAnimation = value;
		}
	}

	public AnimationPlayer(SkinningData skinningData)
	{
		if (skinningData == null)
		{
			throw new ArgumentNullException("skinningData");
		}
		skinningDataValue = skinningData;
		boneTransforms = new Matrix[skinningData.BindPose.Count];
		worldTransforms = new Matrix[skinningData.BindPose.Count];
		skinTransforms = new Matrix[skinningData.BindPose.Count];
		ApplyUserTransform = new UserTransformStruct[skinningData.BindPose.Count];
		for (int i = 0; i < skinningData.BindPose.Count; i++)
		{
			ApplyUserTransform[i].Valid = false;
			ApplyUserTransform[i].Transform = Matrix.Identity;
		}
	}

	public void StartClip(AnimationClip clip)
	{
		if (clip != null)
		{
			elapsedTime = clip.TimeStep;
		}
		currentClipValue = clip;
		currentTimeValue = TimeSpan.Zero;
		currentKeyframe = 1;
		AnimStateFlags = AnimationStateFlags.Clear;
		nextKeyFrame = 1;
		currentInterpolator = 0f;
		skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
		for (int i = 0; i < skinningDataValue.BindPose.Count; i++)
		{
			ApplyUserTransform[i].Valid = false;
		}
	}

	public void ReStartCurrentClip()
	{
		currentTimeValue = TimeSpan.Zero;
		currentKeyframe = 1;
		AnimStateFlags = AnimationStateFlags.Clear;
		nextKeyFrame = 1;
		currentInterpolator = 0f;
	}

	public void Update(TimeSpan time, bool relativeToCurrentTime, Matrix rootTransform)
	{
		RootTransform = rootTransform;
		UpdateBoneTransforms(elapsedTime, relativeToCurrentTime);
	}

	public void UpdateToSkin(TimeSpan time, bool relativeToCurrentTime, Matrix rootTransform)
	{
		RootTransform = rootTransform;
		UpdateBoneTransforms(time, relativeToCurrentTime);
		UpdateWorldTransforms(rootTransform);
		UpdateSkinTransforms();
	}

	public void UpdateTimeStep()
	{
		if (currentClipValue.Keyframes == null || currentClipValue == null)
		{
			return;
		}
		EndReached = false;
		BlendOutTimeReached = false;
		eTime = ElapsedTimeStep;
		eTime += currentTimeValue;
		if (eTime >= currentClipValue.BlendOutTime)
		{
			BlendOutTimeReached = true;
		}
		while (eTime >= currentClipValue.Duration)
		{
			EndReached = true;
			currentKeyframe = 0;
			eTime -= currentClipValue.Duration;
		}
		int i;
		for (i = currentKeyframe; i < currentClipValue.Keyframes.Count && eTime > currentClipValue.Keyframes[i][0].Time; i++)
		{
		}
		float num = 1f;
		float num2 = 1f;
		if (i < currentClipValue.Keyframes.Count)
		{
			if (i > 0)
			{
				currentKeyframe = i - 1;
				num = (currentClipValue.Keyframes[i][0].Time - currentClipValue.Keyframes[currentKeyframe][0].Time).Milliseconds;
				num2 = (eTime - currentClipValue.Keyframes[currentKeyframe][0].Time).Milliseconds;
			}
			else
			{
				currentKeyframe = currentClipValue.Keyframes.Count - 1;
				num = (currentClipValue.Duration - currentClipValue.Keyframes[currentKeyframe][0].Time).Milliseconds;
				num2 = (eTime - currentClipValue.Keyframes[currentKeyframe][0].Time).Milliseconds;
			}
		}
		else
		{
			i = 0;
			currentKeyframe = currentClipValue.Keyframes.Count - 1;
			num = (currentClipValue.Duration - currentClipValue.Keyframes[currentKeyframe][0].Time).Milliseconds;
			num2 = (eTime - currentClipValue.Keyframes[currentKeyframe][0].Time).Milliseconds;
		}
		float num3 = 1f;
		num3 = ((!(num <= num2)) ? (num2 / num) : 1f);
		currentInterpolator = num3;
		nextKeyFrame = i;
		currentTimeValue = eTime;
	}

	public void UpdateJustBoneTransforms(ref Matrix rootTransform)
	{
		RootTransform = rootTransform;
		for (int i = 0; i < boneTransforms.Length; i++)
		{
			if (currentKeyframe >= currentClipValue.Keyframes.Count)
			{
				break;
			}
			bool flag = true;
			tmpMatNext = Matrix.Identity;
			for (int j = 0; j < boneTransforms.Length; j++)
			{
				if (currentClipValue.Keyframes[currentKeyframe][i].Bone == currentClipValue.Keyframes[nextKeyFrame][j].Bone)
				{
					tmpMatNext = currentClipValue.Keyframes[nextKeyFrame][j].Transform;
					flag = false;
					break;
				}
			}
			if (flag)
			{
				ref Matrix reference = ref boneTransforms[currentClipValue.Keyframes[currentKeyframe][i].Bone];
				reference = currentClipValue.Keyframes[currentKeyframe][i].Transform;
			}
			else
			{
				ref Matrix reference2 = ref boneTransforms[currentClipValue.Keyframes[currentKeyframe][i].Bone];
				reference2 = Matrix.Lerp(currentClipValue.Keyframes[currentKeyframe][i].Transform, tmpMatNext, currentInterpolator);
			}
			if (ApplyUserTransform[currentClipValue.Keyframes[currentKeyframe][i].Bone].Valid)
			{
				matUser = boneTransforms[currentClipValue.Keyframes[currentKeyframe][i].Bone];
				matUser.Translation = vecZero;
				matUser *= ApplyUserTransform[currentClipValue.Keyframes[currentKeyframe][i].Bone].Transform;
				matUser.Translation += boneTransforms[currentClipValue.Keyframes[currentKeyframe][i].Bone].Translation;
				ref Matrix reference3 = ref boneTransforms[currentClipValue.Keyframes[currentKeyframe][i].Bone];
				reference3 = matUser;
			}
		}
		UpdateWorldTransforms(rootTransform);
		UpdateSkinTransforms();
	}

	public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
	{
		if (currentClipValue == null || currentClipValue.Keyframes == null)
		{
			return;
		}
		EndReached = false;
		BlendOutTimeReached = false;
		time += currentTimeValue;
		if (time >= currentClipValue.BlendOutTime)
		{
			BlendOutTimeReached = true;
		}
		while (time >= currentClipValue.Duration)
		{
			EndReached = true;
			currentKeyframe = 0;
			time -= currentClipValue.Duration;
		}
		int i;
		for (i = currentKeyframe; i < currentClipValue.Keyframes.Count && time > currentClipValue.Keyframes[i][0].Time; i++)
		{
		}
		float num = 1f;
		float num2 = 1f;
		if (i < currentClipValue.Keyframes.Count)
		{
			if (i > 0)
			{
				currentKeyframe = i - 1;
				num = (currentClipValue.Keyframes[i][0].Time - currentClipValue.Keyframes[currentKeyframe][0].Time).Milliseconds;
				num2 = (time - currentClipValue.Keyframes[currentKeyframe][0].Time).Milliseconds;
			}
			else
			{
				currentKeyframe = currentClipValue.Keyframes.Count - 1;
				num = (currentClipValue.Duration - currentClipValue.Keyframes[currentKeyframe][0].Time).Milliseconds;
				num2 = (time - currentClipValue.Keyframes[currentKeyframe][0].Time).Milliseconds;
			}
		}
		else
		{
			i = 0;
			currentKeyframe = currentClipValue.Keyframes.Count - 1;
			num = (currentClipValue.Duration - currentClipValue.Keyframes[currentKeyframe][0].Time).Milliseconds;
			num2 = (time - currentClipValue.Keyframes[currentKeyframe][0].Time).Milliseconds;
		}
		float num3 = 1f;
		num3 = ((!(num <= num2)) ? (num2 / num) : 1f);
		currentTimeValue = time;
		if (currentKeyframe >= currentClipValue.Keyframes.Count)
		{
			return;
		}
		for (int j = 0; j < currentClipValue.Keyframes[currentKeyframe].Length && j < boneTransforms.Length; j++)
		{
			bool flag = true;
			tmpMatNext = Matrix.Identity;
			for (int k = 0; k < currentClipValue.Keyframes[currentKeyframe].Length && k < boneTransforms.Length; k++)
			{
				if (currentClipValue.Keyframes[currentKeyframe][j].Bone == currentClipValue.Keyframes[i][k].Bone)
				{
					tmpMatNext = currentClipValue.Keyframes[i][k].Transform;
					flag = false;
					break;
				}
			}
			if (currentClipValue.Keyframes[currentKeyframe][j].Bone < boneTransforms.Length)
			{
				if (flag)
				{
					ref Matrix reference = ref boneTransforms[currentClipValue.Keyframes[currentKeyframe][j].Bone];
					reference = currentClipValue.Keyframes[currentKeyframe][j].Transform;
				}
				else
				{
					ref Matrix reference2 = ref boneTransforms[currentClipValue.Keyframes[currentKeyframe][j].Bone];
					reference2 = Matrix.Lerp(currentClipValue.Keyframes[currentKeyframe][j].Transform, tmpMatNext, num3);
				}
				if (ApplyUserTransform[currentClipValue.Keyframes[currentKeyframe][j].Bone].Valid)
				{
					matUser = boneTransforms[currentClipValue.Keyframes[currentKeyframe][j].Bone];
					matUser.Translation = vecZero;
					matUser *= ApplyUserTransform[currentClipValue.Keyframes[currentKeyframe][j].Bone].Transform;
					matUser.Translation += boneTransforms[currentClipValue.Keyframes[currentKeyframe][j].Bone].Translation;
					ref Matrix reference3 = ref boneTransforms[currentClipValue.Keyframes[currentKeyframe][j].Bone];
					reference3 = matUser;
				}
			}
		}
	}

	public void UpdateWorldTransforms(Matrix rootTransform)
	{
		ref Matrix reference = ref worldTransforms[0];
		reference = boneTransforms[0] * rootTransform;
		for (int i = 1; i < worldTransforms.Length; i++)
		{
			int num = skinningDataValue.SkeletonHierarchy[i];
			ref Matrix reference2 = ref worldTransforms[i];
			reference2 = boneTransforms[i] * worldTransforms[num];
		}
	}

	public void UpdateSkinTransforms()
	{
		for (int i = 0; i < skinTransforms.Length; i++)
		{
			ref Matrix reference = ref skinTransforms[i];
			reference = skinningDataValue.InverseBindPose[i] * worldTransforms[i];
		}
	}

	public Matrix[] GetBoneTransforms()
	{
		return boneTransforms;
	}

	public Matrix[] GetWorldTransforms()
	{
		return worldTransforms;
	}

	public Matrix[] GetSkinTransforms()
	{
		return skinTransforms;
	}
}
