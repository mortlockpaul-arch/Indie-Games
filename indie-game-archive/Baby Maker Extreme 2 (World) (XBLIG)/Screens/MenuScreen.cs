using System;
using System.Collections.Generic;
using System.Linq;
using BabyMakerExtreme2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using PlayObjects;
using Renderer;

namespace Screens;

internal class MenuScreen : Screen
{
	private delegate void ButtonActivators();

	private List<ButtonActivators> m_buttonActivators;

	private List<SineModulatedSprite> m_buttons;

	private List<string> m_buttonText;

	private List<ButtonActivators> m_buttonActivatorsBase;

	private List<SineModulatedSprite> m_buttonsBase;

	private List<string> m_buttonTextBase;

	private List<ButtonActivators> m_buttonActivatorsStart;

	private List<SineModulatedSprite> m_buttonsStart;

	private List<string> m_buttonTextStart;

	private List<ButtonActivators> m_buttonActivatorsOthers;

	private List<SineModulatedSprite> m_buttonsOthers;

	private List<string> m_buttonTextOthers;

	private int m_iSelectedIndex;

	private Player m_player;

	private TransitionHelper m_transition;

	private bool m_bTransition;

	private bool m_bRepeatMode;

	private int m_iStartLevel;

	private SineModulatedSprite m_ovum;

	private AnimatedRenderSprite m_cursor;

	private bool m_bActivate;

	private float m_fCursorPerc;

	private bool m_bIsInSubMenuStart;

	private bool m_bIsInSubMenuOther;

	private SoundEffect m_click1;

	private SoundEffect m_click2;

	public MenuScreen(Player player)
		: base(updateParent: true, drawParent: true, inputParent: false)
	{
		SceneRenderer.SetEffect(0);
		m_iStartLevel = 0;
		m_bRepeatMode = false;
		m_player = player;
		SpriteInstance sprite = TextureContainer.GetSprite("images/mainMenu/ovum", SceneRenderer.GetCameraPosition(), 99f);
		sprite.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/mainMenu/ovum2Norm");
		TextureContainer.GetPage("images/mainMenu/ovum").SpecTex = TextureContainer.GetTexture("images/whitesquare");
		sprite.WidthScale = 1200f;
		sprite.Position += new Vector2(450f, 50f);
		sprite.Alpha = 0f;
		m_ovum = new SineModulatedSprite(sprite, 3000, sprite.WidthScale, sprite.WidthScale * 1.05f, invertWidthHeight: false);
		m_bActivate = false;
		SetupLists();
		TextureContainer.GetPage("images/mainMenu/swimmer").NormTex = TextureContainer.GetTexture("images/mainMenu/swimmerNorm");
		TextureContainer.GetPage("images/mainMenu/swimmer").SpecTex = TextureContainer.GetTexture("images/whitesquare");
		m_cursor = new AnimatedRenderSprite("images/mainMenu/swimmer", 0.05f, repeats: true, new Rectangle(0, 0, 128, 40), 2, 5, 98f);
		m_cursor.Position = SceneRenderer.GetCameraPosition() - new Vector2(400f, -200f);
		m_cursor.Rotation = 0f;
		m_fCursorPerc = 1.3f;
		ChangeButton(0, 0, 1);
		m_transition = new TransitionHelper();
		m_transition.TransitionTime = 2000;
		m_transition.TransitionIn();
		m_bTransition = false;
		m_bIsInSubMenuStart = false;
		m_bIsInSubMenuOther = false;
		m_click1 = SoundManager.GetSoundEffect("sounds/click3");
		m_click2 = SoundManager.GetSoundEffect("sounds/click2");
	}

	private void SetupLists()
	{
		m_buttonsBase = new List<SineModulatedSprite>();
		m_buttonActivatorsBase = new List<ButtonActivators>();
		m_buttonTextBase = new List<string>();
		m_buttonsStart = new List<SineModulatedSprite>();
		m_buttonActivatorsStart = new List<ButtonActivators>();
		m_buttonTextStart = new List<string>();
		m_buttonsOthers = new List<SineModulatedSprite>();
		m_buttonActivatorsOthers = new List<ButtonActivators>();
		m_buttonTextOthers = new List<string>();
		m_buttonTextBase.Add("Play");
		m_buttonActivatorsBase.Add(ReturnToGame);
		m_buttonTextBase.Add("Infinite\nModes");
		m_buttonActivatorsBase.Add(ReturnToGame);
		m_buttonTextBase.Add("Outfits");
		m_buttonActivatorsBase.Add(SpawnOutfitScreen);
		if (Game1.IsTrial())
		{
			m_buttonTextBase.Add("Purchase");
			m_buttonActivatorsBase.Add(PurchaseGame);
		}
		m_buttonTextBase.Add("Credits");
		m_buttonActivatorsBase.Add(ShowCredits);
		m_buttonTextBase.Add("Exit");
		m_buttonActivatorsBase.Add(ExitGame);
		optionSet savedData = SaveManager.GetSavedData();
		m_buttonTextStart.Add("Start in Hospital");
		m_buttonActivatorsStart.Add(ReturnToGame);
		m_buttonTextStart.Add("Start in Park");
		if (!savedData.ModeUnlocks[2])
		{
			m_buttonTextStart[1] += " (Locked)";
		}
		m_buttonActivatorsStart.Add(ReturnToGame);
		m_buttonTextStart.Add("Start in Mall");
		if (!savedData.ModeUnlocks[4])
		{
			m_buttonTextStart[2] += " (Locked)";
		}
		m_buttonActivatorsStart.Add(ReturnToGame);
		m_buttonTextOthers.Add("Infinite Hospital");
		if (!savedData.ModeUnlocks[1])
		{
			m_buttonTextOthers[0] += " (Locked)";
		}
		m_buttonActivatorsOthers.Add(ReturnToGame);
		m_buttonTextOthers.Add("Infinite Park");
		if (!savedData.ModeUnlocks[3])
		{
			m_buttonTextOthers[1] += " (Locked)";
		}
		m_buttonActivatorsOthers.Add(ReturnToGame);
		m_buttonTextOthers.Add("Infinite Mall");
		if (!savedData.ModeUnlocks[5])
		{
			m_buttonTextOthers[2] += " (Locked)";
		}
		m_buttonActivatorsOthers.Add(ReturnToGame);
		m_buttonTextOthers.Add("Virtual Baby Maker");
		if (!savedData.ModeUnlocks[6])
		{
			m_buttonTextOthers[3] += " (Locked)";
		}
		m_buttonActivatorsOthers.Add(ReturnToGame);
		InitButtons(m_buttonTextBase, m_buttonsBase);
		InitButtons(m_buttonTextStart, m_buttonsStart);
		InitButtons(m_buttonTextOthers, m_buttonsOthers);
		m_buttonActivators = m_buttonActivatorsBase;
		m_buttons = m_buttonsBase;
		m_buttonText = m_buttonTextBase;
	}

	private void InitButtons(List<string> texts, List<SineModulatedSprite> list)
	{
		for (int i = 0; i < texts.Count; i++)
		{
			SpriteInstance sprite = TextureContainer.GetSprite("images/whitesquare", new Vector2(-800 - i * 300, ((float)i - 2.5f) * 100f), 90f);
			list.Add(new SineModulatedSprite(sprite, 200, 30f, 30f, invertWidthHeight: false));
		}
	}

	private void ChangeButton(int oldButton, int newButton, int dir)
	{
		if (m_buttonText[m_iSelectedIndex].Contains("Locked"))
		{
			return;
		}
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
		while (m_buttonText[m_iSelectedIndex].Contains("Locked"))
		{
			m_iSelectedIndex += dir;
			newButton = m_iSelectedIndex;
			if (newButton < 0)
			{
				m_iSelectedIndex = m_buttons.Count - 1;
			}
			if (newButton >= m_buttons.Count)
			{
				m_iSelectedIndex = 0;
			}
			newButton = m_iSelectedIndex;
		}
		m_buttons[oldButton].SetNewWidths(30f, 30f);
		m_buttons[oldButton].Sprite.Color = Color.White;
		m_buttons[newButton].SetNewWidths(70f, 70f);
		m_buttons[newButton].Sprite.Color = Color.LimeGreen;
		m_cursor.Position = GetPointInOvum(newButton, m_buttons.Count, 0f, m_fCursorPerc);
		m_cursor.Rotation = 0f - GetMenuAngle(newButton, m_buttons.Count);
	}

	private void PurchaseGame()
	{
		Game1.ShowPurchaseScreen(ControlManager.ActiveMenuIndex);
	}

	private void ReturnToGame()
	{
		if (!SaveManager.GetSavedData().ModeUnlocks[1] || m_bIsInSubMenuStart || m_bIsInSubMenuOther)
		{
			if (m_bIsInSubMenuStart || m_bIsInSubMenuOther)
			{
				m_iStartLevel = m_iSelectedIndex;
			}
			else
			{
				m_iStartLevel = 0;
			}
			if (m_bIsInSubMenuOther)
			{
				m_bRepeatMode = true;
			}
			else
			{
				m_bRepeatMode = false;
			}
			ScreenStorage.PopScreen("Start");
			if (ScreenStorage.PeekScreen() is GameScreen)
			{
				((GameScreen)ScreenStorage.PeekScreen()).SetLevelAndRepeat(m_iStartLevel, m_bRepeatMode);
				((GameScreen)ScreenStorage.PeekScreen()).Reset();
			}
		}
	}

	private void ExitGame()
	{
		Game1.ExitGame();
	}

	private void SpawnOutfitScreen()
	{
		new OutfitScreen(m_player);
	}

	private void ShowCredits()
	{
		new CreditScreen();
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (m_bActivate || m_bTransition)
		{
			return;
		}
		if (ControlManager.PressedDown(ControlManager.ActiveMenuIndex))
		{
			int iSelectedIndex = m_iSelectedIndex;
			ChangeButton(m_iSelectedIndex, m_iSelectedIndex + 1, 1);
			if (iSelectedIndex != m_iSelectedIndex)
			{
				SoundManager.AddSoundToPlay(m_click1, 1f, 0f, 0);
			}
		}
		if (ControlManager.PressedUp(ControlManager.ActiveMenuIndex))
		{
			int iSelectedIndex2 = m_iSelectedIndex;
			ChangeButton(m_iSelectedIndex, m_iSelectedIndex - 1, -1);
			if (iSelectedIndex2 != m_iSelectedIndex)
			{
				SoundManager.AddSoundToPlay(m_click1, 1f, 0f, 0);
			}
		}
		if (!m_buttonText[m_iSelectedIndex].Contains("Locked") && ControlManager.PressedActivate(ControlManager.ActiveMenuIndex))
		{
			if (m_iSelectedIndex == 0 && !m_bIsInSubMenuOther && !m_bIsInSubMenuStart && SaveManager.GetSavedData().ModeUnlocks[1])
			{
				SoundManager.AddSoundToPlay(m_click2, 1f, 0f, 0);
				m_bIsInSubMenuStart = true;
				ChangeButton(m_iSelectedIndex, 0, 1);
				m_buttons = m_buttonsStart;
				m_buttonActivators = m_buttonActivatorsStart;
				m_buttonText = m_buttonTextStart;
				ChangeButton(m_iSelectedIndex, 0, 1);
			}
			else if (m_iSelectedIndex == 1 && !m_bIsInSubMenuOther && !m_bIsInSubMenuStart)
			{
				SoundManager.AddSoundToPlay(m_click2, 1f, 0f, 0);
				m_bIsInSubMenuOther = true;
				ChangeButton(m_iSelectedIndex, 0, 1);
				m_buttons = m_buttonsOthers;
				m_buttonActivators = m_buttonActivatorsOthers;
				m_buttonText = m_buttonTextOthers;
				ChangeButton(m_iSelectedIndex, 0, 1);
			}
			else if (m_bIsInSubMenuStart)
			{
				optionSet savedData = SaveManager.GetSavedData();
				if (m_iSelectedIndex == 0)
				{
					m_bActivate = savedData.ModeUnlocks[0];
				}
				if (m_iSelectedIndex == 1)
				{
					m_bActivate = savedData.ModeUnlocks[2];
				}
				if (m_iSelectedIndex == 2)
				{
					m_bActivate = savedData.ModeUnlocks[4];
				}
				if (m_bActivate)
				{
					SoundManager.AddSoundToPlay(m_click2, 1f, 0f, 0);
				}
				if (Game1.IsTrial() && m_iSelectedIndex > 0)
				{
					m_bActivate = false;
					List<SpriteInstance> list = new List<SpriteInstance>();
					list.Add(TextureContainer.GetSprite("images/upsells/upsell2", default(Vector2), 200f));
					list.Last().Position = SceneRenderer.GetCameraPosition() - new Vector2(0f, 130f);
					list.Last().FlatColor = true;
					list.Last().Rotation = 0.14f;
					new UpsellScreen(isGame: false, list, 0, gotFar: false);
				}
			}
			else if (m_bIsInSubMenuOther)
			{
				optionSet savedData2 = SaveManager.GetSavedData();
				if (m_iSelectedIndex == 0)
				{
					m_bActivate = savedData2.ModeUnlocks[1];
				}
				if (m_iSelectedIndex == 1)
				{
					m_bActivate = savedData2.ModeUnlocks[3];
				}
				if (m_iSelectedIndex == 2)
				{
					m_bActivate = savedData2.ModeUnlocks[5];
				}
				if (m_iSelectedIndex == 3)
				{
					m_bActivate = savedData2.ModeUnlocks[6];
				}
				if (m_bActivate)
				{
					SoundManager.AddSoundToPlay(m_click2, 1f, 0f, 0);
				}
				if (Game1.IsTrial())
				{
					m_bActivate = false;
					List<SpriteInstance> list2 = new List<SpriteInstance>();
					list2.Add(TextureContainer.GetSprite("images/upsells/upsell3", default(Vector2), 200f));
					list2.Last().Position = SceneRenderer.GetCameraPosition() - new Vector2(0f, 60f);
					list2.Last().FlatColor = true;
					new UpsellScreen(isGame: false, list2, 0, gotFar: false);
				}
			}
			else
			{
				SoundManager.AddSoundToPlay(m_click2, 1f, 0f, 0);
				m_bActivate = true;
			}
		}
		if ((ControlManager.PressedBackButton(ControlManager.ActiveMenuIndex) || ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.B)) && (m_bIsInSubMenuOther || m_bIsInSubMenuStart))
		{
			m_bIsInSubMenuOther = false;
			m_bIsInSubMenuStart = false;
			SoundManager.AddSoundToPlay(m_click2, 1f, 0.5f, 0);
			ChangeButton(m_iSelectedIndex, 0, 1);
			m_buttons = m_buttonsBase;
			m_buttonActivators = m_buttonActivatorsBase;
			m_buttonText = m_buttonTextBase;
			ChangeButton(m_iSelectedIndex, 0, 1);
		}
	}

	private Vector2 GetPointInOvum(int index1, int maxIndexes, float heightMod, float percentDist)
	{
		float num = percentDist * m_ovum.Sprite.WidthScale / 2f;
		float menuAngle = GetMenuAngle(index1, maxIndexes);
		Vector2 vector = num * new Vector2(0f - (float)Math.Cos(menuAngle), (float)Math.Sin(menuAngle));
		Vector2 vector2 = vector;
		vector2.Normalize();
		vector2 = new Vector2(0f - vector2.Y, vector2.X);
		vector2 *= heightMod;
		return m_ovum.Sprite.Position + vector + vector2;
	}

	private float GetMenuAngle(int index1, int maxIndexes)
	{
		return 1.013417f * (((float)index1 - (float)maxIndexes / 2f) / (float)maxIndexes);
	}

	public override void Draw(TimeTracker gameTime)
	{
		Color black = Color.Black;
		black.A = (byte)(255f * (1f - m_transition.Alpha));
		m_cursor.Draw(gameTime);
		m_ovum.Draw(gameTime);
		for (int i = 0; i < m_buttons.Count; i++)
		{
			float num = m_buttons[i].Sprite.SurfaceScale.X / 70f;
			float num2 = 0f;
			if (m_buttonText[i].Contains('\n'))
			{
				num2 = 30f;
			}
			SceneRenderer.DrawString(fonts.BUTTON_FONT, m_buttonText[i], GetPointInOvum(i, m_buttons.Count, num2 + 42f * num, 0.9f), black, 1.4f * new Vector2(num), isScreenSpace: false, 0f - GetMenuAngle(i, m_buttons.Count), 101f);
		}
		m_transition.Draw(gameTime);
	}

	public override void Update(TimeTracker gameTime)
	{
		m_ovum.Update(gameTime);
		m_cursor.Alpha += gameTime.FractionOfSecond * 2f;
		m_ovum.Sprite.Alpha += gameTime.FractionOfSecond * 2f;
		if (m_ovum.Sprite.Alpha > 0.9f)
		{
			m_ovum.Sprite.Alpha = 0.9f;
			m_cursor.Alpha = 0.9f;
		}
		if (m_buttonActivators.Contains(PurchaseGame) && !Game1.IsTrial())
		{
			ChangeButton(m_iSelectedIndex, 0, 1);
			int index = m_buttonActivators.IndexOf(PurchaseGame);
			m_buttons.Remove(m_buttons.Last());
			m_buttonText.RemoveAt(index);
			m_buttonActivators.RemoveAt(index);
		}
		m_cursor.Update(gameTime);
		for (int i = 0; i < m_buttons.Count; i++)
		{
			m_buttons[i].Update(gameTime);
			if (m_buttons[i].Sprite.Position.X < SceneRenderer.GetCameraPosition().X)
			{
				m_buttons[i].Sprite.Position += new Vector2(gameTime.FractionOfSecond * 1500f, 0f);
			}
		}
		if (m_bActivate)
		{
			m_fCursorPerc -= gameTime.FractionOfSecond;
			m_cursor.Position = GetPointInOvum(m_iSelectedIndex, m_buttons.Count, 0f, m_fCursorPerc);
			if (m_fCursorPerc < 0.9f)
			{
				m_bTransition = true;
				m_fCursorPerc = 1.3f;
				m_bActivate = false;
			}
		}
		if (m_bTransition)
		{
			m_transition.Update(gameTime);
			if (m_transition.IsTransitionedOut)
			{
				m_buttonActivators[m_iSelectedIndex]();
				m_bTransition = false;
				m_transition.TransitionIn();
			}
		}
	}

	public override void OnRegainFocus(string applicatorInfo)
	{
		m_cursor.Position = GetPointInOvum(m_iSelectedIndex, m_buttons.Count, 0f, m_fCursorPerc);
		m_ovum.Sprite.Alpha = 0f;
		m_cursor.Alpha = 0f;
	}
}
