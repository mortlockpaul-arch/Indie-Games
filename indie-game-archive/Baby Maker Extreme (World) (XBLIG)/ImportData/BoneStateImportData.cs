using Microsoft.Xna.Framework;

namespace ImportData;

public class BoneStateImportData
{
	private int m_iTime;

	private float m_fRotation;

	private Vector2 m_vOffset;

	private Vector2 m_vScale;

	public int Time => m_iTime;

	public float Rotation => m_fRotation;

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

	public BoneStateImportData(int time, float rot, Vector2 scale, Vector2 offset)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_iTime = time;
		m_fRotation = rot;
		m_vOffset = offset;
		m_vScale = scale;
	}
}
