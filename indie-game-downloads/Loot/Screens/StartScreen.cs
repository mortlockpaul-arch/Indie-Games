using System;
using Eyehook.Framework;
using Loot.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class StartScreen : Screen
{
	private enum Mode
	{
		Eyehook,
		LoadAll,
		Wait,
		CrossFade,
		Ready,
		Start
	}

	private class Blinker
	{
		private TimeSpan timer = TimeSpan.Zero;

		private TimeSpan duration = TimeSpan.FromSeconds(0.75);

		private bool up;

		public float Alpha => (float)(0.25 + 0.75 * timer.TotalSeconds / duration.TotalSeconds);

		public void Update(GameTime gameTime)
		{
			if (up)
			{
				timer += gameTime.ElapsedGameTime;
				if (timer >= duration)
				{
					timer = duration;
					up = false;
				}
			}
			else
			{
				timer -= gameTime.ElapsedGameTime;
				if (timer <= TimeSpan.Zero)
				{
					timer = TimeSpan.Zero;
					up = true;
				}
			}
		}
	}

	private Mode mode;

	private Particle sword;

	private int swordSpeed = 180;

	private Blinker blinker = new Blinker();

	private PlayerIndex? playerIndex;

	private bool initialized;

	private bool music;

	private ContentManager tmpContent;

	private StillSprite eyehookGames;

	private StillSprite loadingSprite;

	private StillSprite pressStartSprite;

	private SpriteFont font;

	private StillSprite titleThorns;

	private StillSprite titleText;

	private StillSprite titleSword;

	private TimeSpan startTime;

	private TimeSpan crossFadeTimer;

	private readonly TimeSpan waitDuration = TimeSpan.FromSeconds(4.0);

	private readonly TimeSpan crossFadeDuration = TimeSpan.FromSeconds(1.0);

	private Vector2 titleTextPos = new Vector2(640f, 320f);

	private Vector2 subTextPos = new Vector2(640f, 400f);

	private Color startColor = new Color(255, 255, 255);

	public StartScreen()
		: base(modal: true)
	{
		mode = Mode.Eyehook;
		resetSword();
	}

	private void resetSword()
	{
		sword.pos = new Vector2(640f, -512f);
		sword.vel = new Vector2(0f, swordSpeed);
		sword.rotation = (float)Math.PI;
	}

	public override void loadContent(ContentManager content)
	{
		tmpContent = new ContentManager(MC.Game.Services, "Content");
		eyehookGames = new StillSprite(tmpContent.Load<Texture2D>("Sprites\\TitleScreen\\EyehookGames"));
		loadingSprite = new StillSprite(tmpContent.Load<Texture2D>("Sprites\\TitleScreen\\Loading"));
		pressStartSprite = new StillSprite(tmpContent.Load<Texture2D>("Sprites\\TitleScreen\\PressStart"));
		font = tmpContent.Load<SpriteFont>("Fonts\\Miramonte16");
	}

	private void loadTitleContent()
	{
		titleThorns = new StillSprite(tmpContent.Load<Texture2D>("Sprites\\TitleScreen\\Thorns"));
		titleText = new StillSprite(tmpContent.Load<Texture2D>("Sprites\\TitleScreen\\TitleText"));
		titleSword = new StillSprite(tmpContent.Load<Texture2D>("Sprites\\TitleScreen\\TitleSword"));
	}

	public override void unloadContent()
	{
		tmpContent.Dispose();
	}

	public override void update(GameTime gameTime)
	{
		if (mode == Mode.Eyehook)
		{
			startTime = gameTime.TotalGameTime;
			return;
		}
		if (mode == Mode.LoadAll)
		{
			loadTitleContent();
			StaticLoader.LoadAllContent();
			mode = Mode.Wait;
			return;
		}
		if (mode == Mode.Wait)
		{
			if (gameTime.TotalGameTime - startTime >= waitDuration || GamePadManager.scanForButtonDown(Buttons.A).HasValue || GamePadManager.scanForButtonDown(Buttons.Start).HasValue)
			{
				crossFadeTimer = TimeSpan.Zero;
				mode = Mode.CrossFade;
			}
			return;
		}
		if (mode == Mode.CrossFade)
		{
			crossFadeTimer += gameTime.ElapsedGameTime;
			if (crossFadeTimer > crossFadeDuration)
			{
				mode = Mode.Ready;
			}
			return;
		}
		if (!music)
		{
			BgMusic.TitleMusic();
			music = true;
		}
		updateUI(gameTime);
		if (!playerIndex.HasValue)
		{
			playerIndex = GamePadManager.scanForButtonDown(Buttons.Start);
		}
		if (playerIndex.HasValue && !GamerManager.HasStorageAccess(playerIndex.Value))
		{
			playerIndex = null;
			MC.ScreenManager.addScreen(new SignInScreen());
		}
		else if (playerIndex.HasValue && !initialized)
		{
			MC.InitializePlayer(playerIndex.Value);
			initialized = true;
			mode = Mode.Start;
		}
		else if (initialized && MC.StorageManager.Initialized)
		{
			Profile.Load();
			HighScores.Load();
			MC.Game.ResetElapsedTime();
			MC.ScreenManager.removeAllScreens();
			MC.ScreenManager.addScreen(new MainMenu());
		}
	}

	private static void reset()
	{
		MC.Reset();
		MC.Game.ResetGame();
		MC.Game.DisplayStartScreen();
	}

	private void updateUI(GameTime gameTime)
	{
		blinker.Update(gameTime);
		sword.pos += sword.vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (!((double)sword.pos.Y <= 1232.0))
		{
			resetSword();
		}
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin(SpriteSortMode.Immediate, null, GraphicUtil.PointFilter, null, null);
		if (mode == Mode.Eyehook)
		{
			eyehookGames.Draw(base.spriteBatch, new Vector2(640f, 360f));
			loadingSprite.Draw(base.spriteBatch, new Vector2(640f, 480f), Color.White * 0.25f, 1f);
			mode = Mode.LoadAll;
		}
		else if (mode == Mode.LoadAll)
		{
			eyehookGames.Draw(base.spriteBatch, new Vector2(640f, 360f));
		}
		else if (mode == Mode.Wait)
		{
			eyehookGames.Draw(base.spriteBatch, new Vector2(640f, 360f));
		}
		else if (mode == Mode.CrossFade)
		{
			eyehookGames.Draw(base.spriteBatch, new Vector2(640f, 360f), Color.White * (float)(1.0 - crossFadeTimer.TotalSeconds / crossFadeDuration.TotalSeconds), 1f);
			Color color = Color.White * (float)(crossFadeTimer.TotalSeconds / crossFadeDuration.TotalSeconds);
			titleThorns.Draw(base.spriteBatch, new Vector2(640f, 360f), color, 4f);
			titleText.Draw(base.spriteBatch, titleTextPos, color, 1f);
		}
		else
		{
			titleThorns.Draw(base.spriteBatch, new Vector2(640f, 360f), Color.White, 4f);
			titleSword.Rotation = sword.rotation;
			titleSword.Draw(base.spriteBatch, sword.pos, Color.White, 4f);
			titleText.Draw(base.spriteBatch, titleTextPos);
			if (mode == Mode.Ready)
			{
				startColor = Color.White * blinker.Alpha;
				pressStartSprite.Draw(base.spriteBatch, subTextPos, startColor, 1f);
			}
			else if (mode == Mode.Start)
			{
				loadingSprite.Draw(base.spriteBatch, subTextPos);
			}
		}
		base.spriteBatch.End();
	}
}
