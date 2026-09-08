using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Diagnostics;

internal sealed class StackFrameHelper
{
	private int[] rgiOffset;

	private int[] rgiILOffset;

	private object dynamicMethods;

	private nint[] rgMethodHandle;

	private string[] rgAssemblyPath;

	private Assembly[] rgAssembly;

	private nint[] rgLoadedPeAddress;

	private int[] rgiLoadedPeSize;

	private bool[] rgiIsFileLayout;

	private nint[] rgInMemoryPdbAddress;

	private int[] rgiInMemoryPdbSize;

	private int[] rgiMethodToken;

	private string[] rgFilename;

	private int[] rgiLineNumber;

	private int[] rgiColumnNumber;

	private bool[] rgiLastFrameFromForeignExceptionStackTrace;

	private int iFrameCount;

	private static object s_stackTraceSymbolsCache;

	[ThreadStatic]
	private static int t_reentrancy;

	[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
	[return: UnsafeAccessorType("System.Diagnostics.StackTraceSymbols, System.Diagnostics.StackTrace, Version=4.0.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	private static extern object CreateStackTraceSymbols();

	[UnsafeAccessor(UnsafeAccessorKind.Method)]
	private static extern void GetSourceLineInfo([UnsafeAccessorType("System.Diagnostics.StackTraceSymbols, System.Diagnostics.StackTrace, Version=4.0.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")] object target, Assembly assembly, string assemblyPath, nint loadedPeAddress, int loadedPeSize, bool isFileLayout, nint inMemoryPdbAddress, int inMemoryPdbSize, int methodToken, int ilOffset, out string sourceFile, out int sourceLine, out int sourceColumn);

	public StackFrameHelper()
	{
		rgMethodHandle = null;
		rgiMethodToken = null;
		rgiOffset = null;
		rgiILOffset = null;
		rgAssemblyPath = null;
		rgAssembly = null;
		rgLoadedPeAddress = null;
		rgiLoadedPeSize = null;
		rgiIsFileLayout = null;
		rgInMemoryPdbAddress = null;
		rgiInMemoryPdbSize = null;
		dynamicMethods = null;
		rgFilename = null;
		rgiLineNumber = null;
		rgiColumnNumber = null;
		rgiLastFrameFromForeignExceptionStackTrace = null;
		iFrameCount = 0;
	}

	internal void InitializeSourceInfo(bool fNeedFileInfo, Exception exception)
	{
		StackTrace.GetStackFramesInternal(this, fNeedFileInfo, exception);
		if (!fNeedFileInfo || t_reentrancy > 0)
		{
			return;
		}
		t_reentrancy++;
		try
		{
			if (s_stackTraceSymbolsCache == null)
			{
				Interlocked.CompareExchange(ref s_stackTraceSymbolsCache, CreateStackTraceSymbols(), null);
			}
			for (int i = 0; i < iFrameCount; i++)
			{
				if (rgiMethodToken[i] != 0)
				{
					GetSourceLineInfo(s_stackTraceSymbolsCache, rgAssembly[i], rgAssemblyPath[i], rgLoadedPeAddress[i], rgiLoadedPeSize[i], rgiIsFileLayout[i], rgInMemoryPdbAddress[i], rgiInMemoryPdbSize[i], rgiMethodToken[i], rgiILOffset[i], out rgFilename[i], out rgiLineNumber[i], out rgiColumnNumber[i]);
				}
			}
		}
		catch
		{
		}
		finally
		{
			t_reentrancy--;
		}
	}

	public MethodBase GetMethodBase(int i)
	{
		nint num = rgMethodHandle[i];
		if (num == IntPtr.Zero)
		{
			return null;
		}
		return RuntimeType.GetMethodBase(RuntimeMethodHandle.GetTypicalMethodDefinition(new RuntimeMethodInfoStub(new RuntimeMethodHandleInternal(num), this)));
	}

	public int GetOffset(int i)
	{
		return rgiOffset[i];
	}

	public int GetILOffset(int i)
	{
		return rgiILOffset[i];
	}

	public string GetFilename(int i)
	{
		string[] array = rgFilename;
		if (array == null)
		{
			return null;
		}
		return array[i];
	}

	public int GetLineNumber(int i)
	{
		if (rgiLineNumber != null)
		{
			return rgiLineNumber[i];
		}
		return 0;
	}

	public int GetColumnNumber(int i)
	{
		if (rgiColumnNumber != null)
		{
			return rgiColumnNumber[i];
		}
		return 0;
	}

	public bool IsLastFrameFromForeignExceptionStackTrace(int i)
	{
		if (rgiLastFrameFromForeignExceptionStackTrace != null)
		{
			return rgiLastFrameFromForeignExceptionStackTrace[i];
		}
		return false;
	}

	public int GetNumberOfFrames()
	{
		return iFrameCount;
	}
}
