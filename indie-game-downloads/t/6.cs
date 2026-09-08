using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace T;

internal class _6
{
	internal float a5h = a.DefaultKineticFriction;

	internal float a5b = a.DefaultStaticFriction;

	internal float a56 = a.DefaultBounciness;

	private int a5a;

	private Action<T._6> a57;

	[CompilerGenerated]
	private object a5_0006;

	public float KineticFriction
	{
		get
		{
			return a5h;
		}
		set
		{
			a5h = value;
			if (a57 != null)
			{
				a57(this);
			}
		}
	}

	public float StaticFriction
	{
		get
		{
			return a5b;
		}
		set
		{
			a5b = value;
			if (a57 != null)
			{
				a57(this);
			}
		}
	}

	public float Bounciness
	{
		get
		{
			return a56;
		}
		set
		{
			a56 = value;
			if (a57 != null)
			{
				a57(this);
			}
		}
	}

	public object Tag
	{
		[CompilerGenerated]
		get
		{
			return a5_0006;
		}
		[CompilerGenerated]
		set
		{
			a5_0006 = value;
		}
	}

	public event Action<T._6> MaterialChanged
	{
		add
		{
			Action<T._6> action = a57;
			Action<T._6> action2;
			do
			{
				action2 = action;
				Action<T._6> value2 = (Action<T._6>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange(ref a57, value2, action2);
			}
			while ((object)action != action2);
		}
		remove
		{
			Action<T._6> action = a57;
			Action<T._6> action2;
			do
			{
				action2 = action;
				Action<T._6> value2 = (Action<T._6>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange(ref a57, value2, action2);
			}
			while ((object)action != action2);
		}
	}

	public _6()
	{
		a5a = GetHashCode() * 19999999;
	}

	public _6(float staticFriction, float kineticFriction, float bounciness)
		: this()
	{
		a5b = staticFriction;
		a5h = kineticFriction;
		a56 = bounciness;
	}

	public override int GetHashCode()
	{
		return a5a;
	}
}
