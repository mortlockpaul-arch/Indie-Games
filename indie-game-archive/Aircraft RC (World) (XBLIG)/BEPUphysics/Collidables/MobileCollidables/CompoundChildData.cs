using BEPUphysics.Collidables.Events;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes;
using BEPUphysics.Materials;

namespace BEPUphysics.Collidables.MobileCollidables;

/// <summary>
///  Data which can be used to create a CompoundChild.
///  This data is not itself a child yet; another system
///  will use it as input to construct the children.
/// </summary>
public struct CompoundChildData
{
	/// <summary>
	///  Shape entry of the compound child.
	/// </summary>
	public CompoundShapeEntry Entry;

	/// <summary>
	///  Event manager for the new child.
	/// </summary>
	public ContactEventManager<EntityCollidable> Events;

	/// <summary>
	///  Collision rules for the new child.
	/// </summary>
	public CollisionRules CollisionRules;

	/// <summary>
	///  Material for the new child.
	/// </summary>
	public Material Material;

	/// <summary>
	/// Tag to assign to the collidable created for this child.
	/// </summary>
	public object Tag;
}
