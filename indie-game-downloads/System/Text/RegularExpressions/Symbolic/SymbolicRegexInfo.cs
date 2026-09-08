namespace System.Text.RegularExpressions.Symbolic;

internal readonly struct SymbolicRegexInfo : IEquatable<SymbolicRegexInfo>
{
	private readonly uint _info;

	public bool IsNullable => (_info & 1) != 0;

	public bool CanBeNullable => (_info & 8) != 0;

	public bool StartsWithLineAnchor => (_info & 2) != 0;

	public bool ContainsLineAnchor => (_info & 0x100) != 0;

	public bool StartsWithSomeAnchor => (_info & 0x20) != 0;

	public bool ContainsSomeAnchor => (_info & 0x10) != 0;

	public bool IsLazyLoop => (_info & 4) != 0;

	public bool IsHighPriorityNullable => (_info & 0x40) != 0;

	public bool ContainsEffect => (_info & 0x80) != 0;

	public bool ContainsEndZAnchor => (_info & 0x200) != 0;

	private SymbolicRegexInfo(uint i)
	{
		_info = i;
	}

	private static SymbolicRegexInfo Create(bool isAlwaysNullable = false, bool canBeNullable = false, bool startsWithLineAnchor = false, bool containsLineAnchor = false, bool startsWithSomeAnchor = false, bool containsSomeAnchor = false, bool isHighPriorityNullable = false, bool containsEffect = false, bool containsEndZAnchor = false)
	{
		return new SymbolicRegexInfo((isAlwaysNullable ? 1u : 0u) | (uint)(canBeNullable ? 8 : 0) | (uint)(startsWithLineAnchor ? 2 : 0) | (uint)(containsLineAnchor ? 256 : 0) | (uint)(startsWithSomeAnchor ? 32 : 0) | (uint)(containsSomeAnchor ? 16 : 0) | (uint)(isHighPriorityNullable ? 64 : 0) | (uint)(containsEffect ? 128 : 0) | (uint)(containsEndZAnchor ? 512 : 0));
	}

	public static SymbolicRegexInfo Epsilon()
	{
		return Create(isAlwaysNullable: true, canBeNullable: true, startsWithLineAnchor: false, containsLineAnchor: false, startsWithSomeAnchor: false, containsSomeAnchor: false, isHighPriorityNullable: true);
	}

	public static SymbolicRegexInfo Anchor(bool isLineAnchor, bool isEndZAnchor)
	{
		return Create(isAlwaysNullable: false, canBeNullable: true, isLineAnchor, isLineAnchor, startsWithSomeAnchor: true, containsSomeAnchor: true, isHighPriorityNullable: false, containsEffect: false, isEndZAnchor);
	}

	public static SymbolicRegexInfo Alternate(SymbolicRegexInfo left_info, SymbolicRegexInfo right_info)
	{
		return Create(left_info.IsNullable || right_info.IsNullable, left_info.CanBeNullable || right_info.CanBeNullable, left_info.StartsWithLineAnchor || right_info.StartsWithLineAnchor, left_info.ContainsLineAnchor || right_info.ContainsLineAnchor, left_info.StartsWithSomeAnchor || right_info.StartsWithSomeAnchor, left_info.ContainsSomeAnchor || right_info.ContainsSomeAnchor, left_info.IsHighPriorityNullable, left_info.ContainsEffect || right_info.ContainsEffect, left_info.ContainsEndZAnchor || right_info.ContainsEndZAnchor);
	}

	public static SymbolicRegexInfo Concat(SymbolicRegexInfo left_info, SymbolicRegexInfo right_info)
	{
		return Create(left_info.IsNullable && right_info.IsNullable, left_info.CanBeNullable && right_info.CanBeNullable, left_info.StartsWithLineAnchor || (left_info.CanBeNullable && right_info.StartsWithLineAnchor), left_info.ContainsLineAnchor || right_info.ContainsLineAnchor, left_info.StartsWithSomeAnchor || (left_info.CanBeNullable && right_info.StartsWithSomeAnchor), left_info.ContainsSomeAnchor || right_info.ContainsSomeAnchor, left_info.IsHighPriorityNullable && right_info.IsHighPriorityNullable, left_info.ContainsEffect || right_info.ContainsEffect, left_info.ContainsEndZAnchor || right_info.ContainsEndZAnchor);
	}

	public static SymbolicRegexInfo Loop(SymbolicRegexInfo body_info, int lowerBound, bool isLazy)
	{
		uint num = body_info._info;
		if (lowerBound == 0)
		{
			num |= 9;
			if (isLazy)
			{
				num |= 0x40;
			}
		}
		num = ((!isLazy) ? (num & 0xFFFFFFFBu) : (num | 4));
		return new SymbolicRegexInfo(num);
	}

	public static SymbolicRegexInfo Effect(SymbolicRegexInfo childInfo)
	{
		return new SymbolicRegexInfo(childInfo._info | 0x80);
	}

	public override bool Equals(object obj)
	{
		if (obj is SymbolicRegexInfo other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(SymbolicRegexInfo other)
	{
		return _info == other._info;
	}

	public override int GetHashCode()
	{
		return _info.GetHashCode();
	}
}
