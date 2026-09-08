using System;
using Eyehook.Framework;
using Loot.Awardments;
using Loot.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class AwardmentScreen : Screen
{
	private const int awardDx = 104;

	private const int awardDy = 96;

	private int curPage;

	private int curRow;

	private int curCol;

	private GamePadRepeatUtil repeat;

	private static Texture2D pixel;

	private static Texture2D bg;

	private static StillSprite lockIcon;

	private static StillSprite highlight;

	private static Awardment[,,] awards = new Awardment[2, 3, 8]
	{
		{
			{
				Awardment.IntoTheUnknown,
				Awardment.SkillUp,
				Awardment.PoisonMaster,
				Awardment.FrenzyMaster,
				Awardment.FreezeMaster,
				Awardment.OrbMaster,
				Awardment.RegenMaster,
				Awardment.JackOfAllTrades
			},
			{
				Awardment.EpicGet,
				Awardment.EpicSet,
				Awardment.BloodBath,
				Awardment.TripleThreat,
				Awardment.Lucky,
				Awardment.ImRich,
				Awardment.SuperChain,
				Awardment.LeapOfFaith
			},
			{
				Awardment.CloseCall,
				Awardment.Spooky,
				Awardment.FearTheReaper,
				Awardment.Halfway,
				Awardment.Fast,
				Awardment.Escaped,
				Awardment.HardAndFast,
				Awardment.Victory
			}
		},
		{
			{
				Awardment.Cursed,
				Awardment.CurseBreaker,
				Awardment.SwordAndSworcery,
				Awardment.Jackpot,
				Awardment.Slayer,
				Awardment.Destroyer,
				Awardment.Invincible,
				Awardment.WhyChange
			},
			{
				Awardment.Encounter,
				Awardment.Toeless,
				Awardment.BlessedByElves,
				Awardment.OldGods,
				Awardment.Streaker,
				Awardment.UndeadRising,
				Awardment.Anvil,
				Awardment.Drinkaholic
			},
			{
				Awardment.Heartbreaker,
				Awardment.EscapeBerserker,
				Awardment.EscapeShaman,
				Awardment.EscapeTinkerer,
				Awardment.EscapeGambler,
				Awardment.EscapeGoblin,
				Awardment.EscapePeasant,
				Awardment.EscapeAllClasses
			}
		}
	};

	private Vector2 awardPos = new Vector2(276f, 360f);

	private Vector2 titlePos = new Vector2(440f, 104f);

	private static Color titleTextColor = new Color(151, 100, 0);

	private static Color titleTextShadowColor = new Color(255, 255, 0);

	private static Color pageColor = new Color(255, 203, 0);

	private Color textColor = Color.Black;

	private Vector2 textPos = new Vector2(250f, 166f);

	private Vector2 textLineOffset = new Vector2(0f, 40f);

	public AwardmentScreen()
		: base(modal: true)
	{
		repeat = new GamePadRepeatUtil(MC.GamePadManager, TimeSpan.FromMilliseconds(300.0), TimeSpan.FromMilliseconds(150.0));
	}

	public static void Load(ContentManager content)
	{
		pixel = content.Load<Texture2D>("Sprites\\Pixel");
		bg = content.Load<Texture2D>("Sprites\\Awardment\\Screen");
		lockIcon = new StillSprite(content.Load<Texture2D>("Sprites\\Awardment\\Lock"));
		highlight = new StillSprite(content.Load<Texture2D>("Sprites\\Awardment\\Highlight"));
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.B) || MC.GamePadManager.isButtonDown(Buttons.Back) || MC.GamePadManager.isButtonDown(Buttons.Start))
		{
			PlaySound.MenuClick();
			MC.ScreenManager.removeScreen(this);
			return;
		}
		repeat.Update(gameTime);
		if (repeat.Right())
		{
			PlaySound.MenuMove();
			curCol++;
			if (curCol == awards.GetLength(2))
			{
				curCol = 0;
			}
		}
		else if (repeat.Left())
		{
			PlaySound.MenuMove();
			curCol--;
			if (curCol < 0)
			{
				curCol = awards.GetLength(2) - 1;
			}
		}
		else if (repeat.Up())
		{
			PlaySound.MenuMove();
			curRow--;
			if (curRow < 0)
			{
				curRow = awards.GetLength(1) - 1;
			}
		}
		else if (repeat.Down())
		{
			PlaySound.MenuMove();
			curRow++;
			if (curRow == awards.GetLength(1))
			{
				curRow = 0;
			}
		}
		else if (MC.GamePadManager.isNewButtonDown(Buttons.LeftShoulder))
		{
			curCol = 0;
			curRow = 0;
			curPage--;
			if (curPage < 0)
			{
				curPage = awards.GetLength(0) - 1;
			}
		}
		else if (MC.GamePadManager.isNewButtonDown(Buttons.RightShoulder))
		{
			curCol = 0;
			curRow = 0;
			curPage++;
			if (curPage == awards.GetLength(0))
			{
				curPage = 0;
			}
		}
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		base.spriteBatch.Draw(pixel, base.viewportRect, MC.ScreenManager.BackgroundColor);
		base.spriteBatch.Draw(bg, base.viewportRect, Color.White);
		Text.DrawShadowString(base.spriteBatch, titlePos, "Awardments", titleTextColor, titleTextShadowColor, 1f);
		Vector2 v = new Vector2(840f, 104f);
		int num = Text.Width(Profile.Awardments.Points) + Text.Width(Profile.Awardments.MaxPoints);
		v.X -= num;
		Text.DrawShadowString(base.spriteBatch, ref v, Profile.Awardments.Points, titleTextColor, titleTextShadowColor, 1f);
		Text.DrawShadowString(base.spriteBatch, ref v, "/", titleTextColor, titleTextShadowColor, 1f);
		Text.DrawShadowString(base.spriteBatch, ref v, Profile.Awardments.MaxPoints, titleTextColor, titleTextShadowColor, 1f);
		Vector2 v2 = new Vector2(616f, 630f);
		Text.Draw(base.spriteBatch, ref v2, curPage + 1, pageColor);
		Text.Draw(base.spriteBatch, ref v2, '/', pageColor);
		Text.Draw(base.spriteBatch, ref v2, awards.GetLength(0), pageColor);
		for (int i = 0; i < awards.GetLength(1); i++)
		{
			for (int j = 0; j < awards.GetLength(2); j++)
			{
				Awardment awardment = awards[curPage, i, j];
				Vector2 vector = new Vector2(j * 104, i * 96);
				if (Profile.Awardments.IsUnlocked(awardment))
				{
					awardment.Icon.Draw(base.spriteBatch, awardPos + vector);
				}
				else
				{
					lockIcon.Draw(base.spriteBatch, awardPos + vector);
				}
			}
		}
		highlight.Draw(base.spriteBatch, awardPos + new Vector2(curCol * 104, curRow * 96));
		Awardment awardment2 = awards[curPage, curRow, curCol];
		Vector2 v3 = textPos;
		Text.Draw(base.spriteBatch, ref v3, awardment2.Name, textColor);
		Text.Draw(base.spriteBatch, ref v3, ' ', textColor);
		Text.Draw(base.spriteBatch, ref v3, '(', textColor);
		Text.Draw(base.spriteBatch, ref v3, awardment2.Points, textColor);
		Text.Draw(base.spriteBatch, ref v3, ')', textColor);
		if (Profile.Awardments.IsUnlocked(awardment2))
		{
			Text.Draw(base.spriteBatch, textPos + textLineOffset, awardment2.UnlockedText, textColor);
			Vector2 v4 = textPos + textLineOffset * 2f;
			DateTime dateTime = Profile.Awardments.UnlockDate(awardment2);
			Text.Draw(base.spriteBatch, ref v4, "Unlocked: ", textColor);
			Text.Draw(base.spriteBatch, ref v4, Month(dateTime.Month), textColor);
			Text.Draw(base.spriteBatch, ref v4, ' ', textColor);
			Text.Draw(base.spriteBatch, ref v4, dateTime.Day, textColor);
			Text.Draw(base.spriteBatch, ref v4, ',', textColor);
			Text.Draw(base.spriteBatch, ref v4, ' ', textColor);
			Text.Draw(base.spriteBatch, ref v4, dateTime.Year, textColor);
		}
		else
		{
			Text.Draw(base.spriteBatch, textPos + textLineOffset, awardment2.LockedText, textColor);
			Text.Draw(base.spriteBatch, textPos + textLineOffset * 2f, "Locked", textColor);
		}
		base.spriteBatch.End();
		base.draw(gameTime);
	}

	private string Month(int i)
	{
		return i switch
		{
			1 => "Jan", 
			2 => "Feb", 
			3 => "Mar", 
			4 => "Apr", 
			5 => "May", 
			6 => "Jun", 
			7 => "Jul", 
			8 => "Aug", 
			9 => "Sep", 
			10 => "Oct", 
			11 => "Nov", 
			_ => "Dec", 
		};
	}
}
