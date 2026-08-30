using System.Xml;
using ImportData;
using Microsoft.Xna.Framework;

namespace Cutscene;

public class CutsceneCameraAction
{
	private int m_iTime;

	private CutsceneProp m_charTarget;

	private Vector2 m_vNonCharTarget;

	private float m_fZoomLevel;

	public int Time => m_iTime;

	public Vector2 Position
	{
		get
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (m_charTarget != null)
			{
				return m_charTarget.GetCharacter().WorldPosition + m_vNonCharTarget;
			}
			return m_vNonCharTarget;
		}
	}

	public float Zoom => m_fZoomLevel;

	public CutsceneCameraAction(int time, CutsceneProp target, Vector2 offset, float zoom)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_iTime = time;
		m_charTarget = target;
		m_vNonCharTarget = offset;
		m_fZoomLevel = zoom;
	}

	public CutsceneCameraAction(CamActionImportData data, Cutscene scene)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_iTime = data.Time;
		int charIndex = data.CharIndex;
		if (charIndex >= 0)
		{
			m_charTarget = scene.GetProps()[charIndex];
		}
		else
		{
			m_charTarget = null;
		}
		m_vNonCharTarget = data.NonCharTarget;
		m_fZoomLevel = data.Zoom;
	}

	public void SaveCameraAction(XmlWriter writer, Cutscene scene)
	{
		writer.WriteStartElement("cameraAction");
		writer.WriteElementString("time", m_iTime.ToString());
		int num = -1;
		if (m_charTarget != null)
		{
			num = scene.GetProps().IndexOf(m_charTarget);
		}
		writer.WriteElementString("charIndex", num.ToString());
		writer.WriteElementString("offsetX", m_vNonCharTarget.X.ToString());
		writer.WriteElementString("offsetY", m_vNonCharTarget.Y.ToString());
		writer.WriteElementString("zoom", m_fZoomLevel.ToString());
		writer.WriteEndElement();
	}
}
