using System.Net.Security;
using System.Runtime.InteropServices;

namespace System.Net;

internal sealed class SSPIAuthType : ISSPIInterface
{
	private static volatile SecurityPackageInfoClass[] s_securityPackages;

	public SecurityPackageInfoClass[] SecurityPackages
	{
		get
		{
			return s_securityPackages;
		}
		set
		{
			s_securityPackages = value;
		}
	}

	public int EnumerateSecurityPackages(out int pkgnum, out SafeFreeContextBuffer pkgArray)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(this, null, "EnumerateSecurityPackages");
		}
		return SafeFreeContextBuffer.EnumeratePackages(out pkgnum, out pkgArray);
	}

	public int AcquireCredentialsHandle(string moduleName, global::Interop.SspiCli.CredentialUse usage, ref SafeSspiAuthDataHandle authdata, out SafeFreeCredentials outCredential)
	{
		return SafeFreeCredentials.AcquireCredentialsHandle(moduleName, usage, ref authdata, out outCredential);
	}

	public int AcquireDefaultCredential(string moduleName, global::Interop.SspiCli.CredentialUse usage, out SafeFreeCredentials outCredential)
	{
		return SafeFreeCredentials.AcquireDefaultCredential(moduleName, usage, out outCredential);
	}

	public unsafe int AcquireCredentialsHandle(string moduleName, global::Interop.SspiCli.CredentialUse usage, global::Interop.SspiCli.SCHANNEL_CRED* authdata, out SafeFreeCredentials outCredential)
	{
		return SafeFreeCredentials.AcquireCredentialsHandle(moduleName, usage, authdata, out outCredential);
	}

	public unsafe int AcquireCredentialsHandle(string moduleName, global::Interop.SspiCli.CredentialUse usage, global::Interop.SspiCli.SCH_CREDENTIALS* authdata, out SafeFreeCredentials outCredential)
	{
		return SafeFreeCredentials.AcquireCredentialsHandle(moduleName, usage, authdata, out outCredential);
	}

	public int AcceptSecurityContext(SafeFreeCredentials credential, ref SafeDeleteSslContext context, ref InputSecurityBuffers inputBuffers, global::Interop.SspiCli.ContextFlags inFlags, global::Interop.SspiCli.Endianness endianness, ref ProtocolToken outToken, ref global::Interop.SspiCli.ContextFlags outFlags)
	{
		return SafeDeleteContext.AcceptSecurityContext(ref credential, ref context, inFlags, endianness, ref inputBuffers, ref outToken, ref outFlags);
	}

	public int InitializeSecurityContext(ref SafeFreeCredentials credential, ref SafeDeleteSslContext context, string targetName, global::Interop.SspiCli.ContextFlags inFlags, global::Interop.SspiCli.Endianness endianness, ref InputSecurityBuffers inputBuffers, ref ProtocolToken outToken, ref global::Interop.SspiCli.ContextFlags outFlags)
	{
		return SafeDeleteContext.InitializeSecurityContext(ref credential, ref context, targetName, inFlags, endianness, ref inputBuffers, ref outToken, ref outFlags);
	}

	public int EncryptMessage(SafeDeleteContext context, ref global::Interop.SspiCli.SecBufferDesc inputOutput, uint qop)
	{
		bool success = false;
		try
		{
			context.DangerousAddRef(ref success);
			return global::Interop.SspiCli.EncryptMessage(ref context._handle, qop, ref inputOutput, 0u);
		}
		finally
		{
			if (success)
			{
				context.DangerousRelease();
			}
		}
	}

	public unsafe int DecryptMessage(SafeDeleteContext context, ref global::Interop.SspiCli.SecBufferDesc inputOutput, out uint qop)
	{
		int result = -2146893055;
		uint num = 0u;
		bool success = false;
		try
		{
			context.DangerousAddRef(ref success);
			result = global::Interop.SspiCli.DecryptMessage(ref context._handle, ref inputOutput, 0u, &num);
		}
		finally
		{
			if (success)
			{
				context.DangerousRelease();
			}
		}
		qop = num;
		return result;
	}

	public int QueryContextChannelBinding(SafeDeleteContext context, global::Interop.SspiCli.ContextAttribute attribute, out SafeFreeContextBufferChannelBinding binding)
	{
		throw new NotSupportedException();
	}

	public unsafe int QueryContextAttributes(SafeDeleteContext context, global::Interop.SspiCli.ContextAttribute attribute, nint* refHandle)
	{
		return SafeFreeContextBuffer.QueryContextAttributes(context, attribute, refHandle);
	}

	public unsafe int QueryContextAttributes(SafeDeleteContext context, global::Interop.SspiCli.ContextAttribute attribute, Span<byte> buffer, Type handleType, out SafeHandle refHandle)
	{
		refHandle = null;
		if (handleType != null)
		{
			if (!(handleType == typeof(SafeFreeContextBuffer)))
			{
				throw new ArgumentException(System.SR.Format(System.SR.SSPIInvalidHandleType, handleType.FullName), "handleType");
			}
			refHandle = SafeFreeContextBuffer.CreateEmptyHandle();
		}
		fixed (byte* buffer2 = buffer)
		{
			return SafeFreeContextBuffer.QueryContextAttributes(context, attribute, buffer2, refHandle);
		}
	}

	public int QuerySecurityContextToken(SafeDeleteContext phContext, out SecurityContextTokenHandle phToken)
	{
		return GetSecurityContextToken(phContext, out phToken);
	}

	public int CompleteAuthToken(ref SafeDeleteSslContext refContext, in InputSecurityBuffer inputBuffer)
	{
		return SafeDeleteContext.CompleteAuthToken(ref refContext, in inputBuffer);
	}

	private static int GetSecurityContextToken(SafeDeleteContext phContext, out SecurityContextTokenHandle safeHandle)
	{
		bool success = false;
		try
		{
			phContext.DangerousAddRef(ref success);
			return global::Interop.SspiCli.QuerySecurityContextToken(ref phContext._handle, out safeHandle);
		}
		finally
		{
			if (success)
			{
				phContext.DangerousRelease();
			}
		}
	}

	public int ApplyControlToken(ref SafeDeleteSslContext refContext, in SecurityBuffer inputBuffers)
	{
		throw new NotSupportedException();
	}

	int ISSPIInterface.CompleteAuthToken(ref SafeDeleteSslContext refContext, in InputSecurityBuffer inputBuffer)
	{
		return CompleteAuthToken(ref refContext, in inputBuffer);
	}

	int ISSPIInterface.ApplyControlToken(ref SafeDeleteSslContext refContext, in SecurityBuffer inputBuffer)
	{
		return ApplyControlToken(ref refContext, in inputBuffer);
	}
}
