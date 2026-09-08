using System;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

internal class MenuEntry
{
	private string text;

	private float selectionFade;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	public event EventHandler<PlayerIndexEventArgs> Selected;

	protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
	{
		if (Selected != null)
		{
			Selected(this, new PlayerIndexEventArgs(playerIndex));
		}
	}

	public MenuEntry(string text)
	{
		this.text = text;
	}

	public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
	{
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds * 4f;
		if (isSelected)
		{
			selectionFade = Math.Min(selectionFade + num, 1f);
		}
		else
		{
			selectionFade = Math.Max(selectionFade - num, 0f);
		}
	}

	public virtual void Draw(MenuScreen screen, Vector2 position, bool isSelected, GameTime gameTime)
	{
		Color color = (isSelected ? Color.Yellow : Color.White);
		double totalSeconds = gameTime.TotalGameTime.TotalSeconds;
		float num = (float)Math.Sin(totalSeconds * 6.0) + 1f;
		float scale = 1f + num * 0.05f * selectionFade;
		color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);
		ScreenManager screenManager = screen.ScreenManager;
		SpriteBatch spriteBatch = screenManager.SpriteBatch;
		SpriteFont font = screenManager.Font;
		spriteBatch.DrawString(origin: new Vector2(0f, font.LineSpacing / 2), spriteFont: screenManager.GoBoomMiddle, text: text, position: position, color: color, rotation: 0f, scale: scale, effects: SpriteEffects.None, layerDepth: 0f);
	}

	public virtual int GetHeight(MenuScreen screen)
	{
		return screen.ScreenManager.Font.LineSpacing;
	}
}
