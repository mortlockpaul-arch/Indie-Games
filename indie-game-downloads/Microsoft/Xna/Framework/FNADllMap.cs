#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;

namespace Microsoft.Xna.Framework;

internal static class FNADllMap
{
	private static Dictionary<string, string> mapDictionary = new Dictionary<string, string>();

	private static string GetPlatformName()
	{
		if (OperatingSystem.IsWindows())
		{
			return "windows";
		}
		if (OperatingSystem.IsMacOS())
		{
			return "osx";
		}
		if (OperatingSystem.IsLinux())
		{
			return "linux";
		}
		if (OperatingSystem.IsFreeBSD())
		{
			return "freebsd";
		}
		return "unknown";
	}

	private static nint MapAndLoad(string libraryName, Assembly assembly, DllImportSearchPath? dllImportSearchPath)
	{
		if (!mapDictionary.TryGetValue(libraryName, out var value))
		{
			value = libraryName;
		}
		return NativeLibrary.Load(value, assembly, dllImportSearchPath);
	}

	private static nint LoadStaticLibrary(string libraryName, Assembly assembly, DllImportSearchPath? dllImportSearchPath)
	{
		return NativeLibrary.GetMainProgramHandle();
	}

	[ModuleInitializer]
	public static void Init()
	{
		if (!RuntimeFeature.IsDynamicCodeCompiled)
		{
			if (OperatingSystem.IsIOS() || OperatingSystem.IsTvOS())
			{
				NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), LoadStaticLibrary);
			}
			return;
		}
		string platformName = GetPlatformName();
		string value = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
		string value2 = (IntPtr.Size * 8).ToString();
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		string text = Path.Combine(AppContext.BaseDirectory, executingAssembly.GetName().Name + ".dll.config");
		if (!File.Exists(text))
		{
			return;
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(text);
		if (xmlDocument.GetElementsByTagName("dllentry").Count > 0)
		{
			string text2 = "Function remapping is not supported by .NET Core. Ignoring dllentry elements...";
			Console.WriteLine(text2);
			if (Debugger.IsAttached)
			{
				Debug.WriteLine(text2);
			}
		}
		foreach (XmlNode item in xmlDocument.GetElementsByTagName("dllmap"))
		{
			XmlAttribute xmlAttribute = item.Attributes["os"];
			if (xmlAttribute != null)
			{
				bool flag = xmlAttribute.Value.Contains(platformName);
				bool flag2 = xmlAttribute.Value.StartsWith("!");
				if ((!flag && !flag2) || (flag & flag2))
				{
					continue;
				}
			}
			xmlAttribute = item.Attributes["cpu"];
			if (xmlAttribute != null)
			{
				bool flag3 = xmlAttribute.Value.Contains(value);
				bool flag4 = xmlAttribute.Value.StartsWith("!");
				if ((!flag3 && !flag4) || (flag3 & flag4))
				{
					continue;
				}
			}
			xmlAttribute = item.Attributes["wordsize"];
			if (xmlAttribute != null)
			{
				bool flag5 = xmlAttribute.Value.Contains(value2);
				bool flag6 = xmlAttribute.Value.StartsWith("!");
				if ((!flag5 && !flag6) || (flag5 & flag6))
				{
					continue;
				}
			}
			XmlAttribute xmlAttribute2 = item.Attributes["dll"];
			XmlAttribute xmlAttribute3 = item.Attributes["target"];
			if (xmlAttribute2 != null && xmlAttribute3 != null)
			{
				string value3 = xmlAttribute2.Value;
				string value4 = xmlAttribute3.Value;
				if (!string.IsNullOrWhiteSpace(value3) && !string.IsNullOrWhiteSpace(value4) && !mapDictionary.ContainsKey(value3))
				{
					mapDictionary.Add(value3, value4);
				}
			}
		}
		NativeLibrary.SetDllImportResolver(executingAssembly, MapAndLoad);
	}
}
