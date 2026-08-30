using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

public class Intro
{
	public Texture2D tx;

	public float transp;

	public Intro(Texture2D tx)
	{
		this.tx = tx;
		transp = 0f;
	}
}
