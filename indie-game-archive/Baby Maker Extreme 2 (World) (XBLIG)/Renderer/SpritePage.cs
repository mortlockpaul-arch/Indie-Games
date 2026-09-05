using Microsoft.Xna.Framework.Graphics;

namespace Renderer;

public class SpritePage
{
	private Texture2D m_pTexture;

	private Texture2D m_SpecTex;

	private Texture2D m_NormTex;

	private string m_sName;

	public Texture2D DiffuseTex
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

	public Texture2D SpecTex
	{
		get
		{
			return m_SpecTex;
		}
		set
		{
			m_SpecTex = value;
		}
	}

	public Texture2D NormTex
	{
		get
		{
			return m_NormTex;
		}
		set
		{
			m_NormTex = value;
		}
	}

	public string Name => m_sName;

	public SpritePage(Texture2D texture, string texName)
	{
		m_pTexture = texture;
		m_sName = texName;
	}
}
