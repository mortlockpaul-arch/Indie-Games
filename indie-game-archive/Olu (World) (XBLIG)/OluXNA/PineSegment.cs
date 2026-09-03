using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class PineSegment : Enemy
{
	public Pine head;

	public PineSegment parent;

	public PineSegment child;

	public ModelWrapper bodyModel;

	public ModelWrapper legModel;

	public ModelOluAnimator legAnim;

	public AnimationController legAC;

	private Vector3 velocity;

	private Vector3 facingDir;

	private Vector3 up;

	private Queue<TimeMarker> velQueue;

	private float waitTime;

	private Target recentHit;

	private bool tail;

	public float speedWeight => 1f;

	public PineSegment(Pine p_head, PineSegment _parent, int gensLeft, Vector3 _pos, Vector3 vel, Vector3 dir, Vector3 _up, float _waitTime)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		hitPoints = 36;
		head = p_head;
		tail = false;
		if (gensLeft <= 0)
		{
			tail = true;
		}
		velocity = vel;
		pos = _pos;
		facingDir = dir;
		up = _up;
		if (!tail)
		{
			bodyModel = new ModelWrapper(Pine.bodyModel);
			legModel = new ModelWrapper(Pine.g_legModel);
			legAnim = new ModelOluAnimator(BaseGame.Get().CoreGame, legModel, BaseGame.GetFogEffect());
			legAC = new AnimationController(BaseGame.Get().CoreGame, legAnim.Animations[0]);
			double num = legAnim.Animations[0].Duration / 1000;
			num = BaseGame.Get().r.NextDouble() * num;
			((GameComponent)legAC).Update(new GameTime(BaseGame.Get().emptytime.TotalRealTime, BaseGame.Get().emptytime.ElapsedRealTime, BaseGame.Get().emptytime.TotalGameTime, new TimeSpan(0, 0, 0, (int)num, (int)(num * 100.0))));
			BaseGame.RunController(legAnim, legAC);
			addTarget(new Vector3(0f, 1f, 0f), 36, 5, ref legModel, 0, "Armature_LeftArm02", this);
			addTarget(new Vector3(0f, 1f, 0f), 36, 5, ref legModel, 0, "Armature_RightArm02", this);
		}
		else
		{
			bodyModel = new ModelWrapper(Pine.tailModel);
			addTarget(new Vector3(0f, 0.7f, -2.4f), 36, 5, ref bodyModel, 0, -1, this);
		}
		waitTime = _waitTime;
		parent = _parent;
		if (!tail)
		{
			child = new PineSegment(p_head, this, gensLeft - 1, pos + Vector3.Normalize(-facingDir) * head.size, velocity, facingDir, up, waitTime + 0.1f);
			BaseGame.Get().enems.Add(child);
		}
		velQueue = new Queue<TimeMarker>();
		velQueue.Enqueue(new TimeMarker(waitTime, velocity, facingDir, up));
	}

	public override Matrix Transformation()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateScale(head.size) * Matrix.CreateTranslation(getPos());
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		recentHit = toHit.eTarget;
		BaseGame.Get().ps.AddParticles(recentHit.absolutePos(), Vector3.Forward * 40f, 2f, 180f, Vector3.Zero, 0f, 0.55f, 0.2f, 0.2f, new Vector4(0.3f, 0.3f, 1f, 1f), 20, 0.0005f);
		base.hit(toHit);
	}

	public override void die()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		if (parent != null)
		{
			parent.child = child;
		}
		else
		{
			head.child = child;
		}
		if (child != null)
		{
			child.parent = parent;
			child.AddVel(new TimeMarker(0.25f, new Vector3(0f, 0f, 200f), facingDir, up));
		}
		if (recentHit != null)
		{
			for (int i = 0; i < 12; i++)
			{
				Vector3 val = recentHit.absolutePos();
				Vector3 val2 = BaseGame.GetRandVect(new Vector3(0f, 0f, -1f), 55f) * head.size * 10f;
				val2.Z /= 2f;
				BulletC bulletC = new BulletC(val, val2, Vector3.Normalize(Vector3.Cross(val2, Vector3.Right)), this, 0);
				bulletC.start();
				bulletC.Launch();
				BaseGame.Get().enems.Add(bulletC);
			}
		}
		parent = null;
		child = null;
		head.SegmentHit();
		base.die();
	}

	public override void start()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos;
		addCond(new NeverCondition());
		base.start();
		pos = val;
		if (child != null)
		{
			child.start();
		}
	}

	public override string name()
	{
		return "[p1ne]";
	}

	public override void HitSound(int lockNum, float volume)
	{
		head.HitSound(lockNum, volume);
	}

	public override void act(GameTime gametime)
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		if (!tail)
		{
			((GameComponent)legAC).Update(gametime);
			((GameComponent)legAnim).Update(gametime);
		}
		float num = waitTime;
		waitTime -= (float)gametime.ElapsedGameTime.TotalSeconds * speedWeight;
		if (waitTime > 0f)
		{
			pos += velocity * (float)gametime.ElapsedGameTime.TotalSeconds * speedWeight;
		}
		else
		{
			pos += velocity * num;
		}
		while (waitTime <= 0f && velQueue.Count > 0)
		{
			velQueue.Dequeue();
			if (velQueue.Count > 0)
			{
				waitTime /= speedWeight;
				UpdateDirection(velQueue.Peek());
				waitTime *= speedWeight;
				num = waitTime;
				waitTime += velQueue.Peek().gameTime;
				if (waitTime > 0f)
				{
					pos += velocity * (0f - num);
				}
				else
				{
					pos += velocity * velQueue.Peek().gameTime;
				}
			}
		}
		if (velQueue.Count == 0)
		{
			waitTime = 0f;
			velocity = Vector3.Zero;
		}
	}

	public override void draw(GameTime gametime)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Transformation());
		if (!tail)
		{
			BaseGame.Get().DrawModel(ref legModel);
		}
		BaseGame.Get().DrawModel(ref bodyModel);
		BaseGame.Get().matStack.PopMatrix();
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
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		velocity = par_direction;
		setPos(par_position);
		facingDir = par_facingDir;
		up = par_up;
		velQueue.Enqueue(new TimeMarker(_waitTime, par_direction, par_facingDir, par_up));
		if (child != null)
		{
			child.SetupPos(getPos() + Vector3.Normalize(-velocity) * head.size, par_facingDir, par_direction, par_up, _waitTime);
		}
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
}
