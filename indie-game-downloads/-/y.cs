using _0004;
using D;
using Microsoft.Xna.Framework;
using P;
using d;
using r;

namespace _000F
{
	internal struct y
	{
		public Vector3 Position;

		public Vector3 Normal;

		public bool HasTraction;

		public float Depth;

		public P.h SupportObject;
	}
}
namespace _0002
{
	internal class y<T> : global::_0002.b<T>, _000E, h where T : P.h
	{
		private a a5h;

		private _6 a5b;

		private readonly d.h<_0002> a56 = new d.h<_0002>(0);

		public a DirectDispatchPairTouchedEventHandler
		{
			get
			{
				return a5h;
			}
			set
			{
				a5h = value;
				if (value != null)
				{
					AddToEventfuls();
				}
				else
				{
					VerifyEventStatus();
				}
			}
		}

		public _6 DirectDispatchDetectingInitialCollisionEventHandler
		{
			get
			{
				return a5b;
			}
			set
			{
				a5b = value;
				if (value != null)
				{
					AddToEventfuls();
				}
				else
				{
					VerifyEventStatus();
				}
			}
		}

		public y(T owner)
			: base(owner)
		{
		}

		public y()
			: base((T)null)
		{
		}

		protected override bool EventsAreInactive()
		{
			if (a5h == null && a5b == null)
			{
				return base.EventsAreInactive();
			}
			return false;
		}

		protected override void DispatchEvents()
		{
			if (a5h != null)
			{
				_0002 item;
				while (a56.TryUnsafeDequeueFirst(out item))
				{
					a5h.OnPairTouched(owner, item.a5b, item.a5h);
				}
			}
			base.DispatchEvents();
		}

		public void OnCollisionEnded(P.h other, D.b collisionPair)
		{
		}

		public void OnPairTouching(P.h other, D.b collisionPair)
		{
			if (a5h != null)
			{
				a56.Enqueue(new _0002(other, collisionPair));
			}
		}

		public void OnContactCreated(P.h other, D.b collisionPair, _0004.h contact)
		{
		}

		public void OnContactRemoved(P.h other, D.b collisionPair, _0004.h contact)
		{
		}

		public void OnInitialCollisionDetected(P.h other, D.b collisionPair)
		{
			if (a5b != null)
			{
				a5b.OnDetectingInitialCollision(owner, other, collisionPair);
			}
		}

		public override void RemoveAllEvents()
		{
			base.RemoveAllEvents();
		}
	}
}
namespace _0001
{
	internal class y : global::_0001._0018<_0006>
	{
		public y(global::r.B timeStepSettings)
			: base(timeStepSettings)
		{
		}

		public y(global::r.B timeStepSettings, d.b threadManager)
			: base(timeStepSettings, threadManager)
		{
		}

		protected override void MultithreadedUpdate(int i)
		{
			if (simultaneouslyUpdatedUpdateables[i].IsUpdating)
			{
				simultaneouslyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
			}
		}

		protected override void SequentialUpdate(int i)
		{
			if (sequentiallyUpdatedUpdateables[i].IsUpdating)
			{
				sequentiallyUpdatedUpdateables[i].Update(timeStepSettings.TimeStepDuration);
			}
		}
	}
}
