using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Dav1dfile;

public static class Bindings
{
	public enum PixelLayout
	{
		I400,
		I420,
		I422,
		I444
	}

	private const string nativeLibName = "dav1dfile";

	public const uint DAV1DFILE_MAJOR_VERSION = 1u;

	public const uint DAV1DFILE_MINOR_VERSION = 0u;

	public const uint DAV1DFILE_PATCH_VERSION = 0u;

	public const uint DAV1DFILE_COMPILED_VERSION = 10000u;

	[DllImport("dav1dfile", CallingConvention = CallingConvention.Cdecl)]
	public static extern uint df_linked_version();

	[DllImport("dav1dfile", CallingConvention = CallingConvention.Cdecl)]
	public static extern int df_open_from_memory(nint bytes, uint size, out nint context);

	[DllImport("dav1dfile", CallingConvention = CallingConvention.Cdecl, EntryPoint = "df_fopen")]
	private static extern int INTERNAL_df_fopen([MarshalAs(UnmanagedType.LPStr)] string fname, out nint context);

	[DllImport("dav1dfile", CallingConvention = CallingConvention.Cdecl, EntryPoint = "df_fopen")]
	private unsafe static extern int INTERNAL_df_fopen(byte* fname, out nint context);

	[DllImport("dav1dfile", CallingConvention = CallingConvention.Cdecl)]
	public static extern void df_close(nint context);

	[DllImport("dav1dfile", CallingConvention = CallingConvention.Cdecl)]
	public static extern void df_videoinfo(nint context, out int width, out int height, out PixelLayout pixelLayout);

	[DllImport("dav1dfile", CallingConvention = CallingConvention.Cdecl)]
	public static extern void df_videoinfo2(nint context, out int width, out int height, out PixelLayout pixelLayout, out byte hbd);

	[DllImport("dav1dfile", CallingConvention = CallingConvention.Cdecl)]
	public static extern int df_guessframerate(nint context, out double fps);

	[DllImport("dav1dfile", CallingConvention = CallingConvention.Cdecl)]
	public static extern int df_eos(nint context);

	[DllImport("dav1dfile", CallingConvention = CallingConvention.Cdecl)]
	public static extern void df_reset(nint context);

	[DllImport("dav1dfile", CallingConvention = CallingConvention.Cdecl)]
	public static extern int df_readvideo(nint context, int numFrames, out nint yDataPtr, out nint uDataPtr, out nint vDataPtr, out uint yDataLength, out uint uvDataLength, out uint yStride, out uint uvStride);

	private unsafe static byte* Utf8Encode(string str)
	{
		int num = str.Length * 4 + 1;
		byte* ptr = (byte*)Marshal.AllocHGlobal(num);
		fixed (char* chars = str)
		{
			Encoding.UTF8.GetBytes(chars, str.Length + 1, ptr, num);
		}
		return ptr;
	}

	public unsafe static int df_fopen(string fname, out nint context)
	{
		int result;
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			result = INTERNAL_df_fopen(fname, out context);
		}
		else
		{
			byte* ptr = Utf8Encode(fname);
			result = INTERNAL_df_fopen(ptr, out context);
			Marshal.FreeHGlobal((nint)ptr);
		}
		return result;
	}
}
