using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class Fish01Collection : Enemy
{
	public List<Fish01> fillEnemies;

	public List<Fish01> wireEnemies;

	public List<Fish01> oluEnemies;

	public Fish01Collection()
	{
		fillEnemies = new List<Fish01>();
		wireEnemies = new List<Fish01>();
		oluEnemies = new List<Fish01>();
	}

	public override void act(GameTime gametime)
	{
	}

	public override void draw(GameTime gametime)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		Matrix toAdd = BaseGame.Get().matStack.Top();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		BaseGame.Get().fogEffect.Parameters["xEnableLighting"].SetValue(true);
		BaseGame.Get().fogEffect.Parameters["usePalette"].SetValue(false);
		BaseGame.Get().SwitchEffectTechnique("Water");
		BaseGame.Get().SetUpEffect(ref Fish01.model.epc[0], clearEpc: false);
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		foreach (Fish01 fillEnemy in fillEnemies)
		{
			fillEnemy.DrawBody(gametime);
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().SetUpEffect(ref Fish01.wireModel.epc[0], clearEpc: false);
		foreach (Fish01 wireEnemy in wireEnemies)
		{
			wireEnemy.DrawBody(gametime);
		}
		BaseGame.Get().SetUpEffect(ref Fish01.oluModel.epc[0], clearEpc: false);
		foreach (Fish01 oluEnemy in oluEnemies)
		{
			oluEnemy.DrawBody(gametime);
		}
		BaseGame.Get().SetUpEffect(ref Fish01.wireJet.epc[0], clearEpc: false);
		foreach (Fish01 wireEnemy2 in wireEnemies)
		{
			wireEnemy2.DrawJet(gametime);
		}
		BaseGame.Get().SetUpEffect(ref Fish01.oluJet.epc[0], clearEpc: false);
		foreach (Fish01 wireEnemy3 in wireEnemies)
		{
			wireEnemy3.DrawJet(gametime);
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().SetUpEffect(ref Fish01.solidJet.epc[0], clearEpc: false);
		foreach (Fish01 fillEnemy2 in fillEnemies)
		{
			fillEnemy2.DrawJet(gametime);
		}
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
		BaseGame.Get().matStack.ApplyRawMatrix(toAdd);
		BaseGame.Get().SwitchEffectTechnique("Ripple");
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		foreach (Fish01 fillEnemy3 in fillEnemies)
		{
			fillEnemy3.DrawRipple(gametime);
		}
		foreach (Fish01 wireEnemy4 in wireEnemies)
		{
			wireEnemy4.DrawRipple(gametime);
		}
		foreach (Fish01 oluEnemy2 in oluEnemies)
		{
			oluEnemy2.DrawRipple(gametime);
		}
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
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
