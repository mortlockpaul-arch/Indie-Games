using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text.Unicode;

namespace System.Text;

public static class Ascii
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct ToUpperConversion
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct ToLowerConversion
	{
	}

	private interface ILoader<TLeft, TRight> where TLeft : unmanaged, INumberBase<TLeft> where TRight : unmanaged, INumberBase<TRight>
	{
		static abstract nuint Count128 { get; }

		static abstract nuint Count256 { get; }

		static abstract nuint Count512 { get; }

		static abstract Vector128<TRight> Load128(ref TLeft ptr);

		static abstract Vector256<TRight> Load256(ref TLeft ptr);

		static abstract Vector512<TRight> Load512(ref TLeft ptr);

		static abstract bool EqualAndAscii256(ref TLeft left, ref TRight right);

		static abstract bool EqualAndAscii512(ref TLeft left, ref TRight right);
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct PlainLoader<T> : ILoader<T, T> where T : unmanaged, INumberBase<T>
	{
		public static nuint Count128 => (uint)Vector128<T>.Count;

		public static nuint Count256 => (uint)Vector256<T>.Count;

		public static nuint Count512 => (uint)Vector512<T>.Count;

		public static Vector128<T> Load128(ref T ptr)
		{
			return Vector128.LoadUnsafe(in ptr);
		}

		public static Vector256<T> Load256(ref T ptr)
		{
			return Vector256.LoadUnsafe(in ptr);
		}

		public static Vector512<T> Load512(ref T ptr)
		{
			return Vector512.LoadUnsafe(in ptr);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(Avx))]
		public static bool EqualAndAscii256(ref T left, ref T right)
		{
			Vector256<T> vector = Vector256.LoadUnsafe(in left);
			Vector256<T> vector2 = Vector256.LoadUnsafe(in right);
			if (vector != vector2 || !AllCharsInVectorAreAscii(vector))
			{
				return false;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EqualAndAscii512(ref T left, ref T right)
		{
			Vector512<T> vector = Vector512.LoadUnsafe(in left);
			Vector512<T> vector2 = Vector512.LoadUnsafe(in right);
			if (vector != vector2 || !AllCharsInVectorAreAscii(vector))
			{
				return false;
			}
			return true;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private readonly struct WideningLoader : ILoader<byte, ushort>
	{
		public static nuint Count128 => 8u;

		public static nuint Count256 => (uint)Vector128<byte>.Count;

		public static nuint Count512 => (uint)Vector256<byte>.Count;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector128<ushort> Load128(ref byte ptr)
		{
			if (false)
			{
			}
			if (Sse2.IsSupported)
			{
				return Sse2.UnpackLow(Vector128.CreateScalarUnsafe(Unsafe.ReadUnaligned<long>(in ptr)).AsByte(), Vector128<byte>.Zero).AsUInt16();
			}
			if (false)
			{
			}
			var (lower, upper) = Vector64.Widen(Vector64.LoadUnsafe(in ptr));
			return Vector128.Create(lower, upper);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector256<ushort> Load256(ref byte ptr)
		{
			var (lower, upper) = Vector128.Widen(Vector128.LoadUnsafe(in ptr));
			return Vector256.Create(lower, upper);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector512<ushort> Load512(ref byte ptr)
		{
			return Vector512.WidenLower(Vector256.LoadUnsafe(in ptr).ToVector512());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[CompExactlyDependsOn(typeof(Avx))]
		public static bool EqualAndAscii256(ref byte utf8, ref ushort utf16)
		{
			Vector256<byte> vector = Vector256.LoadUnsafe(in utf8);
			if (!AllCharsInVectorAreAscii(vector))
			{
				return false;
			}
			(Vector256<ushort> Lower, Vector256<ushort> Upper) tuple = Vector256.Widen(vector);
			Vector256<ushort> item = tuple.Lower;
			Vector256<ushort> item2 = tuple.Upper;
			Vector256<ushort> vector2 = Vector256.LoadUnsafe(in utf16);
			Vector256<ushort> vector3 = Vector256.LoadUnsafe(in utf16, (uint)Vector256<ushort>.Count);
			if (((item ^ vector2) | (item2 ^ vector3)) != Vector256<ushort>.Zero)
			{
				return false;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool EqualAndAscii512(ref byte utf8, ref ushort utf16)
		{
			Vector512<byte> vector = Vector512.LoadUnsafe(in utf8);
			if (!AllCharsInVectorAreAscii(vector))
			{
				return false;
			}
			(Vector512<ushort> Lower, Vector512<ushort> Upper) tuple = Vector512.Widen(vector);
			Vector512<ushort> item = tuple.Lower;
			Vector512<ushort> item2 = tuple.Upper;
			Vector512<ushort> vector2 = Vector512.LoadUnsafe(in utf16);
			Vector512<ushort> vector3 = Vector512.LoadUnsafe(in utf16, (uint)Vector512<ushort>.Count);
			if (((item ^ vector2) | (item2 ^ vector3)) != Vector512<ushort>.Zero)
			{
				return false;
			}
			return true;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValid(ReadOnlySpan<byte> value)
	{
		return IsValidCore(ref MemoryMarshal.GetReference(value), value.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValid(ReadOnlySpan<char> value)
	{
		return IsValidCore(ref Unsafe.As<char, ushort>(ref MemoryMarshal.GetReference(value)), value.Length);
	}

	public static bool IsValid(byte value)
	{
		return value <= 127;
	}

	public static bool IsValid(char value)
	{
		return value <= '\u007f';
	}

	private unsafe static bool IsValidCore<T>(ref T searchSpace, int length) where T : unmanaged
	{
		if (!Vector128.IsHardwareAccelerated || length < Vector128<T>.Count)
		{
			uint num = (uint)(8 / sizeof(T));
			if (length < num)
			{
				if (typeof(T) == typeof(byte) && length >= 4)
				{
					return AllBytesInUInt32AreAscii(Unsafe.ReadUnaligned<uint>(in Unsafe.As<T, byte>(ref searchSpace)) | Unsafe.ReadUnaligned<uint>(in Unsafe.As<T, byte>(ref Unsafe.Add(ref searchSpace, length - 4))));
				}
				for (nuint num2 = 0u; num2 < (uint)length; num2++)
				{
					if ((typeof(T) == typeof(byte)) ? (Unsafe.BitCast<T, byte>(Unsafe.Add(ref searchSpace, num2)) > 127) : (Unsafe.BitCast<T, char>(Unsafe.Add(ref searchSpace, num2)) > '\u007f'))
					{
						return false;
					}
				}
				return true;
			}
			nuint num3 = 0u;
			if (!Vector128.IsHardwareAccelerated && length > 2 * num)
			{
				nuint num4;
				for (num4 = (nuint)length - (nuint)(2 * num); num3 < num4; num3 += 2 * num)
				{
					if (!AllCharsInUInt64AreAscii<T>(Unsafe.ReadUnaligned<ulong>(in Unsafe.As<T, byte>(ref Unsafe.Add(ref searchSpace, num3))) | Unsafe.ReadUnaligned<ulong>(in Unsafe.As<T, byte>(ref Unsafe.Add(ref searchSpace, num3 + num)))))
					{
						return false;
					}
				}
				num3 = num4;
			}
			return AllCharsInUInt64AreAscii<T>(Unsafe.ReadUnaligned<ulong>(in Unsafe.As<T, byte>(ref Unsafe.Add(ref searchSpace, num3))) | Unsafe.ReadUnaligned<ulong>(in Unsafe.Subtract(ref Unsafe.As<T, byte>(ref Unsafe.Add(ref searchSpace, length)), 8)));
		}
		ref T source = ref Unsafe.Add(ref searchSpace, length);
		if (length <= 2 * Vector128<T>.Count)
		{
			return AllCharsInVectorAreAscii(Vector128.LoadUnsafe(in searchSpace) | Vector128.LoadUnsafe(in Unsafe.Subtract(ref source, Vector128<T>.Count)));
		}
		if (Avx.IsSupported)
		{
			if (length <= 2 * Vector256<T>.Count)
			{
				return AllCharsInVectorAreAscii(Vector256.LoadUnsafe(in searchSpace) | Vector256.LoadUnsafe(in Unsafe.Subtract(ref source, Vector256<T>.Count)));
			}
			if (length > 4 * Vector256<T>.Count)
			{
				if (!AllCharsInVectorAreAscii(Vector256.LoadUnsafe(in searchSpace) | Vector256.LoadUnsafe(in searchSpace, (nuint)Vector256<T>.Count) | Vector256.LoadUnsafe(in searchSpace, (nuint)2u * (nuint)Vector256<T>.Count) | Vector256.LoadUnsafe(in searchSpace, (nuint)3u * (nuint)Vector256<T>.Count)))
				{
					return false;
				}
				nuint num5 = (nuint)4u * (nuint)Vector256<T>.Count;
				nuint num6 = Unsafe.OpportunisticMisalignment(in searchSpace, (uint)Vector256<byte>.Count) / (nuint)sizeof(T);
				num5 -= num6;
				nuint num7;
				for (num7 = (nuint)(length - (nint)4 * (nint)Vector256<T>.Count); num5 < num7; num5 += (nuint)((nint)4 * (nint)Vector256<T>.Count))
				{
					ref T source2 = ref Unsafe.Add(ref searchSpace, num5);
					if (!AllCharsInVectorAreAscii(Vector256.LoadUnsafe(in source2) | Vector256.LoadUnsafe(in source2, (nuint)Vector256<T>.Count) | Vector256.LoadUnsafe(in source2, (nuint)2u * (nuint)Vector256<T>.Count) | Vector256.LoadUnsafe(in source2, (nuint)3u * (nuint)Vector256<T>.Count)))
					{
						return false;
					}
				}
				searchSpace = ref Unsafe.Add(ref searchSpace, num7);
			}
			return AllCharsInVectorAreAscii(Vector256.LoadUnsafe(in searchSpace) | Vector256.LoadUnsafe(in searchSpace, (nuint)Vector256<T>.Count) | Vector256.LoadUnsafe(in Unsafe.Subtract(ref source, 2 * Vector256<T>.Count)) | Vector256.LoadUnsafe(in Unsafe.Subtract(ref source, Vector256<T>.Count)));
		}
		if (length > 4 * Vector128<T>.Count)
		{
			if (!AllCharsInVectorAreAscii(Vector128.LoadUnsafe(in searchSpace) | Vector128.LoadUnsafe(in searchSpace, (nuint)Vector128<T>.Count) | Vector128.LoadUnsafe(in searchSpace, (nuint)2u * (nuint)Vector128<T>.Count) | Vector128.LoadUnsafe(in searchSpace, (nuint)3u * (nuint)Vector128<T>.Count)))
			{
				return false;
			}
			nuint num8 = (nuint)4u * (nuint)Vector128<T>.Count;
			nuint num9 = Unsafe.OpportunisticMisalignment(in searchSpace, (uint)Vector128<byte>.Count) / (nuint)sizeof(T);
			num8 -= num9;
			nuint num10;
			for (num10 = (nuint)(length - (nint)4 * (nint)Vector128<T>.Count); num8 < num10; num8 += (nuint)((nint)4 * (nint)Vector128<T>.Count))
			{
				ref T source3 = ref Unsafe.Add(ref searchSpace, num8);
				if (!AllCharsInVectorAreAscii(Vector128.LoadUnsafe(in source3) | Vector128.LoadUnsafe(in source3, (nuint)Vector128<T>.Count) | Vector128.LoadUnsafe(in source3, (nuint)2u * (nuint)Vector128<T>.Count) | Vector128.LoadUnsafe(in source3, (nuint)3u * (nuint)Vector128<T>.Count)))
				{
					return false;
				}
			}
			searchSpace = ref Unsafe.Add(ref searchSpace, num10);
		}
		return AllCharsInVectorAreAscii(Vector128.LoadUnsafe(in searchSpace) | Vector128.LoadUnsafe(in searchSpace, (nuint)Vector128<T>.Count) | Vector128.LoadUnsafe(in Unsafe.Subtract(ref source, 2 * Vector128<T>.Count)) | Vector128.LoadUnsafe(in Unsafe.Subtract(ref source, Vector128<T>.Count)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OperationStatus ToUpper(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
	{
		return ChangeCase<byte, byte, ToUpperConversion>(source, destination, out bytesWritten);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OperationStatus ToUpper(ReadOnlySpan<char> source, Span<char> destination, out int charsWritten)
	{
		return ChangeCase<ushort, ushort, ToUpperConversion>(MemoryMarshal.Cast<char, ushort>(source), MemoryMarshal.Cast<char, ushort>(destination), out charsWritten);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OperationStatus ToUpper(ReadOnlySpan<byte> source, Span<char> destination, out int charsWritten)
	{
		return ChangeCase<byte, ushort, ToUpperConversion>(source, MemoryMarshal.Cast<char, ushort>(destination), out charsWritten);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OperationStatus ToUpper(ReadOnlySpan<char> source, Span<byte> destination, out int bytesWritten)
	{
		return ChangeCase<ushort, byte, ToUpperConversion>(MemoryMarshal.Cast<char, ushort>(source), destination, out bytesWritten);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OperationStatus ToLower(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
	{
		return ChangeCase<byte, byte, ToLowerConversion>(source, destination, out bytesWritten);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OperationStatus ToLower(ReadOnlySpan<char> source, Span<char> destination, out int charsWritten)
	{
		return ChangeCase<ushort, ushort, ToLowerConversion>(MemoryMarshal.Cast<char, ushort>(source), MemoryMarshal.Cast<char, ushort>(destination), out charsWritten);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OperationStatus ToLower(ReadOnlySpan<byte> source, Span<char> destination, out int charsWritten)
	{
		return ChangeCase<byte, ushort, ToLowerConversion>(source, MemoryMarshal.Cast<char, ushort>(destination), out charsWritten);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OperationStatus ToLower(ReadOnlySpan<char> source, Span<byte> destination, out int bytesWritten)
	{
		return ChangeCase<ushort, byte, ToLowerConversion>(MemoryMarshal.Cast<char, ushort>(source), destination, out bytesWritten);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OperationStatus ToLowerInPlace(Span<byte> value, out int bytesWritten)
	{
		return ChangeCase<byte, ToLowerConversion>(value, out bytesWritten);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OperationStatus ToLowerInPlace(Span<char> value, out int charsWritten)
	{
		return ChangeCase<ushort, ToLowerConversion>(MemoryMarshal.Cast<char, ushort>(value), out charsWritten);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OperationStatus ToUpperInPlace(Span<byte> value, out int bytesWritten)
	{
		return ChangeCase<byte, ToUpperConversion>(value, out bytesWritten);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static OperationStatus ToUpperInPlace(Span<char> value, out int charsWritten)
	{
		return ChangeCase<ushort, ToUpperConversion>(MemoryMarshal.Cast<char, ushort>(value), out charsWritten);
	}

	private unsafe static OperationStatus ChangeCase<TFrom, TTo, TCasing>(ReadOnlySpan<TFrom> source, Span<TTo> destination, out int destinationElementsWritten) where TFrom : unmanaged, IBinaryInteger<TFrom> where TTo : unmanaged, IBinaryInteger<TTo> where TCasing : struct
	{
		if (MemoryMarshal.AsBytes(source).Overlaps(MemoryMarshal.AsBytes(destination)))
		{
			ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_SpanOverlappedOperation);
		}
		nuint num;
		OperationStatus result;
		if (source.Length <= destination.Length)
		{
			num = (uint)source.Length;
			result = OperationStatus.Done;
		}
		else
		{
			num = (uint)destination.Length;
			result = OperationStatus.DestinationTooSmall;
		}
		fixed (TFrom* reference = &MemoryMarshal.GetReference(source))
		{
			fixed (TTo* reference2 = &MemoryMarshal.GetReference(destination))
			{
				nuint num2 = ChangeCase<TFrom, TTo, TCasing>(reference, reference2, num);
				destinationElementsWritten = (int)num2;
				if (num != num2)
				{
					return OperationStatus.InvalidData;
				}
				return result;
			}
		}
	}

	private unsafe static OperationStatus ChangeCase<T, TCasing>(Span<T> buffer, out int elementsWritten) where T : unmanaged, IBinaryInteger<T> where TCasing : struct
	{
		fixed (T* reference = &MemoryMarshal.GetReference(buffer))
		{
			nuint num = ChangeCase<T, T, TCasing>(reference, reference, (nuint)buffer.Length);
			elementsWritten = (int)num;
			if (elementsWritten != buffer.Length)
			{
				return OperationStatus.InvalidData;
			}
			return OperationStatus.Done;
		}
	}

	private unsafe static nuint ChangeCase<TFrom, TTo, TCasing>(TFrom* pSrc, TTo* pDest, nuint elementCount) where TFrom : unmanaged, IBinaryInteger<TFrom> where TTo : unmanaged, IBinaryInteger<TTo> where TCasing : struct
	{
		bool flag = sizeof(TFrom) == 1;
		bool flag2 = sizeof(TTo) == 1;
		bool flag3 = flag && !flag2;
		bool flag4 = typeof(TFrom) == typeof(TTo);
		bool flag5 = typeof(TCasing) == typeof(ToUpperConversion);
		uint num = (uint)(sizeof(Vector128<byte>) / sizeof(TFrom));
		nuint num2 = 0u;
		if (flag4 || Vector128.IsHardwareAccelerated)
		{
			if (Vector128.IsHardwareAccelerated && elementCount >= num)
			{
				Vector128<TFrom> vector = Vector128.LoadUnsafe(in *pSrc);
				if (!VectorContainsNonAsciiChar(vector))
				{
					TFrom val = TFrom.CreateTruncating(1 << 8 * sizeof(TFrom) - 1);
					Vector128<TFrom> vector2 = Vector128.Create(flag5 ? (val + TFrom.CreateTruncating('a')) : (val + TFrom.CreateTruncating('A')));
					Vector128<TFrom> right = Vector128.Create(val + TFrom.CreateTruncating(26));
					Vector128<TFrom> vector3 = Vector128.Create(TFrom.CreateTruncating(32));
					Vector128<TFrom> vector4 = SignedLessThan(vector - vector2, right);
					vector ^= vector4 & vector3;
					ChangeWidthAndWriteTo(vector, pDest, 0u);
					uint num3 = num * (uint)sizeof(TTo);
					num2 = num - (uint)pDest % num3 / (uint)sizeof(TTo);
					while (true)
					{
						if (elementCount - num2 < num)
						{
							if (num2 == elementCount)
							{
								break;
							}
							num2 = elementCount - num;
						}
						vector = Vector128.LoadUnsafe(in *pSrc, num2);
						if (!VectorContainsNonAsciiChar(vector))
						{
							vector4 = SignedLessThan(vector - vector2, right);
							vector ^= vector4 & vector3;
							ChangeWidthAndWriteTo(vector, pDest, num2);
							num2 += num;
							continue;
						}
						goto IL_01db;
					}
					goto IL_0469;
				}
			}
			goto IL_01db;
		}
		goto IL_0464;
		IL_0469:
		return num2;
		IL_01db:
		do
		{
			_ = 8;
			if (elementCount - num2 < (nuint)(8 / sizeof(TFrom)))
			{
				break;
			}
			ulong value = Unsafe.ReadUnaligned<ulong>(pSrc + num2);
			if (flag)
			{
				if (!Utf8Utility.AllBytesInUInt64AreAscii(value))
				{
					break;
				}
				value = (flag5 ? Utf8Utility.ConvertAllAsciiBytesInUInt64ToUppercase(value) : Utf8Utility.ConvertAllAsciiBytesInUInt64ToLowercase(value));
			}
			else
			{
				if (!Utf16Utility.AllCharsInUInt64AreAscii(value))
				{
					break;
				}
				value = (flag5 ? Utf16Utility.ConvertAllAsciiCharsInUInt64ToUppercase(value) : Utf16Utility.ConvertAllAsciiCharsInUInt64ToLowercase(value));
			}
			if (flag4)
			{
				Unsafe.WriteUnaligned(pDest + num2, value);
			}
			else
			{
				Vector128<ulong> vector5 = Vector128.CreateScalarUnsafe(value);
				if (flag3)
				{
					Vector128.WidenLower(vector5.AsByte()).StoreUnsafe(ref *(ushort*)pDest, num2);
				}
				else
				{
					Vector128<ushort> vector6 = vector5.AsUInt16();
					Vector128<uint> vector7 = Vector128.Narrow(vector6, vector6).AsUInt32();
					Unsafe.WriteUnaligned(pDest + num2, vector7.ToScalar());
				}
			}
			num2 += (nuint)(8 / sizeof(TFrom));
		}
		while (!Vector128.IsHardwareAccelerated);
		while (elementCount - num2 >= (nuint)(4 / sizeof(TFrom)))
		{
			uint value2 = Unsafe.ReadUnaligned<uint>(pSrc + num2);
			if (flag)
			{
				if (!Utf8Utility.AllBytesInUInt32AreAscii(value2))
				{
					break;
				}
				value2 = (flag5 ? Utf8Utility.ConvertAllAsciiBytesInUInt32ToUppercase(value2) : Utf8Utility.ConvertAllAsciiBytesInUInt32ToLowercase(value2));
			}
			else
			{
				if (!Utf16Utility.AllCharsInUInt32AreAscii(value2))
				{
					break;
				}
				value2 = (flag5 ? Utf16Utility.ConvertAllAsciiCharsInUInt32ToUppercase(value2) : Utf16Utility.ConvertAllAsciiCharsInUInt32ToLowercase(value2));
			}
			if (flag4)
			{
				Unsafe.WriteUnaligned(pDest + num2, value2);
			}
			else
			{
				Vector128<uint> vector8 = Vector128.CreateScalarUnsafe(value2);
				if (flag3)
				{
					Vector128<ulong> vector9 = Vector128.WidenLower(vector8.AsByte()).AsUInt64();
					Unsafe.WriteUnaligned(pDest + num2, vector9.ToScalar());
				}
				else
				{
					Vector128<ushort> vector10 = vector8.AsUInt16();
					Vector128<ushort> vector11 = Vector128.Narrow(vector10, vector10).AsUInt16();
					Unsafe.WriteUnaligned(pDest + num2, vector11.ToScalar());
				}
			}
			num2 += (nuint)(4 / sizeof(TFrom));
			_ = 8;
			if (Vector128.IsHardwareAccelerated)
			{
				break;
			}
		}
		goto IL_0464;
		IL_0464:
		for (; num2 < elementCount; num2++)
		{
			uint num4 = uint.CreateTruncating(pSrc[num2]);
			if (!UnicodeUtility.IsAsciiCodePoint(num4))
			{
				break;
			}
			if (flag5)
			{
				if (UnicodeUtility.IsInRangeInclusive(num4, 97u, 122u))
				{
					num4 -= 32;
				}
			}
			else if (UnicodeUtility.IsInRangeInclusive(num4, 65u, 90u))
			{
				num4 += 32;
			}
			pDest[num2] = TTo.CreateTruncating(num4);
		}
		goto IL_0469;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void ChangeWidthAndWriteTo<TFrom, TTo>(Vector128<TFrom> vector, TTo* pDest, nuint elementOffset) where TFrom : unmanaged where TTo : unmanaged
	{
		if (sizeof(TFrom) == sizeof(TTo))
		{
			vector.As<TFrom, TTo>().StoreUnsafe(ref *pDest, elementOffset);
		}
		else if (sizeof(TFrom) == 1 && sizeof(TTo) == 2)
		{
			if (Vector256.IsHardwareAccelerated)
			{
				Vector256.WidenLower(vector.AsByte().ToVector256Unsafe()).StoreUnsafe(ref *(ushort*)pDest, elementOffset);
				return;
			}
			Vector128.WidenLower(vector.AsByte()).StoreUnsafe(ref *(ushort*)pDest, elementOffset);
			Vector128.WidenUpper(vector.AsByte()).StoreUnsafe(ref *(ushort*)pDest, elementOffset + 8);
		}
		else
		{
			if (sizeof(TFrom) != 2 || sizeof(TTo) != 1)
			{
				throw new NotSupportedException();
			}
			ExtractAsciiVector(vector.AsUInt16(), vector.AsUInt16()).StoreLowerUnsafe(ref *(byte*)pDest, elementOffset);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static Vector128<T> SignedLessThan<T>(Vector128<T> left, Vector128<T> right) where T : unmanaged
	{
		if (sizeof(T) == 1)
		{
			return Vector128.LessThan(left.AsSByte(), right.AsSByte()).As<sbyte, T>();
		}
		if (sizeof(T) == 2)
		{
			return Vector128.LessThan(left.AsInt16(), right.AsInt16()).As<short, T>();
		}
		throw new NotSupportedException();
	}

	public static bool Equals(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
	{
		if (left.Length == right.Length)
		{
			return Equals<byte, byte, PlainLoader<byte>>(ref MemoryMarshal.GetReference(left), ref MemoryMarshal.GetReference(right), (uint)right.Length);
		}
		return false;
	}

	public static bool Equals(ReadOnlySpan<byte> left, ReadOnlySpan<char> right)
	{
		if (left.Length == right.Length)
		{
			return Equals<byte, ushort, WideningLoader>(ref MemoryMarshal.GetReference(left), ref Unsafe.As<char, ushort>(ref MemoryMarshal.GetReference(right)), (uint)right.Length);
		}
		return false;
	}

	public static bool Equals(ReadOnlySpan<char> left, ReadOnlySpan<byte> right)
	{
		return Equals(right, left);
	}

	public static bool Equals(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
	{
		if (left.Length == right.Length)
		{
			return Equals<ushort, ushort, PlainLoader<ushort>>(ref Unsafe.As<char, ushort>(ref MemoryMarshal.GetReference(left)), ref Unsafe.As<char, ushort>(ref MemoryMarshal.GetReference(right)), (uint)right.Length);
		}
		return false;
	}

	private static bool Equals<TLeft, TRight, TLoader>(ref TLeft left, ref TRight right, nuint length) where TLeft : unmanaged, INumberBase<TLeft> where TRight : unmanaged, INumberBase<TRight> where TLoader : struct, ILoader<TLeft, TRight>
	{
		if (!Vector128.IsHardwareAccelerated || length < (uint)Vector128<TLeft>.Count)
		{
			for (nuint num = 0u; num < length; num++)
			{
				uint num2 = uint.CreateTruncating(Unsafe.Add(ref left, num));
				uint num3 = uint.CreateTruncating(Unsafe.Add(ref right, num));
				if (num2 != num3 || !UnicodeUtility.IsAsciiCodePoint(num2))
				{
					return false;
				}
			}
		}
		else if (Vector512.IsHardwareAccelerated && length >= (uint)Vector512<TLeft>.Count)
		{
			ref TLeft reference = ref left;
			ref TRight reference2 = ref right;
			ref TRight right2 = ref Unsafe.Add(ref reference2, length - (uint)Vector512<TLeft>.Count);
			do
			{
				if (!TLoader.EqualAndAscii512(ref reference, ref reference2))
				{
					return false;
				}
				reference2 = ref Unsafe.Add(ref reference2, Vector512<TLeft>.Count);
				reference = ref Unsafe.Add(ref reference, Vector512<TLeft>.Count);
			}
			while (Unsafe.IsAddressLessThanOrEqualTo(in reference2, in right2));
			if (length % (uint)Vector512<TLeft>.Count != 0)
			{
				return TLoader.EqualAndAscii512(ref Unsafe.Add(ref left, length - (uint)Vector512<TLeft>.Count), ref right2);
			}
		}
		else if (Avx.IsSupported && length >= (uint)Vector256<TLeft>.Count)
		{
			ref TLeft reference3 = ref left;
			ref TRight reference4 = ref right;
			ref TRight right3 = ref Unsafe.Add(ref reference4, length - (uint)Vector256<TLeft>.Count);
			do
			{
				if (!TLoader.EqualAndAscii256(ref reference3, ref reference4))
				{
					return false;
				}
				reference4 = ref Unsafe.Add(ref reference4, Vector256<TLeft>.Count);
				reference3 = ref Unsafe.Add(ref reference3, Vector256<TLeft>.Count);
			}
			while (Unsafe.IsAddressLessThanOrEqualTo(in reference4, in right3));
			if (length % (uint)Vector256<TLeft>.Count != 0)
			{
				return TLoader.EqualAndAscii256(ref Unsafe.Add(ref left, length - (uint)Vector256<TLeft>.Count), ref right3);
			}
		}
		else
		{
			ref TLeft reference5 = ref left;
			ref TLeft ptr = ref Unsafe.Add(ref reference5, length - TLoader.Count128);
			ref TRight reference6 = ref right;
			ref TRight reference7 = ref Unsafe.Add(ref reference6, length - (uint)Vector128<TRight>.Count);
			do
			{
				Vector128<TRight> vector = TLoader.Load128(ref reference5);
				Vector128<TRight> vector2 = Vector128.LoadUnsafe(in reference6);
				if (vector != vector2 || !AllCharsInVectorAreAscii(vector))
				{
					return false;
				}
				reference6 = ref Unsafe.Add(ref reference6, (uint)Vector128<TRight>.Count);
				reference5 = ref Unsafe.Add(ref reference5, TLoader.Count128);
			}
			while (Unsafe.IsAddressLessThanOrEqualTo(in reference6, in reference7));
			if (length % (uint)Vector128<TRight>.Count != 0)
			{
				Vector128<TRight> vector = TLoader.Load128(ref ptr);
				Vector128<TRight> vector2 = Vector128.LoadUnsafe(in reference7);
				if (vector != vector2 || !AllCharsInVectorAreAscii(vector))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static bool EqualsIgnoreCase(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
	{
		if (left.Length == right.Length)
		{
			return EqualsIgnoreCase<byte, byte, PlainLoader<byte>>(ref MemoryMarshal.GetReference(left), ref MemoryMarshal.GetReference(right), (uint)right.Length);
		}
		return false;
	}

	public static bool EqualsIgnoreCase(ReadOnlySpan<byte> left, ReadOnlySpan<char> right)
	{
		if (left.Length == right.Length)
		{
			return EqualsIgnoreCase<byte, ushort, WideningLoader>(ref MemoryMarshal.GetReference(left), ref Unsafe.As<char, ushort>(ref MemoryMarshal.GetReference(right)), (uint)right.Length);
		}
		return false;
	}

	public static bool EqualsIgnoreCase(ReadOnlySpan<char> left, ReadOnlySpan<byte> right)
	{
		return EqualsIgnoreCase(right, left);
	}

	public static bool EqualsIgnoreCase(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
	{
		if (left.Length == right.Length)
		{
			return EqualsIgnoreCase<ushort, ushort, PlainLoader<ushort>>(ref Unsafe.As<char, ushort>(ref MemoryMarshal.GetReference(left)), ref Unsafe.As<char, ushort>(ref MemoryMarshal.GetReference(right)), (uint)right.Length);
		}
		return false;
	}

	internal static bool EqualsIgnoreCase(ref char left, ref char right, nuint length)
	{
		return EqualsIgnoreCase<ushort, ushort, PlainLoader<ushort>>(ref Unsafe.As<char, ushort>(ref left), ref Unsafe.As<char, ushort>(ref right), length);
	}

	private static bool EqualsIgnoreCase<TLeft, TRight, TLoader>(ref TLeft left, ref TRight right, nuint length) where TLeft : unmanaged, INumberBase<TLeft> where TRight : unmanaged, INumberBase<TRight> where TLoader : ILoader<TLeft, TRight>
	{
		if (!Vector128.IsHardwareAccelerated || length < (uint)Vector128<TRight>.Count)
		{
			for (nuint num = 0u; num < length; num++)
			{
				uint num2 = uint.CreateTruncating(Unsafe.Add(ref left, num));
				uint num3 = uint.CreateTruncating(Unsafe.Add(ref right, num));
				if (!UnicodeUtility.IsAsciiCodePoint(num2 | num3))
				{
					return false;
				}
				if (num2 != num3)
				{
					num2 |= 0x20;
					if (num2 - 97 > 25)
					{
						return false;
					}
					if (num2 != (num3 | 0x20))
					{
						return false;
					}
				}
			}
		}
		else if (Vector512.IsHardwareAccelerated && length >= (uint)Vector512<TRight>.Count)
		{
			ref TLeft reference = ref left;
			ref TLeft ptr = ref Unsafe.Add(ref reference, length - TLoader.Count512);
			ref TRight reference2 = ref right;
			ref TRight reference3 = ref Unsafe.Add(ref reference2, length - (uint)Vector512<TRight>.Count);
			Vector512<TRight> vector = Vector512.Create(TRight.CreateTruncating(32));
			Vector512<TRight> vector2 = Vector512.Create(TRight.CreateTruncating('a'));
			Vector512<TRight> right2 = Vector512.Create(TRight.CreateTruncating(25));
			do
			{
				Vector512<TRight> vector3 = TLoader.Load512(ref reference);
				Vector512<TRight> vector4 = Vector512.LoadUnsafe(in reference2);
				if (!AllCharsInVectorAreAscii(vector3 | vector4))
				{
					return false;
				}
				Vector512<TRight> vector5 = ~Vector512.Equals(vector3, vector4);
				if (vector5 != Vector512<TRight>.Zero)
				{
					vector3 |= vector;
					vector4 |= vector;
					if (Vector512.GreaterThanAny((vector3 - vector2) & vector5, right2) || vector3 != vector4)
					{
						return false;
					}
				}
				reference2 = ref Unsafe.Add(ref reference2, (uint)Vector512<TRight>.Count);
				reference = ref Unsafe.Add(ref reference, TLoader.Count512);
			}
			while (Unsafe.IsAddressLessThanOrEqualTo(in reference2, in reference3));
			if (length % (uint)Vector512<TRight>.Count != 0)
			{
				Vector512<TRight> vector3 = TLoader.Load512(ref ptr);
				Vector512<TRight> vector4 = Vector512.LoadUnsafe(in reference3);
				if (!AllCharsInVectorAreAscii(vector3 | vector4))
				{
					return false;
				}
				Vector512<TRight> vector6 = ~Vector512.Equals(vector3, vector4);
				if (vector6 != Vector512<TRight>.Zero)
				{
					vector3 |= vector;
					vector4 |= vector;
					if (Vector512.GreaterThanAny((vector3 - vector2) & vector6, right2) || vector3 != vector4)
					{
						return false;
					}
				}
			}
		}
		else if (Avx.IsSupported && length >= (uint)Vector256<TRight>.Count)
		{
			ref TLeft reference4 = ref left;
			ref TLeft ptr2 = ref Unsafe.Add(ref reference4, length - TLoader.Count256);
			ref TRight reference5 = ref right;
			ref TRight reference6 = ref Unsafe.Add(ref reference5, length - (uint)Vector256<TRight>.Count);
			Vector256<TRight> vector7 = Vector256.Create(TRight.CreateTruncating(32));
			Vector256<TRight> vector8 = Vector256.Create(TRight.CreateTruncating('a'));
			Vector256<TRight> right3 = Vector256.Create(TRight.CreateTruncating(25));
			do
			{
				Vector256<TRight> vector9 = TLoader.Load256(ref reference4);
				Vector256<TRight> vector10 = Vector256.LoadUnsafe(in reference5);
				if (!AllCharsInVectorAreAscii(vector9 | vector10))
				{
					return false;
				}
				Vector256<TRight> vector11 = ~Vector256.Equals(vector9, vector10);
				if (vector11 != Vector256<TRight>.Zero)
				{
					vector9 |= vector7;
					vector10 |= vector7;
					if (Vector256.GreaterThanAny((vector9 - vector8) & vector11, right3) || vector9 != vector10)
					{
						return false;
					}
				}
				reference5 = ref Unsafe.Add(ref reference5, (uint)Vector256<TRight>.Count);
				reference4 = ref Unsafe.Add(ref reference4, TLoader.Count256);
			}
			while (Unsafe.IsAddressLessThanOrEqualTo(in reference5, in reference6));
			if (length % (uint)Vector256<TRight>.Count != 0)
			{
				Vector256<TRight> vector9 = TLoader.Load256(ref ptr2);
				Vector256<TRight> vector10 = Vector256.LoadUnsafe(in reference6);
				if (!AllCharsInVectorAreAscii(vector9 | vector10))
				{
					return false;
				}
				Vector256<TRight> vector12 = ~Vector256.Equals(vector9, vector10);
				if (vector12 != Vector256<TRight>.Zero)
				{
					vector9 |= vector7;
					vector10 |= vector7;
					if (Vector256.GreaterThanAny((vector9 - vector8) & vector12, right3) || vector9 != vector10)
					{
						return false;
					}
				}
			}
		}
		else
		{
			ref TLeft reference7 = ref left;
			ref TLeft ptr3 = ref Unsafe.Add(ref reference7, length - TLoader.Count128);
			ref TRight reference8 = ref right;
			ref TRight reference9 = ref Unsafe.Add(ref reference8, length - (uint)Vector128<TRight>.Count);
			Vector128<TRight> vector13 = Vector128.Create(TRight.CreateTruncating(32));
			Vector128<TRight> vector14 = Vector128.Create(TRight.CreateTruncating('a'));
			Vector128<TRight> right4 = Vector128.Create(TRight.CreateTruncating(25));
			do
			{
				Vector128<TRight> vector15 = TLoader.Load128(ref reference7);
				Vector128<TRight> vector16 = Vector128.LoadUnsafe(in reference8);
				if (!AllCharsInVectorAreAscii(vector15 | vector16))
				{
					return false;
				}
				Vector128<TRight> vector17 = ~Vector128.Equals(vector15, vector16);
				if (vector17 != Vector128<TRight>.Zero)
				{
					vector15 |= vector13;
					vector16 |= vector13;
					if (Vector128.GreaterThanAny((vector15 - vector14) & vector17, right4) || vector15 != vector16)
					{
						return false;
					}
				}
				reference8 = ref Unsafe.Add(ref reference8, (uint)Vector128<TRight>.Count);
				reference7 = ref Unsafe.Add(ref reference7, TLoader.Count128);
			}
			while (Unsafe.IsAddressLessThanOrEqualTo(in reference8, in reference9));
			if (length % (uint)Vector128<TRight>.Count != 0)
			{
				Vector128<TRight> vector15 = TLoader.Load128(ref ptr3);
				Vector128<TRight> vector16 = Vector128.LoadUnsafe(in reference9);
				if (!AllCharsInVectorAreAscii(vector15 | vector16))
				{
					return false;
				}
				Vector128<TRight> vector18 = ~Vector128.Equals(vector15, vector16);
				if (vector18 != Vector128<TRight>.Zero)
				{
					vector15 |= vector13;
					vector16 |= vector13;
					if (Vector128.GreaterThanAny((vector15 - vector14) & vector18, right4) || vector15 != vector16)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public unsafe static OperationStatus ToUtf16(ReadOnlySpan<byte> source, Span<char> destination, out int charsWritten)
	{
		nuint num;
		OperationStatus result;
		if (source.Length <= destination.Length)
		{
			num = (uint)source.Length;
			result = OperationStatus.Done;
		}
		else
		{
			num = (uint)destination.Length;
			result = OperationStatus.DestinationTooSmall;
		}
		fixed (byte* reference = &MemoryMarshal.GetReference(source))
		{
			fixed (char* reference2 = &MemoryMarshal.GetReference(destination))
			{
				nuint num2 = WidenAsciiToUtf16(reference, reference2, num);
				charsWritten = (int)num2;
				if (num != num2)
				{
					return OperationStatus.InvalidData;
				}
				return result;
			}
		}
	}

	public unsafe static OperationStatus FromUtf16(ReadOnlySpan<char> source, Span<byte> destination, out int bytesWritten)
	{
		nuint num;
		OperationStatus result;
		if (source.Length <= destination.Length)
		{
			num = (uint)source.Length;
			result = OperationStatus.Done;
		}
		else
		{
			num = (uint)destination.Length;
			result = OperationStatus.DestinationTooSmall;
		}
		fixed (char* reference = &MemoryMarshal.GetReference(source))
		{
			fixed (byte* reference2 = &MemoryMarshal.GetReference(destination))
			{
				nuint num2 = NarrowUtf16ToAscii(reference, reference2, num);
				bytesWritten = (int)num2;
				if (num != num2)
				{
					return OperationStatus.InvalidData;
				}
				return result;
			}
		}
	}

	public static Range Trim(ReadOnlySpan<byte> value)
	{
		return TrimHelper(value, TrimType.Both);
	}

	public static Range Trim(ReadOnlySpan<char> value)
	{
		return TrimHelper(value, TrimType.Both);
	}

	public static Range TrimStart(ReadOnlySpan<byte> value)
	{
		return TrimHelper(value, TrimType.Head);
	}

	public static Range TrimStart(ReadOnlySpan<char> value)
	{
		return TrimHelper(value, TrimType.Head);
	}

	public static Range TrimEnd(ReadOnlySpan<byte> value)
	{
		return TrimHelper(value, TrimType.Tail);
	}

	public static Range TrimEnd(ReadOnlySpan<char> value)
	{
		return TrimHelper(value, TrimType.Tail);
	}

	private static Range TrimHelper<T>(ReadOnlySpan<T> value, TrimType trimType) where T : unmanaged, IBinaryInteger<T>
	{
		int i = 0;
		if ((trimType & TrimType.Head) != 0)
		{
			for (; i < value.Length; i++)
			{
				uint num = uint.CreateTruncating(value[i]) - 1;
				if (num > 31 || (-2147475712 & (1 << (int)num)) == 0)
				{
					break;
				}
			}
		}
		int num2 = value.Length - 1;
		if ((trimType & TrimType.Tail) != 0)
		{
			while (i <= num2)
			{
				uint num3 = uint.CreateTruncating(value[num2]) - 1;
				if (num3 > 31 || (-2147475712 & (1 << (int)num3)) == 0)
				{
					break;
				}
				num2--;
			}
		}
		return i..(num2 + 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool AllBytesInUInt64AreAscii(ulong value)
	{
		return (value & 0x8080808080808080uL) == 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool AllCharsInUInt32AreAscii(uint value)
	{
		return (value & 0xFF80FF80u) == 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool AllCharsInUInt64AreAscii(ulong value)
	{
		return (value & 0xFF80FF80FF80FF80uL) == 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool AllCharsInUInt64AreAscii<T>(ulong value) where T : unmanaged
	{
		if (!(typeof(T) == typeof(byte)))
		{
			return AllCharsInUInt64AreAscii(value);
		}
		return AllBytesInUInt64AreAscii(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool FirstCharInUInt32IsAscii(uint value)
	{
		_ = BitConverter.IsLittleEndian;
		if ((value & 0xFF80) != 0)
		{
			if (!BitConverter.IsLittleEndian)
			{
			}
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static nuint GetIndexOfFirstNonAsciiByte(byte* pBuffer, nuint bufferLength)
	{
		if (!Vector512.IsHardwareAccelerated && !Vector256.IsHardwareAccelerated && (Sse2.IsSupported ? true : false))
		{
			return GetIndexOfFirstNonAsciiByte_Intrinsified(pBuffer, bufferLength);
		}
		return GetIndexOfFirstNonAsciiByte_Vector(pBuffer, bufferLength);
	}

	private unsafe static nuint GetIndexOfFirstNonAsciiByte_Vector(byte* pBuffer, nuint bufferLength)
	{
		byte* ptr = pBuffer;
		if (Vector512.IsHardwareAccelerated && bufferLength >= (uint)(2 * Vector512<byte>.Count))
		{
			if (Vector512.Load(pBuffer).ExtractMostSignificantBits() == 0L)
			{
				byte* ptr2 = pBuffer + bufferLength - 64;
				pBuffer = (byte*)((nuint)(pBuffer + 64) & (nuint)(~(nint)63));
				while (Vector512.LoadAligned(pBuffer).ExtractMostSignificantBits() == 0L)
				{
					pBuffer += 64;
					if (pBuffer > ptr2)
					{
						break;
					}
				}
				bufferLength -= (nuint)pBuffer;
				bufferLength = (nuint)(bufferLength + ptr);
			}
		}
		else if (Vector256.IsHardwareAccelerated && bufferLength >= (uint)(2 * Vector256<byte>.Count))
		{
			if (Vector256.Load(pBuffer).ExtractMostSignificantBits() == 0)
			{
				byte* ptr3 = pBuffer + bufferLength - 32;
				pBuffer = (byte*)((nuint)(pBuffer + 32) & (nuint)(~(nint)31));
				while (Vector256.LoadAligned(pBuffer).ExtractMostSignificantBits() == 0)
				{
					pBuffer += 32;
					if (pBuffer > ptr3)
					{
						break;
					}
				}
				bufferLength -= (nuint)pBuffer;
				bufferLength = (nuint)(bufferLength + ptr);
			}
		}
		else if (Vector128.IsHardwareAccelerated && bufferLength >= (uint)(2 * Vector128<byte>.Count) && !VectorContainsNonAsciiChar(Vector128.Load(pBuffer)))
		{
			byte* ptr4 = pBuffer + bufferLength - 16;
			pBuffer = (byte*)((nuint)(pBuffer + 16) & (nuint)(~(nint)15));
			while (!VectorContainsNonAsciiChar(Vector128.LoadAligned(pBuffer)))
			{
				pBuffer += 16;
				if (pBuffer > ptr4)
				{
					break;
				}
			}
			bufferLength -= (nuint)pBuffer;
			bufferLength = (nuint)(bufferLength + ptr);
		}
		while (true)
		{
			uint num;
			if (bufferLength >= 8)
			{
				num = Unsafe.ReadUnaligned<uint>(pBuffer);
				uint num2 = Unsafe.ReadUnaligned<uint>(pBuffer + 4);
				if (!AllBytesInUInt32AreAscii(num | num2))
				{
					if (AllBytesInUInt32AreAscii(num))
					{
						num = num2;
						pBuffer += 4;
					}
					goto IL_01a8;
				}
				pBuffer += 8;
				bufferLength -= 8;
				continue;
			}
			if ((bufferLength & 4) != 0)
			{
				num = Unsafe.ReadUnaligned<uint>(pBuffer);
				if (!AllBytesInUInt32AreAscii(num))
				{
					goto IL_01a8;
				}
				pBuffer += 4;
			}
			if ((bufferLength & 2) != 0)
			{
				num = Unsafe.ReadUnaligned<ushort>(pBuffer);
				if (!AllBytesInUInt32AreAscii(num) && BitConverter.IsLittleEndian)
				{
					goto IL_01a8;
				}
				pBuffer += 2;
			}
			if ((bufferLength & 1) != 0 && *pBuffer >= 0)
			{
				pBuffer++;
			}
			break;
			IL_01a8:
			pBuffer += CountNumberOfLeadingAsciiBytesFromUInt32WithSomeNonAsciiData(num);
			break;
		}
		return (nuint)(pBuffer - (nuint)ptr);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool ContainsNonAsciiByte_Sse2(uint sseMask)
	{
		return sseMask != 0;
	}

	private unsafe static nuint GetIndexOfFirstNonAsciiByte_Intrinsified(byte* pBuffer, nuint bufferLength)
	{
		uint num = (uint)sizeof(Vector128<byte>);
		nuint num2 = num - 1;
		if (!BitConverter.IsLittleEndian)
		{
		}
		Vector128<byte> vector = Vector128.Create((ushort)4097).AsByte();
		uint num3 = uint.MaxValue;
		uint num4 = uint.MaxValue;
		uint num5 = uint.MaxValue;
		uint num6 = uint.MaxValue;
		byte* ptr = pBuffer;
		if (bufferLength >= num)
		{
			if (Sse2.IsSupported)
			{
				num3 = (uint)Sse2.MoveMask(Sse2.LoadVector128(pBuffer));
				if (!ContainsNonAsciiByte_Sse2(num3))
				{
					if (bufferLength < 2 * num)
					{
						goto IL_0113;
					}
					pBuffer = (byte*)((nuint)(pBuffer + num) & ~num2);
					bufferLength = (nuint)(bufferLength + ptr);
					bufferLength -= (nuint)pBuffer;
					if (bufferLength < 2 * num)
					{
						goto IL_00e4;
					}
					byte* ptr2 = pBuffer + bufferLength - 2 * num;
					while (true)
					{
						if (Sse2.IsSupported)
						{
							Vector128<byte> value = Sse2.LoadAlignedVector128(pBuffer);
							Vector128<byte> value2 = Sse2.LoadAlignedVector128(pBuffer + num);
							num3 = (uint)Sse2.MoveMask(value);
							num4 = (uint)Sse2.MoveMask(value2);
							if (ContainsNonAsciiByte_Sse2(num3 | num4))
							{
								break;
							}
							pBuffer += 2 * num;
							if (pBuffer <= ptr2)
							{
								continue;
							}
							goto IL_00e4;
						}
						if (false)
						{
						}
						throw new PlatformNotSupportedException();
					}
					if (!Sse2.IsSupported)
					{
						if (false)
						{
						}
						throw new PlatformNotSupportedException();
					}
					if (!ContainsNonAsciiByte_Sse2(num3))
					{
						pBuffer += num;
						num3 = num4;
					}
				}
				goto IL_0180;
			}
			if (false)
			{
			}
			throw new PlatformNotSupportedException();
		}
		if ((bufferLength & 8) != 0)
		{
			_ = 8;
			ulong num7 = Unsafe.ReadUnaligned<ulong>(pBuffer);
			if (!AllBytesInUInt64AreAscii(num7))
			{
				num7 &= 0x8080808080808080uL;
				pBuffer += (nuint)(BitOperations.TrailingZeroCount(num7) >> 3);
				goto IL_0158;
			}
			pBuffer += 8;
		}
		if ((bufferLength & 4) != 0)
		{
			uint value3 = Unsafe.ReadUnaligned<uint>(pBuffer);
			if (!AllBytesInUInt32AreAscii(value3))
			{
				pBuffer += CountNumberOfLeadingAsciiBytesFromUInt32WithSomeNonAsciiData(value3);
				goto IL_0158;
			}
			pBuffer += 4;
		}
		if ((bufferLength & 2) != 0)
		{
			uint value3 = Unsafe.ReadUnaligned<ushort>(pBuffer);
			if (!AllBytesInUInt32AreAscii(value3))
			{
				pBuffer += (nuint)(((nint)(sbyte)value3 >> 7) + 1);
				goto IL_0158;
			}
			pBuffer += 2;
		}
		if ((bufferLength & 1) != 0 && *pBuffer >= 0)
		{
			pBuffer++;
		}
		goto IL_0158;
		IL_00e4:
		if ((bufferLength & num) == 0)
		{
			goto IL_0119;
		}
		if (Sse2.IsSupported)
		{
			num3 = (uint)Sse2.MoveMask(Sse2.LoadAlignedVector128(pBuffer));
			if (!ContainsNonAsciiByte_Sse2(num3))
			{
				goto IL_0113;
			}
			goto IL_0180;
		}
		if (false)
		{
		}
		throw new PlatformNotSupportedException();
		IL_0158:
		return (nuint)(pBuffer - (nuint)ptr);
		IL_0113:
		pBuffer += num;
		goto IL_0119;
		IL_0180:
		if (Sse2.IsSupported)
		{
			pBuffer += (uint)BitOperations.TrailingZeroCount(num3);
			goto IL_0158;
		}
		if (false)
		{
		}
		throw new PlatformNotSupportedException();
		IL_0119:
		if (((byte)bufferLength & num2) != 0)
		{
			pBuffer += (bufferLength & num2) - num;
			if (!Sse2.IsSupported)
			{
				if (false)
				{
				}
				throw new PlatformNotSupportedException();
			}
			num3 = (uint)Sse2.MoveMask(Sse2.LoadVector128(pBuffer));
			if (ContainsNonAsciiByte_Sse2(num3))
			{
				goto IL_0180;
			}
			pBuffer += num;
		}
		goto IL_0158;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static nuint GetIndexOfFirstNonAsciiChar(char* pBuffer, nuint bufferLength)
	{
		if (!Vector512.IsHardwareAccelerated && !Vector256.IsHardwareAccelerated && (Sse2.IsSupported ? true : false))
		{
			return GetIndexOfFirstNonAsciiChar_Intrinsified(pBuffer, bufferLength);
		}
		return GetIndexOfFirstNonAsciiChar_Vector(pBuffer, bufferLength);
	}

	private unsafe static nuint GetIndexOfFirstNonAsciiChar_Vector(char* pBuffer, nuint bufferLength)
	{
		char* ptr = pBuffer;
		if (Vector512.IsHardwareAccelerated && bufferLength >= (uint)(2 * Vector512<ushort>.Count))
		{
			if (!VectorContainsNonAsciiChar(Vector512.Load((ushort*)pBuffer)))
			{
				char* ptr2 = pBuffer + bufferLength - 32u;
				pBuffer = (char*)((nuint)(pBuffer + 32) & (nuint)(~(nint)63));
				while (!VectorContainsNonAsciiChar(Vector512.LoadAligned((ushort*)pBuffer)))
				{
					pBuffer += 32u;
					if (pBuffer > ptr2)
					{
						break;
					}
				}
				bufferLength -= (nuint)(nint)(pBuffer - ptr);
			}
		}
		else if (Vector256.IsHardwareAccelerated && bufferLength >= (uint)(2 * Vector256<ushort>.Count))
		{
			if (!VectorContainsNonAsciiChar(Vector256.Load((ushort*)pBuffer)))
			{
				char* ptr3 = pBuffer + bufferLength - 16u;
				pBuffer = (char*)((nuint)(pBuffer + 16) & (nuint)(~(nint)31));
				while (!VectorContainsNonAsciiChar(Vector256.LoadAligned((ushort*)pBuffer)))
				{
					pBuffer += 16u;
					if (pBuffer > ptr3)
					{
						break;
					}
				}
				bufferLength -= (nuint)(nint)(pBuffer - ptr);
			}
		}
		else if (Vector128.IsHardwareAccelerated && bufferLength >= (uint)(2 * Vector128<ushort>.Count) && !VectorContainsNonAsciiChar(Vector128.Load((ushort*)pBuffer)))
		{
			char* ptr4 = pBuffer + bufferLength - 8u;
			pBuffer = (char*)((nuint)(pBuffer + 8) & (nuint)(~(nint)15));
			while (!VectorContainsNonAsciiChar(Vector128.LoadAligned((ushort*)pBuffer)))
			{
				pBuffer += 8u;
				if (pBuffer > ptr4)
				{
					break;
				}
			}
			bufferLength -= (nuint)(nint)(pBuffer - ptr);
		}
		while (true)
		{
			uint num;
			if (bufferLength >= 4)
			{
				num = Unsafe.ReadUnaligned<uint>(pBuffer);
				uint num2 = Unsafe.ReadUnaligned<uint>(pBuffer + 2);
				if (!AllCharsInUInt32AreAscii(num | num2))
				{
					if (AllCharsInUInt32AreAscii(num))
					{
						num = num2;
						pBuffer += 2;
					}
					goto IL_01bf;
				}
				pBuffer += 4;
				bufferLength -= 4;
				continue;
			}
			if ((bufferLength & 2) != 0)
			{
				num = Unsafe.ReadUnaligned<uint>(pBuffer);
				if (!AllCharsInUInt32AreAscii(num))
				{
					goto IL_01bf;
				}
				pBuffer += 2;
			}
			if ((bufferLength & 1) != 0 && *pBuffer <= '\u007f')
			{
				pBuffer++;
			}
			break;
			IL_01bf:
			if (FirstCharInUInt32IsAscii(num))
			{
				pBuffer++;
			}
			break;
		}
		return (nuint)(pBuffer - ptr);
	}

	private unsafe static nuint GetIndexOfFirstNonAsciiChar_Intrinsified(char* pBuffer, nuint bufferLength)
	{
		if (bufferLength == 0)
		{
			return 0u;
		}
		uint num = 8u;
		char* ptr = pBuffer;
		Vector128<ushort> vector;
		if (bufferLength >= num)
		{
			vector = Vector128.LoadUnsafe(in *(ushort*)pBuffer);
			if (!VectorContainsNonAsciiChar(vector))
			{
				bufferLength <<= 1;
				if (bufferLength < 32)
				{
					goto IL_009c;
				}
				pBuffer = (char*)((nuint)(pBuffer + 8) & (nuint)(~(nint)15));
				nuint num2 = (nuint)((byte*)pBuffer - (nuint)ptr);
				bufferLength -= num2;
				if (bufferLength < 32)
				{
					goto IL_0086;
				}
				char* ptr2 = (char*)((byte*)pBuffer + bufferLength) - 16;
				Vector128<ushort> vector2;
				while (true)
				{
					vector = Vector128.LoadUnsafe(in *(ushort*)pBuffer);
					vector2 = Vector128.LoadUnsafe(in *(ushort*)pBuffer, num);
					if (VectorContainsNonAsciiChar(vector | vector2))
					{
						break;
					}
					pBuffer += 2 * num;
					if (pBuffer <= ptr2)
					{
						continue;
					}
					goto IL_0086;
				}
				if (!VectorContainsNonAsciiChar(vector))
				{
					pBuffer += num;
					vector = vector2;
				}
			}
			goto IL_00f0;
		}
		if ((bufferLength & 4) != 0)
		{
			_ = 8;
			ulong num3 = Unsafe.ReadUnaligned<ulong>(pBuffer);
			if (!AllCharsInUInt64AreAscii(num3))
			{
				num3 &= 0xFF80FF80FF80FF80uL;
				pBuffer = (char*)((byte*)pBuffer + (nuint)((BitOperations.TrailingZeroCount(num3) >> 3) & ~(nint)1));
				goto IL_00d4;
			}
			pBuffer += 4;
		}
		if ((bufferLength & 2) != 0)
		{
			uint value = Unsafe.ReadUnaligned<uint>(pBuffer);
			if (!AllCharsInUInt32AreAscii(value))
			{
				if (FirstCharInUInt32IsAscii(value))
				{
					pBuffer++;
				}
				goto IL_00d4;
			}
			pBuffer += 2;
		}
		if ((bufferLength & 1) != 0 && *pBuffer <= '\u007f')
		{
			pBuffer++;
		}
		goto IL_00d4;
		IL_00f0:
		if (Sse2.IsSupported)
		{
			Vector128<ushort> right = Vector128.Create((ushort)32640);
			uint num4 = (uint)Sse2.MoveMask(Sse2.AddSaturate(vector, right).AsByte());
			num4 &= 0xAAAA;
			pBuffer = (char*)((byte*)pBuffer + (uint)BitOperations.TrailingZeroCount(num4) - 1);
			goto IL_00d4;
		}
		if (false)
		{
		}
		throw new PlatformNotSupportedException();
		IL_00d4:
		return (nuint)(pBuffer - ptr);
		IL_009c:
		pBuffer += num;
		goto IL_00a6;
		IL_00a6:
		if (((byte)bufferLength & 0xF) != 0)
		{
			pBuffer = (char*)((byte*)pBuffer + (bufferLength & 0xF)) - 8;
			vector = Vector128.LoadUnsafe(in *(ushort*)pBuffer);
			if (VectorContainsNonAsciiChar(vector))
			{
				goto IL_00f0;
			}
			pBuffer += num;
		}
		goto IL_00d4;
		IL_0086:
		if ((bufferLength & 0x10) == 0)
		{
			goto IL_00a6;
		}
		vector = Vector128.LoadUnsafe(in *(ushort*)pBuffer);
		if (!VectorContainsNonAsciiChar(vector))
		{
			goto IL_009c;
		}
		goto IL_00f0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void NarrowFourUtf16CharsToAsciiAndWriteToBuffer(ref byte outputBuffer, ulong value)
	{
		if (Sse2.X64.IsSupported)
		{
			Vector128<short> vector = Sse2.X64.ConvertScalarToVector128UInt64(value).AsInt16();
			Vector128<uint> value2 = Sse2.PackUnsignedSaturate(vector, vector).AsUInt32();
			Unsafe.WriteUnaligned(ref outputBuffer, Sse2.ConvertToUInt32(value2));
			return;
		}
		if (false)
		{
		}
		_ = BitConverter.IsLittleEndian;
		outputBuffer = (byte)value;
		value >>= 16;
		Unsafe.Add(ref outputBuffer, 1) = (byte)value;
		value >>= 16;
		Unsafe.Add(ref outputBuffer, 2) = (byte)value;
		value >>= 16;
		Unsafe.Add(ref outputBuffer, 3) = (byte)value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void NarrowTwoUtf16CharsToAsciiAndWriteToBuffer(ref byte outputBuffer, uint value)
	{
		_ = BitConverter.IsLittleEndian;
		outputBuffer = (byte)value;
		Unsafe.Add(ref outputBuffer, 1) = (byte)(value >> 16);
	}

	internal unsafe static nuint NarrowUtf16ToAscii(char* pUtf16Buffer, byte* pAsciiBuffer, nuint elementCount)
	{
		nuint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		ulong num4 = 0uL;
		_ = BitConverter.IsLittleEndian;
		if (Vector128.IsHardwareAccelerated && elementCount >= (uint)(2 * Vector128<byte>.Count))
		{
			_ = 8;
			num4 = Unsafe.ReadUnaligned<ulong>(pUtf16Buffer);
			if (!AllCharsInUInt64AreAscii(num4))
			{
				goto IL_0123;
			}
			num = ((Vector512.IsHardwareAccelerated && elementCount >= (uint)(2 * Vector512<byte>.Count)) ? NarrowUtf16ToAscii_Intrinsified_512(pUtf16Buffer, pAsciiBuffer, elementCount) : ((!Vector256.IsHardwareAccelerated || elementCount < (uint)(2 * Vector256<byte>.Count)) ? NarrowUtf16ToAscii_Intrinsified(pUtf16Buffer, pAsciiBuffer, elementCount) : NarrowUtf16ToAscii_Intrinsified_256(pUtf16Buffer, pAsciiBuffer, elementCount)));
		}
		nuint num5 = elementCount - num;
		if (num5 < 4)
		{
			goto IL_00cf;
		}
		nuint num6 = num + num5 - 4;
		while (true)
		{
			_ = 8;
			num4 = Unsafe.ReadUnaligned<ulong>(pUtf16Buffer + num);
			if (!AllCharsInUInt64AreAscii(num4))
			{
				break;
			}
			NarrowFourUtf16CharsToAsciiAndWriteToBuffer(ref pAsciiBuffer[num], num4);
			num += 4;
			if (num <= num6)
			{
				continue;
			}
			goto IL_00cf;
		}
		goto IL_0123;
		IL_0121:
		return num;
		IL_015c:
		if (FirstCharInUInt32IsAscii(num2))
		{
			if (!BitConverter.IsLittleEndian)
			{
			}
			pAsciiBuffer[num] = (byte)num2;
			num++;
		}
		goto IL_0121;
		IL_00cf:
		if (((int)num5 & 2) != 0)
		{
			num2 = Unsafe.ReadUnaligned<uint>(pUtf16Buffer + num);
			if (!AllCharsInUInt32AreAscii(num2))
			{
				goto IL_015c;
			}
			NarrowTwoUtf16CharsToAsciiAndWriteToBuffer(ref pAsciiBuffer[num], num2);
			num += 2;
		}
		if (((int)num5 & 1) != 0)
		{
			num2 = pUtf16Buffer[num];
			if (num2 <= 127)
			{
				pAsciiBuffer[num] = (byte)num2;
				num++;
			}
		}
		goto IL_0121;
		IL_0123:
		_ = 8;
		_ = BitConverter.IsLittleEndian;
		num2 = (uint)num4;
		if (AllCharsInUInt32AreAscii(num2))
		{
			NarrowTwoUtf16CharsToAsciiAndWriteToBuffer(ref pAsciiBuffer[num], num2);
			_ = BitConverter.IsLittleEndian;
			num2 = (uint)(num4 >> 32);
			num += 2;
		}
		goto IL_015c;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool VectorContainsNonAsciiChar(Vector128<byte> asciiVector)
	{
		if (Sse41.IsSupported)
		{
			return (asciiVector & Vector128.Create((byte)128)) != Vector128<byte>.Zero;
		}
		if (false)
		{
		}
		return asciiVector.ExtractMostSignificantBits() != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool VectorContainsNonAsciiChar(Vector128<ushort> utf16Vector)
	{
		if (Sse41.IsSupported)
		{
			return (utf16Vector & Vector128.Create((ushort)65408)) != Vector128<ushort>.Zero;
		}
		if (Sse2.IsSupported)
		{
			Vector128<ushort> right = Vector128.Create((ushort)32640);
			return (Sse2.MoveMask(Sse2.AddSaturate(utf16Vector, right).AsByte()) & 0xAAAA) != 0;
		}
		if (false)
		{
		}
		return (utf16Vector & Vector128.Create((ushort)65408)) != Vector128<ushort>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool VectorContainsNonAsciiChar(Vector256<ushort> utf16Vector)
	{
		return (utf16Vector & Vector256.Create((ushort)65408)) != Vector256<ushort>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool VectorContainsNonAsciiChar(Vector512<ushort> utf16Vector)
	{
		return (utf16Vector & Vector512.Create((ushort)65408)) != Vector512<ushort>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool VectorContainsNonAsciiChar<T>(Vector128<T> vector) where T : unmanaged
	{
		if (!(typeof(T) == typeof(byte)))
		{
			return VectorContainsNonAsciiChar(vector.AsUInt16());
		}
		return VectorContainsNonAsciiChar(vector.AsByte());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool AllCharsInVectorAreAscii<T>(Vector128<T> vector) where T : unmanaged
	{
		if (typeof(T) == typeof(byte))
		{
			if (!Sse41.IsSupported)
			{
				_ = 0;
				return vector.AsByte().ExtractMostSignificantBits() == 0;
			}
			return (vector.AsByte() & Vector128.Create((byte)128)) == Vector128<byte>.Zero;
		}
		_ = 0;
		return (vector.AsUInt16() & Vector128.Create((ushort)65408)) == Vector128<ushort>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[CompExactlyDependsOn(typeof(Avx))]
	private static bool AllCharsInVectorAreAscii<T>(Vector256<T> vector) where T : unmanaged
	{
		if (typeof(T) == typeof(byte))
		{
			if (!Avx.IsSupported)
			{
				return vector.AsByte().ExtractMostSignificantBits() == 0;
			}
			return (vector.AsByte() & Vector256.Create((byte)128)) == Vector256<byte>.Zero;
		}
		return (vector.AsUInt16() & Vector256.Create((ushort)65408)) == Vector256<ushort>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool AllCharsInVectorAreAscii<T>(Vector512<T> vector) where T : unmanaged
	{
		if (typeof(T) == typeof(byte))
		{
			return vector.AsByte().ExtractMostSignificantBits() == 0;
		}
		return (vector.AsUInt16() & Vector512.Create((ushort)65408)) == Vector512<ushort>.Zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Vector128<byte> ExtractAsciiVector(Vector128<ushort> vectorFirst, Vector128<ushort> vectorSecond)
	{
		if (Sse2.IsSupported)
		{
			return Sse2.PackUnsignedSaturate(vectorFirst.AsInt16(), vectorSecond.AsInt16());
		}
		if (false)
		{
		}
		if (false)
		{
		}
		return Vector128.Narrow(vectorFirst, vectorSecond);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Vector256<byte> ExtractAsciiVector(Vector256<ushort> vectorFirst, Vector256<ushort> vectorSecond)
	{
		if (!Avx2.IsSupported)
		{
			return Vector256.Narrow(vectorFirst, vectorSecond);
		}
		return PackedSpanHelpers.FixUpPackedVector256Result(Avx2.PackUnsignedSaturate(vectorFirst.AsInt16(), vectorSecond.AsInt16()));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Vector512<byte> ExtractAsciiVector(Vector512<ushort> vectorFirst, Vector512<ushort> vectorSecond)
	{
		if (!Avx512BW.IsSupported)
		{
			return Vector512.Narrow(vectorFirst, vectorSecond);
		}
		return PackedSpanHelpers.FixUpPackedVector512Result(Avx512BW.PackUnsignedSaturate(vectorFirst.AsInt16(), vectorSecond.AsInt16()));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static nuint NarrowUtf16ToAscii_Intrinsified(char* pUtf16Buffer, byte* pAsciiBuffer, nuint elementCount)
	{
		uint count = (uint)Vector128<byte>.Count;
		nuint num = count - 1;
		ref ushort source = ref *(ushort*)pUtf16Buffer;
		Vector128<ushort> vector = Vector128.LoadUnsafe(in source);
		if (VectorContainsNonAsciiChar(vector))
		{
			return 0u;
		}
		ref byte destination = ref *pAsciiBuffer;
		ExtractAsciiVector(vector, vector).StoreLowerUnsafe(ref destination, 0u);
		nuint num2 = count / 2;
		if (((uint)(int)pAsciiBuffer & (count / 2)) == 0)
		{
			vector = Vector128.LoadUnsafe(in source, num2);
			if (VectorContainsNonAsciiChar(vector))
			{
				goto IL_00b4;
			}
			ExtractAsciiVector(vector, vector).StoreLowerUnsafe(ref destination, num2);
		}
		num2 = count - ((nuint)pAsciiBuffer & num);
		nuint num3 = elementCount - count;
		do
		{
			vector = Vector128.LoadUnsafe(in source, num2);
			Vector128<ushort> vector2 = Vector128.LoadUnsafe(in source, num2 + count / 2);
			if (!VectorContainsNonAsciiChar(vector | vector2))
			{
				ExtractAsciiVector(vector, vector2).StoreUnsafe(ref destination, num2);
				num2 += count;
				continue;
			}
			if (!VectorContainsNonAsciiChar(vector))
			{
				ExtractAsciiVector(vector, vector).StoreLowerUnsafe(ref destination, num2);
				num2 += count / 2;
			}
			break;
		}
		while (num2 <= num3);
		goto IL_00b4;
		IL_00b4:
		return num2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static nuint NarrowUtf16ToAscii_Intrinsified_256(char* pUtf16Buffer, byte* pAsciiBuffer, nuint elementCount)
	{
		ref ushort source = ref *(ushort*)pUtf16Buffer;
		Vector256<ushort> vector = Vector256.LoadUnsafe(in source);
		if (VectorContainsNonAsciiChar(vector))
		{
			return 0u;
		}
		ref byte destination = ref *pAsciiBuffer;
		ExtractAsciiVector(vector, vector).GetLower().StoreUnsafe(ref destination, 0u);
		nuint num = 16u;
		if (((int)pAsciiBuffer & 0x10) == 0)
		{
			vector = Vector256.LoadUnsafe(in source, num);
			if (VectorContainsNonAsciiChar(vector))
			{
				goto IL_00a7;
			}
			ExtractAsciiVector(vector, vector).GetLower().StoreUnsafe(ref destination, num);
		}
		num = 32 - ((nuint)pAsciiBuffer & (nuint)0x1Fu);
		nuint num2 = elementCount - 32;
		do
		{
			vector = Vector256.LoadUnsafe(in source, num);
			Vector256<ushort> vector2 = Vector256.LoadUnsafe(in source, num + 16);
			if (!VectorContainsNonAsciiChar(vector | vector2))
			{
				ExtractAsciiVector(vector, vector2).StoreUnsafe(ref destination, num);
				num += 32;
				continue;
			}
			if (!VectorContainsNonAsciiChar(vector))
			{
				ExtractAsciiVector(vector, vector).GetLower().StoreUnsafe(ref destination, num);
				num += 16;
			}
			break;
		}
		while (num <= num2);
		goto IL_00a7;
		IL_00a7:
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static nuint NarrowUtf16ToAscii_Intrinsified_512(char* pUtf16Buffer, byte* pAsciiBuffer, nuint elementCount)
	{
		ref ushort source = ref *(ushort*)pUtf16Buffer;
		Vector512<ushort> vector = Vector512.LoadUnsafe(in source);
		if (VectorContainsNonAsciiChar(vector))
		{
			return 0u;
		}
		ref byte destination = ref *pAsciiBuffer;
		ExtractAsciiVector(vector, vector).GetLower().StoreUnsafe(ref destination, 0u);
		nuint num = 32u;
		if (((int)pAsciiBuffer & 0x20) == 0)
		{
			vector = Vector512.LoadUnsafe(in source, num);
			if (VectorContainsNonAsciiChar(vector))
			{
				goto IL_00a7;
			}
			ExtractAsciiVector(vector, vector).GetLower().StoreUnsafe(ref destination, num);
		}
		num = 64 - ((nuint)pAsciiBuffer & (nuint)0x3Fu);
		nuint num2 = elementCount - 64;
		do
		{
			vector = Vector512.LoadUnsafe(in source, num);
			Vector512<ushort> vector2 = Vector512.LoadUnsafe(in source, num + 32);
			if (!VectorContainsNonAsciiChar(vector | vector2))
			{
				ExtractAsciiVector(vector, vector2).StoreUnsafe(ref destination, num);
				num += 64;
				continue;
			}
			if (!VectorContainsNonAsciiChar(vector))
			{
				ExtractAsciiVector(vector, vector).GetLower().StoreUnsafe(ref destination, num);
				num += 32;
			}
			break;
		}
		while (num <= num2);
		goto IL_00a7;
		IL_00a7:
		return num;
	}

	internal unsafe static nuint WidenAsciiToUtf16(byte* pAsciiBuffer, char* pUtf16Buffer, nuint elementCount)
	{
		nuint currentOffset = 0u;
		_ = BitConverter.IsLittleEndian;
		if (Vector128.IsHardwareAccelerated && elementCount >= (uint)Vector128<byte>.Count)
		{
			if (Vector512.IsHardwareAccelerated && elementCount - currentOffset >= (uint)Vector512<byte>.Count)
			{
				WidenAsciiToUtf1_Vector<Vector512<byte>, Vector512<ushort>>(pAsciiBuffer, pUtf16Buffer, ref currentOffset, elementCount);
			}
			else if (Vector256.IsHardwareAccelerated && elementCount - currentOffset >= (uint)Vector256<byte>.Count)
			{
				WidenAsciiToUtf1_Vector<Vector256<byte>, Vector256<ushort>>(pAsciiBuffer, pUtf16Buffer, ref currentOffset, elementCount);
			}
			else if (Vector128.IsHardwareAccelerated && elementCount - currentOffset >= (uint)Vector128<byte>.Count)
			{
				WidenAsciiToUtf1_Vector<Vector128<byte>, Vector128<ushort>>(pAsciiBuffer, pUtf16Buffer, ref currentOffset, elementCount);
			}
		}
		nuint num = elementCount - currentOffset;
		if (num < 4)
		{
			goto IL_00ae;
		}
		nuint num2 = currentOffset + num - 4;
		uint num3;
		while (true)
		{
			num3 = Unsafe.ReadUnaligned<uint>(pAsciiBuffer + currentOffset);
			if (!AllBytesInUInt32AreAscii(num3))
			{
				break;
			}
			WidenFourAsciiBytesToUtf16AndWriteToBuffer(ref pUtf16Buffer[currentOffset], num3);
			currentOffset += 4;
			if (currentOffset <= num2)
			{
				continue;
			}
			goto IL_00ae;
		}
		goto IL_011e;
		IL_011e:
		_ = BitConverter.IsLittleEndian;
		while (((byte)num3 & 0x80) == 0)
		{
			pUtf16Buffer[currentOffset] = (char)(byte)num3;
			currentOffset++;
			num3 >>= 8;
		}
		goto IL_011c;
		IL_00ae:
		if (((int)num & 2) != 0)
		{
			num3 = Unsafe.ReadUnaligned<ushort>(pAsciiBuffer + currentOffset);
			if (!AllBytesInUInt32AreAscii(num3) && BitConverter.IsLittleEndian)
			{
				goto IL_011e;
			}
			_ = BitConverter.IsLittleEndian;
			pUtf16Buffer[currentOffset] = (char)(byte)num3;
			pUtf16Buffer[currentOffset + 1] = (char)(num3 >> 8);
			currentOffset += 2;
		}
		if (((int)num & 1) != 0)
		{
			num3 = pAsciiBuffer[currentOffset];
			if (((byte)num3 & 0x80) == 0)
			{
				pUtf16Buffer[currentOffset] = (char)num3;
				currentOffset++;
			}
		}
		goto IL_011c;
		IL_011c:
		return currentOffset;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void WidenAsciiToUtf1_Vector<TVectorByte, TVectorUInt16>(byte* pAsciiBuffer, char* pUtf16Buffer, ref nuint currentOffset, nuint elementCount) where TVectorByte : unmanaged, ISimdVector<TVectorByte, byte> where TVectorUInt16 : unmanaged, ISimdVector<TVectorUInt16, ushort>
	{
		ushort* ptr = (ushort*)pUtf16Buffer;
		nuint num = elementCount - (nuint)TVectorByte.ElementCount;
		TVectorByte vector = TVectorByte.Load(pAsciiBuffer + currentOffset);
		if (HasMatch(vector))
		{
			return;
		}
		TVectorUInt16 source2;
		TVectorUInt16 source;
		(source, source2) = Widen<TVectorByte, TVectorUInt16>(vector);
		source.Store(ptr);
		source2.Store(ptr + TVectorUInt16.ElementCount);
		ptr += (nuint)(TVectorUInt16.ElementCount * 2);
		if ((nuint)ptr % (nuint)2u == 0)
		{
			ptr = (ushort*)((nuint)ptr & (nuint)(~(nint)(TVectorUInt16.Alignment - 1)));
			nuint num2 = (nuint)((byte*)ptr - (nuint)pUtf16Buffer);
			currentOffset += num2 / 2;
		}
		else
		{
			currentOffset += (nuint)TVectorByte.ElementCount;
		}
		while (currentOffset <= num)
		{
			vector = TVectorByte.Load(pAsciiBuffer + currentOffset);
			if (HasMatch(vector))
			{
				break;
			}
			TVectorUInt16 source3;
			(source, source3) = Widen<TVectorByte, TVectorUInt16>(vector);
			source.Store(ptr);
			source3.Store(ptr + TVectorUInt16.ElementCount);
			currentOffset += (nuint)TVectorByte.ElementCount;
			ptr += (nuint)(TVectorUInt16.ElementCount * 2);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool HasMatch<TVectorByte>(TVectorByte vector) where TVectorByte : unmanaged, ISimdVector<TVectorByte, byte>
	{
		return !(vector & TVectorByte.Create(128)).Equals(TVectorByte.Zero);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static (TVectorUInt16 Lower, TVectorUInt16 Upper) Widen<TVectorByte, TVectorUInt16>(TVectorByte vector) where TVectorByte : unmanaged, ISimdVector<TVectorByte, byte> where TVectorUInt16 : unmanaged, ISimdVector<TVectorUInt16, ushort>
	{
		if (typeof(TVectorByte) == typeof(Vector256<byte>))
		{
			var (vector2, vector3) = Vector256.Widen((Vector256<byte>)(object)vector);
			return (Lower: (TVectorUInt16)(object)vector2, Upper: (TVectorUInt16)(object)vector3);
		}
		if (typeof(TVectorByte) == typeof(Vector512<byte>))
		{
			var (vector4, vector5) = Vector512.Widen((Vector512<byte>)(object)vector);
			return (Lower: (TVectorUInt16)(object)vector4, Upper: (TVectorUInt16)(object)vector5);
		}
		var (vector6, vector7) = Vector128.Widen((Vector128<byte>)(object)vector);
		return (Lower: (TVectorUInt16)(object)vector6, Upper: (TVectorUInt16)(object)vector7);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void WidenFourAsciiBytesToUtf16AndWriteToBuffer(ref char outputBuffer, uint value)
	{
		if (false)
		{
		}
		if (Vector128.IsHardwareAccelerated)
		{
			Vector128<ulong> vector = Vector128.WidenLower(Vector128.CreateScalar(value).AsByte()).AsUInt64();
			Unsafe.WriteUnaligned(ref Unsafe.As<char, byte>(ref outputBuffer), vector.ToScalar());
			return;
		}
		_ = BitConverter.IsLittleEndian;
		outputBuffer = (char)(byte)value;
		value >>= 8;
		Unsafe.Add(ref outputBuffer, 1) = (char)(byte)value;
		value >>= 8;
		Unsafe.Add(ref outputBuffer, 2) = (char)(byte)value;
		value >>= 8;
		Unsafe.Add(ref outputBuffer, 3) = (char)value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool AllBytesInUInt32AreAscii(uint value)
	{
		return (value & 0x80808080u) == 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static uint CountNumberOfLeadingAsciiBytesFromUInt32WithSomeNonAsciiData(uint value)
	{
		_ = BitConverter.IsLittleEndian;
		return (uint)BitOperations.TrailingZeroCount(value & 0x80808080u) >> 3;
	}
}
