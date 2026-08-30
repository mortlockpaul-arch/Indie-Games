using System.Collections.Generic;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Collisions;
using Microsoft.Xna.Framework;
using PlayerData;

namespace Physics;

public static class PhysicsObjectManager
{
	private static PhysicsSimulator m_simulation;

	private static Dictionary<Geom, Player> m_Players;

	private static List<Player> m_PlayerList;

	private static List<Geom> m_savedGeometry;

	private static ScalingController m_scaleControl;

	public static int Contacts
	{
		get
		{
			return m_simulation.MaxContactsToResolve;
		}
		set
		{
			m_simulation.MaxContactsToResolve = value;
		}
	}

	public static int Iterations
	{
		get
		{
			return m_simulation.Iterations;
		}
		set
		{
			m_simulation.Iterations = value;
		}
	}

	public static bool Scaling
	{
		get
		{
			return m_simulation.ControllerList.Contains(m_scaleControl);
		}
		set
		{
			if (value && !m_simulation.ControllerList.Contains(m_scaleControl))
			{
				m_simulation.Add(m_scaleControl);
			}
			else if (!value && m_simulation.ControllerList.Contains(m_scaleControl))
			{
				m_simulation.Remove(m_scaleControl);
			}
		}
	}

	public static void Update(TimeTracker gameTime)
	{
		m_simulation.Update(gameTime.PhysicsFractionOfSecond);
	}

	public static void Clear()
	{
		PhysicsSimulator physicsSimulator = new PhysicsSimulator(m_simulation.Gravity);
		for (int i = 0; i < m_simulation.GeomList.Count; i++)
		{
			physicsSimulator.Add(m_simulation.GeomList[i]);
			physicsSimulator.Add(m_simulation.GeomList[i].Body);
		}
		for (int j = 0; j < m_simulation.JointList.Count; j++)
		{
			physicsSimulator.Add(m_simulation.JointList[j]);
		}
		m_simulation.Clear();
		m_simulation = physicsSimulator;
	}

	public static void Initialize(float gravity)
	{
		if (m_simulation != null)
		{
			m_simulation.Clear();
		}
		else
		{
			m_simulation = new PhysicsSimulator(new Vector2(0f, gravity));
			m_scaleControl = new ScalingController(1f / 60f, 1f / 30f);
		}
		m_Players = new Dictionary<Geom, Player>();
		m_PlayerList = new List<Player>();
		m_savedGeometry = new List<Geom>(1000);
		Scaling = true;
	}

	public static PhysicsSimulator GetSimulation()
	{
		return m_simulation;
	}

	public static void AddObject(Geom g)
	{
		m_savedGeometry.Add(g);
	}

	public static void RemoveObject(Geom g)
	{
		m_savedGeometry.Remove(g);
	}

	public static Player GetPlayer(Geom g)
	{
		return m_Players[g];
	}

	public static CollisionCategory PlayerCollisionGroup()
	{
		return CollisionCategory.Cat1;
	}

	public static CollisionCategory WallCollisionGroup()
	{
		return CollisionCategory.Cat3;
	}

	public static PhysicalRepresentation CreatePhysicalRepresentation(Rectangle r, CollisionCategory c)
	{
		return new PhysicalRepresentation(m_simulation, r, c);
	}

	public static PhysicalRepresentation CreatePhysicalRepresentation(Vector2 pos, Vector2 size, CollisionCategory c)
	{
		return new PhysicalRepresentation(m_simulation, pos, size, c);
	}

	public static PhysicalRepresentation CreatePhysicalRepresentation(int radius, Vector2 pos, CollisionCategory c)
	{
		return new PhysicalRepresentation(m_simulation, radius, pos, c);
	}

	public static PhysicalRepresentation CreatePhysicalRepresentation(List<Vector2> vertices, Vector2 pos, CollisionCategory c)
	{
		return new PhysicalRepresentation(m_simulation, vertices, pos, c);
	}

	public static PhysicalRepresentation CreatePhysicalRepresentation(Vertices vertices, Vector2 pos, CollisionCategory c)
	{
		return new PhysicalRepresentation(m_simulation, vertices, pos, c, subDiv: true);
	}

	public static PhysicalRepresentation CreatePhysicalRepresentationNoSubDiv(Vertices vertBody, Vertices vertGeom, Vector2 pos, CollisionCategory c)
	{
		return new PhysicalRepresentation(m_simulation, vertBody, vertGeom, pos, c);
	}

	public static void AddPlayerGeom(PhysicalRepresentation g, Player p)
	{
		m_Players[g.GetGeom()] = p;
		m_PlayerList.Add(p);
	}

	public static void RemoveObject(PhysicalRepresentation r)
	{
		r.Enabled = false;
		m_savedGeometry.Remove(r.GetGeom());
		m_simulation.Remove(r.GetGeom());
		if (r.GetGeom().CollisionCategories == PlayerCollisionGroup())
		{
			m_Players.Remove(r.GetGeom());
			return;
		}
		_ = r.GetGeom().CollisionCategories;
		WallCollisionGroup();
	}
}
