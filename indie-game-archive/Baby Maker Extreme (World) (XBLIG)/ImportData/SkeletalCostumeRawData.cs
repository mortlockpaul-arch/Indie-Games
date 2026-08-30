using Microsoft.Xna.Framework;

namespace ImportData;

public class SkeletalCostumeRawData
{
	private int m_iJointId;

	private string m_sPageName;

	private Rectangle m_rBounding;

	private float m_fDepth;

	private Vector2 m_vScale;

	private float m_fRotation;

	private Vector2 m_vOffset;

	private int m_iType;

	public int Id => m_iJointId;

	public string PageName => m_sPageName;

	public Rectangle Bounding
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_rBounding;
		}
	}

	public float Depth => m_fDepth;

	public Vector2 Scale
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vScale;
		}
	}

	public float Rotation => m_fRotation;

	public Vector2 Offset
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vOffset;
		}
	}

	public int Type => m_iType;

	public SkeletalCostumeRawData(int id, string page, Rectangle bound, float depth, Vector2 scale, float rotation, Vector2 offset, int type)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_iJointId = id;
		m_sPageName = page;
		m_rBounding = bound;
		m_fDepth = depth;
		m_vScale = scale;
		m_fRotation = rotation;
		m_vOffset = offset;
		m_iType = type;
	}
}
