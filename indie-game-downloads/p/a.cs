using System;
using System.Collections;
using System.Collections.Generic;
using E;
using s;

namespace P
{
	internal struct a(s.b owner) : IEnumerable<E.h>, IEnumerable
	{
		public struct _00065h(a collection) : IEnumerator<E.h>, IDisposable, IEnumerator
		{
			private a a5h = collection;

			private s.b a5b = null;

			private int a56 = -1;

			public E.h Current => a5b.entity;

			object IEnumerator.Current => Current;

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				while (++a56 < a5h.a5h.a56.Count)
				{
					if ((a5b = ((a5h.a5h.a56[a56].a5h.a5h == a5h.a5h) ? a5h.a5h.a56[a56].a5h.a5b : a5h.a5h.a56[a56].a5h.a5h) as s.b) != null)
					{
						return true;
					}
				}
				return false;
			}

			public void Reset()
			{
				a56 = -1;
			}
		}

		internal s.b a5h = owner;

		public _00065h GetEnumerator()
		{
			return new _00065h(this);
		}

		IEnumerator<E.h> IEnumerable<E.h>.GetEnumerator()
		{
			return new _00065h(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new _00065h(this);
		}
	}
}
namespace p
{
	internal class a<T> : p.h<T> where T : class, new()
	{
		private readonly Stack<T> a5h;

		public override int Count => a5h.Count;

		public a(int initialResourceCount, Action<T> initializer)
		{
			base.InstanceInitializer = initializer;
			a5h = new Stack<T>(initialResourceCount);
			Initialize(initialResourceCount);
		}

		public a(int initialResourceCount)
			: this(initialResourceCount, (Action<T>)null)
		{
		}

		public a()
			: this(10)
		{
		}

		public override void GiveBack(T item)
		{
			a5h.Push(item);
		}

		public override void Initialize(int initialResourceCount)
		{
			while (a5h.Count > initialResourceCount)
			{
				a5h.Pop();
			}
			if (base.InstanceInitializer != null)
			{
				foreach (T item in a5h)
				{
					base.InstanceInitializer(item);
				}
			}
			while (a5h.Count < initialResourceCount)
			{
				a5h.Push(CreateNewResource());
			}
		}

		public override T Take()
		{
			if (a5h.Count > 0)
			{
				return a5h.Pop();
			}
			return CreateNewResource();
		}

		public override void Clear()
		{
			a5h.Clear();
		}
	}
}
