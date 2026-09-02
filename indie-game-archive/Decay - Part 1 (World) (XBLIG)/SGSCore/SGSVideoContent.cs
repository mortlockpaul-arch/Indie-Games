using Microsoft.Xna.Framework.Media;

namespace SGSCore;

public class SGSVideoContent : SGSContent
{
	public Video m_video;

	public SGSVideoContent(string path)
		: base(path)
	{
	}

	public override void Clear()
	{
		m_video = null;
	}
}
