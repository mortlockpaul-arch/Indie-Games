namespace BEPUphysics.CollisionTests.CollisionAlgorithms.GJK;

/// <summary>
///  Defines the state of a simplex.
/// </summary>
public enum SimplexState : byte
{
	Empty,
	Point,
	Segment,
	Triangle,
	Tetrahedron
}
