using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Animation
{
	private Texture2D spriteStrip;

	public float scale;

	public float angle;

	private int elapsedTime;

	private int frameTime;

	private int frameCount;

	private int frameRows;

	private int currentFrame;

	private int currentRow;

	public Color color;

	private Rectangle sourceRect = default(Rectangle);

	private Rectangle destinationRect = default(Rectangle);

	public int FrameWidth;

	public int FrameHeight;

	public bool Active;

	public bool Looping;

	public Vector2 Position;

	public int Width => spriteStrip.Width / frameCount;

	public int Height => spriteStrip.Height / frameRows;

	public float Scale()
	{
		return (float)(Width + Height) / 2f * scale;
	}

	public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount, int frameRows, int frametime, Color color, float scale, bool looping)
	{
		if (frameCount < 1)
		{
			frameCount = 1;
		}
		if (frameRows < 1)
		{
			frameRows = 1;
		}
		this.color = color;
		FrameWidth = frameWidth;
		FrameHeight = frameHeight;
		this.frameCount = frameCount;
		this.frameRows = frameRows;
		frameTime = frametime;
		this.scale = scale / (float)frameRows;
		Looping = looping;
		Position = position;
		spriteStrip = texture;
		elapsedTime = 0;
		currentFrame = 0;
		currentRow = 0;
		Active = true;
	}

	public void Update(GameTime gameTime)
	{
		if (!Active)
		{
			return;
		}
		elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
		if (elapsedTime > frameTime)
		{
			currentFrame++;
			if (currentFrame == frameCount)
			{
				currentFrame = 0;
				currentRow++;
				if (currentRow == frameRows)
				{
					currentRow = 0;
					if (!Looping)
					{
						Active = false;
					}
				}
			}
			elapsedTime = 0;
		}
		sourceRect = new Rectangle(currentFrame * Width, currentRow * Height, Width, Height);
		destinationRect = new Rectangle((int)Position.X - (int)((float)FrameWidth * scale) / 2, (int)Position.Y - (int)((float)FrameHeight * scale) / 2, (int)((float)Width * scale), (int)((float)Height * scale));
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		if (Active)
		{
			spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color, angle, new Vector2((float)Width / -2f, (float)Height / -2f), SpriteEffects.None, 0f);
		}
	}
}
