using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class Whale01 : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	private static ModelWrapper _whaleModel;

	private static ModelWrapper _whaleWaves;

	private static Dictionary<ModelBone, int> _whaleWaveBones;

	public ConditionSet openCond;

	public ConditionSet launchCond;

	public int launchNum;

	public ModelWrapper whaleModel;

	public ModelWrapper whaleWaves;

	public ModelOluAnimator whaleAnim;

	public AnimationController opening;

	public AnimationController closed;

	public Vector3 actualPos;

	public Vector3 vel;

	public Vector3 up;

	public float maxSpeed;

	public float accel;

	public bool launched;

	public bool launchDone;

	private int launchChan;

	public float jetScale;

	public BulletB[] shots;

	private Matrix _transformation;

	private bool dirty;

	public List<PlaneEffectHelper> planeIdent;

	public List<int> planeBoneMap;

	public static PlaneEffect[] pFX;

	public float scaleAmount;

	public float scaleRate;

	public float scaleCountdown;

	public Whale01()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		state = 0;
		attackCooldown = 5f;
		hitPoints = 5;
		vel = Vector3.Forward;
		up = Vector3.Up;
		maxSpeed = 40f;
		accel = 2f;
		launched = (launchDone = false);
		jetScale = 1f;
		if (wCond == null)
		{
			wCond = new Dictionary<int, WaitCond>();
			wCond.Add(0, new WaitCond("Fish01", Beats.Eighth));
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
		openCond = new ConditionSet();
		launchCond = new ConditionSet();
		launchNum = 0;
		dirty = true;
	}

	public static void LoadModel()
	{
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		_whaleModel = BaseGame.Get().models.GetModel("Content\\Whale01\\Whale01Mod");
		BaseGame.SetAllEPCs(_whaleModel.epc, "xEnableLighting", true);
		BaseGame.Get().LinkEffect(_whaleModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		_whaleWaves = BaseGame.Get().models.GetModel("Content\\Whale01\\WhaleWave01");
		BaseGame.SetAllEPCs(_whaleWaves.epc, "xEnableLighting", false);
		BaseGame.Get().LinkEffect(_whaleWaves.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.GetFogEffect());
		_whaleWaveBones = new Dictionary<ModelBone, int>();
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)_whaleWaves.model.Bones).Count; i++)
		{
			if (!_whaleWaveBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)_whaleWaves.model.Bones)[i]))
			{
				_whaleWaveBones.Add(((ReadOnlyCollection<ModelBone>)(object)_whaleWaves.model.Bones)[i], i);
			}
		}
		Random random = new Random();
		int num = 4;
		float velocity = 0.2f;
		float vRandom = 0.02f;
		float sideSpeed = 0.05f;
		float sideSpeedRandom = 0.005f;
		Color blue = Color.Blue;
		pFX = new PlaneEffect[8];
		for (int j = 0; j < pFX.Length; j++)
		{
			PlaneEffect planeEffect = new PlaneEffect();
			for (int k = 0; k < num; k++)
			{
				TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, velocity, vRandom, sideSpeed, sideSpeedRandom);
				treeNode.branchTree = false;
				treeNode.setColor(blue);
				planeEffect.addNode(treeNode);
			}
			ref Vector3 reference = ref planeEffect.cornerNodes[0];
			reference = new Vector3(0f, 0f, 1f);
			ref Vector3 reference2 = ref planeEffect.cornerNodes[1];
			reference2 = new Vector3(1f, 0f, 1f);
			ref Vector3 reference3 = ref planeEffect.cornerNodes[2];
			reference3 = new Vector3(0f, 0f, 0f);
			ref Vector3 reference4 = ref planeEffect.cornerNodes[3];
			reference4 = new Vector3(1f, 0f, 0f);
			planeEffect.iteratePlane();
			planeEffect.FinalizeEffect();
			pFX[j] = planeEffect;
		}
	}

	public Whale01(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		launchChan = LevelLoader.GetIntFromAtt(attributes, "channel", 0);
	}

	public override void draw(GameTime gametime)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.Get().matStack.PushMatrix();
		if (scaleAmount > 0.1f)
		{
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1f, scaleAmount, 1f));
		}
		else
		{
			BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(0f));
		}
		BaseGame.SetAllEPCs(whaleModel.epc, "DirLight0Direction", pos - BaseGame.Get().playerPos);
		BaseGame.Get().DrawModel(ref whaleModel);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().SwitchEffectTechnique("Colored");
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		BaseGame.Get().fogEffect.Begin((SaveStateMode)0);
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		for (int i = 0; i < planeIdent.Count; i++)
		{
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(planeIdent[i].transform * whaleWaves.transforms[planeBoneMap[i]]);
			pFX[planeIdent[i].planeNum].draw();
			BaseGame.Get().matStack.PopMatrix();
		}
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
		BaseGame.Get().matStack.PopMatrix();
	}

	public override Matrix Transformation()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (dirty)
		{
			_transformation = BaseGame.MapObjectToSystem(Vector3.Zero, (vel == Vector3.Zero) ? Vector3.Forward : vel, up) * Matrix.CreateScale(new Vector3(3f, 3f, 3f)) * Matrix.CreateTranslation(getPos());
			dirty = false;
		}
		return _transformation;
	}

	public unsafe override void hit(TargetEffectBase toHit)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		if (!toHit.skipSquare)
		{
			BaseGame.Get().ps.AddParticles(toHit.eTarget.absolutePos(), Vector3.Forward * 25f, 2f, 180f, Vector3.Zero, 0f, 0.25f, 0.1f, 0.2f, new Vector4(0f, 1f, 0f, 1f), 320, 0.0005f);
		}
		if (toHit.eTarget.hp == 1)
		{
			whaleAnim.BonePoses[((BoneModelTarget)toHit.eTarget).boneName].enabled = false;
			ModelBone val = whaleWaves.model.Bones[whaleAnim.BonePoses[((BoneModelTarget)toHit.eTarget).boneName].Name];
			ref Matrix reference = ref whaleWaves.transforms[_whaleWaveBones[val]];
			reference = new Matrix(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
			Enumerator enumerator = val.Children.GetEnumerator();
			try
			{
				while (((Enumerator)(ref enumerator)).MoveNext())
				{
					ModelBone current = ((Enumerator)(ref enumerator)).Current;
					ref Matrix reference2 = ref whaleWaves.transforms[_whaleWaveBones[current]];
					reference2 = new Matrix(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
				}
			}
			finally
			{
				((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
			}
		}
		base.hit(toHit);
	}

	public override void act(GameTime gametime)
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		base.act(gametime);
		if (!exists)
		{
			return;
		}
		launchCond.Update();
		if (scaleCountdown >= 0f)
		{
			scaleCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		if (state == 0 && scaleCountdown < 0f)
		{
			state++;
		}
		if (state == 1)
		{
			scaleAmount += (float)gametime.ElapsedGameTime.TotalSeconds * scaleRate;
			if (scaleAmount >= 1f)
			{
				scaleAmount = 1f;
				state++;
				openCond.set.Add(new TimeCondition(4.0));
				addTarget(new Vector3(0f, 0f, 0f), 2, 10, ref whaleModel, 0, "SolidFrontArm_Root");
				addTarget(new Vector3(0f, 0f, 0f), 1, 10, ref whaleModel, 0, "SolidMidArm_Root");
				addTarget(new Vector3(0f, 0f, 0f), 1, 10, ref whaleModel, 0, "SolidBackArm_Root");
				addTarget(new Vector3(0f, 0f, 0f), 1, 10, ref whaleModel, 0, "SolidTail_Root");
			}
		}
		if (state == 2 && openCond.ConditionsMet())
		{
			openCond.set.Clear();
			openCond.set.Add(new NeverCondition());
			((GameComponent)opening).Enabled = true;
			((GameComponent)closed).Enabled = false;
			BaseGame.RunController(whaleAnim, opening);
			launchCond.set.Clear();
			launchCond.set.Add(new TimeCondition(2.0));
			launchCond.Start();
			state++;
		}
		if (state == 3 && launchCond.ConditionsMet())
		{
			launchCond.set.Clear();
			launchCond.set.Add(new ChanCondition(launchChan, 0.95f));
			launchCond.Start();
			state++;
		}
		if (state == 4 && launchCond.ConditionsMet())
		{
			shots[launchNum].Launch();
			launchNum++;
			launchCond.set.Clear();
			if (launchNum > 2)
			{
				launchCond.set.Add(new NeverCondition());
			}
			else
			{
				launchCond.set.Add(new ChanCondition(launchChan, 0.95f));
				launchCond.Start();
			}
		}
		if (((GameComponent)opening).Enabled)
		{
			((GameComponent)opening).Update(gametime);
		}
		if (((GameComponent)closed).Enabled)
		{
			((GameComponent)closed).Update(gametime);
		}
		((GameComponent)whaleAnim).Update(gametime);
		for (int num = planeIdent.Count - 1; num >= 0; num--)
		{
			if (whaleWaves.transforms[planeBoneMap[num]].M44 == 0f)
			{
				planeIdent.RemoveAt(num);
				planeBoneMap.RemoveAt(num);
			}
		}
		dirty = true;
	}

	public unsafe override void start()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		Random random = new Random();
		base.start();
		vel = Vector3.Backward;
		actualPos = pos;
		whaleModel = new ModelWrapper(_whaleModel);
		whaleWaves = new ModelWrapper(_whaleWaves);
		whaleAnim = new ModelOluAnimator(BaseGame.Get().CoreGame, whaleModel, BaseGame.GetFogEffect());
		opening = new AnimationController(BaseGame.Get().CoreGame, whaleAnim.Animations["open"]);
		closed = new AnimationController(BaseGame.Get().CoreGame, whaleAnim.Animations["still"]);
		BaseGame.RunController(whaleAnim, closed);
		closed.IsLooping = false;
		opening.IsLooping = false;
		((GameComponent)opening).Enabled = false;
		addCond(new NeverCondition());
		scaleCountdown = 2f;
		scaleAmount = 0f;
		scaleRate = 1f;
		openCond.Start();
		launchCond.set.Add(new NeverCondition());
		shots = new BulletB[3];
		shots[0] = new BulletB(new Vector3(0f, 0.75f, 0f), Vector3.Up, Vector3.Forward, this, ref whaleModel, "SolidFrontArm_Root");
		shots[1] = new BulletB(new Vector3(0f, 0.75f, 0f), Vector3.Up, Vector3.Forward, this, ref whaleModel, "SolidMidArm_Root");
		shots[2] = new BulletB(new Vector3(0f, 0.75f, 0f), Vector3.Up, Vector3.Forward, this, ref whaleModel, "SolidBackArm_Root");
		for (int i = 0; i < 3; i++)
		{
			shots[i].start();
			BaseGame.Get().enems.Add(shots[i]);
		}
		planeIdent = new List<PlaneEffectHelper>();
		planeBoneMap = new List<int>();
		Enumerator enumerator = whaleWaves.model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				short[] array = new short[current.IndexBuffer.SizeInBytes / 2];
				Vector3[] array2 = (Vector3[])(object)new Vector3[current.VertexBuffer.SizeInBytes / 24];
				current.IndexBuffer.GetData<short>(array);
				current.VertexBuffer.GetData<Vector3>(0, array2, 0, current.VertexBuffer.SizeInBytes / 24, 24);
				for (int j = 0; j < array.Length; j += 3)
				{
					planeIdent.Add(new PlaneEffectHelper());
					EnemyCube.CreateTransformationMatrix(array2[array[j]], array2[array[j + 1]], array2[array[j]], array2[array[j + 2]], out planeIdent[planeIdent.Count - 1].transform);
					planeBoneMap.Add(current.ParentBone.Index);
					planeIdent[planeIdent.Count - 1].planeNum = random.Next(pFX.Length);
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public override Vector3 getPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return pos;
	}

	public override string name()
	{
		return "[qualifier_0xF053]";
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
