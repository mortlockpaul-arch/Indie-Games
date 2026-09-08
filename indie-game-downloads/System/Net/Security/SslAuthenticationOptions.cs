using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Security;

internal sealed class SslAuthenticationOptions : IDisposable
{
	[CompilerGenerated]
	private CipherSuitesPolicy _003CCipherSuitesPolicy_003Ek__BackingField;

	internal bool AllowRenegotiation { get; set; }

	internal string TargetHost { get; set; }

	internal X509CertificateCollection ClientCertificates { get; set; }

	internal List<SslApplicationProtocol> ApplicationProtocols { get; set; }

	internal bool IsServer { get; set; }

	internal SslStreamCertificateContext CertificateContext { get; private set; }

	internal bool OwnsCertificateContext { get; private set; }

	internal SslProtocols EnabledSslProtocols { get; set; }

	internal X509RevocationMode CertificateRevocationCheckMode { get; set; }

	internal EncryptionPolicy EncryptionPolicy { get; set; }

	internal bool RemoteCertRequired { get; set; }

	internal bool CheckCertName { get; set; }

	internal RemoteCertificateValidationCallback CertValidationDelegate { get; set; }

	internal LocalCertificateSelectionCallback CertSelectionDelegate { get; set; }

	internal ServerCertificateSelectionCallback ServerCertSelectionDelegate { get; set; }

	internal CipherSuitesPolicy CipherSuitesPolicy
	{
		[CompilerGenerated]
		set
		{
			_003CCipherSuitesPolicy_003Ek__BackingField = value;
		}
	}

	internal object UserState { get; set; }

	internal ServerOptionsSelectionCallback ServerOptionDelegate { get; set; }

	internal X509ChainPolicy CertificateChainPolicy { get; set; }

	internal bool AllowTlsResume { get; set; }

	internal bool AllowRsaPssPadding { get; set; }

	internal bool AllowRsaPkcs1Padding { get; set; }

	internal SslAuthenticationOptions()
	{
		TargetHost = string.Empty;
	}

	internal void UpdateOptions(SslClientAuthenticationOptions sslClientAuthenticationOptions)
	{
		if (CertValidationDelegate == null)
		{
			CertValidationDelegate = sslClientAuthenticationOptions.RemoteCertificateValidationCallback;
		}
		else if (sslClientAuthenticationOptions.RemoteCertificateValidationCallback != null && (Delegate?)CertValidationDelegate != (Delegate?)sslClientAuthenticationOptions.RemoteCertificateValidationCallback)
		{
			throw new InvalidOperationException(System.SR.Format(System.SR.net_conflicting_options, "RemoteCertificateValidationCallback"));
		}
		if (CertSelectionDelegate == null)
		{
			CertSelectionDelegate = sslClientAuthenticationOptions.LocalCertificateSelectionCallback;
		}
		else if (sslClientAuthenticationOptions.LocalCertificateSelectionCallback != null && (Delegate?)CertSelectionDelegate != (Delegate?)sslClientAuthenticationOptions.LocalCertificateSelectionCallback)
		{
			throw new InvalidOperationException(System.SR.Format(System.SR.net_conflicting_options, "LocalCertificateSelectionCallback"));
		}
		AllowRenegotiation = sslClientAuthenticationOptions.AllowRenegotiation;
		AllowTlsResume = sslClientAuthenticationOptions.AllowTlsResume;
		ApplicationProtocols = sslClientAuthenticationOptions.ApplicationProtocols;
		X509ChainPolicy? certificateChainPolicy = sslClientAuthenticationOptions.CertificateChainPolicy;
		CheckCertName = certificateChainPolicy == null || !certificateChainPolicy.VerificationFlags.HasFlag(X509VerificationFlags.IgnoreInvalidName);
		EnabledSslProtocols = FilterOutIncompatibleSslProtocols(sslClientAuthenticationOptions.EnabledSslProtocols);
		EncryptionPolicy = sslClientAuthenticationOptions.EncryptionPolicy;
		IsServer = false;
		RemoteCertRequired = true;
		CertificateContext = sslClientAuthenticationOptions.ClientCertificateContext;
		TargetHost = sslClientAuthenticationOptions.TargetHost ?? string.Empty;
		AllowRsaPssPadding = sslClientAuthenticationOptions.AllowRsaPssPadding;
		AllowRsaPkcs1Padding = sslClientAuthenticationOptions.AllowRsaPkcs1Padding;
		if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux() && (!sslClientAuthenticationOptions.AllowRsaPssPadding || !sslClientAuthenticationOptions.AllowRsaPkcs1Padding))
		{
			throw new PlatformNotSupportedException(System.SR.net_ssl_allow_rsa_padding_not_supported);
		}
		CertificateRevocationCheckMode = sslClientAuthenticationOptions.CertificateRevocationCheckMode;
		ClientCertificates = sslClientAuthenticationOptions.ClientCertificates;
		CipherSuitesPolicy = sslClientAuthenticationOptions.CipherSuitesPolicy;
		if (sslClientAuthenticationOptions.CertificateChainPolicy != null)
		{
			CertificateChainPolicy = sslClientAuthenticationOptions.CertificateChainPolicy.Clone();
		}
	}

	internal void UpdateOptions(ServerOptionsSelectionCallback optionCallback, object state)
	{
		CheckCertName = false;
		TargetHost = string.Empty;
		IsServer = true;
		UserState = state;
		ServerOptionDelegate = optionCallback;
	}

	internal void UpdateOptions(SslServerAuthenticationOptions sslServerAuthenticationOptions)
	{
		if (sslServerAuthenticationOptions.ServerCertificate == null && sslServerAuthenticationOptions.ServerCertificateContext == null && sslServerAuthenticationOptions.ServerCertificateSelectionCallback == null && CertSelectionDelegate == null)
		{
			throw new NotSupportedException(System.SR.net_ssl_io_no_server_cert);
		}
		if ((sslServerAuthenticationOptions.ServerCertificate != null || sslServerAuthenticationOptions.ServerCertificateContext != null || CertSelectionDelegate != null) && sslServerAuthenticationOptions.ServerCertificateSelectionCallback != null)
		{
			throw new InvalidOperationException(System.SR.Format(System.SR.net_conflicting_options, "ServerCertificateSelectionCallback"));
		}
		if (CertValidationDelegate == null)
		{
			CertValidationDelegate = sslServerAuthenticationOptions.RemoteCertificateValidationCallback;
		}
		else if (sslServerAuthenticationOptions.RemoteCertificateValidationCallback != null && (Delegate?)CertValidationDelegate != (Delegate?)sslServerAuthenticationOptions.RemoteCertificateValidationCallback)
		{
			throw new InvalidOperationException(System.SR.Format(System.SR.net_conflicting_options, "RemoteCertificateValidationCallback"));
		}
		IsServer = true;
		AllowRenegotiation = sslServerAuthenticationOptions.AllowRenegotiation;
		AllowTlsResume = sslServerAuthenticationOptions.AllowTlsResume;
		ApplicationProtocols = sslServerAuthenticationOptions.ApplicationProtocols;
		EnabledSslProtocols = FilterOutIncompatibleSslProtocols(sslServerAuthenticationOptions.EnabledSslProtocols);
		EncryptionPolicy = sslServerAuthenticationOptions.EncryptionPolicy;
		RemoteCertRequired = sslServerAuthenticationOptions.ClientCertificateRequired;
		CipherSuitesPolicy = sslServerAuthenticationOptions.CipherSuitesPolicy;
		CertificateRevocationCheckMode = sslServerAuthenticationOptions.CertificateRevocationCheckMode;
		AllowRsaPssPadding = sslServerAuthenticationOptions.AllowRsaPssPadding;
		AllowRsaPkcs1Padding = sslServerAuthenticationOptions.AllowRsaPkcs1Padding;
		if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux() && (!sslServerAuthenticationOptions.AllowRsaPssPadding || !sslServerAuthenticationOptions.AllowRsaPkcs1Padding))
		{
			throw new PlatformNotSupportedException(System.SR.net_ssl_allow_rsa_padding_not_supported);
		}
		if (sslServerAuthenticationOptions.ServerCertificateContext != null)
		{
			CertificateContext = sslServerAuthenticationOptions.ServerCertificateContext;
		}
		else if (sslServerAuthenticationOptions.ServerCertificate != null)
		{
			if (sslServerAuthenticationOptions.ServerCertificate is X509Certificate2 { HasPrivateKey: not false } x509Certificate)
			{
				bool isEnabled = false;
				AppContext.TryGetSwitch("System.Net.Security.EnableServerOcspStaplingFromOnlyCertificateOnLinux", out isEnabled);
				SetCertificateContextFromCert(x509Certificate, !isEnabled);
			}
			else
			{
				X509Certificate2 x509Certificate2 = SslStream.FindCertificateWithPrivateKey(this, isServer: true, sslServerAuthenticationOptions.ServerCertificate);
				if (x509Certificate2 == null)
				{
					throw new AuthenticationException(System.SR.net_ssl_io_no_server_cert);
				}
				SetCertificateContextFromCert(x509Certificate2);
			}
		}
		if (sslServerAuthenticationOptions.ServerCertificateSelectionCallback != null)
		{
			ServerCertSelectionDelegate = sslServerAuthenticationOptions.ServerCertificateSelectionCallback;
		}
		if (sslServerAuthenticationOptions.CertificateChainPolicy != null)
		{
			CertificateChainPolicy = sslServerAuthenticationOptions.CertificateChainPolicy.Clone();
		}
	}

	private static SslProtocols FilterOutIncompatibleSslProtocols(SslProtocols protocols)
	{
		if ((protocols & (SslProtocols.Tls12 | SslProtocols.Tls13)) != SslProtocols.None)
		{
			protocols &= ~SslProtocols.Ssl2;
		}
		return protocols;
	}

	internal void SetCertificateContextFromCert(X509Certificate2 certificate, bool? noOcspFetch = null)
	{
		CertificateContext = SslStreamCertificateContext.Create(certificate, null, offline: false, null, noOcspFetch ?? true);
		OwnsCertificateContext = true;
	}

	public void Dispose()
	{
		if (OwnsCertificateContext && CertificateContext != null)
		{
			CertificateContext.ReleaseResources();
		}
	}
}
