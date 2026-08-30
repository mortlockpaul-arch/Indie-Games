using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class TextureWithTransition : ObjDrawUpdateWithTransition
{
	public readonly Texture2D Texture;

	public int Width => Texture.Width;

	public int Height => Texture.Height;

	public TextureWithTransition(Texture2D Texture, float transitionTimeSeconds, bool initialShownState)
		: base(transitionTimeSeconds, initialShownState, useAutoUpdate: true, useAutoDraw: false)
	{
		this.Texture = Texture;
	}
}
