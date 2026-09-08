using System;

namespace Microsoft.CSharp.RuntimeBinder;

internal static class Error
{
	internal static Exception InternalCompilerError()
	{
		return new RuntimeBinderInternalCompilerException(System.SR.InternalCompilerError);
	}

	internal static Exception BindPropertyFailedMethodGroup(object p0)
	{
		return new RuntimeBinderException(System.SR.Format(System.SR.BindPropertyFailedMethodGroup, p0));
	}

	internal static Exception BindPropertyFailedEvent(object p0)
	{
		return new RuntimeBinderException(System.SR.Format(System.SR.BindPropertyFailedEvent, p0));
	}

	internal static Exception BindInvokeFailedNonDelegate()
	{
		return new RuntimeBinderException(System.SR.BindInvokeFailedNonDelegate);
	}

	internal static Exception BindStaticRequiresType(string paramName)
	{
		return new ArgumentException(System.SR.TypeArgumentRequiredForStaticCall, paramName);
	}

	internal static Exception NullReferenceOnMemberException()
	{
		return new RuntimeBinderException(System.SR.NullReferenceOnMemberException);
	}

	internal static Exception BindCallToConditionalMethod(object p0)
	{
		return new RuntimeBinderException(System.SR.Format(System.SR.BindCallToConditionalMethod, p0));
	}

	internal static Exception BindToVoidMethodButExpectResult()
	{
		return new RuntimeBinderException(System.SR.BindToVoidMethodButExpectResult);
	}

	internal static Exception DynamicArgumentNeedsValue(string paramName)
	{
		return new ArgumentException(System.SR.DynamicArgumentNeedsValue, paramName);
	}
}
