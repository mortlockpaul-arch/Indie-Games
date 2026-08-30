using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace ImportData;

public class CostumeImportData
{
	private List<SkeletalCostumeRawData> m_lElements;

	public int SpriteCount => m_lElements.Count;

	public CostumeImportData()
	{
		m_lElements = new List<SkeletalCostumeRawData>();
	}

	public void Add(SkeletalCostumeRawData data)
	{
		m_lElements.Add(data);
	}

	public List<SkeletalCostumeRawData> GetRawData()
	{
		return m_lElements;
	}

	public CostumeImportData(XmlReader reader)
	{
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		reader.ReadStartElement("root");
		m_lElements = new List<SkeletalCostumeRawData>();
		Rectangle bound = default(Rectangle);
		Vector2 scale = default(Vector2);
		Vector2 offset = default(Vector2);
		while (reader.Name != "root")
		{
			reader.ReadStartElement("joint");
			int id = int.Parse(reader.ReadElementString("jId"));
			string page = reader.ReadElementString("pageName");
			((Rectangle)(ref bound))._002Ector(int.Parse(reader.ReadElementString("boundingX")), int.Parse(reader.ReadElementString("boundingY")), int.Parse(reader.ReadElementString("boundingW")), int.Parse(reader.ReadElementString("boundingH")));
			float depth = float.Parse(reader.ReadElementString("depth"));
			((Vector2)(ref scale))._002Ector(float.Parse(reader.ReadElementString("scaleX")), float.Parse(reader.ReadElementString("scaleY")));
			float rotation = float.Parse(reader.ReadElementString("rotation"));
			((Vector2)(ref offset))._002Ector(float.Parse(reader.ReadElementString("offsetX")), float.Parse(reader.ReadElementString("offsetY")));
			int type = int.Parse(reader.ReadElementString("type"));
			reader.ReadEndElement();
			m_lElements.Add(new SkeletalCostumeRawData(id, page, bound, depth, scale, rotation, offset, type));
		}
	}
}
