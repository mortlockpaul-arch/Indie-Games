using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework;

internal class GraphicsDeviceInformationComparer : IComparer<GraphicsDeviceInformation>
{
	private GraphicsDeviceManager graphics;

	public GraphicsDeviceInformationComparer(GraphicsDeviceManager graphicsComponent)
	{
		graphics = graphicsComponent;
	}

	public int Compare(GraphicsDeviceInformation d1, GraphicsDeviceInformation d2)
	{
		if (d1.GraphicsProfile != d2.GraphicsProfile)
		{
			if (d1.GraphicsProfile <= d2.GraphicsProfile)
			{
				return 1;
			}
			return -1;
		}
		PresentationParameters presentationParameters = d1.PresentationParameters;
		PresentationParameters presentationParameters2 = d2.PresentationParameters;
		if (presentationParameters.IsFullScreen != presentationParameters2.IsFullScreen)
		{
			if (graphics.IsFullScreen != presentationParameters.IsFullScreen)
			{
				return 1;
			}
			return -1;
		}
		int num = RankFormat(presentationParameters.BackBufferFormat);
		int num2 = RankFormat(presentationParameters2.BackBufferFormat);
		if (num != num2)
		{
			if (num >= num2)
			{
				return 1;
			}
			return -1;
		}
		if (presentationParameters.MultiSampleCount != presentationParameters2.MultiSampleCount)
		{
			if (presentationParameters.MultiSampleCount <= presentationParameters2.MultiSampleCount)
			{
				return 1;
			}
			return -1;
		}
		float num3 = ((graphics.PreferredBackBufferWidth != 0 && graphics.PreferredBackBufferHeight != 0) ? ((float)graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight) : ((float)GraphicsDeviceManager.DefaultBackBufferWidth / (float)GraphicsDeviceManager.DefaultBackBufferHeight));
		float num4 = (float)presentationParameters.BackBufferWidth / (float)presentationParameters.BackBufferHeight;
		float num5 = (float)presentationParameters2.BackBufferWidth / (float)presentationParameters2.BackBufferHeight;
		float num6 = Math.Abs(num4 - num3);
		float num7 = Math.Abs(num5 - num3);
		if (Math.Abs(num6 - num7) > 0.2f)
		{
			if (!(num6 < num7))
			{
				return 1;
			}
			return -1;
		}
		int num8 = 0;
		int num9 = 0;
		if (!graphics.IsFullScreen)
		{
			num8 = ((graphics.PreferredBackBufferWidth != 0 && graphics.PreferredBackBufferHeight != 0) ? (num9 = graphics.PreferredBackBufferWidth * graphics.PreferredBackBufferHeight) : (num9 = GraphicsDeviceManager.DefaultBackBufferWidth * GraphicsDeviceManager.DefaultBackBufferHeight));
		}
		else if (graphics.PreferredBackBufferWidth == 0 || graphics.PreferredBackBufferHeight == 0)
		{
			GraphicsAdapter adapter = d1.Adapter;
			num8 = adapter.CurrentDisplayMode.Width * adapter.CurrentDisplayMode.Height;
			GraphicsAdapter adapter2 = d2.Adapter;
			num9 = adapter2.CurrentDisplayMode.Width * adapter2.CurrentDisplayMode.Height;
		}
		else
		{
			num8 = (num9 = graphics.PreferredBackBufferWidth * graphics.PreferredBackBufferHeight);
		}
		int num10 = Math.Abs(presentationParameters.BackBufferWidth * presentationParameters.BackBufferHeight - num8);
		int num11 = Math.Abs(presentationParameters2.BackBufferWidth * presentationParameters2.BackBufferHeight - num9);
		if (num10 != num11)
		{
			if (num10 >= num11)
			{
				return 1;
			}
			return -1;
		}
		if (d1.Adapter != d2.Adapter)
		{
			if (d1.Adapter.IsDefaultAdapter)
			{
				return -1;
			}
			if (d2.Adapter.IsDefaultAdapter)
			{
				return 1;
			}
		}
		return 0;
	}

	private int RankFormat(SurfaceFormat format)
	{
		if (format == graphics.PreferredBackBufferFormat)
		{
			return 0;
		}
		if (SurfaceFormatBitDepth(format) == SurfaceFormatBitDepth(graphics.PreferredBackBufferFormat))
		{
			return 1;
		}
		return int.MaxValue;
	}

	private static int SurfaceFormatBitDepth(SurfaceFormat format)
	{
		switch (format)
		{
		case SurfaceFormat.Color:
		case SurfaceFormat.Rgba1010102:
			return 32;
		case SurfaceFormat.Bgr565:
		case SurfaceFormat.Bgra5551:
		case SurfaceFormat.Bgra4444:
			return 16;
		default:
			return 0;
		}
	}
}
