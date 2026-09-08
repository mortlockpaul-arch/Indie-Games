using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

namespace System.Net;

internal static class CertificateValidation
{
	internal unsafe static SslPolicyErrors BuildChainAndVerifyProperties(X509Chain chain, X509Certificate2 remoteCertificate, bool checkCertName, bool isServer, string hostName)
	{
		SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;
		bool num = chain.Build(remoteCertificate);
		if (!num && chain.SafeHandle.DangerousGetHandle() == IntPtr.Zero)
		{
			throw new CryptographicException(Marshal.GetLastPInvokeError());
		}
		if (checkCertName)
		{
			global::Interop.Crypt32.SSL_EXTRA_CERT_CHAIN_POLICY_PARA sSL_EXTRA_CERT_CHAIN_POLICY_PARA = new global::Interop.Crypt32.SSL_EXTRA_CERT_CHAIN_POLICY_PARA
			{
				cbSize = (uint)sizeof(global::Interop.Crypt32.SSL_EXTRA_CERT_CHAIN_POLICY_PARA),
				dwAuthType = (isServer ? 1u : 2u),
				fdwChecks = 0u,
				pwszServerName = null
			};
			global::Interop.Crypt32.CERT_CHAIN_POLICY_PARA cpp = new global::Interop.Crypt32.CERT_CHAIN_POLICY_PARA
			{
				cbSize = (uint)sizeof(global::Interop.Crypt32.CERT_CHAIN_POLICY_PARA),
				dwFlags = 0u,
				pvExtraPolicyPara = &sSL_EXTRA_CERT_CHAIN_POLICY_PARA
			};
			fixed (char* pwszServerName = hostName)
			{
				sSL_EXTRA_CERT_CHAIN_POLICY_PARA.pwszServerName = (ushort*)pwszServerName;
				cpp.dwFlags |= 4031u;
				if (Verify(chain.SafeHandle, ref cpp) == 2148204815u)
				{
					sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNameMismatch;
				}
			}
		}
		if (!num)
		{
			sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
		}
		return sslPolicyErrors;
	}

	private unsafe static uint Verify(SafeX509ChainHandle chainContext, ref global::Interop.Crypt32.CERT_CHAIN_POLICY_PARA cpp)
	{
		global::Interop.Crypt32.CERT_CHAIN_POLICY_STATUS pPolicyStatus = new global::Interop.Crypt32.CERT_CHAIN_POLICY_STATUS
		{
			cbSize = (uint)sizeof(global::Interop.Crypt32.CERT_CHAIN_POLICY_STATUS)
		};
		bool flag = global::Interop.Crypt32.CertVerifyCertificateChainPolicy(4, chainContext, ref cpp, ref pPolicyStatus);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(chainContext, $"CertVerifyCertificateChainPolicy returned: {flag}. Status: {pPolicyStatus.dwError}", "Verify");
		}
		return pPolicyStatus.dwError;
	}
}
