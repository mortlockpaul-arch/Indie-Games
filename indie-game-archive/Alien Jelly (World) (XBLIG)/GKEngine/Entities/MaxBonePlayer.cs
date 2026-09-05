using System;
using System.Collections.Generic;
using Common;
using Microsoft.Xna.Framework;

namespace GKEngine.Entities;

public class MaxBonePlayer
{
	public static string DEFAULT_CLIP_NAME = "Take 001";

	public bool active;

	private MaxModel model;

	private AnimationClip clip;

	private TimeSpan currentTimeValue;

	private int currentKeyframe;

	private TimeSpan timeStart;

	private TimeSpan timeEnd;

	private bool loop = true;

	private float speed = 1f;

	public Matrix[] boneTransforms;

	public Matrix[] worldTransforms;

	public Matrix[] skinTransforms;

	private int boneCount;

	public AnimationClip CurrentClip => clip;

	public TimeSpan CurrentTime => currentTimeValue;

	public Matrix[] GetBoneTransforms()
	{
		return boneTransforms;
	}

	public Matrix[] GetWorldTransforms()
	{
		return worldTransforms;
	}

	public MaxBonePlayer(MaxModel oModel)
	{
		model = oModel;
		active = false;
		boneTransforms = new Matrix[model.bones.Count];
		worldTransforms = new Matrix[model.bones.Count];
		skinTransforms = new Matrix[model.bones.Count];
		boneCount = model.bones.Count;
	}

	public void SetClip(AnimationClip oClip)
	{
		clip = oClip;
		Reset();
	}

	public void Reset()
	{
		currentTimeValue = TimeSpan.Zero;
		currentKeyframe = 0;
		timeStart = TimeSpan.Zero;
		timeEnd = clip.Duration;
		loop = true;
		speed = 1f;
		for (int i = 0; i < boneCount; i++)
		{
			ref Matrix reference = ref boneTransforms[i];
			reference = model.bones[i].bind;
		}
	}

	public void GoToAndPlay(TimeSpan oTimeStart, TimeSpan oTimeEnd, bool xLoop, float xSpeed)
	{
		loop = xLoop;
		currentTimeValue = oTimeStart;
		currentKeyframe = 0;
		timeStart = oTimeStart;
		timeEnd = oTimeEnd;
		speed = xSpeed;
		active = true;
	}

	public void GoToAndStop(TimeSpan oTime)
	{
		loop = false;
		currentTimeValue = oTime;
		currentKeyframe = 0;
		timeStart = oTime;
		timeEnd = oTime;
		speed = 1f;
		active = true;
		Update(oTime);
	}

	public void Stop()
	{
		active = false;
	}

	public void Continue()
	{
		active = true;
	}

	public void Update(TimeSpan oTime)
	{
		if (active)
		{
			UpdateBoneTransforms(oTime);
			UpdateWorldTransforms(model.parent.matrix);
			UpdateSkinTransforms();
		}
	}

	public void UpdateBoneTransforms(TimeSpan oTime)
	{
		currentTimeValue += new TimeSpan((long)((float)oTime.Ticks * speed));
		if (currentTimeValue >= timeEnd)
		{
			if (loop)
			{
				currentTimeValue = timeStart + new TimeSpan((currentTimeValue.Ticks - timeStart.Ticks) % (timeEnd.Ticks - timeStart.Ticks));
				currentKeyframe = 0;
			}
			else
			{
				currentTimeValue = timeEnd;
				active = false;
			}
		}
		IList<Keyframe> keyframes = clip.Keyframes;
		int count = keyframes.Count;
		while (currentKeyframe < count)
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
		ref Matrix reference = ref worldTransforms[0];
		reference = boneTransforms[0] * rootTransform;
		for (int i = 1; i < boneCount; i++)
		{
			int parentIndex = model.bones[i].parentIndex;
			ref Matrix reference2 = ref worldTransforms[i];
			reference2 = Matrix.Multiply(boneTransforms[i], worldTransforms[parentIndex]);
		}
	}

	public void UpdateSkinTransforms()
	{
		for (int i = 0; i < boneCount; i++)
		{
			ref Matrix reference = ref skinTransforms[i];
			reference = Matrix.Multiply(model.bones[i].inverse, worldTransforms[i]);
		}
	}
}
