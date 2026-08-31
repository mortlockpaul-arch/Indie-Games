using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Rendering;
using Z;

namespace SynapseGaming.LightingSystem.Components;

/// <summary>
/// Base class used by all components.  Warning: this class does not support
/// serialization and should not be used to create custom components,
/// instead derive from either the BaseComponentAutoSerialization or
/// BaseComponentManualSerialization classes.
///
/// Provides built-in support for sending inter-component messages, calling
/// OnInitialize() when the parent object changes, and automatic ComponentType
/// using the derived class type.
/// </summary>
/// <typeparam name="T">Type of the parent class or interface
/// that contains the components. This strongly types the
/// components ensuring only components of the correct
/// type can be assigned.
///
/// For instance all classes and objects that derive from SceneEntity
/// use the ISceneEntity type allowing them to share components.
///
/// However lights use the ILight type to ensure entity components
/// cannot accidently be assigned to them, and vice versa.</typeparam>
[Serializable]
public class BaseComponent<T> : IComponent<T>, IEditorCreatedObject<IComponent<T>>, IEditorObject, INamedObject where T : class, IComponentObject<T>
{
	private T HCB;

	[CompilerGenerated]
	private bool HC_0002;

	[CompilerGenerated]
	private string HC_0012;

	/// <summary>
	/// Type used to retrieve the component from the parent object. The component
	/// class must derive from or implement this type. This is generally the
	/// class type (eg: "this.GetType();");
	///
	/// Only one component of a specific type can be used by the parent object
	/// at a time. This allows components to replace one another and be used
	/// interchangeably by using the same type.
	/// </summary>
	public virtual Type ComponentType => GetType();

	/// <summary>
	/// Notifies the editor that this object is partially controlled via code. The editor
	/// will display information to the user indicating some property values are
	/// overridden in code and changes may not take effect.
	/// </summary>
	[EditorProperty(false)]
	public bool AffectedInCode
	{
		[CompilerGenerated]
		get
		{
			return HC_0002;
		}
		[CompilerGenerated]
		set
		{
			HC_0002 = value;
		}
	}

	/// <summary>
	/// The object's current name.
	/// </summary>
	[EditorProperty(false)]
	public string Name
	{
		[CompilerGenerated]
		get
		{
			return HC_0012;
		}
		[CompilerGenerated]
		set
		{
			HC_0012 = value;
		}
	}

	/// <summary>
	/// Parent object which the component controls or interacts with.
	/// </summary>
	public T ParentObject
	{
		get
		{
			return HCB;
		}
		set
		{
			if (value == HCB)
			{
				return;
			}
			if (HCB != null)
			{
				if (HCB is ISceneEntity sceneEntity)
				{
					foreach (KeyValuePair<Type, IManagerService> item in sceneEntity.ContainingManagers.Items)
					{
						OnRemovedFromManager(item.Value);
					}
				}
				OnRemovedFromParentObject();
			}
			HCB = value;
			if (value == null)
			{
				return;
			}
			OnInitialize();
			OnAddedToParentObject();
			if (!(value is ISceneEntity sceneEntity2))
			{
				return;
			}
			foreach (KeyValuePair<Type, IManagerService> item2 in sceneEntity2.ContainingManagers.Items)
			{
				OnSubmittedToManager(item2.Value);
			}
		}
	}

	/// <summary>
	/// Deep clones the object including any contained sub-objects and components.
	/// </summary>
	/// <returns></returns>
	public virtual IComponent<T> Clone()
	{
		IComponent<T> component = Create();
		Z._7._0002w(this, component);
		return component;
	}

	/// <summary>
	/// Creates a new instance of the object type. This method assumes the type has a
	/// default constructor. If the type does not have a default constructor this method
	/// can be overridden to manually create the type.
	/// </summary>
	/// <returns></returns>
	protected virtual IComponent<T> Create()
	{
		return (IComponent<T>)Activator.CreateInstance(GetType());
	}

	/// <summary>
	/// Sends an inter-component message to the parent object's components.
	/// </summary>
	public void SendMessage()
	{
		SendMessage(null);
	}

	/// <summary>
	/// Sends an inter-component message to the parent object's components.
	/// </summary>
	/// <param name="message">Message data.</param>
	public void SendMessage(IComponentMessage message)
	{
		HCB.Components.SendMessage(this, message);
	}

	/// <summary>
	/// Event called when the component's parent object is assigned or reassigned.
	/// </summary>
	public virtual void OnInitialize()
	{
	}

	/// <summary>
	/// Event called when components are added or removed from the parent object.
	/// </summary>
	public virtual void OnComponentsListChanged()
	{
	}

	/// <summary>
	/// Event called when another component issues a message to the parent object's components.
	/// </summary>
	/// <param name="sender">Sending component.</param>
	/// <param name="message">Message object.</param>
	public virtual void OnMessage(IComponent<T> sender, IComponentMessage message)
	{
	}

	/// <summary>
	/// Called when the component is added to a parent object.
	/// </summary>
	public virtual void OnAddedToParentObject()
	{
	}

	/// <summary>
	/// Called when the component is removed from a parent object.
	/// </summary>
	public virtual void OnRemovedFromParentObject()
	{
	}

	/// <summary>
	/// Called when the component is created in the SunBurn editor.
	/// </summary>
	public virtual void OnCreatedInEditor()
	{
	}

	/// <summary>
	/// Called when the parent object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnSubmittedToManager(IManagerService manager)
	{
	}

	/// <summary>
	/// Called when the parent object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnRemovedFromManager(IManagerService manager)
	{
	}

	/// <summary>
	/// Event called when the parent object is updated during the game update loop.
	/// </summary>
	/// <param name="gametime"></param>
	public virtual void OnUpdate(GameTime gametime)
	{
	}

	/// <summary>
	/// Event called when the parent object collides with another object.
	/// </summary>
	/// <param name="collider">The moving object.</param>
	/// <param name="collidee">The object hit by the moving object.</param>
	/// <param name="worldcollisionpoint">Contains information about the closest collision point to the collider.</param>
	/// <param name="collisionhandled">Determines if the collision was handled by a prior component.
	/// If this value is true do NOT process any collision reaction code. If the component processes
	/// collision reaction code set this value to true to avoid another component or SunBurn's built-in
	/// reaction code from processing.</param>
	public virtual void OnCollisionReact(IMovableObject collider, IMovableObject collidee, CollisionPoint worldcollisionpoint, ref bool collisionhandled)
	{
	}

	/// <summary>
	/// Event called when the parent object collides with another object, but only
	/// when the parent's CollisionType is set to Trigger.
	///
	/// The component can then apply custom trigger code like damage, apply force, and more.
	/// </summary>
	/// <param name="collider">The moving object.</param>
	/// <param name="trigger">The trigger hit by the moving object.</param>
	public virtual void OnCollisionTrigger(IMovableObject collider, IMovableObject trigger)
	{
	}
}
