using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;

namespace System;

[NonVersionable]
public struct RuntimeMethodHandle : IEquatable<RuntimeMethodHandle>, ISerializable
{
	private readonly IRuntimeMethodInfo m_value;

	public nint Value
	{
		get
		{
			if (m_value == null)
			{
				return IntPtr.Zero;
			}
			return m_value.Value.Value;
		}
	}

	internal static IRuntimeMethodInfo EnsureNonNullMethodInfo(IRuntimeMethodInfo method)
	{
		if (method == null)
		{
			throw new ArgumentNullException(null, SR.Arg_InvalidHandle);
		}
		return method;
	}

	internal RuntimeMethodHandle(IRuntimeMethodInfo method)
	{
		m_value = method;
	}

	internal IRuntimeMethodInfo GetMethodInfo()
	{
		return m_value;
	}

	[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		throw new PlatformNotSupportedException();
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Value);
	}

	public override bool Equals(object? obj)
	{
		if (!(obj is RuntimeMethodHandle runtimeMethodHandle))
		{
			return false;
		}
		return runtimeMethodHandle.Value == Value;
	}

	public static RuntimeMethodHandle FromIntPtr(nint value)
	{
		RuntimeMethodHandleInternal runtimeMethodHandleInternal = new RuntimeMethodHandleInternal(value);
		return new RuntimeMethodHandle(new RuntimeMethodInfoStub(runtimeMethodHandleInternal, GetLoaderAllocator(runtimeMethodHandleInternal)));
	}

	public static nint ToIntPtr(RuntimeMethodHandle value)
	{
		return value.Value;
	}

	public static bool operator ==(RuntimeMethodHandle left, RuntimeMethodHandle right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(RuntimeMethodHandle left, RuntimeMethodHandle right)
	{
		return !left.Equals(right);
	}

	public bool Equals(RuntimeMethodHandle handle)
	{
		return handle.Value == Value;
	}

	internal bool IsNullHandle()
	{
		return m_value == null;
	}

	[DllImport("QCall", EntryPoint = "RuntimeMethodHandle_GetFunctionPointer", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeMethodHandle_GetFunctionPointer")]
	internal static extern nint GetFunctionPointer(RuntimeMethodHandleInternal handle);

	public nint GetFunctionPointer()
	{
		nint functionPointer = GetFunctionPointer(EnsureNonNullMethodInfo(m_value).Value);
		GC.KeepAlive(m_value);
		return functionPointer;
	}

	[DllImport("QCall", EntryPoint = "RuntimeMethodHandle_GetIsCollectible", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeMethodHandle_GetIsCollectible")]
	internal static extern Interop.BOOL GetIsCollectible(RuntimeMethodHandleInternal handle);

	[DllImport("QCall", EntryPoint = "RuntimeMethodHandle_IsCAVisibleFromDecoratedType", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeMethodHandle_IsCAVisibleFromDecoratedType")]
	internal static extern Interop.BOOL IsCAVisibleFromDecoratedType(QCallTypeHandle attrTypeHandle, RuntimeMethodHandleInternal attrCtor, QCallTypeHandle sourceTypeHandle, QCallModule sourceModule);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern MethodAttributes GetAttributes(RuntimeMethodHandleInternal method);

	internal static MethodAttributes GetAttributes(IRuntimeMethodInfo method)
	{
		MethodAttributes attributes = GetAttributes(method.Value);
		GC.KeepAlive(method);
		return attributes;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern MethodImplAttributes GetImplAttributes(IRuntimeMethodInfo method);

	[DllImport("QCall", EntryPoint = "RuntimeMethodHandle_ConstructInstantiation", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeMethodHandle_ConstructInstantiation")]
	private static extern void ConstructInstantiation(RuntimeMethodHandleInternal method, TypeNameFormatFlags format, StringHandleOnStack retString);

	internal static string ConstructInstantiation(IRuntimeMethodInfo method, TypeNameFormatFlags format)
	{
		string s = null;
		IRuntimeMethodInfo runtimeMethodInfo = EnsureNonNullMethodInfo(method);
		ConstructInstantiation(runtimeMethodInfo.Value, format, new StringHandleOnStack(ref s));
		GC.KeepAlive(runtimeMethodInfo);
		return s;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern MethodTable* GetMethodTable(RuntimeMethodHandleInternal method);

	internal unsafe static RuntimeType GetDeclaringType(RuntimeMethodHandleInternal method)
	{
		return RuntimeTypeHandle.GetRuntimeType(GetMethodTable(method));
	}

	internal static RuntimeType GetDeclaringType(IRuntimeMethodInfo method)
	{
		RuntimeType declaringType = GetDeclaringType(method.Value);
		GC.KeepAlive(method);
		return declaringType;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern int GetSlot(RuntimeMethodHandleInternal method);

	internal static int GetSlot(IRuntimeMethodInfo method)
	{
		int slot = GetSlot(method.Value);
		GC.KeepAlive(method);
		return slot;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern int GetMethodDef(RuntimeMethodHandleInternal method);

	internal static int GetMethodDef(IRuntimeMethodInfo method)
	{
		int methodDef = GetMethodDef(method.Value);
		GC.KeepAlive(method);
		return methodDef;
	}

	internal static string GetName(RuntimeMethodHandleInternal method)
	{
		return GetUtf8Name(method).ToString();
	}

	internal static string GetName(IRuntimeMethodInfo method)
	{
		string name = GetName(method.Value);
		GC.KeepAlive(method);
		return name;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void* GetUtf8NameInternal(RuntimeMethodHandleInternal method);

	internal unsafe static MdUtf8String GetUtf8Name(RuntimeMethodHandleInternal method)
	{
		void* utf8NameInternal = GetUtf8NameInternal(method);
		if (utf8NameInternal == null)
		{
			throw new BadImageFormatException();
		}
		return new MdUtf8String(utf8NameInternal);
	}

	[DllImport("QCall", EntryPoint = "RuntimeMethodHandle_InvokeMethod", ExactSpelling = true)]
	[DebuggerStepThrough]
	[DebuggerHidden]
	[LibraryImport("QCall", EntryPoint = "RuntimeMethodHandle_InvokeMethod")]
	private unsafe static extern void InvokeMethod(ObjectHandleOnStack target, void** arguments, ObjectHandleOnStack sig, Interop.BOOL isConstructor, ObjectHandleOnStack result);

	[DebuggerStepThrough]
	[DebuggerHidden]
	internal unsafe static object InvokeMethod(object target, void** arguments, Signature sig, bool isConstructor)
	{
		object o = null;
		InvokeMethod(ObjectHandleOnStack.Create(ref target), arguments, ObjectHandleOnStack.Create(ref sig), isConstructor ? Interop.BOOL.TRUE : Interop.BOOL.FALSE, ObjectHandleOnStack.Create(ref o));
		return o;
	}

	internal unsafe static object ReboxFromNullable(object src)
	{
		if (src == null)
		{
			return null;
		}
		MethodTable* methodTable = RuntimeHelpers.GetMethodTable(src);
		if (!methodTable->IsNullable)
		{
			return src;
		}
		return CastHelpers.ReboxFromNullable(methodTable, src);
	}

	internal unsafe static object ReboxToNullable(object src, RuntimeType destNullableType)
	{
		MethodTable* ptr = destNullableType.GetNativeTypeHandle().AsMethodTable();
		object obj = RuntimeTypeHandle.InternalAlloc(ptr);
		GC.KeepAlive(destNullableType);
		CastHelpers.Unbox_Nullable(ref obj.GetRawData(), ptr, src);
		return obj;
	}

	[DllImport("QCall", EntryPoint = "RuntimeMethodHandle_GetMethodInstantiation", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeMethodHandle_GetMethodInstantiation")]
	private static extern void GetMethodInstantiation(RuntimeMethodHandleInternal method, ObjectHandleOnStack types, Interop.BOOL fAsRuntimeTypeArray);

	internal static RuntimeType[] GetMethodInstantiationInternal(IRuntimeMethodInfo method)
	{
		RuntimeType[] o = null;
		GetMethodInstantiation(EnsureNonNullMethodInfo(method).Value, ObjectHandleOnStack.Create(ref o), Interop.BOOL.TRUE);
		GC.KeepAlive(method);
		return o;
	}

	internal static RuntimeType[] GetMethodInstantiationInternal(RuntimeMethodHandleInternal method)
	{
		RuntimeType[] o = null;
		GetMethodInstantiation(method, ObjectHandleOnStack.Create(ref o), Interop.BOOL.TRUE);
		return o;
	}

	internal static Type[] GetMethodInstantiationPublic(IRuntimeMethodInfo method)
	{
		RuntimeType[] o = null;
		GetMethodInstantiation(EnsureNonNullMethodInfo(method).Value, ObjectHandleOnStack.Create(ref o), Interop.BOOL.FALSE);
		GC.KeepAlive(method);
		return o;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool HasMethodInstantiation(RuntimeMethodHandleInternal method);

	internal static bool HasMethodInstantiation(IRuntimeMethodInfo method)
	{
		bool result = HasMethodInstantiation(method.Value);
		GC.KeepAlive(method);
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern RuntimeMethodHandleInternal GetStubIfNeededInternal(RuntimeMethodHandleInternal method, RuntimeType declaringType);

	[DllImport("QCall", EntryPoint = "RuntimeMethodHandle_GetStubIfNeededSlow", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeMethodHandle_GetStubIfNeededSlow")]
	private static extern RuntimeMethodHandleInternal GetStubIfNeededSlow(RuntimeMethodHandleInternal method, QCallTypeHandle declaringTypeHandle, ObjectHandleOnStack methodInstantiation);

	internal static RuntimeMethodHandleInternal GetStubIfNeeded(RuntimeMethodHandleInternal method, RuntimeType declaringType, RuntimeType[] methodInstantiation)
	{
		if (methodInstantiation == null)
		{
			RuntimeMethodHandleInternal stubIfNeededInternal = GetStubIfNeededInternal(method, declaringType);
			if (!stubIfNeededInternal.IsNullHandle())
			{
				return stubIfNeededInternal;
			}
		}
		return GetStubIfNeededWorker(method, declaringType, methodInstantiation);
		[MethodImpl(MethodImplOptions.NoInlining)]
		static RuntimeMethodHandleInternal GetStubIfNeededWorker(RuntimeMethodHandleInternal method2, RuntimeType type, RuntimeType[] o)
		{
			return GetStubIfNeededSlow(method2, new QCallTypeHandle(ref type), ObjectHandleOnStack.Create(ref o));
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern RuntimeMethodHandleInternal GetMethodFromCanonical(RuntimeMethodHandleInternal method, RuntimeType declaringType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool IsGenericMethodDefinition(RuntimeMethodHandleInternal method);

	internal static bool IsGenericMethodDefinition(IRuntimeMethodInfo method)
	{
		bool result = IsGenericMethodDefinition(method.Value);
		GC.KeepAlive(method);
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTypicalMethodDefinition(IRuntimeMethodInfo method);

	[DllImport("QCall", EntryPoint = "RuntimeMethodHandle_GetTypicalMethodDefinition", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeMethodHandle_GetTypicalMethodDefinition")]
	private static extern void GetTypicalMethodDefinition(RuntimeMethodHandleInternal method, ObjectHandleOnStack outMethod);

	internal static IRuntimeMethodInfo GetTypicalMethodDefinition(IRuntimeMethodInfo method)
	{
		if (!IsTypicalMethodDefinition(method))
		{
			GetTypicalMethodDefinition(method.Value, ObjectHandleOnStack.Create(ref method));
			GC.KeepAlive(method);
		}
		return method;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetGenericParameterCount(RuntimeMethodHandleInternal method);

	internal static int GetGenericParameterCount(IRuntimeMethodInfo method)
	{
		return GetGenericParameterCount(method.Value);
	}

	[DllImport("QCall", EntryPoint = "RuntimeMethodHandle_StripMethodInstantiation", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeMethodHandle_StripMethodInstantiation")]
	private static extern void StripMethodInstantiation(RuntimeMethodHandleInternal method, ObjectHandleOnStack outMethod);

	internal static IRuntimeMethodInfo StripMethodInstantiation(IRuntimeMethodInfo method)
	{
		IRuntimeMethodInfo o = method;
		StripMethodInstantiation(method.Value, ObjectHandleOnStack.Create(ref o));
		GC.KeepAlive(method);
		return o;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool IsDynamicMethod(RuntimeMethodHandleInternal method);

	[DllImport("QCall", EntryPoint = "RuntimeMethodHandle_Destroy", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeMethodHandle_Destroy")]
	internal static extern void Destroy(RuntimeMethodHandleInternal method);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern Resolver GetResolver(RuntimeMethodHandleInternal method);

	[DllImport("QCall", EntryPoint = "RuntimeMethodHandle_GetMethodBody", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "RuntimeMethodHandle_GetMethodBody")]
	private static extern void GetMethodBody(RuntimeMethodHandleInternal method, QCallTypeHandle declaringType, ObjectHandleOnStack result);

	internal static RuntimeMethodBody GetMethodBody(IRuntimeMethodInfo method, RuntimeType declaringType)
	{
		RuntimeMethodBody o = null;
		GetMethodBody(method.Value, new QCallTypeHandle(ref declaringType), ObjectHandleOnStack.Create(ref o));
		GC.KeepAlive(method);
		return o;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool IsConstructor(RuntimeMethodHandleInternal method);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LoaderAllocator GetLoaderAllocatorInternal(RuntimeMethodHandleInternal method);

	internal static LoaderAllocator GetLoaderAllocator(RuntimeMethodHandleInternal method)
	{
		if (method.IsNullHandle())
		{
			throw new ArgumentNullException(SR.Arg_InvalidHandle);
		}
		return GetLoaderAllocatorInternal(method);
	}
}
