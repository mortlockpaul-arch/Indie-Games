using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class Pine : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public static Dictionary<ModelBone, int> hornBones;

	public static ModelWrapper g_hornModel;

	public static ModelWrapper g_headModel;

	public static ModelWrapper g_legModel;

	public static ModelWrapper bodyModel;

	public static ModelWrapper tailModel;

	public static ModelWrapper glowModel;

	public ModelWrapper hornModel;

	public ModelWrapper headModel;

	public List<int>[] hornWireModel;

	public List<EnemyState> states;

	public ModelOluAnimator hornAnim;

	public ModelOluAnimator headAnim;

	public AnimationController hornAC;

	public AnimationController headAC;

	public PineSegment child;

	public int[] spikeHP;

	public bool[] spikeWire;

	public static int spikeStartHP = 8;

	public float size;

	public float origSize;

	public bool finalPhase;

	public float stateChange;

	public Vector3 velocity;

	public Vector3 facingDir;

	public Vector3 up;

	public Vector3 origPos;

	private bool firstState;

	private Vector4 partColor;

	public Vector3 Velocity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return velocity;
		}
	}

	public Vector3 offset
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return pos - origPos;
		}
	}

	public Pine()
	{
		state = 0;
		attackCooldown = 5f;
		hitPoints = 6 * spikeStartHP;
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
		states = new List<EnemyState>();
		spikeHP = new int[6];
		spikeWire = new bool[6];
		finalPhase = false;
	}

	public static void LoadModel()
	{
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		bodyModel = BaseGame.Get().models.GetModel("Content\\Pine\\PineBody");
		tailModel = BaseGame.Get().models.GetModel("Content\\Pine\\PineTail");
		g_headModel = BaseGame.Get().models.GetModel("Content\\Pine\\PineHead");
		g_hornModel = BaseGame.Get().models.GetModel("Content\\Pine\\PineHorns", copyData: true, copyEPC: false);
		g_legModel = BaseGame.Get().models.GetModel("Content\\Pine\\PineLeg");
		glowModel = BaseGame.Get().models.GetModel("Content\\Gift\\GiftDot", copyData: false, copyEPC: true);
		BaseGame.Get().LinkEffect(glowModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.SetAllEPCs(glowModel.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(glowModel.epc, "DiffuseColor", (object)new Vector3(1f, 1f, 1f));
		BaseGame.SetAllEPCs(glowModel.epc, "EmissiveColor", (object)new Vector3(1f, 1f, 1f));
		BaseGame.SetAllEPCs(glowModel.epc, "Alpha", 1f);
		BaseGame.Get().LinkEffect(bodyModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.Get().LinkEffect(tailModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.Get().LinkEffect(g_headModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.Get().LinkEffect(g_hornModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.Get().LinkEffect(g_legModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		BaseGame.SetAllEPCs(bodyModel.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(tailModel.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(g_headModel.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(g_hornModel.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(g_legModel.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(bodyModel.epc, "DirLight0Direction", (object)new Vector3(-0.5f, -0.5f, -1f));
		BaseGame.SetAllEPCs(tailModel.epc, "DirLight0Direction", (object)new Vector3(-0.5f, -0.5f, -1f));
		BaseGame.SetAllEPCs(g_headModel.epc, "DirLight0Direction", (object)new Vector3(-0.5f, -0.5f, -1f));
		BaseGame.SetAllEPCs(g_hornModel.epc, "DirLight0Direction", (object)new Vector3(-0.5f, -0.5f, -1f));
		BaseGame.SetAllEPCs(g_legModel.epc, "DirLight0Direction", (object)new Vector3(-0.5f, -0.5f, -1f));
		BaseGame.SetAllEPCs(bodyModel.epc, "TextureMix", BaseGame.T_MUL);
		BaseGame.SetAllEPCs(tailModel.epc, "TextureMix", BaseGame.T_MUL);
		BaseGame.SetAllEPCs(g_headModel.epc, "TextureMix", BaseGame.T_MUL);
		BaseGame.SetAllEPCs(g_hornModel.epc, "TextureMix", BaseGame.T_MUL);
		BaseGame.SetAllEPCs(g_legModel.epc, "TextureMix", BaseGame.T_MUL);
		hornBones = new Dictionary<ModelBone, int>();
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)g_hornModel.model.Bones).Count; i++)
		{
			if (!hornBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)g_hornModel.model.Bones)[i]))
			{
				hornBones.Add(((ReadOnlyCollection<ModelBone>)(object)g_hornModel.model.Bones)[i], i);
			}
		}
	}

	public Pine(Dictionary<string, string> attributes, XmlNode node)
		: this()
	{
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		size = LevelLoader.GetFloatFromAtt(attributes, "size", 10f);
	}

	public override Matrix Transformation()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		return BaseGame.MapObjectToSystem2(Vector3.Zero, facingDir, up) * Matrix.CreateScale(size) * Matrix.CreateTranslation(getPos());
	}

	public override void draw(GameTime gametime)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.Get().DrawModel(ref hornModel);
		BaseGame.Get().DrawModel(ref headModel);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().DrawModel(ref hornModel, clearEpc: false, disableAnim: false, ref hornWireModel);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		for (int i = 0; i < states.Count; i++)
		{
			states[i].Draw(gametime);
		}
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		int num = hitPoints;
		hitPoints += 5;
		base.hit(toHit);
		hitPoints -= 5;
		if (!(toHit.eTarget is BoneModelTarget))
		{
			return;
		}
		int id = ((BoneModelTarget)toHit.eTarget).id;
		spikeHP[id] = ((BoneModelTarget)toHit.eTarget).hp;
		if (!spikeWire[id])
		{
			hitPoints = num;
		}
		if (spikeHP[id] <= 0)
		{
			if (!spikeWire[id])
			{
				RemoveBone("Armature_002_Spike" + (id + 1), hornModel, hornWireModel);
				spikeWire[id] = true;
				addTarget(new Vector3(0f, (id == 2 || id == 3) ? 1.6f : 1.5f, 0f), 8, 5, ref hornModel, id, "Armature_002_Spike" + (id + 1), this, (FillMode)2);
			}
			else
			{
				RemoveBoneFromDrawList("Armature_002_Spike" + (id + 1), hornModel, hornWireModel);
			}
		}
	}

	public override void die()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Invalid comparison between Unknown and I4
		BaseGame.Get().ps.AddParticles(getPos(), Vector3.Forward * 100f, 2f, 180f, Vector3.Zero, 0f, 0.675f, 0.2f, 0.2f, new Vector4(1f, 1f, 1f, 1f), 1024, 0.0003f, ((int)fillMode == 2) ? 1 : 0);
		PineSegment pineSegment = child;
		while (child != null)
		{
			pineSegment = child.child;
			child.die();
			child = pineSegment;
		}
		base.die();
	}

	public override void act(GameTime gametime)
	{
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		if (!exists)
		{
			return;
		}
		((GameComponent)headAC).Update(gametime);
		((GameComponent)hornAC).Update(gametime);
		((GameComponent)headAnim).Update(gametime);
		((GameComponent)hornAnim).Update(gametime);
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
		if (child != null)
		{
			child.AddVel(new TimeMarker((float)gametime.ElapsedGameTime.TotalSeconds, velocity, facingDir, up));
		}
		pos += velocity * (float)gametime.ElapsedGameTime.TotalSeconds;
		bool flag = true;
		if (!finalPhase)
		{
			return;
		}
		for (int i = 0; i < spikeHP.Length; i++)
		{
			if (spikeHP[i] > 0)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			hitPoints = -100;
		}
	}

	private EnemyState StartState()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		EnemyState result = new EnemyState(StartUpdate, null, StartRemove, TurnState);
		if (child != null)
		{
			child.ClearVel();
			child.SetupPos(getPos(), facingDir, velocity, up, 0.02f);
		}
		return result;
	}

	private void StartUpdate(GameTime gametime)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (BaseGame.Get().r.NextDouble() < 0.005)
		{
			velocity = BaseGame.GetRandVect(Vector3.Up, 180f) * (float)BaseGame.Get().r.NextDouble() * 10f;
			velocity.Y = 0f;
		}
		if ((offset.Z > 30f && velocity.Z > 0f) || (offset.Z < -20f && velocity.Z < 0f))
		{
			ref Vector3 reference = ref velocity;
			reference.Z *= -1f;
		}
		else if ((offset.X > 30f && velocity.X > 0f) || (offset.X < -30f && velocity.X < 0f))
		{
			ref Vector3 reference2 = ref velocity;
			reference2.X *= -1f;
		}
	}

	private bool StartRemove(ConditionSet cs)
	{
		if (child == null)
		{
			return true;
		}
		return false;
	}

	private EnemyState TurnState()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(TurnUpdate, null, null, FaceState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.0));
		enemyState.condSet.Start();
		for (int i = 0; i < 6; i++)
		{
			spikeHP[i] = spikeStartHP * 2;
		}
		addTarget(new Vector3(0f, 1.5f, 0f), spikeHP[0], 5, ref hornModel, 0, "Armature_002_Spike1", this);
		addTarget(new Vector3(0f, 1.5f, 0f), spikeHP[1], 5, ref hornModel, 1, "Armature_002_Spike2", this);
		addTarget(new Vector3(0f, 1.6f, 0f), spikeHP[2], 5, ref hornModel, 2, "Armature_002_Spike3", this);
		addTarget(new Vector3(0f, 1.6f, 0f), spikeHP[3], 5, ref hornModel, 3, "Armature_002_Spike4", this);
		addTarget(new Vector3(0f, 1.5f, 0f), spikeHP[4], 5, ref hornModel, 4, "Armature_002_Spike5", this);
		addTarget(new Vector3(0f, 1.5f, 0f), spikeHP[5], 5, ref hornModel, 5, "Armature_002_Spike6", this);
		finalPhase = true;
		PineSegment pineSegment = child;
		if (firstState)
		{
			while (child != null)
			{
				pineSegment = child.child;
				child.die();
				child = pineSegment;
			}
		}
		origPos = pos;
		origPos.Z = 180f;
		ref Vector3 reference = ref origPos;
		reference.Y -= 20f;
		origPos.X = 0f;
		stateChange = 0f;
		velocity = -offset / 2f;
		return enemyState;
	}

	private void TurnUpdate(GameTime gametime)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		stateChange += (float)gametime.ElapsedGameTime.TotalSeconds;
		facingDir = Vector3.Transform(new Vector3(0f, 0f, 1f), Matrix.CreateRotationY(MathHelper.ToRadians(90f * stateChange)));
	}

	private EnemyState FaceState()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState;
		if (hitPoints <= 0)
		{
			enemyState = DieState();
		}
		else
		{
			enemyState = new EnemyState(FaceUpdate, null, BaseRemove, OpenMouthState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new TimeCondition(4.5));
			enemyState.condSet.Start();
			velocity = Vector3.Zero;
		}
		return enemyState;
	}

	private void FaceUpdate(GameTime gametime)
	{
	}

	private bool BaseRemove(ConditionSet cs)
	{
		if (cs != null)
		{
			if (!cs.ConditionsMet())
			{
				return hitPoints <= 0;
			}
			return true;
		}
		return hitPoints <= 0;
	}

	private EnemyState OpenMouthState()
	{
		EnemyState enemyState;
		if (hitPoints <= 0)
		{
			enemyState = DieState();
		}
		else
		{
			enemyState = new EnemyState(OpenMouthUpdate, OpenMouthDraw, BaseRemove, ShootState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new TimeCondition(3.5));
			enemyState.condSet.Start();
			headAC = new AnimationController(BaseGame.Get().CoreGame, headAnim.Animations["open"]);
			hornAC = new AnimationController(BaseGame.Get().CoreGame, hornAnim.Animations["open"]);
			BaseGame.RunController(headAnim, headAC);
			BaseGame.RunController(hornAnim, hornAC);
			headAC.IsLooping = false;
			hornAC.IsLooping = false;
			((GameComponent)headAC).Update(BaseGame.Get().emptytime);
			((GameComponent)hornAC).Update(BaseGame.Get().emptytime);
		}
		return enemyState;
	}

	private void OpenMouthUpdate(GameTime gametime)
	{
	}

	private void OpenMouthDraw(GameTime gametime)
	{
	}

	private EnemyState ShootState()
	{
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState;
		if (hitPoints <= 0)
		{
			enemyState = DieState();
		}
		else
		{
			enemyState = new EnemyState(ShootUpdate, ShootDraw, BaseRemove, CloseState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new TimeCondition(5.5));
			enemyState.condSet.Start();
			headAC = new AnimationController(BaseGame.Get().CoreGame, headAnim.Animations["openstill"]);
			hornAC = new AnimationController(BaseGame.Get().CoreGame, hornAnim.Animations["openstill"]);
			BaseGame.RunController(headAnim, headAC);
			BaseGame.RunController(hornAnim, hornAC);
			((GameComponent)headAC).Update(BaseGame.Get().emptytime);
			((GameComponent)hornAC).Update(BaseGame.Get().emptytime);
			stateChange = 0f;
			PineSegment pineSegment = child;
			if (firstState)
			{
				while (child != null)
				{
					pineSegment = child.child;
					child.die();
					child = pineSegment;
				}
				velocity = Vector3.Zero;
				pos = new Vector3(0f, pos.Y - 20f, 180f);
				facingDir = new Vector3(0f, 0f, -1f);
			}
			BaseGame.Get().skyPS.AddBackwardsBurstParticles(Vector3.Transform(Vector3.Zero, Matrix.CreateTranslation(new Vector3(0f, 0.85f, 0f)) * Transformation()), new Vector3(0f, 0f, 250f), 0.3f, 180f, 0.4f, 0.03f, new Vector4(1f, 1f, 1f, 1f), 1600, 0.0006f, 0.05f);
			BaseGame.Get().PlayCue("HGShootCrash");
		}
		return enemyState;
	}

	private void ShootUpdate(GameTime gametime)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (stateChange >= -0.1f)
		{
			stateChange += (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		if (stateChange >= 2f)
		{
			for (int i = 0; i < 4; i++)
			{
				Enemy enemy = new BulletGlow(Vector3.Transform(Vector3.Zero, Matrix.CreateScale(1.2f) * Matrix.CreateTranslation(new Vector3(0f, 0.85f, 0f)) * Transformation()), 0.8f * size, 4);
				enemy.start();
				BaseGame.Get().enems.Add(enemy);
			}
			stateChange = -1f;
		}
	}

	private void ShootDraw(GameTime gametime)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (stateChange > 0f)
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(stateChange * 0.6f) * Matrix.CreateTranslation(new Vector3(0f, 0.85f, 0f)) * Transformation());
			BaseGame.Get().DrawModel(ref glowModel, clearEpc: true);
			BaseGame.Get().matStack.PopMatrix();
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		}
	}

	private EnemyState CloseState()
	{
		EnemyState enemyState;
		if (hitPoints <= 0)
		{
			enemyState = DieState();
		}
		else
		{
			enemyState = new EnemyState(CloseUpdate, CloseDraw, BaseRemove, FaceState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new TimeCondition(3.0));
			enemyState.condSet.Start();
			headAC = new AnimationController(BaseGame.Get().CoreGame, headAnim.Animations["close"]);
			hornAC = new AnimationController(BaseGame.Get().CoreGame, hornAnim.Animations["close"]);
			BaseGame.RunController(headAnim, headAC);
			BaseGame.RunController(hornAnim, hornAC);
			headAC.IsLooping = false;
			hornAC.IsLooping = false;
			((GameComponent)headAC).Update(BaseGame.Get().emptytime);
			((GameComponent)hornAC).Update(BaseGame.Get().emptytime);
		}
		return enemyState;
	}

	private void CloseUpdate(GameTime gametime)
	{
	}

	private void CloseDraw(GameTime gametime)
	{
	}

	private bool CloseRemove(ConditionSet cs)
	{
		return false;
	}

	private EnemyState DieState()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(DieUpdate, DieDraw, null, EndEnemyState);
		stateChange = 0f;
		origSize = size;
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(3.0));
		enemyState.condSet.Start();
		BaseGame.Get().skyPS.AddBackwardsBurstParticles(Vector3.Transform(Vector3.Zero, Transformation()), new Vector3(0f, 0f, 380f), 0.3f, 180f, 0.4f, 0.03f, new Vector4(1f, 1f, 1f, 1f), 2000, 0.0006f, 0.05f);
		return enemyState;
	}

	private void DieUpdate(GameTime gametime)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (stateChange <= 2f)
		{
			stateChange += (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		facingDir = Vector3.Lerp(new Vector3(0f, 0f, -1f), new Vector3(-0.2f, 1f, 0f), stateChange / 2f);
		size = (1f - stateChange / 2f) * origSize;
	}

	private void DieDraw(GameTime gametime)
	{
	}

	private EnemyState EndEnemyState()
	{
		EnemyState enemyState = new EnemyState(EndEnemyUpdate, null, null, null);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		die();
		return enemyState;
	}

	private void EndEnemyUpdate(GameTime gametime)
	{
	}

	public override void start()
	{
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Invalid comparison between Unknown and I4
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		addCond(new NeverCondition());
		base.start();
		headModel = new ModelWrapper(g_headModel);
		hornModel = new ModelWrapper(g_hornModel);
		headAnim = new ModelOluAnimator(BaseGame.Get().CoreGame, headModel, BaseGame.GetFogEffect());
		hornAnim = new ModelOluAnimator(BaseGame.Get().CoreGame, hornModel, BaseGame.GetFogEffect());
		headAC = new AnimationController(BaseGame.Get().CoreGame, headAnim.Animations["closestill"]);
		hornAC = new AnimationController(BaseGame.Get().CoreGame, hornAnim.Animations["closestill"]);
		BaseGame.RunController(headAnim, headAC);
		BaseGame.RunController(hornAnim, hornAC);
		velocity = new Vector3(0f, 0f, 5f);
		facingDir = Vector3.Normalize(velocity);
		setPos(pathList.curLocation());
		up = new Vector3(0f, 1f, 0f);
		origPos = getPos();
		for (int i = 0; i < spikeWire.Length; i++)
		{
			spikeWire[i] = false;
		}
		hornWireModel = new List<int>[((ReadOnlyCollection<ModelMesh>)(object)hornModel.model.Meshes).Count];
		for (int j = 0; j < ((ReadOnlyCollection<ModelMesh>)(object)hornModel.model.Meshes).Count; j++)
		{
			hornWireModel[j] = new List<int>();
		}
		child = new PineSegment(this, null, 8, getPos() + Vector3.Normalize(-facingDir) * size * 2f, velocity, facingDir, up, 0.1f);
		BaseGame.Get().enems.Add(child);
		child.start();
		firstState = true;
		states.Add(StartState());
		child.AddVel(new TimeMarker(8f, new Vector3(0f, 0f, -37.5f), facingDir, up));
		firstState = false;
		if ((int)fillMode == 2)
		{
			partColor = new Vector4(1f, 0.65f, 0f, 1f);
		}
		else
		{
			partColor = new Vector4(0.2f, 0.1f, 0f, 1f);
		}
	}

	public void SegmentHit()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ref Vector3 reference = ref origPos;
		reference.Z -= Vector3.Normalize(facingDir).Z * size;
		velocity.Z = -50f;
	}

	public override Vector3 getPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return pos;
	}

	public override string name()
	{
		return "[p1ne]";
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

	private void RemoveBone(string bonename, ModelWrapper model, List<int>[] wireModel)
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
						SplitList(model, wireModel, i, j);
						k = 3;
					}
				}
			}
		}
	}

	private void RemoveBoneFromDrawList(string bonename, ModelWrapper model, List<int>[] wireModel)
	{
		int num = model.boneNames[bonename][1];
		for (int i = 0; i < wireModel.Length; i++)
		{
			for (int num2 = wireModel[i].Count - 2; num2 >= 0; num2 -= 2)
			{
				int num3 = wireModel[i][num2 + 1] - 2;
				while (num3 > 0 && num3 >= wireModel[i][num2])
				{
					for (int j = 0; j < 3; j++)
					{
						if (num == model.vertices[i][model.indices[i][num3 + j]].boneNum(0))
						{
							RawSplitList(wireModel, i, num3);
							num3 = -1;
							num2 = wireModel[i].Count;
							j = 3;
						}
					}
					num3 -= 3;
				}
			}
		}
	}

	private void RawSplitList(List<int>[] toDraw, int _mesh, int _index)
	{
		bool flag = false;
		for (int i = 0; i < toDraw[_mesh].Count - 1; i += 2)
		{
			if (flag)
			{
				break;
			}
			if (_index < toDraw[_mesh][i] || _index > toDraw[_mesh][i + 1])
			{
				continue;
			}
			flag = true;
			if (_index == toDraw[_mesh][i])
			{
				if (_index == toDraw[_mesh][i + 1] - 2)
				{
					toDraw[_mesh].RemoveAt(i);
					toDraw[_mesh].RemoveAt(i);
				}
				else
				{
					toDraw[_mesh][i] = _index + 3;
				}
			}
			else if (_index == toDraw[_mesh][i + 1] - 2)
			{
				toDraw[_mesh][i + 1] = _index - 1;
			}
			else
			{
				toDraw[_mesh].Insert(i + 1, _index + 3);
				toDraw[_mesh].Insert(i + 1, _index - 1);
			}
		}
	}

	private void SplitList(ModelWrapper model, List<int>[] wireModel, int _mesh, int _index)
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
		flag = false;
		if (wireModel == null)
		{
			return;
		}
		for (int j = 0; j < wireModel[_mesh].Count - 1; j += 2)
		{
			if (flag)
			{
				break;
			}
			if (_index > wireModel[_mesh][j + 1] + 3)
			{
				continue;
			}
			flag = true;
			if (_index + 3 == wireModel[_mesh][j])
			{
				wireModel[_mesh][j] = _index;
			}
			else if (_index < wireModel[_mesh][j])
			{
				wireModel[_mesh].Insert(j, _index + 2);
				wireModel[_mesh].Insert(j, _index);
			}
			else if (_index == wireModel[_mesh][j + 1] + 1)
			{
				if (j < wireModel[_mesh].Count - 2 && _index + 3 == wireModel[_mesh][j + 2])
				{
					wireModel[_mesh].RemoveAt(j + 1);
					wireModel[_mesh].RemoveAt(j + 1);
				}
				else
				{
					wireModel[_mesh][j + 1] = _index + 2;
				}
			}
		}
		if (!flag)
		{
			wireModel[_mesh].Add(_index);
			wireModel[_mesh].Add(_index + 2);
		}
	}
}
