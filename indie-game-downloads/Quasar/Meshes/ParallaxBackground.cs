using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quasar.Meshes;

public struct ParallaxBackground
{
	public Texture2D texture;

	public Vector2 scale;

	public Vector2 offset;

	public Vector2 minUV;

	public Vector2 maxUV;

	public Vector2 speed;

	public ParallaxBackground(Texture2D texture)
		: this(texture, Vector2.One, Vector2.Zero, new Vector2(float.MinValue, float.MinValue), new Vector2(float.MaxValue, float.MaxValue), Vector2.Zero)
	{
	}

	public ParallaxBackground(Texture2D texture, Vector2 scale, Vector2 offset, Vector2 minUV, Vector2 maxUV, Vector2 speed)
	{
		this.texture = texture;
		this.scale = scale;
		this.offset = offset;
		this.minUV = minUV;
		this.maxUV = maxUV;
		this.speed = speed;
	}
}
