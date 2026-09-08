using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace System;

internal static class StartupHookProvider
{
	private struct StartupHookNameOrPath
	{
		public AssemblyName AssemblyName;

		public string Path;
	}

	[FeatureSwitchDefinition("System.StartupHookProvider.IsSupported")]
	private static bool IsSupported
	{
		get
		{
			if (!AppContext.TryGetSwitch("System.StartupHookProvider.IsSupported", out var isEnabled))
			{
				return true;
			}
			return isEnabled;
		}
	}

	private unsafe static void ManagedStartup(char* pDiagnosticStartupHooks)
	{
		if (IsSupported)
		{
			ProcessStartupHooks(new string(pDiagnosticStartupHooks));
		}
	}

	private static void ProcessStartupHooks(string diagnosticStartupHooks)
	{
		if (!IsSupported)
		{
			return;
		}
		string text = AppContext.GetData("STARTUP_HOOKS") as string;
		if (text != null || !string.IsNullOrEmpty(diagnosticStartupHooks))
		{
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(diagnosticStartupHooks))
			{
				list.AddRange(diagnosticStartupHooks.Split(Path.PathSeparator));
			}
			if (text != null)
			{
				list.AddRange(text.Split(Path.PathSeparator));
			}
			StartupHookNameOrPath[] array = new StartupHookNameOrPath[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				ParseStartupHook(ref array[i], list[i]);
			}
			for (int j = 0; j < array.Length; j++)
			{
				CallStartupHook(array[j]);
			}
		}
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "An ILLink warning when trimming an app with System.StartupHookProvider.IsSupported=true already exists for ProcessStartupHooks.")]
	private unsafe static void CallStartupHook(char* pStartupHookPart)
	{
		if (IsSupported)
		{
			StartupHookNameOrPath startupHook = default(StartupHookNameOrPath);
			ParseStartupHook(ref startupHook, new string(pStartupHookPart));
			CallStartupHook(startupHook);
		}
	}

	private static void ParseStartupHook(ref StartupHookNameOrPath startupHook, string startupHookPart)
	{
		_003C_003Ey__InlineArray4<char> buffer = default(_003C_003Ey__InlineArray4<char>);
		buffer[0] = Path.DirectorySeparatorChar;
		buffer[1] = Path.AltDirectorySeparatorChar;
		buffer[2] = ' ';
		buffer[3] = ',';
		ReadOnlySpan<char> readOnlySpan = buffer;
		if (string.IsNullOrEmpty(startupHookPart))
		{
			return;
		}
		if (Path.IsPathFullyQualified(startupHookPart))
		{
			startupHook.Path = startupHookPart;
			return;
		}
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			if (startupHookPart.Contains(readOnlySpan[i]))
			{
				throw new ArgumentException(SR.Format(SR.Argument_InvalidStartupHookSimpleAssemblyName, startupHookPart));
			}
		}
		if (startupHookPart.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
		{
			throw new ArgumentException(SR.Format(SR.Argument_InvalidStartupHookSimpleAssemblyName, startupHookPart));
		}
		try
		{
			startupHook.AssemblyName = new AssemblyName(startupHookPart);
		}
		catch (Exception innerException)
		{
			throw new ArgumentException(SR.Format(SR.Argument_InvalidStartupHookSimpleAssemblyName, startupHookPart), innerException);
		}
	}

	[RequiresUnreferencedCode("The StartupHookSupport feature switch has been enabled for this app which is being trimmed. Startup hook code is not observable by the trimmer and so required assemblies, types and members may be removed")]
	private static void CallStartupHook(StartupHookNameOrPath startupHook)
	{
		Assembly assembly;
		try
		{
			if (startupHook.Path != null)
			{
				assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(startupHook.Path);
			}
			else
			{
				if (startupHook.AssemblyName == null)
				{
					return;
				}
				assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(startupHook.AssemblyName);
			}
		}
		catch (Exception innerException)
		{
			throw new ArgumentException(SR.Format(SR.Argument_StartupHookAssemblyLoadFailed, startupHook.Path ?? startupHook.AssemblyName.ToString()), innerException);
		}
		Type type = assembly.GetType("StartupHook", throwOnError: true);
		MethodInfo method = type.GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
		if (method == null)
		{
			try
			{
				if ((object)type.GetMethod("Initialize", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) == null)
				{
					throw new MissingMethodException("StartupHook", "Initialize");
				}
			}
			catch (AmbiguousMatchException)
			{
			}
		}
		if (method == null || method.ReturnType != typeof(void))
		{
			throw new ArgumentException(SR.Format(SR.Argument_InvalidStartupHookSignature, "StartupHook" + Type.Delimiter + "Initialize", startupHook.Path ?? startupHook.AssemblyName.ToString()));
		}
		method.Invoke(null, null);
	}
}
