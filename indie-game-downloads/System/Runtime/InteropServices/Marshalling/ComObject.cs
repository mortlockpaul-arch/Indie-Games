using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Runtime.InteropServices.Marshalling;

public sealed class ComObject : IDynamicInterfaceCastable, IUnmanagedVirtualMethodTableProvider, System.Runtime.InteropServices.Marshalling.ComImportInteropInterfaceDetailsStrategy.IComImportAdapter
{
	private unsafe readonly void* _instancePointer;

	private readonly object _runtimeCallableWrapper;

	private volatile bool _released;

	[FeatureSwitchDefinition("System.Runtime.InteropServices.BuiltInComInterop.IsSupported")]
	internal static bool BuiltInComSupported { get; } = !AppContext.TryGetSwitch("System.Runtime.InteropServices.BuiltInComInterop.IsSupported", out var isEnabled) || isEnabled;

	[FeatureSwitchDefinition("System.Runtime.InteropServices.Marshalling.EnableGeneratedComInterfaceComImportInterop")]
	internal static bool ComImportInteropEnabled { get; } = AppContext.TryGetSwitch("System.Runtime.InteropServices.Marshalling.EnableGeneratedComInterfaceComImportInterop", out var isEnabled2) && isEnabled2;

	private IIUnknownInterfaceDetailsStrategy InterfaceDetailsStrategy { get; }

	private IIUnknownStrategy IUnknownStrategy { get; }

	private IIUnknownCacheStrategy CacheStrategy { get; }

	internal bool UniqueInstance { get; init; }

	internal unsafe ComObject(IIUnknownInterfaceDetailsStrategy interfaceDetailsStrategy, IIUnknownStrategy iunknownStrategy, IIUnknownCacheStrategy cacheStrategy, void* thisPointer)
	{
		InterfaceDetailsStrategy = interfaceDetailsStrategy;
		IUnknownStrategy = iunknownStrategy;
		CacheStrategy = cacheStrategy;
		_instancePointer = IUnknownStrategy.CreateInstancePointer(thisPointer);
		if (OperatingSystem.IsWindows() && BuiltInComSupported && ComImportInteropEnabled)
		{
			_runtimeCallableWrapper = Marshal.GetObjectForIUnknown((nint)thisPointer);
		}
	}

	unsafe ~ComObject()
	{
		CacheStrategy.Clear(IUnknownStrategy);
		IUnknownStrategy.Release(_instancePointer);
	}

	public unsafe void FinalRelease()
	{
		if (UniqueInstance && !Interlocked.Exchange(ref _released, value: true))
		{
			GC.SuppressFinalize(this);
			CacheStrategy.Clear(IUnknownStrategy);
			IUnknownStrategy.Release(_instancePointer);
		}
	}

	RuntimeTypeHandle IDynamicInterfaceCastable.GetInterfaceImplementation(RuntimeTypeHandle interfaceType)
	{
		if (!LookUpVTableInfo(interfaceType, out var result, out var qiHResult))
		{
			Marshal.ThrowExceptionForHR(qiHResult);
		}
		return result.ManagedType;
	}

	bool IDynamicInterfaceCastable.IsInterfaceImplemented(RuntimeTypeHandle interfaceType, bool throwIfNotImplemented)
	{
		if (!LookUpVTableInfo(interfaceType, out var _, out var qiHResult))
		{
			if (throwIfNotImplemented)
			{
				Marshal.ThrowExceptionForHR(qiHResult);
			}
			return false;
		}
		return true;
	}

	private unsafe bool LookUpVTableInfo(RuntimeTypeHandle handle, out IIUnknownCacheStrategy.TableInfo result, out int qiHResult)
	{
		ObjectDisposedException.ThrowIf(_released, this);
		qiHResult = 0;
		if (!CacheStrategy.TryGetTableInfo(handle, out result))
		{
			IIUnknownDerivedDetails iUnknownDerivedDetails = InterfaceDetailsStrategy.GetIUnknownDerivedDetails(handle);
			if (iUnknownDerivedDetails == null)
			{
				return false;
			}
			int num = IUnknownStrategy.QueryInterface(_instancePointer, iUnknownDerivedDetails.Iid, out var ppObj);
			if (num < 0)
			{
				qiHResult = num;
				return false;
			}
			result = CacheStrategy.ConstructTableInfo(handle, iUnknownDerivedDetails, ppObj);
			if (!CacheStrategy.TrySetTableInfo(handle, result))
			{
				CacheStrategy.TryGetTableInfo(handle, out result);
				IUnknownStrategy.Release(ppObj);
			}
		}
		return true;
	}

	unsafe VirtualMethodTableInfo IUnmanagedVirtualMethodTableProvider.GetVirtualMethodTableInfoForKey(Type type)
	{
		if (!LookUpVTableInfo(type.TypeHandle, out var result, out var qiHResult))
		{
			Marshal.ThrowExceptionForHR(qiHResult);
		}
		return new VirtualMethodTableInfo(result.ThisPtr, result.Table);
	}

	object System.Runtime.InteropServices.Marshalling.ComImportInteropInterfaceDetailsStrategy.IComImportAdapter.GetRuntimeCallableWrapper()
	{
		return _runtimeCallableWrapper;
	}
}
