using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

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
			Guide.ShowSignIn(1, true);
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
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected I4, but got Unknown
		m_Player = -1;
		m_SigningIn = false;
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				if (current.IsSignedInToLive)
				{
					m_Player = (int)current.PlayerIndex;
					break;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		if (m_Player != -1)
		{
			return true;
		}
		return false;
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
		((Vector2)(ref val))._002Ector(1f, 1f);
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
