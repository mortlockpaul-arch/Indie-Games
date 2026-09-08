using System;
using Eyehook.Framework;
using Loot.Core;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class StatScreen : SlidingScreen
{
	private int curStat;

	public bool Disabled;

	private static Sprite box;

	private static Sprite statHighlight;

	private static Sprite upA;

	private float twoPiTimer;

	private Color shadowColor = new Color(0, 0, 0) * 0.5f;

	private static Color titleTextColor = new Color(151, 100, 0);

	private static Color titleTextShadowColor = new Color(255, 255, 0);

	private static Vector2 titleShadowOffset = new Vector2(2f, 2f);

	private Vector2 longNameOffset = new Vector2(0f, -244f);

	private Vector2 statNameOffset = new Vector2(-64f, -138f);

	private Vector2 statValueOffset = new Vector2(54f, -138f);

	private Vector2 statHiOffset = new Vector2(4f, -138f);

	private Vector2 lineHeight = new Vector2(0f, 88f);

	private Vector2 arrowOffset = new Vector2(152f, -140f);

	private string pointText = "Points: ";

	private Vector2 pointOffset = new Vector2(0f, 236f);

	public StatScreen()
		: base(new Vector2(-232f, 362f), new Vector2(348f, 362f), TimeSpan.FromMilliseconds(500.0))
	{
	}

	public static void Load(ContentManager content)
	{
		upA = new StillSprite(content.Load<Texture2D>("Sprites\\StatSkill\\UpA"));
		box = new StillSprite(content.Load<Texture2D>("Sprites\\StatSkill\\StatBox"));
		statHighlight = new StillSprite(content.Load<Texture2D>("Sprites\\StatSkill\\StatHighlight"));
	}

	public override void transitionOn()
	{
		curStat = 0;
	}

	public override void update(GameTime gameTime)
	{
		base.update(gameTime);
		twoPiTimer += (float)(gameTime.ElapsedGameTime.TotalSeconds * 6.2831854820251465);
		if ((double)twoPiTimer > 6.2831854820251465)
		{
			twoPiTimer -= (float)Math.PI * 2f;
		}
		if (MC.GamePadManager.isNewButtonDown(Buttons.LeftShoulder))
		{
			if (base.IsOff || base.IsSlideOff)
			{
				SetMode(Mode.SlideOn);
			}
			else if (base.IsOn || base.IsSlideOn)
			{
				SetMode(Mode.SlideOff);
			}
		}
		else
		{
			if (!base.IsOn || Disabled)
			{
				return;
			}
			if (MC.GamePadManager.isNewDirDown())
			{
				PlaySound.MenuMove();
				curStat++;
				if (curStat > 3)
				{
					curStat = 0;
				}
			}
			else if (MC.GamePadManager.isNewDirUp())
			{
				PlaySound.MenuMove();
				curStat--;
				if (curStat < 0)
				{
					curStat = 3;
				}
			}
			else if (DM.Player.StatPoints > 0 && MC.GamePadManager.isNewButtonDown(Buttons.A))
			{
				PlaySound.MenuClick();
				DM.Player.StatPoints--;
				DM.Player.Base.Modify(curStat, 1);
				DM.Player.CalcStats();
			}
		}
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		box.Draw(base.spriteBatch, Offset);
		drawName();
		drawStats();
		drawPoints();
		if (Disabled)
		{
			box.Draw(base.spriteBatch, Offset, shadowColor, 1f);
		}
		base.spriteBatch.End();
	}

	private void drawName()
	{
		Text.DrawCentered(base.spriteBatch, Offset + longNameOffset + titleShadowOffset, Stats.LongName[curStat], titleTextShadowColor);
		Text.DrawCentered(base.spriteBatch, Offset + longNameOffset, Stats.LongName[curStat], titleTextColor);
	}

	private void drawStats()
	{
		for (int i = 0; i < 4; i++)
		{
			if (curStat == i)
			{
				statHighlight.Draw(base.spriteBatch, Offset + statHiOffset + lineHeight * i);
				if (!Disabled && DM.Player.StatPoints > 0)
				{
					upA.Draw(base.spriteBatch, Offset + arrowOffset + lineHeight * i + new Vector2(0f, 4f) * (float)Math.Sin(twoPiTimer));
				}
			}
			Text.DrawCentered(base.spriteBatch, Offset + statNameOffset + lineHeight * i, Stats.Name[i], Color.Black);
			Text.DrawCentered(base.spriteBatch, Offset + statValueOffset + lineHeight * i, DM.Player.Stats.ToString(i), Color.Black);
		}
	}

	private void drawPoints()
	{
		Vector2 v = Offset + pointOffset + new Vector2(-((Text.Width(pointText) + Text.Width(DM.Player.StatPoints) - 24) / 2), 0f);
		Text.Draw(base.spriteBatch, ref v, pointText, Color.Black);
		Text.Draw(base.spriteBatch, ref v, DM.Player.StatPoints, Color.Black);
	}
}
