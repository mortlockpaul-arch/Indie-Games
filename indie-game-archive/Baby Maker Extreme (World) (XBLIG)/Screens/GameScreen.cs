using System;
using System.Threading;
using BabyMaker;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using MusicPlayer;
using Physics;
using PlayerData;
using Renderer;
using Scene;

namespace Screens;

public class GameScreen : Screen
{
	private enum GameState
	{
		LOGO_DRAW,
		CHARGE_PHASE,
		AIR_PHASE
	}

	private GameState m_state;

	private ContractionManager m_Contractions;

	private PushManager m_Pushes;

	private HospitalScene m_scene;

	private LogoScreenHelper m_logo;

	private TransitionHelper m_transition;

	private int m_iStartTimer;

	private RenderSprite m_getReady;

	private TimeTracker updateGameTime;

	private AutoResetEvent myLock1;

	private AutoResetEvent myLock2;

	private bool startSim;

	private Thread physicsThread;

	private bool m_bExit;

	public GameScreen()
		: base(updateParent: false, drawParent: false, inputParent: false)
	{
		FirstLoad();
		PhysicsObjectManager.Initialize(170f);
		m_scene = new HospitalScene();
		PhysicsObjectManager.GetSimulation().Update(1f / 60f);
		GenericParticleImgs.Initialize();
		myLock1 = new AutoResetEvent(initialState: false);
		myLock2 = new AutoResetEvent(initialState: true);
		Initialize(firstRun: true);
		physicsThread = new Thread(PhysicsThreadProc);
		startSim = false;
		physicsThread.Start();
		m_bExit = false;
	}

	public PropPool GetPropPool()
	{
		return m_scene.GetPropPool();
	}

	public void FirstLoad()
	{
		SpriteManager.GetImage("images/boyScore");
		SpriteManager.GetImage("images/boyTips");
		SpriteManager.GetImage("images/girlScore");
		SpriteManager.GetImage("images/girlTips");
		SpriteManager.GetImage("images/pausebg");
		SpriteManager.GetImage("images/trophies/blonds");
		SpriteManager.GetImage("images/trophies/docBlock");
		SpriteManager.GetImage("images/trophies/eatingOut");
		SpriteManager.GetImage("images/trophies/gotCrabs");
		SpriteManager.GetImage("images/trophies/onBottom");
		SpriteManager.GetImage("images/trophies/tripod");
		SpriteManager.GetImage("images/trophies/brokenprotect");
		SpriteManager.GetImage("images/trophies/deflower");
		SpriteManager.GetImage("images/trophies/enlargement");
		SpriteManager.GetImage("images/trophies/lightsout");
		SpriteManager.GetImage("images/trophies/pill");
		SpriteManager.GetImage("images/trophies/pumpIt");
		SpriteManager.GetImage("images/trophies/ridebones");
		SpriteManager.GetImage("images/trophies/seeinside");
		SpriteManager.GetImage("images/trophies/stiffy");
		SpriteManager.GetImage("images/trophies/strapon");
		SpriteManager.GetImage("images/trophies/projectileDysfunc");
		SpriteManager.GetImage("images/controllerButtons/A");
		SpriteManager.GetImage("images/controllerButtons/B");
		SpriteManager.GetImage("images/controllerButtons/X");
		SpriteManager.GetImage("images/controllerButtons/Y");
		SoundManager.GetSoundEffect("sounds/freesound/babies/14264__pfly__babybabblebit01");
		SoundManager.GetSoundEffect("sounds/freesound/babies/18275__Corsica_S__baby_giggle");
		SoundManager.GetSoundEffect("sounds/freesound/babies/47370__reinsamba__baby_laugh1");
		SoundManager.GetSoundEffect("sounds/freesound/babies/47371__reinsamba__baby_laugh2");
		SoundManager.GetSoundEffect("sounds/freesound/babies/47372__reinsamba__baby_laugh3");
		SoundManager.GetSoundEffect("sounds/freesound/babies/47374__reinsamba__baby_voice15");
		SoundManager.GetSoundEffect("sounds/freesound/babies/47375__reinsamba__baby_voice16");
		SoundManager.GetSoundEffect("sounds/freesound/babies/59459__Erdie__Lena_laughes03");
		SoundManager.GetSoundEffect("sounds/freesound/babies/59460__Erdie__Lena_laughes09");
		SoundManager.GetSoundEffect("sounds/freesound/babies/65895__Robinhood76");
		SoundManager.GetSoundEffect("sounds/freesound/babies/81211__bennstir__Baby_laugh1");
		SoundManager.GetSoundEffect("sounds/pop");
		SceneRenderer.GetContentManager().Load<Song>("sounds/incompetech/Big Rock");
	}

	public void PhysicsThreadProc()
	{
		physicsThread.SetProcessorAffinity(new int[1] { 3 });
		while (!m_bExit)
		{
			myLock1.WaitOne();
			Game1.Profiler.StartTimer(ProfileTypes.PROF_PHYSICS);
			if (!m_bExit)
			{
				if (startSim)
				{
					PhysicsObjectManager.Update(updateGameTime);
				}
				else if (updateGameTime != null)
				{
					startSim = true;
				}
			}
			Game1.Profiler.EndTimer(ProfileTypes.PROF_PHYSICS);
			myLock2.Set();
		}
	}

	public void EndPhysicsLoop()
	{
		m_bExit = true;
		myLock1.Set();
	}

	public void Initialize(bool firstRun)
	{
		myLock2.WaitOne();
		PhysicsObjectManager.Clear();
		m_scene.Initialize();
		PhysicsObjectManager.GetSimulation().Update(1f / 120f);
		PropSoundPlayer.Initialize();
		m_transition = new TransitionHelper();
		m_transition.TransitionOut();
		m_state = GameState.LOGO_DRAW;
		if (firstRun)
		{
			m_logo = new LogoScreenHelper(m_scene.GetPlayer());
			m_iStartTimer = 0;
			m_getReady = null;
		}
		else
		{
			m_logo = null;
			m_iStartTimer = 2000;
			m_getReady = SpriteManager.GetSprite("images/getReady", default(Vector2), DepthConsts.LOGO_DEPTH);
		}
		m_Pushes = new PushManager(m_scene.GetPlayer());
		m_Contractions = new ContractionManager(m_Pushes);
		if (firstRun)
		{
			Mp3MusicPlayer.Initialize("sounds/incompetech/Somewhere Sunny", shouldReplay: true, forceReplay: true);
			Mp3MusicPlayer.FadeIn(1f);
		}
		GC.Collect();
		myLock1.Set();
	}

	public override void Draw(TimeTracker gameTime)
	{
		if (!m_transition.IsTransitionedOut)
		{
			m_transition.Draw(gameTime);
		}
		m_scene.Draw(gameTime, m_state == GameState.AIR_PHASE);
		if (m_state == GameState.LOGO_DRAW)
		{
			if (m_logo != null)
			{
				m_logo.Draw(gameTime);
				return;
			}
			m_getReady.Position = SceneRenderer.GetCameraPosition();
			m_getReady.Draw(gameTime);
		}
		else if (m_state == GameState.CHARGE_PHASE)
		{
			m_Contractions.Draw(gameTime);
			m_Pushes.Draw(gameTime, m_Contractions.IsComplete);
		}
		else if (m_state == GameState.AIR_PHASE)
		{
			m_Pushes.Draw(gameTime, m_Contractions.IsComplete);
		}
	}

	public override void HandleInput(TimeTracker gameTime)
	{
		if (!m_transition.IsTransitionedOut)
		{
			return;
		}
		if (m_state == GameState.LOGO_DRAW)
		{
			if (m_logo != null)
			{
				m_logo.HandleInput(gameTime);
			}
		}
		else if (m_state == GameState.CHARGE_PHASE)
		{
			if (m_Contractions.IsComplete)
			{
				m_Pushes.HandleInput(gameTime);
			}
			else
			{
				m_Contractions.HandleInput(gameTime);
			}
		}
		else if (m_state == GameState.AIR_PHASE)
		{
			m_scene.HandleInput(gameTime);
			if (m_scene.IsComplete() && Game1.PeekScreen() == this)
			{
				new ScoreScreen(m_scene.GetPlayer());
			}
		}
		if ((m_state != GameState.LOGO_DRAW || m_logo == null) && (ControlManager.PressedStart(ControlManager.ActiveMenuIndex) || (ControlManager.ActiveMenuIndex >= 0 && !ControlManager.ControlConn(ControlManager.ActiveMenuIndex))))
		{
			new PauseScreen(m_scene.GetPlayer());
		}
	}

	public override void Update(TimeTracker gameTime)
	{
		if (m_state == GameState.AIR_PHASE)
		{
			myLock2.WaitOne();
			updateGameTime = gameTime;
		}
		Game1.Profiler.StartTimer(ProfileTypes.PROF_STARTUPDATE);
		if (!m_transition.IsTransitionedOut)
		{
			m_transition.Update(gameTime);
		}
		m_scene.Update(gameTime, m_state == GameState.AIR_PHASE);
		if (m_state == GameState.LOGO_DRAW)
		{
			if (m_logo != null)
			{
				m_logo.Update(gameTime);
				if (m_logo.IsComplete)
				{
					m_state = GameState.CHARGE_PHASE;
				}
			}
			else
			{
				m_iStartTimer -= gameTime.ElapsedMilli;
				if (m_iStartTimer < 0)
				{
					m_state = GameState.CHARGE_PHASE;
				}
				if (m_iStartTimer < 300)
				{
					m_getReady.Alpha = Math.Max(0f, (float)(m_iStartTimer - 100) / 200f);
				}
			}
		}
		else if (m_state == GameState.CHARGE_PHASE)
		{
			m_Contractions.Update(gameTime);
			m_Pushes.Update(gameTime, m_Contractions.IsComplete);
			if (m_Pushes.IsComplete)
			{
				m_state = GameState.AIR_PHASE;
				ThrowBaby();
				Mp3MusicPlayer.Initialize("sounds/incompetech/Big Rock", shouldReplay: true, forceReplay: true);
				Mp3MusicPlayer.FadeIn(10f);
			}
		}
		else if (m_state == GameState.AIR_PHASE)
		{
			m_Pushes.ClearUpdate(gameTime);
		}
		Game1.Profiler.EndTimer(ProfileTypes.PROF_STARTUPDATE);
		if (m_state == GameState.AIR_PHASE)
		{
			myLock1.Set();
		}
	}

	public override void OnRegainFocus(string applicatorInfo)
	{
		if (applicatorInfo.Equals("Restart"))
		{
			SoundManager.AddSoundToPlay(SoundManager.GetSoundEffect("sounds/freesound/65929__dobroide"), 1f, 0f, 0);
			Initialize(firstRun: false);
		}
	}

	public void ThrowBaby()
	{
		m_scene.Launch(m_Pushes.Angle, m_Pushes.Power);
	}
}
