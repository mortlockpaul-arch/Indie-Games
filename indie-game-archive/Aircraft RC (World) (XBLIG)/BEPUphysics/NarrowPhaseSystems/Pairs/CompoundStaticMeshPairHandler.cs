using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.DataStructures;
using BEPUphysics.Entities;
using BEPUphysics.ResourceManagement;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a compound-static mesh collision pair.
/// </summary>
public class CompoundStaticMeshPairHandler : CompoundGroupPairHandler
{
	private StaticMesh mesh;

	public override Collidable CollidableB => mesh;

	public override Entity EntityB => null;

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		mesh = entryA as StaticMesh;
		if (mesh == null)
		{
			mesh = entryB as StaticMesh;
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
		RawList<CompoundChild> compoundChildList = Resources.GetCompoundChildList();
		compoundInfo.hierarchy.Tree.GetOverlaps(mesh.boundingBox, compoundChildList);
		for (int i = 0; i < compoundChildList.count; i++)
		{
			TryToAdd(compoundChildList.Elements[i].CollisionInformation, mesh, compoundChildList.Elements[i].Material);
		}
		Resources.GiveBack(compoundChildList);
	}
}
