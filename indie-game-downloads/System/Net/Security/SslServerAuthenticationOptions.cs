using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Security;

public class SslServerAuthenticationOptions
{
	private X509RevocationMode _checkCertificateRevocation;

	private SslProtocols _enabledSslProtocols;

	private EncryptionPolicy _encryptionPolicy;

	private bool _allowRenegotiation;

	private bool _allowTlsResume = true;

	private bool _allowRsaPssPadding = true;

	private bool _allowRsaPkcs1Padding = true;

	public bool AllowRenegotiation
	{
		get
		{
			return _allowRenegotiation;
		}
		set
		{
			_allowRenegotiation = value;
		}
	}

	public bool AllowTlsResume
	{
		get
		{
			return _allowTlsResume;
		}
		set
		{
			_allowTlsResume = value;
		}
	}

	public bool ClientCertificateRequired { get; set; }

	public List<SslApplicationProtocol>? ApplicationProtocols { get; set; }

	public RemoteCertificateValidationCallback? RemoteCertificateValidationCallback { get; set; }

	public ServerCertificateSelectionCallback? ServerCertificateSelectionCallback { get; set; }

	public X509Certificate? ServerCertificate { get; set; }

	public SslStreamCertificateContext? ServerCertificateContext { get; set; }

	public SslProtocols EnabledSslProtocols
	{
		get
		{
			return _enabledSslProtocols;
		}
		set
		{
			_enabledSslProtocols = value;
		}
	}

	public X509RevocationMode CertificateRevocationCheckMode
	{
		get
		{
			return _checkCertificateRevocation;
		}
		set
		{
			if (value != X509RevocationMode.NoCheck && value != X509RevocationMode.Offline && value != X509RevocationMode.Online)
			{
				throw new ArgumentException(System.SR.Format(System.SR.net_invalid_enum, "X509RevocationMode"), "value");
			}
			_checkCertificateRevocation = value;
		}
	}

	public EncryptionPolicy EncryptionPolicy
	{
		get
		{
			return _encryptionPolicy;
		}
		set
		{
			if (value != EncryptionPolicy.RequireEncryption && value != EncryptionPolicy.AllowNoEncryption && value != EncryptionPolicy.NoEncryption)
			{
				throw new ArgumentException(System.SR.Format(System.SR.net_invalid_enum, "EncryptionPolicy"), "value");
			}
			_encryptionPolicy = value;
		}
	}

	public CipherSuitesPolicy? CipherSuitesPolicy { get; set; }

	public X509ChainPolicy? CertificateChainPolicy { get; set; }

	public bool AllowRsaPssPadding
	{
		get
		{
			return _allowRsaPssPadding;
		}
		[SupportedOSPlatform("windows")]
		[SupportedOSPlatform("linux")]
		set
		{
			_allowRsaPssPadding = value;
		}
	}

	public bool AllowRsaPkcs1Padding
	{
		get
		{
			return _allowRsaPkcs1Padding;
		}
		[SupportedOSPlatform("windows")]
		[SupportedOSPlatform("linux")]
		set
		{
			_allowRsaPkcs1Padding = value;
		}
	}
}
