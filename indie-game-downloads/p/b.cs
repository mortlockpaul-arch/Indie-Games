using System;
using System.Collections;
using System.Collections.Generic;
using d;

namespace P
{
	internal struct b(h owner) : IList<h>, ICollection<h>, IEnumerable<h>, IEnumerable
	{
		public struct _00065h(b collection) : IEnumerator<h>, IDisposable, IEnumerator
		{
			private b a5h = collection;

			private int a5b = -1;

			public h Current => a5h[a5b];

			object IEnumerator.Current => Current;

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				return ++a5b < a5h.Count;
			}

			public void Reset()
			{
				a5b = -1;
			}
		}

		internal h a5h = owner;

		public h this[int index]
		{
			get
			{
				return (h)((a5h.a56[index].a5h.a5h == a5h) ? a5h.a56[index].a5h.a5b : a5h.a56[index].a5h.a5h);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public int Count => a5h.a56.Count;

		bool ICollection<h>.IsReadOnly => true;

		public _00065h GetEnumerator()
		{
			return new _00065h(this);
		}

		IEnumerator<h> IEnumerable<h>.GetEnumerator()
		{
			return new _00065h(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new _00065h(this);
		}

		public int IndexOf(h item)
		{
			for (int i = 0; i < Count; i++)
			{
				if (item == this[i])
				{
					return i;
				}
			}
			return -1;
		}

		public bool Contains(h item)
		{
			for (int i = 0; i < Count; i++)
			{
				if (item == this[i])
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(h[] array, int arrayIndex)
		{
			for (int i = 0; i < Count; i++)
			{
				array[arrayIndex + i] = this[i];
			}
		}

		bool ICollection<h>.Remove(h item)
		{
			throw new NotSupportedException();
		}

		void ICollection<h>.Add(h item)
		{
			throw new NotSupportedException();
		}

		void ICollection<h>.Clear()
		{
			throw new NotSupportedException();
		}

		void IList<h>.Insert(int index, h item)
		{
			throw new NotSupportedException();
		}

		void IList<h>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
	}
}
namespace p
{
	internal class b<T> : p.h<T> where T : class, new()
	{
		private readonly d.h<T> a5h;

		public override int Count => a5h.Count;

		public b(int initialResourceCount, Action<T> initializer)
		{
			base.InstanceInitializer = initializer;
			a5h = new d.h<T>(initialResourceCount);
			Initialize(initialResourceCount);
		}

		public b(int initialResourceCount)
			: this(initialResourceCount, (Action<T>)null)
		{
		}

		public b()
			: this(10)
		{
		}

		public override void GiveBack(T item)
		{
			a5h.Enqueue(item);
		}

		public override void Initialize(int initialResourceCount)
		{
			while (a5h.Count > initialResourceCount)
			{
				a5h.TryUnsafeDequeueFirst(out var _);
			}
			int num = a5h.a57 - a5h.a5a + 1;
			if (base.InstanceInitializer != null)
			{
				for (int i = 0; i < num; i++)
				{
					base.InstanceInitializer(a5h.a5b[(a5h.a5a + i) % a5h.a5b.Length]);
				}
			}
			while (a5h.Count < initialResourceCount)
			{
				a5h.UnsafeEnqueue(CreateNewResource());
			}
		}

		public override T Take()
		{
			if (a5h.TryDequeueFirst(out var item))
			{
				return item;
			}
			return CreateNewResource();
		}

		public override void Clear()
		{
			while (a5h.Count > 0)
			{
				a5h.TryDequeueFirst(out var _);
			}
		}
	}
}
