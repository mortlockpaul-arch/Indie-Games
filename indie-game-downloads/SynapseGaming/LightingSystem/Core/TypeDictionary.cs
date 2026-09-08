using System;
using System.Collections.Generic;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Dictionary used to store objects by a key type. The key type can either be the
/// object's type or another specified type that the object will be associated with.
///
/// This class is functionally similar to Dictionary{Type, T}.
/// </summary>
/// <typeparam name="T"></typeparam>
public class TypeDictionary<T>
{
	private Dictionary<Type, T> _3A_0018 = new Dictionary<Type, T>();

	/// <summary>
	/// Access to all objects added to the dictionary.
	/// </summary>
	public Dictionary<Type, T> Items => _3A_0018;

	/// <summary>
	/// Add object to the dictionary.
	/// </summary>
	/// <param name="type">Key type the object is associated with.</param>
	/// <param name="item">The object to store.</param>
	public void Add(Type type, T item)
	{
		_3A_0018[type] = item;
	}

	/// <summary>
	/// Remove object from the dictionary.
	/// </summary>
	/// <param name="type">Key type the object is associated with.</param>
	public void Remove(Type type)
	{
		_3A_0018.Remove(type);
	}

	/// <summary>
	/// Get object from the dictionary.
	/// </summary>
	/// <param name="type">Key type the object is associated with.</param>
	public T GetItem(Type type)
	{
		if (_3A_0018.TryGetValue(type, out var value))
		{
			return value;
		}
		return default(T);
	}
}
