using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

namespace Microsoft.VisualBasic;

[StandardModule]
public sealed class Interaction
{
	private static SortedList m_SortedEnvList;

	private static string m_CommandLine;

	private static object m_EnvironSyncObject = new object();

	public static int Shell(string PathName, AppWinStyle Style = AppWinStyle.MinimizedFocus, bool Wait = false, int Timeout = -1)
	{
		return (int)InvokeMethod("Shell", PathName, Style, Wait, Timeout);
	}

	public static void AppActivate(int ProcessId)
	{
		InvokeMethod("AppActivateByProcessId", ProcessId);
	}

	public static void AppActivate(string Title)
	{
		InvokeMethod("AppActivateByTitle", Title);
	}

	public static string Command()
	{
		if (m_CommandLine == null)
		{
			string text = Environment.CommandLine;
			if (text == null || text.Length == 0)
			{
				return "";
			}
			int length = Environment.GetCommandLineArgs()[0].Length;
			int num = default(int);
			do
			{
				num = text.IndexOf('"', num);
				if (num >= 0 && num <= length)
				{
					text = text.Remove(num, 1);
				}
			}
			while (num >= 0 && num <= length);
			if (num == 0 || num > text.Length)
			{
				m_CommandLine = "";
			}
			else
			{
				m_CommandLine = Strings.LTrim(text.Substring(length));
			}
		}
		return m_CommandLine;
	}

	public static string Environ(int Expression)
	{
		if (Expression <= 0 || Expression > 255)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_Range1toFF1, "Expression"));
		}
		if (m_SortedEnvList == null)
		{
			object environSyncObject = m_EnvironSyncObject;
			ObjectFlowControl.CheckForSyncLockOnValueType(environSyncObject);
			bool lockTaken = false;
			try
			{
				Monitor.Enter(environSyncObject, ref lockTaken);
				if (m_SortedEnvList == null)
				{
					m_SortedEnvList = new SortedList(Environment.GetEnvironmentVariables());
				}
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(environSyncObject);
				}
			}
		}
		if (Expression > m_SortedEnvList.Count)
		{
			return "";
		}
		checked
		{
			string? text = m_SortedEnvList.GetKey(Expression - 1).ToString();
			string text2 = m_SortedEnvList.GetByIndex(Expression - 1).ToString();
			return text + "=" + text2;
		}
	}

	public static string Environ(string Expression)
	{
		Expression = Strings.Trim(Expression);
		if (Expression.Length == 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Expression"));
		}
		return Environment.GetEnvironmentVariable(Expression);
	}

	[SupportedOSPlatform("windows")]
	public static void Beep()
	{
		UnsafeNativeMethods.MessageBeep(0);
	}

	public static string InputBox(string Prompt, string Title = "", string DefaultResponse = "", int XPos = -1, int YPos = -1)
	{
		return (string)InvokeMethod("InputBox", Prompt, Title, DefaultResponse, XPos, YPos);
	}

	public static MsgBoxResult MsgBox(object Prompt, MsgBoxStyle Buttons = MsgBoxStyle.OkOnly, object Title = null)
	{
		return (MsgBoxResult)InvokeMethod("MsgBox", Prompt, Buttons, Title);
	}

	private static object InvokeMethod(string methodName, params object[] args)
	{
		return (Type.GetType("Microsoft.VisualBasic._Interaction, Microsoft.VisualBasic.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", throwOnError: false)?.GetMethod(methodName) ?? throw new PlatformNotSupportedException(System.SR.MethodRequiresSystemWindowsForms)).Invoke(null, BindingFlags.DoNotWrapExceptions, null, args, null);
	}

	public static object Choose(double Index, params object[] Choice)
	{
		int num = checked((int)Math.Round(Conversion.Fix(Index) - 1.0));
		if (Choice.Rank != 1)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_RankEQOne1, "Choice"));
		}
		if (num < 0 || num > Choice.GetUpperBound(0))
		{
			return null;
		}
		return Choice[num];
	}

	public static object IIf(bool Expression, object TruePart, object FalsePart)
	{
		if (Expression)
		{
			return TruePart;
		}
		return FalsePart;
	}

	internal static T IIf<T>(bool condition, T truePart, T falsePart)
	{
		if (condition)
		{
			return truePart;
		}
		return falsePart;
	}

	public static string Partition(long Number, long Start, long Stop, long Interval)
	{
		string Buffer = null;
		if (Start < 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Start"));
		}
		if (Stop <= Start)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Stop"));
		}
		if (Interval < 1)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Interval"));
		}
		checked
		{
			long num = default(long);
			bool flag = default(bool);
			long num2 = default(long);
			bool flag2 = default(bool);
			if (Number < Start)
			{
				num = Start - 1;
				flag = true;
			}
			else if (Number > Stop)
			{
				num2 = Stop + 1;
				flag2 = true;
			}
			else if (Interval == 1)
			{
				num2 = Number;
				num = Number;
			}
			else
			{
				num2 = unchecked(checked(Number - Start) / Interval) * Interval + Start;
				num = num2 + Interval - 1;
				if (num > Stop)
				{
					num = Stop;
				}
				if (num2 < Start)
				{
					num2 = Start;
				}
			}
			string expression = Conversions.ToString(Stop + 1);
			string expression2 = Conversions.ToString(Start - 1);
			long num3 = ((Strings.Len(expression) <= Strings.Len(expression2)) ? Strings.Len(expression2) : Strings.Len(expression));
			if (flag)
			{
				expression = Conversions.ToString(num);
				if (num3 < Strings.Len(expression))
				{
					num3 = Strings.Len(expression);
				}
			}
			if (flag)
			{
				InsertSpaces(ref Buffer, num3);
			}
			else
			{
				InsertNumber(ref Buffer, num2, num3);
			}
			Buffer += ":";
			if (flag2)
			{
				InsertSpaces(ref Buffer, num3);
			}
			else
			{
				InsertNumber(ref Buffer, num, num3);
			}
			return Buffer;
		}
	}

	private static void InsertSpaces(ref string Buffer, long Spaces)
	{
		while (Spaces > 0)
		{
			Buffer += " ";
			Spaces = checked(Spaces - 1);
		}
	}

	private static void InsertNumber(ref string Buffer, long Num, long Spaces)
	{
		string text = Conversions.ToString(Num);
		InsertSpaces(ref Buffer, checked(Spaces - Strings.Len(text)));
		Buffer += text;
	}

	public static object Switch(params object[] VarExpr)
	{
		if (VarExpr == null)
		{
			return null;
		}
		int num = VarExpr.Length;
		int num2 = 0;
		if (num % 2 != 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "VarExpr"));
		}
		checked
		{
			while (num > 0)
			{
				if (Conversions.ToBoolean(VarExpr[num2]))
				{
					return VarExpr[num2 + 1];
				}
				num2 += 2;
				num -= 2;
			}
			return null;
		}
	}

	[SupportedOSPlatform("windows")]
	public static void DeleteSetting(string AppName, string Section = null, string Key = null)
	{
		RegistryKey registryKey = null;
		CheckPathComponent(AppName);
		string text = FormRegKey(AppName, Section);
		try
		{
			RegistryKey currentUser = Registry.CurrentUser;
			if (Information.IsNothing(Key) || Key.Length == 0)
			{
				currentUser.DeleteSubKeyTree(text);
				return;
			}
			registryKey = currentUser.OpenSubKey(text, writable: true);
			if (registryKey == null)
			{
				throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Section"));
			}
			registryKey.DeleteValue(Key);
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			registryKey?.Close();
		}
	}

	[SupportedOSPlatform("windows")]
	public static string[,] GetAllSettings(string AppName, string Section)
	{
		CheckPathComponent(AppName);
		CheckPathComponent(Section);
		string name = FormRegKey(AppName, Section);
		RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(name);
		checked
		{
			string[,] result;
			if (registryKey == null)
			{
				result = null;
			}
			else
			{
				result = null;
				try
				{
					if (registryKey.ValueCount != 0)
					{
						string[] valueNames = registryKey.GetValueNames();
						int upperBound = valueNames.GetUpperBound(0);
						string[,] array = new string[upperBound + 1, 2];
						int num = upperBound;
						for (int i = 0; i <= num; i++)
						{
							object value = registryKey.GetValue(array[i, 0] = valueNames[i]);
							if (value != null && value is string)
							{
								array[i, 1] = value.ToString();
							}
						}
						result = array;
					}
				}
				catch (StackOverflowException ex)
				{
					throw ex;
				}
				catch (OutOfMemoryException ex2)
				{
					throw ex2;
				}
				catch (Exception)
				{
				}
				finally
				{
					registryKey.Close();
				}
			}
			return result;
		}
	}

	[SupportedOSPlatform("windows")]
	public static string GetSetting(string AppName, string Section, string Key, string Default = "")
	{
		RegistryKey registryKey = null;
		CheckPathComponent(AppName);
		CheckPathComponent(Section);
		CheckPathComponent(Key);
		if (Default == null)
		{
			Default = "";
		}
		string name = FormRegKey(AppName, Section);
		object value;
		try
		{
			registryKey = Registry.CurrentUser.OpenSubKey(name);
			if (registryKey == null)
			{
				return Default;
			}
			value = registryKey.GetValue(Key, Default);
		}
		finally
		{
			registryKey?.Close();
		}
		if (value == null)
		{
			return null;
		}
		if (value is string)
		{
			return (string)value;
		}
		throw new ArgumentException(System.SR.Argument_InvalidValue);
	}

	[SupportedOSPlatform("windows")]
	public static void SaveSetting(string AppName, string Section, string Key, string Setting)
	{
		CheckPathComponent(AppName);
		CheckPathComponent(Section);
		CheckPathComponent(Key);
		string text = FormRegKey(AppName, Section);
		RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(text);
		if (registryKey == null)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Interaction_ResKeyNotCreated1, text));
		}
		try
		{
			registryKey.SetValue(Key, Setting);
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			registryKey.Close();
		}
	}

	private static string FormRegKey(string sApp, string sSect)
	{
		if (Information.IsNothing(sApp) || sApp.Length == 0)
		{
			return "Software\\VB and VBA Program Settings";
		}
		if (Information.IsNothing(sSect) || sSect.Length == 0)
		{
			return "Software\\VB and VBA Program Settings\\" + sApp;
		}
		return "Software\\VB and VBA Program Settings\\" + sApp + "\\" + sSect;
	}

	private static void CheckPathComponent(string s)
	{
		if (s == null || s.Length == 0)
		{
			throw new ArgumentException(System.SR.Argument_PathNullOrEmpty);
		}
	}

	[SupportedOSPlatform("windows")]
	[RequiresUnreferencedCode("The COM object to be created cannot be statically analyzed and may be trimmed")]
	public static object CreateObject(string ProgId, string ServerName = "")
	{
		if (ProgId.Length == 0)
		{
			throw ExceptionUtils.VbMakeException(429);
		}
		if (ServerName == null || ServerName.Length == 0)
		{
			ServerName = null;
		}
		else if (string.Equals(Environment.MachineName, ServerName, StringComparison.OrdinalIgnoreCase))
		{
			ServerName = null;
		}
		try
		{
			Type type = ((ServerName != null) ? Type.GetTypeFromProgID(ProgId, ServerName, throwOnError: true) : Type.GetTypeFromProgID(ProgId));
			return Activator.CreateInstance(type);
		}
		catch (COMException ex)
		{
			if (ex.ErrorCode == -2147023174)
			{
				throw ExceptionUtils.VbMakeException(462);
			}
			throw ExceptionUtils.VbMakeException(429);
		}
		catch (StackOverflowException ex2)
		{
			throw ex2;
		}
		catch (OutOfMemoryException ex3)
		{
			throw ex3;
		}
		catch (Exception)
		{
			throw ExceptionUtils.VbMakeException(429);
		}
	}

	[SupportedOSPlatform("windows")]
	[RequiresUnreferencedCode("The COM component to be returned cannot be statically analyzed and may be trimmed")]
	public static object GetObject(string PathName = null, string Class = null)
	{
		if (Strings.Len(Class) == 0)
		{
			try
			{
				return Marshal.BindToMoniker(PathName);
			}
			catch (StackOverflowException ex)
			{
				throw ex;
			}
			catch (OutOfMemoryException ex2)
			{
				throw ex2;
			}
			catch (Exception)
			{
				throw ExceptionUtils.VbMakeException(429);
			}
		}
		if (PathName == null)
		{
			return null;
		}
		if (Strings.Len(PathName) == 0)
		{
			try
			{
				return Activator.CreateInstance(Type.GetTypeFromProgID(Class));
			}
			catch (StackOverflowException ex4)
			{
				throw ex4;
			}
			catch (OutOfMemoryException ex5)
			{
				throw ex5;
			}
			catch (Exception)
			{
				throw ExceptionUtils.VbMakeException(429);
			}
		}
		return null;
	}

	[RequiresUnreferencedCode("The type of ObjectRef cannot be statically analyzed and its members may be trimmed.")]
	public static object CallByName(object ObjectRef, string ProcName, CallType UseCallType, params object[] Args)
	{
		switch (UseCallType)
		{
		case CallType.Method:
			return LateBinding.InternalLateCall(ObjectRef, null, ProcName, Args, null, null, IgnoreReturn: false);
		case CallType.Get:
			return LateBinding.LateGet(ObjectRef, null, ProcName, Args, null, null);
		case CallType.Let:
		case CallType.Set:
		{
			Type objType = null;
			LateBinding.InternalLateSet(ObjectRef, ref objType, ProcName, Args, null, OptimisticSet: false, UseCallType);
			return null;
		}
		default:
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "CallType"));
		}
	}
}
