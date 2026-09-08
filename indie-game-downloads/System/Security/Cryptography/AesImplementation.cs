using Internal.Cryptography;

namespace System.Security.Cryptography;

internal sealed class AesImplementation : Aes
{
	private FixedMemoryKeyBox _keyBox;

	public override byte[] Key
	{
		get
		{
			return GetKey().UseKey<string, byte[]>("", (Func<string, ReadOnlySpan<byte>, byte[]>)((string _, ReadOnlySpan<byte> key) => key.ToArray()));
		}
		set
		{
			SetKey(value);
		}
	}

	public override int KeySize
	{
		get
		{
			return base.KeySize;
		}
		set
		{
			base.KeySize = value;
			_keyBox?.Dispose();
			_keyBox = null;
		}
	}

	private FixedMemoryKeyBox GetKey()
	{
		if (_keyBox == null)
		{
			GenerateKey();
		}
		return _keyBox;
	}

	public sealed override ICryptoTransform CreateDecryptor()
	{
		return GetKey().UseKey<AesImplementation, UniversalCryptoTransform>(this, (Func<AesImplementation, ReadOnlySpan<byte>, UniversalCryptoTransform>)((AesImplementation instance, ReadOnlySpan<byte> key) => instance.CreateTransform(key, instance.IV, encrypting: false)));
	}

	public sealed override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
	{
		return CreateTransform(rgbKey, rgbIV.CloneByteArray(), encrypting: false);
	}

	public sealed override ICryptoTransform CreateEncryptor()
	{
		return GetKey().UseKey<AesImplementation, UniversalCryptoTransform>(this, (Func<AesImplementation, ReadOnlySpan<byte>, UniversalCryptoTransform>)((AesImplementation instance, ReadOnlySpan<byte> key) => instance.CreateTransform(key, instance.IV, encrypting: true)));
	}

	public sealed override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
	{
		return CreateTransform(rgbKey, rgbIV.CloneByteArray(), encrypting: true);
	}

	public sealed override void GenerateIV()
	{
		IV = RandomNumberGenerator.GetBytes(BlockSize / 8);
	}

	public sealed override void GenerateKey()
	{
		Span<byte> span = stackalloc byte[KeySize / 8];
		RandomNumberGenerator.Fill(span);
		SetKeyCore(span);
	}

	protected sealed override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_keyBox?.Dispose();
			_keyBox = null;
		}
		base.Dispose(disposing);
	}

	protected override void SetKeyCore(ReadOnlySpan<byte> key)
	{
		KeySizeValue = checked(8 * key.Length);
		_keyBox?.Dispose();
		_keyBox = new FixedMemoryKeyBox(key);
	}

	protected override bool TryDecryptEcbCore(ReadOnlySpan<byte> ciphertext, Span<byte> destination, PaddingMode paddingMode, out int bytesWritten)
	{
		ILiteSymmetricCipher liteSymmetricCipher = GetKey().UseKey<int, BasicSymmetricCipherLiteBCrypt>(BlockSize / 8, (Func<int, ReadOnlySpan<byte>, BasicSymmetricCipherLiteBCrypt>)((int blockSizeBytes, ReadOnlySpan<byte> key) => CreateLiteCipher(CipherMode.ECB, key, default(ReadOnlySpan<byte>), blockSizeBytes, blockSizeBytes, 0, encrypting: false)));
		using (liteSymmetricCipher)
		{
			return UniversalCryptoOneShot.OneShotDecrypt(liteSymmetricCipher, paddingMode, ciphertext, destination, out bytesWritten);
		}
	}

	protected override bool TryEncryptEcbCore(ReadOnlySpan<byte> plaintext, Span<byte> destination, PaddingMode paddingMode, out int bytesWritten)
	{
		ILiteSymmetricCipher liteSymmetricCipher = GetKey().UseKey<int, BasicSymmetricCipherLiteBCrypt>(BlockSize / 8, (Func<int, ReadOnlySpan<byte>, BasicSymmetricCipherLiteBCrypt>)((int blockSizeBytes, ReadOnlySpan<byte> key) => CreateLiteCipher(CipherMode.ECB, key, default(ReadOnlySpan<byte>), blockSizeBytes, blockSizeBytes, 0, encrypting: true)));
		using (liteSymmetricCipher)
		{
			return UniversalCryptoOneShot.OneShotEncrypt(liteSymmetricCipher, paddingMode, plaintext, destination, out bytesWritten);
		}
	}

	protected override bool TryEncryptCbcCore(ReadOnlySpan<byte> plaintext, ReadOnlySpan<byte> iv, Span<byte> destination, PaddingMode paddingMode, out int bytesWritten)
	{
		ILiteSymmetricCipher liteSymmetricCipher = GetKey().UseKey<int, BasicSymmetricCipherLiteBCrypt>(iv, BlockSize / 8, (Func<ReadOnlySpan<byte>, int, ReadOnlySpan<byte>, BasicSymmetricCipherLiteBCrypt>)((ReadOnlySpan<byte> iv2, int blockSizeBytes, ReadOnlySpan<byte> key) => CreateLiteCipher(CipherMode.CBC, key, iv2, blockSizeBytes, blockSizeBytes, 0, encrypting: true)));
		using (liteSymmetricCipher)
		{
			return UniversalCryptoOneShot.OneShotEncrypt(liteSymmetricCipher, paddingMode, plaintext, destination, out bytesWritten);
		}
	}

	protected override bool TryDecryptCbcCore(ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> iv, Span<byte> destination, PaddingMode paddingMode, out int bytesWritten)
	{
		ILiteSymmetricCipher liteSymmetricCipher = GetKey().UseKey<int, BasicSymmetricCipherLiteBCrypt>(iv, BlockSize / 8, (Func<ReadOnlySpan<byte>, int, ReadOnlySpan<byte>, BasicSymmetricCipherLiteBCrypt>)((ReadOnlySpan<byte> iv2, int blockSizeBytes, ReadOnlySpan<byte> key) => CreateLiteCipher(CipherMode.CBC, key, iv2, blockSizeBytes, blockSizeBytes, 0, encrypting: false)));
		using (liteSymmetricCipher)
		{
			return UniversalCryptoOneShot.OneShotDecrypt(liteSymmetricCipher, paddingMode, ciphertext, destination, out bytesWritten);
		}
	}

	protected override bool TryDecryptCfbCore(ReadOnlySpan<byte> ciphertext, ReadOnlySpan<byte> iv, Span<byte> destination, PaddingMode paddingMode, int feedbackSizeInBits, out int bytesWritten)
	{
		ValidateCFBFeedbackSize(feedbackSizeInBits);
		ILiteSymmetricCipher liteSymmetricCipher = GetKey().UseKey<(int, int), BasicSymmetricCipherLiteBCrypt>(iv, (BlockSize / 8, feedbackSizeInBits / 8), (Func<ReadOnlySpan<byte>, (int, int), ReadOnlySpan<byte>, BasicSymmetricCipherLiteBCrypt>)((ReadOnlySpan<byte> iv2, (int BlockSizeBytes, int FeedbackSizeBytes) state, ReadOnlySpan<byte> key) => CreateLiteCipher(CipherMode.CFB, key, iv2, state.BlockSizeBytes, state.FeedbackSizeBytes, state.FeedbackSizeBytes, encrypting: false)));
		using (liteSymmetricCipher)
		{
			return UniversalCryptoOneShot.OneShotDecrypt(liteSymmetricCipher, paddingMode, ciphertext, destination, out bytesWritten);
		}
	}

	protected override bool TryEncryptCfbCore(ReadOnlySpan<byte> plaintext, ReadOnlySpan<byte> iv, Span<byte> destination, PaddingMode paddingMode, int feedbackSizeInBits, out int bytesWritten)
	{
		ValidateCFBFeedbackSize(feedbackSizeInBits);
		ILiteSymmetricCipher liteSymmetricCipher = GetKey().UseKey<(int, int), BasicSymmetricCipherLiteBCrypt>(iv, (BlockSize / 8, feedbackSizeInBits / 8), (Func<ReadOnlySpan<byte>, (int, int), ReadOnlySpan<byte>, BasicSymmetricCipherLiteBCrypt>)((ReadOnlySpan<byte> iv2, (int BlockSizeBytes, int FeedbackSizeBytes) state, ReadOnlySpan<byte> key) => CreateLiteCipher(CipherMode.CFB, key, iv2, state.BlockSizeBytes, state.FeedbackSizeBytes, state.FeedbackSizeBytes, encrypting: true)));
		using (liteSymmetricCipher)
		{
			return UniversalCryptoOneShot.OneShotEncrypt(liteSymmetricCipher, paddingMode, plaintext, destination, out bytesWritten);
		}
	}

	private UniversalCryptoTransform CreateTransform(byte[] rgbKey, byte[] rgbIV, bool encrypting)
	{
		ArgumentNullException.ThrowIfNull(rgbKey, "rgbKey");
		return CreateTransform(new ReadOnlySpan<byte>(rgbKey), rgbIV, encrypting);
	}

	private UniversalCryptoTransform CreateTransform(ReadOnlySpan<byte> rgbKey, byte[] rgbIV, bool encrypting)
	{
		long num = (long)rgbKey.Length * 8L;
		if (num > int.MaxValue || !((int)num).IsLegalSize(LegalKeySizes))
		{
			throw new ArgumentException(System.SR.Cryptography_InvalidKeySize, "rgbKey");
		}
		if (rgbIV != null && (long)rgbIV.Length * 8L != BlockSize)
		{
			throw new ArgumentException(System.SR.Cryptography_InvalidIVSize, "rgbIV");
		}
		if (Mode == CipherMode.CFB)
		{
			ValidateCFBFeedbackSize(FeedbackSize);
		}
		return CreateTransformCore(Mode, Padding, rgbKey, rgbIV, BlockSize / 8, this.GetPaddingSize(Mode, FeedbackSize), FeedbackSize / 8, encrypting);
	}

	private static void ValidateCFBFeedbackSize(int feedback)
	{
		if (feedback != 8 && feedback != 128)
		{
			throw new CryptographicException(System.SR.Format(System.SR.Cryptography_CipherModeFeedbackNotSupported, feedback, CipherMode.CFB));
		}
	}

	private static UniversalCryptoTransform CreateTransformCore(CipherMode cipherMode, PaddingMode paddingMode, ReadOnlySpan<byte> key, byte[] iv, int blockSize, int paddingSize, int feedbackSize, bool encrypting)
	{
		BasicSymmetricCipher cipher = new BasicSymmetricCipherBCrypt(AesBCryptModes.GetSharedHandle(cipherMode, feedbackSize), cipherMode, blockSize, paddingSize, key, ownsParentHandle: false, iv, encrypting);
		return UniversalCryptoTransform.Create(paddingMode, cipher, encrypting);
	}

	private static BasicSymmetricCipherLiteBCrypt CreateLiteCipher(CipherMode cipherMode, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv, int blockSize, int paddingSize, int feedbackSize, bool encrypting)
	{
		return new BasicSymmetricCipherLiteBCrypt(AesBCryptModes.GetSharedHandle(cipherMode, feedbackSize), blockSize, paddingSize, key, ownsParentHandle: false, iv, encrypting);
	}
}
