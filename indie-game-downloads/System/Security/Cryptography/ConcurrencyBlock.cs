using System.Threading;

namespace System.Security.Cryptography;

internal struct ConcurrencyBlock
{
	internal ref struct Scope
	{
		private ref int _parentCount;

		internal Scope(ref int parentCount)
		{
			_parentCount = ref parentCount;
		}

		internal void Dispose()
		{
			Interlocked.Decrement(ref _parentCount);
		}
	}

	private int _count;

	internal static Scope Enter(ref ConcurrencyBlock block)
	{
		if (Interlocked.Increment(ref block._count) != 1)
		{
			Interlocked.Decrement(ref block._count);
			throw new CryptographicException(System.SR.Cryptography_ConcurrentUseNotSupported);
		}
		return new Scope(ref block._count);
	}
}
