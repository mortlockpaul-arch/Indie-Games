using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Asn1.Pkcs12;

namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs12CertBag : Pkcs12SafeBag
{
	private readonly CertBagAsn _decoded;

	[CompilerGenerated]
	private readonly bool _003CIsX509Certificate_003Ek__BackingField;

	private Pkcs12CertBag(ReadOnlyMemory<byte> encodedBagValue, CertBagAsn decoded)
		: base("1.2.840.113549.1.12.10.1.3", encodedBagValue)
	{
		_decoded = decoded;
		_003CIsX509Certificate_003Ek__BackingField = _decoded.CertId == "1.2.840.113549.1.9.22.1";
	}

	internal static Pkcs12CertBag DecodeValue(ReadOnlyMemory<byte> bagValue)
	{
		CertBagAsn decoded = CertBagAsn.Decode(bagValue, AsnEncodingRules.BER);
		return new Pkcs12CertBag(bagValue, decoded);
	}
}
