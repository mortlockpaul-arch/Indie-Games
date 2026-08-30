using System;
using System.Collections.Generic;
using Hammer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Defenders;

internal class Player
{
	private const float speedChallenge = 6f;

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

	public int oldDireccion;

	private int direction;

	private int menuCounter = 0;

	public float scale = 0.5f;

	private Rectangle card = Rectangle.Empty;

	private Primitive2D draw2d;

	private float[] pDist = new float[4];

	private float[] pAngle = new float[4];

	private Vector2[] points = new Vector2[4];

	private Vector2[] pRandom = new Vector2[4];

	public List<Character> characters = new List<Character>(4);

	public ushort SA;

	public int score;

	public int orbs = 0;

	public int maxOrbs = 0;

	public float speed;

	public float boost = 1f;

	public float topSpeed;

	public float angle;

	public float shootRate;

	public float shootDamage;

	public Color shootColor;

	public string shootingType;

	public int shootingChangeCost;

	public int beingHit;

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

	public bool damaged = false;

	public int number = 0;

	private float change = 1f;

	public PlayerIndex index;

	public uint minutes = 0u;

	public ushort seconds = 0;

	public ushort hours = 0;

	public bool ready;

	public bool selected;

	public bool wasActive;

	public bool Active;

	public float smoothness = 0f;

	private Random random = new Random();

	public int Width => texture.Width;

	public int Height => texture.Height;

	public string name
	{
		get
		{
			return characters[number].name;
		}
		set
		{
			characters[number].name = value;
		}
	}

	public string shipClass
	{
		get
		{
			return characters[number].shipClass;
		}
		set
		{
			characters[number].shipClass = value;
		}
	}

	public Color color
	{
		get
		{
			return characters[number].color;
		}
		set
		{
			characters[number].color = value;
		}
	}

	public string abilityType
	{
		get
		{
			return characters[number].abilityType;
		}
		set
		{
			characters[number].abilityType = value;
		}
	}

	public int relics
	{
		get
		{
			return characters[number].relic;
		}
		set
		{
			characters[number].relics[Game1.currentLevel] = (ushort)value;
		}
	}

	public ushort nRelics => characters[number].nRelics();

	public List<float> ability
	{
		get
		{
			return characters[number].ability;
		}
		set
		{
			characters[number].ability = value;
		}
	}

	public uint numberOfKills
	{
		get
		{
			return characters[number].numberOfKills;
		}
		set
		{
			characters[number].numberOfKills = value;
		}
	}

	public int experience
	{
		get
		{
			return characters[number].experience;
		}
		set
		{
			characters[number].experience = value;
		}
	}

	public ushort level
	{
		get
		{
			return characters[number].level;
		}
		set
		{
			characters[number].level = value;
		}
	}

	public int nextLevel
	{
		get
		{
			return characters[number].nextLevel;
		}
		set
		{
			characters[number].nextLevel = value;
		}
	}

	public float size()
	{
		return (float)(Width + Height) / 2f * scale;
	}

	private void createcharacters(int number)
	{
		orbs = 0;
		switch (number)
		{
		case 0:
		{
			List<Character> list4 = characters;
			Color lightCyan = Color.LightCyan;
			ushort[] array = new ushort[15];
			list4.Add(new Character("Mark R.", "Fighter", lightCyan, "Hell Storm", array, new List<float>(4), 0u, 0, 0, 750));
			break;
		}
		case 1:
		{
			List<Character> list3 = characters;
			Color obj3 = new Color(1f, 0.37f, 0f);
			ushort[] array = new ushort[15];
			list3.Add(new Character("Michelle W.", "Defender", obj3, "Sonic Bomb", array, new List<float>(4), 0u, 0, 0, 750));
			break;
		}
		case 2:
		{
			List<Character> list2 = characters;
			Color obj2 = new Color(0f, 0.86f, 0.01f);
			ushort[] array = new ushort[15];
			list2.Add(new Character("James R.", "Fighter", obj2, "EMP", array, new List<float>(4), 0u, 0, 0, 750));
			break;
		}
		case 3:
		{
			List<Character> list = characters;
			Color obj = new Color(0.86f, 0f, 0.84f);
			ushort[] array = new ushort[15];
			list.Add(new Character("Johnny V.", "Defender", obj, "Laser Blades", array, new List<float>(4), 0u, 0, 0, 750));
			break;
		}
		}
	}

	public Player(int number)
	{
		for (int i = 0; i < 4; i++)
		{
			createcharacters(i);
		}
		for (int j = 0; j < 4; j++)
		{
			for (int k = 0; k < 10; k++)
			{
				characters[number].ability.Add(0f);
			}
		}
	}

	public Player(GraphicsDevice GraphicsDevice, Texture2D texture, SpriteFont gameFont, int number, bool fighter, PlayerIndex index, Texture2D ui, Texture2D uiHealth, Texture2D uiBlue, Texture2D txCircle, Texture2D txBar)
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
		for (int i = 0; i < 4; i++)
		{
			createcharacters(i);
		}
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
		characters[number].level = 0;
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
		characters[number].experience = 0;
		characters[number].nextLevel = 750;
		for (int j = 0; j < 4; j++)
		{
			for (int k = 0; k < 10; k++)
			{
				characters[j].ability.Add(0f);
			}
		}
		if (characters[number].shipClass == "Defender")
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
		if (characters[number].shipClass == "Fighter")
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
		if (Game1.gameState == GameState.Challenge)
		{
			speed = 6f;
			smoothness = 1f;
			maximunHealth = 0.01f;
		}
		Health = maximunHealth;
		topSpeed = speed;
		speed = 0f;
		Character character = characters[number];
		ushort[] array = new ushort[15];
		character.relics = array;
		Reset();
	}

	public void AddRelic()
	{
		relics = 1;
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
		damaged = false;
		shootingType = "NORMAL";
		credits = maximunCredits / 4;
		Health = maximunHealth;
		Destroyed = 0f;
		seconds = 0;
		minutes = 0u;
		hours = 0;
		if (characters[number].shipClass == "Fighter")
		{
			maxOrbs = 1;
		}
		else
		{
			maxOrbs = 3;
		}
		if (Game1.gameState == GameState.Challenge)
		{
			speed = 6f;
			smoothness = 1f;
			maximunHealth = 0.01f;
		}
	}

	public void HardReset()
	{
		level = 0;
		nextLevel = 750;
		for (int i = 0; i < 4; i++)
		{
			characters[i].Reset();
		}
		damaged = false;
		if (Game1.gameState == GameState.Challenge)
		{
			speed = 6f;
			smoothness = 1f;
			maximunHealth = 0.01f;
		}
	}

	public void UpdateCharSelection(bool useKeyboardControls, Vector2 mousePos, bool taken)
	{
		oldMouseState = currentMouseState;
		oldKeyboardState = currentKeyboardState;
		oldGamePadState = currentGamePadState;
		currentMouseState = Mouse.GetState();
		currentKeyboardState = Keyboard.GetState();
		currentGamePadState = GamePad.GetState(index);
		damaged = false;
		orbs = 0;
		direction = 0;
		menuCounter--;
		if (menuCounter < 0)
		{
			menuCounter = 0;
		}
		if (taken)
		{
			direction = 1;
		}
		if (ready || selected)
		{
			direction = 0;
			oldDireccion = 0;
		}
		if (isMouseOn(mousePos) && currentMouseState.LeftButton != oldMouseState.LeftButton && currentMouseState.LeftButton == ButtonState.Pressed)
		{
			select(sel: true);
		}
		if (isMouseOn(mousePos) && currentMouseState.RightButton != oldMouseState.RightButton && currentMouseState.RightButton == ButtonState.Pressed)
		{
			select(sel: false);
		}
		if ((oldGamePadState.Buttons.A != ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Pressed) || (oldGamePadState.Buttons.Start != ButtonState.Pressed && currentGamePadState.Buttons.Start == ButtonState.Pressed))
		{
			select(sel: true);
		}
		if (oldGamePadState.Buttons.B != ButtonState.Pressed && currentGamePadState.Buttons.B == ButtonState.Pressed)
		{
			select(sel: false);
		}
		if (menuCounter == 0 && currentGamePadState != oldGamePadState && (GamePad.GetState(index).ThumbSticks.Left.X > 0.75f || GamePad.GetState(index).DPad.Right == ButtonState.Pressed))
		{
			direction = 1;
			menuCounter = 10;
		}
		if (menuCounter == 0 && currentGamePadState != oldGamePadState && (GamePad.GetState(index).ThumbSticks.Left.X < -0.75f || GamePad.GetState(index).DPad.Left == ButtonState.Pressed))
		{
			direction = -1;
			menuCounter = 10;
		}
		if (useKeyboardControls)
		{
			if (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Enter))
			{
				select(sel: true);
			}
			if (currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Escape))
			{
				select(sel: false);
			}
			if (menuCounter == 0 && currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Right))
			{
				direction = 1;
				menuCounter = 10;
			}
			if (menuCounter == 0 && currentKeyboardState != oldKeyboardState && currentKeyboardState.IsKeyDown(Keys.Left))
			{
				direction = -1;
				menuCounter = 10;
			}
		}
		if (selected)
		{
			direction = 0;
		}
		if (direction != oldDireccion && direction == 1 && Active)
		{
			number++;
		}
		if (direction != oldDireccion && direction == -1 && Active)
		{
			number--;
		}
		if (number > 3)
		{
			number = 0;
		}
		if (number < 0)
		{
			number = 3;
		}
		if (characters[number].shipClass == "Fighter")
		{
			shootRate = 9f;
			shootDamage = 1f;
			maximunHealth = 4.5f;
			speed = 10f;
			credits = 0f;
			maximunCredits = 50;
			smoothness = 0.125f;
			creditsMul = 2f;
			maxOrbs = 1;
		}
		else
		{
			shootRate = 10f;
			shootDamage = 0.75f;
			maximunHealth = 7.5f;
			speed = 7f;
			credits = 10f;
			maximunCredits = 100;
			smoothness = 0.25f;
			creditsMul = 1f;
			maxOrbs = 3;
		}
		if (Game1.gameState == GameState.Challenge)
		{
			speed = 6f;
			smoothness = 1f;
			maximunHealth = 0.01f;
		}
		if (direction != 0)
		{
			change = 0f;
		}
		else
		{
			change = MathHelper.Lerp(change, 1f, 0.1f);
		}
		maximunHealth *= 2f - Game1.difficulty;
		Health = maximunHealth;
		topSpeed = speed;
		speed = 0f;
		credits = maximunCredits / 4;
		shootColor = characters[number].color;
		wasActive = Active;
		oldDireccion = direction;
	}

	private void select(bool sel)
	{
		if (sel)
		{
			if (Active)
			{
				if (selected)
				{
					ready = sel;
				}
				else
				{
					selected = sel;
				}
			}
			else
			{
				Active = sel;
			}
		}
		else if (selected)
		{
			if (ready)
			{
				ready = sel;
			}
			else
			{
				selected = sel;
			}
		}
		else
		{
			Active = sel;
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

	public Vector2 UpdateShootingSideScroller(bool useKeyboardControls, Vector2 mousePos, PlayerIndex index)
	{
		MouseState state = Mouse.GetState();
		currentKeyboardState = Keyboard.GetState();
		currentGamePadState = GamePad.GetState(index);
		frame++;
		if (frame > 1000000)
		{
			frame -= 1000000u;
		}
		shootingThrottle = new Vector2(0f, 0f);
		if (frame % 10 != 0)
		{
			return shootingThrottle;
		}
		if (currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed || currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
		{
			shootingThrottle.X = (float)Math.Cos(angle) * 100f;
			shootingThrottle.Y = (float)Math.Sin(angle) * -100f;
		}
		if (Math.Abs(currentGamePadState.ThumbSticks.Right.X) > 0.1f || Math.Abs(currentGamePadState.ThumbSticks.Right.Y) > 0.1f)
		{
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
				shootingThrottle.X = vector.Y;
				shootingThrottle.Y = vector.X;
				angle = 0f;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Space) && Game1.gameState == GameState.Sidescroller)
			{
				angle = 0f;
				shootingThrottle = new Vector2(100f, 0f);
			}
			if (currentKeyboardState.IsKeyDown(Keys.Space) && Game1.gameState == GameState.ChubbyRain)
			{
				shootingThrottle = new Vector2(0f, 100f);
				angle = 0f;
			}
		}
		angle = 0f;
		oldKeyboardState = currentKeyboardState;
		oldGamePadState = currentGamePadState;
		return shootingThrottle;
	}

	public Vector2 UpdateShootingMeteroids(bool useKeyboardControls, Vector2 mousePos, PlayerIndex index)
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
			if ((Math.Abs(shootingThrottle.X) > 0f || Math.Abs(shootingThrottle.Y) > 0f) && (state.LeftButton == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.LeftAlt)))
			{
				Vector2 vector = mousePos - position;
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
		if (Game1.gameState == GameState.ChubbyRain)
		{
			angle = -(float)Math.PI / 2f;
		}
		oldKeyboardState = currentKeyboardState;
		oldGamePadState = currentGamePadState;
		return shootingThrottle;
	}

	public Vector2 UpdateShootingChubbyRain(bool useKeyboardControls, Vector2 mousePos, PlayerIndex index)
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
			if ((Math.Abs(shootingThrottle.X) > 0f || Math.Abs(shootingThrottle.Y) > 0f) && (state.LeftButton == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.LeftAlt)))
			{
				Vector2 vector = mousePos - position;
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
		if (Health <= 0f)
		{
			Health = 0f;
			Destroyed++;
			if (Destroyed > 100f)
			{
				Active = false;
			}
			return result;
		}
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
			accelerationX2 = GamePad.GetState(index).ThumbSticks.Left.X * topSpeed;
		}
		if ((double)Math.Abs(GamePad.GetState(index).ThumbSticks.Left.Y) > 0.1)
		{
			accelerationY2 = GamePad.GetState(index).ThumbSticks.Left.Y * (0f - topSpeed);
		}
		if (GamePad.GetState(index).DPad.Left == ButtonState.Pressed)
		{
			accelerationX2 = 0f - topSpeed;
		}
		if (GamePad.GetState(index).DPad.Right == ButtonState.Pressed)
		{
			accelerationX2 = topSpeed;
		}
		if (GamePad.GetState(index).DPad.Up == ButtonState.Pressed)
		{
			accelerationY2 = 0f - topSpeed;
		}
		if (GamePad.GetState(index).DPad.Down == ButtonState.Pressed)
		{
			accelerationY2 = topSpeed;
		}
		if (useKeyboardControls)
		{
			KeyboardState state2 = Keyboard.GetState();
			if (state2.IsKeyDown(Keys.A) && !state2.IsKeyDown(Keys.D))
			{
				accelerationX2 = 0f - topSpeed;
			}
			if (state2.IsKeyDown(Keys.D) && !state2.IsKeyDown(Keys.A))
			{
				accelerationX2 = topSpeed;
			}
			if (state2.IsKeyDown(Keys.W) && !state2.IsKeyDown(Keys.S))
			{
				accelerationY2 = 0f - topSpeed;
			}
			if (state2.IsKeyDown(Keys.S) && !state2.IsKeyDown(Keys.W))
			{
				accelerationY2 = topSpeed;
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
		if (characters[number].ability[0] > 0.5f && currentGamePadState != oldGamePadState && currentGamePadState.Buttons.A == ButtonState.Pressed)
		{
			result = "SHIELD";
		}
		if (characters[number].ability[0] > 0.5f && useKeyboardControls && ((currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyUp(Keys.LeftControl) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || state.RightButton == ButtonState.Pressed))
		{
			result = "SHIELD";
		}
		if (characters[number].shipClass == "Defender")
		{
			shootingType = "REGULAR";
			if (characters[number].ability[1] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.X == ButtonState.Pressed)
			{
				result = "TURRET";
			}
			if (characters[number].ability[2] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.Y == ButtonState.Pressed)
			{
				result = "HIVE";
			}
			if (characters[number].ability[3] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.B == ButtonState.Pressed)
			{
				result = "SANCTUARY";
			}
			if (useKeyboardControls)
			{
				if (characters[number].ability[1] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D1) || state.MiddleButton == ButtonState.Pressed))
				{
					result = "TURRET";
				}
				if (characters[number].ability[2] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D2) || state.XButton1 == ButtonState.Pressed))
				{
					result = "HIVE";
				}
				if (characters[number].ability[3] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D3) || state.XButton2 == ButtonState.Pressed))
				{
					result = "SANCTUARY";
				}
			}
		}
		else
		{
			if (characters[number].ability[1] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.X == ButtonState.Pressed)
			{
				result = "WIDE";
				shootingType = "WIDE";
			}
			if (characters[number].ability[2] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.Y == ButtonState.Pressed)
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
				if (characters[number].ability[1] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D3) || state.XButton2 == ButtonState.Pressed))
				{
					result = "WIDE";
					shootingType = "WIDE";
				}
				if (characters[number].ability[2] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D2) || state.XButton1 == ButtonState.Pressed))
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
		for (int i = 0; i < nRelics; i++)
		{
			if (i < characters[number].ability.Count)
			{
				characters[number].ability[i] = MathHelper.SmoothStep(characters[number].ability[i], 1.1f, 0.05f);
				characters[number].ability[i] = MathHelper.Clamp(characters[number].ability[i], 0f, 1f);
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

	public string UpdateMeteroids(int maxX, int maxY, int vibration, bool useKeyboardControls, PlayerIndex index)
	{
		if (!Active)
		{
			Destroyed = 100f;
		}
		string result = "";
		if (Health <= 0f)
		{
			Health = 0f;
			Destroyed++;
			if (Destroyed > 100f)
			{
				Active = false;
			}
			return result;
		}
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
		if ((double)GamePad.GetState(index).ThumbSticks.Left.X > 0.1)
		{
			angle += 0.05f;
		}
		if ((double)GamePad.GetState(index).ThumbSticks.Left.X < -0.1)
		{
			angle -= 0.05f;
		}
		if ((double)GamePad.GetState(index).ThumbSticks.Left.Y > 0.1)
		{
			speed += 0.1f;
		}
		if ((double)GamePad.GetState(index).ThumbSticks.Left.Y < -0.1)
		{
			speed -= 0.1f;
		}
		if (GamePad.GetState(index).DPad.Left == ButtonState.Pressed)
		{
			angle -= 0.05f;
		}
		if (GamePad.GetState(index).DPad.Right == ButtonState.Pressed)
		{
			angle += 0.05f;
		}
		if (GamePad.GetState(index).DPad.Up == ButtonState.Pressed)
		{
			speed += 0.1f;
		}
		if (GamePad.GetState(index).DPad.Down == ButtonState.Pressed)
		{
			speed -= 0.1f;
		}
		if (useKeyboardControls)
		{
			KeyboardState state2 = Keyboard.GetState();
			if (state2.IsKeyDown(Keys.A) && !state2.IsKeyDown(Keys.D))
			{
				angle -= 0.05f;
			}
			if (state2.IsKeyDown(Keys.D) && !state2.IsKeyDown(Keys.A))
			{
				angle += 0.05f;
			}
			if (state2.IsKeyDown(Keys.W) && !state2.IsKeyDown(Keys.S))
			{
				speed += 0.1f;
			}
			if (state2.IsKeyDown(Keys.S) && !state2.IsKeyDown(Keys.W))
			{
				speed -= 0.1f;
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
		speed = MathHelper.Lerp(speed, 0f, 0.005f);
		speed = MathHelper.Clamp(speed, 0f - topSpeed, topSpeed);
		position += Math2.AdvanceAngle(angle, speed);
		if (position.X < (float)(-texture.Width))
		{
			position.X = 1280 + texture.Width;
		}
		if (position.X > (float)(1280 + texture.Width))
		{
			position.X = -texture.Width;
		}
		if (position.Y < 0f)
		{
			position.Y = 800f;
		}
		if (position.Y > 800f)
		{
			position.Y = 0f;
		}
		if (characters[number].ability[0] > 0.5f && currentGamePadState != oldGamePadState && currentGamePadState.Buttons.A == ButtonState.Pressed)
		{
			result = "SHIELD";
		}
		if (characters[number].ability[0] > 0.5f && useKeyboardControls && ((currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyUp(Keys.LeftControl) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || state.RightButton == ButtonState.Pressed))
		{
			result = "SHIELD";
		}
		if (characters[number].shipClass == "Defender")
		{
			shootingType = "REGULAR";
			if (characters[number].ability[1] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.X == ButtonState.Pressed)
			{
				result = "TURRET";
			}
			if (characters[number].ability[2] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.Y == ButtonState.Pressed)
			{
				result = "HIVE";
			}
			if (characters[number].ability[3] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.B == ButtonState.Pressed)
			{
				result = "SANCTUARY";
			}
			if (useKeyboardControls)
			{
				if (characters[number].ability[1] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D1) || state.MiddleButton == ButtonState.Pressed))
				{
					result = "TURRET";
				}
				if (characters[number].ability[2] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D2) || state.XButton1 == ButtonState.Pressed))
				{
					result = "HIVE";
				}
				if (characters[number].ability[3] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D3) || state.XButton2 == ButtonState.Pressed))
				{
					result = "SANCTUARY";
				}
			}
		}
		else
		{
			if (characters[number].ability[1] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.X == ButtonState.Pressed)
			{
				result = "WIDE";
				shootingType = "WIDE";
			}
			if (characters[number].ability[2] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.Y == ButtonState.Pressed)
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
				if (characters[number].ability[1] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D3) || state.XButton2 == ButtonState.Pressed))
				{
					result = "WIDE";
					shootingType = "WIDE";
				}
				if (characters[number].ability[2] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D2) || state.XButton1 == ButtonState.Pressed))
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
		for (int i = 0; i < nRelics; i++)
		{
			if (i < characters[number].ability.Count)
			{
				characters[number].ability[i] = MathHelper.SmoothStep(characters[number].ability[i], 1.1f, 0.05f);
				characters[number].ability[i] = MathHelper.Clamp(characters[number].ability[i], 0f, 1f);
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

	public string UpdateChallenge(int maxX, int maxY, int vibration, bool useKeyboardControls)
	{
		string result = "";
		if (!Active)
		{
			Destroyed = 100f;
		}
		if (Health <= 0f)
		{
			Health = 0f;
			Destroyed++;
			if (Destroyed > 100f)
			{
				Active = false;
			}
			return result;
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
		if (currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.A == ButtonState.Pressed)
		{
			result = "SHIELD";
		}
		if ((currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyUp(Keys.LeftControl) && currentKeyboardState.IsKeyUp(Keys.LeftAlt)) || currentMouseState.RightButton == ButtonState.Pressed)
		{
			result = "SHIELD";
		}
		if (characters[number].shipClass == "Defender")
		{
			shootingType = "REGULAR";
			if (characters[number].ability[1] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.X == ButtonState.Pressed)
			{
				result = "TURRET";
			}
			if (characters[number].ability[2] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.Y == ButtonState.Pressed)
			{
				result = "HIVE";
			}
			if (characters[number].ability[3] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.B == ButtonState.Pressed)
			{
				result = "SANCTUARY";
			}
			if (useKeyboardControls)
			{
				if (characters[number].ability[1] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D1) || currentMouseState.MiddleButton == ButtonState.Pressed))
				{
					result = "TURRET";
				}
				if (characters[number].ability[2] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D2) || currentMouseState.XButton1 == ButtonState.Pressed))
				{
					result = "HIVE";
				}
				if (characters[number].ability[3] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D3) || currentMouseState.XButton2 == ButtonState.Pressed))
				{
					result = "SANCTUARY";
				}
			}
		}
		else
		{
			if (characters[number].ability[1] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.X == ButtonState.Pressed)
			{
				result = "WIDE";
				shootingType = "WIDE";
			}
			if (characters[number].ability[2] > 0.5f && currentGamePadState.Buttons != oldGamePadState.Buttons && currentGamePadState.Buttons.Y == ButtonState.Pressed)
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
				if (characters[number].ability[1] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D3) || currentMouseState.XButton2 == ButtonState.Pressed))
				{
					result = "WIDE";
					shootingType = "WIDE";
				}
				if (characters[number].ability[2] > 0.5f && (currentKeyboardState.IsKeyDown(Keys.D2) || currentMouseState.XButton1 == ButtonState.Pressed))
				{
					result = "LONG";
					shootingType = "LONG";
				}
				if (currentKeyboardState.IsKeyDown(Keys.D1) || currentMouseState.MiddleButton == ButtonState.Pressed)
				{
					result = "NORMAL";
					shootingType = "NORMAL";
				}
			}
		}
		boost = MathHelper.SmoothStep(boost, 1f, 0.4f);
		if ((double)Math.Abs(currentGamePadState.ThumbSticks.Left.X) > 0.1)
		{
			accelerationX2 = currentGamePadState.ThumbSticks.Left.X * 6f * boost;
		}
		if ((double)Math.Abs(currentGamePadState.ThumbSticks.Left.Y) > 0.1)
		{
			accelerationY2 = currentGamePadState.ThumbSticks.Left.Y * -6f * boost;
		}
		if (currentGamePadState.DPad.Left == ButtonState.Pressed)
		{
			accelerationX2 = -6f * boost;
		}
		if (currentGamePadState.DPad.Right == ButtonState.Pressed)
		{
			accelerationX2 = 6f * boost;
		}
		if (currentGamePadState.DPad.Up == ButtonState.Pressed)
		{
			accelerationY2 = -6f * boost;
		}
		if (currentGamePadState.DPad.Down == ButtonState.Pressed)
		{
			accelerationY2 = 6f * boost;
		}
		if (useKeyboardControls)
		{
			KeyboardState state = Keyboard.GetState();
			if (state.IsKeyDown(Keys.A) && !state.IsKeyDown(Keys.D))
			{
				accelerationX2 = -6f * boost;
			}
			if (state.IsKeyDown(Keys.D) && !state.IsKeyDown(Keys.A))
			{
				accelerationX2 = 6f * boost;
			}
			if (state.IsKeyDown(Keys.W) && !state.IsKeyDown(Keys.S))
			{
				accelerationY2 = -6f * boost;
			}
			if (state.IsKeyDown(Keys.S) && !state.IsKeyDown(Keys.W))
			{
				accelerationY2 = 6f * boost;
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
		accelerationX = MathHelper.Lerp(accelerationX, accelerationX2, 0.6f);
		accelerationY = MathHelper.Lerp(accelerationY, accelerationY2, 0.6f);
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
		for (int i = 0; i < nRelics; i++)
		{
			if (i < characters[number].ability.Count)
			{
				characters[number].ability[i] = MathHelper.SmoothStep(characters[number].ability[i], 1.1f, 0.05f);
				characters[number].ability[i] = MathHelper.Clamp(characters[number].ability[i], 0f, 1f);
			}
		}
		return result;
	}

	private void updateLevel()
	{
		if (characters[number].level > 20)
		{
			characters[number].level = 20;
		}
		levelOld = characters[number].level;
		if (characters[number].experience >= characters[number].nextLevel)
		{
			characters[number].experience = 0;
			characters[number].nextLevel = (int)((float)characters[number].nextLevel * 3.1f);
			characters[number].level++;
		}
		if (levelOld != characters[number].level)
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
		damaged = true;
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
			float num3 = MathHelper.Clamp(100f - Destroyed, 0f, 100f) / 100f;
			spriteBatch.Draw(texture, position, null, Color.White * num3, angle, new Vector2(Width / 2, Height / 2), scale, SpriteEffects.None, 0.08f);
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

	public void DrawSelect(SpriteBatch sb, Vector2 pos, Texture2D[] tx, int id, Color col, uint frame)
	{
		this.frame = frame;
		if (!Active)
		{
			try
			{
				sb.DrawString(font, "PRESS", new Vector2(pos.X, pos.Y - 20f), new Color((float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f), 0f, new Vector2(font.MeasureString("PRESS").X / 2f, font.MeasureString("PRESS").Y / 2f), 1f, SpriteEffects.None, 0f);
				string text = "START";
				sb.DrawString(font, text, new Vector2(pos.X, pos.Y + 10f), new Color((float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f), 0f, new Vector2(font.MeasureString(text).X / 2f, font.MeasureString(text).Y / 2f), 1f, SpriteEffects.None, 0f);
				sb.DrawString(font, "TO JOIN", new Vector2(pos.X, pos.Y + 40f), new Color((float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f, (float)(Math.Sin((float)frame / 10f) / 2.0) + 0.5f), 0f, new Vector2(font.MeasureString("TO JOIN").X / 2f, font.MeasureString("TO JOIN").Y / 2f), 1f, SpriteEffects.None, 0f);
			}
			catch
			{
			}
			change = 0f;
		}
		else
		{
			if (id >= 0)
			{
				sb.Draw(tx[id], pos, null, Color.White * change, 0f, new Vector2(tx[id].Width / 2, tx[id].Height / 2), Vector2.One, SpriteEffects.None, 0.5f);
			}
			if (number < 0)
			{
				if (oldDireccion == 0)
				{
					number++;
				}
				else
				{
					number += oldDireccion;
				}
			}
			if (number < 0)
			{
				number = 3;
			}
			if (number > 3)
			{
				number = 0;
			}
			sb.DrawString(font, characters[number].name, new Vector2(pos.X, pos.Y - 205f), new Color((float)(int)col.R / 255f + 0.75f, (float)(int)col.G / 255f + 0.75f, (float)(int)col.B / 255f + 0.75f, 1f), 0f, new Vector2(font.MeasureString(characters[number].name).X / 2f, font.MeasureString(characters[number].name).Y / 2f), 0.9f, SpriteEffects.None, 0.4f);
			sb.DrawString(font, "Level: \n" + characters[number].level + "\n\nAbilities: \n" + abilityNames[0].Substring((1 - (int)characters[number].ability[0]) * abilityNames[0].Length) + "\n" + abilityNames[1].Substring((1 - (int)characters[number].ability[1]) * abilityNames[1].Length) + "\n" + abilityNames[2].Substring((1 - (int)characters[number].ability[2]) * abilityNames[2].Length) + "\n" + abilityNames[3].Substring((1 - (int)characters[number].ability[3]) * abilityNames[3].Length) + "\n\nSpecial: \n" + characters[number].abilityType, new Vector2(pos.X - 75f, pos.Y - 165f), new Color((float)(int)col.R / 255f + 0.75f, (float)(int)col.G / 255f + 0.75f, (float)(int)col.B / 255f + 0.75f, 1f), 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0.4f);
			if (selected)
			{
				draw2d.DrawPixel(sb, new Rectangle((int)pos.X - (int)(font.MeasureString("READY!").X / 2f * 1.5f), (int)pos.Y - (int)(font.MeasureString("READY!").Y / 2f * 1.5f), (int)(font.MeasureString("READY!").X * 1.5f), (int)(font.MeasureString("READY!").Y * 1.5f)), Color.Black, 0.3f);
				sb.DrawString(font, "READY!", new Vector2(pos.X, pos.Y), Color.White, 0f, new Vector2(font.MeasureString("READY!").X / 2f, font.MeasureString("READY!").Y / 2f), 1.5f, SpriteEffects.None, 0.2f);
			}
			sb.DrawString(font, characters[number].shipClass, new Vector2(pos.X, pos.Y + 205f), Color.White, 0f, new Vector2(font.MeasureString(characters[number].shipClass).X / 2f, font.MeasureString(characters[number].shipClass).Y / 2f), 1f, SpriteEffects.None, 0.4f);
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
			sb.Draw(txBar, UIpos + vector2 * 0f, null, new Color(1f, 1f, 1f, 1f) * characters[number].ability[0] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(characters[number].ability[0] * num3 + num2 * 3f, 1f), effects, 0.5f);
			sb.Draw(txBar, UIpos + vector2 * 1f, null, new Color(1f, 1f, 1f, 1f) * characters[number].ability[1] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(characters[number].ability[1] * num3 + num2 * 2f, 1f), effects, 0.5f);
			sb.Draw(txBar, UIpos + vector2 * 2f, null, new Color(1f, 1f, 1f, 1f) * characters[number].ability[2] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(characters[number].ability[2] * num3 + num2 * 1f, 1f), effects, 0.5f);
			sb.Draw(txBar, UIpos + vector2 * 3f, null, new Color(1f, 1f, 1f, 1f) * characters[number].ability[3] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(characters[number].ability[3] * num3 + num2 * 0f, 1f), effects, 0.5f);
			UIpos += new Vector2(0f, 4f);
			if (num == 1)
			{
				vector = new Vector2(-100f, 0f);
			}
			sb.DrawString(font, abilityNames[0], UIpos + vector + vector2 * 0f, new Color(0.4f, 0.8f, 1f, 1f) * characters[number].ability[0] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
			sb.DrawString(font, abilityNames[1], UIpos + vector + vector2 * 1f, new Color(0.4f, 0.8f, 1f, 1f) * characters[number].ability[1] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
			sb.DrawString(font, abilityNames[2], UIpos + vector + vector2 * 2f, new Color(0.4f, 0.8f, 1f, 1f) * characters[number].ability[2] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
			sb.DrawString(font, abilityNames[3], UIpos + vector + vector2 * 3f, new Color(0.4f, 0.8f, 1f, 1f) * characters[number].ability[3] * opac, 0f, new Vector2(txBar.Width, 0f) * num, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
			sb.DrawString(font, "XP " + characters[number].experience + "/" + characters[number].nextLevel, UIposText + vector - new Vector2(0f, num2 * 100f), Color.Cyan * opac, 0f, new Vector2(font.MeasureString("XP " + characters[number].experience + "/" + characters[number].nextLevel).X * (float)num, 0f), 0.75f, SpriteEffects.None, 0f);
			sb.DrawString(font, characters[number].shipClass + " lv " + characters[number].level, UIposText + vector, Color.Cyan * opac, 0f, new Vector2(font.MeasureString(characters[number].shipClass + " " + characters[number].level).X * (float)num, 0f), 0.75f, SpriteEffects.None, 0f);
		}
	}
}
