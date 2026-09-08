using System;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Collision.Legacy;

/// <summary>
/// Provides the built-in implement of object collision movement
/// and applying force.
/// </summary>
public class CollisionMove : ICollisionMove
{
	private CollisionMesh a5h;

	private ICollisionObject a5b;

	private bool a56 = true;

	private bool a5a;

	private int a57 = -1;

	private int a5_0006;

	private Plane a5v;

	private float a5B;

	private Vector3 a5X;

	private Vector3 a5_0018;

	private Vector3 a5W;

	private Vector3 a5_0002;

	private Matrix a5_000E;

	private BoundingSphere a5y;

	private BoundingBox a5r;

	private BoundingBox a5_0001;

	/// <summary>
	/// Determines if the object should slide or lose momentum when hitting a surface on a grazing angle.
	/// </summary>
	public bool SlidingEnabled
	{
		get
		{
			return a56;
		}
		set
		{
			a56 = value;
		}
	}

	/// <summary>
	/// Determines if the object is currently contacting another object or surface.
	/// </summary>
	public bool OnSurface => a5a;

	/// <summary>
	/// Plane of the surface the object is in contact with. Use OnSurface to determine
	/// if the object is currently contacting a surface.
	/// </summary>
	public Plane CollisionSurface => a5v;

	/// <summary>
	/// Distance the object will move this frame. Valid after calling Begin().
	/// </summary>
	public float Distance => a5B;

	/// <summary>
	/// Direction the object will move this frame. Valid after calling Begin().
	/// </summary>
	public Vector3 Normal => a5_0002;

	/// <summary>
	/// Amount of force applied to the object this frame from constant
	/// sources such as gravity, wind, or similar.
	///
	/// The amount of force applied to the object is affected by the
	/// collision time period (see GetForce() for details).
	/// </summary>
	public Vector3 ForceConstant
	{
		get
		{
			return a5X;
		}
		set
		{
			a5X = value;
		}
	}

	/// <summary>
	/// Amount of force applied to the object this frame from impulse
	/// sources such as impacts, exlposions, and similar.
	///
	/// The amount of force applied to the object is NOT affected by the
	/// collision time period (see GetForce() for details).
	/// </summary>
	public Vector3 ForceImpulse
	{
		get
		{
			return a5_0018;
		}
		set
		{
			a5_0018 = value;
		}
	}

	/// <summary>
	/// Amount of force carried over into the next frame due to collisions.
	/// </summary>
	public Vector3 CarryOver
	{
		get
		{
			return a5W;
		}
		set
		{
			a5W = value;
		}
	}

	/// <summary>
	/// World bounding area the object will move to this frame.
	/// </summary>
	public BoundingSphere WorldBoundingSphere => a5y;

	/// <summary>
	/// World bounding area the object will move to this frame.
	/// </summary>
	public BoundingBox WorldBoundingBox => a5r;

	/// <summary>
	/// Total world bounding area of the object movement. This includes
	/// both the current and next world bounding areas.
	/// </summary>
	public BoundingBox WorldSweepBoundingBox => a5_0001;

	/// <summary>
	/// Access to the world space geometry used for collision.
	/// </summary>
	public CollisionMesh WorldCollisionMesh
	{
		get
		{
			int moveId = a5b.MoveId;
			if (a57 == moveId)
			{
				return a5h;
			}
			if (a5h == null)
			{
				a5h = new CollisionMesh();
			}
			if (a5b is ISceneObject sceneObject)
			{
				CollisionMeshBuilder.BuildWorldMesh(sceneObject.RenderableMeshes, a5h);
			}
			a57 = moveId;
			return a5h;
		}
	}

	/// <summary>
	/// Creates a new CollisionMove instance.
	/// </summary>
	/// <param name="parent">Collision object to move.</param>
	public CollisionMove(ICollisionObject parent)
	{
		a5b = parent;
	}

	/// <summary>
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Begin().
	/// </summary>
	/// <param name="objectforce">Amount of object-space force to apply to the object.</param>
	public void ApplyObjectForce(Vector3 objectforce)
	{
		Matrix matrix = a5b.World;
		Vector3.TransformNormal(ref objectforce, ref matrix, out var result);
		a5_0018.X += result.X;
		a5_0018.Y += result.Y;
		a5_0018.Z += result.Z;
	}

	/// <summary>
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Update().
	/// </summary>
	/// <param name="objectposition">Object-space location the force is applied to the object.
	/// This allows off-center forces, which cause rotation.</param>
	/// <param name="objectforce">Amount of object-space force to apply to the object.</param>
	public void ApplyObjectForce(Vector3 objectposition, Vector3 objectforce)
	{
		ApplyObjectForce(objectforce);
	}

	/// <summary>
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Begin().
	/// </summary>
	/// <param name="worldforce">Amount of world-space force to apply to the object.</param>
	/// <param name="constantforce">Determines if the force is from a constant
	/// source such as gravity, wind, or similar (eg: applied by the caller
	/// every frame instead of a single time).</param>
	public void ApplyWorldForce(Vector3 worldforce, bool constantforce)
	{
		if (constantforce)
		{
			a5X.X += worldforce.X;
			a5X.Y += worldforce.Y;
			a5X.Z += worldforce.Z;
		}
		else
		{
			a5_0018.X += worldforce.X;
			a5_0018.Y += worldforce.Y;
			a5_0018.Z += worldforce.Z;
		}
	}

	/// <summary>
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Update().
	/// </summary>
	/// <param name="worldposition">World-space location the force is applied to the object.
	/// This allows off-center forces, which cause rotation.</param>
	/// <param name="worldforce">Amount of world-space force to apply to the object.</param>
	public void ApplyWorldForce(Vector3 worldposition, Vector3 worldforce)
	{
		ApplyWorldForce(worldforce, constantforce: false);
	}

	/// <summary>
	/// Removes all accumulated forces acting on the object. This will halt the object
	/// movement, however future forces (such as gravity) can immediately begin acting
	/// on the object again.
	/// </summary>
	public void RemoveForces()
	{
		a5W = (a5X = (a5_0018 = Vector3.Zero));
	}

	/// <summary>
	/// Gets the cumulative amount of force affecting the object for the time period.
	/// </summary>
	/// <param name="timeperiod">Normalized time period from 0.0 (start of the
	/// movement this frame) to 1.0 (end of the movement this frame).</param>
	/// <returns></returns>
	public Vector3 GetForce(float timeperiod)
	{
		return a5X * timeperiod + a5_0018;
	}

	/// <summary>
	/// Prepares the object for movement this frame. Also calculates related
	/// volumes for collision detection.
	/// </summary>
	/// <param name="numberofvirtualupdates">Number of virtual frames that elapsed
	/// since the last call to Begin(). Allows collisions to be calculated without
	/// using a fixed time step.</param>
	public void Begin(float numberofvirtualupdates)
	{
		int moveId = a5b.MoveId;
		if (a5_0006 != moveId || a5b.UpdateType != UpdateType.None)
		{
			Vector3 vector = (a5X + a5_0018) * numberofvirtualupdates;
			if (numberofvirtualupdates > 0f)
			{
				vector += a5W;
				a5W = Vector3.Zero;
			}
			a5B = vector.Length();
			if (a5B <= 0f)
			{
				a5_0002 = Vector3.Zero;
			}
			else
			{
				float num = 1f / a5B;
				a5_0002 = vector * num;
			}
			a5y = a5b.WorldBoundingSphere;
			a5y.Center.X += vector.X;
			a5y.Center.Y += vector.Y;
			a5y.Center.Z += vector.Z;
			BoundingBox additional = (a5r = a5b.WorldBoundingBox);
			a5r.Min.X += vector.X;
			a5r.Min.Y += vector.Y;
			a5r.Min.Z += vector.Z;
			a5r.Max.X += vector.X;
			a5r.Max.Y += vector.Y;
			a5r.Max.Z += vector.Z;
			BoundingBox.CreateMerged(ref a5r, ref additional, out a5_0001);
			a5_000E = a5b.World;
			a5_000E.Translation += vector;
			a5_0006 = moveId;
		}
	}

	/// <summary>
	/// Finishes the object move and changes the object position to the specified
	/// world collision point.
	/// </summary>
	/// <param name="worldcollisionpoint">Contains information about the closest
	/// collision point to the object.</param>
	public void End(CollisionPoint worldcollisionpoint)
	{
		ICollisionObject collisionObject = a5b;
		foreach (ICollisionObject trigger in worldcollisionpoint.Triggers)
		{
			collisionObject.OnCollisionTrigger(collisionObject, trigger);
			trigger.OnCollisionTrigger(collisionObject, trigger);
		}
		if (a5B <= 0f)
		{
			bd();
		}
		else if (worldcollisionpoint.ContactTime < 1f)
		{
			Matrix world = a5b.World;
			world.Translation += a5_0002 * (a5B * worldcollisionpoint.ContactTime);
			React(worldcollisionpoint);
			collisionObject.SetWorldAndWorldToObject(world, Matrix.Invert(world));
			a5a = true;
			a5v.Normal = worldcollisionpoint.SurfaceNormal;
			a5v.D = 0f - Vector3.Dot(worldcollisionpoint.SurfaceNormal, worldcollisionpoint.ContactPoint);
			bd();
			a5_0006 = a5b.MoveId;
		}
		else
		{
			collisionObject.World = a5_000E;
			a5a = false;
			bd();
			a5_0006 = a5b.MoveId;
		}
	}

	private void bd()
	{
		a5_0018 += a5X;
		a5X = Vector3.Zero;
	}

	/// <summary>
	/// Calculates and applies the reaction force between the
	/// object and the collision surface contained in the CollisionPoint.
	/// </summary>
	/// <param name="worldcollisionpoint">Contains information about the closest collision point to the collider.</param>
	public virtual void React(CollisionPoint worldcollisionpoint)
	{
		ICollisionObject contactObject = worldcollisionpoint.ContactObject;
		bool collisionhandled = false;
		if (a5b.CollisionType != CollisionType.Collide || contactObject.CollisionType != CollisionType.Collide)
		{
			return;
		}
		a5b.OnCollisionReact(a5b, contactObject, worldcollisionpoint, ref collisionhandled);
		contactObject.OnCollisionReact(a5b, contactObject, worldcollisionpoint, ref collisionhandled);
		if (!collisionhandled)
		{
			float mass = a5b.Mass;
			float num = 0.5f;
			float num2 = mass + contactObject.Mass;
			if (num2 > 0f)
			{
				num = mass / num2;
			}
			ICollisionMaterial defaultCollisionMaterial = a5b.DefaultCollisionMaterial;
			float num3 = ((defaultCollisionMaterial == null) ? 0.75f : (1f - defaultCollisionMaterial.Elasticity));
			if (worldcollisionpoint.Material != null)
			{
				num3 *= 1f - worldcollisionpoint.Material.Elasticity;
			}
			Vector3 vector = a5_0002;
			Vector3 vector2 = worldcollisionpoint.SurfaceNormal;
			Vector3.Dot(ref vector, ref vector2, out var result);
			result = (1f - Math.Abs(result)) * 2f - 1f;
			result = MathHelper.Clamp(result, 0f, 1f);
			float num4 = result * 0.01f;
			if (!a56)
			{
				vector -= vector2 * num4 * 10f;
			}
			else if (num4 > 0f)
			{
				vector -= vector2 * num4;
				float num5 = 0.8f;
				float num6 = 2f;
				float num7 = worldcollisionpoint.ContactTime - num5;
				num7 *= num6;
				num7 = MathHelper.Clamp(1f - num7, 0f, 1f);
				num3 = MathHelper.Lerp(num3, 1f, result * num7);
			}
			Vector3 force = GetForce(worldcollisionpoint.ContactTime);
			Vector3.Reflect(ref vector, ref vector2, out var result2);
			Vector3 vector3 = result2 * force.Length() * num3;
			a5W = result2 * (a5B * num3 * (1f - worldcollisionpoint.ContactTime));
			if (contactObject.UpdateType == UpdateType.None)
			{
				a5_0018 = vector3;
				a5X = Vector3.Zero;
				return;
			}
			Vector3 vector4 = vector3 * (1f - num);
			Vector3 vector5 = vector3 * num;
			a5_0018 = vector4;
			a5X = Vector3.Zero;
			contactObject.CollisionMove.ApplyWorldForce(-vector5, constantforce: false);
		}
	}

	/// <summary>
	/// Called when the parent object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	public void OnSubmittedToManager(IManagerService manager)
	{
	}

	/// <summary>
	/// Called when the parent object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public void OnRemovedFromManager(IManagerService manager)
	{
	}
}
