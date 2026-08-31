using BEPUphysics.CollisionTests;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.Threading;

namespace BEPUphysics.Collidables.Events;

/// <summary>
///  Event manager for collidables (things which can create contact points).
/// </summary>
/// <typeparam name="T">Some Collidable subclass.</typeparam>
public class ContactEventManager<T> : EntryEventManager<T>, IContactEventTriggerer, IEntryEventTriggerer where T : Collidable
{
	private readonly ConcurrentDeque<EventStorageContactCreated> eventStorageContactCreated = new ConcurrentDeque<EventStorageContactCreated>(0);

	private readonly ConcurrentDeque<EventStorageInitialCollisionDetected> eventStorageInitialCollisionDetected = new ConcurrentDeque<EventStorageInitialCollisionDetected>(0);

	private readonly ConcurrentDeque<EventStorageContactRemoved> eventStorageContactRemoved = new ConcurrentDeque<EventStorageContactRemoved>(0);

	private readonly ConcurrentDeque<EventStorageCollisionEnded> eventStorageCollisionEnded = new ConcurrentDeque<EventStorageCollisionEnded>(0);

	private readonly ConcurrentDeque<EventStoragePairTouched> eventStoragePairTouched = new ConcurrentDeque<EventStoragePairTouched>(0);

	/// <summary>
	/// Fires when the entity stops touching another entity.
	/// </summary>
	public event CollisionEndedEventHandler<T> CollisionEnded
	{
		add
		{
			InternalCollisionEnded += value;
			AddToEventfuls();
		}
		remove
		{
			InternalCollisionEnded -= value;
			VerifyEventStatus();
		}
	}

	/// <summary>
	/// Fires when the entity stops touching another entity.
	/// Unlike the CollisionEnded event, this event will run inline instead of at the end of the space's update.
	/// Some operations are unsupported while the engine is updating, and be especially careful if internal multithreading is enabled.
	/// </summary>
	public event CollisionEndingEventHandler<T> CollisionEnding;

	/// <summary>
	/// Fires when a pair is updated and there are contact points in it.
	/// </summary>
	public event PairTouchedEventHandler<T> PairTouched
	{
		add
		{
			InternalPairTouched += value;
			AddToEventfuls();
		}
		remove
		{
			InternalPairTouched -= value;
			VerifyEventStatus();
		}
	}

	/// <summary>
	/// Fires when a pair is updated and there are contact points in it.
	/// Unlike the PairTouched event, this event will run inline instead of at the end of the space's update.
	/// Some operations are unsupported while the engine is updating, and be especially careful if internal multithreading is enabled.
	/// </summary>
	public event PairTouchingEventHandler<T> PairTouching;

	/// <summary>
	/// Fires when this entity gains a contact point with another entity.
	/// </summary>
	public event ContactCreatedEventHandler<T> ContactCreated
	{
		add
		{
			InternalContactCreated += value;
			AddToEventfuls();
		}
		remove
		{
			InternalContactCreated -= value;
			VerifyEventStatus();
		}
	}

	/// <summary>
	/// Fires when this entity loses a contact point with another entity.
	/// </summary>
	public event ContactRemovedEventHandler<T> ContactRemoved
	{
		add
		{
			InternalContactRemoved += value;
			AddToEventfuls();
		}
		remove
		{
			InternalContactRemoved -= value;
			VerifyEventStatus();
		}
	}

	/// <summary>
	/// Fires when this entity gains a contact point with another entity.
	/// Unlike the ContactCreated event, this event will run inline instead of at the end of the space's update.
	/// Some operations are unsupported while the engine is updating, and be especially careful if internal multithreading is enabled.
	/// </summary>
	public event CreatingContactEventHandler<T> CreatingContact;

	/// <summary>
	/// Fires when a collision first occurs.
	/// Unlike the InitialCollisionDetected event, this event will run inline instead of at the end of the space's update.
	/// Some operations are unsupported while the engine is updating, and be especially careful if internal multithreading is enabled.
	/// </summary>
	public event DetectingInitialCollisionEventHandler<T> DetectingInitialCollision;

	/// <summary>
	/// Fires when a collision first occurs.
	/// </summary>
	public event InitialCollisionDetectedEventHandler<T> InitialCollisionDetected
	{
		add
		{
			InternalInitialCollisionDetected += value;
			AddToEventfuls();
		}
		remove
		{
			InternalInitialCollisionDetected -= value;
			VerifyEventStatus();
		}
	}

	/// <summary>
	/// Fires when this entity loses a contact point with another entity.
	/// Unlike the ContactRemoved event, this event will run inline instead of at the end of the space's update.
	/// Some operations are unsupported while the engine is updating, and be especially careful if internal multithreading is enabled.
	/// </summary>
	public event RemovingContactEventHandler<T> RemovingContact;

	private event CollisionEndedEventHandler<T> InternalCollisionEnded;

	private event PairTouchedEventHandler<T> InternalPairTouched;

	private event ContactCreatedEventHandler<T> InternalContactCreated;

	private event ContactRemovedEventHandler<T> InternalContactRemoved;

	private event InitialCollisionDetectedEventHandler<T> InternalInitialCollisionDetected;

	protected override bool EventsAreInactive()
	{
		if (InternalCollisionEnded == null && InternalPairTouched == null && InternalContactCreated == null && InternalContactRemoved == null && InternalInitialCollisionDetected == null)
		{
			return base.EventsAreInactive();
		}
		return false;
	}

	protected override void DispatchEvents()
	{
		EventStorageContactCreated item;
		while (eventStorageContactCreated.TryUnsafeDequeueFirst(out item))
		{
			if (InternalContactCreated != null)
			{
				InternalContactCreated(owner, item.other, item.pair, item.contactData);
			}
		}
		EventStorageInitialCollisionDetected item2;
		while (eventStorageInitialCollisionDetected.TryUnsafeDequeueFirst(out item2))
		{
			if (InternalInitialCollisionDetected != null)
			{
				InternalInitialCollisionDetected(owner, item2.other, item2.pair);
			}
		}
		EventStorageContactRemoved item3;
		while (eventStorageContactRemoved.TryUnsafeDequeueFirst(out item3))
		{
			if (InternalContactRemoved != null)
			{
				InternalContactRemoved(owner, item3.other, item3.pair, item3.contactData);
			}
		}
		EventStorageCollisionEnded item4;
		while (eventStorageCollisionEnded.TryUnsafeDequeueFirst(out item4))
		{
			if (InternalCollisionEnded != null)
			{
				InternalCollisionEnded(owner, item4.other, item4.pair);
			}
		}
		EventStoragePairTouched item5;
		while (eventStoragePairTouched.TryUnsafeDequeueFirst(out item5))
		{
			if (InternalPairTouched != null)
			{
				InternalPairTouched(owner, item5.other, item5.pair);
			}
		}
		base.DispatchEvents();
	}

	public void OnCollisionEnded(Collidable other, CollidablePairHandler collisionPair)
	{
		if (InternalCollisionEnded != null)
		{
			eventStorageCollisionEnded.Enqueue(new EventStorageCollisionEnded(other, collisionPair));
		}
		if (CollisionEnding != null)
		{
			CollisionEnding(owner, other, collisionPair);
		}
	}

	public void OnPairTouching(Collidable other, CollidablePairHandler collisionPair)
	{
		if (InternalPairTouched != null)
		{
			eventStoragePairTouched.Enqueue(new EventStoragePairTouched(other, collisionPair));
		}
		if (PairTouching != null)
		{
			PairTouching(owner, other, collisionPair);
		}
	}

	public void OnContactCreated(Collidable other, CollidablePairHandler collisionPair, Contact contact)
	{
		if (InternalContactCreated != null)
		{
			ContactData contactData = default(ContactData);
			contactData.Position = contact.Position;
			contactData.Normal = contact.Normal;
			contactData.PenetrationDepth = contact.PenetrationDepth;
			contactData.Id = contact.Id;
			eventStorageContactCreated.Enqueue(new EventStorageContactCreated(other, collisionPair, ref contactData));
		}
		if (CreatingContact != null)
		{
			CreatingContact(owner, other, collisionPair, contact);
		}
	}

	public void OnContactRemoved(Collidable other, CollidablePairHandler collisionPair, Contact contact)
	{
		if (InternalContactRemoved != null)
		{
			ContactData contactData = default(ContactData);
			contactData.Position = contact.Position;
			contactData.Normal = contact.Normal;
			contactData.PenetrationDepth = contact.PenetrationDepth;
			contactData.Id = contact.Id;
			eventStorageContactRemoved.Enqueue(new EventStorageContactRemoved(other, collisionPair, ref contactData));
		}
		if (RemovingContact != null)
		{
			RemovingContact(owner, other, collisionPair, contact);
		}
	}

	public void OnInitialCollisionDetected(Collidable other, CollidablePairHandler collisionPair)
	{
		if (InternalInitialCollisionDetected != null)
		{
			eventStorageInitialCollisionDetected.Enqueue(new EventStorageInitialCollisionDetected(other, collisionPair));
		}
		if (DetectingInitialCollision != null)
		{
			DetectingInitialCollision(owner, other, collisionPair);
		}
	}

	/// <summary>
	///  Removes all event hooks from the event manager.
	/// </summary>
	public override void RemoveAllEvents()
	{
		InternalCollisionEnded = null;
		InternalPairTouched = null;
		InternalContactCreated = null;
		InternalContactRemoved = null;
		InternalInitialCollisionDetected = null;
		CollisionEnding = null;
		DetectingInitialCollision = null;
		CreatingContact = null;
		RemovingContact = null;
		PairTouching = null;
		base.RemoveAllEvents();
	}
}
