using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionTests;
using BEPUphysics.Constraints;
using BEPUphysics.Constraints.Collision;
using BEPUphysics.DataStructures;
using BEPUphysics.Materials;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Superclass of pairs which manage multiple sub-collidable pairs.
/// </summary>
public abstract class GroupPairHandler : CollidablePairHandler, IPairHandlerParent
{
	private ContactManifoldConstraintGroup manifoldConstraintGroup;

	private Dictionary<CollidablePair, CollidablePairHandler> subPairs = new Dictionary<CollidablePair, CollidablePairHandler>();

	private BEPUphysics.DataStructures.HashSet<CollidablePair> containedPairs = new BEPUphysics.DataStructures.HashSet<CollidablePair>();

	private RawList<CollidablePair> pairsToRemove = new RawList<CollidablePair>();

	private int contactCount;

	/// <summary>
	///  Gets a list of the pairs associated with children.
	/// </summary>
	public ReadOnlyDictionary<CollidablePair, CollidablePairHandler> ChildPairs => new ReadOnlyDictionary<CollidablePair, CollidablePairHandler>(subPairs);

	/// <summary>
	/// Gets the number of contacts in the pair.
	/// </summary>
	protected internal override int ContactCount => contactCount;

	/// <summary>
	///  Constructs a new compound-convex pair handler.
	/// </summary>
	protected GroupPairHandler()
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
		foreach (CollidablePairHandler value in subPairs.Values)
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
		foreach (CollidablePairHandler value in subPairs.Values)
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
		foreach (CollidablePairHandler value in subPairs.Values)
		{
			value.CleanUp();
			value.Factory.GiveBack(value);
		}
		subPairs.Clear();
		base.CleanUp();
	}

	protected void TryToAdd(Collidable a, Collidable b)
	{
		TryToAdd(a, b, null, null);
	}

	protected void TryToAdd(Collidable a, Collidable b, Material materialA)
	{
		TryToAdd(a, b, materialA, null);
	}

	protected void TryToAdd(Collidable a, Collidable b, Material materialA, Material materialB)
	{
		CollisionRule collisionRule;
		if ((collisionRule = CollisionRules.collisionRuleCalculator(a, b)) >= CollisionRule.NoNarrowPhasePair)
		{
			return;
		}
		if (collisionRule < base.CollisionRule)
		{
			collisionRule = base.CollisionRule;
		}
		CollidablePair pair = new CollidablePair(a, b);
		if (!subPairs.ContainsKey(pair))
		{
			CollidablePairHandler pairHandler = NarrowPhaseHelper.GetPairHandler(ref pair, collisionRule);
			if (pairHandler != null)
			{
				pairHandler.UpdateMaterialProperties(materialA, materialB);
				pairHandler.Parent = this;
				subPairs.Add(pair, pairHandler);
			}
		}
		containedPairs.Add(pair);
	}

	protected abstract void UpdateContainedPairs();

	/// <summary>
	///  Updates the pair handler's contacts.
	/// </summary>
	/// <param name="dt">Timestep duration.</param>
	protected virtual void UpdateContacts(float dt)
	{
		UpdateContainedPairs();
		foreach (CollidablePair key in subPairs.Keys)
		{
			if (!containedPairs.Contains(key))
			{
				pairsToRemove.Add(key);
			}
		}
		for (int i = 0; i < pairsToRemove.count; i++)
		{
			CollidablePairHandler collidablePairHandler = subPairs[pairsToRemove.Elements[i]];
			subPairs.Remove(pairsToRemove.Elements[i]);
			collidablePairHandler.CleanUp();
			collidablePairHandler.Factory.GiveBack(collidablePairHandler);
		}
		containedPairs.Clear();
		pairsToRemove.Clear();
		foreach (CollidablePairHandler value in subPairs.Values)
		{
			if (value.BroadPhaseOverlap.collisionRule < CollisionRule.NoNarrowPhaseUpdate)
			{
				value.UpdateCollision(dt);
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
		foreach (CollidablePairHandler value in subPairs.Values)
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
		foreach (CollidablePairHandler value in subPairs.Values)
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
		foreach (CollidablePairHandler value in subPairs.Values)
		{
			value.ClearContacts();
		}
		base.ClearContacts();
	}
}
