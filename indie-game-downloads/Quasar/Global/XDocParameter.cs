using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Quasar.Global;

public static class XDocParameter
{
	public static string GetParameter(this XElement xe, string strAttr, string defaultValue)
	{
		if (xe == null)
		{
			return defaultValue;
		}
		XElement xe2 = xe.Element(strAttr);
		return XDocHelper.GetAttribute(xe2, "value", defaultValue);
	}

	public static string GetParameter(this XElement xe, string strAttr)
	{
		return GetParameter(xe, strAttr, "");
	}

	public static int ParseIntParameter(this XElement xe, string strAttr)
	{
		return ParseIntParameter(xe, strAttr, 0);
	}

	public static int ParseIntParameter(this XElement xe, string strAttr, int defaultValue)
	{
		string parameter = GetParameter(xe, strAttr, "");
		return GameMath.ParseInt(parameter, defaultValue);
	}

	public static long ParseLongParameter(this XElement xe, string strAttr)
	{
		return ParseLongParameter(xe, strAttr, 0L);
	}

	public static long ParseLongParameter(this XElement xe, string strAttr, long defaultValue)
	{
		string parameter = GetParameter(xe, strAttr, "");
		return GameMath.ParseLong(parameter, defaultValue);
	}

	public static float ParseFloatParameter(this XElement xe, string strAttr)
	{
		return ParseFloatParameter(xe, strAttr, 0f);
	}

	public static float ParseFloatParameter(this XElement xe, string strAttr, float defaultValue)
	{
		string parameter = GetParameter(xe, strAttr, "");
		return GameMath.ParseFloat(parameter, defaultValue);
	}

	public static List<Vector2> ParseVector2ListParameter(this XElement xe, string attr)
	{
		return GameMath.ParseVector2List(GetParameter(xe, attr));
	}

	public static List<int> ParseIntListParameter(this XElement xe, string attr)
	{
		return GameMath.ParseIntList(GetParameter(xe, attr));
	}

	public static Vector2 ParseVector2Parameter(this XElement xe, string attr)
	{
		return GameMath.ParseVector2(GetParameter(xe, attr));
	}

	public static Vector2 ParseVector2Parameter(this XElement xe, string xAttr, string yAttr)
	{
		return GameMath.ParseVector2(GetParameter(xe, xAttr), GetParameter(xe, yAttr));
	}

	public static Vector2 ParseVector2Parameter(this XElement xe, string xAttr, string yAttr, Vector2 defaultValue)
	{
		return GameMath.ParseVector2(GetParameter(xe, xAttr), GetParameter(xe, yAttr), defaultValue);
	}

	public static Vector3 ParseVector3Parameter(this XElement xe, string xAttr, string yAttr, string zAttr)
	{
		return GameMath.ParseVector3(GetParameter(xe, xAttr), GetParameter(xe, yAttr), GetParameter(xe, zAttr));
	}

	public static Vector3 ParseVector3Parameter(this XElement xe, string xAttr, string yAttr, string zAttr, Vector3 defaultValue)
	{
		return GameMath.ParseVector3(GetParameter(xe, xAttr), GetParameter(xe, yAttr), GetParameter(xe, zAttr), defaultValue);
	}

	public static Vector4 ParseColorParameter(this XElement xe, string attr)
	{
		return GameMath.ParseColor(GetParameter(xe, attr));
	}

	public static Vector4 ParseColorParameter(this XElement xe, string attr, Vector4 defaultValue)
	{
		return GameMath.ParseColor(GetParameter(xe, attr), defaultValue);
	}

	public static bool ParseBoolParameter(this XElement xe, string attr)
	{
		return ParseBoolParameter(xe, attr, defaultValue: false);
	}

	public static bool ParseBoolParameter(this XElement xe, string attr, bool defaultValue)
	{
		string parameter = GetParameter(xe, attr);
		return GameMath.ParseBool(parameter, defaultValue);
	}
}
