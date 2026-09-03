using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class SerpentTailCollection : Enemy
{
	public List<SerpentTail> waterWireNormEnemies;

	public List<SerpentTail> waterWireOluEnemies;

	public List<SerpentTail> waterSolidNormEnemies;

	public List<SerpentTail> normWireNormEnemies;

	public List<SerpentTail> normSolidNormEnemies;

	public SerpentTailCollection()
	{
		waterWireNormEnemies = new List<SerpentTail>();
		waterWireOluEnemies = new List<SerpentTail>();
		waterSolidNormEnemies = new List<SerpentTail>();
		normWireNormEnemies = new List<SerpentTail>();
		normSolidNormEnemies = new List<SerpentTail>();
	}

	public override void act(GameTime gametime)
	{
	}

	public override void draw(GameTime gametime)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		Matrix toAdd = BaseGame.Get().matStack.Top();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		BaseGame.Get().fogEffect.Parameters["xEnableLighting"].SetValue(true);
		BaseGame.Get().fogEffect.Parameters["usePalette"].SetValue(false);
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().SetUpEffect(ref SerpentTail.tailModelWire.epc[0], clearEpc: false);
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		foreach (SerpentTail normWireNormEnemy in normWireNormEnemies)
		{
			normWireNormEnemy.DrawModel(gametime);
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().SetUpEffect(ref SerpentTail.tailModel.epc[0], clearEpc: false);
		foreach (SerpentTail normSolidNormEnemy in normSolidNormEnemies)
		{
			normSolidNormEnemy.DrawModel(gametime);
		}
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
		BaseGame.Get().SwitchEffectTechnique("Water");
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		foreach (SerpentTail waterSolidNormEnemy in waterSolidNormEnemies)
		{
			waterSolidNormEnemy.DrawModel(gametime);
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().SetUpEffect(ref SerpentTail.tailModelWire.epc[0], clearEpc: false);
		foreach (SerpentTail waterWireNormEnemy in waterWireNormEnemies)
		{
			waterWireNormEnemy.DrawModel(gametime);
		}
		BaseGame.Get().SetUpEffect(ref SerpentTail.tailModelOlu.epc[0], clearEpc: false);
		foreach (SerpentTail waterWireOluEnemy in waterWireOluEnemies)
		{
			waterWireOluEnemy.DrawModel(gametime);
		}
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
		BaseGame.Get().matStack.ApplyRawMatrix(toAdd);
		BaseGame.Get().SwitchEffectTechnique("Ripple");
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		foreach (SerpentTail normSolidNormEnemy2 in normSolidNormEnemies)
		{
			normSolidNormEnemy2.DrawRipple(gametime);
		}
		foreach (SerpentTail normWireNormEnemy2 in normWireNormEnemies)
		{
			normWireNormEnemy2.DrawRipple(gametime);
		}
		foreach (SerpentTail waterSolidNormEnemy2 in waterSolidNormEnemies)
		{
			waterSolidNormEnemy2.DrawRipple(gametime);
		}
		foreach (SerpentTail waterWireNormEnemy2 in waterWireNormEnemies)
		{
			waterWireNormEnemy2.DrawRipple(gametime);
		}
		foreach (SerpentTail waterWireOluEnemy2 in waterWireOluEnemies)
		{
			waterWireOluEnemy2.DrawRipple(gametime);
		}
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
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
