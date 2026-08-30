using System;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace ProjectMercury;

public struct VariableFloat3 : IEquatable<VariableFloat3>
{
	public Vector3 Value;

	public Vector3 Variation;

	public Vector3 Sample()
	{
		return new Vector3
		{
			X = RandomHelper.Variation(Value.X, Variation.X),
			Y = RandomHelper.Variation(Value.Y, Variation.Y),
			Z = RandomHelper.Variation(Value.Z, Variation.Z)
		};
	}

	public static implicit operator VariableFloat3(Vector3 value)
	{
		return new VariableFloat3
		{
			Value = value,
			Variation = Vector3.Zero
		};
	}

	public static implicit operator Vector3(VariableFloat3 value)
	{
		return value.Sample();
	}

	public override bool Equals(object obj)
	{
		if (obj is VariableFloat3)
		{
			return Equals((VariableFloat3)obj);
		}
		return false;
	}

	public bool Equals(VariableFloat3 other)
	{
		return Value == other.Value && Variation == other.Variation;
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode() + Variation.GetHashCode();
	}

	public override string ToString()
	{
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		return $"{{Value:{Value}, Variation:{Variation}}}";
	}
}
