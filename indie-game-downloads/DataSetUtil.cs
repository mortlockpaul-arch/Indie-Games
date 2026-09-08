using System;
using System.Data;
using System.Globalization;
using System.Security;
using System.Threading;

internal static class DataSetUtil
{
	internal static void CheckArgumentNull<T>(T argumentValue, string argumentName) where T : class
	{
		if (argumentValue == null)
		{
			throw ArgumentNull(argumentName);
		}
	}

	internal static ArgumentException Argument(string message)
	{
		return new ArgumentException(message);
	}

	internal static ArgumentNullException ArgumentNull(string message)
	{
		return new ArgumentNullException(message);
	}

	internal static ArgumentOutOfRangeException ArgumentOutOfRange(string message, string parameterName)
	{
		return new ArgumentOutOfRangeException(parameterName, message);
	}

	internal static InvalidCastException InvalidCast(string message)
	{
		return new InvalidCastException(message);
	}

	internal static InvalidOperationException InvalidOperation(string message)
	{
		return new InvalidOperationException(message);
	}

	internal static NotSupportedException NotSupported(string message)
	{
		return new NotSupportedException(message);
	}

	internal static ArgumentOutOfRangeException InvalidEnumerationValue(Type type, int value)
	{
		return ArgumentOutOfRange(System.SR.Format(System.SR.DataSetLinq_InvalidEnumerationValue, type.Name, value.ToString(CultureInfo.InvariantCulture)), type.Name);
	}

	internal static ArgumentOutOfRangeException InvalidDataRowState(DataRowState value)
	{
		return InvalidEnumerationValue(typeof(DataRowState), (int)value);
	}

	internal static ArgumentOutOfRangeException InvalidLoadOption(LoadOption value)
	{
		return InvalidEnumerationValue(typeof(LoadOption), (int)value);
	}

	internal static bool IsCatchableExceptionType(Exception e)
	{
		if (e.GetType() != typeof(StackOverflowException) && e.GetType() != typeof(OutOfMemoryException) && e.GetType() != typeof(ThreadAbortException) && e.GetType() != typeof(NullReferenceException) && e.GetType() != typeof(SecurityException))
		{
			return !typeof(SecurityException).IsAssignableFrom(e.GetType());
		}
		return false;
	}
}
