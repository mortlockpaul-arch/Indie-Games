using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class AnimatedSprite
{
	public Animation animation;

	public Vector2 Position;

	public float angle;

	public bool Active;

	public int Width => animation.FrameWidth;

	public int Height => animation.FrameHeight;

	public void Initialize(Animation animation, Vector2 position, float angle)
	{
		this.animation = animation;
		Position = position;
		this.angle = angle;
		Active = true;
	}

	public void Update(GameTime gameTime)
	{
		animation.Position = Position;
		animation.angle = angle;
		animation.Update(gameTime);
		Active = animation.Active;
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		animation.Draw(spriteBatch);
	}
}
