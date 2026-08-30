#define DEBUG
using System;
using System.Globalization;

namespace ProjectMercury;

public struct Range(float minimum, float maximum) : IEquatable<Range>, IFormattable
{
	public float Minimum = minimum;

	public float Maximum = maximum;

	public float Size => Calculator.Abs(Maximum - Minimum);

	public bool Contains(Range range)
	{
		return Minimum <= range.Minimum && Maximum >= range.Maximum;
	}

	public bool Contains(float value)
	{
		return Minimum <= value && Maximum >= value;
	}

	public void Merge(Range value)
	{
		Minimum = ((Minimum < value.Minimum) ? Minimum : value.Minimum);
		Maximum = ((Maximum > value.Maximum) ? Maximum : value.Maximum);
	}

	public void Intersect(Range value)
	{
		Minimum = ((Minimum > value.Minimum) ? Minimum : value.Minimum);
		Maximum = ((Maximum < value.Maximum) ? Maximum : value.Maximum);
	}

	public void Subtract(Range value)
	{
		Range range = Intersect(this, value);
		if (range.Minimum > Minimum)
		{
			Maximum = range.Minimum;
		}
		else if (range.Maximum > Minimum)
		{
			Minimum = range.Maximum;
		}
	}

	public static Range Union(Range x, Range y)
	{
		return new Range
		{
			Minimum = ((x.Minimum < y.Minimum) ? x.Minimum : y.Minimum),
			Maximum = ((x.Maximum > y.Maximum) ? x.Maximum : y.Maximum)
		};
	}

	public static Range Intersect(Range x, Range y)
	{
		return new Range
		{
			Minimum = ((x.Minimum > y.Minimum) ? x.Minimum : y.Minimum),
			Maximum = ((x.Maximum < y.Maximum) ? x.Maximum : y.Maximum)
		};
	}

	public static Range Subtract(Range x, Range y)
	{
		Range range = Intersect(x, y);
		Range result = default(Range);
		if (range.Minimum > x.Minimum)
		{
			result.Maximum = range.Minimum;
		}
		else if (range.Maximum > x.Maximum)
		{
			result.Minimum = range.Maximum;
		}
		return result;
	}

	public static Range Parse(string value)
	{
		return Parse(value, CultureInfo.InvariantCulture);
	}

	public static Range Parse(string value, IFormatProvider format)
	{
		Guard.ArgumentNull("value", value);
		Guard.ArgumentNull("format", format);
		if (value.StartsWith("[") && value.EndsWith("]"))
		{
			NumberFormatInfo instance = NumberFormatInfo.GetInstance(format);
			char[] separator = instance.NumberGroupSeparator.ToCharArray();
			string[] array = value.Trim('[', ']').Split(separator);
			if (array.Length == 2)
			{
				return new Range
				{
					Minimum = float.Parse(array[0], NumberStyles.Float, instance),
					Maximum = float.Parse(array[1], NumberStyles.Float, instance)
				};
			}
		}
		throw new FormatException("Value is not in ISO 31-11 format for a closed interval.");
	}

	public override bool Equals(object obj)
	{
		if (obj != null && obj is Range)
		{
			return Equals((Range)obj);
		}
		return false;
	}

	public bool Equals(Range value)
	{
		return Minimum.Equals(value.Minimum) && Maximum.Equals(value.Maximum);
	}

	public override int GetHashCode()
	{
		return Minimum.GetHashCode() ^ Maximum.GetHashCode();
	}

	public override string ToString()
	{
		return ToString("G", CultureInfo.InvariantCulture);
	}

	public string ToString(IFormatProvider formatProvider)
	{
		return ToString("G", formatProvider);
	}

	public string ToString(string format, IFormatProvider formatProvider)
	{
		NumberFormatInfo instance = NumberFormatInfo.GetInstance(formatProvider);
		string text = Minimum.ToString(format, instance);
		string text2 = Maximum.ToString(format, instance);
		string numberGroupSeparator = instance.NumberGroupSeparator;
		return string.Format(formatProvider, "[{0}{1}{2}]", new object[3] { text, numberGroupSeparator, text2 });
	}

	public static Range operator +(Range x, Range y)
	{
		return Union(x, y);
	}

	public static Range operator -(Range x, Range y)
	{
		return Subtract(x, y);
	}

	public static Range operator |(Range x, Range y)
	{
		return Intersect(x, y);
	}

	public static bool operator ==(Range x, Range y)
	{
		return x.Equals(y);
	}

	public static bool operator !=(Range x, Range y)
	{
		return !x.Equals(y);
	}
}
