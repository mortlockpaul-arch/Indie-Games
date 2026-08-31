using System;
using System.Collections.Generic;
using BEPUphysics.Collidables.Events;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.MathExtensions;
using BEPUphysics.OtherSpaceStages;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Collidables;

/// <summary>
///  Collection of unmoving collidable objects.
/// </summary>
/// <remarks>
///  Batching multiple static objects together into a StaticGroup as opposed to adding them separately to the Space avoids BroadPhase pollution, improving performance.
///  </remarks>
public class StaticGroup : StaticCollidable
{
	protected internal ContactEventManager<StaticGroup> events;

	/// <summary>
	///  Gets the shape used by the mesh.  Unlike most collidable-shape pairs, StaticGroupShapes cannot be shared between multiple StaticGroups.
	/// </summary>
	public new StaticGroupShape Shape => (StaticGroupShape)shape;

	/// <summary>
	///  Gets the event manager used by the mesh.
	/// </summary>
	public ContactEventManager<StaticGroup> Events
	{
		get
		{
			return events;
		}
		set
		{
			if (value.Owner != null && value != events)
			{
				throw new Exception("Event manager is already owned by a mesh; event managers cannot be shared.");
			}
			if (events != null)
			{
				events.Owner = null;
			}
			events = value;
			if (events != null)
			{
				events.Owner = this;
			}
		}
	}

	protected internal override IContactEventTriggerer EventTriggerer => events;

	protected override IDeferredEventCreator EventCreator => events;

	/// <summary>
	///  Constructs a new static mesh.
	/// </summary>
	/// <param name="collidables">List of collidables in the static group.</param>
	public StaticGroup(IList<Collidable> collidables)
	{
		shape = new StaticGroupShape(collidables, this);
		Events = new ContactEventManager<StaticGroup>();
	}

	/// <summary>
	/// Updates the bounding box to the current state of the entry.
	/// </summary>
	public override void UpdateBoundingBox()
	{
		boundingBox = Shape.CollidableTree.BoundingBox;
	}

	/// <summary>
	/// Tests a ray against the entry.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length, in units of the ray's direction's length, to test.</param>
	/// <param name="rayHit">Hit location of the ray on the entry, if any.</param>
	/// <returns>Whether or not the ray hit the entry.</returns>
	public override bool RayCast(Ray ray, float maximumLength, out RayHit rayHit)
	{
		bool result = Shape.RayCast(ray, maximumLength, out var result2);
		rayHit = result2.HitData;
		return result;
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
		bool result = Shape.ConvexCast(castShape, ref startingTransform, ref sweep, out var result2);
		hit = result2.HitData;
		return result;
	}
}
