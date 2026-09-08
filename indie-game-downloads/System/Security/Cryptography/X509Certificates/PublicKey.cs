using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography.X509Certificates;

public sealed class PublicKey
{
	private readonly Oid _oid;

	private AsymmetricAlgorithm _key;

	public AsnEncodedData EncodedKeyValue { get; }

	public AsnEncodedData? EncodedParameters { get; }

	[Obsolete("PublicKey.Key is obsolete. Use the appropriate method to get the public key, such as GetRSAPublicKey.", DiagnosticId = "SYSLIB0027", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public AsymmetricAlgorithm Key
	{
		get
		{
			if (_key == null)
			{
				string value = _oid.Value;
				if (!(value == "1.2.840.113549.1.1.1") && !(value == "1.2.840.10040.4.1"))
				{
					throw new NotSupportedException(System.SR.NotSupported_KeyAlgorithm);
				}
				_key = X509Pal.Instance.DecodePublicKey(_oid, EncodedKeyValue.RawData, EncodedParameters?.RawData, null);
			}
			return _key;
		}
	}

	public Oid Oid => _oid;

	public PublicKey(Oid oid, AsnEncodedData? parameters, AsnEncodedData keyValue)
		: this(oid, parameters, keyValue, skipCopy: false)
	{
	}

	internal PublicKey(Oid oid, AsnEncodedData parameters, AsnEncodedData keyValue, bool skipCopy)
	{
		_oid = oid;
		if (skipCopy)
		{
			EncodedParameters = parameters;
			EncodedKeyValue = keyValue;
		}
		else
		{
			EncodedParameters = ((parameters == null) ? null : new AsnEncodedData(parameters));
			EncodedKeyValue = new AsnEncodedData(keyValue);
		}
	}

	public PublicKey(AsymmetricAlgorithm key)
		: this(key.ExportSubjectPublicKeyInfo())
	{
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public PublicKey(MLKem key)
		: this(key.ExportSubjectPublicKeyInfo())
	{
	}

	[Experimental("SYSLIB5006")]
	public PublicKey(MLDsa key)
		: this(key.ExportSubjectPublicKeyInfo())
	{
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	public PublicKey(SlhDsa key)
		: this(key.ExportSubjectPublicKeyInfo())
	{
	}

	[Experimental("SYSLIB5006")]
	public PublicKey(CompositeMLDsa key)
		: this(key.ExportSubjectPublicKeyInfo())
	{
	}

	private PublicKey(byte[] subjectPublicKeyInfo)
	{
		DecodeSubjectPublicKeyInfo(subjectPublicKeyInfo, out var oid, out var parameters, out var keyValue);
		_oid = oid;
		EncodedParameters = parameters;
		EncodedKeyValue = keyValue;
	}

	public bool TryExportSubjectPublicKeyInfo(Span<byte> destination, out int bytesWritten)
	{
		return EncodeSubjectPublicKeyInfo().TryEncode(destination, out bytesWritten);
	}

	public byte[] ExportSubjectPublicKeyInfo()
	{
		return EncodeSubjectPublicKeyInfo().Encode();
	}

	public static PublicKey CreateFromSubjectPublicKeyInfo(ReadOnlySpan<byte> source, out int bytesRead)
	{
		int num = DecodeSubjectPublicKeyInfo(source, out var oid, out var parameters, out var keyValue);
		bytesRead = num;
		return new PublicKey(oid, parameters, keyValue, skipCopy: true);
	}

	[UnsupportedOSPlatform("browser")]
	public RSA? GetRSAPublicKey()
	{
		if (_oid.Value != "1.2.840.113549.1.1.1")
		{
			return null;
		}
		RSA rSA = RSA.Create();
		try
		{
			rSA.ImportSubjectPublicKeyInfo(ExportSubjectPublicKeyInfo(), out var _);
			return rSA;
		}
		catch
		{
			rSA.Dispose();
			throw;
		}
	}

	[UnsupportedOSPlatform("browser")]
	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("tvos")]
	public DSA? GetDSAPublicKey()
	{
		if (_oid.Value != "1.2.840.10040.4.1")
		{
			return null;
		}
		DSA dSA = DSA.Create();
		try
		{
			dSA.ImportSubjectPublicKeyInfo(ExportSubjectPublicKeyInfo(), out var _);
			return dSA;
		}
		catch
		{
			dSA.Dispose();
			throw;
		}
	}

	[UnsupportedOSPlatform("browser")]
	public ECDsa? GetECDsaPublicKey()
	{
		if (_oid.Value != "1.2.840.10045.2.1")
		{
			return null;
		}
		ECDsa eCDsa = ECDsa.Create();
		try
		{
			eCDsa.ImportSubjectPublicKeyInfo(ExportSubjectPublicKeyInfo(), out var _);
			return eCDsa;
		}
		catch
		{
			eCDsa.Dispose();
			throw;
		}
	}

	[UnsupportedOSPlatform("browser")]
	public ECDiffieHellman? GetECDiffieHellmanPublicKey()
	{
		if (_oid.Value != "1.2.840.10045.2.1")
		{
			return null;
		}
		ECDiffieHellman eCDiffieHellman = ECDiffieHellman.Create();
		try
		{
			eCDiffieHellman.ImportSubjectPublicKeyInfo(ExportSubjectPublicKeyInfo(), out var _);
			return eCDiffieHellman;
		}
		catch
		{
			eCDiffieHellman.Dispose();
			throw;
		}
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[UnsupportedOSPlatform("browser")]
	public MLKem? GetMLKemPublicKey()
	{
		if ((object)MLKemAlgorithm.FromOid(_oid.Value) == null)
		{
			return null;
		}
		return EncodeSubjectPublicKeyInfo().Encode<MLKem>((Func<ReadOnlySpan<byte>, MLKem>)MLKem.ImportSubjectPublicKeyInfo);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[UnsupportedOSPlatform("browser")]
	public MLDsa? GetMLDsaPublicKey()
	{
		if ((object)MLDsaAlgorithm.GetMLDsaAlgorithmFromOid(_oid.Value) == null)
		{
			return null;
		}
		return EncodeSubjectPublicKeyInfo().Encode<MLDsa>((Func<ReadOnlySpan<byte>, MLDsa>)MLDsa.ImportSubjectPublicKeyInfo);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[UnsupportedOSPlatform("browser")]
	public SlhDsa? GetSlhDsaPublicKey()
	{
		if (!Helpers.IsSlhDsaOid(_oid.Value))
		{
			return null;
		}
		return EncodeSubjectPublicKeyInfo().Encode<SlhDsa>((Func<ReadOnlySpan<byte>, SlhDsa>)SlhDsa.ImportSubjectPublicKeyInfo);
	}

	[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[UnsupportedOSPlatform("browser")]
	public CompositeMLDsa? GetCompositeMLDsaPublicKey()
	{
		if ((object)CompositeMLDsaAlgorithm.GetAlgorithmFromOid(_oid.Value) == null)
		{
			return null;
		}
		return EncodeSubjectPublicKeyInfo().Encode<CompositeMLDsa>((Func<ReadOnlySpan<byte>, CompositeMLDsa>)CompositeMLDsa.ImportSubjectPublicKeyInfo);
	}

	internal AsnWriter EncodeSubjectPublicKeyInfo()
	{
		SubjectPublicKeyInfoAsn subjectPublicKeyInfoAsn = new SubjectPublicKeyInfoAsn
		{
			Algorithm = new AlgorithmIdentifierAsn
			{
				Algorithm = (_oid.Value ?? string.Empty),
				Parameters = EncodedParameters?.RawData.ToNullableMemory()
			},
			SubjectPublicKey = EncodedKeyValue.RawData
		};
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
		subjectPublicKeyInfoAsn.Encode(asnWriter);
		return asnWriter;
	}

	private unsafe static int DecodeSubjectPublicKeyInfo(ReadOnlySpan<byte> source, out Oid oid, out AsnEncodedData parameters, out AsnEncodedData keyValue)
	{
		fixed (byte* reference = &MemoryMarshal.GetReference(source))
		{
			using MemoryManager<byte> memoryManager = new PointerMemoryManager<byte>(reference, source.Length);
			AsnValueReader reader = new AsnValueReader(source, AsnEncodingRules.DER);
			int length;
			SubjectPublicKeyInfoAsn decoded;
			try
			{
				length = reader.PeekEncodedValue().Length;
				SubjectPublicKeyInfoAsn.Decode(ref reader, memoryManager.Memory, out decoded);
			}
			catch (AsnContentException inner)
			{
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding, inner);
			}
			DecodeSubjectPublicKeyInfo(ref decoded, out oid, out parameters, out keyValue);
			return length;
		}
	}

	internal static PublicKey DecodeSubjectPublicKeyInfo(ref SubjectPublicKeyInfoAsn spki)
	{
		DecodeSubjectPublicKeyInfo(ref spki, out var oid, out var parameters, out var keyValue);
		return new PublicKey(oid, parameters, keyValue, skipCopy: true);
	}

	private static void DecodeSubjectPublicKeyInfo(ref SubjectPublicKeyInfoAsn spki, out Oid oid, out AsnEncodedData parameters, out AsnEncodedData keyValue)
	{
		oid = new Oid(spki.Algorithm.Algorithm, null);
		keyValue = new AsnEncodedData(spki.SubjectPublicKey.Span);
		ReadOnlyMemory<byte>? parameters2 = spki.Algorithm.Parameters;
		AsnEncodedData asnEncodedData = ((!parameters2.HasValue) ? null : new AsnEncodedData(parameters2.GetValueOrDefault().Span));
		parameters = asnEncodedData;
	}
}
