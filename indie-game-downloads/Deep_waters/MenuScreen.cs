using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

internal abstract class MenuScreen : GameScreen
{
	private List<MenuEntry> menuEntries = new List<MenuEntry>();

	private int selectedEntry;

	private string menuTitle;

	public int value;

	private Vector2 menupos;

	public SpriteFont font;

	public bool isscreenexitable = true;

	protected IList<MenuEntry> MenuEntries => menuEntries;

	public MenuScreen(string menuTitle, Vector2 position)
	{
		menupos = position;
		this.menuTitle = menuTitle;
		base.TransitionOnTime = TimeSpan.FromSeconds(0.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
		currentsettings = new GameSettings();
	}

	public override void HandleInput(InputState input)
	{
		if (input.IsMenuUp(base.ControllingPlayer))
		{
			selectedEntry--;
			base.ScreenManager.audioManager.playsound("select");
			if (selectedEntry < 0)
			{
				selectedEntry = menuEntries.Count - 1;
			}
		}
		if (input.IsMenuDown(base.ControllingPlayer))
		{
			selectedEntry++;
			base.ScreenManager.audioManager.playsound("select");
			if (selectedEntry >= menuEntries.Count)
			{
				selectedEntry = 0;
			}
		}
		if (input.IsMenuSelect(base.ControllingPlayer, out var playerIndex))
		{
			OnSelectEntry(selectedEntry, playerIndex);
		}
		else if (input.IsMenuCancel(base.ControllingPlayer, out playerIndex))
		{
			OnCancel(playerIndex);
		}
	}

	protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
	{
		menuEntries[entryIndex].OnSelectEntry(playerIndex);
		base.ScreenManager.audioManager.playsound("Ok");
	}

	protected virtual void OnCancel(PlayerIndex playerIndex)
	{
		base.ScreenManager.audioManager.playsound("Back");
		if (isscreenexitable)
		{
			ExitScreen();
		}
	}

	protected virtual void OnCancelOptions(PlayerIndex playerIndex)
	{
		if (Gamer.SignedInGamers[playerIndex] != null)
		{
			if (base.ScreenManager.storageManager.device != null)
			{
				base.ScreenManager.storageManager.requestsavesettings(base.ScreenManager.settings, playerIndex);
				base.ScreenManager.settings = base.ScreenManager.storageManager.settings;
			}
			if (isscreenexitable)
			{
				ExitScreen();
			}
		}
		else
		{
			Guide.BeginShowMessageBox("Save disabled", "Your logged as guest and the save option has been disabled, the progress and preferences will not be saved, proceed anyway? ", new string[2] { "OK", "Cancel" }, 0, MessageBoxIcon.Alert, confirmwarning, null);
		}
	}

	public override void LoadContent()
	{
		new ContentManager(base.ScreenManager.Game.Services, "Content");
		base.LoadContent();
	}

	public void confirmwarning(IAsyncResult result)
	{
		int? num = Guide.EndShowMessageBox(result);
		int? num2 = num;
		if (num2.GetValueOrDefault() == 0 && num2.HasValue)
		{
			ExitScreen();
			base.ScreenManager.settings.enablesaving = false;
		}
	}

	protected void OnCancel(object sender, PlayerIndexEventArgs e)
	{
		base.ScreenManager.audioManager.playsound("Back");
		OnCancel(e.PlayerIndex);
	}

	protected void OnCancelOptions(object sender, PlayerIndexEventArgs e)
	{
		base.ScreenManager.audioManager.playsound("Back");
		OnCancelOptions(e.PlayerIndex);
	}

	protected virtual void UpdateMenuEntryLocations()
	{
		float num = (float)Math.Pow(base.TransitionPosition, 2.0);
		Vector2 position = new Vector2(0f, base.ScreenManager.GraphicsDevice.Viewport.Height / 2);
		int num2 = -1;
		for (int i = 0; i < menuEntries.Count; i++)
		{
			MenuEntry menuEntry = menuEntries[i];
			if (i % 7 == 0)
			{
				position.Y = menupos.Y;
				num2++;
			}
			position.X = menupos.X - (float)(menuEntry.GetWidth(this) / 2);
			if (base.ScreenState == ScreenState.TransitionOn)
			{
				position.X -= num * 256f;
			}
			else
			{
				position.X -= num * 256f;
			}
			menuEntry.Position = position;
			position.Y += menuEntry.GetHeight(this);
		}
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		currentsettings = base.ScreenManager.settings;
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		for (int i = 0; i < menuEntries.Count; i++)
		{
			bool selected = base.IsActive && i == selectedEntry;
			menuEntries[i].Update(this, selected, gameTime);
		}
		base.ScreenManager.settings = currentsettings;
	}

	public override void Draw(GameTime gameTime)
	{
		UpdateMenuEntryLocations();
		GraphicsDevice graphicsDevice = base.ScreenManager.GraphicsDevice;
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		SpriteFont spriteFont = base.ScreenManager.Font;
		spriteBatch.Begin();
		for (int i = 0; i < menuEntries.Count; i++)
		{
			MenuEntry menuEntry = menuEntries[i];
			bool isSelected = base.IsActive && i == selectedEntry;
			menuEntry.Draw(this, isSelected, gameTime);
		}
		float num = (float)Math.Pow(base.TransitionPosition, 2.0);
		Vector2 position = new Vector2(graphicsDevice.Viewport.Width / 2, 80f);
		Vector2 origin = spriteFont.MeasureString(menuTitle) / 2f;
		Color color = new Color(192, 192, 192) * base.TransitionAlpha;
		float scale = 1.25f;
		position.Y -= num * 100f;
		spriteBatch.DrawString(spriteFont, menuTitle, position, color, 0f, origin, scale, SpriteEffects.None, 0f);
		spriteBatch.End();
	}

	public void drawfont(string stringa)
	{
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		spriteBatch.Begin();
		spriteBatch.DrawString(font, stringa, new Vector2(20f, 320f), Color.White);
		spriteBatch.End();
	}
}
