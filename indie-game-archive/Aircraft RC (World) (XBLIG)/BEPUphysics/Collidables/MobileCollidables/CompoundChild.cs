using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.CollisionShapes;
using BEPUphysics.Materials;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Collidables.MobileCollidables;

/// <summary>
///  A collidable child of a compound.
/// </summary>
public class CompoundChild : IBoundingBoxOwner
{
	private CompoundShape shape;

	internal int shapeIndex;

	private EntityCollidable collisionInformation;

	/// <summary>
	/// Gets the index of the shape used by this child in the CompoundShape's shapes list.
	/// </summary>
	public int ShapeIndex => shapeIndex;

	/// <summary>
	///  Gets the Collidable associated with the child.
	/// </summary>
	public EntityCollidable CollisionInformation => collisionInformation;

	/// <summary>
	///  Gets or sets the material associated with the child.
	/// </summary>
	public Material Material { get; set; }

	/// <summary>
	/// Gets the index of the shape associated with this child in the CompoundShape's shapes list.
	/// </summary>
	public CompoundShapeEntry Entry => shape.shapes.Elements[shapeIndex];

	/// <summary>
	/// Gets the bounding box of the child.
	/// </summary>
	public BoundingBox BoundingBox => collisionInformation.boundingBox;

	internal CompoundChild(CompoundShape shape, EntityCollidable collisionInformation, Material material, int index)
	{
		this.shape = shape;
		this.collisionInformation = collisionInformation;
		Material = material;
		shapeIndex = index;
	}

	internal CompoundChild(CompoundShape shape, EntityCollidable collisionInformation, int index)
	{
		this.shape = shape;
		this.collisionInformation = collisionInformation;
		shapeIndex = index;
	}
}
