using Microsoft.Xna.Framework.Graphics;

namespace SGSCore;

public class SGSTextureContent : SGSContent
{
	public Texture2D m_texture;

	public SGSTextureContent(string path)
		: base(path)
	{
	}

	public override void Clear()
	{
		if (m_texture != null)
		{
			m_texture.Dispose();
			m_texture = null;
		}
	}
}
