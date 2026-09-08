using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;

namespace System.Runtime.InteropServices;

public sealed class PosixSignalRegistration : IDisposable
{
	private sealed class Token
	{
		public PosixSignal Signal { get; }

		public Action<PosixSignalContext> Handler { get; }

		public int SigNo { get; }

		public Token(PosixSignal signal, int sigNo, Action<PosixSignalContext> handler)
		{
			Signal = signal;
			Handler = handler;
			SigNo = sigNo;
		}
	}

	private Token _token;

	private static readonly Dictionary<int, List<Token>> s_registrations = new Dictionary<int, List<Token>>();

	[UnsupportedOSPlatform("android")]
	[UnsupportedOSPlatform("browser")]
	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("tvos")]
	public static PosixSignalRegistration Create(PosixSignal signal, Action<PosixSignalContext> handler)
	{
		ArgumentNullException.ThrowIfNull(handler, "handler");
		return Register(signal, handler);
	}

	private PosixSignalRegistration(Token token)
	{
		_token = token;
	}

	public void Dispose()
	{
		Unregister();
		GC.SuppressFinalize(this);
	}

	~PosixSignalRegistration()
	{
		Unregister();
	}

	private unsafe static PosixSignalRegistration Register(PosixSignal signal, Action<PosixSignalContext> handler)
	{
		int num = signal switch
		{
			PosixSignal.SIGINT => 0, 
			PosixSignal.SIGQUIT => 1, 
			PosixSignal.SIGTERM => 6, 
			PosixSignal.SIGHUP => 2, 
			_ => throw new PlatformNotSupportedException(), 
		};
		Token token = new Token(signal, num, handler);
		PosixSignalRegistration result = new PosixSignalRegistration(token);
		lock (s_registrations)
		{
			if (s_registrations.Count == 0 && !Interop.Kernel32.SetConsoleCtrlHandler((delegate* unmanaged<int, Interop.BOOL>)(&HandlerRoutine), true))
			{
				throw Win32Marshal.GetExceptionForLastWin32Error();
			}
			if (!s_registrations.TryGetValue(num, out var value))
			{
				value = (s_registrations[num] = new List<Token>());
			}
			value.Add(token);
			return result;
		}
	}

	private unsafe void Unregister()
	{
		lock (s_registrations)
		{
			Token token = _token;
			if (token == null)
			{
				return;
			}
			_token = null;
			if (!s_registrations.TryGetValue(token.SigNo, out var value))
			{
				return;
			}
			value.Remove(token);
			if (value.Count == 0)
			{
				s_registrations.Remove(token.SigNo);
			}
			if (s_registrations.Count == 0 && !Interop.Kernel32.SetConsoleCtrlHandler((delegate* unmanaged<int, Interop.BOOL>)(&HandlerRoutine), false))
			{
				int lastPInvokeError = Marshal.GetLastPInvokeError();
				if (lastPInvokeError != 87)
				{
					throw Win32Marshal.GetExceptionForWin32Error(lastPInvokeError);
				}
			}
		}
	}

	[UnmanagedCallersOnly]
	private static Interop.BOOL HandlerRoutine(int dwCtrlType)
	{
		Token[] array = null;
		lock (s_registrations)
		{
			if (s_registrations.TryGetValue(dwCtrlType, out var value))
			{
				array = new Token[value.Count];
				value.CopyTo(array);
			}
		}
		if (array == null)
		{
			return Interop.BOOL.FALSE;
		}
		PosixSignalContext posixSignalContext = new PosixSignalContext((PosixSignal)0);
		for (int num = array.Length - 1; num >= 0; num--)
		{
			Token token = array[num];
			posixSignalContext.Signal = token.Signal;
			token.Handler(posixSignalContext);
		}
		if (!posixSignalContext.Cancel)
		{
			return Interop.BOOL.FALSE;
		}
		return Interop.BOOL.TRUE;
	}
}
