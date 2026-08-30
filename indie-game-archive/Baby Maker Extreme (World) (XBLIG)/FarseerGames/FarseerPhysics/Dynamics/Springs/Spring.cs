using System;
using FarseerGames.FarseerPhysics.Interfaces;

namespace FarseerGames.FarseerPhysics.Dynamics.Springs;

public abstract class Spring : IIsDisposable, IDisposable
{
	public float Breakpoint = float.MaxValue;

	public float DampingConstant;

	public bool Enabled = true;

	public float SpringConstant;

	private bool _isDisposed;

	public object Tag { get; set; }

	public float SpringError { get; protected set; }

	public bool IsDisposed
	{
		get
		{
			return _isDisposed;
		}
		set
		{
			_isDisposed = value;
		}
	}

	public event EventHandler<EventArgs> Broke;

	public abstract void Validate();

	public virtual void Update(float dt)
	{
		if (Enabled && !(Math.Abs(SpringError) <= Breakpoint))
		{
			Enabled = false;
			if (Broke != null)
			{
				Broke(this, EventArgs.Empty);
			}
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		IsDisposed = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
