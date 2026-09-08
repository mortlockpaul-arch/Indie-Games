using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.X509Certificates.Asn1;
using System.Text;
using Internal.Cryptography;

namespace System.Security.Cryptography.X509Certificates;

public class X509Certificate2 : X509Certificate
{
	private volatile Oid _lazySignatureAlgorithm;

	private volatile int _lazyVersion;

	private volatile X500DistinguishedName _lazySubjectName;

	private volatile X500DistinguishedName _lazyIssuerName;

	private volatile PublicKey _lazyPublicKey;

	private volatile AsymmetricAlgorithm _lazyPrivateKey;

	private volatile X509ExtensionCollection _lazyExtensions;

	private static readonly string[] s_RsaPublicKeyPrivateKeyLabels = new string[2] { "RSA PRIVATE KEY", "PRIVATE KEY" };

	private static readonly string[] s_DsaPublicKeyPrivateKeyLabels = new string[1] { "PRIVATE KEY" };

	internal new ICertificatePal Pal => (ICertificatePal)base.Pal;

	public bool Archived
	{
		get
		{
			ThrowIfInvalid();
			return Pal.Archived;
		}
		[SupportedOSPlatform("windows")]
		set
		{
			ThrowIfInvalid();
			Pal.Archived = value;
		}
	}

	public X509ExtensionCollection Extensions
	{
		get
		{
			ThrowIfInvalid();
			X509ExtensionCollection x509ExtensionCollection = _lazyExtensions;
			if (x509ExtensionCollection == null)
			{
				x509ExtensionCollection = new X509ExtensionCollection();
				foreach (X509Extension extension in Pal.Extensions)
				{
					X509Extension x509Extension = CreateCustomExtensionIfAny(extension.Oid);
					if (x509Extension == null)
					{
						x509ExtensionCollection.Add(extension);
						continue;
					}
					x509Extension.CopyFrom(extension);
					x509ExtensionCollection.Add(x509Extension);
				}
				_lazyExtensions = x509ExtensionCollection;
			}
			return x509ExtensionCollection;
		}
	}

	public string FriendlyName
	{
		get
		{
			ThrowIfInvalid();
			return Pal.FriendlyName;
		}
		[SupportedOSPlatform("windows")]
		set
		{
			ThrowIfInvalid();
			Pal.FriendlyName = value;
		}
	}

	public bool HasPrivateKey
	{
		get
		{
			ThrowIfInvalid();
			return Pal.HasPrivateKey;
		}
	}

	[Obsolete("X509Certificate2.PrivateKey is obsolete. Use the appropriate method to get the private key, such as GetRSAPrivateKey, or use the CopyWithPrivateKey method to create a new instance with a private key.", DiagnosticId = "SYSLIB0028", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public AsymmetricAlgorithm? PrivateKey
	{
		get
		{
			ThrowIfInvalid();
			if (!HasPrivateKey)
			{
				return null;
			}
			if (_lazyPrivateKey == null)
			{
				string keyAlgorithm = GetKeyAlgorithm();
				AsymmetricAlgorithm lazyPrivateKey;
				if (!(keyAlgorithm == "1.2.840.113549.1.1.1"))
				{
					if (!(keyAlgorithm == "1.2.840.10040.4.1"))
					{
						throw new NotSupportedException(System.SR.NotSupported_KeyAlgorithm);
					}
					lazyPrivateKey = Pal.GetDSAPrivateKey();
				}
				else
				{
					lazyPrivateKey = Pal.GetRSAPrivateKey();
				}
				_lazyPrivateKey = lazyPrivateKey;
			}
			return _lazyPrivateKey;
		}
		set
		{
			throw new PlatformNotSupportedException();
		}
	}

	public X500DistinguishedName IssuerName
	{
		get
		{
			ThrowIfInvalid();
			return _lazyIssuerName ?? (_lazyIssuerName = Pal.IssuerName);
		}
	}

	public DateTime NotAfter => GetNotAfter();

	public DateTime NotBefore => GetNotBefore();

	public PublicKey PublicKey
	{
		get
		{
			ThrowIfInvalid();
			PublicKey publicKey = _lazyPublicKey;
			if (publicKey == null)
			{
				string keyAlgorithm = GetKeyAlgorithm();
				byte[] keyAlgorithmParameters = Pal.KeyAlgorithmParameters;
				byte[] publicKeyValue = Pal.PublicKeyValue;
				Oid oid = new Oid(keyAlgorithm);
				publicKey = (_lazyPublicKey = new PublicKey(oid, (keyAlgorithmParameters == null) ? null : new AsnEncodedData(oid, keyAlgorithmParameters), new AsnEncodedData(oid, publicKeyValue), skipCopy: true));
			}
			return publicKey;
		}
	}

	public byte[] RawData => RawDataMemory.ToArray();

	public ReadOnlyMemory<byte> RawDataMemory => base.PalRawDataMemory;

	public string SerialNumber => GetSerialNumberString();

	public Oid SignatureAlgorithm
	{
		get
		{
			ThrowIfInvalid();
			return _lazySignatureAlgorithm ?? (_lazySignatureAlgorithm = new Oid(Pal.SignatureAlgorithm, null));
		}
	}

	public X500DistinguishedName SubjectName
	{
		get
		{
			ThrowIfInvalid();
			return _lazySubjectName ?? (_lazySubjectName = Pal.SubjectName);
		}
	}

	public string Thumbprint => GetCertHashString();

	public int Version
	{
		get
		{
			ThrowIfInvalid();
			int num = _lazyVersion;
			if (num == 0)
			{
				num = (_lazyVersion = Pal.Version);
			}
			return num;
		}
	}

	public override void Reset()
	{
		_lazySignatureAlgorithm = null;
		_lazyVersion = 0;
		_lazySubjectName = null;
		_lazyIssuerName = null;
		_lazyPublicKey = null;
		_lazyPrivateKey = null;
		_lazyExtensions = null;
		base.Reset();
	}

	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[UnsupportedOSPlatform("browser")]
	public X509Certificate2()
	{
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2(byte[] rawData)
		: base(rawData)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2(byte[] rawData, string? password)
		: base(rawData, password)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[CLSCompliant(false)]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2(byte[] rawData, SecureString? password)
		: base(rawData, password)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2(byte[] rawData, string? password, X509KeyStorageFlags keyStorageFlags)
		: base(rawData, password, keyStorageFlags)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[CLSCompliant(false)]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2(byte[] rawData, SecureString? password, X509KeyStorageFlags keyStorageFlags)
		: base(rawData, password, keyStorageFlags)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2(ReadOnlySpan<byte> rawData)
		: base(rawData)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2(ReadOnlySpan<byte> rawData, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
		: base(rawData, password, keyStorageFlags)
	{
	}

	[UnsupportedOSPlatform("browser")]
	public X509Certificate2(nint handle)
		: base(handle)
	{
	}

	internal X509Certificate2(ICertificatePal pal)
		: base(pal)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2(string fileName)
		: base(fileName)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2(string fileName, string? password)
		: base(fileName, password)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[CLSCompliant(false)]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2(string fileName, SecureString? password)
		: base(fileName, password)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2(string fileName, string? password, X509KeyStorageFlags keyStorageFlags)
		: base(fileName, password, keyStorageFlags)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[CLSCompliant(false)]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2(string fileName, SecureString? password, X509KeyStorageFlags keyStorageFlags)
		: base(fileName, password, keyStorageFlags)
	{
	}

	[UnsupportedOSPlatform("browser")]
	[Obsolete("Loading certificate data through the constructor or Import is obsolete. Use X509CertificateLoader instead to load certificates.", DiagnosticId = "SYSLIB0057", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2(string fileName, ReadOnlySpan<char> password, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
		: base(fileName, password, keyStorageFlags)
	{
	}

	[UnsupportedOSPlatform("browser")]
	public X509Certificate2(X509Certificate certificate)
		: base(certificate)
	{
	}

	[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected X509Certificate2(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		throw new PlatformNotSupportedException();
	}

	[UnsupportedOSPlatform("browser")]
	public static X509ContentType GetCertContentType(byte[] rawData)
	{
		if (rawData == null || rawData.Length == 0)
		{
			throw new ArgumentException(System.SR.Arg_EmptyOrNullArray, "rawData");
		}
		return X509Pal.Instance.GetCertContentType(rawData);
	}

	[UnsupportedOSPlatform("browser")]
	public static X509ContentType GetCertContentType(ReadOnlySpan<byte> rawData)
	{
		if (rawData.Length == 0)
		{
			throw new ArgumentException(System.SR.Arg_EmptyOrNullArray, "rawData");
		}
		return X509Pal.Instance.GetCertContentType(rawData);
	}

	[UnsupportedOSPlatform("browser")]
	public static X509ContentType GetCertContentType(string fileName)
	{
		ArgumentNullException.ThrowIfNull(fileName, "fileName");
		Path.GetFullPath(fileName);
		return X509Pal.Instance.GetCertContentType(fileName);
	}

	public string GetNameInfo(X509NameType nameType, bool forIssuer)
	{
		return Pal.GetNameInfo(nameType, forIssuer);
	}

	public override string ToString()
	{
		return base.ToString(fVerbose: true);
	}

	public override string ToString(bool verbose)
	{
		if (!verbose || Pal == null)
		{
			return ToString();
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("[Version]");
		stringBuilder.Append("  V");
		stringBuilder.Append(Version);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("[Subject]");
		stringBuilder.Append("  ");
		stringBuilder.Append(SubjectName.Name);
		string nameInfo = GetNameInfo(X509NameType.SimpleName, forIssuer: false);
		if (nameInfo.Length > 0)
		{
			stringBuilder.AppendLine();
			stringBuilder.Append("  ");
			stringBuilder.Append("Simple Name: ");
			stringBuilder.Append(nameInfo);
		}
		string nameInfo2 = GetNameInfo(X509NameType.EmailName, forIssuer: false);
		if (nameInfo2.Length > 0)
		{
			stringBuilder.AppendLine();
			stringBuilder.Append("  ");
			stringBuilder.Append("Email Name: ");
			stringBuilder.Append(nameInfo2);
		}
		string nameInfo3 = GetNameInfo(X509NameType.UpnName, forIssuer: false);
		if (nameInfo3.Length > 0)
		{
			stringBuilder.AppendLine();
			stringBuilder.Append("  ");
			stringBuilder.Append("UPN Name: ");
			stringBuilder.Append(nameInfo3);
		}
		string nameInfo4 = GetNameInfo(X509NameType.DnsName, forIssuer: false);
		if (nameInfo4.Length > 0)
		{
			stringBuilder.AppendLine();
			stringBuilder.Append("  ");
			stringBuilder.Append("DNS Name: ");
			stringBuilder.Append(nameInfo4);
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("[Issuer]");
		stringBuilder.Append("  ");
		stringBuilder.Append(IssuerName.Name);
		nameInfo = GetNameInfo(X509NameType.SimpleName, forIssuer: true);
		if (nameInfo.Length > 0)
		{
			stringBuilder.AppendLine();
			stringBuilder.Append("  ");
			stringBuilder.Append("Simple Name: ");
			stringBuilder.Append(nameInfo);
		}
		nameInfo2 = GetNameInfo(X509NameType.EmailName, forIssuer: true);
		if (nameInfo2.Length > 0)
		{
			stringBuilder.AppendLine();
			stringBuilder.Append("  ");
			stringBuilder.Append("Email Name: ");
			stringBuilder.Append(nameInfo2);
		}
		nameInfo3 = GetNameInfo(X509NameType.UpnName, forIssuer: true);
		if (nameInfo3.Length > 0)
		{
			stringBuilder.AppendLine();
			stringBuilder.Append("  ");
			stringBuilder.Append("UPN Name: ");
			stringBuilder.Append(nameInfo3);
		}
		nameInfo4 = GetNameInfo(X509NameType.DnsName, forIssuer: true);
		if (nameInfo4.Length > 0)
		{
			stringBuilder.AppendLine();
			stringBuilder.Append("  ");
			stringBuilder.Append("DNS Name: ");
			stringBuilder.Append(nameInfo4);
		}
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("[Serial Number]");
		stringBuilder.Append("  ");
		stringBuilder.AppendLine(SerialNumber);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("[Not Before]");
		stringBuilder.Append("  ");
		stringBuilder.AppendLine(X509Certificate.FormatDate(NotBefore));
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("[Not After]");
		stringBuilder.Append("  ");
		stringBuilder.AppendLine(X509Certificate.FormatDate(NotAfter));
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("[Thumbprint]");
		stringBuilder.Append("  ");
		stringBuilder.AppendLine(Thumbprint);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("[Signature Algorithm]");
		stringBuilder.Append("  ");
		stringBuilder.Append(SignatureAlgorithm.FriendlyName);
		stringBuilder.Append('(');
		stringBuilder.Append(SignatureAlgorithm.Value);
		stringBuilder.AppendLine(")");
		stringBuilder.AppendLine();
		stringBuilder.Append("[Public Key]");
		try
		{
			PublicKey publicKey = PublicKey;
			stringBuilder.AppendLine();
			stringBuilder.Append("  ");
			stringBuilder.Append("Algorithm: ");
			stringBuilder.Append(publicKey.Oid.FriendlyName);
			try
			{
				stringBuilder.AppendLine();
				stringBuilder.Append("  ");
				stringBuilder.Append("Length: ");
				using RSA rSA = this.GetRSAPublicKey();
				if (rSA != null)
				{
					stringBuilder.Append(rSA.KeySize);
				}
			}
			catch (NotSupportedException)
			{
			}
			stringBuilder.AppendLine();
			stringBuilder.Append("  ");
			stringBuilder.Append("Key Blob: ");
			stringBuilder.AppendLine(publicKey.EncodedKeyValue.Format(multiLine: true));
			stringBuilder.Append("  ");
			stringBuilder.Append("Parameters: ");
			AsnEncodedData encodedParameters = publicKey.EncodedParameters;
			if (encodedParameters != null)
			{
				stringBuilder.Append(encodedParameters.Format(multiLine: true));
			}
		}
		catch (CryptographicException)
		{
		}
		Pal.AppendPrivateKeyInfo(stringBuilder);
		X509ExtensionCollection extensions = Extensions;
		if (extensions.Count > 0)
		{
			stringBuilder.AppendLine();
			stringBuilder.AppendLine();
			stringBuilder.Append("[Extensions]");
			foreach (X509Extension item in extensions)
			{
				try
				{
					stringBuilder.AppendLine();
					stringBuilder.Append("* ");
					stringBuilder.Append(item.Oid.FriendlyName);
					stringBuilder.Append('(');
					stringBuilder.Append(item.Oid.Value);
					stringBuilder.Append("):");
					stringBuilder.AppendLine();
					stringBuilder.Append("  ");
					stringBuilder.Append(item.Format(multiLine: true));
				}
				catch (CryptographicException)
				{
				}
			}
		}
		stringBuilder.AppendLine();
		return stringBuilder.ToString();
	}

	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public override void Import(byte[] rawData)
	{
		base.Import(rawData);
	}

	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public override void Import(byte[] rawData, string? password, X509KeyStorageFlags keyStorageFlags)
	{
		base.Import(rawData, password, keyStorageFlags);
	}

	[CLSCompliant(false)]
	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public override void Import(byte[] rawData, SecureString? password, X509KeyStorageFlags keyStorageFlags)
	{
		base.Import(rawData, password, keyStorageFlags);
	}

	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public override void Import(string fileName)
	{
		base.Import(fileName);
	}

	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public override void Import(string fileName, string? password, X509KeyStorageFlags keyStorageFlags)
	{
		base.Import(fileName, password, keyStorageFlags);
	}

	[CLSCompliant(false)]
	[Obsolete("X509Certificate and X509Certificate2 are immutable. Use X509CertificateLoader to create a new certificate.", DiagnosticId = "SYSLIB0026", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public override void Import(string fileName, SecureString? password, X509KeyStorageFlags keyStorageFlags)
	{
		base.Import(fileName, password, keyStorageFlags);
	}

	public bool Verify()
	{
		ThrowIfInvalid();
		using X509Chain x509Chain = new X509Chain();
		bool result = x509Chain.Build(this, throwOnException: false);
		for (int i = 0; i < x509Chain.ChainElements.Count; i++)
		{
			x509Chain.ChainElements[i].Certificate.Dispose();
		}
		return result;
	}

	public ECDiffieHellman? GetECDiffieHellmanPublicKey()
	{
		return this.GetPublicKey<ECDiffieHellman>(HasECDiffieHellmanKeyUsage);
	}

	public ECDiffieHellman? GetECDiffieHellmanPrivateKey()
	{
		return this.GetPrivateKey<ECDiffieHellman>(HasECDiffieHellmanKeyUsage);
	}

	public X509Certificate2 CopyWithPrivateKey(ECDiffieHellman privateKey)
	{
		ArgumentNullException.ThrowIfNull(privateKey, "privateKey");
		if (HasPrivateKey)
		{
			throw new InvalidOperationException(System.SR.Cryptography_Cert_AlreadyHasPrivateKey);
		}
		using (ECDiffieHellman eCDiffieHellman = GetECDiffieHellmanPublicKey())
		{
			if (eCDiffieHellman == null)
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_WrongAlgorithm);
			}
			if (!Helpers.AreSamePublicECParameters(eCDiffieHellman.ExportParameters(includePrivateParameters: false), privateKey.ExportParameters(includePrivateParameters: false)))
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_DoesNotMatch, "privateKey");
			}
		}
		return new X509Certificate2(Pal.CopyWithPrivateKey(privateKey));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public MLKem? GetMLKemPublicKey()
	{
		if ((object)MLKemAlgorithm.FromOid(GetKeyAlgorithm()) == null)
		{
			return null;
		}
		return PublicKey.GetMLKemPublicKey();
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public MLKem? GetMLKemPrivateKey()
	{
		if ((object)MLKemAlgorithm.FromOid(GetKeyAlgorithm()) == null)
		{
			return null;
		}
		return Pal.GetMLKemPrivateKey();
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2 CopyWithPrivateKey(MLKem privateKey)
	{
		ArgumentNullException.ThrowIfNull(privateKey, "privateKey");
		if (HasPrivateKey)
		{
			throw new InvalidOperationException(System.SR.Cryptography_Cert_AlreadyHasPrivateKey);
		}
		using (MLKem mLKem = GetMLKemPublicKey())
		{
			if (mLKem == null)
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_WrongAlgorithm);
			}
			if (mLKem.Algorithm != privateKey.Algorithm)
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_DoesNotMatch, "privateKey");
			}
			using CryptoPoolLease cryptoPoolLease = CryptoPoolLease.Rent(mLKem.Algorithm.EncapsulationKeySizeInBytes, skipClear: true);
			using CryptoPoolLease cryptoPoolLease2 = CryptoPoolLease.Rent(mLKem.Algorithm.EncapsulationKeySizeInBytes, skipClear: true);
			mLKem.ExportEncapsulationKey(cryptoPoolLease.Span);
			privateKey.ExportEncapsulationKey(cryptoPoolLease2.Span);
			if (!((ReadOnlySpan<byte>)cryptoPoolLease.Span).SequenceEqual((ReadOnlySpan<byte>)cryptoPoolLease2.Span))
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_DoesNotMatch, "privateKey");
			}
		}
		return new X509Certificate2(Pal.CopyWithPrivateKey(privateKey));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public MLDsa? GetMLDsaPublicKey()
	{
		if ((object)MLDsaAlgorithm.GetMLDsaAlgorithmFromOid(GetKeyAlgorithm()) == null)
		{
			return null;
		}
		return PublicKey.GetMLDsaPublicKey();
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public MLDsa? GetMLDsaPrivateKey()
	{
		if ((object)MLDsaAlgorithm.GetMLDsaAlgorithmFromOid(GetKeyAlgorithm()) == null)
		{
			return null;
		}
		return Pal.GetMLDsaPrivateKey();
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2 CopyWithPrivateKey(MLDsa privateKey)
	{
		ArgumentNullException.ThrowIfNull(privateKey, "privateKey");
		if (HasPrivateKey)
		{
			throw new InvalidOperationException(System.SR.Cryptography_Cert_AlreadyHasPrivateKey);
		}
		using (MLDsa mLDsa = GetMLDsaPublicKey())
		{
			if (mLDsa == null)
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_WrongAlgorithm);
			}
			if (mLDsa.Algorithm != privateKey.Algorithm)
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_DoesNotMatch, "privateKey");
			}
			using CryptoPoolLease cryptoPoolLease = CryptoPoolLease.Rent(mLDsa.Algorithm.PublicKeySizeInBytes, skipClear: true);
			using CryptoPoolLease cryptoPoolLease2 = CryptoPoolLease.Rent(mLDsa.Algorithm.PublicKeySizeInBytes, skipClear: true);
			mLDsa.ExportMLDsaPublicKey(cryptoPoolLease.Span);
			privateKey.ExportMLDsaPublicKey(cryptoPoolLease2.Span);
			if (!((ReadOnlySpan<byte>)cryptoPoolLease.Span).SequenceEqual((ReadOnlySpan<byte>)cryptoPoolLease2.Span))
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_DoesNotMatch, "privateKey");
			}
		}
		return new X509Certificate2(Pal.CopyWithPrivateKey(privateKey));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public SlhDsa? GetSlhDsaPublicKey()
	{
		if (!Helpers.IsSlhDsaOid(GetKeyAlgorithm()))
		{
			return null;
		}
		return PublicKey.GetSlhDsaPublicKey();
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public SlhDsa? GetSlhDsaPrivateKey()
	{
		if (!Helpers.IsSlhDsaOid(GetKeyAlgorithm()))
		{
			return null;
		}
		return Pal.GetSlhDsaPrivateKey();
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2 CopyWithPrivateKey(SlhDsa privateKey)
	{
		ArgumentNullException.ThrowIfNull(privateKey, "privateKey");
		if (HasPrivateKey)
		{
			throw new InvalidOperationException(System.SR.Cryptography_Cert_AlreadyHasPrivateKey);
		}
		using (SlhDsa slhDsa = GetSlhDsaPublicKey())
		{
			if (slhDsa == null)
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_WrongAlgorithm);
			}
			if (slhDsa.Algorithm != privateKey.Algorithm)
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_DoesNotMatch, "privateKey");
			}
			Span<byte> span = stackalloc byte[128];
			Span<byte> span2 = span.Slice(0, slhDsa.Algorithm.PublicKeySizeInBytes);
			Span<byte> span3 = span.Slice(span2.Length, slhDsa.Algorithm.PublicKeySizeInBytes);
			slhDsa.ExportSlhDsaPublicKey(span2);
			privateKey.ExportSlhDsaPublicKey(span3);
			if (!((ReadOnlySpan<byte>)span2).SequenceEqual((ReadOnlySpan<byte>)span3))
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_DoesNotMatch, "privateKey");
			}
		}
		return new X509Certificate2(Pal.CopyWithPrivateKey(privateKey));
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public CompositeMLDsa? GetCompositeMLDsaPublicKey()
	{
		if ((object)CompositeMLDsaAlgorithm.GetAlgorithmFromOid(GetKeyAlgorithm()) == null)
		{
			return null;
		}
		return PublicKey.GetCompositeMLDsaPublicKey();
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public CompositeMLDsa? GetCompositeMLDsaPrivateKey()
	{
		if ((object)CompositeMLDsaAlgorithm.GetAlgorithmFromOid(GetKeyAlgorithm()) == null)
		{
			return null;
		}
		throw new PlatformNotSupportedException();
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public X509Certificate2 CopyWithPrivateKey(CompositeMLDsa privateKey)
	{
		ArgumentNullException.ThrowIfNull(privateKey, "privateKey");
		if (HasPrivateKey)
		{
			throw new InvalidOperationException(System.SR.Cryptography_Cert_AlreadyHasPrivateKey);
		}
		using (CompositeMLDsa compositeMLDsa = GetCompositeMLDsaPublicKey())
		{
			if (compositeMLDsa == null)
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_WrongAlgorithm);
			}
			if (compositeMLDsa.Algorithm != privateKey.Algorithm)
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_DoesNotMatch, "privateKey");
			}
			byte[] array = compositeMLDsa.ExportCompositeMLDsaPublicKey();
			if (!MemoryExtensions.SequenceEqual(other: privateKey.ExportCompositeMLDsaPublicKey(), span: array))
			{
				throw new ArgumentException(System.SR.Cryptography_PrivateKey_DoesNotMatch, "privateKey");
			}
		}
		throw new PlatformNotSupportedException();
	}

	[UnsupportedOSPlatform("browser")]
	public static X509Certificate2 CreateFromPemFile(string certPemFilePath, string? keyPemFilePath = null)
	{
		ArgumentNullException.ThrowIfNull(certPemFilePath, "certPemFilePath");
		ReadOnlySpan<char> readOnlySpan = File.ReadAllText(certPemFilePath).AsSpan();
		ReadOnlySpan<char> keyPem = ((keyPemFilePath == null) ? readOnlySpan : File.ReadAllText(keyPemFilePath).AsSpan());
		return CreateFromPem(readOnlySpan, keyPem);
	}

	[UnsupportedOSPlatform("browser")]
	public static X509Certificate2 CreateFromEncryptedPemFile(string certPemFilePath, ReadOnlySpan<char> password, string? keyPemFilePath = null)
	{
		ArgumentNullException.ThrowIfNull(certPemFilePath, "certPemFilePath");
		ReadOnlySpan<char> readOnlySpan = File.ReadAllText(certPemFilePath).AsSpan();
		ReadOnlySpan<char> keyPem = ((keyPemFilePath == null) ? readOnlySpan : File.ReadAllText(keyPemFilePath).AsSpan());
		return CreateFromEncryptedPem(readOnlySpan, keyPem, password);
	}

	[UnsupportedOSPlatform("browser")]
	public static X509Certificate2 CreateFromPem(ReadOnlySpan<char> certPem, ReadOnlySpan<char> keyPem)
	{
		using X509Certificate2 x509Certificate = CreateFromPem(certPem);
		string keyAlgorithm = x509Certificate.GetKeyAlgorithm();
		switch (keyAlgorithm)
		{
		case "1.2.840.113549.1.1.1":
			return ExtractKeyFromPem<RSA>(keyPem, (ReadOnlySpan<string>)s_RsaPublicKeyPrivateKeyLabels, (Func<ReadOnlySpan<char>, RSA>)((ReadOnlySpan<char> keyPem2) => CreateAndImport(keyPem2, RSA.Create)), (Func<RSA, X509Certificate2>)x509Certificate.CopyWithPrivateKey);
		case "1.2.840.10040.4.1":
			if (Helpers.IsDSASupported)
			{
				return ExtractKeyFromPem<DSA>(keyPem, (ReadOnlySpan<string>)s_DsaPublicKeyPrivateKeyLabels, (Func<ReadOnlySpan<char>, DSA>)((ReadOnlySpan<char> keyPem2) => CreateAndImport(keyPem2, DSA.Create)), (Func<DSA, X509Certificate2>)x509Certificate.CopyWithPrivateKey);
			}
			break;
		case "1.2.840.10045.2.1":
			return ExtractKeyFromECPem(x509Certificate, keyPem);
		case "2.16.840.1.101.3.4.4.1":
		case "2.16.840.1.101.3.4.4.2":
		case "2.16.840.1.101.3.4.4.3":
			return ExtractKeyFromPem<MLKem>(keyPem, new ReadOnlySpan<string>("PRIVATE KEY"), (Func<ReadOnlySpan<char>, MLKem>)MLKem.ImportFromPem, (Func<MLKem, X509Certificate2>)x509Certificate.CopyWithPrivateKey);
		case "2.16.840.1.101.3.4.3.17":
		case "2.16.840.1.101.3.4.3.18":
		case "2.16.840.1.101.3.4.3.19":
			return ExtractKeyFromPem<MLDsa>(keyPem, new ReadOnlySpan<string>("PRIVATE KEY"), (Func<ReadOnlySpan<char>, MLDsa>)MLDsa.ImportFromPem, (Func<MLDsa, X509Certificate2>)x509Certificate.CopyWithPrivateKey);
		}
		if (Helpers.IsSlhDsaOid(keyAlgorithm))
		{
			return ExtractKeyFromPem<SlhDsa>(keyPem, new ReadOnlySpan<string>("PRIVATE KEY"), (Func<ReadOnlySpan<char>, SlhDsa>)SlhDsa.ImportFromPem, (Func<SlhDsa, X509Certificate2>)x509Certificate.CopyWithPrivateKey);
		}
		throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownKeyAlgorithm, keyAlgorithm));
	}

	[UnsupportedOSPlatform("browser")]
	public static X509Certificate2 CreateFromEncryptedPem(ReadOnlySpan<char> certPem, ReadOnlySpan<char> keyPem, ReadOnlySpan<char> password)
	{
		using X509Certificate2 x509Certificate = CreateFromPem(certPem);
		string keyAlgorithm = x509Certificate.GetKeyAlgorithm();
		switch (keyAlgorithm)
		{
		case "1.2.840.113549.1.1.1":
			return ExtractKeyFromEncryptedPem<RSA>(keyPem, password, (Func<ReadOnlySpan<char>, ReadOnlySpan<char>, RSA>)((ReadOnlySpan<char> keyPem2, ReadOnlySpan<char> password2) => CreateAndImportEncrypted(keyPem2, password2, RSA.Create)), (Func<RSA, X509Certificate2>)x509Certificate.CopyWithPrivateKey);
		case "1.2.840.10040.4.1":
			if (Helpers.IsDSASupported)
			{
				return ExtractKeyFromEncryptedPem<DSA>(keyPem, password, (Func<ReadOnlySpan<char>, ReadOnlySpan<char>, DSA>)((ReadOnlySpan<char> keyPem2, ReadOnlySpan<char> password2) => CreateAndImportEncrypted(keyPem2, password2, DSA.Create)), (Func<DSA, X509Certificate2>)x509Certificate.CopyWithPrivateKey);
			}
			break;
		case "1.2.840.10045.2.1":
			return ExtractKeyFromEncryptedECPem(x509Certificate, keyPem, password);
		case "2.16.840.1.101.3.4.4.1":
		case "2.16.840.1.101.3.4.4.2":
		case "2.16.840.1.101.3.4.4.3":
			return ExtractKeyFromEncryptedPem<MLKem>(keyPem, password, (Func<ReadOnlySpan<char>, ReadOnlySpan<char>, MLKem>)MLKem.ImportFromEncryptedPem, (Func<MLKem, X509Certificate2>)x509Certificate.CopyWithPrivateKey);
		case "2.16.840.1.101.3.4.3.17":
		case "2.16.840.1.101.3.4.3.18":
		case "2.16.840.1.101.3.4.3.19":
			return ExtractKeyFromEncryptedPem<MLDsa>(keyPem, password, (Func<ReadOnlySpan<char>, ReadOnlySpan<char>, MLDsa>)MLDsa.ImportFromEncryptedPem, (Func<MLDsa, X509Certificate2>)x509Certificate.CopyWithPrivateKey);
		}
		if (Helpers.IsSlhDsaOid(keyAlgorithm))
		{
			return ExtractKeyFromEncryptedPem<SlhDsa>(keyPem, password, (Func<ReadOnlySpan<char>, ReadOnlySpan<char>, SlhDsa>)SlhDsa.ImportFromEncryptedPem, (Func<SlhDsa, X509Certificate2>)x509Certificate.CopyWithPrivateKey);
		}
		throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownKeyAlgorithm, keyAlgorithm));
	}

	private static bool IsECDsa(X509Certificate2 certificate)
	{
		using ECDsa eCDsa = certificate.GetECDsaPublicKey();
		return eCDsa != null;
	}

	private static bool IsECDiffieHellman(X509Certificate2 certificate)
	{
		using ECDiffieHellman eCDiffieHellman = certificate.GetECDiffieHellmanPublicKey();
		return eCDiffieHellman != null;
	}

	[UnsupportedOSPlatform("browser")]
	public static X509Certificate2 CreateFromPem(ReadOnlySpan<char> certPem)
	{
		PemEnumerator<char>.Enumerator enumerator = PemEnumerator.Utf16(certPem).GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (readOnlySpan2, pemFields2) = enumerator.Current;
			Range label = pemFields2.Label;
			if (readOnlySpan2[label.Start..label.End].SequenceEqual("CERTIFICATE".AsSpan()))
			{
				byte[] array = System.Security.Cryptography.CryptoPool.Rent(pemFields2.DecodedDataLength);
				label = pemFields2.Base64Data;
				if (!Convert.TryFromBase64Chars(readOnlySpan2[label.Start..label.End], array, out var bytesWritten) || bytesWritten != pemFields2.DecodedDataLength)
				{
					throw new CryptographicException(System.SR.Cryptography_X509_NoPemCertificate);
				}
				ReadOnlyMemory<byte> encoded = new ReadOnlyMemory<byte>(array, 0, bytesWritten);
				try
				{
					CertificateAsn.Decode(encoded, AsnEncodingRules.DER);
				}
				catch (CryptographicException)
				{
					throw new CryptographicException(System.SR.Cryptography_X509_NoPemCertificate);
				}
				X509Certificate2 result = X509CertificateLoader.LoadCertificate(encoded.Span);
				System.Security.Cryptography.CryptoPool.Return(array, 0);
				return result;
			}
		}
		throw new CryptographicException(System.SR.Cryptography_X509_NoPemCertificate);
	}

	public string ExportCertificatePem()
	{
		return PemEncoding.WriteString("CERTIFICATE".AsSpan(), RawDataMemory.Span);
	}

	public bool TryExportCertificatePem(Span<char> destination, out int charsWritten)
	{
		return PemEncoding.TryWrite("CERTIFICATE".AsSpan(), RawDataMemory.Span, destination, out charsWritten);
	}

	public bool MatchesHostname(string hostname, bool allowWildcards = true, bool allowCommonName = true)
	{
		ArgumentNullException.ThrowIfNull(hostname, "hostname");
		if (!IPAddress.TryParse(hostname, out IPAddress address) && Uri.CheckHostName(hostname) != UriHostNameType.Dns)
		{
			throw new ArgumentException(System.SR.Argument_InvalidHostnameOrIPAddress, "hostname");
		}
		X509Extension x509Extension = null;
		foreach (X509Extension extension in Pal.Extensions)
		{
			if (extension.Oid.Value == "2.5.29.17")
			{
				if (x509Extension != null)
				{
					throw new CryptographicException(System.SR.Cryptography_X509_TooManySANs);
				}
				x509Extension = extension;
			}
		}
		if (x509Extension != null)
		{
			X509SubjectAlternativeNameExtension x509SubjectAlternativeNameExtension = new X509SubjectAlternativeNameExtension();
			x509SubjectAlternativeNameExtension.CopyFrom(x509Extension);
			bool flag = false;
			if (address != null)
			{
				foreach (IPAddress item in x509SubjectAlternativeNameExtension.EnumerateIPAddresses())
				{
					if (item.Equals(address))
					{
						return true;
					}
					flag = true;
				}
			}
			else
			{
				ReadOnlySpan<char> readOnlySpan = hostname.AsSpan();
				if (hostname.EndsWith('.'))
				{
					readOnlySpan = readOnlySpan.Slice(0, readOnlySpan.Length - 1);
					if (readOnlySpan.IsEmpty)
					{
						return false;
					}
				}
				ReadOnlySpan<char> other = default(ReadOnlySpan<char>);
				int num = readOnlySpan.IndexOf('.');
				if (num > 0)
				{
					other = readOnlySpan.Slice(num + 1);
				}
				foreach (string item2 in x509SubjectAlternativeNameExtension.EnumerateDnsNames())
				{
					flag = true;
					if (item2.Length == 0)
					{
						continue;
					}
					ReadOnlySpan<char> span = item2.AsSpan();
					if (item2.EndsWith('.'))
					{
						span = span.Slice(0, span.Length - 1);
					}
					if (allowWildcards && span.StartsWith("*.".AsSpan()) && span.Length > 2)
					{
						if (span.Slice(2).Equals(other, StringComparison.OrdinalIgnoreCase))
						{
							return true;
						}
					}
					else if (span.Equals(readOnlySpan, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			if (flag)
			{
				return false;
			}
		}
		if (allowCommonName)
		{
			X500RelativeDistinguishedName x500RelativeDistinguishedName = null;
			foreach (X500RelativeDistinguishedName item3 in SubjectName.EnumerateRelativeDistinguishedNames())
			{
				if (item3.HasMultipleElements)
				{
					AsnValueReader asnValueReader = new AsnValueReader(item3.RawData.Span, AsnEncodingRules.DER);
					AsnValueReader asnValueReader2 = asnValueReader.ReadSetOf(null, skipSortOrderValidation: true);
					while (asnValueReader2.HasData)
					{
						AsnValueReader asnValueReader3 = asnValueReader2.ReadSequence();
						Oid sharedOrNullOid = Oids.GetSharedOrNullOid(ref asnValueReader3);
						if (Oids.CommonNameOid.ValueEquals(sharedOrNullOid))
						{
							return false;
						}
					}
				}
				else if (Oids.CommonNameOid.ValueEquals(item3.GetSingleElementType()))
				{
					if (x500RelativeDistinguishedName != null)
					{
						return false;
					}
					x500RelativeDistinguishedName = item3;
				}
			}
			if (x500RelativeDistinguishedName != null)
			{
				return hostname.Equals(x500RelativeDistinguishedName.GetSingleElementValue(), StringComparison.OrdinalIgnoreCase);
			}
		}
		return false;
	}

	private static TAlg CreateAndImport<TAlg>(ReadOnlySpan<char> keyPem, Func<TAlg> factory) where TAlg : AsymmetricAlgorithm
	{
		TAlg val = factory();
		val.ImportFromPem(keyPem);
		return val;
	}

	private static TAlg CreateAndImportEncrypted<TAlg>(ReadOnlySpan<char> keyPem, ReadOnlySpan<char> password, Func<TAlg> factory) where TAlg : AsymmetricAlgorithm
	{
		TAlg val = factory();
		val.ImportFromEncryptedPem(keyPem, password);
		return val;
	}

	private static X509Certificate2 ExtractKeyFromPem<TAlg>(ReadOnlySpan<char> keyPem, ReadOnlySpan<string> labels, Func<ReadOnlySpan<char>, TAlg> factory, Func<TAlg, X509Certificate2> import) where TAlg : IDisposable
	{
		PemEnumerator<char>.Enumerator enumerator = PemEnumerator.Utf16(keyPem).GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (readOnlySpan2, pemFields2) = enumerator.Current;
			Range label = pemFields2.Label;
			ReadOnlySpan<char> span = readOnlySpan2[label.Start..label.End];
			ReadOnlySpan<string> readOnlySpan3 = labels;
			for (int i = 0; i < readOnlySpan3.Length; i++)
			{
				string text = readOnlySpan3[i];
				if (span.SequenceEqual(text.AsSpan()))
				{
					label = pemFields2.Location;
					return ExtractKeyFromPem<TAlg>(readOnlySpan2[label.Start..label.End], factory, import);
				}
			}
		}
		throw new CryptographicException(System.SR.Cryptography_X509_NoOrMismatchedPemKey);
	}

	private static X509Certificate2 ExtractKeyFromPem<TAlg>(ReadOnlySpan<char> keyPem, Func<ReadOnlySpan<char>, TAlg> factory, Func<TAlg, X509Certificate2> import) where TAlg : IDisposable
	{
		using TAlg arg = factory(keyPem);
		try
		{
			return import(arg);
		}
		catch (ArgumentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_X509_NoOrMismatchedPemKey, inner);
		}
	}

	private static X509Certificate2 ExtractKeyFromEncryptedPem<TAlg>(ReadOnlySpan<char> keyPem, ReadOnlySpan<char> password, Func<ReadOnlySpan<char>, ReadOnlySpan<char>, TAlg> factory, Func<TAlg, X509Certificate2> import) where TAlg : IDisposable
	{
		PemEnumerator<char>.Enumerator enumerator = PemEnumerator.Utf16(keyPem).GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (readOnlySpan2, pemFields2) = enumerator.Current;
			Range label = pemFields2.Label;
			if (!readOnlySpan2[label.Start..label.End].SequenceEqual("ENCRYPTED PRIVATE KEY".AsSpan()))
			{
				continue;
			}
			label = pemFields2.Location;
			using TAlg arg = factory(readOnlySpan2[label.Start..label.End], password);
			try
			{
				return import(arg);
			}
			catch (ArgumentException inner)
			{
				throw new CryptographicException(System.SR.Cryptography_X509_NoOrMismatchedPemKey, inner);
			}
		}
		throw new CryptographicException(System.SR.Cryptography_X509_NoOrMismatchedPemKey);
	}

	private static X509Extension CreateCustomExtensionIfAny(Oid oid)
	{
		return CreateCustomExtensionIfAny(oid.Value);
	}

	internal static X509Extension CreateCustomExtensionIfAny(string oidValue)
	{
		switch (oidValue)
		{
		case "2.5.29.10":
			if (1 == 0)
			{
			}
			return new X509BasicConstraintsExtension();
		case "2.5.29.19":
			return new X509BasicConstraintsExtension();
		case "2.5.29.15":
			return new X509KeyUsageExtension();
		case "2.5.29.37":
			return new X509EnhancedKeyUsageExtension();
		case "2.5.29.14":
			return new X509SubjectKeyIdentifierExtension();
		case "2.5.29.35":
			return new X509AuthorityKeyIdentifierExtension();
		case "1.3.6.1.5.5.7.1.1":
			return new X509AuthorityInformationAccessExtension();
		case "2.5.29.17":
			return new X509SubjectAlternativeNameExtension();
		default:
			return null;
		}
	}

	private static bool HasECDiffieHellmanKeyUsage(X509Certificate2 certificate)
	{
		foreach (X509Extension extension in certificate.Extensions)
		{
			if (extension.Oid?.Value == "2.5.29.15" && extension is X509KeyUsageExtension x509KeyUsageExtension)
			{
				return (x509KeyUsageExtension.KeyUsages & X509KeyUsageFlags.KeyAgreement) != 0;
			}
		}
		return true;
	}

	[UnsupportedOSPlatform("browser")]
	private static X509Certificate2 ExtractKeyFromEncryptedECPem(X509Certificate2 certificate, ReadOnlySpan<char> keyPem, ReadOnlySpan<char> password)
	{
		PemEnumerator<char>.Enumerator enumerator = PemEnumerator.Utf16(keyPem).GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (readOnlySpan2, pemFields2) = enumerator.Current;
			Range label = pemFields2.Label;
			if (!readOnlySpan2[label.Start..label.End].SequenceEqual("ENCRYPTED PRIVATE KEY".AsSpan()))
			{
				continue;
			}
			byte[] array = System.Security.Cryptography.CryptoPool.Rent(pemFields2.DecodedDataLength);
			int clearSize = -1;
			ArraySegment<byte>? arraySegment = null;
			try
			{
				label = pemFields2.Base64Data;
				if (!Convert.TryFromBase64Chars(readOnlySpan2[label.Start..label.End], array, out var bytesWritten) || bytesWritten != pemFields2.DecodedDataLength)
				{
					break;
				}
				clearSize = bytesWritten;
				arraySegment = KeyFormatHelper.DecryptPkcs8(password, array.AsMemory(0, bytesWritten), out var bytesRead);
				if (bytesRead == bytesWritten)
				{
					X509Certificate2 x509Certificate = ExtractKeyFromECPrivateKeyInfo(certificate, arraySegment.Value);
					if (x509Certificate != null)
					{
						return x509Certificate;
					}
				}
			}
			catch (CryptographicException inner)
			{
				throw new CryptographicException(System.SR.Cryptography_X509_NoOrMismatchedPemKey, inner);
			}
			finally
			{
				System.Security.Cryptography.CryptoPool.Return(array, clearSize);
				if (arraySegment.HasValue)
				{
					System.Security.Cryptography.CryptoPool.Return(arraySegment.Value);
				}
			}
			break;
		}
		throw new CryptographicException(System.SR.Cryptography_X509_NoOrMismatchedPemKey);
	}

	[UnsupportedOSPlatform("browser")]
	private static X509Certificate2 ExtractKeyFromECPem(X509Certificate2 certificate, ReadOnlySpan<char> keyPem)
	{
		PemEnumerator<char>.Enumerator enumerator = PemEnumerator.Utf16(keyPem).GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (readOnlySpan2, pemFields2) = enumerator.Current;
			Range label = pemFields2.Label;
			ReadOnlySpan<char> span = readOnlySpan2[label.Start..label.End];
			if (span.SequenceEqual("EC PRIVATE KEY".AsSpan()))
			{
				if (IsECDiffieHellman(certificate))
				{
					return ExtractKeyFromPem<ECDiffieHellman>(keyPem, (Func<ReadOnlySpan<char>, ECDiffieHellman>)((ReadOnlySpan<char> keyPem2) => CreateAndImport(keyPem2, ECDiffieHellman.Create)), (Func<ECDiffieHellman, X509Certificate2>)certificate.CopyWithPrivateKey);
				}
				if (!IsECDsa(certificate))
				{
					break;
				}
				return ExtractKeyFromPem<ECDsa>(keyPem, (Func<ReadOnlySpan<char>, ECDsa>)((ReadOnlySpan<char> keyPem2) => CreateAndImport(keyPem2, ECDsa.Create)), (Func<ECDsa, X509Certificate2>)certificate.CopyWithPrivateKey);
			}
			if (!span.SequenceEqual("PRIVATE KEY".AsSpan()))
			{
				continue;
			}
			byte[] array = System.Security.Cryptography.CryptoPool.Rent(pemFields2.DecodedDataLength);
			int clearSize = -1;
			try
			{
				label = pemFields2.Base64Data;
				if (Convert.TryFromBase64Chars(readOnlySpan2[label.Start..label.End], array, out var bytesWritten) && bytesWritten == pemFields2.DecodedDataLength)
				{
					clearSize = bytesWritten;
					X509Certificate2 x509Certificate = ExtractKeyFromECPrivateKeyInfo(certificate, array.AsMemory(0, bytesWritten));
					if (x509Certificate != null)
					{
						return x509Certificate;
					}
				}
			}
			catch (CryptographicException inner)
			{
				throw new CryptographicException(System.SR.Cryptography_X509_NoOrMismatchedPemKey, inner);
			}
			finally
			{
				System.Security.Cryptography.CryptoPool.Return(array, clearSize);
			}
			break;
		}
		throw new CryptographicException(System.SR.Cryptography_X509_NoOrMismatchedPemKey);
	}

	[UnsupportedOSPlatform("browser")]
	private static X509Certificate2 ExtractKeyFromECPrivateKeyInfo(X509Certificate2 certificate, ReadOnlyMemory<byte> privateKeyInfo)
	{
		PrivateKeyInfoAsn keyInfo = PrivateKeyInfoAsn.Decode(privateKeyInfo, AsnEncodingRules.BER);
		X509KeyUsageFlags? keyUsageFlags = GetKeyUsageFlags(in keyInfo);
		if ((!keyUsageFlags.HasValue || ((uint?)keyUsageFlags & 0xFFFFFF79u) != 0) && IsECDiffieHellman(certificate))
		{
			using (ECDiffieHellman eCDiffieHellman = ECDiffieHellman.Create())
			{
				eCDiffieHellman.ImportPkcs8PrivateKey(privateKeyInfo.Span, out var bytesRead);
				if (bytesRead != privateKeyInfo.Length)
				{
					throw new CryptographicException();
				}
				return certificate.CopyWithPrivateKey(eCDiffieHellman);
			}
		}
		if (IsECDsa(certificate))
		{
			using (ECDsa eCDsa = ECDsa.Create())
			{
				eCDsa.ImportPkcs8PrivateKey(privateKeyInfo.Span, out var bytesRead2);
				if (bytesRead2 != privateKeyInfo.Length)
				{
					throw new CryptographicException();
				}
				return certificate.CopyWithPrivateKey(eCDsa);
			}
		}
		return null;
	}

	private static X509KeyUsageFlags? GetKeyUsageFlags(ref readonly PrivateKeyInfoAsn keyInfo)
	{
		if (keyInfo.Attributes == null)
		{
			return null;
		}
		AttributeAsn[] attributes = keyInfo.Attributes;
		for (int i = 0; i < attributes.Length; i++)
		{
			AttributeAsn attributeAsn = attributes[i];
			if (!(attributeAsn.AttrType != "2.5.29.15"))
			{
				ReadOnlyMemory<byte>[] attrValues = attributeAsn.AttrValues;
				if (attrValues != null && attrValues.Length == 1)
				{
					ReadOnlyMemory<byte> readOnlyMemory = attrValues[0];
					X509KeyUsageExtension.DecodeX509KeyUsageExtension(readOnlyMemory.Span, out var keyUsages);
					return keyUsages;
				}
				throw new CryptographicException(System.SR.Cryptography_X509_NoOrMismatchedPemKey);
			}
		}
		return null;
	}
}
