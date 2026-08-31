using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.ResourceManagement;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a compound and convex collision pair.
/// </summary>
public class CompoundConvexPairHandler : CompoundGroupPairHandler
{
	private ConvexCollidable convexInfo;

	public override Collidable CollidableB => convexInfo;

	public override Entity EntityB => convexInfo.entity;

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		convexInfo = entryA as ConvexCollidable;
		if (convexInfo == null)
		{
			convexInfo = entryB as ConvexCollidable;
			if (convexInfo == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
		}
		base.Initialize(entryA, entryB);
	}

	/// <summary>
	///  Cleans up the pair handler.
	/// </summary>
	public override void CleanUp()
	{
		base.CleanUp();
		convexInfo = null;
	}

	protected override void UpdateContainedPairs()
	{
		RawList<CompoundChild> compoundChildList = Resources.GetCompoundChildList();
		compoundInfo.hierarchy.Tree.GetOverlaps(convexInfo.boundingBox, compoundChildList);
		for (int i = 0; i < compoundChildList.count; i++)
		{
			TryToAdd(compoundChildList.Elements[i].CollisionInformation, CollidableB, compoundChildList.Elements[i].Material);
		}
		Resources.GiveBack(compoundChildList);
	}
}
