using BEPUphysics.CollisionTests.Manifolds;

namespace BEPUphysics.NarrowPhaseSystems.Pairs;

/// <summary>
///  Handles a mobile mesh-convex collision pair.
/// </summary>
public class MobileMeshTrianglePairHandler : MobileMeshPairHandler
{
	private MobileMeshTriangleContactManifold contactManifold = new MobileMeshTriangleContactManifold();

	protected internal override MobileMeshContactManifold MeshManifold => contactManifold;
}
