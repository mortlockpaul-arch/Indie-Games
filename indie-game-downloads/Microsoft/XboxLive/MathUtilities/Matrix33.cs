using System;

namespace Microsoft.XboxLive.MathUtilities;

public struct Matrix33
{
	public float M11;

	public float M12;

	public float M13;

	public float M21;

	public float M22;

	public float M23;

	public float M31;

	public float M32;

	public float M33;

	public void SetIdentity()
	{
		M11 = 1f;
		M12 = 0f;
		M13 = 0f;
		M21 = 0f;
		M22 = 1f;
		M23 = 0f;
		M31 = 0f;
		M32 = 0f;
		M33 = 1f;
	}

	public void SetRotationEuler(float angleX, float angleY, float angleZ)
	{
		float num = (float)Math.Cos(angleX);
		float num2 = (float)Math.Sin(angleX);
		float num3 = (float)Math.Cos(angleY);
		float num4 = (float)Math.Sin(angleY);
		float num5 = (float)Math.Cos(angleZ);
		float num6 = (float)Math.Sin(angleZ);
		M11 = num3 * num5;
		M12 = num2 * num4 * num5 - num * num6;
		M13 = num * num4 * num5 + num2 * num6;
		M21 = num3 * num6;
		M22 = num2 * num4 * num6 + num * num5;
		M23 = num * num4 * num6 - num2 * num5;
		M31 = 0f - num4;
		M32 = num2 * num3;
		M33 = num * num3;
	}
}
