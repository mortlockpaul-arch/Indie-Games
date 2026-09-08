using System;

namespace Microsoft.Xna.Framework.Audio;

public sealed class Cue : IDisposable
{
	private nint handle;

	private SoundBank bank;

	private WeakReference selfReference;

	private bool applied3D;

	private bool played;

	public bool IsCreated
	{
		get
		{
			FAudio.FACTCue_GetState(handle, out var pdwState);
			return (pdwState & 1) != 0;
		}
	}

	public bool IsDisposed { get; private set; }

	public bool IsPaused
	{
		get
		{
			FAudio.FACTCue_GetState(handle, out var pdwState);
			return (pdwState & 0x40) != 0;
		}
	}

	public bool IsPlaying
	{
		get
		{
			FAudio.FACTCue_GetState(handle, out var pdwState);
			return (pdwState & 8) != 0;
		}
	}

	public bool IsPrepared
	{
		get
		{
			FAudio.FACTCue_GetState(handle, out var pdwState);
			return (pdwState & 4) != 0;
		}
	}

	public bool IsPreparing
	{
		get
		{
			FAudio.FACTCue_GetState(handle, out var pdwState);
			return (pdwState & 2) != 0;
		}
	}

	public bool IsStopped
	{
		get
		{
			FAudio.FACTCue_GetState(handle, out var pdwState);
			return (pdwState & 0x20) != 0;
		}
	}

	public bool IsStopping
	{
		get
		{
			FAudio.FACTCue_GetState(handle, out var pdwState);
			return (pdwState & 0x10) != 0;
		}
	}

	public string Name { get; private set; }

	public event EventHandler<EventArgs> Disposing;

	internal Cue(nint cue, string name, SoundBank soundBank)
	{
		handle = cue;
		Name = name;
		bank = soundBank;
		selfReference = new WeakReference(this, trackResurrection: true);
		bank.engine.RegisterPointer(handle, selfReference);
	}

	~Cue()
	{
		if (!AudioEngine.ProgramExiting)
		{
			if (!IsDisposed && IsPlaying)
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

	public void Apply3D(AudioListener listener, AudioEmitter emitter)
	{
		if (listener == null)
		{
			throw new ArgumentNullException("listener");
		}
		if (emitter == null)
		{
			throw new ArgumentNullException("emitter");
		}
		if (!applied3D && played)
		{
			throw new InvalidOperationException("You must call Apply3D on a Cue before calling Play to be able to call Apply3D after calling Play.");
		}
		emitter.emitterData.ChannelCount = bank.dspSettings.SrcChannelCount;
		emitter.emitterData.CurveDistanceScaler = float.MaxValue;
		FAudio.FACT3DCalculate(bank.engine.handle3D, ref listener.listenerData, ref emitter.emitterData, ref bank.dspSettings);
		FAudio.FACT3DApply(ref bank.dspSettings, handle);
		applied3D = true;
	}

	public float GetVariable(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name", "This method does not accept null for this parameter.");
		}
		ushort num = FAudio.FACTCue_GetVariableIndex(handle, name);
		if (num == ushort.MaxValue)
		{
			throw new IndexOutOfRangeException("The specified variable index is invalid.");
		}
		FAudio.FACTCue_GetVariable(handle, num, out var nValue);
		return nValue;
	}

	public void Pause()
	{
		if (IsStopped)
		{
			throw new InvalidOperationException("The method or function that was called cannot be used in the manner requested.");
		}
		FAudio.FACTCue_Pause(handle, 1);
	}

	public void Play()
	{
		FAudio.FACTCue_Play(handle);
		played = true;
	}

	public void Resume()
	{
		if (IsStopped)
		{
			throw new InvalidOperationException("The method or function that was called cannot be used in the manner requested.");
		}
		FAudio.FACTCue_Pause(handle, 0);
	}

	public void SetVariable(string name, float value)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name", "This method does not accept null for this parameter.");
		}
		ushort num = FAudio.FACTCue_GetVariableIndex(handle, name);
		if (num == ushort.MaxValue)
		{
			throw new IndexOutOfRangeException("The specified variable index is invalid.");
		}
		FAudio.FACTCue_SetVariable(handle, num, value);
	}

	public void Stop(AudioStopOptions options)
	{
		if ((uint)options > 1u)
		{
			throw new ArgumentException();
		}
		FAudio.FACTCue_Stop(handle, (uint)options);
	}

	internal void OnCueDestroyed()
	{
		IsDisposed = true;
		handle = IntPtr.Zero;
		selfReference = null;
	}

	private void Dispose(bool disposing)
	{
		lock (bank.engine.gcSync)
		{
			if (!IsDisposed)
			{
				if (!bank.engine.IsDisposed)
				{
					FAudio.FACTCue_Destroy(handle);
				}
				OnCueDestroyed();
				if (disposing && Disposing != null)
				{
					Disposing(this, EventArgs.Empty);
				}
			}
		}
	}
}
