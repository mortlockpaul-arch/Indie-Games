using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Quasar.Global;

public static class XDocHelper
{
	public static string GetAttribute(this XElement xe, string strAttr, string defaultValue)
	{
		if (xe == null)
		{
			return defaultValue;
		}
		XAttribute xAttribute = xe.Attribute(strAttr);
		if (xAttribute != null)
		{
			return xAttribute.Value;
		}
		return defaultValue;
	}

	public static bool HasAttribute(this XElement xe, string strAttr)
	{
		if (xe == null)
		{
			return false;
		}
		XAttribute xAttribute = xe.Attribute(strAttr);
		return xAttribute != null;
	}

	public static void SetAttribute(this XElement xe, string attrName, string attrValue)
	{
		xe?.SetAttributeValue(attrName, attrValue);
	}

	public static void SetIntAttribute(this XElement xe, string attrName, int attrValue)
	{
		SetAttribute(xe, attrName, GameMath.IntToString(attrValue));
	}

	public static void SetInt2Attribute(this XElement xe, string attrName, Int2 attrValue)
	{
		SetAttribute(xe, attrName, GameMath.Int2ToString(attrValue));
	}

	public static void SetIntListAttribute(this XElement xe, string attrName, List<int> attrValue)
	{
		SetAttribute(xe, attrName, GameMath.IntListToString(attrValue));
	}

	public static void SetLongAttribute(this XElement xe, string attrName, long attrValue)
	{
		SetAttribute(xe, attrName, GameMath.LongToString(attrValue));
	}

	public static void SetUIntAttribute(this XElement xe, string attrName, uint attrValue)
	{
		SetAttribute(xe, attrName, GameMath.UIntToString(attrValue));
	}

	public static void SetVector2Attribute(this XElement xe, string attrName, Vector2 attrValue)
	{
		SetAttribute(xe, attrName, GameMath.FloatToString(attrValue.X) + "," + GameMath.FloatToString(attrValue.Y));
	}

	public static void SetVector3Attribute(this XElement xe, string attrName, Vector3 attrValue)
	{
		SetAttribute(xe, attrName, GameMath.FloatToString(attrValue.X) + "," + GameMath.FloatToString(attrValue.Y) + "," + GameMath.FloatToString(attrValue.Z));
	}

	public static void SetVector2ListAttribute(this XElement xe, string attrName, List<Vector2> attrValue)
	{
		SetAttribute(xe, attrName, GameMath.VectorListToString(attrValue));
	}

	public static void SetFloatAttribute(this XElement xe, string attrName, float attrValue)
	{
		SetAttribute(xe, attrName, attrValue.ToString(CultureInfo.InvariantCulture.NumberFormat));
	}

	public static void SetDateAttribute(this XElement xe, string attrName, DateTime attrValue)
	{
		SetAttribute(xe, attrName, GameMath.DateToString(attrValue));
	}

	public static void SetBoolAttribute(this XElement xe, string attrName, bool attrValue)
	{
		SetAttribute(xe, attrName, GameMath.BoolToString(attrValue));
	}

	public static string GetAttribute(this XElement xe, string strAttr)
	{
		return GetAttribute(xe, strAttr, "");
	}

	public static int ParseIntAttribute(this XElement xe, string strAttr)
	{
		return ParseIntAttribute(xe, strAttr, 0);
	}

	public static int ParseIntAttribute(this XElement xe, string strAttr, int defaultValue)
	{
		string attribute = GetAttribute(xe, strAttr, "");
		return GameMath.ParseInt(attribute, defaultValue);
	}

	public static uint ParseUIntAttribute(this XElement xe, string strAttr)
	{
		return ParseUIntAttribute(xe, strAttr, 0u);
	}

	public static uint ParseUIntAttribute(this XElement xe, string strAttr, uint defaultValue)
	{
		string attribute = GetAttribute(xe, strAttr, "");
		return GameMath.ParseUInt(attribute, defaultValue);
	}

	public static long ParseLongAttribute(this XElement xe, string strAttr)
	{
		return ParseLongAttribute(xe, strAttr, 0L);
	}

	public static long ParseLongAttribute(this XElement xe, string strAttr, long defaultValue)
	{
		string attribute = GetAttribute(xe, strAttr, "");
		return GameMath.ParseLong(attribute, defaultValue);
	}

	public static float ParseFloatAttribute(this XElement xe, string strAttr)
	{
		return ParseFloatAttribute(xe, strAttr, 0f);
	}

	public static float ParseFloatAttribute(this XElement xe, string strAttr, float defaultValue)
	{
		string attribute = GetAttribute(xe, strAttr, "");
		return GameMath.ParseFloat(attribute, defaultValue);
	}

	public static List<Vector2> ParseVector2ListAttribute(this XElement xe, string attr)
	{
		return GameMath.ParseVector2List(GetAttribute(xe, attr));
	}

	public static void ParseVector2ListAttribute(this XElement xe, string attr, List<Vector2> destination, bool emptyList)
	{
		GameMath.ParseVector2List(GetAttribute(xe, attr), destination, emptyList);
	}

	public static List<int> ParseIntListAttribute(this XElement xe, string attr)
	{
		return GameMath.ParseIntList(GetAttribute(xe, attr));
	}

	public static void ParseIntListAttribute(this XElement xe, string attr, List<int> destination, bool emptyList)
	{
		GameMath.ParseIntList(GetAttribute(xe, attr), destination, emptyList);
	}

	public static List<float> ParseFloatListAttribute(this XElement xe, string attr)
	{
		return GameMath.ParseFloatList(GetAttribute(xe, attr));
	}

	public static void ParseFloatListAttribute(this XElement xe, string attr, List<float> destination, bool emptyList)
	{
		GameMath.ParseFloatList(GetAttribute(xe, attr), destination, emptyList);
	}

	public static Vector2 ParseVector2Attribute(this XElement xe, string attr)
	{
		return GameMath.ParseVector2(GetAttribute(xe, attr));
	}

	public static Int2 ParseInt2Attribute(this XElement xe, string attr)
	{
		return GameMath.ParseInt2(GetAttribute(xe, attr));
	}

	public static Int2 ParseInt2Attribute(this XElement xe, string attr, Int2 defaultValue)
	{
		return GameMath.ParseInt2(GetAttribute(xe, attr), defaultValue);
	}

	public static Vector3 ParseVector3Attribute(this XElement xe, string attr)
	{
		return GameMath.ParseVector3(GetAttribute(xe, attr));
	}

	public static Vector4 ParseVector4Attribute(this XElement xe, string attr)
	{
		return GameMath.ParseVector4(GetAttribute(xe, attr));
	}

	public static Vector2 ParseVector2Attribute(this XElement xe, string attr, Vector2 defaultValue)
	{
		return GameMath.ParseVector2(GetAttribute(xe, attr), defaultValue);
	}

	public static Vector3 ParseVector3Attribute(this XElement xe, string attr, Vector3 defaultValue)
	{
		return GameMath.ParseVector3(GetAttribute(xe, attr), defaultValue);
	}

	public static Vector4 ParseVector4Attribute(this XElement xe, string attr, Vector4 defaultValue)
	{
		return GameMath.ParseVector4(GetAttribute(xe, attr), defaultValue);
	}

	public static Vector2 ParseVector2Attribute(this XElement xe, string xAttr, string yAttr)
	{
		return GameMath.ParseVector2(GetAttribute(xe, xAttr), GetAttribute(xe, yAttr));
	}

	public static Vector2 ParseVector2Attribute(this XElement xe, string xAttr, string yAttr, Vector2 defaultValue)
	{
		return GameMath.ParseVector2(GetAttribute(xe, xAttr), GetAttribute(xe, yAttr), defaultValue);
	}

	public static Vector3 ParseVector3Attribute(this XElement xe, string xAttr, string yAttr, string zAttr)
	{
		return GameMath.ParseVector3(GetAttribute(xe, xAttr), GetAttribute(xe, yAttr), GetAttribute(xe, zAttr));
	}

	public static Vector3 ParseVector3Attribute(this XElement xe, string xAttr, string yAttr, string zAttr, Vector3 defaultValue)
	{
		return GameMath.ParseVector3(GetAttribute(xe, xAttr), GetAttribute(xe, yAttr), GetAttribute(xe, zAttr), defaultValue);
	}

	public static Vector4 ParseColorAttribute(this XElement xe, string attr)
	{
		return GameMath.ParseColor(GetAttribute(xe, attr));
	}

	public static Vector4 ParseColorAttribute(this XElement xe, string attr, Vector4 defaultValue)
	{
		return GameMath.ParseColor(GetAttribute(xe, attr), defaultValue);
	}

	public static Vector3 ParseColor3Attribute(this XElement xe, string attr)
	{
		return GameMath.ParseColor3(GetAttribute(xe, attr));
	}

	public static Vector3 ParseColor3Attribute(this XElement xe, string attr, Vector3 defaultValue)
	{
		return GameMath.ParseColor3(GetAttribute(xe, attr), defaultValue);
	}

	public static bool ParseBoolAttribute(this XElement xe, string attr)
	{
		return ParseBoolAttribute(xe, attr, defaultValue: false);
	}

	public static bool ParseBoolAttribute(this XElement xe, string attr, bool defaultValue)
	{
		string attribute = GetAttribute(xe, attr);
		return GameMath.ParseBool(attribute, defaultValue);
	}

	public static DateTime ParseDateAttribute(this XElement xe, string attr)
	{
		string attribute = GetAttribute(xe, attr);
		DateTime date = default(DateTime);
		GameMath.ParseDate(attribute, ref date);
		return date;
	}

	public static DateTime ParseDateAttribute(this XElement xe, string attr, DateTime defaultValue)
	{
		string attribute = GetAttribute(xe, attr);
		DateTime date = default(DateTime);
		GameMath.ParseDate(attribute, ref date, ref defaultValue);
		return date;
	}
}
