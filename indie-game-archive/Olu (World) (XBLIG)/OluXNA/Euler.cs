using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class Euler : Enemy
{
	private static Dictionary<ModelBone, int> eulerBones;

	public static ModelWrapper model;

	public static ModelWrapper faceModel;

	public static ModelWrapper transformModel;

	public List<int>[] wireModel;

	public float transformAmount;

	public float transformRate;

	public static ModelWrapper dot;

	public static VertexDeclaration vertDec;

	public static float size = 15f;

	public ModelOluAnimator anim;

	public ModelOluAnimator introAnim;

	public AnimationController spawn;

	public AnimationController walk;

	public AnimationController walk2;

	public List<EnemyState> states;

	public List<MusicPart> bossMusic;

	public BezierHelper[] mainBezier;

	private Matrix _transformation;

	private bool dirty;

	public int curMesh;

	public int curIndex;

	public PlaneDetachColl pdColl;

	private bool[] loaded;

	public int[] bosshp;

	public float particleCooldown;

	public float particleMax;

	public float phaseCountdown;

	public float phaseCountdown2;

	public int phaseLoop;

	public Random r;

	public static PlaneEffect[] pE;

	public List<RippleEffect> rE;

	public List<int> rEIndex;

	public List<Vector3> ripplePos;

	public List<int> boneTree;

	public float phaseExitLimit;

	public bool falling;

	public bool leftArmExists;

	public bool rightArmExists;

	public bool legsExist;

	public bool firstHitUpdate;

	public bool drawEyes;

	public bool drawFace;

	public bool rotateRipple;

	public Vector3[] tentacleWobbles;

	public Vector3[] tentacleVelocity;

	public Vector3 oluPos;

	protected Vector3 tempPos;

	public Euler()
	{
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		transformRate = 0.1f;
		base._002Ector();
		state = 0;
		states = new List<EnemyState>();
		bossMusic = new List<MusicPart>();
		loaded = new bool[20];
		bosshp = new int[20];
		bosshp[0] = 144;
		bosshp[1] = 144;
		bosshp[2] = 180;
		bosshp[3] = 180;
		r = new Random();
		for (int i = 0; i < 20; i++)
		{
			loaded[i] = false;
		}
		attackCooldown = 5f;
		particleMax = 0.02f;
		hitPoints = 14440;
		leftArmExists = true;
		rightArmExists = true;
		rotateRipple = false;
		drawEyes = false;
		drawFace = false;
		dirty = true;
		rE = new List<RippleEffect>();
		rEIndex = new List<int>();
		ripplePos = new List<Vector3>();
		tentacleWobbles = (Vector3[])(object)new Vector3[8];
		tentacleVelocity = (Vector3[])(object)new Vector3[8];
		oluPos = new Vector3(0f, 4f, 0f);
	}

	public static void LoadModel()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		VertexElement[] array = (VertexElement[])(object)new VertexElement[7]
		{
			new VertexElement((short)0, (short)0, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)0, (byte)0),
			new VertexElement((short)0, (short)12, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)0, (byte)1),
			new VertexElement((short)0, (short)24, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)3, (byte)0),
			new VertexElement((short)0, (short)36, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)3, (byte)1),
			new VertexElement((short)0, (short)48, (VertexElementFormat)1, (VertexElementMethod)0, (VertexElementUsage)5, (byte)0),
			new VertexElement((short)0, (short)56, (VertexElementFormat)5, (VertexElementMethod)0, (VertexElementUsage)2, (byte)0),
			new VertexElement((short)0, (short)60, (VertexElementFormat)4, (VertexElementMethod)0, (VertexElementUsage)1, (byte)0)
		};
		vertDec = new VertexDeclaration(BaseGame.Get().graphics.GraphicsDevice, array);
		model = BaseGame.Get().models.GetModel("Content\\Euler\\Euler", copyData: true, copyEPC: false);
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(model.epc, "DirLight0Direction", Vector3.Normalize(new Vector3(-1f, -0.5f, -0.5f)));
		BaseGame.SetAllEPCs(model.epc, "TextureMix", BaseGame.T_MUL);
		faceModel = BaseGame.Get().models.GetModel("Content\\Euler\\EulerPlane", copyData: true, copyEPC: false);
		BaseGame.SetAllEPCs(faceModel.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(faceModel.epc, "DirLight0Direction", Vector3.Normalize(new Vector3(-1f, -0.5f, -0.5f)));
		dot = BaseGame.Get().models.GetModel("Content\\Euler\\Dot", copyData: false, copyEPC: true);
		BaseGame.SetAllEPCs(dot.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(dot.epc, "DiffuseColor", (object)new Vector3(0f, 1f, 0f));
		eulerBones = new Dictionary<ModelBone, int>();
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)model.model.Bones).Count; i++)
		{
			if (!eulerBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i]))
			{
				eulerBones.Add(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i], i);
			}
		}
		pE = new PlaneEffect[12];
		for (int j = 0; j < pE.Length; j++)
		{
			pE[j] = new PlaneEffect();
			for (int k = 0; k < 14; k++)
			{
				TreeNode treeNode = new TreeNode((float)BaseGame.Get().r.NextDouble(), 0f, 0f, 1, 0.006f, 0.002f, 0.05f, 0.025f);
				treeNode.branchTree = false;
				treeNode.setColor(Color.Blue);
				pE[j].addNode(treeNode);
			}
			ref Vector3 reference = ref pE[j].cornerNodes[0];
			reference = new Vector3(-0.5f, 0f, 1.25f);
			ref Vector3 reference2 = ref pE[j].cornerNodes[1];
			reference2 = new Vector3(0.5f, 0f, 1.25f);
			ref Vector3 reference3 = ref pE[j].cornerNodes[2];
			reference3 = new Vector3(-0.5f, 0f, -1.25f);
			ref Vector3 reference4 = ref pE[j].cornerNodes[3];
			reference4 = new Vector3(0.5f, 0f, -1.25f);
			pE[j].iteratePlane();
			pE[j].FinalizeEffect(centerTransform: true);
		}
	}

	public Euler(Dictionary<string, string> attributes, XmlNode node)
		: this()
	{
	}

	public override Matrix Transformation()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (dirty)
		{
			_transformation = Matrix.CreateScale(new Vector3(size, size, 0f - size)) * Matrix.CreateTranslation(getPos());
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
		pdColl.act(gametime);
		for (int num2 = rE.Count - 1; num2 >= 0; num2--)
		{
			rE[num2].Update(gametime);
			if (rE[num2].done)
			{
				rE.RemoveAt(num2);
				rEIndex.RemoveAt(num2);
				ripplePos.RemoveAt(num2);
			}
		}
		dirty = true;
	}

	public override void draw(GameTime gametime)
	{
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		if (exists)
		{
			foreach (EnemyState state in states)
			{
				state.Draw(gametime);
			}
		}
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		for (int num = rE.Count - 1; num >= 0; num--)
		{
			if (!rE[num].done)
			{
				BaseGame.Get().matStack.PushMatrix();
				BaseGame.Get().matStack.ApplyMatrix((rotateRipple ? Matrix.Identity : Matrix.CreateRotationZ(MathHelper.ToRadians(-90f))) * Matrix.CreateScale(150f) * Matrix.CreateTranslation(ripplePos[num]) * Matrix.CreateTranslation(getPos()));
				rE[num].Draw(gametime);
				BaseGame.Get().fogEffect.Begin();
				BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
				pE[rEIndex[num]].draw();
				BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
				BaseGame.Get().fogEffect.End();
				BaseGame.Get().matStack.PopMatrix();
			}
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().SwitchEffectTechnique("Textured");
	}

	public override void start()
	{
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		r = new Random();
		base.start();
		model.ResetIndicesToDraw();
		anim = new ModelOluAnimator(BaseGame.Get().CoreGame, model, BaseGame.GetFogEffect());
		walk = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["walk"]);
		walk.SpeedFactor = 0.5;
		((GameComponent)walk).Update(BaseGame.Get().emptytime);
		BaseGame.RunController(anim, walk);
		((GameComponent)anim).Update((GameTime)null);
		((GameComponent)walk).Enabled = false;
		((GameComponent)anim).Enabled = false;
		walk2 = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["hitright"]);
		walk2.SpeedFactor = 0.5;
		((GameComponent)walk2).Update(BaseGame.Get().emptytime);
		((GameComponent)walk2).Enabled = false;
		transformModel = ModelWrapper.CombineModels(faceModel, model);
		for (int i = 0; i < transformModel.vertDec.Length; i++)
		{
			transformModel.vertDec[i] = vertDec;
		}
		wireModel = new List<int>[((ReadOnlyCollection<ModelMesh>)(object)model.model.Meshes).Count];
		for (int j = 0; j < ((ReadOnlyCollection<ModelMesh>)(object)model.model.Meshes).Count; j++)
		{
			wireModel[j] = new List<int>();
		}
		boneTree = new List<int>();
		curMesh = 0;
		curIndex = 0;
		AddLimbsToTree(eulerBones[model.model.Bones["Armature_RightArm01"]], boneTree, model);
		SetupTiles(boneTree);
		boneTree.Clear();
		int count = targets.Count;
		curMesh = 0;
		curIndex = 0;
		AddLimbsToTree(eulerBones[model.model.Bones["Armature_LeftArm01"]], boneTree, model);
		SetupTiles(boneTree);
		for (int k = count; k < targets.Count; k++)
		{
			((FaceTarget)targets[k]).bossPart = 1;
		}
		pdColl = new PlaneDetachColl(ref model);
		introAnim = new ModelOluAnimator(BaseGame.Get().CoreGame, transformModel, BaseGame.GetFogEffect());
		spawn = new AnimationController(BaseGame.Get().CoreGame, introAnim.Animations["spawn"]);
		spawn.SpeedFactor = 0.5;
		((GameComponent)spawn).Update(BaseGame.Get().emptytime);
		BaseGame.RunController(introAnim, spawn);
		((GameComponent)introAnim).Update((GameTime)null);
		((GameComponent)spawn).Enabled = false;
		states = new List<EnemyState>();
		pos = new Vector3(0f, -30f, 65f);
		if (BaseGame.quickload)
		{
			((GameComponent)spawn).Enabled = false;
			((GameComponent)introAnim).Enabled = false;
			((GameComponent)anim).Enabled = true;
			((GameComponent)walk).Enabled = true;
			((GameComponent)walk2).Enabled = true;
			ShootTiles(0);
			ShootTiles(1);
			SetupLegsState();
			setPos(getPos() + new Vector3(0f, 0f, 35f));
			ShootTiles(2);
			targets.Clear();
			states.Add(SmashFallState());
		}
		else
		{
			states.Add(PreIntroLevelState());
		}
		addCond(new NeverCondition());
	}

	public void AddCue(int beat, string name, int playMeas, int loopMeas)
	{
		bossMusic.Add(new MusicPart(beat, name, playMeas, loopMeas));
	}

	public override Vector3 getPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return pos;
	}

	public override string name()
	{
		return "[euler]";
	}

	public override bool Check(int numEnem)
	{
		return true;
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		FaceTarget faceTarget = (FaceTarget)toHit.eTarget;
		bosshp[faceTarget.bossPart]--;
		if (toHit.fillMode != faceTarget.fillMode)
		{
			bosshp[faceTarget.bossPart]--;
		}
		SplitList(((FaceTarget)toHit.eTarget).meshNum, ((FaceTarget)toHit.eTarget).indexNum);
		pdColl.AddPlane(ref model, ((FaceTarget)toHit.eTarget).meshNum, ((FaceTarget)toHit.eTarget).indexNum, this, (FillMode)3);
		base.hit(toHit);
	}

	public override void HitSound(int lockNum, float volume)
	{
		if (lockNum <= 0)
		{
			BaseGame.Get().PlayCue("", 0f);
		}
	}

	public void NormalUpdate(GameTime gametime)
	{
		if (spawn != null && ((GameComponent)spawn).Enabled)
		{
			((GameComponent)spawn).Update(gametime);
		}
		if (walk != null && ((GameComponent)walk).Enabled)
		{
			((GameComponent)walk).Update(gametime);
		}
		if (walk2 != null && ((GameComponent)walk2).Enabled)
		{
			((GameComponent)walk2).Update(gametime);
		}
		if (((GameComponent)anim).Enabled)
		{
			((GameComponent)anim).Update(gametime);
		}
		if (((GameComponent)introAnim).Enabled)
		{
			((GameComponent)introAnim).Update(gametime);
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
	}

	public void NormalDraw(GameTime gametime)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.Get().DrawModel(ref model, clearEpc: false, disableAnim: false, ref model.indicesToDraw, vertDec);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().DrawModel(ref model, clearEpc: false, disableAnim: false, ref wireModel);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		pdColl.draw(gametime);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
	}

	public void IntroDraw(GameTime gametime)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Morph");
		BaseGame.Get().fogEffect.Parameters["MorphProgress"].SetValue(transformAmount);
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.Get().DrawModelWithOtherBuffer(ref transformModel, clearEpc: false, disableAnim: false, ref model.indicesToDraw, vertDec);
		BaseGame.Get().matStack.PopMatrix();
	}

	private EnemyState PreIntroLevelState()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(PreIntroUpdate, IntroDraw, null, IntroLevelState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(3.0));
		enemyState.condSet.Start();
		phaseCountdown = 2.5f;
		tempPos = getPos();
		setPos(tempPos + new Vector3(0f, -100f, 0f));
		return enemyState;
	}

	public void PreIntroUpdate(GameTime gametime)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		NormalUpdate(gametime);
		if (phaseCountdown > 0f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			setPos(tempPos + new Vector3(0f, -200f, 0f) * phaseCountdown / 2.5f);
		}
	}

	private EnemyState IntroLevelState()
	{
		EnemyState enemyState = new EnemyState(IntroUpdate, IntroDraw, null, RightWalkState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(0.75f / transformRate + 3.1f));
		enemyState.condSet.Start();
		phaseCountdown = 0f;
		return enemyState;
	}

	public void IntroUpdate(GameTime gametime)
	{
		if (transformAmount < 1f)
		{
			transformAmount += (float)gametime.ElapsedGameTime.TotalSeconds * transformRate;
		}
		if (transformAmount > 0.75f && !((GameComponent)spawn).Enabled)
		{
			((GameComponent)spawn).Enabled = true;
		}
		NormalUpdate(gametime);
	}

	private EnemyState RightWalkState()
	{
		walk.SpeedFactor = 1.0;
		walk2.SpeedFactor = 1.0;
		EnemyState enemyState;
		if (!leftArmExists && !rightArmExists)
		{
			enemyState = new EnemyState(null, null, null, SetupLegsState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new AlwaysCondition());
			enemyState.condSet.Start();
		}
		else
		{
			enemyState = new EnemyState(RightWalkUpdate, NormalDraw, RightWalkRemove, HitRightState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new NeverCondition());
			enemyState.condSet.Start();
			((GameComponent)spawn).Enabled = false;
			((GameComponent)introAnim).Enabled = false;
			((GameComponent)anim).Enabled = true;
			((GameComponent)walk).Enabled = true;
			((GameComponent)walk2).Enabled = true;
			phaseCountdown = 0.25f;
			phaseLoop = 3;
			rotateRipple = false;
			firstHitUpdate = true;
			BaseGame.RunController(anim, walk);
		}
		return enemyState;
	}

	private void RightWalkUpdate(GameTime gametime)
	{
		if (phaseCountdown >= 0f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		if (phaseCountdown <= 0f && (float)walk.ElapsedTime / (float)walk.Duration < 0.1f)
		{
			phaseLoop--;
			phaseCountdown = 0.75f;
		}
		NormalUpdate(gametime);
	}

	private void RightFallUpdate(GameTime gametime)
	{
		if (phaseCountdown >= 0f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		if (phaseCountdown <= 0f && (float)walk2.ElapsedTime / (float)walk2.Duration < 0.1f)
		{
			phaseLoop--;
			phaseCountdown = 0.75f;
		}
		NormalUpdate(gametime);
	}

	private bool RightWalkRemove(ConditionSet cs)
	{
		if (phaseLoop <= 0)
		{
			return true;
		}
		return false;
	}

	private EnemyState HitRightState()
	{
		EnemyState enemyState;
		if (bosshp[0] > 10)
		{
			enemyState = new EnemyState(RightUpdate, NormalDraw, RightWalkRemove, LeftWalkState);
			walk2 = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["hitright"]);
			falling = false;
		}
		else
		{
			enemyState = new EnemyState(RightUpdate, NormalDraw, RightWalkRemove, LeftWalkState);
			walk2 = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["fallright"]);
			falling = true;
		}
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		walk2.ElapsedTime = walk.ElapsedTime * walk.Duration / walk2.Duration;
		((GameComponent)walk2).Update(BaseGame.Get().emptytime);
		((GameComponent)walk2).Enabled = true;
		phaseCountdown = 1.25f;
		phaseCountdown2 = 0.25f;
		phaseLoop = 1;
		BaseGame.RunController(anim, walk2);
		return enemyState;
	}

	private void RightUpdate(GameTime gametime)
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		RightFallUpdate(gametime);
		float num = (float)walk2.ElapsedTime / (float)walk2.Duration - (falling ? 0.25f : 0.5f);
		if (phaseCountdown2 <= 0f && num > 0f && num < 0.1f)
		{
			if (falling)
			{
				ripplePos.Add(size * new Vector3(-4f, 0f, -15f + num * 100f));
			}
			else
			{
				ripplePos.Add(size * new Vector3(-4f, 0f, -15f));
				if (firstHitUpdate)
				{
					PathList pathList = new PathList();
					List<IPath> list = new List<IPath>();
					Vector3 val = size * new Vector3(-8f, 5f, 0f) + getPos();
					list.Add(new PBezier(val, val + new Vector3(50f, 0f, 0f), 2.5f, Vector3.Up, 0f, 0f, 1f, 0, 0f, 0.0, 0.0));
					list.Add(new PBezier(val + new Vector3(50f, 0f, 0f), val + new Vector3(60f, 0f, 0f), new Vector3(0f, 0f, 18f), Vector3.Zero, 0.15f, Vector3.Up, 0f, 0f, 1f, 0, 0f, 0.0, 0.0));
					pathList.addPathComboList(list, new PRefLine(Vector3.Zero, Vector3.One, 0f, BaseGame.Get().playerPos));
					GiftFlock giftFlock = new GiftFlock(1f, 2, 8, 20f, 5f, 4f, (FillMode)2, pathList);
					giftFlock.start();
					BaseGame.Get().enems.Add(giftFlock);
					firstHitUpdate = false;
				}
			}
			if (rightArmExists && falling)
			{
				ShootTiles(0);
				rightArmExists = false;
			}
			rE.Add(new RippleEffect(ripplePos[ripplePos.Count - 1], 0.5f, 0.5f, 0f, 1.1f, 10f, _loop: false, 0f));
			rE[rE.Count - 1].fxUpdate = BaseGame.GetFogEffect().Parameters;
			rEIndex.Add(r.Next() % pE.Length);
			phaseCountdown2 = 0.012f;
		}
		else if (phaseCountdown2 < 40f)
		{
			phaseCountdown2 -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
	}

	private EnemyState LeftWalkState()
	{
		EnemyState enemyState;
		if (!leftArmExists && !rightArmExists)
		{
			enemyState = new EnemyState(null, null, null, SetupLegsState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new AlwaysCondition());
			enemyState.condSet.Start();
		}
		else
		{
			enemyState = new EnemyState(LeftWalkUpdate, NormalDraw, LeftWalkRemove, HitLeftState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new NeverCondition());
			enemyState.condSet.Start();
			((GameComponent)spawn).Enabled = false;
			((GameComponent)introAnim).Enabled = false;
			((GameComponent)anim).Enabled = true;
			((GameComponent)walk).Enabled = true;
			((GameComponent)walk2).Enabled = true;
			phaseCountdown = 0.25f;
			phaseLoop = 3;
			walk.ElapsedTime = 0L;
			walk2.ElapsedTime = 0L;
			((GameComponent)walk).Update(BaseGame.Get().emptytime);
			((GameComponent)walk2).Update(BaseGame.Get().emptytime);
			phaseExitLimit = 0.5f;
			firstHitUpdate = true;
			BaseGame.RunController(anim, walk);
		}
		return enemyState;
	}

	private void LeftWalkUpdate(GameTime gametime)
	{
		if (phaseCountdown >= 0f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		if (phaseCountdown <= 0f && (float)walk.ElapsedTime / (float)walk.Duration < phaseExitLimit + 0.1f && (float)walk2.ElapsedTime / (float)walk.Duration > phaseExitLimit)
		{
			phaseLoop--;
			phaseCountdown = 0.75f;
		}
		NormalUpdate(gametime);
	}

	private void LeftFallUpdate(GameTime gametime)
	{
		if (phaseCountdown >= 0f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		if (phaseCountdown <= 0f && (float)walk2.ElapsedTime / (float)walk2.Duration < phaseExitLimit + 0.1f && (float)walk2.ElapsedTime / (float)walk2.Duration > phaseExitLimit)
		{
			phaseLoop--;
			phaseCountdown = 0.75f;
		}
		NormalUpdate(gametime);
	}

	private bool LeftWalkRemove(ConditionSet cs)
	{
		if ((float)phaseLoop <= 0f)
		{
			return true;
		}
		return false;
	}

	private EnemyState HitLeftState()
	{
		phaseCountdown = 0.75f;
		phaseCountdown2 = 0.25f;
		phaseLoop = 1;
		EnemyState enemyState;
		if (bosshp[1] > 10)
		{
			enemyState = new EnemyState(LeftUpdate, NormalDraw, LeftWalkRemove, RightWalkState);
			walk2 = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["hitleft"]);
			falling = false;
			rotateRipple = false;
		}
		else
		{
			enemyState = new EnemyState(LeftUpdate, NormalDraw, LeftWalkRemove, RightWalkState);
			walk2 = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["fallleft"]);
			phaseLoop = 1;
			falling = true;
			rotateRipple = true;
		}
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		walk.ElapsedTime = (long)((float)walk.Duration / 2f);
		walk2.ElapsedTime = (long)((float)walk.ElapsedTime * (float)walk2.Duration / (float)walk.Duration);
		if (bosshp[1] <= 10)
		{
			walk2.ElapsedTime = (long)((float)walk2.ElapsedTime / 2f);
			walk2.ElapsedTime += (long)((float)walk2.Duration / 2f);
		}
		phaseExitLimit = (float)walk2.ElapsedTime / (float)walk2.Duration;
		((GameComponent)walk2).Update(BaseGame.Get().emptytime);
		((GameComponent)walk2).Enabled = true;
		((GameComponent)walk).Enabled = false;
		BaseGame.RunController(anim, walk2);
		return enemyState;
	}

	private void LeftUpdate(GameTime gametime)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		LeftFallUpdate(gametime);
		float num = (float)walk2.ElapsedTime / (float)walk2.Duration - 0.1f;
		if (phaseCountdown2 <= 0f && num > 0f && num < 0.1f)
		{
			if (falling)
			{
				ripplePos.Add(size * new Vector3(9f, 0f, -15f + num * 100f));
			}
			else
			{
				ripplePos.Add(size * new Vector3(4f, 0f, -15f));
				if (firstHitUpdate)
				{
					PathList pathList = new PathList();
					List<IPath> list = new List<IPath>();
					Vector3 val = size * new Vector3(8f, 5f, 0f) + getPos();
					list.Add(new PBezier(val, val + new Vector3(-50f, 0f, 0f), 2.5f, Vector3.Up, 0f, 0f, 1f, 0, 0f, 0.0, 0.0));
					list.Add(new PBezier(val + new Vector3(-50f, 0f, 0f), val + new Vector3(-60f, 0f, 0f), new Vector3(0f, 0f, 18f), Vector3.Zero, 0.15f, Vector3.Up, 0f, 0f, 1f, 0, 0f, 0.0, 0.0));
					pathList.addPathComboList(list, new PRefLine(Vector3.Zero, Vector3.One, 0f, BaseGame.Get().playerPos));
					GiftFlock giftFlock = new GiftFlock(1f, 2, 8, 20f, 5f, 4f, (FillMode)2, pathList);
					giftFlock.start();
					BaseGame.Get().enems.Add(giftFlock);
					firstHitUpdate = false;
				}
			}
			if (leftArmExists && falling)
			{
				ShootTiles(1);
				leftArmExists = false;
			}
			rE.Add(new RippleEffect(ripplePos[ripplePos.Count - 1], 0.5f, 0.5f, 0f, 1.1f, 10f, _loop: false, 0f));
			rE[rE.Count - 1].fxUpdate = BaseGame.GetFogEffect().Parameters;
			rEIndex.Add(r.Next() % pE.Length);
			phaseCountdown2 = 0.012f;
		}
		else if (phaseCountdown2 < 40f)
		{
			phaseCountdown2 -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
	}

	private EnemyState SetupLegsState()
	{
		EnemyState enemyState = new EnemyState(SetupLegsUpdate, NormalDraw, null, Walk2State);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(1.0));
		enemyState.condSet.Start();
		phaseCountdown = 0.9f;
		walk2 = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["pound"]);
		((GameComponent)walk2).Update(BaseGame.Get().emptytime);
		((GameComponent)walk2).Enabled = false;
		boneTree.Clear();
		curMesh = 0;
		curIndex = 0;
		AddLimbsToTree(eulerBones[model.model.Bones["Armature_RightLeg01"]], boneTree, model);
		SetupTiles(boneTree);
		boneTree.Clear();
		_ = targets.Count;
		curMesh = 0;
		curIndex = 0;
		AddLimbsToTree(eulerBones[model.model.Bones["Armature_LeftLeg01"]], boneTree, model);
		SetupTiles(boneTree);
		for (int i = 0; i < targets.Count; i++)
		{
			((FaceTarget)targets[i]).bossPart = 2;
		}
		legsExist = true;
		rotateRipple = true;
		falling = true;
		return enemyState;
	}

	private void SetupLegsUpdate(GameTime gametime)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (phaseCountdown > 0f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			setPos(getPos() + new Vector3(0f, 0f, 35f) * (float)gametime.ElapsedGameTime.TotalSeconds * 1.1f);
		}
		NormalUpdate(gametime);
	}

	private EnemyState Walk2State()
	{
		EnemyState enemyState = new EnemyState(NormalUpdate, NormalDraw, null, SmashState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(3.200000047683716));
		enemyState.condSet.Start();
		((GameComponent)walk).Enabled = true;
		((GameComponent)walk2).Enabled = false;
		BaseGame.RunController(anim, walk);
		((GameComponent)walk).Update(BaseGame.Get().emptytime);
		return enemyState;
	}

	private EnemyState SmashState()
	{
		EnemyState enemyState;
		if (bosshp[2] <= 10)
		{
			enemyState = SmashFallState();
			falling = true;
		}
		else
		{
			enemyState = new EnemyState(SmashUpdate, NormalDraw, null, Walk2State);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new TimeCondition(3.200000047683716));
			enemyState.condSet.Start();
			((GameComponent)walk).Enabled = false;
			((GameComponent)walk2).Enabled = true;
			firstHitUpdate = true;
			falling = false;
			BaseGame.RunController(anim, walk2);
			((GameComponent)walk2).Update(BaseGame.Get().emptytime);
		}
		return enemyState;
	}

	private void SmashUpdate(GameTime gametime)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		NormalUpdate(gametime);
		float num = (float)walk2.ElapsedTime / (float)walk2.Duration - 0.5f;
		if (phaseCountdown2 <= 0f && num > 0f && num < 0.1f)
		{
			if (falling)
			{
				ripplePos.Add(size * new Vector3(6f, -1f, -15f + num * 100f));
			}
			else
			{
				ripplePos.Add(size * new Vector3(6f, -1f, -15f + num * 100f));
				if (firstHitUpdate)
				{
					PathList pathList = new PathList();
					List<IPath> list = new List<IPath>();
					Vector3 val = size * new Vector3(2f, -1f, -2.5f) + getPos();
					list.Add(new PBezier(val, val + new Vector3(0f, 50f, 0f), 2.5f, Vector3.Forward, 0f, 0f, 1f, 0, 0f, 0.0, 0.0));
					list.Add(new PBezier(val + new Vector3(0f, 50f, 0f), val + new Vector3(0f, 60f, 0f), new Vector3(0f, 0f, 18f), Vector3.Zero, 0.15f, Vector3.Forward, 0f, 0f, 1f, 0, 0f, 0.0, 0.0));
					pathList.addPathComboList(list, new PRefLine(Vector3.Zero, Vector3.One, 0f, BaseGame.Get().playerPos));
					GiftFlock giftFlock = new GiftFlock(1f, 2, 8, 20f, 5f, 4f, (FillMode)2, pathList);
					giftFlock.start();
					BaseGame.Get().enems.Add(giftFlock);
					firstHitUpdate = false;
				}
			}
			rE.Add(new RippleEffect(ripplePos[ripplePos.Count - 1], 0.5f, 0.5f, 0f, 1.1f, 10f, _loop: false, 0f));
			rE[rE.Count - 1].fxUpdate = BaseGame.GetFogEffect().Parameters;
			rEIndex.Add(r.Next() % pE.Length);
			phaseCountdown2 = 0.012f;
		}
		else if (phaseCountdown2 < 40f)
		{
			phaseCountdown2 -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
	}

	private EnemyState SmashFallState()
	{
		phaseCountdown = 0.75f;
		phaseCountdown2 = 0.25f;
		phaseLoop = 1;
		EnemyState enemyState = new EnemyState(SmashFallUpdate, NormalDraw, LeftWalkRemove, FinalTransitionState);
		walk2 = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["fallleft"]);
		phaseLoop = 1;
		falling = true;
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		walk.ElapsedTime = (long)((float)walk.Duration / 2f);
		phaseExitLimit = (float)walk2.ElapsedTime / (float)walk2.Duration;
		((GameComponent)walk2).Update(BaseGame.Get().emptytime);
		((GameComponent)walk2).Enabled = true;
		((GameComponent)walk).Enabled = false;
		BaseGame.RunController(anim, walk2);
		return enemyState;
	}

	private void SmashFallUpdate(GameTime gametime)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		LeftFallUpdate(gametime);
		float num = (float)walk2.ElapsedTime / (float)walk2.Duration - 0.1f;
		if (phaseCountdown2 <= 0f && num > 0f && num < 0.1f)
		{
			ripplePos.Add(size * new Vector3(9f, 0f, -15f + num * 100f));
			if (legsExist)
			{
				ShootTiles(2);
				leftArmExists = false;
			}
			rE.Add(new RippleEffect(ripplePos[ripplePos.Count - 1], 0.5f, 0.5f, 0f, 1.1f, 10f, _loop: false, 0f));
			rE[rE.Count - 1].fxUpdate = BaseGame.GetFogEffect().Parameters;
			rEIndex.Add(r.Next() % pE.Length);
			phaseCountdown2 = 0.012f;
		}
		else if (phaseCountdown2 < 40f)
		{
			phaseCountdown2 -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
	}

	private EnemyState FinalTransitionState()
	{
		EnemyState enemyState = new EnemyState(FinalTransitionUpdate, OluDraw, FinalTransitionRemove, OpenMouthState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		phaseCountdown = 42f;
		phaseCountdown2 = 5f;
		curMesh = 0;
		curIndex = 0;
		walk2.IsLooping = false;
		mainBezier = new BezierHelper[4];
		SetupTilesByIndicesToDraw(3);
		return enemyState;
	}

	private void FinalTransitionUpdate(GameTime gametime)
	{
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 2; i++)
		{
			if (wireModel[0].Count > 0)
			{
				SplitListWire(0, wireModel[0][0]);
			}
			else if (phaseCountdown > 2f)
			{
				phaseCountdown = 2f;
				drawEyes = true;
				drawFace = true;
				walk = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["facefront"]);
				((GameComponent)walk).Enabled = true;
				walk.IsLooping = false;
				((GameComponent)walk).Update(BaseGame.Get().emptytime);
				((GameComponent)walk2).Enabled = false;
				walk2.IsLooping = true;
				BaseGame.RunController(anim, walk);
				break;
			}
		}
		if (phaseCountdown2 > 0f)
		{
			phaseCountdown2 -= (float)gametime.ElapsedGameTime.TotalSeconds;
			setPos(getPos() + (float)gametime.ElapsedGameTime.TotalSeconds * new Vector3(0f, 0f, -6f));
		}
		if (phaseCountdown > 0f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		NormalUpdate(gametime);
		mainBezier[0] = new BezierHelper(model.transforms, eulerBones, model.model.Bones, Transformation(), "Armature_LeftWaist");
		mainBezier[1] = new BezierHelper(model.transforms, eulerBones, model.model.Bones, Transformation(), "Armature_RightWaist");
		mainBezier[2] = new BezierHelper(model.transforms, eulerBones, model.model.Bones, Transformation(), "Armature_LeftArm01");
		mainBezier[3] = new BezierHelper(model.transforms, eulerBones, model.model.Bones, Transformation(), "Armature_RightArm01");
	}

	private void OluUpdate(GameTime gametime)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 8; i++)
		{
			if (((Vector3)(ref tentacleVelocity[i])).Equals(Vector3.Zero))
			{
				ref Vector3 reference = ref tentacleVelocity[i];
				reference = new Vector3(0f, 0f, 0.001f);
			}
			ref Vector3 reference2 = ref tentacleVelocity[i];
			reference2 += BaseGame.GetRandVect(Vector3.Normalize(tentacleVelocity[i]), 180f) * 0.2f * size;
			if (((Vector3)(ref tentacleWobbles[i])).Equals(Vector3.Zero))
			{
				ref Vector3 reference3 = ref tentacleWobbles[i];
				reference3 = new Vector3(0f, 0f, 0.001f);
			}
			ref Vector3 reference4 = ref tentacleWobbles[i];
			reference4 = Vector3.Lerp(tentacleWobbles[i], tentacleWobbles[i] + tentacleVelocity[i], 0.15f);
			if (((Vector3)(ref tentacleWobbles[i])).Length() > 0.4f * size)
			{
				ref Vector3 reference5 = ref tentacleWobbles[i];
				reference5 = Vector3.Normalize(tentacleWobbles[i]) * 0.4f * size;
			}
		}
		mainBezier[0] = new BezierHelper(model.transforms, eulerBones, model.model.Bones, Transformation(), "Armature_LeftWaist");
		mainBezier[1] = new BezierHelper(model.transforms, eulerBones, model.model.Bones, Transformation(), "Armature_RightWaist");
		mainBezier[2] = new BezierHelper(model.transforms, eulerBones, model.model.Bones, Transformation(), "Armature_LeftArm01");
		mainBezier[3] = new BezierHelper(model.transforms, eulerBones, model.model.Bones, Transformation(), "Armature_RightArm01");
		for (int j = 0; j < 4; j++)
		{
			mainBezier[j] = new BezierHelper(mainBezier[j].pos[0], mainBezier[j].pos[1], mainBezier[j].pos[2] + new Vector4(tentacleWobbles[2 * j], 0f), mainBezier[j].pos[3] + new Vector4(tentacleWobbles[2 * j + 1], 0f));
		}
	}

	private void OluDraw(GameTime gametime)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		NormalDraw(gametime);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().SwitchEffectTechnique("Bezier");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1f, -1f, -1f));
		BaseGame.Get().fogEffect.Parameters["BezierShift"].SetValue(phaseCountdown2 / 5f);
		if (phaseCountdown2 < 5f)
		{
			for (int i = 0; i < 4; i++)
			{
				BaseGame.Get().fogEffect.Parameters["BezierPos"].SetValue(mainBezier[i].BezierPos);
				BaseGame.Get().fogEffect.Parameters["BezierVel"].SetValue(mainBezier[i].BezierVel);
				BaseGame.Get().fogEffect.Parameters["Pos0"].SetValue(mainBezier[i].pos[0]);
				BaseGame.Get().fogEffect.Parameters["Pos1"].SetValue(mainBezier[i].pos[1]);
				BaseGame.Get().fogEffect.Parameters["Scale"].SetValue(mainBezier[i].scale);
				BaseGame.Get().DrawModel(ref Hypatia.tail);
			}
		}
		BaseGame.Get().fogEffect.Parameters["xPose"].SetValue(Matrix.Identity);
		BaseGame.Get().matStack.PopMatrix();
		if (drawEyes)
		{
			BaseGame.Get().SwitchEffectTechnique("Textured");
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Transformation());
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateTranslation(0.4f, 4.25f, 1.6f));
			BaseGame.Get().DrawModel(ref dot);
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateTranslation(-0.8f, 0f, 0f));
			BaseGame.Get().DrawModel(ref dot);
			BaseGame.Get().matStack.PopMatrix();
		}
		if (drawFace)
		{
			BaseGame.Get().SwitchEffectTechnique("Textured");
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(0.8f) * Matrix.CreateRotationX(MathHelper.ToRadians(-90f)) * Matrix.CreateTranslation(oluPos) * Transformation());
			BaseGame.Get().DrawModel(ref Hypatia.oluBack);
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
			BaseGame.Get().DrawModel(ref Hypatia.olu);
			BaseGame.Get().matStack.PopMatrix();
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
	}

	private bool FinalTransitionRemove(ConditionSet cs)
	{
		return phaseCountdown <= 0f;
	}

	private EnemyState OpenMouthState()
	{
		drawEyes = true;
		drawFace = true;
		EnemyState enemyState;
		if (bosshp[3] <= 0)
		{
			enemyState = new EnemyState(null, null, null, DieState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new AlwaysCondition());
			enemyState.condSet.Start();
		}
		else
		{
			enemyState = new EnemyState(OpenMouthUpdate, OluDraw, null, ShootMouthState);
			enemyState.condSet = new ConditionSet();
			enemyState.condSet.set.Add(new TimeCondition(0.5));
			enemyState.condSet.Start();
			walk = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["openjaw"]);
			((GameComponent)walk).Update(BaseGame.Get().emptytime);
			walk.IsLooping = false;
			BaseGame.RunController(anim, walk);
		}
		return enemyState;
	}

	private void OpenMouthUpdate(GameTime gametime)
	{
		NormalUpdate(gametime);
		OluUpdate(gametime);
	}

	private bool OpenMouthRemove(ConditionSet cs)
	{
		return false;
	}

	private EnemyState ShootMouthState()
	{
		EnemyState enemyState = new EnemyState(ShootMouthUpdate, OluDraw, null, CloseMouthState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(3.0));
		enemyState.condSet.Start();
		phaseCountdown = 0f;
		return enemyState;
	}

	private void ShootMouthUpdate(GameTime gametime)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		NormalUpdate(gametime);
		OluUpdate(gametime);
		phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (phaseCountdown <= 0f)
		{
			Vector3 val = size * new Vector3(0f, 3.42f, -1.21f) + getPos();
			Vector3 val2 = BaseGame.GetRandVect(new Vector3(0f, 0f, -1f), 37.5f) * size * 10f;
			val2.Z /= 2f;
			BulletC bulletC = new BulletC(val, val2, Vector3.Normalize(Vector3.Cross(val2, Vector3.Right)), this, 0);
			bulletC.start();
			bulletC.Launch();
			BaseGame.Get().enems.Add(bulletC);
			phaseCountdown = 0.25f;
		}
	}

	private bool ShootMouthRemove(ConditionSet cs)
	{
		return false;
	}

	private EnemyState CloseMouthState()
	{
		EnemyState enemyState = new EnemyState(CloseMouthUpdate, OluDraw, null, OpenMouthState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(4.5));
		enemyState.condSet.Start();
		walk = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["closejaw"]);
		((GameComponent)walk).Update(BaseGame.Get().emptytime);
		walk.IsLooping = false;
		BaseGame.RunController(anim, walk);
		return enemyState;
	}

	private void CloseMouthUpdate(GameTime gametime)
	{
		NormalUpdate(gametime);
		OluUpdate(gametime);
	}

	private bool CloseMouthRemove(ConditionSet cs)
	{
		return false;
	}

	private EnemyState DieState()
	{
		EnemyState enemyState = new EnemyState(DieUpdate, DieDraw, null, EndLevelState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(6.199999809265137));
		enemyState.condSet.Start();
		ShootTiles(3);
		drawEyes = true;
		drawFace = true;
		Enemy enemy = new GameplayChange("fade", 6.6f);
		BaseGame.Get().enems.Add(enemy);
		enemy.start();
		drawEyes = false;
		phaseCountdown2 = 0f;
		return enemyState;
	}

	private void DieUpdate(GameTime gametime)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		NormalUpdate(gametime);
		if (phaseCountdown2 <= 5f)
		{
			phaseCountdown2 += 1.6666666f * (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		oluPos += new Vector3(0f, -0.2f * (float)gametime.ElapsedGameTime.TotalSeconds, 0.49f * (float)gametime.ElapsedGameTime.TotalSeconds);
		for (int i = 0; i < 3; i++)
		{
			if (wireModel[0].Count > 0)
			{
				SplitListWire(0, wireModel[0][0]);
			}
		}
	}

	private void DieDraw(GameTime gametime)
	{
		OluDraw(gametime);
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
		EnemyState enemyState = new EnemyState(NormalUpdate, NormalDraw, null, null);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		((GameComponent)anim).Enabled = true;
		((GameComponent)anim).Update((GameTime)null);
		return enemyState;
	}

	private EnemyState EndLevelState()
	{
		EnemyState enemyState = new EnemyState(EndLevelUpdate, null, null, null);
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

	private void SetupTiles()
	{
		bool flag = false;
		while (curIndex < model.indices[curMesh].Length - 1 && !flag)
		{
			addTarget(1, 10, ref model, curMesh, curIndex);
			curIndex += 3;
		}
		if (!flag)
		{
			curMesh = -1;
		}
	}

	private void SetupTilesByIndicesToDraw(int bossPart)
	{
		for (int i = 0; i < model.indicesToDraw.Length; i++)
		{
			for (int j = 0; j < model.indicesToDraw[i].Count; j += 2)
			{
				for (int k = model.indicesToDraw[i][j]; k < model.indicesToDraw[i][j + 1]; k += 3)
				{
					addTarget(1, 10, ref model, i, k);
					((FaceTarget)targets[targets.Count - 1]).bossPart = bossPart;
				}
			}
		}
	}

	private void SetupTiles(List<int> boneLimit)
	{
		bool flag = false;
		while (curIndex < model.indices[curMesh].Length - 1 && !flag)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < boneLimit.Count; j++)
				{
					if (model.boneNames.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[boneLimit[j]].Name) && model.boneNames[((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[boneLimit[j]].Name][1] == model.vertices[curMesh][model.indices[curMesh][curIndex + i]].boneNum(0))
					{
						addTarget(1, 10, ref model, curMesh, curIndex);
						j = boneLimit.Count + 1;
						i = 4;
					}
				}
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
		flag = false;
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

	private void SplitListWire(int _mesh, int _index)
	{
		bool flag = false;
		pdColl.AddPlane(ref model, _mesh, _index, this, (FillMode)2);
		for (int i = 0; i < wireModel[_mesh].Count - 1; i += 2)
		{
			if (flag)
			{
				break;
			}
			if (_index < wireModel[_mesh][i] || _index > wireModel[_mesh][i + 1])
			{
				continue;
			}
			flag = true;
			if (_index == wireModel[_mesh][i])
			{
				if (_index == wireModel[_mesh][i + 1] - 2)
				{
					wireModel[_mesh].RemoveAt(i);
					wireModel[_mesh].RemoveAt(i);
				}
				else
				{
					wireModel[_mesh][i] = _index + 3;
				}
			}
			else if (_index == wireModel[_mesh][i + 1] - 2)
			{
				wireModel[_mesh][i + 1] = _index - 1;
			}
			else
			{
				wireModel[_mesh].Insert(i + 1, _index + 3);
				wireModel[_mesh].Insert(i + 1, _index - 1);
			}
		}
	}

	private void AddLimbsToTree(int boneIndex, List<int> boneIndices, ModelWrapper toScan)
	{
		boneIndices.Add(boneIndex);
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)((ReadOnlyCollection<ModelBone>)(object)toScan.model.Bones)[boneIndex].Children).Count; i++)
		{
			AddLimbsToTree(((ReadOnlyCollection<ModelBone>)(object)((ReadOnlyCollection<ModelBone>)(object)toScan.model.Bones)[boneIndex].Children)[i].Index, boneIndices, toScan);
		}
	}

	private void ShootTiles(int bossPart)
	{
		for (int num = targets.Count - 1; num >= 0; num--)
		{
			if (((FaceTarget)targets[num]).bossPart == bossPart)
			{
				SplitList(((FaceTarget)targets[num]).meshNum, ((FaceTarget)targets[num]).indexNum);
				pdColl.AddPlane(ref model, ((FaceTarget)targets[num]).meshNum, ((FaceTarget)targets[num]).indexNum, this, (FillMode)3);
				targets.RemoveAt(num);
			}
		}
	}
}
