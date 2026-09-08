using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers;

internal sealed class ProbabilisticWithAsciiCharSearchValues<TOptimizations> : SearchValues<char> where TOptimizations : struct, IndexOfAnyAsciiSearcher.IOptimizations
{
	private IndexOfAnyAsciiSearcher.AsciiState _asciiState;

	private IndexOfAnyAsciiSearcher.AsciiState _inverseAsciiState;

	private ProbabilisticMapState _map;

	public ProbabilisticWithAsciiCharSearchValues(ReadOnlySpan<char> values, int maxInclusive)
	{
		IndexOfAnyAsciiSearcher.ComputeAsciiState(values, out _asciiState);
		_inverseAsciiState = _asciiState.CreateInverse();
		_map = new ProbabilisticMapState(values, maxInclusive);
	}

	internal override char[] GetValues()
	{
		return _map.GetValues();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal override bool ContainsCore(char value)
	{
		return _map.FastContains(value);
	}

	internal override int IndexOfAny(ReadOnlySpan<char> span)
	{
		int num = 0;
		if (IndexOfAnyAsciiSearcher.IsVectorizationSupported && span.Length >= Vector128<short>.Count && char.IsAscii(span[0]))
		{
			num = (((!Ssse3.IsSupported && 0 == 0) || !(typeof(TOptimizations) == typeof(IndexOfAnyAsciiSearcher.Default))) ? IndexOfAnyAsciiSearcher.IndexOfAny<IndexOfAnyAsciiSearcher.Negate, IndexOfAnyAsciiSearcher.Default, SearchValues.FalseConst>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _inverseAsciiState) : IndexOfAnyAsciiSearcher.IndexOfAny<IndexOfAnyAsciiSearcher.Negate, IndexOfAnyAsciiSearcher.Ssse3AndWasmHandleZeroInNeedle, SearchValues.FalseConst>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _inverseAsciiState));
			if ((uint)num >= (uint)span.Length || char.IsAscii(span[num]))
			{
				return num;
			}
			span = span.Slice(num);
		}
		int num2 = ProbabilisticMap.IndexOfAny<SearchValues.TrueConst>(ref MemoryMarshal.GetReference(span), span.Length, ref _map);
		if (num2 >= 0)
		{
			num2 += num;
		}
		return num2;
	}

	internal override int IndexOfAnyExcept(ReadOnlySpan<char> span)
	{
		int num = 0;
		if (IndexOfAnyAsciiSearcher.IsVectorizationSupported && span.Length >= Vector128<short>.Count && char.IsAscii(span[0]))
		{
			num = IndexOfAnyAsciiSearcher.IndexOfAny<IndexOfAnyAsciiSearcher.Negate, TOptimizations, SearchValues.FalseConst>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _asciiState);
			if ((uint)num >= (uint)span.Length || char.IsAscii(span[num]))
			{
				return num;
			}
			span = span.Slice(num);
		}
		int num2 = ProbabilisticMapState.IndexOfAnySimpleLoop<SearchValues.TrueConst, IndexOfAnyAsciiSearcher.Negate>(ref MemoryMarshal.GetReference(span), span.Length, ref _map);
		if (num2 >= 0)
		{
			num2 += num;
		}
		return num2;
	}

	internal override int LastIndexOfAny(ReadOnlySpan<char> span)
	{
		if (IndexOfAnyAsciiSearcher.IsVectorizationSupported && span.Length >= Vector128<short>.Count)
		{
			if (char.IsAscii(span[span.Length - 1]))
			{
				int num = (((!Ssse3.IsSupported && 0 == 0) || !(typeof(TOptimizations) == typeof(IndexOfAnyAsciiSearcher.Default))) ? IndexOfAnyAsciiSearcher.LastIndexOfAny<IndexOfAnyAsciiSearcher.Negate, IndexOfAnyAsciiSearcher.Default, SearchValues.FalseConst>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _inverseAsciiState) : IndexOfAnyAsciiSearcher.LastIndexOfAny<IndexOfAnyAsciiSearcher.Negate, IndexOfAnyAsciiSearcher.Ssse3AndWasmHandleZeroInNeedle, SearchValues.FalseConst>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _inverseAsciiState));
				if ((uint)num >= (uint)span.Length || char.IsAscii(span[num]))
				{
					return num;
				}
				span = span.Slice(0, num + 1);
			}
		}
		return ProbabilisticMap.LastIndexOfAny<SearchValues.TrueConst>(ref MemoryMarshal.GetReference(span), span.Length, ref _map);
	}

	internal override int LastIndexOfAnyExcept(ReadOnlySpan<char> span)
	{
		if (IndexOfAnyAsciiSearcher.IsVectorizationSupported && span.Length >= Vector128<short>.Count)
		{
			if (char.IsAscii(span[span.Length - 1]))
			{
				int num = IndexOfAnyAsciiSearcher.LastIndexOfAny<IndexOfAnyAsciiSearcher.Negate, TOptimizations, SearchValues.FalseConst>(ref Unsafe.As<char, short>(ref MemoryMarshal.GetReference(span)), span.Length, ref _asciiState);
				if ((uint)num >= (uint)span.Length || char.IsAscii(span[num]))
				{
					return num;
				}
				span = span.Slice(0, num + 1);
			}
		}
		return ProbabilisticMapState.LastIndexOfAnySimpleLoop<SearchValues.TrueConst, IndexOfAnyAsciiSearcher.Negate>(ref MemoryMarshal.GetReference(span), span.Length, ref _map);
	}
}
