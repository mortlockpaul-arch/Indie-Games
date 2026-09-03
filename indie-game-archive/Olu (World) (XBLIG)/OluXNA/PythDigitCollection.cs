using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class PythDigitCollection : Enemy
{
	public List<PythDigit> enemies;

	public PythDigitCollection()
	{
		enemies = new List<PythDigit>();
	}

	public override void act(GameTime gametime)
	{
	}

	public override void draw(GameTime gametime)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		Matrix toAdd = BaseGame.Get().matStack.Top();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		BaseGame.Get().fogEffect.Parameters["xEnableLighting"].SetValue(false);
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().SetUpEffect(ref Digit.wire.epc[0], clearEpc: false);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		BaseGame.Get().SetUpEffect(ref Digit.wire.epc[0], clearEpc: false);
		BaseGame.Get().fogEffect.Parameters["xPose"].SetValue(Digit.wire.transforms[((ReadOnlyCollection<ModelMesh>)(object)Digit.wire.model.Meshes)[0].ParentBone.Index]);
		BaseGame.Get().fogEffect.Parameters["usePalette"].SetValue(false);
		foreach (PythDigit enemy in enemies)
		{
			enemy.DrawWire(gametime);
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
		BaseGame.Get().matStack.ApplyRawMatrix(toAdd);
	}

	public override void start()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base.start();
		BaseGame.Get().actualEnem--;
		pos = Vector3.Zero;
	}

	public override Enemy attack()
	{
		return new ECube();
	}

	public override string name()
	{
		return "[fish_col]";
	}

	public override void HitSound(int lockNum, float volume)
	{
	}

	public override void hit(TargetEffectBase toHit)
	{
		base.hit(toHit);
	}

	public override void die()
	{
		BaseGame.Get().actualEnem++;
		base.die();
	}

	public override void leave()
	{
		BaseGame.Get().actualEnem++;
		base.leave();
	}

	public override Matrix Transformation()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.Identity;
	}

	public override TargetEffectCol lockOn(int targetsLeft)
	{
		return new TargetEffectCol();
	}
}
