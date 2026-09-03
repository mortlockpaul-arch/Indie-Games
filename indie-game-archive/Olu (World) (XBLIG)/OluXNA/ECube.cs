using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class ECube : Enemy
{
	private List<EnemyCube> cubez;

	public static Dictionary<int, WaitCond> wCond;

	public bool oluMode;

	public ECube()
		: this(_oluMode: false)
	{
	}

	public ECube(bool _oluMode)
	{
		Initialize(_oluMode);
	}

	public void Initialize(bool _oluMode)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		state = 0;
		attackCooldown = 5f;
		hitPoints = 2;
		EnemyCube enemyCube = new EnemyCube();
		enemyCube.oluMode = _oluMode;
		EnemyCube enemyCube2 = new EnemyCube();
		enemyCube2.oluMode = _oluMode;
		enemyCube.createCube(10, 0, 5);
		enemyCube2.createCube(8, 5, 5);
		enemyCube.rotAxis = new Vector3(1f, 1f, 1f);
		enemyCube.rotDelta = (float)Math.PI / 5f;
		enemyCube.rotAngle = 0f;
		enemyCube2.rotAxis = new Vector3(1f, 1f, 1f);
		enemyCube2.rotDelta = -(float)Math.PI / 5f;
		enemyCube2.rotAngle = 0f;
		addCond(new ZPosCondition(0f, this));
		addCond(new ZReversePosCondition(100f, this));
		cubez = new List<EnemyCube>();
		cubez.Add(enemyCube);
		cubez.Add(enemyCube2);
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
		fillMode = (FillMode)2;
		oluMode = _oluMode;
	}

	public ECube(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
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
		Initialize(oluMode);
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out pathList, BaseGame.Get().level.activeZone);
	}

	public override void draw(GameTime gametime)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Colored");
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateTranslation(getPos()));
		float num = BaseGame.Get().curBeat % 8;
		num = num / 7f * (float)Math.PI;
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateScale(1.1f - 0.1f * (float)Math.Sin(num)));
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		for (int i = 0; i < cubez.Count; i++)
		{
			cubez[i].draw();
		}
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
	}

	public override void hit(TargetEffectBase toHit)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 6; i++)
		{
			cubez[0].dropOff(getPos(), rotAxis, rotAngle);
		}
		cubez.RemoveAt(0);
		Vector4 col = default(Vector4);
		if (!oluMode)
		{
			if (hitPoints > 1)
			{
				((Vector4)(ref col))._002Ector(1f, 1f, 0f, 1f);
			}
			else
			{
				((Vector4)(ref col))._002Ector(1f, 0f, 0f, 1f);
			}
		}
		else if (hitPoints > 1)
		{
			((Vector4)(ref col))._002Ector(0.56f, 0.93f, 0.56f, 1f);
		}
		else
		{
			((Vector4)(ref col))._002Ector(0f, 0.5f, 0f, 1f);
		}
		BaseGame.Get().ps.AddParticles(getPos(), Vector3.Forward * 25f, 2f, 180f, Vector3.Zero, 0f, 0.35f, 0.1f, 0.2f, col, 20, 0.0005f);
		base.hit(toHit);
	}

	public override void act(GameTime gametime)
	{
		for (int i = 0; i < cubez.Count; i++)
		{
			cubez[i].rotate(gametime);
		}
		base.act(gametime);
	}

	public override void start()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		addTarget(Vector3.Zero, 2, 10);
		addCond(new TimeCondition(0.5));
		base.start();
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
		return "[iCube]";
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
