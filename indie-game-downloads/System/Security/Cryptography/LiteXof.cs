using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography;

internal readonly struct LiteXof : ILiteHash, IDisposable
{
	private readonly nuint _algorithm;

	private readonly SafeBCryptHashHandle _hashHandle;

	public int HashSizeInBytes
	{
		get
		{
			throw new CryptographicException();
		}
	}

	internal LiteXof(string algorithm)
	{
		nuint algorithm2;
		if (!(algorithm == "CSHAKE128"))
		{
			if (!(algorithm == "CSHAKE256"))
			{
				throw FailThrow(algorithm);
			}
			algorithm2 = 1057u;
		}
		else
		{
			algorithm2 = 1041u;
		}
		_algorithm = algorithm2;
		global::Interop.BCrypt.NTSTATUS nTSTATUS = global::Interop.BCrypt.BCryptCreateHash(_algorithm, out var phHash, IntPtr.Zero, 0, ReadOnlySpan<byte>.Empty, 0, global::Interop.BCrypt.BCryptCreateHashFlags.None);
		if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
		{
			phHash.Dispose();
			throw global::Interop.BCrypt.CreateCryptographicException(nTSTATUS);
		}
		_hashHandle = phHash;
		static Exception FailThrow(string text)
		{
			return new CryptographicException();
		}
	}

	private LiteXof(SafeBCryptHashHandle hashHandle, nuint algorithm)
	{
		_algorithm = algorithm;
		_hashHandle = hashHandle;
	}

	public void Append(ReadOnlySpan<byte> data)
	{
		if (!data.IsEmpty)
		{
			global::Interop.BCrypt.NTSTATUS nTSTATUS = global::Interop.BCrypt.BCryptHashData(_hashHandle, data, data.Length, 0);
			if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
			{
				throw global::Interop.BCrypt.CreateCryptographicException(nTSTATUS);
			}
		}
	}

	public unsafe int Finalize(Span<byte> destination)
	{
		fixed (byte* nonNullPinnableReference = &Helpers.GetNonNullPinnableReference(destination))
		{
			global::Interop.BCrypt.NTSTATUS nTSTATUS = global::Interop.BCrypt.BCryptFinishHash(_hashHandle, nonNullPinnableReference, destination.Length, 0);
			if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
			{
				throw global::Interop.BCrypt.CreateCryptographicException(nTSTATUS);
			}
			return destination.Length;
		}
	}

	public int FinalizeAndReset(Span<byte> destination)
	{
		return Finalize(destination);
	}

	public void Reset()
	{
		Finalize(Span<byte>.Empty);
	}

	public unsafe void Current(Span<byte> destination)
	{
		using SafeBCryptHashHandle hHash = global::Interop.BCrypt.BCryptDuplicateHash(_hashHandle);
		fixed (byte* nonNullPinnableReference = &Helpers.GetNonNullPinnableReference(destination))
		{
			global::Interop.BCrypt.NTSTATUS nTSTATUS = global::Interop.BCrypt.BCryptFinishHash(hHash, nonNullPinnableReference, destination.Length, 0);
			if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
			{
				throw global::Interop.BCrypt.CreateCryptographicException(nTSTATUS);
			}
		}
	}

	public LiteXof Clone()
	{
		return new LiteXof(global::Interop.BCrypt.BCryptDuplicateHash(_hashHandle), _algorithm);
	}

	public unsafe void Read(Span<byte> destination)
	{
		fixed (byte* nonNullPinnableReference = &Helpers.GetNonNullPinnableReference(destination))
		{
			global::Interop.BCrypt.NTSTATUS nTSTATUS = global::Interop.BCrypt.BCryptFinishHash(_hashHandle, nonNullPinnableReference, destination.Length, 1);
			if (nTSTATUS != global::Interop.BCrypt.NTSTATUS.STATUS_SUCCESS)
			{
				throw global::Interop.BCrypt.CreateCryptographicException(nTSTATUS);
			}
		}
	}

	public void Dispose()
	{
		_hashHandle.Dispose();
	}
}
