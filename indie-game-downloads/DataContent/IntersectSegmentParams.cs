using Microsoft.Xna.Framework;

namespace DataContent;

public struct IntersectSegmentParams
{
	public bool OnlyWalkable;

	public float SegmentLength;

	public Vector3 SegmentStart;

	public Vector3 SegmentEnd;

	public Vector3 SegmentDirection;

	public Vector3 hitNormal;

	public Vector3 hitPosition;

	public float hitDistance;

	public float Tparameter;

	public int SegmentIndex;

	public int TriggerIndex;

	public int TargetIndex;

	public float oodX;

	public float oodY;

	public float oodZ;

	public Vector3 SegmentMidpoint;

	public Vector3 SegmentHalflength;

	public void PreComputeParameters()
	{
		hitDistance = float.MaxValue;
		SegmentMidpoint = (SegmentStart + SegmentEnd) * 0.5f;
		SegmentHalflength = SegmentEnd - SegmentMidpoint;
		oodX = 1f / (SegmentDirection.X + 1E-07f);
		oodY = 1f / (SegmentDirection.Y + 1E-07f);
		oodZ = 1f / (SegmentDirection.Z + 1E-07f);
	}
}
