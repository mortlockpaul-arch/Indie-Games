using System;
using System.Runtime.CompilerServices;
using _0004;
using E;
using L;
using P;
using T;
using Y;

namespace D
{
	internal abstract class b : h
	{
		protected internal int previousContactCount;

		protected internal float timeOfImpact = 1f;

		protected bool suppressEvents;

		[CompilerGenerated]
		private new B a5h;

		[CompilerGenerated]
		private r a5b;

		protected abstract P.h CollidableA { get; }

		protected abstract P.h CollidableB { get; }

		protected abstract E.h EntityA { get; }

		protected abstract E.h EntityB { get; }

		protected internal abstract int ContactCount { get; }

		public float TimeOfImpact => timeOfImpact;

		public bool SuppressEvents
		{
			get
			{
				return suppressEvents;
			}
			set
			{
				suppressEvents = value;
			}
		}

		public B Parent
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

		public r Contacts
		{
			[CompilerGenerated]
			get
			{
				return a5b;
			}
			[CompilerGenerated]
			private set
			{
				a5b = r2;
			}
		}

		protected b()
		{
			Contacts = new r(this);
		}

		public abstract void UpdateTimeOfImpact(P.h requester, float dt);

		public override void Initialize(global::Y.a entryA, global::Y.a entryB)
		{
			if (!suppressEvents)
			{
				CollidableA.EventTriggerer.OnPairCreated(CollidableB, this);
				CollidableB.EventTriggerer.OnPairCreated(CollidableA, this);
			}
		}

		protected internal override void OnAddedToNarrowPhase()
		{
			CollidableA.a56.Add(this);
			CollidableB.a56.Add(this);
		}

		protected virtual void OnContactAdded(_0004.h contact)
		{
			if (!suppressEvents)
			{
				CollidableA.EventTriggerer.OnContactCreated(CollidableB, this, contact);
				CollidableB.EventTriggerer.OnContactCreated(CollidableA, this, contact);
			}
			if (Parent != null)
			{
				Parent.OnContactAdded(contact);
			}
		}

		protected virtual void OnContactRemoved(_0004.h contact)
		{
			if (!suppressEvents)
			{
				CollidableA.EventTriggerer.OnContactRemoved(CollidableB, this, contact);
				CollidableB.EventTriggerer.OnContactRemoved(CollidableA, this, contact);
			}
			if (Parent != null)
			{
				Parent.OnContactRemoved(contact);
			}
		}

		public override void CleanUp()
		{
			if (previousContactCount > 0 && !suppressEvents)
			{
				CollidableA.EventTriggerer.OnCollisionEnded(CollidableB, this);
				CollidableB.EventTriggerer.OnCollisionEnded(CollidableA, this);
			}
			CollidableA.a56.Remove(this);
			CollidableB.a56.Remove(this);
			if (!suppressEvents)
			{
				CollidableA.EventTriggerer.OnPairRemoved(CollidableB);
				CollidableB.EventTriggerer.OnPairRemoved(CollidableA);
			}
			base.a5h = default(global::Y._7);
			base.NeedsUpdate = false;
			base.NarrowPhase = null;
			suppressEvents = false;
			timeOfImpact = 1f;
			Parent = null;
			previousContactCount = 0;
		}

		public abstract void UpdateMaterialProperties(global::T.b properties);

		public abstract void UpdateMaterialProperties(global::T._6 materialA, global::T._6 materialB);

		public void UpdateMaterialProperties()
		{
			UpdateMaterialProperties(null, null);
		}

		protected internal abstract void GetContactInformation(int index, out _0001 info);

		public virtual void ClearContacts()
		{
			previousContactCount = 0;
		}
	}
	internal interface B
	{
		void OnContactAdded(_0004.h contact);

		void OnContactRemoved(_0004.h contact);

		void AddSolverUpdateable(L.h addedItem);

		void RemoveSolverUpdateable(L.h removedItem);
	}
}
namespace d
{
	internal interface b : IDisposable
	{
		int ThreadCount { get; }

		void AddThread();

		void AddThread(Action<object> initialization, object initializationInformation);

		void EnqueueTask(Action<object> taskBody, object taskInformation);

		void ForLoop(int startIndex, int endIndex, Action<int> loopBody);

		void RemoveThread();

		void WaitForTaskCompletion();
	}
}
