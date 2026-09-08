namespace System.Security.Cryptography;

internal ref struct CryptoPoolLease : IDisposable
{
	private byte[] _rented;

	private bool _skipClear;

	internal Span<byte> Span { get; private set; }

	internal readonly bool IsRented => _rented != null;

	public void Dispose()
	{
		Return();
	}

	internal void Return()
	{
		Return((!_skipClear) ? Span.Length : 0);
	}

	private void Return(int clearSize)
	{
		if (_rented != null)
		{
			System.Security.Cryptography.CryptoPool.Return(_rented, clearSize);
			_rented = null;
		}
		else if (!_skipClear && clearSize > 0)
		{
			CryptographicOperations.ZeroMemory(Span.Slice(0, clearSize));
		}
		Span = default(Span<byte>);
	}

	internal static CryptoPoolLease Rent(int length, bool skipClear = false)
	{
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(length);
		return new CryptoPoolLease
		{
			_rented = array,
			_skipClear = skipClear,
			Span = new Span<byte>(array, 0, length)
		};
	}

	internal static CryptoPoolLease RentConditionally(int length, Span<byte> currentBuffer, bool skipClear = false, bool skipClearIfNotRented = false)
	{
		bool rented;
		return RentConditionally(length, currentBuffer, out rented, skipClear, skipClearIfNotRented);
	}

	internal static CryptoPoolLease RentConditionally(int length, Span<byte> currentBuffer, out bool rented, bool skipClear = false, bool skipClearIfNotRented = false)
	{
		if (currentBuffer.Length >= length)
		{
			rented = false;
			return new CryptoPoolLease
			{
				_rented = null,
				_skipClear = (skipClearIfNotRented | skipClear),
				Span = currentBuffer.Slice(0, length)
			};
		}
		rented = true;
		return Rent(length, skipClear);
	}
}
