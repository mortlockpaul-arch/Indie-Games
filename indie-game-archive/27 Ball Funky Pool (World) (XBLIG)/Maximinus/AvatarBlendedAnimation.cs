using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Maximinus;

public class AvatarBlendedAnimation
{
	private Matrix[] avatarBones = new Matrix[71];

	private ReadOnlyCollection<Matrix> boneTransforms;

	private AvatarAnimationAnyType currentAnimation;

	private AvatarAnimationAnyType targetAnimation;

	private TimeSpan blendTotalTime = new TimeSpan(0, 0, 0, 0, 250);

	private TimeSpan blendCurrentTime;

	private AvatarAnimationAnyType partialAnim;

	private List<int> partialAnimBoneIndexes;

	private bool partialAnimLoop;

	public ReadOnlyCollection<Matrix> BoneTransforms => boneTransforms;

	public AvatarExpression Expression => currentAnimation.Expression;

	public bool AlmostFinished => currentAnimation.Length - currentAnimation.CurrentPosition < blendTotalTime;

	public double PartialAnimPosSeconds => partialAnim.CurrentPosition.TotalSeconds;

	public double PartialAnimLength => partialAnim.Length.TotalSeconds;

	public AvatarBlendedAnimation()
		: this(-1)
	{
	}

	public AvatarBlendedAnimation(int blendTimeMilliseconds)
	{
		ChangeBlendTime(blendTimeMilliseconds);
		currentAnimation = null;
		boneTransforms = new ReadOnlyCollection<Matrix>(avatarBones);
	}

	public void Update(TimeSpan elapsedAnimationTime, bool loop)
	{
		currentAnimation.Update(elapsedAnimationTime, loop);
		if (partialAnim != null)
		{
			partialAnim.Update(elapsedAnimationTime, partialAnimLoop);
		}
		if (targetAnimation == null)
		{
			currentAnimation.BoneTransforms.CopyTo(avatarBones, 0);
			if (partialAnim != null)
			{
				for (int i = 0; i < partialAnimBoneIndexes.Count; i++)
				{
					ref Matrix reference = ref avatarBones[partialAnimBoneIndexes[i]];
					reference = partialAnim.BoneTransforms[partialAnimBoneIndexes[i]];
				}
			}
			return;
		}
		targetAnimation.Update(elapsedAnimationTime, loop);
		List<Matrix> list = new List<Matrix>(currentAnimation.BoneTransforms);
		if (partialAnim != null)
		{
			for (int j = 0; j < partialAnimBoneIndexes.Count; j++)
			{
				list[partialAnimBoneIndexes[j]] = partialAnim.BoneTransforms[partialAnimBoneIndexes[j]];
			}
		}
		ReadOnlyCollection<Matrix> readOnlyCollection = new ReadOnlyCollection<Matrix>(list);
		ReadOnlyCollection<Matrix> readOnlyCollection2 = targetAnimation.BoneTransforms;
		blendCurrentTime += elapsedAnimationTime;
		float num = (float)(blendCurrentTime.TotalSeconds / blendTotalTime.TotalSeconds);
		if (num >= 1f)
		{
			currentAnimation = targetAnimation;
			targetAnimation = null;
			partialAnim = null;
			num = 1f;
		}
		for (int k = 0; k < avatarBones.Length; k++)
		{
			Quaternion quaternion = Quaternion.CreateFromRotationMatrix(readOnlyCollection[k]);
			Quaternion quaternion2 = Quaternion.CreateFromRotationMatrix(readOnlyCollection2[k]);
			Quaternion.Slerp(ref quaternion, ref quaternion2, num, out var result);
			Vector3 value = readOnlyCollection[k].Translation;
			Vector3 value2 = readOnlyCollection2[k].Translation;
			Vector3.Lerp(ref value, ref value2, num, out var result2);
			ref Matrix reference2 = ref avatarBones[k];
			reference2 = Matrix.CreateFromQuaternion(result) * Matrix.CreateTranslation(result2);
		}
	}

	public void Play(AvatarAnimationAnyType nextAnimation)
	{
		if (currentAnimation == null)
		{
			currentAnimation = nextAnimation;
		}
		else if (currentAnimation != nextAnimation)
		{
			targetAnimation = nextAnimation;
			targetAnimation.CurrentPosition = TimeSpan.Zero;
			blendCurrentTime = TimeSpan.Zero;
		}
	}

	public void ChangeBlendTime(int blendTimeMilliseconds)
	{
		if (blendTimeMilliseconds != -1)
		{
			blendTotalTime = new TimeSpan(0, 0, 0, 0, blendTimeMilliseconds);
		}
	}

	public void OverwritePartialAnimation(AvatarAnimationAnyType partialAnim, List<int> partialAnimBoneIndexes, bool partialAnimLoop)
	{
		this.partialAnim = partialAnim;
		this.partialAnimBoneIndexes = partialAnimBoneIndexes;
		this.partialAnimLoop = partialAnimLoop;
	}
}
