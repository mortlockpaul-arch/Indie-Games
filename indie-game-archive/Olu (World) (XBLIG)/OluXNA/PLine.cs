using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class PLine : IPath
{
	public Vector3 start;

	public Vector3 end;

	public float speed;

	private float inc;

	private float baseline;

	private bool loop;

	private Vector3 curPos;

	public PLine(Vector3 _start, Vector3 _end, float _sp)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Initialize(_start, _end, _sp);
	}

	public PLine(PLine other)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		start = other.start;
		end = other.end;
		speed = other.speed;
		inc = other.inc;
		baseline = other.baseline;
		loop = other.loop;
		curPos = other.curPos;
	}

	public PLine(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Vector3 vectorFromAtt = LevelLoader.GetVectorFromAtt(attributes, "start", Vector3.Zero);
		Vector3 vectorFromAtt2 = LevelLoader.GetVectorFromAtt(attributes, "end", new Vector3(0f, 0f, 1f));
		Initialize(vectorFromAtt, vectorFromAtt2, LevelLoader.GetFloatFromAtt(attributes, "speed", 0f));
	}

	public IPath copy()
	{
		return new PLine(this);
	}

	public Vector3 curLocation()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return curPos;
	}

	public Vector3 curEndLocation()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return end;
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
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = end - start;
		val = Vector3.Normalize(val);
		val *= speed * inc;
		val += start;
		curPos = val;
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

	public float maxSpeed()
	{
		return speed;
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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (speed < 0.1f)
		{
			return Vector3.Zero;
		}
		return end - start;
	}

	public void Initialize(Vector3 _start, Vector3 _end, float _sp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		curPos = _start;
		start = _start;
		end = _end;
		speed = _sp;
		inc = 0f;
		loop = false;
		baseline = (float)BaseGame.Get().totalTime;
	}
}
