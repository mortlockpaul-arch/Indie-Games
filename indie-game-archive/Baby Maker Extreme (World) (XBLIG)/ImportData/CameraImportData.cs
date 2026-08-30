using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace ImportData;

public class CameraImportData
{
	private List<CamActionImportData> m_actions;

	public CameraImportData(XmlReader reader)
	{
		m_actions = new List<CamActionImportData>();
		while (reader.IsStartElement() && reader.Name.Equals("cameraAction"))
		{
			m_actions.Add(new CamActionImportData(reader));
		}
	}

	public CameraImportData(ContentReader input)
	{
		m_actions = new List<CamActionImportData>();
		int num = ((BinaryReader)(object)input).ReadInt32();
		for (int i = 0; i < num; i++)
		{
			m_actions.Add(new CamActionImportData(input));
		}
	}

	public List<CamActionImportData> GetActions()
	{
		return m_actions;
	}
}
