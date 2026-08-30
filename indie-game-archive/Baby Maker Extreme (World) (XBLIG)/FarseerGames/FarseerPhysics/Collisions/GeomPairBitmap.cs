using System;
using FarseerGames.FarseerPhysics.Dynamics;

namespace FarseerGames.FarseerPhysics.Collisions;

public class GeomPairBitmap
{
	private int _geomCount = -1;

	private bool[] _bitmap;

	public GeomPairBitmap(int geomCount, ArbiterList arbiterList)
	{
		Clear(geomCount, arbiterList);
	}

	public void Clear(int newGeomCount, ArbiterList arbiterList)
	{
		if (_geomCount >= newGeomCount)
		{
			Array.Clear(_bitmap, 0, _bitmap.Length);
		}
		else
		{
			_geomCount = newGeomCount;
			_bitmap = new bool[CalculateSize(newGeomCount)];
		}
		if (arbiterList == null)
		{
			return;
		}
		foreach (Arbiter arbiter in arbiterList)
		{
			TestAndSet(arbiter.GeometryA, arbiter.GeometryB);
		}
	}

	public bool TestAndSet(Geom geom1, Geom geom2)
	{
		int num = CalculateIndex(geom1, geom2);
		bool result = _bitmap[num];
		_bitmap[num] = true;
		return result;
	}

	private int CalculateSize(int geomCount)
	{
		if (geomCount % 2 == 0)
		{
			return geomCount * (geomCount - 1) - (geomCount * ((geomCount - 1) / 2) + geomCount / 2);
		}
		return geomCount * (geomCount - 1) - geomCount * (geomCount / 2);
	}

	private int CalculateIndex(Geom geom1, Geom geom2)
	{
		int collisionId;
		int collisionId2;
		if (geom1.CollisionId < geom2.CollisionId)
		{
			collisionId = geom1.CollisionId;
			collisionId2 = geom2.CollisionId;
		}
		else
		{
			collisionId = geom2.CollisionId;
			collisionId2 = geom1.CollisionId;
		}
		int num = collisionId * _geomCount;
		num = ((collisionId % 2 != 0) ? (num - ((collisionId + 1) * (collisionId / 2) + (collisionId + 1) / 2)) : (num - (collisionId + 1) * (collisionId / 2)));
		return num + (collisionId2 - collisionId - 1);
	}
}
