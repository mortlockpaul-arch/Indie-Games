using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision.Shapes;

public class EdgeShape : Shape
{
	public bool HasVertex0;

	public bool HasVertex3;

	public Vector2 Vertex0;

	public Vector2 Vertex1;

	public Vector2 Vertex2;

	public Vector2 Vertex3;

	public override int ChildCount => 1;

	internal EdgeShape()
		: base(0f)
	{
		base.ShapeType = ShapeType.Edge;
		Radius = 0.01f;
	}

	public EdgeShape(Vector2 start, Vector2 end)
		: base(0f)
	{
		base.ShapeType = ShapeType.Edge;
		Radius = 0.01f;
		Set(start, end);
	}

	public void Set(Vector2 start, Vector2 end)
	{
		Vertex1 = start;
		Vertex2 = end;
		HasVertex0 = false;
		HasVertex3 = false;
		ComputeProperties();
	}

	public override Shape Clone()
	{
		EdgeShape edgeShape = new EdgeShape();
		edgeShape.HasVertex0 = HasVertex0;
		edgeShape.HasVertex3 = HasVertex3;
		edgeShape.Radius = Radius;
		edgeShape.Vertex0 = Vertex0;
		edgeShape.Vertex1 = Vertex1;
		edgeShape.Vertex2 = Vertex2;
		edgeShape.Vertex3 = Vertex3;
		edgeShape._density = _density;
		edgeShape.MassData = MassData;
		return edgeShape;
	}

	public override bool TestPoint(ref Transform transform, ref Vector2 point)
	{
		return false;
	}

	public override bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex)
	{
		output = default(RayCastOutput);
		Vector2 vector = MathUtils.MultiplyT(ref transform.R, input.Point1 - transform.Position);
		Vector2 vector2 = MathUtils.MultiplyT(ref transform.R, input.Point2 - transform.Position);
		Vector2 vector3 = vector2 - vector;
		Vector2 vertex = Vertex1;
		Vector2 vertex2 = Vertex2;
		Vector2 vector4 = vertex2 - vertex;
		Vector2 vector5 = new Vector2(vector4.Y, 0f - vector4.X);
		vector5.Normalize();
		float num = Vector2.Dot(vector5, vertex - vector);
		float num2 = Vector2.Dot(vector5, vector3);
		if (num2 == 0f)
		{
			return false;
		}
		float num3 = num / num2;
		if (num3 < 0f || 1f < num3)
		{
			return false;
		}
		Vector2 vector6 = vector + num3 * vector3;
		Vector2 vector7 = vertex2 - vertex;
		float num4 = Vector2.Dot(vector7, vector7);
		if (num4 == 0f)
		{
			return false;
		}
		float num5 = Vector2.Dot(vector6 - vertex, vector7) / num4;
		if (num5 < 0f || 1f < num5)
		{
			return false;
		}
		output.Fraction = num3;
		if (num > 0f)
		{
			output.Normal = -vector5;
		}
		else
		{
			output.Normal = vector5;
		}
		return true;
	}

	public override void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex)
	{
		aabb = default(AABB);
		Vector2 value = MathUtils.Multiply(ref transform, Vertex1);
		Vector2 value2 = MathUtils.Multiply(ref transform, Vertex2);
		Vector2 vector = Vector2.Min(value, value2);
		Vector2 vector2 = Vector2.Max(value, value2);
		Vector2 vector3 = new Vector2(Radius, Radius);
		aabb.LowerBound = vector - vector3;
		aabb.UpperBound = vector2 + vector3;
	}

	public override void ComputeProperties()
	{
		MassData.Centroid = 0.5f * (Vertex1 + Vertex2);
	}
}
