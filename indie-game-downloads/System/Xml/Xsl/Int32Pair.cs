using System.Diagnostics.CodeAnalysis;

namespace System.Xml.Xsl;

internal readonly struct Int32Pair(int left, int right) : IEquatable<Int32Pair>
{
	public int Left { get; } = left;

	public int Right { get; } = right;

	public override bool Equals([NotNullWhen(true)] object other)
	{
		if (other is Int32Pair other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(Int32Pair other)
	{
		if (Left == other.Left)
		{
			return Right == other.Right;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Left.GetHashCode() ^ Right.GetHashCode();
	}
}
