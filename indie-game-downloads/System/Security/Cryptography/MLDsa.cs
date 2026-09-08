using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Security.Cryptography.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography;

public abstract class MLDsa : IDisposable
{
	private delegate TResult ExportPkcs8PrivateKeyFunc<TResult>(ReadOnlySpan<byte> pkcs8);

	private protected static readonly string[] KnownOids = new string[3] { "2.16.840.1.101.3.4.3.17", "2.16.840.1.101.3.4.3.18", "2.16.840.1.101.3.4.3.19" };

	private bool _disposed;

	public MLDsaAlgorithm Algorithm { get; }

	public static bool IsSupported { get; } = MLDsaImplementation.SupportsAny();

	protected MLDsa(MLDsaAlgorithm algorithm)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		Algorithm = algorithm;
	}

	private protected void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(_disposed, typeof(MLDsa));
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			Dispose(disposing: true);
			_disposed = true;
			GC.SuppressFinalize(this);
		}
	}

	public void SignData(ReadOnlySpan<byte> data, Span<byte> destination, ReadOnlySpan<byte> context = default(ReadOnlySpan<byte>))
	{
		Helpers.ThrowIfDestinationWrongLength(destination, Algorithm.SignatureSizeInBytes, "destination");
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

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public void SignPreHash(ReadOnlySpan<byte> hash, Span<byte> destination, string hashAlgorithmOid, ReadOnlySpan<byte> context = default(ReadOnlySpan<byte>))
	{
		ArgumentNullException.ThrowIfNull(hashAlgorithmOid, "hashAlgorithmOid");
		Helpers.ThrowIfDestinationWrongLength(destination, Algorithm.SignatureSizeInBytes, "destination");
		if (context.Length > 255)
		{
			throw new ArgumentOutOfRangeException("context", context.Length, System.SR.Argument_SignatureContextTooLong255);
		}
		string text = MapHashOidToAlgorithm(hashAlgorithmOid, out var hashLengthInBytes, out var insufficientCollisionResistance);
		if (text == null)
		{
			throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownHashAlgorithm, hashAlgorithmOid));
		}
		if (insufficientCollisionResistance)
		{
			throw new CryptographicException(System.SR.Format(System.SR.Cryptography_HashMLDsaAlgorithmMismatch, Algorithm.Name, text));
		}
		if (hashLengthInBytes != hash.Length)
		{
			throw new CryptographicException(System.SR.Cryptography_HashLengthMismatch);
		}
		ThrowIfDisposed();
		SignPreHashCore(hash, context, hashAlgorithmOid, destination);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public byte[] SignPreHash(byte[] hash, string hashAlgorithmOid, byte[]? context = null)
	{
		ArgumentNullException.ThrowIfNull(hash, "hash");
		ArgumentNullException.ThrowIfNull(hashAlgorithmOid, "hashAlgorithmOid");
		byte[] array = new byte[Algorithm.SignatureSizeInBytes];
		SignPreHash(new ReadOnlySpan<byte>(hash), array.AsSpan(), hashAlgorithmOid, new ReadOnlySpan<byte>(context));
		return array;
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public bool VerifyPreHash(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> signature, string hashAlgorithmOid, ReadOnlySpan<byte> context = default(ReadOnlySpan<byte>))
	{
		ArgumentNullException.ThrowIfNull(hashAlgorithmOid, "hashAlgorithmOid");
		if (context.Length > 255)
		{
			throw new ArgumentOutOfRangeException("context", context.Length, System.SR.Argument_SignatureContextTooLong255);
		}
		if (((MapHashOidToAlgorithm(hashAlgorithmOid, out var hashLengthInBytes, out var insufficientCollisionResistance) == null) | insufficientCollisionResistance) || signature.Length != Algorithm.SignatureSizeInBytes)
		{
			return false;
		}
		if (hashLengthInBytes != hash.Length)
		{
			throw new CryptographicException(System.SR.Cryptography_HashLengthMismatch);
		}
		ThrowIfDisposed();
		return VerifyPreHashCore(hash, context, hashAlgorithmOid, signature);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public bool VerifyPreHash(byte[] hash, byte[] signature, string hashAlgorithmOid, byte[]? context = null)
	{
		ArgumentNullException.ThrowIfNull(hash, "hash");
		ArgumentNullException.ThrowIfNull(signature, "signature");
		ArgumentNullException.ThrowIfNull(hashAlgorithmOid, "hashAlgorithmOid");
		return VerifyPreHash(new ReadOnlySpan<byte>(hash), new ReadOnlySpan<byte>(signature), hashAlgorithmOid, new ReadOnlySpan<byte>(context));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public byte[] SignMu(byte[] externalMu)
	{
		ArgumentNullException.ThrowIfNull(externalMu, "externalMu");
		return SignMu(new ReadOnlySpan<byte>(externalMu));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public byte[] SignMu(ReadOnlySpan<byte> externalMu)
	{
		byte[] array = new byte[Algorithm.SignatureSizeInBytes];
		SignMu(externalMu, array.AsSpan());
		return array;
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public void SignMu(ReadOnlySpan<byte> externalMu, Span<byte> destination)
	{
		if (externalMu.Length != Algorithm.MuSizeInBytes)
		{
			throw new ArgumentException(System.SR.Argument_MLDsaMuInvalidLength, "externalMu");
		}
		Helpers.ThrowIfDestinationWrongLength(destination, Algorithm.SignatureSizeInBytes, "destination");
		ThrowIfDisposed();
		SignMuCore(externalMu, destination);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	protected abstract void SignMuCore(ReadOnlySpan<byte> externalMu, Span<byte> destination);

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public bool VerifyMu(byte[] externalMu, byte[] signature)
	{
		ArgumentNullException.ThrowIfNull(externalMu, "externalMu");
		ArgumentNullException.ThrowIfNull(signature, "signature");
		return VerifyMu(new ReadOnlySpan<byte>(externalMu), new ReadOnlySpan<byte>(signature));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public bool VerifyMu(ReadOnlySpan<byte> externalMu, ReadOnlySpan<byte> signature)
	{
		if (externalMu.Length != Algorithm.MuSizeInBytes || signature.Length != Algorithm.SignatureSizeInBytes)
		{
			return false;
		}
		ThrowIfDisposed();
		return VerifyMuCore(externalMu, signature);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	protected abstract bool VerifyMuCore(ReadOnlySpan<byte> externalMu, ReadOnlySpan<byte> signature);

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public byte[] ExportSubjectPublicKeyInfo()
	{
		ThrowIfDisposed();
		return ExportSubjectPublicKeyInfoCore().Encode();
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public bool TryExportSubjectPublicKeyInfo(Span<byte> destination, out int bytesWritten)
	{
		ThrowIfDisposed();
		return ExportSubjectPublicKeyInfoCore().TryEncode(destination, out bytesWritten);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public string ExportSubjectPublicKeyInfoPem()
	{
		ThrowIfDisposed();
		AsnWriter writer = ExportSubjectPublicKeyInfoCore();
		return Helpers.EncodeAsnWriterToPem("PUBLIC KEY", writer, clear: false);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public byte[] ExportPkcs8PrivateKey()
	{
		ThrowIfDisposed();
		return ExportPkcs8PrivateKeyCallback((ReadOnlySpan<byte> pkcs8) => pkcs8.ToArray());
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public bool TryExportPkcs8PrivateKey(Span<byte> destination, out int bytesWritten)
	{
		ThrowIfDisposed();
		int num = 12 + Algorithm.PrivateSeedSizeInBytes;
		if (destination.Length < num)
		{
			bytesWritten = 0;
			return false;
		}
		return TryExportPkcs8PrivateKeyCore(destination, out bytesWritten);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	protected abstract bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten);

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public string ExportPkcs8PrivateKeyPem()
	{
		ThrowIfDisposed();
		return ExportPkcs8PrivateKeyCallback((ReadOnlySpan<byte> pkcs8) => PemEncoding.WriteString("PRIVATE KEY".AsSpan(), pkcs8));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
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

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
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

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public byte[] ExportEncryptedPkcs8PrivateKey(string password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		return ExportEncryptedPkcs8PrivateKey(password.AsSpan(), pbeParameters);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
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

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
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

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public bool TryExportEncryptedPkcs8PrivateKey(string password, PbeParameters pbeParameters, Span<byte> destination, out int bytesWritten)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		return TryExportEncryptedPkcs8PrivateKey(password.AsSpan(), pbeParameters, destination, out bytesWritten);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public string ExportEncryptedPkcs8PrivateKeyPem(ReadOnlySpan<char> password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, password, ReadOnlySpan<byte>.Empty);
		ThrowIfDisposed();
		AsnWriter writer = ExportEncryptedPkcs8PrivateKeyCore(password, pbeParameters);
		return Helpers.EncodeAsnWriterToPem("ENCRYPTED PRIVATE KEY", writer, clear: false);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public string ExportEncryptedPkcs8PrivateKeyPem(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, ReadOnlySpan<char>.Empty, passwordBytes);
		ThrowIfDisposed();
		AsnWriter writer = ExportEncryptedPkcs8PrivateKeyCore(passwordBytes, pbeParameters);
		return Helpers.EncodeAsnWriterToPem("ENCRYPTED PRIVATE KEY", writer, clear: false);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public string ExportEncryptedPkcs8PrivateKeyPem(string password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		return ExportEncryptedPkcs8PrivateKeyPem(password.AsSpan(), pbeParameters);
	}

	public byte[] ExportMLDsaPublicKey()
	{
		ThrowIfDisposed();
		byte[] array = new byte[Algorithm.PublicKeySizeInBytes];
		ExportMLDsaPublicKeyCore(array);
		return array;
	}

	public void ExportMLDsaPublicKey(Span<byte> destination)
	{
		Helpers.ThrowIfDestinationWrongLength(destination, Algorithm.PublicKeySizeInBytes, "destination");
		ThrowIfDisposed();
		ExportMLDsaPublicKeyCore(destination);
	}

	public byte[] ExportMLDsaPrivateKey()
	{
		ThrowIfDisposed();
		byte[] array = new byte[Algorithm.PrivateKeySizeInBytes];
		ExportMLDsaPrivateKeyCore(array);
		return array;
	}

	public void ExportMLDsaPrivateKey(Span<byte> destination)
	{
		Helpers.ThrowIfDestinationWrongLength(destination, Algorithm.PrivateKeySizeInBytes, "destination");
		ThrowIfDisposed();
		ExportMLDsaPrivateKeyCore(destination);
	}

	public byte[] ExportMLDsaPrivateSeed()
	{
		ThrowIfDisposed();
		byte[] array = new byte[Algorithm.PrivateSeedSizeInBytes];
		ExportMLDsaPrivateSeedCore(array);
		return array;
	}

	public void ExportMLDsaPrivateSeed(Span<byte> destination)
	{
		Helpers.ThrowIfDestinationWrongLength(destination, Algorithm.PrivateSeedSizeInBytes, "destination");
		ThrowIfDisposed();
		ExportMLDsaPrivateSeedCore(destination);
	}

	public static MLDsa GenerateKey(MLDsaAlgorithm algorithm)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		ThrowIfNotSupported();
		return MLDsaImplementation.GenerateKeyImpl(algorithm);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public unsafe static MLDsa ImportSubjectPublicKeyInfo(ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		fixed (byte* pointer = source)
		{
			using PointerMemoryManager<byte> pointerMemoryManager = new PointerMemoryManager<byte>(pointer, source.Length);
			AsnValueReader reader = new AsnValueReader(source, AsnEncodingRules.DER);
			SubjectPublicKeyInfoAsn.Decode(ref reader, pointerMemoryManager.Memory, out var decoded);
			MLDsaAlgorithm algorithmIdentifier = GetAlgorithmIdentifier(in decoded.Algorithm);
			if (decoded.SubjectPublicKey.Span.Length != algorithmIdentifier.PublicKeySizeInBytes)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
			return MLDsaImplementation.ImportPublicKey(algorithmIdentifier, decoded.SubjectPublicKey.Span);
		}
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLDsa ImportSubjectPublicKeyInfo(byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportSubjectPublicKeyInfo(new ReadOnlySpan<byte>(source));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLDsa ImportPkcs8PrivateKey(ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		KeyFormatHelper.ReadPkcs8(KnownOids, source, (KeyFormatHelper.KeyReader<MLDsa>)MLDsaKeyReader, out int _, out MLDsa ret);
		return ret;
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLDsa ImportPkcs8PrivateKey(byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportPkcs8PrivateKey(new ReadOnlySpan<byte>(source));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLDsa ImportEncryptedPkcs8PrivateKey(ReadOnlySpan<byte> passwordBytes, ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		int bytesRead;
		return KeyFormatHelper.DecryptPkcs8(passwordBytes, source, ImportPkcs8PrivateKey, out bytesRead);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLDsa ImportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		int bytesRead;
		return KeyFormatHelper.DecryptPkcs8(password, source, ImportPkcs8PrivateKey, out bytesRead);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLDsa ImportEncryptedPkcs8PrivateKey(string password, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportEncryptedPkcs8PrivateKey(password.AsSpan(), new ReadOnlySpan<byte>(source));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLDsa ImportFromPem(ReadOnlySpan<char> source)
	{
		ThrowIfNotSupported();
		return PemKeyHelpers.ImportFactoryPem(source, delegate(ReadOnlySpan<char> label)
		{
			if (label.SequenceEqual("PRIVATE KEY".AsSpan()))
			{
				return ImportPkcs8PrivateKey;
			}
			return label.SequenceEqual("PUBLIC KEY".AsSpan()) ? new PemKeyHelpers.ImportFactoryKeyAction<MLDsa>(ImportSubjectPublicKeyInfo) : null;
		});
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLDsa ImportFromPem(string source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ThrowIfNotSupported();
		return ImportFromPem(source.AsSpan());
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLDsa ImportFromEncryptedPem(ReadOnlySpan<char> source, ReadOnlySpan<char> password)
	{
		ThrowIfNotSupported();
		return PemKeyHelpers.ImportEncryptedFactoryPem(source, password, ImportEncryptedPkcs8PrivateKey);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLDsa ImportFromEncryptedPem(ReadOnlySpan<char> source, ReadOnlySpan<byte> passwordBytes)
	{
		ThrowIfNotSupported();
		return PemKeyHelpers.ImportEncryptedFactoryPem(source, passwordBytes, ImportEncryptedPkcs8PrivateKey);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLDsa ImportFromEncryptedPem(string source, string password)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(password, "password");
		ThrowIfNotSupported();
		return ImportFromEncryptedPem(source.AsSpan(), password.AsSpan());
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static MLDsa ImportFromEncryptedPem(string source, byte[] passwordBytes)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(passwordBytes, "passwordBytes");
		ThrowIfNotSupported();
		return ImportFromEncryptedPem(source.AsSpan(), new ReadOnlySpan<byte>(passwordBytes));
	}

	public static MLDsa ImportMLDsaPublicKey(MLDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		if (source.Length != algorithm.PublicKeySizeInBytes)
		{
			throw new ArgumentException(System.SR.Cryptography_KeyWrongSizeForAlgorithm, "source");
		}
		ThrowIfNotSupported();
		return MLDsaImplementation.ImportPublicKey(algorithm, source);
	}

	public static MLDsa ImportMLDsaPublicKey(MLDsaAlgorithm algorithm, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportMLDsaPublicKey(algorithm, new ReadOnlySpan<byte>(source));
	}

	public static MLDsa ImportMLDsaPrivateKey(MLDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		if (source.Length != algorithm.PrivateKeySizeInBytes)
		{
			throw new ArgumentException(System.SR.Argument_PrivateKeyWrongSizeForAlgorithm, "source");
		}
		ThrowIfNotSupported();
		return MLDsaImplementation.ImportPrivateKey(algorithm, source);
	}

	public static MLDsa ImportMLDsaPrivateKey(MLDsaAlgorithm algorithm, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportMLDsaPrivateKey(algorithm, new ReadOnlySpan<byte>(source));
	}

	public static MLDsa ImportMLDsaPrivateSeed(MLDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		if (source.Length != algorithm.PrivateSeedSizeInBytes)
		{
			throw new ArgumentException(System.SR.Cryptography_KeyWrongSizeForAlgorithm, "source");
		}
		ThrowIfNotSupported();
		return MLDsaImplementation.ImportSeed(algorithm, source);
	}

	public static MLDsa ImportMLDsaPrivateSeed(MLDsaAlgorithm algorithm, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportMLDsaPrivateSeed(algorithm, new ReadOnlySpan<byte>(source));
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	protected abstract void SignDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, Span<byte> destination);

	protected abstract bool VerifyDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, ReadOnlySpan<byte> signature);

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	protected abstract void SignPreHashCore(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> context, string hashAlgorithmOid, Span<byte> destination);

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	protected abstract bool VerifyPreHashCore(ReadOnlySpan<byte> hash, ReadOnlySpan<byte> context, string hashAlgorithmOid, ReadOnlySpan<byte> signature);

	protected abstract void ExportMLDsaPublicKeyCore(Span<byte> destination);

	protected abstract void ExportMLDsaPrivateKeyCore(Span<byte> destination);

	protected abstract void ExportMLDsaPrivateSeedCore(Span<byte> destination);

	private AsnWriter ExportSubjectPublicKeyInfoCore()
	{
		int publicKeySizeInBytes = Algorithm.PublicKeySizeInBytes;
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(publicKeySizeInBytes);
		try
		{
			Span<byte> span = array.AsSpan(0, publicKeySizeInBytes);
			ExportMLDsaPublicKeyCore(span);
			int initialCapacity = checked(32 + publicKeySizeInBytes);
			AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER, initialCapacity);
			using (asnWriter.PushSequence())
			{
				using (asnWriter.PushSequence())
				{
					asnWriter.WriteObjectIdentifier(Algorithm.Oid);
				}
				asnWriter.WriteBitString(span);
			}
			return asnWriter;
		}
		finally
		{
			System.Security.Cryptography.CryptoPool.Return(array, 0);
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

	private static void MLDsaKeyReader(ReadOnlyMemory<byte> privateKeyContents, in AlgorithmIdentifierAsn algorithmIdentifier, out MLDsa dsa)
	{
		MLDsaAlgorithm algorithmIdentifier2 = GetAlgorithmIdentifier(in algorithmIdentifier);
		MLDsaPrivateKeyAsn mLDsaPrivateKeyAsn = MLDsaPrivateKeyAsn.Decode(privateKeyContents, AsnEncodingRules.BER);
		ReadOnlyMemory<byte>? seed = mLDsaPrivateKeyAsn.Seed;
		if (seed.HasValue)
		{
			ReadOnlyMemory<byte> valueOrDefault = seed.GetValueOrDefault();
			if (valueOrDefault.Length != algorithmIdentifier2.PrivateSeedSizeInBytes)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
			dsa = ImportMLDsaPrivateSeed(algorithmIdentifier2, valueOrDefault.Span);
			return;
		}
		seed = mLDsaPrivateKeyAsn.ExpandedKey;
		if (seed.HasValue)
		{
			ReadOnlyMemory<byte> valueOrDefault2 = seed.GetValueOrDefault();
			if (valueOrDefault2.Length != algorithmIdentifier2.PrivateKeySizeInBytes)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
			dsa = MLDsaImplementation.ImportPrivateKey(algorithmIdentifier2, valueOrDefault2.Span);
			return;
		}
		MLDsaPrivateKeyBothAsn? both = mLDsaPrivateKeyAsn.Both;
		if (both.HasValue)
		{
			MLDsaPrivateKeyBothAsn valueOrDefault3 = both.GetValueOrDefault();
			int privateKeySizeInBytes = algorithmIdentifier2.PrivateKeySizeInBytes;
			if (valueOrDefault3.Seed.Length != algorithmIdentifier2.PrivateSeedSizeInBytes || valueOrDefault3.ExpandedKey.Length != privateKeySizeInBytes)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
			MLDsa mLDsa = ImportMLDsaPrivateSeed(algorithmIdentifier2, valueOrDefault3.Seed.Span);
			byte[] array = System.Security.Cryptography.CryptoPool.Rent(privateKeySizeInBytes);
			Span<byte> span = array.AsSpan(0, privateKeySizeInBytes);
			try
			{
				mLDsa.ExportMLDsaPrivateKey(span);
				if (CryptographicOperations.FixedTimeEquals(span, valueOrDefault3.ExpandedKey.Span))
				{
					dsa = mLDsa;
					return;
				}
				throw new CryptographicException(System.SR.Cryptography_MLDsaPkcs8KeyMismatch);
			}
			catch
			{
				mLDsa.Dispose();
				throw;
			}
			finally
			{
				System.Security.Cryptography.CryptoPool.Return(array, privateKeySizeInBytes);
			}
		}
		throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
	}

	private static MLDsaAlgorithm GetAlgorithmIdentifier(ref readonly AlgorithmIdentifierAsn identifier)
	{
		MLDsaAlgorithm result = MLDsaAlgorithm.GetMLDsaAlgorithmFromOid(identifier.Algorithm) ?? throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownAlgorithmIdentifier, identifier.Algorithm));
		if (identifier.Parameters.HasValue)
		{
			AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
			identifier.Encode(asnWriter);
			throw Helpers.CreateAlgorithmUnknownException(asnWriter);
		}
		return result;
	}

	internal static void ThrowIfNotSupported()
	{
		if (!IsSupported)
		{
			throw new PlatformNotSupportedException(System.SR.Format(System.SR.Cryptography_AlgorithmNotSupported, "MLDsa"));
		}
	}

	private protected string MapHashOidToAlgorithm(string hashOid, out int hashLengthInBytes, out bool insufficientCollisionResistance)
	{
		int num;
		string result;
		if (hashOid != null)
		{
			int length = hashOid.Length;
			if (length <= 18)
			{
				switch (length)
				{
				case 18:
					if (!(hashOid == "1.2.840.113549.2.5"))
					{
						break;
					}
					hashLengthInBytes = 16;
					insufficientCollisionResistance = true;
					return "MD5";
				case 13:
					if (!(hashOid == "1.3.14.3.2.26"))
					{
						break;
					}
					hashLengthInBytes = 20;
					insufficientCollisionResistance = true;
					return "SHA1";
				}
			}
			else if (length != 22)
			{
				if (length == 23)
				{
					switch (hashOid[22])
					{
					case '0':
						break;
					case '1':
						goto IL_0139;
					case '2':
						goto IL_014e;
					default:
						goto IL_0210;
					}
					if (hashOid == "2.16.840.1.101.3.4.2.10")
					{
						hashLengthInBytes = 64;
						num = 256;
						result = "SHA3-512";
						goto IL_0218;
					}
				}
			}
			else
			{
				switch (hashOid[21])
				{
				case '1':
					break;
				case '8':
					goto IL_00d0;
				case '2':
					goto IL_00e5;
				case '9':
					goto IL_00fa;
				case '3':
					goto IL_010f;
				default:
					goto IL_0210;
				}
				if (hashOid == "2.16.840.1.101.3.4.2.1")
				{
					hashLengthInBytes = 32;
					num = 128;
					result = "SHA256";
					goto IL_0218;
				}
			}
		}
		goto IL_0210;
		IL_00fa:
		if (!(hashOid == "2.16.840.1.101.3.4.2.9"))
		{
			goto IL_0210;
		}
		hashLengthInBytes = 48;
		num = 192;
		result = "SHA3-384";
		goto IL_0218;
		IL_0210:
		hashLengthInBytes = 0;
		insufficientCollisionResistance = false;
		return null;
		IL_0218:
		insufficientCollisionResistance = num < Algorithm.LambdaCollisionStrength;
		return result;
		IL_0139:
		if (!(hashOid == "2.16.840.1.101.3.4.2.11"))
		{
			goto IL_0210;
		}
		hashLengthInBytes = 32;
		num = 128;
		result = "SHAKE128";
		goto IL_0218;
		IL_014e:
		if (!(hashOid == "2.16.840.1.101.3.4.2.12"))
		{
			goto IL_0210;
		}
		hashLengthInBytes = 64;
		num = 256;
		result = "SHAKE256";
		goto IL_0218;
		IL_00e5:
		if (!(hashOid == "2.16.840.1.101.3.4.2.2"))
		{
			goto IL_0210;
		}
		hashLengthInBytes = 48;
		num = 192;
		result = "SHA384";
		goto IL_0218;
		IL_00d0:
		if (!(hashOid == "2.16.840.1.101.3.4.2.8"))
		{
			goto IL_0210;
		}
		hashLengthInBytes = 32;
		num = 128;
		result = "SHA3-256";
		goto IL_0218;
		IL_010f:
		if (!(hashOid == "2.16.840.1.101.3.4.2.3"))
		{
			goto IL_0210;
		}
		hashLengthInBytes = 64;
		num = 256;
		result = "SHA512";
		goto IL_0218;
	}
}
