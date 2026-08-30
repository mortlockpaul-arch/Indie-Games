using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ImportData;

public class PropActionImportData
{
	private int m_iTime;

	private int m_anim;

	private Vector2 m_vPosWorld;

	private float m_fRotationWorld;

	private Vector2 m_vLookAt;

	private int m_iMouthPose;

	private float m_fDepth;

	private float m_fScale;

	private float m_fAnimSpeed;

	private bool m_bMirror;

	public int Time => m_iTime;

	public int Anim => m_anim;

	public Vector2 Position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vPosWorld;
		}
	}

	public float Rotation => m_fRotationWorld;

	public Vector2 LookAt
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vLookAt;
		}
	}

	public int Mouth => m_iMouthPose;

	public float Depth => m_fDepth;

	public float Scale => m_fScale;

	public float AnimSpeed => m_fAnimSpeed;

	public bool Mirror => m_bMirror;

	public PropActionImportData(XmlReader reader)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		reader.ReadStartElement("action");
		m_iTime = int.Parse(reader.ReadElementString("time"));
		m_anim = int.Parse(reader.ReadElementString("anim"));
		m_vPosWorld = new Vector2(float.Parse(reader.ReadElementString("posX")), float.Parse(reader.ReadElementString("posY")));
		m_fRotationWorld = float.Parse(reader.ReadElementString("rotation"));
		m_vLookAt = new Vector2(float.Parse(reader.ReadElementString("lookX")), float.Parse(reader.ReadElementString("lookY")));
		m_iMouthPose = int.Parse(reader.ReadElementString("mouth"));
		m_fDepth = float.Parse(reader.ReadElementString("depth"));
		m_fScale = float.Parse(reader.ReadElementString("scale"));
		m_fAnimSpeed = float.Parse(reader.ReadElementString("animSpeed"));
		m_bMirror = bool.Parse(reader.ReadElementString("mirror"));
		reader.ReadEndElement();
	}

	public PropActionImportData(ContentReader input)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_iTime = ((BinaryReader)(object)input).ReadInt32();
		m_anim = ((BinaryReader)(object)input).ReadInt32();
		m_vPosWorld = input.ReadVector2();
		m_fRotationWorld = ((BinaryReader)(object)input).ReadSingle();
		m_vLookAt = input.ReadVector2();
		m_iMouthPose = ((BinaryReader)(object)input).ReadInt32();
		m_fDepth = ((BinaryReader)(object)input).ReadSingle();
		m_fScale = ((BinaryReader)(object)input).ReadSingle();
		m_fAnimSpeed = ((BinaryReader)(object)input).ReadSingle();
		m_bMirror = ((BinaryReader)(object)input).ReadBoolean();
	}
}
