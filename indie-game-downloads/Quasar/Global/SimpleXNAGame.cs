using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaToFna;

namespace Quasar.Global;

public class SimpleXNAGame : XNAGame
{
	private static SimpleXNAGame instance;

	public static SimpleXNAGame Instance => instance;

	public SimpleXNAGame(int maxScreenSizeX, int maxScreenSizeY, bool fullScreen, bool autoSize, bool vSynch, bool multisampling, bool mouseEnabled)
	{
		instance = this;
		graphics = new GraphicsDeviceManager(this);
		ReinitializeGraphics(maxScreenSizeX, maxScreenSizeY, fullScreen, autoSize, vSynch, multisampling);
		Init(graphics);
	}

	public SimpleXNAGame()
	{
		instance = this;
		graphics = new GraphicsDeviceManager(this);
		Init(graphics);
	}

	public void ReinitializeGraphics(int maxScreenSizeX, int maxScreenSizeY, bool fullScreen, bool autoSize, bool vSynch, bool multisampling)
	{
		int num = maxScreenSizeX;
		int num2 = maxScreenSizeY;
		if (autoSize)
		{
			num = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			num2 = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			if (maxScreenSizeX != -1 && maxScreenSizeY != -1 && (num > maxScreenSizeX || num2 > maxScreenSizeY))
			{
				num = maxScreenSizeX;
				num2 = maxScreenSizeY;
			}
		}
		graphics.PreferredBackBufferWidth = num;
		graphics.PreferredBackBufferHeight = num2;
		graphics.SynchronizeWithVerticalRetrace = vSynch;
		graphics.PreferMultiSampling = multisampling;
		graphics.IsFullScreen = fullScreen;
		try
		{
			GraphicsDeviceManager self = graphics;
			base.Window.IsBorderlessEXT = true;
			XnaToFnaHelper.ApplyChanges(self);
		}
		catch (Exception)
		{
		}
	}
}
