using System;
using System.Runtime.CompilerServices;
using _0002;
using _000E;
using D;
using Y;
using l;
using q;

namespace P
{
	internal abstract class h : Y.a
	{
		internal new _000E.h a5h;

		private Action<_000E.h> a5b;

		internal new l._7<D.b> a56 = new l._7<D.b>();

		[CompilerGenerated]
		private bool a5a;

		public _000E.h Shape
		{
			get
			{
				return a5h;
			}
			protected set
			{
				if (a5h != null)
				{
					a5h.ShapeChanged -= a5b;
				}
				a5h = value;
				if (a5h != null)
				{
					a5h.ShapeChanged += a5b;
				}
				OnShapeChanged(a5h);
			}
		}

		protected internal abstract _0002._000E EventTriggerer { get; }

		public bool IgnoreShapeChanges
		{
			[CompilerGenerated]
			get
			{
				return a5a;
			}
			[CompilerGenerated]
			set
			{
				a5a = value;
			}
		}

		public l.X<D.b> Pairs => new l.X<D.b>(a56);

		public b OverlappedCollidables => new b(this);

		protected h()
		{
			a5b = OnShapeChanged;
		}

		protected virtual void OnShapeChanged(_000E.h collisionShape)
		{
		}

		protected override void CollisionRulesUpdated()
		{
			for (int i = 0; i < a56.Count; i++)
			{
				a56[i].CollisionRule = q._7.CollisionRuleCalculator(a56[i].BroadPhaseOverlap.a5h.a56, a56[i].BroadPhaseOverlap.a5b.a56);
			}
		}
	}
}
namespace p
{
	internal abstract class h<T> where T : class, new()
	{
		[CompilerGenerated]
		private Action<T> a5h;

		public abstract int Count { get; }

		public Action<T> InstanceInitializer
		{
			[CompilerGenerated]
			get
			{
				return a5h;
			}
			[CompilerGenerated]
			set
			{
				a5h = value;
			}
		}

		public abstract void GiveBack(T item);

		public abstract void Initialize(int initialResourceCount);

		public abstract T Take();

		protected T CreateNewResource()
		{
			T val = new T();
			if (InstanceInitializer != null)
			{
				InstanceInitializer(val);
			}
			return val;
		}

		public abstract void Clear();
	}
}
