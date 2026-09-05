using Microsoft.Xna.Framework;
using Renderer;
using Screens;

namespace PlayObjects.Props;

public class VirtualTransitioner : PropEffector
{
	private TransitionHelper m_trans;

	private bool m_bStarted;

	public VirtualTransitioner()
	{
		m_trans = new TransitionHelper();
		m_bStarted = false;
		m_trans.TransitionIn();
	}

	public override void CollisionResponse(Player p, Vector2 pos)
	{
		m_bStarted = true;
	}

	public override void Draw(TimeTracker gameTime)
	{
		if (!m_trans.IsTransitionedOut)
		{
			m_trans.Draw(gameTime);
		}
	}

	public override void Update(TimeTracker gameTime)
	{
		if (m_bStarted)
		{
			m_trans.Update(gameTime);
			if (m_trans.IsTransitionedIn)
			{
				SceneRenderer.SetEffect(1);
			}
		}
	}

	public override void Reset()
	{
		m_bStarted = false;
		m_trans.TransitionIn();
	}
}
