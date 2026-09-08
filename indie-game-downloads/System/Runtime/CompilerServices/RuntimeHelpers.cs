using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;

namespace System.Runtime.CompilerServices;

public static class RuntimeHelpers
{
	public delegate void TryCode(object? userData);

	public delegate void CleanupCode(object? userData, bool exceptionThrown);

	[Obsolete("OffsetToStringData has been deprecated. Use string.GetPinnableReference() instead.")]
	public static int OffsetToStringData
	{
		[NonVersionable]
		get
		{
			return 12;
		}
	}

	[Intrinsic]
	public unsafe static void InitializeArray(Array array, RuntimeFieldHandle fldHandle)
	{
		if (array == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
		}
		if (fldHandle.IsNullHandle())
		{
			throw new ArgumentException(SR.Argument_InvalidHandle);
		}
		IRuntimeFieldInfo runtimeFieldInfo = fldHandle.GetRuntimeFieldInfo();
		if (!RuntimeFieldHandle.GetRVAFieldInfo(runtimeFieldInfo.Value, out var address, out var size))
		{
			throw new ArgumentException(SR.Argument_BadFieldForInitializeArray);
		}
		MethodTable* methodTable = GetMethodTable(array);
		TypeHandle arrayElementTypeHandle = methodTable->GetArrayElementTypeHandle();
		if (arrayElementTypeHandle.IsTypeDesc || !arrayElementTypeHandle.AsMethodTable()->IsPrimitive)
		{
			throw new ArgumentException(SR.Argument_BadArrayForInitializeArray);
		}
		nuint num = methodTable->ComponentSize * array.NativeLength;
		if (num > size)
		{
			throw new ArgumentException(SR.Argument_BadFieldForInitializeArray);
		}
		ref byte src = ref *(byte*)address;
		GC.KeepAlive(runtimeFieldInfo);
		ref byte arrayDataReference = ref MemoryMarshal.GetArrayDataReference(array);
		_ = BitConverter.IsLittleEndian;
		SpanHelpers.Memmove(ref arrayDataReference, ref src, num);
	}

	private unsafe static ref byte GetSpanDataFrom(RuntimeFieldHandle fldHandle, RuntimeTypeHandle targetTypeHandle, out int count)
	{
		if (fldHandle.IsNullHandle())
		{
			throw new ArgumentException(SR.Argument_InvalidHandle);
		}
		IRuntimeFieldInfo runtimeFieldInfo = fldHandle.GetRuntimeFieldInfo();
		if (!RuntimeFieldHandle.GetRVAFieldInfo(runtimeFieldInfo.Value, out var address, out var size))
		{
			throw new ArgumentException(SR.Argument_BadFieldForInitializeArray);
		}
		MethodTable* intPtr = targetTypeHandle.GetRuntimeType().GetNativeTypeHandle().AsMethodTable();
		if (!intPtr->IsPrimitive)
		{
			throw new ArgumentException(SR.Argument_BadArrayForInitializeArray);
		}
		uint numInstanceFieldBytes = intPtr->GetNumInstanceFieldBytes();
		if (((nuint)address & (nuint)(numInstanceFieldBytes - 1)) != 0)
		{
			throw new ArgumentException(SR.Argument_BadFieldForInitializeArray);
		}
		if (!BitConverter.IsLittleEndian)
		{
			throw new PlatformNotSupportedException();
		}
		count = (int)(size / numInstanceFieldBytes);
		ref byte result = ref *(byte*)address;
		GC.KeepAlive(runtimeFieldInfo);
		return ref result;
	}

	[return: NotNullIfNotNull("obj")]
	public unsafe static object? GetObjectValue(object? obj)
	{
		if (obj == null)
		{
			return null;
		}
		MethodTable* methodTable = GetMethodTable(obj);
		if (!methodTable->IsValueType || methodTable->IsPrimitive)
		{
			return obj;
		}
		return obj.MemberwiseClone();
	}

	[DllImport("QCall", EntryPoint = "ReflectionInvocation_RunClassConstructor", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ReflectionInvocation_RunClassConstructor")]
	private static extern void RunClassConstructor(QCallTypeHandle type);

	[RequiresUnreferencedCode("Trimmer can't guarantee existence of class constructor")]
	public static void RunClassConstructor(RuntimeTypeHandle type)
	{
		RuntimeType type2 = type.GetRuntimeType() ?? throw new ArgumentException(SR.InvalidOperation_HandleIsNotInitialized, "type");
		RunClassConstructor(new QCallTypeHandle(ref type2));
	}

	[DllImport("QCall", EntryPoint = "ReflectionInvocation_RunModuleConstructor", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ReflectionInvocation_RunModuleConstructor")]
	private static extern void RunModuleConstructor(QCallModule module);

	public static void RunModuleConstructor(ModuleHandle module)
	{
		RuntimeModule module2 = module.GetRuntimeModule() ?? throw new ArgumentException(SR.InvalidOperation_HandleIsNotInitialized, "module");
		RunModuleConstructor(new QCallModule(ref module2));
	}

	[DllImport("QCall", EntryPoint = "ReflectionInvocation_CompileMethod", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ReflectionInvocation_CompileMethod")]
	internal static extern void CompileMethod(RuntimeMethodHandleInternal method);

	[DllImport("QCall", EntryPoint = "ReflectionInvocation_PrepareMethod", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ReflectionInvocation_PrepareMethod")]
	private unsafe static extern void PrepareMethod(RuntimeMethodHandleInternal method, nint* pInstantiation, int cInstantiation);

	public static void PrepareMethod(RuntimeMethodHandle method)
	{
		PrepareMethod(method, null);
	}

	public unsafe static void PrepareMethod(RuntimeMethodHandle method, RuntimeTypeHandle[]? instantiation)
	{
		IRuntimeMethodInfo runtimeMethodInfo = method.GetMethodInfo() ?? throw new ArgumentException(SR.InvalidOperation_HandleIsNotInitialized, "method");
		instantiation = (RuntimeTypeHandle[])instantiation?.Clone();
		RuntimeTypeHandle[] inHandles = instantiation;
		Span<nint> stackScratch = new Span<nint>(stackalloc byte[(int)checked((nuint)8u * (nuint)8u)], 8);
		ReadOnlySpan<nint> readOnlySpan = RuntimeTypeHandle.CopyRuntimeTypeHandles(inHandles, stackScratch);
		fixed (nint* pInstantiation = readOnlySpan)
		{
			PrepareMethod(runtimeMethodInfo.Value, pInstantiation, readOnlySpan.Length);
			GC.KeepAlive(instantiation);
			GC.KeepAlive(runtimeMethodInfo);
		}
	}

	[DllImport("QCall", EntryPoint = "ReflectionInvocation_PrepareDelegate", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ReflectionInvocation_PrepareDelegate")]
	private static extern void PrepareDelegate(ObjectHandleOnStack d);

	public static void PrepareDelegate(Delegate d)
	{
		if ((object)d != null)
		{
			PrepareDelegate(ObjectHandleOnStack.Create(ref d));
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern int TryGetHashCode(object o);

	[DllImport("QCall", EntryPoint = "ObjectNative_GetHashCodeSlow", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ObjectNative_GetHashCodeSlow")]
	private static extern int GetHashCodeSlow(ObjectHandleOnStack o);

	public static int GetHashCode(object? o)
	{
		int num = TryGetHashCode(o);
		if (num == 0)
		{
			return GetHashCodeWorker(o);
		}
		return num;
		[MethodImpl(MethodImplOptions.NoInlining)]
		static int GetHashCodeWorker(object o2)
		{
			if (o2 == null)
			{
				return 0;
			}
			return GetHashCodeSlow(ObjectHandleOnStack.Create(ref o2));
		}
	}

	public new unsafe static bool Equals(object? o1, object? o2)
	{
		if (o1 == o2)
		{
			return true;
		}
		if (o1 == null || o2 == null)
		{
			return false;
		}
		MethodTable* methodTable = GetMethodTable(o1);
		if (!methodTable->IsValueType)
		{
			return false;
		}
		if (methodTable != GetMethodTable(o2))
		{
			return false;
		}
		return ContentEquals(o1, o2);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool ContentEquals(object o1, object o2);

	public static void EnsureSufficientExecutionStack()
	{
		if (!TryEnsureSufficientExecutionStack())
		{
			throw new InsufficientExecutionStackException();
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool TryEnsureSufficientExecutionStack();

	public static object GetUninitializedObject([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type type)
	{
		RuntimeType obj = type as RuntimeType;
		if ((object)obj == null)
		{
			ArgumentNullException.ThrowIfNull(type, "type");
			throw new SerializationException(SR.Format(SR.Serialization_InvalidType, type));
		}
		return obj.GetUninitializedObject();
	}

	[DllImport("QCall", EntryPoint = "ObjectNative_AllocateUninitializedClone", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ObjectNative_AllocateUninitializedClone")]
	internal static extern void AllocateUninitializedClone(ObjectHandleOnStack objHandle);

	[Intrinsic]
	internal static bool IsBitwiseEquatable<T>()
	{
		throw new InvalidOperationException();
	}

	[Intrinsic]
	internal static bool EnumEquals<T>(T x, T y) where T : struct, Enum
	{
		return x.Equals(y);
	}

	[Intrinsic]
	internal static int EnumCompareTo<T>(T x, T y) where T : struct, Enum
	{
		return x.CompareTo(y);
	}

	[Intrinsic]
	internal unsafe static void CopyConstruct<T>(T* dest, T* src) where T : unmanaged
	{
		throw new InvalidOperationException();
	}

	[DebuggerHidden]
	[DebuggerStepThrough]
	internal static ref byte GetRawData(this object obj)
	{
		return ref Unsafe.As<RawData>(obj).Data;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static nuint GetRawObjectDataSize(object obj)
	{
		MethodTable* methodTable = GetMethodTable(obj);
		nuint num = (nuint)methodTable->BaseSize - (nuint)16u;
		if (methodTable->HasComponentSize)
		{
			num += (nuint)((nint)Unsafe.As<RawArrayData>(obj).Length * (nint)methodTable->ComponentSize);
		}
		GC.KeepAlive(obj);
		return num;
	}

	internal unsafe static ushort GetElementSize(this Array array)
	{
		return GetMethodTable(array)->ComponentSize;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ref int GetMultiDimensionalArrayBounds(this Array array)
	{
		return ref Unsafe.As<byte, int>(ref Unsafe.As<RawArrayData>(array).Data);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static int GetMultiDimensionalArrayRank(this Array array)
	{
		int multiDimensionalArrayRank = GetMethodTable(array)->MultiDimensionalArrayRank;
		GC.KeepAlive(array);
		return multiDimensionalArrayRank;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static bool ObjectHasComponentSize(object obj)
	{
		return GetMethodTable(obj)->HasComponentSize;
	}

	internal unsafe static object Box(MethodTable* methodTable, ref byte data)
	{
		if (!methodTable->IsNullable)
		{
			return CastHelpers.Box(methodTable, ref data);
		}
		return CastHelpers.Box_Nullable(methodTable, ref data);
	}

	[Intrinsic]
	internal unsafe static MethodTable* GetMethodTable(object obj)
	{
		return GetMethodTable(obj);
	}

	[LibraryImport("QCall", EntryPoint = "MethodTable_AreTypesEquivalent")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool AreTypesEquivalent(MethodTable* pMTa, MethodTable* pMTb)
	{
		return __PInvoke(pMTa, pMTb) != 0;
		[DllImport("QCall", EntryPoint = "MethodTable_AreTypesEquivalent", ExactSpelling = true)]
		unsafe static extern int __PInvoke(MethodTable* __pMTa_native, MethodTable* __pMTb_native);
	}

	public static nint AllocateTypeAssociatedMemory(Type type, int size)
	{
		RuntimeType type2 = type as RuntimeType;
		if ((object)type2 == null)
		{
			throw new ArgumentException(SR.Arg_MustBeType, "type");
		}
		ArgumentOutOfRangeException.ThrowIfNegative(size, "size");
		return AllocateTypeAssociatedMemory(new QCallTypeHandle(ref type2), (uint)size);
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_AllocateTypeAssociatedMemory", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_AllocateTypeAssociatedMemory")]
	private static extern nint AllocateTypeAssociatedMemory(QCallTypeHandle type, uint size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern TailCallArgBuffer* GetTailCallArgBuffer();

	[DllImport("QCall", EntryPoint = "TailCallHelp_AllocTailCallArgBufferInternal", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "TailCallHelp_AllocTailCallArgBufferInternal")]
	private unsafe static extern TailCallArgBuffer* AllocTailCallArgBufferInternal(int size);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static TailCallArgBuffer* AllocTailCallArgBuffer(int size, nint gcDesc)
	{
		TailCallArgBuffer* ptr = GetTailCallArgBuffer();
		if (ptr != null && ptr->Size >= size)
		{
			ptr->State = 2;
		}
		else
		{
			ptr = AllocTailCallArgBufferWorker(size);
		}
		ptr->GCDesc = gcDesc;
		new Span<byte>(ptr + 1, size - sizeof(TailCallArgBuffer)).Clear();
		ptr->State = 0;
		return ptr;
		[MethodImpl(MethodImplOptions.NoInlining)]
		unsafe static TailCallArgBuffer* AllocTailCallArgBufferWorker(int size2)
		{
			return AllocTailCallArgBufferInternal(size2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern TailCallTls* GetTailCallInfo(nint retAddrSlot, nint* retAddr);

	[StackTraceHidden]
	private unsafe static void DispatchTailCalls(nint callersRetAddrSlot, delegate*<TailCallArgBuffer*, ref byte, PortableTailCallFrame*, void> callTarget, ref byte retVal)
	{
		Unsafe.SkipInit(out nint num);
		TailCallTls* tailCallInfo = GetTailCallInfo(callersRetAddrSlot, &num);
		PortableTailCallFrame* frame = tailCallInfo->Frame;
		if (num == frame->TailCallAwareReturnAddress)
		{
			frame->NextCall = callTarget;
			return;
		}
		Unsafe.SkipInit(out PortableTailCallFrame portableTailCallFrame);
		portableTailCallFrame.NextCall = null;
		try
		{
			tailCallInfo->Frame = &portableTailCallFrame;
			do
			{
				callTarget(tailCallInfo->ArgBuffer, ref retVal, &portableTailCallFrame);
				callTarget = portableTailCallFrame.NextCall;
			}
			while (callTarget != (delegate*<TailCallArgBuffer*, ref byte, PortableTailCallFrame*, void>)null);
		}
		finally
		{
			tailCallInfo->Frame = frame;
			tailCallInfo->ArgBuffer->State = 2;
		}
	}

	public static object? Box(ref byte target, RuntimeTypeHandle type)
	{
		if (type.IsNullHandle())
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.type);
		}
		return type.GetRuntimeType().Box(ref target);
	}

	[DllImport("QCall", EntryPoint = "ReflectionInvocation_SizeOf", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ReflectionInvocation_SizeOf")]
	[SuppressGCTransition]
	private static extern int SizeOf(QCallTypeHandle handle);

	public static int SizeOf(RuntimeTypeHandle type)
	{
		if (type.IsNullHandle())
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.type);
		}
		int num = SizeOf(new QCallTypeHandle(ref type));
		if (num <= 0)
		{
			throw new ArgumentException(SR.Arg_TypeNotSupported);
		}
		return num;
	}

	public static T[] GetSubArray<T>(T[] array, Range range)
	{
		if (array == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
		}
		var (elementOffset, num) = range.GetOffsetAndLength(array.Length);
		T[] array2;
		if (typeof(T[]) == array.GetType())
		{
			if (num == 0)
			{
				return Array.Empty<T>();
			}
			array2 = new T[num];
		}
		else
		{
			array2 = Unsafe.As<T[]>(Array.CreateInstanceFromArrayType(array.GetType(), num));
		}
		Buffer.Memmove(ref MemoryMarshal.GetArrayDataReference(array2), ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), elementOffset), (uint)num);
		return array2;
	}

	[Obsolete("The Constrained Execution Region (CER) feature is not supported.", DiagnosticId = "SYSLIB0004", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static void ExecuteCodeWithGuaranteedCleanup(TryCode code, CleanupCode backoutCode, object? userData)
	{
		ArgumentNullException.ThrowIfNull(code, "code");
		ArgumentNullException.ThrowIfNull(backoutCode, "backoutCode");
		bool exceptionThrown = true;
		try
		{
			code(userData);
			exceptionThrown = false;
		}
		finally
		{
			backoutCode(userData, exceptionThrown);
		}
	}

	[Obsolete("The Constrained Execution Region (CER) feature is not supported.", DiagnosticId = "SYSLIB0004", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static void PrepareContractedDelegate(Delegate d)
	{
	}

	[Obsolete("The Constrained Execution Region (CER) feature is not supported.", DiagnosticId = "SYSLIB0004", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static void ProbeForSufficientStack()
	{
	}

	[Obsolete("The Constrained Execution Region (CER) feature is not supported.", DiagnosticId = "SYSLIB0004", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static void PrepareConstrainedRegions()
	{
	}

	[Obsolete("The Constrained Execution Region (CER) feature is not supported.", DiagnosticId = "SYSLIB0004", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static void PrepareConstrainedRegionsNoOP()
	{
	}

	internal static bool IsPrimitiveType(this CorElementType et)
	{
		return ((1 << (int)et) & 0x3003FFC) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool CanPrimitiveWiden(CorElementType srcET, CorElementType dstET)
	{
		ReadOnlySpan<short> readOnlySpan = new short[14]
		{
			0, 0, 4, 16264, 13648, 16360, 13632, 16264, 13568, 15872,
			13312, 14336, 12288, 8192
		};
		if ((int)srcET >= readOnlySpan.Length)
		{
			return srcET == dstET;
		}
		return (readOnlySpan[(int)srcET] & (1 << (int)dstET)) != 0;
	}

	[Intrinsic]
	public static ReadOnlySpan<T> CreateSpan<T>(RuntimeFieldHandle fldHandle)
	{
		int count;
		return new ReadOnlySpan<T>(ref Unsafe.As<byte, T>(ref GetSpanDataFrom(fldHandle, typeof(T).TypeHandle, out count)), count);
	}

	[Intrinsic]
	internal static bool IsKnownConstant(Type t)
	{
		return false;
	}

	[Intrinsic]
	internal static bool IsKnownConstant(string t)
	{
		return false;
	}

	[Intrinsic]
	internal static bool IsKnownConstant(char t)
	{
		return false;
	}

	[Intrinsic]
	internal static bool IsKnownConstant<T>(T t) where T : struct
	{
		return false;
	}

	[Intrinsic]
	public static bool IsReferenceOrContainsReferences<T>() where T : allows ref struct
	{
		return IsReferenceOrContainsReferences<T>();
	}
}
