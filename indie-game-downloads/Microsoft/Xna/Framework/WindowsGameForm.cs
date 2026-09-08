using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.GamerServices;
using XnaToFna.ProxyDrawing;
using XnaToFna.ProxyForms;

namespace Microsoft.Xna.Framework;

internal class WindowsGameForm : XnaToFna.ProxyForms.Form
{
	private bool freezeOurEvents;

	private Screen screen;

	private XnaToFna.ProxyForms.FormWindowState resizeWindowState;

	private Size startResizeSize = Size.Empty;

	private bool hidMouse;

	private bool isMouseVisible;

	private bool allowUserResizing;

	private bool userResizing;

	private bool? deviceChangeWillBeFullScreen;

	private bool deviceChangeChangedVisible;

	private bool oldVisible;

	private Size oldClientSize;

	private bool centerScreen = true;

	private bool isFullScreenMaximized;

	private XnaToFna.ProxyForms.FormBorderStyle savedFormBorderStyle;

	private XnaToFna.ProxyForms.FormWindowState savedWindowState;

	private XnaToFna.ProxyDrawing.Rectangle savedBounds;

	private XnaToFna.ProxyDrawing.Rectangle savedRestoreBounds;

	private bool firstPaint = true;

	internal bool AllowUserResizing
	{
		get
		{
			return allowUserResizing;
		}
		set
		{
			if (allowUserResizing != value)
			{
				allowUserResizing = value;
				UpdateBorderStyle();
			}
		}
	}

	internal bool IsMouseVisible
	{
		get
		{
			return isMouseVisible;
		}
		set
		{
			if (isMouseVisible == value)
			{
				return;
			}
			isMouseVisible = value;
			if (isMouseVisible)
			{
				if (hidMouse)
				{
					XnaToFna.ProxyForms.Cursor.Show();
					hidMouse = false;
				}
			}
			else if (!hidMouse)
			{
				XnaToFna.ProxyForms.Cursor.Hide();
				hidMouse = true;
			}
		}
	}

	internal Screen DeviceScreen => screen;

	internal Rectangle ClientBounds
	{
		get
		{
			XnaToFna.ProxyDrawing.Point point = ((XnaToFna.ProxyForms.Control)this).PointToScreen(XnaToFna.ProxyDrawing.Point.Empty);
			return new Rectangle(point.X, point.Y, ((XnaToFna.ProxyForms.Form)this).ClientSize.Width, ((XnaToFna.ProxyForms.Form)this).ClientSize.Height);
		}
	}

	internal bool IsMinimized
	{
		get
		{
			if (((XnaToFna.ProxyForms.Form)this).ClientSize.Width != 0)
			{
				return ((XnaToFna.ProxyForms.Form)this).ClientSize.Height == 0;
			}
			return true;
		}
	}

	internal event EventHandler Suspend;

	internal event EventHandler Resume;

	internal event EventHandler ScreenChanged;

	internal event EventHandler UserResized;

	internal event EventHandler ApplicationActivated;

	internal event EventHandler ApplicationDeactivated;

	public WindowsGameForm()
	{
		((XnaToFna.ProxyForms.Control)this).SuspendLayout();
		((ContainerControl)(object)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)(object)this).AutoScaleMode = AutoScaleMode.Font;
		((XnaToFna.ProxyForms.Control)this).CausesValidation = false;
		((XnaToFna.ProxyForms.Form)this).ClientSize = new Size(292, 266);
		((XnaToFna.ProxyForms.Control)this).Name = "GameForm";
		((XnaToFna.ProxyForms.Control)this).Text = "GameForm";
		((XnaToFna.ProxyForms.Form)this).ResizeBegin += Form_ResizeBegin;
		((XnaToFna.ProxyForms.Control)this).ClientSizeChanged += Form_ClientSizeChanged;
		((XnaToFna.ProxyForms.Control)this).Resize += Form_Resize;
		((XnaToFna.ProxyForms.Control)this).LocationChanged += Form_LocationChanged;
		((XnaToFna.ProxyForms.Form)this).ResizeEnd += Form_ResizeEnd;
		MouseEnter += Form_MouseEnter;
		MouseLeave += Form_MouseLeave;
		((XnaToFna.ProxyForms.Control)this).ResumeLayout(false);
		try
		{
			freezeOurEvents = true;
			resizeWindowState = WindowState;
			screen = WindowsGameWindow.ScreenFromHandle(Handle);
			((XnaToFna.ProxyForms.Control)this).SetStyle(ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint, false);
			((XnaToFna.ProxyForms.Form)this).ClientSize = new Size(GameWindow.DefaultClientWidth, GameWindow.DefaultClientHeight);
			UpdateBorderStyle();
		}
		finally
		{
			freezeOurEvents = false;
		}
	}

	private void UpdateBorderStyle()
	{
		if (!allowUserResizing)
		{
			((XnaToFna.ProxyForms.Form)this).MaximizeBox = false;
			if (!isFullScreenMaximized)
			{
				FormBorderStyle = XnaToFna.ProxyForms.FormBorderStyle.FixedSingle;
			}
		}
		else
		{
			((XnaToFna.ProxyForms.Form)this).MaximizeBox = true;
			if (!isFullScreenMaximized)
			{
				FormBorderStyle = XnaToFna.ProxyForms.FormBorderStyle.Sizable;
			}
		}
	}

	private void UpdateScreen()
	{
		if (freezeOurEvents)
		{
			return;
		}
		Screen obj = Screen.FromHandle(Handle);
		if (screen == null || !screen.Equals(obj))
		{
			screen = obj;
			if (screen != null)
			{
				OnScreenChanged();
			}
		}
	}

	private void OnSuspend()
	{
		if (Suspend != null)
		{
			Suspend(this, EventArgs.Empty);
		}
	}

	private void OnResume()
	{
		if (Resume != null)
		{
			Resume(this, EventArgs.Empty);
		}
	}

	private void OnScreenChanged()
	{
		if (ScreenChanged != null)
		{
			ScreenChanged(this, EventArgs.Empty);
		}
	}

	private void OnUserResized(bool forceEvent)
	{
		if ((!freezeOurEvents || forceEvent) && UserResized != null)
		{
			UserResized(this, EventArgs.Empty);
		}
	}

	private void OnActivateApp(bool active)
	{
		if (active)
		{
			firstPaint = true;
			freezeOurEvents = false;
			if (isFullScreenMaximized)
			{
				((XnaToFna.ProxyForms.Form)this).TopMost = true;
			}
			if (ApplicationActivated != null)
			{
				ApplicationActivated(this, EventArgs.Empty);
			}
		}
		else
		{
			if (ApplicationDeactivated != null)
			{
				ApplicationDeactivated(this, EventArgs.Empty);
			}
			freezeOurEvents = true;
		}
	}

	protected virtual void OnPaintBackground(PaintEventArgs e)
	{
		if (firstPaint)
		{
			((ScrollableControl)(object)this).OnPaintBackground(e);
			firstPaint = false;
		}
	}

	protected override void WndProc(ref XnaToFna.ProxyForms.Message m)
	{
		if (m.Msg == 28)
		{
			bool active = ((m.WParam != IntPtr.Zero) ? true : false);
			OnActivateApp(active);
		}
		WndProc(ref m);
	}

	protected virtual bool ProcessDialogKey(Keys keyData)
	{
		Keys keys = keyData & Keys.KeyCode;
		Keys keys2 = keyData & Keys.Alt;
		if (keys2 == Keys.Alt && (keys == Keys.F4 || keys == Keys.None))
		{
			return ((XnaToFna.ProxyForms.Form)this).ProcessDialogKey(keyData);
		}
		if (GamerServicesDispatcher.IsInitialized && (keys == Keys.Home || Guide.IsVisible))
		{
			return ((XnaToFna.ProxyForms.Form)this).ProcessDialogKey(keyData);
		}
		return true;
	}

	protected override void Dispose(bool disposing)
	{
		((XnaToFna.ProxyForms.Form)this).Dispose(disposing);
	}

	private void Form_ResizeBegin(object sender, EventArgs e)
	{
		startResizeSize = ((XnaToFna.ProxyForms.Form)this).ClientSize;
		userResizing = true;
		OnSuspend();
	}

	private void Form_Resize(object sender, EventArgs e)
	{
		if (resizeWindowState != WindowState)
		{
			resizeWindowState = WindowState;
			firstPaint = true;
			OnUserResized(forceEvent: false);
			((XnaToFna.ProxyForms.Control)this).Invalidate();
		}
		if (userResizing && ((XnaToFna.ProxyForms.Form)this).ClientSize != startResizeSize)
		{
			((XnaToFna.ProxyForms.Control)this).Invalidate();
		}
	}

	private void Form_ResizeEnd(object sender, EventArgs e)
	{
		userResizing = false;
		if (((XnaToFna.ProxyForms.Form)this).ClientSize != startResizeSize)
		{
			centerScreen = false;
			OnUserResized(forceEvent: false);
		}
		firstPaint = true;
		OnResume();
	}

	private void Form_ClientSizeChanged(object sender, EventArgs e)
	{
		UpdateScreen();
	}

	private void Form_LocationChanged(object sender, EventArgs e)
	{
		if (userResizing)
		{
			centerScreen = false;
		}
		UpdateScreen();
	}

	private void Form_MouseEnter(object sender, EventArgs e)
	{
		if (!isMouseVisible && !hidMouse)
		{
			XnaToFna.ProxyForms.Cursor.Hide();
			hidMouse = true;
		}
	}

	private void Form_MouseLeave(object sender, EventArgs e)
	{
		if (hidMouse)
		{
			XnaToFna.ProxyForms.Cursor.Show();
			hidMouse = false;
		}
	}

	internal void BeginScreenDeviceChange(bool willBeFullScreen)
	{
		oldClientSize = ((XnaToFna.ProxyForms.Form)this).ClientSize;
		if (willBeFullScreen && !isFullScreenMaximized)
		{
			savedFormBorderStyle = FormBorderStyle;
			savedWindowState = WindowState;
			savedBounds = Bounds;
			if (WindowState == XnaToFna.ProxyForms.FormWindowState.Maximized)
			{
				savedRestoreBounds = ((XnaToFna.ProxyForms.Form)this).RestoreBounds;
			}
		}
		if (willBeFullScreen != isFullScreenMaximized)
		{
			deviceChangeChangedVisible = true;
			oldVisible = ((XnaToFna.ProxyForms.Control)this).Visible;
			((XnaToFna.ProxyForms.Control)this).Visible = false;
		}
		else
		{
			deviceChangeChangedVisible = false;
		}
		if (!willBeFullScreen && isFullScreenMaximized)
		{
			((XnaToFna.ProxyForms.Form)this).TopMost = false;
			FormBorderStyle = savedFormBorderStyle;
			if (savedWindowState == XnaToFna.ProxyForms.FormWindowState.Maximized)
			{
				((XnaToFna.ProxyForms.Control)this).SetBoundsCore(screen.Bounds.X, screen.Bounds.Y, savedRestoreBounds.Width, savedRestoreBounds.Height, BoundsSpecified.Size);
			}
			else
			{
				((XnaToFna.ProxyForms.Control)this).SetBoundsCore(screen.Bounds.X, screen.Bounds.Y, savedBounds.Width, savedBounds.Height, BoundsSpecified.Size);
			}
		}
		if (willBeFullScreen != isFullScreenMaximized)
		{
			((XnaToFna.ProxyForms.Control)this).SendToBack();
		}
		deviceChangeWillBeFullScreen = willBeFullScreen;
	}

	internal void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
	{
		if (!deviceChangeWillBeFullScreen.HasValue)
		{
			throw new InvalidOperationException(Resources.MustCallBeginDeviceChange);
		}
		bool flag = false;
		if (deviceChangeWillBeFullScreen.Value)
		{
			Screen screen = WindowsGameWindow.ScreenFromDeviceName(screenDeviceName);
			XnaToFna.ProxyDrawing.Rectangle bounds = Screen.GetBounds(new XnaToFna.ProxyDrawing.Point(screen.Bounds.X, screen.Bounds.Y));
			if (!isFullScreenMaximized)
			{
				flag = true;
				((XnaToFna.ProxyForms.Form)this).TopMost = true;
				FormBorderStyle = XnaToFna.ProxyForms.FormBorderStyle.None;
				WindowState = XnaToFna.ProxyForms.FormWindowState.Normal;
				((XnaToFna.ProxyForms.Control)this).BringToFront();
			}
			((XnaToFna.ProxyForms.Form)this).Location = new XnaToFna.ProxyDrawing.Point(bounds.X, bounds.Y);
			((XnaToFna.ProxyForms.Form)this).ClientSize = new Size(bounds.Width, bounds.Height);
			isFullScreenMaximized = true;
		}
		else
		{
			if (isFullScreenMaximized)
			{
				flag = true;
				((XnaToFna.ProxyForms.Control)this).BringToFront();
			}
			ResizeWindow(screenDeviceName, clientWidth, clientHeight, centerScreen);
		}
		if (deviceChangeChangedVisible)
		{
			((XnaToFna.ProxyForms.Control)this).Visible = oldVisible;
		}
		if (flag && oldClientSize != ((XnaToFna.ProxyForms.Form)this).ClientSize)
		{
			OnUserResized(forceEvent: true);
		}
		deviceChangeWillBeFullScreen = null;
	}

	private void ResizeWindow(string screenDeviceName, int clientWidth, int clientHeight, bool center)
	{
		Screen screen = WindowsGameWindow.ScreenFromDeviceName(screenDeviceName);
		XnaToFna.ProxyDrawing.Rectangle bounds = Screen.GetBounds(new XnaToFna.ProxyDrawing.Point(screen.Bounds.X, screen.Bounds.Y));
		int x;
		int y;
		if (screenDeviceName != WindowsGameWindow.DeviceNameFromScreen(DeviceScreen))
		{
			x = bounds.X;
			y = bounds.Y;
		}
		else
		{
			x = this.screen.Bounds.X;
			y = this.screen.Bounds.Y;
		}
		if (isFullScreenMaximized)
		{
			Size size = ((XnaToFna.ProxyForms.Control)this).SizeFromClientSize(new Size(clientWidth, clientHeight));
			if (savedWindowState == XnaToFna.ProxyForms.FormWindowState.Maximized)
			{
				int num = savedRestoreBounds.X - this.screen.Bounds.X + x;
				int num2 = savedRestoreBounds.Y - this.screen.Bounds.Y + y;
				((XnaToFna.ProxyForms.Control)this).SetBoundsCore(num, num2, savedRestoreBounds.Width, savedRestoreBounds.Height, BoundsSpecified.All);
			}
			else if (center)
			{
				int num3 = x + bounds.Width / 2 - size.Width / 2;
				int num4 = y + bounds.Height / 2 - size.Height / 2;
				((XnaToFna.ProxyForms.Control)this).SetBoundsCore(num3, num4, size.Width, size.Height, BoundsSpecified.All);
			}
			else
			{
				int num5 = savedBounds.X - this.screen.Bounds.X + x;
				int num6 = savedBounds.Y - this.screen.Bounds.Y + y;
				((XnaToFna.ProxyForms.Control)this).SetBoundsCore(num5, num6, size.Width, size.Height, BoundsSpecified.All);
			}
			WindowState = savedWindowState;
			isFullScreenMaximized = false;
		}
		else if (WindowState == XnaToFna.ProxyForms.FormWindowState.Normal)
		{
			int num7;
			int num8;
			if (center)
			{
				Size size2 = ((XnaToFna.ProxyForms.Control)this).SizeFromClientSize(new Size(clientWidth, clientHeight));
				num7 = x + bounds.Width / 2 - size2.Width / 2;
				num8 = y + bounds.Height / 2 - size2.Height / 2;
			}
			else
			{
				num7 = x + Bounds.X - this.screen.Bounds.X;
				num8 = y + Bounds.Y - this.screen.Bounds.Y;
			}
			if (num7 != ((XnaToFna.ProxyForms.Form)this).Location.X || num8 != ((XnaToFna.ProxyForms.Form)this).Location.Y)
			{
				((XnaToFna.ProxyForms.Form)this).Location = new XnaToFna.ProxyDrawing.Point(num7, num8);
			}
			if (((XnaToFna.ProxyForms.Form)this).ClientSize.Width != clientWidth || ((XnaToFna.ProxyForms.Form)this).ClientSize.Height != clientHeight)
			{
				((XnaToFna.ProxyForms.Form)this).ClientSize = new Size(clientWidth, clientHeight);
			}
		}
	}
}
