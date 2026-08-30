using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Particles
{
	public Vector2 position;

	public Vector2 positionEnd;

	private float angle;

	public float rotation;

	private float size;

	private float currentSize;

	private float speed;

	public Color color;

	public Texture2D texture;

	public void Initialize(Vector2 position, Vector2 positionEnd, Texture2D texture, float speed, float iniSize, float size, float angle, Color color)
	{
		this.position = position;
		this.positionEnd = positionEnd;
		this.texture = texture;
		this.speed = speed;
		this.size = size;
		currentSize = iniSize;
		this.angle = angle;
		this.color = color;
	}

	public void LoadContent()
	{
	}

	public void Update(GameTime gameTime)
	{
		angle += rotation;
		angle = MathHelper.WrapAngle(angle);
		position.X = MathHelper.Lerp(position.X, positionEnd.X, speed);
		position.Y = MathHelper.Lerp(position.Y, positionEnd.Y, speed);
		color.R = (byte)MathHelper.Lerp((int)color.R, 0f, speed * 1.2f);
		color.G = (byte)MathHelper.Lerp((int)color.G, 0f, speed * 1.2f);
		color.B = (byte)MathHelper.Lerp((int)color.B, 0f, speed * 1.2f);
		color.A = (byte)MathHelper.Lerp((int)color.A, 0f, speed * 1.2f);
		currentSize = MathHelper.Lerp(currentSize, size, speed * 1f);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(texture, position, null, color, angle, new Vector2(texture.Width / 2, texture.Height / 2), currentSize, SpriteEffects.None, 0f);
	}
}
