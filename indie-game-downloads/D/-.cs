using System;
using System.Collections.Generic;
using System.Threading;
using _0004;
using E;
using Microsoft.Xna.Framework;
using P;
using Y;
using i;
using l;
using p;
using s;
using y;

namespace D
{
	internal class _0006 : _7
	{
		private new s.B<global::y._6> a5h;

		private s.B<global::y._6> a5b;

		private i.b a56 = new i.b();

		protected override P.h CollidableA => a5h;

		protected override P.h CollidableB => a5b;

		protected override E.h EntityA => a5h.entity;

		protected override E.h EntityB => a5b.entity;

		public override i.h ContactManifold => a56;

		public override void Initialize(global::Y.a entryA, global::Y.a entryB)
		{
			a5h = entryA as s.B<global::y._6>;
			a5b = entryB as s.B<global::y._6>;
			if (a5h == null || a5b == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
			base.Initialize(entryA, entryB);
		}

		public override void CleanUp()
		{
			base.CleanUp();
			a5h = null;
			a5b = null;
		}
	}
	internal abstract class _0018 : X
	{
		protected s._6 compoundInfo;

		protected override P.h CollidableA => compoundInfo;

		protected override E.h EntityA => compoundInfo.entity;

		public override void Initialize(global::Y.a entryA, global::Y.a entryB)
		{
			compoundInfo = entryA as s._6;
			if (compoundInfo == null)
			{
				compoundInfo = entryB as s._6;
				if (compoundInfo == null)
				{
					throw new Exception("Inappropriate types used to initialize pair.");
				}
			}
			base.Initialize(entryA, entryB);
		}

		public override void CleanUp()
		{
			base.CleanUp();
			compoundInfo = null;
		}
	}
	internal class _0002 : D._0018
	{
		private new s._6 a5h;

		private l._7<l._000E<s._7, s._7>> a5b = new l._7<l._000E<s._7, s._7>>();

		protected override P.h CollidableB => a5h;

		protected override E.h EntityB => a5h.entity;

		public override void Initialize(global::Y.a entryA, global::Y.a entryB)
		{
			a5h = entryB as s._6;
			if (a5h == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
			base.Initialize(entryA, entryB);
		}

		public override void CleanUp()
		{
			base.CleanUp();
			a5h = null;
		}

		protected override void UpdateContainedPairs()
		{
			compoundInfo.a5b.Tree.GetOverlaps(a5h.a5b.Tree, a5b);
			for (int i = 0; i < a5b.a5h; i++)
			{
				l._000E<s._7, s._7> obj = a5b.Elements[i];
				TryToAdd(obj.OverlapA.CollisionInformation, obj.OverlapB.CollisionInformation, obj.OverlapA.Material, obj.OverlapB.Material);
			}
			a5b.Clear();
		}
	}
	internal class _000E : D._0018
	{
		private new P._7 a5h;

		protected override P.h CollidableB => a5h;

		protected override E.h EntityB => null;

		public override void Initialize(global::Y.a entryA, global::Y.a entryB)
		{
			a5h = entryA as P._7;
			if (a5h == null)
			{
				a5h = entryB as P._7;
				if (a5h == null)
				{
					throw new Exception("Inappropriate types used to initialize pair.");
				}
			}
			base.Initialize(entryA, entryB);
		}

		public override void CleanUp()
		{
			base.CleanUp();
			a5h = null;
		}

		protected override void UpdateContainedPairs()
		{
			l._7<s._7> compoundChildList = p._6.GetCompoundChildList();
			compoundInfo.a5b.Tree.GetOverlaps(a5h.boundingBox, compoundChildList);
			for (int i = 0; i < compoundChildList.a5h; i++)
			{
				TryToAdd(compoundChildList.Elements[i].CollisionInformation, a5h, compoundChildList.Elements[i].Material);
			}
			p._6.GiveBack(compoundChildList);
		}
	}
	internal struct _0001 : IEquatable<_0001>
	{
		public _0004.h Contact;

		public b Pair;

		public float NormalForce;

		public float FrictionForce;

		public Vector3 RelativeVelocity;

		public override string ToString()
		{
			return string.Concat(Contact, " NormalForce: ", NormalForce, " FrictionForce: ", FrictionForce, " RelativeVelocity: ", RelativeVelocity);
		}

		public bool Equals(_0001 other)
		{
			return other.Contact == Contact;
		}
	}
	internal class _000F : _7
	{
		private new s.v a5h;

		private s.v a5b;

		private i.a a56 = new i.a();

		protected override P.h CollidableA => a5h;

		protected override P.h CollidableB => a5b;

		public override i.h ContactManifold => a56;

		protected override E.h EntityA => a5h.entity;

		protected override E.h EntityB => a5b.entity;

		public override void Initialize(global::Y.a entryA, global::Y.a entryB)
		{
			a5h = entryA as s.v;
			a5b = entryB as s.v;
			if (a5h == null || a5b == null)
			{
				throw new Exception("Inappropriate types used to initialize pair.");
			}
			base.Initialize(entryA, entryB);
		}

		public override void CleanUp()
		{
			base.CleanUp();
			a5h = null;
			a5b = null;
		}
	}
}
namespace d
{
	internal class _0006 : b, IDisposable
	{
		private struct _00065h
		{
			internal readonly object a5h;

			internal readonly Action<object> a5b;

			internal _00065h(Action<object> P_0, object P_1)
			{
				a5b = P_0;
				a5h = P_1;
			}
		}

		private class _00065b
		{
			internal readonly int a5h;

			internal readonly int a5b;

			internal int a56;

			internal Action<int> a5a;

			internal _00065b(int P_0, int P_1)
			{
				a5h = P_0;
				a5b = P_1;
			}
		}

		private class _000656 : IDisposable
		{
			private readonly object a5h = new object();

			private readonly object a5b;

			private readonly _0006 a56;

			private readonly d.h<_00065h> a5a;

			private readonly Thread a57;

			private readonly Action<object> a5_0006;

			private bool a5v;

			private int a5B;

			private AutoResetEvent a5X = new AutoResetEvent(initialState: false);

			internal _000656(int P_0, _0006 P_1, Action<object> P_2, object P_3)
			{
				a56 = P_1;
				a57 = new Thread(az);
				a57.IsBackground = true;
				a5a = new d.h<_00065h>();
				a5_0006 = P_2;
				a5b = P_3;
				_6k(P_0);
				a57.Start();
			}

			~_000656()
			{
				Dispose();
			}

			public void Dispose()
			{
				lock (a5h)
				{
					if (!a5v)
					{
						a5v = true;
						a5X.Close();
						a5X = null;
						a56.a5a.Remove(this);
						GC.SuppressFinalize(this);
					}
				}
			}

			internal void au(Action<object> P_0, object P_1)
			{
				Interlocked.Increment(ref a56.a5X);
				a5a.Enqueue(new _00065h(P_0, P_1));
				a5X.Set();
			}

			private bool aL(int P_0, out _00065h P_1)
			{
				return a56.a5a[P_0].a5a.TryDequeueLast(out P_1);
			}

			private void az()
			{
				if (a5_0006 != null)
				{
					a5_0006(a5b);
				}
				while (true)
				{
					a5X.WaitOne();
					while (true)
					{
						if (!a5a.TryDequeueFirst(out var item))
						{
							bool flag = false;
							for (int i = 1; i < a56.a5a.Count; i++)
							{
								if (aL((a5B + i) % a56.a5a.Count, out item))
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								break;
							}
						}
						try
						{
							if (item.a5b != null)
							{
								item.a5b(item.a5h);
							}
							if (Interlocked.Decrement(ref a56.a5X) == 0)
							{
								a56.a57.Set();
							}
							if (item.a5b == null)
							{
								return;
							}
						}
						catch (ArithmeticException innerException)
						{
							throw new ArithmeticException("Some internal multithreaded arithmetic has encountered an invalid state.  Check for invalid entity momentums, velocities, and positions; propagating NaN's will generally trigger this exception in the getExtremePoint function.", innerException);
						}
					}
				}
			}

			private void _6k(int P_0)
			{
				a5B = P_0;
			}
		}

		private readonly object a5h = new object();

		private readonly Action<object> a5b;

		private readonly List<_00065b> a56 = new List<_00065b>();

		private readonly List<_000656> a5a = new List<_000656>();

		private ManualResetEvent a57 = new ManualResetEvent(initialState: false);

		private int a5_0006;

		private bool a5v;

		private int a5B;

		private int a5X = 1;

		public int LoopTasksPerThread
		{
			get
			{
				return a5B;
			}
			set
			{
				a5B = value;
				aZ();
			}
		}

		public int ThreadCount => a5a.Count;

		public _0006()
		{
			LoopTasksPerThread = 1;
			a5b = a_000F;
		}

		~_0006()
		{
			Dispose();
		}

		public void WaitForTaskCompletion()
		{
			if (Interlocked.Decrement(ref a5X) == 0)
			{
				a57.Set();
			}
			a57.WaitOne();
			a5X = 1;
			a57.Reset();
		}

		public void AddThread()
		{
			AddThread(null, null);
		}

		public void AddThread(Action<object> initialization, object initializationInformation)
		{
			lock (a5a)
			{
				_000656 item = new _000656(a5a.Count, this, initialization, initializationInformation);
				a5a.Add(item);
				aZ();
			}
		}

		public void RemoveThread()
		{
			if (a5a.Count > 0)
			{
				a5a[0].au(null, null);
				WaitForTaskCompletion();
				a5a[0].Dispose();
			}
		}

		public void EnqueueTask(Action<object> task, object taskInformation)
		{
			lock (a5a)
			{
				a5_0006 = (a5_0006 + 1) % a5a.Count;
				a5a[a5_0006].au(task, taskInformation);
			}
		}

		public void ForLoop(int startIndex, int endIndex, Action<int> loopBody)
		{
			int num = a5a.Count * a5B;
			int num2 = endIndex - startIndex;
			for (int i = 0; i < num; i++)
			{
				a56[i].a5a = loopBody;
				a56[i].a56 = num2;
				EnqueueTaskSequentially(a5b, a56[i]);
			}
			WaitForTaskCompletion();
		}

		public void Dispose()
		{
			lock (a5h)
			{
				if (!a5v)
				{
					a5v = true;
					while (a5a.Count > 0)
					{
						RemoveThread();
					}
					a57.Close();
					a57 = null;
				}
			}
		}

		public void EnqueueTaskSequentially(Action<object> task, object taskInformation)
		{
			a5a[a5_0006].au(task, taskInformation);
			a5_0006 = (a5_0006 + 1) % a5a.Count;
		}

		private static void a_000F(object P_0)
		{
			_00065b obj = P_0 as _00065b;
			int num = obj.a56 * (obj.a5h + 1) / obj.a5b;
			for (int i = obj.a56 * obj.a5h / obj.a5b; i < num; i++)
			{
				obj.a5a(i);
			}
		}

		private void aZ()
		{
			a56.Clear();
			int count = a5a.Count;
			int num = count * a5B;
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < a5B; j++)
				{
					a56.Add(new _00065b(i * a5B + j, num));
				}
			}
		}
	}
}
