using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Formats.Asn1;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.X509Certificates.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography.X509Certificates;

internal sealed class Pkcs10CertificationRequestInfo
{
	internal X500DistinguishedName Subject { get; set; }

	internal PublicKey PublicKey { get; set; }

	internal Collection<X501Attribute> Attributes { get; } = new Collection<X501Attribute>();

	internal Pkcs10CertificationRequestInfo(X500DistinguishedName subject, PublicKey publicKey, IEnumerable<X501Attribute> attributes)
	{
		ArgumentNullException.ThrowIfNull(subject, "subject");
		ArgumentNullException.ThrowIfNull(publicKey, "publicKey");
		Subject = subject;
		PublicKey = publicKey;
		if (attributes != null)
		{
			Attributes.AddRange(attributes);
		}
	}

	internal byte[] ToPkcs10Request(X509SignatureGenerator signatureGenerator, HashAlgorithmName hashAlgorithm)
	{
		AlgorithmIdentifierAsn signatureAlgorithm = AlgorithmIdentifierAsn.Decode(signatureGenerator.GetSignatureAlgorithmIdentifier(hashAlgorithm), AsnEncodingRules.DER);
		if (signatureAlgorithm.Parameters.HasValue)
		{
			Helpers.ValidateDer(signatureAlgorithm.Parameters.Value.Span);
		}
		SubjectPublicKeyInfoAsn subjectPublicKeyInfo = new SubjectPublicKeyInfoAsn
		{
			Algorithm = new AlgorithmIdentifierAsn
			{
				Algorithm = PublicKey.Oid.Value,
				Parameters = PublicKey.EncodedParameters?.RawData.ToNullableMemory()
			},
			SubjectPublicKey = PublicKey.EncodedKeyValue.RawData
		};
		AttributeAsn[] array = new AttributeAsn[Attributes.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new AttributeAsn(Attributes[i]);
		}
		CertificationRequestInfoAsn certificationRequestInfo = new CertificationRequestInfoAsn
		{
			Version = 0,
			Subject = Subject.RawData,
			SubjectPublicKeyInfo = subjectPublicKeyInfo,
			Attributes = array
		};
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
		certificationRequestInfo.Encode(asnWriter);
		byte[] data = asnWriter.Encode();
		asnWriter.Reset();
		new CertificationRequestAsn
		{
			CertificationRequestInfo = certificationRequestInfo,
			SignatureAlgorithm = signatureAlgorithm,
			SignatureValue = signatureGenerator.SignData(data, hashAlgorithm)
		}.Encode(asnWriter);
		return asnWriter.Encode();
	}
}
