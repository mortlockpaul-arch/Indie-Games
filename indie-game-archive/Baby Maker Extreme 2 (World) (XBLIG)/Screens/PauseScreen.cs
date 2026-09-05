using System.Collections.Generic;
using System.Linq;
using BabyMakerExtreme2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Renderer;
using Scene;

namespace Screens;

public class PauseScreen : Screen
{
	private delegate void ButtonActivators();

	private List<SineModulatedSprite> m_buttons;

	private List<ButtonActivators> m_buttonActivators;

	private int m_iSelectedIndex;

	private List<string> m_buttonText;

	private SceneContainer m_scene;

	private ParticleRect m_particleRect;

	public PauseScreen(SceneContainer scene)
		: base(updateParent: false, drawParent: true, inputParent: false)
	{
		m_scene = scene;
		m_buttons = new List<SineModulatedSprite>();
		m_buttonActivators = new List<ButtonActivators>();
		m_buttonText = new List<string>();
		for (int i = 0; i < 5; i++)
		{
			SpriteInstance sprite = TextureContainer.GetSprite("images/Buttons/tapeButton", new Vector2(0f, (i - 2) * 100), 100f);
			sprite.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/Buttons/tapeButtonNorm");
			sprite.GetSpriteImage().GetSpritePage().SpecTex = TextureContainer.GetTexture("images/whitesquare");
			m_buttons.Add(new SineModulatedSprite(sprite, 600, 250f, 260f, invertWidthHeight: true));
		}
		m_buttonText.Add("Resume");
		m_buttonActivators.Add(ReturnToGame);
		if (Game1.IsTrial())
		{
			m_buttonText.Add("Purchase");
			m_buttonActivators.Add(PurchaseGame);
			Vector2 vector = SceneRenderer.GetCameraPosition() + new Vector2(0f, -100f) - m_buttons[1].Sprite.SurfaceScale / 2f;
			Vector2 surfaceScale = m_buttons[1].Sprite.SurfaceScale;
			m_particleRect = new ParticleRect(new Rectangle((int)vector.X, (int)vector.Y, (int)surfaceScale.X, (int)surfaceScale.Y), 10, 200, 100f, 200f, m_buttons[0].Sprite.Depth - 1f);
		}
		else
		{
			m_buttons.Remove(m_buttons.Last());
			m_particleRect = null;
		}
		m_buttonText.Add("Restart");
		m_buttonActivators.Add(RestartLevel);
		m_buttonText.Add("High Scores");
		m_buttonActivators.Add(ShowScores);
		m_buttonText.Add("Exit to Main Menu");
		m_buttonActivators.Add(ExitMenu);
		ChangeButton(0, 0);
	}

	public override void Draw(TimeTracker gameTime)
	{
		for (int i = 0; i < m_buttons.Count; i++)
		{
			m_buttons[i].Sprite.Position = SceneRenderer.GetCameraPosition() + new Vector2(0f, (i - 2) * 100);
			m_buttons[i].Draw(gameTime);
			SceneRenderer.DrawStringCentered(fonts.BASE_FONT, m_buttonText[i], m_buttons[i].Sprite.Position, Color.Black, new Vector2(m_buttons[i].Sprite.SurfaceScale.X / 300f, m_buttons[i].Sprite.SurfaceScale.Y / 60f), 100f);
		}
	}

	private void ChangeButton(int oldButton, int newButton)
	{
		m_iSelectedIndex = newButton;
		if (newButton < 0)
		{
			m_iSelectedIndex = m_buttons.Count - 1;
		}
		if (newButton >= m_buttons.Count)
		{
			m_iSelectedIndex = 0;
		}
		newButton = m_iSelectedIndex;
		m_buttons[oldButton].SetNewWidths(250f, 260f);
		m_buttons[oldButton].Sprite.Color = Color.White;
		m_buttons[newButton].SetNewWidths(300f, 350f);
		m_buttons[newButton].Sprite.Color = Color.LimeGreen;
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (ControlManager.PressedDown(ControlManager.ActiveMenuIndex))
		{
			ChangeButton(m_iSelectedIndex, m_iSelectedIndex + 1);
		}
		if (ControlManager.PressedUp(ControlManager.ActiveMenuIndex))
		{
			ChangeButton(m_iSelectedIndex, m_iSelectedIndex - 1);
		}
		if (ControlManager.PressedActivate(ControlManager.ActiveMenuIndex))
		{
			m_buttonActivators[m_iSelectedIndex]();
		}
		if (ControlManager.PressedStart(ControlManager.ActiveMenuIndex) || ControlManager.PressedBackButton(ControlManager.ActiveMenuIndex) || ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.B))
		{
			ReturnToGame();
		}
	}

	private void ReturnToGame()
	{
		ScreenStorage.PopScreen("Unpause");
	}

	private void PurchaseGame()
	{
		Game1.ShowPurchaseScreen(ControlManager.ActiveMenuIndex);
	}

	private void RestartLevel()
	{
		SaveManager.AddDist(m_scene.GetPlayer().DistanceTravelled);
		ScreenStorage.PopScreen("Reset");
	}

	private void ExitMenu()
	{
		SaveManager.AddDist(m_scene.GetPlayer().DistanceTravelled);
		ScreenStorage.PopScreen("ExitMenu");
	}

	private void ShowScores()
	{
		int num = 0;
		if (m_scene.GetSceneObjectSpawner().IsWorldInf())
		{
			num += 1 + m_scene.GetSceneObjectSpawner().GetWorldType();
		}
		new HighScoreList(num, -1);
	}

	public override void Update(TimeTracker gameTime)
	{
		if (m_buttonActivators.Contains(PurchaseGame) && !Game1.IsTrial())
		{
			ChangeButton(m_iSelectedIndex, 0);
			int index = m_buttonActivators.IndexOf(PurchaseGame);
			m_buttons.Remove(m_buttons.Last());
			m_buttonActivators.RemoveAt(index);
			m_buttonText.RemoveAt(index);
		}
		for (int i = 0; i < m_buttons.Count; i++)
		{
			if (m_buttonActivators[m_iSelectedIndex] == new ButtonActivators(PurchaseGame))
			{
				m_particleRect.Update(gameTime);
			}
			m_buttons[i].Update(gameTime);
		}
	}
}
