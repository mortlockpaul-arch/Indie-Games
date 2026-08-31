using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Entities.Prefabs;

/// <summary>
/// Acts as a grouping of multiple other objects.  Can be used to form physically simulated concave shapes.
/// </summary>
public class MobileMesh : Entity<MobileMeshCollidable>
{
	/// <summary>
	/// Creates a new kinematic MobileMesh.
	/// </summary>
	/// <param name="vertices">Vertices in the mesh.</param>
	/// <param name="indices">Indices of the mesh.</param>
	/// <param name="localTransform">Affine transform to apply to the vertices.</param>
	/// <param name="solidity">Solidity/sidedness of the mesh.  "Solid" is only permitted if the mesh is closed.</param>
	public MobileMesh(Vector3[] vertices, int[] indices, AffineTransform localTransform, MobileMeshSolidity solidity)
	{
		MobileMeshShape shape = new MobileMeshShape(vertices, indices, localTransform, solidity, out var distributionInfo);
		Initialize(new MobileMeshCollidable(shape));
		base.Position = distributionInfo.Center;
	}

	/// <summary>
	/// Creates a new dynamic MobileMesh.
	/// </summary>
	/// <param name="vertices">Vertices in the mesh.</param>
	/// <param name="indices">Indices of the mesh.</param>
	/// <param name="localTransform">Affine transform to apply to the vertices.</param>
	/// <param name="solidity">Solidity/sidedness of the mesh.  "Solid" is only permitted if the mesh is closed.</param>
	/// <param name="mass">Mass of the mesh.</param>
	public MobileMesh(Vector3[] vertices, int[] indices, AffineTransform localTransform, MobileMeshSolidity solidity, float mass)
	{
		MobileMeshShape shape = new MobileMeshShape(vertices, indices, localTransform, solidity, out var distributionInfo);
		Matrix3X3.Multiply(ref distributionInfo.VolumeDistribution, mass * InertiaHelper.InertiaTensorScale, out var result);
		Initialize(new MobileMeshCollidable(shape), mass, result, distributionInfo.Volume);
		base.Position = distributionInfo.Center;
	}
}
