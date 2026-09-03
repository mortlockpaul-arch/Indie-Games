using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Core;

public class Language
{
	protected string m_xml_path = "";

	protected XDocument m_xml_doc;

	protected Game m_game;

	protected Dictionary<string, string> m_strings = new Dictionary<string, string>();

	public Language(Game game, string xml_path)
	{
		try
		{
			m_game = game;
			m_xml_path = xml_path;
			LoadXML();
			if (m_xml_doc == null)
			{
				return;
			}
			XNode xNode = m_xml_doc.Root.FirstNode;
			XElement xElement = null;
			while (xNode != null)
			{
				if ((object)xNode.GetType() == typeof(XElement))
				{
					xElement = (XElement)xNode;
					parseElement(xElement);
				}
				xNode = xNode.NextNode;
			}
			xElement = null;
			xNode = null;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public virtual void Clear()
	{
		try
		{
			m_xml_doc = null;
			m_game = null;
			if (m_strings != null)
			{
				m_strings.Clear();
				m_strings = null;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected void LoadXML()
	{
		if (m_xml_path != "")
		{
			m_xml_doc = XDocument.Load(m_xml_path + ".xml");
		}
	}

	protected virtual bool parseElement(XElement element)
	{
		try
		{
			if (element == null)
			{
				return false;
			}
			string text;
			if ((text = element.Name.ToString()) != null && text == "String")
			{
				return parseString(element);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return false;
	}

	protected virtual bool parseString(XElement element)
	{
		try
		{
			if (!element.HasAttributes)
			{
				return false;
			}
			string text = "";
			string text2 = "";
			XAttribute xAttribute = element.Attribute("id");
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			xAttribute = element.Attribute("text");
			if (xAttribute == null)
			{
				return false;
			}
			text2 = xAttribute.Value;
			m_strings.Add(text, text2);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return false;
	}

	public virtual string GetString(string id)
	{
		try
		{
			if (m_strings == null)
			{
				return id;
			}
			if (m_strings.ContainsKey(id))
			{
				return m_strings[id];
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return id;
	}
}
