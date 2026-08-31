using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.OtherSpaceStages;
using BEPUphysics.Threading;

namespace BEPUphysics.Collidables.Events;

/// <summary>
///  Event manager for BroadPhaseEntries (all types that live in the broad phase).
/// </summary>
/// <typeparam name="T">Some BroadPhaseEntry subclass.</typeparam>
public class EntryEventManager<T> : IDeferredEventCreator, IEntryEventTriggerer where T : BroadPhaseEntry
{
	protected internal int childDeferredEventCreators;

	private CompoundEventManager parent;

	protected internal T owner;

	private DeferredEventDispatcher deferredEventDispatcher;

	private readonly ConcurrentDeque<EventStoragePairCreated> eventStoragePairCreated = new ConcurrentDeque<EventStoragePairCreated>(0);

	private readonly ConcurrentDeque<EventStoragePairRemoved> eventStoragePairRemoved = new ConcurrentDeque<EventStoragePairRemoved>(0);

	private readonly ConcurrentDeque<EventStoragePairUpdated> eventStoragePairUpdated = new ConcurrentDeque<EventStoragePairUpdated>(0);

	private bool isActive;

	/// <summary>
	/// Number of child deferred event creators.
	/// </summary>
	int IDeferredEventCreator.ChildDeferredEventCreators
	{
		get
		{
			return childDeferredEventCreators;
		}
		set
		{
			int num = childDeferredEventCreators;
			childDeferredEventCreators = value;
			if (childDeferredEventCreators == 0 && num != 0)
			{
				if (EventsAreInactive())
				{
					((IDeferredEventCreator)this).IsActive = false;
				}
			}
			else if (childDeferredEventCreators != 0 && num == 0)
			{
				((IDeferredEventCreator)this).IsActive = true;
			}
		}
	}

	/// <summary>
	/// The parent of the event manager, if any.
	/// </summary>
	protected internal CompoundEventManager Parent
	{
		get
		{
			return parent;
		}
		set
		{
			if (parent != null && isActive)
			{
				((IDeferredEventCreator)parent).ChildDeferredEventCreators--;
			}
			parent = value;
			if (parent != null && isActive)
			{
				((IDeferredEventCreator)parent).ChildDeferredEventCreators++;
			}
		}
	}

	/// <summary>
	///  Owner of the event manager.
	/// </summary>
	public T Owner
	{
		get
		{
			return owner;
		}
		protected internal set
		{
			owner = value;
		}
	}

	DeferredEventDispatcher IDeferredEventCreator.DeferredEventDispatcher
	{
		get
		{
			return deferredEventDispatcher;
		}
		set
		{
			deferredEventDispatcher = value;
		}
	}

	bool IDeferredEventCreator.IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			if (!isActive && value)
			{
				isActive = true;
				if (parent != null)
				{
					((IDeferredEventCreator)parent).ChildDeferredEventCreators++;
				}
				if (deferredEventDispatcher != null)
				{
					deferredEventDispatcher.CreatorActivityChanged(this);
				}
			}
			else if (isActive && !value)
			{
				isActive = false;
				if (parent != null)
				{
					((IDeferredEventCreator)parent).ChildDeferredEventCreators--;
				}
				if (deferredEventDispatcher != null)
				{
					deferredEventDispatcher.CreatorActivityChanged(this);
				}
			}
		}
	}

	/// <summary>
	/// Fires when this entity's bounding box newly overlaps another entity's bounding box.
	/// </summary>
	public event PairCreatedEventHandler<T> PairCreated
	{
		add
		{
			InternalPairCreated += value;
			AddToEventfuls();
		}
		remove
		{
			InternalPairCreated -= value;
			VerifyEventStatus();
		}
	}

	/// <summary>
	/// Fires when this entity's bounding box no longer overlaps another entity's bounding box.
	/// </summary>
	public event PairRemovedEventHandler<T> PairRemoved
	{
		add
		{
			InternalPairRemoved += value;
			AddToEventfuls();
		}
		remove
		{
			InternalPairRemoved -= value;
			VerifyEventStatus();
		}
	}

	/// <summary>
	/// Fires when a pair is updated.
	/// </summary>
	public event PairUpdatedEventHandler<T> PairUpdated
	{
		add
		{
			InternalPairUpdated += value;
			AddToEventfuls();
		}
		remove
		{
			InternalPairUpdated -= value;
			VerifyEventStatus();
		}
	}

	/// <summary>
	/// Fires when a pair is updated.
	/// Unlike the PairUpdated event, this event will run inline instead of at the end of the space's update.
	/// Some operations are unsupported while the engine is updating, and be especially careful if internal multithreading is enabled.
	/// </summary>
	public event PairUpdatingEventHandler<T> PairUpdating;

	/// <summary>
	/// Fires when this entity's bounding box newly overlaps another entity's bounding box.
	/// Unlike the PairCreated event, this event will run inline instead of at the end of the space's update.
	/// Some operations are unsupported while the engine is updating, and be especially careful if internal multithreading is enabled.
	/// </summary>
	public event CreatingPairEventHandler<T> CreatingPair;

	/// <summary>
	/// Fires when this entity's bounding box no longer overlaps another entity's bounding box.
	/// Unlike the PairRemoved event, this event will run inline instead of at the end of the space's update.
	/// Some operations are unsupported while the engine is updating, and be especially careful if internal multithreading is enabled.
	/// </summary>
	public event RemovingPairEventHandler<T> RemovingPair;

	private event PairCreatedEventHandler<T> InternalPairCreated;

	private event PairRemovedEventHandler<T> InternalPairRemoved;

	private event PairUpdatedEventHandler<T> InternalPairUpdated;

	/// <summary>
	/// Removes the entity from the space's list of eventful entities if no events are active.
	/// </summary>
	protected void VerifyEventStatus()
	{
		if (EventsAreInactive() && childDeferredEventCreators == 0)
		{
			((IDeferredEventCreator)this).IsActive = false;
		}
	}

	protected virtual bool EventsAreInactive()
	{
		if (InternalPairCreated == null && InternalPairRemoved == null)
		{
			return InternalPairUpdated == null;
		}
		return false;
	}

	protected void AddToEventfuls()
	{
		((IDeferredEventCreator)this).IsActive = true;
	}

	void IDeferredEventCreator.DispatchEvents()
	{
		DispatchEvents();
	}

	protected virtual void DispatchEvents()
	{
		EventStoragePairCreated item;
		while (eventStoragePairCreated.TryUnsafeDequeueFirst(out item))
		{
			if (InternalPairCreated != null)
			{
				InternalPairCreated(owner, item.other, item.pair);
			}
		}
		EventStoragePairRemoved item2;
		while (eventStoragePairRemoved.TryUnsafeDequeueFirst(out item2))
		{
			if (InternalPairRemoved != null)
			{
				InternalPairRemoved(owner, item2.other);
			}
		}
		EventStoragePairUpdated item3;
		while (eventStoragePairUpdated.TryUnsafeDequeueFirst(out item3))
		{
			if (InternalPairUpdated != null)
			{
				InternalPairUpdated(owner, item3.other, item3.pair);
			}
		}
	}

	public void OnPairCreated(BroadPhaseEntry other, NarrowPhasePair collisionPair)
	{
		if (InternalPairCreated != null)
		{
			eventStoragePairCreated.Enqueue(new EventStoragePairCreated(other, collisionPair));
		}
		if (CreatingPair != null)
		{
			CreatingPair(owner, other, collisionPair);
		}
	}

	public void OnPairRemoved(BroadPhaseEntry other)
	{
		if (InternalPairRemoved != null)
		{
			eventStoragePairRemoved.Enqueue(new EventStoragePairRemoved(other));
		}
		if (RemovingPair != null)
		{
			RemovingPair(owner, other);
		}
	}

	public void OnPairUpdated(BroadPhaseEntry other, NarrowPhasePair collisionPair)
	{
		if (InternalPairUpdated != null)
		{
			eventStoragePairUpdated.Enqueue(new EventStoragePairUpdated(other, collisionPair));
		}
		if (PairUpdating != null)
		{
			PairUpdating(owner, other, collisionPair);
		}
	}

	/// <summary>
	///  Removes all event hooks from the manager.
	/// </summary>
	public virtual void RemoveAllEvents()
	{
		PairUpdating = null;
		CreatingPair = null;
		RemovingPair = null;
		InternalPairCreated = null;
		InternalPairRemoved = null;
		InternalPairUpdated = null;
		VerifyEventStatus();
	}
}
