using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;

namespace MapEdit.glows;

public class Glow
{
	private Vector2 loc;

	private float r;

	private float g;

	private float b;

	private float size;

	internal void Init(Vector2 loc, float r, float g, float b, float size)
	{
		this.loc = loc;
		this.r = r;
		this.g = g;
		this.b = b;
		this.size = size;
	}

	internal void Draw(Texture2D xt, int type)
	{
		SpriteTools.sprite.Draw(xt, loc, new Rectangle(576, 128, 192, 192), new Color(r, g, b, 1f), 0f, new Vector2(96f, 96f), size, SpriteEffects.None, 1f);
	}
}
