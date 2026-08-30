using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class UIBox
{
	protected static int border = -1;

	public static Color borderColDefault = Color.Transparent;

	protected static SpriteBatch sb;

	protected static Texture2D blankTex;

	protected Rectangle insideRect;

	public Color borderCol;

	protected float depth;

	protected Rectangle[] rects = new Rectangle[4];

	protected float transitionRatio;

	public Rectangle InsideRect => insideRect;

	public Rectangle Rect
	{
		set
		{
			ref Rectangle reference = ref rects[0];
			reference = new Rectangle(value.X, value.Y - border, value.Width, border * 2);
			ref Rectangle reference2 = ref rects[1];
			reference2 = new Rectangle(value.X, value.Y + value.Height - border, value.Width, border * 2);
			ref Rectangle reference3 = ref rects[2];
			reference3 = new Rectangle(value.X - border, value.Y, border * 2, value.Height);
			ref Rectangle reference4 = ref rects[3];
			reference4 = new Rectangle(value.X + value.Width - border, value.Y, border * 2, value.Height);
			insideRect = new Rectangle(rects[0].X + border, rects[2].Y + border, rects[0].Width - 2 * border, rects[2].Height - 2 * border);
		}
	}

	public static void InitializeStyle(int borderValue, Color borderColValue)
	{
		border = borderValue;
		borderColDefault = borderColValue;
	}

	public static void Initialize(Drawing2D draw2D)
	{
		sb = draw2D.SpriteBatch;
		blankTex = draw2D.BlankTex;
	}

	public UIBox(Rectangle rect, float depth)
	{
		Reset(rect, depth);
	}

	public void Reset(Rectangle rect, float depth)
	{
		this.depth = depth;
		borderCol = borderColDefault;
		Rect = rect;
		transitionRatio = 1f;
	}

	public void Draw(float transitionRatio)
	{
		this.transitionRatio = transitionRatio;
		Draw();
	}

	public virtual void Draw()
	{
		Rectangle[] array = rects;
		foreach (Rectangle destinationRectangle in array)
		{
			Color color = Utils.ColorWithAlpha(borderCol, transitionRatio * (float)(int)borderCol.A / 255f);
			sb.Draw(blankTex, destinationRectangle, null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
		}
	}
}
