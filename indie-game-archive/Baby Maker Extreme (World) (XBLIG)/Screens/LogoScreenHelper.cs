using System;
using System.Collections.Generic;
using System.Linq;
using BabyMaker;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using MusicPlayer;
using PlayerData;
using Renderer;

namespace Screens;

internal class LogoScreenHelper
{
	private const int COMPLETE_TIME = 2500;

	private const int SHAKE_STOP = 1000;

	private RenderSprite m_logo1;

	private RenderSprite m_pressA;

	private AnimatedRenderSprite m_logo2;

	private int m_iCompleteTimer;

	private Vector2 logoPos2;

	private SoundEffect m_eActivateSound;

	private Vector2 m_startScale;

	private float m_fTimer;

	private List<RenderSprite> m_outfitSprites;

	private Player m_player;

	private bool m_bIsComplete;

	private RenderSprite m_getReady;

	private float m_fReadyTimer;

	private RenderSprite m_chooseOne;

	private RenderSprite m_babyMode;

	private RenderSprite m_avatarMode;

	public bool IsComplete
	{
		get
		{
			if (m_bIsComplete)
			{
				return (double)m_fReadyTimer > Math.PI;
			}
			return false;
		}
	}

	public LogoScreenHelper(Player p)
	{
		m_bIsComplete = false;
		m_player = p;
		m_iCompleteTimer = 0;
		Vector2 pos = SceneRenderer.GetScreenDim() / 2f - new Vector2(0f, 100f);
		logoPos2 = SceneRenderer.GetScreenDim() / 2f + new Vector2(0f, 100f);
		m_logo1 = SpriteManager.GetSprite("images/babyMaker", pos, DepthConsts.LOGO_DEPTH);
		m_pressA = SpriteManager.GetSprite("images/pressA", logoPos2, DepthConsts.LOGO_DEPTH + 1f);
		m_startScale = m_logo1.SurfaceScale;
		List<RenderSprite> list = new List<RenderSprite>();
		List<float> list2 = new List<float>();
		list.Add(SpriteManager.GetSprite("images/extremeText", logoPos2, DepthConsts.LOGO_DEPTH + 1f));
		list2.Add(0.2f);
		m_logo2 = new AnimatedRenderSprite(list, list2, repeats: true);
		m_logo2.Alpha = 0f;
		m_eActivateSound = SoundManager.GetSoundEffect("sounds/freesound/string_whip_GTRBND_MIX");
		m_fTimer = 0f;
		GenerateOutfit();
		m_getReady = SpriteManager.GetSprite("images/getReady", default(Vector2), DepthConsts.LOGO_DEPTH);
		m_getReady.Alpha = 0f;
		m_fReadyTimer = 0f;
		m_chooseOne = SpriteManager.GetSprite("images/chooseOneScreen/chooseOne", default(Vector2), DepthConsts.LOGO_DEPTH - 1f);
		m_babyMode = SpriteManager.GetSprite("images/chooseOneScreen/babyMode", default(Vector2), DepthConsts.LOGO_DEPTH - 1f);
		m_avatarMode = SpriteManager.GetSprite("images/chooseOneScreen/avatarMode", default(Vector2), DepthConsts.LOGO_DEPTH - 1f);
		m_chooseOne.Alpha = 0f;
		m_babyMode.Alpha = 0f;
		m_avatarMode.Alpha = 0f;
	}

	public void GenerateOutfit()
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
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Alpha = 0f;
		}
		m_outfitSprites = list;
	}

	public void Draw(TimeTracker gameTime)
	{
		if (m_iCompleteTimer <= 2500)
		{
			m_logo1.Draw(gameTime);
			m_pressA.Draw(gameTime);
			m_logo2.Draw(gameTime);
			return;
		}
		m_chooseOne.Draw(gameTime);
		m_avatarMode.Draw(gameTime);
		m_babyMode.Draw(gameTime);
		for (int i = 0; i < m_outfitSprites.Count; i++)
		{
			m_outfitSprites[i].Draw(gameTime);
		}
		AvatarHandler avatar = Game1.GetAvatar();
		if (avatar != null)
		{
			avatar.ShouldDraw = true;
			avatar.SetRotations(-1f, 0f, 0.5f, 0f, new Vector2(-200f - m_fReadyTimer * 1000f, -200f), DepthConsts.PAUSE_DEPTH);
		}
		if (m_bIsComplete)
		{
			m_getReady.Draw(gameTime);
		}
	}

	public void HandleInput(TimeTracker gameTime)
	{
		if (m_iCompleteTimer <= 2500)
		{
			if (ControlManager.ActiveMenuIndex >= 0)
			{
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (ControlManager.PressedActivate(i) || ControlManager.PressedStart(i))
				{
					ControlManager.ActiveMenuIndex = i;
					Mp3MusicPlayer.FadeOut(1f);
					SoundManager.AddSoundToPlay(m_eActivateSound, 1f, 0f, 0);
				}
			}
		}
		else if (ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.X))
		{
			m_player.Avatar = false;
			m_bIsComplete = true;
		}
		else if (ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.B))
		{
			m_player.Avatar = true;
			m_bIsComplete = true;
		}
	}

	public void Update(TimeTracker gameTime)
	{
		if (m_iCompleteTimer <= 2500)
		{
			m_logo2.Update(gameTime);
			if (ControlManager.ActiveMenuIndex >= 0)
			{
				m_pressA.Alpha -= gameTime.FractionOfSecond * 5f;
				if (m_pressA.Alpha < 0f)
				{
					m_pressA.Alpha = 0f;
				}
				if (m_iCompleteTimer < 2200)
				{
					m_logo2.Alpha += gameTime.FractionOfSecond * 3f;
				}
				else
				{
					m_logo2.Alpha = (float)(2500 - m_iCompleteTimer) / 300f;
					m_logo1.Alpha = m_logo2.Alpha;
					if (m_logo1.Alpha < 0f)
					{
						m_logo1.Alpha = 0f;
					}
				}
				if (m_logo2.Alpha > 1f)
				{
					m_logo2.Alpha = 1f;
				}
				if (m_logo2.Alpha < 0f)
				{
					m_logo2.Alpha = 0f;
				}
				m_iCompleteTimer += gameTime.ElapsedMilli;
				if (m_iCompleteTimer > 2500)
				{
					m_player.Avatar = true;
				}
				m_logo2.Position = logoPos2 + Math.Max(0f, 1f - (float)m_iCompleteTimer / 1000f) * new Vector2(SceneRenderer.GetRand(-30f, 30f), SceneRenderer.GetRand(-30f, 30f));
				ControlManager.SetFlatVibration(ControlManager.ActiveMenuIndex, Math.Max(0f, 1f - (float)m_iCompleteTimer / 1000f));
			}
			m_fTimer += gameTime.FractionOfSecond;
			m_logo1.SurfaceScale = new Vector2((0.9f + 0.1f * (float)Math.Sin(m_fTimer * 3f)) * m_startScale.X, (0.9f + 0.1f * (float)Math.Cos(m_fTimer * 3f)) * m_startScale.Y);
			return;
		}
		m_chooseOne.Position = SceneRenderer.GetCameraPosition() - new Vector2(0f, 200f);
		m_babyMode.Position = SceneRenderer.GetCameraPosition() - new Vector2(200f, 0f);
		m_avatarMode.Position = SceneRenderer.GetCameraPosition() - new Vector2(-200f, 0f);
		if (!m_bIsComplete)
		{
			m_chooseOne.Alpha += gameTime.FractionOfSecond * 6f;
			if (m_chooseOne.Alpha > 1f)
			{
				m_chooseOne.Alpha = 1f;
			}
			m_babyMode.Alpha = m_chooseOne.Alpha;
			m_avatarMode.Alpha = m_chooseOne.Alpha;
		}
		for (int i = 0; i < m_outfitSprites.Count; i++)
		{
			if (!m_bIsComplete)
			{
				m_outfitSprites[i].Alpha = m_chooseOne.Alpha;
			}
			m_outfitSprites[i].Position = SceneRenderer.GetCameraPosition() - new Vector2(200f + m_fReadyTimer * 1000f, -200f);
		}
		if (m_bIsComplete)
		{
			m_fReadyTimer += gameTime.FractionOfSecond / 2f;
			m_getReady.Position = SceneRenderer.GetCameraPosition();
			m_getReady.Alpha = (float)Math.Sin(m_fReadyTimer);
			m_chooseOne.Alpha -= gameTime.FractionOfSecond * 6f;
			if (m_chooseOne.Alpha < 0f)
			{
				m_chooseOne.Alpha = 0f;
			}
			m_babyMode.Alpha = m_chooseOne.Alpha;
			m_avatarMode.Alpha = m_chooseOne.Alpha;
		}
	}
}
