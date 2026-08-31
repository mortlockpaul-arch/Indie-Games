using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.Events;
using BEPUphysics.CollisionTests;
using BEPUphysics.CollisionTests.Manifolds;
using BEPUphysics.Constraints.Collision;
using BEPUphysics.Materials;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a standard pair handler that has a direct manifold and constraint.
/// </summary>
public abstract class StandardPairHandler : CollidablePairHandler
{
	/// <summary>
	/// Gets the contact manifold used by the pair handler.
	/// </summary>
	public abstract ContactManifold ContactManifold { get; }

	/// <summary>
	/// Gets the contact constraint usd by the pair handler.
	/// </summary>
	public abstract ContactManifoldConstraint ContactConstraint { get; }

	/// <summary>
	/// Gets the number of contacts associated with this pair handler.
	/// </summary>
	protected internal override int ContactCount => ContactManifold.contacts.count;

	/// <summary>
	///  Constructs a pair handler.
	/// </summary>
	protected StandardPairHandler()
	{
		ContactManifold.ContactAdded += OnContactAdded;
		ContactManifold.ContactRemoved += OnContactRemoved;
	}

	protected override void OnContactAdded(Contact contact)
	{
		ContactConstraint.AddContact(contact);
		base.OnContactAdded(contact);
	}

	protected override void OnContactRemoved(Contact contact)
	{
		ContactConstraint.RemoveContact(contact);
		base.OnContactRemoved(contact);
	}

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		ContactManifold.Initialize(CollidableA, CollidableB);
		ContactConstraint.Initialize(EntityA, EntityB, this);
		base.Initialize(entryA, entryB);
	}

	/// <summary>
	///  Forces an update of the pair's material properties.
	/// </summary>
	public override void UpdateMaterialProperties(Material a, Material b)
	{
		ContactConstraint.UpdateMaterialProperties(a ?? ((EntityA == null) ? null : EntityA.material), b ?? ((EntityB == null) ? null : EntityB.material));
	}

	/// <summary>
	/// Updates the material interaction properties of the pair handler's constraint.
	/// </summary>
	/// <param name="properties">Properties to use.</param>
	public override void UpdateMaterialProperties(InteractionProperties properties)
	{
		ContactConstraint.MaterialInteraction = properties;
	}

	/// <summary>
	///  Cleans up the pair handler.
	/// </summary>
	public override void CleanUp()
	{
		for (int num = ContactManifold.contacts.count - 1; num >= 0; num--)
		{
			OnContactRemoved(ContactManifold.contacts[num]);
		}
		if (ContactConstraint.solver != null)
		{
			ContactConstraint.pair = null;
			if (base.Parent != null)
			{
				base.Parent.RemoveSolverUpdateable(ContactConstraint);
			}
			else if (base.NarrowPhase != null)
			{
				base.NarrowPhase.NotifyUpdateableRemoved(ContactConstraint);
			}
		}
		else
		{
			ContactConstraint.CleanUpReferences();
			if (base.Parent != null && ContactConstraint.SolverGroup != null)
			{
				base.Parent.RemoveSolverUpdateable(ContactConstraint);
			}
		}
		ContactConstraint.CleanUp();
		base.CleanUp();
		ContactManifold.CleanUp();
	}

	/// <summary>
	///  Updates the pair handler.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void UpdateCollision(float dt)
	{
		Collidable collidableA = CollidableA;
		Collidable collidableB = CollidableB;
		IContactEventTriggerer eventTriggerer = collidableA.EventTriggerer;
		IContactEventTriggerer eventTriggerer2 = collidableB.EventTriggerer;
		if (!suppressEvents)
		{
			eventTriggerer.OnPairUpdated(collidableB, this);
			eventTriggerer2.OnPairUpdated(collidableA, this);
		}
		ContactManifold.Update(dt);
		if (ContactManifold.contacts.count > 0)
		{
			if (!suppressEvents)
			{
				eventTriggerer.OnPairTouching(collidableB, this);
				eventTriggerer2.OnPairTouching(collidableA, this);
			}
			if (previousContactCount == 0)
			{
				if (base.Parent != null)
				{
					base.Parent.AddSolverUpdateable(ContactConstraint);
				}
				else if (base.NarrowPhase != null)
				{
					base.NarrowPhase.NotifyUpdateableAdded(ContactConstraint);
				}
				if (!suppressEvents)
				{
					eventTriggerer.OnInitialCollisionDetected(collidableB, this);
					eventTriggerer2.OnInitialCollisionDetected(collidableA, this);
				}
			}
		}
		else if (previousContactCount > 0)
		{
			if (base.Parent != null)
			{
				base.Parent.RemoveSolverUpdateable(ContactConstraint);
			}
			else if (base.NarrowPhase != null)
			{
				base.NarrowPhase.NotifyUpdateableRemoved(ContactConstraint);
			}
			if (!suppressEvents)
			{
				eventTriggerer.OnCollisionEnded(collidableB, this);
				eventTriggerer2.OnCollisionEnded(collidableA, this);
			}
		}
		previousContactCount = ContactManifold.contacts.count;
	}

	/// <summary>
	/// Clears the contacts associated with this pair handler.
	/// </summary>
	public override void ClearContacts()
	{
		if (previousContactCount > 0)
		{
			if (base.Parent != null)
			{
				base.Parent.RemoveSolverUpdateable(ContactConstraint);
			}
			else if (base.NarrowPhase != null)
			{
				base.NarrowPhase.NotifyUpdateableRemoved(ContactConstraint);
			}
			if (!suppressEvents)
			{
				Collidable collidableA = CollidableA;
				Collidable collidableB = CollidableB;
				collidableA.EventTriggerer.OnCollisionEnded(collidableB, this);
				collidableB.EventTriggerer.OnCollisionEnded(collidableA, this);
			}
		}
		ContactManifold.ClearContacts();
		base.ClearContacts();
	}
}
