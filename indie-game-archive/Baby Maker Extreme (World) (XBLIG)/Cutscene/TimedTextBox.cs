using Microsoft.Xna.Framework.Input;
using Renderer;

namespace Cutscene;

public class TimedTextBox
{
	private int m_iTextAppearTime;

	private int m_iTimer;

	private TextBox m_TextBox;

	private bool m_bCompleted;

	public bool IsShowing
	{
		get
		{
			if (m_iTimer >= m_iTextAppearTime)
			{
				return !m_bCompleted;
			}
			return false;
		}
	}

	public TimedTextBox(int timer, bool alignLeft, bool alignBottom, RenderSprite spr, string text)
	{
		m_bCompleted = false;
		m_iTextAppearTime = timer;
		m_iTimer = 0;
		m_TextBox = new TextBox(alignLeft, alignBottom, spr, text);
	}

	public void SetTime(int i)
	{
		m_iTimer = i;
		if (i < m_iTextAppearTime)
		{
			m_bCompleted = false;
			m_TextBox.ResetText();
		}
	}

	public void Update(TimeTracker gameTime)
	{
		m_iTimer += gameTime.ElapsedMilli;
		if (IsShowing)
		{
			m_TextBox.Update(gameTime);
		}
	}

	public void HandleInput(TimeTracker gameTime)
	{
		if (m_iTimer >= m_iTextAppearTime && (ControlManager.PressedButton(ControlManager.ActiveMenuIndex, (Buttons)4096) || ControlManager.PressedButton(ControlManager.ActiveMenuIndex, (Buttons)8192) || ControlManager.PressedButton(ControlManager.ActiveMenuIndex, (Buttons)16384) || ControlManager.PressedButton(ControlManager.ActiveMenuIndex, (Buttons)32768) || ControlManager.PressedActivate(ControlManager.ActiveMenuIndex)))
		{
			if (m_TextBox.ShownAllText)
			{
				m_bCompleted = true;
			}
			else
			{
				m_TextBox.ShownAllText = true;
			}
		}
	}

	public void Draw(TimeTracker gameTime)
	{
		m_TextBox.Draw(gameTime);
	}
}
