using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class OluSnake : Enemy
{
	public static Dictionary<int, WaitCond> wCond;

	public BezierHelper[] paths;

	public OluSnakeEnd snakeEnd;

	protected float countdown;

	protected Enemy parent;

	protected int bossPart;

	public OluSnake(Enemy _parent, int _bossPart)
	{
		state = 0;
		attackCooldown = 5f;
		hitPoints = 16;
		parent = _parent;
		bossPart = _bossPart;
		if (wCond == null)
		{
			wCond = new Dictionary<int, WaitCond>();
			wCond.Add(0, new WaitCond("Boop01", Beats.Quarter));
			wCond.Add(1, new WaitCond("Boop01", Beats.Eighth));
			wCond.Add(2, new WaitCond("Boop02", Beats.Eighth, Beats.Quarter));
			wCond.Add(3, new WaitCond("Boop02", Beats.Eighth, Beats.Quarter));
			wCond.Add(4, new WaitCond("Boop01", Beats.Eighth, Beats.Quarter));
			wCond.Add(5, new WaitCond("Boop02", Beats.Eighth));
			wCond.Add(6, new WaitCond("Boop02", Beats.Eighth));
			wCond.Add(7, new WaitCond("Boop01", Beats.Eighth));
			wCond.Add(8, new WaitCond("Boop01", Beats.Eighth));
		}
		_eCond = wCond;
	}

	public static void LoadModel()
	{
	}

	public override Matrix Transformation()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateScale(new Vector3(2f, 2f, 2f)) * Matrix.CreateTranslation(getPos());
	}

	public override void draw(GameTime gametime)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		if (pathList.curPathIndex < pathList.publicPaths.Count - 1)
		{
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
			BaseGame.Get().matStack.ApplyMatrix(Transformation());
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
			BaseGame.Get().DrawModel(ref Note.model);
			BaseGame.Get().matStack.PopMatrix();
		}
		DrawPath(gametime);
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		((Olu)parent).NoteHit(bossPart, (toHit.fillMode == fillMode) ? 1 : 2);
		base.hit(toHit);
	}

	public override void die()
	{
		base.die();
	}

	public override void act(GameTime gametime)
	{
		_ = (PBezier)pathList.publicPaths[pathList.curPathIndex];
		if (countdown >= 0f)
		{
			countdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		}
		if (countdown < 0f && snakeEnd == null)
		{
			snakeEnd = new OluSnakeEnd(this);
			snakeEnd.start();
		}
		if (snakeEnd != null)
		{
			snakeEnd.act(gametime);
		}
		base.act(gametime);
		_ = pathList.curPathIndex;
		_ = pathList.publicPaths.Count - 1;
		if (snakeEnd != null && pathList.curPathIndex == pathList.publicPaths.Count - 1 && snakeEnd.pathList.curPathIndex == snakeEnd.pathList.publicPaths.Count - 1)
		{
			snakeEnd.leave();
			snakeEnd = null;
			countdown = 2.5f;
			pathList.curPathIndex = 0;
			pathList.ResetCurrent();
		}
	}

	public void DrawPath(GameTime gametime)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		BaseGame.Get().SwitchEffectTechnique("Bezier");
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1f, -1f, -1f));
		int num = 0;
		if (snakeEnd != null)
		{
			num = snakeEnd.pathList.curPathIndex;
		}
		int curPathIndex = pathList.curPathIndex;
		BaseGame.SetAllEPCs(Olu.tail.epc, "DiffuseColor", (object)new Vector3(1f, 1f, 1f));
		for (int i = num; i <= curPathIndex && i < paths.Length; i++)
		{
			BaseGame.Get().fogEffect.Parameters["BezierShift"].SetValue(0f);
			if (i == num && snakeEnd != null)
			{
				BaseGame.Get().fogEffect.Parameters["BezierShift"].SetValue(0f - ((PBezier)snakeEnd.pathList.publicPaths[snakeEnd.pathList.curPathIndex]).GetProgress());
			}
			if (i == curPathIndex)
			{
				BaseGame.Get().fogEffect.Parameters["BezierShift"].SetValue(1f - ((PBezier)pathList.publicPaths[pathList.curPathIndex]).GetProgress());
			}
			BaseGame.Get().fogEffect.Parameters["BezierPos"].SetValue(paths[i].BezierPos);
			BaseGame.Get().fogEffect.Parameters["BezierVel"].SetValue(paths[i].BezierVel);
			BaseGame.Get().fogEffect.Parameters["Pos0"].SetValue(paths[i].pos[0]);
			BaseGame.Get().fogEffect.Parameters["Pos1"].SetValue(paths[i].pos[1]);
			BaseGame.Get().fogEffect.Parameters["Scale"].SetValue(paths[i].scale);
			BaseGame.Get().DrawModel(ref Olu.tail);
		}
		BaseGame.Get().fogEffect.Parameters["xPose"].SetValue(Matrix.Identity);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
	}

	public override void start()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		addTarget(Vector3.Zero, 32, 10);
		addCond(new NeverCondition());
		base.start();
		paths = new BezierHelper[pathList.publicPaths.Count - 1];
		countdown = 2.5f;
		for (int i = 0; i < pathList.publicPaths.Count - 1; i++)
		{
			PBezier pBezier = (PBezier)pathList.publicPaths[i];
			paths[i] = new BezierHelper(pBezier.pos[0], pBezier.pos[1], pBezier.pos[2], pBezier.pos[3]);
		}
		float num = 0f;
		for (int j = 0; j < paths.Length; j++)
		{
			num += paths[j].scale;
		}
		num /= (float)paths.Length;
		for (int k = 0; k < paths.Length; k++)
		{
			paths[k].scale = num;
		}
	}

	public override string name()
	{
		return "[0lLoup]";
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
