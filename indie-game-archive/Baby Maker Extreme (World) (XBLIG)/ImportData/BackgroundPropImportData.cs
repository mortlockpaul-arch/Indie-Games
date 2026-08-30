using Microsoft.Xna.Framework;

namespace ImportData;

public class BackgroundPropImportData
{
	private string m_sPagename;

	private Rectangle m_bounding;

	private float m_fDepth;

	private float m_fRotation;

	private Vector2 m_vPosition;

	private Vector2 m_vScale;

	public string PageName => m_sPagename;

	public Rectangle Bounding
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_bounding;
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

	public Vector2 Position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vPosition;
		}
	}

	public float Rotation => m_fRotation;

	public BackgroundPropImportData(string page, Rectangle bound, float depth, Vector2 scale, float rot, Vector2 pos)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_sPagename = page;
		m_bounding = bound;
		m_fDepth = depth;
		m_vScale = scale;
		m_fRotation = rot;
		m_vPosition = pos;
	}
}
