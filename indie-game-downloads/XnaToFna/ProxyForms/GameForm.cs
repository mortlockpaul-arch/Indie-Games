using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDL2;
using XnaToFna.ProxyDrawing;

namespace XnaToFna.ProxyForms;

public sealed class GameForm : Form
{
	public static GameForm Instance;

	private bool _Dirty;

	private bool FakeFullscreenWindow;

	private Microsoft.Xna.Framework.Rectangle _WindowedBounds;

	private Microsoft.Xna.Framework.Rectangle _Bounds;

	private FormBorderStyle _FormBorderStyle = FormBorderStyle.FixedDialog;

	private FormWindowState _WindowState;

	private FormStartPosition _StartPosition = FormStartPosition.WindowsDefaultLocation;

	private bool Dirty
	{
		get
		{
			return _Dirty;
		}
		set
		{
			if (value)
			{
				_FormBorderStyle = FormBorderStyle;
				_WindowState = WindowState;
			}
			_Dirty = value;
		}
	}

	public override XnaToFna.ProxyDrawing.Rectangle Bounds
	{
		get
		{
			return new XnaToFna.ProxyDrawing.Rectangle(_Bounds.X, _Bounds.Y, _Bounds.Width, _Bounds.Height);
		}
		set
		{
			SDLBounds = (_Bounds = (_WindowedBounds = new Microsoft.Xna.Framework.Rectangle(value.X, value.Y, value.Width, value.Height)));
		}
	}

	public Microsoft.Xna.Framework.Rectangle SDLBounds
	{
		get
		{
			return XnaToFnaHelper.Game.Window.ClientBounds;
		}
		set
		{
			IntPtr handle = XnaToFnaHelper.Game.Window.Handle;
			SDL.SDL_SetWindowSize(handle, value.Width, value.Height);
			SDL.SDL_SetWindowPosition(handle, value.X, value.Y);
		}
	}

	protected override XnaToFna.ProxyDrawing.Rectangle _ClientRectangle
	{
		get
		{
			Microsoft.Xna.Framework.Rectangle clientBounds = XnaToFnaHelper.Game.Window.ClientBounds;
			return new XnaToFna.ProxyDrawing.Rectangle(0, 0, clientBounds.Width, clientBounds.Height);
		}
	}

	public override XnaToFna.ProxyDrawing.Point Location
	{
		get
		{
			SDL.SDL_GetWindowPosition(XnaToFnaHelper.Game.Window.Handle, out var x, out var y);
			return new XnaToFna.ProxyDrawing.Point(x, y);
		}
		set
		{
			SDL.SDL_SetWindowPosition(XnaToFnaHelper.Game.Window.Handle, value.X, value.Y);
		}
	}

	public override FormBorderStyle FormBorderStyle
	{
		get
		{
			if (Dirty)
			{
				return _FormBorderStyle;
			}
			if (XnaToFnaHelper.Game.Window.IsBorderlessEXT || FakeFullscreenWindow)
			{
				return FormBorderStyle.None;
			}
			if (XnaToFnaHelper.Game.Window.AllowUserResizing)
			{
				return FormBorderStyle.Sizable;
			}
			return FormBorderStyle.FixedDialog;
		}
		set
		{
			Dirty = true;
			_FormBorderStyle = value;
		}
	}

	public override FormWindowState WindowState
	{
		get
		{
			if (Dirty)
			{
				return _WindowState;
			}
			uint num = SDL.SDL_GetWindowFlags(XnaToFnaHelper.Game.Window.Handle);
			if ((num & 0x80) != 0 || FakeFullscreenWindow)
			{
				return FormWindowState.Maximized;
			}
			if ((num & 0x40) != 0)
			{
				return FormWindowState.Minimized;
			}
			return FormWindowState.Normal;
		}
		set
		{
			Dirty = true;
			_WindowState = value;
		}
	}

	public override FormStartPosition StartPosition
	{
		get
		{
			return _StartPosition;
		}
		set
		{
			if ((SDL.SDL_GetWindowFlags(XnaToFnaHelper.Game.Window.Handle) & 8) == 8)
			{
				switch (value)
				{
				case FormStartPosition.CenterScreen:
				case FormStartPosition.CenterParent:
					SDL.SDL_SetWindowPosition(XnaToFnaHelper.Game.Window.Handle, 805240832, 805240832);
					break;
				case FormStartPosition.WindowsDefaultLocation:
				case FormStartPosition.WindowsDefaultBounds:
					SDL.SDL_SetWindowPosition(XnaToFnaHelper.Game.Window.Handle, 805240832, 805240832);
					break;
				}
				_StartPosition = value;
			}
		}
	}

	public override bool Focused => XnaToFnaHelper.Game.IsActive;

	protected override void SetVisibleCore(bool visible)
	{
	}

	public void SDLWindowSizeChanged(object sender, EventArgs e)
	{
		Microsoft.Xna.Framework.Rectangle sDLBounds = SDLBounds;
		_Bounds = new Microsoft.Xna.Framework.Rectangle(sDLBounds.X, sDLBounds.Y, sDLBounds.Width, sDLBounds.Height);
		if ((SDL.SDL_GetWindowFlags(XnaToFnaHelper.Game.Window.Handle) & 1) == 0 && !FakeFullscreenWindow)
		{
			_WindowedBounds = _Bounds;
		}
	}

	public void SDLWindowChanged(IntPtr window, int clientWidth, int clientHeight, bool wantsFullscreen, string screenDeviceName, ref string resultDeviceName)
	{
		SDLWindowSizeChanged(null, null);
	}

	protected override void _Close()
	{
		XnaToFnaHelper.Game.Exit();
	}

	public void ApplyChanges()
	{
		if (!Dirty || Environment.GetEnvironmentVariable("FNADROID") == "1")
		{
			return;
		}
		XnaToFnaGame game = XnaToFnaHelper.Game;
		IntPtr handle = game.Window.Handle;
		GraphicsDeviceManager service = XnaToFnaHelper.GetService<IGraphicsDeviceManager, GraphicsDeviceManager>();
		bool isFullScreen = service.IsFullScreen;
		bool flag = FormBorderStyle == FormBorderStyle.None;
		bool flag2 = WindowState == FormWindowState.Maximized;
		bool fakeFullscreenWindow = FakeFullscreenWindow;
		FakeFullscreenWindow = flag2 & flag;
		XnaToFnaHelper.Log("[ProxyForms] Applying changes from ProxyForms.Form to SDL window");
		XnaToFnaHelper.Log($"[ProxyForms] Currently fullscreen: {isFullScreen}; Fake fullscreen window: {FakeFullscreenWindow}; Border: {FormBorderStyle}; State: {WindowState}");
		if (FakeFullscreenWindow)
		{
			XnaToFnaHelper.Log("[ProxyForms] Game expects borderless fullscreen... give it proper fullscreen instead.");
			if (!isFullScreen)
			{
				_WindowedBounds = SDLBounds;
			}
			XnaToFnaHelper.Log($"[ProxyForms] Last window size: {_WindowedBounds.Width} x {_WindowedBounds.Height}");
			DisplayMode displayMode = service.GraphicsDevice.DisplayMode;
			service.PreferredBackBufferWidth = displayMode.Width;
			service.PreferredBackBufferHeight = displayMode.Height;
			service.IsFullScreen = true;
			service.ApplyChanges();
			_Bounds = SDLBounds;
		}
		else
		{
			if (fakeFullscreenWindow)
			{
				XnaToFnaHelper.Log("[ProxyForms] Leaving fake borderless fullscreen.");
				service.IsFullScreen = false;
			}
			game.Window.IsBorderlessEXT = flag;
			if (flag2)
			{
				SDL.SDL_MaximizeWindow(handle);
				_Bounds = SDLBounds;
			}
			else
			{
				SDL.SDL_RestoreWindow(handle);
				SDLBounds = (_Bounds = _WindowedBounds);
			}
			XnaToFnaHelper.Log($"[ProxyForms] New window size: {_Bounds.Width} x {_Bounds.Height}");
			service.PreferredBackBufferWidth = _Bounds.Width;
			service.PreferredBackBufferHeight = _Bounds.Height;
			service.ApplyChanges();
		}
		Dirty = false;
	}
}
