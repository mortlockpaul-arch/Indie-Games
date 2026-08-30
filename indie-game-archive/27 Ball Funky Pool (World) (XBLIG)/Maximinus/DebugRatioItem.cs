using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class DebugRatioItem
{
	public enum Direction
	{
		LEFT,
		RIGHT,
		TOP,
		BOTTOM
	}

	private Drawing2D draw2D;

	private float ratio;

	private Vector2 pos;

	private string name;

	private float rotation;

	public Color colorBack;

	public Color colorFrontFull;

	public Color colorFrontEmpty;

	public DebugRatioItem(Drawing2D draw2D, string name, Vector2 posAsRatio)
		: this(draw2D, name, posAsRatio, Direction.RIGHT)
	{
	}

	public DebugRatioItem(Drawing2D draw2D, string name, Vector2 posAsRatio, Direction direction)
	{
		this.draw2D = draw2D;
		this.name = name;
		switch (direction)
		{
		case Direction.RIGHT:
			rotation = 0f;
			break;
		case Direction.LEFT:
			rotation = (float)Math.PI;
			break;
		case Direction.TOP:
			rotation = (float)Math.PI / 2f;
			break;
		case Direction.BOTTOM:
			rotation = -(float)Math.PI / 2f;
			break;
		}
		UpdatePos(posAsRatio);
		Update(0f);
		colorBack = Color.White;
		colorFrontFull = Color.Green;
		colorFrontEmpty = Color.Red;
	}

	public void UpdatePos(Vector2 posAsRatio)
	{
		pos = new Vector2(draw2D.ScreenSize.X * posAsRatio.X, draw2D.ScreenSize.Y * posAsRatio.Y);
	}

	public void Update(float newRatio)
	{
		ratio = newRatio;
	}

	public void Draw()
	{
		Draw(showTextValue: false);
	}

	public void Draw(bool showTextValue)
	{
		Rectangle destinationRectangle = new Rectangle(0, 0, draw2D.ScreenSizePoint.X / 6, draw2D.ScreenSizePoint.Y / 25);
		destinationRectangle.X = (int)(pos.X - (float)(destinationRectangle.Width / 2));
		destinationRectangle.Y = (int)(pos.Y - (float)(destinationRectangle.Height / 2));
		Vector2 vector = new Vector2(destinationRectangle.Width / 2, destinationRectangle.Height / 2);
		draw2D.SpriteBatch.Draw(draw2D.BlankTex, destinationRectangle, null, colorBack, rotation, Vector2.Zero, SpriteEffects.None, 0.02f);
		vector = new Vector2((int)((float)destinationRectangle.Width * ratio), destinationRectangle.Height * 8 / 10);
		draw2D.SpriteBatch.Draw(draw2D.BlankTex, new Rectangle(destinationRectangle.X, destinationRectangle.Y + 1 + destinationRectangle.Height / 10, (int)vector.X, (int)vector.Y), null, Utils.LerpColor(colorFrontEmpty, colorFrontFull, Utils.clampRatio((ratio - 0.5f) * 2f)), rotation, Vector2.Zero, SpriteEffects.None, 0.01f);
		if (name != "")
		{
			Vector2 vector2 = draw2D.Font.MeasureString(name + "  ");
			draw2D.DrawString(name, new Vector2((float)destinationRectangle.X - vector2.X, destinationRectangle.Y), colorBack);
		}
		if (showTextValue)
		{
			string text = ratio.ToString("0.0000");
			draw2D.DrawString(text, new Vector2((float)destinationRectangle.Center.X - draw2D.Font.MeasureString(text).X / 2f, (float)destinationRectangle.Center.Y - draw2D.Font.MeasureString(text).Y / 2f), Color.Black);
		}
	}
}
