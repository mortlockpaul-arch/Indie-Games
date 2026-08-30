using Microsoft.Xna.Framework;

namespace Renderer;

public class SpriteImage
{
	private SpritePage m_pSpritePage;

	private Rectangle m_rPageCoords;

	public int X
	{
		get
		{
			return m_rPageCoords.X;
		}
		set
		{
			m_rPageCoords.X = value;
		}
	}

	public int Y
	{
		get
		{
			return m_rPageCoords.Y;
		}
		set
		{
			m_rPageCoords.Y = value;
		}
	}

	public float Width
	{
		get
		{
			return m_rPageCoords.Width;
		}
		set
		{
			m_rPageCoords.Width = (int)value;
		}
	}

	public float Height
	{
		get
		{
			return m_rPageCoords.Height;
		}
		set
		{
			m_rPageCoords.Height = (int)value;
		}
	}

	public SpriteImage(SpritePage page, Rectangle coords)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_pSpritePage = page;
		m_rPageCoords = coords;
	}

	public Rectangle GetPageRect()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_rPageCoords;
	}

	public SpritePage GetSpritePage()
	{
		return m_pSpritePage;
	}
}
