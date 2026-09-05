using MathTools;
using Microsoft.Xna.Framework;

namespace Renderer;

public class SpriteInstance : UpdateableSprite
{
	private SpriteImage m_pImage;

	private Vector2 m_vSurfaceScale;

	private float m_fRotation;

	private Vector2 m_vPosition;

	private float m_fAlpha;

	private Color m_cTint;

	private VertexPositionColoredNBTTextured[] m_vCornerPoints;

	private Vector2 m_vOrigin;

	private bool m_bUpdateCornerPos;

	private bool m_bUpdateColors;

	private bool m_bUpdateRotations;

	private bool m_bFlatColor;

	private bool m_bShadows;

	private bool m_bAdd;

	public bool FlatColor
	{
		get
		{
			return m_bFlatColor;
		}
		set
		{
			m_bFlatColor = value;
		}
	}

	public bool Additive
	{
		get
		{
			return m_bAdd;
		}
		set
		{
			m_bAdd = value;
		}
	}

	public bool Shadowed
	{
		get
		{
			return m_bShadows;
		}
		set
		{
			m_bShadows = value;
		}
	}

	public Color Color
	{
		get
		{
			return m_cTint;
		}
		set
		{
			m_cTint = value;
			m_bUpdateColors = true;
		}
	}

	public float Alpha
	{
		get
		{
			return m_fAlpha;
		}
		set
		{
			m_fAlpha = value;
			if (m_fAlpha < 0f)
			{
				m_fAlpha = 0f;
			}
			if (m_fAlpha > 1f)
			{
				m_fAlpha = 1f;
			}
			m_bUpdateColors = true;
		}
	}

	public float WidthScale
	{
		get
		{
			return m_vSurfaceScale.X;
		}
		set
		{
			float num = m_vSurfaceScale.Y / m_vSurfaceScale.X;
			m_vSurfaceScale.X = value;
			m_vSurfaceScale.Y = value * num;
			m_bUpdateCornerPos = true;
		}
	}

	public Vector2 Position
	{
		get
		{
			return m_vPosition;
		}
		set
		{
			m_vPosition = value;
			m_bUpdateCornerPos = true;
		}
	}

	public Vector2 Origin
	{
		get
		{
			return m_vOrigin;
		}
		set
		{
			m_vOrigin = value;
			m_bUpdateCornerPos = true;
		}
	}

	public float Rotation
	{
		get
		{
			return m_fRotation;
		}
		set
		{
			m_fRotation = value;
			m_bUpdateCornerPos = true;
			m_bUpdateRotations = true;
		}
	}

	public Vector2 SurfaceScale
	{
		get
		{
			return m_vSurfaceScale;
		}
		set
		{
			m_vSurfaceScale = value;
			m_bUpdateCornerPos = true;
		}
	}

	public SpriteInstance(SpriteImage img, Vector2 scenePosition, float depth)
		: base(depth)
	{
		m_cTint = Color.White;
		m_vCornerPoints = new VertexPositionColoredNBTTextured[4];
		for (int i = 0; i < 4; i++)
		{
			ref VertexPositionColoredNBTTextured reference = ref m_vCornerPoints[i];
			reference = new VertexPositionColoredNBTTextured(default(Vector3), m_cTint, new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f), new Vector3(1f, 0f, 0f), new Vector2(0f, 1f));
		}
		Initialize(img, scenePosition, depth);
	}

	public void Initialize(SpriteImage img, Vector2 scenePosition, float depth)
	{
		m_fDepth = depth;
		m_pImage = img;
		if (img != null)
		{
			m_vSurfaceScale = new Vector2(img.Width, img.Height);
		}
		else
		{
			m_vSurfaceScale = default(Vector2);
		}
		m_fRotation = 0f;
		m_vPosition = scenePosition;
		m_vOrigin = new Vector2(0f, 0f);
		m_fAlpha = 1f;
		if (img != null)
		{
			Rectangle pageRect = img.GetPageRect();
			float num = img.GetSpritePage().DiffuseTex.Width;
			float num2 = img.GetSpritePage().DiffuseTex.Height;
			m_vCornerPoints[0].TexCoord = new Vector2((float)pageRect.Left / num, (float)pageRect.Bottom / num2);
			m_vCornerPoints[1].TexCoord = new Vector2((float)pageRect.Right / num, (float)img.GetPageRect().Bottom / num2);
			m_vCornerPoints[2].TexCoord = new Vector2((float)pageRect.Left / num, (float)pageRect.Top / num2);
			m_vCornerPoints[3].TexCoord = new Vector2((float)pageRect.Right / num, (float)pageRect.Top / num2);
		}
		else
		{
			m_vCornerPoints[0].TexCoord = new Vector2(0f, 1f);
			m_vCornerPoints[1].TexCoord = new Vector2(1f, 1f);
			m_vCornerPoints[2].TexCoord = new Vector2(0f, 0f);
			m_vCornerPoints[3].TexCoord = new Vector2(1f, 0f);
		}
		UpdateCornerPoints();
		m_bUpdateCornerPos = false;
		m_bUpdateColors = false;
		m_bUpdateRotations = false;
		m_bFlatColor = false;
		m_bShadows = true;
		m_bAdd = false;
	}

	public void RecalcTexCoordinates()
	{
		if (m_pImage != null)
		{
			Rectangle pageRect = m_pImage.GetPageRect();
			float num = m_pImage.GetSpritePage().DiffuseTex.Width;
			float num2 = m_pImage.GetSpritePage().DiffuseTex.Height;
			m_vCornerPoints[0].TexCoord = new Vector2((float)pageRect.Left / num, (float)pageRect.Bottom / num2);
			m_vCornerPoints[1].TexCoord = new Vector2((float)pageRect.Right / num, (float)pageRect.Bottom / num2);
			m_vCornerPoints[2].TexCoord = new Vector2((float)pageRect.Left / num, (float)pageRect.Top / num2);
			m_vCornerPoints[3].TexCoord = new Vector2((float)pageRect.Right / num, (float)pageRect.Top / num2);
		}
		else
		{
			m_vCornerPoints[0].TexCoord = new Vector2(0f, 1f);
			m_vCornerPoints[1].TexCoord = new Vector2(1f, 1f);
			m_vCornerPoints[2].TexCoord = new Vector2(0f, 0f);
			m_vCornerPoints[3].TexCoord = new Vector2(1f, 0f);
		}
	}

	public SpriteImage GetSpriteImage()
	{
		return m_pImage;
	}

	private void UpdateCornerPoints()
	{
		m_bUpdateCornerPos = false;
		Vector2 vector = new Vector2(m_vPosition.X, 0f - m_vPosition.Y);
		Vector2 vector2 = new Vector2(m_vOrigin.X, 0f - m_vOrigin.Y);
		Vector2 vector3 = new Vector2(m_vSurfaceScale.X, m_vSurfaceScale.Y);
		Vector2 vector4 = vector + VectorTools.Rotate(-vector2 - vector3 / 2f, 0f - m_fRotation);
		m_vCornerPoints[0].Position = new Vector3(vector4.X, vector4.Y, 0f);
		vector4 = vector + VectorTools.Rotate(-vector2 + new Vector2(vector3.X / 2f, (0f - vector3.Y) / 2f), 0f - m_fRotation);
		m_vCornerPoints[1].Position = new Vector3(vector4.X, vector4.Y, 0f);
		vector4 = vector + VectorTools.Rotate(-vector2 + new Vector2((0f - vector3.X) / 2f, vector3.Y / 2f), 0f - m_fRotation);
		m_vCornerPoints[2].Position = new Vector3(vector4.X, vector4.Y, 0f);
		vector4 = vector + VectorTools.Rotate(-vector2 + new Vector2(vector3.X / 2f, vector3.Y / 2f), 0f - m_fRotation);
		m_vCornerPoints[3].Position = new Vector3(vector4.X, vector4.Y, 0f);
	}

	private void UpdatePointRotations()
	{
		m_bUpdateRotations = false;
		Matrix matrix = Matrix.CreateRotationZ(0f - m_fRotation);
		m_vCornerPoints[0].Binormal = matrix.Up;
		m_vCornerPoints[0].Tangent = matrix.Right;
		for (int i = 1; i < 4; i++)
		{
			m_vCornerPoints[i].Binormal = m_vCornerPoints[0].Binormal;
			m_vCornerPoints[i].Tangent = m_vCornerPoints[0].Tangent;
		}
	}

	private void UpdateColors()
	{
		m_bUpdateColors = false;
		Color cTint = m_cTint;
		cTint.A = (byte)(m_fAlpha * 255f);
		for (int i = 0; i < 4; i++)
		{
			m_vCornerPoints[i].Color = cTint;
		}
	}

	public VertexPositionColoredNBTTextured[] GetCornerPoints()
	{
		return m_vCornerPoints;
	}

	public override void Draw(TimeTracker gameTime)
	{
		if (m_fAlpha > 0f)
		{
			if (m_bUpdateColors)
			{
				UpdateColors();
			}
			if (m_bUpdateCornerPos)
			{
				UpdateCornerPoints();
			}
			if (m_bUpdateRotations)
			{
				UpdatePointRotations();
			}
			SceneRenderer.AddSpriteToDraw(this);
		}
	}

	public override void Update(TimeTracker gameTime)
	{
	}

	public override UpdateableSprite Clone()
	{
		SpriteInstance spriteInstance = new SpriteInstance(m_pImage, m_vPosition, m_fDepth);
		spriteInstance.Origin = Origin;
		spriteInstance.Alpha = Alpha;
		return spriteInstance;
	}

	public bool OnScreen(ref Rectangle r)
	{
		int num = (int)m_vCornerPoints[0].Position.X;
		int num2 = (int)m_vCornerPoints[0].Position.X;
		int num3 = (int)m_vCornerPoints[0].Position.Y;
		int num4 = (int)m_vCornerPoints[0].Position.Y;
		for (int i = 0; i < m_vCornerPoints.Length; i++)
		{
			if ((float)num > m_vCornerPoints[i].Position.X)
			{
				num = (int)m_vCornerPoints[i].Position.X;
			}
			if ((float)num2 < m_vCornerPoints[i].Position.X)
			{
				num2 = (int)m_vCornerPoints[i].Position.X;
			}
			if ((float)num3 > m_vCornerPoints[i].Position.Y)
			{
				num3 = (int)m_vCornerPoints[i].Position.Y;
			}
			if ((float)num4 < m_vCornerPoints[i].Position.Y)
			{
				num4 = (int)m_vCornerPoints[i].Position.Y;
			}
		}
		Rectangle value = new Rectangle(num, num3, num2 - num, num4 - num3);
		return r.Intersects(value);
	}
}
