using BEPUphysics.CollisionTests.Manifolds;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a static mesh-sphere collision pair.
/// </summary>
public class StaticMeshSpherePairHandler : StaticMeshPairHandler
{
	private StaticMeshSphereContactManifold contactManifold = new StaticMeshSphereContactManifold();

	protected override StaticMeshContactManifold MeshManifold => contactManifold;
}
