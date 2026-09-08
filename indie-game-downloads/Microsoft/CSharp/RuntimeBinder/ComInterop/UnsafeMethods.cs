using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.Marshalling;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal static class UnsafeMethods
{
	private static readonly object s_lock = new object();

	private static ModuleBuilder s_dynamicModule;

	internal static ModuleBuilder DynamicModule
	{
		[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
		get
		{
			if (s_dynamicModule != null)
			{
				return s_dynamicModule;
			}
			lock (s_lock)
			{
				if (s_dynamicModule == null)
				{
					string text = typeof(VariantArray).Namespace + ".DynamicAssembly";
					s_dynamicModule = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(text), AssemblyBuilderAccess.Run).DefineDynamicModule(text);
				}
				return s_dynamicModule;
			}
		}
	}

	public unsafe static nint ConvertInt32ByrefToPtr(ref int value)
	{
		return (nint)Unsafe.AsPointer(in value);
	}

	public unsafe static nint ConvertVariantByrefToPtr(ref ComVariant value)
	{
		return (nint)Unsafe.AsPointer(in value);
	}

	internal static ComVariant GetVariantForObject(object obj)
	{
		ComVariant variant = default(ComVariant);
		if (obj == null)
		{
			return variant;
		}
		InitVariantForObject(obj, ref variant);
		return variant;
	}

	internal static void InitVariantForObject(object obj, ref ComVariant variant)
	{
		if (obj is IDispatch)
		{
			variant = ComVariant.CreateRaw<nint>(VarEnum.VT_DISPATCH, (obj != null) ? Marshal.GetIDispatchForObject(obj) : 0);
		}
		else
		{
			Marshal.GetNativeVariantForObject(obj, ConvertVariantByrefToPtr(ref variant));
		}
	}

	public static object GetObjectForVariant(ComVariant variant)
	{
		return Marshal.GetObjectForNativeVariant(ConvertVariantByrefToPtr(ref variant));
	}

	public unsafe static int IUnknownRelease(nint interfacePointer)
	{
		return ((delegate* unmanaged<nint, int>)(*(IntPtr*)((nint)(*(IntPtr*)interfacePointer) + (nint)2 * (nint)sizeof(void*))))(interfacePointer);
	}

	public static void IUnknownReleaseNotZero(nint interfacePointer)
	{
		if (interfacePointer != IntPtr.Zero)
		{
			IUnknownRelease(interfacePointer);
		}
	}

	public unsafe static int IDispatchInvoke(nint dispatchPointer, int memberDispId, INVOKEKIND flags, ref DISPPARAMS dispParams, out ComVariant result, out ExcepInfo excepInfo, out uint argErr)
	{
		Guid guid = default(Guid);
		fixed (DISPPARAMS* ptr = &dispParams)
		{
			fixed (ComVariant* ptr2 = &result)
			{
				fixed (ExcepInfo* ptr3 = &excepInfo)
				{
					fixed (uint* ptr4 = &argErr)
					{
						delegate* unmanaged<nint, int, Guid*, int, ushort, DISPPARAMS*, ComVariant*, ExcepInfo*, uint*, int> obj = (delegate* unmanaged<nint, int, Guid*, int, ushort, DISPPARAMS*, ComVariant*, ExcepInfo*, uint*, int>)(*(IntPtr*)((nint)(*(IntPtr*)dispatchPointer) + (nint)6 * (nint)sizeof(void*)));
						int num = obj(dispatchPointer, memberDispId, &guid, 0, (ushort)flags, ptr, ptr2, ptr3, ptr4);
						if (num == -2147352573 && (flags & INVOKEKIND.INVOKE_FUNC) != 0 && (flags & (INVOKEKIND.INVOKE_PROPERTYPUT | INVOKEKIND.INVOKE_PROPERTYPUTREF)) == 0)
						{
							num = obj(dispatchPointer, memberDispId, &guid, 0, 1, ptr, null, ptr3, ptr4);
						}
						return num;
					}
				}
			}
		}
	}

	public static nint GetIdsOfNamedParameters(IDispatch dispatch, string[] names, int methodDispId, out GCHandle pinningHandle)
	{
		pinningHandle = GCHandle.Alloc(null, GCHandleType.Pinned);
		int[] array = new int[names.Length];
		Guid iid = Guid.Empty;
		int num = dispatch.TryGetIDsOfNames(ref iid, names, (uint)names.Length, 0, array);
		if (num < 0)
		{
			Marshal.ThrowExceptionForHR(num);
		}
		if (methodDispId != array[0])
		{
			throw Error.GetIDsOfNamesInvalid(names[0]);
		}
		int[] arr = (int[])(pinningHandle.Target = array.RemoveFirst());
		return Marshal.UnsafeAddrOfPinnedArrayElement(arr, 0);
	}
}
