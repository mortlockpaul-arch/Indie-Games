using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_ScreenRect;
	}

	public override void OnScreenResize()
	{
		RecalcScreenRect();
		base.OnScreenResize();
	}

	private void RecalcScreenRect()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(500f, 200f);
		Viewport viewport = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
		float num = ((Viewport)(ref viewport)).Width;
		float num2 = num * 0.5f;
		Viewport viewport2 = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
		float num3 = ((Viewport)(ref viewport2)).Height;
		float num4 = num3 * 0.5f;
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(num2 - val.X * 0.5f, num4 - val.Y * 0.5f);
		m_ScreenRect = default(Rectangle);
		m_ScreenRect.X = (int)val2.X;
		m_ScreenRect.Y = (int)val2.Y;
		m_ScreenRect.Width = (int)val.X;
		m_ScreenRect.Height = (int)val.Y;
	}
}
