using FarseerPhysics.Collision;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Contacts;

internal struct PositionSolverManifold
{
	internal Vector2 Normal;

	internal Vector2 Point;

	internal float Separation;

	internal PositionSolverManifold(ref ContactConstraint cc, int index)
	{
		switch (cc.Type)
		{
		case ManifoldType.Circles:
		{
			Vector2 worldPoint5 = cc.BodyA.GetWorldPoint(ref cc.LocalPoint);
			Vector2 worldPoint6 = cc.BodyB.GetWorldPoint(ref cc.Points[0].LocalPoint);
			if (Vector2.DistanceSquared(worldPoint5, worldPoint6) > 1.4210855E-14f)
			{
				Normal = worldPoint6 - worldPoint5;
				Normal.Normalize();
			}
			else
			{
				Normal = new Vector2(1f, 0f);
			}
			Point = 0.5f * (worldPoint5 + worldPoint6);
			Separation = Vector2.Dot(worldPoint6 - worldPoint5, Normal) - cc.Radius;
			break;
		}
		case ManifoldType.FaceA:
		{
			Normal = cc.BodyA.GetWorldVector(ref cc.LocalNormal);
			Vector2 worldPoint3 = cc.BodyA.GetWorldPoint(ref cc.LocalPoint);
			Vector2 worldPoint4 = cc.BodyB.GetWorldPoint(ref cc.Points[index].LocalPoint);
			Separation = Vector2.Dot(worldPoint4 - worldPoint3, Normal) - cc.Radius;
			Point = worldPoint4;
			break;
		}
		case ManifoldType.FaceB:
		{
			Normal = cc.BodyB.GetWorldVector(ref cc.LocalNormal);
			Vector2 worldPoint = cc.BodyB.GetWorldPoint(ref cc.LocalPoint);
			Vector2 worldPoint2 = cc.BodyA.GetWorldPoint(ref cc.Points[index].LocalPoint);
			Separation = Vector2.Dot(worldPoint2 - worldPoint, Normal) - cc.Radius;
			Point = worldPoint2;
			Normal = -Normal;
			break;
		}
		default:
			Normal = Vector2.Zero;
			Point = Vector2.Zero;
			Separation = 0f;
			break;
		}
	}
}
