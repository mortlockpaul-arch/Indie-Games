using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Runtime.Versioning;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.X509Certificates.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography.X509Certificates;

[UnsupportedOSPlatform("browser")]
public sealed class CertificateRequest
{
	private readonly object _key;

	private readonly X509SignatureGenerator _generator;

	private readonly RSASignaturePadding _rsaPadding;

	public X500DistinguishedName SubjectName { get; }

	public Collection<X509Extension> CertificateExtensions { get; } = new Collection<X509Extension>();

	public Collection<AsnEncodedData> OtherRequestAttributes { get; } = new Collection<AsnEncodedData>();

	public PublicKey PublicKey { get; }

	public HashAlgorithmName HashAlgorithm { get; }

	public CertificateRequest(string subjectName, ECDsa key, HashAlgorithmName hashAlgorithm)
	{
		ArgumentNullException.ThrowIfNull(subjectName, "subjectName");
		ArgumentNullException.ThrowIfNull(key, "key");
		ArgumentException.ThrowIfNullOrEmpty(hashAlgorithm.Name, "hashAlgorithm");
		SubjectName = new X500DistinguishedName(subjectName);
		_key = key;
		_generator = X509SignatureGenerator.CreateForECDsa(key);
		PublicKey = _generator.PublicKey;
		HashAlgorithm = hashAlgorithm;
	}

	public CertificateRequest(X500DistinguishedName subjectName, ECDsa key, HashAlgorithmName hashAlgorithm)
	{
		ArgumentNullException.ThrowIfNull(subjectName, "subjectName");
		ArgumentNullException.ThrowIfNull(key, "key");
		ArgumentException.ThrowIfNullOrEmpty(hashAlgorithm.Name, "hashAlgorithm");
		SubjectName = subjectName;
		_key = key;
		_generator = X509SignatureGenerator.CreateForECDsa(key);
		PublicKey = _generator.PublicKey;
		HashAlgorithm = hashAlgorithm;
	}

	public CertificateRequest(string subjectName, RSA key, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
	{
		ArgumentNullException.ThrowIfNull(subjectName, "subjectName");
		ArgumentNullException.ThrowIfNull(key, "key");
		ArgumentException.ThrowIfNullOrEmpty(hashAlgorithm.Name, "hashAlgorithm");
		ArgumentNullException.ThrowIfNull(padding, "padding");
		SubjectName = new X500DistinguishedName(subjectName);
		_key = key;
		_generator = X509SignatureGenerator.CreateForRSA(key, padding);
		_rsaPadding = padding;
		PublicKey = _generator.PublicKey;
		HashAlgorithm = hashAlgorithm;
	}

	public CertificateRequest(X500DistinguishedName subjectName, RSA key, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
	{
		ArgumentNullException.ThrowIfNull(subjectName, "subjectName");
		ArgumentNullException.ThrowIfNull(key, "key");
		ArgumentException.ThrowIfNullOrEmpty(hashAlgorithm.Name, "hashAlgorithm");
		ArgumentNullException.ThrowIfNull(padding, "padding");
		SubjectName = subjectName;
		_key = key;
		_generator = X509SignatureGenerator.CreateForRSA(key, padding);
		_rsaPadding = padding;
		PublicKey = _generator.PublicKey;
		HashAlgorithm = hashAlgorithm;
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public CertificateRequest(string subjectName, MLDsa key)
	{
		ArgumentNullException.ThrowIfNull(subjectName, "subjectName");
		ArgumentNullException.ThrowIfNull(key, "key");
		SubjectName = new X500DistinguishedName(subjectName);
		_key = key;
		_generator = X509SignatureGenerator.CreateForMLDsa(key);
		PublicKey = _generator.PublicKey;
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public CertificateRequest(X500DistinguishedName subjectName, MLDsa key)
	{
		ArgumentNullException.ThrowIfNull(subjectName, "subjectName");
		ArgumentNullException.ThrowIfNull(key, "key");
		SubjectName = subjectName;
		_key = key;
		_generator = X509SignatureGenerator.CreateForMLDsa(key);
		PublicKey = _generator.PublicKey;
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public CertificateRequest(string subjectName, SlhDsa key)
	{
		ArgumentNullException.ThrowIfNull(subjectName, "subjectName");
		ArgumentNullException.ThrowIfNull(key, "key");
		SubjectName = new X500DistinguishedName(subjectName);
		_key = key;
		_generator = X509SignatureGenerator.CreateForSlhDsa(key);
		PublicKey = _generator.PublicKey;
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public CertificateRequest(X500DistinguishedName subjectName, SlhDsa key)
	{
		ArgumentNullException.ThrowIfNull(subjectName, "subjectName");
		ArgumentNullException.ThrowIfNull(key, "key");
		SubjectName = subjectName;
		_key = key;
		_generator = X509SignatureGenerator.CreateForSlhDsa(key);
		PublicKey = _generator.PublicKey;
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public CertificateRequest(string subjectName, CompositeMLDsa key)
	{
		ArgumentNullException.ThrowIfNull(subjectName, "subjectName");
		ArgumentNullException.ThrowIfNull(key, "key");
		throw new PlatformNotSupportedException();
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public CertificateRequest(X500DistinguishedName subjectName, CompositeMLDsa key)
	{
		ArgumentNullException.ThrowIfNull(subjectName, "subjectName");
		ArgumentNullException.ThrowIfNull(key, "key");
		throw new PlatformNotSupportedException();
	}

	public CertificateRequest(X500DistinguishedName subjectName, PublicKey publicKey, HashAlgorithmName hashAlgorithm)
	{
		ArgumentNullException.ThrowIfNull(subjectName, "subjectName");
		ArgumentNullException.ThrowIfNull(publicKey, "publicKey");
		SubjectName = subjectName;
		PublicKey = publicKey;
		HashAlgorithm = hashAlgorithm;
	}

	public CertificateRequest(X500DistinguishedName subjectName, PublicKey publicKey, HashAlgorithmName hashAlgorithm, RSASignaturePadding? rsaSignaturePadding = null)
	{
		ArgumentNullException.ThrowIfNull(subjectName, "subjectName");
		ArgumentNullException.ThrowIfNull(publicKey, "publicKey");
		SubjectName = subjectName;
		PublicKey = publicKey;
		HashAlgorithm = hashAlgorithm;
		_rsaPadding = rsaSignaturePadding;
	}

	public byte[] CreateSigningRequest()
	{
		if (_generator == null)
		{
			throw new InvalidOperationException(System.SR.Cryptography_CertReq_NoKeyProvided);
		}
		return CreateSigningRequest(_generator);
	}

	public byte[] CreateSigningRequest(X509SignatureGenerator signatureGenerator)
	{
		ArgumentNullException.ThrowIfNull(signatureGenerator, "signatureGenerator");
		if (string.IsNullOrEmpty(HashAlgorithm.Name) && Helpers.HashAlgorithmRequired(signatureGenerator.PublicKey.Oid.Value))
		{
			throw new InvalidOperationException(System.SR.Cryptography_CertReq_NoHashAlgorithmProvided);
		}
		X501Attribute[] array = Array.Empty<X501Attribute>();
		bool flag = CertificateExtensions.Count > 0;
		if ((OtherRequestAttributes.Count > 0) | flag)
		{
			array = new X501Attribute[OtherRequestAttributes.Count + (flag ? 1 : 0)];
		}
		int num = 0;
		foreach (AsnEncodedData otherRequestAttribute in OtherRequestAttributes)
		{
			if (otherRequestAttribute == null)
			{
				throw new InvalidOperationException(System.SR.Format(System.SR.Cryptography_CertReq_NullValueInCollection, "OtherRequestAttributes"));
			}
			if (otherRequestAttribute.Oid == null || otherRequestAttribute.Oid.Value == null)
			{
				throw new InvalidOperationException(System.SR.Format(System.SR.Cryptography_CertReq_MissingOidInCollection, "OtherRequestAttributes"));
			}
			if (otherRequestAttribute.Oid.Value == "1.2.840.113549.1.9.14")
			{
				throw new InvalidOperationException(System.SR.Cryptography_CertReq_ExtensionRequestInOtherAttributes);
			}
			Helpers.ValidateDer(otherRequestAttribute.RawData);
			array[num] = new X501Attribute(otherRequestAttribute.Oid.Value, otherRequestAttribute.RawData);
			num++;
		}
		if (flag)
		{
			array[num] = new Pkcs9ExtensionRequest(CertificateExtensions);
		}
		return new Pkcs10CertificationRequestInfo(SubjectName, PublicKey, array).ToPkcs10Request(signatureGenerator, HashAlgorithm);
	}

	public string CreateSigningRequestPem()
	{
		byte[] array = CreateSigningRequest();
		return PemEncoding.WriteString("CERTIFICATE REQUEST".AsSpan(), array);
	}

	public string CreateSigningRequestPem(X509SignatureGenerator signatureGenerator)
	{
		ArgumentNullException.ThrowIfNull(signatureGenerator, "signatureGenerator");
		byte[] array = CreateSigningRequest(signatureGenerator);
		return PemEncoding.WriteString("CERTIFICATE REQUEST".AsSpan(), array);
	}

	public X509Certificate2 CreateSelfSigned(DateTimeOffset notBefore, DateTimeOffset notAfter)
	{
		if (notAfter < notBefore)
		{
			throw new ArgumentException(System.SR.Cryptography_CertReq_DatesReversed);
		}
		if (_key == null)
		{
			throw new InvalidOperationException(System.SR.Cryptography_CertReq_NoKeyProvided);
		}
		Span<byte> span = stackalloc byte[8];
		RandomNumberGenerator.Fill(span);
		using X509Certificate2 x509Certificate = Create(SubjectName, _generator, notBefore, notAfter, span);
		object key = _key;
		if (!(key is RSA privateKey))
		{
			if (!(key is ECDsa privateKey2))
			{
				if (!(key is MLDsa privateKey3))
				{
					if (key is SlhDsa privateKey4)
					{
						return x509Certificate.CopyWithPrivateKey(privateKey4);
					}
					throw new CryptographicException();
				}
				return x509Certificate.CopyWithPrivateKey(privateKey3);
			}
			return x509Certificate.CopyWithPrivateKey(privateKey2);
		}
		return x509Certificate.CopyWithPrivateKey(privateKey);
	}

	public X509Certificate2 Create(X509Certificate2 issuerCertificate, DateTimeOffset notBefore, DateTimeOffset notAfter, byte[] serialNumber)
	{
		return Create(issuerCertificate, notBefore, notAfter, new ReadOnlySpan<byte>(serialNumber));
	}

	public X509Certificate2 Create(X509Certificate2 issuerCertificate, DateTimeOffset notBefore, DateTimeOffset notAfter, ReadOnlySpan<byte> serialNumber)
	{
		ArgumentNullException.ThrowIfNull(issuerCertificate, "issuerCertificate");
		if (!issuerCertificate.HasPrivateKey)
		{
			throw new ArgumentException(System.SR.Cryptography_CertReq_IssuerRequiresPrivateKey, "issuerCertificate");
		}
		if (notAfter < notBefore)
		{
			throw new ArgumentException(System.SR.Cryptography_CertReq_DatesReversed);
		}
		if (serialNumber.IsEmpty)
		{
			throw new ArgumentException(System.SR.Arg_EmptyOrNullArray, "serialNumber");
		}
		if (issuerCertificate.PublicKey.Oid.Value != PublicKey.Oid.Value)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Cryptography_CertReq_AlgorithmMustMatch, issuerCertificate.PublicKey.Oid.Value, PublicKey.Oid.Value), "issuerCertificate");
		}
		DateTime localDateTime = notBefore.LocalDateTime;
		if (localDateTime < issuerCertificate.NotBefore)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Cryptography_CertReq_NotBeforeNotNested, localDateTime, issuerCertificate.NotBefore), "notBefore");
		}
		DateTime localDateTime2 = notAfter.LocalDateTime;
		long ticks = localDateTime2.Ticks;
		long num = ticks % 10000000;
		ticks -= num;
		localDateTime2 = new DateTime(ticks, localDateTime2.Kind);
		if (localDateTime2 > issuerCertificate.NotAfter)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Cryptography_CertReq_NotAfterNotNested, localDateTime2, issuerCertificate.NotAfter), "notAfter");
		}
		X509BasicConstraintsExtension obj = (X509BasicConstraintsExtension)issuerCertificate.Extensions["2.5.29.19"];
		X509KeyUsageExtension x509KeyUsageExtension = (X509KeyUsageExtension)issuerCertificate.Extensions["2.5.29.15"];
		if (obj == null)
		{
			throw new ArgumentException(System.SR.Cryptography_CertReq_BasicConstraintsRequired, "issuerCertificate");
		}
		if (!obj.CertificateAuthority)
		{
			throw new ArgumentException(System.SR.Cryptography_CertReq_IssuerBasicConstraintsInvalid, "issuerCertificate");
		}
		if (x509KeyUsageExtension != null && (x509KeyUsageExtension.KeyUsages & X509KeyUsageFlags.KeyCertSign) == 0)
		{
			throw new ArgumentException(System.SR.Cryptography_CertReq_IssuerKeyUsageInvalid, "issuerCertificate");
		}
		AsymmetricAlgorithm asymmetricAlgorithm = null;
		string keyAlgorithm = issuerCertificate.GetKeyAlgorithm();
		try
		{
			X509SignatureGenerator generator;
			if (!(keyAlgorithm == "1.2.840.113549.1.1.1"))
			{
				if (!(keyAlgorithm == "1.2.840.10045.2.1"))
				{
					throw new ArgumentException(System.SR.Format(System.SR.Cryptography_UnknownKeyAlgorithm, keyAlgorithm), "issuerCertificate");
				}
				generator = X509SignatureGenerator.CreateForECDsa((ECDsa)(asymmetricAlgorithm = issuerCertificate.GetECDsaPrivateKey()));
			}
			else
			{
				if (_rsaPadding == null)
				{
					throw new InvalidOperationException(System.SR.Cryptography_CertReq_RSAPaddingRequired);
				}
				generator = X509SignatureGenerator.CreateForRSA((RSA)(asymmetricAlgorithm = issuerCertificate.GetRSAPrivateKey()), _rsaPadding);
			}
			return Create(issuerCertificate.SubjectName, generator, notBefore, notAfter, serialNumber);
		}
		finally
		{
			asymmetricAlgorithm?.Dispose();
		}
	}

	public X509Certificate2 Create(X500DistinguishedName issuerName, X509SignatureGenerator generator, DateTimeOffset notBefore, DateTimeOffset notAfter, byte[] serialNumber)
	{
		return Create(issuerName, generator, notBefore, notAfter, new ReadOnlySpan<byte>(serialNumber));
	}

	public X509Certificate2 Create(X500DistinguishedName issuerName, X509SignatureGenerator generator, DateTimeOffset notBefore, DateTimeOffset notAfter, ReadOnlySpan<byte> serialNumber)
	{
		ArgumentNullException.ThrowIfNull(issuerName, "issuerName");
		ArgumentNullException.ThrowIfNull(generator, "generator");
		if (notAfter < notBefore)
		{
			throw new ArgumentException(System.SR.Cryptography_CertReq_DatesReversed);
		}
		if (serialNumber.Length < 1)
		{
			throw new ArgumentException(System.SR.Arg_EmptyOrNullArray, "serialNumber");
		}
		if (string.IsNullOrEmpty(HashAlgorithm.Name) && Helpers.HashAlgorithmRequired(generator.PublicKey.Oid.Value))
		{
			throw new InvalidOperationException(System.SR.Cryptography_CertReq_NoHashAlgorithmProvided);
		}
		AlgorithmIdentifierAsn signatureAlgorithm = AlgorithmIdentifierAsn.Decode(generator.GetSignatureAlgorithmIdentifier(HashAlgorithm), AsnEncodingRules.DER);
		if (signatureAlgorithm.Parameters.HasValue)
		{
			Helpers.ValidateDer(signatureAlgorithm.Parameters.Value.Span);
		}
		ArraySegment<byte> arraySegment = NormalizeSerialNumber(serialNumber);
		TbsCertificateAsn tbsCertificate = new TbsCertificateAsn
		{
			Version = 2,
			SerialNumber = arraySegment,
			SignatureAlgorithm = signatureAlgorithm,
			Issuer = issuerName.RawData,
			SubjectPublicKeyInfo = new SubjectPublicKeyInfoAsn
			{
				Algorithm = new AlgorithmIdentifierAsn
				{
					Algorithm = PublicKey.Oid.Value,
					Parameters = PublicKey.EncodedParameters?.RawData.ToNullableMemory()
				},
				SubjectPublicKey = PublicKey.EncodedKeyValue.RawData
			},
			Validity = new ValidityAsn(notBefore, notAfter),
			Subject = SubjectName.RawData
		};
		if (CertificateExtensions.Count > 0)
		{
			HashSet<string> hashSet = new HashSet<string>(CertificateExtensions.Count);
			List<X509ExtensionAsn> list = new List<X509ExtensionAsn>(CertificateExtensions.Count);
			foreach (X509Extension certificateExtension in CertificateExtensions)
			{
				if (certificateExtension != null)
				{
					if (!hashSet.Add(certificateExtension.Oid.Value))
					{
						throw new InvalidOperationException(System.SR.Format(System.SR.Cryptography_CertReq_DuplicateExtension, certificateExtension.Oid.Value));
					}
					list.Add(new X509ExtensionAsn(certificateExtension));
				}
			}
			if (list.Count > 0)
			{
				tbsCertificate.Extensions = list.ToArray();
			}
		}
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
		tbsCertificate.Encode(asnWriter);
		byte[] data = asnWriter.Encode();
		asnWriter.Reset();
		new CertificateAsn
		{
			TbsCertificate = tbsCertificate,
			SignatureAlgorithm = signatureAlgorithm,
			SignatureValue = generator.SignData(data, HashAlgorithm)
		}.Encode(asnWriter);
		X509Certificate2 result = asnWriter.Encode<X509Certificate2>((Func<ReadOnlySpan<byte>, X509Certificate2>)X509CertificateLoader.LoadCertificate);
		System.Security.Cryptography.CryptoPool.Return(arraySegment);
		return result;
	}

	private static ArraySegment<byte> NormalizeSerialNumber(ReadOnlySpan<byte> serialNumber)
	{
		byte[] array;
		if (serialNumber[0] >= 128)
		{
			array = System.Security.Cryptography.CryptoPool.Rent(serialNumber.Length + 1);
			array[0] = 0;
			serialNumber.CopyTo(array.AsSpan(1));
			return new ArraySegment<byte>(array, 0, serialNumber.Length + 1);
		}
		int i;
		for (i = 0; i < serialNumber.Length - 1 && serialNumber[i] == 0 && serialNumber[i + 1] < 128; i++)
		{
		}
		int num = serialNumber.Length - i;
		array = System.Security.Cryptography.CryptoPool.Rent(num);
		serialNumber.Slice(i).CopyTo(array);
		return new ArraySegment<byte>(array, 0, num);
	}

	public static CertificateRequest LoadSigningRequestPem(string pkcs10Pem, HashAlgorithmName signerHashAlgorithm, CertificateRequestLoadOptions options = CertificateRequestLoadOptions.Default, RSASignaturePadding? signerSignaturePadding = null)
	{
		ArgumentNullException.ThrowIfNull(pkcs10Pem, "pkcs10Pem");
		return LoadSigningRequestPem(pkcs10Pem.AsSpan(), signerHashAlgorithm, options, signerSignaturePadding);
	}

	public static CertificateRequest LoadSigningRequestPem(ReadOnlySpan<char> pkcs10Pem, HashAlgorithmName signerHashAlgorithm, CertificateRequestLoadOptions options = CertificateRequestLoadOptions.Default, RSASignaturePadding? signerSignaturePadding = null)
	{
		if ((options & ~(CertificateRequestLoadOptions.SkipSignatureValidation | CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions)) != CertificateRequestLoadOptions.Default)
		{
			throw new ArgumentOutOfRangeException("options", options, System.SR.Argument_InvalidFlag);
		}
		PemEnumerator<char>.Enumerator enumerator = PemEnumerator.Utf16(pkcs10Pem).GetEnumerator();
		while (enumerator.MoveNext())
		{
			var (readOnlySpan2, pemFields2) = enumerator.Current;
			Range label = pemFields2.Label;
			if (readOnlySpan2[label.Start..label.End].SequenceEqual("CERTIFICATE REQUEST".AsSpan()))
			{
				byte[] array = ArrayPool<byte>.Shared.Rent(pemFields2.DecodedDataLength);
				label = pemFields2.Base64Data;
				int length = readOnlySpan2.Length;
				int offset = label.Start.GetOffset(length);
				int bytesConsumed = label.End.GetOffset(length) - offset;
				if (!Convert.TryFromBase64Chars(readOnlySpan2.Slice(offset, bytesConsumed), array, out var bytesWritten) || bytesWritten != pemFields2.DecodedDataLength)
				{
					throw new UnreachableException();
				}
				try
				{
					return LoadSigningRequest(array.AsSpan(0, bytesWritten), permitTrailingData: false, signerHashAlgorithm, out bytesConsumed, options, signerSignaturePadding);
				}
				finally
				{
					ArrayPool<byte>.Shared.Return(array);
				}
			}
		}
		throw new CryptographicException(System.SR.Format(System.SR.Cryptography_NoPemOfLabel, "CERTIFICATE REQUEST"));
	}

	public static CertificateRequest LoadSigningRequest(byte[] pkcs10, HashAlgorithmName signerHashAlgorithm, CertificateRequestLoadOptions options = CertificateRequestLoadOptions.Default, RSASignaturePadding? signerSignaturePadding = null)
	{
		ArgumentNullException.ThrowIfNull(pkcs10, "pkcs10");
		int bytesConsumed;
		return LoadSigningRequest(pkcs10, permitTrailingData: false, signerHashAlgorithm, out bytesConsumed, options, signerSignaturePadding);
	}

	public static CertificateRequest LoadSigningRequest(ReadOnlySpan<byte> pkcs10, HashAlgorithmName signerHashAlgorithm, out int bytesConsumed, CertificateRequestLoadOptions options = CertificateRequestLoadOptions.Default, RSASignaturePadding? signerSignaturePadding = null)
	{
		return LoadSigningRequest(pkcs10, permitTrailingData: true, signerHashAlgorithm, out bytesConsumed, options, signerSignaturePadding);
	}

	private unsafe static CertificateRequest LoadSigningRequest(ReadOnlySpan<byte> pkcs10, bool permitTrailingData, HashAlgorithmName signerHashAlgorithm, out int bytesConsumed, CertificateRequestLoadOptions options, RSASignaturePadding signerSignaturePadding)
	{
		if ((options & ~(CertificateRequestLoadOptions.SkipSignatureValidation | CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions)) != CertificateRequestLoadOptions.Default)
		{
			throw new ArgumentOutOfRangeException("options", options, System.SR.Argument_InvalidFlag);
		}
		bool flag = (options & CertificateRequestLoadOptions.SkipSignatureValidation) != 0;
		bool flag2 = (options & CertificateRequestLoadOptions.UnsafeLoadCertificateExtensions) != 0;
		try
		{
			AsnValueReader asnValueReader = new AsnValueReader(pkcs10, AsnEncodingRules.DER);
			int length = asnValueReader.PeekEncodedValue().Length;
			AsnValueReader reader = asnValueReader.ReadSequence();
			if (!permitTrailingData)
			{
				asnValueReader.ThrowIfNotEmpty();
			}
			CertificateRequest certificateRequest;
			fixed (byte* pointer = pkcs10)
			{
				using PointerMemoryManager<byte> pointerMemoryManager = new PointerMemoryManager<byte>(pointer, length);
				ReadOnlyMemory<byte> rebind = pointerMemoryManager.Memory;
				ReadOnlySpan<byte> toBeSigned = reader.PeekEncodedValue();
				CertificationRequestInfoAsn.Decode(ref reader, rebind, out var decoded);
				AlgorithmIdentifierAsn.Decode(ref reader, rebind, out var decoded2);
				if (!reader.TryReadPrimitiveBitString(out var unusedBitCount, out var value))
				{
					throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
				}
				reader.ThrowIfNotEmpty();
				if (decoded.Version < 0L)
				{
					throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
				}
				if (decoded.Version != 0L)
				{
					throw new CryptographicException(System.SR.Format(System.SR.Cryptography_CertReq_Load_VersionTooNew, decoded.Version, 0));
				}
				PublicKey publicKey = System.Security.Cryptography.X509Certificates.PublicKey.DecodeSubjectPublicKeyInfo(ref decoded.SubjectPublicKeyInfo);
				if (!flag && (unusedBitCount != 0 || !VerifyX509Signature(toBeSigned, value, publicKey, decoded2)))
				{
					throw new CryptographicException(System.SR.Cryptography_CertReq_SignatureVerificationFailed);
				}
				certificateRequest = new CertificateRequest(new X500DistinguishedName(decoded.Subject.Span), publicKey, signerHashAlgorithm, signerSignaturePadding);
				if (decoded.Attributes != null)
				{
					bool flag3 = false;
					AttributeAsn[] attributes = decoded.Attributes;
					for (int i = 0; i < attributes.Length; i++)
					{
						AttributeAsn attributeAsn = attributes[i];
						if (attributeAsn.AttrType == "1.2.840.113549.1.9.14")
						{
							if (flag3)
							{
								throw new CryptographicException(System.SR.Cryptography_CertReq_Load_DuplicateExtensionRequests);
							}
							flag3 = true;
							if (attributeAsn.AttrValues.Length != 1)
							{
								throw new CryptographicException(System.SR.Cryptography_CertReq_Load_DuplicateExtensionRequests);
							}
							AsnValueReader asnValueReader2 = new AsnValueReader(attributeAsn.AttrValues[0].Span, AsnEncodingRules.DER);
							AsnValueReader reader2 = asnValueReader2.ReadSequence();
							asnValueReader2.ThrowIfNotEmpty();
							do
							{
								X509ExtensionAsn.Decode(ref reader2, rebind, out var decoded3);
								if (flag2)
								{
									X509Extension x509Extension = new X509Extension(decoded3.ExtnId, decoded3.ExtnValue.Span, decoded3.Critical);
									X509Extension x509Extension2 = X509Certificate2.CreateCustomExtensionIfAny(decoded3.ExtnId);
									if (x509Extension2 != null)
									{
										x509Extension2.CopyFrom(x509Extension);
										certificateRequest.CertificateExtensions.Add(x509Extension2);
									}
									else
									{
										certificateRequest.CertificateExtensions.Add(x509Extension);
									}
								}
							}
							while (reader2.HasData);
						}
						else
						{
							if (attributeAsn.AttrValues.Length == 0)
							{
								throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
							}
							ReadOnlyMemory<byte>[] attrValues = attributeAsn.AttrValues;
							foreach (ReadOnlyMemory<byte> readOnlyMemory in attrValues)
							{
								certificateRequest.OtherRequestAttributes.Add(new AsnEncodedData(attributeAsn.AttrType, readOnlyMemory.Span));
							}
						}
					}
				}
			}
			bytesConsumed = length;
			return certificateRequest;
		}
		catch (AsnContentException inner)
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
		}
	}

	private static bool VerifyX509Signature(ReadOnlySpan<byte> toBeSigned, ReadOnlySpan<byte> signature, PublicKey publicKey, AlgorithmIdentifierAsn algorithmIdentifier)
	{
		RSA rSAPublicKey = publicKey.GetRSAPublicKey();
		ECDsa eCDsaPublicKey = publicKey.GetECDsaPublicKey();
		MLDsa mLDsaPublicKey = publicKey.GetMLDsaPublicKey();
		SlhDsa slhDsaPublicKey = publicKey.GetSlhDsaPublicKey();
		try
		{
			HashAlgorithmName hashAlgorithm;
			if (algorithmIdentifier.Algorithm == "1.2.840.113549.1.1.10")
			{
				if (rSAPublicKey == null || !algorithmIdentifier.Parameters.HasValue)
				{
					return false;
				}
				PssParamsAsn pssParamsAsn = PssParamsAsn.Decode(algorithmIdentifier.Parameters.GetValueOrDefault(), AsnEncodingRules.DER);
				RSASignaturePadding signaturePadding = pssParamsAsn.GetSignaturePadding();
				hashAlgorithm = HashAlgorithmName.FromOid(pssParamsAsn.HashAlgorithm.Algorithm);
				return rSAPublicKey.VerifyData(toBeSigned, signature, hashAlgorithm, signaturePadding);
			}
			string algorithm = algorithmIdentifier.Algorithm;
			switch (algorithm)
			{
			default:
				if (algorithm != null && Helpers.IsSlhDsaOid(algorithm))
				{
					hashAlgorithm = default(HashAlgorithmName);
					break;
				}
				throw new NotSupportedException(System.SR.Format(System.SR.Cryptography_UnknownKeyAlgorithm, algorithmIdentifier.Algorithm));
			case "1.2.840.113549.1.1.11":
			case "1.2.840.10045.4.3.2":
				hashAlgorithm = HashAlgorithmName.SHA256;
				break;
			case "1.2.840.113549.1.1.12":
			case "1.2.840.10045.4.3.3":
				hashAlgorithm = HashAlgorithmName.SHA384;
				break;
			case "1.2.840.113549.1.1.13":
			case "1.2.840.10045.4.3.4":
				hashAlgorithm = HashAlgorithmName.SHA512;
				break;
			case "1.2.840.113549.1.1.5":
			case "1.2.840.10045.4.1":
				hashAlgorithm = HashAlgorithmName.SHA1;
				break;
			case "2.16.840.1.101.3.4.3.17":
			case "2.16.840.1.101.3.4.3.18":
			case "2.16.840.1.101.3.4.3.19":
				hashAlgorithm = default(HashAlgorithmName);
				break;
			}
			if (!algorithmIdentifier.HasNullEquivalentParameters())
			{
				return false;
			}
			string algorithm2 = algorithmIdentifier.Algorithm;
			switch (algorithm2)
			{
			default:
				if (algorithm2 != null && Helpers.IsSlhDsaOid(algorithm2))
				{
					return slhDsaPublicKey?.VerifyData(toBeSigned, signature) ?? false;
				}
				return false;
			case "1.2.840.113549.1.1.11":
			case "1.2.840.113549.1.1.12":
			case "1.2.840.113549.1.1.13":
			case "1.2.840.113549.1.1.5":
				return rSAPublicKey?.VerifyData(toBeSigned, signature, hashAlgorithm, RSASignaturePadding.Pkcs1) ?? false;
			case "1.2.840.10045.4.3.2":
			case "1.2.840.10045.4.3.3":
			case "1.2.840.10045.4.3.4":
			case "1.2.840.10045.4.1":
				return eCDsaPublicKey?.VerifyData(toBeSigned, signature, hashAlgorithm, DSASignatureFormat.Rfc3279DerSequence) ?? false;
			case "2.16.840.1.101.3.4.3.17":
			case "2.16.840.1.101.3.4.3.18":
			case "2.16.840.1.101.3.4.3.19":
				return mLDsaPublicKey?.VerifyData(toBeSigned, signature) ?? false;
			}
		}
		catch (AsnContentException)
		{
			return false;
		}
		catch (CryptographicException)
		{
			return false;
		}
		finally
		{
			rSAPublicKey?.Dispose();
			eCDsaPublicKey?.Dispose();
			mLDsaPublicKey?.Dispose();
			slhDsaPublicKey?.Dispose();
		}
	}
}
