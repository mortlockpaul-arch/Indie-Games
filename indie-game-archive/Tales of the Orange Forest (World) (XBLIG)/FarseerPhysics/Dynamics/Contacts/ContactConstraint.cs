using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Contacts;

public class ContactConstraint
{
	public Body BodyA;

	public Body BodyB;

	public float Friction;

	public Mat22 K;

	public Vector2 LocalNormal;

	public Vector2 LocalPoint;

	public Manifold Manifold;

	public Vector2 Normal;

	public Mat22 NormalMass;

	public int PointCount;

	public FixedArray2<ContactConstraintPoint> Points;

	public float Radius;

	public ManifoldType Type;

	public ContactConstraint()
	{
		Points[0] = new ContactConstraintPoint();
		Points[1] = new ContactConstraintPoint();
	}
}
