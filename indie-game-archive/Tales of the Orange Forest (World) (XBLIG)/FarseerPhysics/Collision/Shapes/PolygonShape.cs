using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision.Shapes;

public class PolygonShape : Shape
{
	public Vector2 Centroid;

	public Vertices Normals;

	public Vertices Vertices;

	public override int ChildCount => 1;

	public PolygonShape(Vertices vertices)
	{
		base.ShapeType = ShapeType.Polygon;
		Radius = 0.01f;
		Centroid = Vector2.Zero;
		Set(vertices);
	}

	public PolygonShape()
	{
		base.ShapeType = ShapeType.Polygon;
		Radius = 0.01f;
		Centroid = Vector2.Zero;
	}

	public override Shape Clone()
	{
		PolygonShape polygonShape = new PolygonShape();
		polygonShape.ShapeType = base.ShapeType;
		polygonShape.Radius = Radius;
		polygonShape.Centroid = Centroid;
		polygonShape.Vertices = new Vertices(Vertices);
		polygonShape.Normals = Normals;
		return polygonShape;
	}

	public void Set(Vertices vertices)
	{
		Vertices = new Vertices(vertices);
		Normals = new Vertices(vertices.Count);
		for (int i = 0; i < vertices.Count; i++)
		{
			int index = i;
			int index2 = ((i + 1 < vertices.Count) ? (i + 1) : 0);
			Vector2 vector = Vertices[index2] - Vertices[index];
			Vector2 item = new Vector2(1f * vector.Y, -1f * vector.X);
			item.Normalize();
			Normals.Add(item);
		}
		Centroid = ComputeCentroid(Vertices);
	}

	private static Vector2 ComputeCentroid(Vertices vertices)
	{
		Vector2 vector = new Vector2(0f, 0f);
		float num = 0f;
		if (vertices.Count == 2)
		{
			return 0.5f * (vertices[0] + vertices[1]);
		}
		Vector2 vector2 = new Vector2(0f, 0f);
		for (int i = 0; i < vertices.Count; i++)
		{
			Vector2 vector3 = vector2;
			Vector2 vector4 = vertices[i];
			Vector2 vector5 = ((i + 1 < vertices.Count) ? vertices[i + 1] : vertices[0]);
			Vector2 a = vector4 - vector3;
			Vector2 b = vector5 - vector3;
			float num2 = MathUtils.Cross(a, b);
			float num3 = 0.5f * num2;
			num += num3;
			vector += num3 * (1f / 3f) * (vector3 + vector4 + vector5);
		}
		return vector * (1f / num);
	}

	public void SetAsBox(float halfWidth, float halfHeight)
	{
		Set(PolygonTools.CreateRectangle(halfWidth, halfHeight));
	}

	public void SetAsBox(float halfWidth, float halfHeight, Vector2 center, float angle)
	{
		Set(PolygonTools.CreateRectangle(halfWidth, halfHeight, center, angle));
	}

	public void SetAsEdge(Vector2 start, Vector2 end)
	{
		Set(PolygonTools.CreateEdge(start, end));
	}

	public override bool TestPoint(ref Transform transform, ref Vector2 point)
	{
		Vector2 vector = MathUtils.MultiplyT(ref transform.R, point - transform.Position);
		for (int i = 0; i < Vertices.Count; i++)
		{
			float num = Vector2.Dot(Normals[i], vector - Vertices[i]);
			if (num > 0f)
			{
				return false;
			}
		}
		return true;
	}

	public override bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex)
	{
		output = default(RayCastOutput);
		Vector2 vector = MathUtils.MultiplyT(ref transform.R, input.Point1 - transform.Position);
		Vector2 vector2 = MathUtils.MultiplyT(ref transform.R, input.Point2 - transform.Position);
		Vector2 vector3 = vector2 - vector;
		if (Vertices.Count == 2)
		{
			Vector2 vector4 = Vertices[0];
			Vector2 vector5 = Vertices[1];
			Vector2 vector6 = Normals[0];
			float num = Vector2.Dot(vector6, vector4 - vector);
			float num2 = Vector2.Dot(vector6, vector3);
			if (num2 == 0f)
			{
				return false;
			}
			float num3 = num / num2;
			if (num3 < 0f || 1f < num3)
			{
				return false;
			}
			Vector2 vector7 = vector + num3 * vector3;
			Vector2 vector8 = vector5 - vector4;
			float num4 = Vector2.Dot(vector8, vector8);
			if (num4 == 0f)
			{
				return false;
			}
			float num5 = Vector2.Dot(vector7 - vector4, vector8) / num4;
			if (num5 < 0f || 1f < num5)
			{
				return false;
			}
			output.Fraction = num3;
			if (num > 0f)
			{
				output.Normal = -vector6;
			}
			else
			{
				output.Normal = vector6;
			}
			return true;
		}
		float num6 = 0f;
		float num7 = input.MaxFraction;
		int num8 = -1;
		for (int i = 0; i < Vertices.Count; i++)
		{
			float num9 = Vector2.Dot(Normals[i], Vertices[i] - vector);
			float num10 = Vector2.Dot(Normals[i], vector3);
			if (num10 == 0f)
			{
				if (num9 < 0f)
				{
					return false;
				}
			}
			else if (num10 < 0f && num9 < num6 * num10)
			{
				num6 = num9 / num10;
				num8 = i;
			}
			else if (num10 > 0f && num9 < num7 * num10)
			{
				num7 = num9 / num10;
			}
			if (num7 < num6)
			{
				return false;
			}
		}
		if (num8 >= 0)
		{
			output.Fraction = num6;
			output.Normal = MathUtils.Multiply(ref transform.R, Normals[num8]);
			return true;
		}
		return false;
	}

	public override void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex)
	{
		Vector2 vector = MathUtils.Multiply(ref transform, Vertices[0]);
		Vector2 vector2 = vector;
		for (int i = 1; i < Vertices.Count; i++)
		{
			Vector2 value = MathUtils.Multiply(ref transform, Vertices[i]);
			vector = Vector2.Min(vector, value);
			vector2 = Vector2.Max(vector2, value);
		}
		Vector2 vector3 = new Vector2(Radius, Radius);
		aabb.LowerBound = vector - vector3;
		aabb.UpperBound = vector2 + vector3;
	}

	public override void ComputeMass(out MassData massData, float density)
	{
		if (Vertices.Count == 2)
		{
			massData.Center = 0.5f * (Vertices[0] + Vertices[1]);
			massData.Mass = 0f;
			massData.Inertia = 0f;
			return;
		}
		Vector2 center = new Vector2(0f, 0f);
		float num = 0f;
		float num2 = 0f;
		Vector2 vector = new Vector2(0f, 0f);
		for (int i = 0; i < Vertices.Count; i++)
		{
			Vector2 vector2 = vector;
			Vector2 vector3 = Vertices[i];
			Vector2 vector4 = ((i + 1 < Vertices.Count) ? Vertices[i + 1] : Vertices[0]);
			Vector2 a = vector3 - vector2;
			Vector2 b = vector4 - vector2;
			MathUtils.Cross(ref a, ref b, out var c);
			float num3 = 0.5f * c;
			num += num3;
			center += num3 * (1f / 3f) * (vector2 + vector3 + vector4);
			float x = vector2.X;
			float y = vector2.Y;
			float x2 = a.X;
			float y2 = a.Y;
			float x3 = b.X;
			float y3 = b.Y;
			float num4 = 1f / 3f * (0.25f * (x2 * x2 + x3 * x2 + x3 * x3) + (x * x2 + x * x3)) + 0.5f * x * x;
			float num5 = 1f / 3f * (0.25f * (y2 * y2 + y3 * y2 + y3 * y3) + (y * y2 + y * y3)) + 0.5f * y * y;
			num2 += c * (num4 + num5);
		}
		massData.Mass = density * num;
		center *= 1f / num;
		massData.Center = center;
		massData.Inertia = density * num2;
	}
}
