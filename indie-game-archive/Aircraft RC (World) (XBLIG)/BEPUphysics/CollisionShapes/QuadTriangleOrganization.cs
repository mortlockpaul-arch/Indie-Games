namespace BEPUphysics.CollisionShapes;

/// <summary>
/// Defines how a Terrain organizes triangles in its quads.
/// </summary>
public enum QuadTriangleOrganization
{
	/// <summary>
	/// Triangle with a right angle at the (-i,-j) position and another at the (+i,+j) position.
	/// </summary>
	BottomLeftUpperRight,
	/// <summary>
	/// Triangle with a right angle at the (+i,-j) position and another at the high (-i,+j) position.
	/// </summary>
	BottomRightUpperLeft
}
