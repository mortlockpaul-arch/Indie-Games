using System.Formats.Asn1;

namespace System.Security.Cryptography.X509Certificates;

public sealed class X509EnhancedKeyUsageExtension : X509Extension
{
	private OidCollection _enhancedKeyUsages;

	private bool _decoded;

	public OidCollection EnhancedKeyUsages
	{
		get
		{
			if (!_decoded)
			{
				DecodeX509EnhancedKeyUsageExtension(base.RawData, out _enhancedKeyUsages);
				_decoded = true;
			}
			OidCollection oidCollection = new OidCollection(_enhancedKeyUsages.Count);
			foreach (Oid enhancedKeyUsage in _enhancedKeyUsages)
			{
				oidCollection.Add(enhancedKeyUsage);
			}
			return oidCollection;
		}
	}

	public X509EnhancedKeyUsageExtension()
		: base(Oids.EnhancedKeyUsageOid)
	{
		_enhancedKeyUsages = new OidCollection();
		_decoded = true;
	}

	public X509EnhancedKeyUsageExtension(AsnEncodedData encodedEnhancedKeyUsages, bool critical)
		: base(Oids.EnhancedKeyUsageOid, encodedEnhancedKeyUsages.RawData, critical)
	{
	}

	public X509EnhancedKeyUsageExtension(OidCollection enhancedKeyUsages, bool critical)
		: base(Oids.EnhancedKeyUsageOid, EncodeExtension(enhancedKeyUsages), critical, skipCopy: true)
	{
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		base.CopyFrom(asnEncodedData);
		_decoded = false;
	}

	private static byte[] EncodeExtension(OidCollection enhancedKeyUsages)
	{
		ArgumentNullException.ThrowIfNull(enhancedKeyUsages, "enhancedKeyUsages");
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
		using (asnWriter.PushSequence())
		{
			foreach (Oid enhancedKeyUsage in enhancedKeyUsages)
			{
				asnWriter.WriteObjectIdentifierForCrypto(enhancedKeyUsage.Value);
			}
		}
		return asnWriter.Encode();
	}

	private static void DecodeX509EnhancedKeyUsageExtension(byte[] encoded, out OidCollection usages)
	{
		try
		{
			AsnReader asnReader = new AsnReader(encoded, AsnEncodingRules.BER);
			AsnReader asnReader2 = asnReader.ReadSequence();
			asnReader.ThrowIfNotEmpty();
			usages = new OidCollection();
			while (asnReader2.HasData)
			{
				usages.Add(new Oid(asnReader2.ReadObjectIdentifier(), null));
			}
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
	}
}
