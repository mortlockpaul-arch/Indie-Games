using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision.Shapes;

public abstract class Shape
{
	public float Radius;

	public ShapeType ShapeType { get; internal set; }

	public abstract int ChildCount { get; }

	protected Shape()
	{
		ShapeType = ShapeType.Unknown;
	}

	public abstract Shape Clone();

	public abstract bool TestPoint(ref Transform transform, ref Vector2 point);

	public abstract bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex);

	public abstract void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex);

	public abstract void ComputeMass(out MassData massData, float density);
}
