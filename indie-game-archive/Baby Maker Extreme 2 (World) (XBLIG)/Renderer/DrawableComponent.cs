namespace Renderer;

public class DrawableComponent
{
	protected float m_fDepth;

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

	public DrawableComponent(float depth)
	{
		m_fDepth = depth;
	}

	public virtual void Draw(TimeTracker gameTime)
	{
	}
}
