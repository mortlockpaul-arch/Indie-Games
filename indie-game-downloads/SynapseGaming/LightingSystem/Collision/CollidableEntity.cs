using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Serialization;

namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Entity that provides collision and control over size (width, height, and depth)
/// without the overhead of being renderable.
/// </summary>
[Serializable]
[EditorCreatedObject]
[EditorObject(true)]
public class CollidableEntity : SceneEntity, ICollisionObject, ISceneEntity, IMovableObject, IWorldBoundingBoxObject, IComponentObject<ISceneEntity>, IEditorCreatedObject<ISceneEntity>, IEditorObject, INamedObject, IEditorRenderableObject, ICollisionMaterial
{
	private CollisionReactDelegate _3A_0018;

	private CollisionTriggerDelegate _3AL;

	private bool _3A_0019;

	private float _3A3;

	private float _3A6;

	private float _3AD;

	private float _3A_0017;

	private float _3A_0003;

	private float _3Al;

	private CollisionType _3At;

	private ICollisionMove _3AF;

	private static Vector3[] _3Ac = new Vector3[8];

	[CompilerGenerated]
	private ICollisionMaterial _3Ag;

	/// <summary>
	/// Determines the entity width.
	/// </summary>
	[EditorProperty(true, HorizontalAlignment = true, MajorGrouping = 6, MinorGrouping = 1)]
	public float Width
	{
		get
		{
			return _3A3;
		}
		set
		{
			_3A3 = value;
			CalculateBounds();
		}
	}

	/// <summary>
	/// Determines the entity height.
	/// </summary>
	[EditorProperty(true, HorizontalAlignment = true, MajorGrouping = 6, MinorGrouping = 2)]
	public float Height
	{
		get
		{
			return _3A6;
		}
		set
		{
			_3A6 = value;
			CalculateBounds();
		}
	}

	/// <summary>
	/// Determines the entity depth.
	/// </summary>
	[EditorProperty(true, HorizontalAlignment = true, MajorGrouping = 6, MinorGrouping = 3)]
	public float Depth
	{
		get
		{
			return _3AD;
		}
		set
		{
			_3AD = value;
			CalculateBounds();
		}
	}

	/// <summary>
	/// Amount material absorbs impact force.
	/// </summary>
	[EditorProperty(true, HorizontalAlignment = true, MajorGrouping = 7, MinorGrouping = 3)]
	public float Elasticity
	{
		get
		{
			return _3A_0003;
		}
		set
		{
			_3A_0003 = value;
			_CollisionId++;
		}
	}

	/// <summary>
	/// Amount material resists objects moving across its surface.
	/// </summary>
	[EditorProperty(true, HorizontalAlignment = true, MajorGrouping = 7, MinorGrouping = 2)]
	public float Friction
	{
		get
		{
			return _3A_0017;
		}
		set
		{
			_3A_0017 = value;
			_CollisionId++;
		}
	}

	/// <summary>
	/// Determines if gravity will cause the object to fall. For an object to be affected
	/// by gravity its UpdateType must be Automatic and CollisionType must be Collide.
	/// </summary>
	[EditorProperty(true, Description = "Affected By Gravity", MajorGrouping = 5, MinorGrouping = 1)]
	public bool AffectedByGravity
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
			_CollisionId++;
			if (!_3A_0019 && _3AF != null)
			{
				_3AF.RemoveForces();
			}
		}
	}

	/// <summary>
	/// Indicates if collision related properties changed. This value increments each time the object
	/// collision properties change.
	/// </summary>
	public int CollisionId
	{
		get
		{
			return _CollisionId;
		}
		set
		{
			_CollisionId = value;
		}
	}

	/// <summary>
	/// Move helper used by this object to determine its momentum, next location, and sweep volume.
	/// </summary>
	public ICollisionMove CollisionMove
	{
		get
		{
			return _3AF;
		}
		set
		{
			if (value == _3AF)
			{
				return;
			}
			if (_3AF != null)
			{
				foreach (KeyValuePair<Type, IManagerService> item in base.ContainingManagers.Items)
				{
					_3AF.OnRemovedFromManager(item.Value);
				}
			}
			_3AF = value;
			if (value == null)
			{
				return;
			}
			foreach (KeyValuePair<Type, IManagerService> item2 in base.ContainingManagers.Items)
			{
				value.OnSubmittedToManager(item2.Value);
			}
		}
	}

	/// <summary>
	/// Determines how an object interacts with the scene.
	/// </summary>
	[EditorProperty(true, Description = "Collision Type", HorizontalAlignment = true, MajorGrouping = 5, MinorGrouping = 3)]
	public CollisionType CollisionType
	{
		get
		{
			return _3At;
		}
		set
		{
			_3At = value;
			_CollisionId++;
		}
	}

	/// <summary>
	/// Default material used when collision surface does not implement material info.
	/// </summary>
	public ICollisionMaterial DefaultCollisionMaterial
	{
		[CompilerGenerated]
		get
		{
			return _3Ag;
		}
		[CompilerGenerated]
		set
		{
			_3Ag = value;
		}
	}

	/// <summary>
	/// Mass of the object.
	/// </summary>
	[EditorProperty(true, Description = "Mass", HorizontalAlignment = true, MajorGrouping = 7, MinorGrouping = 1)]
	[EditorNumberPadOptions(3, 0.001, 10000.0, 0.1)]
	public float Mass
	{
		get
		{
			return _3Al;
		}
		set
		{
			_3Al = value;
			_CollisionId++;
		}
	}

	/// <summary>
	/// Event used to detect when the object collides with another object, or to
	/// override the default reaction behavior between objects.
	/// </summary>
	public event CollisionReactDelegate CollisionReactEvent
	{
		add
		{
			CollisionReactDelegate collisionReactDelegate = _3A_0018;
			CollisionReactDelegate collisionReactDelegate2;
			do
			{
				collisionReactDelegate2 = collisionReactDelegate;
				CollisionReactDelegate value2 = (CollisionReactDelegate)Delegate.Combine(collisionReactDelegate2, value);
				collisionReactDelegate = Interlocked.CompareExchange(ref _3A_0018, value2, collisionReactDelegate2);
			}
			while ((object)collisionReactDelegate != collisionReactDelegate2);
		}
		remove
		{
			CollisionReactDelegate collisionReactDelegate = _3A_0018;
			CollisionReactDelegate collisionReactDelegate2;
			do
			{
				collisionReactDelegate2 = collisionReactDelegate;
				CollisionReactDelegate value2 = (CollisionReactDelegate)Delegate.Remove(collisionReactDelegate2, value);
				collisionReactDelegate = Interlocked.CompareExchange(ref _3A_0018, value2, collisionReactDelegate2);
			}
			while ((object)collisionReactDelegate != collisionReactDelegate2);
		}
	}

	/// <summary>
	/// Event used to detect when another object collides with this object, but only
	/// when this object's CollisionType is set to Trigger.
	///
	/// The event handler can then apply custom trigger code like damage, apply force, and more.
	/// </summary>
	public event CollisionTriggerDelegate CollisionTriggerEvent
	{
		add
		{
			CollisionTriggerDelegate collisionTriggerDelegate = _3AL;
			CollisionTriggerDelegate collisionTriggerDelegate2;
			do
			{
				collisionTriggerDelegate2 = collisionTriggerDelegate;
				CollisionTriggerDelegate value2 = (CollisionTriggerDelegate)Delegate.Combine(collisionTriggerDelegate2, value);
				collisionTriggerDelegate = Interlocked.CompareExchange(ref _3AL, value2, collisionTriggerDelegate2);
			}
			while ((object)collisionTriggerDelegate != collisionTriggerDelegate2);
		}
		remove
		{
			CollisionTriggerDelegate collisionTriggerDelegate = _3AL;
			CollisionTriggerDelegate collisionTriggerDelegate2;
			do
			{
				collisionTriggerDelegate2 = collisionTriggerDelegate;
				CollisionTriggerDelegate value2 = (CollisionTriggerDelegate)Delegate.Remove(collisionTriggerDelegate2, value);
				collisionTriggerDelegate = Interlocked.CompareExchange(ref _3AL, value2, collisionTriggerDelegate2);
			}
			while ((object)collisionTriggerDelegate != collisionTriggerDelegate2);
		}
	}

	/// <summary>
	/// Creates a new CollidableEntity instance.
	/// </summary>
	public CollidableEntity()
	{
		CollisionType = CollisionType.Collide;
		Mass = 0f;
		AffectedByGravity = false;
		DefaultCollisionMaterial = this;
		Elasticity = 0.5f;
		Friction = 0.25f;
		Width = 1f;
		Height = 1f;
		Depth = 1f;
	}

	/// <summary>
	/// Calculates the object bounds.
	/// </summary>
	/// <param name="objectboundingbox">Object bounds to update.</param>
	/// <param name="objectboundingsphere">Object bounds to update.</param>
	protected override void CalculateObjectBounds(ref BoundingBox objectboundingbox, ref BoundingSphere objectboundingsphere)
	{
		Vector3 vector = new Vector3(_3A3 * 0.5f, _3A6 * 0.5f, _3AD * 0.5f);
		objectboundingbox = new BoundingBox(-vector, vector);
		BoundingSphere.CreateFromBoundingBox(ref objectboundingbox, out objectboundingsphere);
	}

	/// <summary>
	/// Called when the object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	public override void OnSubmittedToManager(IManagerService manager)
	{
		base.OnSubmittedToManager(manager);
		if (_3AF != null)
		{
			_3AF.OnSubmittedToManager(manager);
		}
	}

	/// <summary>
	/// Called when the object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public override void OnRemovedFromManager(IManagerService manager)
	{
		base.OnRemovedFromManager(manager);
		if (_3AF != null)
		{
			_3AF.OnRemovedFromManager(manager);
		}
	}

	/// <summary>
	/// Used to trigger the CollisionReactEvent event when two objects collide.
	/// </summary>
	/// <param name="collider">The moving object.</param>
	/// <param name="collidee">The object hit by the moving object.</param>
	/// <param name="worldcollisionpoint">Contains information about the closest collision point to the collider.</param>
	/// <param name="collisionhandled">Determines if the collision was handled by a prior event hander.
	/// If this value is true do NOT process any collision reaction code. If the event handler processes
	/// collision reaction code set this value to true to avoid another handler or SunBurn's built-in
	/// reaction code from processing.</param>
	public void OnCollisionReact(IMovableObject collider, IMovableObject collidee, CollisionPoint worldcollisionpoint, ref bool collisionhandled)
	{
		if (_3A_0018 != null)
		{
			_3A_0018(collider, collidee, worldcollisionpoint, ref collisionhandled);
		}
		base.Components.OnCollisionReact(collider, collidee, worldcollisionpoint, ref collisionhandled);
	}

	/// <summary>
	/// Used to trigger the CollisionTriggerEvent event when an object passes through or overlaps a trigger.
	/// </summary>
	/// <param name="collider">The moving object.</param>
	/// <param name="trigger">The trigger hit by the moving object.</param>
	public void OnCollisionTrigger(IMovableObject collider, IMovableObject trigger)
	{
		if (_3AL != null)
		{
			_3AL(collider, trigger);
		}
		base.Components.OnCollisionTrigger(collider, trigger);
	}

	/// <summary>
	/// Implements rendering of in-editor icons and helpers.
	///
	/// This method is called twice per-frame: once with scene depth clipping enable, and once with it disabled.
	/// </summary>
	/// <param name="scenestate">Current state used to render the scene.</param>
	/// <param name="renderhelper">Helper used to draw lines associated with the object. Only calling Submit() is
	/// supported in this method, using other methods may affect rendering of lines drawn by other objects.</param>
	/// <param name="highlighted">Indicates if the object is currently highlighted by the editor.</param>
	/// <param name="selected">Indicates if the object is currently selected by the editor.</param>
	/// <param name="sceneoccludedpass">Indicates if the current rendering pass depth clips with the scene.
	/// If so rendered icons and helpers are occluded by scene objects.</param>
	public override void RenderEditorIcon(ISceneState scenestate, BoundingBoxRenderHelper renderhelper, bool highlighted, bool selected, bool sceneoccludedpass)
	{
		base.RenderEditorIcon(scenestate, renderhelper, highlighted, selected, sceneoccludedpass);
		if (sceneoccludedpass)
		{
			Matrix world = base.World;
			base.ObjectBoundingBox.GetCorners(_3Ac);
			for (int i = 0; i < 8; i++)
			{
				ref Vector3 reference = ref _3Ac[i];
				reference = Vector3.Transform(_3Ac[i], world);
			}
			Color mediumSlateBlue = Color.MediumSlateBlue;
			renderhelper.Submit(_3Ac[0], _3Ac[4], mediumSlateBlue);
			renderhelper.Submit(_3Ac[1], _3Ac[5], mediumSlateBlue);
			renderhelper.Submit(_3Ac[2], _3Ac[6], mediumSlateBlue);
			renderhelper.Submit(_3Ac[3], _3Ac[7], mediumSlateBlue);
			renderhelper.Submit(_3Ac[0], _3Ac[1], mediumSlateBlue);
			renderhelper.Submit(_3Ac[1], _3Ac[2], mediumSlateBlue);
			renderhelper.Submit(_3Ac[2], _3Ac[3], mediumSlateBlue);
			renderhelper.Submit(_3Ac[3], _3Ac[0], mediumSlateBlue);
			renderhelper.Submit(_3Ac[4], _3Ac[5], mediumSlateBlue);
			renderhelper.Submit(_3Ac[5], _3Ac[6], mediumSlateBlue);
			renderhelper.Submit(_3Ac[6], _3Ac[7], mediumSlateBlue);
			renderhelper.Submit(_3Ac[7], _3Ac[4], mediumSlateBlue);
		}
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public override void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		SerializationHelper.DeserializeField(ref _3A3, info, "Width", usedefault: true);
		SerializationHelper.DeserializeField(ref _3A6, info, "Height", usedefault: true);
		SerializationHelper.DeserializeField(ref _3AD, info, "Depth", usedefault: true);
		SerializationHelper.DeserializeField(ref _3A_0003, info, "Elasticity", usedefault: true);
		SerializationHelper.DeserializeField(ref _3A_0017, info, "Friction", usedefault: true);
		SerializationHelper.DeserializeField(ref _3Al, info, "Mass", usedefault: true);
		SerializationHelper.DeserializeField(ref _3A_0019, info, "AffectedByGravity", usedefault: true);
		SerializationHelper.DeserializeEnum(ref _3At, info, "CollisionType", isflag: false);
		_CollisionId++;
		CalculateBounds();
		base.SetObjectData(info, context);
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		SerializationHelper.SerializeFieldOrEnum(ref _3A3, info, "Width");
		SerializationHelper.SerializeFieldOrEnum(ref _3A6, info, "Height");
		SerializationHelper.SerializeFieldOrEnum(ref _3AD, info, "Depth");
		SerializationHelper.SerializeFieldOrEnum(ref _3A_0003, info, "Elasticity");
		SerializationHelper.SerializeFieldOrEnum(ref _3A_0017, info, "Friction");
		SerializationHelper.SerializeFieldOrEnum(ref _3Al, info, "Mass");
		SerializationHelper.SerializeFieldOrEnum(ref _3A_0019, info, "AffectedByGravity");
		SerializationHelper.SerializeFieldOrEnum(ref _3At, info, "CollisionType");
	}
}
