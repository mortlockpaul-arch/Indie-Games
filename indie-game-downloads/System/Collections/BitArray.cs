using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Serialization;

namespace System.Collections;

[Serializable]
[TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public sealed class BitArray : ICollection, IEnumerable, ICloneable, ISerializable
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct AndBinaryOp : IBinaryOp
	{
		public static int Invoke(int value1, int value2)
		{
			return value1 & value2;
		}

		public static TVector Invoke<TVector>(TVector value1, TVector value2) where TVector : ISimdVector<TVector, byte>
		{
			return value1 & value2;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct OrBinaryOp : IBinaryOp
	{
		public static int Invoke(int value1, int value2)
		{
			return value1 | value2;
		}

		public static TVector Invoke<TVector>(TVector value1, TVector value2) where TVector : ISimdVector<TVector, byte>
		{
			return value1 | value2;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct XorBinaryOp : IBinaryOp
	{
		public static int Invoke(int value1, int value2)
		{
			return value1 ^ value2;
		}

		public static TVector Invoke<TVector>(TVector value1, TVector value2) where TVector : ISimdVector<TVector, byte>
		{
			return value1 ^ value2;
		}
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct NotBinaryOp : IBinaryOp
	{
		public static int Invoke(int value1, int _)
		{
			return ~value1;
		}

		public static TVector Invoke<TVector>(TVector value1, TVector value2) where TVector : ISimdVector<TVector, byte>
		{
			return ~value1;
		}
	}

	private interface IBinaryOp
	{
		static abstract int Invoke(int value1, int value2);

		static abstract TVector Invoke<TVector>(TVector value1, TVector value2) where TVector : ISimdVector<TVector, byte>;
	}

	private sealed class BitArrayEnumeratorSimple : IEnumerator, ICloneable
	{
		private static readonly object s_boxedTrue = true;

		private static readonly object s_boxedFalse = false;

		private readonly BitArray _bitArray;

		private readonly int _version;

		private int _index;

		private object _currentElement = s_boxedFalse;

		public object Current
		{
			get
			{
				if ((uint)_index >= (uint)_bitArray._bitLength)
				{
					throw GetInvalidOperationException(_index);
				}
				return _currentElement;
			}
		}

		internal BitArrayEnumeratorSimple(BitArray bitArray)
		{
			_bitArray = bitArray;
			_index = -1;
			_version = bitArray._version;
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public bool MoveNext()
		{
			if (_version != _bitArray._version)
			{
				throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
			}
			if (_index < _bitArray._bitLength - 1)
			{
				_index++;
				_currentElement = (_bitArray.Get(_index) ? s_boxedTrue : s_boxedFalse);
				return true;
			}
			_index = _bitArray._bitLength;
			return false;
		}

		public void Reset()
		{
			if (_version != _bitArray._version)
			{
				throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
			}
			_index = -1;
		}

		private InvalidOperationException GetInvalidOperationException(int index)
		{
			if (index == -1)
			{
				return new InvalidOperationException(SR.InvalidOperation_EnumNotStarted);
			}
			return new InvalidOperationException(SR.InvalidOperation_EnumEnded);
		}
	}

	internal byte[] _array;

	private int _bitLength;

	private int _version;

	public bool this[int index]
	{
		get
		{
			return Get(index);
		}
		set
		{
			Set(index, value);
		}
	}

	public int Length
	{
		get
		{
			return _bitLength;
		}
		set
		{
			ArgumentOutOfRangeException.ThrowIfNegative(value, "value");
			int alignedByteArrayLength = GetAlignedByteArrayLength(value);
			if (alignedByteArrayLength > _array.Length)
			{
				Array.Resize(ref _array, alignedByteArrayLength);
			}
			else
			{
				int byteArrayLengthFromBitLength = GetByteArrayLengthFromBitLength(_bitLength);
				if (alignedByteArrayLength > byteArrayLengthFromBitLength)
				{
					_array.AsSpan(byteArrayLengthFromBitLength).Clear();
				}
				else if (alignedByteArrayLength < _array.Length - 1024)
				{
					Array.Resize(ref _array, alignedByteArrayLength);
				}
			}
			_bitLength = value;
			ClearHighExtraBits();
			_version++;
		}
	}

	public int Count => _bitLength;

	public object SyncRoot => this;

	public bool IsSynchronized => false;

	public bool IsReadOnly => false;

	public BitArray(int length)
		: this(length, defaultValue: false)
	{
	}

	public BitArray(int length, bool defaultValue)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(length, "length");
		_array = AllocateByteArray(length);
		_bitLength = length;
		if (defaultValue)
		{
			Array.Fill(_array, byte.MaxValue);
			ClearHighExtraBits();
		}
	}

	private BitArray(SerializationInfo info, StreamingContext context)
	{
		ArgumentNullException.ThrowIfNull(info, "info");
		int[] array = (int[])info.GetValue("m_array", typeof(int[]));
		_bitLength = info.GetInt32("m_length");
		_version = info.GetInt32("_version");
		if (array == null || (uint)_bitLength > checked((uint)array.Length * 32))
		{
			throw new SerializationException(SR.Serialization_InvalidData);
		}
		_array = AllocateByteArray(_bitLength);
		_ = BitConverter.IsLittleEndian;
		MemoryMarshal.AsBytes(array).CopyTo(_array);
	}

	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
	{
		ArgumentNullException.ThrowIfNull(info, "info");
		int[] array = new int[GetInt32ArrayLengthFromBitLength(_bitLength)];
		CopyTo(array, 0);
		info.AddValue("m_array", array);
		info.AddValue("m_length", _bitLength);
		info.AddValue("_version", _version);
	}

	private void ClearHighExtraBits()
	{
		var (index, num) = Math.DivRem((uint)_bitLength, 32u);
		if (num != 0)
		{
			MemoryMarshal.Cast<byte, int>((Span<byte>)_array)[(int)index] &= ReverseIfBE((1 << (int)num) - 1);
		}
	}

	public BitArray(byte[] bytes)
	{
		ArgumentNullException.ThrowIfNull(bytes, "bytes");
		if (bytes.Length > 268435455)
		{
			throw new ArgumentException(SR.Format(SR.Argument_ArrayTooLarge, 8), "bytes");
		}
		_bitLength = bytes.Length * 8;
		_array = AllocateByteArray(_bitLength);
		Array.Copy(bytes, _array, bytes.Length);
	}

	public BitArray(bool[] values)
	{
		ArgumentNullException.ThrowIfNull(values, "values");
		_array = AllocateByteArray(values.Length);
		_bitLength = values.Length;
		uint num = 0u;
		_ = BitConverter.IsLittleEndian;
		if (values.Length >= Vector256<byte>.Count)
		{
			ref byte arrayDataReference = ref MemoryMarshal.GetArrayDataReference(_array);
			ref byte source = ref Unsafe.As<bool, byte>(ref MemoryMarshal.GetArrayDataReference(values));
			if (Vector512.IsHardwareAccelerated)
			{
				for (; num <= (uint)values.Length - Vector512<byte>.Count; num += (uint)Vector512<byte>.Count)
				{
					ulong num2 = Vector512.Equals(Vector512.LoadUnsafe(in source, num), Vector512<byte>.Zero).ExtractMostSignificantBits();
					Unsafe.WriteUnaligned(ref Unsafe.Add(ref arrayDataReference, 8 * (num / 64)), ~num2);
				}
			}
			else if (Vector256.IsHardwareAccelerated)
			{
				for (; num <= (uint)values.Length - Vector256<byte>.Count; num += (uint)Vector256<byte>.Count)
				{
					uint num3 = Vector256.Equals(Vector256.LoadUnsafe(in source, num), Vector256<byte>.Zero).ExtractMostSignificantBits();
					Unsafe.WriteUnaligned(ref Unsafe.Add(ref arrayDataReference, 4 * (num / 32)), ~num3);
				}
			}
			else if (Vector128.IsHardwareAccelerated)
			{
				for (; num <= (uint)values.Length - (long)Vector128<byte>.Count * 2L; num += (uint)(Vector128<byte>.Count * 2))
				{
					uint num4 = Vector128.Equals(Vector128.LoadUnsafe(in source, num), Vector128<byte>.Zero).ExtractMostSignificantBits();
					uint num5 = Vector128.Equals(Vector128.LoadUnsafe(in source, num + (uint)Vector128<byte>.Count), Vector128<byte>.Zero).ExtractMostSignificantBits();
					Unsafe.WriteUnaligned(ref Unsafe.Add(ref arrayDataReference, 4 * (num / 32)), ~((num5 << 16) | num4));
				}
			}
		}
		for (; num < (uint)values.Length; num++)
		{
			if (values[num])
			{
				var (num6, num7) = Math.DivRem(num, 8u);
				_array[num6] |= (byte)(1 << (int)num7);
			}
		}
	}

	public BitArray(int[] values)
	{
		ArgumentNullException.ThrowIfNull(values, "values");
		if (values.Length > 67108863)
		{
			throw new ArgumentException(SR.Format(SR.Argument_ArrayTooLarge, 32), "values");
		}
		_bitLength = values.Length * 32;
		_array = AllocateByteArray(_bitLength);
		_ = BitConverter.IsLittleEndian;
		MemoryMarshal.AsBytes(values).CopyTo(_array);
	}

	public BitArray(BitArray bits)
	{
		ArgumentNullException.ThrowIfNull(bits, "bits");
		_bitLength = bits._bitLength;
		_array = AllocateByteArray(_bitLength);
		Array.Copy(bits._array, _array, _array.Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Get(int index)
	{
		if ((uint)index >= (uint)_bitLength)
		{
			ThrowArgumentOutOfRangeException(index);
		}
		var (num, num2) = Math.DivRem((uint)index, 8u);
		return (_array[num] & (1 << (int)num2)) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Set(int index, bool value)
	{
		if ((uint)index >= (uint)_bitLength)
		{
			ThrowArgumentOutOfRangeException(index);
		}
		(uint Quotient, uint Remainder) tuple = Math.DivRem((uint)index, 8u);
		uint item = tuple.Quotient;
		uint item2 = tuple.Remainder;
		ref byte reference = ref _array[item];
		byte b = (byte)(1 << (int)item2);
		if (value)
		{
			reference |= b;
		}
		else
		{
			reference &= (byte)(~b);
		}
		_version++;
	}

	public void SetAll(bool value)
	{
		if (value)
		{
			_array.AsSpan(0, GetByteArrayLengthFromBitLength(_bitLength)).Fill(byte.MaxValue);
			ClearHighExtraBits();
		}
		else
		{
			_array.AsSpan(0, GetByteArrayLengthFromBitLength(_bitLength)).Clear();
		}
		_version++;
	}

	public BitArray And(BitArray value)
	{
		return Invoke<AndBinaryOp>(value);
	}

	public BitArray Or(BitArray value)
	{
		return Invoke<OrBinaryOp>(value);
	}

	public BitArray Xor(BitArray value)
	{
		return Invoke<XorBinaryOp>(value);
	}

	public BitArray Not()
	{
		Invoke<NotBinaryOp>(this);
		ClearHighExtraBits();
		return this;
	}

	private BitArray Invoke<TBinaryOp>(BitArray value) where TBinaryOp : struct, IBinaryOp
	{
		ArgumentNullException.ThrowIfNull(value, "value");
		byte[] array = _array;
		byte[] array2 = value._array;
		int byteArrayLengthFromBitLength = GetByteArrayLengthFromBitLength(Length);
		if (Length != value.Length || (uint)byteArrayLengthFromBitLength > (uint)array.Length || (uint)byteArrayLengthFromBitLength > (uint)array2.Length)
		{
			throw new ArgumentException(SR.Arg_ArrayLengthsDiffer);
		}
		int num = 0;
		if (Vector512.IsHardwareAccelerated)
		{
			num = Apply<Vector512<byte>>(byteArrayLengthFromBitLength, array, array2);
		}
		else if (Vector256.IsHardwareAccelerated)
		{
			num = Apply<Vector256<byte>>(byteArrayLengthFromBitLength, array, array2);
		}
		else if (Vector128.IsHardwareAccelerated)
		{
			num = Apply<Vector128<byte>>(byteArrayLengthFromBitLength, array, array2);
		}
		if (num != byteArrayLengthFromBitLength)
		{
			int length = RoundUpToMultipleSizeOfInt32(byteArrayLengthFromBitLength - num);
			Span<int> span = MemoryMarshal.Cast<byte, int>(((Span<byte>)array).Slice(num, length));
			Span<int> span2 = MemoryMarshal.Cast<byte, int>(((Span<byte>)array2).Slice(num, length));
			for (num = 0; num < span.Length; num++)
			{
				span[num] = TBinaryOp.Invoke(span[num], span2[num]);
			}
		}
		_version++;
		return this;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int Apply<TVector>(int count, byte[] thisArray, byte[] valueArray) where TVector : ISimdVector<TVector, byte>
		{
			ref byte arrayDataReference = ref MemoryMarshal.GetArrayDataReference(thisArray);
			ref byte arrayDataReference2 = ref MemoryMarshal.GetArrayDataReference(valueArray);
			int i;
			for (i = 0; i <= count - ((ISimdVector<TVector, byte>)TVector).ElementCount; i += ((ISimdVector<TVector, byte>)TVector).ElementCount)
			{
				((IBinaryOp)TBinaryOp/*cast due to constrained. prefix*/).Invoke<TVector>(((ISimdVector<TVector, byte>)TVector/*cast due to constrained. prefix*/).LoadUnsafe(in arrayDataReference, (nuint)(uint)i), ((ISimdVector<TVector, byte>)TVector/*cast due to constrained. prefix*/).LoadUnsafe(in arrayDataReference2, (nuint)(uint)i)).StoreUnsafe<TVector, byte>(ref arrayDataReference, (nuint)(uint)i);
			}
			return i;
		}
	}

	public BitArray RightShift(int count)
	{
		if (count <= 0)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(count, "count");
			_version++;
			return this;
		}
		Span<int> destination = MemoryMarshal.Cast<byte, int>((Span<byte>)_array);
		int num = 0;
		int int32ArrayLengthFromBitLength = GetInt32ArrayLengthFromBitLength(_bitLength);
		if (count < _bitLength)
		{
			(int Quotient, int Remainder) tuple = Math.DivRem(count, 32);
			int num2 = tuple.Quotient;
			int item = tuple.Remainder;
			int num3 = (int)((uint)_bitLength % 32u);
			if (item == 0)
			{
				uint value = uint.MaxValue >> 32 - num3;
				destination[int32ArrayLengthFromBitLength - 1] &= ReverseIfBE((int)value);
				destination.Slice(num2, int32ArrayLengthFromBitLength - num2).CopyTo(destination);
				num = int32ArrayLengthFromBitLength - num2;
			}
			else
			{
				int num4 = int32ArrayLengthFromBitLength - 1;
				while (num2 < num4)
				{
					uint num5 = (uint)ReverseIfBE(destination[num2]) >> item;
					int num6 = ReverseIfBE(destination[++num2]) << 32 - item;
					destination[num++] = ReverseIfBE(num6 | (int)num5);
				}
				uint num7 = uint.MaxValue >> 32 - num3;
				num7 &= (uint)ReverseIfBE(destination[num2]);
				destination[num++] = ReverseIfBE((int)(num7 >> item));
			}
		}
		destination.Slice(num, int32ArrayLengthFromBitLength - num).Clear();
		_version++;
		return this;
	}

	public BitArray LeftShift(int count)
	{
		if (count <= 0)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(count, "count");
			_version++;
			return this;
		}
		Span<int> span = MemoryMarshal.Cast<byte, int>((Span<byte>)_array);
		int num2;
		if (count < _bitLength)
		{
			int num = (int)((uint)(_bitLength - 1) / 32u);
			int num3;
			(num2, num3) = Math.DivRem(count, 32);
			if (num3 == 0)
			{
				span.Slice(0, num + 1 - num2).CopyTo(span.Slice(num2));
			}
			else
			{
				int num4 = num - num2;
				while (num4 > 0)
				{
					int num5 = ReverseIfBE(span[num4]) << num3;
					uint num6 = (uint)ReverseIfBE(span[--num4]) >> 32 - num3;
					span[num] = ReverseIfBE(num5 | (int)num6);
					num--;
				}
				span[num] = ReverseIfBE(ReverseIfBE(span[num4]) << num3);
			}
		}
		else
		{
			num2 = GetInt32ArrayLengthFromBitLength(_bitLength);
		}
		span.Slice(0, num2).Clear();
		_version++;
		return this;
	}

	public unsafe void CopyTo(Array array, int index)
	{
		ArgumentNullException.ThrowIfNull(array, "array");
		ArgumentOutOfRangeException.ThrowIfNegative(index, "index");
		if (array.Rank != 1)
		{
			throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, "array");
		}
		if (array is int[] array2)
		{
			int int32ArrayLengthFromBitLength = GetInt32ArrayLengthFromBitLength(_bitLength);
			if (array.Length - index < int32ArrayLengthFromBitLength)
			{
				throw new ArgumentException(SR.Argument_InvalidOffLen);
			}
			if (int32ArrayLengthFromBitLength > 0)
			{
				Span<int> span = MemoryMarshal.Cast<byte, int>((Span<byte>)_array).Slice(0, int32ArrayLengthFromBitLength);
				_ = BitConverter.IsLittleEndian;
				span.CopyTo(array2.AsSpan(index));
				uint num = (uint)_bitLength % 32u;
				if (num != 0)
				{
					array2[index + int32ArrayLengthFromBitLength - 1] = ReverseIfBE(span[span.Length - 1]) & ((1 << (int)num) - 1);
				}
			}
			return;
		}
		if (array is byte[] array3)
		{
			int byteArrayLengthFromBitLength = GetByteArrayLengthFromBitLength(_bitLength);
			if (array.Length - index < byteArrayLengthFromBitLength)
			{
				throw new ArgumentException(SR.Argument_InvalidOffLen);
			}
			if (byteArrayLengthFromBitLength > 0)
			{
				ReadOnlySpan<byte> readOnlySpan = _array.AsSpan(0, byteArrayLengthFromBitLength);
				readOnlySpan.CopyTo(array3.AsSpan(index));
				uint num2 = (uint)_bitLength % 8u;
				if (num2 != 0)
				{
					array3[index + byteArrayLengthFromBitLength - 1] = (byte)(readOnlySpan[readOnlySpan.Length - 1] & ((1 << (int)num2) - 1));
				}
			}
			return;
		}
		uint num3;
		if (array is bool[] array4)
		{
			if (array4.Length - index < _bitLength)
			{
				throw new ArgumentException(SR.Argument_InvalidOffLen);
			}
			num3 = 0u;
			_ = BitConverter.IsLittleEndian;
			if (_bitLength >= 32)
			{
				Span<int> span2 = MemoryMarshal.Cast<byte, int>((Span<byte>)_array);
				Vector128<byte> vector = Vector128.Create(0L, 72340172838076673L).AsByte();
				Vector128<byte> vector2 = Vector128.Create(144680345676153346L, 217020518514230019L).AsByte();
				if (Avx512BW.IsSupported && (uint)_bitLength >= Vector512<byte>.Count)
				{
					Vector256<byte> upper = Vector256.Create(289360691352306692L, 361700864190383365L, 434041037028460038L, 506381209866536711L).AsByte();
					Vector512<byte> mask = Vector512.Create(Vector256.Create(vector, vector2), upper);
					Vector512<byte> right = Vector512.Create(9241421688590303745uL).AsByte();
					Vector512<byte> right2 = Vector512.Create((byte)1);
					fixed (bool* ptr = &array4[index])
					{
						for (; num3 + Vector512<byte>.Count <= (uint)_bitLength; num3 += (uint)Vector512<byte>.Count)
						{
							Vector512<byte> source = Avx512BW.Min(Avx512F.And(Avx512BW.Shuffle(Vector512.Create((ulong)((uint)span2[(int)(num3 / 32)] + ((long)span2[(int)(num3 / 32 + 1)] << 32))).AsByte(), mask), right), right2);
							Avx512F.Store((byte*)(ptr + num3), source);
						}
					}
				}
				else if (Avx2.IsSupported && (uint)_bitLength >= Vector256<byte>.Count)
				{
					Vector256<byte> mask2 = Vector256.Create(vector, vector2);
					Vector256<byte> right3 = Vector256.Create(9241421688590303745uL).AsByte();
					Vector256<byte> right4 = Vector256.Create((byte)1);
					fixed (bool* ptr2 = &array4[index])
					{
						for (; num3 + Vector256<byte>.Count <= (uint)_bitLength; num3 += (uint)Vector256<byte>.Count)
						{
							Vector256<byte> source2 = Avx2.Min(Avx2.And(Avx2.Shuffle(Vector256.Create(span2[(int)(num3 / 32)]).AsByte(), mask2), right3), right4);
							Avx.Store((byte*)(ptr2 + num3), source2);
						}
					}
				}
				else if (Ssse3.IsSupported && (uint)_bitLength >= (long)Vector128<byte>.Count * 2L)
				{
					Vector128<byte> mask3 = vector;
					Vector128<byte> mask4 = vector2;
					Vector128<byte> right5 = Vector128.Create((byte)1);
					Vector128<byte> right6 = Vector128.Create(9241421688590303745uL).AsByte();
					fixed (bool* ptr3 = &array4[index])
					{
						for (; num3 + (long)Vector128<byte>.Count * 2L <= (uint)_bitLength; num3 += (uint)(Vector128<byte>.Count * 2))
						{
							Vector128<int> vector3 = Vector128.CreateScalarUnsafe(span2[(int)(num3 / 32)]);
							Vector128<byte> source3 = Sse2.Min(Sse2.And(Ssse3.Shuffle(vector3.AsByte(), mask3), right6), right5);
							Sse2.Store((byte*)(ptr3 + num3), source3);
							Vector128<byte> source4 = Sse2.Min(Sse2.And(Ssse3.Shuffle(vector3.AsByte(), mask4), right6), right5);
							Sse2.Store((byte*)(ptr3 + num3 + Vector128<byte>.Count), source4);
						}
					}
				}
				else if (false)
				{
					goto IL_0493;
				}
			}
			goto IL_04cb;
		}
		throw new ArgumentException(SR.Arg_BitArrayTypeUnsupported, "array");
		IL_04cb:
		if (num3 >= (uint)_bitLength)
		{
			return;
		}
		goto IL_0493;
		IL_0493:
		var (num4, num5) = Math.DivRem(num3, 8u);
		array4[index + (int)num3] = (_array[num4] & (1 << (int)num5)) != 0;
		num3++;
		goto IL_04cb;
	}

	public bool HasAllSet()
	{
		uint num = (uint)_bitLength % 8u;
		int num2 = GetByteArrayLengthFromBitLength(_bitLength);
		if (num != 0)
		{
			num2--;
		}
		if (((ReadOnlySpan<byte>)_array.AsSpan(0, num2)).ContainsAnyExcept(byte.MaxValue))
		{
			return false;
		}
		if (num == 0)
		{
			return true;
		}
		byte b = (byte)((1 << (int)num) - 1);
		return (_array[num2] & b) == b;
	}

	public bool HasAnySet()
	{
		uint num = (uint)_bitLength % 8u;
		int num2 = GetByteArrayLengthFromBitLength(_bitLength);
		if (num != 0)
		{
			num2--;
		}
		if (((ReadOnlySpan<byte>)_array.AsSpan(0, num2)).ContainsAnyExcept((byte)0))
		{
			return true;
		}
		if (num == 0)
		{
			return false;
		}
		byte b = (byte)((1 << (int)num) - 1);
		return (_array[num2] & b) != 0;
	}

	public object Clone()
	{
		return new BitArray(this);
	}

	public IEnumerator GetEnumerator()
	{
		return new BitArrayEnumeratorSimple(this);
	}

	private static int GetInt32ArrayLengthFromBitLength(int bitLength)
	{
		return bitLength + 31 >>> 5;
	}

	internal static int GetByteArrayLengthFromBitLength(int bitLength)
	{
		return bitLength + 7 >>> 3;
	}

	private static int RoundUpToMultipleSizeOfInt32(int value)
	{
		return (value + 3) & -4;
	}

	private static int GetAlignedByteArrayLength(int bitLength)
	{
		return RoundUpToMultipleSizeOfInt32(GetByteArrayLengthFromBitLength(bitLength));
	}

	private static byte[] AllocateByteArray(int bitLength)
	{
		int alignedByteArrayLength = GetAlignedByteArrayLength(bitLength);
		if (bitLength == 0)
		{
			return Array.Empty<byte>();
		}
		return new byte[alignedByteArrayLength];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int ReverseIfBE(int value)
	{
		if (!BitConverter.IsLittleEndian)
		{
		}
		return value;
	}

	private static void ThrowArgumentOutOfRangeException(int index)
	{
		throw new ArgumentOutOfRangeException("index", index, SR.ArgumentOutOfRange_IndexMustBeLess);
	}
}
