using System.Formats.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography.X509Certificates;

public sealed class X509SubjectKeyIdentifierExtension : X509Extension
{
	private byte[] _subjectKeyIdentifierBytes;

	private string _subjectKeyIdentifierString;

	private bool _decoded;

	public string? SubjectKeyIdentifier
	{
		get
		{
			if (!_decoded)
			{
				Decode(base.RawData);
			}
			return _subjectKeyIdentifierString;
		}
	}

	public ReadOnlyMemory<byte> SubjectKeyIdentifierBytes
	{
		get
		{
			if (_subjectKeyIdentifierBytes == null)
			{
				Decode(base.RawData);
			}
			return _subjectKeyIdentifierBytes;
		}
	}

	public X509SubjectKeyIdentifierExtension()
		: base(Oids.SubjectKeyIdentifierOid)
	{
		_decoded = true;
	}

	public X509SubjectKeyIdentifierExtension(AsnEncodedData encodedSubjectKeyIdentifier, bool critical)
		: base(Oids.SubjectKeyIdentifierOid, encodedSubjectKeyIdentifier.RawData, critical)
	{
	}

	public X509SubjectKeyIdentifierExtension(byte[] subjectKeyIdentifier, bool critical)
		: this((ReadOnlySpan<byte>)(subjectKeyIdentifier ?? throw new ArgumentNullException("subjectKeyIdentifier")), critical)
	{
	}

	public X509SubjectKeyIdentifierExtension(ReadOnlySpan<byte> subjectKeyIdentifier, bool critical)
		: base(Oids.SubjectKeyIdentifierOid, EncodeExtension(subjectKeyIdentifier), critical, skipCopy: true)
	{
	}

	public X509SubjectKeyIdentifierExtension(PublicKey key, bool critical)
		: this(key, X509SubjectKeyIdentifierHashAlgorithm.Sha1, critical)
	{
	}

	public X509SubjectKeyIdentifierExtension(PublicKey key, X509SubjectKeyIdentifierHashAlgorithm algorithm, bool critical)
		: base(Oids.SubjectKeyIdentifierOid, EncodeExtension(key, algorithm), critical, skipCopy: true)
	{
	}

	public X509SubjectKeyIdentifierExtension(string subjectKeyIdentifier, bool critical)
		: base(Oids.SubjectKeyIdentifierOid, EncodeExtension(subjectKeyIdentifier), critical, skipCopy: true)
	{
	}

	public override void CopyFrom(AsnEncodedData asnEncodedData)
	{
		base.CopyFrom(asnEncodedData);
		_decoded = false;
	}

	private void Decode(byte[] rawData)
	{
		_subjectKeyIdentifierBytes = DecodeX509SubjectKeyIdentifierExtension(rawData);
		_subjectKeyIdentifierString = _subjectKeyIdentifierBytes.ToHexStringUpper();
		_decoded = true;
	}

	internal static byte[] DecodeX509SubjectKeyIdentifierExtension(byte[] encoded)
	{
		ReadOnlySpan<byte> value;
		try
		{
			if (!AsnDecoder.TryReadPrimitiveOctetString(encoded, AsnEncodingRules.BER, out value, out var bytesConsumed) || bytesConsumed != encoded.Length)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
		return value.ToArray();
	}

	private static byte[] EncodeExtension(ReadOnlySpan<byte> subjectKeyIdentifier)
	{
		if (subjectKeyIdentifier.Length == 0)
		{
			throw new ArgumentException(System.SR.Arg_EmptyOrNullArray, "subjectKeyIdentifier");
		}
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
		asnWriter.WriteOctetString(subjectKeyIdentifier);
		return asnWriter.Encode();
	}

	private static byte[] EncodeExtension(string subjectKeyIdentifier)
	{
		ArgumentNullException.ThrowIfNull(subjectKeyIdentifier, "subjectKeyIdentifier");
		return EncodeExtension(subjectKeyIdentifier.LaxDecodeHexString());
	}

	private static byte[] EncodeExtension(PublicKey key, X509SubjectKeyIdentifierHashAlgorithm algorithm)
	{
		ArgumentNullException.ThrowIfNull(key, "key");
		return EncodeExtension(GenerateSubjectKeyIdentifierFromPublicKey(key, algorithm));
	}

	private static byte[] GenerateSubjectKeyIdentifierFromPublicKey(PublicKey key, X509SubjectKeyIdentifierHashAlgorithm algorithm)
	{
		switch (algorithm)
		{
		case X509SubjectKeyIdentifierHashAlgorithm.Sha1:
			return SHA1.HashData(key.EncodedKeyValue.RawData);
		case X509SubjectKeyIdentifierHashAlgorithm.ShortSha1:
		{
			Span<byte> destination = stackalloc byte[20];
			SHA1.HashData(key.EncodedKeyValue.RawData, destination);
			byte[] array = destination.Slice(12).ToArray();
			array[0] &= 15;
			array[0] |= 64;
			return array;
		}
		case X509SubjectKeyIdentifierHashAlgorithm.CapiSha1:
			return HashSubjectPublicKeyInfo(key, HashAlgorithmName.SHA1);
		case X509SubjectKeyIdentifierHashAlgorithm.Sha256:
			return HashSubjectPublicKeyInfo(key, HashAlgorithmName.SHA256);
		case X509SubjectKeyIdentifierHashAlgorithm.Sha384:
			return HashSubjectPublicKeyInfo(key, HashAlgorithmName.SHA384);
		case X509SubjectKeyIdentifierHashAlgorithm.Sha512:
			return HashSubjectPublicKeyInfo(key, HashAlgorithmName.SHA512);
		case X509SubjectKeyIdentifierHashAlgorithm.ShortSha256:
			return HashSubjectPublicKeyLeft160Bits(key, HashAlgorithmName.SHA256);
		case X509SubjectKeyIdentifierHashAlgorithm.ShortSha384:
			return HashSubjectPublicKeyLeft160Bits(key, HashAlgorithmName.SHA384);
		case X509SubjectKeyIdentifierHashAlgorithm.ShortSha512:
			return HashSubjectPublicKeyLeft160Bits(key, HashAlgorithmName.SHA512);
		default:
			throw new ArgumentException(System.SR.Format(System.SR.Arg_EnumIllegalVal, algorithm), "algorithm");
		}
	}

	private static byte[] HashSubjectPublicKeyLeft160Bits(PublicKey key, HashAlgorithmName hashAlgorithmName)
	{
		Span<byte> destination = stackalloc byte[64];
		CryptographicOperations.HashData(hashAlgorithmName, key.EncodedKeyValue.RawData, destination);
		return destination.Slice(0, 20).ToArray();
	}

	private static byte[] HashSubjectPublicKeyInfo(PublicKey key, HashAlgorithmName hashAlgorithmName)
	{
		return key.EncodeSubjectPublicKeyInfo().Encode<HashAlgorithmName, byte[]>(hashAlgorithmName, (Func<HashAlgorithmName, ReadOnlySpan<byte>, byte[]>)((HashAlgorithmName hashAlgorithm, ReadOnlySpan<byte> encoded) => CryptographicOperations.HashData(hashAlgorithm, encoded)));
	}
}
