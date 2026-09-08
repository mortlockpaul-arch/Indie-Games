using System.Runtime.CompilerServices;

namespace System.Text.Json;

internal static class StackHelper
{
	public static bool TryEnsureSufficientExecutionStack()
	{
		return RuntimeHelpers.TryEnsureSufficientExecutionStack();
	}
}
