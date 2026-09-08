using System;
using System.Runtime.InteropServices;
using System.Text;

public static class Theorafile
{
	public enum SeekWhence
	{
		TF_SEEK_SET,
		TF_SEEK_CUR,
		TF_SEEK_END
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate nint read_func(nint ptr, nint size, nint nmemb, nint datasource);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int seek_func(nint datasource, long offset, SeekWhence whence);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int close_func(nint datasource);

	public enum th_pixel_fmt
	{
		TH_PF_420,
		TH_PF_RSVD,
		TH_PF_422,
		TH_PF_444,
		TH_PF_NFORMATS
	}

	public struct tf_callbacks
	{
		public read_func read_func;

		public seek_func seek_func;

		public close_func close_func;
	}

	private const string nativeLibName = "libtheorafile";

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

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl, EntryPoint = "tf_open_callbacks")]
	private static extern int INTERNAL_tf_open_callbacks(nint datasource, nint file, tf_callbacks io);

	public static int tf_open_callbacks(nint datasource, out nint file, tf_callbacks io)
	{
		file = AllocTheoraFile();
		return INTERNAL_tf_open_callbacks(datasource, file, io);
	}

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl, EntryPoint = "tf_fopen")]
	private unsafe static extern int INTERNAL_tf_fopen(byte* fname, nint file);

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl, EntryPoint = "tf_fopen")]
	private static extern int INTERNAL_tf_fopen([MarshalAs(UnmanagedType.LPStr)] string fname, nint file);

	public unsafe static int tf_fopen(string fname, out nint file)
	{
		file = AllocTheoraFile();
		int result;
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			result = INTERNAL_tf_fopen(fname, file);
		}
		else
		{
			byte* ptr = Utf8Encode(fname);
			result = INTERNAL_tf_fopen(ptr, file);
			Marshal.FreeHGlobal((nint)ptr);
		}
		return result;
	}

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl, EntryPoint = "tf_close")]
	private static extern void INTERNAL_tf_close(nint file);

	public static void tf_close(ref nint file)
	{
		INTERNAL_tf_close(file);
		Marshal.FreeHGlobal(file);
		file = IntPtr.Zero;
	}

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tf_hasaudio(nint file);

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tf_hasvideo(nint file);

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl)]
	public static extern void tf_videoinfo(nint file, out int width, out int height, out double fps, out th_pixel_fmt fmt);

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl)]
	public static extern void tf_audioinfo(nint file, out int channels, out int samplerate);

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tf_setaudiotrack(nint file, int track);

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tf_setvideotrack(nint file, int track);

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tf_eos(nint file);

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl)]
	public static extern void tf_reset(nint file);

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tf_readvideo(nint file, nint buffer, int numframes);

	[DllImport("libtheorafile", CallingConvention = CallingConvention.Cdecl)]
	public static extern int tf_readaudio(nint file, nint buffer, int length);

	private static nint AllocTheoraFile()
	{
		PlatformID platform = Environment.OSVersion.Platform;
		if (IntPtr.Size == 4)
		{
			return Marshal.AllocHGlobal(1160);
		}
		if (IntPtr.Size == 8)
		{
			return platform switch
			{
				PlatformID.Unix => Marshal.AllocHGlobal(1472), 
				PlatformID.Win32NT => Marshal.AllocHGlobal(1328), 
				_ => throw new NotSupportedException("Unhandled platform!"), 
			};
		}
		throw new NotSupportedException("Unhandled architecture!");
	}
}
