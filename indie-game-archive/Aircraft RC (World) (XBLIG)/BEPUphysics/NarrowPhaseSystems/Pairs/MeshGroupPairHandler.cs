using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionTests;
using BEPUphysics.Constraints;
using BEPUphysics.Constraints.Collision;
using BEPUphysics.DataStructures;
using BEPUphysics.Materials;
using BEPUphysics.ResourceManagement;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Superclass of pair handlers which have multiple index-based collidable child pairs.
/// </summary>
public abstract class MeshGroupPairHandler : CollidablePairHandler, IPairHandlerParent
{
	private ContactManifoldConstraintGroup manifoldConstraintGroup;

	private Dictionary<TriangleEntry, MobileMeshPairHandler> subPairs = new Dictionary<TriangleEntry, MobileMeshPairHandler>();

	private BEPUphysics.DataStructures.HashSet<TriangleEntry> containedPairs = new BEPUphysics.DataStructures.HashSet<TriangleEntry>();

	private RawList<TriangleEntry> pairsToRemove = new RawList<TriangleEntry>();

	private int contactCount;

	/// <summary>
	///  Gets a list of the pairs associated with children.
	/// </summary>
	public ReadOnlyDictionary<TriangleEntry, MobileMeshPairHandler> ChildPairs => new ReadOnlyDictionary<TriangleEntry, MobileMeshPairHandler>(subPairs);

	/// <summary>
	/// Material of the first collidable.
	/// </summary>
	protected abstract Material MaterialA { get; }

	/// <summary>
	/// Material of the second collidable.
	/// </summary>
	protected abstract Material MaterialB { get; }

	/// <summary>
	/// Gets the number of contacts in the pair.
	/// </summary>
	protected internal override int ContactCount => contactCount;

	/// <summary>
	///  Constructs a new compound-convex pair handler.
	/// </summary>
	protected MeshGroupPairHandler()
	{
		manifoldConstraintGroup = new ContactManifoldConstraintGroup();
	}

	/// <summary>
	///  Forces an update of the pair's material properties.
	/// </summary>
	/// <param name="a">Material of the first member of the pair.</param>
	/// <param name="b">Material of the second member of the pair.</param>
	public override void UpdateMaterialProperties(Material a, Material b)
	{
		foreach (MobileMeshPairHandler value in subPairs.Values)
		{
			value.UpdateMaterialProperties(a, b);
		}
	}

	/// <summary>
	/// Updates the material interaction properties of the pair handler's constraint.
	/// </summary>
	/// <param name="properties">Properties to use.</param>
	public override void UpdateMaterialProperties(InteractionProperties properties)
	{
		foreach (MobileMeshPairHandler value in subPairs.Values)
		{
			value.UpdateMaterialProperties(properties);
		}
	}

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		manifoldConstraintGroup.Initialize(EntityA, EntityB);
		base.Initialize(entryA, entryB);
	}

	/// <summary>
	///  Cleans up the pair handler.
	/// </summary>
	public override void CleanUp()
	{
		foreach (MobileMeshPairHandler value in subPairs.Values)
		{
			value.CleanUp();
			value.Factory.GiveBack(value);
		}
		subPairs.Clear();
		base.CleanUp();
	}

	protected void TryToAdd(int index)
	{
		TriangleEntry triangleEntry = new TriangleEntry
		{
			Index = index
		};
		if (!subPairs.ContainsKey(triangleEntry))
		{
			CollidablePair pair = new CollidablePair(CollidableA, triangleEntry.Collidable = GetOpposingCollidable(index));
			MobileMeshPairHandler mobileMeshPairHandler = (MobileMeshPairHandler)NarrowPhaseHelper.GetPairHandler(ref pair);
			if (mobileMeshPairHandler != null)
			{
				mobileMeshPairHandler.CollisionRule = base.CollisionRule;
				mobileMeshPairHandler.UpdateMaterialProperties(MaterialA, MaterialB);
				mobileMeshPairHandler.Parent = this;
				subPairs.Add(triangleEntry, mobileMeshPairHandler);
			}
		}
		containedPairs.Add(triangleEntry);
	}

	/// <summary>
	/// Get a collidable from CollidableB to represent the object at the given index.
	/// </summary>
	/// <param name="index">Index to create a collidable for.</param>
	/// <returns>Collidable for the object at the given index.</returns>
	protected abstract TriangleCollidable GetOpposingCollidable(int index);

	/// <summary>
	/// Configure a triangle from CollidableB to represent the object at the given index.
	/// </summary>
	/// <param name="entry">Entry to configure.</param>
	/// <param name="dt">Time step duration.</param>
	protected abstract void ConfigureCollidable(TriangleEntry entry, float dt);

	/// <summary>
	/// Cleans up the collidable.
	/// </summary>
	/// <param name="collidable">Collidable to clean up.</param>
	protected virtual void CleanUpCollidable(TriangleCollidable collidable)
	{
		Resources.GiveBack(collidable);
	}

	protected abstract void UpdateContainedPairs(float dt);

	/// <summary>
	///  Updates the pair handler's contacts.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	protected virtual void UpdateContacts(float dt)
	{
		UpdateContainedPairs(dt);
		foreach (TriangleEntry key in subPairs.Keys)
		{
			if (!containedPairs.Contains(key))
			{
				pairsToRemove.Add(key);
			}
		}
		for (int i = 0; i < pairsToRemove.count; i++)
		{
			MobileMeshPairHandler mobileMeshPairHandler = subPairs[pairsToRemove.Elements[i]];
			subPairs.Remove(pairsToRemove.Elements[i]);
			mobileMeshPairHandler.CleanUp();
			mobileMeshPairHandler.Factory.GiveBack(mobileMeshPairHandler);
		}
		containedPairs.Clear();
		pairsToRemove.Clear();
		foreach (KeyValuePair<TriangleEntry, MobileMeshPairHandler> subPair in subPairs)
		{
			if (subPair.Value.BroadPhaseOverlap.collisionRule < CollisionRule.NoNarrowPhaseUpdate)
			{
				ConfigureCollidable(subPair.Key, dt);
				subPair.Value.MeshManifold.parentContactCount = contactCount;
				subPair.Value.UpdateCollision(dt);
			}
		}
	}

	/// <summary>
	///  Updates the pair handler.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	public override void UpdateCollision(float dt)
	{
		if (!suppressEvents)
		{
			CollidableA.EventTriggerer.OnPairUpdated(CollidableB, this);
			CollidableB.EventTriggerer.OnPairUpdated(CollidableA, this);
		}
		UpdateContacts(dt);
		if (contactCount > 0)
		{
			if (!suppressEvents)
			{
				CollidableA.EventTriggerer.OnPairTouching(CollidableB, this);
				CollidableB.EventTriggerer.OnPairTouching(CollidableA, this);
			}
			if (previousContactCount == 0)
			{
				CollidableA.EventTriggerer.OnInitialCollisionDetected(CollidableB, this);
				CollidableB.EventTriggerer.OnInitialCollisionDetected(CollidableA, this);
			}
		}
		else if (previousContactCount > 0 && !suppressEvents)
		{
			CollidableA.EventTriggerer.OnCollisionEnded(CollidableB, this);
			CollidableB.EventTriggerer.OnCollisionEnded(CollidableA, this);
		}
		previousContactCount = contactCount;
	}

	/// <summary>
	///  Updates the time of impact for the pair.
	/// </summary>
	/// <param name="requester">Collidable requesting the update.</param>
	/// <param name="dt">Timestep duration.</param>
	public override void UpdateTimeOfImpact(Collidable requester, float dt)
	{
		timeOfImpact = 1f;
		foreach (MobileMeshPairHandler value in subPairs.Values)
		{
			if (base.BroadPhaseOverlap.entryA == requester)
			{
				value.UpdateTimeOfImpact((Collidable)value.BroadPhaseOverlap.entryA, dt);
			}
			else
			{
				value.UpdateTimeOfImpact((Collidable)value.BroadPhaseOverlap.entryB, dt);
			}
			if (value.timeOfImpact < timeOfImpact)
			{
				timeOfImpact = value.timeOfImpact;
			}
		}
	}

	protected internal override void GetContactInformation(int index, out ContactInformation info)
	{
		foreach (MobileMeshPairHandler value in subPairs.Values)
		{
			int count = value.Contacts.Count;
			if (index - count < 0)
			{
				value.GetContactInformation(index, out info);
				return;
			}
			index -= count;
		}
		throw new IndexOutOfRangeException("Contact index is not present in the pair.");
	}

	void IPairHandlerParent.AddSolverUpdateable(EntitySolverUpdateable addedItem)
	{
		manifoldConstraintGroup.Add(addedItem);
		if (manifoldConstraintGroup.SolverUpdateables.Count == 1)
		{
			if (base.Parent != null)
			{
				base.Parent.AddSolverUpdateable(manifoldConstraintGroup);
			}
			else if (base.NarrowPhase != null)
			{
				base.NarrowPhase.NotifyUpdateableAdded(manifoldConstraintGroup);
			}
		}
	}

	void IPairHandlerParent.RemoveSolverUpdateable(EntitySolverUpdateable removedItem)
	{
		manifoldConstraintGroup.Remove(removedItem);
		if (manifoldConstraintGroup.SolverUpdateables.Count == 0)
		{
			if (base.Parent != null)
			{
				base.Parent.RemoveSolverUpdateable(manifoldConstraintGroup);
			}
			else if (base.NarrowPhase != null)
			{
				base.NarrowPhase.NotifyUpdateableRemoved(manifoldConstraintGroup);
			}
		}
	}

	void IPairHandlerParent.OnContactAdded(Contact contact)
	{
		contactCount++;
		OnContactAdded(contact);
	}

	void IPairHandlerParent.OnContactRemoved(Contact contact)
	{
		contactCount--;
		OnContactRemoved(contact);
	}

	/// <summary>
	/// Clears the pair's contacts.
	/// </summary>
	public override void ClearContacts()
	{
		foreach (MobileMeshPairHandler value in subPairs.Values)
		{
			value.ClearContacts();
		}
		base.ClearContacts();
	}
}
