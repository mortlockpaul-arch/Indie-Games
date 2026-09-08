using System;

namespace Microsoft.Xna.Framework.Input.Touch;

internal static class Touch
{
	private static IntPtr _windowHandle;

	private static object SyncObject;

	internal static IntPtr WindowHandle
	{
		get
		{
			return _windowHandle;
		}
		set
		{
			_windowHandle = value;
		}
	}

	static Touch()
	{
		SyncObject = new object();
	}
}
