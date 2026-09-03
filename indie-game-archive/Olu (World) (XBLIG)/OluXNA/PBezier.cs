using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class PBezier : IPath
{
	public Vector3[] pos;

	public float speed;

	private float inc;

	private float baseline;

	private bool loop;

	private Vector3 curPos;

	private Vector3 curDir;

	private Vector3 up;

	private BezierHelper bh;

	private float xAmp;

	private float yAmp;

	private float periods;

	private int channel;

	private float boost;

	private double ydif;

	private double xdif;

	public PBezier(Vector3 _p0, Vector3 _p3, float _sp, Vector3 _up, float _xAmp, float _yAmp, float _periods, int _channel, float _boost, double _xdif, double _ydif)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Initialize(_p0, 2f * _p0 / 3f + _p3 / 3f, 2f * _p3 / 3f + _p0 / 3f, _p3, _sp, _up, _xAmp, _yAmp, _periods, _channel, _boost, _xdif, _ydif);
	}

	public PBezier(Vector3 _p0, Vector3 _p1, Vector3 _p2, Vector3 _p3, float _sp, Vector3 _up, float _xAmp, float _yAmp, float _periods, int _channel, float _boost, double _xdif, double _ydif)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Initialize(_p0, _p1, _p2, _p3, _sp, _up, _xAmp, _yAmp, _periods, _channel, _boost, _xdif, _ydif);
	}

	public PBezier(PBezier other)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		pos = (Vector3[])(object)new Vector3[4];
		for (int i = 0; i < pos.Length; i++)
		{
			ref Vector3 reference = ref pos[i];
			reference = other.pos[i];
		}
		speed = other.speed;
		inc = other.inc;
		baseline = other.baseline;
		loop = other.loop;
		xAmp = other.xAmp;
		yAmp = other.yAmp;
		periods = other.periods;
		channel = other.channel;
		boost = other.boost;
		curPos = other.curPos;
		curDir = other.curDir;
		up = other.up;
		bh = other.bh;
		xdif = other.xdif;
		ydif = other.ydif;
	}

	public PBezier(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Vector3 vectorFromAtt = LevelLoader.GetVectorFromAtt(attributes, "pos0");
		Vector3 vectorFromAtt2 = LevelLoader.GetVectorFromAtt(attributes, "pos3");
		Initialize(vectorFromAtt, LevelLoader.GetVectorFromAtt(attributes, "pos1", (2f * vectorFromAtt + vectorFromAtt2) / 3f), LevelLoader.GetVectorFromAtt(attributes, "pos2", (vectorFromAtt + 2f * vectorFromAtt2) / 3f), vectorFromAtt2, LevelLoader.GetFloatFromAtt(attributes, "speed", 0.25f), LevelLoader.GetVectorFromAtt(attributes, "up", new Vector3(0f, 0f, -1f)), LevelLoader.GetFloatFromAtt(attributes, "xamp", 0f), LevelLoader.GetFloatFromAtt(attributes, "yamp", 0f), LevelLoader.GetFloatFromAtt(attributes, "periods", 1f), LevelLoader.GetIntFromAtt(attributes, "chan", 11), LevelLoader.GetFloatFromAtt(attributes, "boost", 0f), LevelLoader.GetDoubleFromAtt(attributes, "xdif", 0.0), LevelLoader.GetDoubleFromAtt(attributes, "ydif", 0.0));
	}

	public IPath copy()
	{
		return new PBezier(this);
	}

	public float GetProgress()
	{
		return speed * inc;
	}

	public Vector3 curLocation()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return curPos;
	}

	public Vector3 curEndLocation()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return pos[3];
	}

	private void calculate()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		Vector4 bezierCoords = BezierHelper.GetBezierCoords(speed * inc);
		Vector4 val = Vector4.Transform(bezierCoords, bh.BezierPos);
		Vector4 val2 = Vector4.Transform(bezierCoords, bh.BezierVel);
		curPos = new Vector3(val.X, val.Y, val.Z);
		curDir = new Vector3(val2.X, val2.Y, val2.Z);
		Vector3 val3 = Vector3.Normalize(Vector3.Cross(curDir, up));
		up = Vector3.Normalize(Vector3.Cross(val3, curDir));
		double num = (double)(periods * speed * inc) * 2.0 * Math.PI;
		curPos += ((float)Math.Sin(num + xdif) * val3 * xAmp + (float)Math.Sin(num + ydif) * up * yAmp) * bh.scale;
		curDir += ((float)Math.Cos(num + xdif) * val3 * xAmp + (float)Math.Cos(num + ydif) * up * yAmp) * bh.scale * periods * 2f * (float)Math.PI;
	}

	public float advance()
	{
		float num = 0f;
		inc += ((float)BaseGame.Get().totalTime - baseline) * (1f + boost * BaseGame.Get().channels[channel]);
		baseline = (float)BaseGame.Get().totalTime;
		if (speed * inc > 1f)
		{
			if (loop)
			{
				inc = (speed * inc - 1f) / speed;
			}
			else
			{
				num = (speed * inc - 1f) / speed;
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
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		inc = 0f;
		curPos = pos[0];
		curDir = Vector3.Normalize(pos[1] - pos[0]);
		baseline = (float)BaseGame.Get().totalTime;
	}

	public Vector3 dir()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (speed < 0.01f)
		{
			return Vector3.Zero;
		}
		return curDir;
	}

	public void Initialize(Vector3 _p0, Vector3 _p1, Vector3 _p2, Vector3 _p3, float _sp, Vector3 _up, float _xAmp, float _yAmp, float _periods, int _chan, float _boost, double _xdif, double _ydif)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		pos = (Vector3[])(object)new Vector3[4];
		pos[0] = _p0;
		pos[1] = _p1;
		pos[2] = _p2;
		pos[3] = _p3;
		curPos = _p0;
		curDir = Vector3.Normalize(_p1 - _p0);
		speed = _sp;
		up = Vector3.Normalize(_up);
		inc = 0f;
		loop = false;
		xAmp = _xAmp;
		yAmp = _yAmp;
		periods = _periods;
		channel = _chan;
		boost = _boost;
		xdif = MathHelper.ToRadians((float)_xdif);
		ydif = MathHelper.ToRadians((float)_ydif);
		baseline = (float)BaseGame.Get().totalTime;
		bh = new BezierHelper(_p0, _p1, _p2, _p3, swapYZ: false);
		calculate();
	}
}
