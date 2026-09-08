using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace System.Security.Cryptography;

public abstract class Aes : SymmetricAlgorithm
{
	private static readonly KeySizes[] s_legalBlockSizes = new KeySizes[1]
	{
		new KeySizes(128, 128, 0)
	};

	private static readonly KeySizes[] s_legalKeySizes = new KeySizes[1]
	{
		new KeySizes(128, 256, 64)
	};

	protected Aes()
	{
		LegalBlockSizesValue = s_legalBlockSizes.CloneKeySizesArray();
		LegalKeySizesValue = s_legalKeySizes.CloneKeySizesArray();
		BlockSizeValue = 128;
		FeedbackSizeValue = 8;
		KeySizeValue = 256;
		ModeValue = CipherMode.CBC;
	}

	[UnsupportedOSPlatform("browser")]
	public new static Aes Create()
	{
		return new AesImplementation();
	}

	[Obsolete("Cryptographic factory methods accepting an algorithm name are obsolete. Use the parameterless Create factory method on the algorithm type instead.", DiagnosticId = "SYSLIB0045", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[RequiresUnreferencedCode("The default algorithm implementations might be removed, use strong type references like 'RSA.Create()' instead.")]
	public new static Aes? Create(string algorithmName)
	{
		return (Aes)CryptoConfig.CreateFromName(algorithmName);
	}

	public static int GetKeyWrapPaddedLength(int plaintextLengthInBytes)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(plaintextLengthInBytes, "plaintextLengthInBytes");
		if (plaintextLengthInBytes > 2147483632)
		{
			throw new ArgumentOutOfRangeException("plaintextLengthInBytes", System.SR.Cryptography_PlaintextTooLarge);
		}
		checked
		{
			return (unchecked(checked(plaintextLengthInBytes + 7) / 8) + 1) * 8;
		}
	}

	public byte[] EncryptKeyWrapPadded(byte[] plaintext)
	{
		if (plaintext == null || plaintext.Length == 0)
		{
			throw new ArgumentException(System.SR.Arg_EmptyOrNullArray, "plaintext");
		}
		return EncryptKeyWrapPadded(new ReadOnlySpan<byte>(plaintext));
	}

	public byte[] EncryptKeyWrapPadded(ReadOnlySpan<byte> plaintext)
	{
		if (plaintext.IsEmpty)
		{
			throw new ArgumentException(System.SR.Arg_EmptySpan, "plaintext");
		}
		byte[] array = new byte[GetKeyWrapPaddedLength(plaintext.Length)];
		EncryptKeyWrapPaddedCore(plaintext, array);
		return array;
	}

	public void EncryptKeyWrapPadded(ReadOnlySpan<byte> plaintext, Span<byte> destination)
	{
		if (plaintext.IsEmpty)
		{
			throw new ArgumentException(System.SR.Arg_EmptySpan, "plaintext");
		}
		int keyWrapPaddedLength = GetKeyWrapPaddedLength(plaintext.Length);
		if (destination.Length != keyWrapPaddedLength)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_DestinationImprecise, keyWrapPaddedLength), "destination");
		}
		if (plaintext.Overlaps(destination))
		{
			throw new CryptographicException(System.SR.Cryptography_OverlappingBuffers);
		}
		EncryptKeyWrapPaddedCore(plaintext, destination);
	}

	public byte[] DecryptKeyWrapPadded(byte[] ciphertext)
	{
		ArgumentNullException.ThrowIfNull(ciphertext, "ciphertext");
		return DecryptKeyWrapPadded(new ReadOnlySpan<byte>(ciphertext));
	}

	public byte[] DecryptKeyWrapPadded(ReadOnlySpan<byte> ciphertext)
	{
		if (ciphertext.Length < 16 || ciphertext.Length % 8 != 0)
		{
			throw new ArgumentException(System.SR.Cryptography_KeyWrap_InvalidLength, "ciphertext");
		}
		using CryptoPoolLease cryptoPoolLease = CryptoPoolLease.Rent(ciphertext.Length - 8, skipClear: true);
		int length = DecryptKeyWrapPadded(ciphertext, cryptoPoolLease.Span);
		return cryptoPoolLease.Span.Slice(0, length).ToArray();
	}

	public int DecryptKeyWrapPadded(ReadOnlySpan<byte> ciphertext, Span<byte> destination)
	{
		if (ciphertext.Length < 16 || ciphertext.Length % 8 != 0)
		{
			throw new ArgumentException(System.SR.Cryptography_KeyWrap_InvalidLength, "ciphertext");
		}
		if (TryDecryptKeyWrapPadded(ciphertext, destination, out var bytesWritten))
		{
			return bytesWritten;
		}
		throw new ArgumentException(System.SR.Argument_DestinationTooShort, "destination");
	}

	public bool TryDecryptKeyWrapPadded(ReadOnlySpan<byte> ciphertext, Span<byte> destination, out int bytesWritten)
	{
		if (ciphertext.Length < 16 || ciphertext.Length % 8 != 0)
		{
			throw new ArgumentException(System.SR.Cryptography_KeyWrap_InvalidLength, "ciphertext");
		}
		int num = ciphertext.Length - 8;
		int num2 = num - 7;
		if (destination.Length < num2)
		{
			bytesWritten = 0;
			return false;
		}
		if (destination.Length > num)
		{
			destination = destination.Slice(0, num);
		}
		if (ciphertext.Overlaps(destination))
		{
			throw new CryptographicException(System.SR.Cryptography_OverlappingBuffers);
		}
		bool rented;
		using CryptoPoolLease cryptoPoolLease = CryptoPoolLease.RentConditionally(num, destination, out rented, skipClear: false, skipClearIfNotRented: true);
		try
		{
			int num3 = DecryptKeyWrapPaddedCore(ciphertext, cryptoPoolLease.Span);
			if (num3 < num2 || num3 > num)
			{
				throw new CryptographicException();
			}
			if (num3 > destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			if (rented)
			{
				cryptoPoolLease.Span.Slice(0, num3).CopyTo(destination);
			}
			destination.Slice(num3).Clear();
			bytesWritten = num3;
			return true;
		}
		catch
		{
			CryptographicOperations.ZeroMemory(destination);
			throw;
		}
	}

	protected virtual int DecryptKeyWrapPaddedCore(ReadOnlySpan<byte> source, Span<byte> destination)
	{
		ulong num;
		if (source.Length == 16)
		{
			Span<byte> span = stackalloc byte[16];
			DecryptEcb(source, span, PaddingMode.None);
			num = BinaryPrimitives.ReadUInt64BigEndian(span);
			span.Slice(8).CopyTo(destination);
		}
		else
		{
			num = Rfc3394Unwrap(source, destination);
		}
		uint num2 = (uint)num;
		int num3 = (int)(num >> 32);
		int num4 = (int)num2;
		uint num5 = (uint)(source.Length - 8) - num2;
		if (num3 != -1504093786 || num5 > 7 || ((ReadOnlySpan<byte>)destination.Slice(num4)).IndexOfAnyExcept((byte)0) >= 0)
		{
			throw new CryptographicException(System.SR.Cryptography_KeyWrap_DecryptFailed);
		}
		return num4;
	}

	protected virtual void EncryptKeyWrapPaddedCore(ReadOnlySpan<byte> source, Span<byte> destination)
	{
		ulong num = (ulong)(-6460033620986822656L | (uint)source.Length);
		if (source.Length <= 8)
		{
			Span<byte> span = stackalloc byte[16];
			BinaryPrimitives.WriteUInt64BigEndian(span, num);
			Span<byte> span2 = span.Slice(8);
			span2.Clear();
			source.CopyTo(span2);
			EncryptEcb(span, destination, PaddingMode.None);
			CryptographicOperations.ZeroMemory(span2);
			return;
		}
		if (source.Length % 8 == 0)
		{
			Rfc3394Wrap(num, source, destination);
			return;
		}
		using CryptoPoolLease cryptoPoolLease = CryptoPoolLease.Rent(checked(source.Length + 7) / 8 * 8);
		source.CopyTo(cryptoPoolLease.Span);
		cryptoPoolLease.Span.Slice(source.Length).Clear();
		Rfc3394Wrap(num, cryptoPoolLease.Span, destination);
	}

	private void Rfc3394Wrap(ulong iv, ReadOnlySpan<byte> source, Span<byte> destination)
	{
		Span<byte> span = stackalloc byte[16];
		Span<byte> destination2 = span.Slice(0, 8);
		Span<byte> span2 = destination2.Slice(4, 4);
		uint num = 1u;
		source.CopyTo(destination.Slice(8));
		BinaryPrimitives.WriteUInt64BigEndian(destination2, iv);
		for (uint num2 = 0u; num2 < 6; num2++)
		{
			Span<byte> destination3 = destination.Slice(8);
			uint num3 = 0u;
			while (num3 < source.Length)
			{
				destination3.Slice(0, 8).CopyTo(span.Slice(8));
				EncryptEcb(span, span, PaddingMode.None);
				uint num4 = BinaryPrimitives.ReadUInt32BigEndian(span2);
				num4 ^= num;
				BinaryPrimitives.WriteUInt32BigEndian(span2, num4);
				span.Slice(8, 8).CopyTo(destination3);
				num3 += 8;
				num++;
				destination3 = destination3.Slice(8);
			}
		}
		destination2.CopyTo(destination);
	}

	private ulong Rfc3394Unwrap(ReadOnlySpan<byte> source, Span<byte> destination)
	{
		Span<byte> span = stackalloc byte[16];
		Span<byte> span2 = span.Slice(0, 8);
		Span<byte> span3 = span2.Slice(4, 4);
		_ = source.Length;
		uint num = (uint)(6 * (source.Length / 8) - 6);
		source.Slice(0, 8).CopyTo(span2);
		source.Slice(8).CopyTo(destination);
		for (uint num2 = 0u; num2 < 6; num2++)
		{
			for (int num3 = source.Length - 16; num3 >= 0; num3 -= 8)
			{
				Span<byte> destination2 = destination.Slice(num3);
				uint num4 = BinaryPrimitives.ReadUInt32BigEndian(span3);
				num4 ^= num;
				BinaryPrimitives.WriteUInt32BigEndian(span3, num4);
				destination2.Slice(0, 8).CopyTo(span.Slice(8));
				DecryptEcb(span, span, PaddingMode.None);
				span.Slice(8).CopyTo(destination2);
				num--;
			}
		}
		return BinaryPrimitives.ReadUInt64BigEndian(span2);
	}
}
