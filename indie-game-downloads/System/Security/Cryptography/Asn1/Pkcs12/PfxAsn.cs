using System.Formats.Asn1;
using System.Security.Cryptography.Asn1.Pkcs7;
using System.Security.Cryptography.Pkcs;

namespace System.Security.Cryptography.Asn1.Pkcs12;

internal struct PfxAsn
{
	internal int Version;

	internal ContentInfoAsn AuthSafe;

	internal MacData? MacData;

	internal bool VerifyMac(ReadOnlySpan<char> macPassword, ReadOnlySpan<byte> authSafeContents)
	{
		string algorithm = MacData.Value.Mac.DigestAlgorithm.Algorithm;
		int num;
		HashAlgorithmName hashAlgorithm;
		switch (algorithm)
		{
		case "1.2.840.113549.2.5":
			num = 16;
			hashAlgorithm = HashAlgorithmName.MD5;
			break;
		case "1.3.14.3.2.26":
			num = 20;
			hashAlgorithm = HashAlgorithmName.SHA1;
			break;
		case "2.16.840.1.101.3.4.2.1":
			num = 32;
			hashAlgorithm = HashAlgorithmName.SHA256;
			break;
		case "2.16.840.1.101.3.4.2.2":
			num = 48;
			hashAlgorithm = HashAlgorithmName.SHA384;
			break;
		case "2.16.840.1.101.3.4.2.3":
			num = 64;
			hashAlgorithm = HashAlgorithmName.SHA512;
			break;
		default:
			throw new CryptographicException(System.SR.Format(System.SR.Cryptography_UnknownHashAlgorithm, algorithm));
		}
		if (MacData.Value.Mac.Digest.Length != num)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		Span<byte> span = stackalloc byte[num];
		int iterationCount = PasswordBasedEncryption.NormalizeIterationCount(MacData.Value.IterationCount);
		Pkcs12Kdf.DeriveMacKey(macPassword, hashAlgorithm, iterationCount, MacData.Value.MacSalt.Span, span);
		using IncrementalHash incrementalHash = IncrementalHash.CreateHMAC(hashAlgorithm, span);
		incrementalHash.AppendData(authSafeContents);
		if (!incrementalHash.TryGetHashAndReset(span, out var bytesWritten) || bytesWritten != num)
		{
			throw new CryptographicException();
		}
		return CryptographicOperations.FixedTimeEquals(span, MacData.Value.Mac.Digest.Span);
	}

	internal static PfxAsn Decode(ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
	{
		return Decode(Asn1Tag.Sequence, encoded, ruleSet);
	}

	internal static PfxAsn Decode(Asn1Tag expectedTag, ReadOnlyMemory<byte> encoded, AsnEncodingRules ruleSet)
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

	private static void DecodeCore(ref AsnValueReader reader, Asn1Tag expectedTag, ReadOnlyMemory<byte> rebind, out PfxAsn decoded)
	{
		decoded = default(PfxAsn);
		AsnValueReader reader2 = reader.ReadSequence(expectedTag);
		if (!reader2.TryReadInt32(out decoded.Version))
		{
			reader2.ThrowIfNotEmpty();
		}
		ContentInfoAsn.Decode(ref reader2, rebind, out decoded.AuthSafe);
		if (reader2.HasData && reader2.PeekTag().HasSameClassAndValue(Asn1Tag.Sequence))
		{
			System.Security.Cryptography.Asn1.Pkcs12.MacData.Decode(ref reader2, rebind, out var decoded2);
			decoded.MacData = decoded2;
		}
		reader2.ThrowIfNotEmpty();
	}
}
