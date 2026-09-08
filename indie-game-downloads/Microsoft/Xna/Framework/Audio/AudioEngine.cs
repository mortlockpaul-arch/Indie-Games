using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace Microsoft.Xna.Framework.Audio;

public class AudioEngine : IDisposable
{
	private class IntPtrComparer : IEqualityComparer<nint>
	{
		public bool Equals(nint x, nint y)
		{
			return x == y;
		}

		public int GetHashCode(nint obj)
		{
			return ((IntPtr)obj).GetHashCode();
		}
	}

	public const int ContentVersion = 46;

	internal readonly nint handle;

	internal readonly byte[] handle3D;

	internal readonly ushort channels;

	internal readonly object gcSync = new object();

	private RendererDetail[] rendererDetails;

	private readonly FAudio.FACTNotificationCallback xactNotificationFunc;

	private FAudio.FACTNotificationDescription notificationDesc;

	private static readonly IntPtrComparer comparer;

	private static readonly Dictionary<nint, WeakReference> xactPtrs;

	internal static bool ProgramExiting;

	public ReadOnlyCollection<RendererDetail> RendererDetails => new ReadOnlyCollection<RendererDetail>(rendererDetails);

	public bool IsDisposed { get; private set; }

	public event EventHandler<EventArgs> Disposing;

	public AudioEngine(string settingsFile)
		: this(settingsFile, new TimeSpan(0, 0, 0, 0, 250), null)
	{
	}

	public unsafe AudioEngine(string settingsFile, TimeSpan lookAheadTime, string rendererId)
	{
		if (string.IsNullOrEmpty(settingsFile))
		{
			throw new ArgumentNullException("settingsFile", "This method does not accept null for this parameter.");
		}
		FAudio.FACTCreateEngine(0u, out handle);
		FAudio.FACTAudioEngine_GetRendererCount(handle, out var pnRendererCount);
		if (pnRendererCount == 0)
		{
			FAudio.FACTAudioEngine_Release(handle);
			throw new NoAudioHardwareException();
		}
		rendererDetails = new RendererDetail[pnRendererCount];
		for (ushort num = 0; num < pnRendererCount; num++)
		{
			FAudio.FACTAudioEngine_GetRendererDetails(handle, num, out var pRendererDetails);
			rendererDetails[num].FriendlyName = new string((char*)pRendererDetails.displayName);
			rendererDetails[num].RendererId = new string((char*)pRendererDetails.rendererID);
		}
		nint pGlobalSettingsBuffer = TitleContainer.ReadToPointer(settingsFile, out var size);
		FAudio.FACTRuntimeParameters pParams = new FAudio.FACTRuntimeParameters
		{
			pGlobalSettingsBuffer = pGlobalSettingsBuffer,
			globalSettingsBufferSize = (uint)size,
			globalSettingsFlags = 1u
		};
		xactNotificationFunc = OnXACTNotification;
		pParams.fnNotificationCallback = Marshal.GetFunctionPointerForDelegate(xactNotificationFunc);
		pParams.lookAheadTime = (uint)lookAheadTime.Milliseconds;
		if (!string.IsNullOrEmpty(rendererId))
		{
			pParams.pRendererID = Marshal.StringToHGlobalUni(rendererId);
		}
		uint num2 = FAudio.FACTAudioEngine_Initialize(handle, ref pParams);
		if (pParams.pRendererID != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(pParams.pRendererID);
		}
		switch (num2)
		{
		case 2328297479u:
			FAudio.FACTAudioEngine_Release(handle);
			throw new ArgumentException("XACT could not load the data provided. Make sure you are using the correct version of the XACT tool.");
		default:
			FAudio.FACTAudioEngine_Release(handle);
			throw new InvalidOperationException("Engine initialization failed!");
		case 0u:
		{
			handle3D = new byte[20];
			FAudio.FACT3DInitialize(handle, handle3D);
			FAudio.FACTAudioEngine_GetFinalMixFormat(handle, out var pFinalMixFormat);
			channels = pFinalMixFormat.Format.nChannels;
			notificationDesc = default(FAudio.FACTNotificationDescription);
			notificationDesc.flags = 1;
			notificationDesc.type = 7;
			FAudio.FACTAudioEngine_RegisterNotification(handle, ref notificationDesc);
			notificationDesc.type = 6;
			FAudio.FACTAudioEngine_RegisterNotification(handle, ref notificationDesc);
			notificationDesc.type = 4;
			FAudio.FACTAudioEngine_RegisterNotification(handle, ref notificationDesc);
			break;
		}
		}
	}

	static AudioEngine()
	{
		comparer = new IntPtrComparer();
		xactPtrs = new Dictionary<nint, WeakReference>(comparer);
		ProgramExiting = false;
		AppDomain.CurrentDomain.ProcessExit += ProgramExit;
	}

	~AudioEngine()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public AudioCategory GetCategory(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name", "This method does not accept null for this parameter.");
		}
		AudioCategory result = default(AudioCategory);
		result.index = FAudio.FACTAudioEngine_GetCategory(handle, name);
		if (result.index == ushort.MaxValue)
		{
			throw new InvalidOperationException("This resource could not be created.");
		}
		result.parent = this;
		result.name = name;
		return result;
	}

	public float GetGlobalVariable(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name", "This method does not accept null for this parameter.");
		}
		ushort num = FAudio.FACTAudioEngine_GetGlobalVariableIndex(handle, name);
		if (num == ushort.MaxValue)
		{
			throw new IndexOutOfRangeException("The specified variable index is invalid.");
		}
		FAudio.FACTAudioEngine_GetGlobalVariable(handle, num, out var pnValue);
		return pnValue;
	}

	public void SetGlobalVariable(string name, float value)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name", "This method does not accept null for this parameter.");
		}
		ushort num = FAudio.FACTAudioEngine_GetGlobalVariableIndex(handle, name);
		if (num == ushort.MaxValue)
		{
			throw new IndexOutOfRangeException("The specified variable index is invalid.");
		}
		FAudio.FACTAudioEngine_SetGlobalVariable(handle, num, value);
	}

	public void Update()
	{
		FAudio.FACTAudioEngine_DoWork(handle);
	}

	protected virtual void Dispose(bool disposing)
	{
		lock (gcSync)
		{
			if (!IsDisposed)
			{
				FAudio.FACTAudioEngine_ShutDown(handle);
				FAudio.FACTAudioEngine_Release(handle);
				rendererDetails = null;
				IsDisposed = true;
				if (disposing && Disposing != null)
				{
					Disposing(this, EventArgs.Empty);
				}
			}
		}
	}

	internal void RegisterPointer(nint ptr, WeakReference reference)
	{
		lock (xactPtrs)
		{
			xactPtrs[ptr] = reference;
		}
	}

	[MonoPInvokeCallback(typeof(FAudio.FACTNotificationCallback))]
	private unsafe static void OnXACTNotification(nint notification)
	{
		WeakReference value;
		if (((FAudio.FACTNotification*)notification)->type == 7)
		{
			nint pWaveBank = ((FAudio.FACTNotification*)notification)->anon.waveBank.pWaveBank;
			lock (xactPtrs)
			{
				if (xactPtrs.TryGetValue(pWaveBank, out value) && value.IsAlive)
				{
					(value.Target as WaveBank).OnWaveBankDestroyed();
				}
				xactPtrs.Remove(pWaveBank);
				return;
			}
		}
		if (((FAudio.FACTNotification*)notification)->type == 6)
		{
			nint pSoundBank = ((FAudio.FACTNotification*)notification)->anon.soundBank.pSoundBank;
			lock (xactPtrs)
			{
				if (xactPtrs.TryGetValue(pSoundBank, out value) && value.IsAlive)
				{
					(value.Target as SoundBank).OnSoundBankDestroyed();
				}
				xactPtrs.Remove(pSoundBank);
				return;
			}
		}
		if (((FAudio.FACTNotification*)notification)->type != 4)
		{
			return;
		}
		nint pCue = ((FAudio.FACTNotification*)notification)->anon.cue.pCue;
		lock (xactPtrs)
		{
			if (xactPtrs.TryGetValue(pCue, out value) && value.IsAlive)
			{
				(value.Target as Cue).OnCueDestroyed();
			}
			xactPtrs.Remove(pCue);
		}
	}

	internal static void ProgramExit(object sender, EventArgs e)
	{
		ProgramExiting = true;
	}
}
