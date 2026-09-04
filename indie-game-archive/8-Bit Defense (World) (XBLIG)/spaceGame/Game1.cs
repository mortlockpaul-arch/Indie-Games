using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace spaceGame;

public class Game1 : Game
{
	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private SoundEffect sndLaser;

	private SoundEffect sndEnemyLaser;

	private SoundEffect sndEnemyMove;

	private SoundEffect sndImpact;

	private SoundEffect sndEnemyDestroy;

	private SoundEffect sndExtraLife;

	private SoundEffect sndGameOver;

	private SoundEffect sndPlayerDeath;

	private SoundEffect sndUpgradeSelect;

	private SoundEffect sndUpgradeSound;

	private SoundEffect sndStartSound;

	public SpriteFont font;

	private Background gameBackground;

	public UpgradeMenu theUpgradeMenu;

	public AI_Controller AIC;

	public MainShip mMainShipSprite;

	public Barricade Barricade1;

	public Barricade Barricade2;

	public Barricade Barricade3;

	public Barricade Barricade4;

	public int iGameState;

	private int restartDelay;

	private bool confirmQuit;

	private bool quitCanceled;

	private bool bGOSoundPlayed;

	private KeyboardState keyBoardState;

	private GamePadState gamePadState;

	private KeyboardState oldKeyBoardState;

	private GamePadState oldGamePadState;

	private GamePadState p1oldGamePadState;

	private GamePadState p2oldGamePadState;

	private GamePadState p3oldGamePadState;

	private GamePadState p4oldGamePadState;

	public PlayerIndex ThePlayer;

	public Game1()
	{
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Viewport.Height;
		graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Viewport.Width;
		graphics.PreferredBackBufferHeight = 720;
		graphics.PreferredBackBufferWidth = 1280;
		graphics.ApplyChanges();
		gameBackground = new Background(this);
		theUpgradeMenu = new UpgradeMenu(this);
		mMainShipSprite = new MainShip(this, 1);
		AIC = new AI_Controller(this);
		Barricade1 = new Barricade(this, new Vector2(416f, 544f));
		Barricade2 = new Barricade(this, new Vector2(544f, 544f));
		Barricade3 = new Barricade(this, new Vector2(672f, 544f));
		Barricade4 = new Barricade(this, new Vector2(800f, 544f));
		iGameState = 0;
		restartDelay = 60;
		confirmQuit = false;
		quitCanceled = false;
		bGOSoundPlayed = false;
		ThePlayer = PlayerIndex.One;
		base.Initialize();
		AIC.SpawnBasicEnemy(new Vector2(500f, 320f));
		AIC.SpawnBasicEnemy(new Vector2(625f, 320f));
		AIC.SpawnBasicEnemy(new Vector2(750f, 320f));
		AIC.EnemyArray[1].EnableShooting();
		AIC.EnemyArray[2].EnableDefending();
		AIC.EnemyArray[2].iHealth = 1;
		SoundExtraLife();
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		gameBackground.LoadContent(base.Content);
		theUpgradeMenu.LoadContent(base.Content);
		font = base.Content.Load<SpriteFont>("HighScoreFont");
		mMainShipSprite.LoadContent(base.Content);
		AIC.LoadContent(base.Content);
		Barricade1.LoadContent(base.Content);
		Barricade2.LoadContent(base.Content);
		Barricade3.LoadContent(base.Content);
		Barricade4.LoadContent(base.Content);
		sndLaser = base.Content.Load<SoundEffect>("PlayerShoot");
		sndEnemyLaser = base.Content.Load<SoundEffect>("EnemyShoot");
		sndEnemyMove = base.Content.Load<SoundEffect>("EnemyMovement");
		sndImpact = base.Content.Load<SoundEffect>("Impact");
		sndEnemyDestroy = base.Content.Load<SoundEffect>("EnemyDestroy");
		sndExtraLife = base.Content.Load<SoundEffect>("ExtraLife");
		sndGameOver = base.Content.Load<SoundEffect>("GameOver");
		sndPlayerDeath = base.Content.Load<SoundEffect>("PlayerDeath");
		sndUpgradeSelect = base.Content.Load<SoundEffect>("UpgradeSelect");
		sndUpgradeSound = base.Content.Load<SoundEffect>("UpgradeSound");
		sndStartSound = base.Content.Load<SoundEffect>("StartSound");
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		oldKeyBoardState = keyBoardState;
		oldGamePadState = gamePadState;
		keyBoardState = Keyboard.GetState();
		gamePadState = GamePad.GetState(ThePlayer);
		if (keyBoardState.IsKeyDown(Keys.Escape))
		{
			Exit();
		}
		switch (iGameState)
		{
		case 0:
			if (confirmQuit)
			{
				if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
				{
					Exit();
				}
				if (GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed)
				{
					Exit();
				}
				if (GamePad.GetState(PlayerIndex.Three).Buttons.A == ButtonState.Pressed)
				{
					Exit();
				}
				if (GamePad.GetState(PlayerIndex.Four).Buttons.A == ButtonState.Pressed)
				{
					Exit();
				}
				if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed && p1oldGamePadState.Buttons.B != ButtonState.Pressed)
				{
					confirmQuit = false;
					quitCanceled = true;
					SoundUpgradeMove();
				}
				if (GamePad.GetState(PlayerIndex.Two).Buttons.B == ButtonState.Pressed && p2oldGamePadState.Buttons.B != ButtonState.Pressed)
				{
					confirmQuit = false;
					quitCanceled = true;
					SoundUpgradeMove();
				}
				if (GamePad.GetState(PlayerIndex.Three).Buttons.B == ButtonState.Pressed && p3oldGamePadState.Buttons.B != ButtonState.Pressed)
				{
					confirmQuit = false;
					quitCanceled = true;
					SoundUpgradeMove();
				}
				if (GamePad.GetState(PlayerIndex.Four).Buttons.B == ButtonState.Pressed && p4oldGamePadState.Buttons.B != ButtonState.Pressed)
				{
					confirmQuit = false;
					quitCanceled = true;
					SoundUpgradeMove();
				}
			}
			else if (restartDelay <= 0)
			{
				if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || (keyBoardState.IsKeyDown(Keys.Space) && !oldKeyBoardState.IsKeyDown(Keys.Space)))
				{
					iGameState = 1;
					ThePlayer = PlayerIndex.One;
					SoundStartSound();
				}
				if (GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed)
				{
					iGameState = 1;
					ThePlayer = PlayerIndex.Two;
					SoundStartSound();
				}
				if (GamePad.GetState(PlayerIndex.Three).Buttons.A == ButtonState.Pressed)
				{
					iGameState = 1;
					ThePlayer = PlayerIndex.Three;
					SoundStartSound();
				}
				if (GamePad.GetState(PlayerIndex.Four).Buttons.A == ButtonState.Pressed)
				{
					iGameState = 1;
					ThePlayer = PlayerIndex.Four;
					SoundStartSound();
				}
				if (iGameState == 1)
				{
					AIC.EnemyArray[0].Despawn();
					AIC.EnemyArray[1].Despawn();
					AIC.EnemyArray[2].Despawn();
				}
			}
			else
			{
				restartDelay--;
			}
			if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed && p1oldGamePadState.Buttons.B != ButtonState.Pressed)
			{
				SoundUpgradeSelect();
				if (!quitCanceled)
				{
					confirmQuit = true;
				}
			}
			if (GamePad.GetState(PlayerIndex.Two).Buttons.B == ButtonState.Pressed && p2oldGamePadState.Buttons.B != ButtonState.Pressed)
			{
				SoundUpgradeSelect();
				if (!quitCanceled)
				{
					confirmQuit = true;
				}
			}
			if (GamePad.GetState(PlayerIndex.Three).Buttons.B == ButtonState.Pressed && p3oldGamePadState.Buttons.B != ButtonState.Pressed)
			{
				SoundUpgradeSelect();
				if (!quitCanceled)
				{
					confirmQuit = true;
				}
			}
			if (GamePad.GetState(PlayerIndex.Four).Buttons.B == ButtonState.Pressed && p4oldGamePadState.Buttons.B != ButtonState.Pressed)
			{
				SoundUpgradeSelect();
				if (!quitCanceled)
				{
					confirmQuit = true;
				}
			}
			quitCanceled = false;
			break;
		case 1:
			if (base.IsActive)
			{
				mMainShipSprite.Update(gameTime);
				AIC.Update(gameTime);
			}
			if (!GamePad.GetState(ThePlayer).IsConnected)
			{
				iGameState = 2;
			}
			if (!mMainShipSprite.GetAlive() && mMainShipSprite.GetLives() == 0)
			{
				iGameState = 3;
				if (!bGOSoundPlayed)
				{
					SoundGameOver();
					bGOSoundPlayed = true;
					restartDelay = 120;
				}
			}
			else
			{
				if (!(AIC.GetEnemiesLeft() <= 0.0))
				{
					break;
				}
				AIC.bWaveEnded = true;
				if (!theUpgradeMenu.GetActive())
				{
					mMainShipSprite.immunityTimer = 360;
					theUpgradeMenu.SetActive(num: true);
					break;
				}
				theUpgradeMenu.Update(gameTime);
				if (!theUpgradeMenu.GetActive())
				{
					Barricade1.Restore();
					Barricade2.Restore();
					Barricade3.Restore();
					Barricade4.Restore();
					AIC.NextWave();
				}
			}
			break;
		case 2:
			if (confirmQuit)
			{
				if (GamePad.GetState(ThePlayer).Buttons.A == ButtonState.Pressed)
				{
					Initialize();
				}
				if (GamePad.GetState(ThePlayer).Buttons.B == ButtonState.Pressed && oldGamePadState.Buttons.B != ButtonState.Pressed)
				{
					confirmQuit = false;
					quitCanceled = true;
					SoundUpgradeMove();
				}
			}
			else if (GamePad.GetState(ThePlayer).Buttons.A == ButtonState.Pressed)
			{
				iGameState = 1;
			}
			if (GamePad.GetState(ThePlayer).Buttons.B == ButtonState.Pressed && oldGamePadState.Buttons.B != ButtonState.Pressed)
			{
				SoundUpgradeSelect();
				if (!quitCanceled)
				{
					confirmQuit = true;
				}
			}
			quitCanceled = false;
			break;
		case 3:
			if (restartDelay <= 0)
			{
				if (GamePad.GetState(ThePlayer).Buttons.A == ButtonState.Pressed && oldGamePadState.Buttons.A != ButtonState.Pressed)
				{
					Initialize();
				}
				if (keyBoardState.IsKeyDown(Keys.Space) && !oldKeyBoardState.IsKeyDown(Keys.Space))
				{
					Initialize();
				}
			}
			else
			{
				restartDelay--;
			}
			break;
		}
		PauseGame(keyBoardState, GamePad.GetState(ThePlayer));
		p1oldGamePadState = GamePad.GetState(PlayerIndex.One);
		p2oldGamePadState = GamePad.GetState(PlayerIndex.Two);
		p3oldGamePadState = GamePad.GetState(PlayerIndex.Three);
		p4oldGamePadState = GamePad.GetState(PlayerIndex.Four);
		base.Update(gameTime);
	}

	private void PauseGame(KeyboardState currentState, GamePadState gamePadState)
	{
		if (currentState.IsKeyDown(Keys.P) && !oldKeyBoardState.IsKeyDown(Keys.P))
		{
			if (iGameState == 1)
			{
				iGameState = 2;
			}
			else if (iGameState == 2)
			{
				iGameState = 1;
				confirmQuit = false;
			}
		}
		if (GamePad.GetState(ThePlayer).Buttons.Start == ButtonState.Pressed && oldGamePadState.Buttons.Start != ButtonState.Pressed)
		{
			if (iGameState == 1)
			{
				iGameState = 2;
			}
			else if (iGameState == 2)
			{
				iGameState = 1;
				confirmQuit = false;
			}
		}
	}

	private void DrawText()
	{
		spriteBatch.DrawString(font, "Score: " + mMainShipSprite.GetTotalPoints(), new Vector2(388f, 16f), Color.Gray);
		spriteBatch.DrawString(font, "Score: " + mMainShipSprite.GetTotalPoints(), new Vector2(390f, 18f), Color.White);
		spriteBatch.DrawString(font, "Lives: " + mMainShipSprite.GetLives(), new Vector2(388f, 672f), Color.Gray);
		spriteBatch.DrawString(font, "Lives: " + mMainShipSprite.GetLives(), new Vector2(390f, 674f), Color.White);
		spriteBatch.DrawString(font, "Wave: " + AIC.GetWaveNumber(), new Vector2(756f, 672f), Color.Gray);
		spriteBatch.DrawString(font, "Wave: " + AIC.GetWaveNumber(), new Vector2(758f, 674f), Color.White);
		switch (iGameState)
		{
		case 0:
			if (!confirmQuit)
			{
				spriteBatch.DrawString(font, "8-BIT DEFENSE", new Vector2(532f, 210f), Color.Gray);
				spriteBatch.DrawString(font, "8-BIT DEFENSE", new Vector2(534f, 212f), Color.White);
				spriteBatch.DrawString(font, "PRESS (A) TO PLAY", new Vector2(504f, 464f), Color.Gray);
				spriteBatch.DrawString(font, "PRESS (A) TO PLAY", new Vector2(506f, 466f), Color.White);
				spriteBatch.DrawString(font, "PRESS (B) TO QUIT", new Vector2(504f, 500f), Color.Gray);
				spriteBatch.DrawString(font, "PRESS (B) TO QUIT", new Vector2(506f, 502f), Color.White);
				spriteBatch.DrawString(font, "GRUNT", new Vector2(466f, 370f), Color.Gray);
				spriteBatch.DrawString(font, "GRUNT", new Vector2(468f, 372f), Color.White);
				spriteBatch.DrawString(font, "GUNNER", new Vector2(584f, 370f), Color.Gray);
				spriteBatch.DrawString(font, "GUNNER", new Vector2(586f, 372f), Color.White);
				spriteBatch.DrawString(font, "GUARD", new Vector2(719f, 370f), Color.Gray);
				spriteBatch.DrawString(font, "GUARD", new Vector2(721f, 372f), Color.White);
			}
			else
			{
				spriteBatch.DrawString(font, "Are you sure you want to quit?", new Vector2(386f, 360f), Color.Gray);
				spriteBatch.DrawString(font, "Are you sure you want to quit?", new Vector2(388f, 362f), Color.White);
				spriteBatch.DrawString(font, "(A) = QUIT - (B) = CANCEL", new Vector2(468f, 424f), Color.Gray);
				spriteBatch.DrawString(font, "(A) = QUIT - (B) = CANCEL", new Vector2(470f, 426f), Color.White);
			}
			break;
		case 1:
			break;
		case 2:
			if (!confirmQuit)
			{
				spriteBatch.DrawString(font, "PAUSED", new Vector2(580f, 360f), Color.Gray);
				spriteBatch.DrawString(font, "PAUSED", new Vector2(584f, 364f), Color.Gray);
				spriteBatch.DrawString(font, "PAUSED", new Vector2(582f, 362f), Color.White);
				spriteBatch.DrawString(font, "(A) = Resume - (B) = Quit", new Vector2(464f, 424f), Color.Gray);
				spriteBatch.DrawString(font, "(A) = Resume - (B) = Quit", new Vector2(468f, 428f), Color.Gray);
				spriteBatch.DrawString(font, "(A) = Resume - (B) = Quit", new Vector2(466f, 426f), Color.White);
			}
			else
			{
				spriteBatch.DrawString(font, "Are you sure you want to quit?", new Vector2(386f, 360f), Color.Gray);
				spriteBatch.DrawString(font, "Are you sure you want to quit?", new Vector2(390f, 364f), Color.Gray);
				spriteBatch.DrawString(font, "Are you sure you want to quit?", new Vector2(388f, 362f), Color.White);
				spriteBatch.DrawString(font, "(A) = QUIT - (B) = CANCEL", new Vector2(468f, 424f), Color.Gray);
				spriteBatch.DrawString(font, "(A) = QUIT - (B) = CANCEL", new Vector2(472f, 428f), Color.Gray);
				spriteBatch.DrawString(font, "(A) = QUIT - (B) = CANCEL", new Vector2(470f, 426f), Color.White);
			}
			break;
		case 3:
			spriteBatch.DrawString(font, "GAME OVER", new Vector2(556f, 360f), Color.Gray);
			spriteBatch.DrawString(font, "GAME OVER", new Vector2(560f, 364f), Color.Gray);
			spriteBatch.DrawString(font, "GAME OVER", new Vector2(558f, 362f), Color.White);
			break;
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin();
		gameBackground.Draw(spriteBatch);
		Barricade1.Draw(spriteBatch);
		Barricade2.Draw(spriteBatch);
		Barricade3.Draw(spriteBatch);
		Barricade4.Draw(spriteBatch);
		AIC.Draw(spriteBatch);
		mMainShipSprite.Draw(spriteBatch);
		if (theUpgradeMenu.GetActive())
		{
			theUpgradeMenu.Draw(spriteBatch);
		}
		DrawText();
		spriteBatch.End();
		base.Draw(gameTime);
	}

	public void laser()
	{
		sndLaser.Play();
	}

	public void SoundEnemyLaser()
	{
		sndEnemyLaser.Play();
	}

	public void SoundEnemyMove()
	{
		sndEnemyMove.Play();
	}

	public void SoundImpact()
	{
		sndImpact.Play();
	}

	public void SoundEnemyDestroy()
	{
		sndEnemyDestroy.Play();
	}

	public void SoundExtraLife()
	{
		sndExtraLife.Play();
	}

	public void SoundGameOver()
	{
		sndGameOver.Play();
	}

	public void SoundPlayerDeath()
	{
		sndPlayerDeath.Play();
	}

	public void SoundUpgradeSelect()
	{
		sndUpgradeSelect.Play();
	}

	public void SoundUpgradeMove()
	{
		sndUpgradeSound.Play();
	}

	public void SoundStartSound()
	{
		sndStartSound.Play();
	}
}
