using System;
using Eyehook.Framework;
using Loot.Awardments;
using Loot.Core;
using Loot.Dungeon;
using Loot.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Slots;

public class SlotScreen : Screen
{
	private enum GameMode
	{
		NewGame,
		Spin1,
		Hold,
		Spin2
	}

	private int Winnings;

	private int HoldState;

	private SlotReel[] Reels = new SlotReel[5];

	private bool[] Hold = new bool[5];

	private int[] SuitCount = new int[6];

	private Color textColor = new Color(152, 101, 0);

	private GameMode Mode;

	private Texture2D Cabinet;

	private StillSprite Hand;

	private StillSprite HoldOn;

	private StillSprite ReelSprite;

	private StillSprite Spin2;

	private float handTimer;

	private Vector2 HandOffset = Vector2.Zero;

	private Vector2 reelPos = new Vector2(466f, 326f);

	private Rectangle cabinetRect = new Rectangle(0, 0, 1280, 720);

	private Vector2 spin2Pos = new Vector2(740f, 440f);

	private Vector2 textPos = new Vector2(994f, 412f);

	private Vector2 holdPos = new Vector2(314f, 488f);

	private Vector2 holdOffset = new Vector2(76f, 0f);

	private Vector2 handHoldPos = new Vector2(314f, 534f);

	private Vector2 handSpinPos = new Vector2(740f, 534f);

	public SlotScreen()
		: base(modal: true)
	{
		for (int i = 0; i < 5; i++)
		{
			Reels[i] = new SlotReel();
		}
	}

	public override void transitionOn()
	{
		BgMusic.SlotsOpen();
	}

	public override void transitionOff()
	{
		BgMusic.SlotsClose();
	}

	public override void loadContent(ContentManager content)
	{
		Cabinet = content.Load<Texture2D>("Sprites\\Slots\\Cabinet");
		Hand = new StillSprite(content.Load<Texture2D>("Sprites\\Slots\\Hand"));
		HoldOn = new StillSprite(content.Load<Texture2D>("Sprites\\Slots\\HoldOn"));
		ReelSprite = new StillSprite(content.Load<Texture2D>("Sprites\\Slots\\Reels"));
		Spin2 = new StillSprite(content.Load<Texture2D>("Sprites\\Slots\\Spin2"));
	}

	private void noop()
	{
	}

	private void exitScreen()
	{
		MC.ScreenManager.removeAllScreens();
		MC.ScreenManager.addScreen(new DungeonView());
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.Start))
		{
			MC.ScreenManager.addScreen(new PauseScreen());
			return;
		}
		if (MC.GamePadManager.isNewButtonDown(Buttons.B))
		{
			if (Mode == GameMode.NewGame)
			{
				exitScreen();
			}
			else
			{
				MessageBox.Display("Exit Slots", "You still have one more spin.\nAre you sure you want to exit?", exitScreen, noop);
			}
			return;
		}
		UpdateHand(gameTime);
		bool flag = false;
		for (int i = 0; i < 5; i++)
		{
			Reels[i].Update(gameTime);
			if (Reels[i].IsSpinning)
			{
				flag = true;
			}
		}
		if (flag)
		{
			return;
		}
		if (Mode == GameMode.NewGame && MC.GamePadManager.isNewButtonDown(Buttons.A))
		{
			for (int j = 0; j < 5; j++)
			{
				Reels[j].Spin(j);
			}
			DM.Player.Gold -= 25;
			Mode = GameMode.Spin1;
			HoldState = 5;
			SlotSounds.Spin();
			return;
		}
		if (Mode == GameMode.Spin1)
		{
			Mode = GameMode.Hold;
			UpdateSuitCount();
			for (int k = 0; k < 5; k++)
			{
				SlotSuit value = Reels[k].Value;
				Hold[k] = value != SlotSuit.Skull && SuitCount[(int)value] > 1;
			}
		}
		if (Mode == GameMode.Hold)
		{
			if (MC.GamePadManager.isNewDirRight())
			{
				HoldState++;
				if (HoldState == 6)
				{
					HoldState = 0;
				}
			}
			else if (MC.GamePadManager.isNewDirLeft())
			{
				HoldState--;
				if (HoldState < 0)
				{
					HoldState = 5;
				}
			}
			else
			{
				if (!MC.GamePadManager.isNewButtonDown(Buttons.A))
				{
					return;
				}
				if (HoldState < 5)
				{
					SlotSounds.ReelHold();
					Hold[HoldState] = !Hold[HoldState];
					return;
				}
				bool flag2 = true;
				for (int l = 0; l < 5; l++)
				{
					if (!Hold[l])
					{
						Reels[l].Spin(l);
						flag2 = false;
					}
				}
				Mode = GameMode.Spin2;
				if (!flag2)
				{
					SlotSounds.Spin();
				}
			}
		}
		else if (Mode == GameMode.Spin2)
		{
			Mode = GameMode.NewGame;
			UpdateSuitCount();
			if (CalcWinnings())
			{
				SlotSounds.Win();
			}
			else
			{
				SlotSounds.Lose();
			}
		}
	}

	private bool CalcWinnings()
	{
		for (int i = 1; i < 6; i++)
		{
			if (SuitCount[i] == 5)
			{
				Winnings = 150;
				DM.Player.Gold += 150;
				Profile.Awardments.Unlock(Awardment.Jackpot);
				return true;
			}
			if (SuitCount[i] == 4)
			{
				Winnings = 75;
				DM.Player.Gold += 75;
				return true;
			}
			if (SuitCount[i] == 3)
			{
				for (int j = 1; j < 6; j++)
				{
					if (SuitCount[j] == 2)
					{
						Winnings = 50;
						DM.Player.Gold += 50;
						return true;
					}
				}
				Winnings = 25;
				DM.Player.Gold += 25;
				return true;
			}
			if (SuitCount[i] != 2)
			{
				continue;
			}
			for (int k = i + 1; k < 6; k++)
			{
				if (SuitCount[k] == 2)
				{
					Winnings = 25;
					DM.Player.Gold += 25;
					return true;
				}
			}
		}
		Winnings = -25;
		return false;
	}

	private void UpdateSuitCount()
	{
		for (int i = 0; i < 6; i++)
		{
			SuitCount[i] = 0;
		}
		for (int j = 0; j < 5; j++)
		{
			SuitCount[(int)Reels[j].Value]++;
		}
	}

	private void UpdateHand(GameTime gameTime)
	{
		handTimer += (float)(4.0 * gameTime.ElapsedGameTime.TotalSeconds);
		if ((double)handTimer > 6.2831854820251465)
		{
			handTimer -= (float)Math.PI * 2f;
		}
		HandOffset.Y = 4f * (float)Math.Sin(handTimer);
	}

	public override void draw(GameTime gameTime)
	{
		base.spriteBatch.Begin();
		ReelSprite.Draw(base.spriteBatch, reelPos);
		for (int i = 0; i < 5; i++)
		{
			Reels[i].Draw(base.spriteBatch, new Vector2(314 + i * 76, 154f));
		}
		base.spriteBatch.Draw(Cabinet, cabinetRect, Color.White);
		if (Mode == GameMode.Hold || Mode == GameMode.Spin2)
		{
			Spin2.Draw(base.spriteBatch, spin2Pos);
		}
		drawGold(base.spriteBatch);
		drawHold(base.spriteBatch);
		drawHand(base.spriteBatch);
		drawText(base.spriteBatch);
		base.spriteBatch.End();
	}

	private void drawText(SpriteBatch spriteBatch)
	{
		if (Mode == GameMode.NewGame)
		{
			if (Winnings == 0)
			{
				Text.DrawCentered(spriteBatch, textPos, "SPIN", textColor);
				return;
			}
			if (Winnings < 0)
			{
				Text.DrawCentered(spriteBatch, textPos, "YOU LOSE", textColor);
				return;
			}
			int num = Text.Width(Winnings) + 72;
			Vector2 v = textPos;
			v.X -= num / 2;
			Text.Draw(spriteBatch, ref v, '+', textColor);
			Text.Draw(spriteBatch, ref v, Winnings, textColor);
			Text.Draw(spriteBatch, ref v, " GP", textColor);
		}
		else if (Mode == GameMode.Hold)
		{
			Text.DrawCentered(spriteBatch, textPos, "HOLD/SPIN", textColor);
		}
		else
		{
			Text.DrawCentered(spriteBatch, textPos, "...", textColor);
		}
	}

	private void drawGold(SpriteBatch spriteBatch)
	{
		Vector2 v = new Vector2(660 - Text.Width(DM.Player.Gold), 596f);
		Text.Draw(spriteBatch, ref v, DM.Player.Gold, textColor);
	}

	private void drawHold(SpriteBatch spriteBatch)
	{
		if (Mode != GameMode.Hold && Mode != GameMode.Spin2)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			if (Hold[i])
			{
				HoldOn.Draw(spriteBatch, holdPos + i * holdOffset);
			}
		}
	}

	private void drawHand(SpriteBatch spriteBatch)
	{
		if (Mode == GameMode.NewGame)
		{
			Hand.Draw(spriteBatch, handSpinPos + HandOffset);
		}
		else if (Mode == GameMode.Hold)
		{
			if (HoldState == 5)
			{
				Hand.Draw(spriteBatch, handSpinPos + HandOffset);
				return;
			}
			Vector2 position = handHoldPos + HandOffset;
			position.X += HoldState * 76;
			Hand.Draw(spriteBatch, position);
		}
	}
}
