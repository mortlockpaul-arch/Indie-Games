using System;
using Eyehook.Framework;
using Loot.Core;
using Loot.Dungeon;
using Loot.PC;
using Loot.Skills;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class ClassScreen : Screen
{
	private enum Mode
	{
		Ready,
		SlideRight,
		SlideLeft
	}

	private const int descWidth = 298;

	private Texture2D pixel;

	private Sprite bg;

	private Sprite star;

	private Sprite arrow;

	private string skillName;

	private FormattedText skillDesc;

	private Mode mode;

	private PlayerClassType curClass;

	private GameOptions gameOptions;

	private TimeSpan slideTime = TimeSpan.Zero;

	private TimeSpan slideDuration = TimeSpan.FromMilliseconds(500.0);

	private Vector2 bgPos = new Vector2(640f, 360f);

	private string title = "Select a Class";

	private Vector2 titlePos = new Vector2(351f, 150f);

	private Vector2 classPos = new Vector2(351f, 238f);

	private string primarySkill = "Primary Skill";

	private Vector2 primarySkillPos = new Vector2(351f, 298f);

	private Color primarySkillColor = new Color(128, 123, 102);

	private Vector2 starPos = new Vector2(188f, 336f);

	private Vector2 skillPos = new Vector2(226f, 374f);

	private Vector2 skillNamePos = new Vector2(290f, 374f);

	private Vector2 skillDescPos = new Vector2(222f, 440f);

	private Vector2 leftArrowPos = new Vector2(170f, 238f);

	private Vector2 rightArrowPos = new Vector2(534f, 238f);

	private string buttonText = "\u0080\u0081Accept \u0082\u0083Back";

	private Vector2 buttonPos = new Vector2(351f, 570f);

	private Vector2 portraitPos = new Vector2(864f, 360f);

	private Rectangle portraitMask = new Rectangle(608, 104, 512, 512);

	public ClassScreen(GameOptions gameOptions)
		: base(modal: true)
	{
		this.gameOptions = gameOptions;
		mode = Mode.Ready;
		curClass = PlayerClassType.Berserker;
		setSkillInfo();
	}

	private string className(PlayerClassType pct)
	{
		return pct switch
		{
			PlayerClassType.Berserker => "Berserker", 
			PlayerClassType.Shaman => "Shaman", 
			PlayerClassType.Tinkerer => "Tinkerer", 
			PlayerClassType.Gambler => "Gambler", 
			PlayerClassType.Goblin => "Goblin", 
			PlayerClassType.Peasant => "Peasant", 
			_ => throw new Exception("Unknown class type: " + pct), 
		};
	}

	private void setSkillInfo()
	{
		switch (curClass)
		{
		case PlayerClassType.Berserker:
			skillName = "Frenzy";
			skillDesc = new FormattedText("A powerful spin attack.", 298);
			break;
		case PlayerClassType.Shaman:
			skillName = "Freeze";
			skillDesc = new FormattedText("Stops nearby enemies.", 298);
			break;
		case PlayerClassType.Tinkerer:
			skillName = "Orb";
			skillDesc = new FormattedText("A mechanical ally.", 298);
			break;
		case PlayerClassType.Gambler:
			skillName = "Poison";
			skillDesc = new FormattedText("Deals damage over time.", 298);
			break;
		case PlayerClassType.Goblin:
			skillName = "Regen";
			skillDesc = new FormattedText("Heal wounds over time.", 298);
			break;
		case PlayerClassType.Peasant:
			skillName = "None";
			skillDesc = new FormattedText("But, he sure knows sheep.", 298);
			break;
		}
	}

	private Sprite getPortrait(PlayerClassType pct)
	{
		return pct switch
		{
			PlayerClassType.Berserker => Picture.Berserker, 
			PlayerClassType.Shaman => Picture.Shaman, 
			PlayerClassType.Tinkerer => Picture.Tinkerer, 
			PlayerClassType.Gambler => Picture.Gambler, 
			PlayerClassType.Goblin => Picture.Goblin, 
			PlayerClassType.Peasant => Picture.Peasant, 
			_ => throw new Exception("Unknown class type: " + pct), 
		};
	}

	private Sprite getSkillSprite(PlayerClassType pct)
	{
		return pct switch
		{
			PlayerClassType.Berserker => SkillSprite.Frenzy, 
			PlayerClassType.Shaman => SkillSprite.Freeze, 
			PlayerClassType.Tinkerer => SkillSprite.Orb, 
			PlayerClassType.Gambler => SkillSprite.Poison, 
			PlayerClassType.Goblin => SkillSprite.Regen, 
			PlayerClassType.Peasant => SkillSprite.None, 
			_ => throw new Exception("Unknown class type: " + pct), 
		};
	}

	private PlayerClassType getClassRight()
	{
		int num = (int)(curClass + 1);
		if (num > 5)
		{
			num = 0;
		}
		return (PlayerClassType)num;
	}

	private PlayerClassType getClassLeft()
	{
		int num = (int)(curClass - 1);
		if (num < 0)
		{
			num = 5;
		}
		return (PlayerClassType)num;
	}

	public override void update(GameTime gameTime)
	{
		arrow.Update(gameTime);
		if (slideTime > TimeSpan.Zero)
		{
			slideTime -= gameTime.ElapsedGameTime;
			if (slideTime <= TimeSpan.Zero)
			{
				curClass = ((mode != Mode.SlideRight) ? getClassLeft() : getClassRight());
				setSkillInfo();
				mode = Mode.Ready;
			}
		}
		else if (MC.GamePadManager.isNewButtonDown(Buttons.A))
		{
			BgMusic.Next();
			PlaySound.Encounter();
			MC.ScreenManager.addScreen(new RippleScreen(newGame));
		}
		else if (MC.GamePadManager.isNewButtonDown(Buttons.B))
		{
			PlaySound.MenuClick();
			MC.ScreenManager.removeAllScreens();
			MC.ScreenManager.addScreen(new MainMenu());
		}
		else if (MC.GamePadManager.isNewDirRight())
		{
			PlaySound.MenuMove();
			slideTime = slideDuration;
			mode = Mode.SlideRight;
		}
		else if (MC.GamePadManager.isNewDirLeft())
		{
			PlaySound.MenuMove();
			slideTime = slideDuration;
			mode = Mode.SlideLeft;
		}
	}

	private void newGame()
	{
		MC.ScreenManager.removeAllScreens();
		DM.NewGame(curClass, gameOptions);
	}

	public override void loadContent(ContentManager content)
	{
		pixel = content.Load<Texture2D>("Sprites\\Pixel");
		bg = new StillSprite(content.Load<Texture2D>("Sprites\\UI\\ClassScreen"));
		star = new StillSprite(content.Load<Texture2D>("Sprites\\StatSkill\\Star"));
		arrow = new AnimatedSprite(content.Load<Texture2D>("Sprites\\UI\\ClassArrow"), new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 8, TimeSpan.FromMilliseconds(100.0));
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin(SpriteSortMode.Immediate, null, GraphicUtil.PointFilter, null, null);
		bg.Draw(base.spriteBatch, bgPos);
		arrow.SpriteEffects = SpriteEffects.FlipHorizontally;
		arrow.Draw(base.spriteBatch, leftArrowPos);
		arrow.SpriteEffects = SpriteEffects.None;
		arrow.Draw(base.spriteBatch, rightArrowPos);
		Text.DrawCentered(base.spriteBatch, titlePos, title, Color.Black);
		Text.DrawCentered(base.spriteBatch, classPos, className(curClass), Color.Black);
		Text.DrawCentered(base.spriteBatch, primarySkillPos, primarySkill, primarySkillColor);
		getSkillSprite(curClass).Draw(base.spriteBatch, skillPos);
		star.Draw(base.spriteBatch, starPos);
		Text.Draw(base.spriteBatch, skillNamePos, skillName, Color.Black);
		Text.Draw(base.spriteBatch, skillDescPos, skillDesc, Color.Black);
		Text.DrawCentered(base.spriteBatch, buttonPos, buttonText, Color.Black);
		Sprite portrait = getPortrait(curClass);
		if (mode == Mode.Ready)
		{
			portrait.Draw(base.spriteBatch, portraitPos, Color.White, 4f);
			base.spriteBatch.End();
			return;
		}
		base.spriteBatch.End();
		base.spriteBatch.GraphicsDevice.Clear(ClearOptions.Stencil, Color.Black, 0f, 0);
		base.spriteBatch.Begin(SpriteSortMode.Immediate, GraphicUtil.NoColorWrite, GraphicUtil.PointFilter, GraphicUtil.Stencil1Always, null, GraphicUtil.AlphaGreaterThan0);
		base.spriteBatch.Draw(pixel, portraitMask, Color.White);
		base.spriteBatch.End();
		base.spriteBatch.Begin(SpriteSortMode.Immediate, null, GraphicUtil.PointFilter, GraphicUtil.Stencil1Match, null);
		if (mode == Mode.SlideRight)
		{
			int num = (int)(512.0 * (1.0 - slideTime.TotalSeconds / slideDuration.TotalSeconds));
			portrait.Draw(base.spriteBatch, portraitPos + new Vector2(num, 0f), Color.White, 4f);
			getPortrait(getClassRight()).Draw(base.spriteBatch, portraitPos + new Vector2(num - 512, 0f), Color.White, 4f);
		}
		else if (mode == Mode.SlideLeft)
		{
			int num2 = -(int)(512.0 * (1.0 - slideTime.TotalSeconds / slideDuration.TotalSeconds));
			portrait.Draw(base.spriteBatch, portraitPos + new Vector2(num2, 0f), Color.White, 4f);
			getPortrait(getClassLeft()).Draw(base.spriteBatch, portraitPos + new Vector2(num2 + 512, 0f), Color.White, 4f);
		}
		base.spriteBatch.End();
	}
}
