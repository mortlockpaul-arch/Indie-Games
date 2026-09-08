using System;

namespace Microsoft.Xna.Framework;

internal abstract class GameHost
{
	internal abstract GameWindow Window { get; }

	internal event EventHandler<EventArgs> Suspend;

	internal event EventHandler<EventArgs> Resume;

	internal event EventHandler<EventArgs> Activated;

	internal event EventHandler<EventArgs> Deactivated;

	internal event EventHandler<EventArgs> Idle;

	internal event EventHandler<EventArgs> Exiting;

	internal abstract void Run();

	internal abstract void RunOneFrame();

	internal abstract void StartGameLoop();

	internal abstract void Exit();

	protected void OnSuspend()
	{
		if (Suspend != null)
		{
			Suspend(this, EventArgs.Empty);
		}
	}

	protected void OnResume()
	{
		if (Resume != null)
		{
			Resume(this, EventArgs.Empty);
		}
	}

	protected void OnActivated()
	{
		if (Activated != null)
		{
			Activated(this, EventArgs.Empty);
		}
	}

	protected void OnDeactivated()
	{
		if (Deactivated != null)
		{
			Deactivated(this, EventArgs.Empty);
		}
	}

	protected void OnIdle()
	{
		if (Idle != null)
		{
			Idle(this, EventArgs.Empty);
		}
	}

	protected void OnExiting()
	{
		if (Exiting != null)
		{
			Exiting(this, EventArgs.Empty);
		}
	}

	internal virtual bool ShowMissingRequirementMessage(Exception exception)
	{
		return false;
	}
}
