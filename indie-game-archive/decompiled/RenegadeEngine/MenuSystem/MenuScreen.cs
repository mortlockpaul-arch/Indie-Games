using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RenegadeEngine.MenuSystem;

public abstract class MenuScreen
{
	protected MenuButton[] buttons;

	protected MenuButton highlightedButton;

	protected int highlightedIndex;

	protected SpriteBatch spriteBatch = EngineManager.GetSpriteBatch;

	protected GraphicsDevice device = EngineManager.GetGraphicsDeviceManager.GraphicsDevice;

	protected Texture2D backGround;

	protected bool isExiting;

	protected ScreenState currentState = ScreenState.TransitionIn;

	protected FadeTransition tracker = new FadeTransition();

	public Rectangle Boundary = new Rectangle(0, 0, Global.ScreenWidth, Global.ScreenHeight);

	public PlayerIndex ControllingPlayer;

	public bool IsPopUp;

	public bool IsExiting => isExiting;

	public TransitionTracker GetTracker => tracker;

	public ScreenState ScreenState => currentState;

	public float Transition => tracker.Transition;

	public event EventHandler Activated;

	public event EventHandler Deactivated;

	public event EventHandler Disposed;

	public void ChangeState(ScreenState newState)
	{
		currentState = newState;
		switch (currentState)
		{
		case ScreenState.TransitionOut:
			tracker.State = TransitionState.Out;
			break;
		case ScreenState.TransitionToBackground:
			tracker.State = TransitionState.PartialOut;
			break;
		case ScreenState.TransitionIn:
			tracker.State = TransitionState.In;
			break;
		case ScreenState.Active:
			tracker.State = TransitionState.Idle;
			break;
		}
	}

	public MenuScreen()
	{
		tracker.State = TransitionState.In;
		tracker.InCompleted += On_TrackerInCompleted;
		tracker.PartialCompleted += On_TrackerPartialCompleted;
		tracker.OutCompleted += On_TrackerOutCompleted;
	}

	public virtual void Dispose()
	{
		isExiting = true;
		currentState = ScreenState.TransitionOut;
		tracker.State = TransitionState.Out;
	}

	public virtual void Update(GameTime gameTime)
	{
		if (currentState == ScreenState.TransitionIn || currentState == ScreenState.TransitionOut || currentState == ScreenState.TransitionToBackground)
		{
			tracker.Update(gameTime);
		}
	}

	public virtual void UpdateInput(GameTime gameTime)
	{
		if (Input.MenuUp(ControllingPlayer))
		{
			highlightedButton.HasFocus(hasFocus: false);
			highlightedIndex--;
			if (highlightedIndex < 0)
			{
				highlightedIndex = buttons.Length - 1;
			}
			highlightedButton = buttons[highlightedIndex];
			highlightedButton.HasFocus(hasFocus: true);
		}
		if (Input.MenuDown(ControllingPlayer))
		{
			highlightedButton.HasFocus(hasFocus: false);
			highlightedIndex++;
			if (highlightedIndex >= buttons.Length)
			{
				highlightedIndex = 0;
			}
			highlightedButton = buttons[highlightedIndex];
			highlightedButton.HasFocus(hasFocus: true);
		}
		if (Input.MenuSelect(ControllingPlayer))
		{
			highlightedButton.OnActivated(ControllingPlayer);
		}
	}

	public virtual void Draw(GameTime gameTime)
	{
		spriteBatch.Begin();
		if (backGround != null)
		{
			spriteBatch.Draw(backGround, Boundary, Color.White);
		}
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].Draw(gameTime);
		}
		if (currentState != ScreenState.Active && currentState != ScreenState.Hidden)
		{
			tracker.Area = Boundary;
			tracker.Draw();
		}
		spriteBatch.End();
	}

	protected virtual void OrganizeButtonsVertically(Point location, MenuObject[] items, int spaceBetweenMenuObjects)
	{
		items[0].X = location.X;
		items[0].Y = location.Y + spaceBetweenMenuObjects;
		for (int i = 1; i < items.Length; i++)
		{
			items[i].X = location.X;
			items[i].Y = spaceBetweenMenuObjects + items[i - 1].Boundary.Bottom;
		}
	}

	protected internal virtual void On_Activated(EventArgs e)
	{
		if (Activated != null)
		{
			Activated(this, e);
		}
	}

	protected internal virtual void On_Deactivated(EventArgs e)
	{
		if (Deactivated != null)
		{
			Deactivated(this, e);
		}
	}

	protected internal virtual void On_Disposed(EventArgs e)
	{
		if (Disposed != null)
		{
			Disposed(this, e);
		}
	}

	protected internal virtual void On_TrackerInCompleted(object sender, EventArgs e)
	{
		currentState = ScreenState.Active;
		On_Activated(e);
	}

	protected internal virtual void On_TrackerPartialCompleted(object sender, EventArgs e)
	{
		currentState = ScreenState.Inactive;
		On_Deactivated(e);
	}

	protected internal virtual void On_TrackerOutCompleted(object sender, EventArgs e)
	{
		currentState = ScreenState.Hidden;
		if (isExiting)
		{
			On_Disposed(e);
		}
		else
		{
			On_Deactivated(e);
		}
	}
}
