using BabyMaker;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Physics;
using Renderer;
using Scene;

namespace Screens;

public class LoadingScreen : Screen
{
	private string m_sLoadString;

	private RenderSprite m_bg;

	private PropPool m_pool;

	private string m_sLoadString2;

	private string m_dots;

	private float m_fTimer;

	public LoadingScreen(PropPool pool)
		: base(updateParent: false, drawParent: true, inputParent: false)
	{
		m_bg = SpriteManager.GetSprite("images/whitesquare", SceneRenderer.GetCameraPosition(), 1000f);
		m_bg.Color = Color.Black;
		m_bg.SurfaceScale = new Vector2(1280f, 1280f);
		m_sLoadString = "LOADING, please wait...";
		m_dots = "";
		m_sLoadString2 = "Conceiving" + m_dots;
		m_pool = pool;
		m_fTimer = 0f;
	}

	public override void Draw(TimeTracker gameTime)
	{
		m_bg.Position = SceneRenderer.GetCameraPosition();
		m_bg.Draw(gameTime);
		SceneRenderer.DrawString(fonts.CASH_COUNT_FONT, m_sLoadString, SceneRenderer.GetCameraPosition() - new Vector2(SceneRenderer.GetStringWidth(fonts.CASH_COUNT_FONT, m_sLoadString) / 2f, 70f), Color.White, m_bg.Depth + 1f);
		SceneRenderer.DrawString(fonts.CASH_COUNT_FONT, m_sLoadString2, SceneRenderer.GetCameraPosition() - new Vector2(SceneRenderer.GetStringWidth(fonts.CASH_COUNT_FONT, m_sLoadString2) / 2f, 0f), Color.White, m_bg.Depth + 1f);
	}

	public override void Update(TimeTracker gameTime)
	{
		if (m_pool.IsCompleteInit())
		{
			m_bg.Alpha -= gameTime.FractionOfSecond;
			if (m_bg.Alpha <= 0f)
			{
				m_bg.Alpha = 0f;
				m_pool.ResetProps();
				Game1.PopScreen("");
				PhysicsObjectManager.GetSimulation().Update(1f / 60f);
			}
		}
		m_pool.PercentLoaded();
		m_pool.UpdateInit();
		float num = m_pool.PercentLoaded();
		m_fTimer += gameTime.FractionOfSecond;
		if (m_fTimer > 0.5f)
		{
			m_dots += ".";
			if (m_dots.Length > 3)
			{
				m_dots = "";
			}
			m_fTimer = 0f;
		}
		if (num <= 0.2f)
		{
			m_sLoadString2 = "Fertilizing" + m_dots;
		}
		else if (num <= 0.3f)
		{
			m_sLoadString2 = "Gestating" + m_dots;
		}
		else if (num <= 0.4f)
		{
			m_sLoadString2 = "Breaking water" + m_dots;
		}
		else if (num <= 0.5f)
		{
			m_sLoadString2 = "Finding overnight bag" + m_dots;
		}
		else if (num <= 0.6f)
		{
			m_sLoadString2 = "Driving to hospital" + m_dots;
		}
		else if (num <= 0.7f)
		{
			m_sLoadString2 = "Getting stuck in traffic" + m_dots;
		}
		else if (num <= 0.8f)
		{
			m_sLoadString2 = "Entering admissions" + m_dots;
		}
		else if (num <= 1f)
		{
			m_sLoadString2 = "Entering maternity ward" + m_dots;
		}
	}

	public override void HandleInput(TimeTracker gameTime)
	{
	}
}
