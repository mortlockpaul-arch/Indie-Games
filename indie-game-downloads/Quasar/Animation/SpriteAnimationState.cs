using System;

namespace Quasar.Animation;

public struct SpriteAnimationState
{
	private SpriteAnimation currentAnimation;

	private bool flipX;

	private bool flipY;

	private int animationElapsed;

	public int CurrentFrame;

	public string Id
	{
		get
		{
			if (currentAnimation == null)
			{
				return "";
			}
			return currentAnimation.Id;
		}
	}

	public int Priority
	{
		get
		{
			if (currentAnimation == null)
			{
				return 0;
			}
			return currentAnimation.Priority;
		}
	}

	public bool FlipX => flipX;

	public bool FlipY => flipY;

	public bool Loop
	{
		get
		{
			if (currentAnimation != null)
			{
				return currentAnimation.Loop;
			}
			return true;
		}
	}

	public bool Finished
	{
		get
		{
			if (currentAnimation != null)
			{
				if (!currentAnimation.Loop)
				{
					return CurrentFrame == currentAnimation.Frames.Length - 1;
				}
				return false;
			}
			return true;
		}
	}

	public SpriteAnimationState(SpriteAnimation animation)
	{
		if (animation != null)
		{
			flipX = animation.FlipX;
			flipY = animation.FlipY;
		}
		else
		{
			flipX = (flipY = false);
		}
		currentAnimation = animation;
		animationElapsed = 0;
		CurrentFrame = animation.FirstFrame;
	}

	public SpriteAnimationState(SpriteAnimation animation, SpriteAnimation nextAnimation)
	{
		if (animation != null)
		{
			flipX = animation.FlipX;
			flipY = animation.FlipY;
		}
		else
		{
			flipX = (flipY = false);
		}
		currentAnimation = animation;
		animationElapsed = 0;
		CurrentFrame = animation.FirstFrame;
	}

	public void Update(int elapsedTime)
	{
		animationElapsed += elapsedTime;
		if (currentAnimation == null)
		{
			return;
		}
		if (animationElapsed >= currentAnimation.Length)
		{
			if (currentAnimation.Loop)
			{
				animationElapsed %= currentAnimation.Length;
			}
			else if (currentAnimation.NextAnimation != null)
			{
				SetAnimation(animationElapsed - currentAnimation.Length, currentAnimation.NextAnimation);
			}
			else
			{
				SetAnimation(0, null);
			}
		}
		if (currentAnimation != null && currentAnimation.Frames != null)
		{
			int num = Math.Min(animationElapsed / currentAnimation.Period, currentAnimation.Frames.Length - 1);
			CurrentFrame = currentAnimation.Frames[num];
		}
	}

	private void SetAnimation(int elapsedTime, SpriteAnimation animation)
	{
		animationElapsed = elapsedTime;
		if (animation != null)
		{
			flipX = animation.FlipX;
			flipY = animation.FlipY;
		}
		currentAnimation = animation;
	}
}
