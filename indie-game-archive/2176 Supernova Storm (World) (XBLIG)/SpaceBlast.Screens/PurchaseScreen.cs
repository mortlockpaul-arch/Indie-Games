using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace SpaceBlast.Screens;

internal class PurchaseScreen(ScreenManager manager) : GameScreen(manager)
{
	private Rectangle m_ScreenRect;

	private int m_Player = -1;

	private bool m_SigningIn;

	private bool m_Purchasing;

	public override void LoadContent()
	{
		RecalcScreenRect();
		base.LoadContent();
	}

	public override void OnShowScreen()
	{
		m_Purchasing = false;
		m_SigningIn = false;
		if (!FindLivePlayer())
		{
			Guide.ShowSignIn(1, onlineOnly: true);
			m_SigningIn = true;
		}
		base.OnShowScreen();
	}

	public override void Draw(float alpha)
	{
		base.Draw(alpha);
	}

	public override void Update()
	{
		if (!Guide.IsVisible)
		{
			if (m_SigningIn)
			{
				if (FindLivePlayer())
				{
					m_SigningIn = false;
				}
				else
				{
					m_ScreenManager.ShowScreen(ScreenType.MainMenu);
				}
			}
			if (m_Purchasing)
			{
				if (Guide.IsTrialMode)
				{
					m_ScreenManager.ShowScreen(ScreenType.MainMenu);
				}
				else
				{
					m_ScreenManager.ShowScreen(ScreenType.ThankYou);
				}
			}
			else if (FindLivePlayer())
			{
				Guide.ShowMarketplace((PlayerIndex)m_Player);
				m_Purchasing = true;
			}
		}
		base.Update();
	}

	private bool FindLivePlayer()
	{
		m_Player = -1;
		m_SigningIn = false;
		foreach (SignedInGamer signedInGamer in Gamer.SignedInGamers)
		{
			if (signedInGamer.IsSignedInToLive)
			{
				m_Player = (int)signedInGamer.PlayerIndex;
				break;
			}
		}
		if (m_Player != -1)
		{
			return true;
		}
		return false;
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
		Vector2 vector = new Vector2(1f, 1f);
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
