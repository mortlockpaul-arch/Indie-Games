using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class CustomAvatarAnimationPlayer : CustomAvatarAnimationData
{
	private int currentKeyframe;

	private TimeSpan currentPosition = TimeSpan.Zero;

	private Matrix[] avatarBoneTransforms = new Matrix[71];

	public TimeSpan CurrentPosition
	{
		get
		{
			return currentPosition;
		}
		set
		{
			currentPosition = value;
			currentKeyframe = 0;
			Update(TimeSpan.Zero, loop: false);
		}
	}

	public IList<Matrix> BoneTransforms => avatarBoneTransforms;

	public CustomAvatarAnimationPlayer(string name, TimeSpan length, List<AvatarKeyFrame> keyframes)
		: base(name, length, keyframes)
	{
		for (int i = 0; i < 71; i++)
		{
			ref Matrix reference = ref avatarBoneTransforms[i];
			reference = Matrix.Identity;
		}
		Update(TimeSpan.Zero, loop: false);
	}

	public void Update(TimeSpan timeSpan, bool loop)
	{
		currentPosition += timeSpan;
		if (currentPosition > base.Length)
		{
			if (loop)
			{
				while (currentPosition > base.Length)
				{
					currentPosition -= base.Length;
				}
				currentKeyframe = 0;
			}
			else
			{
				currentPosition = base.Length;
			}
		}
		else if (currentPosition < TimeSpan.Zero)
		{
			if (loop)
			{
				while (currentPosition < TimeSpan.Zero)
				{
					currentPosition += base.Length;
				}
				currentKeyframe = base.Keyframes.Count - 1;
			}
			else
			{
				currentPosition = TimeSpan.Zero;
			}
		}
		UpdateBoneTransforms(timeSpan >= TimeSpan.Zero);
	}

	private void UpdateBoneTransforms(bool playingForward)
	{
		if (playingForward)
		{
			while (currentKeyframe < base.Keyframes.Count)
			{
				AvatarKeyFrame avatarKeyFrame = base.Keyframes[currentKeyframe];
				if (avatarKeyFrame.Time > currentPosition)
				{
					break;
				}
				ref Matrix reference = ref avatarBoneTransforms[avatarKeyFrame.Bone];
				reference = avatarKeyFrame.Transform;
				currentKeyframe++;
			}
			return;
		}
		while (currentKeyframe >= 0)
		{
			AvatarKeyFrame avatarKeyFrame2 = base.Keyframes[currentKeyframe];
			if (avatarKeyFrame2.Time < currentPosition)
			{
				break;
			}
			ref Matrix reference2 = ref avatarBoneTransforms[avatarKeyFrame2.Bone];
			reference2 = avatarKeyFrame2.Transform;
			currentKeyframe--;
		}
	}
}
