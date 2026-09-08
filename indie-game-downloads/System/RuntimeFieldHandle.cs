using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;

namespace System;

[NonVersionable]
public struct RuntimeFieldHandle : IEquatable<RuntimeFieldHandle>, ISerializable
{
	private readonly IRuntimeFieldInfo m_ptr;

	public nint Value
	{
		get
		{
			if (m_ptr == null)
			{
				return IntPtr.Zero;
			}
			return m_ptr.Value.Value;
		}
	}

	internal RuntimeFieldHandle(IRuntimeFieldInfo fieldInfo)
	{
		m_ptr = fieldInfo;
	}

	internal IRuntimeFieldInfo GetRuntimeFieldInfo()
	{
		return m_ptr;
	}

	internal bool IsNullHandle()
	{
		return m_ptr == null;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Value);
	}

	public override bool Equals(object? obj)
	{
		if (!(obj is RuntimeFieldHandle runtimeFieldHandle))
		{
			return false;
		}
		return runtimeFieldHandle.Value == Value;
	}

	public bool Equals(RuntimeFieldHandle handle)
	{
		return handle.Value == Value;
	}

	public static RuntimeFieldHandle FromIntPtr(nint value)
	{
		RuntimeFieldHandleInternal runtimeFieldHandleInternal = new RuntimeFieldHandleInternal(value);
		return new RuntimeFieldHandle(new RuntimeFieldInfoStub(runtimeFieldHandleInternal, GetLoaderAllocator(runtimeFieldHandleInternal)));
	}

	public static nint ToIntPtr(RuntimeFieldHandle value)
	{
		return value.Value;
	}

	public static bool operator ==(RuntimeFieldHandle left, RuntimeFieldHandle right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(RuntimeFieldHandle left, RuntimeFieldHandle right)
	{
		return !left.Equals(right);
	}

	internal static string GetName(IRuntimeFieldInfo field)
	{
		string result = GetUtf8Name(field.Value).ToString();
		GC.KeepAlive(field);
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void* GetUtf8NameInternal(RuntimeFieldHandleInternal field);

	internal unsafe static MdUtf8String GetUtf8Name(RuntimeFieldHandleInternal field)
	{
		void* utf8NameInternal = GetUtf8NameInternal(field);
		if (utf8NameInternal == null)
		{
			throw new BadImageFormatException();
		}
		return new MdUtf8String(utf8NameInternal);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern FieldAttributes GetAttributes(RuntimeFieldHandleInternal field);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern MethodTable* GetApproxDeclaringMethodTable(RuntimeFieldHandleInternal field);

	internal unsafe static RuntimeType GetApproxDeclaringType(RuntimeFieldHandleInternal field)
	{
		return RuntimeTypeHandle.GetRuntimeType(GetApproxDeclaringMethodTable(field));
	}

	internal static RuntimeType GetApproxDeclaringType(IRuntimeFieldInfo field)
	{
		RuntimeType approxDeclaringType = GetApproxDeclaringType(field.Value);
		GC.KeepAlive(field);
		return approxDeclaringType;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool IsFastPathSupported(RtFieldInfo field);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern int GetInstanceFieldOffset(RtFieldInfo field);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern nint GetStaticFieldAddress(RtFieldInfo field);

	[LibraryImport("QCall", EntryPoint = "RuntimeFieldHandle_GetRVAFieldInfo")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool GetRVAFieldInfo(RuntimeFieldHandleInternal field, out void* address, out uint size)
	{
		address = default(void*);
		size = 0u;
		int num;
		fixed (uint* _size_native = &size)
		{
			fixed (void** _address_native = &address)
			{
				num = __PInvoke(field, _address_native, _size_native);
			}
		}
		return num != 0;
		[DllImport("QCall", EntryPoint = "RuntimeFieldHandle_GetRVAFieldInfo", ExactSpelling = true)]
		unsafe static extern int __PInvoke(RuntimeFieldHandleInternal __field_native, void** __address_native, uint* __size_native);
	}

	internal static ref byte GetFieldDataReference(object target, RuntimeFieldInfo field)
	{
		ByteRef byteRef = default(ByteRef);
		GetFieldDataReference(((RtFieldInfo)field).GetFieldDesc(), ObjectHandleOnStack.Create(ref target), ByteRefOnStack.Create(ref byteRef));
		GC.KeepAlive(field);
		return ref byteRef.Get();
	}

	internal static ref byte GetFieldDataReference(ref byte target, RuntimeFieldInfo field)
	{
		int instanceFieldOffset = GetInstanceFieldOffset((RtFieldInfo)field);
		return ref Unsafe.AddByteOffset(ref target, instanceFieldOffset);
	}

	[DllImport("QCall", EntryPoint = "RuntimeFieldHandle_GetFieldDataReference", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeFieldHandle_GetFieldDataReference")]
	private static extern void GetFieldDataReference(nint fieldDesc, ObjectHandleOnStack target, ByteRefOnStack fieldDataRef);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern int GetToken(nint fieldDesc);

	internal static int GetToken(RtFieldInfo field)
	{
		int token = GetToken(field.GetFieldDesc());
		GC.KeepAlive(field);
		return token;
	}

	[LibraryImport("QCall", EntryPoint = "RuntimeFieldHandle_GetValue")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static void GetValue(nint fieldDesc, ObjectHandleOnStack instance, QCallTypeHandle fieldType, QCallTypeHandle declaringType, [MarshalAs(UnmanagedType.Bool)] ref bool isClassInitialized, ObjectHandleOnStack result)
	{
		int num = (isClassInitialized ? 1 : 0);
		__PInvoke(fieldDesc, instance, fieldType, declaringType, &num, result);
		isClassInitialized = num != 0;
		[DllImport("QCall", EntryPoint = "RuntimeFieldHandle_GetValue", ExactSpelling = true)]
		unsafe static extern void __PInvoke(nint __fieldDesc_native, ObjectHandleOnStack __instance_native, QCallTypeHandle __fieldType_native, QCallTypeHandle __declaringType_native, int* __isClassInitialized_native, ObjectHandleOnStack __result_native);
	}

	internal static object GetValue(RtFieldInfo field, object instance, RuntimeType fieldType, RuntimeType declaringType, ref bool isClassInitialized)
	{
		if ((object)field == null || (object)fieldType == null)
		{
			throw new ArgumentNullException(SR.Arg_InvalidHandle);
		}
		object o = null;
		GetValue(field.GetFieldDesc(), ObjectHandleOnStack.Create(ref instance), new QCallTypeHandle(ref fieldType), new QCallTypeHandle(ref declaringType), ref isClassInitialized, ObjectHandleOnStack.Create(ref o));
		GC.KeepAlive(field);
		return o;
	}

	[DllImport("QCall", EntryPoint = "RuntimeFieldHandle_GetValueDirect", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeFieldHandle_GetValueDirect")]
	private unsafe static extern void GetValueDirect(nint fieldDesc, void* pTypedRef, QCallTypeHandle fieldType, QCallTypeHandle declaringType, ObjectHandleOnStack result);

	internal unsafe static object GetValueDirect(RtFieldInfo field, RuntimeType fieldType, TypedReference typedRef, RuntimeType contextType)
	{
		if ((object)field == null || (object)fieldType == null)
		{
			throw new ArgumentNullException(SR.Arg_InvalidHandle);
		}
		object o = null;
		GetValueDirect(field.GetFieldDesc(), &typedRef, new QCallTypeHandle(ref fieldType), new QCallTypeHandle(ref contextType), ObjectHandleOnStack.Create(ref o));
		GC.KeepAlive(field);
		return o;
	}

	[LibraryImport("QCall", EntryPoint = "RuntimeFieldHandle_SetValue")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static void SetValue(nint fieldDesc, ObjectHandleOnStack instance, ObjectHandleOnStack value, QCallTypeHandle fieldType, QCallTypeHandle declaringType, [MarshalAs(UnmanagedType.Bool)] ref bool isClassInitialized)
	{
		int num = (isClassInitialized ? 1 : 0);
		__PInvoke(fieldDesc, instance, value, fieldType, declaringType, &num);
		isClassInitialized = num != 0;
		[DllImport("QCall", EntryPoint = "RuntimeFieldHandle_SetValue", ExactSpelling = true)]
		unsafe static extern void __PInvoke(nint __fieldDesc_native, ObjectHandleOnStack __instance_native, ObjectHandleOnStack __value_native, QCallTypeHandle __fieldType_native, QCallTypeHandle __declaringType_native, int* __isClassInitialized_native);
	}

	internal static void SetValue(RtFieldInfo field, object obj, object value, RuntimeType fieldType, RuntimeType declaringType, ref bool isClassInitialized)
	{
		if ((object)field == null || (object)fieldType == null)
		{
			throw new ArgumentNullException(SR.Arg_InvalidHandle);
		}
		SetValue(field.GetFieldDesc(), ObjectHandleOnStack.Create(ref obj), ObjectHandleOnStack.Create(ref value), new QCallTypeHandle(ref fieldType), new QCallTypeHandle(ref declaringType), ref isClassInitialized);
		GC.KeepAlive(field);
	}

	[DllImport("QCall", EntryPoint = "RuntimeFieldHandle_SetValueDirect", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeFieldHandle_SetValueDirect")]
	private unsafe static extern void SetValueDirect(nint fieldDesc, void* pTypedRef, ObjectHandleOnStack value, QCallTypeHandle fieldType, QCallTypeHandle declaringType);

	internal unsafe static void SetValueDirect(RtFieldInfo field, RuntimeType fieldType, TypedReference typedRef, object value, RuntimeType contextType)
	{
		if ((object)field == null || (object)fieldType == null)
		{
			throw new ArgumentNullException(SR.Arg_InvalidHandle);
		}
		SetValueDirect(field.GetFieldDesc(), &typedRef, ObjectHandleOnStack.Create(ref value), new QCallTypeHandle(ref fieldType), new QCallTypeHandle(ref contextType));
		GC.KeepAlive(field);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern RuntimeFieldHandleInternal GetStaticFieldForGenericType(RuntimeFieldHandleInternal field, MethodTable* pMT);

	internal unsafe static RuntimeFieldHandleInternal GetStaticFieldForGenericType(RuntimeFieldHandleInternal field, RuntimeType declaringType)
	{
		return GetStaticFieldForGenericType(field, declaringType.GetNativeTypeHandle().AsMethodTable());
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool AcquiresContextFromThis(RuntimeFieldHandleInternal field);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LoaderAllocator GetLoaderAllocatorInternal(RuntimeFieldHandleInternal field);

	internal static LoaderAllocator GetLoaderAllocator(RuntimeFieldHandleInternal field)
	{
		if (field.IsNullHandle())
		{
			throw new ArgumentNullException(SR.Arg_InvalidHandle);
		}
		return GetLoaderAllocatorInternal(field);
	}

	[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		throw new PlatformNotSupportedException();
	}

	[DllImport("QCall", EntryPoint = "RuntimeFieldHandle_GetEnCFieldAddr", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeFieldHandle_GetEnCFieldAddr")]
	private unsafe static extern void* GetEnCFieldAddr(ObjectHandleOnStack tgt, void* pFD);

	[StackTraceHidden]
	[DebuggerStepThrough]
	[DebuggerHidden]
	internal unsafe static void* GetFieldAddr(object tgt, void* pFD)
	{
		void* enCFieldAddr = GetEnCFieldAddr(ObjectHandleOnStack.Create(ref tgt), pFD);
		if (enCFieldAddr == null)
		{
			throw new NullReferenceException();
		}
		return enCFieldAddr;
	}

	[StackTraceHidden]
	[DebuggerStepThrough]
	[DebuggerHidden]
	internal unsafe static void* GetStaticFieldAddr(void* pFD)
	{
		object o = null;
		void* enCFieldAddr = GetEnCFieldAddr(ObjectHandleOnStack.Create(ref o), pFD);
		if (enCFieldAddr == null)
		{
			throw new NullReferenceException();
		}
		return enCFieldAddr;
	}
}
