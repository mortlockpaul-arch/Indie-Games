using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace ImportData;

public class CutsceneImportData
{
	private List<PropImportData> m_props;

	private List<TextImportData> m_texts;

	private CameraImportData m_camera;

	public CutsceneImportData(XmlReader reader)
	{
		m_props = new List<PropImportData>();
		m_texts = new List<TextImportData>();
		reader.ReadStartElement("root");
		while (reader.IsStartElement() && reader.Name.Equals("prop"))
		{
			m_props.Add(new PropImportData(reader));
		}
		m_camera = new CameraImportData(reader);
		while (reader.IsStartElement() && reader.Name.Equals("textPopup"))
		{
			m_texts.Add(new TextImportData(reader));
		}
	}

	public CutsceneImportData(ContentReader input)
	{
		m_props = new List<PropImportData>();
		m_texts = new List<TextImportData>();
		int num = ((BinaryReader)(object)input).ReadInt32();
		for (int i = 0; i < num; i++)
		{
			m_props.Add(new PropImportData(input));
		}
		m_camera = new CameraImportData(input);
		int num2 = ((BinaryReader)(object)input).ReadInt32();
		for (int j = 0; j < num2; j++)
		{
			m_texts.Add(new TextImportData(input));
		}
	}

	public List<PropImportData> GetProps()
	{
		return m_props;
	}

	public CameraImportData GetCamera()
	{
		return m_camera;
	}

	public List<TextImportData> GetTextData()
	{
		return m_texts;
	}
}
