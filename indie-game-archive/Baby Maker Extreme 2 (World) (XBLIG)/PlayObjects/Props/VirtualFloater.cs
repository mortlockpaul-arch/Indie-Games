using Microsoft.Xna.Framework;
using PhysicsHandler;
using Renderer;

namespace PlayObjects.Props;

public class VirtualFloater : PropEffector
{
	private bool m_bHit;

	private float m_fGoalHeight;

	private PhysicalRepresentation m_obj;

	private float m_fpow;

	public VirtualFloater(PhysicalRepresentation obj)
	{
		m_bHit = false;
		m_obj = obj;
		m_fGoalHeight = SceneRenderer.GetRand(230f, 300f);
		m_fpow = SceneRenderer.GetRand(400f, 1000f);
	}

	public override void Update(TimeTracker gameTime)
	{
		if (!m_bHit && m_obj.Position.Y > m_fGoalHeight)
		{
			m_obj.ApplyImpulse(new Vector2(0f, (0f - m_fpow) * gameTime.FractionOfSecond));
		}
		else if (!m_bHit)
		{
			m_obj.ApplyImpulse(new Vector2(0f, m_fpow * gameTime.FractionOfSecond));
		}
	}

	public override void CollisionResponse(Player p, Vector2 pos)
	{
		m_bHit = true;
	}

	public override void Reset()
	{
		m_bHit = false;
		m_fGoalHeight = SceneRenderer.GetRand(230f, 300f);
		m_fpow = SceneRenderer.GetRand(400f, 1000f);
	}
}
