using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class Pythagoras : Enemy
{
	private static Dictionary<ModelBone, int> pythBones;

	private static Dictionary<ModelBone, int> oluBones;

	public static ModelWrapper model;

	public static ModelWrapper cubeGuide;

	public static ModelWrapper roomModel;

	public static ModelWrapper dot;

	public static ModelWrapper olu;

	public static ModelWrapper oluBack;

	public static ModelWrapper tail;

	public static int[] planeBones;

	public static float size = 15f;

	public ModelOluAnimator anim;

	public ModelOluAnimator cubeAnim;

	public AnimationController spawn;

	public AnimationController cubeSpawn;

	private Vector3 shinePos;

	private float shineRate;

	private float shineDist;

	private float rotAmount;

	private float rotMax;

	private float rotRate;

	public ModelOluAnimator[] oluAnim;

	public AnimationController[] enter;

	public BezierHelper[] mainBezier;

	public BulletPlaneCollection bpColl;

	public List<PythDigit> dColl;

	public List<EnemyState> states;

	public List<MusicPart> bossMusic;

	private Vector3 vel;

	private Vector3 up;

	private Matrix _transformation;

	private bool dirty;

	private bool[] loaded;

	public int[] bosshp;

	private int finalStageHP;

	public Vector3[] targetColor;

	public float shootCooldown;

	public float maxCooldown;

	public int serpentPathIndex;

	public Dictionary<int, string> legBones;

	public int launchIndex;

	public float particleCooldown;

	public float particleMax;

	public Random r;

	public int curMesh;

	public int curIndex;

	public int curPart;

	private Vector3 startCol;

	private Vector3 endCol;

	private float baseHealth;

	private PathList stageOnePath;

	private PathList[] serpentPath;

	private bool followPath;

	private bool drawWireFrame;

	public Pythagoras()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		startCol = new Vector3(0f, 1f, 0f);
		endCol = new Vector3(0.7f, 0f, 0f);
		baseHealth = 64f;
		base._002Ector();
		state = 0;
		states = new List<EnemyState>();
		bossMusic = new List<MusicPart>();
		loaded = new bool[20];
		bosshp = new int[20];
		targetColor = (Vector3[])(object)new Vector3[20];
		planeBones = new int[1];
		bosshp[0] = 144;
		bosshp[1] = 96;
		bosshp[2] = 64;
		bosshp[3] = 64;
		bosshp[4] = 64;
		bosshp[5] = 64;
		bosshp[6] = 64;
		bosshp[7] = 64;
		bosshp[8] = 64;
		bosshp[9] = 64;
		finalStageHP = 512;
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
		drawWireFrame = false;
		dColl = new List<PythDigit>();
	}

	public static void LoadModel()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		model = BaseGame.Get().models.GetModel("Content\\Pythagoras\\Pythagoras", copyData: true, copyEPC: false);
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(model.epc, "DirLight0Direction", Vector3.Normalize(new Vector3(-1f, -0.5f, -0.5f)));
		BaseGame.SetAllEPCs(model.epc, "TextureMix", BaseGame.T_MUL);
		cubeGuide = BaseGame.Get().models.GetModel("Content\\Pythagoras\\PythagorasCubeSpawn", copyData: true, copyEPC: false);
		roomModel = BaseGame.Get().models.GetModel("Content\\Level02\\BossRoom");
		BaseGame.SetAllEPCs(roomModel.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(roomModel.epc, "DirLight0Direction", Vector3.Normalize(new Vector3(-1f, -0.5f, -0.5f)));
		BaseGame.SetAllEPCs(roomModel.epc, "TextureMix", BaseGame.T_MUL);
		BaseGame.SetAllEPCs(roomModel.epc, "DiffuseColor", (object)new Vector3(1f, 1f, 1f));
		BaseGame.SetAllEPCs(roomModel.epc, "Alpha", 1f);
		dot = BaseGame.Get().models.GetModel("Content\\Bird01\\Dot", copyData: false, copyEPC: true);
		BaseGame.SetAllEPCs(dot.epc, "xEnableLighting", false);
		tail = BaseGame.Get().models.GetModel("Content\\Pythagoras\\Tail", copyData: false, copyEPC: false);
		BaseGame.SetAllEPCs(tail.epc, "xEnableLighting", false);
		pythBones = new Dictionary<ModelBone, int>();
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)model.model.Bones).Count; i++)
		{
			if (!pythBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i]))
			{
				pythBones.Add(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i], i);
			}
		}
		if (olu == null)
		{
			LoadOluModel();
		}
	}

	public static void LoadOluModel()
	{
		olu = BaseGame.Get().models.GetModel("Content\\Pythagoras\\OluGlowFace", copyData: false, copyEPC: false);
		BaseGame.SetAllEPCs(olu.epc, "xEnableLighting", false);
		oluBack = BaseGame.Get().models.GetModel("Content\\Pythagoras\\OluEnter", copyData: false, copyEPC: false);
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

	public Pythagoras(Dictionary<string, string> attributes, XmlNode node)
		: this()
	{
	}

	public override Matrix Transformation()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (dirty)
		{
			_transformation = Matrix.CreateRotationY(MathHelper.ToRadians(rotAmount)) * Matrix.CreateScale(new Vector3(size, size, 0f - size)) * Matrix.CreateTranslation(getPos());
			dirty = false;
		}
		return _transformation;
	}

	public override void act(GameTime gametime)
	{
		if (!exists)
		{
			return;
		}
		if (enter[0] != null)
		{
			((GameComponent)enter[0]).Update(gametime);
			((GameComponent)enter[1]).Update(gametime);
		}
		if (spawn != null)
		{
			((GameComponent)spawn).Update(gametime);
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

	public override void draw(GameTime gametime)
	{
		if (!exists)
		{
			return;
		}
		foreach (EnemyState state in states)
		{
			state.Draw(gametime);
		}
	}

	public override void start()
	{
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		new Random();
		base.start();
		anim = new ModelOluAnimator(BaseGame.Get().CoreGame, model, BaseGame.GetFogEffect());
		cubeAnim = new ModelOluAnimator(BaseGame.Get().CoreGame, cubeGuide, BaseGame.GetFogEffect());
		cubeSpawn = new AnimationController(BaseGame.Get().CoreGame, cubeAnim.Animations["ceilstill"]);
		BaseGame.RunController(cubeAnim, cubeSpawn);
		cubeSpawn.IsLooping = false;
		((GameComponent)cubeAnim).Update((GameTime)null);
		model.ResetIndicesToDraw();
		oluAnim = new ModelOluAnimator[2];
		oluAnim[0] = new ModelOluAnimator(BaseGame.Get().CoreGame, olu, BaseGame.GetFogEffect());
		oluAnim[1] = new ModelOluAnimator(BaseGame.Get().CoreGame, oluBack, BaseGame.GetFogEffect());
		enter = new AnimationController[2];
		enter[0] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[0].Animations["rise"]);
		enter[1] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[1].Animations["rise"]);
		BaseGame.RunController(oluAnim[0], enter[0]);
		BaseGame.RunController(oluAnim[1], enter[1]);
		enter[0].IsLooping = false;
		enter[1].IsLooping = false;
		mainBezier = new BezierHelper[4];
		states = new List<EnemyState>();
		states.Add(EnterState());
		addCond(new NeverCondition());
		bpColl = new BulletPlaneCollection(ref model);
		serpentPathIndex = 0;
		serpentPath = new PathList[3];
		List<IPath> list = new List<IPath>();
		serpentPath[0] = new PathList();
		list.Add(new PBezier(getPos(), getPos() + new Vector3(0f, -20f, 0f), 1f, Vector3.Forward, 0f, 0f, 1f, 0, 0f, 0.0, 0.0));
		list.Add(new PBezier(getPos() + new Vector3(0f, -20f, 0f), getPos() + new Vector3(0f, -40f, 0f), getPos() + new Vector3(20f, -40f, 0f), getPos() + new Vector3(35f, 15f, 0f), 0.2f, Vector3.Forward, 0.1f, 0.1f, 3f, 0, 0f, 0.0, 90.0));
		list.Add(new PBezier(getPos() + new Vector3(35f, 15f, 0f), BaseGame.Get().playerPos, 0.05f, Vector3.Forward, 0.1f, 0.1f, 8f, 0, 0f, 0.0, 90.0));
		serpentPath[0].addPathComboList(list, new PLine(Vector3.Zero, Vector3.Forward, 0f));
		list.Clear();
		serpentPath[1] = new PathList();
		list.Add(new PBezier(getPos(), getPos() + new Vector3(0f, -20f, 0f), 1f, Vector3.Forward, 0f, 0f, 1f, 0, 0f, 0.0, 0.0));
		list.Add(new PBezier(getPos() + new Vector3(0f, -20f, 0f), getPos() + new Vector3(0f, -40f, 0f), getPos() + new Vector3(-20f, -40f, 0f), getPos() + new Vector3(-35f, 15f, 0f), 0.2f, Vector3.Forward, 0.1f, 0.1f, 3f, 0, 0f, 0.0, 90.0));
		list.Add(new PBezier(getPos() + new Vector3(-35f, 15f, 0f), BaseGame.Get().playerPos, 0.05f, Vector3.Forward, 0.1f, 0.1f, 8f, 0, 0f, 0.0, 90.0));
		serpentPath[1].addPathComboList(list, new PLine(Vector3.Zero, Vector3.Forward, 0f));
		list.Clear();
		serpentPath[2] = new PathList();
		list.Add(new PBezier(getPos(), getPos() + new Vector3(0f, -20f, 0f), 1f, Vector3.Forward, 0f, 0f, 1f, 0, 0f, 0.0, 0.0));
		list.Add(new PBezier(getPos() + new Vector3(0f, -20f, 0f), getPos() + new Vector3(0f, 30f, 0f), 0.14f, Vector3.Forward, 1.6f, 1.6f, 3f, 0, 0f, 0.0, 90.0));
		list.Add(new PBezier(getPos() + new Vector3(0f, 30f, 0f), BaseGame.Get().playerPos, 0.05f, Vector3.Forward, 0.1f, 0.1f, 8f, 0, 0f, 0.0, 90.0));
		serpentPath[2].addPathComboList(list, new PLine(Vector3.Zero, Vector3.Forward, 0f));
		legBones = new Dictionary<int, string>();
		legBones.Add(0, "Armature_LegAFar");
		legBones.Add(1, "Armature_LegBFar");
		legBones.Add(2, "Armature_LegCFar");
		legBones.Add(3, "Armature_LegDFar");
	}

	public void AddCue(int beat, string name, int playMeas, int loopMeas)
	{
		bossMusic.Add(new MusicPart(beat, name, playMeas, loopMeas));
	}

	public override Vector3 getPos()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(0f, -30f, 60f);
	}

	public override string name()
	{
		return "[pyt{H}agoras]";
	}

	public override bool Check(int numEnem)
	{
		return true;
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		BoneModelTarget boneModelTarget = (BoneModelTarget)toHit.eTarget;
		bosshp[boneModelTarget.id]--;
		if (toHit.fillMode != boneModelTarget.fillMode)
		{
			bosshp[boneModelTarget.id]--;
		}
		ref Vector3 reference = ref targetColor[boneModelTarget.id];
		reference = Vector3.Lerp(endCol, startCol, (float)bosshp[boneModelTarget.id] / baseHealth);
		if (boneModelTarget.id >= 2 && boneModelTarget.id <= 9)
		{
			finalStageHP--;
			if (toHit.fillMode != boneModelTarget.fillMode)
			{
				finalStageHP--;
			}
		}
		if (bosshp[boneModelTarget.id] <= 0)
		{
			switch (boneModelTarget.id)
			{
			case 2:
				RemoveBone("Armature_LegAFar");
				legBones.Remove(0);
				legBones.Add(0, "Armature_LegAClose");
				addTarget(new Vector3(0f, 1.5f, 0f), bosshp[3], 5, ref model, 3, "Armature_LegAClose");
				break;
			case 3:
				RemoveBone("Armature_LegAClose");
				legBones.Remove(0);
				anim.BonePoses[pythBones[model.model.Bones["Armature_LegAClose"]]].enabled = false;
				break;
			case 4:
				RemoveBone("Armature_LegBFar");
				legBones.Remove(1);
				legBones.Add(1, "Armature_LegBClose");
				addTarget(new Vector3(0f, 1.5f, 0f), bosshp[5], 5, ref model, 5, "Armature_LegBClose");
				break;
			case 5:
				RemoveBone("Armature_LegBClose");
				legBones.Remove(1);
				anim.BonePoses[pythBones[model.model.Bones["Armature_LegBClose"]]].enabled = false;
				break;
			case 6:
				RemoveBone("Armature_LegCFar");
				legBones.Remove(2);
				legBones.Add(2, "Armature_LegCClose");
				addTarget(new Vector3(0f, 1.5f, 0f), bosshp[7], 5, ref model, 7, "Armature_LegCClose");
				break;
			case 7:
				RemoveBone("Armature_LegCClose");
				legBones.Remove(2);
				anim.BonePoses[pythBones[model.model.Bones["Armature_LegCClose"]]].enabled = false;
				break;
			case 8:
				RemoveBone("Armature_LegDFar");
				legBones.Remove(3);
				legBones.Add(3, "Armature_LegDClose");
				addTarget(new Vector3(0f, 1.5f, 0f), bosshp[9], 5, ref model, 9, "Armature_LegDClose");
				break;
			case 9:
				RemoveBone("Armature_LegDClose");
				legBones.Remove(3);
				anim.BonePoses[pythBones[model.model.Bones["Armature_LegDClose"]]].enabled = false;
				break;
			}
		}
		if (!toHit.skipSquare)
		{
			BaseGame.Get().ps.AddParticles(toHit.eTarget.absolutePos(), Vector3.Forward * 12f, 2f, 180f, Vector3.Zero, 0f, 0.35f, 0.25f, 0.2f, new Vector4(targetColor[boneModelTarget.id] + new Vector3(0.2f, 0.2f, 0.2f), 1f), 320, 6.25E-05f);
		}
	}

	public override void HitSound(int lockNum, float volume)
	{
	}

	public void NormalUpdate(GameTime gametime)
	{
		if (((GameComponent)anim).Enabled)
		{
			((GameComponent)anim).Update(gametime);
		}
		if (((GameComponent)oluAnim[0]).Enabled)
		{
			((GameComponent)oluAnim[0]).Update(gametime);
		}
		if (((GameComponent)oluAnim[1]).Enabled)
		{
			((GameComponent)oluAnim[1]).Update(gametime);
		}
		if (!BaseGame.Get().FREEZE_ON)
		{
			PlayMusic(gametime);
		}
		particleCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (particleCooldown < 0f)
		{
			particleCooldown += particleMax;
		}
		rotAmount += rotRate * (float)gametime.ElapsedGameTime.TotalSeconds;
	}

	public void NormalDraw(GameTime gametime)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		if (drawWireFrame)
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		}
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.Get().DrawModel(ref model);
		BaseGame.Get().matStack.PopMatrix();
		if (drawWireFrame)
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		}
		bpColl.draw(gametime);
	}

	public void IntroUpdate(GameTime gametime)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0853: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_0859: Unknown result type (might be due to invalid IL or missing references)
		//IL_0864: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_087d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_090a: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Matrix.CreateScale(0.75f) * Matrix.CreateTranslation(new Vector3(0f, 0f, -0.2f)) * Transformation();
		NormalUpdate(gametime);
		mainBezier[0] = new BezierHelper(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_1"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_2"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_3"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_Foot"]]] * val));
		mainBezier[1] = new BezierHelper(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_1"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_2"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_3"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_Foot"]]] * val));
		mainBezier[2] = new BezierHelper(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_1"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_2"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_3"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_Foot"]]] * val));
		mainBezier[3] = new BezierHelper(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_1"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_2"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_3"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_Foot"]]] * val));
		particleCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (particleCooldown <= 0f)
		{
			if (Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_Foot"]]] * val).Y >= getPos().Y)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_Foot"]]] * val), Vector3.Up * 5f, 0.2f, 180f, Vector3.Zero, 0f, 0.5f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), 16, 0.003125f);
			}
			if (Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_Foot"]]] * val).Y >= getPos().Y)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_Foot"]]] * val), Vector3.Up * 5f, 0.2f, 180f, Vector3.Zero, 0f, 0.5f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), 16, 0.003125f);
			}
			if (Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_Foot"]]] * val).Y >= getPos().Y)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_Foot"]]] * val), Vector3.Up * 5f, 0.2f, 180f, Vector3.Zero, 0f, 0.5f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), 16, 0.003125f);
			}
			if (Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_Foot"]]] * val).Y >= getPos().Y)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_Foot"]]] * val), Vector3.Up * 5f, 0.2f, 180f, Vector3.Zero, 0f, 0.5f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), 16, 0.003125f);
			}
			particleCooldown += particleMax;
		}
	}

	public void IntroDraw(GameTime gametime)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Water");
		BaseGame.Get().fogEffect.Parameters["WaterHeight"].SetValue(-30f);
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.Get().DrawModel(ref model);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().SwitchEffectTechnique("WaterBezier");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1f, -1f, -1f));
		for (int i = 0; i < 4; i++)
		{
			BaseGame.Get().fogEffect.Parameters["BezierPos"].SetValue(mainBezier[i].BezierPos);
			BaseGame.Get().fogEffect.Parameters["BezierVel"].SetValue(mainBezier[i].BezierVel);
			BaseGame.Get().fogEffect.Parameters["Pos0"].SetValue(mainBezier[i].pos[0]);
			BaseGame.Get().fogEffect.Parameters["Pos1"].SetValue(mainBezier[i].pos[1]);
			BaseGame.Get().fogEffect.Parameters["Scale"].SetValue(mainBezier[i].scale);
			BaseGame.Get().DrawModel(ref tail);
		}
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().SwitchEffectTechnique("Water");
		BaseGame.Get().fogEffect.Parameters["WaterHeight"].SetValue(-30f);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(0.75f) * Transformation());
		BaseGame.Get().DrawModel(ref oluBack);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().DrawModel(ref olu);
		BaseGame.Get().matStack.PopMatrix();
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

	public bool AlwaysRemove(ConditionSet cs)
	{
		return true;
	}

	private EnemyState EnterState()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(IntroUpdate, IntroDraw, null, PullupState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.0));
		enemyState.condSet.Start();
		((GameComponent)oluAnim[0]).Enabled = true;
		((GameComponent)oluAnim[1]).Enabled = true;
		((GameComponent)oluAnim[0]).Update((GameTime)null);
		((GameComponent)oluAnim[1]).Update((GameTime)null);
		BaseGame.Get().skyFlowToggle = false;
		BaseGame.Get().ps.AddParticles(getPos(), Vector3.Forward * 60f, 0.2f, 180f, Vector3.Zero, 0f, 0.8f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), 3500, 8E-05f);
		return enemyState;
	}

	private EnemyState PullupState()
	{
		EnemyState enemyState = new EnemyState(IntroUpdate, IntroDraw, null, RaiseArmsState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.0));
		enemyState.condSet.Start();
		enter[0] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[0].Animations["lowerarms"]);
		enter[1] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[1].Animations["lowerarms"]);
		BaseGame.RunController(oluAnim[0], enter[0]);
		BaseGame.RunController(oluAnim[1], enter[1]);
		enter[0].IsLooping = false;
		enter[1].IsLooping = false;
		((GameComponent)oluAnim[0]).Update((GameTime)null);
		((GameComponent)oluAnim[1]).Update((GameTime)null);
		return enemyState;
	}

	private EnemyState RaiseArmsState()
	{
		EnemyState enemyState = new EnemyState(IntroUpdate, IntroDraw, null, BossForm1State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.0));
		enemyState.condSet.Start();
		enter[0] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[0].Animations["raisearms"]);
		enter[1] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[1].Animations["raisearms"]);
		BaseGame.RunController(oluAnim[0], enter[0]);
		BaseGame.RunController(oluAnim[1], enter[1]);
		enter[0].IsLooping = false;
		enter[1].IsLooping = false;
		((GameComponent)oluAnim[0]).Update((GameTime)null);
		((GameComponent)oluAnim[1]).Update((GameTime)null);
		spawn = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["rise"]);
		BaseGame.RunController(anim, spawn);
		spawn.IsLooping = false;
		((GameComponent)spawn).Update(BaseGame.Get().emptytime);
		((GameComponent)spawn).Enabled = true;
		((GameComponent)anim).Enabled = true;
		return enemyState;
	}

	private EnemyState BossForm1State()
	{
		EnemyState enemyState = new EnemyState(IntroUpdate, IntroDraw, null, BossForm2State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.5));
		enemyState.condSet.Start();
		enter[0] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[0].Animations["leave"]);
		enter[1] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[1].Animations["leave"]);
		BaseGame.RunController(oluAnim[0], enter[0]);
		BaseGame.RunController(oluAnim[1], enter[1]);
		enter[0].IsLooping = false;
		enter[1].IsLooping = false;
		((GameComponent)oluAnim[0]).Update((GameTime)null);
		((GameComponent)oluAnim[1]).Update((GameTime)null);
		spawn = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["attach"]);
		BaseGame.RunController(anim, spawn);
		spawn.IsLooping = false;
		((GameComponent)spawn).Update(BaseGame.Get().emptytime);
		return enemyState;
	}

	private EnemyState BossForm2State()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, NormalDraw, null, BossForm3State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.0));
		enemyState.condSet.Start();
		((GameComponent)enter[0]).Enabled = false;
		((GameComponent)enter[1]).Enabled = false;
		((GameComponent)oluAnim[0]).Enabled = false;
		((GameComponent)oluAnim[1]).Enabled = false;
		spawn = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["pullup"]);
		BaseGame.RunController(anim, spawn);
		spawn.IsLooping = false;
		((GameComponent)spawn).Update(BaseGame.Get().emptytime);
		return enemyState;
	}

	private EnemyState BossForm3State()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, NormalDraw, null, BossForm4State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(1.2999999523162842));
		enemyState.condSet.Start();
		spawn = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["ceilattach"]);
		BaseGame.RunController(anim, spawn);
		spawn.IsLooping = false;
		((GameComponent)spawn).Update(BaseGame.Get().emptytime);
		return enemyState;
	}

	private EnemyState BossForm4State()
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(RoomSpawnUpdate, RoomSpawnDraw, null, BossForm5State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(4.5));
		enemyState.condSet.Start();
		spawn = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["ceilstill"]);
		BaseGame.RunController(anim, spawn);
		spawn.IsLooping = false;
		((GameComponent)spawn).Update(BaseGame.Get().emptytime);
		((GameComponent)spawn).Enabled = false;
		((GameComponent)anim).Enabled = false;
		shinePos = new Vector3(0f, 0f, -0.6f);
		shineRate = 0.4f;
		shineDist = 0.05f;
		BaseGame.SetAllEPCs(roomModel.epc, "ShinePos", shinePos);
		return enemyState;
	}

	public void RoomSpawnUpdate(GameTime gameTime)
	{
		shineDist += shineRate * (float)gameTime.ElapsedGameTime.TotalSeconds;
		NormalUpdate(gameTime);
	}

	public void RoomSpawnDraw(GameTime gameTime)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		NormalDraw(gameTime);
		BaseGame.Get().SwitchEffectTechnique("ShineClamp");
		BaseGame.Get().fogEffect.Parameters["xFogStart"].SetValue(1000f);
		BaseGame.Get().fogEffect.Parameters["xFogEnd"].SetValue(2000f);
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(8f, 8f, 8f) * Transformation());
		BaseGame.SetAllEPCs(roomModel.epc, "ShineDist", shineDist);
		BaseGame.Get().DrawModel(ref roomModel);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().fogEffect.Parameters["xFogStart"].SetValue(BaseGame.FOG_START);
		BaseGame.Get().fogEffect.Parameters["xFogEnd"].SetValue(BaseGame.FOG_END);
	}

	private EnemyState BossForm5State()
	{
		EnemyState enemyState = new EnemyState(RoomSpinUpdate, RoomSpinDraw, null, Attack1Stage1State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(5.0));
		enemyState.condSet.Start();
		rotAmount = 0f;
		rotMax = 45f;
		rotRate = 0f;
		return enemyState;
	}

	public void RoomSpinUpdate(GameTime gameTime)
	{
		rotRate += rotMax / 4f * (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (rotRate > rotMax)
		{
			rotRate = rotMax;
		}
		NormalUpdate(gameTime);
	}

	public void RoomSpinDraw(GameTime gameTime)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		NormalDraw(gameTime);
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().fogEffect.Parameters["xFogStart"].SetValue(1000f);
		BaseGame.Get().fogEffect.Parameters["xFogEnd"].SetValue(2000f);
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(8f, 8f, 8f) * Transformation());
		BaseGame.Get().DrawModel(ref roomModel);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().fogEffect.Parameters["xFogStart"].SetValue(BaseGame.FOG_START);
		BaseGame.Get().fogEffect.Parameters["xFogEnd"].SetValue(BaseGame.FOG_END);
	}

	private EnemyState Attack1Stage1State()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(Attack1Stage1Update, RoomSpinDraw, Attack1Stage1Remove, Attack1Stage2State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		stageOnePath = new PathList();
		stageOnePath.Add(new PLine(Vector3.Zero, Vector3.Zero, 0f));
		planeBones = new int[1];
		planeBones[0] = model.boneNames["Armature_StageOneTop"][1];
		curIndex = 0;
		curMesh = 0;
		curPart = 0;
		followPath = false;
		maxCooldown = 6f;
		shootCooldown = 2f;
		while (curMesh >= 0)
		{
			ShootTile();
		}
		return enemyState;
	}

	public void Attack1Stage1Update(GameTime gameTime)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		NormalUpdate(gameTime);
		shootCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (shootCooldown < 0f)
		{
			Enemy enemy = new Serpent(16, 0.2f, 0, 0f, Vector3.Forward, serpentPath[serpentPathIndex]);
			serpentPathIndex++;
			serpentPathIndex %= serpentPath.Length;
			enemy.start();
			BaseGame.Get().enems.Add(enemy);
			shootCooldown += maxCooldown;
		}
	}

	public bool Attack1Stage1Remove(ConditionSet cs)
	{
		if (bosshp[0] <= 0)
		{
			return true;
		}
		return cs.ConditionsMet();
	}

	private EnemyState Attack1Stage2State()
	{
		EnemyState enemyState = new EnemyState(Attack1Stage2Update, RoomSpinDraw, Attack1Stage2Remove, Move1Stage3State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		curIndex = 0;
		curMesh = 0;
		curPart = 0;
		followPath = false;
		maxCooldown = 2f;
		shootCooldown = 1f;
		drawWireFrame = true;
		for (int num = BaseGame.Get().enems.Count - 1; num >= 0; num--)
		{
			if (BaseGame.Get().enems[num] is Serpent || BaseGame.Get().enems[num] is SerpentTail)
			{
				BaseGame.Get().enems[num].leave();
			}
		}
		SetupDigits();
		return enemyState;
	}

	public void Attack1Stage2Update(GameTime gameTime)
	{
		NormalUpdate(gameTime);
		shootCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (shootCooldown < 0f && dColl.Count > 0)
		{
			ShootDigit();
			shootCooldown += maxCooldown;
		}
	}

	public bool Attack1Stage2Remove(ConditionSet cs)
	{
		if (bosshp[1] <= 0)
		{
			return true;
		}
		return cs.ConditionsMet();
	}

	private EnemyState Move1Stage3State()
	{
		EnemyState enemyState = new EnemyState(RoomSpawnUpdate, RoomSpawnDraw, null, Move2Stage3State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.0));
		enemyState.condSet.Start();
		anim.bonePoses[pythBones[model.model.Bones["Armature_StageTwo"]]].enabled = false;
		spawn = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["ceilfall"]);
		BaseGame.RunController(anim, spawn);
		spawn.IsLooping = false;
		((GameComponent)spawn).Update(BaseGame.Get().emptytime);
		((GameComponent)spawn).Enabled = true;
		((GameComponent)anim).Enabled = true;
		drawWireFrame = false;
		return enemyState;
	}

	private EnemyState Move2Stage3State()
	{
		EnemyState enemyState = new EnemyState(RoomSpinUpdate, RoomSpinDraw, null, Attack1Stage3State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(1.399999976158142));
		enemyState.condSet.Start();
		spawn = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["flooropen"]);
		BaseGame.RunController(anim, spawn);
		spawn.IsLooping = false;
		((GameComponent)spawn).Update(BaseGame.Get().emptytime);
		drawWireFrame = false;
		return enemyState;
	}

	private EnemyState Attack1Stage3State()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(Attack1Stage3Update, RoomSpinShrinkDraw, Attack1Stage3Remove, DieState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		addTarget(new Vector3(0f, 1.7f, 0f), bosshp[2], 5, ref model, 2, "Armature_LegAFar");
		addTarget(new Vector3(0f, 1.7f, 0f), bosshp[4], 5, ref model, 4, "Armature_LegBFar");
		addTarget(new Vector3(0f, 1.7f, 0f), bosshp[6], 5, ref model, 6, "Armature_LegCFar");
		addTarget(new Vector3(0f, 1.7f, 0f), bosshp[8], 5, ref model, 8, "Armature_LegDFar");
		spawn = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["floorstill"]);
		BaseGame.RunController(anim, spawn);
		spawn.IsLooping = false;
		((GameComponent)spawn).Update(BaseGame.Get().emptytime);
		shinePos = new Vector3(0f, 0f, 0.6f);
		shineDist = 2.2f;
		shineRate = 0.1f;
		BaseGame.SetAllEPCs(roomModel.epc, "ShinePos", shinePos);
		maxCooldown = 6f;
		shootCooldown = 1f;
		launchIndex = 0;
		return enemyState;
	}

	public void Attack1Stage3Update(GameTime gameTime)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		NormalUpdate(gameTime);
		shootCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (!(shootCooldown < 0f) || legBones.Count <= 0)
		{
			return;
		}
		PathList pathList = new PathList();
		List<IPath> list = new List<IPath>();
		launchIndex++;
		launchIndex %= 4;
		shootCooldown += maxCooldown;
		if (legBones.ContainsKey(launchIndex))
		{
			list.Add(new PBezier(Vector3.Transform(new Vector3(0f, 1.5f, 0f), model.transforms[pythBones[model.model.Bones[legBones[launchIndex]]]] * Transformation()), Vector3.Transform(new Vector3(0f, 4f, 0f), model.transforms[pythBones[model.model.Bones[legBones[launchIndex]]]] * Transformation()), 1f, Vector3.Forward, 0f, 0f, 0f, 0, 0f, 0.0, 0.0));
			list.Add(new PBezier(Vector3.Transform(new Vector3(0f, 4f, 0f), model.transforms[pythBones[model.model.Bones[legBones[launchIndex]]]] * Transformation()), BaseGame.Get().playerPos, 0.1f, Vector3.Forward, 0.2f, 0.2f, 3f, 0, 0f, 0.0, 90.0));
			if ((((PBezier)list[0]).curEndLocation() - ((PBezier)list[0]).curLocation()).Z > 0f)
			{
				pathList.addPathComboList(list, new PLine(Vector3.Zero, Vector3.Forward, 0f));
				Enemy enemy = new Serpent(18, 0.2f, 0, 0f, Vector3.Forward, pathList);
				launchIndex++;
				launchIndex %= 4;
				enemy.start();
				BaseGame.Get().enems.Add(enemy);
			}
			else
			{
				shootCooldown -= maxCooldown;
			}
		}
	}

	public void RoomSpinShrinkDraw(GameTime gameTime)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		NormalDraw(gameTime);
		BaseGame.Get().SwitchEffectTechnique("ShineClamp");
		BaseGame.Get().fogEffect.Parameters["xFogStart"].SetValue(1000f);
		BaseGame.Get().fogEffect.Parameters["xFogEnd"].SetValue(2000f);
		BaseGame.SetAllEPCs(roomModel.epc, "ShineDist", shineRate + shineDist * ((float)finalStageHP / 512f));
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(8f, 8f, 8f) * Transformation());
		BaseGame.Get().DrawModel(ref roomModel);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().fogEffect.Parameters["xFogStart"].SetValue(BaseGame.FOG_START);
		BaseGame.Get().fogEffect.Parameters["xFogEnd"].SetValue(BaseGame.FOG_END);
		BaseGame.Get().SwitchEffectTechnique("Textured");
	}

	public bool Attack1Stage3Remove(ConditionSet cs)
	{
		if (bosshp[2] <= 0 && bosshp[3] <= 0 && bosshp[4] <= 0 && bosshp[5] <= 0 && bosshp[6] <= 0 && bosshp[7] <= 0 && bosshp[8] <= 0 && bosshp[9] <= 0)
		{
			return true;
		}
		return cs.ConditionsMet();
	}

	public EnemyState DieState()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(null, DieDraw, null, EndLevelState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(4.0));
		enemyState.condSet.Start();
		bossMusic.Clear();
		AddCue(0, "Kick01", 0, 1);
		shootCooldown = 2f;
		BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, Matrix.CreateTranslation(new Vector3(0f, 0f, -4f)) * Transformation()), Vector3.Up * 20f, 0.3f, 180f, Vector3.Zero, 0f, 1.5f, 0.3f, 0f, new Vector4(1f, 1f, 1f, 1f), 4500, 6.25E-05f);
		drawWireFrame = true;
		return enemyState;
	}

	public void DieDraw(GameTime gameTime)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		NormalDraw(gameTime);
		BaseGame.Get().SwitchEffectTechnique("ShineClamp");
		BaseGame.Get().fogEffect.Parameters["xFogStart"].SetValue(1000f);
		BaseGame.Get().fogEffect.Parameters["xFogEnd"].SetValue(2000f);
		BaseGame.SetAllEPCs(roomModel.epc, "ShineDist", shineRate + shineDist * ((float)finalStageHP / 512f));
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(8f, 8f, 8f) * Transformation());
		BaseGame.Get().DrawModel(ref roomModel);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().fogEffect.Parameters["xFogStart"].SetValue(BaseGame.FOG_START);
		BaseGame.Get().fogEffect.Parameters["xFogEnd"].SetValue(BaseGame.FOG_END);
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
	}

	private EnemyState EndLevelState()
	{
		EnemyState enemyState = new EnemyState(EndLevelUpdate, null, null, DieState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		for (int num = BaseGame.Get().enems.Count - 1; num >= 0; num--)
		{
			if (BaseGame.Get().enems[num] != this)
			{
				BaseGame.Get().enems[num].die();
			}
		}
		bossMusic.Clear();
		die();
		return enemyState;
	}

	private void EndLevelUpdate(GameTime gametime)
	{
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
				bpColl.AddAttachedPlane(this, ref model, curMesh, curIndex, num, new Vector3(1f, 0.2f, 0.2f), new Vector3(0.2f, 0f, 0f), stageOnePath, curPart, 0f, followPath);
				SplitList(curMesh, curIndex);
			}
			curIndex += 3;
		}
		if (!flag)
		{
			curMesh = -1;
		}
	}

	private void RemoveBone(string bonename)
	{
		int num = model.boneNames[bonename][1];
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)model.model.Meshes).Count; i++)
		{
			for (int j = 0; j < model.indices[i].Length - 1; j += 3)
			{
				for (int k = 0; k < 3; k++)
				{
					if (num == model.vertices[i][model.indices[i][j + k]].boneNum(0))
					{
						SplitList(i, j);
						k = 3;
					}
				}
			}
		}
	}

	private void SetupDigits()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<Vector3, int> dictionary = new Dictionary<Vector3, int>();
		bool flag = false;
		while (curIndex < cubeGuide.vertices[curMesh].Length && !flag)
		{
			if (!dictionary.ContainsKey(cubeGuide.vertices[curMesh][curIndex].position))
			{
				PythDigit pythDigit = new PythDigit(0.9f);
				int boneIndex = cubeGuide.vertices[curMesh][curIndex].boneNum(0);
				dictionary.Add(cubeGuide.vertices[curMesh][curIndex].position, curIndex);
				pythDigit.AttachToPyth(this, ref cubeGuide, curMesh, curIndex, boneIndex, 1, rotRate);
				pythDigit.start();
				BaseGame.Get().enems.Add(pythDigit);
				dColl.Add(pythDigit);
			}
			curIndex++;
		}
	}

	private void ShootDigit()
	{
		int index = r.Next(dColl.Count);
		if (dColl[index].state == 0)
		{
			dColl[index].Launch();
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
}
