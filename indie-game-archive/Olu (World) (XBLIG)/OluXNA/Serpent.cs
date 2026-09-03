using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class Serpent : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public static ModelWrapper headModel;

	public static ModelWrapper headModelWire;

	public static ModelWrapper headModelOlu;

	public static PlaneEffect pE;

	public static PlaneEffect pEwire;

	public static PlaneEffect pEOlu;

	public int activeTails;

	public int maxTails;

	public float launchInt;

	public float maxLaunch;

	private int channel;

	private float boost;

	private float size;

	private float waterHeight;

	private bool waterEnabled;

	private bool underWater;

	private bool oldWater;

	public List<RippleEffect> rE;

	public Vector3 ripplePos;

	public bool oluMode;

	private Vector3 up;

	private SerpentTail[] tails;

	public bool CreateRipple => underWater ^ oldWater;

	public Serpent()
	{
		state = 0;
		attackCooldown = 5f;
		hitPoints = 1;
		if (wCond == null)
		{
			wCond = new Dictionary<int, WaitCond>();
			wCond.Add(0, new WaitCond("Serp01", Beats.Eighth));
			wCond.Add(1, new WaitCond("Serp02", Beats.Eighth));
			wCond.Add(2, new WaitCond("Serp03", Beats.Eighth));
			wCond.Add(3, new WaitCond("Serp04", Beats.Eighth));
			wCond.Add(4, new WaitCond("Serp05", Beats.Eighth));
			wCond.Add(5, new WaitCond("Serp06", Beats.Eighth));
			wCond.Add(6, new WaitCond("Serp07", Beats.Eighth));
			wCond.Add(7, new WaitCond("Serp08", Beats.Eighth));
		}
		_eCond = wCond;
		waterEnabled = (oldWater = false);
		rE = new List<RippleEffect>();
	}

	public Serpent(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		Initialize(LevelLoader.GetIntFromAtt(attributes, "numtails", 3), LevelLoader.GetFloatFromAtt(attributes, "spacing", 0.2f), LevelLoader.GetIntFromAtt(attributes, "chan", 11), LevelLoader.GetFloatFromAtt(attributes, "boost", 0f), LevelLoader.GetVectorFromAtt(attributes, "up", new Vector3(0f, 0f, -1f)));
		tails = new SerpentTail[maxTails];
		oluMode = false;
		if (attributes.ContainsKey("fill"))
		{
			if (attributes["fill"].Equals("wire"))
			{
				fillMode = (FillMode)2;
			}
			if (attributes["fill"].Equals("olu"))
			{
				fillMode = (FillMode)2;
				oluMode = true;
			}
		}
		size = LevelLoader.GetFloatFromAtt(attributes, "size", 1f);
		waterEnabled = true;
		if (attributes.ContainsKey("waterheight"))
		{
			waterHeight = LevelLoader.GetFloatFromAtt(attributes, "waterheight", 0f);
		}
		else
		{
			waterEnabled = false;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		for (int i = 0; i < maxTails; i++)
		{
			tails[i] = new SerpentTail(attributes, node);
		}
	}

	public Serpent(int _maxTails, float _spacing, int _channel, float _boost, Vector3 _up, PathList _pathList)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(_maxTails, _spacing, _channel, _boost, _up, _pathList, (FillMode)3, _oluMode: false, _waterEnabled: false, 0f);
	}

	public Serpent(int _maxTails, float _spacing, int _channel, float _boost, Vector3 _up, PathList _pathList, FillMode _fillMode, bool _oluMode, bool _waterEnabled, float _waterLevel)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		Initialize(_maxTails, _spacing, _channel, _boost, _up);
		fillMode = _fillMode;
		waterEnabled = _waterEnabled;
		waterHeight = _waterLevel;
		size = 1f;
		oluMode = _oluMode;
		pathList = _pathList.Clone();
		tails = new SerpentTail[maxTails];
		for (int i = 0; i < maxTails; i++)
		{
			tails[i] = new SerpentTail(_pathList.Clone(), _up, _fillMode, _oluMode, _waterEnabled, _waterLevel);
		}
	}

	protected void Initialize(int _maxTails, float _spacing, int _channel, float _boost, Vector3 _up)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		maxTails = _maxTails;
		activeTails = 0;
		launchInt = (maxLaunch = _spacing);
		channel = _channel;
		boost = _boost;
		up = _up;
	}

	public static void LoadModel()
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		headModel = BaseGame.Get().models.GetModel("Content\\Serpent\\SerpentHead");
		BaseGame.Get().LinkEffect(headModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.Get().fogEffect);
		BaseGame.SetAllEPCs(headModel.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(headModel.epc, "DiffuseColor", (object)new Vector3(1f, 0f, 0f));
		headModelWire = BaseGame.Get().models.GetModel("Content\\Serpent\\SerpentHead", copyData: false, copyEPC: true);
		BaseGame.Get().LinkEffect(headModelWire.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.Get().fogEffect);
		BaseGame.SetAllEPCs(headModelWire.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(headModelWire.epc, "DiffuseColor", (object)new Vector3(0.5f, 0.5f, 1f));
		headModelOlu = BaseGame.Get().models.GetModel("Content\\Serpent\\SerpentHead", copyData: false, copyEPC: true);
		BaseGame.Get().LinkEffect(headModelOlu.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.Get().fogEffect);
		BaseGame.SetAllEPCs(headModelOlu.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(headModelOlu.epc, "DiffuseColor", (object)new Vector3(0.2f, 1f, 0.2f));
		Random random = new Random();
		pE = new PlaneEffect();
		for (int i = 0; i < 4; i++)
		{
			TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, 0.12f, 0.04f, 0.12f, 0.04f);
			treeNode.branchTree = false;
			treeNode.setColor(Color.Blue);
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
		pE.FinalizeEffect();
		pEwire = new PlaneEffect();
		for (int j = 0; j < 4; j++)
		{
			TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, 0.12f, 0.04f, 0.12f, 0.04f);
			treeNode.branchTree = false;
			treeNode.setColor(Color.Red);
			pEwire.addNode(treeNode);
		}
		ref Vector3 reference5 = ref pEwire.cornerNodes[0];
		reference5 = new Vector3(-0.5f, 0f, 0.5f);
		ref Vector3 reference6 = ref pEwire.cornerNodes[1];
		reference6 = new Vector3(0.5f, 0f, 0.5f);
		ref Vector3 reference7 = ref pEwire.cornerNodes[2];
		reference7 = new Vector3(-0.5f, 0f, -0.5f);
		ref Vector3 reference8 = ref pEwire.cornerNodes[3];
		reference8 = new Vector3(0.5f, 0f, -0.5f);
		pEwire.iteratePlane();
		pEwire.FinalizeEffect();
		pEOlu = new PlaneEffect();
		for (int k = 0; k < 4; k++)
		{
			TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, 0.12f, 0.04f, 0.12f, 0.04f);
			treeNode.branchTree = false;
			treeNode.setColor(Color.Green);
			pEOlu.addNode(treeNode);
		}
		ref Vector3 reference9 = ref pEOlu.cornerNodes[0];
		reference9 = new Vector3(-0.5f, 0f, 0.5f);
		ref Vector3 reference10 = ref pEOlu.cornerNodes[1];
		reference10 = new Vector3(0.5f, 0f, 0.5f);
		ref Vector3 reference11 = ref pEOlu.cornerNodes[2];
		reference11 = new Vector3(-0.5f, 0f, -0.5f);
		ref Vector3 reference12 = ref pEOlu.cornerNodes[3];
		reference12 = new Vector3(0.5f, 0f, -0.5f);
		pEOlu.iteratePlane();
		pEOlu.FinalizeEffect();
	}

	public override void draw(GameTime gametime)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Invalid comparison between Unknown and I4
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Invalid comparison between Unknown and I4
		if (waterEnabled)
		{
			BaseGame.Get().SwitchEffectTechnique("Water");
			BaseGame.Get().fogEffect.Parameters["WaterHeight"].SetValue(waterHeight);
		}
		else
		{
			BaseGame.Get().SwitchEffectTechnique("Textured");
		}
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateRotationY(MathHelper.ToRadians(-90f)) * BaseGame.MapObjectToSystem(Vector3.Zero, getDir(), up) * Matrix.CreateRotationX(MathHelper.ToRadians(90f)) * Matrix.CreateScale(-2f, 2f, 2f) * Matrix.CreateScale(size) * Matrix.CreateTranslation(getPos()));
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = fillMode;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		if (oluMode)
		{
			BaseGame.Get().DrawModel(ref headModelOlu);
		}
		else if ((int)fillMode == 3)
		{
			BaseGame.Get().DrawModel(ref headModel);
		}
		else
		{
			BaseGame.Get().DrawModel(ref headModelWire);
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().SwitchEffectTechnique("Textured");
		for (int num = rE.Count - 1; num >= 0; num--)
		{
			if (!rE[num].done)
			{
				BaseGame.Get().matStack.PushMatrix();
				rE[num].Draw(gametime);
				BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(120f) * Matrix.CreateTranslation(rE[num].pos));
				BaseGame.Get().fogEffect.Begin();
				BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
				if (oluMode)
				{
					pEOlu.draw();
				}
				else if ((int)fillMode == 3)
				{
					pE.draw();
				}
				else
				{
					pEwire.draw();
				}
				BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
				BaseGame.Get().fogEffect.End();
				BaseGame.Get().matStack.PopMatrix();
			}
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Invalid comparison between Unknown and I4
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		ParticleSystem ps = BaseGame.Get().ps;
		Vector3 center = getPos();
		Vector3 vel = Vector3.Forward * 25f;
		Vector3 zero = Vector3.Zero;
		_003F col;
		if (!oluMode)
		{
			col = (((int)fillMode == 3) ? new Vector4(1f, 0f, 0f, 1f) : new Vector4(0.5f, 0.5f, 1f, 1f));
		}
		else
		{
			Color green = Color.Green;
			col = ((Color)(ref green)).ToVector4();
		}
		ps.AddParticles(center, vel, 2f, 180f, zero, 0f, 0.35f, 0.1f, 0.2f, (Vector4)col, 20, 0.0005f);
		base.hit(toHit);
	}

	public override void act(GameTime gametime)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		oldWater = underWater;
		if (activeTails < maxTails)
		{
			launchInt -= (float)gametime.ElapsedGameTime.TotalSeconds * (1f + boost * BaseGame.Get().channels[channel]);
			if (launchInt <= 0f)
			{
				launchInt += maxLaunch;
				tails[activeTails].start();
				BaseGame.Get().enems.Add(tails[activeTails]);
				activeTails++;
			}
		}
		if (getPos().Y > waterHeight)
		{
			underWater = false;
		}
		else
		{
			underWater = true;
		}
		if (waterEnabled && CreateRipple)
		{
			ripplePos = new Vector3(getPos().X, waterHeight, getPos().Z);
			rE.Add(new RippleEffect(ripplePos, 0.5f, 0.5f, 0f, 0.4f, 1.5f, _loop: false, 0f));
			rE[rE.Count - 1].fxUpdate = BaseGame.GetFogEffect().Parameters;
		}
		for (int num = rE.Count - 1; num >= 0; num--)
		{
			rE[num].Update(gametime);
			if (rE[num].done)
			{
				rE.RemoveAt(num);
			}
		}
		base.act(gametime);
		if (pathList.curPathIndex == pathList.publicPaths.Count - 1 && pathList.publicPaths.Count > 1)
		{
			PlayerHit();
		}
	}

	public override void start()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (waterEnabled)
		{
			addTarget(Vector3.Zero, 1, 10, waterHeight);
		}
		else
		{
			addTarget(Vector3.Zero, 1, 10);
		}
		addCond(new NeverCondition());
		base.start();
		if (getPos().Y > waterHeight)
		{
			underWater = (oldWater = false);
		}
		else
		{
			underWater = (oldWater = true);
		}
	}

	public override Enemy attack()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Enemy enemy = new Enemy();
		enemy = new BulletA(getPos());
		enemy.start();
		return enemy;
	}

	public override string name()
	{
		return "[serpent]";
	}

	public Vector3 getDir()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return pathList.curDir();
	}

	public override bool Check(int numEnem)
	{
		return wCond[numEnem].Check(BaseGame.Get().curBeat);
	}

	public override void HitSound(int lockNum, float volume)
	{
		if (lockNum <= 8)
		{
			BaseGame.Get().PlayCue(wCond[BaseGame.Get().curBeat / 2].cueName, volume);
		}
	}
}
