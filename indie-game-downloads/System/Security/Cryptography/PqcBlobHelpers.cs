using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Cryptography;

internal static class PqcBlobHelpers
{
	internal delegate TResult EncodeBlobFunc<TResult>(ReadOnlySpan<byte> blob);

	internal delegate TReturn EncodeMLKemBlobCallback<TState, TReturn>(TState state, string blobKind, ReadOnlySpan<byte> blob);

	internal static string GetMLDsaParameterSet(MLDsaAlgorithm algorithm)
	{
		if (algorithm == MLDsaAlgorithm.MLDsa44)
		{
			return "44";
		}
		if (algorithm == MLDsaAlgorithm.MLDsa65)
		{
			return "65";
		}
		if (algorithm == MLDsaAlgorithm.MLDsa87)
		{
			return "87";
		}
		throw new PlatformNotSupportedException();
	}

	internal static TResult EncodeMLDsaBlob<TResult>(ReadOnlySpan<char> parameterSet, ReadOnlySpan<byte> data, string blobType, EncodeBlobFunc<TResult> callback)
	{
		return EncodePQDsaBlob(blobType switch
		{
			"PQDSAPUBLICBLOB" => global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLDSA_PUBLIC_MAGIC, 
			"PQDSAPRIVATEBLOB" => global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLDSA_PRIVATE_MAGIC, 
			"PQDSAPRIVATESEEDBLOB" => global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLDSA_PRIVATE_SEED_MAGIC, 
			_ => throw new CryptographicException(), 
		}, parameterSet, data, callback);
	}

	internal static ReadOnlySpan<byte> DecodeMLDsaBlob(ReadOnlySpan<byte> blob, out ReadOnlySpan<char> parameterSet, out string blobType)
	{
		ReadOnlySpan<byte> result = DecodePQDsaBlob(blob, out var magic, out parameterSet);
		switch (magic)
		{
		case global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLDSA_PUBLIC_MAGIC:
			blobType = "PQDSAPUBLICBLOB";
			break;
		case global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLDSA_PRIVATE_MAGIC:
			blobType = "PQDSAPRIVATEBLOB";
			break;
		case global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLDSA_PRIVATE_SEED_MAGIC:
			blobType = "PQDSAPRIVATESEEDBLOB";
			break;
		default:
			throw new CryptographicException();
		}
		return result;
	}

	private static TResult EncodePQDsaBlob<TResult>(global::Interop.BCrypt.KeyBlobMagicNumber magic, ReadOnlySpan<char> parameterSet, ReadOnlySpan<byte> data, EncodeBlobFunc<TResult> callback)
	{
		int num = Unsafe.SizeOf<global::Interop.BCrypt.BCRYPT_PQDSA_KEY_BLOB>();
		int num2;
		int num3;
		byte[] array;
		Span<byte> span2;
		checked
		{
			num2 = 2 * (parameterSet.Length + 1);
			num3 = num + num2 + data.Length;
			array = null;
			Span<byte> span = ((num3 > 64) ? ((Span<byte>)(array = System.Security.Cryptography.CryptoPool.Rent(num3))) : stackalloc byte[64]);
			span2 = span;
			span2 = span2.Slice(0, num3);
		}
		try
		{
			int num4 = 0;
			ref global::Interop.BCrypt.BCRYPT_PQDSA_KEY_BLOB reference = ref MemoryMarshal.Cast<byte, global::Interop.BCrypt.BCRYPT_PQDSA_KEY_BLOB>(span2)[0];
			reference.Magic = magic;
			reference.cbParameterSet = num2;
			reference.cbKey = data.Length;
			num4 += num;
			Span<char> destination = MemoryMarshal.Cast<byte, char>(span2.Slice(num4));
			parameterSet.CopyTo(destination);
			destination[parameterSet.Length] = '\0';
			num4 += num2;
			data.CopyTo(span2.Slice(num4));
			num4 += data.Length;
			return callback(span2);
		}
		finally
		{
			if (array != null)
			{
				System.Security.Cryptography.CryptoPool.Return(array, num3);
			}
		}
	}

	private static ReadOnlySpan<byte> DecodePQDsaBlob(ReadOnlySpan<byte> blobBytes, out global::Interop.BCrypt.KeyBlobMagicNumber magic, out ReadOnlySpan<char> parameterSet)
	{
		int num = 0;
		ref readonly global::Interop.BCrypt.BCRYPT_PQDSA_KEY_BLOB reference = ref MemoryMarshal.Cast<byte, global::Interop.BCrypt.BCRYPT_PQDSA_KEY_BLOB>(blobBytes)[0];
		magic = reference.Magic;
		int length = reference.cbParameterSet - 2;
		int cbKey = reference.cbKey;
		num += Unsafe.SizeOf<global::Interop.BCrypt.BCRYPT_PQDSA_KEY_BLOB>();
		parameterSet = MemoryMarshal.Cast<byte, char>(blobBytes.Slice(num, length));
		num += reference.cbParameterSet;
		return blobBytes.Slice(num, cbKey);
	}

	internal static string GetMLKemParameterSet(MLKemAlgorithm algorithm)
	{
		if (algorithm == MLKemAlgorithm.MLKem512)
		{
			return "512";
		}
		if (algorithm == MLKemAlgorithm.MLKem768)
		{
			return "768";
		}
		if (algorithm == MLKemAlgorithm.MLKem1024)
		{
			return "1024";
		}
		throw new PlatformNotSupportedException();
	}

	internal static string MLKemBlobMagicToBlobType(global::Interop.BCrypt.KeyBlobMagicNumber magicNumber)
	{
		return magicNumber switch
		{
			global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PRIVATE_SEED_MAGIC => "MLKEMPRIVATESEEDBLOB", 
			global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PRIVATE_MAGIC => "MLKEMPRIVATEBLOB", 
			global::Interop.BCrypt.KeyBlobMagicNumber.BCRYPT_MLKEM_PUBLIC_MAGIC => "MLKEMPUBLICBLOB", 
			_ => throw Fail(magicNumber), 
		};
		static CryptographicException Fail(global::Interop.BCrypt.KeyBlobMagicNumber other)
		{
			return new CryptographicException();
		}
	}

	internal unsafe static TReturn EncodeMLKemBlob<TState, TReturn>(global::Interop.BCrypt.KeyBlobMagicNumber kind, MLKemAlgorithm algorithm, ReadOnlySpan<byte> key, TState state, EncodeMLKemBlobCallback<TState, TReturn> callback)
	{
		string mLKemParameterSet = GetMLKemParameterSet(algorithm);
		int num = Marshal.SizeOf<global::Interop.BCrypt.BCRYPT_MLKEM_KEY_BLOB>();
		checked
		{
			int num2 = (mLKemParameterSet.Length + 1) * 2;
			int num3 = num + num2 + key.Length;
			byte[] array = null;
			Span<byte> span = (((uint)num3 > 128) ? ((Span<byte>)(array = System.Security.Cryptography.CryptoPool.Rent(num3))) : stackalloc byte[128]);
			Span<byte> span2 = span;
			try
			{
				span2.Clear();
				fixed (byte* ptr = span2)
				{
					global::Interop.BCrypt.BCRYPT_MLKEM_KEY_BLOB* ptr2 = unchecked((global::Interop.BCrypt.BCRYPT_MLKEM_KEY_BLOB*)ptr);
					ptr2->dwMagic = kind;
					ptr2->cbParameterSet = (uint)num2;
					ptr2->cbKey = (uint)key.Length;
				}
				Encoding.Unicode.GetBytes(mLKemParameterSet.AsSpan(), span2.Slice(num));
				key.CopyTo(span2.Slice(num + num2));
				string blobKind = MLKemBlobMagicToBlobType(kind);
				return callback(state, blobKind, span2.Slice(0, num3));
			}
			finally
			{
				CryptographicOperations.ZeroMemory(span2.Slice(0, num3));
				if (array != null)
				{
					System.Security.Cryptography.CryptoPool.Return(array, 0);
				}
			}
		}
	}
}
