using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlatformerFromHell.Asset_Classes;

namespace PlatformerFromHell;

internal class Player
{
	private const float MaxMoveSpeed = 1750f;

	private const float AirDragFactor = 0.58f;

	public const float MaxFallSpeed = 550f;

	private const float MoveStickScale = 1f;

	private const float AccelerometerScale = 1.5f;

	private const Buttons JumpButton = Buttons.A;

	private const float JumpLaunchVelocity = -3500f;

	private Vector2 poofOffset = new Vector2(20f, 32f);

	private Vector2 poofOffsetForFlip = new Vector2(5f, 32f);

	private Vector2 timedVelocity = default(Vector2);

	private Vector2 nextPosition = default(Vector2);

	private Vector2 positionToDraw = default(Vector2);

	private Vector2 spriteAdjustment = new Vector2(16f, 32f);

	public GameTime playerGameTime;

	private float previousElapsed = 42f;

	private float gameplayTime;

	public string killer;

	public long vibrationDeath = 500L;

	private float jump1Volume = 0.4f;

	private float jump2Volume = 0.4f;

	private float gruntDeathVolume = 1f;

	private float restartVolume = 1f;

	private float platformSwitchVolume = 0.5f;

	private float spikeDeathVolume = 1f;

	private float lavaDeathVolume = 1f;

	private float runningVolume = 1f;

	private float walkingVolume = 1f;

	private float gravityWarpVolume = 0.7f;

	private float pausedSoundVolume = 0.6f;

	private float victoryDanceVolume = 0.6f;

	private float sludgeDeathVolume = 1f;

	private float electricDeathVolume = 0.5f;

	public bool moving;

	public float movingTimer;

	public SoundEffect jump1;

	public SoundEffectInstance jump1Instance;

	public SoundEffect jump2;

	public SoundEffectInstance jump2Instance;

	public SoundEffect gruntDeath;

	public SoundEffectInstance gruntDeathInstance;

	public SoundEffect restart;

	public SoundEffectInstance restartInstance;

	public SoundEffect platformSwitch;

	public SoundEffectInstance platformSwitchInstance;

	public SoundEffect spikeDeath;

	public SoundEffectInstance spikeDeathInstance;

	public SoundEffect lavaDeath;

	public SoundEffectInstance lavaDeathInstance;

	public SoundEffect running;

	public SoundEffectInstance runningInstance;

	public SoundEffect walking;

	public SoundEffectInstance walkingInstance;

	public SoundEffect gravityWarp;

	public SoundEffectInstance gravityWarpInstance;

	public SoundEffect pausedSound;

	public SoundEffectInstance pausedSoundInstance;

	public SoundEffect victoryDance;

	public SoundEffectInstance victoryDanceInstance;

	public SoundEffect sludgeDeath;

	public SoundEffectInstance sludgeDeathInstance;

	public SoundEffect electricDeath;

	public SoundEffectInstance electricDeathInstance;

	public Animation redSwitch;

	public Animation normalSwitch;

	public Animation burnAnimation;

	public Animation idleAnimation;

	public Animation jumpAnimation;

	public Animation runAnimation;

	public Animation slicedAnimation;

	public Animation spikedAnimation;

	public Animation danceAnimation;

	public Animation headshotAnimation;

	public Animation turnAnimation;

	public Animation downFlip;

	public Animation upFlip;

	public Animation sludgeDeathAnimation;

	public Animation blueLavaDeathAnimation;

	public Animation World4Burn;

	public Animation World5Burn;

	public Animation electricDeathAnimation;

	public Animation freezeAnimation;

	public Animation blackholeDeathAnimation;

	public Animation blackLavaDeathAnimation;

	public Animation purpleLavaDeathAnimation;

	private SpriteEffects flip = SpriteEffects.None;

	private AnimationPlayer sprite;

	private Vector2 poofposition = default(Vector2);

	private AnimationPlayer poofsprite;

	private Animation poofAnimation;

	public float gravityTimer;

	public string gravityFlipType;

	public Gravity.GravDir currentGravity;

	public Gravity.GravDir pastGravity;

	public Gravity.GravDir jumpDirection;

	public float sprintStart;

	private bool isSprinting;

	public bool isMoving;

	private bool wasMoving;

	public float turnTimer;

	public float startTimer = 51f;

	public bool leftSlide;

	public bool rightSlide;

	public bool downSlide;

	public bool upSlide;

	public int jumpSoundNumber = 0;

	public Texture2D personTexture;

	public Color[] personTextureData;

	public Rectangle blockRectangle;

	public Rectangle previousRectangle;

	public KeyboardState previousKeyboardState;

	public bool restartPressed;

	public bool skipPressed = false;

	public bool switchesEnabled = true;

	public Level level;

	public bool isAlive;

	public Vector2 position;

	public Vector2 lastPosition;

	public Vector2 velocity;

	public Vector2 previousPosition;

	public float MoveAcceleration = 13000f;

	private float GroundDragFactor = 0.48f;

	public float MaxJumpTime = 0.17f;

	public float GravityAcceleration = 3400f;

	public bool isOnGround;

	private float movement;

	public bool isJumping;

	public bool wasJumping;

	public float jumpTime = 0f;

	public double timeTillSwitch;

	public float elapsed;

	private bool isSprintJump;

	private float JumpControlPower = 0.05f;

	public float mostRecentY = 0f;

	public Level Level => level;

	public bool IsAlive => isAlive;

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public Vector2 Velocity
	{
		get
		{
			return velocity;
		}
		set
		{
			velocity = value;
		}
	}

	public bool IsOnGround => isOnGround;

	public Rectangle personRectangle => new Rectangle((int)position.X, (int)position.Y, personTexture.Width, personTexture.Height);

	public Player(Level level, Vector2 position)
	{
		this.level = level;
		LoadContent();
		startTimer = 0f;
		Reset(position);
	}

	public void LoadContent()
	{
		normalSwitch = new Animation(Level.Content.Load<Texture2D>("Sprites/Assets/world" + Level.worldNumber + "/switch_world" + Level.worldNumber), 0.1f, isLooping: true);
		redSwitch = new Animation(Level.Content.Load<Texture2D>("Sprites/Assets/switch_red"), 0.1f, isLooping: true);
		personTexture = Level.Content.Load<Texture2D>("Sprites/Player/Person");
		personTextureData = new Color[personTexture.Width * personTexture.Height];
		personTexture.GetData(personTextureData);
		burnAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Burn"), 0.1f, isLooping: false);
		World4Burn = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/World4Burn"), 0.1f, isLooping: false);
		World5Burn = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/World5Burn"), 0.1f, isLooping: false);
		purpleLavaDeathAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/blackLavaDeath"), 0.1f, isLooping: false);
		blackLavaDeathAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/blackLavaDeath"), 0.1f, isLooping: false);
		blackholeDeathAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/blackholeDeath"), 0.1f, isLooping: false);
		idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.2f, isLooping: true);
		jumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Jump"), 0.1f, isLooping: false);
		runAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Run"), 0.1f, isLooping: true);
		slicedAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Sliced"), 0.1f, isLooping: false);
		danceAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Dance"), 0.1f, isLooping: true);
		headshotAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Headshot"), 0.1f, isLooping: false);
		turnAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Turn"), 0.1f, isLooping: true);
		spikedAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/Player/Sliced"), 0.1f, isLooping: false);
		sludgeDeathAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/Player/sludgeDeath"), 0.1f, isLooping: false);
		freezeAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/Player/freeze"), 0.1f, isLooping: false);
		blueLavaDeathAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/Player/blueLavaDeath"), 0.1f, isLooping: false);
		electricDeathAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/Player/electricDeath"), 0.1f, isLooping: false);
		downFlip = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/downFlipped"), 0.1f, isLooping: false);
		upFlip = new Animation(Level.Content.Load<Texture2D>("SPrites/Player/upFlipped"), 0.1f, isLooping: false);
		poofAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/poof"), 0.2f, isLooping: true);
		gruntDeath = Level.Content.Load<SoundEffect>("Sounds/gruntDeath");
		platformSwitch = Level.Content.Load<SoundEffect>("Sounds/platformSwitch");
		running = Level.Content.Load<SoundEffect>("Sounds/running");
		walking = Level.Content.Load<SoundEffect>("Sounds/walking");
		lavaDeath = Level.Content.Load<SoundEffect>("Sounds/lava");
		gravityWarp = Level.Content.Load<SoundEffect>("Sounds/gravity warp");
		restart = Level.Content.Load<SoundEffect>("Sounds/restart");
		spikeDeath = Level.Content.Load<SoundEffect>("Sounds/spikeDeath");
		pausedSound = Level.Content.Load<SoundEffect>("Sounds/Pause Sound");
		jump1 = Level.Content.Load<SoundEffect>("Sounds/Jump Part 1");
		jump2 = Level.Content.Load<SoundEffect>("Sounds/Jump Part 2");
		victoryDance = Level.Content.Load<SoundEffect>("Sounds/victoryDance");
		sludgeDeath = Level.Content.Load<SoundEffect>("Sounds/sludgeDeath");
		electricDeath = Level.Content.Load<SoundEffect>("Sounds/electricDeath");
		gruntDeathInstance = gruntDeath.CreateInstance();
		platformSwitchInstance = platformSwitch.CreateInstance();
		runningInstance = running.CreateInstance();
		walkingInstance = walking.CreateInstance();
		lavaDeathInstance = lavaDeath.CreateInstance();
		gravityWarpInstance = gravityWarp.CreateInstance();
		restartInstance = restart.CreateInstance();
		spikeDeathInstance = spikeDeath.CreateInstance();
		pausedSoundInstance = pausedSound.CreateInstance();
		jump1Instance = jump1.CreateInstance();
		jump2Instance = jump2.CreateInstance();
		victoryDanceInstance = victoryDance.CreateInstance();
		sludgeDeathInstance = sludgeDeath.CreateInstance();
		electricDeathInstance = electricDeath.CreateInstance();
		gruntDeathInstance.Volume = gruntDeathVolume;
		platformSwitchInstance.Volume = platformSwitchVolume;
		runningInstance.Volume = runningVolume;
		walkingInstance.Volume = walkingVolume;
		lavaDeathInstance.Volume = lavaDeathVolume;
		gravityWarpInstance.Volume = gravityWarpVolume;
		restartInstance.Volume = restartVolume;
		spikeDeathInstance.Volume = spikeDeathVolume;
		pausedSoundInstance.Volume = pausedSoundVolume;
		jump1Instance.Volume = jump1Volume;
		jump2Instance.Volume = jump2Volume;
		victoryDanceInstance.Volume = victoryDanceVolume;
		sludgeDeathInstance.Volume = sludgeDeathVolume;
		electricDeathInstance.Volume = electricDeathVolume;
	}

	public void Reset(Vector2 position)
	{
		Position = position;
		Console.Out.WriteLine("Reset called with " + position);
		Velocity = Vector2.Zero;
		isAlive = true;
		sprite.PlayAnimation(idleAnimation);
		level.moneyGrabbed = false;
		foreach (Asset asset in level.assets)
		{
			asset.disabled = false;
			if (asset is Switch)
			{
				asset.fullTexture = normalSwitch.Texture;
			}
		}
	}

	public void Update(GameTime gameTime, KeyboardState keyboardState, GamePadState gamePadState)
	{
		playerGameTime = gameTime;
		GetInput(keyboardState, gamePadState);
		if (skipPressed)
		{
			Program.game.gotMoney();
			level.ReachedExit = true;
		}
		if (restartPressed)
		{
			OnKilled("restart");
		}
		if (timeTillSwitch > 0.0)
		{
			timeTillSwitch -= (float)gameTime.ElapsedGameTime.TotalSeconds * 60f;
		}
		else if (!switchesEnabled)
		{
			foreach (Asset asset in level.assets)
			{
				if (asset is Switch)
				{
					asset.fullTexture = normalSwitch.Texture;
				}
			}
			switchesEnabled = true;
		}
		if (isMoving && !wasMoving)
		{
			sprintStart = (float)gameTime.TotalGameTime.TotalSeconds;
		}
		if (startTimer <= 50f)
		{
			startTimer += (float)gameTime.ElapsedGameTime.TotalSeconds * 60f;
		}
		if (IsOnGround)
		{
			if (Math.Abs(Velocity.X) - 0.02f > 0f)
			{
				sprite.PlayAnimation(runAnimation);
			}
			else
			{
				sprite.PlayAnimation(idleAnimation);
			}
		}
		if (startTimer > 50f)
		{
			NewApplyPhysics(gameTime, gamePadState, keyboardState);
		}
		movement = 0f;
		isJumping = false;
		wasMoving = isMoving;
	}

	private void GetInput(KeyboardState keyboardState, GamePadState gamePadState)
	{
		if (keyboardState.IsKeyDown(Keys.Escape) && !previousKeyboardState.IsKeyDown(Keys.Escape))
		{
			pausedSoundInstance.Play();
		}
		movement = gamePadState.ThumbSticks.Left.X * 1f;
		if (Math.Abs(movement) < 0.5f)
		{
			movement = 0f;
		}
		if (startTimer > 50f)
		{
			if (gamePadState.IsButtonDown(Buttons.DPadLeft) || (keyboardState.IsKeyDown(Keys.Left) && keyboardState.IsKeyUp(Keys.Right)))
			{
				movement = -1f;
				if (!isMoving)
				{
					if (walkingInstance.State == SoundState.Stopped && velocity.X > 0f)
					{
						walkingInstance.Play();
					}
				}
				else if (runningInstance.State == SoundState.Stopped && velocity.X > 0f)
				{
					runningInstance.Play();
				}
			}
			else if (gamePadState.IsButtonDown(Buttons.DPadRight) || (keyboardState.IsKeyDown(Keys.Right) && keyboardState.IsKeyUp(Keys.Left)))
			{
				movement = 1f;
				if (!isSprinting)
				{
					if (walkingInstance.State == SoundState.Stopped && velocity.X > 0f)
					{
						walkingInstance.Play();
					}
				}
				else if (runningInstance.State == SoundState.Stopped && velocity.X > 0f)
				{
					runningInstance.Play();
				}
			}
		}
		if (keyboardState.IsKeyDown(Keys.K) || gamePadState.IsButtonDown(Buttons.Y))
		{
			restartPressed = true;
		}
		else
		{
			restartPressed = false;
		}
		bool flag = true;
		if ((keyboardState.IsKeyDown(Keys.A) && keyboardState.IsKeyUp(Keys.D)) || gamePadState.IsButtonDown(Buttons.RightThumbstickLeft))
		{
			leftSlide = true;
		}
		else
		{
			leftSlide = false;
		}
		if ((keyboardState.IsKeyDown(Keys.D) && keyboardState.IsKeyUp(Keys.A)) || gamePadState.IsButtonDown(Buttons.RightThumbstickRight))
		{
			rightSlide = true;
		}
		else
		{
			rightSlide = false;
		}
		if ((keyboardState.IsKeyDown(Keys.W) && keyboardState.IsKeyUp(Keys.S)) || gamePadState.IsButtonDown(Buttons.RightThumbstickUp))
		{
			upSlide = true;
		}
		else
		{
			upSlide = false;
		}
		if ((keyboardState.IsKeyDown(Keys.S) && keyboardState.IsKeyUp(Keys.W)) || gamePadState.IsButtonDown(Buttons.RightThumbstickDown))
		{
			downSlide = true;
		}
		else
		{
			downSlide = false;
		}
		isJumping = gamePadState.IsButtonDown(Buttons.A) || keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.Up);
		isMoving = gamePadState.IsButtonDown(Buttons.DPadLeft) || (keyboardState.IsKeyDown(Keys.Left) && keyboardState.IsKeyUp(Keys.Right)) || gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft) || gamePadState.IsButtonDown(Buttons.DPadRight) || gamePadState.IsButtonDown(Buttons.LeftThumbstickRight) || (keyboardState.IsKeyDown(Keys.Right) && keyboardState.IsKeyUp(Keys.Left));
		isSprinting = gamePadState.IsButtonDown(Buttons.X) || keyboardState.IsKeyDown(Keys.LeftShift) || gamePadState.IsButtonDown(Buttons.RightTrigger);
		if (position != lastPosition)
		{
			moving = true;
		}
		else if (movingTimer > 0f)
		{
			movingTimer -= (float)playerGameTime.ElapsedGameTime.TotalSeconds * 60f;
			if (movingTimer <= 0f)
			{
				movingTimer = 0f;
			}
			moving = true;
		}
		else
		{
			moving = false;
		}
		previousKeyboardState = keyboardState;
		lastPosition = position;
	}

	public void ApplyPhysics(GameTime gameTime, GamePadState gamePadState, KeyboardState keyBoardState)
	{
		NewApplyPhysics(gameTime, gamePadState, keyBoardState);
	}

	public void NewApplyPhysics(GameTime gameTime, GamePadState gamePadState, KeyboardState keyboardState)
	{
		elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (elapsed >= previousElapsed * 1.5f)
		{
			elapsed /= 1.5f;
		}
		previousElapsed = elapsed;
		gameplayTime = (float)gameTime.TotalGameTime.TotalSeconds;
		previousPosition = position;
		pastGravity = currentGravity;
		currentGravity = CollisionManager.GetGravity(this);
		isOnGround = CollisionManager.getIsOnGround(this, currentGravity);
		GroundDragFactor = 0.46f;
		velocity.X += movement * MoveAcceleration * elapsed;
		velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -550f, 550f);
		if (isOnGround)
		{
			velocity.Y = 0f;
		}
		velocity.Y = DoJump(velocity.Y, gameTime);
		if (IsOnGround)
		{
			velocity.X *= GroundDragFactor;
		}
		else
		{
			velocity.X *= 0.58f;
		}
		velocity.X = MathHelper.Clamp(velocity.X, -1750f, 1750f);
		if (currentGravity == Gravity.GravDir.Up)
		{
			flip |= SpriteEffects.FlipVertically;
			GravityAcceleration = -3400f;
		}
		else
		{
			flip &= ~SpriteEffects.FlipVertically;
			GravityAcceleration = 3400f;
		}
		Vector2.Multiply(ref velocity, elapsed, out timedVelocity);
		Vector2.Add(ref position, ref timedVelocity, out position);
		position.X = (float)Math.Round(Position.X);
		position.Y = (float)Math.Round(Position.Y);
		position.X = MathHelper.Clamp(position.X, 0f, level.levelWidth - 20);
		position.Y = MathHelper.Clamp(position.Y, -31f, level.levelHeight);
		previousRectangle.X = (int)previousPosition.X;
		previousRectangle.Y = (int)previousPosition.Y;
		previousRectangle.Width = personTexture.Width;
		previousRectangle.Height = personTexture.Height;
		if (Velocity.Y != 0f)
		{
			mostRecentY = Velocity.Y;
		}
		CollisionManager.HandleCollisions(this, currentGravity);
	}

	private float DoJump(float velocityY, GameTime gameTime)
	{
		return NewDoJump(velocityY, gameTime);
	}

	private float OldDoJump(float velocityY, GameTime gameTime)
	{
		if (isJumping)
		{
			if ((!wasJumping && IsOnGround) || jumpTime > 0f)
			{
				if (jumpSoundNumber > 1)
				{
					jumpSoundNumber = 0;
				}
				if (jumpTime == 0f)
				{
					jumpDirection = currentGravity;
					if (jumpSoundNumber == 0)
					{
						jump1.Play();
					}
					else if (jumpSoundNumber == 1)
					{
						jump2.Play();
					}
					jumpSoundNumber++;
				}
				jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
				sprite.PlayAnimation(jumpAnimation);
			}
			if (0f < jumpTime && jumpTime <= MaxJumpTime)
			{
				velocityY = -3500f * (1f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
				if (jumpDirection == Gravity.GravDir.Up)
				{
					velocityY = -1f * velocityY;
				}
			}
			if (isSprinting)
			{
				MaxJumpTime = 0.34f;
			}
			else
			{
				MaxJumpTime = 0.34f;
			}
		}
		else
		{
			jumpTime = 0f;
		}
		wasJumping = isJumping;
		return velocityY;
	}

	private float NewDoJump(float velocityY, GameTime gameTime)
	{
		if (isJumping && !wasJumping && IsOnGround && jumpTime <= 0f)
		{
			jumpTime = 0f;
			jumpDirection = currentGravity;
			isSprintJump = isSprinting;
			if (isSprintJump)
			{
				MaxJumpTime = 0.225f;
				JumpControlPower = 0.06f;
			}
			else
			{
				MaxJumpTime = 0.225f;
				JumpControlPower = 0.06f;
			}
			if (jumpSoundNumber == 0)
			{
				jump1.Play();
			}
			else
			{
				jump2.Play();
			}
			jumpSoundNumber = (jumpSoundNumber + 1) % 2;
			jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
			sprite.PlayAnimation(jumpAnimation);
		}
		float num = JumpControlPower;
		float num2 = MaxJumpTime;
		if (isJumping && 0f < jumpTime && jumpTime <= MaxJumpTime)
		{
			float num3 = 0.25f;
			if (jumpTime < num3)
			{
				num += 0.1f;
				num2 += jumpTime;
			}
		}
		if (0f < jumpTime && jumpTime < MaxJumpTime)
		{
			jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
			sprite.PlayAnimation(jumpAnimation);
			velocityY = -3500f * (1f - (float)Math.Pow(jumpTime / num2, num));
			if (jumpDirection == Gravity.GravDir.Up)
			{
				velocityY = -1f * velocityY;
			}
		}
		if (jumpTime > MaxJumpTime)
		{
			jumpTime = 0f;
		}
		wasJumping = jumpTime > 0f || isJumping;
		return velocityY;
	}

	public void Dispose()
	{
	}

	private static bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
	{
		int num = Math.Max(rectangleA.Top, rectangleB.Top);
		int num2 = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
		int num3 = Math.Max(rectangleA.Left, rectangleB.Left);
		int num4 = Math.Min(rectangleA.Right, rectangleB.Right);
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				Color color = dataA[j - rectangleA.Left + (i - rectangleA.Top) * rectangleA.Width];
				Color color2 = dataB[j - rectangleB.Left + (i - rectangleB.Top) * rectangleB.Width];
				if (color.A != 0 && color2.A != 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void flipPlatforms(char switchChar)
	{
		platformSwitchInstance.Play();
		foreach (Asset asset in level.assets)
		{
			if (asset is Platform && ((Platform)asset).GetSwitchID() == switchChar)
			{
				if (asset.GetFlip() < Asset.Dir.UpRight)
				{
					asset.ChangeFlip(2);
				}
				else
				{
					asset.ChangeFlip(-2);
				}
			}
			else if (asset is Switch)
			{
				asset.fullTexture = redSwitch.Texture;
				switchesEnabled = false;
			}
		}
	}

	public void OnKilled(string killer)
	{
		OnKilled(killer, fromBelow: false);
	}

	public void OnKilled(string killer, bool fromBelow)
	{
		PlatformerGame.StartVibration(vibrationDeath);
		isAlive = false;
		this.killer = killer;
		level.deaths++;
		if (killer.Contains("lava_world1") || killer.Contains("world1_fireball"))
		{
			position.Y += 35f;
			sprite.PlayAnimation(burnAnimation);
			lavaDeathInstance.Play();
		}
		else if (killer.Contains("lava_world4") || killer.Contains("world4_fireball"))
		{
			position.Y += 35f;
			sprite.PlayAnimation(World4Burn);
			lavaDeathInstance.Play();
		}
		else if (killer.Contains("lava_world5"))
		{
			position.Y += 35f;
			sprite.PlayAnimation(blackholeDeathAnimation);
			lavaDeathInstance.Play();
		}
		else if (killer.Contains("world5_fireball"))
		{
			position.Y += 35f;
			sprite.PlayAnimation(blackholeDeathAnimation);
			lavaDeathInstance.Play();
		}
		else if (killer.Contains("lava_world2") || killer.Contains("world2_fireball"))
		{
			position.Y += 35f;
			sprite.PlayAnimation(sludgeDeathAnimation);
			lavaDeathInstance.Play();
		}
		else if (killer.Contains("lava_world3") || killer.Contains("world3_fireball"))
		{
			position = new Vector2(position.X - 16f, position.Y + 32f);
			sprite.PlayAnimation(freezeAnimation);
			sludgeDeathInstance.Play();
		}
		else if (killer.Contains("lava") || killer.Contains("fireball"))
		{
			position.Y += 35f;
			sprite.PlayAnimation(burnAnimation);
			lavaDeathInstance.Play();
		}
		else if (killer == "World 4 platform")
		{
			position.Y += 32f;
			sprite.PlayAnimation(electricDeathAnimation);
			electricDeathInstance.Play();
		}
		else if (killer == "restart")
		{
			if (gravityFlipType == "downFlip")
			{
				position.Y += 64f;
			}
			sprite.PlayAnimation(headshotAnimation);
			restartInstance.Play();
		}
		else
		{
			if (!fromBelow)
			{
				position.Y += 35f;
			}
			sprite.PlayAnimation(spikedAnimation);
			spikeDeathInstance.Play();
		}
		level.timeRemaining = TimeSpan.FromMinutes(5.0);
	}

	public void OnReachedExit()
	{
		sprite.PlayAnimation(danceAnimation);
		victoryDanceInstance.Play();
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		if (turnTimer <= 0f)
		{
			if (flip == SpriteEffects.None && Velocity.X < 0f && (double)GroundDragFactor >= 0.46)
			{
				turnTimer = 15f;
				Vector2.Add(ref position, ref poofOffset, out poofposition);
				flip |= SpriteEffects.FlipHorizontally;
			}
			else if (flip == SpriteEffects.FlipHorizontally && Velocity.X > 0f && (double)GroundDragFactor >= 0.46)
			{
				turnTimer = 15f;
				Vector2.Add(ref position, ref poofOffsetForFlip, out poofposition);
				flip &= ~SpriteEffects.FlipHorizontally;
			}
		}
		if (currentGravity == Gravity.GravDir.Up && pastGravity == Gravity.GravDir.Down)
		{
			gravityWarp.Play();
			gravityTimer = 20f;
			gravityFlipType = "downFlip";
		}
		if (currentGravity == Gravity.GravDir.Down && pastGravity == Gravity.GravDir.Up)
		{
			gravityWarp.Play();
			gravityTimer = 20f;
			gravityFlipType = "upFlip";
		}
		if (turnTimer <= 0f)
		{
			if (Velocity.X > 0f)
			{
				flip &= ~SpriteEffects.FlipHorizontally;
			}
			else if (Velocity.X < 0f)
			{
				flip |= SpriteEffects.FlipHorizontally;
			}
		}
		else
		{
			turnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds * 60f;
		}
		if (gravityTimer > 0f)
		{
			gravityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds * 60f;
		}
		if (isAlive)
		{
			if (turnTimer > 0f)
			{
				sprite.PlayAnimation(turnAnimation);
			}
			if (gravityTimer > 0f)
			{
				if (gravityFlipType == "upFlip")
				{
					sprite.PlayAnimation(upFlip);
				}
				else
				{
					sprite.PlayAnimation(downFlip);
				}
			}
		}
		else if (killer == "restart")
		{
			sprite.PlayAnimation(headshotAnimation);
		}
		Vector2.Add(ref position, ref spriteAdjustment, out positionToDraw);
		sprite.Draw(gameTime, spriteBatch, positionToDraw, flip);
		if (isAlive && turnTimer > 0f && isOnGround)
		{
			poofsprite.PlayAnimation(poofAnimation);
			poofsprite.Draw(gameTime, spriteBatch, poofposition, SpriteEffects.None);
		}
		pastGravity = currentGravity;
	}
}
