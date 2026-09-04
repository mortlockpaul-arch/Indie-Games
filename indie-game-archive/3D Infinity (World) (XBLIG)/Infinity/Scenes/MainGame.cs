#define TRACE
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infinity.GameObjects;
using InfinityLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using XnaLibrary;
using XnaLibrary.Input;

namespace Infinity.Scenes;

public class MainGame : AnaglyphScene
{
	private enum GamePhase
	{
		Play,
		GameOver,
		BossDestruct,
		Clear,
		Loading
	}

	[Flags]
	public enum SoundFlag
	{
		Nothing = 0,
		Vulcan = 1,
		Damage = 2,
		NoDamage = 4,
		Break = 8,
		LockOn = 0x10,
		Missile = 0x20
	}

	private const string BossBgAsset = "Models/Models/boss/boss_bg";

	private const int ExplosionFireCount = 5;

	private const int ExplosionSmokeCount = 10;

	private const int ExplosionFragmentCount = 10;

	private const int ITEM_MAX = 8;

	private const int EnemyArraySize = 30;

	private Color WhiteOutColor;

	private Color PauseColor;

	private readonly Vector2 ImageManual1Position;

	private readonly Vector2 ImageManual2Position;

	private Vector2 ImagePausePosition;

	private Vector2 ImagePauseQuitPosition;

	private SpriteBatch spriteBatch;

	private Texture2D image_manual1;

	private Texture2D image_manual2;

	private Texture2D image_pause;

	private Texture2D image_pause_quit;

	private Stage stage;

	private Player player;

	private XSIModel[] enemyModels;

	private XSIModel[] enemyColModels;

	private XSIModel sightLockOn;

	private XSIModel screenScore;

	private XSIModel screenLifegauge;

	private XSIModel[] scoreModels;

	private XSIModel gameOver;

	private XSIModel loading;

	private Boss boss;

	private ParticleSystem particleExplosionSmoke;

	private ParticleSystem particleExplosionFire;

	private ParticleSystem particleExplosionFragment;

	private ParticleSystem particleExplosionFragment2;

	private ParticleSystem particleHormingMissileSmoke;

	private ParticleSystem particleDestructionSmoke;

	private ParticleSystem particleDestructionFire;

	private ParticleSystem particleBurner;

	private ParticleSystem particleBreakSmoke;

	private CustomParticleSystem vulcanParticleSystem;

	private int vulcanCount;

	private int vulcanInterval;

	private MissileManager missileManager;

	private CustomParticleSystem enemyShotParticleSystem;

	private Item[] items;

	private Dictionary<string, Action> itemEffects;

	private Zako[] enemies;

	private GameSettings gameSettings;

	private EnemySettings[] enemySettings;

	private BoundingSphere[] enemySpheres;

	private BoundingSphere vulcanSphere;

	private Ray sightRay;

	private BoundingFrustum sightFrustum;

	private int score;

	private int currentHp;

	private int viewHp;

	private int lap;

	private int stageIndex;

	private int chapterIndex;

	private StageSettings[] stageSettings;

	private EnemyMoveSettings[] enemyMoveSettings;

	private EnemyShotSettings[] enemyShotSettings;

	private GamePhase gamePhase;

	private bool isReverb;

	private float reverbValue;

	private float whiteOut;

	private Cue bgmMain;

	private Cue bgmBoss;

	private float bgmMainVolume;

	private float bgmBossVolume;

	private SoundFlag soundFlag;

	public StageSettings CurrentStage => stageSettings[stageIndex];

	public ChapterSettings CurrentChapter => CurrentStage.Chapters[chapterIndex];

	public DifficultSettings Difficulty => gameSettings.Difficulty[Global.SaveData.DifficultIndex];

	public bool IsPause { get; set; }

	public MainGame(Game game)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		WhiteOutColor = Color.White;
		PauseColor = new Color((byte)0, (byte)0, (byte)0, (byte)128);
		ImageManual1Position = new Vector2(968f, 590f);
		ImageManual2Position = new Vector2(58f, 590f);
		vulcanSphere = new BoundingSphere(Vector3.Zero, 3f);
		base._002Ector(game);
		base.update += SceneUpdate;
		base.draw += SceneDraw;
	}

	public override void Initialize()
	{
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		gameSettings = base.Content.Load<GameSettings>("GameSettings");
		enemySettings = base.Content.Load<EnemySettings[]>("EnemySettings");
		enemyMoveSettings = base.Content.Load<EnemyMoveSettings[]>("EnemyMoveSettings");
		enemyShotSettings = base.Content.Load<EnemyShotSettings[]>("EnemyShotSettings");
		stageSettings = base.Content.Load<StageSettings[]>("StageSettings");
		Global.GameSpeed = 1f;
		lap = 0;
		stageIndex = 0;
		chapterIndex = 0;
		viewHp = 100;
		whiteOut = 1f;
		gamePhase = GamePhase.Play;
		image_manual1 = base.Content.Load<Texture2D>("Textures/image_manual1");
		image_manual2 = base.Content.Load<Texture2D>("Textures/image_manual2");
		image_pause = base.Content.Load<Texture2D>("Textures/image_pause");
		image_pause_quit = base.Content.Load<Texture2D>("Textures/image_pause_quit");
		spriteBatch = new SpriteBatch(base.Game.GraphicsDevice);
		InitializeModels();
		InitializeItems();
		InitializeEnemy();
		InitializeParticles();
		InitializeBoundingSphere();
		InitializeStageModel(CurrentChapter, loop: false);
		missileManager = new MissileManager(base.Game);
		((GameComponent)missileManager).Initialize();
		missileManager.SmokeParticle = particleHormingMissileSmoke;
		missileManager.EntrySE += delegate(SoundFlag flag)
		{
			EntrySE(flag);
		};
		missileManager.Explosion += delegate(Vector3 position)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			CreateExplosion(position);
		};
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)(Global.ScreenArea.Width >> 1), (float)(Global.ScreenArea.Height >> 1));
		ImagePausePosition = val - new Vector2((float)(image_pause.Width >> 1), (float)(image_pause.Height >> 1));
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(0f, 192f);
		ImagePauseQuitPosition = val - new Vector2((float)(image_pause_quit.Width >> 1), (float)(image_pause_quit.Height >> 1)) + val2;
		GC.Collect();
		base.Initialize();
	}

	private void InitializeItems()
	{
		items = new Item[8];
		for (int i = 0; i < items.Length; i++)
		{
			items[i] = new Item(base.Game);
		}
		itemEffects = new Dictionary<string, Action>();
		itemEffects.Add("energy", delegate
		{
			player.Restore(gameSettings.ItemRestore);
		});
	}

	private void InitializeModels()
	{
		player = new Player(base.Game);
		screenScore = new XSIModel("Models/Models/screen/screen_score", base.Content);
		screenLifegauge = new XSIModel("Models/Models/screen/screen_lifegauge", base.Content);
		gameOver = new XSIModel("Models/Models/screen/screen_gameover", base.Content);
		gameOver.Finished += delegate
		{
			base.SceneManager.AddScene(new Title(base.Game, Title.Phase.SelectMenu));
			FadeOut();
		};
		loading = new XSIModel("Models/Models/screen/screen_loading", base.Content);
		enemyModels = new XSIModel[3]
		{
			new XSIModel("Models/Models/enemy/enemy01", base.Content),
			new XSIModel("Models/Models/enemy/enemy02", base.Content),
			new XSIModel("Models/Models/enemy/enemy03", base.Content)
		};
		enemyColModels = new XSIModel[3]
		{
			new XSIModel("Models/Models/enemy/enemy01_col", base.Content),
			new XSIModel("Models/Models/enemy/enemy02_col", base.Content),
			new XSIModel("Models/Models/enemy/enemy03_col", base.Content)
		};
		XSIModel[] array = enemyModels;
		foreach (XSIModel xSIModel in array)
		{
			xSIModel.Play(isLoop: true);
		}
		sightLockOn = new XSIModel("Models/Models/player/player_lockon", base.Content);
		sightLockOn.Play();
		screenScore.Play(isLoop: true);
		screenLifegauge.Play(isLoop: false);
		scoreModels = new XSIModel[10];
		for (int num2 = 0; num2 < 10; num2++)
		{
			string assetPath = $"Models/Models/font_number/font_num{num2}";
			scoreModels[num2] = new XSIModel(assetPath, base.Content);
			scoreModels[num2].Play(isLoop: true);
		}
		player.SoundPlay += delegate(string cueName)
		{
			base.Sound.PlaySE(cueName);
		};
		player.Vulcan += delegate
		{
			if (whiteOut == 0f && vulcanInterval <= 0)
			{
				CreateVulcan(vulcanParticleSystem.particles);
				vulcanInterval = 3;
			}
			else
			{
				vulcanInterval--;
			}
		};
		player.Missile += delegate
		{
			CreateHormingMissile();
		};
		player.Crush += delegate
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			SetShaker();
			base.PadVibration[Global.CurrentPlayer] = gameSettings.CrushPadVibration;
			PlaySE("SE05");
		};
		Player obj = player;
		obj.Destruction = (Action<int>)Delegate.Combine(obj.Destruction, (Action<int>)delegate
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			CreateExplosion(player.GetPosition());
			SetupGameOver();
		});
	}

	private void InitializeStageModel(ChapterSettings chapterSettings, bool loop)
	{
		ContentManager content = Global.AsyncLoader.Content;
		stage = new Stage(base.Game, content, chapterSettings, loop);
		chapterSettings.Dispose();
		chapterSettings.Generate += CreateEnemy;
		chapterSettings.SoundPlay += delegate(string cueName)
		{
			PlaySE(cueName);
		};
		chapterSettings.SoundReverb += delegate(bool reverb)
		{
			isReverb = reverb;
		};
		chapterSettings.Item += delegate(string name, Vector3 position)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			CreateItem(name, position);
		};
		stage.Finished += StageAnimationFinished;
		if (player != null)
		{
			player.IsHandling = true;
		}
	}

	private void StageAnimationFinished()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		chapterIndex++;
		if (chapterIndex < stageSettings.Length)
		{
			InitializeStageModel(CurrentChapter, loop: false);
			return;
		}
		InitializeStageModel(new ChapterSettings
		{
			BgModelAsset = "Models/Models/boss/boss_bg"
		}, loop: true);
		if (boss == null)
		{
			CreateEnemy(EnemyType.Boss, 0, null, new Vector3(0f, -30f, -100f));
		}
	}

	private void InitializeEnemy()
	{
		enemies = new Zako[30];
		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i] = new Zako(base.Game);
		}
	}

	private void InitializeParticles()
	{
		particleExplosionSmoke = new ParticleSystem(base.Game, base.Content, "Particle/ExplosionSmokeSettings");
		particleExplosionFire = new ParticleSystem(base.Game, base.Content, "Particle/ExplosionSettings");
		particleExplosionFragment = new ParticleSystem(base.Game, base.Content, "Particle/ExplosionFragmentSettings");
		particleExplosionFragment2 = new ParticleSystem(base.Game, base.Content, "Particle/ExplosionFragment2Settings");
		particleHormingMissileSmoke = new ParticleSystem(base.Game, base.Content, "Particle/HormingMissileSmokeSettings");
		particleDestructionSmoke = new ParticleSystem(base.Game, base.Content, "Particle/DestructionSmokeSettings");
		particleDestructionFire = new ParticleSystem(base.Game, base.Content, "Particle/DestructionSettings");
		particleBurner = new ParticleSystem(base.Game, base.Content, "Particle/BurnerSettings");
		particleBreakSmoke = new ParticleSystem(base.Game, base.Content, "Particle/BreakSmokeSettings");
		((GameComponent)particleExplosionSmoke).Initialize();
		((GameComponent)particleExplosionFire).Initialize();
		((GameComponent)particleExplosionFragment).Initialize();
		((GameComponent)particleExplosionFragment2).Initialize();
		((GameComponent)particleHormingMissileSmoke).Initialize();
		((GameComponent)particleDestructionSmoke).Initialize();
		((GameComponent)particleDestructionFire).Initialize();
		((GameComponent)particleBurner).Initialize();
		((GameComponent)particleBreakSmoke).Initialize();
		vulcanParticleSystem = new CustomParticleSystem(base.Game, base.Content, gameSettings.ValcunMaxCount, "Models/Textures/effect_image/player_shot");
		for (int i = 0; i < vulcanParticleSystem.particles.Length; i++)
		{
			DisposeVulcan(ref vulcanParticleSystem.particles[i]);
		}
		enemyShotParticleSystem = new CustomParticleSystem(base.Game, base.Content, 64, "Models/Textures/effect_image/enemy_shot");
		for (int j = 0; j < enemyShotParticleSystem.particles.Length; j++)
		{
			DisposeVulcan(ref enemyShotParticleSystem.particles[j]);
		}
	}

	private void InitializeBoundingSphere()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		enemySpheres = (BoundingSphere[])(object)new BoundingSphere[enemyColModels.Length];
		for (int i = 0; i < enemySpheres.Length; i++)
		{
			ref BoundingSphere reference = ref enemySpheres[i];
			reference = ((ReadOnlyCollection<ModelMesh>)(object)enemyColModels[i].CrosswalkModel.Meshes)[0].BoundingSphere;
		}
	}

	public override void Dispose()
	{
		if (Global.AsyncLoader.LoadThread != null)
		{
			Global.AsyncLoader.LoadThread.Join();
		}
		if (score > Global.SaveData.HiScore)
		{
			Global.SaveData.HiScore = score;
			Global.Save(base.Storage);
		}
		base.Content.Unload();
		spriteBatch.Dispose();
		((GameComponent)particleExplosionSmoke).Dispose();
		((GameComponent)particleExplosionFire).Dispose();
		base.Sound.Stop(bgmMain);
		base.Sound.Stop(bgmBoss);
		base.Sound.SetReverb(0f);
		base.Dispose();
	}

	private void DisposeParticles(ParticleVertex[] particles)
	{
		for (int i = 0; i < particles.Length; i++)
		{
			DisposeVulcan(ref particles[i]);
		}
	}

	private void SceneUpdate(object sender, GameTime gameTime)
	{
		_ = gameTime.ElapsedGameTime;
		TimeSpan elapsedGameTime = new TimeSpan((long)((float)gameTime.ElapsedGameTime.Ticks * Global.GameSpeed));
		UpdatePause(gameTime);
		if (IsPause)
		{
			return;
		}
		UpdateSound();
		if (gamePhase == GamePhase.Loading)
		{
			loading.Update(gameTime);
			return;
		}
		if (fadePhase != FadePhase.In)
		{
			if (fadePhase == FadePhase.Main)
			{
				UpdateMain(gameTime);
			}
			else
			{
				_ = fadePhase;
				_ = 2;
			}
		}
		if (chapterIndex < stageSettings.Length)
		{
			CurrentChapter.Update(elapsedGameTime);
		}
		UpdateSightFrustum(gameTime);
		UpdateModels(gameTime);
		UpdateImtes(elapsedGameTime);
		UpdateHormingMissile(gameTime);
		UpdateParticles(gameTime);
		UpdateShaker(gameTime);
		UpdateEnemies(gameTime.ElapsedGameTime);
	}

	private void UpdatePause(GameTime gameTime)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		VirtualPadState virtualPadState = base.Input[Global.CurrentPlayer];
		VirtualPadButtons buttons = virtualPadState.Buttons;
		if (InputState.IsPush(buttons.Start))
		{
			PlaySE("SE17");
			IsPause = !IsPause;
		}
		if (IsPause && InputState.IsPush(buttons.Back))
		{
			base.SceneManager.AddScene(new Title(base.Game, Title.Phase.SelectMenu));
			FadeOut();
		}
	}

	private void UpdateMain(GameTime gameTime)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected I4, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		VirtualPadState virtualPadState = base.Input[Global.CurrentPlayer];
		_ = virtualPadState.Buttons;
		_ = virtualPadState.ThumbSticks.Left;
		_ = virtualPadState.DPad;
		_ = base.Input.GamePadStates[(int)Global.CurrentPlayer];
		if (gamePhase == GamePhase.Clear)
		{
			whiteOut += 0.01f;
			float num = -5f;
			if (player != null)
			{
				player.Thrust = ((num == 0f) ? 0f : (player.Thrust + num));
			}
			if (whiteOut >= 1f)
			{
				LoadNextStageBegin();
			}
		}
		else if (gamePhase == GamePhase.Play)
		{
			whiteOut = Math.Max(whiteOut - 0.01f, 0f);
			float num2 = 0f;
			if (player != null)
			{
				player.Thrust = ((num2 == 0f) ? 0f : (player.Thrust + num2));
			}
		}
		if (gamePhase == GamePhase.Play && player != null)
		{
			HitTest();
		}
		currentHp = ((player != null) ? player.Vitality : 0);
		if (viewHp > currentHp)
		{
			viewHp--;
		}
		else if (viewHp < currentHp)
		{
			viewHp++;
		}
		TimeSpan time = new TimeSpan(166666L * (long)(100 - viewHp));
		screenLifegauge.FixedUpdate(time);
	}

	private void UpdateSightFrustum(GameTime gameTime)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		if (player != null)
		{
			float num = 3f;
			Vector3 val = cameraPosition;
			Matrix sightWorld = player.GetSightWorld();
			Matrix val2 = Matrix.CreateLookAt(val, ((Matrix)(ref sightWorld)).Translation, Vector3.Up);
			float num2 = MathHelper.ToRadians(num);
			Viewport viewport = base.GraphicsDevice.Viewport;
			Matrix val3 = Matrix.CreatePerspectiveFieldOfView(num2, ((Viewport)(ref viewport)).AspectRatio, Global.SASData.Camera.NearFarClipping.X, Global.SASData.Camera.NearFarClipping.Y);
			Matrix val4 = val2 * val3;
			if (sightFrustum == (BoundingFrustum)null)
			{
				sightFrustum = new BoundingFrustum(val4);
			}
			else
			{
				sightFrustum.Matrix = val4;
			}
		}
	}

	private void UpdateModels(GameTime gameTime)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan elapsedGameTime = gameTime.ElapsedGameTime;
		TimeSpan elapsedGameTime2 = new TimeSpan((long)((float)gameTime.ElapsedGameTime.Ticks * Global.GameSpeed));
		stage.Update(elapsedGameTime2);
		if (player != null)
		{
			player.Update(elapsedGameTime);
			Vector3[] barnerPositions = player.BarnerPositions;
			foreach (Vector3 position in barnerPositions)
			{
				particleBurner.AddParticle(position, Vector3.Zero);
			}
		}
		UpdateModel(gameTime, sightLockOn);
		UpdateModel(gameTime, screenScore);
		if (gamePhase == GamePhase.GameOver)
		{
			UpdateModel(gameTime, gameOver);
		}
		if (gamePhase == GamePhase.Loading)
		{
			UpdateModel(gameTime, loading);
		}
		XSIModel[] array = scoreModels;
		foreach (XSIModel model in array)
		{
			UpdateModel(gameTime, model);
		}
		XSIModel[] array2 = enemyModels;
		foreach (XSIModel model2 in array2)
		{
			UpdateModel(gameTime, model2);
		}
	}

	private void UpdateModel(GameTime gameTime, XSIModel model)
	{
		model?.Update(gameTime);
	}

	private void UpdateParticles(GameTime gameTime)
	{
		UpdateVulcan(gameTime, vulcanParticleSystem.particles);
		UpdateEnemyShot(gameTime, enemyShotParticleSystem.particles);
		((GameComponent)particleExplosionSmoke).Update(gameTime);
		((GameComponent)particleExplosionFire).Update(gameTime);
		((GameComponent)particleExplosionFragment).Update(gameTime);
		((GameComponent)particleExplosionFragment2).Update(gameTime);
		((GameComponent)particleDestructionSmoke).Update(gameTime);
		((GameComponent)particleDestructionFire).Update(gameTime);
		((GameComponent)particleHormingMissileSmoke).Update(gameTime);
		((GameComponent)particleBurner).Update(gameTime);
		((GameComponent)particleBreakSmoke).Update(gameTime);
	}

	private void CreateVulcan(ParticleVertex[] particles)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < particles.Length; i++)
		{
			if (!(particles[i].Velocity != Vector3.Zero))
			{
				particles[i].Position = player.GetPosition();
				particles[i].Velocity = Vector3.Forward * 5f;
				particles[i].Time = 0f;
				particles[i].Random = Color.White;
				PlaySE("SE02");
				break;
			}
		}
	}

	private void UpdateVulcan(GameTime gameTime, ParticleVertex[] particles)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		vulcanCount = 0;
		for (int i = 0; i < particles.Length; i++)
		{
			if (!(particles[i].Velocity == Vector3.Zero))
			{
				UpdateVulcan(gameTime, ref particles[i]);
				vulcanCount++;
			}
		}
	}

	private void UpdateVulcan(GameTime gameTime, ref ParticleVertex particle)
	{
		if (particle.Position.Z > -200f)
		{
			particle.Time += (float)gameTime.ElapsedGameTime.TotalSeconds;
			ref Vector3 position = ref particle.Position;
			position.X += particle.Velocity.X;
			ref Vector3 position2 = ref particle.Position;
			position2.Y += particle.Velocity.Y;
			ref Vector3 position3 = ref particle.Position;
			position3.Z += particle.Velocity.Z;
		}
		else
		{
			DisposeVulcan(ref particle);
		}
	}

	private void DisposeVulcan(ref ParticleVertex particle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		particle.Position = Vector3.Zero;
		particle.Time = 0f;
		particle.Velocity = Vector3.Zero;
		particle.Position = new Vector3(0f, 0f, 10000f);
	}

	private bool EnableVulcan(ref ParticleVertex particle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return particle.Velocity != Vector3.Zero;
	}

	private void UpdateHormingMissile(GameTime gameTime)
	{
		((GameComponent)missileManager).Update(gameTime);
	}

	private void CreateHormingMissile()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (player == null)
		{
			return;
		}
		Vector3 position = player.GetPosition();
		Zako[] array = enemies;
		foreach (Zako enemy in array)
		{
			missileManager.CreateMissile(position, enemy);
		}
		if (boss != null && boss.IsBattle)
		{
			missileManager.CreateMissile(position, boss.core);
			BossShield[] shields = boss.shields;
			foreach (BossShield enemy2 in shields)
			{
				missileManager.CreateMissile(position, enemy2);
			}
			BossHand[] hands = boss.hands;
			foreach (BossHand enemy3 in hands)
			{
				missileManager.CreateMissile(position, enemy3);
			}
		}
	}

	private void CreateItem(string name, Vector3 position)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < items.Length; i++)
		{
			Item item = items[i];
			if (!item.Use)
			{
				item.Use = true;
				item.Enable = true;
				item.Visible = true;
				item.Velocity = gameSettings.ItemVelocity;
				item.Position = position;
				item.Name = name;
				item.Effect = itemEffects[name];
				item.Initialize();
				break;
			}
		}
	}

	private void UpdateImtes(TimeSpan elapsedGameTime)
	{
		for (int i = 0; i < items.Length; i++)
		{
			Item item = items[i];
			if (item.Use)
			{
				item.Update(elapsedGameTime);
				if (item.Position.Z > cameraPosition.Z)
				{
					item.Dispose();
				}
			}
		}
	}

	private void UpdateEnemies(TimeSpan elapsedGameTime)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan elapsedGameTime2 = new TimeSpan((long)((float)elapsedGameTime.Ticks * Global.GameSpeed));
		Zako[] array = enemies;
		foreach (Zako zako in array)
		{
			if (zako.Use)
			{
				zako.Update(elapsedGameTime2);
				_ = enemySettings[(int)zako.Type];
				EnemyMoveSettings enemyMoveSettings = this.enemyMoveSettings[zako.Move];
				double totalSeconds = zako.AnimationTime.TotalSeconds;
				Vector3 velocity = enemyMoveSettings.Velocity;
				velocity.X += (float)Math.Sin(totalSeconds * (double)enemyMoveSettings.Frequency.X) * enemyMoveSettings.WaveRange.X;
				velocity.Y += (float)Math.Cos(totalSeconds * (double)enemyMoveSettings.Frequency.Y) * enemyMoveSettings.WaveRange.Y;
				velocity.Z += (float)Math.Sin(totalSeconds * (double)enemyMoveSettings.Frequency.Z) * enemyMoveSettings.WaveRange.Z;
				zako.Position += velocity * Global.GameSpeed;
				if (zako.GetPosition().Z > gameSettings.LockOutDistance)
				{
					zako.Unlock(missileManager);
				}
				if (zako.GetPosition().Z > cameraPosition.Z)
				{
					zako.Dispose(missileManager);
				}
				zako.collision.UpdateBoundingSphere(zako.GetWorld());
			}
		}
		if (boss == null)
		{
			return;
		}
		BossHand[] hands = boss.hands;
		foreach (BossHand bossHand in hands)
		{
			if (bossHand.IsBreak && !bossHand.IsBreakMotion)
			{
				Vector3 position = bossHand.GetPosition();
				position.X += ((float)random.NextDouble() - 0.5f) * 2f * 3f;
				position.Y += ((float)random.NextDouble() - 0.5f) * 2f * 3f;
				particleBreakSmoke.AddParticle(position, new Vector3(0f, 0f, 200f));
			}
		}
		boss.Update(elapsedGameTime2, (player != null) ? player.GetPosition() : Vector3.Zero);
	}

	private void UpdateEnemyShot(GameTime gameTime, ParticleVertex[] particles)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < particles.Length; i++)
		{
			if (!(particles[i].Velocity == Vector3.Zero))
			{
				UpdateEnemyShot(gameTime, ref particles[i]);
			}
		}
	}

	private void UpdateEnemyShot(GameTime gameTime, ref ParticleVertex particle)
	{
		if (particle.Position.Z < cameraPosition.Z)
		{
			particle.Time += (float)gameTime.ElapsedGameTime.TotalSeconds * Global.GameSpeed;
			ref Vector3 position = ref particle.Position;
			position.X += particle.Velocity.X * Global.GameSpeed;
			ref Vector3 position2 = ref particle.Position;
			position2.Y += particle.Velocity.Y * Global.GameSpeed;
			ref Vector3 position3 = ref particle.Position;
			position3.Z += particle.Velocity.Z * Global.GameSpeed;
		}
		else
		{
			DisposeVulcan(ref particle);
		}
	}

	private void UpdateSound()
	{
		if (bgmMain == null && boss == null && bgmBossVolume == 0f && gamePhase == GamePhase.Play)
		{
			base.Sound.Stop(bgmBoss);
			bgmBoss = null;
			bgmMain = base.Sound.PlayBGM("StreamBGM", Sounds.BGM_Stages[stageIndex]);
			bgmMainVolume = 1f;
		}
		else if (bgmBoss == null && boss != null && bgmMainVolume == 0f && gamePhase == GamePhase.Play)
		{
			base.Sound.Stop(bgmMain);
			bgmMain = null;
			bgmBoss = base.Sound.PlayBGM("StreamBGM", "BGM_Boss1");
			bgmBossVolume = 1f;
		}
		float value = (Global.GameSpeed - 1f) * 0.1f;
		if (bgmMain != null)
		{
			base.Sound.SetVolume(bgmMain, bgmMainVolume);
			base.Sound.SetPitch(bgmMain, value);
		}
		if (bgmBoss != null)
		{
			base.Sound.SetVolume(bgmBoss, bgmBossVolume);
			base.Sound.SetPitch(bgmBoss, value);
		}
		if (boss == null)
		{
			bgmMainVolume = Math.Min(bgmMainVolume + 0.01f, 1f);
			bgmBossVolume = Math.Max(bgmBossVolume - 0.005f, 0f);
		}
		else if (boss != null)
		{
			bgmMainVolume = Math.Max(bgmMainVolume - 0.005f, 0f);
			bgmBossVolume = Math.Min(bgmBossVolume + 0.01f, 1f);
		}
		reverbValue = MathHelper.Clamp(reverbValue + (isReverb ? 0.01f : (-0.01f)), 0f, 1f);
		base.Sound.SetReverb(reverbValue);
		if ((soundFlag & SoundFlag.Vulcan) == SoundFlag.Vulcan)
		{
			PlaySE("SE02");
		}
		if ((soundFlag & SoundFlag.Damage) == SoundFlag.Damage)
		{
			PlaySE("SE11");
		}
		if ((soundFlag & SoundFlag.NoDamage) == SoundFlag.NoDamage)
		{
			PlaySE("SE12");
		}
		if ((soundFlag & SoundFlag.Break) == SoundFlag.Break)
		{
			PlaySE("SE04");
		}
		if ((soundFlag & SoundFlag.LockOn) == SoundFlag.LockOn)
		{
			PlaySE("SE03");
		}
		if ((soundFlag & SoundFlag.Missile) == SoundFlag.Missile)
		{
			PlaySE("SE01");
		}
		soundFlag = SoundFlag.Nothing;
	}

	private void CreateSmoke(Vector3 position, int count, Vector3 velocity)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < count; i++)
		{
			particleExplosionSmoke.AddParticle(position, velocity);
		}
	}

	private void CreateSmoke(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		CreateSmoke(position, 10, Vector3.Zero);
	}

	private void CreateExplosion(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		CreateSmoke(position);
		for (int i = 0; i < 5; i++)
		{
			particleExplosionFire.AddParticle(position, Vector3.Zero);
		}
		for (int j = 0; j < 10; j++)
		{
			particleExplosionFragment.AddParticle(position, Vector3.Zero);
			particleExplosionFragment2.AddParticle(position, Vector3.Zero);
		}
	}

	private void CreateDestructionParticle(Vector3 position)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 100; i++)
		{
			particleDestructionSmoke.AddParticle(position, Vector3.Zero);
		}
		for (int j = 0; j < 50; j++)
		{
			particleDestructionFire.AddParticle(position, Vector3.Zero);
		}
		for (int k = 0; k < 100; k++)
		{
			particleExplosionFragment.AddParticle(position, new Vector3(0f, 0f, 100f));
			particleExplosionFragment2.AddParticle(position, new Vector3(0f, 0f, 100f));
		}
	}

	private void SceneDraw(object sender, GameTime gameTime, SpriteBatch spriteBatch)
	{
		anaglyphRender.Draw(gameTime, base.SASData);
	}

	private void Draw2DTextures(GameTime gameTime, SpriteBatch spriteBatch)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		int backBufferWidth = base.Game.GraphicsDevice.PresentationParameters.BackBufferWidth;
		int backBufferHeight = base.Game.GraphicsDevice.PresentationParameters.BackBufferHeight;
		Rectangle rectangle = default(Rectangle);
		((Rectangle)(ref rectangle))._002Ector(0, 0, backBufferWidth, backBufferHeight);
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)backBufferWidth / 1280f, (float)backBufferHeight / 720f);
		if (gamePhase != GamePhase.Loading)
		{
			spriteBatch.Begin();
			spriteBatch.Draw(image_manual1, ImageManual1Position * val, (Rectangle?)null, Color.White, 0f, Vector2.Zero, val, (SpriteEffects)0, 0f);
			spriteBatch.Draw(image_manual2, ImageManual2Position * val, (Rectangle?)null, Color.White, 0f, Vector2.Zero, val, (SpriteEffects)0, 0f);
			spriteBatch.End();
			((Color)(ref WhiteOutColor)).A = (byte)Math.Min(255f * whiteOut, 255f);
			spriteBatch.Begin((SpriteBlendMode)2);
			base.DrawHelper.DrawFillRect(spriteBatch, rectangle, WhiteOutColor);
			spriteBatch.End();
		}
		if (IsPause)
		{
			spriteBatch.Begin();
			base.DrawHelper.DrawFillRect(spriteBatch, rectangle, PauseColor);
			spriteBatch.Draw(image_pause, ImagePausePosition * val, (Rectangle?)null, Color.White, 0f, Vector2.Zero, val, (SpriteEffects)0, 0f);
			spriteBatch.Draw(image_pause_quit, ImagePauseQuitPosition * val, (Rectangle?)null, Color.White, 0f, Vector2.Zero, val, (SpriteEffects)0, 0f);
			spriteBatch.End();
		}
	}

	private void DrawModel(XSIModel model)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Matrix world = Matrix.CreateTranslation(model.Position);
		DrawModel(model, world);
	}

	private void DrawModel(XSIModel model, Matrix world)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		model.Draw(base.SASData, world);
	}

	protected override void DrawInitializeLeft(GameTime gameTime)
	{
		base.DrawInitializeLeft(gameTime);
		SetParticleCamera();
	}

	protected override void DrawInitializeRight(GameTime gameTime)
	{
		base.DrawInitializeRight(gameTime);
		SetParticleCamera();
	}

	private void SetParticleCamera()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		particleExplosionSmoke.SetCamera(base.SASData.View, base.SASData.Projection);
		particleExplosionFire.SetCamera(base.SASData.View, base.SASData.Projection);
		particleExplosionFragment.SetCamera(base.SASData.View, base.SASData.Projection);
		particleExplosionFragment2.SetCamera(base.SASData.View, base.SASData.Projection);
		particleDestructionSmoke.SetCamera(base.SASData.View, base.SASData.Projection);
		particleDestructionFire.SetCamera(base.SASData.View, base.SASData.Projection);
		particleHormingMissileSmoke.SetCamera(base.SASData.View, base.SASData.Projection);
		particleBurner.SetCamera(base.SASData.View, base.SASData.Projection);
		particleBreakSmoke.SetCamera(base.SASData.View, base.SASData.Projection);
		vulcanParticleSystem.SetCamera(base.SASData.View, base.SASData.Projection);
		enemyShotParticleSystem.SetCamera(base.SASData.View, base.SASData.Projection);
	}

	private void DrawSight(EnemyData enemy, Vector3 position, float offset)
	{
		if (enemy.LockOnIndex >= 0)
		{
			sightLockOn.Position.X = position.X;
			sightLockOn.Position.Y = position.Y;
			sightLockOn.Position.Z = position.Z + offset;
			sightLockOn.FixedUpdate(enemy.SightLockTime);
			DrawModel(sightLockOn);
		}
	}

	private void DrawEnemyModels(GameTime gameTime)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		Zako[] array = enemies;
		foreach (Zako zako in array)
		{
			if (zako.Use)
			{
				XSIModel xSIModel = enemyModels[(int)zako.Type];
				xSIModel.FixedUpdate(zako.AnimationTime);
				xSIModel.Position.X = zako.Position.X;
				xSIModel.Position.Y = zako.Position.Y;
				xSIModel.Position.Z = zako.Position.Z;
				xSIModel.AmbientLightColor = Vector3.Lerp(Vector3.Zero, gameSettings.DamageColor, zako.FlashAmount);
				DrawModel(xSIModel);
				DrawSight(zako, zako.Position, 5f);
			}
		}
		if (boss != null)
		{
			boss.Draw(gameTime);
			DrawSight(boss.core, boss.GetPosition(), 10f);
			BossHand[] hands = boss.hands;
			foreach (BossHand bossHand in hands)
			{
				DrawSight(bossHand, bossHand.GetPosition(), 10f);
			}
		}
	}

	private void DrawScoreModels()
	{
		DrawScoreModels(score, gameSettings.ScorePosition);
		DrawScoreModels((score > Global.SaveData.HiScore) ? score : Global.SaveData.HiScore, gameSettings.HiScorePosition);
	}

	private void DrawScoreModels(int score, Vector3[] positions)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < positions.Length; i++)
		{
			XSIModel xSIModel = null;
			int num = 0;
			num = score % 10;
			score = (int)((float)score * 0.1f);
			xSIModel = scoreModels[num];
			if (xSIModel != null)
			{
				Vector3 position = positions[i];
				xSIModel.Position = position;
				DrawModel(xSIModel);
			}
		}
	}

	private void DrawHormingMissile(GameTime gameTime)
	{
		((DrawableGameComponent)missileManager).Draw(gameTime);
	}

	private void DrawItems(GameTime gameTime)
	{
		Item[] array = items;
		foreach (Item item in array)
		{
			if (item.Use)
			{
				item.Draw(gameTime);
			}
		}
	}

	private void DrawGameScreen(GameTime gameTime)
	{
		CustomParticleSystem.SetParticleRenderStates(base.GraphicsDevice.RenderState, (SpriteBlendMode)1);
		stage.Draw(gameTime);
		DrawModel(screenLifegauge);
		if (gamePhase == GamePhase.GameOver)
		{
			DrawModel(gameOver);
		}
		DrawModel(screenScore);
		DrawScoreModels();
		DrawEnemyModels(gameTime);
		if (gamePhase != GamePhase.GameOver)
		{
			player.Draw(gameTime);
		}
		DrawHormingMissile(gameTime);
		DrawItems(gameTime);
		((DrawableGameComponent)particleExplosionSmoke).Draw(gameTime);
		((DrawableGameComponent)particleExplosionFragment).Draw(gameTime);
		((DrawableGameComponent)particleExplosionFragment2).Draw(gameTime);
		((DrawableGameComponent)particleExplosionFire).Draw(gameTime);
		((DrawableGameComponent)particleDestructionSmoke).Draw(gameTime);
		((DrawableGameComponent)particleDestructionFire).Draw(gameTime);
		((DrawableGameComponent)particleHormingMissileSmoke).Draw(gameTime);
		((DrawableGameComponent)particleBurner).Draw(gameTime);
		((DrawableGameComponent)particleBreakSmoke).Draw(gameTime);
		enemyShotParticleSystem.Draw(gameTime, (SpriteBlendMode)2);
		vulcanParticleSystem.Draw(gameTime, (SpriteBlendMode)2);
		base.DrawHelper.SetRenderState((SpriteBlendMode)1);
		Draw2DTextures(gameTime, spriteBatch);
	}

	private void DrawLoadScreen(GameTime gameTime)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		loading.Draw(Global.SASData, Matrix.Identity);
	}

	protected override void DrawScene(GameTime gameTime)
	{
		if (gamePhase != GamePhase.Loading)
		{
			DrawGameScreen(gameTime);
		}
		else
		{
			DrawLoadScreen(gameTime);
		}
		base.DrawScene(gameTime);
	}

	private void SetSightRay()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		sightRay.Position = cameraPosition;
		float num = 0f;
		float num2 = 0f;
		Matrix sightWorld = player.GetSightWorld();
		Vector3 val = ((Matrix)(ref sightWorld)).Translation - cameraPosition;
		Vector3 val2 = Vector3.Cross(Vector3.Up, val);
		Vector3 val3 = Vector3.Cross(val2, Vector3.Up);
		Matrix val4 = Matrix.CreateFromAxisAngle(val2, num);
		Matrix val5 = Matrix.CreateFromAxisAngle(Vector3.Up, num2);
		Vector3 val6 = Vector3.TransformNormal(val, val4 * val5);
		if (Vector3.Dot(val6, val3) > 0.001f)
		{
			val = Vector3.Normalize(val6);
		}
		sightRay.Direction = val;
	}

	public void HitTest()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		base.TimeRuler.BeginMark("HitTest", Color.HotPink);
		base.TimeRuler.BeginMark("HitTestVulcan", Color.Purple);
		HitTestVulcan();
		base.TimeRuler.EndMark("HitTestVulcan");
		base.TimeRuler.BeginMark("HitTestMissile", Color.Purple);
		HitTestMissile();
		base.TimeRuler.EndMark("HitTestMissile");
		base.TimeRuler.BeginMark("HitTestItem", Color.Purple);
		HitTestItem();
		base.TimeRuler.EndMark("HitTestItem");
		base.TimeRuler.BeginMark("HitTestStage", Color.Purple);
		HitTestStage();
		base.TimeRuler.EndMark("HitTestStage");
		base.TimeRuler.BeginMark("HitTestEnemy", Color.Purple);
		HitTestEnemy();
		base.TimeRuler.EndMark("HitTestEnemy");
		base.TimeRuler.BeginMark("HitTestEnemyShot", Color.Purple);
		HitTestEnemyShot();
		base.TimeRuler.EndMark("HitTestEnemyShot");
		base.TimeRuler.BeginMark("HitTestRay", Color.Purple);
		HitTestSight();
		base.TimeRuler.EndMark("HitTestRay");
		base.TimeRuler.EndMark("HitTest");
	}

	public void HitTestVulcan()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		ParticleVertex[] particles = vulcanParticleSystem.particles;
		for (int i = 0; i < particles.Length; i++)
		{
			if (particles[i].Velocity == Vector3.Zero)
			{
				continue;
			}
			vulcanSphere.Center = particles[i].Position;
			for (int j = 0; j < enemies.Length; j++)
			{
				Zako zako = enemies[j];
				if (zako.Use)
				{
					int type = (int)zako.Type;
					enemySpheres[type].Center = zako.Position;
					if (HitTestVulcan(zako, enemySpheres[type], ref particles[i]))
					{
						return;
					}
				}
			}
			if (boss == null || !boss.IsBattle)
			{
				continue;
			}
			if (HitTestVulcan(boss.core, ref particles[i]))
			{
				break;
			}
			BossShield[] shields = boss.shields;
			foreach (BossShield parts in shields)
			{
				if (HitTestVulcan(parts, ref particles[i]))
				{
					return;
				}
			}
			BossHand[] hands = boss.hands;
			foreach (BossHand parts2 in hands)
			{
				if (HitTestVulcan(parts2, ref particles[i]))
				{
					return;
				}
			}
		}
	}

	private bool HitTestVulcan(EnemyData enemy, BoundingSphere enemySphere, ref ParticleVertex vulcan)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (EnableVulcan(ref vulcan) && ((BoundingSphere)(ref vulcanSphere)).Intersects(enemySphere))
		{
			DisposeVulcan(ref vulcan);
			if (DamageEnemy(enemy, gameSettings.ValcunAttackPower))
			{
				EntrySE(SoundFlag.Break);
			}
			else
			{
				EntrySE(SoundFlag.Damage);
			}
			return true;
		}
		return false;
	}

	private bool HitTestVulcan(BossParts parts, ref ParticleVertex vulcan)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (EnableVulcan(ref vulcan))
		{
			BoundingSphere[] boundingSpheres = parts.BoundingSpheres;
			foreach (BoundingSphere val in boundingSpheres)
			{
				if (((BoundingSphere)(ref vulcanSphere)).Intersects(val))
				{
					DisposeVulcan(ref vulcan);
					if (parts.Damage(gameSettings.ValcunAttackPower))
					{
						EntrySE(SoundFlag.Damage);
					}
					else
					{
						EntrySE(SoundFlag.NoDamage);
					}
					return true;
				}
			}
		}
		return false;
	}

	public bool HitTestMissile(HormingMissile missile, EnemyData enemy)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!missile.Use)
		{
			return false;
		}
		_ = enemy.LockOnIndex;
		enemy.Unlock(missileManager);
		CreateExplosion(missile.GetPosition());
		missile.Dispose();
		return true;
	}

	public void HitTestMissile(BossParts parts, HormingMissile missile)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		BoundingSphere missileSphere = GetMissileSphere(missile);
		BoundingSphere[] boundingSpheres = parts.BoundingSpheres;
		foreach (BoundingSphere val in boundingSpheres)
		{
			if ((int)((BoundingSphere)(ref missileSphere)).Contains(val) == 2)
			{
				if (HitTestMissile(missile, parts))
				{
					parts.Unlock(missileManager);
					EntrySE(parts.Damage(gameSettings.MissileAttackPower));
				}
				break;
			}
		}
	}

	private BoundingSphere GetMissileSphere(HormingMissile missile)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		BoundingSphere result = missile.BoundingSpheres[0];
		result.Radius *= 2f;
		return result;
	}

	public void HitTestMissile()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		HormingMissile[] missiles = missileManager.Missiles;
		foreach (HormingMissile hormingMissile in missiles)
		{
			if (!hormingMissile.Use || hormingMissile.Target == null || !hormingMissile.Target.Use)
			{
				continue;
			}
			EnemyData target = hormingMissile.Target;
			BoundingSphere val = hormingMissile.BoundingSpheres[0];
			val.Radius *= 2f;
			if (target is Zako)
			{
				Zako zako = target as Zako;
				BoundingSphere val2 = enemySpheres[(int)zako.Type];
				val2.Center = zako.GetPosition();
				if (((BoundingSphere)(ref val)).Intersects(val2))
				{
					HitTestMissile(hormingMissile, target);
					EntrySE(DamageEnemy(target, gameSettings.MissileAttackPower));
				}
			}
			else if (boss != null && boss.IsBattle)
			{
				HitTestMissile(boss.core, hormingMissile);
				BossShield[] shields = boss.shields;
				foreach (BossShield parts in shields)
				{
					HitTestMissile(parts, hormingMissile);
				}
				BossHand[] hands = boss.hands;
				foreach (BossHand parts2 in hands)
				{
					HitTestMissile(parts2, hormingMissile);
				}
			}
		}
	}

	public void HitTestItem()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < items.Length; i++)
		{
			Item item = items[i];
			if (!item.Use)
			{
				continue;
			}
			BoundingSphere[] boundingSpheres = item.BoundingSpheres;
			for (int j = 0; j < boundingSpheres.Length; j++)
			{
				BoundingSphere val = boundingSpheres[j];
				if (player == null || !item.Use)
				{
					continue;
				}
				BoundingSphere[] boundingSpheres2 = player.BoundingSpheres;
				foreach (BoundingSphere val2 in boundingSpheres2)
				{
					if (((BoundingSphere)(ref val)).Intersects(val2))
					{
						PlaySE("SE16");
						item.ActionEffect();
						item.Dispose();
						break;
					}
				}
			}
		}
	}

	public void HitTestStage()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (stage.BoundingSpheres == null)
		{
			return;
		}
		BoundingSphere[] boundingSpheres = stage.BoundingSpheres;
		for (int i = 0; i < boundingSpheres.Length; i++)
		{
			BoundingSphere val = boundingSpheres[i];
			if (val.Center.Z > 0f || val.Center.Z < -200f)
			{
				continue;
			}
			if (player != null && player.ShiledAlpha <= 0f)
			{
				BoundingSphere[] boundingSpheres2 = player.BoundingSpheres;
				foreach (BoundingSphere val2 in boundingSpheres2)
				{
					if (((BoundingSphere)(ref val)).Intersects(val2))
					{
						player.Damage(gameSettings.Damage.Wall, Difficulty.DamageRate);
						break;
					}
				}
			}
			for (int k = 0; k < vulcanParticleSystem.particles.Length; k++)
			{
				if (!(vulcanParticleSystem.particles[k].Velocity == Vector3.Zero))
				{
					vulcanSphere.Center = vulcanParticleSystem.particles[k].Position;
					if (((BoundingSphere)(ref val)).Intersects(vulcanSphere))
					{
						DisposeVulcan(ref vulcanParticleSystem.particles[k]);
						EntrySE(SoundFlag.NoDamage);
					}
				}
			}
		}
	}

	private void HitTestEnemy(BossParts parts)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		BoundingSphere[] boundingSpheres = parts.BoundingSpheres;
		foreach (BoundingSphere val in boundingSpheres)
		{
			if (player == null)
			{
				continue;
			}
			BoundingSphere[] boundingSpheres2 = player.BoundingSpheres;
			for (int j = 0; j < boundingSpheres2.Length; j++)
			{
				BoundingSphere val2 = boundingSpheres2[j];
				if (((BoundingSphere)(ref val2)).Intersects(val))
				{
					player.Damage(gameSettings.Damage.Boss, Difficulty.DamageRate);
					break;
				}
			}
		}
	}

	private void HitTestEnemy()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		foreach (Zako item in GetActiveEnemy())
		{
			int type = (int)item.Type;
			enemySpheres[type].Center = item.Position;
			for (int i = 0; i < vulcanParticleSystem.particles.Length; i++)
			{
				if (!(vulcanParticleSystem.particles[i].Velocity == Vector3.Zero))
				{
					vulcanSphere.Center = vulcanParticleSystem.particles[i].Position;
					if (((BoundingSphere)(ref vulcanSphere)).Intersects(enemySpheres[type]))
					{
						DisposeVulcan(ref vulcanParticleSystem.particles[i]);
						DamageEnemy(item, gameSettings.ValcunAttackPower);
					}
				}
			}
			if (player == null || !(player.ShiledAlpha <= 0f))
			{
				continue;
			}
			BoundingSphere[] boundingSpheres = player.BoundingSpheres;
			for (int j = 0; j < boundingSpheres.Length; j++)
			{
				BoundingSphere val = boundingSpheres[j];
				if (((BoundingSphere)(ref val)).Intersects(enemySpheres[type]))
				{
					player.Damage(gameSettings.Damage.Enemy, Difficulty.DamageRate);
					break;
				}
			}
		}
		if (boss != null && boss.IsBattle)
		{
			HitTestEnemy(boss.core);
			BossShield[] shields = boss.shields;
			foreach (BossShield parts in shields)
			{
				HitTestEnemy(parts);
			}
			BossHand[] hands = boss.hands;
			foreach (BossHand parts2 in hands)
			{
				HitTestEnemy(parts2);
			}
		}
	}

	private bool HitTestSight(EnemyData enemy, BoundingSphere sphere)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!enemy.IsLockOnEnabled || (int)sightFrustum.Contains(sphere) != 2 || enemy.GetPosition().Z > gameSettings.LockOutDistance || enemy.LockOnIndex >= 0)
		{
			return false;
		}
		bool flag = missileManager.LockCheck(enemy);
		if (flag)
		{
			EntrySE(SoundFlag.LockOn);
		}
		return flag;
	}

	private bool HitTestSight(BossParts parts)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!parts.IsBreak)
		{
			BoundingSphere[] boundingSpheres = parts.BoundingSpheres;
			foreach (BoundingSphere sphere in boundingSpheres)
			{
				if (HitTestSight(parts, sphere))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void HitTestSight()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		foreach (Zako item in GetActiveEnemy())
		{
			BoundingSphere enemySphere = GetEnemySphere(item.Type);
			enemySphere.Center = item.GetPosition();
			if (HitTestSight(item, enemySphere))
			{
				break;
			}
		}
		if (boss == null || !boss.IsBattle || HitTestSight(boss.core))
		{
			return;
		}
		BossShield[] shields = boss.shields;
		foreach (BossShield parts in shields)
		{
			if (HitTestSight(parts))
			{
				break;
			}
		}
		BossHand[] hands = boss.hands;
		foreach (BossHand parts2 in hands)
		{
			if (HitTestSight(parts2))
			{
				break;
			}
		}
	}

	private void HitTestEnemyShot()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		BoundingSphere val = default(BoundingSphere);
		((BoundingSphere)(ref val))._002Ector(Vector3.Zero, 1f);
		ParticleVertex[] particles = enemyShotParticleSystem.particles;
		for (int i = 0; i < particles.Length; i++)
		{
			if (particles[i].Velocity == Vector3.Zero || player == null || particles[i].Position.Z < -10f)
			{
				continue;
			}
			val.Center = particles[i].Position;
			BoundingSphere[] boundingSpheres = player.BoundingSpheres;
			for (int j = 0; j < boundingSpheres.Length; j++)
			{
				BoundingSphere val2 = boundingSpheres[j];
				if (((BoundingSphere)(ref val2)).Intersects(val))
				{
					DisposeVulcan(ref particles[i]);
					player.Damage(gameSettings.Damage.Shot, Difficulty.DamageRate);
					break;
				}
			}
		}
	}

	private void AddScore(int value)
	{
		int num = (int)((float)value * Difficulty.ScoreRate);
		score = Math.Min(score + num, 9999999);
	}

	private void StageClear()
	{
		if (player != null)
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].Dispose();
			}
			DisposeParticles(enemyShotParticleSystem.particles);
			DisposeParticles(vulcanParticleSystem.particles);
			missileManager.Clear();
			if (boss != null)
			{
				boss.Dispose();
				boss = null;
			}
			gamePhase = GamePhase.Clear;
			player.IsHandling = false;
			PlaySE("SE10");
		}
	}

	private void NextStage()
	{
		int length = stageSettings.GetLength(0);
		stageIndex = (stageIndex + 1) % length;
		if (stageIndex == 0)
		{
			Global.GameSpeed += gameSettings.SpeedUp;
			lap++;
		}
		chapterIndex = 0;
		InitializeStageModel(CurrentChapter, loop: false);
		missileManager.DisableReserb();
		gamePhase = GamePhase.Play;
		GC.Collect();
	}

	private void CreateEnemy(EnemyType type, int move, int? shot, Vector3 position)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (type >= EnemyType.MaxCount)
		{
			return;
		}
		if (type == EnemyType.Boss)
		{
			CreateBoss(position);
			return;
		}
		for (int i = 0; i < enemies.Length; i++)
		{
			Zako zako = enemies[i];
			if (!zako.Use)
			{
				zako.model = enemyModels[(int)type];
				zako.collision = enemyColModels[(int)type];
				zako.Use = true;
				zako.Type = type;
				zako.Position = position;
				zako.Enable = true;
				zako.Move = move;
				zako.IsLockOnEnabled = true;
				zako.Visible = true;
				zako.Vitality = enemySettings[(int)type].Vitality;
				zako.Score = enemySettings[(int)type].Score;
				zako.AnimationTime = TimeSpan.Zero;
				if (shot.HasValue)
				{
					zako.ShotSettings = enemyShotSettings[shot.Value];
					zako.Shot += CreateEnemyShot;
				}
				break;
			}
		}
	}

	private void CreateBoss(Vector3 position)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		boss = new Boss(base.Game, CurrentStage.Boss, Global.AsyncLoader.Content);
		boss.Position = position;
		boss.Shot += boss_Shot;
		boss.Explosion += boss_Explosion;
		boss.Destruction += boss_Destruction;
		boss.BreakMotionFinished += boss_BreakMotionFinished;
		boss.BattleFinished += boss_BattleFinished;
		BossCore core = boss.core;
		core.Destruction = (Action<int>)Delegate.Combine(core.Destruction, new Action<int>(AddScore));
		boss.core.SoundPlay += SoundPlay;
		BossHand[] hands = boss.hands;
		foreach (BossHand bossHand in hands)
		{
			bossHand.SoundPlay += SoundPlay;
			bossHand.Explosion += BossPartsExplosion;
			bossHand.Destruction = (Action<int>)Delegate.Combine(bossHand.Destruction, new Action<int>(AddScore));
		}
		BossShield[] shields = boss.shields;
		foreach (BossShield bossShield in shields)
		{
			bossShield.SoundPlay += SoundPlay;
			bossShield.Explosion += BossPartsExplosion;
			bossShield.Destruction = (Action<int>)Delegate.Combine(bossShield.Destruction, new Action<int>(AddScore));
		}
	}

	private void BossPartsExplosion(ExplosionType type, Vector3 position)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		PlaySE("SE04");
		CreateExplosion(position);
	}

	private void boss_BattleFinished()
	{
		if (boss != null)
		{
			boss.Unlock(missileManager);
		}
		gamePhase = GamePhase.BossDestruct;
	}

	private void boss_BreakMotionFinished()
	{
		StageClear();
		if (player != null)
		{
			player.Restore(gameSettings.ClearRestore);
		}
	}

	private void boss_Destruction(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		CreateDestructionParticle(position);
		BossHand[] hands = boss.hands;
		foreach (BossHand bossHand in hands)
		{
			CreateDestructionParticle(bossHand.GetDockPosition());
		}
	}

	private void boss_Explosion(Vector3 position)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		PlaySE("SE04");
		Vector3 zero = Vector3.Zero;
		zero.X = ((float)random.NextDouble() - 0.5f) * 30f;
		zero.Y = ((float)random.NextDouble() - 0.5f) * 30f;
		zero.Z = ((float)random.NextDouble() - 0.5f) * 30f;
		CreateExplosion(position + zero);
	}

	private void boss_Shot(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		CreateEnemyShot(position, null, 1f);
	}

	private void SoundPlay(string name)
	{
		PlaySE(name);
	}

	private void CreateEnemyShot(Vector3 position, Vector3? normal, float speed)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		if (player == null || Vector3.Distance(position, player.GetPosition()) < 5f || position.Z > gameSettings.EnemyShotNear || position.Z < gameSettings.EnemyShotFar)
		{
			return;
		}
		ParticleVertex[] particles = enemyShotParticleSystem.particles;
		for (int i = 0; i < particles.Length; i++)
		{
			if (particles[i].Velocity != Vector3.Zero)
			{
				continue;
			}
			particles[i].Position = position;
			if (normal.HasValue)
			{
				particles[i].Velocity = normal.Value * speed;
				break;
			}
			float num = 0f;
			float num2 = 0f;
			Vector3 val = player.GetPosition() - position;
			Vector3 val2 = Vector3.Cross(Vector3.Up, val);
			Vector3 val3 = Vector3.Cross(val2, Vector3.Up);
			Matrix val4 = Matrix.CreateFromAxisAngle(val2, num);
			Matrix val5 = Matrix.CreateFromAxisAngle(Vector3.Up, num2);
			Vector3 val6 = Vector3.TransformNormal(val, val4 * val5);
			if (Vector3.Dot(val6, val3) > 0.001f)
			{
				val = Vector3.Normalize(val6);
			}
			val *= speed;
			if (val.Z > 0f)
			{
				particles[i].Velocity = val;
			}
			else
			{
				DisposeVulcan(ref particles[i]);
			}
			break;
		}
	}

	private IEnumerable<Zako> GetActiveEnemy()
	{
		for (int enemyIndex = 0; enemyIndex < enemies.Length; enemyIndex++)
		{
			Zako enemy = enemies[enemyIndex];
			if (enemy.Use)
			{
				yield return enemy;
			}
		}
	}

	private IEnumerable<BoundingSphere> GetActiveVulcan()
	{
		for (int vulcanIndex = 0; vulcanIndex < vulcanParticleSystem.particles.Length; vulcanIndex++)
		{
			if (!(vulcanParticleSystem.particles[vulcanIndex].Velocity == Vector3.Zero))
			{
				vulcanSphere.Center = vulcanParticleSystem.particles[vulcanIndex].Position;
				yield return vulcanSphere;
			}
		}
	}

	private bool DamageEnemy(EnemyData enemy, int damage)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		enemy.Vitality = Math.Max(enemy.Vitality - damage, 0);
		if (enemy.Vitality > 0)
		{
			enemy.FlashAmount = 1f;
			return false;
		}
		AddScore(enemy.Score);
		CreateExplosion(enemy.Position);
		missileManager.DisableReserb(enemy.LockOnIndex);
		enemy.Dispose();
		return true;
	}

	private void SetupGameOver()
	{
		PlaySE("SE04");
		PlaySE("SE14");
		gameOver.Play();
		gamePhase = GamePhase.GameOver;
		player = null;
		for (int i = 0; i < vulcanParticleSystem.particles.Length; i++)
		{
			DisposeVulcan(ref vulcanParticleSystem.particles[i]);
		}
		missileManager.Clear();
	}

	private BoundingSphere GetEnemySphere(EnemyType type)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return enemySpheres[(int)type];
	}

	private IEnumerable<string> GetNextStageAssets()
	{
		StageSettings stage = ((stageIndex + 1 < stageSettings.Length) ? stageSettings[stageIndex + 1] : stageSettings[0]);
		try
		{
			ChapterSettings[] chapters = stage.Chapters;
			foreach (ChapterSettings chapter in chapters)
			{
				yield return chapter.StageModelAsset;
				yield return chapter.CollisionModelAsset;
				yield return chapter.BgModelAsset;
			}
		}
		finally
		{
		}
		yield return "Models/Models/boss/boss_bg";
		yield return stage.Boss.MotionAppearAsset;
		yield return stage.Boss.MotionBattleAsset;
	}

	private void LoadNextStageBegin()
	{
		if (Guide.IsTrialMode)
		{
			base.SceneManager.AddScene(new TrialDemo(base.Game));
			FadeOut();
			return;
		}
		gamePhase = GamePhase.Loading;
		loading.Play(isLoop: true);
		Global.AsyncLoader.Content.Unload();
		Global.AsyncLoader.AsyncLoad(GetNextStageAssets(), delegate
		{
			NextStage();
		});
	}

	private void EntrySE(SoundFlag flag)
	{
		soundFlag |= flag;
	}

	private void EntrySE(bool destoroy)
	{
		EntrySE(destoroy ? SoundFlag.Break : SoundFlag.Damage);
	}

	private void PlaySE(string name)
	{
		base.Sound.PlaySE(name);
	}
}
