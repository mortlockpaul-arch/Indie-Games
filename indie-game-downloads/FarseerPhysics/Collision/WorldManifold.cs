using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision;

public struct WorldManifold
{
	public Vector2 Normal;

	public FixedArray2<Vector2> Points = default(FixedArray2<Vector2>);

	public WorldManifold(ref Manifold manifold, ref Transform transformA, float radiusA, ref Transform transformB, float radiusB)
	{
		if (manifold.PointCount == 0)
		{
			Normal = Vector2.UnitY;
			return;
		}
		switch (manifold.Type)
		{
		case ManifoldType.Circles:
		{
			Vector2 vector9 = MathUtils.Multiply(ref transformA, manifold.LocalPoint);
			Vector2 vector10 = MathUtils.Multiply(ref transformB, manifold.Points[0].LocalPoint);
			Normal = new Vector2(1f, 0f);
			if (Vector2.DistanceSquared(vector9, vector10) > 1.4210855E-14f)
			{
				Normal = vector10 - vector9;
				Normal.Normalize();
			}
			Vector2 vector11 = vector9 + radiusA * Normal;
			Vector2 vector12 = vector10 - radiusB * Normal;
			Points[0] = 0.5f * (vector11 + vector12);
			break;
		}
		case ManifoldType.FaceA:
		{
			Normal = MathUtils.Multiply(ref transformA.R, manifold.LocalNormal);
			Vector2 vector5 = MathUtils.Multiply(ref transformA, manifold.LocalPoint);
			for (int j = 0; j < manifold.PointCount; j++)
			{
				Vector2 vector6 = MathUtils.Multiply(ref transformB, manifold.Points[j].LocalPoint);
				Vector2 vector7 = vector6 + (radiusA - Vector2.Dot(vector6 - vector5, Normal)) * Normal;
				Vector2 vector8 = vector6 - radiusB * Normal;
				Points[j] = 0.5f * (vector7 + vector8);
			}
			break;
		}
		case ManifoldType.FaceB:
		{
			Normal = MathUtils.Multiply(ref transformB.R, manifold.LocalNormal);
			Vector2 vector = MathUtils.Multiply(ref transformB, manifold.LocalPoint);
			for (int i = 0; i < manifold.PointCount; i++)
			{
				Vector2 vector2 = MathUtils.Multiply(ref transformA, manifold.Points[i].LocalPoint);
				Vector2 vector3 = vector2 - radiusA * Normal;
				Vector2 vector4 = vector2 + (radiusB - Vector2.Dot(vector2 - vector, Normal)) * Normal;
				Points[i] = 0.5f * (vector3 + vector4);
			}
			Normal *= -1f;
			break;
		}
		default:
			Normal = Vector2.UnitY;
			break;
		}
	}
}
