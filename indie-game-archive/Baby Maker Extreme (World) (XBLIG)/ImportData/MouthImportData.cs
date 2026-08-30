using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace ImportData;

public class MouthImportData
{
	private List<List<MouthAnimImportData>> m_anims;

	public MouthImportData(XmlReader reader)
	{
		m_anims = new List<List<MouthAnimImportData>>();
		reader.ReadStartElement("root");
		while (reader.IsStartElement() && reader.Name.Equals("anim"))
		{
			m_anims.Add(new List<MouthAnimImportData>());
			reader.ReadStartElement("anim");
			while (reader.IsStartElement() && reader.Name.Equals("p"))
			{
				reader.ReadStartElement("p");
				int t = int.Parse(reader.ReadElementString("time"));
				int i = int.Parse(reader.ReadElementString("i"));
				reader.ReadEndElement();
				m_anims.Last().Add(new MouthAnimImportData(t, i));
			}
			reader.ReadEndElement();
		}
	}

	public MouthImportData(ContentReader input)
	{
		m_anims = new List<List<MouthAnimImportData>>();
		int num = ((BinaryReader)(object)input).ReadInt32();
		for (int i = 0; i < num; i++)
		{
			m_anims.Add(new List<MouthAnimImportData>());
			int num2 = ((BinaryReader)(object)input).ReadInt32();
			for (int j = 0; j < num2; j++)
			{
				m_anims.Last().Add(new MouthAnimImportData(((BinaryReader)(object)input).ReadInt32(), ((BinaryReader)(object)input).ReadInt32()));
			}
		}
	}

	public List<List<MouthAnimImportData>> GetData()
	{
		return m_anims;
	}
}
