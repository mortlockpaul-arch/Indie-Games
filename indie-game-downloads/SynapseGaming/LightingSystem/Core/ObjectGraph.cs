using System;
using System.Collections.Generic;
using _0003;
using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Acts as a storage tree / scenegraph for objects of a particular
/// class or interface.  Supports object adding, moving, and removing
/// as well as auto-detecting movement of dynamic objects using the
/// MoveDynamicObjects method.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectGraph<T> : IQuery<T>, ISubmit<T> where T : IMovableObject
{
	private class DA_0018
	{
		public SystemStatistic ObjectsSubmitted = SystemConsole.GetStatistic("SceneGraph_ObjectsSubmitted", SystemStatisticCategory.SceneGraph);

		public SystemStatistic ObjectsMoved = SystemConsole.GetStatistic("SceneGraph_ObjectsMoved", SystemStatisticCategory.SceneGraph);

		public SystemStatistic ObjectsMovedDynamic = SystemConsole.GetStatistic("SceneGraph_ObjectsMovedDynamic", SystemStatisticCategory.SceneGraph);

		public SystemStatistic ObjectsRemoved = SystemConsole.GetStatistic("SceneGraph_ObjectsRemoved", SystemStatisticCategory.SceneGraph);

		public SystemStatistic ObjectsRetrieved = SystemConsole.GetStatistic("SceneGraph_ObjectsRetrieved", SystemStatisticCategory.SceneGraph);

		public SystemStatistic Optimized = SystemConsole.GetStatistic("SceneGraph_Optimized", SystemStatisticCategory.SceneGraph);
	}

	private BoundingBox _3A_0018;

	private int _3AL = 20;

	private bool _3A_0019 = true;

	private int _3A3;

	private ObjectIndex<T> _3A6 = new ObjectIndex<T>();

	private _0003.L<T> _3AD = new _0003.L<T>();

	private Vector3[] _3A_0017 = new Vector3[8];

	private List<T> _3A_0003 = new List<T>(32);

	private List<T> _3Al = new List<T>(32);

	private DA_0018 _3At = new DA_0018();

	/// <summary>
	/// The current containment volume for this object.
	/// </summary>
	public BoundingBox WorldBoundingBox => _3A_0018;

	/// <summary>
	/// Enables automatic optimizations on the tree used to store contained objects. 
	/// Optimization occurs when a large number of objects fall outside of the tree bounds.
	/// </summary>
	public bool AutoOptimize
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
		}
	}

	/// <summary>
	/// Determines if the tree used to store contained objects requires optimization.
	/// </summary>
	public bool RequiresOptimization
	{
		get
		{
			if (_3A3 < 10)
			{
				_3A3++;
				return false;
			}
			_3A3 = 0;
			return _3AD.H();
		}
	}

	/// <summary>
	/// Index of all objects contained in the manager / object graph.
	/// </summary>
	protected ObjectIndex<T> ObjectIndex => _3A6;

	/// <summary>
	/// Creates a new ObjectGraph using the default world size and tree depth.
	/// </summary>
	public ObjectGraph()
	{
		_3A_0018 = new BoundingBox(new Vector3(-1000f, -1000f, -1000f), new Vector3(1000f, 1000f, 1000f));
		Resize(_3A_0018, _3AL);
	}

	/// <summary>
	/// Creates a new ObjectGraph using the provided world size and tree depth.
	/// </summary>
	/// <param name="worldboundingbox">The smallest bounding area that completely
	/// contains the scene. Helps build an optimal scene tree.</param>
	/// <param name="worldtreemaxdepth">Maximum depth for entries in the scene tree. Small
	/// scenes with few objects see better performance with shallow trees. Large complex
	/// scenes often need deeper trees.</param>
	public ObjectGraph(BoundingBox worldboundingbox, int worldtreemaxdepth)
	{
		Resize(worldboundingbox, worldtreemaxdepth);
	}

	/// <summary>
	/// Resizes the tree used to store contained objects.
	/// </summary>
	/// <param name="worldboundingbox">The smallest bounding area that completely
	/// contains the scene. Helps the ObjectGraph build an optimal scene tree.</param>
	/// <param name="worldtreemaxdepth">Maximum depth for entries in the scene tree. Small
	/// scenes with few objects see better performance with shallow trees. Large complex
	/// scenes often need deeper trees.</param>
	public virtual void Resize(BoundingBox worldboundingbox, int worldtreemaxdepth)
	{
		_3A_0018 = worldboundingbox;
		_3AL = worldtreemaxdepth;
		_3AD._0018(ref worldboundingbox, worldtreemaxdepth);
	}

	/// <summary>
	/// Optimizes the tree used to store contained objects.
	/// </summary>
	public virtual void Optimize()
	{
		Optimize(0);
	}

	/// <summary>
	/// Optimizes the tree used to store contained objects using a fixed tree depth.
	/// </summary>
	/// <param name="worldtreemaxdepth">Fixed tree depth used to optimize the tree.</param>
	public virtual void Optimize(int worldtreemaxdepth)
	{
		BoundingBox boundingBox = default(BoundingBox);
		Dictionary<T, int> allObjects = _3A6.AllObjects;
		bool flag = false;
		foreach (KeyValuePair<T, int> item in allObjects)
		{
			T key = item.Key;
			if (!key.InfiniteBounds)
			{
				boundingBox = BoundingBox.CreateMerged(boundingBox, key.WorldBoundingBox);
				flag = true;
			}
		}
		if (!flag)
		{
			boundingBox = new BoundingBox(new Vector3(float.MinValue, float.MinValue, float.MinValue), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
		}
		int num = worldtreemaxdepth;
		if (num <= 0)
		{
			num = Math.Max(1, allObjects.Count / 20);
		}
		Resize(boundingBox, num);
		foreach (KeyValuePair<T, int> item2 in allObjects)
		{
			T key2 = item2.Key;
			_3AD.R(key2.WorldBoundingBox, key2);
		}
		_3At.Optimized.AccumulationValue++;
	}

	/// <summary>
	/// Adds an object to the container. This does not transfer ownership, disposable
	/// objects should be maintained and disposed separately.
	/// </summary>
	/// <param name="obj"></param>
	public virtual void Submit(T obj)
	{
		_3A6.Add(obj);
		_3AD.R(obj.WorldBoundingBox, obj);
		_3At.ObjectsSubmitted.AccumulationValue++;
	}

	/// <summary>
	/// Repositions an object within the container. This method is used when a static object
	/// moves to reposition it in the storage tree / scenegraph.
	/// </summary>
	/// <param name="obj"></param>
	public virtual void Move(T obj)
	{
		_3AD._2(obj.WorldBoundingBox, obj);
		_3At.ObjectsMoved.AccumulationValue++;
	}

	private void _7()
	{
		LightingSystemPerformance.Begin("ObjectGraph.MoveDynamicObjects");
		Dictionary<T, int> dynamicObjects = _3A6.DynamicObjects;
		_3A_0003.Clear();
		if (_3A_0019 && RequiresOptimization)
		{
			Optimize();
			foreach (KeyValuePair<T, int> item in dynamicObjects)
			{
				_3A_0003.Add(item.Key);
			}
			_3At.Optimized.AccumulationValue++;
		}
		else
		{
			foreach (KeyValuePair<T, int> item2 in dynamicObjects)
			{
				T key = item2.Key;
				if (key.MoveId != item2.Value)
				{
					_3AD._2(key.WorldBoundingBox, key);
					_3A_0003.Add(key);
					_3At.ObjectsMovedDynamic.AccumulationValue++;
				}
			}
			_3AD.n();
		}
		foreach (T item3 in _3A_0003)
		{
			dynamicObjects[item3] = item3.MoveId;
		}
	}

	/// <summary>
	/// Updates the object and its contained resources.
	/// </summary>
	/// <param name="gametime"></param>
	public virtual void Update(GameTime gametime)
	{
		Dictionary<T, int> dynamicObjects = _3A6.DynamicObjects;
		_3Al.Clear();
		foreach (KeyValuePair<T, int> item in dynamicObjects)
		{
			_3Al.Add(item.Key);
		}
		foreach (T item2 in _3Al)
		{
			item2.Update(gametime);
		}
		_7();
	}

	/// <summary>
	/// Retrieves an object of a specific type by name.  If no object
	/// is found an exception is thrown.
	///
	/// Note: if multiple objects are submitted using the same name the
	/// method will return the last object submitted using that name.
	/// </summary>
	/// <typeparam name="TCastType">Type of object to find.</typeparam>
	/// <param name="name">Name of the object to find.</param>
	/// <param name="onlysearchdynamicobjects">Determines if only dynamic
	/// objects are considered during the search. This emulates SunBurn 2.0.16
	/// and earlier behavior.</param>
	/// <returns>Returned object.</returns>
	public TCastType Find<TCastType>(string name, bool onlysearchdynamicobjects) where TCastType : class
	{
		if (Find<TCastType>(name, onlysearchdynamicobjects, out var obj))
		{
			return obj;
		}
		throw new Exception($"Unable to find object named '{name}'.");
	}

	/// <summary>
	/// Retrieves an object of a specific type by UniqueId.  If no object
	/// is found an exception is thrown.
	///
	/// Note: if multiple objects are submitted using the same UniqueId the
	/// method will return the last object submitted using that UniqueId.
	/// </summary>
	/// <typeparam name="TCastType">Type of object to find.</typeparam>
	/// <param name="uniqueid">UniqueId of the object to find.</param>
	/// <returns>Returned object.</returns>
	public TCastType Find<TCastType>(int uniqueid) where TCastType : class
	{
		if (Find<TCastType>(uniqueid, out var obj))
		{
			return obj;
		}
		throw new Exception($"Unable to find object with id '{uniqueid}'.");
	}

	/// <summary>
	/// Retrieves an object of a specific type by name.
	///
	/// Note: if multiple objects are submitted using the same name the
	/// method will return the last object submitted using that name.
	/// </summary>
	/// <typeparam name="TCastType">Type of object to find.</typeparam>
	/// <param name="name">Name of the object to find.</param>
	/// <param name="onlysearchdynamicobjects">Determines if only dynamic
	/// objects are considered during the search. This emulates SunBurn 2.0.16
	/// and earlier behavior.</param>
	/// <param name="obj">Returned object.</param>
	/// <returns>Returns true if an object was found.</returns>
	public bool Find<TCastType>(string name, bool onlysearchdynamicobjects, out TCastType obj) where TCastType : class
	{
		T value;
		if (onlysearchdynamicobjects)
		{
			_3A6.DynamicObjectsByName.TryGetValue(name, out value);
		}
		else
		{
			_3A6.AllObjectsByName.TryGetValue(name, out value);
		}
		obj = value as TCastType;
		return obj != null;
	}

	/// <summary>
	/// Retrieves an object of a specific type by UniqueId.
	///
	/// Note: if multiple objects are submitted using the same UniqueId the
	/// method will return the last object submitted using that UniqueId.
	/// </summary>
	/// <typeparam name="TCastType">Type of object to find.</typeparam>
	/// <param name="uniqueid">UniqueId of the object to find.</param>
	/// <param name="obj">Returned object.</param>
	/// <returns>Returns true if an object was found.</returns>
	public bool Find<TCastType>(int uniqueid, out TCastType obj) where TCastType : class
	{
		_3A6.AllObjectsByUniqueId.TryGetValue(uniqueid, out var value);
		obj = value as TCastType;
		return obj != null;
	}

	/// <summary>
	/// Finds all contained objects that match a set of filter attributes
	/// and overlap with or are contained in a bounding area.
	///
	/// Note: list will contain null entries when objects returned by the
	/// scenegraph are removed by the object filter.
	/// </summary>
	/// <param name="foundobjects">List used to store found objects during the query.</param>
	/// <param name="worldbounds">Bounding area used to limit query results.</param>
	/// <param name="objectfilter">Filter used to limit query results to objects with specific attributes.</param>
	public virtual void Find(List<T> foundobjects, BoundingFrustum worldbounds, ObjectFilter objectfilter)
	{
		int count = foundobjects.Count;
		worldbounds.GetCorners(_3A_0017);
		BoundingBox boundingBox = CoreHelper.CreateBoundingBoxFromPoints(_3A_0017);
		_3AD._0005(ref worldbounds, ref boundingBox, false, foundobjects);
		bool flag = (objectfilter & ObjectFilter.Dynamic) != 0;
		bool flag2 = (objectfilter & ObjectFilter.Static) != 0;
		int count2 = foundobjects.Count;
		if (flag && flag2)
		{
			_3At.ObjectsRetrieved.AccumulationValue += count2 - count;
			return;
		}
		for (int i = count; i < count2; i++)
		{
			UpdateType updateType = foundobjects[i].UpdateType;
			if ((!flag || updateType != UpdateType.Automatic) && (!flag2 || updateType != UpdateType.None))
			{
				foundobjects[i] = default(T);
			}
		}
		_3At.ObjectsRetrieved.AccumulationValue += foundobjects.Count - count;
	}

	/// <summary>
	/// Finds all contained objects that match a set of filter attributes
	/// and overlap with or are contained in a bounding area.
	///
	/// Note: list will contain null entries when objects returned by the
	/// scenegraph are removed by the object filter.
	/// </summary>
	/// <param name="foundobjects">List used to store found objects during the query.</param>
	/// <param name="worldbounds">Bounding area used to limit query results.</param>
	/// <param name="objectfilter">Filter used to limit query results to objects with specific attributes.</param>
	public virtual void Find(List<T> foundobjects, BoundingBox worldbounds, ObjectFilter objectfilter)
	{
		int count = foundobjects.Count;
		_3AD._0005(ref worldbounds, false, foundobjects);
		bool flag = (objectfilter & ObjectFilter.Dynamic) != 0;
		bool flag2 = (objectfilter & ObjectFilter.Static) != 0;
		int count2 = foundobjects.Count;
		if (flag && flag2)
		{
			_3At.ObjectsRetrieved.AccumulationValue += count2 - count;
			return;
		}
		for (int i = count; i < count2; i++)
		{
			UpdateType updateType = foundobjects[i].UpdateType;
			if ((!flag || updateType != UpdateType.Automatic) && (!flag2 || updateType != UpdateType.None))
			{
				foundobjects[i] = default(T);
			}
		}
		_3At.ObjectsRetrieved.AccumulationValue += foundobjects.Count - count;
	}

	/// <summary>
	/// Finds all contained objects that match a set of filter attributes.
	///
	/// Note: list will contain null entries when objects returned by the
	/// scenegraph are removed by the object filter.
	/// </summary>
	/// <param name="foundobjects">List used to store found objects during the query.</param>
	/// <param name="objectfilter">Filter used to limit query results to objects with specific attributes.</param>
	public virtual void Find(List<T> foundobjects, ObjectFilter objectfilter)
	{
		bool flag = (objectfilter & ObjectFilter.Dynamic) != 0;
		bool flag2 = (objectfilter & ObjectFilter.Static) != 0;
		if (flag && !flag2)
		{
			Dictionary<T, int> dynamicObjects = _3A6.DynamicObjects;
			foreach (KeyValuePair<T, int> item in dynamicObjects)
			{
				foundobjects.Add(item.Key);
			}
			_3At.ObjectsRetrieved.AccumulationValue += dynamicObjects.Count;
			return;
		}
		int count = foundobjects.Count;
		Dictionary<T, int> allObjects = _3A6.AllObjects;
		foreach (KeyValuePair<T, int> item2 in allObjects)
		{
			foundobjects.Add(item2.Key);
		}
		int count2 = foundobjects.Count;
		if (flag && flag2)
		{
			_3At.ObjectsRetrieved.AccumulationValue += count2 - count;
			return;
		}
		for (int i = count; i < count2; i++)
		{
			UpdateType updateType = foundobjects[i].UpdateType;
			if ((!flag || updateType != UpdateType.Automatic) && (!flag2 || updateType != UpdateType.None))
			{
				foundobjects[i] = default(T);
			}
		}
		_3At.ObjectsRetrieved.AccumulationValue += count2 - count;
	}

	/// <summary>
	/// Quickly finds all objects near a bounding area without the overhead of
	/// filtering by object type, checking if objects are enabled, or verifying
	/// containment within the bounds.
	/// </summary>
	/// <param name="foundobjects">List used to store found objects during the query.</param>
	/// <param name="worldbounds">Bounding area used to limit query results.</param>
	public virtual void FindFast(List<T> foundobjects, BoundingBox worldbounds)
	{
		int count = foundobjects.Count;
		_3AD._0005(ref worldbounds, true, foundobjects);
		_3At.ObjectsRetrieved.AccumulationValue += foundobjects.Count - count;
	}

	/// <summary>
	/// Quickly finds all objects without the overhead of filtering by object
	/// type or checking if objects are enabled.
	/// </summary>
	/// <param name="foundobjects">List used to store found objects during the query.</param>
	public virtual void FindFast(List<T> foundobjects)
	{
		int count = foundobjects.Count;
		Dictionary<T, int> allObjects = _3A6.AllObjects;
		foreach (KeyValuePair<T, int> item in allObjects)
		{
			foundobjects.Add(item.Key);
		}
		_3At.ObjectsRetrieved.AccumulationValue += foundobjects.Count - count;
	}

	/// <summary>
	/// Removes an object from the container.
	/// </summary>
	/// <param name="obj"></param>
	public virtual void Remove(T obj)
	{
		_3A6.Remove(obj);
		_3AD._0002(obj.WorldBoundingBox, obj);
		_3At.ObjectsRemoved.AccumulationValue++;
	}

	/// <summary>
	/// Removes resources managed by this object. Commonly used while clearing the scene.
	/// </summary>
	public virtual void Clear()
	{
		_3A6.Clear();
		_3AD.U();
	}
}
