using System;
using System.Collections.Generic;
using BabyMakerExtreme2;
using Microsoft.Xna.Framework;
using Renderer;

namespace Screens;

public class UpsellScreen : Screen
{
	private SpriteInstance m_bg;

	private string m_text;

	private List<string> m_options;

	private int m_index;

	private bool m_bEscape;

	private List<SpriteInstance> m_sprites;

	public UpsellScreen(bool isGame, List<SpriteInstance> sprites, int msgNum, bool gotFar)
		: base(updateParent: false, drawParent: true, inputParent: false)
	{
		m_sprites = sprites;
		m_bg = TextureContainer.GetSprite("images/score", default(Vector2), DepthConsts.PAUSE_DEPTH - 0.1f);
		m_bg.Alpha = 0f;
		m_bg.WidthScale *= 0.5f;
		m_bg.SurfaceScale = new Vector2(m_bg.WidthScale, m_bg.WidthScale * 0.7f);
		if (isGame)
		{
			msgNum = Math.Min(5, msgNum);
			switch (msgNum)
			{
			case 0:
				if (gotFar)
				{
					m_text = "Oh no! It's the upsell bear!\nPurchase now to break\nthrough and continue into\nthe park and beyond.\n";
				}
				else
				{
					m_text = "Upsell bear is here to\ntell you there's a big\nworld out there to\nfly through if you purchase\nthe full game";
				}
				break;
			case 1:
				m_text = "Upsell bear would like to\nremind you of all the special\nbabies you could be using if\nyou purchased the full game!";
				break;
			case 2:
				m_text = "Upsell bear doesn't eat fish.\nHe could starve if you don't\nhelp feed him in points!";
				break;
			case 3:
				m_text = "Upsell bear has seen into\nthe machines and knows, the\nonly true path to high scores\nis to unlock the full game.";
				break;
			case 4:
				m_text = "Upsell bear would love to\ndress up, but lacks the\nopposible thumbs, fashion\nsense, and XXXXXXL shirts";
				break;
			case 5:
				m_text = "Upsell bear has run out of\nquips, but will not let you\nexplore the park or beyond\nunless you purchase the full\ngame";
				break;
			}
		}
		else
		{
			m_text = "This mode can't be\naccessed in the trial.\nWould you like to purchase\nthe full game?";
		}
		m_options = new List<string>();
		m_options.Add("Purchase");
		m_options.Add("Cancel");
		m_index = 0;
		m_bEscape = false;
	}

	public override void Draw(TimeTracker gameTime)
	{
		Color black = Color.Black;
		Color red = Color.Red;
		black.A = (byte)(255f * m_bg.Alpha);
		red.A = black.A;
		m_bg.Position = SceneRenderer.GetCameraPosition() + new Vector2(0f, 150f);
		m_bg.Draw(gameTime);
		for (int i = 0; i < m_sprites.Count; i++)
		{
			m_sprites[i].Alpha = m_bg.Alpha;
			m_sprites[i].Draw(gameTime);
		}
		SceneRenderer.DrawString(fonts.BASE_FONT, m_text, m_bg.Position + new Vector2(-160f, -150f), black, DepthConsts.PAUSE_DEPTH);
		for (int j = 0; j < m_options.Count; j++)
		{
			Color c = black;
			if (j == m_index)
			{
				c = red;
			}
			SceneRenderer.DrawStringCentered(fonts.BUTTON_FONT, m_options[j], m_bg.Position + new Vector2(0f, 50 + 50 * j), c, DepthConsts.PAUSE_DEPTH);
		}
	}

	public override void Update(TimeTracker gameTime)
	{
		if (!m_bEscape)
		{
			m_bg.Alpha += gameTime.FractionOfSecond * 2f;
			if (m_bg.Alpha > 1f)
			{
				m_bg.Alpha = 1f;
			}
		}
		else
		{
			m_bg.Alpha -= gameTime.FractionOfSecond;
			if (m_bg.Alpha <= 0f)
			{
				ScreenStorage.PopScreen("");
			}
		}
		if (!Game1.IsTrial())
		{
			m_bEscape = true;
		}
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (m_bEscape || !(m_bg.Alpha > 0.9f))
		{
			return;
		}
		if (ControlManager.PressedActivate(ControlManager.ActiveMenuIndex))
		{
			if (m_index == 0)
			{
				Game1.ShowPurchaseScreen(ControlManager.ActiveMenuIndex);
			}
			else
			{
				m_bEscape = true;
			}
		}
		if (ControlManager.PressedDown(ControlManager.ActiveMenuIndex) || ControlManager.PressedUp(ControlManager.ActiveMenuIndex))
		{
			m_index = 1 - m_index;
		}
	}
}
