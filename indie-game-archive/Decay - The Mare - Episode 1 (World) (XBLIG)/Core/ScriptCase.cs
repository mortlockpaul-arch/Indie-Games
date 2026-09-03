using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Core;

public class ScriptCase
{
	private List<ScriptCheck> m_checks = new List<ScriptCheck>();

	private XElement m_true;

	private XElement m_false;

	public ScriptCase(ScriptObject script_object, XElement element)
	{
		if (script_object == null || element == null)
		{
			return;
		}
		ScriptCheck scriptCheck = null;
		XElement xElement = null;
		XNode xNode;
		for (xNode = element.FirstNode; xNode != null; xNode = xNode.NextNode)
		{
			if ((object)xNode.GetType() == typeof(XElement))
			{
				xElement = (XElement)xNode;
				if (xElement.Name == "Check")
				{
					scriptCheck = new ScriptCheck(xElement);
					if (scriptCheck != null)
					{
						m_checks.Add(scriptCheck);
					}
				}
				else if (xElement.Name == "True")
				{
					m_true = xElement;
				}
				else if (xElement.Name == "False")
				{
					m_false = xElement;
				}
			}
		}
		scriptCheck = null;
		xElement = null;
		xNode = null;
	}

	public void Clear()
	{
		if (m_checks != null)
		{
			m_checks.Clear();
			m_checks = null;
		}
		m_true = null;
		m_false = null;
	}

	public XNode Execute(Game game, ScriptObject script_object)
	{
		try
		{
			bool flag = false;
			for (int i = 0; i < m_checks.Count; i++)
			{
				if (!m_checks[i].Validate(game, script_object))
				{
					flag = false;
					if (m_checks[i].m_nextCheck != "OR")
					{
						break;
					}
				}
				else
				{
					flag = true;
					if (m_checks[i].m_nextCheck == "OR")
					{
						break;
					}
				}
			}
			if (!flag)
			{
				if (m_false == null)
				{
					script_object.ScriptError("Case element with no 'FALSE' node specified!");
					return null;
				}
				return m_false.FirstNode;
			}
			if (m_true == null)
			{
				script_object.ScriptError("Case element with no 'TRUE' node specified!");
				return null;
			}
			return m_true.FirstNode;
		}
		catch (Exception ex)
		{
			Console.Write(ex.Message);
		}
		return null;
	}
}
