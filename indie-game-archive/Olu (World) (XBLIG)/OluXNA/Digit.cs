using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class Digit : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public static Dictionary<ModelBone, int> dBones;

	public static Dictionary<Vector3, int> sides;

	public static ModelWrapper model;

	public static ModelWrapper wire;

	public Vector3 actualPos;

	public Vector3 vel;

	public Vector3 up;

	public float maxSpeed;

	public float termLength;

	public float accel;

	public bool invince;

	public ModelOluAnimator anim;

	public AnimationController still;

	public float pCooldown;

	public float pMax;

	public string curSide;

	public Vector3 revVel;

	public bool launched;

	public Enemy enem;

	public float size;

	public ConditionSet velChangeCond;

	public int channelNum;

	public Digit()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
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
		launched = true;
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
		fillMode = (FillMode)2;
	}

	public static void LoadModel()
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		model = BaseGame.Get().models.GetModel("Content\\Digit\\Digit");
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", false);
		BaseGame.Get().LinkEffect(model.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		wire = BaseGame.Get().models.GetModel("Content\\Digit\\DigitWire");
		BaseGame.SetAllEPCs(wire.epc, "xEnableLighting", false);
		BaseGame.Get().LinkEffect(wire.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		dBones = new Dictionary<ModelBone, int>();
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)model.model.Bones).Count; i++)
		{
			if (!dBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i]))
			{
				dBones.Add(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i], i);
			}
		}
		sides = new Dictionary<Vector3, int>();
		sides.Add(Vector3.Down, dBones[model.model.Bones["Armature_Top"]]);
		sides.Add(Vector3.Up, dBones[model.model.Bones["Armature_Bottom"]]);
		sides.Add(Vector3.Forward, dBones[model.model.Bones["Armature_Front"]]);
		sides.Add(Vector3.Backward, dBones[model.model.Bones["Armature_Back"]]);
		sides.Add(Vector3.Left, dBones[model.model.Bones["Armature_Right"]]);
		sides.Add(Vector3.Right, dBones[model.model.Bones["Armature_Left"]]);
	}

	public Digit(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		size = LevelLoader.GetFloatFromAtt(attributes, "size", 5f);
		channelNum = LevelLoader.GetIntFromAtt(attributes, "channel", 10);
		invince = LevelLoader.GetBoolFromAtt(attributes, "inv", defVal: false);
	}

	public override Matrix Transformation()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		CheckLaunch();
		if (!launched)
		{
			return Matrix.CreateScale(size / Sled01.size) * Matrix.CreateTranslation(getPos()) * enem.Transformation();
		}
		return Matrix.CreateScale(size) * Matrix.CreateTranslation(getPos());
	}

	public override void draw(GameTime gametime)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().DrawModel(ref wire);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().DrawModel(ref model);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().matStack.PopMatrix();
	}

	public override void hit(TargetEffectBase toHit)
	{
		base.hit(toHit);
	}

	public override void die()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (invince)
		{
			BaseGame.Get().actualEnem++;
		}
		BaseGame.Get().ps.AddParticles(actualPos, Vector3.Forward * 25f, 2f, 180f, Vector3.Zero, 0f, 0.25f, 0.1f, 0.2f, new Vector4(1f, 1f, 1f, 1f), 80, 0.0005f);
		base.die();
	}

	public override void leave()
	{
		if (invince)
		{
			BaseGame.Get().actualEnem++;
		}
		base.leave();
	}

	public void CheckLaunch()
	{
		if (!launched && !enem.exists)
		{
			Launch();
		}
	}

	public void Launch()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = enem.Transformation();
		launched = true;
		setPos(Vector3.Transform(getPos(), val));
		((PRefLine)((PComboPath)pathList.publicPaths[pathList.curPathIndex]).first).start = getPos();
		PRefLine obj = (PRefLine)((PComboPath)pathList.publicPaths[pathList.curPathIndex]).first;
		obj.end += getPos();
		((PRefLine)((PComboPath)pathList.publicPaths[pathList.curPathIndex]).first).reset();
	}

	public override void act(GameTime gametime)
	{
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		CheckLaunch();
		if (!exists || !launched)
		{
			return;
		}
		velChangeCond.Update();
		if (velChangeCond.ConditionsMet())
		{
			pathList.curPathIndex++;
			foreach (Vector3 key in sides.Keys)
			{
				if (Vector3.Dot(vel, key) > 0f)
				{
					anim.bonePoses[sides[key]].enabled = false;
				}
			}
			vel = pathList.curDir() * ((PRefLine)((PComboPath)pathList.publicPaths[pathList.curPathIndex]).first).speed;
			foreach (Vector3 key2 in sides.Keys)
			{
				if (Vector3.Dot(vel, key2) > 0f)
				{
					anim.bonePoses[sides[key2]].enabled = true;
					curSide = anim.BonePoses[sides[key2]].Name;
				}
			}
			((GameComponent)anim).Update(BaseGame.Get().emptytime);
			((GameComponent)still).Update(BaseGame.Get().emptytime);
			velChangeCond.set.Clear();
			if (pathList.curPathIndex == pathList.publicPaths.Count - 1)
			{
				velChangeCond.set.Add(new NeverCondition());
			}
			else
			{
				velChangeCond.set.Add(new ChanCondition(channelNum, 0.9f));
			}
			velChangeCond.Start();
		}
		if (((Vector3)(ref actualPos)).LengthSquared() > termLength)
		{
			leave();
		}
		actualPos += vel * (float)gametime.ElapsedGameTime.TotalSeconds;
		pCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (pCooldown <= 0f)
		{
			BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, model.transforms[dBones[model.model.Bones[curSide]]] * Transformation()), new Vector3((0f - vel.X) * 2f, (0f - vel.Y) * 2f, (0f - vel.Z) * 2f), 0.5f, 20f, Vector3.Zero, 0f, 0.2f, 0.1f, 0f, new Vector4(1f, 0.3f, 0f, 1f), 20, 0.0025f);
			pCooldown += pMax;
		}
	}

	public override void start()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		if (!invince)
		{
			addTarget(Vector3.Zero, 1, 10);
		}
		addCond(new NeverCondition());
		anim = new ModelOluAnimator(BaseGame.Get().CoreGame, model, BaseGame.GetFogEffect());
		still = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["still"]);
		anim.bonePoses[dBones[model.model.Bones["Armature_Top"]]].enabled = false;
		anim.bonePoses[dBones[model.model.Bones["Armature_Bottom"]]].enabled = false;
		anim.bonePoses[dBones[model.model.Bones["Armature_Left"]]].enabled = false;
		anim.bonePoses[dBones[model.model.Bones["Armature_Right"]]].enabled = false;
		anim.bonePoses[dBones[model.model.Bones["Armature_Front"]]].enabled = false;
		((GameComponent)anim).Update(BaseGame.Get().emptytime);
		((GameComponent)anim).Update(BaseGame.Get().emptytime);
		base.start();
		if (invince)
		{
			BaseGame.Get().actualEnem--;
		}
		vel = pathList.curDir() * ((PRefLine)((PComboPath)pathList.publicPaths[0]).first).speed;
		curSide = "Armature_Back";
		actualPos = pos;
		velChangeCond = new ConditionSet();
		velChangeCond.set.Add(new ChanCondition(channelNum, 0.9f));
		velChangeCond.set.Add(new TimeCondition(0.10000000149011612));
		velChangeCond.Start();
		termLength = ((Vector3)(ref ((PRefLine)((PComboPath)pathList.publicPaths[pathList.publicPaths.Count - 1]).first).end)).LengthSquared();
	}

	public override Vector3 getPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return actualPos;
	}

	public override void setPos(Vector3 val)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		actualPos = val;
	}

	public override string name()
	{
		return "[diG_t]";
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
