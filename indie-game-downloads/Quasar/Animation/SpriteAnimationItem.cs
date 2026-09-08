using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;

namespace Quasar.Animation;

public class SpriteAnimationItem : RenderItem
{
	protected SpriteAnimationMesh mesh;

	private SpriteAnimationState currentAnimationState;

	private SpriteAnimationSet animationSet;

	private bool paused;

	private Timer timer = Timer.DefaultTimer;

	public new SpriteAnimationMesh Mesh => mesh;

	public bool PauseAnimation
	{
		get
		{
			return paused;
		}
		set
		{
			paused = value;
		}
	}

	public Timer Timer
	{
		get
		{
			return timer;
		}
		set
		{
			timer = value;
		}
	}

	public Vector2 Size
	{
		get
		{
			return mesh.Size;
		}
		set
		{
			mesh.Size = value;
		}
	}

	public SpriteAnimationItem(SpriteAnimationSet animationSet, Texture texture, Vector2 size)
	{
		this.animationSet = animationSet;
		mesh = new SpriteAnimationMesh(animationSet, texture, size);
		addMesh(mesh);
		currentAnimationState = new SpriteAnimationState(animationSet.DefaultAnimation);
	}

	public void forceAnimation(string animation)
	{
		currentAnimationState = new SpriteAnimationState(animationSet.GetAnimation(animation));
	}

	public void startAnimation(string animation)
	{
		if (animation != null)
		{
			SpriteAnimation animation2 = animationSet.GetAnimation(animation);
			if (animation2 != null && currentAnimationState.Id != animation2.Id && (currentAnimationState.Loop || animation2.Priority >= currentAnimationState.Priority || currentAnimationState.Finished))
			{
				currentAnimationState = new SpriteAnimationState(animation2);
			}
		}
	}

	public bool HasAnimation(string animation)
	{
		return animationSet.GetAnimation(animation) != null;
	}

	protected override void DoUpdate()
	{
		currentAnimationState.Update((!paused) ? timer.LastInterval : 0);
		mesh.UpdateAnimation(ref currentAnimationState);
		base.DoUpdate();
	}
}
