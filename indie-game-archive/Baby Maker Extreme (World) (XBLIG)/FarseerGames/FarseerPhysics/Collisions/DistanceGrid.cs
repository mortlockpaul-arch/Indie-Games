using System;
using System.Collections.Generic;
using FarseerGames.FarseerPhysics.Interfaces;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

public class DistanceGrid : INarrowPhaseCollider
{
	private const float _gridCellSizeAABBFactor = 0.1f;

	private static DistanceGrid _instance;

	private Dictionary<int, DistanceGridData> _distanceGrids = new Dictionary<int, DistanceGridData>();

	private Feature _feature;

	private Vector2 _localVertex;

	private Vector2 _vertRef;

	public static DistanceGrid Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new DistanceGrid();
			}
			return _instance;
		}
	}

	private DistanceGrid()
	{
	}

	public void Collide(Geom geomA, Geom geomB, ContactList contactList)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		DistanceGridData distanceGridData = _distanceGrids[geomA.id];
		for (int i = 0; i < geomB.worldVertices.Count; i++)
		{
			if (contactList.Count == PhysicsSimulator.MaxContactsToDetect)
			{
				break;
			}
			num++;
			_vertRef = geomB.WorldVertices[i];
			geomA.TransformToLocalCoordinates(ref _vertRef, out _localVertex);
			if (distanceGridData.Intersect(ref _localVertex, out _feature) && _feature.Distance < 0f)
			{
				geomA.TransformNormalToWorld(ref _feature.Normal, out _feature.Normal);
				Contact item = new Contact(geomB.WorldVertices[i], _feature.Normal, _feature.Distance, new ContactId(geomB.id, num, geomA.id));
				contactList.Add(item);
			}
		}
		DistanceGridData distanceGridData2 = _distanceGrids[geomB.id];
		for (int j = 0; j < geomA.WorldVertices.Count; j++)
		{
			if (contactList.Count == PhysicsSimulator.MaxContactsToDetect)
			{
				break;
			}
			num++;
			_vertRef = geomA.WorldVertices[j];
			geomB.TransformToLocalCoordinates(ref _vertRef, out _localVertex);
			if (distanceGridData2.Intersect(ref _localVertex, out _feature) && _feature.Distance < 0f)
			{
				geomB.TransformNormalToWorld(ref _feature.Normal, out _feature.Normal);
				_feature.Normal = -_feature.Normal;
				Contact item2 = new Contact(geomA.WorldVertices[j], _feature.Normal, _feature.Distance, new ContactId(geomA.id, num, geomB.id));
				contactList.Add(item2);
			}
		}
	}

	public void CreateDistanceGrid(Geom geom)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		if (geom == null)
		{
			throw new ArgumentNullException("geom", "Geometry can't be null");
		}
		if (_distanceGrids.ContainsKey(geom.id))
		{
			return;
		}
		if (geom.GridCellSize <= 0f)
		{
			geom.GridCellSize = CalculateGridCellSizeFromAABB(ref geom.AABB);
		}
		Matrix matrix = geom.Matrix;
		geom.Matrix = Matrix.Identity;
		AABB aABB = new AABB(ref geom.AABB);
		float num = 1f / geom.GridCellSize;
		int num2 = (int)Math.Ceiling((double)(aABB.Max.X - aABB.Min.X) * (double)num) + 1;
		int num3 = (int)Math.Ceiling((double)(aABB.Max.Y - aABB.Min.Y) * (double)num) + 1;
		float[,] array = new float[num2, num3];
		Vector2 point = aABB.Min;
		int num4 = 0;
		while (num4 < num2)
		{
			point.Y = aABB.Min.Y;
			int num5 = 0;
			while (num5 < num3)
			{
				array[num4, num5] = geom.GetNearestDistance(ref point);
				num5++;
				point.Y += geom.GridCellSize;
			}
			num4++;
			point.X += geom.GridCellSize;
		}
		geom.Matrix = matrix;
		DistanceGridData value = new DistanceGridData
		{
			AABB = aABB,
			GridCellSize = geom.GridCellSize,
			GridCellSizeInv = num,
			Nodes = array
		};
		_distanceGrids.Add(geom.id, value);
	}

	public void RemoveDistanceGrid(Geom geom)
	{
		_distanceGrids.Remove(geom.id);
	}

	public void Copy(int fromId, int toId)
	{
		_distanceGrids.Add(toId, _distanceGrids[fromId]);
	}

	private float CalculateGridCellSizeFromAABB(ref AABB aabb)
	{
		return aabb.GetShortestSide() * 0.1f;
	}

	public bool Intersect(Geom geom, ref Vector2 point)
	{
		Feature feature;
		return _distanceGrids[geom.id].Intersect(ref point, out feature);
	}
}
