using System;
using System.Collections.Generic;
using E;
using L;
using Microsoft.Xna.Framework;
using Y;
using d;
using l;
using p;
using r;
using u;

namespace g
{
	internal class h : Y.b
	{
		internal struct _00065h
		{
			internal b a5h;

			internal b a5b;
		}

		internal b a5h;

		private l._7<b> a5b = new l._7<b>(4);

		private Action<int> a56;

		private l._7<_00065h> a5a = new l._7<_00065h>(10);

		private Action<int> a57;

		private p.a<a> a5_0006 = new p.a<a>();

		public h()
		{
			a56 = bO;
			a57 = b3;
			base.QueryAccelerator = new _7(this);
		}

		public h(d.b threadManager)
			: base(threadManager)
		{
			a56 = bO;
			a57 = b3;
			base.QueryAccelerator = new _7(this);
		}

		protected override void UpdateMultithreaded()
		{
			lock (base.Locker)
			{
				base.Overlaps.Clear();
				if (a5h != null)
				{
					int num = (int)Math.Ceiling(Math.Log(base.ThreadManager.ThreadCount, 2.0));
					a5h.snD_0005v(num, 1, a5b);
					base.ThreadManager.ForLoop(0, a5b.a5h, a56);
					a5b.Clear();
					a5h.PDR97(num, 1);
					if (!a5h.IsLeaf)
					{
						a5h.GPr3r5(a5h, num, 1, this, a5a);
						base.ThreadManager.ForLoop(0, a5a.a5h, a57);
						a5a.Clear();
					}
				}
			}
		}

		private void bO(int P_0)
		{
			a5b.Elements[P_0].bQS_0001D5();
		}

		private void b3(int P_0)
		{
			_00065h obj = a5a.Elements[P_0];
			obj.a5h.y61P_00175(obj.a5b, this);
		}

		protected override void UpdateSingleThreaded()
		{
			lock (base.Locker)
			{
				base.Overlaps.Clear();
				if (a5h != null)
				{
					a5h.bQS_0001D5();
					if (!a5h.IsLeaf)
					{
						a5h.y61P_00175(a5h, this);
					}
				}
			}
		}

		public override void Add(Y.a entry)
		{
			Vector3.Subtract(ref entry.boundingBox.Max, ref entry.boundingBox.Min, out var result);
			if (result.X * result.Y * result.Z == 0f)
			{
				entry.UpdateBoundingBox();
			}
			a a2 = a5_0006.Take();
			a2._4(entry);
			if (a5h == null)
			{
				a5h = a2;
				return;
			}
			if (a5h.IsLeaf)
			{
				a5h._00138_0015_0006f5(a2, out a5h);
				return;
			}
			BoundingBox.CreateMerged(ref a2.a5h, ref a5h.a5h, out a5h.a5h);
			_6 obj = (_6)a5h;
			Vector3.Subtract(ref a5h.a5h.Max, ref a5h.a5h.Min, out result);
			obj.a56 = result.X * result.Y * result.Z;
			b b2 = a5h;
			while (!b2._00138_0015_0006f5(a2, out b2))
			{
			}
		}

		public override void Remove(Y.a entry)
		{
			if (a5h == null)
			{
				throw new InvalidOperationException("Entry not present in the hierarchy.");
			}
			if (a5h._0013e_0003d(entry, out var a2, out a5h))
			{
				a2.bV();
				a5_0006.GiveBack(a2);
				return;
			}
			throw new InvalidOperationException("Entry not present in the hierarchy.");
		}

		internal void bt(List<int> P_0, out int P_1)
		{
			P_1 = 0;
			a5h._0017_0017HY1(P_0, 0, ref P_1);
		}

		internal void bo()
		{
			((_6)a5h).bF();
		}
	}
}
namespace G
{
	internal abstract class h : L.h
	{
		internal new readonly l._7<L.h> a5h = new l._7<L.h>();

		public l.X<L.h> SolverUpdateables => new l.X<L.h>(a5h);

		public override u.b Solver
		{
			get
			{
				return solver;
			}
			internal set
			{
				base.Solver = b2;
				for (int i = 0; i < a5h.a5h; i++)
				{
					a5h.Elements[i].Solver = b2;
				}
			}
		}

		protected internal override void CollectInvolvedEntities(l._7<E.h> outputInvolvedEntities)
		{
			foreach (L.h item in a5h)
			{
				for (int i = 0; i < item.involvedEntities.a5h; i++)
				{
					if (!outputInvolvedEntities.Contains(item.involvedEntities.Elements[i]))
					{
						outputInvolvedEntities.Add(item.involvedEntities.Elements[i]);
					}
				}
			}
		}

		public override void UpdateSolverActivity()
		{
			if (isActive)
			{
				isActiveInSolver = false;
				for (int i = 0; i < a5h.a5h; i++)
				{
					L.h h2 = a5h.Elements[i];
					h2.UpdateSolverActivity();
					isActiveInSolver |= h2.isActiveInSolver;
				}
			}
			else
			{
				isActiveInSolver = false;
			}
		}

		protected void UpdateUpdateable(L.h item, float dt)
		{
			item.SolverSettings.a5h = 0;
			item.SolverSettings.a57 = 0;
			if (item.isActiveInSolver)
			{
				item.Update(dt);
			}
		}

		protected void ExclusiveUpdateUpdateable(L.h item)
		{
			if (item.isActiveInSolver)
			{
				item.ExclusiveUpdate();
			}
		}

		public override void Update(float dt)
		{
			for (int i = 0; i < a5h.a5h; i++)
			{
				UpdateUpdateable(a5h.Elements[i], dt);
			}
		}

		public override void ExclusiveUpdate()
		{
			for (int i = 0; i < a5h.a5h; i++)
			{
				ExclusiveUpdateUpdateable(a5h.Elements[i]);
			}
		}

		protected void SolveUpdateable(L.h item, ref int activeConstraints)
		{
			if (!item.isActiveInSolver)
			{
				return;
			}
			L.b b2 = item.solverSettings;
			b2.a5h++;
			if (b2.a5h <= solver.a5b && b2.a5h <= b2.a5b)
			{
				if (item.SolveIteration() < b2.a5a)
				{
					b2.a57++;
					if (b2.a57 > b2.a56)
					{
						item.isActiveInSolver = false;
					}
					else
					{
						activeConstraints++;
					}
				}
				else
				{
					b2.a57 = 0;
					activeConstraints++;
				}
			}
			else
			{
				item.isActiveInSolver = false;
			}
		}

		public override float SolveIteration()
		{
			int activeConstraints = 0;
			for (int i = 0; i < a5h.a5h; i++)
			{
				SolveUpdateable(a5h.Elements[i], ref activeConstraints);
			}
			isActiveInSolver = activeConstraints > 0;
			return solverSettings.a5a + 1f;
		}

		protected void Add(L.h solverUpdateable)
		{
			if (solverUpdateable.solver == null)
			{
				if (solverUpdateable.SolverGroup == null)
				{
					a5h.Add(solverUpdateable);
					solverUpdateable.SolverGroup = this;
					solverUpdateable.Solver = solver;
					OnInvolvedEntitiesChanged();
					return;
				}
				throw new InvalidOperationException("Cannot add SolverUpdateable to SolverGroup; it already belongs to a SolverGroup.");
			}
			throw new InvalidOperationException("Cannot add SolverUpdateable to SolverGroup; it already belongs to a solver.");
		}

		protected void Remove(L.h solverUpdateable)
		{
			if (solverUpdateable.SolverGroup == this)
			{
				a5h.Remove(solverUpdateable);
				solverUpdateable.SolverGroup = null;
				solverUpdateable.Solver = null;
				OnInvolvedEntitiesChanged();
				return;
			}
			throw new InvalidOperationException("Cannot remove SolverUpdateable from SolverGroup; it doesn't belong to this SolverGroup.");
		}

		public override void OnAdditionToSpace(r.a newSpace)
		{
			for (int i = 0; i < a5h.Count; i++)
			{
				a5h[i].OnAdditionToSpace(newSpace);
			}
		}

		public override void OnRemovalFromSpace(r.a oldSpace)
		{
			for (int i = 0; i < a5h.Count; i++)
			{
				a5h[i].OnRemovalFromSpace(oldSpace);
			}
		}
	}
}
