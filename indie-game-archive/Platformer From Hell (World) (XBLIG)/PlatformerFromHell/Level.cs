using System;
using System.Collections.Generic;
using Containers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlatformerFromHell.Asset_Classes;

namespace PlatformerFromHell;

internal class Level : IDisposable
{
	private const int EntityLayer = 2;

	private const int PointsPerSecond = 5;

	public PlatformerGame platformerGame;

	private LevelLoader loader;

	public int levelWidth;

	public int levelHeight;

	public int levelTime;

	public int levelNumber;

	public int worldNumber;

	public bool finishedLoading;

	public int testScore;

	public float Zoom = 1f;

	public bool firstLoad = true;

	public int deaths;

	private Layer[] layers;

	public int left;

	public int right;

	public int top;

	public int bottom;

	private float cameraPosition;

	private float cameraPositionY;

	public bool sliding;

	public bool moneyGrabbed = false;

	public int deathTimer = 0;

	private Player player;

	public List<Asset> assets = new List<Asset>();

	public XHashSet<Asset>[,] zonePanels;

	public Vector2 start;

	private Point exit = InvalidPosition;

	private static readonly Point InvalidPosition = new Point(-1, -1);

	private Random random = new Random(354668);

	public int score;

	public bool ReachedExit;

	public TimeSpan timeRemaining;

	private ContentManager content;

	private Rectangle playerSquare = default(Rectangle);

	private List<Asset> assets2 = new List<Asset>();

	private float topMargin = 0.2f;

	private float bottomMargin = 0.2f;

	public Player Player => player;

	public int Score => score;

	public TimeSpan TimeRemaining => timeRemaining;

	public ContentManager Content => content;

	public Level(IServiceProvider serviceProvider, string fileStream, int levelIndex, int points, int deaths, int worldNumber, PlatformerGame platformerGame)
	{
		this.platformerGame = platformerGame;
		platformerGame.GameState = PlatformerGame.GameStates.Loading;
		finishedLoading = false;
		platformerGame.levelfinishedLoading = false;
		content = new ContentManager(serviceProvider, "Content");
		timeRemaining = TimeSpan.FromMinutes(5.0);
		levelNumber = levelIndex;
		this.worldNumber = worldNumber;
		layers = new Layer[3];
		layers[0] = new Layer(Content, "Layer0", 0.2f, worldNumber);
		layers[1] = new Layer(Content, "Layer1", 0.5f, worldNumber);
		layers[2] = new Layer(Content, "Layer2", 0.8f, worldNumber);
		score = points;
		this.deaths = deaths;
		LoadLevel(fileStream);
		firstLoad = false;
		testScore = score;
		finishedLoading = true;
		platformerGame.levelfinishedLoading = true;
		player.moving = true;
		player.movingTimer = 50f;
	}

	public void LoadLevel(string fileName)
	{
		loader = new LevelLoader(this, fileName);
		levelWidth = loader.levelWidth;
		levelHeight = loader.levelHeight;
		levelTime = loader.levelTime;
		assets = loader.levelAssets;
		zonePanels = loader.zonePanels;
		InitPlayer(loader.startLocation);
		moneyGrabbed = false;
		platformerGame.GraphicsDevice.Clear(Color.Black);
		platformerGame.GameState = PlatformerGame.GameStates.Normal;
	}

	public void InitPlayer(Vector2 loc)
	{
		start = loc;
		start.X += 17f;
		player = new Player(this, start);
	}

	public void Dispose()
	{
		Content.Unload();
		assets.Clear();
		int num = levelWidth / 100 + 1;
		int num2 = levelHeight / 100 + 1;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				zonePanels[i, j].Clear();
			}
		}
		Asset.StaticDispose();
		loader.Dispose();
		Console.Out.WriteLine("Disposed");
	}

	public void Update(GameTime gameTime, KeyboardState keyboardState, GamePadState gamePadState)
	{
		if (player.leftSlide || player.rightSlide || player.upSlide || player.downSlide)
		{
			sliding = true;
		}
		else if (player.isMoving || !player.isOnGround)
		{
			sliding = false;
		}
		if (!player.IsAlive)
		{
			deathTimer++;
		}
		if (Player.IsAlive)
		{
			if (ReachedExit)
			{
				int val = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0);
				val = Math.Min(val, (int)Math.Ceiling(TimeRemaining.TotalSeconds));
				timeRemaining -= TimeSpan.FromSeconds(val);
				score += val;
			}
			else
			{
				timeRemaining -= gameTime.ElapsedGameTime;
				Player.Update(gameTime, keyboardState, gamePadState);
				foreach (Asset asset in assets)
				{
					asset.Update(gameTime);
				}
			}
		}
		if (timeRemaining < TimeSpan.Zero)
		{
			timeRemaining = TimeSpan.Zero;
		}
	}

	private void OnPlayerKilled(string killer)
	{
		Player.OnKilled(killer);
	}

	public void OnExitReached()
	{
		Player.OnReachedExit();
		ReachedExit = true;
		if (moneyGrabbed)
		{
			Program.game.gotMoney();
			Console.Out.WriteLine("LEVEL Money got!!!!!!");
		}
	}

	public void StartNewLife()
	{
		foreach (Asset asset in assets)
		{
			if (asset is Platform)
			{
				asset.Flip(asset.originalFlip);
			}
		}
		player.startTimer = 45f;
		Player.Reset(start);
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		spriteBatch.Begin();
		for (int i = 0; i <= 2; i++)
		{
			layers[i].Draw(spriteBatch, cameraPosition, cameraPositionY);
		}
		spriteBatch.End();
		if (player.isAlive)
		{
			ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
		}
		Matrix transformMatrix = Matrix.CreateTranslation(0f - cameraPosition, 0f - cameraPositionY, 0f) * Matrix.CreateScale(Zoom, Zoom, 1f);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, transformMatrix);
		left = (int)cameraPosition;
		right = left + 1280;
		top = (int)cameraPositionY;
		bottom = top + 720;
		for (int i = 0; i < assets.Count; i++)
		{
			Asset asset = assets[i];
			if (asset.Position.X + (float)asset.frameWidth >= (float)left && asset.Position.X <= (float)right && asset.Position.Y + (float)asset.frameHeight >= (float)top && asset.Position.Y <= (float)bottom)
			{
				asset.Draw(gameTime, spriteBatch);
			}
		}
		Player.Draw(gameTime, spriteBatch);
		spriteBatch.End();
		spriteBatch.Begin();
		for (int i = 3; i < layers.Length; i++)
		{
			layers[i].Draw(spriteBatch, cameraPosition, cameraPositionY);
		}
		spriteBatch.End();
	}

	private void ScrollCamera(Viewport viewport)
	{
		if (sliding)
		{
			float num = 0f;
			float num2 = 0f;
			if (player.leftSlide)
			{
				num = -6f;
			}
			if (player.rightSlide)
			{
				num = 6f;
			}
			if (player.upSlide)
			{
				num2 = -6f;
			}
			if (player.downSlide)
			{
				num2 = 6f;
			}
			cameraPositionY += num2;
			cameraPosition += num;
		}
		else if (player.moving)
		{
			float num3 = (float)viewport.Width / Zoom * 0.45f;
			float num4 = cameraPosition + num3;
			float num5 = cameraPosition + (float)viewport.Width / Zoom - num3;
			float num = 0f;
			if (Player.Position.X < num4)
			{
				num = Player.Position.X - num4;
			}
			else if (Player.Position.X > num5)
			{
				num = Player.Position.X - num5;
			}
			if (Player.Velocity.Y == 550f && !Player.isJumping && Player.currentGravity == Gravity.GravDir.Down)
			{
				bottomMargin = Math.Min(bottomMargin + 0.015f, 0.49f);
			}
			else
			{
				bottomMargin = 0.2f;
			}
			if (Player.Velocity.Y == -550f && !Player.isJumping && Player.currentGravity == Gravity.GravDir.Up)
			{
				topMargin = Math.Min(topMargin + 0.015f, 0.49f);
			}
			else
			{
				topMargin = 0.2f;
			}
			float num6 = cameraPositionY + (float)viewport.Height / Zoom * topMargin;
			float num7 = cameraPositionY + (float)viewport.Height / Zoom - (float)viewport.Height / Zoom * bottomMargin;
			float num2 = 0f;
			if (Player.Position.Y < num6)
			{
				num2 = Player.Position.Y - num6;
			}
			else if (Player.Position.Y > num7)
			{
				num2 = Player.Position.Y - num7;
			}
			cameraPositionY += num2;
			cameraPosition += num;
		}
		ClampCamera(viewport);
	}

	public List<Asset> getAssetsInPlayerSquare(Player p)
	{
		playerSquare.X = (int)Math.Min(p.Position.X, levelWidth);
		playerSquare.Y = (int)Math.Max(p.Position.Y, 0f);
		playerSquare.Height = p.personTexture.Width;
		playerSquare.Width = p.personTexture.Height;
		assets2.Clear();
		int num = playerSquare.Left / 100;
		int num2 = playerSquare.Right / 100;
		int num3 = playerSquare.Top / 100;
		int num4 = playerSquare.Bottom / 100;
		for (int num5 = num2; num5 >= num; num5--)
		{
			for (int num6 = num4; num6 >= num3; num6--)
			{
				foreach (Asset item in zonePanels[num5, num6])
				{
					assets2.Add(item);
				}
			}
		}
		return assets2;
	}

	private void ClampCamera(Viewport viewport)
	{
		cameraPositionY = MathHelper.Clamp(cameraPositionY, 0f, (float)levelHeight - (float)viewport.Height / Zoom);
		cameraPosition = MathHelper.Clamp(cameraPosition, 0f, (float)levelWidth - (float)viewport.Width / Zoom);
	}
}
