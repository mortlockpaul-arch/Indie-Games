using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class Chand : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public static Dictionary<ModelBone, int> cBones;

	public static ModelWrapper model;

	public Vector3 vel;

	public Vector3 up;

	public float maxSpeed;

	public float accel;

	public ModelOluAnimator anim;

	public AnimationController downAnim;

	public AnimationController upAnim;

	public ConditionSet spinCond;

	public float pCooldown;

	public float pMax;

	public Vector3 revVel;

	public float size;

	public float rotAmount;

	public float rotSpeed;

	public float spinDelay;

	public float maxRot;

	public float blendAmount => Math.Min(rotSpeed / maxRot, 1f) * Math.Min(rotSpeed / maxRot, 1f);

	public Chand()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		pMax = 0.05f;
		maxRot = 720f;
		base._002Ector();
		state = 0;
		attackCooldown = 5f;
		hitPoints = 2;
		vel = Vector3.Forward;
		revVel = -vel;
		up = Vector3.Up;
		maxSpeed = 40f;
		accel = 2f;
		addCond(new ZReversePosCondition(100f, this));
		addCond(new ZPosCondition(0f, this));
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
		spinCond = new ConditionSet();
		rotAmount = 0f;
		rotSpeed = 0f;
	}

	public static void LoadModel()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		model = BaseGame.Get().models.GetModel("Content\\Chand\\Chand");
		BaseGame.SetAllEPCs(model.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(model.epc, "DirLight0Direction", (object)new Vector3(-0.5f, -0.5f, -1f));
		int modelIndex = BaseGame.GetModelIndex(model, "Sphere");
		BaseGame.SetAllEPCs(model.epc, "xGlow", false);
		model.epc[modelIndex]["xGlow"] = true;
		model.epc[modelIndex]["xEnableLighting"] = false;
		cBones = new Dictionary<ModelBone, int>();
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)model.model.Bones).Count; i++)
		{
			if (!cBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i]))
			{
				cBones.Add(((ReadOnlyCollection<ModelBone>)(object)model.model.Bones)[i], i);
			}
		}
	}

	public Chand(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
		size = LevelLoader.GetFloatFromAtt(attributes, "size", 5f);
		spinDelay = LevelLoader.GetFloatFromAtt(attributes, "delay", 3f);
	}

	public override Matrix Transformation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateScale(size) * Matrix.CreateTranslation(getPos());
	}

	public override void draw(GameTime gametime)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateRotationY(MathHelper.ToRadians(rotAmount)) * Transformation());
		BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
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
		base.act(gametime);
		if (!exists)
		{
			return;
		}
		BaseGame.RunController(anim, downAnim, upAnim, blendAmount);
		((GameComponent)anim).Update(gametime);
		spinCond.Update();
		if (state == 0 && spinCond.ConditionsMet())
		{
			state++;
			spinCond.set.Clear();
			spinCond.set.Add(new TimeCondition(6.0));
			spinCond.Start();
		}
		else if (state == 1)
		{
			rotSpeed += 180f * (float)gametime.ElapsedGameTime.TotalSeconds;
			rotSpeed = Math.Min(rotSpeed, maxRot);
			if (spinCond.ConditionsMet())
			{
				state++;
			}
		}
		else if (state == 2 && rotSpeed > 0f)
		{
			rotSpeed -= 180f * (float)gametime.ElapsedGameTime.TotalSeconds;
			if (rotSpeed < 0f)
			{
				rotSpeed = 0f;
			}
		}
		rotAmount += rotSpeed * (float)gametime.ElapsedGameTime.TotalSeconds;
		pCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (pCooldown <= 0f)
		{
			pCooldown += pMax;
		}
	}

	public override void start()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		addTarget(Vector3.Zero, 2, 10);
		addCond(new TimeCondition(spinDelay));
		anim = new ModelOluAnimator(BaseGame.Get().CoreGame, model, BaseGame.GetFogEffect());
		downAnim = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["down"], component: false);
		upAnim = new AnimationController(BaseGame.Get().CoreGame, anim.Animations["up"], component: false);
		BaseGame.RunController(anim, downAnim, upAnim, blendAmount);
		((GameComponent)anim).Update(BaseGame.Get().emptytime);
		((GameComponent)downAnim).Update(BaseGame.Get().emptytime);
		((GameComponent)upAnim).Update(BaseGame.Get().emptytime);
		spinCond.set.Add(new TimeCondition(spinDelay));
		spinCond.Start();
		base.start();
	}

	public override string name()
	{
		return "[ch@nd_util]";
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

	public override Enemy attack()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		BulletC bulletC = new BulletC(getPos(), BaseGame.GetRandVect(Vector3.Normalize(BaseGame.Get().playerPos - getPos()) * 10f, (float)Math.PI / 4f), Vector3.Up, this, 0);
		bulletC.fillMode = (FillMode)3;
		bulletC.start();
		bulletC.Launch();
		attackCond.set.Clear();
		attackCond.set.Add(new NeverCondition());
		attackCond.Start();
		return bulletC;
	}
}
