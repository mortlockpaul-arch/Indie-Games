using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SgMotion;
using SgMotion.Controllers;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace Deep_waters;

public class FloatingItem : SceneObject
{
	public SkinnedModel skinnedModel;

	public AnimationController animationController;

	public Vector3 Position = Vector3.Zero;

	public Vector3 Velocity = Vector3.Zero;

	public Vector3 Rotation = Vector3.Zero;

	public float Scale;

	private float totalTime;

	private Model m;

	public FloatingItem(Model model, Vector3 pos, Vector3 vel, Vector3 rot, float scale)
		: base(model)
	{
		m = model;
		Position = pos;
		Velocity = vel;
		Rotation = rot;
		Scale = scale;
		base.UpdateType = UpdateType.Automatic;
		base.Visibility = ObjectVisibility.RenderedAndCastShadows;
		base.World = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(Rotation.Y), MathHelper.ToRadians(Rotation.X), MathHelper.ToRadians(Rotation.Z)) * Matrix.CreateTranslation(pos);
		if (model.Tag is SkinnedModel)
		{
			skinnedModel = model.Tag as SkinnedModel;
			animationController = new AnimationController(skinnedModel.SkeletonBones);
			animationController.SwitchToClip(skinnedModel.AnimationClips["Idle"]);
			animationController.LoopEnabled = true;
			animationController.TranslationInterpolation = InterpolationMode.Linear;
			animationController.OrientationInterpolation = InterpolationMode.Linear;
			animationController.Speed = 1f;
		}
	}

	public override void Update(GameTime gameTime)
	{
		if (m.Tag is SkinnedModel && skinnedModel == null)
		{
			skinnedModel = m.Tag as SkinnedModel;
			animationController = new AnimationController(skinnedModel.SkeletonBones);
			animationController.SwitchToClip(skinnedModel.AnimationClips["Idle"]);
			animationController.LoopEnabled = true;
			animationController.TranslationInterpolation = InterpolationMode.Linear;
			animationController.OrientationInterpolation = InterpolationMode.Linear;
			animationController.Speed = 1f;
		}
		Position.Y = (0f - (float)(Math.Sin(totalTime * 16f) / 16.0)) * 0.1f;
		if (animationController != null)
		{
			animationController.Update(gameTime.ElapsedGameTime, Matrix.Identity);
			base.SkinBones = animationController.SkinnedBoneTransforms;
		}
		base.Update(gameTime);
	}
}
