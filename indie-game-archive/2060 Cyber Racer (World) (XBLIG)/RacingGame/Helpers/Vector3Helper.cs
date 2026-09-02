using System;
using Microsoft.Xna.Framework;

namespace RacingGame.Helpers;

internal class Vector3Helper
{
	private Vector3Helper()
	{
	}

	public static float GetAngleBetweenVectors(Vector3 vec1, Vector3 vec2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return (float)Math.Acos(Vector3.Dot(vec1, vec2));
	}

	public static float DistanceToLine(Vector3 point, Vector3 linePos1, Vector3 linePos2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = linePos2 - linePos1;
		Vector3 val2 = linePos1 - point;
		Vector3 val3 = Vector3.Cross(val, val2);
		return ((Vector3)(ref val3)).Length() / ((Vector3)(ref val)).Length();
	}

	public static float SignedDistanceToPlane(Vector3 point, Vector3 planePosition, Vector3 planeNormal)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = planePosition - point;
		return Vector3.Dot(planeNormal, val);
	}
}
