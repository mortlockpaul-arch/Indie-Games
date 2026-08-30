using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus.DebugTools;

public struct Layout
{
	public Rectangle ClientArea;

	public Rectangle SafeArea;

	public Layout(Rectangle clientArea, Rectangle safeArea)
	{
		ClientArea = clientArea;
		SafeArea = safeArea;
	}

	public Layout(Rectangle clientArea)
		: this(clientArea, clientArea)
	{
	}

	public Layout(Viewport viewport)
	{
		ClientArea = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);
		SafeArea = viewport.TitleSafeArea;
	}

	public Vector2 Place(Vector2 size, float horizontalMargin, float verticalMargine, Alignment alignment)
	{
		Rectangle region = new Rectangle(0, 0, (int)size.X, (int)size.Y);
		region = Place(region, horizontalMargin, verticalMargine, alignment);
		return new Vector2(region.X, region.Y);
	}

	public Rectangle Place(Rectangle region, float horizontalMargin, float verticalMargine, Alignment alignment)
	{
		if ((alignment & Alignment.Left) != Alignment.None)
		{
			region.X = ClientArea.X + (int)((float)ClientArea.Width * horizontalMargin);
		}
		else if ((alignment & Alignment.Right) != Alignment.None)
		{
			region.X = ClientArea.X + (int)((float)ClientArea.Width * (1f - horizontalMargin)) - region.Width;
		}
		else if ((alignment & Alignment.HorizontalCenter) != Alignment.None)
		{
			region.X = ClientArea.X + (ClientArea.Width - region.Width) / 2 + (int)(horizontalMargin * (float)ClientArea.Width);
		}
		if ((alignment & Alignment.Top) != Alignment.None)
		{
			region.Y = ClientArea.Y + (int)((float)ClientArea.Height * verticalMargine);
		}
		else if ((alignment & Alignment.Bottom) != Alignment.None)
		{
			region.Y = ClientArea.Y + (int)((float)ClientArea.Height * (1f - verticalMargine)) - region.Height;
		}
		else if ((alignment & Alignment.VerticalCenter) != Alignment.None)
		{
			region.Y = ClientArea.Y + (ClientArea.Height - region.Height) / 2 + (int)(verticalMargine * (float)ClientArea.Height);
		}
		if (region.Left < SafeArea.Left)
		{
			region.X = SafeArea.Left;
		}
		if (region.Right > SafeArea.Right)
		{
			region.X = SafeArea.Right - region.Width;
		}
		if (region.Top < SafeArea.Top)
		{
			region.Y = SafeArea.Top;
		}
		if (region.Bottom > SafeArea.Bottom)
		{
			region.Y = SafeArea.Bottom - region.Height;
		}
		return region;
	}
}
