using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class GiftFlock : Enemy
{
	public PathList spawnList;

	public float size;

	public float waitAmount;

	public float beatdown;

	public float beatInterval;

	public int numPerInterval;

	public int totalNum;

	public float error;

	public bool oluMode;

	public Random r;

	public GiftFlock()
	{
		r = new Random();
	}

	public GiftFlock(Dictionary<string, string> attributes, XmlNode node)
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
		LevelLoader.BuildPath(node.SelectSingleNode("paths"), out spawnList, BaseGame.Get().level.activeZone);
		beatInterval = (float)LevelLoader.GetIntFromAtt(attributes, "interval", 2) * BaseGame.BEAT;
		numPerInterval = LevelLoader.GetIntFromAtt(attributes, "num", 1);
		totalNum = LevelLoader.GetIntFromAtt(attributes, "total", 4);
		error = LevelLoader.GetFloatFromAtt(attributes, "error", 10f);
		beatdown = 0f;
		size = LevelLoader.GetFloatFromAtt(attributes, "size", 5f);
		waitAmount = LevelLoader.GetFloatFromAtt(attributes, "openwait", 4f);
	}

	public GiftFlock(float _beatInterval, int _numPerInt, int _totalNum, float _error, float _size, float _waitAmount, FillMode _fillMode, PathList _pList)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		beatInterval = _beatInterval * BaseGame.BEAT;
		numPerInterval = _numPerInt;
		totalNum = _totalNum;
		error = _error;
		size = _size;
		waitAmount = _waitAmount;
		fillMode = _fillMode;
		spawnList = _pList;
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
				Enemy item = new Gift(SetFirst(spawnList.Clone(), new PLine(GetRandomPos(), Vector3.Forward, 0f)), size, waitAmount, fillMode, oluMode);
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
