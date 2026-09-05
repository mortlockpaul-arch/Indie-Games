using Microsoft.Xna.Framework;
using Renderer;

namespace Scene;

public class AmbianceElement
{
	private SpriteInstance m_spr;

	private float m_fSpeed;

	public SpriteInstance Sprite => m_spr;

	public AmbianceElement(SpriteInstance spr, float speed)
	{
		m_spr = spr;
		m_fSpeed = speed;
	}

	public void Update(TimeTracker gameTime, Vector2 distChange)
	{
		m_spr.Position += distChange * new Vector2(m_fSpeed, 0f);
	}

	public void Draw(TimeTracker gameTime)
	{
		m_spr.Draw(gameTime);
	}
}
