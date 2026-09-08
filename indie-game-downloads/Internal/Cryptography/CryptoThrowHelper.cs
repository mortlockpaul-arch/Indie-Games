using System.Security.Cryptography;

namespace Internal.Cryptography;

internal static class CryptoThrowHelper
{
	public static CryptographicException ToCryptographicException(this int hr)
	{
		string message = global::Interop.Kernel32.GetMessage(hr);
		if (hr >= 0)
		{
			hr = (hr & 0xFFFF) | -2147024896;
		}
		return new CryptographicException(message)
		{
			HResult = hr
		};
	}
}
