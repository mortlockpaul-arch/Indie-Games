using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class Fish01 : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public static ModelWrapper model;

	public static ModelWrapper solidJet;

	public static ModelWrapper wireJet;

	public static ModelWrapper wireModel;

	public static PlaneEffect pE;

	public static PlaneEffect oluPE;

	public static ModelWrapper oluJet;

	public static ModelWrapper oluModel;

	public Matrix posTransform;

	public Vector3 actualPos;

	public Vector3 vel;

	public Vector3 oldvel;

	public Vector3 up;

	public float maxSpeed;

	public float accel;

	public Vector4 partColor;

	public float pCooldown;

	public float pMax;

	public float waterHeight;

	public RippleEffect rE;

	public Vector3 ripplePos;

	public bool drawRipple;

	public bool drawWater;

	public bool launched;

	public bool launchDone;

	public float jetScale;

	public bool oluMode;

	public Fish01()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		pMax = 0.02f;
		base._002Ector();
		state = 0;
		attackCooldown = 5f;
		hitPoints = 1;
		vel = Vector3.Forward;
		up = Vector3.Up;
		maxSpeed = 40f;
		accel = 2f;
		launched = (launchDone = false);
		jetScale = 1f;
		if (wCond == null)
		{
			wCond = new Dictionary<int, WaitCond>();
			wCond.Add(0, new WaitCond("Fish01", Beats.Quarter));
			wCond.Add(1, new WaitCond("Fish01", Beats.Eighth));
			wCond.Add(2, new WaitCond("Fish02", Beats.Eighth));
			wCond.Add(3, new WaitCond("Fish03", Beats.Eighth));
			wCond.Add(4, new WaitCond("Fish04", Beats.Eighth));
			wCond.Add(5, new WaitCond("Fish05", Beats.Eighth));
			wCond.Add(6, new WaitCond("Fish06", Beats.Eighth));
			wCond.Add(7, new WaitCond("Fish07", Beats.Eighth));
			wCond.Add(8, new WaitCond("Fish08", Beats.Eighth));
		}
		_eCond = wCond;
	}

	public static void LoadModel()
	{
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		model = BaseGame.Get().models.GetModel("Content\\Fish01\\Fish01Body");
		wireJet = BaseGame.Get().models.GetModel("Content\\Fish01\\Fish01Tail", copyData: false, copyEPC: true);
		solidJet = BaseGame.Get().models.GetModel("Content\\Fish01\\Fish01Tail", copyData: false, copyEPC: true);
		wireModel = BaseGame.Get().models.GetModel("Content\\Fish01\\Fish01Wire", copyData: false, copyEPC: true);
		oluModel = BaseGame.Get().models.GetModel("Content\\Fish01\\Fish01Wire", copyData: false, copyEPC: true);
		oluJet = (wireJet = BaseGame.Get().models.GetModel("Content\\Fish01\\Fish01Tail", copyData: false, copyEPC: true));
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(model.epc, "DiffuseColor", (object)new Vector3(0.6f, 0f, 0.1f));
		BaseGame.Get().LinkEffect(model.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.SetAllEPCs(wireModel.epc, "xEnableLighting", false);
		BaseGame.Get().LinkEffect(wireModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.SetAllEPCs(solidJet.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(solidJet.epc, "DiffuseColor", (object)new Vector3(0.2f, 0.2f, 0f));
		BaseGame.Get().LinkEffect(solidJet.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.SetAllEPCs(wireJet.epc, "xEnableLighting", false);
		BaseGame.Get().LinkEffect(wireJet.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.SetAllEPCs(oluModel.epc, "xEnableLighting", false);
		EffectParameterCollectionRedux[] epc = oluModel.epc;
		Color lightGreen = Color.LightGreen;
		BaseGame.SetAllEPCs(epc, "DiffuseColor", ((Color)(ref lightGreen)).ToVector3() / 2f);
		BaseGame.Get().LinkEffect(oluModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.SetAllEPCs(oluJet.epc, "xEnableLighting", false);
		EffectParameterCollectionRedux[] epc2 = oluJet.epc;
		Color green = Color.Green;
		BaseGame.SetAllEPCs(epc2, "DiffuseColor", ((Color)(ref green)).ToVector3() / 2f);
		BaseGame.Get().LinkEffect(oluJet.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		Random random = new Random();
		pE = new PlaneEffect();
		for (int i = 0; i < 4; i++)
		{
			TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, 0.12f, 0.04f, 0.12f, 0.04f);
			treeNode.branchTree = false;
			treeNode.setColor(Color.Red);
			pE.addNode(treeNode);
		}
		ref Vector3 reference = ref pE.cornerNodes[0];
		reference = new Vector3(-0.5f, 0f, 0.5f);
		ref Vector3 reference2 = ref pE.cornerNodes[1];
		reference2 = new Vector3(0.5f, 0f, 0.5f);
		ref Vector3 reference3 = ref pE.cornerNodes[2];
		reference3 = new Vector3(-0.5f, 0f, -0.5f);
		ref Vector3 reference4 = ref pE.cornerNodes[3];
		reference4 = new Vector3(0.5f, 0f, -0.5f);
		pE.iteratePlane();
		pE.FinalizeEffect(centerTransform: true);
		oluPE = new PlaneEffect();
		for (int j = 0; j < 4; j++)
		{
			TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, 0.12f, 0.04f, 0.12f, 0.04f);
			treeNode.branchTree = false;
			treeNode.setColor(Color.Green);
			oluPE.addNode(treeNode);
		}
		ref Vector3 reference5 = ref oluPE.cornerNodes[0];
		reference5 = new Vector3(-0.5f, 0f, 0.5f);
		ref Vector3 reference6 = ref oluPE.cornerNodes[1];
		reference6 = new Vector3(0.5f, 0f, 0.5f);
		ref Vector3 reference7 = ref oluPE.cornerNodes[2];
		reference7 = new Vector3(-0.5f, 0f, -0.5f);
		ref Vector3 reference8 = ref oluPE.cornerNodes[3];
		reference8 = new Vector3(0.5f, 0f, -0.5f);
		oluPE.iteratePlane();
		oluPE.FinalizeEffect(centerTransform: true);
	}

	public Fish01(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		drawWater = true;
		if (attributes.ContainsKey("waterheight"))
		{
			waterHeight = LevelLoader.GetFloatFromAtt(attributes, "waterheight", 0f);
		}
		else
		{
			drawWater = false;
			waterHeight = -100000f;
		}
		drawRipple = LevelLoader.GetBoolFromAtt(attributes, "ripple", drawWater);
	}

	public Fish01(PathList _pathList, float _waterHeight, bool _drawRipple, FillMode _fill)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(_pathList, _waterHeight, _drawRipple, _fill, _oluMode: false);
	}

	public Fish01(PathList _pathList, float _waterHeight, bool _drawRipple, FillMode _fill, bool _oluMode)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		pathList = _pathList;
		waterHeight = _waterHeight;
		drawRipple = _drawRipple;
		fillMode = _fill;
		oluMode = _oluMode;
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

	public void DrawBody(GameTime gametime)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		posTransform = Transformation();
		BaseGame.Get().fogEffect.Parameters["WaterHeight"].SetValue(waterHeight);
		BaseGame.Get().fogEffect.Parameters["xPose"].SetValue(model.transforms[((ReadOnlyCollection<ModelMesh>)(object)model.model.Meshes)[0].ParentBone.Index]);
		BaseGame.Get().matStack.ApplyRawMatrix(posTransform);
		BaseGame.Get().DrawModelEffectStarted(ref model);
	}

	public void DrawJet(GameTime gametime)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().fogEffect.Parameters["WaterHeight"].SetValue(waterHeight);
		BaseGame.Get().fogEffect.Parameters["xPose"].SetValue(solidJet.transforms[((ReadOnlyCollection<ModelMesh>)(object)solidJet.model.Meshes)[0].ParentBone.Index]);
		BaseGame.Get().matStack.ApplyRawMatrix(Matrix.CreateScale(1.5f * jetScale) * posTransform);
		BaseGame.Get().DrawModelEffectStarted(ref solidJet);
	}

	public void DrawRipple(GameTime gametime)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (rE != null && !rE.done && drawRipple)
		{
			BaseGame.Get().matStack.PushMatrix();
			rE.Draw(gametime);
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(60f) * Matrix.CreateTranslation(ripplePos));
			(oluMode ? oluPE : pE).draw();
			BaseGame.Get().matStack.PopMatrix();
		}
	}

	public override void draw(GameTime gametime)
	{
	}

	public override void die()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		if (oluMode)
		{
			BaseGame.Get().fish01.oluEnemies.Remove(this);
		}
		else if ((int)fillMode == 3)
		{
			BaseGame.Get().fish01.fillEnemies.Remove(this);
		}
		else
		{
			BaseGame.Get().fish01.wireEnemies.Remove(this);
		}
		base.die();
	}

	public override void leave()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		if (oluMode)
		{
			BaseGame.Get().fish01.oluEnemies.Remove(this);
		}
		else if ((int)fillMode == 3)
		{
			BaseGame.Get().fish01.fillEnemies.Remove(this);
		}
		else
		{
			BaseGame.Get().fish01.wireEnemies.Remove(this);
		}
		base.leave();
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!toHit.skipSquare)
		{
			BaseGame.Get().ps.AddParticles(actualPos, Vector3.Forward * 25f, 2f, 180f, Vector3.Zero, 0f, 0.25f, 0.1f, 0.2f, partColor, 80, 0.0005f);
		}
		base.hit(toHit);
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
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		base.act(gametime);
		if (exists)
		{
			oldvel = vel;
			vel += (pos - actualPos) * accel * (float)gametime.ElapsedGameTime.TotalSeconds;
			if (((Vector3)(ref vel)).Length() > 2f * pathList.maxSpeed())
			{
				vel = Vector3.Normalize(vel) * 2f * pathList.maxSpeed();
			}
			actualPos += vel * (float)gametime.ElapsedGameTime.TotalSeconds;
			if (vel != oldvel)
			{
				up = BaseGame.GetUpVector(vel, up, oldvel);
			}
			if (rE != null)
			{
				rE.Update(gametime);
			}
			pCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			if (pCooldown < 0f)
			{
				BaseGame.Get().ps.AddParticles(actualPos, -vel, 0.2f, 30f, Vector3.Zero, 0f, 0.25f, 0.1f, 0.2f, partColor, 16, 0.00125f);
				pCooldown += pMax;
			}
			if (!launched && !launchDone && BaseGame.Get().channels[0] >= 0.3f)
			{
				launched = true;
			}
			else if (launched && !launchDone && BaseGame.Get().channels[0] <= 0.1f)
			{
				launchDone = true;
			}
			if (launchDone)
			{
				jetScale = 1f;
			}
			else if (launched)
			{
				jetScale = 1f + 10f * BaseGame.Get().channels[0];
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
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Invalid comparison between Unknown and I4
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Invalid comparison between Unknown and I4
		addTarget(Vector3.Zero, 1, 10);
		addCond(new NeverCondition());
		base.start();
		vel = BaseGame.FaceUpward(pathList);
		actualPos = pos;
		if (drawRipple)
		{
			ripplePos = new Vector3(pos.X, waterHeight, pos.Z);
			rE = new RippleEffect(ripplePos, 0.5f, 0.5f, 0f, 0.25f, 1.5f, _loop: false, 0f);
			rE.fxUpdate = BaseGame.GetFogEffect().Parameters;
			rE.done = !drawRipple;
		}
		partColor = (oluMode ? new Vector4(0f, 1f, 0f, 1f) : (((int)fillMode == 2) ? new Vector4(1f, 1f, 0f, 1f) : new Vector4(0.25f, 0.25f, 0f, 1f)));
		if (oluMode)
		{
			BaseGame.Get().fish01.oluEnemies.Add(this);
		}
		else if ((int)fillMode == 3)
		{
			BaseGame.Get().fish01.fillEnemies.Add(this);
		}
		else
		{
			BaseGame.Get().fish01.wireEnemies.Add(this);
		}
	}

	public override Vector3 getPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return actualPos;
	}

	public override string name()
	{
		return "[drone_0xF]";
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
