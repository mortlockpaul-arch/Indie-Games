namespace System;

internal static class AppContextSwitchHelper
{
	internal static bool GetBooleanConfig(string switchName, bool defaultValue = false)
	{
		if (!AppContext.TryGetSwitch(switchName, out var isEnabled))
		{
			return defaultValue;
		}
		return isEnabled;
	}

	internal static bool GetBooleanConfig(string switchName, string envVariable, bool defaultValue = false)
	{
		if (AppContext.TryGetSwitch(switchName, out var isEnabled))
		{
			return isEnabled;
		}
		string environmentVariable = Environment.GetEnvironmentVariable(envVariable);
		if (environmentVariable != null)
		{
			if (environmentVariable == "1" || string.Equals(environmentVariable, bool.TrueString, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (environmentVariable == "0" || string.Equals(environmentVariable, bool.FalseString, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
		}
		return defaultValue;
	}
}
