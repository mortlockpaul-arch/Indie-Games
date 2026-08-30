using System;
using System.Collections.Generic;
using System.Linq;
using BabyMaker;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MusicPlayer;
using PlayerData;
using Renderer;

namespace Screens;

public class PauseScreen : Screen
{
	public delegate void ActivateEvent();

	private const int PARTICLE_SPAWN_TIME = 30;

	private List<ActivateEvent> m_buttonEvents;

	private List<string> m_buttonText;

	private string m_pauseText;

	private string m_tipsText;

	private string m_outfitsText;

	private int m_iIndex;

	private bool m_bTips;

	private List<string> m_tips;

	private RenderSprite m_pauseBg;

	private int m_iTipsOffset;

	private bool m_bIsTrial;

	private int m_iParticleTimer;

	private List<RenderSprite> m_outfitSprites;

	private List<RenderSprite> m_propDecor;

	private bool m_bChangeOutfit;

	private Player m_player;

	private List<bool> m_enabledDecor;

	private int m_iOutfitIndex;

	private List<string> m_outfitPieceNames;

	private List<ActivateEvent> m_outfitActivators;

	private List<string> m_outfitPieceButtons;

	private List<RenderSprite> m_otherGames;

	private int m_iOtherGame;

	private bool m_bOtherGames;

	private TransitionHelper m_trans;

	private bool m_bReset;

	public PauseScreen(Player p)
		: base(updateParent: false, drawParent: true, inputParent: false)
	{
		m_bReset = false;
		m_player = p;
		m_iParticleTimer = 0;
		m_pauseBg = SpriteManager.GetSprite("images/pausebg", default(Vector2), DepthConsts.PAUSE_DEPTH - 1f);
		m_pauseText = "Paused";
		m_tipsText = "Hints & Tips";
		m_outfitsText = "Outfits";
		m_buttonEvents = new List<ActivateEvent>();
		m_buttonText = new List<string>();
		m_iIndex = 0;
		m_buttonText.Add("Resume Game");
		m_buttonEvents.Add(Unpause);
		if (Game1.IsTrial())
		{
			m_bIsTrial = true;
			m_buttonText.Add("Purchase Game");
			m_buttonEvents.Add(Purchase);
		}
		else
		{
			m_bIsTrial = false;
		}
		m_buttonText.Add("Restart Level");
		m_buttonEvents.Add(Restart);
		m_buttonText.Add("Outfits");
		m_buttonEvents.Add(ChangeOutfit);
		m_buttonText.Add("Hints & Tips");
		m_buttonEvents.Add(Tips);
		m_buttonText.Add("Music Volume: " + (int)Math.Round(Mp3MusicPlayer.Volume * 10f) * 10 + "%");
		m_buttonEvents.Add(ChangeMusic);
		m_buttonText.Add("SFX Volume: " + (int)Math.Round(Game1.SFXVolume * 10f) * 10 + "%");
		m_buttonEvents.Add(ChangeSound);
		m_buttonText.Add("Credits");
		m_buttonEvents.Add(Credits);
		m_buttonText.Add("Exit Game");
		m_buttonEvents.Add(Exit);
		m_buttonText.Add("Other Games");
		m_buttonEvents.Add(OtherGames);
		m_bTips = false;
		m_bChangeOutfit = false;
		m_tips = new List<string>();
		for (int i = 0; i < 9; i++)
		{
			m_tips.Add(ScoreScreen.GenerateTip(null, i));
		}
		m_iTipsOffset = 0;
		m_iOutfitIndex = 0;
		m_outfitSprites = CreateOutfit();
		m_otherGames = new List<RenderSprite>();
		m_otherGames.Add(SpriteManager.GetSprite("images/upsells/MMMUpsell", default(Vector2), DepthConsts.PAUSE_DEPTH + 10f));
		m_otherGames.Add(SpriteManager.GetSprite("images/upsells/tankUpsell", default(Vector2), DepthConsts.PAUSE_DEPTH + 10f));
		m_iOtherGame = 0;
		m_bOtherGames = false;
		m_trans = new TransitionHelper();
	}

	public List<RenderSprite> CreateOutfit()
	{
		List<RenderSprite> list = new List<RenderSprite>();
		list.Add(SpriteManager.GetSprite("images/bodypartsUpper", new Rectangle(164, 1495, 100, 106), default(Vector2), DepthConsts.PAUSE_DEPTH + 0.02f));
		list.Last().Origin = new Vector2(9.6f, 0f);
		list.Add(SpriteManager.GetSprite("images/bodypartsUpper", new Rectangle(67, 1405, 82, 84), default(Vector2), DepthConsts.PAUSE_DEPTH + 0.03f));
		list.Last().Origin = new Vector2(11.2f, 51.19999f);
		list.Add(SpriteManager.GetSprite("images/bodypartsUpper", new Rectangle(163, 1404, 83, 89), default(Vector2), DepthConsts.PAUSE_DEPTH + 0.04f));
		list.Last().Origin = new Vector2(-22.4f, -9.6f);
		list.Add(SpriteManager.GetSprite("images/bodypartsUpper", new Rectangle(55, 1499, 92, 76), default(Vector2), DepthConsts.PAUSE_DEPTH + 0.05f));
		list.Last().Origin = new Vector2(8f, 8f);
		list.Add(SpriteManager.GetSprite("images/bodypartsUpper", new Rectangle(161, 1405, 83, 87), default(Vector2), DepthConsts.PAUSE_DEPTH + 0.01f));
		list.Last().Origin = new Vector2(-16f, -3.2f);
		m_propDecor = new List<RenderSprite>();
		m_enabledDecor = m_player.GetEnabledDecor();
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(25, 5, 82, 84), default(Vector2), list[1].Depth + 0.005f));
		m_propDecor.Last().Origin = list[1].Origin;
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(296, 5, 82, 84), default(Vector2), list[1].Depth + 0.005f));
		m_propDecor.Last().Origin = list[1].Origin;
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(112, 5, 82, 84), default(Vector2), list[1].Depth + 0.004f));
		m_propDecor.Last().Origin = list[1].Origin;
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(204, 5, 82, 84), default(Vector2), list[1].Depth + 0.006f));
		m_propDecor.Last().Origin = list[1].Origin;
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(13, 99, 92, 76), default(Vector2), list[3].Depth + 0.001f));
		m_propDecor.Last().Origin = list[3].Origin;
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(13, 181, 92, 76), default(Vector2), list[3].Depth + 0.001f));
		m_propDecor.Last().Origin = list[3].Origin;
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(122, 95, 100, 106), default(Vector2), list[0].Depth + 0.001f));
		m_propDecor.Last().Origin = list[0].Origin;
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(122, 211, 100, 106), default(Vector2), list[0].Depth + 0.001f));
		m_propDecor.Last().Origin = list[0].Origin;
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(230, 105, 83, 89), default(Vector2), list[2].Depth + 0.001f));
		m_propDecor.Last().Origin = list[2].Origin;
		m_propDecor.Add(SpriteManager.GetSprite("images/babyProps", new Rectangle(228, 106, 83, 87), default(Vector2), list[4].Depth + 0.001f));
		m_propDecor.Last().Origin = list[4].Origin;
		m_outfitPieceNames = new List<string>();
		m_outfitActivators = new List<ActivateEvent>();
		m_outfitPieceButtons = new List<string>();
		m_outfitPieceNames.Add("Avatar Mode");
		m_outfitActivators.Add(SetAvatar);
		m_outfitPieceNames.Add("Diaper");
		m_outfitActivators.Add(SetDiaper);
		m_outfitPieceNames.Add("Shoes");
		m_outfitActivators.Add(SetShoes);
		m_outfitPieceNames.Add("Vest");
		m_outfitActivators.Add(SetVest);
		m_outfitPieceNames.Add("Gloves");
		m_outfitActivators.Add(SetGloves);
		m_outfitPieceNames.Add("Glasses");
		m_outfitActivators.Add(SetGlasses);
		m_outfitPieceNames.Add("Helmet");
		m_outfitActivators.Add(SetHelmet);
		m_outfitPieceNames.Add("Tattoo");
		m_outfitActivators.Add(SetTattoo);
		m_outfitPieceNames.Add("Scuba");
		m_outfitActivators.Add(SetScuba);
		for (int i = 0; i < m_outfitPieceNames.Count; i++)
		{
			m_outfitPieceButtons.Add("");
			m_iOutfitIndex = i;
			if (i != 0)
			{
				m_outfitActivators[i]();
				m_outfitActivators[i]();
			}
		}
		if (m_player.Avatar)
		{
			m_outfitPieceButtons[0] = "X";
		}
		else
		{
			m_outfitPieceButtons[0] = "";
		}
		m_iOutfitIndex = 0;
		return list;
	}

	public void SetAvatar()
	{
		m_player.Avatar = !m_player.Avatar;
		if (m_player.Avatar)
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "X";
		}
		else
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "";
		}
	}

	public void SetDiaper()
	{
		m_enabledDecor[6] = !m_enabledDecor[6];
		if (m_enabledDecor[6])
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "X";
		}
		else
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "";
		}
	}

	public void SetShoes()
	{
		m_enabledDecor[8] = !m_enabledDecor[8];
		m_enabledDecor[9] = !m_enabledDecor[9];
		if (m_enabledDecor[8])
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "X";
		}
		else
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "";
		}
	}

	public void SetVest()
	{
		m_enabledDecor[7] = !m_enabledDecor[7];
		if (m_enabledDecor[7])
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "X";
		}
		else
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "";
		}
	}

	public void SetGloves()
	{
		m_enabledDecor[5] = !m_enabledDecor[5];
		if (m_enabledDecor[5])
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "X";
		}
		else
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "";
		}
	}

	public void SetHelmet()
	{
		if (m_enabledDecor[0] || m_enabledDecor[1])
		{
			m_enabledDecor[0] = false;
			m_enabledDecor[1] = false;
		}
		else if (m_player.GetSex().Equals("boy"))
		{
			m_enabledDecor[0] = true;
		}
		else
		{
			m_enabledDecor[1] = true;
		}
		if (m_enabledDecor[0] || m_enabledDecor[1])
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "X";
		}
		else
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "";
		}
	}

	public void SetTattoo()
	{
		m_enabledDecor[4] = !m_enabledDecor[4];
		if (m_enabledDecor[4])
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "X";
		}
		else
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "";
		}
	}

	public void SetGlasses()
	{
		m_enabledDecor[2] = !m_enabledDecor[2];
		if (m_enabledDecor[2])
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "X";
		}
		else
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "";
		}
	}

	public void SetScuba()
	{
		m_enabledDecor[3] = !m_enabledDecor[3];
		if (m_enabledDecor[3])
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "X";
		}
		else
		{
			m_outfitPieceButtons[m_iOutfitIndex] = "";
		}
	}

	public void ChangeOutfit()
	{
		m_bChangeOutfit = true;
		m_iOutfitIndex = 0;
	}

	public void Unpause()
	{
		Game1.PopScreen("");
	}

	public void Purchase()
	{
		Game1.ShowPurchaseScreen(ControlManager.ActiveMenuIndex);
	}

	public void Credits()
	{
		new CreditScreen();
	}

	public void Restart()
	{
		if (!m_bReset)
		{
			m_trans.TransitionIn();
			Mp3MusicPlayer.FadeOut(0.1f);
			m_bReset = true;
		}
	}

	public void Tips()
	{
		m_bTips = true;
		m_iTipsOffset = 0;
	}

	public void ChangeMusic()
	{
		if (Mp3MusicPlayer.Volume == 1f)
		{
			Mp3MusicPlayer.Volume = 0f;
		}
		else
		{
			Mp3MusicPlayer.Volume = (float)Math.Round(Mp3MusicPlayer.Volume * 10f) / 10f + 0.1f;
		}
		m_buttonText[m_iIndex] = "Music Volume: " + (int)Math.Round(Mp3MusicPlayer.Volume * 10f) * 10 + "%";
	}

	public void ChangeSound()
	{
		if (Game1.SFXVolume == 1f)
		{
			Game1.SFXVolume = 0f;
		}
		else
		{
			Game1.SFXVolume = (float)Math.Round(Game1.SFXVolume * 10f) / 10f + 0.1f;
		}
		m_buttonText[m_iIndex] = "SFX Volume: " + (int)Math.Round(Game1.SFXVolume * 10f) * 10 + "%";
	}

	public void Exit()
	{
		Game1.ExitGame();
	}

	public void OtherGames()
	{
		m_iOtherGame = 0;
		m_bOtherGames = true;
	}

	public override void Draw(TimeTracker gameTime)
	{
		m_pauseBg.Position = SceneRenderer.GetCameraPosition();
		m_pauseBg.Draw(gameTime);
		if (!m_bTips && !m_bChangeOutfit && !m_bOtherGames)
		{
			SceneRenderer.DrawString(fonts.BUTTON_FONT, m_pauseText, SceneRenderer.GetCameraPosition() + new Vector2((0f - SceneRenderer.GetStringWidth(fonts.BUTTON_FONT, m_pauseText)) / 2f, -260f), Color.Black, DepthConsts.PAUSE_DEPTH);
			for (int i = 0; i < m_buttonText.Count; i++)
			{
				Color c = Color.Black;
				if (i == m_iIndex)
				{
					c = Color.Red;
					c.A = 100;
					SceneRenderer.DrawString(fonts.BASE_FONT, m_buttonText[i], SceneRenderer.GetCameraPosition() + new Vector2((0f - SceneRenderer.GetStringWidth(fonts.BASE_FONT, m_buttonText[i])) / 2f, -180 + i * 45) + new Vector2(SceneRenderer.GetRand(-5f, 5f), SceneRenderer.GetRand(-5f, 5f)), c, DepthConsts.PAUSE_DEPTH);
					c = Color.Red;
				}
				SceneRenderer.DrawString(fonts.BASE_FONT, m_buttonText[i], SceneRenderer.GetCameraPosition() + new Vector2((0f - SceneRenderer.GetStringWidth(fonts.BASE_FONT, m_buttonText[i])) / 2f, -180 + i * 45), c, DepthConsts.PAUSE_DEPTH);
			}
		}
		else if (m_bTips)
		{
			SceneRenderer.DrawString(fonts.BUTTON_FONT, m_tipsText, SceneRenderer.GetCameraPosition() + new Vector2((0f - SceneRenderer.GetStringWidth(fonts.BUTTON_FONT, m_tipsText)) / 2f, -260f), Color.Black, DepthConsts.PAUSE_DEPTH);
			for (int j = 0; j < 9; j++)
			{
				if (j - m_iTipsOffset > 0 && j - m_iTipsOffset < 4)
				{
					SceneRenderer.DrawString(fonts.BASE_FONT, m_tips[j], SceneRenderer.GetCameraPosition() + new Vector2(-250f, -260 + (j - m_iTipsOffset) * 120), Color.Black, new Vector2(0.8f), DepthConsts.PAUSE_DEPTH);
				}
				SceneRenderer.DrawString(fonts.BASE_FONT, "< >", SceneRenderer.GetCameraPosition() + new Vector2(170f, 200f), Color.Black, DepthConsts.PAUSE_DEPTH);
			}
		}
		else if (m_bChangeOutfit)
		{
			SceneRenderer.DrawString(fonts.BUTTON_FONT, m_outfitsText, SceneRenderer.GetCameraPosition() + new Vector2((0f - SceneRenderer.GetStringWidth(fonts.BUTTON_FONT, m_outfitsText)) / 2f, -260f), Color.Black, DepthConsts.PAUSE_DEPTH);
			if (m_player.Avatar)
			{
				AvatarHandler avatar = Game1.GetAvatar();
				avatar.ShouldDraw = true;
				avatar.SetRotations(-1f, 0f, 0.5f, 0f, new Vector2(-100f, 0f), DepthConsts.PAUSE_DEPTH);
			}
			else
			{
				for (int k = 0; k < m_outfitSprites.Count; k++)
				{
					m_outfitSprites[k].Position = m_pauseBg.Position + new Vector2(100f, 0f);
					m_outfitSprites[k].Draw(gameTime);
				}
				for (int l = 0; l < m_propDecor.Count; l++)
				{
					if (m_enabledDecor[l])
					{
						m_propDecor[l].Position = m_pauseBg.Position + new Vector2(100f, 0f);
						m_propDecor[l].Draw(gameTime);
					}
				}
			}
			for (int m = 0; m < m_outfitPieceNames.Count; m++)
			{
				Color c2 = Color.Black;
				if (m_player.Avatar)
				{
					c2 = Color.Gray;
				}
				if (m == m_iOutfitIndex)
				{
					c2 = Color.Red;
				}
				SceneRenderer.DrawString(fonts.BASE_FONT, m_outfitPieceNames[m], m_pauseBg.Position + new Vector2(-200f, -200 + m * 40), c2, DepthConsts.PAUSE_DEPTH);
				SceneRenderer.DrawString(fonts.BASE_FONT, m_outfitPieceButtons[m], m_pauseBg.Position + new Vector2(-240f, -200 + m * 40), c2, DepthConsts.PAUSE_DEPTH);
			}
		}
		else if (m_bOtherGames)
		{
			m_otherGames[m_iOtherGame].Position = SceneRenderer.GetCameraPosition();
			m_otherGames[m_iOtherGame].Draw(gameTime);
		}
		m_trans.Draw(gameTime);
	}

	public override void Update(TimeTracker gameTime)
	{
		m_trans.Update(gameTime);
		if (m_bIsTrial)
		{
			if (!Game1.IsTrial())
			{
				m_bIsTrial = false;
				m_buttonText.RemoveAt(1);
				m_buttonEvents.RemoveAt(1);
				if (m_iIndex > 0)
				{
					m_iIndex--;
				}
			}
			if (m_iIndex == 1)
			{
				m_iParticleTimer += gameTime.ElapsedMilli;
				while (m_iParticleTimer > 30)
				{
					m_iParticleTimer -= 30;
					float stringWidth = SceneRenderer.GetStringWidth(fonts.BASE_FONT, m_buttonText[1]);
					float rand = SceneRenderer.GetRand(-3.14f, 3.14f);
					Color color = new Color(SceneRenderer.GetRand(0f, 1f), SceneRenderer.GetRand(0f, 1f), SceneRenderer.GetRand(0f, 1f));
					ParticleManager.GetParticle().Initialize(GenericParticleImgs.GetParticle(), SceneRenderer.GetCameraPosition() + new Vector2(SceneRenderer.GetRand((0f - stringWidth) / 2f, stringWidth / 2f), -130f), DepthConsts.PAUSE_DEPTH - 0.0001f, (int)SceneRenderer.GetRand(200f, 800f), SceneRenderer.GetRand(100f, 200f) * new Vector2((float)Math.Cos(rand), (float)Math.Sin(rand)), fadesOut: true, color, color, 20f, 40f, additive: false, new Vector2(0f, 0f));
				}
			}
		}
		if (m_trans.IsTransitionedIn)
		{
			Game1.PopScreen("Restart");
			Game1.SaveGlobalOptions();
		}
		if (!m_bChangeOutfit)
		{
			Game1.GetAvatar()?.SetRotations(-1f, 0f, 0.5f, 0f, new Vector2(-100f, 0f), DepthConsts.BABY_DEPTH);
		}
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (m_bReset)
		{
			return;
		}
		if (!m_bTips && !m_bChangeOutfit && !m_bOtherGames)
		{
			if (ControlManager.PressedActivate(ControlManager.ActiveMenuIndex))
			{
				m_buttonEvents[m_iIndex]();
			}
			else if (ControlManager.PressedUp(ControlManager.ActiveMenuIndex))
			{
				m_iIndex--;
				if (m_iIndex < 0)
				{
					m_iIndex = m_buttonText.Count - 1;
				}
			}
			else if (ControlManager.PressedDown(ControlManager.ActiveMenuIndex))
			{
				m_iIndex++;
				if (m_iIndex >= m_buttonText.Count)
				{
					m_iIndex = 0;
				}
			}
			else if (ControlManager.PressedBackButton(ControlManager.ActiveMenuIndex) || ControlManager.PressedStart(ControlManager.ActiveMenuIndex) || ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.B))
			{
				Unpause();
			}
		}
		else if (m_bTips)
		{
			if (ControlManager.PressedBackButton(ControlManager.ActiveMenuIndex) || ControlManager.PressedStart(ControlManager.ActiveMenuIndex) || ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.B))
			{
				m_bTips = false;
			}
			if (ControlManager.PressedDown(ControlManager.ActiveMenuIndex) || ControlManager.PressedRight(ControlManager.ActiveMenuIndex) || ControlManager.PressedActivate(ControlManager.ActiveMenuIndex))
			{
				m_iTipsOffset++;
				if (m_iTipsOffset >= 9)
				{
					m_iTipsOffset = 8;
				}
			}
			else if (ControlManager.PressedUp(ControlManager.ActiveMenuIndex) || ControlManager.PressedLeft(ControlManager.ActiveMenuIndex))
			{
				m_iTipsOffset--;
				if (m_iTipsOffset < 0)
				{
					m_iTipsOffset = 0;
				}
			}
		}
		else if (m_bChangeOutfit)
		{
			if (ControlManager.PressedActivate(ControlManager.ActiveMenuIndex))
			{
				m_outfitActivators[m_iOutfitIndex]();
			}
			else if (ControlManager.PressedBackButton(ControlManager.ActiveMenuIndex) || ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.B) || ControlManager.PressedStart(ControlManager.ActiveMenuIndex))
			{
				m_bChangeOutfit = !m_bChangeOutfit;
			}
			if (ControlManager.PressedDown(ControlManager.ActiveMenuIndex) && !m_player.Avatar)
			{
				m_iOutfitIndex++;
				if (m_iOutfitIndex >= m_outfitPieceNames.Count)
				{
					m_iOutfitIndex = 0;
				}
			}
			if (ControlManager.PressedUp(ControlManager.ActiveMenuIndex) && !m_player.Avatar)
			{
				m_iOutfitIndex--;
				if (m_iOutfitIndex < 0)
				{
					m_iOutfitIndex = m_outfitPieceNames.Count - 1;
				}
			}
		}
		else
		{
			if (!m_bOtherGames)
			{
				return;
			}
			if (ControlManager.PressedLeft(ControlManager.ActiveMenuIndex) || ControlManager.PressedUp(ControlManager.ActiveMenuIndex))
			{
				m_iOtherGame--;
				if (m_iOtherGame < 0)
				{
					m_iOtherGame = m_otherGames.Count - 1;
				}
			}
			if (ControlManager.PressedRight(ControlManager.ActiveMenuIndex) || ControlManager.PressedActivate(ControlManager.ActiveMenuIndex) || ControlManager.PressedDown(ControlManager.ActiveMenuIndex))
			{
				m_iOtherGame++;
				if (m_iOtherGame >= m_otherGames.Count)
				{
					m_iOtherGame = 0;
				}
			}
			if (ControlManager.PressedBackButton(ControlManager.ActiveMenuIndex) || ControlManager.PressedStart(ControlManager.ActiveMenuIndex) || ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.B))
			{
				m_bOtherGames = false;
			}
		}
	}
}
