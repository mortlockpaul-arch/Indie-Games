using Eyehook.Framework;
using Loot.Core;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class PauseScreen : MenuBoxScreen
{
	private class Resume : MenuItem
	{
		private Screen parent;

		public override string Name => "Resume";

		public override Align Align => Align.Center;

		public Resume(Screen parent)
		{
			this.parent = parent;
		}

		public override void Click()
		{
			PlaySound.MenuClick();
			MC.ScreenManager.removeScreen(parent);
		}
	}

	private class Awardments : MenuItem
	{
		private Screen parent;

		public override string Name => "Awardments";

		public override Align Align => Align.Center;

		public Awardments(Screen parent)
		{
			this.parent = parent;
		}

		public override void Click()
		{
			PlaySound.MenuClick();
			MC.ScreenManager.removeScreen(parent);
			MC.ScreenManager.addScreen(new AwardmentScreen());
		}
	}

	private class Controls : MenuItem
	{
		private Screen parent;

		public override string Name => "Controls";

		public override Align Align => Align.Center;

		public Controls(Screen parent)
		{
			this.parent = parent;
		}

		public override void Click()
		{
			PlaySound.MenuClick();
			MC.ScreenManager.removeScreen(parent);
			MC.ScreenManager.addScreen(new ControlsScreen());
		}
	}

	private class Options : MenuItem
	{
		private Screen parent;

		public override string Name => "Options";

		public override Align Align => Align.Center;

		public Options(Screen parent)
		{
			this.parent = parent;
		}

		public override void Click()
		{
			PlaySound.MenuClick();
			MC.ScreenManager.removeScreen(parent);
			MC.ScreenManager.addScreen(new OptionsScreen());
		}
	}

	private class HelpItem : MenuItem
	{
		private Screen parent;

		public override string Name => "Help";

		public override Align Align => Align.Center;

		public HelpItem(Screen parent)
		{
			this.parent = parent;
		}

		public override void Click()
		{
			PlaySound.MenuClick();
			MC.ScreenManager.removeScreen(parent);
			HelpScreen.Display();
		}
	}

	private class SaveItem : MenuItem
	{
		public override string Name => "Save & Quit";

		public override Align Align => Align.Center;

		public override void Click()
		{
			PlaySound.MenuClick();
			SaveScreen.SaveAndQuit();
		}
	}

	private Vector2 timeBoxTextPos;

	private Rectangle timeBoxRect;

	private Vector2 clockPos;

	private Vector2 difficultyPos;

	private Rectangle killBoxRect;

	private Vector2 killBoxTextPos;

	private static StretchyTexture stretchyBox;

	private static Sprite clock;

	private static Sprite difficultyEasy;

	private static Sprite difficultyNormal;

	private static Sprite difficultyEpic;

	private static Sprite bloodKillCount;

	private Color dim = Color.Black * 0.25f;

	private Color killColor = new Color(176, 0, 0);

	private Vector2 bloodPos = new Vector2(1002f, 460f);

	public PauseScreen()
		: base("Game Paused", 416, 0)
	{
		AddMenuItem(new Resume(this));
		AddMenuItem(new Awardments(this));
		AddMenuItem(new Controls(this));
		AddMenuItem(new Options(this));
		AddMenuItem(new HelpItem(this));
		AddMenuItem(new SaveItem());
		BoxRect.Y -= 40;
		timeBoxRect = new Rectangle(BoxRect.Left, BoxRect.Bottom + 4, BoxRect.Width, 80);
		timeBoxTextPos = new Vector2(timeBoxRect.Center.X, timeBoxRect.Center.Y);
		clockPos = new Vector2(timeBoxRect.Right - 58, timeBoxRect.Center.Y + 2);
		difficultyPos = new Vector2(timeBoxRect.Left + 58, timeBoxRect.Center.Y);
		killBoxTextPos = new Vector2(1002f, 318f);
		killBoxRect = new Rectangle(852, 278, 300, 120);
	}

	public static void Load(ContentManager content)
	{
		stretchyBox = new StretchyTexture(content.Load<Texture2D>("Sprites\\UI\\MenuBox"), new Rectangle(16, 16, 96, 96));
		clock = new StillSprite(content.Load<Texture2D>("Sprites\\UI\\Clock"));
		Texture2D texture = content.Load<Texture2D>("Sprites\\UI\\Difficulty");
		difficultyEasy = new StillSprite(texture, new Rectangle(0, 0, 128, 128), new Vector2(64f, 64f));
		difficultyNormal = new StillSprite(texture, new Rectangle(128, 0, 128, 128), new Vector2(64f, 64f));
		difficultyEpic = new StillSprite(texture, new Rectangle(256, 0, 128, 128), new Vector2(64f, 64f));
		bloodKillCount = new StillSprite(content.Load<Texture2D>("Sprites\\UI\\BloodKillCount"));
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.B))
		{
			PlaySound.MenuCancel();
			MC.ScreenManager.removeScreen(this);
		}
		else
		{
			base.update(gameTime);
		}
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		Pixel.Draw(base.spriteBatch, base.viewportRect, dim);
		base.spriteBatch.End();
		base.draw(gameTime);
		base.spriteBatch.Begin();
		stretchyBox.Draw(base.spriteBatch, timeBoxRect);
		switch (DM.GameOptions.Difficulty)
		{
		case Difficulty.Easy:
			difficultyEasy.Draw(base.spriteBatch, difficultyPos);
			break;
		case Difficulty.Normal:
			difficultyNormal.Draw(base.spriteBatch, difficultyPos);
			break;
		case Difficulty.Hard:
			difficultyEpic.Draw(base.spriteBatch, difficultyPos);
			break;
		}
		Clock.DrawCentered(base.spriteBatch, timeBoxTextPos, DM.Player.PlayTime, Color.Black);
		clock.Draw(base.spriteBatch, clockPos);
		stretchyBox.Draw(base.spriteBatch, killBoxRect);
		Text.DrawCentered(base.spriteBatch, killBoxTextPos, "Kill Count", Color.Black);
		Text.DrawCentered(base.spriteBatch, killBoxTextPos + new Vector2(0f, 40f), DM.Player.KillCount, killColor);
		bloodKillCount.Draw(base.spriteBatch, bloodPos);
		base.spriteBatch.End();
	}
}
