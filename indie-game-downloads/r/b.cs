using System;
using System.Runtime.CompilerServices;
using System.Threading;
using d;

namespace r;

internal abstract class b
{
	private Action a5h;

	private Action a5b;

	[CompilerGenerated]
	private bool a56;

	[CompilerGenerated]
	private bool a5a;

	[CompilerGenerated]
	private d.b a57;

	public virtual bool Enabled
	{
		[CompilerGenerated]
		get
		{
			return a56;
		}
		[CompilerGenerated]
		set
		{
			a56 = value;
		}
	}

	public bool AllowMultithreading
	{
		[CompilerGenerated]
		get
		{
			return a5a;
		}
		[CompilerGenerated]
		set
		{
			a5a = value;
		}
	}

	public d.b ThreadManager
	{
		[CompilerGenerated]
		get
		{
			return a57;
		}
		[CompilerGenerated]
		set
		{
			a57 = value;
		}
	}

	protected bool ShouldUseMultithreading
	{
		get
		{
			if (AllowMultithreading && ThreadManager != null)
			{
				return ThreadManager.ThreadCount > 1;
			}
			return false;
		}
	}

	public event Action Starting
	{
		add
		{
			Action action = a5h;
			Action action2;
			do
			{
				action2 = action;
				Action value2 = (Action)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange(ref a5h, value2, action2);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = a5h;
			Action action2;
			do
			{
				action2 = action;
				Action value2 = (Action)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange(ref a5h, value2, action2);
			}
			while ((object)action != action2);
		}
	}

	public event Action Finishing
	{
		add
		{
			Action action = a5b;
			Action action2;
			do
			{
				action2 = action;
				Action value2 = (Action)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange(ref a5b, value2, action2);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action action = a5b;
			Action action2;
			do
			{
				action2 = action;
				Action value2 = (Action)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange(ref a5b, value2, action2);
			}
			while ((object)action != action2);
		}
	}

	public void Update()
	{
		if (Enabled)
		{
			if (a5h != null)
			{
				a5h();
			}
			if (ShouldUseMultithreading)
			{
				UpdateMultithreaded();
			}
			else
			{
				UpdateSingleThreaded();
			}
			if (a5b != null)
			{
				a5b();
			}
		}
	}

	protected abstract void UpdateMultithreaded();

	protected abstract void UpdateSingleThreaded();
}
internal class B
{
	public int MaximumTimeStepsPerFrame = 3;

	public float TimeStepDuration = 1f / 60f;

	public float AccumulatedTime;
}
