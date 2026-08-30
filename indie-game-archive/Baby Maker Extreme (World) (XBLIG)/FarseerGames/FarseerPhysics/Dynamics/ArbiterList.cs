using System.Collections.Generic;
using FarseerGames.FarseerPhysics.Collisions;

namespace FarseerGames.FarseerPhysics.Dynamics;

public class ArbiterList : List<Arbiter>
{
	private GeomPairBitmap _geomPairBitmap;

	public ArbiterList(int capacity)
	{
		base.Capacity = capacity;
	}

	public void PrepareForBroadphaseCollision(List<Geom> geomList)
	{
		if (_geomPairBitmap == null)
		{
			_geomPairBitmap = new GeomPairBitmap(geomList.Count, this);
		}
		else
		{
			_geomPairBitmap.Clear(geomList.Count, this);
		}
		CollisionIdGenerator collisionIdGenerator = new CollisionIdGenerator();
		foreach (Geom geom in geomList)
		{
			geom.CollisionId = collisionIdGenerator.NextCollisionId();
		}
	}

	public void AddArbiterForGeomPair(PhysicsSimulator physicsSimulator, Geom geom1, Geom geom2)
	{
		if (!_geomPairBitmap.TestAndSet(geom1, geom2))
		{
			Arbiter arbiter = physicsSimulator.arbiterPool.Fetch();
			arbiter.ConstructArbiter(geom1, geom2, physicsSimulator);
			Add(arbiter);
		}
	}

	public void RemoveContactCountEqualsZero(Pool<Arbiter> arbiterPool)
	{
		for (int num = base.Count - 1; num >= 0; num--)
		{
			if (base[num].ContactList.Count == 0)
			{
				Arbiter item = base[num];
				RemoveAt(num);
				arbiterPool.Insert(item);
			}
		}
	}

	public void CleanArbiterList(Pool<Arbiter> arbiterPool)
	{
		for (int num = base.Count - 1; num >= 0; num--)
		{
			if (base[num].ContainsInvalidGeom())
			{
				Arbiter item = base[num];
				RemoveAt(num);
				arbiterPool.Insert(item);
			}
		}
	}
}
