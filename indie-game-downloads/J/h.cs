using System;

namespace J;

internal struct h(Type a, Type b) : IEquatable<h>
{
	public Type A = a;

	public Type B = b;

	public override int GetHashCode()
	{
		return A.GetHashCode() + B.GetHashCode();
	}

	public bool Equals(h other)
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
