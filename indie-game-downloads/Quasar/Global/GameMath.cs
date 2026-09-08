using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Xna.Framework;
using Quasar.Global.Vertex;

namespace Quasar.Global;

public static class GameMath
{
	public const string TRUE_VALUE = "true";

	public const string FALSE_VALUE = "false";

	public const float DOWN = -(float)Math.PI / 2f;

	public const float UP = (float)Math.PI / 2f;

	public const float RIGHT = 0f;

	public const float LEFT = (float)Math.PI;

	public const float PI = (float)Math.PI;

	public const float QUARTER_CIRCLE = (float)Math.PI / 2f;

	public const float HALF_CIRCLE = (float)Math.PI;

	public const float FULL_CIRCLE = (float)Math.PI * 2f;

	public const float HALFQUARTER_CIRCLE = (float)Math.PI / 4f;

	public const float DEGREE_TO_RADIAN = (float)Math.PI / 180f;

	public const float RADIAN_TO_DEGREE = 57.29578f;

	private static FastRandom random = new FastRandom();

	public static FastRandom Random => random;

	public static void ParseVector2List(string list, List<Vector2> destination, bool emptyList)
	{
		string[] array = list.Split(';');
		if (emptyList)
		{
			destination.Clear();
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Length != 0)
			{
				destination.Add(ParseVector2(text));
			}
		}
	}

	public static List<Vector2> ParseVector2List(string list)
	{
		List<Vector2> list2 = new List<Vector2>();
		ParseVector2List(list, list2, emptyList: false);
		return list2;
	}

	public static void ParseIntList(string list, List<int> destination, bool emptyList)
	{
		string[] array = list.Split(',');
		if (emptyList)
		{
			destination.Clear();
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Length != 0)
			{
				destination.Add(ParseInt(text));
			}
		}
	}

	public static string IntListToString(List<int> list)
	{
		string text = "";
		string text2 = "";
		foreach (int item in list)
		{
			text = text + text2 + IntToString(item);
			text2 = ",";
		}
		return text;
	}

	public static List<int> ParseIntList(string list)
	{
		List<int> list2 = new List<int>();
		ParseIntList(list, list2, emptyList: false);
		return list2;
	}

	public static void ParseFloatList(string list, List<float> destination, bool emptyList)
	{
		string[] array = list.Split(',');
		if (emptyList)
		{
			destination.Clear();
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Length != 0)
			{
				destination.Add(ParseFloat(text));
			}
		}
	}

	public static List<float> ParseFloatList(string list)
	{
		List<float> list2 = new List<float>();
		ParseFloatList(list, list2, emptyList: false);
		return list2;
	}

	public static int ParseInt(string text)
	{
		return ParseInt(text, 0);
	}

	public static int ParseInt(string text, int defaultValue)
	{
		if (text.Length == 0)
		{
			return defaultValue;
		}
		try
		{
			return int.Parse(text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
		}
		catch (Exception)
		{
			return defaultValue;
		}
	}

	public static uint ParseUInt(string text)
	{
		return ParseUInt(text, 0u);
	}

	public static uint ParseUInt(string text, uint defaultValue)
	{
		if (text.Length == 0)
		{
			return defaultValue;
		}
		try
		{
			return uint.Parse(text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
		}
		catch (Exception)
		{
			return defaultValue;
		}
	}

	public static long ParseLong(string text)
	{
		return ParseLong(text, 0L);
	}

	public static long ParseLong(string text, long defaultValue)
	{
		if (text.Length == 0)
		{
			return defaultValue;
		}
		try
		{
			return long.Parse(text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
		}
		catch (Exception)
		{
			return defaultValue;
		}
	}

	public static float ParseFloat(string text)
	{
		return ParseFloat(text, 0f);
	}

	public static float ParseFloat(string text, float defaultValue)
	{
		if (text.Length == 0)
		{
			return defaultValue;
		}
		try
		{
			return float.Parse(text, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
		}
		catch (Exception)
		{
			return defaultValue;
		}
	}

	public static bool ParseBool(string text)
	{
		return ParseBool(text, defaultValue: false);
	}

	public static bool ParseBool(string text, bool defaultValue)
	{
		if (text == "")
		{
			return defaultValue;
		}
		return text.Equals("true", StringComparison.InvariantCultureIgnoreCase);
	}

	public static string BoolToString(bool value)
	{
		if (!value)
		{
			return "false";
		}
		return "true";
	}

	public static Int2 ParseInt2(string xy)
	{
		return ParseInt2(xy, Int2.Zero);
	}

	public static Int2 ParseInt2(string xy, Int2 defaultValue)
	{
		string[] array = xy.Split(' ', ',');
		if (array.Length != 2)
		{
			return defaultValue;
		}
		return ParseInt2(array[0], array[1], defaultValue);
	}

	public static Int2 ParseInt2(string x, string y)
	{
		return ParseInt2(x, y, Int2.Zero);
	}

	public static Int2 ParseInt2(string x, string y, Int2 defaultValues)
	{
		return ParseInt2(x, y, ref defaultValues);
	}

	public static Int2 ParseInt2(string x, string y, ref Int2 defaultValues)
	{
		if (x.Length == 0 || y.Length == 0)
		{
			return defaultValues;
		}
		try
		{
			return new Int2(int.Parse(x, NumberStyles.Integer, NumberFormatInfo.InvariantInfo), int.Parse(y, NumberStyles.Integer, NumberFormatInfo.InvariantInfo));
		}
		catch (Exception)
		{
			return defaultValues;
		}
	}

	public static Vector2 ParseVector2(string xy)
	{
		return ParseVector2(xy, Vector2.Zero);
	}

	public static Vector2 ParseVector2(string xy, Vector2 defaultValue)
	{
		string[] array = xy.Split(' ', ',');
		if (array.Length != 2)
		{
			return defaultValue;
		}
		return ParseVector2(array[0], array[1], defaultValue);
	}

	public static string VectorToString(Vector2 v)
	{
		return v.X.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + v.Y.ToString(CultureInfo.InvariantCulture.NumberFormat);
	}

	public static string VectorListToString(List<Vector2> list)
	{
		string text = "";
		bool flag = false;
		foreach (Vector2 item in list)
		{
			if (flag)
			{
				text += ";";
			}
			text += VectorToString(item);
			flag = true;
		}
		return text;
	}

	public static string VectorToString(ref Vector2 v)
	{
		return v.X.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + v.Y.ToString(CultureInfo.InvariantCulture.NumberFormat);
	}

	public static string VectorToString(ref Vector3 v)
	{
		return v.X.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + v.Y.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + v.Z.ToString(CultureInfo.InvariantCulture.NumberFormat);
	}

	public static string VectorToString(Vector3 v)
	{
		return v.X.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + v.Y.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + v.Z.ToString(CultureInfo.InvariantCulture.NumberFormat);
	}

	public static string Int2ToString(Int2 v)
	{
		return IntToString(v.X) + "," + IntToString(v.Y);
	}

	public static void VectorToString(ref Vector2 v, StringBuilder sb)
	{
		FloatToString(v.X, sb);
		sb.Append(",");
		FloatToString(v.Y, sb);
	}

	public static void VectorToString(ref Vector3 v, StringBuilder sb)
	{
		FloatToString(v.X, sb);
		sb.Append(",");
		FloatToString(v.Y, sb);
		sb.Append(",");
		FloatToString(v.Z, sb);
	}

	public static void Int2ToString(ref Int2 v, StringBuilder sb)
	{
		StringBuilderExtensions.AppendNumber(sb, v.X);
		sb.Append(",");
		StringBuilderExtensions.AppendNumber(sb, v.Y);
	}

	public static string IntToString(int i)
	{
		return i.ToString(CultureInfo.InvariantCulture.NumberFormat);
	}

	public static string IntToString(int i, string format)
	{
		return i.ToString(format, CultureInfo.InvariantCulture.NumberFormat);
	}

	public static string FloatToString(float f, string format)
	{
		return f.ToString(format, CultureInfo.InvariantCulture.NumberFormat);
	}

	public static string FloatToString(float f)
	{
		return f.ToString(CultureInfo.InvariantCulture.NumberFormat);
	}

	public static void FloatToString(float f, StringBuilder sb)
	{
		sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", new object[1] { f });
	}

	public static string UIntToString(uint i)
	{
		return i.ToString(CultureInfo.InvariantCulture.NumberFormat);
	}

	public static string UIntToString(uint i, string format)
	{
		return i.ToString(format, CultureInfo.InvariantCulture.NumberFormat);
	}

	public static string LongToString(long l)
	{
		return l.ToString(CultureInfo.InvariantCulture.NumberFormat);
	}

	public static string LongToString(long l, string format)
	{
		return l.ToString(format, CultureInfo.InvariantCulture.NumberFormat);
	}

	public static Vector2 ParseVector2(string x, string y)
	{
		return ParseVector2(x, y, Vector2.Zero);
	}

	public static Vector2 ParseVector2(string x, string y, Vector2 defaultValues)
	{
		return ParseVector2(x, y, ref defaultValues);
	}

	public static Vector2 ParseVector2(string x, string y, ref Vector2 defaultValues)
	{
		if (x.Length == 0 || y.Length == 0)
		{
			return defaultValues;
		}
		try
		{
			return new Vector2(float.Parse(x, NumberStyles.Float, NumberFormatInfo.InvariantInfo), float.Parse(y, NumberStyles.Float, NumberFormatInfo.InvariantInfo));
		}
		catch (Exception)
		{
			return defaultValues;
		}
	}

	public static Vector3 ParseVector3(string xyz)
	{
		return ParseVector3(xyz, Vector3.Zero);
	}

	public static Vector3 ParseVector3(string xyz, Vector3 defaultValue)
	{
		string[] array = xyz.Split(' ', ',');
		if (array.Length != 3)
		{
			return defaultValue;
		}
		return ParseVector3(array[0], array[1], array[2], defaultValue);
	}

	public static Vector3 ParseVector3(string x, string y, string z)
	{
		return ParseVector3(x, y, z, Vector3.Zero);
	}

	public static Vector3 ParseVector3(string x, string y, string z, Vector3 defaultValues)
	{
		return ParseVector3(x, y, z, ref defaultValues);
	}

	public static Vector3 ParseVector3(string x, string y, string z, ref Vector3 defaultValues)
	{
		if (x.Length == 0 || y.Length == 0 || z.Length == 0)
		{
			return defaultValues;
		}
		try
		{
			return new Vector3(float.Parse(x, NumberStyles.Float, NumberFormatInfo.InvariantInfo), float.Parse(y, NumberStyles.Float, NumberFormatInfo.InvariantInfo), float.Parse(z, NumberStyles.Float, NumberFormatInfo.InvariantInfo));
		}
		catch (Exception)
		{
			return defaultValues;
		}
	}

	public static Vector4 ParseVector4(string xyzw)
	{
		return ParseVector4(xyzw, Vector4.Zero);
	}

	public static Vector4 ParseVector4(string xyzw, Vector4 defaultValue)
	{
		string[] array = xyzw.Split(' ', ',');
		return ParseVector4(array[0], array[1], array[2], array[3], defaultValue);
	}

	public static Vector4 ParseVector4(string x, string y, string z, string w)
	{
		return ParseVector4(x, y, z, w, Vector4.Zero);
	}

	public static Vector4 ParseVector4(string x, string y, string z, string w, Vector4 defaultValues)
	{
		return ParseVector4(x, y, z, w, ref defaultValues);
	}

	public static Vector4 ParseVector4(string x, string y, string z, string w, ref Vector4 defaultValues)
	{
		if (x.Length == 0 || y.Length == 0 || z.Length == 0 || w.Length == 0)
		{
			return defaultValues;
		}
		try
		{
			return new Vector4(float.Parse(x, NumberStyles.Float, NumberFormatInfo.InvariantInfo), float.Parse(y, NumberStyles.Float, NumberFormatInfo.InvariantInfo), float.Parse(z, NumberStyles.Float, NumberFormatInfo.InvariantInfo), float.Parse(w, NumberStyles.Float, NumberFormatInfo.InvariantInfo));
		}
		catch (Exception)
		{
			return defaultValues;
		}
	}

	public static Vector4 ParseColor(string text)
	{
		return ParseColor(text, Vector4.One);
	}

	public static Vector4 ParseColor(string text, Vector4 defaultColor)
	{
		int num = 0;
		if (text.Length == 0)
		{
			return defaultColor;
		}
		int num2 = 0;
		int startIndex = 0;
		while ((startIndex = text.IndexOf(',', startIndex)) != -1)
		{
			startIndex++;
			num2++;
		}
		if (text[0] == '(' && text.Length > 2)
		{
			switch (num2)
			{
			case 2:
			{
				Vector3 vector2 = ParseVector3(text.Substring(1, text.Length - 2));
				return new Vector4(vector2 / 255f, defaultColor.W);
			}
			case 3:
			{
				Vector4 vector = ParseVector4(text.Substring(1, text.Length - 2));
				return vector / 255f;
			}
			}
		}
		switch (num2)
		{
		case 2:
			return new Vector4(ParseVector3(text, new Vector3(defaultColor.X, defaultColor.Y, defaultColor.Z)), defaultColor.W);
		case 3:
			return ParseVector4(text, defaultColor);
		default:
			try
			{
				if (text[0] == '#')
				{
					num = int.Parse(text.Substring(1), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
					return new Vector4(RGBToVector((byte)((num & 0xFF0000) >> 16), (byte)((num & 0xFF00) >> 8), (byte)(num & 0xFF)), 1f);
				}
				num = int.Parse(text, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
				return new Vector4(RGBToVector((byte)((num & 0xFF0000) >> 16), (byte)((num & 0xFF00) >> 8), (byte)(num & 0xFF)), 1f);
			}
			catch (Exception)
			{
				return defaultColor;
			}
		}
	}

	public static Vector3 ParseColor3(string text)
	{
		return ParseColor3(text, Vector3.One);
	}

	public static Vector3 ParseColor3(string text, Vector3 defaultColor)
	{
		int num = 0;
		if (text.Length == 0)
		{
			return defaultColor;
		}
		int num2 = 0;
		int startIndex = 0;
		while ((startIndex = text.IndexOf(',', startIndex)) != -1)
		{
			startIndex++;
			num2++;
		}
		if (text[0] == '(' && text.Length > 2 && num2 == 2)
		{
			Vector3 vector = ParseVector3(text.Substring(1, text.Length - 2));
			return vector / 255f;
		}
		if (num2 == 2)
		{
			return ParseVector3(text, defaultColor);
		}
		try
		{
			if (text[0] == '#')
			{
				num = int.Parse(text.Substring(1), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
				return RGBToVector((byte)((num & 0xFF0000) >> 16), (byte)((num & 0xFF00) >> 8), (byte)(num & 0xFF));
			}
			num = int.Parse(text, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
			return RGBToVector((byte)((num & 0xFF0000) >> 16), (byte)((num & 0xFF00) >> 8), (byte)(num & 0xFF));
		}
		catch (Exception)
		{
			return defaultColor;
		}
	}

	public static Color ParseColor(string text, ref Color defaultColor)
	{
		int num = 0;
		if (text.Length == 0)
		{
			return defaultColor;
		}
		try
		{
			if (text[0] == '#')
			{
				num = int.Parse(text.Substring(1), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
				return new Color((byte)((num & 0xFF0000) >> 16), (byte)((num & 0xFF00) >> 8), (byte)(num & 0xFF));
			}
			num = int.Parse(text, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
			return new Color((byte)((num & 0xFF0000) >> 16), (byte)((num & 0xFF00) >> 8), (byte)(num & 0xFF));
		}
		catch (Exception)
		{
		}
		return defaultColor;
	}

	public static string ColorToString(Color color)
	{
		return "#" + $"{color.R:X2}{color.G:X2}{color.B:X2}";
	}

	public static string ColorToString(ref Color color)
	{
		return "#" + $"{color.R:X2}{color.G:X2}{color.B:X2}";
	}

	public static string DateToString(DateTime date)
	{
		return DateToString(ref date);
	}

	public static string DateToString(ref DateTime date)
	{
		return date.ToString("MM/dd/yyyy");
	}

	public static void ParseDate(string text, ref DateTime date)
	{
		DateTime defaultDate = default(DateTime);
		ParseDate(text, ref date, ref defaultDate);
	}

	public static void ParseDate(string text, ref DateTime date, ref DateTime defaultDate)
	{
		if (text.Length == 0)
		{
			date = defaultDate;
		}
		try
		{
			date = DateTime.ParseExact(text, "MM/dd/yyyy", CultureInfo.InvariantCulture);
		}
		catch (Exception)
		{
		}
	}

	public static string RemoveSpecialChars(string text)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in text)
		{
			if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || ((c >= 'a' && c <= 'z') | (c == ' ')))
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	public static string GetFilenameFromString(string text)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in text)
		{
			if (c >= '0' && c <= '9')
			{
				stringBuilder.Append(c);
			}
			else if (c >= 'A' && c <= 'Z')
			{
				stringBuilder.Append(char.ToLower(c));
			}
			else if (c >= 'a' && c <= 'z')
			{
				stringBuilder.Append(c);
			}
			else if (c == ' ')
			{
				stringBuilder.Append('_');
			}
		}
		return stringBuilder.ToString();
	}

	public static string OrdinalFor(int num)
	{
		return OrdinalFor(num, toUpper: false);
	}

	public static string OrdinalFor(int num, bool toUpper)
	{
		switch (num % 100)
		{
		case 11:
		case 12:
		case 13:
			if (!toUpper)
			{
				return "th";
			}
			return "TH";
		default:
			switch (num % 10)
			{
			case 1:
				if (!toUpper)
				{
					return "st";
				}
				return "ST";
			case 2:
				if (!toUpper)
				{
					return "nd";
				}
				return "ND";
			case 3:
				if (!toUpper)
				{
					return "rd";
				}
				return "RD";
			default:
				if (!toUpper)
				{
					return "th";
				}
				return "TH";
			}
		}
	}

	public static void OrdinalFor(int num, StringBuilder sb)
	{
		sb.Append(OrdinalFor(num));
	}

	public static void OrdinalFor(int num, StringBuilder sb, bool toUpper)
	{
		sb.Append(OrdinalFor(num, toUpper));
	}

	public unsafe static void CopyIntoString(ref string dest_string, char[] char_buffer, int length)
	{
		fixed (char* ptr = dest_string)
		{
			for (int i = 0; i < length; i++)
			{
				ptr[i] = char_buffer[i];
			}
			if (length < dest_string.Length)
			{
				ptr[length] = '\0';
			}
		}
	}

	public static float QuadraticInterpolation(float x)
	{
		return 3f * x * x - 2f * x * x * x;
	}

	public static float Approximate(float from, float to, float stepSize)
	{
		float num = from - to;
		if (Math.Abs(num) <= stepSize)
		{
			return to;
		}
		if (num > 0f)
		{
			return from - stepSize;
		}
		return from + stepSize;
	}

	public static Vector2 Approximate(Vector2 from, Vector2 to, float stepSize)
	{
		Vector2 v = to - from;
		if (v.Length() <= stepSize)
		{
			return to;
		}
		return from + VectorSetLength(v, stepSize);
	}

	public static Vector3 Approximate(Vector3 from, Vector3 to, float stepSize)
	{
		Vector3 v = to - from;
		if (v.Length() <= stepSize)
		{
			return to;
		}
		return from + VectorSetLength(v, stepSize);
	}

	public static int Approximate(int from, int to, int stepSize)
	{
		int num = from - to;
		if (Math.Abs(num) <= stepSize)
		{
			return to;
		}
		if (num > 0)
		{
			return from - stepSize;
		}
		return from + stepSize;
	}

	public static float Interpolate(float from, float to, float amount)
	{
		return to * amount + from * (1f - amount);
	}

	public static float InterpolateClamp(float from, float to, float amount)
	{
		amount = ((amount < 0f) ? 0f : ((amount > 1f) ? 1f : amount));
		return to * amount + from * (1f - amount);
	}

	public static float Damping(float from, float to, float smoothness)
	{
		return from * smoothness + to * (1f - smoothness);
	}

	public static Vector3 Damping(Vector3 from, Vector3 to, float smoothness)
	{
		return Vector3.Lerp(from, to, 1f - smoothness);
	}

	public static float Damping(float from, float to, float smoothness, float timeInterval)
	{
		float num = (float)Math.Pow(smoothness, timeInterval);
		return from * num + to * (1f - num);
	}

	public static Vector2 Damping(Vector2 from, Vector2 to, float smoothness, float timeInterval)
	{
		float num = (float)Math.Pow(smoothness, timeInterval);
		return from * num + to * (1f - num);
	}

	public static Vector2 Damping(ref Vector2 from, ref Vector2 to, float smoothness, float timeInterval)
	{
		float num = (float)Math.Pow(smoothness, timeInterval);
		return from * num + to * (1f - num);
	}

	public static Vector3 Damping(ref Vector3 from, ref Vector3 to, float smoothness, float timeInterval)
	{
		float num = (float)Math.Pow(smoothness, timeInterval);
		return from * num + to * (1f - num);
	}

	public static Vector3 Damping(Vector3 from, Vector3 to, float smoothness, float timeInterval)
	{
		float num = (float)Math.Pow(smoothness, timeInterval);
		return from * num + to * (1f - num);
	}

	public static Color Damping(Color from, Color to, float smoothness, float timeInterval)
	{
		float num = (float)Math.Pow(smoothness, timeInterval);
		return Color.Lerp(from, to, 1f - num);
	}

	public static Vector3 Damping(ref Vector3 from, ref Vector3 to, ref Vector3 smoothness, float timeInterval)
	{
		Vector3 zero = Vector3.Zero;
		zero.X = Damping(from.X, to.X, smoothness.X, timeInterval);
		zero.Y = Damping(from.Y, to.Y, smoothness.Y, timeInterval);
		zero.Z = Damping(from.Z, to.Z, smoothness.Z, timeInterval);
		return zero;
	}

	public static Vector3 Damping(Vector3 from, Vector3 to, Vector3 smoothness, float timeInterval)
	{
		Vector3 zero = Vector3.Zero;
		zero.X = Damping(from.X, to.X, smoothness.X, timeInterval);
		zero.Y = Damping(from.Y, to.Y, smoothness.Y, timeInterval);
		zero.Z = Damping(from.Z, to.Z, smoothness.Z, timeInterval);
		return zero;
	}

	public static Vector2 Normal(ref Vector2 line)
	{
		float x = line.X;
		float y = line.Y;
		Vector2 vector = default(Vector2);
		if (x != 0f)
		{
			vector.X = (0f - y) / x;
			vector.Y = 1f / (float)Math.Sqrt(vector.X * vector.X + 1f);
			vector.Normalize();
		}
		else if (y != 0f)
		{
			vector.Y = 0f;
			vector.X = ((line.Y < 0f) ? 1 : (-1));
		}
		else
		{
			vector.X = 1f;
			vector.Y = 0f;
		}
		if (x < 0f)
		{
			return vector * -1f;
		}
		return vector;
	}

	public static Vector2 Normal(Vector2 line)
	{
		float x = line.X;
		float y = line.Y;
		Vector2 vector = default(Vector2);
		if (x != 0f)
		{
			vector.X = (0f - y) / x;
			vector.Y = 1f / (float)Math.Sqrt(vector.X * vector.X + 1f);
			vector.Normalize();
		}
		else if (y != 0f)
		{
			vector.Y = 0f;
			vector.X = ((line.Y < 0f) ? 1 : (-1));
		}
		else
		{
			vector.X = 1f;
			vector.Y = 0f;
		}
		if (x < 0f)
		{
			return vector * -1f;
		}
		return vector;
	}

	public static Vector2 Normal(Vector2 from, Vector2 to)
	{
		return Normal(to - from);
	}

	public static Vector2 Normal(ref Vector2 from, ref Vector2 to)
	{
		Vector2 line = to - from;
		return Normal(ref line);
	}

	public static float VectorMaxValue(Vector3 v)
	{
		return Math.Max(v.X, Math.Max(v.Y, v.Z));
	}

	public static int Int2MaxValue(Int2 v)
	{
		return Math.Max(v.X, v.Y);
	}

	public static int Int2MinValue(Int2 v)
	{
		return Math.Min(v.X, v.Y);
	}

	public static float VectorMaxValue(ref Vector3 v)
	{
		return Math.Max(v.X, Math.Max(v.Y, v.Z));
	}

	public static float VectorMinValue(Vector3 v)
	{
		return Math.Min(v.X, Math.Min(v.Y, v.Z));
	}

	public static float VectorMinValue(ref Vector3 v)
	{
		return Math.Min(v.X, Math.Min(v.Y, v.Z));
	}

	public static Vector2 VectorAbs(Vector2 v)
	{
		return new Vector2(Math.Abs(v.X), Math.Abs(v.Y));
	}

	public static Vector2 VectorAbs(ref Vector2 v)
	{
		return new Vector2(Math.Abs(v.X), Math.Abs(v.Y));
	}

	public static Vector3 VectorAbs(Vector3 v)
	{
		return new Vector3(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));
	}

	public static Vector3 VectorAbs(ref Vector3 v)
	{
		return new Vector3(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));
	}

	public static Vector4 VectorAbs(Vector4 v)
	{
		return new Vector4(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z), Math.Abs(v.W));
	}

	public static Vector4 VectorAbs(ref Vector4 v)
	{
		return new Vector4(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z), Math.Abs(v.W));
	}

	public static Vector4 UnprojectVector(Vector4 v)
	{
		return v / v.W;
	}

	public static Vector4 UnprojectVector(ref Vector4 v)
	{
		return v / v.W;
	}

	public static Color RandomColor()
	{
		return new Color(random.NextFloat(), random.NextFloat(), random.NextFloat());
	}

	public static float RandomAngle()
	{
		return random.NextFloat() * ((float)Math.PI * 2f);
	}

	public static string RandomString(int length)
	{
		char[] array = new char[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = (char)Random.Next(97, 123);
		}
		return new string(array);
	}

	public static Vector2 RandomPointInCircle()
	{
		float length = (float)Math.Sqrt(Random.NextFloat());
		float angle = RandomAngle();
		return VectorFromAngle(angle, length);
	}

	public static Vector2 RandomVector2()
	{
		return new Vector2(random.NextFloat(), random.NextFloat());
	}

	public static Vector2 RandomVector2(Vector2 maxValue)
	{
		return new Vector2(random.NextFloat(maxValue.X), random.NextFloat(maxValue.Y));
	}

	public static Vector2 RandomVector2(float minValue, float maxValue)
	{
		return new Vector2(random.NextFloat(minValue, maxValue), random.NextFloat(minValue, maxValue));
	}

	public static Vector3 RandomVector3()
	{
		return new Vector3(random.NextFloat(), random.NextFloat(), random.NextFloat());
	}

	public static Vector3 RandomVector3(float minValue, float maxValue)
	{
		return new Vector3(random.NextFloat(minValue, maxValue), random.NextFloat(minValue, maxValue), random.NextFloat(minValue, maxValue));
	}

	public static Vector4 RandomVector4()
	{
		return new Vector4(random.NextFloat(), random.NextFloat(), random.NextFloat(), random.NextFloat());
	}

	public static float VectorAngle(Vector2 v)
	{
		if (v.X != 0f)
		{
			return (float)Math.Atan2(v.Y, v.X);
		}
		return (float)((v.Y > 0f) ? (Math.PI / 2.0) : (-Math.PI / 2.0));
	}

	public static float VectorAngle(ref Vector2 v)
	{
		if (v.X != 0f)
		{
			return (float)Math.Atan2(v.Y, v.X);
		}
		return (float)((v.Y > 0f) ? (Math.PI / 2.0) : (-Math.PI / 2.0));
	}

	public static float AngleInterpolate(float from, float to, float amount)
	{
		from = WrapAngle(from);
		to = WrapAngle(to);
		if (from - to > (float)Math.PI)
		{
			return Interpolate(from, to + (float)Math.PI * 2f, amount);
		}
		if (to - from > (float)Math.PI)
		{
			return Interpolate(from + (float)Math.PI * 2f, to, amount);
		}
		return Interpolate(from, to, amount);
	}

	public static float AngleApproximate(float from, float to, float stepSize)
	{
		from = WrapAngle(from);
		to = WrapAngle(to);
		float num = AngleDifferenceRadians(from, to);
		if (Math.Abs(num) <= stepSize)
		{
			return to;
		}
		if (num > 0f)
		{
			return from - stepSize;
		}
		return from + stepSize;
	}

	public static float AngleDamping(float from, float to, float smoothness)
	{
		from = WrapAngle(from);
		to = WrapAngle(to);
		if (from - to > (float)Math.PI)
		{
			return Damping(from, to + (float)Math.PI * 2f, smoothness);
		}
		if (to - from > (float)Math.PI)
		{
			return Damping(from + (float)Math.PI * 2f, to, smoothness);
		}
		return Damping(from, to, smoothness);
	}

	public static float AngleDamping(float from, float to, float smoothness, float time)
	{
		from = WrapAngle(from);
		to = WrapAngle(to);
		if (from - to > (float)Math.PI)
		{
			return Damping(from, to + (float)Math.PI * 2f, smoothness, time);
		}
		if (to - from > (float)Math.PI)
		{
			return Damping(from + (float)Math.PI * 2f, to, smoothness, time);
		}
		return Damping(from, to, smoothness, time);
	}

	public static Vector3 AngleDamping(Vector3 from, Vector3 to, float smoothness, float time)
	{
		return new Vector3(AngleDamping(from.X, to.X, smoothness, time), AngleDamping(from.Y, to.Y, smoothness, time), AngleDamping(from.Z, to.Z, smoothness, time));
	}

	public static float MinimumAngleDifferenceRadians(float a1, float a2)
	{
		float num = Math.Abs(a1 - a2);
		if (num > (float)Math.PI)
		{
			num -= (float)Math.Ceiling((num - (float)Math.PI) / ((float)Math.PI * 2f)) * 2f * (float)Math.PI;
		}
		return Math.Abs(num);
	}

	public static float AngleDifferenceRadians(float a1, float a2)
	{
		float num = a1 - a2;
		if (num > (float)Math.PI)
		{
			num -= (float)Math.Ceiling((num - (float)Math.PI) / ((float)Math.PI * 2f)) * 2f * (float)Math.PI;
		}
		if (num < -(float)Math.PI)
		{
			num += (float)Math.Ceiling((0f - (num + (float)Math.PI)) / ((float)Math.PI * 2f)) * 2f * (float)Math.PI;
		}
		return num;
	}

	public static Vector3 AngleDifferenceRadians(Vector3 a1, Vector3 a2)
	{
		return new Vector3(AngleDifferenceRadians(a1.X, a2.X), AngleDifferenceRadians(a1.Y, a2.Y), AngleDifferenceRadians(a1.Z, a2.Z));
	}

	public static bool IsFacing(Vector2 v, float angleCheck, float threshold)
	{
		float a = VectorAngle(ref v);
		float num = MinimumAngleDifferenceRadians(a, angleCheck);
		return num <= threshold;
	}

	public static bool IsFacing(ref Vector2 v, float angleCheck, float threshold)
	{
		float a = VectorAngle(ref v);
		float num = MinimumAngleDifferenceRadians(a, angleCheck);
		return num <= threshold;
	}

	public static Vector2 VectorSetLength(Vector2 v, float length)
	{
		if (ZeroLength(v))
		{
			return new Vector2(length, 0f);
		}
		return Vector2.Normalize(v) * length;
	}

	public static void VectorSetLength(ref Vector2 v, float length, out Vector2 result)
	{
		if (ZeroLength(v))
		{
			result = new Vector2(length, 0f);
			return;
		}
		Vector2.Normalize(ref v, out result);
		Vector2.Multiply(ref result, length, out result);
	}

	public static Vector3 VectorSetLength(Vector3 v, float length)
	{
		return Vector3.Normalize(v) * length;
	}

	public static void VectorSetLength(ref Vector3 v, float length, out Vector3 result)
	{
		Vector3.Normalize(ref v, out result);
		result = v * length;
	}

	public static Vector2 VectorFromAngle(float angle, float length)
	{
		return new Vector2((float)Math.Cos(angle) * length, (float)Math.Sin(angle) * length);
	}

	public static Vector2 VectorFromAngle(float angle)
	{
		return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
	}

	public static float Clamp(float min, float max, float value)
	{
		return Math.Max(min, Math.Min(max, value));
	}

	public static Vector2 Clamp(Vector2 min, Vector2 max, Vector2 value)
	{
		return new Vector2(Math.Max(min.X, Math.Min(max.X, value.X)), Math.Max(min.Y, Math.Min(max.Y, value.Y)));
	}

	public static float WrapAngle(float value)
	{
		return Mod(value, (float)Math.PI * 2f);
	}

	public static float WrapAngleDegrees(float value)
	{
		return Mod(value, 360f);
	}

	public static float ClampAngle(float value)
	{
		return Clamp(0f, (float)Math.PI * 2f, value);
	}

	public static Vector3 ClampAngle(Vector3 value)
	{
		return new Vector3(ClampAngle(value.X), ClampAngle(value.Y), ClampAngle(value.Z));
	}

	public static int Clamp(int min, int max, int value)
	{
		return Math.Max(min, Math.Min(max, value));
	}

	public static uint Clamp(uint min, uint max, uint value)
	{
		return Math.Max(min, Math.Min(max, value));
	}

	public static bool Between(float min, float max, float value)
	{
		if (value >= min)
		{
			return value <= max;
		}
		return false;
	}

	public static bool Between(int min, int max, int value)
	{
		if (value >= min)
		{
			return value <= max;
		}
		return false;
	}

	public static Vector3 MatrixScale(ref Matrix m)
	{
		return new Vector3(new Vector3(m.M11, m.M21, m.M31).Length(), new Vector3(m.M12, m.M22, m.M32).Length(), new Vector3(m.M13, m.M23, m.M33).Length());
	}

	public static Vector3 RGBToVector(byte r, byte g, byte b)
	{
		return new Vector3((float)(int)r / 255f, (float)(int)g / 255f, (float)(int)b / 255f);
	}

	public static Vector3 RGBToVector3(int rgb)
	{
		return RGBToVector((byte)((rgb & 0xFF000000u) >> 24), (byte)((rgb & 0xFF0000) >> 16), (byte)((rgb & 0xFF00) >> 8));
	}

	public static Vector3 RGBToVector3(uint rgb)
	{
		return RGBToVector((byte)((rgb & 0xFF000000u) >> 24), (byte)((rgb & 0xFF0000) >> 16), (byte)((rgb & 0xFF00) >> 8));
	}

	public static int VectorToRGB(Vector3 rgb)
	{
		return ((int)Math.Round(rgb.X * 255f) << 24) + ((int)Math.Round(rgb.Y * 255f) << 16) + ((int)Math.Round(rgb.Z * 255f) << 8) + 255;
	}

	public static Vector4 RGBAToVector(byte r, byte g, byte b, byte a)
	{
		return new Vector4((float)(int)r / 255f, (float)(int)g / 255f, (float)(int)b / 255f, (float)(int)a / 255f);
	}

	public static Vector3 RGBToHSV(Vector3 RGB)
	{
		return RGBToHSV(ref RGB);
	}

	public static Vector3 RGBToHSV(ref Vector3 RGB)
	{
		Vector3 result = default(Vector3);
		float num;
		float num2;
		if (RGB.X > RGB.Y)
		{
			num = RGB.X;
			num2 = RGB.Y;
		}
		else
		{
			num = RGB.Y;
			num2 = RGB.X;
		}
		if (RGB.Z > num)
		{
			num = RGB.Z;
		}
		else if (RGB.Z < num2)
		{
			num2 = RGB.Z;
		}
		float num3 = num - num2;
		result.Z = num;
		if (num == 0f)
		{
			result.Y = 0f;
		}
		else
		{
			result.Y = num3 / num;
		}
		float num4 = ((num3 != 0f) ? (60f / num3) : 0f);
		if (num == RGB.X)
		{
			if (RGB.Y < RGB.Z)
			{
				result.X = (360f + num4 * (RGB.Y - RGB.Z)) / 360f;
			}
			else
			{
				result.X = num4 * (RGB.Y - RGB.Z) / 360f;
			}
		}
		else if (num == RGB.Y)
		{
			result.X = (120f + num4 * (RGB.Z - RGB.X)) / 360f;
		}
		else if (num == RGB.Z)
		{
			result.X = (240f + num4 * (RGB.X - RGB.Y)) / 360f;
		}
		else
		{
			RGB.X = 0f;
		}
		return result;
	}

	public static Vector3 HSVtoRGB(Vector3 HSV)
	{
		return HSVtoRGB(ref HSV);
	}

	public static Vector3 HSVtoRGB(ref Vector3 HSV)
	{
		float x = HSV.X;
		float y = HSV.Y;
		float z = HSV.Z;
		float x2;
		float y2;
		float z2;
		if (y == 0f)
		{
			x2 = z;
			y2 = z;
			z2 = z;
		}
		else
		{
			float num = x * 6f;
			float num2 = (float)Math.Floor(num);
			float num3 = z * (1f - y);
			float num4 = z * (1f - y * (num - num2));
			float num5 = z * (1f - y * (1f - (num - num2)));
			if (num2 == 0f)
			{
				x2 = z;
				y2 = num5;
				z2 = num3;
			}
			else if (num2 == 1f)
			{
				x2 = num4;
				y2 = z;
				z2 = num3;
			}
			else if (num2 == 2f)
			{
				x2 = num3;
				y2 = z;
				z2 = num5;
			}
			else if (num2 == 3f)
			{
				x2 = num3;
				y2 = num4;
				z2 = z;
			}
			else if (num2 == 4f)
			{
				x2 = num5;
				y2 = num3;
				z2 = z;
			}
			else
			{
				x2 = z;
				y2 = num3;
				z2 = num4;
			}
		}
		return new Vector3(x2, y2, z2);
	}

	public static float Mod(float x, float y)
	{
		float num = x / y;
		float num2 = (float)Math.Floor(num);
		return (num - num2) * y;
	}

	public static float Div(float x, float y)
	{
		return (float)Math.Floor(x / y);
	}

	public static bool InsideBox(Vector2 boxCenter, Vector2 boxSize, Vector2 point)
	{
		Vector2 vector = VectorAbs(point - boxCenter);
		if (vector.X <= boxSize.X * 0.5f && vector.Y <= boxSize.Y * 0.5f)
		{
			return true;
		}
		return false;
	}

	public static bool InsideBounds(Vector2 minBound, Vector2 maxBound, Vector2 point)
	{
		if (point.X < minBound.X || point.X > maxBound.X || point.Y < minBound.Y || point.Y > maxBound.Y)
		{
			return false;
		}
		return true;
	}

	public static bool InsideBounds(Int2 minBound, Int2 maxBound, Int2 point)
	{
		if (point.X < minBound.X || point.X > maxBound.X || point.Y < minBound.Y || point.Y > maxBound.Y)
		{
			return false;
		}
		return true;
	}

	public static bool BoxInsideBounds(Vector2 minBound, Vector2 maxBound, Vector2 point, Vector2 size)
	{
		float num = size.X * 0.5f;
		float num2 = size.Y * 0.5f;
		if (point.X - num < minBound.X || point.X + num > maxBound.X || point.Y - num2 < minBound.Y || point.Y + num2 > maxBound.Y)
		{
			return false;
		}
		return true;
	}

	public static Vector2 ClampBoxInsideBounds(Vector2 minBound, Vector2 maxBound, Vector2 point, Vector2 size)
	{
		float x = size.X * 0.5f;
		float y = size.Y * 0.5f;
		return Vector2.Clamp(point, minBound + new Vector2(x, y), maxBound - new Vector2(x, y));
	}

	public static Vector2 ToVector2(this Vector3 v)
	{
		return new Vector2(v.X, v.Y);
	}

	public static float Saturate(float v)
	{
		if (!(v < 0f))
		{
			if (!(v > 1f))
			{
				return v;
			}
			return 1f;
		}
		return 0f;
	}

	public static Vector2 Saturate(ref Vector2 v)
	{
		return new Vector2((v.X < 0f) ? 0f : ((v.X > 1f) ? 1f : v.X), (v.Y < 0f) ? 0f : ((v.Y > 1f) ? 1f : v.Y));
	}

	public static bool ZeroLength(this Vector2 v)
	{
		if (v.X == 0f)
		{
			return v.Y == 0f;
		}
		return false;
	}

	public static bool ZeroLength(this Vector3 v)
	{
		if (v.X == 0f && v.Y == 0f)
		{
			return v.Z == 0f;
		}
		return false;
	}

	public static Vector4 ToVector4(this Vector3 v)
	{
		return new Vector4(v.X, v.Y, v.Z, 0f);
	}

	public static Vector3 ToVector3(this Vector4 v)
	{
		return new Vector3(v.X, v.Y, v.Z);
	}

	public static Vector3 ToVector3(this Vector2 v)
	{
		return new Vector3(v.X, v.Y, 0f);
	}

	public static Vector2 ToVector2(this Vector4 v)
	{
		return new Vector2(v.X, v.Y);
	}

	public static bool Overlaps(this List<string> l, List<string> l2)
	{
		int count = l.Count;
		for (int i = 0; i < count; i++)
		{
			if (l2.Contains(l[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static BoundingSphere CreateBoundingSphereFromPoints<T>(T[] vertices, int usedVertices) where T : struct, IPositionVertex
	{
		if (usedVertices == 0)
		{
			return new BoundingSphere(Vector3.Zero, 0f);
		}
		Vector3 vector = vertices[0].Position;
		Vector3 vector2 = vertices[0].Position;
		for (int i = 1; i < usedVertices; i++)
		{
			vector = Vector3.Min(vector, vertices[i].Position);
			vector2 = Vector3.Max(vector2, vertices[i].Position);
		}
		return new BoundingSphere((vector + vector2) * 0.5f, Vector3.Distance(vector, vector2) * 0.5f);
	}

	public static Vector3 QuaternionToYawPitchRoll(Quaternion q)
	{
		Vector3 zero = Vector3.Zero;
		zero.X = (float)Math.Atan2(2f * q.Y * q.W - 2f * q.X * q.Z, 1.0 - 2.0 * Math.Pow(q.Y, 2.0) - 2.0 * Math.Pow(q.Z, 2.0));
		zero.Z = (float)Math.Asin(2f * q.X * q.Y + 2f * q.Z * q.W);
		zero.Y = (float)Math.Atan2(2f * q.X * q.W - 2f * q.Y * q.Z, 1.0 - 2.0 * Math.Pow(q.X, 2.0) - 2.0 * Math.Pow(q.Z, 2.0));
		if ((double)(q.X * q.Y + q.Z * q.W) == 0.5)
		{
			zero.X = (float)(2.0 * Math.Atan2(q.X, q.W));
			zero.Y = 0f;
		}
		else if ((double)(q.X * q.Y + q.Z * q.W) == -0.5)
		{
			zero.X = (float)(-2.0 * Math.Atan2(q.X, q.W));
			zero.Y = 0f;
		}
		return zero;
	}

	public static float VerticalFOVToHorizontal(float verticalFOV, float aspectRatio)
	{
		return 2f * (float)Math.Atan(Math.Tan(verticalFOV / 2f) * (double)aspectRatio);
	}

	public static Vector2 InterceptDirection(Vector2 targetPosition, Vector2 targetSpeed, Vector2 turretPosition, float projectileSpeed)
	{
		if (targetPosition == turretPosition)
		{
			return Vector2.Zero;
		}
		if (projectileSpeed == 0f)
		{
			return Vector2.Zero;
		}
		float num = (targetPosition - turretPosition).Length();
		num /= projectileSpeed;
		Vector2 vector = targetPosition + targetSpeed * num;
		Vector2 result = vector - turretPosition;
		result.Normalize();
		return result;
	}

	public static Vector2 InterceptPoint(Vector2 targetPosition, Vector2 targetSpeed, Vector2 turretPosition, float projectileSpeed)
	{
		if (targetPosition == turretPosition)
		{
			return Vector2.Zero;
		}
		if (projectileSpeed == 0f)
		{
			return Vector2.Zero;
		}
		float num = (targetPosition - turretPosition).Length();
		num /= projectileSpeed;
		return targetPosition + targetSpeed * num;
	}

	public static Vector2 RotateVector(Vector2 v, float angle)
	{
		return v * (float)Math.Cos(angle) + new Vector2(0f - v.Y, v.X) * (float)Math.Sin(angle);
	}

	public static void RotateVector(ref Vector2 v, float angle, out Vector2 result)
	{
		result = v * (float)Math.Cos(angle) + new Vector2(0f - v.Y, v.X) * (float)Math.Sin(angle);
	}

	public static float DistancePointToLine(Vector2 lineA, Vector2 lineB, Vector2 point)
	{
		return Vector2.Distance(point, ClosestPointOnLine(lineA, lineB, point));
	}

	public static Vector2 ClosestPointOnLine(Vector2 A, Vector2 B, Vector2 P)
	{
		Vector2 vector = B - A;
		float num = Vector2.Dot(P - A, vector);
		if (num <= 0f)
		{
			return A;
		}
		float num2 = Vector2.Dot(vector, vector);
		if (num >= num2)
		{
			return B;
		}
		return A + num / num2 * vector;
	}

	public static bool IsPowerOfTwo(int value)
	{
		if ((value & (value - 1)) == 0)
		{
			return value != 0;
		}
		return false;
	}

	public static bool MatrixEquals(ref Matrix m1, ref Matrix m2)
	{
		if (m1.M11 == m2.M11 && m1.M12 == m2.M12 && m1.M13 == m2.M13 && m1.M14 == m2.M14 && m1.M21 == m2.M21 && m1.M22 == m2.M22 && m1.M23 == m2.M23 && m1.M24 == m2.M24 && m1.M31 == m2.M31 && m1.M32 == m2.M32 && m1.M33 == m2.M33 && m1.M34 == m2.M34 && m1.M41 == m2.M41 && m1.M42 == m2.M42 && m1.M43 == m2.M43)
		{
			return m1.M44 == m2.M44;
		}
		return false;
	}

	public static bool TestRectangleIntersection(ref Vector2 r1Pos, ref Vector2 r1Size, float r1Rotation, ref Vector2 r2Pos, ref Vector2 r2Size, float r2Rotation)
	{
		Vector2 vector = RotateVector(r1Size * 0.5f, r1Rotation);
		Vector2 vector2 = RotateVector(new Vector2(r1Size.X * 0.5f, (0f - r1Size.Y) * 0.5f), r1Rotation);
		Vector2 x = r1Pos + vector;
		Vector2 x2 = r1Pos + vector2;
		Vector2 x3 = r1Pos - vector;
		Vector2 x4 = r1Pos - vector2;
		Vector2 vector3 = RotateVector(r2Size * 0.5f, r2Rotation);
		Vector2 vector4 = RotateVector(new Vector2(r2Size.X * 0.5f, (0f - r2Size.Y) * 0.5f), r2Rotation);
		Vector2 q = r2Pos + vector3;
		Vector2 q2 = r2Pos + vector4;
		Vector2 q3 = r2Pos - vector3;
		Vector2 q4 = r2Pos - vector4;
		if (DoAxisSeparationTest(ref x, ref x2, ref x3, ref q, ref q2, ref q3, ref q4))
		{
			return false;
		}
		if (DoAxisSeparationTest(ref x, ref x4, ref x3, ref q, ref q2, ref q3, ref q4))
		{
			return false;
		}
		if (DoAxisSeparationTest(ref x4, ref x3, ref x, ref q, ref q2, ref q3, ref q4))
		{
			return false;
		}
		if (DoAxisSeparationTest(ref x3, ref x2, ref x, ref q, ref q2, ref q3, ref q4))
		{
			return false;
		}
		if (DoAxisSeparationTest(ref q, ref q2, ref q3, ref x, ref x2, ref x3, ref x4))
		{
			return false;
		}
		if (DoAxisSeparationTest(ref q, ref q4, ref q3, ref x, ref x2, ref x3, ref x4))
		{
			return false;
		}
		if (DoAxisSeparationTest(ref q4, ref q3, ref q, ref x, ref x2, ref x3, ref x4))
		{
			return false;
		}
		if (DoAxisSeparationTest(ref q3, ref q2, ref q, ref x, ref x2, ref x3, ref x4))
		{
			return false;
		}
		return true;
	}

	private static bool DoAxisSeparationTest(ref Vector2 x1, ref Vector2 x2, ref Vector2 x3, ref Vector2 q1, ref Vector2 q2, ref Vector2 q3, ref Vector2 q4)
	{
		Vector2 vector = x2 - x1;
		Vector2 vector2 = new Vector2(0f - vector.Y, vector.X);
		bool flag = vector2.X * (x3.X - x1.X) + vector2.Y * (x3.Y - x1.Y) >= 0f;
		bool flag2 = vector2.X * (q1.X - x1.X) + vector2.Y * (q1.Y - x1.Y) >= 0f;
		if (flag2 == flag)
		{
			return false;
		}
		flag2 = vector2.X * (q2.X - x1.X) + vector2.Y * (q2.Y - x1.Y) >= 0f;
		if (flag2 == flag)
		{
			return false;
		}
		flag2 = vector2.X * (q3.X - x1.X) + vector2.Y * (q3.Y - x1.Y) >= 0f;
		if (flag2 == flag)
		{
			return false;
		}
		flag2 = vector2.X * (q4.X - x1.X) + vector2.Y * (q4.Y - x1.Y) >= 0f;
		if (flag2 == flag)
		{
			return false;
		}
		return true;
	}

	public static bool LineIntersectsRectangle(ref Vector2 l1, ref Vector2 l2, ref Vector2 rectanglePos, ref Vector2 rectangleSize, float rectangleRotation, out float fraction)
	{
		fraction = 0f;
		Vector2 v = l1 - rectanglePos;
		Vector2 v2 = l2 - rectanglePos;
		RotateVector(ref v, 0f - rectangleRotation, out v);
		RotateVector(ref v2, 0f - rectangleRotation, out v2);
		v += rectanglePos;
		v2 += rectanglePos;
		float num = float.MinValue;
		float num2 = float.MaxValue;
		Vector2 vector = v;
		Vector2 v3 = v2 - v;
		Vector2 vector2 = VectorAbs(v3);
		Vector2 vector3 = rectanglePos - rectangleSize * 0.5f;
		Vector2 vector4 = rectanglePos + rectangleSize * 0.5f;
		_ = Vector2.Zero;
		for (int i = 0; i < 2; i++)
		{
			float num3 = ((i == 0) ? vector2.X : vector2.Y);
			float num4 = ((i == 0) ? vector3.X : vector3.Y);
			float num5 = ((i == 0) ? vector4.X : vector4.Y);
			float num6 = ((i == 0) ? vector.X : vector.Y);
			if (num3 < 1E-05f)
			{
				if (num6 < num4 || num5 < num6)
				{
					return false;
				}
				continue;
			}
			float num7 = ((i == 0) ? v3.X : v3.Y);
			float num8 = 1f / num7;
			float num9 = (num4 - num6) * num8;
			float num10 = (num5 - num6) * num8;
			float num11 = -1f;
			if (num9 > num10)
			{
				float num12 = num10;
				num10 = num9;
				num9 = num12;
				num11 = 1f;
			}
			if (num9 > num)
			{
				num = num9;
			}
			num2 = Math.Min(num2, num10);
			if (num > num2)
			{
				return false;
			}
		}
		if (num < 0f || num > 1f)
		{
			return false;
		}
		fraction = num;
		return true;
	}

	public static bool LineIntersectsLine(ref Vector2 l1begin, ref Vector2 l1end, ref Vector2 l2begin, ref Vector2 l2end, out Vector2 point)
	{
		float num = (l2end.X - l2begin.X) * (l1begin.Y - l2begin.Y) - (l2end.Y - l2begin.Y) * (l1begin.X - l2begin.X);
		float num2 = (l1end.X - l1begin.X) * (l1begin.Y - l2begin.Y) - (l1end.Y - l1begin.Y) * (l1begin.X - l2begin.X);
		float num3 = (l2end.Y - l2begin.Y) * (l1end.X - l1begin.X) - (l2end.X - l2begin.X) * (l1end.Y - l1begin.Y);
		if (Math.Abs(num3) <= 1E-05f)
		{
			if (Math.Abs(num) <= 1E-05f && Math.Abs(num2) <= 1E-05f)
			{
				point = (l1begin + l1end) / 2f;
				return true;
			}
		}
		else
		{
			num /= num3;
			num2 /= num3;
			if (num >= 0f && num <= 1f && num2 >= 0f && num2 <= 1f)
			{
				point = new Vector2(l1begin.X + num * (l1end.X - l1begin.X), l1begin.Y + num * (l1end.Y - l1begin.Y));
				return true;
			}
		}
		point = Vector2.Zero;
		return false;
	}

	public static void Shuffle<T>(this List<T> list)
	{
		int count = list.Count;
		for (int i = 0; i < count - 1; i++)
		{
			int num = Random.Next(count - i);
			T value = list[i + num];
			list[i + num] = list[i];
			list[i] = value;
		}
	}

	public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent)
	{
		normal.Normalize();
		Vector3 vector = normal * Vector3.Dot(tangent, normal);
		tangent -= vector;
		tangent.Normalize();
	}

	public static Quaternion CreateQuaternionFromAxis(ref Vector3 right, ref Vector3 up, ref Vector3 forward)
	{
		float num = right.X + up.Y + forward.Z;
		Quaternion result = default(Quaternion);
		if (num > 0f)
		{
			float num2 = (float)Math.Sqrt(num + 1f);
			result.W = num2 * 0.5f;
			num2 = 0.5f / num2;
			result.X = (up.Z - forward.Y) * num2;
			result.Y = (forward.X - right.Z) * num2;
			result.Z = (right.Y - up.X) * num2;
			return result;
		}
		if (right.X >= up.Y && right.X >= forward.Z)
		{
			float num3 = (float)Math.Sqrt(1f + right.X - up.Y - forward.Z);
			float num4 = 0.5f / num3;
			result.X = 0.5f * num3;
			result.Y = (right.Y + up.X) * num4;
			result.Z = (right.Z + forward.X) * num4;
			result.W = (up.Z - forward.Y) * num4;
			return result;
		}
		if (up.Y > forward.Z)
		{
			float num5 = (float)Math.Sqrt(1f + up.Y - right.X - forward.Z);
			float num6 = 0.5f / num5;
			result.X = (up.X + right.Y) * num6;
			result.Y = 0.5f * num5;
			result.Z = (forward.Y + up.Z) * num6;
			result.W = (forward.X - right.Z) * num6;
			return result;
		}
		float num7 = (float)Math.Sqrt(1f + forward.Z - right.X - up.Y);
		float num8 = 0.5f / num7;
		result.X = (forward.X + right.Z) * num8;
		result.Y = (forward.Y + up.Z) * num8;
		result.Z = 0.5f * num7;
		result.W = (right.Y - up.X) * num8;
		return result;
	}

	public static Quaternion CreateLookAtQuaternion(Vector3 forward, Vector3 up)
	{
		forward.Normalize();
		Vector3.Cross(ref up, ref forward, out var result);
		result.Normalize();
		Vector3.Cross(ref forward, ref result, out up);
		return CreateQuaternionFromAxis(ref result, ref up, ref forward);
	}
}
