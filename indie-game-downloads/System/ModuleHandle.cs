using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System;

public struct ModuleHandle : IEquatable<ModuleHandle>
{
	public static readonly ModuleHandle EmptyHandle;

	private readonly RuntimeModule m_ptr;

	public int MDStreamVersion => GetMDStreamVersion(GetRuntimeModule());

	internal ModuleHandle(RuntimeModule module)
	{
		m_ptr = module;
	}

	internal RuntimeModule GetRuntimeModule()
	{
		return m_ptr;
	}

	public override int GetHashCode()
	{
		if (!(m_ptr != null))
		{
			return 0;
		}
		return m_ptr.GetHashCode();
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (!(obj is ModuleHandle))
		{
			return false;
		}
		return ((ModuleHandle)obj).m_ptr == m_ptr;
	}

	public bool Equals(ModuleHandle handle)
	{
		return handle.m_ptr == m_ptr;
	}

	public static bool operator ==(ModuleHandle left, ModuleHandle right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ModuleHandle left, ModuleHandle right)
	{
		return !left.Equals(right);
	}

	[LibraryImport("QCall", EntryPoint = "ModuleHandle_GetDynamicMethod", StringMarshalling = StringMarshalling.Utf8)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static void GetDynamicMethod(QCallModule module, string name, byte[] sig, int sigLen, ObjectHandleOnStack resolver, ObjectHandleOnStack result)
	{
		byte* ptr = default(byte*);
		Utf8StringMarshaller.ManagedToUnmanagedIn managedToUnmanagedIn = default(Utf8StringMarshaller.ManagedToUnmanagedIn);
		try
		{
			Span<byte> buffer = stackalloc byte[256];
			managedToUnmanagedIn.FromManaged(name, buffer);
			fixed (byte* ptr2 = &ArrayMarshaller<byte, byte>.ManagedToUnmanagedIn.GetPinnableReference(sig))
			{
				void* _sig_native = ptr2;
				ptr = managedToUnmanagedIn.ToUnmanaged();
				__PInvoke(module, ptr, (byte*)_sig_native, sigLen, resolver, result);
			}
		}
		finally
		{
			managedToUnmanagedIn.Free();
		}
		[DllImport("QCall", EntryPoint = "ModuleHandle_GetDynamicMethod", ExactSpelling = true)]
		unsafe static extern void __PInvoke(QCallModule __module_native, byte* __name_native, byte* __sig_native, int __sigLen_native, ObjectHandleOnStack __resolver_native, ObjectHandleOnStack __result_native);
	}

	internal static IRuntimeMethodInfo GetDynamicMethod(RuntimeModule module, string name, byte[] sig, Resolver resolver)
	{
		IRuntimeMethodInfo o = null;
		GetDynamicMethod(new QCallModule(ref module), name, sig, sig.Length, ObjectHandleOnStack.Create(ref resolver), ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "ModuleHandle_GetToken", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ModuleHandle_GetToken")]
	[SuppressGCTransition]
	private static extern int GetToken(QCallModule module);

	internal static int GetToken(RuntimeModule module)
	{
		return GetToken(new QCallModule(ref module));
	}

	private static void ValidateModulePointer(RuntimeModule module)
	{
		if ((object)module == null)
		{
			ThrowInvalidOperationException();
		}
		[StackTraceHidden]
		[DoesNotReturn]
		static void ThrowInvalidOperationException()
		{
			throw new InvalidOperationException(SR.InvalidOperation_NullModuleHandle);
		}
	}

	[RequiresUnreferencedCode("Trimming changes metadata tokens")]
	public RuntimeTypeHandle GetRuntimeTypeHandleFromMetadataToken(int typeToken)
	{
		return ResolveTypeHandle(typeToken);
	}

	[RequiresUnreferencedCode("Trimming changes metadata tokens")]
	public RuntimeTypeHandle ResolveTypeHandle(int typeToken)
	{
		return ResolveTypeHandle(typeToken, null, null);
	}

	[RequiresUnreferencedCode("Trimming changes metadata tokens")]
	public unsafe RuntimeTypeHandle ResolveTypeHandle(int typeToken, RuntimeTypeHandle[]? typeInstantiationContext, RuntimeTypeHandle[]? methodInstantiationContext)
	{
		RuntimeModule module = GetRuntimeModule();
		ValidateModulePointer(module);
		ReadOnlySpan<nint> readOnlySpan = default(ReadOnlySpan<nint>);
		ReadOnlySpan<nint> readOnlySpan2 = default(ReadOnlySpan<nint>);
		if (typeInstantiationContext != null && typeInstantiationContext.Length != 0)
		{
			typeInstantiationContext = (RuntimeTypeHandle[])typeInstantiationContext.Clone();
			RuntimeTypeHandle[] inHandles = typeInstantiationContext;
			Span<nint> stackScratch = new Span<nint>(stackalloc byte[(int)checked((nuint)8u * (nuint)8u)], 8);
			readOnlySpan = RuntimeTypeHandle.CopyRuntimeTypeHandles(inHandles, stackScratch);
		}
		if (methodInstantiationContext != null && methodInstantiationContext.Length != 0)
		{
			methodInstantiationContext = (RuntimeTypeHandle[])methodInstantiationContext.Clone();
			RuntimeTypeHandle[] inHandles = methodInstantiationContext;
			Span<nint> stackScratch = new Span<nint>(stackalloc byte[(int)checked((nuint)8u * (nuint)8u)], 8);
			readOnlySpan2 = RuntimeTypeHandle.CopyRuntimeTypeHandles(inHandles, stackScratch);
		}
		fixed (nint* typeInstArgs = readOnlySpan)
		{
			fixed (nint* methodInstArgs = readOnlySpan2)
			{
				try
				{
					RuntimeType o = null;
					ResolveType(new QCallModule(ref module), typeToken, typeInstArgs, readOnlySpan.Length, methodInstArgs, readOnlySpan2.Length, ObjectHandleOnStack.Create(ref o));
					GC.KeepAlive(typeInstantiationContext);
					GC.KeepAlive(methodInstantiationContext);
					return new RuntimeTypeHandle(o);
				}
				catch (Exception)
				{
					if (!module.MetadataImport.IsValidToken(typeToken))
					{
						throw new ArgumentOutOfRangeException("typeToken", SR.Format(SR.Argument_InvalidToken, typeToken, new ModuleHandle(module)));
					}
					throw;
				}
			}
		}
	}

	[DllImport("QCall", EntryPoint = "ModuleHandle_ResolveType", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ModuleHandle_ResolveType")]
	private unsafe static extern void ResolveType(QCallModule module, int typeToken, nint* typeInstArgs, int typeInstCount, nint* methodInstArgs, int methodInstCount, ObjectHandleOnStack type);

	[RequiresUnreferencedCode("Trimming changes metadata tokens")]
	public RuntimeMethodHandle GetRuntimeMethodHandleFromMetadataToken(int methodToken)
	{
		return ResolveMethodHandle(methodToken);
	}

	[RequiresUnreferencedCode("Trimming changes metadata tokens")]
	public RuntimeMethodHandle ResolveMethodHandle(int methodToken)
	{
		return ResolveMethodHandle(methodToken, null, null);
	}

	[RequiresUnreferencedCode("Trimming changes metadata tokens")]
	public unsafe RuntimeMethodHandle ResolveMethodHandle(int methodToken, RuntimeTypeHandle[]? typeInstantiationContext, RuntimeTypeHandle[]? methodInstantiationContext)
	{
		RuntimeModule runtimeModule = GetRuntimeModule();
		typeInstantiationContext = (RuntimeTypeHandle[])typeInstantiationContext?.Clone();
		methodInstantiationContext = (RuntimeTypeHandle[])methodInstantiationContext?.Clone();
		RuntimeTypeHandle[] inHandles = typeInstantiationContext;
		Span<nint> stackScratch = new Span<nint>(stackalloc byte[(int)checked((nuint)8u * (nuint)8u)], 8);
		ReadOnlySpan<nint> typeInstantiationContext2 = RuntimeTypeHandle.CopyRuntimeTypeHandles(inHandles, stackScratch);
		inHandles = methodInstantiationContext;
		stackScratch = new Span<nint>(stackalloc byte[(int)checked((nuint)8u * (nuint)8u)], 8);
		ReadOnlySpan<nint> methodInstantiationContext2 = RuntimeTypeHandle.CopyRuntimeTypeHandles(inHandles, stackScratch);
		RuntimeMethodHandleInternal runtimeMethodHandleInternal = ResolveMethodHandleInternal(runtimeModule, methodToken, typeInstantiationContext2, methodInstantiationContext2);
		RuntimeMethodInfoStub method = new RuntimeMethodInfoStub(runtimeMethodHandleInternal, RuntimeMethodHandle.GetLoaderAllocator(runtimeMethodHandleInternal));
		GC.KeepAlive(typeInstantiationContext);
		GC.KeepAlive(methodInstantiationContext);
		return new RuntimeMethodHandle(method);
	}

	internal unsafe static RuntimeMethodHandleInternal ResolveMethodHandleInternal(RuntimeModule module, int methodToken, ReadOnlySpan<nint> typeInstantiationContext, ReadOnlySpan<nint> methodInstantiationContext)
	{
		ValidateModulePointer(module);
		try
		{
			fixed (nint* typeInstArgs = typeInstantiationContext)
			{
				fixed (nint* methodInstArgs = methodInstantiationContext)
				{
					return ResolveMethod(new QCallModule(ref module), methodToken, typeInstArgs, typeInstantiationContext.Length, methodInstArgs, methodInstantiationContext.Length);
				}
			}
		}
		catch (Exception)
		{
			if (!module.MetadataImport.IsValidToken(methodToken))
			{
				throw new ArgumentOutOfRangeException("methodToken", SR.Format(SR.Argument_InvalidToken, methodToken, new ModuleHandle(module)));
			}
			throw;
		}
	}

	[DllImport("QCall", EntryPoint = "ModuleHandle_ResolveMethod", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ModuleHandle_ResolveMethod")]
	private unsafe static extern RuntimeMethodHandleInternal ResolveMethod(QCallModule module, int methodToken, nint* typeInstArgs, int typeInstCount, nint* methodInstArgs, int methodInstCount);

	[RequiresUnreferencedCode("Trimming changes metadata tokens")]
	public RuntimeFieldHandle GetRuntimeFieldHandleFromMetadataToken(int fieldToken)
	{
		return ResolveFieldHandle(fieldToken);
	}

	[RequiresUnreferencedCode("Trimming changes metadata tokens")]
	public RuntimeFieldHandle ResolveFieldHandle(int fieldToken)
	{
		return ResolveFieldHandle(fieldToken, null, null);
	}

	[RequiresUnreferencedCode("Trimming changes metadata tokens")]
	public unsafe RuntimeFieldHandle ResolveFieldHandle(int fieldToken, RuntimeTypeHandle[]? typeInstantiationContext, RuntimeTypeHandle[]? methodInstantiationContext)
	{
		RuntimeModule module = GetRuntimeModule();
		ValidateModulePointer(module);
		ReadOnlySpan<nint> readOnlySpan = default(ReadOnlySpan<nint>);
		ReadOnlySpan<nint> readOnlySpan2 = default(ReadOnlySpan<nint>);
		if (typeInstantiationContext != null && typeInstantiationContext.Length != 0)
		{
			typeInstantiationContext = (RuntimeTypeHandle[])typeInstantiationContext.Clone();
			RuntimeTypeHandle[] inHandles = typeInstantiationContext;
			Span<nint> stackScratch = new Span<nint>(stackalloc byte[(int)checked((nuint)8u * (nuint)8u)], 8);
			readOnlySpan = RuntimeTypeHandle.CopyRuntimeTypeHandles(inHandles, stackScratch);
		}
		if (methodInstantiationContext != null && methodInstantiationContext.Length != 0)
		{
			methodInstantiationContext = (RuntimeTypeHandle[])methodInstantiationContext.Clone();
			RuntimeTypeHandle[] inHandles = methodInstantiationContext;
			Span<nint> stackScratch = new Span<nint>(stackalloc byte[(int)checked((nuint)8u * (nuint)8u)], 8);
			readOnlySpan2 = RuntimeTypeHandle.CopyRuntimeTypeHandles(inHandles, stackScratch);
		}
		fixed (nint* typeInstArgs = readOnlySpan)
		{
			fixed (nint* methodInstArgs = readOnlySpan2)
			{
				try
				{
					IRuntimeFieldInfo o = null;
					ResolveField(new QCallModule(ref module), fieldToken, typeInstArgs, readOnlySpan.Length, methodInstArgs, readOnlySpan2.Length, ObjectHandleOnStack.Create(ref o));
					GC.KeepAlive(typeInstantiationContext);
					GC.KeepAlive(methodInstantiationContext);
					return new RuntimeFieldHandle(o);
				}
				catch (Exception)
				{
					if (!module.MetadataImport.IsValidToken(fieldToken))
					{
						throw new ArgumentOutOfRangeException("fieldToken", SR.Format(SR.Argument_InvalidToken, fieldToken, new ModuleHandle(module)));
					}
					throw;
				}
			}
		}
	}

	[DllImport("QCall", EntryPoint = "ModuleHandle_ResolveField", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ModuleHandle_ResolveField")]
	private unsafe static extern void ResolveField(QCallModule module, int fieldToken, nint* typeInstArgs, int typeInstCount, nint* methodInstArgs, int methodInstCount, ObjectHandleOnStack retField);

	[DllImport("QCall", EntryPoint = "ModuleHandle_GetModuleType", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ModuleHandle_GetModuleType")]
	internal static extern void GetModuleType(QCallModule handle, ObjectHandleOnStack type);

	internal static RuntimeType GetModuleType(RuntimeModule module)
	{
		RuntimeType o = null;
		GetModuleType(new QCallModule(ref module), ObjectHandleOnStack.Create(ref o));
		return o;
	}

	[DllImport("QCall", EntryPoint = "ModuleHandle_GetPEKind", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ModuleHandle_GetPEKind")]
	private unsafe static extern void GetPEKind(QCallModule handle, int* peKind, int* machine);

	internal unsafe static void GetPEKind(RuntimeModule module, out PortableExecutableKinds peKind, out ImageFileMachine machine)
	{
		Unsafe.SkipInit(out int num);
		Unsafe.SkipInit(out int num2);
		GetPEKind(new QCallModule(ref module), &num, &num2);
		peKind = (PortableExecutableKinds)num;
		machine = (ImageFileMachine)num2;
	}

	[DllImport("QCall", EntryPoint = "ModuleHandle_GetMDStreamVersion", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "ModuleHandle_GetMDStreamVersion")]
	[SuppressGCTransition]
	private static extern int GetMDStreamVersion(QCallModule module);

	internal static int GetMDStreamVersion(RuntimeModule module)
	{
		return GetMDStreamVersion(new QCallModule(ref module));
	}
}
