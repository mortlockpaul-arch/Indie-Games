using System;
using System.Runtime.InteropServices;

namespace SDL2;

public static class SDL_image
{
	[Flags]
	public enum IMG_InitFlags
	{
		IMG_INIT_JPG = 1,
		IMG_INIT_PNG = 2,
		IMG_INIT_TIF = 4,
		IMG_INIT_WEBP = 8
	}

	public struct IMG_Animation
	{
		public int w;

		public int h;

		public IntPtr frames;

		public IntPtr delays;
	}

	private const string nativeLibName = "SDL2_image";

	public const int SDL_IMAGE_MAJOR_VERSION = 2;

	public const int SDL_IMAGE_MINOR_VERSION = 0;

	public const int SDL_IMAGE_PATCHLEVEL = 6;

	public static void SDL_IMAGE_VERSION(out SDL.SDL_version X)
	{
		X.major = 2;
		X.minor = 0;
		X.patch = 6;
	}

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IMG_Linked_Version")]
	private static extern IntPtr INTERNAL_IMG_Linked_Version();

	public static SDL.SDL_version IMG_Linked_Version()
	{
		IntPtr ptr = INTERNAL_IMG_Linked_Version();
		return (SDL.SDL_version)Marshal.PtrToStructure(ptr, typeof(SDL.SDL_version));
	}

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl)]
	public static extern int IMG_Init(IMG_InitFlags flags);

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl)]
	public static extern void IMG_Quit();

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IMG_Load")]
	private unsafe static extern IntPtr INTERNAL_IMG_Load(byte* file);

	public unsafe static IntPtr IMG_Load(string file)
	{
		byte* ptr = SDL.Utf8EncodeHeap(file);
		IntPtr result = INTERNAL_IMG_Load(ptr);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr IMG_Load_RW(IntPtr src, int freesrc);

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IMG_LoadTyped_RW")]
	private unsafe static extern IntPtr INTERNAL_IMG_LoadTyped_RW(IntPtr src, int freesrc, byte* type);

	public unsafe static IntPtr IMG_LoadTyped_RW(IntPtr src, int freesrc, string type)
	{
		int num = SDL.Utf8Size(type);
		byte* buffer = stackalloc byte[(int)(uint)num];
		return INTERNAL_IMG_LoadTyped_RW(src, freesrc, SDL.Utf8Encode(type, buffer, num));
	}

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IMG_LoadTexture")]
	private unsafe static extern IntPtr INTERNAL_IMG_LoadTexture(IntPtr renderer, byte* file);

	public unsafe static IntPtr IMG_LoadTexture(IntPtr renderer, string file)
	{
		byte* ptr = SDL.Utf8EncodeHeap(file);
		IntPtr result = INTERNAL_IMG_LoadTexture(renderer, ptr);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr IMG_LoadTexture_RW(IntPtr renderer, IntPtr src, int freesrc);

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IMG_LoadTextureTyped_RW")]
	private unsafe static extern IntPtr INTERNAL_IMG_LoadTextureTyped_RW(IntPtr renderer, IntPtr src, int freesrc, byte* type);

	public unsafe static IntPtr IMG_LoadTextureTyped_RW(IntPtr renderer, IntPtr src, int freesrc, string type)
	{
		byte* ptr = SDL.Utf8EncodeHeap(type);
		IntPtr result = INTERNAL_IMG_LoadTextureTyped_RW(renderer, src, freesrc, ptr);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr IMG_ReadXPMFromArray([In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] xpm);

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IMG_SavePNG")]
	private unsafe static extern int INTERNAL_IMG_SavePNG(IntPtr surface, byte* file);

	public unsafe static int IMG_SavePNG(IntPtr surface, string file)
	{
		byte* ptr = SDL.Utf8EncodeHeap(file);
		int result = INTERNAL_IMG_SavePNG(surface, ptr);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl)]
	public static extern int IMG_SavePNG_RW(IntPtr surface, IntPtr dst, int freedst);

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl, EntryPoint = "IMG_SaveJPG")]
	private unsafe static extern int INTERNAL_IMG_SaveJPG(IntPtr surface, byte* file, int quality);

	public unsafe static int IMG_SaveJPG(IntPtr surface, string file, int quality)
	{
		byte* ptr = SDL.Utf8EncodeHeap(file);
		int result = INTERNAL_IMG_SaveJPG(surface, ptr, quality);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl)]
	public static extern int IMG_SaveJPG_RW(IntPtr surface, IntPtr dst, int freedst, int quality);

	public static string IMG_GetError()
	{
		return SDL.SDL_GetError();
	}

	public static void IMG_SetError(string fmtAndArglist)
	{
		SDL.SDL_SetError(fmtAndArglist);
	}

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr IMG_LoadAnimation([In][MarshalAs(UnmanagedType.LPStr)] string file);

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr IMG_LoadAnimation_RW(IntPtr src, int freesrc);

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr IMG_LoadAnimationTyped_RW(IntPtr src, int freesrc, [In][MarshalAs(UnmanagedType.LPStr)] string type);

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl)]
	public static extern void IMG_FreeAnimation(IntPtr anim);

	[DllImport("SDL2_image", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr IMG_LoadGIFAnimation_RW(IntPtr src);
}
