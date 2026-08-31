using System;
using System.Collections.Generic;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Collidables.Events;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.DataStructures;
using BEPUphysics.Materials;
using BEPUphysics.MathExtensions;
using BEPUphysics.ResourceManagement;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Collidables.MobileCollidables;

/// <summary>
///  Collidable used by compound shapes.
/// </summary>
public class CompoundCollidable : EntityCollidable
{
	internal RawList<CompoundChild> children = new RawList<CompoundChild>();

	internal CompoundHierarchy hierarchy;

	/// <summary>
	/// Gets or sets the event manager for the collidable.
	/// Compound collidables must use a special CompoundEventManager in order for the deferred events created
	/// by child collidables to be dispatched.
	/// If this method is bypassed and a different event manager is used, this method will return null and 
	/// deferred events from children will fail.
	/// </summary>
	public new CompoundEventManager Events
	{
		get
		{
			return events as CompoundEventManager;
		}
		set
		{
			_ = events;
			foreach (CompoundChild child in children)
			{
				child.CollisionInformation.events.Parent = value;
			}
			base.Events = value;
		}
	}

	/// <summary>
	///  Gets the shape of the collidable.
	/// </summary>
	public new CompoundShape Shape
	{
		get
		{
			return (CompoundShape)shape;
		}
		protected internal set
		{
			base.Shape = value;
		}
	}

	/// <summary>
	///  Gets a list of the children in the collidable.
	/// </summary>
	public ReadOnlyList<CompoundChild> Children => new ReadOnlyList<CompoundChild>(children);

	/// <summary>
	///  Gets the hierarchy of children used by the collidable.
	/// </summary>
	public CompoundHierarchy Hierarchy => hierarchy;

	protected override void OnEntityChanged()
	{
		for (int i = 0; i < children.count; i++)
		{
			children.Elements[i].CollisionInformation.Entity = entity;
			if (children.Elements[i].Material == null)
			{
				children.Elements[i].Material = entity.material;
			}
		}
		base.OnEntityChanged();
	}

	private CompoundChild GetChild(CompoundChildData data, int index)
	{
		EntityCollidable collidableInstance = data.Entry.Shape.GetCollidableInstance();
		if (data.Events != null)
		{
			collidableInstance.Events = data.Events;
		}
		collidableInstance.events.Parent = Events;
		if (data.CollisionRules != null)
		{
			collidableInstance.CollisionRules = data.CollisionRules;
		}
		collidableInstance.Tag = data.Tag;
		if (data.Material == null)
		{
			data.Material = new Material();
		}
		return new CompoundChild(Shape, collidableInstance, data.Material, index);
	}

	private CompoundChild GetChild(CompoundShapeEntry entry, int index)
	{
		EntityCollidable collidableInstance = entry.Shape.GetCollidableInstance();
		collidableInstance.events.Parent = Events;
		return new CompoundChild(Shape, collidableInstance, index);
	}

	internal CompoundCollidable()
	{
		Events = new CompoundEventManager();
		hierarchy = new CompoundHierarchy(this);
	}

	/// <summary>
	///  Constructs a compound collidable using additional information about the shapes in the compound.
	/// </summary>
	/// <param name="children">Data representing the children of the compound collidable.</param>
	public CompoundCollidable(IList<CompoundChildData> children)
	{
		Events = new CompoundEventManager();
		RawList<CompoundShapeEntry> rawList = new RawList<CompoundShapeEntry>();
		for (int i = 0; i < children.Count; i++)
		{
			rawList.Add(children[i].Entry);
		}
		base.Shape = new CompoundShape(rawList);
		for (int j = 0; j < children.Count; j++)
		{
			this.children.Add(GetChild(children[j], j));
		}
		hierarchy = new CompoundHierarchy(this);
	}

	/// <summary>
	///  Constructs a compound collidable using additional information about the shapes in the compound.
	/// </summary>
	/// <param name="children">Data representing the children of the compound collidable.</param>
	/// <param name="center">Location computed to be the center of the compound object.</param>
	public CompoundCollidable(IList<CompoundChildData> children, out Vector3 center)
	{
		Events = new CompoundEventManager();
		RawList<CompoundShapeEntry> rawList = new RawList<CompoundShapeEntry>();
		for (int i = 0; i < children.Count; i++)
		{
			rawList.Add(children[i].Entry);
		}
		base.Shape = new CompoundShape(rawList, out center);
		for (int j = 0; j < children.Count; j++)
		{
			this.children.Add(GetChild(children[j], j));
		}
		hierarchy = new CompoundHierarchy(this);
	}

	/// <summary>
	///  Constructs a new CompoundCollidable.
	/// </summary>
	/// <param name="compoundShape">Compound shape to use for the collidable.</param>
	public CompoundCollidable(CompoundShape compoundShape)
		: base(compoundShape)
	{
		Events = new CompoundEventManager();
		for (int i = 0; i < compoundShape.shapes.count; i++)
		{
			CompoundChild child = GetChild(compoundShape.shapes.Elements[i], i);
			children.Add(child);
		}
		hierarchy = new CompoundHierarchy(this);
	}

	/// <summary>
	///  Updates the world transform of the collidable.
	/// </summary>
	/// <param name="position">Position to use for the calculation.</param>
	/// <param name="orientation">Orientation to use for the calculation.</param>
	public override void UpdateWorldTransform(ref Vector3 position, ref Quaternion orientation)
	{
		base.UpdateWorldTransform(ref position, ref orientation);
		RawList<CompoundShapeEntry> shapes = Shape.shapes;
		for (int i = 0; i < children.count; i++)
		{
			RigidTransform.Transform(ref shapes.Elements[children.Elements[i].shapeIndex].LocalTransform, ref worldTransform, out var combined);
			children.Elements[i].CollisionInformation.UpdateWorldTransform(ref combined.Position, ref combined.Orientation);
		}
	}

	protected internal override void UpdateBoundingBoxInternal(float dt)
	{
		for (int i = 0; i < children.count; i++)
		{
			children.Elements[i].CollisionInformation.UpdateBoundingBoxInternal(dt);
		}
		hierarchy.Tree.Refit();
		boundingBox = hierarchy.Tree.BoundingBox;
	}

	/// <summary>
	/// Tests a ray against the collidable.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length, in units of the ray's direction's length, to test.</param>
	/// <param name="rayHit">Hit location of the ray on the collidable, if any.</param>
	/// <returns>Whether or not the ray hit the collidable.</returns>
	public override bool RayCast(Ray ray, float maximumLength, out RayHit rayHit)
	{
		rayHit = default(RayHit);
		RawList<CompoundChild> compoundChildList = Resources.GetCompoundChildList();
		if (hierarchy.Tree.GetOverlaps(ray, maximumLength, compoundChildList))
		{
			rayHit.T = float.MaxValue;
			for (int i = 0; i < compoundChildList.count; i++)
			{
				if (compoundChildList.Elements[i].CollisionInformation.RayCast(ray, maximumLength, out var rayHit2) && rayHit2.T < rayHit.T)
				{
					rayHit = rayHit2;
				}
			}
			Resources.GiveBack(compoundChildList);
			return rayHit.T != float.MaxValue;
		}
		Resources.GiveBack(compoundChildList);
		return false;
	}

	/// <summary>
	/// Tests a ray against the collidable.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length, in units of the ray's direction's length, to test.</param>
	/// <param name="filter">Can be used to filter sets of objects out of the raycasting.</param>
	/// <param name="rayHit">Hit location of the ray on the collidable, if any.</param>
	/// <returns>Whether or not the ray hit the collidable.</returns>
	public override bool RayCast(Ray ray, float maximumLength, Func<BroadPhaseEntry, bool> filter, out RayHit rayHit)
	{
		rayHit = default(RayHit);
		if (filter(this))
		{
			RawList<CompoundChild> compoundChildList = Resources.GetCompoundChildList();
			if (hierarchy.Tree.GetOverlaps(ray, maximumLength, compoundChildList))
			{
				rayHit.T = float.MaxValue;
				for (int i = 0; i < compoundChildList.count; i++)
				{
					if (compoundChildList.Elements[i].CollisionInformation.RayCast(ray, maximumLength, filter, out var rayHit2) && rayHit2.T < rayHit.T)
					{
						rayHit = rayHit2;
					}
				}
				Resources.GiveBack(compoundChildList);
				return rayHit.T != float.MaxValue;
			}
			Resources.GiveBack(compoundChildList);
		}
		return false;
	}

	/// <summary>
	/// Tests a ray against the compound.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length, in units of the ray's direction's length, to test.</param>
	/// <param name="rayHit">Hit data and the hit child collidable, if any.</param>
	/// <returns>Whether or not the ray hit the entry.</returns>
	public bool RayCast(Ray ray, float maximumLength, out RayCastResult rayHit)
	{
		rayHit = default(RayCastResult);
		RawList<CompoundChild> compoundChildList = Resources.GetCompoundChildList();
		if (hierarchy.Tree.GetOverlaps(ray, maximumLength, compoundChildList))
		{
			rayHit.HitData.T = float.MaxValue;
			for (int i = 0; i < compoundChildList.count; i++)
			{
				EntityCollidable collisionInformation = compoundChildList.Elements[i].CollisionInformation;
				if (collisionInformation.RayCast(ray, maximumLength, out var rayHit2) && rayHit2.T < rayHit.HitData.T)
				{
					rayHit.HitData = rayHit2;
					rayHit.HitObject = collisionInformation;
				}
			}
			Resources.GiveBack(compoundChildList);
			return rayHit.HitData.T != float.MaxValue;
		}
		Resources.GiveBack(compoundChildList);
		return false;
	}

	/// <summary>
	/// Tests a ray against the collidable.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length, in units of the ray's direction's length, to test.</param>
	/// <param name="rayHit">Hit data, if any.</param>
	/// <param name="hitChild">Child collidable hit by the ray, if any.</param>
	/// <returns>Whether or not the ray hit the entry.</returns>
	public bool RayCast(Ray ray, float maximumLength, out RayHit rayHit, out CompoundChild hitChild)
	{
		rayHit = default(RayHit);
		hitChild = null;
		RawList<CompoundChild> compoundChildList = Resources.GetCompoundChildList();
		if (hierarchy.Tree.GetOverlaps(ray, maximumLength, compoundChildList))
		{
			rayHit.T = float.MaxValue;
			for (int i = 0; i < compoundChildList.count; i++)
			{
				EntityCollidable collisionInformation = compoundChildList.Elements[i].CollisionInformation;
				if (collisionInformation.RayCast(ray, maximumLength, out var rayHit2) && rayHit2.T < rayHit.T)
				{
					rayHit = rayHit2;
					hitChild = compoundChildList.Elements[i];
				}
			}
			Resources.GiveBack(compoundChildList);
			return rayHit.T != float.MaxValue;
		}
		Resources.GiveBack(compoundChildList);
		return false;
	}

	/// <summary>
	/// Casts a convex shape against the collidable.
	/// </summary>
	/// <param name="castShape">Shape to cast.</param>
	/// <param name="startingTransform">Initial transform of the shape.</param>
	/// <param name="sweep">Sweep to apply to the shape.</param>
	/// <param name="hit">Hit data, if any.</param>
	/// <returns>Whether or not the cast hit anything.</returns>
	public override bool ConvexCast(ConvexShape castShape, ref RigidTransform startingTransform, ref Vector3 sweep, out RayHit hit)
	{
		hit = default(RayHit);
		Toolbox.GetExpandedBoundingBox(ref castShape, ref startingTransform, ref sweep, out var boundingBox);
		RawList<CompoundChild> compoundChildList = Resources.GetCompoundChildList();
		if (hierarchy.Tree.GetOverlaps(boundingBox, compoundChildList))
		{
			hit.T = float.MaxValue;
			for (int i = 0; i < compoundChildList.count; i++)
			{
				EntityCollidable collisionInformation = compoundChildList.Elements[i].CollisionInformation;
				if (collisionInformation.ConvexCast(castShape, ref startingTransform, ref sweep, out var hit2) && hit2.T < hit.T)
				{
					hit = hit2;
				}
			}
			Resources.GiveBack(compoundChildList);
			return hit.T != float.MaxValue;
		}
		Resources.GiveBack(compoundChildList);
		return false;
	}
}
