using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.Collidables;
using BEPUphysics.CollisionTests;
using BEPUphysics.Entities;
using BEPUphysics.Materials;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Superclass of pairs between collidables that generate contact points.
/// </summary>
public abstract class CollidablePairHandler : NarrowPhasePair
{
	/// <summary>
	/// Index of this pair in CollidableA's pairs list.
	/// </summary>
	internal int listIndexA = -1;

	/// <summary>
	/// Index of this pair in CollidableB's pairs list.
	/// </summary>
	internal int listIndexB = -1;

	protected internal int previousContactCount;

	protected internal float timeOfImpact = 1f;

	protected bool suppressEvents;

	/// <summary>
	/// Gets the first collidable associated with the pair.
	/// </summary>
	public abstract Collidable CollidableA { get; }

	/// <summary>
	/// Gets the second collidable associated with the pair.
	/// </summary>
	public abstract Collidable CollidableB { get; }

	/// <summary>
	/// Gets the first entity associated with the pair.  This could be null if no entity is associated with CollidableA.
	/// </summary>
	public abstract Entity EntityA { get; }

	/// <summary>
	/// Gets the second entity associated with the pair.  This could be null if no entity is associated with CollidableB.
	/// </summary>
	public abstract Entity EntityB { get; }

	protected internal abstract int ContactCount { get; }

	/// <summary>
	///  Gets the last computed time of impact of the pair handler.
	///  This is only computed when one of the members is a continuously
	///  updated object.
	/// </summary>
	public float TimeOfImpact => timeOfImpact;

	/// <summary>
	///  Gets or sets whether or not to suppress events from this pair handler.
	/// </summary>
	public bool SuppressEvents
	{
		get
		{
			return suppressEvents;
		}
		set
		{
			suppressEvents = value;
		}
	}

	/// <summary>
	///  Gets or sets the parent of this pair handler.
	///  Pairs with parents report to their parents various
	///  changes in state.  This is mainly used to support
	///  hierarchies of pairs for compound collisions.
	/// </summary>
	public IPairHandlerParent Parent { get; set; }

	/// <summary>
	///  Gets a list of the contacts in the pair and their associated constraint information.
	/// </summary>
	public ContactCollection Contacts { get; private set; }

	protected CollidablePairHandler()
	{
		Contacts = new ContactCollection(this);
	}

	/// <summary>
	///  Updates the time of impact for the pair.
	/// </summary>
	/// <param name="requester">Collidable requesting the update.</param>
	/// <param name="dt">Timestep duration.</param>
	public abstract void UpdateTimeOfImpact(Collidable requester, float dt);

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		if (!suppressEvents)
		{
			CollidableA.EventTriggerer.OnPairCreated(CollidableB, this);
			CollidableB.EventTriggerer.OnPairCreated(CollidableA, this);
		}
	}

	/// <summary>
	///  Called when the pair handler is added to the narrow phase.
	/// </summary>
	protected internal override void OnAddedToNarrowPhase()
	{
		CollidableA.AddPair(this, ref listIndexA);
		CollidableB.AddPair(this, ref listIndexB);
	}

	protected virtual void OnContactAdded(Contact contact)
	{
		if (!suppressEvents)
		{
			CollidableA.EventTriggerer.OnContactCreated(CollidableB, this, contact);
			CollidableB.EventTriggerer.OnContactCreated(CollidableA, this, contact);
		}
		if (Parent != null)
		{
			Parent.OnContactAdded(contact);
		}
	}

	protected virtual void OnContactRemoved(Contact contact)
	{
		if (!suppressEvents)
		{
			CollidableA.EventTriggerer.OnContactRemoved(CollidableB, this, contact);
			CollidableB.EventTriggerer.OnContactRemoved(CollidableA, this, contact);
		}
		if (Parent != null)
		{
			Parent.OnContactRemoved(contact);
		}
	}

	/// <summary>
	///  Cleans up the pair handler.
	/// </summary>
	public override void CleanUp()
	{
		if (previousContactCount > 0 && !suppressEvents)
		{
			CollidableA.EventTriggerer.OnCollisionEnded(CollidableB, this);
			CollidableB.EventTriggerer.OnCollisionEnded(CollidableA, this);
		}
		if (listIndexA != -1)
		{
			CollidableA.RemovePair(this, ref listIndexA);
			CollidableB.RemovePair(this, ref listIndexB);
		}
		if (!suppressEvents)
		{
			CollidableA.EventTriggerer.OnPairRemoved(CollidableB);
			CollidableB.EventTriggerer.OnPairRemoved(CollidableA);
		}
		broadPhaseOverlap = default(BroadPhaseOverlap);
		suppressEvents = false;
		timeOfImpact = 1f;
		Parent = null;
		previousContactCount = 0;
	}

	/// <summary>
	///  Forces an update of the pair's material properties.
	/// </summary>
	///  <param name="properties">Properties to use in the collision.</param>
	public abstract void UpdateMaterialProperties(InteractionProperties properties);

	/// <summary>
	///  Forces an update of the pair's material properties.
	/// </summary>
	///  <param name="materialA">First material to use.</param>
	///  <param name="materialB">Second material to use.</param>
	public abstract void UpdateMaterialProperties(Material materialA, Material materialB);

	/// <summary>
	///  Forces an update of the pair's material properties.
	///  Uses default choices (such as the owning entities' materials).
	/// </summary>
	public void UpdateMaterialProperties()
	{
		UpdateMaterialProperties(null, null);
	}

	protected internal abstract void GetContactInformation(int index, out ContactInformation info);

	/// <summary>
	/// Forces the pair handler to clean out its contacts.
	/// </summary>
	public virtual void ClearContacts()
	{
		previousContactCount = 0;
	}
}
