using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionTests.Manifolds;
using BEPUphysics.DataStructures;
using BEPUphysics.DeactivationManagement;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.ResourceManagement;

/// <summary>
/// Handles allocation and management of commonly used resources.
/// </summary>
public static class Resources
{
	private static ResourcePool<RawList<RayHit>> SubPoolRayHitList;

	private static ResourcePool<RawList<RayCastResult>> SubPoolRayCastResultList;

	private static ResourcePool<RawList<BroadPhaseEntry>> SubPoolBroadPhaseEntryList;

	private static ResourcePool<RawList<Collidable>> SubPoolCollidableList;

	private static ResourcePool<RawList<int>> SubPoolIntList;

	private static ResourcePool<HashSet<int>> SubPoolIntSet;

	private static ResourcePool<RawList<float>> SubPoolFloatList;

	private static ResourcePool<RawList<Vector3>> SubPoolVectorList;

	private static ResourcePool<RawList<Entity>> SubPoolEntityRawList;

	private static ResourcePool<TriangleShape> SubPoolTriangleShape;

	private static ResourcePool<RawList<CompoundChild>> SubPoolCompoundChildList;

	private static ResourcePool<TriangleCollidable> SubPoolTriangleCollidables;

	private static ResourcePool<RawList<TriangleMeshConvexContactManifold.TriangleIndices>> SubPoolTriangleIndicesList;

	private static ResourcePool<SimulationIslandConnection> SimulationIslandConnections;

	static Resources()
	{
		ResetPools();
	}

	public static void ResetPools()
	{
		SubPoolRayHitList = new LockingResourcePool<RawList<RayHit>>();
		SubPoolRayCastResultList = new LockingResourcePool<RawList<RayCastResult>>();
		SubPoolBroadPhaseEntryList = new LockingResourcePool<RawList<BroadPhaseEntry>>();
		SubPoolCollidableList = new LockingResourcePool<RawList<Collidable>>();
		SubPoolCompoundChildList = new LockingResourcePool<RawList<CompoundChild>>();
		SubPoolIntList = new LockingResourcePool<RawList<int>>();
		SubPoolIntSet = new LockingResourcePool<HashSet<int>>();
		SubPoolFloatList = new LockingResourcePool<RawList<float>>();
		SubPoolVectorList = new LockingResourcePool<RawList<Vector3>>();
		SubPoolEntityRawList = new LockingResourcePool<RawList<Entity>>(16);
		SubPoolTriangleShape = new LockingResourcePool<TriangleShape>();
		SubPoolTriangleCollidables = new LockingResourcePool<TriangleCollidable>();
		SubPoolTriangleIndicesList = new LockingResourcePool<RawList<TriangleMeshConvexContactManifold.TriangleIndices>>();
		SimulationIslandConnections = new LockingResourcePool<SimulationIslandConnection>();
	}

	/// <summary>
	/// Retrieves a ray cast result list from the resource pool.
	/// </summary>
	/// <returns>Empty ray cast result list.</returns>
	public static RawList<RayCastResult> GetRayCastResultList()
	{
		return SubPoolRayCastResultList.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="list">List to return.</param>
	public static void GiveBack(RawList<RayCastResult> list)
	{
		list.Clear();
		SubPoolRayCastResultList.GiveBack(list);
	}

	/// <summary>
	/// Retrieves a ray hit list from the resource pool.
	/// </summary>
	/// <returns>Empty ray hit list.</returns>
	public static RawList<RayHit> GetRayHitList()
	{
		return SubPoolRayHitList.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="list">List to return.</param>
	public static void GiveBack(RawList<RayHit> list)
	{
		list.Clear();
		SubPoolRayHitList.GiveBack(list);
	}

	/// <summary>
	/// Retrieves an BroadPhaseEntry list from the resource pool.
	/// </summary>
	/// <returns>Empty BroadPhaseEntry list.</returns>
	public static RawList<BroadPhaseEntry> GetBroadPhaseEntryList()
	{
		return SubPoolBroadPhaseEntryList.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="list">List to return.</param>
	public static void GiveBack(RawList<BroadPhaseEntry> list)
	{
		list.Clear();
		SubPoolBroadPhaseEntryList.GiveBack(list);
	}

	/// <summary>
	/// Retrieves a Collidable list from the resource pool.
	/// </summary>
	/// <returns>Empty Collidable list.</returns>
	public static RawList<Collidable> GetCollidableList()
	{
		return SubPoolCollidableList.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="list">List to return.</param>
	public static void GiveBack(RawList<Collidable> list)
	{
		list.Clear();
		SubPoolCollidableList.GiveBack(list);
	}

	/// <summary>
	/// Retrieves an CompoundChild list from the resource pool.
	/// </summary>
	/// <returns>Empty information list.</returns>
	public static RawList<CompoundChild> GetCompoundChildList()
	{
		return SubPoolCompoundChildList.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="list">List to return.</param>
	public static void GiveBack(RawList<CompoundChild> list)
	{
		list.Clear();
		SubPoolCompoundChildList.GiveBack(list);
	}

	/// <summary>
	/// Retrieves a int list from the resource pool.
	/// </summary>
	/// <returns>Empty int list.</returns>
	public static RawList<int> GetIntList()
	{
		return SubPoolIntList.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="list">List to return.</param>
	public static void GiveBack(RawList<int> list)
	{
		list.Clear();
		SubPoolIntList.GiveBack(list);
	}

	/// <summary>
	/// Retrieves a int hash set from the resource pool.
	/// </summary>
	/// <returns>Empty int set.</returns>
	public static HashSet<int> GetIntSet()
	{
		return SubPoolIntSet.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="set">Set to return.</param>
	public static void GiveBack(HashSet<int> set)
	{
		set.Clear();
		SubPoolIntSet.GiveBack(set);
	}

	/// <summary>
	/// Retrieves a float list from the resource pool.
	/// </summary>
	/// <returns>Empty float list.</returns>
	public static RawList<float> GetFloatList()
	{
		return SubPoolFloatList.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="list">List to return.</param>
	public static void GiveBack(RawList<float> list)
	{
		list.Clear();
		SubPoolFloatList.GiveBack(list);
	}

	/// <summary>
	/// Retrieves a Vector3 list from the resource pool.
	/// </summary>
	/// <returns>Empty Vector3 list.</returns>
	public static RawList<Vector3> GetVectorList()
	{
		return SubPoolVectorList.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="list">List to return.</param>
	public static void GiveBack(RawList<Vector3> list)
	{
		list.Clear();
		SubPoolVectorList.GiveBack(list);
	}

	/// <summary>
	/// Retrieves an Entity RawList from the resource pool.
	/// </summary>
	/// <returns>Empty Entity raw list.</returns>
	public static RawList<Entity> GetEntityRawList()
	{
		return SubPoolEntityRawList.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="list">List to return.</param>
	public static void GiveBack(RawList<Entity> list)
	{
		list.Clear();
		SubPoolEntityRawList.GiveBack(list);
	}

	/// <summary>
	/// Retrieves a Triangle shape from the resource pool.
	/// </summary>
	/// <param name="v1">Position of the first vertex.</param>
	/// <param name="v2">Position of the second vertex.</param>
	/// <param name="v3">Position of the third vertex.</param>
	/// <returns>Initialized TriangleShape.</returns>
	public static TriangleShape GetTriangle(ref Vector3 v1, ref Vector3 v2, ref Vector3 v3)
	{
		TriangleShape triangleShape = SubPoolTriangleShape.Take();
		triangleShape.vA = v1;
		triangleShape.vB = v2;
		triangleShape.vC = v3;
		return triangleShape;
	}

	/// <summary>
	/// Retrieves a Triangle shape from the resource pool.
	/// </summary>
	/// <returns>Initialized TriangleShape.</returns>
	public static TriangleShape GetTriangle()
	{
		return SubPoolTriangleShape.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="triangle">Triangle to return.</param>
	public static void GiveBack(TriangleShape triangle)
	{
		triangle.collisionMargin = 0f;
		triangle.sidedness = TriangleSidedness.DoubleSided;
		SubPoolTriangleShape.GiveBack(triangle);
	}

	/// <summary>
	/// Retrieves a TriangleCollidable from the resource pool.
	/// </summary>
	/// <param name="a">First vertex in the triangle.</param>
	/// <param name="b">Second vertex in the triangle.</param>
	/// <param name="c">Third vertex in the triangle.</param>
	/// <returns>Initialized TriangleCollidable.</returns>
	public static TriangleCollidable GetTriangleCollidable(ref Vector3 a, ref Vector3 b, ref Vector3 c)
	{
		TriangleCollidable triangleCollidable = SubPoolTriangleCollidables.Take();
		TriangleShape shape = triangleCollidable.Shape;
		shape.vA = a;
		shape.vB = b;
		shape.vC = c;
		RigidTransform transform = RigidTransform.Identity;
		triangleCollidable.UpdateBoundingBoxForTransform(ref transform);
		return triangleCollidable;
	}

	/// <summary>
	/// Retrieves a TriangleCollidable from the resource pool.
	/// </summary>
	/// <returns>Initialized TriangleCollidable.</returns>
	public static TriangleCollidable GetTriangleCollidable()
	{
		return SubPoolTriangleCollidables.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="triangle">Triangle collidable to return.</param>
	public static void GiveBack(TriangleCollidable triangle)
	{
		triangle.CleanUp();
		SubPoolTriangleCollidables.GiveBack(triangle);
	}

	/// <summary>
	/// Retrieves a TriangleIndices list from the resource pool.
	/// </summary>
	/// <returns>TriangleIndices list.</returns>
	public static RawList<TriangleMeshConvexContactManifold.TriangleIndices> GetTriangleIndicesList()
	{
		return SubPoolTriangleIndicesList.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="triangleIndices">TriangleIndices list to return.</param>
	public static void GiveBack(RawList<TriangleMeshConvexContactManifold.TriangleIndices> triangleIndices)
	{
		triangleIndices.Clear();
		SubPoolTriangleIndicesList.GiveBack(triangleIndices);
	}

	/// <summary>
	/// Retrieves a simulation island connection from the resource pool.
	/// </summary>
	/// <returns>Uninitialized simulation island connection.</returns>
	public static SimulationIslandConnection GetSimulationIslandConnection()
	{
		return SimulationIslandConnections.Take();
	}

	/// <summary>
	/// Returns a resource to the pool.
	/// </summary>
	/// <param name="connection">Connection to return.</param>
	public static void GiveBack(SimulationIslandConnection connection)
	{
		connection.CleanUp();
		SimulationIslandConnections.GiveBack(connection);
	}
}
