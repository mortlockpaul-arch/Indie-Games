using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace SpaceBlast.Screens;

internal class SplitScreenMenuScreen : MenuScreen
{
	private Rectangle m_ScreenRect;

	public SplitScreenMenuScreen(ScreenManager manager)
		: base(manager)
	{
		MenuItems.Add(new MenuItem("Player Vs Player", 0));
		MenuItems.Add(new MenuItem("Humans Vs Cyborgs (Co-Op Play)", 1));
		MenuItems.Add(new MenuItem("All Against All", 2));
		RedButtonText = "Back";
		GreenButtonText = "Select";
		YellowButtonText = null;
		BlueButtonText = null;
	}

	public override void LoadContent()
	{
		RecalcScreenRect();
		base.LoadContent();
	}

	protected override void OnGreenButtonPressed()
	{
		switch (base.SelectedItemID)
		{
		case 0:
		{
			SplitScreenGameOptions gameOptions = new SplitScreenGameOptions();
			MainGame.Instance.StartNewGame(gameOptions);
			break;
		}
		case 1:
			if (Guide.IsTrialMode)
			{
				m_ScreenManager.ShowScreen(ScreenType.TrialRestriction);
			}
			else
			{
				m_ScreenManager.ShowScreen(ScreenType.SplitScreenCoOpOptions);
			}
			break;
		case 2:
			if (Guide.IsTrialMode)
			{
				m_ScreenManager.ShowScreen(ScreenType.TrialRestriction);
			}
			else
			{
				m_ScreenManager.ShowScreen(ScreenType.SplitScreenAllVsAllOptions);
			}
			break;
		case 3:
			if (Guide.IsTrialMode)
			{
				m_ScreenManager.ShowScreen(ScreenType.TrialRestriction);
			}
			else
			{
				m_ScreenManager.ShowScreen(ScreenType.SplitScreenCustomOptions);
			}
			break;
		}
		base.OnGreenButtonPressed();
	}

	protected override void OnRedButtonPressed()
	{
		m_ScreenManager.ShowScreen(ScreenType.MainMenu);
		base.OnRedButtonPressed();
	}

	public override Rectangle GetScreenRect()
	{
		return m_ScreenRect;
	}

	public override void OnScreenResize()
	{
		RecalcScreenRect();
		base.OnScreenResize();
	}

	private void RecalcScreenRect()
	{
		Vector2 vector = new Vector2(500f, 200f);
		float num = MainGame.Instance.GraphicsDevice.Viewport.Width;
		float num2 = num * 0.5f;
		float num3 = MainGame.Instance.GraphicsDevice.Viewport.Height;
		float num4 = num3 * 0.5f;
		Vector2 vector2 = new Vector2(num2 - vector.X * 0.5f, num4 - vector.Y * 0.5f);
		m_ScreenRect = default(Rectangle);
		m_ScreenRect.X = (int)vector2.X;
		m_ScreenRect.Y = (int)vector2.Y;
		m_ScreenRect.Width = (int)vector.X;
		m_ScreenRect.Height = (int)vector.Y;
	}
}
