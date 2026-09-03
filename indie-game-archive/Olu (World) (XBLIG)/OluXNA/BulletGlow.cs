using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class BulletGlow : Enemy
{
	public static ModelWrapper model;

	private float size;

	public BulletGlow(Vector3 startPos, float _size, int _hp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(startPos, _size, _hp, pineMode: true);
	}

	public BulletGlow(Vector3 startPos, float _size, int _hp, bool pineMode)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		setPos(startPos);
		state = 0;
		hitPoints = _hp;
		size = _size;
		pathList = new PathList();
		if (pineMode)
		{
			Matrix val = Matrix.CreateRotationZ(2f * (float)(Math.PI * BaseGame.Get().r.NextDouble()));
			pathList.Add(new PRefLine(startPos, startPos + Vector3.Transform(new Vector3(40f, 0f, -16f), val), 40f, BaseGame.Get().cameraLoc));
			pathList.Add(new PBezier(startPos + Vector3.Transform(new Vector3(40f, 0f, -16f), val), startPos + Vector3.Transform(new Vector3(45f, 20f, -30f), val), BaseGame.Get().playerPos + Vector3.Transform(new Vector3(20f, 6f, 4f), val), BaseGame.Get().playerPos, 0.105f, Vector3.Up, 0f, 0f, 0f, 0, 0f, 0.0, 0.0));
		}
		else
		{
			pathList.Add(new PRefLine(startPos, startPos + new Vector3(0f, 0f, 30f), 30f, BaseGame.Get().cameraLoc));
			PathList obj = pathList;
			Vector3 val2 = startPos + new Vector3(0f, 0f, 30f);
			Vector3 playerPos = BaseGame.Get().playerPos;
			Vector3 val3 = startPos + new Vector3(0f, 0f, 30f) - BaseGame.Get().playerPos;
			obj.Add(new PRefLine(val2, playerPos, ((Vector3)(ref val3)).Length() * 0.18f, BaseGame.Get().cameraLoc));
		}
		pathList.ResetCurrent();
		fillMode = (FillMode)2;
	}

	public static void LoadModel()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		model = BaseGame.Get().models.GetModel("Content\\Gift\\GiftDot");
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(model.epc, "DiffuseColor", (object)new Vector3(1f, 1f, 1f));
		BaseGame.Get().LinkEffect(model.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
	}

	public override void draw(GameTime gametime)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (exists)
		{
			base.draw(gametime);
			BaseGame.Get().SwitchEffectTechnique("Textured");
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = fillMode;
			BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Transformation());
			BaseGame.Get().DrawModel(ref model);
			BaseGame.Get().matStack.PopMatrix();
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		}
	}

	public override Matrix Transformation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateScale(new Vector3(size)) * Matrix.CreateTranslation(getPos());
	}

	public override void act(GameTime gametime)
	{
		if (exists)
		{
			base.act(gametime);
			PlayerHit();
		}
	}

	public override void start()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		addTarget(new Vector3(0f, 0f, 0f), hitPoints, 10);
		base.start();
		BaseGame.Get().actualEnem--;
	}

	public override Enemy attack()
	{
		return new ECube();
	}

	public override string name()
	{
		return "[dos 0x0402]";
	}

	public override void HitSound(int lockNum, float volume)
	{
		if (lockNum <= 8)
		{
			BaseGame.Get().PlayCue("muteCrash", volume);
		}
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!toHit.skipSquare)
		{
			BaseGame.Get().ps.AddParticles(getPos(), Vector3.Forward * 25f, 0f, 180f, Vector3.Zero, 0f, 0.25f, 0f, 0.2f, new Vector4(1f, 1f, 0f, 1f), 80, 0.0005f);
		}
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
}
