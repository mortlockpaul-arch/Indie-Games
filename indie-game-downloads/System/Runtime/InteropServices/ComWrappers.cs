using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading;

namespace System.Runtime.InteropServices;

[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
[CLSCompliant(false)]
public abstract class ComWrappers
{
	internal sealed class ManagedObjectWrapperHolder
	{
		private readonly ManagedObjectWrapperReleaser _releaser;

		private readonly object _wrappedObject;

		private unsafe readonly ManagedObjectWrapper* _wrapper;

		public unsafe nint ComIp => _wrapper->As(in IID_IUnknown);

		public object WrappedObject => _wrappedObject;

		public unsafe bool IsActivated => _wrapper->Flags.HasFlag(CreateComInterfaceFlagsEx.IsComActivated);

		[DllImport("QCall", EntryPoint = "ComWrappers_RegisterIsRootedCallback", ExactSpelling = true)]
		[LibraryImport("QCall", EntryPoint = "ComWrappers_RegisterIsRootedCallback")]
		private static extern void RegisterIsRootedCallback();

		private static nint AllocateRefCountedHandle(ManagedObjectWrapperHolder holder)
		{
			return AllocateRefCountedHandle(ObjectHandleOnStack.Create(ref holder));
		}

		[DllImport("QCall", EntryPoint = "ComWrappers_AllocateRefCountedHandle", ExactSpelling = true)]
		[LibraryImport("QCall", EntryPoint = "ComWrappers_AllocateRefCountedHandle")]
		private static extern nint AllocateRefCountedHandle(ObjectHandleOnStack obj);

		static ManagedObjectWrapperHolder()
		{
			RegisterIsRootedCallback();
		}

		public unsafe ManagedObjectWrapperHolder(ManagedObjectWrapper* wrapper, object wrappedObject)
		{
			_wrapper = wrapper;
			_wrappedObject = wrappedObject;
			_releaser = new ManagedObjectWrapperReleaser(wrapper);
			_wrapper->HolderHandle = AllocateRefCountedHandle(this);
		}

		public unsafe uint AddRef()
		{
			return _wrapper->AddRef();
		}
	}

	public struct ComInterfaceEntry
	{
		public Guid IID;

		public nint Vtable;
	}

	public struct ComInterfaceDispatch
	{
		public nint Vtable;

		public unsafe static T GetInstance<T>(ComInterfaceDispatch* dispatchPtr) where T : class
		{
			return Unsafe.As<T>(ToManagedObjectWrapper(dispatchPtr)->Holder.WrappedObject);
		}

		internal unsafe static ManagedObjectWrapper* ToManagedObjectWrapper(ComInterfaceDispatch* dispatchPtr)
		{
			InternalComInterfaceDispatch* ptr = (InternalComInterfaceDispatch*)((nuint)dispatchPtr & unchecked((nuint)(-64)));
			return ptr->_thisPtr;
		}
	}

	internal struct InternalComInterfaceDispatch
	{
		[InlineArray(7)]
		internal struct DispatchTable
		{
			private nint _element;
		}

		internal unsafe ManagedObjectWrapper* _thisPtr;

		public DispatchTable Vtables;
	}

	internal enum CreateComInterfaceFlagsEx
	{
		None = 0,
		CallerDefinedIUnknown = 1,
		TrackerSupport = 2,
		LacksICustomQueryInterface = 536870912,
		IsComActivated = 1073741824,
		IsPegged = int.MinValue,
		InternalMask = -536870912
	}

	internal struct ManagedObjectWrapper
	{
		public volatile nint HolderHandle;

		public ulong RefCount;

		internal CreateComInterfaceFlagsEx Flags;

		public int UserDefinedCount;

		public unsafe ComInterfaceEntry* UserDefined;

		internal unsafe InternalComInterfaceDispatch* Dispatches;

		public ManagedObjectWrapperHolder Holder
		{
			get
			{
				nint holderHandle = HolderHandle;
				if (holderHandle == IntPtr.Zero)
				{
					return null;
				}
				return Unsafe.As<ManagedObjectWrapperHolder>(GCHandle.FromIntPtr(holderHandle).Target);
			}
		}

		public readonly bool MarkedToDestroy => IsMarkedToDestroy(RefCount);

		public uint AddRef()
		{
			return GetComCount(Interlocked.Increment(ref RefCount));
		}

		public nint As(in Guid riid)
		{
			nint num = AsRuntimeDefined(in riid);
			if (num == IntPtr.Zero)
			{
				num = AsUserDefined(in riid);
			}
			return num;
		}

		public bool Destroy()
		{
			if (HolderHandle == IntPtr.Zero)
			{
				return true;
			}
			ulong refCount;
			ulong num;
			do
			{
				refCount = RefCount;
				num = refCount | 0x80000000u;
			}
			while (Interlocked.CompareExchange(ref RefCount, num, refCount) != refCount);
			if (num == 2147483648u)
			{
				nint num2 = Interlocked.Exchange(ref HolderHandle, IntPtr.Zero);
				if (num2 != IntPtr.Zero)
				{
					GCHandle.InternalFree(num2);
				}
				return true;
			}
			return false;
		}

		private unsafe nint GetDispatchPointerAtIndex(int index)
		{
			nint* ptr = (nint*)(&Dispatches[index / 7].Vtables);
			return (nint)((byte*)ptr + (nint)(index % 7) * (nint)8);
		}

		private nint AsRuntimeDefined(in Guid riid)
		{
			int num = UserDefinedCount;
			if ((Flags & CreateComInterfaceFlagsEx.CallerDefinedIUnknown) == 0)
			{
				if (riid == IID_IUnknown)
				{
					return GetDispatchPointerAtIndex(num);
				}
				num++;
			}
			if ((Flags & CreateComInterfaceFlagsEx.TrackerSupport) != CreateComInterfaceFlagsEx.None)
			{
				if (riid == IID_IReferenceTrackerTarget)
				{
					return GetDispatchPointerAtIndex(num);
				}
				num++;
			}
			if (riid == IID_TaggedImpl)
			{
				return GetDispatchPointerAtIndex(num);
			}
			return IntPtr.Zero;
		}

		private unsafe nint AsUserDefined(in Guid riid)
		{
			for (int i = 0; i < UserDefinedCount; i++)
			{
				if (UserDefined[i].IID == riid)
				{
					return GetDispatchPointerAtIndex(i);
				}
			}
			return IntPtr.Zero;
		}

		private static uint GetComCount(ulong c)
		{
			return (uint)(c & 0x7FFFFFFF);
		}

		private static bool IsMarkedToDestroy(ulong c)
		{
			return (c & 0x80000000u) != 0;
		}
	}

	internal sealed class ManagedObjectWrapperReleaser
	{
		private unsafe ManagedObjectWrapper* _wrapper;

		public unsafe ManagedObjectWrapperReleaser(ManagedObjectWrapper* wrapper)
		{
			_wrapper = wrapper;
		}

		unsafe ~ManagedObjectWrapperReleaser()
		{
			nint holderHandle = _wrapper->HolderHandle;
			if (holderHandle != IntPtr.Zero && GCHandle.InternalGet(holderHandle) != null)
			{
				GC.ReRegisterForFinalize(this);
			}
			else
			{
				if (_wrapper->Destroy())
				{
					NativeMemory.AlignedFree(_wrapper);
					_wrapper = null;
				}
				else
				{
					GC.ReRegisterForFinalize(this);
				}
			}
		}
	}

	internal class NativeObjectWrapper
	{
		private ComWrappers _comWrappers;

		private nint _externalComObject;

		private nint _inner;

		private GCHandle _proxyHandle;

		private GCHandle _proxyHandleTrackingResurrection;

		private readonly bool _aggregatedManagedObjectWrapper;

		private readonly bool _uniqueInstance;

		internal nint ExternalComObject => _externalComObject;

		internal ComWrappers ComWrappers => _comWrappers;

		internal GCHandle ProxyHandle => _proxyHandle;

		internal bool IsUniqueInstance => _uniqueInstance;

		internal bool IsAggregatedWithManagedObjectWrapper => _aggregatedManagedObjectWrapper;

		unsafe static NativeObjectWrapper()
		{
			ComAwareWeakReference.InitializeCallbacks((delegate*<nint, object, object>)(&ComWeakRefToObject), (delegate*<object, bool>)(&PossiblyComObject), (delegate*<object, out object, nint>)(&ObjectToComWeakRef));
		}

		public static NativeObjectWrapper Create(nint externalComObject, nint inner, ComWrappers comWrappers, object comProxy, CreateObjectFlags flags, ref nint referenceTrackerMaybe)
		{
			if (flags.HasFlag(CreateObjectFlags.TrackerObject))
			{
				nint ppv = referenceTrackerMaybe;
				referenceTrackerMaybe = IntPtr.Zero;
				if (ppv != IntPtr.Zero || Marshal.QueryInterface(externalComObject, in IID_IReferenceTracker, out ppv) == 0)
				{
					return new ReferenceTrackerNativeObjectWrapper(externalComObject, inner, comWrappers, comProxy, flags, ppv);
				}
			}
			return new NativeObjectWrapper(externalComObject, inner, comWrappers, comProxy, flags);
		}

		protected unsafe NativeObjectWrapper(nint externalComObject, nint inner, ComWrappers comWrappers, object comProxy, CreateObjectFlags flags)
		{
			_externalComObject = externalComObject;
			_inner = inner;
			_comWrappers = comWrappers;
			_uniqueInstance = flags.HasFlag(CreateObjectFlags.UniqueInstance);
			_proxyHandle = GCHandle.Alloc(comProxy, GCHandleType.Weak);
			_proxyHandleTrackingResurrection = GCHandle.Alloc(comProxy, GCHandleType.WeakTrackResurrection);
			_aggregatedManagedObjectWrapper = flags.HasFlag(CreateObjectFlags.Aggregation) && TryGetComInterfaceDispatch(_externalComObject) != null;
			if (_aggregatedManagedObjectWrapper)
			{
				Marshal.Release(externalComObject);
			}
		}

		public virtual void Release()
		{
			if (!_uniqueInstance && _comWrappers != null)
			{
				_comWrappers._rcwCache.Remove(_externalComObject, this);
				_comWrappers = null;
			}
			if (_proxyHandle.IsAllocated)
			{
				_proxyHandle.Free();
			}
			if (_proxyHandleTrackingResurrection.IsAllocated)
			{
				_proxyHandleTrackingResurrection.Free();
			}
			if (_inner != IntPtr.Zero)
			{
				Marshal.Release(_inner);
				_inner = IntPtr.Zero;
			}
			_externalComObject = IntPtr.Zero;
		}

		~NativeObjectWrapper()
		{
			if (_proxyHandleTrackingResurrection.IsAllocated && _proxyHandleTrackingResurrection.Target != null)
			{
				GC.ReRegisterForFinalize(this);
			}
			else
			{
				Release();
			}
		}
	}

	internal sealed class ReferenceTrackerNativeObjectWrapper : NativeObjectWrapper
	{
		private nint _trackerObject;

		internal readonly nint _contextToken;

		private int _trackerObjectDisconnected;

		private readonly bool _releaseTrackerObject;

		internal readonly GCHandle _nativeObjectWrapperWeakHandle;

		public ReferenceTrackerNativeObjectWrapper(nint externalComObject, nint inner, ComWrappers comWrappers, object comProxy, CreateObjectFlags flags, nint trackerObject)
			: base(externalComObject, inner, comWrappers, comProxy, flags)
		{
			_trackerObject = trackerObject;
			_releaseTrackerObject = true;
			TrackerObjectManager.OnIReferenceTrackerFound(_trackerObject);
			TrackerObjectManager.AfterWrapperCreated(_trackerObject);
			if (flags.HasFlag(CreateObjectFlags.Aggregation))
			{
				_releaseTrackerObject = false;
				IReferenceTracker.ReleaseFromTrackerSource(_trackerObject);
				Marshal.Release(_trackerObject);
			}
			_contextToken = TrackerObjectManager.GetContextToken();
			_nativeObjectWrapperWeakHandle = GCHandle.Alloc(this, GCHandleType.Weak);
		}

		public override void Release()
		{
			if (_nativeObjectWrapperWeakHandle.IsAllocated)
			{
				TrackerObjectManager.s_referenceTrackerNativeObjectWrapperCache.Remove(_nativeObjectWrapperWeakHandle);
				_nativeObjectWrapperWeakHandle.Free();
			}
			DisconnectTracker();
			base.Release();
		}

		public void DisconnectTracker()
		{
			if (_trackerObject != IntPtr.Zero && Interlocked.CompareExchange(ref _trackerObjectDisconnected, 1, 0) == 0)
			{
				IReferenceTracker.ReleaseFromTrackerSource(_trackerObject);
				if (_releaseTrackerObject)
				{
					IReferenceTracker.ReleaseFromTrackerSource(_trackerObject);
					Marshal.Release(_trackerObject);
					_trackerObject = IntPtr.Zero;
				}
			}
		}
	}

	private sealed class NoUserState
	{
		public static readonly NoUserState Instance = new NoUserState();

		private NoUserState()
		{
		}
	}

	private sealed class RcwCache
	{
		private readonly Lock _lock = new Lock(useTrivialWaits: true);

		private readonly Dictionary<nint, GCHandle> _cache = new Dictionary<nint, GCHandle>();

		public (NativeObjectWrapper actualWrapper, object actualProxy) GetOrAddProxyForComInstance(nint comPointer, NativeObjectWrapper wrapper, object comProxy)
		{
			using (_lock.EnterScope())
			{
				ref GCHandle valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(_cache, comPointer, out var exists);
				if (!exists)
				{
					valueRefOrAddDefault = GCHandle.Alloc(wrapper, GCHandleType.Weak);
				}
				else if (!(valueRefOrAddDefault.Target is NativeObjectWrapper { ProxyHandle: var proxyHandle } nativeObjectWrapper))
				{
					valueRefOrAddDefault.Target = wrapper;
				}
				else
				{
					object target = proxyHandle.Target;
					if (target != null)
					{
						return (actualWrapper: nativeObjectWrapper, actualProxy: target);
					}
					valueRefOrAddDefault.Target = wrapper;
				}
				return (actualWrapper: wrapper, actualProxy: comProxy);
			}
		}

		public object FindProxyForComInstance(nint comPointer)
		{
			using (_lock.EnterScope())
			{
				if (_cache.TryGetValue(comPointer, out var value))
				{
					if (value.Target is NativeObjectWrapper { ProxyHandle: var proxyHandle })
					{
						object target = proxyHandle.Target;
						if (target != null)
						{
							return target;
						}
					}
					_cache.Remove(comPointer);
					value.Free();
				}
				return null;
			}
		}

		public void Remove(nint comPointer, NativeObjectWrapper wrapper)
		{
			using (_lock.EnterScope())
			{
				Remove_Locked(comPointer, wrapper);
			}
		}

		public void RemoveAll(IEnumerable<NativeObjectWrapper> wrappers)
		{
			using (_lock.EnterScope())
			{
				foreach (NativeObjectWrapper wrapper in wrappers)
				{
					Remove_Locked(wrapper.ExternalComObject, wrapper);
				}
			}
		}

		private void Remove_Locked(nint comPointer, NativeObjectWrapper wrapper)
		{
			if (_cache.TryGetValue(comPointer, out var value) && (wrapper == value.Target || value.Target == null))
			{
				_cache.Remove(comPointer);
				value.Free();
			}
		}
	}

	private static class IWeakReference
	{
		public unsafe static int Resolve(nint pThis, Guid guid, out nint inspectable)
		{
			fixed (nint* ptr = &inspectable)
			{
				return ((delegate* unmanaged[MemberFunction]<nint, Guid*, nint*, int>)(*(IntPtr*)((nint)(*(IntPtr*)pThis) + (nint)3 * (nint)sizeof(delegate* unmanaged[MemberFunction]<nint, Guid*, nint*, int>))))(pThis, &guid, ptr);
			}
		}
	}

	private static class IWeakReferenceSource
	{
		public unsafe static int GetWeakReference(nint pThis, out nint weakReference)
		{
			fixed (nint* ptr = &weakReference)
			{
				return ((delegate* unmanaged[MemberFunction]<nint, nint*, int>)(*(IntPtr*)((nint)(*(IntPtr*)pThis) + (nint)3 * (nint)sizeof(delegate* unmanaged[MemberFunction]<nint, nint*, int>))))(pThis, ptr);
			}
		}
	}

	internal static readonly Guid IID_IUnknown = new Guid(0, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

	internal static readonly Guid IID_IReferenceTrackerTarget = new Guid(1690125304u, 49134, 20164, 183, 235, 41, 53, 21, 141, 174, 33);

	internal static readonly Guid IID_TaggedImpl = new Guid(1544807708, 20274, 18214, 163, 253, 243, 237, 214, 61, 163, 160);

	internal static readonly Guid IID_IReferenceTracker = new Guid(299086138, 6158, 18313, 168, 190, 119, 18, 136, 40, 147, 230);

	internal static readonly Guid IID_IReferenceTrackerHost = new Guid(698817642, 15426, 17430, 163, 157, 226, 130, 90, 7, 167, 115);

	internal static readonly Guid IID_IReferenceTrackerManager = new Guid(1022461108, 31947, 19930, 132, 85, 126, 108, 233, 154, 50, 152);

	internal static readonly Guid IID_IFindReferenceTargetsCallback = new Guid(78858348, 18055, 16937, 141, 20, 80, 90, 181, 132, 221, 136);

	private static readonly Guid IID_IInspectable = new Guid(2944852704u, 45357, 19562, 156, 90, 215, 170, 101, 16, 30, 144);

	private static readonly Guid IID_IWeakReferenceSource = new Guid(56, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

	private static readonly ConditionalWeakTable<object, NativeObjectWrapper> s_nativeObjectWrapperTable = new ConditionalWeakTable<object, NativeObjectWrapper>();

	private static readonly ConditionalWeakTable<object, List<ManagedObjectWrapperHolder>> s_allManagedObjectWrapperTable = new ConditionalWeakTable<object, List<ManagedObjectWrapperHolder>>();

	private readonly ConditionalWeakTable<object, ManagedObjectWrapperHolder> _managedObjectWrapperTable = new ConditionalWeakTable<object, ManagedObjectWrapperHolder>();

	private readonly RcwCache _rcwCache = new RcwCache();

	private static ComWrappers s_globalInstanceForTrackerSupport;

	private static ComWrappers s_globalInstanceForMarshalling;

	internal static nint DefaultIUnknownVftblPtr { get; } = CreateDefaultIUnknownVftbl();

	internal static nint TaggedImplVftblPtr { get; } = CreateTaggedImplVftbl();

	internal static nint DefaultIReferenceTrackerTargetVftblPtr { get; } = CreateDefaultIReferenceTrackerTargetVftbl();

	internal static ComWrappers? GlobalInstanceForTrackerSupport => s_globalInstanceForTrackerSupport;

	public static void GetIUnknownImpl(out nint fpQueryInterface, out nint fpAddRef, out nint fpRelease)
	{
		GetIUnknownImplInternal(out fpQueryInterface, out fpAddRef, out fpRelease);
	}

	[LibraryImport("QCall", EntryPoint = "ComWrappers_GetIUnknownImpl")]
	[SuppressGCTransition]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static void GetIUnknownImplInternal(out nint fpQueryInterface, out nint fpAddRef, out nint fpRelease)
	{
		fpQueryInterface = 0;
		fpAddRef = 0;
		fpRelease = 0;
		fixed (nint* _fpRelease_native = &fpRelease)
		{
			fixed (nint* _fpAddRef_native = &fpAddRef)
			{
				fixed (nint* _fpQueryInterface_native = &fpQueryInterface)
				{
					__PInvoke(_fpQueryInterface_native, _fpAddRef_native, _fpRelease_native);
				}
			}
		}
		[DllImport("QCall", EntryPoint = "ComWrappers_GetIUnknownImpl", ExactSpelling = true)]
		[SuppressGCTransition]
		unsafe static extern void __PInvoke(nint* __fpQueryInterface_native, nint* __fpAddRef_native, nint* __fpRelease_native);
	}

	internal unsafe static void GetUntrackedIUnknownImpl(out delegate* unmanaged[MemberFunction]<nint, uint> fpAddRef, out delegate* unmanaged[MemberFunction]<nint, uint> fpRelease)
	{
		Unsafe.As<delegate* unmanaged[MemberFunction]<nint, uint>, IntPtr>(ref fpAddRef) = (Unsafe.As<delegate* unmanaged[MemberFunction]<nint, uint>, IntPtr>(ref fpRelease) = (nint)GetUntrackedAddRefRelease());
	}

	[DllImport("QCall", EntryPoint = "ComWrappers_GetUntrackedAddRefRelease", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ComWrappers_GetUntrackedAddRefRelease")]
	[SuppressGCTransition]
	private unsafe static extern delegate* unmanaged[MemberFunction]<nint, uint> GetUntrackedAddRefRelease();

	private unsafe static nint CreateDefaultIUnknownVftbl()
	{
		nint* ptr = (nint*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(ComWrappers), 3 * 8);
		GetIUnknownImpl(out *ptr, out *(nint*)((byte*)ptr + 8), out *(nint*)((byte*)ptr + (nint)2 * (nint)8));
		return (nint)ptr;
	}

	private unsafe static nint CreateTaggedImplVftbl()
	{
		nint* ptr = (nint*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(ComWrappers), 4 * 8);
		GetIUnknownImpl(out *ptr, out *(nint*)((byte*)ptr + 8), out *(nint*)((byte*)ptr + (nint)2 * (nint)8));
		*(nint*)((byte*)ptr + (nint)3 * (nint)8) = GetTaggedImplCurrentVersion();
		return (nint)ptr;
	}

	internal static int CallICustomQueryInterface(ManagedObjectWrapperHolder holder, ref Guid iid, out nint ppObject)
	{
		if (holder.WrappedObject is ICustomQueryInterface customQueryInterface)
		{
			return (int)customQueryInterface.GetInterface(ref iid, out ppObject);
		}
		ppObject = IntPtr.Zero;
		return -1;
	}

	internal static nint GetOrCreateComInterfaceForObjectWithGlobalMarshallingInstance(object obj)
	{
		if (s_globalInstanceForMarshalling == null)
		{
			return IntPtr.Zero;
		}
		try
		{
			return ComInterfaceForObject(obj);
		}
		catch (ArgumentException)
		{
			return IntPtr.Zero;
		}
	}

	internal static object GetOrCreateObjectForComInstanceWithGlobalMarshallingInstance(nint comObject, CreateObjectFlags flags)
	{
		if (s_globalInstanceForMarshalling == null)
		{
			return null;
		}
		try
		{
			return ComObjectForInterface(comObject, flags);
		}
		catch (ArgumentNullException)
		{
			return null;
		}
	}

	[DllImport("QCall", EntryPoint = "ComWrappers_GetIReferenceTrackerTargetVftbl", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ComWrappers_GetIReferenceTrackerTargetVftbl")]
	[SuppressGCTransition]
	private static extern nint GetDefaultIReferenceTrackerTargetVftbl();

	private static nint CreateDefaultIReferenceTrackerTargetVftbl()
	{
		return GetDefaultIReferenceTrackerTargetVftbl();
	}

	private static nint GetTaggedImplCurrentVersion()
	{
		return GetTaggedImpl();
	}

	[DllImport("QCall", EntryPoint = "ComWrappers_GetTaggedImpl", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ComWrappers_GetTaggedImpl")]
	[SuppressGCTransition]
	private static extern nint GetTaggedImpl();

	internal static bool TryGetComInstanceForIID(object obj, Guid iid, out nint unknown, out ComWrappers comWrappers)
	{
		if (obj == null || !s_nativeObjectWrapperTable.TryGetValue(obj, out var value))
		{
			unknown = IntPtr.Zero;
			comWrappers = null;
			return false;
		}
		comWrappers = value.ComWrappers;
		return Marshal.QueryInterface(value.ExternalComObject, in iid, out unknown) == 0;
	}

	public static bool TryGetComInstance(object obj, out nint unknown)
	{
		unknown = IntPtr.Zero;
		if (obj == null || !s_nativeObjectWrapperTable.TryGetValue(obj, out var value))
		{
			return false;
		}
		return Marshal.QueryInterface(value.ExternalComObject, in IID_IUnknown, out unknown) == 0;
	}

	public unsafe static bool TryGetObject(nint unknown, [NotNullWhen(true)] out object? obj)
	{
		obj = null;
		if (unknown == IntPtr.Zero)
		{
			return false;
		}
		ComInterfaceDispatch* ptr = TryGetComInterfaceDispatch(unknown);
		if (ptr == null || ComInterfaceDispatch.ToManagedObjectWrapper(ptr)->MarkedToDestroy)
		{
			return false;
		}
		obj = ComInterfaceDispatch.GetInstance<object>(ptr);
		return true;
	}

	internal static object GetOrCreateObjectFromWrapper(ComWrappers wrapper, nint externalComObject)
	{
		if (wrapper == null)
		{
			return null;
		}
		if (s_globalInstanceForTrackerSupport == wrapper)
		{
			return s_globalInstanceForTrackerSupport.GetOrCreateObjectForComInstance(externalComObject, CreateObjectFlags.TrackerObject);
		}
		if (s_globalInstanceForMarshalling == wrapper)
		{
			return ComObjectForInterface(externalComObject, CreateObjectFlags.TrackerObject | CreateObjectFlags.Unwrap);
		}
		return null;
	}

	public unsafe nint GetOrCreateComInterfaceForObject(object instance, CreateComInterfaceFlags flags)
	{
		ArgumentNullException.ThrowIfNull(instance, "instance");
		ManagedObjectWrapperHolder orAdd = _managedObjectWrapperTable.GetOrAdd(instance, (object c, state) => new ManagedObjectWrapperHolder(state.This.CreateManagedObjectWrapper(c, state.flags), c), new
		{
			This = this,
			flags = flags
		});
		orAdd.AddRef();
		RegisterManagedObjectWrapperForDiagnostics(instance, orAdd);
		return orAdd.ComIp;
	}

	private static void RegisterManagedObjectWrapperForDiagnostics(object instance, ManagedObjectWrapperHolder wrapper)
	{
		if (!Debugger.IsSupported)
		{
			return;
		}
		List<ManagedObjectWrapperHolder> orCreateValue = s_allManagedObjectWrapperTable.GetOrCreateValue(instance);
		lock (orCreateValue)
		{
			orCreateValue.Add(wrapper);
		}
	}

	private static nuint AlignUp(nuint value, nuint alignment)
	{
		nuint num = alignment - 1;
		return (value + num) & ~num;
	}

	private unsafe ManagedObjectWrapper* CreateManagedObjectWrapper(object instance, CreateComInterfaceFlags flags)
	{
		ComInterfaceEntry* ptr = ComputeVtables(instance, flags, out var count);
		if ((ptr == null && count != 0) || count < 0)
		{
			throw new ArgumentException();
		}
		Span<nint> span = new Span<nint>(stackalloc byte[(int)checked((nuint)3u * (nuint)8u)], 3);
		int num = 0;
		if ((flags & CreateComInterfaceFlags.CallerDefinedIUnknown) == 0)
		{
			span[num++] = DefaultIUnknownVftblPtr;
		}
		if ((flags & CreateComInterfaceFlags.TrackerSupport) != CreateComInterfaceFlags.None)
		{
			span[num++] = DefaultIReferenceTrackerTargetVftblPtr;
		}
		span[num++] = TaggedImplVftblPtr;
		int num2 = num + count;
		int num3 = num2 / 7;
		if (num2 % 7 != 0)
		{
			num3++;
		}
		nuint num4 = AlignUp((nuint)sizeof(ManagedObjectWrapper), 64u);
		nuint num5 = (nuint)((nint)num2 * (nint)sizeof(void*) + (nint)num3 * (nint)sizeof(void*));
		nint num6 = (nint)NativeMemory.AlignedAlloc(num4 + num5, 64u);
		InternalComInterfaceDispatch* ptr2 = (InternalComInterfaceDispatch*)((nuint)num6 + num4);
		Span<InternalComInterfaceDispatch> span2 = new Span<InternalComInterfaceDispatch>(ptr2, num3);
		for (int i = 0; i < span2.Length; i++)
		{
			span2[i]._thisPtr = (ManagedObjectWrapper*)num6;
			Span<nint> span3 = span2[i].Vtables;
			for (int j = 0; j < span3.Length; j++)
			{
				int num7 = i * span3.Length + j;
				if (num7 >= num2)
				{
					break;
				}
				span3[j] = ((num7 < count) ? ptr[num7].Vtable : span[num7 - count]);
			}
		}
		ManagedObjectWrapper* ptr3 = (ManagedObjectWrapper*)num6;
		ptr3->HolderHandle = IntPtr.Zero;
		ptr3->RefCount = 0uL;
		ptr3->UserDefinedCount = count;
		ptr3->UserDefined = ptr;
		ptr3->Flags = (CreateComInterfaceFlagsEx)flags;
		ptr3->Dispatches = ptr2;
		return ptr3;
	}

	public object GetOrCreateObjectForComInstance(nint externalComObject, CreateObjectFlags flags)
	{
		if (!TryGetOrCreateObjectForComInstanceInternal(externalComObject, IntPtr.Zero, flags, null, NoUserState.Instance, out var retValue))
		{
			throw new ArgumentNullException("externalComObject");
		}
		return retValue;
	}

	public object GetOrCreateObjectForComInstance(nint externalComObject, CreateObjectFlags flags, object? userState)
	{
		if (!TryGetOrCreateObjectForComInstanceInternal(externalComObject, IntPtr.Zero, flags, null, userState, out var retValue))
		{
			throw new ArgumentNullException("externalComObject");
		}
		return retValue;
	}

	public object GetOrRegisterObjectForComInstance(nint externalComObject, CreateObjectFlags flags, object wrapper)
	{
		return GetOrRegisterObjectForComInstance(externalComObject, flags, wrapper, IntPtr.Zero);
	}

	public object GetOrRegisterObjectForComInstance(nint externalComObject, CreateObjectFlags flags, object wrapper, nint inner)
	{
		ArgumentNullException.ThrowIfNull(wrapper, "wrapper");
		if (!TryGetOrCreateObjectForComInstanceInternal(externalComObject, inner, flags, wrapper, NoUserState.Instance, out var retValue))
		{
			throw new ArgumentNullException("externalComObject");
		}
		return retValue;
	}

	private unsafe static ComInterfaceDispatch* TryGetComInterfaceDispatch(nint comObject)
	{
		nint num = *(*(nint**)comObject);
		if (num != (nint)(*(IntPtr*)DefaultIUnknownVftblPtr) || num != (nint)(*(IntPtr*)DefaultIReferenceTrackerTargetVftblPtr))
		{
			if (Marshal.QueryInterface(comObject, in IID_TaggedImpl, out var ppv) != 0)
			{
				return null;
			}
			nint taggedImplCurrentVersion = GetTaggedImplCurrentVersion();
			int num2 = ((delegate* unmanaged[MemberFunction]<nint, nint, int>)(*(IntPtr*)((nint)(*(IntPtr*)ppv) + (nint)3 * (nint)sizeof(void*))))(ppv, taggedImplCurrentVersion);
			Marshal.Release(ppv);
			if (num2 != 0)
			{
				return null;
			}
		}
		return (ComInterfaceDispatch*)comObject;
	}

	private static void DetermineIdentityAndInner(nint externalComObject, nint innerMaybe, CreateObjectFlags flags, out nint identity, out nint inner, out nint referenceTrackerMaybe)
	{
		inner = innerMaybe;
		nint num = externalComObject;
		bool flag = flags.HasFlag(CreateObjectFlags.TrackerObject) && flags.HasFlag(CreateObjectFlags.Aggregation);
		if (flag && Marshal.QueryInterface(externalComObject, in IID_IReferenceTracker, out var ppv) == 0)
		{
			num = ppv;
			referenceTrackerMaybe = ppv;
			Marshal.ThrowExceptionForHR(Marshal.QueryInterface(num, in IID_IUnknown, out identity));
		}
		else
		{
			referenceTrackerMaybe = IntPtr.Zero;
			Marshal.ThrowExceptionForHR(Marshal.QueryInterface(externalComObject, in IID_IUnknown, out identity));
		}
		if ((innerMaybe == IntPtr.Zero && num != externalComObject && externalComObject != identity) & flag)
		{
			inner = externalComObject;
		}
	}

	private unsafe bool TryGetOrCreateObjectForComInstanceInternal(nint externalComObject, nint innerMaybe, CreateObjectFlags flags, object wrapperMaybe, object userState, [NotNullWhen(true)] out object retValue)
	{
		if (externalComObject == IntPtr.Zero)
		{
			throw new ArgumentNullException("externalComObject");
		}
		if (innerMaybe != IntPtr.Zero && !flags.HasFlag(CreateObjectFlags.Aggregation))
		{
			throw new InvalidOperationException(SR.InvalidOperation_SuppliedInnerMustBeMarkedAggregation);
		}
		DetermineIdentityAndInner(externalComObject, innerMaybe, flags, out var identity, out var inner, out var referenceTrackerMaybe);
		try
		{
			if (flags.HasFlag(CreateObjectFlags.UniqueInstance))
			{
				retValue = CreateAndRegisterObjectForComInstance(identity, inner, flags, userState, ref referenceTrackerMaybe);
				return retValue != null;
			}
			object obj = _rcwCache.FindProxyForComInstance(identity);
			if (obj != null)
			{
				retValue = obj;
				return true;
			}
			if (wrapperMaybe != null)
			{
				retValue = RegisterObjectForComInstance(identity, inner, wrapperMaybe, flags, ref referenceTrackerMaybe);
				return retValue != null;
			}
			if (flags.HasFlag(CreateObjectFlags.Unwrap))
			{
				ComInterfaceDispatch* ptr = TryGetComInterfaceDispatch(identity);
				if (ptr != null)
				{
					object instance = ComInterfaceDispatch.GetInstance<object>(ptr);
					if (_managedObjectWrapperTable.TryGetValue(instance, out var value) && value.ComIp == identity && !value.IsActivated)
					{
						retValue = instance;
						return true;
					}
				}
			}
			retValue = CreateAndRegisterObjectForComInstance(identity, inner, flags, userState, ref referenceTrackerMaybe);
			return retValue != null;
		}
		finally
		{
			Marshal.Release(identity);
			if (referenceTrackerMaybe != IntPtr.Zero)
			{
				Marshal.Release(referenceTrackerMaybe);
			}
		}
	}

	private object CreateAndRegisterObjectForComInstance(nint identity, nint inner, CreateObjectFlags flags, object userState, ref nint referenceTrackerMaybe)
	{
		CreatedWrapperFlags wrapperFlags = CreatedWrapperFlags.None;
		object obj = ((userState is NoUserState) ? CreateObject(identity, flags) : CreateObject(identity, flags, userState, out wrapperFlags));
		if (obj == null)
		{
			return null;
		}
		if (wrapperFlags.HasFlag(CreatedWrapperFlags.NonWrapping))
		{
			return obj;
		}
		if (wrapperFlags.HasFlag(CreatedWrapperFlags.TrackerObject))
		{
			flags |= CreateObjectFlags.TrackerObject;
		}
		return RegisterObjectForComInstance(identity, inner, obj, flags, ref referenceTrackerMaybe);
	}

	private object RegisterObjectForComInstance(nint identity, nint inner, object comProxy, CreateObjectFlags flags, ref nint referenceTrackerMaybe)
	{
		NativeObjectWrapper nativeObjectWrapper = NativeObjectWrapper.Create(identity, inner, this, comProxy, flags, ref referenceTrackerMaybe);
		object obj = comProxy;
		NativeObjectWrapper nativeObjectWrapper2 = nativeObjectWrapper;
		if (!nativeObjectWrapper.IsUniqueInstance)
		{
			(nativeObjectWrapper2, obj) = _rcwCache.GetOrAddProxyForComInstance(identity, nativeObjectWrapper, comProxy);
			if (nativeObjectWrapper2 != nativeObjectWrapper)
			{
				nativeObjectWrapper.Release();
			}
		}
		RegisterWrapperForObject(nativeObjectWrapper2, obj);
		return obj;
	}

	private void RegisterWrapperForObject(NativeObjectWrapper wrapper, object comProxy)
	{
		NativeObjectWrapper orAdd = s_nativeObjectWrapperTable.GetOrAdd(comProxy, wrapper);
		if (orAdd != wrapper)
		{
			wrapper.Release();
			throw new NotSupportedException();
		}
		AddWrapperToReferenceTrackerHandleCache(orAdd);
	}

	private static void AddWrapperToReferenceTrackerHandleCache(NativeObjectWrapper wrapper)
	{
		if (wrapper is ReferenceTrackerNativeObjectWrapper referenceTrackerNativeObjectWrapper)
		{
			TrackerObjectManager.s_referenceTrackerNativeObjectWrapperCache.Add(referenceTrackerNativeObjectWrapper._nativeObjectWrapperWeakHandle);
		}
	}

	internal void RemoveWrappersFromCache(IEnumerable<NativeObjectWrapper> wrappers)
	{
		_rcwCache.RemoveAll(wrappers);
	}

	public static void RegisterForTrackerSupport(ComWrappers instance)
	{
		ArgumentNullException.ThrowIfNull(instance, "instance");
		if (Interlocked.CompareExchange(ref s_globalInstanceForTrackerSupport, instance, null) != null)
		{
			throw new InvalidOperationException(SR.InvalidOperation_ResetGlobalComWrappersInstance);
		}
	}

	[SupportedOSPlatform("windows")]
	public static void RegisterForMarshalling(ComWrappers instance)
	{
		ArgumentNullException.ThrowIfNull(instance, "instance");
		if (Interlocked.CompareExchange(ref s_globalInstanceForMarshalling, instance, null) != null)
		{
			throw new InvalidOperationException(SR.InvalidOperation_ResetGlobalComWrappersInstance);
		}
	}

	protected unsafe abstract ComInterfaceEntry* ComputeVtables(object obj, CreateComInterfaceFlags flags, out int count);

	protected abstract object? CreateObject(nint externalComObject, CreateObjectFlags flags);

	protected virtual object? CreateObject(nint externalComObject, CreateObjectFlags flags, object? userState, out CreatedWrapperFlags wrapperFlags)
	{
		throw new NotImplementedException(SR.NotImplemented_CreateObjectWithUserState);
	}

	protected internal abstract void ReleaseObjects(IEnumerable objects);

	internal static nint ComInterfaceForObject(object instance)
	{
		if (s_globalInstanceForMarshalling == null)
		{
			throw new NotSupportedException(SR.InvalidOperation_ComInteropRequireComWrapperInstance);
		}
		if (TryGetComInstance(instance, out var unknown))
		{
			return unknown;
		}
		return s_globalInstanceForMarshalling.GetOrCreateComInterfaceForObject(instance, CreateComInterfaceFlags.TrackerSupport);
	}

	internal static object ComObjectForInterface(nint externalComObject, CreateObjectFlags flags)
	{
		if (s_globalInstanceForMarshalling == null)
		{
			throw new NotSupportedException(SR.InvalidOperation_ComInteropRequireComWrapperInstance);
		}
		return s_globalInstanceForMarshalling.GetOrCreateObjectForComInstance(externalComObject, flags);
	}

	internal static nint GetOrCreateTrackerTarget(nint externalComObject)
	{
		if (s_globalInstanceForTrackerSupport == null)
		{
			throw new NotSupportedException(SR.InvalidOperation_ComInteropRequireComWrapperTrackerInstance);
		}
		object orCreateObjectForComInstance = s_globalInstanceForTrackerSupport.GetOrCreateObjectForComInstance(externalComObject, CreateObjectFlags.TrackerObject);
		return s_globalInstanceForTrackerSupport.GetOrCreateComInterfaceForObject(orCreateObjectForComInstance, CreateComInterfaceFlags.TrackerSupport);
	}

	private static object ComWeakRefToObject(nint pComWeakRef, object context)
	{
		if (!(context is ComWrappers wrapper))
		{
			return null;
		}
		if (IWeakReference.Resolve(pComWeakRef, IID_IInspectable, out var inspectable) == 0 && inspectable != IntPtr.Zero)
		{
			using ComHolder comHolder = new ComHolder(inspectable);
			if (Marshal.QueryInterface(comHolder.Ptr, in IID_IUnknown, out var ppv) == 0)
			{
				using (ComHolder comHolder2 = new ComHolder(ppv))
				{
					return GetOrCreateObjectFromWrapper(wrapper, comHolder2.Ptr);
				}
			}
		}
		return null;
	}

	private static bool PossiblyComObject(object target)
	{
		if (s_nativeObjectWrapperTable.TryGetValue(target, out var value))
		{
			return !value.IsAggregatedWithManagedObjectWrapper;
		}
		return false;
	}

	private static nint ObjectToComWeakRef(object target, out object context)
	{
		context = null;
		if (TryGetComInstanceForIID(target, IID_IWeakReferenceSource, out var unknown, out var comWrappers))
		{
			context = comWrappers;
			using ComHolder comHolder = new ComHolder(unknown);
			if (IWeakReferenceSource.GetWeakReference(comHolder.Ptr, out var weakReference) == 0)
			{
				return weakReference;
			}
		}
		return IntPtr.Zero;
	}
}
