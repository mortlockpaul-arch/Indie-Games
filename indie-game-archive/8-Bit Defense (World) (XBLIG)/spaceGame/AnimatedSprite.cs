using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace spaceGame;

public class AnimatedSprite
{
	public int currentFrame;

	public int totalFrames;

	public int spriteWidth = 8;

	public int spriteHeight = 8;

	public float scale = 1f;

	public float timer = 0f;

	public float interval = 200f;

	public Vector2 Position;

	public Rectangle Size;

	public Texture2D Texture { get; set; }

	public int Rows { get; set; }

	public int Columns { get; set; }

	public void LoadContent(ContentManager theContentManager, string theASsetName)
	{
		Texture = theContentManager.Load<Texture2D>(theASsetName);
	}

	public void SetupAnimatedSprite(int rows, int columns, int currentframe, int totalframes, float scale1)
	{
		Rows = rows;
		Columns = columns;
		totalFrames = totalframes;
		scale = scale1;
		currentFrame = currentframe;
		Size = new Rectangle(0, 0, (int)((float)(Texture.Width / Columns) * scale), (int)((float)(Texture.Height / Rows) * scale));
		int num = Texture.Width / Columns;
		int num2 = Texture.Height / Rows;
	}

	public void Update()
	{
		currentFrame++;
		if (currentFrame == totalFrames)
		{
			currentFrame = 0;
		}
	}

	public void SetFrame(int frame)
	{
		currentFrame = frame;
	}

	public virtual void Draw(SpriteBatch spriteBatch, Color theColor)
	{
		int num = Texture.Width / Columns;
		int num2 = Texture.Height / Rows;
		int num3 = (int)((float)currentFrame / (float)Columns);
		int num4 = currentFrame % Columns;
		Rectangle value = new Rectangle(num * num4, num2 * num3, num, num2);
		Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, num, num2);
		spriteBatch.Draw(destinationRectangle: new Rectangle((int)Position.X - 2, (int)Position.Y - 2, num, num2), texture: Texture, sourceRectangle: value, color: Color.Gray);
		spriteBatch.Draw(Texture, destinationRectangle, value, theColor);
	}
}
