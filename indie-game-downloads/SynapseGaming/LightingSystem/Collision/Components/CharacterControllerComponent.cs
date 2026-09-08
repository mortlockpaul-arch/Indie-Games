using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Collision.Controllers;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Serialization;

namespace SynapseGaming.LightingSystem.Collision.Components;

/// <summary>
/// Provides a convenient wrapper for the PhysicsMoveCharacterController
/// character controller.
///
/// Adding this component to an object will automatically provide character controller
/// support, as well as serialization of controller properties.
/// </summary>
[Serializable]
public class CharacterControllerComponent : BaseComponentAutoSerialization<ISceneEntity>, ICharacterControllerProperties
{
	private PhysicsMoveCharacterController a5h;

	[CompilerGenerated]
	private float a5b;

	[CompilerGenerated]
	private float a56;

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
	private PlayerIndex a5W;

	/// <summary>
	/// Height of object when in crouched stance.
	/// </summary>
	[SerializeMember]
	[EditorProperty(true, Description = "Crouch Height", HorizontalAlignment = true, MajorGrouping = 1, MinorGrouping = 2)]
	public float CrouchHeight
	{
		[CompilerGenerated]
		get
		{
			return a5b;
		}
		[CompilerGenerated]
		set
		{
			a5b = value;
		}
	}

	/// <summary>
	/// Amount of smoothing applied to object movement.
	/// </summary>
	[EditorProperty(true, Description = "Move Smoothing", HorizontalAlignment = true, MajorGrouping = 2, MinorGrouping = 2)]
	[SerializeMember]
	public float MovementSmoothing
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
	/// Vertical speed applied to object when jumping.
	/// </summary>
	[SerializeMember]
	[EditorProperty(true, Description = "Jump Speed", HorizontalAlignment = true, MajorGrouping = 3, MinorGrouping = 3)]
	public float JumpSpeed
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
	/// Maximum height of obstruction object can step up onto.
	/// </summary>
	[EditorProperty(true, Description = "Step Height", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 3)]
	[SerializeMember]
	public float MaximumStepHeight
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
	/// Maximum speed of object when moving.
	/// </summary>
	[EditorProperty(true, Description = "Move Speed", HorizontalAlignment = true, MajorGrouping = 3, MinorGrouping = 1)]
	[SerializeMember]
	public float MaximumSpeed
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
	/// Maximum speed of object when moving in crouched stance.
	/// </summary>
	[EditorProperty(true, Description = "Crouch Speed", HorizontalAlignment = true, MajorGrouping = 3, MinorGrouping = 2)]
	[SerializeMember]
	public float MaximumCrouchSpeed
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
	/// Maximum speed of object when sliding across a surface.
	/// </summary>
	[EditorProperty(true, Description = "Slide Speed", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 1)]
	[SerializeMember]
	public float MaximumSlideSpeed
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
	/// Maximum speed of object when falling.
	/// </summary>
	[EditorProperty(true, Description = "Fall Speed", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 2)]
	[SerializeMember]
	public float MaximumAirborneSpeed
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
	/// Amount of force applied to the object each update. This controls how quickly the object reaches its maximum speed.
	/// </summary>
	[SerializeMember]
	[EditorProperty(true, Description = "Move Force", HorizontalAlignment = true, MajorGrouping = 2, MinorGrouping = 1, ToolTipText = "Amount of force applied to the object each update, controls how quickly the object reaches its maximum speed.")]
	public float MovementForce
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
	/// Player the object is controlled by.
	/// </summary>
	[EditorProperty(true, Description = "Player", HorizontalAlignment = true, MajorGrouping = 1, MinorGrouping = 1)]
	[SerializeMember]
	public PlayerIndex PlayerIndex
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
	/// Provides access to the PhysicsMoveCharacterController this component wraps.
	/// </summary>
	public PhysicsMoveCharacterController Controller => a5h;

	/// <summary>
	/// Creates a new CharacterControllerComponent instance.
	/// </summary>
	public CharacterControllerComponent()
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
	/// Called when the component is added to a parent object.
	/// </summary>
	public override void OnAddedToParentObject()
	{
		base.OnAddedToParentObject();
		if (base.ParentObject is ICollisionObject collisionObject)
		{
			a5h = new PhysicsMoveCharacterController(collisionObject);
			collisionObject.CollisionMove = a5h;
			bN();
		}
	}

	/// <summary>
	/// Called when the component is removed from a parent object.
	/// </summary>
	public override void OnRemovedFromParentObject()
	{
		base.OnRemovedFromParentObject();
		if (a5h != null)
		{
			a5h.Dispose();
			a5h = null;
		}
	}

	/// <summary>
	/// Event called when the parent object is updated during the game update loop.
	/// </summary>
	/// <param name="gametime"></param>
	public override void OnUpdate(GameTime gametime)
	{
		base.OnUpdate(gametime);
		bN();
	}

	private void bN()
	{
		if (a5h != null)
		{
			if (CrouchHeight == 0f)
			{
				CrouchHeight = a5h.CrouchHeight;
			}
			if (MaximumStepHeight == 0f)
			{
				MaximumStepHeight = a5h.MaximumStepHeight;
			}
			a5h.CrouchHeight = CrouchHeight;
			a5h.MovementSmoothing = MovementSmoothing;
			a5h.JumpSpeed = JumpSpeed;
			a5h.MaximumStepHeight = MaximumStepHeight;
			a5h.MaximumSpeed = MaximumSpeed;
			a5h.MaximumCrouchSpeed = MaximumCrouchSpeed;
			a5h.MaximumSlideSpeed = MaximumSlideSpeed;
			a5h.MaximumAirborneSpeed = MaximumAirborneSpeed;
			a5h.MovementForce = MovementForce;
			a5h.PlayerIndex = PlayerIndex;
		}
	}
}
