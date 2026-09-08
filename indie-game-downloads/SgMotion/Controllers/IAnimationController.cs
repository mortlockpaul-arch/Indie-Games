using System;
using Microsoft.Xna.Framework;

namespace SgMotion.Controllers;

public interface IAnimationController
{
	AnimationClip AnimationClip { get; }

	TimeSpan Time { get; set; }

	float Speed { get; set; }

	bool LoopEnabled { get; set; }

	PlaybackMode PlaybackMode { get; set; }

	InterpolationMode TranslationInterpolation { get; set; }

	InterpolationMode OrientationInterpolation { get; set; }

	InterpolationMode ScaleInterpolation { get; set; }

	bool HasFinished { get; }

	bool IsPlaying { get; }

	Pose[] LocalBonePoses { get; }

	Matrix[] SkinnedBoneTransforms { get; }

	void StartClip(AnimationClip animationClip);

	[Obsolete("PlayClip has been replaced with SwitchToClip - please update your code")]
	void PlayClip(AnimationClip animationClip);

	void SwitchToClip(AnimationClip animationClip);

	void CrossFade(AnimationClip animationClip, TimeSpan fadeTime);

	void CrossFade(AnimationClip animationClip, TimeSpan fadeTime, InterpolationMode translationInterpolation, InterpolationMode orientationInterpolation, InterpolationMode scaleInterpolation);

	void Update(TimeSpan elapsedTime, Matrix parent);
}
