using Microsoft.Xna.Framework.Audio;

namespace SGSCore;

public class SGSSoundContent : SGSContent
{
	public SoundEffect m_sound;

	public SGSSoundContent(string path)
		: base(path)
	{
	}

	public override void Clear()
	{
		if (m_sound != null)
		{
			m_sound.Dispose();
			m_sound = null;
		}
	}
}
