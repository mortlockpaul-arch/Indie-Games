using System;
using System.Runtime.CompilerServices;
using E;
using Microsoft.Xna.Framework;
using N;
using d;
using r;

namespace n;

internal class v : r.b
{
	private b a5h;

	private N._0006[] a5b;

	private N._0006[] a56 = new N._0006[64];

	private float a5a;

	private Action<int> a57;

	[CompilerGenerated]
	private object a5_0006;

	public override bool Enabled
	{
		get
		{
			return base.Enabled;
		}
		set
		{
			if (base.Enabled && !value)
			{
				_6S();
				base.Enabled = false;
			}
			else if (!base.Enabled && value)
			{
				if (!a5h.ReadBuffers.Enabled)
				{
					throw new InvalidOperationException("Cannot enable interpolated states unless the read buffers are enabled.");
				}
				_6w();
				base.Enabled = true;
			}
		}
	}

	public object FlipLocker
	{
		[CompilerGenerated]
		get
		{
			return a5_0006;
		}
		[CompilerGenerated]
		private set
		{
			a5_0006 = obj;
		}
	}

	public float BlendAmount
	{
		get
		{
			return a5a;
		}
		set
		{
			a5a = MathHelper.Clamp(value, 0f, 1f);
		}
	}

	internal void _6w()
	{
		lock (FlipLocker)
		{
			int num = Math.Max(a5h.a5h.Count, 64);
			a5b = new N._0006[num];
			a56 = new N._0006[num];
			for (int i = 0; i < a5h.a5h.Count; i++)
			{
				E.h h2 = a5h.a5h[i];
				a5b[i].Position = h2.a5h;
				a5b[i].Orientation = h2.a5b;
			}
			Array.Copy(a5b, a56, a5b.Length);
		}
	}

	internal void _6S()
	{
		lock (FlipLocker)
		{
			a5b = null;
			a56 = null;
		}
	}

	public v(b manager)
	{
		a5h = manager;
		a57 = _6k;
		FlipLocker = new object();
	}

	public v(b manager, d.b threadManager)
	{
		a5h = manager;
		a57 = _6k;
		FlipLocker = new object();
		base.ThreadManager = threadManager;
		base.AllowMultithreading = true;
	}

	private void _6k(int P_0)
	{
		E.h h2 = a5h.a5h[P_0];
		Vector3.Lerp(ref a5h.ReadBuffers.a5b[P_0].Position, ref h2.a5h, a5a, out a5b[P_0].Position);
		Quaternion.Slerp(ref a5h.ReadBuffers.a5b[P_0].Orientation, ref h2.a5b, a5a, out a5b[P_0].Orientation);
	}

	protected override void UpdateMultithreaded()
	{
		base.ThreadManager.ForLoop(0, a5h.a5h.Count, a57);
		FlipBuffers();
	}

	protected override void UpdateSingleThreaded()
	{
		for (int i = 0; i < a5h.a5h.Count; i++)
		{
			_6k(i);
		}
		FlipBuffers();
	}

	public void FlipBuffers()
	{
		lock (FlipLocker)
		{
			N._0006[] array = a56;
			a56 = a5b;
			a5b = array;
		}
	}

	public N._0006 GetState(int motionStateIndex)
	{
		return a56[motionStateIndex];
	}

	public void GetStates(N._0006[] states)
	{
		lock (FlipLocker)
		{
			if (states.Length < a5h.a5h.Count)
			{
				throw new ArgumentException("Array is not large enough to hold the buffer.", "states");
			}
			Array.Copy(a56, states, a5h.a5h.Count);
		}
	}

	internal void _64(E.h P_0)
	{
		if (a56.Length <= P_0.BufferedStates.a5h)
		{
			N._0006[] array = new N._0006[a56.Length * 2];
			a56.CopyTo(array, 0);
			a56 = array;
		}
		a56[P_0.BufferedStates.a5h].Position = P_0.a5h;
		a56[P_0.BufferedStates.a5h].Orientation = P_0.a5b;
		if (a5b.Length <= P_0.BufferedStates.a5h)
		{
			N._0006[] array2 = new N._0006[a5b.Length * 2];
			a5b.CopyTo(array2, 0);
			a5b = array2;
		}
		a5b[P_0.BufferedStates.a5h].Position = P_0.a5h;
		a5b[P_0.BufferedStates.a5h].Orientation = P_0.a5b;
	}

	internal void _6e(int P_0, int P_1)
	{
		ref N._0006 reference = ref a56[P_0];
		reference = a56[P_1];
		ref N._0006 reference2 = ref a5b[P_0];
		reference2 = a5b[P_1];
	}
}
