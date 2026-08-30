using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework.Content;

namespace ImportData;

public class SpawnerImportData
{
	private List<int> m_unitIDs;

	private List<int> m_spawnOffsets;

	private List<int> m_rows;

	private List<int> m_spawnTimes;

	public List<int> UnitIDs => m_unitIDs;

	public List<int> SpawnDelays => m_spawnOffsets;

	public List<int> Rows => m_rows;

	public List<int> SpawnTimes => m_spawnTimes;

	public SpawnerImportData(XmlReader reader)
	{
		m_unitIDs = new List<int>();
		m_spawnOffsets = new List<int>();
		m_rows = new List<int>();
		m_spawnTimes = new List<int>();
		reader.ReadStartElement("root");
		int num = 0;
		while (reader.Name.Equals("unitID"))
		{
			m_unitIDs.Add(int.Parse(reader.ReadElementString("unitID")));
			m_spawnOffsets.Add(int.Parse(reader.ReadElementString("spawnIn")));
			m_rows.Add(int.Parse(reader.ReadElementString("row")));
			num += m_spawnOffsets.Last();
			m_spawnTimes.Add(num);
		}
		reader.ReadEndElement();
	}

	public SpawnerImportData(ContentReader reader)
	{
		m_rows = new List<int>();
		m_unitIDs = new List<int>();
		m_spawnOffsets = new List<int>();
		m_spawnTimes = new List<int>();
		int num = ((BinaryReader)(object)reader).ReadInt32();
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			m_unitIDs.Add(((BinaryReader)(object)reader).ReadInt32());
			m_spawnOffsets.Add(((BinaryReader)(object)reader).ReadInt32());
			m_rows.Add(((BinaryReader)(object)reader).ReadInt32());
			num2 += m_spawnOffsets.Last();
			m_spawnTimes.Add(num2);
		}
	}
}
