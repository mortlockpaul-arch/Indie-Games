using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ImportData;

public class SkeletonImportData
{
	private JointImportData root;

	private int numJoints;

	public SkeletonImportData(ContentReader input)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		((BinaryReader)(object)input).ReadInt32();
		int id = ((BinaryReader)(object)input).ReadInt32();
		Vector2 pos = input.ReadVector2();
		Vector2 offset = input.ReadVector2();
		Vector2 scale = input.ReadVector2();
		root = new JointImportData(id, pos, offset, scale);
		root.ReadData(input, ((BinaryReader)(object)input).ReadInt32(), 0);
	}

	public SkeletonImportData(XmlReader reader)
	{
		numJoints = 0;
		Read(reader);
	}

	public void Read(XmlReader reader)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		reader.ReadStartElement("root");
		if (reader.Name.Equals("joint"))
		{
			if (reader.IsStartElement())
			{
				reader.ReadStartElement("joint");
			}
			int id = int.Parse(reader.ReadElementString("id"));
			Vector2 pos = default(Vector2);
			((Vector2)(ref pos))._002Ector(float.Parse(reader.ReadElementString("posX")), float.Parse(reader.ReadElementString("posY")));
			Vector2 offset = default(Vector2);
			((Vector2)(ref offset))._002Ector(float.Parse(reader.ReadElementString("boxOffsetX")), float.Parse(reader.ReadElementString("boxOffsetY")));
			Vector2 scale = default(Vector2);
			((Vector2)(ref scale))._002Ector(float.Parse(reader.ReadElementString("boxSizeX")), float.Parse(reader.ReadElementString("boxSizeY")));
			root = new JointImportData(id, pos, offset, scale);
			numJoints = root.ReadChildJoints(reader) + 1;
			reader.ReadEndElement();
		}
	}

	public JointImportData GetRoot()
	{
		return root;
	}
}
