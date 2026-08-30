using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ImportData;

public class TextImportData
{
	private int m_iActiveTime;

	private bool m_bAlignLeft;

	private bool m_bAlignBottom;

	private string m_imgName;

	private Rectangle m_rBounding;

	private string m_text;

	public int ActiveTime => m_iActiveTime;

	public bool AlignLeft => m_bAlignLeft;

	public bool AlignBottom => m_bAlignBottom;

	public string ImageFile => m_imgName;

	public Rectangle Bounding
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_rBounding;
		}
	}

	public string Text => m_text;

	public TextImportData(XmlReader reader)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		reader.ReadStartElement("textPopup");
		m_iActiveTime = int.Parse(reader.ReadElementString("activateTime"));
		m_bAlignLeft = bool.Parse(reader.ReadElementString("alignLeft"));
		m_bAlignBottom = bool.Parse(reader.ReadElementString("alignBottom"));
		m_imgName = reader.ReadElementString("imgFile");
		m_rBounding = new Rectangle(int.Parse(reader.ReadElementString("boundingX")), int.Parse(reader.ReadElementString("boundingY")), int.Parse(reader.ReadElementString("boundingW")), int.Parse(reader.ReadElementString("boundingH")));
		m_text = reader.ReadElementString("text");
		reader.ReadEndElement();
	}

	public TextImportData(ContentReader input)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_iActiveTime = ((BinaryReader)(object)input).ReadInt32();
		m_bAlignLeft = ((BinaryReader)(object)input).ReadBoolean();
		m_bAlignBottom = ((BinaryReader)(object)input).ReadBoolean();
		m_imgName = ((BinaryReader)(object)input).ReadString();
		m_rBounding = new Rectangle(((BinaryReader)(object)input).ReadInt32(), ((BinaryReader)(object)input).ReadInt32(), ((BinaryReader)(object)input).ReadInt32(), ((BinaryReader)(object)input).ReadInt32());
		m_text = ((BinaryReader)(object)input).ReadString();
	}
}
