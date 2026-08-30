using System;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Dynamics.Contacts;

public class ContactSolver
{
	public ContactConstraint[] Constraints;

	private int _constraintCount;

	private Contact[] _contacts;

	public void Reset(Contact[] contacts, int contactCount, float impulseRatio)
	{
		_contacts = contacts;
		_constraintCount = contactCount;
		if (Constraints == null || Constraints.Length < _constraintCount)
		{
			Constraints = new ContactConstraint[_constraintCount * 2];
			for (int i = 0; i < _constraintCount * 2; i++)
			{
				Constraints[i] = new ContactConstraint();
			}
		}
		for (int j = 0; j < _constraintCount; j++)
		{
			Contact contact = contacts[j];
			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;
			Shape shape = fixtureA.Shape;
			Shape shape2 = fixtureB.Shape;
			float radius = shape.Radius;
			float radius2 = shape2.Radius;
			Body body = fixtureA.Body;
			Body body2 = fixtureB.Body;
			contact.GetManifold(out var manifold);
			float friction = Settings.MixFriction(fixtureA.Friction, fixtureB.Friction);
			float num = Settings.MixRestitution(fixtureA.Restitution, fixtureB.Restitution);
			Vector2 linearVelocityInternal = body.LinearVelocityInternal;
			Vector2 linearVelocityInternal2 = body2.LinearVelocityInternal;
			float angularVelocityInternal = body.AngularVelocityInternal;
			float angularVelocityInternal2 = body2.AngularVelocityInternal;
			WorldManifold worldManifold = new WorldManifold(ref manifold, ref body.Xf, radius, ref body2.Xf, radius2);
			ContactConstraint contactConstraint = Constraints[j];
			contactConstraint.BodyA = body;
			contactConstraint.BodyB = body2;
			contactConstraint.Manifold = manifold;
			contactConstraint.Normal = worldManifold.Normal;
			contactConstraint.PointCount = manifold.PointCount;
			contactConstraint.Friction = friction;
			contactConstraint.LocalNormal = manifold.LocalNormal;
			contactConstraint.LocalPoint = manifold.LocalPoint;
			contactConstraint.Radius = radius + radius2;
			contactConstraint.Type = manifold.Type;
			for (int k = 0; k < contactConstraint.PointCount; k++)
			{
				ManifoldPoint manifoldPoint = manifold.Points[k];
				ContactConstraintPoint contactConstraintPoint = contactConstraint.Points[k];
				contactConstraintPoint.NormalImpulse = impulseRatio * manifoldPoint.NormalImpulse;
				contactConstraintPoint.TangentImpulse = impulseRatio * manifoldPoint.TangentImpulse;
				contactConstraintPoint.LocalPoint = manifoldPoint.LocalPoint;
				contactConstraintPoint.rA = worldManifold.Points[k] - body.Sweep.c;
				contactConstraintPoint.rB = worldManifold.Points[k] - body2.Sweep.c;
				float num2 = contactConstraintPoint.rA.X * contactConstraint.Normal.Y - contactConstraintPoint.rA.Y * contactConstraint.Normal.X;
				float num3 = contactConstraintPoint.rB.X * contactConstraint.Normal.Y - contactConstraintPoint.rB.Y * contactConstraint.Normal.X;
				num2 *= num2;
				num3 *= num3;
				float num4 = body.InvMass + body2.InvMass + body.InvI * num2 + body2.InvI * num3;
				contactConstraintPoint.NormalMass = 1f / num4;
				Vector2 vector = new Vector2(contactConstraint.Normal.Y, 0f - contactConstraint.Normal.X);
				float num5 = contactConstraintPoint.rA.X * vector.Y - contactConstraintPoint.rA.Y * vector.X;
				float num6 = contactConstraintPoint.rB.X * vector.Y - contactConstraintPoint.rB.Y * vector.X;
				num5 *= num5;
				num6 *= num6;
				float num7 = body.InvMass + body2.InvMass + body.InvI * num5 + body2.InvI * num6;
				contactConstraintPoint.TangentMass = 1f / num7;
				contactConstraintPoint.VelocityBias = 0f;
				float num8 = Vector2.Dot(contactConstraint.Normal, linearVelocityInternal2 + MathUtils.Cross(angularVelocityInternal2, contactConstraintPoint.rB) - linearVelocityInternal - MathUtils.Cross(angularVelocityInternal, contactConstraintPoint.rA));
				if (num8 < -1f)
				{
					contactConstraintPoint.VelocityBias = (0f - num) * num8;
				}
			}
			if (contactConstraint.PointCount == 2)
			{
				ContactConstraintPoint contactConstraintPoint2 = contactConstraint.Points[0];
				ContactConstraintPoint contactConstraintPoint3 = contactConstraint.Points[1];
				float invMass = body.InvMass;
				float invI = body.InvI;
				float invMass2 = body2.InvMass;
				float invI2 = body2.InvI;
				float num9 = MathUtils.Cross(contactConstraintPoint2.rA, contactConstraint.Normal);
				float num10 = MathUtils.Cross(contactConstraintPoint2.rB, contactConstraint.Normal);
				float num11 = MathUtils.Cross(contactConstraintPoint3.rA, contactConstraint.Normal);
				float num12 = MathUtils.Cross(contactConstraintPoint3.rB, contactConstraint.Normal);
				float num13 = invMass + invMass2 + invI * num9 * num9 + invI2 * num10 * num10;
				float num14 = invMass + invMass2 + invI * num11 * num11 + invI2 * num12 * num12;
				float num15 = invMass + invMass2 + invI * num9 * num11 + invI2 * num10 * num12;
				if (num13 * num13 < 100f * (num13 * num14 - num15 * num15))
				{
					contactConstraint.K = new Mat22(new Vector2(num13, num15), new Vector2(num15, num14));
					contactConstraint.NormalMass = contactConstraint.K.Inverse;
				}
				else
				{
					contactConstraint.PointCount = 1;
				}
			}
			if (fixtureA.PostSolve != null)
			{
				fixtureA.PostSolve(contactConstraint);
			}
			if (fixtureB.PostSolve != null)
			{
				fixtureB.PostSolve(contactConstraint);
			}
		}
	}

	public void WarmStart()
	{
		for (int i = 0; i < _constraintCount; i++)
		{
			ContactConstraint contactConstraint = Constraints[i];
			Body bodyA = contactConstraint.BodyA;
			Body bodyB = contactConstraint.BodyB;
			float invMass = bodyA.InvMass;
			float invI = bodyA.InvI;
			float invMass2 = bodyB.InvMass;
			float invI2 = bodyB.InvI;
			Vector2 normal = contactConstraint.Normal;
			Vector2 vector = new Vector2(normal.Y, 0f - normal.X);
			for (int j = 0; j < contactConstraint.PointCount; j++)
			{
				ContactConstraintPoint contactConstraintPoint = contactConstraint.Points[j];
				Vector2 vector2 = new Vector2(contactConstraintPoint.NormalImpulse * normal.X + contactConstraintPoint.TangentImpulse * vector.X, contactConstraintPoint.NormalImpulse * normal.Y + contactConstraintPoint.TangentImpulse * vector.Y);
				bodyA.AngularVelocityInternal -= invI * (contactConstraintPoint.rA.X * vector2.Y - contactConstraintPoint.rA.Y * vector2.X);
				bodyA.LinearVelocityInternal.X -= invMass * vector2.X;
				bodyA.LinearVelocityInternal.Y -= invMass * vector2.Y;
				bodyB.AngularVelocityInternal += invI2 * (contactConstraintPoint.rB.X * vector2.Y - contactConstraintPoint.rB.Y * vector2.X);
				bodyB.LinearVelocityInternal.X += invMass2 * vector2.X;
				bodyB.LinearVelocityInternal.Y += invMass2 * vector2.Y;
			}
		}
	}

	public void SolveVelocityConstraints()
	{
		for (int i = 0; i < _constraintCount; i++)
		{
			ContactConstraint contactConstraint = Constraints[i];
			Body bodyA = contactConstraint.BodyA;
			Body bodyB = contactConstraint.BodyB;
			float num = bodyA.AngularVelocityInternal;
			float num2 = bodyB.AngularVelocityInternal;
			Vector2 linearVelocityInternal = bodyA.LinearVelocityInternal;
			Vector2 linearVelocityInternal2 = bodyB.LinearVelocityInternal;
			float invMass = bodyA.InvMass;
			float invI = bodyA.InvI;
			float invMass2 = bodyB.InvMass;
			float invI2 = bodyB.InvI;
			Vector2 normal = contactConstraint.Normal;
			Vector2 vector = new Vector2(normal.Y, 0f - normal.X);
			float friction = contactConstraint.Friction;
			for (int j = 0; j < contactConstraint.PointCount; j++)
			{
				ContactConstraintPoint contactConstraintPoint = contactConstraint.Points[j];
				Vector2 vector2 = new Vector2(linearVelocityInternal2.X + (0f - num2) * contactConstraintPoint.rB.Y - linearVelocityInternal.X - (0f - num) * contactConstraintPoint.rA.Y, linearVelocityInternal2.Y + num2 * contactConstraintPoint.rB.X - linearVelocityInternal.Y - num * contactConstraintPoint.rA.X);
				float num3 = vector2.X * vector.X + vector2.Y * vector.Y;
				float num4 = contactConstraintPoint.TangentMass * (0f - num3);
				float num5 = friction * contactConstraintPoint.NormalImpulse;
				float num6 = MathUtils.Clamp(contactConstraintPoint.TangentImpulse + num4, 0f - num5, num5);
				num4 = num6 - contactConstraintPoint.TangentImpulse;
				Vector2 vector3 = new Vector2(num4 * vector.X, num4 * vector.Y);
				linearVelocityInternal.X -= invMass * vector3.X;
				linearVelocityInternal.Y -= invMass * vector3.Y;
				num -= invI * (contactConstraintPoint.rA.X * vector3.Y - contactConstraintPoint.rA.Y * vector3.X);
				linearVelocityInternal2.X += invMass2 * vector3.X;
				linearVelocityInternal2.Y += invMass2 * vector3.Y;
				num2 += invI2 * (contactConstraintPoint.rB.X * vector3.Y - contactConstraintPoint.rB.Y * vector3.X);
				contactConstraintPoint.TangentImpulse = num6;
			}
			if (contactConstraint.PointCount == 1)
			{
				ContactConstraintPoint contactConstraintPoint2 = contactConstraint.Points[0];
				Vector2 vector4 = new Vector2(linearVelocityInternal2.X + (0f - num2) * contactConstraintPoint2.rB.Y - linearVelocityInternal.X - (0f - num) * contactConstraintPoint2.rA.Y, linearVelocityInternal2.Y + num2 * contactConstraintPoint2.rB.X - linearVelocityInternal.Y - num * contactConstraintPoint2.rA.X);
				float num7 = vector4.X * normal.X + vector4.Y * normal.Y;
				float num8 = (0f - contactConstraintPoint2.NormalMass) * (num7 - contactConstraintPoint2.VelocityBias);
				float num9 = Math.Max(contactConstraintPoint2.NormalImpulse + num8, 0f);
				num8 = num9 - contactConstraintPoint2.NormalImpulse;
				Vector2 vector5 = new Vector2(num8 * normal.X, num8 * normal.Y);
				linearVelocityInternal.X -= invMass * vector5.X;
				linearVelocityInternal.Y -= invMass * vector5.Y;
				num -= invI * (contactConstraintPoint2.rA.X * vector5.Y - contactConstraintPoint2.rA.Y * vector5.X);
				linearVelocityInternal2.X += invMass2 * vector5.X;
				linearVelocityInternal2.Y += invMass2 * vector5.Y;
				num2 += invI2 * (contactConstraintPoint2.rB.X * vector5.Y - contactConstraintPoint2.rB.Y * vector5.X);
				contactConstraintPoint2.NormalImpulse = num9;
			}
			else
			{
				ContactConstraintPoint contactConstraintPoint3 = contactConstraint.Points[0];
				ContactConstraintPoint contactConstraintPoint4 = contactConstraint.Points[1];
				Vector2 v = new Vector2(contactConstraintPoint3.NormalImpulse, contactConstraintPoint4.NormalImpulse);
				Vector2 vector6 = new Vector2(linearVelocityInternal2.X + (0f - num2) * contactConstraintPoint3.rB.Y - linearVelocityInternal.X - (0f - num) * contactConstraintPoint3.rA.Y, linearVelocityInternal2.Y + num2 * contactConstraintPoint3.rB.X - linearVelocityInternal.Y - num * contactConstraintPoint3.rA.X);
				Vector2 vector7 = new Vector2(linearVelocityInternal2.X + (0f - num2) * contactConstraintPoint4.rB.Y - linearVelocityInternal.X - (0f - num) * contactConstraintPoint4.rA.Y, linearVelocityInternal2.Y + num2 * contactConstraintPoint4.rB.X - linearVelocityInternal.Y - num * contactConstraintPoint4.rA.X);
				float num10 = vector6.X * normal.X + vector6.Y * normal.Y;
				float num11 = vector7.X * normal.X + vector7.Y * normal.Y;
				Vector2 v2 = new Vector2(num10 - contactConstraintPoint3.VelocityBias, num11 - contactConstraintPoint4.VelocityBias);
				v2 -= MathUtils.Multiply(ref contactConstraint.K, ref v);
				Vector2 vector8 = -MathUtils.Multiply(ref contactConstraint.NormalMass, ref v2);
				if (vector8.X >= 0f && vector8.Y >= 0f)
				{
					Vector2 vector9 = new Vector2(vector8.X - v.X, vector8.Y - v.Y);
					Vector2 vector10 = new Vector2(vector9.X * normal.X, vector9.X * normal.Y);
					Vector2 vector11 = new Vector2(vector9.Y * normal.X, vector9.Y * normal.Y);
					Vector2 vector12 = new Vector2(vector10.X + vector11.X, vector10.Y + vector11.Y);
					linearVelocityInternal.X -= invMass * vector12.X;
					linearVelocityInternal.Y -= invMass * vector12.Y;
					num -= invI * (contactConstraintPoint3.rA.X * vector10.Y - contactConstraintPoint3.rA.Y * vector10.X + (contactConstraintPoint4.rA.X * vector11.Y - contactConstraintPoint4.rA.Y * vector11.X));
					linearVelocityInternal2.X += invMass2 * vector12.X;
					linearVelocityInternal2.Y += invMass2 * vector12.Y;
					num2 += invI2 * (contactConstraintPoint3.rB.X * vector10.Y - contactConstraintPoint3.rB.Y * vector10.X + (contactConstraintPoint4.rB.X * vector11.Y - contactConstraintPoint4.rB.Y * vector11.X));
					contactConstraintPoint3.NormalImpulse = vector8.X;
					contactConstraintPoint4.NormalImpulse = vector8.Y;
				}
				else
				{
					vector8.X = (0f - contactConstraintPoint3.NormalMass) * v2.X;
					vector8.Y = 0f;
					num10 = 0f;
					num11 = contactConstraint.K.col1.Y * vector8.X + v2.Y;
					if (vector8.X >= 0f && num11 >= 0f)
					{
						Vector2 vector13 = new Vector2(vector8.X - v.X, vector8.Y - v.Y);
						Vector2 vector14 = new Vector2(vector13.X * normal.X, vector13.X * normal.Y);
						Vector2 vector15 = new Vector2(vector13.Y * normal.X, vector13.Y * normal.Y);
						Vector2 vector16 = new Vector2(vector14.X + vector15.X, vector14.Y + vector15.Y);
						linearVelocityInternal.X -= invMass * vector16.X;
						linearVelocityInternal.Y -= invMass * vector16.Y;
						num -= invI * (contactConstraintPoint3.rA.X * vector14.Y - contactConstraintPoint3.rA.Y * vector14.X + (contactConstraintPoint4.rA.X * vector15.Y - contactConstraintPoint4.rA.Y * vector15.X));
						linearVelocityInternal2.X += invMass2 * vector16.X;
						linearVelocityInternal2.Y += invMass2 * vector16.Y;
						num2 += invI2 * (contactConstraintPoint3.rB.X * vector14.Y - contactConstraintPoint3.rB.Y * vector14.X + (contactConstraintPoint4.rB.X * vector15.Y - contactConstraintPoint4.rB.Y * vector15.X));
						contactConstraintPoint3.NormalImpulse = vector8.X;
						contactConstraintPoint4.NormalImpulse = vector8.Y;
					}
					else
					{
						vector8.X = 0f;
						vector8.Y = (0f - contactConstraintPoint4.NormalMass) * v2.Y;
						num10 = contactConstraint.K.col2.X * vector8.Y + v2.X;
						num11 = 0f;
						if (vector8.Y >= 0f && num10 >= 0f)
						{
							Vector2 vector17 = new Vector2(vector8.X - v.X, vector8.Y - v.Y);
							Vector2 vector18 = new Vector2(vector17.X * normal.X, vector17.X * normal.Y);
							Vector2 vector19 = new Vector2(vector17.Y * normal.X, vector17.Y * normal.Y);
							Vector2 vector20 = new Vector2(vector18.X + vector19.X, vector18.Y + vector19.Y);
							linearVelocityInternal.X -= invMass * vector20.X;
							linearVelocityInternal.Y -= invMass * vector20.Y;
							num -= invI * (contactConstraintPoint3.rA.X * vector18.Y - contactConstraintPoint3.rA.Y * vector18.X + (contactConstraintPoint4.rA.X * vector19.Y - contactConstraintPoint4.rA.Y * vector19.X));
							linearVelocityInternal2.X += invMass2 * vector20.X;
							linearVelocityInternal2.Y += invMass2 * vector20.Y;
							num2 += invI2 * (contactConstraintPoint3.rB.X * vector18.Y - contactConstraintPoint3.rB.Y * vector18.X + (contactConstraintPoint4.rB.X * vector19.Y - contactConstraintPoint4.rB.Y * vector19.X));
							contactConstraintPoint3.NormalImpulse = vector8.X;
							contactConstraintPoint4.NormalImpulse = vector8.Y;
						}
						else
						{
							vector8.X = 0f;
							vector8.Y = 0f;
							num10 = v2.X;
							num11 = v2.Y;
							if (num10 >= 0f && num11 >= 0f)
							{
								Vector2 vector21 = new Vector2(vector8.X - v.X, vector8.Y - v.Y);
								Vector2 vector22 = new Vector2(vector21.X * normal.X, vector21.X * normal.Y);
								Vector2 vector23 = new Vector2(vector21.Y * normal.X, vector21.Y * normal.Y);
								Vector2 vector24 = new Vector2(vector22.X + vector23.X, vector22.Y + vector23.Y);
								linearVelocityInternal.X -= invMass * vector24.X;
								linearVelocityInternal.Y -= invMass * vector24.Y;
								num -= invI * (contactConstraintPoint3.rA.X * vector22.Y - contactConstraintPoint3.rA.Y * vector22.X + (contactConstraintPoint4.rA.X * vector23.Y - contactConstraintPoint4.rA.Y * vector23.X));
								linearVelocityInternal2.X += invMass2 * vector24.X;
								linearVelocityInternal2.Y += invMass2 * vector24.Y;
								num2 += invI2 * (contactConstraintPoint3.rB.X * vector22.Y - contactConstraintPoint3.rB.Y * vector22.X + (contactConstraintPoint4.rB.X * vector23.Y - contactConstraintPoint4.rB.Y * vector23.X));
								contactConstraintPoint3.NormalImpulse = vector8.X;
								contactConstraintPoint4.NormalImpulse = vector8.Y;
							}
						}
					}
				}
			}
			bodyA.LinearVelocityInternal = linearVelocityInternal;
			bodyA.AngularVelocityInternal = num;
			bodyB.LinearVelocityInternal = linearVelocityInternal2;
			bodyB.AngularVelocityInternal = num2;
		}
	}

	public void StoreImpulses()
	{
		for (int i = 0; i < _constraintCount; i++)
		{
			ContactConstraint contactConstraint = Constraints[i];
			Manifold manifold = contactConstraint.Manifold;
			for (int j = 0; j < contactConstraint.PointCount; j++)
			{
				ManifoldPoint value = manifold.Points[j];
				ContactConstraintPoint contactConstraintPoint = contactConstraint.Points[j];
				value.NormalImpulse = contactConstraintPoint.NormalImpulse;
				value.TangentImpulse = contactConstraintPoint.TangentImpulse;
				manifold.Points[j] = value;
			}
			contactConstraint.Manifold = manifold;
			_contacts[i].Manifold = manifold;
		}
	}

	public bool SolvePositionConstraints(float baumgarte)
	{
		float num = 0f;
		for (int i = 0; i < _constraintCount; i++)
		{
			ContactConstraint cc = Constraints[i];
			Body bodyA = cc.BodyA;
			Body bodyB = cc.BodyB;
			float num2 = bodyA.Mass * bodyA.InvMass;
			float num3 = bodyA.Mass * bodyA.InvI;
			float num4 = bodyB.Mass * bodyB.InvMass;
			float num5 = bodyB.Mass * bodyB.InvI;
			for (int j = 0; j < cc.PointCount; j++)
			{
				PositionSolverManifold positionSolverManifold = new PositionSolverManifold(ref cc, j);
				Vector2 normal = positionSolverManifold.Normal;
				Vector2 point = positionSolverManifold.Point;
				float separation = positionSolverManifold.Separation;
				Vector2 a = point - bodyA.Sweep.c;
				Vector2 a2 = point - bodyB.Sweep.c;
				num = Math.Min(num, separation);
				float num6 = MathUtils.Clamp(baumgarte * (separation + 0.005f), -0.2f, 0f);
				float num7 = MathUtils.Cross(a, normal);
				float num8 = MathUtils.Cross(a2, normal);
				float num9 = num2 + num4 + num3 * num7 * num7 + num5 * num8 * num8;
				float num10 = ((num9 > 0f) ? ((0f - num6) / num9) : 0f);
				Vector2 vector = new Vector2(num10 * normal.X, num10 * normal.Y);
				bodyA.Sweep.c.X -= num2 * vector.X;
				bodyA.Sweep.c.Y -= num2 * vector.Y;
				bodyA.Sweep.a -= num3 * (a.X * vector.Y - a.Y * vector.X);
				bodyB.Sweep.c.X += num4 * vector.X;
				bodyB.Sweep.c.Y += num4 * vector.Y;
				bodyB.Sweep.a += num5 * (a2.X * vector.Y - a2.Y * vector.X);
				bodyA.SynchronizeTransform();
				bodyB.SynchronizeTransform();
			}
		}
		return num >= -0.0075f;
	}
}
