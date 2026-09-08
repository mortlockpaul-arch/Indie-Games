using System.ComponentModel;
using System.Globalization;
using System.Net.Security;
using System.Runtime.InteropServices;

namespace System.Net;

internal static class SSPIWrapper
{
	internal static SecurityPackageInfoClass[] EnumerateSecurityPackages(ISSPIInterface secModule)
	{
		if (secModule.SecurityPackages == null)
		{
			lock (secModule)
			{
				if (secModule.SecurityPackages == null)
				{
					int pkgnum = 0;
					SafeFreeContextBuffer pkgArray = null;
					try
					{
						int num = secModule.EnumerateSecurityPackages(out pkgnum, out pkgArray);
						if (System.Net.NetEventSource.Log.IsEnabled())
						{
							System.Net.NetEventSource.Info(null, $"arrayBase: {pkgArray}", "EnumerateSecurityPackages");
						}
						if (num != 0)
						{
							throw new Win32Exception(num);
						}
						SecurityPackageInfoClass[] array = new SecurityPackageInfoClass[pkgnum];
						for (int i = 0; i < pkgnum; i++)
						{
							array[i] = new SecurityPackageInfoClass(pkgArray, i);
							if (System.Net.NetEventSource.Log.IsEnabled())
							{
								System.Net.NetEventSource.Log.EnumerateSecurityPackages(array[i].Name);
							}
						}
						secModule.SecurityPackages = array;
					}
					finally
					{
						pkgArray?.Dispose();
					}
				}
			}
		}
		return secModule.SecurityPackages;
	}

	internal static SecurityPackageInfoClass GetVerifyPackageInfo(ISSPIInterface secModule, string packageName, bool throwIfMissing)
	{
		SecurityPackageInfoClass[] array = EnumerateSecurityPackages(secModule);
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (string.Equals(array[i].Name, packageName, StringComparison.OrdinalIgnoreCase))
				{
					return array[i];
				}
			}
		}
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Log.SspiPackageNotFound(packageName);
		}
		if (throwIfMissing)
		{
			throw new NotSupportedException(System.SR.net_securitypackagesupport);
		}
		return null;
	}

	public static SafeFreeCredentials AcquireDefaultCredential(ISSPIInterface secModule, string package, global::Interop.SspiCli.CredentialUse intent)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Log.AcquireDefaultCredential(package, intent);
		}
		SafeFreeCredentials outCredential = null;
		int num = secModule.AcquireDefaultCredential(package, intent, out outCredential);
		if (num != 0)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(null, System.SR.Format(System.SR.net_log_operation_failed_with_error, "AcquireDefaultCredential", $"0x{num:X}"), "AcquireDefaultCredential");
			}
			throw new Win32Exception(num);
		}
		return outCredential;
	}

	public static SafeFreeCredentials AcquireCredentialsHandle(ISSPIInterface secModule, string package, global::Interop.SspiCli.CredentialUse intent, ref SafeSspiAuthDataHandle authdata)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Log.AcquireCredentialsHandle(package, intent, authdata);
		}
		SafeFreeCredentials outCredential = null;
		int num = secModule.AcquireCredentialsHandle(package, intent, ref authdata, out outCredential);
		if (num != 0)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(null, System.SR.Format(System.SR.net_log_operation_failed_with_error, "AcquireCredentialsHandle", $"0x{num:X}"), "AcquireCredentialsHandle");
			}
			throw new Win32Exception(num);
		}
		return outCredential;
	}

	public unsafe static SafeFreeCredentials AcquireCredentialsHandle(ISSPIInterface secModule, string package, global::Interop.SspiCli.CredentialUse intent, global::Interop.SspiCli.SCHANNEL_CRED* scc)
	{
		int num = secModule.AcquireCredentialsHandle(package, intent, scc, out var outCredential);
		if (num != 0)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(null, System.SR.Format(System.SR.net_log_operation_failed_with_error, "AcquireCredentialsHandle", $"0x{num:X}"), "AcquireCredentialsHandle");
			}
			throw new Win32Exception(num);
		}
		return outCredential;
	}

	public unsafe static SafeFreeCredentials AcquireCredentialsHandle(ISSPIInterface secModule, string package, global::Interop.SspiCli.CredentialUse intent, global::Interop.SspiCli.SCH_CREDENTIALS* scc)
	{
		int num = secModule.AcquireCredentialsHandle(package, intent, scc, out var outCredential);
		if (num != 0)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(null, System.SR.Format(System.SR.net_log_operation_failed_with_error, "AcquireCredentialsHandle", $"0x{num:X}"), "AcquireCredentialsHandle");
			}
			throw new Win32Exception(num);
		}
		return outCredential;
	}

	internal static int InitializeSecurityContext(ISSPIInterface secModule, ref SafeFreeCredentials credential, ref SafeDeleteSslContext context, string targetName, global::Interop.SspiCli.ContextFlags inFlags, global::Interop.SspiCli.Endianness datarep, ref InputSecurityBuffers inputBuffers, ref ProtocolToken outToken, ref global::Interop.SspiCli.ContextFlags outFlags)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Log.InitializeSecurityContext(credential, context, targetName, inFlags);
		}
		int num = secModule.InitializeSecurityContext(ref credential, ref context, targetName, inFlags, datarep, ref inputBuffers, ref outToken, ref outFlags);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Log.SecurityContextInputBuffers("InitializeSecurityContext", inputBuffers.Count, outToken.Size, (global::Interop.SECURITY_STATUS)num);
		}
		return num;
	}

	internal static int AcceptSecurityContext(ISSPIInterface secModule, SafeFreeCredentials credential, ref SafeDeleteSslContext context, global::Interop.SspiCli.ContextFlags inFlags, global::Interop.SspiCli.Endianness datarep, ref InputSecurityBuffers inputBuffers, ref ProtocolToken outToken, ref global::Interop.SspiCli.ContextFlags outFlags)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Log.AcceptSecurityContext(credential, context, inFlags);
		}
		int num = secModule.AcceptSecurityContext(credential, ref context, ref inputBuffers, inFlags, datarep, ref outToken, ref outFlags);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Log.SecurityContextInputBuffers("AcceptSecurityContext", inputBuffers.Count, outToken.Size, (global::Interop.SECURITY_STATUS)num);
		}
		return num;
	}

	internal static int CompleteAuthToken(ISSPIInterface secModule, ref SafeDeleteSslContext context, in InputSecurityBuffer inputBuffer)
	{
		int num = secModule.CompleteAuthToken(ref context, in inputBuffer);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Log.OperationReturnedSomething("CompleteAuthToken", (global::Interop.SECURITY_STATUS)num);
		}
		return num;
	}

	internal static int ApplyControlToken(ISSPIInterface secModule, ref SafeDeleteSslContext context, in SecurityBuffer inputBuffer)
	{
		int num = secModule.ApplyControlToken(ref context, in inputBuffer);
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Log.OperationReturnedSomething("ApplyControlToken", (global::Interop.SECURITY_STATUS)num);
		}
		return num;
	}

	public static int QuerySecurityContextToken(ISSPIInterface secModule, SafeDeleteContext context, out SecurityContextTokenHandle token)
	{
		return secModule.QuerySecurityContextToken(context, out token);
	}

	public static SafeFreeContextBufferChannelBinding QueryContextChannelBinding(ISSPIInterface secModule, SafeDeleteContext securityContext, global::Interop.SspiCli.ContextAttribute contextAttribute)
	{
		int num = secModule.QueryContextChannelBinding(securityContext, contextAttribute, out var refHandle);
		if (num != 0)
		{
			refHandle.Dispose();
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(null, $"ERROR = {ErrorDescription(num)}", "QueryContextChannelBinding");
			}
			return null;
		}
		return refHandle;
	}

	public static bool QueryBlittableContextAttributes<T>(ISSPIInterface secModule, SafeDeleteContext securityContext, global::Interop.SspiCli.ContextAttribute contextAttribute, ref T attribute) where T : unmanaged
	{
		Span<T> span = new Span<T>(ref attribute);
		int num = secModule.QueryContextAttributes(securityContext, contextAttribute, MemoryMarshal.AsBytes(span), null, out var refHandle);
		using (refHandle)
		{
			if (num != 0)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(null, $"ERROR = {ErrorDescription(num)}", "QueryBlittableContextAttributes");
				}
				return false;
			}
			return true;
		}
	}

	public static bool QueryBlittableContextAttributes<T>(ISSPIInterface secModule, SafeDeleteContext securityContext, global::Interop.SspiCli.ContextAttribute contextAttribute, Type safeHandleType, out SafeHandle sspiHandle, ref T attribute) where T : unmanaged
	{
		Span<T> span = new Span<T>(ref attribute);
		int num = secModule.QueryContextAttributes(securityContext, contextAttribute, MemoryMarshal.AsBytes(span), safeHandleType, out sspiHandle);
		if (num != 0)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(null, $"ERROR = {ErrorDescription(num)}", "QueryBlittableContextAttributes");
			}
			return false;
		}
		return true;
	}

	public static string QueryStringContextAttributes(ISSPIInterface secModule, SafeDeleteContext securityContext, global::Interop.SspiCli.ContextAttribute contextAttribute)
	{
		Span<byte> buffer = stackalloc byte[IntPtr.Size];
		int num = secModule.QueryContextAttributes(securityContext, contextAttribute, buffer, typeof(SafeFreeContextBuffer), out var refHandle);
		using (refHandle)
		{
			if (num != 0)
			{
				if (System.Net.NetEventSource.Log.IsEnabled())
				{
					System.Net.NetEventSource.Error(null, $"ERROR = {ErrorDescription(num)}", "QueryStringContextAttributes");
				}
				return null;
			}
			string text = Marshal.PtrToStringUni(refHandle.DangerousGetHandle());
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Info(null, text, "QueryStringContextAttributes");
			}
			return text;
		}
	}

	private unsafe static bool QueryCertContextAttribute(ISSPIInterface secModule, SafeDeleteContext securityContext, global::Interop.SspiCli.ContextAttribute attribute, out SafeFreeCertContext certContext)
	{
		nint zero = IntPtr.Zero;
		certContext = null;
		try
		{
			int num = secModule.QueryContextAttributes(securityContext, attribute, &zero);
			bool flag = num == 0 || num == -2146893042;
			if (num == 0 && zero != IntPtr.Zero)
			{
				certContext = new SafeFreeCertContext();
				certContext.Set(zero);
				zero = IntPtr.Zero;
			}
			if (!flag && System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(null, $"ERROR = {ErrorDescription(num)}", "QueryCertContextAttribute");
			}
			return flag;
		}
		finally
		{
			if (zero != IntPtr.Zero)
			{
				global::Interop.Crypt32.CertFreeCertificateContext(zero);
			}
		}
	}

	public static bool QueryContextAttributes_SECPKG_ATTR_REMOTE_CERT_CONTEXT(ISSPIInterface secModule, SafeDeleteContext securityContext, out SafeFreeCertContext certContext)
	{
		return QueryCertContextAttribute(secModule, securityContext, global::Interop.SspiCli.ContextAttribute.SECPKG_ATTR_REMOTE_CERT_CONTEXT, out certContext);
	}

	public static bool QueryContextAttributes_SECPKG_ATTR_LOCAL_CERT_CONTEXT(ISSPIInterface secModule, SafeDeleteContext securityContext, out SafeFreeCertContext certContext)
	{
		return QueryCertContextAttribute(secModule, securityContext, global::Interop.SspiCli.ContextAttribute.SECPKG_ATTR_LOCAL_CERT_CONTEXT, out certContext);
	}

	public static bool QueryContextAttributes_SECPKG_ATTR_REMOTE_CERT_CHAIN(ISSPIInterface secModule, SafeDeleteContext securityContext, out SafeFreeCertContext certContext)
	{
		return QueryCertContextAttribute(secModule, securityContext, global::Interop.SspiCli.ContextAttribute.SECPKG_ATTR_REMOTE_CERT_CHAIN, out certContext);
	}

	public static bool QueryContextAttributes_SECPKG_ATTR_ISSUER_LIST_EX(ISSPIInterface secModule, SafeDeleteContext securityContext, ref global::Interop.SspiCli.SecPkgContext_IssuerListInfoEx ctx, out SafeHandle sspiHandle)
	{
		Span<global::Interop.SspiCli.SecPkgContext_IssuerListInfoEx> span = new Span<global::Interop.SspiCli.SecPkgContext_IssuerListInfoEx>(ref ctx);
		int num = secModule.QueryContextAttributes(securityContext, global::Interop.SspiCli.ContextAttribute.SECPKG_ATTR_ISSUER_LIST_EX, MemoryMarshal.AsBytes(span), typeof(SafeFreeContextBuffer), out sspiHandle);
		if (num != 0)
		{
			if (System.Net.NetEventSource.Log.IsEnabled())
			{
				System.Net.NetEventSource.Error(null, $"ERROR = {ErrorDescription(num)}", "QueryContextAttributes_SECPKG_ATTR_ISSUER_LIST_EX");
			}
			return false;
		}
		return true;
	}

	public static string ErrorDescription(int errorCode)
	{
		if (errorCode == -1)
		{
			return "An exception when invoking Win32 API";
		}
		return (global::Interop.SECURITY_STATUS)errorCode switch
		{
			global::Interop.SECURITY_STATUS.InvalidHandle => "Invalid handle", 
			global::Interop.SECURITY_STATUS.InvalidToken => "Invalid token", 
			global::Interop.SECURITY_STATUS.ContinueNeeded => "Continue needed", 
			global::Interop.SECURITY_STATUS.IncompleteMessage => "Message incomplete", 
			global::Interop.SECURITY_STATUS.WrongPrincipal => "Wrong principal", 
			global::Interop.SECURITY_STATUS.TargetUnknown => "Target unknown", 
			global::Interop.SECURITY_STATUS.PackageNotFound => "Package not found", 
			global::Interop.SECURITY_STATUS.BufferNotEnough => "Buffer not enough", 
			global::Interop.SECURITY_STATUS.MessageAltered => "Message altered", 
			global::Interop.SECURITY_STATUS.UntrustedRoot => "Untrusted root", 
			_ => "0x" + errorCode.ToString("x", NumberFormatInfo.InvariantInfo), 
		};
	}
}
