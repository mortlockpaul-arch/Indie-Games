using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class BulletC : Enemy
{
	public static ModelWrapper model;

	private Vector3 oldvel;

	private Vector3 vel;

	private Vector3 up;

	private float speed;

	private float maxspeed;

	private float rot;

	private static float rotInc = (float)Math.PI * 2f;

	private Enemy enem;

	private int part;

	private Matrix _transformation;

	private bool dirty;

	private bool launched;

	public bool active;

	public bool IsLaunched => launched;

	public BulletC(Vector3 _start, Vector3 _vel, Vector3 _up, Enemy _enem, int _stagePart)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		setPos(_start);
		vel = Vector3.Normalize(_vel);
		up = Vector3.Normalize(_up);
		speed = 25f;
		maxspeed = 5f;
		state = 0;
		hitPoints = 1;
		launched = false;
		active = true;
		dirty = true;
		enem = _enem;
		part = _stagePart;
		fillMode = (FillMode)2;
	}

	public static void LoadModel()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		model = BaseGame.Get().models.GetModel("Content\\BulletB\\BulletB");
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(model.epc, "DirLight0Direction", (object)new Vector3(-0.5f, -0.5f, 1f));
		BaseGame.Get().LinkEffect(model.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
	}

	public override void draw(GameTime gametime)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (active && exists)
		{
			base.draw(gametime);
			BaseGame.Get().SwitchEffectTechnique("Textured");
			BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
			if (launched)
			{
				BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
			}
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Transformation());
			BaseGame.Get().DrawModel(ref model);
			BaseGame.Get().matStack.PopMatrix();
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		}
	}

	public override Matrix Transformation()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (dirty)
		{
			_transformation = Matrix.CreateRotationZ(rot) * Matrix.CreateScale(new Vector3(2f, 2f, 2f)) * BaseGame.MapObjectToSystem(Vector3.Zero, vel, up) * Matrix.CreateTranslation(getPos());
			dirty = false;
		}
		return _transformation;
	}

	public override void act(GameTime gametime)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		dirty = true;
		if (exists && launched)
		{
			oldvel = vel;
			Vector3 val = Vector3.Normalize(BaseGame.Get().playerPos - getPos());
			vel = oldvel * 0.95f + val * 0.05f;
			vel = Vector3.Normalize(vel);
			if (speed > maxspeed)
			{
				speed *= 0.98f;
			}
			val = vel * (float)((double)speed * gametime.ElapsedGameTime.TotalSeconds);
			setPos(getPos() + val);
			if (vel != oldvel)
			{
				up = BaseGame.GetUpVector(vel, up, oldvel);
			}
			rot += rotInc * (float)gametime.ElapsedGameTime.TotalSeconds;
			PlayerHit();
		}
		else if (exists && hitPoints <= 0)
		{
			leave();
		}
	}

	public void Launch()
	{
		launched = true;
	}

	public override void start()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos;
		addTarget(new Vector3(0f, 0f, 0f), 1, 10);
		base.start();
		BaseGame.Get().actualEnem--;
		pos = val;
	}

	public override Enemy attack()
	{
		return new ECube();
	}

	public override string name()
	{
		return "[shot 0x0002]";
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
		if (enem is Hypatia)
		{
			((Hypatia)enem).hp[part]--;
		}
		BaseGame.Get().actualEnem++;
		base.die();
	}

	public override void leave()
	{
		BaseGame.Get().actualEnem++;
		base.leave();
	}

	public override TargetEffectCol lockOn(int targetsLeft)
	{
		if (active)
		{
			return base.lockOn(targetsLeft);
		}
		return new TargetEffectCol();
	}
}
