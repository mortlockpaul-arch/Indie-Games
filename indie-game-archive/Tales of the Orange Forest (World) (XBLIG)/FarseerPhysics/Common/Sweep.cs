using System;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common;

public struct Sweep
{
	public float a;

	public float a0;

	public Vector2 c;

	public Vector2 c0;

	public Vector2 LocalCenter;

	public void GetTransform(out Transform xf, float alpha)
	{
		xf = default(Transform);
		xf.Position = (1f - alpha) * c0 + alpha * c;
		float angle = (1f - alpha) * a0 + alpha * a;
		xf.R.Set(angle);
		xf.Position -= MathUtils.Multiply(ref xf.R, LocalCenter);
	}

	public void Advance(float t)
	{
		c0 = (1f - t) * c0 + t * c;
		a0 = (1f - t) * a0 + t * a;
	}

	public void Normalize()
	{
		float num = (float)Math.PI * 2f;
		float num2 = num * (float)Math.Floor(a0 / num);
		a0 -= num2;
		a -= num2;
	}
}
