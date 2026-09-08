using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Audio;

public sealed class DynamicSoundEffectInstance : SoundEffectInstance
{
	internal FAudio.FAudioWaveFormatEx format;

	private List<nint> queuedBuffers;

	private List<uint> queuedSizes;

	private const int MINIMUM_BUFFER_CHECK = 3;

	public int PendingBufferCount
	{
		get
		{
			if (base.IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
			}
			return queuedBuffers.Count;
		}
	}

	public override bool IsLooped
	{
		get
		{
			if (base.IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
			}
			return false;
		}
		set
		{
			if (base.IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
			}
			if (value)
			{
				throw new InvalidOperationException("The method call is invalid.");
			}
		}
	}

	public event EventHandler<EventArgs> BufferNeeded;

	public DynamicSoundEffectInstance(int sampleRate, AudioChannels channels)
		: base(null)
	{
		if ((long)sampleRate < 1000L || (long)sampleRate > 200000L)
		{
			throw new ArgumentOutOfRangeException("sampleRate");
		}
		if (channels < AudioChannels.Mono || channels > AudioChannels.Stereo)
		{
			throw new ArgumentOutOfRangeException("channels");
		}
		FAudio.FAudio_AddRef(SoundEffect.Device().Handle);
		format.wFormatTag = 1;
		format.nChannels = (ushort)channels;
		format.nSamplesPerSec = (uint)sampleRate;
		format.wBitsPerSample = 16;
		format.nBlockAlign = (ushort)(2 * format.nChannels);
		format.nAvgBytesPerSec = format.nBlockAlign * format.nSamplesPerSec;
		format.cbSize = 0;
		queuedBuffers = new List<nint>();
		queuedSizes = new List<uint>();
		InitDSPSettings(format.nChannels);
	}

	public TimeSpan GetSampleDuration(int sizeInBytes)
	{
		if (base.IsDisposed)
		{
			throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
		}
		if (sizeInBytes < 0)
		{
			throw new ArgumentException("Buffer size cannot be negative.");
		}
		return SoundEffect.INTERNAL_GetSampleDuration(sizeInBytes, (int)format.nSamplesPerSec, format.nBlockAlign);
	}

	public int GetSampleSizeInBytes(TimeSpan duration)
	{
		if (base.IsDisposed)
		{
			throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
		}
		if (duration.TotalMilliseconds < 0.0 || duration.TotalMilliseconds > 2147483647.0)
		{
			throw new ArgumentOutOfRangeException("duration");
		}
		return SoundEffect.INTERNAL_GetSampleSizeInBytes(duration, (int)format.nSamplesPerSec, format.nBlockAlign);
	}

	public override void Play()
	{
		if (base.IsDisposed)
		{
			throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
		}
		Update();
		base.Play();
		lock (FrameworkDispatcher.Streams)
		{
			if (!FrameworkDispatcher.Streams.Contains(this))
			{
				FrameworkDispatcher.Streams.Add(this);
			}
		}
	}

	public void SubmitBuffer(byte[] buffer)
	{
		SubmitBuffer(buffer, 0, buffer.Length);
	}

	public void SubmitBuffer(byte[] buffer, int offset, int count)
	{
		if (base.IsDisposed)
		{
			throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
		}
		if (buffer == null || buffer.Length == 0 || buffer.Length % format.nBlockAlign != 0)
		{
			throw new ArgumentException("Buffer is invalid. Ensure that the buffer length is non-zero and meets the block alignment requirements for the audio format.");
		}
		if ((uint)offset >= (uint)buffer.Length || offset % format.nBlockAlign != 0)
		{
			throw new ArgumentException("Byte offset is invalid. Ensure that it falls within the buffer and meets the block alignment requirements for the audio format.");
		}
		if (count <= 0 || (uint)(offset + count) > (uint)buffer.Length || count % format.nBlockAlign != 0)
		{
			throw new ArgumentException("Number of samples to play is invalid. Ensure that it meets the block alignment requirements for the audio format.");
		}
		nint num = FNAPlatform.Malloc(count);
		Marshal.Copy(buffer, offset, num, count);
		lock (queuedBuffers)
		{
			queuedBuffers.Add(num);
			if (base.State != SoundState.Stopped)
			{
				FAudio.FAudioBuffer pBuffer = default(FAudio.FAudioBuffer);
				pBuffer.AudioBytes = (uint)count;
				pBuffer.pAudioData = num;
				pBuffer.PlayLength = pBuffer.AudioBytes / format.nBlockAlign;
				FAudio.FAudioSourceVoice_SubmitSourceBuffer(handle, ref pBuffer, IntPtr.Zero);
			}
			else
			{
				queuedSizes.Add((uint)count);
			}
		}
	}

	public void SubmitFloatBufferEXT(float[] buffer)
	{
		SubmitFloatBufferEXT(buffer, 0, buffer.Length);
	}

	public void SubmitFloatBufferEXT(float[] buffer, int offset, int count)
	{
		if (base.State != SoundState.Stopped && format.wFormatTag == 1)
		{
			throw new InvalidOperationException("Submit a float buffer before Playing!");
		}
		format.wFormatTag = 3;
		format.wBitsPerSample = 32;
		format.nBlockAlign = (ushort)(4 * format.nChannels);
		format.nAvgBytesPerSec = format.nBlockAlign * format.nSamplesPerSec;
		nint num = FNAPlatform.Malloc(count * 4);
		Marshal.Copy(buffer, offset, num, count);
		lock (queuedBuffers)
		{
			queuedBuffers.Add(num);
			if (base.State != SoundState.Stopped)
			{
				FAudio.FAudioBuffer pBuffer = default(FAudio.FAudioBuffer);
				pBuffer.AudioBytes = (uint)(count * 4);
				pBuffer.pAudioData = num;
				pBuffer.PlayLength = pBuffer.AudioBytes / format.nBlockAlign;
				FAudio.FAudioSourceVoice_SubmitSourceBuffer(handle, ref pBuffer, IntPtr.Zero);
			}
			else
			{
				queuedSizes.Add((uint)(count * 4));
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		bool flag = !base.IsDisposed;
		base.Dispose(disposing);
		if (flag)
		{
			FAudio.FAudio_Release(SoundEffect.Device().Handle);
		}
	}

	internal void QueueInitialBuffers()
	{
		FAudio.FAudioBuffer pBuffer = default(FAudio.FAudioBuffer);
		lock (queuedBuffers)
		{
			for (int i = 0; i < queuedBuffers.Count; i++)
			{
				pBuffer.AudioBytes = queuedSizes[i];
				pBuffer.pAudioData = queuedBuffers[i];
				pBuffer.PlayLength = pBuffer.AudioBytes / format.nBlockAlign;
				FAudio.FAudioSourceVoice_SubmitSourceBuffer(handle, ref pBuffer, IntPtr.Zero);
			}
			queuedSizes.Clear();
		}
	}

	internal void ClearBuffers()
	{
		lock (queuedBuffers)
		{
			foreach (nint queuedBuffer in queuedBuffers)
			{
				FNAPlatform.Free(queuedBuffer);
			}
			queuedBuffers.Clear();
			queuedSizes.Clear();
		}
	}

	internal void Update()
	{
		if (base.State != SoundState.Playing)
		{
			return;
		}
		if (handle != IntPtr.Zero)
		{
			FAudio.FAudioSourceVoice_GetState(handle, out var pVoiceState, 256u);
			while (PendingBufferCount > pVoiceState.BuffersQueued)
			{
				lock (queuedBuffers)
				{
					FNAPlatform.Free(queuedBuffers[0]);
					queuedBuffers.RemoveAt(0);
				}
			}
		}
		int num = 3 - PendingBufferCount;
		while (num > 0 && BufferNeeded != null)
		{
			BufferNeeded(this, null);
			num--;
		}
	}
}
