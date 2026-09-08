using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using _000F;
using Microsoft.Xna.Framework;
using Q;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Collision.Controllers;

/// <summary>
/// Provides a character controller that utilizes the physics system and is directly assignable to scene objects by implementing ICollisionMove.
/// </summary>
public class PhysicsMoveCharacterController : PhysicsMove, ICharacterController, IObjectController, ICharacterControllerProperties, IDisposable
{
	private Q.h a5h;

	private static List<ICharacterController> a5b = new List<ICharacterController>();

	[CompilerGenerated]
	private bool a56;

	[CompilerGenerated]
	private float a5a;

	[CompilerGenerated]
	private float a57;

	[CompilerGenerated]
	private float a5_0006;

	[CompilerGenerated]
	private float a5v;

	[CompilerGenerated]
	private float a5B;

	[CompilerGenerated]
	private float a5X;

	[CompilerGenerated]
	private float a5_0018;

	[CompilerGenerated]
	private float a5W;

	[CompilerGenerated]
	private float a5_0002;

	[CompilerGenerated]
	private PlayerIndex a5_000E;

	/// <summary>
	/// Provides access to all active PhysicsMoveCharacterController objects.
	/// </summary>
	public static List<ICharacterController> CharacterControllers => a5b;

	/// <summary>
	/// Determines if the character is currently crouching.
	/// </summary>
	public bool Crouching
	{
		get
		{
			if (a5h == null)
			{
				return false;
			}
			return a5h.Controller.StanceManager.CurrentStance == _000F.v.Crouching;
		}
	}

	/// <summary>
	/// Determines if the character is currently sliding across a surface.
	/// </summary>
	public bool Sliding
	{
		get
		{
			if (a5h == null)
			{
				return false;
			}
			_000F.X supportFinder = a5h.Controller.SupportFinder;
			if (supportFinder.HasSupport)
			{
				return !supportFinder.HasTraction;
			}
			return false;
		}
	}

	/// <summary>
	/// Determines if the character is currently on a surface and has traction.
	/// </summary>
	public bool Resting
	{
		get
		{
			if (a5h == null)
			{
				return false;
			}
			_000F.X supportFinder = a5h.Controller.SupportFinder;
			if (supportFinder.HasSupport)
			{
				return supportFinder.HasTraction;
			}
			return false;
		}
	}

	/// <summary>
	/// Causes the character to crouch.
	/// </summary>
	public bool Crouch
	{
		[CompilerGenerated]
		get
		{
			return a56;
		}
		[CompilerGenerated]
		set
		{
			a56 = value;
		}
	}

	/// <summary>
	/// Height of object when in crouched stance.
	/// </summary>
	public float CrouchHeight
	{
		[CompilerGenerated]
		get
		{
			return a5a;
		}
		[CompilerGenerated]
		set
		{
			a5a = value;
		}
	}

	/// <summary>
	/// Amount of smoothing applied to object movement.
	/// </summary>
	public float MovementSmoothing
	{
		[CompilerGenerated]
		get
		{
			return a57;
		}
		[CompilerGenerated]
		set
		{
			a57 = value;
		}
	}

	/// <summary>
	/// Vertical speed applied to object when jumping.
	/// </summary>
	public float JumpSpeed
	{
		[CompilerGenerated]
		get
		{
			return a5_0006;
		}
		[CompilerGenerated]
		set
		{
			a5_0006 = value;
		}
	}

	/// <summary>
	/// Maximum height of obstruction object can step up onto.
	/// </summary>
	public float MaximumStepHeight
	{
		[CompilerGenerated]
		get
		{
			return a5v;
		}
		[CompilerGenerated]
		set
		{
			a5v = value;
		}
	}

	/// <summary>
	/// Maximum speed of object when moving.
	/// </summary>
	public float MaximumSpeed
	{
		[CompilerGenerated]
		get
		{
			return a5B;
		}
		[CompilerGenerated]
		set
		{
			a5B = value;
		}
	}

	/// <summary>
	/// Maximum speed of object when moving in crouched stance.
	/// </summary>
	public float MaximumCrouchSpeed
	{
		[CompilerGenerated]
		get
		{
			return a5X;
		}
		[CompilerGenerated]
		set
		{
			a5X = value;
		}
	}

	/// <summary>
	/// Maximum speed of object when sliding across a surface.
	/// </summary>
	public float MaximumSlideSpeed
	{
		[CompilerGenerated]
		get
		{
			return a5_0018;
		}
		[CompilerGenerated]
		set
		{
			a5_0018 = value;
		}
	}

	/// <summary>
	/// Maximum speed of object when falling.
	/// </summary>
	public float MaximumAirborneSpeed
	{
		[CompilerGenerated]
		get
		{
			return a5W;
		}
		[CompilerGenerated]
		set
		{
			a5W = value;
		}
	}

	/// <summary>
	/// Amount of force applied to the object each update. This controls how quickly the object reaches its maximum speed.
	/// </summary>
	public float MovementForce
	{
		[CompilerGenerated]
		get
		{
			return a5_0002;
		}
		[CompilerGenerated]
		set
		{
			a5_0002 = value;
		}
	}

	/// <summary>
	/// Player the object is controlled by.
	/// </summary>
	public PlayerIndex PlayerIndex
	{
		[CompilerGenerated]
		get
		{
			return a5_000E;
		}
		[CompilerGenerated]
		set
		{
			a5_000E = value;
		}
	}

	/// <summary>
	/// Creates a new PhysicsMoveCharacterController instance.
	/// </summary>
	/// <param name="parent">Collision object to move.</param>
	public PhysicsMoveCharacterController(ICollisionObject parent)
		: base(parent)
	{
		JumpSpeed = 4.5f;
		MovementSmoothing = 0.25f;
		MaximumSpeed = 8f;
		MaximumCrouchSpeed = 3f;
		MaximumSlideSpeed = 6f;
		MaximumAirborneSpeed = 1f;
		MovementForce = 1000f;
	}

	/// <summary>
	/// Creates a new collision / physics system entity, which represents the ParentObject in the simulation.
	/// </summary>
	/// <returns></returns>
	protected override ICollisionEntity CreateCollisionEntity()
	{
		if (base.PhysicsManager == null)
		{
			return null;
		}
		a5h = new Q.h(base.ParentObject);
		a5h.Build(base.PhysicsManager.Space);
		_000F._0006 stanceManager = a5h.Controller.StanceManager;
		if (CrouchHeight == 0f)
		{
			CrouchHeight = stanceManager.CrouchingHeight;
		}
		if (MaximumStepHeight == 0f)
		{
			MaximumStepHeight = stanceManager.StandingHeight * 0.2f;
		}
		a5b.Add(this);
		return a5h;
	}

	/// <summary>
	/// Called when the parent object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public override void OnRemovedFromManager(IManagerService manager)
	{
		base.OnRemovedFromManager(manager);
		Dispose();
	}

	/// <summary />
	public void Dispose()
	{
		if (a5h != null)
		{
			a5h.Dispose();
			a5h = null;
		}
		a5b.Remove(this);
	}

	/// <summary>
	/// Prepares the object for movement this frame. Also calculates related
	/// volumes for collision detection.
	/// </summary>
	public override void Begin()
	{
		base.Begin();
		if (a5h != null)
		{
			_000F.h controller = a5h.Controller;
			_000F._0006 stanceManager = controller.StanceManager;
			_000F.B stepManager = controller.StepManager;
			_000F.b horizontalMotionConstraint = controller.HorizontalMotionConstraint;
			a5h.MovementSmoothing = MovementSmoothing;
			stanceManager.DesiredStance = (Crouch ? _000F.v.Crouching : _000F.v.Standing);
			stanceManager.CrouchingHeight = CrouchHeight;
			stepManager.MaximumStepHeight = MaximumStepHeight;
			controller.JumpSpeed = JumpSpeed;
			controller.SlidingJumpSpeed = JumpSpeed * 0.5f;
			horizontalMotionConstraint.Speed = MaximumSpeed;
			horizontalMotionConstraint.CrouchingSpeed = MaximumCrouchSpeed;
			horizontalMotionConstraint.SlidingSpeed = MaximumSlideSpeed;
			horizontalMotionConstraint.AirSpeed = MaximumAirborneSpeed;
			horizontalMotionConstraint.MaximumForce = MovementForce;
			horizontalMotionConstraint.MaximumSlidingForce = MovementForce * 0.05f;
			horizontalMotionConstraint.MaximumAirForce = MovementForce * 0.25f;
		}
	}

	/// <summary>
	/// Finishes the object move and changes the object position to the specified
	/// world collision point.
	/// </summary>
	public override void End()
	{
		base.End();
		if (a5h != null)
		{
			a5h.Controller.HorizontalMotionConstraint.MovementDirection = Vector2.Zero;
		}
	}

	private Vector2 bJ(Vector2 P_0)
	{
		Matrix world = base.ParentObject.World;
		Vector3 forward = world.Forward;
		forward.Y = 0f;
		forward.Normalize();
		Vector3 right = world.Right;
		return P_0.Y * new Vector2(forward.X, forward.Z) + P_0.X * new Vector2(right.X, right.Z);
	}

	/// <summary>
	/// Applies movement in object space to the ParentObject.
	/// </summary>
	/// <param name="objectvector"></param>
	public void Move(Vector2 objectvector)
	{
		if (a5h != null)
		{
			a5h.Controller.HorizontalMotionConstraint.MovementDirection += bJ(objectvector);
		}
	}

	/// <summary>
	/// Applies forward movement to the ParentObject.
	/// </summary>
	/// <param name="amount"></param>
	public void MoveForward(float amount)
	{
		Move(Vector2.UnitY);
	}

	/// <summary>
	/// Applies backward movement to the ParentObject.
	/// </summary>
	/// <param name="amount"></param>
	public void MoveBackward(float amount)
	{
		Move(-Vector2.UnitY);
	}

	/// <summary>
	/// Applies left movement to the ParentObject.
	/// </summary>
	/// <param name="amount"></param>
	public void MoveLeft(float amount)
	{
		Move(-Vector2.UnitX);
	}

	/// <summary>
	/// Applies right movement to the ParentObject.
	/// </summary>
	/// <param name="amount"></param>
	public void MoveRight(float amount)
	{
		Move(Vector2.UnitX);
	}

	/// <summary>
	/// Applies object space rotation around the Y axis (the "Up" axis) to the ParentObject.
	/// </summary>
	/// <param name="amount"></param>
	public void Turn(float amount)
	{
		if (a5h != null)
		{
			a5h.RotationalMotionConstraint.Turn(amount);
		}
	}

	/// <summary>
	/// Causes the character to jump.
	/// </summary>
	public void Jump()
	{
		if (a5h != null)
		{
			a5h.Controller.Jump();
		}
	}
}
