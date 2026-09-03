using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class Olu : Enemy
{
	private static Dictionary<ModelBone, int> oluBones;

	private static Dictionary<ModelBone, int> legBones;

	private static Dictionary<ModelBone, int> kokoBones;

	public static ModelWrapper olu;

	public static ModelWrapper oluBack;

	public static ModelWrapper oluLegs;

	public static ModelWrapper tail;

	public static ModelWrapper s_oluPlane;

	public static ModelWrapper s_koko;

	public ModelWrapper oluPlane;

	public ModelWrapper koko;

	public BulletPlaneCollection bpColl;

	public static PlaneEffect pE;

	public static Random r;

	public ModelOluAnimator[] oluAnim;

	public AnimationController[] oluAC;

	public AnimationController[] oluAC2;

	public BezierHelper[] mainBezier;

	public List<BezierHelper>[] spiralBezier;

	public List<Vector3[]>[] spiralPoints;

	public Vector3[][] spiralBasePoints;

	public ModelOluAnimator legAnim;

	public AnimationController legAC;

	public BezierHelper[] legBezier;

	public ModelOluAnimator kokoAnim;

	public AnimationController kokoAC;

	public BezierHelper[] kokoBez;

	public BezierHelper[] kokoPlayerBez;

	public PlaneDetachColl pdColl;

	public List<int>[] playerIndicesToDraw;

	public static float targetTotal;

	public float curTargetCount;

	public List<EnemyState> states;

	public List<MusicPart> bossMusic;

	public List<MusicPart> tentBossMusic;

	public MusicPart basePart;

	private Vector3 vel;

	private Vector3 up;

	private Matrix _transformation;

	private bool dirty;

	private bool[] loaded;

	public int[] bosshp;

	public float particleCooldown;

	public float particleMax;

	public float phaseCountdown;

	public float phaseMax;

	public float phaseCountdown2;

	public float phaseMax2;

	public int curMesh;

	public int curIndex;

	public int curPart;

	private Vector3 startCol;

	private Vector3 endCol;

	private int curAnim;

	private List<int> activeLegs;

	private int curLeg;

	private float waterLevel;

	public RippleEffect rE;

	public Vector3 ripplePos;

	private bool splashActive;

	private bool drawLegParts;

	public List<RippleEffect> reL;

	private float linePos;

	private float size;

	private float offset;

	private float legProgress;

	private float alphaAmount;

	private float legAlphaAmount;

	private float planeAlphaAmount;

	private bool drawFace;

	private bool drawLegs;

	private bool resetMusic;

	private float finalTurnAmount;

	public static Dictionary<int, WaitCond> wCond;

	private string[] message;

	private int drawMode;

	protected float GetHairProgress(int hairNum)
	{
		float num = 0.25f * (float)hairNum;
		float result = 0f;
		if (curTargetCount / targetTotal > num + 0.2499f)
		{
			result = 1f;
		}
		else if (curTargetCount / targetTotal > num)
		{
			result = (curTargetCount / targetTotal - num) * 4f;
		}
		return result;
	}

	public Olu()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		startCol = new Vector3(0f, 1f, 0f);
		endCol = new Vector3(0.7f, 0f, 0f);
		waterLevel = 100f;
		drawLegParts = true;
		linePos = -10f;
		size = 10f;
		offset = 4f;
		alphaAmount = 1f;
		legAlphaAmount = 1f;
		base._002Ector();
		state = 0;
		states = new List<EnemyState>();
		bossMusic = new List<MusicPart>();
		tentBossMusic = new List<MusicPart>();
		loaded = new bool[20];
		bosshp = new int[20];
		if (BaseGame.release)
		{
			bosshp[0] = 36;
			bosshp[1] = 115;
			bosshp[2] = 118;
			bosshp[3] = 56;
			bosshp[4] = 118;
		}
		else
		{
			bosshp[0] = 8;
			bosshp[1] = 10;
			bosshp[2] = 1;
			bosshp[3] = 16;
			bosshp[4] = 1;
		}
		curMesh = (curIndex = 0);
		for (int i = 0; i < 20; i++)
		{
			loaded[i] = false;
		}
		attackCooldown = 5f;
		particleMax = 0.02f;
		hitPoints = 500;
		vel = Vector3.Backward;
		up = Vector3.Up;
		dirty = true;
		activeLegs = new List<int>();
		activeLegs.Add(0);
		activeLegs.Add(1);
		activeLegs.Add(2);
		activeLegs.Add(3);
		curLeg = r.Next(4);
		spiralBezier = new List<BezierHelper>[4];
		spiralBezier[0] = new List<BezierHelper>();
		spiralBezier[1] = new List<BezierHelper>();
		spiralBezier[2] = new List<BezierHelper>();
		spiralBezier[3] = new List<BezierHelper>();
		spiralPoints = new List<Vector3[]>[4];
		spiralPoints[0] = new List<Vector3[]>();
		spiralPoints[1] = new List<Vector3[]>();
		spiralPoints[2] = new List<Vector3[]>();
		spiralPoints[3] = new List<Vector3[]>();
		spiralBasePoints = new Vector3[4][];
		spiralBasePoints[0] = (Vector3[])(object)new Vector3[4];
		spiralBasePoints[1] = (Vector3[])(object)new Vector3[4];
		spiralBasePoints[2] = (Vector3[])(object)new Vector3[4];
		spiralBasePoints[3] = (Vector3[])(object)new Vector3[4];
		message = new string[4];
		message[3] = "Life exists everywhere, even when not your own.";
		message[2] = "With that life exists a pulse that permeates all.";
		message[1] = "The pulse cannot exist without life, and life cannot exist without the pulse.";
		message[0] = "Carry on that which I guarded, this digital pulse.";
		drawFace = true;
		drawLegs = true;
		if (wCond == null)
		{
			wCond = new Dictionary<int, WaitCond>();
			wCond.Add(0, new WaitCond("OluDrums01", Beats.Eighth));
			wCond.Add(1, new WaitCond("OluDrums01", Beats.Eighth));
			wCond.Add(2, new WaitCond("OluDrums02", Beats.Eighth));
			wCond.Add(3, new WaitCond("OluDrums03", Beats.Eighth));
			wCond.Add(4, new WaitCond("OluDrums04", Beats.Eighth));
			wCond.Add(5, new WaitCond("OluDrums05", Beats.Eighth));
			wCond.Add(6, new WaitCond("OluDrums06", Beats.Eighth));
			wCond.Add(7, new WaitCond("OluDrums07", Beats.Eighth));
			wCond.Add(8, new WaitCond("OluDrums08", Beats.Eighth));
		}
		_eCond = wCond;
	}

	public static void LoadOluModel()
	{
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		olu = BaseGame.Get().models.GetModel("Content\\FinalLevel\\Olu", copyData: false, copyEPC: false);
		BaseGame.SetAllEPCs(olu.epc, "xEnableLighting", false);
		oluBack = BaseGame.Get().models.GetModel("Content\\FinalLevel\\OluBack", copyData: false, copyEPC: false);
		BaseGame.SetAllEPCs(oluBack.epc, "xEnableLighting", false);
		oluBones = new Dictionary<ModelBone, int>();
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)olu.model.Bones).Count; i++)
		{
			if (!oluBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)olu.model.Bones)[i]))
			{
				oluBones.Add(((ReadOnlyCollection<ModelBone>)(object)olu.model.Bones)[i], i);
			}
		}
		oluLegs = BaseGame.Get().models.GetModel("Content\\FinalLevel\\OluLegs", copyData: false, copyEPC: false);
		BaseGame.SetAllEPCs(oluLegs.epc, "xEnableLighting", false);
		legBones = new Dictionary<ModelBone, int>();
		for (int j = 0; j < ((ReadOnlyCollection<ModelBone>)(object)oluLegs.model.Bones).Count; j++)
		{
			if (!legBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)oluLegs.model.Bones)[j]))
			{
				legBones.Add(((ReadOnlyCollection<ModelBone>)(object)oluLegs.model.Bones)[j], j);
			}
		}
		tail = BaseGame.Get().models.GetModel("Content\\FinalLevel\\Tail", copyData: false, copyEPC: false);
		BaseGame.SetAllEPCs(tail.epc, "xEnableLighting", false);
		s_oluPlane = BaseGame.Get().models.GetModel("Content\\FinalLevel\\OluPlane", copyData: true, copyEPC: false);
		BaseGame.SetAllEPCs(s_oluPlane.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(s_oluPlane.epc, "DirLight0Direction", (object)new Vector3(-0.5f, -0.5f, -1f));
		s_koko = BaseGame.Get().models.GetModel("Content\\Kokopelli\\Kokopelli", copyData: true, copyEPC: true);
		BaseGame.SetAllEPCs(s_koko.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(s_koko.epc, "DirLight0Direction", (object)new Vector3(-0.5f, -0.5f, -1f));
		targetTotal = (float)s_koko.indices[0].Length / 3f;
		kokoBones = new Dictionary<ModelBone, int>();
		for (int k = 0; k < ((ReadOnlyCollection<ModelBone>)(object)s_koko.model.Bones).Count; k++)
		{
			if (!kokoBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)s_koko.model.Bones)[k]))
			{
				kokoBones.Add(((ReadOnlyCollection<ModelBone>)(object)s_koko.model.Bones)[k], k);
			}
		}
		r = new Random();
		pE = new PlaneEffect();
		for (int l = 0; l < 24; l++)
		{
			TreeNode treeNode = new TreeNode((float)r.NextDouble(), 0f, 0f, 1, 0.03f, 0.012f, 0.04f, 0.022f);
			treeNode.branchTree = false;
			treeNode.setColor(Color.Green);
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
	}

	public Olu(Dictionary<string, string> attributes, XmlNode node)
		: this()
	{
	}

	public void TailUpdate(GameTime gametime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0812: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Transformation();
		val = Matrix.CreateTranslation(new Vector3(0f, 0f, -0.2f)) * Transformation();
		mainBezier[0] = new BezierHelper(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_1"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_2"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_3"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_Foot"]]] * val));
		mainBezier[1] = new BezierHelper(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_1"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_2"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_3"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_Foot"]]] * val));
		mainBezier[2] = new BezierHelper(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_1"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_2"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_3"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_Foot"]]] * val));
		mainBezier[3] = new BezierHelper(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_1"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_2"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_3"]]] * val), Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_Foot"]]] * val));
		particleCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (particleCooldown <= 0f && drawLegParts)
		{
			if (drawMode != 1 && (float)bosshp[1] > 0f)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmC_Foot"]]] * val), Vector3.Up * 5f, 0.2f, 180f, Vector3.Zero, 0f, 0.5f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), 16, 0.003125f);
			}
			if (drawMode != 2 && (float)bosshp[2] > 0f)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmB_Foot"]]] * val), Vector3.Up * 5f, 0.2f, 180f, Vector3.Zero, 0f, 0.5f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), 16, 0.003125f);
			}
			if (drawMode != 3 && (float)bosshp[3] > 0f)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmA_Foot"]]] * val), Vector3.Up * 5f, 0.2f, 180f, Vector3.Zero, 0f, 0.5f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), 16, 0.003125f);
			}
			if (drawMode != 4 && (float)bosshp[4] > 0f)
			{
				BaseGame.Get().ps.AddParticles(Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_ArmD_Foot"]]] * val), Vector3.Up * 5f, 0.2f, 180f, Vector3.Zero, 0f, 0.5f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), 16, 0.003125f);
			}
			particleCooldown += particleMax;
		}
	}

	public void KokoHairUpdate(GameTime gametime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		Matrix transformation = PreKokoMatrix() * Transformation();
		kokoBez[0] = new BezierHelper(koko.transforms, kokoBones, koko.model.Bones, transformation, "root_2", 3f);
		kokoBez[1] = new BezierHelper(koko.transforms, kokoBones, koko.model.Bones, transformation, "root_3", 3f);
		kokoBez[2] = new BezierHelper(koko.transforms, kokoBones, koko.model.Bones, transformation, "root_4", 3f);
		kokoBez[3] = new BezierHelper(koko.transforms, kokoBones, koko.model.Bones, transformation, "root_5", 3f);
		KokoHairPlayerUpdate(gametime);
	}

	public void KokoHairPlayerUpdate(GameTime gametime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		Matrix transformation = PlayerKokoTransform();
		kokoPlayerBez[0] = new BezierHelper(koko.transforms, kokoBones, koko.model.Bones, transformation, "root_2", 1.2f);
		kokoPlayerBez[1] = new BezierHelper(koko.transforms, kokoBones, koko.model.Bones, transformation, "root_3", 1.2f);
		kokoPlayerBez[2] = new BezierHelper(koko.transforms, kokoBones, koko.model.Bones, transformation, "root_4", 1.2f);
		kokoPlayerBez[3] = new BezierHelper(koko.transforms, kokoBones, koko.model.Bones, transformation, "root_5", 1.2f);
	}

	public void LegPhaseBUpdate(GameTime gametime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Transformation();
		val = Matrix.CreateTranslation(new Vector3(0f, 0f, -0.2f)) * Transformation();
		legBezier[0] = new BezierHelper(Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmA_1"]]] * val), Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmA_2"]]] * val), Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmA_3"]]] * val), Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmA_Foot"]]] * val));
		legBezier[1] = new BezierHelper(Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmB_1"]]] * val), Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmB_2"]]] * val), Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmB_3"]]] * val), Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmB_Foot"]]] * val));
		legBezier[2] = new BezierHelper(Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmC_1"]]] * val), Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmC_2"]]] * val), Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmC_3"]]] * val), Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmC_Foot"]]] * val));
		legBezier[3] = new BezierHelper(Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmD_1"]]] * val), Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmD_2"]]] * val), Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmD_3"]]] * val), Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmD_Foot"]]] * val));
	}

	public override void draw(GameTime gametime)
	{
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Invalid comparison between Unknown and I4
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Invalid comparison between Unknown and I4
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		if (waterLevel < 5f)
		{
			BaseGame.Get().SwitchEffectTechnique("Water");
			BaseGame.Get().fogEffect.Parameters["WaterHeight"].SetValue(waterLevel);
		}
		if (drawFace && alphaAmount > 0.1f)
		{
			if (alphaAmount < 1f)
			{
				BaseGame.Get().SpecifyAlpha = true;
				BaseGame.Get().fogEffect.Parameters["Alpha"].SetValue(alphaAmount);
			}
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Transformation());
			if ((int)fillMode == 2)
			{
				BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
				BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
			}
			else
			{
				BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
			}
			BaseGame.Get().DrawModel(ref oluBack);
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
			BaseGame.Get().DrawModel(ref olu);
			BaseGame.Get().matStack.PopMatrix();
		}
		if (drawLegs && legAlphaAmount > 0.1f)
		{
			if (legAlphaAmount < 1f)
			{
				BaseGame.Get().SpecifyAlpha = true;
				BaseGame.Get().fogEffect.Parameters["Alpha"].SetValue(legAlphaAmount);
			}
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
			BaseGame.Get().SwitchEffectTechnique("Bezier");
			if (waterLevel < 5f)
			{
				BaseGame.Get().SwitchEffectTechnique("WaterBezier");
				BaseGame.Get().fogEffect.Parameters["WaterHeight"].SetValue(waterLevel);
			}
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1f, -1f, -1f));
			for (int i = 0; i < 4; i++)
			{
				if (drawMode != i + 1 && (float)bosshp[i + 1] > 0f)
				{
					BaseGame.Get().fogEffect.Parameters["BezierPos"].SetValue(mainBezier[i].BezierPos);
					BaseGame.Get().fogEffect.Parameters["BezierVel"].SetValue(mainBezier[i].BezierVel);
					BaseGame.Get().fogEffect.Parameters["Pos0"].SetValue(mainBezier[i].pos[0]);
					BaseGame.Get().fogEffect.Parameters["Pos1"].SetValue(mainBezier[i].pos[1]);
					BaseGame.Get().fogEffect.Parameters["Scale"].SetValue(mainBezier[i].scale);
					BaseGame.Get().fogEffect.Parameters["BezierShift"].SetValue(0f);
					BaseGame.Get().DrawModel(ref Hypatia.tail);
				}
			}
			BaseGame.Get().matStack.PopMatrix();
		}
		if ((int)fillMode == 2)
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		}
		BaseGame.Get().SwitchEffectTechnique("Textured");
		if (planeAlphaAmount > 0.1f)
		{
			BaseGame.Get().SpecifyAlpha = true;
			BaseGame.Get().fogEffect.Parameters["Alpha"].SetValue(planeAlphaAmount);
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Transformation());
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
			BaseGame.Get().DrawModel(ref oluPlane);
			BaseGame.Get().matStack.PopMatrix();
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().SpecifyAlpha = false;
		BaseGame.Get().fogEffect.Parameters["Alpha"].SetValue(1f);
		pdColl.draw(gametime);
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		if (rE != null)
		{
			BaseGame.Get().SwitchEffectTechnique("Ripple");
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(150f) * Matrix.CreateTranslation(ripplePos));
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateTranslation(new Vector3(0.5f, 0f, -0.5f)));
			rE.Draw(gametime);
			BaseGame.Get().fogEffect.Begin();
			BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
			pE.draw();
			BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
			BaseGame.Get().fogEffect.End();
			BaseGame.Get().matStack.PopMatrix();
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().SwitchEffectTechnique("Textured");
		foreach (EnemyState state in states)
		{
			state.Draw(gametime);
		}
	}

	public Matrix PreKokoMatrix()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateScale(0.01f) * Matrix.CreateTranslation(new Vector3(0f, -1.5f, 0f));
	}

	public void KokoDraw(GameTime gametime)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		EffectParameterCollectionRedux[] epc = koko.epc;
		Color red = Color.Red;
		Vector3 val = ((Color)(ref red)).ToVector3() * 1.2f;
		Color darkBlue = Color.DarkBlue;
		BaseGame.SetAllEPCs(epc, "DiffuseColor", Vector3.Lerp(val, ((Color)(ref darkBlue)).ToVector3() * 1.2f, 1f - curTargetCount / targetTotal));
		BaseGame.Get().SwitchEffectTechnique("Side");
		if (alphaAmount < 0.9f)
		{
			if (alphaAmount > 0f)
			{
				BaseGame.Get().SpecifyAlpha = true;
				BaseGame.Get().fogEffect.Parameters["Alpha"].SetValue(1f - alphaAmount);
			}
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(PreKokoMatrix() * Transformation());
			BaseGame.Get().fogEffect.Parameters["SideSplice"].SetValue((BaseGame.Get().weaponMode - 0.5f) * 24f);
			BaseGame.Get().fogEffect.Parameters["SideShowLeft"].SetValue(false);
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
			BaseGame.Get().DrawModel(ref koko);
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
			BaseGame.Get().fogEffect.Parameters["SideShowLeft"].SetValue(true);
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
			BaseGame.Get().DrawModel(ref koko);
			BaseGame.Get().SwitchEffectTechnique("Textured");
			if (BaseGame.Get().weaponMode > 0.1f && BaseGame.Get().weaponMode < 0.9f)
			{
				BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
				BaseGame.Get().matStack.PushMatrix();
				BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateTranslation(new Vector3((BaseGame.Get().weaponMode - 0.5f) * 90f, 15f, 0f)));
				BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(150f));
				BaseGame.Get().DrawModel(ref BaseGame.Get().player.mGrid);
				BaseGame.Get().matStack.PopMatrix();
			}
			BaseGame.Get().matStack.PopMatrix();
		}
		BaseGame.Get().SwitchEffectTechnique("Side");
		BaseGame.Get().fogEffect.Parameters["xDoubleSided"].SetValue(true);
		BaseGame.Get().fogEffect.Parameters["SideSplice"].SetValue((BaseGame.Get().weaponMode - 0.5f) * 4f);
		BaseGame.Get().fogEffect.Parameters["SideShowLeft"].SetValue(true);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(PlayerKokoTransform());
		BaseGame.Get().DrawModel(ref koko, clearEpc: false, disableAnim: false, ref playerIndicesToDraw);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().fogEffect.Parameters["SideShowLeft"].SetValue(false);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().DrawModel(ref koko, clearEpc: false, disableAnim: false, ref playerIndicesToDraw);
		BaseGame.Get().matStack.PopMatrix();
		DrawKokoHair(gametime);
		BaseGame.Get().SpecifyAlpha = false;
		BaseGame.Get().fogEffect.Parameters["Alpha"].SetValue(1f);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
	}

	private Matrix PlayerKokoTransform()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = Matrix.CreateRotationY(finalTurnAmount);
		val = BaseGame.MapObjectToSystem(Vector3.Zero, BaseGame.Get().playerDir, BaseGame.Get().playerUp) * Matrix.CreateTranslation(BaseGame.Get().playerPos) * val;
		val = PreKokoMatrix() * val;
		val = Matrix.CreateScale(1.6f) * val;
		return Matrix.CreateTranslation(new Vector3(0f, -10f, 0f)) * val;
	}

	public void DrawKokoHair(GameTime gametime)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().SwitchEffectTechnique("Bezier");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1f, -1f, -1f));
		for (int i = 0; i < kokoBez.Length; i++)
		{
			BaseGame.Get().fogEffect.Parameters["BezierPos"].SetValue(kokoBez[i].BezierPos);
			BaseGame.Get().fogEffect.Parameters["BezierVel"].SetValue(kokoBez[i].BezierVel);
			BaseGame.Get().fogEffect.Parameters["Pos0"].SetValue(kokoBez[i].pos[0]);
			BaseGame.Get().fogEffect.Parameters["Pos1"].SetValue(kokoBez[i].pos[1]);
			BaseGame.Get().fogEffect.Parameters["Scale"].SetValue(kokoBez[i].scale);
			if (curTargetCount != targetTotal)
			{
				BaseGame.Get().fogEffect.Parameters["BezierShift"].SetValue(0.99f - GetHairProgress(i) * 1.01f);
				if (GetHairProgress(i) > 0.01f)
				{
					BaseGame.Get().DrawModel(ref Hypatia.tail);
				}
			}
			else
			{
				BaseGame.Get().fogEffect.Parameters["BezierShift"].SetValue(0f);
				BaseGame.Get().DrawModel(ref Hypatia.tail);
			}
			BaseGame.Get().DrawModel(ref tail);
		}
		BaseGame.Get().fogEffect.Parameters["xPose"].SetValue(Matrix.Identity);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1f, -1f, -1f));
		for (int j = 0; j < kokoPlayerBez.Length; j++)
		{
			BaseGame.Get().fogEffect.Parameters["BezierPos"].SetValue(kokoPlayerBez[j].BezierPos);
			BaseGame.Get().fogEffect.Parameters["BezierVel"].SetValue(kokoPlayerBez[j].BezierVel);
			BaseGame.Get().fogEffect.Parameters["Pos0"].SetValue(kokoPlayerBez[j].pos[0]);
			BaseGame.Get().fogEffect.Parameters["Pos1"].SetValue(kokoPlayerBez[j].pos[1]);
			BaseGame.Get().fogEffect.Parameters["Scale"].SetValue(kokoPlayerBez[j].scale);
			if (curTargetCount != targetTotal)
			{
				BaseGame.Get().fogEffect.Parameters["BezierShift"].SetValue(-0.01f + GetHairProgress(j) * 1.01f);
				if (GetHairProgress(j) < 0.99f)
				{
					BaseGame.Get().DrawModel(ref Hypatia.tail);
				}
			}
		}
		BaseGame.Get().fogEffect.Parameters["xPose"].SetValue(Matrix.Identity);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
	}

	public void DrawLegs(GameTime gametime)
	{
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().SwitchEffectTechnique("Bezier");
		if (waterLevel < 5f)
		{
			BaseGame.Get().SwitchEffectTechnique("WaterBezier");
			BaseGame.Get().fogEffect.Parameters["WaterHeight"].SetValue(waterLevel);
		}
		if (linePos > -1f)
		{
			BaseGame.Get().SwitchEffectTechnique("BezierLine");
			BaseGame.Get().fogEffect.Parameters["LineBasis"].SetValue(linePos);
		}
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1f, -1f, -1f));
		for (int i = 0; i < 4; i++)
		{
			BaseGame.SetAllEPCs(tail.epc, "DiffuseColor", new Vector3(1f, 1f, 1f) * (BaseGame.Get().channels[13 + i] * 0.95f + 0.05f));
			BaseGame.Get().fogEffect.Parameters["BezierPos"].SetValue(legBezier[i].BezierPos);
			BaseGame.Get().fogEffect.Parameters["BezierVel"].SetValue(legBezier[i].BezierVel);
			BaseGame.Get().fogEffect.Parameters["Pos0"].SetValue(legBezier[i].pos[0]);
			BaseGame.Get().fogEffect.Parameters["Pos1"].SetValue(legBezier[i].pos[1]);
			BaseGame.Get().fogEffect.Parameters["Scale"].SetValue(legBezier[i].scale);
			BaseGame.Get().fogEffect.Parameters["BezierShift"].SetValue(0f);
			BaseGame.Get().DrawModel(ref tail);
		}
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().fogEffect.Parameters["LineBasis"].SetValue(-11);
	}

	public void DrawPhase3Legs(GameTime gametime)
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().SwitchEffectTechnique("Bezier");
		if (linePos > -1f)
		{
			BaseGame.Get().SwitchEffectTechnique("BezierLine");
			BaseGame.Get().fogEffect.Parameters["LineBasis"].SetValue(linePos);
		}
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1f, -1f, -1f));
		for (int i = 0; i < 4; i++)
		{
			int num = i;
			switch (i)
			{
			case 1:
				num = 2;
				break;
			case 2:
				num = 1;
				break;
			}
			BaseGame.SetAllEPCs(tail.epc, "DiffuseColor", new Vector3(1f, 1f, 1f) * (BaseGame.Get().channels[13 + num] * 0.95f + 0.05f));
			BaseGame.Get().fogEffect.Parameters["BezierShift"].SetValue(0f);
			for (int j = 0; j < spiralBezier[i].Count && (float)j < legProgress; j++)
			{
				if ((float)(j + 1) - legProgress < 1f && (float)(j + 1) - legProgress > 0f)
				{
					BaseGame.Get().fogEffect.Parameters["BezierShift"].SetValue((float)(j + 1) - legProgress);
				}
				BaseGame.Get().fogEffect.Parameters["BezierPos"].SetValue(spiralBezier[i][j].BezierPos);
				BaseGame.Get().fogEffect.Parameters["BezierVel"].SetValue(spiralBezier[i][j].BezierVel);
				BaseGame.Get().fogEffect.Parameters["Pos0"].SetValue(spiralBezier[i][j].pos[0]);
				BaseGame.Get().fogEffect.Parameters["Pos1"].SetValue(spiralBezier[i][j].pos[1]);
				BaseGame.Get().fogEffect.Parameters["Scale"].SetValue(spiralBezier[i][j].scale);
				BaseGame.Get().DrawModel(ref tail);
			}
		}
		BaseGame.Get().fogEffect.Parameters["xPose"].SetValue(Matrix.Identity);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().fogEffect.Parameters["LineBasis"].SetValue(-15);
	}

	public override Matrix Transformation()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (dirty)
		{
			_transformation = Matrix.CreateTranslation(new Vector3(0f, 0f, 0f - offset)) * Matrix.CreateScale(new Vector3(size, size, 0f - size)) * Matrix.CreateTranslation(getPos());
			dirty = false;
		}
		return _transformation;
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		if (toHit.eTarget is FaceTarget)
		{
			FaceTarget faceTarget = (FaceTarget)toHit.eTarget;
			bosshp[5] -= 2;
			hitPoints += 2;
			curTargetCount--;
			TargetRemove(faceTarget);
			targets.Remove(faceTarget);
			for (int i = 0; i < 5; i++)
			{
				if (targets.Count > 0)
				{
					int index = r.Next(targets.Count);
					if (targets[index] is FaceTarget && targets[index].selected == 0)
					{
						TargetRemove((FaceTarget)targets[index]);
						targets.RemoveAt(index);
						curTargetCount--;
					}
				}
			}
		}
		else if (toHit.eTarget is BoneModelTarget)
		{
			BoneModelTarget boneModelTarget = (BoneModelTarget)toHit.eTarget;
			bosshp[boneModelTarget.id]--;
			hitPoints++;
			if (boneModelTarget.fillMode != toHit.fillMode)
			{
				bosshp[boneModelTarget.id]--;
				hitPoints++;
			}
		}
	}

	public override void HitSound(int lockNum, float volume)
	{
		if (lockNum <= 8)
		{
			BaseGame.Get().PlayCue(wCond[BaseGame.Get().curBeat / 2].cueName, volume);
		}
	}

	public void TargetRemove(FaceTarget ft)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		SplitList(koko, ft.meshNum, ft.indexNum);
		pdColl.AddPlanePath(ref koko, ft.meshNum, ft.indexNum, this, ft.fillMode, ft.modMatrix);
	}

	public void NoteHit(int notePart, int damage)
	{
		bosshp[notePart] -= damage;
	}

	public override void act(GameTime gametime)
	{
		if (!exists)
		{
			return;
		}
		if (oluAC[0] != null)
		{
			((GameComponent)oluAC[0]).Update(gametime);
		}
		if (oluAC[1] != null)
		{
			((GameComponent)oluAC[1]).Update(gametime);
		}
		if (oluAC2[0] != null)
		{
			((GameComponent)oluAC2[0]).Update(gametime);
		}
		if (oluAC2[1] != null)
		{
			((GameComponent)oluAC2[1]).Update(gametime);
		}
		if (legAC != null)
		{
			((GameComponent)legAC).Update(gametime);
		}
		if (kokoAC != null)
		{
			((GameComponent)kokoAC).Update(gametime);
		}
		TailUpdate(gametime);
		pdColl.act(gametime);
		if (rE != null)
		{
			rE.Update(gametime);
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
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		new Random();
		base.start();
		reL = new List<RippleEffect>();
		oluPlane = new ModelWrapper(s_oluPlane, copyEPC: true);
		oluPlane.ResetIndicesToDraw();
		koko = new ModelWrapper(s_koko, copyEPC: true);
		koko.ResetIndicesToDraw();
		bpColl = new BulletPlaneCollection(ref oluPlane);
		bpColl.start();
		BaseGame.Get().enems.Add(bpColl);
		oluAnim = new ModelOluAnimator[2];
		oluAnim[0] = new ModelOluAnimator(BaseGame.Get().CoreGame, olu, BaseGame.GetFogEffect());
		oluAnim[1] = new ModelOluAnimator(BaseGame.Get().CoreGame, oluBack, BaseGame.GetFogEffect());
		oluAC = new AnimationController[2];
		oluAC2 = new AnimationController[2];
		legAnim = new ModelOluAnimator(BaseGame.Get().CoreGame, oluLegs, BaseGame.GetFogEffect());
		kokoAnim = new ModelOluAnimator(BaseGame.Get().CoreGame, koko, BaseGame.GetFogEffect());
		mainBezier = new BezierHelper[4];
		legBezier = new BezierHelper[4];
		kokoBez = new BezierHelper[4];
		kokoPlayerBez = new BezierHelper[4];
		float num = 0.85f;
		float num2 = 3f;
		Vector3[] array = (Vector3[])(object)new Vector3[4];
		Vector3[] array2 = (Vector3[])(object)new Vector3[4]
		{
			new Vector3(0f, (0f - size) * num2, (offset + 3.3f) * size),
			new Vector3(num2 * 0.5f * size, (0f - size) * num2, (offset + 3.3f - num / 4f) * size),
			new Vector3(num2 * size, (0f - num2) * 0.5f * size, (offset + 3.3f - num * 3f / 4f) * size),
			new Vector3(num2 * size, 0f, (offset + 3.3f - num) * size)
		};
		for (int i = 0; i < 4; i++)
		{
			ref Vector3 reference = ref array2[i];
			reference = Vector3.Transform(array2[i], Matrix.CreateRotationZ(MathHelper.ToRadians(90f)));
		}
		for (int j = 0; j < 10; j++)
		{
			for (int k = 0; k < 4; k++)
			{
				array = (Vector3[])(object)new Vector3[4];
				for (int l = 0; l < 4; l++)
				{
					ref Vector3 reference2 = ref array[l];
					reference2 = Vector3.Transform(array2[l], Matrix.CreateRotationZ(0f - MathHelper.ToRadians((float)k * 90f)));
				}
				spiralPoints[k].Add(array);
				BezierHelper item = new BezierHelper(array[0], array[1], array[2], array[3]);
				spiralBezier[k].Add(item);
			}
			for (int m = 0; m < 4; m++)
			{
				ref Vector3 reference3 = ref array2[m];
				reference3 = Vector3.Transform(array2[m], Matrix.CreateRotationZ(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(new Vector3(0f, 0f, (0f - num) * size)));
			}
		}
		pdColl = new PlaneDetachColl(ref koko);
		pdColl.eParent = this;
		curTargetCount = targetTotal;
		playerIndicesToDraw = new List<int>[((ReadOnlyCollection<ModelMesh>)(object)koko.model.Meshes).Count];
		for (int n = 0; n < ((ReadOnlyCollection<ModelMesh>)(object)koko.model.Meshes).Count; n++)
		{
			playerIndicesToDraw[n] = new List<int>();
		}
		states = new List<EnemyState>();
		if (BaseGame.release)
		{
			states.Add(PhaseStartState());
		}
		else
		{
			states.Add(PhaseStartState());
		}
		resetMusic = false;
		addCond(new NeverCondition());
		basePart = new MusicPart(0, "Silence", 0, 8);
		AddCue(0, "41Chords", 0, 4);
		AddCue(0, "41Drums", 0, 4);
		AddCue(0, "41Triangle", 0, 4);
		finalTurnAmount = 0f;
		linePos = -10f;
		size = 10f;
		offset = 4f;
		legProgress = 0f;
		alphaAmount = 1f;
		legAlphaAmount = 1f;
		planeAlphaAmount = 0f;
	}

	public void AddCue(int beat, string name, int playMeas, int loopMeas)
	{
		bossMusic.Add(new MusicPart(beat, name, playMeas, loopMeas));
	}

	public void AddTentCue(int beat, string name, int playMeas, int loopMeas)
	{
		tentBossMusic.Add(new MusicPart(beat, name, playMeas, loopMeas));
	}

	public void PlayMusic(GameTime gametime)
	{
		basePart.Update(gametime);
		if (tentBossMusic.Count > 0 && (basePart.curMeasure == 3 || basePart.curMeasure == 7 || resetMusic) && BaseGame.Get().curBeat == 15)
		{
			if (resetMusic)
			{
				basePart.curMeasure = 7;
				resetMusic = false;
			}
			bossMusic.Clear();
			foreach (MusicPart item in tentBossMusic)
			{
				bossMusic.Add(new MusicPart(item));
			}
			tentBossMusic.Clear();
		}
		for (int num = bossMusic.Count - 1; num >= 0; num--)
		{
			bossMusic[num].Update(gametime);
			if (bossMusic[num].done)
			{
				bossMusic.RemoveAt(num);
			}
		}
	}

	private void ShootTile()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		bpColl.AddPlane(this, ref oluPlane, curMesh, curIndex, 0, new Vector3(0.7f, 1f, 0.7f), new Vector3(0f, 1f, 0f), new PathList(), curPart, 0.1f, followPath: false, (FillMode)3);
		SplitList(oluPlane, curMesh, curIndex);
		int index = bpColl.enemies.Count - 1;
		bpColl.enemies[index].LaunchOlu();
		bpColl.detached.Add(bpColl.enemies[index]);
		bpColl.enemies.RemoveAt(index);
		curIndex += 3;
		if (curIndex >= oluPlane.indices[curMesh].Length - 1)
		{
			curMesh = -1;
		}
	}

	private void ShootTileKoko()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		curMesh = 0;
		curIndex = 0;
		for (curIndex = 0; curIndex < koko.indices[curMesh].Length - 1; curIndex += 3)
		{
			addTarget(1, 10, ref koko, curMesh, curIndex, PreKokoMatrix());
		}
		curMesh = -1;
		curIndex = -1;
	}

	private void SplitList(ModelWrapper mwModel, int _mesh, int _index)
	{
		bool flag = false;
		for (int i = 0; i < mwModel.indicesToDraw[_mesh].Count - 1; i += 2)
		{
			if (flag)
			{
				break;
			}
			if (_index < mwModel.indicesToDraw[_mesh][i] || _index > mwModel.indicesToDraw[_mesh][i + 1])
			{
				continue;
			}
			flag = true;
			if (_index == mwModel.indicesToDraw[_mesh][i])
			{
				if (_index == mwModel.indicesToDraw[_mesh][i + 1] - 2)
				{
					mwModel.indicesToDraw[_mesh].RemoveAt(i);
					mwModel.indicesToDraw[_mesh].RemoveAt(i);
				}
				else
				{
					mwModel.indicesToDraw[_mesh][i] = _index + 3;
				}
			}
			else if (_index == mwModel.indicesToDraw[_mesh][i + 1] - 2)
			{
				mwModel.indicesToDraw[_mesh][i + 1] = _index - 1;
			}
			else
			{
				mwModel.indicesToDraw[_mesh].Insert(i + 1, _index + 3);
				mwModel.indicesToDraw[_mesh].Insert(i + 1, _index - 1);
			}
		}
	}

	public void AddFace(int _mesh, int _index)
	{
		AddList(playerIndicesToDraw, _mesh, _index);
	}

	private void AddList(List<int>[] mwModel, int _mesh, int _index)
	{
		bool flag = false;
		for (int i = 0; i < mwModel[_mesh].Count - 1; i += 2)
		{
			if (flag)
			{
				break;
			}
			if (_index > mwModel[_mesh][i + 1] + 3)
			{
				continue;
			}
			flag = true;
			if (_index + 3 == mwModel[_mesh][i])
			{
				mwModel[_mesh][i] = _index;
			}
			else if (_index < mwModel[_mesh][i])
			{
				mwModel[_mesh].Insert(i, _index + 2);
				mwModel[_mesh].Insert(i, _index);
			}
			else if (_index == mwModel[_mesh][i + 1] + 1)
			{
				if (i < mwModel[_mesh].Count - 2 && _index + 3 == mwModel[_mesh][i + 2])
				{
					mwModel[_mesh].RemoveAt(i + 1);
					mwModel[_mesh].RemoveAt(i + 1);
				}
				else
				{
					mwModel[_mesh][i + 1] = _index + 2;
				}
			}
		}
		if (!flag)
		{
			mwModel[_mesh].Add(_index);
			mwModel[_mesh].Add(_index + 2);
		}
	}

	public void CreatePhaseATargets()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 250; i++)
		{
			double num = r.NextDouble();
			double num2 = Math.PI * 2.0 * r.NextDouble();
			addTarget(new Vector3((float)(num * Math.Cos(num2)), 0f, (float)(num * Math.Sin(num2))), 1, 10, ref olu, 0, "Armature_Face");
		}
		bosshp[0] = 36;
	}

	public void RemoveAllTargets()
	{
		for (int num = targets.Count - 1; num >= 0; num--)
		{
			targets[num].Dispose();
		}
		targets.Clear();
	}

	public override Vector3 getPos()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(0f, 0f, 0f);
	}

	public override string name()
	{
		return "Olu - Protector class";
	}

	public void SetAnimation(string strAnim, bool loop)
	{
		for (int i = 0; i < 2; i++)
		{
			oluAC[i] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[i].Animations[strAnim], component: false);
			BaseGame.RunController(oluAnim[i], oluAC[i]);
			oluAC[i].IsLooping = loop;
			((GameComponent)oluAC[i]).Update(BaseGame.Get().emptytime);
		}
	}

	public void SetAnimation(string strAnim, string strAnim2, bool loop)
	{
		SetAnimation(strAnim, strAnim2, loop, reset1: true, reset2: true);
	}

	public void SetAnimation(string strAnim, string strAnim2, bool loop, bool reset1, bool reset2)
	{
		for (int i = 0; i < 2; i++)
		{
			if (reset1)
			{
				oluAC[i] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[i].Animations[strAnim], component: false);
			}
			if (reset2)
			{
				oluAC2[i] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[i].Animations[strAnim2], component: false);
			}
			BaseGame.RunController(oluAnim[i], oluAC[i], oluAC2[i], 0f);
			oluAC[i].IsLooping = loop;
			oluAC2[i].IsLooping = loop;
			((GameComponent)oluAC2[i]).Update(BaseGame.Get().emptytime);
			((GameComponent)oluAC[i]).Update(BaseGame.Get().emptytime);
		}
	}

	public void SetAnimLegs(string strAnim, bool loop)
	{
		legAC = new AnimationController(BaseGame.Get().CoreGame, legAnim.Animations[strAnim], component: false);
		BaseGame.RunController(legAnim, legAC);
		legAC.IsLooping = loop;
		((GameComponent)legAC).Update(BaseGame.Get().emptytime);
	}

	public void SetAnimKoko(string strAnim, bool loop)
	{
		kokoAC = new AnimationController(BaseGame.Get().CoreGame, kokoAnim.Animations[strAnim], component: false);
		BaseGame.RunController(kokoAnim, kokoAC);
		kokoAC.IsLooping = loop;
		((GameComponent)kokoAC).Update(BaseGame.Get().emptytime);
	}

	public void SetBlend(float amount)
	{
		for (int i = 0; i < 2; i++)
		{
			BaseGame.RunController(oluAnim[i], oluAC[i], oluAC2[i], amount);
		}
	}

	public bool AlwaysRemove(ConditionSet cs)
	{
		return true;
	}

	public bool AnimFinished(ConditionSet cs)
	{
		if (oluAC[0] != null)
		{
			return oluAC[0].Duration - oluAC[0].ElapsedTime < 2;
		}
		return false;
	}

	public bool AnimFinished2(ConditionSet cs)
	{
		if (oluAC2[0] != null)
		{
			return oluAC2[0].Duration - oluAC2[0].ElapsedTime < 2;
		}
		return false;
	}

	public bool AnimFinishedLegs(ConditionSet cs)
	{
		if (legAC != null)
		{
			return legAC.Duration - legAC.ElapsedTime < 2;
		}
		return false;
	}

	public bool HPStage1Remove(ConditionSet cs)
	{
		return bosshp[0] <= 0;
	}

	public void NormalUpdate(GameTime gametime)
	{
		if (oluAnim[0] != null)
		{
			((GameComponent)oluAnim[0]).Update(gametime);
		}
		if (oluAnim[1] != null)
		{
			((GameComponent)oluAnim[1]).Update(gametime);
		}
		if (legAnim != null)
		{
			((GameComponent)legAnim).Update(gametime);
		}
		if (kokoAnim != null)
		{
			((GameComponent)kokoAnim).Update(gametime);
		}
		PlayMusic(gametime);
		particleCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (particleCooldown < 0f)
		{
			particleCooldown += particleMax;
		}
	}

	private EnemyState PhaseStartState()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		EnemyState result = ((!(r.NextDouble() > 0.5)) ? PhaseARightStartState() : PhaseALeftStartState());
		fillMode = (FillMode)2;
		CreatePhaseATargets();
		drawMode = 0;
		drawLegParts = true;
		waterLevel = 10f;
		return result;
	}

	private void ResetGraphicsSettings()
	{
		drawMode = 0;
		drawLegParts = true;
		waterLevel = 10f;
		alphaAmount = 1f;
		legAlphaAmount = 1f;
	}

	private EnemyState PhaseALeftStartState()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, null, AnimFinished, PhaseALeftFightState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		linePos = -10f;
		SetAnimation("enterleft", loop: false);
		ResetGraphicsSettings();
		return enemyState;
	}

	private EnemyState PhaseARightStartState()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, null, AnimFinished, PhaseARightFightState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		linePos = -10f;
		SetAnimation("enterright", loop: false);
		ResetGraphicsSettings();
		return enemyState;
	}

	private EnemyState PhaseALeftFightState()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, null, HPStage1Remove, PhaseALeftDiveState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		SetAnimation("hoverleft", loop: true);
		return enemyState;
	}

	private EnemyState PhaseARightFightState()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, null, HPStage1Remove, PhaseARightDiveState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		SetAnimation("hoverright", loop: true);
		return enemyState;
	}

	private EnemyState PhaseALeftDiveState()
	{
		EnemyState enemyState = new EnemyState(PhaseADiveUpdate, null, AnimFinished2, PhaseASpawnState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		RemoveAllTargets();
		SetAnimation("hoverleft", "diveleft", loop: false, reset1: false, reset2: true);
		phaseCountdown = 0f;
		phaseMax = 1.2f;
		waterLevel = -40f;
		splashActive = false;
		return enemyState;
	}

	private EnemyState PhaseARightDiveState()
	{
		EnemyState enemyState = new EnemyState(PhaseADiveUpdate, null, AnimFinished2, PhaseASpawnState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		RemoveAllTargets();
		SetAnimation("hoverright", "diveright", loop: false, reset1: false, reset2: true);
		phaseCountdown = 0f;
		phaseMax = 1.2f;
		waterLevel = -40f;
		splashActive = false;
		return enemyState;
	}

	public void PhaseADiveUpdate(GameTime gametime)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if (phaseCountdown < phaseMax)
		{
			phaseCountdown += (float)gametime.ElapsedGameTime.TotalSeconds;
			if (phaseCountdown > phaseMax)
			{
				phaseCountdown = phaseMax;
			}
			SetBlend(phaseCountdown / phaseMax);
		}
		Vector3 val = Vector3.Transform(Vector3.Zero, olu.transforms[oluBones[olu.model.Bones["Armature_Face"]]] * Transformation());
		if (val.Y < waterLevel && !splashActive)
		{
			ripplePos = new Vector3(val.X, waterLevel, val.Z);
			rE = new RippleEffect(ripplePos, 0.25f, 0.25f, 0f, 1.25f, 6f, _loop: false, 0f);
			rE.fxUpdate = BaseGame.GetFogEffect().Parameters;
			rE.done = false;
			splashActive = true;
		}
		NormalUpdate(gametime);
	}

	private EnemyState PhaseASpawnState()
	{
		EnemyState result = new EnemyState(PhaseASpawnUpdate, null, PhaseASpawnRemove, PhaseABTransitionState);
		drawLegParts = false;
		tentBossMusic.Clear();
		AddTentCue(0, "42Transition", 0, 8);
		curLeg = r.Next(4);
		phaseCountdown = 0f;
		phaseMax = 0f;
		switch (curLeg)
		{
		case 0:
			phaseMax = 25f;
			break;
		case 1:
			phaseMax = 20f;
			break;
		case 2:
			phaseMax = 8f;
			break;
		case 3:
			phaseMax = 22f;
			break;
		}
		phaseCountdown2 = 10f / phaseMax;
		return result;
	}

	private void PhaseASpawnUpdate(GameTime gametime)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		NormalUpdate(gametime);
		phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (phaseCountdown < 0f && phaseMax > 0f)
		{
			switch (curLeg)
			{
			case 0:
			{
				ECube eCube = new ECube(_oluMode: true);
				Vector3 randPosSide = BaseGame.GetRandPosSide(new Vector3(-30f, -30f, 80f), new Vector3(30f, -30f, 110f));
				eCube.addPath(new PBezier(randPosSide, randPosSide + new Vector3(0f, 40f, -1f), 0.5f, Vector3.Up, 0f, 0f, 1f, 19, 0f, 0.0, 0.0));
				eCube.addPath(new PBezier(randPosSide + new Vector3(0f, 40f, -1f), BaseGame.Get().playerPos, 0.15f, Vector3.Up, 0f, 0f, 1f, 19, 0f, 0.0, 0.0));
				eCube.start();
				BaseGame.Get().enems.Add(eCube);
				break;
			}
			case 1:
			{
				Vector3 randPosSide = BaseGame.GetRandPosSide(new Vector3(-30f, -45f, 80f), new Vector3(30f, -45f, 110f));
				base.pathList = new PathList();
				base.pathList.Add(new PLine(randPosSide, randPosSide + new Vector3(0f, 55f, 0f), 20f));
				base.pathList.Add(new PLine(randPosSide + new Vector3(0f, 55f, 0f), BaseGame.Get().playerPos + new Vector3(0f, 0f, -30f), 40f));
				Fish01 fish = new Fish01(base.pathList, -30f, _drawRipple: true, (FillMode)2, _oluMode: true);
				fish.start();
				BaseGame.Get().enems.Add(fish);
				break;
			}
			case 2:
			{
				Serpent serpent = new Serpent(8, 0.15f, 19, 0f, Vector3.Forward, CreateSerpentPath(10), (FillMode)2, _oluMode: true, _waterEnabled: true, -29f);
				serpent.start();
				BaseGame.Get().enems.Add(serpent);
				break;
			}
			case 3:
			{
				Vector3 randPosSide = BaseGame.GetRandPosSide(new Vector3(-30f, -30f, 80f), new Vector3(30f, -30f, 110f));
				PathList pathList = new PathList();
				pathList.Add(new PBezier(randPosSide, randPosSide + new Vector3(0f, 40f, -1f), 0.5f, Vector3.Up, 0f, 0f, 1f, 19, 0f, 0.0, 0.0));
				pathList.Add(new PBezier(randPosSide + new Vector3(0f, 40f, -1f), BaseGame.Get().playerPos, 0.15f, Vector3.Up, 0f, 0f, 1f, 19, 0f, 0.0, 0.0));
				Gift gift = new Gift(pathList, 10f, 0.5f, (FillMode)2, _oluMode: true);
				gift.start();
				BaseGame.Get().enems.Add(gift);
				break;
			}
			}
			phaseCountdown += phaseCountdown2;
			phaseMax--;
		}
	}

	private PathList CreateSerpentPath(int pathSegments)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		PathList pathList = new PathList();
		Vector3[] array = (Vector3[])(object)new Vector3[pathSegments + 1];
		ref Vector3 reference = ref array[0];
		reference = BaseGame.GetRandPosSide(new Vector3(-60f, -30f, 30f), new Vector3(60f, -30f, 90f));
		ref Vector3 reference2 = ref array[pathSegments];
		reference2 = BaseGame.Get().playerPos;
		for (int i = 1; i < pathSegments; i++)
		{
			ref Vector3 reference3 = ref array[i];
			reference3 = BaseGame.GetRandPosCube(new Vector3(-30f, -30f, 30f), new Vector3(30f, 30f, 90f));
		}
		Vector3[] array2 = (Vector3[])(object)new Vector3[pathSegments * 3 + 1];
		for (int j = 0; j <= pathSegments; j++)
		{
			ref Vector3 reference4 = ref array2[j * 3];
			reference4 = array[j];
		}
		for (int k = 0; k < pathSegments; k++)
		{
			Vector3 val = array2[(k + 1) * 3] - array2[k * 3];
			val /= 2f;
			Vector3 val2 = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(val), Vector3.Up));
			val2 *= ((Vector3)(ref val)).Length();
			val2 = Vector3.Transform(val2, Matrix.CreateFromAxisAngle(Vector3.Normalize(val), MathHelper.ToRadians(360f * (float)r.NextDouble())));
			ref Vector3 reference5 = ref array2[3 * k + 1];
			reference5 = array2[3 * k] + val + val2;
		}
		for (int l = 0; l < pathSegments; l++)
		{
			ref Vector3 reference6 = ref array2[3 * l + 2];
			reference6 = (3f * array2[3 * l + 1] + 2f * array2[3 * l + 3]) / 5f;
			ref Vector3 reference7 = ref array2[3 * l + 1];
			reference7 = (3f * array2[3 * l + 1] + 2f * array2[3 * l]) / 5f;
		}
		for (int m = 1; m < pathSegments; m++)
		{
			ref Vector3 reference8 = ref array2[m * 3];
			reference8 = (array2[m * 3 - 1] + array2[m * 3 + 1]) / 2f;
		}
		for (int n = 0; n < pathSegments - 1; n++)
		{
			float num2 = num;
			Vector3 val3 = array2[(n + 1) * 3] - array2[n * 3];
			num = num2 + ((Vector3)(ref val3)).Length();
		}
		for (int num3 = 0; num3 < pathSegments; num3++)
		{
			Vector3 p = array2[num3 * 3];
			Vector3 p2 = array2[num3 * 3 + 1];
			Vector3 p3 = array2[num3 * 3 + 2];
			Vector3 p4 = array2[num3 * 3 + 3];
			float num4 = 0.075f * num;
			Vector3 val4 = array[num3 + 1] - array[num3];
			pathList.Add(new PBezier(p, p2, p3, p4, num4 / ((Vector3)(ref val4)).Length(), Vector3.Up, 0f, 0f, 1f, 19, 0f, 0.0, 0.0));
		}
		return pathList;
	}

	private bool PhaseASpawnRemove(ConditionSet cs)
	{
		if (phaseMax < 0.1f)
		{
			return BaseGame.Get().enems.Count == 2;
		}
		return false;
	}

	private EnemyState PhaseABTransitionState()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		waterLevel = 10f;
		oluAC2[0] = null;
		oluAC2[1] = null;
		fillMode = (FillMode)3;
		tentBossMusic.Clear();
		AddTentCue(0, "43Bass", 0, 4);
		AddTentCue(0, "43Chord", 0, -1);
		AddTentCue(0, "43Lead", 0, 4);
		AddTentCue(0, "43Drums", 0, 4);
		AddTentCue(0, "43Triangle", 0, 4);
		return PhaseBNoseChooseState();
	}

	private EnemyState PhaseBNoseChooseState()
	{
		curLeg = r.Next(activeLegs.Count);
		curLeg = activeLegs[curLeg] + 1;
		if (BaseGame.quickload)
		{
			curLeg = 3;
		}
		EnemyState enemyState = new EnemyState(PhaseBNoseChooseUpdate, null, AnimFinished, PhaseBLeg1State);
		switch (curLeg)
		{
		case 1:
			enemyState = new EnemyState(PhaseBNoseChooseUpdate, null, AnimFinished, PhaseBLeg4State);
			SetAnimation("phaseshiftd", loop: false);
			break;
		case 2:
			enemyState = new EnemyState(PhaseBNoseChooseUpdate, null, AnimFinished, PhaseBLeg3State);
			SetAnimation("phaseshiftc", loop: false);
			break;
		case 3:
			enemyState = new EnemyState(PhaseBNoseChooseUpdate, null, AnimFinished, PhaseBLeg2State);
			SetAnimation("phaseshiftb", loop: false);
			break;
		case 4:
			enemyState = new EnemyState(PhaseBNoseChooseUpdate, null, AnimFinished, PhaseBLeg1State);
			SetAnimation("phaseshifta", loop: false);
			break;
		}
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		alphaAmount = 0f;
		legAlphaAmount = 0f;
		phaseCountdown = 0f;
		phaseMax = 4.2f;
		oluAC[0] = null;
		oluAC[1] = null;
		return enemyState;
	}

	private void PhaseBNoseChooseUpdate(GameTime gametime)
	{
		NormalUpdate(gametime);
		if (!(phaseCountdown < phaseMax))
		{
			return;
		}
		phaseCountdown += (float)gametime.ElapsedGameTime.TotalSeconds;
		if (phaseCountdown >= phaseMax && oluAC[0] == null)
		{
			phaseCountdown = phaseMax;
			switch (curLeg)
			{
			case 1:
				SetAnimation("phaseshiftd", loop: false);
				break;
			case 2:
				SetAnimation("phaseshiftc", loop: false);
				break;
			case 3:
				SetAnimation("phaseshiftb", loop: false);
				break;
			case 4:
				SetAnimation("phaseshifta", loop: false);
				break;
			}
		}
		alphaAmount = phaseCountdown / phaseMax;
		legAlphaAmount = alphaAmount;
	}

	private void PhaseBLegUpdate(GameTime gametime)
	{
		NormalUpdate(gametime);
		LegPhaseBUpdate(gametime);
	}

	private EnemyState PhaseBLeg1State()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg1Update, DrawLegs, AnimFinishedLegs, PhaseBLeg1PreFightState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		drawMode = 4;
		SetAnimLegs("legaintro", loop: false);
		alphaAmount = 1f;
		legAlphaAmount = 1f;
		phaseCountdown = 3.2f;
		phaseMax = 0f;
		return enemyState;
	}

	private void PhaseBLeg1Update(GameTime gametime)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		PhaseBLegUpdate(gametime);
		for (int i = 13; i < 17; i++)
		{
			BaseGame.Get().channels[i] = 1f;
		}
		if (phaseCountdown > 0f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			if (phaseCountdown <= 0f && phaseMax <= 0.1f)
			{
				phaseCountdown = 3.2f;
				phaseMax = 3.2f;
			}
			alphaAmount = phaseCountdown / phaseMax;
			legAlphaAmount = alphaAmount;
		}
		if (phaseMax > 0f)
		{
			BaseGame.Get().MovePlayerDir(Vector3.Transform(new Vector3(0f, 0f, 1f), Matrix.CreateRotationY((float)Math.PI * -5f / 14f * (phaseMax - phaseCountdown) / phaseMax)));
		}
	}

	private EnemyState PhaseBLeg1PreFightState()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg1PreFightUpdate, DrawLegs, null, PhaseBLeg1FightState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.200000047683716));
		enemyState.condSet.Start();
		phaseCountdown = 1.8f;
		phaseMax = 1.8f;
		return enemyState;
	}

	private void PhaseBLeg1PreFightUpdate(GameTime gametime)
	{
		PhaseBLegUpdate(gametime);
		if (phaseCountdown > 0f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			if (phaseCountdown <= 0f)
			{
				phaseCountdown = 0f;
			}
		}
	}

	private EnemyState PhaseBLeg1FightState()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg1FightUpdate, DrawLegs, PhaseBLeg1FightRemove, PhaseBLeg1FinishState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		if (BaseGame.release)
		{
			phaseMax = 64f;
		}
		else
		{
			phaseMax = 2f;
		}
		phaseCountdown = 0.2f;
		return enemyState;
	}

	private void PhaseBLeg1FightUpdate(GameTime gametime)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		PhaseBLegUpdate(gametime);
		if (phaseMax >= 0f && (BaseGame.Get().OnExactBeat(0) || BaseGame.Get().OnExactBeat(6) || BaseGame.Get().OnExactBeat(8) || BaseGame.Get().OnExactBeat(14)))
		{
			int num = r.Next(4);
			Vector3 val = Vector3.Zero;
			switch (num)
			{
			case 0:
				val = Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmA_1"]]] * Transformation());
				break;
			case 1:
				val = Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmB_1"]]] * Transformation());
				break;
			case 2:
				val = Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmC_1"]]] * Transformation());
				break;
			case 3:
				val = Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmD_1"]]] * Transformation());
				break;
			}
			BaseGame.Get().channels[13 + num] = 1f;
			Enemy enemy = new Note(val, new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f), this, 4);
			BaseGame.Get().enems.Add(enemy);
			enemy.start();
			phaseMax--;
		}
	}

	private bool PhaseBLeg1FightRemove(ConditionSet cs)
	{
		if (phaseMax <= 0.01f)
		{
			return BaseGame.Get().enems.Count == 2;
		}
		return false;
	}

	private EnemyState PhaseBLeg1FinishState()
	{
		if (bosshp[4] <= 0)
		{
			return PhaseBLeg1FinishDieState();
		}
		return PhaseBLeg1FinishDiveState();
	}

	private EnemyState PhaseBLeg1FinishDiveState()
	{
		EnemyState enemyState = new EnemyState(PhaseBLegUpdate, DrawLegs, AnimFinishedLegs, PhaseBLeg1FinishDive2State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		waterLevel = -40f;
		SetAnimLegs("legadive", loop: false);
		return enemyState;
	}

	private EnemyState PhaseBLeg1FinishDive2State()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg1FinishDive2Update, null, null, PhaseStartState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.200000047683716));
		enemyState.condSet.Start();
		phaseCountdown = 2.2f;
		phaseMax = 2.2f;
		return enemyState;
	}

	private void PhaseBLeg1FinishDive2Update(GameTime gametime)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		NormalUpdate(gametime);
		phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (phaseCountdown < 0f)
		{
			phaseCountdown = 0f;
		}
		BaseGame.Get().MovePlayerDir(Vector3.Transform(new Vector3(0f, 0f, 1f), Matrix.CreateRotationY((float)Math.PI * -5f / 14f * phaseCountdown / phaseMax)));
	}

	private EnemyState PhaseBLeg1FinishDieState()
	{
		float num = 6f;
		EnemyState enemyState = new EnemyState(PhaseBLeg1FinishDieUpdate, DrawLegs, null, PhaseBLeg1FinishDie2State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(num + 0.5f));
		enemyState.condSet.Start();
		phaseCountdown = (phaseMax = num);
		return enemyState;
	}

	private void PhaseBLeg1FinishDieUpdate(GameTime gametime)
	{
		phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (phaseCountdown < 0f)
		{
			phaseCountdown = 0f;
		}
		PhaseBLegUpdate(gametime);
		linePos = 480f * (phaseCountdown / phaseMax);
		for (int i = 13; i < 17; i++)
		{
			BaseGame.Get().channels[i] = (phaseMax - phaseCountdown) / phaseMax;
		}
	}

	private EnemyState PhaseBLeg1FinishDie2State()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg1FinishDie2Update, null, null, PhaseDetermineLegsGoneState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.200000047683716));
		enemyState.condSet.Start();
		phaseCountdown = 2.2f;
		phaseMax = 2.2f;
		activeLegs.Remove(3);
		return enemyState;
	}

	private void PhaseBLeg1FinishDie2Update(GameTime gametime)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		NormalUpdate(gametime);
		phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (phaseCountdown < 0f)
		{
			phaseCountdown = 0f;
		}
		BaseGame.Get().MovePlayerDir(Vector3.Transform(new Vector3(0f, 0f, 1f), Matrix.CreateRotationY((float)Math.PI * -5f / 14f * phaseCountdown / phaseMax)));
	}

	private EnemyState PhaseBLeg2State()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg2Update, DrawLegs, AnimFinishedLegs, PhaseBLeg2PreFightState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		drawMode = 3;
		SetAnimLegs("legbintro", loop: false);
		alphaAmount = 1f;
		legAlphaAmount = 1f;
		phaseCountdown = 0.2f;
		phaseMax = 0f;
		return enemyState;
	}

	private void PhaseBLeg2Update(GameTime gametime)
	{
		PhaseBLegUpdate(gametime);
		for (int i = 13; i < 17; i++)
		{
			BaseGame.Get().channels[i] = 1f;
		}
		if (phaseCountdown > 0f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			if (phaseCountdown <= 0f && phaseMax <= 0.1f)
			{
				phaseCountdown = 2.12f;
				phaseMax = 2.12f;
			}
			alphaAmount = phaseCountdown / phaseMax;
			legAlphaAmount = alphaAmount;
		}
	}

	private EnemyState PhaseBLeg2PreFightState()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg2PreFightUpdate, DrawLegs, null, PhaseBLeg2FightState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.200000047683716));
		enemyState.condSet.Start();
		phaseCountdown = 0f;
		phaseMax = 1.8f;
		planeAlphaAmount = 0f;
		curMesh = 0;
		curIndex = 0;
		curPart = 3;
		oluPlane.ResetIndicesToDraw();
		return enemyState;
	}

	private void PhaseBLeg2PreFightUpdate(GameTime gametime)
	{
		PhaseBLegUpdate(gametime);
		if (phaseCountdown < phaseMax)
		{
			phaseCountdown += (float)gametime.ElapsedGameTime.TotalSeconds;
			if (phaseCountdown >= phaseMax)
			{
				phaseCountdown = phaseMax;
			}
			planeAlphaAmount = phaseCountdown / phaseMax;
		}
	}

	private EnemyState PhaseBLeg2FightState()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg2FightUpdate, DrawLegs, PhaseBLeg2FightRemove, PhaseBLeg2FinishState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		phaseCountdown = 0f;
		phaseMax = 0.03f;
		return enemyState;
	}

	private void PhaseBLeg2FightUpdate(GameTime gametime)
	{
		PhaseBLegUpdate(gametime);
		phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (curMesh >= 0 && phaseCountdown <= 0f)
		{
			ShootTile();
			phaseCountdown += phaseMax;
		}
	}

	private bool PhaseBLeg2FightRemove(ConditionSet cs)
	{
		if (BaseGame.Get().enems.Count == 2)
		{
			return curMesh == -1;
		}
		return false;
	}

	private EnemyState PhaseBLeg2FinishState()
	{
		planeAlphaAmount = 0f;
		if (bosshp[3] <= 0)
		{
			return PhaseBLeg2FinishDieState();
		}
		return PhaseBLeg2FinishDiveState();
	}

	private EnemyState PhaseBLeg2FinishDiveState()
	{
		EnemyState enemyState = new EnemyState(PhaseBLegUpdate, DrawLegs, AnimFinishedLegs, PhaseStartState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		waterLevel = -40f;
		SetAnimLegs("legbdive", loop: false);
		return enemyState;
	}

	private EnemyState PhaseBLeg2FinishDieState()
	{
		float num = 6f;
		EnemyState enemyState = new EnemyState(PhaseBLeg2FinishDieUpdate, DrawLegs, null, PhaseDetermineLegsGoneState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(num + 0.5f));
		enemyState.condSet.Start();
		phaseCountdown = (phaseMax = num);
		return enemyState;
	}

	private void PhaseBLeg2FinishDieUpdate(GameTime gametime)
	{
		phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (phaseCountdown < 0f)
		{
			phaseCountdown = 0f;
		}
		PhaseBLegUpdate(gametime);
		linePos = 480f * (phaseCountdown / phaseMax);
		for (int i = 13; i < 17; i++)
		{
			BaseGame.Get().channels[i] = (phaseMax - phaseCountdown) / phaseMax;
		}
		activeLegs.Remove(2);
	}

	private EnemyState PhaseBLeg3State()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg3Update, DrawLegs, AnimFinishedLegs, PhaseBLeg3PreFightState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		drawMode = 2;
		SetAnimLegs("legcintro", loop: false);
		alphaAmount = 1f;
		legAlphaAmount = 1f;
		phaseCountdown = 2.6f;
		phaseMax = 2.6f;
		return enemyState;
	}

	private void PhaseBLeg3Update(GameTime gametime)
	{
		PhaseBLegUpdate(gametime);
		for (int i = 13; i < 17; i++)
		{
			BaseGame.Get().channels[i] = 1f;
		}
		if (phaseCountdown > 0f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			alphaAmount = phaseCountdown / phaseMax;
			legAlphaAmount = alphaAmount;
		}
	}

	private EnemyState PhaseBLeg3PreFightState()
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(PhaseBLeg3PreFightUpdate, PhaseBLeg3FightDraw, null, PhaseBLeg3FightState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(3.200000047683716));
		enemyState.condSet.Start();
		phaseCountdown = 0f;
		phaseMax = 3.2f;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				ref Vector3 reference = ref spiralBasePoints[i][j];
				reference = new Vector3(legBezier[i].pos[j].X, legBezier[i].pos[j].Y, legBezier[i].pos[j].Z);
			}
		}
		return enemyState;
	}

	private void PhaseBLeg3PreFightUpdate(GameTime gametime)
	{
		PhaseBLegUpdate(gametime);
		if (phaseCountdown < phaseMax)
		{
			phaseCountdown += (float)gametime.ElapsedGameTime.TotalSeconds;
			if (phaseCountdown >= phaseMax)
			{
				phaseCountdown = phaseMax;
			}
			legProgress = 10f * phaseCountdown / phaseMax;
		}
	}

	private EnemyState PhaseBLeg3FightState()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg3FightUpdate, PhaseBLeg3FightDraw, PhaseBLeg3FightRemove, PhaseBLeg3FinishState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		if (BaseGame.release)
		{
			phaseMax = 64f;
		}
		else
		{
			phaseMax = 2f;
		}
		phaseCountdown = 0.2f;
		return enemyState;
	}

	private void PhaseBLeg3FightUpdate(GameTime gametime)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		PhaseBLegUpdate(gametime);
		if (phaseMax >= 0f && (BaseGame.Get().OnExactBeat(0) || BaseGame.Get().OnExactBeat(6) || BaseGame.Get().OnExactBeat(8) || BaseGame.Get().OnExactBeat(14)))
		{
			int num = r.Next(4);
			int num2 = 0;
			Vector3 val = Vector3.Zero;
			switch (num)
			{
			case 0:
				num2 = 0;
				val = Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmA_1"]]] * Transformation());
				break;
			case 1:
				num2 = 2;
				val = Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmB_1"]]] * Transformation());
				break;
			case 2:
				num2 = 1;
				val = Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmC_1"]]] * Transformation());
				break;
			case 3:
				num2 = 3;
				val = Vector3.Transform(Vector3.Zero, oluLegs.transforms[legBones[oluLegs.model.Bones["Armature_ArmD_1"]]] * Transformation());
				break;
			}
			BaseGame.Get().channels[13 + num] = 1f;
			Enemy enemy = new Note(val, new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f), this, 2);
			BaseGame.Get().enems.Add(enemy);
			enemy.addPath(new PBezier(spiralBasePoints[num][0], spiralBasePoints[num][1], spiralBasePoints[num][2], spiralBasePoints[num][3], 1f, Vector3.Up, 0f, 0f, 1f, 19, 0f, 0.0, 0.0));
			for (int i = 0; i < spiralPoints[num2].Count && i < 4; i++)
			{
				enemy.addPath(new PBezier(spiralPoints[num2][i][0], spiralPoints[num2][i][1], spiralPoints[num2][i][2], spiralPoints[num2][i][3], 0.75f, Vector3.Up, 0f, 0f, 1f, 19, 0f, 0.0, 0.0));
			}
			Vector3 p = ((PBezier)enemy.pathList.publicPaths[enemy.pathList.publicPaths.Count - 1]).pos[3];
			enemy.addPath(new PBezier(p, BaseGame.Get().playerPos, 0.25f, Vector3.Up, 0f, 0f, 1f, 19, 0f, 0.0, 0.0));
			enemy.start();
			phaseMax--;
		}
	}

	private void PhaseBLeg3FightDraw(GameTime gametime)
	{
		DrawLegs(gametime);
		DrawPhase3Legs(gametime);
	}

	private bool PhaseBLeg3FightRemove(ConditionSet cs)
	{
		if (phaseMax <= 0.01f)
		{
			return BaseGame.Get().enems.Count == 2;
		}
		return false;
	}

	private EnemyState PhaseBLeg3FinishState()
	{
		if (bosshp[2] <= 0)
		{
			return PhaseBLeg3FinishDieState();
		}
		return PhaseBLeg3FinishDiveState();
	}

	private EnemyState PhaseBLeg3FinishDiveState()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg3FinishDiveUpdate, PhaseBLeg3FightDraw, null, PhaseBLeg3FinishDive2State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.200000047683716));
		enemyState.condSet.Start();
		phaseCountdown = 2.2f;
		phaseMax = 2.2f;
		return enemyState;
	}

	private void PhaseBLeg3FinishDiveUpdate(GameTime gametime)
	{
		PhaseBLegUpdate(gametime);
		phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (phaseCountdown < 0f)
		{
			phaseCountdown = 0f;
		}
		legProgress = 10f * phaseCountdown / phaseMax;
	}

	private EnemyState PhaseBLeg3FinishDive2State()
	{
		EnemyState enemyState = new EnemyState(PhaseBLegUpdate, DrawLegs, AnimFinishedLegs, PhaseStartState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		waterLevel = -40f;
		SetAnimLegs("legcdive", loop: false);
		return enemyState;
	}

	private EnemyState PhaseBLeg3FinishDieState()
	{
		float num = 6f;
		EnemyState enemyState = new EnemyState(PhaseBLeg3FinishDieUpdate, PhaseBLeg3FightDraw, null, PhaseBLeg3FinishDie2State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(num + 0.5f));
		enemyState.condSet.Start();
		phaseCountdown = (phaseMax = num);
		return enemyState;
	}

	private void PhaseBLeg3FinishDieUpdate(GameTime gametime)
	{
		phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (phaseCountdown < 0f)
		{
			phaseCountdown = 0f;
		}
		legProgress = 10f * phaseCountdown / phaseMax;
		PhaseBLegUpdate(gametime);
		linePos = 480f * (phaseCountdown / phaseMax);
		for (int i = 13; i < 17; i++)
		{
			BaseGame.Get().channels[i] = (phaseMax - phaseCountdown) / phaseMax;
		}
	}

	private EnemyState PhaseBLeg3FinishDie2State()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg3FinishDie2Update, null, null, PhaseDetermineLegsGoneState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.200000047683716));
		enemyState.condSet.Start();
		phaseCountdown = 2.2f;
		phaseMax = 2.2f;
		linePos = -10f;
		activeLegs.Remove(1);
		return enemyState;
	}

	private void PhaseBLeg3FinishDie2Update(GameTime gametime)
	{
		NormalUpdate(gametime);
		phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (phaseCountdown < 0f)
		{
			phaseCountdown = 0f;
		}
	}

	private EnemyState PhaseBLeg4State()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg4Update, DrawLegs, AnimFinishedLegs, PhaseBLeg4PreFightState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		drawMode = 1;
		SetAnimLegs("legdintro", loop: false);
		alphaAmount = 1f;
		legAlphaAmount = alphaAmount;
		waterLevel = -40f;
		phaseCountdown = 0.2f;
		phaseMax = 0f;
		return enemyState;
	}

	private void PhaseBLeg4Update(GameTime gametime)
	{
		PhaseBLegUpdate(gametime);
		for (int i = 13; i < 17; i++)
		{
			BaseGame.Get().channels[i] = 1f;
		}
		if (phaseCountdown > 0f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			if (phaseCountdown <= 0f && phaseMax <= 0.1f)
			{
				phaseCountdown = 2.12f;
				phaseMax = 2.12f;
			}
			alphaAmount = phaseCountdown / phaseMax;
			legAlphaAmount = alphaAmount;
		}
	}

	private EnemyState PhaseBLeg4PreFightState()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg4PreFightUpdate, DrawLegs, null, PhaseBLeg4FightState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.200000047683716));
		enemyState.condSet.Start();
		phaseCountdown = 0f;
		phaseMax = 1.8f;
		return enemyState;
	}

	private void PhaseBLeg4PreFightUpdate(GameTime gametime)
	{
		PhaseBLegUpdate(gametime);
		if (phaseCountdown < phaseMax)
		{
			phaseCountdown += (float)gametime.ElapsedGameTime.TotalSeconds;
			if (phaseCountdown >= phaseMax)
			{
				phaseCountdown = phaseMax;
			}
		}
	}

	private EnemyState PhaseBLeg4FightState()
	{
		EnemyState enemyState = new EnemyState(PhaseBLeg4FightUpdate, null, PhaseBLeg4FightRemove, PhaseBLeg4FinishState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		phaseCountdown = 0f;
		phaseMax = 8f;
		return enemyState;
	}

	private void PhaseBLeg4FightUpdate(GameTime gametime)
	{
		PhaseBLegUpdate(gametime);
		phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (phaseCountdown <= 0f && phaseMax > 0.1f)
		{
			Enemy enemy = new OluSnake(this, 1);
			CreatePhase4Path((OluSnake)enemy);
			BaseGame.Get().enems.Add(enemy);
			enemy.start();
			phaseMax--;
			phaseCountdown = 1.2f;
		}
	}

	private void CreatePhase4Path(Enemy n)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		int num2 = 6;
		Vector3[] array = (Vector3[])(object)new Vector3[num2 + 1];
		int num3 = r.Next(4);
		ref Vector3 reference = ref array[0];
		reference = BaseGame.GetRandPosSide(Vector3.Transform(new Vector3(-60f, -30f, 30f), Matrix.CreateRotationZ(MathHelper.ToRadians(90f * (float)num3))), Vector3.Transform(new Vector3(-60f, 30f, 90f), Matrix.CreateRotationZ(MathHelper.ToRadians(90f * (float)num3))));
		num3 = r.Next(4);
		ref Vector3 reference2 = ref array[num2];
		reference2 = BaseGame.GetRandPosSide(Vector3.Transform(new Vector3(-60f, -30f, 30f), Matrix.CreateRotationZ(MathHelper.ToRadians(90f * (float)num3))), Vector3.Transform(new Vector3(-60f, 30f, 90f), Matrix.CreateRotationZ(MathHelper.ToRadians(90f * (float)num3))));
		for (int i = 1; i < num2; i++)
		{
			ref Vector3 reference3 = ref array[i];
			reference3 = BaseGame.GetRandPosCube(new Vector3(-30f, -30f, 30f), new Vector3(30f, 30f, 90f));
		}
		Vector3[] array2 = (Vector3[])(object)new Vector3[num2 * 3 + 1];
		for (int j = 0; j <= num2; j++)
		{
			ref Vector3 reference4 = ref array2[j * 3];
			reference4 = array[j];
		}
		for (int k = 0; k < num2; k++)
		{
			Vector3 val = array2[(k + 1) * 3] - array2[k * 3];
			val /= 2f;
			Vector3 val2 = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(val), Vector3.Up));
			val2 *= ((Vector3)(ref val)).Length();
			val2 = Vector3.Transform(val2, Matrix.CreateFromAxisAngle(Vector3.Normalize(val), MathHelper.ToRadians(360f * (float)r.NextDouble())));
			ref Vector3 reference5 = ref array2[3 * k + 1];
			reference5 = array2[3 * k] + val + val2;
		}
		for (int l = 0; l < num2; l++)
		{
			ref Vector3 reference6 = ref array2[3 * l + 2];
			reference6 = (3f * array2[3 * l + 1] + 2f * array2[3 * l + 3]) / 5f;
			ref Vector3 reference7 = ref array2[3 * l + 1];
			reference7 = (3f * array2[3 * l + 1] + 2f * array2[3 * l]) / 5f;
		}
		for (int m = 1; m < num2; m++)
		{
			ref Vector3 reference8 = ref array2[m * 3];
			reference8 = (array2[m * 3 - 1] + array2[m * 3 + 1]) / 2f;
		}
		for (int num4 = 0; num4 < num2 - 1; num4++)
		{
			float num5 = num;
			Vector3 val3 = array2[(num4 + 1) * 3] - array2[num4 * 3];
			num = num5 + ((Vector3)(ref val3)).Length();
		}
		for (int num6 = 0; num6 < num2; num6++)
		{
			Vector3 p = array2[num6 * 3];
			Vector3 p2 = array2[num6 * 3 + 1];
			Vector3 p3 = array2[num6 * 3 + 2];
			Vector3 p4 = array2[num6 * 3 + 3];
			float num7 = 0.2f * num;
			Vector3 val4 = array[num6 + 1] - array[num6];
			n.addPath(new PBezier(p, p2, p3, p4, num7 / ((Vector3)(ref val4)).Length(), Vector3.Up, 0f, 0f, 1f, 19, 0f, 0.0, 0.0));
		}
		n.addPath(new PBezier(array2[num2 * 3], array2[num2 * 3] + new Vector3(0f, 0f, 5f), 0f, Vector3.Up, 0f, 0f, 1f, 19, 0f, 0.0, 0.0));
	}

	private bool PhaseBLeg4FightRemove(ConditionSet cs)
	{
		if (phaseMax < 0.1f)
		{
			return BaseGame.Get().enems.Count == 2;
		}
		return false;
	}

	private EnemyState PhaseBLeg4FinishState()
	{
		if (bosshp[1] <= 0)
		{
			return PhaseBLeg4FinishDieState();
		}
		return PhaseBLeg4FinishDiveState();
	}

	private EnemyState PhaseBLeg4FinishDiveState()
	{
		EnemyState enemyState = new EnemyState(PhaseBLegUpdate, DrawLegs, null, PhaseStartState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(3.200000047683716));
		enemyState.condSet.Start();
		waterLevel = -50f;
		return enemyState;
	}

	private EnemyState PhaseBLeg4FinishDieState()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, DrawLegs, null, PhaseDetermineLegsGoneState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(1.5));
		enemyState.condSet.Start();
		activeLegs.Remove(0);
		return enemyState;
	}

	private EnemyState PhaseDetermineLegsGoneState()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, null, null, PhaseDetermineLegsGone2State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(8.0));
		enemyState.condSet.Start();
		tentBossMusic.Clear();
		AddTentCue(0, "43Bass", 0, 4);
		AddTentCue(0, "43Drums", 0, 4);
		AddTentCue(0, "43Triangle", 0, 4);
		TextDisplay textDisplay = new TextDisplay(message[activeLegs.Count], 8f, _requireButton: false, 0.1f);
		BaseGame.Get().enems.Add(textDisplay);
		textDisplay.start();
		return enemyState;
	}

	private EnemyState PhaseDetermineLegsGone2State()
	{
		if (!BaseGame.release)
		{
			bosshp[1] = (bosshp[2] = (bosshp[3] = (bosshp[4] = -1)));
		}
		if (bosshp[1] <= 0 && bosshp[2] <= 0 && bosshp[3] <= 0 && bosshp[4] <= 0)
		{
			return PhaseTransitionToFinalPhaseState();
		}
		return PhaseStartState();
	}

	private EnemyState PhaseTransitionToFinalPhaseState()
	{
		EnemyState enemyState = new EnemyState(PhaseTransitionToFinalPhaseUpdate, null, AnimFinished, PhaseTransitionToFinalPhase2State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		resetMusic = true;
		tentBossMusic.Clear();
		AddTentCue(0, "44Transition", 0, 4);
		bosshp[1] = (bosshp[2] = (bosshp[3] = (bosshp[4] = 1)));
		SetAnimation("phaselast", loop: false);
		phaseCountdown = 0f;
		phaseMax = 3.5f;
		return enemyState;
	}

	private void PhaseTransitionToFinalPhaseUpdate(GameTime gametime)
	{
		PhaseBLegUpdate(gametime);
		drawLegParts = false;
		if (phaseCountdown <= phaseMax)
		{
			phaseCountdown += (float)gametime.ElapsedGameTime.TotalSeconds;
			if (phaseCountdown > phaseMax)
			{
				phaseCountdown = phaseMax;
			}
			alphaAmount = phaseCountdown / phaseMax;
			legAlphaAmount = alphaAmount;
		}
	}

	private EnemyState PhaseTransitionToFinalPhase2State()
	{
		EnemyState enemyState = new EnemyState(PhaseTransitionToFinalPhase2Update, KokoDraw, null, PhaseTransitionToFinalPhase3State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(3.5));
		enemyState.condSet.Start();
		tentBossMusic.Clear();
		AddTentCue(0, "45Bass", 0, 8);
		AddTentCue(0, "45ChordAmbience", 0, 8);
		AddTentCue(0, "45ChordRhythm", 0, 8);
		AddTentCue(0, "45Drums", 0, 8);
		AddTentCue(0, "45Lead", 0, 8);
		AddTentCue(0, "45Triangle", 0, 8);
		SetAnimKoko("dance", loop: true);
		phaseCountdown = (phaseMax = 3.5f);
		return enemyState;
	}

	private void PhaseTransitionToFinalPhase2Update(GameTime gametime)
	{
		PhaseBLegUpdate(gametime);
		KokoHairUpdate(gametime);
		if (phaseCountdown >= 0f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			if (phaseCountdown < 0f)
			{
				phaseCountdown = 0f;
			}
			alphaAmount = phaseCountdown / phaseMax;
			legAlphaAmount = alphaAmount;
		}
	}

	private EnemyState PhaseTransitionToFinalPhase3State()
	{
		EnemyState enemyState = new EnemyState(Phase3Update, KokoDraw, Phase3Remove, PreEndLevelState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		ShootTileKoko();
		return enemyState;
	}

	private void Phase3Update(GameTime gametime)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		NormalUpdate(gametime);
		LegPhaseBUpdate(gametime);
		KokoHairUpdate(gametime);
		if (BaseGame.Get().fillMode != fillMode)
		{
			return;
		}
		fillMode = (FillMode)(((int)fillMode == 3) ? 2 : 3);
		foreach (Target target in targets)
		{
			if (target.selected == 0)
			{
				target.fillMode = fillMode;
			}
		}
	}

	private bool Phase3Remove(ConditionSet cs)
	{
		return targets.Count == 0;
	}

	private EnemyState AnimTestState()
	{
		EnemyState enemyState = new EnemyState(AnimTestUpdate, null, null, AnimTestState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		return enemyState;
	}

	private void AnimTestUpdate(GameTime gametime)
	{
		NormalUpdate(gametime);
		if (BaseGame.Get().input.KeyPressed((Keys)85))
		{
			curAnim += oluAnim[0].Animations.Count + 1;
			curAnim %= oluAnim[0].Animations.Count;
			oluAC[0] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[0].Animations[curAnim], component: false);
			oluAC[1] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[1].Animations[curAnim], component: false);
			BaseGame.RunController(oluAnim[0], oluAC[0]);
			BaseGame.RunController(oluAnim[1], oluAC[1]);
		}
		if (BaseGame.Get().input.KeyPressed((Keys)68))
		{
			curAnim += oluAnim[0].Animations.Count - 1;
			curAnim %= oluAnim[0].Animations.Count;
			oluAC[0] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[0].Animations[curAnim], component: false);
			oluAC[1] = new AnimationController(BaseGame.Get().CoreGame, oluAnim[1].Animations[curAnim], component: false);
			BaseGame.RunController(oluAnim[0], oluAC[0]);
			BaseGame.RunController(oluAnim[1], oluAC[1]);
		}
	}

	private EnemyState PreEndLevelState()
	{
		EnemyState enemyState = new EnemyState(PreEndLevelUpdate, KokoDraw, null, EndLevelState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(5.0));
		enemyState.condSet.Start();
		phaseCountdown = 0f;
		phaseMax = 3f;
		return enemyState;
	}

	private void PreEndLevelUpdate(GameTime gametime)
	{
		Phase3Update(gametime);
		if (phaseCountdown < phaseMax)
		{
			phaseCountdown += (float)gametime.ElapsedGameTime.TotalSeconds;
			if (phaseCountdown >= phaseMax)
			{
				phaseCountdown = phaseMax;
			}
			finalTurnAmount = (float)Math.PI * (phaseCountdown / phaseMax);
		}
	}

	private EnemyState EndLevelState()
	{
		EnemyState enemyState = new EnemyState(Phase3Update, KokoDraw, null, EndLevelState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		for (int num = BaseGame.Get().enems.Count - 1; num >= 0; num--)
		{
			if (!(BaseGame.Get().enems[num] is Olu))
			{
				BaseGame.Get().enems[num].die();
			}
		}
		BaseGame.Get().actualEnem = 0;
		return enemyState;
	}

	private void EndLevelUpdate(GameTime gametime)
	{
	}
}
