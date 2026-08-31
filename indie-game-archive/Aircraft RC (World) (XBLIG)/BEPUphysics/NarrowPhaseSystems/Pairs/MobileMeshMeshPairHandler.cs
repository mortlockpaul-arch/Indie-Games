using System;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.Entities;
using BEPUphysics.Materials;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a mobile mesh and mesh collision pair.
/// </summary>
public abstract class MobileMeshMeshPairHandler : MeshGroupPairHandler
{
	public MobileMeshCollidable mobileMesh;

	public override Collidable CollidableA => mobileMesh;

	public override Entity EntityA => mobileMesh.entity;

	protected override Material MaterialA => mobileMesh.entity.material;

	/// <summary>
	///  Initializes the pair handler.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	/// <param name="entryB">Second entry in the pair.</param>
	public override void Initialize(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		mobileMesh = entryA as MobileMeshCollidable;
		if (mobileMesh == null)
		{
			mobileMesh = entryB as MobileMeshCollidable;
			if (mobileMesh == null)
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
		mobileMesh = null;
	}
}
