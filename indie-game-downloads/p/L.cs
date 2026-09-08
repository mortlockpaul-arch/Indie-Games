using System;
using System.Threading;

namespace p;

internal class L
{
	private int _3A_0018;

	internal L()
	{
		_3A_0018 = Thread.CurrentThread.ManagedThreadId;
	}

	internal void _6i()
	{
		if (_3A_0018 != Thread.CurrentThread.ManagedThreadId)
		{
			throw new Exception("Calls to object cannot be made from threads other than the one that created it.");
		}
	}
}
