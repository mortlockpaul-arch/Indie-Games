using System;
using System.Collections.Generic;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

internal abstract class MenuScreen : GameScreen
{
	private List<MenuEntry> menuEntries = new List<MenuEntry>();

	private int selectedEntry;

	private string menuTitle;

	protected bool bLockMenu;

	protected int StartX = 100;

	protected int StartY = 150;

	private float InputTimer;

	protected IList<MenuEntry> MenuEntries => menuEntries;

	public MenuScreen(string menuTitle)
	{
		this.menuTitle = menuTitle;
		base.TransitionOnTime = TimeSpan.FromSeconds(0.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
	}

	public override void HandleInput()
	{
		if (!(InputTimer > InputManager.INPUT_LATENCY) || bLockMenu)
		{
			return;
		}
		if (!base.ControllingPlayer.HasValue)
		{
			for (int i = 0; i < 4; i++)
			{
				if (GameContext.Pinfo[i].Controller != PlayerController.PLAYER)
				{
					continue;
				}
				int pIndex = (int)GameContext.Pinfo[i].pIndex;
				if (InputManager.GamerIndex[pIndex] == -1)
				{
					continue;
				}
				if (InputManager.GetKeyState(GameContext.Pinfo[i].pIndex, 0) == ButtonState.Pressed)
				{
					selectedEntry--;
					if (selectedEntry < 0)
					{
						selectedEntry = menuEntries.Count - 1;
					}
					InputTimer = 0f;
				}
				if (InputManager.GetKeyState(GameContext.Pinfo[i].pIndex, 2) == ButtonState.Pressed)
				{
					selectedEntry++;
					if (selectedEntry >= menuEntries.Count)
					{
						selectedEntry = 0;
					}
					InputTimer = 0f;
				}
				if (InputManager.GetKeyState(GameContext.Pinfo[i].pIndex, 4) == ButtonState.Pressed)
				{
					OnSelectEntry(selectedEntry, GameContext.Pinfo[i].pIndex);
				}
			}
			return;
		}
		if (InputManager.GetKeyState(base.ControllingPlayer.Value, 0) == ButtonState.Pressed)
		{
			selectedEntry--;
			if (selectedEntry < 0)
			{
				selectedEntry = menuEntries.Count - 1;
			}
			InputTimer = 0f;
		}
		if (InputManager.GetKeyState(base.ControllingPlayer.Value, 2) == ButtonState.Pressed)
		{
			selectedEntry++;
			if (selectedEntry >= menuEntries.Count)
			{
				selectedEntry = 0;
			}
			InputTimer = 0f;
		}
		if (InputManager.GetKeyState(base.ControllingPlayer.Value, 4) == ButtonState.Pressed)
		{
			OnSelectEntry(selectedEntry, base.ControllingPlayer.Value);
		}
	}

	protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
	{
		menuEntries[selectedEntry].OnSelectEntry(playerIndex);
	}

	protected virtual void OnCancel(PlayerIndex playerIndex)
	{
		ExitScreen();
	}

	protected void OnCancel(object sender, PlayerIndexEventArgs e)
	{
		OnCancel(e.PlayerIndex);
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		for (int i = 0; i < menuEntries.Count; i++)
		{
			bool isSelected = base.IsActive && i == selectedEntry;
			menuEntries[i].Update(this, isSelected, gameTime);
		}
		InputTimer += gameTime.ElapsedGameTime.Milliseconds;
	}

	public override void Draw(GameTime gameTime)
	{
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		SpriteFont font = base.ScreenManager.Font;
		Vector2 position = new Vector2(StartX, StartY);
		float num = (float)Math.Pow(base.TransitionPosition, 2.0);
		if (base.ScreenState == ScreenState.TransitionOn)
		{
			position.X -= num * 256f;
		}
		else
		{
			position.X += num * 512f;
		}
		spriteBatch.Begin();
		for (int i = 0; i < menuEntries.Count; i++)
		{
			MenuEntry menuEntry = menuEntries[i];
			bool isSelected = base.IsActive && i == selectedEntry;
			if (!bLockMenu)
			{
				menuEntry.Draw(this, position, isSelected, gameTime);
			}
			position.Y += menuEntry.GetHeight(this) + 15;
		}
		Vector2 position2 = new Vector2(426f, 80f);
		Vector2 origin = font.MeasureString(menuTitle) / 2f;
		Color color = new Color(192, 192, 192, base.TransitionAlpha);
		float scale = 1.25f;
		position2.Y -= num * 100f;
		spriteBatch.DrawString(font, menuTitle, position2, color, 0f, origin, scale, SpriteEffects.None, 0f);
		spriteBatch.End();
	}
}
