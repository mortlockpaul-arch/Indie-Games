using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Eyehook.Framework;

public class ScreenManager
{
	private static ScreenManager screenManager;

	private Color backgroundColor;

	private readonly object screenLock = new object();

	private readonly object deadLock = new object();

	private List<Screen> screenList = new List<Screen>();

	private List<Screen> deadList = new List<Screen>();

	public readonly Rectangle viewportRect;

	public Effect ScreenEffect;

	private RenderTarget2D screenTarget;

	public static ScreenManager instance
	{
		get
		{
			if (screenManager == null)
			{
				screenManager = new ScreenManager();
			}
			return screenManager;
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

	~ScreenManager()
	{
	}

	private ScreenManager()
	{
		backgroundColor = Color.Gray;
		int width = MC.Game.GraphicsDevice.Viewport.Width;
		int height = MC.Game.GraphicsDevice.Viewport.Height;
		viewportRect = new Rectangle(0, 0, width, height);
		PresentationParameters presentationParameters = MC.Game.GraphicsDevice.PresentationParameters;
		screenTarget = new RenderTarget2D(MC.Game.GraphicsDevice, width, height, mipMap: false, presentationParameters.BackBufferFormat, presentationParameters.DepthStencilFormat);
	}

	public static void ForceReset()
	{
		screenManager = new ScreenManager();
	}

	private void activate(Screen screen)
	{
		screen.initialize();
		screen.loadContent(MC.Content);
		screen.transitionOn();
	}

	private void deactivate(Screen screen)
	{
		screen.transitionOff();
		screen.unloadContent();
	}

	public void addScreen(Screen screen)
	{
		lock (screenLock)
		{
			screenList.Add(screen);
			activate(screen);
		}
	}

	public bool containsScreen(Screen screen)
	{
		lock (screenLock)
		{
			return screenList.Contains(screen);
		}
	}

	public int indexOfScreen(Screen screen)
	{
		lock (screenLock)
		{
			return screenList.IndexOf(screen);
		}
	}

	public void insertBefore(Screen screen, Screen currentScreen)
	{
		insertScreen(screen, indexOfScreen(currentScreen));
	}

	public void insertScreen(Screen screen, int index)
	{
		lock (screenLock)
		{
			screenList.Insert(index, screen);
			activate(screen);
		}
	}

	public void removeScreen(Screen screen)
	{
		lock (deadLock)
		{
			if (!deadList.Contains(screen))
			{
				deadList.Add(screen);
			}
		}
	}

	public void removeAllScreens()
	{
		lock (screenLock)
		{
			for (int i = 0; i < screenList.Count; i++)
			{
				removeScreen(screenList[i]);
			}
		}
	}

	private void removeDeadScreens()
	{
		lock (deadLock)
		{
			lock (screenLock)
			{
				for (int i = 0; i < deadList.Count; i++)
				{
					Screen screen = deadList[i];
					screenList.Remove(screen);
					deactivate(screen);
				}
			}
			deadList.Clear();
		}
	}

	public void update(GameTime gameTime)
	{
		lock (screenLock)
		{
			bool flag = false;
			for (int num = screenList.Count - 1; num >= 0; num--)
			{
				Screen screen = screenList[num];
				if (!flag || screen.IsBackground)
				{
					screen.update(gameTime);
				}
				if (screen.IsModal)
				{
					flag = true;
				}
			}
		}
		if (deadList.Count > 0)
		{
			removeDeadScreens();
		}
	}

	public void draw(GameTime gameTime)
	{
		MC.Game.GraphicsDevice.SetRenderTarget(screenTarget);
		MC.Game.GraphicsDevice.Clear(BackgroundColor);
		lock (screenLock)
		{
			for (int i = 0; i < screenList.Count; i++)
			{
				screenList[i].draw(gameTime);
			}
		}
		MC.Game.GraphicsDevice.SetRenderTarget(null);
		MC.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, ScreenEffect);
		MC.SpriteBatch.Draw(screenTarget, viewportRect, Color.White);
		MC.SpriteBatch.End();
	}
}
