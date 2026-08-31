using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using JetStarUniverse.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace JetStarUniverse;

public class Game1 : Game
{
	public enum Difficulty
	{
		Easy = 1,
		Normal,
		Hard
	}

	public struct SaveGameData
	{
		public string PlayerName;

		public int Level;

		public Difficulty Difficulty;

		public int Score;

		public DateTime SaveTime;
	}

	private const double LEVEL_TIME = 120000.0;

	private const double STAGE_CLEAR_TIME = 10000.0;

	private const double SPAWN_TIME = 5000.0;

	private GraphicsDeviceManager _graphics;

	private SpriteBatch _spriteBatch;

	private SpriteFont _spriteFont;

	private Texture2D _backgroundSplit;

	private Texture2D _layer1Split;

	private Texture2D _lifeBlock;

	private Texture2D _bulletBlock;

	private Texture2D _level1;

	private Texture2D _level2;

	private Texture2D _level3;

	private Texture2D _level4;

	private Texture2D _level5;

	private Texture2D _level6;

	private Texture2D _level7;

	private Texture2D _level8;

	private Texture2D _noLevel;

	private Texture2D _stageClear;

	private Texture2D _startTitle;

	private Texture2D _unlockTitle;

	private Texture2D _optionsTitle;

	private Texture2D _helpTitle;

	private Texture2D _exitTitle;

	private Texture2D _continueTitle;

	private Texture2D _jetStarUniverse;

	private Texture2D _jetStarUniverseTrial;

	private Texture2D _xboxControllerConfig;

	private Texture2D _pressStartButton;

	private Texture2D _easy;

	private Texture2D _normal;

	private Texture2D _hard;

	private Texture2D _congratulations;

	private Texture2D _pressStartToBegin;

	private SoundEffect _shoot;

	private SoundEffect _pickup;

	private SoundEffect _hit;

	private SoundEffect _playerHit;

	private SoundEffect _playerExplosion;

	private SoundEffect _select;

	private SoundEffect _select2;

	private SoundEffect _select3;

	private SoundEffect _select4;

	private SoundEffect _select5;

	private SoundEffect _powerup;

	private SoundEffect _bossHit;

	private PlayerIndex _playerIndex = PlayerIndex.One;

	private Song _crystalis;

	private Song _ballad;

	private Song _levelFinish;

	private Song _angryRobot;

	private Song _currentSong;

	private Song _azimuth;

	private Song _blueChill;

	private Song _crisson;

	private Song _warningBlitz;

	private Song _clickThenAction;

	private Song _collidescope;

	private Song _confuze;

	private Song _xMorph;

	private Player _player;

	private PowerupItem _powerupItem;

	private Rectangle _screenSize;

	private int _spaceLeftToDrawOnScreen;

	private int _layer1LeftToDrawOnScreen;

	private int _xScroll;

	private int _layer1Scroll = 800;

	private int _powerupItems = 3;

	private int _powerupItemPrice = 1;

	private int _score = 0;

	private int _currentLevel = 1;

	private int _titleSelect = 0;

	private int _miniBossDeadCount = 0;

	private int _difficulty = 1;

	private List<Bird> _birds = new List<Bird>();

	private List<Enemy> _enemies = new List<Enemy>();

	private List<Enemy2> _enemies2 = new List<Enemy2>();

	private List<Enemy3> _enemies3 = new List<Enemy3>();

	private List<Enemy4> _enemies4 = new List<Enemy4>();

	private List<Enemy5> _enemies5 = new List<Enemy5>();

	private List<Enemy6> _enemies6 = new List<Enemy6>();

	private List<Enemy7> _enemies7 = new List<Enemy7>();

	private List<Miniboss> _miniBossList = new List<Miniboss>();

	private List<Finalboss> _finalBossList = new List<Finalboss>();

	private List<Rectangle> _sourceBordersForInventory = new List<Rectangle>();

	private List<double> _levelTime = new List<double>();

	private List<Rectangle> _layer1BoundingBoxes = new List<Rectangle>();

	private double _selectionBorderFlashTime = 0.0;

	private double _selectionInventoryTime = 0.0;

	private double _stageClearTime = 0.0;

	private double _startUpTime = 0.0;

	private double _layer1StartFadeTime;

	private double _spawnTime;

	private double _pressStartButtonShowTime = 0.0;

	private double _assistProjectileTime = 0.0;

	private double _backButtonPressTime = 0.0;

	private double _gameCompleteFadeTime = 0.0;

	private double _pressStartToBeginShowTime = 0.0;

	private double _highScoreTimeFade = 0.0;

	private double _pressStartButtonTimeFade = 0.0;

	private double _titleTimeFade = 0.0;

	private double _helpTimeFade = 0.0;

	private double _difficultyTimeFade = 0.0;

	private double _invisibilityTime = 20000.0;

	private float _margins = 25f;

	private float _fadeIncrement = 0f;

	private double? _fadeSelectTime = null;

	private double? _titleSelectPress = null;

	private double? _countDownTime = null;

	private double? _splashSelectPress = null;

	private double? _xScrollTime = null;

	private bool _selectionBorderShow = true;

	private bool _itemQualify = true;

	private bool _startUpShow = true;

	private bool _fadeOut = false;

	private bool _stageCleared = false;

	private bool _splashScreen = true;

	private bool _title = false;

	private bool _paused = false;

	private bool _help = false;

	private bool _pressStartButtonShow = true;

	private bool _miniBossStart = false;

	private bool _finalBossStart = false;

	private bool _difficultySelect = false;

	private bool _highScoreScreen = false;

	private bool _gameStarted = false;

	private bool _gameComplete = false;

	private bool _showPressStartToBegin = false;

	private string _customMessage = "";

	private double _customMessageTime = 0.0;

	private bool _enableCustomMessage = false;

	private bool _showCustomMessage = false;

	private bool _loadingData = false;

	private bool _savingData = false;

	private bool _options = false;

	private int _optionSelect = 0;

	private float _musicVolume = 1f;

	private float _soundEffectVolume = 1f;

	private IAsyncResult _result;

	private List<SaveGameData> _data;

	private int _damage = 0;

	private float _bullets = 500f;

	private double _rechargeBulletsTime = 0.0;

	private StorageDevice _device;

	public static GamerServicesComponent GamerServices { get; private set; }

	public double GetTotalGameTime(GameTime gameTime)
	{
		return gameTime.TotalGameTime.TotalMilliseconds;
	}

	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		base.Initialize();
		GamerServices = new GamerServicesComponent(this);
		base.Components.Add(GamerServices);
		GamerServices.Initialize();
	}

	protected override void LoadContent()
	{
		_screenSize = base.GraphicsDevice.Viewport.TitleSafeArea;
		_powerupItemPrice *= _difficulty;
		LoadContentForPlayer();
		LoadContentForPowerupItem();
		LoadContentForBirds();
		LoadContentOnSourceBordersForInventory();
		_layer1BoundingBoxes.Add(new Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, 150));
		_spriteBatch = new SpriteBatch(base.GraphicsDevice);
		_spriteFont = base.Content.Load<SpriteFont>("MyFont");
		_backgroundSplit = base.Content.Load<Texture2D>("space_stars");
		_layer1Split = base.Content.Load<Texture2D>("layer1_background");
		_shoot = base.Content.Load<SoundEffect>("shoot2");
		_crystalis = base.Content.Load<Song>("DST-Crysalis");
		_crisson = base.Content.Load<Song>("DST-Crisson");
		_warningBlitz = base.Content.Load<Song>("DST-WarningBlitz");
		_pickup = base.Content.Load<SoundEffect>("pickup1");
		_hit = base.Content.Load<SoundEffect>("hit2");
		_playerHit = base.Content.Load<SoundEffect>("hit1");
		_lifeBlock = base.Content.Load<Texture2D>("block");
		_bulletBlock = base.Content.Load<Texture2D>("block2");
		_playerExplosion = base.Content.Load<SoundEffect>("explosion5");
		_select = base.Content.Load<SoundEffect>("select");
		_select2 = base.Content.Load<SoundEffect>("select2");
		_select3 = base.Content.Load<SoundEffect>("select3");
		_select4 = base.Content.Load<SoundEffect>("select4");
		_select5 = base.Content.Load<SoundEffect>("select5");
		_powerup = base.Content.Load<SoundEffect>("powerup");
		_ballad = base.Content.Load<Song>("DST-2ndBallad");
		_level1 = base.Content.Load<Texture2D>("level1");
		_levelFinish = base.Content.Load<Song>("DST-AmbientKingdom");
		_level2 = base.Content.Load<Texture2D>("level2");
		_level3 = base.Content.Load<Texture2D>("level3");
		_level4 = base.Content.Load<Texture2D>("level4");
		_level5 = base.Content.Load<Texture2D>("level5");
		_level6 = base.Content.Load<Texture2D>("level6");
		_level7 = base.Content.Load<Texture2D>("level7");
		_level8 = base.Content.Load<Texture2D>("level8");
		_noLevel = base.Content.Load<Texture2D>("nolevel");
		_stageClear = base.Content.Load<Texture2D>("stageclear");
		_angryRobot = base.Content.Load<Song>("DST-AngryRobotIII");
		_azimuth = base.Content.Load<Song>("DST-Azimuth");
		_currentSong = _crystalis;
		_blueChill = base.Content.Load<Song>("DST-BlueChill");
		_startTitle = base.Content.Load<Texture2D>("start_title");
		_unlockTitle = base.Content.Load<Texture2D>("unlock_title");
		_optionsTitle = base.Content.Load<Texture2D>("options_title");
		_helpTitle = base.Content.Load<Texture2D>("help_title");
		_exitTitle = base.Content.Load<Texture2D>("exit_title");
		_continueTitle = base.Content.Load<Texture2D>("continue_title");
		_jetStarUniverse = base.Content.Load<Texture2D>("jet_star_universe");
		_jetStarUniverseTrial = base.Content.Load<Texture2D>("jet_star_universe_trial");
		_xboxControllerConfig = base.Content.Load<Texture2D>("xbox_controller_config");
		_pressStartButton = base.Content.Load<Texture2D>("press_start_button");
		_clickThenAction = base.Content.Load<Song>("DST-ClickThenAction");
		_collidescope = base.Content.Load<Song>("DST-Collidescope");
		_confuze = base.Content.Load<Song>("DST-ConFuze");
		_xMorph = base.Content.Load<Song>("DST-Xmorph");
		_easy = base.Content.Load<Texture2D>("easy");
		_normal = base.Content.Load<Texture2D>("normal");
		_hard = base.Content.Load<Texture2D>("hard");
		_bossHit = base.Content.Load<SoundEffect>("hit3");
		_congratulations = base.Content.Load<Texture2D>("congratulations");
		_pressStartToBegin = base.Content.Load<Texture2D>("press_start_to_begin");
		MediaPlayer.Play(_crisson);
		MediaPlayer.Volume = 1f;
		MediaPlayer.IsRepeating = true;
	}

	private void LoadContentForEnemies()
	{
		foreach (Enemy enemy in _enemies)
		{
			SetTextureExplosionForEnemy(enemy);
		}
		foreach (Enemy6 item in _enemies6)
		{
			SetTextureExplosionForEnemy(item);
		}
		foreach (Enemy2 item2 in _enemies2)
		{
			SetTextureExplosionForEnemy(item2);
		}
		foreach (Enemy3 item3 in _enemies3)
		{
			SetTextureExplosionForEnemy(item3);
		}
		foreach (Enemy4 item4 in _enemies4)
		{
			SetTextureExplosionForEnemy(item4);
		}
		foreach (Enemy7 item5 in _enemies7)
		{
			SetTextureExplosionForEnemy(item5);
		}
		foreach (Enemy5 item6 in _enemies5)
		{
			SetTextureExplosionForEnemy(item6);
		}
		foreach (Miniboss miniBoss in _miniBossList)
		{
			SetTextureExplosionForEnemy(miniBoss);
		}
		foreach (Finalboss finalBoss in _finalBossList)
		{
			SetTextureExplosionForEnemy(finalBoss);
		}
	}

	private void LoadContentOnSourceBordersForInventory()
	{
		_sourceBordersForInventory.Add(new Rectangle(95, _screenSize.Bottom - 28, 65, 25));
		_sourceBordersForInventory.Add(new Rectangle(170, _screenSize.Bottom - 28, 65, 25));
		_sourceBordersForInventory.Add(new Rectangle(247, _screenSize.Bottom - 28, 70, 25));
		_sourceBordersForInventory.Add(new Rectangle(328, _screenSize.Bottom - 28, 150, 25));
		_sourceBordersForInventory.Add(new Rectangle(490, _screenSize.Bottom - 28, 75, 25));
		_sourceBordersForInventory.Add(new Rectangle(575, _screenSize.Bottom - 28, 65, 25));
		_sourceBordersForInventory.Add(new Rectangle(645, _screenSize.Bottom - 28, 70, 25));
	}

	private void LoadContentForBirds()
	{
		Bird bird = new Bird(25, 21);
		bird.Texture2D = base.Content.Load<Texture2D>("bird");
		bird.Position = new Vector2(_screenSize.Right + 30, _screenSize.Bottom / 2);
		bird.SourceRectangles.Add(new Rectangle(5, 2, bird.Width, bird.Height));
		bird.SourceRectangles.Add(new Rectangle(44, 2, 25, 33));
		bird.SourceRectangles.Add(new Rectangle(82, 5, 25, 19));
		bird.SourceRectangles.Add(new Rectangle(4, 45, 26, 20));
		bird.SourceRectangles.Add(new Rectangle(44, 45, 25, 21));
		bird.SourceRectangles.Add(new Rectangle(82, 45, 30, 18));
		_birds.Add(bird);
		Bird bird2 = new Bird(bird.Width, bird.Height);
		bird2.Texture2D = bird.Texture2D;
		bird2.SourceRectangles = bird.SourceRectangles;
		_birds.Add(bird2);
		Bird bird3 = new Bird(bird.Width, bird.Height);
		bird3.Texture2D = bird.Texture2D;
		bird3.SourceRectangles = bird.SourceRectangles;
		_birds.Add(bird3);
		Bird bird4 = new Bird(bird.Width, bird.Height);
		bird4.Texture2D = bird.Texture2D;
		bird4.SourceRectangles = bird.SourceRectangles;
		_birds.Add(bird4);
		Bird bird5 = new Bird(bird.Width, bird.Height);
		bird5.Texture2D = bird.Texture2D;
		bird5.SourceRectangles = bird.SourceRectangles;
		_birds.Add(bird5);
		Bird.ResetAllBirdPositions(_birds, base.GraphicsDevice, _screenSize);
		foreach (Bird bird6 in _birds)
		{
			bird6.TextureOfExplosion = base.Content.Load<Texture2D>("explosion");
			bird6.SourceOfExplosion.Add(new Rectangle(0, 0, 64, 64));
			bird6.SourceOfExplosion.Add(new Rectangle(64, 0, 64, 64));
			bird6.SourceOfExplosion.Add(new Rectangle(128, 0, 64, 64));
			bird6.SourceOfExplosion.Add(new Rectangle(196, 0, 64, 64));
			bird6.SourceOfExplosion.Add(new Rectangle(196, 64, 64, 64));
			bird6.SourceOfExplosion.Add(new Rectangle(196, 64, 64, 64));
			bird6.SourceOfExplosion.Add(new Rectangle(196, 64, 64, 64));
			bird6.SourceOfExplosion.Add(new Rectangle(196, 64, 64, 64));
			bird6.SourceOfExplosion.Add(new Rectangle(196, 128, 64, 64));
			bird6.SourceOfExplosion.Add(new Rectangle(196, 128, 64, 64));
			bird6.SourceOfExplosion.Add(new Rectangle(196, 128, 64, 64));
			bird6.SourceOfExplosion.Add(new Rectangle(196, 128, 64, 64));
			bird6.SourceOfExplosion.Add(new Rectangle(0, 196, 64, 64));
			bird6.NextFrameIndexOfExplosion = 0;
		}
	}

	private void LoadContentForPowerupItem()
	{
		_powerupItem = new PowerupItem(30, 24);
		_powerupItem.Texture2D = base.Content.Load<Texture2D>("powerup_item");
		_powerupItem.Position = _powerupItem.RandomLocation(_screenSize.Right + 30, _screenSize.Bottom);
		_powerupItem.SourceRectangles.Add(new Rectangle(0, 0, _powerupItem.Width, _powerupItem.Height));
		_powerupItem.SourceRectangles.Add(new Rectangle(30, 0, _powerupItem.Width, _powerupItem.Height));
		_powerupItem.SourceRectangles.Add(new Rectangle(60, 0, _powerupItem.Width, _powerupItem.Height));
		_powerupItem.Hidden = true;
	}

	private void LoadContentForPlayer()
	{
		_player = new Player(56, 28);
		_player.Texture2D = base.Content.Load<Texture2D>("player_ship");
		_player.Position = new Vector2(_screenSize.Left + 30, _screenSize.Top + 30);
		_player.SourceRectangles.Add(new Rectangle(0, 0, _player.Width, _player.Height));
		_player.SourceRectangles.Add(new Rectangle(66, 0, _player.Width, _player.Height));
		_player.SourceRectangles.Add(new Rectangle(130, 0, _player.Width, _player.Height));
		_player.SourceProjectile = new Rectangle(215, 10, 10, 6);
		_player.TextureOfExplosion = base.Content.Load<Texture2D>("explosion");
		_player.SourceOfExplosion = new List<Rectangle>();
		_player.SourceOfExplosion.Add(new Rectangle(0, 0, 64, 64));
		_player.SourceOfExplosion.Add(new Rectangle(64, 0, 64, 64));
		_player.SourceOfExplosion.Add(new Rectangle(128, 0, 64, 64));
		_player.SourceOfExplosion.Add(new Rectangle(196, 0, 64, 64));
		_player.SourceOfExplosion.Add(new Rectangle(196, 64, 64, 64));
		_player.SourceOfExplosion.Add(new Rectangle(196, 64, 64, 64));
		_player.SourceOfExplosion.Add(new Rectangle(196, 64, 64, 64));
		_player.SourceOfExplosion.Add(new Rectangle(196, 64, 64, 64));
		_player.SourceOfExplosion.Add(new Rectangle(196, 128, 64, 64));
		_player.SourceOfExplosion.Add(new Rectangle(196, 128, 64, 64));
		_player.SourceOfExplosion.Add(new Rectangle(196, 128, 64, 64));
		_player.SourceOfExplosion.Add(new Rectangle(196, 128, 64, 64));
		_player.SourceOfExplosion.Add(new Rectangle(0, 196, 64, 64));
		_player.NextFrameIndexOfExplosion = 0;
	}

	private void AddNewEnemy()
	{
		Enemy enemy = new Enemy(32, 32);
		enemy.Texture2D = base.Content.Load<Texture2D>("space_enemy1");
		Random random = new Random((int)DateTime.Now.Ticks);
		enemy.Position = new Vector2(_screenSize.Right + 100, random.Next(_screenSize.Top + 100, _screenSize.Bottom - 100));
		enemy.SourceRectangles.Add(new Rectangle(0, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(38, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(76, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(114, 0, enemy.Width, enemy.Height));
		enemy.SourceProjectile = new Rectangle(167, 12, 10, 6);
		enemy.Score = 38;
		_enemies.Add(enemy);
	}

	private void AddNewEnemy6()
	{
		Enemy6 enemy = new Enemy6(32, 32);
		enemy.Texture2D = base.Content.Load<Texture2D>("space_enemy6");
		enemy.Position = new Vector2(_screenSize.Right + 30, 140f);
		enemy.SourceRectangles.Add(new Rectangle(0, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(38, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(76, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(114, 0, enemy.Width, enemy.Height));
		enemy.SourceProjectile = new Rectangle(167, 12, 10, 6);
		enemy.Score = 175;
		_enemies6.Add(enemy);
	}

	private void AddNewEnemy2()
	{
		Enemy2 enemy = new Enemy2(28, 29);
		enemy.Texture2D = base.Content.Load<Texture2D>("space_enemy2");
		Random random = new Random((int)DateTime.Now.Ticks);
		enemy.Position = new Vector2(_screenSize.Right + 100, random.Next(_screenSize.Top + 100, _screenSize.Bottom - 100));
		enemy.SourceRectangles.Add(new Rectangle(3, 2, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(45, 4, 24, 24));
		enemy.SourceRectangles.Add(new Rectangle(81, 8, 16, 16));
		enemy.SourceRectangles.Add(new Rectangle(106, 10, 13, 12));
		enemy.SourceRectangles.Add(new Rectangle(138, 12, 7, 7));
		enemy.Score = 24;
		_enemies2.Add(enemy);
	}

	private void AddNewEnemy3()
	{
		Enemy3 enemy = new Enemy3(30, 30);
		enemy.Texture2D = base.Content.Load<Texture2D>("space_enemy3");
		enemy.Position = new Vector2(_screenSize.Right + 30, 100f);
		enemy.SourceRectangles.Add(new Rectangle(0, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(32, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(64, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(96, 0, enemy.Width, enemy.Height));
		enemy.SourceProjectile = new Rectangle(136, 11, 10, 6);
		enemy.Score = 56;
		_enemies3.Add(enemy);
	}

	private void AddNewEnemy4()
	{
		Enemy4 enemy = new Enemy4(32, 32);
		enemy.Texture2D = base.Content.Load<Texture2D>("space_enemy4");
		enemy.Position = new Vector2(_screenSize.Right + 30, _screenSize.Top + 10);
		enemy.SourceRectangles.Add(new Rectangle(0, 0, enemy.Width, enemy.Height));
		enemy.SourceProjectile = new Rectangle(38, 6, 20, 18);
		Random random = new Random((int)DateTime.Now.Ticks);
		enemy.Speed = random.Next(5, 15);
		enemy.Score = 125;
		_enemies4.Add(enemy);
	}

	private void AddNewEnemy7()
	{
		Enemy7 enemy = new Enemy7(32, 32);
		enemy.Texture2D = base.Content.Load<Texture2D>("space_enemy7");
		enemy.Position = new Vector2(_screenSize.Right + 30, 20f);
		enemy.SourceRectangles.Add(new Rectangle(0, 0, enemy.Width, enemy.Height));
		enemy.SourceProjectile = new Rectangle(38, 6, 20, 18);
		Random random = new Random((int)DateTime.Now.Ticks);
		enemy.Speed = random.Next(5, 15);
		enemy.Score = 250;
		_enemies7.Add(enemy);
	}

	private void AddNewEnemy5()
	{
		Enemy5 enemy = new Enemy5(56, 28);
		enemy.Texture2D = base.Content.Load<Texture2D>("space_enemy5");
		enemy.Position = new Vector2(_screenSize.Right - enemy.Width - 10, (float)_screenSize.Top + 30f);
		enemy.SourceRectangles.Add(new Rectangle(194, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(128, 6, 56, 22));
		enemy.SourceRectangles.Add(new Rectangle(64, 0, enemy.Width, enemy.Height));
		enemy.SourceProjectile = new Rectangle(25, 10, 10, 6);
		Random random = new Random((int)DateTime.Now.Ticks);
		enemy.Score = 250;
		_enemies5.Add(enemy);
	}

	private void AddNewMiniboss()
	{
		Miniboss miniboss = new Miniboss(48, 48);
		miniboss.Texture2D = base.Content.Load<Texture2D>("miniboss");
		miniboss.Position = new Vector2(_screenSize.Right - miniboss.Width - 10, (float)_screenSize.Top + 30f);
		miniboss.SourceRectangles.Add(new Rectangle(0, 0, miniboss.Width, miniboss.Height));
		miniboss.SourceProjectile = new Rectangle(66, 19, 10, 6);
		Random random = new Random((int)DateTime.Now.Ticks);
		miniboss.Score = 2000;
		_miniBossList.Add(miniboss);
	}

	private void AddNewFinalboss()
	{
		Finalboss finalboss = new Finalboss(168, 124);
		finalboss.Texture2D = base.Content.Load<Texture2D>("finalboss");
		finalboss.Position = new Vector2(_screenSize.Right / 2, 200f);
		finalboss.SourceRectangles.Add(new Rectangle(2, 8, finalboss.Width, finalboss.Height));
		finalboss.SourceProjectile = new Rectangle(196, 54, 18, 18);
		Random random = new Random((int)DateTime.Now.Ticks);
		finalboss.Score = 10000;
		_finalBossList.Add(finalboss);
	}

	private void SetTextureExplosionForEnemy(Enemy enemy)
	{
		enemy.TextureOfExplosion = _birds.First().TextureOfExplosion;
		enemy.SourceOfExplosion = _birds.First().SourceOfExplosion;
		enemy.NextFrameIndexOfExplosion = 0;
	}

	private void SetTextureExplosionForEnemy(Enemy6 enemy)
	{
		enemy.TextureOfExplosion = _birds.First().TextureOfExplosion;
		enemy.SourceOfExplosion = _birds.First().SourceOfExplosion;
		enemy.NextFrameIndexOfExplosion = 0;
	}

	private void SetTextureExplosionForEnemy(Enemy2 enemy)
	{
		enemy.TextureOfExplosion = _birds.First().TextureOfExplosion;
		enemy.SourceOfExplosion = _birds.First().SourceOfExplosion;
		enemy.NextFrameIndexOfExplosion = 0;
	}

	private void SetTextureExplosionForEnemy(Enemy3 enemy)
	{
		enemy.TextureOfExplosion = _birds.First().TextureOfExplosion;
		enemy.SourceOfExplosion = _birds.First().SourceOfExplosion;
		enemy.NextFrameIndexOfExplosion = 0;
	}

	private void SetTextureExplosionForEnemy(Enemy4 enemy)
	{
		enemy.TextureOfExplosion = _birds.First().TextureOfExplosion;
		enemy.SourceOfExplosion = _birds.First().SourceOfExplosion;
		enemy.NextFrameIndexOfExplosion = 0;
	}

	private void SetTextureExplosionForEnemy(Enemy7 enemy)
	{
		enemy.TextureOfExplosion = _birds.First().TextureOfExplosion;
		enemy.SourceOfExplosion = _birds.First().SourceOfExplosion;
		enemy.NextFrameIndexOfExplosion = 0;
	}

	private void SetTextureExplosionForEnemy(Enemy5 enemy)
	{
		enemy.TextureOfExplosion = _birds.First().TextureOfExplosion;
		enemy.SourceOfExplosion = _birds.First().SourceOfExplosion;
		enemy.NextFrameIndexOfExplosion = 0;
	}

	private void SetTextureExplosionForEnemy(Miniboss enemy)
	{
		enemy.TextureOfExplosion = _birds.First().TextureOfExplosion;
		enemy.SourceOfExplosion = _birds.First().SourceOfExplosion;
		enemy.NextFrameIndexOfExplosion = 0;
	}

	private void SetTextureExplosionForEnemy(Finalboss enemy)
	{
		enemy.TextureOfExplosion = _birds.First().TextureOfExplosion;
		enemy.SourceOfExplosion = _birds.First().SourceOfExplosion;
		enemy.NextFrameIndexOfExplosion = 0;
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		if (_splashScreen && !_title)
		{
			if (!_splashSelectPress.HasValue)
			{
				_splashSelectPress = GetTotalGameTime(gameTime);
				_fadeSelectTime = _splashSelectPress;
				_titleSelectPress = _splashSelectPress;
			}
			if (GetTotalGameTime(gameTime) - _splashSelectPress >= 250.0)
			{
				for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
				{
					if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed && !Guide.IsVisible)
					{
						if (!_loadingData)
						{
							_select3.Play(0.25f, 0f, 0f);
							_playerIndex = playerIndex;
							_splashSelectPress = GetTotalGameTime(gameTime);
							_enableCustomMessage = true;
							_customMessage = "Loading data, please wait....";
							_xScrollTime = _splashSelectPress;
							_loadingData = true;
							_customMessageTime = 1500.0;
						}
						break;
					}
				}
			}
			if (_loadingData && _customMessageTime == 0.0)
			{
				_splashScreen = false;
				_title = true;
				_levelTime.Add(0.0);
				_currentLevel = 1;
				_difficulty = 1;
				_score = 0;
				CreateDummyData();
				if (!Guide.IsVisible)
				{
					_result = StorageDevice.BeginShowSelector(GetDeviceForLoad, null);
				}
				_titleTimeFade = 0.0;
				_enableCustomMessage = false;
				_loadingData = false;
			}
			_pressStartButtonShowTime += gameTime.ElapsedGameTime.Milliseconds;
			if (_pressStartButtonTimeFade >= 1000.0 && _pressStartButtonShowTime >= 250.0)
			{
				_pressStartButtonShowTime = 0.0;
				_pressStartButtonShow = !_pressStartButtonShow;
			}
		}
		else
		{
			if (_backButtonPressTime >= 250.0 && _title)
			{
				if (GamePad.GetState(_playerIndex).Buttons.Back == ButtonState.Pressed)
				{
					_backButtonPressTime = 0.0;
					_highScoreScreen = !_highScoreScreen;
					_paused = !_paused;
					_highScoreTimeFade = 0.0;
				}
			}
			else
			{
				_backButtonPressTime += gameTime.ElapsedGameTime.TotalMilliseconds;
			}
			if (!_highScoreScreen)
			{
				_spaceLeftToDrawOnScreen = base.GraphicsDevice.Viewport.Width - _backgroundSplit.Width;
				_xScroll = (int)(0.0 - _xScrollTime / 8.0 % (double)_backgroundSplit.Width).Value;
				_xScrollTime = GetTotalGameTime(gameTime);
				if (_gameComplete)
				{
					if (GamePad.GetState(_playerIndex).Buttons.Start == ButtonState.Pressed && _showPressStartToBegin)
					{
						_title = true;
						_savingData = true;
						_enableCustomMessage = true;
						_customMessage = "Saving data, please wait....";
						_customMessageTime = 1500.0;
						_gameComplete = false;
						_titleSelectPress = GetTotalGameTime(gameTime);
						_levelTime.Clear();
						_levelTime.Add(0.0);
						_gameStarted = true;
						MediaPlayer.Play(_crystalis);
						_finalBossStart = false;
						RemoveAllEnemies();
					}
				}
				else
				{
					if (_help)
					{
						double? num = GetTotalGameTime(gameTime) - _titleSelectPress;
						if (num.GetValueOrDefault() >= 250.0 && num.HasValue && GamePad.GetState(_playerIndex).Buttons.A == ButtonState.Pressed)
						{
							_help = false;
							_titleSelectPress = GetTotalGameTime(gameTime);
							_titleTimeFade = 0.0;
						}
					}
					if (_options && GetTotalGameTime(gameTime) - _titleSelectPress >= 250.0)
					{
						if (GamePad.GetState(_playerIndex).DPad.Down == ButtonState.Pressed || GamePad.GetState(_playerIndex).ThumbSticks.Left.Y <= -0.25f)
						{
							_titleSelectPress = GetTotalGameTime(gameTime);
							if (++_optionSelect > 2)
							{
								_optionSelect = 0;
							}
							_select4.Play();
						}
						else if (GamePad.GetState(_playerIndex).DPad.Up == ButtonState.Pressed || GamePad.GetState(_playerIndex).ThumbSticks.Left.Y >= 0.25f)
						{
							_titleSelectPress = GetTotalGameTime(gameTime);
							if (--_optionSelect < 0)
							{
								_optionSelect = 2;
							}
							_select4.Play();
						}
						else if (GamePad.GetState(_playerIndex).DPad.Left == ButtonState.Pressed || GamePad.GetState(_playerIndex).ThumbSticks.Left.X <= -0.25f)
						{
							switch (_optionSelect)
							{
							case 0:
								_musicVolume -= 0.01f;
								if (_musicVolume <= 0f)
								{
									_musicVolume = 0f;
								}
								MediaPlayer.Volume = _musicVolume;
								break;
							case 1:
								_soundEffectVolume -= 0.01f;
								if (_soundEffectVolume <= 0f)
								{
									_soundEffectVolume = 0f;
								}
								SoundEffect.MasterVolume = _soundEffectVolume;
								break;
							}
						}
						else if (GamePad.GetState(_playerIndex).DPad.Right == ButtonState.Pressed || GamePad.GetState(_playerIndex).ThumbSticks.Left.X >= 0.25f)
						{
							switch (_optionSelect)
							{
							case 0:
								_musicVolume += 0.01f;
								if (_musicVolume >= 1f)
								{
									_musicVolume = 1f;
								}
								MediaPlayer.Volume = _musicVolume;
								break;
							case 1:
								_soundEffectVolume += 0.01f;
								if (_soundEffectVolume >= 1f)
								{
									_soundEffectVolume = 1f;
								}
								SoundEffect.MasterVolume = _soundEffectVolume;
								break;
							}
						}
						else if (GamePad.GetState(_playerIndex).Buttons.A == ButtonState.Pressed && _optionSelect == 2)
						{
							_title = true;
							_options = false;
							_titleSelectPress = GetTotalGameTime(gameTime);
						}
					}
					if (_title && !_help && !_options)
					{
						if (_savingData && _customMessageTime == 0.0)
						{
							GetDeviceForSave(_result);
							_player.Life = 10;
							_player.Projectiles = new List<Projectile>();
							_player.Projectiles.Add(new Projectile());
							_player.Projectiles.Add(new Projectile());
							_player.Projectiles.Add(new Projectile());
							_player.TintColor = Color.White;
							_player.Power = 3;
							_player.Inventory = 0;
							_player.Speed = 1;
							_player.NextPowerupBonus = 2500;
							_player.AssistProjectiles.Clear();
							_player.Assist = 0;
							_currentLevel = 1;
							_powerupItems = 3 * _difficulty;
							_score = 0;
							_savingData = false;
						}
						if (GetTotalGameTime(gameTime) - _titleSelectPress >= 250.0)
						{
							if (GamePad.GetState(_playerIndex).DPad.Down == ButtonState.Pressed || GamePad.GetState(_playerIndex).ThumbSticks.Left.Y <= -0.25f)
							{
								_titleSelectPress = GetTotalGameTime(gameTime);
								if (++_titleSelect > 4)
								{
									_titleSelect = 0;
								}
								_select4.Play();
							}
							else if (GamePad.GetState(_playerIndex).DPad.Up == ButtonState.Pressed || GamePad.GetState(_playerIndex).ThumbSticks.Left.Y >= 0.25f)
							{
								_titleSelectPress = GetTotalGameTime(gameTime);
								if (--_titleSelect < 0)
								{
									_titleSelect = 4;
								}
								_select4.Play();
							}
							else if (GamePad.GetState(_playerIndex).Buttons.A == ButtonState.Pressed)
							{
								_titleSelectPress = GetTotalGameTime(gameTime);
								switch (_titleSelect)
								{
								case 0:
									if (_currentLevel > 1)
									{
										if (Guide.IsTrialMode)
										{
											if (!Guide.IsVisible && _title && _playerIndex.CanBuyGame())
											{
												Guide.ShowMarketplace(_playerIndex);
											}
										}
										else
										{
											_title = false;
											_paused = false;
										}
										break;
									}
									_title = false;
									if (!_gameStarted)
									{
										_difficultySelect = true;
										_difficultyTimeFade = 0.0;
										_player.Dead = false;
										_select2.Play();
									}
									else
									{
										_paused = false;
									}
									break;
								case 1:
									if (Guide.IsTrialMode && !Guide.IsVisible && _title && _playerIndex.CanBuyGame())
									{
										Guide.ShowMarketplace(_playerIndex);
									}
									break;
								case 2:
									_options = true;
									break;
								case 3:
									_help = true;
									_helpTimeFade = 0.0;
									break;
								case 4:
									Exit();
									break;
								}
							}
						}
					}
					if (_difficultySelect && !_gameStarted && GetTotalGameTime(gameTime) - _titleSelectPress >= 250.0)
					{
						if (GamePad.GetState(_playerIndex).DPad.Down == ButtonState.Pressed || GamePad.GetState(_playerIndex).ThumbSticks.Left.Y <= -0.25f)
						{
							_titleSelectPress = GetTotalGameTime(gameTime);
							if (++_difficulty > 3)
							{
								_difficulty = 1;
							}
							_select4.Play();
						}
						else if (GamePad.GetState(_playerIndex).DPad.Up == ButtonState.Pressed || GamePad.GetState(_playerIndex).ThumbSticks.Left.Y >= 0.25f)
						{
							_titleSelectPress = GetTotalGameTime(gameTime);
							if (--_difficulty < 1)
							{
								_difficulty = 3;
							}
							_select4.Play();
						}
						else if (GamePad.GetState(_playerIndex).Buttons.A == ButtonState.Pressed)
						{
							_powerupItems = 3 * _difficulty;
							_difficultySelect = false;
							_gameStarted = true;
							_titleSelectPress = GetTotalGameTime(gameTime);
							_title = false;
							_paused = false;
							_select2.Play();
							MediaPlayer.Play(_crystalis);
						}
					}
					if (_gameStarted && !_title)
					{
						double? num = GetTotalGameTime(gameTime) - _titleSelectPress;
						if (num.GetValueOrDefault() >= 250.0 && num.HasValue && GamePad.GetState(_playerIndex).Buttons.Start == ButtonState.Pressed)
						{
							_titleTimeFade = 0.0;
							_titleSelectPress = GetTotalGameTime(gameTime);
							_title = true;
							_paused = true;
							_select2.Play();
						}
					}
					if (_gameStarted && !_title && !_paused)
					{
						if (!GamePad.GetState(_playerIndex).IsConnected || Guide.IsVisible)
						{
							_paused = true;
							_title = true;
						}
						float num2 = 5f;
						if ((GamePad.GetState(_playerIndex).DPad.Left == ButtonState.Pressed || GamePad.GetState(_playerIndex).ThumbSticks.Left.X <= -0.25f) && _player.Position.X > (float)_screenSize.Left)
						{
							_player.Position = new Vector2(_player.Position.X - num2 - (float)_player.Speed, _player.Position.Y);
						}
						if ((GamePad.GetState(_playerIndex).DPad.Right == ButtonState.Pressed || GamePad.GetState(_playerIndex).ThumbSticks.Left.X >= 0.25f) && _player.Position.X < (float)(_screenSize.Right - _player.Width))
						{
							_player.Position = new Vector2(_player.Position.X + num2 + (float)_player.Speed, _player.Position.Y);
						}
						if (GamePad.GetState(_playerIndex).DPad.Down == ButtonState.Pressed || GamePad.GetState(_playerIndex).ThumbSticks.Left.Y <= -0.25f)
						{
							_player.NextFrameIndex = 2;
							if (_player.Position.Y < (float)(_screenSize.Bottom - _player.Height - 30))
							{
								_player.Position = new Vector2(_player.Position.X, _player.Position.Y + num2 + (float)_player.Speed);
							}
						}
						if (GamePad.GetState(_playerIndex).DPad.Up == ButtonState.Pressed || GamePad.GetState(_playerIndex).ThumbSticks.Left.Y >= 0.25f)
						{
							_player.NextFrameIndex = 1;
							if (_player.Position.Y > (float)_screenSize.Top + _margins)
							{
								_player.Position = new Vector2(_player.Position.X, _player.Position.Y - num2 - (float)_player.Speed);
							}
						}
						if (GamePad.GetState(_playerIndex).DPad.Up == ButtonState.Released && GamePad.GetState(_playerIndex).DPad.Down == ButtonState.Released && GamePad.GetState(_playerIndex).ThumbSticks.Left.Y == 0f)
						{
							_player.NextFrameIndex = 0;
						}
						_player.KeyPressTime += gameTime.ElapsedGameTime.TotalMilliseconds;
						double? num = GetTotalGameTime(gameTime) - _titleSelectPress;
						if (num.GetValueOrDefault() >= 250.0 && num.HasValue && GamePad.GetState(_playerIndex).Buttons.A == ButtonState.Pressed && _player.KeyPressTime >= 100.0)
						{
							foreach (Projectile projectile in _player.Projectiles)
							{
								if (!projectile.Show && !_player.Dead)
								{
									if (!(--_bullets < 0f))
									{
										projectile.Show = true;
										projectile.Position = _player.CenterRight;
										_shoot.Play(0.25f, 0f, 0f);
										_player.KeyPressTime = 0.0;
										break;
									}
									_bullets = 0f;
								}
							}
						}
						_assistProjectileTime += gameTime.ElapsedGameTime.TotalMilliseconds;
						if (_assistProjectileTime >= 250.0)
						{
							float num3 = 1f;
							foreach (Projectile assistProjectile in _player.AssistProjectiles)
							{
								if (!assistProjectile.Show && !_player.Dead)
								{
									assistProjectile.Show = true;
									assistProjectile.Position = _player.CenterRight;
									assistProjectile.Position = new Vector2(assistProjectile.Position.X - num3 * 3f, assistProjectile.Position.Y - 30f);
									break;
								}
								num3++;
							}
							_assistProjectileTime = 0.0;
						}
						_selectionInventoryTime += gameTime.ElapsedGameTime.TotalMilliseconds;
						if (GamePad.GetState(_playerIndex).Buttons.LeftShoulder == ButtonState.Pressed && _powerupItems > 0 && _selectionInventoryTime >= 200.0)
						{
							if (--_player.Inventory < 0)
							{
								_player.Inventory = _sourceBordersForInventory.Count - 1;
							}
							CheckForItemQualify();
							_select5.Play();
							_selectionBorderShow = true;
							_selectionInventoryTime = 0.0;
						}
						if (GamePad.GetState(_playerIndex).Buttons.RightShoulder == ButtonState.Pressed && _powerupItems > 0 && _selectionInventoryTime >= 200.0)
						{
							if (++_player.Inventory >= _sourceBordersForInventory.Count)
							{
								_player.Inventory = 0;
							}
							CheckForItemQualify();
							_select5.Play();
							_selectionBorderShow = true;
							_selectionInventoryTime = 0.0;
						}
						if (GamePad.GetState(_playerIndex).Buttons.B == ButtonState.Pressed)
						{
							CheckForItemQualify();
							if (_itemQualify && _selectionInventoryTime >= 200.0)
							{
								bool flag = false;
								switch (_player.Inventory)
								{
								case 0:
									_player.Speed++;
									break;
								case 1:
									_player.Projectiles.Add(new Projectile());
									_player.Power++;
									break;
								case 2:
									foreach (Enemy enemy in _enemies)
									{
										if (!enemy.Dead)
										{
											flag = true;
											enemy.Dead = true;
											enemy.NextFrameIndexOfExplosion = 0;
											enemy.ShowExplosion = true;
											enemy.PositionOfExplosion = enemy.Position;
											_score += enemy.Score * _difficulty;
										}
									}
									foreach (Enemy6 item in _enemies6)
									{
										if (!item.Dead)
										{
											flag = true;
											item.Dead = true;
											item.NextFrameIndexOfExplosion = 0;
											item.ShowExplosion = true;
											item.PositionOfExplosion = item.Position;
											_score += item.Score * _difficulty;
										}
									}
									foreach (Enemy2 item2 in _enemies2)
									{
										if (!item2.Dead)
										{
											flag = true;
											item2.Dead = true;
											item2.NextFrameIndexOfExplosion = 0;
											item2.ShowExplosion = true;
											item2.PositionOfExplosion = item2.Position;
											_score += item2.Score * _difficulty;
										}
									}
									foreach (Enemy3 item3 in _enemies3)
									{
										if (!item3.Dead)
										{
											flag = true;
											item3.Dead = true;
											item3.NextFrameIndexOfExplosion = 0;
											item3.ShowExplosion = true;
											item3.PositionOfExplosion = item3.Position;
											_score += item3.Score * _difficulty;
										}
									}
									foreach (Enemy4 item4 in _enemies4)
									{
										if (!item4.Dead)
										{
											flag = true;
											item4.Dead = true;
											item4.NextFrameIndexOfExplosion = 0;
											item4.ShowExplosion = true;
											item4.PositionOfExplosion = item4.Position;
											_score += item4.Score * _difficulty;
										}
									}
									foreach (Enemy7 item5 in _enemies7)
									{
										if (!item5.Dead)
										{
											flag = true;
											item5.Dead = true;
											item5.NextFrameIndexOfExplosion = 0;
											item5.ShowExplosion = true;
											item5.PositionOfExplosion = item5.Position;
											_score += item5.Score * _difficulty;
										}
									}
									foreach (Bird bird in _birds)
									{
										if (!bird.Dead)
										{
											flag = true;
											bird.Dead = true;
											bird.NextFrameIndexOfExplosion = 0;
											bird.ShowExplosion = true;
											bird.PositionOfExplosion = bird.Position;
										}
									}
									if (flag)
									{
										_hit.Play();
										CheckForItemQualify();
									}
									break;
								case 3:
									MediaPlayer.Play(_ballad);
									_player.Invincibility = true;
									_player.InvincibilityTimeFlash = DateTime.Now;
									_player.InvincibilityTime = DateTime.Now;
									break;
								case 4:
									_player.Dead = false;
									_player.Life = 10;
									_player.Speed = 1;
									_player.Power = 3;
									_player.Projectiles.Clear();
									_player.Projectiles.Add(new Projectile());
									_player.Projectiles.Add(new Projectile());
									_player.Projectiles.Add(new Projectile());
									_player.Assist = 0;
									_player.AssistProjectiles.Clear();
									break;
								case 5:
									_player.Life = 10;
									break;
								case 6:
								{
									_player.Assist = 1;
									for (int i = 0; i < _difficulty; i++)
									{
										_player.AssistProjectiles.Add(new Projectile());
									}
									break;
								}
								}
								if (!_player.Dead && ((_player.Inventory == 2 && flag) || _player.Inventory != 2))
								{
									_powerupItems -= _powerupItemPrice;
									_powerup.Play();
									_selectionInventoryTime = 0.0;
								}
							}
						}
						foreach (Projectile projectile2 in _player.Projectiles)
						{
							if (!projectile2.Show)
							{
								continue;
							}
							if (projectile2.Position.X > (float)base.GraphicsDevice.Viewport.Width)
							{
								projectile2.Show = false;
								continue;
							}
							projectile2.Position = new Vector2(projectile2.Position.X + 20f, projectile2.Position.Y);
							Rectangle rectangle = new Rectangle((int)projectile2.Position.X, (int)projectile2.Position.Y, _player.SourceProjectile.Width, _player.SourceProjectile.Height);
							foreach (Bird bird2 in _birds)
							{
								if (!bird2.Dead && rectangle.Intersects(bird2.BoxRectangle))
								{
									bird2.Dead = true;
									projectile2.Show = false;
									_hit.Play();
									bird2.NextFrameIndexOfExplosion = 0;
									bird2.ShowExplosion = true;
									bird2.PositionOfExplosion = bird2.Position;
								}
							}
							foreach (Enemy enemy2 in _enemies)
							{
								if (!enemy2.Dead && rectangle.Intersects(enemy2.BoxRectangle))
								{
									projectile2.ShowTime = 1000.0;
									projectile2.Show = false;
									if (--enemy2.Life <= 0)
									{
										_score += enemy2.Score * _difficulty;
										enemy2.Dead = true;
										_hit.Play();
										enemy2.NextFrameIndexOfExplosion = 0;
										enemy2.ShowExplosion = true;
										enemy2.PositionOfExplosion = enemy2.Position;
									}
								}
							}
							foreach (Enemy6 item6 in _enemies6)
							{
								if (!item6.Dead && rectangle.Intersects(item6.BoxRectangle))
								{
									projectile2.ShowTime = 1000.0;
									projectile2.Show = false;
									if (--item6.Life <= 0)
									{
										_score += item6.Score * _difficulty;
										item6.Dead = true;
										_hit.Play();
										item6.NextFrameIndexOfExplosion = 0;
										item6.ShowExplosion = true;
										item6.PositionOfExplosion = item6.Position;
									}
								}
							}
							foreach (Enemy2 item7 in _enemies2)
							{
								if (!item7.Dead && rectangle.Intersects(item7.BoxRectangle))
								{
									projectile2.ShowTime = 1000.0;
									projectile2.Show = false;
									if (--item7.Life <= 0)
									{
										_score += item7.Score * _difficulty;
										item7.Dead = true;
										_hit.Play();
										item7.NextFrameIndexOfExplosion = 0;
										item7.ShowExplosion = true;
										item7.PositionOfExplosion = item7.Position;
									}
								}
							}
							foreach (Enemy3 item8 in _enemies3)
							{
								if (!item8.Dead && rectangle.Intersects(item8.BoxRectangle))
								{
									projectile2.ShowTime = 1000.0;
									projectile2.Show = false;
									if (--item8.Life <= 0)
									{
										_score += item8.Score * _difficulty;
										item8.Dead = true;
										_hit.Play();
										item8.NextFrameIndexOfExplosion = 0;
										item8.ShowExplosion = true;
										item8.PositionOfExplosion = item8.Position;
									}
								}
							}
							foreach (Enemy4 item9 in _enemies4)
							{
								if (!item9.Dead && rectangle.Intersects(item9.BoxRectangle))
								{
									projectile2.ShowTime = 1000.0;
									projectile2.Show = false;
									if (--item9.Life <= 0)
									{
										_score += item9.Score * _difficulty;
										item9.Dead = true;
										_hit.Play();
										item9.NextFrameIndexOfExplosion = 0;
										item9.ShowExplosion = true;
										item9.PositionOfExplosion = item9.Position;
									}
								}
							}
							foreach (Enemy7 item10 in _enemies7)
							{
								if (!item10.Dead && rectangle.Intersects(item10.BoxRectangle))
								{
									projectile2.ShowTime = 1000.0;
									projectile2.Show = false;
									if (--item10.Life <= 0)
									{
										_score += item10.Score * _difficulty;
										item10.Dead = true;
										_hit.Play();
										item10.NextFrameIndexOfExplosion = 0;
										item10.ShowExplosion = true;
										item10.PositionOfExplosion = item10.Position;
									}
								}
							}
							foreach (Enemy5 item11 in _enemies5)
							{
								if (!item11.Dead && rectangle.Intersects(item11.BoxRectangle))
								{
									projectile2.ShowTime = 1000.0;
									projectile2.Show = false;
									if (--item11.Life <= 0)
									{
										_score += item11.Score * _difficulty;
										item11.Dead = true;
										_hit.Play();
										item11.NextFrameIndexOfExplosion = 0;
										item11.ShowExplosion = true;
										item11.PositionOfExplosion = item11.Position;
									}
								}
							}
							foreach (Miniboss miniBoss in _miniBossList)
							{
								if (!miniBoss.Dead && rectangle.Intersects(miniBoss.BoxRectangle))
								{
									projectile2.ShowTime = 1000.0;
									projectile2.Show = false;
									Random random = new Random((int)DateTime.Now.Ticks);
									miniBoss.TintColor = new Color(random.Next(1, 255), random.Next(1, 255), random.Next(1, 255));
									miniBoss.Hit = true;
									_bossHit.Play();
									if (--miniBoss.Life <= 0)
									{
										_score += miniBoss.Score * _difficulty;
										miniBoss.Dead = true;
										_hit.Play();
										miniBoss.NextFrameIndexOfExplosion = 0;
										miniBoss.ShowExplosion = true;
										miniBoss.PositionOfExplosion = miniBoss.Position;
										_miniBossDeadCount++;
									}
								}
							}
							foreach (Finalboss finalBoss in _finalBossList)
							{
								if (!finalBoss.Dead && rectangle.Intersects(finalBoss.BoxRectangle))
								{
									projectile2.ShowTime = 1000.0;
									projectile2.Show = false;
									Random random = new Random((int)DateTime.Now.Ticks);
									finalBoss.TintColor = new Color(random.Next(1, 255), random.Next(1, 255), random.Next(1, 255));
									finalBoss.Hit = true;
									_bossHit.Play();
									if (--finalBoss.Life <= 0)
									{
										_score += finalBoss.Score * _difficulty;
										finalBoss.Dead = true;
										_hit.Play();
										finalBoss.NextFrameIndexOfExplosion = 0;
										finalBoss.ShowExplosion = true;
										finalBoss.PositionOfExplosion = finalBoss.Position;
										_gameComplete = true;
										MediaPlayer.Play(_levelFinish);
									}
								}
							}
						}
						foreach (Projectile assistProjectile2 in _player.AssistProjectiles)
						{
							if (!assistProjectile2.Show)
							{
								continue;
							}
							if (assistProjectile2.Position.X > (float)base.GraphicsDevice.Viewport.Width)
							{
								assistProjectile2.Show = false;
								continue;
							}
							assistProjectile2.Position = new Vector2(assistProjectile2.Position.X + 20f, assistProjectile2.Position.Y);
							Rectangle rectangle = new Rectangle((int)assistProjectile2.Position.X, (int)assistProjectile2.Position.Y, _player.SourceProjectile.Width, _player.SourceProjectile.Height);
							foreach (Bird bird3 in _birds)
							{
								if (!bird3.Dead && rectangle.Intersects(bird3.BoxRectangle))
								{
									bird3.Dead = true;
									assistProjectile2.Show = false;
									_hit.Play();
									bird3.NextFrameIndexOfExplosion = 0;
									bird3.ShowExplosion = true;
									bird3.PositionOfExplosion = bird3.Position;
								}
							}
							foreach (Enemy enemy3 in _enemies)
							{
								if (!enemy3.Dead && rectangle.Intersects(enemy3.BoxRectangle))
								{
									assistProjectile2.ShowTime = 1000.0;
									assistProjectile2.Show = false;
									if (--enemy3.Life <= 0)
									{
										_score += enemy3.Score * _difficulty;
										enemy3.Dead = true;
										_hit.Play();
										enemy3.NextFrameIndexOfExplosion = 0;
										enemy3.ShowExplosion = true;
										enemy3.PositionOfExplosion = enemy3.Position;
									}
								}
							}
							foreach (Enemy6 item12 in _enemies6)
							{
								if (!item12.Dead && rectangle.Intersects(item12.BoxRectangle))
								{
									assistProjectile2.ShowTime = 1000.0;
									assistProjectile2.Show = false;
									if (--item12.Life <= 0)
									{
										_score += item12.Score * _difficulty;
										item12.Dead = true;
										_hit.Play();
										item12.NextFrameIndexOfExplosion = 0;
										item12.ShowExplosion = true;
										item12.PositionOfExplosion = item12.Position;
									}
								}
							}
							foreach (Enemy2 item13 in _enemies2)
							{
								if (!item13.Dead && rectangle.Intersects(item13.BoxRectangle))
								{
									assistProjectile2.ShowTime = 1000.0;
									assistProjectile2.Show = false;
									if (--item13.Life <= 0)
									{
										_score += item13.Score * _difficulty;
										item13.Dead = true;
										_hit.Play();
										item13.NextFrameIndexOfExplosion = 0;
										item13.ShowExplosion = true;
										item13.PositionOfExplosion = item13.Position;
									}
								}
							}
							foreach (Enemy3 item14 in _enemies3)
							{
								if (!item14.Dead && rectangle.Intersects(item14.BoxRectangle))
								{
									assistProjectile2.ShowTime = 1000.0;
									assistProjectile2.Show = false;
									if (--item14.Life <= 0)
									{
										_score += item14.Score * _difficulty;
										item14.Dead = true;
										_hit.Play();
										item14.NextFrameIndexOfExplosion = 0;
										item14.ShowExplosion = true;
										item14.PositionOfExplosion = item14.Position;
									}
								}
							}
							foreach (Enemy4 item15 in _enemies4)
							{
								if (!item15.Dead && rectangle.Intersects(item15.BoxRectangle))
								{
									assistProjectile2.ShowTime = 1000.0;
									assistProjectile2.Show = false;
									if (--item15.Life <= 0)
									{
										_score += item15.Score * _difficulty;
										item15.Dead = true;
										_hit.Play();
										item15.NextFrameIndexOfExplosion = 0;
										item15.ShowExplosion = true;
										item15.PositionOfExplosion = item15.Position;
									}
								}
							}
							foreach (Enemy7 item16 in _enemies7)
							{
								if (!item16.Dead && rectangle.Intersects(item16.BoxRectangle))
								{
									assistProjectile2.ShowTime = 1000.0;
									assistProjectile2.Show = false;
									if (--item16.Life <= 0)
									{
										_score += item16.Score * _difficulty;
										item16.Dead = true;
										_hit.Play();
										item16.NextFrameIndexOfExplosion = 0;
										item16.ShowExplosion = true;
										item16.PositionOfExplosion = item16.Position;
									}
								}
							}
							foreach (Enemy5 item17 in _enemies5)
							{
								if (!item17.Dead && rectangle.Intersects(item17.BoxRectangle))
								{
									assistProjectile2.ShowTime = 1000.0;
									assistProjectile2.Show = false;
									if (--item17.Life <= 0)
									{
										_score += item17.Score * _difficulty;
										item17.Dead = true;
										_hit.Play();
										item17.NextFrameIndexOfExplosion = 0;
										item17.ShowExplosion = true;
										item17.PositionOfExplosion = item17.Position;
									}
								}
							}
							foreach (Miniboss miniBoss2 in _miniBossList)
							{
								if (!miniBoss2.Dead && rectangle.Intersects(miniBoss2.BoxRectangle))
								{
									assistProjectile2.ShowTime = 1000.0;
									assistProjectile2.Show = false;
									Random random = new Random((int)DateTime.Now.Ticks);
									miniBoss2.TintColor = new Color(random.Next(1, 255), random.Next(1, 255), random.Next(1, 255));
									miniBoss2.Hit = true;
									_bossHit.Play();
									if (--miniBoss2.Life <= 0)
									{
										_score += miniBoss2.Score * _difficulty;
										miniBoss2.Dead = true;
										_hit.Play();
										miniBoss2.NextFrameIndexOfExplosion = 0;
										miniBoss2.ShowExplosion = true;
										miniBoss2.PositionOfExplosion = miniBoss2.Position;
										_miniBossDeadCount++;
									}
								}
							}
							foreach (Finalboss finalBoss2 in _finalBossList)
							{
								if (!finalBoss2.Dead && rectangle.Intersects(finalBoss2.BoxRectangle))
								{
									assistProjectile2.ShowTime = 1000.0;
									assistProjectile2.Show = false;
									Random random = new Random((int)DateTime.Now.Ticks);
									finalBoss2.TintColor = new Color(random.Next(1, 255), random.Next(1, 255), random.Next(1, 255));
									finalBoss2.Hit = true;
									_bossHit.Play();
									if (--finalBoss2.Life <= 0)
									{
										_score += finalBoss2.Score * _difficulty;
										finalBoss2.Dead = true;
										_hit.Play();
										finalBoss2.NextFrameIndexOfExplosion = 0;
										finalBoss2.ShowExplosion = true;
										finalBoss2.PositionOfExplosion = finalBoss2.Position;
										_gameComplete = true;
										MediaPlayer.Play(_levelFinish);
										_player.Dead = true;
									}
								}
							}
						}
						foreach (Enemy enemy4 in _enemies)
						{
							foreach (Projectile projectile3 in enemy4.Projectiles)
							{
								projectile3.ShowTime += gameTime.ElapsedGameTime.TotalMilliseconds;
								if (projectile3.ShowTime >= 1000.0 && !enemy4.Dead)
								{
									projectile3.Show = true;
									projectile3.Position = new Vector2(enemy4.CenterRight.X - (float)enemy4.Width, enemy4.CenterRight.Y);
									projectile3.ShowTime = 0.0;
								}
								if (!projectile3.Show)
								{
									continue;
								}
								if (projectile3.Position.X <= 0f)
								{
									projectile3.Show = false;
									continue;
								}
								projectile3.Position = new Vector2(projectile3.Position.X - 10f, projectile3.Position.Y);
								Rectangle rectangle = new Rectangle((int)projectile3.Position.X - enemy4.SourceProjectile.Width, (int)projectile3.Position.Y, enemy4.SourceProjectile.Width, enemy4.SourceProjectile.Height);
								if (!_player.Dead && !_player.Hit && rectangle.Intersects(_player.BoxRectangle) && !_player.Invincibility)
								{
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									projectile3.Show = false;
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
						}
						foreach (Enemy6 item18 in _enemies6)
						{
							foreach (Projectile projectile4 in item18.Projectiles)
							{
								projectile4.ShowTime += gameTime.ElapsedGameTime.TotalMilliseconds;
								if (projectile4.ShowTime >= 1000.0 && !item18.Dead)
								{
									projectile4.Show = true;
									projectile4.Position = new Vector2(item18.CenterRight.X - (float)item18.Width, item18.CenterRight.Y);
									projectile4.ShowTime = 0.0;
								}
								if (!projectile4.Show)
								{
									continue;
								}
								if (projectile4.Position.X <= 0f)
								{
									projectile4.Show = false;
									continue;
								}
								projectile4.Position = new Vector2(projectile4.Position.X - 10f, projectile4.Position.Y);
								Rectangle rectangle = new Rectangle((int)projectile4.Position.X - item18.SourceProjectile.Width, (int)projectile4.Position.Y, item18.SourceProjectile.Width, item18.SourceProjectile.Height);
								if (!_player.Dead && !_player.Hit && rectangle.Intersects(_player.BoxRectangle) && !_player.Invincibility)
								{
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									projectile4.Show = false;
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
						}
						foreach (Enemy3 item19 in _enemies3)
						{
							foreach (Projectile projectile5 in item19.Projectiles)
							{
								projectile5.ShowTime += gameTime.ElapsedGameTime.TotalMilliseconds;
								if (projectile5.ShowTime >= 1000.0 && !item19.Dead)
								{
									projectile5.Show = true;
									projectile5.Position = new Vector2(item19.CenterRight.X - (float)item19.Width, item19.CenterRight.Y);
									projectile5.ShowTime = 0.0;
								}
								if (!projectile5.Show)
								{
									continue;
								}
								if (projectile5.Position.X <= 0f)
								{
									projectile5.Show = false;
									continue;
								}
								projectile5.Position = new Vector2(projectile5.Position.X - 10f, projectile5.Position.Y);
								Rectangle rectangle = new Rectangle((int)projectile5.Position.X - item19.SourceProjectile.Width, (int)projectile5.Position.Y, item19.SourceProjectile.Width, item19.SourceProjectile.Height);
								if (!_player.Dead && !_player.Hit && rectangle.Intersects(_player.BoxRectangle) && !_player.Invincibility)
								{
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									projectile5.Show = false;
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
						}
						foreach (Enemy4 item20 in _enemies4)
						{
							foreach (Projectile projectile6 in item20.Projectiles)
							{
								projectile6.ShowTime += gameTime.ElapsedGameTime.TotalMilliseconds;
								if (projectile6.ShowTime >= 1000.0 && !item20.Dead)
								{
									projectile6.Show = true;
									projectile6.Position = new Vector2(item20.CenterRight.X - (float)item20.Width, item20.CenterRight.Y);
									projectile6.ShowTime = 0.0;
								}
								if (!projectile6.Show)
								{
									continue;
								}
								if (projectile6.Position.Y >= (float)base.GraphicsDevice.Viewport.Height)
								{
									projectile6.Show = false;
									continue;
								}
								projectile6.Position = new Vector2(projectile6.Position.X - 10f, projectile6.Position.Y + 10f);
								Rectangle rectangle = new Rectangle((int)projectile6.Position.X - item20.SourceProjectile.Width, (int)projectile6.Position.Y, item20.SourceProjectile.Width, item20.SourceProjectile.Height);
								if (!_player.Dead && !_player.Hit && rectangle.Intersects(_player.BoxRectangle) && !_player.Invincibility)
								{
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									projectile6.Show = false;
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
						}
						foreach (Enemy7 item21 in _enemies7)
						{
							foreach (Projectile projectile7 in item21.Projectiles)
							{
								projectile7.ShowTime += gameTime.ElapsedGameTime.TotalMilliseconds;
								if (projectile7.ShowTime >= 2000.0 && !item21.Dead)
								{
									projectile7.Show = true;
									projectile7.Position = new Vector2(item21.CenterRight.X - (float)item21.Width, item21.CenterRight.Y);
									projectile7.ShowTime = 0.0;
								}
								if (!projectile7.Show)
								{
									continue;
								}
								if (projectile7.Position.Y >= (float)(base.GraphicsDevice.Viewport.Height + item21.Height))
								{
									projectile7.Show = false;
									continue;
								}
								if (projectile7.Position.X >= _player.Position.X - 5f && projectile7.Position.X <= _player.Position.X + 5f)
								{
									projectile7.Position = new Vector2(projectile7.Position.X, projectile7.Position.Y + 5f);
								}
								else if (projectile7.Position.X <= _player.Position.X)
								{
									projectile7.Position = new Vector2(projectile7.Position.X + 5f, projectile7.Position.Y + 5f);
								}
								else
								{
									projectile7.Position = new Vector2(projectile7.Position.X - 5f, projectile7.Position.Y + 5f);
								}
								Rectangle rectangle = new Rectangle((int)projectile7.Position.X - item21.SourceProjectile.Width, (int)projectile7.Position.Y, item21.SourceProjectile.Width, item21.SourceProjectile.Height);
								if (!_player.Dead && !_player.Hit && rectangle.Intersects(_player.BoxRectangle) && !_player.Invincibility)
								{
									_damage = 1;
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									projectile7.Show = false;
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
						}
						foreach (Enemy5 item22 in _enemies5)
						{
							foreach (Projectile projectile8 in item22.Projectiles)
							{
								projectile8.ShowTime += gameTime.ElapsedGameTime.TotalMilliseconds;
								if (projectile8.ShowTime >= 1000.0 && !item22.Dead)
								{
									projectile8.Show = true;
									projectile8.Position = new Vector2(item22.CenterRight.X - (float)item22.Width, item22.CenterRight.Y);
									projectile8.ShowTime = 0.0;
								}
								if (!projectile8.Show)
								{
									continue;
								}
								if (projectile8.Position.X <= 0f)
								{
									projectile8.Show = false;
									continue;
								}
								projectile8.Position = new Vector2(projectile8.Position.X - 20f, projectile8.Position.Y);
								Rectangle rectangle = new Rectangle((int)projectile8.Position.X - item22.SourceProjectile.Width, (int)projectile8.Position.Y, item22.SourceProjectile.Width, item22.SourceProjectile.Height);
								if (!_player.Dead && !_player.Hit && rectangle.Intersects(_player.BoxRectangle) && !_player.Invincibility)
								{
									_damage = 1;
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									projectile8.Show = false;
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
						}
						foreach (Miniboss miniBoss3 in _miniBossList)
						{
							foreach (Projectile projectile9 in miniBoss3.Projectiles)
							{
								projectile9.ShowTime += gameTime.ElapsedGameTime.TotalMilliseconds;
								if (projectile9.ShowTime >= 1000.0 && !miniBoss3.Dead)
								{
									projectile9.Show = true;
									projectile9.Position = new Vector2(miniBoss3.CenterRight.X - (float)miniBoss3.Width, miniBoss3.CenterRight.Y);
									projectile9.ShowTime = 0.0;
								}
								if (!projectile9.Show)
								{
									continue;
								}
								if (projectile9.Position.X <= 0f)
								{
									projectile9.Show = false;
									continue;
								}
								projectile9.Position = new Vector2(projectile9.Position.X - 30f, projectile9.Position.Y);
								Rectangle rectangle = new Rectangle((int)projectile9.Position.X - miniBoss3.SourceProjectile.Width, (int)projectile9.Position.Y, miniBoss3.SourceProjectile.Width, miniBoss3.SourceProjectile.Height);
								if (!_player.Dead && !_player.Hit && rectangle.Intersects(_player.BoxRectangle) && !_player.Invincibility)
								{
									_damage = 2;
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									projectile9.Show = false;
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
						}
						foreach (Finalboss finalBoss3 in _finalBossList)
						{
							int num4 = 1;
							foreach (Projectile projectile10 in finalBoss3.Projectiles)
							{
								projectile10.ShowTime += gameTime.ElapsedGameTime.TotalMilliseconds;
								if (projectile10.ShowTime >= 2000.0 && !finalBoss3.Dead)
								{
									projectile10.Show = true;
									projectile10.Position = new Vector2(finalBoss3.CenterRight.X - (float)finalBoss3.Width, finalBoss3.CenterRight.Y);
									projectile10.ShowTime = 0.0;
								}
								if (!projectile10.Show)
								{
									continue;
								}
								if (projectile10.Position.X <= 0f)
								{
									projectile10.Show = false;
								}
								else
								{
									if (projectile10.Position.Y >= _player.Position.Y - 5f && projectile10.Position.Y <= _player.Position.Y + 5f)
									{
										projectile10.Position = new Vector2(projectile10.Position.X - 5f + (float)num4, projectile10.Position.Y);
									}
									if (projectile10.Position.Y >= _player.Position.Y)
									{
										projectile10.Position = new Vector2(projectile10.Position.X - 5f + (float)num4, projectile10.Position.Y - 5f);
									}
									else
									{
										projectile10.Position = new Vector2(projectile10.Position.X - 5f + (float)num4, projectile10.Position.Y + 5f);
									}
									Rectangle rectangle = new Rectangle((int)projectile10.Position.X - finalBoss3.SourceProjectile.Width, (int)projectile10.Position.Y, finalBoss3.SourceProjectile.Width, finalBoss3.SourceProjectile.Height);
									if (!_player.Dead && !_player.Hit && rectangle.Intersects(_player.BoxRectangle) && !_player.Invincibility)
									{
										_damage = 3;
										_player.Hit = true;
										_player.HitTime = 0.0;
										_playerHit.Play();
										projectile10.Show = false;
										CheckForItemQualify();
										if (_itemQualify)
										{
											_selectionBorderShow = true;
										}
									}
								}
								num4 += 3;
							}
						}
						_layer1LeftToDrawOnScreen = base.GraphicsDevice.Viewport.Width - _layer1Split.Width;
						if (_currentLevel >= 2)
						{
							_layer1Scroll = (int)(0.0 - GetTotalGameTime(gameTime) / 5.0 % (double)_layer1Split.Width);
						}
						if (!_powerupItem.Hidden && _player.BoxRectangle.Intersects(_powerupItem.BoxRectangle) && !_player.Dead)
						{
							_pickup.Play();
							_powerupItem.Hidden = true;
							_powerupItems++;
							CheckForItemQualify();
						}
						_powerupItem.Position = new Vector2(_powerupItem.Position.X - 2f, _powerupItem.Position.Y);
						if (_powerupItem.Position.X <= -1000f)
						{
							_powerupItem.Position = _powerupItem.RandomLocation(_screenSize.Right, _screenSize.Bottom);
						}
						if (_birds.Count((Bird p) => p.Dead) == _birds.Count)
						{
							_powerupItem.Position = _birds.First().Position;
							_powerupItem.Hidden = false;
							Bird.ResetAllBirdPositions(_birds, base.GraphicsDevice, _screenSize);
						}
						if (_birds.Count((Bird p) => p.Position.X <= 0f) != 0)
						{
							Bird.ResetAllBirdPositions(_birds, base.GraphicsDevice, _screenSize);
						}
						if ((DateTime.Now - _powerupItem.FrameTime).TotalMilliseconds >= 250.0)
						{
							if (++_powerupItem.NextFrameIndex >= _powerupItem.SourceRectangles.Count)
							{
								_powerupItem.NextFrameIndex = 0;
							}
							_powerupItem.FrameTime = DateTime.Now;
						}
						foreach (Bird bird4 in _birds)
						{
							if (!_player.Hit && _player.BoxRectangle.Intersects(bird4.BoxRectangle) && !bird4.Dead && !_player.Dead && !_player.Invincibility)
							{
								_player.Hit = true;
								_player.HitTime = 0.0;
								_playerHit.Play();
								CheckForItemQualify();
								if (_itemQualify)
								{
									_selectionBorderShow = true;
								}
							}
							if ((DateTime.Now - bird4.FrameTime).TotalMilliseconds >= 100.0)
							{
								if (!bird4.Dead)
								{
									if (!bird4.Reverse)
									{
										if (++bird4.NextFrameIndex >= bird4.SourceRectangles.Count - 1)
										{
											bird4.Reverse = true;
										}
									}
									else if (--bird4.NextFrameIndex <= 1)
									{
										bird4.Reverse = false;
									}
									float num5 = bird4.Position.X - 5f;
									double num6 = Math.Sin(MathHelper.ToRadians(num5)) * 7.0;
									bird4.Position = new Vector2(num5, (int)((double)bird4.Position.Y + num6));
								}
								bird4.FrameTime = DateTime.Now;
							}
							if (bird4.Dead && bird4.ShowExplosion && (DateTime.Now - bird4.ExplosionFrameTime).TotalMilliseconds >= 33.0)
							{
								if (++bird4.NextFrameIndexOfExplosion >= bird4.SourceOfExplosion.Count)
								{
									bird4.ShowExplosion = false;
									bird4.NextFrameIndexOfExplosion = 0;
								}
								bird4.ExplosionFrameTime = DateTime.Now;
							}
						}
						if (!_player.Hit)
						{
							foreach (Enemy enemy5 in _enemies)
							{
								if (_player.BoxRectangle.Intersects(enemy5.BoxRectangle) && !enemy5.Dead && !_player.Dead && !_player.Invincibility)
								{
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
							foreach (Enemy6 item23 in _enemies6)
							{
								if (_player.BoxRectangle.Intersects(item23.BoxRectangle) && !item23.Dead && !_player.Dead && !_player.Invincibility)
								{
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
							foreach (Enemy2 item24 in _enemies2)
							{
								if (_player.BoxRectangle.Intersects(item24.BoxRectangle) && !item24.Dead && !_player.Dead && !_player.Invincibility)
								{
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
							foreach (Enemy3 item25 in _enemies3)
							{
								if (_player.BoxRectangle.Intersects(item25.BoxRectangle) && !item25.Dead && !_player.Dead && !_player.Invincibility)
								{
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
							foreach (Enemy4 item26 in _enemies4)
							{
								if (_player.BoxRectangle.Intersects(item26.BoxRectangle) && !item26.Dead && !_player.Dead && !_player.Invincibility)
								{
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
							foreach (Enemy7 item27 in _enemies7)
							{
								if (_player.BoxRectangle.Intersects(item27.BoxRectangle) && !item27.Dead && !_player.Dead && !_player.Invincibility)
								{
									_damage = 1;
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
							foreach (Enemy5 item28 in _enemies5)
							{
								if (_player.BoxRectangle.Intersects(item28.BoxRectangle) && !item28.Dead && !_player.Dead && !_player.Invincibility)
								{
									_damage = 1;
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
							foreach (Miniboss miniBoss4 in _miniBossList)
							{
								if (_player.BoxRectangle.Intersects(miniBoss4.BoxRectangle) && !miniBoss4.Dead && !_player.Dead && !_player.Invincibility)
								{
									_damage = 2;
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
							foreach (Finalboss finalBoss4 in _finalBossList)
							{
								if (_player.BoxRectangle.Intersects(finalBoss4.BoxRectangle) && !finalBoss4.Dead && !_player.Dead && !_player.Invincibility)
								{
									_damage = 3;
									_player.Hit = true;
									_player.HitTime = 0.0;
									_playerHit.Play();
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
								}
							}
						}
						foreach (Enemy enemy6 in _enemies)
						{
							if ((DateTime.Now - enemy6.FrameTime).TotalMilliseconds >= 33.0)
							{
								if (++enemy6.NextFrameIndex >= enemy6.SourceRectangles.Count - 1)
								{
									enemy6.NextFrameIndex = 0;
								}
								if ((DateTime.Now - enemy6.ChangePositionTime).TotalMilliseconds >= 500.0)
								{
									enemy6.ChangePositionTime = DateTime.Now;
									enemy6.ReversePosition = !enemy6.ReversePosition;
								}
								if (enemy6.Position.X <= 0f)
								{
									Random random = new Random((int)DateTime.Now.Ticks);
									enemy6.Position = new Vector2(_screenSize.Right + 100, random.Next(_screenSize.Top + 100, _screenSize.Bottom - 100));
								}
								else if (!enemy6.ReversePosition)
								{
									enemy6.Position = new Vector2(enemy6.Position.X - 10f, enemy6.Position.Y - 3f);
								}
								else
								{
									enemy6.Position = new Vector2(enemy6.Position.X - 10f, enemy6.Position.Y + 3f);
								}
								enemy6.FrameTime = DateTime.Now;
							}
						}
						foreach (Enemy6 item29 in _enemies6)
						{
							if ((DateTime.Now - item29.FrameTime).TotalMilliseconds >= 33.0)
							{
								if (++item29.NextFrameIndex >= item29.SourceRectangles.Count - 1)
								{
									item29.NextFrameIndex = 0;
								}
								if (item29.Position.X <= 0f)
								{
									Random random = new Random((int)DateTime.Now.Ticks);
									item29.Position = new Vector2(_screenSize.Right + 30, random.Next(60, _screenSize.Bottom - 30));
								}
								else if (item29.Position.Y >= _player.Position.Y - 5f && item29.Position.Y <= _player.Position.Y + 5f)
								{
									item29.Position = new Vector2(item29.Position.X - 10f, item29.Position.Y);
								}
								else if (item29.Position.Y >= _player.Position.Y)
								{
									item29.Position = new Vector2(item29.Position.X - 10f, item29.Position.Y - 5f);
								}
								else
								{
									item29.Position = new Vector2(item29.Position.X - 10f, item29.Position.Y + 5f);
								}
								item29.FrameTime = DateTime.Now;
							}
						}
						foreach (Enemy2 item30 in _enemies2)
						{
							if (!((DateTime.Now - item30.FrameTime).TotalMilliseconds >= 100.0))
							{
								continue;
							}
							if (item30.Reverse)
							{
								if (--item30.NextFrameIndex <= 0)
								{
									item30.Reverse = false;
								}
							}
							else if (++item30.NextFrameIndex >= item30.SourceRectangles.Count - 1)
							{
								item30.Reverse = true;
							}
							if (item30.Position.X <= 0f)
							{
								Random random = new Random((int)DateTime.Now.Ticks);
								item30.Position = new Vector2(_screenSize.Right + 100, random.Next(_screenSize.Top + 100, _screenSize.Bottom - 100));
							}
							else
							{
								item30.Position = new Vector2(item30.Position.X - 10f, item30.Position.Y);
							}
							item30.FrameTime = DateTime.Now;
						}
						foreach (Enemy3 item31 in _enemies3)
						{
							if (!((DateTime.Now - item31.FrameTime).TotalMilliseconds >= 33.0))
							{
								continue;
							}
							if (item31.Reverse)
							{
								if (--item31.NextFrameIndex <= 0)
								{
									item31.Reverse = false;
								}
							}
							else if (++item31.NextFrameIndex >= item31.SourceRectangles.Count - 1)
							{
								item31.Reverse = true;
							}
							if (item31.Position.X <= 0f)
							{
								Random random = new Random((int)DateTime.Now.Ticks);
								item31.Position = new Vector2(_screenSize.Right + 30, random.Next(60, _screenSize.Bottom - 30));
							}
							else
							{
								item31.Position = new Vector2(item31.Position.X - 10f, item31.Position.Y);
							}
							item31.FrameTime = DateTime.Now;
						}
						foreach (Enemy4 item32 in _enemies4)
						{
							if (GetTotalGameTime(gameTime) - item32.GameTime >= 33.0)
							{
								if (item32.Position.X <= 0f)
								{
									item32.Position = new Vector2(_screenSize.Right + 30, _screenSize.Top + 10);
								}
								else
								{
									item32.Position = new Vector2(item32.Position.X - (float)item32.Speed, item32.Position.Y);
								}
								item32.GameTime = GetTotalGameTime(gameTime);
							}
						}
						foreach (Enemy7 item33 in _enemies7)
						{
							if (GetTotalGameTime(gameTime) - item33.GameTime >= 33.0)
							{
								if (item33.Position.X <= 0f)
								{
									item33.Position = new Vector2(_screenSize.Right + 30, 20f);
								}
								else
								{
									item33.Position = new Vector2(item33.Position.X - (float)item33.Speed, item33.Position.Y);
								}
								item33.GameTime = GetTotalGameTime(gameTime);
							}
						}
						foreach (Enemy5 item34 in _enemies5)
						{
							if (!_player.Dead && (DateTime.Now - item34.FrameTime).TotalMilliseconds >= 33.0)
							{
								if (item34.Position.Y <= _player.Position.Y)
								{
									item34.Position = new Vector2(item34.Position.X, item34.Position.Y + 5f);
								}
								else if (item34.Position.Y - _player.Position.Y <= 5f)
								{
									item34.Position = new Vector2(item34.Position.X, item34.Position.Y);
								}
								else
								{
									item34.Position = new Vector2(item34.Position.X, item34.Position.Y - 5f);
								}
								item34.FrameTime = DateTime.Now;
							}
						}
						bool flag2 = true;
						foreach (Miniboss miniBoss5 in _miniBossList)
						{
							if (!_player.Dead && (DateTime.Now - miniBoss5.FrameTime).TotalMilliseconds >= 25.0)
							{
								if (miniBoss5.Position.Y >= (float)(_screenSize.Bottom - miniBoss5.Height) - 100f)
								{
									miniBoss5.ReversePosition = true;
								}
								else if (miniBoss5.Position.Y <= (float)_screenSize.Top + 100f)
								{
									miniBoss5.ReversePosition = false;
								}
								if (!miniBoss5.ReversePosition)
								{
									miniBoss5.Position = new Vector2(miniBoss5.Position.X, miniBoss5.Position.Y + 5f);
								}
								else
								{
									miniBoss5.Position = new Vector2(miniBoss5.Position.X, miniBoss5.Position.Y - 5f);
								}
								miniBoss5.FrameTime = DateTime.Now;
							}
							if (!miniBoss5.Dead)
							{
								flag2 = false;
							}
						}
						if (flag2 && _miniBossStart && _miniBossList.Count > 0)
						{
							_stageCleared = true;
							_levelTime[_currentLevel - 1] = gameTime.TotalGameTime.TotalMilliseconds;
							_miniBossStart = false;
						}
						_player.HitTime += gameTime.ElapsedGameTime.TotalMilliseconds;
						if (_player.Hit)
						{
							if (_player.HitTime <= 1000.0)
							{
								if ((DateTime.Now - _player.HitFrameTime).TotalMilliseconds >= 150.0)
								{
									_player.TintColor = ((_player.TintColor == Color.White) ? Color.Red : Color.White);
									_player.HitFrameTime = DateTime.Now;
								}
							}
							else
							{
								_player.TintColor = Color.White;
								_player.Hit = false;
								_player.Life -= _difficulty + _damage;
								_damage = 0;
								if (_player.Life <= 0)
								{
									_titleSelectPress = GetTotalGameTime(gameTime) + 1000.0;
									_player.Dead = true;
									_player.Assist = 0;
									_player.AssistProjectiles.Clear();
									CheckForItemQualify();
									if (_itemQualify)
									{
										_selectionBorderShow = true;
									}
									_player.NextFrameIndexOfExplosion = 0;
									_player.ShowExplosion = true;
									_player.PositionOfExplosion = _player.Position;
									_playerExplosion.Play();
								}
							}
						}
						if (_player.Dead)
						{
							if (!_countDownTime.HasValue)
							{
								_countDownTime = GetTotalGameTime(gameTime);
							}
							int num7 = 10 - (int)((GetTotalGameTime(gameTime) - _countDownTime) / 1000.0).Value;
							if (num7 <= 0 || _powerupItems < 5 * _difficulty)
							{
								_savingData = true;
								_enableCustomMessage = true;
								_customMessage = "Saving data, please wait....";
								_customMessageTime = 1500.0;
								MediaPlayer.Play(_crisson);
								_gameStarted = false;
								_title = true;
								_miniBossStart = false;
								_finalBossStart = false;
								Bird.ResetAllBirdPositions(_birds, base.GraphicsDevice, _screenSize);
								_startUpShow = true;
								_startUpTime = 0.0;
								_levelTime.Clear();
								_levelTime.Add(0.0);
								_fadeIncrement = 0f;
								_fadeOut = false;
								_stageCleared = false;
								_stageClearTime = 0.0;
								RemoveAllEnemies();
								CheckForItemQualify();
								_title = true;
								_help = false;
								_options = false;
								_paused = false;
							}
							if (GamePad.GetState(_playerIndex).Buttons.B == ButtonState.Pressed)
							{
								_player.Dead = false;
								_player.Life = 10;
								_player.Speed = 1;
								_player.Power = 3;
								_player.Projectiles.Clear();
								_player.Projectiles.Add(new Projectile());
								_player.Projectiles.Add(new Projectile());
								_player.Projectiles.Add(new Projectile());
								_player.Assist = 0;
								_player.AssistProjectiles.Clear();
								_powerupItems -= 5 * _difficulty;
								_powerup.Play();
								_selectionInventoryTime = 0.0;
								_bullets = 500f;
							}
							if ((DateTime.Now - _player.ExplosionFrameTime).TotalMilliseconds >= 33.0)
							{
								if (++_player.NextFrameIndexOfExplosion >= _player.SourceOfExplosion.Count)
								{
									_player.ShowExplosion = false;
								}
								_player.ExplosionFrameTime = DateTime.Now;
							}
						}
						else
						{
							_countDownTime = null;
						}
						foreach (Enemy enemy7 in _enemies)
						{
							if (enemy7.Dead && (DateTime.Now - enemy7.ExplosionFrameTime).TotalMilliseconds >= 33.0)
							{
								if (++enemy7.NextFrameIndexOfExplosion >= enemy7.SourceOfExplosion.Count)
								{
									enemy7.ShowExplosion = false;
								}
								enemy7.ExplosionFrameTime = DateTime.Now;
							}
						}
						foreach (Enemy6 item35 in _enemies6)
						{
							if (item35.Dead && (DateTime.Now - item35.ExplosionFrameTime).TotalMilliseconds >= 33.0)
							{
								if (++item35.NextFrameIndexOfExplosion >= item35.SourceOfExplosion.Count)
								{
									item35.ShowExplosion = false;
								}
								item35.ExplosionFrameTime = DateTime.Now;
							}
						}
						foreach (Enemy2 item36 in _enemies2)
						{
							if (item36.Dead && (DateTime.Now - item36.ExplosionFrameTime).TotalMilliseconds >= 33.0)
							{
								if (++item36.NextFrameIndexOfExplosion >= item36.SourceOfExplosion.Count)
								{
									item36.ShowExplosion = false;
								}
								item36.ExplosionFrameTime = DateTime.Now;
							}
						}
						foreach (Enemy3 item37 in _enemies3)
						{
							if (item37.Dead && (DateTime.Now - item37.ExplosionFrameTime).TotalMilliseconds >= 33.0)
							{
								if (++item37.NextFrameIndexOfExplosion >= item37.SourceOfExplosion.Count)
								{
									item37.ShowExplosion = false;
								}
								item37.ExplosionFrameTime = DateTime.Now;
							}
						}
						foreach (Enemy4 item38 in _enemies4)
						{
							if (item38.Dead && (DateTime.Now - item38.ExplosionFrameTime).TotalMilliseconds >= 33.0)
							{
								if (++item38.NextFrameIndexOfExplosion >= item38.SourceOfExplosion.Count)
								{
									item38.ShowExplosion = false;
								}
								item38.ExplosionFrameTime = DateTime.Now;
							}
						}
						foreach (Enemy7 item39 in _enemies7)
						{
							if (item39.Dead && (DateTime.Now - item39.ExplosionFrameTime).TotalMilliseconds >= 33.0)
							{
								if (++item39.NextFrameIndexOfExplosion >= item39.SourceOfExplosion.Count)
								{
									item39.ShowExplosion = false;
								}
								item39.ExplosionFrameTime = DateTime.Now;
							}
						}
						foreach (Enemy5 item40 in _enemies5)
						{
							if (item40.Dead && (DateTime.Now - item40.ExplosionFrameTime).TotalMilliseconds >= 33.0)
							{
								if (++item40.NextFrameIndexOfExplosion >= item40.SourceOfExplosion.Count)
								{
									item40.ShowExplosion = false;
								}
								item40.ExplosionFrameTime = DateTime.Now;
							}
						}
						foreach (Miniboss miniBoss6 in _miniBossList)
						{
							if (miniBoss6.Dead && (DateTime.Now - miniBoss6.ExplosionFrameTime).TotalMilliseconds >= 33.0)
							{
								if (++miniBoss6.NextFrameIndexOfExplosion >= miniBoss6.SourceOfExplosion.Count)
								{
									miniBoss6.ShowExplosion = false;
								}
								miniBoss6.ExplosionFrameTime = DateTime.Now;
							}
						}
						foreach (Finalboss finalBoss5 in _finalBossList)
						{
							if (finalBoss5.Dead && (DateTime.Now - finalBoss5.ExplosionFrameTime).TotalMilliseconds >= 33.0)
							{
								if (++finalBoss5.NextFrameIndexOfExplosion >= finalBoss5.SourceOfExplosion.Count)
								{
									finalBoss5.ShowExplosion = false;
								}
								finalBoss5.ExplosionFrameTime = DateTime.Now;
							}
						}
						double num8 = 1.0;
						if (_currentLevel >= 7)
						{
							num8 = 4.0;
						}
						if (GetTotalGameTime(gameTime) - _spawnTime >= 5000.0 / num8 && !_stageCleared)
						{
							if (_currentLevel >= 9)
							{
								if (_finalBossList.Count < 1)
								{
									AddNewFinalboss();
									SetTextureExplosionForEnemy(_finalBossList.Last());
								}
							}
							else
							{
								if (_currentLevel >= 5 && _miniBossStart)
								{
									if (_miniBossList.Count <= 5)
									{
										AddNewMiniboss();
										SetTextureExplosionForEnemy(_miniBossList.Last());
									}
								}
								else if (!_miniBossStart)
								{
									if (_enemies.Count < 10)
									{
										AddNewEnemy();
										SetTextureExplosionForEnemy(_enemies.Last());
									}
									if (_currentLevel >= 6)
									{
										if (_enemies6.Count < 3)
										{
											AddNewEnemy6();
											SetTextureExplosionForEnemy(_enemies6.Last());
										}
										if (_enemies7.Count < 2)
										{
											AddNewEnemy7();
											SetTextureExplosionForEnemy(_enemies7.Last());
										}
									}
									if (_enemies2.Count < 10)
									{
										AddNewEnemy2();
										SetTextureExplosionForEnemy(_enemies2.Last());
									}
									if (_currentLevel >= 2 && _enemies4.Count < 2)
									{
										AddNewEnemy4();
										SetTextureExplosionForEnemy(_enemies4.Last());
									}
									if (_currentLevel >= 3 && _enemies3.Count < 2)
									{
										AddNewEnemy3();
										SetTextureExplosionForEnemy(_enemies3.Last());
									}
									if (_currentLevel >= 4 && _enemies5.Count < 1)
									{
										AddNewEnemy5();
										SetTextureExplosionForEnemy(_enemies5.Last());
									}
								}
								if (_currentLevel <= 4)
								{
									foreach (Enemy enemy8 in _enemies)
									{
										if (enemy8.Dead)
										{
											Random random = new Random((int)DateTime.Now.Ticks);
											enemy8.Position = new Vector2(_screenSize.Right + 100, random.Next(_screenSize.Top + 100, _screenSize.Bottom - 100));
											enemy8.Dead = false;
											enemy8.Life = 2 + _difficulty;
										}
									}
									foreach (Enemy2 item41 in _enemies2)
									{
										if (item41.Dead)
										{
											item41.Position = new Vector2(_screenSize.Right + 100, item41.Position.Y);
											item41.Dead = false;
											item41.Life = 3 + _difficulty;
										}
									}
									foreach (Enemy3 item42 in _enemies3)
									{
										if (item42.Dead)
										{
											item42.Position = new Vector2(_screenSize.Right + 30, item42.Position.Y);
											item42.Dead = false;
											item42.Life = 2 + _difficulty;
										}
									}
									foreach (Enemy4 item43 in _enemies4)
									{
										if (item43.Dead)
										{
											item43.Position = new Vector2(_screenSize.Right + 30, item43.Position.Y);
											item43.Dead = false;
											item43.Life = 5 + _difficulty;
										}
									}
									foreach (Enemy7 item44 in _enemies7)
									{
										if (item44.Dead)
										{
											item44.Position = new Vector2(_screenSize.Right + 30, item44.Position.Y);
											item44.Dead = false;
											item44.Life = 5 + _difficulty;
										}
									}
									foreach (Enemy5 item45 in _enemies5)
									{
										if (item45.Dead)
										{
											item45.Position = new Vector2(_screenSize.Right - item45.Width - 10, item45.Position.Y);
											item45.Dead = false;
											item45.Life = 10 + _difficulty;
										}
									}
								}
								if (_currentLevel >= 6)
								{
									foreach (Enemy6 item46 in _enemies6)
									{
										if (item46.Dead)
										{
											item46.Position = new Vector2(_screenSize.Right + 30, item46.Position.Y);
											item46.Dead = false;
											item46.Life = 5 + _difficulty;
										}
									}
								}
								if (_miniBossStart && (_miniBossList.Count <= 5 || _miniBossDeadCount <= 5))
								{
									foreach (Miniboss miniBoss7 in _miniBossList)
									{
										if (miniBoss7.Dead)
										{
											miniBoss7.Position = new Vector2(_screenSize.Right - miniBoss7.Width - 10, miniBoss7.Position.Y);
											miniBoss7.Dead = false;
											miniBoss7.Life = 20 + _difficulty;
										}
									}
								}
							}
							CheckForItemQualify();
							_spawnTime = GetTotalGameTime(gameTime);
						}
						if (_powerupItems > 0)
						{
							if (_selectionBorderFlashTime >= 250.0)
							{
								_selectionBorderShow = !_selectionBorderShow;
								_selectionBorderFlashTime = 0.0;
							}
							_selectionBorderFlashTime += gameTime.ElapsedGameTime.TotalMilliseconds;
						}
						else
						{
							_selectionBorderShow = false;
						}
						if ((DateTime.Now - _player.InvincibilityTimeFlash).TotalMilliseconds >= 50.0 && _player.Invincibility)
						{
							_player.InvisibilityColor = ((_player.InvisibilityColor == Color.White) ? Color.Black : Color.White);
							_player.InvincibilityTimeFlash = DateTime.Now;
						}
						if ((DateTime.Now - _player.InvincibilityTime).TotalMilliseconds >= _invisibilityTime / (double)_difficulty && _player.Invincibility)
						{
							MediaPlayer.Play(_currentSong);
							_player.Invincibility = false;
							CheckForItemQualify();
						}
						_startUpTime += gameTime.ElapsedGameTime.Milliseconds;
						if (_startUpTime >= 15000.0)
						{
							_startUpShow = false;
							_startUpTime = 0.0;
						}
						if (!_player.Dead)
						{
							if (!_paused && _levelTime.Count == _currentLevel)
							{
								_levelTime[_currentLevel - 1] += gameTime.ElapsedGameTime.TotalMilliseconds;
							}
							if (_stageCleared)
							{
								_stageClearTime += gameTime.ElapsedGameTime.Milliseconds;
							}
							if (_levelTime[_currentLevel - 1] >= 120000.0 && !_miniBossStart && !_finalBossStart && !_player.Dead)
							{
								_currentLevel++;
								if (_currentLevel == 5)
								{
									MediaPlayer.Play(_warningBlitz);
									_miniBossStart = true;
									_currentSong = _warningBlitz;
								}
								else if (_currentLevel == 9)
								{
									MediaPlayer.Play(_xMorph);
									_finalBossStart = true;
									_currentSong = _xMorph;
								}
								else
								{
									MediaPlayer.Play(_levelFinish);
									_stageCleared = true;
									_startUpShow = true;
									_startUpTime = 0.0;
									_fadeIncrement = 0f;
									_fadeOut = false;
									_stageClearTime = 0.0;
									RemoveAllEnemies();
								}
								_levelTime.Add(0.0);
							}
							else if (_stageCleared && _stageClearTime >= 10000.0)
							{
								if (Guide.IsTrialMode)
								{
									_title = true;
									_paused = true;
									_titleSelectPress = GetTotalGameTime(gameTime) + 1000.0;
								}
								else
								{
									switch (_currentLevel)
									{
									case 1:
										MediaPlayer.Play(_crystalis);
										_currentSong = _crystalis;
										break;
									case 2:
										MediaPlayer.Play(_angryRobot);
										_currentSong = _angryRobot;
										_layer1StartFadeTime = GetTotalGameTime(gameTime);
										break;
									case 3:
										MediaPlayer.Play(_azimuth);
										_currentSong = _azimuth;
										break;
									case 4:
										MediaPlayer.Play(_blueChill);
										_currentSong = _blueChill;
										break;
									case 6:
										MediaPlayer.Play(_clickThenAction);
										_currentSong = _clickThenAction;
										break;
									case 7:
										MediaPlayer.Play(_collidescope);
										_currentSong = _collidescope;
										break;
									case 8:
										MediaPlayer.Play(_confuze);
										_currentSong = _confuze;
										break;
									}
									_spawnTime = GetTotalGameTime(gameTime);
									_stageCleared = false;
									_startUpShow = true;
									_startUpTime = 0.0;
									_fadeIncrement = 0f;
									_fadeOut = false;
								}
							}
						}
						if (_score >= _player.NextPowerupBonus)
						{
							_bullets = 500f;
							_player.NextPowerupBonus += 2500 + (_difficulty + 250);
							_pickup.Play();
							_powerupItems += 2 + _difficulty;
							if (_player.Life >= 7)
							{
								_player.Life = 10;
							}
							else
							{
								_player.Life += 5 - _difficulty;
							}
							CheckForItemQualify();
						}
						_rechargeBulletsTime += gameTime.ElapsedGameTime.TotalMilliseconds;
						if (_rechargeBulletsTime > 500.0)
						{
							_rechargeBulletsTime = 0.0;
							if (++_bullets > 500f)
							{
								_bullets = 500f;
							}
						}
					}
				}
			}
		}
		base.Update(gameTime);
	}

	private void GetDeviceForLoad(IAsyncResult result)
	{
		_device = StorageDevice.EndShowSelector(result);
		if (_device == null || !_device.IsConnected)
		{
			return;
		}
		result = _device.BeginOpenContainer("StorageDemo", null, null);
		result.AsyncWaitHandle.WaitOne();
		if (_device != null && _device.IsConnected)
		{
			StorageContainer storageContainer = _device.EndOpenContainer(result);
			result.AsyncWaitHandle.Close();
			string file = "savegame.sav";
			string[] fileNames = storageContainer.GetFileNames();
			if (!storageContainer.FileExists(file))
			{
				storageContainer.Dispose();
				return;
			}
			Stream stream = storageContainer.OpenFile(file, FileMode.Open);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<SaveGameData>));
			_data = (List<SaveGameData>)xmlSerializer.Deserialize(stream);
			stream.Close();
			storageContainer.Dispose();
		}
	}

	private void GetDeviceForSave(IAsyncResult result)
	{
		SaveGameData item = new SaveGameData
		{
			Level = _currentLevel,
			Difficulty = (Difficulty)_difficulty,
			Score = _score,
			SaveTime = DateTime.Now
		};
		if (_data == null)
		{
			_data = new List<SaveGameData>();
		}
		SignedInGamer signedInGamer = Gamer.SignedInGamers[_playerIndex];
		if (signedInGamer != null)
		{
			item.PlayerName = signedInGamer.Gamertag;
			_data.Add(item);
		}
		else
		{
			item.PlayerName = string.Format(CultureInfo.InvariantCulture, "Player {0}", new object[1] { _playerIndex });
			_data.Add(item);
		}
		if (_device != null && _device.IsConnected)
		{
			result = _device.BeginOpenContainer("StorageDemo", null, null);
			result.AsyncWaitHandle.WaitOne();
			StorageContainer storageContainer = _device.EndOpenContainer(result);
			result.AsyncWaitHandle.Close();
			string file = "savegame.sav";
			if (storageContainer.FileExists(file))
			{
				storageContainer.DeleteFile(file);
			}
			string[] fileNames = storageContainer.GetFileNames();
			Stream stream = storageContainer.CreateFile(file);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<SaveGameData>));
			xmlSerializer.Serialize(stream, _data);
			stream.Close();
			storageContainer.Dispose();
		}
	}

	private void CreateDummyData()
	{
		_data = new List<SaveGameData>();
		_data.Add(new SaveGameData
		{
			PlayerName = "John",
			Level = 5,
			Score = 32343,
			Difficulty = Difficulty.Easy
		});
		_data.Add(new SaveGameData
		{
			PlayerName = "David",
			Level = 6,
			Score = 54123,
			Difficulty = Difficulty.Normal
		});
		_data.Add(new SaveGameData
		{
			PlayerName = "Nestor",
			Level = 6,
			Score = 75412,
			Difficulty = Difficulty.Hard
		});
		_data.Add(new SaveGameData
		{
			PlayerName = "Graham",
			Level = 9,
			Score = 254120,
			Difficulty = Difficulty.Hard
		});
		_data.Add(new SaveGameData
		{
			PlayerName = "Julia",
			Level = 5,
			Score = 25485,
			Difficulty = Difficulty.Easy
		});
		_data.Add(new SaveGameData
		{
			PlayerName = "David",
			Level = 7,
			Score = 62531,
			Difficulty = Difficulty.Normal
		});
		_data.Add(new SaveGameData
		{
			PlayerName = "Billy",
			Level = 5,
			Score = 25641,
			Difficulty = Difficulty.Easy
		});
		_data.Add(new SaveGameData
		{
			PlayerName = "Fred",
			Level = 9,
			Score = 135210,
			Difficulty = Difficulty.Hard
		});
		_data.Add(new SaveGameData
		{
			PlayerName = "Roger",
			Level = 9,
			Score = 74123,
			Difficulty = Difficulty.Normal
		});
		_data.Add(new SaveGameData
		{
			PlayerName = "Tommy",
			Level = 9,
			Score = 135421,
			Difficulty = Difficulty.Hard
		});
	}

	private void RemoveAllEnemies()
	{
		_enemies.Clear();
		_enemies2.Clear();
		_enemies3.Clear();
		_enemies4.Clear();
		_enemies5.Clear();
		_enemies6.Clear();
		_enemies7.Clear();
		_miniBossList.Clear();
		_finalBossList.Clear();
	}

	private void CheckForItemQualify()
	{
		switch (_player.Inventory)
		{
		case 0:
			if (_powerupItems >= _difficulty && _player.Speed < 4)
			{
				_itemQualify = true;
			}
			else
			{
				_itemQualify = false;
			}
			_powerupItemPrice = _difficulty;
			break;
		case 1:
			if (_powerupItems >= 2 * _difficulty && _player.Projectiles.Count < 6)
			{
				_itemQualify = true;
			}
			else
			{
				_itemQualify = false;
			}
			_powerupItemPrice = 2 * _difficulty;
			break;
		case 2:
			if (_powerupItems >= 3 * _difficulty)
			{
				bool flag = false;
				foreach (Enemy enemy in _enemies)
				{
					if (!enemy.Dead)
					{
						flag = true;
					}
				}
				foreach (Enemy6 item in _enemies6)
				{
					if (!item.Dead)
					{
						flag = true;
					}
				}
				foreach (Enemy2 item2 in _enemies2)
				{
					if (!item2.Dead)
					{
						flag = true;
					}
				}
				foreach (Enemy3 item3 in _enemies3)
				{
					if (!item3.Dead)
					{
						flag = true;
					}
				}
				foreach (Enemy4 item4 in _enemies4)
				{
					if (!item4.Dead)
					{
						flag = true;
					}
				}
				foreach (Enemy7 item5 in _enemies7)
				{
					if (!item5.Dead)
					{
						flag = true;
					}
				}
				foreach (Enemy5 item6 in _enemies5)
				{
					if (!item6.Dead)
					{
						flag = true;
					}
				}
				foreach (Miniboss miniBoss in _miniBossList)
				{
					if (!miniBoss.Dead)
					{
						flag = true;
					}
				}
				foreach (Finalboss finalBoss in _finalBossList)
				{
					if (!finalBoss.Dead)
					{
						flag = true;
					}
				}
				foreach (Bird bird in _birds)
				{
					if (!bird.Dead)
					{
						flag = true;
					}
				}
				if (flag)
				{
					_itemQualify = true;
				}
				else
				{
					_itemQualify = false;
				}
			}
			else
			{
				_itemQualify = false;
			}
			_powerupItemPrice = 3 * _difficulty;
			break;
		case 3:
			if (_powerupItems >= 4 * _difficulty && !_player.Invincibility)
			{
				_itemQualify = true;
			}
			else
			{
				_itemQualify = false;
			}
			_powerupItemPrice = 4 * _difficulty;
			break;
		case 4:
			if (_powerupItems >= 5 * _difficulty && _player.Dead)
			{
				_itemQualify = true;
			}
			else
			{
				_itemQualify = false;
			}
			_powerupItemPrice = 5 * _difficulty;
			break;
		case 5:
			if (_powerupItems >= 5 * _difficulty && !_player.Dead && _player.Life < 10)
			{
				_itemQualify = true;
			}
			else
			{
				_itemQualify = false;
			}
			_powerupItemPrice = 5 * _difficulty;
			break;
		case 6:
			if (_powerupItems >= 6 * _difficulty && !_player.Dead && _player.Assist == 0)
			{
				_itemQualify = true;
			}
			else
			{
				_itemQualify = false;
			}
			_powerupItemPrice = 6 * _difficulty;
			break;
		}
		if (_player.Dead && _player.Inventory != 4)
		{
			_itemQualify = false;
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.Black);
		_spriteBatch.Begin();
		_spriteBatch.Draw(_backgroundSplit, new Rectangle(_xScroll, 0, _backgroundSplit.Width, _backgroundSplit.Height), Color.White);
		_spriteBatch.Draw(_backgroundSplit, new Vector2(_backgroundSplit.Width + _xScroll, 0f), new Rectangle(0, 0, _spaceLeftToDrawOnScreen - _xScroll, _backgroundSplit.Height), Color.White);
		if (_enableCustomMessage && _customMessageTime <= 3000.0)
		{
			if (_customMessageTime % 1000.0 > 500.0)
			{
				_showCustomMessage = true;
			}
			else
			{
				_showCustomMessage = false;
			}
			if (_showCustomMessage)
			{
				Vector2 vector = _spriteFont.MeasureString(_customMessage);
				_spriteBatch.DrawString(_spriteFont, _customMessage, new Vector2((float)(base.GraphicsDevice.Viewport.Width / 2) - vector.X / 2f, (float)(base.GraphicsDevice.Viewport.Height / 2) - vector.Y / 2f), Color.Yellow);
			}
			_customMessageTime += gameTime.ElapsedGameTime.TotalMilliseconds;
			if (_customMessageTime >= 3000.0)
			{
				_enableCustomMessage = false;
				_customMessageTime = 0.0;
			}
		}
		else if (_splashScreen)
		{
			float num = (float)(_pressStartButtonTimeFade / 1000.0);
			if (_pressStartButtonShow)
			{
				int num2 = _pressStartButton.Width / 2;
				int num3 = _pressStartButton.Height / 2;
				_spriteBatch.Draw(_pressStartButton, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - num2, base.GraphicsDevice.Viewport.Height / 2 - num3), Color.White * num);
			}
			_pressStartButtonTimeFade += gameTime.ElapsedGameTime.TotalMilliseconds;
		}
		else if (_highScoreScreen)
		{
			_highScoreTimeFade += gameTime.ElapsedGameTime.TotalMilliseconds;
			float num = (float)(_highScoreTimeFade / 1000.0);
			_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "Top High Scores"), new Vector2(_screenSize.Right / 2 - 70, _screenSize.Top + 10), Color.Yellow * num);
			IOrderedEnumerable<SaveGameData> source = _data.OrderByDescending((SaveGameData item) => item.Score);
			SaveGameData[] array = source.Take(10).ToArray();
			for (int num4 = 0; num4 < array.Length; num4++)
			{
				SaveGameData saveGameData = array[num4];
				string text = ((num4 >= 9) ? (num4 + 1 + ". " + saveGameData.PlayerName) : (num4 + 1 + ".  " + saveGameData.PlayerName));
				_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "{0} (Lvl {1}, {2}) - {3:n0} Pts", text, (saveGameData.Level >= 9) ? "8 Complete" : saveGameData.Level.ToString(), saveGameData.Difficulty, saveGameData.Score), new Vector2(100f, 85f + (float)num4 * 30f), Color.White * num);
			}
		}
		else
		{
			if (_help)
			{
				float num = (float)(_helpTimeFade / 1000.0);
				_helpTimeFade += gameTime.ElapsedGameTime.TotalMilliseconds;
				_spriteBatch.Draw(_xboxControllerConfig, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2 - 175, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 160), Color.White * num);
			}
			if (_options)
			{
				float num = (float)(_helpTimeFade / 1000.0);
				_helpTimeFade += gameTime.ElapsedGameTime.TotalMilliseconds;
				if (_optionSelect == 0)
				{
					_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "Music:   {0:p0}", new object[1] { _musicVolume }), new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 50), Color.Yellow * num);
				}
				else
				{
					_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "Music:   {0:p0}", new object[1] { _musicVolume }), new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 50), Color.White * num);
				}
				if (_optionSelect == 1)
				{
					_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "SFX:   {0:p0}", new object[1] { _soundEffectVolume }), new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 15), Color.Yellow * num);
				}
				else
				{
					_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "SFX:   {0:p0}", new object[1] { _soundEffectVolume }), new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 15), Color.White * num);
				}
				if (_optionSelect == 2)
				{
					_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "Back"), new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 + 20), Color.Yellow * num);
				}
				else
				{
					_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "Back"), new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 + 20), Color.White * num);
				}
			}
			if (_title && !_help && !_options)
			{
				float num = (float)(_titleTimeFade / 2000.0);
				_titleTimeFade += gameTime.ElapsedGameTime.TotalMilliseconds;
				if (Guide.IsTrialMode)
				{
					_spriteBatch.Draw(_jetStarUniverseTrial, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2 - 120, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 210), Color.White * num);
				}
				else
				{
					_spriteBatch.Draw(_jetStarUniverse, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2 - 120, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 210), Color.White * num);
				}
				if (!_paused)
				{
					_spriteBatch.Draw(_startTitle, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 70), Color.White * num);
				}
				else
				{
					_spriteBatch.Draw(_continueTitle, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 70), Color.White * num);
				}
				Color white = Color.White;
				if (Guide.IsTrialMode)
				{
					_spriteBatch.Draw(_unlockTitle, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 10), Color.White * num);
				}
				else
				{
					_spriteBatch.Draw(_unlockTitle, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 10), Color.White * num * 0.25f);
				}
				_spriteBatch.Draw(_optionsTitle, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 + 50), Color.White * num);
				_spriteBatch.Draw(_helpTitle, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 + 110), Color.White * num);
				_spriteBatch.Draw(_exitTitle, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 + 165), Color.White * num);
				Color color = (((GetTotalGameTime(gameTime) - _fadeSelectTime) % 150.0 >= 75.0) ? Color.White : Color.Black);
				switch (_titleSelect)
				{
				case 0:
					_spriteBatch.Draw(_player.Texture2D, new Rectangle(240, 155, _player.SourceRectangles[0].Width, _player.SourceRectangles[0].Height), _player.SourceRectangles[0], color);
					break;
				case 1:
					_spriteBatch.Draw(_player.Texture2D, new Rectangle(240, 220, _player.SourceRectangles[0].Width, _player.SourceRectangles[0].Height), _player.SourceRectangles[0], color);
					break;
				case 2:
					_spriteBatch.Draw(_player.Texture2D, new Rectangle(240, 275, _player.SourceRectangles[0].Width, _player.SourceRectangles[0].Height), _player.SourceRectangles[0], color);
					break;
				case 3:
					_spriteBatch.Draw(_player.Texture2D, new Rectangle(240, 330, _player.SourceRectangles[0].Width, _player.SourceRectangles[0].Height), _player.SourceRectangles[0], color);
					break;
				case 4:
					_spriteBatch.Draw(_player.Texture2D, new Rectangle(240, 390, _player.SourceRectangles[0].Width, _player.SourceRectangles[0].Height), _player.SourceRectangles[0], color);
					break;
				}
			}
			if (_difficultySelect)
			{
				float num = (float)(_difficultyTimeFade / 1000.0);
				_difficultyTimeFade += gameTime.ElapsedGameTime.TotalMilliseconds;
				if (Guide.IsTrialMode)
				{
					_spriteBatch.Draw(_jetStarUniverseTrial, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2 - 120, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 210), Color.White * num);
				}
				else
				{
					_spriteBatch.Draw(_jetStarUniverse, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2 - 120, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 210), Color.White * num);
				}
				_spriteBatch.Draw(_easy, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2 - 35, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 75), Color.White * num);
				_spriteBatch.Draw(_normal, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2 - 35, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 + 35), Color.White * num);
				_spriteBatch.Draw(_hard, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2 - 35, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 + 135), Color.White * num);
				Color color = (((GetTotalGameTime(gameTime) - _fadeSelectTime) % 150.0 >= 75.0) ? Color.White : Color.Black);
				switch (_difficulty)
				{
				case 1:
					_spriteBatch.Draw(_player.Texture2D, new Rectangle(180, 170, _player.SourceRectangles[0].Width, _player.SourceRectangles[0].Height), _player.SourceRectangles[0], color);
					break;
				case 2:
					_spriteBatch.Draw(_player.Texture2D, new Rectangle(180, 270, _player.SourceRectangles[0].Width, _player.SourceRectangles[0].Height), _player.SourceRectangles[0], color);
					break;
				case 3:
					_spriteBatch.Draw(_player.Texture2D, new Rectangle(180, 370, _player.SourceRectangles[0].Width, _player.SourceRectangles[0].Height), _player.SourceRectangles[0], color);
					break;
				}
			}
			if (_gameStarted && !_title)
			{
				if (_gameComplete)
				{
					float num = (float)(_gameCompleteFadeTime / 2000.0);
					_spriteBatch.Draw(_congratulations, new Vector2(_screenSize.Left + 10, _screenSize.Top + 10), Color.White * num);
					if (num >= 1f && _pressStartToBeginShowTime >= 250.0)
					{
						_showPressStartToBegin = !_showPressStartToBegin;
						_pressStartToBeginShowTime = 0.0;
					}
					if (_showPressStartToBegin)
					{
						int num2 = _pressStartToBegin.Width / 2;
						int num3 = _pressStartToBegin.Height / 2;
						_spriteBatch.Draw(_pressStartToBegin, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - num2, base.GraphicsDevice.Viewport.Height / 2 - num3), Color.White * num);
					}
					_pressStartToBeginShowTime += gameTime.ElapsedGameTime.TotalMilliseconds;
					_gameCompleteFadeTime += gameTime.ElapsedGameTime.TotalMilliseconds;
				}
				else
				{
					if (_currentLevel >= 2)
					{
						double num5 = 0.0;
						if (_currentLevel == 2 && !_stageCleared)
						{
							num5 = (GetTotalGameTime(gameTime) - _layer1StartFadeTime) / 1000.0;
						}
						else if (_currentLevel > 2)
						{
							num5 = 1.0;
						}
					}
					if (_startUpShow)
					{
						Texture2D texture = _currentLevel switch
						{
							1 => _level1, 
							2 => _level2, 
							3 => _level3, 
							4 => _level4, 
							6 => _level5, 
							7 => _level6, 
							8 => _level7, 
							9 => _level8, 
							_ => _noLevel, 
						};
						if (_startUpTime <= 15000.0)
						{
							if (_fadeOut)
							{
								if (_startUpTime >= 5000.0)
								{
									_fadeIncrement -= 0.01f;
								}
								if (_stageCleared)
								{
									_spriteBatch.Draw(_stageClear, new Vector2(200f, 100f), Color.White * _fadeIncrement);
								}
								else
								{
									_spriteBatch.Draw(texture, new Vector2(150f, 100f), Color.White * _fadeIncrement);
								}
							}
							else
							{
								_fadeIncrement += 0.01f;
								if (_fadeIncrement >= 1f)
								{
									_fadeOut = true;
								}
								if (_stageCleared)
								{
									_spriteBatch.Draw(_stageClear, new Vector2(200f, 100f), Color.White * _fadeIncrement);
								}
								else
								{
									_spriteBatch.Draw(texture, new Vector2(150f, 100f), Color.White * _fadeIncrement);
								}
							}
						}
					}
					if (!_powerupItem.Hidden)
					{
						_spriteBatch.Draw(_powerupItem.Texture2D, _powerupItem.Position, _powerupItem.SourceRectangles[_powerupItem.NextFrameIndex], Color.White);
					}
					foreach (Projectile projectile in _player.Projectiles)
					{
						if (projectile.Show)
						{
							_spriteBatch.Draw(_player.Texture2D, projectile.Position, _player.SourceProjectile, Color.DeepSkyBlue);
						}
					}
					foreach (Projectile assistProjectile in _player.AssistProjectiles)
					{
						if (assistProjectile.Show)
						{
							_spriteBatch.Draw(_player.Texture2D, assistProjectile.Position, _player.SourceProjectile, Color.White);
						}
					}
					for (float num6 = 1f; num6 <= (float)_player.Assist; num6++)
					{
						Vector2 position = new Vector2(_player.Position.X + 10f, _player.Position.Y - 20f * num6);
						_spriteBatch.Draw(_player.Texture2D, position, _player.AssistSource, Color.White);
					}
					foreach (Enemy enemy in _enemies)
					{
						foreach (Projectile projectile2 in enemy.Projectiles)
						{
							if (projectile2.Show)
							{
								_spriteBatch.Draw(enemy.Texture2D, projectile2.Position, enemy.SourceProjectile, Color.White);
							}
						}
					}
					foreach (Enemy6 item in _enemies6)
					{
						foreach (Projectile projectile3 in item.Projectiles)
						{
							if (projectile3.Show)
							{
								_spriteBatch.Draw(item.Texture2D, projectile3.Position, item.SourceProjectile, Color.White);
							}
						}
					}
					foreach (Enemy3 item2 in _enemies3)
					{
						foreach (Projectile projectile4 in item2.Projectiles)
						{
							if (projectile4.Show)
							{
								_spriteBatch.Draw(item2.Texture2D, projectile4.Position, item2.SourceProjectile, Color.White);
							}
						}
					}
					foreach (Enemy4 item3 in _enemies4)
					{
						foreach (Projectile projectile5 in item3.Projectiles)
						{
							if (projectile5.Show)
							{
								_spriteBatch.Draw(item3.Texture2D, projectile5.Position, item3.SourceProjectile, Color.White);
							}
						}
					}
					foreach (Enemy7 item4 in _enemies7)
					{
						foreach (Projectile projectile6 in item4.Projectiles)
						{
							if (projectile6.Show)
							{
								_spriteBatch.Draw(item4.Texture2D, projectile6.Position, item4.SourceProjectile, Color.White);
							}
						}
					}
					foreach (Enemy5 item5 in _enemies5)
					{
						foreach (Projectile projectile7 in item5.Projectiles)
						{
							if (projectile7.Show)
							{
								_spriteBatch.Draw(item5.Texture2D, projectile7.Position, item5.SourceProjectile, Color.White);
							}
						}
					}
					foreach (Miniboss miniBoss in _miniBossList)
					{
						foreach (Projectile projectile8 in miniBoss.Projectiles)
						{
							if (projectile8.Show)
							{
								_spriteBatch.Draw(miniBoss.Texture2D, projectile8.Position, miniBoss.SourceProjectile, Color.White);
							}
						}
					}
					foreach (Finalboss finalBoss in _finalBossList)
					{
						foreach (Projectile projectile9 in finalBoss.Projectiles)
						{
							if (projectile9.Show)
							{
								Random random = new Random((int)DateTime.Now.Ticks);
								Color color2 = new Color(random.Next(125, 255), random.Next(125, 255), random.Next(125, 255));
								_spriteBatch.Draw(finalBoss.Texture2D, projectile9.Position, finalBoss.SourceProjectile, color2);
							}
						}
					}
					foreach (Bird bird in _birds)
					{
						if (!bird.Dead)
						{
							_spriteBatch.Draw(bird.Texture2D, bird.Position, bird.SourceRectangles[bird.NextFrameIndex], Color.White);
						}
						else if (bird.ShowExplosion)
						{
							_spriteBatch.Draw(bird.TextureOfExplosion, bird.PositionOfExplosion, bird.SourceOfExplosion[bird.NextFrameIndexOfExplosion], Color.White);
						}
					}
					foreach (Enemy enemy2 in _enemies)
					{
						if (!enemy2.Dead)
						{
							if (enemy2.Hit)
							{
								_spriteBatch.Draw(enemy2.Texture2D, enemy2.Position, enemy2.SourceRectangles[enemy2.NextFrameIndex], enemy2.TintColor);
							}
							else
							{
								_spriteBatch.Draw(enemy2.Texture2D, enemy2.Position, enemy2.SourceRectangles[enemy2.NextFrameIndex], Color.White);
							}
						}
						else if (enemy2.ShowExplosion)
						{
							_spriteBatch.Draw(enemy2.TextureOfExplosion, enemy2.PositionOfExplosion, enemy2.SourceOfExplosion[enemy2.NextFrameIndexOfExplosion], Color.White);
						}
					}
					foreach (Enemy6 item6 in _enemies6)
					{
						if (!item6.Dead)
						{
							if (item6.Hit)
							{
								_spriteBatch.Draw(item6.Texture2D, item6.Position, item6.SourceRectangles[item6.NextFrameIndex], item6.TintColor);
							}
							else
							{
								_spriteBatch.Draw(item6.Texture2D, item6.Position, item6.SourceRectangles[item6.NextFrameIndex], Color.White);
							}
						}
						else if (item6.ShowExplosion)
						{
							_spriteBatch.Draw(item6.TextureOfExplosion, item6.PositionOfExplosion, item6.SourceOfExplosion[item6.NextFrameIndexOfExplosion], Color.White);
						}
					}
					foreach (Enemy2 item7 in _enemies2)
					{
						if (!item7.Dead)
						{
							if (item7.Hit)
							{
								_spriteBatch.Draw(item7.Texture2D, item7.Position, item7.SourceRectangles[item7.NextFrameIndex], item7.TintColor);
							}
							else
							{
								_spriteBatch.Draw(item7.Texture2D, item7.Position, item7.SourceRectangles[item7.NextFrameIndex], Color.White);
							}
						}
						else if (item7.ShowExplosion)
						{
							_spriteBatch.Draw(item7.TextureOfExplosion, item7.PositionOfExplosion, item7.SourceOfExplosion[item7.NextFrameIndexOfExplosion], Color.White);
						}
					}
					foreach (Enemy3 item8 in _enemies3)
					{
						if (!item8.Dead)
						{
							if (item8.Hit)
							{
								_spriteBatch.Draw(item8.Texture2D, item8.Position, item8.SourceRectangles[item8.NextFrameIndex], item8.TintColor);
							}
							else
							{
								_spriteBatch.Draw(item8.Texture2D, item8.Position, item8.SourceRectangles[item8.NextFrameIndex], Color.White);
							}
						}
						else if (item8.ShowExplosion)
						{
							_spriteBatch.Draw(item8.TextureOfExplosion, item8.PositionOfExplosion, item8.SourceOfExplosion[item8.NextFrameIndexOfExplosion], Color.White);
						}
					}
					foreach (Enemy4 item9 in _enemies4)
					{
						if (!item9.Dead)
						{
							if (item9.Hit)
							{
								_spriteBatch.Draw(item9.Texture2D, item9.Position, item9.SourceRectangles[item9.NextFrameIndex], item9.TintColor);
							}
							else
							{
								_spriteBatch.Draw(item9.Texture2D, item9.Position, item9.SourceRectangles[item9.NextFrameIndex], Color.White);
							}
						}
						else if (item9.ShowExplosion)
						{
							_spriteBatch.Draw(item9.TextureOfExplosion, item9.PositionOfExplosion, item9.SourceOfExplosion[item9.NextFrameIndexOfExplosion], Color.White);
						}
					}
					foreach (Enemy7 item10 in _enemies7)
					{
						if (!item10.Dead)
						{
							if (item10.Hit)
							{
								_spriteBatch.Draw(item10.Texture2D, item10.Position, item10.SourceRectangles[item10.NextFrameIndex], item10.TintColor);
							}
							else
							{
								_spriteBatch.Draw(item10.Texture2D, item10.Position, item10.SourceRectangles[item10.NextFrameIndex], Color.White);
							}
						}
						else if (item10.ShowExplosion)
						{
							_spriteBatch.Draw(item10.TextureOfExplosion, item10.PositionOfExplosion, item10.SourceOfExplosion[item10.NextFrameIndexOfExplosion], Color.White);
						}
					}
					foreach (Enemy5 item11 in _enemies5)
					{
						if (!item11.Dead)
						{
							if (item11.Hit)
							{
								_spriteBatch.Draw(item11.Texture2D, item11.Position, item11.SourceRectangles[item11.NextFrameIndex], item11.TintColor);
							}
							else
							{
								_spriteBatch.Draw(item11.Texture2D, item11.Position, item11.SourceRectangles[item11.NextFrameIndex], Color.White);
							}
						}
						else if (item11.ShowExplosion)
						{
							_spriteBatch.Draw(item11.TextureOfExplosion, item11.PositionOfExplosion, item11.SourceOfExplosion[item11.NextFrameIndexOfExplosion], Color.White);
						}
					}
					foreach (Miniboss miniBoss2 in _miniBossList)
					{
						if (!miniBoss2.Dead)
						{
							if (miniBoss2.Hit)
							{
								_spriteBatch.Draw(miniBoss2.Texture2D, miniBoss2.Position, miniBoss2.SourceRectangles[miniBoss2.NextFrameIndex], miniBoss2.TintColor);
							}
							else
							{
								_spriteBatch.Draw(miniBoss2.Texture2D, miniBoss2.Position, miniBoss2.SourceRectangles[miniBoss2.NextFrameIndex], Color.White);
							}
						}
						else if (miniBoss2.ShowExplosion)
						{
							_spriteBatch.Draw(miniBoss2.TextureOfExplosion, miniBoss2.PositionOfExplosion, miniBoss2.SourceOfExplosion[miniBoss2.NextFrameIndexOfExplosion], Color.White);
						}
					}
					foreach (Finalboss finalBoss2 in _finalBossList)
					{
						if (!finalBoss2.Dead)
						{
							if (finalBoss2.Hit)
							{
								_spriteBatch.Draw(finalBoss2.Texture2D, finalBoss2.Position, finalBoss2.SourceRectangles[finalBoss2.NextFrameIndex], finalBoss2.TintColor);
							}
							else
							{
								_spriteBatch.Draw(finalBoss2.Texture2D, finalBoss2.Position, finalBoss2.SourceRectangles[finalBoss2.NextFrameIndex], Color.White);
							}
						}
						else if (finalBoss2.ShowExplosion)
						{
							_spriteBatch.Draw(finalBoss2.TextureOfExplosion, finalBoss2.PositionOfExplosion, finalBoss2.SourceOfExplosion[finalBoss2.NextFrameIndexOfExplosion], Color.White);
						}
					}
					if (!_player.Dead)
					{
						if (_player.Invincibility)
						{
							_spriteBatch.Draw(_player.Texture2D, _player.Position, _player.SourceRectangles[_player.NextFrameIndex], _player.InvisibilityColor);
						}
						else if (_player.Hit)
						{
							_spriteBatch.Draw(_player.Texture2D, _player.Position, _player.SourceRectangles[_player.NextFrameIndex], _player.TintColor);
						}
						else
						{
							_spriteBatch.Draw(_player.Texture2D, _player.Position, _player.SourceRectangles[_player.NextFrameIndex], Color.White);
						}
						Vector2 position = _player.Position;
						position.X += 5f;
						position.Y += 35f;
						int num7 = (int)_bullets / 100 + 1;
						for (int num4 = 0; num4 < num7; num4++)
						{
							_spriteBatch.Draw(_bulletBlock, position, new Rectangle(0, 0, 8, 8), Color.White);
							position.X += 8f;
						}
					}
					else if (_player.ShowExplosion)
					{
						_spriteBatch.Draw(_player.TextureOfExplosion, _player.PositionOfExplosion, _player.SourceOfExplosion[_player.NextFrameIndexOfExplosion], Color.White);
					}
					Color azure = Color.Azure;
					_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "Power"), new Vector2(_screenSize.Left + 10, _screenSize.Top + 10), azure);
					for (int num4 = 0; num4 < _player.Power; num4++)
					{
						_spriteBatch.Draw(_player.Texture2D, new Vector2(_screenSize.Left + 80 + num4 * 20, _screenSize.Top + 22), _player.SourceProjectile, Color.DeepSkyBlue);
					}
					Color white = ((_player.TintColor == Color.White) ? azure : _player.TintColor);
					_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "Life"), new Vector2(_screenSize.Left + 220, _screenSize.Top + 10), white);
					for (int num4 = 0; num4 < _player.Life; num4++)
					{
						if (_player.Life <= 3)
						{
							_spriteBatch.Draw(_lifeBlock, new Vector2(_screenSize.Left + 295 + num4 * 16, _screenSize.Top + 15), new Rectangle(0, 0, 16, 16), Color.Red);
						}
						else if (_player.Life <= 6)
						{
							_spriteBatch.Draw(_lifeBlock, new Vector2(_screenSize.Left + 295 + num4 * 16, _screenSize.Top + 15), new Rectangle(0, 0, 16, 16), Color.YellowGreen);
						}
						else
						{
							_spriteBatch.Draw(_lifeBlock, new Vector2(_screenSize.Left + 295 + num4 * 16, _screenSize.Top + 15), new Rectangle(0, 0, 16, 16), Color.Green);
						}
					}
					_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "{0:n0} Pts", new object[1] { _score }), new Vector2(_screenSize.Left + 495, _screenSize.Top + 10), azure);
					Texture2D texture2D = new Texture2D(base.GraphicsDevice, 1, 1);
					texture2D.SetData(new Color[1] { Color.White });
					Rectangle rectangle = _sourceBordersForInventory[_player.Inventory];
					int num8 = 2;
					_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "Speed  Power  Crash  Invincibility  Revive  Life  Assist"), new Vector2(100f, _screenSize.Bottom - 30), Color.White);
					if (_selectionBorderShow)
					{
						if (_itemQualify)
						{
							_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Left, rectangle.Top, num8, rectangle.Height), Color.Blue);
							_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Right, rectangle.Top, num8, rectangle.Height), Color.Blue);
							_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, num8), Color.Blue);
							_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, num8), Color.Blue);
						}
						else
						{
							_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Left, rectangle.Top, num8, rectangle.Height), Color.Gray);
							_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Right, rectangle.Top, num8, rectangle.Height), Color.Gray);
							_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, num8), Color.Gray);
							_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, num8), Color.Gray);
						}
						_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[2] { _powerupItems, _powerupItemPrice }), new Vector2(rectangle.Left + 5, rectangle.Top - 25), Color.Yellow);
					}
					if (_player.Dead)
					{
						int num9 = (int)((GetTotalGameTime(gameTime) - _countDownTime) / 1000.0).Value;
						_spriteBatch.DrawString(_spriteFont, string.Format(CultureInfo.InvariantCulture, "Press B to Revive. Continue? {0}", new object[1] { 10 - num9 }), new Vector2(base.GraphicsDevice.Viewport.Width / 2 - 200, base.GraphicsDevice.Viewport.Height / 2 - 100), Color.Yellow);
					}
				}
			}
		}
		_spriteBatch.End();
		base.Draw(gameTime);
	}
}
