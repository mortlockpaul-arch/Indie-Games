using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class TRotation : ITransform
{
	private int _bStart;

	private int _bEnd;

	private double totalTime;

	private double elapsedTime;

	private Vector3 axis;

	private float endAngle;

	private float startAngle;

	public TRotation(int start, int end)
	{
		Initialize(start, end);
	}

	public void Initialize(int start, int end)
	{
		_bStart = start;
		_bEnd = end;
		totalTime = (double)(_bEnd - _bStart) * (double)BaseGame.BEAT;
		elapsedTime = 0.0;
	}

	public TRotation(Dictionary<string, string> attributes, XmlNode node, int start, int end)
		: this(attributes, node)
	{
		Initialize(start, end);
	}

	public TRotation(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		startAngle = MathHelper.ToRadians(LevelLoader.GetFloatFromAtt(attributes, "startdeg", 0f));
		endAngle = MathHelper.ToRadians(LevelLoader.GetFloatFromAtt(attributes, "enddeg", 0f));
		axis = LevelLoader.GetVectorFromAtt(attributes, "axis");
	}

	public float PercentDone()
	{
		if (_bStart == -1 && _bEnd == -1)
		{
			return 0f;
		}
		return (float)(elapsedTime / totalTime);
	}

	public float CurAngle()
	{
		return startAngle + PercentDone() * (endAngle - startAngle);
	}

	public float CurAngle(float amountDone)
	{
		return startAngle + amountDone * (endAngle - startAngle);
	}

	public void Update(double gametime)
	{
		if (BaseGame.Get().elaspedEndTime >= _bStart && BaseGame.Get().elaspedEndTime < _bEnd)
		{
			elapsedTime += gametime;
		}
	}

	public Matrix GetMatrix()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if ((BaseGame.Get().elaspedEndTime >= _bStart && BaseGame.Get().elaspedEndTime < _bEnd) || (!BaseGame.Get().movingToNextZone && _bStart == -1))
		{
			return Matrix.CreateFromAxisAngle(axis, CurAngle());
		}
		return Matrix.Identity;
	}

	public Matrix GetMatrix(float amountDone)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateFromAxisAngle(axis, CurAngle(amountDone));
	}
}
