using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers.Text;

internal static class Base64Helper
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal readonly struct Base64DecoderByte : IBase64Decoder<byte>
	{
		public ReadOnlySpan<sbyte> DecodingMap => new sbyte[256]
		{
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, 62, -1, -1, -1, 63, 52, 53,
			54, 55, 56, 57, 58, 59, 60, 61, -1, -1,
			-1, -1, -1, -1, -1, 0, 1, 2, 3, 4,
			5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
			15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
			25, -1, -1, -1, -1, -1, -1, 26, 27, 28,
			29, 30, 31, 32, 33, 34, 35, 36, 37, 38,
			39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
			49, 50, 51, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1
		};

		public ReadOnlySpan<uint> VbmiLookup0 => new uint[16]
		{
			2155905152u, 2155905152u, 2155905152u, 2155905152u, 2155905152u, 2155905152u, 2155905152u, 2155905152u, 2155905152u, 2155905152u,
			1048608896u, 1065386112u, 926299444u, 993671480u, 2155887932u, 2155905152u
		};

		public ReadOnlySpan<uint> VbmiLookup1 => new uint[16]
		{
			33620096u, 100992003u, 168364039u, 235736075u, 303108111u, 370480147u, 2149128215u, 2155905152u, 471538304u, 538910237u,
			606282273u, 673654309u, 741026345u, 808398381u, 2150838833u, 2155905152u
		};

		public ReadOnlySpan<sbyte> Avx2LutHigh => new sbyte[32]
		{
			16, 16, 1, 2, 4, 8, 4, 8, 16, 16,
			16, 16, 16, 16, 16, 16, 16, 16, 1, 2,
			4, 8, 4, 8, 16, 16, 16, 16, 16, 16,
			16, 16
		};

		public ReadOnlySpan<sbyte> Avx2LutLow => new sbyte[32]
		{
			21, 17, 17, 17, 17, 17, 17, 17, 17, 17,
			19, 26, 27, 27, 27, 26, 21, 17, 17, 17,
			17, 17, 17, 17, 17, 17, 19, 26, 27, 27,
			27, 26
		};

		public ReadOnlySpan<sbyte> Avx2LutShift => new sbyte[32]
		{
			0, 16, 19, 4, -65, -65, -71, -71, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 16, 19, 4,
			-65, -65, -71, -71, 0, 0, 0, 0, 0, 0,
			0, 0
		};

		public byte MaskSlashOrUnderscore => 47;

		public ReadOnlySpan<int> Vector128LutHigh => new int[4] { 33624080, 134481924, 269488144, 269488144 };

		public ReadOnlySpan<int> Vector128LutLow => new int[4] { 286331157, 286331153, 437457169, 437984027 };

		public ReadOnlySpan<uint> Vector128LutShift => new uint[4] { 68358144u, 3115958207u, 0u, 0u };

		public ReadOnlySpan<uint> AdvSimdLutOne3 => new uint[4] { 4294967295u, 4294967295u, 1056964607u, 1073741823u };

		public uint AdvSimdLutTwo3Uint1 => 454754303u;

		public int GetMaxDecodedLength(int utf8Length)
		{
			return Base64.GetMaxDecodedFromUtf8Length(utf8Length);
		}

		public bool IsInvalidLength(int bufferLength)
		{
			return bufferLength % 4 != 0;
		}

		public bool IsValidPadding(uint padChar)
		{
			return padChar == 61;
		}

		public int SrcLength(bool _, int utf8Length)
		{
			return utf8Length & -4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
		[CompExactlyDependsOn(typeof(Ssse3))]
		public bool TryDecode128Core(Vector128<byte> str, Vector128<byte> hiNibbles, Vector128<byte> maskSlashOrUnderscore, Vector128<byte> mask8F, Vector128<byte> lutLow, Vector128<byte> lutHigh, Vector128<sbyte> lutShift, Vector128<byte> _, out Vector128<byte> result)
		{
			Vector128<byte> right = str & maskSlashOrUnderscore;
			Vector128<byte> vector = SimdShuffle(lutHigh, hiNibbles, mask8F);
			if ((SimdShuffle(lutLow, right, mask8F) & vector) != Vector128<byte>.Zero)
			{
				result = default(Vector128<byte>);
				return false;
			}
			Vector128<byte> vector2 = Vector128.Equals(str, maskSlashOrUnderscore);
			Vector128<byte> vector3 = SimdShuffle(lutShift.AsByte(), vector2 + hiNibbles, mask8F);
			result = str + vector3;
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(Avx2))]
		public bool TryDecode256Core(Vector256<sbyte> str, Vector256<sbyte> hiNibbles, Vector256<sbyte> maskSlashOrUnderscore, Vector256<sbyte> lutLow, Vector256<sbyte> lutHigh, Vector256<sbyte> lutShift, Vector256<sbyte> _, out Vector256<sbyte> result)
		{
			Vector256<sbyte> mask = Avx2.And(str, maskSlashOrUnderscore);
			Vector256<sbyte> vector = Avx2.Shuffle(lutHigh, hiNibbles);
			if ((Avx2.Shuffle(lutLow, mask) & vector) != Vector256<sbyte>.Zero)
			{
				result = default(Vector256<sbyte>);
				return false;
			}
			Vector256<sbyte> left = Avx2.CompareEqual(str, maskSlashOrUnderscore);
			Vector256<sbyte> right = Avx2.Shuffle(lutShift, Avx2.Add(left, hiNibbles));
			result = Avx2.Add(str, right);
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool TryLoadVector512(byte* src, byte* srcStart, int sourceLength, out Vector512<sbyte> str)
		{
			str = Vector512.Load(src).AsSByte();
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(Avx2))]
		public unsafe bool TryLoadAvxVector256(byte* src, byte* srcStart, int sourceLength, out Vector256<sbyte> str)
		{
			str = Avx.LoadVector256(src).AsSByte();
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool TryLoadVector128(byte* src, byte* srcStart, int sourceLength, out Vector128<byte> str)
		{
			str = Vector128.LoadUnsafe(in *src);
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
		public unsafe bool TryLoadArmVector128x4(byte* src, byte* srcStart, int sourceLength, out Vector128<byte> str1, out Vector128<byte> str2, out Vector128<byte> str3, out Vector128<byte> str4)
		{
			(str1, str2, str3, str4) = AdvSimd.Arm64.Load4xVector128AndUnzip(src);
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe int DecodeFourElements(byte* source, ref sbyte decodingMap)
		{
			uint elementOffset = *source;
			uint elementOffset2 = source[1];
			uint elementOffset3 = source[2];
			uint elementOffset4 = source[3];
			sbyte num = Unsafe.Add(ref decodingMap, (int)elementOffset);
			int num2 = Unsafe.Add(ref decodingMap, (int)elementOffset2);
			int num3 = Unsafe.Add(ref decodingMap, (int)elementOffset3);
			int num4 = Unsafe.Add(ref decodingMap, (int)elementOffset4);
			int num5 = num << 18;
			num2 <<= 12;
			num3 <<= 6;
			int num6 = num5 | num4;
			num2 |= num3;
			return num6 | num2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe int DecodeRemaining(byte* srcEnd, ref sbyte decodingMap, long remaining, out uint t2, out uint t3)
		{
			t2 = 61u;
			t3 = 61u;
			long num = remaining - 2;
			if ((ulong)num <= 2uL)
			{
				uint num2;
				uint num3;
				sbyte num4;
				int num6;
				int num5;
				switch ((int)num)
				{
				case 0:
					num2 = srcEnd[-2];
					num3 = srcEnd[-1];
					goto IL_006b;
				case 1:
					num2 = srcEnd[-3];
					num3 = srcEnd[-2];
					t2 = srcEnd[-1];
					goto IL_006b;
				case 2:
					{
						num2 = srcEnd[-4];
						num3 = srcEnd[-3];
						t2 = srcEnd[-2];
						t3 = srcEnd[-1];
						goto IL_006b;
					}
					IL_006b:
					num4 = Unsafe.Add(ref decodingMap, (nint)num2);
					num5 = Unsafe.Add(ref decodingMap, (nint)num3);
					num6 = num4 << 18;
					num5 <<= 12;
					return num6 | num5;
				}
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOfAnyExceptWhiteSpace(ReadOnlySpan<byte> span)
		{
			for (int i = 0; i < span.Length; i++)
			{
				if (!IsWhiteSpace(span[i]))
				{
					return i;
				}
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public OperationStatus DecodeWithWhiteSpaceBlockwiseWrapper<TBase64Decoder>(TBase64Decoder decoder, ReadOnlySpan<byte> utf8, Span<byte> bytes, ref int bytesConsumed, ref int bytesWritten, bool isFinalBlock = true) where TBase64Decoder : IBase64Decoder<byte>
		{
			return DecodeWithWhiteSpaceBlockwise(decoder, utf8, bytes, ref bytesConsumed, ref bytesWritten, isFinalBlock);
		}
	}

	internal interface IBase64Encoder<T> where T : unmanaged
	{
		ReadOnlySpan<byte> EncodingMap { get; }

		sbyte Avx2LutChar62 { get; }

		sbyte Avx2LutChar63 { get; }

		ReadOnlySpan<byte> AdvSimdLut4 { get; }

		uint Ssse3AdvSimdLutE3 { get; }

		int IncrementPadTwo { get; }

		int IncrementPadOne { get; }

		int GetMaxSrcLength(int srcLength, int destLength);

		int GetMaxEncodedLength(int srcLength);

		uint GetInPlaceDestinationLength(int encodedLength, int leftOver);

		unsafe void EncodeOneOptionallyPadTwo(byte* oneByte, T* dest, ref byte encodingMap);

		unsafe void EncodeTwoOptionallyPadOne(byte* oneByte, T* dest, ref byte encodingMap);

		unsafe void EncodeThreeAndWrite(byte* threeBytes, T* destination, ref byte encodingMap);

		unsafe void StoreVector512ToDestination(T* dest, T* destStart, int destLength, Vector512<byte> str);

		unsafe void StoreVector256ToDestination(T* dest, T* destStart, int destLength, Vector256<byte> str);

		unsafe void StoreVector128ToDestination(T* dest, T* destStart, int destLength, Vector128<byte> str);

		unsafe void StoreArmVector128x4ToDestination(T* dest, T* destStart, int destLength, Vector128<byte> res1, Vector128<byte> res2, Vector128<byte> res3, Vector128<byte> res4);
	}

	internal interface IBase64Decoder<T> where T : unmanaged
	{
		ReadOnlySpan<sbyte> DecodingMap { get; }

		ReadOnlySpan<uint> VbmiLookup0 { get; }

		ReadOnlySpan<uint> VbmiLookup1 { get; }

		ReadOnlySpan<sbyte> Avx2LutHigh { get; }

		ReadOnlySpan<sbyte> Avx2LutLow { get; }

		ReadOnlySpan<sbyte> Avx2LutShift { get; }

		byte MaskSlashOrUnderscore { get; }

		ReadOnlySpan<int> Vector128LutHigh { get; }

		ReadOnlySpan<int> Vector128LutLow { get; }

		ReadOnlySpan<uint> Vector128LutShift { get; }

		ReadOnlySpan<uint> AdvSimdLutOne3 { get; }

		uint AdvSimdLutTwo3Uint1 { get; }

		int SrcLength(bool isFinalBlock, int sourceLength);

		int GetMaxDecodedLength(int sourceLength);

		bool IsInvalidLength(int bufferLength);

		bool IsValidPadding(uint padChar);

		bool TryDecode128Core(Vector128<byte> str, Vector128<byte> hiNibbles, Vector128<byte> maskSlashOrUnderscore, Vector128<byte> mask8F, Vector128<byte> lutLow, Vector128<byte> lutHigh, Vector128<sbyte> lutShift, Vector128<byte> shiftForUnderscore, out Vector128<byte> result);

		bool TryDecode256Core(Vector256<sbyte> str, Vector256<sbyte> hiNibbles, Vector256<sbyte> maskSlashOrUnderscore, Vector256<sbyte> lutLow, Vector256<sbyte> lutHigh, Vector256<sbyte> lutShift, Vector256<sbyte> shiftForUnderscore, out Vector256<sbyte> result);

		unsafe bool TryLoadVector512(T* src, T* srcStart, int sourceLength, out Vector512<sbyte> str);

		unsafe bool TryLoadAvxVector256(T* src, T* srcStart, int sourceLength, out Vector256<sbyte> str);

		unsafe bool TryLoadVector128(T* src, T* srcStart, int sourceLength, out Vector128<byte> str);

		unsafe bool TryLoadArmVector128x4(T* src, T* srcStart, int sourceLength, out Vector128<byte> str1, out Vector128<byte> str2, out Vector128<byte> str3, out Vector128<byte> str4);

		unsafe int DecodeFourElements(T* source, ref sbyte decodingMap);

		unsafe int DecodeRemaining(T* srcEnd, ref sbyte decodingMap, long remaining, out uint t2, out uint t3);

		int IndexOfAnyExceptWhiteSpace(ReadOnlySpan<T> span);

		OperationStatus DecodeWithWhiteSpaceBlockwiseWrapper<TTBase64Decoder>(TTBase64Decoder decoder, ReadOnlySpan<T> source, Span<byte> bytes, ref int bytesConsumed, ref int bytesWritten, bool isFinalBlock = true) where TTBase64Decoder : IBase64Decoder<T>;
	}

	internal interface IBase64Validatable<T>
	{
		int IndexOfAnyExcept(ReadOnlySpan<T> span);

		bool IsWhiteSpace(T value);

		bool IsEncodingPad(T value);

		bool ValidateAndDecodeLength(T lastChar, int length, int paddingCount, out int decodedLength);
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal readonly struct Base64CharValidatable : IBase64Validatable<char>
	{
		private static readonly SearchValues<char> s_validBase64Chars = SearchValues.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".AsSpan());

		public int IndexOfAnyExcept(ReadOnlySpan<char> span)
		{
			return span.IndexOfAnyExcept(s_validBase64Chars);
		}

		public bool IsWhiteSpace(char value)
		{
			return Base64Helper.IsWhiteSpace((int)value);
		}

		public bool IsEncodingPad(char value)
		{
			return value == '=';
		}

		public bool ValidateAndDecodeLength(char lastChar, int length, int paddingCount, out int decodedLength)
		{
			return default(Base64ByteValidatable).ValidateAndDecodeLength((byte)lastChar, length, paddingCount, out decodedLength);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal readonly struct Base64ByteValidatable : IBase64Validatable<byte>
	{
		private static readonly SearchValues<byte> s_validBase64Chars = SearchValues.Create(default(Base64EncoderByte).EncodingMap);

		public int IndexOfAnyExcept(ReadOnlySpan<byte> span)
		{
			return span.IndexOfAnyExcept(s_validBase64Chars);
		}

		public bool IsWhiteSpace(byte value)
		{
			return Base64Helper.IsWhiteSpace((int)value);
		}

		public bool IsEncodingPad(byte value)
		{
			return value == 61;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ValidateAndDecodeLength(byte lastChar, int length, int paddingCount, out int decodedLength)
		{
			if (length % 4 == 0)
			{
				int num = default(Base64DecoderByte).DecodingMap[lastChar];
				if ((paddingCount == 1 && (num & 3) != 0) || (paddingCount == 2 && (num & 0xF) != 0))
				{
					decodedLength = 0;
					return false;
				}
				decodedLength = (int)((uint)length / 4u * 3) - paddingCount;
				return true;
			}
			decodedLength = 0;
			return false;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal readonly struct Base64EncoderByte : IBase64Encoder<byte>
	{
		public ReadOnlySpan<byte> EncodingMap => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"u8;

		public sbyte Avx2LutChar62 => -19;

		public sbyte Avx2LutChar63 => -16;

		public ReadOnlySpan<byte> AdvSimdLut4 => "wxyz0123456789+/"u8;

		public uint Ssse3AdvSimdLutE3 => 61677u;

		public int IncrementPadTwo => 4;

		public int IncrementPadOne => 4;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetMaxSrcLength(int srcLength, int destLength)
		{
			if (srcLength > 1610612733 || destLength < Base64.GetMaxEncodedToUtf8Length(srcLength))
			{
				return (destLength >> 2) * 3;
			}
			return srcLength;
		}

		public uint GetInPlaceDestinationLength(int encodedLength, int _)
		{
			return (uint)(encodedLength - 4);
		}

		public int GetMaxEncodedLength(int srcLength)
		{
			return Base64.GetMaxEncodedToUtf8Length(srcLength);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void EncodeOneOptionallyPadTwo(byte* oneByte, byte* dest, ref byte encodingMap)
		{
			uint num = (uint)(*oneByte << 8);
			byte i = Unsafe.Add(ref encodingMap, (nint)(num >> 10));
			uint i2 = Unsafe.Add(ref encodingMap, (nint)((num >> 4) & 0x3F));
			uint value = ConstructResult(i, i2, 61u, 61u);
			Unsafe.WriteUnaligned(dest, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void EncodeTwoOptionallyPadOne(byte* twoBytes, byte* dest, ref byte encodingMap)
		{
			byte num = *twoBytes;
			uint num2 = twoBytes[1];
			uint num3 = (uint)(num << 16) | (num2 << 8);
			byte i = Unsafe.Add(ref encodingMap, (nint)(num3 >> 18));
			uint i2 = Unsafe.Add(ref encodingMap, (nint)((num3 >> 12) & 0x3F));
			uint i3 = Unsafe.Add(ref encodingMap, (nint)((num3 >> 6) & 0x3F));
			uint value = ConstructResult(i, i2, i3, 61u);
			Unsafe.WriteUnaligned(dest, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void StoreVector512ToDestination(byte* dest, byte* destStart, int destLength, Vector512<byte> str)
		{
			str.Store(dest);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(Avx2))]
		public unsafe void StoreVector256ToDestination(byte* dest, byte* destStart, int destLength, Vector256<byte> str)
		{
			Avx.Store(dest, str.AsByte());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void StoreVector128ToDestination(byte* dest, byte* destStart, int destLength, Vector128<byte> str)
		{
			str.Store(dest);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
		public unsafe void StoreArmVector128x4ToDestination(byte* dest, byte* destStart, int destLength, Vector128<byte> res1, Vector128<byte> res2, Vector128<byte> res3, Vector128<byte> res4)
		{
			AdvSimd.Arm64.StoreVectorAndZip(dest, (Value1: res1, Value2: res2, Value3: res3, Value4: res4));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void EncodeThreeAndWrite(byte* threeBytes, byte* destination, ref byte encodingMap)
		{
			uint value = Encode(threeBytes, ref encodingMap);
			Unsafe.WriteUnaligned(destination, value);
		}
	}

	internal unsafe static OperationStatus DecodeFrom<TBase64Decoder, T>(TBase64Decoder decoder, ReadOnlySpan<T> source, Span<byte> bytes, out int bytesConsumed, out int bytesWritten, bool isFinalBlock, bool ignoreWhiteSpace) where TBase64Decoder : IBase64Decoder<T> where T : unmanaged
	{
		if (source.IsEmpty)
		{
			bytesConsumed = 0;
			bytesWritten = 0;
			return OperationStatus.Done;
		}
		fixed (T* reference = &MemoryMarshal.GetReference(source))
		{
			fixed (byte* reference2 = &MemoryMarshal.GetReference(bytes))
			{
				int num = decoder.SrcLength(isFinalBlock, source.Length);
				int length = bytes.Length;
				int num2 = num;
				int maxDecodedLength = decoder.GetMaxDecodedLength(num);
				if (length < maxDecodedLength - 2)
				{
					num2 = length / 3 * 4;
				}
				T* srcBytes = reference;
				byte* destBytes = reference2;
				T* ptr = reference + (uint)num;
				T* ptr2 = reference + (uint)num2;
				if (num2 >= 24)
				{
					T* ptr3 = ptr2 - 88;
					if (Vector512.IsHardwareAccelerated && Avx512Vbmi.IsSupported && ptr3 >= srcBytes)
					{
						Avx512Decode(decoder, ref srcBytes, ref destBytes, ptr3, num2, length, reference, reference2);
						if (srcBytes == ptr)
						{
							goto IL_03b6;
						}
					}
					ptr3 = ptr2 - 45;
					if (Avx2.IsSupported && ptr3 >= srcBytes)
					{
						Avx2Decode(decoder, ref srcBytes, ref destBytes, ptr3, num2, length, reference, reference2);
						if (srcBytes == ptr)
						{
							goto IL_03b6;
						}
					}
					ptr3 = ptr2 - 66;
					if (false)
					{
					}
					ptr3 = ptr2 - 24;
					if (Ssse3.IsSupported ? true : false)
					{
						_ = BitConverter.IsLittleEndian;
						if (ptr3 >= srcBytes)
						{
							Vector128Decode(decoder, ref srcBytes, ref destBytes, ptr3, num2, length, reference, reference2);
							if (srcBytes == ptr)
							{
								goto IL_03b6;
							}
						}
					}
				}
				int num3 = (isFinalBlock ? 4 : 0);
				if (length >= maxDecodedLength)
				{
					num2 = num - num3;
				}
				else
				{
					(int Quotient, int Remainder) tuple = int.DivRem(length, 3);
					num2 = tuple.Quotient;
					int item = tuple.Remainder;
					num2 *= 4;
					if (isFinalBlock && item > 0)
					{
						num &= -4;
					}
				}
				ref sbyte reference3 = ref MemoryMarshal.GetReference(decoder.DecodingMap);
				ptr2 = reference + num2;
				while (true)
				{
					if (srcBytes < ptr2)
					{
						T* source2 = srcBytes;
						int num4 = decoder.DecodeFourElements(source2, ref reference3);
						if (num4 >= 0)
						{
							WriteThreeLowOrderBytes(destBytes, num4);
							srcBytes += 4;
							destBytes += 3;
							continue;
						}
					}
					else
					{
						if (num2 != num - num3)
						{
							goto IL_03d2;
						}
						if (srcBytes == ptr)
						{
							if (!isFinalBlock)
							{
								if (srcBytes == reference + source.Length)
								{
									break;
								}
								bytesConsumed = (int)(srcBytes - reference);
								bytesWritten = (int)(destBytes - reference2);
								return OperationStatus.NeedMoreData;
							}
						}
						else
						{
							long num5 = ptr - srcBytes;
							int num6 = decoder.DecodeRemaining(ptr, ref reference3, num5, out var t, out var t2);
							if (num6 >= 0)
							{
								byte* ptr4 = reference2 + (uint)length;
								uint padChar = t2;
								if (!decoder.IsValidPadding(padChar))
								{
									int num7 = Unsafe.Add(ref reference3, (nint)t);
									int num8 = Unsafe.Add(ref reference3, (nint)t2);
									num7 <<= 6;
									num6 |= num8;
									num6 |= num7;
									if (num6 >= 0)
									{
										if (destBytes + 3 <= ptr4)
										{
											WriteThreeLowOrderBytes(destBytes, num6);
											destBytes += 3;
											srcBytes += 4;
											goto IL_03ab;
										}
										goto IL_03d2;
									}
								}
								else
								{
									uint padChar2 = t;
									if (!decoder.IsValidPadding(padChar2))
									{
										int num9 = Unsafe.Add(ref reference3, (nint)t);
										num9 <<= 6;
										num6 |= num9;
										if ((num6 & 0x800000C0u) == 0L)
										{
											if (destBytes + 2 <= ptr4)
											{
												*destBytes = (byte)(num6 >> 16);
												destBytes[1] = (byte)(num6 >> 8);
												destBytes += 2;
												srcBytes += num5;
												goto IL_03ab;
											}
											goto IL_03d2;
										}
									}
									else if ((num6 & 0x8000F000u) == 0L)
									{
										if (destBytes + 1 <= ptr4)
										{
											*destBytes = (byte)(num6 >> 16);
											destBytes++;
											srcBytes += num5;
											goto IL_03ab;
										}
										goto IL_03d2;
									}
								}
							}
						}
					}
					goto IL_041d;
					IL_03d2:
					if (!((num != source.Length) & isFinalBlock))
					{
						bytesConsumed = (int)(srcBytes - reference);
						bytesWritten = (int)(destBytes - reference2);
						return OperationStatus.DestinationTooSmall;
					}
					goto IL_041d;
					IL_041d:
					bytesConsumed = (int)(srcBytes - reference);
					bytesWritten = (int)(destBytes - reference2);
					if (!ignoreWhiteSpace)
					{
						return OperationStatus.InvalidData;
					}
					return InvalidDataFallback(decoder, source, bytes, ref bytesConsumed, ref bytesWritten, isFinalBlock);
					IL_03ab:
					if (num == source.Length)
					{
						break;
					}
					goto IL_041d;
				}
				goto IL_03b6;
				IL_03b6:
				bytesConsumed = (int)(srcBytes - reference);
				bytesWritten = (int)(destBytes - reference2);
				return OperationStatus.Done;
			}
		}
		static OperationStatus InvalidDataFallback(TBase64Decoder val, ReadOnlySpan<T> readOnlySpan, Span<byte> span, ref int reference4, ref int reference5, bool isFinalBlock2)
		{
			readOnlySpan = readOnlySpan.Slice(reference4);
			span = span.Slice(reference5);
			OperationStatus operationStatus;
			do
			{
				ReadOnlySpan<T> span2 = readOnlySpan;
				int bytesConsumed2 = val.IndexOfAnyExceptWhiteSpace(span2);
				if (bytesConsumed2 < 0)
				{
					reference4 += readOnlySpan.Length;
					operationStatus = OperationStatus.Done;
					break;
				}
				if (bytesConsumed2 == 0)
				{
					TBase64Decoder decoder2 = val;
					ReadOnlySpan<T> source3 = readOnlySpan;
					Span<byte> bytes2 = span;
					return val.DecodeWithWhiteSpaceBlockwiseWrapper(decoder2, source3, bytes2, ref reference4, ref reference5, isFinalBlock2);
				}
				reference4 += bytesConsumed2;
				readOnlySpan = readOnlySpan.Slice(bytesConsumed2);
				operationStatus = DecodeFrom(val, readOnlySpan, span, out bytesConsumed2, out var bytesWritten2, isFinalBlock2, ignoreWhiteSpace: false);
				reference4 += bytesConsumed2;
				reference5 += bytesWritten2;
				if (operationStatus != OperationStatus.InvalidData)
				{
					break;
				}
				readOnlySpan = readOnlySpan.Slice(bytesConsumed2);
				span = span.Slice(bytesWritten2);
			}
			while (!readOnlySpan.IsEmpty);
			return operationStatus;
		}
	}

	internal unsafe static OperationStatus DecodeFromUtf8InPlace<TBase64Decoder>(TBase64Decoder decoder, Span<byte> buffer, out int bytesWritten, bool ignoreWhiteSpace) where TBase64Decoder : IBase64Decoder<byte>
	{
		if (buffer.IsEmpty)
		{
			bytesWritten = 0;
			return OperationStatus.Done;
		}
		fixed (byte* reference = &MemoryMarshal.GetReference(buffer))
		{
			uint length = (uint)buffer.Length;
			uint num = 0u;
			uint num2 = 0u;
			if (!decoder.IsInvalidLength(buffer.Length))
			{
				ref sbyte reference2 = ref MemoryMarshal.GetReference(decoder.DecodingMap);
				if (length > 4)
				{
					while (num < length - 4)
					{
						int num3 = decoder.DecodeFourElements(reference + num, ref reference2);
						if (num3 >= 0)
						{
							WriteThreeLowOrderBytes(reference + num2, num3);
							num2 += 3;
							num += 4;
							continue;
						}
						goto IL_022c;
					}
				}
				uint elementOffset;
				uint elementOffset2;
				uint num4;
				uint num5;
				int num7;
				int num6;
				switch (length - num)
				{
				case 2u:
					elementOffset = reference[length - 2];
					elementOffset2 = reference[length - 1];
					num4 = 61u;
					num5 = 61u;
					goto IL_012b;
				case 3u:
					elementOffset = reference[length - 3];
					elementOffset2 = reference[length - 2];
					num4 = reference[length - 1];
					num5 = 61u;
					goto IL_012b;
				case 4u:
					{
						elementOffset = reference[length - 4];
						elementOffset2 = reference[length - 3];
						num4 = reference[length - 2];
						num5 = reference[length - 1];
						goto IL_012b;
					}
					IL_012b:
					num6 = Unsafe.Add(ref reference2, (int)elementOffset);
					num7 = Unsafe.Add(ref reference2, (int)elementOffset2);
					num6 <<= 18;
					num7 <<= 12;
					num6 |= num7;
					if (!decoder.IsValidPadding(num5))
					{
						int num8 = Unsafe.Add(ref reference2, (int)num4);
						int num9 = Unsafe.Add(ref reference2, (int)num5);
						num8 <<= 6;
						num6 |= num9;
						num6 |= num8;
						if (num6 < 0)
						{
							break;
						}
						WriteThreeLowOrderBytes(reference + num2, num6);
						num2 += 3;
					}
					else if (!decoder.IsValidPadding(num4))
					{
						int num10 = Unsafe.Add(ref reference2, (int)num4);
						num10 <<= 6;
						num6 |= num10;
						if ((num6 & 0x800000C0u) != 0L)
						{
							break;
						}
						reference[num2] = (byte)(num6 >> 16);
						reference[num2 + 1] = (byte)(num6 >> 8);
						num2 += 2;
					}
					else
					{
						if ((num6 & 0x8000F000u) != 0L)
						{
							break;
						}
						reference[num2] = (byte)(num6 >> 16);
						num2++;
					}
					bytesWritten = (int)num2;
					return OperationStatus.Done;
				}
			}
			goto IL_022c;
			IL_022c:
			bytesWritten = (int)num2;
			if (!ignoreWhiteSpace)
			{
				return OperationStatus.InvalidData;
			}
			return DecodeWithWhiteSpaceFromUtf8InPlace(decoder, buffer, ref bytesWritten, num);
		}
	}

	internal static OperationStatus DecodeWithWhiteSpaceBlockwise<TBase64Decoder>(TBase64Decoder decoder, ReadOnlySpan<byte> source, Span<byte> bytes, ref int bytesConsumed, ref int bytesWritten, bool isFinalBlock = true) where TBase64Decoder : IBase64Decoder<byte>
	{
		Span<byte> span = stackalloc byte[4];
		OperationStatus operationStatus = OperationStatus.Done;
		while (!source.IsEmpty)
		{
			int i = 0;
			int num = 0;
			int num2 = 0;
			for (; i < source.Length; i++)
			{
				if ((uint)num >= (uint)span.Length)
				{
					break;
				}
				if (IsWhiteSpace(source[i]))
				{
					num2++;
					continue;
				}
				span[num] = source[i];
				num++;
			}
			source = source.Slice(i);
			bytesConsumed += num2;
			if (num == 0)
			{
				continue;
			}
			bool flag = ((!(typeof(TBase64Decoder) == typeof(Base64DecoderByte))) ? (source.Length > 1) : (source.Length >= 4));
			bool flag2 = !flag;
			if (flag && GetPaddingCount(decoder, ref span[3]) > 0)
			{
				flag = false;
				flag2 = true;
			}
			if (flag2 && !isFinalBlock)
			{
				flag2 = false;
			}
			operationStatus = DecodeFrom(decoder, span.Slice(0, num), bytes, out var bytesConsumed2, out var bytesWritten2, flag2, ignoreWhiteSpace: false);
			bytesConsumed += bytesConsumed2;
			bytesWritten += bytesWritten2;
			if (operationStatus != OperationStatus.Done)
			{
				return operationStatus;
			}
			if (!flag)
			{
				for (int j = 0; j < source.Length; j++)
				{
					if (!IsWhiteSpace(source[j]))
					{
						bytesConsumed -= bytesConsumed2;
						bytesWritten -= bytesWritten2;
						return OperationStatus.InvalidData;
					}
					bytesConsumed++;
				}
				break;
			}
			bytes = bytes.Slice(bytesWritten2);
		}
		return operationStatus;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetPaddingCount<TBase64Decoder>(TBase64Decoder decoder, ref byte ptrToLastElement) where TBase64Decoder : IBase64Decoder<byte>
	{
		int num = 0;
		byte padChar = ptrToLastElement;
		if (decoder.IsValidPadding(padChar))
		{
			num++;
		}
		if (decoder.IsValidPadding(Unsafe.Subtract(ref ptrToLastElement, 1)))
		{
			num++;
		}
		return num;
	}

	private static OperationStatus DecodeWithWhiteSpaceFromUtf8InPlace<TBase64Decoder>(TBase64Decoder decoder, Span<byte> source, ref int destIndex, uint sourceIndex) where TBase64Decoder : IBase64Decoder<byte>
	{
		int num = Math.Min(source.Length - (int)sourceIndex, 4);
		Span<byte> buffer = stackalloc byte[num];
		OperationStatus operationStatus = OperationStatus.Done;
		int num2 = destIndex;
		bool flag = false;
		int bytesWritten = 0;
		while (sourceIndex < (uint)source.Length)
		{
			int num3 = 0;
			while (num3 < num && sourceIndex < (uint)source.Length)
			{
				if (!IsWhiteSpace(source[(int)sourceIndex]))
				{
					buffer[num3] = source[(int)sourceIndex];
					num3++;
				}
				sourceIndex++;
			}
			if (num3 == 0)
			{
				continue;
			}
			if (num3 != 4)
			{
				if (decoder is Base64DecoderByte || num3 == 1)
				{
					operationStatus = OperationStatus.InvalidData;
					break;
				}
				while (num3 < num)
				{
					buffer[num3++] = 61;
				}
			}
			if (flag)
			{
				num2 -= bytesWritten;
				operationStatus = OperationStatus.InvalidData;
				break;
			}
			operationStatus = DecodeFromUtf8InPlace(decoder, buffer, out bytesWritten, ignoreWhiteSpace: false);
			num2 += bytesWritten;
			flag = bytesWritten < 3;
			if (operationStatus != OperationStatus.Done)
			{
				break;
			}
			for (int i = 0; i < bytesWritten; i++)
			{
				source[num2 - bytesWritten + i] = buffer[i];
			}
		}
		destIndex = num2;
		return operationStatus;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512BW))]
	[CompExactlyDependsOn(typeof(Avx512Vbmi))]
	private unsafe static void Avx512Decode<TBase64Decoder, T>(TBase64Decoder decoder, ref T* srcBytes, ref byte* destBytes, T* srcEnd, int sourceLength, int destLength, T* srcStart, byte* destStart) where TBase64Decoder : IBase64Decoder<T> where T : unmanaged
	{
		T* ptr = srcBytes;
		byte* ptr2 = destBytes;
		Vector512<sbyte> lower = Vector512.Create(decoder.VbmiLookup0).AsSByte();
		Vector512<sbyte> upper = Vector512.Create(decoder.VbmiLookup1).AsSByte();
		Vector512<byte> control = Vector512.Create(100663554, 151651333, 202182152, 370151698, 421139477, 471670296, 639639842, 690627621, 741158440, 909127986, 960115765, 1010646584, 0, 0, 0, 0).AsByte();
		Vector512<sbyte> right = Vector512.Create(20971840).AsSByte();
		Vector512<short> right2 = Vector512.Create(69632).AsInt16();
		Vector512<sbyte> str;
		while (decoder.TryLoadVector512(ptr, srcStart, sourceLength, out str))
		{
			Vector512<sbyte> vector = Avx512Vbmi.PermuteVar64x8x2(lower, str, upper);
			if ((vector.AsInt32() | str.AsInt32()).AsSByte().ExtractMostSignificantBits() != 0L)
			{
				break;
			}
			str = Avx512Vbmi.PermuteVar64x8(Avx512BW.MultiplyAddAdjacent(Avx512BW.MultiplyAddAdjacent(vector.AsByte(), right), right2).AsByte(), control).AsSByte();
			str.Store((sbyte*)ptr2);
			ptr += 64;
			ptr2 += 48;
			if (ptr > srcEnd)
			{
				break;
			}
		}
		srcBytes = ptr;
		destBytes = ptr2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private unsafe static void Avx2Decode<TBase64Decoder, T>(TBase64Decoder decoder, ref T* srcBytes, ref byte* destBytes, T* srcEnd, int sourceLength, int destLength, T* srcStart, byte* destStart) where TBase64Decoder : IBase64Decoder<T> where T : unmanaged
	{
		Vector256<sbyte> lutHigh = Vector256.Create(decoder.Avx2LutHigh);
		Vector256<sbyte> lutLow = Vector256.Create(decoder.Avx2LutLow);
		Vector256<sbyte> lutShift = Vector256.Create(decoder.Avx2LutShift);
		Vector256<sbyte> mask = Vector256.Create(2, 1, 0, 6, 5, 4, 10, 9, 8, 14, 13, 12, -1, -1, -1, -1, 2, 1, 0, 6, 5, 4, 10, 9, 8, 14, 13, 12, -1, -1, -1, -1);
		Vector256<int> control = Vector256.Create(0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 4, 0, 0, 0, 5, 0, 0, 0, 6, 0, 0, 0, -1, -1, -1, -1, -1, -1, -1, -1).AsInt32();
		Vector256<sbyte> vector = Vector256.Create((sbyte)decoder.MaskSlashOrUnderscore);
		Vector256<sbyte> shiftForUnderscore = Vector256.Create((sbyte)33);
		Vector256<sbyte> right = Vector256.Create(20971840).AsSByte();
		Vector256<short> right2 = Vector256.Create(69632).AsInt16();
		T* ptr = srcBytes;
		byte* ptr2 = destBytes;
		Vector256<sbyte> str;
		while (decoder.TryLoadAvxVector256(ptr, srcStart, sourceLength, out str))
		{
			Vector256<sbyte> hiNibbles = Avx2.And(Avx2.ShiftRightLogical(str.AsInt32(), 4).AsSByte(), vector);
			Vector256<sbyte> str2 = str;
			if (!decoder.TryDecode256Core(str2, hiNibbles, vector, lutLow, lutHigh, lutShift, shiftForUnderscore, out str))
			{
				break;
			}
			str = Avx2.PermuteVar8x32(Avx2.Shuffle(Avx2.MultiplyAddAdjacent(Avx2.MultiplyAddAdjacent(str.AsByte(), right), right2).AsSByte(), mask).AsInt32(), control).AsSByte();
			Avx.Store(ptr2, str.AsByte());
			ptr += 32;
			ptr2 += 24;
			if (ptr > srcEnd)
			{
				break;
			}
		}
		srcBytes = ptr;
		destBytes = ptr2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	internal static Vector128<byte> SimdShuffle(Vector128<byte> left, Vector128<byte> right, Vector128<byte> mask8F)
	{
		if (Ssse3.IsSupported)
		{
			return Ssse3.Shuffle(left, right);
		}
		return AdvSimd.Arm64.VectorTableLookup(left, right & mask8F);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	[CompExactlyDependsOn(typeof(Ssse3))]
	private unsafe static void Vector128Decode<TBase64Decoder, T>(TBase64Decoder decoder, ref T* srcBytes, ref byte* destBytes, T* srcEnd, int sourceLength, int destLength, T* srcStart, byte* destStart) where TBase64Decoder : IBase64Decoder<T> where T : unmanaged
	{
		Vector128<byte> lutHigh = Vector128.Create(decoder.Vector128LutHigh).AsByte();
		Vector128<byte> lutLow = Vector128.Create(decoder.Vector128LutLow).AsByte();
		Vector128<sbyte> lutShift = Vector128.Create(decoder.Vector128LutShift).AsSByte();
		Vector128<sbyte> vector = Vector128.Create(100663554u, 151651333u, 202182152u, uint.MaxValue).AsSByte();
		Vector128<byte> vector2 = Vector128.Create(20971840).AsByte();
		Vector128<short> right = Vector128.Create(69632).AsInt16();
		Vector128<byte> vector3 = Vector128.Create((byte)1);
		Vector128<byte> vector4 = Vector128.Create(decoder.MaskSlashOrUnderscore);
		Vector128<byte> mask8F = Vector128.Create((byte)143);
		Vector128<byte> shiftForUnderscore = Vector128.Create((byte)33);
		T* ptr = srcBytes;
		byte* ptr2 = destBytes;
		Vector128<byte> str;
		while (decoder.TryLoadVector128(ptr, srcStart, sourceLength, out str))
		{
			Vector128<byte> hiNibbles = Vector128.ShiftRightLogical(str.AsInt32(), 4).AsByte() & vector4;
			Vector128<byte> str2 = str;
			if (!decoder.TryDecode128Core(str2, hiNibbles, vector4, mask8F, lutLow, lutHigh, lutShift, shiftForUnderscore, out str))
			{
				break;
			}
			Vector128<short> left;
			if (Ssse3.IsSupported)
			{
				left = Ssse3.MultiplyAddAdjacent(str.AsByte(), vector2.AsSByte());
			}
			else
			{
				if (false)
				{
				}
				ThrowUnreachableException();
				left = default(Vector128<short>);
			}
			Vector128<int> vector5;
			if (Ssse3.IsSupported)
			{
				vector5 = Sse2.MultiplyAddAdjacent(left, right);
			}
			else
			{
				if (false)
				{
				}
				ThrowUnreachableException();
				vector5 = default(Vector128<int>);
			}
			str = SimdShuffle(vector5.AsByte(), vector.AsByte(), mask8F);
			str.Store(ptr2);
			ptr += 16;
			ptr2 += 12;
			if (ptr > srcEnd)
			{
				break;
			}
		}
		srcBytes = ptr;
		destBytes = ptr2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void WriteThreeLowOrderBytes(byte* destination, int value)
	{
		*destination = (byte)(value >> 16);
		destination[1] = (byte)(value >> 8);
		destination[2] = (byte)value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsWhiteSpace(int value)
	{
		uint num;
		return (int)((uint)(-939523840 << (int)(short)(num = (ushort)(value - 9))) & (num - 32)) < 0;
	}

	[DoesNotReturn]
	internal static void ThrowUnreachableException()
	{
		throw new UnreachableException();
	}

	internal static bool IsValid<T, TBase64Validatable>(TBase64Validatable validatable, ReadOnlySpan<T> base64Text, out int decodedLength) where T : struct where TBase64Validatable : IBase64Validatable<T>
	{
		int num = 0;
		int num2 = 0;
		T lastChar = default(T);
		if (!base64Text.IsEmpty)
		{
			while (true)
			{
				if (!base64Text.IsEmpty)
				{
					ReadOnlySpan<T> span = base64Text;
					int num3 = validatable.IndexOfAnyExcept(span);
					if ((uint)num3 >= (uint)base64Text.Length)
					{
						num += base64Text.Length;
						lastChar = base64Text[base64Text.Length - 1];
					}
					else
					{
						num += num3;
						if (num3 != 0)
						{
							lastChar = base64Text[num3 - 1];
						}
						T value = base64Text[num3];
						base64Text = base64Text.Slice(num3 + 1);
						if (validatable.IsWhiteSpace(value))
						{
							while (!base64Text.IsEmpty && validatable.IsWhiteSpace(base64Text[0]))
							{
								base64Text = base64Text.Slice(1);
							}
							continue;
						}
						if (!validatable.IsEncodingPad(value))
						{
							break;
						}
						num2 = 1;
						ReadOnlySpan<T> readOnlySpan = base64Text;
						for (int i = 0; i < readOnlySpan.Length; i++)
						{
							T value2 = readOnlySpan[i];
							if (validatable.IsEncodingPad(value2))
							{
								if (num2 >= 2)
								{
									goto end_IL_015f;
								}
								num2++;
							}
							else if (!validatable.IsWhiteSpace(value2))
							{
								goto end_IL_015f;
							}
						}
						num += num2;
					}
				}
				if (!validatable.ValidateAndDecodeLength(lastChar, num, num2, out decodedLength))
				{
					break;
				}
				return true;
				continue;
				end_IL_015f:
				break;
			}
			decodedLength = 0;
			return false;
		}
		decodedLength = 0;
		return true;
	}

	internal unsafe static OperationStatus EncodeTo<TBase64Encoder, T>(TBase64Encoder encoder, ReadOnlySpan<byte> source, Span<T> destination, out int bytesConsumed, out int bytesWritten, bool isFinalBlock = true) where TBase64Encoder : IBase64Encoder<T> where T : unmanaged
	{
		if (source.IsEmpty)
		{
			bytesConsumed = 0;
			bytesWritten = 0;
			return OperationStatus.Done;
		}
		fixed (byte* reference = &MemoryMarshal.GetReference(source))
		{
			fixed (T* reference2 = &MemoryMarshal.GetReference(destination))
			{
				int length = source.Length;
				int length2 = destination.Length;
				int maxSrcLength = encoder.GetMaxSrcLength(length, length2);
				byte* srcBytes = reference;
				T* destBytes = reference2;
				byte* ptr = reference + (uint)length;
				byte* ptr2 = reference + (uint)maxSrcLength;
				if (maxSrcLength >= 16)
				{
					byte* ptr3 = ptr2 - 64;
					if (Vector512.IsHardwareAccelerated && Avx512Vbmi.IsSupported && ptr3 >= srcBytes)
					{
						Avx512Encode(encoder, ref srcBytes, ref destBytes, ptr3, maxSrcLength, length2, reference, reference2);
						if (srcBytes == ptr)
						{
							goto IL_01ea;
						}
					}
					ptr3 = ptr2 - 32;
					if (Avx2.IsSupported && ptr3 >= srcBytes)
					{
						Avx2Encode(encoder, ref srcBytes, ref destBytes, ptr3, maxSrcLength, length2, reference, reference2);
						if (srcBytes == ptr)
						{
							goto IL_01ea;
						}
					}
					ptr3 = ptr2 - 48;
					if (false)
					{
					}
					ptr3 = ptr2 - 16;
					if (Ssse3.IsSupported ? true : false)
					{
						_ = BitConverter.IsLittleEndian;
						if (ptr3 >= srcBytes)
						{
							Vector128Encode(encoder, ref srcBytes, ref destBytes, ptr3, maxSrcLength, length2, reference, reference2);
							if (srcBytes == ptr)
							{
								goto IL_01ea;
							}
						}
					}
				}
				ref byte reference3 = ref MemoryMarshal.GetReference(encoder.EncodingMap);
				ptr2 -= 2;
				while (srcBytes < ptr2)
				{
					byte* threeBytes = srcBytes;
					T* destination2 = destBytes;
					encoder.EncodeThreeAndWrite(threeBytes, destination2, ref reference3);
					srcBytes += 3;
					destBytes += 4;
				}
				if (ptr2 + 2 == ptr)
				{
					if (!isFinalBlock)
					{
						if (srcBytes != ptr)
						{
							bytesConsumed = (int)(srcBytes - reference);
							bytesWritten = (int)(destBytes - reference2);
							return OperationStatus.NeedMoreData;
						}
					}
					else if (srcBytes + 1 == ptr)
					{
						byte* oneByte = srcBytes;
						T* dest = destBytes;
						encoder.EncodeOneOptionallyPadTwo(oneByte, dest, ref reference3);
						srcBytes++;
						destBytes += encoder.IncrementPadTwo;
					}
					else if (srcBytes + 2 == ptr)
					{
						byte* oneByte2 = srcBytes;
						T* dest2 = destBytes;
						encoder.EncodeTwoOptionallyPadOne(oneByte2, dest2, ref reference3);
						srcBytes += 2;
						destBytes += encoder.IncrementPadOne;
					}
					goto IL_01ea;
				}
				bytesConsumed = (int)(srcBytes - reference);
				bytesWritten = (int)(destBytes - reference2);
				return OperationStatus.DestinationTooSmall;
				IL_01ea:
				bytesConsumed = (int)(srcBytes - reference);
				bytesWritten = (int)(destBytes - reference2);
				return OperationStatus.Done;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx512BW))]
	[CompExactlyDependsOn(typeof(Avx512Vbmi))]
	private unsafe static void Avx512Encode<TBase64Encoder, T>(TBase64Encoder encoder, ref byte* srcBytes, ref T* destBytes, byte* srcEnd, int sourceLength, int destLength, byte* srcStart, T* destStart) where TBase64Encoder : IBase64Encoder<T> where T : unmanaged
	{
		byte* ptr = srcBytes;
		T* ptr2 = destBytes;
		Vector512<sbyte> control = Vector512.Create(16908289, 67437316, 117966343, 168495370, 219024397, 269553424, 320082451, 370611478, 421140505, 471669532, 522198559, 572727586, 623256613, 673785640, 724314667, 774843694).AsSByte();
		Vector512<sbyte> left = Vector512.Create(encoder.EncodingMap).AsSByte();
		Vector512<ushort> vector = Vector512.Create(264305664u).AsUInt16();
		Vector512<uint> condition = Vector512.Create(1056980736u);
		Vector512<ushort> count = Vector512.Create(393226u).AsUInt16();
		Vector512<ushort> count2 = Vector512.Create(524292u).AsUInt16();
		Vector512<sbyte> left2 = Vector512.Load(ptr).AsSByte();
		while (true)
		{
			left2 = Avx512Vbmi.PermuteVar64x8(left2, control);
			Vector512<ushort> vector2 = Avx512BW.ShiftRightLogicalVariable(left2.AsUInt16() & vector, count).AsUInt16();
			Vector512<ushort> vector3 = Avx512BW.ShiftLeftLogicalVariable(left2.AsUInt16(), count2).AsUInt16();
			left2 = Vector512.ConditionalSelect(condition, vector3.AsUInt32(), vector2.AsUInt32()).AsSByte();
			left2 = Avx512Vbmi.PermuteVar64x8(left, left2);
			encoder.StoreVector512ToDestination(ptr2, destStart, destLength, left2.AsByte());
			ptr += 48;
			ptr2 += 64;
			if (ptr > srcEnd)
			{
				break;
			}
			left2 = Vector512.Load(ptr).AsSByte();
		}
		srcBytes = ptr;
		destBytes = ptr2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx2))]
	private unsafe static void Avx2Encode<TBase64Encoder, T>(TBase64Encoder encoder, ref byte* srcBytes, ref T* destBytes, byte* srcEnd, int sourceLength, int destLength, byte* srcStart, T* destStart) where TBase64Encoder : IBase64Encoder<T> where T : unmanaged
	{
		Vector256<sbyte> mask = Vector256.Create(5, 4, 6, 5, 8, 7, 9, 8, 11, 10, 12, 11, 14, 13, 15, 14, 1, 0, 2, 1, 4, 3, 5, 4, 7, 6, 8, 7, 10, 9, 11, 10);
		Vector256<sbyte> value = Vector256.Create(65, 71, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, encoder.Avx2LutChar62, encoder.Avx2LutChar63, 0, 0, 65, 71, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, encoder.Avx2LutChar62, encoder.Avx2LutChar63, 0, 0);
		Vector256<sbyte> right = Vector256.Create(264305664).AsSByte();
		Vector256<sbyte> right2 = Vector256.Create(4129776).AsSByte();
		Vector256<ushort> right3 = Vector256.Create(67108928).AsUInt16();
		Vector256<short> right4 = Vector256.Create(16777232).AsInt16();
		Vector256<byte> right5 = Vector256.Create((byte)51);
		Vector256<sbyte> right6 = Vector256.Create((sbyte)25);
		byte* ptr = srcBytes;
		T* ptr2 = destBytes;
		Vector256<sbyte> vector = Avx.LoadVector256(ptr).AsSByte();
		vector = Avx2.PermuteVar8x32(vector.AsInt32(), Vector256.Create(0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0, 4, 0, 0, 0, 5, 0, 0, 0, 6, 0, 0, 0).AsInt32()).AsSByte();
		ptr -= 4;
		while (true)
		{
			vector = Avx2.Shuffle(vector, mask);
			Vector256<sbyte> vector2 = Avx2.And(vector, right);
			Vector256<sbyte> vector3 = Avx2.And(vector, right2);
			Vector256<ushort> vector4 = Avx2.MultiplyHigh(vector2.AsUInt16(), right3);
			vector = Avx2.Or(right: Avx2.MultiplyLow(vector3.AsInt16(), right4).AsSByte(), left: vector4.AsSByte());
			Vector256<byte> vector5 = Avx2.SubtractSaturate(vector.AsByte(), right5);
			Vector256<sbyte> mask2 = Avx2.Subtract(right: Avx2.CompareGreaterThan(vector, right6), left: vector5.AsSByte());
			vector = Avx2.Add(vector, Avx2.Shuffle(value, mask2));
			encoder.StoreVector256ToDestination(ptr2, destStart, destLength, vector.AsByte());
			ptr += 24;
			ptr2 += 32;
			if (ptr > srcEnd)
			{
				break;
			}
			vector = Avx.LoadVector256(ptr).AsSByte();
		}
		srcBytes = ptr + 4;
		destBytes = ptr2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	private unsafe static void Vector128Encode<TBase64Encoder, T>(TBase64Encoder encoder, ref byte* srcBytes, ref T* destBytes, byte* srcEnd, int sourceLength, int destLength, byte* srcStart, T* destStart) where TBase64Encoder : IBase64Encoder<T> where T : unmanaged
	{
		Vector128<byte> right = Vector128.Create(16908289, 67437316, 117966343, 168495370).AsByte();
		Vector128<byte> left = Vector128.Create(4244391745u, 4244438268u, 4244438268u, encoder.Ssse3AdvSimdLutE3).AsByte();
		Vector128<byte> vector = Vector128.Create(264305664).AsByte();
		Vector128<byte> vector2 = Vector128.Create(4129776).AsByte();
		Vector128<ushort> right2 = Vector128.Create(67108928).AsUInt16();
		Vector128<short> vector3 = Vector128.Create(16777232).AsInt16();
		Vector128<byte> right3 = Vector128.Create((byte)51);
		Vector128<sbyte> right4 = Vector128.Create((sbyte)25);
		Vector128<byte> mask8F = Vector128.Create((byte)143);
		byte* ptr = srcBytes;
		T* ptr2 = destBytes;
		do
		{
			Vector128<byte> left2 = Vector128.LoadUnsafe(in *ptr);
			left2 = SimdShuffle(left2, right, mask8F);
			Vector128<byte> vector4 = left2 & vector;
			Vector128<byte> vector5 = left2 & vector2;
			Vector128<ushort> vector6;
			if (Ssse3.IsSupported)
			{
				vector6 = Sse2.MultiplyHigh(vector4.AsUInt16(), right2);
			}
			else
			{
				if (false)
				{
				}
				ThrowUnreachableException();
				vector6 = default(Vector128<ushort>);
			}
			Vector128<short> vector7 = vector5.AsInt16() * vector3;
			left2 = vector6.AsByte() | vector7.AsByte();
			Vector128<byte> vector8;
			if (Ssse3.IsSupported)
			{
				vector8 = Sse2.SubtractSaturate(left2.AsByte(), right3);
			}
			else
			{
				if (false)
				{
				}
				ThrowUnreachableException();
				vector8 = default(Vector128<byte>);
			}
			Vector128<sbyte> vector9 = Vector128.GreaterThan(left2.AsSByte(), right4);
			Vector128<sbyte> vector10 = vector8.AsSByte() - vector9;
			left2 += SimdShuffle(left, vector10.AsByte(), mask8F);
			encoder.StoreVector128ToDestination(ptr2, destStart, destLength, left2);
			ptr += 12;
			ptr2 += 16;
		}
		while (ptr <= srcEnd);
		srcBytes = ptr;
		destBytes = ptr2;
	}

	internal unsafe static OperationStatus EncodeToUtf8InPlace<TBase64Encoder>(TBase64Encoder encoder, Span<byte> buffer, int dataLength, out int bytesWritten) where TBase64Encoder : IBase64Encoder<byte>
	{
		if (buffer.IsEmpty)
		{
			bytesWritten = 0;
			return OperationStatus.Done;
		}
		fixed (byte* reference = &MemoryMarshal.GetReference(buffer))
		{
			int maxEncodedLength = encoder.GetMaxEncodedLength(dataLength);
			if (buffer.Length < maxEncodedLength)
			{
				bytesWritten = 0;
				return OperationStatus.DestinationTooSmall;
			}
			int num = (int)((uint)dataLength % 3u);
			uint num2 = encoder.GetInPlaceDestinationLength(maxEncodedLength, num);
			uint num3 = (uint)(dataLength - num);
			ref byte reference2 = ref MemoryMarshal.GetReference(encoder.EncodingMap);
			if (num != 0)
			{
				if (num == 1)
				{
					encoder.EncodeOneOptionallyPadTwo(reference + num3, reference + num2, ref reference2);
				}
				else
				{
					encoder.EncodeTwoOptionallyPadOne(reference + num3, reference + num2, ref reference2);
				}
				num2 -= 4;
			}
			num3 -= 3;
			while ((int)num3 >= 0)
			{
				uint value = Encode(reference + num3, ref reference2);
				Unsafe.WriteUnaligned(reference + num2, value);
				num2 -= 4;
				num3 -= 3;
			}
			bytesWritten = maxEncodedLength;
			return OperationStatus.Done;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static uint Encode(byte* threeBytes, ref byte encodingMap)
	{
		byte num = *threeBytes;
		uint num2 = threeBytes[1];
		uint num3 = threeBytes[2];
		uint num4 = (uint)(num << 16) | (num2 << 8) | num3;
		byte i = Unsafe.Add(ref encodingMap, (nint)(num4 >> 18));
		uint i2 = Unsafe.Add(ref encodingMap, (nint)((num4 >> 12) & 0x3F));
		uint i3 = Unsafe.Add(ref encodingMap, (nint)((num4 >> 6) & 0x3F));
		uint i4 = Unsafe.Add(ref encodingMap, (nint)(num4 & 0x3F));
		return ConstructResult(i, i2, i3, i4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ConstructResult(uint i0, uint i1, uint i2, uint i3)
	{
		_ = BitConverter.IsLittleEndian;
		return i0 | (i1 << 8) | (i2 << 16) | (i3 << 24);
	}
}
