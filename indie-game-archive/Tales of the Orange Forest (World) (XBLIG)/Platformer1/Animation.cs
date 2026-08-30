using Microsoft.Xna.Framework.Graphics;

namespace Platformer1;

internal class Animation
{
	private Texture2D texture;

	private float frameTime;

	private bool isLooping;

	public Texture2D Texture => texture;

	public float FrameTime => frameTime;

	public bool IsLooping => isLooping;

	public int FrameCount => Texture.Width / FrameWidth;

	public int FrameWidth => Texture.Height;

	public int FrameHeight => Texture.Height;

	public Animation(Texture2D texture, float frameTime, bool isLooping)
	{
		this.texture = texture;
		this.frameTime = frameTime;
		this.isLooping = isLooping;
	}
}
