using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class CurveRelationshipSplineControl
{
	public enum Mode
	{
		Spline,
		Lerp,
		SmoothStep
	}

	public readonly Mode mode;

	private SplineTraj spline;

	private List<Vector2> controlValues;

	public CurveRelationshipSplineControl(Mode mode, List<Vector2> controlValues)
	{
		this.mode = mode;
		this.controlValues = controlValues;
		if (mode == Mode.Spline)
		{
			spline = new SplineTraj(controlValues);
			return;
		}
		float x = controlValues[0].X;
		for (int i = 1; i < controlValues.Count; i++)
		{
			if (controlValues[i].X <= x)
			{
				throw new Exception("control values indexes (.X) is not always increasing " + x + " -> " + controlValues[i].X);
			}
			x = controlValues[i].X;
		}
	}

	public float Value(float xRatio)
	{
		if (mode == Mode.Spline)
		{
			return spline.GetByRatio(xRatio).Y;
		}
		if (xRatio < 0f || xRatio > 1f)
		{
			throw new Exception("ratio not between 0 and 1 : " + xRatio);
		}
		for (int i = 1; i < controlValues.Count; i++)
		{
			Vector2 vector = controlValues[i - 1];
			Vector2 vector2 = controlValues[i];
			if (xRatio >= vector.X && xRatio <= vector2.X)
			{
				float amount = (xRatio - vector.X) / (vector2.X - vector.X);
				if (mode != Mode.Lerp)
				{
					return MathHelper.SmoothStep(vector.Y, vector2.Y, amount);
				}
				return MathHelper.Lerp(vector.Y, vector2.Y, amount);
			}
		}
		throw new Exception("impossible :)");
	}
}
