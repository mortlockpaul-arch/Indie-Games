using System.Collections.Generic;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace PhysicsHandler;

public static class PhysicsObjectManager
{
	public const float CONVERSION = 100f;

	private static World m_simulation;

	private static List<Body> m_savedGeometry;

	private static Dictionary<Body, object> m_Players;

	private static List<object> m_PlayerList;

	public static Category PlayerCollisionGroup()
	{
		return Category.Cat1;
	}

	public static Category WallCollisionGroup()
	{
		return Category.Cat3;
	}

	public static object GetPlayer(Body g)
	{
		return m_Players[g];
	}

	public static void AddPlayerGeom(PhysicalRepresentation g, object p)
	{
		m_Players[g.GetGeom()] = p;
		m_PlayerList.Add(p);
	}

	public static void RemovePlayerGeom(PhysicalRepresentation g)
	{
		m_Players.Remove(g.GetGeom());
	}

	public static void Update(TimeTracker gameTime)
	{
		m_simulation.Step(gameTime.PhysicsFractionOfSecond);
	}

	public static void Clear()
	{
	}

	public static void Initialize(float gravity)
	{
		gravity /= 100f;
		if (m_simulation == null)
		{
			m_simulation = new World(new Vector2(0f, gravity));
		}
		m_Players = new Dictionary<Body, object>();
		m_PlayerList = new List<object>();
		m_savedGeometry = new List<Body>(1000);
		Settings.UseFPECollisionCategories = true;
	}

	public static World GetSimulation()
	{
		return m_simulation;
	}

	public static void AddObject(Body g)
	{
		m_savedGeometry.Add(g);
	}

	public static void RemoveObject(Body g)
	{
		m_savedGeometry.Remove(g);
	}

	public static void RemoveObject(PhysicalRepresentation r)
	{
		r.Enabled = false;
		if (r.CollisionCategory == PlayerCollisionGroup())
		{
			m_Players.Remove(r.GetGeom());
		}
		m_savedGeometry.Remove(r.GetGeom());
		m_simulation.RemoveBody(r.GetGeom());
	}

	public static PhysicalRepresentation CreatePhysicalRepresentation(Rectangle r, Category c, bool scale)
	{
		return new PhysicalRepresentation(m_simulation, r, c, scale);
	}

	public static PhysicalRepresentation CreatePhysicalRepresentation(Vector2 pos, Vector2 size, Category c, bool scale)
	{
		return new PhysicalRepresentation(m_simulation, pos, size, c, scale);
	}

	public static PhysicalRepresentation CreatePhysicalRepresentation(int radius, Vector2 pos, Category c, bool scale)
	{
		return new PhysicalRepresentation(m_simulation, radius, pos, c, scale);
	}

	public static PhysicalRepresentation CreatePhysicalRepresentation(List<Vector2> vertices, Vector2 pos, Category c, bool scale)
	{
		return new PhysicalRepresentation(m_simulation, vertices, pos, c, scale);
	}
}
