using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ImportData;

public class JointImportData
{
	private int m_iId;

	private Vector2 m_vPosition;

	private List<JointImportData> m_lChildren;

	private Vector2 m_vOffset;

	private Vector2 m_vScale;

	public int Id => m_iId;

	public Vector2 Position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vPosition;
		}
	}

	public Vector2 Offset
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vOffset;
		}
	}

	public Vector2 Scale
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vScale;
		}
	}

	public List<JointImportData> Children => m_lChildren;

	public JointImportData(int id, Vector2 pos, Vector2 offset, Vector2 scale)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_iId = id;
		m_vPosition = pos;
		m_vOffset = offset;
		m_vScale = scale;
		m_lChildren = new List<JointImportData>();
	}

	public int ReadChildJoints(XmlReader reader)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		JointImportData jointImportData = null;
		Vector2 pos = default(Vector2);
		Vector2 offset = default(Vector2);
		Vector2 scale = default(Vector2);
		while (reader.IsStartElement() && reader.Name.Equals("joint"))
		{
			reader.ReadStartElement("joint");
			int id = int.Parse(reader.ReadElementString("id"));
			((Vector2)(ref pos))._002Ector(float.Parse(reader.ReadElementString("posX")), float.Parse(reader.ReadElementString("posY")));
			((Vector2)(ref offset))._002Ector(float.Parse(reader.ReadElementString("boxOffsetX")), float.Parse(reader.ReadElementString("boxOffsetY")));
			((Vector2)(ref scale))._002Ector(float.Parse(reader.ReadElementString("boxSizeX")), float.Parse(reader.ReadElementString("boxSizeY")));
			jointImportData = new JointImportData(id, pos, offset, scale);
			m_lChildren.Add(jointImportData);
			num++;
			if (reader.IsStartElement())
			{
				num += jointImportData.ReadChildJoints(reader);
			}
			reader.ReadEndElement();
		}
		return num;
	}

	public int ReadData(ContentReader input, int nextLevel, int level)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		while (nextLevel > level)
		{
			int id = ((BinaryReader)(object)input).ReadInt32();
			Vector2 pos = input.ReadVector2();
			Vector2 offset = input.ReadVector2();
			Vector2 scale = input.ReadVector2();
			JointImportData jointImportData = new JointImportData(id, pos, offset, scale);
			Children.Add(jointImportData);
			nextLevel = ((BinaryReader)(object)input).ReadInt32();
			if (nextLevel > level + 1)
			{
				nextLevel = jointImportData.ReadData(input, nextLevel, level + 1);
			}
		}
		return nextLevel;
	}
}
