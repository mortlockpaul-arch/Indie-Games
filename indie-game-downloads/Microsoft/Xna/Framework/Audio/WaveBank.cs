using System;
using System.IO;
using MonoGame.Utilities;

namespace Microsoft.Xna.Framework.Audio;

public class WaveBank : IDisposable
{
	private nint handle;

	private AudioEngine engine;

	private WeakReference selfReference;

	private nint bankData;

	private nint bankDataLen;

	public bool IsDisposed { get; private set; }

	public bool IsPrepared
	{
		get
		{
			FAudio.FACTWaveBank_GetState(handle, out var pdwState);
			return (pdwState & 4) != 0;
		}
	}

	public bool IsInUse
	{
		get
		{
			FAudio.FACTWaveBank_GetState(handle, out var pdwState);
			return (pdwState & 0x80) != 0;
		}
	}

	public event EventHandler<EventArgs> Disposing;

	public WaveBank(AudioEngine audioEngine, string nonStreamingWaveBankFilename)
	{
		if (audioEngine == null)
		{
			throw new ArgumentNullException("audioEngine", "You must pass in a valid audio engine.");
		}
		if (string.IsNullOrEmpty(nonStreamingWaveBankFilename))
		{
			throw new ArgumentNullException("nonStreamingWaveBankFilename", "This method does not accept null for this parameter.");
		}
		bankData = TitleContainer.ReadToPointer(nonStreamingWaveBankFilename, out bankDataLen);
		uint num = FAudio.FACTAudioEngine_CreateInMemoryWaveBank(audioEngine.handle, bankData, (uint)bankDataLen, 0u, 0u, out handle);
		if (num == 2328297479u)
		{
			FNAPlatform.FreeFilePointer(bankData);
			throw new ArgumentException("XACT could not load the data provided. Make sure you are using the correct version of the XACT tool.");
		}
		engine = audioEngine;
		selfReference = new WeakReference(this, trackResurrection: true);
		engine.RegisterPointer(handle, selfReference);
		IsDisposed = false;
	}

	public WaveBank(AudioEngine audioEngine, string streamingWaveBankFilename, int offset, short packetsize)
	{
		if (audioEngine == null)
		{
			throw new ArgumentNullException("audioEngine", "You must pass in a valid audio engine.");
		}
		if (string.IsNullOrEmpty(streamingWaveBankFilename))
		{
			throw new ArgumentNullException("streamingWaveBankFilename", "This method does not accept null for this parameter.");
		}
		string text = FileHelpers.NormalizeFilePathSeparators(streamingWaveBankFilename);
		if (!Path.IsPathRooted(text))
		{
			text = Path.Combine(TitleLocation.Path, text);
		}
		bankData = FAudio.FAudio_fopen(text);
		FAudio.FACTStreamingParameters pParms = new FAudio.FACTStreamingParameters
		{
			file = bankData
		};
		uint num = FAudio.FACTAudioEngine_CreateStreamingWaveBank(audioEngine.handle, ref pParms, out handle);
		if (num == 2328297479u)
		{
			FAudio.FAudio_close(bankData);
			throw new ArgumentException("XACT could not load the data provided. Make sure you are using the correct version of the XACT tool.");
		}
		engine = audioEngine;
		selfReference = new WeakReference(this, trackResurrection: true);
		engine.RegisterPointer(handle, selfReference);
		IsDisposed = false;
	}

	~WaveBank()
	{
		if (!AudioEngine.ProgramExiting)
		{
			if (!IsDisposed && IsInUse)
			{
				GC.ReRegisterForFinalize(this);
			}
			else
			{
				Dispose(disposing: false);
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		lock (engine.gcSync)
		{
			if (!IsDisposed)
			{
				if (!engine.IsDisposed)
				{
					FAudio.FACTWaveBank_Destroy(handle);
				}
				OnWaveBankDestroyed();
				if (disposing && Disposing != null)
				{
					Disposing(this, EventArgs.Empty);
				}
			}
		}
	}

	internal void OnWaveBankDestroyed()
	{
		IsDisposed = true;
		if (bankData != IntPtr.Zero)
		{
			if (bankDataLen != IntPtr.Zero)
			{
				FNAPlatform.FreeFilePointer(bankData);
				bankDataLen = IntPtr.Zero;
			}
			else
			{
				FAudio.FAudio_close(bankData);
			}
			bankData = IntPtr.Zero;
		}
		handle = IntPtr.Zero;
		selfReference = null;
	}
}
