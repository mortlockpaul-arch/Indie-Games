using System;
using System.Collections.Generic;
using r;

namespace W;

internal class _7 : r._6
{
	private List<h> a5h = new List<h>();

	public _7()
	{
		Enabled = true;
	}

	public void AddEventCreator(h creator)
	{
		if (creator.DeferredEventDispatcher == null)
		{
			creator.DeferredEventDispatcher = this;
			if (creator.IsActive)
			{
				a5h.Add(creator);
			}
			return;
		}
		throw new ArgumentException("The event creator is already managed by a dispatcher; it cannot be added.", "creator");
	}

	public void RemoveEventCreator(h creator)
	{
		if (creator.DeferredEventDispatcher == this)
		{
			creator.DeferredEventDispatcher = null;
			if (creator.IsActive)
			{
				a5h.Remove(creator);
			}
			return;
		}
		throw new ArgumentException("The event creator is managed by a different dispatcher; it cannot be removed.", "creator");
	}

	public void CreatorActivityChanged(h creator)
	{
		if (creator.IsActive)
		{
			if (a5h.Contains(creator))
			{
				throw new ArgumentException("The event creator was already active in the dispatcher; make sure the CreatorActivityChanged function is only called when the state actually changes.", "creator");
			}
			a5h.Add(creator);
		}
		else if (!a5h.Remove(creator))
		{
			throw new ArgumentException("The event creator not active in the dispatcher; make sure the CreatorActivityChanged function is only called when the state actually changes.", "creator");
		}
	}

	protected override void UpdateStage()
	{
		for (int num = a5h.Count - 1; num >= 0; num--)
		{
			a5h[num].DispatchEvents();
			if (num > a5h.Count)
			{
				num = a5h.Count;
			}
		}
	}
}
