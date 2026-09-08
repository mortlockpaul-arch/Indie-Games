using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Formats.Asn1;
using System.Security.Cryptography.Asn1.Pkcs12;
using System.Security.Cryptography.Asn1.Pkcs7;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs12Info
{
	private PfxAsn _decoded;

	private ReadOnlyMemory<byte> _authSafeContents;

	public ReadOnlyCollection<Pkcs12SafeContents> AuthenticatedSafe { get; private set; }

	public Pkcs12IntegrityMode IntegrityMode { get; private set; }

	private Pkcs12Info()
	{
	}

	public bool VerifyMac(ReadOnlySpan<char> password)
	{
		if (IntegrityMode != Pkcs12IntegrityMode.Password)
		{
			throw new InvalidOperationException(System.SR.Format(System.SR.Cryptography_Pkcs12_WrongModeForVerify, Pkcs12IntegrityMode.Password, IntegrityMode));
		}
		return _decoded.VerifyMac(password, _authSafeContents.Span);
	}

	public static Pkcs12Info Decode(ReadOnlyMemory<byte> encodedBytes, out int bytesConsumed, bool skipCopy = false)
	{
		int num = PkcsHelpers.FirstBerValueLength(encodedBytes.Span);
		ReadOnlyMemory<byte> readOnlyMemory = encodedBytes.Slice(0, num);
		PfxAsn decoded = PfxAsn.Decode(skipCopy ? readOnlyMemory : ((ReadOnlyMemory<byte>)readOnlyMemory.ToArray()), AsnEncodingRules.BER);
		if (decoded.Version != 3)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		ReadOnlyMemory<byte> readOnlyMemory2 = ReadOnlyMemory<byte>.Empty;
		Pkcs12IntegrityMode pkcs12IntegrityMode = Pkcs12IntegrityMode.Unknown;
		if (decoded.AuthSafe.ContentType == "1.2.840.113549.1.7.1")
		{
			readOnlyMemory2 = PkcsHelpers.DecodeOctetStringAsMemory(decoded.AuthSafe.Content);
			pkcs12IntegrityMode = ((!decoded.MacData.HasValue) ? Pkcs12IntegrityMode.None : Pkcs12IntegrityMode.Password);
		}
		else if (decoded.AuthSafe.ContentType == "1.2.840.113549.1.7.2")
		{
			SignedDataAsn signedDataAsn = SignedDataAsn.Decode(decoded.AuthSafe.Content, AsnEncodingRules.BER);
			pkcs12IntegrityMode = Pkcs12IntegrityMode.PublicKey;
			if (signedDataAsn.EncapContentInfo.ContentType == "1.2.840.113549.1.7.1")
			{
				readOnlyMemory2 = signedDataAsn.EncapContentInfo.Content.GetValueOrDefault();
			}
			if (decoded.MacData.HasValue)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
		}
		if (pkcs12IntegrityMode == Pkcs12IntegrityMode.Unknown)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		List<ContentInfoAsn> list = new List<ContentInfoAsn>();
		try
		{
			AsnValueReader asnValueReader = new AsnValueReader(readOnlyMemory2.Span, AsnEncodingRules.BER);
			AsnValueReader reader = asnValueReader.ReadSequence();
			asnValueReader.ThrowIfNotEmpty();
			while (reader.HasData)
			{
				ContentInfoAsn.Decode(ref reader, readOnlyMemory2, out var decoded2);
				list.Add(decoded2);
			}
			ReadOnlyCollection<Pkcs12SafeContents> authenticatedSafe;
			if (list.Count == 0)
			{
				authenticatedSafe = new ReadOnlyCollection<Pkcs12SafeContents>(Array.Empty<Pkcs12SafeContents>());
			}
			else
			{
				Pkcs12SafeContents[] array = new Pkcs12SafeContents[list.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new Pkcs12SafeContents(list[i]);
				}
				authenticatedSafe = new ReadOnlyCollection<Pkcs12SafeContents>(array);
			}
			bytesConsumed = num;
			return new Pkcs12Info
			{
				AuthenticatedSafe = authenticatedSafe,
				IntegrityMode = pkcs12IntegrityMode,
				_decoded = decoded,
				_authSafeContents = readOnlyMemory2
			};
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
	}
}
