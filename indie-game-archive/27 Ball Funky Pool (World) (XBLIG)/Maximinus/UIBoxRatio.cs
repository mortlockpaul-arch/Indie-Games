using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class UIBoxRatio : UIBox
{
	public enum Direction
	{
		Left,
		Right,
		Top,
		Bottom
	}

	private float ratio;

	private Color bgCol;

	private Direction direction;

	private string name;

	private Vector2 namePos;

	public UIBoxRatio(string name, Point Center, Point Size, float depth, float initRatio, Direction direction, Color bgCol)
		: this(name, Utils.RectangleFromCenterAndSize(Center, Size), depth, initRatio, direction, bgCol)
	{
	}

	public UIBoxRatio(string name, Rectangle rect, float depth, float initRatio, Direction direction, Color bgCol)
		: base(rect, depth)
	{
		ratio = initRatio;
		this.direction = direction;
		this.bgCol = bgCol;
		this.name = name;
		if (name != "")
		{
			Vector2 vector = MaximinusGame.Draw2D.Font.MeasureString(name + "  ");
			namePos = new Vector2((float)rect.X - vector.X, rect.Y);
		}
	}

	public override void Draw()
	{
		base.Draw();
		Rectangle destinationRectangle = insideRect;
		switch (direction)
		{
		case Direction.Right:
			destinationRectangle.Width = (int)((float)destinationRectangle.Width * ratio);
			MaximinusGame.Draw2D.DrawString(ratio.ToString("0.00"), new Vector2(destinationRectangle.Center.X, destinationRectangle.Center.Y), Color.White);
			break;
		case Direction.Left:
		{
			int num2 = (int)((float)destinationRectangle.Width * (1f - ratio));
			destinationRectangle.Width -= num2;
			destinationRectangle.X += num2;
			break;
		}
		case Direction.Top:
		{
			int num = (int)((float)destinationRectangle.Height * (1f - ratio));
			destinationRectangle.Height -= num;
			destinationRectangle.Y += num;
			break;
		}
		case Direction.Bottom:
			destinationRectangle.Height = (int)((float)destinationRectangle.Height * ratio);
			break;
		}
		UIBox.sb.Draw(UIBox.blankTex, destinationRectangle, null, Utils.ColorWithAlpha(bgCol, transitionRatio), 0f, Vector2.Zero, SpriteEffects.None, depth + 0.01f);
		if (name != "")
		{
			MaximinusGame.Draw2D.DrawString(name, namePos, Utils.ColorWithAlpha(bgCol, 255));
		}
	}

	public void DrawThisRatio(float ratio)
	{
		this.ratio = ratio;
		Draw();
	}
}
