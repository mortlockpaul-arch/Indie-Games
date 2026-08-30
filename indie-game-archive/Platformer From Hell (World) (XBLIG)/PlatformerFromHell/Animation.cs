using Microsoft.Xna.Framework.Graphics;

namespace PlatformerFromHell;

internal class Animation
{
	private Texture2D texture;

	private float frameTime;

	private bool isLooping;

	private int frameCount;

	private Texture2D hitmapTexture;

	public Texture2D Texture => texture;

	public float FrameTime => frameTime;

	public bool IsLooping => isLooping;

	public int FrameCount => frameCount;

	public int FrameWidth => Texture.Height;

	public int FrameHeight => Texture.Height;

	public Texture2D HitmapTexture => hitmapTexture;

	public Animation(Texture2D texture, Texture2D hitmapTexture, float frameTime, int frameCount, bool isLooping)
	{
		this.texture = texture;
		this.frameTime = frameTime;
		this.isLooping = isLooping;
		this.frameCount = frameCount;
		this.hitmapTexture = hitmapTexture;
	}

	public Animation(Texture2D texture, float frameTime, int frameCount, bool isLooping)
	{
		this.texture = texture;
		this.frameTime = frameTime;
		this.isLooping = isLooping;
		this.frameCount = frameCount;
		hitmapTexture = null;
	}

	public Animation(Texture2D texture, float frameTime, bool isLooping)
	{
		this.texture = texture;
		this.frameTime = frameTime;
		this.isLooping = isLooping;
		frameCount = Texture.Width / FrameWidth;
		hitmapTexture = null;
	}
}
