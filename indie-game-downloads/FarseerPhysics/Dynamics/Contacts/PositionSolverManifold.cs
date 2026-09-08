using FarseerPhysics.Collision;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Contacts;

internal static class PositionSolverManifold
{
	public static void Solve(ref ContactConstraint cc, int index, out Vector2 normal, out Vector2 point, out float separation)
	{
		switch (cc.Type)
		{
		case ManifoldType.Circles:
		{
			Vector2 worldPoint5 = cc.BodyA.GetWorldPoint(ref cc.LocalPoint);
			Vector2 worldPoint6 = cc.BodyB.GetWorldPoint(ref cc.Points[0].LocalPoint);
			if (Vector2.DistanceSquared(worldPoint5, worldPoint6) > 1.4210855E-14f)
			{
				normal = worldPoint6 - worldPoint5;
				normal.Normalize();
			}
			else
			{
				normal = new Vector2(1f, 0f);
			}
			point = 0.5f * (worldPoint5 + worldPoint6);
			separation = Vector2.Dot(worldPoint6 - worldPoint5, normal) - cc.RadiusA - cc.RadiusB;
			break;
		}
		case ManifoldType.FaceA:
		{
			normal = cc.BodyA.GetWorldVector(ref cc.LocalNormal);
			Vector2 worldPoint3 = cc.BodyA.GetWorldPoint(ref cc.LocalPoint);
			Vector2 worldPoint4 = cc.BodyB.GetWorldPoint(ref cc.Points[index].LocalPoint);
			separation = Vector2.Dot(worldPoint4 - worldPoint3, normal) - cc.RadiusA - cc.RadiusB;
			point = worldPoint4;
			break;
		}
		case ManifoldType.FaceB:
		{
			normal = cc.BodyB.GetWorldVector(ref cc.LocalNormal);
			Vector2 worldPoint = cc.BodyB.GetWorldPoint(ref cc.LocalPoint);
			Vector2 worldPoint2 = cc.BodyA.GetWorldPoint(ref cc.Points[index].LocalPoint);
			separation = Vector2.Dot(worldPoint2 - worldPoint, normal) - cc.RadiusA - cc.RadiusB;
			point = worldPoint2;
			normal = -normal;
			break;
		}
		default:
			normal = Vector2.Zero;
			point = Vector2.Zero;
			separation = 0f;
			break;
		}
	}
}
