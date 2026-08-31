using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.ResourceManagement;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a compound-instanced mesh collision pair.
/// </summary>
public class StaticGroupMobileMeshPairHandler : StaticGroupPairHandler
{
	private MobileMeshCollidable mesh;

	public override Collidable CollidableB => mesh;

	public override Entity EntityB => mesh.entity;

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		mesh = entryA as MobileMeshCollidable;
		if (mesh == null)
		{
			mesh = entryB as MobileMeshCollidable;
			if (mesh == null)
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
		mesh = null;
	}

	protected override void UpdateContainedPairs()
	{
		RawList<Collidable> collidableList = Resources.GetCollidableList();
		staticGroup.Shape.CollidableTree.GetOverlaps(mesh.boundingBox, collidableList);
		for (int i = 0; i < collidableList.count; i++)
		{
			StaticCollidable staticCollidable = collidableList.Elements[i] as StaticCollidable;
			TryToAdd(collidableList.Elements[i], mesh, staticCollidable?.Material);
		}
		Resources.GiveBack(collidableList);
	}
}
