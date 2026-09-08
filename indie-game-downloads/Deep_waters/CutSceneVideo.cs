using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class CutSceneVideo : CutScene
{
	public quicktime QTE;

	public Action bstate;

	public bool gotonextlevel;

	private int levelnumber;

	public Vector3 boss;

	public Vector3 head;

	public bool Splash;

	private bool spawnsplash;

	public CutSceneVideo(List<Actor> actors, string initialAnim, int leveln)
		: base(actors, initialAnim, isloop: false, 1f)
	{
		levelnumber = leveln;
	}

	public void update(GameTime gameTime)
	{
		if (bstate == Action.Intro && Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips["Intro"])
		{
			if (Actors.Count >= 3)
			{
				boss = Actors[2].animationController.GetBoneAbsoluteTransform("CATRigHub004Bone002").Translation * 0.05f;
			}
			if (Actors[1] != null)
			{
				head = Actors[1].animationController.GetBoneAbsoluteTransform("BaseNeck2").Translation * 0.05f;
			}
			if (levelnumber == 2 && !spawnsplash && Actors[0].animationController.Time > new TimeSpan(0, 0, 0, 20, 230))
			{
				Splash = true;
				spawnsplash = true;
			}
			if (levelnumber == 3 && !spawnsplash && Actors[0].animationController.Time > new TimeSpan(0, 0, 0, 4, 60))
			{
				Splash = true;
				spawnsplash = true;
			}
			bcpos = Actors[0].animationController.GetBoneAbsoluteTransform("campos");
			bclook = Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
			if (Actors[0].animationController.HasFinished)
			{
				bstate = Action.Null;
				gotonextlevel = true;
			}
		}
	}

	public void draw(SpriteBatch sb, GameTime gameTime)
	{
	}
}
