using System.Collections.Generic;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.CollisionShapes;
using BEPUphysics.DataStructures;

namespace BEPUphysics.Entities.Prefabs;

/// <summary>
/// Acts as a grouping of multiple other objects.  Can be used to form physically simulated concave shapes.
/// </summary>
public class CompoundBody : Entity<CompoundCollidable>
{
	/// <summary>
	///  Gets the list of shapes in the compound.
	/// </summary>
	public ReadOnlyList<CompoundShapeEntry> Shapes => base.CollisionInformation.Shape.Shapes;

	/// <summary>
	/// Creates a new kinematic CompoundBody with the given subbodies.
	/// </summary>
	/// <param name="bodies">List of entities to use as subbodies of the compound body.</param>
	/// <exception cref="T:System.InvalidOperationException">Thrown when the bodies list is empty or there is a mix of kinematic and dynamic entities in the body list.</exception>
	public CompoundBody(IList<CompoundShapeEntry> bodies)
	{
		CompoundShape compoundShape = new CompoundShape(bodies, out var center);
		Initialize(new CompoundCollidable(compoundShape));
		base.Position = center;
	}

	/// <summary>
	/// Creates a new dynamic CompoundBody with the given subbodies.
	/// </summary>
	/// <param name="bodies">List of entities to use as subbodies of the compound body.</param>
	/// <param name="mass">Mass of the compound.</param>
	/// <exception cref="T:System.InvalidOperationException">Thrown when the bodies list is empty or there is a mix of kinematic and dynamic entities in the body list.</exception>
	public CompoundBody(IList<CompoundShapeEntry> bodies, float mass)
	{
		CompoundShape compoundShape = new CompoundShape(bodies, out var center);
		Initialize(new CompoundCollidable(compoundShape), mass);
		base.Position = center;
	}

	/// <summary>
	///  Constructs a kinematic compound body from the children data.
	/// </summary>
	/// <param name="children">Children data to construct the compound from.</param>
	public CompoundBody(IList<CompoundChildData> children)
	{
		CompoundCollidable compoundCollidable = new CompoundCollidable(children, out var center);
		Initialize(compoundCollidable);
		base.Position = center;
	}

	/// <summary>
	///  Constructs a dynamic compound body from the children data.
	/// </summary>
	/// <param name="children">Children data to construct the compound from.</param>
	/// <param name="mass">Mass of the compound body.</param>
	public CompoundBody(IList<CompoundChildData> children, float mass)
	{
		CompoundCollidable compoundCollidable = new CompoundCollidable(children, out var center);
		Initialize(compoundCollidable, mass);
		base.Position = center;
	}
}
