using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XnaToFna.ProxyDrawing;

namespace XnaToFna.ProxyForms;

public sealed class Cursor : IDisposable
{
	public static List<WeakReference<Cursor>> AllCursors = new List<WeakReference<Cursor>>();

	public int GlobalIndex;

	internal bool INTERNAL_IsNullCursor;

	private bool _IsDisposed;

	public IntPtr Handle => (IntPtr)GlobalIndex;

	public static Cursor Current { get; set; } = new Cursor();

	public static XnaToFna.ProxyDrawing.Rectangle Clip
	{
		get
		{
			if (!MouseEvents.Clip.HasValue)
			{
				return default(XnaToFna.ProxyDrawing.Rectangle);
			}
			Microsoft.Xna.Framework.Rectangle value = MouseEvents.Clip.Value;
			return new XnaToFna.ProxyDrawing.Rectangle(value.X, value.Y, value.Width, value.Height);
		}
		set
		{
			if (value == default(XnaToFna.ProxyDrawing.Rectangle))
			{
				MouseEvents.Clip = null;
			}
			else
			{
				MouseEvents.Clip = new Microsoft.Xna.Framework.Rectangle(value.X, value.Y, value.Width, value.Height);
			}
		}
	}

	public static XnaToFna.ProxyDrawing.Point Position
	{
		get
		{
			Microsoft.Xna.Framework.Rectangle clientBounds = XnaToFnaHelper.Game.Window.ClientBounds;
			MouseState state = Mouse.GetState();
			return new XnaToFna.ProxyDrawing.Point(state.X + clientBounds.X, state.Y + clientBounds.Y);
		}
		set
		{
			if (!(Position == value))
			{
				Microsoft.Xna.Framework.Rectangle clientBounds = XnaToFnaHelper.Game.Window.ClientBounds;
				Mouse.SetPosition(value.X - clientBounds.X, value.Y - clientBounds.Y);
			}
		}
	}

	public XnaToFna.ProxyDrawing.Point HotSpot { get; internal set; }

	public object Tag { get; set; }

	private Cursor()
	{
		GlobalIndex = AllCursors.Count + 1;
		XnaToFnaHelper.Log($"[ProxyForms] Creating null cursor, globally #{GlobalIndex}");
		INTERNAL_IsNullCursor = true;
		AllCursors.Add(new WeakReference<Cursor>(this));
	}

	public Cursor(Type type, string resource)
	{
		throw new NotSupportedException("Loading cursors from resources currently not supported!");
	}

	public Cursor(IntPtr handle)
	{
		GlobalIndex = AllCursors.Count + 1;
		XnaToFnaHelper.Log($"[ProxyForms] Creating reapplied cursor from #{handle}, globally #{GlobalIndex}");
		_Apply(_FromHandle(handle));
		AllCursors.Add(new WeakReference<Cursor>(this));
	}

	public Cursor(string fileName)
	{
		GlobalIndex = AllCursors.Count + 1;
		XnaToFnaHelper.Log($"[ProxyForms] Creating cursor from file, globally #{GlobalIndex}");
		using (Stream stream = File.OpenRead(fileName))
		{
			_Load(stream);
		}
		AllCursors.Add(new WeakReference<Cursor>(this));
	}

	public Cursor(Stream stream)
	{
		GlobalIndex = AllCursors.Count + 1;
		XnaToFnaHelper.Log($"[ProxyForms] Creating cursor from stream, globally #{GlobalIndex}");
		_Load(stream);
		AllCursors.Add(new WeakReference<Cursor>(this));
	}

	private static Cursor _FromHandle(IntPtr ptr)
	{
		int num = (int)ptr - 1;
		if (num < 0 || AllCursors.Count <= num)
		{
			return null;
		}
		WeakReference<Cursor> weakReference = AllCursors[num];
		if (weakReference == null || !weakReference.TryGetTarget(out var target))
		{
			AllCursors[num] = null;
			return null;
		}
		return target;
	}

	private void _Apply(Cursor other)
	{
		if (other == null)
		{
			INTERNAL_IsNullCursor = true;
		}
	}

	private void _Load(Stream stream)
	{
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	private void Dispose(bool disposing)
	{
		if (!_IsDisposed)
		{
			_IsDisposed = true;
		}
	}
}
