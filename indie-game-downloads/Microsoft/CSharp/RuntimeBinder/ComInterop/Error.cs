using System;
using System.Reflection;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal static class Error
{
	internal static Exception SetComObjectDataFailed()
	{
		return new InvalidOperationException(System.SR.COMSetComObjectDataFailed);
	}

	internal static Exception UnexpectedVarEnum(object p0)
	{
		return new InvalidOperationException(System.SR.Format(System.SR.COMUnexpectedVarEnum, p0));
	}

	internal static Exception DispBadParamCount(object p0)
	{
		return new TargetParameterCountException(System.SR.Format(System.SR.COMDispatchInvokeError, p0));
	}

	internal static Exception DispMemberNotFound(object p0)
	{
		return new MissingMemberException(System.SR.Format(System.SR.COMDispatchInvokeError, p0));
	}

	internal static Exception DispNoNamedArgs(object p0)
	{
		return new ArgumentException(System.SR.Format(System.SR.COMDispatchInvokeErrorNoNamedArgs, p0));
	}

	internal static Exception DispOverflow(object p0)
	{
		return new OverflowException(System.SR.Format(System.SR.COMDispatchInvokeError, p0));
	}

	internal static Exception DispTypeMismatch(object p0, object p1)
	{
		return new ArgumentException(System.SR.Format(System.SR.COMDispatchInvokeErrorTypeMismatch, p0, p1));
	}

	internal static Exception DispParamNotOptional(object p0)
	{
		return new ArgumentException(System.SR.Format(System.SR.COMDispatchInvokeErrorParamNotOptional, p0));
	}

	internal static Exception CannotRetrieveTypeInformation()
	{
		return new InvalidOperationException(System.SR.COMCannotRetrieveTypeInfo);
	}

	internal static Exception GetIDsOfNamesInvalid(object p0)
	{
		return new ArgumentException(System.SR.Format(System.SR.COMGetIDsOfNamesInvalid, p0));
	}

	internal static Exception UnsupportedHandlerType()
	{
		return new InvalidOperationException(System.SR.COMUnsupportedEventHandlerType);
	}

	internal static Exception CouldNotGetDispId(object p0, object p1)
	{
		return new MissingMemberException(System.SR.Format(System.SR.COMGetDispatchIdFailed, p0, p1));
	}

	internal static Exception AmbiguousConversion(object p0, object p1)
	{
		return new AmbiguousMatchException(System.SR.Format(System.SR.COMAmbiguousConversion, p0, p1));
	}
}
