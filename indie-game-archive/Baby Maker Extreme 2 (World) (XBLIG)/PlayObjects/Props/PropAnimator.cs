using Microsoft.Xna.Framework;
using PhysicsHandler;
using Renderer;

namespace PlayObjects.Props;

public class PropAnimator : PropEffector
{
	private AnimatedRenderSprite m_spr;

	private PhysicalRepresentation m_objConnect;

	private bool m_bCollided;

	public PropAnimator(AnimatedRenderSprite spr, PhysicalRepresentation objConnect, int mode)
	{
		m_spr = spr;
		m_objConnect = objConnect;
		m_bCollided = false;
	}

	public override void CollisionResponse(Player p, Vector2 pos)
	{
		m_bCollided = true;
	}

	public override void Draw(TimeTracker gameTime)
	{
		m_spr.Draw(gameTime);
	}

	public override void Reset()
	{
		m_bCollided = false;
		m_spr.Alpha = 1f;
	}

	public override void Update(TimeTracker gameTime)
	{
		m_spr.Position = m_objConnect.Position;
		m_spr.Rotation = m_objConnect.Rotation;
		m_spr.Update(gameTime);
		if (m_bCollided)
		{
			m_spr.Alpha -= gameTime.FractionOfSecond * 2f;
			if (m_spr.Alpha < 0f)
			{
				m_spr.Alpha = 0f;
			}
		}
	}
}
