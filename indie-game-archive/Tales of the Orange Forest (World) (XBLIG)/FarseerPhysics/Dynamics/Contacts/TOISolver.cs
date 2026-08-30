using System;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Contacts;

internal class TOISolver
{
	private TOIConstraint[] _constraints = new TOIConstraint[8];

	private int _count;

	private Body _toiBody;

	public void Initialize(Contact[] contacts, int count, Body toiBody)
	{
		_count = count;
		_toiBody = toiBody;
		if (_constraints.Length < _count)
		{
			_constraints = new TOIConstraint[Math.Max(_constraints.Length * 2, _count)];
		}
		for (int i = 0; i < _count; i++)
		{
			Contact contact = contacts[i];
			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;
			Shape shape = fixtureA.Shape;
			Shape shape2 = fixtureB.Shape;
			float radius = shape.Radius;
			float radius2 = shape2.Radius;
			Body body = fixtureA.Body;
			Body body2 = fixtureB.Body;
			contact.GetManifold(out var manifold);
			TOIConstraint tOIConstraint = _constraints[i];
			tOIConstraint.BodyA = body;
			tOIConstraint.BodyB = body2;
			tOIConstraint.LocalNormal = manifold.LocalNormal;
			tOIConstraint.LocalPoint = manifold.LocalPoint;
			tOIConstraint.Type = manifold.Type;
			tOIConstraint.PointCount = manifold.PointCount;
			tOIConstraint.Radius = radius + radius2;
			for (int j = 0; j < tOIConstraint.PointCount; j++)
			{
				tOIConstraint.LocalPoints[j] = manifold.Points[j].LocalPoint;
			}
			_constraints[i] = tOIConstraint;
		}
	}

	public bool Solve(float baumgarte)
	{
		float num = 0f;
		for (int i = 0; i < _count; i++)
		{
			TOIConstraint cc = _constraints[i];
			Body bodyA = cc.BodyA;
			Body bodyB = cc.BodyB;
			float num2 = bodyA.Mass;
			float num3 = bodyB.Mass;
			if (bodyA == _toiBody)
			{
				num3 = 0f;
			}
			else
			{
				num2 = 0f;
			}
			float num4 = num2 * bodyA.InvMass;
			float num5 = num2 * bodyA.InvI;
			float num6 = num3 * bodyB.InvMass;
			float num7 = num3 * bodyB.InvI;
			for (int j = 0; j < cc.PointCount; j++)
			{
				TOISolverManifold tOISolverManifold = new TOISolverManifold(ref cc, j);
				Vector2 normal = tOISolverManifold.Normal;
				Vector2 point = tOISolverManifold.Point;
				float separation = tOISolverManifold.Separation;
				Vector2 a = point - bodyA.Sweep.c;
				Vector2 a2 = point - bodyB.Sweep.c;
				num = Math.Min(num, separation);
				float num8 = MathUtils.Clamp(baumgarte * (separation + 0.005f), -0.2f, 0f);
				float num9 = MathUtils.Cross(a, normal);
				float num10 = MathUtils.Cross(a2, normal);
				float num11 = num4 + num6 + num5 * num9 * num9 + num7 * num10 * num10;
				float num12 = ((num11 > 0f) ? ((0f - num8) / num11) : 0f);
				Vector2 vector = num12 * normal;
				bodyA.Sweep.c -= num4 * vector;
				bodyA.Sweep.a -= num5 * MathUtils.Cross(a, vector);
				bodyA.SynchronizeTransform();
				bodyB.Sweep.c += num6 * vector;
				bodyB.Sweep.a += num7 * MathUtils.Cross(a2, vector);
				bodyB.SynchronizeTransform();
			}
		}
		return num >= -0.0075f;
	}
}
