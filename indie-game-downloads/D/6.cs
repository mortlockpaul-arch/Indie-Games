using System;
using System.Collections.Generic;
using System.Threading;
using _0002;
using _0004;
using _0010;
using P;
using T;
using Y;
using i;

namespace D
{
	internal abstract class _6 : b
	{
		public abstract i.h ContactManifold { get; }

		public abstract _0010.b ContactConstraint { get; }

		protected internal override int ContactCount => ContactManifold.contacts.a5h;

		protected _6()
		{
			ContactManifold.ContactAdded += OnContactAdded;
			ContactManifold.ContactRemoved += OnContactRemoved;
		}

		protected override void OnContactAdded(_0004.h contact)
		{
			ContactConstraint.AddContact(contact);
			base.OnContactAdded(contact);
		}

		protected override void OnContactRemoved(_0004.h contact)
		{
			ContactConstraint.RemoveContact(contact);
			base.OnContactRemoved(contact);
		}

		public override void Initialize(global::Y.a entryA, global::Y.a entryB)
		{
			ContactManifold.Initialize(CollidableA, CollidableB);
			ContactConstraint.Initialize(EntityA, EntityB, this);
			base.Initialize(entryA, entryB);
		}

		public override void UpdateMaterialProperties(global::T._6 a, global::T._6 b)
		{
			ContactConstraint.UpdateMaterialProperties((a != null) ? a : ((EntityA == null) ? null : EntityA.a5Z), (b != null) ? b : ((EntityB == null) ? null : EntityB.a5Z));
		}

		public override void UpdateMaterialProperties(global::T.b properties)
		{
			ContactConstraint.MaterialInteraction = properties;
		}

		public override void CleanUp()
		{
			for (int num = ContactManifold.contacts.a5h - 1; num >= 0; num--)
			{
				OnContactRemoved(ContactManifold.contacts[num]);
			}
			if (ContactConstraint.solver != null)
			{
				ContactConstraint.pair = null;
				if (base.Parent != null)
				{
					base.Parent.RemoveSolverUpdateable(ContactConstraint);
				}
				else if (base.NarrowPhase != null)
				{
					base.NarrowPhase.NotifyUpdateableRemoved(ContactConstraint);
				}
			}
			else
			{
				ContactConstraint.CleanUpReferences();
				if (base.Parent != null && ContactConstraint.SolverGroup != null)
				{
					base.Parent.RemoveSolverUpdateable(ContactConstraint);
				}
			}
			ContactConstraint.CleanUp();
			base.CleanUp();
			ContactManifold.CleanUp();
		}

		public override void UpdateCollision(float dt)
		{
			P.h collidableA = CollidableA;
			P.h collidableB = CollidableB;
			global::_0002._000E eventTriggerer = collidableA.EventTriggerer;
			global::_0002._000E eventTriggerer2 = collidableB.EventTriggerer;
			if (!suppressEvents)
			{
				eventTriggerer.OnPairUpdated(collidableB, this);
				eventTriggerer2.OnPairUpdated(collidableA, this);
			}
			ContactManifold.Update(dt);
			if (ContactManifold.contacts.a5h > 0)
			{
				if (!suppressEvents)
				{
					eventTriggerer.OnPairTouching(collidableB, this);
					eventTriggerer2.OnPairTouching(collidableA, this);
				}
				if (previousContactCount == 0)
				{
					if (base.Parent != null)
					{
						base.Parent.AddSolverUpdateable(ContactConstraint);
					}
					else if (base.NarrowPhase != null)
					{
						base.NarrowPhase.NotifyUpdateableAdded(ContactConstraint);
					}
					if (!suppressEvents)
					{
						eventTriggerer.OnInitialCollisionDetected(collidableB, this);
						eventTriggerer2.OnInitialCollisionDetected(collidableA, this);
					}
				}
			}
			else if (previousContactCount > 0)
			{
				if (base.Parent != null)
				{
					base.Parent.RemoveSolverUpdateable(ContactConstraint);
				}
				else if (base.NarrowPhase != null)
				{
					base.NarrowPhase.NotifyUpdateableRemoved(ContactConstraint);
				}
				if (!suppressEvents)
				{
					eventTriggerer.OnCollisionEnded(collidableB, this);
					eventTriggerer2.OnCollisionEnded(collidableA, this);
				}
			}
			previousContactCount = ContactManifold.contacts.a5h;
		}

		public override void ClearContacts()
		{
			if (previousContactCount > 0)
			{
				if (base.Parent != null)
				{
					base.Parent.RemoveSolverUpdateable(ContactConstraint);
				}
				else if (base.NarrowPhase != null)
				{
					base.NarrowPhase.NotifyUpdateableRemoved(ContactConstraint);
				}
				if (!suppressEvents)
				{
					P.h collidableA = CollidableA;
					P.h collidableB = CollidableB;
					collidableA.EventTriggerer.OnCollisionEnded(collidableB, this);
					collidableB.EventTriggerer.OnCollisionEnded(collidableA, this);
				}
			}
			ContactManifold.ClearContacts();
			base.ClearContacts();
		}
	}
}
namespace d
{
	internal class _6 : IDisposable
	{
		private readonly AutoResetEvent a5h;

		private int a5b;

		internal List<a> a56 = new List<a>();

		internal int a5a;

		internal int a57;

		internal Action<int> a5_0006;

		internal int a5v;

		private int a5B = 3;

		private int a5X = 80;

		internal int a5_0018;

		internal int a5W;

		private bool a5_0002;

		private readonly object a5_000E = new object();

		public int MinimumTasksPerThread
		{
			get
			{
				return a5B;
			}
			set
			{
				a5B = value;
			}
		}

		public int MaximumIterationsPerTask
		{
			get
			{
				return a5X;
			}
			set
			{
				a5X = value;
			}
		}

		public _6()
		{
			a5h = new AutoResetEvent(initialState: false);
		}

		internal void a_000E()
		{
			a_000E(null, null);
		}

		internal void a_000E(Action<object> P_0, object P_1)
		{
			a56.Add(new a(this, P_0, P_1));
		}

		internal void ay()
		{
			if (a56.Count <= 0)
			{
				return;
			}
			lock (a56[0].a56)
			{
				if (!a56[0].a5b)
				{
					a5_0006 = null;
					a5b = 1;
					a56[0].a57.Set();
					a5h.WaitOne();
					a56[0].Dispose();
				}
			}
			a56.RemoveAt(0);
		}

		public void ForLoop(int beginIndex, int endIndex, Action<int> loopBody)
		{
			a5b = a56.Count;
			int num = endIndex - beginIndex;
			int num2 = Math.Max(a5B, num / a5X);
			int num3 = a5b * num2;
			a5a = beginIndex;
			a57 = endIndex;
			a5_0006 = loopBody;
			a5v = Math.Max(1, num / num3);
			a5_0018 = 0;
			float num4 = (float)num / (float)a5v;
			if (num4 % 1f == 0f)
			{
				a5W = (int)num4;
			}
			else
			{
				a5W = 1 + (int)num4;
			}
			for (int i = 0; i < a56.Count; i++)
			{
				a56[i].a5a = endIndex;
				a56[i].a5v = a5v;
				a56[i].a57.Set();
			}
			a5h.WaitOne();
		}

		internal void ar()
		{
			if (Interlocked.Decrement(ref a5b) == 0)
			{
				a5h.Set();
			}
		}

		public void Dispose()
		{
			lock (a5_000E)
			{
				if (!a5_0002)
				{
					a5_0002 = true;
					while (a56.Count > 0)
					{
						ay();
					}
					a5h.Close();
					GC.SuppressFinalize(this);
				}
			}
		}

		~_6()
		{
			Dispose();
		}
	}
}
