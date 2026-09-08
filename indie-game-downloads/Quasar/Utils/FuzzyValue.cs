using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.Utils;

public static class FuzzyValue
{
	private sealed class Vector3Parser : IFuzzyValueParser<Vector3>
	{
		public Vector3 DefaultValue => Vector3.Zero;

		public Vector3 Next(Vector3 baseVal, Vector3 rndVal)
		{
			return baseVal + new Vector3(rndVal.X * GameMath.Random.NextFloat(-1f, 1f), rndVal.Y * GameMath.Random.NextFloat(-1f, 1f), rndVal.Z * GameMath.Random.NextFloat(-1f, 1f));
		}

		public Vector3 Parse(string field)
		{
			return GameMath.ParseVector3(field);
		}

		public bool NotEmpty(Vector3 value)
		{
			return value != Vector3.Zero;
		}
	}

	private sealed class FloatParser : IFuzzyValueParser<float>
	{
		public float DefaultValue => 0f;

		public float Next(float baseVal, float rndVal)
		{
			return baseVal + rndVal * GameMath.Random.NextFloat(-1f, 1f);
		}

		public float Lerp(float from, float to, float value)
		{
			return MathHelper.Lerp(from, to, value);
		}

		public float Parse(string field)
		{
			return GameMath.ParseFloat(field);
		}

		public bool NotEmpty(float value)
		{
			return value != 0f;
		}
	}

	private static Vector3Parser _Vector3Parser = new Vector3Parser();

	private static FloatParser _FloatParser = new FloatParser();

	public static FuzzyValue<Vector3> ParseXmlVector3(XElement xe, string field)
	{
		return ParseXml(xe, field, _Vector3Parser);
	}

	public static FuzzyValue<Vector3> Create(Vector3 baseVal, Vector3 randomVal)
	{
		return new FuzzyValue<Vector3>(baseVal, randomVal, _Vector3Parser);
	}

	public static FuzzyValue<float> ParseXmlFloat(XElement xe, string field)
	{
		return ParseXml(xe, field, _FloatParser);
	}

	public static FuzzyValue<float> Create(float baseVal, float randomVal)
	{
		return new FuzzyValue<float>(baseVal, randomVal, _FloatParser);
	}

	private static FuzzyValue<T> ParseXml<T>(XElement xe, string field, IFuzzyValueParser<T> parser) where T : struct
	{
		XElement xe2 = xe.Element(field);
		return new FuzzyValue<T>(parser.Parse(XDocHelper.GetAttribute(xe2, "base")), parser.Parse(XDocHelper.GetAttribute(xe2, "random")), parser);
	}
}
public class FuzzyValue<T> where T : struct
{
	private T baseVal;

	private T rndVal;

	private bool valid;

	private IFuzzyValueParser<T> parser;

	public T BaseVal
	{
		get
		{
			return baseVal;
		}
		set
		{
			baseVal = value;
			updateValid();
		}
	}

	public T RndVal
	{
		get
		{
			return rndVal;
		}
		set
		{
			rndVal = value;
			updateValid();
		}
	}

	public bool Valid => valid;

	protected void updateValid()
	{
		valid = parser.NotEmpty(baseVal) || parser.NotEmpty(rndVal);
	}

	public FuzzyValue(T baseVal, T rndVal, IFuzzyValueParser<T> parser)
	{
		this.parser = parser;
		this.baseVal = baseVal;
		this.rndVal = rndVal;
		updateValid();
	}

	public T Next()
	{
		if (!valid)
		{
			return parser.DefaultValue;
		}
		return parser.Next(baseVal, rndVal);
	}

	public FuzzyValue<T> Clone()
	{
		return new FuzzyValue<T>(baseVal, rndVal, parser);
	}
}
