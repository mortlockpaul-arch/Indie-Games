using BEPUphysics.CollisionTests.CollisionAlgorithms;
using BEPUphysics.ResourceManagement;

namespace BEPUphysics.CollisionTests.Manifolds;

/// <summary>
///  Manages persistent contacts between a convex and an instanced mesh.
/// </summary>
public class InstancedMeshConvexContactManifold : InstancedMeshContactManifold
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
