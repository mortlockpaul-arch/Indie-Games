using Microsoft.Xna.Framework.Graphics;

namespace SGSCore;

public class SGSFontContent : SGSContent
{
	public SpriteFont m_font;

	public SGSFontContent(string path)
		: base(path)
	{
	}

	public override void Clear()
	{
		m_font = null;
	}
}
