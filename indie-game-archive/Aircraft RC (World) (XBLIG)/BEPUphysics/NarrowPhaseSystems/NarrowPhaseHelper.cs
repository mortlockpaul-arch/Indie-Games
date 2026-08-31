using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.NarrowPhaseSystems;

/// <summary>
///  Contains the collision managers dictionary and other helper methods for creating pairs.
/// </summary>
public static class NarrowPhaseHelper
{
	internal static Dictionary<TypePair, NarrowPhasePairFactory> collisionManagers;

	/// <summary>
	/// Gets the factories used by default to construct various pair types in the narrow phase.
	/// These do not necessarily reflect the state of the narrow phase helper's CollisionManagers dictionary
	/// if changes are made to its entries.
	/// </summary>
	public static Factories Factories { get; private set; }

	/// <summary>
	///  Gets or sets the dictionary that defines the factory to use for various type pairs.
	/// </summary>
	public static Dictionary<TypePair, NarrowPhasePairFactory> CollisionManagers
	{
		get
		{
			return collisionManagers;
		}
		set
		{
			collisionManagers = value;
		}
	}

	static NarrowPhaseHelper()
	{
		Factories = new Factories();
		collisionManagers = new Dictionary<TypePair, NarrowPhasePairFactory>();
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<BoxShape>), typeof(ConvexCollidable<BoxShape>)), Factories.BoxBox);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<BoxShape>), typeof(ConvexCollidable<SphereShape>)), Factories.BoxSphere);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<SphereShape>), typeof(ConvexCollidable<SphereShape>)), Factories.SphereSphere);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<BoxShape>), typeof(ConvexCollidable<TriangleShape>)), Factories.TriangleConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<SphereShape>), typeof(ConvexCollidable<TriangleShape>)), Factories.TriangleConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CapsuleShape>), typeof(ConvexCollidable<TriangleShape>)), Factories.TriangleConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TriangleShape>), typeof(ConvexCollidable<TriangleShape>)), Factories.TriangleConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CylinderShape>), typeof(ConvexCollidable<TriangleShape>)), Factories.TriangleConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConeShape>), typeof(ConvexCollidable<TriangleShape>)), Factories.TriangleConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TransformableShape>), typeof(ConvexCollidable<TriangleShape>)), Factories.TriangleConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<MinkowskiSumShape>), typeof(ConvexCollidable<TriangleShape>)), Factories.TriangleConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<WrappedShape>), typeof(ConvexCollidable<TriangleShape>)), Factories.TriangleConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConvexHullShape>), typeof(ConvexCollidable<TriangleShape>)), Factories.TriangleConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<BoxShape>), typeof(StaticMesh)), Factories.StaticMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<SphereShape>), typeof(StaticMesh)), Factories.StaticMeshSphere);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CapsuleShape>), typeof(StaticMesh)), Factories.StaticMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TriangleShape>), typeof(StaticMesh)), Factories.StaticMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CylinderShape>), typeof(StaticMesh)), Factories.StaticMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConeShape>), typeof(StaticMesh)), Factories.StaticMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TransformableShape>), typeof(StaticMesh)), Factories.StaticMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<MinkowskiSumShape>), typeof(StaticMesh)), Factories.StaticMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<WrappedShape>), typeof(StaticMesh)), Factories.StaticMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConvexHullShape>), typeof(StaticMesh)), Factories.StaticMeshConvex);
		collisionManagers.Add(new TypePair(typeof(TriangleCollidable), typeof(StaticMesh)), Factories.StaticMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<BoxShape>), typeof(Terrain)), Factories.TerrainConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<SphereShape>), typeof(Terrain)), Factories.TerrainSphere);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CapsuleShape>), typeof(Terrain)), Factories.TerrainConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TriangleShape>), typeof(Terrain)), Factories.TerrainConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CylinderShape>), typeof(Terrain)), Factories.TerrainConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConeShape>), typeof(Terrain)), Factories.TerrainConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TransformableShape>), typeof(Terrain)), Factories.TerrainConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<MinkowskiSumShape>), typeof(Terrain)), Factories.TerrainConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<WrappedShape>), typeof(Terrain)), Factories.TerrainConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConvexHullShape>), typeof(Terrain)), Factories.TerrainConvex);
		collisionManagers.Add(new TypePair(typeof(TriangleCollidable), typeof(Terrain)), Factories.TerrainConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<BoxShape>), typeof(InstancedMesh)), Factories.InstancedMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<SphereShape>), typeof(InstancedMesh)), Factories.InstancedMeshSphere);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CapsuleShape>), typeof(InstancedMesh)), Factories.InstancedMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TriangleShape>), typeof(InstancedMesh)), Factories.InstancedMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CylinderShape>), typeof(InstancedMesh)), Factories.InstancedMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConeShape>), typeof(InstancedMesh)), Factories.InstancedMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TransformableShape>), typeof(InstancedMesh)), Factories.InstancedMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<MinkowskiSumShape>), typeof(InstancedMesh)), Factories.InstancedMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<WrappedShape>), typeof(InstancedMesh)), Factories.InstancedMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConvexHullShape>), typeof(InstancedMesh)), Factories.InstancedMeshConvex);
		collisionManagers.Add(new TypePair(typeof(TriangleCollidable), typeof(InstancedMesh)), Factories.InstancedMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<BoxShape>), typeof(CompoundCollidable)), Factories.CompoundConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<SphereShape>), typeof(CompoundCollidable)), Factories.CompoundConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CapsuleShape>), typeof(CompoundCollidable)), Factories.CompoundConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TriangleShape>), typeof(CompoundCollidable)), Factories.CompoundConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CylinderShape>), typeof(CompoundCollidable)), Factories.CompoundConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConeShape>), typeof(CompoundCollidable)), Factories.CompoundConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TransformableShape>), typeof(CompoundCollidable)), Factories.CompoundConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<MinkowskiSumShape>), typeof(CompoundCollidable)), Factories.CompoundConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<WrappedShape>), typeof(CompoundCollidable)), Factories.CompoundConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConvexHullShape>), typeof(CompoundCollidable)), Factories.CompoundConvex);
		collisionManagers.Add(new TypePair(typeof(TriangleCollidable), typeof(CompoundCollidable)), Factories.CompoundConvex);
		collisionManagers.Add(new TypePair(typeof(CompoundCollidable), typeof(CompoundCollidable)), Factories.CompoundCompound);
		collisionManagers.Add(new TypePair(typeof(CompoundCollidable), typeof(StaticMesh)), Factories.CompoundStaticMesh);
		collisionManagers.Add(new TypePair(typeof(CompoundCollidable), typeof(Terrain)), Factories.CompoundTerrain);
		collisionManagers.Add(new TypePair(typeof(CompoundCollidable), typeof(InstancedMesh)), Factories.CompoundInstancedMesh);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<BoxShape>), typeof(MobileMeshCollidable)), Factories.MobileMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<SphereShape>), typeof(MobileMeshCollidable)), Factories.MobileMeshSphere);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CapsuleShape>), typeof(MobileMeshCollidable)), Factories.MobileMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TriangleShape>), typeof(MobileMeshCollidable)), Factories.MobileMeshTriangle);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CylinderShape>), typeof(MobileMeshCollidable)), Factories.MobileMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConeShape>), typeof(MobileMeshCollidable)), Factories.MobileMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TransformableShape>), typeof(MobileMeshCollidable)), Factories.MobileMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<MinkowskiSumShape>), typeof(MobileMeshCollidable)), Factories.MobileMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<WrappedShape>), typeof(MobileMeshCollidable)), Factories.MobileMeshConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConvexHullShape>), typeof(MobileMeshCollidable)), Factories.MobileMeshConvex);
		collisionManagers.Add(new TypePair(typeof(CompoundCollidable), typeof(MobileMeshCollidable)), Factories.CompoundMobileMesh);
		collisionManagers.Add(new TypePair(typeof(MobileMeshCollidable), typeof(StaticMesh)), Factories.MobileMeshStaticMesh);
		collisionManagers.Add(new TypePair(typeof(MobileMeshCollidable), typeof(InstancedMesh)), Factories.MobileMeshInstancedMesh);
		collisionManagers.Add(new TypePair(typeof(MobileMeshCollidable), typeof(Terrain)), Factories.MobileMeshTerrain);
		collisionManagers.Add(new TypePair(typeof(MobileMeshCollidable), typeof(MobileMeshCollidable)), Factories.MobileMeshMobileMesh);
		collisionManagers.Add(new TypePair(typeof(MobileMeshCollidable), typeof(TriangleCollidable)), Factories.MobileMeshTriangle);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<BoxShape>), typeof(StaticGroup)), Factories.StaticGroupConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<SphereShape>), typeof(StaticGroup)), Factories.StaticGroupConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CapsuleShape>), typeof(StaticGroup)), Factories.StaticGroupConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TriangleShape>), typeof(StaticGroup)), Factories.StaticGroupConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<CylinderShape>), typeof(StaticGroup)), Factories.StaticGroupConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConeShape>), typeof(StaticGroup)), Factories.StaticGroupConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<TransformableShape>), typeof(StaticGroup)), Factories.StaticGroupConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<MinkowskiSumShape>), typeof(StaticGroup)), Factories.StaticGroupConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<WrappedShape>), typeof(StaticGroup)), Factories.StaticGroupConvex);
		collisionManagers.Add(new TypePair(typeof(ConvexCollidable<ConvexHullShape>), typeof(StaticGroup)), Factories.StaticGroupConvex);
		collisionManagers.Add(new TypePair(typeof(TriangleCollidable), typeof(StaticGroup)), Factories.StaticGroupConvex);
		collisionManagers.Add(new TypePair(typeof(CompoundCollidable), typeof(StaticGroup)), Factories.StaticGroupCompound);
		collisionManagers.Add(new TypePair(typeof(MobileMeshCollidable), typeof(StaticGroup)), Factories.StaticGroupMobileMesh);
		collisionManagers.Add(new TypePair(typeof(DetectorVolume), typeof(ConvexCollidable<BoxShape>)), Factories.DetectorVolumeConvex);
		collisionManagers.Add(new TypePair(typeof(DetectorVolume), typeof(ConvexCollidable<SphereShape>)), Factories.DetectorVolumeConvex);
		collisionManagers.Add(new TypePair(typeof(DetectorVolume), typeof(ConvexCollidable<CapsuleShape>)), Factories.DetectorVolumeConvex);
		collisionManagers.Add(new TypePair(typeof(DetectorVolume), typeof(ConvexCollidable<TriangleShape>)), Factories.DetectorVolumeConvex);
		collisionManagers.Add(new TypePair(typeof(DetectorVolume), typeof(ConvexCollidable<CylinderShape>)), Factories.DetectorVolumeConvex);
		collisionManagers.Add(new TypePair(typeof(DetectorVolume), typeof(ConvexCollidable<ConeShape>)), Factories.DetectorVolumeConvex);
		collisionManagers.Add(new TypePair(typeof(DetectorVolume), typeof(ConvexCollidable<TransformableShape>)), Factories.DetectorVolumeConvex);
		collisionManagers.Add(new TypePair(typeof(DetectorVolume), typeof(ConvexCollidable<MinkowskiSumShape>)), Factories.DetectorVolumeConvex);
		collisionManagers.Add(new TypePair(typeof(DetectorVolume), typeof(ConvexCollidable<WrappedShape>)), Factories.DetectorVolumeConvex);
		collisionManagers.Add(new TypePair(typeof(DetectorVolume), typeof(ConvexCollidable<ConvexHullShape>)), Factories.DetectorVolumeConvex);
		collisionManagers.Add(new TypePair(typeof(DetectorVolume), typeof(TriangleCollidable)), Factories.DetectorVolumeConvex);
		collisionManagers.Add(new TypePair(typeof(DetectorVolume), typeof(MobileMeshCollidable)), Factories.DetectorVolumeMobileMesh);
		collisionManagers.Add(new TypePair(typeof(DetectorVolume), typeof(CompoundCollidable)), Factories.DetectorVolumeCompound);
	}

	/// <summary>
	///  Gets a narrow phase pair for a given broad phase overlap.
	/// </summary>
	/// <param name="pair">Overlap to use to create the pair.</param>
	/// <returns>A INarrowPhasePair for the overlap.</returns>
	public static NarrowPhasePair GetPairHandler(ref BroadPhaseOverlap pair)
	{
		if (collisionManagers.TryGetValue(new TypePair(pair.entryA.GetType(), pair.entryB.GetType()), out var value))
		{
			NarrowPhasePair narrowPhasePair = value.GetNarrowPhasePair();
			narrowPhasePair.BroadPhaseOverlap = pair;
			narrowPhasePair.Factory = value;
			return narrowPhasePair;
		}
		ConvexCollidable convexCollidable = pair.entryA as ConvexCollidable;
		ConvexCollidable convexCollidable2 = pair.entryB as ConvexCollidable;
		if (convexCollidable != null && convexCollidable2 != null)
		{
			NarrowPhasePair narrowPhasePair2 = Factories.ConvexConvex.GetNarrowPhasePair();
			narrowPhasePair2.BroadPhaseOverlap = pair;
			narrowPhasePair2.Factory = Factories.ConvexConvex;
			return narrowPhasePair2;
		}
		return null;
	}

	/// <summary>
	///  Gets a narrow phase pair for a given pair of entries.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	///  <param name="entryB">Second entry in the pair.</param>
	///  <param name="rule">Collision rule governing the pair.</param>
	/// <returns>A NarrowPhasePair for the overlap.</returns>
	public static NarrowPhasePair GetPairHandler(BroadPhaseEntry entryA, BroadPhaseEntry entryB, CollisionRule rule)
	{
		BroadPhaseOverlap pair = new BroadPhaseOverlap(entryA, entryB, rule);
		return GetPairHandler(ref pair);
	}

	/// <summary>
	///  Gets a narrow phase pair for a given pair of entries.
	/// </summary>
	/// <param name="entryA">First entry in the pair.</param>
	///  <param name="entryB">Second entry in the pair.</param>
	/// <returns>AINarrowPhasePair for the overlap.</returns>
	public static NarrowPhasePair GetPairHandler(BroadPhaseEntry entryA, BroadPhaseEntry entryB)
	{
		BroadPhaseOverlap pair = new BroadPhaseOverlap(entryA, entryB);
		return GetPairHandler(ref pair);
	}

	/// <summary>
	/// Gets a collidable pair handler for a pair of collidables.
	/// </summary>
	/// <param name="pair">Pair of collidables to use to create the pair handler.</param>
	/// <param name="rule">Collision rule governing the pair.</param>
	/// <returns>CollidablePairHandler for the pair.</returns>
	public static CollidablePairHandler GetPairHandler(ref CollidablePair pair, CollisionRule rule)
	{
		BroadPhaseOverlap pair2 = new BroadPhaseOverlap(pair.collidableA, pair.collidableB, rule);
		return GetPairHandler(ref pair2) as CollidablePairHandler;
	}

	/// <summary>
	/// Gets a collidable pair handler for a pair of collidables.
	/// </summary>
	/// <param name="pair">Pair of collidables to use to create the pair handler.</param>
	/// <returns>CollidablePairHandler for the pair.</returns>
	public static CollidablePairHandler GetPairHandler(ref CollidablePair pair)
	{
		BroadPhaseOverlap pair2 = new BroadPhaseOverlap(pair.collidableA, pair.collidableB);
		return GetPairHandler(ref pair2) as CollidablePairHandler;
	}

	/// <summary>
	/// Tests the pair of collidables for intersection without regard for collision rules.
	/// </summary>
	/// <param name="pair">Pair to test.</param>
	/// <returns>Whether or not the pair is intersecting.</returns>
	public static bool Intersecting(ref CollidablePair pair)
	{
		CollidablePairHandler pairHandler = GetPairHandler(ref pair);
		pairHandler.SuppressEvents = true;
		pairHandler.UpdateCollision(0f);
		bool result = pairHandler.ContactCount > 0;
		pairHandler.SuppressEvents = false;
		pairHandler.CleanUp();
		pairHandler.Factory.GiveBack(pairHandler);
		return result;
	}
}
