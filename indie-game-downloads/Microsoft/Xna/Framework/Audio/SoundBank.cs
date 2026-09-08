using System;

namespace Microsoft.Xna.Framework.Audio;

public class SoundBank : IDisposable
{
	internal AudioEngine engine;

	internal FAudio.F3DAUDIO_DSP_SETTINGS dspSettings;

	private nint handle;

	private WeakReference selfReference;

	public bool IsDisposed { get; private set; }

	public bool IsInUse
	{
		get
		{
			FAudio.FACTSoundBank_GetState(handle, out var pdwState);
			return (pdwState & 0x80) != 0;
		}
	}

	public event EventHandler<EventArgs> Disposing;

	public SoundBank(AudioEngine audioEngine, string filename)
	{
		if (audioEngine == null)
		{
			throw new ArgumentNullException("audioEngine", "You must pass in a valid audio engine.");
		}
		if (string.IsNullOrEmpty(filename))
		{
			throw new ArgumentNullException("filename", "This method does not accept null for this parameter.");
		}
		nint num = TitleContainer.ReadToPointer(filename, out var size);
		uint num2 = FAudio.FACTAudioEngine_CreateSoundBank(audioEngine.handle, num, (uint)size, 0u, 0u, out handle);
		FNAPlatform.FreeFilePointer(num);
		if (num2 == 2328297479u)
		{
			throw new ArgumentException("XACT could not load the data provided. Make sure you are using the correct version of the XACT tool.");
		}
		engine = audioEngine;
		selfReference = new WeakReference(this, trackResurrection: true);
		dspSettings = default(FAudio.F3DAUDIO_DSP_SETTINGS);
		dspSettings.SrcChannelCount = 1u;
		dspSettings.DstChannelCount = engine.channels;
		dspSettings.pMatrixCoefficients = FNAPlatform.Malloc((int)(4 * dspSettings.SrcChannelCount * dspSettings.DstChannelCount));
		engine.RegisterPointer(handle, selfReference);
		IsDisposed = false;
	}

	~SoundBank()
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

	protected void Dispose(bool disposing)
	{
		lock (engine.gcSync)
		{
			if (!IsDisposed)
			{
				if (!engine.IsDisposed)
				{
					FAudio.FACTSoundBank_Destroy(handle);
				}
				OnSoundBankDestroyed();
				if (disposing && Disposing != null)
				{
					Disposing(this, EventArgs.Empty);
				}
			}
		}
	}

	public Cue GetCue(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name", "This method does not accept null for this parameter.");
		}
		ushort num = FAudio.FACTSoundBank_GetCueIndex(handle, name);
		if (num == ushort.MaxValue)
		{
			throw new ArgumentException("An error occurred trying to play the cue named \"" + name + "\". Is the cue name correct?");
		}
		if (FAudio.FACTSoundBank_Prepare(handle, num, 0u, 0, out var ppCue) == 2328297491u)
		{
			throw new InvalidOperationException("No wavebank exists for the requested operation.");
		}
		return new Cue(ppCue, name, this);
	}

	public void PlayCue(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name", "This method does not accept null for this parameter.");
		}
		ushort num = FAudio.FACTSoundBank_GetCueIndex(handle, name);
		if (num == ushort.MaxValue)
		{
			throw new InvalidOperationException("An error occurred trying to play the cue named \"" + name + "\". Is the cue name correct?");
		}
		FAudio.FACTSoundBank_Play(handle, num, 0u, 0, IntPtr.Zero);
	}

	public void PlayCue(string name, AudioListener listener, AudioEmitter emitter)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name", "This method does not accept null for this parameter.");
		}
		if (listener == null)
		{
			throw new ArgumentNullException("listener");
		}
		if (emitter == null)
		{
			throw new ArgumentNullException("emitter");
		}
		ushort num = FAudio.FACTSoundBank_GetCueIndex(handle, name);
		if (num == ushort.MaxValue)
		{
			throw new InvalidOperationException("An error occurred trying to play the cue named \"" + name + "\". Is the cue name correct?");
		}
		emitter.emitterData.ChannelCount = dspSettings.SrcChannelCount;
		emitter.emitterData.CurveDistanceScaler = float.MaxValue;
		FAudio.FACT3DCalculate(engine.handle3D, ref listener.listenerData, ref emitter.emitterData, ref dspSettings);
		FAudio.FACTSoundBank_Play3D(handle, num, 0u, 0, ref dspSettings, IntPtr.Zero);
	}

	internal void OnSoundBankDestroyed()
	{
		IsDisposed = true;
		handle = IntPtr.Zero;
		selfReference = null;
		if (dspSettings.pMatrixCoefficients != IntPtr.Zero)
		{
			FNAPlatform.Free(dspSettings.pMatrixCoefficients);
			dspSettings.pMatrixCoefficients = IntPtr.Zero;
		}
	}
}
