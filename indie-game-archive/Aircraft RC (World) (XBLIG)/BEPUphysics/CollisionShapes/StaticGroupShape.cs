using System.Collections.Generic;
using BEPUphysics.Collidables;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DataStructures;
using BEPUphysics.MathExtensions;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.CollisionShapes;

/// <summary>
///  The shape information used by a StaticGroup.
///  Unlike most shapes, a StaticGroupShape cannot be shared between multiple StaticGroups;
///  a StaticGroupShape is linked to a single StaticGroup.
/// </summary>
public class StaticGroupShape : CollisionShape
{
	/// <summary>
	/// Gets the StaticGroup associated with this StaticGroupShape.  Unlike most shapes, there is a one-to-one relationship
	/// between StaticGroupShapes and StaticGroups.
	/// </summary>
	public StaticGroup StaticGroup { get; private set; }

	/// <summary>
	/// Gets the bounding box tree associated with this shape.
	/// Contains Collidable instances as opposed to shapes.
	/// </summary>
	public BoundingBoxTree<Collidable> CollidableTree { get; private set; }

	/// <summary>
	///  Constructs a new StaticGroupShape.
	/// </summary>
	/// <param name="collidables">List of collidables in the StaticGroup.</param>
	/// <param name="owner">StaticGroup directly associated with this shape.</param>
	public StaticGroupShape(IList<Collidable> collidables, StaticGroup owner)
	{
		StaticGroup = owner;
		CollidableTree = new BoundingBoxTree<Collidable>(collidables);
	}

	/// <summary>
	/// Tests a ray against the collidable.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length, in units of the ray's direction's length, to test.</param>
	/// <param name="result">Hit data, if any.</param>
	/// <returns>Whether or not the ray hit the entry.</returns>
	public bool RayCast(Ray ray, float maximumLength, out RayCastResult result)
	{
		RawList<Collidable> collidableList = Resources.GetCollidableList();
		RawList<RayHit> rayHitList = Resources.GetRayHitList();
		CollidableTree.GetOverlaps(ray, maximumLength, collidableList);
		result = default(RayCastResult);
		result.HitData.T = float.MaxValue;
		for (int i = 0; i < collidableList.count; i++)
		{
			if (collidableList.Elements[i].RayCast(ray, maximumLength, out var rayHit) && rayHit.T < result.HitData.T)
			{
				result.HitData = rayHit;
				result.HitObject = collidableList.Elements[i];
			}
		}
		Resources.GiveBack(rayHitList);
		Resources.GiveBack(collidableList);
		return result.HitData.T < float.MaxValue;
	}

	/// <summary>
	/// Casts a convex shape against the collidable.
	/// </summary>
	/// <param name="castShape">Shape to cast.</param>
	/// <param name="startingTransform">Initial transform of the shape.</param>
	/// <param name="sweep">Sweep to apply to the shape.</param>
	/// <param name="result">Hit data, if any.</param>
	/// <returns>Whether or not the cast hit anything.</returns>
	public bool ConvexCast(ConvexShape castShape, ref RigidTransform startingTransform, ref Vector3 sweep, out RayCastResult result)
	{
		RawList<Collidable> collidableList = Resources.GetCollidableList();
		RawList<RayHit> rayHitList = Resources.GetRayHitList();
		Toolbox.GetExpandedBoundingBox(ref castShape, ref startingTransform, ref sweep, out var boundingBox);
		CollidableTree.GetOverlaps(boundingBox, collidableList);
		result = default(RayCastResult);
		result.HitData.T = float.MaxValue;
		for (int i = 0; i < collidableList.count; i++)
		{
			if (collidableList.Elements[i].ConvexCast(castShape, ref startingTransform, ref sweep, out var hit) && hit.T < result.HitData.T)
			{
				result.HitData = hit;
				result.HitObject = collidableList.Elements[i];
			}
		}
		Resources.GiveBack(rayHitList);
		Resources.GiveBack(collidableList);
		return result.HitData.T < float.MaxValue;
	}
}
