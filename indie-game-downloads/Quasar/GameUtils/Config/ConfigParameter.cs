using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.GameUtils.Config;

public static class ConfigParameter
{
	private sealed class StringParser : IValueParser<string>
	{
		public string Parse(XElement xe, string defaultValue)
		{
			return xe.GetAttribute("value");
		}

		public void ToXml(XElement xe, string value)
		{
			xe.SetAttributeValue("value", value);
		}
	}

	private sealed class IntParser : IValueParser<int>
	{
		public int Parse(XElement xe, int defaultValue)
		{
			return GameMath.ParseInt(xe.GetAttribute("value"), defaultValue);
		}

		public void ToXml(XElement xe, int value)
		{
			xe.SetAttributeValue("value", GameMath.IntToString(value));
		}
	}

	private sealed class FloatParser : IValueParser<float>
	{
		public float Parse(XElement xe, float defaultValue)
		{
			return GameMath.ParseFloat(xe.GetAttribute("value"), defaultValue);
		}

		public void ToXml(XElement xe, float value)
		{
			xe.SetAttributeValue("value", GameMath.FloatToString(value));
		}
	}

	private sealed class BoolParser : IValueParser<bool>
	{
		public bool Parse(XElement xe, bool defaultValue)
		{
			return GameMath.ParseBool(xe.GetAttribute("value"), defaultValue);
		}

		public void ToXml(XElement xe, bool value)
		{
			xe.SetAttributeValue("value", GameMath.BoolToString(value));
		}
	}

	private sealed class Vector2Parser : IValueParser<Vector2>
	{
		public Vector2 Parse(XElement xe, Vector2 defaultValue)
		{
			return GameMath.ParseVector2(xe.GetAttribute("value"), defaultValue);
		}

		public void ToXml(XElement xe, Vector2 value)
		{
			xe.SetAttributeValue("value", GameMath.VectorToString(value));
		}
	}

	private static StringParser _StringParser = new StringParser();

	private static IntParser _IntParser = new IntParser();

	private static FloatParser _FloatParser = new FloatParser();

	private static BoolParser _BoolParser = new BoolParser();

	private static Vector2Parser _Vector2Parser = new Vector2Parser();

	public static ConfigParameter<int> CreateIntParameter(string name, int defaultValue)
	{
		return new ConfigParameter<int>(name, defaultValue, _IntParser);
	}

	public static ConfigParameter<Vector2> CreateVector2Parameter(string name, Vector2 defaultValue)
	{
		return new ConfigParameter<Vector2>(name, defaultValue, _Vector2Parser);
	}

	public static ConfigParameter<float> CreateFloatParameter(string name, float defaultValue)
	{
		return new ConfigParameter<float>(name, defaultValue, _FloatParser);
	}

	public static ConfigParameter<bool> CreateBoolParameter(string name, bool defaultValue)
	{
		return new ConfigParameter<bool>(name, defaultValue, _BoolParser);
	}

	public static ConfigParameter<string> CreateStringParameter(string name, string defaultValue)
	{
		return new ConfigParameter<string>(name, defaultValue, _StringParser);
	}
}
public class ConfigParameter<T> : IConfigItem
{
	private T value;

	private T defaultValue;

	private IValueParser<T> parser;

	private string name;

	public string Name => name;

	public T Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	public ConfigParameter(string name, T defaultValue, IValueParser<T> parser)
	{
		this.defaultValue = defaultValue;
		value = defaultValue;
		this.name = name;
		this.parser = parser;
	}

	public void Apply()
	{
	}

	public void SetDefaults()
	{
		value = defaultValue;
	}

	public void ToXml(XElement parent)
	{
		XElement xElement = new XElement(name);
		parser.ToXml(xElement, value);
		parent.Add(xElement);
	}

	public void FromXml(XElement parent)
	{
		XElement xElement = parent.Element(name);
		if (xElement != null)
		{
			value = parser.Parse(xElement, defaultValue);
		}
	}
}
