using System.Collections.Generic;

namespace RenegadeEngine;

public static class ErrorLogger
{
	private static List<string> errorList = new List<string>();

	public static void LogError(string error)
	{
		errorList.Add(error);
	}

	public static void PrintLog()
	{
		_ = errorList.Count;
		_ = 0;
	}
}
