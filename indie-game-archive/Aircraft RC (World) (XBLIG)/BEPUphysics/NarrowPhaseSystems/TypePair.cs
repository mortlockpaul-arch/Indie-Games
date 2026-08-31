using System;

namespace BEPUphysics.NarrowPhaseSystems;

/// <summary>
///  Pair of types.
/// </summary>
public struct TypePair : IEquatable<TypePair>
{
	/// <summary>
	///  First type in the pair.
	/// </summary>
	public Type A;

	/// <summary>
	///  Second type in the pair.
	/// </summary>
	public Type B;

	/// <summary>
	///  Constructs a new type pair.
	/// </summary>
	/// <param name="a">First type in the pair.</param>
	/// <param name="b">Second type in the pair.</param>
	public TypePair(Type a, Type b)
	{
		A = a;
		B = b;
	}

	/// <summary>
	/// Returns the hash code for this instance.
	/// </summary>
	/// <returns>
	/// A 32-bit signed integer that is the hash code for this instance.
	/// </returns>
	/// <filterpriority>2</filterpriority>
	public override int GetHashCode()
	{
		return A.GetHashCode() + B.GetHashCode();
	}

	/// <summary>
	/// Indicates whether the current object is equal to another object of the same type.
	/// </summary>
	/// <returns>
	/// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
	/// </returns>
	/// <param name="other">An object to compare with this object.</param>
	public bool Equals(TypePair other)
	{
		if ((object)other.A != A || (object)other.B != B)
		{
			if ((object)other.B == A)
			{
				return (object)other.A == B;
			}
			return false;
		}
		return true;
	}
}
