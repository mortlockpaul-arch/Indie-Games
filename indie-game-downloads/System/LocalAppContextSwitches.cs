using System.Globalization;

namespace System;

internal static class LocalAppContextSwitches
{
	internal static long Pkcs12UnspecifiedPasswordIterationLimit { get; } = InitializePkcs12UnspecifiedPasswordIterationLimit();

	private static long InitializePkcs12UnspecifiedPasswordIterationLimit()
	{
		object data = AppContext.GetData("System.Security.Cryptography.Pkcs12UnspecifiedPasswordIterationLimit");
		if (data == null)
		{
			return 600000L;
		}
		try
		{
			return Convert.ToInt64(data, CultureInfo.InvariantCulture);
		}
		catch
		{
			return 600000L;
		}
	}
}
