using System;
using DataContent;
using SkinnedModel;

namespace EGEngine;

public class AnimationEventArgs : EventArgs
{
	public bool StartedNewAnimation;

	public int AnimationPlayerIndex;

	public WeaponAnim CurrentAnimation;

	public AnimationType CurrentAnimationType;

	public EventHandler<AnimationEventArgs> NewHandler;

	public bool ValidateEvent(AnimationPlayer animPlayer, WeaponAnim curAnim, int playerIndex, AnimationType animType, AnimationStateFlags flags)
	{
		if ((animPlayer.CurrentClip.AnimFlag & AnimFlag.AF_ONEOFF) > AnimFlag.AF_CLEAR && (animPlayer.AnimStateFlags & flags) == 0)
		{
			animPlayer.AnimStateFlags |= flags;
			CurrentAnimation = curAnim;
			AnimationPlayerIndex = playerIndex;
			CurrentAnimationType = animType;
			StartedNewAnimation = false;
			NewHandler = null;
			return true;
		}
		return false;
	}
}
