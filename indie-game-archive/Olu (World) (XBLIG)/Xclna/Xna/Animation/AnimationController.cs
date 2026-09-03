using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace Xclna.Xna.Animation;

public class AnimationController : GameComponent, IAnimationController
{
	private AnimationInfo animation;

	private double speedFactor = 1.0;

	private long elapsedTime = 0L;

	private long elapsed;

	private bool isLooping = true;

	public bool IsLooping
	{
		get
		{
			return isLooping;
		}
		set
		{
			isLooping = value;
		}
	}

	public long Duration => animation.Duration;

	public AnimationInfo AnimationSource => animation;

	public long ElapsedTime
	{
		get
		{
			return elapsedTime;
		}
		set
		{
			if (value < 0 || value > animation.Duration)
			{
				throw new ArgumentOutOfRangeException("ElapsedTime", "When setting the ElapsedTime for an animation, the value  must be between 0 and the animation duration.");
			}
			elapsedTime = value;
		}
	}

	public double SpeedFactor
	{
		get
		{
			return speedFactor;
		}
		set
		{
			speedFactor = value;
		}
	}

	public event EventHandler AnimationEnded;

	public event EventHandler AnimationTracksChanged;

	public AnimationController(Game game, AnimationInfo sourceAnimation)
		: this(game, sourceAnimation, component: false)
	{
	}

	public AnimationController(Game game, AnimationInfo sourceAnimation, bool component)
		: base(game)
	{
		animation = sourceAnimation;
		((GameComponent)this).UpdateOrder = 0;
		if (component)
		{
			((Collection<IGameComponent>)(object)game.Components).Add((IGameComponent)(object)this);
		}
	}

	protected virtual void OnAnimationEnded(EventArgs args)
	{
		if (AnimationEnded != null)
		{
			AnimationEnded(this, args);
		}
	}

	public override void Update(GameTime gameTime)
	{
		elapsed = (long)(speedFactor * (double)gameTime.ElapsedGameTime.Ticks);
		if (isLooping)
		{
			if (elapsed != 0)
			{
				elapsedTime += elapsed;
				if (elapsedTime > animation.Duration)
				{
					OnAnimationEnded(null);
					elapsedTime %= animation.Duration + 1;
				}
			}
		}
		else if (elapsedTime != animation.Duration && elapsed != 0)
		{
			elapsedTime += elapsed;
			if (elapsedTime >= animation.Duration || elapsedTime < 0)
			{
				elapsedTime = animation.Duration;
				OnAnimationEnded(null);
			}
		}
	}

	public virtual Matrix GetCurrentBoneTransform(BonePose pose)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		AnimationChannelCollection animationChannels = animation.AnimationChannels;
		BoneKeyframeCollection boneKeyframeCollection = animationChannels[pose.Name];
		int indexByTime = boneKeyframeCollection.GetIndexByTime(elapsedTime);
		return boneKeyframeCollection[indexByTime].Transform;
	}

	public bool ContainsAnimationTrack(BonePose pose)
	{
		return animation.AnimationChannels.AffectsBone(pose.Name);
	}

	protected virtual void OnAnimationTracksChanged(EventArgs e)
	{
		if (AnimationTracksChanged != null)
		{
			AnimationTracksChanged(this, e);
		}
	}
}
