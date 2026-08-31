using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Z;

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
	private class _0001CB
	{
		public SystemStatistic ObjectsSubmitted = SystemConsole.GetStatistic("SceneGraph_ObjectsSubmitted", SystemStatisticCategory.SceneGraph);

		public SystemStatistic ObjectsMoved = SystemConsole.GetStatistic("SceneGraph_ObjectsMoved", SystemStatisticCategory.SceneGraph);

		public SystemStatistic ObjectsMovedDynamic = SystemConsole.GetStatistic("SceneGraph_ObjectsMovedDynamic", SystemStatisticCategory.SceneGraph);

		public SystemStatistic ObjectsRemoved = SystemConsole.GetStatistic("SceneGraph_ObjectsRemoved", SystemStatisticCategory.SceneGraph);

		public SystemStatistic ObjectsRetrieved = SystemConsole.GetStatistic("SceneGraph_ObjectsRetrieved", SystemStatisticCategory.SceneGraph);

		public SystemStatistic Optimized = SystemConsole.GetStatistic("SceneGraph_Optimized", SystemStatisticCategory.SceneGraph);
	}

	private BoundingBox HCB;

	private int HC_0002 = 20;

	private bool HC_0012 = true;

	private int HCH;

	private ObjectIndex<T> HC7 = new ObjectIndex<T>();

	private Z._0002<T> HC_0001 = new Z._0002<T>();

	private Vector3[] HCw = new Vector3[8];

	private List<T> HCZ = new List<T>(32);

	private List<T> HC_000F = new List<T>(32);

	private _0001CB HCy = new _0001CB();

	/// <summary>
	/// The current containment volume for this object.
	/// </summary>
	public BoundingBox WorldBoundingBox => HCB;

	/// <summary>
	/// Enables automatic optimizations on the tree used to store contained objects. 
	/// Optimization occurs when a large number of objects fall outside of the tree bounds.
	/// </summary>
	public bool AutoOptimize
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
		}
	}

	/// <summary>
	/// Determines if the tree used to store contained objects requires optimization.
	/// </summary>
	public bool RequiresOptimization
	{
		get
		{
			if (HCH < 10)
			{
				HCH++;
				return false;
			}
			HCH = 0;
			return HC_0001._0017();
		}
	}

	/// <summary>
	/// Index of all objects contained in the manager / object graph.
	/// </summary>
	protected ObjectIndex<T> ObjectIndex => HC7;

	/// <summary>
	/// Creates a new ObjectGraph using the default world size and tree depth.
	/// </summary>
	public ObjectGraph()
	{
		HCB = new BoundingBox(new Vector3(-1000f, -1000f, -1000f), new Vector3(1000f, 1000f, 1000f));
		Resize(HCB, HC_0002);
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
		HCB = worldboundingbox;
		HC_0002 = worldtreemaxdepth;
		HC_0001.B(ref worldboundingbox, worldtreemaxdepth);
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
		Dictionary<T, int> allObjects = HC7.AllObjects;
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
			HC_0001._0015(key2.WorldBoundingBox, key2);
		}
		HCy.Optimized.AccumulationValue++;
	}

	/// <summary>
	/// Adds an object to the container. This does not transfer ownership, disposable
	/// objects should be maintained and disposed separately.
	/// </summary>
	/// <param name="obj"></param>
	public virtual void Submit(T obj)
	{
		HC7.Add(obj);
		HC_0001._0015(obj.WorldBoundingBox, obj);
		HCy.ObjectsSubmitted.AccumulationValue++;
	}

	/// <summary>
	/// Repositions an object within the container. This method is used when a static object
	/// moves to reposition it in the storage tree / scenegraph.
	/// </summary>
	/// <param name="obj"></param>
	public virtual void Move(T obj)
	{
		HC_0001.U(obj.WorldBoundingBox, obj);
		HCy.ObjectsMoved.AccumulationValue++;
	}

	private void V()
	{
		LightingSystemPerformance.Begin("ObjectGraph.MoveDynamicObjects");
		Dictionary<T, int> dynamicObjects = HC7.DynamicObjects;
		HCZ.Clear();
		if (HC_0012 && RequiresOptimization)
		{
			Optimize();
			foreach (KeyValuePair<T, int> item in dynamicObjects)
			{
				HCZ.Add(item.Key);
			}
			HCy.Optimized.AccumulationValue++;
		}
		else
		{
			foreach (KeyValuePair<T, int> item2 in dynamicObjects)
			{
				T key = item2.Key;
				if (key.MoveId != item2.Value)
				{
					HC_0001.U(key.WorldBoundingBox, key);
					HCZ.Add(key);
					HCy.ObjectsMovedDynamic.AccumulationValue++;
				}
			}
			HC_0001.e();
		}
		foreach (T item3 in HCZ)
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
		Dictionary<T, int> dynamicObjects = HC7.DynamicObjects;
		HC_000F.Clear();
		foreach (KeyValuePair<T, int> item in dynamicObjects)
		{
			HC_000F.Add(item.Key);
		}
		foreach (T item2 in HC_000F)
		{
			item2.Update(gametime);
		}
		V();
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
			HC7.DynamicObjectsByName.TryGetValue(name, out value);
		}
		else
		{
			HC7.AllObjectsByName.TryGetValue(name, out value);
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
		HC7.AllObjectsByUniqueId.TryGetValue(uniqueid, out var value);
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
		worldbounds.GetCorners(HCw);
		BoundingBox boundingBox = CoreHelper.CreateBoundingBoxFromPoints(HCw);
		HC_0001.i(ref worldbounds, ref boundingBox, false, foundobjects);
		bool flag = (objectfilter & ObjectFilter.Dynamic) != 0;
		bool flag2 = (objectfilter & ObjectFilter.Static) != 0;
		int count2 = foundobjects.Count;
		if (flag && flag2)
		{
			HCy.ObjectsRetrieved.AccumulationValue += count2 - count;
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
		HCy.ObjectsRetrieved.AccumulationValue += foundobjects.Count - count;
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
		HC_0001.i(ref worldbounds, false, foundobjects);
		bool flag = (objectfilter & ObjectFilter.Dynamic) != 0;
		bool flag2 = (objectfilter & ObjectFilter.Static) != 0;
		int count2 = foundobjects.Count;
		if (flag && flag2)
		{
			HCy.ObjectsRetrieved.AccumulationValue += count2 - count;
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
		HCy.ObjectsRetrieved.AccumulationValue += foundobjects.Count - count;
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
			Dictionary<T, int> dynamicObjects = HC7.DynamicObjects;
			foreach (KeyValuePair<T, int> item in dynamicObjects)
			{
				foundobjects.Add(item.Key);
			}
			HCy.ObjectsRetrieved.AccumulationValue += dynamicObjects.Count;
			return;
		}
		int count = foundobjects.Count;
		Dictionary<T, int> allObjects = HC7.AllObjects;
		foreach (KeyValuePair<T, int> item2 in allObjects)
		{
			foundobjects.Add(item2.Key);
		}
		int count2 = foundobjects.Count;
		if (flag && flag2)
		{
			HCy.ObjectsRetrieved.AccumulationValue += count2 - count;
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
		HCy.ObjectsRetrieved.AccumulationValue += count2 - count;
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
		HC_0001.i(ref worldbounds, true, foundobjects);
		HCy.ObjectsRetrieved.AccumulationValue += foundobjects.Count - count;
	}

	/// <summary>
	/// Quickly finds all objects without the overhead of filtering by object
	/// type or checking if objects are enabled.
	/// </summary>
	/// <param name="foundobjects">List used to store found objects during the query.</param>
	public virtual void FindFast(List<T> foundobjects)
	{
		int count = foundobjects.Count;
		Dictionary<T, int> allObjects = HC7.AllObjects;
		foreach (KeyValuePair<T, int> item in allObjects)
		{
			foundobjects.Add(item.Key);
		}
		HCy.ObjectsRetrieved.AccumulationValue += foundobjects.Count - count;
	}

	/// <summary>
	/// Removes an object from the container.
	/// </summary>
	/// <param name="obj"></param>
	public virtual void Remove(T obj)
	{
		HC7.Remove(obj);
		HC_0001._8(obj.WorldBoundingBox, obj);
		HCy.ObjectsRemoved.AccumulationValue++;
	}

	/// <summary>
	/// Removes resources managed by this object. Commonly used while clearing the scene.
	/// </summary>
	public virtual void Clear()
	{
		HC7.Clear();
		HC_0001.G();
	}
}
