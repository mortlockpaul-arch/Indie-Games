using System.Buffers;
using System.Formats.Asn1;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;

namespace System.Security.Cryptography;

internal static class RSAKeyFormatHelper
{
	internal delegate TRet RSAParametersCallback<TRet>(RSAParameters parameters);

	private static readonly string[] s_validOids = new string[1] { "1.2.840.113549.1.1.1" };

	internal static void FromPkcs1PrivateKey(ReadOnlyMemory<byte> keyData, in AlgorithmIdentifierAsn algId, out RSAParameters ret)
	{
		if (!algId.HasNullEquivalentParameters())
		{
			throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
		}
		ret = FromPkcs1PrivateKey(keyData, (RSAParameters rsaParameters) => rsaParameters, pinAndClearParameters: false);
	}

	internal static void ReadRsaPublicKey(ReadOnlyMemory<byte> keyData, in AlgorithmIdentifierAsn algId, out RSAParameters ret)
	{
		ret = FromPkcs1PublicKey(keyData, (RSAParameters rsaParameters) => rsaParameters);
	}

	internal static ReadOnlyMemory<byte> ReadSubjectPublicKeyInfo(ReadOnlyMemory<byte> source, out int bytesRead)
	{
		return KeyFormatHelper.ReadSubjectPublicKeyInfo(s_validOids, source, out bytesRead);
	}

	internal static ReadOnlyMemory<byte> ReadPkcs8(ReadOnlyMemory<byte> source, out int bytesRead)
	{
		return KeyFormatHelper.ReadPkcs8(s_validOids, source, out bytesRead);
	}

	internal static AsnWriter WriteSubjectPublicKeyInfo(ReadOnlySpan<byte> pkcs1PublicKey)
	{
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
		asnWriter.PushSequence();
		WriteAlgorithmIdentifier(asnWriter);
		asnWriter.WriteBitString(pkcs1PublicKey);
		asnWriter.PopSequence();
		return asnWriter;
	}

	internal static AsnWriter WritePkcs8PrivateKey(ReadOnlySpan<byte> pkcs1PrivateKey, AsnWriter copyFrom = null)
	{
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.BER);
		using (asnWriter.PushSequence())
		{
			asnWriter.WriteInteger(0L);
			WriteAlgorithmIdentifier(asnWriter);
			if (copyFrom != null)
			{
				using (asnWriter.PushOctetString())
				{
					copyFrom.CopyTo(asnWriter);
				}
			}
			else
			{
				asnWriter.WriteOctetString(pkcs1PrivateKey);
			}
		}
		return asnWriter;
	}

	internal static AsnWriter WritePkcs8PrivateKey(in RSAParameters rsaParameters)
	{
		AsnWriter copyFrom = WritePkcs1PrivateKey(in rsaParameters);
		return WritePkcs8PrivateKey(ReadOnlySpan<byte>.Empty, copyFrom);
	}

	private static void WriteAlgorithmIdentifier(AsnWriter writer)
	{
		writer.PushSequence();
		writer.WriteObjectIdentifier("1.2.840.113549.1.1.1");
		writer.WriteNull();
		writer.PopSequence();
	}

	internal static void ReadEncryptedPkcs8(ReadOnlySpan<byte> source, ReadOnlySpan<char> password, out int bytesRead, out RSAParameters key)
	{
		KeyFormatHelper.ReadEncryptedPkcs8(s_validOids, source, password, (KeyFormatHelper.KeyReader<RSAParameters>)FromPkcs1PrivateKey, out bytesRead, out key);
	}

	internal static void ReadEncryptedPkcs8(ReadOnlySpan<byte> source, ReadOnlySpan<byte> passwordBytes, out int bytesRead, out RSAParameters key)
	{
		KeyFormatHelper.ReadEncryptedPkcs8(s_validOids, source, passwordBytes, (KeyFormatHelper.KeyReader<RSAParameters>)FromPkcs1PrivateKey, out bytesRead, out key);
	}

	internal unsafe static TRet FromPkcs1PrivateKey<TRet>(ReadOnlySpan<byte> keyData, RSAParametersCallback<TRet> parametersReader, bool pinAndClearParameters = true)
	{
		fixed (byte* reference = &MemoryMarshal.GetReference(keyData))
		{
			using MemoryManager<byte> memoryManager = new PointerMemoryManager<byte>(reference, keyData.Length);
			return FromPkcs1PrivateKey(memoryManager.Memory, parametersReader, pinAndClearParameters);
		}
	}

	internal static TRet FromPkcs1PrivateKey<TRet>(ReadOnlyMemory<byte> keyData, RSAParametersCallback<TRet> parametersReader, bool pinAndClearParameters = true)
	{
		RSAPrivateKeyAsn key = RSAPrivateKeyAsn.Decode(keyData, AsnEncodingRules.BER);
		if (key.Version > 0)
		{
			throw new CryptographicException(System.SR.Format(System.SR.Cryptography_RSAPrivateKey_VersionTooNew, key.Version, 0));
		}
		byte[] array = key.Modulus.ToUnsignedIntegerBytes();
		int num = (array.Length + 1) / 2;
		RSAParameters parameters = new RSAParameters
		{
			Modulus = array,
			Exponent = key.PublicExponent.ToUnsignedIntegerBytes(),
			D = new byte[array.Length],
			P = new byte[num],
			Q = new byte[num],
			DP = new byte[num],
			DQ = new byte[num],
			InverseQ = new byte[num]
		};
		if (pinAndClearParameters)
		{
			using (PinAndClear.Track(parameters.D))
			{
				using (PinAndClear.Track(parameters.P))
				{
					using (PinAndClear.Track(parameters.Q))
					{
						using (PinAndClear.Track(parameters.DP))
						{
							using (PinAndClear.Track(parameters.DQ))
							{
								using (PinAndClear.Track(parameters.InverseQ))
								{
									return ExtractParametersWithCallback(parametersReader, ref key, ref parameters);
								}
							}
						}
					}
				}
			}
		}
		return ExtractParametersWithCallback(parametersReader, ref key, ref parameters);
		static TRet ExtractParametersWithCallback(RSAParametersCallback<TRet> rSAParametersCallback, ref RSAPrivateKeyAsn reference, ref RSAParameters reference2)
		{
			reference.PrivateExponent.ToUnsignedIntegerBytes(reference2.D);
			reference.Prime1.ToUnsignedIntegerBytes(reference2.P);
			reference.Prime2.ToUnsignedIntegerBytes(reference2.Q);
			reference.Exponent1.ToUnsignedIntegerBytes(reference2.DP);
			reference.Exponent2.ToUnsignedIntegerBytes(reference2.DQ);
			reference.Coefficient.ToUnsignedIntegerBytes(reference2.InverseQ);
			return rSAParametersCallback(reference2);
		}
	}

	internal static TRet FromPkcs1PublicKey<TRet>(ReadOnlyMemory<byte> keyData, RSAParametersCallback<TRet> parametersReader)
	{
		RSAPublicKeyAsn rSAPublicKeyAsn = RSAPublicKeyAsn.Decode(keyData, AsnEncodingRules.BER);
		RSAParameters parameters = new RSAParameters
		{
			Modulus = rSAPublicKeyAsn.Modulus.ToUnsignedIntegerBytes(),
			Exponent = rSAPublicKeyAsn.PublicExponent.ToUnsignedIntegerBytes()
		};
		return parametersReader(parameters);
	}

	internal static AsnWriter WritePkcs1PublicKey(in RSAParameters rsaParameters)
	{
		if (rsaParameters.Modulus == null || rsaParameters.Exponent == null)
		{
			throw new CryptographicException(System.SR.Cryptography_InvalidRsaParameters);
		}
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
		asnWriter.PushSequence();
		asnWriter.WriteKeyParameterInteger(rsaParameters.Modulus);
		asnWriter.WriteKeyParameterInteger(rsaParameters.Exponent);
		asnWriter.PopSequence();
		return asnWriter;
	}

	internal static AsnWriter WritePkcs1PrivateKey(in RSAParameters rsaParameters)
	{
		if (rsaParameters.Modulus == null || rsaParameters.Exponent == null)
		{
			throw new CryptographicException(System.SR.Cryptography_InvalidRsaParameters);
		}
		if (rsaParameters.D == null || rsaParameters.P == null || rsaParameters.Q == null || rsaParameters.DP == null || rsaParameters.DQ == null || rsaParameters.InverseQ == null)
		{
			throw new CryptographicException(System.SR.Cryptography_NotValidPrivateKey);
		}
		AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
		asnWriter.PushSequence();
		asnWriter.WriteInteger(0L);
		asnWriter.WriteKeyParameterInteger(rsaParameters.Modulus);
		asnWriter.WriteKeyParameterInteger(rsaParameters.Exponent);
		asnWriter.WriteKeyParameterInteger(rsaParameters.D);
		asnWriter.WriteKeyParameterInteger(rsaParameters.P);
		asnWriter.WriteKeyParameterInteger(rsaParameters.Q);
		asnWriter.WriteKeyParameterInteger(rsaParameters.DP);
		asnWriter.WriteKeyParameterInteger(rsaParameters.DQ);
		asnWriter.WriteKeyParameterInteger(rsaParameters.InverseQ);
		asnWriter.PopSequence();
		return asnWriter;
	}
}
