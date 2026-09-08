namespace System.Security.Cryptography;

internal struct ConcurrentSafeKmac
{
	private readonly LiteKmac _liteKmac;

	private ConcurrencyBlock _block;

	internal ConcurrentSafeKmac(string algorithmId, ReadOnlySpan<byte> key, ReadOnlySpan<byte> customizationString, bool xof)
	{
		_block = default(ConcurrencyBlock);
		_liteKmac = LiteHashProvider.CreateKmac(algorithmId, key, customizationString, xof);
	}

	private ConcurrentSafeKmac(LiteKmac liteKmac)
	{
		_block = default(ConcurrencyBlock);
		_liteKmac = liteKmac;
	}

	public void Append(ReadOnlySpan<byte> data)
	{
		using (ConcurrencyBlock.Enter(ref _block))
		{
			_liteKmac.Append(data);
		}
	}

	public int Current(Span<byte> destination)
	{
		using (ConcurrencyBlock.Enter(ref _block))
		{
			return _liteKmac.Current(destination);
		}
	}

	public int Finalize(Span<byte> destination)
	{
		using (ConcurrencyBlock.Enter(ref _block))
		{
			return _liteKmac.Finalize(destination);
		}
	}

	public void Reset()
	{
		using (ConcurrencyBlock.Enter(ref _block))
		{
			_liteKmac.Reset();
		}
	}

	public ConcurrentSafeKmac Clone()
	{
		using (ConcurrencyBlock.Enter(ref _block))
		{
			return new ConcurrentSafeKmac(_liteKmac.Clone());
		}
	}

	public void Dispose()
	{
		_liteKmac.Dispose();
	}
}
