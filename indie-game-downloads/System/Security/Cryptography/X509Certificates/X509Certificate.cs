using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Text;
using Internal.Cryptography;
using Microsoft.Win32.SafeHandles;

namespace System.Security.Cryptography.X509Certificates;

public class X509Certificate : IDisposable, IDeserializationCallback, ISerializable
{
	private volatile byte[] _lazyCertHash;

	private volatile string _lazyIssuer;

	private volatile string _lazySubject;

	private volatile byte[] _lazySerialNumber;

	private volatile string _lazyKeyAlgorithm;

	private volatile byte[] _lazyKeyAlgorithmParameters;

	private volatile byte[] _lazyPublicKey;

	private volatile byte[] _lazyRawData;

	private volatile bool _lazyKeyAlgorithmParametersCreated;

	private DateTime _lazyNotBefore = DateTime.MinValue;

	private DateTime _lazyNotAfter = DateTime.MinValue;

	private static Pkcs12LoaderLimits s_legacyLimits;

	private protected ReadOnlyMemory<byte> PalRawDataMemory
	{
		get
		{
			ThrowIfInvalid();
			return _lazyRawData ?? (_lazyRawData = Pal.RawData);
		}
	}

	public nint Handle
	{
		get
		{
			if (Pal != null)
			{
				return Pal.Handle;
			}
			return IntPtr.Zero;
		}
	}

	public string Issuer
	{
		get
		{
			ThrowIfInvalid();
			return _lazyIssuer ?? (_lazyIssuer = Pal.Issuer);
		}
	}

	public string Subject
	{
		get
		{
			ThrowIfInvalid();
			return _lazySubject ?? (_lazySubject = Pal.Subject);
		}
	}

	public ReadOnlyMemory<byte> SerialNumberBytes
	{
		get
		{
			ThrowIfInvalid();
			return GetRawSerialNumber();
		}
	}

	internal ICertificatePalCore? Pal { get; private set; }

	public virtual void Reset()
	{
		_lazyCertHash = null;
		_lazyIssuer = null;
		_lazySubject = null;
		_lazySerialNumber = null;
		_lazyKeyAlgorithm = null;
		_lazyKeyAlgorithmParameters = null;
		_lazyPublicKey = null;
		_lazyRawData = null;
		_lazyNotBefore = DateTime.MinValue;
		_lazyNotAfter = DateTime.MinValue;
		_lazyKeyAlgorithmParametersCreated = false;
		ICertificatePalCore pal = Pal;
		if (pal != null)
		{
			Pal = null;
			pal.Dispose();
		}
	}

	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[UnsupportedOSPlatform("browser")]
	public X509Certificate()
	{
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate(byte[] data)
		: this(new ReadOnlySpan<byte>(data))
	{
	}

	private protected X509Certificate(ReadOnlySpan<byte> data)
	{
		if (!data.IsEmpty)
		{
			Pal = CertificatePal.FromBlob(data, SafePasswordHandle.InvalidHandle, X509KeyStorageFlags.DefaultKeySet);
		}
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate(byte[] rawData, string? password)
		: this(rawData, password, X509KeyStorageFlags.DefaultKeySet)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[CLSCompliant(false)]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate(byte[] rawData, SecureString? password)
		: this(rawData, password, X509KeyStorageFlags.DefaultKeySet)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate(byte[] rawData, string? password, X509KeyStorageFlags keyStorageFlags)
	{
		if (rawData == null || rawData.Length == 0)
		{
			throw new ArgumentException(System.SR.Arg_EmptyOrNullArray, "rawData");
		}
		ValidateKeyStorageFlags(keyStorageFlags);
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		Pal = CertificatePal.FromBlob(rawData, password2, keyStorageFlags);
	}

	[UnsupportedOSPlatform("browser")]
	[CLSCompliant(false)]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate(byte[] rawData, SecureString? password, X509KeyStorageFlags keyStorageFlags)
	{
		if (rawData == null || rawData.Length == 0)
		{
			throw new ArgumentException(System.SR.Arg_EmptyOrNullArray, "rawData");
		}
		ValidateKeyStorageFlags(keyStorageFlags);
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		Pal = CertificatePal.FromBlob(rawData, password2, keyStorageFlags);
	}

	private protected X509Certificate(ReadOnlySpan<byte> rawData, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags)
	{
		if (rawData.IsEmpty)
		{
			throw new ArgumentException(System.SR.Arg_EmptyOrNullArray, "rawData");
		}
		ValidateKeyStorageFlags(keyStorageFlags);
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		Pal = CertificatePal.FromBlob(rawData, password2, keyStorageFlags);
	}

	[UnsupportedOSPlatform("browser")]
	public X509Certificate(nint handle)
	{
		Pal = CertificatePal.FromHandle(handle);
	}

	internal X509Certificate(ICertificatePalCore pal)
	{
		Pal = pal;
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate(string fileName)
		: this(fileName, (string?)null, X509KeyStorageFlags.DefaultKeySet)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate(string fileName, string? password)
		: this(fileName, password, X509KeyStorageFlags.DefaultKeySet)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[CLSCompliant(false)]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate(string fileName, SecureString? password)
		: this(fileName, password, X509KeyStorageFlags.DefaultKeySet)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate(string fileName, string? password, X509KeyStorageFlags keyStorageFlags)
	{
		ArgumentNullException.ThrowIfNull(fileName, "fileName");
		ValidateKeyStorageFlags(keyStorageFlags);
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		Pal = CertificatePal.FromFile(fileName, password2, keyStorageFlags);
	}

	private protected X509Certificate(string fileName, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags)
	{
		ArgumentNullException.ThrowIfNull(fileName, "fileName");
		ValidateKeyStorageFlags(keyStorageFlags);
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		Pal = CertificatePal.FromFile(fileName, password2, keyStorageFlags);
	}

	[UnsupportedOSPlatform("browser")]
	[CLSCompliant(false)]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate(string fileName, SecureString? password, X509KeyStorageFlags keyStorageFlags)
		: this()
	{
		ArgumentNullException.ThrowIfNull(fileName, "fileName");
		ValidateKeyStorageFlags(keyStorageFlags);
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		Pal = CertificatePal.FromFile(fileName, password2, keyStorageFlags);
	}

	[UnsupportedOSPlatform("browser")]
	public X509Certificate(X509Certificate cert)
	{
		ArgumentNullException.ThrowIfNull(cert, "cert");
		if (cert.Pal != null)
		{
			Pal = CertificatePal.FromOtherCert(cert);
		}
	}

	[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public X509Certificate(SerializationInfo info, StreamingContext context)
		: this()
	{
		throw new PlatformNotSupportedException();
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static X509Certificate CreateFromCertFile(string filename)
	{
		return new X509Certificate(filename);
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public static X509Certificate CreateFromSignedFile(string filename)
	{
		return new X509Certificate(filename);
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		throw new PlatformNotSupportedException();
	}

	void IDeserializationCallback.OnDeserialization(object sender)
	{
		throw new PlatformNotSupportedException();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			Reset();
		}
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is X509Certificate other)
		{
			return Equals(other);
		}
		return false;
	}

	public virtual bool Equals([NotNullWhen(true)] X509Certificate? other)
	{
		if (other == null)
		{
			return false;
		}
		if (Pal == null)
		{
			return other.Pal == null;
		}
		if (!Issuer.Equals(other.Issuer))
		{
			return false;
		}
		ReadOnlySpan<byte> span = GetRawSerialNumber();
		ReadOnlySpan<byte> other2 = other.GetRawSerialNumber();
		return span.SequenceEqual(other2);
	}

	public virtual byte[] Export(X509ContentType contentType)
	{
		return Export(contentType, (string?)null);
	}

	public virtual byte[] Export(X509ContentType contentType, string? password)
	{
		VerifyContentType(contentType);
		if (Pal == null)
		{
			throw new CryptographicException(-2147467261);
		}
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		return Pal.Export(contentType, password2);
	}

	[CLSCompliant(false)]
	public virtual byte[] Export(X509ContentType contentType, SecureString? password)
	{
		VerifyContentType(contentType);
		if (Pal == null)
		{
			throw new CryptographicException(-2147467261);
		}
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		return Pal.Export(contentType, password2);
	}

	public byte[] ExportPkcs12(Pkcs12ExportPbeParameters exportParameters, string? password)
	{
		Helpers.ThrowIfInvalidPkcs12ExportParameters(exportParameters);
		Helpers.ThrowIfPasswordContainsNullCharacter(password);
		if (Pal == null)
		{
			throw new CryptographicException(-2147467261);
		}
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		return Pal.ExportPkcs12(exportParameters, password2);
	}

	public byte[] ExportPkcs12(PbeParameters exportParameters, string? password)
	{
		ArgumentNullException.ThrowIfNull(exportParameters, "exportParameters");
		Helpers.ThrowIfInvalidPkcs12ExportParameters(exportParameters);
		Helpers.ThrowIfPasswordContainsNullCharacter(password);
		if (Pal == null)
		{
			throw new CryptographicException(-2147467261);
		}
		using SafePasswordHandle password2 = new SafePasswordHandle(password, passwordProvided: true);
		return Pal.ExportPkcs12(exportParameters, password2);
	}

	public virtual string GetRawCertDataString()
	{
		ThrowIfInvalid();
		return GetRawCertData().ToHexStringUpper();
	}

	public virtual byte[] GetCertHash()
	{
		ThrowIfInvalid();
		return GetRawCertHash().CloneByteArray();
	}

	public virtual byte[] GetCertHash(HashAlgorithmName hashAlgorithm)
	{
		ThrowIfInvalid();
		return CryptographicOperations.HashData(hashAlgorithm, PalRawDataMemory.Span);
	}

	public virtual bool TryGetCertHash(HashAlgorithmName hashAlgorithm, Span<byte> destination, out int bytesWritten)
	{
		ThrowIfInvalid();
		return CryptographicOperations.TryHashData(hashAlgorithm, PalRawDataMemory.Span, destination, out bytesWritten);
	}

	public virtual string GetCertHashString()
	{
		ThrowIfInvalid();
		return GetRawCertHash().ToHexStringUpper();
	}

	public virtual string GetCertHashString(HashAlgorithmName hashAlgorithm)
	{
		ThrowIfInvalid();
		return GetCertHashString(hashAlgorithm, PalRawDataMemory.Span);
	}

	internal static string GetCertHashString(HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> rawData)
	{
		Span<byte> destination = stackalloc byte[64];
		return Convert.ToHexString(destination[..CryptographicOperations.HashData(hashAlgorithm, rawData, destination)]);
	}

	private byte[] GetRawCertHash()
	{
		return _lazyCertHash ?? (_lazyCertHash = Pal.Thumbprint);
	}

	public virtual string GetEffectiveDateString()
	{
		return GetNotBefore().ToString();
	}

	public virtual string GetExpirationDateString()
	{
		return GetNotAfter().ToString();
	}

	public virtual string GetFormat()
	{
		return "X509";
	}

	public virtual string GetPublicKeyString()
	{
		return GetPublicKey().ToHexStringUpper();
	}

	public virtual byte[] GetRawCertData()
	{
		ThrowIfInvalid();
		return PalRawDataMemory.ToArray();
	}

	public override int GetHashCode()
	{
		if (Pal == null)
		{
			return 0;
		}
		byte[] rawCertHash = GetRawCertHash();
		int num = 0;
		for (int i = 0; i < rawCertHash.Length && i < 4; i++)
		{
			num = (num << 8) | rawCertHash[i];
		}
		return num;
	}

	public virtual string GetKeyAlgorithm()
	{
		ThrowIfInvalid();
		return _lazyKeyAlgorithm ?? (_lazyKeyAlgorithm = Pal.KeyAlgorithm);
	}

	public virtual byte[]? GetKeyAlgorithmParameters()
	{
		ThrowIfInvalid();
		if (!_lazyKeyAlgorithmParametersCreated)
		{
			_lazyKeyAlgorithmParameters = Pal.KeyAlgorithmParameters;
			_lazyKeyAlgorithmParametersCreated = true;
		}
		return _lazyKeyAlgorithmParameters.CloneByteArray();
	}

	public virtual string? GetKeyAlgorithmParametersString()
	{
		ThrowIfInvalid();
		return GetKeyAlgorithmParameters()?.ToHexStringUpper();
	}

	public virtual byte[] GetPublicKey()
	{
		ThrowIfInvalid();
		return (_lazyPublicKey ?? (_lazyPublicKey = Pal.PublicKeyValue)).CloneByteArray();
	}

	public virtual byte[] GetSerialNumber()
	{
		ThrowIfInvalid();
		byte[] array = GetRawSerialNumber().CloneByteArray();
		Array.Reverse(array);
		return array;
	}

	public virtual string GetSerialNumberString()
	{
		ThrowIfInvalid();
		return GetRawSerialNumber().ToHexStringUpper();
	}

	private byte[] GetRawSerialNumber()
	{
		return _lazySerialNumber ?? (_lazySerialNumber = Pal.SerialNumber);
	}

	[Obsolete("X509Certificate.GetName has been deprecated. Use the Subject property instead.")]
	public virtual string GetName()
	{
		ThrowIfInvalid();
		return Pal.LegacySubject;
	}

	[Obsolete("X509Certificate.GetIssuerName has been deprecated. Use the Issuer property instead.")]
	public virtual string GetIssuerName()
	{
		ThrowIfInvalid();
		return Pal.LegacyIssuer;
	}

	public override string ToString()
	{
		return ToString(fVerbose: false);
	}

	public virtual string ToString(bool fVerbose)
	{
		if (!fVerbose || Pal == null)
		{
			return GetType().ToString();
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("[Subject]");
		stringBuilder.Append("  ");
		stringBuilder.AppendLine(Subject);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("[Issuer]");
		stringBuilder.Append("  ");
		stringBuilder.AppendLine(Issuer);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("[Serial Number]");
		stringBuilder.Append("  ");
		byte[] serialNumber = GetSerialNumber();
		Array.Reverse(serialNumber);
		stringBuilder.Append(serialNumber.ToHexArrayUpper());
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("[Not Before]");
		stringBuilder.Append("  ");
		stringBuilder.AppendLine(FormatDate(GetNotBefore()));
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("[Not After]");
		stringBuilder.Append("  ");
		stringBuilder.AppendLine(FormatDate(GetNotAfter()));
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("[Thumbprint]");
		stringBuilder.Append("  ");
		stringBuilder.Append(GetRawCertHash().ToHexArrayUpper());
		stringBuilder.AppendLine();
		return stringBuilder.ToString();
	}

	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public virtual void Import(byte[] rawData)
	{
		throw new PlatformNotSupportedException(System.SR.NotSupported_ImmutableX509Certificate);
	}

	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public virtual void Import(byte[] rawData, string? password, X509KeyStorageFlags keyStorageFlags)
	{
		throw new PlatformNotSupportedException(System.SR.NotSupported_ImmutableX509Certificate);
	}

	[CLSCompliant(false)]
	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public virtual void Import(byte[] rawData, SecureString? password, X509KeyStorageFlags keyStorageFlags)
	{
		throw new PlatformNotSupportedException(System.SR.NotSupported_ImmutableX509Certificate);
	}

	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public virtual void Import(string fileName)
	{
		throw new PlatformNotSupportedException(System.SR.NotSupported_ImmutableX509Certificate);
	}

	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public virtual void Import(string fileName, string? password, X509KeyStorageFlags keyStorageFlags)
	{
		throw new PlatformNotSupportedException(System.SR.NotSupported_ImmutableX509Certificate);
	}

	[CLSCompliant(false)]
	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public virtual void Import(string fileName, SecureString? password, X509KeyStorageFlags keyStorageFlags)
	{
		throw new PlatformNotSupportedException(System.SR.NotSupported_ImmutableX509Certificate);
	}

	internal DateTime GetNotAfter()
	{
		ThrowIfInvalid();
		DateTime dateTime = _lazyNotAfter;
		if (dateTime == DateTime.MinValue)
		{
			dateTime = (_lazyNotAfter = Pal.NotAfter);
		}
		return dateTime;
	}

	internal DateTime GetNotBefore()
	{
		ThrowIfInvalid();
		DateTime dateTime = _lazyNotBefore;
		if (dateTime == DateTime.MinValue)
		{
			dateTime = (_lazyNotBefore = Pal.NotBefore);
		}
		return dateTime;
	}

	[MemberNotNull("Pal")]
	internal void ThrowIfInvalid()
	{
		if (Pal == null)
		{
			throw new CryptographicException(System.SR.Format(System.SR.Cryptography_InvalidHandle, "m_safeCertContext"));
		}
	}

	protected static string FormatDate(DateTime date)
	{
		CultureInfo cultureInfo = CultureInfo.CurrentCulture;
		if (!cultureInfo.DateTimeFormat.Calendar.IsValidDay(date.Year, date.Month, date.Day, 0))
		{
			if (cultureInfo.DateTimeFormat.Calendar is UmAlQuraCalendar)
			{
				cultureInfo = cultureInfo.Clone() as CultureInfo;
				cultureInfo.DateTimeFormat.Calendar = new HijriCalendar();
			}
			else
			{
				cultureInfo = CultureInfo.InvariantCulture;
			}
		}
		return date.ToString(cultureInfo);
	}

	internal static void ValidateKeyStorageFlags(X509KeyStorageFlags keyStorageFlags)
	{
		X509CertificateLoader.ValidateKeyStorageFlags(keyStorageFlags);
	}

	private static void VerifyContentType(X509ContentType contentType)
	{
		if (contentType != X509ContentType.Cert && contentType != X509ContentType.SerializedCert && contentType != X509ContentType.Pfx)
		{
			throw new CryptographicException(System.SR.Cryptography_X509_InvalidContentType);
		}
	}

	internal static Pkcs12LoaderLimits GetPkcs12Limits(bool fromFile, SafePasswordHandle safePasswordHandle)
	{
		if (fromFile || safePasswordHandle.PasswordProvided)
		{
			return Pkcs12LoaderLimits.DangerousNoLimits;
		}
		return s_legacyLimits ?? (s_legacyLimits = MakeLegacyLimits());
	}

	private static Pkcs12LoaderLimits MakeLegacyLimits()
	{
		Pkcs12LoaderLimits pkcs12LoaderLimits = new Pkcs12LoaderLimits(Pkcs12LoaderLimits.DangerousNoLimits)
		{
			MacIterationLimit = 600000,
			IndividualKdfIterationLimit = 600000
		};
		long pkcs12UnspecifiedPasswordIterationLimit = System.LocalAppContextSwitches.Pkcs12UnspecifiedPasswordIterationLimit;
		if (pkcs12UnspecifiedPasswordIterationLimit == -1)
		{
			pkcs12LoaderLimits.TotalKdfIterationLimit = null;
		}
		else if (pkcs12UnspecifiedPasswordIterationLimit < 0)
		{
			pkcs12LoaderLimits.TotalKdfIterationLimit = 600000;
		}
		else
		{
			pkcs12LoaderLimits.TotalKdfIterationLimit = (int)long.Min(2147483647L, pkcs12UnspecifiedPasswordIterationLimit);
		}
		pkcs12LoaderLimits.MakeReadOnly();
		return pkcs12LoaderLimits;
	}
}
