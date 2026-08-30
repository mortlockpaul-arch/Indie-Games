using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Renderer;

public class RenderSprite : DrawableComponent
{
	private SpriteImage m_pImage;

	private Vector2 m_vOrigin;

	private bool m_bRescalesDepth;

	private bool m_bBlendAdditive;

	private Vector2 m_vSecondarySurfaceScale;

	private float m_fSecondaryRotation;

	private float m_fXMirrorPos;

	private bool m_bScreenSpace;

	public bool ScreenSpace
	{
		get
		{
			return m_bScreenSpace;
		}
		set
		{
			m_bScreenSpace = value;
		}
	}

	public float DiagDistance => ((Vector2)(ref m_vSurfaceScale)).Length();

	public float MirrorPos
	{
		get
		{
			return m_fXMirrorPos;
		}
		set
		{
			m_fXMirrorPos = value;
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
		}
	}

	public Vector2 Origin
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vOrigin;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_vOrigin = value;
		}
	}

	public bool BlendAdditive
	{
		get
		{
			return m_bBlendAdditive;
		}
		set
		{
			m_bBlendAdditive = value;
		}
	}

	public Texture2D Texture => GetSpriteImage().GetSpritePage().Texture;

	public RenderSprite(SpriteImage img, Vector2 scenePosition, float depth)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(depth);
		Initialize(img, scenePosition, depth);
	}

	public void Initialize(SpriteImage img, Vector2 scenePosition, float depth)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		m_bScreenSpace = false;
		m_fXMirrorPos = -999f;
		m_vSecondarySurfaceScale = new Vector2(1f, 1f);
		m_fSecondaryRotation = 0f;
		base.Depth = depth;
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
		float num = 1f;
		_ = m_bRescalesDepth;
		m_vPosition = scenePosition * num;
		m_vOrigin = new Vector2(0f, 0f);
		m_fAlpha = 1f;
		m_cTint = Color.White;
		m_bRescalesDepth = true;
		m_bBlendAdditive = false;
	}

	public override void Draw(TimeTracker gameTime)
	{
		SceneRenderer.DrawRenderSprite(this);
	}

	public SpriteImage GetSpriteImage()
	{
		return m_pImage;
	}

	public void SetSecondarySurfaceScale(float angle, Vector2 scale)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		m_fSecondaryRotation = angle;
		m_vSecondarySurfaceScale = scale;
	}
}
