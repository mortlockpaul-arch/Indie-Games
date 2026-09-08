using System;

namespace Microsoft.Xna.Framework.Graphics;

[Serializable]
public class DisplayMode
{
	public float AspectRatio
	{
		get
		{
			if (Height == 0)
			{
				return 0f;
			}
			return (float)Width / (float)Height;
		}
	}

	public SurfaceFormat Format { get; private set; }

	public int Height { get; private set; }

	public int Width { get; private set; }

	public Rectangle TitleSafeArea => new Rectangle(0, 0, Width, Height);

	internal DisplayMode(int width, int height, SurfaceFormat format)
	{
		Width = width;
		Height = height;
		Format = format;
	}

	public override string ToString()
	{
		return "{{Width:" + Width + " Height:" + Height + " Format:" + Format.ToString() + "}}";
	}
}
