using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Security.Cryptography.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography;

[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
public abstract class CompositeMLDsa : IDisposable
{
	private delegate TResult ProcessExportedContent<TResult>(ReadOnlySpan<byte> exportedContent);

	private static readonly string[] s_knownOids = new string[18]
	{
		"1.3.6.1.5.5.7.6.37", "1.3.6.1.5.5.7.6.38", "1.3.6.1.5.5.7.6.39", "1.3.6.1.5.5.7.6.40", "1.3.6.1.5.5.7.6.41", "1.3.6.1.5.5.7.6.42", "1.3.6.1.5.5.7.6.43", "1.3.6.1.5.5.7.6.44", "1.3.6.1.5.5.7.6.45", "1.3.6.1.5.5.7.6.46",
		"1.3.6.1.5.5.7.6.47", "1.3.6.1.5.5.7.6.48", "1.3.6.1.5.5.7.6.49", "1.3.6.1.5.5.7.6.50", "1.3.6.1.5.5.7.6.51", "1.3.6.1.5.5.7.6.52", "1.3.6.1.5.5.7.6.53", "1.3.6.1.5.5.7.6.54"
	};

	private bool _disposed;

	public static bool IsSupported { get; } = CompositeMLDsaImplementation.SupportsAny();

	public CompositeMLDsaAlgorithm Algorithm { get; }

	protected CompositeMLDsa(CompositeMLDsaAlgorithm algorithm)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		Algorithm = algorithm;
	}

	public static bool IsAlgorithmSupported(CompositeMLDsaAlgorithm algorithm)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		return CompositeMLDsaImplementation.IsAlgorithmSupportedImpl(algorithm);
	}

	public byte[] SignData(byte[] data, byte[]? context = null)
	{
		ArgumentNullException.ThrowIfNull(data, "data");
		if (context != null && context.Length > 255)
		{
			throw new ArgumentOutOfRangeException("context", context.Length, System.SR.Argument_SignatureContextTooLong255);
		}
		ThrowIfDisposed();
		if (Algorithm.MinSignatureSizeInBytes == Algorithm.MaxSignatureSizeInBytes)
		{
			byte[] array = new byte[Algorithm.MaxSignatureSizeInBytes];
			int num = SignDataCore(new ReadOnlySpan<byte>(data), new ReadOnlySpan<byte>(context), array);
			if (array.Length != num)
			{
				throw new CryptographicException();
			}
			return array;
		}
		using CryptoPoolLease cryptoPoolLease = CryptoPoolLease.Rent(Algorithm.MaxSignatureSizeInBytes, skipClear: true);
		int num2 = SignDataCore(new ReadOnlySpan<byte>(data), new ReadOnlySpan<byte>(context), cryptoPoolLease.Span);
		if (!Algorithm.IsValidSignatureSize(num2))
		{
			throw new CryptographicException();
		}
		return cryptoPoolLease.Span.Slice(0, num2).ToArray();
	}

	public int SignData(ReadOnlySpan<byte> data, Span<byte> destination, ReadOnlySpan<byte> context = default(ReadOnlySpan<byte>))
	{
		if (context.Length > 255)
		{
			throw new ArgumentOutOfRangeException("context", context.Length, System.SR.Argument_SignatureContextTooLong255);
		}
		if (destination.Length < Algorithm.MaxSignatureSizeInBytes)
		{
			throw new ArgumentException(System.SR.Argument_DestinationTooShort, "destination");
		}
		ThrowIfDisposed();
		int num = SignDataCore(data, context, destination.Slice(0, Algorithm.MaxSignatureSizeInBytes));
		if (!Algorithm.IsValidSignatureSize(num))
		{
			CryptographicOperations.ZeroMemory(destination);
			throw new CryptographicException();
		}
		return num;
	}

	protected abstract int SignDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, Span<byte> destination);

	public bool VerifyData(byte[] data, byte[] signature, byte[]? context = null)
	{
		ArgumentNullException.ThrowIfNull(data, "data");
		ArgumentNullException.ThrowIfNull(signature, "signature");
		return VerifyData(new ReadOnlySpan<byte>(data), new ReadOnlySpan<byte>(signature), new ReadOnlySpan<byte>(context));
	}

	public bool VerifyData(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature, ReadOnlySpan<byte> context = default(ReadOnlySpan<byte>))
	{
		if (context.Length > 255)
		{
			throw new ArgumentOutOfRangeException("context", context.Length, System.SR.Argument_SignatureContextTooLong255);
		}
		ThrowIfDisposed();
		if (!Algorithm.IsValidSignatureSize(signature.Length))
		{
			return false;
		}
		return VerifyDataCore(data, context, signature);
	}

	protected abstract bool VerifyDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, ReadOnlySpan<byte> signature);

	public static CompositeMLDsa GenerateKey(CompositeMLDsaAlgorithm algorithm)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		ThrowIfNotSupported(algorithm);
		return CompositeMLDsaImplementation.GenerateKeyImpl(algorithm);
	}

	public static CompositeMLDsa ImportFromEncryptedPem(string source, string password)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(password, "password");
		ThrowIfNotSupported();
		return ImportFromEncryptedPem(source.AsSpan(), password.AsSpan());
	}

	public static CompositeMLDsa ImportFromEncryptedPem(ReadOnlySpan<char> source, ReadOnlySpan<char> password)
	{
		ThrowIfNotSupported();
		return PemKeyHelpers.ImportEncryptedFactoryPem(source, password, ImportEncryptedPkcs8PrivateKey);
	}

	public static CompositeMLDsa ImportFromEncryptedPem(string source, byte[] passwordBytes)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ArgumentNullException.ThrowIfNull(passwordBytes, "passwordBytes");
		ThrowIfNotSupported();
		return ImportFromEncryptedPem(source.AsSpan(), new ReadOnlySpan<byte>(passwordBytes));
	}

	public static CompositeMLDsa ImportFromEncryptedPem(ReadOnlySpan<char> source, ReadOnlySpan<byte> passwordBytes)
	{
		ThrowIfNotSupported();
		return PemKeyHelpers.ImportEncryptedFactoryPem(source, passwordBytes, ImportEncryptedPkcs8PrivateKey);
	}

	public static CompositeMLDsa ImportFromPem(string source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ThrowIfNotSupported();
		return ImportFromPem(source.AsSpan());
	}

	public static CompositeMLDsa ImportFromPem(ReadOnlySpan<char> source)
	{
		ThrowIfNotSupported();
		return PemKeyHelpers.ImportFactoryPem(source, delegate(ReadOnlySpan<char> label)
		{
			if (label.SequenceEqual("PRIVATE KEY".AsSpan()))
			{
				return ImportPkcs8PrivateKey;
			}
			return label.SequenceEqual("PUBLIC KEY".AsSpan()) ? new PemKeyHelpers.ImportFactoryKeyAction<CompositeMLDsa>(ImportSubjectPublicKeyInfo) : null;
		});
	}

	public static CompositeMLDsa ImportSubjectPublicKeyInfo(byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportSubjectPublicKeyInfo(new ReadOnlySpan<byte>(source));
	}

	public static CompositeMLDsa ImportSubjectPublicKeyInfo(ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		KeyFormatHelper.ReadSubjectPublicKeyInfo(s_knownOids, source, (KeyFormatHelper.KeyReader<CompositeMLDsa>)SubjectPublicKeyReader, out int _, out CompositeMLDsa ret);
		return ret;
		static void SubjectPublicKeyReader(ReadOnlyMemory<byte> key, in AlgorithmIdentifierAsn identifier, out CompositeMLDsa dsa)
		{
			CompositeMLDsaAlgorithm algorithmIdentifier = GetAlgorithmIdentifier(in identifier);
			if (!algorithmIdentifier.IsValidPublicKeySize(key.Length))
			{
				throw new CryptographicException(System.SR.Argument_PublicKeyWrongSizeForAlgorithm);
			}
			dsa = CompositeMLDsaImplementation.ImportCompositeMLDsaPublicKeyImpl(algorithmIdentifier, key.Span);
		}
	}

	public static CompositeMLDsa ImportEncryptedPkcs8PrivateKey(string password, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportEncryptedPkcs8PrivateKey(password.AsSpan(), new ReadOnlySpan<byte>(source));
	}

	public static CompositeMLDsa ImportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		int bytesRead;
		return KeyFormatHelper.DecryptPkcs8(password, source, ImportPkcs8PrivateKey, out bytesRead);
	}

	public static CompositeMLDsa ImportEncryptedPkcs8PrivateKey(ReadOnlySpan<byte> passwordBytes, ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		int bytesRead;
		return KeyFormatHelper.DecryptPkcs8(passwordBytes, source, ImportPkcs8PrivateKey, out bytesRead);
	}

	public static CompositeMLDsa ImportPkcs8PrivateKey(byte[] source)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportPkcs8PrivateKey(new ReadOnlySpan<byte>(source));
	}

	public static CompositeMLDsa ImportPkcs8PrivateKey(ReadOnlySpan<byte> source)
	{
		Helpers.ThrowIfAsnInvalidLength(source);
		ThrowIfNotSupported();
		KeyFormatHelper.ReadPkcs8(s_knownOids, source, (KeyFormatHelper.KeyReader<CompositeMLDsa>)PrivateKeyReader, out int _, out CompositeMLDsa ret);
		return ret;
		static void PrivateKeyReader(ReadOnlyMemory<byte> privateKeyContents, in AlgorithmIdentifierAsn algorithmIdentifier, out CompositeMLDsa dsa)
		{
			CompositeMLDsaAlgorithm algorithmIdentifier2 = GetAlgorithmIdentifier(in algorithmIdentifier);
			if (!algorithmIdentifier2.IsValidPrivateKeySize(privateKeyContents.Length))
			{
				throw new CryptographicException(System.SR.Argument_PrivateKeyWrongSizeForAlgorithm);
			}
			dsa = CompositeMLDsaImplementation.ImportCompositeMLDsaPrivateKeyImpl(algorithmIdentifier2, privateKeyContents.Span);
		}
	}

	public static CompositeMLDsa ImportCompositeMLDsaPublicKey(CompositeMLDsaAlgorithm algorithm, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportCompositeMLDsaPublicKey(algorithm, new ReadOnlySpan<byte>(source));
	}

	public static CompositeMLDsa ImportCompositeMLDsaPublicKey(CompositeMLDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		ThrowIfNotSupported(algorithm);
		if (!algorithm.IsValidPublicKeySize(source.Length))
		{
			throw new CryptographicException(System.SR.Argument_PublicKeyWrongSizeForAlgorithm);
		}
		return CompositeMLDsaImplementation.ImportCompositeMLDsaPublicKeyImpl(algorithm, source);
	}

	public static CompositeMLDsa ImportCompositeMLDsaPrivateKey(CompositeMLDsaAlgorithm algorithm, byte[] source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		ArgumentNullException.ThrowIfNull(source, "source");
		return ImportCompositeMLDsaPrivateKey(algorithm, new ReadOnlySpan<byte>(source));
	}

	public static CompositeMLDsa ImportCompositeMLDsaPrivateKey(CompositeMLDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		ArgumentNullException.ThrowIfNull(algorithm, "algorithm");
		ThrowIfNotSupported(algorithm);
		if (!algorithm.IsValidPrivateKeySize(source.Length))
		{
			throw new CryptographicException(System.SR.Argument_PrivateKeyWrongSizeForAlgorithm);
		}
		return CompositeMLDsaImplementation.ImportCompositeMLDsaPrivateKeyImpl(algorithm, source);
	}

	public string ExportEncryptedPkcs8PrivateKeyPem(string password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		return ExportEncryptedPkcs8PrivateKeyPem(password.AsSpan(), pbeParameters);
	}

	public string ExportEncryptedPkcs8PrivateKeyPem(ReadOnlySpan<char> password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, password, ReadOnlySpan<byte>.Empty);
		ThrowIfDisposed();
		AsnWriter writer = WriteEncryptedPkcs8PrivateKeyToAsnWriter(password, pbeParameters);
		return Helpers.EncodeAsnWriterToPem("ENCRYPTED PRIVATE KEY", writer, clear: false);
	}

	public string ExportEncryptedPkcs8PrivateKeyPem(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, ReadOnlySpan<char>.Empty, passwordBytes);
		ThrowIfDisposed();
		AsnWriter writer = WriteEncryptedPkcs8PrivateKeyToAsnWriter(passwordBytes, pbeParameters);
		return Helpers.EncodeAsnWriterToPem("ENCRYPTED PRIVATE KEY", writer, clear: false);
	}

	public byte[] ExportEncryptedPkcs8PrivateKey(string password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(password, "password");
		return ExportEncryptedPkcs8PrivateKey(password.AsSpan(), pbeParameters);
	}

	public byte[] ExportEncryptedPkcs8PrivateKey(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, ReadOnlySpan<char>.Empty, passwordBytes);
		ThrowIfDisposed();
		AsnWriter asnWriter = WriteEncryptedPkcs8PrivateKeyToAsnWriter(passwordBytes, pbeParameters);
		try
		{
			return asnWriter.Encode();
		}
		finally
		{
			asnWriter.Reset();
		}
	}

	public byte[] ExportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, password, ReadOnlySpan<byte>.Empty);
		ThrowIfDisposed();
		AsnWriter asnWriter = WriteEncryptedPkcs8PrivateKeyToAsnWriter(password, pbeParameters);
		try
		{
			return asnWriter.Encode();
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

	public bool TryExportEncryptedPkcs8PrivateKey(ReadOnlySpan<char> password, PbeParameters pbeParameters, Span<byte> destination, out int bytesWritten)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, password, ReadOnlySpan<byte>.Empty);
		ThrowIfDisposed();
		AsnWriter asnWriter = WriteEncryptedPkcs8PrivateKeyToAsnWriter(password, pbeParameters);
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
		AsnWriter asnWriter = WriteEncryptedPkcs8PrivateKeyToAsnWriter(passwordBytes, pbeParameters);
		try
		{
			return asnWriter.TryEncode(destination, out bytesWritten);
		}
		finally
		{
			asnWriter.Reset();
		}
	}

	public string ExportPkcs8PrivateKeyPem()
	{
		ThrowIfDisposed();
		return ExportPkcs8PrivateKeyCallback((ReadOnlySpan<byte> pkcs8) => PemEncoding.WriteString("PRIVATE KEY".AsSpan(), pkcs8));
	}

	public byte[] ExportPkcs8PrivateKey()
	{
		ThrowIfDisposed();
		return ExportPkcs8PrivateKeyCallback((ReadOnlySpan<byte> pkcs8) => pkcs8.ToArray());
	}

	public bool TryExportPkcs8PrivateKey(Span<byte> destination, out int bytesWritten)
	{
		ThrowIfDisposed();
		int minPrivateKeySizeInBytes = Algorithm.MinPrivateKeySizeInBytes;
		if (destination.Length < minPrivateKeySizeInBytes)
		{
			bytesWritten = 0;
			return false;
		}
		return TryExportPkcs8PrivateKeyCore(destination, out bytesWritten);
	}

	protected abstract bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten);

	public string ExportSubjectPublicKeyInfoPem()
	{
		ThrowIfDisposed();
		AsnWriter writer = WriteSubjectPublicKeyToAsnWriter();
		return Helpers.EncodeAsnWriterToPem("PUBLIC KEY", writer, clear: false);
	}

	public byte[] ExportSubjectPublicKeyInfo()
	{
		ThrowIfDisposed();
		return WriteSubjectPublicKeyToAsnWriter().Encode();
	}

	public bool TryExportSubjectPublicKeyInfo(Span<byte> destination, out int bytesWritten)
	{
		ThrowIfDisposed();
		return WriteSubjectPublicKeyToAsnWriter().TryEncode(destination, out bytesWritten);
	}

	public byte[] ExportCompositeMLDsaPublicKey()
	{
		ThrowIfDisposed();
		byte[] array = new byte[Algorithm.MaxPublicKeySizeInBytes];
		if (!TryExportCompositeMLDsaPublicKey(array, out var bytesWritten))
		{
			throw new CryptographicException();
		}
		if (bytesWritten < array.Length)
		{
			Array.Resize(ref array, bytesWritten);
		}
		return array;
	}

	public int ExportCompositeMLDsaPublicKey(Span<byte> destination)
	{
		ThrowIfDisposed();
		if (destination.Length < Algorithm.MinPublicKeySizeInBytes)
		{
			throw new CryptographicException(System.SR.Argument_DestinationTooShort);
		}
		if (!TryExportCompositeMLDsaPublicKey(destination, out var bytesWritten))
		{
			throw new CryptographicException(System.SR.Argument_DestinationTooShort);
		}
		return bytesWritten;
	}

	public bool TryExportCompositeMLDsaPublicKey(Span<byte> destination, out int bytesWritten)
	{
		ThrowIfDisposed();
		if (destination.Length < Algorithm.MinPublicKeySizeInBytes)
		{
			bytesWritten = 0;
			return false;
		}
		using CryptoPoolLease cryptoPoolLease = CryptoPoolLease.RentConditionally(Algorithm.MaxPublicKeySizeInBytes, destination, skipClear: true);
		int num = ExportCompositeMLDsaPublicKeyCore(cryptoPoolLease.Span);
		if (!Algorithm.IsValidPublicKeySize(num))
		{
			bytesWritten = 0;
			throw new CryptographicException();
		}
		if (cryptoPoolLease.IsRented)
		{
			if (num > destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			cryptoPoolLease.Span.Slice(0, num).CopyTo(destination);
		}
		bytesWritten = num;
		return true;
	}

	public byte[] ExportCompositeMLDsaPrivateKey()
	{
		ThrowIfDisposed();
		byte[] array = new byte[Algorithm.MaxPrivateKeySizeInBytes];
		if (!TryExportCompositeMLDsaPrivateKey(array, out var bytesWritten))
		{
			throw new CryptographicException();
		}
		if (bytesWritten < array.Length)
		{
			byte[] array2 = new byte[bytesWritten];
			Array.Copy(array, array2, bytesWritten);
			CryptographicOperations.ZeroMemory(array);
			array = array2;
		}
		return array;
	}

	public int ExportCompositeMLDsaPrivateKey(Span<byte> destination)
	{
		ThrowIfDisposed();
		if (destination.Length < Algorithm.MinPrivateKeySizeInBytes)
		{
			throw new CryptographicException(System.SR.Argument_DestinationTooShort);
		}
		if (!TryExportCompositeMLDsaPrivateKey(destination, out var bytesWritten))
		{
			throw new CryptographicException(System.SR.Argument_DestinationTooShort);
		}
		return bytesWritten;
	}

	public bool TryExportCompositeMLDsaPrivateKey(Span<byte> destination, out int bytesWritten)
	{
		ThrowIfDisposed();
		if (destination.Length < Algorithm.MinPrivateKeySizeInBytes)
		{
			bytesWritten = 0;
			return false;
		}
		using CryptoPoolLease cryptoPoolLease = CryptoPoolLease.RentConditionally(Algorithm.MaxPrivateKeySizeInBytes, destination, skipClear: false, skipClearIfNotRented: true);
		int num = ExportCompositeMLDsaPrivateKeyCore(cryptoPoolLease.Span);
		if (!Algorithm.IsValidPrivateKeySize(num))
		{
			if (!cryptoPoolLease.IsRented)
			{
				CryptographicOperations.ZeroMemory(destination);
			}
			bytesWritten = 0;
			throw new CryptographicException();
		}
		if (cryptoPoolLease.IsRented)
		{
			if (num > destination.Length)
			{
				bytesWritten = 0;
				return false;
			}
			cryptoPoolLease.Span.Slice(0, num).CopyTo(destination);
		}
		bytesWritten = num;
		return true;
	}

	protected abstract int ExportCompositeMLDsaPublicKeyCore(Span<byte> destination);

	protected abstract int ExportCompositeMLDsaPrivateKeyCore(Span<byte> destination);

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

	private AsnWriter WriteEncryptedPkcs8PrivateKeyToAsnWriter(ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
	{
		AsnWriter asnWriter = null;
		try
		{
			asnWriter = WritePkcs8ToAsnWriter();
			return KeyFormatHelper.WriteEncryptedPkcs8(passwordBytes, asnWriter, pbeParameters);
		}
		finally
		{
			asnWriter?.Reset();
		}
	}

	private AsnWriter WriteEncryptedPkcs8PrivateKeyToAsnWriter(ReadOnlySpan<char> password, PbeParameters pbeParameters)
	{
		AsnWriter asnWriter = null;
		try
		{
			asnWriter = WritePkcs8ToAsnWriter();
			return KeyFormatHelper.WriteEncryptedPkcs8(password, asnWriter, pbeParameters);
		}
		finally
		{
			asnWriter?.Reset();
		}
	}

	private AsnWriter WritePkcs8ToAsnWriter()
	{
		return ExportPkcs8PrivateKeyCallback(delegate(ReadOnlySpan<byte> pkcs8)
		{
			AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.BER, pkcs8.Length);
			try
			{
				asnWriter.WriteEncodedValueForCrypto(pkcs8);
				return asnWriter;
			}
			catch
			{
				asnWriter.Reset();
				throw;
			}
		});
	}

	private AsnWriter WriteSubjectPublicKeyToAsnWriter()
	{
		byte[] array = new byte[Algorithm.MaxPublicKeySizeInBytes];
		int num = ExportCompositeMLDsaPublicKeyCore(array);
		if (!Algorithm.IsValidPublicKeySize(num))
		{
			throw new CryptographicException();
		}
		ReadOnlySpan<byte> value = array.AsSpan(0, num);
		int initialCapacity = checked(32 + value.Length);
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER, initialCapacity);
		using (asnWriter.PushSequence())
		{
			using (asnWriter.PushSequence())
			{
				asnWriter.WriteObjectIdentifier(Algorithm.Oid);
			}
			asnWriter.WriteBitString(value);
			return asnWriter;
		}
	}

	private TResult ExportPkcs8PrivateKeyCallback<TResult>(ProcessExportedContent<TResult> func)
	{
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(Algorithm.MaxPrivateKeySizeInBytes);
		int bytesWritten;
		while (!TryExportPkcs8PrivateKeyCore(array, out bytesWritten))
		{
			int num = array.Length;
			System.Security.Cryptography.CryptoPool.Return(array);
			array = System.Security.Cryptography.CryptoPool.Rent(checked(num * 2));
		}
		if ((uint)bytesWritten > (uint)array.Length)
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

	private static CompositeMLDsaAlgorithm GetAlgorithmIdentifier(ref readonly AlgorithmIdentifierAsn identifier)
	{
		CompositeMLDsaAlgorithm algorithmFromOid = CompositeMLDsaAlgorithm.GetAlgorithmFromOid(identifier.Algorithm);
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
			throw new PlatformNotSupportedException(System.SR.Format(System.SR.Cryptography_AlgorithmNotSupported, "CompositeMLDsa"));
		}
	}

	private static void ThrowIfNotSupported(CompositeMLDsaAlgorithm algorithm)
	{
		if (!IsSupported || !IsAlgorithmSupported(algorithm))
		{
			throw new PlatformNotSupportedException(System.SR.Format(System.SR.Cryptography_AlgorithmNotSupported, "CompositeMLDsa"));
		}
	}

	private void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(_disposed, typeof(CompositeMLDsa));
	}
}
