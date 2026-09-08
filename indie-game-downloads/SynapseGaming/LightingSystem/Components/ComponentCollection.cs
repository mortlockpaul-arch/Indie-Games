using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using _0017;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Serialization;

namespace SynapseGaming.LightingSystem.Components;

/// <summary>
/// Container that stores, manages, and updates object components.
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
public class ComponentCollection<T> : IGroup<IComponent<T>>, IFullSerializable, ISerializable where T : class
{
	private T _3A_0018;

	private List<IComponent<T>> _3AL = new List<IComponent<T>>();

	private Dictionary<Type, IComponent<T>> _3A_0019 = new Dictionary<Type, IComponent<T>>();

	private IList<IComponent<T>> _3A3;

	/// <summary>
	/// Read only list of all contained components.
	/// </summary>
	public IList<IComponent<T>> Components => _3A3;

	/// <summary>
	/// Parent object which the components control and interact with.
	/// </summary>
	public T ParentObject
	{
		get
		{
			return _3A_0018;
		}
		set
		{
			_3A_0018 = value;
			f();
		}
	}

	/// <summary>
	/// Creates a new ComponentCollection instance.
	/// </summary>
	public ComponentCollection()
		: this((T)null)
	{
	}

	/// <summary>
	/// Creates a new ComponentCollection instance.
	/// </summary>
	/// <param name="parentobject">Parent object which the components
	/// control and interact with.</param>
	public ComponentCollection(T parentobject)
	{
		_3A3 = _3AL.AsReadOnly();
		_3A_0018 = parentobject;
	}

	private void f()
	{
		foreach (IComponent<T> item in _3AL)
		{
			item.ParentObject = _3A_0018;
		}
	}

	private void _1()
	{
		_3A_0019.Clear();
		foreach (IComponent<T> item in _3AL)
		{
			_3A_0019.Add(item.ComponentType, item);
		}
		foreach (IComponent<T> item2 in _3AL)
		{
			item2.OnComponentsListChanged();
		}
	}

	/// <summary>
	/// Adds a component that will control or interact with the parent
	/// object. Also adds any dependent components not in the container.
	/// </summary>
	/// <param name="component"></param>
	public void AddWithDependencies(IComponent<T> component)
	{
		List<Type> dependencies = ComponentDependencyCache.GetDependencies(component.GetType());
		for (int num = dependencies.Count - 1; num >= 0; num--)
		{
			Type type = dependencies[num];
			if (!_3A_0019.ContainsKey(type) && Activator.CreateInstance(type, new object[1] { true }) is IComponent<T> component2 && !_3A_0019.ContainsKey(component2.ComponentType))
			{
				Add(component2);
			}
		}
		Add(component);
	}

	/// <summary>
	/// Adds a component that will control or interact with the parent object.
	/// </summary>
	/// <param name="component"></param>
	public void Add(IComponent<T> component)
	{
		G(component.ComponentType);
		_3AL.Add(component);
		_1();
		component.ParentObject = _3A_0018;
	}

	/// <summary>
	/// Inserts a component at the specified index that will control or interact with the parent object.
	/// </summary>
	/// <param name="index"></param>
	/// <param name="component"></param>
	public void Insert(int index, IComponent<T> component)
	{
		G(component.ComponentType);
		_3AL.Insert(index, component);
		_1();
		component.ParentObject = _3A_0018;
	}

	/// <summary>
	/// Inserts a component after a component of the specified type,
	/// that will control or interact with the parent object.
	///
	/// Used to ensure a component of the specified type will be processed
	/// before the inserted component.
	/// </summary>
	/// <param name="component"></param>
	/// <param name="othercomponenttype">Component type to insert the component after.</param>
	public void InsertAfter(IComponent<T> component, Type othercomponenttype)
	{
		G(component.ComponentType);
		int count = _3AL.Count;
		int num = count;
		for (int i = 0; i < count; i++)
		{
			IComponent<T> component2 = _3AL[i];
			if ((object)component2.ComponentType == othercomponenttype)
			{
				num = i + 1;
				break;
			}
		}
		if (num <= count)
		{
			_3AL.Insert(num, component);
		}
		else
		{
			_3AL.Add(component);
		}
		_1();
		component.ParentObject = _3A_0018;
	}

	/// <summary>
	/// Clears all components from the container.
	/// </summary>
	public void Clear()
	{
		foreach (IComponent<T> item in _3AL)
		{
			item.ParentObject = null;
		}
		_3AL.Clear();
		_1();
	}

	/// <summary>
	/// Removes the component at the specified index from the container.
	/// </summary>
	/// <param name="index"></param>
	public void RemoveAt(int index)
	{
		_3AL[index].ParentObject = null;
		_3AL.RemoveAt(index);
		_1();
	}

	/// <summary>
	/// Removes a component of the specified type from the container.
	///
	/// Must be the same type provided in IComponent.ComponentType.
	/// </summary>
	/// <typeparam name="TComponentType"></typeparam>
	public void Remove<TComponentType>()
	{
		Remove(typeof(TComponentType));
	}

	/// <summary>
	/// Removes a specific component from the container.
	/// </summary>
	/// <param name="component"></param>
	public void Remove(IComponent<T> component)
	{
		Remove(component.ComponentType);
	}

	/// <summary>
	/// Removes a component of the specified type from the container.
	///
	/// Must be the same type provided in IComponent.ComponentType.
	/// </summary>
	/// <param name="componenttype"></param>
	public void Remove(Type componenttype)
	{
		if (G(componenttype))
		{
			_1();
		}
	}

	private bool G(Type P_0)
	{
		IComponent<T> component = GetComponent(P_0, required: false);
		if (component == null)
		{
			return false;
		}
		component.ParentObject = null;
		_3AL.Remove(component);
		return true;
	}

	/// <summary>
	/// Gets a component of the specified type from the container.
	///
	/// Must be the same type provided in IComponent.ComponentType.
	/// </summary>
	/// <typeparam name="TComponentType"></typeparam>
	/// <param name="required">Determines if the component is required.
	/// If it is and the component doesn't exist the method will throw an exception.</param>
	/// <returns></returns>
	public TComponentType GetComponent<TComponentType>(bool required)
	{
		Type typeFromHandle = typeof(TComponentType);
		TComponentType val = (TComponentType)GetComponent(typeFromHandle, required);
		if (val == null && required)
		{
			throw new Exception("Component container does not contain a component assigned to the '" + typeFromHandle.Name + "' type.");
		}
		return val;
	}

	/// <summary>
	/// Gets a component of the specified type from the container.
	///
	/// Must be the same type provided in IComponent.ComponentType.
	/// </summary>
	/// <param name="componenttype"></param>
	/// <param name="required">Determines if the component is required.
	/// If it is and the component doesn't exist the method will throw an exception.</param>
	/// <returns></returns>
	public IComponent<T> GetComponent(Type componenttype, bool required)
	{
		if (_3A_0019.TryGetValue(componenttype, out var value))
		{
			return value;
		}
		if (!required)
		{
			return null;
		}
		throw new Exception("Component container does not contain a component assigned to the '" + componenttype.Name + "' type.");
	}

	/// <summary>
	/// Sends an inter-component message to the parent object's components.
	/// </summary>
	/// <param name="sender">Sender component.</param>
	/// <param name="message">Message data.</param>
	public void SendMessage(IComponent<T> sender, IComponentMessage message)
	{
		int count = _3AL.Count;
		for (int i = 0; i < count; i++)
		{
			IComponent<T> component = _3AL[i];
			if (component != sender)
			{
				component.OnMessage(sender, message);
			}
		}
	}

	/// <summary>
	/// Called when the parent object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	public void OnSubmittedToManager(IManagerService manager)
	{
		foreach (IComponent<T> item in _3AL)
		{
			item.OnSubmittedToManager(manager);
		}
	}

	/// <summary>
	/// Called when the parent object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public void OnRemovedFromManager(IManagerService manager)
	{
		foreach (IComponent<T> item in _3AL)
		{
			item.OnRemovedFromManager(manager);
		}
	}

	/// <summary>
	/// Event called when the parent object is updated during the game update loop.
	/// </summary>
	/// <param name="gametime"></param>
	public void OnUpdate(GameTime gametime)
	{
		int count = _3AL.Count;
		for (int i = 0; i < count; i++)
		{
			_3AL[i].OnUpdate(gametime);
		}
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
	public void OnCollisionReact(IMovableObject collider, IMovableObject collidee, CollisionPoint worldcollisionpoint, ref bool collisionhandled)
	{
		int count = _3AL.Count;
		for (int i = 0; i < count; i++)
		{
			_3AL[i].OnCollisionReact(collider, collidee, worldcollisionpoint, ref collisionhandled);
		}
	}

	/// <summary>
	/// Event called when the parent object collides with another object, but only
	/// when the parent's CollisionType is set to Trigger.
	///
	/// The component can then apply custom trigger code like damage, apply force, and more.
	/// </summary>
	/// <param name="collider">The moving object.</param>
	/// <param name="trigger">The trigger hit by the moving object.</param>
	public void OnCollisionTrigger(IMovableObject collider, IMovableObject trigger)
	{
		int count = _3AL.Count;
		for (int i = 0; i < count; i++)
		{
			_3AL[i].OnCollisionTrigger(collider, trigger);
		}
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		_3AL.Clear();
		List<IComponent<T>> field = null;
		SerializationHelper.DeserializeField(ref field, info, _0017._0018._3A_0018, usedefault: false);
		if (field != null)
		{
			_3AL.AddRange(field);
		}
		_1();
		f();
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue(_0017._0018._3A_0018, _3AL);
	}
}
