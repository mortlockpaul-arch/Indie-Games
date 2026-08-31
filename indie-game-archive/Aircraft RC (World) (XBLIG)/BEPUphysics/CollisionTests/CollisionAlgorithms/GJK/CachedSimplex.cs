namespace BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;

/// <summary>
///  Stored simplex used to warmstart closest point GJK runs.
/// </summary>
public struct CachedSimplex
{
	/// <summary>
	///  Simplex in the local space of shape A.
	/// </summary>
	public ContributingShapeSimplex LocalSimplexA;

	/// <summary>
	///  Simplex in the local space of shape B.
	/// </summary>
	public ContributingShapeSimplex LocalSimplexB;

	/// <summary>
	/// State of the simplex at the termination of the last GJK run.
	/// </summary>
	public SimplexState State;
}
