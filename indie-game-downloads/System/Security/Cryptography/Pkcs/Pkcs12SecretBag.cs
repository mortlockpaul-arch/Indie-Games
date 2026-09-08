using System.Formats.Asn1;
using System.Security.Cryptography.Asn1.Pkcs12;

namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs12SecretBag : Pkcs12SafeBag
{
	private readonly SecretBagAsn _decoded;

	private Pkcs12SecretBag(ReadOnlyMemory<byte> encodedBagValue)
		: base("1.2.840.113549.1.12.10.1.5", encodedBagValue, skipCopy: true)
	{
	}

	private Pkcs12SecretBag(SecretBagAsn secretBagAsn, ReadOnlyMemory<byte> encodedBagValue)
		: this(encodedBagValue)
	{
		_decoded = secretBagAsn;
	}

	internal static Pkcs12SecretBag DecodeValue(ReadOnlyMemory<byte> bagValue)
	{
		return new Pkcs12SecretBag(SecretBagAsn.Decode(bagValue, AsnEncodingRules.BER), bagValue);
	}
}
