using System;

namespace ProjectMercury;

public static class Calculator
{
	public const float Pi = 3.141593f;

	public const float TwoPi = 6.283185f;

	public const float PiOver2 = 1.570796f;

	public const float PiOver4 = (float)Math.PI / 4f;

	public static float Clamp(float value, float min, float max)
	{
		value = ((value > max) ? max : value);
		value = ((value < min) ? min : value);
		return value;
	}

	public static float Clamp(float value, Range range)
	{
		value = ((value > range.Maximum) ? range.Maximum : value);
		value = ((value < range.Minimum) ? range.Minimum : value);
		return value;
	}

	public static void Clamp(ref float value, float min, float max)
	{
		value = ((value > max) ? max : value);
		value = ((value < min) ? min : value);
	}

	public static void Clamp(ref float value, Range range)
	{
		value = ((value > range.Maximum) ? range.Maximum : value);
		value = ((value < range.Minimum) ? range.Minimum : value);
	}

	public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
	{
		value = ((value.CompareTo(max) > 0) ? max : value);
		value = ((value.CompareTo(min) < 0) ? min : value);
		return value;
	}

	public static float Wrap(float value, float min, float max)
	{
		float num = max - min;
		if (value < min)
		{
			do
			{
				value += num;
			}
			while (value < min);
		}
		else if (value > max)
		{
			do
			{
				value -= num;
			}
			while (value > max);
		}
		return value;
	}

	public static float Wrap(float value, Range range)
	{
		if (value < range.Minimum)
		{
			do
			{
				value += range.Size;
			}
			while (value < range.Minimum);
		}
		else if (value > range.Maximum)
		{
			do
			{
				value -= range.Size;
			}
			while (value > range.Maximum);
		}
		return value;
	}

	public static void Wrap(ref float value, float min, float max)
	{
		float num = max - min;
		if (value < min)
		{
			do
			{
				value += num;
			}
			while (value < min);
		}
		else if (value > max)
		{
			do
			{
				value -= num;
			}
			while (value > max);
		}
	}

	public static void Wrap(ref float value, Range range)
	{
		if (value < range.Minimum)
		{
			do
			{
				value += range.Size;
			}
			while (value < range.Minimum);
		}
		else if (value > range.Maximum)
		{
			do
			{
				value -= range.Size;
			}
			while (value > range.Maximum);
		}
	}

	public static float LinearInterpolate(float value1, float value2, float amount)
	{
		return value1 + (value2 - value1) * amount;
	}

	public static void LinearInterpolate(ref float value, float value1, float value2, float amount)
	{
		value = value1 + (value2 - value1) * amount;
	}

	public static float LinearInterpolate(float value1, float value2, float value2Position, float value3, float amount)
	{
		if (amount < value2Position)
		{
			return LinearInterpolate(value1, value2, amount / value2Position);
		}
		return LinearInterpolate(value2, value3, (amount - value2Position) / (1f - value2Position));
	}

	public static float CubicInterpolate(float value1, float value2, float amount)
	{
		Clamp(ref amount, 0f, 1f);
		return LinearInterpolate(value1, value2, amount * amount * (3f - 2f * amount));
	}

	public static void CubicInterpolate(ref float value, float value1, float value2, float amount)
	{
		Clamp(ref amount, 0f, 1f);
		LinearInterpolate(ref value, value1, value2, amount * amount * (3f - 2f * amount));
	}

	public static float Max(float value1, float value2)
	{
		return (value1 >= value2) ? value1 : value2;
	}

	public static float Max(float value1, float value2, float value3)
	{
		return (!(value2 >= value3)) ? ((value1 >= value3) ? value1 : value3) : ((value1 >= value2) ? value1 : value2);
	}

	public static void Max(ref float value, float value1, float value2)
	{
		value = ((value1 >= value2) ? value1 : value2);
	}

	public static void Max(ref float value, float value1, float value2, float value3)
	{
		value = ((!(value2 >= value3)) ? ((value1 >= value3) ? value1 : value3) : ((value1 >= value2) ? value1 : value2));
	}

	public static T Max<T>(T value1, T value2) where T : IComparable<T>
	{
		return (value1.CompareTo(value2) >= 0) ? value1 : value2;
	}

	public static T Max<T>(T value1, T value2, T value3) where T : IComparable<T>
	{
		object result;
		if (value2.CompareTo(value3) < 0)
		{
			result = ((value1.CompareTo(value3) >= 0) ? value1 : value3);
		}
		else
		{
			T other = value2;
			result = ((value1.CompareTo(other) >= 0) ? value1 : value2);
		}
		return (T)result;
	}

	public static float Min(float value1, float value2)
	{
		return (value1 <= value2) ? value1 : value2;
	}

	public static float Min(float value1, float value2, float value3)
	{
		return (!(value2 <= value3)) ? ((value1 <= value3) ? value1 : value3) : ((value1 <= value2) ? value1 : value2);
	}

	public static void Min(ref float value, float value1, float value2)
	{
		value = ((value1 <= value2) ? value1 : value2);
	}

	public static void Min(ref float value, float value1, float value2, float value3)
	{
		value = ((!(value2 <= value3)) ? ((value1 <= value3) ? value1 : value3) : ((value1 <= value2) ? value1 : value2));
	}

	public static T Min<T>(T value1, T value2) where T : IComparable<T>
	{
		return (value1.CompareTo(value2) <= 0) ? value1 : value2;
	}

	public static T Min<T>(T value1, T value2, T value3) where T : IComparable<T>
	{
		object result;
		if (value2.CompareTo(value3) > 0)
		{
			result = ((value1.CompareTo(value3) <= 0) ? value1 : value3);
		}
		else
		{
			T other = value2;
			result = ((value1.CompareTo(other) <= 0) ? value1 : value2);
		}
		return (T)result;
	}

	public static float Abs(float value)
	{
		return (value >= 0f) ? value : (0f - value);
	}

	public static void Abs(ref float value)
	{
		value = ((value >= 0f) ? value : (0f - value));
	}

	public static float Acos(float value)
	{
		return (float)Math.Acos(value);
	}

	public static float Asin(float value)
	{
		return (float)Math.Asin(value);
	}

	public static float Atan(float value)
	{
		return (float)Math.Atan(value);
	}

	public static float Atan2(float y, float x)
	{
		return (float)Math.Atan2(y, x);
	}

	public static float Sin(float value)
	{
		return (float)Math.Sin(value);
	}

	public static float Sinh(float value)
	{
		return (float)Math.Sinh(value);
	}

	public static float Cos(float value)
	{
		return (float)Math.Cos(value);
	}

	public static float Cosh(float value)
	{
		return (float)Math.Cosh(value);
	}

	public static float Tan(float value)
	{
		return (float)Math.Tan(value);
	}

	public static float Tanh(float value)
	{
		return (float)Math.Tanh(value);
	}

	public static float Log(float value)
	{
		return (float)Math.Log(value);
	}

	public static float Pow(float value, float power)
	{
		return (float)Math.Pow(value, power);
	}

	public static float Sqrt(float value)
	{
		return (float)Math.Sqrt(value);
	}
}
