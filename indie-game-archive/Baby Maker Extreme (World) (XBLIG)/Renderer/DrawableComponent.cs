using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Renderer;

public class DrawableComponent
{
	private float m_fDepth;

	protected float m_fRotation;

	protected Vector2 m_vSurfaceScale;

	protected Vector2 m_vPosition;

	protected float m_fAlpha;

	protected Color m_cTint;

	public float Depth
	{
		get
		{
			return m_fDepth;
		}
		set
		{
			m_fDepth = value;
		}
	}

	public virtual float Rotation
	{
		get
		{
			return m_fRotation;
		}
		set
		{
			m_fRotation = value;
		}
	}

	public virtual Vector2 SurfaceScale
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vSurfaceScale;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_vSurfaceScale = value;
		}
	}

	public virtual Vector2 Position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vPosition;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_vPosition = value;
		}
	}

	public virtual float Alpha
	{
		get
		{
			return m_fAlpha;
		}
		set
		{
			m_fAlpha = value;
			if (m_fAlpha < 0f)
			{
				m_fAlpha = 0f;
			}
		}
	}

	public virtual Color Color
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_cTint;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_cTint = value;
		}
	}

	public DrawableComponent(float depth)
	{
		m_fDepth = depth;
	}

	public virtual void Update(TimeTracker gameTime)
	{
	}

	public virtual void Draw(TimeTracker gameTime)
	{
	}

	public virtual int GetAnimationTimer()
	{
		return 0;
	}
}
