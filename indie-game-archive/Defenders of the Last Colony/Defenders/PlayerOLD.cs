using System;
using System.Collections.Generic;
using Hammer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Defenders;

internal class PlayerOLD
{
	public class Character
	{
		public string name;

		public string shipClass;

		public Color color;

		public string abilityType;

		public ushort relics = 0;

		public List<float> ability = new List<float>(4);
	}

	public Texture2D texture;

	private SpriteFont font;

	private Texture2D ui;

	private Texture2D uiHealth;

	private Texture2D uiBlue;

	private Texture2D txCircle;

	private Texture2D txBar;

	private uint frame = 0u;

	public Vector2 position;

	public Vector2 UIpos = Vector2.Zero;

	public Vector2 UIposText = Vector2.Zero;

	public float accelerationX;

	public float accelerationY;

	public float accelerationX2;

	public float accelerationY2;

	public Vector2 shootingThrottle = new Vector2(0f, 0f);

	public ushort missiles = 0;

	public bool used = false;

	public int taken = -1;

	private int oldDireccion;

	private int direction;

	private int menuCounter = 0;

	public float scale = 0.5f;

	public uint numberOfKills = 0u;

	private Rectangle card = Rectangle.Empty;

	private Primitive2D draw2d;

	private float[] pDist = new float[4];

	private float[] pAngle = new float[4];

	private Vector2[] points = new Vector2[4];

	private Vector2[] pRandom = new Vector2[4];

	public Character characters;

	public ushort SA;

	public int score;

	public int orbs = 0;

	public int maxOrbs = 0;

	public float speed;

	public float angle;

	public float shootRate;

	public float shootDamage;

	public string shootingType;

	public int shootingChangeCost;

	public int beingHit;

	public ushort level;

	public int experience;

	public int nextLevel;

	private ushort levelOld;

	public bool levelUpdated = false;

	private int levelMessage = 0;

	private float levelTransp = 0f;

	public int abilityTimer;

	private string[] abilityNames = new string[4];

	public float vibrationLeft = 0f;

	public float vibrationRight = 0f;

	private MouseState currentMouseState;

	private MouseState oldMouseState;

	private KeyboardState currentKeyboardState;

	private KeyboardState oldKeyboardState;

	private GamePadState currentGamePadState;

	private GamePadState oldGamePadState;

	public bool fighter;

	public float credits;

	public float creditsMul;

	public int maximunCredits;

	public float maximunHealth;

	public float Health;

	public float Destroyed;

	public int number;

	private float change = 1f;

	public PlayerIndex index;

	public bool ready;

	public bool selected;

	public bool wasActive;

	public bool Active;

	public float smoothness = 0f;

	private Random random = new Random();

	public int Width => texture.Width;

	public int Height => texture.Height;

	public float size()
	{
		return (float)(Width + Height) / 2f * scale;
	}

	private void createCharacters(int number)
	{
		orbs = 0;
		Character character = new Character();
		switch (number)
		{
		case 0:
		{
			Color lightCyan = Color.LightCyan;
			character = new Character();
			character.name = "Mark R.";
			character.shipClass = "Fighter";
			character.color = lightCyan;
			character.relics = 0;
			character.abilityType = "Hell Storm";
			characters = character;
			break;
		}
		case 1:
		{
			Color lightCyan = new Color(1f, 0.37f, 0f);
			character = new Character();
			character.name = "Michelle W.";
			character.shipClass = "Defender";
			character.color = lightCyan;
			character.relics = 0;
			character.abilityType = "Sonic Bomb";
			characters = character;
			break;
		}
		case 2:
		{
			Color lightCyan = new Color(0f, 0.86f, 0.01f);
			character = new Character();
			character.name = "James R.";
			character.shipClass = "Fighter";
			character.color = lightCyan;
			character.relics = 0;
			character.abilityType = "EMP";
			characters = character;
			break;
		}
		case 3:
		{
			Color lightCyan = new Color(0.86f, 0f, 0.84f);
			character = new Character();
			character.name = "Johnny V.";
			character.shipClass = "Defender";
			character.color = lightCyan;
			character.relics = 0;
			character.abilityType = "Laser Blades";
			characters = character;
			break;
		}
		default:
		{
			Color lightCyan = Color.LightCyan;
			break;
		}
		}
	}

	public PlayerOLD(int number)
	{
		createCharacters(number);
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				characters.ability.Add(0f);
			}
		}
	}

	public PlayerOLD(GraphicsDevice GraphicsDevice, Texture2D texture, SpriteFont gameFont, int number, bool fighter, PlayerIndex index, Texture2D ui, Texture2D uiHealth, Texture2D uiBlue, Texture2D txCircle, Texture2D txBar)
	{
		Initialize(GraphicsDevice, texture, gameFont, number, fighter, index, ui, uiHealth, uiBlue, txCircle, txBar);
	}

	public void Initialize(GraphicsDevice GraphicsDevice, Texture2D texture, SpriteFont gameFont, int number, bool fighter, PlayerIndex index, Texture2D ui, Texture2D uiHealth, Texture2D uiBlue, Texture2D txCircle, Texture2D txBar)
	{
		Primitive2D primitive2D = new Primitive2D(GraphicsDevice);
		this.texture = texture;
		font = gameFont;
		this.fighter = fighter;
		this.number = number;
		this.index = index;
		this.ui = ui;
		this.uiHealth = uiHealth;
		this.uiBlue = uiBlue;
		this.txCircle = txCircle;
		this.txBar = txBar;
		score = 0;
		this.number = number;
		createCharacters(number);
		creditsMul = 1f;
		orbs = 0;
		CalculateVectorial();
		draw2d = new Primitive2D(GraphicsDevice);
		position = new Vector2(random.Next(200, 400), random.Next(100, 300));
		SA = 1;
		accelerationX = 0f;
		accelerationY = 0f;
		accelerationX2 = 0f;
		accelerationY2 = 0f;
		level = 0;
		levelOld = 0;
		beingHit = 0;
		angle = 0f;
		shootRate = 15f;
		shootDamage = 1f;
		shootingType = "NORMAL";
		shootingChangeCost = 10;
		maximunHealth = 4f * (2f - Game1.difficulty);
		speed = 5f;
		credits = 0f;
		experience = 0;
		nextLevel = 750;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				characters.ability.Add(0f);
			}
		}
		if (characters.shipClass == "Defender")
		{
			abilityNames[0] = "  (A)  Shields";
			abilityNames[1] = "  (X)  Turrets";
			abilityNames[2] = "  (Y)  Hive";
			abilityNames[3] = "  (B)  Sanctuary";
		}
		else
		{
			abilityNames[0] = "  (A)  Shields";
			abilityNames[1] = "  (X)  Missiles";
			abilityNames[2] = "  (Y)  Linear Gun";
			abilityNames[3] = "  (B)  Double Shot";
		}
		if (characters.shipClass == "Fighter")
		{
			shootRate = 9f;
			shootDamage = 1f;
			maximunHealth = 3f;
			speed = 10f;
			credits = 0f;
			maximunCredits = 50;
			smoothness = 0.15f;
			creditsMul = 2f;
			maxOrbs = 1;
		}
		else
		{
			shootRate = 10f;
			shootDamage = 0.75f;
			maximunHealth = 5f;
			speed = 7f;
			credits = 10f;
			maximunCredits = 100;
			smoothness = 0.25f;
			creditsMul = 1f;
			maxOrbs = 3;
		}
		Health = maximunHealth;
		Reset();
	}

	public void AddRelic()
	{
		characters.relics++;
	}

	private void CalculateVectorial()
	{
		float num = 20f;
		for (int i = 0; i < pDist.Length; i++)
		{
			pDist[i] = num;
			pAngle[i] = 0f;
			ref Vector2 reference = ref points[i];
			reference = Vector2.Zero;
			if (Destroyed == 0f)
			{
				ref Vector2 reference2 = ref pRandom[i];
				reference2 = new Vector2((float)random.Next(-5, 5) / 100000f, (float)random.Next(-5, 5) / 100000f);
			}
		}
		pDist[2] = 0f;
		pAngle[1] = (float)Math.PI * 3f / 4f;
		pAngle[3] = 3.926991f;
		if (frame % 10 == 0)
		{
			for (int i = 0; i < pDist.Length; i++)
			{
				pDist[i] += Destroyed / 100000f;
				pAngle[i] += Destroyed / 100000f;
			}
		}
		for (int i = 0; i < points.Length; i++)
		{
			Vector2 vector = ((Destroyed != 0f) ? pRandom[i] : Vector2.Zero);
			float num2 = vector.X - vector.Y;
			float num3 = (vector.X + vector.Y) * 0.5f;
			ref Vector2 reference3 = ref points[i];
			reference3 = Math2.AdvanceAngle(vector, pAngle[i] + angle - num2, pDist[i] + num3);
		}
	}

	public void Reset()
	{
		Reset(new Vector2(random.Next(200, 400), random.Next(100, 300)));
	}

	public void Reset(Vector2 pos)
	{
		position = pos;
		score = 0;
		SA = 1;
		accelerationX = 0f;
		accelerationY = 0f;
		accelerationX2 = 0f;
		accelerationY2 = 0f;
		beingHit = 0;
		angle = 0f;
		orbs = 0;
		shootingType = "NORMAL";
		credits = maximunCredits / 4;
		Health = maximunHealth;
		Destroyed = 0f;
		if (characters.shipClass == "Fighter")
		{
			maxOrbs = 1;
		}
		else
		{
			maxOrbs = 3;
		}
	}

	public bool isMouseOn(Vector2 mousePos)
	{
		return new Rectangle((int)mousePos.X - 1, (int)mousePos.Y - 1, 2, 2).Intersects(card);
	}

	public Vector2 UpdateShooting(bool useKeyboardControls, Vector2 mousePos, PlayerIndex index)
	{
		MouseState state = Mouse.GetState();
		currentKeyboardState = Keyboard.GetState();
		currentGamePadState = GamePad.GetState(index);
		shootingThrottle = new Vector2(0f, 0f);
		if (currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed || currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
		{
			shootingThrottle.X = (float)Math.Cos(angle) * 100f;
			shootingThrottle.Y = (float)Math.Sin(angle) * -100f;
		}
		if (Math.Abs(currentGamePadState.ThumbSticks.Right.X) > 0.1f || Math.Abs(currentGamePadState.ThumbSticks.Right.Y) > 0.1f)
		{
			angle = (float)(Math.Atan2(currentGamePadState.ThumbSticks.Right.X, currentGamePadState.ThumbSticks.Right.Y) - 1.5707963705062866);
			shootingThrottle.X = currentGamePadState.ThumbSticks.Right.X;
			shootingThrottle.Y = currentGamePadState.ThumbSticks.Right.Y;
		}
		if (useKeyboardControls)
		{
			if (currentKeyboardState.IsKeyDown(Keys.Right))
			{
				shootingThrottle.X = 1f;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Left))
			{
				shootingThrottle.X = -1f;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Up))
			{
				shootingThrottle.Y = 1f;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down))
			{
				shootingThrottle.Y = -1f;
			}
			if (Math.Abs(shootingThrottle.X) > 0f || Math.Abs(shootingThrottle.Y) > 0f)
			{
				angle = (float)(Math.Atan2(shootingThrottle.X, shootingThrottle.Y) - 1.5707963705062866);
			}
			if (state.LeftButton == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.LeftAlt))
			{
				Vector2 vector = mousePos - position;
				angle = (float)Math.Atan2(vector.Y, vector.X);
				shootingThrottle.X = vector.Y;
				shootingThrottle.Y = vector.X;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Space) && Game1.gameState == GameState.Sidescroller)
			{
				angle = 0f;
				shootingThrottle = new Vector2(100f, 0f);
			}
			if (currentKeyboardState.IsKeyDown(Keys.Space) && Game1.gameState == GameState.ChubbyRain)
			{
				angle = -(float)Math.PI / 2f;
				shootingThrottle = new Vector2(0f, 100f);
			}
		}
		oldKeyboardState = currentKeyboardState;
		oldGamePadState = currentGamePadState;
		return shootingThrottle;
	}

	public bool UpdateSpecialAbility()
	{
		bool result = false;
		if (abilityTimer > 0)
		{
			abilityTimer--;
		}
		if (SA > 0 && abilityTimer <= 0 && ((double)currentGamePadState.Triggers.Right > 0.2 || currentKeyboardState.IsKeyDown(Keys.Space)))
		{
			SA--;
			abilityTimer = 100;
			result = true;
		}
		return result;
	}

	public string Update(int maxX, int maxY, int vibration, bool useKeyboardControls, PlayerIndex index)
	{
		if (!Active)
		{
			Destroyed = 100f;
		}
		string result = "";
		this.index = index;
		if (levelMessage > 0)
		{
			levelMessage--;
			levelTransp = MathHelper.Lerp(levelTransp, 1f, 0.01f);
		}
		else
		{
			levelTransp = MathHelper.Lerp(levelTransp, 0f, 0.01f);
		}
		if (missiles > 0)
		{
			missiles--;
		}
		accelerationX2 = 0f;
		accelerationY2 = 0f;
		currentKeyboardState = Keyboard.GetState();
		currentGamePadState = GamePad.GetState(index);
		MouseState state = Mouse.GetState();
		if ((double)Math.Abs(GamePad.GetState(index).ThumbSticks.Left.X) > 0.1)
		{
			accelerationX2 = GamePad.GetState(index).ThumbSticks.Left.X * speed;
		}
		if ((double)Math.Abs(GamePad.GetState(index).ThumbSticks.Left.Y) > 0.1)
		{
			accelerationY2 = GamePad.GetState(index).ThumbSticks.Left.Y * (0f - speed);
		}
		if (GamePad.GetState(index).DPad.Left == ButtonState.Pressed)
		{
			accelerationX2 = 0f - speed;
		}
		if (GamePad.GetState(index).DPad.Right == ButtonState.Pressed)
		{
			accelerationX2 = speed;
		}
		if (GamePad.GetState(index).DPad.Up == ButtonState.Pressed)
		{
			accelerationY2 = 0f - speed;
		}
		if (GamePad.GetState(index).DPad.Down == ButtonState.Pressed)
		{
			accelerationY2 = speed;
		}
		if (useKeyboardControls)
		{
			KeyboardState state2 = Keyboard.GetState();
			if (state2.IsKeyDown(Keys.A) && !state2.IsKeyDown(Keys.D))
			{
				accelerationX2 = 0f - speed;
			}
			if (state2.IsKeyDown(Keys.D) && !state2.IsKeyDown(Keys.A))
			{
				accelerationX2 = speed;
			}
			if (state2.IsKeyDown(Keys.W) && !state2.IsKeyDown(Keys.S))
			{
				accelerationY2 = 0f - speed;
			}
			if (state2.IsKeyDown(Keys.S) && !state2.IsKeyDown(Keys.W))
			{
				accelerationY2 = speed;
			}
		}
		if (beingHit > 195)
		{
			vibrationLeft = 1f * (float)vibration;
			vibrationRight = 1f * (float)vibration;
		}
		if (vibrationLeft > 0f)
		{
			vibrationLeft -= 0.05f;
		}
		if (vibrationRight > 0f)
		{
			vibrationRight -= 0.05f;
		}
		vibrationLeft = MathHelper.Clamp(vibrationLeft, 0f, 1f);
		vibrationRight = MathHelper.Clamp(vibrationRight, 0f, 1f);
		GamePad.SetVibration(index, vibrationLeft, vibrationRight);
		if (!Active)
		{
			accelerationX2 = 0f;
			accelerationY2 = 0f;
		}
		accelerationX = MathHelper.SmoothStep(accelerationX, accelerationX2, smoothness);
		accelerationY = MathHelper.SmoothStep(accelerationY, accelerationY2, smoothness);
		position.X = MathHelper.Clamp(position.X, (float)maxX * -0.5f + (float)Width * 0.5f, (float)maxX * 1.5f - (float)Width * 0.5f);
		position.Y = MathHelper.Clamp(position.Y, (float)maxY * -0.5f + (float)Height * 0.5f, (float)maxY * 1.5f - (float)Height * 0.5f);
		if (characters.ability[0] > 0.5f && currentGamePadState != oldGamePadState && currentGamePadState.Buttons.A == ButtonState.Pressed)
		{
			result = "SHIELD";
		}
		if (characters.ability[0] > 0.5f && useKeyboardControls && ((currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyUp(Keys.LeftControl) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || state.RightButton == ButtonState.Pressed))
		{
			result = "SHIELD";
		}
		if (characters.shipClass == "Defender")
		{
			shootingType = "REGULAR";
			if (characters.ability[1] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.X == ButtonState.Pressed)
			{
				result = "TURRET";
			}
			if (characters.ability[2] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.Y == ButtonState.Pressed)
			{
				result = "HIVE";
			}
			if (characters.ability[3] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.B == ButtonState.Pressed)
			{
				result = "SANCTUARY";
			}
			if (useKeyboardControls)
			{
				if (characters.ability[1] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D1) || state.MiddleButton == ButtonState.Pressed))
				{
					result = "TURRET";
				}
				if (characters.ability[2] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D2) || state.XButton1 == ButtonState.Pressed))
				{
					result = "HIVE";
				}
				if (characters.ability[3] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D3) || state.XButton2 == ButtonState.Pressed))
				{
					result = "SANCTUARY";
				}
			}
		}
		else
		{
			if (characters.ability[1] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.X == ButtonState.Pressed)
			{
				result = "WIDE";
				shootingType = "WIDE";
			}
			if (characters.ability[2] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.Y == ButtonState.Pressed)
			{
				result = "LONG";
				shootingType = "LONG";
			}
			if (currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.B == ButtonState.Pressed)
			{
				result = "NORMAL";
				shootingType = "NORMAL";
			}
			if (useKeyboardControls)
			{
				if (characters.ability[1] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D3) || state.XButton2 == ButtonState.Pressed))
				{
					result = "WIDE";
					shootingType = "WIDE";
				}
				if (characters.ability[2] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D2) || state.XButton1 == ButtonState.Pressed))
				{
					result = "LONG";
					shootingType = "LONG";
				}
				if (currentKeyboardState.IsKeyDown(Keys.D1) || state.MiddleButton == ButtonState.Pressed)
				{
					result = "NORMAL";
					shootingType = "NORMAL";
				}
			}
		}
		if (Math.Abs(accelerationX) > 0f || Math.Abs(accelerationY) > 0f || Math.Abs(accelerationX2) > 0f || Math.Abs(accelerationY2) > 0f)
		{
			angle = (float)Math.Atan2(accelerationY, accelerationX);
		}
		updateLevel();
		if (beingHit > 0)
		{
			beingHit--;
		}
		position += new Vector2(accelerationX, accelerationY);
		if (Health <= 0f)
		{
			Health = 0f;
			Destroyed++;
			if (Destroyed > 100f)
			{
				Active = false;
			}
		}
		Health = MathHelper.Clamp(Health, 0f, maximunHealth);
		credits = MathHelper.Clamp(credits, 0f, maximunCredits);
		for (int i = 0; i < characters.relics; i++)
		{
			if (i < characters.ability.Count)
			{
				characters.ability[i] = MathHelper.SmoothStep(characters.ability[i], 1.1f, 0.05f);
				characters.ability[i] = MathHelper.Clamp(characters.ability[i], 0f, 1f);
			}
		}
		oldKeyboardState = currentKeyboardState;
		oldGamePadState = currentGamePadState;
		if (!Active)
		{
			result = "";
		}
		return result;
	}

	public void UpdateChallenge(int maxX, int maxY, int vibration, bool useKeyboardControls)
	{
		if (!Active)
		{
			Destroyed = 100f;
		}
		if (levelMessage > 0)
		{
			levelMessage--;
			levelTransp = MathHelper.Lerp(levelTransp, 1f, 0.01f);
		}
		else
		{
			levelTransp = MathHelper.Lerp(levelTransp, 0f, 0.01f);
		}
		accelerationX2 = 0f;
		accelerationY2 = 0f;
		oldKeyboardState = currentKeyboardState;
		oldGamePadState = currentGamePadState;
		oldMouseState = currentMouseState;
		currentKeyboardState = Keyboard.GetState();
		currentGamePadState = GamePad.GetState(index);
		currentMouseState = Mouse.GetState();
		if ((double)Math.Abs(currentGamePadState.ThumbSticks.Left.X) > 0.1)
		{
			accelerationX2 = currentGamePadState.ThumbSticks.Left.X * 10f;
		}
		if ((double)Math.Abs(currentGamePadState.ThumbSticks.Left.Y) > 0.1)
		{
			accelerationY2 = currentGamePadState.ThumbSticks.Left.Y * -10f;
		}
		if (currentGamePadState.DPad.Left == ButtonState.Pressed)
		{
			accelerationX2 = -10f;
		}
		if (currentGamePadState.DPad.Right == ButtonState.Pressed)
		{
			accelerationX2 = 10f;
		}
		if (currentGamePadState.DPad.Up == ButtonState.Pressed)
		{
			accelerationY2 = -10f;
		}
		if (currentGamePadState.DPad.Down == ButtonState.Pressed)
		{
			accelerationY2 = 10f;
		}
		if (useKeyboardControls)
		{
			KeyboardState state = Keyboard.GetState();
			if (state.IsKeyDown(Keys.A) && !state.IsKeyDown(Keys.D))
			{
				accelerationX2 = -10f;
			}
			if (state.IsKeyDown(Keys.D) && !state.IsKeyDown(Keys.A))
			{
				accelerationX2 = 10f;
			}
			if (state.IsKeyDown(Keys.W) && !state.IsKeyDown(Keys.S))
			{
				accelerationY2 = -10f;
			}
			if (state.IsKeyDown(Keys.S) && !state.IsKeyDown(Keys.W))
			{
				accelerationY2 = 10f;
			}
		}
		if (beingHit > 195)
		{
			vibrationLeft = 1f * (float)vibration;
			vibrationRight = 1f * (float)vibration;
		}
		if (vibrationLeft > 0f)
		{
			vibrationLeft -= 0.05f;
		}
		if (vibrationRight > 0f)
		{
			vibrationRight -= 0.05f;
		}
		vibrationLeft = MathHelper.Clamp(vibrationLeft, 0f, 1f);
		vibrationRight = MathHelper.Clamp(vibrationRight, 0f, 1f);
		GamePad.SetVibration(index, vibrationLeft, vibrationRight);
		if (!Active)
		{
			accelerationX2 = 0f;
			accelerationY2 = 0f;
		}
		accelerationX = MathHelper.SmoothStep(accelerationX, accelerationX2, 0.2f);
		accelerationY = MathHelper.SmoothStep(accelerationY, accelerationY2, 0.2f);
		position.X = MathHelper.Clamp(position.X, (float)maxX * -0.5f + (float)Width * 0.5f, (float)maxX * 1.5f - (float)Width * 0.5f);
		position.Y = MathHelper.Clamp(position.Y, (float)maxY * -0.5f + (float)Height * 0.5f, (float)maxY * 1.5f - (float)Height * 0.5f);
		if (Math.Abs(accelerationX) > 0f || Math.Abs(accelerationY) > 0f || Math.Abs(accelerationX2) > 0f || Math.Abs(accelerationY2) > 0f)
		{
			angle = (float)Math.Atan2(accelerationY, accelerationX);
		}
		updateLevel();
		if (beingHit > 0)
		{
			beingHit--;
		}
		position += new Vector2(accelerationX, accelerationY);
		if (Health <= 0f)
		{
			Health = 0f;
			Destroyed++;
			if (Destroyed > 100f)
			{
				Active = false;
			}
		}
		Health = MathHelper.Clamp(Health, 0f, maximunHealth);
		credits = MathHelper.Clamp(credits, 0f, maximunCredits);
		for (int i = 0; i < characters.relics; i++)
		{
			if (i < characters.ability.Count)
			{
				characters.ability[i] = MathHelper.SmoothStep(characters.ability[i], 1.1f, 0.05f);
				characters.ability[i] = MathHelper.Clamp(characters.ability[i], 0f, 1f);
			}
		}
	}

	private void updateLevel()
	{
		if (level > 20)
		{
			level = 20;
		}
		levelOld = level;
		if (experience >= nextLevel)
		{
			experience = 0;
			nextLevel = (int)((float)nextLevel * 3.1f);
			level++;
		}
		if (levelOld != level)
		{
			shootRate -= 0.25f;
			maximunHealth += 0.1f;
			Health++;
			speed += 0.1f;
			SA++;
			levelUpdated = true;
			levelMessage = 100;
			beingHit = 100;
			shootDamage += 0.05f;
		}
		shootRate = MathHelper.Clamp(shootRate, 3f, 20f);
		maximunHealth = MathHelper.Clamp(maximunHealth, 0f, 15f);
	}

	public void hit(float damage)
	{
		if (beingHit <= 1)
		{
			beingHit = 200;
			Health -= damage;
		}
	}

	public void Draw(SpriteBatch spriteBatch, Vector2 colonyPosition, float opac)
	{
		if (Active && Destroyed < 100f)
		{
			for (int num = 2; num > 0; num--)
			{
				float num2 = MathHelper.Clamp((float)beingHit / 50f, opac * 0.05f + (1f - Health / maximunHealth) / 2f, 1f);
				spriteBatch.Draw(uiHealth, position, new Rectangle(0, 0, (int)(Health / maximunHealth * (float)uiHealth.Width), uiHealth.Height), new Color((1f - Health / maximunHealth) / 2f, Health / maximunHealth / 2f, 0f, 0.5f) * opac * num2 * 0.5f, 0f, new Vector2(uiHealth.Width / 2, uiHealth.Height / 2), 0.75f + (float)num / 25f, SpriteEffects.None, 0.13f);
				spriteBatch.Draw(uiBlue, position, new Rectangle(0, 0, (int)(credits / (float)maximunCredits * (float)uiBlue.Width), uiBlue.Height), new Color(0f, 1f, 2f, 0.5f) * opac * num2 * 0.5f, 0f, new Vector2(uiBlue.Width / 2, uiBlue.Height / 2), 0.75f + (float)num / 25f, SpriteEffects.None, 0.12f);
				spriteBatch.Draw(ui, position, null, new Color(1f, 1f, 1f, 0.5f) * opac * num2 * 0.5f, 0f, new Vector2(ui.Width / 2, ui.Height / 2), 0.74f + (float)num / 25f, SpriteEffects.None, 0.11f);
				spriteBatch.Draw(ui, position, null, new Color(1f, 1f, 1f, 0.5f) * opac * num2 * 0.5f, 0f, new Vector2(ui.Width / 2, ui.Height / 2), 0.76f + (float)num / 25f, SpriteEffects.None, 0.11f);
			}
			if (beingHit > 0)
			{
				spriteBatch.Draw(texture, position, null, new Color((float)Math.Abs(Math.Sin((float)beingHit / 10f)) / 2f * 0.5f, (float)Math.Abs(Math.Sin((float)beingHit / 10f)) * 1.5f, (float)Math.Abs(Math.Sin((float)beingHit / 10f)) * 2f, (float)Math.Abs(Math.Sin((float)beingHit / 10f))), angle, new Vector2(Width / 2, Height / 2), scale, SpriteEffects.None, 0.08f);
			}
			spriteBatch.Draw(texture, position, null, Color.White, angle, new Vector2(Width / 2, Height / 2), scale, SpriteEffects.None, 0.08f);
			string text = "";
			string text2 = "";
			string text3 = "";
			for (int num = 0; num < SA; num++)
			{
				text += "+";
				text2 += "o";
				text3 += "T";
			}
			spriteBatch.DrawString(font, text, position, Color.Blue, (float)Math.PI / 2f, new Vector2(-30f, 0f), 0.5f, SpriteEffects.None, 0.1f);
			spriteBatch.DrawString(font, text2, position, Color.Blue, (float)Math.PI / 2f, new Vector2(-30f, 0f), 0.5f, SpriteEffects.None, 0.1f);
			spriteBatch.DrawString(font, text3, position, Color.Blue, (float)Math.PI / 2f, new Vector2(-30f, 0f), 0.5f, SpriteEffects.None, 0.1f);
			if (colonyPosition.X > -1000f)
			{
				spriteBatch.Draw(txCircle, position, null, new Color(1f, 1f, 1f, 0.5f) * opac * 0.2f, (float)Math.Atan2(colonyPosition.Y - position.Y, colonyPosition.X - position.X), new Vector2((float)txCircle.Width / 2f, (float)txCircle.Height / 2f), 0.6f, SpriteEffects.None, 0.1f);
			}
			if (levelTransp > 0.01f)
			{
				spriteBatch.DrawString(font, "Level Up!", position - new Vector2(0f, (100f - (float)levelMessage) / 2f), Color.LightCyan * levelTransp * 2f, 0f, font.MeasureString("Level Up!") * 0.5f, 1f, SpriteEffects.None, 0.09f);
				spriteBatch.DrawString(font, "Level Up!", position - new Vector2(0f, (100f - (float)levelMessage) * (100f - (float)levelMessage) / 2f), Color.LightCyan * levelTransp, 0f, font.MeasureString("Level Up!") * 0.5f, 1f, SpriteEffects.None, 0.09f);
				spriteBatch.DrawString(font, "Level Up!", position - new Vector2(0f, (100f - (float)levelMessage) * (100f - (float)levelMessage) * (100f - (float)levelMessage) / 2f), Color.LightCyan * levelTransp * 0.5f, 0f, font.MeasureString("Level Up!") * 0.5f, 1f, SpriteEffects.None, 0.09f);
			}
		}
	}

	public void DrawVectorial(SpriteBatch sb, Color shootColor)
	{
		if (Destroyed < 100f)
		{
			CalculateVectorial();
			draw2d.drawLine(sb, position + points[0], position + points[1], 2f, shootColor);
			draw2d.drawLine(sb, position + points[1], position + points[2], 2f, shootColor);
			draw2d.drawLine(sb, position + points[2], position + points[3], 2f, shootColor);
			draw2d.drawLine(sb, position + points[3], position + points[0], 2f, shootColor);
		}
	}

	public void DrawSelect(SpriteBatch sb, Vector2 pos, Texture2D[] tx, bool Active, bool selected, bool ready, int id, Color col, uint frame)
	{
		this.Active = Active;
		this.frame = frame;
		if (!Active)
		{
			try
			{
				sb.DrawString(font, "PRESS", new Vector2(pos.X, pos.Y - 20f), new Color((float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f), 0f, new Vector2(font.MeasureString("PRESS").X / 2f, font.MeasureString("PRESS").Y / 2f), 1f, SpriteEffects.None, 0f);
				sb.DrawString(font, "TO JOIN", new Vector2(pos.X, pos.Y + 40f), new Color((float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f), 0f, new Vector2(font.MeasureString("TO JOIN").X / 2f, font.MeasureString("TO JOIN").Y / 2f), 1f, SpriteEffects.None, 0f);
			}
			catch
			{
			}
			change = 0f;
		}
		else
		{
			sb.Draw(tx[id], pos, null, Color.White * change, 0f, new Vector2(tx[id].Width / 2, tx[id].Height / 2), Vector2.One, SpriteEffects.None, 0.5f);
			sb.DrawString(font, characters.name, new Vector2(pos.X, pos.Y - 205f), new Color((float)(int)col.R / 255f + 0.75f, (float)(int)col.G / 255f + 0.75f, (float)(int)col.B / 255f + 0.75f, 1f), 0f, new Vector2(font.MeasureString(characters.name).X / 2f, font.MeasureString(characters.name).Y / 2f), 0.9f, SpriteEffects.None, 0.4f);
			sb.DrawString(font, "Level: \n" + level + "\n\nAbilities: \n" + abilityNames[0].Substring((1 - (int)characters.ability[0]) * abilityNames[0].Length) + "\n" + abilityNames[1].Substring((1 - (int)characters.ability[1]) * abilityNames[1].Length) + "\n" + abilityNames[2].Substring((1 - (int)characters.ability[2]) * abilityNames[2].Length) + "\n" + abilityNames[3].Substring((1 - (int)characters.ability[3]) * abilityNames[3].Length) + "\n\nSpecial: \n" + characters.abilityType, new Vector2(pos.X - 75f, pos.Y - 165f), new Color((float)(int)col.R / 255f + 0.75f, (float)(int)col.G / 255f + 0.75f, (float)(int)col.B / 255f + 0.75f, 1f), 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.4f);
			if (selected)
			{
				draw2d.DrawPixel(sb, new Rectangle((int)pos.X - (int)(font.MeasureString("READY!").X / 2f * 1.5f), (int)pos.Y - (int)(font.MeasureString("READY!").Y / 2f * 1.5f), (int)(font.MeasureString("READY!").X * 1.5f), (int)(font.MeasureString("READY!").Y * 1.5f)), Color.Black, 0.3f);
				sb.DrawString(font, "READY!", new Vector2(pos.X, pos.Y), Color.White, 0f, new Vector2(font.MeasureString("READY!").X / 2f, font.MeasureString("READY!").Y / 2f), 1.5f, SpriteEffects.None, 0.2f);
			}
			sb.DrawString(font, characters.shipClass, new Vector2(pos.X, pos.Y + 205f), Color.White, 0f, new Vector2(font.MeasureString(characters.shipClass).X / 2f, font.MeasureString(characters.shipClass).Y / 2f), 1f, SpriteEffects.None, 0.4f);
		}
		card = new Rectangle((int)pos.X - texture.Width / 2, (int)pos.Y - texture.Height / 2, texture.Width, texture.Height);
		if (frame > 1000000)
		{
			frame -= 1000000;
		}
	}

	public void DrawUI(SpriteBatch sb, int viewportW, float opac)
	{
		if (Active)
		{
			Vector2 vector = new Vector2(10f, 0f);
			Vector2 vector2 = new Vector2(0f, 25f);
			int num = 0;
			float num2 = 0f;
			float num3 = 1f;
			SpriteEffects effects = SpriteEffects.None;
			switch (index)
			{
			case PlayerIndex.One:
				UIpos = new Vector2(0f, 90f);
				num = 0;
				num2 = 0.2f;
				num3 = 1.6f;
				UIposText = new Vector2(0f, 65f);
				break;
			case PlayerIndex.Two:
				UIpos = new Vector2(viewportW, 90f);
				num = 1;
				effects = SpriteEffects.FlipHorizontally;
				num2 = 0.2f;
				num3 = 1.6f;
				UIposText = new Vector2(viewportW + 40, 65f);
				break;
			case PlayerIndex.Three:
				UIpos = new Vector2(0f, 600f);
				num = 0;
				num2 = -0.2f;
				num3 = 2.2f;
				UIposText = new Vector2(0f, 715f);
				break;
			case PlayerIndex.Four:
				UIpos = new Vector2(viewportW, 600f);
				num = 1;
				effects = SpriteEffects.FlipHorizontally;
				num2 = -0.2f;
				num3 = 2.2f;
				UIposText = new Vector2(viewportW + 40, 715f);
				break;
			}
			sb.Draw(txBar, UIpos + vector2 * 0f, null, new Color(1f, 1f, 1f, 1f) * characters.ability[0] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(characters.ability[0] * num3 + num2 * 3f, 1f), effects, 0.5f);
			sb.Draw(txBar, UIpos + vector2 * 1f, null, new Color(1f, 1f, 1f, 1f) * characters.ability[1] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(characters.ability[1] * num3 + num2 * 2f, 1f), effects, 0.5f);
			sb.Draw(txBar, UIpos + vector2 * 2f, null, new Color(1f, 1f, 1f, 1f) * characters.ability[2] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(characters.ability[2] * num3 + num2 * 1f, 1f), effects, 0.5f);
			sb.Draw(txBar, UIpos + vector2 * 3f, null, new Color(1f, 1f, 1f, 1f) * characters.ability[3] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(characters.ability[3] * num3 + num2 * 0f, 1f), effects, 0.5f);
			UIpos += new Vector2(0f, 4f);
			if (num == 1)
			{
				vector = new Vector2(-100f, 0f);
			}
			sb.DrawString(font, abilityNames[0], UIpos + vector + vector2 * 0f, new Color(0.4f, 0.8f, 1f, 1f) * characters.ability[0] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
			sb.DrawString(font, abilityNames[1], UIpos + vector + vector2 * 1f, new Color(0.4f, 0.8f, 1f, 1f) * characters.ability[1] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
			sb.DrawString(font, abilityNames[2], UIpos + vector + vector2 * 2f, new Color(0.4f, 0.8f, 1f, 1f) * characters.ability[2] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
			sb.DrawString(font, abilityNames[3], UIpos + vector + vector2 * 3f, new Color(0.4f, 0.8f, 1f, 1f) * characters.ability[3] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
			sb.DrawString(font, "XP " + experience + "/" + nextLevel, UIposText + vector - new Vector2(0f, num2 * 100f), Color.Cyan * opac, 0f, new Vector2(font.MeasureString("XP " + experience + "/" + nextLevel).X * (float)num, 0f), 0.75f, SpriteEffects.None, 0f);
			sb.DrawString(font, characters.shipClass + " lv " + level, UIposText + vector, Color.Cyan * opac, 0f, new Vector2(font.MeasureString(characters.shipClass + " " + level).X * (float)num, 0f), 0.75f, SpriteEffects.None, 0f);
		}
	}
}
