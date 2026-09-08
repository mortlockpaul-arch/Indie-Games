using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Security.Cryptography.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography;

[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
public abstract class SlhDsa : IDisposable
{
	private delegate TResult ExportPkcs8PrivateKeyFunc<TResult>(ReadOnlySpan<byte> pkcs8);

	private static readonly string[] s_knownOids = new string[12]
	{
		"2.16.840.1.101.3.4.3.20", "2.16.840.1.101.3.4.3.26", "2.16.840.1.101.3.4.3.21", "2.16.840.1.101.3.4.3.27", "2.16.840.1.101.3.4.3.22", "2.16.840.1.101.3.4.3.28", "2.16.840.1.101.3.4.3.23", "2.16.840.1.101.3.4.3.29", "2.16.840.1.101.3.4.3.24", "2.16.840.1.101.3.4.3.30",
		"2.16.840.1.101.3.4.3.25", "2.16.840.1.101.3.4.3.31"
	};

	private bool _disposed;

	public static bool IsSupported { get; } = false;

	public SlhDsaAlgorithm Algorithm { get; }

	protected SlhDsa(SlhDsaAlgorithm algorithm)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		Algorithm = algorithm;
	}

	private protected void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(_disposed, typeof(SlhDsa));
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

	public void SignData(ReadOnlySpan<byte> data, Span<byte> destination, ReadOnlySpan<byte> context = default(ReadOnlySpan<byte>))
	{
		int signatureSizeInBytes = Algorithm.SignatureSizeInBytes;
		if (destination.Length != signatureSizeInBytes)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_DestinationImprecise, signatureSizeInBytes), "destination");
		}
		if (context.Length > 255)
		{
			throw new ArgumentOutOfRangeException("context", context.Length, System.SR.Argument_SignatureContextTooLong255);
		}
		ThrowIfDisposed();
		SignDataCore(data, context, destination);
	}

	public byte[] SignData(byte[] data, byte[]? context = null)
	{
		ArgumentNullException.ThrowIfNull(data, "data");
		byte[] array = new byte[Algorithm.SignatureSizeInBytes];
		SignData(new ReadOnlySpan<byte>(data), array.AsSpan(), new ReadOnlySpan<byte>(context));
		return array;
	}

	public bool VerifyData(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> context = default(ReadOnlySpan<byte>))
	{
		if (context.Length > 255)
		{
			throw new ArgumentOutOfRangeException("context", context.Length, System.SR.Argument_SignatureContextTooLong255);
		}
		ThrowIfDisposed();
		if (signature.Length != Algorithm.SignatureSizeInBytes)
		{
			return false;
		}
		return VerifyDataCore(data, context, signature);
	}

	public bool VerifyData(byte[] data, byte[] signature, byte[]? context = null)
	{
		ArgumentNullException.ThrowIfNull(data, "data");
		ArgumentNullException.ThrowIfNull(signature, "signature");
		return VerifyData(new ReadOnlySpan<byte>(data), new ReadOnlySpan<byte>(signature), new ReadOnlySpan<byte>(context));
	}

	public void SignPreHash(ReadOnlySpan<byte> hash, Span<byte> destination, string hashAlgorithmOid, ReadOnlySpan<byte> context = default(ReadOnlySpan<byte>))
	{
		ArgumentNullException.ThrowIfNull(hashAlgorithmOid, "hashAlgorithmOid");
		if (destination.Length != Algorithm.SignatureSizeInBytes)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_DestinationImprecise, Algorithm.SignatureSizeInBytes), "destination");
		}
		if (context.Length > 255)
		{
			throw new ArgumentOutOfRangeException("context", context.Length, System.SR.Argument_SignatureContextTooLong255);
		}
		Helpers.ValidateHashLength(hash, hashAlgorithmOid.AsSpan());
		ThrowIfDisposed();
		SignPreHashCore(hash, context, hashAlgorithmOid, destination);
	}

	public byte[] SignPreHash(byte[] hash, string hashAlgorithmOid, byte[]? context = null)
	{
		ArgumentNullException.ThrowIfNull(hash, "hash");
		ArgumentNullException.ThrowIfNull(hashAlgorithmOid, "hashAlgorithmOid");
		byte[] array = new byte[Algorithm.SignatureSizeInBytes];
		SignPreHash(new ReadOnlySpan<byte>(hash), array.AsSpan(), hashAlgorithmOid, new ReadOnlySpan<byte>(context));
		return array;
	}

	public bool VerifyPreHash(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature, string hashAlgorithmOid, ReadOnlySpan<byte> context = default(ReadOnlySpan<byte>))
	{
		ArgumentNullException.ThrowIfNull(hashAlgorithmOid, "hashAlgorithmOid");
		if (context.Length > 255)
		{
			throw new ArgumentOutOfRangeException("context", context.Length, System.SR.Argument_SignatureContextTooLong255);
		}
		Helpers.ValidateHashLength(hash, hashAlgorithmOid.AsSpan());
		ThrowIfDisposed();
		if (signature.Length != Algorithm.SignatureSizeInBytes)
		{
			return false;
		}
		return VerifyPreHashCore(hash, context, hashAlgorithmOid, signature);
	}

	public bool VerifyPreHash(byte[] hash, byte[] signature, string hashAlgorithmOid, byte[]? context = null)
	{
		ArgumentNullException.ThrowIfNull(hash, "hash");
		ArgumentNullException.ThrowIfNull(signature, "signature");
		ArgumentNullException.ThrowIfNull(hashAlgorithmOid, "hashAlgorithmOid");
		return VerifyPreHash(new ReadOnlySpan<byte>(hash), new ReadOnlySpan<byte>(signature), hashAlgorithmOid, new ReadOnlySpan<byte>(context));
	}

	public byte[] ExportSubjectPublicKeyInfo()
	{
		ThrowIfDisposed();
		return ExportSubjectPublicKeyInfoCore().Encode();
	}

	public bool TryExportSubjectPublicKeyInfo(Span<byte> destination, out int bytesWritten)
	{
		ThrowIfDisposed();
		return ExportSubjectPublicKeyInfoCore().TryEncode(destination, out bytesWritten);
	}

	public string ExportSubjectPublicKeyInfoPem()
	{
		ThrowIfDisposed();
		AsnWriter writer = ExportSubjectPublicKeyInfoCore();
		return Helpers.EncodeAsnWriterToPem("PUBLIC KEY", writer, clear: false);
	}

	public byte[] ExportPkcs8PrivateKey()
	{
		ThrowIfDisposed();
		return ExportPkcs8PrivateKeyCallback((ReadOnlySpan<byte> pkcs8) => pkcs8.ToArray());
	}

	public bool TryExportPkcs8PrivateKey(Span<byte> destination, out int bytesWritten)
	{
		ThrowIfDisposed();
		int num = 12 + Algorithm.PrivateKeySizeInBytes;
		if (destination.Length < num)
		{
			bytesWritten = 0;
			return false;
		}
		return TryExportPkcs8PrivateKeyCore(destination, out bytesWritten);
	}

	protected virtual bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten)
	{
		int privateKeySizeInBytes = Algorithm.PrivateKeySizeInBytes;
		Span<byte> span = stackalloc byte[128].Slice(0, privateKeySizeInBytes);
		try
		{
			ExportSlhDsaPrivateKey(span);
			int initialCapacity = checked(32 + privateKeySizeInBytes);
			AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER, initialCapacity);
			using (asnWriter.PushSequence())
			{
				asnWriter.WriteInteger(0L);
				using (asnWriter.PushSequence())
				{
					asnWriter.WriteObjectIdentifier(Algorithm.Oid);
				}
				asnWriter.WriteOctetString(span);
			}
			return asnWriter.TryEncode(destination, out bytesWritten);
		}
		finally
		{
			CryptographicOperations.ZeroMemory(span);
		}
	}

	public string ExportPkcs8PrivateKeyPem()
	{
		ThrowIfDisposed();
		return ExportPkcs8PrivateKeyCallback((ReadOnlySpan<byte> pkcs8) => PemEncoding.WriteString("PRIVATE KEY".AsSpan(), pkcs8));
	}

	public byte[] ExportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, password, ReadOnlySpan<byte>.Empty);
		ThrowIfDisposed();
		AsnWriter asnWriter = ExportEncryptedPkcs8PrivateKeyCore(password, pbeParameters);
		try
		{
			return asnWriter.Encode();
		}
		finally
		{
			asnWriter.Reset();
		}
	}

	public byte[] ExportEncryptedPkcs8PrivateKey(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, ReadOnlySpan<char>.Empty, passwordBytes);
		ThrowIfDisposed();
		AsnWriter asnWriter = ExportEncryptedPkcs8PrivateKeyCore(passwordBytes, pbeParameters);
		try
		{
			return asnWriter.Encode();
		}
		finally
		{
			asnWriter.Reset();
		}
	}

	public byte[] ExportEncryptedPkcs8PrivateKey(string password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		return ExportEncryptedPkcs8PrivateKey(password.AsSpan(), pbeParameters);
	}

	public bool TryExportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, PbeParameters pbeParameters, Span<byte> destination, out int bytesWritten)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, password, ReadOnlySpan<byte>.Empty);
		ThrowIfDisposed();
		AsnWriter asnWriter = ExportEncryptedPkcs8PrivateKeyCore(password, pbeParameters);
		try
		{
			return asnWriter.TryEncode(destination, out bytesWritten);
		}
		finally
		{
			asnWriter.Reset();
		}
	}

	public bool TryExportEncryptedPkcs8PrivateKey(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters, Span<byte> destination, out int bytesWritten)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, ReadOnlySpan<char>.Empty, passwordBytes);
		ThrowIfDisposed();
		AsnWriter asnWriter = ExportEncryptedPkcs8PrivateKeyCore(passwordBytes, pbeParameters);
		try
		{
			return asnWriter.TryEncode(destination, out bytesWritten);
		}
		finally
		{
			asnWriter.Reset();
		}
	}

	public bool TryExportEncryptedPkcs8PrivateKey(string password, PbeParameters pbeParameters, Span<byte> destination, out int bytesWritten)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		return TryExportEncryptedPkcs8PrivateKey(password.AsSpan(), pbeParameters, destination, out bytesWritten);
	}

	public string ExportEncryptedPkcs8PrivateKeyPem(ReadOnlySpan<char> password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, password, ReadOnlySpan<byte>.Empty);
		ThrowIfDisposed();
		AsnWriter asnWriter = ExportEncryptedPkcs8PrivateKeyCore(password, pbeParameters);
		try
		{
			return Helpers.EncodeAsnWriterToPem("ENCRYPTED PRIVATE KEY", asnWriter, clear: false);
		}
		finally
		{
			asnWriter.Reset();
		}
	}

	public string ExportEncryptedPkcs8PrivateKeyPem(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, ReadOnlySpan<char>.Empty, passwordBytes);
		ThrowIfDisposed();
		AsnWriter asnWriter = ExportEncryptedPkcs8PrivateKeyCore(passwordBytes, pbeParameters);
		try
		{
			return Helpers.EncodeAsnWriterToPem("ENCRYPTED PRIVATE KEY", asnWriter, clear: false);
		}
		finally
		{
			asnWriter.Reset();
		}
	}

	public string ExportEncryptedPkcs8PrivateKeyPem(string password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		return ExportEncryptedPkcs8PrivateKeyPem(password.AsSpan(), pbeParameters);
	}

	public void ExportSlhDsaPublicKey(Span<byte> destination)
	{
		int publicKeySizeInBytes = Algorithm.PublicKeySizeInBytes;
		if (destination.Length != publicKeySizeInBytes)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_DestinationImprecise, publicKeySizeInBytes), "destination");
		}
		ThrowIfDisposed();
		ExportSlhDsaPublicKeyCore(destination);
	}

	public byte[] ExportSlhDsaPublicKey()
	{
		ThrowIfDisposed();
		byte[] array = new byte[Algorithm.PublicKeySizeInBytes];
		ExportSlhDsaPublicKeyCore(array);
		return array;
	}

	public void ExportSlhDsaPrivateKey(Span<byte> destination)
	{
		int privateKeySizeInBytes = Algorithm.PrivateKeySizeInBytes;
		if (destination.Length != privateKeySizeInBytes)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_DestinationImprecise, privateKeySizeInBytes), "destination");
		}
		ThrowIfDisposed();
		ExportSlhDsaPrivateKeyCore(destination);
	}

	public byte[] ExportSlhDsaPrivateKey()
	{
		ThrowIfDisposed();
		byte[] array = new byte[Algorithm.PrivateKeySizeInBytes];
		ExportSlhDsaPrivateKeyCore(array);
		return array;
	}

	public static SlhDsa GenerateKey(SlhDsaAlgorithm algorithm)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		ThrowIfNotSupported();
		return SlhDsaImplementation.GenerateKeyCore(algorithm);
	}

	public static SlhDsa ImportSubjectPublicKeyInfo(ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		KeyFormatHelper.ReadSubjectPublicKeyInfo(s_knownOids, source, (KeyFormatHelper.KeyReader<SlhDsa>)SubjectPublicKeyReader, out int _, out SlhDsa ret);
		return ret;
		static void SubjectPublicKeyReader(ReadOnlyMemory<byte> key, in AlgorithmIdentifierAsn identifier, out SlhDsa slhDsa)
		{
			SlhDsaAlgorithm algorithmIdentifier = GetAlgorithmIdentifier(in identifier);
			if (key.Length != algorithmIdentifier.PublicKeySizeInBytes)
			{
				throw new CryptographicException(System.SR.Argument_PublicKeyWrongSizeForAlgorithm);
			}
			slhDsa = SlhDsaImplementation.ImportPublicKey(algorithmIdentifier, key.Span);
		}
	}

	public static SlhDsa ImportSubjectPublicKeyInfo(byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportSubjectPublicKeyInfo(new ReadOnlySpan<byte>(source));
	}

	public static SlhDsa ImportPkcs8PrivateKey(ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		KeyFormatHelper.ReadPkcs8(s_knownOids, source, delegate(ReadOnlyMemory<byte> key, in AlgorithmIdentifierAsn algId, out SlhDsa reference)
		{
			SlhDsaAlgorithm algorithmIdentifier = GetAlgorithmIdentifier(in algId);
			if (key.Span.Length != algorithmIdentifier.PrivateKeySizeInBytes)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
			reference = ImportSlhDsaPrivateKey(algorithmIdentifier, key.Span);
		}, out var _, out var ret);
		return ret;
	}

	public static SlhDsa ImportPkcs8PrivateKey(byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportPkcs8PrivateKey(new ReadOnlySpan<byte>(source));
	}

	public static SlhDsa ImportEncryptedPkcs8PrivateKey(ReadOnlySpan<byte> passwordBytes, ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		int bytesRead;
		return KeyFormatHelper.DecryptPkcs8(passwordBytes, source, ImportPkcs8PrivateKey, out bytesRead);
	}

	public static SlhDsa ImportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		int bytesRead;
		return KeyFormatHelper.DecryptPkcs8(password, source, ImportPkcs8PrivateKey, out bytesRead);
	}

	public static SlhDsa ImportEncryptedPkcs8PrivateKey(string password, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportEncryptedPkcs8PrivateKey(password.AsSpan(), new ReadOnlySpan<byte>(source));
	}

	public static SlhDsa ImportFromPem(ReadOnlySpan<char> source)
	{
		ThrowIfNotSupported();
		return PemKeyHelpers.ImportFactoryPem(source, delegate(ReadOnlySpan<char> label)
		{
			if (label.SequenceEqual("PRIVATE KEY".AsSpan()))
			{
				return ImportPkcs8PrivateKey;
			}
			return label.SequenceEqual("PUBLIC KEY".AsSpan()) ? new PemKeyHelpers.ImportFactoryKeyAction<SlhDsa>(ImportSubjectPublicKeyInfo) : null;
		});
	}

	public static SlhDsa ImportFromPem(string source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ThrowIfNotSupported();
		return ImportFromPem(source.AsSpan());
	}

	public static SlhDsa ImportFromEncryptedPem(ReadOnlySpan<char> source, ReadOnlySpan<char> password)
	{
		ThrowIfNotSupported();
		return PemKeyHelpers.ImportEncryptedFactoryPem(source, password, ImportEncryptedPkcs8PrivateKey);
	}

	public static SlhDsa ImportFromEncryptedPem(ReadOnlySpan<char> source, ReadOnlySpan<byte> passwordBytes)
	{
		ThrowIfNotSupported();
		return PemKeyHelpers.ImportEncryptedFactoryPem(source, passwordBytes, ImportEncryptedPkcs8PrivateKey);
	}

	public static SlhDsa ImportFromEncryptedPem(string source, string password)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(password, "password");
		ThrowIfNotSupported();
		return ImportFromEncryptedPem(source.AsSpan(), password.AsSpan());
	}

	public static SlhDsa ImportFromEncryptedPem(string source, byte[] passwordBytes)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(passwordBytes, "passwordBytes");
		ThrowIfNotSupported();
		return ImportFromEncryptedPem(source.AsSpan(), new ReadOnlySpan<byte>(passwordBytes));
	}

	public static SlhDsa ImportSlhDsaPublicKey(SlhDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		if (source.Length != algorithm.PublicKeySizeInBytes)
		{
			throw new ArgumentException(System.SR.Argument_PublicKeyWrongSizeForAlgorithm, "source");
		}
		ThrowIfNotSupported();
		return SlhDsaImplementation.ImportPublicKey(algorithm, source);
	}

	public static SlhDsa ImportSlhDsaPublicKey(SlhDsaAlgorithm algorithm, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportSlhDsaPublicKey(algorithm, new ReadOnlySpan<byte>(source));
	}

	public static SlhDsa ImportSlhDsaPrivateKey(SlhDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		if (source.Length != algorithm.PrivateKeySizeInBytes)
		{
			throw new ArgumentException(System.SR.Argument_PrivateKeyWrongSizeForAlgorithm, "source");
		}
		ThrowIfNotSupported();
		return SlhDsaImplementation.ImportPrivateKey(algorithm, source);
	}

	public static SlhDsa ImportSlhDsaPrivateKey(SlhDsaAlgorithm algorithm, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportSlhDsaPrivateKey(algorithm, new ReadOnlySpan<byte>(source));
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	protected abstract void SignDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, Span<byte> destination);

	protected abstract bool VerifyDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, ReadOnlySpan<byte> signature);

	protected abstract void SignPreHashCore(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> context, string hashAlgorithmOid, Span<byte> destination);

	protected abstract bool VerifyPreHashCore(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> context, string hashAlgorithmOid, ReadOnlySpan<byte> signature);

	protected abstract void ExportSlhDsaPublicKeyCore(Span<byte> destination);

	protected abstract void ExportSlhDsaPrivateKeyCore(Span<byte> destination);

	private AsnWriter ExportSubjectPublicKeyInfoCore()
	{
		int publicKeySizeInBytes = Algorithm.PublicKeySizeInBytes;
		Span<byte> span = stackalloc byte[64].Slice(0, publicKeySizeInBytes);
		ExportSlhDsaPublicKeyCore(span);
		int initialCapacity = checked(32 + publicKeySizeInBytes);
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER, initialCapacity);
		using (asnWriter.PushSequence())
		{
			using (asnWriter.PushSequence())
			{
				asnWriter.WriteObjectIdentifier(Algorithm.Oid);
			}
			asnWriter.WriteBitString(span);
			return asnWriter;
		}
	}

	private AsnWriter ExportEncryptedPkcs8PrivateKeyCore(ReadOnlySpan<char> password, PbeParameters pbeParameters)
	{
		AsnWriter asnWriter = ExportPkcs8PrivateKeyCallback(delegate(ReadOnlySpan<byte> pkcs8)
		{
			AsnWriter asnWriter2 = new AsnWriter(AsnEncodingRules.BER, pkcs8.Length);
			try
			{
				asnWriter2.WriteEncodedValueForCrypto(pkcs8);
				return asnWriter2;
			}
			catch
			{
				asnWriter2.Reset();
				throw;
			}
		});
		try
		{
			return KeyFormatHelper.WriteEncryptedPkcs8(password, asnWriter, pbeParameters);
		}
		finally
		{
			asnWriter.Reset();
		}
	}

	private AsnWriter ExportEncryptedPkcs8PrivateKeyCore(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
	{
		AsnWriter asnWriter = ExportPkcs8PrivateKeyCallback(delegate(ReadOnlySpan<byte> pkcs8)
		{
			AsnWriter asnWriter2 = new AsnWriter(AsnEncodingRules.BER, pkcs8.Length);
			try
			{
				asnWriter2.WriteEncodedValueForCrypto(pkcs8);
				return asnWriter2;
			}
			catch
			{
				asnWriter2.Reset();
				throw;
			}
		});
		try
		{
			return KeyFormatHelper.WriteEncryptedPkcs8(passwordBytes, asnWriter, pbeParameters);
		}
		finally
		{
			asnWriter.Reset();
		}
	}

	private TResult ExportPkcs8PrivateKeyCallback<TResult>(ExportPkcs8PrivateKeyFunc<TResult> func)
	{
		int num = Algorithm.PrivateKeySizeInBytes + 32;
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(num);
		int bytesWritten;
		while (!TryExportPkcs8PrivateKeyCore(array, out bytesWritten))
		{
			System.Security.Cryptography.CryptoPool.Return(array);
			num = checked(num * 2);
			array = System.Security.Cryptography.CryptoPool.Rent(num);
		}
		if ((uint)bytesWritten > array.Length)
		{
			CryptographicOperations.ZeroMemory(array);
			throw new CryptographicException();
		}
		try
		{
			return func(array.AsSpan(0, bytesWritten));
		}
		finally
		{
			System.Security.Cryptography.CryptoPool.Return(array, bytesWritten);
		}
	}

	private static SlhDsaAlgorithm GetAlgorithmIdentifier(ref readonly AlgorithmIdentifierAsn identifier)
	{
		SlhDsaAlgorithm algorithmFromOid = SlhDsaAlgorithm.GetAlgorithmFromOid(identifier.Algorithm);
		if (identifier.Parameters.HasValue)
		{
			AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
			identifier.Encode(asnWriter);
			throw Helpers.CreateAlgorithmUnknownException(asnWriter);
		}
		return algorithmFromOid;
	}

	private static void ThrowIfNotSupported()
	{
		if (!IsSupported)
		{
			throw new PlatformNotSupportedException(System.SR.Format(System.SR.Cryptography_AlgorithmNotSupported, "SlhDsa"));
		}
	}
}
