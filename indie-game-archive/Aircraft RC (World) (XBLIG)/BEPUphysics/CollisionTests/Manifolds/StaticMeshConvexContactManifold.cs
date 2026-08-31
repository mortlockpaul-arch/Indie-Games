using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.ResourceManagement;

namespace BEPUphysics.CollisionTests.Manifolds;

/// <summary>
///  Manages persistent contacts between a static mesh and a convex.
/// </summary>
public class StaticMeshConvexContactManifold : StaticMeshContactManifold
{
	private UnsafeResourcePool<TriangleConvexPairTester> testerPool = new UnsafeResourcePool<TriangleConvexPairTester>();

	protected override void GiveBackTester(TrianglePairTester tester)
	{
		testerPool.GiveBack((TriangleConvexPairTester)tester);
	}

	protected override TrianglePairTester GetTester()
	{
		return testerPool.Take();
	}
}
