using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar.Meshes.SkinnedBodies;

public class AnimationPlayer
{
	private TimeSpan currentTimeValue;

	private int currentKeyframe;

	private bool interpolate;

	private AnimationClip clip;

	private List<AnimationClip> clipQueue;

	private Matrix[] boneTransforms;

	private Matrix[] worldTransforms;

	private Matrix[] skinTransforms;

	private int[] keyframeTimes;

	private bool[] keyframesInterpolated;

	private SkinningData skinningDataValue;

	public bool Loop
	{
		get
		{
			if (clip != null)
			{
				return clip.Loop;
			}
			return false;
		}
		set
		{
			if (clip != null)
			{
				clip.Loop = value;
			}
		}
	}

	public AnimationClip CurrentClip => clip;

	public AnimationClip FirstClip
	{
		get
		{
			if (clipQueue.Count <= 0)
			{
				return clip;
			}
			return clipQueue[0];
		}
	}

	public TimeSpan CurrentTime => currentTimeValue;

	public bool Interpolate => interpolate;

	public AnimationPlayer(SkinningData skinningData, bool interpolate)
	{
		if (skinningData == null)
		{
			throw new ArgumentNullException("skinningData");
		}
		skinningDataValue = skinningData;
		boneTransforms = new Matrix[skinningData.BindPose.Count];
		worldTransforms = new Matrix[skinningData.BindPose.Count];
		skinTransforms = new Matrix[skinningData.BindPose.Count];
		this.interpolate = interpolate;
		if (interpolate)
		{
			keyframeTimes = new int[skinningData.BindPose.Count];
			keyframesInterpolated = new bool[skinningData.BindPose.Count];
		}
		clipQueue = new List<AnimationClip>();
	}

	public AnimationPlayer(AnimationPlayer animationPlayer)
	{
		skinningDataValue = animationPlayer.skinningDataValue;
		boneTransforms = new Matrix[skinningDataValue.BindPose.Count];
		worldTransforms = new Matrix[skinningDataValue.BindPose.Count];
		skinTransforms = new Matrix[skinningDataValue.BindPose.Count];
		interpolate = animationPlayer.interpolate;
		if (interpolate)
		{
			keyframeTimes = new int[skinningDataValue.BindPose.Count];
			keyframesInterpolated = new bool[skinningDataValue.BindPose.Count];
		}
		clipQueue = new List<AnimationClip>();
		foreach (AnimationClip item in animationPlayer.clipQueue)
		{
			clipQueue.Add(item);
		}
		clip = animationPlayer.clip;
		currentKeyframe = animationPlayer.currentKeyframe;
		currentTimeValue = animationPlayer.currentTimeValue;
		for (int i = 0; i < skinningDataValue.BindPose.Count; i++)
		{
			ref Matrix reference = ref boneTransforms[i];
			reference = animationPlayer.boneTransforms[i];
			ref Matrix reference2 = ref worldTransforms[i];
			reference2 = animationPlayer.worldTransforms[i];
			ref Matrix reference3 = ref skinTransforms[i];
			reference3 = animationPlayer.skinTransforms[i];
			if (interpolate)
			{
				keyframeTimes[i] = animationPlayer.keyframeTimes[i];
				keyframesInterpolated[i] = animationPlayer.keyframesInterpolated[i];
			}
		}
	}

	public bool EnqueueClip(AnimationClip clip)
	{
		if (clip != null)
		{
			if (clipQueue.Count > 0 && clipQueue[clipQueue.Count - 1].Loop)
			{
				return false;
			}
			clipQueue.Add(clip);
			return true;
		}
		return StartClip(clip, 0f);
	}

	public bool StartClip(AnimationClip clip, float timeOffset)
	{
		if (clip == null)
		{
			throw new ArgumentNullException("StartClip: clip param is null");
		}
		if (clip == this.clip)
		{
			return false;
		}
		clipQueue.Clear();
		this.clip = clip;
		initCurrentClip(timeOffset);
		return true;
	}

	private void initCurrentClip(float timeOffset)
	{
		if (!clip.Loop)
		{
			currentTimeValue = TimeSpan.Zero;
		}
		else
		{
			currentTimeValue = TimeSpan.FromSeconds(timeOffset);
		}
		currentKeyframe = 0;
		skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
		if (interpolate)
		{
			for (int i = 0; i < keyframesInterpolated.Length; i++)
			{
				keyframeTimes[i] = -1;
			}
		}
	}

	public void Update(TimeSpan time, bool relativeToCurrentTime)
	{
		UpdateBoneTransforms(time, relativeToCurrentTime);
		UpdateWorldTransforms(Matrix.Identity);
		UpdateSkinTransforms();
	}

	public void Update(TimeSpan time, bool relativeToCurrentTime, Matrix rootTransform)
	{
		if (UpdateBoneTransforms(time, relativeToCurrentTime))
		{
			UpdateWorldTransforms(rootTransform);
			UpdateSkinTransforms();
		}
	}

	public bool UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
	{
		if (clip == null)
		{
			return false;
		}
		bool result = false;
		if (relativeToCurrentTime)
		{
			time += currentTimeValue;
			if (time >= clip.Duration)
			{
				if (clip.Loop)
				{
					while (time >= clip.Duration)
					{
						time -= clip.Duration;
					}
				}
				else if (clipQueue.Count > 0)
				{
					clip = clipQueue[0];
					clipQueue.RemoveAt(0);
					initCurrentClip(0f);
					result = true;
					time = TimeSpan.Zero;
				}
				else
				{
					time = clip.Duration;
				}
			}
		}
		if (time < TimeSpan.Zero || time > clip.Duration)
		{
			throw new ArgumentOutOfRangeException("time");
		}
		if (time < currentTimeValue)
		{
			currentKeyframe = 0;
			skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
			if (interpolate)
			{
				for (int i = 0; i < keyframesInterpolated.Length; i++)
				{
					keyframeTimes[i] = -1;
				}
			}
			result = true;
		}
		currentTimeValue = time;
		IList<Keyframe> keyframes = clip.Keyframes;
		int num = currentKeyframe;
		while (currentKeyframe < keyframes.Count)
		{
			Keyframe keyframe = keyframes[currentKeyframe];
			if (keyframe.Time > currentTimeValue)
			{
				break;
			}
			if (interpolate)
			{
				keyframeTimes[keyframe.Bone] = currentKeyframe;
			}
			ref Matrix reference = ref boneTransforms[keyframe.Bone];
			reference = keyframe.Transform;
			currentKeyframe++;
		}
		if (currentKeyframe != num)
		{
			result = true;
		}
		if (interpolate && time != TimeSpan.Zero)
		{
			for (int j = 0; j < keyframesInterpolated.Length; j++)
			{
				keyframesInterpolated[j] = false;
			}
			int num2 = 0;
			int num3 = currentKeyframe;
			while (num3 < keyframes.Count)
			{
				Keyframe keyframe2 = keyframes[num3];
				if (keyframesInterpolated[keyframe2.Bone])
				{
					num3++;
					continue;
				}
				int num4 = keyframeTimes[keyframe2.Bone];
				if (num4 >= 0)
				{
					Keyframe keyframe3 = keyframes[num4];
					TimeSpan timeSpan = keyframe2.Time.Subtract(keyframe3.Time);
					float amount = (float)time.Subtract(keyframe3.Time).TotalSeconds / (float)timeSpan.TotalSeconds;
					ref Matrix reference2 = ref boneTransforms[keyframe2.Bone];
					reference2 = Matrix.Lerp(keyframe3.Transform, keyframe2.Transform, amount);
				}
				else
				{
					float amount2 = (float)time.TotalSeconds / (float)keyframe2.Time.TotalSeconds;
					ref Matrix reference3 = ref boneTransforms[keyframe2.Bone];
					reference3 = Matrix.Lerp(skinningDataValue.BindPose[keyframe2.Bone], keyframe2.Transform, amount2);
				}
				keyframesInterpolated[keyframe2.Bone] = true;
				num2++;
				result = true;
				if (num2 == keyframesInterpolated.Length)
				{
					break;
				}
				num3++;
			}
		}
		return result;
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
