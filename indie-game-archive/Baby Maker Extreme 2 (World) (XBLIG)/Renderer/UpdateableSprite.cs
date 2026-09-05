namespace Renderer;

public abstract class UpdateableSprite : DrawableComponent
{
	public UpdateableSprite(float depth)
		: base(depth)
	{
	}

	public abstract void Update(TimeTracker gameTime);

	public abstract override void Draw(TimeTracker gameTime);

	public abstract UpdateableSprite Clone();
}
