using System;
using System.IO;
using System.Reflection;

namespace MonoMod.Utils;

[MonoMod__OldName__("MonoMod.Helpers.PlatformHelper")]
public static class PlatformHelper
{
	public static Platform Current { get; private set; }

	static PlatformHelper()
	{
		PropertyInfo property = typeof(Environment).GetProperty("Platform", BindingFlags.Static | BindingFlags.NonPublic);
		string text = ((property == null) ? Environment.OSVersion.Platform.ToString() : property.GetValue(null, new object[0]).ToString());
		text = text.ToLowerInvariant();
		Current = Platform.Unknown;
		if (text.Contains("win"))
		{
			Current = Platform.Windows;
		}
		else if (text.Contains("mac") || text.Contains("osx"))
		{
			Current = Platform.MacOS;
		}
		else if (text.Contains("lin") || text.Contains("unix"))
		{
			Current = Platform.Linux;
		}
		if (Directory.Exists("/data") && File.Exists("/system/build.prop"))
		{
			Current = Platform.Android;
		}
		else if (Directory.Exists("/Applications") && Directory.Exists("/System"))
		{
			Current = Platform.iOS;
		}
		Current |= (Platform)((IntPtr.Size != 4) ? 2 : 0);
	}

	public static bool Is(Platform platform)
	{
		return (Current & platform) == platform;
	}
}
