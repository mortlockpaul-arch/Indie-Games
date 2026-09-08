using System;
using System.Runtime.InteropServices;

namespace SDL2;

public static class SDL_ttf
{
	private const string nativeLibName = "SDL2_ttf";

	public const int SDL_TTF_MAJOR_VERSION = 2;

	public const int SDL_TTF_MINOR_VERSION = 0;

	public const int SDL_TTF_PATCHLEVEL = 16;

	public const int UNICODE_BOM_NATIVE = 65279;

	public const int UNICODE_BOM_SWAPPED = 65534;

	public const int TTF_STYLE_NORMAL = 0;

	public const int TTF_STYLE_BOLD = 1;

	public const int TTF_STYLE_ITALIC = 2;

	public const int TTF_STYLE_UNDERLINE = 4;

	public const int TTF_STYLE_STRIKETHROUGH = 8;

	public const int TTF_HINTING_NORMAL = 0;

	public const int TTF_HINTING_LIGHT = 1;

	public const int TTF_HINTING_MONO = 2;

	public const int TTF_HINTING_NONE = 3;

	public const int TTF_HINTING_LIGHT_SUBPIXEL = 4;

	public static void SDL_TTF_VERSION(out SDL.SDL_version X)
	{
		X.major = 2;
		X.minor = 0;
		X.patch = 16;
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_LinkedVersion")]
	private static extern IntPtr INTERNAL_TTF_LinkedVersion();

	public static SDL.SDL_version TTF_LinkedVersion()
	{
		IntPtr ptr = INTERNAL_TTF_LinkedVersion();
		return (SDL.SDL_version)Marshal.PtrToStructure(ptr, typeof(SDL.SDL_version));
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern void TTF_ByteSwappedUNICODE(int swapped);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_Init();

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_OpenFont")]
	private unsafe static extern IntPtr INTERNAL_TTF_OpenFont(byte* file, int ptsize);

	public unsafe static IntPtr TTF_OpenFont(string file, int ptsize)
	{
		byte* ptr = SDL.Utf8EncodeHeap(file);
		IntPtr result = INTERNAL_TTF_OpenFont(ptr, ptsize);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_OpenFontRW(IntPtr src, int freesrc, int ptsize);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_OpenFontIndex")]
	private unsafe static extern IntPtr INTERNAL_TTF_OpenFontIndex(byte* file, int ptsize, long index);

	public unsafe static IntPtr TTF_OpenFontIndex(string file, int ptsize, long index)
	{
		byte* ptr = SDL.Utf8EncodeHeap(file);
		IntPtr result = INTERNAL_TTF_OpenFontIndex(ptr, ptsize, index);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_OpenFontIndexRW(IntPtr src, int freesrc, int ptsize, long index);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_SetFontSize(IntPtr font, int ptsize);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_GetFontStyle(IntPtr font);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern void TTF_SetFontStyle(IntPtr font, int style);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_GetFontOutline(IntPtr font);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern void TTF_SetFontOutline(IntPtr font, int outline);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_GetFontHinting(IntPtr font);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern void TTF_SetFontHinting(IntPtr font, int hinting);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_FontHeight(IntPtr font);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_FontAscent(IntPtr font);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_FontDescent(IntPtr font);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_FontLineSkip(IntPtr font);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_GetFontKerning(IntPtr font);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern void TTF_SetFontKerning(IntPtr font, int allowed);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_FontFaces(IntPtr font);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_FontFaceIsFixedWidth(IntPtr font);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_FontFaceFamilyName")]
	private static extern IntPtr INTERNAL_TTF_FontFaceFamilyName(IntPtr font);

	public static string TTF_FontFaceFamilyName(IntPtr font)
	{
		return SDL.UTF8_ToManaged(INTERNAL_TTF_FontFaceFamilyName(font));
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_FontFaceStyleName")]
	private static extern IntPtr INTERNAL_TTF_FontFaceStyleName(IntPtr font);

	public static string TTF_FontFaceStyleName(IntPtr font)
	{
		return SDL.UTF8_ToManaged(INTERNAL_TTF_FontFaceStyleName(font));
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_GlyphIsProvided(IntPtr font, ushort ch);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_GlyphIsProvided32(IntPtr font, uint ch);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_GlyphMetrics(IntPtr font, ushort ch, out int minx, out int maxx, out int miny, out int maxy, out int advance);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_GlyphMetrics32(IntPtr font, uint ch, out int minx, out int maxx, out int miny, out int maxy, out int advance);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_SizeText(IntPtr font, [In][MarshalAs(UnmanagedType.LPStr)] string text, out int w, out int h);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_SizeUTF8")]
	public unsafe static extern int INTERNAL_TTF_SizeUTF8(IntPtr font, byte* text, out int w, out int h);

	public unsafe static int TTF_SizeUTF8(IntPtr font, string text, out int w, out int h)
	{
		byte* ptr = SDL.Utf8EncodeHeap(text);
		int result = INTERNAL_TTF_SizeUTF8(font, ptr, out w, out h);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_SizeUNICODE(IntPtr font, [In][MarshalAs(UnmanagedType.LPWStr)] string text, out int w, out int h);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_MeasureText(IntPtr font, [In][MarshalAs(UnmanagedType.LPStr)] string text, int measure_width, out int extent, out int count);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_MeasureUTF8")]
	public unsafe static extern int INTERNAL_TTF_MeasureUTF8(IntPtr font, byte* text, int measure_width, out int extent, out int count);

	public unsafe static int TTF_MeasureUTF8(IntPtr font, string text, int measure_width, out int extent, out int count)
	{
		byte* ptr = SDL.Utf8EncodeHeap(text);
		int result = INTERNAL_TTF_MeasureUTF8(font, ptr, measure_width, out extent, out count);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_MeasureUNICODE(IntPtr font, [In][MarshalAs(UnmanagedType.LPWStr)] string text, int measure_width, out int extent, out int count);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderText_Solid(IntPtr font, [In][MarshalAs(UnmanagedType.LPStr)] string text, SDL.SDL_Color fg);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_RenderUTF8_Solid")]
	private unsafe static extern IntPtr INTERNAL_TTF_RenderUTF8_Solid(IntPtr font, byte* text, SDL.SDL_Color fg);

	public unsafe static IntPtr TTF_RenderUTF8_Solid(IntPtr font, string text, SDL.SDL_Color fg)
	{
		byte* ptr = SDL.Utf8EncodeHeap(text);
		IntPtr result = INTERNAL_TTF_RenderUTF8_Solid(font, ptr, fg);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderUNICODE_Solid(IntPtr font, [In][MarshalAs(UnmanagedType.LPWStr)] string text, SDL.SDL_Color fg);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderText_Solid_Wrapped(IntPtr font, [In][MarshalAs(UnmanagedType.LPStr)] string text, SDL.SDL_Color fg, uint wrapLength);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_RenderUTF8_Solid_Wrapped")]
	public unsafe static extern IntPtr INTERNAL_TTF_RenderUTF8_Solid_Wrapped(IntPtr font, byte* text, SDL.SDL_Color fg, uint wrapLength);

	public unsafe static IntPtr TTF_RenderUTF8_Solid_Wrapped(IntPtr font, string text, SDL.SDL_Color fg, uint wrapLength)
	{
		byte* ptr = SDL.Utf8EncodeHeap(text);
		IntPtr result = INTERNAL_TTF_RenderUTF8_Solid_Wrapped(font, ptr, fg, wrapLength);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderUNICODE_Solid_Wrapped(IntPtr font, [In][MarshalAs(UnmanagedType.LPWStr)] string text, SDL.SDL_Color fg, uint wrapLength);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderGlyph_Solid(IntPtr font, ushort ch, SDL.SDL_Color fg);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderGlyph32_Solid(IntPtr font, uint ch, SDL.SDL_Color fg);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderText_Shaded(IntPtr font, [In][MarshalAs(UnmanagedType.LPStr)] string text, SDL.SDL_Color fg, SDL.SDL_Color bg);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_RenderUTF8_Shaded")]
	private unsafe static extern IntPtr INTERNAL_TTF_RenderUTF8_Shaded(IntPtr font, byte* text, SDL.SDL_Color fg, SDL.SDL_Color bg);

	public unsafe static IntPtr TTF_RenderUTF8_Shaded(IntPtr font, string text, SDL.SDL_Color fg, SDL.SDL_Color bg)
	{
		byte* ptr = SDL.Utf8EncodeHeap(text);
		IntPtr result = INTERNAL_TTF_RenderUTF8_Shaded(font, ptr, fg, bg);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderUNICODE_Shaded(IntPtr font, [In][MarshalAs(UnmanagedType.LPWStr)] string text, SDL.SDL_Color fg, SDL.SDL_Color bg);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderText_Shaded_Wrapped(IntPtr font, [In][MarshalAs(UnmanagedType.LPStr)] string text, SDL.SDL_Color fg, SDL.SDL_Color bg, uint wrapLength);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_RenderUTF8_Shaded_Wrapped")]
	public unsafe static extern IntPtr INTERNAL_TTF_RenderUTF8_Shaded_Wrapped(IntPtr font, byte* text, SDL.SDL_Color fg, SDL.SDL_Color bg, uint wrapLength);

	public unsafe static IntPtr TTF_RenderUTF8_Shaded_Wrapped(IntPtr font, string text, SDL.SDL_Color fg, SDL.SDL_Color bg, uint wrapLength)
	{
		byte* ptr = SDL.Utf8EncodeHeap(text);
		IntPtr result = INTERNAL_TTF_RenderUTF8_Shaded_Wrapped(font, ptr, fg, bg, wrapLength);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderUNICODE_Shaded_Wrapped(IntPtr font, [In][MarshalAs(UnmanagedType.LPWStr)] string text, SDL.SDL_Color fg, SDL.SDL_Color bg, uint wrapLength);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderGlyph_Shaded(IntPtr font, ushort ch, SDL.SDL_Color fg, SDL.SDL_Color bg);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderGlyph32_Shaded(IntPtr font, uint ch, SDL.SDL_Color fg, SDL.SDL_Color bg);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderText_Blended(IntPtr font, [In][MarshalAs(UnmanagedType.LPStr)] string text, SDL.SDL_Color fg);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_RenderUTF8_Blended")]
	private unsafe static extern IntPtr INTERNAL_TTF_RenderUTF8_Blended(IntPtr font, byte* text, SDL.SDL_Color fg);

	public unsafe static IntPtr TTF_RenderUTF8_Blended(IntPtr font, string text, SDL.SDL_Color fg)
	{
		byte* ptr = SDL.Utf8EncodeHeap(text);
		IntPtr result = INTERNAL_TTF_RenderUTF8_Blended(font, ptr, fg);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderUNICODE_Blended(IntPtr font, [In][MarshalAs(UnmanagedType.LPWStr)] string text, SDL.SDL_Color fg);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderText_Blended_Wrapped(IntPtr font, [In][MarshalAs(UnmanagedType.LPStr)] string text, SDL.SDL_Color fg, uint wrapped);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_RenderUTF8_Blended_Wrapped")]
	private unsafe static extern IntPtr INTERNAL_TTF_RenderUTF8_Blended_Wrapped(IntPtr font, byte* text, SDL.SDL_Color fg, uint wrapped);

	public unsafe static IntPtr TTF_RenderUTF8_Blended_Wrapped(IntPtr font, string text, SDL.SDL_Color fg, uint wrapped)
	{
		byte* ptr = SDL.Utf8EncodeHeap(text);
		IntPtr result = INTERNAL_TTF_RenderUTF8_Blended_Wrapped(font, ptr, fg, wrapped);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderUNICODE_Blended_Wrapped(IntPtr font, [In][MarshalAs(UnmanagedType.LPWStr)] string text, SDL.SDL_Color fg, uint wrapped);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderGlyph_Blended(IntPtr font, ushort ch, SDL.SDL_Color fg);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr TTF_RenderGlyph32_Blended(IntPtr font, uint ch, SDL.SDL_Color fg);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_SetDirection(int direction);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_SetScript(int script);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern void TTF_CloseFont(IntPtr font);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern void TTF_Quit();

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_WasInit();

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int SDL_GetFontKerningSize(IntPtr font, int prev_index, int index);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_GetFontKerningSizeGlyphs(IntPtr font, ushort previous_ch, ushort ch);

	[DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl)]
	public static extern int TTF_GetFontKerningSizeGlyphs32(IntPtr font, ushort previous_ch, ushort ch);
}
