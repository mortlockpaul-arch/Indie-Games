using System.Threading;

namespace System.Runtime.ExceptionServices;

public static class ExceptionHandling
{
	private static Func<Exception, bool> s_handler;

	internal static bool IsHandledByGlobalHandler(Exception ex)
	{
		return s_handler?.Invoke(ex) ?? false;
	}

	public static void SetUnhandledExceptionHandler(Func<Exception, bool> handler)
	{
		ArgumentNullException.ThrowIfNull(handler, "handler");
		if (Interlocked.CompareExchange(ref s_handler, handler, null) != null)
		{
			throw new InvalidOperationException(SR.InvalidOperation_CannotRegisterSecondHandler);
		}
	}

	public static void RaiseAppDomainUnhandledExceptionEvent(object exception)
	{
		ArgumentNullException.ThrowIfNull(exception, "exception");
		AppContext.OnUnhandledException(exception);
	}
}
