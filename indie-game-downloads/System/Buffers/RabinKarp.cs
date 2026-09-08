using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Buffers;

internal readonly struct RabinKarp
{
	private readonly string[][] _buckets;

	private readonly int _hashLength;

	private readonly nuint _hashUpdateMultiplier;

	public RabinKarp(ReadOnlySpan<string> values)
	{
		int num = int.MaxValue;
		ReadOnlySpan<string> readOnlySpan = values;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			string text = readOnlySpan[i];
			num = Math.Min(num, text.Length);
		}
		_hashLength = num;
		_hashUpdateMultiplier = (nuint)((nint)1 << (((num - 1) * 2) & 0x3F));
		if (num > 17)
		{
			_buckets = null;
			return;
		}
		string[][] array = (_buckets = new string[64][]);
		ReadOnlySpan<string> readOnlySpan2 = values;
		for (int i = 0; i < readOnlySpan2.Length; i++)
		{
			string text2 = readOnlySpan2[i];
			if (text2.Length <= 17)
			{
				nuint num2 = 0u;
				for (int j = 0; j < num; j++)
				{
					num2 = (num2 << (2 & 0x3F)) + text2[j];
				}
				nuint num3 = num2 % 64;
				string[] array2 = array[num3];
				string[] array3;
				if (array2 != null)
				{
					array3 = new string[array2.Length + 1];
					array2.AsSpan().CopyTo(array3);
				}
				else
				{
					array3 = new string[1];
				}
				array3[^1] = text2;
				array[num3] = array3;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int IndexOfAny<TCaseSensitivity>(ReadOnlySpan<char> span) where TCaseSensitivity : struct, StringSearchValuesHelper.ICaseSensitivity
	{
		if (!(typeof(TCaseSensitivity) == typeof(StringSearchValuesHelper.CaseInsensitiveUnicode)))
		{
			return IndexOfAnyCore<TCaseSensitivity>(span);
		}
		return IndexOfAnyCaseInsensitiveUnicode(span);
	}

	private int IndexOfAnyCore<TCaseSensitivity>(ReadOnlySpan<char> span) where TCaseSensitivity : struct, StringSearchValuesHelper.ICaseSensitivity
	{
		ref char reference = ref MemoryMarshal.GetReference(span);
		int hashLength = _hashLength;
		if (span.Length >= hashLength)
		{
			ref char right = ref Unsafe.Add(ref MemoryMarshal.GetReference(span), (uint)(span.Length - hashLength));
			nuint num = 0u;
			for (uint num2 = 0u; num2 < hashLength; num2++)
			{
				num = (num << (2 & 0x3F)) + TCaseSensitivity.TransformInput(Unsafe.Add(ref reference, num2));
			}
			ref string[] arrayDataReference = ref MemoryMarshal.GetArrayDataReference(_buckets);
			while (true)
			{
				string[] array = Unsafe.Add(ref arrayDataReference, num % 64);
				if (array != null)
				{
					int num3 = (int)((nuint)Unsafe.ByteOffset(in MemoryMarshal.GetReference(span), in reference) / (nuint)2u);
					if (StringSearchValuesHelper.StartsWith<TCaseSensitivity>(ref reference, span.Length - num3, array))
					{
						return num3;
					}
				}
				if (Unsafe.IsAddressGreaterThanOrEqualTo(in reference, in right))
				{
					break;
				}
				char c = TCaseSensitivity.TransformInput(reference);
				char c2 = TCaseSensitivity.TransformInput(Unsafe.Add(ref reference, (uint)hashLength));
				num = (num - c * _hashUpdateMultiplier << (2 & 0x3F)) + c2;
				reference = ref Unsafe.Add(ref reference, 1);
			}
		}
		return -1;
	}

	private int IndexOfAnyCaseInsensitiveUnicode(ReadOnlySpan<char> span)
	{
		if (_hashLength > span.Length)
		{
			return -1;
		}
		Span<char> span2 = stackalloc char[17].Slice(0, span.Length);
		Ordinal.ToUpperOrdinal(span, span2);
		return IndexOfAnyCore<StringSearchValuesHelper.CaseSensitive>(span2);
	}
}
