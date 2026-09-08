using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Eyehook.Framework;

public class StretchyTexture
{
	private Texture2D tex;

	private Rectangle nw;

	private Rectangle n;

	private Rectangle ne;

	private Rectangle w;

	private Rectangle c;

	private Rectangle e;

	private Rectangle sw;

	private Rectangle s;

	private Rectangle se;

	public StretchyTexture(Texture2D t, Rectangle center)
	{
		tex = t;
		nw = new Rectangle(0, 0, center.Left, center.Top);
		n = new Rectangle(center.Left, 0, center.Width, center.Top);
		ne = new Rectangle(center.Right, 0, tex.Width - center.Right, center.Top);
		w = new Rectangle(0, center.Top, center.Left, center.Height);
		c = center;
		e = new Rectangle(center.Right, center.Top, tex.Width - center.Right, center.Height);
		sw = new Rectangle(0, center.Bottom, center.Left, tex.Height - center.Bottom);
		s = new Rectangle(center.Left, center.Bottom, center.Width, tex.Height - center.Bottom);
		se = new Rectangle(center.Right, center.Bottom, tex.Width - center.Right, tex.Height - center.Bottom);
	}

	public void Draw(SpriteBatch spriteBatch, Rectangle destRect)
	{
		Draw(spriteBatch, destRect, Color.White);
	}

	public void Draw(SpriteBatch spriteBatch, Rectangle destRect, Color color)
	{
		int left = destRect.Left;
		int x = destRect.Left + w.Width;
		int x2 = destRect.Right - e.Width;
		int top = destRect.Top;
		int y = destRect.Top + n.Height;
		int y2 = destRect.Bottom - s.Height;
		int width = destRect.Width - w.Width - e.Width;
		int height = destRect.Height - n.Height - s.Height;
		Rectangle destinationRectangle = new Rectangle(left, top, nw.Width, nw.Height);
		Rectangle destinationRectangle2 = new Rectangle(x, top, width, nw.Height);
		Rectangle destinationRectangle3 = new Rectangle(x2, top, ne.Width, ne.Height);
		Rectangle destinationRectangle4 = new Rectangle(left, y, w.Width, height);
		Rectangle destinationRectangle5 = new Rectangle(x, y, width, height);
		Rectangle destinationRectangle6 = new Rectangle(x2, y, e.Width, height);
		Rectangle destinationRectangle7 = new Rectangle(left, y2, sw.Width, sw.Height);
		Rectangle destinationRectangle8 = new Rectangle(x, y2, width, s.Height);
		Rectangle destinationRectangle9 = new Rectangle(x2, y2, se.Width, se.Height);
		spriteBatch.Draw(tex, destinationRectangle, nw, color);
		spriteBatch.Draw(tex, destinationRectangle2, n, color);
		spriteBatch.Draw(tex, destinationRectangle3, ne, color);
		spriteBatch.Draw(tex, destinationRectangle4, w, color);
		spriteBatch.Draw(tex, destinationRectangle5, c, color);
		spriteBatch.Draw(tex, destinationRectangle6, e, color);
		spriteBatch.Draw(tex, destinationRectangle7, sw, color);
		spriteBatch.Draw(tex, destinationRectangle8, s, color);
		spriteBatch.Draw(tex, destinationRectangle9, se, color);
	}
}
