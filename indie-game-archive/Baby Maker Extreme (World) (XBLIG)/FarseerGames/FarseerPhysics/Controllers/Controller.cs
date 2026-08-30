using System;
using FarseerGames.FarseerPhysics.Interfaces;

namespace FarseerGames.FarseerPhysics.Controllers;

public abstract class Controller : IIsDisposable, IDisposable
{
	public bool Enabled = true;

	private bool _isDisposed;

	public object Tag { get; set; }

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

	protected virtual void Dispose(bool disposing)
	{
		IsDisposed = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public abstract void Validate();

	public abstract void Update(float dt, float dtReal);
}
