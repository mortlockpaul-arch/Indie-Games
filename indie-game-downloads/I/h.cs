using System;
using System.Threading;
using _0004;
using Microsoft.Xna.Framework;
using P;
using l;
using p;

namespace I
{
	internal struct h : IEquatable<h>
	{
		public Vector3 Position;

		public float Depth;

		public int Id;

		public bool Equals(h other)
		{
			return Id == other.Id;
		}
	}
}
namespace i
{
	internal abstract class h
	{
		protected l._7<int> contactIndicesToRemove;

		protected internal l._7<_0004.h> contacts;

		protected p.a<_0004.h> unusedContacts;

		private Action<_0004.h> a5h;

		private Action<_0004.h> a5b;

		public l.X<_0004.h> Contacts => new l.X<_0004.h>(contacts);

		public event Action<_0004.h> ContactAdded
		{
			add
			{
				Action<_0004.h> action = a5h;
				Action<_0004.h> action2;
				do
				{
					action2 = action;
					Action<_0004.h> value2 = (Action<_0004.h>)Delegate.Combine(action2, value);
					action = Interlocked.CompareExchange(ref a5h, value2, action2);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<_0004.h> action = a5h;
				Action<_0004.h> action2;
				do
				{
					action2 = action;
					Action<_0004.h> value2 = (Action<_0004.h>)Delegate.Remove(action2, value);
					action = Interlocked.CompareExchange(ref a5h, value2, action2);
				}
				while ((object)action != action2);
			}
		}

		public event Action<_0004.h> ContactRemoved
		{
			add
			{
				Action<_0004.h> action = a5b;
				Action<_0004.h> action2;
				do
				{
					action2 = action;
					Action<_0004.h> value2 = (Action<_0004.h>)Delegate.Combine(action2, value);
					action = Interlocked.CompareExchange(ref a5b, value2, action2);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<_0004.h> action = a5b;
				Action<_0004.h> action2;
				do
				{
					action2 = action;
					Action<_0004.h> value2 = (Action<_0004.h>)Delegate.Remove(action2, value);
					action = Interlocked.CompareExchange(ref a5b, value2, action2);
				}
				while ((object)action != action2);
			}
		}

		protected void RemoveQueuedContacts()
		{
			for (int num = contactIndicesToRemove.a5h - 1; num >= 0; num--)
			{
				Remove(contactIndicesToRemove.Elements[num]);
			}
			contactIndicesToRemove.Clear();
		}

		protected virtual void Remove(int contactIndex)
		{
			_0004.h h2 = contacts.Elements[contactIndex];
			contacts.FastRemoveAt(contactIndex);
			OnRemoved(h2);
			unusedContacts.GiveBack(h2);
		}

		protected virtual void Add(ref _0004.b contactCandidate)
		{
			_0004.h h2 = unusedContacts.Take();
			h2.Setup(ref contactCandidate);
			contacts.Add(h2);
			OnAdded(h2);
		}

		protected void OnAdded(_0004.h contact)
		{
			if (a5h != null)
			{
				a5h(contact);
			}
		}

		protected void OnRemoved(_0004.h contact)
		{
			if (a5b != null)
			{
				a5b(contact);
			}
		}

		public abstract void Initialize(P.h newCollidableA, P.h newCollidableB);

		public virtual void CleanUp()
		{
		}

		public abstract void Update(float dt);

		public virtual void ClearContacts()
		{
			for (int num = contacts.a5h - 1; num >= 0; num--)
			{
				Remove(num);
			}
		}
	}
}
