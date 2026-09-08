using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SgMotion;
using SgMotion.Controllers;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace Deep_waters;

public class Shark : SceneObject
{
	public SkinnedModel skinnedModel;

	public AnimationController animationController;

	public Vector3 Position;

	public Vector3 Rotation;

	public float Scale;

	private float totalTime;

	public Behaviour behaviour;

	public float awareness;

	public bool istargeting = true;

	public Vector3 nextrotation = Vector3.Zero;

	public Random rnd;

	public bool imcolliding;

	private Vector3 nextPosition;

	public bool canimove = true;

	private Vector3 oldpos;

	public Matrix targetworld;

	private float Area = 50f;

	public float rotationvel = 0.05f;

	public Shark(Model model, Vector3 pos, Vector3 rot, float scale, int seed)
		: base(model)
	{
		skinnedModel = model.Tag as SkinnedModel;
		base.UpdateType = UpdateType.Automatic;
		base.Visibility = ObjectVisibility.RenderedAndCastShadows;
		base.World = Matrix.CreateScale(scale) * Matrix.CreateRotationY(MathHelper.ToRadians(rot.Y)) * Matrix.CreateTranslation(pos);
		Rotation = rot;
		Position = pos;
		nextPosition = Position;
		Scale = scale;
		Position.Y = -10.5f;
		rnd = new Random(seed);
		animationController = new AnimationController(skinnedModel.SkeletonBones);
		animationController.SwitchToClip(skinnedModel.AnimationClips["Idle"]);
		animationController.LoopEnabled = true;
		animationController.TranslationInterpolation = InterpolationMode.Linear;
		animationController.OrientationInterpolation = InterpolationMode.Linear;
		animationController.Speed = 1f;
		targetworld = base.World;
	}

	private void stayinbounds(int LR)
	{
		if (!(nextrotation == Rotation))
		{
			return;
		}
		if (Position.X > targetworld.Translation.X + Area)
		{
			nextrotation.Y = Rotation.Y + (float)(LR * 180);
		}
		if (Position.X < targetworld.Translation.X - Area)
		{
			nextrotation.Y = Rotation.Y + (float)(LR * 180);
		}
		if (Position.Z > targetworld.Translation.Z + Area)
		{
			nextrotation.Y = Rotation.Y + (float)(LR * 180);
		}
		if (Position.Z < targetworld.Translation.Z - Area)
		{
			nextrotation.Y = Rotation.Y + (float)(LR * 180);
		}
		if (targetworld != base.World)
		{
			if (Position.X < targetworld.Translation.X + 12f)
			{
				nextrotation.Y = Rotation.Y + (float)(LR * 180);
			}
			if (Position.X > targetworld.Translation.X - 12f)
			{
				nextrotation.Y = Rotation.Y + (float)(LR * 180);
			}
			if (Position.Z < targetworld.Translation.Z + 12f)
			{
				nextrotation.Y = Rotation.Y + (float)(LR * 180);
			}
			if (Position.Z > targetworld.Translation.Z - 12f)
			{
				nextrotation.Y = Rotation.Y + (float)(LR * 180);
			}
		}
	}

	public override void Update(GameTime gametime)
	{
		Vector3 vector = -base.World.Forward;
		int num = rnd.Next(-2, 2);
		if (num == 0)
		{
			num = 1;
		}
		if (behaviour == Behaviour.patrol)
		{
			Position.X = MathHelper.Clamp(Position.X, targetworld.Translation.X - Area, targetworld.Translation.X + Area);
			Position.Z = MathHelper.Clamp(Position.Z, targetworld.Translation.Z - Area, targetworld.Translation.Z + Area);
			if (Rotation.Y == nextrotation.Y && totalTime > 2000f)
			{
				nextrotation.Y = Rotation.Y + (float)(num * 180);
				totalTime = 0f;
			}
			totalTime += gametime.ElapsedGameTime.Milliseconds;
			stayinbounds(num);
			rotationvel = 0.07f;
			Position += vector * gametime.ElapsedGameTime.Milliseconds * 0.2f;
		}
		if (behaviour == Behaviour.attack1 || behaviour == Behaviour.attack2)
		{
			rotationvel = 0.1f;
			Position += vector * gametime.ElapsedGameTime.Milliseconds * 0.4f;
			facetarget();
		}
		if (behaviour == Behaviour.hit)
		{
			rotationvel = 0.07f;
			Position += vector * gametime.ElapsedGameTime.Milliseconds * 0.2f;
		}
		Position.Y = MathHelper.Clamp(Position.Y, -20f, -0f);
		animationController.Update(gametime.ElapsedGameTime, Matrix.Identity);
		base.SkinBones = animationController.SkinnedBoneTransforms;
		rotateme(gametime);
		base.World = Matrix.CreateScale(Scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(Rotation.Y), MathHelper.ToRadians(Rotation.X), MathHelper.ToRadians(Rotation.Z)) * Matrix.CreateTranslation(Position);
		oldpos = Position;
		base.Update(gametime);
	}

	private void facetarget()
	{
		float num = Position.X - targetworld.Translation.X;
		float num2 = Position.Z - targetworld.Translation.Z;
		float radians = (float)Math.Atan2(0f - num, 0f - num2);
		if (Position.Y < -1f)
		{
			nextrotation.Y = MathHelper.ToDegrees(radians) + 180f;
		}
		else
		{
			nextrotation.Y = MathHelper.ToDegrees(radians);
		}
	}

	public void rotateme(GameTime gametime)
	{
		if (Rotation.Y == nextrotation.Y)
		{
			return;
		}
		if (nextrotation.Y < 0f)
		{
			nextrotation.Y += 360f;
		}
		if (nextrotation.Y > 360f)
		{
			nextrotation.Y -= 360f;
		}
		if (nextrotation.Y > Rotation.Y)
		{
			if (nextrotation.Y - Rotation.Y > 180f)
			{
				Rotation.Y += 360f;
			}
		}
		else if (Rotation.Y - nextrotation.Y > 180f)
		{
			Rotation.Y -= 360f;
		}
		if (nextrotation.Y > Rotation.Y)
		{
			Rotation.Y += (float)gametime.ElapsedGameTime.Milliseconds * rotationvel;
			Rotation.Y = MathHelper.Clamp(Rotation.Y, Rotation.Y, nextrotation.Y);
			if (nextrotation.Y < Rotation.Y)
			{
				Rotation.Y = nextrotation.Y;
			}
		}
		if (nextrotation.Y < Rotation.Y)
		{
			Rotation.Y -= (float)gametime.ElapsedGameTime.Milliseconds * rotationvel;
			Rotation.Y = MathHelper.Clamp(Rotation.Y, nextrotation.Y, Rotation.Y);
			if (nextrotation.Y > Rotation.Y)
			{
				Rotation.Y = nextrotation.Y;
			}
		}
		if (nextrotation.X > Rotation.X)
		{
			Rotation.X += (float)gametime.ElapsedGameTime.Milliseconds * rotationvel;
			Rotation.X = MathHelper.Clamp(Rotation.X, Rotation.X, nextrotation.X);
		}
		if (nextrotation.X < Rotation.X)
		{
			Rotation.X -= (float)gametime.ElapsedGameTime.Milliseconds * rotationvel;
			Rotation.X = MathHelper.Clamp(Rotation.X, nextrotation.X, Rotation.X);
		}
	}
}
