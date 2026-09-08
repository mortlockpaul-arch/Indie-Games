using System.Collections.Generic;
using System.Formats.Asn1;

namespace System.Security.Cryptography.Asn1.Pkcs7;

internal struct SignedDataAsn
{
	internal int Version;

	internal AlgorithmIdentifierAsn[] DigestAlgorithms;

	internal EncapsulatedContentInfoAsn EncapContentInfo;

	internal CertificateChoiceAsn[] CertificateSet;

	internal ReadOnlyMemory<byte>[] Crls;

	internal SignerInfoAsn[] SignerInfos;

	internal static SignedDataAsn Decode(ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
	{
		return Decode(Asn1Tag.Sequence, encoded, ruleSet);
	}

	internal static SignedDataAsn Decode(Asn1Tag expectedTag, ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
	{
		try
		{
			AsnValueReader reader = new AsnValueReader(encoded.Span, ruleSet);
			DecodeCore(ref reader, expectedTag, encoded, out var decoded);
			reader.ThrowIfNotEmpty();
			return decoded;
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
	}

	private static void DecodeCore(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out SignedDataAsn decoded)
	{
		decoded = default(SignedDataAsn);
		AsnValueReader reader2 = reader.ReadSequence(expectedTag);
		ReadOnlySpan<byte> span = rebind.Span;
		if (!reader2.TryReadInt32(out decoded.Version))
		{
			reader2.ThrowIfNotEmpty();
		}
		AsnValueReader reader3 = reader2.ReadSetOf();
		List<AlgorithmIdentifierAsn> list = new List<AlgorithmIdentifierAsn>();
		while (reader3.HasData)
		{
			AlgorithmIdentifierAsn.Decode(ref reader3, rebind, out var decoded2);
			list.Add(decoded2);
		}
		decoded.DigestAlgorithms = list.ToArray();
		EncapsulatedContentInfoAsn.Decode(ref reader2, rebind, out decoded.EncapContentInfo);
		if (reader2.HasData && reader2.PeekTag().HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 0)))
		{
			reader3 = reader2.ReadSetOf(new Asn1Tag(TagClass.ContextSpecific, 0));
			List<CertificateChoiceAsn> list2 = new List<CertificateChoiceAsn>();
			while (reader3.HasData)
			{
				CertificateChoiceAsn.Decode(ref reader3, rebind, out var decoded3);
				list2.Add(decoded3);
			}
			decoded.CertificateSet = list2.ToArray();
		}
		if (reader2.HasData && reader2.PeekTag().HasSameClassAndValue(new Asn1Tag(TagClass.ContextSpecific, 1)))
		{
			reader3 = reader2.ReadSetOf(new Asn1Tag(TagClass.ContextSpecific, 1));
			List<ReadOnlyMemory<byte>> list3 = new List<ReadOnlyMemory<byte>>();
			while (reader3.HasData)
			{
				ReadOnlySpan<byte> other = reader3.ReadEncodedValue();
				ReadOnlyMemory<byte> item = (span.Overlaps(other, out var elementOffset) ? rebind.Slice(elementOffset, other.Length) : ((ReadOnlyMemory<byte>)other.ToArray()));
				list3.Add(item);
			}
			decoded.Crls = list3.ToArray();
		}
		reader3 = reader2.ReadSetOf();
		List<SignerInfoAsn> list4 = new List<SignerInfoAsn>();
		while (reader3.HasData)
		{
			SignerInfoAsn.Decode(ref reader3, rebind, out var decoded4);
			list4.Add(decoded4);
		}
		decoded.SignerInfos = list4.ToArray();
		reader2.ThrowIfNotEmpty();
	}
}
