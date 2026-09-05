using System.Collections.Generic;
using MathTools;
using Microsoft.Xna.Framework;
using PhysicsHandler;
using Screens;

namespace PlayObjects.Props;

public class PropParticleStream : PropEffector
{
	private List<ParticleEmitter> m_emitters;

	private List<PhysicalRepresentation> m_objs;

	private List<Vector2> m_relativePos;

	private bool m_bActivated;

	private bool m_bStartActivated;

	private List<float> m_angles;

	public PropParticleStream(List<ParticleEmitter> emitters, List<PhysicalRepresentation> objs, List<Vector2> relPos, bool startsActive)
	{
		m_relativePos = relPos;
		m_emitters = emitters;
		m_objs = objs;
		m_bStartActivated = startsActive;
		m_bActivated = m_bStartActivated;
		m_angles = new List<float>();
		for (int i = 0; i < m_emitters.Count; i++)
		{
			m_angles.Add(m_emitters[i].Angle);
		}
	}

	public override void CollisionResponse(Player p, Vector2 pos)
	{
		m_bActivated = true;
	}

	public override void Reset()
	{
		m_bActivated = m_bStartActivated;
	}

	public override void Update(TimeTracker gameTime)
	{
		if (m_bActivated)
		{
			for (int i = 0; i < m_emitters.Count; i++)
			{
				Vector2 vector = VectorTools.Rotate(m_relativePos[i], m_objs[i].Rotation);
				m_emitters[i].Position = m_objs[i].Position + vector;
				m_emitters[i].Angle = m_angles[i] + m_objs[i].Rotation;
				m_emitters[i].Update(gameTime);
			}
		}
	}
}
