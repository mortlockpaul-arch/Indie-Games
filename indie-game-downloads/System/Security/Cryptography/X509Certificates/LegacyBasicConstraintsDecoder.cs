using System.Runtime.InteropServices;
using Internal.Cryptography;

namespace System.Security.Cryptography.X509Certificates;

internal static class LegacyBasicConstraintsDecoder
{
	internal unsafe static void DecodeX509BasicConstraintsExtension(byte[] encoded, out bool certificateAuthority, out bool hasPathLengthConstraint, out int pathLengthConstraint)
	{
		(certificateAuthority, hasPathLengthConstraint, pathLengthConstraint) = encoded.DecodeObject(CryptDecodeObjectStructType.X509_BASIC_CONSTRAINTS, (void* pvDecoded, int cbDecoded) => ((Marshal.ReadByte(((CERT_BASIC_CONSTRAINTS_INFO*)pvDecoded)->SubjectType.pbData) & 0x80) != 0, ((CERT_BASIC_CONSTRAINTS_INFO*)pvDecoded)->fPathLenConstraint != 0, ((CERT_BASIC_CONSTRAINTS_INFO*)pvDecoded)->dwPathLenConstraint));
	}
}
