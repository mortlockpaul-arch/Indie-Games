namespace BEPUphysics.CollisionShapes;

/// <summary>
///  Solidity of a triangle or mesh.
///  A triangle can be double sided, or allow one of its sides to let interacting objects through.
///  The entire mesh can be made solid, which means objects on the interior still generate contacts even if there aren't any triangles to hit.
///  Solidity requires the mesh to be closed.
/// </summary>
public enum MobileMeshSolidity
{
	/// <summary>
	/// The mesh will interact with objects coming from both directions.
	/// </summary>
	DoubleSided,
	/// <summary>
	/// The mesh will interact with objects from which the winding of the triangles appears to be clockwise.
	/// </summary>
	Clockwise,
	/// <summary>
	/// The mesh will interact with objects from which the winding of the triangles appears to be counterclockwise.
	/// </summary>
	Counterclockwise,
	/// <summary>
	/// The mesh will treat objects inside of its concave shell as if the mesh had volume.  Mesh must be closed for this to work properly.
	/// </summary>
	Solid
}
