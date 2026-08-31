using System;
using BEPUphysics.Collidables.Events;
using BEPUphysics.CollisionShapes;
using BEPUphysics.Entities;
using BEPUphysics.MathExtensions;
using BEPUphysics.PositionUpdating;
using BEPUphysics.Settings;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Collidables.MobileCollidables;

/// <summary>
///  Mobile collidable acting as a collision proxy for an entity.
/// </summary>
public abstract class EntityCollidable : MobileCollidable
{
	protected internal Entity entity;

	protected internal RigidTransform worldTransform;

	protected internal Vector3 localPosition;

	protected internal ContactEventManager<EntityCollidable> events;

	/// <summary>
	/// Gets the shape of the collidable.
	/// </summary>
	public new EntityShape Shape
	{
		get
		{
			return (EntityShape)shape;
		}
		protected set
		{
			base.Shape = value;
		}
	}

	/// <summary>
	///  Gets the entity owning the collidable.
	/// </summary>
	public Entity Entity
	{
		get
		{
			return entity;
		}
		protected internal set
		{
			entity = value;
			OnEntityChanged();
		}
	}

	/// <summary>
	///  Gets or sets the world transform of the collidable.
	///  The EntityCollidable's LocalPosition is ignored for this process; the shape will end up
	///  centered exactly on the world transform.
	///  Setting this property also updates the bounding box.
	/// </summary>
	public RigidTransform WorldTransform
	{
		get
		{
			return worldTransform;
		}
		set
		{
			Quaternion.Conjugate(ref value.Orientation, out var result);
			Vector3.Transform(ref localPosition, ref result, out var result2);
			Vector3.Subtract(ref value.Position, ref result2, out value.Position);
			UpdateBoundingBoxForTransform(ref value);
		}
	}

	protected internal override bool IsActive
	{
		get
		{
			if (entity == null)
			{
				return false;
			}
			return entity.activityInformation.IsActive;
		}
	}

	/// <summary>
	///  Gets or sets the local position of the collidable.
	///  The local position can be used to offset the collision geometry
	///  from an entity's center of mass.
	/// </summary>
	public Vector3 LocalPosition
	{
		get
		{
			return localPosition;
		}
		set
		{
			localPosition = value;
		}
	}

	/// <summary>
	///  Gets or sets the event manager of the collidable.
	/// </summary>
	public ContactEventManager<EntityCollidable> Events
	{
		get
		{
			return events;
		}
		set
		{
			if (value.Owner != null && value != events)
			{
				throw new Exception("Event manager is already owned by an entity; event managers cannot be shared.");
			}
			CompoundEventManager parent = null;
			if (events != null)
			{
				events.Owner = null;
				parent = events.Parent;
				events.Parent = null;
			}
			events = value;
			if (events != null)
			{
				events.Owner = this;
				events.Parent = parent;
			}
		}
	}

	protected internal override IContactEventTriggerer EventTriggerer => events;

	/// <summary>
	///  Gets an enumerable collection of all entities overlapping this collidable.
	/// </summary>
	public EntityCollidableCollection OverlappedEntities => new EntityCollidableCollection(this);

	protected EntityCollidable()
	{
	}

	protected EntityCollidable(EntityShape shape)
	{
		base.Shape = shape;
	}

	protected virtual void OnEntityChanged()
	{
	}

	/// <summary>
	///  Updates the bounding box of the mobile collidable according to the associated entity's current state.
	///  Do not use this if the EntityCollidable does not have an associated entity; consider using
	///  UpdateBoundingBoxForTransform instead.
	/// </summary>
	public override void UpdateBoundingBox()
	{
		UpdateBoundingBox(0f);
	}

	/// <summary>
	///  Updates the bounding box of the mobile collidable according to the associated entity's current state.
	///  Do not use this if the EntityCollidable does not have an associated entity; consider using
	///  UpdateBoundingBoxForTransform instead.
	/// </summary>
	/// <param name="dt">Timestep with which to update the bounding box.</param>
	public override void UpdateBoundingBox(float dt)
	{
		UpdateWorldTransform(ref entity.position, ref entity.orientation);
		UpdateBoundingBoxInternal(dt);
	}

	/// <summary>
	///  Updates the world transform of the shape using the given position and orientation.
	///  The world transform of the shape is offset from the given position and orientation by the collidable's LocalPosition.
	/// </summary>
	/// <param name="position">Position to use for the calculation.</param>
	/// <param name="orientation">Orientation to use for the calculation.</param>
	public virtual void UpdateWorldTransform(ref Vector3 position, ref Quaternion orientation)
	{
		Vector3.Transform(ref localPosition, ref orientation, out worldTransform.Position);
		Vector3.Add(ref worldTransform.Position, ref position, out worldTransform.Position);
		worldTransform.Orientation = orientation;
	}

	/// <summary>
	/// Updates the collidable's world transform and bounding box.  The transform provided
	/// will be offset by the collidable's LocalPosition to get the shape transform.
	/// This is a convenience method for external modification of the collidable's data.
	/// </summary>
	/// <param name="transform">Transform to use for the collidable.</param>
	/// <param name="dt">Duration of the simulation time step.  Used to expand the
	/// bounding box using the owning entity's velocity.  If the collidable
	/// does not have an owning entity, this must be zero.</param>
	public void UpdateBoundingBoxForTransform(ref RigidTransform transform, float dt)
	{
		UpdateWorldTransform(ref transform.Position, ref transform.Orientation);
		UpdateBoundingBoxInternal(dt);
	}

	/// <summary>
	/// Updates the collidable's world transform and bounding box.
	/// This is a convenience method for external modification of the collidable's data.
	/// </summary>
	/// <param name="transform">Transform to use for the collidable.</param>
	public void UpdateBoundingBoxForTransform(ref RigidTransform transform)
	{
		UpdateBoundingBoxForTransform(ref transform, 0f);
	}

	protected internal abstract void UpdateBoundingBoxInternal(float dt);

	internal void ExpandBoundingBox(ref BoundingBox boundingBox, float dt)
	{
		if (!(dt > 0f))
		{
			return;
		}
		bool flag = MotionSettings.UseExtraExpansionForContinuousBoundingBoxes && entity.PositionUpdateMode == PositionUpdateMode.Continuous;
		float num = ((!flag) ? 1 : 2);
		if (entity.linearVelocity.X > 0f)
		{
			boundingBox.Max.X += entity.linearVelocity.X * dt * num;
		}
		else
		{
			boundingBox.Min.X += entity.linearVelocity.X * dt * num;
		}
		if (entity.linearVelocity.Y > 0f)
		{
			boundingBox.Max.Y += entity.linearVelocity.Y * dt * num;
		}
		else
		{
			boundingBox.Min.Y += entity.linearVelocity.Y * dt * num;
		}
		if (entity.linearVelocity.Z > 0f)
		{
			boundingBox.Max.Z += entity.linearVelocity.Z * dt * num;
		}
		else
		{
			boundingBox.Min.Z += entity.linearVelocity.Z * dt * num;
		}
		if (!flag)
		{
			return;
		}
		float num2 = 0f;
		foreach (Entity overlappedEntity in OverlappedEntities)
		{
			float num3 = overlappedEntity.linearVelocity.LengthSquared();
			if (num3 > num2)
			{
				num2 = num3;
			}
		}
		num2 = (float)Math.Sqrt(num2) * dt;
		boundingBox.Min.X -= num2;
		boundingBox.Min.Y -= num2;
		boundingBox.Min.Z -= num2;
		boundingBox.Max.X += num2;
		boundingBox.Max.Y += num2;
		boundingBox.Max.Z += num2;
	}

	protected override void CollisionRulesUpdated()
	{
		if (entity != null)
		{
			entity.activityInformation.Activate();
		}
	}
}
