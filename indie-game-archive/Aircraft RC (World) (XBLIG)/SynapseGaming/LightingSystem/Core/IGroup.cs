namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface used by groups that contain other objects.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGroup<T>
{
	/// <summary>
	/// Adds an object to the group.
	/// </summary>
	/// <param name="obj"></param>
	void Add(T obj);

	/// <summary>
	/// Removes an object to the group.
	/// </summary>
	/// <param name="obj"></param>
	void Remove(T obj);

	/// <summary>
	/// Removes the object at a specific index.
	/// </summary>
	/// <param name="index"></param>
	void RemoveAt(int index);

	/// <summary>
	/// Removes all objects from the group.
	/// </summary>
	void Clear();
}
