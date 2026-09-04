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
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		Ship theShip = m_Brain.Player.TheShip;
		Waypoint waypoint = m_PlannedPath.Route[0];
		Vector2 val = waypoint.Position - new Vector2(theShip.Position.X, theShip.Position.Y);
		if (m_PlannedPath.Route.Count == 1 && ((Vector2)(ref val)).Length() < destProximity)
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
		if (m_Brain.TimeSlice == 2 && m_PlannedPath.Route.Count > 1 && ((Vector2)(ref val)).Length() < 2000f)
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
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(m_PlannedPath.Route[0].Position, 0f);
			Vector3 vec = val2 - position;
			float targetRotation = Utils.AngleFromVector(ref vec) - (float)Math.PI / 2f;
			float num = ((Vector3)(ref vec)).Length();
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
