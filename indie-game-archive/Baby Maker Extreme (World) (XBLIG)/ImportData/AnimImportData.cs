using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ImportData;

public class AnimImportData
{
	private List<BoneImportData> m_lBones;

	public AnimImportData(XmlReader reader)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_lBones = new List<BoneImportData>();
		reader.ReadStartElement("root");
		Vector2 offset = default(Vector2);
		Vector2 scale = default(Vector2);
		while (reader.Name.Equals("joint"))
		{
			m_lBones.Add(new BoneImportData());
			List<BoneStateImportData> jointStates = m_lBones.Last().GetJointStates();
			jointStates.Clear();
			reader.ReadStartElement("joint");
			while (!reader.Name.Equals("joint"))
			{
				int time = int.Parse(reader.ReadElementString("time"));
				float rot = float.Parse(reader.ReadElementString("rotation"));
				((Vector2)(ref offset))._002Ector(float.Parse(reader.ReadElementString("offsetX")), float.Parse(reader.ReadElementString("offsetY")));
				((Vector2)(ref scale))._002Ector(float.Parse(reader.ReadElementString("scaleX")), float.Parse(reader.ReadElementString("scaleY")));
				jointStates.Add(new BoneStateImportData(time, rot, scale, offset));
			}
			reader.ReadEndElement();
		}
	}

	public AnimImportData(ContentReader input)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_lBones = new List<BoneImportData>();
		int num = ((BinaryReader)(object)input).ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int num2 = ((BinaryReader)(object)input).ReadInt32();
			m_lBones.Add(new BoneImportData());
			List<BoneStateImportData> jointStates = m_lBones.Last().GetJointStates();
			for (int j = 0; j < num2; j++)
			{
				int time = ((BinaryReader)(object)input).ReadInt32();
				float rot = ((BinaryReader)(object)input).ReadSingle();
				Vector2 offset = input.ReadVector2();
				Vector2 scale = input.ReadVector2();
				jointStates.Add(new BoneStateImportData(time, rot, scale, offset));
			}
		}
	}

	public List<BoneImportData> GetBoneData()
	{
		return m_lBones;
	}
}
