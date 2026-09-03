using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class BulletB : Enemy
{
	public static ModelWrapper model;

	private Enemy enem;

	private Vector3 oldvel;

	private Vector3 vel;

	private Vector3 up;

	private ModelWrapper parent;

	private int parentBone;

	private float speed;

	private float maxspeed;

	private float rot;

	private static float rotInc = (float)Math.PI * 2f;

	private Matrix _transformation;

	private bool dirty;

	private bool launched;

	public BulletB(Vector3 _start, Vector3 _vel, Vector3 _up, Enemy _enem, ref ModelWrapper _model, string _boneName)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		setPos(_start);
		enem = _enem;
		parent = _model;
		parentBone = ((ReadOnlyCollection<ModelBone>)(object)parent.model.Bones).IndexOf(parent.model.Bones[_boneName]);
		vel = Vector3.Normalize(_vel);
		up = Vector3.Normalize(_up);
		speed = 25f;
		maxspeed = 5f;
		state = 0;
		hitPoints = 1;
		launched = false;
		dirty = true;
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
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (exists)
		{
			base.draw(gametime);
			BaseGame.Get().SwitchEffectTechnique("Textured");
			BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Transformation());
			if (((Whale01)enem).scaleAmount <= 0.95f)
			{
				BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(0f));
			}
			BaseGame.Get().DrawModel(ref model);
			BaseGame.Get().matStack.PopMatrix();
		}
	}

	public override Matrix Transformation()
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (dirty)
		{
			CheckLaunch();
			if (!launched)
			{
				_transformation = BaseGame.MapObjectToSystem(Vector3.Zero, vel, up) * Matrix.CreateTranslation(getPos()) * parent.transforms[parentBone] * enem.Transformation();
			}
			else
			{
				_transformation = Matrix.CreateRotationZ(rot) * BaseGame.MapObjectToSystem(Vector3.Zero, vel, up) * Matrix.CreateScale(new Vector3(3f, 3f, -3f)) * Matrix.CreateTranslation(getPos());
			}
			dirty = false;
		}
		return _transformation;
	}

	public void CheckLaunch()
	{
		if (!launched && (!enem.exists || parent.transforms[parentBone].M44 == 0f))
		{
			Launch();
		}
	}

	public void Launch()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = parent.transforms[parentBone] * enem.Transformation();
		launched = true;
		setPos(Vector3.Transform(getPos(), val));
	}

	public override void act(GameTime gametime)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		dirty = true;
		CheckLaunch();
		if (exists && launched)
		{
			oldvel = vel;
			Vector3 val = Vector3.Normalize(BaseGame.Get().playerPos + new Vector3(0f, 0f, 3f) - getPos());
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
		BaseGame.Get().actualEnem++;
		base.die();
	}

	public override void leave()
	{
		BaseGame.Get().actualEnem++;
		base.leave();
	}
}
