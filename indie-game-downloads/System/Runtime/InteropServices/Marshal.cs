using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;
using System.Security;
using System.StubHelpers;
using System.Text;

namespace System.Runtime.InteropServices;

public static class Marshal
{
	internal static readonly Guid IID_IUnknown = new Guid(0, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

	internal static readonly Guid IID_IDispatch = new Guid(132096, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

	public static readonly int SystemDefaultCharSize = 2;

	public static readonly int SystemMaxDBCSCharSize = GetSystemMaxDBCSCharSize();

	[FeatureSwitchDefinition("System.Runtime.InteropServices.BuiltInComInterop.IsSupported")]
	internal static bool IsBuiltInComSupported { get; } = IsBuiltInComSupportedInternal();

	internal static int SizeOfHelper(RuntimeType t, [MarshalAs(UnmanagedType.Bool)] bool throwIfNotMarshalable)
	{
		return SizeOfHelper(new QCallTypeHandle(ref t), throwIfNotMarshalable);
	}

	[LibraryImport("QCall", EntryPoint = "MarshalNative_SizeOfHelper")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private static int SizeOfHelper(QCallTypeHandle t, [MarshalAs(UnmanagedType.Bool)] bool throwIfNotMarshalable)
	{
		int _throwIfNotMarshalable_native = (throwIfNotMarshalable ? 1 : 0);
		return __PInvoke(t, _throwIfNotMarshalable_native);
		[DllImport("QCall", EntryPoint = "MarshalNative_SizeOfHelper", ExactSpelling = true)]
		static extern int __PInvoke(QCallTypeHandle __t_native, int __throwIfNotMarshalable_native);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070:UnrecognizedReflectionPattern", Justification = "Trimming doesn't affect types eligible for marshalling. Different exception for invalid inputs doesn't matter.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static nint OffsetOf(Type t, string fieldName)
	{
		ArgumentNullException.ThrowIfNull(t, "t");
		RtFieldInfo obj = ((t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? throw new ArgumentException(SR.Format(SR.Argument_OffsetOfFieldNotFound, t.FullName), "fieldName")) as RtFieldInfo) ?? throw new ArgumentException(SR.Argument_MustBeRuntimeFieldInfo, "fieldName");
		nint result = OffsetOf(obj.GetFieldDesc());
		GC.KeepAlive(obj);
		return result;
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_OffsetOf", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_OffsetOf")]
	private static extern nint OffsetOf(nint pFD);

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ReadByte(Object, Int32) may be unavailable in future releases.")]
	[RequiresDynamicCode("Marshalling code for the object might not be available")]
	public static byte ReadByte(object ptr, int ofs)
	{
		return ReadValueSlow(ptr, ofs, ReadByte);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ReadInt16(Object, Int32) may be unavailable in future releases.")]
	[RequiresDynamicCode("Marshalling code for the object might not be available")]
	public static short ReadInt16(object ptr, int ofs)
	{
		return ReadValueSlow(ptr, ofs, ReadInt16);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ReadInt32(Object, Int32) may be unavailable in future releases.")]
	[RequiresDynamicCode("Marshalling code for the object might not be available")]
	public static int ReadInt32(object ptr, int ofs)
	{
		return ReadValueSlow(ptr, ofs, ReadInt32);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ReadInt64(Object, Int32) may be unavailable in future releases.")]
	[RequiresDynamicCode("Marshalling code for the object might not be available")]
	public static long ReadInt64([In][MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs)
	{
		return ReadValueSlow(ptr, ofs, ReadInt64);
	}

	private unsafe static T ReadValueSlow<T>(object ptr, int ofs, Func<nint, int, T> readValueHelper)
	{
		if (ptr == null)
		{
			throw new AccessViolationException();
		}
		MngdNativeArrayMarshaler.MarshalerState marshalerState = default(MngdNativeArrayMarshaler.MarshalerState);
		AsAnyMarshaler asAnyMarshaler = new AsAnyMarshaler(new IntPtr(&marshalerState));
		nint num = IntPtr.Zero;
		try
		{
			num = asAnyMarshaler.ConvertToNative(ptr, 285147391);
			return readValueHelper(num, ofs);
		}
		finally
		{
			asAnyMarshaler.ClearNative(num);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("WriteByte(Object, Int32, Byte) may be unavailable in future releases.")]
	[RequiresDynamicCode("Marshalling code for the object might not be available")]
	public static void WriteByte(object ptr, int ofs, byte val)
	{
		WriteValueSlow(ptr, ofs, val, WriteByte);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("WriteInt16(Object, Int32, Int16) may be unavailable in future releases.")]
	[RequiresDynamicCode("Marshalling code for the object might not be available")]
	public static void WriteInt16(object ptr, int ofs, short val)
	{
		WriteValueSlow(ptr, ofs, val, WriteInt16);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("WriteInt32(Object, Int32, Int32) may be unavailable in future releases.")]
	[RequiresDynamicCode("Marshalling code for the object might not be available")]
	public static void WriteInt32(object ptr, int ofs, int val)
	{
		WriteValueSlow(ptr, ofs, val, WriteInt32);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("WriteInt64(Object, Int32, Int64) may be unavailable in future releases.")]
	[RequiresDynamicCode("Marshalling code for the object might not be available")]
	public static void WriteInt64(object ptr, int ofs, long val)
	{
		WriteValueSlow(ptr, ofs, val, WriteInt64);
	}

	private unsafe static void WriteValueSlow<T>(object ptr, int ofs, T val, Action<nint, int, T> writeValueHelper)
	{
		if (ptr == null)
		{
			throw new AccessViolationException();
		}
		MngdNativeArrayMarshaler.MarshalerState marshalerState = default(MngdNativeArrayMarshaler.MarshalerState);
		AsAnyMarshaler asAnyMarshaler = new AsAnyMarshaler(new IntPtr(&marshalerState));
		nint num = IntPtr.Zero;
		try
		{
			num = asAnyMarshaler.ConvertToNative(ptr, 822018303);
			writeValueHelper(num, ofs, val);
			asAnyMarshaler.ConvertToManaged(ptr, num);
		}
		finally
		{
			asAnyMarshaler.ClearNative(num);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern int GetLastPInvokeError();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void SetLastPInvokeError(int error);

	private static void PrelinkCore(MethodInfo m)
	{
		RuntimeMethodInfo obj = (m as RuntimeMethodInfo) ?? throw new ArgumentException(SR.Argument_MustBeRuntimeMethodInfo, "m");
		InternalPrelink(((IRuntimeMethodInfo)obj).Value);
		GC.KeepAlive(obj);
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_Prelink", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_Prelink")]
	private static extern void InternalPrelink(RuntimeMethodHandleInternal m);

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern nint GetExceptionPointers();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("GetExceptionCode() may be unavailable in future releases.")]
	public static extern int GetExceptionCode();

	[RequiresDynamicCode("Marshalling code for the object might not be available. Use the StructureToPtr<T> overload instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public unsafe static void StructureToPtr(object structure, nint ptr, bool fDeleteOld)
	{
		ArgumentNullException.ThrowIfNull(ptr, "ptr");
		ArgumentNullException.ThrowIfNull(structure, "structure");
		MethodTable* methodTable = RuntimeHelpers.GetMethodTable(structure);
		if (methodTable->HasInstantiation)
		{
			throw new ArgumentException(SR.Argument_NeedNonGenericObject, "structure");
		}
		Unsafe.SkipInit(out delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void> obj);
		Unsafe.SkipInit(out nuint len);
		if (!TryGetStructMarshalStub((nint)methodTable, &obj, &len))
		{
			throw new ArgumentException(SR.Argument_MustHaveLayoutOrBeBlittable, "structure");
		}
		if (obj != (delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void>)null)
		{
			if (fDeleteOld)
			{
				obj(ref structure.GetRawData(), (byte*)ptr, 2, ref Unsafe.NullRef<CleanupWorkListElement>());
			}
			obj(ref structure.GetRawData(), (byte*)ptr, 0, ref Unsafe.NullRef<CleanupWorkListElement>());
		}
		else
		{
			SpanHelpers.Memmove(ref *(byte*)ptr, ref structure.GetRawData(), len);
		}
	}

	private unsafe static void PtrToStructureHelper(nint ptr, object structure, bool allowValueClasses)
	{
		MethodTable* methodTable = RuntimeHelpers.GetMethodTable(structure);
		if (!allowValueClasses && methodTable->IsValueType)
		{
			throw new ArgumentException(SR.Argument_StructMustNotBeValueClass, "structure");
		}
		Unsafe.SkipInit(out delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void> obj);
		Unsafe.SkipInit(out nuint len);
		if (!TryGetStructMarshalStub((nint)methodTable, &obj, &len))
		{
			throw new ArgumentException(SR.Argument_MustHaveLayoutOrBeBlittable, "structure");
		}
		if (obj != (delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void>)null)
		{
			obj(ref structure.GetRawData(), (byte*)ptr, 1, ref Unsafe.NullRef<CleanupWorkListElement>());
		}
		else
		{
			SpanHelpers.Memmove(ref structure.GetRawData(), ref *(byte*)ptr, len);
		}
	}

	[RequiresDynamicCode("Marshalling code for the object might not be available. Use the DestroyStructure<T> overload instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public unsafe static void DestroyStructure(nint ptr, Type structuretype)
	{
		ArgumentNullException.ThrowIfNull(ptr, "ptr");
		ArgumentNullException.ThrowIfNull(structuretype, "structuretype");
		RuntimeType obj = (structuretype as RuntimeType) ?? throw new ArgumentException(SR.Argument_MustBeRuntimeType, "structuretype");
		if (obj.IsGenericType)
		{
			throw new ArgumentException(SR.Argument_NeedNonGenericType, "structuretype");
		}
		Unsafe.SkipInit(out delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void> obj2);
		Unsafe.SkipInit(out nuint num);
		if (!TryGetStructMarshalStub(obj.GetUnderlyingNativeHandle(), &obj2, &num))
		{
			throw new ArgumentException(SR.Argument_MustHaveLayoutOrBeBlittable, "structuretype");
		}
		GC.KeepAlive(obj);
		if (obj2 != (delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void>)null)
		{
			obj2(ref Unsafe.NullRef<byte>(), (byte*)ptr, 2, ref Unsafe.NullRef<CleanupWorkListElement>());
		}
	}

	[LibraryImport("QCall", EntryPoint = "MarshalNative_TryGetStructMarshalStub")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool TryGetStructMarshalStub(nint th, delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void>* structMarshalStub, nuint* size)
	{
		return __PInvoke(th, structMarshalStub, size) != 0;
		[DllImport("QCall", EntryPoint = "MarshalNative_TryGetStructMarshalStub", ExactSpelling = true)]
		unsafe static extern int __PInvoke(nint __th_native, delegate*<ref byte, byte*, int, ref CleanupWorkListElement, void>* __structMarshalStub_native, nuint* __size_native);
	}

	internal unsafe static bool IsPinnable(object obj)
	{
		if (obj != null)
		{
			return !RuntimeHelpers.GetMethodTable(obj)->ContainsGCPointers;
		}
		return true;
	}

	[LibraryImport("QCall", EntryPoint = "MarshalNative_IsBuiltInComSupported")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static bool IsBuiltInComSupportedInternal()
	{
		return __PInvoke() != 0;
		[DllImport("QCall", EntryPoint = "MarshalNative_IsBuiltInComSupported", ExactSpelling = true)]
		static extern int __PInvoke();
	}

	[RequiresAssemblyFiles("Windows only assigns HINSTANCE to assemblies loaded from disk. This API will return -1 for modules without a file on disk.")]
	public static nint GetHINSTANCE(Module m)
	{
		ArgumentNullException.ThrowIfNull(m, "m");
		RuntimeModule module = m as RuntimeModule;
		if ((object)module != null)
		{
			return GetHINSTANCE(new QCallModule(ref module));
		}
		return -1;
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetHINSTANCE", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetHINSTANCE")]
	private static extern nint GetHINSTANCE(QCallModule m);

	internal static Exception GetExceptionForHRInternal(int errorCode, nint errorInfo)
	{
		Exception o = null;
		GetExceptionForHRInternal(errorCode, errorInfo, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetExceptionForHR", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetExceptionForHR")]
	private static extern void GetExceptionForHRInternal(int errorCode, nint errorInfo, ObjectHandleOnStack exception);

	public static int GetHRForException(Exception? e)
	{
		return GetHRForException(ObjectHandleOnStack.Create(ref e));
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetHRForException", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetHRForException")]
	private static extern int GetHRForException(ObjectHandleOnStack exception);

	[SupportedOSPlatform("windows")]
	public static string GetTypeInfoName(ITypeInfo typeInfo)
	{
		ArgumentNullException.ThrowIfNull(typeInfo, "typeInfo");
		typeInfo.GetDocumentation(-1, out string strName, out string _, out int _, out string _);
		return strName;
	}

	internal static Type GetTypeFromCLSID(Guid clsid, string server, bool throwOnError)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		Type o = null;
		GetTypeFromCLSID(in clsid, server, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetTypeFromCLSID", StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static void GetTypeFromCLSID(in Guid clsid, string server, ObjectHandleOnStack retType)
	{
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(server))
		{
			void* _server_native = ptr;
			fixed (Guid* _clsid_native = &clsid)
			{
				__PInvoke(_clsid_native, (ushort*)_server_native, retType);
			}
		}
		[DllImport("QCall", EntryPoint = "MarshalNative_GetTypeFromCLSID", ExactSpelling = true)]
		unsafe static extern void __PInvoke(Guid* __clsid_native, ushort* __server_native, ObjectHandleOnStack __retType_native);
	}

	[SupportedOSPlatform("windows")]
	public static nint GetIUnknownForObject(object o)
	{
		ArgumentNullException.ThrowIfNull(o, "o");
		return GetIUnknownForObject(ObjectHandleOnStack.Create(ref o));
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetIUnknownForObject", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetIUnknownForObject")]
	private static extern nint GetIUnknownForObject(ObjectHandleOnStack o);

	[SupportedOSPlatform("windows")]
	public static nint GetIDispatchForObject(object o)
	{
		ArgumentNullException.ThrowIfNull(o, "o");
		return GetIDispatchForObject(ObjectHandleOnStack.Create(ref o));
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetIDispatchForObject", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetIDispatchForObject")]
	private static extern nint GetIDispatchForObject(ObjectHandleOnStack o);

	[SupportedOSPlatform("windows")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static nint GetComInterfaceForObject(object o, Type T)
	{
		return GetComInterfaceForObject(o, T, CustomQueryInterfaceMode.Allow);
	}

	[SupportedOSPlatform("windows")]
	public static nint GetComInterfaceForObject<T, TInterface>([DisallowNull] T o)
	{
		return GetComInterfaceForObject(o, typeof(TInterface), CustomQueryInterfaceMode.Allow);
	}

	[SupportedOSPlatform("windows")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static nint GetComInterfaceForObject(object o, Type T, CustomQueryInterfaceMode mode)
	{
		ArgumentNullException.ThrowIfNull(o, "o");
		ArgumentNullException.ThrowIfNull(T, "T");
		RuntimeType type = T as RuntimeType;
		if ((object)type == null)
		{
			throw new ArgumentException(SR.Argument_MustBeRuntimeType, "T");
		}
		return GetComInterfaceForObject(ObjectHandleOnStack.Create(ref o), new QCallTypeHandle(ref type), mode == CustomQueryInterfaceMode.Allow);
	}

	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetComInterfaceForObject")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private static nint GetComInterfaceForObject(ObjectHandleOnStack o, QCallTypeHandle t, [MarshalAs(UnmanagedType.Bool)] bool fEnableCustomizedQueryInterface)
	{
		int _fEnableCustomizedQueryInterface_native = (fEnableCustomizedQueryInterface ? 1 : 0);
		return __PInvoke(o, t, _fEnableCustomizedQueryInterface_native);
		[DllImport("QCall", EntryPoint = "MarshalNative_GetComInterfaceForObject", ExactSpelling = true)]
		static extern nint __PInvoke(ObjectHandleOnStack __o_native, QCallTypeHandle __t_native, int __fEnableCustomizedQueryInterface_native);
	}

	[SupportedOSPlatform("windows")]
	public static object GetObjectForIUnknown(nint pUnk)
	{
		ArgumentNullException.ThrowIfNull(pUnk, "pUnk");
		object o = null;
		GetObjectForIUnknown(pUnk, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetObjectForIUnknown", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetObjectForIUnknown")]
	private static extern void GetObjectForIUnknown(nint pUnk, ObjectHandleOnStack retObject);

	[SupportedOSPlatform("windows")]
	public static object GetUniqueObjectForIUnknown(nint unknown)
	{
		ArgumentNullException.ThrowIfNull(unknown, "unknown");
		object o = null;
		GetUniqueObjectForIUnknown(unknown, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetUniqueObjectForIUnknown", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetUniqueObjectForIUnknown")]
	private static extern void GetUniqueObjectForIUnknown(nint unknown, ObjectHandleOnStack retObject);

	[SupportedOSPlatform("windows")]
	public static object GetTypedObjectForIUnknown(nint pUnk, Type t)
	{
		ArgumentNullException.ThrowIfNull(pUnk, "pUnk");
		ArgumentNullException.ThrowIfNull(t, "t");
		RuntimeType type = t as RuntimeType;
		if ((object)type == null)
		{
			throw new ArgumentException(SR.Argument_MustBeRuntimeType, "t");
		}
		object o = null;
		GetTypedObjectForIUnknown(pUnk, new QCallTypeHandle(ref type), ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetTypedObjectForIUnknown", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetTypedObjectForIUnknown")]
	private static extern void GetTypedObjectForIUnknown(nint pUnk, QCallTypeHandle t, ObjectHandleOnStack retObject);

	[SupportedOSPlatform("windows")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static nint CreateAggregatedObject(nint pOuter, object o)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		ArgumentNullException.ThrowIfNull(pOuter, "pOuter");
		ArgumentNullException.ThrowIfNull(o, "o");
		return CreateAggregatedObject(pOuter, ObjectHandleOnStack.Create(ref o));
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_CreateAggregatedObject", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_CreateAggregatedObject")]
	private static extern nint CreateAggregatedObject(nint pOuter, ObjectHandleOnStack o);

	[SupportedOSPlatform("windows")]
	public static nint CreateAggregatedObject<T>(nint pOuter, T o) where T : notnull
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		return CreateAggregatedObject(pOuter, (object)o);
	}

	public static void CleanupUnusedObjectsInCurrentContext()
	{
		InternalCleanupUnusedObjectsInCurrentContext();
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_CleanupUnusedObjectsInCurrentContext", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_CleanupUnusedObjectsInCurrentContext")]
	private static extern void InternalCleanupUnusedObjectsInCurrentContext();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern bool AreComObjectsAvailableForCleanup();

	public static bool IsComObject(object o)
	{
		ArgumentNullException.ThrowIfNull(o, "o");
		return o is __ComObject;
	}

	[SupportedOSPlatform("windows")]
	public static int ReleaseComObject(object o)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		if (o == null)
		{
			throw new NullReferenceException();
		}
		__ComObject o2 = o as __ComObject;
		if (o2 == null)
		{
			throw new ArgumentException(SR.Argument_ObjNotComObject, "o");
		}
		return ReleaseComObject(ObjectHandleOnStack.Create(ref o2));
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_ReleaseComObject", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_ReleaseComObject")]
	private static extern int ReleaseComObject(ObjectHandleOnStack o);

	[SupportedOSPlatform("windows")]
	public static int FinalReleaseComObject(object o)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		ArgumentNullException.ThrowIfNull(o, "o");
		__ComObject o2 = o as __ComObject;
		if (o2 == null)
		{
			throw new ArgumentException(SR.Argument_ObjNotComObject, "o");
		}
		FinalReleaseComObject(ObjectHandleOnStack.Create(ref o2));
		return 0;
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_FinalReleaseComObject", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_FinalReleaseComObject")]
	private static extern void FinalReleaseComObject(ObjectHandleOnStack o);

	[SupportedOSPlatform("windows")]
	public static object? GetComObjectData(object obj, object key)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		ArgumentNullException.ThrowIfNull(obj, "obj");
		ArgumentNullException.ThrowIfNull(key, "key");
		return ((obj as __ComObject) ?? throw new ArgumentException(SR.Argument_ObjNotComObject, "obj")).GetData(key);
	}

	[SupportedOSPlatform("windows")]
	public static bool SetComObjectData(object obj, object key, object? data)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		ArgumentNullException.ThrowIfNull(obj, "obj");
		ArgumentNullException.ThrowIfNull(key, "key");
		return ((obj as __ComObject) ?? throw new ArgumentException(SR.Argument_ObjNotComObject, "obj")).SetData(key, data);
	}

	[SupportedOSPlatform("windows")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[return: NotNullIfNotNull("o")]
	public static object? CreateWrapperOfType(object? o, Type t)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		ArgumentNullException.ThrowIfNull(t, "t");
		if (!t.IsCOMObject)
		{
			throw new ArgumentException(SR.Argument_TypeNotComObject, "t");
		}
		if (t.IsGenericType)
		{
			throw new ArgumentException(SR.Argument_NeedNonGenericType, "t");
		}
		if (o == null)
		{
			return null;
		}
		if (!o.GetType().IsCOMObject)
		{
			throw new ArgumentException(SR.Argument_ObjNotComObject, "o");
		}
		if (o.GetType() == t)
		{
			return o;
		}
		object obj = GetComObjectData(o, t);
		if (obj == null)
		{
			obj = InternalCreateWrapperOfType(o, t);
			if (!SetComObjectData(o, t, obj))
			{
				obj = GetComObjectData(o, t);
			}
		}
		return obj;
	}

	[SupportedOSPlatform("windows")]
	public static TWrapper CreateWrapperOfType<T, TWrapper>(T? o)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		return (TWrapper)CreateWrapperOfType(o, typeof(TWrapper));
	}

	private static object InternalCreateWrapperOfType(object o, Type t)
	{
		RuntimeType type = t as RuntimeType;
		if ((object)type == null)
		{
			throw new ArgumentException(SR.Argument_MustBeRuntimeType, "t");
		}
		object o2 = null;
		InternalCreateWrapperOfType(ObjectHandleOnStack.Create(ref o), new QCallTypeHandle(ref type), ObjectHandleOnStack.Create(ref o2));
		return o2;
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_InternalCreateWrapperOfType", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_InternalCreateWrapperOfType")]
	private static extern void InternalCreateWrapperOfType(ObjectHandleOnStack o, QCallTypeHandle rt, ObjectHandleOnStack retObject);

	public static bool IsTypeVisibleFromCom(Type t)
	{
		ArgumentNullException.ThrowIfNull(t, "t");
		RuntimeType type = t as RuntimeType;
		if ((object)type == null)
		{
			throw new ArgumentException(SR.Argument_MustBeRuntimeType, "t");
		}
		return IsTypeVisibleFromCom(new QCallTypeHandle(ref type));
	}

	[LibraryImport("QCall", EntryPoint = "MarshalNative_IsTypeVisibleFromCom")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static bool IsTypeVisibleFromCom(QCallTypeHandle rt)
	{
		return __PInvoke(rt) != 0;
		[DllImport("QCall", EntryPoint = "MarshalNative_IsTypeVisibleFromCom", ExactSpelling = true)]
		static extern int __PInvoke(QCallTypeHandle __rt_native);
	}

	[SupportedOSPlatform("windows")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void GetNativeVariantForObject(object? obj, nint pDstNativeVariant)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		ArgumentNullException.ThrowIfNull(pDstNativeVariant, "pDstNativeVariant");
		GetNativeVariantForObject(ObjectHandleOnStack.Create(ref obj), pDstNativeVariant);
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetNativeVariantForObject", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetNativeVariantForObject")]
	private static extern void GetNativeVariantForObject(ObjectHandleOnStack obj, nint pDstNativeVariant);

	[SupportedOSPlatform("windows")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void GetNativeVariantForObject<T>(T? obj, nint pDstNativeVariant)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		GetNativeVariantForObject((object?)obj, pDstNativeVariant);
	}

	[SupportedOSPlatform("windows")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static object? GetObjectForNativeVariant(nint pSrcNativeVariant)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		ArgumentNullException.ThrowIfNull(pSrcNativeVariant, "pSrcNativeVariant");
		object o = null;
		GetObjectForNativeVariant(pSrcNativeVariant, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetObjectForNativeVariant", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetObjectForNativeVariant")]
	private static extern void GetObjectForNativeVariant(nint pSrcNativeVariant, ObjectHandleOnStack retObject);

	[SupportedOSPlatform("windows")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static T? GetObjectForNativeVariant<T>(nint pSrcNativeVariant)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		return (T)GetObjectForNativeVariant(pSrcNativeVariant);
	}

	[SupportedOSPlatform("windows")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static object?[] GetObjectsForNativeVariants(nint aSrcNativeVariant, int cVars)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		ArgumentNullException.ThrowIfNull(aSrcNativeVariant, "aSrcNativeVariant");
		ArgumentOutOfRangeException.ThrowIfNegative(cVars, "cVars");
		object[] o = null;
		GetObjectsForNativeVariants(aSrcNativeVariant, cVars, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetObjectsForNativeVariants", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetObjectsForNativeVariants")]
	private static extern void GetObjectsForNativeVariants(nint aSrcNativeVariant, int cVars, ObjectHandleOnStack retArray);

	[SupportedOSPlatform("windows")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static T[] GetObjectsForNativeVariants<T>(nint aSrcNativeVariant, int cVars)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		object[] objectsForNativeVariants = GetObjectsForNativeVariants(aSrcNativeVariant, cVars);
		T[] array = new T[objectsForNativeVariants.Length];
		Array.Copy(objectsForNativeVariants, array, objectsForNativeVariants.Length);
		return array;
	}

	[SupportedOSPlatform("windows")]
	public static int GetStartComSlot(Type t)
	{
		ArgumentNullException.ThrowIfNull(t, "t");
		RuntimeType type = t as RuntimeType;
		if ((object)type == null)
		{
			throw new ArgumentException(SR.Argument_MustBeRuntimeType, "t");
		}
		return GetStartComSlot(new QCallTypeHandle(ref type));
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetStartComSlot", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetStartComSlot")]
	private static extern int GetStartComSlot(QCallTypeHandle rt);

	[SupportedOSPlatform("windows")]
	public static int GetEndComSlot(Type t)
	{
		ArgumentNullException.ThrowIfNull(t, "t");
		RuntimeType type = t as RuntimeType;
		if ((object)type == null)
		{
			throw new ArgumentException(SR.Argument_MustBeRuntimeType, "t");
		}
		return GetEndComSlot(new QCallTypeHandle(ref type));
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetEndComSlot", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetEndComSlot")]
	private static extern int GetEndComSlot(QCallTypeHandle rt);

	[RequiresUnreferencedCode("Built-in COM support is not trim compatible", Url = "https://aka.ms/dotnet-illink/com")]
	[SupportedOSPlatform("windows")]
	public static object BindToMoniker(string monikerName)
	{
		if (!IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		ThrowExceptionForHR(CreateBindCtx(0u, out var ppbc));
		try
		{
			ThrowExceptionForHR(MkParseDisplayName(ppbc, monikerName, out var _, out var ppmk));
			try
			{
				ThrowExceptionForHR(BindMoniker(ppmk, 0u, in IID_IUnknown, out var ppvResult));
				try
				{
					return GetObjectForIUnknown(ppvResult);
				}
				finally
				{
					Release(ppvResult);
				}
			}
			finally
			{
				Release(ppmk);
			}
		}
		finally
		{
			Release(ppbc);
		}
	}

	[LibraryImport("ole32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static int CreateBindCtx(uint reserved, out nint ppbc)
	{
		ppbc = 0;
		int result;
		fixed (nint* _ppbc_native = &ppbc)
		{
			result = __PInvoke(reserved, _ppbc_native);
		}
		return result;
		[DllImport("ole32.dll", EntryPoint = "CreateBindCtx", ExactSpelling = true)]
		unsafe static extern int __PInvoke(uint __reserved_native, nint* __ppbc_native);
	}

	[LibraryImport("ole32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static int MkParseDisplayName(nint pbc, [MarshalAs(UnmanagedType.LPWStr)] string szUserName, out uint pchEaten, out nint ppmk)
	{
		pchEaten = 0u;
		ppmk = 0;
		int result;
		fixed (nint* _ppmk_native = &ppmk)
		{
			fixed (uint* _pchEaten_native = &pchEaten)
			{
				fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(szUserName))
				{
					void* _szUserName_native = ptr;
					result = __PInvoke(pbc, (ushort*)_szUserName_native, _pchEaten_native, _ppmk_native);
				}
			}
		}
		return result;
		[DllImport("ole32.dll", EntryPoint = "MkParseDisplayName", ExactSpelling = true)]
		unsafe static extern int __PInvoke(nint __pbc_native, ushort* __szUserName_native, uint* __pchEaten_native, nint* __ppmk_native);
	}

	[LibraryImport("ole32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static int BindMoniker(nint pmk, uint grfOpt, in Guid iidResult, out nint ppvResult)
	{
		ppvResult = 0;
		int result;
		fixed (nint* _ppvResult_native = &ppvResult)
		{
			fixed (Guid* _iidResult_native = &iidResult)
			{
				result = __PInvoke(pmk, grfOpt, _iidResult_native, _ppvResult_native);
			}
		}
		return result;
		[DllImport("ole32.dll", EntryPoint = "BindMoniker", ExactSpelling = true)]
		unsafe static extern int __PInvoke(nint __pmk_native, uint __grfOpt_native, Guid* __iidResult_native, nint* __ppvResult_native);
	}

	[SupportedOSPlatform("windows")]
	public static void ChangeWrapperHandleStrength(object otp, bool fIsWeak)
	{
		ArgumentNullException.ThrowIfNull(otp, "otp");
		ChangeWrapperHandleStrength(ObjectHandleOnStack.Create(ref otp), fIsWeak);
	}

	[LibraryImport("QCall", EntryPoint = "MarshalNative_ChangeWrapperHandleStrength")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private static void ChangeWrapperHandleStrength(ObjectHandleOnStack otp, [MarshalAs(UnmanagedType.Bool)] bool fIsWeak)
	{
		int _fIsWeak_native = (fIsWeak ? 1 : 0);
		__PInvoke(otp, _fIsWeak_native);
		[DllImport("QCall", EntryPoint = "MarshalNative_ChangeWrapperHandleStrength", ExactSpelling = true)]
		static extern void __PInvoke(ObjectHandleOnStack __otp_native, int __fIsWeak_native);
	}

	internal static Delegate GetDelegateForFunctionPointerInternal(nint ptr, RuntimeType t)
	{
		Delegate o = null;
		GetDelegateForFunctionPointerInternal(ptr, new QCallTypeHandle(ref t), ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetDelegateForFunctionPointerInternal", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetDelegateForFunctionPointerInternal")]
	private static extern void GetDelegateForFunctionPointerInternal(nint ptr, QCallTypeHandle t, ObjectHandleOnStack retDelegate);

	internal static nint GetFunctionPointerForDelegateInternal(Delegate d)
	{
		return GetFunctionPointerForDelegateInternal(ObjectHandleOnStack.Create(ref d));
	}

	[DllImport("QCall", EntryPoint = "MarshalNative_GetFunctionPointerForDelegateInternal", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "MarshalNative_GetFunctionPointerForDelegateInternal")]
	private static extern nint GetFunctionPointerForDelegateInternal(ObjectHandleOnStack d);

	public static nint AllocHGlobal(int cb)
	{
		return AllocHGlobal((nint)cb);
	}

	public unsafe static string? PtrToStringAnsi(nint ptr)
	{
		if (IsNullOrWin32Atom(ptr))
		{
			return null;
		}
		return new string((sbyte*)ptr);
	}

	public unsafe static string PtrToStringAnsi(nint ptr, int len)
	{
		ArgumentNullException.ThrowIfNull(ptr, "ptr");
		ArgumentOutOfRangeException.ThrowIfNegative(len, "len");
		return new string((sbyte*)ptr, 0, len);
	}

	public unsafe static string? PtrToStringUni(nint ptr)
	{
		if (IsNullOrWin32Atom(ptr))
		{
			return null;
		}
		return new string((char*)ptr);
	}

	public unsafe static string PtrToStringUni(nint ptr, int len)
	{
		ArgumentNullException.ThrowIfNull(ptr, "ptr");
		ArgumentOutOfRangeException.ThrowIfNegative(len, "len");
		return new string((char*)ptr, 0, len);
	}

	public unsafe static string? PtrToStringUTF8(nint ptr)
	{
		if (IsNullOrWin32Atom(ptr))
		{
			return null;
		}
		int byteLength = string.strlen((byte*)ptr);
		return string.CreateStringFromEncoding((byte*)ptr, byteLength, Encoding.UTF8);
	}

	public unsafe static string PtrToStringUTF8(nint ptr, int byteLen)
	{
		ArgumentNullException.ThrowIfNull(ptr, "ptr");
		ArgumentOutOfRangeException.ThrowIfNegative(byteLen, "byteLen");
		return string.CreateStringFromEncoding((byte*)ptr, byteLen, Encoding.UTF8);
	}

	[RequiresDynamicCode("Marshalling code for the object might not be available. Use the SizeOf<T> overload instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static int SizeOf(object structure)
	{
		ArgumentNullException.ThrowIfNull(structure, "structure");
		return SizeOfHelper((RuntimeType)structure.GetType(), throwIfNotMarshalable: true);
	}

	public static int SizeOf<T>(T structure)
	{
		ArgumentNullException.ThrowIfNull(structure, "structure");
		return SizeOfHelper((RuntimeType)structure.GetType(), throwIfNotMarshalable: true);
	}

	[RequiresDynamicCode("Marshalling code for the object might not be available. Use the SizeOf<T> overload instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static int SizeOf(Type t)
	{
		ArgumentNullException.ThrowIfNull(t, "t");
		RuntimeType obj = (t as RuntimeType) ?? throw new ArgumentException(SR.Argument_MustBeRuntimeType, "t");
		if (obj.IsGenericType)
		{
			throw new ArgumentException(SR.Argument_NeedNonGenericType, "t");
		}
		return SizeOfHelper(obj, throwIfNotMarshalable: true);
	}

	public static int SizeOf<T>()
	{
		RuntimeType obj = (RuntimeType)typeof(T);
		if (obj.IsGenericType)
		{
			throw new ArgumentException(SR.Argument_NeedNonGenericType, "T");
		}
		return SizeOfHelper(obj, throwIfNotMarshalable: true);
	}

	public unsafe static int QueryInterface(nint pUnk, in Guid iid, out nint ppv)
	{
		ArgumentNullException.ThrowIfNull(pUnk, "pUnk");
		fixed (Guid* ptr = &iid)
		{
			fixed (nint* ptr2 = &ppv)
			{
				return ((delegate* unmanaged<nint, Guid*, nint*, int>)(*(*(IntPtr**)pUnk)))(pUnk, ptr, ptr2);
			}
		}
	}

	public unsafe static int AddRef(nint pUnk)
	{
		ArgumentNullException.ThrowIfNull(pUnk, "pUnk");
		return ((delegate* unmanaged<nint, int>)(*(IntPtr*)((nint)(*(IntPtr*)pUnk) + sizeof(void*))))(pUnk);
	}

	public unsafe static int Release(nint pUnk)
	{
		ArgumentNullException.ThrowIfNull(pUnk, "pUnk");
		return ((delegate* unmanaged<nint, int>)(*(IntPtr*)((nint)(*(IntPtr*)pUnk) + (nint)2 * (nint)sizeof(void*))))(pUnk);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public unsafe static nint UnsafeAddrOfPinnedArrayElement(Array arr, int index)
	{
		ArgumentNullException.ThrowIfNull(arr, "arr");
		return (nint)((byte*)Unsafe.AsPointer(in MemoryMarshal.GetArrayDataReference(arr)) + (nuint)((nint)(uint)index * (nint)arr.GetElementSize()));
	}

	public unsafe static nint UnsafeAddrOfPinnedArrayElement<T>(T[] arr, int index)
	{
		ArgumentNullException.ThrowIfNull(arr, "arr");
		return (nint)((byte*)Unsafe.AsPointer(in MemoryMarshal.GetArrayDataReference(arr)) + (nuint)((nint)(uint)index * (nint)Unsafe.SizeOf<T>()));
	}

	public static nint OffsetOf<T>(string fieldName)
	{
		return OffsetOf(typeof(T), fieldName);
	}

	public static void Copy(int[] source, int startIndex, nint destination, int length)
	{
		CopyToNative(source, startIndex, destination, length);
	}

	public static void Copy(char[] source, int startIndex, nint destination, int length)
	{
		CopyToNative(source, startIndex, destination, length);
	}

	public static void Copy(short[] source, int startIndex, nint destination, int length)
	{
		CopyToNative(source, startIndex, destination, length);
	}

	public static void Copy(long[] source, int startIndex, nint destination, int length)
	{
		CopyToNative(source, startIndex, destination, length);
	}

	public static void Copy(float[] source, int startIndex, nint destination, int length)
	{
		CopyToNative(source, startIndex, destination, length);
	}

	public static void Copy(double[] source, int startIndex, nint destination, int length)
	{
		CopyToNative(source, startIndex, destination, length);
	}

	public static void Copy(byte[] source, int startIndex, nint destination, int length)
	{
		CopyToNative(source, startIndex, destination, length);
	}

	public static void Copy(nint[] source, int startIndex, nint destination, int length)
	{
		CopyToNative(source, startIndex, destination, length);
	}

	private unsafe static void CopyToNative<T>(T[] source, int startIndex, nint destination, int length)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(destination, "destination");
		new Span<T>(source, startIndex, length).CopyTo(new Span<T>((void*)destination, length));
	}

	public static void Copy(nint source, int[] destination, int startIndex, int length)
	{
		CopyToManaged(source, destination, startIndex, length);
	}

	public static void Copy(nint source, char[] destination, int startIndex, int length)
	{
		CopyToManaged(source, destination, startIndex, length);
	}

	public static void Copy(nint source, short[] destination, int startIndex, int length)
	{
		CopyToManaged(source, destination, startIndex, length);
	}

	public static void Copy(nint source, long[] destination, int startIndex, int length)
	{
		CopyToManaged(source, destination, startIndex, length);
	}

	public static void Copy(nint source, float[] destination, int startIndex, int length)
	{
		CopyToManaged(source, destination, startIndex, length);
	}

	public static void Copy(nint source, double[] destination, int startIndex, int length)
	{
		CopyToManaged(source, destination, startIndex, length);
	}

	public static void Copy(nint source, byte[] destination, int startIndex, int length)
	{
		CopyToManaged(source, destination, startIndex, length);
	}

	public static void Copy(nint source, nint[] destination, int startIndex, int length)
	{
		CopyToManaged(source, destination, startIndex, length);
	}

	private unsafe static void CopyToManaged<T>(nint source, T[] destination, int startIndex, int length)
	{
		ArgumentNullException.ThrowIfNull(destination, "destination");
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentOutOfRangeException.ThrowIfNegative(startIndex, "startIndex");
		ArgumentOutOfRangeException.ThrowIfNegative(length, "length");
		new Span<T>((void*)source, length).CopyTo(new Span<T>(destination, startIndex, length));
	}

	public unsafe static byte ReadByte(nint ptr, int ofs)
	{
		try
		{
			return *(byte*)(ptr + ofs);
		}
		catch (NullReferenceException)
		{
			throw new AccessViolationException();
		}
	}

	public static byte ReadByte(nint ptr)
	{
		return ReadByte(ptr, 0);
	}

	public unsafe static short ReadInt16(nint ptr, int ofs)
	{
		try
		{
			byte* ptr2 = (byte*)(ptr + ofs);
			if (((int)ptr2 & 1) == 0)
			{
				return *(short*)ptr2;
			}
			return Unsafe.ReadUnaligned<short>(ptr2);
		}
		catch (NullReferenceException)
		{
			throw new AccessViolationException();
		}
	}

	public static short ReadInt16(nint ptr)
	{
		return ReadInt16(ptr, 0);
	}

	public unsafe static int ReadInt32(nint ptr, int ofs)
	{
		try
		{
			byte* ptr2 = (byte*)(ptr + ofs);
			if (((int)ptr2 & 3) == 0)
			{
				return *(int*)ptr2;
			}
			return Unsafe.ReadUnaligned<int>(ptr2);
		}
		catch (NullReferenceException)
		{
			throw new AccessViolationException();
		}
	}

	public static int ReadInt32(nint ptr)
	{
		return ReadInt32(ptr, 0);
	}

	[RequiresDynamicCode("Marshalling code for the object might not be available")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ReadIntPtr(Object, Int32) may be unavailable in future releases.")]
	public static nint ReadIntPtr(object ptr, int ofs)
	{
		return (nint)ReadInt64(ptr, ofs);
	}

	public static nint ReadIntPtr(nint ptr, int ofs)
	{
		return (nint)ReadInt64(ptr, ofs);
	}

	public static nint ReadIntPtr(nint ptr)
	{
		return ReadIntPtr(ptr, 0);
	}

	public unsafe static long ReadInt64(nint ptr, int ofs)
	{
		try
		{
			byte* ptr2 = (byte*)(ptr + ofs);
			if (((int)ptr2 & 7) == 0)
			{
				return *(long*)ptr2;
			}
			return Unsafe.ReadUnaligned<long>(ptr2);
		}
		catch (NullReferenceException)
		{
			throw new AccessViolationException();
		}
	}

	public static long ReadInt64(nint ptr)
	{
		return ReadInt64(ptr, 0);
	}

	public unsafe static void WriteByte(nint ptr, int ofs, byte val)
	{
		try
		{
			*(byte*)(ptr + ofs) = val;
		}
		catch (NullReferenceException)
		{
			throw new AccessViolationException();
		}
	}

	public static void WriteByte(nint ptr, byte val)
	{
		WriteByte(ptr, 0, val);
	}

	public unsafe static void WriteInt16(nint ptr, int ofs, short val)
	{
		try
		{
			byte* ptr2 = (byte*)(ptr + ofs);
			if (((int)ptr2 & 1) == 0)
			{
				*(short*)ptr2 = val;
			}
			else
			{
				Unsafe.WriteUnaligned(ptr2, val);
			}
		}
		catch (NullReferenceException)
		{
			throw new AccessViolationException();
		}
	}

	public static void WriteInt16(nint ptr, short val)
	{
		WriteInt16(ptr, 0, val);
	}

	public static void WriteInt16(nint ptr, int ofs, char val)
	{
		WriteInt16(ptr, ofs, (short)val);
	}

	[RequiresDynamicCode("Marshalling code for the object might not be available")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("WriteInt16(Object, Int32, Char) may be unavailable in future releases.")]
	public static void WriteInt16([In][Out] object ptr, int ofs, char val)
	{
		WriteInt16(ptr, ofs, (short)val);
	}

	public static void WriteInt16(nint ptr, char val)
	{
		WriteInt16(ptr, 0, (short)val);
	}

	public unsafe static void WriteInt32(nint ptr, int ofs, int val)
	{
		try
		{
			byte* ptr2 = (byte*)(ptr + ofs);
			if (((int)ptr2 & 3) == 0)
			{
				*(int*)ptr2 = val;
			}
			else
			{
				Unsafe.WriteUnaligned(ptr2, val);
			}
		}
		catch (NullReferenceException)
		{
			throw new AccessViolationException();
		}
	}

	public static void WriteInt32(nint ptr, int val)
	{
		WriteInt32(ptr, 0, val);
	}

	public static void WriteIntPtr(nint ptr, int ofs, nint val)
	{
		WriteInt64(ptr, ofs, val);
	}

	[RequiresDynamicCode("Marshalling code for the object might not be available")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("WriteIntPtr(Object, Int32, IntPtr) may be unavailable in future releases.")]
	public static void WriteIntPtr(object ptr, int ofs, nint val)
	{
		WriteInt64(ptr, ofs, val);
	}

	public static void WriteIntPtr(nint ptr, nint val)
	{
		WriteIntPtr(ptr, 0, val);
	}

	public unsafe static void WriteInt64(nint ptr, int ofs, long val)
	{
		try
		{
			byte* ptr2 = (byte*)(ptr + ofs);
			if (((int)ptr2 & 7) == 0)
			{
				*(long*)ptr2 = val;
			}
			else
			{
				Unsafe.WriteUnaligned(ptr2, val);
			}
		}
		catch (NullReferenceException)
		{
			throw new AccessViolationException();
		}
	}

	public static void WriteInt64(nint ptr, long val)
	{
		WriteInt64(ptr, 0, val);
	}

	public static void Prelink(MethodInfo m)
	{
		ArgumentNullException.ThrowIfNull(m, "m");
		PrelinkCore(m);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2070:UnrecognizedReflectionPattern", Justification = "This only needs to prelink methods that are actually used")]
	public static void PrelinkAll(Type c)
	{
		ArgumentNullException.ThrowIfNull(c, "c");
		MethodInfo[] methods = c.GetMethods();
		for (int i = 0; i < methods.Length; i++)
		{
			Prelink(methods[i]);
		}
	}

	[UnconditionalSuppressMessage("AotAnalysis", "IL3050:AotUnfriendlyApi", Justification = "AOT compilers can see the T.")]
	public static void StructureToPtr<T>([DisallowNull] T structure, nint ptr, bool fDeleteOld)
	{
		StructureToPtr((object)structure, ptr, fDeleteOld);
	}

	[RequiresDynamicCode("Marshalling code for the object might not be available")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static object? PtrToStructure(nint ptr, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type structureType)
	{
		ArgumentNullException.ThrowIfNull(structureType, "structureType");
		if (ptr == IntPtr.Zero)
		{
			return null;
		}
		if (structureType.IsGenericType)
		{
			throw new ArgumentException(SR.Argument_NeedNonGenericType, "structureType");
		}
		if (!(structureType is RuntimeType))
		{
			throw new ArgumentException(SR.Argument_MustBeRuntimeType, "structureType");
		}
		object obj = Activator.CreateInstance(structureType, nonPublic: true);
		PtrToStructureHelper(ptr, obj, allowValueClasses: true);
		return obj;
	}

	[RequiresDynamicCode("Marshalling code for the object might not be available")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void PtrToStructure(nint ptr, object structure)
	{
		ArgumentNullException.ThrowIfNull(ptr, "ptr");
		ArgumentNullException.ThrowIfNull(structure, "structure");
		PtrToStructureHelper(ptr, structure, allowValueClasses: false);
	}

	public static void PtrToStructure<T>(nint ptr, [DisallowNull] T structure)
	{
		ArgumentNullException.ThrowIfNull(ptr, "ptr");
		object obj = structure;
		ArgumentNullException.ThrowIfNull(obj, "structure");
		PtrToStructureHelper(ptr, obj, allowValueClasses: false);
	}

	public static T? PtrToStructure<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] T>(nint ptr)
	{
		if (ptr == IntPtr.Zero)
		{
			return (T)(object)null;
		}
		Type? typeFromHandle = typeof(T);
		if (typeFromHandle.IsGenericType)
		{
			throw new ArgumentException(SR.Argument_NeedNonGenericType, "T");
		}
		object obj = Activator.CreateInstance(typeFromHandle, nonPublic: true);
		PtrToStructureHelper(ptr, obj, allowValueClasses: true);
		return (T)obj;
	}

	[UnconditionalSuppressMessage("AotAnalysis", "IL3050:AotUnfriendlyApi", Justification = "AOT compilers can see the T.")]
	public static void DestroyStructure<T>(nint ptr)
	{
		DestroyStructure(ptr, typeof(T));
	}

	public static Exception? GetExceptionForHR(int errorCode)
	{
		return GetExceptionForHR(errorCode, IntPtr.Zero);
	}

	public static Exception? GetExceptionForHR(int errorCode, nint errorInfo)
	{
		if (errorCode >= 0)
		{
			return null;
		}
		return GetExceptionForHRInternal(errorCode, errorInfo);
	}

	public static Exception? GetExceptionForHR(int errorCode, in Guid iid, nint pUnk)
	{
		if (errorCode >= 0)
		{
			return null;
		}
		return GetExceptionForHRInternal(errorCode, in iid, pUnk);
	}

	private unsafe static Exception GetExceptionForHRInternal(int errorCode, in Guid iid, nint pUnk)
	{
		nint ppErrorInfo = -1;
		Interop.OleAut32.GetErrorInfo(0u, out ppErrorInfo);
		if (ppErrorInfo == IntPtr.Zero)
		{
			ppErrorInfo = -1;
		}
		if (ppErrorInfo != -1 && pUnk != IntPtr.Zero)
		{
			int num = QueryInterface(pUnk, new Guid(3742055776u, 21647, 4123, 142, 101, 8, 0, 43, 43, 209, 25), out var ppv);
			if (num == 0)
			{
				fixed (Guid* ptr = &iid)
				{
					num = ((delegate* unmanaged[MemberFunction]<nint, Guid*, int>)(*(IntPtr*)((nint)(*(IntPtr*)ppv) + (nint)3 * (nint)sizeof(void*))))(ppv, ptr);
				}
				Release(ppv);
			}
			if (num != 0)
			{
				Release(ppErrorInfo);
				ppErrorInfo = -1;
			}
		}
		return GetExceptionForHRInternal(errorCode, ppErrorInfo);
	}

	public static void ThrowExceptionForHR(int errorCode)
	{
		if (errorCode < 0)
		{
			throw GetExceptionForHR(errorCode);
		}
	}

	public static void ThrowExceptionForHR(int errorCode, nint errorInfo)
	{
		if (errorCode < 0)
		{
			throw GetExceptionForHR(errorCode, errorInfo);
		}
	}

	public static void ThrowExceptionForHR(int errorCode, in Guid iid, nint pUnk)
	{
		if (errorCode < 0)
		{
			throw GetExceptionForHR(errorCode, in iid, pUnk);
		}
	}

	public static nint SecureStringToBSTR(SecureString s)
	{
		if (s == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
		}
		return s.MarshalToBSTR();
	}

	public static nint SecureStringToCoTaskMemAnsi(SecureString s)
	{
		if (s == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
		}
		return s.MarshalToString(globalAlloc: false, unicode: false);
	}

	public static nint SecureStringToCoTaskMemUnicode(SecureString s)
	{
		if (s == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
		}
		return s.MarshalToString(globalAlloc: false, unicode: true);
	}

	public static nint SecureStringToGlobalAllocAnsi(SecureString s)
	{
		if (s == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
		}
		return s.MarshalToString(globalAlloc: true, unicode: false);
	}

	public static nint SecureStringToGlobalAllocUnicode(SecureString s)
	{
		if (s == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
		}
		return s.MarshalToString(globalAlloc: true, unicode: true);
	}

	public unsafe static nint StringToHGlobalAnsi(string? s)
	{
		if (s == null)
		{
			return IntPtr.Zero;
		}
		long num = (long)(s.Length + 1) * (long)SystemMaxDBCSCharSize;
		int num2 = (int)num;
		if (num2 != num)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.s);
		}
		nint num3 = AllocHGlobal((nint)num2);
		StringToAnsiString(s, (byte*)num3, num2);
		return num3;
	}

	public unsafe static nint StringToHGlobalUni(string? s)
	{
		if (s == null)
		{
			return IntPtr.Zero;
		}
		int num = (s.Length + 1) * 2;
		if (num < s.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.s);
		}
		nint num2 = AllocHGlobal((nint)num);
		s.CopyTo(new Span<char>((void*)num2, s.Length));
		*(short*)(num2 + (nint)s.Length * (nint)2) = 0;
		return num2;
	}

	public unsafe static nint StringToCoTaskMemUni(string? s)
	{
		if (s == null)
		{
			return IntPtr.Zero;
		}
		int num = (s.Length + 1) * 2;
		if (num < s.Length)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.s);
		}
		nint num2 = AllocCoTaskMem(num);
		s.CopyTo(new Span<char>((void*)num2, s.Length));
		*(short*)(num2 + (nint)s.Length * (nint)2) = 0;
		return num2;
	}

	public unsafe static nint StringToCoTaskMemUTF8(string? s)
	{
		if (s == null)
		{
			return IntPtr.Zero;
		}
		int maxByteCount = Encoding.UTF8.GetMaxByteCount(s.Length);
		byte* ptr;
		byte* result = (ptr = (byte*)AllocCoTaskMem(checked(maxByteCount + 1)));
		int bytes = Encoding.UTF8.GetBytes(s.AsSpan(), new Span<byte>(ptr, maxByteCount));
		ptr[bytes] = 0;
		return (nint)result;
	}

	public unsafe static nint StringToCoTaskMemAnsi(string? s)
	{
		if (s == null)
		{
			return IntPtr.Zero;
		}
		long num = (long)(s.Length + 1) * (long)SystemMaxDBCSCharSize;
		int num2 = (int)num;
		if (num2 != num)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.s);
		}
		nint num3 = AllocCoTaskMem(num2);
		StringToAnsiString(s, (byte*)num3, num2);
		return num3;
	}

	public static Guid GenerateGuidForType(Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		if (!(type is RuntimeType))
		{
			throw new ArgumentException(SR.Argument_MustBeRuntimeType, "type");
		}
		return type.GUID;
	}

	[RequiresUnreferencedCode("Built-in COM support is not trim compatible", Url = "https://aka.ms/dotnet-illink/com")]
	public static string? GenerateProgIdForType(Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		if (type.IsImport)
		{
			throw new ArgumentException(SR.Argument_TypeMustNotBeComImport, "type");
		}
		if (type.IsGenericType)
		{
			throw new ArgumentException(SR.Argument_NeedNonGenericType, "type");
		}
		ProgIdAttribute customAttribute = type.GetCustomAttribute<ProgIdAttribute>();
		if (customAttribute != null)
		{
			return customAttribute.Value ?? string.Empty;
		}
		return type.FullName;
	}

	[RequiresDynamicCode("Marshalling code for the delegate might not be available. Use the GetDelegateForFunctionPointer<TDelegate> overload instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static Delegate GetDelegateForFunctionPointer(nint ptr, Type t)
	{
		ArgumentNullException.ThrowIfNull(t, "t");
		ArgumentNullException.ThrowIfNull(ptr, "ptr");
		if (!(t is RuntimeType runtimeType))
		{
			throw new ArgumentException(SR.Argument_MustBeRuntimeType, "t");
		}
		if (runtimeType.IsGenericType)
		{
			throw new ArgumentException(SR.Argument_NeedNonGenericType, "t");
		}
		if (runtimeType.BaseType != typeof(MulticastDelegate) && runtimeType != typeof(MulticastDelegate))
		{
			throw new ArgumentException(SR.Arg_MustBeDelegate, "t");
		}
		return GetDelegateForFunctionPointerInternal(ptr, runtimeType);
	}

	public static TDelegate GetDelegateForFunctionPointer<TDelegate>(nint ptr)
	{
		ArgumentNullException.ThrowIfNull(ptr, "ptr");
		RuntimeType runtimeType = (RuntimeType)typeof(TDelegate);
		if (runtimeType.IsGenericType)
		{
			throw new ArgumentException(SR.Argument_NeedNonGenericType, "TDelegate");
		}
		if (runtimeType.BaseType != typeof(MulticastDelegate) && runtimeType != typeof(MulticastDelegate))
		{
			throw new ArgumentException(SR.Arg_MustBeDelegate, "TDelegate");
		}
		return (TDelegate)(object)GetDelegateForFunctionPointerInternal(ptr, runtimeType);
	}

	[RequiresDynamicCode("Marshalling code for the delegate might not be available. Use the GetFunctionPointerForDelegate<TDelegate> overload instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static nint GetFunctionPointerForDelegate(Delegate d)
	{
		ArgumentNullException.ThrowIfNull(d, "d");
		return GetFunctionPointerForDelegateInternal(d);
	}

	[UnconditionalSuppressMessage("AotAnalysis", "IL3050:AotUnfriendlyApi", Justification = "AOT compilers can see the T.")]
	public static nint GetFunctionPointerForDelegate<TDelegate>(TDelegate d) where TDelegate : notnull
	{
		return GetFunctionPointerForDelegate((Delegate)(object)d);
	}

	public static int GetHRForLastWin32Error()
	{
		int lastPInvokeError = GetLastPInvokeError();
		if ((lastPInvokeError & 0x80000000u) == 2147483648u)
		{
			return lastPInvokeError;
		}
		return (lastPInvokeError & 0xFFFF) | -2147024896;
	}

	public unsafe static void ZeroFreeBSTR(nint s)
	{
		if (s != IntPtr.Zero)
		{
			NativeMemory.Clear((void*)s, SysStringByteLen(s));
			FreeBSTR(s);
		}
	}

	public static void ZeroFreeCoTaskMemAnsi(nint s)
	{
		ZeroFreeCoTaskMemUTF8(s);
	}

	public unsafe static void ZeroFreeCoTaskMemUnicode(nint s)
	{
		if (s != IntPtr.Zero)
		{
			NativeMemory.Clear((void*)s, (nuint)string.wcslen((char*)s) * (nuint)2u);
			FreeCoTaskMem(s);
		}
	}

	public unsafe static void ZeroFreeCoTaskMemUTF8(nint s)
	{
		if (s != IntPtr.Zero)
		{
			NativeMemory.Clear((void*)s, (nuint)string.strlen((byte*)s));
			FreeCoTaskMem(s);
		}
	}

	public unsafe static void ZeroFreeGlobalAllocAnsi(nint s)
	{
		if (s != IntPtr.Zero)
		{
			NativeMemory.Clear((void*)s, (nuint)string.strlen((byte*)s));
			FreeHGlobal(s);
		}
	}

	public unsafe static void ZeroFreeGlobalAllocUnicode(nint s)
	{
		if (s != IntPtr.Zero)
		{
			NativeMemory.Clear((void*)s, (nuint)string.wcslen((char*)s) * (nuint)2u);
			FreeHGlobal(s);
		}
	}

	public unsafe static nint StringToBSTR(string? s)
	{
		if (s == null)
		{
			return IntPtr.Zero;
		}
		nint num = AllocBSTR(s.Length);
		s.CopyTo(new Span<char>((void*)num, s.Length));
		return num;
	}

	public static string PtrToStringBSTR(nint ptr)
	{
		ArgumentNullException.ThrowIfNull(ptr, "ptr");
		return PtrToStringUni(ptr, (int)(SysStringByteLen(ptr) / 2));
	}

	internal unsafe static uint SysStringByteLen(nint s)
	{
		return *(uint*)(s - 4);
	}

	[SupportedOSPlatform("windows")]
	public static Type? GetTypeFromCLSID(Guid clsid)
	{
		return GetTypeFromCLSID(clsid, null, throwOnError: false);
	}

	public static void InitHandle(SafeHandle safeHandle, nint handle)
	{
		safeHandle.SetHandle(handle);
	}

	public static int GetLastWin32Error()
	{
		return GetLastPInvokeError();
	}

	public static string GetLastPInvokeErrorMessage()
	{
		return GetPInvokeErrorMessage(GetLastPInvokeError());
	}

	public static string? PtrToStringAuto(nint ptr, int len)
	{
		return PtrToStringUni(ptr, len);
	}

	public static string? PtrToStringAuto(nint ptr)
	{
		return PtrToStringUni(ptr);
	}

	public static nint StringToHGlobalAuto(string? s)
	{
		return StringToHGlobalUni(s);
	}

	public static nint StringToCoTaskMemAuto(string? s)
	{
		return StringToCoTaskMemUni(s);
	}

	private unsafe static int GetSystemMaxDBCSCharSize()
	{
		Interop.Kernel32.CPINFO cPINFO = default(Interop.Kernel32.CPINFO);
		if (Interop.Kernel32.GetCPInfo(0u, &cPINFO) == Interop.BOOL.FALSE)
		{
			return 2;
		}
		return cPINFO.MaxCharSize;
	}

	private static bool IsNullOrWin32Atom(nint ptr)
	{
		long num = ptr;
		return (num & -65536) == 0;
	}

	internal unsafe static int StringToAnsiString(string s, byte* buffer, int bufferLength, bool bestFit = false, bool throwOnUnmappableChar = false)
	{
		uint dwFlags = ((!bestFit) ? 1024u : 0u);
		Interop.BOOL bOOL = Interop.BOOL.FALSE;
		int num;
		fixed (char* lpWideCharStr = s)
		{
			num = Interop.Kernel32.WideCharToMultiByte(0u, dwFlags, lpWideCharStr, s.Length, buffer, bufferLength, null, throwOnUnmappableChar ? (&bOOL) : null);
		}
		if (bOOL != Interop.BOOL.FALSE)
		{
			throw new ArgumentException(SR.Interop_Marshal_Unmappable_Char);
		}
		buffer[num] = 0;
		return num;
	}

	internal unsafe static int GetAnsiStringByteCount(ReadOnlySpan<char> chars)
	{
		int num;
		if (chars.Length == 0)
		{
			num = 0;
		}
		else
		{
			fixed (char* lpWideCharStr = chars)
			{
				num = Interop.Kernel32.WideCharToMultiByte(0u, 1024u, lpWideCharStr, chars.Length, null, 0, null, null);
				if (num <= 0)
				{
					throw new ArgumentException();
				}
			}
		}
		return checked(num + 1);
	}

	internal unsafe static void GetAnsiStringBytes(ReadOnlySpan<char> chars, Span<byte> bytes)
	{
		int num;
		if (chars.Length == 0)
		{
			num = 0;
		}
		else
		{
			fixed (char* lpWideCharStr = chars)
			{
				fixed (byte* lpMultiByteStr = bytes)
				{
					num = Interop.Kernel32.WideCharToMultiByte(0u, 1024u, lpWideCharStr, chars.Length, lpMultiByteStr, bytes.Length, null, null);
					if (num <= 0)
					{
						throw new ArgumentException();
					}
				}
			}
		}
		bytes[num] = 0;
	}

	public unsafe static nint AllocHGlobal(nint cb)
	{
		void* intPtr = Interop.Kernel32.LocalAlloc((nuint)cb);
		if (intPtr == null)
		{
			throw new OutOfMemoryException();
		}
		return (nint)intPtr;
	}

	public unsafe static void FreeHGlobal(nint hglobal)
	{
		if (!IsNullOrWin32Atom(hglobal))
		{
			Interop.Kernel32.LocalFree((void*)hglobal);
		}
	}

	public unsafe static nint ReAllocHGlobal(nint pv, nint cb)
	{
		if (pv == IntPtr.Zero)
		{
			return AllocHGlobal(cb);
		}
		void* intPtr = Interop.Kernel32.LocalReAlloc((void*)pv, (nuint)cb);
		if (intPtr == null)
		{
			throw new OutOfMemoryException();
		}
		return (nint)intPtr;
	}

	public static nint AllocCoTaskMem(int cb)
	{
		nint num = Interop.Ole32.CoTaskMemAlloc((uint)cb);
		if (num == IntPtr.Zero)
		{
			throw new OutOfMemoryException();
		}
		return num;
	}

	public static void FreeCoTaskMem(nint ptr)
	{
		if (!IsNullOrWin32Atom(ptr))
		{
			Interop.Ole32.CoTaskMemFree(ptr);
		}
	}

	public static nint ReAllocCoTaskMem(nint pv, int cb)
	{
		nint num = Interop.Ole32.CoTaskMemRealloc(pv, (uint)cb);
		if (num == IntPtr.Zero && cb != 0)
		{
			throw new OutOfMemoryException();
		}
		return num;
	}

	internal static nint AllocBSTR(int length)
	{
		nint num = Interop.OleAut32.SysAllocStringLen(IntPtr.Zero, (uint)length);
		if (num == IntPtr.Zero)
		{
			throw new OutOfMemoryException();
		}
		return num;
	}

	internal static nint AllocBSTRByteLen(uint length)
	{
		nint num = Interop.OleAut32.SysAllocStringByteLen(null, length);
		if (num == IntPtr.Zero)
		{
			throw new OutOfMemoryException();
		}
		return num;
	}

	public static void FreeBSTR(nint ptr)
	{
		if (!IsNullOrWin32Atom(ptr))
		{
			Interop.OleAut32.SysFreeString(ptr);
		}
	}

	internal static Type GetTypeFromProgID(string progID, string server, bool throwOnError)
	{
		ArgumentNullException.ThrowIfNull(progID, "progID");
		int num = Interop.Ole32.CLSIDFromProgID(progID, out var lpclsid);
		if (num < 0)
		{
			if (throwOnError)
			{
				throw GetExceptionForHR(num, new IntPtr(-1));
			}
			return null;
		}
		return GetTypeFromCLSID(lpclsid, server, throwOnError);
	}

	public static int GetLastSystemError()
	{
		return Interop.Kernel32.GetLastError();
	}

	public static void SetLastSystemError(int error)
	{
		Interop.Kernel32.SetLastError(error);
	}

	public static string GetPInvokeErrorMessage(int error)
	{
		return Interop.Kernel32.GetMessage(error);
	}
}
