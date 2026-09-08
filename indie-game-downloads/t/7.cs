using System;

namespace T;

internal struct _7(T._6 a, T._6 b) : IEquatable<_7>
{
	public T._6 MaterialA = a;

	public T._6 MaterialB = b;

	public override int GetHashCode()
	{
		return MaterialA.GetHashCode() + MaterialB.GetHashCode();
	}

	public bool Equals(_7 other)
	{
		if (other.MaterialA != MaterialA || other.MaterialB != MaterialB)
		{
			if (other.MaterialA == MaterialB)
			{
				return other.MaterialB == MaterialA;
			}
			return false;
		}
		return true;
	}
}
