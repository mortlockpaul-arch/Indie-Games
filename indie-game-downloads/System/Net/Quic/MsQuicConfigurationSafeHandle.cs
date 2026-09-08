using System.Threading;
using Microsoft.Quic;

namespace System.Net.Quic;

internal sealed class MsQuicConfigurationSafeHandle : MsQuicSafeHandle, ISafeHandleCachable
{
	private volatile int _rentCount;

	public unsafe MsQuicConfigurationSafeHandle(QUIC_HANDLE* handle)
		: base(handle, SafeHandleType.Configuration)
	{
	}

	public bool TryAddRentCount()
	{
		int rentCount;
		do
		{
			rentCount = _rentCount;
			if (rentCount < 0)
			{
				return false;
			}
		}
		while (Interlocked.CompareExchange(ref _rentCount, rentCount + 1, rentCount) != rentCount);
		return true;
	}

	public bool TryMarkForDispose()
	{
		return Interlocked.CompareExchange(ref _rentCount, -1, 0) == 0;
	}

	protected override void Dispose(bool disposing)
	{
		if (Interlocked.Decrement(ref _rentCount) < 0)
		{
			base.Dispose(disposing);
		}
	}
}
