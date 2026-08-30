using System;
using System.Globalization;

namespace ProjectMercury;

public class InvalidEnumArgumentException : ArgumentException
{
	public InvalidEnumArgumentException()
	{
	}

	public InvalidEnumArgumentException(string message)
		: base(message)
	{
	}

	public InvalidEnumArgumentException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public InvalidEnumArgumentException(string argumentName, int invalidValue, Type enumClass)
		: base($"The value of argument '{argumentName}' ({invalidValue.ToString(CultureInfo.CurrentCulture)}) is invalid for Enum type '{enumClass.Name}'.", argumentName)
	{
	}
}
