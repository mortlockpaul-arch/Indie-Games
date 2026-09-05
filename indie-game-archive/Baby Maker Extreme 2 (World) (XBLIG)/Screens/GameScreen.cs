using System;
using BabyMakerExtreme2;
using Microsoft.Xna.Framework;
using MusicPlayer;
using PlayObjects;
using Renderer;
using Scene;

namespace Screens;

public class GameScreen : Screen
{
	private SceneContainer m_scene;

	private TransitionHelper m_transition;

	private bool m_bReset;

	private bool m_bSaveGlobalOptions;

	private bool m_bExit;

	private int m_iJustFinishedSave;

	public GameScreen()
		: base(updateParent: false, drawParent: false, inputParent: false)
	{
		SpritePage page;
		for (int i = 0; i < 7; i++)
		{
			page = TextureContainer.GetPage("images/Spritesheets/hospital/sheet" + (i + 1));
			page.NormTex = TextureContainer.GetTexture("images/Spritesheets/hospital/sheet" + (i + 1) + "norm");
			page.SpecTex = TextureContainer.GetTexture("images/whitesquare");
		}
		for (int j = 0; j < 7; j++)
		{
			page = TextureContainer.GetPage("images/Spritesheets/park/sheet" + (j + 1));
			page.NormTex = TextureContainer.GetTexture("images/Spritesheets/park/sheet" + (j + 1) + "norm");
			page.SpecTex = TextureContainer.GetTexture("images/whitesquare");
		}
		for (int k = 0; k < 5; k++)
		{
			page = TextureContainer.GetPage("images/Spritesheets/mall/sheet" + (k + 1));
			page.NormTex = TextureContainer.GetTexture("images/Spritesheets/mall/sheet" + (k + 1) + "norm");
			page.SpecTex = TextureContainer.GetTexture("images/whitesquare");
		}
		page = TextureContainer.GetPage("images/Spritesheets/outfitPieces");
		page.NormTex = TextureContainer.GetTexture("images/Spritesheets/outfitPiecesNorm");
		page.SpecTex = TextureContainer.GetTexture("images/whitesquare");
		page = TextureContainer.GetPage("images/Spritesheets/glassShards");
		page.NormTex = TextureContainer.GetTexture("images/Spritesheets/glassShardsNorm");
		page.SpecTex = TextureContainer.GetTexture("images/whitesquare");
		page = TextureContainer.GetPage("images/Spritesheets/boxes");
		page.NormTex = TextureContainer.GetTexture("images/Spritesheets/boxes");
		page.SpecTex = TextureContainer.GetTexture("images/whitesquare");
		page = TextureContainer.GetPage("images/Buttons/abxy");
		page.NormTex = TextureContainer.GetTexture("images/Buttons/abxyNorm");
		page.SpecTex = TextureContainer.GetTexture("images/whitesquare");
		page = TextureContainer.GetPage("images/Spritesheets/upsellBear");
		page.NormTex = TextureContainer.GetTexture("images/Spritesheets/upsellBearNorm");
		page.SpecTex = TextureContainer.GetTexture("images/whitesquare");
		page = TextureContainer.GetPage("images/awards1");
		page.NormTex = TextureContainer.GetTexture("images/awards1Norm");
		page.SpecTex = TextureContainer.GetTexture("images/whitesquare");
		page = TextureContainer.GetPage("images/awards2");
		page.NormTex = TextureContainer.GetTexture("images/awards2Norm");
		page.SpecTex = TextureContainer.GetTexture("images/whitesquare");
		page = TextureContainer.GetPage("images/awards3");
		page.NormTex = TextureContainer.GetTexture("images/awards3Norm");
		page.SpecTex = TextureContainer.GetTexture("images/whitesquare");
		page = TextureContainer.GetPage("images/awards4");
		page.NormTex = TextureContainer.GetTexture("images/awards4Norm");
		page.SpecTex = TextureContainer.GetTexture("images/whitesquare");
		page = TextureContainer.GetPage("images/particles");
		page.NormTex = TextureContainer.GetTexture("images/particlesNorm");
		page.SpecTex = TextureContainer.GetTexture("images/whitesquare");
		m_scene = new SceneContainer();
		SoundEffects.Initialize();
		m_transition = new TransitionHelper();
		m_transition.TransitionOut();
		m_bReset = false;
		m_bSaveGlobalOptions = false;
		m_bExit = false;
		m_iJustFinishedSave = 0;
		Mp3MusicPlayer.Initialize("sounds/incompetech/Somewhere Sunny", shouldReplay: true, forceReplay: true);
		GC.Collect();
	}

	public override void Draw(TimeTracker gameTime)
	{
		m_scene.Draw(gameTime, m_transition.Alpha);
		if (!m_transition.IsTransitionedOut)
		{
			m_transition.Draw(gameTime);
		}
	}

	public override void Update(TimeTracker gameTime)
	{
		if (m_bExit)
		{
			if (!SaveManager.IsSaving())
			{
				m_bExit = false;
				m_scene.Reset();
				SceneRenderer.MoveCamera(default(Vector2), 0f, 1f);
				m_transition.TransitionOut();
				new TitleScreen(showLogos: false, m_scene.GetPlayer());
			}
			return;
		}
		m_scene.Update(gameTime);
		if (!m_transition.IsTransitionedOut && !SaveManager.IsSaving())
		{
			if (m_iJustFinishedSave < 2)
			{
				m_iJustFinishedSave++;
			}
			else
			{
				m_transition.Update(gameTime);
			}
		}
		if (m_bReset && m_transition.IsTransitionedIn)
		{
			if (m_bSaveGlobalOptions && !SaveManager.IsSaving())
			{
				m_transition.TransitionOut();
				SaveManager.SaveGlobalOptions();
				m_bSaveGlobalOptions = false;
				m_iJustFinishedSave = 0;
			}
			else if (!m_bSaveGlobalOptions && !SaveManager.IsSaving())
			{
				m_scene.Reset();
				m_bReset = false;
				m_transition.TransitionOut();
				SoundManager.AddSoundToPlay(SoundManager.GetSoundEffect("sounds/freesound/65929__dobroide"), 1f, 0f, 100);
				GC.Collect();
			}
		}
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (!m_bExit)
		{
			m_scene.HandleInput(gameTime);
			if (ControlManager.PressedStart(ControlManager.ActiveMenuIndex) || !ControlManager.ControlConn(ControlManager.ActiveMenuIndex))
			{
				new PauseScreen(m_scene);
			}
		}
	}

	public override void OnRegainFocus(string applicatorInfo)
	{
		if (!applicatorInfo.Equals("Unpause"))
		{
			if (applicatorInfo.Equals("Reset"))
			{
				m_bReset = true;
				m_transition.TransitionIn();
			}
			else if (applicatorInfo.Equals("ResetSave"))
			{
				m_bReset = true;
				m_transition.TransitionIn();
				m_bSaveGlobalOptions = true;
			}
			else if (applicatorInfo.Equals("ExitMenu"))
			{
				SaveManager.SaveGlobalOptions();
				m_bExit = true;
			}
		}
	}

	public void Reset()
	{
		m_bReset = true;
	}

	public Player GetPlayer()
	{
		return m_scene.GetPlayer();
	}

	public void SetLevelAndRepeat(int level, bool b)
	{
		m_scene.SetInfiniteWorld(b);
		m_scene.SetDefaultWorld(level);
	}

	public SceneContainer GetScene()
	{
		return m_scene;
	}
}
