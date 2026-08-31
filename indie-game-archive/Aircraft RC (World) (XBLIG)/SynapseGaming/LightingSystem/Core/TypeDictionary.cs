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
	private Dictionary<Type, T> HCB = new Dictionary<Type, T>();

	/// <summary>
	/// Access to all objects added to the dictionary.
	/// </summary>
	public Dictionary<Type, T> Items => HCB;

	/// <summary>
	/// Add object to the dictionary.
	/// </summary>
	/// <param name="type">Key type the object is associated with.</param>
	/// <param name="item">The object to store.</param>
	public void Add(Type type, T item)
	{
		HCB[type] = item;
	}

	/// <summary>
	/// Remove object from the dictionary.
	/// </summary>
	/// <param name="type">Key type the object is associated with.</param>
	public void Remove(Type type)
	{
		HCB.Remove(type);
	}

	/// <summary>
	/// Get object from the dictionary.
	/// </summary>
	/// <param name="type">Key type the object is associated with.</param>
	public T GetItem(Type type)
	{
		if (HCB.TryGetValue(type, out var value))
		{
			return value;
		}
		return default(T);
	}
}
