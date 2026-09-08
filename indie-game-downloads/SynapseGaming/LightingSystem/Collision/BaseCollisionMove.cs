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
	private ICollisionObject _3A_0018;

	private ICollisionEntity _3AL;

	[CompilerGenerated]
	private float _3A_0019;

	[CompilerGenerated]
	private Vector3 _3A3;

	/// <summary>
	/// Distance the object will move this frame.
	/// </summary>
	public float Distance
	{
		[CompilerGenerated]
		get
		{
			return _3A_0019;
		}
		[CompilerGenerated]
		protected set
		{
			_3A_0019 = value;
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
			return _3A3;
		}
		[CompilerGenerated]
		protected set
		{
			_3A3 = value;
		}
	}

	/// <summary>
	/// Object movement is applied to.
	/// </summary>
	public ICollisionObject ParentObject => _3A_0018;

	/// <summary>
	/// Interface to the collision / physics system entity, which represents the ParentObject in the simulation.
	/// </summary>
	public ICollisionEntity CollisionEntity => _3AL;

	/// <summary>
	/// Creates a new PhysicsMove instance.
	/// </summary>
	/// <param name="parent">Collision object to move.</param>
	public BaseCollisionMove(ICollisionObject parent)
	{
		_3A_0018 = parent;
	}

	private bool _5()
	{
		if (_3AL != null && _3A_0018.CollisionType == CollisionType.Collide)
		{
			return _3A_0018.UpdateType == UpdateType.Automatic;
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
		if (_5())
		{
			Vector3 worldforce = Vector3.TransformNormal(objectforce, _3A_0018.World);
			_3AL.ApplyWorldForce(ref worldforce);
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
		if (_5())
		{
			Matrix matrix = _3A_0018.World;
			Vector3.Transform(ref objectposition, ref matrix, out var result);
			Vector3.TransformNormal(ref objectforce, ref matrix, out var result2);
			_3AL.ApplyWorldForce(ref result, ref result2);
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
		if (_5())
		{
			_3AL.ApplyWorldForce(ref worldforce);
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
		if (_5())
		{
			_3AL.ApplyWorldForce(ref worldposition, ref worldforce);
		}
	}

	/// <summary>
	/// Prepares the object for movement this frame. Also calculates related
	/// volumes for collision detection.
	/// </summary>
	public virtual void Begin()
	{
		if (_3AL != null)
		{
			if (_3AL.CheckSceneObjectChanged())
			{
				_3AL.SyncToPhysicsEntity();
			}
			if (_3A_0018.UpdateType == UpdateType.Automatic)
			{
				Distance = _3AL.Distance;
				Normal = _3AL.Normal;
			}
		}
	}

	/// <summary>
	/// Finishes the object move and changes the object position to the specified
	/// world collision point.
	/// </summary>
	public virtual void End()
	{
		if (_3AL != null && _3A_0018.UpdateType != UpdateType.None)
		{
			_3AL.SyncToSceneObject();
		}
	}

	/// <summary>
	/// Removes all accumulated forces acting on the object. This will halt the object
	/// movement, however future forces (such as gravity) can immediately begin acting
	/// on the object again.
	/// </summary>
	public virtual void RemoveForces()
	{
		if (_3AL != null)
		{
			_3AL.RemoveForces();
		}
	}

	/// <summary>
	/// Called when the parent object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnRemovedFromManager(IManagerService manager)
	{
		if (_3AL != null)
		{
			_3AL.Dispose();
			_3AL = null;
		}
	}

	/// <summary>
	/// Called when the parent object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnSubmittedToManager(IManagerService manager)
	{
		if (_3A_0018.CollisionType != CollisionType.None)
		{
			if (_3AL != null)
			{
				_3AL.Dispose();
			}
			_3AL = CreateCollisionEntity();
		}
	}

	/// <summary>
	/// Creates a new collision / physics system entity, which represents the ParentObject in the simulation.
	/// </summary>
	/// <returns></returns>
	protected abstract ICollisionEntity CreateCollisionEntity();
}
