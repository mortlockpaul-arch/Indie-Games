using System;
using System.Collections.Generic;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Factories;

public class GeomFactory
{
	private static GeomFactory _instance;

	public static GeomFactory Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GeomFactory();
			}
			return _instance;
		}
	}

	private GeomFactory()
	{
	}

	public Geom CreateRectangleGeom(PhysicsSimulator physicsSimulator, Body body, float width, float height)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CreateRectangleGeom(physicsSimulator, body, width, height, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateRectangleGeom(Body body, float width, float height)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return CreateRectangleGeom(body, width, height, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateRectangleGeom(PhysicsSimulator physicsSimulator, Body body, float width, float height, Vector2 positionOffset, float rotationOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CreateRectangleGeom(physicsSimulator, body, width, height, positionOffset, rotationOffset, 0f);
	}

	public Geom CreateRectangleGeom(Body body, float width, float height, Vector2 positionOffset, float rotationOffset)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return CreateRectangleGeom(body, width, height, positionOffset, rotationOffset, 0f);
	}

	public Geom CreateRectangleGeom(PhysicsSimulator physicsSimulator, Body body, float width, float height, float collisionGridSize)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CreateRectangleGeom(physicsSimulator, body, width, height, Vector2.Zero, 0f, collisionGridSize);
	}

	public Geom CreateRectangleGeom(Body body, float width, float height, float collisionGridSize)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return CreateRectangleGeom(body, width, height, Vector2.Zero, 0f, collisionGridSize);
	}

	public Geom CreateRectangleGeom(PhysicsSimulator physicsSimulator, Body body, float width, float height, Vector2 positionOffset, float rotationOffset, float collisionGridSize)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		Geom geom = CreateRectangleGeom(body, width, height, positionOffset, rotationOffset, collisionGridSize);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateRectangleGeom(Body body, float width, float height, Vector2 positionOffset, float rotationOffset, float collisionGridSize)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (width <= 0f)
		{
			throw new ArgumentOutOfRangeException("width", "Width must be more than 0");
		}
		if (height <= 0f)
		{
			throw new ArgumentOutOfRangeException("height", "Height must be more than 0");
		}
		Vertices vertices = Vertices.CreateRectangle(width, height);
		return new Geom(body, vertices, positionOffset, rotationOffset, collisionGridSize);
	}

	public Geom CreateEllipseGeom(PhysicsSimulator physicsSimulator, Body body, float xRadius, float yRadius, int numberOfEdges)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return CreateEllipseGeom(physicsSimulator, body, xRadius, yRadius, numberOfEdges, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateEllipseGeom(Body body, float xRadius, float yRadius, int numberOfEdges)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CreateEllipseGeom(body, xRadius, yRadius, numberOfEdges, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateEllipseGeom(PhysicsSimulator physicsSimulator, Body body, float xRadius, float yRadius, int numberOfEdges, Vector2 offset, float rotationOffset)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return CreateEllipseGeom(physicsSimulator, body, xRadius, yRadius, numberOfEdges, offset, rotationOffset, 0f);
	}

	public Geom CreateEllipseGeom(Body body, float xRadius, float yRadius, int numberOfEdges, Vector2 offset, float rotationOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CreateEllipseGeom(body, xRadius, yRadius, numberOfEdges, offset, rotationOffset, 0f);
	}

	public Geom CreateEllipseGeom(PhysicsSimulator physicsSimulator, Body body, float xRadius, float yRadius, int numberOfEdges, float collisionGridSize)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return CreateEllipseGeom(physicsSimulator, body, xRadius, yRadius, numberOfEdges, Vector2.Zero, 0f, collisionGridSize);
	}

	public Geom CreateEllipseGeom(Body body, float xRadius, float yRadius, int numberOfEdges, float collisionGridSize)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CreateEllipseGeom(body, xRadius, yRadius, numberOfEdges, Vector2.Zero, 0f, collisionGridSize);
	}

	public Geom CreateEllipseGeom(PhysicsSimulator physicsSimulator, Body body, float xRadius, float yRadius, int numberOfEdges, Vector2 offset, float rotationOffset, float collisionGridSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Geom geom = CreateEllipseGeom(body, xRadius, yRadius, numberOfEdges, offset, rotationOffset, collisionGridSize);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateEllipseGeom(Body body, float xRadius, float yRadius, int numberOfEdges, Vector2 offset, float rotationOffset, float collisionGridSize)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (xRadius <= 0f)
		{
			throw new ArgumentOutOfRangeException("xRadius", "xRadius must be more than 0");
		}
		if (yRadius <= 0f)
		{
			throw new ArgumentOutOfRangeException("yRadius", "yRadius must be more than 0");
		}
		Vertices vertices = Vertices.CreateEllipse(xRadius, yRadius, numberOfEdges);
		return new Geom(body, vertices, offset, rotationOffset, collisionGridSize);
	}

	public Geom CreateCircleGeom(PhysicsSimulator physicsSimulator, Body body, float radius, int numberOfEdges)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CreateCircleGeom(physicsSimulator, body, radius, numberOfEdges, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateCircleGeom(Body body, float radius, int numberOfEdges)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return CreateCircleGeom(body, radius, numberOfEdges, Vector2.Zero, 0f, 0f);
	}

	public Geom CreateCircleGeom(PhysicsSimulator physicsSimulator, Body body, float radius, int numberOfEdges, Vector2 offset, float rotationOffset)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CreateCircleGeom(physicsSimulator, body, radius, numberOfEdges, offset, rotationOffset, 0f);
	}

	public Geom CreateCircleGeom(Body body, float radius, int numberOfEdges, Vector2 offset, float rotationOffset)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return CreateCircleGeom(body, radius, numberOfEdges, offset, rotationOffset, 0f);
	}

	public Geom CreateCircleGeom(PhysicsSimulator physicsSimulator, Body body, float radius, int numberOfEdges, float collisionGridSize)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CreateCircleGeom(physicsSimulator, body, radius, numberOfEdges, Vector2.Zero, 0f, collisionGridSize);
	}

	public Geom CreateCircleGeom(Body body, float radius, int numberOfEdges, float collisionGridSize)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return CreateCircleGeom(body, radius, numberOfEdges, Vector2.Zero, 0f, collisionGridSize);
	}

	public Geom CreateCircleGeom(PhysicsSimulator physicsSimulator, Body body, float radius, int numberOfEdges, Vector2 offset, float rotationOffset, float collisionGridSize)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		Geom geom = CreateCircleGeom(body, radius, numberOfEdges, offset, rotationOffset, collisionGridSize);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateCircleGeom(Body body, float radius, int numberOfEdges, Vector2 offset, float rotationOffset, float collisionGridSize)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (radius <= 0f)
		{
			throw new ArgumentOutOfRangeException("radius", "Radius must be more than 0");
		}
		Vertices vertices = Vertices.CreateCircle(radius, numberOfEdges);
		return new Geom(body, vertices, offset, rotationOffset, collisionGridSize);
	}

	public Geom CreatePolygonGeom(PhysicsSimulator physicsSimulator, Body body, Vertices vertices, float collisionGridSize)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return CreatePolygonGeom(physicsSimulator, body, vertices, Vector2.Zero, 0f, collisionGridSize);
	}

	public Geom CreatePolygonGeom(Body body, Vertices vertices, float collisionGridSize)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return CreatePolygonGeom(body, vertices, Vector2.Zero, 0f, collisionGridSize);
	}

	public Geom CreatePolygonGeom(PhysicsSimulator physicsSimulator, Body body, Vertices vertices, Vector2 positionOffset, float rotationOffset, float collisionGridSize)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Geom geom = CreatePolygonGeom(body, vertices, positionOffset, rotationOffset, collisionGridSize);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreatePolygonGeom(Body body, Vertices vertices, Vector2 positionOffset, float rotationOffset, float collisionGridSize)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (body == null)
		{
			throw new ArgumentNullException("body", "Body must not be null");
		}
		if (vertices == null)
		{
			throw new ArgumentNullException("vertices", "Vertices must not be null");
		}
		Vector2 vector = vertices.GetCentroid();
		Vector2.Multiply(ref vector, -1f, ref vector);
		vertices.Translate(ref vector);
		return new Geom(body, vertices, positionOffset, rotationOffset, collisionGridSize);
	}

	public List<Geom> CreateSATPolygonGeom(PhysicsSimulator physicsSimulator, Body body, Vertices vertices, int maxGeoms)
	{
		List<Geom> list = CreateSATPolygonGeom(body, vertices, maxGeoms);
		foreach (Geom item in list)
		{
			physicsSimulator.Add(item);
		}
		return list;
	}

	public List<Geom> CreateSATPolygonGeom(Body body, Vertices vertices, int numberOfGeoms)
	{
		if (body == null)
		{
			throw new ArgumentNullException("body", "Body must not be null");
		}
		if (vertices == null)
		{
			throw new ArgumentNullException("vertices", "Vertices must not be null");
		}
		return Vertices.DecomposeGeom(vertices, body, numberOfGeoms);
	}

	public Geom CreateGeom(PhysicsSimulator physicsSimulator, Body body, Geom geometry)
	{
		Geom geom = CreateGeom(body, geometry);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateGeom(Body body, Geom geometry)
	{
		return new Geom(body, geometry);
	}

	public Geom CreateGeom(PhysicsSimulator physicsSimulator, Body body, Geom geometry, Vector2 offset, float rotationOffset)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Geom geom = CreateGeom(body, geometry, offset, rotationOffset);
		physicsSimulator.Add(geom);
		return geom;
	}

	public Geom CreateGeom(Body body, Geom geometry, Vector2 offset, float rotationOffset)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return new Geom(body, geometry, offset, rotationOffset);
	}
}
