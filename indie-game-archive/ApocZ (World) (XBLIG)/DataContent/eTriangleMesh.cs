using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.eTriangleMesh, DataContent")]
public class eTriangleMesh
{
	public eOOBB oobb;

	public MaterialType Material;

	public GeometryFlags Flags;

	public Vector3[] Ocluder;

	public int numTriangles;

	public TriangleData[] triangleMesh;

	private static Vector3 closestPoint = Vector3.Zero;

	private static Vector3 TorsoCenter = Vector3.Zero;

	private static BoundingBox tmpIntersectBBox = default(BoundingBox);

	private static Vector3 tmpHitPosition = Vector3.Zero;

	private static Vector3 tmpHitNormal = Vector3.Zero;

	private static IntersectSegmentParams tmpSegmentParams = default(IntersectSegmentParams);

	public bool IntersectFPSSphere(ref BoundingSphere e)
	{
		return false;
	}

	public bool IntersectSphere(ref BoundingSphere e)
	{
		return false;
	}

	public bool TestSphereTriangle(ref BoundingSphere e, ref Vector3 n)
	{
		float num = (oobb.boundingShere.Center - e.Center).LengthSquared();
		float num2 = oobb.boundingShere.Radius + e.Radius;
		if (num <= num2 * num2)
		{
			tmpIntersectBBox.Min = oobb.Min;
			tmpIntersectBBox.Max = oobb.Max;
			if (tmpIntersectBBox.Intersects(e))
			{
				float radiusSqr = e.Radius * e.Radius;
				if (triangleMesh != null)
				{
					for (int i = 0; i < triangleMesh.Length; i++)
					{
						float disSqr = 1000000f;
						if (MyMath.IntersectSphereTriangle(ref e.Center, radiusSqr, ref triangleMesh[i], ref closestPoint, ref disSqr))
						{
							n = triangleMesh[i].Normal;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public bool TestSphere(ref BoundingSphere e)
	{
		if ((Flags & GeometryFlags.Walkable) > GeometryFlags.Clear)
		{
			tmpIntersectBBox.Min = oobb.Min;
			tmpIntersectBBox.Max = oobb.Max;
			if (tmpIntersectBBox.Intersects(e))
			{
				return true;
			}
		}
		return false;
	}

	public bool IntersectFPSCharacter(ref BoundingSphere e, ref CollisionStruct c, bool Crouched, float yOffset)
	{
		bool result = false;
		float num = e.Radius * e.Radius;
		float num2 = (yOffset + 2f + e.Radius) * (yOffset + 2f + e.Radius);
		tmpSegmentParams.SegmentDirection = -Vector3.UnitY;
		tmpSegmentParams.SegmentLength = 500f;
		tmpSegmentParams.SegmentStart = e.Center;
		tmpSegmentParams.SegmentEnd = e.Center + Vector3.UnitY * (0f - tmpSegmentParams.SegmentLength);
		tmpSegmentParams.PreComputeParameters();
		if ((Flags & GeometryFlags.Walkable) > GeometryFlags.Clear)
		{
			tmpIntersectBBox.Min = oobb.Min;
			tmpIntersectBBox.Max = oobb.Max;
			e.Radius += 36f;
			bool flag = tmpIntersectBBox.Intersects(e);
			e.Radius -= 36f;
			if (flag && triangleMesh != null)
			{
				for (int i = 0; i < triangleMesh.Length; i++)
				{
					if (Vector3.Dot(triangleMesh[i].Normal, Vector3.UnitY) > 0.6f && MyMath.IntersectSegmentTriangle(ref tmpSegmentParams, ref triangleMesh[i]))
					{
						float num3 = (tmpSegmentParams.hitPosition - tmpSegmentParams.SegmentStart).LengthSquared();
						if (num3 < num2)
						{
							c.onWalkable = true;
							e.Center.Y += yOffset + 2f + e.Radius - (tmpSegmentParams.hitPosition - tmpSegmentParams.SegmentStart).Length() - 2f;
						}
					}
					float disSqr = 1000000f;
					if (MyMath.IntersectSphereTriangle(ref e.Center, num, ref triangleMesh[i], ref closestPoint, ref disSqr))
					{
						result = true;
						float num4 = disSqr / num;
						closestPoint.Normalize();
						c.depth = e.Radius - num4 * e.Radius;
						c.hitNormal = closestPoint;
						e.Center += c.hitNormal * c.depth;
						continue;
					}
					disSqr = 1000000f;
					TorsoCenter = e.Center;
					if (Crouched)
					{
						TorsoCenter.Y += 0f;
					}
					else
					{
						TorsoCenter.Y += 42f;
					}
					if (MyMath.IntersectSphereTriangle(ref TorsoCenter, num, ref triangleMesh[i], ref closestPoint, ref disSqr))
					{
						result = true;
						float num5 = disSqr / num;
						closestPoint.Normalize();
						e.Center += closestPoint * (e.Radius - num5 * e.Radius);
					}
				}
			}
		}
		return result;
	}

	public float RayCast(ref IntersectSegmentParams segParams, bool sqrRootResult)
	{
		return 1E+10f;
	}
}
