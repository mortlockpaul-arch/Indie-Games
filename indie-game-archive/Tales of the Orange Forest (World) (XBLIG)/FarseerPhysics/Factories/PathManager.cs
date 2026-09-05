using System;
using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using Path = FarseerPhysics.Common.Path;

namespace FarseerPhysics.Factories;

public static class PathManager
{
	public enum LinkType
	{
		Revolute,
		Slider
	}

	public static void ConvertPathToEdges(Path path, Body body, int subdivisions)
	{
		List<Vector2> vertices = path.GetVertices(subdivisions);
		for (int i = 1; i < vertices.Count; i++)
		{
			body.CreateFixture(new PolygonShape(PolygonTools.CreateEdge(vertices[i], vertices[i - 1])), 0f);
		}
		if (path.Closed)
		{
			body.CreateFixture(new PolygonShape(PolygonTools.CreateEdge(vertices[vertices.Count - 1], vertices[0])), 0f);
		}
	}

	public static void ConvertPathToPolygon(Path path, Body body, float density, int subdivisions)
	{
		if (!path.Closed)
		{
			throw new Exception("The path must be closed to convert to a polygon.");
		}
		List<Vector2> vertices = path.GetVertices(subdivisions);
		List<Vertices> list = EarclipDecomposer.ConvexPartition(new Vertices(vertices));
		foreach (Vertices item in list)
		{
			body.CreateFixture(new PolygonShape(item), density);
		}
	}

	public static List<Body> EvenlyDistibuteShapesAlongPath(World world, Path path, IEnumerable<Shape> shapes, BodyType type, int copies, float density)
	{
		List<Vector3> list = path.SubdivideEvenly(copies);
		List<Body> list2 = new List<Body>();
		for (int i = 0; i < list.Count; i++)
		{
			Body body = world.CreateBody();
			body.BodyType = type;
			body.Position = new Vector2(list[i].X, list[i].Y);
			body.Rotation = list[i].Z;
			foreach (Shape shape in shapes)
			{
				body.CreateFixture(shape, density);
			}
			list2.Add(body);
		}
		return list2;
	}

	public static List<Body> EvenlyDistibuteShapesAlongPath(World world, Path path, Shape shape, BodyType type, int copies, float density)
	{
		List<Shape> list = new List<Shape>(1);
		list.Add(shape);
		return EvenlyDistibuteShapesAlongPath(world, path, list, type, copies, density);
	}

	public static void MoveBodyOnPath(Path path, Body body, float time, float strength, float timeStep)
	{
		Vector2 position = path.GetPosition(time);
		Vector2 vector = body.Position - position;
		Vector2 vector2 = vector / timeStep * strength;
		body.LinearVelocity = -vector2;
	}

	public static void AttachBodiesWithRevoluteJoint(World world, List<Body> bodies, Vector2 localAnchorA, Vector2 localAnchorB, bool connectFirstAndLast, bool collideConnected)
	{
		for (int i = 1; i < bodies.Count; i++)
		{
			RevoluteJoint revoluteJoint = new RevoluteJoint(bodies[i], bodies[i - 1], localAnchorA, localAnchorB);
			revoluteJoint.CollideConnected = collideConnected;
			world.AddJoint(revoluteJoint);
		}
		if (connectFirstAndLast)
		{
			RevoluteJoint revoluteJoint2 = new RevoluteJoint(bodies[0], bodies[bodies.Count - 1], localAnchorA, localAnchorB);
			revoluteJoint2.CollideConnected = collideConnected;
			world.AddJoint(revoluteJoint2);
		}
	}

	public static void AttachBodiesWithSliderJoint(World world, List<Body> bodies, Vector2 localAnchorA, Vector2 localAnchorB, bool connectFirstAndLast, bool collideConnected, float minLength, float maxLength)
	{
		for (int i = 1; i < bodies.Count; i++)
		{
			SliderJoint sliderJoint = new SliderJoint(bodies[i], bodies[i - 1], localAnchorA, localAnchorB, minLength, maxLength);
			sliderJoint.CollideConnected = collideConnected;
			world.AddJoint(sliderJoint);
		}
		if (connectFirstAndLast)
		{
			SliderJoint sliderJoint2 = new SliderJoint(bodies[0], bodies[bodies.Count - 1], localAnchorA, localAnchorB, minLength, maxLength);
			sliderJoint2.CollideConnected = collideConnected;
			world.AddJoint(sliderJoint2);
		}
	}
}
