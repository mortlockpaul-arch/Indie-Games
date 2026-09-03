using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class Bird01 : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public static ModelWrapper model;

	public static ModelWrapper wireModel;

	public static ModelWrapper wireJet;

	public static ModelWrapper solidJet;

	public Vector3 actualPos;

	public Vector3 vel;

	public Vector3 up;

	public Vector3 oldvel;

	public float maxSpeed;

	public float accel;

	private Vector4 partColor;

	public float pCooldown;

	public float pMax;

	public Vector3 revVel;

	public Bird01()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		pMax = 0.05f;
		base._002Ector();
		state = 0;
		attackCooldown = 5f;
		hitPoints = 1;
		vel = Vector3.Forward;
		revVel = -vel;
		up = Vector3.Up;
		maxSpeed = 40f;
		accel = 2f;
		if (wCond == null)
		{
			wCond = new Dictionary<int, WaitCond>();
			wCond.Add(0, new WaitCond("Bird01", Beats.Quarter));
			wCond.Add(1, new WaitCond("Bird01", Beats.Eighth));
			wCond.Add(2, new WaitCond("Bird02", Beats.Eighth, Beats.Quarter));
			wCond.Add(3, new WaitCond("Bird03", Beats.Eighth, Beats.Quarter));
			wCond.Add(4, new WaitCond("Bird04", Beats.Eighth, Beats.Quarter));
			wCond.Add(5, new WaitCond("Bird05", Beats.Eighth));
			wCond.Add(6, new WaitCond("Bird06", Beats.Eighth));
			wCond.Add(7, new WaitCond("Bird07", Beats.Eighth));
			wCond.Add(8, new WaitCond("Bird08", Beats.Eighth));
		}
		_eCond = wCond;
	}

	public static void LoadModel()
	{
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		model = BaseGame.Get().models.GetModel("Content\\Bird01\\Bird01");
		wireModel = BaseGame.Get().models.GetModel("Content\\Bird01\\Bird01Wire");
		wireJet = BaseGame.Get().models.GetModel("Content\\Bird01\\Jet", copyData: false, copyEPC: true);
		solidJet = BaseGame.Get().models.GetModel("Content\\Bird01\\Jet", copyData: false, copyEPC: true);
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", false);
		BaseGame.Get().LinkEffect(model.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.SetAllEPCs(wireModel.epc, "xEnableLighting", false);
		BaseGame.Get().LinkEffect(wireModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.SetAllEPCs(wireJet.epc, "xEnableLighting", false);
		BaseGame.Get().LinkEffect(wireJet.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.SetAllEPCs(solidJet.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(solidJet.epc, "DiffuseColor", (object)new Vector3(0.2f, 0.1f, 0f));
		BaseGame.Get().LinkEffect(solidJet.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
	}

	public Bird01(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
	}

	public override Matrix Transformation()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		return BaseGame.MapObjectToSystem(Vector3.Zero, (vel == Vector3.Zero) ? Vector3.Forward : vel, up) * Matrix.CreateScale(new Vector3(5f, 5f, 5f)) * Matrix.CreateTranslation(getPos());
	}

	public override void draw(GameTime gametime)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Invalid comparison between Unknown and I4
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		if ((int)fillMode == 3)
		{
			BaseGame.Get().DrawModel(ref model);
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1.2f));
			BaseGame.Get().DrawModel(ref solidJet);
		}
		else
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
			BaseGame.Get().DrawModel(ref wireModel);
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1.2f));
			BaseGame.Get().DrawModel(ref wireJet);
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		}
		BaseGame.Get().matStack.PopMatrix();
	}

	public override void hit(TargetEffectBase toHit)
	{
		base.hit(toHit);
	}

	public override void die()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Invalid comparison between Unknown and I4
		BaseGame.Get().ps.AddParticles(actualPos, Vector3.Forward * 25f, 2f, 180f, Vector3.Zero, 0f, 0.25f, 0.1f, 0.2f, new Vector4(1f, 1f, 1f, 1f), 80, 0.0005f, ((int)fillMode == 2) ? 1 : 0);
		base.die();
	}

	public override void act(GameTime gametime)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Invalid comparison between Unknown and I4
		base.act(gametime);
		if (exists)
		{
			oldvel = vel;
			vel += (pos - actualPos) * accel * (float)gametime.ElapsedGameTime.TotalSeconds;
			if (((Vector3)(ref vel)).Length() > pathList.maxSpeed())
			{
				vel = Vector3.Normalize(vel) * pathList.maxSpeed();
			}
			actualPos += vel * (float)gametime.ElapsedGameTime.TotalSeconds;
			if (vel != oldvel)
			{
				up = BaseGame.GetUpVector(vel, up, oldvel);
			}
			pCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			if (pCooldown < 0f)
			{
				BaseGame.Get().ps.AddParticles(actualPos, -vel, 0.2f, 10f, Vector3.Zero, 0f, 0.5f, 0.5f, 0.2f, partColor, 15, 0.001f, ((int)fillMode == 2) ? 1 : 0);
				pCooldown += pMax;
			}
		}
	}

	public override void start()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		addTarget(Vector3.Zero, 1, 10);
		addCond(new NeverCondition());
		base.start();
		vel = BaseGame.FaceUpward(pathList);
		actualPos = pos;
		if ((int)fillMode == 2)
		{
			partColor = new Vector4(1f, 0.65f, 0f, 1f);
		}
		else
		{
			partColor = new Vector4(0.2f, 0.1f, 0f, 1f);
		}
	}

	public override Vector3 getPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return actualPos;
	}

	public override string name()
	{
		return "[sentry_0xA]";
	}

	public override bool Check(int numEnem)
	{
		return wCond[numEnem].Check(BaseGame.Get().curBeat);
	}

	public override void HitSound(int lockNum, float volume)
	{
		if (lockNum <= 8)
		{
			BaseGame.Get().PlayCue(wCond[lockNum].cueName, volume);
		}
	}
}
