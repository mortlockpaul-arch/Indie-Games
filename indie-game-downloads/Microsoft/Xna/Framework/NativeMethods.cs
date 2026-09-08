using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using XnaToFna.ProxyDrawing;

namespace Microsoft.Xna.Framework;

internal sealed class NativeMethods
{
	internal enum WindowMessage : uint
	{
		Destroy = 2u,
		Close = 16u,
		Quit = 18u,
		Paint = 15u,
		SetCursor = 32u,
		ActivateApplication = 28u,
		EnterMenuLoop = 529u,
		ExitMenuLoop = 530u,
		NonClientHitTest = 132u,
		PowerBroadcast = 536u,
		SystemCommand = 274u,
		GetMinMax = 36u,
		KeyDown = 256u,
		KeyUp = 257u,
		Character = 258u,
		SystemKeyDown = 260u,
		SystemKeyUp = 261u,
		SystemCharacter = 262u,
		MouseMove = 512u,
		LeftButtonDown = 513u,
		LeftButtonUp = 514u,
		LeftButtonDoubleClick = 515u,
		RightButtonDown = 516u,
		RightButtonUp = 517u,
		RightButtonDoubleClick = 518u,
		MiddleButtonDown = 519u,
		MiddleButtonUp = 520u,
		MiddleButtonDoubleClick = 521u,
		MouseWheel = 522u,
		XButtonDown = 523u,
		XButtonUp = 524u,
		XButtonDoubleClick = 525u,
		MouseFirst = LeftButtonDown,
		MouseLast = XButtonDoubleClick,
		EnterSizeMove = 561u,
		ExitSizeMove = 562u,
		Size = 5u
	}

	public enum MouseButtons
	{
		Left = 1,
		Right = 2,
		Middle = 0x10,
		Side1 = 0x20,
		Side2 = 0x40
	}

	public struct Message
	{
		public IntPtr hWnd;

		public WindowMessage msg;

		public IntPtr wParam;

		public IntPtr lParam;

		public uint time;

		public XnaToFna.ProxyDrawing.Point p;
	}

	public struct MinMaxInformation
	{
		public XnaToFna.ProxyDrawing.Point reserved;

		public XnaToFna.ProxyDrawing.Point MaxSize;

		public XnaToFna.ProxyDrawing.Point MaxPosition;

		public XnaToFna.ProxyDrawing.Point MinTrackSize;

		public XnaToFna.ProxyDrawing.Point MaxTrackSize;
	}

	public struct MonitorInformation
	{
		public uint Size;

		public XnaToFna.ProxyDrawing.Rectangle MonitorRectangle;

		public XnaToFna.ProxyDrawing.Rectangle WorkRectangle;

		public uint Flags;
	}

	public struct RECT
	{
		public int Left;

		public int Top;

		public int Right;

		public int Bottom;
	}

	public struct POINT
	{
		public int X;

		public int Y;
	}

	private NativeMethods()
	{
	}

	[DllImport("kernel32")]
	[SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
	[SuppressUnmanagedCodeSecurity]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

	[DllImport("kernel32")]
	[SuppressUnmanagedCodeSecurity]
	[SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool QueryPerformanceCounter(ref long PerformanceCount);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	[SuppressUnmanagedCodeSecurity]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

	[DllImport("user32.dll")]
	[SuppressUnmanagedCodeSecurity]
	[SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

	[DllImport("user32.dll")]
	[SuppressUnmanagedCodeSecurity]
	[SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool GetClientRect(IntPtr hWnd, out RECT rect);

	[DllImport("user32.dll")]
	[SuppressUnmanagedCodeSecurity]
	[SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static extern bool ClientToScreen(IntPtr hWnd, out POINT point);

	[DllImport("user32.dll")]
	[SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
	[SuppressUnmanagedCodeSecurity]
	internal static extern IntPtr MonitorFromWindow(IntPtr hWnd, uint flags);
}
