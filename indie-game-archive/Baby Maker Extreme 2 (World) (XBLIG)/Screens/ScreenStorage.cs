using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Renderer;

namespace Screens;

public static class ScreenStorage
{
	private static List<Screen> m_stkScreens;

	private static List<string> m_fpsTexts;

	private static List<int> m_DrawFrameWait;

	private static SpriteInstance m_border;

	public static void Initialize()
	{
		m_fpsTexts = new List<string>();
		for (int i = 0; i < 70; i++)
		{
			m_fpsTexts.Add(string.Concat(i));
		}
		m_DrawFrameWait = new List<int>();
		m_stkScreens = new List<Screen>();
		ParticleManager.Initialize();
		m_border = TextureContainer.GetSprite("images/whitesquare", default(Vector2), 20000f);
		m_border.SurfaceScale = SceneRenderer.GetScreenDim() * 0.8f;
		m_border.Alpha = 0.2f;
		m_border.FlatColor = true;
	}

	public static void Update(TimeTracker gameTime)
	{
		m_stkScreens.Last().Update(gameTime);
		int num = m_stkScreens.Count - 1;
		while (m_stkScreens[num].UpdateParent)
		{
			num--;
			m_stkScreens[num].Update(gameTime);
		}
		ParticleManager.Update(gameTime);
	}

	public static void HandleInput(TimeTracker gameTime)
	{
		m_stkScreens.Last().HandleInput(gameTime);
		int num = m_stkScreens.Count - 1;
		while (m_stkScreens[num].HandleInputParent)
		{
			num--;
			m_stkScreens[num].HandleInput(gameTime);
		}
	}

	public static void Draw(TimeTracker gameTime)
	{
		int effectMode = SceneRenderer.GetEffectMode();
		float cameraZoom = SceneRenderer.GetCameraZoom();
		if (!(PeekScreen() is GameScreen))
		{
			SceneRenderer.MoveCamera(SceneRenderer.GetCameraPosition(), 0f, 1f);
		}
		if (!(PeekScreen() is GameScreen) && effectMode == 1)
		{
			SceneRenderer.SetEffect(0);
		}
		SceneRenderer.SetRendering();
		int num = m_stkScreens.Count - 1;
		while (num >= 0 && m_stkScreens[num].DrawParent)
		{
			num--;
			if (num < 0)
			{
				continue;
			}
			if (m_stkScreens[num] is GameScreen)
			{
				SceneRenderer.MoveCamera(SceneRenderer.GetCameraPosition(), 0f, cameraZoom);
				SceneRenderer.ResetWorldParam();
				if (effectMode == 1)
				{
					SceneRenderer.SetEffect(1);
				}
			}
			m_stkScreens[num].Draw(gameTime);
			if (!(m_stkScreens[num] is GameScreen) && !(m_stkScreens[num] is OutfitScreen) && SceneRenderer.Avatar != null)
			{
				SceneRenderer.Avatar.ShouldDraw = false;
			}
			SceneRenderer.RenderScene();
			if (m_stkScreens[num] is GameScreen)
			{
				SceneRenderer.MoveCamera(SceneRenderer.GetCameraPosition(), 0f, 1f);
				SceneRenderer.ResetWorldParam();
				if (effectMode == 1)
				{
					SceneRenderer.SetEffect(0);
				}
			}
		}
		m_stkScreens.Last().Draw(gameTime);
		ParticleManager.Draw(gameTime);
		m_DrawFrameWait.Add(gameTime.ElapsedMilli);
		if (m_DrawFrameWait.Count > 20)
		{
			m_DrawFrameWait.RemoveAt(0);
		}
		DrawFPS(gameTime);
		if (!(PeekScreen() is GameScreen) && !(PeekScreen() is OutfitScreen) && SceneRenderer.Avatar != null)
		{
			SceneRenderer.Avatar.ShouldDraw = false;
		}
		SceneRenderer.RenderScene();
		SceneRenderer.EndRendering();
		if (effectMode != SceneRenderer.GetEffectMode())
		{
			SceneRenderer.SetEffect(effectMode);
		}
		SceneRenderer.MoveCamera(SceneRenderer.GetCameraPosition(), 0f, cameraZoom);
	}

	private static void DrawFPS(TimeTracker gameTime)
	{
		m_border.Position = SceneRenderer.GetCameraPosition();
		float num = 0f;
		for (int i = 0; i < m_DrawFrameWait.Count; i++)
		{
			num += (float)m_DrawFrameWait[i];
		}
		num /= (float)m_DrawFrameWait.Count;
		num /= 1000f;
		if (num == 0f)
		{
			num = 1f;
		}
	}

	public static void PushScreen(Screen s)
	{
		m_stkScreens.Add(s);
	}

	public static void PopScreen(string s)
	{
		m_stkScreens.Remove(m_stkScreens.Last());
		if (m_stkScreens.Count > 0)
		{
			m_stkScreens.Last().OnRegainFocus(s);
		}
	}

	public static Screen PeekScreen()
	{
		return m_stkScreens.Last();
	}
}
