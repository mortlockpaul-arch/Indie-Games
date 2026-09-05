using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace PhysicsHandler;

public class PhysicalRepresentation
{
	private const float CollisionGridSize = 0f;

	private World m_simulation;

	private Body m_body;

	private List<Shape> m_shapes;

	private List<Fixture> m_fixtures;

	private bool m_bEnabled;

	private Category m_cat;

	public float FrictionCoeff
	{
		get
		{
			return m_fixtures[0].Friction;
		}
		set
		{
			for (int i = 0; i < m_fixtures.Count; i++)
			{
				m_fixtures[i].Friction = value;
			}
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
			return m_fixtures[0].Restitution;
		}
		set
		{
			for (int i = 0; i < m_fixtures.Count; i++)
			{
				m_fixtures[i].Restitution = value;
			}
		}
	}

	public float AirDrag
	{
		get
		{
			return m_body.LinearDamping;
		}
		set
		{
			m_body.LinearDamping = value;
		}
	}

	public float RotationalDrag
	{
		get
		{
			return m_body.AngularDamping;
		}
		set
		{
			m_body.AngularDamping = value;
		}
	}

	public bool Sensor
	{
		get
		{
			return m_fixtures[0].IsSensor;
		}
		set
		{
			for (int i = 0; i < m_fixtures.Count; i++)
			{
				m_fixtures[i].IsSensor = value;
			}
		}
	}

	public bool Static
	{
		get
		{
			return m_body.IsStatic;
		}
		set
		{
			if (value)
			{
				if (!m_body.IsStatic)
				{
					m_body.BodyType = BodyType.Static;
				}
			}
			else if (m_body.IsStatic)
			{
				m_body.BodyType = BodyType.Dynamic;
			}
		}
	}

	public bool Enabled
	{
		get
		{
			return m_body.Enabled;
		}
		set
		{
			m_bEnabled = value;
			if (m_body.Enabled != value)
			{
				m_body.Enabled = value;
				m_body.Awake = value;
			}
		}
	}

	public Category CollisionCategory
	{
		get
		{
			return m_fixtures[0].CollisionFilter.CollisionCategories;
		}
		set
		{
			for (int i = 0; i < m_fixtures.Count; i++)
			{
				m_fixtures[i].CollisionFilter.CollisionCategories = value;
			}
			m_cat = value;
		}
	}

	public Category CollidesWith
	{
		get
		{
			return m_fixtures[0].CollisionFilter.CollidesWith;
		}
		set
		{
			for (int i = 0; i < m_fixtures.Count; i++)
			{
				m_fixtures[i].CollisionFilter.CollidesWith = value;
			}
		}
	}

	public bool CollisionsEnabled
	{
		get
		{
			return m_body.Enabled;
		}
		set
		{
			m_body.Enabled = value;
		}
	}

	public Vector2 Position
	{
		get
		{
			return m_body.Position * 100f;
		}
		set
		{
			m_body.Position = value / 100f;
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
			return m_body.LinearVelocity * 100f;
		}
		set
		{
			m_body.LinearVelocity = value / 100f;
		}
	}

	private void InitLists()
	{
		m_shapes = new List<Shape>();
		m_fixtures = new List<Fixture>();
	}

	public PhysicalRepresentation(World simulation, Rectangle r, Category cat, bool scale)
	{
		InitLists();
		InitRect(simulation, r, scale);
		Initialize(cat);
	}

	public PhysicalRepresentation(World simulation, Vector2 pos, Vector2 size, Category cat, bool scale)
	{
		InitLists();
		InitRect(simulation, pos, size, scale);
		Initialize(cat);
	}

	public PhysicalRepresentation(World simulation, int radius, Vector2 pos, Category cat, bool scale)
	{
		InitLists();
		InitCircle(simulation, radius, pos, scale);
		Initialize(cat);
	}

	public PhysicalRepresentation(World simulation, List<Vector2> vertices, Vector2 pos, Category cat, bool scale)
	{
		InitLists();
		InitPoly(simulation, vertices, pos, scale);
		Initialize(cat);
	}

	public PhysicalRepresentation(World simulation, PhysicalRepresentation clone)
	{
		m_simulation = simulation;
		InitLists();
		m_body = new Body(simulation);
		for (int i = 0; i < clone.m_shapes.Count; i++)
		{
			Shape shape = clone.m_shapes[i].Clone();
			m_shapes.Add(shape);
			m_fixtures.Add(m_body.CreateFixture(shape, 1f));
		}
	}

	private void InitRect(World simulation, Rectangle r, bool scale)
	{
		Vector2 size = new Vector2(r.Width, r.Height);
		Vector2 pos = new Vector2((float)r.Left + (float)r.Width / 2f, (float)r.Top + (float)r.Height / 2f);
		InitRect(simulation, pos, size, scale);
	}

	private void InitRect(World simulation, Vector2 pos, Vector2 size, bool scale)
	{
		if (scale)
		{
			pos /= 100f;
			size /= 100f;
		}
		m_simulation = simulation;
		Vertices vertices = new Vertices();
		vertices.Add(new Vector2((0f - size.X) / 2f, (0f - size.Y) / 2f));
		vertices.Add(new Vector2(size.X / 2f, (0f - size.Y) / 2f));
		vertices.Add(new Vector2(size.X / 2f, size.Y / 2f));
		vertices.Add(new Vector2((0f - size.X) / 2f, size.Y / 2f));
		PolygonShape polygonShape = new PolygonShape(vertices, 1f);
		m_shapes.Add(polygonShape);
		m_body = new Body(simulation);
		m_fixtures.Add(m_body.CreateFixture(polygonShape, 1f));
		m_body.Position = pos;
	}

	private void InitCircle(World simulation, float radius, Vector2 pos, bool scale)
	{
		if (scale)
		{
			pos /= 100f;
			radius /= 100f;
		}
		m_simulation = simulation;
		CircleShape circleShape = new CircleShape(radius, 1f);
		m_shapes.Add(circleShape);
		m_body = new Body(simulation);
		m_fixtures.Add(m_body.CreateFixture(circleShape, 1f));
		m_body.Position = pos;
	}

	private void InitPoly(World simulation, List<Vector2> vertices, Vector2 pos, bool scale)
	{
		List<Vector2> list = new List<Vector2>(vertices);
		float area = GetArea(list);
		Vector2 centroid = GetCentroid(list, area);
		InitPoly(simulation, list, pos, centroid, scale);
	}

	private void InitPoly(World simulation, List<Vector2> vertices, Vector2 pos, Vector2 offset, bool scale)
	{
		if (scale)
		{
			pos /= 100f;
			offset /= 100f;
		}
		for (int i = 0; i < vertices.Count; i++)
		{
			vertices[i] /= 100f;
		}
		m_simulation = simulation;
		Vertices vertices2 = new Vertices(vertices);
		PolygonShape shape = new PolygonShape(vertices2, 1f);
		m_body = new Body(simulation);
		m_fixtures.Add(m_body.CreateFixture(shape, 1f));
		m_body.Position = pos;
	}

	private void Initialize(Category cat)
	{
		m_bEnabled = true;
		m_cat = cat;
		for (int i = 0; i < m_fixtures.Count; i++)
		{
			m_fixtures[i].CollisionFilter.CollisionCategories = cat;
			m_fixtures[i].CollisionFilter.CollidesWith = Category.All;
			m_fixtures[i].Friction = 0.5f;
		}
		Static = false;
		PhysicsObjectManager.AddObject(GetGeom());
	}

	public void RigidAttach(PhysicalRepresentation r2)
	{
		JointFactory.CreateWeldJoint(m_simulation, m_body, r2.m_body, r2.m_body.GetLocalPoint(r2.Position));
	}

	public RevoluteJoint RevoluteAttach(PhysicalRepresentation r2, Vector2 pos)
	{
		pos /= 100f;
		Joint joint = JointFactory.CreateRevoluteJoint(m_simulation, m_body, r2.m_body, r2.m_body.GetLocalPoint(pos));
		return (RevoluteJoint)joint;
	}

	public AngleJoint AngleAttach(PhysicalRepresentation r2)
	{
		return JointFactory.CreateAngleJoint(m_simulation, m_body, r2.m_body);
	}

	public Body GetGeom()
	{
		return m_body;
	}

	public void SetCollisionHandler(OnCollisionEventHandler target)
	{
		for (int i = 0; i < m_fixtures.Count; i++)
		{
			m_fixtures[i].OnCollision = target;
		}
	}

	public Vector2 GetWorldCenter()
	{
		return m_body.WorldCenter * 100f;
	}

	public void ApplyImpulse(Vector2 amount)
	{
		m_body.ApplyLinearImpulse(amount * 0.85f / 100f, m_body.WorldCenter);
	}

	public void Rotate(float f)
	{
		m_body.AngularVelocity = f;
	}

	public void CleanObject()
	{
		PhysicsObjectManager.RemoveObject(this);
	}

	public void ResetSimulation()
	{
		m_simulation = PhysicsObjectManager.GetSimulation();
	}

	public void ResetDynamics()
	{
		m_body.Rotation = 0f;
	}

	public List<Fixture> GetFixtures()
	{
		return m_fixtures;
	}

	public static float GetArea(List<Vector2> v)
	{
		float num = 0f;
		for (int i = 0; i < v.Count; i++)
		{
			int index = (i + 1) % v.Count;
			num += v[i].X * v[index].Y;
			num -= v[i].Y * v[index].X;
		}
		num /= 2f;
		if (!(num < 0f))
		{
			return num;
		}
		return 0f - num;
	}

	public static Vector2 GetCentroid(List<Vector2> v, float area)
	{
		float num = 0f;
		float num2 = 0f;
		float signedArea = GetSignedArea(v);
		float num5;
		if (signedArea > 0f)
		{
			for (int num3 = v.Count - 1; num3 >= 0; num3--)
			{
				int num4 = (num3 - 1) % v.Count;
				if (num4 < 0)
				{
					num4 += v.Count;
				}
				num5 = 0f - (v[num3].X * v[num4].Y - v[num4].X * v[num3].Y);
				num += (v[num3].X + v[num4].X) * num5;
				num2 += (v[num3].Y + v[num4].Y) * num5;
			}
		}
		else
		{
			for (int num3 = 0; num3 < v.Count; num3++)
			{
				int index = (num3 + 1) % v.Count;
				num5 = 0f - (v[num3].X * v[index].Y - v[index].X * v[num3].Y);
				num += (v[num3].X + v[index].X) * num5;
				num2 += (v[num3].Y + v[index].Y) * num5;
			}
		}
		area *= 6f;
		num5 = 1f / area;
		num *= num5;
		num2 *= num5;
		return new Vector2
		{
			X = num,
			Y = num2
		};
	}

	private static float GetSignedArea(List<Vector2> v)
	{
		float num = 0f;
		for (int i = 0; i < v.Count; i++)
		{
			int index = (i + 1) % v.Count;
			num += v[i].X * v[index].Y;
			num -= v[i].Y * v[index].X;
		}
		return num / 2f;
	}
}
