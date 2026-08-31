using System;
using System.Threading;

namespace F;

internal class _0002
{
	private int HCB;

	internal _0002()
	{
		HCB = Thread.CurrentThread.ManagedThreadId;
	}

	internal void _7M()
	{
		if (HCB != Thread.CurrentThread.ManagedThreadId)
		{
			throw new Exception("Calls to object cannot be made from threads other than the one that created it.");
		}
	}
}
