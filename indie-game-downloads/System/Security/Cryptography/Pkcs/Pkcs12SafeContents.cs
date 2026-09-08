using System.Collections.Generic;
using System.Formats.Asn1;
using System.Security.Cryptography.Asn1.Pkcs12;
using System.Security.Cryptography.Asn1.Pkcs7;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs12SafeContents
{
	private ReadOnlyMemory<byte> _encrypted;

	private List<Pkcs12SafeBag> _bags;

	public Pkcs12ConfidentialityMode ConfidentialityMode { get; private set; }

	public bool IsReadOnly { get; }

	public Pkcs12SafeContents()
	{
		ConfidentialityMode = Pkcs12ConfidentialityMode.None;
	}

	internal Pkcs12SafeContents(ReadOnlyMemory<byte> serialized)
	{
		IsReadOnly = true;
		ConfidentialityMode = Pkcs12ConfidentialityMode.None;
		_bags = ReadBags(serialized);
	}

	internal Pkcs12SafeContents(ContentInfoAsn contentInfoAsn)
	{
		IsReadOnly = true;
		switch (contentInfoAsn.ContentType)
		{
		case "1.2.840.113549.1.7.6":
			ConfidentialityMode = Pkcs12ConfidentialityMode.Password;
			_encrypted = contentInfoAsn.Content;
			break;
		case "1.2.840.113549.1.7.3":
			ConfidentialityMode = Pkcs12ConfidentialityMode.PublicKey;
			_encrypted = contentInfoAsn.Content;
			break;
		case "1.2.840.113549.1.7.1":
			ConfidentialityMode = Pkcs12ConfidentialityMode.None;
			_bags = ReadBags(PkcsHelpers.DecodeOctetStringAsMemory(contentInfoAsn.Content));
			break;
		default:
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
	}

	public void AddSafeBag(Pkcs12SafeBag safeBag)
	{
		ArgumentNullException.ThrowIfNull(safeBag, "safeBag");
		if (IsReadOnly)
		{
			throw new InvalidOperationException(System.SR.Cryptography_Pkcs12_SafeContentsIsReadOnly);
		}
		if (_bags == null)
		{
			_bags = new List<Pkcs12SafeBag>();
		}
		_bags.Add(safeBag);
	}

	public void Decrypt(ReadOnlySpan<char> password)
	{
		Decrypt(password, ReadOnlySpan<byte>.Empty);
	}

	private void Decrypt(ReadOnlySpan<char> password, ReadOnlySpan<byte> passwordBytes)
	{
		if (ConfidentialityMode != Pkcs12ConfidentialityMode.Password)
		{
			throw new InvalidOperationException(System.SR.Format(System.SR.Cryptography_Pkcs12_WrongModeForDecrypt, Pkcs12ConfidentialityMode.Password, ConfidentialityMode));
		}
		EncryptedDataAsn encryptedDataAsn = EncryptedDataAsn.Decode(_encrypted, AsnEncodingRules.BER);
		if (encryptedDataAsn.Version != 0 && encryptedDataAsn.Version != 2)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		if (encryptedDataAsn.EncryptedContentInfo.ContentType != "1.2.840.113549.1.7.1")
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		if (!encryptedDataAsn.EncryptedContentInfo.EncryptedContent.HasValue)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		byte[] array = new byte[encryptedDataAsn.EncryptedContentInfo.EncryptedContent.Value.Length];
		int length = PasswordBasedEncryption.Decrypt(in encryptedDataAsn.EncryptedContentInfo.ContentEncryptionAlgorithm, password, passwordBytes, encryptedDataAsn.EncryptedContentInfo.EncryptedContent.Value.Span, array);
		List<Pkcs12SafeBag> bags;
		try
		{
			bags = ReadBags(array.AsMemory(0, length));
		}
		catch
		{
			CryptographicOperations.ZeroMemory(array.AsSpan(0, length));
			throw;
		}
		_encrypted = ReadOnlyMemory<byte>.Empty;
		_bags = bags;
		ConfidentialityMode = Pkcs12ConfidentialityMode.None;
	}

	public IEnumerable<Pkcs12SafeBag> GetBags()
	{
		if (ConfidentialityMode != Pkcs12ConfidentialityMode.None)
		{
			throw new InvalidOperationException(System.SR.Cryptography_Pkcs12_SafeContentsIsEncrypted);
		}
		if (_bags == null)
		{
			return Array.Empty<Pkcs12SafeBag>();
		}
		return _bags.AsReadOnly();
	}

	private static List<Pkcs12SafeBag> ReadBags(ReadOnlyMemory<byte> serialized)
	{
		List<SafeBagAsn> list = new List<SafeBagAsn>();
		try
		{
			AsnValueReader asnValueReader = new AsnValueReader(serialized.Span, AsnEncodingRules.BER);
			AsnValueReader reader = asnValueReader.ReadSequence();
			asnValueReader.ThrowIfNotEmpty();
			while (reader.HasData)
			{
				SafeBagAsn.Decode(ref reader, serialized, out var decoded);
				list.Add(decoded);
			}
			if (list.Count == 0)
			{
				return new List<Pkcs12SafeBag>(0);
			}
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
		List<Pkcs12SafeBag> list2 = new List<Pkcs12SafeBag>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			ReadOnlyMemory<byte> bagValue = list[i].BagValue;
			Pkcs12SafeBag pkcs12SafeBag = null;
			try
			{
				switch (list[i].BagId)
				{
				case "1.2.840.113549.1.12.10.1.1":
					pkcs12SafeBag = new Pkcs12KeyBag(bagValue);
					break;
				case "1.2.840.113549.1.12.10.1.2":
					pkcs12SafeBag = new Pkcs12ShroudedKeyBag(bagValue);
					break;
				case "1.2.840.113549.1.12.10.1.3":
					pkcs12SafeBag = Pkcs12CertBag.DecodeValue(bagValue);
					break;
				case "1.2.840.113549.1.12.10.1.5":
					pkcs12SafeBag = Pkcs12SecretBag.DecodeValue(bagValue);
					break;
				case "1.2.840.113549.1.12.10.1.6":
					pkcs12SafeBag = Pkcs12SafeContentsBag.Decode(bagValue);
					break;
				case "1.2.840.113549.1.12.10.1.4":
					break;
				}
			}
			catch (AsnContentException)
			{
			}
			catch (CryptographicException)
			{
			}
			if (pkcs12SafeBag == null)
			{
				pkcs12SafeBag = new Pkcs12SafeBag.UnknownBag(list[i].BagId, bagValue);
			}
			pkcs12SafeBag.Attributes = PkcsHelpers.MakeAttributeCollection(list[i].BagAttributes);
			list2.Add(pkcs12SafeBag);
		}
		return list2;
	}

	internal byte[] Encrypt(ReadOnlySpan<char> password, ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
	{
		AsnWriter asnWriter = Encode();
		PasswordBasedEncryption.InitiateEncryption(pbeParameters, out var cipher, out var hmacOid, out var encryptionAlgorithmOid, out var isPkcs);
		int num = cipher.BlockSize / 8;
		byte[] array = System.Security.Cryptography.CryptoPool.Rent(asnWriter.GetEncodedLength() + num);
		Span<byte> span = Span<byte>.Empty;
		Span<byte> span2 = stackalloc byte[num];
		Span<byte> span3 = stackalloc byte[16];
		RandomNumberGenerator.Fill(span3);
		try
		{
			int length = PasswordBasedEncryption.Encrypt(password, passwordBytes, cipher, isPkcs, asnWriter, pbeParameters, span3, array, span2);
			span = array.AsSpan(0, length);
			AsnWriter asnWriter2 = new AsnWriter(AsnEncodingRules.DER);
			asnWriter2.PushSequence();
			asnWriter2.WriteInteger(0L);
			asnWriter2.PushSequence();
			asnWriter2.WriteObjectIdentifierForCrypto("1.2.840.113549.1.7.1");
			PasswordBasedEncryption.WritePbeAlgorithmIdentifier(asnWriter2, isPkcs, encryptionAlgorithmOid, span3, pbeParameters.IterationCount, hmacOid, span2);
			asnWriter2.WriteOctetString(span, new Asn1Tag(TagClass.ContextSpecific, 0));
			asnWriter2.PopSequence();
			asnWriter2.PopSequence();
			return asnWriter2.Encode();
		}
		finally
		{
			CryptographicOperations.ZeroMemory(span);
			System.Security.Cryptography.CryptoPool.Return(array, 0);
		}
	}

	internal AsnWriter Encode()
	{
		AsnWriter asnWriter;
		if (ConfidentialityMode == Pkcs12ConfidentialityMode.Password || ConfidentialityMode == Pkcs12ConfidentialityMode.PublicKey)
		{
			asnWriter = new AsnWriter(AsnEncodingRules.BER);
			asnWriter.WriteEncodedValueForCrypto(_encrypted.Span);
			return asnWriter;
		}
		asnWriter = new AsnWriter(AsnEncodingRules.BER);
		asnWriter.PushSequence();
		if (_bags != null)
		{
			foreach (Pkcs12SafeBag bag in _bags)
			{
				bag.EncodeTo(asnWriter);
			}
		}
		asnWriter.PopSequence();
		return asnWriter;
	}

	internal ContentInfoAsn EncodeToContentInfo()
	{
		AsnWriter asnWriter = Encode();
		if (ConfidentialityMode == Pkcs12ConfidentialityMode.None)
		{
			AsnWriter asnWriter2 = new AsnWriter(AsnEncodingRules.DER);
			using (asnWriter2.PushOctetString())
			{
				asnWriter.CopyTo(asnWriter2);
			}
			return new ContentInfoAsn
			{
				ContentType = "1.2.840.113549.1.7.1",
				Content = asnWriter2.Encode()
			};
		}
		if (ConfidentialityMode == Pkcs12ConfidentialityMode.Password)
		{
			return new ContentInfoAsn
			{
				ContentType = "1.2.840.113549.1.7.6",
				Content = asnWriter.Encode()
			};
		}
		if (ConfidentialityMode == Pkcs12ConfidentialityMode.PublicKey)
		{
			return new ContentInfoAsn
			{
				ContentType = "1.2.840.113549.1.7.3",
				Content = asnWriter.Encode()
			};
		}
		throw new CryptographicException();
	}
}
