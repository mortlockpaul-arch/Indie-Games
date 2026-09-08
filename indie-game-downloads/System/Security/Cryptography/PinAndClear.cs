using System.Runtime.InteropServices;

namespace System.Security.Cryptography;

internal struct PinAndClear : IDisposable
{
	private byte[] _data;

	private PinnedGCHandle<byte[]> _gcHandle;

	internal static PinAndClear Track(byte[] data)
	{
		return new PinAndClear
		{
			_gcHandle = new PinnedGCHandle<byte[]>(data),
			_data = data
		};
	}

	public void Dispose()
	{
		Array.Clear(_data);
		_gcHandle.Dispose();
	}
}
