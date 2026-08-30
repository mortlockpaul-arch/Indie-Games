using System.Xml;
using ImportData;
using Microsoft.Xna.Framework;

namespace Cutscene;

public class CutsceneAction
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

	public int MouthPose => m_iMouthPose;

	public float Depth => m_fDepth;

	public float Scale => m_fScale;

	public float AnimSpeed => m_fAnimSpeed;

	public bool Mirror => m_bMirror;

	public CutsceneAction(int time, int anim, Vector2 pos, float rotation, Vector2 lookAt, int mouth, float depth, float scale, float animSpeed, bool mirror)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_iTime = time;
		m_anim = anim;
		m_vPosWorld = pos;
		m_fRotationWorld = rotation;
		m_vLookAt = lookAt;
		m_iMouthPose = mouth;
		m_fDepth = depth;
		m_fScale = scale;
		m_fAnimSpeed = animSpeed;
		m_bMirror = mirror;
	}

	public CutsceneAction(PropActionImportData data)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_iTime = data.Time;
		m_anim = data.Anim;
		m_vPosWorld = data.Position;
		m_fRotationWorld = data.Rotation;
		m_vLookAt = data.LookAt;
		m_iMouthPose = data.Mouth;
		m_fDepth = data.Depth;
		m_fScale = data.Scale;
		m_fAnimSpeed = data.AnimSpeed;
		m_bMirror = data.Mirror;
	}

	public void SaveAction(XmlWriter writer)
	{
		writer.WriteStartElement("action");
		writer.WriteElementString("time", m_iTime.ToString());
		writer.WriteElementString("anim", m_anim.ToString());
		writer.WriteElementString("posX", m_vPosWorld.X.ToString());
		writer.WriteElementString("posY", m_vPosWorld.Y.ToString());
		writer.WriteElementString("rotation", m_fRotationWorld.ToString());
		writer.WriteElementString("lookX", m_vLookAt.X.ToString());
		writer.WriteElementString("lookY", m_vLookAt.Y.ToString());
		writer.WriteElementString("mouth", m_iMouthPose.ToString());
		writer.WriteElementString("depth", m_fDepth.ToString());
		writer.WriteElementString("scale", m_fScale.ToString());
		writer.WriteElementString("animSpeed", m_fAnimSpeed.ToString());
		writer.WriteElementString("mirror", m_bMirror.ToString());
		writer.WriteEndElement();
	}
}
