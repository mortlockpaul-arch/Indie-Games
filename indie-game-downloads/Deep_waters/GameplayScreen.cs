using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Effects;
using SynapseGaming.LightingSystem.Effects.Forward;
using SynapseGaming.LightingSystem.Rendering;

namespace Deep_waters;

internal class GameplayScreen : GameScreen
{
	private BlendState TrueAdditiveBlend = new BlendState
	{
		ColorSourceBlend = Blend.One,
		ColorDestinationBlend = Blend.One,
		AlphaSourceBlend = Blend.One,
		AlphaDestinationBlend = Blend.One
	};

	private ContentManager content;

	private SpriteFont gameFont;

	private int levelnumber = 1;

	private Random random = new Random();

	private FrameRateCounter fpscounter;

	private float pauseAlpha;

	private Level CurrentLevel;

	private GamePlayState gamePlayState;

	private Player shelly;

	private List<Shark> sharks;

	private Shark AttackingShark;

	private float currentmaxawareness;

	private FloatingItem ExitPoint;

	private SpriteFont subtitles;

	private Texture2D smoketext;

	private Texture2D firetext;

	private Texture2D Sparkletext;

	private Texture2D splat;

	private Texture2D Compass;

	private Texture2D Arrow;

	private Texture2D EnergyFrame;

	private Texture2D Stamina;

	public bool bossalreadyreached;

	private Effect particleeffect;

	private int randomcamera;

	private int randomqte;

	private List<Texture2D> QTE;

	public List<ScreenFlashesTextures> Blood;

	private bool nextlevel;

	private RenderTarget2D modelrenderer;

	private Effect postprocess;

	private float timerpp;

	private float alphavalue;

	private Model sharkmodel;

	private int AttackingSide;

	private bool assignedangle;

	private bool success;

	private Splat bleeding;

	private float timeranimation;

	private Shark sharknexttoattack;

	private SceneObject ep;

	private float compassrot;

	private Boss currentBoss;

	private Video intro;

	private VideoPlayer vp;

	private CutSceneAction introlevel1;

	private ContentRepository contentRepository;

	private SceneInterface sceneInterface;

	private FrameBuffers frameBuffers;

	private SceneState sceneState;

	private SceneEnvironment environment;

	private SystemPreferences preferences;

	private Plane waterWorldPlane;

	private SasEffect waterEffect;

	private int reflectionRefractionTargetSize = 512;

	private int reflectionRefractionTargetMultiSampleAmount = 2;

	private FrameBuffers reflectionRefractionFrameBuffers;

	private RenderTargetHelper refractionTarget;

	private RenderTargetHelper waterReflectionTarget;

	private SystemPreferences refractionPreferences;

	private DetailPreference renderQuality;

	private SpriteBatch spriteBatch;

	private ParticleManager particleManager;

	private SunBurnCoreSystem lightingSystemManager;

	private SceneObject water;

	private bool imunderwater;

	private CutSceneVideo introlevel2;

	private SceneObject entryp;

	private FloatingItem EntryPoint;

	private Texture2D journal;

	private bool aispressed;

	private FinalBoss FinalCurrBoss;

	private bool imunloading;

	private bool imdisposed;

	private bool haslreadyplayed;

	private Texture2D waterparticle;

	private bloodfountain bloodfn;

	private sharkwatertrail sharkwater;

	private string batt1 = "Virginia do you read me... this is the port captain's office \n do you read me?";

	private string batt2 = "we no longer receive signals from two days,\n you have changed course ... do you need help?";

	private string batt3 = "... I am Shelly Sanders, ... please send help!";

	private bool bossplaynomusic;

	public GameplayScreen(int leveln, bool bossalreadyreach)
	{
		bossalreadyreached = bossalreadyreach;
		levelnumber = leveln;
		base.TransitionOnTime = TimeSpan.FromSeconds(1.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
		vp = new VideoPlayer();
	}

	private void PrepareDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
	{
		e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PlatformContents;
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		modelrenderer = new RenderTarget2D(base.ScreenManager.Game.GraphicsDevice, base.ScreenManager.GraphicsDevice.Viewport.Width, base.ScreenManager.GraphicsDevice.Viewport.Height, mipMap: true, SurfaceFormat.Dxt5, DepthFormat.Depth24Stencil8, 3, RenderTargetUsage.DiscardContents);
		base.ScreenManager.currentlevelnumber = levelnumber;
		base.ScreenManager.reachedboss = bossalreadyreached;
		graphics = base.ScreenManager.graphics;
		graphics.SynchronizeWithVerticalRetrace = false;
		graphics.PreparingDeviceSettings += PrepareDeviceSettings;
		graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
		renderQuality = DetailPreference.Medium;
		reflectionRefractionTargetSize = 512;
		reflectionRefractionTargetMultiSampleAmount = 2;
		graphics.PreferMultiSampling = true;
		graphics.SynchronizeWithVerticalRetrace = true;
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		subtitles = content.Load<SpriteFont>("Font/Subtitles");
		spriteBatch = new SpriteBatch(base.ScreenManager.GraphicsDevice);
		intro = content.Load<Video>("Video/intro");
		init_managers();
		if (contentRepository == null)
		{
			contentRepository = content.Load<ContentRepository>("Scenes/scene" + levelnumber + "/Content");
		}
		else
		{
			contentRepository.Dispose();
			contentRepository = content.Load<ContentRepository>("Scenes/scene" + levelnumber + "/Content");
		}
		QTE = new List<Texture2D>
		{
			content.Load<Texture2D>("QTE/Up"),
			content.Load<Texture2D>("QTE/Left"),
			content.Load<Texture2D>("QTE/Right"),
			content.Load<Texture2D>("QTE/Down"),
			content.Load<Texture2D>("QTE/A"),
			content.Load<Texture2D>("QTE/B"),
			content.Load<Texture2D>("QTE/X"),
			content.Load<Texture2D>("QTE/Y"),
			content.Load<Texture2D>("QTE/LeftTrigger"),
			content.Load<Texture2D>("QTE/RightTrigger"),
			content.Load<Texture2D>("QTE/LeftShoulder"),
			content.Load<Texture2D>("QTE/RightShoulder")
		};
		gameFont = content.Load<SpriteFont>("Font/MenuUnselected");
		fpscounter = new FrameRateCounter();
		sceneInterface.ObjectManager.AutoOptimize = true;
		smoketext = content.Load<Texture2D>("Sprites/smoke");
		Sparkletext = content.Load<Texture2D>("Sprites/sparkle");
		firetext = content.Load<Texture2D>("Sprites/fire");
		splat = content.Load<Texture2D>("Sprites/splat");
		EnergyFrame = content.Load<Texture2D>("Sprites/PlayerBar");
		Stamina = content.Load<Texture2D>("Sprites/Stamina");
		Compass = content.Load<Texture2D>("Sprites/compass");
		Arrow = content.Load<Texture2D>("Sprites/arrow");
		waterparticle = content.Load<Texture2D>("Sprites/Water");
		postprocess = content.Load<Effect>("Effects/WaterPP");
		CurrentLevel = new Level(content, sceneInterface, sceneState, levelnumber);
		CurrentLevel.LoadContent();
		sceneInterface.Submit(CurrentLevel.scene);
		CurrentLevel.initialize();
		sceneInterface.Editor.UserHandledView = false;
		particleeffect = content.Load<Effect>("Effects/ParticleEffect");
		shelly = new Player(content.Load<Model>("Models/Shelly"), base.ControllingPlayer.Value, new Vector3(0f, -6f, 0f), new Vector3(0f, 0f, 0f), 0.05f);
		CurrentLevel.mCamera.endlookat = new Vector3(shelly.World.Translation.X, 2f, shelly.World.Translation.Z);
		CurrentLevel.mCamera.endposition = new Vector3(shelly.World.Translation.X, 2f, shelly.World.Translation.Z) + new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.Rotation.Y)) * 12f, 2f, (float)Math.Cos(MathHelper.ToRadians(shelly.Rotation.Y)) * -12f);
		CurrentLevel.mCamera.lookat = new Vector3(shelly.World.Translation.X, 2f, shelly.World.Translation.Z);
		CurrentLevel.mCamera.position = new Vector3(shelly.World.Translation.X, 2f, shelly.World.Translation.Z) + new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.Rotation.Y)) * 12f, 2f, (float)Math.Cos(MathHelper.ToRadians(shelly.Rotation.Y)) * -12f);
		sharkmodel = content.Load<Model>("Models/sharksmall");
		sharks = new List<Shark>();
		sceneInterface.ObjectManager.Find("ExitPoint", false, out ep);
		sceneInterface.ObjectManager.Find("EntryPoint", false, out entryp);
		ExitPoint = new FloatingItem(content.Load<Model>("Scenes/Scene" + levelnumber + "/exitpoint"), Vector3.Zero, Vector3.Zero, Vector3.Zero, 0f);
		if (levelnumber != 3)
		{
			EntryPoint = new FloatingItem(content.Load<Model>("Scenes/Scene" + levelnumber + "/entrypoint"), Vector3.Zero, Vector3.Zero, Vector3.Zero, 0f);
		}
		if (ep != null)
		{
			ExitPoint.World = ep.World;
			ep.Visibility = ObjectVisibility.RenderedInEditor;
		}
		if (entryp != null)
		{
			EntryPoint.World = entryp.World;
			entryp.Visibility = ObjectVisibility.RenderedInEditor;
		}
		if (levelnumber == 1)
		{
			currentBoss = new Boss(base.ControllingPlayer.Value, new List<Actor>
			{
				new Actor(content.Load<Model>("Scenes/Scene1/Boss/SceneEnvironment"), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Intro"),
				new Actor(content.Load<Model>("Scenes/Scene1/Boss/MainChar"), new Vector3(0f, -4.3f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Intro"),
				new Actor(content.Load<Model>("Scenes/Scene1/Boss/Boss"), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Intro")
			}, QTE, new List<quicktime>
			{
				quicktime.single,
				quicktime.single,
				quicktime.single,
				quicktime.single,
				quicktime.single,
				quicktime.single
			}, 3, 2, "Intro", isloop: false, 1f, levelnumber);
			introlevel1 = new CutSceneAction(base.ControllingPlayer.Value, new List<Actor>
			{
				new Actor(content.Load<Model>("Scenes/Scene1/CutScene1/SceneEnvironment"), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Intro"),
				new Actor(content.Load<Model>("Scenes/Scene1/CutScene1/MainChar"), new Vector3(0f, -4.3f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Intro")
			}, QTE, quicktime.mash, "Intro", isloop: false, levelnumber);
		}
		if (levelnumber == 2)
		{
			journal = content.Load<Texture2D>("Background/Journal");
			currentBoss = new Boss(base.ControllingPlayer.Value, new List<Actor>
			{
				new Actor(content.Load<Model>("Scenes/Scene2/Boss/SceneEnvironment"), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Intro"),
				new Actor(content.Load<Model>("Scenes/Scene2/Boss/MainChar"), new Vector3(0f, -3.3f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Intro"),
				new Actor(content.Load<Model>("Scenes/Scene2/Boss/Boss"), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Intro")
			}, QTE, new List<quicktime>
			{
				quicktime.single,
				quicktime.single,
				quicktime.single,
				quicktime.single,
				quicktime.single,
				quicktime.single
			}, 3, 2, "Intro", isloop: false, 1f, levelnumber);
			introlevel2 = new CutSceneVideo(new List<Actor>
			{
				new Actor(content.Load<Model>("Scenes/Scene2/CutScene1/SceneEnvironment"), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Intro"),
				new Actor(content.Load<Model>("Scenes/Scene2/CutScene1/MainChar"), new Vector3(0f, -4.3f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Intro")
			}, "Intro", levelnumber);
		}
		if (levelnumber == 3)
		{
			FinalCurrBoss = new FinalBoss(base.ControllingPlayer.Value, new List<Actor>
			{
				new Actor(content.Load<Model>("Scenes/Scene3/Boss/SceneEnvironment"), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Quicktime1"),
				new Actor(content.Load<Model>("Scenes/Scene3/Boss/MainChar"), new Vector3(0f, -4.3f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Quicktime1"),
				new Actor(content.Load<Model>("Scenes/Scene3/Boss/Boss"), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Quicktime1"),
				new Actor(content.Load<Model>("Scenes/Scene3/Boss/bombola"), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Quicktime1")
			}, QTE, new List<quicktime>
			{
				quicktime.mash,
				quicktime.single,
				quicktime.mash,
				quicktime.combo
			}, "Quicktime1", isloop: false, 1f);
			introlevel2 = new CutSceneVideo(new List<Actor>
			{
				new Actor(content.Load<Model>("Scenes/Scene3/CutScene1/SceneEnvironment"), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Intro"),
				new Actor(content.Load<Model>("Scenes/Scene3/CutScene1/MainChar"), new Vector3(0f, -4.3f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Intro"),
				new Actor(content.Load<Model>("Scenes/Scene3/CutScene1/Shark"), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), 0.05f, "Intro")
			}, "Intro", levelnumber);
		}
		reflectionRefractionFrameBuffers = new FrameBuffers(reflectionRefractionTargetSize, reflectionRefractionTargetSize, DetailPreference.Medium, DetailPreference.Medium);
		refractionTarget = new RenderTargetHelper(RenderTargetHelper.TargetType.Standard, reflectionRefractionTargetSize, reflectionRefractionTargetSize, mipmapped: false, SurfaceFormat.Color, reflectionRefractionTargetMultiSampleAmount, RenderTargetUsage.PlatformContents);
		waterReflectionTarget = new RenderTargetHelper(RenderTargetHelper.TargetType.Reflection, reflectionRefractionTargetSize, reflectionRefractionTargetSize, mipmapped: false, SurfaceFormat.Color, reflectionRefractionTargetMultiSampleAmount, RenderTargetUsage.PlatformContents);
		sceneInterface.ResourceManager.AssignOwnership(frameBuffers);
		sceneInterface.ResourceManager.AssignOwnership(reflectionRefractionFrameBuffers);
		sceneInterface.ResourceManager.AssignOwnership(refractionTarget);
		sceneInterface.ResourceManager.AssignOwnership(waterReflectionTarget);
		refractionPreferences = new SystemPreferences();
		refractionPreferences.EffectDetail = DetailPreference.Low;
		refractionPreferences.MaxAnisotropy = 0;
		refractionPreferences.PostProcessingDetail = DetailPreference.Low;
		refractionPreferences.ShadowDetail = DetailPreference.Low;
		refractionPreferences.ShadowQuality = 0.25f;
		refractionPreferences.TextureSampling = SamplingPreference.Trilinear;
		refractionTarget.ApplyPreferences(refractionPreferences);
		waterReflectionTarget.ApplyPreferences(refractionPreferences);
		waterEffect = content.Load<SasEffect>("Scenes/Scene" + levelnumber + "/WaterRef");
		waterWorldPlane = new Plane(Vector3.Down, 1.2f);
		water = new SceneObject(content.Load<Model>("Scenes/Scene" + levelnumber + "/OceanMesh"));
		water.RenderableMeshes[0].Effect = content.Load<SasEffect>("Scenes/Scene" + levelnumber + "/water");
		water.World = Matrix.CreateScale(new Vector3(25f, 0.5f, 25f)) * Matrix.CreateTranslation(new Vector3(0f, 1f, 0f));
		CurrentLevel.WaterPlane = water;
		sceneInterface.ObjectManager.Submit(water);
		Thread.Sleep(1000);
		if (!bossalreadyreached)
		{
			if (levelnumber == 1)
			{
				initCutScene();
				vp.Play(intro);
				CurrentLevel.mCamera.lookat = Vector3.Zero;
				CurrentLevel.mCamera.endlookat = Vector3.Zero;
				CurrentLevel.mCamera.position = new Vector3(0f, 100f, 100f);
				CurrentLevel.mCamera.endposition = new Vector3(0f, 100f, 100f);
			}
			if (levelnumber == 2)
			{
				gamePlayState = GamePlayState.InitCutscene;
				initCutScene();
				CurrentLevel.mCamera.lookat = Vector3.Zero;
				CurrentLevel.mCamera.endlookat = Vector3.Zero;
				CurrentLevel.mCamera.position = new Vector3(0f, 100f, 100f);
				CurrentLevel.mCamera.endposition = new Vector3(0f, 100f, 100f);
			}
			if (levelnumber == 3)
			{
				gamePlayState = GamePlayState.InitCutscene;
				initCutScene();
				CurrentLevel.mCamera.lookat = Vector3.Zero;
				CurrentLevel.mCamera.endlookat = Vector3.Zero;
				CurrentLevel.mCamera.position = new Vector3(0f, 100f, 100f);
				CurrentLevel.mCamera.endposition = new Vector3(0f, 100f, 100f);
			}
		}
		else
		{
			gamePlayState = GamePlayState.Boss;
			BossInit();
			CurrentLevel.mCamera.lookat = Vector3.Zero;
			CurrentLevel.mCamera.endlookat = Vector3.Zero;
			CurrentLevel.mCamera.position = new Vector3(0f, 100f, 100f);
			CurrentLevel.mCamera.endposition = new Vector3(0f, 100f, 100f);
		}
		base.ScreenManager.Game.ResetElapsedTime();
	}

	public void init_managers()
	{
		lightingSystemManager = new SunBurnCoreSystem(base.ScreenManager.Game.Services, content);
		sceneState = new SceneState();
		sceneInterface = new SceneInterface();
		sceneInterface.CreateDefaultManagers(RenderingSystemType.Forward, CollisionSystemType.Physics, autoloadpluginmanagers: true);
		frameBuffers = new FrameBuffers(DetailPreference.High, DetailPreference.Medium);
		sceneInterface.ResourceManager.AssignOwnership(frameBuffers);
		particleManager = new ParticleManager(graphics);
		particleManager.ManagerProcessOrder = 0;
		sceneInterface.AddManager(particleManager);
		sceneInterface.ShowConsole = true;
	}

	public void intilevel()
	{
		sceneInterface.ObjectManager.Submit(ExitPoint);
		if (levelnumber != 1 && levelnumber != 3)
		{
			sceneInterface.ObjectManager.Submit(EntryPoint);
		}
		sceneInterface.ObjectManager.Submit(shelly);
		for (int i = 0; i < 6; i++)
		{
			sharks.Add(new Shark(sharkmodel, new Vector3(10 * i + random.Next(-100, 100), random.Next(-7, -5), 10 * i + random.Next(-100, 100)), new Vector3(0f, random.Next(0, 360), 0f), 0.05f, i));
			sceneInterface.ObjectManager.Submit(sharks[i]);
		}
		if (introlevel1 != null)
		{
			foreach (Actor actor in introlevel1.Actors)
			{
				sceneInterface.ObjectManager.Remove(actor);
			}
		}
		if (introlevel2 == null)
		{
			return;
		}
		foreach (Actor actor2 in introlevel2.Actors)
		{
			sceneInterface.ObjectManager.Remove(actor2);
		}
	}

	public override void UnloadContent()
	{
		if (!imdisposed)
		{
			imunloading = true;
			sceneInterface.Unload();
			lightingSystemManager.Unload();
			gameFont = null;
			random = new Random();
			base.ScreenManager.audioManager.stopallsfx();
			base.ScreenManager.audioManager.stopMusic();
			CurrentLevel = null;
			shelly = null;
			sharks = null;
			AttackingShark = null;
			ExitPoint = null;
			smoketext = null;
			firetext = null;
			Sparkletext = null;
			splat = null;
			Compass = null;
			Arrow = null;
			EnergyFrame = null;
			waterparticle = null;
			Stamina = null;
			particleeffect = null;
			QTE = null;
			modelrenderer = null;
			postprocess = null;
			sharkmodel = null;
			bleeding = null;
			sharknexttoattack = null;
			ep = null;
			currentBoss = null;
			intro = null;
			vp = null;
			introlevel1 = null;
			sceneInterface = null;
			frameBuffers = null;
			sceneState = null;
			environment = null;
			preferences = null;
			waterEffect = null;
			reflectionRefractionFrameBuffers = null;
			refractionTarget = null;
			waterReflectionTarget = null;
			refractionPreferences = null;
			spriteBatch = null;
			particleManager = null;
			lightingSystemManager = null;
			water = null;
			introlevel2 = null;
			entryp = null;
			EntryPoint = null;
			FinalCurrBoss = null;
			content.Unload();
			contentRepository.Clear();
			contentRepository.Dispose();
			imdisposed = true;
		}
	}

	private void checksharckCollision(Shark shark)
	{
		if (shark.behaviour != Behaviour.patrol)
		{
			return;
		}
		foreach (Shark shark2 in sharks)
		{
			if (shark == shark2)
			{
				continue;
			}
			if (Vector3.Distance(shark.Position, shark2.Position) <= 20f && !shark.imcolliding)
			{
				if (shark != AttackingShark)
				{
					if (shark.Position.Y <= -0.5f)
					{
						shark.nextrotation.X = -20f;
					}
					else
					{
						shark.nextrotation.X = 0f;
					}
					shark.imcolliding = true;
					shark.nextrotation.Y = shark.Rotation.Y - 180f;
				}
				if (shark2 != AttackingShark)
				{
					if (shark2.Position.Y > -10f)
					{
						shark2.nextrotation.X = 20f;
					}
					else
					{
						shark2.nextrotation.X = 0f;
					}
					shark2.imcolliding = true;
					shark2.nextrotation.Y = shark.Rotation.Y + 180f;
				}
				break;
			}
			if (shark.imcolliding)
			{
				shark.nextrotation.X = 0f;
				shark.nextrotation.Y = shark.Rotation.Y;
				shark.imcolliding = false;
				shark2.nextrotation.X = 0f;
				shark2.nextrotation.Y = shark2.Rotation.Y;
				shark2.imcolliding = false;
			}
		}
	}

	private void updatemanager(GameTime gameTime)
	{
		if (gamePlayState == GamePlayState.Intro)
		{
			CurrentLevel.mCamera.update(gameTime);
			if (vp.State == MediaState.Stopped)
			{
				gamePlayState = GamePlayState.InitCutscene;
				initCutScene();
			}
			return;
		}
		if (!haslreadyplayed)
		{
			base.ScreenManager.audioManager.playsound("sea");
			base.ScreenManager.audioManager.playsound("underwater");
			base.ScreenManager.audioManager.pausesound("underwater");
			haslreadyplayed = true;
		}
		else if (CurrentLevel.mCamera.position.Y > 1f)
		{
			if (gamePlayState == GamePlayState.Game || gamePlayState == GamePlayState.Attacked || gamePlayState == GamePlayState.QTE)
			{
				if (shelly != null)
				{
					if (shelly.splushsfx)
					{
						particleManager.submit(new WaterDrop(base.ScreenManager.GraphicsDevice, waterparticle, particleeffect, new Vector3(shelly.Position.X, 1.2f, shelly.Position.Z)));
						base.ScreenManager.audioManager.playsound("upwater");
						shelly.splushsfx = false;
					}
					if (base.ScreenManager.audioManager.isplayngsfx("Hit") || base.ScreenManager.audioManager.isplayngsfx("Death"))
					{
						base.ScreenManager.audioManager.stopsfx("breath");
					}
					else if (!shelly.movingfast)
					{
						base.ScreenManager.audioManager.stopsfx("swimfast");
						if (!base.ScreenManager.audioManager.isplayngsfx("breath"))
						{
							base.ScreenManager.audioManager.playsound("breath");
						}
						if (shelly.moving)
						{
							if (!base.ScreenManager.audioManager.isplayngsfx("swimslow"))
							{
								base.ScreenManager.audioManager.playsound("swimslow");
							}
						}
						else
						{
							base.ScreenManager.audioManager.stopsfx("swimslow");
						}
					}
					else
					{
						if (!base.ScreenManager.audioManager.isplayngsfx("swimfast"))
						{
							base.ScreenManager.audioManager.playsound("swimfast");
						}
						base.ScreenManager.audioManager.stopsfx("swimslow");
						base.ScreenManager.audioManager.stopsfx("breath");
					}
				}
			}
			else
			{
				base.ScreenManager.audioManager.stopsfx("swimfast");
				if (levelnumber == 1)
				{
					if (gamePlayState != GamePlayState.InitCutscene)
					{
						base.ScreenManager.audioManager.stopsfx("breath");
					}
					else if (!base.ScreenManager.audioManager.isplayngsfx("breath"))
					{
						base.ScreenManager.audioManager.playsound("breath");
					}
				}
				else
				{
					base.ScreenManager.audioManager.stopsfx("breath");
				}
			}
			base.ScreenManager.audioManager.resumesound("sea");
			base.ScreenManager.audioManager.pausesound("underwater");
		}
		else
		{
			base.ScreenManager.audioManager.resumesound("underwater");
			base.ScreenManager.audioManager.stopsfx("breath");
			base.ScreenManager.audioManager.pausesound("sea");
		}
		particleManager.Update(gameTime);
		particleManager.updatecamera(CurrentLevel.mCamera);
		if (shelly.scream)
		{
			base.ScreenManager.audioManager.playsound("Death");
			shelly.scream = false;
		}
		if (gamePlayState == GamePlayState.InitCutscene)
		{
			if (levelnumber == 1)
			{
				if (introlevel1.bclook != introlevel1.bcpos)
				{
					CurrentLevel.mCamera.lookat = introlevel1.bclook.Translation * 0.05f;
					CurrentLevel.mCamera.endlookat = introlevel1.bclook.Translation * 0.05f;
					CurrentLevel.mCamera.position = introlevel1.bcpos.Translation * 0.05f;
					CurrentLevel.mCamera.endposition = introlevel1.bcpos.Translation * 0.05f;
					CurrentLevel.Update(gameTime);
				}
				introlevel1.update(gameTime);
				timerpp += gameTime.ElapsedGameTime.Milliseconds;
				sceneInterface.Update(gameTime);
				sceneInterface.ObjectManager.Optimize();
				if (introlevel1.bstate == Action.Null && introlevel1.gotonextlevel)
				{
					intilevel();
					gamePlayState = GamePlayState.Game;
				}
				if (introlevel1.bstate == Action.Null && introlevel1.gotogameover)
				{
					base.ScreenManager.AddScreen(new BackgroundScreen("gameover"), null);
					base.ScreenManager.AddScreen(new GameOverScreen(), null);
					introlevel1.gotonextlevel = false;
					UnloadContent();
					imunloading = true;
				}
				return;
			}
			if (levelnumber == 2)
			{
				if (introlevel2.Splash)
				{
					particleManager.submit(new WaterDrop(base.ScreenManager.GraphicsDevice, waterparticle, particleeffect, new Vector3(introlevel2.head.X, 1.1f, introlevel2.head.Z)));
					base.ScreenManager.audioManager.playsound("upwater");
					introlevel2.Splash = false;
				}
				if (introlevel2.bclook != introlevel2.bcpos)
				{
					CurrentLevel.mCamera.lookat = introlevel2.bclook.Translation * 0.05f;
					CurrentLevel.mCamera.endlookat = introlevel2.bclook.Translation * 0.05f;
					CurrentLevel.mCamera.position = introlevel2.bcpos.Translation * 0.05f;
					CurrentLevel.mCamera.endposition = introlevel2.bcpos.Translation * 0.05f;
					CurrentLevel.Update(gameTime);
				}
				introlevel2.update(gameTime);
				timerpp += gameTime.ElapsedGameTime.Milliseconds;
				sceneInterface.Update(gameTime);
				sceneInterface.ObjectManager.Optimize();
				if (introlevel2.bstate == Action.Null && introlevel2.gotonextlevel)
				{
					intilevel();
					gamePlayState = GamePlayState.Game;
				}
				return;
			}
			if (levelnumber == 3)
			{
				if (introlevel2.Splash)
				{
					particleManager.submit(new SharkWaterDrop(base.ScreenManager.GraphicsDevice, waterparticle, particleeffect, new Vector3(introlevel2.boss.X, 1.1f, introlevel2.boss.Z)));
					base.ScreenManager.audioManager.playsound("upwater");
					base.ScreenManager.audioManager.playsound("explosionwood");
					introlevel2.Splash = false;
				}
				if (introlevel2.bclook != introlevel2.bcpos)
				{
					CurrentLevel.mCamera.lookat = introlevel2.bclook.Translation * 0.05f;
					CurrentLevel.mCamera.endlookat = introlevel2.bclook.Translation * 0.05f;
					CurrentLevel.mCamera.position = introlevel2.bcpos.Translation * 0.05f;
					CurrentLevel.mCamera.endposition = introlevel2.bcpos.Translation * 0.05f;
					CurrentLevel.Update(gameTime);
				}
				introlevel2.update(gameTime);
				timerpp += gameTime.ElapsedGameTime.Milliseconds;
				sceneInterface.Update(gameTime);
				sceneInterface.ObjectManager.Optimize();
				if (introlevel2.bstate == Action.Null && introlevel2.gotonextlevel)
				{
					intilevel();
					gamePlayState = GamePlayState.Game;
				}
				return;
			}
		}
		if (GamePlayState.Journal == gamePlayState)
		{
			base.ScreenManager.audioManager.stopMusic();
			if (currentBoss.gotonextlevel && aispressed)
			{
				base.ScreenManager.reachedboss = false;
				LoadingScreen.Load(base.ScreenManager, true, base.ControllingPlayer, new GameplayScreen(levelnumber + 1, bossalreadyreach: false));
				currentBoss.gotonextlevel = false;
				UnloadContent();
				imunloading = true;
				return;
			}
		}
		if (gamePlayState == GamePlayState.Boss)
		{
			if (levelnumber != 3)
			{
				if (currentBoss.bstate == bossState.Win || currentBoss.bstate == bossState.Nextlevel)
				{
					if (base.ScreenManager.audioManager.BGMCue.IsPlaying)
					{
						base.ScreenManager.audioManager.stopMusic();
					}
					bossplaynomusic = true;
				}
				else if (!bossplaynomusic && base.ScreenManager.audioManager != null)
				{
					if (base.ScreenManager.audioManager.BGMCue == null)
					{
						base.ScreenManager.audioManager.PlayMusic("boss");
					}
					else if (base.ScreenManager.audioManager.BGMCue.IsStopped)
					{
						base.ScreenManager.audioManager.PlayMusic("boss");
					}
				}
				if (currentBoss.bclook != currentBoss.bcpos)
				{
					CurrentLevel.mCamera.lookat = currentBoss.bclook.Translation * 0.05f;
					CurrentLevel.mCamera.endlookat = currentBoss.bclook.Translation * 0.05f;
					CurrentLevel.mCamera.position = currentBoss.bcpos.Translation * 0.05f;
					CurrentLevel.mCamera.endposition = currentBoss.bcpos.Translation * 0.05f;
				}
				if (currentBoss.Splash)
				{
					particleManager.submit(new SharkWaterDrop(base.ScreenManager.GraphicsDevice, waterparticle, particleeffect, new Vector3(currentBoss.boss.X, 1.1f, currentBoss.boss.Z)));
					base.ScreenManager.audioManager.playsound("upwater");
					currentBoss.Splash = false;
				}
				if (currentBoss.Splash2)
				{
					particleManager.submit(new SharkWaterDrop(base.ScreenManager.GraphicsDevice, waterparticle, particleeffect, new Vector3(currentBoss.boss.X, 1.1f, currentBoss.boss.Z)));
					base.ScreenManager.audioManager.playsound("upwater");
					currentBoss.Splash2 = false;
				}
				if (currentBoss.scream)
				{
					base.ScreenManager.audioManager.playsound("Death");
					currentBoss.scream = false;
				}
				if (currentBoss.bleed)
				{
					bloodfn = new bloodfountain(base.ScreenManager.GraphicsDevice, splat, particleeffect, new Vector3(currentBoss.head.X, currentBoss.head.Y, currentBoss.head.Z));
					particleManager.submit(bloodfn);
					currentBoss.bleed = false;
				}
				if (bloodfn != null)
				{
					bloodfn.Emitter = new Vector3(currentBoss.head.X, currentBoss.head.Y, currentBoss.head.Z);
				}
				if (currentBoss.bstate == bossState.Null && currentBoss.gotonextlevel)
				{
					if (levelnumber == 3)
					{
						UnloadContent();
						imunloading = true;
						LoadingScreen.Load(base.ScreenManager, true, base.ControllingPlayer, new CreditsScreen());
						currentBoss.gotonextlevel = false;
					}
					else if (levelnumber == 1)
					{
						currentBoss.gotonextlevel = false;
						if (Guide.IsTrialMode)
						{
							LoadingScreen.Load(base.ScreenManager, true, base.ControllingPlayer, new BuyScreen(comfgp: true));
						}
						else
						{
							base.ScreenManager.reachedboss = false;
							LoadingScreen.Load(base.ScreenManager, true, base.ControllingPlayer, new GameplayScreen(levelnumber + 1, bossalreadyreach: false));
						}
						UnloadContent();
						imunloading = true;
					}
					else if (levelnumber == 2)
					{
						gamePlayState = GamePlayState.Journal;
					}
				}
				else if (currentBoss.bstate == bossState.Null && currentBoss.gotogameover)
				{
					base.ScreenManager.AddScreen(new BackgroundScreen("gameover"), null);
					base.ScreenManager.AddScreen(new GameOverScreen(), null);
					currentBoss.gotonextlevel = false;
					UnloadContent();
					imunloading = true;
				}
				else
				{
					CurrentLevel.Update(gameTime);
					currentBoss.update(gameTime);
					timerpp += gameTime.ElapsedGameTime.Milliseconds;
					sceneInterface.Update(gameTime);
					sceneInterface.ObjectManager.Optimize();
				}
				return;
			}
			if (FinalCurrBoss.bstate == bossState.Win || FinalCurrBoss.bstate == bossState.Nextlevel)
			{
				if (base.ScreenManager.audioManager.BGMCue.IsPlaying)
				{
					base.ScreenManager.audioManager.stopMusic();
				}
				bossplaynomusic = true;
			}
			else if (!bossplaynomusic && base.ScreenManager.audioManager != null)
			{
				if (base.ScreenManager.audioManager.BGMCue == null)
				{
					base.ScreenManager.audioManager.PlayMusic("boss");
				}
				else if (base.ScreenManager.audioManager.BGMCue.IsStopped)
				{
					base.ScreenManager.audioManager.PlayMusic("boss");
				}
			}
			if (FinalCurrBoss.explosion)
			{
				particleManager.submit(new ExplosionParticleSystem(base.ScreenManager.GraphicsDevice, firetext, particleeffect, FinalCurrBoss.bombola));
				base.ScreenManager.audioManager.playsound("explosion");
				FinalCurrBoss.explosion = false;
			}
			if (FinalCurrBoss.Splash)
			{
				particleManager.submit(new SharkWaterDrop(base.ScreenManager.GraphicsDevice, waterparticle, particleeffect, new Vector3(FinalCurrBoss.boss.X, 1.1f, FinalCurrBoss.boss.Z)));
				base.ScreenManager.audioManager.playsound("upwater");
				FinalCurrBoss.Splash = false;
			}
			if (FinalCurrBoss.Splash2)
			{
				sharkwater = new sharkwatertrail(base.ScreenManager.GraphicsDevice, waterparticle, particleeffect, new Vector3(FinalCurrBoss.bosship.X, 1.1f, FinalCurrBoss.bosship.Z));
				particleManager.submit(sharkwater);
				base.ScreenManager.audioManager.playsound("upwater");
				FinalCurrBoss.Splash2 = false;
			}
			if (sharkwater != null)
			{
				sharkwater.Emitter = new Vector3(FinalCurrBoss.boss.X, 1.1f, FinalCurrBoss.boss.Z);
				if (FinalCurrBoss.Actors[0].animationController.AnimationClip.Name != "Success2")
				{
					particleManager.ParticleList.Remove(sharkwater);
				}
			}
			if (FinalCurrBoss.scream)
			{
				base.ScreenManager.audioManager.playsound("Death");
				FinalCurrBoss.scream = false;
			}
			if (FinalCurrBoss.bclook != FinalCurrBoss.bcpos)
			{
				CurrentLevel.mCamera.lookat = FinalCurrBoss.bclook.Translation * 0.05f;
				CurrentLevel.mCamera.endlookat = FinalCurrBoss.bclook.Translation * 0.05f;
				CurrentLevel.mCamera.position = FinalCurrBoss.bcpos.Translation * 0.05f;
				CurrentLevel.mCamera.endposition = FinalCurrBoss.bcpos.Translation * 0.05f;
			}
			if (FinalCurrBoss.bstate == bossState.Null && FinalCurrBoss.gotonextlevel)
			{
				FinalCurrBoss.gotonextlevel = false;
				UnloadContent();
				imunloading = true;
				LoadingScreen.Load(base.ScreenManager, true, base.ControllingPlayer, new CreditsScreen());
			}
			else if (FinalCurrBoss.bstate == bossState.Null && FinalCurrBoss.gotogameover)
			{
				FinalCurrBoss.gotonextlevel = false;
				base.ScreenManager.AddScreen(new BackgroundScreen("gameover"), null);
				base.ScreenManager.AddScreen(new GameOverScreen(), null);
				UnloadContent();
				imunloading = true;
			}
			else
			{
				CurrentLevel.Update(gameTime);
				FinalCurrBoss.update(gameTime);
				timerpp += gameTime.ElapsedGameTime.Milliseconds;
				sceneInterface.Update(gameTime);
				sceneInterface.ObjectManager.Optimize();
			}
			return;
		}
		if (ep != null)
		{
			ExitPoint.World = ep.World;
			if (ExitPoint != null)
			{
				float num = shelly.Position.X - ep.World.Translation.X;
				float num2 = shelly.Position.Z - ep.World.Translation.Z;
				float radians = (float)Math.Atan2(0f - num, 0f - num2);
				compassrot = MathHelper.ToDegrees(radians);
			}
		}
		if (sharknexttoattack != null)
		{
			currentmaxawareness = sharknexttoattack.awareness;
		}
		CurrentLevel.Update(gameTime);
		if (shelly != null)
		{
			if (shelly.playerState == playerState.dead && shelly.gotogameover)
			{
				base.ScreenManager.AddScreen(new BackgroundScreen("gameover"), null);
				base.ScreenManager.AddScreen(new GameOverScreen(), null);
				shelly.gotogameover = false;
				UnloadContent();
				imunloading = true;
				return;
			}
			shelly.totalTime = CurrentLevel.totalTime;
		}
		if (gamePlayState == GamePlayState.Game)
		{
			if (shelly.playerState == playerState.game)
			{
				if (ep != null)
				{
					float num3 = Vector3.Distance(shelly.Position, ep.World.Translation);
					if (levelnumber == 3)
					{
						if (num3 < 52f)
						{
							bossalreadyreached = true;
							base.ScreenManager.reachedboss = true;
							gamePlayState = GamePlayState.Boss;
							base.ScreenManager.audioManager.stopallsfx();
							BossInit();
							return;
						}
					}
					else if (num3 < 15f)
					{
						bossalreadyreached = true;
						base.ScreenManager.reachedboss = true;
						gamePlayState = GamePlayState.Boss;
						base.ScreenManager.audioManager.stopallsfx();
						BossInit();
						return;
					}
				}
				if (!shelly.isfirstperson)
				{
					CurrentLevel.mCamera.Velocity = 0.015f;
					CurrentLevel.mCamera.endlookat = new Vector3(shelly.World.Translation.X, 2f, shelly.World.Translation.Z);
					CurrentLevel.mCamera.lookat = new Vector3(shelly.World.Translation.X, 2f, shelly.World.Translation.Z);
					CurrentLevel.mCamera.endposition = new Vector3(shelly.World.Translation.X, 2f, shelly.World.Translation.Z) + new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.Rotation.Y)) * 12f, 2f, (float)Math.Cos(MathHelper.ToRadians(shelly.Rotation.Y)) * -12f);
					CurrentLevel.mCamera.position = new Vector3(shelly.World.Translation.X, 2f, shelly.World.Translation.Z) + new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.Rotation.Y)) * 12f, 2f, (float)Math.Cos(MathHelper.ToRadians(shelly.Rotation.Y)) * -12f);
				}
				else
				{
					CurrentLevel.mCamera.Velocity = 0.015f;
					if ((double)shelly.Position.Y <= 12.5)
					{
						CurrentLevel.mCamera.endposition = shelly.World.Translation - new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.Rotation.Y)) * 0.8f, -13f, (float)Math.Cos(MathHelper.ToRadians(shelly.Rotation.Y)) * -0.8f);
					}
					CurrentLevel.mCamera.endlookat = shelly.World.Translation - new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.Rotation.Y)) * 0.8f, -13f, (float)Math.Cos(MathHelper.ToRadians(shelly.Rotation.Y)) * -0.8f) - Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(shelly.headrotation.Y), MathHelper.ToRadians(shelly.headrotation.X), MathHelper.ToRadians(shelly.headrotation.Z)).Forward;
				}
			}
			else
			{
				CurrentLevel.mCamera.Velocity = 0.015f;
				CurrentLevel.mCamera.endlookat = new Vector3(shelly.World.Translation.X, shelly.World.Translation.Y + 12.5f, shelly.World.Translation.Z);
				CurrentLevel.mCamera.lookat = new Vector3(shelly.World.Translation.X, shelly.World.Translation.Y + 12.5f, shelly.World.Translation.Z);
				CurrentLevel.mCamera.endposition = new Vector3(shelly.World.Translation.X, shelly.World.Translation.Y + 12.5f, shelly.World.Translation.Z) + new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.Rotation.Y)) * 12f, 2f, (float)Math.Cos(MathHelper.ToRadians(shelly.Rotation.Y)) * -12f);
				CurrentLevel.mCamera.position = new Vector3(shelly.World.Translation.X, shelly.World.Translation.Y + 12.5f, shelly.World.Translation.Z) + new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.Rotation.Y)) * 12f, 2f, (float)Math.Cos(MathHelper.ToRadians(shelly.Rotation.Y)) * -12f);
			}
			foreach (Shark shark in sharks)
			{
				checksharckCollision(shark);
				if (shark.animationController.AnimationClip.Name != "Idle")
				{
					shark.animationController.SwitchToClip(shark.skinnedModel.AnimationClips["Idle"]);
				}
				if (!shark.istargeting)
				{
					continue;
				}
				if (Vector3.Distance(shark.Position, shark.targetworld.Translation) <= 20f)
				{
					shark.nextrotation.Y = shark.Rotation.Y - 90f;
				}
				shark.targetworld = shelly.World;
				float num4 = Vector3.Distance(shark.targetworld.Translation, shark.World.Translation);
				if (num4 < 1f)
				{
					num4 = 1f;
				}
				if (shelly.movingfast)
				{
					if (!shelly.generatedparticle)
					{
						shelly.particletrail = new watertrail(base.ScreenManager.GraphicsDevice, waterparticle, particleeffect, new Vector3(shelly.Position.X, 1f, shelly.Position.Z));
						particleManager.submit(shelly.particletrail);
						shelly.generatedparticle = true;
					}
					if (shelly.particletrail != null)
					{
						shelly.particletrail.Emitter = new Vector3(shelly.World.Translation.X, 0.5f, shelly.World.Translation.Z);
					}
					base.ScreenManager.audioManager.stopsfx("breath");
					if (num4 != 0f)
					{
						shark.awareness += (float)gameTime.ElapsedGameTime.Milliseconds * 0.02f / num4;
					}
				}
				else
				{
					if (shelly.particletrail != null && shelly.generatedparticle)
					{
						particleManager.ParticleList.Remove(shelly.particletrail);
						shelly.generatedparticle = false;
					}
					if (shelly.moving)
					{
						if (shelly.nexthealth > 0f && num4 != 0f)
						{
							shark.awareness += (float)gameTime.ElapsedGameTime.Milliseconds * 0.003f / num4;
						}
					}
					else
					{
						shark.awareness -= (float)gameTime.ElapsedGameTime.Milliseconds * 0.01f;
						if (shark.awareness < 0f)
						{
							shark.awareness = 0f;
						}
					}
				}
				if (shark.awareness >= 3f)
				{
					gamePlayState = GamePlayState.Attacked;
					shelly.playerState = playerState.waitingqte;
					shelly.animationController.CrossFade(shelly.skinnedModel.AnimationClips["Idle"], new TimeSpan(0, 0, 0, 0, 250));
					shelly.movingfast = false;
					shelly.moving = false;
					shelly.Velocity = Vector3.Zero;
					AttackingShark = shark;
					AttackingSide = random.Next(0, 4);
					if (base.ScreenManager.audioManager.BGMCue.IsPlaying)
					{
						base.ScreenManager.audioManager.stopMusic();
					}
					base.ScreenManager.audioManager.PlayMusic("sharkattack");
					AttackingShark.behaviour = Behaviour.attack1;
					randomqte = random.Next(0, 4);
					if (randomqte > 3)
					{
						randomqte = 3;
					}
					randomcamera = random.Next(0, 5);
					AttackingShark.animationController.CrossFade(AttackingShark.skinnedModel.AnimationClips["Run"], new TimeSpan(0, 0, 0, 0, 250));
					break;
				}
			}
		}
		else
		{
			if (gamePlayState == GamePlayState.Attacked)
			{
				if (shelly.particletrail != null)
				{
					particleManager.ParticleList.Remove(shelly.particletrail);
					shelly.generatedparticle = false;
				}
				foreach (Shark shark2 in sharks)
				{
					if (shark2.istargeting)
					{
						shark2.targetworld = shelly.World;
						shark2.targetworld = shelly.World;
					}
					if (shark2 != AttackingShark)
					{
						checksharckCollision(shark2);
						shark2.awareness = 0f;
					}
				}
				if (AttackingShark != null)
				{
					shelly.lookat = AttackingShark.World;
					if (AttackingShark.imcolliding)
					{
						AttackingShark.imcolliding = false;
					}
					if (AttackingShark.Position.Y < -0.1f)
					{
						AttackingShark.nextrotation.X = -20f;
					}
					else
					{
						AttackingShark.nextrotation.X = 0f;
						if (Vector3.Distance(AttackingShark.Position, AttackingShark.targetworld.Translation) <= 20f && Behaviour.attack1 == AttackingShark.behaviour)
						{
							AttackingShark.animationController.Speed = 0.5f;
							AttackingShark.animationController.CrossFade(AttackingShark.skinnedModel.AnimationClips["Attack"], new TimeSpan(0, 0, 0, 0, 100));
							AttackingShark.animationController.LoopEnabled = false;
							gamePlayState = GamePlayState.QTE;
							shelly.playerState = playerState.qte;
							AttackingShark.behaviour = Behaviour.hit;
						}
					}
				}
			}
			if (gamePlayState == GamePlayState.QTE && AttackingShark != null)
			{
				foreach (Shark shark3 in sharks)
				{
					if (shark3.istargeting)
					{
						shark3.targetworld = shelly.World;
						shark3.targetworld = shelly.World;
					}
					if (shark3 != AttackingShark)
					{
						checksharckCollision(shark3);
						shark3.awareness = 0f;
					}
				}
				if (timeranimation <= (float)AttackingShark.animationController.AnimationClip.Duration.Milliseconds - (float)AttackingShark.animationController.AnimationClip.Duration.Milliseconds * 0.1f)
				{
					timeranimation += gameTime.ElapsedGameTime.Milliseconds;
					if (shelly.dir != -1)
					{
						if (shelly.dir == randomqte)
						{
							success = true;
						}
						else
						{
							success = false;
						}
					}
				}
				else
				{
					timeranimation = 0f;
					currentmaxawareness = 0f;
					if (shelly.playerState != playerState.Dodge && shelly.playerState != playerState.Hit)
					{
						if (success)
						{
							shelly.dir = -1;
							shelly.Velocity = 2f * -Vector3.Normalize(AttackingShark.World.Left);
							shelly.playerState = playerState.Dodge;
							shelly.animationController.CrossFade(shelly.skinnedModel.AnimationClips["Dodge"], new TimeSpan(0, 0, 0, 0, 100));
							shelly.animationController.LoopEnabled = false;
						}
						else if (!shelly.alreadyhitted)
						{
							shelly.alreadyhitted = true;
							shelly.dir = -1;
							base.ScreenManager.audioManager.playsound("Hit");
							shelly.nexthealth -= 34f;
							shelly.Velocity = 2f * -Vector3.Normalize(AttackingShark.World.Forward);
							shelly.playerState = playerState.Hit;
							shelly.animationController.CrossFade(shelly.skinnedModel.AnimationClips["Hit"], new TimeSpan(0, 0, 0, 0, 100));
							bleeding = new Splat(base.ScreenManager.GraphicsDevice, splat, particleeffect, new Vector3(shelly.World.Translation.X, -2.5f, shelly.World.Translation.Z));
							particleManager.submit(bleeding);
							shelly.animationController.LoopEnabled = false;
						}
					}
					if (AttackingShark.animationController.HasFinished)
					{
						shelly.alreadyhitted = false;
						shelly.dir = -1;
						AttackingShark.animationController.Speed = 1f;
						gamePlayState = GamePlayState.Game;
						success = false;
						AttackingShark.awareness = 0f;
						AttackingShark.behaviour = Behaviour.patrol;
						AttackingShark.animationController.CrossFade(AttackingShark.skinnedModel.AnimationClips["Idle"], new TimeSpan(0, 0, 0, 0, 100));
						AttackingShark.animationController.LoopEnabled = true;
						AttackingShark = null;
					}
				}
			}
			if ((gamePlayState == GamePlayState.QTE || gamePlayState == GamePlayState.Attacked) && AttackingShark != null)
			{
				if (randomcamera <= 0)
				{
					CurrentLevel.mCamera.Velocity = 0.015f;
					CurrentLevel.mCamera.endposition = new Vector3(shelly.World.Translation.X, 2f, shelly.World.Translation.Z) + new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.nextrotation)) * 12f, 2f, (float)Math.Cos(MathHelper.ToRadians(shelly.nextrotation)) * -12f);
					CurrentLevel.mCamera.position = new Vector3(shelly.World.Translation.X, 2f, shelly.World.Translation.Z) + new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.nextrotation)) * 12f, 2f, (float)Math.Cos(MathHelper.ToRadians(shelly.nextrotation)) * -12f);
					CurrentLevel.mCamera.endlookat = AttackingShark.World.Translation;
					CurrentLevel.mCamera.lookat = AttackingShark.World.Translation;
				}
				if (randomcamera == 1)
				{
					if (Vector3.Distance(AttackingShark.Position, AttackingShark.targetworld.Translation) <= 20f)
					{
						randomcamera = 2;
						CurrentLevel.mCamera.Velocity = 0.015f;
						CurrentLevel.mCamera.endposition = new Vector3(shelly.World.Translation.X, -10f, shelly.World.Translation.Z) + new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.nextrotation)) * 20f, -10f, (float)Math.Cos(MathHelper.ToRadians(shelly.nextrotation)) * -20f);
						CurrentLevel.mCamera.position = new Vector3(shelly.World.Translation.X, -10f, shelly.World.Translation.Z) + new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.nextrotation)) * 20f, -10f, (float)Math.Cos(MathHelper.ToRadians(shelly.nextrotation)) * -20f);
						CurrentLevel.mCamera.endlookat = AttackingShark.World.Translation;
						CurrentLevel.mCamera.lookat = AttackingShark.World.Translation;
					}
					else
					{
						CurrentLevel.mCamera.Velocity = 0.015f;
						CurrentLevel.mCamera.endlookat = AttackingShark.World.Translation - Vector3.Normalize(AttackingShark.World.Forward) * 10f;
						CurrentLevel.mCamera.endposition = AttackingShark.World.Translation - Vector3.Normalize(AttackingShark.World.Forward) * 9f;
						CurrentLevel.mCamera.lookat = AttackingShark.World.Translation - Vector3.Normalize(AttackingShark.World.Forward) * 10f;
						CurrentLevel.mCamera.position = AttackingShark.World.Translation - Vector3.Normalize(AttackingShark.World.Forward) * 9f;
					}
				}
				if (randomcamera == 2)
				{
					CurrentLevel.mCamera.Velocity = 0.015f;
					CurrentLevel.mCamera.endposition = new Vector3(shelly.World.Translation.X, -10f, shelly.World.Translation.Z) + new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.nextrotation)) * 20f, -10f, (float)Math.Cos(MathHelper.ToRadians(shelly.nextrotation)) * -20f);
					CurrentLevel.mCamera.position = new Vector3(shelly.World.Translation.X, -10f, shelly.World.Translation.Z) + new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.nextrotation)) * 20f, -10f, (float)Math.Cos(MathHelper.ToRadians(shelly.nextrotation)) * -20f);
					CurrentLevel.mCamera.endlookat = AttackingShark.World.Translation;
					CurrentLevel.mCamera.lookat = AttackingShark.World.Translation;
				}
				if (randomcamera == 3)
				{
					CurrentLevel.mCamera.Velocity = 0.015f;
					CurrentLevel.mCamera.endlookat = AttackingShark.World.Translation - Vector3.Normalize(AttackingShark.World.Forward) * 10f;
					CurrentLevel.mCamera.endposition = new Vector3(0f, 5f, 0f) + AttackingShark.World.Translation + Vector3.Normalize(AttackingShark.World.Forward) * 10f;
					CurrentLevel.mCamera.lookat = AttackingShark.World.Translation - Vector3.Normalize(AttackingShark.World.Forward) * 10f;
					CurrentLevel.mCamera.position = new Vector3(0f, 5f, 0f) + AttackingShark.World.Translation + Vector3.Normalize(AttackingShark.World.Forward) * 10f;
				}
				if (randomcamera >= 4)
				{
					CurrentLevel.mCamera.Velocity = 0.03f;
					CurrentLevel.mCamera.endposition = new Vector3(shelly.World.Translation.X, 30f, shelly.World.Translation.Z) + new Vector3(0f - (float)Math.Sin(MathHelper.ToRadians(shelly.nextrotation)) * 2f, 10f, (float)Math.Cos(MathHelper.ToRadians(shelly.nextrotation)) * -2f);
					CurrentLevel.mCamera.endlookat = new Vector3(shelly.World.Translation.X, 0f, shelly.World.Translation.Z);
				}
			}
		}
		timerpp += gameTime.ElapsedGameTime.Milliseconds;
		if (bleeding != null && shelly != null)
		{
			bleeding.Emitter = new Vector3(shelly.World.Translation.X, -2.5f, shelly.World.Translation.Z);
		}
		sceneInterface.Update(gameTime);
		sceneInterface.ObjectManager.Optimize();
	}

	private void initCutScene()
	{
		if (levelnumber == 1)
		{
			foreach (Actor actor in introlevel1.Actors)
			{
				sceneInterface.ObjectManager.Submit(actor);
			}
			CurrentLevel.initialize();
			waterWorldPlane = new Plane(Vector3.Down, 1.2f);
			introlevel1.bcpos = introlevel1.Actors[0].animationController.GetBoneAbsoluteTransform("campos");
			introlevel1.bclook = introlevel1.Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
			sceneInterface.ObjectManager.Submit(EntryPoint);
			return;
		}
		foreach (Actor actor2 in introlevel2.Actors)
		{
			sceneInterface.ObjectManager.Submit(actor2);
		}
		CurrentLevel.initialize();
		waterWorldPlane = new Plane(Vector3.Down, 1.2f);
		introlevel2.bcpos = introlevel2.Actors[0].animationController.GetBoneAbsoluteTransform("campos");
		introlevel2.bclook = introlevel2.Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
	}

	private void BossInit()
	{
		sceneInterface.ObjectManager.Remove(shelly);
		foreach (Shark shark in sharks)
		{
			sceneInterface.ObjectManager.Remove(shark);
		}
		sceneInterface.ObjectManager.Remove(ExitPoint);
		sceneInterface.ObjectManager.Remove(ep);
		if (levelnumber != 3)
		{
			sceneInterface.ObjectManager.Remove(EntryPoint);
			sceneInterface.ObjectManager.Remove(entryp);
			foreach (Actor actor in currentBoss.Actors)
			{
				sceneInterface.ObjectManager.Submit(actor);
			}
			currentBoss.bcpos = currentBoss.Actors[0].animationController.GetBoneAbsoluteTransform("campos");
			currentBoss.bclook = currentBoss.Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
			return;
		}
		foreach (Actor actor2 in FinalCurrBoss.Actors)
		{
			sceneInterface.ObjectManager.Submit(actor2);
		}
		FinalCurrBoss.bcpos = FinalCurrBoss.Actors[0].animationController.GetBoneAbsoluteTransform("campos");
		FinalCurrBoss.bclook = FinalCurrBoss.Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen: false);
		if (coveredByOtherScreen)
		{
			pauseAlpha = Math.Min(pauseAlpha + 1f / 32f, 1f);
		}
		else
		{
			pauseAlpha = Math.Max(pauseAlpha - 1f / 32f, 0f);
		}
		if (!imunloading && !Guide.IsVisible && base.IsActive)
		{
			if (base.ScreenManager.audioManager.BGMCue.IsPaused)
			{
				base.ScreenManager.audioManager.BGMCue.Resume();
			}
			updatemanager(gameTime);
			if (CurrentLevel != null)
			{
				waterEffect.Parameters["time"].SetValue(CurrentLevel.totalTime);
				waterWorldPlane = new Plane(Vector3.Down, 1.2f + (float)Math.Sin((double)CurrentLevel.totalTime * 0.0001));
			}
		}
	}

	public override void HandleInput(InputState input)
	{
		if (imunloading)
		{
			return;
		}
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		int value = (int)base.ControllingPlayer.Value;
		_ = input.CurrentKeyboardStates[value];
		GamePadState gamePadState = input.CurrentGamePadStates[value];
		bool flag = !gamePadState.IsConnected && input.GamePadWasConnected[value];
		if (GamePlayState.Journal == gamePlayState && input.IsMenuSelect(base.ControllingPlayer, out var _))
		{
			aispressed = true;
		}
		if (!input.IsPauseGame(base.ControllingPlayer) && !flag)
		{
			return;
		}
		if (gamePlayState == GamePlayState.Intro)
		{
			initCutScene();
			gamePlayState = GamePlayState.InitCutscene;
			if (vp.State == MediaState.Playing)
			{
				vp.Stop();
			}
		}
		else
		{
			base.ScreenManager.AddScreen(new PauseMenuScreen(new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2, base.ScreenManager.GraphicsDevice.Viewport.Height / 3)), base.ControllingPlayer);
			if (base.ScreenManager.audioManager.BGMCue != null && base.ScreenManager.audioManager.BGMCue.IsPlaying)
			{
				base.ScreenManager.audioManager.BGMCue.Pause();
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		if (imunloading)
		{
			return;
		}
		base.ScreenManager.GraphicsDevice.SetRenderTarget(null);
		base.ScreenManager.GraphicsDevice.SetRenderTarget(modelrenderer);
		sceneInterface.ObjectManager.Remove(water);
		sceneState.BeginFrameRendering(CurrentLevel.mCamera.View, CurrentLevel.mCamera.Projection, gameTime, CurrentLevel.env, reflectionRefractionFrameBuffers, renderingtoscreen: false);
		refractionTarget.BeginFrameRendering(sceneState);
		base.ScreenManager.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Gray, 1f, 0);
		RenderTarget(refractionTarget);
		refractionTarget.EndFrameRendering();
		sceneState.EndFrameRendering();
		sceneState.BeginFrameRendering(CurrentLevel.mCamera.View, CurrentLevel.mCamera.Projection, gameTime, CurrentLevel.env, reflectionRefractionFrameBuffers, renderingtoscreen: false);
		waterReflectionTarget.BeginFrameRendering(sceneState, waterWorldPlane);
		base.ScreenManager.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Gray, 1f, 0);
		RenderTarget(waterReflectionTarget);
		waterReflectionTarget.EndFrameRendering();
		sceneState.EndFrameRendering();
		sceneInterface.ObjectManager.Submit(water);
		sceneState.BeginFrameRendering(CurrentLevel.mCamera.View, CurrentLevel.mCamera.Projection, gameTime, CurrentLevel.env, frameBuffers, renderingtoscreen: true);
		sceneInterface.BeginFrameRendering(sceneState);
		base.ScreenManager.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1f, 0);
		sceneInterface.RenderManager.Render();
		base.ScreenManager.GraphicsDevice.BlendState = TrueAdditiveBlend;
		base.ScreenManager.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		RenderMesh(water, sceneState, waterEffect, waterReflectionTarget.GetTexture(), refractionTarget.GetTexture());
		base.ScreenManager.GraphicsDevice.BlendState = BlendState.Opaque;
		base.ScreenManager.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
		sceneInterface.EndFrameRendering();
		sceneState.EndFrameRendering();
		base.ScreenManager.GraphicsDevice.SetRenderTarget(null);
		Texture2D refltext = modelrenderer;
		CurrentLevel.refltext = refltext;
		postprocess.Parameters["timervalue"].SetValue(timerpp * 0.01f);
		postprocess.Parameters["tint"].SetValue(new Vector4(0.3f, 0.4f, 0.6f, 1f));
		base.ScreenManager.GraphicsDevice.SetRenderTarget(base.ScreenManager.rt);
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.AnisotropicWrap, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwise, null);
		spriteBatch.Draw(modelrenderer, Vector2.Zero, Color.White);
		spriteBatch.End();
		if (CurrentLevel.mCamera.position.Y <= 0.5f)
		{
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.AnisotropicWrap, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwise, postprocess);
			spriteBatch.Draw(modelrenderer, new Rectangle(0, 0, 1280, 720), new Color(1, 1, 1, 1));
			spriteBatch.End();
		}
		spriteBatch.Begin();
		if (gamePlayState != GamePlayState.Boss && gamePlayState != GamePlayState.InitCutscene)
		{
			spriteBatch.Draw(EnergyFrame, new Vector2(64f, base.ScreenManager.GraphicsDevice.Viewport.Height - EnergyFrame.Height - 32), Color.White);
			spriteBatch.Draw(Stamina, new Vector2(64f, base.ScreenManager.GraphicsDevice.Viewport.Height - EnergyFrame.Height - 32), new Rectangle(0, 0, (int)(shelly.Stamina * (float)Stamina.Width) / 22, Stamina.Height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
			if (ExitPoint != null)
			{
				spriteBatch.Draw(Compass, new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width - Compass.Width - 32, base.ScreenManager.GraphicsDevice.Viewport.Height - Compass.Height - 32), Color.White);
				Vector2 origin = new Vector2(Arrow.Width / 2, Arrow.Height / 2);
				spriteBatch.Draw(Arrow, new Vector2((float)base.ScreenManager.GraphicsDevice.Viewport.Width - origin.X - 32f, (float)base.ScreenManager.GraphicsDevice.Viewport.Height - origin.Y - 32f), new Rectangle(0, 0, Arrow.Width, Arrow.Height), Color.White, MathHelper.ToRadians(shelly.Rotation.Y - compassrot), origin, 1f, SpriteEffects.None, 1f);
			}
		}
		if (GamePlayState.Journal == gamePlayState)
		{
			spriteBatch.Draw(journal, Vector2.Zero, Color.White);
		}
		if (gamePlayState == GamePlayState.Boss)
		{
			if (levelnumber == 3)
			{
				FinalCurrBoss.draw(spriteBatch, gameTime);
			}
			else
			{
				currentBoss.draw(spriteBatch, gameTime);
			}
			if (levelnumber == 3 && FinalCurrBoss != null)
			{
				if (FinalCurrBoss.batt1)
				{
					spriteBatch.DrawString(subtitles, batt1, new Vector2((float)(base.ScreenManager.GraphicsDevice.Viewport.Width / 2) - subtitles.MeasureString(batt1).X / 2f * 0.8f, 640f), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
				}
				if (FinalCurrBoss.batt2)
				{
					spriteBatch.DrawString(subtitles, batt2, new Vector2((float)(base.ScreenManager.GraphicsDevice.Viewport.Width / 2) - subtitles.MeasureString(batt2).X / 2f * 0.8f, 640f), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
				}
				if (FinalCurrBoss.batt3)
				{
					spriteBatch.DrawString(subtitles, batt3, new Vector2((float)(base.ScreenManager.GraphicsDevice.Viewport.Width / 2) - subtitles.MeasureString(batt3).X / 2f * 0.8f, 640f), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
				}
			}
		}
		if (gamePlayState == GamePlayState.InitCutscene)
		{
			if (levelnumber == 1)
			{
				introlevel1.draw(spriteBatch, gameTime);
			}
			if (levelnumber == 2)
			{
				introlevel2.draw(spriteBatch, gameTime);
			}
		}
		if (gamePlayState == GamePlayState.QTE)
		{
			spriteBatch.Draw(QTE[randomqte], new Vector2(base.ScreenManager.GraphicsDevice.Viewport.Width / 2 - QTE[randomqte].Width / 2, base.ScreenManager.GraphicsDevice.Viewport.Height / 3 - QTE[randomqte].Height / 2), Color.White);
		}
		if (gamePlayState == GamePlayState.Intro)
		{
			Texture2D texture = vp.GetTexture();
			spriteBatch.Draw(texture, new Rectangle(0, 0, 1280, 720), Color.White);
		}
		spriteBatch.End();
		if (base.TransitionPosition > 0f || pauseAlpha > 0f)
		{
			float alpha = MathHelper.Lerp(1f - base.TransitionAlpha, 1f, pauseAlpha / 2f);
			base.ScreenManager.FadeBackBufferToBlack(alpha);
		}
	}

	private void RenderMesh(SceneObject sceneObject, SceneState sceneState, SasEffect effect, Texture2D reflecttexture, Texture2D refracttexture)
	{
		effect.View = sceneState.View;
		effect.Projection = sceneState.Projection;
		effect.Parameters["ReflectTexture"].SetValue(reflecttexture);
		effect.Parameters["RefractTexture"].SetValue(refracttexture);
		EffectPassCollection passes = effect.CurrentTechnique.Passes;
		for (int i = 0; i < passes.Count; i++)
		{
			EffectPass effectPass = passes[i];
			for (int j = 0; j < sceneObject.RenderableMeshes.Count; j++)
			{
				RenderableMesh renderableMesh = sceneObject.RenderableMeshes[j];
				effect.World = renderableMesh.World;
				Effect effect2 = renderableMesh.Effect;
				if (effect2 is BaseMaterialEffect)
				{
					effect.Parameters["BumpTexture"].SetValue((effect2 as BaseMaterialEffect).NormalMapTexture);
				}
				else
				{
					effect.Parameters["BumpTexture"].SetValue(effect2.Parameters["NormalMap"].GetValueTexture2D());
				}
				effectPass.Apply();
				base.ScreenManager.GraphicsDevice.SetVertexBuffer(renderableMesh.VertexBuffer, renderableMesh.VertexStreamOffset);
				base.ScreenManager.GraphicsDevice.Indices = renderableMesh.IndexBuffer;
				base.ScreenManager.GraphicsDevice.DrawIndexedPrimitives(renderableMesh.PrimitiveType, renderableMesh.VertexBase, 0, renderableMesh.VertexCount, renderableMesh.ElementStart, renderableMesh.PrimitiveCount);
			}
		}
	}

	private void RenderTarget(RenderTargetHelper refractionTarget)
	{
		sceneInterface.ApplyPreferences(refractionTarget.Preferences);
		sceneInterface.BeginFrameRendering(refractionTarget.SceneState);
		sceneInterface.RenderManager.Render();
		sceneInterface.EndFrameRendering();
	}
}
