using System;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;

namespace FarseerGames.FarseerPhysics.Factories;

public class BodyFactory
{
	private static BodyFactory _instance;

	public static BodyFactory Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new BodyFactory();
			}
			return _instance;
		}
	}

	private BodyFactory()
	{
	}

	public Body CreateRectangleBody(PhysicsSimulator physicsSimulator, float width, float height, float mass)
	{
		Body body = CreateRectangleBody(width, height, mass);
		physicsSimulator.Add(body);
		return body;
	}

	public Body CreateRectangleBody(float width, float height, float mass)
	{
		if (width <= 0f)
		{
			throw new ArgumentOutOfRangeException("width", "Width must be more than 0");
		}
		if (height <= 0f)
		{
			throw new ArgumentOutOfRangeException("height", "Height must be more than 0");
		}
		if (mass <= 0f)
		{
			throw new ArgumentOutOfRangeException("mass", "Mass must be more than 0");
		}
		Body body = new Body();
		body.Mass = mass;
		body.MomentOfInertia = mass * (width * width + height * height) / 12f;
		return body;
	}

	public Body CreateCircleBody(PhysicsSimulator physicsSimulator, float radius, float mass)
	{
		Body body = CreateCircleBody(radius, mass);
		physicsSimulator.Add(body);
		return body;
	}

	public Body CreateCircleBody(float radius, float mass)
	{
		if (radius <= 0f)
		{
			throw new ArgumentOutOfRangeException("radius", "Radius must be more than 0");
		}
		if (mass <= 0f)
		{
			throw new ArgumentOutOfRangeException("mass", "Mass must be more than 0");
		}
		Body body = new Body();
		body.Mass = mass;
		body.MomentOfInertia = 0.5f * mass * (float)Math.Pow(radius, 2.0);
		return body;
	}

	public Body CreateEllipseBody(PhysicsSimulator physicsSimulator, float xRadius, float yRadius, float mass)
	{
		Body body = CreateEllipseBody(xRadius, yRadius, mass);
		physicsSimulator.Add(body);
		return body;
	}

	public Body CreateEllipseBody(float xRadius, float yRadius, float mass)
	{
		if (xRadius <= 0f)
		{
			throw new ArgumentOutOfRangeException("xRadius", "xRadius must be more than 0");
		}
		if (yRadius <= 0f)
		{
			throw new ArgumentOutOfRangeException("yRadius", "yRadius must be more than 0");
		}
		if (mass <= 0f)
		{
			throw new ArgumentOutOfRangeException("mass", "Mass must be more than 0");
		}
		if (xRadius == yRadius)
		{
			return CreateCircleBody(xRadius, mass);
		}
		Body body = new Body();
		body.Mass = mass;
		body.MomentOfInertia = 0.25f * mass * (xRadius * xRadius + yRadius * yRadius);
		return body;
	}

	public Body CreatePolygonBody(PhysicsSimulator physicsSimulator, Vertices vertices, float mass)
	{
		Body body = CreatePolygonBody(vertices, mass);
		physicsSimulator.Add(body);
		return body;
	}

	public Body CreatePolygonBody(Vertices vertices, float mass)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (vertices == null)
		{
			throw new ArgumentNullException("vertices", "Vertices must not be null");
		}
		if (mass <= 0f)
		{
			throw new ArgumentOutOfRangeException("mass", "Mass must be more than 0");
		}
		Body body = new Body();
		body.Mass = mass;
		body.MomentOfInertia = mass * vertices.GetMomentOfInertia();
		body.position = vertices.GetCentroid();
		return body;
	}

	public Body CreateBody(PhysicsSimulator physicsSimulator, float mass, float momentOfInertia)
	{
		Body body = CreateBody(mass, momentOfInertia);
		physicsSimulator.Add(body);
		return body;
	}

	public Body CreateBody(float mass, float momentOfInertia)
	{
		if (mass <= 0f)
		{
			throw new ArgumentOutOfRangeException("mass", "Mass must be more than 0");
		}
		if (momentOfInertia <= 0f)
		{
			throw new ArgumentOutOfRangeException("momentOfInertia", "MOI must be more than 0");
		}
		Body body = new Body();
		body.Mass = mass;
		body.MomentOfInertia = momentOfInertia;
		return body;
	}

	public Body CreateBody(PhysicsSimulator physicsSimulator, Body body)
	{
		Body body2 = CreateBody(body);
		physicsSimulator.Add(body2);
		return body2;
	}

	public Body CreateBody(Body body)
	{
		return new Body(body);
	}
}
