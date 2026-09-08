namespace System.Net.Security;

internal sealed class SafeFreeCredential_SECURITY : SafeFreeCredentials
{
	public bool HasLocalCertificate;

	protected override bool ReleaseHandle()
	{
		return global::Interop.SspiCli.FreeCredentialsHandle(ref _handle) == 0;
	}
}
