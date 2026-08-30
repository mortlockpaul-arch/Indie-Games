using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Interfaces;

namespace FarseerGames.FarseerPhysics.Collisions;

public class BruteForceCollider : IBroadPhaseCollider
{
	private Geom _geometryA;

	private Geom _geometryB;

	private PhysicsSimulator _physicsSimulator;

	public event BroadPhaseCollisionHandler OnBroadPhaseCollision;

	public BruteForceCollider(PhysicsSimulator physicsSimulator)
	{
		_physicsSimulator = physicsSimulator;
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
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _physicsSimulator.GeomList.Count - 1; i++)
		{
			for (int j = i + 1; j < _physicsSimulator.GeomList.Count; j++)
			{
				_geometryA = _physicsSimulator.GeomList[i];
				_geometryB = _physicsSimulator.GeomList[j];
				if (!_geometryA.body.Enabled || !_geometryB.body.Enabled || (_geometryA.CollisionGroup == _geometryB.CollisionGroup && _geometryA.CollisionGroup != 0 && _geometryB.CollisionGroup != 0) || !_geometryA.CollisionEnabled || !_geometryB.CollisionEnabled || (_geometryA.body.isStatic && _geometryB.body.isStatic) || _geometryA.body == _geometryB.body || (((_geometryA.CollisionCategories & _geometryB.CollidesWith) == 0) & ((_geometryB.CollisionCategories & _geometryA.CollidesWith) == 0)) || _geometryA.IsGeometryIgnored(_geometryB) || _geometryB.IsGeometryIgnored(_geometryA))
				{
					continue;
				}
				bool flag = true;
				if (_geometryA.AABB.min.X > _geometryB.AABB.max.X || _geometryB.AABB.min.X > _geometryA.AABB.max.X)
				{
					flag = false;
				}
				else if (_geometryA.AABB.min.Y > _geometryB.AABB.Max.Y || _geometryB.AABB.min.Y > _geometryA.AABB.Max.Y)
				{
					flag = false;
				}
				if (OnBroadPhaseCollision != null)
				{
					flag = OnBroadPhaseCollision(_geometryA, _geometryB);
				}
				if (flag)
				{
					Arbiter arbiter = _physicsSimulator.arbiterPool.Fetch();
					arbiter.ConstructArbiter(_geometryA, _geometryB, _physicsSimulator);
					if (!_physicsSimulator.ArbiterList.Contains(arbiter))
					{
						_physicsSimulator.ArbiterList.Add(arbiter);
					}
					else
					{
						_physicsSimulator.arbiterPool.Insert(arbiter);
					}
				}
			}
		}
	}
}
