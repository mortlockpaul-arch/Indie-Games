using System.Runtime.InteropServices;

namespace Microsoft.Quic;

internal struct QUIC_CREDENTIAL_CONFIG
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct _Anonymous_e__Union
	{
		[FieldOffset(0)]
		internal unsafe QUIC_CERTIFICATE_HASH* CertificateHash;

		[FieldOffset(0)]
		internal unsafe QUIC_CERTIFICATE_HASH_STORE* CertificateHashStore;

		[FieldOffset(0)]
		internal unsafe void* CertificateContext;

		[FieldOffset(0)]
		internal unsafe QUIC_CERTIFICATE_FILE* CertificateFile;

		[FieldOffset(0)]
		internal unsafe QUIC_CERTIFICATE_FILE_PROTECTED* CertificateFileProtected;

		[FieldOffset(0)]
		internal unsafe QUIC_CERTIFICATE_PKCS12* CertificatePkcs12;
	}

	internal QUIC_CREDENTIAL_TYPE Type;

	internal QUIC_CREDENTIAL_FLAGS Flags;

	internal _Anonymous_e__Union Anonymous;

	internal unsafe sbyte* Principal;

	internal unsafe void* Reserved;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void*, int, void> AsyncHandler;

	internal QUIC_ALLOWED_CIPHER_SUITE_FLAGS AllowedCipherSuites;

	internal unsafe sbyte* CaCertificateFile;

	internal unsafe ref void* CertificateContext => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref this, 1)).Anonymous.CertificateContext;

	internal unsafe ref QUIC_CERTIFICATE_PKCS12* CertificatePkcs12 => ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref this, 1)).Anonymous.CertificatePkcs12;
}
