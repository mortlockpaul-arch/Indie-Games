using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision.Shapes;

public abstract class Shape
{
	public MassData MassData;

	public float Radius;

	internal float _density;

	public ShapeType ShapeType { get; internal set; }

	public abstract int ChildCount { get; }

	public float Density
	{
		get
		{
			return _density;
		}
		set
		{
			_density = value;
			ComputeProperties();
		}
	}

	protected Shape(float density)
	{
		_density = density;
		ShapeType = ShapeType.Unknown;
	}

	public abstract Shape Clone();

	public abstract bool TestPoint(ref Transform transform, ref Vector2 point);

	public abstract bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex);

	public abstract void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex);

	public abstract void ComputeProperties();
}
