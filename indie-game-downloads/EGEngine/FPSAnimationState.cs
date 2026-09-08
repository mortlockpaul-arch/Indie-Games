using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;

namespace EGEngine;

public class FPSAnimationState
{
	public string Name;

	public AnimFlag Flags;

	public bool BlendOverRide;

	public float BlendInTime;

	public TimeSpan BlendOutTime;

	public float Speed;

	public float FOV;

	public bool Sighted;

	public AnimationType AnimType;

	public AnimationClip Clip;

	public Texture2D AnimationTexture;

	public List<AnimationKeyEvent> KeyEvents;

	public int[] BoneIndices;

	public FPSAnimationState()
	{
	}

	public FPSAnimationState(FPSAnimationState e)
	{
		Name = e.Name;
		Flags = e.Flags;
		BlendOverRide = e.BlendOverRide;
		BlendInTime = e.BlendInTime;
		BlendOutTime = e.BlendOutTime;
		Speed = e.Speed;
		FOV = e.FOV;
		Sighted = e.Sighted;
		AnimType = e.AnimType;
		Clip = e.Clip;
		AnimationTexture = e.AnimationTexture;
		KeyEvents = e.KeyEvents;
	}

	public FPSAnimationState(string name, AnimFlag flag, bool bor, float bit, TimeSpan bot, float speed, float fov, bool sighted, AnimationType animtype)
	{
		Name = name;
		Flags = flag;
		BlendOverRide = bor;
		BlendInTime = bit;
		BlendOutTime = bot;
		Speed = speed;
		FOV = fov;
		Sighted = sighted;
		AnimType = animtype;
		Clip = null;
		AnimationTexture = null;
		KeyEvents = new List<AnimationKeyEvent>();
	}

	public void SetAnimationKey(int frame, EventHandler<AnimationEventArgs> cbMethod)
	{
		if (Clip != null)
		{
			AnimationKeyEvent animationKeyEvent = new AnimationKeyEvent();
			_ = Clip.Duration.TotalMilliseconds / 63.0;
			animationKeyEvent.Set(frame, new TimeSpan((int)((float)frame * 345833.34f)), cbMethod);
			KeyEvents.Add(animationKeyEvent);
		}
	}

	public void UpdateKeyEvents(TimeSpan currentTime)
	{
		foreach (AnimationKeyEvent keyEvent in KeyEvents)
		{
			keyEvent.Update(currentTime);
		}
	}
}
