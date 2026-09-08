using System;

namespace q;

internal struct _6 : IEquatable<_6>
{
	public readonly b A;

	public readonly b B;

	private readonly int a5h;

	public _6(b groupA, b groupB)
	{
		if (groupA == null)
		{
			throw new ArgumentNullException("groupA", "The first collision group in the pair is null.  If this pair was being created for CollisionRule calculation purposes, simply consider the rule to be CollisionRule.Defer.");
		}
		if (groupB == null)
		{
			throw new ArgumentNullException("groupB", "The second collision group in the pair is null.  If this pair was being created for CollisionRule calculation purposes, simply consider the rule to be CollisionRule.Defer.");
		}
		A = groupA;
		B = groupB;
		ulong num = (ulong)(((long)groupA.GetHashCode() + (long)groupB.GetHashCode()) * 3625334849u);
		a5h = (int)num;
	}

	bool IEquatable<_6>.Equals(_6 other)
	{
		if (other.A != A || other.B != B)
		{
			if (other.B == A)
			{
				return other.A == B;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		_6 obj2 = (_6)obj;
		if (obj2.A != A || obj2.B != B)
		{
			if (obj2.B == A)
			{
				return obj2.A == B;
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return a5h;
	}
}
