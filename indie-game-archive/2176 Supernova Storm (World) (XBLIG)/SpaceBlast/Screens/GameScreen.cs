using Microsoft.Xna.Framework;

namespace SpaceBlast.Screens;

internal abstract class GameScreen
{
	protected ScreenManager m_ScreenManager;

	protected bool m_NoBackground;

	public bool HasBackground => !m_NoBackground;

	public GameScreen(ScreenManager manager)
	{
		m_ScreenManager = manager;
	}

	public virtual void LoadContent()
	{
	}

	public virtual void OnScreenResize()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void Draw(float alpha)
	{
	}

	public virtual void OnShowScreen()
	{
	}

	public virtual void OnHideScreen()
	{
	}

	public abstract Rectangle GetScreenRect();
}
