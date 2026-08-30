using System;
using System.Globalization;

namespace ProjectMercury;

public struct VariableFloat : IEquatable<VariableFloat>
{
	public float Value;

	public float Variation;

	public float Sample()
	{
		if (Calculator.Abs(Variation) <= float.Epsilon)
		{
			return Value;
		}
		return RandomHelper.NextFloat(Value - Variation, Value + Variation);
	}

	public float Sample(Range clampRange)
	{
		if (Calculator.Abs(Variation) <= float.Epsilon)
		{
			return Calculator.Clamp(Value, clampRange);
		}
		float value = RandomHelper.NextFloat(Value - Variation, Value + Variation);
		return Calculator.Clamp(value, clampRange);
	}

	public static implicit operator VariableFloat(float value)
	{
		return new VariableFloat
		{
			Value = value,
			Variation = 0f
		};
	}

	public static implicit operator float(VariableFloat value)
	{
		return value.Sample();
	}

	public override bool Equals(object obj)
	{
		if (obj is VariableFloat)
		{
			return Equals((VariableFloat)obj);
		}
		return false;
	}

	public bool Equals(VariableFloat other)
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
		return $"{{Value:{Value.ToString(currentCulture)}, Variation:{Variation.ToString(currentCulture)}}}";
	}
}
