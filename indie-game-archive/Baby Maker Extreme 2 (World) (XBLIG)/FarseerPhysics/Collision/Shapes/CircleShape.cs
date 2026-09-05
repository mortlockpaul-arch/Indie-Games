using System;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision.Shapes;

public class CircleShape : Shape
{
	public Vector2 Position;

	public override int ChildCount => 1;

	public CircleShape(float radius, float density)
		: base(density)
	{
		base.ShapeType = ShapeType.Circle;
		Radius = radius;
		Position = Vector2.Zero;
		ComputeProperties();
	}

	internal CircleShape()
		: base(0f)
	{
		base.ShapeType = ShapeType.Circle;
		Radius = 0f;
		Position = Vector2.Zero;
	}

	public override Shape Clone()
	{
		CircleShape circleShape = new CircleShape();
		circleShape.ShapeType = base.ShapeType;
		circleShape.Radius = Radius;
		circleShape.Position = Position;
		circleShape._density = _density;
		circleShape.MassData = MassData;
		return circleShape;
	}

	public override bool TestPoint(ref Transform transform, ref Vector2 point)
	{
		Vector2 vector = transform.Position + MathUtils.Multiply(ref transform.R, Position);
		Vector2 vector2 = point - vector;
		return Vector2.Dot(vector2, vector2) <= Radius * Radius;
	}

	public override bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex)
	{
		output = default(RayCastOutput);
		Vector2 vector = transform.Position + MathUtils.Multiply(ref transform.R, Position);
		Vector2 vector2 = input.Point1 - vector;
		float num = Vector2.Dot(vector2, vector2) - Radius * Radius;
		Vector2 vector3 = input.Point2 - input.Point1;
		float num2 = Vector2.Dot(vector2, vector3);
		float num3 = Vector2.Dot(vector3, vector3);
		float num4 = num2 * num2 - num3 * num;
		if (num4 < 0f || num3 < 1.1920929E-07f)
		{
			return false;
		}
		float num5 = 0f - (num2 + (float)Math.Sqrt(num4));
		if (0f <= num5 && num5 <= input.MaxFraction * num3)
		{
			Vector2 normal = vector2 + (output.Fraction = num5 / num3) * vector3;
			normal.Normalize();
			output.Normal = normal;
			return true;
		}
		return false;
	}

	public override void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex)
	{
		Vector2 vector = transform.Position + MathUtils.Multiply(ref transform.R, Position);
		aabb.LowerBound = new Vector2(vector.X - Radius, vector.Y - Radius);
		aabb.UpperBound = new Vector2(vector.X + Radius, vector.Y + Radius);
	}

	public sealed override void ComputeProperties()
	{
		float num = (float)Math.PI * Radius * Radius;
		MassData.Area = num;
		MassData.Mass = _density * num;
		MassData.Centroid = Position;
		MassData.Inertia = MassData.Mass * (0.5f * Radius * Radius + Vector2.Dot(Position, Position));
	}
}
