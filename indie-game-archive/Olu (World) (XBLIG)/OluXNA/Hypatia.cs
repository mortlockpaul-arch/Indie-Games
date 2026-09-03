using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class Hypatia : Enemy
{
	private static Dictionary<ModelBone, int> hypatiaBones;

	private static Dictionary<ModelBone, int> oluBones;

	public static ModelWrapper model;

	public static ModelWrapper jet;

	public static ModelWrapper dot;

	public static ModelWrapper olu;

	public static ModelWrapper oluBack;

	public static ModelWrapper tail;

	public static int[] planeBones;

	public ModelOluAnimator anim;

	public AnimationController enter;

	public AnimationController open;

	public ModelOluAnimator[] oluAnim;

	public AnimationController[] escape;

	public BezierHelper[] mainBezier;

	public BulletC[] shots;

	public BulletPlaneCollection bpColl;

	public PathList bulletCircle;

	public List<EnemyState> states;

	public List<MusicPart> bossMusic;

	private Vector3 vel;

	private Vector3 up;

	private Vector3 tempPos;

	private Matrix _transformation;

	private bool dirty;

	private bool[] loaded;

	public int[] hp;

	public int[] bosshp;

	public Vector3[] targetColor;

	public float phaseCountdown;

	public float phaseMax;

	public float shootCooldown;

	public float maxCooldown;

	public float launchCooldown;

	public float maxLaunchCooldown;

	public float particleCooldown;

	public float particleMax;

	public float accel;

	public bool followPath;

	public Random r;

	public int curMesh;

	public int curIndex;

	public int curPart;

	private Vector3 startCol;

	private Vector3 endCol;

	private float baseHealth;

	protected string playMusic;

	public Hypatia()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		startCol = new Vector3(0f, 1f, 0f);
		endCol = new Vector3(0.7f, 0f, 0f);
		baseHealth = 64f;
		playMusic = "";
		base._002Ector();
		state = 0;
		states = new List<EnemyState>();
		bossMusic = new List<MusicPart>();
		loaded = new bool[20];
		hp = new int[20];
		bosshp = new int[20];
		targetColor = (Vector3[])(object)new Vector3[20];
		planeBones = new int[2];
		hp[0] = 100;
		hp[1] = 100;
		hp[2] = 50;
		hp[3] = 107;
		hp[4] = 107;
		bosshp[0] = 64;
		bosshp[1] = 64;
		bosshp[2] = 64;
		bosshp[3] = 64;
		bosshp[4] = 64;
		bosshp[5] = 64;
		bosshp[6] = 64;
		bosshp[7] = 128;
		bosshp[8] = 128;
		bosshp[9] = 192;
		for (int i = 0; i < targetColor.Length; i++)
		{
			ref Vector3 reference = ref targetColor[i];
			reference = new Vector3(0f, 0f, 1f);
		}
		ref Vector3 reference2 = ref targetColor[7];
		reference2 = new Vector3(1f, 1f, 1f);
		curMesh = (curIndex = 0);
		r = new Random();
		for (int j = 0; j < 20; j++)
		{
			loaded[j] = false;
		}
		attackCooldown = 5f;
		particleMax = 0.02f;
		hitPoints = 5;
		vel = Vector3.Backward;
		up = Vector3.Up;
		dirty = true;
	}

	public static void LoadModel()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		model = BaseGame.Get().models.GetModel("Content\\Hypatia\\Hypatia01", copyData: true, copyEPC: false);
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(model.epc, "DirLight0Direction", Vector3.Normalize(new Vector3(-1f, -0.5f, -0.5f)));
		BaseGame.SetAllEPCs(model.epc, "TextureMix", BaseGame.T_MUL);
		tail = BaseGame.Get().models.GetModel("Content\\Olu\\Tail", copyData: false, copyEPC: false);
		BaseGame.SetAllEPCs(tail.epc, "xEnableLighting", false);
		hypatiaBones = new Dictionary<ModelBone, int>();
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)model.model.Bones).Count; i++)
		{
			if (!hypatiaBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i]))
			{
				hypatiaBones.Add(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i], i);
			}
		}
		if (olu == null)
		{
			LoadOluModel();
		}
	}

	public static void LoadOluModel()
	{
		olu = BaseGame.Get().models.GetModel("Content\\Olu\\Olu", copyData: false, copyEPC: false);
		BaseGame.SetAllEPCs(olu.epc, "xEnableLighting", false);
		oluBack = BaseGame.Get().models.GetModel("Content\\Olu\\OluBack", copyData: false, copyEPC: false);
		BaseGame.SetAllEPCs(oluBack.epc, "xEnableLighting", false);
		oluBones = new Dictionary<ModelBone, int>();
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)olu.model.Bones).Count; i++)
		{
			if (!oluBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)olu.model.Bones)[i]))
			{
				oluBones.Add(((ReadOnlyCollection<ModelBone>)(object)olu.model.Bones)[i], i);
			}
		}
	}

	public Hypatia(Dictionary<string, string> attributes, XmlNode node)
		: this()
	{
	}

	public override void draw(GameTime gametime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		new Vector3(0f, 1f, 0f);
		new Vector3(0.5f, 0f, 0f);
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.SetAllEPCs(model.epc, "DirLight0Direction", Vector3.Normalize(Vector3.Transform(Vector3.Zero, model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_Root"]]] * Transformation()) * new Vector3(-1f, 1f, 1f)) - BaseGame.Get().playerPos);
		BaseGame.Get().DrawModel(ref model);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		if (bosshp[0] > 0)
		{
			BaseGame.SetAllEPCs(jet.epc, "DiffuseColor", targetColor[0]);
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateTranslation(new Vector3(0f, 0f, -0.26f)) * Matrix.CreateScale(3f, 3f, 1f) * Matrix.CreateRotationX(MathHelper.ToRadians(90f)) * model.transforms[hypatiaBones[model.model.Bones["EnemyArmA_RightJet"]]]);
			BaseGame.Get().DrawModel(ref jet, clearEpc: true);
			BaseGame.Get().matStack.PopMatrix();
		}
		if (bosshp[1] > 0)
		{
			BaseGame.SetAllEPCs(jet.epc, "DiffuseColor", targetColor[1]);
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateTranslation(new Vector3(0f, 0f, -0.26f)) * Matrix.CreateScale(3f, 3f, 1f) * Matrix.CreateRotationX(MathHelper.ToRadians(90f)) * model.transforms[hypatiaBones[model.model.Bones["EnemyArmA_LeftJet"]]]);
			BaseGame.Get().DrawModel(ref jet, clearEpc: true);
			BaseGame.Get().matStack.PopMatrix();
		}
		if (bosshp[2] > 0)
		{
			BaseGame.SetAllEPCs(dot.epc, "DiffuseColor", targetColor[2]);
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(3f, 3f, 3f) * Matrix.CreateTranslation(new Vector3(0f, -0.3f, -0.7f)) * model.transforms[hypatiaBones[model.model.Bones["EnemyArmA_Root"]]]);
			BaseGame.Get().DrawModel(ref dot, clearEpc: true);
			BaseGame.Get().matStack.PopMatrix();
		}
		if (bosshp[3] > 0)
		{
			BaseGame.SetAllEPCs(dot.epc, "DiffuseColor", targetColor[3]);
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_LeftUpDoor2"]]]);
			BaseGame.Get().DrawModel(ref dot, clearEpc: true);
			BaseGame.Get().matStack.PopMatrix();
		}
		if (bosshp[4] > 0)
		{
			BaseGame.SetAllEPCs(dot.epc, "DiffuseColor", targetColor[4]);
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_LeftDownDoor2"]]]);
			BaseGame.Get().DrawModel(ref dot, clearEpc: true);
			BaseGame.Get().matStack.PopMatrix();
		}
		if (bosshp[5] > 0)
		{
			BaseGame.SetAllEPCs(dot.epc, "DiffuseColor", targetColor[5]);
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_RightUpDoor2"]]]);
			BaseGame.Get().DrawModel(ref dot, clearEpc: true);
			BaseGame.Get().matStack.PopMatrix();
		}
		if (bosshp[6] > 0)
		{
			BaseGame.SetAllEPCs(dot.epc, "DiffuseColor", targetColor[6]);
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_RightDownDoor2"]]]);
			BaseGame.Get().DrawModel(ref dot, clearEpc: true);
			BaseGame.Get().matStack.PopMatrix();
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().matStack.PopMatrix();
		foreach (EnemyState state in states)
		{
			state.Draw(gametime);
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
			_transformation = Matrix.CreateScale(new Vector3(10f, 10f, -10f)) * Matrix.CreateTranslation(getPos());
			dirty = false;
		}
		return _transformation;
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		BoneModelTarget boneModelTarget = (BoneModelTarget)toHit.eTarget;
		bosshp[boneModelTarget.id]--;
		if (boneModelTarget.fillMode != toHit.fillMode)
		{
			bosshp[boneModelTarget.id]--;
		}
		ref Vector3 reference = ref targetColor[boneModelTarget.id];
		reference = Vector3.Lerp(endCol, startCol, (float)bosshp[boneModelTarget.id] / baseHealth);
		if (!toHit.skipSquare)
		{
			BaseGame.Get().ps.AddParticles(toHit.eTarget.absolutePos(), Vector3.Forward * 12f, 2f, 180f, Vector3.Zero, 0f, 0.75f, 0.5f, 0.2f, new Vector4(targetColor[boneModelTarget.id] + new Vector3(0.2f, 0.2f, 0.2f), 1f), 320, 6.25E-05f);
		}
	}

	public override void act(GameTime gametime)
	{
		if (!exists)
		{
			return;
		}
		if (enter != null)
		{
			((GameComponent)enter).Update(gametime);
		}
		if (open != null)
		{
			((GameComponent)open).Update(gametime);
		}
		foreach (EnemyState state in states)
		{
			state.Update(gametime);
		}
		for (int num = states.Count - 1; num >= 0; num--)
		{
			if (states[num].Remove())
			{
				states.Add(states[num].GetNewState());
				states.RemoveAt(num);
				states[states.Count - 1].Update(gametime);
				while (states[states.Count - 1].Remove())
				{
					states.Add(states[states.Count - 1].GetNewState());
					states.RemoveAt(states.Count - 2);
					states[states.Count - 1].Update(gametime);
				}
			}
		}
		dirty = true;
	}

	public override void start()
	{
		new Random();
		base.start();
		anim = new ModelOluAnimator(BaseGame.Get().CoreGame, model, BaseGame.GetFogEffect());
		jet = BaseGame.Get().models.GetModel("Content\\Fish01\\Fish01Tail", copyData: false, copyEPC: true);
		BaseGame.SetAllEPCs(jet.epc, "xEnableLighting", false);
		dot = BaseGame.Get().models.GetModel("Content\\Bird01\\Dot", copyData: false, copyEPC: true);
		BaseGame.SetAllEPCs(dot.epc, "xEnableLighting", false);
		model.ResetIndicesToDraw();
		oluAnim = new ModelOluAnimator[2];
		oluAnim[0] = new ModelOluAnimator(BaseGame.Get().CoreGame, olu, BaseGame.GetFogEffect());
		oluAnim[1] = new ModelOluAnimator(BaseGame.Get().CoreGame, oluBack, BaseGame.GetFogEffect());
		escape = new AnimationController[2];
		escape[0] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[0].Animations["appear"]);
		escape[1] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[1].Animations["appear"]);
		BaseGame.RunController(oluAnim[0], escape[0]);
		BaseGame.RunController(oluAnim[1], escape[1]);
		((GameComponent)oluAnim[0]).Enabled = false;
		((GameComponent)oluAnim[1]).Enabled = false;
		escape[0].IsLooping = false;
		escape[1].IsLooping = false;
		mainBezier = new BezierHelper[4];
		shots = new BulletC[1000];
		states = new List<EnemyState>();
		if (BaseGame.quickload)
		{
			states.Add(FrontForwardState());
		}
		else
		{
			states.Add(EnterState());
		}
		AddCue(0, "BTPattern3", 0, 0);
		AddCue(0, "BTPattern5", 0, 0);
		AddCue(0, "BTPattern20", 0, 8);
		addCond(new NeverCondition());
	}

	public void AddCue(int beat, string name, int playMeas, int loopMeas)
	{
		bossMusic.Add(new MusicPart(beat, name, playMeas, loopMeas));
	}

	public override Vector3 getPos()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(0f, 0f, 0f);
	}

	public override string name()
	{
		return "[h{Y}patia]";
	}

	public override bool Check(int numEnem)
	{
		return true;
	}

	public override void HitSound(int lockNum, float volume)
	{
	}

	public void FillPlane(Matrix boneMat, Vector3 start, Vector3 right, Vector3 up, int rows, int columns, int offset, int partNum)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = default(Matrix);
		((Matrix)(ref val))._002Ector(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
		val = Matrix.Add(boneMat, val);
		Vector3 val2 = Vector3.Transform(start, val);
		val.M41 = 0f;
		val.M42 = 0f;
		val.M43 = 0f;
		Vector3 val3 = Vector3.Transform(right, val);
		Vector3 val4 = Vector3.Transform(up, val);
		Vector3 val5 = Vector3.Normalize(Vector3.Cross(right, up));
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				shots[offset + i * columns + j] = new BulletC(val2 + (float)j / (float)(columns - 1) * val3 + (float)i / (float)(rows - 1) * val4, val5, Vector3.Normalize(val4), this, partNum);
				shots[offset + i * columns + j].start();
				BaseGame.Get().enems.Add(shots[offset + i * columns + j]);
			}
		}
	}

	public void NormalUpdate(GameTime gametime)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		if (((GameComponent)anim).Enabled)
		{
			((GameComponent)anim).Update(gametime);
		}
		BaseGame.Get().MovePlayerDir(Vector3.Normalize(Vector3.Transform(Vector3.Zero, model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_Root"]]] * Transformation()) * new Vector3(1f, 1f, 1f)));
		if (!BaseGame.Get().FREEZE_ON)
		{
			PlayMusic(gametime);
		}
		if (playMusic != "" && bossMusic.Count > 2 && bossMusic[2].curMeasure == 7 && BaseGame.Get().curBeat == 15)
		{
			switch (playMusic)
			{
			case "BTPattern19":
				AddCue(0, playMusic, 0, 8);
				break;
			case "BTPattern10":
				AddCue(0, "BTPattern10", 0, 4);
				AddCue(0, "BTPattern11", 2, 8);
				AddCue(0, "BTPattern12", 6, 8);
				break;
			case "BTPattern15":
				AddCue(0, "BTPattern15", 0, 4);
				AddCue(0, "BTPattern16", 2, 8);
				AddCue(0, "BTPattern17", 6, 8);
				break;
			}
			playMusic = "";
		}
		particleCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (particleCooldown < 0f)
		{
			if (bosshp[0] > 0)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(new Vector3(0f, 0.5f, 0f), model.transforms[((ReadOnlyCollection<ModelBone>)(object)model.model.Bones).IndexOf(model.model.Bones["EnemyArmA_RightJet"])] * Transformation()), Vector3.Forward * 25f, 0.2f, 10f, Vector3.Zero, 0f, 1.5f, 0.5f, 0.2f, new Vector4(targetColor[0], 1f), 32, 0.000625f);
			}
			if (bosshp[1] > 0)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(new Vector3(0f, 0.5f, 0f), model.transforms[((ReadOnlyCollection<ModelBone>)(object)model.model.Bones).IndexOf(model.model.Bones["EnemyArmA_LeftJet"])] * Transformation()), Vector3.Forward * 25f, 0.2f, 10f, Vector3.Zero, 0f, 1.5f, 0.5f, 0.2f, new Vector4(targetColor[1], 1f), 32, 0.000625f);
			}
			if (bosshp[2] > 0)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(new Vector3(0f, -0.3f, -0.7f), model.transforms[hypatiaBones[model.model.Bones["EnemyArmA_Root"]]] * Transformation()), Vector3.Forward * 15f, 0.2f, 180f, Vector3.Zero, 0f, 0.25f, 0.25f, 0f, new Vector4(targetColor[2], 1f), 32, 0.000625f);
			}
			particleCooldown += particleMax;
		}
		if (bosshp[0] <= 0)
		{
			anim.BonePoses[hypatiaBones[model.model.Bones["EnemyArmA_RightDoorRoot"]]].enabled = false;
			Deactivate(0, 100);
		}
		if (bosshp[1] <= 0)
		{
			anim.BonePoses[hypatiaBones[model.model.Bones["EnemyArmA_LeftDoorRoot"]]].enabled = false;
			Deactivate(100, 100);
		}
		if (bosshp[3] <= 0)
		{
			anim.BonePoses[hypatiaBones[model.model.Bones["EnemyArmB_LeftUpDoor1"]]].enabled = false;
			ShootAll(250, 25);
			ShootAll(357, 25);
		}
		if (bosshp[4] <= 0)
		{
			anim.BonePoses[hypatiaBones[model.model.Bones["EnemyArmB_LeftDownDoor1"]]].enabled = false;
			ShootAll(275, 16);
			ShootAll(316, 16);
			ShootAll(382, 16);
			ShootAll(423, 16);
		}
		if (bosshp[5] <= 0)
		{
			anim.BonePoses[hypatiaBones[model.model.Bones["EnemyArmB_RightUpDoor1"]]].enabled = false;
			ShootAll(291, 25);
			ShootAll(398, 25);
		}
		if (bosshp[6] <= 0)
		{
			anim.BonePoses[hypatiaBones[model.model.Bones["EnemyArmB_RightDownDoor1"]]].enabled = false;
			ShootAll(332, 25);
			ShootAll(439, 25);
		}
	}

	public void PlayMusic(GameTime gametime)
	{
		for (int num = bossMusic.Count - 1; num >= 0; num--)
		{
			bossMusic[num].Update(gametime);
			if (bossMusic[num].done)
			{
				bossMusic.RemoveAt(num);
			}
		}
	}

	public void Shoot(int offset, int groupSize)
	{
		int num = 0;
		int num2 = r.Next(offset, offset + groupSize);
		while ((shots[num2] == null || shots[num2].IsLaunched) && num < 35)
		{
			num2 = r.Next(offset, offset + groupSize);
			num++;
		}
		shots[num2].Launch();
	}

	public void ShootAll(int offset, int groupSize)
	{
		for (int i = 0; i < groupSize; i++)
		{
			if (shots[i + offset] != null && !shots[i + offset].IsLaunched && shots[i + offset].active)
			{
				shots[i + offset].Launch();
			}
		}
	}

	public bool AlwaysRemove(ConditionSet cs)
	{
		return true;
	}

	private void Activate(int offset, int number)
	{
		for (int i = 0; i < number; i++)
		{
			if (shots[offset + i] != null && !shots[offset + i].IsLaunched)
			{
				shots[offset + i].active = true;
			}
		}
	}

	private void Deactivate(int offset, int number)
	{
		for (int i = 0; i < number; i++)
		{
			if (shots[offset + i] != null && !shots[offset + i].IsLaunched)
			{
				shots[offset + i].active = false;
			}
		}
	}

	private void PermDeactivate(int offset, int number)
	{
		for (int i = 0; i < number; i++)
		{
			if (shots[offset + i] != null && !shots[offset + i].IsLaunched)
			{
				shots[offset + i].exists = true;
				shots[offset + i].hitPoints = -1;
			}
		}
	}

	private EnemyState EnterState()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, null, null, RightOpenState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(6.0));
		enemyState.condSet.Start();
		enter = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["enter"]);
		BaseGame.RunController(anim, enter);
		enter.IsLooping = false;
		return enemyState;
	}

	private EnemyState RightOpenState()
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(RightOpenUpdate, null, LeftRemove, RightCloseState);
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["rightopen"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		shootCooldown = (maxCooldown = 400f);
		if (!loaded[0])
		{
			((GameComponent)anim).Update((GameTime)null);
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmA_Root"]]] * Transformation(), new Vector3(1.4f, 0.8f, -1.2f), new Vector3(0f, 0f, 2.4f), new Vector3(-0.4f, 0.8f, 0f), 5, 20, 0, 0);
			addTarget(new Vector3(0f, 0.5f, 0f), bosshp[0], 10, ref model, 0, "EnemyArmA_RightJet");
			addTarget(new Vector3(0f, 0.5f, 0f), bosshp[1], 10, ref model, 1, "EnemyArmA_LeftJet");
			ref Vector3 reference = ref targetColor[0];
			reference = new Vector3(0f, 1f, 0f);
			ref Vector3 reference2 = ref targetColor[1];
			reference2 = new Vector3(0f, 1f, 0f);
			loaded[0] = true;
		}
		else
		{
			Activate(0, 100);
		}
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(10.0));
		enemyState.condSet.Start();
		return enemyState;
	}

	private void RightOpenUpdate(GameTime gametime)
	{
		NormalUpdate(gametime);
		shootCooldown -= (float)gametime.ElapsedGameTime.TotalMilliseconds;
		if (shootCooldown <= 0f && hp[0] > 0)
		{
			Shoot(0, 100);
			shootCooldown += maxCooldown;
		}
	}

	public bool LeftRemove(ConditionSet cs)
	{
		if (bosshp[0] <= 0)
		{
			return true;
		}
		return cs.ConditionsMet();
	}

	private EnemyState RightCloseState()
	{
		EnemyState enemyState = new EnemyState(CloseUpdate, null, LeftRemove, LeftToCenterState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(5.0));
		enemyState.condSet.Start();
		phaseMax = (phaseCountdown = 1.5f);
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["rightclose"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		return enemyState;
	}

	private void CloseUpdate(GameTime gametime)
	{
		NormalUpdate(gametime);
		phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (phaseCountdown < 0f)
		{
			Deactivate(0, 100);
		}
	}

	private EnemyState LeftToCenterState()
	{
		EnemyState enemyState = ((bosshp[1] > 0) ? new EnemyState(NormalUpdate, null, null, CenterToRightState) : ((bosshp[0] > 0) ? new EnemyState(NormalUpdate, null, null, CenterToLeftState) : new EnemyState(NormalUpdate, null, null, CenterMoveForwardState)));
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(3.0));
		enemyState.condSet.Start();
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["lefttocenter"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		return enemyState;
	}

	private EnemyState CenterToRightState()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, null, null, LeftOpenState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(3.0));
		enemyState.condSet.Start();
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["centertoright"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		return enemyState;
	}

	private EnemyState LeftOpenState()
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(LeftOpenUpdate, null, RightRemove, LeftCloseState);
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["leftopen"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		shootCooldown = (maxCooldown = 400f);
		if (!loaded[1])
		{
			((GameComponent)anim).Update((GameTime)null);
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmA_Root"]]] * Transformation(), new Vector3(-1.4f, 0.8f, -1.2f), new Vector3(0.4f, 0.8f, 0f), new Vector3(0f, 0f, 2.4f), 20, 5, 100, 1);
			loaded[1] = true;
		}
		else
		{
			Activate(100, 100);
		}
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(10.0));
		enemyState.condSet.Start();
		return enemyState;
	}

	private void LeftOpenUpdate(GameTime gametime)
	{
		NormalUpdate(gametime);
		shootCooldown -= (float)gametime.ElapsedGameTime.TotalMilliseconds;
		if (shootCooldown <= 0f && hp[1] > 0)
		{
			Shoot(100, 100);
			shootCooldown += maxCooldown;
		}
	}

	public bool RightRemove(ConditionSet cs)
	{
		if (bosshp[1] <= 0)
		{
			return true;
		}
		return cs.ConditionsMet();
	}

	private EnemyState LeftCloseState()
	{
		EnemyState enemyState = new EnemyState(CloseUpdate, null, RightRemove, RightToCenterState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(5.0));
		enemyState.condSet.Start();
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["leftclose"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		return enemyState;
	}

	private EnemyState RightToCenterState()
	{
		EnemyState enemyState = ((bosshp[0] > 0) ? new EnemyState(NormalUpdate, null, null, CenterToLeftState) : ((bosshp[1] > 0) ? new EnemyState(NormalUpdate, null, null, CenterToRightState) : new EnemyState(NormalUpdate, null, null, CenterMoveForwardState)));
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(3.0));
		enemyState.condSet.Start();
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["righttocenter"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		Deactivate(100, 100);
		return enemyState;
	}

	private EnemyState CenterToLeftState()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, null, null, RightOpenState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(3.0));
		enemyState.condSet.Start();
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["centertoleft"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		return enemyState;
	}

	private EnemyState CenterMoveForwardState()
	{
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(CenterUpdate, null, CenterRemove, CenterGrowState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(3.0));
		enemyState.condSet.Start();
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["centermoveforward"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		shootCooldown = (maxCooldown = 400f);
		playMusic = "BTPattern19";
		if (!loaded[2])
		{
			((GameComponent)anim).Update((GameTime)null);
			((GameComponent)anim).Enabled = false;
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmA_Root"]]] * Transformation(), new Vector3(1f, 0f, -1f), new Vector3(-2f, 0f, 0f), new Vector3(0f, 0f, 2f), 5, 5, 200, 2);
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmA_Root"]]] * Transformation(), new Vector3(1f, 0.5f, -1f), new Vector3(-2f, 0f, 0f), new Vector3(0f, 0f, 2f), 5, 5, 225, 2);
			addTarget(new Vector3(0f, -1.45f, 2.25f), bosshp[2], 10, ref model, 2, "EnemyArmA_Root");
			loaded[2] = true;
			ref Vector3 reference = ref targetColor[2];
			reference = new Vector3(0f, 1f, 0f);
		}
		else
		{
			Activate(200, 50);
		}
		((GameComponent)open).Enabled = false;
		return enemyState;
	}

	private void CenterUpdate(GameTime gametime)
	{
		NormalUpdate(gametime);
		if (bosshp[2] <= 0)
		{
			((GameComponent)anim).Update(gametime);
			if (!((GameComponent)open).Enabled)
			{
				((GameComponent)anim).Enabled = true;
				((GameComponent)open).Enabled = true;
				anim.BonePoses[hypatiaBones[model.model.Bones["EnemyArmA_Root"]]].enabled = false;
				ShootAll(200, 50);
			}
		}
		else
		{
			shootCooldown -= (float)gametime.ElapsedGameTime.TotalMilliseconds;
			if (shootCooldown <= 0f && hp[2] > 0)
			{
				Shoot(200, 50);
				shootCooldown += maxCooldown;
			}
		}
	}

	public bool CenterRemove(ConditionSet cs)
	{
		if (bosshp[2] <= 0)
		{
			return open.ElapsedTime > 3;
		}
		return false;
	}

	private EnemyState CenterGrowState()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, null, null, CenterToVerticalState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(4.0));
		enemyState.condSet.Start();
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["centergrow"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		return enemyState;
	}

	private EnemyState CenterToVerticalState()
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(NormalUpdate, null, null, VerticalStillState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.5));
		enemyState.condSet.Start();
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["centertovertical"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		if (!loaded[3])
		{
			((GameComponent)anim).Update((GameTime)null);
			addTarget(new Vector3(0f, 0f, 0f), bosshp[3], 10, ref model, 3, "EnemyArmB_LeftUpDoor2");
			addTarget(new Vector3(0f, 0f, 0f), bosshp[4], 10, ref model, 4, "EnemyArmB_LeftDownDoor2");
			addTarget(new Vector3(0f, 0f, 0f), bosshp[5], 10, ref model, 5, "EnemyArmB_RightUpDoor2");
			addTarget(new Vector3(0f, 0f, 0f), bosshp[6], 10, ref model, 6, "EnemyArmB_RightDownDoor2");
			ref Vector3 reference = ref targetColor[3];
			reference = new Vector3(0f, 1f, 0f);
			ref Vector3 reference2 = ref targetColor[4];
			reference2 = new Vector3(0f, 1f, 0f);
			ref Vector3 reference3 = ref targetColor[5];
			reference3 = new Vector3(0f, 1f, 0f);
			ref Vector3 reference4 = ref targetColor[6];
			reference4 = new Vector3(0f, 1f, 0f);
			loaded[3] = true;
		}
		return enemyState;
	}

	private EnemyState VerticalStillState()
	{
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(VerticalUpdate, null, HorizVertRemove, VerticalToHorizontalState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(10.0));
		enemyState.condSet.Start();
		if (bossMusic.Count < 5)
		{
			playMusic = "BTPattern10";
		}
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["verticalstill"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		shootCooldown = (maxCooldown = 400f);
		if (!loaded[4])
		{
			((GameComponent)anim).Update((GameTime)null);
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_Root"]]] * Transformation(), new Vector3(1f, -1f, 0.3f), new Vector3(-2f, 0f, 0f), new Vector3(0f, 2f, 0f), 5, 5, 250, 3);
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_Root"]]] * Transformation(), new Vector3(0.75f, -0.75f, 0.4f), new Vector3(-1.5f, 0f, 0f), new Vector3(0f, 1.5f, 0f), 4, 4, 275, 3);
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_Root"]]] * Transformation(), new Vector3(1f, -1f, 0.5f), new Vector3(-2f, 0f, 0f), new Vector3(0f, 2f, 0f), 5, 5, 291, 3);
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_Root"]]] * Transformation(), new Vector3(0.75f, -0.75f, 0.6f), new Vector3(-1.5f, 0f, 0f), new Vector3(0f, 1.5f, 0f), 4, 4, 316, 3);
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_Root"]]] * Transformation(), new Vector3(1f, -1f, 0.7f), new Vector3(-2f, 0f, 0f), new Vector3(0f, 2f, 0f), 5, 5, 332, 3);
			loaded[4] = true;
		}
		else
		{
			Activate(250, 107);
		}
		return enemyState;
	}

	private void VerticalUpdate(GameTime gametime)
	{
		NormalUpdate(gametime);
		shootCooldown -= (float)gametime.ElapsedGameTime.TotalMilliseconds;
		if (shootCooldown <= 0f && hp[3] > 0)
		{
			Shoot(250, 107);
			shootCooldown += maxCooldown;
		}
	}

	public bool HorizVertRemove(ConditionSet cs)
	{
		if (bosshp[3] <= 0 && bosshp[4] <= 0 && bosshp[5] <= 0 && bosshp[6] <= 0)
		{
			return true;
		}
		return cs.ConditionsMet();
	}

	private EnemyState VerticalToHorizontalState()
	{
		EnemyState enemyState;
		if (bosshp[3] <= 0 && bosshp[4] <= 0 && bosshp[5] <= 0 && bosshp[6] <= 0)
		{
			enemyState = new EnemyState(NormalUpdate, null, null, CenterFallState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new TimeCondition(2.5));
		}
		else
		{
			enemyState = new EnemyState(NormalUpdate, null, null, HorizontalStillState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new TimeCondition(3.0));
		}
		enemyState.condSet.Start();
		Deactivate(250, 214);
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["verticaltohorizontal"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		return enemyState;
	}

	private EnemyState HorizontalStillState()
	{
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(HorizontalUpdate, null, HorizVertRemove, HorizontalToVerticalState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(10.0));
		enemyState.condSet.Start();
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["horizontalstill"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		shootCooldown = (maxCooldown = 400f);
		if (!loaded[5])
		{
			((GameComponent)anim).Update((GameTime)null);
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_Root"]]] * Transformation(), new Vector3(1f, -0.25f, 0.3f), new Vector3(-2f, 0f, 0f), new Vector3(0f, 1f, 0f), 5, 5, 357, 4);
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_Root"]]] * Transformation(), new Vector3(0.75f, -0.125f, 0.4f), new Vector3(-1.5f, 0f, 0f), new Vector3(0f, 0.75f, 0f), 4, 4, 382, 4);
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_Root"]]] * Transformation(), new Vector3(1f, -0.25f, 0.5f), new Vector3(-2f, 0f, 0f), new Vector3(0f, 1f, 0f), 5, 5, 398, 4);
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_Root"]]] * Transformation(), new Vector3(0.75f, -0.125f, 0.6f), new Vector3(-1.5f, 0f, 0f), new Vector3(0f, 0.75f, 0f), 4, 4, 423, 4);
			FillPlane(model.transforms[hypatiaBones[model.model.Bones["EnemyArmB_Root"]]] * Transformation(), new Vector3(1f, -0.25f, 0.7f), new Vector3(-2f, 0f, 0f), new Vector3(0f, 1f, 0f), 5, 5, 439, 4);
			loaded[5] = true;
		}
		else
		{
			Activate(357, 107);
		}
		return enemyState;
	}

	private void HorizontalUpdate(GameTime gametime)
	{
		NormalUpdate(gametime);
		shootCooldown -= (float)gametime.ElapsedGameTime.TotalMilliseconds;
		if (shootCooldown <= 0f && hp[4] > 0)
		{
			Shoot(357, 107);
			shootCooldown += maxCooldown;
		}
	}

	private EnemyState HorizontalToVerticalState()
	{
		EnemyState enemyState;
		if (bosshp[3] <= 0 && bosshp[4] <= 0 && bosshp[5] <= 0 && bosshp[6] <= 0)
		{
			enemyState = new EnemyState(NormalUpdate, null, AlwaysRemove, CenterFallState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new TimeCondition(-1.0));
		}
		else
		{
			enemyState = new EnemyState(NormalUpdate, null, null, VerticalStillState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new TimeCondition(3.0));
		}
		enemyState.condSet.Start();
		Deactivate(250, 214);
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["horizontaltovertical"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		return enemyState;
	}

	private EnemyState CenterFallState()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, null, null, FrontForwardState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.0));
		enemyState.condSet.Start();
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["centerfall"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		return enemyState;
	}

	private EnemyState FrontForwardState()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, null, null, FrontExpandState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.0));
		enemyState.condSet.Start();
		playMusic = "BTPattern15";
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["frontforward"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		return enemyState;
	}

	private EnemyState FrontExpandState()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, null, null, FrontWallStillState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(3.0));
		enemyState.condSet.Start();
		anim.BonePoses[hypatiaBones[model.model.Bones["EnemyArmA_Root"]]].enabled = false;
		anim.BonePoses[hypatiaBones[model.model.Bones["EnemyArmB_LeftUpDoor1"]]].enabled = false;
		anim.BonePoses[hypatiaBones[model.model.Bones["EnemyArmB_LeftDownDoor1"]]].enabled = false;
		anim.BonePoses[hypatiaBones[model.model.Bones["EnemyArmB_RightUpDoor1"]]].enabled = false;
		anim.BonePoses[hypatiaBones[model.model.Bones["EnemyArmB_RightDownDoor1"]]].enabled = false;
		bosshp[0] = 0;
		bosshp[1] = 0;
		bosshp[2] = 0;
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["frontexpand"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		return enemyState;
	}

	private EnemyState FrontWallStillState()
	{
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(FrontWallStillUpdate, null, FrontWallStillRemove, FrontFloorGrowState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		shootCooldown = (maxCooldown = 50f);
		curPart = 7;
		curMesh = 2;
		curIndex = 0;
		launchCooldown = (maxLaunchCooldown = 1000f);
		accel = 1f;
		followPath = false;
		bpColl = new BulletPlaneCollection(ref model);
		bpColl.start();
		BaseGame.Get().enems.Add(bpColl);
		bulletCircle = new PathList();
		bulletCircle.Add(new PCircle(Vector3.Transform(Vector3.Zero, model.transforms[hypatiaBones[model.model.Bones["EnemyArmC_Root"]]] * Transformation()) + new Vector3(0f, 0f, -15f), 20f, 10f, _loop: true));
		if (!loaded[6])
		{
			planeBones[0] = model.boneNames["EnemyArmC_LeftUpWall"][1];
			planeBones[1] = model.boneNames["EnemyArmC_RightDownWall"][1];
			loaded[6] = true;
			shootCooldown = 0f;
			launchCooldown = 0f;
		}
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["frontwallstill"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		return enemyState;
	}

	private void FrontWallStillUpdate(GameTime gametime)
	{
		NormalUpdate(gametime);
		shootCooldown -= (float)gametime.ElapsedGameTime.TotalMilliseconds;
		if (shootCooldown <= 0f && curMesh >= 0)
		{
			ShootTile();
			shootCooldown += maxCooldown;
		}
		launchCooldown -= (float)gametime.ElapsedGameTime.TotalMilliseconds;
		if (launchCooldown <= 0f)
		{
			bpColl.Launch();
			launchCooldown += maxLaunchCooldown;
		}
		if (bosshp[7] <= 0 && !loaded[7])
		{
			planeBones[0] = model.boneNames["EnemyArmC_LeftDownWall"][1];
			planeBones[1] = model.boneNames["EnemyArmC_RightUpWall"][1];
			curPart = 8;
			curMesh = 2;
			curIndex = 0;
			loaded[7] = true;
		}
	}

	public bool FrontWallStillRemove(ConditionSet cs)
	{
		if (bosshp[7] <= 0 && bosshp[8] <= 0)
		{
			return true;
		}
		return cs.ConditionsMet();
	}

	private void ShootTile()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		while (curIndex < model.indices[curMesh].Length - 1 && !flag)
		{
			int num = model.vertices[curMesh][model.indices[curMesh][curIndex]].boneNum(0);
			for (int i = 0; i < planeBones.Length; i++)
			{
				if (planeBones[i] == num)
				{
					flag = true;
				}
			}
			if (flag)
			{
				bpColl.AddPlane(this, ref model, curMesh, curIndex, num, new Vector3(0.7f, 1f, 0.7f), new Vector3(0f, 1f, 0f), bulletCircle, curPart, accel, followPath);
				SplitList(curMesh, curIndex);
			}
			curIndex += 3;
		}
		if (!flag)
		{
			curMesh = -1;
		}
	}

	private void SplitList(int _mesh, int _index)
	{
		bool flag = false;
		for (int i = 0; i < model.indicesToDraw[_mesh].Count - 1; i += 2)
		{
			if (flag)
			{
				break;
			}
			if (_index < model.indicesToDraw[_mesh][i] || _index > model.indicesToDraw[_mesh][i + 1])
			{
				continue;
			}
			flag = true;
			if (_index == model.indicesToDraw[_mesh][i])
			{
				if (_index == model.indicesToDraw[_mesh][i + 1] - 2)
				{
					model.indicesToDraw[_mesh].RemoveAt(i);
					model.indicesToDraw[_mesh].RemoveAt(i);
				}
				else
				{
					model.indicesToDraw[_mesh][i] = _index + 3;
				}
			}
			else if (_index == model.indicesToDraw[_mesh][i + 1] - 2)
			{
				model.indicesToDraw[_mesh][i + 1] = _index - 1;
			}
			else
			{
				model.indicesToDraw[_mesh].Insert(i + 1, _index + 3);
				model.indicesToDraw[_mesh].Insert(i + 1, _index - 1);
			}
		}
	}

	private EnemyState FrontFloorGrowState()
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(FrontFloorGrowUpdate, FrontFloorGrowDraw, null, FrontFloorStillState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.0));
		enemyState.condSet.Start();
		shootCooldown = 0f;
		particleCooldown = (particleMax = 0.02f);
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["frontfloorgrow"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		((GameComponent)anim).Update((GameTime)null);
		tempPos = Vector3.Transform(Vector3.Zero, model.transforms[hypatiaBones[model.model.Bones["EnemyArmC_Root"]]] * Transformation());
		return enemyState;
	}

	private void FrontFloorGrowUpdate(GameTime gametime)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		NormalUpdate(gametime);
		shootCooldown += (float)gametime.ElapsedGameTime.TotalSeconds;
		particleCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (particleCooldown <= 0f)
		{
			BaseGame.Get().ps.AddParticles(tempPos, Vector3.Forward * 25f * shootCooldown, 0.1f, 180f, Vector3.Zero, 0f, 0.2f, 0.02f, 0f, new Vector4(1f, 1f, 1f, 1f), 250, 8E-05f);
			particleCooldown += particleMax;
		}
	}

	private void FrontFloorGrowDraw(GameTime gametime)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(shootCooldown * 3f) * model.transforms[hypatiaBones[model.model.Bones["EnemyArmC_Root"]]] * Transformation());
		BaseGame.SetAllEPCs(dot.epc, "DiffuseColor", targetColor[7]);
		BaseGame.SetAllEPCs(dot.epc, "EmissiveColor", targetColor[7]);
		BaseGame.Get().DrawModel(ref dot, clearEpc: true);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
	}

	private EnemyState FrontFloorStillState()
	{
		EnemyState enemyState = new EnemyState(FrontFloorStillUpdate, FrontFloorStillDraw, FrontFloorStillRemove, DieState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["frontfloorstill"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		if (bpColl == null)
		{
			bpColl = new BulletPlaneCollection(ref model);
			bpColl.start();
			BaseGame.Get().enems.Add(bpColl);
		}
		if (bulletCircle == null)
		{
			bulletCircle = new PathList();
		}
		if (!loaded[8])
		{
			planeBones = new int[1];
			planeBones[0] = model.boneNames["EnemyArmC_Root"][1];
			loaded[8] = true;
			shootCooldown = 0f;
			launchCooldown = 0f;
			curPart = 9;
			curMesh = 2;
			curIndex = 0;
			accel = 10f;
			followPath = true;
			shootCooldown = (maxCooldown = 30f);
			launchCooldown = (maxLaunchCooldown = 1000f);
		}
		return enemyState;
	}

	private void FrontFloorStillUpdate(GameTime gametime)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		NormalUpdate(gametime);
		shootCooldown -= (float)gametime.ElapsedGameTime.TotalMilliseconds;
		if (shootCooldown <= 0f && curMesh >= 0)
		{
			bulletCircle.publicPaths.Clear();
			Vector3 randVect = BaseGame.GetRandVect(Vector3.Up * 25f, 180f);
			float num = (float)Math.Atan(randVect.Z / randVect.X);
			if (randVect.X <= 0f)
			{
				num += (float)Math.PI;
			}
			if (num < 0f)
			{
				num += (float)Math.PI * 2f;
			}
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(0f, randVect.Y, 0f);
			randVect.Y = 0f;
			float radius = ((Vector3)(ref randVect)).Length();
			randVect.Y = val.Y;
			bulletCircle.Add(new PLine(Vector3.Zero, Vector3.Transform(Vector3.Zero, model.transforms[hypatiaBones[model.model.Bones["EnemyArmC_Root"]]] * Transformation()) + randVect, 30f));
			bulletCircle.Add(new PCircleHoriz(Vector3.Transform(Vector3.Zero, model.transforms[hypatiaBones[model.model.Bones["EnemyArmC_Root"]]] * Transformation()) + val, radius, 2.1f, _loop: true, num));
			bulletCircle.SetLoop(1);
			ShootTile();
			shootCooldown += maxCooldown;
		}
		if (bossMusic.Count > 1 && bosshp[9] < (int)(21.333334f * (float)(bossMusic.Count - 1)))
		{
			bossMusic.RemoveAt(bossMusic.Count - 1);
		}
		launchCooldown -= (float)gametime.ElapsedGameTime.TotalMilliseconds;
		if (launchCooldown <= 0f)
		{
			bpColl.Launch();
			launchCooldown += maxLaunchCooldown;
		}
		particleCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (particleCooldown <= 0f)
		{
			BaseGame.Get().ps.AddParticles(tempPos, Vector3.Forward * 50f, 0.1f, 180f, Vector3.Zero, 0f, 0.2f, 0.02f, 0f, new Vector4(1f, 1f, 1f, 1f), 250, 8E-05f);
			particleCooldown += particleMax;
		}
	}

	private void FrontFloorStillDraw(GameTime gametime)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(6f) * model.transforms[hypatiaBones[model.model.Bones["EnemyArmC_Root"]]] * Transformation());
		BaseGame.SetAllEPCs(dot.epc, "DiffuseColor", targetColor[7]);
		BaseGame.SetAllEPCs(dot.epc, "EmissiveColor", targetColor[7]);
		BaseGame.Get().DrawModel(ref dot, clearEpc: true);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
	}

	public bool FrontFloorStillRemove(ConditionSet cs)
	{
		if (bosshp[9] <= 0)
		{
			return true;
		}
		return cs.ConditionsMet();
	}

	private EnemyState DieState()
	{
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(DieUpdate, DieDraw, null, EndLevelState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(16.0));
		enemyState.condSet.Start();
		open = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["die"]);
		BaseGame.RunController(anim, open);
		open.IsLooping = false;
		((GameComponent)oluAnim[0]).Update((GameTime)null);
		((GameComponent)oluAnim[1]).Update((GameTime)null);
		BaseGame.Get().level.ActiveZone.music.Add(new MusicPart(bossMusic[0].startBeat, bossMusic[0].cueName));
		bossMusic.Clear();
		shootCooldown = 2f;
		particleCooldown = (particleMax = 0.05f);
		PermDeactivate(0, shots.Length);
		BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, Matrix.CreateScale((2f - shootCooldown) / 2f) * Matrix.CreateTranslation(new Vector3(0f, 0f, -4f)) * Transformation()), Vector3.Up * 20f, 0.3f, 180f, Vector3.Zero, 0f, 1.5f, 0.3f, 0f, new Vector4(1f, 1f, 1f, 1f), 4500, 6.25E-05f);
		return enemyState;
	}

	private void DieUpdate(GameTime gametime)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0759: Unknown result type (might be due to invalid IL or missing references)
		//IL_0786: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_0886: Unknown result type (might be due to invalid IL or missing references)
		//IL_088b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
		if (shootCooldown >= 0f)
		{
			shootCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		else
		{
			((GameComponent)oluAnim[0]).Enabled = true;
			((GameComponent)oluAnim[1]).Enabled = true;
			((GameComponent)escape[0]).Update(gametime);
			((GameComponent)escape[1]).Update(gametime);
			((GameComponent)oluAnim[0]).Update(gametime);
			((GameComponent)oluAnim[1]).Update(gametime);
		}
		NormalUpdate(gametime);
		Matrix val = Matrix.CreateScale((2f - shootCooldown) / 2f) * Matrix.CreateTranslation(new Vector3(0f, 0f, -4f)) * Transformation();
		BaseGame.Get().MovePlayerDir(Vector3.Normalize(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_Face"]]] * val)) * new Vector3(1f, 1f, 1f));
		val = Matrix.CreateScale((2f - shootCooldown) / 2f) * Matrix.CreateTranslation(new Vector3(0f, 0f, -4.2f)) * Transformation();
		mainBezier[0] = new BezierHelper(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_1"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_2"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_3"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_Foot"]]] * val));
		mainBezier[1] = new BezierHelper(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_1"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_2"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_3"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_Foot"]]] * val));
		mainBezier[2] = new BezierHelper(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_1"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_2"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_3"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_Foot"]]] * val));
		mainBezier[3] = new BezierHelper(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_1"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_2"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_3"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_Foot"]]] * val));
		particleCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (particleCooldown <= 0f)
		{
			BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_Foot"]]] * val), Vector3.Up * 5f, 0.2f, 180f, Vector3.Zero, 0f, 0.5f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), 16, 0.003125f);
			BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_Foot"]]] * val), Vector3.Up * 5f, 0.2f, 180f, Vector3.Zero, 0f, 0.5f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), 16, 0.003125f);
			BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_Foot"]]] * val), Vector3.Up * 5f, 0.2f, 180f, Vector3.Zero, 0f, 0.5f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), 16, 0.003125f);
			BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_Foot"]]] * val), Vector3.Up * 5f, 0.2f, 180f, Vector3.Zero, 0f, 0.5f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), 16, 0.003125f);
			particleCooldown += particleMax;
		}
	}

	private void DieDraw(GameTime gametime)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(shootCooldown * 3f) * model.transforms[hypatiaBones[model.model.Bones["EnemyArmC_Root"]]] * Transformation());
		BaseGame.SetAllEPCs(dot.epc, "DiffuseColor", targetColor[7]);
		BaseGame.SetAllEPCs(dot.epc, "EmissiveColor", targetColor[7]);
		BaseGame.Get().DrawModel(ref dot, clearEpc: true);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale((2f - shootCooldown) / 2f) * Matrix.CreateTranslation(new Vector3(0f, 0f, -4f)) * Transformation());
		BaseGame.Get().DrawModel(ref oluBack);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().DrawModel(ref olu);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().SwitchEffectTechnique("Bezier");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1f, -1f, -1f));
		for (int i = 0; i < 4; i++)
		{
			BaseGame.Get().fogEffect.Parameters["BezierPos"].SetValue(mainBezier[i].BezierPos);
			BaseGame.Get().fogEffect.Parameters["BezierVel"].SetValue(mainBezier[i].BezierVel);
			BaseGame.Get().fogEffect.Parameters["Pos0"].SetValue(mainBezier[i].pos[0]);
			BaseGame.Get().fogEffect.Parameters["Pos1"].SetValue(mainBezier[i].pos[1]);
			BaseGame.Get().fogEffect.Parameters["Scale"].SetValue(mainBezier[i].scale);
			BaseGame.Get().fogEffect.Parameters["BezierShift"].SetValue(0f);
			BaseGame.Get().DrawModel(ref tail);
		}
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
	}

	private EnemyState EndLevelState()
	{
		EnemyState enemyState = new EnemyState(EndLevelUpdate, null, null, DieState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		bossMusic.Clear();
		die();
		bpColl.hitPoints = -1;
		return enemyState;
	}

	private void EndLevelUpdate(GameTime gametime)
	{
	}
}
