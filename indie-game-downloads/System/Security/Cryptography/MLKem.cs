using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography;

public abstract class MLKem : IDisposable
{
	private delegate TResult ExportPkcs8PrivateKeyFunc<TResult>(ReadOnlySpan<byte> pkcs8);

	private delegate AsnWriter WriteEncryptedPkcs8Func<TChar>(ReadOnlySpan<TChar> password, AsnWriter writer, PbeParameters pbeParameters);

	private static readonly string[] s_knownOids = new string[3] { "2.16.840.1.101.3.4.4.1", "2.16.840.1.101.3.4.4.2", "2.16.840.1.101.3.4.4.3" };

	private bool _disposed;

	public static bool IsSupported => MLKemImplementation.IsSupported;

	public MLKemAlgorithm Algorithm { get; }

	protected MLKem(MLKemAlgorithm algorithm)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		Algorithm = algorithm;
	}

	public static MLKem GenerateKey(MLKemAlgorithm algorithm)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		ThrowIfNotSupported();
		return MLKemImplementation.GenerateKeyImpl(algorithm);
	}

	public void Encapsulate(Span<byte> ciphertext, Span<byte> sharedSecret)
	{
		if (ciphertext.Length != Algorithm.CiphertextSizeInBytes)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_DestinationImprecise, Algorithm.CiphertextSizeInBytes), "ciphertext");
		}
		if (sharedSecret.Length != Algorithm.SharedSecretSizeInBytes)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_DestinationImprecise, Algorithm.SharedSecretSizeInBytes), "sharedSecret");
		}
		if (((ReadOnlySpan<byte>)ciphertext).Overlaps((ReadOnlySpan<byte>)sharedSecret))
		{
			throw new CryptographicException(System.SR.Cryptography_OverlappingBuffers);
		}
		ThrowIfDisposed();
		EncapsulateCore(ciphertext, sharedSecret);
	}

	public void Encapsulate(out byte[] ciphertext, out byte[] sharedSecret)
	{
		ThrowIfDisposed();
		byte[] array = new byte[Algorithm.CiphertextSizeInBytes];
		byte[] array2 = new byte[Algorithm.SharedSecretSizeInBytes];
		EncapsulateCore(array, array2);
		sharedSecret = array2;
		ciphertext = array;
	}

	protected abstract void EncapsulateCore(Span<byte> ciphertext, Span<byte> sharedSecret);

	public void Decapsulate(ReadOnlySpan<byte> ciphertext, Span<byte> sharedSecret)
	{
		if (ciphertext.Length != Algorithm.CiphertextSizeInBytes)
		{
			throw new ArgumentException(System.SR.Argument_KemInvalidCiphertextLength, "ciphertext");
		}
		if (sharedSecret.Length != Algorithm.SharedSecretSizeInBytes)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_DestinationImprecise, Algorithm.SharedSecretSizeInBytes), "sharedSecret");
		}
		ThrowIfDisposed();
		DecapsulateCore(ciphertext, sharedSecret);
	}

	public byte[] Decapsulate(byte[] ciphertext)
	{
		ArgumentNullException.ThrowIfNull(ciphertext, "ciphertext");
		if (ciphertext.Length != Algorithm.CiphertextSizeInBytes)
		{
			throw new ArgumentException(System.SR.Argument_KemInvalidCiphertextLength, "ciphertext");
		}
		ThrowIfDisposed();
		byte[] array = new byte[Algorithm.SharedSecretSizeInBytes];
		DecapsulateCore(ciphertext, array);
		return array;
	}

	protected abstract void DecapsulateCore(ReadOnlySpan<byte> ciphertext, Span<byte> sharedSecret);

	public void ExportPrivateSeed(Span<byte> destination)
	{
		if (destination.Length != Algorithm.PrivateSeedSizeInBytes)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_DestinationImprecise, Algorithm.PrivateSeedSizeInBytes), "destination");
		}
		ThrowIfDisposed();
		ExportPrivateSeedCore(destination);
	}

	public byte[] ExportPrivateSeed()
	{
		ThrowIfDisposed();
		byte[] array = new byte[Algorithm.PrivateSeedSizeInBytes];
		ExportPrivateSeedCore(array);
		return array;
	}

	protected abstract void ExportPrivateSeedCore(Span<byte> destination);

	public static MLKem ImportPrivateSeed(MLKemAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		if (source.Length != algorithm.PrivateSeedSizeInBytes)
		{
			throw new ArgumentException(System.SR.Argument_KemInvalidSeedLength, "source");
		}
		ThrowIfNotSupported();
		return MLKemImplementation.ImportPrivateSeedImpl(algorithm, source);
	}

	public static MLKem ImportPrivateSeed(MLKemAlgorithm algorithm, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportPrivateSeed(algorithm, new ReadOnlySpan<byte>(source));
	}

	public static MLKem ImportDecapsulationKey(MLKemAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		if (source.Length != algorithm.DecapsulationKeySizeInBytes)
		{
			throw new ArgumentException(System.SR.Argument_KemInvalidDecapsulationKeyLength, "source");
		}
		ThrowIfNotSupported();
		return MLKemImplementation.ImportDecapsulationKeyImpl(algorithm, source);
	}

	public static MLKem ImportDecapsulationKey(MLKemAlgorithm algorithm, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportDecapsulationKey(algorithm, new ReadOnlySpan<byte>(source));
	}

	public static MLKem ImportEncapsulationKey(MLKemAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		if (source.Length != algorithm.EncapsulationKeySizeInBytes)
		{
			throw new ArgumentException(System.SR.Argument_KemInvalidEncapsulationKeyLength, "source");
		}
		ThrowIfNotSupported();
		return MLKemImplementation.ImportEncapsulationKeyImpl(algorithm, source);
	}

	public static MLKem ImportEncapsulationKey(MLKemAlgorithm algorithm, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportEncapsulationKey(algorithm, new ReadOnlySpan<byte>(source));
	}

	public void ExportDecapsulationKey(Span<byte> destination)
	{
		if (destination.Length != Algorithm.DecapsulationKeySizeInBytes)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_DestinationImprecise, Algorithm.DecapsulationKeySizeInBytes), "destination");
		}
		ThrowIfDisposed();
		ExportDecapsulationKeyCore(destination);
	}

	public byte[] ExportDecapsulationKey()
	{
		ThrowIfDisposed();
		byte[] array = new byte[Algorithm.DecapsulationKeySizeInBytes];
		ExportDecapsulationKeyCore(array);
		return array;
	}

	protected abstract void ExportDecapsulationKeyCore(Span<byte> destination);

	public void ExportEncapsulationKey(Span<byte> destination)
	{
		if (destination.Length != Algorithm.EncapsulationKeySizeInBytes)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_DestinationImprecise, Algorithm.EncapsulationKeySizeInBytes), "destination");
		}
		ThrowIfDisposed();
		ExportEncapsulationKeyCore(destination);
	}

	public byte[] ExportEncapsulationKey()
	{
		ThrowIfDisposed();
		byte[] array = new byte[Algorithm.EncapsulationKeySizeInBytes];
		ExportEncapsulationKeyCore(array);
		return array;
	}

	protected abstract void ExportEncapsulationKeyCore(Span<byte> destination);

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public bool TryExportSubjectPublicKeyInfo(Span<byte> destination, out int bytesWritten)
	{
		ThrowIfDisposed();
		return ExportSubjectPublicKeyInfoCore().TryEncode(destination, out bytesWritten);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public byte[] ExportSubjectPublicKeyInfo()
	{
		ThrowIfDisposed();
		return ExportSubjectPublicKeyInfoCore().Encode();
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public string ExportSubjectPublicKeyInfoPem()
	{
		ThrowIfDisposed();
		AsnWriter writer = ExportSubjectPublicKeyInfoCore();
		return Helpers.EncodeAsnWriterToPem("PUBLIC KEY", writer, clear: false);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public bool TryExportPkcs8PrivateKey(Span<byte> destination, out int bytesWritten)
	{
		ThrowIfDisposed();
		if (destination.Length < 86)
		{
			bytesWritten = 0;
			return false;
		}
		return TryExportPkcs8PrivateKeyCore(destination, out bytesWritten);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public byte[] ExportPkcs8PrivateKey()
	{
		ThrowIfDisposed();
		return ExportPkcs8PrivateKeyCallback((ReadOnlySpan<byte> pkcs8) => pkcs8.ToArray());
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public string ExportPkcs8PrivateKeyPem()
	{
		ThrowIfDisposed();
		return ExportPkcs8PrivateKeyCallback((ReadOnlySpan<byte> pkcs8) => PemEncoding.WriteString("PRIVATE KEY".AsSpan(), pkcs8));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	protected abstract bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten);

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public bool TryExportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, PbeParameters pbeParameters, Span<byte> destination, out int bytesWritten)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, password, ReadOnlySpan<byte>.Empty);
		ThrowIfDisposed();
		return ExportEncryptedPkcs8PrivateKeyCore(password, pbeParameters, KeyFormatHelper.WriteEncryptedPkcs8).TryEncode(destination, out bytesWritten);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public bool TryExportEncryptedPkcs8PrivateKey(string password, PbeParameters pbeParameters, Span<byte> destination, out int bytesWritten)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		return TryExportEncryptedPkcs8PrivateKey(password.AsSpan(), pbeParameters, destination, out bytesWritten);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public bool TryExportEncryptedPkcs8PrivateKey(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters, Span<byte> destination, out int bytesWritten)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, ReadOnlySpan<char>.Empty, passwordBytes);
		ThrowIfDisposed();
		return ExportEncryptedPkcs8PrivateKeyCore(passwordBytes, pbeParameters, KeyFormatHelper.WriteEncryptedPkcs8).TryEncode(destination, out bytesWritten);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public byte[] ExportEncryptedPkcs8PrivateKey(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, ReadOnlySpan<char>.Empty, passwordBytes);
		ThrowIfDisposed();
		return ExportEncryptedPkcs8PrivateKeyCore(passwordBytes, pbeParameters, KeyFormatHelper.WriteEncryptedPkcs8).Encode();
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public byte[] ExportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, password, ReadOnlySpan<byte>.Empty);
		ThrowIfDisposed();
		return ExportEncryptedPkcs8PrivateKeyCore(password, pbeParameters, KeyFormatHelper.WriteEncryptedPkcs8).Encode();
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public byte[] ExportEncryptedPkcs8PrivateKey(string password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		return ExportEncryptedPkcs8PrivateKey(password.AsSpan(), pbeParameters);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public string ExportEncryptedPkcs8PrivateKeyPem(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, ReadOnlySpan<char>.Empty, passwordBytes);
		ThrowIfDisposed();
		AsnWriter writer = ExportEncryptedPkcs8PrivateKeyCore(passwordBytes, pbeParameters, KeyFormatHelper.WriteEncryptedPkcs8);
		return Helpers.EncodeAsnWriterToPem("ENCRYPTED PRIVATE KEY", writer, clear: false);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public string ExportEncryptedPkcs8PrivateKeyPem(ReadOnlySpan<char> password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, password, ReadOnlySpan<byte>.Empty);
		ThrowIfDisposed();
		AsnWriter writer = ExportEncryptedPkcs8PrivateKeyCore(password, pbeParameters, KeyFormatHelper.WriteEncryptedPkcs8);
		return Helpers.EncodeAsnWriterToPem("ENCRYPTED PRIVATE KEY", writer, clear: false);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public string ExportEncryptedPkcs8PrivateKeyPem(string password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		return ExportEncryptedPkcs8PrivateKeyPem(password.AsSpan(), pbeParameters);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLKem ImportSubjectPublicKeyInfo(ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		KeyFormatHelper.ReadSubjectPublicKeyInfo(s_knownOids, source, (KeyFormatHelper.KeyReader<MLKem>)SubjectPublicKeyReader, out int _, out MLKem ret);
		return ret;
		static void SubjectPublicKeyReader(ReadOnlyMemory<byte> key, in AlgorithmIdentifierAsn identifier, out MLKem kem)
		{
			MLKemAlgorithm algorithmIdentifier = GetAlgorithmIdentifier(in identifier);
			if (key.Length != algorithmIdentifier.EncapsulationKeySizeInBytes)
			{
				throw new CryptographicException(System.SR.Argument_KemInvalidEncapsulationKeyLength);
			}
			kem = MLKemImplementation.ImportEncapsulationKeyImpl(algorithmIdentifier, key.Span);
		}
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLKem ImportSubjectPublicKeyInfo(byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportSubjectPublicKeyInfo(new ReadOnlySpan<byte>(source));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLKem ImportPkcs8PrivateKey(ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		KeyFormatHelper.ReadPkcs8(s_knownOids, source, (KeyFormatHelper.KeyReader<MLKem>)MLKemKeyReader, out int _, out MLKem ret);
		return ret;
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLKem ImportPkcs8PrivateKey(byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportPkcs8PrivateKey(new ReadOnlySpan<byte>(source));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLKem ImportEncryptedPkcs8PrivateKey(ReadOnlySpan<byte> passwordBytes, ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		int bytesRead;
		return KeyFormatHelper.DecryptPkcs8(passwordBytes, source, ImportPkcs8PrivateKey, out bytesRead);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLKem ImportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		int bytesRead;
		return KeyFormatHelper.DecryptPkcs8(password, source, ImportPkcs8PrivateKey, out bytesRead);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLKem ImportEncryptedPkcs8PrivateKey(string password, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		ArgumentNullException.ThrowIfNull(source, "source");
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		int bytesRead;
		return KeyFormatHelper.DecryptPkcs8(password.AsSpan(), source, ImportPkcs8PrivateKey, out bytesRead);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLKem ImportFromPem(ReadOnlySpan<char> source)
	{
		ThrowIfNotSupported();
		return PemKeyHelpers.ImportFactoryPem(source, delegate(ReadOnlySpan<char> label)
		{
			if (label.SequenceEqual("PRIVATE KEY".AsSpan()))
			{
				return ImportPkcs8PrivateKey;
			}
			return label.SequenceEqual("PUBLIC KEY".AsSpan()) ? new PemKeyHelpers.ImportFactoryKeyAction<MLKem>(ImportSubjectPublicKeyInfo) : null;
		});
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLKem ImportFromPem(string source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportFromPem(source.AsSpan());
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLKem ImportFromEncryptedPem(ReadOnlySpan<char> source, ReadOnlySpan<char> password)
	{
		return PemKeyHelpers.ImportEncryptedFactoryPem(source, password, ImportEncryptedPkcs8PrivateKey);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLKem ImportFromEncryptedPem(ReadOnlySpan<char> source, ReadOnlySpan<byte> passwordBytes)
	{
		return PemKeyHelpers.ImportEncryptedFactoryPem(source, passwordBytes, ImportEncryptedPkcs8PrivateKey);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLKem ImportFromEncryptedPem(string source, string password)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(password, "password");
		return ImportFromEncryptedPem(source.AsSpan(), password.AsSpan());
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLKem ImportFromEncryptedPem(string source, byte[] passwordBytes)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(passwordBytes, "passwordBytes");
		return ImportFromEncryptedPem(source.AsSpan(), new ReadOnlySpan<byte>(passwordBytes));
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	private AsnWriter ExportSubjectPublicKeyInfoCore()
	{
		int encapsulationKeySizeInBytes = Algorithm.EncapsulationKeySizeInBytes;
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(encapsulationKeySizeInBytes);
		Memory<byte> memory = array.AsMemory(0, encapsulationKeySizeInBytes);
		try
		{
			ExportEncapsulationKeyCore(memory.Span);
			SubjectPublicKeyInfoAsn subjectPublicKeyInfoAsn = new SubjectPublicKeyInfoAsn
			{
				Algorithm = new AlgorithmIdentifierAsn
				{
					Algorithm = Algorithm.Oid,
					Parameters = null
				},
				SubjectPublicKey = memory
			};
			int initialCapacity = checked(32 + encapsulationKeySizeInBytes);
			AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER, initialCapacity);
			subjectPublicKeyInfoAsn.Encode(asnWriter);
			return asnWriter;
		}
		finally
		{
			System.Security.Cryptography.CryptoPool.Return(array, 0);
		}
	}

	private protected static void ThrowIfNotSupported()
	{
		if (!IsSupported)
		{
			throw new PlatformNotSupportedException(System.SR.Format(System.SR.Cryptography_AlgorithmNotSupported, "MLKem"));
		}
	}

	private static MLKemAlgorithm GetAlgorithmIdentifier(ref readonly AlgorithmIdentifierAsn identifier)
	{
		MLKemAlgorithm result = MLKemAlgorithm.FromOid(identifier.Algorithm);
		if (identifier.Parameters.HasValue)
		{
			AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
			identifier.Encode(asnWriter);
			throw Helpers.CreateAlgorithmUnknownException(asnWriter);
		}
		return result;
	}

	private static void MLKemKeyReader(ReadOnlyMemory<byte> privateKeyContents, in AlgorithmIdentifierAsn algorithmIdentifier, out MLKem kem)
	{
		MLKemAlgorithm algorithmIdentifier2 = GetAlgorithmIdentifier(in algorithmIdentifier);
		MLKemPrivateKeyAsn mLKemPrivateKeyAsn = MLKemPrivateKeyAsn.Decode(privateKeyContents, AsnEncodingRules.BER);
		ReadOnlyMemory<byte>? seed = mLKemPrivateKeyAsn.Seed;
		if (seed.HasValue)
		{
			ReadOnlyMemory<byte> valueOrDefault = seed.GetValueOrDefault();
			if (valueOrDefault.Length != algorithmIdentifier2.PrivateSeedSizeInBytes)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
			kem = MLKemImplementation.ImportPrivateSeedImpl(algorithmIdentifier2, valueOrDefault.Span);
			return;
		}
		seed = mLKemPrivateKeyAsn.ExpandedKey;
		if (seed.HasValue)
		{
			ReadOnlyMemory<byte> valueOrDefault2 = seed.GetValueOrDefault();
			if (valueOrDefault2.Length != algorithmIdentifier2.DecapsulationKeySizeInBytes)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
			kem = MLKemImplementation.ImportDecapsulationKeyImpl(algorithmIdentifier2, valueOrDefault2.Span);
			return;
		}
		MLKemPrivateKeyBothAsn? both = mLKemPrivateKeyAsn.Both;
		if (both.HasValue)
		{
			MLKemPrivateKeyBothAsn valueOrDefault3 = both.GetValueOrDefault();
			int decapsulationKeySizeInBytes = algorithmIdentifier2.DecapsulationKeySizeInBytes;
			if (valueOrDefault3.Seed.Length != algorithmIdentifier2.PrivateSeedSizeInBytes || valueOrDefault3.ExpandedKey.Length != decapsulationKeySizeInBytes)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
			MLKem mLKem = MLKemImplementation.ImportPrivateSeedImpl(algorithmIdentifier2, valueOrDefault3.Seed.Span);
			byte[] array = System.Security.Cryptography.CryptoPool.Rent(decapsulationKeySizeInBytes);
			Span<byte> span = array.AsSpan(0, decapsulationKeySizeInBytes);
			try
			{
				mLKem.ExportDecapsulationKey(span);
				if (CryptographicOperations.FixedTimeEquals(span, valueOrDefault3.ExpandedKey.Span))
				{
					kem = mLKem;
					return;
				}
				throw new CryptographicException(System.SR.Cryptography_KemPkcs8KeyMismatch);
			}
			catch
			{
				mLKem.Dispose();
				throw;
			}
			finally
			{
				System.Security.Cryptography.CryptoPool.Return(array, decapsulationKeySizeInBytes);
			}
		}
		throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
	}

	private protected void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(_disposed, typeof(MLKem));
	}

	private AsnWriter ExportEncryptedPkcs8PrivateKeyCore<TChar>(ReadOnlySpan<TChar> password, PbeParameters pbeParameters, WriteEncryptedPkcs8Func<TChar> encryptor)
	{
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(Algorithm.DecapsulationKeySizeInBytes + 32);
		int bytesWritten;
		while (!TryExportPkcs8PrivateKey(array, out bytesWritten))
		{
			System.Security.Cryptography.CryptoPool.Return(array, 0);
			array = System.Security.Cryptography.CryptoPool.Rent(array.Length * 2);
		}
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.BER, bytesWritten);
		try
		{
			asnWriter.WriteEncodedValueForCrypto(array.AsSpan(0, bytesWritten));
			return encryptor(password, asnWriter, pbeParameters);
		}
		finally
		{
			asnWriter.Reset();
			System.Security.Cryptography.CryptoPool.Return(array, bytesWritten);
		}
	}

	private TResult ExportPkcs8PrivateKeyCallback<TResult>(ExportPkcs8PrivateKeyFunc<TResult> func)
	{
		int num = Algorithm.DecapsulationKeySizeInBytes + 32;
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(num);
		int bytesWritten;
		while (!TryExportPkcs8PrivateKeyCore(array, out bytesWritten))
		{
			System.Security.Cryptography.CryptoPool.Return(array);
			num = checked(num * 2);
			array = System.Security.Cryptography.CryptoPool.Rent(num);
		}
		if (bytesWritten < 0 || bytesWritten > array.Length)
		{
			CryptographicOperations.ZeroMemory(array);
			throw new CryptographicException();
		}
		TResult result = func(array.AsSpan(0, bytesWritten));
		System.Security.Cryptography.CryptoPool.Return(array, bytesWritten);
		return result;
	}

	private protected static void ThrowIfNoSeed(bool hasSeed)
	{
		if (!hasSeed)
		{
			throw new CryptographicException(System.SR.Cryptography_PqcNoSeed);
		}
	}

	private protected static void ThrowIfNoDecapsulationKey(bool hasDecapsulationKey)
	{
		if (!hasDecapsulationKey)
		{
			throw new CryptographicException(System.SR.Cryptography_KemNoDecapsulationKey);
		}
	}

	private protected unsafe void ReadCngMLKemBlob(global::Interop.BCrypt.KeyBlobMagicNumber kind, ReadOnlySpan<byte> exportedSpan, Span<byte> destination)
	{
		fixed (byte* ptr = exportedSpan)
		{
			global::Interop.BCrypt.BCRYPT_MLKEM_KEY_BLOB* ptr2 = (global::Interop.BCrypt.BCRYPT_MLKEM_KEY_BLOB*)ptr;
			if (ptr2->dwMagic != kind)
			{
				throw new CryptographicException();
			}
			int num = Marshal.SizeOf<global::Interop.BCrypt.BCRYPT_MLKEM_KEY_BLOB>();
			int num2;
			int num3;
			checked
			{
				num2 = (int)ptr2->cbKey;
				if (num2 != destination.Length)
				{
					throw new CryptographicException(System.SR.Cryptography_NotValidPublicOrPrivateKey);
				}
				num3 = (int)ptr2->cbParameterSet;
			}
			ReadOnlySpan<char> readOnlySpan = new ReadOnlySpan<char>(ptr + num, num3 / 2);
			ReadOnlySpan<char> span = readOnlySpan.Slice(0, readOnlySpan.Length - 1);
			ReadOnlySpan<char> other = PqcBlobHelpers.GetMLKemParameterSet(Algorithm).AsSpan();
			if (!span.SequenceEqual(other))
			{
				goto IL_009a;
			}
			if (readOnlySpan[readOnlySpan.Length - 1] != 0)
			{
				goto IL_009a;
			}
			exportedSpan.Slice(num + num3, num2).CopyTo(destination);
			goto end_IL_000a;
			IL_009a:
			throw new CryptographicException(System.SR.Cryptography_NotValidPublicOrPrivateKey);
			end_IL_000a:;
		}
	}
}
