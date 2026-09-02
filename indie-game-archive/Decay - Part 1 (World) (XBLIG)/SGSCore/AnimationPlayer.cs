using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SGSCore;

public class AnimationPlayer
{
	public enum ANIMATION_STATE
	{
		PLAYING,
		PAUSED,
		STOPPED
	}

	private AnimationClip currentClipValue;

	private TimeSpan currentTimeValue;

	private int currentKeyframe;

	private Matrix[] boneTransforms;

	private Matrix[] worldTransforms;

	private Matrix[] skinTransforms;

	private SkinningData skinningDataValue;

	public ANIMATION_STATE m_state = ANIMATION_STATE.STOPPED;

	public bool m_loop;

	public AnimationClip CurrentClip => currentClipValue;

	public TimeSpan CurrentTime => currentTimeValue;

	public AnimationPlayer(SkinningData skinningData)
	{
		if (skinningData == null)
		{
			throw new ArgumentNullException("skinningData");
		}
		skinningDataValue = skinningData;
		boneTransforms = (Matrix[])(object)new Matrix[skinningData.BindPose.Count];
		worldTransforms = (Matrix[])(object)new Matrix[skinningData.BindPose.Count];
		skinTransforms = (Matrix[])(object)new Matrix[skinningData.BindPose.Count];
	}

	public void StartClip(AnimationClip clip)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (clip == null)
		{
			throw new ArgumentNullException("clip");
		}
		currentClipValue = clip;
		currentTimeValue = TimeSpan.Zero;
		currentKeyframe = 0;
		skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
		UpdateBoneTransforms(default(TimeSpan), relativeToCurrentTime: true);
		UpdateWorldTransforms(Matrix.Identity);
		UpdateSkinTransforms();
		m_state = ANIMATION_STATE.PLAYING;
	}

	public void Stop()
	{
		m_state = ANIMATION_STATE.STOPPED;
	}

	public void Update(TimeSpan time, bool relativeToCurrentTime, Matrix rootTransform)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (currentClipValue != null && m_state == ANIMATION_STATE.PLAYING)
		{
			UpdateBoneTransforms(time, relativeToCurrentTime);
			UpdateWorldTransforms(rootTransform);
			UpdateSkinTransforms();
		}
	}

	public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
	{
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		if (currentClipValue == null)
		{
			throw new InvalidOperationException("AnimationPlayer.Update was called before StartClip");
		}
		if (relativeToCurrentTime)
		{
			time += currentTimeValue;
			if (time >= currentClipValue.Duration)
			{
				if (m_loop)
				{
					time -= currentClipValue.Duration;
				}
				else
				{
					time = currentClipValue.Duration - new TimeSpan(1L);
					m_state = ANIMATION_STATE.STOPPED;
				}
			}
		}
		if (time < TimeSpan.Zero || time >= currentClipValue.Duration)
		{
			throw new ArgumentOutOfRangeException("time");
		}
		if (time < currentTimeValue)
		{
			currentKeyframe = 0;
			skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
		}
		currentTimeValue = time;
		IList<Keyframe> keyframes = currentClipValue.Keyframes;
		while (currentKeyframe < keyframes.Count)
		{
			Keyframe keyframe = keyframes[currentKeyframe];
			if (keyframe.Time > currentTimeValue)
			{
				break;
			}
			ref Matrix reference = ref boneTransforms[keyframe.Bone];
			reference = keyframe.Transform;
			currentKeyframe++;
		}
	}

	public void UpdateWorldTransforms(Matrix rootTransform)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
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
