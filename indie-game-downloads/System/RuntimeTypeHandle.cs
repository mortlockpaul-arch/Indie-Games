using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Serialization;
using System.Runtime.Versioning;

namespace System;

[NonVersionable]
public struct RuntimeTypeHandle : IEquatable<RuntimeTypeHandle>, ISerializable
{
	internal struct IntroducedMethodEnumerator
	{
		private bool _firstCall;

		private RuntimeMethodHandleInternal _handle;

		public RuntimeMethodHandleInternal Current => _handle;

		internal IntroducedMethodEnumerator(RuntimeType type)
		{
			_handle = GetFirstIntroducedMethod(type);
			_firstCall = true;
		}

		public bool MoveNext()
		{
			if (_firstCall)
			{
				_firstCall = false;
			}
			else if (_handle.Value != IntPtr.Zero)
			{
				GetNextIntroducedMethod(ref _handle);
			}
			return _handle.Value != IntPtr.Zero;
		}

		public IntroducedMethodEnumerator GetEnumerator()
		{
			return this;
		}
	}

	internal RuntimeType m_type;

	public nint Value => m_type?.m_handle ?? 0;

	internal RuntimeTypeHandle GetNativeHandle()
	{
		return new RuntimeTypeHandle(GetRuntimeTypeChecked());
	}

	internal RuntimeType GetRuntimeTypeChecked()
	{
		return m_type ?? throw new ArgumentNullException(null, SR.Arg_InvalidHandle);
	}

	public static RuntimeTypeHandle FromIntPtr(nint value)
	{
		return new RuntimeTypeHandle(GetRuntimeTypeFromHandleMaybeNull(value));
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetRuntimeTypeFromHandleSlow", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetRuntimeTypeFromHandleSlow")]
	private static extern void GetRuntimeTypeFromHandleSlow(nint handle, ObjectHandleOnStack typeObject);

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static RuntimeType GetRuntimeTypeFromHandleSlow(nint handle)
	{
		RuntimeType o = null;
		GetRuntimeTypeFromHandleSlow(handle, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	internal unsafe static RuntimeType GetRuntimeTypeFromHandle(nint handle)
	{
		TypeHandle typeHandle = new TypeHandle((void*)handle);
		return (typeHandle.IsTypeDesc ? typeHandle.AsTypeDesc()->ExposedClassObject : typeHandle.AsMethodTable()->AuxiliaryData->ExposedClassObject) ?? GetRuntimeTypeFromHandleSlow(handle);
	}

	internal static RuntimeType GetRuntimeTypeFromHandleMaybeNull(nint handle)
	{
		if (handle == IntPtr.Zero)
		{
			return null;
		}
		return GetRuntimeTypeFromHandle(handle);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static RuntimeType GetRuntimeType(MethodTable* pMT)
	{
		return pMT->AuxiliaryData->ExposedClassObject ?? GetRuntimeTypeFromHandleSlow((nint)pMT);
	}

	[Intrinsic]
	public static nint ToIntPtr(RuntimeTypeHandle value)
	{
		return value.Value;
	}

	public static bool operator ==(RuntimeTypeHandle left, object? right)
	{
		return left.Equals(right);
	}

	public static bool operator ==(object? left, RuntimeTypeHandle right)
	{
		return right.Equals(left);
	}

	public static bool operator !=(RuntimeTypeHandle left, object? right)
	{
		return !left.Equals(right);
	}

	public static bool operator !=(object? left, RuntimeTypeHandle right)
	{
		return !right.Equals(left);
	}

	public override int GetHashCode()
	{
		return m_type?.GetHashCode() ?? 0;
	}

	public override bool Equals(object? obj)
	{
		if (obj is RuntimeTypeHandle runtimeTypeHandle)
		{
			return (object)runtimeTypeHandle.m_type == m_type;
		}
		return false;
	}

	public bool Equals(RuntimeTypeHandle handle)
	{
		return (object)handle.m_type == m_type;
	}

	internal RuntimeTypeHandle(RuntimeType type)
	{
		m_type = type;
	}

	internal bool IsNullHandle()
	{
		return m_type == null;
	}

	internal static bool IsTypeDefinition(RuntimeType type)
	{
		CorElementType corElementType = type.GetCorElementType();
		if (((int)corElementType < 1 || (int)corElementType >= 15) && corElementType != CorElementType.ELEMENT_TYPE_VALUETYPE && corElementType != CorElementType.ELEMENT_TYPE_CLASS && corElementType != CorElementType.ELEMENT_TYPE_TYPEDBYREF && corElementType != CorElementType.ELEMENT_TYPE_I && corElementType != CorElementType.ELEMENT_TYPE_U && corElementType != CorElementType.ELEMENT_TYPE_OBJECT)
		{
			return false;
		}
		if (type.IsConstructedGenericType)
		{
			return false;
		}
		return true;
	}

	internal static bool IsPrimitive(RuntimeType type)
	{
		return type.GetCorElementType().IsPrimitiveType();
	}

	internal static bool IsByRef(RuntimeType type)
	{
		return type.GetCorElementType() == CorElementType.ELEMENT_TYPE_BYREF;
	}

	internal static bool IsPointer(RuntimeType type)
	{
		return type.GetCorElementType() == CorElementType.ELEMENT_TYPE_PTR;
	}

	internal static bool IsArray(RuntimeType type)
	{
		CorElementType corElementType = type.GetCorElementType();
		if (corElementType != CorElementType.ELEMENT_TYPE_ARRAY)
		{
			return corElementType == CorElementType.ELEMENT_TYPE_SZARRAY;
		}
		return true;
	}

	internal static bool IsSZArray(RuntimeType type)
	{
		return type.GetCorElementType() == CorElementType.ELEMENT_TYPE_SZARRAY;
	}

	internal static bool IsFunctionPointer(RuntimeType type)
	{
		return type.GetCorElementType() == CorElementType.ELEMENT_TYPE_FNPTR;
	}

	internal static bool HasElementType(RuntimeType type)
	{
		CorElementType corElementType = type.GetCorElementType();
		if (corElementType != CorElementType.ELEMENT_TYPE_ARRAY && corElementType != CorElementType.ELEMENT_TYPE_SZARRAY && corElementType != CorElementType.ELEMENT_TYPE_PTR)
		{
			return corElementType == CorElementType.ELEMENT_TYPE_BYREF;
		}
		return true;
	}

	internal static ReadOnlySpan<nint> CopyRuntimeTypeHandles(RuntimeTypeHandle[] inHandles, Span<nint> stackScratch)
	{
		if (inHandles == null || inHandles.Length == 0)
		{
			return default(ReadOnlySpan<nint>);
		}
		Span<nint> span = ((inHandles.Length <= stackScratch.Length) ? stackScratch.Slice(0, inHandles.Length) : ((Span<nint>)new nint[inHandles.Length]));
		for (int i = 0; i < inHandles.Length; i++)
		{
			span[i] = inHandles[i].Value;
		}
		return span;
	}

	internal static nint[] CopyRuntimeTypeHandles(Type[] inHandles, out int length)
	{
		if (inHandles == null || inHandles.Length == 0)
		{
			length = 0;
			return null;
		}
		nint[] array = new nint[inHandles.Length];
		for (int i = 0; i < inHandles.Length; i++)
		{
			array[i] = inHandles[i].TypeHandle.Value;
		}
		length = array.Length;
		return array;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2067:ParameterDoesntMeetParameterRequirements", Justification = "The parameter 'type' is passed by ref to QCallTypeHandle which only instantiatesthe type using the public parameterless constructor and doesn't modify it")]
	internal unsafe static object CreateInstanceForAnotherGenericParameter([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] RuntimeType type, RuntimeType genericParameter)
	{
		object o = null;
		nint value = genericParameter.TypeHandle.Value;
		CreateInstanceForAnotherGenericParameter(new QCallTypeHandle(ref type), &value, 1, ObjectHandleOnStack.Create(ref o));
		GC.KeepAlive(genericParameter);
		return o;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2067:ParameterDoesntMeetParameterRequirements", Justification = "The parameter 'type' is passed by ref to QCallTypeHandle which only instantiatesthe type using the public parameterless constructor and doesn't modify it")]
	internal unsafe static object CreateInstanceForAnotherGenericParameter([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] RuntimeType type, RuntimeType genericParameter1, RuntimeType genericParameter2)
	{
		object o = null;
		byte* num = stackalloc byte[(int)checked((nuint)2u * (nuint)8u)];
		*(nint*)num = genericParameter1.TypeHandle.Value;
		*(nint*)(num + 8) = genericParameter2.TypeHandle.Value;
		nint* pTypeHandles = (nint*)num;
		CreateInstanceForAnotherGenericParameter(new QCallTypeHandle(ref type), pTypeHandles, 2, ObjectHandleOnStack.Create(ref o));
		GC.KeepAlive(genericParameter1);
		GC.KeepAlive(genericParameter2);
		return o;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_CreateInstanceForAnotherGenericParameter", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_CreateInstanceForAnotherGenericParameter")]
	private unsafe static extern void CreateInstanceForAnotherGenericParameter(QCallTypeHandle baseType, nint* pTypeHandles, int cTypeHandles, ObjectHandleOnStack instantiatedObject);

	internal unsafe static object InternalAlloc(MethodTable* pMT)
	{
		object o = null;
		InternalAlloc(pMT, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	internal unsafe static object InternalAlloc(RuntimeType type)
	{
		object result = InternalAlloc(type.GetNativeTypeHandle().AsMethodTable());
		GC.KeepAlive(type);
		return result;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_InternalAlloc", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_InternalAlloc")]
	private unsafe static extern void InternalAlloc(MethodTable* pMT, ObjectHandleOnStack result);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static object InternalAllocNoChecks(MethodTable* pMT)
	{
		return InternalAllocNoChecks_FastPath(pMT) ?? InternalAllocNoChecksWorker(pMT);
		[MethodImpl(MethodImplOptions.NoInlining)]
		unsafe static object InternalAllocNoChecksWorker(MethodTable* pMT2)
		{
			object o = null;
			InternalAllocNoChecks(pMT2, ObjectHandleOnStack.Create(ref o));
			return o;
		}
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_InternalAllocNoChecks", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_InternalAllocNoChecks")]
	private unsafe static extern void InternalAllocNoChecks(MethodTable* pMT, ObjectHandleOnStack result);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern object InternalAllocNoChecks_FastPath(MethodTable* pMT);

	internal unsafe static void GetActivationInfo(RuntimeType rt, out delegate*<void*, object> pfnAllocator, out void* vAllocatorFirstArg, out delegate*<object, void> pfnRefCtor, out delegate*<ref byte, void> pfnValueCtor, out bool ctorIsPublic)
	{
		delegate*<void*, object> obj = default(delegate*<void*, object>);
		void* ptr = default(void*);
		delegate*<object, void> obj2 = default(delegate*<object, void>);
		delegate*<ref byte, void> obj3 = default(delegate*<ref byte, void>);
		Interop.BOOL bOOL = Interop.BOOL.FALSE;
		GetActivationInfo(ObjectHandleOnStack.Create(ref rt), &obj, &ptr, &obj2, &obj3, &bOOL);
		Unsafe.As<delegate*<void*, object>, IntPtr>(ref pfnAllocator) = (nint)obj;
		vAllocatorFirstArg = ptr;
		Unsafe.As<delegate*<object, void>, IntPtr>(ref pfnRefCtor) = (nint)obj2;
		Unsafe.As<delegate*<ref byte, void>, IntPtr>(ref pfnValueCtor) = (nint)obj3;
		ctorIsPublic = bOOL != Interop.BOOL.FALSE;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetActivationInfo", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetActivationInfo")]
	private unsafe static extern void GetActivationInfo(ObjectHandleOnStack pRuntimeType, delegate*<void*, object>* ppfnAllocator, void** pvAllocatorFirstArg, delegate*<object, void>* ppfnRefCtor, delegate*<ref byte, void>* ppfnValueCtor, Interop.BOOL* pfCtorIsPublic);

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_AllocateComObject", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_AllocateComObject")]
	private unsafe static extern void AllocateComObject(void* pClassFactory, ObjectHandleOnStack result);

	private unsafe static object AllocateComObject(void* pClassFactory)
	{
		object o = null;
		AllocateComObject(pClassFactory, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	internal RuntimeType GetRuntimeType()
	{
		return m_type;
	}

	internal static RuntimeAssembly GetAssembly(RuntimeType type)
	{
		return GetAssemblyIfExists(type) ?? GetAssemblyWorker(type);
		[MethodImpl(MethodImplOptions.NoInlining)]
		static RuntimeAssembly GetAssemblyWorker(RuntimeType o2)
		{
			RuntimeAssembly o = null;
			GetAssemblySlow(ObjectHandleOnStack.Create(ref o2), ObjectHandleOnStack.Create(ref o));
			return o;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RuntimeAssembly GetAssemblyIfExists(RuntimeType type);

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetAssemblySlow", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetAssemblySlow")]
	private static extern void GetAssemblySlow(ObjectHandleOnStack type, ObjectHandleOnStack assembly);

	internal static RuntimeModule GetModule(RuntimeType type)
	{
		return GetModuleIfExists(type) ?? GetModuleWorker(type);
		[MethodImpl(MethodImplOptions.NoInlining)]
		static RuntimeModule GetModuleWorker(RuntimeType o2)
		{
			RuntimeModule o = null;
			GetModuleSlow(ObjectHandleOnStack.Create(ref o2), ObjectHandleOnStack.Create(ref o));
			return o;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RuntimeModule GetModuleIfExists(RuntimeType type);

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetModuleSlow", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetModuleSlow")]
	private static extern void GetModuleSlow(ObjectHandleOnStack type, ObjectHandleOnStack module);

	public ModuleHandle GetModuleHandle()
	{
		if ((object)m_type == null)
		{
			throw new ArgumentNullException(SR.Arg_InvalidHandle);
		}
		return new ModuleHandle(GetModule(m_type));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern TypeAttributes GetAttributes(RuntimeType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern nint GetElementTypeHandle(nint handle);

	internal static RuntimeType GetElementType(RuntimeType type)
	{
		nint elementTypeHandle = GetElementTypeHandle(type.GetUnderlyingNativeHandle());
		if (elementTypeHandle == IntPtr.Zero)
		{
			return null;
		}
		RuntimeType runtimeTypeFromHandle = GetRuntimeTypeFromHandle(elementTypeHandle);
		GC.KeepAlive(type);
		return runtimeTypeFromHandle;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool CompareCanonicalHandles(RuntimeType left, RuntimeType right);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern int GetArrayRank(RuntimeType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern int GetToken(RuntimeType type);

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetMethodAt", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetMethodAt")]
	private unsafe static extern nint GetMethodAt(MethodTable* pMT, int slot);

	internal unsafe static RuntimeMethodHandleInternal GetMethodAt(RuntimeType type, int slot)
	{
		TypeHandle nativeTypeHandle = type.GetNativeTypeHandle();
		if (nativeTypeHandle.IsTypeDesc)
		{
			throw new ArgumentException(SR.Arg_InvalidHandle);
		}
		if (slot < 0)
		{
			throw new ArgumentException(SR.Arg_ArgumentOutOfRangeException);
		}
		return new RuntimeMethodHandleInternal(GetMethodAt(nativeTypeHandle.AsMethodTable(), slot));
	}

	internal static Type[] GetArgumentTypesFromFunctionPointer(RuntimeType type)
	{
		Type[] o = null;
		GetArgumentTypesFromFunctionPointer(new QCallTypeHandle(ref type), ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetArgumentTypesFromFunctionPointer", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetArgumentTypesFromFunctionPointer")]
	private static extern void GetArgumentTypesFromFunctionPointer(QCallTypeHandle type, ObjectHandleOnStack argTypes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool IsUnmanagedFunctionPointer(RuntimeType type);

	internal static IntroducedMethodEnumerator GetIntroducedMethods(RuntimeType type)
	{
		return new IntroducedMethodEnumerator(type);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RuntimeMethodHandleInternal GetFirstIntroducedMethod(RuntimeType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetNextIntroducedMethod(ref RuntimeMethodHandleInternal method);

	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetFields")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static Interop.BOOL GetFields(MethodTable* pMT, Span<nint> data, ref int usedCount)
	{
		Interop.BOOL result;
		fixed (int* _usedCount_native = &usedCount)
		{
			fixed (nint* ptr = &SpanMarshaller<nint, nint>.ManagedToUnmanagedIn.GetPinnableReference(data))
			{
				void* _data_native = ptr;
				result = __PInvoke(pMT, (nint*)_data_native, _usedCount_native);
			}
		}
		return result;
		[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetFields", ExactSpelling = true)]
		unsafe static extern Interop.BOOL __PInvoke(MethodTable* __pMT_native, nint* __data_native, int* __usedCount_native);
	}

	internal unsafe static bool GetFields(RuntimeType type, Span<nint> buffer, out int count)
	{
		TypeHandle nativeTypeHandle = type.GetNativeTypeHandle();
		if (nativeTypeHandle.IsTypeDesc)
		{
			count = 0;
			return true;
		}
		int usedCount = buffer.Length;
		bool result = GetFields(nativeTypeHandle.AsMethodTable(), buffer, ref usedCount) != Interop.BOOL.FALSE;
		GC.KeepAlive(type);
		count = usedCount;
		return result;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetInterfaces", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetInterfaces")]
	private unsafe static extern void GetInterfaces(MethodTable* pMT, ObjectHandleOnStack result);

	internal unsafe static Type[] GetInterfaces(RuntimeType type)
	{
		TypeHandle nativeTypeHandle = type.GetNativeTypeHandle();
		if (nativeTypeHandle.IsTypeDesc)
		{
			return Array.Empty<Type>();
		}
		Type[] o = Array.Empty<Type>();
		GetInterfaces(nativeTypeHandle.AsMethodTable(), ObjectHandleOnStack.Create(ref o));
		GC.KeepAlive(type);
		return o;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetConstraints", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetConstraints")]
	private static extern void GetConstraints(QCallTypeHandle handle, ObjectHandleOnStack types);

	internal Type[] GetConstraints()
	{
		Type[] o = null;
		RuntimeTypeHandle rth = GetNativeHandle();
		GetConstraints(new QCallTypeHandle(ref rth), ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "QCall_GetGCHandleForTypeHandle", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "QCall_GetGCHandleForTypeHandle")]
	private static extern nint GetGCHandle(QCallTypeHandle handle, GCHandleType type);

	internal nint GetGCHandle(GCHandleType type)
	{
		RuntimeTypeHandle rth = GetNativeHandle();
		return GetGCHandle(new QCallTypeHandle(ref rth), type);
	}

	[DllImport("QCall", EntryPoint = "QCall_FreeGCHandleForTypeHandle", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "QCall_FreeGCHandleForTypeHandle")]
	private static extern nint FreeGCHandle(QCallTypeHandle typeHandle, nint objHandle);

	internal nint FreeGCHandle(nint objHandle)
	{
		RuntimeTypeHandle rth = GetNativeHandle();
		return FreeGCHandle(new QCallTypeHandle(ref rth), objHandle);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern int GetNumVirtuals(RuntimeType type);

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetNumVirtualsAndStaticVirtuals", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetNumVirtualsAndStaticVirtuals")]
	private static extern int GetNumVirtualsAndStaticVirtuals(QCallTypeHandle type);

	internal static int GetNumVirtualsAndStaticVirtuals(RuntimeType type)
	{
		return GetNumVirtualsAndStaticVirtuals(new QCallTypeHandle(ref type));
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_VerifyInterfaceIsImplemented", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_VerifyInterfaceIsImplemented")]
	private static extern void VerifyInterfaceIsImplemented(QCallTypeHandle handle, QCallTypeHandle interfaceHandle);

	internal void VerifyInterfaceIsImplemented(RuntimeTypeHandle interfaceHandle)
	{
		RuntimeTypeHandle rth = GetNativeHandle();
		RuntimeTypeHandle rth2 = interfaceHandle.GetNativeHandle();
		VerifyInterfaceIsImplemented(new QCallTypeHandle(ref rth), new QCallTypeHandle(ref rth2));
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetInterfaceMethodImplementation", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetInterfaceMethodImplementation")]
	private static extern RuntimeMethodHandleInternal GetInterfaceMethodImplementation(QCallTypeHandle handle, QCallTypeHandle interfaceHandle, RuntimeMethodHandleInternal interfaceMethodHandle);

	internal RuntimeMethodHandleInternal GetInterfaceMethodImplementation(RuntimeTypeHandle interfaceHandle, RuntimeMethodHandleInternal interfaceMethodHandle)
	{
		RuntimeTypeHandle rth = GetNativeHandle();
		RuntimeTypeHandle rth2 = interfaceHandle.GetNativeHandle();
		return GetInterfaceMethodImplementation(new QCallTypeHandle(ref rth), new QCallTypeHandle(ref rth2), interfaceMethodHandle);
	}

	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_IsVisible")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static bool _IsVisible(QCallTypeHandle typeHandle)
	{
		return __PInvoke(typeHandle) != 0;
		[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_IsVisible", ExactSpelling = true)]
		static extern int __PInvoke(QCallTypeHandle __typeHandle_native);
	}

	internal static bool IsVisible(RuntimeType type)
	{
		return _IsVisible(new QCallTypeHandle(ref type));
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_ConstructName", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_ConstructName")]
	private static extern void ConstructName(QCallTypeHandle handle, TypeNameFormatFlags formatFlags, StringHandleOnStack retString);

	internal string ConstructName(TypeNameFormatFlags formatFlags)
	{
		string s = null;
		RuntimeTypeHandle rth = GetNativeHandle();
		ConstructName(new QCallTypeHandle(ref rth), formatFlags, new StringHandleOnStack(ref s));
		return s;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void* GetUtf8NameInternal(MethodTable* pMT);

	internal unsafe static MdUtf8String GetUtf8Name(RuntimeType type)
	{
		TypeHandle nativeTypeHandle = type.GetNativeTypeHandle();
		if (nativeTypeHandle.IsTypeDesc || nativeTypeHandle.AsMethodTable()->IsArray)
		{
			throw new ArgumentException(SR.Arg_InvalidHandle);
		}
		void* utf8NameInternal = GetUtf8NameInternal(nativeTypeHandle.AsMethodTable());
		if (utf8NameInternal == null)
		{
			throw new BadImageFormatException();
		}
		return new MdUtf8String(utf8NameInternal);
	}

	internal static bool CanCastTo(RuntimeType type, RuntimeType target)
	{
		bool result = TypeHandle.CanCastToForReflection(type.GetNativeTypeHandle(), target.GetNativeTypeHandle());
		GC.KeepAlive(type);
		GC.KeepAlive(target);
		return result;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetDeclaringTypeHandleForGenericVariable", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetDeclaringTypeHandleForGenericVariable")]
	private static extern nint GetDeclaringTypeHandleForGenericVariable(nint typeHandle);

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetDeclaringTypeHandle", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetDeclaringTypeHandle")]
	private static extern nint GetDeclaringTypeHandle(nint typeHandle);

	internal static RuntimeType GetDeclaringType(RuntimeType type)
	{
		nint num = IntPtr.Zero;
		TypeHandle nativeTypeHandle = type.GetNativeTypeHandle();
		if (nativeTypeHandle.IsTypeDesc)
		{
			CorElementType corElementType = (CorElementType)nativeTypeHandle.GetCorElementType();
			if ((corElementType == CorElementType.ELEMENT_TYPE_VAR || corElementType == CorElementType.ELEMENT_TYPE_MVAR) ? true : false)
			{
				num = GetDeclaringTypeHandleForGenericVariable(type.GetUnderlyingNativeHandle());
			}
		}
		else
		{
			num = GetDeclaringTypeHandle(type.GetUnderlyingNativeHandle());
		}
		if (num == IntPtr.Zero)
		{
			return null;
		}
		RuntimeType runtimeTypeFromHandle = GetRuntimeTypeFromHandle(num);
		GC.KeepAlive(type);
		return runtimeTypeFromHandle;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetDeclaringMethodForGenericParameter", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetDeclaringMethodForGenericParameter")]
	private static extern void GetDeclaringMethodForGenericParameter(QCallTypeHandle typeHandle, ObjectHandleOnStack result);

	internal static IRuntimeMethodInfo GetDeclaringMethodForGenericParameter(RuntimeType type)
	{
		IRuntimeMethodInfo o = null;
		GetDeclaringMethodForGenericParameter(new QCallTypeHandle(ref type), ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetInstantiation", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetInstantiation")]
	internal static extern void GetInstantiation(QCallTypeHandle type, ObjectHandleOnStack types, Interop.BOOL fAsRuntimeTypeArray);

	internal RuntimeType[] GetInstantiationInternal()
	{
		RuntimeType[] o = null;
		RuntimeTypeHandle rth = GetNativeHandle();
		GetInstantiation(new QCallTypeHandle(ref rth), ObjectHandleOnStack.Create(ref o), Interop.BOOL.TRUE);
		return o;
	}

	internal Type[] GetInstantiationPublic()
	{
		Type[] o = null;
		RuntimeTypeHandle rth = GetNativeHandle();
		GetInstantiation(new QCallTypeHandle(ref rth), ObjectHandleOnStack.Create(ref o), Interop.BOOL.FALSE);
		return o;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_Instantiate", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_Instantiate")]
	private unsafe static extern void Instantiate(QCallTypeHandle handle, nint* pInst, int numGenericArgs, ObjectHandleOnStack type);

	internal unsafe RuntimeType Instantiate(RuntimeType inst)
	{
		nint value = inst.TypeHandle.Value;
		RuntimeType o = null;
		RuntimeTypeHandle rth = GetNativeHandle();
		Instantiate(new QCallTypeHandle(ref rth), &value, 1, ObjectHandleOnStack.Create(ref o));
		GC.KeepAlive(inst);
		return o;
	}

	internal unsafe RuntimeType Instantiate(Type[] inst)
	{
		int length;
		fixed (nint* pInst = CopyRuntimeTypeHandles(inst, out length))
		{
			RuntimeType o = null;
			RuntimeTypeHandle rth = GetNativeHandle();
			Instantiate(new QCallTypeHandle(ref rth), pInst, length, ObjectHandleOnStack.Create(ref o));
			GC.KeepAlive(inst);
			return o;
		}
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_MakeArray", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_MakeArray")]
	private static extern void MakeArray(QCallTypeHandle handle, int rank, ObjectHandleOnStack type);

	internal RuntimeType MakeArray(int rank)
	{
		RuntimeType o = null;
		RuntimeTypeHandle rth = GetNativeHandle();
		MakeArray(new QCallTypeHandle(ref rth), rank, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_MakeSZArray", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_MakeSZArray")]
	private static extern void MakeSZArray(QCallTypeHandle handle, ObjectHandleOnStack type);

	internal RuntimeType MakeSZArray()
	{
		RuntimeType o = null;
		RuntimeTypeHandle rth = GetNativeHandle();
		MakeSZArray(new QCallTypeHandle(ref rth), ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_MakeByRef", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_MakeByRef")]
	private static extern void MakeByRef(QCallTypeHandle handle, ObjectHandleOnStack type);

	internal RuntimeType MakeByRef()
	{
		RuntimeType o = null;
		RuntimeTypeHandle rth = GetNativeHandle();
		MakeByRef(new QCallTypeHandle(ref rth), ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_MakePointer", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_MakePointer")]
	private static extern void MakePointer(QCallTypeHandle handle, ObjectHandleOnStack type);

	internal RuntimeType MakePointer()
	{
		RuntimeType o = null;
		RuntimeTypeHandle rth = GetNativeHandle();
		MakePointer(new QCallTypeHandle(ref rth), ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_IsCollectible", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_IsCollectible")]
	internal static extern Interop.BOOL IsCollectible(QCallTypeHandle handle);

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_GetGenericTypeDefinition", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_GetGenericTypeDefinition")]
	internal static extern void GetGenericTypeDefinition(QCallTypeHandle type, ObjectHandleOnStack retType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool IsGenericVariable(RuntimeType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetGenericVariableIndex(RuntimeType type);

	internal int GetGenericVariableIndex()
	{
		RuntimeType runtimeTypeChecked = GetRuntimeTypeChecked();
		if (!IsGenericVariable(runtimeTypeChecked))
		{
			throw new InvalidOperationException(SR.Arg_NotGenericParameter);
		}
		return GetGenericVariableIndex(runtimeTypeChecked);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool ContainsGenericVariables(RuntimeType handle);

	internal bool ContainsGenericVariables()
	{
		return ContainsGenericVariables(GetRuntimeTypeChecked());
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_SatisfiesConstraints", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_SatisfiesConstraints")]
	private static extern Interop.BOOL SatisfiesConstraints(QCallTypeHandle paramType, QCallTypeHandle pTypeContext, RuntimeMethodHandleInternal pMethodContext, QCallTypeHandle toType);

	internal static bool SatisfiesConstraints(RuntimeType paramType, RuntimeType typeContext, RuntimeMethodInfo methodContext, RuntimeType toType)
	{
		RuntimeMethodHandleInternal pMethodContext = ((IRuntimeMethodInfo)methodContext)?.Value ?? RuntimeMethodHandleInternal.EmptyHandle;
		bool result = SatisfiesConstraints(new QCallTypeHandle(ref paramType), new QCallTypeHandle(ref typeContext), pMethodContext, new QCallTypeHandle(ref toType)) != Interop.BOOL.FALSE;
		GC.KeepAlive(methodContext);
		return result;
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_RegisterCollectibleTypeDependency", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_RegisterCollectibleTypeDependency")]
	private static extern void RegisterCollectibleTypeDependency(QCallTypeHandle type, QCallAssembly assembly);

	internal static void RegisterCollectibleTypeDependency(RuntimeType type, RuntimeAssembly assembly)
	{
		RegisterCollectibleTypeDependency(new QCallTypeHandle(ref type), new QCallAssembly(ref assembly));
	}

	[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		throw new PlatformNotSupportedException();
	}

	[DllImport("QCall", EntryPoint = "RuntimeTypeHandle_IsEquivalentTo", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeTypeHandle_IsEquivalentTo")]
	private static extern Interop.BOOL IsEquivalentTo(QCallTypeHandle rtType1, QCallTypeHandle rtType2);

	internal static bool IsEquivalentTo(RuntimeType rtType1, RuntimeType rtType2)
	{
		return IsEquivalentTo(new QCallTypeHandle(ref rtType1), new QCallTypeHandle(ref rtType2)) == Interop.BOOL.TRUE;
	}
}
