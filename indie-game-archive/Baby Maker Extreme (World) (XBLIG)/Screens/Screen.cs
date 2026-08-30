using BabyMaker;

namespace Screens;

public abstract class Screen
{
	private bool m_bUpdateParentScreen;

	private bool m_bDrawParentScreen;

	private bool m_bHandleInputParentScreen;

	public bool DrawParent => m_bDrawParentScreen;

	public bool UpdateParent => m_bUpdateParentScreen;

	public bool HandleInputParent => m_bHandleInputParentScreen;

	public Screen(bool updateParent, bool drawParent, bool inputParent)
	{
		m_bUpdateParentScreen = updateParent;
		m_bDrawParentScreen = drawParent;
		m_bHandleInputParentScreen = inputParent;
		Game1.PushScreen(this);
	}

	public abstract void Update(TimeTracker gameTime);

	public abstract void Draw(TimeTracker gameTime);

	public abstract void HandleInput(TimeTracker gameTime);

	public virtual void OnRegainFocus(string applicatorInfo)
	{
	}
}
