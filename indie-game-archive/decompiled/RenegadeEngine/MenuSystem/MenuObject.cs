using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RenegadeEngine.MenuSystem;

public class MenuObject
{
	protected SpriteBatch spriteBatch = EngineManager.GetSpriteBatch;

	protected Texture2D image = AssetManager.GetAsset(ImageKeys.pixel);

	protected SpriteFont font = AssetManager.GetAsset(FontKeys.MenuFont);

	protected string textLabel = "MenuObject";

	protected Color textColor = Color.Blue;

	protected Color backgroundColor = Color.Transparent;

	protected Color highlightedColor = Color.Red;

	protected Color currentColor = Color.Blue;

	protected Rectangle boundary = new Rectangle(0, 0, 20, 10);

	protected int widthMargin = 4;

	protected int heightMargin = 2;

	protected Vector2 origin = new Vector2(10f, 5f);

	protected Vector2 position = new Vector2(4f, 2f);

	protected bool active = true;

	protected bool centerHoriz;

	protected bool centerVert;

	protected bool resizeToFont;

	public bool Active
	{
		get
		{
			return active;
		}
		set
		{
			active = value;
			if (!active)
			{
				Activated = null;
			}
		}
	}

	public bool Show { get; set; }

	public bool CenterHorizontally
	{
		get
		{
			return centerHoriz;
		}
		set
		{
			centerHoriz = value;
		}
	}

	public bool CenterVertically
	{
		get
		{
			return centerVert;
		}
		set
		{
			centerHoriz = value;
		}
	}

	public Texture2D Image
	{
		get
		{
			return image;
		}
		set
		{
			image = value;
		}
	}

	public SpriteFont Font
	{
		get
		{
			return font;
		}
		set
		{
			font = value;
			resize();
		}
	}

	public string Label
	{
		get
		{
			return textLabel;
		}
		set
		{
			textLabel = value;
		}
	}

	public Color TextColor
	{
		get
		{
			return textColor;
		}
		set
		{
			textColor = value;
		}
	}

	public Color BackgroundColor
	{
		get
		{
			return backgroundColor;
		}
		set
		{
			backgroundColor = value;
		}
	}

	public Color HighlightedColor
	{
		get
		{
			return highlightedColor;
		}
		set
		{
			highlightedColor = value;
		}
	}

	public Point Location
	{
		get
		{
			return boundary.Location;
		}
		set
		{
			boundary.Location = value;
		}
	}

	public Rectangle Boundary => boundary;

	public int Height
	{
		get
		{
			return boundary.Height;
		}
		set
		{
			boundary.Height = value;
			resize();
		}
	}

	public int Width
	{
		get
		{
			return boundary.Width;
		}
		set
		{
			boundary.Width = value;
			resize();
		}
	}

	public int X
	{
		get
		{
			return boundary.X;
		}
		set
		{
			boundary.X = value;
		}
	}

	public int Y
	{
		get
		{
			return boundary.Y;
		}
		set
		{
			boundary.Y = value;
		}
	}

	public event EventHandler<PlayerIndexEventArgs> Activated;

	public void HasFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			currentColor = highlightedColor;
		}
		else
		{
			currentColor = textColor;
		}
	}

	public MenuObject(string label, bool resizeToFont)
	{
		textLabel = label;
		this.resizeToFont = resizeToFont;
		Active = true;
		Show = true;
		CenterHorizontally = false;
		CenterVertically = false;
		if (resizeToFont)
		{
			resize();
		}
	}

	public MenuObject(string label, bool resizeToFont, MenuObject menuObject)
	{
		textLabel = label;
		this.resizeToFont = resizeToFont;
		image = menuObject.Image;
		font = menuObject.Font;
		Active = menuObject.Active;
		Show = menuObject.Show;
		CenterHorizontally = menuObject.CenterHorizontally;
		CenterVertically = menuObject.CenterVertically;
		textColor = menuObject.TextColor;
		highlightedColor = menuObject.HighlightedColor;
		backgroundColor = menuObject.BackgroundColor;
		Width = menuObject.Width;
		Height = menuObject.Height;
	}

	public virtual void Draw(GameTime gameTime)
	{
		if (Show)
		{
			if (CenterHorizontally)
			{
				boundary.X = Global.ScreenWidth / 2 - boundary.Width / 2;
			}
			if (CenterVertically)
			{
				boundary.Y = Global.ScreenHeight / 2 - boundary.Height / 2;
			}
			if (image != null)
			{
				spriteBatch.Draw(image, boundary, backgroundColor);
			}
			if (textLabel != null && !(textLabel == "") && !(textLabel == string.Empty))
			{
				position.X = boundary.Left + widthMargin;
				position.Y = boundary.Top + heightMargin;
				spriteBatch.DrawString(font, textLabel, position, currentColor);
			}
		}
	}

	public virtual void Draw(Rectangle area)
	{
		if (Show)
		{
			if (CenterHorizontally)
			{
				boundary.X = area.Width / 2 - boundary.Width / 2;
			}
			else if (CenterVertically)
			{
				boundary.Y = area.Height / 2 - boundary.Height / 2;
			}
			if (image != null)
			{
				spriteBatch.Draw(image, boundary, backgroundColor);
			}
			if (textLabel != null && !(textLabel == "") && !(textLabel == string.Empty))
			{
				position.X = boundary.Left + widthMargin;
				position.Y = boundary.Top + heightMargin;
				spriteBatch.DrawString(font, textLabel, position, currentColor);
			}
		}
	}

	protected virtual void resize()
	{
		if (textLabel != null && !(textLabel == "") && !(textLabel == string.Empty))
		{
			Vector2 vector = font.MeasureString(textLabel);
			origin.X = vector.X / 2f;
			origin.Y = font.LineSpacing / 2;
			boundary.Width = (int)(vector.X + (float)widthMargin + (float)widthMargin);
			boundary.Height = (int)(vector.Y + (float)heightMargin + (float)heightMargin);
		}
	}

	protected internal virtual void OnActivated(PlayerIndex playerIndex)
	{
		if (Activated != null)
		{
			Activated(this, new PlayerIndexEventArgs(playerIndex));
		}
	}
}
