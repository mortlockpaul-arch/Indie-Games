using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace r;

internal abstract class _6
{
	private Action a5h;

	private Action a5b;

	[CompilerGenerated]
	private bool a56;

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
			UpdateStage();
			if (a5b != null)
			{
				a5b();
			}
		}
	}

	protected abstract void UpdateStage();
}
