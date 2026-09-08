using System;
using _0010;
using Microsoft.Xna.Framework;

namespace D
{
	internal abstract class _7 : a
	{
		private new _0010._7 a5h = new _0010._7();

		public override _0010.b ContactConstraint => a5h;

		protected internal override void GetContactInformation(int index, out _0001 info)
		{
			info.Contact = ContactManifold.contacts.Elements[index];
			float num = 0f;
			info.NormalForce = 0f;
			for (int i = 0; i < a5h.a56.a5h; i++)
			{
				num += a5h.a56.Elements[i].a5b;
				if (a5h.a56.Elements[i].a5h == info.Contact)
				{
					info.NormalForce = a5h.a56.Elements[i].a5b;
				}
			}
			Vector3.Distance(ref a5h.a5b.a5r, ref info.Contact.Position, out var result);
			if (num > 0f)
			{
				info.FrictionForce = info.NormalForce / num * (a5h.a5b.a5b.Length() + a5h.a5h.a56 * result);
			}
			else
			{
				info.FrictionForce = 0f;
			}
			Vector3 result2;
			if (EntityA != null)
			{
				Vector3.Subtract(ref info.Contact.Position, ref EntityA.a5h, out result2);
				Vector3.Cross(ref EntityA.a5_0006, ref result2, out result2);
				Vector3.Add(ref result2, ref EntityA.a5a, out info.RelativeVelocity);
			}
			else
			{
				info.RelativeVelocity = default(Vector3);
			}
			if (EntityB != null)
			{
				Vector3.Subtract(ref info.Contact.Position, ref EntityB.a5h, out result2);
				Vector3.Cross(ref EntityB.a5_0006, ref result2, out result2);
				Vector3.Add(ref result2, ref EntityB.a5a, out result2);
				Vector3.Subtract(ref info.RelativeVelocity, ref result2, out info.RelativeVelocity);
			}
			info.Pair = this;
		}
	}
}
namespace d
{
	internal class _7 : b, IDisposable
	{
		private readonly object a5h = new object();

		private bool a5b;

		private _6 a56;

		private _0006 a5a;

		public _6 LoopManager
		{
			get
			{
				return a56;
			}
			set
			{
				a56 = value;
			}
		}

		public _0006 TaskManager
		{
			get
			{
				return a5a;
			}
			set
			{
				a5a = value;
			}
		}

		public int ThreadCount => a5a.ThreadCount;

		public _7()
		{
			a5a = new _0006();
			a56 = new _6();
		}

		~_7()
		{
			Dispose();
		}

		public void AddThread()
		{
			a5a.AddThread();
			a56.a_000E();
		}

		public void AddThread(Action<object> initialization, object initializationInformation)
		{
			a5a.AddThread(initialization, initializationInformation);
			a56.a_000E(initialization, initializationInformation);
		}

		public void RemoveThread()
		{
			a5a.RemoveThread();
			a56.ay();
		}

		public void EnqueueTask(Action<object> taskBody, object taskInformation)
		{
			a5a.EnqueueTask(taskBody, taskInformation);
		}

		public void ForLoop(int startIndex, int endIndex, Action<int> loopBody)
		{
			a56.ForLoop(startIndex, endIndex, loopBody);
		}

		public void WaitForTaskCompletion()
		{
			a5a.WaitForTaskCompletion();
		}

		public void Dispose()
		{
			lock (a5h)
			{
				if (!a5b)
				{
					a5b = true;
					a5a.Dispose();
					a56.Dispose();
					GC.SuppressFinalize(this);
				}
			}
		}
	}
}
