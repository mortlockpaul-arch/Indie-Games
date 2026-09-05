using System;
using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Factories;

public static class FixtureFactory
{
	public static Fixture CreateEdge(World world, Vector2 start, Vector2 end)
	{
		return CreateEdge(world, start, end, null);
	}

	public static Fixture CreateEdge(World world, Vector2 start, Vector2 end, object userData)
	{
		Body body = BodyFactory.CreateBody(world);
		return CreateEdge(start, end, body, userData);
	}

	public static Fixture CreateEdge(Vector2 start, Vector2 end, Body body)
	{
		return CreateEdge(start, end, body, null);
	}

	public static Fixture CreateEdge(Vector2 start, Vector2 end, Body body, object userData)
	{
		EdgeShape shape = new EdgeShape(start, end);
		return body.CreateFixture(shape, userData);
	}

	public static Fixture CreateLoopShape(World world, Vertices vertices, float density)
	{
		return CreateLoopShape(world, vertices, density, null);
	}

	public static Fixture CreateLoopShape(World world, Vertices vertices, float density, object userData)
	{
		return CreateLoopShape(world, vertices, density, Vector2.Zero, userData);
	}

	public static Fixture CreateLoopShape(World world, Vertices vertices, float density, Vector2 position)
	{
		return CreateLoopShape(world, vertices, density, position, null);
	}

	public static Fixture CreateLoopShape(World world, Vertices vertices, float density, Vector2 position, object userData)
	{
		Body body = BodyFactory.CreateBody(world, position);
		return CreateLoopShape(vertices, density, body, userData);
	}

	public static Fixture CreateLoopShape(Vertices vertices, float density, Body body)
	{
		return CreateLoopShape(vertices, density, body, null);
	}

	public static Fixture CreateLoopShape(Vertices vertices, float density, Body body, object userData)
	{
		LoopShape shape = new LoopShape(vertices, density);
		return body.CreateFixture(shape, userData);
	}

	public static Fixture CreateRectangle(World world, float width, float height, float density)
	{
		return CreateRectangle(world, width, height, density, null);
	}

	public static Fixture CreateRectangle(World world, float width, float height, float density, object userData)
	{
		return CreateRectangle(world, width, height, density, Vector2.Zero, userData);
	}

	public static Fixture CreateRectangle(World world, float width, float height, float density, Vector2 position)
	{
		return CreateRectangle(world, width, height, density, position, null);
	}

	public static Fixture CreateRectangle(World world, float width, float height, float density, Vector2 position, object userData)
	{
		if (width <= 0f)
		{
			throw new ArgumentOutOfRangeException("width", "Width must be more than 0 meters");
		}
		if (height <= 0f)
		{
			throw new ArgumentOutOfRangeException("height", "Height must be more than 0 meters");
		}
		Body body = BodyFactory.CreateBody(world, position);
		Vertices vertices = PolygonTools.CreateRectangle(width / 2f, height / 2f);
		PolygonShape shape = new PolygonShape(vertices, density);
		return body.CreateFixture(shape, userData);
	}

	public static Fixture CreateRectangle(float width, float height, float density, Vector2 offset, Body body, object userData)
	{
		Vertices vertices = PolygonTools.CreateRectangle(width / 2f, height / 2f);
		vertices.Translate(ref offset);
		PolygonShape shape = new PolygonShape(vertices, density);
		return body.CreateFixture(shape, userData);
	}

	public static Fixture CreateRectangle(float width, float height, float density, Vector2 offset, Body body)
	{
		return CreateRectangle(width, height, density, offset, body, null);
	}

	public static Fixture CreateCircle(World world, float radius, float density)
	{
		return CreateCircle(world, radius, density, null);
	}

	public static Fixture CreateCircle(World world, float radius, float density, object userData)
	{
		return CreateCircle(world, radius, density, Vector2.Zero, userData);
	}

	public static Fixture CreateCircle(World world, float radius, float density, Vector2 position)
	{
		return CreateCircle(world, radius, density, position, null);
	}

	public static Fixture CreateCircle(World world, float radius, float density, Vector2 position, object userData)
	{
		Body body = BodyFactory.CreateBody(world, position);
		return CreateCircle(radius, density, body, userData);
	}

	public static Fixture CreateCircle(float radius, float density, Body body)
	{
		return CreateCircle(radius, density, body, null);
	}

	public static Fixture CreateCircle(float radius, float density, Body body, object userData)
	{
		if (radius <= 0f)
		{
			throw new ArgumentOutOfRangeException("radius", "Radius must be more than 0 meters");
		}
		CircleShape shape = new CircleShape(radius, density);
		return body.CreateFixture(shape, userData);
	}

	public static Fixture CreateCircle(float radius, float density, Body body, Vector2 offset)
	{
		return CreateCircle(radius, density, body, offset, null);
	}

	public static Fixture CreateCircle(float radius, float density, Body body, Vector2 offset, object userData)
	{
		if (radius <= 0f)
		{
			throw new ArgumentOutOfRangeException("radius", "Radius must be more than 0 meters");
		}
		CircleShape circleShape = new CircleShape(radius, density);
		circleShape.Position = offset;
		return body.CreateFixture(circleShape, userData);
	}

	public static Fixture CreateEllipse(World world, float xRadius, float yRadius, int edges, float density)
	{
		return CreateEllipse(world, xRadius, yRadius, edges, density, null);
	}

	public static Fixture CreateEllipse(World world, float xRadius, float yRadius, int edges, float density, object userData)
	{
		return CreateEllipse(world, xRadius, yRadius, edges, density, Vector2.Zero, userData);
	}

	public static Fixture CreateEllipse(World world, float xRadius, float yRadius, int edges, float density, Vector2 position)
	{
		return CreateEllipse(world, xRadius, yRadius, edges, density, position, null);
	}

	public static Fixture CreateEllipse(World world, float xRadius, float yRadius, int edges, float density, Vector2 position, object userData)
	{
		Body body = BodyFactory.CreateBody(world, position);
		return CreateEllipse(xRadius, yRadius, edges, density, body, userData);
	}

	public static Fixture CreateEllipse(float xRadius, float yRadius, int edges, float density, Body body)
	{
		return CreateEllipse(xRadius, yRadius, edges, density, body, null);
	}

	public static Fixture CreateEllipse(float xRadius, float yRadius, int edges, float density, Body body, object userData)
	{
		if (xRadius <= 0f)
		{
			throw new ArgumentOutOfRangeException("xRadius", "X-radius must be more than 0");
		}
		if (yRadius <= 0f)
		{
			throw new ArgumentOutOfRangeException("yRadius", "Y-radius must be more than 0");
		}
		Vertices vertices = PolygonTools.CreateEllipse(xRadius, yRadius, edges);
		PolygonShape shape = new PolygonShape(vertices, density);
		return body.CreateFixture(shape, userData);
	}

	public static Fixture CreatePolygon(World world, Vertices vertices, float density)
	{
		return CreatePolygon(world, vertices, density, null);
	}

	public static Fixture CreatePolygon(World world, Vertices vertices, float density, object userData)
	{
		return CreatePolygon(world, vertices, density, Vector2.Zero, userData);
	}

	public static Fixture CreatePolygon(World world, Vertices vertices, float density, Vector2 position)
	{
		return CreatePolygon(world, vertices, density, position, null);
	}

	public static Fixture CreatePolygon(World world, Vertices vertices, float density, Vector2 position, object userData)
	{
		Body body = BodyFactory.CreateBody(world, position);
		return CreatePolygon(vertices, density, body, userData);
	}

	public static Fixture CreatePolygon(Vertices vertices, float density, Body body)
	{
		return CreatePolygon(vertices, density, body, null);
	}

	public static Fixture CreatePolygon(Vertices vertices, float density, Body body, object userData)
	{
		if (vertices.Count <= 1)
		{
			throw new ArgumentOutOfRangeException("vertices", "Too few points to be a polygon");
		}
		PolygonShape shape = new PolygonShape(vertices, density);
		return body.CreateFixture(shape, userData);
	}

	public static List<Fixture> CreateCompoundPolygon(World world, List<Vertices> list, float density)
	{
		return CreateCompoundPolygon(world, list, density, BodyType.Static);
	}

	public static List<Fixture> CreateCompoundPolygon(World world, List<Vertices> list, float density, BodyType type)
	{
		List<Fixture> list2 = CreateCompoundPolygon(world, list, density, null);
		list2[0].Body.BodyType = type;
		return list2;
	}

	public static List<Fixture> CreateCompoundPolygon(World world, List<Vertices> list, float density, object userData)
	{
		return CreateCompoundPolygon(world, list, density, Vector2.Zero, userData);
	}

	public static List<Fixture> CreateCompoundPolygon(World world, List<Vertices> list, float density, Vector2 position)
	{
		return CreateCompoundPolygon(world, list, density, position, null);
	}

	public static List<Fixture> CreateCompoundPolygon(World world, List<Vertices> list, float density, Vector2 position, object userData)
	{
		Body body = BodyFactory.CreateBody(world, position);
		return CreateCompoundPolygon(list, density, body, userData);
	}

	public static List<Fixture> CreateCompoundPolygon(List<Vertices> list, float density, Body body)
	{
		return CreateCompoundPolygon(list, density, body, null);
	}

	public static List<Fixture> CreateCompoundPolygon(List<Vertices> list, float density, Body body, object userData)
	{
		List<Fixture> list2 = new List<Fixture>(list.Count);
		foreach (Vertices item in list)
		{
			PolygonShape shape = new PolygonShape(item, density);
			list2.Add(body.CreateFixture(shape, userData));
		}
		return list2;
	}

	public static List<Fixture> CreateGear(World world, float radius, int numberOfTeeth, float tipPercentage, float toothHeight, float density)
	{
		return CreateGear(world, radius, numberOfTeeth, tipPercentage, toothHeight, density, null);
	}

	public static List<Fixture> CreateGear(World world, float radius, int numberOfTeeth, float tipPercentage, float toothHeight, float density, object userData)
	{
		Vertices vertices = PolygonTools.CreateGear(radius, numberOfTeeth, tipPercentage, toothHeight);
		if (!vertices.IsConvex())
		{
			List<Vertices> list = EarclipDecomposer.ConvexPartition(vertices);
			return CreateCompoundPolygon(world, list, density, userData);
		}
		List<Fixture> list2 = new List<Fixture>();
		list2.Add(CreatePolygon(world, vertices, density, userData));
		return list2;
	}

	public static List<Fixture> CreateCapsule(World world, float height, float topRadius, int topEdges, float bottomRadius, int bottomEdges, float density, Vector2 position, object userData)
	{
		Vertices vertices = PolygonTools.CreateCapsule(height, topRadius, topEdges, bottomRadius, bottomEdges);
		if (vertices.Count >= Settings.MaxPolygonVertices)
		{
			List<Vertices> list = EarclipDecomposer.ConvexPartition(vertices);
			List<Fixture> list2 = CreateCompoundPolygon(world, list, density, userData);
			list2[0].Body.Position = position;
			return list2;
		}
		List<Fixture> list3 = new List<Fixture>();
		list3.Add(CreatePolygon(world, vertices, density, userData));
		return list3;
	}

	public static List<Fixture> CreateCapsule(World world, float height, float topRadius, int topEdges, float bottomRadius, int bottomEdges, float density, Vector2 position)
	{
		return CreateCapsule(world, height, topRadius, topEdges, bottomRadius, bottomEdges, density, position, null);
	}

	public static List<Fixture> CreateCapsule(World world, float height, float endRadius, float density)
	{
		return CreateCapsule(world, height, endRadius, density, null);
	}

	public static List<Fixture> CreateCapsule(World world, float height, float endRadius, float density, object userData)
	{
		Vertices item = PolygonTools.CreateRectangle(endRadius, height / 2f);
		List<Vertices> list = new List<Vertices>();
		list.Add(item);
		List<Fixture> list2 = CreateCompoundPolygon(world, list, density, userData);
		CircleShape circleShape = new CircleShape(endRadius, density);
		circleShape.Position = new Vector2(0f, height / 2f);
		list2.Add(list2[0].Body.CreateFixture(circleShape, userData));
		CircleShape circleShape2 = new CircleShape(endRadius, density);
		circleShape2.Position = new Vector2(0f, 0f - height / 2f);
		list2.Add(list2[0].Body.CreateFixture(circleShape2, userData));
		return list2;
	}

	public static List<Fixture> CreateRoundedRectangle(World world, float width, float height, float xRadius, float yRadius, int segments, float density, Vector2 position, object userData)
	{
		Vertices vertices = PolygonTools.CreateRoundedRectangle(width, height, xRadius, yRadius, segments);
		if (vertices.Count >= Settings.MaxPolygonVertices)
		{
			List<Vertices> list = EarclipDecomposer.ConvexPartition(vertices);
			List<Fixture> list2 = CreateCompoundPolygon(world, list, density, userData);
			list2[0].Body.Position = position;
			return list2;
		}
		List<Fixture> list3 = new List<Fixture>();
		list3.Add(CreatePolygon(world, vertices, density));
		return list3;
	}

	public static List<Fixture> CreateRoundedRectangle(World world, float width, float height, float xRadius, float yRadius, int segments, float density, Vector2 position)
	{
		return CreateRoundedRectangle(world, width, height, xRadius, yRadius, segments, density, position, null);
	}

	public static List<Fixture> CreateRoundedRectangle(World world, float width, float height, float xRadius, float yRadius, int segments, float density)
	{
		return CreateRoundedRectangle(world, width, height, xRadius, yRadius, segments, density, null);
	}

	public static List<Fixture> CreateRoundedRectangle(World world, float width, float height, float xRadius, float yRadius, int segments, float density, object userData)
	{
		return CreateRoundedRectangle(world, width, height, xRadius, yRadius, segments, density, Vector2.Zero, userData);
	}

	public static BreakableBody CreateBreakableBody(World world, Vertices vertices, float density)
	{
		return CreateBreakableBody(world, vertices, density, null);
	}

	public static BreakableBody CreateBreakableBody(World world, Vertices vertices, float density, object userData)
	{
		return CreateBreakableBody(world, vertices, density, Vector2.Zero, userData);
	}

	public static BreakableBody CreateBreakableBody(World world, Vertices vertices, float density, Vector2 position, object userData)
	{
		List<Vertices> vertices2 = EarclipDecomposer.ConvexPartition(vertices);
		BreakableBody breakableBody = new BreakableBody(vertices2, world, density, userData);
		breakableBody.MainBody.Position = position;
		world.AddBreakableBody(breakableBody);
		return breakableBody;
	}

	public static BreakableBody CreateBreakableBody(World world, Vertices vertices, float density, Vector2 position)
	{
		return CreateBreakableBody(world, vertices, density, position, null);
	}
}
