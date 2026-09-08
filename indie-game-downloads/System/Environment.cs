using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using Internal.Win32;
using Microsoft.Win32.SafeHandles;

namespace System;

public static class Environment
{
	public readonly struct ProcessCpuUsage
	{
		public TimeSpan UserTime { get; internal init; }

		public TimeSpan PrivilegedTime { get; internal init; }

		public TimeSpan TotalTime => UserTime + PrivilegedTime;
	}

	public enum SpecialFolder
	{
		ApplicationData = 26,
		CommonApplicationData = 35,
		LocalApplicationData = 28,
		Cookies = 33,
		Desktop = 0,
		Favorites = 6,
		History = 34,
		InternetCache = 32,
		Programs = 2,
		MyComputer = 17,
		MyMusic = 13,
		MyPictures = 39,
		MyVideos = 14,
		Recent = 8,
		SendTo = 9,
		StartMenu = 11,
		Startup = 7,
		System = 37,
		Templates = 21,
		DesktopDirectory = 16,
		Personal = 5,
		MyDocuments = Personal,
		ProgramFiles = 38,
		CommonProgramFiles = 43,
		AdminTools = 48,
		CDBurning = 59,
		CommonAdminTools = 47,
		CommonDocuments = 46,
		CommonMusic = 53,
		CommonOemLinks = 58,
		CommonPictures = 54,
		CommonStartMenu = 22,
		CommonPrograms = 23,
		CommonStartup = 24,
		CommonDesktopDirectory = 25,
		CommonTemplates = 45,
		CommonVideos = 55,
		Fonts = 20,
		NetworkShortcuts = 19,
		PrinterShortcuts = 27,
		UserProfile = 40,
		CommonProgramFilesX86 = 44,
		ProgramFilesX86 = 42,
		Resources = 56,
		LocalizedResources = 57,
		SystemX86 = 41,
		Windows = 36
	}

	public enum SpecialFolderOption
	{
		None = 0,
		Create = 32768,
		DoNotVerify = 16384
	}

	private static class WindowsVersion
	{
		internal static readonly bool IsWindows8OrAbove = OperatingSystem.IsWindowsVersionAtLeast(6, 2);
	}

	private static volatile sbyte s_privilegedProcess;

	internal static string[] s_commandLineArgs;

	private static volatile int s_processId;

	private static volatile string s_processPath;

	private static volatile OperatingSystem s_osVersion;

	private static volatile int s_systemPageSize;

	public static extern int CurrentManagedThreadId
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	public static extern int ExitCode
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public static int ProcessorCount { get; } = GetProcessorCount();

	internal static bool IsSingleProcessor => ProcessorCount == 1;

	public static bool IsPrivilegedProcess
	{
		get
		{
			sbyte b = s_privilegedProcess;
			if (b == 0)
			{
				b = (s_privilegedProcess = (sbyte)(IsPrivilegedProcessCore() ? 1 : (-1)));
			}
			return b > 0;
		}
	}

	public static bool HasShutdownStarted => false;

	public static string CommandLine => PasteArguments.Paste(GetCommandLineArgs(), pasteFirstArgumentUsingArgV0Rules: true);

	public static string CurrentDirectory
	{
		get
		{
			return CurrentDirectoryCore;
		}
		set
		{
			ArgumentException.ThrowIfNullOrEmpty(value, "value");
			CurrentDirectoryCore = value;
		}
	}

	public static int ProcessId
	{
		get
		{
			int num = s_processId;
			if (num == 0)
			{
				num = (s_processId = GetProcessId());
			}
			return num;
		}
	}

	public static string? ProcessPath
	{
		get
		{
			string text = s_processPath;
			if (text == null)
			{
				Interlocked.CompareExchange(ref s_processPath, GetProcessPath() ?? "", null);
				text = s_processPath;
			}
			if (text.Length == 0)
			{
				return null;
			}
			return text;
		}
	}

	public static bool Is64BitProcess => 8 == 8;

	public static bool Is64BitOperatingSystem
	{
		get
		{
			if (1 == 0)
			{
			}
			return true;
		}
	}

	public static string NewLine => "\r\n";

	public static OperatingSystem OSVersion
	{
		get
		{
			OperatingSystem operatingSystem = s_osVersion;
			if (operatingSystem == null)
			{
				Interlocked.CompareExchange(ref s_osVersion, GetOSVersion(), null);
				operatingSystem = s_osVersion;
			}
			return operatingSystem;
		}
	}

	public static string StackTrace
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return new StackTrace(fNeedFileInfo: true).ToString(System.Diagnostics.StackTrace.TraceFormat.Normal);
		}
	}

	public static int SystemPageSize
	{
		get
		{
			int num = s_systemPageSize;
			if (num == 0)
			{
				num = (s_systemPageSize = GetSystemPageSize());
			}
			return num;
		}
	}

	public static int TickCount => (int)TickCount64;

	internal static bool IsWindows8OrAbove => WindowsVersion.IsWindows8OrAbove;

	public static string UserName
	{
		get
		{
			Span<char> initialBuffer = stackalloc char[40];
			ValueStringBuilder builder = new ValueStringBuilder(initialBuffer);
			GetUserName(ref builder);
			ReadOnlySpan<char> span = builder.AsSpan();
			int num = span.IndexOf('\\');
			if (num >= 0)
			{
				span = span.Slice(num + 1);
			}
			string result = span.ToString();
			builder.Dispose();
			return result;
		}
	}

	public static string UserDomainName
	{
		get
		{
			Span<char> initialBuffer = stackalloc char[40];
			ValueStringBuilder builder = new ValueStringBuilder(initialBuffer);
			GetUserName(ref builder);
			int num = builder.AsSpan().IndexOf('\\');
			if (num >= 0)
			{
				builder.Length = num;
				return builder.ToString();
			}
			initialBuffer = stackalloc char[64];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(initialBuffer);
			uint cchReferencedDomainName = (uint)valueStringBuilder.Capacity;
			Span<byte> span = stackalloc byte[68];
			uint cbSid = 68u;
			uint peUse;
			while (!Interop.Advapi32.LookupAccountNameW(null, ref builder.GetPinnableReference(), ref MemoryMarshal.GetReference(span), ref cbSid, ref valueStringBuilder.GetPinnableReference(), ref cchReferencedDomainName, out peUse))
			{
				int lastPInvokeError = Marshal.GetLastPInvokeError();
				if (lastPInvokeError != 122)
				{
					throw new InvalidOperationException(Marshal.GetPInvokeErrorMessage(lastPInvokeError));
				}
				valueStringBuilder.EnsureCapacity((int)cchReferencedDomainName);
			}
			builder.Dispose();
			valueStringBuilder.Length = (int)cchReferencedDomainName;
			return valueStringBuilder.ToString();
		}
	}

	private static string CurrentDirectoryCore
	{
		get
		{
			Span<char> initialBuffer = stackalloc char[260];
			ValueStringBuilder outputBuilder = new ValueStringBuilder(initialBuffer);
			uint currentDirectory;
			while ((currentDirectory = Interop.Kernel32.GetCurrentDirectory((uint)outputBuilder.Capacity, ref outputBuilder.GetPinnableReference())) > outputBuilder.Capacity)
			{
				outputBuilder.EnsureCapacity((int)currentDirectory);
			}
			if (currentDirectory == 0)
			{
				throw Win32Marshal.GetExceptionForLastWin32Error();
			}
			outputBuilder.Length = (int)currentDirectory;
			if (outputBuilder.AsSpan().Contains('~'))
			{
				string result = PathHelper.TryExpandShortFileName(ref outputBuilder, null);
				outputBuilder.Dispose();
				return result;
			}
			return outputBuilder.ToString();
		}
		set
		{
			if (!Interop.Kernel32.SetCurrentDirectory(value))
			{
				int lastPInvokeError = Marshal.GetLastPInvokeError();
				throw Win32Marshal.GetExceptionForWin32Error((lastPInvokeError == 2) ? 3 : lastPInvokeError, value);
			}
		}
	}

	public static string MachineName => Interop.Kernel32.GetComputerName() ?? throw new InvalidOperationException(SR.InvalidOperation_ComputerName);

	public static string SystemDirectory
	{
		get
		{
			Span<char> initialBuffer = stackalloc char[32];
			ValueStringBuilder valueStringBuilder = new ValueStringBuilder(initialBuffer);
			uint systemDirectoryW;
			while ((systemDirectoryW = Interop.Kernel32.GetSystemDirectoryW(ref valueStringBuilder.GetPinnableReference(), (uint)valueStringBuilder.Capacity)) > valueStringBuilder.Capacity)
			{
				valueStringBuilder.EnsureCapacity((int)systemDirectoryW);
			}
			if (systemDirectoryW == 0)
			{
				throw Win32Marshal.GetExceptionForLastWin32Error();
			}
			valueStringBuilder.Length = (int)systemDirectoryW;
			return valueStringBuilder.ToString();
		}
	}

	public unsafe static bool UserInteractive
	{
		get
		{
			nint processWindowStation = Interop.User32.GetProcessWindowStation();
			if (processWindowStation != IntPtr.Zero)
			{
				Interop.User32.USEROBJECTFLAGS uSEROBJECTFLAGS = default(Interop.User32.USEROBJECTFLAGS);
				uint lpnLengthNeeded = 0u;
				if (Interop.User32.GetUserObjectInformationW(processWindowStation, 1, &uSEROBJECTFLAGS, (uint)sizeof(Interop.User32.USEROBJECTFLAGS), ref lpnLengthNeeded))
				{
					return (uSEROBJECTFLAGS.dwFlags & 1) != 0;
				}
			}
			return true;
		}
	}

	public unsafe static long WorkingSet
	{
		get
		{
			Interop.Kernel32.PROCESS_MEMORY_COUNTERS ppsmemCounters = new Interop.Kernel32.PROCESS_MEMORY_COUNTERS
			{
				cb = (uint)sizeof(Interop.Kernel32.PROCESS_MEMORY_COUNTERS)
			};
			if (!Interop.Kernel32.GetProcessMemoryInfo(Interop.Kernel32.GetCurrentProcess(), ref ppsmemCounters, ppsmemCounters.cb))
			{
				return 0L;
			}
			return (long)ppsmemCounters.WorkingSetSize;
		}
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("tvos")]
	[UnsupportedOSPlatform("browser")]
	[SupportedOSPlatform("maccatalyst")]
	public static ProcessCpuUsage CpuUsage
	{
		get
		{
			if (!Interop.Kernel32.GetProcessTimes(Interop.Kernel32.GetCurrentProcess(), out var _, out var _, out var kernel, out var user))
			{
				return new ProcessCpuUsage
				{
					UserTime = TimeSpan.Zero,
					PrivilegedTime = TimeSpan.Zero
				};
			}
			return new ProcessCpuUsage
			{
				UserTime = new TimeSpan(user),
				PrivilegedTime = new TimeSpan(kernel)
			};
		}
	}

	public static long TickCount64 => (long)Interop.Kernel32.GetTickCount64();

	public static Version Version => new Version(10, 0, 11);

	[DllImport("QCall", EntryPoint = "Environment_Exit", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Environment_Exit")]
	[DoesNotReturn]
	private static extern void _Exit(int exitCode);

	[DoesNotReturn]
	public static void Exit(int exitCode)
	{
		_Exit(exitCode);
	}

	[DoesNotReturn]
	public static void FailFast(string? message)
	{
		StackCrawlMark mark = StackCrawlMark.LookForMyCaller;
		FailFast(ref mark, message, null, null);
	}

	[DoesNotReturn]
	public static void FailFast(string? message, Exception? exception)
	{
		StackCrawlMark mark = StackCrawlMark.LookForMyCaller;
		FailFast(ref mark, message, exception, null);
	}

	[DoesNotReturn]
	internal static void FailFast(string message, Exception exception, string errorMessage)
	{
		StackCrawlMark mark = StackCrawlMark.LookForMyCaller;
		FailFast(ref mark, message, exception, errorMessage);
	}

	[DoesNotReturn]
	private static void FailFast(ref StackCrawlMark mark, string message, Exception exception, string errorMessage)
	{
		FailFast(new StackCrawlMarkHandle(ref mark), message, ObjectHandleOnStack.Create(ref exception), errorMessage);
	}

	[LibraryImport("QCall", EntryPoint = "Environment_FailFast", StringMarshalling = StringMarshalling.Utf16)]
	[DoesNotReturn]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	private unsafe static void FailFast(StackCrawlMarkHandle mark, string message, ObjectHandleOnStack exception, string errorMessage)
	{
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(errorMessage))
		{
			void* _errorMessage_native = ptr;
			fixed (char* ptr2 = &Utf16StringMarshaller.GetPinnableReference(message))
			{
				void* _message_native = ptr2;
				__PInvoke(mark, (ushort*)_message_native, exception, (ushort*)_errorMessage_native);
			}
		}
		[DllImport("QCall", EntryPoint = "Environment_FailFast", ExactSpelling = true)]
		unsafe static extern void __PInvoke(StackCrawlMarkHandle __mark_native, ushort* __message_native, ObjectHandleOnStack __exception_native, ushort* __errorMessage_native);
	}

	private unsafe static string[] InitializeCommandLineArgs(char* exePath, int argc, char** argv)
	{
		string[] array = new string[argc + 1];
		string[] array2 = new string[argc];
		array[0] = new string(exePath);
		for (int i = 0; i < array2.Length; i++)
		{
			array[i + 1] = (array2[i] = new string(argv[i]));
		}
		s_commandLineArgs = array;
		return array2;
	}

	[DllImport("QCall", EntryPoint = "Environment_GetProcessorCount", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Environment_GetProcessorCount")]
	private static extern int GetProcessorCount();

	internal static string GetResourceStringLocal(string key)
	{
		return SR.GetResourceString(key);
	}

	public static string? GetEnvironmentVariable(string variable)
	{
		ArgumentNullException.ThrowIfNull(variable, "variable");
		return GetEnvironmentVariableCore(variable);
	}

	public static string? GetEnvironmentVariable(string variable, EnvironmentVariableTarget target)
	{
		if (target == EnvironmentVariableTarget.Process)
		{
			return GetEnvironmentVariable(variable);
		}
		ArgumentNullException.ThrowIfNull(variable, "variable");
		bool fromMachine = ValidateAndConvertRegistryTarget(target);
		return GetEnvironmentVariableFromRegistry(variable, fromMachine);
	}

	public static IDictionary GetEnvironmentVariables(EnvironmentVariableTarget target)
	{
		if (target == EnvironmentVariableTarget.Process)
		{
			return GetEnvironmentVariables();
		}
		return GetEnvironmentVariablesFromRegistry(ValidateAndConvertRegistryTarget(target));
	}

	public static void SetEnvironmentVariable(string variable, string? value)
	{
		ValidateVariable(variable);
		SetEnvironmentVariableCore(variable, value);
	}

	public static void SetEnvironmentVariable(string variable, string? value, EnvironmentVariableTarget target)
	{
		if (target == EnvironmentVariableTarget.Process)
		{
			SetEnvironmentVariable(variable, value);
			return;
		}
		ValidateVariable(variable);
		bool fromMachine = ValidateAndConvertRegistryTarget(target);
		SetEnvironmentVariableFromRegistry(variable, value, fromMachine);
	}

	public static string[] GetCommandLineArgs()
	{
		if (s_commandLineArgs == null)
		{
			return GetCommandLineArgsNative();
		}
		return (string[])s_commandLineArgs.Clone();
	}

	public static string ExpandEnvironmentVariables(string name)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		if (name.Length == 0)
		{
			return name;
		}
		return ExpandEnvironmentVariablesCore(name);
	}

	public static string GetFolderPath(SpecialFolder folder)
	{
		return GetFolderPathCore(folder, SpecialFolderOption.None);
	}

	public static string GetFolderPath(SpecialFolder folder, SpecialFolderOption option)
	{
		if (option != SpecialFolderOption.None && option != SpecialFolderOption.Create && option != SpecialFolderOption.DoNotVerify)
		{
			Throw(option);
		}
		return GetFolderPathCore(folder, option);
		static void Throw(SpecialFolderOption specialFolderOption)
		{
			throw new ArgumentOutOfRangeException("option", specialFolderOption, SR.Format(SR.Arg_EnumIllegalVal, specialFolderOption));
		}
	}

	private static bool ValidateAndConvertRegistryTarget(EnvironmentVariableTarget target)
	{
		return target switch
		{
			EnvironmentVariableTarget.Machine => true, 
			EnvironmentVariableTarget.User => false, 
			_ => throw new ArgumentOutOfRangeException("target", target, SR.Format(SR.Arg_EnumIllegalVal, target)), 
		};
	}

	private static void ValidateVariable(string variable)
	{
		ArgumentException.ThrowIfNullOrEmpty(variable, "variable");
		if (variable[0] == '\0')
		{
			throw new ArgumentException(SR.Argument_StringFirstCharIsZero, "variable");
		}
		if (variable.Contains('='))
		{
			throw new ArgumentException(SR.Argument_IllegalEnvVarName, "variable");
		}
	}

	private static string GetEnvironmentVariableFromRegistry(string variable, bool fromMachine)
	{
		using RegistryKey registryKey = OpenEnvironmentKeyIfExists(fromMachine, writable: false);
		return registryKey?.GetValue(variable) as string;
	}

	private unsafe static void SetEnvironmentVariableFromRegistry(string variable, string value, bool fromMachine)
	{
		if (!fromMachine && variable.Length >= 255)
		{
			throw new ArgumentException(SR.Argument_LongEnvVarValue, "variable");
		}
		using (RegistryKey registryKey = OpenEnvironmentKeyIfExists(fromMachine, writable: true))
		{
			if (registryKey != null)
			{
				if (value == null)
				{
					registryKey.DeleteValue(variable, throwOnMissingValue: false);
				}
				else
				{
					registryKey.SetValue(variable, value);
				}
			}
		}
		fixed (char* lParam = &"Environment".GetPinnableReference())
		{
			Unsafe.SkipInit(out nint num);
			Interop.User32.SendMessageTimeout(new IntPtr(65535), 26, IntPtr.Zero, (nint)lParam, 0, 1000, &num);
		}
	}

	private static Hashtable GetEnvironmentVariablesFromRegistry(bool fromMachine)
	{
		Hashtable hashtable = new Hashtable();
		using (RegistryKey registryKey = OpenEnvironmentKeyIfExists(fromMachine, writable: false))
		{
			if (registryKey != null)
			{
				string[] valueNames = registryKey.GetValueNames();
				foreach (string text in valueNames)
				{
					string value = registryKey.GetValue(text, "").ToString();
					try
					{
						hashtable.Add(text, value);
					}
					catch (ArgumentException)
					{
					}
				}
			}
		}
		return hashtable;
	}

	private static RegistryKey OpenEnvironmentKeyIfExists(bool fromMachine, bool writable)
	{
		RegistryKey registryKey;
		string name;
		if (fromMachine)
		{
			registryKey = Registry.LocalMachine;
			name = "System\\CurrentControlSet\\Control\\Session Manager\\Environment";
		}
		else
		{
			registryKey = Registry.CurrentUser;
			name = "Environment";
		}
		return registryKey.OpenSubKey(name, writable);
	}

	private static void GetUserName(ref ValueStringBuilder builder)
	{
		uint lpnSize = 0u;
		while (Interop.Secur32.GetUserNameExW(2, ref builder.GetPinnableReference(), ref lpnSize) == Interop.BOOLEAN.FALSE)
		{
			if (Marshal.GetLastPInvokeError() == 234)
			{
				builder.EnsureCapacity(checked((int)lpnSize));
				continue;
			}
			builder.Length = 0;
			return;
		}
		builder.Length = (int)lpnSize;
	}

	private static string GetFolderPathCore(SpecialFolder folder, SpecialFolderOption option)
	{
		string text = null;
		ReadOnlySpan<byte> b;
		switch (folder)
		{
		case SpecialFolder.System:
			return SystemDirectory;
		case SpecialFolder.ApplicationData:
			b = Interop.Shell32.KnownFolders.RoamingAppData;
			text = "APPDATA";
			break;
		case SpecialFolder.CommonApplicationData:
			b = Interop.Shell32.KnownFolders.ProgramData;
			text = "ProgramData";
			break;
		case SpecialFolder.LocalApplicationData:
			b = Interop.Shell32.KnownFolders.LocalAppData;
			text = "LOCALAPPDATA";
			break;
		case SpecialFolder.Cookies:
			b = Interop.Shell32.KnownFolders.Cookies;
			break;
		case SpecialFolder.Desktop:
			b = Interop.Shell32.KnownFolders.Desktop;
			break;
		case SpecialFolder.Favorites:
			b = Interop.Shell32.KnownFolders.Favorites;
			break;
		case SpecialFolder.History:
			b = Interop.Shell32.KnownFolders.History;
			break;
		case SpecialFolder.InternetCache:
			b = Interop.Shell32.KnownFolders.InternetCache;
			break;
		case SpecialFolder.Programs:
			b = Interop.Shell32.KnownFolders.Programs;
			break;
		case SpecialFolder.MyComputer:
			b = Interop.Shell32.KnownFolders.ComputerFolder;
			break;
		case SpecialFolder.MyMusic:
			b = Interop.Shell32.KnownFolders.Music;
			break;
		case SpecialFolder.MyPictures:
			b = Interop.Shell32.KnownFolders.Pictures;
			break;
		case SpecialFolder.MyVideos:
			b = Interop.Shell32.KnownFolders.Videos;
			break;
		case SpecialFolder.Recent:
			b = Interop.Shell32.KnownFolders.Recent;
			break;
		case SpecialFolder.SendTo:
			b = Interop.Shell32.KnownFolders.SendTo;
			break;
		case SpecialFolder.StartMenu:
			b = Interop.Shell32.KnownFolders.StartMenu;
			break;
		case SpecialFolder.Startup:
			b = Interop.Shell32.KnownFolders.Startup;
			break;
		case SpecialFolder.Templates:
			b = Interop.Shell32.KnownFolders.Templates;
			break;
		case SpecialFolder.DesktopDirectory:
			b = Interop.Shell32.KnownFolders.Desktop;
			break;
		case SpecialFolder.Personal:
			b = Interop.Shell32.KnownFolders.Documents;
			break;
		case SpecialFolder.ProgramFiles:
			b = Interop.Shell32.KnownFolders.ProgramFiles;
			text = "ProgramFiles";
			break;
		case SpecialFolder.CommonProgramFiles:
			b = Interop.Shell32.KnownFolders.ProgramFilesCommon;
			text = "CommonProgramFiles";
			break;
		case SpecialFolder.AdminTools:
			b = Interop.Shell32.KnownFolders.AdminTools;
			break;
		case SpecialFolder.CDBurning:
			b = Interop.Shell32.KnownFolders.CDBurning;
			break;
		case SpecialFolder.CommonAdminTools:
			b = Interop.Shell32.KnownFolders.CommonAdminTools;
			break;
		case SpecialFolder.CommonDocuments:
			b = Interop.Shell32.KnownFolders.PublicDocuments;
			break;
		case SpecialFolder.CommonMusic:
			b = Interop.Shell32.KnownFolders.PublicMusic;
			break;
		case SpecialFolder.CommonOemLinks:
			b = Interop.Shell32.KnownFolders.CommonOEMLinks;
			break;
		case SpecialFolder.CommonPictures:
			b = Interop.Shell32.KnownFolders.PublicPictures;
			break;
		case SpecialFolder.CommonStartMenu:
			b = Interop.Shell32.KnownFolders.CommonStartMenu;
			break;
		case SpecialFolder.CommonPrograms:
			b = Interop.Shell32.KnownFolders.CommonPrograms;
			break;
		case SpecialFolder.CommonStartup:
			b = Interop.Shell32.KnownFolders.CommonStartup;
			break;
		case SpecialFolder.CommonDesktopDirectory:
			b = Interop.Shell32.KnownFolders.PublicDesktop;
			break;
		case SpecialFolder.CommonTemplates:
			b = Interop.Shell32.KnownFolders.CommonTemplates;
			break;
		case SpecialFolder.CommonVideos:
			b = Interop.Shell32.KnownFolders.PublicVideos;
			break;
		case SpecialFolder.Fonts:
			b = Interop.Shell32.KnownFolders.Fonts;
			break;
		case SpecialFolder.NetworkShortcuts:
			b = Interop.Shell32.KnownFolders.NetHood;
			break;
		case SpecialFolder.PrinterShortcuts:
			b = Interop.Shell32.KnownFolders.PrintersFolder;
			break;
		case SpecialFolder.UserProfile:
			b = Interop.Shell32.KnownFolders.Profile;
			text = "USERPROFILE";
			break;
		case SpecialFolder.CommonProgramFilesX86:
			b = Interop.Shell32.KnownFolders.ProgramFilesCommonX86;
			text = "CommonProgramFiles(x86)";
			break;
		case SpecialFolder.ProgramFilesX86:
			b = Interop.Shell32.KnownFolders.ProgramFilesX86;
			text = "ProgramFiles(x86)";
			break;
		case SpecialFolder.Resources:
			b = Interop.Shell32.KnownFolders.ResourceDir;
			break;
		case SpecialFolder.LocalizedResources:
			b = Interop.Shell32.KnownFolders.LocalizedResourcesDir;
			break;
		case SpecialFolder.SystemX86:
			b = Interop.Shell32.KnownFolders.SystemX86;
			break;
		case SpecialFolder.Windows:
			b = Interop.Shell32.KnownFolders.Windows;
			text = "windir";
			break;
		default:
			throw new ArgumentOutOfRangeException("folder", folder, SR.Format(SR.Arg_EnumIllegalVal, folder));
		}
		if (Interop.Shell32.SHGetKnownFolderPath(new Guid(b), (uint)option, IntPtr.Zero, out var ppszPath) == 0)
		{
			return ppszPath;
		}
		if (text == null)
		{
			return string.Empty;
		}
		return GetEnvironmentVariable(text) ?? string.Empty;
	}

	public static string[] GetLogicalDrives()
	{
		return DriveInfoInternal.GetLogicalDrives();
	}

	private unsafe static int GetSystemPageSize()
	{
		Unsafe.SkipInit(out Interop.Kernel32.SYSTEM_INFO sYSTEM_INFO);
		Interop.Kernel32.GetSystemInfo(&sYSTEM_INFO);
		return sYSTEM_INFO.dwPageSize;
	}

	private static string ExpandEnvironmentVariablesCore(string name)
	{
		Span<char> initialBuffer = stackalloc char[128];
		ValueStringBuilder valueStringBuilder = new ValueStringBuilder(initialBuffer);
		uint num;
		while ((num = Interop.Kernel32.ExpandEnvironmentStrings(name, ref valueStringBuilder.GetPinnableReference(), (uint)valueStringBuilder.Capacity)) > valueStringBuilder.Capacity)
		{
			valueStringBuilder.EnsureCapacity((int)num);
		}
		if (num == 0)
		{
			throw Win32Marshal.GetExceptionForLastWin32Error();
		}
		valueStringBuilder.Length = (int)(num - 1);
		return valueStringBuilder.ToString();
	}

	private unsafe static bool IsPrivilegedProcessCore()
	{
		SafeTokenHandle TokenHandle = null;
		try
		{
			if (Interop.Advapi32.OpenProcessToken(Interop.Kernel32.GetCurrentProcess(), 131080, out TokenHandle))
			{
				Interop.Advapi32.TOKEN_ELEVATION tOKEN_ELEVATION = default(Interop.Advapi32.TOKEN_ELEVATION);
				if (Interop.Advapi32.GetTokenInformation(TokenHandle, Interop.Advapi32.TOKEN_INFORMATION_CLASS.TokenElevation, &tOKEN_ELEVATION, (uint)sizeof(Interop.Advapi32.TOKEN_ELEVATION), out var _))
				{
					return tOKEN_ELEVATION.TokenIsElevated != Interop.BOOL.FALSE;
				}
			}
			throw Win32Marshal.GetExceptionForLastWin32Error();
		}
		finally
		{
			TokenHandle?.Dispose();
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static int GetProcessId()
	{
		return (int)Interop.Kernel32.GetCurrentProcessId();
	}

	private static string GetProcessPath()
	{
		Span<char> initialBuffer = stackalloc char[260];
		ValueStringBuilder valueStringBuilder = new ValueStringBuilder(initialBuffer);
		uint moduleFileName;
		while ((moduleFileName = Interop.Kernel32.GetModuleFileName(IntPtr.Zero, ref valueStringBuilder.GetPinnableReference(), (uint)valueStringBuilder.Capacity)) >= valueStringBuilder.Capacity)
		{
			valueStringBuilder.EnsureCapacity(valueStringBuilder.Capacity * 2);
		}
		if (moduleFileName == 0)
		{
			throw Win32Marshal.GetExceptionForLastWin32Error();
		}
		valueStringBuilder.Length = (int)moduleFileName;
		return valueStringBuilder.ToString();
	}

	private unsafe static OperatingSystem GetOSVersion()
	{
		if (Interop.NtDll.RtlGetVersionEx(out var osvi) != 0)
		{
			throw new InvalidOperationException(SR.InvalidOperation_GetVersion);
		}
		Version version = new Version((int)osvi.dwMajorVersion, (int)osvi.dwMinorVersion, (int)osvi.dwBuildNumber, 0);
		if (osvi.szCSDVersion[0] == '\0')
		{
			return new OperatingSystem(PlatformID.Win32NT, version);
		}
		return new OperatingSystem(PlatformID.Win32NT, version, new string(osvi.szCSDVersion));
	}

	private unsafe static string[] GetCommandLineArgsNative()
	{
		return SegmentCommandLine(Interop.Kernel32.GetCommandLine());
	}

	private unsafe static string[] SegmentCommandLine(char* cmdLine)
	{
		ArrayBuilder<string> arrayBuilder = default(ArrayBuilder<string>);
		Span<char> initialBuffer = stackalloc char[260];
		char* ptr = cmdLine;
		bool flag = false;
		ValueStringBuilder valueStringBuilder = new ValueStringBuilder(initialBuffer);
		char c;
		bool flag2;
		do
		{
			if (*ptr == '"')
			{
				flag = !flag;
				c = *(ptr++);
			}
			else
			{
				c = *(ptr++);
				valueStringBuilder.Append(c);
			}
			flag2 = c != '\0';
			if (flag2)
			{
				bool flag3 = flag;
				if (!flag3)
				{
					bool flag4 = ((c == '\t' || c == ' ') ? true : false);
					flag3 = !flag4;
				}
				flag2 = flag3;
			}
		}
		while (flag2);
		if (c == '\0')
		{
			ptr--;
		}
		valueStringBuilder.Length--;
		arrayBuilder.Add(valueStringBuilder.ToString());
		flag = false;
		while (true)
		{
			if (*ptr != 0)
			{
				while (true)
				{
					char c2 = *ptr;
					if ((c2 != '\t' && c2 != ' ') || 1 == 0)
					{
						break;
					}
					ptr++;
				}
			}
			if (*ptr == '\0')
			{
				break;
			}
			valueStringBuilder = new ValueStringBuilder(initialBuffer);
			while (true)
			{
				bool flag5 = true;
				int num = 0;
				while (*ptr == '\\')
				{
					ptr++;
					num++;
				}
				if (*ptr == '"')
				{
					if (num % 2 == 0)
					{
						if (flag && ptr[1] == '"')
						{
							ptr++;
						}
						else
						{
							flag5 = false;
							flag = !flag;
						}
					}
					num /= 2;
				}
				while (num-- > 0)
				{
					valueStringBuilder.Append('\\');
				}
				flag2 = *ptr == '\0';
				if (!flag2)
				{
					bool flag3 = !flag;
					if (flag3)
					{
						char c2 = *ptr;
						bool flag4 = ((c2 == '\t' || c2 == ' ') ? true : false);
						flag3 = flag4;
					}
					flag2 = flag3;
				}
				if (flag2)
				{
					break;
				}
				if (flag5)
				{
					valueStringBuilder.Append(*ptr);
				}
				ptr++;
			}
			arrayBuilder.Add(valueStringBuilder.ToString());
		}
		return arrayBuilder.ToArray();
	}

	private static string GetEnvironmentVariableCore(string variable)
	{
		Span<char> initialBuffer = stackalloc char[128];
		ValueStringBuilder valueStringBuilder = new ValueStringBuilder(initialBuffer);
		uint environmentVariable;
		while ((environmentVariable = Interop.Kernel32.GetEnvironmentVariable(variable, ref valueStringBuilder.GetPinnableReference(), (uint)valueStringBuilder.Capacity)) > valueStringBuilder.Capacity)
		{
			valueStringBuilder.EnsureCapacity((int)environmentVariable);
		}
		if (environmentVariable == 0 && Marshal.GetLastPInvokeError() == 203)
		{
			valueStringBuilder.Dispose();
			return null;
		}
		valueStringBuilder.Length = (int)environmentVariable;
		return valueStringBuilder.ToString();
	}

	internal static string GetEnvironmentVariableCore_NoArrayPool(string variable)
	{
		Span<char> span = stackalloc char[128];
		uint environmentVariable = Interop.Kernel32.GetEnvironmentVariable(variable, ref MemoryMarshal.GetReference(span), (uint)span.Length);
		if (environmentVariable == 0 || environmentVariable > span.Length)
		{
			return null;
		}
		return span.Slice(0, (int)environmentVariable).ToString();
	}

	private static void SetEnvironmentVariableCore(string variable, string value)
	{
		if (!Interop.Kernel32.SetEnvironmentVariable(variable, value))
		{
			int lastPInvokeError = Marshal.GetLastPInvokeError();
			switch (lastPInvokeError)
			{
			case 203:
				break;
			case 206:
				throw new ArgumentException(SR.Argument_LongEnvVarValue);
			case 8:
			case 1450:
				throw new OutOfMemoryException(Marshal.GetPInvokeErrorMessage(lastPInvokeError));
			default:
				throw new ArgumentException(Marshal.GetPInvokeErrorMessage(lastPInvokeError));
			}
		}
	}

	public unsafe static IDictionary GetEnvironmentVariables()
	{
		char* environmentStringsW = Interop.Kernel32.GetEnvironmentStringsW();
		if (environmentStringsW == null)
		{
			throw new OutOfMemoryException();
		}
		try
		{
			Hashtable hashtable = new Hashtable();
			char* ptr = environmentStringsW;
			while (true)
			{
				ReadOnlySpan<char> span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(ptr);
				if (span.IsEmpty)
				{
					break;
				}
				int num = span.IndexOf('=');
				if (num > 0)
				{
					string key = new string(span.Slice(0, num));
					string value = new string(span.Slice(num + 1));
					try
					{
						hashtable.Add(key, value);
					}
					catch (ArgumentException)
					{
					}
				}
				ptr += span.Length + 1;
			}
			return hashtable;
		}
		finally
		{
			Interop.Kernel32.FreeEnvironmentStringsW(environmentStringsW);
		}
	}
}
