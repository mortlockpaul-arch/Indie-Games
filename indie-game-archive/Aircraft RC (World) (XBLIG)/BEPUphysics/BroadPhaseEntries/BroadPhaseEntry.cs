using System;
using BEPUphysics.BroadPhaseSystems;
using BEPUphysics.CollisionRuleManagement;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.BroadPhaseEntries;

/// <summary>
/// Superclass of all objects which live inside the broad phase.
/// The BroadPhase will generate pairs between BroadPhaseEntries.
/// </summary>
public abstract class BroadPhaseEntry : IBoundingBoxOwner, ICollisionRulesOwner
{
	internal int hashCode;

	private Action collisionRulesUpdatedDelegate;

	protected internal BoundingBox boundingBox;

	internal CollisionRules collisionRules;

	/// <summary>
	/// Gets the broad phase to which this broad phase entry belongs.
	/// </summary>
	public BroadPhase BroadPhase { get; internal set; }

	/// <summary>
	/// Gets or sets the bounding box of the entry.
	/// </summary>
	public BoundingBox BoundingBox
	{
		get
		{
			return boundingBox;
		}
		set
		{
			boundingBox = value;
		}
	}

	protected internal abstract bool IsActive { get; }

	/// <summary>
	/// Gets the entry's collision rules.
	/// </summary>
	public CollisionRules CollisionRules
	{
		get
		{
			return collisionRules;
		}
		set
		{
			if (collisionRules != value)
			{
				if (collisionRules != null)
				{
					collisionRules.CollisionRulesChanged -= collisionRulesUpdatedDelegate;
				}
				collisionRules = value;
				if (collisionRules != null)
				{
					collisionRules.CollisionRulesChanged += collisionRulesUpdatedDelegate;
				}
				CollisionRulesUpdated();
			}
		}
	}

	/// <summary>
	/// Gets or sets the user data associated with this entry.
	/// </summary>
	public object Tag { get; set; }

	protected BroadPhaseEntry()
	{
		CollisionRules = new CollisionRules();
		collisionRulesUpdatedDelegate = CollisionRulesUpdated;
		hashCode = (int)(base.GetHashCode() * 3625334849u);
	}

	/// <summary>
	/// Gets the object's hash code.
	/// </summary>
	/// <returns>Hash code for the object.</returns>
	public override int GetHashCode()
	{
		return hashCode;
	}

	protected abstract void CollisionRulesUpdated();

	/// <summary>
	/// Tests a ray against the entry.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length, in units of the ray's direction's length, to test.</param>
	/// <param name="rayHit">Hit location of the ray on the entry, if any.</param>
	/// <returns>Whether or not the ray hit the entry.</returns>
	public abstract bool RayCast(Ray ray, float maximumLength, out RayHit rayHit);

	/// <summary>
	/// Tests a ray against the entry.
	/// </summary>
	/// <param name="ray">Ray to test.</param>
	/// <param name="maximumLength">Maximum length, in units of the ray's direction's length, to test.</param>
	/// <param name="filter">Test to apply to try on the entry.  If a collidable hierarchy is present
	/// in the entry, this filter will be passed into inner ray casts.</param>
	/// <param name="rayHit">Hit location of the ray on the entry, if any.</param>
	/// <returns>Whether or not the ray hit the entry.</returns>
	public virtual bool RayCast(Ray ray, float maximumLength, Func<BroadPhaseEntry, bool> filter, out RayHit rayHit)
	{
		if (filter(this))
		{
			return RayCast(ray, maximumLength, out rayHit);
		}
		rayHit = default(RayHit);
		return false;
	}

	/// <summary>
	/// Sweeps a convex shape against the entry.
	/// </summary>
	/// <param name="castShape">Swept shape.</param>
	/// <param name="startingTransform">Beginning location and orientation of the cast shape.</param>
	/// <param name="sweep">Sweep motion to apply to the cast shape.</param>
	/// <param name="hit">Hit data of the ray on the entry, if any.</param>
	/// <returns>Whether or not the ray hit the entry.</returns>
	public abstract bool ConvexCast(ConvexShape castShape, ref RigidTransform startingTransform, ref Vector3 sweep, out RayHit hit);

	/// <summary>
	/// Updates the bounding box to the current state of the entry.
	/// </summary>
	public abstract void UpdateBoundingBox();
}
