using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.ResourceManagement;

namespace BEPUphysics.CollisionTests.Manifolds;

public class TerrainConvexContactManifold : TerrainContactManifold
{
	private UnsafeResourcePool<TriangleConvexPairTester> testerPool = new UnsafeResourcePool<TriangleConvexPairTester>();

	protected override TrianglePairTester GetTester()
	{
		return testerPool.Take();
	}

	protected override void GiveBackTester(TrianglePairTester tester)
	{
		testerPool.GiveBack((TriangleConvexPairTester)tester);
	}
}
