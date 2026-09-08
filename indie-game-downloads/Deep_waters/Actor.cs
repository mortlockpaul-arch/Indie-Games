using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SgMotion;
using SgMotion.Controllers;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace Deep_waters;

public class Actor : SceneObject
{
	public SkinnedModel skinnedModel;

	public AnimationController animationController;

	public Vector3 Position = Vector3.Zero;

	public Vector3 Velocity = Vector3.Zero;

	public Vector3 Rotation = Vector3.Zero;

	public float Scale;

	public Actor(Model model, Vector3 pos, Vector3 rot, float scale, string initanim)
		: base(model)
	{
		skinnedModel = model.Tag as SkinnedModel;
		Rotation = rot;
		Position = pos;
		Scale = scale;
		base.UpdateType = UpdateType.Automatic;
		base.AffectedInCode = true;
		base.Visibility = ObjectVisibility.RenderedAndCastShadows;
		base.World = Matrix.CreateScale(Scale) * Matrix.CreateRotationY(MathHelper.ToRadians(Rotation.Y)) * Matrix.CreateTranslation(Position);
		animationController = new AnimationController(skinnedModel.SkeletonBones);
		animationController.SwitchToClip(skinnedModel.AnimationClips[initanim]);
		animationController.LoopEnabled = false;
		animationController.TranslationInterpolation = InterpolationMode.None;
		animationController.OrientationInterpolation = InterpolationMode.None;
		animationController.Speed = 1f;
		foreach (RenderableMesh renderableMesh in base.RenderableMeshes)
		{
			renderableMesh.MeshBoundingSphere = new BoundingSphere(Vector3.Zero, 500000f);
		}
		CalculateBounds();
	}

	protected override void Init(string name, bool infinitebounds)
	{
		base.Init(name, true);
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		if (skinnedModel != null)
		{
			animationController.Update(gameTime.ElapsedGameTime, Matrix.Identity);
			base.SkinBones = animationController.SkinnedBoneTransforms;
		}
	}
}
