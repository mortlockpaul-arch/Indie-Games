#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Xbox360Game1.Sprites;

namespace Xbox360Game1;

public class Game1 : Game
{
	private const double STAGE_CLEAR_TIME = 10000.0;

	private const double SPAWN_TIME = 10000.0;

	private const double INVINCIBILITY_TIME = 20000.0;

	private GraphicsDeviceManager _graphics;

	private SpriteBatch _spriteBatch;

	private SpriteFont _spriteFont;

	private Texture2D _backgroundSplit;

	private Texture2D _layer1Split;

	private int _spaceLeftToDrawOnScreen;

	private int _layer1LeftToDrawOnScreen;

	private int _xScroll;

	private int _layer1Scroll = 800;

	private double _layer1StartFadeTime;

	private Player _player;

	private SoundEffect _shoot;

	private Song _crystalis;

	private PowerupItem _powerupItem;

	private SoundEffect _pickup;

	private List<Bird> _birds;

	private SoundEffect _hit;

	private SoundEffect _playerHit;

	private Texture2D _lifeBlock;

	private SoundEffect _playerExplosion;

	private float _margins = 25f;

	private List<Enemy> _enemies = new List<Enemy>();

	private List<Enemy2> _enemies2 = new List<Enemy2>();

	private List<Enemy3> _enemies3 = new List<Enemy3>();

	private List<Enemy4> _enemies4 = new List<Enemy4>();

	private List<Enemy5> _enemies5 = new List<Enemy5>();

	private double _spawnTime;

	private List<Rectangle> _sourceBordersForInventory = new List<Rectangle>();

	private int _powerupItems = 3;

	private bool _selectionBorderShow = true;

	private DateTime _selectionBorderFlashTime = DateTime.Now;

	private DateTime _selectionInventoryTime = DateTime.Now;

	private SoundEffect _select;

	private SoundEffect _select2;

	private bool _itemQualify = true;

	private int _powerupItemPrice = 0;

	private SoundEffect _powerup;

	private Song _ballad;

	private int _score = 0;

	private DateTime _startUpTime = DateTime.Now;

	private Texture2D _level1;

	private bool _startUpShow = true;

	private bool _fadeOut = false;

	private float _fadeIncrement = 0f;

	private DateTime _powerupTime = DateTime.Now;

	private List<DateTime> _levelTime = new List<DateTime>();

	private int _currentLevel = 1;

	private Song _levelFinish;

	private Texture2D _level2;

	private Texture2D _level3;

	private Texture2D _level4;

	private Texture2D _stageClear;

	private bool _stageCleared = false;

	private DateTime _stageClearTime;

	private Song _angryRobot;

	private Song _currentSong;

	private Song _azimuth;

	private Song _blueChill;

	private Song _crisson;

	private bool _gameStarted = false;

	private bool _title = true;

	private bool _paused = false;

	private bool _help = false;

	private double LEVEL_TIME = 120000.0;

	private Texture2D _startTitle;

	private Texture2D _optionsTitle;

	private Texture2D _exitTitle;

	private Texture2D _continueTitle;

	private double? _titleStartFadeTime = null;

	private int _titleSelect = 0;

	private double? _fadeSelectTime = null;

	private double? _titleSelectPress = null;

	private double? _countDownTime = null;

	private int _levelFinishCount = 0;

	private Texture2D _jetStarUniverse;

	private Texture2D _xboxControllerConfig;

	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		base.Initialize();
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(base.GraphicsDevice);
		_spriteFont = base.Content.Load<SpriteFont>("MyFont");
		_backgroundSplit = base.Content.Load<Texture2D>("space_stars");
		_layer1Split = base.Content.Load<Texture2D>("layer1_background");
		_player = new Player(56, 28);
		_player.Texture2D = base.Content.Load<Texture2D>("gradius_ship");
		_player.Position = new Vector2(30f, 30f);
		_player.SourceRectangles.Add(new Rectangle(0, 0, _player.Width, _player.Height));
		_player.SourceRectangles.Add(new Rectangle(66, 0, _player.Width, _player.Height));
		_player.SourceRectangles.Add(new Rectangle(130, 0, _player.Width, _player.Height));
		_player.SourceProjectile = new Rectangle(215, 10, 10, 6);
		_shoot = base.Content.Load<SoundEffect>("shoot2");
		_crystalis = base.Content.Load<Song>("DST-Crysalis");
		_crisson = base.Content.Load<Song>("DST-Crisson");
		MediaPlayer.Play(_crisson);
		MediaPlayer.Volume = 1f;
		MediaPlayer.IsRepeating = true;
		_powerupItem = new PowerupItem(30, 24);
		_powerupItem.Texture2D = base.Content.Load<Texture2D>("powerup_item");
		_powerupItem.Position = _powerupItem.RandomLocation((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f, (float)base.GraphicsDevice.Viewport.Height - (float)base.GraphicsDevice.Viewport.Height * 0.03f);
		_powerupItem.SourceRectangles.Add(new Rectangle(0, 0, _powerupItem.Width, _powerupItem.Height));
		_powerupItem.SourceRectangles.Add(new Rectangle(30, 0, _powerupItem.Width, _powerupItem.Height));
		_powerupItem.SourceRectangles.Add(new Rectangle(60, 0, _powerupItem.Width, _powerupItem.Height));
		_powerupItem.Hidden = true;
		_pickup = base.Content.Load<SoundEffect>("pickup1");
		Bird bird = new Bird(25, 21);
		bird.Texture2D = base.Content.Load<Texture2D>("bird");
		bird.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 200f, base.GraphicsDevice.Viewport.Height / 2);
		bird.SourceRectangles.Add(new Rectangle(5, 2, bird.Width, bird.Height));
		bird.SourceRectangles.Add(new Rectangle(44, 2, 25, 33));
		bird.SourceRectangles.Add(new Rectangle(82, 5, 25, 19));
		bird.SourceRectangles.Add(new Rectangle(4, 45, 26, 20));
		bird.SourceRectangles.Add(new Rectangle(44, 45, 25, 21));
		bird.SourceRectangles.Add(new Rectangle(82, 45, 30, 18));
		_birds = new List<Bird>();
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
		Bird.ResetAllBirdPositions(_birds, base.GraphicsDevice);
		_hit = base.Content.Load<SoundEffect>("hit2");
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
		_playerHit = base.Content.Load<SoundEffect>("hit1");
		_lifeBlock = base.Content.Load<Texture2D>("block");
		_player.TextureOfExplosion = _birds.First().TextureOfExplosion;
		_player.SourceOfExplosion = _birds.First().SourceOfExplosion;
		_player.NextFrameIndexOfExplosion = 0;
		_playerExplosion = base.Content.Load<SoundEffect>("explosion5");
		foreach (Enemy enemy in _enemies)
		{
			SetTextureExplosionForEnemy(enemy);
		}
		foreach (Enemy2 item in _enemies2)
		{
			SetTextureExplosionForEnemy(item);
		}
		foreach (Enemy3 item2 in _enemies3)
		{
			SetTextureExplosionForEnemy(item2);
		}
		foreach (Enemy4 item3 in _enemies4)
		{
			SetTextureExplosionForEnemy(item3);
		}
		foreach (Enemy5 item4 in _enemies5)
		{
			SetTextureExplosionForEnemy(item4);
		}
		_sourceBordersForInventory.Add(new Rectangle(125, (int)((float)base.GraphicsDevice.Viewport.Height - (float)base.GraphicsDevice.Viewport.Height * 0.03f - 28f), 65, 25));
		_sourceBordersForInventory.Add(new Rectangle(200, (int)((float)base.GraphicsDevice.Viewport.Height - (float)base.GraphicsDevice.Viewport.Height * 0.03f - 28f), 65, 25));
		_sourceBordersForInventory.Add(new Rectangle(277, (int)((float)base.GraphicsDevice.Viewport.Height - (float)base.GraphicsDevice.Viewport.Height * 0.03f - 28f), 70, 25));
		_sourceBordersForInventory.Add(new Rectangle(358, (int)((float)base.GraphicsDevice.Viewport.Height - (float)base.GraphicsDevice.Viewport.Height * 0.03f - 28f), 150, 25));
		_sourceBordersForInventory.Add(new Rectangle(520, (int)((float)base.GraphicsDevice.Viewport.Height - (float)base.GraphicsDevice.Viewport.Height * 0.03f - 28f), 75, 25));
		_sourceBordersForInventory.Add(new Rectangle(605, (int)((float)base.GraphicsDevice.Viewport.Height - (float)base.GraphicsDevice.Viewport.Height * 0.03f - 28f), 70, 25));
		_select = base.Content.Load<SoundEffect>("select");
		_select2 = base.Content.Load<SoundEffect>("select2");
		_powerup = base.Content.Load<SoundEffect>("powerup");
		_ballad = base.Content.Load<Song>("DST-2ndBallad");
		_level1 = base.Content.Load<Texture2D>("level1");
		_levelTime.Add(DateTime.Now);
		_levelFinish = base.Content.Load<Song>("DST-AmbientKingdom");
		_level2 = base.Content.Load<Texture2D>("level2");
		_level3 = base.Content.Load<Texture2D>("level3");
		_level4 = base.Content.Load<Texture2D>("level4");
		_stageClear = base.Content.Load<Texture2D>("stageclear");
		_angryRobot = base.Content.Load<Song>("DST-AngryRobotIII");
		_azimuth = base.Content.Load<Song>("DST-Azimuth");
		_currentSong = _crystalis;
		_blueChill = base.Content.Load<Song>("DST-BlueChill");
		_startTitle = base.Content.Load<Texture2D>("start_title");
		_optionsTitle = base.Content.Load<Texture2D>("options_title");
		_exitTitle = base.Content.Load<Texture2D>("exit_title");
		_continueTitle = base.Content.Load<Texture2D>("continue_title");
		_jetStarUniverse = base.Content.Load<Texture2D>("jet_star_universe");
		_xboxControllerConfig = base.Content.Load<Texture2D>("xbox_controller_config");
	}

	private void AddNewEnemy()
	{
		Enemy enemy = new Enemy(32, 32);
		enemy.Texture2D = base.Content.Load<Texture2D>("space_enemy1");
		enemy.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, 140f);
		enemy.SourceRectangles.Add(new Rectangle(0, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(38, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(76, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(114, 0, enemy.Width, enemy.Height));
		enemy.SourceProjectile = new Rectangle(167, 12, 10, 6);
		enemy.Score = 38;
		_enemies.Add(enemy);
	}

	private void AddNewEnemy2()
	{
		Enemy2 enemy = new Enemy2(28, 29);
		enemy.Texture2D = base.Content.Load<Texture2D>("space_enemy2");
		enemy.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, 120f);
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
		enemy.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, 100f);
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
		enemy.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, 30f);
		enemy.SourceRectangles.Add(new Rectangle(0, 0, enemy.Width, enemy.Height));
		enemy.SourceProjectile = new Rectangle(38, 6, 20, 18);
		Random random = new Random((int)DateTime.Now.Ticks);
		enemy.Speed = random.Next(5, 15);
		enemy.Score = 125;
		_enemies4.Add(enemy);
	}

	private void AddNewEnemy5()
	{
		Enemy5 enemy = new Enemy5(56, 28);
		enemy.Texture2D = base.Content.Load<Texture2D>("space_enemy5");
		enemy.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, 30f);
		enemy.SourceRectangles.Add(new Rectangle(194, 0, enemy.Width, enemy.Height));
		enemy.SourceRectangles.Add(new Rectangle(128, 6, 56, 22));
		enemy.SourceRectangles.Add(new Rectangle(64, 0, enemy.Width, enemy.Height));
		enemy.SourceProjectile = new Rectangle(25, 10, 10, 6);
		Random random = new Random((int)DateTime.Now.Ticks);
		enemy.Score = 1000;
		_enemies5.Add(enemy);
	}

	private void SetTextureExplosionForEnemy(Enemy enemy)
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

	private void SetTextureExplosionForEnemy(Enemy5 enemy)
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
		if (!_titleStartFadeTime.HasValue)
		{
			_titleStartFadeTime = gameTime.TotalGameTime.TotalMilliseconds;
			_fadeSelectTime = _titleStartFadeTime;
			_titleSelectPress = _titleStartFadeTime;
		}
		_spaceLeftToDrawOnScreen = _graphics.PreferredBackBufferWidth - _backgroundSplit.Width;
		_xScroll = (int)(0.0 - gameTime.TotalGameTime.TotalMilliseconds / 8.0 % (double)_backgroundSplit.Width);
		if (_help)
		{
			double? num = gameTime.TotalGameTime.TotalMilliseconds - _titleSelectPress;
			if (num.GetValueOrDefault() >= 250.0 && num.HasValue && GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
			{
				_help = false;
				_titleSelectPress = gameTime.TotalGameTime.TotalMilliseconds;
			}
		}
		if (_title && !_help && gameTime.TotalGameTime.TotalMilliseconds - _titleSelectPress >= 250.0)
		{
			if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y <= -0.25f)
			{
				_titleSelectPress = gameTime.TotalGameTime.TotalMilliseconds;
				if (++_titleSelect > 2)
				{
					_titleSelect = 0;
				}
				_select.Play();
			}
			else if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y >= 0.25f)
			{
				_titleSelectPress = gameTime.TotalGameTime.TotalMilliseconds;
				if (--_titleSelect < 0)
				{
					_titleSelect = 2;
				}
				_select.Play();
			}
			else if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
			{
				_titleSelectPress = gameTime.TotalGameTime.TotalMilliseconds;
				switch (_titleSelect)
				{
				case 0:
					if (!_gameStarted)
					{
						MediaPlayer.Play(_crystalis);
					}
					_gameStarted = true;
					_title = false;
					_paused = false;
					_select2.Play();
					break;
				case 1:
					_help = true;
					break;
				case 2:
					Exit();
					break;
				}
			}
		}
		if (_gameStarted)
		{
			double? num = gameTime.TotalGameTime.TotalMilliseconds - _titleSelectPress;
			if (num.GetValueOrDefault() >= 250.0 && num.HasValue && GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
			{
				_titleSelectPress = gameTime.TotalGameTime.TotalMilliseconds;
				_title = true;
				_paused = true;
				_select2.Play();
			}
		}
		if (_gameStarted && !_title && !_paused)
		{
			float num2 = 5f;
			Debug.WriteLine(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X);
			if ((GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X <= -0.25f) && _player.Position.X > (float)base.GraphicsDevice.Viewport.Width * 0.03f)
			{
				_player.Position = new Vector2(_player.Position.X - num2 - (float)_player.Speed, _player.Position.Y);
			}
			if ((GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X >= 0.25f) && _player.Position.X < (float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - (float)_player.Width)
			{
				_player.Position = new Vector2(_player.Position.X + num2 + (float)_player.Speed, _player.Position.Y);
			}
			if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y <= -0.25f)
			{
				_player.NextFrameIndex = 2;
				if (_player.Position.Y < (float)base.GraphicsDevice.Viewport.Height - (float)base.GraphicsDevice.Viewport.Height * 0.03f - (float)_player.Height - 30f)
				{
					_player.Position = new Vector2(_player.Position.X, _player.Position.Y + num2 + (float)_player.Speed);
				}
			}
			if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y >= 0.25f)
			{
				_player.NextFrameIndex = 1;
				if (_player.Position.Y > (float)base.GraphicsDevice.Viewport.Height * 0.03f + _margins)
				{
					_player.Position = new Vector2(_player.Position.X, _player.Position.Y - num2 - (float)_player.Speed);
				}
			}
			if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Released && GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Released)
			{
				_player.NextFrameIndex = 0;
			}
			if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed && (DateTime.Now - _player.KeyPressTime).TotalMilliseconds >= 200.0 - (double)(_player.Power * 15))
			{
				foreach (Projectile projectile in _player.Projectiles)
				{
					if (!projectile.Show && !_player.Dead)
					{
						projectile.Show = true;
						projectile.Position = _player.CenterRight;
						_shoot.Play(0.25f, 0f, 0f);
						_player.KeyPressTime = DateTime.Now;
						break;
					}
				}
			}
			if (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed && _powerupItems > 0 && (DateTime.Now - _selectionInventoryTime).TotalMilliseconds >= 200.0)
			{
				if (--_player.Inventory < 0)
				{
					_player.Inventory = _sourceBordersForInventory.Count - 1;
				}
				CheckForItemQualify();
				_select.Play();
				_selectionBorderShow = true;
				_selectionInventoryTime = DateTime.Now;
			}
			if (GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed && _powerupItems > 0 && (DateTime.Now - _selectionInventoryTime).TotalMilliseconds >= 200.0)
			{
				if (++_player.Inventory >= _sourceBordersForInventory.Count)
				{
					_player.Inventory = 0;
				}
				CheckForItemQualify();
				_select.Play();
				_selectionBorderShow = true;
				_selectionInventoryTime = DateTime.Now;
			}
			if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed)
			{
				CheckForItemQualify();
				if (_itemQualify && (DateTime.Now - _selectionInventoryTime).TotalMilliseconds >= 200.0)
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
								_score += enemy.Score;
							}
						}
						foreach (Enemy2 item in _enemies2)
						{
							if (!item.Dead)
							{
								flag = true;
								item.Dead = true;
								item.NextFrameIndexOfExplosion = 0;
								item.ShowExplosion = true;
								item.PositionOfExplosion = item.Position;
								_score += item.Score;
							}
						}
						foreach (Enemy3 item2 in _enemies3)
						{
							if (!item2.Dead)
							{
								flag = true;
								item2.Dead = true;
								item2.NextFrameIndexOfExplosion = 0;
								item2.ShowExplosion = true;
								item2.PositionOfExplosion = item2.Position;
								_score += item2.Score;
							}
						}
						foreach (Enemy4 item3 in _enemies4)
						{
							if (!item3.Dead)
							{
								flag = true;
								item3.Dead = true;
								item3.NextFrameIndexOfExplosion = 0;
								item3.ShowExplosion = true;
								item3.PositionOfExplosion = item3.Position;
								_score += item3.Score;
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
						_player.Power = 1;
						_player.Projectiles.Clear();
						_player.Projectiles.Add(new Projectile());
						break;
					case 5:
						_player.Life = 10;
						break;
					}
					if (!_player.Dead && ((_player.Inventory == 2 && flag) || _player.Inventory != 2))
					{
						_powerupItems -= _powerupItemPrice;
						_powerup.Play();
						_selectionInventoryTime = DateTime.Now;
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
						_player.KeyPressTime = DateTime.Now;
					}
				}
				foreach (Enemy enemy2 in _enemies)
				{
					if (!enemy2.Dead && rectangle.Intersects(enemy2.BoxRectangle))
					{
						projectile2.Show = false;
						if (--enemy2.Life <= 0)
						{
							_score += enemy2.Score;
							enemy2.Dead = true;
							_hit.Play();
							enemy2.NextFrameIndexOfExplosion = 0;
							enemy2.ShowExplosion = true;
							enemy2.PositionOfExplosion = enemy2.Position;
						}
						_player.KeyPressTime = DateTime.Now;
					}
				}
				foreach (Enemy2 item4 in _enemies2)
				{
					if (!item4.Dead && rectangle.Intersects(item4.BoxRectangle))
					{
						projectile2.Show = false;
						if (--item4.Life <= 0)
						{
							_score += item4.Score;
							item4.Dead = true;
							_hit.Play();
							item4.NextFrameIndexOfExplosion = 0;
							item4.ShowExplosion = true;
							item4.PositionOfExplosion = item4.Position;
						}
						_player.KeyPressTime = DateTime.Now;
					}
				}
				foreach (Enemy3 item5 in _enemies3)
				{
					if (!item5.Dead && rectangle.Intersects(item5.BoxRectangle))
					{
						projectile2.Show = false;
						if (--item5.Life <= 0)
						{
							_score += item5.Score;
							item5.Dead = true;
							_hit.Play();
							item5.NextFrameIndexOfExplosion = 0;
							item5.ShowExplosion = true;
							item5.PositionOfExplosion = item5.Position;
						}
						_player.KeyPressTime = DateTime.Now;
					}
				}
				foreach (Enemy4 item6 in _enemies4)
				{
					if (!item6.Dead && rectangle.Intersects(item6.BoxRectangle))
					{
						projectile2.Show = false;
						if (--item6.Life <= 0)
						{
							_score += item6.Score;
							item6.Dead = true;
							_hit.Play();
							item6.NextFrameIndexOfExplosion = 0;
							item6.ShowExplosion = true;
							item6.PositionOfExplosion = item6.Position;
						}
						_player.KeyPressTime = DateTime.Now;
					}
				}
				foreach (Enemy5 item7 in _enemies5)
				{
					if (!item7.Dead && rectangle.Intersects(item7.BoxRectangle))
					{
						projectile2.Show = false;
						if (--item7.Life <= 0)
						{
							_score += item7.Score;
							item7.Dead = true;
							_hit.Play();
							item7.NextFrameIndexOfExplosion = 0;
							item7.ShowExplosion = true;
							item7.PositionOfExplosion = item7.Position;
						}
						_player.KeyPressTime = DateTime.Now;
					}
				}
			}
			foreach (Enemy enemy3 in _enemies)
			{
				foreach (Projectile projectile3 in enemy3.Projectiles)
				{
					if ((DateTime.Now - projectile3.ShowTime).TotalMilliseconds >= 1000.0 && !enemy3.Dead)
					{
						projectile3.Show = true;
						projectile3.Position = new Vector2(enemy3.CenterRight.X - (float)enemy3.Width, enemy3.CenterRight.Y);
						projectile3.ShowTime = DateTime.Now;
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
					Rectangle rectangle = new Rectangle((int)projectile3.Position.X - enemy3.SourceProjectile.Width, (int)projectile3.Position.Y, enemy3.SourceProjectile.Width, enemy3.SourceProjectile.Height);
					if (!_player.Dead && !_player.Hit && rectangle.Intersects(_player.BoxRectangle) && !_player.Invincibility)
					{
						_player.Hit = true;
						_player.HitTime = DateTime.Now;
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
			foreach (Enemy3 item8 in _enemies3)
			{
				foreach (Projectile projectile4 in item8.Projectiles)
				{
					if ((DateTime.Now - projectile4.ShowTime).TotalMilliseconds >= 1000.0 && !item8.Dead)
					{
						projectile4.Show = true;
						projectile4.Position = new Vector2(item8.CenterRight.X - (float)item8.Width, item8.CenterRight.Y);
						projectile4.ShowTime = DateTime.Now;
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
					Rectangle rectangle = new Rectangle((int)projectile4.Position.X - item8.SourceProjectile.Width, (int)projectile4.Position.Y, item8.SourceProjectile.Width, item8.SourceProjectile.Height);
					if (!_player.Dead && !_player.Hit && rectangle.Intersects(_player.BoxRectangle) && !_player.Invincibility)
					{
						_player.Hit = true;
						_player.HitTime = DateTime.Now;
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
			foreach (Enemy4 item9 in _enemies4)
			{
				foreach (Projectile projectile5 in item9.Projectiles)
				{
					if ((DateTime.Now - projectile5.ShowTime).TotalMilliseconds >= 1000.0 && !item9.Dead)
					{
						projectile5.Show = true;
						projectile5.Position = new Vector2(item9.CenterRight.X - (float)item9.Width, item9.CenterRight.Y);
						projectile5.ShowTime = DateTime.Now;
					}
					if (!projectile5.Show)
					{
						continue;
					}
					if (projectile5.Position.Y >= (float)_graphics.PreferredBackBufferHeight)
					{
						projectile5.Show = false;
						continue;
					}
					projectile5.Position = new Vector2(projectile5.Position.X - 10f, projectile5.Position.Y + 10f);
					Rectangle rectangle = new Rectangle((int)projectile5.Position.X - item9.SourceProjectile.Width, (int)projectile5.Position.Y, item9.SourceProjectile.Width, item9.SourceProjectile.Height);
					if (!_player.Dead && !_player.Hit && rectangle.Intersects(_player.BoxRectangle) && !_player.Invincibility)
					{
						_player.Hit = true;
						_player.HitTime = DateTime.Now;
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
			foreach (Enemy5 item10 in _enemies5)
			{
				foreach (Projectile projectile6 in item10.Projectiles)
				{
					if ((DateTime.Now - projectile6.ShowTime).TotalMilliseconds >= 1000.0 && !item10.Dead)
					{
						projectile6.Show = true;
						projectile6.Position = new Vector2(item10.CenterRight.X - (float)item10.Width, item10.CenterRight.Y);
						projectile6.ShowTime = DateTime.Now;
					}
					if (!projectile6.Show)
					{
						continue;
					}
					if (projectile6.Position.X <= 0f)
					{
						projectile6.Show = false;
						continue;
					}
					projectile6.Position = new Vector2(projectile6.Position.X - 20f, projectile6.Position.Y);
					Rectangle rectangle = new Rectangle((int)projectile6.Position.X - item10.SourceProjectile.Width, (int)projectile6.Position.Y, item10.SourceProjectile.Width, item10.SourceProjectile.Height);
					if (!_player.Dead && !_player.Hit && rectangle.Intersects(_player.BoxRectangle) && !_player.Invincibility)
					{
						_player.Hit = true;
						_player.HitTime = DateTime.Now;
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
			_layer1LeftToDrawOnScreen = _graphics.PreferredBackBufferWidth - _layer1Split.Width;
			if (_currentLevel >= 2)
			{
				_layer1Scroll = (int)(0.0 - gameTime.TotalGameTime.TotalMilliseconds / 5.0 % (double)_layer1Split.Width);
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
				_powerupItem.Position = _powerupItem.RandomLocation((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f, (float)base.GraphicsDevice.Viewport.Height - (float)base.GraphicsDevice.Viewport.Height * 0.03f);
			}
			if (_birds.Count((Bird p) => p.Dead) == _birds.Count)
			{
				_powerupItem.Position = _birds.First().Position;
				_powerupItem.Hidden = false;
				Bird.ResetAllBirdPositions(_birds, base.GraphicsDevice);
			}
			if (_birds.Count((Bird p) => p.Position.X <= 0f) != 0)
			{
				Bird.ResetAllBirdPositions(_birds, base.GraphicsDevice);
			}
			if ((DateTime.Now - _powerupItem.FrameTime).TotalMilliseconds >= 250.0)
			{
				if (++_powerupItem.NextFrameIndex >= _powerupItem.SourceRectangles.Count)
				{
					_powerupItem.NextFrameIndex = 0;
				}
				_powerupItem.FrameTime = DateTime.Now;
			}
			foreach (Bird bird3 in _birds)
			{
				if (!_player.Hit && _player.BoxRectangle.Intersects(bird3.BoxRectangle) && !bird3.Dead && !_player.Dead && !_player.Invincibility)
				{
					_player.Hit = true;
					_player.HitTime = DateTime.Now;
					_playerHit.Play();
					CheckForItemQualify();
					if (_itemQualify)
					{
						_selectionBorderShow = true;
					}
				}
				if ((DateTime.Now - bird3.FrameTime).TotalMilliseconds >= 100.0)
				{
					if (!bird3.Dead)
					{
						if (!bird3.Reverse)
						{
							if (++bird3.NextFrameIndex >= bird3.SourceRectangles.Count - 1)
							{
								bird3.Reverse = true;
							}
						}
						else if (--bird3.NextFrameIndex <= 1)
						{
							bird3.Reverse = false;
						}
						float num3 = bird3.Position.X - 5f;
						double num4 = Math.Sin(MathHelper.ToRadians(num3)) * 7.0;
						bird3.Position = new Vector2(num3, (int)((double)bird3.Position.Y + num4));
					}
					bird3.FrameTime = DateTime.Now;
				}
				if (bird3.Dead && bird3.ShowExplosion && (DateTime.Now - bird3.ExplosionFrameTime).TotalMilliseconds >= 33.0)
				{
					if (++bird3.NextFrameIndexOfExplosion >= bird3.SourceOfExplosion.Count)
					{
						bird3.ShowExplosion = false;
						bird3.NextFrameIndexOfExplosion = 0;
					}
					bird3.ExplosionFrameTime = DateTime.Now;
				}
			}
			if (!_player.Hit)
			{
				foreach (Enemy enemy4 in _enemies)
				{
					if (_player.BoxRectangle.Intersects(enemy4.BoxRectangle) && !enemy4.Dead && !_player.Dead && !_player.Invincibility)
					{
						_player.Hit = true;
						_player.HitTime = DateTime.Now;
						_playerHit.Play();
						CheckForItemQualify();
						if (_itemQualify)
						{
							_selectionBorderShow = true;
						}
					}
				}
				foreach (Enemy2 item11 in _enemies2)
				{
					if (_player.BoxRectangle.Intersects(item11.BoxRectangle) && !item11.Dead && !_player.Dead && !_player.Invincibility)
					{
						_player.Hit = true;
						_player.HitTime = DateTime.Now;
						_playerHit.Play();
						CheckForItemQualify();
						if (_itemQualify)
						{
							_selectionBorderShow = true;
						}
					}
				}
				foreach (Enemy3 item12 in _enemies3)
				{
					if (_player.BoxRectangle.Intersects(item12.BoxRectangle) && !item12.Dead && !_player.Dead && !_player.Invincibility)
					{
						_player.Hit = true;
						_player.HitTime = DateTime.Now;
						_playerHit.Play();
						CheckForItemQualify();
						if (_itemQualify)
						{
							_selectionBorderShow = true;
						}
					}
				}
				foreach (Enemy4 item13 in _enemies4)
				{
					if (_player.BoxRectangle.Intersects(item13.BoxRectangle) && !item13.Dead && !_player.Dead && !_player.Invincibility)
					{
						_player.Hit = true;
						_player.HitTime = DateTime.Now;
						_playerHit.Play();
						CheckForItemQualify();
						if (_itemQualify)
						{
							_selectionBorderShow = true;
						}
					}
				}
				foreach (Enemy5 item14 in _enemies5)
				{
					if (_player.BoxRectangle.Intersects(item14.BoxRectangle) && !item14.Dead && !_player.Dead && !_player.Invincibility)
					{
						_player.Hit = true;
						_player.HitTime = DateTime.Now;
						_playerHit.Play();
						CheckForItemQualify();
						if (_itemQualify)
						{
							_selectionBorderShow = true;
						}
					}
				}
			}
			foreach (Enemy enemy5 in _enemies)
			{
				if ((DateTime.Now - enemy5.FrameTime).TotalMilliseconds >= 33.0)
				{
					if (++enemy5.NextFrameIndex >= enemy5.SourceRectangles.Count - 1)
					{
						enemy5.NextFrameIndex = 0;
					}
					if ((DateTime.Now - enemy5.ChangePositionTime).TotalMilliseconds >= 500.0)
					{
						enemy5.ChangePositionTime = DateTime.Now;
						enemy5.ReversePosition = !enemy5.ReversePosition;
					}
					if (enemy5.Position.X <= 0f)
					{
						Random random = new Random((int)DateTime.Now.Ticks);
						enemy5.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, random.Next(60, (int)((float)base.GraphicsDevice.Viewport.Height - (float)base.GraphicsDevice.Viewport.Height * 0.03f - 30f)));
					}
					else if (!enemy5.ReversePosition)
					{
						enemy5.Position = new Vector2(enemy5.Position.X - 10f, enemy5.Position.Y - 5f);
					}
					else
					{
						enemy5.Position = new Vector2(enemy5.Position.X - 10f, enemy5.Position.Y + 5f);
					}
					enemy5.FrameTime = DateTime.Now;
				}
			}
			foreach (Enemy2 item15 in _enemies2)
			{
				if (!((DateTime.Now - item15.FrameTime).TotalMilliseconds >= 100.0))
				{
					continue;
				}
				if (item15.Reverse)
				{
					if (--item15.NextFrameIndex <= 0)
					{
						item15.Reverse = false;
					}
				}
				else if (++item15.NextFrameIndex >= item15.SourceRectangles.Count - 1)
				{
					item15.Reverse = true;
				}
				if (item15.Position.X <= 0f)
				{
					Random random = new Random((int)DateTime.Now.Ticks);
					item15.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, random.Next(60, (int)((float)base.GraphicsDevice.Viewport.Height - (float)base.GraphicsDevice.Viewport.Height * 0.03f - 30f)));
				}
				else
				{
					item15.Position = new Vector2(item15.Position.X - 10f, item15.Position.Y);
				}
				item15.FrameTime = DateTime.Now;
			}
			foreach (Enemy3 item16 in _enemies3)
			{
				if (!((DateTime.Now - item16.FrameTime).TotalMilliseconds >= 33.0))
				{
					continue;
				}
				if (item16.Reverse)
				{
					if (--item16.NextFrameIndex <= 0)
					{
						item16.Reverse = false;
					}
				}
				else if (++item16.NextFrameIndex >= item16.SourceRectangles.Count - 1)
				{
					item16.Reverse = true;
				}
				if (item16.Position.X <= 0f)
				{
					Random random = new Random((int)DateTime.Now.Ticks);
					item16.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, random.Next(60, (int)((float)base.GraphicsDevice.Viewport.Height - (float)base.GraphicsDevice.Viewport.Height * 0.03f - 30f)));
				}
				else
				{
					item16.Position = new Vector2(item16.Position.X - 10f, item16.Position.Y);
				}
				item16.FrameTime = DateTime.Now;
			}
			foreach (Enemy4 item17 in _enemies4)
			{
				if (gameTime.TotalGameTime.TotalMilliseconds - item17.GameTime >= 33.0)
				{
					if (item17.Position.X <= 0f)
					{
						item17.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, 30f);
					}
					else
					{
						item17.Position = new Vector2(item17.Position.X - (float)item17.Speed, item17.Position.Y);
					}
					item17.GameTime = gameTime.TotalGameTime.TotalMilliseconds;
				}
			}
			foreach (Enemy5 item18 in _enemies5)
			{
				if (!_player.Dead && (DateTime.Now - item18.FrameTime).TotalMilliseconds >= 33.0)
				{
					if (item18.Position.Y <= _player.Position.Y)
					{
						item18.Position = new Vector2(item18.Position.X, item18.Position.Y + 5f);
					}
					else if (item18.Position.Y - _player.Position.Y <= 5f)
					{
						item18.Position = new Vector2(item18.Position.X, item18.Position.Y);
					}
					else
					{
						item18.Position = new Vector2(item18.Position.X, item18.Position.Y - 5f);
					}
					item18.FrameTime = DateTime.Now;
				}
			}
			if (_player.Hit)
			{
				if ((DateTime.Now - _player.HitTime).TotalMilliseconds <= 1000.0)
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
					_player.Life--;
					if (_player.Life <= 0)
					{
						_player.Dead = true;
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
					_countDownTime = gameTime.TotalGameTime.TotalMilliseconds;
				}
				int num5 = 10 - (int)((gameTime.TotalGameTime.TotalMilliseconds - _countDownTime) / 1000.0).Value;
				if (num5 <= 0 || _powerupItems < 6)
				{
					_gameStarted = false;
					_title = true;
					_player.Dead = false;
					_player.Life = 10;
					_player.Projectiles = new List<Projectile>();
					_player.Projectiles.Add(new Projectile());
					_player.TintColor = Color.White;
					_player.Power = 1;
					_player.Inventory = 0;
					_player.Speed = 1;
					_player.NextPowerupBonus = 2500;
					_currentLevel = 1;
					_powerupItems = 3;
					MediaPlayer.Play(_crisson);
					Bird.ResetAllBirdPositions(_birds, base.GraphicsDevice);
					_startUpTime = DateTime.Now;
					_levelTime[_currentLevel - 1] = DateTime.Now;
					_startUpShow = true;
					_startUpTime = DateTime.Now;
					_levelTime.Clear();
					_levelTime.Add(DateTime.Now);
					_fadeIncrement = 0f;
					_fadeOut = false;
					_stageClearTime = DateTime.Now;
					_score = 0;
					RemoveAllEnemies();
					CheckForItemQualify();
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
			foreach (Enemy enemy6 in _enemies)
			{
				if (enemy6.Dead && (DateTime.Now - enemy6.ExplosionFrameTime).TotalMilliseconds >= 33.0)
				{
					if (++enemy6.NextFrameIndexOfExplosion >= enemy6.SourceOfExplosion.Count)
					{
						enemy6.ShowExplosion = false;
					}
					enemy6.ExplosionFrameTime = DateTime.Now;
				}
			}
			foreach (Enemy2 item19 in _enemies2)
			{
				if (item19.Dead && (DateTime.Now - item19.ExplosionFrameTime).TotalMilliseconds >= 33.0)
				{
					if (++item19.NextFrameIndexOfExplosion >= item19.SourceOfExplosion.Count)
					{
						item19.ShowExplosion = false;
					}
					item19.ExplosionFrameTime = DateTime.Now;
				}
			}
			foreach (Enemy3 item20 in _enemies3)
			{
				if (item20.Dead && (DateTime.Now - item20.ExplosionFrameTime).TotalMilliseconds >= 33.0)
				{
					if (++item20.NextFrameIndexOfExplosion >= item20.SourceOfExplosion.Count)
					{
						item20.ShowExplosion = false;
					}
					item20.ExplosionFrameTime = DateTime.Now;
				}
			}
			foreach (Enemy4 item21 in _enemies4)
			{
				if (item21.Dead && (DateTime.Now - item21.ExplosionFrameTime).TotalMilliseconds >= 33.0)
				{
					if (++item21.NextFrameIndexOfExplosion >= item21.SourceOfExplosion.Count)
					{
						item21.ShowExplosion = false;
					}
					item21.ExplosionFrameTime = DateTime.Now;
				}
			}
			foreach (Enemy5 item22 in _enemies5)
			{
				if (item22.Dead && (DateTime.Now - item22.ExplosionFrameTime).TotalMilliseconds >= 33.0)
				{
					if (++item22.NextFrameIndexOfExplosion >= item22.SourceOfExplosion.Count)
					{
						item22.ShowExplosion = false;
					}
					item22.ExplosionFrameTime = DateTime.Now;
				}
			}
			if (gameTime.TotalGameTime.TotalMilliseconds - _spawnTime >= 10000.0 && !_stageCleared)
			{
				if (_enemies.Count < 3 + _levelFinishCount)
				{
					AddNewEnemy();
					SetTextureExplosionForEnemy(_enemies.Last());
				}
				if (_enemies2.Count < 2 + _levelFinishCount)
				{
					AddNewEnemy2();
					SetTextureExplosionForEnemy(_enemies2.Last());
				}
				if (_currentLevel >= 2 && _enemies4.Count < 2 + _levelFinishCount)
				{
					AddNewEnemy4();
					SetTextureExplosionForEnemy(_enemies4.Last());
				}
				if (_currentLevel >= 3 && _enemies3.Count < 2 + _levelFinishCount)
				{
					AddNewEnemy3();
					SetTextureExplosionForEnemy(_enemies3.Last());
				}
				if (_currentLevel >= 4 && _enemies5.Count < 1 + _levelFinishCount)
				{
					AddNewEnemy5();
					SetTextureExplosionForEnemy(_enemies5.Last());
				}
				foreach (Enemy enemy7 in _enemies)
				{
					if (enemy7.Dead)
					{
						enemy7.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, enemy7.Position.Y);
						enemy7.Dead = false;
						enemy7.Life = 2;
					}
				}
				foreach (Enemy2 item23 in _enemies2)
				{
					if (item23.Dead)
					{
						item23.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, item23.Position.Y);
						item23.Dead = false;
						item23.Life = 3;
					}
				}
				foreach (Enemy3 item24 in _enemies3)
				{
					if (item24.Dead)
					{
						item24.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, item24.Position.Y);
						item24.Dead = false;
						item24.Life = 2;
					}
				}
				foreach (Enemy4 item25 in _enemies4)
				{
					if (item25.Dead)
					{
						item25.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, item25.Position.Y);
						item25.Dead = false;
						item25.Life = 5;
					}
				}
				foreach (Enemy5 item26 in _enemies5)
				{
					if (item26.Dead)
					{
						item26.Position = new Vector2((float)base.GraphicsDevice.Viewport.Width - (float)base.GraphicsDevice.Viewport.Width * 0.03f - 50f, item26.Position.Y);
						item26.Dead = false;
						item26.Life = 10;
					}
				}
				CheckForItemQualify();
				_spawnTime = gameTime.TotalGameTime.TotalMilliseconds;
			}
			if (_powerupItems > 0)
			{
				if ((DateTime.Now - _selectionBorderFlashTime).TotalMilliseconds >= 250.0)
				{
					_selectionBorderShow = !_selectionBorderShow;
					_selectionBorderFlashTime = DateTime.Now;
				}
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
			if ((DateTime.Now - _player.InvincibilityTime).TotalMilliseconds >= 20000.0 && _player.Invincibility)
			{
				MediaPlayer.Play(_currentSong);
				_player.Invincibility = false;
				CheckForItemQualify();
			}
			if ((DateTime.Now - _startUpTime).TotalMilliseconds >= 15000.0)
			{
				_startUpShow = false;
				_startUpTime = DateTime.Now;
			}
			if (!_player.Dead)
			{
				if ((DateTime.Now - _levelTime[_currentLevel - 1]).TotalMilliseconds >= LEVEL_TIME)
				{
					_levelFinishCount++;
					LEVEL_TIME += 30000.0;
					MediaPlayer.Play(_levelFinish);
					_stageCleared = true;
					_levelTime[_currentLevel - 1] = DateTime.Now;
					if (++_currentLevel > 4)
					{
						_levelTime.Clear();
						_currentLevel = 1;
					}
					_startUpShow = true;
					_startUpTime = DateTime.Now;
					_levelTime.Add(DateTime.Now);
					_fadeIncrement = 0f;
					_fadeOut = false;
					_stageClearTime = DateTime.Now;
					RemoveAllEnemies();
				}
				else if (_stageCleared && (DateTime.Now - _stageClearTime).TotalMilliseconds >= 10000.0)
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
						_layer1StartFadeTime = gameTime.TotalGameTime.TotalMilliseconds;
						break;
					case 3:
						MediaPlayer.Play(_azimuth);
						_currentSong = _azimuth;
						break;
					case 4:
						MediaPlayer.Play(_blueChill);
						_currentSong = _blueChill;
						break;
					}
					_spawnTime = gameTime.TotalGameTime.TotalMilliseconds;
					_stageCleared = false;
					_startUpShow = true;
					_startUpTime = DateTime.Now;
					_fadeIncrement = 0f;
					_fadeOut = false;
				}
			}
			if (_score >= _player.NextPowerupBonus)
			{
				_player.NextPowerupBonus += 2500;
				_pickup.Play();
				_powerupItems += 2;
				CheckForItemQualify();
			}
		}
		base.Update(gameTime);
	}

	private void RemoveAllEnemies()
	{
		_enemies.Clear();
		_enemies2.Clear();
		_enemies3.Clear();
		_enemies4.Clear();
		_enemies5.Clear();
	}

	private void CheckForItemQualify()
	{
		switch (_player.Inventory)
		{
		case 0:
			if (_powerupItems >= 1 && _player.Speed < 4)
			{
				_itemQualify = true;
			}
			else
			{
				_itemQualify = false;
			}
			_powerupItemPrice = 1;
			break;
		case 1:
			if (_powerupItems >= 2 && _player.Projectiles.Count < 5)
			{
				_itemQualify = true;
			}
			else
			{
				_itemQualify = false;
			}
			_powerupItemPrice = 2;
			break;
		case 2:
			if (_powerupItems >= 4)
			{
				bool flag = false;
				foreach (Enemy enemy in _enemies)
				{
					if (!enemy.Dead)
					{
						flag = true;
					}
				}
				foreach (Enemy2 item in _enemies2)
				{
					if (!item.Dead)
					{
						flag = true;
					}
				}
				foreach (Enemy3 item2 in _enemies3)
				{
					if (!item2.Dead)
					{
						flag = true;
					}
				}
				foreach (Enemy4 item3 in _enemies4)
				{
					if (!item3.Dead)
					{
						flag = true;
					}
				}
				foreach (Enemy5 item4 in _enemies5)
				{
					if (!item4.Dead)
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
			_powerupItemPrice = 4;
			break;
		case 3:
			if (_powerupItems >= 6 && !_player.Invincibility)
			{
				_itemQualify = true;
			}
			else
			{
				_itemQualify = false;
			}
			_powerupItemPrice = 6;
			break;
		case 4:
			if (_powerupItems >= 6 && _player.Dead)
			{
				_itemQualify = true;
			}
			else
			{
				_itemQualify = false;
			}
			_powerupItemPrice = 6;
			break;
		case 5:
			if (_powerupItems >= 10 && !_player.Dead && _player.Life < 10)
			{
				_itemQualify = true;
			}
			else
			{
				_itemQualify = false;
			}
			_powerupItemPrice = 10;
			break;
		}
		if (_player.Dead && _player.Inventory != 4)
		{
			_itemQualify = false;
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.CornflowerBlue);
		_spriteBatch.Begin();
		_spriteBatch.Draw(_backgroundSplit, new Rectangle(_xScroll, 0, _backgroundSplit.Width, _backgroundSplit.Height), Color.White);
		_spriteBatch.Draw(_backgroundSplit, new Vector2(_backgroundSplit.Width + _xScroll, 0f), new Rectangle(0, 0, _spaceLeftToDrawOnScreen - _xScroll, _backgroundSplit.Height), Color.White);
		if (_help)
		{
			float num = (float)((gameTime.TotalGameTime.TotalMilliseconds - _titleStartFadeTime) / 1000.0).Value;
			_spriteBatch.Draw(_xboxControllerConfig, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2 - 150, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 150), Color.White * num);
		}
		if (_title && !_help)
		{
			float num = (float)((gameTime.TotalGameTime.TotalMilliseconds - _titleStartFadeTime) / 1000.0).Value;
			_spriteBatch.Draw(_jetStarUniverse, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2 - 120, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 195), Color.White * num);
			if (!_paused)
			{
				_spriteBatch.Draw(_startTitle, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 50), Color.White * num);
			}
			else
			{
				_spriteBatch.Draw(_continueTitle, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 - 50), Color.White * num);
			}
			_spriteBatch.Draw(_optionsTitle, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 + 50), Color.White * num);
			_spriteBatch.Draw(_exitTitle, new Vector2(base.GraphicsDevice.Viewport.Width / 2 - _startTitle.Width / 2, base.GraphicsDevice.Viewport.Height / 2 - _startTitle.Height / 2 + 150), Color.White * num);
			Color color = (((gameTime.TotalGameTime.TotalMilliseconds - _fadeSelectTime) % 150.0 >= 75.0) ? Color.White : Color.Black);
			switch (_titleSelect)
			{
			case 0:
				_spriteBatch.Draw(_player.Texture2D, new Rectangle(180, 170, _player.SourceRectangles[0].Width, _player.SourceRectangles[0].Height), _player.SourceRectangles[0], color);
				break;
			case 1:
				_spriteBatch.Draw(_player.Texture2D, new Rectangle(180, 270, _player.SourceRectangles[0].Width, _player.SourceRectangles[0].Height), _player.SourceRectangles[0], color);
				break;
			case 2:
				_spriteBatch.Draw(_player.Texture2D, new Rectangle(180, 370, _player.SourceRectangles[0].Width, _player.SourceRectangles[0].Height), _player.SourceRectangles[0], color);
				break;
			}
		}
		if (_gameStarted && !_title)
		{
			if (_player.Dead)
			{
				int num2 = (int)((gameTime.TotalGameTime.TotalMilliseconds - _countDownTime) / 1000.0).Value;
				_spriteBatch.DrawString(_spriteFont, $"Continue? {10 - num2}", new Vector2(base.GraphicsDevice.Viewport.Width / 2, base.GraphicsDevice.Viewport.Height / 2), Color.White);
			}
			if (_currentLevel >= 2)
			{
				double num3 = 0.0;
				if (_currentLevel == 2 && !_stageCleared)
				{
					num3 = (gameTime.TotalGameTime.TotalMilliseconds - _layer1StartFadeTime) / 1000.0;
				}
				else if (_currentLevel > 2)
				{
					num3 = 1.0;
				}
				_spriteBatch.Draw(_layer1Split, new Rectangle(_layer1Scroll, 0, _layer1Split.Width, _layer1Split.Height), Color.White * (float)num3);
				_spriteBatch.Draw(_layer1Split, new Vector2(_layer1Split.Width + _layer1Scroll, 0f), new Rectangle(0, 0, _layer1LeftToDrawOnScreen - _layer1Scroll, _layer1Split.Height), Color.White * (float)num3);
			}
			if (_startUpShow)
			{
				Texture2D texture = _currentLevel switch
				{
					1 => _level1, 
					2 => _level2, 
					3 => _level3, 
					4 => _level4, 
					_ => _level1, 
				};
				if ((DateTime.Now - _startUpTime).TotalMilliseconds <= 15000.0)
				{
					if (_fadeOut)
					{
						if ((DateTime.Now - _startUpTime).TotalMilliseconds >= 5000.0)
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
					_spriteBatch.Draw(_player.Texture2D, projectile.Position, _player.SourceProjectile, Color.White);
				}
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
			foreach (Enemy3 item in _enemies3)
			{
				foreach (Projectile projectile3 in item.Projectiles)
				{
					if (projectile3.Show)
					{
						_spriteBatch.Draw(item.Texture2D, projectile3.Position, item.SourceProjectile, Color.White);
					}
				}
			}
			foreach (Enemy4 item2 in _enemies4)
			{
				foreach (Projectile projectile4 in item2.Projectiles)
				{
					if (projectile4.Show)
					{
						_spriteBatch.Draw(item2.Texture2D, projectile4.Position, item2.SourceProjectile, Color.White);
					}
				}
			}
			foreach (Enemy5 item3 in _enemies5)
			{
				foreach (Projectile projectile5 in item3.Projectiles)
				{
					if (projectile5.Show)
					{
						_spriteBatch.Draw(item3.Texture2D, projectile5.Position, item3.SourceProjectile, Color.White);
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
			foreach (Enemy2 item4 in _enemies2)
			{
				if (!item4.Dead)
				{
					if (item4.Hit)
					{
						_spriteBatch.Draw(item4.Texture2D, item4.Position, item4.SourceRectangles[item4.NextFrameIndex], item4.TintColor);
					}
					else
					{
						_spriteBatch.Draw(item4.Texture2D, item4.Position, item4.SourceRectangles[item4.NextFrameIndex], Color.White);
					}
				}
				else if (item4.ShowExplosion)
				{
					_spriteBatch.Draw(item4.TextureOfExplosion, item4.PositionOfExplosion, item4.SourceOfExplosion[item4.NextFrameIndexOfExplosion], Color.White);
				}
			}
			foreach (Enemy3 item5 in _enemies3)
			{
				if (!item5.Dead)
				{
					if (item5.Hit)
					{
						_spriteBatch.Draw(item5.Texture2D, item5.Position, item5.SourceRectangles[item5.NextFrameIndex], item5.TintColor);
					}
					else
					{
						_spriteBatch.Draw(item5.Texture2D, item5.Position, item5.SourceRectangles[item5.NextFrameIndex], Color.White);
					}
				}
				else if (item5.ShowExplosion)
				{
					_spriteBatch.Draw(item5.TextureOfExplosion, item5.PositionOfExplosion, item5.SourceOfExplosion[item5.NextFrameIndexOfExplosion], Color.White);
				}
			}
			foreach (Enemy4 item6 in _enemies4)
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
			foreach (Enemy5 item7 in _enemies5)
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
			}
			else if (_player.ShowExplosion)
			{
				_spriteBatch.Draw(_player.TextureOfExplosion, _player.PositionOfExplosion, _player.SourceOfExplosion[_player.NextFrameIndexOfExplosion], Color.White);
			}
			_spriteBatch.DrawString(_spriteFont, $"Power", new Vector2(50f, 10f), Color.White);
			for (int i = 0; i < _player.Power; i++)
			{
				_spriteBatch.Draw(_player.Texture2D, new Vector2(130f + (float)(i * 20), 22f), _player.SourceProjectile, Color.White);
			}
			_spriteBatch.DrawString(_spriteFont, $"Life", new Vector2(250f, 10f), _player.TintColor);
			for (int i = 0; i < _player.Life; i++)
			{
				if (_player.Life <= 3)
				{
					_spriteBatch.Draw(_lifeBlock, new Vector2(325f + (float)(i * 16), 15f), new Rectangle(0, 0, 16, 16), Color.Red);
				}
				else if (_player.Life <= 6)
				{
					_spriteBatch.Draw(_lifeBlock, new Vector2(325f + (float)(i * 16), 15f), new Rectangle(0, 0, 16, 16), Color.YellowGreen);
				}
				else
				{
					_spriteBatch.Draw(_lifeBlock, new Vector2(325f + (float)(i * 16), 15f), new Rectangle(0, 0, 16, 16), Color.Green);
				}
			}
			_spriteBatch.DrawString(_spriteFont, $"Score: {_score:n0} Pts", new Vector2(525f, 10f), Color.White);
			Texture2D texture2D = new Texture2D(base.GraphicsDevice, 1, 1);
			texture2D.SetData(new Color[1] { Color.White });
			Rectangle rectangle = _sourceBordersForInventory[_player.Inventory];
			int num4 = 2;
			_spriteBatch.DrawString(_spriteFont, $"Speed  Power  Crash  Invincibility  Revive  Life", new Vector2(130f, (float)base.GraphicsDevice.Viewport.Height - (float)base.GraphicsDevice.Viewport.Height * 0.03f - 30f), Color.White);
			if (_selectionBorderShow)
			{
				if (_itemQualify)
				{
					_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Left, rectangle.Top, num4, rectangle.Height), Color.Blue);
					_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Right, rectangle.Top, num4, rectangle.Height), Color.Blue);
					_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, num4), Color.Blue);
					_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, num4), Color.Blue);
				}
				else
				{
					_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Left, rectangle.Top, num4, rectangle.Height), Color.Gray);
					_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Right, rectangle.Top, num4, rectangle.Height), Color.Gray);
					_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, num4), Color.Gray);
					_spriteBatch.Draw(texture2D, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, num4), Color.Gray);
				}
				_spriteBatch.DrawString(_spriteFont, $"{_powerupItems}/{_powerupItemPrice}", new Vector2(rectangle.Left + 5, rectangle.Top - 25), Color.Yellow);
			}
		}
		_spriteBatch.End();
		base.Draw(gameTime);
	}
}
