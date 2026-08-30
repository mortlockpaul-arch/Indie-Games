using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace ImportData;

public class PropImportData
{
	private List<PropActionImportData> m_actions;

	private string m_file;

	public string CharacterFile => m_file;

	public PropImportData(XmlReader reader)
	{
		m_actions = new List<PropActionImportData>();
		reader.ReadStartElement("prop");
		m_file = reader.ReadElementString("character");
		while (reader.IsStartElement() && reader.Name.Equals("action"))
		{
			m_actions.Add(new PropActionImportData(reader));
		}
		reader.ReadEndElement();
	}

	public PropImportData(ContentReader input)
	{
		m_actions = new List<PropActionImportData>();
		m_file = ((BinaryReader)(object)input).ReadString();
		int num = ((BinaryReader)(object)input).ReadInt32();
		for (int i = 0; i < num; i++)
		{
			m_actions.Add(new PropActionImportData(input));
		}
	}

	public List<PropActionImportData> GetActions()
	{
		return m_actions;
	}
}
