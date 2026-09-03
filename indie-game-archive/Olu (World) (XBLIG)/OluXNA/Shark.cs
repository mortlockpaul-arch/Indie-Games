using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class Shark : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public static ModelWrapper headModel;

	public static PlaneEffect pE;

	public List<EnemyState> states;

	private int childrenHP;

	private int threshold;

	public int numTails;

	private Vector3 velocity;

	private Vector3 facingDirection;

	private float size;

	private int leaveCountdown;

	private float phaseCountdown;

	private float phaseMax;

	private float waterHeight;

	private bool underWater;

	private bool oldWater;

	public List<RippleEffect> rE;

	public Vector3 ripplePos;

	public Vector3 origPos;

	public Vector3 offset;

	private Vector3 up;

	public SharkTail child;

	public Vector3 Velocity
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return velocity;
		}
	}

	public bool CreateRipple => underWater ^ oldWater;

	public Vector3 actualPos
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return origPos + offset;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			origPos = value;
			offset = Vector3.Zero;
		}
	}

	public Shark()
	{
		state = 0;
		attackCooldown = 5f;
		hitPoints = 84;
		threshold = 40;
		leaveCountdown = 6;
		if (wCond == null)
		{
			wCond = new Dictionary<int, WaitCond>();
			wCond.Add(0, new WaitCond("Boop01", Beats.Quarter));
			wCond.Add(1, new WaitCond("Boop01", Beats.Eighth));
			wCond.Add(2, new WaitCond("Boop01", Beats.Eighth));
			wCond.Add(3, new WaitCond("Boop02", Beats.Eighth, Beats.Quarter));
			wCond.Add(4, new WaitCond("Boop01", Beats.Quarter));
			wCond.Add(5, new WaitCond("Boop01", Beats.Eighth));
			wCond.Add(6, new WaitCond("Boop02", Beats.Eighth, Beats.Quarter));
			wCond.Add(7, new WaitCond("Boop01", Beats.Eighth));
			wCond.Add(8, new WaitCond("Boop01", Beats.Eighth));
		}
		_eCond = wCond;
		oldWater = false;
		rE = new List<RippleEffect>();
		states = new List<EnemyState>();
	}

	public Shark(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		Initialize(LevelLoader.GetVectorFromAtt(attributes, "up", new Vector3(0f, 1f, 0f)));
		size = LevelLoader.GetFloatFromAtt(attributes, "size", 1f);
		waterHeight = LevelLoader.GetFloatFromAtt(attributes, "waterheight", 0f);
		numTails = LevelLoader.GetIntFromAtt(attributes, "numtails", 16);
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
	}

	protected void Initialize(Vector3 _up)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		up = _up;
	}

	public static void LoadModel()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		headModel = BaseGame.Get().models.GetModel("Content\\Shark\\SharkHead");
		BaseGame.Get().LinkEffect(headModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.Get().fogEffect);
		BaseGame.SetAllEPCs(headModel.epc, "xEnableLighting", true);
		BaseGame.SetAllEPCs(headModel.epc, "TextureMix", BaseGame.T_MUL);
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
	}

	public override Matrix Transformation()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		return BaseGame.MapObjectToSystem2(Vector3.Zero, getDir(), up) * Matrix.CreateScale(1f, 1f, 1f) * Matrix.CreateScale(size) * Matrix.CreateTranslation(getPos());
	}

	public override void draw(GameTime gametime)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Water");
		BaseGame.Get().fogEffect.Parameters["WaterHeight"].SetValue(waterHeight);
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		BaseGame.Get().DrawModel(ref headModel);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		for (int num = rE.Count - 1; num >= 0; num--)
		{
			if (!rE[num].done)
			{
				BaseGame.Get().matStack.PushMatrix();
				rE[num].Draw(gametime);
				BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(120f) * Matrix.CreateTranslation(rE[num].pos));
				BaseGame.Get().fogEffect.Begin();
				BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
				pE.draw();
				BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
				BaseGame.Get().fogEffect.End();
				BaseGame.Get().matStack.PopMatrix();
			}
		}
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
	}

	public void HitMeh(int damage)
	{
		childrenHP -= damage;
		threshold -= damage;
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().ps.AddParticles(Vector3.Transform(new Vector3(0f, 2.4f, 0.6f), Transformation()), Vector3.Forward * 25f, 2f, 180f, Vector3.Zero, 0f, 0.35f, 0.1f, 0.2f, new Vector4(0.5f, 0.5f, 1f, 1f), 20, 0.0005f);
		threshold--;
		if (toHit.fillMode != toHit.eTarget.fillMode)
		{
			threshold--;
		}
		base.hit(toHit);
		if (child != null)
		{
			hitPoints = 84;
		}
		if (targets.Count > 0)
		{
			targets[0].hp = hitPoints;
		}
	}

	public override void die()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().ps.AddParticles(getPos(), Vector3.Forward * 25f, 2f, 180f, Vector3.Zero, 0f, 0.35f, 0.2f, 0f, new Vector4(0.8f, 0.8f, 1f, 1f), 200, 5E-05f);
		base.die();
	}

	private void SlowlyUpdateDirection()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		facingDirection = Vector3.Normalize(Vector3.Transform(facingDirection, Matrix.CreateFromAxisAngle(Vector3.Cross(facingDirection, Vector3.Normalize(velocity)), (float)Math.Acos(Vector3.Dot(facingDirection, Vector3.Normalize(velocity))) * 0.04f)));
		up = Vector3.Normalize(Vector3.Cross(facingDirection, Vector3.Right));
	}

	public override void act(GameTime gametime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		oldWater = underWater;
		if (getPos().Y > waterHeight)
		{
			underWater = false;
		}
		else
		{
			underWater = true;
		}
		if (CreateRipple)
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
		foreach (EnemyState state in states)
		{
			state.Update(gametime);
		}
		for (int num2 = states.Count - 1; num2 >= 0; num2--)
		{
			if (states[num2].Remove())
			{
				states.Add(states[num2].GetNewState());
				states.RemoveAt(num2);
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
			child.AddVel(new TimeMarker((float)gametime.ElapsedGameTime.TotalSeconds, velocity, facingDirection, up));
		}
		offset += velocity * (float)gametime.ElapsedGameTime.TotalSeconds;
	}

	public override void leave()
	{
		if (child != null)
		{
			child.leave();
		}
		base.leave();
	}

	public override void start()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		addTarget(new Vector3(0f, 2.4f, 0.6f), 240, 1, waterHeight);
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
		velocity = new Vector3(0f, 0f, 5f);
		setPos(new Vector3(50f, -40f, 30f));
		states.Add(SurfaceState());
		childrenHP = numTails * 24;
		child = new SharkTail(waterHeight, size, up, numTails, numTails, null, this, 0.05f, velocity, facingDirection, getPos());
		child.start();
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
		return "[shark]";
	}

	public Vector3 getDir()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return facingDirection;
	}

	public override Vector3 getPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return actualPos;
	}

	public override void setPos(Vector3 _pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		actualPos = _pos;
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

	private EnemyState SurfaceState()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(null, null, null, SideState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(2.0));
		enemyState.condSet.Start();
		leaveCountdown--;
		velocity = new Vector3(0f, 30f, 50f);
		facingDirection = new Vector3(0f, 0f, 1f);
		up = new Vector3(0f, 1f, 0f);
		threshold = 40;
		setPos(new Vector3((origPos.X < 0f) ? (-50f) : 50f, -40f, 30f));
		setPos(getPos() - 2f * velocity);
		if (child != null)
		{
			child.ClearVel();
			child.SetupPos(getPos(), facingDirection, velocity, up, 0.02f);
			child.FollowModeStop();
		}
		return enemyState;
	}

	private EnemyState SideState()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(SideUpdate, null, SideRemove, DiveState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(15.0));
		enemyState.condSet.Start();
		velocity = new Vector3(0f, 0f, 5f);
		setPos(new Vector3((origPos.X < 0f) ? (-50f) : 50f, -40f, 30f));
		if (child != null)
		{
			child.ClearVel();
			child.SetupPos(getPos(), facingDirection, velocity, up, 0.02f);
		}
		return enemyState;
	}

	private void SideUpdate(GameTime gametime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (BaseGame.Get().r.NextDouble() < 0.005)
		{
			velocity = BaseGame.GetRandVect(Vector3.Up, 180f) * (float)BaseGame.Get().r.NextDouble() * 10f;
		}
		if ((offset.Z > 30f && velocity.Z > 0f) || (offset.Z < -20f && velocity.Z < 0f))
		{
			ref Vector3 reference = ref velocity;
			reference.Z *= -1f;
		}
		else if ((offset.Y > 0f && velocity.Y > 0f) || (offset.Y < -25f && velocity.Y < 0f))
		{
			ref Vector3 reference2 = ref velocity;
			reference2.Y *= -1f;
		}
		else if ((offset.X > 10f && velocity.X > 0f) || (offset.X < -10f && velocity.X < 0f))
		{
			ref Vector3 reference3 = ref velocity;
			reference3.X *= -1f;
		}
	}

	private bool SideRemove(ConditionSet cs)
	{
		if ((float)threshold <= 0f || cs.ConditionsMet())
		{
			return true;
		}
		return false;
	}

	private EnemyState DiveState()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(DiveUpdate, null, null, MoveForwardState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(8.0));
		enemyState.condSet.Start();
		Vector3 val = offset;
		val.Y = 70f;
		val.Z += 60f;
		velocity = val - offset;
		velocity /= 2f;
		if (child != null)
		{
			child.FollowModeStart(getPos(), facingDirection, velocity, up);
		}
		return enemyState;
	}

	private void DiveUpdate(GameTime gametime)
	{
		SlowlyUpdateDirection();
		if (velocity.Y > -100f)
		{
			ref Vector3 reference = ref velocity;
			reference.Y -= 25f * (float)gametime.ElapsedGameTime.TotalSeconds;
		}
	}

	private EnemyState MoveForwardState()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(null, null, null, JumpState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new AlwaysCondition());
		enemyState.condSet.Start();
		origPos.X = ((actualPos.X < 0f) ? (-50f) : 50f);
		origPos.Y = waterHeight - 15f;
		origPos.Z = 120f;
		offset = Vector3.Zero;
		return enemyState;
	}

	private EnemyState JumpState()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = new EnemyState(JumpUpdate, null, null, MoveBackwardState);
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(8.0));
		enemyState.condSet.Start();
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(0f, waterHeight + 50f, actualPos.Z);
		velocity = val - actualPos;
		ref Vector3 reference = ref velocity;
		reference.X /= 2f;
		facingDirection = velocity;
		up = new Vector3(0f, 0f, -1f);
		if (child != null)
		{
			child.ClearVel();
			child.SetupPos(getPos(), facingDirection, velocity, up, 0.02f);
			child.FollowModeStart(getPos(), facingDirection, velocity, up);
		}
		phaseMax = (phaseCountdown = 1.5f);
		return enemyState;
	}

	private void JumpUpdate(GameTime gametime)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (velocity.Y > -100f)
		{
			ref Vector3 reference = ref velocity;
			reference.Y -= 25f * (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		facingDirection = velocity;
		if (getPos().Y > 20f)
		{
			phaseCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			if (phaseCountdown < phaseMax)
			{
				BulletGlow bulletGlow = new BulletGlow(getPos(), size * 0.6f, 2, pineMode: false);
				bulletGlow.start();
				BaseGame.Get().enems.Add(bulletGlow);
				phaseCountdown += phaseMax;
			}
		}
	}

	private EnemyState MoveBackwardState()
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState;
		if (child != null)
		{
			enemyState = ((!(origPos.Z < 80f)) ? new EnemyState(null, null, null, JumpState) : ((leaveCountdown <= 0) ? new EnemyState(null, null, null, LeaveState) : new EnemyState(null, null, null, SurfaceState)));
		}
		else
		{
			enemyState = new EnemyState(null, null, null, JumpDiagonalState);
			leaveCountdown = 6;
		}
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new AlwaysCondition());
		enemyState.condSet.Start();
		origPos.X = ((actualPos.X < 0f) ? (-50f) : 50f);
		origPos.Y = waterHeight - 50f;
		origPos.Z = actualPos.Z - 25f;
		offset = Vector3.Zero;
		return enemyState;
	}

	private EnemyState JumpDiagonalState()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		EnemyState enemyState = ((leaveCountdown > 0) ? new EnemyState(JumpDiagonalUpdate, null, null, JumpDiagonalState) : new EnemyState(null, null, null, LeaveState));
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new TimeCondition(7.5));
		enemyState.condSet.Start();
		leaveCountdown--;
		origPos.X = ((actualPos.X < 0f) ? (-50f) : 50f);
		origPos.Y = waterHeight - 50f;
		origPos.Z = 40f;
		offset = Vector3.Zero;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(actualPos.X, waterHeight + 30f, actualPos.Z + 25f);
		val.X += ((actualPos.X > 0f) ? (-25f) : 25f);
		velocity = val - actualPos;
		facingDirection = Vector3.Normalize(velocity);
		up = Vector3.Cross(facingDirection, new Vector3(0f, 1f, 0f));
		up = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(up), facingDirection));
		if (child != null)
		{
			child.ClearVel();
			child.SetupPos(getPos(), facingDirection, velocity, up, 0.02f);
			child.FollowModeStart(getPos(), facingDirection, velocity, up);
		}
		return enemyState;
	}

	private void JumpDiagonalUpdate(GameTime gametime)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (velocity.Y > -100f)
		{
			ref Vector3 reference = ref velocity;
			reference.Y -= 25f * (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		facingDirection = velocity;
		up = Vector3.Cross(facingDirection, new Vector3(0f, 1f, 0f));
		up = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(up), facingDirection));
	}

	private EnemyState LeaveState()
	{
		EnemyState enemyState = new EnemyState(null, null, null, LeaveState);
		leave();
		enemyState.condSet = new ConditionSet();
		enemyState.condSet.set.Add(new NeverCondition());
		enemyState.condSet.Start();
		return enemyState;
	}
}
