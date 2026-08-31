using System;
using BEPUphysics.DataStructures;

namespace BEPUphysics.Collidables.MobileCollidables;

/// <summary>
///  Hierarchy of children used to accelerate queries and tests for compound collidables.
/// </summary>
public class CompoundHierarchy
{
	private BoundingBoxTree<CompoundChild> tree;

	private CompoundCollidable owner;

	/// <summary>
	///  Gets the bounding box tree of the hierarchy.
	/// </summary>
	public BoundingBoxTree<CompoundChild> Tree => tree;

	/// <summary>
	///  Gets the CompoundCollidable that owns this hierarchy.
	/// </summary>
	public CompoundCollidable Owner => owner;

	/// <summary>
	///  Constructs a new compound hierarchy.
	/// </summary>
	/// <param name="owner">Owner of the hierarchy.</param>
	public CompoundHierarchy(CompoundCollidable owner)
	{
		this.owner = owner;
		CompoundChild[] array = new CompoundChild[owner.children.count];
		Array.Copy(owner.children.Elements, array, owner.children.count);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].CollisionInformation.worldTransform = owner.Shape.shapes.Elements[i].LocalTransform;
			array[i].CollisionInformation.UpdateBoundingBoxInternal(0f);
		}
		tree = new BoundingBoxTree<CompoundChild>(array);
	}
}
