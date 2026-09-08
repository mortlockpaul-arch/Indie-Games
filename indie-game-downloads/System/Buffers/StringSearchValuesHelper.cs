using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;

namespace System.Buffers;

internal static class StringSearchValuesHelper
{
	public interface IValueLength
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct ValueLengthLessThan4 : IValueLength
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct ValueLength4To8 : IValueLength
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct ValueLength9To16 : IValueLength
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct ValueLengthLongOrUnknown : IValueLength
	{
	}

	public readonly struct SingleValueState
	{
		public readonly string Value;

		public readonly nint SecondReadByteOffset;

		public readonly Vector256<ushort> Value256;

		public readonly Vector256<ushort> ToUpperMask256;

		public ulong Value64_0 => Value256.AsUInt64()[0];

		public ulong Value64_1 => Value256.AsUInt64()[1];

		public uint Value32_0 => Value256.AsUInt32()[0];

		public uint Value32_1 => Value256.AsUInt32()[1];

		public ulong ToUpperMask64_0 => ToUpperMask256.AsUInt64()[0];

		public ulong ToUpperMask64_1 => ToUpperMask256.AsUInt64()[1];

		public uint ToUpperMask32_0 => ToUpperMask256.AsUInt32()[0];

		public uint ToUpperMask32_1 => ToUpperMask256.AsUInt32()[1];

		public SingleValueState(string value, bool ignoreCase)
		{
			SecondReadByteOffset = 0;
			Value256 = default(Vector256<ushort>);
			ToUpperMask256 = default(Vector256<ushort>);
			Value = value;
			if (value.Length <= 16)
			{
				if (value.Length > 8)
				{
					SecondReadByteOffset = (value.Length - 8) * 2;
					Value256 = Vector256.Create(Vector128.LoadUnsafe(in value.GetRawStringDataAsUInt16()), Vector128.LoadUnsafe(in Unsafe.AddByteOffset(ref value.GetRawStringDataAsUInt16(), SecondReadByteOffset)));
				}
				else if (value.Length >= 4)
				{
					SecondReadByteOffset = (value.Length - 4) * 2;
					Value256 = Vector256.Create(Vector128.Create(Unsafe.ReadUnaligned<ulong>(in value.GetRawStringDataAsUInt8()), Unsafe.ReadUnaligned<ulong>(in Unsafe.Add(ref value.GetRawStringDataAsUInt8(), SecondReadByteOffset)))).AsUInt16();
				}
				else
				{
					SecondReadByteOffset = (value.Length - 2) * 2;
					Value256 = Vector256.Create(Vector128.Create(Vector64.Create(Unsafe.ReadUnaligned<uint>(in value.GetRawStringDataAsUInt8()), Unsafe.ReadUnaligned<uint>(in Unsafe.Add(ref value.GetRawStringDataAsUInt8(), SecondReadByteOffset))))).AsUInt16();
				}
				if (ignoreCase)
				{
					Vector256<ushort> condition = Vector256.GreaterThanOrEqual(Value256, Vector256.Create((ushort)65)) & Vector256.LessThanOrEqual(Value256, Vector256.Create((ushort)90));
					ToUpperMask256 = Vector256.ConditionalSelect(condition, Vector256.Create((ushort)65503), Vector256.Create(ushort.MaxValue));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MatchesLength9To16_CaseSensitive(ref char matchStart)
		{
			if (Vector256.IsHardwareAccelerated)
			{
				return Vector256.Create(Vector128.LoadUnsafe(ref matchStart), Vector128.LoadUnsafe(ref Unsafe.AddByteOffset(ref matchStart, SecondReadByteOffset))) == Value256;
			}
			return ((Vector128.LoadUnsafe(ref matchStart) ^ Value256.GetLower()) | (Vector128.LoadUnsafe(ref Unsafe.AddByteOffset(ref matchStart, SecondReadByteOffset)) ^ Value256.GetUpper())) == Vector128<ushort>.Zero;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MatchesLength9To16_CaseInsensitiveAscii(ref char matchStart)
		{
			if (Vector256.IsHardwareAccelerated)
			{
				return (Vector256.Create(Vector128.LoadUnsafe(ref matchStart), Vector128.LoadUnsafe(ref Unsafe.AddByteOffset(ref matchStart, SecondReadByteOffset))) & ToUpperMask256) == Value256;
			}
			return (((Vector128.LoadUnsafe(ref matchStart) & ToUpperMask256.GetLower()) ^ Value256.GetLower()) | ((Vector128.LoadUnsafe(ref Unsafe.AddByteOffset(ref matchStart, SecondReadByteOffset)) & ToUpperMask256.GetUpper()) ^ Value256.GetUpper())) == Vector128<ushort>.Zero;
		}
	}

	public interface ICaseSensitivity
	{
		static abstract char TransformInput(char input);

		static abstract Vector128<byte> TransformInput(Vector128<byte> input);

		static abstract Vector256<byte> TransformInput(Vector256<byte> input);

		static abstract Vector512<byte> TransformInput(Vector512<byte> input);

		static abstract bool Equals<TValueLength>(ref char matchStart, ref readonly SingleValueState state) where TValueLength : struct, IValueLength;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct CaseSensitive : ICaseSensitivity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char TransformInput(char input)
		{
			return input;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector128<byte> TransformInput(Vector128<byte> input)
		{
			return input;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector256<byte> TransformInput(Vector256<byte> input)
		{
			return input;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector512<byte> TransformInput(Vector512<byte> input)
		{
			return input;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Equals<TValueLength>(ref char matchStart, ref readonly SingleValueState state) where TValueLength : struct, IValueLength
		{
			if (typeof(TValueLength) == typeof(ValueLengthLongOrUnknown))
			{
				return UnknownLengthEquals<CaseSensitive>(ref matchStart, state.Value);
			}
			if (typeof(TValueLength) == typeof(ValueLength9To16))
			{
				return state.MatchesLength9To16_CaseSensitive(ref matchStart);
			}
			if (typeof(TValueLength) == typeof(ValueLength4To8))
			{
				ref byte source = ref Unsafe.As<char, byte>(ref matchStart);
				return ((Unsafe.ReadUnaligned<ulong>(in source) - state.Value64_0) | (Unsafe.ReadUnaligned<ulong>(in Unsafe.Add(ref source, state.SecondReadByteOffset)) - state.Value64_1)) == 0;
			}
			ref byte source2 = ref Unsafe.As<char, byte>(ref matchStart);
			if (false)
			{
			}
			return Unsafe.ReadUnaligned<uint>(in Unsafe.Add(ref source2, state.SecondReadByteOffset)) == state.Value32_1;
		}

		static bool ICaseSensitivity.Equals<TValueLength>(ref char matchStart, ref readonly SingleValueState state)
		{
			return Equals<TValueLength>(ref matchStart, in state);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct CaseInsensitiveAsciiLetters : ICaseSensitivity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char TransformInput(char input)
		{
			return (char)(input & -33);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector128<byte> TransformInput(Vector128<byte> input)
		{
			return input & Vector128.Create((byte)223);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector256<byte> TransformInput(Vector256<byte> input)
		{
			return input & Vector256.Create((byte)223);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector512<byte> TransformInput(Vector512<byte> input)
		{
			return input & Vector512.Create((byte)223);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Equals<TValueLength>(ref char matchStart, ref readonly SingleValueState state) where TValueLength : struct, IValueLength
		{
			if (typeof(TValueLength) == typeof(ValueLengthLongOrUnknown))
			{
				return UnknownLengthEquals<CaseInsensitiveAsciiLetters>(ref matchStart, state.Value);
			}
			if (typeof(TValueLength) == typeof(ValueLength9To16))
			{
				return state.MatchesLength9To16_CaseInsensitiveAscii(ref matchStart);
			}
			if (typeof(TValueLength) == typeof(ValueLength4To8))
			{
				ref byte source = ref Unsafe.As<char, byte>(ref matchStart);
				return (((Unsafe.ReadUnaligned<ulong>(in source) & 0xFFDFFFDFFFDFFFDFuL) - state.Value64_0) | ((Unsafe.ReadUnaligned<ulong>(in Unsafe.Add(ref source, state.SecondReadByteOffset)) & 0xFFDFFFDFFFDFFFDFuL) - state.Value64_1)) == 0;
			}
			ref byte source2 = ref Unsafe.As<char, byte>(ref matchStart);
			if (false)
			{
			}
			return (Unsafe.ReadUnaligned<uint>(in Unsafe.Add(ref source2, state.SecondReadByteOffset)) & 0xFFDFFFDFu) == state.Value32_1;
		}

		static bool ICaseSensitivity.Equals<TValueLength>(ref char matchStart, ref readonly SingleValueState state)
		{
			return Equals<TValueLength>(ref matchStart, in state);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct CaseInsensitiveAscii : ICaseSensitivity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char TransformInput(char input)
		{
			return TextInfo.ToUpperAsciiInvariant(input);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector128<byte> TransformInput(Vector128<byte> input)
		{
			Vector128<byte> vector = Vector128.Create((byte)225);
			Vector128<byte> vector2 = Vector128.Create((byte)154);
			Vector128<byte> vector3 = Vector128.Create((byte)32);
			Vector128<byte> vector4 = Vector128.LessThan((input - vector).AsSByte(), vector2.AsSByte()).AsByte();
			return input ^ (vector4 & vector3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector256<byte> TransformInput(Vector256<byte> input)
		{
			Vector256<byte> vector = Vector256.Create((byte)225);
			Vector256<byte> vector2 = Vector256.Create((byte)154);
			Vector256<byte> vector3 = Vector256.Create((byte)32);
			Vector256<byte> vector4 = Vector256.LessThan((input - vector).AsSByte(), vector2.AsSByte()).AsByte();
			return input ^ (vector4 & vector3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector512<byte> TransformInput(Vector512<byte> input)
		{
			Vector512<byte> vector = Vector512.Create((byte)225);
			Vector512<byte> vector2 = Vector512.Create((byte)154);
			Vector512<byte> vector3 = Vector512.Create((byte)32);
			Vector512<byte> vector4 = Vector512.LessThan((input - vector).AsSByte(), vector2.AsSByte()).AsByte();
			return input ^ (vector4 & vector3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Equals<TValueLength>(ref char matchStart, ref readonly SingleValueState state) where TValueLength : struct, IValueLength
		{
			if (typeof(TValueLength) == typeof(ValueLengthLongOrUnknown))
			{
				return UnknownLengthEquals<CaseInsensitiveAscii>(ref matchStart, state.Value);
			}
			if (typeof(TValueLength) == typeof(ValueLength9To16))
			{
				return state.MatchesLength9To16_CaseInsensitiveAscii(ref matchStart);
			}
			if (typeof(TValueLength) == typeof(ValueLength4To8))
			{
				ref byte source = ref Unsafe.As<char, byte>(ref matchStart);
				return (((Unsafe.ReadUnaligned<ulong>(in source) & state.ToUpperMask64_0) - state.Value64_0) | ((Unsafe.ReadUnaligned<ulong>(in Unsafe.Add(ref source, state.SecondReadByteOffset)) & state.ToUpperMask64_1) - state.Value64_1)) == 0;
			}
			ref byte source2 = ref Unsafe.As<char, byte>(ref matchStart);
			return (((Unsafe.ReadUnaligned<uint>(in source2) & state.ToUpperMask32_0) - state.Value32_0) | ((Unsafe.ReadUnaligned<uint>(in Unsafe.Add(ref source2, state.SecondReadByteOffset)) & state.ToUpperMask32_1) - state.Value32_1)) == 0;
		}

		static bool ICaseSensitivity.Equals<TValueLength>(ref char matchStart, ref readonly SingleValueState state)
		{
			return Equals<TValueLength>(ref matchStart, in state);
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct CaseInsensitiveUnicode : ICaseSensitivity
	{
		public static char TransformInput(char input)
		{
			throw new UnreachableException();
		}

		public static Vector128<byte> TransformInput(Vector128<byte> input)
		{
			throw new UnreachableException();
		}

		public static Vector256<byte> TransformInput(Vector256<byte> input)
		{
			throw new UnreachableException();
		}

		public static Vector512<byte> TransformInput(Vector512<byte> input)
		{
			throw new UnreachableException();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Equals<TValueLength>(ref char matchStart, ref readonly SingleValueState state) where TValueLength : struct, IValueLength
		{
			if (typeof(TValueLength) == typeof(ValueLengthLongOrUnknown))
			{
				return UnknownLengthEquals<CaseInsensitiveUnicode>(ref matchStart, state.Value);
			}
			return Ordinal.EqualsIgnoreCase_Scalar(ref matchStart, ref state.Value.GetRawStringData(), state.Value.Length);
		}

		static bool ICaseSensitivity.Equals<TValueLength>(ref char matchStart, ref readonly SingleValueState state)
		{
			return Equals<TValueLength>(ref matchStart, in state);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool StartsWith<TCaseSensitivity>(ref char matchStart, int lengthRemaining, string[] candidates) where TCaseSensitivity : struct, ICaseSensitivity
	{
		foreach (string candidate in candidates)
		{
			if (StartsWith<TCaseSensitivity>(ref matchStart, lengthRemaining, candidate))
			{
				return true;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool StartsWith<TCaseSensitivity>(ref char matchStart, int lengthRemaining, string candidate) where TCaseSensitivity : struct, ICaseSensitivity
	{
		if (lengthRemaining < candidate.Length)
		{
			return false;
		}
		return UnknownLengthEquals<TCaseSensitivity>(ref matchStart, candidate);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool UnknownLengthEquals<TCaseSensitivity>(ref char matchStart, string candidate) where TCaseSensitivity : struct, ICaseSensitivity
	{
		if (typeof(TCaseSensitivity) == typeof(CaseSensitive))
		{
			return SpanHelpers.SequenceEqual(ref Unsafe.As<char, byte>(ref matchStart), ref candidate.GetRawStringDataAsUInt8(), (uint)(candidate.Length * 2));
		}
		if (typeof(TCaseSensitivity) == typeof(CaseInsensitiveAscii) || typeof(TCaseSensitivity) == typeof(CaseInsensitiveAsciiLetters))
		{
			return Ascii.EqualsIgnoreCase(ref matchStart, ref candidate.GetRawStringData(), (uint)candidate.Length);
		}
		return Ordinal.EqualsIgnoreCase(ref matchStart, ref candidate.GetRawStringData(), candidate.Length);
	}
}
