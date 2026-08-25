using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RenegadeEngine.MenuSystem;

public class MenuButton : MenuObject
{
	protected Texture2D imageDepressed;

	protected Color depressedColor = Color.Blue;

	protected TimeSpan timeDepressed = TimeSpan.Zero;

	protected TimeSpan maxTimeDepressed = new TimeSpan(0, 0, 0, 0, 700);

	protected bool isDepressed;

	public Texture2D ImageDepressed
	{
		get
		{
			return imageDepressed;
		}
		set
		{
			imageDepressed = value;
		}
	}

	public Color DepressedColor
	{
		get
		{
			return depressedColor;
		}
		set
		{
			depressedColor = value;
		}
	}

	public TimeSpan TimeDepressed
	{
		get
		{
			return timeDepressed;
		}
		set
		{
			timeDepressed = value;
		}
	}

	public void ResizeToLabel()
	{
		Vector2 vector = font.MeasureString(base.Label);
		boundary.Width = (int)vector.X + widthMargin + widthMargin;
		boundary.Height = (int)vector.Y + heightMargin + heightMargin;
	}

	public MenuButton(string label, bool resizeToFont)
		: base(label, resizeToFont)
	{
	}

	public MenuButton(string label, bool resizeToFont, MenuButton menuButton)
		: base(label, resizeToFont, menuButton)
	{
		imageDepressed = menuButton.ImageDepressed;
		depressedColor = menuButton.DepressedColor;
		timeDepressed = menuButton.TimeDepressed;
	}

	public override void Draw(GameTime gameTime)
	{
		if (isDepressed)
		{
			if (timeDepressed < maxTimeDepressed)
			{
				timeDepressed += gameTime.ElapsedGameTime;
			}
			else
			{
				timeDepressed = TimeSpan.Zero;
				currentColor = highlightedColor;
				isDepressed = false;
			}
		}
		base.Draw(gameTime);
	}

	protected internal override void OnActivated(PlayerIndex playerIndex)
	{
		isDepressed = true;
		base.OnActivated(playerIndex);
	}
}
