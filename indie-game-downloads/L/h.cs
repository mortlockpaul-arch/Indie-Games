using System.Collections.Generic;
using System.Runtime.CompilerServices;
using E;
using G;
using Microsoft.Xna.Framework;
using Y;
using Z;
using l;
using p;
using u;

namespace L
{
	internal abstract class h : u.h
	{
		private class _00065h : IComparer<E.h>
		{
			int IComparer<E.h>.Compare(E.h x, E.h y)
			{
				if (x.GetHashCode() > y.GetHashCode())
				{
					return 1;
				}
				if (x.GetHashCode() < y.GetHashCode())
				{
					return -1;
				}
				return 0;
			}
		}

		protected internal readonly l._7<E.h> involvedEntities = new l._7<E.h>(2);

		protected internal int numberOfInvolvedEntities;

		private new static _00065h a5h = new _00065h();

		[CompilerGenerated]
		private G.h a5b;

		public l.X<E.h> InvolvedEntities => new l.X<E.h>(involvedEntities);

		public G.h SolverGroup
		{
			[CompilerGenerated]
			get
			{
				return a5b;
			}
			[CompilerGenerated]
			protected internal set
			{
				a5b = value;
			}
		}

		public override void EnterLock()
		{
			for (int i = 0; i < numberOfInvolvedEntities; i++)
			{
				if (involvedEntities.Elements[i].a5B)
				{
					involvedEntities.Elements[i].locker.Enter();
				}
			}
		}

		public override void ExitLock()
		{
			for (int num = numberOfInvolvedEntities - 1; num >= 0; num--)
			{
				if (involvedEntities.Elements[num].a5B)
				{
					involvedEntities.Elements[num].locker.Exit();
				}
			}
		}

		public override bool TryEnterLock()
		{
			for (int i = 0; i < numberOfInvolvedEntities; i++)
			{
				if (!involvedEntities.Elements[i].a5B || involvedEntities.Elements[i].locker.TryEnter())
				{
					continue;
				}
				for (i--; i >= 0; i--)
				{
					if (involvedEntities[i].a5B)
					{
						involvedEntities.Elements[i].locker.Exit();
					}
				}
				return false;
			}
			return true;
		}

		protected internal virtual void OnInvolvedEntitiesChanged()
		{
			bool flag = false;
			l._7<E.h> entityRawList = p._6.GetEntityRawList();
			CollectInvolvedEntities(entityRawList);
			if (entityRawList.a5h == involvedEntities.a5h)
			{
				for (int i = 0; i < entityRawList.Count; i++)
				{
					if (entityRawList.Elements[i] != involvedEntities.Elements[i])
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				for (int j = 0; j < involvedEntities.a5h; j++)
				{
					E.h h2 = involvedEntities.Elements[j];
					if (h2.a5B)
					{
						h2.a5L.Activate();
						break;
					}
				}
				CollectInvolvedEntities();
				if (SolverGroup != null)
				{
					SolverGroup.OnInvolvedEntitiesChanged();
				}
				for (int k = 0; k < involvedEntities.a5h; k++)
				{
					E.h h3 = involvedEntities.Elements[k];
					if (h3.a5B)
					{
						h3.a5L.Activate();
						break;
					}
				}
			}
			p._6.GiveBack(entityRawList);
		}

		protected internal void CollectInvolvedEntities()
		{
			involvedEntities.Clear();
			CollectInvolvedEntities(involvedEntities);
			SortInvolvedEntities();
			bX();
		}

		protected internal abstract void CollectInvolvedEntities(l._7<E.h> outputInvolvedEntities);

		protected internal void SortInvolvedEntities()
		{
			numberOfInvolvedEntities = involvedEntities.Count;
			involvedEntities.Sort(a5h);
		}

		private void bX()
		{
			Z._6 deactivationManager = simulationIslandConnection.DeactivationManager;
			if (deactivationManager != null)
			{
				simulationIslandConnection.Owner = null;
				deactivationManager.Remove(simulationIslandConnection);
			}
			else if (!simulationIslandConnection.SlatedForRemoval)
			{
				p._6.GiveBack(simulationIslandConnection);
			}
			simulationIslandConnection = p._6.GetSimulationIslandConnection();
			for (int i = 0; i < involvedEntities.a5h; i++)
			{
				simulationIslandConnection.a5h.Add(involvedEntities.Elements[i].a5L);
			}
			simulationIslandConnection.Owner = this;
			deactivationManager?.Add(simulationIslandConnection);
		}
	}
}
namespace l
{
	internal class h<T> where T : Y._6
	{
		internal abstract class _00065h
		{
			internal BoundingBox a5h;

			internal abstract bool IsLeaf { get; }

			internal abstract _00065h ChildA { get; }

			internal abstract _00065h ChildB { get; }

			internal abstract T Element { get; }

			internal abstract void y61P_00175(ref BoundingBox P_0, IList<T> P_1);

			internal abstract void y61P_00175(ref BoundingSphere P_0, IList<T> P_1);

			internal abstract void y61P_00175(ref BoundingFrustum P_0, IList<T> P_1);

			internal abstract void y61P_00175(ref Ray P_0, float P_1, IList<T> P_2);

			internal abstract void _6_0013<TElement>(l.h<TElement>._00065h P_0, IList<l._000E<T, TElement>> P_1) where TElement : Y._6;

			internal abstract bool _00138_0015_0006f5(_000656 P_0, out _00065h P_1);

			internal abstract void _0017_0017HY1(List<int> P_0, int P_1, ref int P_2);

			internal abstract void bQS_0001D5();
		}

		internal sealed class _00065b : _00065h
		{
			internal new _00065h a5h;

			internal _00065h a5b;

			internal override _00065h ChildA => a5h;

			internal override _00065h ChildB => a5b;

			internal override T Element => default(T);

			internal override bool IsLeaf => false;

			internal override void y61P_00175(ref BoundingBox P_0, IList<T> P_1)
			{
				a5h.a5h.Intersects(ref P_0, out var result);
				if (result)
				{
					a5h.y61P_00175(ref P_0, P_1);
				}
				a5b.a5h.Intersects(ref P_0, out result);
				if (result)
				{
					a5b.y61P_00175(ref P_0, P_1);
				}
			}

			internal override void y61P_00175(ref BoundingSphere P_0, IList<T> P_1)
			{
				a5h.a5h.Intersects(ref P_0, out var result);
				if (result)
				{
					a5h.y61P_00175(ref P_0, P_1);
				}
				a5b.a5h.Intersects(ref P_0, out result);
				if (result)
				{
					a5b.y61P_00175(ref P_0, P_1);
				}
			}

			internal override void y61P_00175(ref BoundingFrustum P_0, IList<T> P_1)
			{
				P_0.Intersects(ref a5h.a5h, out var result);
				if (result)
				{
					a5h.y61P_00175(ref P_0, P_1);
				}
				P_0.Intersects(ref a5b.a5h, out result);
				if (result)
				{
					a5b.y61P_00175(ref P_0, P_1);
				}
			}

			internal override void y61P_00175(ref Ray P_0, float P_1, IList<T> P_2)
			{
				P_0.Intersects(ref a5h.a5h, out var result);
				if (result.HasValue && result < P_1)
				{
					a5h.y61P_00175(ref P_0, P_1, P_2);
				}
				P_0.Intersects(ref a5b.a5h, out result);
				if (result.HasValue && result < P_1)
				{
					a5b.y61P_00175(ref P_0, P_1, P_2);
				}
			}

			internal override void _6_0013<TElement>(l.h<TElement>._00065h P_0, IList<l._000E<T, TElement>> P_1)
			{
				bool result;
				if (P_0.IsLeaf)
				{
					a5h.a5h.Intersects(ref P_0.a5h, out result);
					if (result)
					{
						a5h._6_0013(P_0, P_1);
					}
					a5b.a5h.Intersects(ref P_0.a5h, out result);
					if (result)
					{
						a5b._6_0013(P_0, P_1);
					}
					return;
				}
				l.h<TElement>._00065h obj = P_0.ChildA;
				l.h<TElement>._00065h obj2 = P_0.ChildB;
				a5h.a5h.Intersects(ref obj.a5h, out result);
				if (result)
				{
					a5h._6_0013(obj, P_1);
				}
				a5h.a5h.Intersects(ref obj2.a5h, out result);
				if (result)
				{
					a5h._6_0013(obj2, P_1);
				}
				a5b.a5h.Intersects(ref obj.a5h, out result);
				if (result)
				{
					a5b._6_0013(obj, P_1);
				}
				a5b.a5h.Intersects(ref obj2.a5h, out result);
				if (result)
				{
					a5b._6_0013(obj2, P_1);
				}
			}

			internal override bool _00138_0015_0006f5(_000656 P_0, out _00065h P_1)
			{
				BoundingBox.CreateMerged(ref a5h.a5h, ref P_0.a5h, out var result);
				BoundingBox.CreateMerged(ref a5b.a5h, ref P_0.a5h, out var result2);
				Vector3.Subtract(ref a5h.a5h.Max, ref a5h.a5h.Min, out var result3);
				float num = result3.X * result3.Y * result3.Z;
				Vector3.Subtract(ref a5b.a5h.Max, ref a5b.a5h.Min, out result3);
				float num2 = result3.X * result3.Y * result3.Z;
				Vector3.Subtract(ref result.Max, ref result.Min, out result3);
				float num3 = result3.X * result3.Y * result3.Z;
				Vector3.Subtract(ref result2.Max, ref result2.Min, out result3);
				float num4 = result3.X * result3.Y * result3.Z;
				if (num3 - num < num4 - num2)
				{
					if (a5h.IsLeaf)
					{
						a5h = new _00065b
						{
							a5h = result,
							a5h = a5h,
							a5b = P_0
						};
						P_1 = null;
						return true;
					}
					a5h.a5h = result;
					P_1 = a5h;
					return false;
				}
				if (a5b.IsLeaf)
				{
					a5b = new _00065b
					{
						a5h = result2,
						a5h = P_0,
						a5b = a5b
					};
					P_1 = null;
					return true;
				}
				a5b.a5h = result2;
				P_1 = a5b;
				return false;
			}

			public override string ToString()
			{
				return "{" + a5h.ToString() + ", " + a5b.ToString() + "}";
			}

			internal override void _0017_0017HY1(List<int> P_0, int P_1, ref int P_2)
			{
				P_2++;
				a5h._0017_0017HY1(P_0, P_1 + 1, ref P_2);
				a5b._0017_0017HY1(P_0, P_1 + 1, ref P_2);
			}

			internal override void bQS_0001D5()
			{
				a5h.bQS_0001D5();
				a5b.bQS_0001D5();
				BoundingBox.CreateMerged(ref a5h.a5h, ref a5b.a5h, out base.a5h);
			}
		}

		internal sealed class _000656 : _00065h
		{
			private new T a5h;

			internal override _00065h ChildA => null;

			internal override _00065h ChildB => null;

			internal override T Element => a5h;

			internal override bool IsLeaf => true;

			internal _000656(T P_0)
			{
				a5h = P_0;
				base.a5h = P_0.BoundingBox;
				base.a5h.Max.X += l.h<T>.LeafMargin;
				base.a5h.Max.Y += l.h<T>.LeafMargin;
				base.a5h.Max.Z += l.h<T>.LeafMargin;
				base.a5h.Min.X -= l.h<T>.LeafMargin;
				base.a5h.Min.Y -= l.h<T>.LeafMargin;
				base.a5h.Min.Z -= l.h<T>.LeafMargin;
			}

			internal override void y61P_00175(ref BoundingBox P_0, IList<T> P_1)
			{
				P_1.Add(a5h);
			}

			internal override void y61P_00175(ref BoundingSphere P_0, IList<T> P_1)
			{
				P_1.Add(a5h);
			}

			internal override void y61P_00175(ref BoundingFrustum P_0, IList<T> P_1)
			{
				P_1.Add(a5h);
			}

			internal override void y61P_00175(ref Ray P_0, float P_1, IList<T> P_2)
			{
				P_2.Add(a5h);
			}

			internal override void _6_0013<TElement>(l.h<TElement>._00065h P_0, IList<l._000E<T, TElement>> P_1)
			{
				if (P_0.IsLeaf)
				{
					P_1.Add(new l._000E<T, TElement>(a5h, P_0.Element));
					return;
				}
				l.h<TElement>._00065h obj = P_0.ChildA;
				l.h<TElement>._00065h obj2 = P_0.ChildB;
				base.a5h.Intersects(ref obj.a5h, out var result);
				if (result)
				{
					_6_0013(obj, P_1);
				}
				base.a5h.Intersects(ref obj2.a5h, out result);
				if (result)
				{
					_6_0013(obj2, P_1);
				}
			}

			internal override bool _00138_0015_0006f5(_000656 P_0, out _00065h P_1)
			{
				_00065b obj = new _00065b();
				BoundingBox.CreateMerged(ref base.a5h, ref ((_00065h)P_0).a5h, out ((_00065h)obj).a5h);
				obj.a5h = this;
				obj.a5b = P_0;
				P_1 = obj;
				return true;
			}

			public override string ToString()
			{
				return a5h.ToString();
			}

			internal override void _0017_0017HY1(List<int> P_0, int P_1, ref int P_2)
			{
				P_2++;
				P_0.Add(P_1);
			}

			internal override void bQS_0001D5()
			{
				base.a5h = a5h.BoundingBox;
				base.a5h.Max.X += l.h<T>.LeafMargin;
				base.a5h.Max.Y += l.h<T>.LeafMargin;
				base.a5h.Max.Z += l.h<T>.LeafMargin;
				base.a5h.Min.X -= l.h<T>.LeafMargin;
				base.a5h.Min.Y -= l.h<T>.LeafMargin;
				base.a5h.Min.Z -= l.h<T>.LeafMargin;
			}
		}

		private _00065h a5h;

		public static float LeafMargin = 0.001f;

		public BoundingBox BoundingBox
		{
			get
			{
				if (a5h != null)
				{
					return a5h.a5h;
				}
				return default(BoundingBox);
			}
		}

		public h(IList<T> elements)
		{
			Reconstruct(elements);
		}

		public void Reconstruct(IList<T> elements)
		{
			a5h = null;
			int count = elements.Count;
			for (int i = 0; i < count; i++)
			{
				Add(elements[(int)(1208299L * (long)i % count)]);
			}
		}

		public void Refit()
		{
			if (a5h != null)
			{
				a5h.bQS_0001D5();
			}
		}

		private void bt(out List<int> P_0, out int P_1, out int P_2, out int P_3)
		{
			P_0 = new List<int>();
			P_3 = 0;
			a5h._0017_0017HY1(P_0, 0, ref P_3);
			P_2 = 0;
			P_1 = int.MaxValue;
			for (int i = 0; i < P_0.Count; i++)
			{
				if (P_0[i] > P_2)
				{
					P_2 = P_0[i];
				}
				if (P_0[i] < P_1)
				{
					P_1 = P_0[i];
				}
			}
		}

		public void Add(T element)
		{
			_000656 obj = new _000656(element);
			if (a5h == null)
			{
				a5h = obj;
				return;
			}
			if (a5h.IsLeaf)
			{
				a5h._00138_0015_0006f5(obj, out a5h);
				return;
			}
			BoundingBox.CreateMerged(ref obj.a5h, ref a5h.a5h, out a5h.a5h);
			_00065h obj2 = a5h;
			while (!obj2._00138_0015_0006f5(obj, out obj2))
			{
			}
		}

		public bool GetOverlaps(BoundingBox boundingBox, IList<T> outputOverlappedElements)
		{
			if (a5h != null)
			{
				a5h.a5h.Intersects(ref boundingBox, out var result);
				if (result)
				{
					a5h.y61P_00175(ref boundingBox, outputOverlappedElements);
				}
			}
			return outputOverlappedElements.Count > 0;
		}

		public bool GetOverlaps(BoundingSphere boundingSphere, IList<T> outputOverlappedElements)
		{
			if (a5h != null)
			{
				a5h.a5h.Intersects(ref boundingSphere, out var result);
				if (result)
				{
					a5h.y61P_00175(ref boundingSphere, outputOverlappedElements);
				}
			}
			return outputOverlappedElements.Count > 0;
		}

		public bool GetOverlaps(BoundingFrustum boundingFrustum, IList<T> outputOverlappedElements)
		{
			if (a5h != null)
			{
				boundingFrustum.Intersects(ref a5h.a5h, out var result);
				if (result)
				{
					a5h.y61P_00175(ref boundingFrustum, outputOverlappedElements);
				}
			}
			return outputOverlappedElements.Count > 0;
		}

		public bool GetOverlaps(Ray ray, IList<T> outputOverlappedElements)
		{
			if (a5h != null)
			{
				ray.Intersects(ref a5h.a5h, out var result);
				if (result.HasValue)
				{
					a5h.y61P_00175(ref ray, float.MaxValue, outputOverlappedElements);
				}
			}
			return outputOverlappedElements.Count > 0;
		}

		public bool GetOverlaps(Ray ray, float maximumLength, IList<T> outputOverlappedElements)
		{
			if (a5h != null)
			{
				ray.Intersects(ref a5h.a5h, out var result);
				if (result.HasValue)
				{
					a5h.y61P_00175(ref ray, maximumLength, outputOverlappedElements);
				}
			}
			return outputOverlappedElements.Count > 0;
		}

		public bool GetOverlaps<TElement>(l.h<TElement> tree, IList<l._000E<T, TElement>> outputOverlappedElements) where TElement : Y._6
		{
			a5h.a5h.Intersects(ref tree.a5h.a5h, out var result);
			if (result)
			{
				a5h._6_0013(tree.a5h, outputOverlappedElements);
			}
			return outputOverlappedElements.Count > 0;
		}
	}
}
