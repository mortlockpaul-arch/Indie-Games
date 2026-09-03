using System.Xml.Linq;

namespace Core;

public class ScriptCheck
{
	public string m_type1 = "";

	public string m_value1 = "";

	public string m_rule = "";

	public string m_type2 = "";

	public string m_value2 = "";

	public string m_nextCheck = "";

	public ScriptCheck(XElement element)
	{
		if (element == null)
		{
			return;
		}
		XAttribute xAttribute = element.Attribute("type1");
		if (xAttribute == null)
		{
			return;
		}
		m_type1 = xAttribute.Value;
		xAttribute = element.Attribute("value1");
		if (xAttribute == null)
		{
			return;
		}
		m_value1 = xAttribute.Value;
		xAttribute = element.Attribute("rule");
		if (xAttribute == null)
		{
			return;
		}
		m_rule = xAttribute.Value;
		xAttribute = element.Attribute("type2");
		if (xAttribute == null)
		{
			return;
		}
		m_type2 = xAttribute.Value;
		xAttribute = element.Attribute("value2");
		if (xAttribute != null)
		{
			m_value2 = xAttribute.Value;
			xAttribute = element.Attribute("nextCheck");
			if (xAttribute != null)
			{
				m_nextCheck = xAttribute.Value.ToUpper();
			}
		}
	}

	public bool Validate(Game game, ScriptObject script_object)
	{
		string typeValue = getTypeValue(game, m_type1, m_value1, script_object);
		string typeValue2 = getTypeValue(game, m_type2, m_value2, script_object);
		if (m_rule == "==" && typeValue != typeValue2)
		{
			return false;
		}
		if (m_rule == "!=" && typeValue == typeValue2)
		{
			return false;
		}
		if (m_type2.ToLowerInvariant() == "number")
		{
			float num = ScriptObject.ParseFloatValue(typeValue);
			float num2 = ScriptObject.ParseFloatValue(typeValue2);
			if (m_rule.ToLowerInvariant() == "lower")
			{
				return num < num2;
			}
			if (m_rule.ToLowerInvariant() == "lower_equal")
			{
				return num <= num2;
			}
			if (m_rule.ToLowerInvariant() == "higher")
			{
				return num > num2;
			}
			if (m_rule.ToLowerInvariant() == "higher_equal")
			{
				return num >= num2;
			}
		}
		return true;
	}

	protected string getTypeValue(Game game, string type, string value, ScriptObject script_object)
	{
		switch (type)
		{
		case "GetState":
		case "GlobalState":
			return game.m_game_data.GetState(value);
		case "LocalState":
			return script_object.GetLocalState(value);
		default:
			return value;
		}
	}
}
