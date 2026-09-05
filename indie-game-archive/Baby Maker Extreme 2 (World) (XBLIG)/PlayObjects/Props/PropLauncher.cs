using System.Collections.Generic;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using PhysicsHandler;

namespace PlayObjects.Props;

public class PropLauncher : PropEffector
{
	private List<Joint> m_jointsToDisconnect;

	private List<PhysicalRepresentation> m_bodiesToLaunch;

	private List<Vector2> m_vDirectionsToLaunch;

	private List<float> m_spins;

	private bool m_bActivated;

	private bool m_bActivateOnUpdate;

	public PropLauncher(List<Joint> joints, List<PhysicalRepresentation> bodies, List<Vector2> vel, List<float> spins)
	{
		m_jointsToDisconnect = joints;
		m_bodiesToLaunch = bodies;
		m_vDirectionsToLaunch = vel;
		m_spins = spins;
		m_bActivated = false;
	}

	public override void CollisionResponse(Player p, Vector2 pos)
	{
		if (!m_bActivated)
		{
			m_bActivated = true;
			m_bActivateOnUpdate = true;
		}
	}

	public override void Update(TimeTracker gameTime)
	{
		if (m_bActivateOnUpdate)
		{
			for (int i = 0; i < m_jointsToDisconnect.Count; i++)
			{
				PhysicsObjectManager.GetSimulation().RemoveJoint(m_jointsToDisconnect[i]);
			}
			for (int j = 0; j < m_bodiesToLaunch.Count; j++)
			{
				m_bodiesToLaunch[j].Velocity += m_vDirectionsToLaunch[j];
				m_bodiesToLaunch[j].Rotate(m_spins[j]);
			}
			m_bActivateOnUpdate = false;
		}
	}

	public override void Reset()
	{
		if (m_bActivated)
		{
			for (int i = 0; i < m_jointsToDisconnect.Count; i++)
			{
				PhysicsObjectManager.GetSimulation().AddJoint(m_jointsToDisconnect[i]);
			}
			m_bActivated = false;
		}
	}
}
