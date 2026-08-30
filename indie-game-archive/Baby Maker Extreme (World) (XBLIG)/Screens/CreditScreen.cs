using System;
using System.Collections.Generic;
using BabyMaker;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Renderer;

namespace Screens;

public class CreditScreen : Screen
{
	private List<string> m_textBase;

	private List<string> m_textMusic;

	private List<string> m_textFreeSound;

	private float m_fOffset;

	private float m_fFadeTimer;

	private RenderSprite m_bg;

	private bool m_bExit;

	private float m_fExitPos;

	public CreditScreen()
		: base(updateParent: false, drawParent: true, inputParent: false)
	{
		m_bg = SpriteManager.GetSprite("images/whitesquare", default(Vector2), DepthConsts.FADE_DEPTH);
		m_bg.Color = Color.Black;
		m_bg.SurfaceScale = new Vector2(1280f, 1280f);
		m_fOffset = 0f;
		m_fFadeTimer = 0f;
		m_bExit = false;
		m_textBase = new List<string>();
		m_textBase.Add("Design and Development");
		m_textBase.Add("Daniel Steger");
		m_textBase.Add("www.stegersaurus.com");
		m_textMusic = new List<string>();
		m_textMusic.Add("Music");
		m_textMusic.Add("Kevin MacLeod");
		m_textMusic.Add("www.incompetech.com");
		m_textMusic.Add("Licensed under Creative Commons \"Attribution 3.0\"");
		m_textMusic.Add("creativecommons.org/licenses/by/3.0/");
		m_textFreeSound = new List<string>();
		m_textFreeSound.Add("Cover Photograph");
		m_textFreeSound.Add("From Meagan Jean via flickr");
		m_textFreeSound.Add("www.flickr.com/people/meaganjean/");
		m_textFreeSound.Add("www.meaganjeanphotography.com");
		m_textFreeSound.Add("Licensed under Creative Commons \"Attribution 2.0 Generic\"");
		m_textFreeSound.Add("creativecommons.org/licenses/by/2.0/deed.en_CA");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("Freesound.org SFX Samples");
		m_textFreeSound.Add("Licensed under Creative Commons \"Sampling Plus 1.0\"");
		m_textFreeSound.Add("creativecommons.org/licenses/sampling+/1.0/");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("from Robinhood76 (userid: 321967)");
		m_textFreeSound.Add("00767 baby laugh 2 (id: 65895)");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("from bennstir (userid: 16052)");
		m_textFreeSound.Add("Baby laugh1 (id: 81211)");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("from pfly (userid: 33854)");
		m_textFreeSound.Add("babybabblebit01 (id: 14264)");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("from reinsamba (userid: 18799)");
		m_textFreeSound.Add("baby_voice16 (id: 47375)");
		m_textFreeSound.Add("baby_voice15 (id: 47374)");
		m_textFreeSound.Add("baby_laugh2 (id: 47371)");
		m_textFreeSound.Add("baby_laugh3 (id: 47372)");
		m_textFreeSound.Add("baby_laugh1 (id: 47370)");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("from Corsica_S (userid: 7037)");
		m_textFreeSound.Add("baby_giggle (id: 65929)");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("from Erdie (userid: 118241)");
		m_textFreeSound.Add("Lena-laughes09 (id: 59460)");
		m_textFreeSound.Add("Lena-laughes03 (id: 59459)");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("from dobroide (userid: 8043)");
		m_textFreeSound.Add("20090102.street.noise (id: 65929)");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("from Halleck (userid: 6281)");
		m_textFreeSound.Add("curtain_call-16 (id: 18666)");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("from hanstimm (userid: 96)");
		m_textFreeSound.Add("z1 (id: 7805)");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("from pitx (userid: 40665)");
		m_textFreeSound.Add("Grito 07 (id: 15921)");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("from SpeedY (userid: 6479)");
		m_textFreeSound.Add("Dist_Chr_Gmin_2strs_12th (id: 11790)");
		m_textFreeSound.Add("Dist_Chr_Dmin_2strs_12th (id: 11782)");
		m_textFreeSound.Add("Dist_Chr_D&G_strs_12th (id: 11780)");
		m_textFreeSound.Add("Dist_Chr_B&E_strs_12th (id: 11770)");
		m_textFreeSound.Add("Dist_Chr_Amj_2strs_12th_up (id: 11767)");
		m_textFreeSound.Add("Dist_Chr_A_oct (id: 11925)");
		m_textFreeSound.Add("Dist_Chr_A5th_up (id: 11923)");
		m_textFreeSound.Add("Dist_PickSlideup (id: 11806)");
		m_textFreeSound.Add("");
		m_textFreeSound.Add("from sleep (userid: 838)");
		m_textFreeSound.Add("GTRBND (id: 928)");
		m_textFreeSound.Add("string_whip (id: 1408)");
		m_textFreeSound.Add("");
		m_fExitPos = 5000f;
	}

	public override void Draw(TimeTracker gameTime)
	{
		m_bg.Position = SceneRenderer.GetCameraPosition();
		m_bg.Draw(gameTime);
		Color white = Color.White;
		white.A = (byte)(255f * m_bg.Alpha);
		float num = 400f;
		for (int i = 0; i < m_textBase.Count; i++)
		{
			SceneRenderer.DrawString(fonts.ITEM_COUNT_FONT, m_textBase[i], SceneRenderer.GetCameraPosition() + new Vector2((0f - SceneRenderer.GetStringWidth(fonts.ITEM_COUNT_FONT, m_textBase[i])) / 2f, 0f - m_fOffset + num), white, m_bg.Depth + 1f);
			num += 70f;
		}
		num += 40f;
		for (int j = 0; j < m_textMusic.Count; j++)
		{
			SceneRenderer.DrawString(fonts.CASH_COUNT_FONT, m_textMusic[j], SceneRenderer.GetCameraPosition() + new Vector2((0f - SceneRenderer.GetStringWidth(fonts.CASH_COUNT_FONT, m_textMusic[j])) / 2f, 0f - m_fOffset + num), white, m_bg.Depth + 1f);
			num += 40f;
		}
		num += 40f;
		for (int k = 0; k < m_textFreeSound.Count; k++)
		{
			SceneRenderer.DrawString(fonts.CASH_COUNT_FONT, m_textFreeSound[k], SceneRenderer.GetCameraPosition() + new Vector2((0f - SceneRenderer.GetStringWidth(fonts.CASH_COUNT_FONT, m_textFreeSound[k])) / 2f, 0f - m_fOffset + num), white, m_bg.Depth + 1f);
			num += 40f;
		}
		m_fExitPos = num + 720f;
	}

	public override void Update(TimeTracker gameTime)
	{
		m_fOffset += gameTime.FractionOfSecond * 60f;
		if (m_fOffset > m_fExitPos && !m_bExit)
		{
			m_bExit = true;
			m_fFadeTimer = 1f;
		}
		if (m_bExit)
		{
			m_fFadeTimer -= gameTime.FractionOfSecond;
		}
		else
		{
			m_fFadeTimer += gameTime.FractionOfSecond;
		}
		m_bg.Alpha = Math.Min(Math.Max(m_fFadeTimer, 0f), 1f);
		if (m_fFadeTimer < 0f)
		{
			Game1.PopScreen("");
		}
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (ControlManager.PressedBackButton(ControlManager.ActiveMenuIndex) || ControlManager.PressedButton(ControlManager.ActiveMenuIndex, Buttons.B))
		{
			m_bExit = true;
			m_fFadeTimer = 1f;
		}
	}
}
