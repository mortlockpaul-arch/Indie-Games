using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Asn1;
using Internal.Cryptography;

namespace System.Security.Cryptography;

[Experimental("SYSLIB5006", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
internal sealed class CompositeMLDsaManaged : CompositeMLDsa
{
	private abstract class ComponentAlgorithm : IDisposable
	{
		private bool _disposed;

		internal abstract bool TryExportPublicKey(Span<byte> destination, out int bytesWritten);

		internal abstract bool TryExportPrivateKey(Span<byte> destination, out int bytesWritten);

		internal abstract int SignData(ReadOnlySpan<byte> data, Span<byte> destination);

		internal abstract bool VerifyData(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature);

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				Dispose(disposing: true);
				GC.SuppressFinalize(this);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
		}
	}

	private sealed class AlgorithmMetadata(MLDsaAlgorithm mldsaAlgorithm, object traditionalAlgorithm, byte[] label, HashAlgorithmName hashAlgorithmName)
	{
		internal MLDsaAlgorithm MLDsaAlgorithm { get; } = mldsaAlgorithm;

		internal object TraditionalAlgorithm { get; } = traditionalAlgorithm;

		internal byte[] Label { get; } = label;

		internal HashAlgorithmName HashAlgorithmName { get; } = hashAlgorithmName;
	}

	private sealed class RsaAlgorithm(int keySizeInBits, HashAlgorithmName hashAlgorithmName, RSASignaturePadding padding)
	{
		internal int KeySizeInBits { get; } = keySizeInBits;

		internal HashAlgorithmName HashAlgorithmName { get; } = hashAlgorithmName;

		internal RSASignaturePadding Padding { get; } = padding;
	}

	private sealed class ECDsaAlgorithm
	{
		internal int KeySizeInBits { get; }

		internal HashAlgorithmName HashAlgorithmName { get; }

		internal ECCurve Curve { get; }

		internal Oid CurveOid => Curve.Oid;

		internal string CurveOidValue => CurveOid.Value;

		internal int KeySizeInBytes => (KeySizeInBits + 7) / 8;

		private ECDsaAlgorithm(int keySizeInBits, ECCurve curve, HashAlgorithmName hashAlgorithmName)
		{
			KeySizeInBits = keySizeInBits;
			HashAlgorithmName = hashAlgorithmName;
			Curve = curve;
		}

		internal static ECDsaAlgorithm CreateP256(HashAlgorithmName hashAlgorithmName)
		{
			return new ECDsaAlgorithm(256, ECCurve.NamedCurves.nistP256, hashAlgorithmName);
		}

		internal static ECDsaAlgorithm CreateP384(HashAlgorithmName hashAlgorithmName)
		{
			return new ECDsaAlgorithm(384, ECCurve.NamedCurves.nistP384, hashAlgorithmName);
		}

		internal static ECDsaAlgorithm CreateP521(HashAlgorithmName hashAlgorithmName)
		{
			return new ECDsaAlgorithm(521, ECCurve.NamedCurves.nistP521, hashAlgorithmName);
		}

		internal static ECDsaAlgorithm CreateBrainpoolP256r1(HashAlgorithmName hashAlgorithmName)
		{
			return new ECDsaAlgorithm(256, ECCurve.NamedCurves.brainpoolP256r1, hashAlgorithmName);
		}

		internal static ECDsaAlgorithm CreateBrainpoolP384r1(HashAlgorithmName hashAlgorithmName)
		{
			return new ECDsaAlgorithm(384, ECCurve.NamedCurves.brainpoolP384r1, hashAlgorithmName);
		}
	}

	private sealed class EdDsaAlgorithm
	{
	}

	private sealed class ECDsaComponent : ComponentAlgorithm
	{
		private readonly ECDsaAlgorithm _algorithm;

		private ECDsa _ecdsa;

		private ECDsaComponent(ECDsa ecdsa, ECDsaAlgorithm algorithm)
		{
			_ecdsa = ecdsa;
			_algorithm = algorithm;
		}

		public static bool IsAlgorithmSupported(ECDsaAlgorithm algorithm)
		{
			switch (algorithm.CurveOidValue)
			{
			case "1.2.840.10045.3.1.7":
			case "1.3.132.0.34":
			case "1.3.132.0.35":
				return true;
			default:
				return false;
			}
		}

		public static ECDsaComponent GenerateKey(ECDsaAlgorithm algorithm)
		{
			return new ECDsaComponent(ECDsa.Create(algorithm.Curve), algorithm);
		}

		public unsafe static ECDsaComponent ImportPrivateKey(ECDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
		{
			Helpers.ThrowIfAsnInvalidLength(source);
			fixed (byte* reference = &MemoryMarshal.GetReference(source))
			{
				using MemoryManager<byte> memoryManager = new PointerMemoryManager<byte>(reference, source.Length);
				ECPrivateKey eCPrivateKey = ECPrivateKey.Decode(memoryManager.Memory, AsnEncodingRules.BER);
				if (eCPrivateKey.Version == 1)
				{
					ReadOnlyMemory<byte>? publicKey = eCPrivateKey.PublicKey;
					if (!publicKey.HasValue)
					{
						if (eCPrivateKey.Parameters?.Named != algorithm.CurveOidValue)
						{
							throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
						}
						byte[] array = new byte[eCPrivateKey.PrivateKey.Length];
						using (PinAndClear.Track(array))
						{
							eCPrivateKey.PrivateKey.CopyTo(array);
							ECParameters parameters = new ECParameters
							{
								Curve = algorithm.Curve,
								Q = new ECPoint
								{
									X = null,
									Y = null
								},
								D = array
							};
							parameters.Validate();
							return new ECDsaComponent(ECDsa.Create(parameters), algorithm);
						}
					}
				}
				throw new CryptographicException(System.SR.Cryptography_Der_Invalid_Encoding);
			}
		}

		public static ECDsaComponent ImportPublicKey(ECDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
		{
			int keySizeInBytes = algorithm.KeySizeInBytes;
			if (source.Length != 1 + keySizeInBytes * 2)
			{
				throw new CryptographicException();
			}
			if (source[0] != 4)
			{
				throw new CryptographicException(System.SR.Cryptography_NotValidPublicOrPrivateKey);
			}
			byte[] x = source.Slice(1, keySizeInBytes).ToArray();
			byte[] y = source.Slice(1 + keySizeInBytes).ToArray();
			return new ECDsaComponent(ECDsa.Create(new ECParameters
			{
				Curve = algorithm.Curve,
				Q = new ECPoint
				{
					X = x,
					Y = y
				}
			}), algorithm);
		}

		internal override bool TryExportPrivateKey(Span<byte> destination, out int bytesWritten)
		{
			ECParameters eCParameters = _ecdsa.ExportParameters(includePrivateParameters: true);
			using (PinAndClear.Track(eCParameters.D))
			{
				eCParameters.Validate();
				if (eCParameters.D.Length != _algorithm.KeySizeInBytes)
				{
					throw new CryptographicException();
				}
				if (!eCParameters.Curve.IsNamed || (eCParameters.Curve.Oid.Value != _algorithm.CurveOidValue && eCParameters.Curve.Oid.FriendlyName != _algorithm.CurveOid.FriendlyName))
				{
					throw new CryptographicException();
				}
				AsnWriter asnWriter = new AsnWriter(AsnEncodingRules.DER);
				try
				{
					WriteKey(eCParameters.D, _algorithm.CurveOidValue, asnWriter);
					return asnWriter.TryEncode(destination, out bytesWritten);
				}
				finally
				{
					asnWriter.Reset();
				}
			}
			static void WriteKey(byte[] d, string curveOid, AsnWriter writer)
			{
				using (writer.PushSequence())
				{
					writer.WriteInteger(1L);
					writer.WriteOctetString(d);
					using (writer.PushSequence(new Asn1Tag(TagClass.ContextSpecific, 0, isConstructed: true)))
					{
						writer.WriteObjectIdentifier(curveOid);
					}
				}
			}
		}

		internal override bool TryExportPublicKey(Span<byte> destination, out int bytesWritten)
		{
			int keySizeInBytes = _algorithm.KeySizeInBytes;
			if (destination.Length < 1 + 2 * keySizeInBytes)
			{
				bytesWritten = 0;
				return false;
			}
			byte[] array = null;
			ECParameters eCParameters = _ecdsa.ExportParameters(includePrivateParameters: false);
			eCParameters.Validate();
			byte[]? x = eCParameters.Q.X;
			array = eCParameters.Q.Y;
			if (x.Length != keySizeInBytes || array.Length != keySizeInBytes)
			{
				throw new CryptographicException();
			}
			destination[0] = 4;
			x.CopyTo(destination.Slice(1, keySizeInBytes));
			array.CopyTo(destination.Slice(1 + keySizeInBytes));
			bytesWritten = 1 + 2 * keySizeInBytes;
			return true;
		}

		internal override bool VerifyData(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature)
		{
			return _ecdsa.VerifyData(data, signature, _algorithm.HashAlgorithmName, DSASignatureFormat.Rfc3279DerSequence);
		}

		internal override int SignData(ReadOnlySpan<byte> data, Span<byte> destination)
		{
			if (!_ecdsa.TrySignData(data, destination, _algorithm.HashAlgorithmName, DSASignatureFormat.Rfc3279DerSequence, out var bytesWritten))
			{
				throw new CryptographicException();
			}
			return bytesWritten;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_ecdsa?.Dispose();
				_ecdsa = null;
			}
			base.Dispose(disposing);
		}
	}

	private sealed class RsaComponent : ComponentAlgorithm
	{
		private readonly HashAlgorithmName _hashAlgorithmName;

		private readonly RSASignaturePadding _padding;

		private RSA _rsa;

		private RsaComponent(RSA rsa, HashAlgorithmName hashAlgorithmName, RSASignaturePadding padding)
		{
			_rsa = rsa;
			_hashAlgorithmName = hashAlgorithmName;
			_padding = padding;
		}

		private static RSA CreateRSA()
		{
			return RSA.Create();
		}

		private static RSA CreateRSA(int keySizeInBits)
		{
			return RSA.Create(keySizeInBits);
		}

		internal override int SignData(ReadOnlySpan<byte> data, Span<byte> destination)
		{
			return _rsa.SignData(data, destination, _hashAlgorithmName, _padding);
		}

		internal override bool VerifyData(ReadOnlySpan<byte> data, ReadOnlySpan<byte> signature)
		{
			return _rsa.VerifyData(data, signature, _hashAlgorithmName, _padding);
		}

		public static RsaComponent GenerateKey(RsaAlgorithm algorithm)
		{
			RSA rSA = null;
			try
			{
				rSA = CreateRSA(algorithm.KeySizeInBits);
				rSA.ExportParameters(includePrivateParameters: false);
				return new RsaComponent(rSA, algorithm.HashAlgorithmName, algorithm.Padding);
			}
			catch (CryptographicException)
			{
				rSA?.Dispose();
				throw;
			}
		}

		public static RsaComponent ImportPrivateKey(RsaAlgorithm algorithm, ReadOnlySpan<byte> source)
		{
			RSA rSA = null;
			try
			{
				rSA = CreateRSA();
				rSA.ImportRSAPrivateKey(source, out var bytesRead);
				if (rSA.KeySize != algorithm.KeySizeInBits)
				{
					throw new CryptographicException(System.SR.Argument_PrivateKeyWrongSizeForAlgorithm);
				}
				if (bytesRead != source.Length)
				{
					throw new CryptographicException(System.SR.Argument_PrivateKeyWrongSizeForAlgorithm);
				}
			}
			catch (CryptographicException)
			{
				rSA?.Dispose();
				throw;
			}
			return new RsaComponent(rSA, algorithm.HashAlgorithmName, algorithm.Padding);
		}

		public static RsaComponent ImportPublicKey(RsaAlgorithm algorithm, ReadOnlySpan<byte> source)
		{
			RSA rSA = null;
			try
			{
				rSA = CreateRSA();
				rSA.ImportRSAPublicKey(source, out var bytesRead);
				if (rSA.KeySize != algorithm.KeySizeInBits)
				{
					throw new CryptographicException(System.SR.Argument_PublicKeyWrongSizeForAlgorithm);
				}
				if (bytesRead != source.Length)
				{
					throw new CryptographicException(System.SR.Argument_PublicKeyWrongSizeForAlgorithm);
				}
			}
			catch (CryptographicException)
			{
				rSA?.Dispose();
				throw;
			}
			return new RsaComponent(rSA, algorithm.HashAlgorithmName, algorithm.Padding);
		}

		internal override bool TryExportPublicKey(Span<byte> destination, out int bytesWritten)
		{
			return _rsa.TryExportRSAPublicKey(destination, out bytesWritten);
		}

		internal override bool TryExportPrivateKey(Span<byte> destination, out int bytesWritten)
		{
			return _rsa.TryExportRSAPrivateKey(destination, out bytesWritten);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_rsa?.Dispose();
				_rsa = null;
			}
			base.Dispose(disposing);
		}
	}

	private static readonly Dictionary<CompositeMLDsaAlgorithm, AlgorithmMetadata> s_algorithmMetadata = CreateAlgorithmMetadata();

	private static readonly ConcurrentDictionary<CompositeMLDsaAlgorithm, bool> s_algorithmSupport = new ConcurrentDictionary<CompositeMLDsaAlgorithm, bool>();

	private MLDsa _mldsa;

	private ComponentAlgorithm _componentAlgorithm;

	[CompilerGenerated]
	private AlgorithmMetadata _003CAlgorithmDetails_003Ek__BackingField;

	private static ReadOnlySpan<byte> MessageRepresentativePrefix => "CompositeAlgorithmSignatures2025"u8;

	private AlgorithmMetadata AlgorithmDetails => _003CAlgorithmDetails_003Ek__BackingField ?? (_003CAlgorithmDetails_003Ek__BackingField = s_algorithmMetadata[base.Algorithm]);

	private CompositeMLDsaManaged(CompositeMLDsaAlgorithm algorithm, MLDsa mldsa, ComponentAlgorithm componentAlgorithm)
		: base(algorithm)
	{
		_mldsa = mldsa;
		_componentAlgorithm = componentAlgorithm;
	}

	internal static bool SupportsAny()
	{
		return MLDsaImplementation.SupportsAny();
	}

	internal static bool IsAlgorithmSupportedImpl(CompositeMLDsaAlgorithm algorithm)
	{
		AlgorithmMetadata metadata = s_algorithmMetadata[algorithm];
		return s_algorithmSupport.GetOrAdd(algorithm, delegate
		{
			bool flag = MLDsaImplementation.IsAlgorithmSupported(metadata.MLDsaAlgorithm);
			if (flag)
			{
				object traditionalAlgorithm = metadata.TraditionalAlgorithm;
				bool flag2 = traditionalAlgorithm is RsaAlgorithm || (traditionalAlgorithm is ECDsaAlgorithm algorithm2 && ECDsaComponent.IsAlgorithmSupported(algorithm2));
				flag = flag2;
			}
			return flag;
		});
	}

	internal static CompositeMLDsa GenerateKeyImpl(CompositeMLDsaAlgorithm algorithm)
	{
		AlgorithmMetadata algorithmMetadata = s_algorithmMetadata[algorithm];
		MLDsa mLDsa = MLDsa.GenerateKey(algorithmMetadata.MLDsaAlgorithm);
		ComponentAlgorithm componentAlgorithm2;
		try
		{
			object traditionalAlgorithm = algorithmMetadata.TraditionalAlgorithm;
			ComponentAlgorithm componentAlgorithm;
			if (!(traditionalAlgorithm is RsaAlgorithm algorithm2))
			{
				if (!(traditionalAlgorithm is ECDsaAlgorithm algorithm3))
				{
					throw FailAndGetException();
				}
				componentAlgorithm = ECDsaComponent.GenerateKey(algorithm3);
			}
			else
			{
				componentAlgorithm = RsaComponent.GenerateKey(algorithm2);
			}
			componentAlgorithm2 = componentAlgorithm;
		}
		catch (CryptographicException)
		{
			mLDsa.Dispose();
			throw;
		}
		return new CompositeMLDsaManaged(algorithm, mLDsa, componentAlgorithm2);
		static CryptographicException FailAndGetException()
		{
			return new CryptographicException();
		}
	}

	internal static CompositeMLDsa ImportCompositeMLDsaPublicKeyImpl(CompositeMLDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		AlgorithmMetadata algorithmMetadata = s_algorithmMetadata[algorithm];
		ReadOnlySpan<byte> source2 = source.Slice(0, algorithmMetadata.MLDsaAlgorithm.PublicKeySizeInBytes);
		ReadOnlySpan<byte> source3 = source.Slice(algorithmMetadata.MLDsaAlgorithm.PublicKeySizeInBytes);
		MLDsaImplementation mldsa = MLDsaImplementation.ImportPublicKey(algorithmMetadata.MLDsaAlgorithm, source2);
		object traditionalAlgorithm = algorithmMetadata.TraditionalAlgorithm;
		ComponentAlgorithm componentAlgorithm;
		if (!(traditionalAlgorithm is RsaAlgorithm algorithm2))
		{
			if (!(traditionalAlgorithm is ECDsaAlgorithm algorithm3))
			{
				throw FailAndGetException();
			}
			componentAlgorithm = ECDsaComponent.ImportPublicKey(algorithm3, source3);
		}
		else
		{
			componentAlgorithm = RsaComponent.ImportPublicKey(algorithm2, source3);
		}
		ComponentAlgorithm componentAlgorithm2 = componentAlgorithm;
		return new CompositeMLDsaManaged(algorithm, mldsa, componentAlgorithm2);
		static CryptographicException FailAndGetException()
		{
			return new CryptographicException();
		}
	}

	internal static CompositeMLDsa ImportCompositeMLDsaPrivateKeyImpl(CompositeMLDsaAlgorithm algorithm, ReadOnlySpan<byte> source)
	{
		AlgorithmMetadata algorithmMetadata = s_algorithmMetadata[algorithm];
		ReadOnlySpan<byte> source2 = source.Slice(0, algorithmMetadata.MLDsaAlgorithm.PrivateSeedSizeInBytes);
		ReadOnlySpan<byte> source3 = source.Slice(algorithmMetadata.MLDsaAlgorithm.PrivateSeedSizeInBytes);
		MLDsaImplementation mldsa = MLDsaImplementation.ImportSeed(algorithmMetadata.MLDsaAlgorithm, source2);
		object traditionalAlgorithm = algorithmMetadata.TraditionalAlgorithm;
		ComponentAlgorithm componentAlgorithm;
		if (!(traditionalAlgorithm is RsaAlgorithm algorithm2))
		{
			if (!(traditionalAlgorithm is ECDsaAlgorithm algorithm3))
			{
				throw FailAndGetException();
			}
			componentAlgorithm = ECDsaComponent.ImportPrivateKey(algorithm3, source3);
		}
		else
		{
			componentAlgorithm = RsaComponent.ImportPrivateKey(algorithm2, source3);
		}
		ComponentAlgorithm componentAlgorithm2 = componentAlgorithm;
		return new CompositeMLDsaManaged(algorithm, mldsa, componentAlgorithm2);
		static CryptographicException FailAndGetException()
		{
			return new CryptographicException();
		}
	}

	protected override int SignDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, Span<byte> destination)
	{
		byte[] messageRepresentative = GetMessageRepresentative(AlgorithmDetails, context, data);
		Span<byte> destination2 = destination.Slice(0, AlgorithmDetails.MLDsaAlgorithm.SignatureSizeInBytes);
		Span<byte> destination3 = destination.Slice(AlgorithmDetails.MLDsaAlgorithm.SignatureSizeInBytes);
		bool flag = false;
		bool flag2 = false;
		try
		{
			_mldsa.SignData(messageRepresentative, destination2, AlgorithmDetails.Label);
			flag = true;
		}
		catch (CryptographicException)
		{
		}
		int num = 0;
		try
		{
			num = _componentAlgorithm.SignData(messageRepresentative, destination3);
			flag2 = true;
		}
		catch (CryptographicException)
		{
		}
		if (Or(!flag, !flag2))
		{
			CryptographicOperations.ZeroMemory(destination);
			throw new CryptographicException(System.SR.Cryptography_CompositeSignDataError);
		}
		return destination2.Length + num;
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		static bool Or(bool x, bool y)
		{
			return x | y;
		}
	}

	protected override bool VerifyDataCore(ReadOnlySpan<byte> data, ReadOnlySpan<byte> context, ReadOnlySpan<byte> signature)
	{
		ReadOnlySpan<byte> signature2 = signature.Slice(0, AlgorithmDetails.MLDsaAlgorithm.SignatureSizeInBytes);
		ReadOnlySpan<byte> signature3 = signature.Slice(AlgorithmDetails.MLDsaAlgorithm.SignatureSizeInBytes);
		byte[] messageRepresentative = GetMessageRepresentative(AlgorithmDetails, context, data);
		return And(_mldsa.VerifyData(messageRepresentative, signature2, AlgorithmDetails.Label), _componentAlgorithm.VerifyData(messageRepresentative, signature3));
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		static bool And(bool x, bool y)
		{
			return x & y;
		}
	}

	protected override bool TryExportPkcs8PrivateKeyCore(Span<byte> destination, out int bytesWritten)
	{
		AsnWriter asnWriter = null;
		try
		{
			using (CryptoPoolLease cryptoPoolLease = CryptoPoolLease.Rent(base.Algorithm.MaxPrivateKeySizeInBytes))
			{
				int num = ExportCompositeMLDsaPrivateKeyCore(cryptoPoolLease.Span);
				int initialCapacity = 32 + num;
				asnWriter = new AsnWriter(AsnEncodingRules.DER, initialCapacity);
				using (asnWriter.PushSequence())
				{
					asnWriter.WriteInteger(0L);
					using (asnWriter.PushSequence())
					{
						asnWriter.WriteObjectIdentifier(base.Algorithm.Oid);
					}
					asnWriter.WriteOctetString(cryptoPoolLease.Span.Slice(0, num));
				}
			}
			return asnWriter.TryEncode(destination, out bytesWritten);
		}
		finally
		{
			asnWriter?.Reset();
		}
	}

	protected override int ExportCompositeMLDsaPublicKeyCore(Span<byte> destination)
	{
		_mldsa.ExportMLDsaPublicKey(destination.Slice(0, AlgorithmDetails.MLDsaAlgorithm.PublicKeySizeInBytes));
		int num = 0 + AlgorithmDetails.MLDsaAlgorithm.PublicKeySizeInBytes;
		if (!_componentAlgorithm.TryExportPublicKey(destination.Slice(AlgorithmDetails.MLDsaAlgorithm.PublicKeySizeInBytes), out var bytesWritten))
		{
			throw new CryptographicException();
		}
		return num + bytesWritten;
	}

	protected override int ExportCompositeMLDsaPrivateKeyCore(Span<byte> destination)
	{
		try
		{
			_mldsa.ExportMLDsaPrivateSeed(destination.Slice(0, AlgorithmDetails.MLDsaAlgorithm.PrivateSeedSizeInBytes));
			int num = 0 + AlgorithmDetails.MLDsaAlgorithm.PrivateSeedSizeInBytes;
			if (!_componentAlgorithm.TryExportPrivateKey(destination.Slice(AlgorithmDetails.MLDsaAlgorithm.PrivateSeedSizeInBytes), out var bytesWritten))
			{
				throw new CryptographicException();
			}
			return num + bytesWritten;
		}
		catch (CryptographicException)
		{
			CryptographicOperations.ZeroMemory(destination);
			throw;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_mldsa?.Dispose();
			_mldsa = null;
			_componentAlgorithm?.Dispose();
			_componentAlgorithm = null;
		}
		base.Dispose(disposing);
	}

	private static byte[] GetMessageRepresentative(AlgorithmMetadata metadata, ReadOnlySpan<byte> context, ReadOnlySpan<byte> message)
	{
		checked
		{
			using IncrementalHash incrementalHash = IncrementalHash.CreateHash(metadata.HashAlgorithmName);
			int hashLengthInBytes = incrementalHash.HashLengthInBytes;
			byte[] array = new byte[MessageRepresentativePrefix.Length + metadata.Label.Length + 1 + context.Length + hashLengthInBytes];
			int num = 0;
			MessageRepresentativePrefix.CopyTo(array.AsSpan(num, MessageRepresentativePrefix.Length));
			num += MessageRepresentativePrefix.Length;
			metadata.Label.AsSpan().CopyTo(array.AsSpan(num, metadata.Label.Length));
			num += metadata.Label.Length;
			array[num] = (byte)context.Length;
			num++;
			context.CopyTo(array.AsSpan(num, context.Length));
			num += context.Length;
			incrementalHash.AppendData(message);
			incrementalHash.GetHashAndReset(array.AsSpan(num, hashLengthInBytes));
			num += hashLengthInBytes;
			return array;
		}
	}

	private static Dictionary<CompositeMLDsaAlgorithm, AlgorithmMetadata> CreateAlgorithmMetadata()
	{
		Dictionary<CompositeMLDsaAlgorithm, AlgorithmMetadata> dictionary = new Dictionary<CompositeMLDsaAlgorithm, AlgorithmMetadata>(18);
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa44WithRSA2048Pss, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa44, new RsaAlgorithm(2048, HashAlgorithmName.SHA256, RSASignaturePadding.Pss), "COMPSIG-MLDSA44-RSA2048-PSS-SHA256"u8.ToArray(), HashAlgorithmName.SHA256));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa44WithRSA2048Pkcs15, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa44, new RsaAlgorithm(2048, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1), "COMPSIG-MLDSA44-RSA2048-PKCS15-SHA256"u8.ToArray(), HashAlgorithmName.SHA256));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa44WithEd25519, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa44, new EdDsaAlgorithm(), "COMPSIG-MLDSA44-Ed25519-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa44WithECDsaP256, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa44, ECDsaAlgorithm.CreateP256(HashAlgorithmName.SHA256), "COMPSIG-MLDSA44-ECDSA-P256-SHA256"u8.ToArray(), HashAlgorithmName.SHA256));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa65WithRSA3072Pss, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa65, new RsaAlgorithm(3072, HashAlgorithmName.SHA256, RSASignaturePadding.Pss), "COMPSIG-MLDSA65-RSA3072-PSS-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa65WithRSA3072Pkcs15, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa65, new RsaAlgorithm(3072, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1), "COMPSIG-MLDSA65-RSA3072-PKCS15-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa65WithRSA4096Pss, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa65, new RsaAlgorithm(4096, HashAlgorithmName.SHA384, RSASignaturePadding.Pss), "COMPSIG-MLDSA65-RSA4096-PSS-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa65WithRSA4096Pkcs15, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa65, new RsaAlgorithm(4096, HashAlgorithmName.SHA384, RSASignaturePadding.Pkcs1), "COMPSIG-MLDSA65-RSA4096-PKCS15-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa65WithECDsaP256, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa65, ECDsaAlgorithm.CreateP256(HashAlgorithmName.SHA256), "COMPSIG-MLDSA65-ECDSA-P256-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa65WithECDsaP384, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa65, ECDsaAlgorithm.CreateP384(HashAlgorithmName.SHA384), "COMPSIG-MLDSA65-ECDSA-P384-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa65WithECDsaBrainpoolP256r1, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa65, ECDsaAlgorithm.CreateBrainpoolP256r1(HashAlgorithmName.SHA256), "COMPSIG-MLDSA65-ECDSA-BP256-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa65WithEd25519, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa65, new EdDsaAlgorithm(), "COMPSIG-MLDSA65-Ed25519-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa87WithECDsaP384, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa87, ECDsaAlgorithm.CreateP384(HashAlgorithmName.SHA384), "COMPSIG-MLDSA87-ECDSA-P384-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa87WithECDsaBrainpoolP384r1, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa87, ECDsaAlgorithm.CreateBrainpoolP384r1(HashAlgorithmName.SHA384), "COMPSIG-MLDSA87-ECDSA-BP384-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa87WithEd448, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa87, new EdDsaAlgorithm(), "COMPSIG-MLDSA87-Ed448-SHAKE256"u8.ToArray(), new HashAlgorithmName("SHAKE256")));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa87WithRSA3072Pss, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa87, new RsaAlgorithm(3072, HashAlgorithmName.SHA256, RSASignaturePadding.Pss), "COMPSIG-MLDSA87-RSA3072-PSS-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa87WithRSA4096Pss, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa87, new RsaAlgorithm(4096, HashAlgorithmName.SHA384, RSASignaturePadding.Pss), "COMPSIG-MLDSA87-RSA4096-PSS-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		dictionary.Add(CompositeMLDsaAlgorithm.MLDsa87WithECDsaP521, new AlgorithmMetadata(MLDsaAlgorithm.MLDsa87, ECDsaAlgorithm.CreateP521(HashAlgorithmName.SHA512), "COMPSIG-MLDSA87-ECDSA-P521-SHA512"u8.ToArray(), HashAlgorithmName.SHA512));
		return dictionary;
	}
}
