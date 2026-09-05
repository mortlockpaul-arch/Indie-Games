using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Renderer;

namespace Screens;

public class UnlockScreen : Screen
{
	private SpriteInstance m_bg;

	private string m_text1;

	private List<string> m_unlocks;

	private string m_continue;

	private ParticleRect m_particleRect;

	public UnlockScreen(List<string> names)
		: base(updateParent: false, drawParent: true, inputParent: false)
	{
		m_bg = TextureContainer.GetSprite("images/score", SceneRenderer.GetCameraPosition(), 100f);
		m_bg.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/scoreNorm");
		m_bg.SurfaceScale = new Vector2(650f, 500f);
		m_text1 = "Congratulations!";
		m_unlocks = names;
		m_continue = "Press A to continue";
		Vector2 vector = m_bg.Position - m_bg.SurfaceScale * 0.4f;
		Vector2 vector2 = m_bg.SurfaceScale * 0.8f;
		m_particleRect = new ParticleRect(new Rectangle((int)vector.X, (int)vector.Y, (int)vector2.X, (int)vector2.Y), 20, 50, 300f, 500f, 0f);
	}

	public override void Draw(TimeTracker gameTime)
	{
		m_bg.Draw(gameTime);
		SceneRenderer.DrawStringCentered(fonts.BUTTON_FONT, m_text1, m_bg.Position - new Vector2(0f, 200f), Color.Black, m_bg.Depth + 1f);
		for (int i = 0; i < m_unlocks.Count; i++)
		{
			if (-(160 - 50 * i) < 120)
			{
				SceneRenderer.DrawString(fonts.BASE_FONT, m_unlocks[i], m_bg.Position - new Vector2(250f, 160 - 50 * i), Color.Black, m_bg.Depth + 1f);
				continue;
			}
			SceneRenderer.DrawString(fonts.BASE_FONT, "Plus " + (m_unlocks.Count - i) + " other unlocks", m_bg.Position - new Vector2(250f, 160 - 50 * i), Color.Black, m_bg.Depth + 1f);
			break;
		}
		SceneRenderer.DrawStringCentered(fonts.BUTTON_FONT, m_continue, m_bg.Position + new Vector2(0f, 200f), Color.Black, m_bg.Depth + 1f);
	}

	public override void Update(TimeTracker gameTime)
	{
		m_particleRect.Update(gameTime);
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (ControlManager.PressedActivate(ControlManager.ActiveMenuIndex))
		{
			ScreenStorage.PopScreen("");
		}
	}
}
