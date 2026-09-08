using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

internal static class AvatarFarmPortableLauncher
{
	private enum StartupResult
	{
		Ready,
		Exited,
		Stalled
	}

	private struct JobObjectBasicLimitInformation
	{
		public long PerProcessUserTimeLimit;

		public long PerJobUserTimeLimit;

		public uint LimitFlags;

		public UIntPtr MinimumWorkingSetSize;

		public UIntPtr MaximumWorkingSetSize;

		public uint ActiveProcessLimit;

		public UIntPtr Affinity;

		public uint PriorityClass;

		public uint SchedulingClass;
	}

	private struct IoCounters
	{
		public ulong ReadOperationCount;

		public ulong WriteOperationCount;

		public ulong OtherOperationCount;

		public ulong ReadTransferCount;

		public ulong WriteTransferCount;

		public ulong OtherTransferCount;
	}

	private struct JobObjectExtendedLimitInformationData
	{
		public JobObjectBasicLimitInformation BasicLimitInformation;

		public IoCounters IoInfo;

		public UIntPtr ProcessMemoryLimit;

		public UIntPtr JobMemoryLimit;

		public UIntPtr PeakProcessMemoryUsed;

		public UIntPtr PeakJobMemoryUsed;
	}

	private const uint JobObjectExtendedLimitInformation = 9u;

	private const uint JobObjectLimitKillOnJobClose = 8192u;

	private const int InterruptedGraphicsCooldownMilliseconds = 20000;

	private const int StartupPollMilliseconds = 250;

	private const int StartupPresentTimeoutMilliseconds = 60000;

	private const int StartupWatchdogDisarmMilliseconds = 120000;

	private const int StartupRetryCount = 1;

	[STAThread]
	private static int Main(string[] args)
	{
		string fullPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
		string text = Path.Combine(fullPath, "Game");
		string text2 = Path.Combine(text, "AvatarFarmOnline.exe");
		string text3 = Path.Combine(text, "UserData");
		string path = Path.Combine(text3, "launcher-session.state");
		try
		{
			Directory.CreateDirectory(text3);
			if (!File.Exists(text2))
			{
				throw new FileNotFoundException("The portable game executable is missing.", text2);
			}
			bool createdNew;
			using Mutex mutex = new Mutex(initiallyOwned: true, BuildMutexName(fullPath), out createdNew);
			if (!createdNew)
			{
				ShowMessage("Avatar Farm Online is already running from this portable folder.", "Avatar Farm Online");
				return 0;
			}
			if (WasRecentSessionInterrupted(path))
			{
				Thread.Sleep(20000);
			}
			WriteSessionState(path, "RUNNING");
			int num = int.MinValue;
			try
			{
				num = RunSupervised(text2, text, args);
				return num;
			}
			finally
			{
				WriteSessionState(path, num switch
				{
					int.MinValue => "LAUNCH_FAILED", 
					0 => "CLEAN exit=0", 
					_ => "ABNORMAL exit=" + num, 
				});
				if (num != 0 && num != int.MinValue)
				{
					AppendChildExit(text3, num);
				}
				mutex.ReleaseMutex();
			}
		}
		catch (Exception ex)
		{
			try
			{
				Directory.CreateDirectory(text3);
				File.AppendAllText(Path.Combine(text3, "launcher-error.log"), string.Concat(DateTime.UtcNow.ToString("O"), Environment.NewLine, ex, Environment.NewLine, Environment.NewLine));
			}
			catch
			{
			}
			ShowMessage("Avatar Farm Online could not start.\n\n" + ex.Message + "\n\nDetails were saved under Game\\UserData.", "Avatar Farm Online");
			return 1;
		}
	}

	private static bool WasRecentSessionInterrupted(string path)
	{
		try
		{
			if (!File.Exists(path))
			{
				return false;
			}
			string text = File.ReadAllText(path);
			if (!text.StartsWith("RUNNING", StringComparison.Ordinal))
			{
				return false;
			}
			TimeSpan timeSpan = DateTime.UtcNow - File.GetLastWriteTimeUtc(path);
			return timeSpan >= TimeSpan.Zero && timeSpan < TimeSpan.FromMinutes(5.0);
		}
		catch
		{
			return false;
		}
	}

	private static void WriteSessionState(string path, string state)
	{
		try
		{
			File.WriteAllText(path, state + " " + DateTime.UtcNow.ToString("O") + Environment.NewLine);
		}
		catch
		{
		}
	}

	private static void AppendChildExit(string userData, int exitCode)
	{
		try
		{
			File.AppendAllText(Path.Combine(userData, "launcher-child-exit.log"), DateTime.UtcNow.ToString("O") + " exit=" + exitCode + Environment.NewLine);
		}
		catch
		{
		}
	}

	private static int RunSupervised(string executable, string workingDirectory, string[] args)
	{
		IntPtr intPtr = CreateJobObject(IntPtr.Zero, null);
		if (intPtr == IntPtr.Zero)
		{
			throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not create the game supervisor.");
		}
		try
		{
			JobObjectExtendedLimitInformationData structure = new JobObjectExtendedLimitInformationData
			{
				BasicLimitInformation = 
				{
					LimitFlags = 8192u
				}
			};
			int num = Marshal.SizeOf(typeof(JobObjectExtendedLimitInformationData));
			IntPtr intPtr2 = Marshal.AllocHGlobal(num);
			try
			{
				Marshal.StructureToPtr(structure, intPtr2, fDeleteOld: false);
				if (!SetInformationJobObject(intPtr, 9u, intPtr2, (uint)num))
				{
					throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not configure the game supervisor.");
				}
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr2);
			}
			string text = Path.Combine(workingDirectory, "UserData", "compat-hooks.log");
			for (int i = 0; i <= 1; i++)
			{
				long fileLength = GetFileLength(text);
				using (Process process = Process.Start(BuildStartInfo(executable, workingDirectory, args)))
				{
					if (process == null)
					{
						throw new InvalidOperationException("Windows did not create the game process.");
					}
					if (!AssignProcessToJobObject(intPtr, process.Handle))
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						try
						{
							process.Kill();
							process.WaitForExit();
						}
						catch
						{
						}
						throw new Win32Exception(lastWin32Error, "Could not attach the game to its supervisor.");
					}
					string stallReason;
					switch (WaitForStartup(process, text, fileLength, out stallReason))
					{
					case StartupResult.Exited:
						return process.ExitCode;
					case StartupResult.Ready:
						if (i > 0)
						{
							AppendStartupRecovery(workingDirectory, "recovered attempt=" + (i + 1) + " pid=" + process.Id);
						}
						process.WaitForExit();
						return process.ExitCode;
					default:
						AppendStartupRecovery(workingDirectory, "stalled attempt=" + (i + 1) + " pid=" + process.Id + " reason=" + stallReason);
						try
						{
							if (!process.HasExited)
							{
								process.Kill();
							}
							if (!process.WaitForExit(10000))
							{
								throw new InvalidOperationException("The stalled game process did not exit.");
							}
						}
						catch (Exception innerException)
						{
							throw new InvalidOperationException("The launcher could not retire the stalled game before its recovery attempt.", innerException);
						}
						break;
					}
				}
				if (i < 1)
				{
					Thread.Sleep(20000);
				}
			}
			throw new InvalidOperationException("The graphics driver did not complete the first frame after an automatic recovery attempt. Restart Windows before launching the game again.");
		}
		finally
		{
			CloseHandle(intPtr);
		}
	}

	private static ProcessStartInfo BuildStartInfo(string executable, string workingDirectory, string[] args)
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = executable;
		processStartInfo.WorkingDirectory = workingDirectory;
		processStartInfo.UseShellExecute = false;
		processStartInfo.Arguments = JoinArguments(args);
		ProcessStartInfo processStartInfo2 = processStartInfo;
		processStartInfo2.EnvironmentVariables["FNA3D_FORCE_DRIVER"] = "D3D11";
		processStartInfo2.EnvironmentVariables["SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS"] = "0";
		processStartInfo2.EnvironmentVariables["SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS"] = "1";
		processStartInfo2.EnvironmentVariables["SDL_GAMECONTROLLERCONFIG_FILE"] = Path.Combine(workingDirectory, "gamecontrollerdb.txt");
		return processStartInfo2;
	}

	private static StartupResult WaitForStartup(Process game, string startupLog, long startupLogOffset, out string stallReason)
	{
		DateTime utcNow = DateTime.UtcNow;
		DateTime? dateTime = null;
		StringBuilder stringBuilder = new StringBuilder();
		while (true)
		{
			if (game.HasExited)
			{
				stallReason = string.Empty;
				return StartupResult.Exited;
			}
			string text = ReadAppendedLog(startupLog, ref startupLogOffset);
			if (text.Length > 0)
			{
				stringBuilder.Append(text);
				if (HasPidMarker(stringBuilder, game.Id, "main-thread-preload-complete"))
				{
					stallReason = string.Empty;
					return StartupResult.Ready;
				}
				if (!dateTime.HasValue && HasPidMarker(stringBuilder, game.Id, "RenderItem.PineTreeModel"))
				{
					dateTime = DateTime.UtcNow;
				}
				if (stringBuilder.Length > 262144)
				{
					stringBuilder.Remove(0, stringBuilder.Length - 131072);
				}
			}
			DateTime utcNow2 = DateTime.UtcNow;
			if (dateTime.HasValue && (utcNow2 - dateTime.Value).TotalMilliseconds >= 60000.0)
			{
				stallReason = "first-present-no-progress";
				return StartupResult.Stalled;
			}
			if ((utcNow2 - utcNow).TotalMilliseconds >= 120000.0 && !dateTime.HasValue)
			{
				break;
			}
			Thread.Sleep(250);
		}
		stallReason = string.Empty;
		return StartupResult.Ready;
	}

	private static long GetFileLength(string path)
	{
		try
		{
			return File.Exists(path) ? new FileInfo(path).Length : 0;
		}
		catch
		{
			return 0L;
		}
	}

	private static string ReadAppendedLog(string path, ref long offset)
	{
		try
		{
			using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
			if (fileStream.Length < offset)
			{
				offset = 0L;
			}
			fileStream.Position = Math.Min(offset, fileStream.Length);
			using StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, 4096, leaveOpen: false);
			string result = streamReader.ReadToEnd();
			offset = fileStream.Position;
			return result;
		}
		catch
		{
			return string.Empty;
		}
	}

	private static bool HasPidMarker(StringBuilder text, int processId, string marker)
	{
		string value = "pid=" + processId + " ";
		using (StringReader stringReader = new StringReader(text.ToString()))
		{
			string text2;
			while ((text2 = stringReader.ReadLine()) != null)
			{
				if (text2.IndexOf(value, StringComparison.Ordinal) >= 0 && text2.IndexOf(marker, StringComparison.Ordinal) >= 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static void AppendStartupRecovery(string workingDirectory, string message)
	{
		try
		{
			string text = Path.Combine(workingDirectory, "UserData");
			Directory.CreateDirectory(text);
			File.AppendAllText(Path.Combine(text, "launcher-startup-recovery.log"), DateTime.UtcNow.ToString("O") + " " + message + Environment.NewLine);
		}
		catch
		{
		}
	}

	private static string BuildMutexName(string root)
	{
		byte[] array;
		using (SHA256 sHA = SHA256.Create())
		{
			array = sHA.ComputeHash(Encoding.UTF8.GetBytes(root.ToUpperInvariant()));
		}
		StringBuilder stringBuilder = new StringBuilder(24);
		for (int i = 0; i < 12; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return "Local\\AvatarFarmOnlinePortable_" + stringBuilder;
	}

	private static string JoinArguments(string[] args)
	{
		if (args == null || args.Length == 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string text in args)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append(QuoteArgument(text ?? string.Empty));
		}
		return stringBuilder.ToString();
	}

	private static string QuoteArgument(string argument)
	{
		if (argument.Length > 0 && argument.IndexOfAny(new char[5] { ' ', '\t', '\n', '\v', '"' }) < 0)
		{
			return argument;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('"');
		int num = 0;
		foreach (char c in argument)
		{
			switch (c)
			{
			case '\\':
				num++;
				break;
			case '"':
				stringBuilder.Append('\\', num * 2 + 1);
				stringBuilder.Append('"');
				num = 0;
				break;
			default:
				stringBuilder.Append('\\', num);
				num = 0;
				stringBuilder.Append(c);
				break;
			}
		}
		stringBuilder.Append('\\', num * 2);
		stringBuilder.Append('"');
		return stringBuilder.ToString();
	}

	private static void ShowMessage(string message, string title)
	{
		MessageBox(IntPtr.Zero, message, title, 16u);
	}

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern IntPtr CreateJobObject(IntPtr securityAttributes, string name);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool SetInformationJobObject(IntPtr job, uint informationClass, IntPtr information, uint informationLength);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool CloseHandle(IntPtr handle);

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	private static extern int MessageBox(IntPtr window, string text, string caption, uint type);
}
