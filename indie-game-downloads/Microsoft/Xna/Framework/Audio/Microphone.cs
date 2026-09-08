using System;
using System.Collections.ObjectModel;

namespace Microsoft.Xna.Framework.Audio;

public class Microphone
{
	public readonly string Name;

	private TimeSpan bufferDuration;

	private readonly nint handle;

	internal static ReadOnlyCollection<Microphone> micList;

	internal const int SAMPLERATE = 44100;

	public static ReadOnlyCollection<Microphone> All
	{
		get
		{
			if (micList == null)
			{
				micList = new ReadOnlyCollection<Microphone>(FNAPlatform.GetMicrophones());
			}
			return micList;
		}
	}

	public static Microphone Default
	{
		get
		{
			if (All.Count == 0)
			{
				return null;
			}
			return All[0];
		}
	}

	public TimeSpan BufferDuration
	{
		get
		{
			return bufferDuration;
		}
		set
		{
			if (value.TotalMilliseconds < 100.0 || value.TotalMilliseconds > 1000.0 || value.TotalMilliseconds % 10.0 != 0.0)
			{
				throw new ArgumentOutOfRangeException("value", "Microphone buffer duration must be between 100ms and 1sec and  10ms aligned.");
			}
			bufferDuration = value;
		}
	}

	public bool IsHeadset => true;

	public int SampleRate => 44100;

	public MicrophoneState State { get; private set; }

	public event EventHandler<EventArgs> BufferReady;

	internal Microphone(nint id, string name)
	{
		handle = id;
		Name = name;
		bufferDuration = TimeSpan.FromSeconds(1.0);
		State = MicrophoneState.Stopped;
	}

	public int GetData(byte[] buffer)
	{
		return GetData(buffer, 0, buffer.Length);
	}

	public int GetData(byte[] buffer, int offset, int count)
	{
		if (buffer == null || buffer.Length == 0 || buffer.Length % 2 != 0)
		{
			throw new ArgumentException("Buffer is invalid. Ensure that the buffer length is non-zero and meets the block alignment requirements for the audio format.");
		}
		if ((uint)offset >= (uint)buffer.Length || offset % 2 != 0)
		{
			throw new ArgumentException("Byte offset is invalid. Ensure that it falls within the buffer and meets the block alignment requirements for the audio format.");
		}
		if (count <= 0 || (uint)(offset + count) > (uint)buffer.Length || count % 2 != 0)
		{
			throw new ArgumentException("Number of samples to play is invalid. Ensure that it meets the block alignment requirements for the audio format.");
		}
		return FNAPlatform.GetMicrophoneSamples(handle, buffer, offset, count);
	}

	public TimeSpan GetSampleDuration(int sizeInBytes)
	{
		if (sizeInBytes < 0)
		{
			throw new ArgumentException("Buffer size cannot be negative.");
		}
		return SoundEffect.INTERNAL_GetSampleDuration(sizeInBytes, SampleRate, 2);
	}

	public int GetSampleSizeInBytes(TimeSpan duration)
	{
		if (duration.TotalMilliseconds < 0.0 || duration.TotalMilliseconds > 2147483647.0)
		{
			throw new ArgumentOutOfRangeException("duration");
		}
		return SoundEffect.INTERNAL_GetSampleSizeInBytes(duration, SampleRate, 2);
	}

	public void Start()
	{
		FNAPlatform.StartMicrophone(handle);
		State = MicrophoneState.Started;
	}

	public void Stop()
	{
		FNAPlatform.StopMicrophone(handle);
		State = MicrophoneState.Stopped;
	}

	internal void CheckBuffer()
	{
		if (BufferReady != null && GetSampleDuration(FNAPlatform.GetMicrophoneQueuedBytes(handle)) > bufferDuration)
		{
			BufferReady(this, EventArgs.Empty);
		}
	}
}
