using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.Utils;

public static class KeyedValue
{
	public sealed class Vector3Parser : IKeyedValueParser<Vector3>
	{
		public void Lerp(ref Vector3 from, ref Vector3 to, float value, out Vector3 result)
		{
			result = new Vector3(to.X * value + from.X * (1f - value), to.Y * value + from.Y * (1f - value), to.Z * value + from.Z * (1f - value));
		}

		public Vector3 Parse(string field)
		{
			return GameMath.ParseVector3(field);
		}
	}

	public sealed class FloatParser : IKeyedValueParser<float>
	{
		public void Lerp(ref float from, ref float to, float value, out float result)
		{
			result = to * value + from * (1f - value);
		}

		public float Parse(string field)
		{
			return GameMath.ParseFloat(field);
		}
	}

	public static Vector3Parser _Vector3Parser = new Vector3Parser();

	public static FloatParser _FloatParser = new FloatParser();

	public static KeyedValue<Vector3> ParseXmlVector3(XElement xe, string field)
	{
		return ParseXml(xe, field, _Vector3Parser);
	}

	public static KeyedValue<Vector3> Create(Vector3 value, int keyCount)
	{
		List<KeyValuePair<float, Vector3>> list = new List<KeyValuePair<float, Vector3>>();
		float num = 0f;
		int num2 = keyCount;
		if (num2 < 2)
		{
			num2 = 2;
		}
		float num3 = 1f / (float)(num2 - 1);
		for (int i = 0; i < keyCount; i++)
		{
			list.Add(new KeyValuePair<float, Vector3>(num, value));
			num += num3;
		}
		return new KeyedValue<Vector3>(list, _Vector3Parser);
	}

	public static KeyedValue<float> ParseXmlFloat(XElement xe, string field)
	{
		return ParseXml(xe, field, _FloatParser);
	}

	public static KeyedValue<float> Create(float value, int keyCount)
	{
		List<KeyValuePair<float, float>> list = new List<KeyValuePair<float, float>>();
		float num = 0f;
		int num2 = keyCount;
		if (num2 < 2)
		{
			num2 = 2;
		}
		float num3 = 1f / (float)(num2 - 1);
		for (int i = 0; i < keyCount; i++)
		{
			list.Add(new KeyValuePair<float, float>(num, value));
			num += num3;
		}
		return new KeyedValue<float>(list, _FloatParser);
	}

	private static KeyedValue<T> ParseXml<T>(XElement xe, string field, IKeyedValueParser<T> parser) where T : struct
	{
		XElement xElement = xe.Element(field);
		List<KeyValuePair<float, T>> list = new List<KeyValuePair<float, T>>();
		foreach (XElement item in xElement.Elements("Key"))
		{
			list.Add(new KeyValuePair<float, T>(XDocHelper.ParseFloatAttribute(item, "time"), parser.Parse(XDocHelper.GetAttribute(item, "value"))));
		}
		return new KeyedValue<T>(list, parser);
	}
}
public class KeyedValue<T> : IComparer<KeyValuePair<float, T>> where T : struct
{
	private float[] keys;

	private T[] values;

	private IKeyedValueParser<T> parser;

	private int keyCount;

	public int KeyCount => keys.Length;

	public KeyedValue(List<KeyValuePair<float, T>> keys, IKeyedValueParser<T> parser)
	{
		keys.Sort(this);
		keyCount = keys.Count;
		this.keys = new float[keyCount];
		values = new T[keyCount];
		for (int i = 0; i < keyCount; i++)
		{
			this.keys[i] = keys[i].Key;
			values[i] = keys[i].Value;
		}
		this.parser = parser;
	}

	public float GetKey(int index)
	{
		if (index >= 0 && index < KeyCount)
		{
			return keys[index];
		}
		return -1f;
	}

	public void SetKey(int index, float value)
	{
		int num = index - 1;
		int num2 = index + 1;
		if (num >= 0 && GetKey(num) <= value && num2 < KeyCount && value <= GetKey(num2))
		{
			keys[index] = value;
		}
	}

	public void SetValue(int index, T value)
	{
		if (index >= 0 && index < KeyCount)
		{
			values[index] = value;
		}
	}

	public void GetValue(float time, out T result)
	{
		float num = 0f;
		for (int i = 0; i < keyCount; i++)
		{
			float num2 = keys[i];
			if (num2 >= time)
			{
				if (i == 0)
				{
					result = values[0];
					return;
				}
				float value = (time - num) / (num2 - num);
				parser.Lerp(ref values[i - 1], ref values[i], value, out result);
				return;
			}
			num = num2;
		}
		result = values[keyCount - 1];
	}

	public int Compare(KeyValuePair<float, T> x, KeyValuePair<float, T> y)
	{
		return x.Key.CompareTo(y.Key);
	}

	public override string ToString()
	{
		return keys.Length + " keys";
	}
}
