using System;
using System.Collections.Generic;
using System.Linq;
using BabyMakerExtreme2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Renderer;

namespace Screens;

public class HighScoreList : Screen
{
	private List<string> m_names;

	private List<string> m_scores;

	private List<string> m_types;

	private List<SpriteInstance> m_sprites;

	private SpriteInstance m_bg;

	private SpriteInstance m_bg2;

	private RenderLight m_light;

	private int m_iLightTimer;

	private SpriteInstance m_nextPageButton;

	private string m_nextPageText;

	public HighScoreList(int scoreMode, int highlightIndex)
		: base(updateParent: false, drawParent: false, inputParent: false)
	{
		m_bg = TextureContainer.GetSprite("images/bg", SceneRenderer.GetCameraPosition(), 0f);
		m_bg.Color = Color.Gold;
		m_bg.SurfaceScale = SceneRenderer.GetScreenDim();
		m_bg2 = TextureContainer.GetSprite("images/score", SceneRenderer.GetCameraPosition(), 0.1f);
		m_bg2.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/scoreNorm");
		m_bg2.WidthScale = 750f;
		m_sprites = new List<SpriteInstance>();
		TextureContainer.GetPage("images/scoreScreen/scoreListing").NormTex = TextureContainer.GetTexture("images/scoreScreen/scoreListingNorm");
		TextureContainer.GetPage("images/scoreScreen/scoreListing").SpecTex = TextureContainer.GetTexture("images/whitesquare");
		for (int i = 0; i < 20; i++)
		{
			m_sprites.Add(TextureContainer.GetSprite("images/scoreScreen/scoreListing", SceneRenderer.GetCameraPosition() - new Vector2(0f, 280 - 28 * i), 1f));
			m_sprites.Last().SurfaceScale = new Vector2(m_sprites.Last().WidthScale, 27f);
			if (i == highlightIndex)
			{
				m_sprites[i].Color = Color.Red;
				m_sprites[i].Depth++;
				m_sprites[i].Additive = true;
			}
		}
		m_names = new List<string>();
		m_scores = new List<string>();
		m_types = new List<string>();
		optionSet savedData = SaveManager.GetSavedData();
		m_names = savedData.HighScoreNames[scoreMode];
		for (int j = 0; j < savedData.HighScores[scoreMode].Count; j++)
		{
			m_scores.Add(string.Concat(savedData.HighScores[scoreMode][j]));
			m_types.Add(MasterOfUnlocking.GetPowerupName(savedData.HighScoresBabyTypes[scoreMode][j]));
		}
		m_light = new RenderLight(new Vector3(-1000f, 0f, 1000f), 0f, 1400, new Color(1f, 1f, 0.7f));
		m_iLightTimer = 0;
		m_nextPageButton = TextureContainer.GetSprite("images/Buttons/abxy", new Rectangle(50, 47, 50, 47), m_bg.Position + new Vector2(270f, 280f), m_bg.Depth + 2f);
		m_nextPageButton.SurfaceScale *= 0.65f;
		m_nextPageText = "Go Back";
	}

	public override void Draw(TimeTracker gameTime)
	{
		m_light.Draw(gameTime);
		m_bg.Draw(gameTime);
		m_bg2.Draw(gameTime);
		for (int i = 0; i < m_names.Count; i++)
		{
			m_sprites[i].Draw(gameTime);
			Vector2 vector = m_sprites[i].Position - new Vector2(m_sprites[i].WidthScale / 2f, 0f);
			SceneRenderer.DrawString(fonts.BASE_FONT, m_names[i], vector + new Vector2(20f, -16f), Color.Black, 1f);
			SceneRenderer.DrawString(fonts.BASE_FONT, m_types[i], vector + new Vector2(300f, -16f), Color.Black, 1f);
			SceneRenderer.DrawString(fonts.BASE_FONT, m_scores[i], vector + new Vector2(700f, -16f), Color.Black, 1f);
		}
		m_nextPageButton.Draw(gameTime);
		SceneRenderer.DrawString(fonts.BASE_FONT, m_nextPageText, m_nextPageButton.Position + new Vector2(30f, -15f), Color.Black, 100f);
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (ControlManager.PressedActivate(ControlManager.ActiveMenuIndex) || ControlManager.PressedBackButton(ControlManager.ActiveMenuIndex) || ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.B))
		{
			ScreenStorage.PopScreen("");
		}
	}

	public override void Update(TimeTracker gameTime)
	{
		m_light.pos = new Vector3(SceneRenderer.GetCameraPosition().X, 0f - SceneRenderer.GetCameraPosition().Y, 1000f);
		m_iLightTimer += gameTime.ElapsedMilli;
		m_light.pos += 1200f * new Vector3((float)Math.Sin((float)m_iLightTimer / 500f), (float)Math.Cos((float)m_iLightTimer / 500f), 0f);
	}
}
