using System.Collections.Generic;
using System.Linq;
using BabyMakerExtreme2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PlayObjects;
using Renderer;
using Scene;

namespace Screens;

public class ScoreScreen : Screen
{
	private Player m_player;

	private SpriteInstance m_bg;

	private SpriteInstance m_bg2;

	private SpriteInstance m_bgLocale;

	private List<string> m_text;

	private string m_scoreString;

	private string m_distString;

	private List<SpriteInstance> m_sprites;

	private List<string> m_counters;

	private int m_iPage;

	private int m_iSelectedIndex;

	private List<string> m_selectionText;

	private bool m_bIsTrial;

	private SpriteInstance m_nextPageButton;

	private string m_nextPageText;

	private SpriteInstance m_shareButton;

	private string m_shareText;

	private bool m_bCanSendMsg;

	private int m_iDist;

	private int m_iScore;

	private SceneContainer m_scene;

	private int m_iHighScoreIndex;

	private SpriteInstance m_projectileDys;

	private bool m_bSelectLocale;

	private bool m_bCanSelectLocale;

	private List<string> m_LocaleText;

	private int m_iLocaleIndex;

	private List<int> m_iSelectLevelIndex;

	private List<bool> m_bSelectLevelRepeats;

	private ParticleRect m_particleRect;

	public ScoreScreen(Player player, SceneContainer scene)
		: base(updateParent: false, drawParent: true, inputParent: false)
	{
		PlayBabySound();
		m_scene = scene;
		m_iSelectedIndex = 0;
		m_player = player;
		m_text = new List<string>();
		GenerateText(player.DistanceTravelled);
		m_iDist = player.DistanceTravelled;
		m_iScore = player.GetScore();
		m_scoreString = "Final Score:" + m_iScore + " points";
		m_distString = "Distance Travelled: " + m_iDist + " feet";
		m_bg = TextureContainer.GetSprite("images/score", SceneRenderer.GetCameraPosition() - new Vector2(150f, 0f), 100f);
		m_bg.GetSpriteImage().GetSpritePage().NormTex = TextureContainer.GetTexture("images/scoreNorm");
		m_bg.WidthScale = 640f;
		m_bg2 = (SpriteInstance)m_bg.Clone();
		m_bg2.SurfaceScale = new Vector2(300f, 300f);
		m_bg2.Position = SceneRenderer.GetCameraPosition() + new Vector2(310f, 100f);
		m_bg2.Depth = m_bg.Depth + 0.1f;
		m_bgLocale = (SpriteInstance)m_bg.Clone();
		m_bgLocale.SurfaceScale = new Vector2(300f, 300f);
		m_bgLocale.Position = SceneRenderer.GetCameraPosition() + new Vector2(310f, 100f);
		m_bgLocale.Depth = m_bg.Depth + 0.1f;
		List<int> scoreCounters = player.GetScoreCounters();
		m_sprites = new List<SpriteInstance>();
		m_counters = new List<string>();
		m_iPage = 0;
		int num = 0;
		for (int i = 0; i < scoreCounters.Count; i++)
		{
			if (scoreCounters[i] > 0)
			{
				m_sprites.Add(AwardPopup.GetSpriteType((PropType)i));
				m_sprites.Last().WidthScale *= 0.5f;
				m_counters.Add("x" + scoreCounters[i]);
				m_sprites.Last().Position = m_bg.Position - new Vector2(180f, 150f) + new Vector2(num % 3 * 180, num / 3 % 3 * 160);
				m_sprites.Last().Depth = m_bg.Depth + 1f;
				m_sprites.Last().Alpha = 1f;
				num++;
			}
		}
		m_selectionText = new List<string>();
		m_selectionText.Add("Restart");
		if (Game1.IsTrial())
		{
			m_bIsTrial = true;
			m_selectionText.Add("Purchase");
		}
		else
		{
			m_bIsTrial = false;
			m_bg2.SurfaceScale = new Vector2(300f, 250f);
		}
		m_selectionText.Add("Change Outfit");
		if (MasterOfUnlocking.IsModeAvail(1) && !m_bIsTrial)
		{
			m_selectionText.Add("Change Locale");
			m_bCanSelectLocale = true;
			m_bg2.SurfaceScale = new Vector2(300f, 300f);
		}
		else
		{
			m_bCanSelectLocale = false;
		}
		m_selectionText.Add("High Scores");
		m_selectionText.Add("Main Menu");
		m_nextPageButton = TextureContainer.GetSprite("images/Buttons/abxy", new Rectangle(50, 0, 50, 47), m_bg.Position + new Vector2(-250f, 260f), m_bg.Depth + 1f);
		m_nextPageButton.SurfaceScale *= 0.65f;
		m_nextPageText = "Next Page";
		m_shareButton = TextureContainer.GetSprite("images/Buttons/abxy", new Rectangle(0, 0, 50, 47), m_bg.Position + new Vector2(-250f, 292f), m_bg.Depth + 1f);
		m_shareButton.SurfaceScale *= 0.65f;
		m_shareText = "Share";
		m_bCanSendMsg = Game1.CanSendMessageToFriend();
		m_iHighScoreIndex = SaveManager.AddScore(Game1.GetPlayerName(), player.GetScore(), (int)m_player.BabyType, m_scene.GetSceneObjectSpawner().GetWorldType(), m_scene.GetSceneObjectSpawner().IsWorldInf());
		SaveManager.AddDist(player.DistanceTravelled);
		if (m_sprites.Count == 0)
		{
			m_projectileDys = TextureContainer.GetSprite("images/projectileDys", m_bg.Position, m_bg.Depth + 1f);
			m_projectileDys.FlatColor = true;
		}
		else
		{
			m_projectileDys = null;
		}
		m_bSelectLocale = false;
		m_LocaleText = new List<string>();
		m_iSelectLevelIndex = new List<int>();
		m_bSelectLevelRepeats = new List<bool>();
		if (MasterOfUnlocking.IsModeAvail(0))
		{
			m_LocaleText.Add("Start in Hospital");
			m_iSelectLevelIndex.Add(0);
			m_bSelectLevelRepeats.Add(item: false);
		}
		if (MasterOfUnlocking.IsModeAvail(2))
		{
			m_LocaleText.Add("Start in Park");
			m_iSelectLevelIndex.Add(1);
			m_bSelectLevelRepeats.Add(item: false);
		}
		if (MasterOfUnlocking.IsModeAvail(4))
		{
			m_LocaleText.Add("Start in Mall");
			m_iSelectLevelIndex.Add(2);
			m_bSelectLevelRepeats.Add(item: false);
		}
		if (MasterOfUnlocking.IsModeAvail(6))
		{
			m_LocaleText.Add("Virtual Baby Maker");
			m_iSelectLevelIndex.Add(3);
			m_bSelectLevelRepeats.Add(item: true);
		}
		if (MasterOfUnlocking.IsModeAvail(1))
		{
			m_LocaleText.Add("Infinite Hospital");
			m_iSelectLevelIndex.Add(0);
			m_bSelectLevelRepeats.Add(item: true);
		}
		if (MasterOfUnlocking.IsModeAvail(3))
		{
			m_LocaleText.Add("Infinite Park");
			m_iSelectLevelIndex.Add(1);
			m_bSelectLevelRepeats.Add(item: true);
		}
		if (MasterOfUnlocking.IsModeAvail(5))
		{
			m_LocaleText.Add("Infinite Mall");
			m_iSelectLevelIndex.Add(2);
			m_bSelectLevelRepeats.Add(item: true);
		}
		m_LocaleText.Add("Go Back");
		m_iLocaleIndex = 0;
		m_bgLocale.SurfaceScale = new Vector2(300f, 50 + 50 * m_LocaleText.Count);
		m_bgLocale.Position = SceneRenderer.GetCameraPosition() + new Vector2(310f, 0f);
		Vector2 vector = m_bg2.Position - m_bg2.SurfaceScale / 2f + new Vector2(20f, 70f);
		m_particleRect = new ParticleRect(new Rectangle((int)vector.X, (int)vector.Y, 140, 50), 10, 100, 100f, 200f, m_bg.Depth + 2f);
	}

	private void PlayBabySound()
	{
		SoundManager.AddSoundToPlay((int)SceneRenderer.GetRand(0f, 11f) switch
		{
			0 => SoundManager.GetSoundEffect("sounds/freesound/babies/14264__pfly__babybabblebit01"), 
			1 => SoundManager.GetSoundEffect("sounds/freesound/babies/18275__Corsica_S__baby_giggle"), 
			2 => SoundManager.GetSoundEffect("sounds/freesound/babies/47370__reinsamba__baby_laugh1"), 
			3 => SoundManager.GetSoundEffect("sounds/freesound/babies/47371__reinsamba__baby_laugh2"), 
			4 => SoundManager.GetSoundEffect("sounds/freesound/babies/47372__reinsamba__baby_laugh3"), 
			5 => SoundManager.GetSoundEffect("sounds/freesound/babies/47374__reinsamba__baby_voice15"), 
			6 => SoundManager.GetSoundEffect("sounds/freesound/babies/47375__reinsamba__baby_voice16"), 
			7 => SoundManager.GetSoundEffect("sounds/freesound/babies/59459__Erdie__Lena_laughes03"), 
			8 => SoundManager.GetSoundEffect("sounds/freesound/babies/59460__Erdie__Lena_laughes09"), 
			9 => SoundManager.GetSoundEffect("sounds/freesound/babies/65895__Robinhood76"), 
			10 => SoundManager.GetSoundEffect("sounds/freesound/babies/81211__bennstir__Baby_laugh1"), 
			_ => SoundManager.GetSoundEffect("sounds/freesound/babies/14264__pfly__babybabblebit01"), 
		}, 1f, 0f, 0);
	}

	private void GenerateText(int dist)
	{
		switch ((int)SceneRenderer.GetRand(0f, 5f))
		{
		case 0:
			m_text.Add("Your bouncing baby met the cow and");
			m_text.Add("spoon by the moon and went " + dist + " feet");
			break;
		case 1:
			m_text.Add("Fly baby fly! Your cosmonautical");
			m_text.Add("child flew " + dist + " feet");
			break;
		case 2:
			m_text.Add("What did that mother eat to get that?");
			m_text.Add("She launched that baby " + dist + " feet!");
			break;
		case 3:
			m_text.Add("Your baby shot for the sky, but only");
			m_text.Add("made it " + dist + " feet");
			break;
		default:
			m_text.Add("Your baby flew " + dist + " feet");
			m_text.Add("I have no words for this event");
			break;
		}
	}

	public override void Draw(TimeTracker gameTime)
	{
		m_bg.Draw(gameTime);
		if (m_bSelectLocale)
		{
			m_bgLocale.Draw(gameTime);
		}
		else
		{
			m_bg2.Draw(gameTime);
		}
		for (int i = 0; i < m_text.Count; i++)
		{
			SceneRenderer.DrawStringCentered(fonts.BASE_FONT, m_text[i], m_bg.Position - new Vector2(0f, 280 - i * 30), Color.Black, 100f);
		}
		if (m_bSelectLocale)
		{
			for (int j = 0; j < m_LocaleText.Count; j++)
			{
				Color color = Color.Black;
				float num = 0.7f;
				if (j == m_iLocaleIndex)
				{
					color = Color.Lime;
					Color c = color;
					c *= 0.4f;
					for (int k = 0; k < 2; k++)
					{
						SceneRenderer.DrawString(fonts.BUTTON_FONT, m_LocaleText[j], m_bgLocale.Position - m_bgLocale.SurfaceScale / 2f + new Vector2(20f, -15f * num + 35f + (float)(50 * j)) + new Vector2(SceneRenderer.GetRand(-3f, 3f), SceneRenderer.GetRand(-5f, 5f)), c, new Vector2(num), 100f);
					}
				}
				SceneRenderer.DrawString(fonts.BUTTON_FONT, m_LocaleText[j], m_bgLocale.Position - m_bgLocale.SurfaceScale / 2f + new Vector2(20f, -15f * num + 35f + (float)(50 * j)), color, new Vector2(num), 100f);
			}
		}
		else
		{
			for (int l = 0; l < m_selectionText.Count; l++)
			{
				Color color2 = Color.Black;
				float num2 = 0.7f;
				if (l == m_iSelectedIndex)
				{
					color2 = Color.Lime;
					num2 = 1f;
					if (m_bIsTrial && m_iSelectedIndex == 1)
					{
						m_particleRect.Update(gameTime);
					}
					Color c2 = color2;
					c2 *= 0.4f;
					for (int m = 0; m < 2; m++)
					{
						SceneRenderer.DrawString(fonts.BUTTON_FONT, m_selectionText[l], m_bg2.Position - m_bg2.SurfaceScale / 2f + new Vector2(20f, -15f * num2 + 35f + (float)(50 * l)) + new Vector2(SceneRenderer.GetRand(-3f, 3f), SceneRenderer.GetRand(-7f, 7f)), c2, new Vector2(num2), 100f);
					}
				}
				SceneRenderer.DrawString(fonts.BUTTON_FONT, m_selectionText[l], m_bg2.Position - m_bg2.SurfaceScale / 2f + new Vector2(20f, -15f * num2 + 35f + (float)(50 * l)), color2, new Vector2(num2), 100f);
			}
		}
		for (int n = 0; n < m_sprites.Count; n++)
		{
			if (n < (m_iPage + 1) * 9 && n >= m_iPage * 9)
			{
				m_sprites[n].Draw(gameTime);
				SceneRenderer.DrawString(fonts.BASE_FONT, m_counters[n], m_sprites[n].Position + new Vector2(70f, -20f), Color.Black, m_sprites[n].Depth + 1f);
			}
		}
		SceneRenderer.DrawString(fonts.BASE_FONT, m_scoreString, m_bg.Position + new Vector2(-50f, 250f), Color.Black, 100f);
		SceneRenderer.DrawString(fonts.BASE_FONT, m_distString, m_bg.Position + new Vector2(-50f, 280f), Color.Black, 100f);
		if (m_sprites.Count > 9)
		{
			m_nextPageButton.Draw(gameTime);
			SceneRenderer.DrawString(fonts.BASE_FONT, m_nextPageText, m_nextPageButton.Position + new Vector2(30f, -15f), Color.Black, 100f);
		}
		if (m_bCanSendMsg)
		{
			m_shareButton.Draw(gameTime);
			SceneRenderer.DrawString(fonts.BASE_FONT, m_shareText, m_shareButton.Position + new Vector2(30f, -15f), Color.Black, 100f);
		}
		if (m_projectileDys != null)
		{
			m_projectileDys.Draw(gameTime);
		}
	}

	public override void Update(TimeTracker gameTime)
	{
		if (m_bIsTrial && !Game1.IsTrial())
		{
			m_bIsTrial = false;
			m_selectionText.Remove("Purchase");
		}
	}

	public string GenerateScoreMessage()
	{
		string text = "!";
		if (m_scene.GetSceneObjectSpawner().IsWorldInf())
		{
			text = " in the ";
			text = m_scene.GetSceneObjectSpawner().GetWorldType() switch
			{
				0 => text + "Infinite Hospital!", 
				1 => text + "Infinite Park!", 
				2 => text + "Infinite Mall!", 
				3 => text + "Virtual Baby Maker!", 
				_ => "!", 
			};
		}
		return "I just launched my " + MasterOfUnlocking.GetPowerupName((int)m_player.GetProp().PropType) + " " + m_iDist + " feet and earned a score of " + m_iScore + " in Baby Maker Extreme 2" + text;
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (m_bSelectLocale)
		{
			if (ControlManager.PressedUp(ControlManager.ActiveMenuIndex))
			{
				m_iLocaleIndex--;
				if (m_iLocaleIndex < 0)
				{
					m_iLocaleIndex = m_LocaleText.Count - 1;
				}
			}
			else if (ControlManager.PressedDown(ControlManager.ActiveMenuIndex))
			{
				m_iLocaleIndex++;
				if (m_iLocaleIndex >= m_LocaleText.Count)
				{
					m_iLocaleIndex = 0;
				}
			}
		}
		else if (ControlManager.PressedUp(ControlManager.ActiveMenuIndex))
		{
			m_iSelectedIndex--;
			if (m_iSelectedIndex < 0)
			{
				m_iSelectedIndex = m_selectionText.Count - 1;
			}
		}
		else if (ControlManager.PressedDown(ControlManager.ActiveMenuIndex))
		{
			m_iSelectedIndex++;
			if (m_iSelectedIndex >= m_selectionText.Count)
			{
				m_iSelectedIndex = 0;
			}
		}
		if (m_bCanSendMsg && ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.B))
		{
			Game1.SendMessageToFriend(GenerateScoreMessage());
		}
		if (ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.X))
		{
			m_iPage++;
			if ((float)m_iPage >= (float)m_sprites.Count / 9f)
			{
				m_iPage = 0;
			}
		}
		if (m_bSelectLocale)
		{
			if (ControlManager.PressedActivate(ControlManager.ActiveMenuIndex))
			{
				if (m_iLocaleIndex == m_LocaleText.Count - 1)
				{
					m_bSelectLocale = false;
					return;
				}
				ScreenStorage.PopScreen("ResetSave");
				m_scene.SetInfiniteWorld(m_bSelectLevelRepeats[m_iLocaleIndex]);
				m_scene.SetDefaultWorld(m_iSelectLevelIndex[m_iLocaleIndex]);
			}
		}
		else
		{
			if (!ControlManager.PressedActivate(ControlManager.ActiveMenuIndex))
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			if (!m_bIsTrial)
			{
				num = -1;
			}
			if (!m_bCanSelectLocale)
			{
				num2 = -1;
			}
			if (m_iSelectedIndex == 0)
			{
				ScreenStorage.PopScreen("ResetSave");
			}
			else if (m_iSelectedIndex == 1 + num)
			{
				Game1.ShowPurchaseScreen(ControlManager.ActiveMenuIndex);
			}
			else if (m_iSelectedIndex == 2 + num)
			{
				new OutfitScreen(m_player);
			}
			else if (m_iSelectedIndex == 3 + num + num2)
			{
				m_bSelectLocale = true;
				m_iLocaleIndex = 0;
			}
			else if (m_iSelectedIndex == 4 + num + num2)
			{
				int num3 = 0;
				if (m_scene.GetSceneObjectSpawner().IsWorldInf())
				{
					num3 += 1 + m_scene.GetSceneObjectSpawner().GetWorldType();
				}
				new HighScoreList(num3, m_iHighScoreIndex);
			}
			else if (m_iSelectedIndex == 5 + num + num2)
			{
				ScreenStorage.PopScreen("ExitMenu");
			}
		}
	}
}
