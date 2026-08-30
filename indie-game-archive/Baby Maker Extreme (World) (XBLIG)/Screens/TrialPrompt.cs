using System.Collections.Generic;
using BabyMaker;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Renderer;

namespace Screens;

public class TrialPrompt : Screen
{
	private RenderSprite m_bg;

	private string m_text;

	private List<string> m_options;

	private int m_index;

	private bool m_bEscape;

	public TrialPrompt()
		: base(updateParent: false, drawParent: true, inputParent: false)
	{
		m_bg = SpriteManager.GetSprite("images/pausebg", default(Vector2), DepthConsts.PAUSE_DEPTH - 0.1f);
		m_bg.Alpha = 0f;
		m_text = "Oh no! It's the trial wall!\nPurchase now to break\nthrough and continue into\nthe hospital grounds.\n";
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
		m_bg.Position = SceneRenderer.GetCameraPosition();
		m_bg.Draw(gameTime);
		SceneRenderer.DrawString(fonts.BASE_FONT, m_text, m_bg.Position + new Vector2(-230f, -200f), black, DepthConsts.PAUSE_DEPTH);
		for (int i = 0; i < m_options.Count; i++)
		{
			Color c = black;
			if (i == m_index)
			{
				c = red;
			}
			SceneRenderer.DrawString(fonts.BUTTON_FONT, m_options[i], m_bg.Position + new Vector2((0f - SceneRenderer.GetStringWidth(fonts.BUTTON_FONT, m_options[i])) / 2f, 100 + 50 * i), c, DepthConsts.PAUSE_DEPTH);
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
				Game1.PopScreen("");
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
