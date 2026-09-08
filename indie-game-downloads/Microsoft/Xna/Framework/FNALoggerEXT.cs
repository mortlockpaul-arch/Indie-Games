using System;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ObjCRuntime;

namespace Microsoft.Xna.Framework;

public static class FNALoggerEXT
{
	public static Action<string> LogInfo;

	public static Action<string> LogWarn;

	public static Action<string> LogError;

	private static FNA3D.FNA3D_LogFunc LogInfoFunc = FNA3DLogInfo;

	private static FNA3D.FNA3D_LogFunc LogWarnFunc = FNA3DLogWarn;

	private static FNA3D.FNA3D_LogFunc LogErrorFunc = FNA3DLogError;

	internal static void Initialize()
	{
		if (LogInfo == null)
		{
			LogInfo = Console.WriteLine;
		}
		if (LogWarn == null)
		{
			LogWarn = Console.WriteLine;
		}
		if (LogError == null)
		{
			LogError = Console.WriteLine;
		}
	}

	internal static void HookFNA3D()
	{
		try
		{
			FNA3D.FNA3D_HookLogFunctions(LogInfoFunc, LogWarnFunc, LogErrorFunc);
		}
		catch (DllNotFoundException)
		{
		}
	}

	[MonoPInvokeCallback(typeof(FNA3D.FNA3D_LogFunc))]
	private static void FNA3DLogInfo(nint msg)
	{
		LogInfo(UTF8_ToManaged(msg));
	}

	[MonoPInvokeCallback(typeof(FNA3D.FNA3D_LogFunc))]
	private static void FNA3DLogWarn(nint msg)
	{
		LogWarn(UTF8_ToManaged(msg));
	}

	[MonoPInvokeCallback(typeof(FNA3D.FNA3D_LogFunc))]
	private static void FNA3DLogError(nint msg)
	{
		string text = UTF8_ToManaged(msg);
		LogError(text);
		throw new InvalidOperationException(text);
	}

	private unsafe static string UTF8_ToManaged(nint s)
	{
		byte* ptr;
		for (ptr = (byte*)s; *ptr != 0; ptr++)
		{
		}
		int num = (int)(ptr - (byte*)s);
		if (num == 0)
		{
			return string.Empty;
		}
		char* ptr2 = stackalloc char[num];
		int chars = Encoding.UTF8.GetChars((byte*)s, num, ptr2, num);
		return new string(ptr2, 0, chars);
	}
}
