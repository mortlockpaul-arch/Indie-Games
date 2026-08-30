using System;
using System.Collections.Generic;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace Physics;

public class PhysicalRepresentation
{
	private const float CollisionGridSize = 0f;

	private PhysicsSimulator m_simulation;

	private Body m_body;

	private Geom m_geom;

	private bool m_bEnabled;

	private bool m_bProjectileEnabled;

	public float FrictionCoeff
	{
		get
		{
			return m_geom.FrictionCoefficient;
		}
		set
		{
			m_geom.FrictionCoefficient = value;
		}
	}

	public float Mass
	{
		get
		{
			return m_body.Mass;
		}
		set
		{
			m_body.Mass = value;
		}
	}

	public float Bounciness
	{
		get
		{
			return m_geom.RestitutionCoefficient;
		}
		set
		{
			m_geom.RestitutionCoefficient = value;
		}
	}

	public float AirDrag
	{
		get
		{
			return m_body.LinearDragCoefficient;
		}
		set
		{
			m_body.LinearDragCoefficient = value;
		}
	}

	public float RotationalDrag
	{
		get
		{
			return m_body.RotationalDragCoefficient;
		}
		set
		{
			m_body.RotationalDragCoefficient = value;
		}
	}

	public bool Sensor
	{
		get
		{
			return m_geom.IsSensor;
		}
		set
		{
			m_geom.IsSensor = value;
		}
	}

	public bool Static
	{
		get
		{
			return m_geom.Body.IsStatic;
		}
		set
		{
			m_geom.Body.IsStatic = value;
		}
	}

	public bool Enabled
	{
		get
		{
			return m_geom.Body.Enabled;
		}
		set
		{
			if (m_geom.Body.Enabled == value)
			{
				if (value != m_bEnabled)
				{
					if (value)
					{
						m_simulation.Add(m_body);
						m_simulation.Add(m_geom);
					}
					else
					{
						m_simulation.Remove(m_body);
						m_simulation.Remove(m_geom);
					}
				}
				m_bEnabled = value;
			}
			m_geom.Body.Enabled = value;
		}
	}

	public bool ProjectileEnabled
	{
		get
		{
			return m_bProjectileEnabled;
		}
		set
		{
			if (m_bProjectileEnabled != value)
			{
				if (value)
				{
					PhysicsObjectManager.AddObject(GetGeom());
				}
				else
				{
					PhysicsObjectManager.RemoveObject(GetGeom());
				}
			}
			m_bProjectileEnabled = value;
		}
	}

	public CollisionCategory CollisionCategory
	{
		get
		{
			return m_geom.CollisionCategories;
		}
		set
		{
			m_geom.CollisionCategories = value;
		}
	}

	public CollisionCategory CollidesWith
	{
		get
		{
			return m_geom.CollidesWith;
		}
		set
		{
			m_geom.CollidesWith = value;
		}
	}

	public bool CollisionsEnabled
	{
		get
		{
			return m_geom.CollisionEnabled;
		}
		set
		{
			m_geom.CollisionEnabled = value;
		}
	}

	public Vector2 Position
	{
		get
		{
			return m_body.Position;
		}
		set
		{
			m_body.Position = value;
		}
	}

	public float Rotation
	{
		get
		{
			return m_body.Rotation;
		}
		set
		{
			m_body.Rotation = value;
		}
	}

	public Vector2 Velocity
	{
		get
		{
			return m_body.LinearVelocity;
		}
		set
		{
			m_body.LinearVelocity = value;
		}
	}

	public PhysicalRepresentation(PhysicsSimulator simulation, Rectangle r, CollisionCategory cat)
	{
		InitRect(simulation, r);
		Initialize(cat);
	}

	public PhysicalRepresentation(PhysicsSimulator simulation, Vector2 pos, Vector2 size, CollisionCategory cat)
	{
		InitRect(simulation, pos, size);
		Initialize(cat);
	}

	public PhysicalRepresentation(PhysicsSimulator simulation, int radius, Vector2 pos, CollisionCategory cat)
	{
		InitCircle(simulation, radius, pos);
		Initialize(cat);
	}

	public PhysicalRepresentation(PhysicsSimulator simulation, List<Vector2> vertices, Vector2 pos, CollisionCategory cat)
	{
		InitPoly(simulation, vertices, pos);
		Initialize(cat);
	}

	public PhysicalRepresentation(PhysicsSimulator simulation, Vertices vertices, Vector2 pos, CollisionCategory cat, bool subDiv)
	{
		InitPoly(simulation, vertices, pos, default(Vector2), subDiv);
		Initialize(cat);
	}

	public PhysicalRepresentation(PhysicsSimulator simulation, Vertices vertBody, Vertices vertGeom, Vector2 pos, CollisionCategory cat)
	{
		InitPoly(simulation, vertBody, vertGeom, pos, default(Vector2));
		Initialize(cat);
	}

	public PhysicalRepresentation(PhysicsSimulator simulation, PhysicalRepresentation clone)
	{
		m_simulation = simulation;
		m_body = BodyFactory.Instance.CreateBody(m_simulation, clone.m_body);
		m_geom = GeomFactory.Instance.CreateGeom(m_simulation, m_body, clone.m_geom);
	}

	private void InitRect(PhysicsSimulator simulation, Rectangle r)
	{
		m_simulation = simulation;
		m_body = BodyFactory.Instance.CreateRectangleBody(simulation, r.Width, r.Height, 1f);
		m_body.Position = new Vector2((float)r.Left + (float)r.Width / 2f, (float)r.Top + (float)r.Height / 2f);
		if (PhysicsSimulator.NarrowPhaseCollider == NarrowPhaseCollider.SAT)
		{
			m_geom = GeomFactory.Instance.CreatePolygonGeom(m_simulation, m_body, Vertices.CreateSimpleRectangle(r.Width, r.Height), (float)Math.Min(r.Width, r.Height) * 0f);
		}
		else
		{
			m_geom = GeomFactory.Instance.CreateRectangleGeom(m_simulation, m_body, r.Width, r.Height, default(Vector2), 0f, (float)Math.Min(r.Width, r.Height) * 0f);
		}
	}

	private void InitRect(PhysicsSimulator simulation, Vector2 pos, Vector2 size)
	{
		m_simulation = simulation;
		m_body = BodyFactory.Instance.CreateRectangleBody(simulation, size.X, size.Y, 1f);
		m_body.Position = pos;
		if (PhysicsSimulator.NarrowPhaseCollider == NarrowPhaseCollider.SAT)
		{
			m_geom = GeomFactory.Instance.CreatePolygonGeom(m_simulation, m_body, Vertices.CreateSimpleRectangle(size.X, size.Y), Math.Min(size.X, size.Y) * 0f);
		}
		else
		{
			m_geom = GeomFactory.Instance.CreateRectangleGeom(m_simulation, m_body, size.X, size.Y, default(Vector2), 0f, Math.Min(size.X, size.Y) * 0f);
		}
	}

	private void InitCircle(PhysicsSimulator simulation, int radius, Vector2 pos)
	{
		m_simulation = simulation;
		m_body = BodyFactory.Instance.CreateCircleBody(simulation, radius, 1f);
		m_body.Position = pos;
		float collisionGridSize = 8f;
		m_geom = GeomFactory.Instance.CreateCircleGeom(m_simulation, m_body, radius, 30, collisionGridSize);
	}

	private void InitPoly(PhysicsSimulator simulation, List<Vector2> vertices, Vector2 pos)
	{
		Vertices vertices2 = new Vertices(vertices);
		if (PhysicsSimulator.NarrowPhaseCollider != NarrowPhaseCollider.SAT)
		{
			vertices2.SubDivideEdges(20f);
		}
		Vector2 centroid = vertices2.GetCentroid();
		InitPoly(simulation, vertices2, pos, centroid, subDiv: false);
	}

	private void InitPoly(PhysicsSimulator simulation, Vertices vertices, Vector2 pos, Vector2 offset, bool subDiv)
	{
		m_simulation = simulation;
		m_body = BodyFactory.Instance.CreatePolygonBody(simulation, vertices, 1f);
		m_body.Position = pos;
		if (subDiv && PhysicsSimulator.NarrowPhaseCollider != NarrowPhaseCollider.SAT)
		{
			vertices.SubDivideEdges(20f);
		}
		float collisionGridSize = 8f;
		m_geom = GeomFactory.Instance.CreatePolygonGeom(m_simulation, m_body, vertices, offset, 0f, collisionGridSize);
	}

	private void InitPoly(PhysicsSimulator simulation, Vertices vertBody, Vertices vertGeom, Vector2 pos, Vector2 offset)
	{
		m_simulation = simulation;
		m_body = BodyFactory.Instance.CreatePolygonBody(simulation, vertBody, 1f);
		m_body.Position = pos;
		float collisionGridSize = 8f;
		m_geom = GeomFactory.Instance.CreatePolygonGeom(m_simulation, m_body, vertGeom, offset, 0f, collisionGridSize);
	}

	private void Initialize(CollisionCategory cat)
	{
		m_bEnabled = true;
		m_geom.CollisionCategories = cat;
		m_geom.CollidesWith = CollisionCategory.All;
		m_geom.FrictionCoefficient = 0.2f;
		m_body.LinearDragCoefficient = 5f;
		m_body.RotationalDragCoefficient = 5f;
		PhysicsObjectManager.AddObject(GetGeom());
		m_bProjectileEnabled = true;
	}

	public void RigidAttach(PhysicalRepresentation r2)
	{
		m_simulation.Add(new WeldJoint(m_body, r2.m_body, Position));
	}

	public RevoluteJoint RevoluteAttach(PhysicalRepresentation r2, Vector2 pos)
	{
		return JointFactory.Instance.CreateRevoluteJoint(m_simulation, m_body, r2.m_body, pos);
	}

	public AngleJoint AngleAttach(PhysicalRepresentation r2)
	{
		return JointFactory.Instance.CreateAngleJoint(m_simulation, m_body, r2.m_body);
	}

	public Geom GetGeom()
	{
		return m_geom;
	}

	public void SetCollisionHandler(CollisionEventHandler target)
	{
		m_geom.OnCollision = target;
	}

	public void SetStatic(bool b)
	{
		m_body.IsStatic = b;
	}

	public void ApplyImpulse(Vector2 amount)
	{
		m_body.ApplyImpulse(amount * 0.85f);
	}

	public void Rotate(float f)
	{
		m_body.AngularVelocity = f;
	}

	public void CleanObject()
	{
		if (ProjectileEnabled)
		{
			ProjectileEnabled = false;
		}
		PhysicsObjectManager.RemoveObject(this);
		m_geom.Dispose();
		m_body.Dispose();
	}

	public void ResetSimulation()
	{
		m_simulation = PhysicsObjectManager.GetSimulation();
	}
}
