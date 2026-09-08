using System;
using System.Collections.Generic;
using Eyehook.Framework;
using Loot.Core;
using Loot.PC;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class HighScoreScreen : Screen
{
	private Difficulty mode;

	private Vector2 scrollPos = new Vector2(1280f, 360f);

	private Rectangle scrollMask = new Rectangle(1280, 0, 0, 640);

	private bool open;

	private bool close;

	private float delta;

	private static Sprite scroll;

	private static Sprite modeBox;

	private static Texture2D background;

	private static Texture2D pixel;

	private static Sprite berserker;

	private static Sprite gambler;

	private static Sprite shaman;

	private static Sprite tinkerer;

	private static Sprite goblin;

	private static Sprite peasant;

	private Color bgColor = MC.ScreenManager.BackgroundColor;

	private Vector2 modePos = new Vector2(640f, 88f);

	private Vector2 topLeft = new Vector2(190f, 140f);

	private Vector2 dx0 = new Vector2(120f, 0f);

	private Vector2 dx1 = new Vector2(384f, 0f);

	private Vector2 dx2 = new Vector2(144f, 0f);

	private Vector2 dx3 = new Vector2(206f, 0f);

	private Vector2 dy1 = new Vector2(0f, 40f);

	private string s0 = "Rank";

	private string s1 = "Name";

	private string s2 = "Depth";

	private string s3 = "Time";

	private string s4 = "Level";

	private Color titleColor = new Color(102, 68, 0);

	private Color entryColor = new Color(152, 101, 0);

	private string winText = "-WIN-";

	private Vector2 entryOffset = new Vector2(0f, 40f);

	public HighScoreScreen()
		: base(modal: true)
	{
		open = true;
		if (HighScores.HardEntries.Count > 0)
		{
			mode = Difficulty.Hard;
		}
		else if (HighScores.NormalEntries.Count > 0)
		{
			mode = Difficulty.Normal;
		}
		else if (HighScores.EasyEntries.Count > 0)
		{
			mode = Difficulty.Easy;
		}
		else
		{
			mode = Difficulty.Normal;
		}
	}

	public HighScoreScreen(Difficulty difficulty)
		: base(modal: true)
	{
		open = true;
		mode = difficulty;
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.A) || MC.GamePadManager.isNewButtonDown(Buttons.B))
		{
			open = false;
			close = true;
		}
		if (open)
		{
			delta += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if ((double)delta >= 1.0)
			{
				delta = 1f;
				open = false;
			}
		}
		if (close)
		{
			delta -= (float)gameTime.ElapsedGameTime.TotalSeconds;
			if ((double)delta <= 0.0)
			{
				MC.ScreenManager.removeScreen(this);
				return;
			}
		}
		int num = (int)((1.0 - (double)delta) * 1132.0 + 148.0);
		scrollPos.X = num;
		scrollMask = new Rectangle(num, 0, 1280 - num, 640);
		if (MC.GamePadManager.isNewButtonDown(Buttons.LeftShoulder))
		{
			switch (mode)
			{
			case Difficulty.Easy:
				mode = Difficulty.Hard;
				break;
			case Difficulty.Normal:
				mode = Difficulty.Easy;
				break;
			case Difficulty.Hard:
				mode = Difficulty.Normal;
				break;
			}
		}
		if (MC.GamePadManager.isNewButtonDown(Buttons.RightShoulder))
		{
			switch (mode)
			{
			case Difficulty.Easy:
				mode = Difficulty.Normal;
				break;
			case Difficulty.Normal:
				mode = Difficulty.Hard;
				break;
			case Difficulty.Hard:
				mode = Difficulty.Easy;
				break;
			}
		}
	}

	public static void Load(ContentManager content)
	{
		scroll = new StillSprite(content.Load<Texture2D>("Sprites\\HighScore\\Scroll"));
		modeBox = new StillSprite(content.Load<Texture2D>("Sprites\\HighScore\\ModeBox"));
		background = content.Load<Texture2D>("Sprites\\HighScore\\Background");
		pixel = content.Load<Texture2D>("Sprites\\Pixel");
		berserker = new StillSprite(content.Load<Texture2D>("Sprites\\HighScore\\Berserker"));
		gambler = new StillSprite(content.Load<Texture2D>("Sprites\\HighScore\\Gambler"));
		shaman = new StillSprite(content.Load<Texture2D>("Sprites\\HighScore\\Shaman"));
		tinkerer = new StillSprite(content.Load<Texture2D>("Sprites\\HighScore\\Tinkerer"));
		goblin = new StillSprite(content.Load<Texture2D>("Sprites\\HighScore\\Goblin"));
		peasant = new StillSprite(content.Load<Texture2D>("Sprites\\HighScore\\Peasant"));
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		base.spriteBatch.Draw(pixel, base.viewportRect, bgColor * delta);
		drawMode(base.spriteBatch);
		base.spriteBatch.End();
		drawMask(base.spriteBatch);
		drawBody(base.spriteBatch);
		base.spriteBatch.Begin();
		scroll.Draw(base.spriteBatch, scrollPos);
		base.spriteBatch.End();
	}

	private void drawMode(SpriteBatch spriteBatch)
	{
		if (!((double)delta < 0.75))
		{
			Color color = new Color(151, 100, 0);
			Vector2 vector = new Vector2(0f, (float)(44.0 * (1.0 - ((double)delta - 0.75) * 4.0)));
			modeBox.Draw(spriteBatch, modePos + vector);
			switch (mode)
			{
			case Difficulty.Easy:
				Text.DrawCentered(spriteBatch, modePos + vector, "Easy Mode", color);
				break;
			case Difficulty.Normal:
				Text.DrawCentered(spriteBatch, modePos + vector, "Normal Mode", color);
				break;
			case Difficulty.Hard:
				Text.DrawCentered(spriteBatch, modePos + vector, "Hard Mode", color);
				break;
			}
		}
	}

	private void drawMask(SpriteBatch spriteBatch)
	{
		spriteBatch.GraphicsDevice.Clear(ClearOptions.Stencil, Color.Black, 0f, 0);
		spriteBatch.Begin(SpriteSortMode.Immediate, GraphicUtil.NoColorWrite, null, GraphicUtil.Stencil1Always, null, GraphicUtil.AlphaGreaterThan0);
		spriteBatch.Draw(pixel, scrollMask, Color.White);
		spriteBatch.End();
	}

	private void drawBody(SpriteBatch spriteBatch)
	{
		spriteBatch.Begin(SpriteSortMode.Immediate, null, null, GraphicUtil.Stencil1Match, null);
		spriteBatch.Draw(background, base.viewportRect, Color.White);
		Vector2 vector = topLeft;
		Text.Draw(spriteBatch, vector, s0, titleColor);
		Vector2 vector2 = vector + dx0;
		Text.Draw(spriteBatch, vector2, s1, titleColor);
		Vector2 vector3 = vector2 + dx1;
		Text.Draw(spriteBatch, vector3, s2, titleColor);
		Vector2 vector4 = vector3 + dx2;
		Text.Draw(spriteBatch, vector4, s3, titleColor);
		Vector2 v = vector4 + dx3;
		Text.Draw(spriteBatch, v, s4, titleColor);
		List<HighScores.Entry> list = mode switch
		{
			Difficulty.Easy => HighScores.EasyEntries, 
			Difficulty.Normal => HighScores.NormalEntries, 
			Difficulty.Hard => HighScores.HardEntries, 
			_ => throw new Exception("Unknown mode: " + mode), 
		};
		for (int i = 0; i < list.Count; i++)
		{
			drawEntry(i + 1, list[i]);
		}
		spriteBatch.End();
	}

	private void drawEntry(int rank, HighScores.Entry entry)
	{
		Vector2 vector = topLeft + entryOffset + dy1 * rank;
		Text.Draw(base.spriteBatch, vector, romanNumeral(rank), entryColor);
		Vector2 vector2 = vector + dx0;
		Text.Draw(base.spriteBatch, vector2, entry.Tag, entryColor);
		Vector2 vector3 = vector2 + dx1;
		if (entry.Depth >= 51)
		{
			Text.Draw(base.spriteBatch, vector3, winText, entryColor);
		}
		else
		{
			Text.Draw(base.spriteBatch, vector3, entry.Depth, entryColor);
		}
		Vector2 vector4 = vector3 + dx2;
		Clock.Draw(base.spriteBatch, vector4, TimeSpan.FromSeconds(entry.PlayTime), entryColor);
		Vector2 vector5 = vector4 + dx3;
		GetSprite((PlayerClassType)entry.PlayerClass).Draw(base.spriteBatch, vector5 + new Vector2(10f, 0f));
		Text.Draw(base.spriteBatch, vector5 + new Vector2(46f, 0f), entry.Level, entryColor);
	}

	private Sprite GetSprite(PlayerClassType pct)
	{
		return pct switch
		{
			PlayerClassType.Berserker => berserker, 
			PlayerClassType.Shaman => shaman, 
			PlayerClassType.Tinkerer => tinkerer, 
			PlayerClassType.Gambler => gambler, 
			PlayerClassType.Goblin => goblin, 
			PlayerClassType.Peasant => peasant, 
			_ => throw new Exception("Unknown PlayerClassType: " + pct), 
		};
	}

	private string romanNumeral(int i)
	{
		return i switch
		{
			1 => "I", 
			2 => "II", 
			3 => "III", 
			4 => "IV", 
			5 => "V", 
			6 => "VI", 
			7 => "VII", 
			8 => "VIII", 
			9 => "IX", 
			10 => "X", 
			_ => "?", 
		};
	}
}
