using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using PhysicsHandler;
using Renderer;

namespace PlayObjects;

public class ObstacleRect
{
	private PhysicalRepresentation m_obj;

	private SpriteInstance m_spr;

	public bool Static
	{
		get
		{
			return m_obj.Static;
		}
		set
		{
			m_obj.Static = value;
		}
	}

	public Vector2 Position
	{
		get
		{
			return m_obj.Position;
		}
		set
		{
			m_obj.Position = value;
		}
	}

	public bool Enabled
	{
		get
		{
			return m_obj.Enabled;
		}
		set
		{
			m_obj.Enabled = value;
		}
	}

	public ObstacleRect(Vector2 scale, bool isCeil)
	{
		m_obj = PhysicsObjectManager.CreatePhysicalRepresentation(default(Vector2), scale, Category.Cat2, scale: true);
		if (isCeil)
		{
			m_spr = TextureContainer.GetSprite("images/ceiling", default(Vector2), 2f);
			m_spr.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/ceilingNorm");
		}
		else
		{
			m_spr = TextureContainer.GetSprite("images/floor", default(Vector2), 2f);
			m_spr.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/floorNorm");
		}
		m_spr.SurfaceScale = scale;
	}

	public void Update(TimeTracker gameTime)
	{
		m_spr.Rotation = m_obj.Rotation;
		m_spr.Position = m_obj.Position;
	}

	public void Draw(TimeTracker gameTime)
	{
		m_spr.Draw(gameTime);
	}
}
