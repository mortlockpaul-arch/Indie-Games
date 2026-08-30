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
	public static Fixture CreateEdge(World world, Vector2 start, Vector2 end, float density)
	{
		Body body = BodyFactory.CreateBody(world);
		Vertices vertices = PolygonTools.CreateEdge(start, end);
		PolygonShape shape = new PolygonShape(vertices);
		return body.CreateFixture(shape, density);
	}

	public static Fixture CreateRectangle(World world, float width, float height, float density)
	{
		return CreateRectangle(world, width, height, density, Vector2.Zero, null);
	}

	public static Fixture CreateRectangle(World world, float width, float height, float density, Vector2 position)
	{
		return CreateRectangle(world, width, height, density, position, null);
	}

	public static Fixture CreateRectangle(World world, float width, float height, float density, Vector2 offset, Body body)
	{
		if (width <= 0f)
		{
			throw new ArgumentOutOfRangeException("width", "Width must be more than 0");
		}
		if (height <= 0f)
		{
			throw new ArgumentOutOfRangeException("height", "Height must be more than 0");
		}
		if (density <= 0f)
		{
			throw new ArgumentOutOfRangeException("density", "Density must be more than 0");
		}
		if (body != null)
		{
			Vertices vertices = PolygonTools.CreateRectangle(width / 2f, height / 2f);
			vertices.Translate(ref offset);
			PolygonShape shape = new PolygonShape(vertices);
			return body.CreateFixture(shape, density);
		}
		Body body2 = BodyFactory.CreateBody(world, offset);
		Vertices vertices2 = PolygonTools.CreateRectangle(width / 2f, height / 2f);
		PolygonShape shape2 = new PolygonShape(vertices2);
		return body2.CreateFixture(shape2, density);
	}

	public static Fixture CreateCircle(World world, float radius, float density)
	{
		return CreateCircle(world, radius, density, Vector2.Zero, null);
	}

	public static Fixture CreateCircle(World world, float radius, float density, Vector2 position)
	{
		return CreateCircle(world, radius, density, position, null);
	}

	public static Fixture CreateCircle(World world, float radius, float density, Vector2 offset, Body body)
	{
		if (radius <= 0f)
		{
			throw new ArgumentOutOfRangeException("radius", "Radius must be more than 0");
		}
		if (density <= 0f)
		{
			throw new ArgumentOutOfRangeException("density", "Density must be more than 0");
		}
		if (body != null)
		{
			CircleShape circleShape = new CircleShape(radius);
			circleShape.Position = offset;
			return body.CreateFixture(circleShape, density);
		}
		Body body2 = BodyFactory.CreateBody(world, offset);
		CircleShape shape = new CircleShape(radius);
		return body2.CreateFixture(shape, density);
	}

	public static Fixture CreateEllipse(World world, float xRadius, float yRadius, int edges, float density)
	{
		return CreateEllipse(world, xRadius, yRadius, edges, density, Vector2.Zero);
	}

	public static Fixture CreateEllipse(World world, float xRadius, float yRadius, int edges, float density, Vector2 position)
	{
		if (xRadius <= 0f)
		{
			throw new ArgumentOutOfRangeException("xRadius", "X-radius must be more than 0");
		}
		if (yRadius <= 0f)
		{
			throw new ArgumentOutOfRangeException("yRadius", "Y-radius must be more than 0");
		}
		if (density <= 0f)
		{
			throw new ArgumentOutOfRangeException("density", "Density must be more than 0");
		}
		Body body = BodyFactory.CreateBody(world, position);
		Vertices vertices = PolygonTools.CreateEllipse(xRadius, yRadius, edges);
		PolygonShape shape = new PolygonShape(vertices);
		return body.CreateFixture(shape, density);
	}

	public static Fixture CreatePolygon(World world, Vertices vertices, float density)
	{
		return CreatePolygon(world, vertices, density, Vector2.Zero);
	}

	public static Fixture CreatePolygon(World world, Vertices vertices, float density, Vector2 position)
	{
		if (density <= 0f)
		{
			throw new ArgumentOutOfRangeException("density", "Density must be more than 0");
		}
		Body body = BodyFactory.CreateBody(world, position);
		PolygonShape shape = new PolygonShape(vertices);
		return body.CreateFixture(shape, density);
	}

	public static List<Fixture> CreateCompundPolygon(World world, List<Vertices> list, float density)
	{
		Body body = BodyFactory.CreateBody(world);
		List<Fixture> list2 = new List<Fixture>(list.Count);
		foreach (Vertices item in list)
		{
			PolygonShape shape = new PolygonShape(item);
			list2.Add(body.CreateFixture(shape, density));
		}
		return list2;
	}

	public static List<Fixture> CreateGear(World world, float radius, int numberOfTeeth, float tipPercentage, float toothHeight, float density)
	{
		Vertices vertices = PolygonTools.CreateGear(radius, numberOfTeeth, tipPercentage, toothHeight);
		if (!vertices.IsConvex())
		{
			List<Vertices> list = EarclipDecomposer.ConvexPartition(vertices);
			return CreateCompundPolygon(world, list, density);
		}
		List<Fixture> list2 = new List<Fixture>();
		list2.Add(CreatePolygon(world, vertices, density));
		return list2;
	}

	public static List<Fixture> CreateCapsule(World world, float height, float topRadius, int topEdges, float bottomRadius, int bottomEdges, float density, Vector2 position)
	{
		Vertices vertices = PolygonTools.CreateCapsule(height, topRadius, topEdges, bottomRadius, bottomEdges);
		if (vertices.Count >= 8)
		{
			List<Vertices> list = EarclipDecomposer.ConvexPartition(vertices);
			List<Fixture> list2 = CreateCompundPolygon(world, list, density);
			list2[0].Body.Position = position;
			return list2;
		}
		List<Fixture> list3 = new List<Fixture>();
		list3.Add(CreatePolygon(world, vertices, density));
		return list3;
	}

	public static List<Fixture> CreateCapsule(World world, float height, float endRadius, float density)
	{
		Vertices item = PolygonTools.CreateRectangle(endRadius, height / 2f);
		List<Vertices> list = new List<Vertices>();
		list.Add(item);
		List<Fixture> list2 = CreateCompundPolygon(world, list, density);
		CircleShape circleShape = new CircleShape(endRadius);
		circleShape.Position = new Vector2(0f, height / 2f);
		list2.Add(list2[0].Body.CreateFixture(circleShape, density));
		CircleShape circleShape2 = new CircleShape(endRadius);
		circleShape2.Position = new Vector2(0f, 0f - height / 2f);
		list2.Add(list2[0].Body.CreateFixture(circleShape2, density));
		return list2;
	}

	public static List<Fixture> CreateRoundedRectangle(World world, float width, float height, float xRadius, float yRadius, int segments, float density, Vector2 position)
	{
		Vertices vertices = PolygonTools.CreateRoundedRectangle(width, height, xRadius, yRadius, segments);
		if (vertices.Count >= 8)
		{
			List<Vertices> list = EarclipDecomposer.ConvexPartition(vertices);
			List<Fixture> list2 = CreateCompundPolygon(world, list, density);
			list2[0].Body.Position = position;
			return list2;
		}
		List<Fixture> list3 = new List<Fixture>();
		list3.Add(CreatePolygon(world, vertices, density));
		return list3;
	}

	public static List<Fixture> CreateRoundedRectangle(World world, float width, float height, float xRadius, float yRadius, int segments, float density)
	{
		return CreateRoundedRectangle(world, width, height, xRadius, yRadius, segments, density, Vector2.Zero);
	}

	public static BreakableBody CreateBreakableBody(World world, Vertices vertices, float density)
	{
		return CreateBreakableBody(world, vertices, density, Vector2.Zero);
	}

	public static BreakableBody CreateBreakableBody(World world, Vertices vertices, float density, Vector2 position)
	{
		List<Vertices> vertices2 = EarclipDecomposer.ConvexPartition(vertices);
		BreakableBody breakableBody = new BreakableBody(vertices2, world, density);
		breakableBody.MainBody.Position = position;
		world.AddBreakableBody(breakableBody);
		return breakableBody;
	}
}
