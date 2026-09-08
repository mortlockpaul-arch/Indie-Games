using System.Collections.Generic;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Indexes a collection of objects that implement IMovableObject and stores
/// them for fast retrieval by name, update type, or to access all objects at once.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectIndex<T> where T : IMovableObject
{
	private Dictionary<T, int> _3A_0018 = new Dictionary<T, int>(32);

	private Dictionary<T, int> _3AL = new Dictionary<T, int>(32);

	private Dictionary<string, T> _3A_0019 = new Dictionary<string, T>(32);

	private Dictionary<string, T> _3A3 = new Dictionary<string, T>(32);

	private Dictionary<int, T> _3A6 = new Dictionary<int, T>(32);

	/// <summary>
	/// Dictionary of all contained objects with an UpdateType of Automatic (eg: "dynamic" objects).
	/// </summary>
	public Dictionary<T, int> DynamicObjects => _3A_0018;

	/// <summary>
	/// Dictionary of all contained objects.
	/// </summary>
	public Dictionary<T, int> AllObjects => _3AL;

	/// <summary>
	/// Index by name of all contained objects with an UpdateType of Automatic (eg: "dynamic" objects).
	/// </summary>
	public Dictionary<string, T> DynamicObjectsByName => _3A_0019;

	/// <summary>
	/// Index by name of all contained objects.
	/// </summary>
	public Dictionary<string, T> AllObjectsByName => _3A3;

	/// <summary>
	/// Index by UniqueId of all contained objects.
	/// </summary>
	public Dictionary<int, T> AllObjectsByUniqueId => _3A6;

	/// <summary>
	/// Add an object to the index.
	/// </summary>
	/// <param name="obj"></param>
	public void Add(T obj)
	{
		string text = obj.Name;
		if (text == null)
		{
			text = string.Empty;
		}
		_3AL[obj] = obj.MoveId;
		_3A3[text] = obj;
		_3A6[obj.UniqueId] = obj;
		if (obj.UpdateType == UpdateType.Automatic)
		{
			_3A_0018[obj] = obj.MoveId;
			_3A_0019[text] = obj;
		}
	}

	/// <summary>
	/// Remove an object from the index.
	/// </summary>
	/// <param name="obj"></param>
	public void Remove(T obj)
	{
		string text = obj.Name;
		if (text == null)
		{
			text = string.Empty;
		}
		_3AL.Remove(obj);
		_3A_0018.Remove(obj);
		_3A3.Remove(text);
		_3A_0019.Remove(text);
		_3A6.Remove(obj.UniqueId);
	}

	/// <summary>
	/// Remove all objects from the index.
	/// </summary>
	public void Clear()
	{
		_3AL.Clear();
		_3A_0018.Clear();
		_3A3.Clear();
		_3A_0019.Clear();
		_3A6.Clear();
	}
}
