using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a compound-compound collision pair.
/// </summary>
public class CompoundPairHandler : CompoundGroupPairHandler
{
	private CompoundCollidable compoundInfoB;

	private RawList<TreeOverlapPair<CompoundChild, CompoundChild>> overlappedElements = new RawList<TreeOverlapPair<CompoundChild, CompoundChild>>();

	public override Collidable CollidableB => compoundInfoB;

	public override Entity EntityB => compoundInfoB.entity;

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		compoundInfoB = entryB as CompoundCollidable;
		if (compoundInfoB == null)
		{
			throw new Exception("Inappropriate types used to initialize pair.");
		}
		base.Initialize(entryA, entryB);
	}

	/// <summary>
	///  Cleans up the pair handler.
	/// </summary>
	public override void CleanUp()
	{
		base.CleanUp();
		compoundInfoB = null;
	}

	protected override void UpdateContainedPairs()
	{
		compoundInfo.hierarchy.Tree.GetOverlaps(compoundInfoB.hierarchy.Tree, overlappedElements);
		for (int i = 0; i < overlappedElements.count; i++)
		{
			TreeOverlapPair<CompoundChild, CompoundChild> treeOverlapPair = overlappedElements.Elements[i];
			TryToAdd(treeOverlapPair.OverlapA.CollisionInformation, treeOverlapPair.OverlapB.CollisionInformation, treeOverlapPair.OverlapA.Material, treeOverlapPair.OverlapB.Material);
		}
		overlappedElements.Clear();
	}
}
