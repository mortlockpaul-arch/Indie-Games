using System;
using System.Xml.Serialization;
using FarseerGames.FarseerPhysics.Interfaces;
using Microsoft.Xna.Framework.Content;

namespace FarseerGames.FarseerPhysics.Dynamics.Joints;

public abstract class Joint : IIsDisposable, IDisposable
{
	public float BiasFactor = 0.2f;

	public float Breakpoint = float.MaxValue;

	public bool Enabled = true;

	public float Softness;

	private bool _isDisposed;

	[XmlIgnore]
	[ContentSerializerIgnore]
	public object Tag { get; set; }

	public float JointError { get; protected set; }

	[XmlIgnore]
	[ContentSerializerIgnore]
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

	public abstract void PreStep(float inverseDt);

	public virtual void Update()
	{
		if (Enabled && !(Math.Abs(JointError) <= Breakpoint))
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
