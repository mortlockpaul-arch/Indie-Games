using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Diagnostics.Tracing;

internal static class EventPipeInternal
{
	private struct EventPipeProviderConfigurationNative
	{
		private unsafe char* m_pProviderName;

		private ulong m_keywords;

		private uint m_loggingLevel;

		private unsafe char* m_pFilterData;

		internal unsafe static void MarshalToNative(EventPipeProviderConfiguration managed, ref EventPipeProviderConfigurationNative native)
		{
			native.m_pProviderName = (char*)Marshal.StringToCoTaskMemUni(managed.ProviderName);
			native.m_keywords = managed.Keywords;
			native.m_loggingLevel = managed.LoggingLevel;
			native.m_pFilterData = (char*)Marshal.StringToCoTaskMemUni(managed.FilterData);
		}

		internal unsafe void Release()
		{
			if (m_pProviderName != null)
			{
				Marshal.FreeCoTaskMem((nint)m_pProviderName);
			}
			if (m_pFilterData != null)
			{
				Marshal.FreeCoTaskMem((nint)m_pFilterData);
			}
		}
	}

	internal unsafe static ulong Enable(string outputFile, EventPipeSerializationFormat format, uint circularBufferSizeInMB, EventPipeProviderConfiguration[] providers)
	{
		Span<EventPipeProviderConfigurationNative> span = new Span<EventPipeProviderConfigurationNative>((void*)Marshal.AllocCoTaskMem(sizeof(EventPipeProviderConfigurationNative) * providers.Length), providers.Length);
		span.Clear();
		try
		{
			for (int i = 0; i < providers.Length; i++)
			{
				EventPipeProviderConfigurationNative.MarshalToNative(providers[i], ref span[i]);
			}
			fixed (char* outputFile2 = outputFile)
			{
				fixed (EventPipeProviderConfigurationNative* providers2 = span)
				{
					return Enable(outputFile2, format, circularBufferSizeInMB, providers2, (uint)span.Length);
				}
			}
		}
		finally
		{
			for (int j = 0; j < providers.Length; j++)
			{
				span[j].Release();
			}
			fixed (EventPipeProviderConfigurationNative* ptr = span)
			{
				Marshal.FreeCoTaskMem((nint)ptr);
			}
		}
	}

	[DllImport("QCall", EntryPoint = "EventPipeInternal_Enable", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "EventPipeInternal_Enable")]
	private unsafe static extern ulong Enable(char* outputFile, EventPipeSerializationFormat format, uint circularBufferSizeInMB, EventPipeProviderConfigurationNative* providers, uint numProviders);

	[DllImport("QCall", EntryPoint = "EventPipeInternal_Disable", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "EventPipeInternal_Disable")]
	internal static extern void Disable(ulong sessionID);

	[LibraryImport("QCall", EntryPoint = "EventPipeInternal_CreateProvider", StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	internal unsafe static nint CreateProvider(string providerName, delegate* unmanaged<byte*, int, byte, long, long, Interop.Advapi32.EVENT_FILTER_DESCRIPTOR*, void*, void> callbackFunc, void* callbackContext)
	{
		nint result;
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(providerName))
		{
			void* _providerName_native = ptr;
			result = __PInvoke((ushort*)_providerName_native, callbackFunc, callbackContext);
		}
		return result;
		[DllImport("QCall", EntryPoint = "EventPipeInternal_CreateProvider", ExactSpelling = true)]
		unsafe static extern nint __PInvoke(ushort* __providerName_native, delegate* unmanaged<byte*, int, byte, long, long, Interop.Advapi32.EVENT_FILTER_DESCRIPTOR*, void*, void> __callbackFunc_native, void* __callbackContext_native);
	}

	[DllImport("QCall", EntryPoint = "EventPipeInternal_DefineEvent", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "EventPipeInternal_DefineEvent")]
	internal unsafe static extern nint DefineEvent(nint provHandle, uint eventID, long keywords, uint eventVersion, uint level, void* pMetadata, uint metadataLength);

	[LibraryImport("QCall", EntryPoint = "EventPipeInternal_GetProvider", StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	internal unsafe static nint GetProvider(string providerName)
	{
		nint result;
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(providerName))
		{
			void* _providerName_native = ptr;
			result = __PInvoke((ushort*)_providerName_native);
		}
		return result;
		[DllImport("QCall", EntryPoint = "EventPipeInternal_GetProvider", ExactSpelling = true)]
		unsafe static extern nint __PInvoke(ushort* __providerName_native);
	}

	[DllImport("QCall", EntryPoint = "EventPipeInternal_DeleteProvider", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "EventPipeInternal_DeleteProvider")]
	internal static extern void DeleteProvider(nint provHandle);

	[LibraryImport("QCall", EntryPoint = "EventPipeInternal_EventActivityIdControl")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	internal unsafe static int EventActivityIdControl(uint controlCode, ref Guid activityId)
	{
		int result;
		fixed (Guid* _activityId_native = &activityId)
		{
			result = __PInvoke(controlCode, _activityId_native);
		}
		return result;
		[DllImport("QCall", EntryPoint = "EventPipeInternal_EventActivityIdControl", ExactSpelling = true)]
		unsafe static extern int __PInvoke(uint __controlCode_native, Guid* __activityId_native);
	}

	[DllImport("QCall", EntryPoint = "EventPipeInternal_WriteEventData", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "EventPipeInternal_WriteEventData")]
	internal unsafe static extern void WriteEventData(nint eventHandle, EventProvider.EventData* pEventData, uint dataCount, Guid* activityId, Guid* relatedActivityId);

	[LibraryImport("QCall", EntryPoint = "EventPipeInternal_GetSessionInfo")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool GetSessionInfo(ulong sessionID, EventPipeSessionInfo* pSessionInfo)
	{
		return __PInvoke(sessionID, pSessionInfo) != 0;
		[DllImport("QCall", EntryPoint = "EventPipeInternal_GetSessionInfo", ExactSpelling = true)]
		unsafe static extern int __PInvoke(ulong __sessionID_native, EventPipeSessionInfo* __pSessionInfo_native);
	}

	[LibraryImport("QCall", EntryPoint = "EventPipeInternal_GetNextEvent")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool GetNextEvent(ulong sessionID, EventPipeEventInstanceData* pInstance)
	{
		return __PInvoke(sessionID, pInstance) != 0;
		[DllImport("QCall", EntryPoint = "EventPipeInternal_GetNextEvent", ExactSpelling = true)]
		unsafe static extern int __PInvoke(ulong __sessionID_native, EventPipeEventInstanceData* __pInstance_native);
	}

	[LibraryImport("QCall", EntryPoint = "EventPipeInternal_SignalSession")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool SignalSession(ulong sessionID)
	{
		return __PInvoke(sessionID) != 0;
		[DllImport("QCall", EntryPoint = "EventPipeInternal_SignalSession", ExactSpelling = true)]
		static extern int __PInvoke(ulong __sessionID_native);
	}

	[LibraryImport("QCall", EntryPoint = "EventPipeInternal_WaitForSessionSignal")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool WaitForSessionSignal(ulong sessionID, int timeoutMs)
	{
		return __PInvoke(sessionID, timeoutMs) != 0;
		[DllImport("QCall", EntryPoint = "EventPipeInternal_WaitForSessionSignal", ExactSpelling = true)]
		static extern int __PInvoke(ulong __sessionID_native, int __timeoutMs_native);
	}
}
