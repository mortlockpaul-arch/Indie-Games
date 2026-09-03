using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class PCircleHoriz : IPath
{
	public Vector3 center;

	public Vector3 baseRad;

	protected float radius;

	protected float startRad;

	protected float speed;

	protected float inc;

	protected float baseline;

	protected bool loop;

	protected Vector3 curPos;

	public PCircleHoriz(Vector3 _center, float _radius, float _sp, bool _loop)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Initialize(_center, _radius, _sp, _loop, 0f);
	}

	public PCircleHoriz(Vector3 _center, float _radius, float _sp, bool _loop, float _startRad)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Initialize(_center, _radius, _sp, _loop, startRad);
	}

	public PCircleHoriz(PCircleHoriz other)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		center = other.center;
		radius = other.radius;
		speed = other.speed;
		inc = other.inc;
		baseline = other.baseline;
		loop = other.loop;
		curPos = other.curPos;
		baseRad = other.baseRad;
		startRad = other.startRad;
	}

	public PCircleHoriz(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Vector3 vectorFromAtt = LevelLoader.GetVectorFromAtt(attributes, "center");
		float floatFromAtt = LevelLoader.GetFloatFromAtt(attributes, "radius", 10f);
		float floatFromAtt2 = LevelLoader.GetFloatFromAtt(attributes, "startrad", 0f);
		Initialize(vectorFromAtt, floatFromAtt, LevelLoader.GetFloatFromAtt(attributes, "speed", 1f), LevelLoader.GetBoolFromAtt(attributes, "loop", defVal: true), floatFromAtt2);
	}

	public void calculate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		curPos = center + Vector3.Transform(baseRad, Matrix.CreateRotationY((0f - inc) * speed));
	}

	public void Initialize(Vector3 _center, float _rad, float _sp, bool _loop, float _startRad)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		center = _center;
		radius = _rad;
		baseRad = Vector3.Right * _rad;
		startRad = MathHelper.ToRadians(_startRad);
		baseRad = Vector3.Transform(baseRad, Matrix.CreateRotationY(startRad));
		curPos = _center + baseRad;
		speed = _sp;
		inc = 0f;
		loop = _loop;
		baseline = (float)BaseGame.Get().totalTime;
	}

	public virtual float advance()
	{
		float num = 0f;
		inc = (float)BaseGame.Get().totalTime - baseline;
		if ((double)(speed * inc) > Math.PI * 2.0)
		{
			if (loop)
			{
				inc = (speed * inc - (float)Math.PI * 2f) / speed;
			}
			else
			{
				num = (speed * inc - (float)Math.PI * 2f) / speed;
			}
		}
		if (num == 0f)
		{
			calculate();
		}
		return num;
	}

	public IPath copy()
	{
		return new PCircleHoriz(this);
	}

	public Vector3 curLocation()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return curPos;
	}

	public float maxSpeed()
	{
		return radius * speed;
	}

	public void reset()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		inc = 0f;
		curPos = center + baseRad;
		baseline = (float)BaseGame.Get().totalTime;
	}

	public Vector3 dir()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Forward;
	}
}
