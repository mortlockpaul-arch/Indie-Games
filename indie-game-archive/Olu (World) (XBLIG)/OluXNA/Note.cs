using System;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class Note : Enemy
{
	public static ModelWrapper model;

	private Vector3 oldvel;

	private Vector3 vel;

	private Vector3 up;

	private float speed;

	private float maxspeed;

	private float rot;

	private static float rotInc = (float)Math.PI * 2f;

	private Enemy parent;

	private float phaseCountdown;

	private Matrix _transformation;

	private bool dirty;

	private int bossPart;

	public Note(Vector3 _start, Vector3 _vel, Vector3 _up, Enemy _parent, int _part)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		setPos(_start);
		parent = _parent;
		vel = Vector3.Normalize(_vel);
		up = Vector3.Normalize(_up);
		speed = 25f;
		maxspeed = 5f;
		phaseCountdown = 1.5f;
		hitPoints = 1;
		dirty = true;
		bossPart = _part;
	}

	public static void LoadModel()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		model = BaseGame.Get().models.GetModel("Content\\FinalLevel\\Note");
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(model.epc, "DirLight0Direction", (object)new Vector3(-0.5f, -0.5f, 1f));
		BaseGame.Get().LinkEffect(model.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
	}

	public override void draw(GameTime gametime)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (exists)
		{
			base.draw(gametime);
			BaseGame.Get().SwitchEffectTechnique("Textured");
			BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Transformation());
			BaseGame.Get().DrawModel(ref model);
			BaseGame.Get().matStack.PopMatrix();
		}
	}

	public override Matrix Transformation()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (dirty)
		{
			_transformation = Matrix.CreateScale(new Vector3(1.8f, 1.8f, -1.8f)) * Matrix.CreateTranslation(getPos());
			dirty = false;
		}
		return _transformation;
	}

	public override void act(GameTime gametime)
	{
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (pathList.publicPaths.Count > 0)
		{
			base.act(gametime);
		}
		dirty = true;
		if (!exists)
		{
			return;
		}
		if (pathList.publicPaths.Count == 0)
		{
			if (phaseCountdown > 0f)
			{
				phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			}
			oldvel = vel;
			Vector3 val;
			if (phaseCountdown < 0f)
			{
				val = Vector3.Normalize(BaseGame.Get().playerPos - getPos());
				vel = oldvel * 0.95f + val * 0.05f;
				vel = Vector3.Normalize(vel);
				if (speed > maxspeed)
				{
					speed *= 0.98f;
				}
			}
			val = vel * (float)((double)speed * gametime.ElapsedGameTime.TotalSeconds);
			setPos(getPos() + val);
			if (vel != oldvel)
			{
				up = BaseGame.GetUpVector(vel, up, oldvel);
			}
		}
		rot += rotInc * (float)gametime.ElapsedGameTime.TotalSeconds;
		BaseGame.SetAllEPCs(model.epc, "DirLight0Direction", Vector3.Transform(new Vector3(-0.5f, -0.5f, 1f), Matrix.CreateRotationZ(rot)));
		PlayerHit();
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
		if (pathList.publicPaths.Count == 0)
		{
			pos = val;
		}
	}

	public override Enemy attack()
	{
		return null;
	}

	public override string name()
	{
		return "[cleftel^as]";
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
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!toHit.skipSquare)
		{
			BaseGame.Get().ps.AddParticles(getPos(), Vector3.Forward * 25f, 0f, 180f, Vector3.Zero, 0f, 0.25f, 0f, 0.2f, new Vector4(1f, 1f, 1f, 1f), 80, 0.0005f);
		}
		((Olu)parent).NoteHit(bossPart, (toHit.fillMode == fillMode) ? 1 : 2);
		base.hit(toHit);
	}
}
