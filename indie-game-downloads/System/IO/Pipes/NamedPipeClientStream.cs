using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes;

public sealed class NamedPipeClientStream : PipeStream
{
	private readonly string _normalizedPipePath;

	private readonly TokenImpersonationLevel _impersonationLevel;

	private readonly PipeOptions _pipeOptions;

	private readonly HandleInheritability _inheritability;

	private readonly PipeDirection _direction;

	private readonly int _accessRights;

	[SupportedOSPlatform("windows")]
	public unsafe int NumberOfServerInstances
	{
		get
		{
			CheckPipePropertyOperations();
			Unsafe.SkipInit(out uint result);
			if (!global::Interop.Kernel32.GetNamedPipeHandleStateW(base.InternalHandle, null, &result, null, null, null, 0u))
			{
				throw WinIOError(Marshal.GetLastPInvokeError());
			}
			return (int)result;
		}
	}

	public NamedPipeClientStream(string pipeName)
		: this(".", pipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None, HandleInheritability.None)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName)
		: this(serverName, pipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None, HandleInheritability.None)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction)
		: this(serverName, pipeName, direction, PipeOptions.None, TokenImpersonationLevel.None, HandleInheritability.None)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options)
		: this(serverName, pipeName, direction, options, TokenImpersonationLevel.None, HandleInheritability.None)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options, TokenImpersonationLevel impersonationLevel)
		: this(serverName, pipeName, direction, options, impersonationLevel, HandleInheritability.None)
	{
	}

	public NamedPipeClientStream(string serverName, string pipeName, PipeDirection direction, PipeOptions options, TokenImpersonationLevel impersonationLevel, HandleInheritability inheritability)
		: base(direction, 0)
	{
		ArgumentException.ThrowIfNullOrEmpty(pipeName, "pipeName");
		ArgumentNullException.ThrowIfNull(serverName, "serverName");
		if (serverName.Length == 0)
		{
			throw new ArgumentException(System.SR.Argument_EmptyServerName);
		}
		if ((options & (PipeOptions)0x1FFFFFFF) != PipeOptions.None)
		{
			throw new ArgumentOutOfRangeException("options", System.SR.ArgumentOutOfRange_OptionsInvalid);
		}
		if (impersonationLevel < TokenImpersonationLevel.None || impersonationLevel > TokenImpersonationLevel.Delegation)
		{
			throw new ArgumentOutOfRangeException("impersonationLevel", System.SR.ArgumentOutOfRange_ImpersonationInvalid);
		}
		if (inheritability < HandleInheritability.None || inheritability > HandleInheritability.Inheritable)
		{
			throw new ArgumentOutOfRangeException("inheritability", System.SR.ArgumentOutOfRange_HandleInheritabilityNoneOrInheritable);
		}
		if ((options & PipeOptions.CurrentUserOnly) != PipeOptions.None)
		{
			base.IsCurrentUserOnly = true;
		}
		_normalizedPipePath = PipeStream.GetPipePath(serverName, pipeName);
		_direction = direction;
		_inheritability = inheritability;
		_impersonationLevel = impersonationLevel;
		_pipeOptions = options;
		_accessRights = AccessRightsFromDirection(direction);
	}

	public NamedPipeClientStream(PipeDirection direction, bool isAsync, bool isConnected, SafePipeHandle safePipeHandle)
		: base(direction, 0)
	{
		ArgumentNullException.ThrowIfNull(safePipeHandle, "safePipeHandle");
		if (safePipeHandle.IsInvalid)
		{
			throw new ArgumentException(System.SR.Argument_InvalidHandle, "safePipeHandle");
		}
		PipeStream.ValidateHandleIsPipe(safePipeHandle);
		InitializeHandle(safePipeHandle, isExposed: true, isAsync);
		if (isConnected)
		{
			base.State = PipeState.Connected;
		}
	}

	~NamedPipeClientStream()
	{
		Dispose(disposing: false);
	}

	public void Connect()
	{
		Connect(-1);
	}

	public void Connect(int timeout)
	{
		CheckConnectOperationsClient();
		ArgumentOutOfRangeException.ThrowIfLessThan(timeout, -1, "timeout");
		ConnectInternal(timeout, CancellationToken.None, Environment.TickCount);
	}

	public void Connect(TimeSpan timeout)
	{
		Connect(ToTimeoutMilliseconds(timeout));
	}

	private void ConnectInternal(int timeout, CancellationToken cancellationToken, int startTime)
	{
		int num = 0;
		SpinWait spinWait = default(SpinWait);
		do
		{
			cancellationToken.ThrowIfCancellationRequested();
			int num2 = ((timeout == -1) ? 50 : (timeout - num));
			if (cancellationToken.CanBeCanceled && num2 > 50)
			{
				num2 = 50;
			}
			if (TryConnect(num2))
			{
				return;
			}
			spinWait.SpinOnce();
		}
		while (timeout == -1 || (num = Environment.TickCount - startTime) < timeout);
		throw new TimeoutException();
	}

	public Task ConnectAsync()
	{
		return ConnectAsync(-1, CancellationToken.None);
	}

	public Task ConnectAsync(int timeout)
	{
		return ConnectAsync(timeout, CancellationToken.None);
	}

	public Task ConnectAsync(CancellationToken cancellationToken)
	{
		return ConnectAsync(-1, cancellationToken);
	}

	public Task ConnectAsync(int timeout, CancellationToken cancellationToken)
	{
		CheckConnectOperationsClient();
		ArgumentOutOfRangeException.ThrowIfLessThan(timeout, -1, "timeout");
		if (cancellationToken.IsCancellationRequested)
		{
			return Task.FromCanceled(cancellationToken);
		}
		int tickCount = Environment.TickCount;
		return Task.Factory.StartNew(delegate(object state)
		{
			(NamedPipeClientStream, int, CancellationToken, int) tuple = ((NamedPipeClientStream, int, CancellationToken, int))state;
			tuple.Item1.ConnectInternal(tuple.Item2, tuple.Item3, tuple.Item4);
		}, (this, timeout, cancellationToken, tickCount), cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
	}

	public Task ConnectAsync(TimeSpan timeout, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ConnectAsync(ToTimeoutMilliseconds(timeout), cancellationToken);
	}

	private static int ToTimeoutMilliseconds(TimeSpan timeout)
	{
		long num = (long)timeout.TotalMilliseconds;
		ArgumentOutOfRangeException.ThrowIfLessThan(num, -1L, "timeout");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(num, 2147483647L, "timeout");
		return (int)num;
	}

	protected internal override void CheckPipePropertyOperations()
	{
		base.CheckPipePropertyOperations();
		if (base.State == PipeState.WaitingToConnect)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_PipeNotYetConnected);
		}
		if (base.State == PipeState.Broken)
		{
			throw new IOException(System.SR.IO_PipeBroken);
		}
	}

	private void CheckConnectOperationsClient()
	{
		if (base.State == PipeState.Connected)
		{
			throw new InvalidOperationException(System.SR.InvalidOperation_PipeAlreadyConnected);
		}
		if (base.State == PipeState.Closed)
		{
			throw Error.GetPipeNotOpen();
		}
	}

	[SupportedOSPlatform("windows")]
	public NamedPipeClientStream(string serverName, string pipeName, PipeAccessRights desiredAccessRights, PipeOptions options, TokenImpersonationLevel impersonationLevel, HandleInheritability inheritability)
		: this(serverName, pipeName, DirectionFromRights(desiredAccessRights, "desiredAccessRights"), options, impersonationLevel, inheritability)
	{
		_accessRights = (int)desiredAccessRights;
	}

	private static PipeDirection DirectionFromRights(PipeAccessRights desiredAccessRights, [CallerArgumentExpression("desiredAccessRights")] string argumentName = null)
	{
		if ((desiredAccessRights & ~(PipeAccessRights.FullControl | PipeAccessRights.AccessSystemSecurity)) != 0 || (desiredAccessRights & (PipeAccessRights.ReadData | PipeAccessRights.WriteData)) == 0)
		{
			throw new ArgumentOutOfRangeException(argumentName, System.SR.ArgumentOutOfRange_NeedValidPipeAccessRights);
		}
		PipeDirection pipeDirection = (PipeDirection)0;
		if ((desiredAccessRights & PipeAccessRights.ReadData) != 0)
		{
			pipeDirection |= PipeDirection.In;
		}
		if ((desiredAccessRights & PipeAccessRights.WriteData) != 0)
		{
			pipeDirection |= PipeDirection.Out;
		}
		return pipeDirection;
	}

	private static int AccessRightsFromDirection(PipeDirection direction)
	{
		int num = 0;
		if ((PipeDirection.In & direction) != 0)
		{
			num |= int.MinValue;
		}
		if ((PipeDirection.Out & direction) != 0)
		{
			num |= 0x40000000;
		}
		return num;
	}

	private bool TryConnect(int timeout)
	{
		global::Interop.Kernel32.SECURITY_ATTRIBUTES secAttrs = PipeStream.GetSecAttrs(_inheritability);
		int num = (int)(_pipeOptions & ~PipeOptions.CurrentUserOnly);
		if (_impersonationLevel != TokenImpersonationLevel.None)
		{
			num |= 0x100000;
			num |= (int)(_impersonationLevel - 1) << 16;
		}
		SafePipeHandle safePipeHandle = CreateNamedPipeClient(_normalizedPipePath, ref secAttrs, num, _accessRights);
		if (safePipeHandle.IsInvalid)
		{
			int lastPInvokeError = Marshal.GetLastPInvokeError();
			safePipeHandle.Dispose();
			switch (lastPInvokeError)
			{
			case 2:
				return false;
			default:
				throw System.IO.Win32Marshal.GetExceptionForWin32Error(lastPInvokeError);
			case 231:
				break;
			}
			if (!global::Interop.Kernel32.WaitNamedPipe(_normalizedPipePath, timeout))
			{
				lastPInvokeError = Marshal.GetLastPInvokeError();
				if (lastPInvokeError == 2 || lastPInvokeError == 121)
				{
					return false;
				}
				throw System.IO.Win32Marshal.GetExceptionForWin32Error(lastPInvokeError);
			}
			safePipeHandle = CreateNamedPipeClient(_normalizedPipePath, ref secAttrs, num, _accessRights);
			if (safePipeHandle.IsInvalid)
			{
				lastPInvokeError = Marshal.GetLastPInvokeError();
				safePipeHandle.Dispose();
				if (lastPInvokeError == 231 || lastPInvokeError == 2)
				{
					return false;
				}
				throw System.IO.Win32Marshal.GetExceptionForWin32Error(lastPInvokeError);
			}
		}
		InitializeHandle(safePipeHandle, isExposed: false, (_pipeOptions & PipeOptions.Asynchronous) != 0);
		base.State = PipeState.Connected;
		ValidateRemotePipeUser();
		return true;
		static SafePipeHandle CreateNamedPipeClient(string path, ref global::Interop.Kernel32.SECURITY_ATTRIBUTES secAttrs2, int pipeFlags, int access)
		{
			return global::Interop.Kernel32.CreateNamedPipeClient(path, access, FileShare.None, ref secAttrs2, FileMode.Open, pipeFlags, IntPtr.Zero);
		}
	}

	private void ValidateRemotePipeUser()
	{
		if (!base.IsCurrentUserOnly)
		{
			return;
		}
		IdentityReference owner = this.GetAccessControl().GetOwner(typeof(SecurityIdentifier));
		using WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
		SecurityIdentifier owner2 = windowsIdentity.Owner;
		if (owner != owner2)
		{
			base.State = PipeState.Closed;
			throw new UnauthorizedAccessException(System.SR.UnauthorizedAccess_NotOwnedByCurrentUser);
		}
	}
}
