using System.Formats.Asn1;
using System.Security.Cryptography.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs8PrivateKeyInfo
{
	public Oid AlgorithmId { get; }

	public ReadOnlyMemory<byte>? AlgorithmParameters { get; }

	public CryptographicAttributeObjectCollection Attributes { get; }

	public ReadOnlyMemory<byte> PrivateKeyBytes { get; }

	private Pkcs8PrivateKeyInfo(Oid algorithmId, ReadOnlyMemory<byte>? algorithmParameters, ReadOnlyMemory<byte> privateKey, CryptographicAttributeObjectCollection attributes)
	{
		AlgorithmId = algorithmId;
		AlgorithmParameters = algorithmParameters;
		PrivateKeyBytes = privateKey;
		Attributes = attributes;
	}

	public static Pkcs8PrivateKeyInfo Decode(ReadOnlyMemory<byte> source, out int bytesRead, bool skipCopy = false)
	{
		try
		{
			AsnValueReader reader = new AsnValueReader(source.Span, AsnEncodingRules.BER);
			ReadOnlyMemory<byte> rebind = (skipCopy ? source : default(ReadOnlyMemory<byte>));
			int length = reader.PeekEncodedValue().Length;
			PrivateKeyInfoAsn.Decode(ref reader, rebind, out var decoded);
			bytesRead = length;
			return new Pkcs8PrivateKeyInfo(new Oid(decoded.PrivateKeyAlgorithm.Algorithm, null), decoded.PrivateKeyAlgorithm.Parameters, decoded.PrivateKey, PkcsHelpers.MakeAttributeCollection(decoded.Attributes));
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
	}

	public byte[] Encrypt(ReadOnlySpan<char> password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, password, ReadOnlySpan<byte>.Empty);
		AsnWriter pkcs8Writer = WritePkcs8();
		return KeyFormatHelper.WriteEncryptedPkcs8(password, pkcs8Writer, pbeParameters).Encode();
	}

	public static Pkcs8PrivateKeyInfo DecryptAndDecode(ReadOnlySpan<char> password, ReadOnlyMemory<byte> source, out int bytesRead)
	{
		ArraySegment<byte> arraySegment = KeyFormatHelper.DecryptPkcs8(password, source, out var bytesRead2);
		Memory<byte> memory = arraySegment;
		try
		{
			Pkcs8PrivateKeyInfo result = Decode(memory, out var bytesRead3);
			if (bytesRead3 != memory.Length)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
			bytesRead = bytesRead2;
			return result;
		}
		finally
		{
			System.Security.Cryptography.CryptoPool.Return(arraySegment);
		}
	}

	private AsnWriter WritePkcs8()
	{
		PrivateKeyInfoAsn privateKeyInfoAsn = new PrivateKeyInfoAsn
		{
			PrivateKeyAlgorithm = 
			{
				Algorithm = AlgorithmId.Value
			},
			PrivateKey = PrivateKeyBytes
		};
		ReadOnlyMemory<byte>? algorithmParameters = AlgorithmParameters;
		if (algorithmParameters.HasValue && algorithmParameters.GetValueOrDefault().Length > 0)
		{
			privateKeyInfoAsn.PrivateKeyAlgorithm.Parameters = AlgorithmParameters;
		}
		if (Attributes.Count > 0)
		{
			privateKeyInfoAsn.Attributes = PkcsHelpers.NormalizeAttributeSet(PkcsHelpers.BuildAttributes(Attributes).ToArray());
		}
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.BER);
		privateKeyInfoAsn.Encode(asnWriter);
		return asnWriter;
	}
}
