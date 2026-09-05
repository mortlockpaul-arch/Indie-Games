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
		m_pSpritePage = page;
		m_rPageCoords = coords;
	}

	public Rectangle GetPageRect()
	{
		return m_rPageCoords;
	}

	public SpritePage GetSpritePage()
	{
		return m_pSpritePage;
	}

	public void SetSpritePage(SpritePage page)
	{
		m_pSpritePage = page;
	}
}
