using Microsoft.Xna.Framework.Graphics;

namespace Renderer;

public class SpritePage
{
	private Texture2D m_pTexture;

	private bool m_IsTheme;

	private string m_sName;

	public Texture2D Texture
	{
		get
		{
			return m_pTexture;
		}
		set
		{
			m_pTexture = value;
		}
	}

	public bool IsThemePage => m_IsTheme;

	public string Name => m_sName;

	public SpritePage(Texture2D texture, string texName, bool b)
	{
		m_pTexture = texture;
		m_IsTheme = b;
		m_sName = texName;
	}
}
