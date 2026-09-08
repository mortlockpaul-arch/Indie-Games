using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal static class ComRuntimeHelpers
{
	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static void CheckThrowException(int hresult, ref ExcepInfo excepInfo, uint argErr, string message)
	{
		if (!ComHresults.IsSuccess(hresult))
		{
			switch (hresult)
			{
			case -2147352562:
				throw Error.DispBadParamCount(message);
			case -2147352567:
				throw excepInfo.GetException();
			case -2147352573:
				throw Error.DispMemberNotFound(message);
			case -2147352569:
				throw Error.DispNoNamedArgs(message);
			case -2147352566:
				throw Error.DispOverflow(message);
			case -2147352571:
				throw Error.DispTypeMismatch(argErr, message);
			case -2147352561:
				throw Error.DispParamNotOptional(message);
			}
			Marshal.ThrowExceptionForHR(hresult);
		}
	}

	internal static void GetInfoFromType(ITypeInfo typeInfo, out string name, out string documentation)
	{
		typeInfo.GetDocumentation(-1, out name, out documentation, out int _, out string _);
	}

	internal static string GetNameOfMethod(ITypeInfo typeInfo, int memid)
	{
		string[] array = new string[1];
		typeInfo.GetNames(memid, array, 1, out var _);
		return array[0];
	}

	internal static string GetNameOfLib(ITypeLib typeLib)
	{
		typeLib.GetDocumentation(-1, out string strName, out string _, out int _, out string _);
		return strName;
	}

	internal static string GetNameOfType(ITypeInfo typeInfo)
	{
		GetInfoFromType(typeInfo, out var name, out var _);
		return name;
	}

	internal static ITypeInfo GetITypeInfoFromIDispatch(IDispatch dispatch)
	{
		int errorCode = dispatch.TryGetTypeInfoCount(out var pctinfo);
		if (pctinfo == 0)
		{
			return null;
		}
		Marshal.ThrowExceptionForHR(errorCode);
		errorCode = dispatch.TryGetTypeInfo(0u, 0, out var info);
		if (!ComHresults.IsSuccess(errorCode))
		{
			if (errorCode == -2147467262)
			{
				return null;
			}
			Marshal.ThrowExceptionForHR(errorCode);
		}
		if (info == IntPtr.Zero)
		{
			Marshal.ThrowExceptionForHR(-2147467259);
		}
		ITypeInfo typeInfo = null;
		try
		{
			return Marshal.GetObjectForIUnknown(info) as ITypeInfo;
		}
		finally
		{
			Marshal.Release(info);
		}
	}

	internal unsafe static TYPEATTR GetTypeAttrForTypeInfo(ITypeInfo typeInfo)
	{
		typeInfo.GetTypeAttr(out var ppTypeAttr);
		if (ppTypeAttr == IntPtr.Zero)
		{
			throw Error.CannotRetrieveTypeInformation();
		}
		try
		{
			return *(TYPEATTR*)ppTypeAttr;
		}
		finally
		{
			typeInfo.ReleaseTypeAttr(ppTypeAttr);
		}
	}

	internal unsafe static TYPELIBATTR GetTypeAttrForTypeLib(ITypeLib typeLib)
	{
		typeLib.GetLibAttr(out var ppTLibAttr);
		if (ppTLibAttr == IntPtr.Zero)
		{
			throw Error.CannotRetrieveTypeInformation();
		}
		try
		{
			return *(TYPELIBATTR*)ppTLibAttr;
		}
		finally
		{
			typeLib.ReleaseTLibAttr(ppTLibAttr);
		}
	}

	[RequiresUnreferencedCode("Using dynamic types might cause types or members to be removed by trimmer.")]
	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static BoundDispEvent CreateComEvent(object rcw, Guid sourceIid, int dispid)
	{
		return new BoundDispEvent(rcw, sourceIid, dispid);
	}

	[RequiresDynamicCode("The 'dynamic' feature requires runtime-code generation, which is incompatible with AOT.")]
	public static DispCallable CreateDispCallable(IDispatchComObject dispatch, ComMethodDesc method)
	{
		return new DispCallable(dispatch, method.Name, method.DispId);
	}
}
