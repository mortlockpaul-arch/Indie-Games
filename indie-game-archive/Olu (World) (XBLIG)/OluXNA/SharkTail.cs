using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class SharkTail : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public static ModelWrapper tailModel;

	public static PlaneEffect[] pE;

	public static int numIndices;

	public static PlaneDetachColl pdColl;

	private bool followHeadSpeed;

	private float waterHeight;

	private bool underWater;

	private bool oldWater;

	public Shark head;

	public SharkTail child;

	public SharkTail parent;

	private float size;

	private float waitTime;

	private Vector3 velocity;

	private Vector3 facingDir;

	private Vector3 actualPos;

	private Queue<TimeMarker> velQueue;

	public List<RippleEffect> rE;

	public Vector3 ripplePos;

	public int planeIndex;

	private Vector3 up;

	public bool CreateRipple => underWater ^ oldWater;

	public float speedWeight
	{
		get
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (!followHeadSpeed)
			{
				return 1f;
			}
			Vector3 val = head.Velocity;
			return ((Vector3)(ref val)).Length() / ((Vector3)(ref velocity)).Length();
		}
	}

	public SharkTail()
	{
		state = 0;
		attackCooldown = 5f;
		hitPoints = 24;
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
		rE = new List<RippleEffect>();
		velQueue = new Queue<TimeMarker>();
		followHeadSpeed = false;
	}

	public SharkTail(float _waterHeight, float _size, Vector3 _up, int _numTails, int _tailsLeft, SharkTail _parent, Shark _head, float _waitTime, Vector3 direction, Vector3 _facingDir, Vector3 position)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		waterHeight = _waterHeight;
		if (_tailsLeft == 1)
		{
			size = _size;
		}
		else
		{
			size = (0.9f * (float)_tailsLeft / (float)_numTails + 0.1f) * _size;
		}
		fillMode = (FillMode)2;
		parent = _parent;
		head = _head;
		if (parent != null)
		{
			parent.child = this;
		}
		velocity = direction;
		setPos(position);
		facingDir = _facingDir;
		up = _up;
		velQueue.Enqueue(new TimeMarker(_waitTime, direction, _facingDir, up));
		waitTime = _waitTime;
		planeIndex = BaseGame.Get().r.Next(12);
		BaseGame.Get().enems.Add(this);
		if (_tailsLeft > 1)
		{
			new SharkTail(_waterHeight, _size, _up, _numTails, _tailsLeft - 1, this, _head, waitTime + 0.05f, direction, _facingDir, position + Vector3.Normalize(-direction) * size * 0.6f);
		}
	}

	public static void LoadModel()
	{
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		tailModel = BaseGame.Get().models.GetModel("Content\\Shark\\SharkTail", copyData: true, copyEPC: false);
		BaseGame.Get().LinkEffect(tailModel.model, BaseGame.Get().graphics.GraphicsDevice, BaseGame.Get().fogEffect);
		BaseGame.SetAllEPCs(tailModel.epc, "xEnableLighting", false);
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
		pdColl = new PlaneDetachColl(ref tailModel);
		numIndices = ((ReadOnlyCollection<ModelMesh>)(object)tailModel.model.Meshes)[0].IndexBuffer.SizeInBytes / 2;
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
		return BaseGame.MapObjectToSystem2(Vector3.Zero, getDir(), up) * Matrix.CreateScale(size) * Matrix.CreateTranslation(getPos());
	}

	public override void draw(GameTime gametime)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Water");
		BaseGame.Get().fogEffect.Parameters["WaterHeight"].SetValue(waterHeight);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = fillMode;
		BaseGame.Get().DrawModel(ref tailModel);
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
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
				pE[planeIndex].draw();
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
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().ps.AddParticles(getPos(), Vector3.Forward * 25f, 2f, 180f, Vector3.Zero, 0f, 0.35f, 0.1f, 0.2f, new Vector4(0.3f, 0.3f, 1f, 1f), 20, 0.0005f);
		base.hit(toHit);
	}

	public override void die()
	{
		for (int i = 0; i < numIndices; i += 3)
		{
			pdColl.AddPlane(ref tailModel, 0, i, this, (FillMode)2);
		}
		if (parent != null)
		{
			parent.child = null;
		}
		else
		{
			head.child = null;
		}
		parent = null;
		if (child != null)
		{
			child.die();
		}
		child = null;
		base.die();
	}

	public void AddVel(TimeMarker toAdd)
	{
		velQueue.Enqueue(toAdd);
		if (child != null)
		{
			child.AddVel(toAdd);
		}
	}

	public void ClearVel()
	{
		velQueue.Clear();
		if (child != null)
		{
			child.ClearVel();
		}
	}

	public void SetupPos(Vector3 par_position, Vector3 par_facingDir, Vector3 par_direction, Vector3 par_up, float _waitTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		velocity = par_direction;
		setPos(par_position);
		facingDir = par_facingDir;
		up = par_up;
		velQueue.Enqueue(new TimeMarker(_waitTime, par_direction, par_facingDir, par_up));
		if (child != null)
		{
			child.SetupPos(getPos() + Vector3.Normalize(-velocity) * size * 0.6f, par_facingDir, par_direction, par_up, _waitTime);
		}
	}

	public void FollowModeStart(Vector3 par_position, Vector3 par_facingDir, Vector3 par_direction, Vector3 par_up)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = par_position - getPos();
		if (((Vector3)(ref val)).LengthSquared() < 0.001f)
		{
			val = Vector3.Normalize(par_direction) * 0.001f;
		}
		float num = ((Vector3)(ref par_direction)).Length();
		float g = ((Vector3)(ref val)).Length() / num;
		val = Vector3.Normalize(val) * num;
		Vector3 val2 = Vector3.Cross(val, up);
		val2 = Vector3.Cross(val2, val);
		Vector3 par_direction2 = Vector3.Normalize(velocity) * num;
		followHeadSpeed = true;
		if (child != null)
		{
			child.FollowModeStart(getPos(), facingDir, par_direction2, up);
		}
		AddVel(new TimeMarker(g, val, par_facingDir, val2));
	}

	public void FollowModeStop()
	{
		followHeadSpeed = false;
		if (child != null)
		{
			child.FollowModeStop();
		}
	}

	public override void act(GameTime gametime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
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
			rE.Add(new RippleEffect(ripplePos, 0.5f, 0.5f, 0f, 0.3f, 1f, _loop: false, 0f));
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
		float num2 = waitTime;
		waitTime -= (float)gametime.ElapsedGameTime.TotalSeconds * speedWeight;
		if (waitTime > 0f)
		{
			actualPos += velocity * (float)gametime.ElapsedGameTime.TotalSeconds * speedWeight;
		}
		else
		{
			actualPos += velocity * num2;
		}
		while (waitTime <= 0f && velQueue.Count > 0)
		{
			velQueue.Dequeue();
			if (velQueue.Count > 0)
			{
				waitTime /= speedWeight;
				UpdateDirection(velQueue.Peek());
				waitTime *= speedWeight;
				num2 = waitTime;
				waitTime += velQueue.Peek().gameTime;
				if (waitTime > 0f)
				{
					actualPos += velocity * (0f - num2);
				}
				else
				{
					actualPos += velocity * velQueue.Peek().gameTime;
				}
			}
		}
		if (velQueue.Count == 0)
		{
			waitTime = 0f;
		}
	}

	public override void leave()
	{
		if (child != null)
		{
			child.leave();
		}
		base.leave();
	}

	public void UpdateDirection(TimeMarker tm)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		up = tm.up;
		velocity = tm.direction;
		facingDir = tm.facingDir;
	}

	public override void start()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		addTarget(new Vector3(0f, 0f, -0.5f), 24, 10, waterHeight);
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
		if (child != null)
		{
			child.start();
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
		return "[shark_tail]";
	}

	public Vector3 getDir()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return facingDir;
	}

	public override Vector3 getPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return actualPos;
	}

	public override void setPos(Vector3 _pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
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
}
