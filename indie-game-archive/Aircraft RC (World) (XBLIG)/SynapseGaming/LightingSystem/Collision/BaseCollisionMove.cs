using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Abstract class that provides a base implementation of the ICollisionMove interface.
/// Used to create custom CollisionMove classes for custom and 3rd party collision / physics systems.
/// </summary>
public abstract class BaseCollisionMove : ICollisionMove
{
	private ICollisionObject HCB;

	private ICollisionEntity HC_0002;

	[CompilerGenerated]
	private float HC_0012;

	[CompilerGenerated]
	private Vector3 HCH;

	/// <summary>
	/// Distance the object will move this frame.
	/// </summary>
	public float Distance
	{
		[CompilerGenerated]
		get
		{
			return HC_0012;
		}
		[CompilerGenerated]
		protected set
		{
			HC_0012 = value;
		}
	}

	/// <summary>
	/// Normalized direction the object will move this frame.
	/// </summary>
	public Vector3 Normal
	{
		[CompilerGenerated]
		get
		{
			return HCH;
		}
		[CompilerGenerated]
		protected set
		{
			HCH = value;
		}
	}

	/// <summary>
	/// Object movement is applied to.
	/// </summary>
	public ICollisionObject ParentObject => HCB;

	/// <summary>
	/// Interface to the collision / physics system entity, which represents the ParentObject in the simulation.
	/// </summary>
	public ICollisionEntity CollisionEntity => HC_0002;

	/// <summary>
	/// Creates a new PhysicsMove instance.
	/// </summary>
	/// <param name="parent">Collision object to move.</param>
	public BaseCollisionMove(ICollisionObject parent)
	{
		HCB = parent;
	}

	private bool a()
	{
		if (HC_0002 != null && HCB.CollisionType == CollisionType.Collide)
		{
			return HCB.UpdateType == UpdateType.Automatic;
		}
		return false;
	}

	/// <summary>
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Update().
	/// </summary>
	/// <param name="objectforce">Amount of object-space force to apply to the object.</param>
	public virtual void ApplyObjectForce(Vector3 objectforce)
	{
		if (a())
		{
			Vector3 worldforce = Vector3.TransformNormal(objectforce, HCB.World);
			HC_0002.ApplyWorldForce(ref worldforce);
		}
	}

	/// <summary>
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Update().
	/// </summary>
	/// <param name="objectposition">Object-space location the force is applied to the object.
	/// This allows off-center forces, which cause rotation.</param>
	/// <param name="objectforce">Amount of object-space force to apply to the object.</param>
	public virtual void ApplyObjectForce(Vector3 objectposition, Vector3 objectforce)
	{
		if (a())
		{
			Matrix matrix = HCB.World;
			Vector3.Transform(ref objectposition, ref matrix, out var result);
			Vector3.TransformNormal(ref objectforce, ref matrix, out var result2);
			HC_0002.ApplyWorldForce(ref result, ref result2);
		}
	}

	/// <summary>
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Update().
	/// </summary>
	/// <param name="worldforce">Amount of world-space force to apply to the object.</param>
	/// <param name="constantforce">Determines if the force is from a constant
	/// source such as gravity, wind, or similar (eg: applied by the caller
	/// every frame instead of a single time).</param>
	public virtual void ApplyWorldForce(Vector3 worldforce, bool constantforce)
	{
		if (a())
		{
			HC_0002.ApplyWorldForce(ref worldforce);
		}
	}

	/// <summary>
	/// Applies force to the object. The total force is used to move the
	/// object during the next call to Update().
	/// </summary>
	/// <param name="worldposition">World-space location the force is applied to the object.
	/// This allows off-center forces, which cause rotation.</param>
	/// <param name="worldforce">Amount of world-space force to apply to the object.</param>
	public virtual void ApplyWorldForce(Vector3 worldposition, Vector3 worldforce)
	{
		if (a())
		{
			HC_0002.ApplyWorldForce(ref worldposition, ref worldforce);
		}
	}

	/// <summary>
	/// Prepares the object for movement this frame. Also calculates related
	/// volumes for collision detection.
	/// </summary>
	public virtual void Begin()
	{
		if (HC_0002 != null)
		{
			if (HC_0002.CheckSceneObjectChanged())
			{
				HC_0002.SyncToPhysicsEntity();
			}
			if (HCB.UpdateType == UpdateType.Automatic)
			{
				Distance = HC_0002.Distance;
				Normal = HC_0002.Normal;
			}
		}
	}

	/// <summary>
	/// Finishes the object move and changes the object position to the specified
	/// world collision point.
	/// </summary>
	public virtual void End()
	{
		if (HC_0002 != null && HCB.UpdateType != UpdateType.None)
		{
			HC_0002.SyncToSceneObject();
		}
	}

	/// <summary>
	/// Removes all accumulated forces acting on the object. This will halt the object
	/// movement, however future forces (such as gravity) can immediately begin acting
	/// on the object again.
	/// </summary>
	public virtual void RemoveForces()
	{
		if (HC_0002 != null)
		{
			HC_0002.RemoveForces();
		}
	}

	/// <summary>
	/// Called when the parent object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnRemovedFromManager(IManagerService manager)
	{
		if (HC_0002 != null)
		{
			HC_0002.Dispose();
			HC_0002 = null;
		}
	}

	/// <summary>
	/// Called when the parent object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnSubmittedToManager(IManagerService manager)
	{
		if (HCB.CollisionType != CollisionType.None)
		{
			if (HC_0002 != null)
			{
				HC_0002.Dispose();
			}
			HC_0002 = CreateCollisionEntity();
		}
	}

	/// <summary>
	/// Creates a new collision / physics system entity, which represents the ParentObject in the simulation.
	/// </summary>
	/// <returns></returns>
	protected abstract ICollisionEntity CreateCollisionEntity();
}
