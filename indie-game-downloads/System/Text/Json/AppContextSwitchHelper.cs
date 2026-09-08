namespace System.Text.Json;

internal static class AppContextSwitchHelper
{
	public static bool IsSourceGenReflectionFallbackEnabled { get; } = AppContext.TryGetSwitch("System.Text.Json.Serialization.EnableSourceGenReflectionFallback", out var isEnabled) && isEnabled;

	public static bool RespectNullableAnnotationsDefault { get; } = AppContext.TryGetSwitch("System.Text.Json.Serialization.RespectNullableAnnotationsDefault", out var isEnabled2) && isEnabled2;

	public static bool RespectRequiredConstructorParametersDefault { get; } = AppContext.TryGetSwitch("System.Text.Json.Serialization.RespectRequiredConstructorParametersDefault", out var isEnabled3) && isEnabled3;
}
