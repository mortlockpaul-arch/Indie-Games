using System.Collections.Generic;
using FarseerGames.FarseerPhysics.Interfaces;

namespace FarseerGames.FarseerPhysics.Collisions;

public sealed class SelectiveSweepCollider : IBroadPhaseCollider
{
	private sealed class Stub
	{
		public bool Begin;

		public float Value;

		public Wrapper Wrapper;

		public Stub(Wrapper wrapper, bool begin)
		{
			Wrapper = wrapper;
			Begin = begin;
		}
	}

	private sealed class StubComparer : IComparer<Stub>
	{
		public int Compare(Stub left, Stub right)
		{
			if (left.Value < right.Value)
			{
				return -1;
			}
			if (left.Value > right.Value)
			{
				return 1;
			}
			if (left != right)
			{
				if (!left.Begin)
				{
					return 1;
				}
				return -1;
			}
			return 0;
		}
	}

	private sealed class Wrapper
	{
		private Stub _xBegin;

		private Stub _xEnd;

		private Stub _yBegin;

		private Stub _yEnd;

		public Geom Geom;

		public float Max;

		public float Min;

		public LinkedListNode<Wrapper> Node;

		public bool ShouldAddNode;

		public Wrapper(Geom body)
		{
			Geom = body;
			Node = new LinkedListNode<Wrapper>(this);
			_xBegin = new Stub(this, begin: true);
			_xEnd = new Stub(this, begin: false);
			_yBegin = new Stub(this, begin: true);
			_yEnd = new Stub(this, begin: false);
		}

		public void AddStubs(List<Stub> xStubs, List<Stub> yStubs)
		{
			xStubs.Add(_xBegin);
			xStubs.Add(_xEnd);
			yStubs.Add(_yBegin);
			yStubs.Add(_yEnd);
		}

		public void Update()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			AABB aABB = Geom.AABB;
			ShouldAddNode = aABB.Min.X != aABB.Max.X || aABB.Min.Y != aABB.Max.Y;
			_xBegin.Value = aABB.Min.X;
			_xEnd.Value = aABB.Max.X;
			_yBegin.Value = aABB.Min.Y;
			_yEnd.Value = aABB.Max.Y;
		}

		public void SetX()
		{
			Min = _xBegin.Value;
			Max = _xEnd.Value;
		}

		public void SetY()
		{
			Min = _yBegin.Value;
			Max = _yEnd.Value;
		}
	}

	private static StubComparer _comparer = new StubComparer();

	private LinkedList<Wrapper> _currentBodies = new LinkedList<Wrapper>();

	private PhysicsSimulator _physicsSimulator;

	private List<Wrapper> _wrappers;

	private List<Stub> _xStubs;

	private List<Stub> _yStubs;

	public event BroadPhaseCollisionHandler OnBroadPhaseCollision;

	public SelectiveSweepCollider(PhysicsSimulator physicsSimulator)
	{
		_physicsSimulator = physicsSimulator;
		_wrappers = new List<Wrapper>();
		_xStubs = new List<Stub>();
		_yStubs = new List<Stub>();
	}

	public void Add(Geom geom)
	{
		Wrapper wrapper = new Wrapper(geom);
		_wrappers.Add(wrapper);
		wrapper.AddStubs(_xStubs, _yStubs);
	}

	public void ProcessRemovedGeoms()
	{
		if (_wrappers.RemoveAll(WrapperIsRemoved) > 0)
		{
			_xStubs.RemoveAll(StubIsRemoved);
			_yStubs.RemoveAll(StubIsRemoved);
		}
	}

	public void ProcessDisposedGeoms()
	{
		if (_wrappers.RemoveAll(WrapperIsDisposed) > 0)
		{
			_xStubs.RemoveAll(StubIsDisposed);
			_yStubs.RemoveAll(StubIsDisposed);
		}
	}

	public void Update()
	{
		InternalUpdate();
		DetectInternal(ShouldDoX());
	}

	public void Clear()
	{
		_wrappers.Clear();
		_xStubs.Clear();
		_yStubs.Clear();
	}

	private static bool WrapperIsDisposed(Wrapper wrapper)
	{
		return wrapper.Geom.IsDisposed;
	}

	private static bool StubIsDisposed(Stub stub)
	{
		return stub.Wrapper.Geom.IsDisposed;
	}

	private static bool WrapperIsRemoved(Wrapper wrapper)
	{
		return !wrapper.Geom.InSimulation;
	}

	private static bool StubIsRemoved(Stub stub)
	{
		return !stub.Wrapper.Geom.InSimulation;
	}

	private void InternalUpdate()
	{
		for (int i = 0; i < _wrappers.Count; i++)
		{
			_wrappers[i].Update();
		}
		_xStubs.Sort(_comparer);
		_yStubs.Sort(_comparer);
	}

	private bool ShouldDoX()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < _xStubs.Count; i++)
		{
			if (_xStubs[i].Begin)
			{
				num += num2++;
			}
			else
			{
				num2--;
			}
			if (_yStubs[i].Begin)
			{
				num3 += num4++;
			}
			else
			{
				num4--;
			}
		}
		return num < num3;
	}

	private void DetectInternal(bool doX)
	{
		List<Stub> list = (doX ? _xStubs : _yStubs);
		_currentBodies.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			Stub stub = list[i];
			Wrapper wrapper = stub.Wrapper;
			if (stub.Begin)
			{
				if (doX)
				{
					wrapper.SetY();
				}
				else
				{
					wrapper.SetX();
				}
				Geom geom = wrapper.Geom;
				for (LinkedListNode<Wrapper> linkedListNode = _currentBodies.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					Wrapper value = linkedListNode.Value;
					Geom geom2 = value.Geom;
					if (wrapper.Min <= value.Max && value.Min <= wrapper.Max && geom.body.Enabled && geom2.body.Enabled && (geom.CollisionGroup != geom2.CollisionGroup || geom.CollisionGroup == 0 || geom2.CollisionGroup == 0) && geom.CollisionEnabled && geom2.CollisionEnabled && (!geom.body.isStatic || !geom2.body.isStatic) && geom.body != geom2.body && !(((geom.CollisionCategories & geom2.CollidesWith) == 0) & ((geom2.CollisionCategories & geom.CollidesWith) == 0)) && !geom.IsGeometryIgnored(geom2) && !geom2.IsGeometryIgnored(geom))
					{
						bool flag = true;
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
				if (wrapper.ShouldAddNode)
				{
					_currentBodies.AddLast(wrapper.Node);
				}
			}
			else if (wrapper.ShouldAddNode)
			{
				_currentBodies.Remove(wrapper.Node);
			}
		}
	}
}
