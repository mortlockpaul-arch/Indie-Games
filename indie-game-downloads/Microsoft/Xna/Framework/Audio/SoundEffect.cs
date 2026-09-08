using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Audio;

public sealed class SoundEffect : IDisposable
{
	internal class FAudioContext
	{
		public static FAudioContext Context;

		public static bool ProgramExiting;

		public readonly nint Handle;

		public readonly byte[] Handle3D;

		public readonly nint MasterVoice;

		public readonly FAudio.FAudioDeviceDetails DeviceDetails;

		public float CurveDistanceScaler;

		public float DopplerScale;

		public float SpeedOfSound;

		public nint ReverbVoice;

		private FAudio.FAudioVoiceSends reverbSends;

		private FAudioContext(nint ctx, uint devices)
		{
			Handle = ctx;
			uint num;
			for (num = 0u; num < devices; num++)
			{
				FAudio.FAudio_GetDeviceDetails(Handle, num, out DeviceDetails);
				if ((DeviceDetails.Role & FAudio.FAudioDeviceRole.FAudioDefaultGameDevice) == FAudio.FAudioDeviceRole.FAudioDefaultGameDevice)
				{
					break;
				}
			}
			if (num == devices)
			{
				num = 0u;
				FAudio.FAudio_GetDeviceDetails(Handle, num, out DeviceDetails);
			}
			if (FAudio.FAudio_CreateMasteringVoice(Handle, out MasterVoice, 0u, 0u, 0u, num, IntPtr.Zero) != 0)
			{
				FAudio.FAudio_Release(ctx);
				Handle = IntPtr.Zero;
				FNALoggerEXT.LogError("Failed to create mastering voice!");
				return;
			}
			CurveDistanceScaler = 1f;
			DopplerScale = 1f;
			SpeedOfSound = 343.5f;
			Handle3D = new byte[20];
			FAudio.F3DAudioInitialize(DeviceDetails.OutputFormat.dwChannelMask, SpeedOfSound, Handle3D);
			Context = this;
		}

		public void Dispose()
		{
			if (ReverbVoice != IntPtr.Zero)
			{
				FAudio.FAudioVoice_DestroyVoice(ReverbVoice);
				ReverbVoice = IntPtr.Zero;
				FNAPlatform.Free(reverbSends.pSends);
			}
			if (MasterVoice != IntPtr.Zero)
			{
				FAudio.FAudioVoice_DestroyVoice(MasterVoice);
			}
			if (Handle != IntPtr.Zero)
			{
				FAudio.FAudio_Release(Handle);
			}
		}

		public unsafe void AttachReverb(nint voice)
		{
			if (ReverbVoice == IntPtr.Zero)
			{
				FAudio.FAudioCreateReverb(out var ppApo, 0u);
				nint num = FNAPlatform.Malloc(MarshalHelper.SizeOf<FAudio.FAudioEffectChain>());
				FAudio.FAudioEffectChain* ptr = (FAudio.FAudioEffectChain*)num;
				ptr->EffectCount = 1u;
				ptr->pEffectDescriptors = FNAPlatform.Malloc(MarshalHelper.SizeOf<FAudio.FAudioEffectDescriptor>());
				FAudio.FAudioEffectDescriptor* pEffectDescriptors = (FAudio.FAudioEffectDescriptor*)ptr->pEffectDescriptors;
				pEffectDescriptors->InitialState = 1;
				pEffectDescriptors->OutputChannels = ((DeviceDetails.OutputFormat.Format.nChannels != 6) ? 1u : 6u);
				pEffectDescriptors->pEffect = ppApo;
				FAudio.FAudio_CreateSubmixVoice(Handle, out ReverbVoice, 1u, DeviceDetails.OutputFormat.Format.nSamplesPerSec, 0u, 0u, IntPtr.Zero, num);
				FAudio.FAPOBase_Release(ppApo);
				FNAPlatform.Free(ptr->pEffectDescriptors);
				FNAPlatform.Free(num);
				nint num2 = FNAPlatform.Malloc(MarshalHelper.SizeOf<FAudio.FAudioFXReverbParameters>());
				FAudio.FAudioFXReverbParameters* ptr2 = (FAudio.FAudioFXReverbParameters*)num2;
				ptr2->WetDryMix = 100f;
				ptr2->ReflectionsDelay = 7u;
				ptr2->ReverbDelay = 11;
				ptr2->RearDelay = 5;
				ptr2->PositionLeft = 6;
				ptr2->PositionRight = 6;
				ptr2->PositionMatrixLeft = 27;
				ptr2->PositionMatrixRight = 27;
				ptr2->EarlyDiffusion = 15;
				ptr2->LateDiffusion = 15;
				ptr2->LowEQGain = 8;
				ptr2->LowEQCutoff = 4;
				ptr2->HighEQGain = 8;
				ptr2->HighEQCutoff = 6;
				ptr2->RoomFilterFreq = 5000f;
				ptr2->RoomFilterMain = -10f;
				ptr2->RoomFilterHF = -1f;
				ptr2->ReflectionsGain = -26.02f;
				ptr2->ReverbGain = 10f;
				ptr2->DecayTime = 1.49f;
				ptr2->Density = 100f;
				ptr2->RoomSize = 100f;
				FAudio.FAudioVoice_SetEffectParameters(ReverbVoice, 0u, num2, (uint)MarshalHelper.SizeOf<FAudio.FAudioFXReverbParameters>(), 0u);
				FNAPlatform.Free(num2);
				reverbSends = default(FAudio.FAudioVoiceSends);
				reverbSends.SendCount = 2u;
				reverbSends.pSends = FNAPlatform.Malloc(2 * MarshalHelper.SizeOf<FAudio.FAudioSendDescriptor>());
				FAudio.FAudioSendDescriptor* pSends = (FAudio.FAudioSendDescriptor*)reverbSends.pSends;
				pSends->Flags = 0u;
				pSends->pOutputVoice = MasterVoice;
				pSends[1].Flags = 0u;
				pSends[1].pOutputVoice = ReverbVoice;
			}
			FAudio.FAudioVoice_SetOutputVoices(voice, ref reverbSends);
		}

		public static void Create()
		{
			nint ppFAudio;
			try
			{
				FAudio.FAudioCreate(out ppFAudio, 0u, uint.MaxValue);
			}
			catch (Exception ex)
			{
				FNALoggerEXT.LogWarn("FAudio failed to load: " + ex.ToString());
				return;
			}
			FAudio.FAudio_GetDeviceCount(ppFAudio, out var pCount);
			if (pCount == 0)
			{
				FAudio.FAudio_Release(ppFAudio);
				return;
			}
			FAudioContext fAudioContext = new FAudioContext(ppFAudio, pCount);
			if (fAudioContext.Handle == IntPtr.Zero)
			{
				fAudioContext.Dispose();
				return;
			}
			Context = fAudioContext;
			AppDomain.CurrentDomain.ProcessExit += ProgramExit;
		}

		private static void ProgramExit(object sender, EventArgs e)
		{
			ProgramExiting = true;
			if (Context != null)
			{
				GC.Collect();
				Context.Dispose();
			}
		}
	}

	internal List<WeakReference> Instances = new List<WeakReference>();

	internal FAudio.FAudioBuffer handle;

	internal nint formatPtr;

	internal ushort channels;

	internal uint sampleRate;

	internal uint loopStart;

	internal uint loopLength;

	private string name;

	private static readonly object createLock = new object();

	public TimeSpan Duration => TimeSpan.FromSeconds((double)handle.PlayLength / (double)sampleRate);

	public bool IsDisposed { get; private set; }

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
			}
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}
			name = value;
		}
	}

	public static float MasterVolume
	{
		get
		{
			FAudio.FAudioVoice_GetVolume(Device().MasterVoice, out var pVolume);
			return pVolume;
		}
		set
		{
			if (!(value >= -16777216f) || !(value <= 16777216f))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			FAudio.FAudioVoice_SetVolume(Device().MasterVoice, value, 0u);
		}
	}

	public static float DistanceScale
	{
		get
		{
			return Device().CurveDistanceScaler;
		}
		set
		{
			if (value < 0f)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			Device().CurveDistanceScaler = value;
		}
	}

	public static float DopplerScale
	{
		get
		{
			return Device().DopplerScale;
		}
		set
		{
			if (!(value >= 0f))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			Device().DopplerScale = value;
		}
	}

	public static float SpeedOfSound
	{
		get
		{
			return Device().SpeedOfSound;
		}
		set
		{
			if (!(value > 0f))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			FAudioContext fAudioContext = Device();
			fAudioContext.SpeedOfSound = value;
			FAudio.F3DAudioInitialize(fAudioContext.DeviceDetails.OutputFormat.dwChannelMask, fAudioContext.SpeedOfSound, fAudioContext.Handle3D);
		}
	}

	public SoundEffect(byte[] buffer, int sampleRate, AudioChannels channels)
		: this(string.Empty, buffer, 0, buffer.Length, null, 1, (ushort)channels, (uint)sampleRate, (uint)(sampleRate * ((ushort)channels * 2)), (ushort)((ushort)channels * 2), 16, 0, 0)
	{
	}

	public SoundEffect(byte[] buffer, int offset, int count, int sampleRate, AudioChannels channels, int loopStart, int loopLength)
		: this(string.Empty, buffer, offset, count, null, 1, (ushort)channels, (uint)sampleRate, (uint)(sampleRate * ((ushort)channels * 2)), (ushort)((ushort)channels * 2), 16, loopStart, loopLength)
	{
	}

	internal unsafe SoundEffect(string name, byte[] buffer, int offset, int count, byte[] extraData, ushort wFormatTag, ushort nChannels, uint nSamplesPerSec, uint nAvgBytesPerSec, ushort nBlockAlign, ushort wBitsPerSample, int loopStart, int loopLength)
	{
		FAudio.FAudio_AddRef(Device().Handle);
		this.name = name;
		channels = nChannels;
		sampleRate = nSamplesPerSec;
		this.loopStart = (uint)loopStart;
		this.loopLength = (uint)loopLength;
		if (extraData == null)
		{
			formatPtr = FNAPlatform.Malloc(MarshalHelper.SizeOf<FAudio.FAudioWaveFormatEx>());
		}
		else
		{
			formatPtr = FNAPlatform.Malloc(MarshalHelper.SizeOf<FAudio.FAudioWaveFormatEx>() + extraData.Length);
			Marshal.Copy(extraData, 0, formatPtr + MarshalHelper.SizeOf<FAudio.FAudioWaveFormatEx>(), extraData.Length);
		}
		FAudio.FAudioWaveFormatEx* ptr = (FAudio.FAudioWaveFormatEx*)formatPtr;
		ptr->wFormatTag = wFormatTag;
		ptr->nChannels = nChannels;
		ptr->nSamplesPerSec = nSamplesPerSec;
		ptr->nAvgBytesPerSec = nAvgBytesPerSec;
		ptr->nBlockAlign = nBlockAlign;
		ptr->wBitsPerSample = wBitsPerSample;
		ptr->cbSize = (ushort)((extraData != null) ? ((uint)extraData.Length) : 0u);
		handle = default(FAudio.FAudioBuffer);
		handle.Flags = 64u;
		handle.pContext = IntPtr.Zero;
		handle.AudioBytes = (uint)count;
		handle.pAudioData = FNAPlatform.Malloc(count);
		Marshal.Copy(buffer, offset, handle.pAudioData, count);
		handle.PlayBegin = 0u;
		switch (wFormatTag)
		{
		case 1:
			handle.PlayLength = (uint)(count / nChannels / (wBitsPerSample / 8));
			break;
		case 2:
			handle.PlayLength = (uint)(count / nBlockAlign * ((nBlockAlign / nChannels - 6) * 2));
			break;
		case 358:
		{
			FAudio.FAudioXMA2WaveFormatEx* ptr2 = (FAudio.FAudioXMA2WaveFormatEx*)formatPtr;
			handle.PlayLength = ptr2->dwPlayLength;
			break;
		}
		}
		handle.LoopBegin = 0u;
		handle.LoopLength = 0u;
		handle.LoopCount = 0u;
	}

	~SoundEffect()
	{
		if (!FAudioContext.ProgramExiting && Instances.Count > 0)
		{
			GC.ReRegisterForFinalize(this);
		}
		else
		{
			Dispose();
		}
	}

	public void Dispose()
	{
		if (IsDisposed)
		{
			return;
		}
		WeakReference[] array = Instances.ToArray();
		foreach (WeakReference weakReference in array)
		{
			object target = weakReference.Target;
			if (target != null)
			{
				(target as IDisposable).Dispose();
			}
		}
		Instances.Clear();
		FAudio.FAudio_Release(Device().Handle);
		FNAPlatform.Free(formatPtr);
		FNAPlatform.Free(handle.pAudioData);
		IsDisposed = true;
	}

	public bool Play()
	{
		return Play(1f, 0f, 0f);
	}

	public bool Play(float volume, float pitch, float pan)
	{
		if (!FrameworkDispatcher.IsUpdated)
		{
			throw new InvalidOperationException("FrameworkDispatcher.Update has not been called. Regular FrameworkDispatcher.Update calls are necessary for fire and forget sound effects and framework events to function correctly. See http://go.microsoft.com/fwlink/?LinkId=193853 for details.");
		}
		if (IsDisposed)
		{
			throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
		}
		SoundEffectInstance soundEffectInstance = new SoundEffectInstance(this);
		soundEffectInstance.Volume = volume;
		soundEffectInstance.Pitch = pitch;
		soundEffectInstance.Pan = pan;
		soundEffectInstance.Play();
		if (soundEffectInstance.State != SoundState.Playing)
		{
			soundEffectInstance.Dispose();
			return false;
		}
		return true;
	}

	public SoundEffectInstance CreateInstance()
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
		}
		return new SoundEffectInstance(this);
	}

	public static TimeSpan GetSampleDuration(int sizeInBytes, int sampleRate, AudioChannels channels)
	{
		if (sizeInBytes < 0)
		{
			throw new ArgumentException("Buffer size cannot be negative.", "sizeInBytes");
		}
		if ((long)sampleRate < 1000L || (long)sampleRate > 200000L)
		{
			throw new ArgumentOutOfRangeException("sampleRate");
		}
		if (channels < AudioChannels.Mono || channels > AudioChannels.Stereo)
		{
			throw new ArgumentOutOfRangeException("channels");
		}
		return INTERNAL_GetSampleDuration(sizeInBytes, sampleRate, 2 * (int)channels);
	}

	public static int GetSampleSizeInBytes(TimeSpan duration, int sampleRate, AudioChannels channels)
	{
		if (duration.TotalMilliseconds < 0.0 || duration.TotalMilliseconds > 2147483647.0)
		{
			throw new ArgumentOutOfRangeException("duration");
		}
		if ((long)sampleRate < 1000L || (long)sampleRate > 200000L)
		{
			throw new ArgumentOutOfRangeException("sampleRate");
		}
		if (channels < AudioChannels.Mono || channels > AudioChannels.Stereo)
		{
			throw new ArgumentOutOfRangeException("channels");
		}
		return INTERNAL_GetSampleSizeInBytes(duration, sampleRate, 2 * (int)channels);
	}

	public static SoundEffect FromStream(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		int num = 0;
		int num2 = 0;
		ushort wFormatTag;
		ushort nChannels;
		uint nSamplesPerSec;
		uint nAvgBytesPerSec;
		ushort nBlockAlign;
		ushort wBitsPerSample;
		byte[] array;
		using (BinaryReader binaryReader = new BinaryReader(stream))
		{
			string text = new string(binaryReader.ReadChars(4));
			if (text != "RIFF")
			{
				throw new NotSupportedException("Specified stream is not a wave file.");
			}
			binaryReader.ReadUInt32();
			string text2 = new string(binaryReader.ReadChars(4));
			if (text2 != "WAVE")
			{
				throw new NotSupportedException("Specified stream is not a wave file.");
			}
			string text3 = new string(binaryReader.ReadChars(4));
			while (text3 != "fmt ")
			{
				binaryReader.ReadBytes(binaryReader.ReadInt32());
				text3 = new string(binaryReader.ReadChars(4));
			}
			int num3 = binaryReader.ReadInt32();
			wFormatTag = binaryReader.ReadUInt16();
			nChannels = binaryReader.ReadUInt16();
			nSamplesPerSec = binaryReader.ReadUInt32();
			nAvgBytesPerSec = binaryReader.ReadUInt32();
			nBlockAlign = binaryReader.ReadUInt16();
			wBitsPerSample = binaryReader.ReadUInt16();
			if (num3 > 16)
			{
				binaryReader.ReadBytes(num3 - 16);
			}
			string text4 = new string(binaryReader.ReadChars(4));
			while (text4.ToLowerInvariant() != "data")
			{
				binaryReader.ReadBytes(binaryReader.ReadInt32());
				text4 = new string(binaryReader.ReadChars(4));
			}
			if (text4 != "data")
			{
				throw new NotSupportedException("Specified wave file is not supported.");
			}
			int count = binaryReader.ReadInt32();
			array = binaryReader.ReadBytes(count);
			while (binaryReader.PeekChar() != -1)
			{
				char[] array2 = binaryReader.ReadChars(4);
				if (array2.Length < 4)
				{
					break;
				}
				byte[] array3 = binaryReader.ReadBytes(4);
				if (array3.Length < 4)
				{
					break;
				}
				string text5 = new string(array2);
				int count2 = BitConverter.ToInt32(array3, 0);
				if (text5 == "smpl")
				{
					binaryReader.ReadUInt32();
					binaryReader.ReadUInt32();
					binaryReader.ReadUInt32();
					binaryReader.ReadUInt32();
					binaryReader.ReadUInt32();
					binaryReader.ReadUInt32();
					binaryReader.ReadUInt32();
					uint num4 = binaryReader.ReadUInt32();
					int num5 = binaryReader.ReadInt32();
					for (int i = 0; i < num4; i++)
					{
						binaryReader.ReadUInt32();
						binaryReader.ReadUInt32();
						int num6 = binaryReader.ReadInt32();
						int num7 = binaryReader.ReadInt32();
						binaryReader.ReadUInt32();
						binaryReader.ReadUInt32();
						if (i == 0)
						{
							num = num6;
							num2 = num7;
						}
					}
					if (num5 != 0)
					{
						binaryReader.ReadBytes(num5);
					}
				}
				else
				{
					binaryReader.ReadBytes(count2);
				}
			}
		}
		return new SoundEffect(string.Empty, array, 0, array.Length, null, wFormatTag, nChannels, nSamplesPerSec, nAvgBytesPerSec, nBlockAlign, wBitsPerSample, num, num2 - num);
	}

	internal static TimeSpan INTERNAL_GetSampleDuration(int sizeInBytes, int sampleRate, int blockAlign)
	{
		int milliseconds = (int)((float)(sizeInBytes / blockAlign) / ((float)sampleRate / 1000f));
		return new TimeSpan(0, 0, 0, 0, milliseconds);
	}

	internal static int INTERNAL_GetSampleSizeInBytes(TimeSpan duration, int sampleRate, int blockAlign)
	{
		return (int)(duration.TotalSeconds * (double)sampleRate * (double)blockAlign);
	}

	internal static FAudioContext Device()
	{
		if (FAudioContext.Context != null)
		{
			return FAudioContext.Context;
		}
		lock (createLock)
		{
			if (FAudioContext.Context != null)
			{
				return FAudioContext.Context;
			}
			FAudioContext.Create();
			if (FAudioContext.Context == null)
			{
				throw new NoAudioHardwareException();
			}
		}
		return FAudioContext.Context;
	}
}
