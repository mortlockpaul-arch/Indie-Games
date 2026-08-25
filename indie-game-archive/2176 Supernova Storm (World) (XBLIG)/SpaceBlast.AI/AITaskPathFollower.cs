using System;
using Microsoft.Xna.Framework;
using SpaceBlast.PathFinding;

namespace SpaceBlast.AI;

internal abstract class AITaskPathFollower : AITask
{
	protected const float constWaypointProximityDistance = 2000f;

	protected const float constMaxSpeedProximity = 10000f;

	protected PlannedPath m_PlannedPath;

	protected Waypoint m_PreviousWaypoint;

	public AITaskPathFollower(AIBrain brain, PlannedPath path)
		: base(brain)
	{
		m_PlannedPath = path;
	}

	public bool FollowPath()
	{
		return FollowPath(2000f);
	}

	public bool FollowPath(float destProximity)
	{
		Ship theShip = m_Brain.Player.TheShip;
		Waypoint waypoint = m_PlannedPath.Route[0];
		Vector2 vector = waypoint.Position - new Vector2(theShip.Position.X, theShip.Position.Y);
		if (m_PlannedPath.Route.Count == 1 && vector.Length() < destProximity)
		{
			return true;
		}
		if (m_Brain.TimeSlice == 0 && m_PlannedPath.Route.Count > 1)
		{
			Vector3 start = theShip.Position;
			Vector3 end = new Vector3(m_PlannedPath.Route[1].Position, 0f);
			Line line = new Line(ref start, ref end, (int)theShip.Diameter);
			if (!MainGame.LevelData.StaticWorldObjects.CollisionTest(line))
			{
				m_PreviousWaypoint = m_PlannedPath.Route[0];
				m_PlannedPath.Route.RemoveAt(0);
			}
		}
		if (m_Brain.TimeSlice == 2 && m_PlannedPath.Route.Count > 1 && vector.Length() < 2000f)
		{
			m_PreviousWaypoint = m_PlannedPath.Route[0];
			m_PlannedPath.Route.RemoveAt(0);
		}
		if (m_Brain.TimeSlice == 4)
		{
			MainGame.DebugObj = null;
			Vector3 start2 = theShip.Position;
			Vector3 end2 = new Vector3(m_PlannedPath.Route[0].Position, 0f);
			Line line2 = (MainGame.DebugLine = new Line(ref start2, ref end2, (int)theShip.Diameter));
			if (MainGame.LevelData.StaticWorldObjects.CollisionTest(line2))
			{
				theShip.TargetSpeed = 50f;
				return true;
			}
		}
		if (m_Brain.TimeSlice == 6)
		{
			Vector3 position = theShip.Position;
			Vector3 vector2 = new Vector3(m_PlannedPath.Route[0].Position, 0f);
			Vector3 vec = vector2 - position;
			float targetRotation = Utils.AngleFromVector(ref vec) - (float)Math.PI / 2f;
			float num = vec.Length();
			Vector3 velocity = theShip.Velocity;
			Vector3 vec2 = Vector3.Reflect(velocity, vec);
			Utils.AngleFromVector(ref vec2);
			float targetSpeed = theShip.MaxSpeed;
			if (num < 10000f)
			{
				targetSpeed = 50f;
			}
			theShip.TargetRotation = targetRotation;
			theShip.TargetSpeed = targetSpeed;
		}
		return false;
	}
}
