using System.Globalization;

namespace System;

internal static class AppContextConfigHelper
{
	internal static bool GetBooleanConfig(string switchName, bool defaultValue)
	{
		if (!AppContext.TryGetSwitch(switchName, out var isEnabled))
		{
			return defaultValue;
		}
		return isEnabled;
	}

	internal static bool GetBooleanConfig(string switchName, string envVariable, bool defaultValue = false)
	{
		string environmentVariable = Environment.GetEnvironmentVariable(envVariable);
		if (environmentVariable != null)
		{
			if (environmentVariable == "1" || bool.IsTrueStringIgnoreCase(environmentVariable.AsSpan()))
			{
				return true;
			}
			if (environmentVariable == "0" || bool.IsFalseStringIgnoreCase(environmentVariable.AsSpan()))
			{
				return false;
			}
		}
		return GetBooleanConfig(switchName, defaultValue);
	}

	internal static bool GetBooleanComPlusOrDotNetConfig(string configName, string envVariable, bool defaultValue)
	{
		string text = Environment.GetEnvironmentVariable("DOTNET_" + envVariable) ?? Environment.GetEnvironmentVariable("COMPlus_" + envVariable);
		if (text != null && text.StartsWith("0x", StringComparison.Ordinal))
		{
			text = text.Substring(2);
		}
		if (text != null && uint.TryParse(text, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out var result))
		{
			return result != 0;
		}
		return GetBooleanConfig(configName, defaultValue);
	}

	internal static int GetInt32Config(string configName, int defaultValue, bool allowNegative = true)
	{
		try
		{
			object data = AppContext.GetData(configName);
			int num = defaultValue;
			if (!(data is uint num2))
			{
				if (data is string text)
				{
					num = ((!text.StartsWith('0')) ? int.Parse(text, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo) : ((text.Length < 2 || text[1] != 'x') ? Convert.ToInt32(text, 8) : Convert.ToInt32(text, 16)));
				}
				else if (data is IConvertible convertible)
				{
					num = convertible.ToInt32(NumberFormatInfo.InvariantInfo);
				}
			}
			else
			{
				num = (int)num2;
			}
			return (!allowNegative && num < 0) ? defaultValue : num;
		}
		catch (FormatException)
		{
			return defaultValue;
		}
		catch (OverflowException)
		{
			return defaultValue;
		}
	}

	internal static int GetInt32Config(string configName, string envVariable, int defaultValue, bool allowNegative = true)
	{
		string environmentVariable = Environment.GetEnvironmentVariable(envVariable);
		if (environmentVariable != null)
		{
			try
			{
				int num = ((!environmentVariable.StartsWith('0')) ? int.Parse(environmentVariable, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo) : ((environmentVariable.Length < 2 || environmentVariable[1] != 'x') ? Convert.ToInt32(environmentVariable, 8) : Convert.ToInt32(environmentVariable, 16)));
				if (allowNegative || num >= 0)
				{
					return num;
				}
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}
		}
		return GetInt32Config(configName, defaultValue, allowNegative);
	}

	internal static int GetInt32ComPlusOrDotNetConfig(string configName, string envVariable, int defaultValue, bool allowNegative)
	{
		string text = Environment.GetEnvironmentVariable("DOTNET_" + envVariable) ?? Environment.GetEnvironmentVariable("COMPlus_" + envVariable);
		if (text != null && text.StartsWith("0x", StringComparison.Ordinal))
		{
			text = text.Substring(2);
		}
		if (text != null && uint.TryParse(text, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out var result))
		{
			int num = (int)result;
			if (allowNegative || num >= 0)
			{
				return num;
			}
		}
		return GetInt32Config(configName, defaultValue, allowNegative);
	}

	internal static short GetInt16Config(string configName, short defaultValue, bool allowNegative = true)
	{
		try
		{
			object data = AppContext.GetData(configName);
			short num = defaultValue;
			if (!(data is uint num2))
			{
				if (data is string text)
				{
					num = (text.StartsWith("0x") ? Convert.ToInt16(text, 16) : ((!text.StartsWith('0')) ? short.Parse(text, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo) : Convert.ToInt16(text, 8)));
				}
				else if (data is IConvertible convertible)
				{
					num = convertible.ToInt16(NumberFormatInfo.InvariantInfo);
				}
			}
			else
			{
				num = (short)num2;
				if ((uint)num != num2)
				{
					return defaultValue;
				}
			}
			return (!allowNegative && num < 0) ? defaultValue : num;
		}
		catch (FormatException)
		{
			return defaultValue;
		}
		catch (OverflowException)
		{
			return defaultValue;
		}
	}

	internal static short GetInt16Config(string configName, string envVariable, short defaultValue, bool allowNegative = true)
	{
		string environmentVariable = Environment.GetEnvironmentVariable(envVariable);
		if (environmentVariable != null)
		{
			try
			{
				short num = ((!environmentVariable.StartsWith('0')) ? short.Parse(environmentVariable, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo) : ((environmentVariable.Length < 2 || environmentVariable[1] != 'x') ? Convert.ToInt16(environmentVariable, 8) : Convert.ToInt16(environmentVariable, 16)));
				if (allowNegative || num >= 0)
				{
					return num;
				}
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}
		}
		return GetInt16Config(configName, defaultValue, allowNegative);
	}

	internal static short GetInt16ComPlusOrDotNetConfig(string configName, string envVariable, short defaultValue, bool allowNegative)
	{
		string text = Environment.GetEnvironmentVariable("DOTNET_" + envVariable) ?? Environment.GetEnvironmentVariable("COMPlus_" + envVariable);
		if (text != null && text.StartsWith("0x", StringComparison.Ordinal))
		{
			text = text.Substring(2);
		}
		if (text != null && ushort.TryParse(text, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out var result))
		{
			short num = (short)result;
			if (allowNegative || num >= 0)
			{
				return num;
			}
		}
		return GetInt16Config(configName, defaultValue, allowNegative);
	}
}
