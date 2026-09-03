using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class Fish01Flock : Enemy
{
	public PathList spawnList;

	public Vector3 vel;

	public Vector3 up;

	public float maxSpeed;

	public float accel;

	public float waterHeight;

	public bool drawRipple;

	public float beatdown;

	public float beatInterval;

	public int numPerInterval;

	public int totalNum;

	public float error;

	public Random r;

	public Fish01Flock()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		state = 0;
		attackCooldown = 5f;
		hitPoints = 1;
		vel = Vector3.Forward;
		up = Vector3.Up;
		maxSpeed = 40f;
		accel = 2f;
		r = new Random();
	}

	public Fish01Flock(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		if (attributes.ContainsKey("fill") && attributes["fill"].Equals("wire"))
		{
			fillMode = (FillMode)2;
		}
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out spawnList, BaseGame.Get().level.activeZone);
		waterHeight = LevelLoader.GetFloatFromAtt(attributes, "waterheight", 0f);
		drawRipple = LevelLoader.GetBoolFromAtt(attributes, "ripple", defVal: true);
		beatInterval = (float)LevelLoader.GetIntFromAtt(attributes, "interval", 2) * BaseGame.BEAT;
		numPerInterval = LevelLoader.GetIntFromAtt(attributes, "num", 1);
		totalNum = LevelLoader.GetIntFromAtt(attributes, "total", 4);
		error = LevelLoader.GetFloatFromAtt(attributes, "error", 10f);
		beatdown = 0f;
	}

	public override void draw(GameTime gametime)
	{
	}

	public override void hit(TargetEffectBase toHit)
	{
		base.hit(toHit);
	}

	public override void act(GameTime gametime)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!exists)
		{
			return;
		}
		if (beatdown < 0.01f)
		{
			for (int i = 0; i < numPerInterval; i++)
			{
				if (totalNum <= 0)
				{
					break;
				}
				Enemy item = new Fish01(SetFirst(spawnList.Clone(), new PLine(GetRandomPos(), Vector3.Forward, 0f)), waterHeight, drawRipple, fillMode);
				BaseGame.Get().enems.Add(item);
				BaseGame.Get().enems[BaseGame.Get().enems.Count - 1].start();
				totalNum--;
			}
			beatdown += beatInterval;
		}
		if (totalNum <= 0)
		{
			exists = false;
			leave();
		}
		beatdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
	}

	public PathList SetFirst(PathList toChange, IPath toSet)
	{
		PathList pathList = toChange.Clone();
		foreach (PComboPath publicPath in pathList.publicPaths)
		{
			publicPath.second = toSet.copy();
		}
		return pathList;
	}

	public Vector3 GetRandomPos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX((float)r.NextDouble() * (float)Math.PI) * Matrix.CreateRotationZ((float)r.NextDouble() * (float)Math.PI * 2f)) * error * (float)r.NextDouble();
	}

	public override void start()
	{
		base.start();
		beatdown = -0.02f;
		BaseGame.Get().actualEnem--;
	}

	public override Vector3 getPos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Zero;
	}

	public override string name()
	{
		return "[flock]";
	}

	public override bool Check(int numEnem)
	{
		return false;
	}

	public override void HitSound(int lockNum, float volume)
	{
	}

	public override void die()
	{
		BaseGame.Get().actualEnem++;
		base.die();
	}

	public override void leave()
	{
		BaseGame.Get().actualEnem++;
		base.leave();
	}
}
