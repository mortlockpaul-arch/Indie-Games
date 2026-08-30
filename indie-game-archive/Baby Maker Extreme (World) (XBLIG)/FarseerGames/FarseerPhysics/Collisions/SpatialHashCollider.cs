using System;
using System.Collections.Generic;
using FarseerGames.FarseerPhysics.Interfaces;

namespace FarseerGames.FarseerPhysics.Collisions;

public class SpatialHashCollider : IBroadPhaseCollider
{
	private PhysicsSimulator _physicsSimulator;

	private Dictionary<long, List<Geom>> _hash;

	private Dictionary<long, object> _filter;

	private List<long> _keysToRemove;

	private float _cellSize;

	private float _cellSizeInv;

	public bool AutoAdjustCellSize = true;

	public float CellSize
	{
		get
		{
			return _cellSize;
		}
		set
		{
			_cellSize = value;
			_cellSizeInv = 1f / value;
		}
	}

	public event BroadPhaseCollisionHandler OnBroadPhaseCollision;

	public SpatialHashCollider(PhysicsSimulator physicsSimulator)
		: this(physicsSimulator, 50f, 2048)
	{
		_physicsSimulator = physicsSimulator;
	}

	public SpatialHashCollider(PhysicsSimulator physicsSimulator, float cellSize, int hashCapacity)
	{
		_physicsSimulator = physicsSimulator;
		_hash = new Dictionary<long, List<Geom>>(hashCapacity);
		_keysToRemove = new List<long>(hashCapacity);
		_filter = new Dictionary<long, object>();
		_cellSize = cellSize;
		_cellSizeInv = 1f / cellSize;
	}

	public void ProcessRemovedGeoms()
	{
	}

	public void ProcessDisposedGeoms()
	{
	}

	public void Add(Geom geom)
	{
	}

	public void Update()
	{
		if (_physicsSimulator.GeomList.Count != 0)
		{
			FillHash();
			RunHash();
		}
	}

	private void FillHash()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = 0; i < _physicsSimulator.GeomList.Count; i++)
		{
			Geom geom = _physicsSimulator.GeomList[i];
			AABB aABB = geom.AABB;
			if (AutoAdjustCellSize)
			{
				num += Math.Max(aABB.Max.X - aABB.Min.X, aABB.Max.Y - aABB.Min.Y);
			}
			int num2 = (int)(aABB.Min.X * _cellSizeInv);
			int num3 = (int)(aABB.Max.X * _cellSizeInv) + 1;
			int num4 = (int)(aABB.Min.Y * _cellSizeInv);
			int num5 = (int)(aABB.Max.Y * _cellSizeInv) + 1;
			for (int j = num2; j < num3; j++)
			{
				for (int k = num4; k < num5; k++)
				{
					long hash = PairID.GetHash(j, k);
					if (!_hash.TryGetValue(hash, out var value))
					{
						value = new List<Geom>();
						_hash.Add(hash, value);
					}
					value.Add(geom);
				}
			}
		}
		if (AutoAdjustCellSize)
		{
			CellSize = 2f * num / (float)_physicsSimulator.GeomList.Count;
		}
	}

	private void RunHash()
	{
		_keysToRemove.Clear();
		foreach (KeyValuePair<long, List<Geom>> item in _hash)
		{
			List<Geom> value = item.Value;
			if (value.Count == 0)
			{
				_keysToRemove.Add(item.Key);
				continue;
			}
			for (int i = 0; i < value.Count - 1; i++)
			{
				Geom geom = value[i];
				for (int j = i + 1; j < value.Count; j++)
				{
					Geom geom2 = value[j];
					if (!geom.body.Enabled || !geom2.body.Enabled || (geom.CollisionGroup == geom2.CollisionGroup && geom.CollisionGroup != 0 && geom2.CollisionGroup != 0) || !geom.CollisionEnabled || !geom2.CollisionEnabled || (geom.body.isStatic && geom2.body.isStatic) || geom.body == geom2.body || (((geom.CollisionCategories & geom2.CollidesWith) == 0) & ((geom2.CollisionCategories & geom.CollidesWith) == 0)) || geom.IsGeometryIgnored(geom2) || geom2.IsGeometryIgnored(geom))
					{
						continue;
					}
					long id = PairID.GetId(geom.id, geom2.id);
					if (!_filter.ContainsKey(id))
					{
						_filter.Add(id, null);
						bool flag = AABB.Intersect(ref geom.AABB, ref geom2.AABB);
						if (OnBroadPhaseCollision != null)
						{
							flag = OnBroadPhaseCollision(geom, geom2);
						}
						if (flag)
						{
							_physicsSimulator.ArbiterList.AddArbiterForGeomPair(_physicsSimulator, geom, geom2);
						}
					}
				}
			}
			value.Clear();
		}
		_filter.Clear();
		for (int k = 0; k < _keysToRemove.Count; k++)
		{
			_hash.Remove(_keysToRemove[k]);
		}
	}
}
