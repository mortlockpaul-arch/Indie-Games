using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SgMotion.Controllers;

namespace Deep_waters;

public class CutScene
{
	public List<Actor> Actors;

	public Matrix bcpos;

	public Matrix bclook;

	public CutScene(List<Actor> actors, string intialAnim, bool isloop, float animspeed)
	{
		Actors = actors;
	}

	public void changeAnimation(string nameAnim, bool isloop, float animspeed)
	{
		foreach (Actor actor in Actors)
		{
			actor.animationController.LoopEnabled = isloop;
			actor.animationController.TranslationInterpolation = InterpolationMode.None;
			actor.animationController.OrientationInterpolation = InterpolationMode.None;
			actor.animationController.Speed = animspeed;
			actor.animationController.StartClip(actor.skinnedModel.AnimationClips[nameAnim]);
		}
	}
}
