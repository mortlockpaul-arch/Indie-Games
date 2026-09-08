using System;
using System.Runtime.InteropServices;

namespace SDL2;

public static class SDL_mixer
{
	[Flags]
	public enum MIX_InitFlags
	{
		MIX_INIT_FLAC = 1,
		MIX_INIT_MOD = 2,
		MIX_INIT_MP3 = 8,
		MIX_INIT_OGG = 0x10,
		MIX_INIT_MID = 0x20,
		MIX_INIT_OPUS = 0x40
	}

	public struct MIX_Chunk
	{
		public int allocated;

		public IntPtr abuf;

		public uint alen;

		public byte volume;
	}

	public enum Mix_Fading
	{
		MIX_NO_FADING,
		MIX_FADING_OUT,
		MIX_FADING_IN
	}

	public enum Mix_MusicType
	{
		MUS_NONE,
		MUS_CMD,
		MUS_WAV,
		MUS_MOD,
		MUS_MID,
		MUS_OGG,
		MUS_MP3,
		MUS_MP3_MAD_UNUSED,
		MUS_FLAC,
		MUS_MODPLUG_UNUSED,
		MUS_OPUS
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void MixFuncDelegate(IntPtr udata, IntPtr stream, int len);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void Mix_EffectFunc_t(int chan, IntPtr stream, int len, IntPtr udata);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void Mix_EffectDone_t(int chan, IntPtr udata);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void MusicFinishedDelegate();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ChannelFinishedDelegate(int channel);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int SoundFontDelegate(IntPtr a, IntPtr b);

	private const string nativeLibName = "SDL2_mixer";

	public const int SDL_MIXER_MAJOR_VERSION = 2;

	public const int SDL_MIXER_MINOR_VERSION = 0;

	public const int SDL_MIXER_PATCHLEVEL = 5;

	public const int MIX_CHANNELS = 8;

	public static readonly int MIX_DEFAULT_FREQUENCY = 44100;

	public static readonly ushort MIX_DEFAULT_FORMAT = (ushort)(BitConverter.IsLittleEndian ? 32784 : 36880);

	public static readonly int MIX_DEFAULT_CHANNELS = 2;

	public static readonly byte MIX_MAX_VOLUME = 128;

	public static void SDL_MIXER_VERSION(out SDL.SDL_version X)
	{
		X.major = 2;
		X.minor = 0;
		X.patch = 5;
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl, EntryPoint = "MIX_Linked_Version")]
	private static extern IntPtr INTERNAL_MIX_Linked_Version();

	public static SDL.SDL_version MIX_Linked_Version()
	{
		IntPtr ptr = INTERNAL_MIX_Linked_Version();
		return (SDL.SDL_version)Marshal.PtrToStructure(ptr, typeof(SDL.SDL_version));
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_Init(MIX_InitFlags flags);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern void Mix_Quit();

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_OpenAudio(int frequency, ushort format, int channels, int chunksize);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_AllocateChannels(int numchans);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_QuerySpec(out int frequency, out ushort format, out int channels);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr Mix_LoadWAV_RW(IntPtr src, int freesrc);

	public static IntPtr Mix_LoadWAV(string file)
	{
		IntPtr src = SDL.SDL_RWFromFile(file, "rb");
		return Mix_LoadWAV_RW(src, 1);
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mix_LoadMUS")]
	private unsafe static extern IntPtr INTERNAL_Mix_LoadMUS(byte* file);

	public unsafe static IntPtr Mix_LoadMUS(string file)
	{
		byte* ptr = SDL.Utf8EncodeHeap(file);
		IntPtr result = INTERNAL_Mix_LoadMUS(ptr);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr Mix_QuickLoad_WAV([In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] mem);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr Mix_QuickLoad_RAW([In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 1)] byte[] mem, uint len);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern void Mix_FreeChunk(IntPtr chunk);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern void Mix_FreeMusic(IntPtr music);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_GetNumChunkDecoders();

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mix_GetChunkDecoder")]
	private static extern IntPtr INTERNAL_Mix_GetChunkDecoder(int index);

	public static string Mix_GetChunkDecoder(int index)
	{
		return SDL.UTF8_ToManaged(INTERNAL_Mix_GetChunkDecoder(index));
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_GetNumMusicDecoders();

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mix_GetMusicDecoder")]
	private static extern IntPtr INTERNAL_Mix_GetMusicDecoder(int index);

	public static string Mix_GetMusicDecoder(int index)
	{
		return SDL.UTF8_ToManaged(INTERNAL_Mix_GetMusicDecoder(index));
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern Mix_MusicType Mix_GetMusicType(IntPtr music);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mix_GetMusicTitle")]
	public static extern IntPtr INTERNAL_Mix_GetMusicTitle(IntPtr music);

	public static string Mix_GetMusicTitle(IntPtr music)
	{
		return SDL.UTF8_ToManaged(INTERNAL_Mix_GetMusicTitle(music));
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mix_GetMusicTitleTag")]
	public static extern IntPtr INTERNAL_Mix_GetMusicTitleTag(IntPtr music);

	public static string Mix_GetMusicTitleTag(IntPtr music)
	{
		return SDL.UTF8_ToManaged(INTERNAL_Mix_GetMusicTitleTag(music));
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mix_GetMusicArtistTag")]
	public static extern IntPtr INTERNAL_Mix_GetMusicArtistTag(IntPtr music);

	public static string Mix_GetMusicArtistTag(IntPtr music)
	{
		return SDL.UTF8_ToManaged(INTERNAL_Mix_GetMusicArtistTag(music));
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mix_GetMusicAlbumTag")]
	public static extern IntPtr INTERNAL_Mix_GetMusicAlbumTag(IntPtr music);

	public static string Mix_GetMusicAlbumTag(IntPtr music)
	{
		return SDL.UTF8_ToManaged(INTERNAL_Mix_GetMusicAlbumTag(music));
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mix_GetMusicCopyrightTag")]
	public static extern IntPtr INTERNAL_Mix_GetMusicCopyrightTag(IntPtr music);

	public static string Mix_GetMusicCopyrightTag(IntPtr music)
	{
		return SDL.UTF8_ToManaged(INTERNAL_Mix_GetMusicCopyrightTag(music));
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern void Mix_SetPostMix(MixFuncDelegate mix_func, IntPtr arg);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern void Mix_HookMusic(MixFuncDelegate mix_func, IntPtr arg);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern void Mix_HookMusicFinished(MusicFinishedDelegate music_finished);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr Mix_GetMusicHookData();

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern void Mix_ChannelFinished(ChannelFinishedDelegate channel_finished);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_RegisterEffect(int chan, Mix_EffectFunc_t f, Mix_EffectDone_t d, IntPtr arg);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_UnregisterEffect(int channel, Mix_EffectFunc_t f);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_UnregisterAllEffects(int channel);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_SetPanning(int channel, byte left, byte right);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_SetPosition(int channel, short angle, byte distance);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_SetDistance(int channel, byte distance);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_SetReverseStereo(int channel, int flip);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_ReserveChannels(int num);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_GroupChannel(int which, int tag);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_GroupChannels(int from, int to, int tag);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_GroupAvailable(int tag);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_GroupCount(int tag);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_GroupOldest(int tag);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_GroupNewer(int tag);

	public static int Mix_PlayChannel(int channel, IntPtr chunk, int loops)
	{
		return Mix_PlayChannelTimed(channel, chunk, loops, -1);
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_PlayChannelTimed(int channel, IntPtr chunk, int loops, int ticks);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_PlayMusic(IntPtr music, int loops);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_FadeInMusic(IntPtr music, int loops, int ms);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_FadeInMusicPos(IntPtr music, int loops, int ms, double position);

	public static int Mix_FadeInChannel(int channel, IntPtr chunk, int loops, int ms)
	{
		return Mix_FadeInChannelTimed(channel, chunk, loops, ms, -1);
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_FadeInChannelTimed(int channel, IntPtr chunk, int loops, int ms, int ticks);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_Volume(int channel, int volume);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_VolumeChunk(IntPtr chunk, int volume);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_VolumeMusic(int volume);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_GetVolumeMusicStream(IntPtr music);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_HaltChannel(int channel);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_HaltGroup(int tag);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_HaltMusic();

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_ExpireChannel(int channel, int ticks);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_FadeOutChannel(int which, int ms);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_FadeOutGroup(int tag, int ms);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_FadeOutMusic(int ms);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern Mix_Fading Mix_FadingMusic();

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern Mix_Fading Mix_FadingChannel(int which);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern void Mix_Pause(int channel);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern void Mix_Resume(int channel);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_Paused(int channel);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern void Mix_PauseMusic();

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern void Mix_ResumeMusic();

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern void Mix_RewindMusic();

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_PausedMusic();

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_SetMusicPosition(double position);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern double Mix_GetMusicPosition(IntPtr music);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern double Mix_MusicDuration(IntPtr music);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern double Mix_GetMusicLoopStartTime(IntPtr music);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern double Mix_GetMusicLoopEndTime(IntPtr music);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern double Mix_GetMusicLoopLengthTime(IntPtr music);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_Playing(int channel);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_PlayingMusic();

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mix_SetMusicCMD")]
	private unsafe static extern int INTERNAL_Mix_SetMusicCMD(byte* command);

	public unsafe static int Mix_SetMusicCMD(string command)
	{
		byte* ptr = SDL.Utf8EncodeHeap(command);
		int result = INTERNAL_Mix_SetMusicCMD(ptr);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_SetSynchroValue(int value);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_GetSynchroValue();

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mix_SetSoundFonts")]
	private unsafe static extern int INTERNAL_Mix_SetSoundFonts(byte* paths);

	public unsafe static int Mix_SetSoundFonts(string paths)
	{
		byte* ptr = SDL.Utf8EncodeHeap(paths);
		int result = INTERNAL_Mix_SetSoundFonts(ptr);
		Marshal.FreeHGlobal((IntPtr)ptr);
		return result;
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mix_GetSoundFonts")]
	private static extern IntPtr INTERNAL_Mix_GetSoundFonts();

	public static string Mix_GetSoundFonts()
	{
		return SDL.UTF8_ToManaged(INTERNAL_Mix_GetSoundFonts());
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_EachSoundFont(SoundFontDelegate function, IntPtr data);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern int Mix_SetTimidityCfg([In][MarshalAs(UnmanagedType.LPStr)] string path);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mix_GetTimidityCfg")]
	public static extern IntPtr INTERNAL_Mix_GetTimidityCfg();

	public static string Mix_GetTimidityCfg()
	{
		return SDL.UTF8_ToManaged(INTERNAL_Mix_GetTimidityCfg());
	}

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr Mix_GetChunk(int channel);

	[DllImport("SDL2_mixer", CallingConvention = CallingConvention.Cdecl)]
	public static extern void Mix_CloseAudio();
}
