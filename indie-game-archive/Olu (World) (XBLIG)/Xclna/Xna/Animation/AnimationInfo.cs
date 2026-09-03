using System.Collections.ObjectModel;

namespace Xclna.Xna.Animation;

public class AnimationInfo
{
	private long duration = 0L;

	private string animationName;

	private AnimationChannelCollection boneAnimations;

	public AnimationChannelCollection AnimationChannels => boneAnimations;

	public ReadOnlyCollection<string> AffectedBones => boneAnimations.AffectedBones;

	public long Duration => duration;

	public string Name => animationName;

	internal AnimationInfo(string animationName, AnimationChannelCollection anims)
	{
		this.animationName = animationName;
		boneAnimations = anims;
		foreach (BoneKeyframeCollection anim in anims)
		{
			if (anim.Duration > duration)
			{
				duration = anim.Duration;
			}
		}
	}

	public bool AffectsBone(string boneName)
	{
		return boneAnimations.AffectsBone(boneName);
	}
}
