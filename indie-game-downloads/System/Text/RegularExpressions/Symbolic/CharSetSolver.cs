using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Text.RegularExpressions.Symbolic;

internal sealed class CharSetSolver : ISolver<BDD>
{
	private enum BooleanOperation
	{
		Or,
		And,
		Xor,
		Not
	}

	private static readonly BDD[] s_asciiCache = new BDD[128];

	private readonly Dictionary<(int ordinal, BDD one, BDD zero), BDD> _bddCache = new Dictionary<(int, BDD, BDD), BDD>();

	private readonly Dictionary<(int op, BDD a, BDD b), BDD> _operationCache = new Dictionary<(int, BDD, BDD), BDD>();

	public BDD Full => BDD.True;

	public BDD Empty => BDD.False;

	public BDD CreateBDDFromChar(char c)
	{
		BDD[] array = s_asciiCache;
		if ((uint)c < (uint)array.Length)
		{
			return array[(uint)c] ?? Interlocked.CompareExchange(ref array[(uint)c], CreateBdd(c), null) ?? array[(uint)c];
		}
		return CreateBdd(c);
		BDD CreateBdd(ushort num)
		{
			BDD bDD = BDD.True;
			for (int i = 0; i < 16; i++)
			{
				bDD = (((num & (1 << i)) != 0) ? GetOrCreateBDD(i, bDD, BDD.False) : GetOrCreateBDD(i, BDD.False, bDD));
			}
			return bDD;
		}
	}

	public BDD ConvertFromBDD(BDD set, CharSetSolver _)
	{
		return set;
	}

	BDD[] ISolver<BDD>.GetMinterms()
	{
		return null;
	}

	public BDD Or(BDD set1, BDD set2)
	{
		return ApplyBinaryOp(BooleanOperation.Or, set1, set2);
	}

	public BDD Or(ReadOnlySpan<BDD> sets)
	{
		if (sets.Length == 0)
		{
			return Empty;
		}
		BDD bDD = sets[0];
		for (int i = 1; i < sets.Length; i++)
		{
			bDD = Or(bDD, sets[i]);
		}
		return bDD;
	}

	public BDD And(BDD a, BDD b)
	{
		return ApplyBinaryOp(BooleanOperation.And, a, b);
	}

	public BDD And(ReadOnlySpan<BDD> sets)
	{
		if (sets.Length == 0)
		{
			return Empty;
		}
		BDD bDD = sets[0];
		for (int i = 1; i < sets.Length; i++)
		{
			bDD = And(bDD, sets[i]);
		}
		return bDD;
	}

	public BDD Not(BDD set)
	{
		if (set == Empty)
		{
			return Full;
		}
		if (set == Full)
		{
			return Empty;
		}
		(int, BDD, BDD) key = (3, set, null);
		if (!_operationCache.TryGetValue(key, out var value))
		{
			value = (_operationCache[key] = GetOrCreateBDD(set.Ordinal, Not(set.One), Not(set.Zero)));
		}
		return value;
	}

	private BDD ApplyBinaryOp(BooleanOperation op, BDD set1, BDD set2)
	{
		switch (op)
		{
		case BooleanOperation.Or:
			if (set1 == Empty)
			{
				return set2;
			}
			if (set2 == Empty)
			{
				return set1;
			}
			if (set1 == Full || set2 == Full)
			{
				return Full;
			}
			if (set1 == set2)
			{
				return set1;
			}
			break;
		case BooleanOperation.And:
			if (set1 == Full)
			{
				return set2;
			}
			if (set2 == Full)
			{
				return set1;
			}
			if (set1 == Empty || set2 == Empty)
			{
				return Empty;
			}
			if (set1 == set2)
			{
				return set1;
			}
			break;
		case BooleanOperation.Xor:
			if (set1 == Empty)
			{
				return set2;
			}
			if (set2 == Empty)
			{
				return set1;
			}
			if (set1 == set2)
			{
				return Empty;
			}
			if (set1 == Full)
			{
				return Not(set2);
			}
			if (set2 == Full)
			{
				return Not(set1);
			}
			break;
		}
		if (set1.GetHashCode() > set2.GetHashCode())
		{
			BDD bDD = set1;
			set1 = set2;
			set2 = bDD;
		}
		if (!_operationCache.TryGetValue(((int)op, set1, set2), out var value))
		{
			BDD bDD2;
			BDD bDD3;
			int ordinal;
			if (set1.IsLeaf || set2.Ordinal > set1.Ordinal)
			{
				bDD2 = ApplyBinaryOp(op, set1, set2.One);
				bDD3 = ApplyBinaryOp(op, set1, set2.Zero);
				ordinal = set2.Ordinal;
			}
			else if (set2.IsLeaf || set1.Ordinal > set2.Ordinal)
			{
				bDD2 = ApplyBinaryOp(op, set1.One, set2);
				bDD3 = ApplyBinaryOp(op, set1.Zero, set2);
				ordinal = set1.Ordinal;
			}
			else
			{
				bDD2 = ApplyBinaryOp(op, set1.One, set2.One);
				bDD3 = ApplyBinaryOp(op, set1.Zero, set2.Zero);
				ordinal = set1.Ordinal;
			}
			value = (_operationCache[((int)op, set1, set2)] = ((bDD2 == bDD3) ? bDD2 : GetOrCreateBDD(ordinal, bDD2, bDD3)));
		}
		return value;
	}

	public bool IsFull(BDD set)
	{
		return ApplyBinaryOp(BooleanOperation.Xor, set, Full) == Empty;
	}

	public bool IsEmpty(BDD set)
	{
		return set == Empty;
	}

	public BDD CreateBDDFromRange(char lower, char upper)
	{
		if (upper >= lower)
		{
			if (upper != lower)
			{
				return CreateBDDFromRangeImpl(lower, upper, 15);
			}
			return CreateBDDFromChar(lower);
		}
		return Empty;
		BDD CreateBDDFromRangeImpl(uint num3, uint num2, int maxBit)
		{
			uint num = (uint)(1 << maxBit);
			if (num == 1)
			{
				if (num2 != 0)
				{
					if (num3 != 1)
					{
						return Full;
					}
					return GetOrCreateBDD(maxBit, Full, Empty);
				}
				return GetOrCreateBDD(maxBit, Empty, Full);
			}
			if (num3 == 0 && num2 == (num << 1) - 1)
			{
				return Full;
			}
			uint num4 = num3 & num;
			if ((num2 & num) == 0)
			{
				BDD zero = CreateBDDFromRangeImpl(num3, num2, maxBit - 1);
				return GetOrCreateBDD(maxBit, Empty, zero);
			}
			if (num4 == num)
			{
				BDD one = CreateBDDFromRangeImpl(num3 & ~num, num2 & ~num, maxBit - 1);
				return GetOrCreateBDD(maxBit, one, Empty);
			}
			BDD zero2 = CreateBDDFromRangeImpl(num3, num - 1, maxBit - 1);
			BDD one2 = CreateBDDFromRangeImpl(0u, num2 & ~num, maxBit - 1);
			return GetOrCreateBDD(maxBit, one2, zero2);
		}
	}

	private BDD GetOrCreateBDD(int ordinal, BDD one, BDD zero)
	{
		ref BDD valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(_bddCache, (ordinal, one, zero), out var _);
		return valueRefOrAddDefault ?? (valueRefOrAddDefault = new BDD(ordinal, one, zero));
	}
}
