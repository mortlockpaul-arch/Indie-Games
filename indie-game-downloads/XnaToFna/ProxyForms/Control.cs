using System;
using System.Collections.Generic;
using XnaToFna.ProxyDrawing;

namespace XnaToFna.ProxyForms;

public class Control : IDisposable
{
	public static List<WeakReference<Control>> AllControls = new List<WeakReference<Control>>();

	public int GlobalIndex;

	public Form Form;

	protected bool _IsDisposed;

	public IntPtr Handle => (IntPtr)GlobalIndex;

	public static Point MousePosition => Cursor.Position;

	public virtual Rectangle Bounds { get; set; }

	protected virtual Rectangle _ClientRectangle { get; set; }

	public Rectangle ClientRectangle => _ClientRectangle;

	public virtual Point Location { get; set; }

	public virtual Cursor Cursor { get; set; }

	public virtual bool Focused { get; protected set; }

	public bool IsDisposed => _IsDisposed;

	public event EventHandler MouseEnter;

	public event MouseEventHandler MouseMove;

	public event EventHandler MouseHover;

	public event MouseEventHandler MouseDown;

	public event MouseEventHandler MouseWheel;

	public event MouseEventHandler MouseUp;

	public event EventHandler MouseLeave;

	public Control()
	{
		GlobalIndex = AllControls.Count + 1;
		XnaToFnaHelper.Log($"[ProxyForms] Creating control {GetType().Name}, globally #{GlobalIndex}");
		AllControls.Add(new WeakReference<Control>(this));
	}

	public static Control FromHandle(IntPtr ptr)
	{
		int num = (int)ptr - 1;
		if (num < 0 || AllControls.Count <= num)
		{
			return null;
		}
		WeakReference<Control> weakReference = AllControls[num];
		if (weakReference == null || !weakReference.TryGetTarget(out var target))
		{
			AllControls[num] = null;
			return null;
		}
		return target;
	}

	public Form FindForm()
	{
		return Form ?? GameForm.Instance;
	}

	public void SetBounds(int x, int y, int w, int h)
	{
		Bounds = new Rectangle(x, y, w, h);
	}

	protected virtual void CreateHandle()
	{
	}

	public object Invoke(Delegate method)
	{
		return method.DynamicInvoke();
	}

	public object Invoke(Delegate method, params object[] args)
	{
		return method.DynamicInvoke(args);
	}

	public IAsyncResult BeginInvoke(Delegate method)
	{
		return new SyncResult(method.DynamicInvoke());
	}

	public IAsyncResult BeginInvoke(Delegate method, params object[] args)
	{
		return new SyncResult(method.DynamicInvoke(args));
	}

	public object EndInvoke(IAsyncResult result)
	{
		return result.AsyncState;
	}

	public Rectangle RectangleToScreen(Rectangle r)
	{
		Rectangle bounds = Bounds;
		return new Rectangle(r.X + bounds.X, r.Y + bounds.Y, r.Width, r.Height);
	}

	public Rectangle RectangleToClient(Rectangle r)
	{
		Rectangle bounds = Bounds;
		return new Rectangle(r.X - bounds.X, r.Y - bounds.Y, r.Width, r.Height);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_IsDisposed)
		{
			_IsDisposed = true;
		}
	}

	protected virtual void SetVisibleCore(bool visible)
	{
	}

	protected virtual void WndProc(ref Message msg)
	{
	}
}
