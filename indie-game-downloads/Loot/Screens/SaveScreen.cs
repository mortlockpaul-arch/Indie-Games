using System;
using Eyehook.Framework;
using Loot.Core;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Screens;

public class SaveScreen : Screen
{
	private enum Mode
	{
		New,
		Depth,
		Quit,
		GameOver
	}

	private enum State
	{
		FadeIn,
		Save,
		FadeOut
	}

	private Mode mode;

	private State state;

	private bool hasDrawnSave;

	private bool hasSaved;

	private int destDepth;

	private DM.LocationDelegate playerLoc;

	private DM.EnterCallback onEnter;

	private int gameOverRank;

	private static StretchyTexture scroll;

	private float alpha;

	private TimeSpan fadeTimer = TimeSpan.Zero;

	private TimeSpan fadeDurartion = TimeSpan.FromMilliseconds(500.0);

	private TimeSpan saveDuration = TimeSpan.FromMilliseconds(500.0);

	private DateTime startSaveTime;

	public static void NewGame(int destDepth, DM.LocationDelegate playerLoc, DM.EnterCallback onEnter)
	{
		MC.ScreenManager.addScreen(new SaveScreen(Mode.New, destDepth, playerLoc, onEnter));
	}

	public static void GoToDepth(int destDepth, DM.LocationDelegate playerLoc)
	{
		MC.ScreenManager.addScreen(new SaveScreen(Mode.Depth, destDepth, playerLoc));
	}

	public static void GoToDepth(int destDepth, DM.LocationDelegate playerLoc, DM.EnterCallback onEnter)
	{
		MC.ScreenManager.addScreen(new SaveScreen(Mode.Depth, destDepth, playerLoc, onEnter));
	}

	public static void SaveAndQuit()
	{
		MC.ScreenManager.addScreen(new SaveScreen(Mode.Quit));
	}

	public static void GameOver()
	{
		MC.ScreenManager.addScreen(new SaveScreen(Mode.GameOver));
	}

	private SaveScreen(Mode mode)
		: base(modal: true)
	{
		init(mode, 0, null, null);
	}

	private SaveScreen(Mode mode, int destDepth, DM.LocationDelegate playerLoc)
		: base(modal: true)
	{
		init(mode, destDepth, playerLoc, null);
	}

	private SaveScreen(Mode mode, int destDepth, DM.LocationDelegate playerLoc, DM.EnterCallback onEnter)
		: base(modal: true)
	{
		init(mode, destDepth, playerLoc, onEnter);
	}

	private void init(Mode mode, int destDepth, DM.LocationDelegate playerLoc, DM.EnterCallback onEnter)
	{
		state = State.FadeIn;
		hasDrawnSave = false;
		hasSaved = false;
		this.mode = mode;
		this.destDepth = destDepth;
		this.playerLoc = playerLoc;
		this.onEnter = onEnter;
		switch (mode)
		{
		case Mode.New:
			SkipFadeIn();
			break;
		case Mode.Depth:
			BgMusic.NextIfTime();
			break;
		}
	}

	private void SkipFadeIn()
	{
		alpha = 1f;
		startSaveTime = DateTime.Now;
		state = State.Save;
	}

	public static void Load(ContentManager content)
	{
		scroll = new StretchyTexture(content.Load<Texture2D>("Sprites\\UI\\Body"), new Rectangle(64, 64, 128, 128));
	}

	public override void update(GameTime gameTime)
	{
		if (state == State.FadeIn)
		{
			fadeTimer += gameTime.ElapsedGameTime;
			if (fadeTimer > fadeDurartion)
			{
				alpha = 1f;
				startSaveTime = DateTime.Now;
				state = State.Save;
			}
			else
			{
				alpha = (float)(fadeTimer.TotalSeconds / 0.5);
			}
		}
		else if (state == State.Save)
		{
			if (!hasDrawnSave)
			{
				return;
			}
			if (!hasSaved)
			{
				if (mode == Mode.New)
				{
					DM.SaveNewGame(destDepth, playerLoc);
				}
				else if (mode == Mode.Depth)
				{
					DM.GoToDepth(destDepth, playerLoc);
				}
				else if (mode == Mode.Quit)
				{
					DM.SaveAndQuit();
				}
				else
				{
					if (mode != Mode.GameOver)
					{
						throw new Exception("Unknown mode: " + mode);
					}
					gameOverRank = DM.GameOver();
				}
				hasSaved = true;
			}
			else if (DateTime.Now - startSaveTime > saveDuration)
			{
				state = State.FadeOut;
			}
		}
		else
		{
			if (state != State.FadeOut)
			{
				return;
			}
			MC.ScreenManager.removeAllScreens();
			if (mode == Mode.New || mode == Mode.Depth)
			{
				MC.ScreenManager.addScreen(new DungeonView());
				DM.OnEnterDepth(destDepth, onEnter);
			}
			else if (mode == Mode.Quit)
			{
				MC.ScreenManager.addScreen(new MainMenu());
			}
			else
			{
				if (mode != Mode.GameOver)
				{
					throw new Exception("Unknown mode: " + mode);
				}
				MC.ScreenManager.addScreen(new GameOverScreen(gameOverRank));
			}
			MC.ScreenManager.addScreen(new FadeInScreen(fadeDurartion, MC.ScreenManager.BackgroundColor, null));
		}
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		Pixel.Draw(base.spriteBatch, base.viewportRect, MC.ScreenManager.BackgroundColor * alpha);
		if (state == State.Save)
		{
			scroll.Draw(base.spriteBatch, new Rectangle(340, 310, 600, 100));
			if (mode == Mode.Depth)
			{
				Text.DrawCentered(base.spriteBatch, new Vector2(640f, 348f), "Dungeon Level " + destDepth, Color.Black);
				Text.Draw(base.spriteBatch, new Vector2(935f, 625f), "Saving...", new Color(192, 192, 192));
			}
			else
			{
				Text.DrawCentered(base.spriteBatch, new Vector2(640f, 348f), "Saving...", Color.Black);
			}
			hasDrawnSave = true;
		}
		base.spriteBatch.End();
	}
}
