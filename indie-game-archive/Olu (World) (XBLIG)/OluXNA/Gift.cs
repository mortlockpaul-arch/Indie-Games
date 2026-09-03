using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class Gift : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public static ModelWrapper model;

	public static ModelWrapper dot;

	public ModelWrapper modelLocal;

	public ModelWrapper dotLocal;

	public Vector3 vel;

	public Vector3 up;

	public Vector3 staticPos;

	public float maxSpeed;

	public float accel;

	public ModelOluAnimator anim;

	public AnimationController openAnim;

	public ConditionSet openCond;

	public float pCooldown;

	public float pMax;

	public float size;

	public float waitAmount;

	public bool oluMode;

	public Gift()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		pMax = 0.05f;
		base._002Ector();
		state = 0;
		attackCooldown = 5f;
		hitPoints = 2;
		vel = Vector3.Forward;
		up = Vector3.Up;
		maxSpeed = 40f;
		accel = 2f;
		if (wCond == null)
		{
			wCond = new Dictionary<int, WaitCond>();
			wCond.Add(0, new WaitCond("GiftDrum1", Beats.Quarter));
			wCond.Add(1, new WaitCond("GiftDrum2", Beats.Eighth));
			wCond.Add(2, new WaitCond("GiftDrum2", Beats.Eighth, Beats.Quarter));
			wCond.Add(3, new WaitCond("GiftDrum1", Beats.Eighth, Beats.Quarter));
			wCond.Add(4, new WaitCond("GiftDrum1", Beats.Eighth, Beats.Quarter));
			wCond.Add(5, new WaitCond("GiftDrum2", Beats.Eighth));
			wCond.Add(6, new WaitCond("GiftDrum1", Beats.Eighth));
			wCond.Add(7, new WaitCond("GiftDrum2", Beats.Eighth));
			wCond.Add(8, new WaitCond("GiftDrum2", Beats.Eighth));
		}
		_eCond = wCond;
		openCond = new ConditionSet();
	}

	public static void LoadModel()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		model = BaseGame.Get().models.GetModel("Content\\Gift\\Gift");
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(model.epc, "TextureMix", BaseGame.T_MUL);
		dot = BaseGame.Get().models.GetModel("Content\\Gift\\GiftDot", copyData: false, copyEPC: true);
		BaseGame.SetAllEPCs(dot.epc, "xEnableLighting", false);
		BaseGame.SetAllEPCs(dot.epc, "TextureMix", BaseGame.T_MUL);
	}

	public Gift(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
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
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		size = LevelLoader.GetFloatFromAtt(attributes, "size", 5f);
		waitAmount = LevelLoader.GetFloatFromAtt(attributes, "openwait", 4f);
	}

	public Gift(PathList _pathList, float _size, float _waitAmount, FillMode _fill)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(_pathList, _size, _waitAmount, _fill, _oluMode: false);
	}

	public Gift(PathList _pathList, float _size, float _waitAmount, FillMode _fill, bool _oluMode)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		pathList = _pathList;
		size = _size;
		waitAmount = _waitAmount;
		fillMode = _fill;
		oluMode = _oluMode;
	}

	public Matrix BoxTransformation()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateRotationX(MathHelper.ToRadians(90f)) * Matrix.CreateScale(size) * Matrix.CreateTranslation(staticPos);
	}

	public override Matrix Transformation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateScale(size) * Matrix.CreateTranslation(pos);
	}

	public override void draw(GameTime gametime)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I4
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = fillMode;
		if ((int)fillMode == 2)
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		}
		if (state < 3)
		{
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(BoxTransformation());
			BaseGame.Get().DrawModel(ref modelLocal);
			BaseGame.Get().matStack.PopMatrix();
		}
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.Get().DrawModel(ref dotLocal);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
	}

	public override void hit(TargetEffectBase toHit)
	{
		base.hit(toHit);
	}

	public override void die()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().ps.AddParticles(getPos(), Vector3.Forward * 25f, 2f, 180f, Vector3.Zero, 0f, 0.25f, 0.1f, 0.2f, new Vector4(1f, 1f, 1f, 1f), 80, 0.0005f);
		base.die();
	}

	public override void act(GameTime gametime)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		base.act(gametime);
		if (!exists)
		{
			return;
		}
		((GameComponent)anim).Update(gametime);
		openCond.Update();
		if (state == 0)
		{
			staticPos = pos;
			if (openCond.ConditionsMet())
			{
				state++;
				openCond.set.Clear();
				openCond.set.Add(new TimeCondition(4.0));
				openCond.Start();
				openAnim = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["opening"], component: false);
				((GameComponent)openAnim).Update(BaseGame.Get().emptytime);
				BaseGame.RunController(anim, openAnim);
				pathList = new PathList();
				pathList.Add(new PBezier(staticPos, staticPos + new Vector3(0f, 20f, 0.05f), BaseGame.Get().playerPos + new Vector3(0f, 0f, 15f), BaseGame.Get().playerPos, 0.1f, Vector3.Up, 0f, 0f, 0f, 0, 0f, 0.0, 0.0));
				pathList.ResetCurrent();
			}
		}
		else if (state == 1)
		{
			((GameComponent)openAnim).Update(gametime);
			if (openCond.ConditionsMet())
			{
				openAnim = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["fall"], component: false);
				((GameComponent)openAnim).Update(BaseGame.Get().emptytime);
				openCond.set.Clear();
				openCond.set.Add(new TimeCondition(1.7999999523162842));
				openCond.Start();
				BaseGame.RunController(anim, openAnim);
				state++;
			}
		}
		else if (state == 2)
		{
			((GameComponent)openAnim).Update(gametime);
			if (openCond.ConditionsMet())
			{
				state++;
			}
		}
		PlayerHit();
		pCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (pCooldown <= 0f)
		{
			pCooldown += pMax;
		}
	}

	public override void start()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		modelLocal = new ModelWrapper(model, copyEPC: true);
		dotLocal = new ModelWrapper(dot, copyEPC: true);
		addTarget(Vector3.Zero, 2, 10);
		addCond(new NeverCondition());
		anim = new ModelOluAnimator(BaseGame.Get().CoreGame, modelLocal, BaseGame.GetFogEffect());
		openAnim = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["closed"], component: false);
		((GameComponent)openAnim).Update(BaseGame.Get().emptytime);
		BaseGame.RunController(anim, openAnim);
		((GameComponent)anim).Update(BaseGame.Get().emptytime);
		if (oluMode)
		{
			EffectParameterCollectionRedux[] epc = modelLocal.epc;
			Color green = Color.Green;
			BaseGame.SetAllEPCs(epc, "DiffuseColor", ((Color)(ref green)).ToVector3());
			BaseGame.SetAllEPCs(modelLocal.epc, "xEnableLighting", false);
			EffectParameterCollectionRedux[] epc2 = dotLocal.epc;
			Color lightGreen = Color.LightGreen;
			BaseGame.SetAllEPCs(epc2, "DiffuseColor", ((Color)(ref lightGreen)).ToVector3());
		}
		openCond.set.Add(new TimeCondition(waitAmount));
		openCond.Start();
		base.start();
		staticPos = pos;
	}

	public override Vector3 getPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return pos;
	}

	public override string name()
	{
		return "[paq_1t]";
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
