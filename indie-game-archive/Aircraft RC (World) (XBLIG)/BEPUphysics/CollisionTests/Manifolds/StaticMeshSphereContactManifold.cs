using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.ResourceManagement;

namespace BEPUphysics.CollisionTests.Manifolds;

/// <summary>
///  Manages persistent contacts between a static mesh and a convex.
/// </summary>
public class StaticMeshSphereContactManifold : StaticMeshContactManifold
{
	private UnsafeResourcePool<TriangleSpherePairTester> testerPool = new UnsafeResourcePool<TriangleSpherePairTester>();

	protected override void GiveBackTester(TrianglePairTester tester)
	{
		testerPool.GiveBack((TriangleSpherePairTester)tester);
	}

	protected override TrianglePairTester GetTester()
	{
		return testerPool.Take();
	}
}
