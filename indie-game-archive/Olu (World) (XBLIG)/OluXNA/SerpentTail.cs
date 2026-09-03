using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class SerpentTail : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public static ModelWrapper tailModel;

	public static ModelWrapper tailModelWire;

	public static ModelWrapper tailModelOlu;

	public static PlaneEffect[] pE;

	public static PlaneEffect[] pEwire;

	public static PlaneEffect[] pEOlu;

	public Matrix preRotMatrix;

	public Matrix postRotMatrix;

	private float waterHeight;

	private bool waterEnabled;

	private bool underWater;

	private bool oldWater;

	private float size;

	public List<RippleEffect> rE;

	public Vector3 ripplePos;

	public int planeIndex;

	public bool oluMode;

	private Vector3 up;

	public bool CreateRipple => underWater ^ oldWater;

	public SerpentTail()
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
		rE = new List<RippleEffect>();
	}

	public SerpentTail(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
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
		waterEnabled = true;
		if (attributes.ContainsKey("waterheight"))
		{
			waterHeight = LevelLoader.GetFloatFromAtt(attributes, "waterheight", 0f);
		}
		else
		{
			waterEnabled = false;
		}
		size = LevelLoader.GetFloatFromAtt(attributes, "size", 1f);
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		up = LevelLoader.GetVectorFromAtt(attributes, "up", new Vector3(0f, 0f, -1f));
		planeIndex = BaseGame.Get().r.Next(12);
	}

	public SerpentTail(PathList _path, Vector3 _up, FillMode _fillMode, bool _oluMode, bool _waterEnabled, float _waterLevel)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		fillMode = _fillMode;
		waterEnabled = _waterEnabled;
		waterHeight = _waterLevel;
		size = 1f;
		oluMode = _oluMode;
		pathList = _path;
		up = _up;
	}

	public static void LoadModel()
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_055c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		tailModel = BaseGame.Get().models.GetModel("Content\\Serpent\\SerpentTail");
		BaseGame.Get().LinkEffect(tailModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.Get().fogEffect);
		BaseGame.SetAllEPCs(tailModel.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(tailModel.epc, "DiffuseColor", (object)new Vector3(1f, 0f, 0f));
		tailModelWire = BaseGame.Get().models.GetModel("Content\\Serpent\\SerpentTail", copyData: false, copyEPC: true);
		BaseGame.Get().LinkEffect(tailModelWire.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.Get().fogEffect);
		BaseGame.SetAllEPCs(tailModelWire.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(tailModelWire.epc, "DiffuseColor", (object)new Vector3(0.3f, 0.3f, 1f));
		tailModelOlu = BaseGame.Get().models.GetModel("Content\\Serpent\\SerpentTail", copyData: false, copyEPC: true);
		BaseGame.Get().LinkEffect(tailModelOlu.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.Get().fogEffect);
		BaseGame.SetAllEPCs(tailModelOlu.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(tailModelOlu.epc, "DiffuseColor", (object)new Vector3(0.1f, 0.7f, 0.1f));
		pE = new PlaneEffect[12];
		for (int i = 0; i < pE.Length; i++)
		{
			pE[i] = new PlaneEffect();
			for (int j = 0; j < 4; j++)
			{
				TreeNode treeNode = new TreeNode((float)BaseGame.Get().r.NextDouble(), 0f, 0f, 1, 0.12f, 0.04f, 0.12f, 0.04f);
				treeNode.branchTree = false;
				treeNode.setColor(Color.Blue);
				pE[i].addNode(treeNode);
			}
			ref Vector3 reference = ref pE[i].cornerNodes[0];
			reference = new Vector3(-0.5f, 0f, 0.5f);
			ref Vector3 reference2 = ref pE[i].cornerNodes[1];
			reference2 = new Vector3(0.5f, 0f, 0.5f);
			ref Vector3 reference3 = ref pE[i].cornerNodes[2];
			reference3 = new Vector3(-0.5f, 0f, -0.5f);
			ref Vector3 reference4 = ref pE[i].cornerNodes[3];
			reference4 = new Vector3(0.5f, 0f, -0.5f);
			pE[i].iteratePlane();
			pE[i].FinalizeEffect();
		}
		pEwire = new PlaneEffect[12];
		for (int k = 0; k < pEwire.Length; k++)
		{
			pEwire[k] = new PlaneEffect();
			for (int l = 0; l < 4; l++)
			{
				TreeNode treeNode = new TreeNode((float)BaseGame.Get().r.NextDouble(), 0f, 0f, 1, 0.12f, 0.04f, 0.12f, 0.04f);
				treeNode.branchTree = false;
				treeNode.setColor(Color.Blue);
				pEwire[k].addNode(treeNode);
			}
			ref Vector3 reference5 = ref pEwire[k].cornerNodes[0];
			reference5 = new Vector3(-0.5f, 0f, 0.5f);
			ref Vector3 reference6 = ref pEwire[k].cornerNodes[1];
			reference6 = new Vector3(0.5f, 0f, 0.5f);
			ref Vector3 reference7 = ref pEwire[k].cornerNodes[2];
			reference7 = new Vector3(-0.5f, 0f, -0.5f);
			ref Vector3 reference8 = ref pEwire[k].cornerNodes[3];
			reference8 = new Vector3(0.5f, 0f, -0.5f);
			pEwire[k].iteratePlane();
			pEwire[k].FinalizeEffect();
		}
		pEOlu = new PlaneEffect[12];
		for (int m = 0; m < pEOlu.Length; m++)
		{
			pEOlu[m] = new PlaneEffect();
			for (int n = 0; n < 4; n++)
			{
				TreeNode treeNode = new TreeNode((float)BaseGame.Get().r.NextDouble(), 0f, 0f, 1, 0.12f, 0.04f, 0.12f, 0.04f);
				treeNode.branchTree = false;
				treeNode.setColor(Color.Green);
				pEOlu[m].addNode(treeNode);
			}
			ref Vector3 reference9 = ref pEOlu[m].cornerNodes[0];
			reference9 = new Vector3(-0.5f, 0f, 0.5f);
			ref Vector3 reference10 = ref pEOlu[m].cornerNodes[1];
			reference10 = new Vector3(0.5f, 0f, 0.5f);
			ref Vector3 reference11 = ref pEOlu[m].cornerNodes[2];
			reference11 = new Vector3(-0.5f, 0f, -0.5f);
			ref Vector3 reference12 = ref pEOlu[m].cornerNodes[3];
			reference12 = new Vector3(0.5f, 0f, -0.5f);
			pEOlu[m].iteratePlane();
			pEOlu[m].FinalizeEffect();
		}
	}

	public override void draw(GameTime gametime)
	{
	}

	public void DrawModel(GameTime gametime)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Invalid comparison between Unknown and I4
		if (waterEnabled)
		{
			BaseGame.Get().fogEffect.Parameters["WaterHeight"].SetValue(waterHeight);
		}
		BaseGame.Get().fogEffect.Parameters["xPose"].SetValue(tailModel.transforms[((ReadOnlyCollection<ModelMesh>)(object)tailModel.model.Meshes)[0].ParentBone.Index]);
		BaseGame.Get().matStack.ApplyRawMatrix(preRotMatrix * BaseGame.MapObjectToSystem(Vector3.Zero, getDir(), up) * postRotMatrix * Matrix.CreateTranslation(getPos()));
		if (oluMode)
		{
			BaseGame.Get().DrawModelEffectStarted(ref tailModelOlu);
		}
		else if ((int)fillMode == 3)
		{
			BaseGame.Get().DrawModelEffectStarted(ref tailModel);
		}
		else
		{
			BaseGame.Get().DrawModelEffectStarted(ref tailModelWire);
		}
	}

	public void DrawRipple(GameTime gametime)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Invalid comparison between Unknown and I4
		for (int num = rE.Count - 1; num >= 0; num--)
		{
			if (!rE[num].done)
			{
				BaseGame.Get().matStack.PushMatrix();
				rE[num].Draw(gametime);
				BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(120f) * Matrix.CreateTranslation(rE[num].pos));
				if (oluMode)
				{
					pEOlu[planeIndex].draw();
				}
				else if ((int)fillMode == 3)
				{
					pE[planeIndex].draw();
				}
				else
				{
					pEwire[planeIndex].draw();
				}
				BaseGame.Get().matStack.PopMatrix();
			}
		}
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
			col = (((int)fillMode == 3) ? new Vector4(1f, 0f, 0f, 1f) : new Vector4(0.3f, 0.3f, 1f, 1f));
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
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		oldWater = underWater;
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
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Invalid comparison between Unknown and I4
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
		preRotMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(-90f));
		postRotMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(90f)) * Matrix.CreateScale(-2f, 2f, 2f) * Matrix.CreateScale(size);
		if (oluMode)
		{
			BaseGame.Get().sTail01.waterWireOluEnemies.Add(this);
		}
		else if ((int)fillMode == 3)
		{
			if (waterEnabled)
			{
				BaseGame.Get().sTail01.waterSolidNormEnemies.Add(this);
			}
			else
			{
				BaseGame.Get().sTail01.normSolidNormEnemies.Add(this);
			}
		}
		else if (waterEnabled)
		{
			BaseGame.Get().sTail01.waterWireNormEnemies.Add(this);
		}
		else
		{
			BaseGame.Get().sTail01.normWireNormEnemies.Add(this);
		}
	}

	public override void leave()
	{
		RemoveFromColl();
		base.leave();
	}

	public override void die()
	{
		RemoveFromColl();
		base.die();
	}

	public void RemoveFromColl()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		if (oluMode)
		{
			BaseGame.Get().sTail01.waterWireOluEnemies.Remove(this);
		}
		else if ((int)fillMode == 3)
		{
			if (waterEnabled)
			{
				BaseGame.Get().sTail01.waterSolidNormEnemies.Remove(this);
			}
			else
			{
				BaseGame.Get().sTail01.normSolidNormEnemies.Remove(this);
			}
		}
		else if (waterEnabled)
		{
			BaseGame.Get().sTail01.waterWireNormEnemies.Remove(this);
		}
		else
		{
			BaseGame.Get().sTail01.normWireNormEnemies.Remove(this);
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
