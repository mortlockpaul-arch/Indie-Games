using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace System.Buffers;

internal readonly struct AhoCorasick(AhoCorasickNode[] nodes, IndexOfAnyAsciiSearcher.AsciiState startingAsciiChars)
{
	public interface IFastScan
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct IndexOfAnyAsciiFastScan : IFastScan
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public readonly struct NoFastScan : IFastScan
	{
	}

	private readonly AhoCorasickNode[] _nodes = nodes;

	private readonly IndexOfAnyAsciiSearcher.AsciiState _startingAsciiChars = startingAsciiChars;

	public bool ShouldUseAsciiFastScan
	{
		get
		{
			if (IndexOfAnyAsciiSearcher.IsVectorizationSupported && _startingAsciiChars.Bitmap != default(Vector256<byte>))
			{
				float num = 0f;
				for (int i = 0; i < 128; i++)
				{
					if (_startingAsciiChars.Lookup.Contains256((char)i))
					{
						num += CharacterFrequencyHelper.AsciiFrequency[i];
					}
				}
				return num <= 50f;
			}
			return false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int IndexOfAny<TCaseSensitivity, TFastScanVariant>(ReadOnlySpan<char> span) where TCaseSensitivity : struct, StringSearchValuesHelper.ICaseSensitivity where TFastScanVariant : struct, IFastScan
	{
		if (!(typeof(TCaseSensitivity) == typeof(StringSearchValuesHelper.CaseInsensitiveUnicode)))
		{
			return IndexOfAnyCore<TCaseSensitivity, TFastScanVariant>(span);
		}
		return IndexOfAnyCaseInsensitiveUnicode<TFastScanVariant>(span);
	}

	private int IndexOfAnyCore<TCaseSensitivity, TFastScanVariant>(ReadOnlySpan<char> span) where TCaseSensitivity : struct, StringSearchValuesHelper.ICaseSensitivity where TFastScanVariant : struct, IFastScan
	{
		ref AhoCorasickNode arrayDataReference = ref MemoryMarshal.GetArrayDataReference(_nodes);
		int num = 0;
		int num2 = -1;
		int num3 = 0;
		while (true)
		{
			if (IndexOfAnyAsciiSearcher.IsVectorizationSupported && typeof(TFastScanVariant) == typeof(IndexOfAnyAsciiFastScan))
			{
				int num4 = span.Length - num3;
				if (num4 >= Vector128<ushort>.Count)
				{
					int num5 = IndexOfAnyAsciiSearcher.IndexOfAny<IndexOfAnyAsciiSearcher.DontNegate, IndexOfAnyAsciiSearcher.Default, SearchValues.FalseConst>(ref Unsafe.As<char, short>(ref Unsafe.Add(ref MemoryMarshal.GetReference(span), num3)), num4, ref Unsafe.AsRef(in _startingAsciiChars));
					if (num5 < 0)
					{
						break;
					}
					num3 += num5;
					goto IL_0086;
				}
			}
			goto IL_007c;
			IL_00dd:
			if (num2 >= 0)
			{
				break;
			}
			num3++;
			continue;
			IL_007c:
			if ((uint)num3 >= (uint)span.Length)
			{
				break;
			}
			goto IL_0086;
			IL_0086:
			char c = TCaseSensitivity.TransformInput(Unsafe.Add(ref MemoryMarshal.GetReference(span), num3));
			int index;
			while (true)
			{
				ref AhoCorasickNode reference = ref Unsafe.Add(ref arrayDataReference, (uint)num);
				if (reference.TryGetChild(c, out index))
				{
					break;
				}
				if (num != 0)
				{
					num = reference.SuffixLink;
					if (num < 0)
					{
						goto end_IL_0012;
					}
					continue;
				}
				goto IL_00dd;
			}
			num = index;
			int matchLength = Unsafe.Add(ref arrayDataReference, (uint)num).MatchLength;
			if (matchLength != 0)
			{
				num2 = num3 + 1 - matchLength;
			}
			num3++;
			goto IL_007c;
			continue;
			end_IL_0012:
			break;
		}
		return num2;
	}

	private int IndexOfAnyCaseInsensitiveUnicode<TFastScanVariant>(ReadOnlySpan<char> span) where TFastScanVariant : struct, IFastScan
	{
		ref AhoCorasickNode arrayDataReference = ref MemoryMarshal.GetArrayDataReference(_nodes);
		int num = 0;
		int num2 = -1;
		int num3 = 0;
		char lr = '\0';
		while (true)
		{
			if (!IndexOfAnyAsciiSearcher.IsVectorizationSupported || !(typeof(TFastScanVariant) == typeof(IndexOfAnyAsciiFastScan)))
			{
				goto IL_0083;
			}
			if (lr == '\0')
			{
				int num4 = span.Length - num3;
				if (num4 < Vector128<ushort>.Count)
				{
					goto IL_0083;
				}
				int num5 = IndexOfAnyAsciiSearcher.IndexOfAny<IndexOfAnyAsciiSearcher.DontNegate, IndexOfAnyAsciiSearcher.Default, SearchValues.FalseConst>(ref Unsafe.As<char, short>(ref Unsafe.Add(ref MemoryMarshal.GetReference(span), num3)), num4, ref Unsafe.AsRef(in _startingAsciiChars));
				if (num5 < 0)
				{
					break;
				}
				num3 += num5;
			}
			goto IL_0090;
			IL_0090:
			char hr;
			if (lr != 0)
			{
				hr = lr;
				lr = '\0';
			}
			else
			{
				hr = Unsafe.Add(ref MemoryMarshal.GetReference(span), num3);
				char l;
				if (char.IsHighSurrogate(hr) && (uint)(num3 + 1) < (uint)span.Length && char.IsLowSurrogate(l = Unsafe.Add(ref MemoryMarshal.GetReference(span), num3 + 1)))
				{
					if (GlobalizationMode.UseNls)
					{
						SurrogateToUpperNLS(hr, l, out hr, out lr);
					}
					else
					{
						SurrogateCasing.ToUpper(hr, l, out hr, out lr);
					}
				}
				else
				{
					hr = TextInfo.ToUpperOrdinal(hr);
				}
			}
			int index;
			while (true)
			{
				ref AhoCorasickNode reference = ref Unsafe.Add(ref arrayDataReference, (uint)num);
				if (reference.TryGetChild(hr, out index))
				{
					break;
				}
				if (num != 0)
				{
					num = reference.SuffixLink;
					if (num < 0)
					{
						goto end_IL_0015;
					}
					continue;
				}
				goto IL_0148;
			}
			num = index;
			int matchLength = Unsafe.Add(ref arrayDataReference, (uint)num).MatchLength;
			if (matchLength != 0)
			{
				num2 = num3 + 1 - matchLength;
			}
			num3++;
			goto IL_0083;
			IL_0148:
			if (num2 >= 0)
			{
				break;
			}
			num3++;
			continue;
			IL_0083:
			if ((uint)num3 >= (uint)span.Length)
			{
				break;
			}
			goto IL_0090;
			continue;
			end_IL_0015:
			break;
		}
		return num2;
	}

	private static void SurrogateToUpperNLS(char h, char l, out char hr, out char lr)
	{
		_003C_003Ey__InlineArray2<char> buffer = default(_003C_003Ey__InlineArray2<char>);
		buffer[0] = h;
		buffer[1] = l;
		ReadOnlySpan<char> source = buffer;
		Span<char> destination = stackalloc char[2];
		Ordinal.ToUpperOrdinal(source, destination);
		hr = destination[0];
		lr = destination[1];
	}
}
