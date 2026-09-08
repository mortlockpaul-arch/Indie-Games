using System.Runtime.InteropServices;
using Microsoft.Quic;

namespace System.Net.Quic;

internal sealed class MsQuicContextSafeHandle : MsQuicSafeHandle
{
	private GCHandle _context;

	private readonly MsQuicSafeHandle _parent;

	private IDisposable _disposable;

	public IDisposable Disposable
	{
		set
		{
			_disposable = value;
		}
	}

	public unsafe MsQuicContextSafeHandle(QUIC_HANDLE* handle, GCHandle context, SafeHandleType safeHandleType, MsQuicSafeHandle parent = null)
		: base(handle, safeHandleType)
	{
		_context = context;
		if (parent != null)
		{
			bool success = false;
			parent.DangerousAddRef(ref success);
			_parent = parent;
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"{this} {_parent} ref count incremented", ".ctor");
			}
		}
	}

	protected override bool ReleaseHandle()
	{
		base.ReleaseHandle();
		if (_context.IsAllocated)
		{
			_context.Free();
		}
		if (_parent != null)
		{
			_parent.DangerousRelease();
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(this, $"{this} {_parent} ref count decremented", "ReleaseHandle");
			}
		}
		_disposable?.Dispose();
		return true;
	}
}
