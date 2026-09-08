using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Graphics;

public abstract class GraphicsResource : IDisposable
{
	protected string _Name;

	private GCHandle selfReference;

	private GraphicsDevice graphicsDevice;

	public GraphicsDevice GraphicsDevice
	{
		get
		{
			return graphicsDevice;
		}
		internal set
		{
			if (graphicsDevice != value)
			{
				if (graphicsDevice != null && selfReference.IsAllocated && graphicsDevice.RemoveResourceReference(selfReference))
				{
					selfReference.Free();
				}
				graphicsDevice = value;
				selfReference = GCHandle.Alloc(this, GCHandleType.Weak);
				graphicsDevice.AddResourceReference(selfReference);
			}
		}
	}

	public bool IsDisposed { get; private set; }

	public virtual string Name
	{
		get
		{
			return _Name;
		}
		set
		{
			_Name = value;
		}
	}

	public object Tag { get; set; }

	protected internal virtual bool IsHarmlessToLeakInstance => false;

	public event EventHandler<EventArgs> Disposing;

	internal GraphicsResource()
	{
	}

	~GraphicsResource()
	{
		if (!IsDisposed && (graphicsDevice != null && !graphicsDevice.IsDisposed))
		{
			if (!IsHarmlessToLeakInstance)
			{
				FNALoggerEXT.LogWarn($"A resource of type {GetType().Name} with tag {Tag} and name {Name} was not Disposed.");
			}
			Dispose(disposing: false);
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public override string ToString()
	{
		return string.IsNullOrEmpty(Name) ? base.ToString() : Name;
	}

	protected internal virtual void GraphicsDeviceResetting()
	{
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			if (disposing && Disposing != null)
			{
				Disposing(this, EventArgs.Empty);
			}
			if (graphicsDevice != null && selfReference.IsAllocated && graphicsDevice.RemoveResourceReference(selfReference))
			{
				selfReference.Free();
			}
			IsDisposed = true;
		}
	}
}
