using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Contacts;

internal struct TOIConstraint
{
	public Body BodyA;

	public Body BodyB;

	public Vector2 LocalNormal;

	public Vector2 LocalPoint;

	public FixedArray2<Vector2> LocalPoints;

	public int PointCount;

	public float Radius;

	public ManifoldType Type;
}
