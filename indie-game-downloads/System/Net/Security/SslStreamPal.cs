using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace System.Net.Security;

internal static class SslStreamPal
{
	private static readonly byte[] s_http1 = global::Interop.Sec_Application_Protocols.ToByteArray(new List<SslApplicationProtocol> { SslApplicationProtocol.Http11 });

	private static readonly byte[] s_http2 = global::Interop.Sec_Application_Protocols.ToByteArray(new List<SslApplicationProtocol> { SslApplicationProtocol.Http2 });

	private static readonly byte[] s_http12 = global::Interop.Sec_Application_Protocols.ToByteArray(new List<SslApplicationProtocol>
	{
		SslApplicationProtocol.Http11,
		SslApplicationProtocol.Http2
	});

	private static readonly byte[] s_http21 = global::Interop.Sec_Application_Protocols.ToByteArray(new List<SslApplicationProtocol>
	{
		SslApplicationProtocol.Http2,
		SslApplicationProtocol.Http11
	});

	private static readonly bool UseNewCryptoApi = Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= 18836;

	private static readonly byte[] s_sessionTokenBuffer = InitSessionTokenBuffer();

	private static readonly byte[] s_schannelShutdownBytes = BitConverter.GetBytes(1);

	public static Exception GetException(SecurityStatusPal status)
	{
		return new Win32Exception((int)SecurityStatusAdapterPal.GetInteropFromSecurityStatusPal(status));
	}

	private static byte[] InitSessionTokenBuffer()
	{
		return MemoryMarshal.AsBytes(new ReadOnlySpan<global::Interop.SChannel.SCHANNEL_SESSION_TOKEN>(new global::Interop.SChannel.SCHANNEL_SESSION_TOKEN
		{
			dwTokenType = 3u,
			dwFlags = 2u
		})).ToArray();
	}

	private unsafe static void SetAlpn(ref InputSecurityBuffers inputBuffers, List<SslApplicationProtocol> alpn, Span<byte> localBuffer)
	{
		if (alpn.Count == 1 && alpn[0] == SslApplicationProtocol.Http11)
		{
			inputBuffers.SetNextBuffer(new InputSecurityBuffer(s_http1, SecurityBufferType.SECBUFFER_APPLICATION_PROTOCOLS));
			return;
		}
		if (alpn.Count == 1 && alpn[0] == SslApplicationProtocol.Http2)
		{
			inputBuffers.SetNextBuffer(new InputSecurityBuffer(s_http2, SecurityBufferType.SECBUFFER_APPLICATION_PROTOCOLS));
			return;
		}
		if (alpn.Count == 2 && alpn[0] == SslApplicationProtocol.Http11 && alpn[1] == SslApplicationProtocol.Http2)
		{
			inputBuffers.SetNextBuffer(new InputSecurityBuffer(s_http12, SecurityBufferType.SECBUFFER_APPLICATION_PROTOCOLS));
			return;
		}
		if (alpn.Count == 2 && alpn[0] == SslApplicationProtocol.Http2 && alpn[1] == SslApplicationProtocol.Http11)
		{
			inputBuffers.SetNextBuffer(new InputSecurityBuffer(s_http21, SecurityBufferType.SECBUFFER_APPLICATION_PROTOCOLS));
			return;
		}
		int protocolLength = global::Interop.Sec_Application_Protocols.GetProtocolLength(alpn);
		int num = sizeof(global::Interop.Sec_Application_Protocols) + protocolLength;
		Span<byte> span = ((num <= localBuffer.Length) ? localBuffer : ((Span<byte>)new byte[num]));
		global::Interop.Sec_Application_Protocols.SetProtocols(span, alpn, protocolLength);
		inputBuffers.SetNextBuffer(new InputSecurityBuffer(span, SecurityBufferType.SECBUFFER_APPLICATION_PROTOCOLS));
	}

	public static SecurityStatusPal SelectApplicationProtocol(SafeFreeCredentials credentialsHandle, SafeDeleteSslContext context, SslAuthenticationOptions sslAuthenticationOptions, ReadOnlySpan<byte> clientProtocols)
	{
		throw new PlatformNotSupportedException("SelectApplicationProtocol");
	}

	public static ProtocolToken AcceptSecurityContext(ref SafeFreeCredentials credentialsHandle, ref SafeDeleteSslContext context, ReadOnlySpan<byte> inputBuffer, out int consumed, SslAuthenticationOptions sslAuthenticationOptions)
	{
		global::Interop.SspiCli.ContextFlags outFlags = global::Interop.SspiCli.ContextFlags.Zero;
		InputSecurityBuffers inputBuffers = default(InputSecurityBuffers);
		inputBuffers.SetNextBuffer(new InputSecurityBuffer(inputBuffer, SecurityBufferType.SECBUFFER_TOKEN));
		inputBuffers.SetNextBuffer(new InputSecurityBuffer(default(ReadOnlySpan<byte>), SecurityBufferType.SECBUFFER_EMPTY));
		if (context == null && sslAuthenticationOptions.ApplicationProtocols != null && sslAuthenticationOptions.ApplicationProtocols.Count != 0)
		{
			Span<byte> localBuffer = stackalloc byte[64];
			SetAlpn(ref inputBuffers, sslAuthenticationOptions.ApplicationProtocols, localBuffer);
		}
		ProtocolToken outToken = new ProtocolToken
		{
			RentBuffer = true
		};
		int win32SecurityStatus = SSPIWrapper.AcceptSecurityContext(GlobalSSPI.SSPISecureChannel, credentialsHandle, ref context, (global::Interop.SspiCli.ContextFlags)(0x1811C | (sslAuthenticationOptions.RemoteCertRequired ? 2 : 0)), global::Interop.SspiCli.Endianness.SECURITY_NATIVE_DREP, ref inputBuffers, ref outToken, ref outFlags);
		consumed = inputBuffer.Length;
		if (inputBuffers._item1.Type == SecurityBufferType.SECBUFFER_EXTRA)
		{
			consumed -= inputBuffers._item1.Token.Length;
		}
		outToken.Status = SecurityStatusAdapterPal.GetSecurityStatusPalFromNativeInt(win32SecurityStatus);
		return outToken;
	}

	public static bool TryUpdateClintCertificate(SafeFreeCredentials _1, SafeDeleteSslContext _2, SslAuthenticationOptions _3)
	{
		return false;
	}

	public static ProtocolToken InitializeSecurityContext(ref SafeFreeCredentials credentialsHandle, ref SafeDeleteSslContext context, string targetName, ReadOnlySpan<byte> inputBuffer, out int consumed, SslAuthenticationOptions sslAuthenticationOptions)
	{
		bool flag = context == null;
		global::Interop.SspiCli.ContextFlags outFlags = global::Interop.SspiCli.ContextFlags.Zero;
		InputSecurityBuffers inputBuffers = default(InputSecurityBuffers);
		inputBuffers.SetNextBuffer(new InputSecurityBuffer(inputBuffer, SecurityBufferType.SECBUFFER_TOKEN));
		inputBuffers.SetNextBuffer(new InputSecurityBuffer(default(ReadOnlySpan<byte>), SecurityBufferType.SECBUFFER_EMPTY));
		if (context == null && sslAuthenticationOptions.ApplicationProtocols != null && sslAuthenticationOptions.ApplicationProtocols.Count != 0)
		{
			Span<byte> localBuffer = stackalloc byte[64];
			SetAlpn(ref inputBuffers, sslAuthenticationOptions.ApplicationProtocols, localBuffer);
		}
		ProtocolToken outToken = new ProtocolToken
		{
			RentBuffer = true
		};
		int win32SecurityStatus = SSPIWrapper.InitializeSecurityContext(GlobalSSPI.SSPISecureChannel, ref credentialsHandle, ref context, targetName, global::Interop.SspiCli.ContextFlags.ReplayDetect | global::Interop.SspiCli.ContextFlags.SequenceDetect | global::Interop.SspiCli.ContextFlags.Confidentiality | global::Interop.SspiCli.ContextFlags.AllocateMemory | global::Interop.SspiCli.ContextFlags.InitManualCredValidation, global::Interop.SspiCli.Endianness.SECURITY_NATIVE_DREP, ref inputBuffers, ref outToken, ref outFlags);
		outToken.Status = SecurityStatusAdapterPal.GetSecurityStatusPalFromNativeInt(win32SecurityStatus);
		consumed = inputBuffer.Length;
		if (inputBuffers._item1.Type == SecurityBufferType.SECBUFFER_EXTRA)
		{
			consumed -= inputBuffers._item1.Token.Length;
		}
		if (((!sslAuthenticationOptions.AllowTlsResume || SslStream.DisableTlsResume) & flag) && context != null)
		{
			SecurityBuffer inputBuffer2 = new SecurityBuffer(s_sessionTokenBuffer, SecurityBufferType.SECBUFFER_TOKEN);
			SecurityStatusPal securityStatusPalFromNativeInt = SecurityStatusAdapterPal.GetSecurityStatusPalFromNativeInt(SSPIWrapper.ApplyControlToken(GlobalSSPI.SSPISecureChannel, ref context, in inputBuffer2));
			if (securityStatusPalFromNativeInt.ErrorCode != SecurityStatusPalErrorCode.OK)
			{
				outToken.Status = securityStatusPalFromNativeInt;
			}
		}
		return outToken;
	}

	public static ProtocolToken Renegotiate(ref SafeFreeCredentials credentialsHandle, ref SafeDeleteSslContext context, SslAuthenticationOptions sslAuthenticationOptions)
	{
		int consumed;
		return AcceptSecurityContext(ref credentialsHandle, ref context, ReadOnlySpan<byte>.Empty, out consumed, sslAuthenticationOptions);
	}

	public static SafeFreeCredentials AcquireCredentialsHandle(SslAuthenticationOptions sslAuthenticationOptions, bool newCredentialsRequested)
	{
		SslStreamCertificateContext certificateContext = sslAuthenticationOptions.CertificateContext;
		try
		{
			EncryptionPolicy encryptionPolicy = sslAuthenticationOptions.EncryptionPolicy;
			SafeFreeCredentials safeFreeCredentials = ((!UseNewCryptoApi || encryptionPolicy == EncryptionPolicy.NoEncryption) ? AcquireCredentialsHandleSchannelCred(sslAuthenticationOptions) : AcquireCredentialsHandleSchCredentials(sslAuthenticationOptions));
			if (certificateContext != null && certificateContext.Trust != null && certificateContext.Trust._sendTrustInHandshake)
			{
				AttachCertificateStore(safeFreeCredentials, certificateContext.Trust._store);
			}
			if (newCredentialsRequested && sslAuthenticationOptions.CertificateContext != null)
			{
				((SafeFreeCredential_SECURITY)safeFreeCredentials).HasLocalCertificate = true;
			}
			return safeFreeCredentials;
		}
		catch (Win32Exception ex) when (ex.NativeErrorCode == -2146893042 && certificateContext != null)
		{
			using Microsoft.Win32.SafeHandles.SafeCertContextHandle safeCertContextHandle = global::Interop.Crypt32.CertDuplicateCertificateContext(certificateContext.TargetCertificate.Handle);
			throw new AuthenticationException(safeCertContextHandle.HasEphemeralPrivateKey ? System.SR.net_auth_ephemeral : System.SR.net_auth_SSPI, ex);
		}
		catch (Win32Exception innerException)
		{
			throw new AuthenticationException(System.SR.net_auth_SSPI, innerException);
		}
	}

	private unsafe static void AttachCertificateStore(SafeFreeCredentials cred, X509Store store)
	{
		global::Interop.SspiCli.SecPkgCred_ClientCertPolicy pBuffer = default(global::Interop.SspiCli.SecPkgCred_ClientCertPolicy);
		fixed (char* name = store.Name)
		{
			pBuffer.pwszSslCtlStoreName = name;
			global::Interop.SECURITY_STATUS sECURITY_STATUS = global::Interop.SspiCli.SetCredentialsAttributesW(in cred._handle, 96L, in pBuffer, sizeof(global::Interop.SspiCli.SecPkgCred_ClientCertPolicy));
			if (sECURITY_STATUS != global::Interop.SECURITY_STATUS.OK)
			{
				throw new Win32Exception((int)sECURITY_STATUS);
			}
		}
	}

	public unsafe static SafeFreeCredentials AcquireCredentialsHandleSchannelCred(SslAuthenticationOptions authOptions)
	{
		X509Certificate2 x509Certificate = authOptions.CertificateContext?.TargetCertificate;
		bool isServer = authOptions.IsServer;
		int protocolFlagsFromSslProtocols = GetProtocolFlagsFromSslProtocols(authOptions.EnabledSslProtocols, isServer);
		bool flag = authOptions.AllowTlsResume && !SslStream.DisableTlsResume;
		global::Interop.SspiCli.CredentialUse credUsage;
		global::Interop.SspiCli.SCHANNEL_CRED.Flags flags;
		if (!isServer)
		{
			credUsage = global::Interop.SspiCli.CredentialUse.SECPKG_CRED_OUTBOUND;
			flags = global::Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_CRED_MANUAL_CRED_VALIDATION | global::Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_CRED_NO_DEFAULT_CREDS | global::Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_SEND_AUX_RECORD;
			if (authOptions.CertificateRevocationCheckMode != X509RevocationMode.NoCheck)
			{
				flags |= global::Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_CRED_REVOCATION_CHECK_END_CERT | global::Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_CRED_IGNORE_NO_REVOCATION_CHECK | global::Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_CRED_IGNORE_REVOCATION_OFFLINE;
			}
		}
		else
		{
			credUsage = global::Interop.SspiCli.CredentialUse.SECPKG_CRED_INBOUND;
			flags = global::Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_CRED_NO_SYSTEM_MAPPER | global::Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_SEND_AUX_RECORD;
			if (!flag)
			{
				flags |= global::Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_CRED_DISABLE_RECONNECTS;
			}
		}
		EncryptionPolicy encryptionPolicy = authOptions.EncryptionPolicy;
		if ((protocolFlagsFromSslProtocols == 0 || (protocolFlagsFromSslProtocols & -61) != 0) && encryptionPolicy != EncryptionPolicy.AllowNoEncryption && encryptionPolicy != EncryptionPolicy.NoEncryption)
		{
			flags |= global::Interop.SspiCli.SCHANNEL_CRED.Flags.SCH_USE_STRONG_CRYPTO;
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info($"flags=({flags}), ProtocolFlags=({protocolFlagsFromSslProtocols}), EncryptionPolicy={encryptionPolicy}", null, "AcquireCredentialsHandleSchannelCred");
		}
		global::Interop.SspiCli.SCHANNEL_CRED sCHANNEL_CRED = CreateSecureCredential(flags, protocolFlagsFromSslProtocols, encryptionPolicy);
		if (!isServer && !flag)
		{
			sCHANNEL_CRED.dwSessionLifespan = -1;
		}
		if (x509Certificate != null)
		{
			sCHANNEL_CRED.cCreds = 1;
			global::Interop.Crypt32.CERT_CONTEXT* handle = (global::Interop.Crypt32.CERT_CONTEXT*)x509Certificate.Handle;
			sCHANNEL_CRED.paCred = &handle;
		}
		return AcquireCredentialsHandle(credUsage, &sCHANNEL_CRED);
	}

	public unsafe static SafeFreeCredentials AcquireCredentialsHandleSchCredentials(SslAuthenticationOptions authOptions)
	{
		X509Certificate2 x509Certificate = authOptions.CertificateContext?.TargetCertificate;
		bool isServer = authOptions.IsServer;
		int protocolFlagsFromSslProtocols = GetProtocolFlagsFromSslProtocols(authOptions.EnabledSslProtocols, isServer);
		bool flag = authOptions.AllowTlsResume && !SslStream.DisableTlsResume;
		global::Interop.SspiCli.CredentialUse credUsage;
		global::Interop.SspiCli.SCH_CREDENTIALS.Flags flags;
		if (isServer)
		{
			credUsage = global::Interop.SspiCli.CredentialUse.SECPKG_CRED_INBOUND;
			flags = global::Interop.SspiCli.SCH_CREDENTIALS.Flags.SCH_CRED_NO_SYSTEM_MAPPER | global::Interop.SspiCli.SCH_CREDENTIALS.Flags.SCH_SEND_AUX_RECORD;
			if (!flag)
			{
				flags |= global::Interop.SspiCli.SCH_CREDENTIALS.Flags.SCH_CRED_DISABLE_RECONNECTS;
			}
		}
		else
		{
			credUsage = global::Interop.SspiCli.CredentialUse.SECPKG_CRED_OUTBOUND;
			flags = global::Interop.SspiCli.SCH_CREDENTIALS.Flags.SCH_CRED_MANUAL_CRED_VALIDATION | global::Interop.SspiCli.SCH_CREDENTIALS.Flags.SCH_CRED_NO_DEFAULT_CREDS | global::Interop.SspiCli.SCH_CREDENTIALS.Flags.SCH_SEND_AUX_RECORD;
			if (authOptions.CertificateRevocationCheckMode != X509RevocationMode.NoCheck)
			{
				flags |= global::Interop.SspiCli.SCH_CREDENTIALS.Flags.SCH_CRED_REVOCATION_CHECK_END_CERT | global::Interop.SspiCli.SCH_CREDENTIALS.Flags.SCH_CRED_IGNORE_NO_REVOCATION_CHECK | global::Interop.SspiCli.SCH_CREDENTIALS.Flags.SCH_CRED_IGNORE_REVOCATION_OFFLINE;
			}
		}
		EncryptionPolicy encryptionPolicy = authOptions.EncryptionPolicy;
		switch (encryptionPolicy)
		{
		case EncryptionPolicy.RequireEncryption:
			if ((protocolFlagsFromSslProtocols & 0x30) == 0)
			{
				flags |= global::Interop.SspiCli.SCH_CREDENTIALS.Flags.SCH_USE_STRONG_CRYPTO;
			}
			break;
		case EncryptionPolicy.AllowNoEncryption:
			flags |= global::Interop.SspiCli.SCH_CREDENTIALS.Flags.SCH_ALLOW_NULL_ENCRYPTION;
			break;
		default:
			throw new ArgumentException(System.SR.Format(System.SR.net_invalid_enum, "EncryptionPolicy"), "policy");
		}
		global::Interop.SspiCli.SCH_CREDENTIALS sCH_CREDENTIALS = new global::Interop.SspiCli.SCH_CREDENTIALS
		{
			dwVersion = 5,
			dwFlags = flags
		};
		if (!isServer && !flag)
		{
			sCH_CREDENTIALS.dwSessionLifespan = -1;
		}
		if (x509Certificate != null)
		{
			sCH_CREDENTIALS.cCreds = 1;
			global::Interop.Crypt32.CERT_CONTEXT* handle = (global::Interop.Crypt32.CERT_CONTEXT*)x509Certificate.Handle;
			sCH_CREDENTIALS.paCred = &handle;
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info($"flags=({flags}), ProtocolFlags=({protocolFlagsFromSslProtocols}), EncryptionPolicy={encryptionPolicy}", null, "AcquireCredentialsHandleSchCredentials");
		}
		global::Interop.SspiCli.TLS_PARAMETERS tLS_PARAMETERS = default(global::Interop.SspiCli.TLS_PARAMETERS);
		sCH_CREDENTIALS.cTlsParameters = 1;
		sCH_CREDENTIALS.pTlsParameters = &tLS_PARAMETERS;
		if (protocolFlagsFromSslProtocols != 0)
		{
			tLS_PARAMETERS.grbitDisabledProtocols = (uint)(protocolFlagsFromSslProtocols ^ -1);
		}
		Span<global::Interop.SspiCli.CRYPTO_SETTINGS> span = stackalloc global::Interop.SspiCli.CRYPTO_SETTINGS[2];
		nint* num = stackalloc nint[2];
		*num = IntPtr.Zero;
		num[1] = IntPtr.Zero;
		Span<nint> span2 = new Span<nint>(num, 2);
		int num2 = 0;
		try
		{
			if (!authOptions.AllowRsaPkcs1Padding)
			{
				span2[num2] = Marshal.StringToHGlobalUni("SCH_RSA_PKCS_PAD");
				span[num2] = new global::Interop.SspiCli.CRYPTO_SETTINGS
				{
					eAlgorithmUsage = global::Interop.SspiCli.CRYPTO_SETTINGS.TlsAlgorithmUsage.TlsParametersCngAlgUsageCertSig
				};
				global::Interop.NtDll.RtlInitUnicodeString(out span[num2].strCngAlgId, span2[num2]);
				num2++;
			}
			if (!authOptions.AllowRsaPssPadding)
			{
				span2[num2] = Marshal.StringToHGlobalUni("SCH_RSA_PSS_PAD");
				span[num2] = new global::Interop.SspiCli.CRYPTO_SETTINGS
				{
					eAlgorithmUsage = global::Interop.SspiCli.CRYPTO_SETTINGS.TlsAlgorithmUsage.TlsParametersCngAlgUsageCertSig
				};
				global::Interop.NtDll.RtlInitUnicodeString(out span[num2].strCngAlgId, span2[num2]);
				num2++;
			}
			tLS_PARAMETERS.pDisabledCrypto = (global::Interop.SspiCli.CRYPTO_SETTINGS*)Unsafe.AsPointer(in MemoryMarshal.GetReference(span));
			tLS_PARAMETERS.cDisabledCrypto = num2;
			return AcquireCredentialsHandle(credUsage, &sCH_CREDENTIALS);
		}
		finally
		{
			Span<nint> span3 = span2.Slice(0, num2);
			for (int i = 0; i < span3.Length; i++)
			{
				nint num3 = span3[i];
				if (num3 != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(num3);
				}
			}
		}
	}

	public unsafe static ProtocolToken EncryptMessage(SafeDeleteSslContext securityContext, ReadOnlyMemory<byte> input, int headerSize, int trailerSize)
	{
		ProtocolToken result = default(ProtocolToken);
		result.RentBuffer = true;
		int size = checked(input.Length + headerSize + trailerSize);
		result.EnsureAvailableSpace(size);
		input.Span.CopyTo(result.AvailableSpan.Slice(headerSize, input.Length));
		Span<global::Interop.SspiCli.SecBuffer> span = stackalloc global::Interop.SspiCli.SecBuffer[4];
		global::Interop.SspiCli.SecBufferDesc secBufferDesc = new global::Interop.SspiCli.SecBufferDesc(4);
		secBufferDesc.pBuffers = Unsafe.AsPointer(in MemoryMarshal.GetReference(span));
		global::Interop.SspiCli.SecBufferDesc inputOutput = secBufferDesc;
		fixed (byte* payload = result.Payload)
		{
			ref global::Interop.SspiCli.SecBuffer reference = ref span[0];
			reference.BufferType = SecurityBufferType.SECBUFFER_STREAM_HEADER;
			reference.pvBuffer = (nint)payload;
			reference.cbBuffer = headerSize;
			ref global::Interop.SspiCli.SecBuffer reference2 = ref span[1];
			reference2.BufferType = SecurityBufferType.SECBUFFER_DATA;
			reference2.pvBuffer = (nint)(payload + headerSize);
			reference2.cbBuffer = input.Length;
			ref global::Interop.SspiCli.SecBuffer reference3 = ref span[2];
			reference3.BufferType = SecurityBufferType.SECBUFFER_STREAM_TRAILER;
			reference3.pvBuffer = (nint)(payload + headerSize + input.Length);
			reference3.cbBuffer = trailerSize;
			ref global::Interop.SspiCli.SecBuffer reference4 = ref span[3];
			reference4.BufferType = SecurityBufferType.SECBUFFER_EMPTY;
			reference4.cbBuffer = 0;
			reference4.pvBuffer = IntPtr.Zero;
			int num = GlobalSSPI.SSPISecureChannel.EncryptMessage(securityContext, ref inputOutput, 0u);
			if (num != 0)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Info(securityContext, $"Encrypt ERROR {num:X}", "EncryptMessage");
				}
				result.Size = 0;
				result.Status = SecurityStatusAdapterPal.GetSecurityStatusPalFromNativeInt(num);
				return result;
			}
			result.Size = checked(reference.cbBuffer + reference2.cbBuffer + reference3.cbBuffer);
			result.Status = new SecurityStatusPal(SecurityStatusPalErrorCode.OK);
		}
		return result;
	}

	public unsafe static SecurityStatusPal DecryptMessage(SafeDeleteSslContext securityContext, Span<byte> buffer, out int offset, out int count)
	{
		Span<global::Interop.SspiCli.SecBuffer> span = stackalloc global::Interop.SspiCli.SecBuffer[4];
		for (int i = 1; i < 4; i++)
		{
			ref global::Interop.SspiCli.SecBuffer reference = ref span[i];
			reference.BufferType = SecurityBufferType.SECBUFFER_EMPTY;
			reference.pvBuffer = IntPtr.Zero;
			reference.cbBuffer = 0;
		}
		fixed (byte* ptr = buffer)
		{
			ref global::Interop.SspiCli.SecBuffer reference2 = ref span[0];
			reference2.BufferType = SecurityBufferType.SECBUFFER_DATA;
			reference2.pvBuffer = (nint)ptr;
			reference2.cbBuffer = buffer.Length;
			global::Interop.SspiCli.SecBufferDesc secBufferDesc = new global::Interop.SspiCli.SecBufferDesc(4);
			secBufferDesc.pBuffers = Unsafe.AsPointer(in MemoryMarshal.GetReference(span));
			global::Interop.SspiCli.SecBufferDesc inputOutput = secBufferDesc;
			global::Interop.SECURITY_STATUS sECURITY_STATUS = (global::Interop.SECURITY_STATUS)GlobalSSPI.SSPISecureChannel.DecryptMessage(securityContext, ref inputOutput, out var _);
			count = 0;
			offset = 0;
			for (int j = 0; j < 4; j++)
			{
				if ((sECURITY_STATUS == global::Interop.SECURITY_STATUS.OK && span[j].BufferType == SecurityBufferType.SECBUFFER_DATA) || (sECURITY_STATUS != global::Interop.SECURITY_STATUS.OK && span[j].BufferType == SecurityBufferType.SECBUFFER_EXTRA))
				{
					offset = (int)((byte*)span[j].pvBuffer - ptr);
					count = span[j].cbBuffer;
					break;
				}
			}
			return SecurityStatusAdapterPal.GetSecurityStatusPalFromInterop(sECURITY_STATUS);
		}
	}

	public static SecurityStatusPal ApplyAlertToken(SafeDeleteSslContext securityContext, TlsAlertType alertType, TlsAlertMessage alertMessage)
	{
		byte[] data = MemoryMarshal.AsBytes(new ReadOnlySpan<global::Interop.SChannel.SCHANNEL_ALERT_TOKEN>(new global::Interop.SChannel.SCHANNEL_ALERT_TOKEN
		{
			dwTokenType = 2u,
			dwAlertType = (uint)alertType,
			dwAlertNumber = (uint)alertMessage
		})).ToArray();
		SecurityBuffer inputBuffer = new SecurityBuffer(data, SecurityBufferType.SECBUFFER_TOKEN);
		return SecurityStatusAdapterPal.GetSecurityStatusPalFromInterop((global::Interop.SECURITY_STATUS)SSPIWrapper.ApplyControlToken(GlobalSSPI.SSPISecureChannel, ref securityContext, in inputBuffer), attachException: true);
	}

	public static SecurityStatusPal ApplyShutdownToken(SafeDeleteSslContext securityContext)
	{
		SecurityBuffer inputBuffer = new SecurityBuffer(s_schannelShutdownBytes, SecurityBufferType.SECBUFFER_TOKEN);
		return SecurityStatusAdapterPal.GetSecurityStatusPalFromInterop((global::Interop.SECURITY_STATUS)SSPIWrapper.ApplyControlToken(GlobalSSPI.SSPISecureChannel, ref securityContext, in inputBuffer), attachException: true);
	}

	public static SafeFreeContextBufferChannelBinding QueryContextChannelBinding(SafeDeleteContext securityContext, ChannelBindingKind attribute)
	{
		return SSPIWrapper.QueryContextChannelBinding(GlobalSSPI.SSPISecureChannel, securityContext, (global::Interop.SspiCli.ContextAttribute)attribute);
	}

	public static void QueryContextStreamSizes(SafeDeleteContext securityContext, out StreamSizes streamSizes)
	{
		SecPkgContext_StreamSizes attribute = default(SecPkgContext_StreamSizes);
		SSPIWrapper.QueryBlittableContextAttributes(GlobalSSPI.SSPISecureChannel, securityContext, global::Interop.SspiCli.ContextAttribute.SECPKG_ATTR_STREAM_SIZES, ref attribute);
		streamSizes = new StreamSizes(attribute);
	}

	public static void QueryContextConnectionInfo(SafeDeleteContext securityContext, ref SslConnectionInfo connectionInfo)
	{
		connectionInfo.UpdateSslConnectionInfo(securityContext);
	}

	private static int GetProtocolFlagsFromSslProtocols(SslProtocols protocols, bool isServer)
	{
		int num = (int)protocols;
		if (isServer)
		{
			return num & 0x1554;
		}
		return num & 0x2AA8;
	}

	private static global::Interop.SspiCli.SCHANNEL_CRED CreateSecureCredential(global::Interop.SspiCli.SCHANNEL_CRED.Flags flags, int protocols, EncryptionPolicy policy)
	{
		global::Interop.SspiCli.SCHANNEL_CRED result = new global::Interop.SspiCli.SCHANNEL_CRED
		{
			hRootStore = IntPtr.Zero,
			aphMappers = IntPtr.Zero,
			palgSupportedAlgs = IntPtr.Zero,
			paCred = null,
			cCreds = 0,
			cMappers = 0,
			cSupportedAlgs = 0,
			dwSessionLifespan = 0,
			reserved = 0,
			dwVersion = 4
		};
		switch (policy)
		{
		case EncryptionPolicy.RequireEncryption:
			result.dwMinimumCipherStrength = 0;
			result.dwMaximumCipherStrength = 0;
			break;
		case EncryptionPolicy.AllowNoEncryption:
			result.dwMinimumCipherStrength = -1;
			result.dwMaximumCipherStrength = 0;
			break;
		case EncryptionPolicy.NoEncryption:
			result.dwMinimumCipherStrength = -1;
			result.dwMaximumCipherStrength = -1;
			break;
		default:
			throw new ArgumentException(System.SR.Format(System.SR.net_invalid_enum, "EncryptionPolicy"), "policy");
		}
		result.dwFlags = flags;
		result.grbitEnabledProtocols = protocols;
		return result;
	}

	private unsafe static SafeFreeCredentials AcquireCredentialsHandle(global::Interop.SspiCli.CredentialUse credUsage, global::Interop.SspiCli.SCHANNEL_CRED* secureCredential)
	{
		try
		{
			using SafeAccessTokenHandle safeAccessTokenHandle = SafeAccessTokenHandle.InvalidHandle;
			return WindowsIdentity.RunImpersonated(safeAccessTokenHandle, () => SSPIWrapper.AcquireCredentialsHandle(GlobalSSPI.SSPISecureChannel, "Microsoft Unified Security Protocol Provider", credUsage, secureCredential));
		}
		catch
		{
			return SSPIWrapper.AcquireCredentialsHandle(GlobalSSPI.SSPISecureChannel, "Microsoft Unified Security Protocol Provider", credUsage, secureCredential);
		}
	}

	private unsafe static SafeFreeCredentials AcquireCredentialsHandle(global::Interop.SspiCli.CredentialUse credUsage, global::Interop.SspiCli.SCH_CREDENTIALS* secureCredential)
	{
		try
		{
			using SafeAccessTokenHandle safeAccessTokenHandle = SafeAccessTokenHandle.InvalidHandle;
			return WindowsIdentity.RunImpersonated(safeAccessTokenHandle, () => SSPIWrapper.AcquireCredentialsHandle(GlobalSSPI.SSPISecureChannel, "Microsoft Unified Security Protocol Provider", credUsage, secureCredential));
		}
		catch
		{
			return SSPIWrapper.AcquireCredentialsHandle(GlobalSSPI.SSPISecureChannel, "Microsoft Unified Security Protocol Provider", credUsage, secureCredential);
		}
	}
}
