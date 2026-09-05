using Microsoft.Xna.Framework;
using Renderer;
using Scene;

namespace Screens;

public class LoadScreen : Screen
{
	private SceneObjectSpawner m_spawner;

	private SpriteInstance m_bg1;

	private SpriteInstance m_bg2;

	public LoadScreen(SceneObjectSpawner spawner)
		: base(updateParent: false, drawParent: false, inputParent: false)
	{
		m_spawner = spawner;
		m_bg1 = TextureContainer.GetSprite("images/whitesquare", SceneRenderer.GetCameraPosition(), 0f);
		m_bg2 = TextureContainer.GetSprite("images/stegersaurusLogo", SceneRenderer.GetCameraPosition(), 1f);
		m_bg2.WidthScale *= 0.9f;
		m_bg1.Color = Color.Black;
		m_bg1.SurfaceScale = SceneRenderer.GetScreenDim();
		m_bg1.FlatColor = true;
		m_bg2.FlatColor = true;
		m_bg2.Alpha = 0f;
	}

	public override void Draw(TimeTracker gameTime)
	{
		m_bg1.Draw(gameTime);
		m_bg2.Draw(gameTime);
	}

	public override void Update(TimeTracker gameTime)
	{
		if (m_spawner.IsFullyLoaded())
		{
			m_bg2.Alpha -= gameTime.FractionOfSecond;
			if (m_bg2.Alpha <= 0f)
			{
				ScreenStorage.PopScreen("");
			}
			return;
		}
		m_bg2.Alpha += gameTime.FractionOfSecond;
		if (m_bg2.Alpha > 1f)
		{
			m_bg2.Alpha = 1f;
		}
		m_spawner.UpdateLoad();
	}

	public override void HandleInput(TimeTracker gameTime)
	{
	}
}
