using System;
using Microsoft.Xna.Framework;

namespace MathTools;

public static class VectorTools
{
	private const int numAngleSeparators = 1400;

	private static Vector2[] angles = (Vector2[])(object)new Vector2[1400];

	private static Matrix[] rotMat = (Matrix[])(object)new Matrix[1400];

	private static Matrix rotBuf;

	public static void Initialize()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(1f, 0f);
		for (int i = 0; i < 1400; i++)
		{
			ref Matrix reference = ref rotMat[i];
			reference = Matrix.CreateFromAxisAngle(new Vector3(0f, 0f, 1f), (float)Math.PI * 2f * ((float)i / 1400f));
			ref Vector2 reference2 = ref angles[i];
			reference2 = Vector2.Transform(val, rotMat[i]);
		}
	}

	public static void RotMat(float f, out Matrix m)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		int i;
		for (i = (int)((double)f / (Math.PI * 2.0) * 1400.0); i < 0; i += 1400)
		{
		}
		while (i >= 1400)
		{
			i -= 1400;
		}
		m = rotMat[i];
	}

	public static Vector2 Rotate(ref Vector2 vec, float rot, out Vector2 result)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		RotMat(rot, out rotBuf);
		Vector2.Transform(ref vec, ref rotBuf, ref result);
		return result;
	}

	public static void ScaleAtAngle(float angle, Vector2 scale, Vector2 source, out Vector2 result)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Rotate(ref source, 0f - angle, out result);
		result *= scale;
		Rotate(ref result, angle, out result);
	}

	public static float GetAngleFromVector(Vector2 vec)
	{
		if (vec.X != 0f)
		{
			float num = (float)Math.Atan(vec.Y / vec.X);
			if (vec.X > 0f)
			{
				return num;
			}
			return num + (float)Math.PI;
		}
		if (vec.Y > 0f)
		{
			return (float)Math.PI / 2f;
		}
		return -(float)Math.PI / 2f;
	}
}
