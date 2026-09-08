using System.Diagnostics;

namespace System.Threading.Tasks;

internal static class LoggingExtensions
{
	public static string GetMethodName(this Delegate @delegate)
	{
		return DiagnosticMethodInfo.Create(@delegate)?.Name ?? "<unknown>";
	}
}
