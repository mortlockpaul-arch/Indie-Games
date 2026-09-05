using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision;

public struct DistanceProxy
{
	internal FixedArray2<Vector2> Buffer;

	internal float Radius;

	internal Vertices Vertices;

	public void Set(Shape shape, int index)
	{
		switch (shape.ShapeType)
		{
		case ShapeType.Circle:
		{
			CircleShape circleShape = (CircleShape)shape;
			Vertices = new Vertices(1);
			Vertices.Add(circleShape.Position);
			Radius = circleShape.Radius;
			break;
		}
		case ShapeType.Polygon:
		{
			PolygonShape polygonShape = (PolygonShape)shape;
			Vertices = polygonShape.Vertices;
			Radius = polygonShape.Radius;
			break;
		}
		case ShapeType.Loop:
		{
			LoopShape loopShape = (LoopShape)shape;
			Buffer[0] = loopShape.Vertices[index];
			if (index + 1 < loopShape.Vertices.Count)
			{
				Buffer[1] = loopShape.Vertices[index + 1];
			}
			else
			{
				Buffer[1] = loopShape.Vertices[0];
			}
			Vertices = new Vertices(2);
			Vertices.Add(Buffer[0]);
			Vertices.Add(Buffer[1]);
			Radius = loopShape.Radius;
			break;
		}
		case ShapeType.Edge:
		{
			EdgeShape edgeShape = (EdgeShape)shape;
			Vertices = new Vertices(2);
			Vertices.Add(edgeShape.Vertex1);
			Vertices.Add(edgeShape.Vertex2);
			Radius = edgeShape.Radius;
			break;
		}
		}
	}

	public int GetSupport(Vector2 direction)
	{
		int result = 0;
		float num = Vector2.Dot(Vertices[0], direction);
		for (int i = 1; i < Vertices.Count; i++)
		{
			float num2 = Vector2.Dot(Vertices[i], direction);
			if (num2 > num)
			{
				result = i;
				num = num2;
			}
		}
		return result;
	}

	public Vector2 GetSupportVertex(Vector2 direction)
	{
		int index = 0;
		float num = Vector2.Dot(Vertices[0], direction);
		for (int i = 1; i < Vertices.Count; i++)
		{
			float num2 = Vector2.Dot(Vertices[i], direction);
			if (num2 > num)
			{
				index = i;
				num = num2;
			}
		}
		return Vertices[index];
	}

	public Vector2 GetVertex(int index)
	{
		return Vertices[index];
	}
}
