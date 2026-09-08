using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace System.Buffers;

internal sealed class AsciiStringSearchValuesTeddyBucketizedN3<TStartCaseSensitivity, TCaseSensitivity> : AsciiStringSearchValuesTeddyBase<SearchValues.TrueConst, TStartCaseSensitivity, TCaseSensitivity> where TStartCaseSensitivity : struct, StringSearchValuesHelper.ICaseSensitivity where TCaseSensitivity : struct, StringSearchValuesHelper.ICaseSensitivity
{
	public AsciiStringSearchValuesTeddyBucketizedN3(string[][] buckets, ReadOnlySpan<string> values, HashSet<string> uniqueValues)
		: base(buckets, values, uniqueValues, 3)
	{
	}

	[CompExactlyDependsOn(typeof(Ssse3))]
	[CompExactlyDependsOn(typeof(AdvSimd.Arm64))]
	internal override int IndexOfAnyMultiString(ReadOnlySpan<char> span)
	{
		return IndexOfAnyN3(span);
	}
}
