using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework;

public class DrawableGameComponent : GameComponent, IDrawable
{
	private bool _initialized;

	private int _drawOrder;

	private bool _visible = true;

	public GraphicsDevice GraphicsDevice
	{
		get
		{
			if (!_initialized)
			{
				throw new InvalidOperationException("The GraphicsDevice property cannot be used before Initialize has been called.");
			}
			return base.Game.GraphicsDevice;
		}
	}

	public int DrawOrder
	{
		get
		{
			return _drawOrder;
		}
		set
		{
			if (_drawOrder != value)
			{
				_drawOrder = value;
				OnDrawOrderChanged(this, EventArgs.Empty);
			}
		}
	}

	public bool Visible
	{
		get
		{
			return _visible;
		}
		set
		{
			if (_visible != value)
			{
				_visible = value;
				OnVisibleChanged(this, EventArgs.Empty);
			}
		}
	}

	public event EventHandler<EventArgs> DrawOrderChanged;

	public event EventHandler<EventArgs> VisibleChanged;

	public DrawableGameComponent(Game game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		if (!_initialized)
		{
			_initialized = true;
			IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService)base.Game.Services.INTERNAL_GetService(typeof(IGraphicsDeviceService));
			if (graphicsDeviceService == null)
			{
				throw new InvalidOperationException("Drawable components require a graphics device service in the game service container.");
			}
			if (graphicsDeviceService.GraphicsDevice != null)
			{
				LoadContent();
			}
			else
			{
				graphicsDeviceService.DeviceCreated += OnDeviceCreated;
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (_initialized)
		{
			UnloadContent();
		}
		base.Dispose(disposing);
	}

	private void OnDeviceCreated(object sender, EventArgs e)
	{
		LoadContent();
	}

	public virtual void Draw(GameTime gameTime)
	{
	}

	protected virtual void LoadContent()
	{
	}

	protected virtual void UnloadContent()
	{
	}

	protected virtual void OnVisibleChanged(object sender, EventArgs args)
	{
		if (VisibleChanged != null)
		{
			VisibleChanged(this, args);
		}
	}

	protected virtual void OnDrawOrderChanged(object sender, EventArgs args)
	{
		if (DrawOrderChanged != null)
		{
			DrawOrderChanged(this, args);
		}
	}
}
