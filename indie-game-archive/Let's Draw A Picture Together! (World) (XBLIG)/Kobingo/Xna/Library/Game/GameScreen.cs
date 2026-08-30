using System;
using Microsoft.Xna.Framework;

namespace Kobingo.Xna.Library.Game;

public class GameScreen
{
	public ScreenManager ScreenManager { get; private set; }

	public bool IsClosing { get; private set; }

	public bool IsPopup { get; set; }

	private GameTimeEventArgs GameTimeEventArgs { get; set; }

	public event EventHandler<GameTimeEventArgs> Updating;

	public event EventHandler<GameTimeEventArgs> Drawing;

	public event EventHandler Showing;

	public event EventHandler Closing;

	public GameScreen(ScreenManager screenManager)
	{
		if (screenManager == null)
		{
			throw new ArgumentNullException("screenManager");
		}
		ScreenManager = screenManager;
		GameTimeEventArgs = new GameTimeEventArgs();
	}

	public virtual void Update(GameTime gameTime, bool active)
	{
		if (Updating != null)
		{
			GameTimeEventArgs.Value = gameTime;
			Updating(this, GameTimeEventArgs);
		}
	}

	public virtual void Draw(GameTime gameTime, float transition)
	{
		if (Drawing != null)
		{
			GameTimeEventArgs.Value = gameTime;
			Drawing(this, GameTimeEventArgs);
		}
	}

	public virtual void HandleInput()
	{
	}

	public virtual void Show()
	{
		if (Showing != null)
		{
			Showing(this, EventArgs.Empty);
		}
		ScreenManager.Add(this);
	}

	public virtual void Close()
	{
		if (Closing != null)
		{
			Closing(this, EventArgs.Empty);
		}
		ScreenManager.Remove(this);
	}
}
