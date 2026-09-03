using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class TScale : ITransform
{
	private int _bStart;

	private int _bEnd;

	private double totalTime;

	private double elapsedTime;

	private Vector3 startScale;

	private Vector3 endScale;

	public TScale(int start, int end)
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

	public TScale(Dictionary<string, string> attributes, XmlNode node, int start, int end)
		: this(attributes, node)
	{
	}

	public TScale(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		startScale = LevelLoader.GetVectorFromAtt(attributes, "startscale");
		endScale = LevelLoader.GetVectorFromAtt(attributes, "endscale");
	}

	public float PercentDone()
	{
		if (_bStart == -1 && _bEnd == -1)
		{
			return 0f;
		}
		return (float)(elapsedTime / totalTime);
	}

	public Vector3 CurScale()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return startScale + PercentDone() * (endScale - startScale);
	}

	public Vector3 CurScale(float amountDone)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		return startScale + amountDone * (endScale - startScale);
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
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((BaseGame.Get().elaspedEndTime >= _bStart && BaseGame.Get().elaspedEndTime < _bEnd) || (!BaseGame.Get().movingToNextZone && _bStart == -1))
		{
			return Matrix.CreateScale(CurScale());
		}
		return Matrix.Identity;
	}

	public Matrix GetMatrix(float amountDone)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateScale(CurScale(amountDone));
	}
}
