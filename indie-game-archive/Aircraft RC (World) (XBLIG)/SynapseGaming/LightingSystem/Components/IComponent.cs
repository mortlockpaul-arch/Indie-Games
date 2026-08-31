using System;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;

namespace SynapseGaming.LightingSystem.Components;

/// <summary>
/// Interface used by components that plug into other objects (also called object-level components).
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
public interface IComponent<T> : IEditorCreatedObject<IComponent<T>>, IEditorObject, INamedObject
{
	/// <summary>
	/// Type used to retrieve the component from the parent object. The component
	/// class must derive from or implement this type. This is generally the
	/// class type (eg: "this.GetType();");
	///
	/// Only one component of a specific type can be used by the parent object
	/// at a time. This allows components to replace one another and be used
	/// interchangeably by using the same type.
	/// </summary>
	Type ComponentType { get; }

	/// <summary>
	/// Parent object which the component controls or interacts with.
	/// </summary>
	T ParentObject { get; set; }

	/// <summary>
	/// Event called when the component's parent object is assigned or reassigned.
	/// </summary>
	void OnInitialize();

	/// <summary>
	/// Event called when components are added or removed from the parent object.
	/// </summary>
	void OnComponentsListChanged();

	/// <summary>
	/// Event called when another component issues a message to the parent object's components.
	/// </summary>
	/// <param name="sender">Sending component.</param>
	/// <param name="message">Message object.</param>
	void OnMessage(IComponent<T> sender, IComponentMessage message);

	/// <summary>
	/// Called when the component is added to a parent object.
	/// </summary>
	void OnAddedToParentObject();

	/// <summary>
	/// Called when the component is removed from a parent object.
	/// </summary>
	void OnRemovedFromParentObject();

	/// <summary>
	/// Called when the parent object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	void OnSubmittedToManager(IManagerService manager);

	/// <summary>
	/// Called when the parent object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	void OnRemovedFromManager(IManagerService manager);

	/// <summary>
	/// Event called when the parent object is updated during the game update loop.
	/// </summary>
	/// <param name="gametime"></param>
	void OnUpdate(GameTime gametime);

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
	void OnCollisionReact(IMovableObject collider, IMovableObject collidee, CollisionPoint worldcollisionpoint, ref bool collisionhandled);

	/// <summary>
	/// Event called when the parent object collides with another object, but only
	/// when the parent's CollisionType is set to Trigger.
	///
	/// The component can then apply custom trigger code like damage, apply force, and more.
	/// </summary>
	/// <param name="collider">The moving object.</param>
	/// <param name="trigger">The trigger hit by the moving object.</param>
	void OnCollisionTrigger(IMovableObject collider, IMovableObject trigger);
}
