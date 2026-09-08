using System;
using System.Runtime.CompilerServices;
using J;
using Y;
using q;

namespace D
{
	internal abstract class h
	{
		internal global::Y._7 a5h;

		[CompilerGenerated]
		private bool a5b;

		[CompilerGenerated]
		private J._7 a56;

		[CompilerGenerated]
		private J.b a5a;

		public bool NeedsUpdate
		{
			[CompilerGenerated]
			get
			{
				return a5b;
			}
			[CompilerGenerated]
			set
			{
				a5b = value;
			}
		}

		public global::q.a CollisionRule
		{
			get
			{
				return a5h.a56;
			}
			set
			{
				a5h.a56 = value;
			}
		}

		public global::Y._7 BroadPhaseOverlap
		{
			get
			{
				return a5h;
			}
			set
			{
				a5h = value;
				Initialize(value.a5h, value.a5b);
			}
		}

		public J._7 Factory
		{
			[CompilerGenerated]
			get
			{
				return a56;
			}
			[CompilerGenerated]
			internal set
			{
				a56 = obj;
			}
		}

		public J.b NarrowPhase
		{
			[CompilerGenerated]
			get
			{
				return a5a;
			}
			[CompilerGenerated]
			internal set
			{
				a5a = b2;
			}
		}

		public abstract void UpdateCollision(float dt);

		public abstract void Initialize(global::Y.a entryA, global::Y.a entryB);

		protected internal abstract void OnAddedToNarrowPhase();

		public abstract void CleanUp();
	}
}
namespace d
{
	internal class h<T>
	{
		private readonly v a5h = new v();

		internal T[] a5b;

		private int a56;

		internal int a5a;

		internal int a57 = -1;

		public int Count => a56;

		public h(int capacity)
		{
			a5b = new T[capacity];
		}

		public h()
			: this(16)
		{
		}

		public override string ToString()
		{
			return "Count: " + a56;
		}

		public void Enqueue(T item)
		{
			a5h.Enter();
			try
			{
				if (a56 == a5b.Length)
				{
					T[] array = a5b;
					a5b = new T[Math.Max(4, array.Length * 2)];
					Array.Copy(array, a5a, a5b, 0, array.Length - a5a);
					Array.Copy(array, 0, a5b, array.Length - a5a, a5a);
					a5a = 0;
					a57 = a56 - 1;
				}
				int num = (a57 + 1) % a5b.Length;
				a5b[num] = item;
				a57 = num;
				a56++;
			}
			finally
			{
				a5h.Exit();
			}
		}

		public bool TryDequeueFirst(out T item)
		{
			a5h.Enter();
			try
			{
				if (a56 > 0)
				{
					item = a5b[a5a];
					a5b[a5a] = default(T);
					a5a = (a5a + 1) % a5b.Length;
					a56--;
					return true;
				}
				item = default(T);
				return false;
			}
			finally
			{
				a5h.Exit();
			}
		}

		public bool TryDequeueLast(out T item)
		{
			a5h.Enter();
			try
			{
				if (a56 > 0)
				{
					item = a5b[a57];
					a5b[a57] = default(T);
					a57--;
					if (a57 < 0)
					{
						a57 += a5b.Length;
					}
					a56--;
					return true;
				}
				item = default(T);
				return false;
			}
			finally
			{
				a5h.Exit();
			}
		}

		public bool TryUnsafeDequeueFirst(out T item)
		{
			if (a56 > 0)
			{
				item = a5b[a5a];
				a5b[a5a] = default(T);
				a5a = (a5a + 1) % a5b.Length;
				a56--;
				return true;
			}
			item = default(T);
			return false;
		}

		public bool TryUnsafeDequeueLast(out T item)
		{
			if (a56 > 0)
			{
				item = a5b[a57];
				a5b[a57] = default(T);
				a57--;
				if (a57 < 0)
				{
					a57 += a5b.Length;
				}
				a56--;
				return true;
			}
			item = default(T);
			return false;
		}

		public void UnsafeEnqueue(T item)
		{
			if (a56 == a5b.Length)
			{
				T[] array = a5b;
				a5b = new T[array.Length * 2];
				Array.Copy(array, a5a, a5b, 0, array.Length - a5a);
				Array.Copy(array, 0, a5b, array.Length - a5a, a5a);
				a5a = 0;
				a57 = a56 - 1;
			}
			int num = (a57 + 1) % a5b.Length;
			a5b[num] = item;
			a57 = num;
			a56++;
		}
	}
}
