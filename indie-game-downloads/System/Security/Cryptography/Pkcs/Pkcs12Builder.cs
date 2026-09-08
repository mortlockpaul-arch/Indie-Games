using System.Collections.Generic;
using System.Formats.Asn1;
using System.Security.Cryptography.Asn1.Pkcs7;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs;

internal sealed class Pkcs12Builder
{
	private ReadOnlyMemory<byte> _sealedData;

	private List<ContentInfoAsn> _contents;

	public bool IsSealed => !_sealedData.IsEmpty;

	public void AddSafeContentsEncrypted(Pkcs12SafeContents safeContents, ReadOnlySpan<char> password, PbeParameters pbeParameters)
	{
		ArgumentNullException.ThrowIfNull(safeContents, "safeContents");
		ArgumentNullException.ThrowIfNull(pbeParameters, "pbeParameters");
		if (pbeParameters.IterationCount < 1)
		{
			throw new ArgumentOutOfRangeException("pbeParameters");
		}
		if (safeContents.ConfidentialityMode != Pkcs12ConfidentialityMode.None)
		{
			throw new ArgumentException(System.SR.Cryptography_Pkcs12_CannotProcessEncryptedSafeContents, "safeContents");
		}
		if (IsSealed)
		{
			throw new InvalidOperationException(System.SR.Cryptography_Pkcs12_PfxIsSealed);
		}
		PasswordBasedEncryption.ValidatePbeParameters(pbeParameters, password, ReadOnlySpan<byte>.Empty);
		byte[] array = safeContents.Encrypt(password, ReadOnlySpan<byte>.Empty, pbeParameters);
		if (_contents == null)
		{
			_contents = new List<ContentInfoAsn>();
		}
		_contents.Add(new ContentInfoAsn
		{
			ContentType = "1.2.840.113549.1.7.6",
			Content = array
		});
	}

	public void AddSafeContentsUnencrypted(Pkcs12SafeContents safeContents)
	{
		ArgumentNullException.ThrowIfNull(safeContents, "safeContents");
		if (IsSealed)
		{
			throw new InvalidOperationException(System.SR.Cryptography_Pkcs12_PfxIsSealed);
		}
		if (_contents == null)
		{
			_contents = new List<ContentInfoAsn>();
		}
		_contents.Add(safeContents.EncodeToContentInfo());
	}

	public byte[] Encode()
	{
		if (!IsSealed)
		{
			throw new InvalidOperationException(System.SR.Cryptography_Pkcs12_PfxMustBeSealed);
		}
		return _sealedData.ToArray();
	}

	public void SealWithMac(ReadOnlySpan<char> password, HashAlgorithmName hashAlgorithm, int iterationCount)
	{
		if (iterationCount < 1)
		{
			throw new ArgumentOutOfRangeException("iterationCount");
		}
		if (IsSealed)
		{
			throw new InvalidOperationException(System.SR.Cryptography_Pkcs12_PfxIsSealed);
		}
		byte[] array = null;
		Span<byte> span = default(Span<byte>);
		byte[] array2 = null;
		Span<byte> span2 = default(Span<byte>);
		Span<byte> span3 = default(Span<byte>);
		try
		{
			AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.BER);
			using (IncrementalHash incrementalHash = IncrementalHash.CreateHash(hashAlgorithm))
			{
				asnWriter.PushSequence();
				if (_contents != null)
				{
					foreach (ContentInfoAsn content in _contents)
					{
						content.Encode(asnWriter);
					}
				}
				asnWriter.PopSequence();
				array = System.Security.Cryptography.CryptoPool.Rent(asnWriter.GetEncodedLength());
				if (!asnWriter.TryEncode(array, out var bytesWritten))
				{
					throw new InvalidOperationException();
				}
				span = array.AsSpan(0, bytesWritten);
				byte[] hashAndReset = incrementalHash.GetHashAndReset();
				array2 = System.Security.Cryptography.CryptoPool.Rent(hashAndReset.Length);
				span2 = array2.AsSpan(0, hashAndReset.Length);
				span3 = stackalloc byte[Math.Min(hashAndReset.Length, 128)];
				RandomNumberGenerator.Fill(span3);
				Pkcs12Kdf.DeriveMacKey(password, hashAlgorithm, iterationCount, span3, hashAndReset);
				using IncrementalHash incrementalHash2 = IncrementalHash.CreateHMAC(hashAlgorithm, hashAndReset);
				incrementalHash2.AppendData(span);
				if (!incrementalHash2.TryGetHashAndReset(span2, out var bytesWritten2) || bytesWritten2 != span2.Length)
				{
					throw new CryptographicException();
				}
			}
			AsnWriter asnWriter2 = new AsnWriter(AsnEncodingRules.BER);
			asnWriter2.PushSequence();
			asnWriter2.WriteInteger(3L);
			asnWriter2.PushSequence();
			asnWriter2.WriteObjectIdentifierForCrypto("1.2.840.113549.1.7.1");
			Asn1Tag value = new Asn1Tag(TagClass.ContextSpecific, 0);
			asnWriter2.PushSequence(value);
			asnWriter2.WriteOctetString(span);
			asnWriter2.PopSequence(value);
			asnWriter2.PopSequence();
			asnWriter2.PushSequence();
			asnWriter2.PushSequence();
			asnWriter2.PushSequence();
			asnWriter2.WriteObjectIdentifierForCrypto(PkcsHelpers.GetOidFromHashAlgorithm(hashAlgorithm));
			asnWriter2.PopSequence();
			asnWriter2.WriteOctetString(span2);
			asnWriter2.PopSequence();
			asnWriter2.WriteOctetString(span3);
			if (iterationCount > 1)
			{
				asnWriter2.WriteInteger(iterationCount);
			}
			asnWriter2.PopSequence();
			asnWriter2.PopSequence();
			_sealedData = asnWriter2.Encode();
		}
		finally
		{
			CryptographicOperations.ZeroMemory(span2);
			CryptographicOperations.ZeroMemory(span);
			if (array2 != null)
			{
				System.Security.Cryptography.CryptoPool.Return(array2, 0);
			}
			if (array != null)
			{
				System.Security.Cryptography.CryptoPool.Return(array, 0);
			}
		}
	}
}
