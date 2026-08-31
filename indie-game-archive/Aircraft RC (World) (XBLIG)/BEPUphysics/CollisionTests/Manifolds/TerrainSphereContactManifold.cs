using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.ResourceManagement;

namespace BEPUphysics.CollisionTests.Manifolds;

public class TerrainSphereContactManifold : TerrainContactManifold
{
	private UnsafeResourcePool<TriangleSpherePairTester> testerPool = new UnsafeResourcePool<TriangleSpherePairTester>();

	protected override TrianglePairTester GetTester()
	{
		return testerPool.Take();
	}

	protected override void GiveBackTester(TrianglePairTester tester)
	{
		testerPool.GiveBack((TriangleSpherePairTester)tester);
	}
}
