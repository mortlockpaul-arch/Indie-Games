using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

internal class MenuEntry
{
	private string text;

	public bool isSelected;

	private float selectionFade;

	private Vector2 position;

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

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
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

	public virtual void Update(MenuScreen screen, bool Selected, GameTime gameTime)
	{
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds * 4f;
		if (Selected)
		{
			selectionFade = Math.Min(selectionFade + num, 1f);
			isSelected = true;
		}
		else
		{
			selectionFade = Math.Max(selectionFade - num, 0f);
			isSelected = false;
		}
	}

	public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
	{
		Color color = (isSelected ? Color.White : Color.White);
		_ = gameTime.TotalGameTime.TotalSeconds;
		float num = 1f;
		float scale = 1f + num * 0.05f * selectionFade;
		color *= screen.TransitionAlpha;
		ScreenManager screenManager = screen.ScreenManager;
		SpriteBatch spriteBatch = screenManager.SpriteBatch;
		SpriteFont spriteFont = (isSelected ? screenManager.Font : screenManager.unselectedFont);
		spriteBatch.DrawString(origin: new Vector2(0f, spriteFont.MeasureString(text).Y / 2f), spriteFont: spriteFont, text: text, position: position, color: color, rotation: 0f, scale: scale, effects: SpriteEffects.None, layerDepth: 0f);
	}

	public virtual int GetHeight(MenuScreen screen)
	{
		ScreenManager screenManager = screen.ScreenManager;
		SpriteFont spriteFont = (isSelected ? screenManager.Font : screenManager.unselectedFont);
		return spriteFont.LineSpacing;
	}

	public virtual int GetWidth(MenuScreen screen)
	{
		ScreenManager screenManager = screen.ScreenManager;
		SpriteFont spriteFont = (isSelected ? screenManager.Font : screenManager.unselectedFont);
		return (int)spriteFont.MeasureString(Text).X;
	}
}
