using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Symbolic;

internal sealed class BDD : IComparable<BDD>, IEquatable<BDD>
{
	public static readonly BDD True = new BDD(-2, null, null);

	public static readonly BDD False = new BDD(-1, null, null);

	public readonly BDD One;

	public readonly BDD Zero;

	public readonly int Ordinal;

	private readonly int _hashcode;

	[MemberNotNullWhen(false, "One")]
	[MemberNotNullWhen(false, "Zero")]
	public bool IsLeaf
	{
		[MemberNotNullWhen(false, "One")]
		[MemberNotNullWhen(false, "Zero")]
		get
		{
			return One == null;
		}
	}

	public bool IsFull => this == True;

	public bool IsEmpty => this == False;

	internal BDD(int ordinal, BDD one, BDD zero)
	{
		One = one;
		Zero = zero;
		Ordinal = ordinal;
		_hashcode = HashCode.Combine(ordinal, one, zero);
	}

	public ulong GetMin()
	{
		BDD bDD = this;
		if (bDD.IsFull)
		{
			return 0uL;
		}
		ulong num = 0uL;
		while (!bDD.IsLeaf)
		{
			if (bDD.Zero.IsEmpty)
			{
				num |= (ulong)(1L << bDD.Ordinal);
				bDD = bDD.One;
			}
			else
			{
				bDD = bDD.Zero;
			}
		}
		return num;
	}

	public override int GetHashCode()
	{
		return _hashcode;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as BDD);
	}

	public bool Equals(BDD bdd)
	{
		if (bdd != null)
		{
			if (this != bdd)
			{
				if (Ordinal == bdd.Ordinal && One == bdd.One)
				{
					return Zero == bdd.Zero;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public static BDD Deserialize(ReadOnlySpan<byte> bytes)
	{
		int num = bytes[0];
		int num2 = (bytes.Length - 1) / num;
		int num3 = (int)Get(num, bytes, 0);
		int num4 = (int)Get(num, bytes, 1);
		long num5 = (1 << num3) - 1;
		long num6 = (1 << num4) - 1;
		BitLayout(num3, num4, out var zero_node_shift, out var one_node_shift, out var ordinal_shift);
		BDD[] array = new BDD[num2];
		array[0] = False;
		array[1] = True;
		for (int i = 2; i < num2; i++)
		{
			long num7 = Get(num, bytes, i);
			int ordinal = (int)((num7 >> ordinal_shift) & num5);
			int num8 = (int)((num7 >> one_node_shift) & num6);
			int num9 = (int)((num7 >> zero_node_shift) & num6);
			array[i] = new BDD(ordinal, array[num8], array[num9]);
		}
		return array[num2 - 1];
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static long Get(int bytesPerLong, ReadOnlySpan<byte> readOnlySpan, int num12)
		{
			ulong num10 = 0uL;
			for (int num11 = bytesPerLong; num11 > 0; num11--)
			{
				num10 = (num10 << 8) | readOnlySpan[bytesPerLong * num12 + num11];
			}
			return (long)num10;
		}
	}

	private static void BitLayout(int ordinal_bits, int node_bits, out int zero_node_shift, out int one_node_shift, out int ordinal_shift)
	{
		zero_node_shift = ordinal_bits + node_bits;
		one_node_shift = ordinal_bits;
		ordinal_shift = 0;
	}

	public int CompareTo(BDD other)
	{
		if (other == null)
		{
			return -1;
		}
		if (IsLeaf)
		{
			if (other.IsLeaf && Ordinal >= other.Ordinal)
			{
				return (Ordinal != other.Ordinal) ? 1 : 0;
			}
			return -1;
		}
		if (other.IsLeaf)
		{
			return 1;
		}
		ulong min = GetMin();
		ulong min2 = other.GetMin();
		if (min >= min2)
		{
			if (min2 >= min)
			{
				return Ordinal.CompareTo(other.Ordinal);
			}
			return 1;
		}
		return -1;
	}
}
