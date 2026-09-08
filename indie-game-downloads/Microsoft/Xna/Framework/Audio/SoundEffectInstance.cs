using System;

namespace Microsoft.Xna.Framework.Audio;

public class SoundEffectInstance : IDisposable
{
	private bool INTERNAL_looped = false;

	private float INTERNAL_pan = 0f;

	private float INTERNAL_pitch = 0f;

	private SoundState INTERNAL_state = SoundState.Stopped;

	private float INTERNAL_volume = 1f;

	internal nint handle;

	private SoundEffect parentEffect;

	private WeakReference selfReference;

	private bool isDynamic;

	private bool hasStarted;

	private bool is3D;

	private bool usingReverb;

	private FAudio.F3DAUDIO_DSP_SETTINGS dspSettings;

	private static readonly float maxFreqRatio = ((Environment.GetEnvironmentVariable("FNA_SOUNDEFFECT_UNCAPPED_PITCH") == "1") ? 1024f : 2f);

	public bool IsDisposed { get; protected set; }

	public virtual bool IsLooped
	{
		get
		{
			return INTERNAL_looped;
		}
		set
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
			}
			if (hasStarted)
			{
				throw new InvalidOperationException("Loop must be set before the first Play call.");
			}
			INTERNAL_looped = value;
		}
	}

	public float Pan
	{
		get
		{
			return INTERNAL_pan;
		}
		set
		{
			if (!(value >= -1f) || !(value <= 1f))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
			}
			if (hasStarted && is3D)
			{
				throw new InvalidOperationException("Pan cannot be set on a 3D sound. To ensure a 2D sound avoid calling Apply3D and ensure Pan is set before the first Play call.");
			}
			is3D = false;
			INTERNAL_pan = value;
			SetPanMatrixCoefficients();
			if (handle != IntPtr.Zero)
			{
				FAudio.FAudioVoice_SetOutputMatrix(handle, SoundEffect.Device().MasterVoice, dspSettings.SrcChannelCount, dspSettings.DstChannelCount, dspSettings.pMatrixCoefficients, 0u);
			}
		}
	}

	public float Pitch
	{
		get
		{
			return INTERNAL_pitch;
		}
		set
		{
			if (!(value >= -1f) || !(value <= 1f))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
			}
			INTERNAL_pitch = value;
			if (handle != IntPtr.Zero)
			{
				UpdatePitch();
			}
		}
	}

	public SoundState State
	{
		get
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
			}
			if (!isDynamic && handle != IntPtr.Zero && INTERNAL_state == SoundState.Playing)
			{
				FAudio.FAudioSourceVoice_GetState(handle, out var pVoiceState, 0u);
				if (pVoiceState.BuffersQueued == 0 && pVoiceState.SamplesPlayed == 0)
				{
					Stop(immediate: true);
				}
			}
			return INTERNAL_state;
		}
	}

	public float Volume
	{
		get
		{
			return INTERNAL_volume;
		}
		set
		{
			if (!(value >= -16777216f) || !(value <= 16777216f))
			{
				throw new ArgumentOutOfRangeException("value");
			}
			if (IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
			}
			INTERNAL_volume = value;
			if (handle != IntPtr.Zero)
			{
				FAudio.FAudioVoice_SetVolume(handle, INTERNAL_volume, 0u);
			}
		}
	}

	internal SoundEffectInstance(SoundEffect parent)
	{
		SoundEffect.Device();
		selfReference = new WeakReference(this, trackResurrection: true);
		parentEffect = parent;
		hasStarted = false;
		is3D = false;
		usingReverb = false;
		INTERNAL_state = SoundState.Stopped;
		if (parentEffect != null)
		{
			InitDSPSettings(parentEffect.channels);
			parentEffect.Instances.Add(selfReference);
		}
		else
		{
			isDynamic = true;
		}
	}

	~SoundEffectInstance()
	{
		if (!SoundEffect.FAudioContext.ProgramExiting && (!IsDisposed && State == SoundState.Playing))
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
		if (IsDisposed)
		{
			throw new ObjectDisposedException("SoundEffectInstance");
		}
		if (hasStarted && !is3D)
		{
			throw new InvalidOperationException("The sound is not a 3D sound. Call Apply3D before the first Play call to configure it to be a 3D sound.");
		}
		is3D = true;
		SoundEffect.FAudioContext fAudioContext = SoundEffect.Device();
		emitter.emitterData.CurveDistanceScaler = fAudioContext.CurveDistanceScaler;
		emitter.emitterData.ChannelCount = dspSettings.SrcChannelCount;
		FAudio.F3DAudioCalculate(fAudioContext.Handle3D, ref listener.listenerData, ref emitter.emitterData, 33u, ref dspSettings);
		if (handle != IntPtr.Zero)
		{
			UpdatePitch();
			FAudio.FAudioVoice_SetOutputMatrix(handle, SoundEffect.Device().MasterVoice, dspSettings.SrcChannelCount, dspSettings.DstChannelCount, dspSettings.pMatrixCoefficients, 0u);
		}
	}

	public void Apply3D(AudioListener[] listeners, AudioEmitter emitter)
	{
		if (listeners == null)
		{
			throw new ArgumentNullException("listeners");
		}
		if (listeners.Length == 1)
		{
			Apply3D(listeners[0], emitter);
			return;
		}
		throw new NotSupportedException("Only one listener is supported.");
	}

	public virtual void Play()
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
		}
		if (State == SoundState.Playing)
		{
			return;
		}
		if (State == SoundState.Paused)
		{
			FAudio.FAudioSourceVoice_Start(handle, 0u, 0u);
			INTERNAL_state = SoundState.Playing;
			return;
		}
		SoundEffect.FAudioContext fAudioContext = SoundEffect.Device();
		if (isDynamic)
		{
			FAudio.FAudio_CreateSourceVoice(fAudioContext.Handle, out handle, ref (this as DynamicSoundEffectInstance).format, 8u, maxFreqRatio, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		}
		else
		{
			FAudio.FAudio_CreateSourceVoice(fAudioContext.Handle, out handle, parentEffect.formatPtr, 8u, maxFreqRatio, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		}
		if (handle == IntPtr.Zero)
		{
			return;
		}
		FAudio.FAudioVoice_SetVolume(handle, INTERNAL_volume, 0u);
		UpdatePitch();
		if (is3D || Pan != 0f)
		{
			FAudio.FAudioVoice_SetOutputMatrix(handle, SoundEffect.Device().MasterVoice, dspSettings.SrcChannelCount, dspSettings.DstChannelCount, dspSettings.pMatrixCoefficients, 0u);
		}
		if (isDynamic)
		{
			(this as DynamicSoundEffectInstance).QueueInitialBuffers();
		}
		else
		{
			if (IsLooped)
			{
				parentEffect.handle.LoopCount = 255u;
				parentEffect.handle.LoopBegin = parentEffect.loopStart;
				parentEffect.handle.LoopLength = parentEffect.loopLength;
			}
			else
			{
				parentEffect.handle.LoopCount = 0u;
				parentEffect.handle.LoopBegin = 0u;
				parentEffect.handle.LoopLength = 0u;
			}
			FAudio.FAudioSourceVoice_SubmitSourceBuffer(handle, ref parentEffect.handle, IntPtr.Zero);
		}
		FAudio.FAudioSourceVoice_Start(handle, 0u, 0u);
		INTERNAL_state = SoundState.Playing;
		hasStarted = true;
	}

	public void Pause()
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
		}
		if (handle != IntPtr.Zero && State == SoundState.Playing)
		{
			FAudio.FAudioSourceVoice_Stop(handle, 0u, 0u);
			INTERNAL_state = SoundState.Paused;
		}
	}

	public void Resume()
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
		}
		SoundState state = State;
		if (handle == IntPtr.Zero)
		{
			Play();
		}
		else if (state == SoundState.Paused)
		{
			FAudio.FAudioSourceVoice_Start(handle, 0u, 0u);
			INTERNAL_state = SoundState.Playing;
		}
	}

	public void Stop()
	{
		Stop(immediate: true);
	}

	public void Stop(bool immediate)
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException(GetType().Name, "This object has already been disposed.");
		}
		if (handle == IntPtr.Zero)
		{
			return;
		}
		if (immediate)
		{
			FAudio.FAudioSourceVoice_Stop(handle, 0u, 0u);
			FAudio.FAudioSourceVoice_FlushSourceBuffers(handle);
			FAudio.FAudioVoice_DestroyVoice(handle);
			handle = IntPtr.Zero;
			usingReverb = false;
			INTERNAL_state = SoundState.Stopped;
			if (isDynamic)
			{
				lock (FrameworkDispatcher.Streams)
				{
					FrameworkDispatcher.Streams.Remove(this as DynamicSoundEffectInstance);
				}
				(this as DynamicSoundEffectInstance).ClearBuffers();
			}
		}
		else
		{
			if (isDynamic)
			{
				throw new InvalidOperationException();
			}
			FAudio.FAudioSourceVoice_ExitLoop(handle, 0u);
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			Stop(immediate: true);
			if (parentEffect != null)
			{
				parentEffect.Instances.Remove(selfReference);
			}
			selfReference = null;
			FNAPlatform.Free(dspSettings.pMatrixCoefficients);
			IsDisposed = true;
		}
	}

	internal unsafe void InitDSPSettings(uint srcChannels)
	{
		dspSettings = default(FAudio.F3DAUDIO_DSP_SETTINGS);
		dspSettings.DopplerFactor = 1f;
		dspSettings.SrcChannelCount = srcChannels;
		dspSettings.DstChannelCount = SoundEffect.Device().DeviceDetails.OutputFormat.Format.nChannels;
		int num = (int)(4 * dspSettings.SrcChannelCount * dspSettings.DstChannelCount);
		dspSettings.pMatrixCoefficients = FNAPlatform.Malloc(num);
		byte* pMatrixCoefficients = (byte*)dspSettings.pMatrixCoefficients;
		for (int i = 0; i < num; i++)
		{
			pMatrixCoefficients[i] = 0;
		}
		SetPanMatrixCoefficients();
	}

	internal unsafe void INTERNAL_applyReverb(float rvGain)
	{
		if (handle != IntPtr.Zero)
		{
			if (!usingReverb)
			{
				SoundEffect.Device().AttachReverb(handle);
				usingReverb = true;
			}
			float* pMatrixCoefficients = (float*)dspSettings.pMatrixCoefficients;
			*pMatrixCoefficients = rvGain;
			if (dspSettings.SrcChannelCount == 2)
			{
				pMatrixCoefficients[1] = rvGain;
			}
			FAudio.FAudioVoice_SetOutputMatrix(handle, SoundEffect.Device().ReverbVoice, dspSettings.SrcChannelCount, 1u, dspSettings.pMatrixCoefficients, 0u);
		}
	}

	internal void INTERNAL_applyLowPassFilter(float cutoff)
	{
		if (handle != IntPtr.Zero)
		{
			FAudio.FAudioFilterParameters pParameters = new FAudio.FAudioFilterParameters
			{
				Type = FAudio.FAudioFilterType.FAudioLowPassFilter,
				Frequency = cutoff,
				OneOverQ = 1f
			};
			FAudio.FAudioVoice_SetFilterParameters(handle, ref pParameters, 0u);
		}
	}

	internal void INTERNAL_applyHighPassFilter(float cutoff)
	{
		if (handle != IntPtr.Zero)
		{
			FAudio.FAudioFilterParameters pParameters = new FAudio.FAudioFilterParameters
			{
				Type = FAudio.FAudioFilterType.FAudioHighPassFilter,
				Frequency = cutoff,
				OneOverQ = 1f
			};
			FAudio.FAudioVoice_SetFilterParameters(handle, ref pParameters, 0u);
		}
	}

	internal void INTERNAL_applyBandPassFilter(float center)
	{
		if (handle != IntPtr.Zero)
		{
			FAudio.FAudioFilterParameters pParameters = new FAudio.FAudioFilterParameters
			{
				Type = FAudio.FAudioFilterType.FAudioBandPassFilter,
				Frequency = center,
				OneOverQ = 1f
			};
			FAudio.FAudioVoice_SetFilterParameters(handle, ref pParameters, 0u);
		}
	}

	private void UpdatePitch()
	{
		float dopplerScale = SoundEffect.Device().DopplerScale;
		float num = ((is3D && dopplerScale != 0f) ? (dspSettings.DopplerFactor * dopplerScale) : 1f);
		FAudio.FAudioSourceVoice_SetFrequencyRatio(handle, (float)Math.Pow(2.0, INTERNAL_pitch) * num, 0u);
	}

	private unsafe void SetPanMatrixCoefficients()
	{
		float* pMatrixCoefficients = (float*)dspSettings.pMatrixCoefficients;
		if (dspSettings.SrcChannelCount == 1)
		{
			if (dspSettings.DstChannelCount == 1)
			{
				*pMatrixCoefficients = 1f;
				return;
			}
			*pMatrixCoefficients = ((INTERNAL_pan > 0f) ? (1f - INTERNAL_pan) : 1f);
			pMatrixCoefficients[1] = ((INTERNAL_pan < 0f) ? (1f + INTERNAL_pan) : 1f);
		}
		else if (dspSettings.DstChannelCount == 1)
		{
			*pMatrixCoefficients = 1f;
			pMatrixCoefficients[1] = 1f;
		}
		else if (INTERNAL_pan <= 0f)
		{
			*pMatrixCoefficients = 0.5f * INTERNAL_pan + 1f;
			pMatrixCoefficients[1] = 0.5f * (0f - INTERNAL_pan);
			pMatrixCoefficients[2] = 0f;
			pMatrixCoefficients[3] = INTERNAL_pan + 1f;
		}
		else
		{
			*pMatrixCoefficients = 0f - INTERNAL_pan + 1f;
			pMatrixCoefficients[1] = 0f;
			pMatrixCoefficients[2] = 0.5f * INTERNAL_pan;
			pMatrixCoefficients[3] = 0.5f * (0f - INTERNAL_pan) + 1f;
		}
	}
}
