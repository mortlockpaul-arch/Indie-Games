using System;
using System.Collections.Generic;

namespace BEPUphysics.OtherSpaceStages;

/// <summary>
///  Manages the deferred events spawned by IDeferredEventCreators and dispatches them on update.
/// </summary>
public class DeferredEventDispatcher : ProcessingStage
{
	private List<IDeferredEventCreator> activeEventCreators = new List<IDeferredEventCreator>();

	/// <summary>
	///  Constructs the dispatcher.
	/// </summary>
	public DeferredEventDispatcher()
	{
		Enabled = true;
	}

	/// <summary>
	///  Adds an event creator.
	/// </summary>
	/// <param name="creator">Creator to add.</param>
	/// <exception cref="T:System.ArgumentException">Thrown when the creator is already managed by a dispatcher.</exception>
	public void AddEventCreator(IDeferredEventCreator creator)
	{
		if (creator.DeferredEventDispatcher == null)
		{
			creator.DeferredEventDispatcher = this;
			if (creator.IsActive)
			{
				activeEventCreators.Add(creator);
			}
			return;
		}
		throw new ArgumentException("The event creator is already managed by a dispatcher; it cannot be added.", "creator");
	}

	/// <summary>
	/// Removes an event creator.
	/// </summary>
	/// <param name="creator">Creator to remove.</param>
	public void RemoveEventCreator(IDeferredEventCreator creator)
	{
		if (creator.DeferredEventDispatcher == this)
		{
			creator.DeferredEventDispatcher = null;
			if (creator.IsActive)
			{
				activeEventCreators.Remove(creator);
			}
			return;
		}
		throw new ArgumentException("The event creator is managed by a different dispatcher; it cannot be removed.", "creator");
	}

	/// <summary>
	///  Notifies the dispatcher that the event activity of a creator has changed.
	/// </summary>
	/// <param name="creator">Cretor whose activity has changed.</param>
	/// <exception cref="T:System.ArgumentException">Thrown when the event creator's state hasn't changed.</exception>
	public void CreatorActivityChanged(IDeferredEventCreator creator)
	{
		if (creator.IsActive)
		{
			if (activeEventCreators.Contains(creator))
			{
				throw new ArgumentException("The event creator was already active in the dispatcher; make sure the CreatorActivityChanged function is only called when the state actually changes.", "creator");
			}
			activeEventCreators.Add(creator);
		}
		else if (!activeEventCreators.Remove(creator))
		{
			throw new ArgumentException("The event creator not active in the dispatcher; make sure the CreatorActivityChanged function is only called when the state actually changes.", "creator");
		}
	}

	protected override void UpdateStage()
	{
		for (int num = activeEventCreators.Count - 1; num >= 0; num--)
		{
			activeEventCreators[num].DispatchEvents();
			if (num > activeEventCreators.Count)
			{
				num = activeEventCreators.Count;
			}
		}
	}
}
