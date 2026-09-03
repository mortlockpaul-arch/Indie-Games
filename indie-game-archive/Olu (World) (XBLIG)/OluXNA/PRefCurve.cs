using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class PRefCurve : IPath
{
	public Vector3 start;

	public Vector3 end;

	public Vector3 refPoint;

	private Vector3 curPos;

	private float speed;

	private float inc;

	private bool loop;

	private float baseline;

	public PRefCurve()
	{
	}

	public PRefCurve(Vector3 _start, Vector3 _end, float _sp, Vector3 _refPoint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(_start, _end, _sp, _refPoint, _loop: false);
	}

	public PRefCurve(Vector3 _start, Vector3 _end, float _sp, Vector3 _refPoint, bool _loop)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Initialize(_start, _end, _sp, _refPoint, _loop);
	}

	public PRefCurve(PRefCurve other)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		start = other.start;
		end = other.end;
		refPoint = other.refPoint;
		curPos = other.curPos;
		speed = other.speed;
		inc = other.inc;
		loop = other.loop;
		baseline = other.baseline;
	}

	public IPath copy()
	{
		return new PRefCurve(this);
	}

	public Vector3 curLocation()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return curPos;
	}

	public float maxSpeed()
	{
		return speed;
	}

	public float advance()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		inc = (float)BaseGame.Get().totalTime - baseline;
		if (speed * inc > Vector3.Distance(end, start))
		{
			if (loop)
			{
				inc = (speed * inc - Vector3.Distance(end, start)) / speed;
			}
			else
			{
				num = (speed * inc - Vector3.Distance(end, start)) / speed;
			}
		}
		if (num == 0f)
		{
			calculate();
		}
		return num;
	}

	public void reset()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		inc = 0f;
		curPos = start;
		baseline = (float)BaseGame.Get().totalTime;
	}

	public Vector3 dir()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return end - start;
	}

	public void Initialize(Vector3 _start, Vector3 _end, float _sp, Vector3 _refPoint, bool _loop)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		start = _start;
		end = _end;
		speed = _sp;
		refPoint = _refPoint;
		inc = 0f;
		loop = _loop;
		baseline = (float)BaseGame.Get().totalTime;
	}

	public PRefCurve(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Vector3 vectorFromAtt = LevelLoader.GetVectorFromAtt(attributes, "start");
		Vector3 vectorFromAtt2 = LevelLoader.GetVectorFromAtt(attributes, "end");
		if (attributes["track"] == "camera")
		{
			if (attributes.ContainsKey("loop"))
			{
				Initialize(vectorFromAtt, vectorFromAtt2, LevelLoader.GetFloatFromAtt(attributes, "speed", 10f), BaseGame.Get().playerPos, !(attributes["loop"] == "false"));
			}
			else
			{
				Initialize(vectorFromAtt, vectorFromAtt2, LevelLoader.GetFloatFromAtt(attributes, "speed", 10f), BaseGame.Get().playerPos, _loop: false);
			}
		}
	}

	private void calculate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = end - start;
		val = Vector3.Normalize(val);
		val *= speed * inc;
		val += start;
		val += refPoint;
		curPos = val;
	}
}
