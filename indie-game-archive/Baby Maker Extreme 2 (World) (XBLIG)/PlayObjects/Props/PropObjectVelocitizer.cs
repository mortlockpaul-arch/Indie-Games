using Microsoft.Xna.Framework;
using PhysicsHandler;

namespace PlayObjects.Props;

public class PropObjectVelocitizer : PropEffector
{
	private PhysicalRepresentation m_body;

	private Vector2 m_vel;

	private int m_counter;

	public PropObjectVelocitizer(PhysicalRepresentation body, Vector2 vel)
	{
		body.Velocity = vel;
		m_body = body;
		m_vel = vel;
		m_counter = 0;
	}

	public override void Reset()
	{
		m_body.Velocity = m_vel;
		m_counter = 0;
	}

	public override void Update(TimeTracker gameTime)
	{
		if (m_counter < 10 && m_body.Enabled && !m_body.Static)
		{
			m_counter++;
			m_body.Velocity = m_vel;
		}
	}
}
