using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Controllers;

public class GravityController : Controller
{
	public List<Body> Bodies = new List<Body>();

	public List<Vector2> Points = new List<Vector2>();

	public float MinRadius { get; set; }

	public float MaxRadius { get; set; }

	public float Strength { get; set; }

	public GravityType GravityType { get; set; }

	public GravityController(float strength)
	{
		Strength = strength;
		MaxRadius = float.MaxValue;
	}

	public GravityController(float strength, float maxRadius, float minRadius)
	{
		MinRadius = minRadius;
		MaxRadius = maxRadius;
		Strength = strength;
	}

	public override void Update(float dt)
	{
		Vector2 force = Vector2.Zero;
		foreach (Body body in base.World.BodyList)
		{
			if (!body.Active || body.IgnoreGravity || body.IsStatic)
			{
				continue;
			}
			foreach (Body body2 in Bodies)
			{
				if (body == body2 || (body.IsStatic && body2.IsStatic) || !body2.Active)
				{
					continue;
				}
				Vector2 vector = body2.Position - body.Position;
				float num = vector.LengthSquared();
				if (num < 1.1920929E-07f)
				{
					continue;
				}
				float num2 = vector.Length();
				if (!(num2 >= MaxRadius) && !(num2 <= MinRadius))
				{
					switch (GravityType)
					{
					case GravityType.DistanceSquared:
						force = Strength / num / (float)Math.Sqrt(num) * body.Mass * body2.Mass * vector;
						break;
					case GravityType.Linear:
						force = Strength / num * body.Mass * body2.Mass * vector;
						break;
					}
					body.ApplyForce(ref force);
					Vector2.Negate(ref force, out force);
					body2.ApplyForce(ref force);
				}
			}
			foreach (Vector2 point in Points)
			{
				Vector2 vector2 = point - body.Position;
				float num3 = vector2.LengthSquared();
				if (num3 < 1.1920929E-07f)
				{
					continue;
				}
				float num4 = vector2.Length();
				if (!(num4 >= MaxRadius) && !(num4 <= MinRadius))
				{
					switch (GravityType)
					{
					case GravityType.DistanceSquared:
						force = Strength / num3 / (float)Math.Sqrt(num3) * body.Mass * vector2;
						break;
					case GravityType.Linear:
						force = Strength / num3 * body.Mass * vector2;
						break;
					}
					body.ApplyForce(ref force);
				}
			}
		}
	}

	public void AddBody(Body body)
	{
		Bodies.Add(body);
	}

	public void AddPoint(Vector2 point)
	{
		Points.Add(point);
	}
}
