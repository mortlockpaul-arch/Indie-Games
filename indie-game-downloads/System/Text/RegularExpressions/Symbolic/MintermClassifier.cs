using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Symbolic;

internal sealed class MintermClassifier
{
	private readonly byte[] _lookup;

	private readonly int[] _intLookup;

	public byte[] ByteLookup => _lookup;

	public MintermClassifier(BDD[] minterms)
	{
		if (minterms.Length == 1)
		{
			_lookup = Array.Empty<byte>();
			return;
		}
		object[] array = ArrayPool<object>.Shared.Rent(minterms.Length);
		Span<object> span = array.AsSpan(0, minterms.Length);
		int num = -1;
		for (int i = 1; i < minterms.Length; i++)
		{
			(uint, uint)[] array2 = BDDRangeConverter.ToRanges(minterms[i]);
			span[i] = array2;
			num = Math.Max(num, (int)array2[^1].Item2);
		}
		if (minterms.Length > 255)
		{
			_intLookup = CreateLookup<int>(minterms, span, num);
		}
		else
		{
			_lookup = CreateLookup<byte>(minterms, span, num);
		}
		span.Clear();
		ArrayPool<object>.Shared.Return(array);
		static T[] CreateLookup<T>(BDD[] array4, ReadOnlySpan<object> charRangesPerMinterm, int _maxChar) where T : IBinaryInteger<T>
		{
			T[] array3 = new T[_maxChar + 1];
			for (int j = 1; j < array4.Length; j++)
			{
				(uint, uint)[] array5 = ((uint, uint)[])charRangesPerMinterm[j];
				for (int k = 0; k < array5.Length; k++)
				{
					var (num2, num3) = array5[k];
					array3.AsSpan((int)num2, (int)(num3 + 1 - num2)).Fill(T.CreateTruncating(j));
				}
			}
			return array3;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetMintermID(int c)
	{
		if (_lookup != null)
		{
			byte[] lookup = _lookup;
			if ((uint)c >= (uint)lookup.Length)
			{
				return 0;
			}
			return lookup[c];
		}
		int[] intLookup = _intLookup;
		if ((uint)c >= (uint)intLookup.Length)
		{
			return 0;
		}
		return intLookup[c];
	}
}
