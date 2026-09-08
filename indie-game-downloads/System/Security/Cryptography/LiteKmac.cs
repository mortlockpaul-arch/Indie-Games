using Internal.Cryptography;
using Internal.NativeCrypto;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography;

internal readonly struct LiteKmac : ILiteHash, IDisposable
{
	private readonly SafeBCryptHashHandle _hashHandle;

	private readonly int _finishFlags;

	public int HashSizeInBytes
	{
		get
		{
			throw new CryptographicException();
		}
	}

	internal LiteKmac(string algorithm, ReadOnlySpan<byte> key, ReadOnlySpan<byte> customizationString, bool xof)
	{
		_finishFlags = (xof ? 1 : 0);
		nuint hAlgorithm;
		if (!(algorithm == "KMAC128"))
		{
			if (!(algorithm == "KMAC256"))
			{
				throw FailThrow(algorithm);
			}
			hAlgorithm = 1089u;
		}
		else
		{
			hAlgorithm = 1073u;
		}
		global::Interop.BCrypt.NTSTATUS nTSTATUS = global::Interop.BCrypt.BCryptCreateHash(hAlgorithm, out var phHash, IntPtr.Zero, 0, key, key.Length, global::Interop.BCrypt.BCryptCreateHashFlags.None);
		if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
		{
			phHash.Dispose();
			throw global::Interop.BCrypt.CreateCryptographicException(nTSTATUS);
		}
		if (!customizationString.IsEmpty)
		{
			nTSTATUS = Cng.Interop.BCryptSetProperty(phHash, "CustomizationString", customizationString, customizationString.Length, 0);
			if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
			{
				phHash.Dispose();
				throw global::Interop.BCrypt.CreateCryptographicException(nTSTATUS);
			}
		}
		_hashHandle = phHash;
		static Exception FailThrow(string text)
		{
			return new CryptographicException();
		}
	}

	private LiteKmac(SafeBCryptHashHandle hashHandle, int finishFlags)
	{
		_hashHandle = hashHandle;
		_finishFlags = finishFlags;
	}

	public void Reset()
	{
		if ((_finishFlags & 1) == 1)
		{
			Span<byte> pbOutput = stackalloc byte[1];
			CheckStatus(global::Interop.BCrypt.BCryptFinishHash(_hashHandle, pbOutput, 0, 0));
		}
	}

	public void Append(ReadOnlySpan<byte> data)
	{
		if (!data.IsEmpty)
		{
			CheckStatus(global::Interop.BCrypt.BCryptHashData(_hashHandle, data, data.Length, 0));
		}
	}

	public unsafe int Current(Span<byte> destination)
	{
		fixed (byte* nonNullPinnableReference = &Helpers.GetNonNullPinnableReference(destination))
		{
			using SafeBCryptHashHandle hHash = global::Interop.BCrypt.BCryptDuplicateHash(_hashHandle);
			CheckStatus(global::Interop.BCrypt.BCryptFinishHash(hHash, nonNullPinnableReference, destination.Length, _finishFlags));
		}
		return destination.Length;
	}

	public unsafe int Finalize(Span<byte> destination)
	{
		fixed (byte* nonNullPinnableReference = &Helpers.GetNonNullPinnableReference(destination))
		{
			CheckStatus(global::Interop.BCrypt.BCryptFinishHash(_hashHandle, nonNullPinnableReference, destination.Length, _finishFlags));
		}
		return destination.Length;
	}

	public LiteKmac Clone()
	{
		return new LiteKmac(global::Interop.BCrypt.BCryptDuplicateHash(_hashHandle), _finishFlags);
	}

	public void Dispose()
	{
		_hashHandle.Dispose();
	}

	private static void CheckStatus(global::Interop.BCrypt.NTSTATUS status)
	{
		if (status != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
		{
			throw global::Interop.BCrypt.CreateCryptographicException(status);
		}
	}
}
