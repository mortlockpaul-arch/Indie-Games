using System.Collections.Generic;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Indexes a collection of objects that implement IMovableObject and stores
/// them for fast retrieval by name, update type, or to access all objects at once.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectIndex<T> where T : IMovableObject
{
	private Dictionary<T, int> HCB = new Dictionary<T, int>(32);

	private Dictionary<T, int> HC_0002 = new Dictionary<T, int>(32);

	private Dictionary<string, T> HC_0012 = new Dictionary<string, T>(32);

	private Dictionary<string, T> HCH = new Dictionary<string, T>(32);

	private Dictionary<int, T> HC7 = new Dictionary<int, T>(32);

	/// <summary>
	/// Dictionary of all contained objects with an UpdateType of Automatic (eg: "dynamic" objects).
	/// </summary>
	public Dictionary<T, int> DynamicObjects => HCB;

	/// <summary>
	/// Dictionary of all contained objects.
	/// </summary>
	public Dictionary<T, int> AllObjects => HC_0002;

	/// <summary>
	/// Index by name of all contained objects with an UpdateType of Automatic (eg: "dynamic" objects).
	/// </summary>
	public Dictionary<string, T> DynamicObjectsByName => HC_0012;

	/// <summary>
	/// Index by name of all contained objects.
	/// </summary>
	public Dictionary<string, T> AllObjectsByName => HCH;

	/// <summary>
	/// Index by UniqueId of all contained objects.
	/// </summary>
	public Dictionary<int, T> AllObjectsByUniqueId => HC7;

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
		HC_0002[obj] = obj.MoveId;
		HCH[text] = obj;
		HC7[obj.UniqueId] = obj;
		if (obj.UpdateType == UpdateType.Automatic)
		{
			HCB[obj] = obj.MoveId;
			HC_0012[text] = obj;
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
		HC_0002.Remove(obj);
		HCB.Remove(obj);
		HCH.Remove(text);
		HC_0012.Remove(text);
		HC7.Remove(obj.UniqueId);
	}

	/// <summary>
	/// Remove all objects from the index.
	/// </summary>
	public void Clear()
	{
		HC_0002.Clear();
		HCB.Clear();
		HCH.Clear();
		HC_0012.Clear();
		HC7.Clear();
	}
}
