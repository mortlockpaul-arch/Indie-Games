using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ImportData;

public class CamActionImportData
{
	private int m_iTime;

	private int m_charIndex;

	private Vector2 m_vNonCharTarget;

	private float m_fZoomLevel;

	public int Time => m_iTime;

	public int CharIndex => m_charIndex;

	public Vector2 NonCharTarget
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vNonCharTarget;
		}
	}

	public float Zoom => m_fZoomLevel;

	public CamActionImportData(XmlReader reader)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		reader.ReadStartElement("cameraAction");
		m_iTime = int.Parse(reader.ReadElementString("time"));
		m_charIndex = int.Parse(reader.ReadElementString("charIndex"));
		m_vNonCharTarget = new Vector2(float.Parse(reader.ReadElementString("offsetX")), float.Parse(reader.ReadElementString("offsetY")));
		m_fZoomLevel = float.Parse(reader.ReadElementString("zoom"));
		reader.ReadEndElement();
	}

	public CamActionImportData(ContentReader input)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_iTime = ((BinaryReader)(object)input).ReadInt32();
		m_charIndex = ((BinaryReader)(object)input).ReadInt32();
		m_vNonCharTarget = input.ReadVector2();
		m_fZoomLevel = ((BinaryReader)(object)input).ReadSingle();
	}
}
